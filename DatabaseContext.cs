public class DatabaseContext : DbContext
{
   private readonly ICurrentUserAccessor _currentUserAccessor;
   
   public DatabaseContext (DbContextOptions options, ICurrentUserAccessor currentUserAccessor) : base(options)
   {
    _currentUserAccessor = currentUserAccessor;
   }
   
   public DbSet<Audit> AuditRecords => Set<Audit>();
   
   protected override void OnModelCreating (ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);
      
      foreach(var entityType in modelBuilder.Model.GetEntityTypes().Where(t => typeof(Entity).IsAssignableFrom(t.ClrType)))
      {
         entityType.AddISoftDeleteQueryFilter();
      }
      
      ConfigureManyToManyRelationships(modelBuilder);
   }
   
   public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
   {
      InterceptChanges();
      return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
   }
   
   public override int SaveChanges (bool acceptAllChangesOnSuccess)
   {
      InterceptChanges();
      return base.SaveChanges(acceptAllChangesOnSuccess);
   }
   
   private static void ConfigureManyToManyRelationships(ModelBuilder modelBuilder)
   {
      foreach(var type in typeof(DatabaseContext).Assembly.GetTypes())
      {
         var attribute = type.GetCustomAttributes(false).OfType<ManyToManyEntityAttribute>().SingleOrDefault();
         if (attribute == null) continue;
         
         modelBuilder
            .Entity(type)
            .HasKey(attribute.FirstKey, attribute.SecondKey);
      }
   }
   
   private void InterceptChanges()
   {
      // Orders matters here as the soft delete override changes Deleted records to Modified
      var user = _currentUserAccessor.GetCurrentUser();
      AuditRecords.AddRange(ChangeTracker.GenerateAuditRecords(user));
      ChangeTracker.ApplySoftDeleteOverride();
   }
}
