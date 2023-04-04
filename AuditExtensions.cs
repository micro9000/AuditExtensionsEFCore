using System;
using System.Collection.Generic;
using System.Linq;
using System.Text.Json;
using ProjectNamespace.To.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public static class AuditExtensions
{
  public static List<Audit> GenerateAuditRecords(this ChangeTracker changeTracker, UserContext? currentUser)
  {
    var auditRecords = new List<Audit>();
    foreach (var entry in changeTracker.Entries())
    {
      if (entity.Entity is Audit)
        continue; // Don't audit the audits. The fabric of the universe can't handle it
       
      if (entry.State == EntityState.Unchanged || entry.State == EntityState.Detached)
        continue; // Nothing to audit
        
      Dictionary<string, object> oldValues = new Dictionary<string object>();
      Dictionary<string, object> newValues = new Dictionary<string object>();
      Dictionary<string, object> keys = new Dictionary<string object>();
      
      foreach (var property in entry.Properties)
      {
        if (property.Metadata.IsPrimaryKey())
        {
          if (property.CurrentValue != null)
          {
            keys.Add(property.Metadata.Name, property.CurrentValue);
          }
          continue;
        }
        
        switch (entry.State)
        {
          case EntityState.Deleted:
            if (property.OriginalValue != null)
            {
              oldValues.Add(property.Metadata.Name, property.OriginalValue);
            }
            break;
          case EntityState.Modified:
            if (property.IsModified)
            {
              if (property.OriginalValue != null)
              {
                oldValues.Add(property.Metadata.Name, property.OriginalValue);
              }
              if (property.CurrentValue != null)
              {
                newValues.Add(property.Metadata.Name, property.CurrentValue);
              }
            }
            break;
          case EntityState.Added:
            if (property.CurrentValue != null)
            {
              newValues.Add(property.Metadata.Name, property.CurrentValue);
            }
            break;
        }
        
      }
      
      var options = JsonSerializerOptionsFactory.Create();
      
      var auditRecord = new Audit
      {
        UserId = currentUser?.UserId ?? -1,
        User = currentUser?.UserName ?? "<<UNKNOWN>>",
        TimeStamp = DateTime.UtcNow,
        Action = entry.State.ToString(),
        Entity = entry.Entity.GetType().Name,
        key = keys.Any() ? JsonSerializer.Serialize(keys, options) : null,
        OldValues = oldValues.Any() ? JsonSerializer.Serialize(oldValues, options) : null,
        NewValues = newValues.Any() ? JsonSerializer.Serialize(newValues, options) : null,
      }
      
      auditRecords.Add(auditRecord);
    }
    return auditRecords;
  }
}
