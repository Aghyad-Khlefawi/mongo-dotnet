using Bogus;
using Microsoft.AspNetCore.Mvc;
using MongoCorp;
using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var mongoClient = new MongoClient("mongodb://admin:admin@localhost/");
var database = mongoClient.GetDatabase("mongocorp");


app.MapPost("/api/corporate", async ([FromBody] CreateCorporateRequest request) =>
{
});

app.MapGet("/api/corporate", async () =>
{
});

app.MapPost("/api/corporate/{corporateId}/employee", async ([FromRoute] string corporateId, [FromBody] CreateEmployeeRequest request) =>
{
});

app.MapGet("/api/corporate/{corporateId}/employee", async ([FromRoute] string corporateId) =>
{
});

app.MapDelete("/api/corporate/{corporateId}/employee/{employeeId}", async ([FromRoute] string corporateId, [FromRoute] string employeeId) =>
{
});

app.MapGet("/api/employee", async () =>
{
});

app.Run();
