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
        OUT
    }
    class Program
    {
        public static string AllelementsList(List<string>list,List<string>statuslist)
        {
            string result="";
            for(int i =0;i<list.Count;i++)
                result = result+ (i+1)+". " + list[i] +$"\t\tStatus: {statuslist[i]}"+ '\n';
            return result;
        }
        static async Task Main(string[] args)
        {
            var list = new List<string>() { };
            var statuslist = new List<string>() { };
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
                    list.Add(message.Text);
                    statuslist.Add("❌");
                    await botClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Задача успешно создана!"+'\n'+message.Text,
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);
                    state = States.NONE;
                }
                else if(state == States.DELETE)
                {
                    bool parse = int.TryParse(update.Message.Text,out int result );
                    if(parse)
                    {
                        if(result > list.Count || result<=0)
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Задачи с таким номером не существует!\n",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        else
                        {
                            result--;
                            list.RemoveAt(result);
                            statuslist.RemoveAt(result);
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
                        if(result >list.Count || result<0)
                        {
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Задачи с таким номером не существует!\n",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        }
                        else if(statuslist[--result] == "✅")
                        {
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Задачи с таким номером уже выполнена\n",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        }
                        else 
                        {
                            statuslist[result] = "✅";
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
                        if (list.Count == 0)
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "У вас пока нет задач!",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        else
                        {
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: AllelementsList(list,statuslist),
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        }
                        break;
                    case "Delete task":
                        if (list.Count > 0)
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: "Выберите номер задачи:\n" + AllelementsList(list,statuslist),
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
                        if(list.Count > 0 )
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: "Выберите задачe:\n" + AllelementsList(list, statuslist),
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
