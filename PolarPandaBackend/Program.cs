using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace PolarPandaBackend
{
    class Program
    {
        TwitchClient client;
        GameManger gameManager;
        static void Main(string[] args)
        {
           
            Program pg = new Program();
            Console.ReadLine();
           
        }
        
        public Program()
        {
            Console.WriteLine("Starting Backend");
           
            gameManager = new GameManger();
            gameManager.SetCurrentGame(0);
            ConnectionCredentials credentials = new ConnectionCredentials("polarpandagames", "oauth:t49g5o0pbc1s7qct0pxpuc1kadj9g9");
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, "polarpandagames");

            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;

            client.Connect();

        }

        private void initConnection()
        {
 
        }
        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("Successfully Joined the Main Chat");
            client.SendMessage(e.Channel, "PolarPanda Backend has started!");
            SendAdminMessage("I am Online c:");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
           
            if (e.ChatMessage.Message.StartsWith("!"))
            {
               RunCommand(e.ChatMessage.Message, e.ChatMessage.Username, e.ChatMessage.UserId);
            }
            
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            Console.WriteLine("Recieved Whisper");
            if (GetAdminList().Contains(e.WhisperMessage.UserId))
            {
                client.SendWhisper(e.WhisperMessage.Username, "Hey there Admin!");
                if (e.WhisperMessage.Message.StartsWith("!"))
                {
                    RunAdminCommand(e.WhisperMessage.Message, e.WhisperMessage.Username);
                }
            }
                
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            else
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");
        }


        private void RunCommand(string cmd, string user, string id)
        {
          
            if (cmd == "!currentgame")
            {
                string game = gameManager.GetCurrentGame();
                client.SendMessage("polarpandagames", $"@" + user + " The Current Game: " + game);


            }
            else if (cmd == "!c")
            {
                string game = gameManager.GetCurrentGame();
                client.SendMessage("polarpandagames", $"@" + user + " Current Game: " + game);


            }
        }
        private void RunAdminCommand(string cmd, string user)
        {
            if (cmd == "!start")
            {
                gameManager.SetCurrentGame(1);

                SendMessage("Admin: @" + user + " Started " + gameManager.GetCurrentGame());
             


            }
        }
        public void SendMessage(string msg)
        {
            Task.Run(async () => {
                client.SendMessage("polarpandagames", $"" + msg);
            });
        }
        public void SendAdminMessage(string msg)
        {
            // Will not work until Bot is Verified;
          foreach (string s in GetAdminList())
            {
                client.SendWhisper("duhstenlive", "Hey there Admin!");
            }
        }

        public List<string> GetAdminList()
        {
            List<string> result = new List<string>();
            using (var reader = new StringReader(File.ReadAllText(@"cfg/admins.txt")))
            {
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    result.Add(line);
                }
            }
            return result;
        }
    }

}
