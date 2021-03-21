using System;
using System.IO;
using System.Linq;

using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DictionaryTgBot
{
    class Program
    {
        private TelegramBotClient botClient;
        private ReminderContext db;
        private string tocken = "";
        private Random rand = new Random();

        private ReplyKeyboardMarkup markup;

        private Program(string tocken, bool reloadData)
        {
            db = new ReminderContext();
            
            this.tocken = tocken;

            if (reloadData) {
                db.Reminds.RemoveRange(db.Reminds);
                db.SaveChanges();
            }

            if (db.Reminds.Count() < 1)
                loadPreparedData();

            markup = new ReplyKeyboardMarkup { Keyboard = new[] { new[] { new KeyboardButton("Random") } } };
            //markup = Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup();
            botClient = new TelegramBotClient(tocken);
            //markup = new InlineKeyboardMarkup(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("Random"));
            
            Console.WriteLine("Ready to work");
            
        }

        private void loadPreparedData()
        {
            string[] fileList = { "latin.txt" };
            foreach (var fileString in fileList)
            {
                foreach (string line in File.ReadLines(fileString))
                    db.Reminds.Add(new Remind { Text = line });
                db.SaveChanges();
                Console.WriteLine("Added "+fileString);
            }
        }

        static void Main(string[] args)
        {
            bool reload = false;
            if (args.Length > 1)
                if (args[1].Equals("-r"))
                    reload = true;
            Program p = new Program(args[0], reload);
            p.botClient.OnMessage += p.OnMessRecieved;
            p.botClient.StartReceiving();
            Console.ReadLine();
            p.botClient.StopReceiving();
        }

        private async void OnMessRecieved(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message.Text.Equals("Random"))
            {
                var Req = new Telegram.Bot.Requests.SendMessageRequest(message.Chat.Id, db.Reminds.Select(p => p.Text).Skip(rand.Next(db.Reminds.Count())).First()) { ReplyMarkup = markup };
                await botClient.MakeRequestAsync(Req);
            }
            else
            {
                var sel = db.Reminds.Select(p => p.Text).Where(p => p.StartsWith(message.Text)).ToArray();
                string res = (sel.Length > 0) ? string.Join("\n\n", sel) : "Nothing found";
                var Req = new Telegram.Bot.Requests.SendMessageRequest(message.Chat.Id, res) { ReplyMarkup = markup };
                await botClient.MakeRequestAsync(Req);
            }
            //await botClient.SendTextMessageAsync(message.Chat.Id, db.Reminds.Find(rand.Next(db.Reminds.Count()) + 1).Text, replyMarkup: markup);
            
            
        }
    }
}
