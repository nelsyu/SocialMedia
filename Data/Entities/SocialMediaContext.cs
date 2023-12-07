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
        => optionsBuilder.UseSqlServer(DbConfig.GetConnectionString());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Friendsh__1E39AEF2142916AE");

            entity.Property(e => e.CreateTime).HasColumnType("datetime");
            entity.Property(e => e.UserId1).HasColumnName("UserID1");
            entity.Property(e => e.UserId2).HasColumnName("UserID2");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Friendships)
                .HasForeignKey(d => d.Status)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friendships_FriendshipStatus");

            entity.HasOne(d => d.UserId1Navigation).WithMany(p => p.FriendshipUserId1Navigations)
                .HasForeignKey(d => d.UserId1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friendships_userId1_Users_Id");

            entity.HasOne(d => d.UserId2Navigation).WithMany(p => p.FriendshipUserId2Navigations)
                .HasForeignKey(d => d.UserId2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friendships_userId2_Users_Id");
        });

        modelBuilder.Entity<FriendshipStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Friendsh__36257A188A11FB29");

            entity.ToTable("FriendshipStatus");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Likes__4FC592DB659F808E");

            entity.Property(e => e.EmojiSymbol).HasMaxLength(255);

            entity.HasOne(d => d.Post).WithMany(p => p.Likes)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__Likes__postId__367C1819");

            entity.HasOne(d => d.Reply).WithMany(p => p.Likes)
                .HasForeignKey(d => d.ReplyId)
                .HasConstraintName("FK__Likes__replyId__37703C52");

            entity.HasOne(d => d.User).WithMany(p => p.Likes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Likes__userId__3587F3E0");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Messages__4808B99399672F9C");

            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.CreateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__4BA5CEA942D20211");

            entity.Property(e => e.CreateTime).HasColumnType("datetime");

            entity.HasOne(d => d.ReceiverUser).WithMany(p => p.NotificationReceiverUsers)
                .HasForeignKey(d => d.ReceiverUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notifications_userId_Users_userId");

            entity.HasOne(d => d.SenderUser).WithMany(p => p.NotificationSenderUsers)
                .HasForeignKey(d => d.SenderUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notifications_sourceUserId_Users_userId");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Posts__DD0C739A52276F9A");

            entity.ToTable(tb => tb.HasTrigger("UpdatePostTrigger"));

            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.EditDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Topic).WithMany(p => p.Posts)
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Posts__topicId__76969D2E");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Posts__userId__6FE99F9F");
        });

        modelBuilder.Entity<Reply>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Replies__36BBF688FCD752D2");

            entity.Property(e => e.Content).HasMaxLength(255);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.Replies)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Replies__postId__70DDC3D8");

            entity.HasOne(d => d.User).WithMany(p => p.Replies)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Replies__userId__71D1E811");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__CD98462A3CB5777C");

            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Topics__72C15B41C5E0A7F3");

            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Topics)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Topics__userId__72C60C4A");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__CB9A1CFF36F4A4B8");

            entity.ToTable(tb => tb.HasTrigger("InsteadOfDeleteUsersTrigger"));

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
                        .HasConstraintName("FK__UserRoles__roleI__3D2915A8"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserRoles__userI__3C34F16F"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK__UserRole__7743989D46B8FC1E");
                        j.ToTable("UserRoles");
                        j.IndexerProperty<int>("UserId").HasColumnName("userId");
                        j.IndexerProperty<int>("RoleId").HasColumnName("roleId");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
