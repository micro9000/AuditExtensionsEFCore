using System;

[AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false)]
public class ManyToManyEntityAttribute : Attribute
{
  public string FirstKey {get; }
  public string SecondKey {get;}
  
  public ManyToManyEntityAttribute(string firstKey, string secondKey)
  {
    FirstKey = firstKey;
    SecondKey = secondKey;
  }
}
