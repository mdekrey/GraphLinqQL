using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace GraphQlResolver.Test
{
    public class QueryableTest
    {
        public class EntityA {
            public int Id { get; set; }
        }
        public class EntityB { 
            public int Id { get; set; }
        }
        public class EntityContext : Microsoft.EntityFrameworkCore.DbContext
        {
            public DbSet<EntityA> A { get; set; }
            public DbSet<EntityB> B { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase(nameof(QueryableTest));
                base.OnConfiguring(optionsBuilder);
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<EntityA>().HasKey(a => a.Id);
                modelBuilder.Entity<EntityB>().HasKey(b => b.Id);
                base.OnModelCreating(modelBuilder);
            }
        }

        [Fact]
        public void CanMakeAQueryable()
        {
            using (var ctx = new EntityContext())
            {
                var temp = from a in ctx.A
                           from b in ctx.B
                           select new { a, b };
                
            }
        }
    }
}
