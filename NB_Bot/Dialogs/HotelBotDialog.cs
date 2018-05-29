using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using NB_Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace NB_Bot.Dialogs
{
    public class HotelBotDialog
    {
        public static readonly IDialog<string> dialog = Chain.PostToChain()
            .Select(msg => msg.Text)
            .Switch(
            new RegexCase<IDialog<string>>(new Regex("^hi", RegexOptions.IgnoreCase), (context, text) =>
            {
                return Chain.ContinueWith(new GreetingDialog(), AfterGreetingContinuationAsync);
            }),
            new DefaultCase<string, IDialog<string>>((context, text) =>
            {
                return Chain.ContinueWith(FormDialog.FromForm(RoomReservation.BuildForm, FormOptions.PromptInStart), AfterGreetingContinuationAsync);
            }))
        .Unwrap()
        .PostToUser();

        private static async Task<IDialog<string>> AfterGreetingContinuationAsync(IBotContext context, IAwaitable<object> item)
        {
            var token = await item;
            var name = "User";
            context.UserData.TryGetValue<string>("Name", out name);
            return Chain.Return($"Thank you for using the hotel bot: {name}");
        }

    }
}