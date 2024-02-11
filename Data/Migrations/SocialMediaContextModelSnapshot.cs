﻿// <auto-generated />
using System;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Data.Migrations
{
    [DbContext(typeof(SocialMediaContext))]
    partial class SocialMediaContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Data.Entities.Friendship", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime");

                    b.Property<int>("FriendshipStatusId")
                        .HasColumnType("int");

                    b.Property<int>("UserId1")
                        .HasColumnType("int");

                    b.Property<int>("UserId2")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FriendshipStatusId");

                    b.HasIndex("UserId1");

                    b.HasIndex("UserId2");

                    b.ToTable("Friendship", (string)null);
                });

            modelBuilder.Entity("Data.Entities.FriendshipStatus", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("FriendshipStatus", (string)null);
                });

            modelBuilder.Entity("Data.Entities.Like", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("EmojiSymbol")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("PostId")
                        .HasColumnType("int");

                    b.Property<int?>("ReplyId")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("ReplyId");

                    b.HasIndex("UserId");

                    b.ToTable("Like", (string)null);
                });

            modelBuilder.Entity("Data.Entities.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("datetime");

                    b.Property<bool?>("IsArchived")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsRead")
                        .HasColumnType("bit");

                    b.Property<int?>("ReceiverUserId")
                        .HasColumnType("int");

                    b.Property<int?>("SenderUserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Message", (string)null);
                });

            modelBuilder.Entity("Data.Entities.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ReceiverUserId")
                        .HasColumnType("int");

                    b.Property<int>("SenderUserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ReceiverUserId");

                    b.ToTable("Notification", (string)null);
                });

            modelBuilder.Entity("Data.Entities.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("EditDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("TopicId")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TopicId");

                    b.HasIndex("UserId");

                    b.ToTable("Post", null, t =>
                        {
                            t.HasTrigger("UpdatePostTrigger");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("Data.Entities.Reply", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<int?>("PostId")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("Reply", (string)null);
                });

            modelBuilder.Entity("Data.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Role", (string)null);
                });

            modelBuilder.Entity("Data.Entities.Topic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Topic", (string)null);
                });

            modelBuilder.Entity("Data.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Totp")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("TOTP");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("User", null, t =>
                        {
                            t.HasTrigger("InsteadOfDeleteUserTrigger");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("UserRole", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRole", (string)null);
                });

            modelBuilder.Entity("Data.Entities.Friendship", b =>
                {
                    b.HasOne("Data.Entities.FriendshipStatus", "FriendshipStatus")
                        .WithMany("Friendships")
                        .HasForeignKey("FriendshipStatusId")
                        .IsRequired()
                        .HasConstraintName("FK_Friendship_StatusId_FriendshipStatus_Id");

                    b.HasOne("Data.Entities.User", "UserId1Navigation")
                        .WithMany("FriendshipUserId1Navigations")
                        .HasForeignKey("UserId1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Friendship_UserId1_User_Id");

                    b.HasOne("Data.Entities.User", "UserId2Navigation")
                        .WithMany("FriendshipUserId2Navigations")
                        .HasForeignKey("UserId2")
                        .IsRequired()
                        .HasConstraintName("FK_Friendship_UserId2_User_Id");

                    b.Navigation("FriendshipStatus");

                    b.Navigation("UserId1Navigation");

                    b.Navigation("UserId2Navigation");
                });

            modelBuilder.Entity("Data.Entities.Like", b =>
                {
                    b.HasOne("Data.Entities.Post", "Post")
                        .WithMany("Likes")
                        .HasForeignKey("PostId")
                        .HasConstraintName("FK_Like_PostId_Post_Id");

                    b.HasOne("Data.Entities.Reply", "Reply")
                        .WithMany("Likes")
                        .HasForeignKey("ReplyId")
                        .HasConstraintName("FK_Like_ReplyId_Reply_Id");

                    b.HasOne("Data.Entities.User", "User")
                        .WithMany("Likes")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_Like_UserId_User_Id");

                    b.Navigation("Post");

                    b.Navigation("Reply");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.Entities.Notification", b =>
                {
                    b.HasOne("Data.Entities.User", "ReceiverUser")
                        .WithMany("Notifications")
                        .HasForeignKey("ReceiverUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Notification_ReceiverUserId_User_Id");

                    b.Navigation("ReceiverUser");
                });

            modelBuilder.Entity("Data.Entities.Post", b =>
                {
                    b.HasOne("Data.Entities.Topic", "Topic")
                        .WithMany("Posts")
                        .HasForeignKey("TopicId")
                        .IsRequired()
                        .HasConstraintName("FK_Post_TopicId_Topic_Id");

                    b.HasOne("Data.Entities.User", "User")
                        .WithMany("Posts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("FK_Post_UserId_User_Id");

                    b.Navigation("Topic");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.Entities.Reply", b =>
                {
                    b.HasOne("Data.Entities.Post", "Post")
                        .WithMany("Replies")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_Reply_PostId_Post_Id");

                    b.HasOne("Data.Entities.User", "User")
                        .WithMany("Replies")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("FK_Reply_UserId_User_Id");

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.Entities.Topic", b =>
                {
                    b.HasOne("Data.Entities.User", "User")
                        .WithMany("Topics")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_Topic_UserId_User_Id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("UserRole", b =>
                {
                    b.HasOne("Data.Entities.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .IsRequired()
                        .HasConstraintName("FK_UserRole_RoleId_Role_Id");

                    b.HasOne("Data.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_UserRole_UserId_User_Id");
                });

            modelBuilder.Entity("Data.Entities.FriendshipStatus", b =>
                {
                    b.Navigation("Friendships");
                });

            modelBuilder.Entity("Data.Entities.Post", b =>
                {
                    b.Navigation("Likes");

                    b.Navigation("Replies");
                });

            modelBuilder.Entity("Data.Entities.Reply", b =>
                {
                    b.Navigation("Likes");
                });

            modelBuilder.Entity("Data.Entities.Topic", b =>
                {
                    b.Navigation("Posts");
                });

            modelBuilder.Entity("Data.Entities.User", b =>
                {
                    b.Navigation("FriendshipUserId1Navigations");

                    b.Navigation("FriendshipUserId2Navigations");

                    b.Navigation("Likes");

                    b.Navigation("Notifications");

                    b.Navigation("Posts");

                    b.Navigation("Replies");

                    b.Navigation("Topics");
                });
#pragma warning restore 612, 618
        }
    }
}
