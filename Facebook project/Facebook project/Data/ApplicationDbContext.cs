using System;
using System.Collections.Generic;
using System.Text;
using Facebook_project.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Facebook_project.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser,AppRole,string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet <Like> Likes { get; set; }
        public DbSet <Comment> Comments { get; set; }
        public DbSet <Friend> Friends { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Like>().HasKey(k => new { k.UserID, k.PostID });
            builder.Entity<Like>(entity => entity.HasOne(l => l.Post).WithMany(l => l.Like).OnDelete(DeleteBehavior.ClientSetNull));

            builder.Entity<Comment>().HasKey(k => new { k.UserID, k.PostID });
            builder.Entity<Comment>(entity => entity.HasOne(l => l.Post).WithMany(l => l.Comment).OnDelete(DeleteBehavior.ClientSetNull));

            builder.Entity<Friend>().HasKey(k => new { k.senderUserID, k.receiverUserID });
            builder.Entity<Friend>(entity => entity.HasOne(f => f.RecieverUser).WithMany(l => l.Friends).OnDelete(DeleteBehavior.ClientSetNull));

        }

    }
}
