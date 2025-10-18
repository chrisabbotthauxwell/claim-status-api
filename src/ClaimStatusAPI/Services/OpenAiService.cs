using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.Text.Json;

namespace ClaimStatusAPI.Services
{
    public class OpenAiService : IOpenAiService
    {
        private readonly ILogger<OpenAiService> _logger;
        private readonly ChatClient _chatClient;
        private bool _disposed;

        public OpenAiService(IConfiguration config, ILogger<OpenAiService> logger)
        {
            _logger = logger;

            var endpoint = config["AzureOpenAI:Endpoint"] ?? config["AZURE_OPENAI_ENDPOINT"];
            var key = config["AzureOpenAI:Key"] ?? config["AZURE_OPENAI_KEY"];
            var deploymentName = config["AzureOpenAI:Model"] ?? config["OPENAI_MODEL"];

            if (string.IsNullOrEmpty(endpoint))
            {
                throw new ArgumentException("Azure OpenAI endpoint not configured (AzureOpenAI:Endpoint or AZURE_OPENAI_ENDPOINT)");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Azure OpenAI key not configured (AzureOpenAI:Key or AZURE_OPENAI_KEY)");
            }
            else
            {
                _logger.LogInformation("Using Azure OpenAI key from configuration");
                
                // Initialise the Azure Key credential
                AzureKeyCredential credential = new AzureKeyCredential(key);

                // Initialize the AzureOpenAIClient
                AzureOpenAIClient azureOpenAiClient = new(new Uri(endpoint), credential);

                // Initialize the ChatClient with the specified deployment name
                _chatClient = azureOpenAiClient.GetChatClient(deploymentName);
            }
        }

        public async Task<ChatCompletion?> CreateChatCompletionAsync(IEnumerable<ChatMessage> messages)
        {
            if (messages == null)
            {
                _logger.LogError("Messages parameter is null");
                throw new ArgumentNullException(nameof(messages));
            }

            // Create chat completion options
            var options = new ChatCompletionOptions
            {
                Temperature = (float)0.7,
                MaxOutputTokenCount = 800,

                TopP = (float)0.95,
                FrequencyPenalty = (float)0,
                PresencePenalty = (float)0
            };
            
            // Try to get chat completions
            try
            {
                // Create the chat completion request
                ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);

                // Print the response
                if (completion != null)
                {
                    _logger.LogInformation("Received chat completion from OpenAI");
                    return completion;
                }
                else
                {
                    _logger.LogWarning("No response received from OpenAI");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting chat completion");
                throw;
            }
        }
    }
}
