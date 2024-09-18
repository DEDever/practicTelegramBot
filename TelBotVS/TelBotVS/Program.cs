using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient("7420531426:AAHzmRGfHX5HJcWoR1Vv89iQgB0zEUIJsRU", cancellationToken: cts.Token); //Токен бота
var me = await bot.GetMeAsync();

//Шляхи до файлів
string ReadWorkers = @"C:\Artem\Programming\ForFiles\WorkersID.txt";
string OrderPath = @"C:\Artem\Programming\ForFiles\Orders.txt";
string order; //для зберігання замовлення

bot.OnMessage += OnMessage;
bot.OnError += OnError;
bot.OnUpdate += OnUpdate;

Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
Console.ReadLine();
cts.Cancel(); // Зубинка при нажатті enter

bool WorkerCheck(Message msg)
{
    foreach (string line in System.IO.File.ReadLines(ReadWorkers))
    {
        if (msg.From.Id == Convert.ToInt64(line)) return true;
    }
    return false;
}

// method that handle messages received by the bot: 


async Task OnMessage(Message msg, UpdateType type)
{
    if (msg.Text is null) return;	// тільки текстові повідомлення
    Console.WriteLine($"Received {type} '{msg.Text}' in {msg.Chat} at {msg.Date.AddHours(3)} UTC+3"); //пише в консоль повідомлення користувачів
    if (WorkerCheck(msg) == true)
    {
        if (msg.Text.Equals("/start") == true)
        {
            await bot.SendTextMessageAsync(msg.Chat, "Можете писати замовлення");
            return;
        }
        order = $"{msg.From} said: {msg.Text}" + Environment.NewLine;
        await bot.SendTextMessageAsync(msg.Chat, order);// Відправляє відповідь
        //System.IO.File.AppendAllLines(OrderPath, order); невдала спроба записати у файл
        using (StreamWriter w = System.IO.File.AppendText(OrderPath))
        {
            w.WriteLine(order); //записує замовлення у файл
        }
    }
    else
    {
        Console.WriteLine($"User {msg.From} is not worker. Access denied");
        await bot.SendTextMessageAsync(msg.Chat, "Вам треба бути записаними у список робітників. Зверніться до адміністратора");
    }
    
    /*switch (msg.Chat.Id)
    {
        case 334635746:
            await bot.SendTextMessageAsync(495402271, $"Guru:{msg.From} said:{msg.Text}");
            break;
        default:
            await bot.SendTextMessageAsync(495402271, $"Iddiot who's name is:{msg.From} said:{msg.Text}");
            break;
    }*/

}

async Task OnError(Exception exception, HandleErrorSource source)
{
    Console.WriteLine(exception); // just dump the exception to the console
}
async Task OnUpdate(Update update)
{
    if (update is { CallbackQuery: { } query }) // non-null CallbackQuery
    {
        await bot.AnswerCallbackQueryAsync(query.Id, $"You picked {query.Data}");
        await bot.SendTextMessageAsync(query.Message!.Chat, $"User {query.From} clicked on {query.Data}");
    }
}


















