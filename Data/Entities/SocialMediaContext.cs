using System;
using System.Collections.Generic;
using Library.Config;
using Microsoft.EntityFrameworkCore;

namespace Data.Entities;

public partial class SocialMediaContext : DbContext
{
    public SocialMediaContext()
    {
    }

    public SocialMediaContext(DbContextOptions<SocialMediaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Friendship> Friendships { get; set; }

    public virtual DbSet<FriendshipStatus> FriendshipStatuses { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Reply> Replies { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => DbConfig.ConfigureDbContext(optionsBuilder);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.ToTable("Friendship");

            entity.Property(e => e.CreateTime).HasColumnType("datetime");

            entity.HasOne(d => d.FriendshipStatus).WithMany(p => p.Friendships)
                .HasForeignKey(d => d.FriendshipStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friendship_StatusId_FriendshipStatus_Id");

            entity.HasOne(d => d.UserId1Navigation).WithMany(p => p.FriendshipUserId1Navigations)
                .HasForeignKey(d => d.UserId1)
                .HasConstraintName("FK_Friendship_UserId1_User_Id");

            entity.HasOne(d => d.UserId2Navigation).WithMany(p => p.FriendshipUserId2Navigations)
                .HasForeignKey(d => d.UserId2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friendship_UserId2_User_Id");
        });

        modelBuilder.Entity<FriendshipStatus>(entity =>
        {
            entity.ToTable("FriendshipStatus");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.ToTable("Like");

            entity.Property(e => e.EmojiSymbol).HasMaxLength(255);

            entity.HasOne(d => d.Post).WithMany(p => p.Likes)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_Like_PostId_Post_Id");

            entity.HasOne(d => d.Reply).WithMany(p => p.Likes)
                .HasForeignKey(d => d.ReplyId)
                .HasConstraintName("FK_Like_ReplyId_Reply_Id");

            entity.HasOne(d => d.User).WithMany(p => p.Likes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Like_UserId_User_Id");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Message");

            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.CreateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notification");

            entity.Property(e => e.CreateTime).HasColumnType("datetime");

            entity.HasOne(d => d.ReceiverUser).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.ReceiverUserId)
                .HasConstraintName("FK_Notification_ReceiverUserId_User_Id");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToTable("Post", tb => tb.HasTrigger("UpdatePostTrigger"));

            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.EditDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Topic).WithMany(p => p.Posts)
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Post_TopicId_Topic_Id");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Post_UserId_User_Id");
        });

        modelBuilder.Entity<Reply>(entity =>
        {
            entity.ToTable("Reply");

            entity.Property(e => e.Content).HasMaxLength(255);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.Replies)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Reply_PostId_Post_Id");

            entity.HasOne(d => d.User).WithMany(p => p.Replies)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Reply_UserId_User_Id");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.ToTable("Topic");

            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Topics)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Topic_UserId_User_Id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User", tb => tb.HasTrigger("InsteadOfDeleteUserTrigger"));

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Totp).HasColumnName("TOTP");
            entity.Property(e => e.Username).HasMaxLength(255);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UserRole_RoleId_Role_Id"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UserRole_UserId_User_Id"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("UserRole");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
