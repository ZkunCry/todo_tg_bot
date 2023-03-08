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
        CREATEDATE,
        CREATELIST,
        PRECREATE,
        PREDELETE,
        PREFINISH
    }
    class Program
    {
        public static string AllelementsList(List<Todolist> list)
        {
            string result="";
            for (int i = 0; i < list.Count; i++)
            {
                result += $"Список: {list[i].NameList}\n";
                if (list[i].List.Count == 0)
                    continue;
                for(int j =0;j<list[i].List.Count;j++)
                    result += (j + 1) + ". " + $"Дата создания:  { list[i].List[j].Date}\t\t|" + "Задача:\t " + list[i].List[j].Task 
                        + $"|\t\tStatus: { list[i].List[j].Finish}" + "\n\n";
            }
            return result;
        }
        static async Task Main(string[] args)
        {
            List<Todolist> todouserlist = new();
            int IndexList = 0;
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
                bool parse = false;
                int result = 0;
                ReplyKeyboardMarkup replyKeyboardMarkup = new
                (new[]
                {
                    new KeyboardButton[] 
                    {   "Create list",
                        "Create task", 
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
                if(state == States.CREATELIST)
                {
                    todouserlist.Add(new() 
                    { 
                        NameList = message.Text 
                    }
                    );
                    await botClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Список успешно создан!\n",
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);
                    state = States.NONE;
                }
                else if(state == States.PRECREATE)
                {
                    if(todouserlist.Count > 0 && todouserlist is not null)
                    {
                        parse = int.TryParse(update.Message.Text, out result);
                        if (parse)
                        {
                            if (result > 0 && result <= todouserlist.Count)
                            {
                                await botClient.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: "Введите задачу:",
                                replyMarkup: replyKeyboardMarkup,
                                cancellationToken: cancellationToken);
                                state = States.CREATE;
                                IndexList = result;
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(
                                 chatId: update.Message.Chat.Id,
                                text: "Списка с таким номером не существует!",
                                 replyMarkup: replyKeyboardMarkup,
                                cancellationToken: cancellationToken);
                                state = States.NONE;
                            }
                        }
                        else 
                        {
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Неверно введены данные!",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                            state = States.NONE;
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Список не создан!",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                            state = States.NONE;
                    }
                }
                else if (state == States.PREDELETE)
                {
                    if (todouserlist.Count > 0 && todouserlist is not null)
                    {
                         parse = int.TryParse(update.Message.Text, out  result);
                        int temp = result;
                        if (parse)
                        {
                            if(result > todouserlist.Count)
                            {
                                await botClient.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                               text: "Списка с таким номером не существует!",
                                replyMarkup: replyKeyboardMarkup,
                               cancellationToken: cancellationToken);
                                state = States.NONE;
                            }
                            else if (todouserlist[--temp].List.Count == 0)
                            {
                                await botClient.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: "В списке нет задач.\n",
                                replyMarkup: replyKeyboardMarkup,
                                cancellationToken: cancellationToken);
                                state = States.NONE;
                            }
                           else if (result > 0 && result <= todouserlist.Count)
                            {
                                await botClient.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: "Введите номер задачи:",
                                replyMarkup: replyKeyboardMarkup,
                                cancellationToken: cancellationToken);
                                state = States.DELETE;
                                IndexList = result;
                            }
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Неверно введены данные!",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                            state = States.NONE;
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Список не создан!",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        state = States.NONE;
                    }
                }
                else if (state == States.CREATE)
                {
                    todouserlist[--IndexList].Add(new Tasks(message.Text));
                    await botClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Задача успешно создана.\n",
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);
                    state = States.NONE;
                }
                else if(state == States.DELETE)
                {
                    parse = int.TryParse(update.Message.Text,out result );
                    int temp = IndexList;
                    if(parse)
                    {
                         if (result > todouserlist[--IndexList].List.Count || result <= 0)
                        {
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Задачи с таким номером не существует!\n",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                         
                        }
                        else
                        {
                            todouserlist[IndexList].Delete(result);
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Задача успешно удалена!",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                     
                        }
                    }
                    else 
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Неверно введены данные\n",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                    }
                    state = States.NONE;
                }
                else if(state == States.FINISH)
                {
                    parse = int.TryParse(update.Message.Text, out  result);
                    int temp = IndexList;
                    temp--;
                    if (parse)
                    {
                        if(result > todouserlist[temp].List.Count  || result<0)
                        {
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Задачи с таким номером не существует!\n",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        }
                        else if(todouserlist[temp].List[--result].Finish == "✅")
                        {
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Задачи с таким номером уже выполнена\n",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        }
                        else 
                            todouserlist[temp].SetAccept(++result);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Некорректный ввод!\n",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                    }
                    state = States.NONE;
                }
                else if (state == States.PREFINISH)
                {
                    if (todouserlist.Count > 0 && todouserlist is not null)
                    {
                        parse = int.TryParse(update.Message.Text, out  result);
                        int temp = result;
                        if (parse)
                        {
                            if (result > todouserlist.Count)
                            {
                                await botClient.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                               text: "Списка с таким номером не существует!",
                                replyMarkup: replyKeyboardMarkup,
                               cancellationToken: cancellationToken);
                                state = States.NONE;
                            }
                           else  if (todouserlist[--temp].List.Count == 0)
                            {
                                await botClient.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: "В списке нет задач.\n",
                                replyMarkup: replyKeyboardMarkup,
                                cancellationToken: cancellationToken);
                                state = States.NONE;
                            }
                            else if (result > 0 && result <= todouserlist.Count)
                            {
                                await botClient.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: "Введите номер задачи:",
                                replyMarkup: replyKeyboardMarkup,
                                cancellationToken: cancellationToken);
                                state = States.FINISH;
                                IndexList = result;
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(
                                 chatId: update.Message.Chat.Id,
                                text: "Списка с таким номером не существует!",
                                 replyMarkup: replyKeyboardMarkup,
                                cancellationToken: cancellationToken);
                                state = States.NONE;
                            }
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Неверно введены данные!",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                            state = States.NONE;
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Список не создан!",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        state = States.NONE;
                    }
                }
                switch (message.Text) 
                {
                    case "Create list":
                        await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Введите название вашего списка:",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        state = States.CREATELIST;
                        break;
                    case "Create task":
                        if (todouserlist.Count > 0)
                        {
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Введите номер списка:",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                            state = States.PRECREATE;
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Для начала нужно создать список.",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        }
                        break;
                    case "Out list":
                        if (todouserlist.Count == 0)
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Вы не создали ни одного списка!",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        else
                        {
                            await botClient.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: AllelementsList(todouserlist),
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken);
                        }
                        break;
                    case "Delete task":
                        if (todouserlist.Count  > 0)
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: "Выберите список:\n" + AllelementsList(todouserlist),
                                replyMarkup: replyKeyboardMarkup,
                                cancellationToken: cancellationToken);
                            state = States.PREDELETE;
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: "Для начала нужно создать список.",
                                replyMarkup: replyKeyboardMarkup,
                                cancellationToken: cancellationToken);
                        }
                        break;
                    case "Finish task ✅":
                        if(todouserlist.Count   > 0 )
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: "Выберите список:\n" + AllelementsList(todouserlist),
                                replyMarkup: replyKeyboardMarkup,
                                cancellationToken: cancellationToken);
                            state = States.PREFINISH;
                        }
                        else 
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: "Для начала нужно создать список.",
                                replyMarkup: replyKeyboardMarkup,
                                cancellationToken: cancellationToken);
                        }
                        break;
                    default:
                        if(state == States.NONE)
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
