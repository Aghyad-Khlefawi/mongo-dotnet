using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoCorp;

public class Corporate
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public required string Id { get; set; }
  public required string Name { get; set; }
  public List<Employee> Employees { get; set; } = [];

  public static Corporate Create(string name) => new() { Id = ObjectId.GenerateNewId().ToString(), Name = name };
}

public class Employee
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public required string Id { get; set; }
  public required string FullName { get; set; }
  public static Employee Create(string name) => new() { Id = ObjectId.GenerateNewId().ToString(), FullName = name, IsActive = true };
  public bool IsActive { get; set; }
}

public class Card
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public required string Id { get; set; }
  public required string CardNumber { get; set; }
  public DateTime ExpiryDate { get; set; }

  [BsonRepresentation(BsonType.ObjectId)]
  public required string EmployeeId { get; set; }
  public required string EmployeeFullName { get; set; }

  public static Card Create(string cardNumber, DateTime expiryDate, string employeeId, string employeeFullName) => new() { Id = ObjectId.GenerateNewId().ToString(), ExpiryDate = expiryDate, EmployeeId = employeeId, EmployeeFullName = employeeFullName, CardNumber = cardNumber };
}


public record CreateCorporateRequest(string Name);
public record CreateEmployeeRequest(string FullName);
public record CreateCardRequest(string CardNumber, DateTime ExpiryDate);


