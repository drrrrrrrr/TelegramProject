
using System.Collections.Generic;

namespace telegramBod.Models
{
    /// <summary>
    /// This object represents one button of an inline keyboard. You must use exactly one of the optional fields.
    /// </summary>
    /// <remarks>
    /// Note: This will only work in Telegram versions released after 9 April, 2016. Older clients will display unsupported message.
    /// </remarks>
    //public class InlineKeyboardButton
    //{
    //    /// <summary>
    //    /// Label text on the button
    //    /// </summary>
    //    public string text { get; set; }

    //    /// <summary>
    //    /// Optional. HTTP url to be opened when button is pressed
    //    /// </summary>
    //    public string url { get; set; }
    //    public InlineKeyboardButton(string _text, string _callback_data = "")
    //    {
    //        text = _text;
    //        callback_data = _callback_data == "" ? _text : _callback_data;
    //    }
    //    /// <summary>
    //    /// Optional. Data to be sent in a callback query to the bot when button is pressed, 1-64 bytes
    //    /// </summary>
    //    /// 
    //    public string callback_data { get; set; }

    //    /// <summary>
    //    /// Optional. If set, pressing the button will prompt the user to select one of their chats,
    //    /// open that chat and insert the bot‘s username and the specified inline query in the input field.
    //    /// Can be empty, in which case just the bot’s username will be inserted.
    //    /// </summary>
    //    /// <remarks>
    //    /// Note: This offers an easy way for users to start using your bot in inline mode when they are currently
    //    /// in a private chat with it. Especially useful when combined with switch_pm… actions – in this case the user
    //    /// will be automatically returned to the chat they switched from, skipping the chat selection screen.
    //    /// </remarks>
    //    public string switchInlineQuery { get; set; }

    //    /// <summary>
    //    /// Optional. If set, pressing the button will insert the bot‘s username and the specified inline query in the current chat's input field.
    //    /// Can be empty, in which case only the bot’s username will be inserted.
    //    /// This offers a quick way for the user to open your bot in inline mode
    //    /// in the same chat – good for selecting something from multiple options.
    //    /// </summary>
    //    public string switchInlineQueryCurrentChat { get; set; }

    //    /// <summary>
    //    /// Optional. Description of the game that will be launched when the user presses the button.
    //    /// NOTE: This type of button must always be the first button in the first row.
    //    /// </summary>
    //}
    public class InlineKeyboardButton
    {
        public string text { get; set; }
        //     public string url { get; set; }
        public string callback_data { get; set; }
        public InlineKeyboardButton(string _text, string _callback_data = "")
        {
            text = _text;
            callback_data = _callback_data == "" ? _text : _callback_data;
        }
    }
    public class InlineKeyboard
    {
        public List<List<InlineKeyboardButton>> inline_keyboard { get; set; }
        public InlineKeyboard(List<List<InlineKeyboardButton>> inline)
        {
            inline_keyboard = inline;
        }
        public InlineKeyboard()
        {
            inline_keyboard = new List<List<InlineKeyboardButton>>();
        }
        public void AddButton(InlineKeyboardButton button)
        {
            inline_keyboard.Add(new List<InlineKeyboardButton> { button });
        }
        public void AddLine(List<InlineKeyboardButton> key)
        {
            inline_keyboard.Add(key);
        }
        public void AddButton(InlineKeyboardButton button, int n)
        {
            while (inline_keyboard.Count <= n)
                inline_keyboard.Add(new List<InlineKeyboardButton>());
            inline_keyboard[n].Add(button);
        }

    }
}
