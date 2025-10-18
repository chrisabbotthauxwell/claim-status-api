using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClaimStatusAPI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClaimStatusAPI.UnitTests
{
    public class OpenAiServiceTests
    {
        [Fact]
        public void Ctor_Throws_When_EndpointMissing()
        {
            // Arrange
            var inMem = new Dictionary<string, string>();
            var config = new ConfigurationBuilder().AddInMemoryCollection(inMem).Build();
            var logger = new Mock<ILogger<OpenAiService>>().Object;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new OpenAiService(config, logger));
        }

        [Fact]
        public void Ctor_Throws_When_KeyMissing()
        {
            // Arrange: only endpoint provided
            var inMem = new Dictionary<string, string>
            {
                { "AzureOpenAI:Endpoint", "https://example.openai.azure.com/" }
            };
            var config = new ConfigurationBuilder().AddInMemoryCollection(inMem).Build();
            var logger = new Mock<ILogger<OpenAiService>>().Object;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new OpenAiService(config, logger));
        }

        [Fact]
        public async Task CreateChatCompletionAsync_Throws_When_Messages_Null()
        {
            // Arrange: provide endpoint + key so constructor succeeds
            var inMem = new Dictionary<string, string>
            {
                { "AzureOpenAI:Endpoint", "https://example.openai.azure.com/" },
                { "AzureOpenAI:Key", "fake-key-for-tests" },
                { "AzureOpenAI:Model", "gpt-4o-mini" }
            };
            var config = new ConfigurationBuilder().AddInMemoryCollection(inMem).Build();
            var logger = new Mock<ILogger<OpenAiService>>().Object;

            var svc = new OpenAiService(config, logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await svc.CreateChatCompletionAsync(null!));
        }
    }
}
