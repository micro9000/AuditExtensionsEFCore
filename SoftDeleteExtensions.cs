using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable S3011 //Reflection should no be used to increase accessibility of classes, methods, or fields
// The reflection here is to create a closed generic version of a method within the same class

public static class SoftDeleteExtensions
{
  public static void AddISoftDeleteQueryFilter(this IMutableEntityType entityData)
  {
    var methodToCall = typeof(SoftDeleteExtensions).GetMethod(nameof(GetIsActiveFilter), BindingFlags.NonPublic | BindingFlags.Static)!
      .MakeGenericMethod(entityData.ClrType);
      
      var filter = methodToCall.Invoke(null, new object[] {});
      entityData.SetQueryFilter((LambdaExpression)filter!);
  }
  
  
  public static void ApplySoftDeleteOverride(this ChangeTracker changeTracker)
  {
    foreach(var entry in changeTracker.Entries())
    {
      if (entry.Entity is Entity)
      {
        switch(entry.State)
        {
          case EntityState.Added:
            entry.CurrentValues[nameof(Entity.IsActive)] = true;
            break;
          case EntityState.Deleted:
            entry.State = EnttiyState.Modified;
            entry.CurrentValues[nameof(Entity.IsActive)] = false;
            break;
        }
      }
    }
  }
  
  
  private static LambdaExpression GetIsActiveFilter<TEntity>() where TEntity : BaseEntityClass
  {
    Expression<Func<TEntity, bool>> filter = e => e.IsActive;
    return filter;
  }
}
