using OpenAI.Chat;

namespace ClaimStatusAPI.Services
{
    public interface IOpenAiService
    {
        /// <summary>
        /// Sends a chat completion request and returns the assistant reply as string.
        /// </summary>
        /// <param name="messages">Ordered list of messages (system, user, assistant)</param>
        Task<ChatCompletion?> CreateChatCompletionAsync(IEnumerable<ChatMessage> messages);
    }
}
