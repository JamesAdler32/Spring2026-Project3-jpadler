using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using VaderSharp2;

namespace Spring2026_Project3_jpadler.Controllers
{
    public class Review
    {
        public required string ReviewStr;
        public double Sentiment;
    }
    public class ReviewBundle
    {
        public required IEnumerable<Review> Reviews;
        public double SentimentAvg;
    }

    public class ReviewGenerator
    {
        private static Uri? ApiEndpoint;
        private static ApiKeyCredential? ApiCredential;
        private static string? AiDeployment;

        private int Year = 0;
        private string Name = "";

        public ReviewGenerator(string Name, int Year)
        {
            this.Name = Name;
            this.Year = Year;
        }

        public static void Init(IConfiguration config)
        {
            ApiEndpoint = new(config["ENDPOINT"]!);
            ApiCredential = new ApiKeyCredential(config["API_KEY"]!);
            AiDeployment = config["MODEL_DEPLOYMENT_NAME"]!;
        }

        public async Task<ReviewBundle> MovieReviews()
        {
            return await MovieReviewsSingleCallParsed();
        }
        public async Task<ReviewBundle> TwitterReviews()
        {
            return await TwitterApiSim();
        }

        private async Task<ReviewBundle> MovieReviewsSingleCallParsed()
        {
            //Console.WriteLine("Asking reviewers...");
            ChatClient client = new AzureOpenAIClient(ApiEndpoint, ApiCredential).GetChatClient(AiDeployment);

            string[] personas = { "is harsh", "loves romance", "loves comedy", "loves thrillers", "loves fantasy" };
            var messages = new ChatMessage[]
            {
            new SystemChatMessage($"You represent a group of {personas.Length} film critics who have the following personalities: {string.Join(",", personas)}. When you receive a question, respond as each member of the group with each response separated by a '|', but don't indicate which member you are."),
            new UserChatMessage($"How would you rate the movie {Name} released in {Year} out of 10 in 150 words or less?")
            };
            ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages);
            string[] reviews = result.Value.Content[0].Text.Split('|').Select(s => s.Trim()).ToArray();

            var analyzer = new SentimentIntensityAnalyzer();
            double sentimentTotal = 0;
            List<Review> ReviewsSentiments = new List<Review>();

            for (int i = 0; i < reviews.Length; i++)
            {
                string review = reviews[i];
                SentimentAnalysisResults sentiment = analyzer.PolarityScores(review);
                sentimentTotal += sentiment.Compound;

                //Console.WriteLine($"Review {i + 1} (sentiment {sentiment.Compound})");
                //Console.WriteLine(review);
                //Console.WriteLine();

                ReviewsSentiments.Add(new Review { ReviewStr = review, Sentiment = sentiment.Compound });
            }

            double sentimentAverage = sentimentTotal / reviews.Length;
            //Console.Write($"#####\n# Sentiment Average: {sentimentAverage:#.###}\n#####\n");

            var rb = new ReviewBundle {
                Reviews = ReviewsSentiments,
                SentimentAvg = sentimentAverage,
            };

            return rb;
        }

        private async Task<ReviewBundle> TwitterApiSim()
        {
            //Console.WriteLine("Polling Twitter...");

            ChatClient client = new AzureOpenAIClient(ApiEndpoint, ApiCredential).GetChatClient(AiDeployment);

            var messages = new ChatMessage[]
            {
            new SystemChatMessage($"You represent the Twitter social media platform. Generate an answer with a valid JSON formatted array of objects containing the tweet and username. The response should start with [."),
            new UserChatMessage($"Generate 10 tweets from a variety of users about the actor {Name} aged {Year}.")
            };
            ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages);

            string tweetsJsonString = result.Value.Content.FirstOrDefault()?.Text ?? "[]";
            //Console.WriteLine(tweetsJsonString);
            JsonArray json = JsonNode.Parse(tweetsJsonString)!.AsArray();

            var analyzer = new SentimentIntensityAnalyzer();
            double sentimentTotal = 0;
            List<Review> ReviewsSentiments = new List<Review>();

            var tweets = json.Select(t => new { Username = t!["username"]?.ToString() ?? "", Text = t!["tweet"]?.ToString() ?? "" }).ToArray();
            foreach (var tweet in tweets)
            {
                SentimentAnalysisResults sentiment = analyzer.PolarityScores(tweet.Text);
                sentimentTotal += sentiment.Compound;

                //Console.WriteLine($"{tweet.Username}: \"{tweet.Text}\" (sentiment {sentiment.Compound})\n");

                string review = tweet.Username + ":\n" + tweet.Text;
                ReviewsSentiments.Add(new Review { ReviewStr = review, Sentiment = sentiment.Compound });
            }

            double sentimentAverage = sentimentTotal / tweets.Length;
            //Console.Write($"#####\n# Sentiment Average: {sentimentAverage:#.###}\n#####\n");

            var rb = new ReviewBundle
            {
                Reviews = ReviewsSentiments,
                SentimentAvg = sentimentAverage,
            };

            return rb;
        }

        private record class Tweet(string Username, string Text);
        private record class Tweets(Tweet[] Items);
    }
}
