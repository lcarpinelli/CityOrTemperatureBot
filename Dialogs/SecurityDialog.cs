using DialogBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DialogBot.Dialogs
{
    public class SecurityDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;
        static CovidModel model = new CovidModel();

        public SecurityDialog() : base(nameof(SecurityDialog))
        {
            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {
                Welcome,
                GetData,
                Restart
            };

            // Aggiungi finestre di dialogo con nome al DialogSet. Questi nomi vengono salvati nello stato della finestra di dialogo.
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> Welcome(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Context.Activity.ChannelId.Equals(Microsoft.Bot.Connector.Channels.Telegram))
            {
                var data = stepContext.Context.Activity.ChannelData as dynamic;
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Ciao " + data?.message?.from?.first_name + ", Inserisci la data (mm-gg-yyyy) del giorno che vuoi visionare") }, cancellationToken);
            }
            else
            {
                if (model.Id == null)
                {
                    model.Id = stepContext.Context.Activity.From.Id;
                    model.Name = stepContext.Context.Activity.Text;
                }
                else if (!model.Id.Equals(stepContext.Context.Activity.From.Id))
                {
                    model.Name = stepContext.Context.Activity.Text;
                }
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Ciao " + model.Name + ", Inserisci la data (mm-gg-yyyy) del giorno che vuoi visionare.") }, cancellationToken);
            }
        }
        private static async Task<DialogTurnResult> GetData(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            model.Day = (string)stepContext.Result;

            HttpClient client = new HttpClient();
            string url = "http://raw.githubusercontent.com/pcm-dpc/COVID-19/master/dati-json/dpc-covid19-ita-andamento-nazionale.json";
            model.Response =  await client.GetAsync(url);

            string result = await model.Response.Content.ReadAsStringAsync();
            var Days = JsonConvert.DeserializeObject<ICollection<DayModel>>(result);

            try
            {
                var day = DateTime.Parse(model.Day);

                foreach (var x in Days)
                {
                    if (x.Date.Value.ToString("d") == day.ToString("d"))
                    {
                        model.Deceduti = x.Deaths;
                        model.Positivi = x.TotalPositives;
                        break;
                    }
                }

                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                   new PromptOptions
                   {
                       Prompt = MessageFactory.Text($"Data: {model.Day}, Deceduti: {model.Deceduti}, Attualmente Positivi: { model.Positivi}"),
                       Choices = ChoiceFactory.ToChoices(new List<string> { "Restart", "End" }),
                   }, cancellationToken);

            }
            catch (Exception e)
            {
                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                 new PromptOptions
                 {
                     Prompt = MessageFactory.Text("Data Errata"),
                     Choices = ChoiceFactory.ToChoices(new List<string> { "Restart", "End" }),
                 }, cancellationToken);
            }

        }

        private static async Task<DialogTurnResult> Restart(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            model.Restart = ((FoundChoice)stepContext.Result).Value;

            if (model.Restart == "Restart")
            {
                return await stepContext.ReplaceDialogAsync("WaterfallDialog");
            }
            else
            {
                await stepContext.CancelAllDialogsAsync(cancellationToken);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Grazie e Buona Giornata!") }, cancellationToken);
            }
        }

    }
}
