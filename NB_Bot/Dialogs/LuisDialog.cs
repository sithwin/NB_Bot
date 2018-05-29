using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using NB_Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NB_Bot.Dialogs
{
    [LuisModel("1c34364f-dbaf-4771-bb06-a48e6d0f0a8a", "ff46453060b149868c763c341bb4d645")]
    [Serializable]
    public class LuisDialog: LuisDialog<RoomReservation>
    {
        private readonly BuildFormDelegate<RoomReservation> ReserveRoom;

        public LuisDialog(BuildFormDelegate<RoomReservation> reserveRoom)
        {
            this.ReserveRoom = reserveRoom;
        }
        
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry I don't know what you mean.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            context.Call(new GreetingDialog(), Callback);
        }

        private async Task Callback(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
        }

        [LuisIntent("Reservation")]
        public async Task RoomReservation(IDialogContext context, LuisResult result)
        {
            var enrollmentForm = new FormDialog<RoomReservation>(new RoomReservation(), this.ReserveRoom, FormOptions.PromptInStart);
            context.Call<RoomReservation>(enrollmentForm, Callback);
        }

        [LuisIntent("Query Amenities")]
        public async Task QueryAmenities(IDialogContext context, LuisResult result)
        {
            foreach (var entity in result.Entities.Where(Entity => Entity.Type == "Amenity"))
            {
                var value = entity.Entity.ToLower();
                if (value == "pool" || value == "gym" || value == "wifi" || value == "towels")
                {
                    await context.PostAsync("Yes we have that!");
                    context.Wait(MessageReceived);
                    return;
                }
                else
                {
                    await context.PostAsync("I'm sorry we don't have that.");
                    context.Wait(MessageReceived);
                    return;
                }
            }
            await context.PostAsync("I'm sorry we don't have that.");
            context.Wait(MessageReceived);
            return;
        }

    }
}