public abstract class Entity
{
  [Key]
  public int Id {get;set;}
  
  [Required]
  public bool IsActive {get;set;}
}
