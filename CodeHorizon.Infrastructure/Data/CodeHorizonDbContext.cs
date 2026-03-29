using Microsoft.EntityFrameworkCore;
using CodeHorizon.Core.Entities;

namespace CodeHorizon.Infrastructure.Data
{
    public class CodeHorizonDbContext : DbContext
    {
        public CodeHorizonDbContext(DbContextOptions<CodeHorizonDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Snippet> Snippets { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<SnippetTag> SnippetTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Snippet>().ToTable("Snippet");
            modelBuilder.Entity<Bookmark>().ToTable("Bookmark");
            modelBuilder.Entity<Tag>().ToTable("Tag");
            modelBuilder.Entity<SnippetTag>().ToTable("SnippetTag");

            // Configure SnippetTag composite key
            modelBuilder.Entity<SnippetTag>()
                .HasKey(st => new { st.SnippetId, st.TagId });

            // Configure relationships
            modelBuilder.Entity<SnippetTag>()
                .HasOne(st => st.Snippet)
                .WithMany(s => s.SnippetTags)
                .HasForeignKey(st => st.SnippetId);

            modelBuilder.Entity<SnippetTag>()
                .HasOne(st => st.Tag)
                .WithMany(t => t.SnippetTags)
                .HasForeignKey(st => st.TagId);

            modelBuilder.Entity<Bookmark>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookmarks)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bookmark>()
                .HasOne(b => b.Snippet)
                .WithMany(s => s.Bookmarks)
                .HasForeignKey(b => b.SnippetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Name)
                .IsUnique();

            // Add indexes for performance
            modelBuilder.Entity<Snippet>()
                .HasIndex(s => s.Language);

            modelBuilder.Entity<Snippet>()
                .HasIndex(s => s.CreatedAt);

            modelBuilder.Entity<Snippet>()
                .HasIndex(s => s.IsPublic);
        }
    }
}