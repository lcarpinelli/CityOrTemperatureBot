using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
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

        public SecurityDialog()
            : base(nameof(SecurityDialog))
        {

            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {
                //Città o temperatura con CAP
                AskCity,
                CityOrTemp,
                GetCityOrTemp

                //Dialogo
                //TransportStepAsync,
                //NameStepAsync,
                //NameConfirmStepAsync,
                //AgeStepAsync,
                //PictureStepAsync,
                //ConfirmStepAsync,
                //SummaryStepAsync,
            };

            // Aggiungi finestre di dialogo con nome al DialogSet. Questi nomi vengono salvati nello stato della finestra di dialogo.
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        //Città o temperatura con CAP
        private static async Task<DialogTurnResult> AskCity(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Sei in difficoltà?") }, cancellationToken);
        }
        private static async Task<DialogTurnResult> CityOrTemp(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["cap"] = (string)stepContext.Result;

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Vuoi sapere la temperatura o il nome della città?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Nome", "Temperatura" }),
                }, cancellationToken);
        }
        private static async Task<DialogTurnResult> GetCityOrTemp(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string choice = ((FoundChoice)stepContext.Result).Value;

            HttpClient client = new HttpClient();
            string url = $"http://api.openweathermap.org/data/2.5/weather?zip=" + stepContext.Values["cap"] + ",IT&appid=c649e81366b62803db5824367c4da223&units=metric";

            HttpResponseMessage response = await client.GetAsync(url);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("This CAP is Wrong") }, cancellationToken);
            }
            else
            {
                string r = response.Content.ReadAsStringAsync().Result;
                JObject j = JObject.Parse(r);
                if (choice.Equals("Nome"))
                {
                    string city = j.SelectToken("name").ToString();
                                   
                    return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Your city is " + city) }, cancellationToken); 
                }
                else
                {
                    string temp = j.SelectToken("main").SelectToken("temp").ToString();
                    return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("La temperatura è di " + temp + "°C") }, cancellationToken);
                }
            }
        }
    }
}
