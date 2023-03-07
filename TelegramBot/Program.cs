using System;
using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Linq;

namespace TelegramBot
{
    enum States
    {
        NONE,
        CREATE,
        FINISH,
        DELETE,
        OUT,
        CREATEDATE
    }
    class Program
    {
        public static string AllelementsList(Todolist list)
        {
           
            string result="";
            for (int i = 0; i < list.List.Count; i++)
            {
                result += (i + 1)+". "+ $"Дата создания:  { list.List[i].Date}\t\t|"+ "Задача:\t " + list.List[i].Task + $"|\t\tStatus: { list.List[i].Finish}" + "\n\n";
            }
            return result;
        }
        static async Task Main(string[] args)
        {
            var list = new List<string>() { };
            var statuslist = new List<string>() { };
            var datelist = new List<string>() { };
            Todolist userlist = new();
            States state = States.NONE;

            var botClient = new TelegramBotClient("6227872855:AAFhaB6lTjq1UFu-2d5nKfSVxYowvLIxNS8");
            using CancellationTokenSource cts = new();
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                
                ReplyKeyboardMarkup replyKeyboardMarkup = new
                (new[]
                {
                    new KeyboardButton[] 
                    {   "Create task", 
                        "Finish task ✅",
                        "Out list",
                        "Delete task"
                    },
                }
                )
                {
                    ResizeKeyboard = true
                };
                var message = update.Message;
                if (state == States.CREATE)
                {
                    userlist.Add(new Tasks(message.Text));
                    await botClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Задача успешно создана.\n",
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);
                    state = States.NONE;
                }
                else if(state == States.DELETE)
                {
                    bool parse = int.TryParse(update.Message.Text,out int result );
                    if(parse)
                    {
                        if(result > userlist.List.Count || result<=0)
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Задачи с таким номером не существует!\n",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        else
                        {
                            userlist.Delete(result);
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Задача успешно удалена!",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                            state = States.NONE;
                        }
                    }
                }
                else if(state == States.FINISH)
                {
                    bool parse = int.TryParse(update.Message.Text, out int result);
                    if(parse)
                    {
                        if(result > userlist.List.Count || result<0)
                        {
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Задачи с таким номером не существует!\n",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        }
                        else if(userlist.List[--result].Finish == "✅")
                        {
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Задачи с таким номером уже выполнена\n",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        }
                        else 
                        {
                            userlist.SetAccept(++result);
                            state = States.NONE;
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Некорректный ввод!\n",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        state = States.NONE;
                    }
                }
                switch (message.Text) 
                {
                    case "Create task":
                        await botClient.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: "Введите задачу:",
                        replyMarkup: replyKeyboardMarkup,
                        cancellationToken: cancellationToken);
                        state = States.CREATE;
                        break;
                    case "Out list":
                        if (userlist.List.Count == 0)
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "У вас пока нет задач!",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        else
                        {
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: AllelementsList(userlist),
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        }
                        break;
                    case "Delete task":
                        if (userlist.List.Count > 0)
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: "Выберите номер задачи:\n" + AllelementsList(userlist),
                                replyMarkup: replyKeyboardMarkup,
                                cancellationToken: cancellationToken);
                            state = States.DELETE;
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: "У вас нет ни одной задачи!",
                                replyMarkup: replyKeyboardMarkup,
                                cancellationToken: cancellationToken);
                        }
                        break;
                    case "Finish task ✅":
                        if(userlist.List.Count > 0 )
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: "Выберите задачу:\n" + AllelementsList(userlist),
                                replyMarkup: replyKeyboardMarkup,
                                cancellationToken: cancellationToken);
                            state = States.FINISH;
                        }
                        else 
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: "У вас нет ни одной задачи!",
                                replyMarkup: replyKeyboardMarkup,
                                cancellationToken: cancellationToken);
                        }
                        break;
                    default:
                        await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Выберите что хотите сделать:",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        break;
                }
            }

            Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException
                        => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                Console.WriteLine(ErrorMessage);
                return Task.CompletedTask;
            }
            var me = await botClient.GetMeAsync();
            Console.ReadLine();

            cts.Cancel();
        }

    }
}
