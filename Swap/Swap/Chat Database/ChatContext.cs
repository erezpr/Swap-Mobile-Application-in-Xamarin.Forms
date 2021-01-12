using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using Xamarin.Forms;

namespace Swap.Chat_Database
{
    public class ChatContext : DbContext
    {
        public DbSet<Chat> Chats { get; private set; }
        public DbSet<UserToGroup> Groups { get; private set; }
        public DbSet<InstantMessage> Messages { get; private set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SetupChatToMessagesRelationship(modelBuilder);
            SetupUserToGroupToChatRelationship(modelBuilder);
        }

        private void SetupChatToMessagesRelationship(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InstantMessage>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId);
        }

        private void SetupUserToGroupToChatRelationship(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>()
                .HasMany(c => c.UsersToGroup)
                .WithOne(g => g.Chat)
                .HasForeignKey(c => c.Guid);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string databaseName = "swApp.db";
            string databasePath = string.Empty;
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    SQLitePCL.Batteries_V2.Init();
                    databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseName); ;
                    break;
                case Device.Android:
                    databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseName);
                    break;
                default:
                    throw new NotImplementedException("Platform not supported");
            }

            // Specify that we will use sqlite and the path of the database here
            optionsBuilder.UseSqlite($"Filename={databasePath}");
        }
    }
}
