using Minio;
using Moq;
using SWKOM_paperless.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SWKOM_paperless.BusinessLogic.Interfaces;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace Tests
{
    [TestFixture]

    public class RabbitMQServiceTest
    {
        /*** currently with sideeffects on test queue ***/


        private IQueueService _mockRabbitMQ;

        [SetUp]
        public void SetUp()
        {
            _mockRabbitMQ = new RabbitMQService("localhost", "rabbitmq", "rabbitmq");
        }

        [Test, Order(1)]
        public async Task EnsureQueue()
        {
            var check = await _mockRabbitMQ.QueueExistsAsync("test_queue");

            
            Assert.IsFalse(check);

            await _mockRabbitMQ.EnsureQueueExistsAsync("test_queue");

            check = await _mockRabbitMQ.QueueExistsAsync("test_queue");
            Assert.IsTrue(check);
        }

        [Test, Order(2)]
        public async Task Queueing()
        {
           string message = "testmessage";
           await _mockRabbitMQ.EnqueueAsync("test_queue", message);
            
           var result = await _mockRabbitMQ.DequeueAsync<string>("test_queue");



           Assert.AreEqual(result, "testmessage");
        }

        [Test, Order(3)]
        public async Task DeleteQueue()
        {
            _mockRabbitMQ.DeleteQueueAsync("test_queue");

            var check = await _mockRabbitMQ.QueueExistsAsync("test_queue");
            Assert.IsFalse(check);

        }


    }
}
