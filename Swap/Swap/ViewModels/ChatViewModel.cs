using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Caching.Memory;
using Swap.Chat_Database;
using Swap.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Swap.ViewModels
{
    public class ChatViewModel
    {
        private HubConnection m_HubConnection = (Application.Current as App).HubConnection;
        private readonly Database m_DataBase = (Application.Current as App).DataBase;
        private readonly MemoryCache m_MemoryCache = (Application.Current as App).MemoryCache;
        public event PropertyChangedEventHandler PropertyChanged;
        private string m_Message;
        private ObservableCollection<MessageModel> m_Messages;
        private bool m_IsConnected;
        private Contact m_contact;

        public string Message
        {
            get
            {
                return m_Message;
            }
            set
            {
                m_Message = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MessageModel> Messages
        {
            get
            {
                return m_Messages;
            }
            set
            {
                m_Messages = value;
                OnPropertyChanged();
            }
        }

        public bool IsConnected
        {
            get
            {
                return m_IsConnected;
            }
            set
            {
                m_IsConnected = value;
                OnPropertyChanged();
            }
        }

        public Command SendMessageCommand { get; }

        public ChatViewModel(Contact i_contact)
        {
            m_contact = i_contact;
            int fromId = (Application.Current as App).UserId;
            int toId = i_contact.UserId;
            Messages = loadChatHistory(i_contact);
            SendMessageCommand = new Command(async () => { await sendMessage(m_contact.ChatId, toId, Message); });

            m_HubConnection.On<string, int, string>("ReceiveMessageViewModel", (fromUserName, fromUserid, body) =>
            {
               
                Messages.Add(new MessageModel() { Message = body, User = fromUserName, IsOwnMessage = fromId == fromUserid });
            });
        }

        private ObservableCollection<MessageModel> loadChatHistory(Contact i_contact)
        {
            int myId = (Application.Current as App).UserId;
            ObservableCollection<MessageModel> result = new ObservableCollection<MessageModel>();
            Database database = (Application.Current as App).DataBase;
            IEnumerable<InstantMessage> instantMessages = database.InstantMessageTable.GetLastMessages(i_contact.ChatId,30);

            foreach (InstantMessage instantMessage in instantMessages )
            {

                result.Add(new MessageModel { User = instantMessage.UserName, Message = instantMessage.Body, IsOwnMessage = instantMessage.UserId == myId });
            }

            return result;
        }

        private async Task sendMessage(string chatId, int toId, string body)
        {
            await m_HubConnection.InvokeAsync("SendMessage", chatId, (Application.Current as App).UserId/*int fromUserId*/ , toId, body);
        }

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}