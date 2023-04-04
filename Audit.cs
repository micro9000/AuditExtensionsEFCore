using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Audit")]
public class Audit
{
  [Key]
  public int Id {get;set;}
  
  public int UserId {get;set;}
  public string? User {get;set;}
  
  public DateTime TimeStamp {get;set;}
  public string? Action {get;set;}
  public string? Entity {get;set;}
  public string? Key {get;set;}
  public string? OldValues {get;set;}
  public string? NewValues {get;set;}
}
