using Bogus;
using Microsoft.AspNetCore.Mvc;
using MongoCorp;
using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var mongoClient = new MongoClient("mongodb://admin:admin@localhost/");
var database = mongoClient.GetDatabase("mongocorp");

if (database.GetCollection<Corporate>("corporates").CountDocuments(_=>true) == 0)
  for (int i = 0; i < 200; i++)
  {
    var corp = new Faker<Corporate>().RuleFor(e => e.Name, e => e.Company.CompanyName())
      .RuleFor(e => e.Id, ObjectId.GenerateNewId().ToString())
      .Generate();
    var cards = new List<Card>();
    for (int j = 0; j < Random.Shared.Next(2, 10); j++)
    {
      Employee item = new Faker<Employee>().RuleFor(e => e.Id, ObjectId.GenerateNewId().ToString()).RuleFor(e => e.FullName, e => e.Name.FullName()).RuleFor(e => e.IsActive, true).Generate();
      corp.Employees.Add(item);
      for (int k = 0; k < Random.Shared.Next(1, 2); k++)
      {
        cards.Add(new Faker<Card>().RuleFor(e => e.Id, ObjectId.GenerateNewId().ToString())
            .RuleFor(e => e.EmployeeId, item.Id)
            .RuleFor(e => e.EmployeeFullName, item.FullName)
            .RuleFor(e => e.CardNumber, e => e.Random.ReplaceNumbers("####-####-####-####"))
            .RuleFor(e => e.ExpiryDate, e => e.Date.Future(3))
            .Generate());
      }
    }
    database.GetCollection<Card>("cards").InsertMany(cards);
    database.GetCollection<Corporate>("corporates").InsertOne(corp);
  }

string corporatesCollection = "corporates";

app.MapPost("/api/corporate", async ([FromBody] CreateCorporateRequest request) =>
{
  var corporate = Corporate.Create(request.Name);
  await database.GetCollection<Corporate>(corporatesCollection).InsertOneAsync(corporate);
  return Results.Ok(corporate);
});

app.MapGet("/api/corporate", async () =>
{
  return Results.Ok(await database.GetCollection<Corporate>(corporatesCollection).Find(FilterDefinition<Corporate>.Empty).ToListAsync());
});

app.MapPost("/api/corporate/{corporateId}/employee", async ([FromRoute] string corporateId, [FromBody] CreateEmployeeRequest request) =>
{
  var employee = Employee.Create(request.FullName);
  if (!await database.GetCollection<Corporate>(corporatesCollection).Find(e => e.Id == corporateId).AnyAsync())
    return Results.BadRequest("Invalid corporate id");

  await database.GetCollection<Corporate>(corporatesCollection).UpdateOneAsync(e => e.Id == corporateId, Builders<Corporate>.Update.Push(e => e.Employees, employee));
  return Results.Ok(employee);
});

app.MapGet("/api/corporate/{corporateId}/employee", async ([FromRoute] string corporateId) =>
{
  var corporate = (await database.GetCollection<Corporate>(corporatesCollection).FindAsync(e => e.Id == corporateId)).FirstOrDefault();
  if (corporate == null)
    return Results.BadRequest("Invalid corporate id");
  return Results.Ok(corporate.Employees);
});

app.MapDelete("/api/corporate/{corporateId}/employee/{employeeId}", async ([FromRoute] string corporateId, [FromRoute] string employeeId) =>
{
  if (!await database.GetCollection<Corporate>(corporatesCollection).Find(e => e.Id == corporateId).AnyAsync())
    return Results.BadRequest("Invalid corporate id");

  await database.GetCollection<Corporate>(corporatesCollection).UpdateOneAsync(e => e.Id == corporateId && e.Employees.Any(e => e.Id == employeeId), Builders<Corporate>.Update.Set(e => e.Employees[-1].IsActive, false));
  return Results.Ok("Employee deleted");
});

app.MapGet("/api/employee", async () =>
{
  return Results.Ok(await database.GetCollection<Corporate>(corporatesCollection)
  .Aggregate()
  .Unwind(e => e.Employees)
  .ReplaceRoot<Employee>("$Employees")
  .Match(e => e.IsActive == true)
  .ToListAsync());
});
app.MapGet("/", () => "Hello World!");

app.Run();
