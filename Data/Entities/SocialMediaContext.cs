using System;
using System.Collections.Generic;
using Library.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Data.Entities
{
    public partial class SocialMediaContext : DbContext
    {
        public SocialMediaContext()
        {
        }

        public SocialMediaContext(DbContextOptions<SocialMediaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Friendship> Friendships { get; set; } = null!;
        public virtual DbSet<Like> Likes { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<Reply> Replies { get; set; } = null!;
        public virtual DbSet<Topic> Topics { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer(DbConfig.GetConnectionString());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Friendship>(entity =>
            {
                entity.Property(e => e.FriendshipId).HasColumnName("friendshipId");

                entity.Property(e => e.CreatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("createdTime");

                entity.Property(e => e.Status)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.UserId1).HasColumnName("userID1");

                entity.Property(e => e.UserId2).HasColumnName("userID2");
            });

            modelBuilder.Entity<Like>(entity =>
            {
                entity.Property(e => e.LikeId).HasColumnName("likeId");

                entity.Property(e => e.EmojiSymbol)
                    .HasMaxLength(255)
                    .HasColumnName("emojiSymbol");

                entity.Property(e => e.PostId).HasColumnName("postId");

                entity.Property(e => e.ReplyId).HasColumnName("replyId");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("FK__Likes__postId__367C1819");

                entity.HasOne(d => d.Reply)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.ReplyId)
                    .HasConstraintName("FK__Likes__replyId__37703C52");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Likes__userId__3587F3E0");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.Property(e => e.MessageId).HasColumnName("messageId");

                entity.Property(e => e.Content)
                    .HasColumnType("text")
                    .HasColumnName("content");

                entity.Property(e => e.IsArchived).HasColumnName("isArchived");

                entity.Property(e => e.IsRead).HasColumnName("isRead");

                entity.Property(e => e.ReceiverId).HasColumnName("receiverId");

                entity.Property(e => e.SenderId).HasColumnName("senderId");

                entity.Property(e => e.SentTime)
                    .HasColumnType("datetime")
                    .HasColumnName("sentTime");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.Property(e => e.PostId).HasColumnName("postId");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.LastEditDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastEditDate");

                entity.Property(e => e.PostDate)
                    .HasColumnType("datetime")
                    .HasColumnName("postDate");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");

                entity.Property(e => e.TopicId).HasColumnName("topicId");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.TopicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Posts__topicId__76969D2E");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Posts__userId__6FE99F9F");
            });

            modelBuilder.Entity<Reply>(entity =>
            {
                entity.Property(e => e.ReplyId).HasColumnName("replyId");

                entity.Property(e => e.Content)
                    .HasMaxLength(255)
                    .HasColumnName("content");

                entity.Property(e => e.PostId).HasColumnName("postId");

                entity.Property(e => e.ReplyDate)
                    .HasColumnType("datetime")
                    .HasColumnName("reply_date");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Replies)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Replies__postId__70DDC3D8");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Replies)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Replies__userId__71D1E811");
            });

            modelBuilder.Entity<Topic>(entity =>
            {
                entity.Property(e => e.TopicId).HasColumnName("topicId");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Topics)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Topics__userId__72C60C4A");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.Totp).HasColumnName("TOTP");

                entity.Property(e => e.Username)
                    .HasMaxLength(255)
                    .HasColumnName("username");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
