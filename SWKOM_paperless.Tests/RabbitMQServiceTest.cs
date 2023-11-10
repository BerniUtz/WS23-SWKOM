using Minio;
using Moq;
using SWKOM_paperless.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SWKOM_paperless.BusinessLogic.Interfaces;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using RabbitMQ.Client;

namespace Tests
{
    [TestFixture]

    public class RabbitMQServiceTest
    {
        private Mock<IConnection> _mockConnection = null!;
        private Mock<IModel> _mockChannel = null!;
        private RabbitMQService _service = null!;
        
        [SetUp]
        public void SetUp()
        {
            _mockConnection = new Mock<IConnection>();
            _mockChannel = new Mock<IModel>();

            // Setup mock connection to return mock channel
            _mockConnection.Setup(c => c.CreateModel()).Returns(_mockChannel.Object);

            _service = new RabbitMQService(_mockConnection.Object, _mockChannel.Object, new ConnectionFactory());
        }

        [Test]
        public async Task CreateQueueAsync_CreatesQueue()
        {
            string queueName = "testQueue";
            _mockChannel.Setup(c => c.QueueDeclare(queueName, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>()));

            await _service.CreateQueueAsync(queueName);

            _mockChannel.Verify(c => c.QueueDeclare(queueName, true, false, false, null), Times.Once);
        }
        
        [Test]
        public async Task DeleteQueueAsync_DeletesQueue()
        {
            string queueName = "testQueue";
            _mockChannel.Setup(c => c.QueueDelete(queueName, false, false));
            
            await _service.DeleteQueueAsync(queueName);

            _mockChannel.Verify(c => c.QueueDelete(queueName, false, false), Times.Once);
        }
        
        [Test]
        public async Task DequeueAsync_DequeuesMessage()
        {
            string queueName = "testQueue";
            var message = new TestMessage { Text = "Hello World" };
            var serializedMessage = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(serializedMessage);
            var basicProperties = Mock.Of<IBasicProperties>(); // Create a mock basic properties if needed
            var basicGetResult = new BasicGetResult(1, false, "", queueName, 0, basicProperties, body);
    
            _mockChannel.Setup(c => c.BasicGet(queueName, true)).Returns(basicGetResult);

            var result = await _service.DequeueAsync<TestMessage>(queueName);

            Assert.That(result.Text, Is.EqualTo(message.Text));
        }
    }

    public class TestMessage
    {
        public string Text { get; set; }
    }
}
