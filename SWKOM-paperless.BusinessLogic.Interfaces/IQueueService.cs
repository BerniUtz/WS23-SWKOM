using System.Threading.Tasks;

namespace SWKOM_paperless.BusinessLogic.Interfaces
{
    public interface IQueueService
    {
        /// <summary>
        ///     Enqueues an object onto the specified queue.
        ///     The object is serialized into a message format such as JSON.
        /// </summary>
        /// <typeparam name="T">The type of the object to be enqueued.</typeparam>
        /// <param name="queueName">The name of the queue.</param>
        /// <param name="messageObject">The object to enqueue.</param>
        /// <returns>A task that represents the asynchronous enqueue operation.</returns>
        Task EnqueueAsync<T>(string queueName, T messageObject);

        /// <summary>
        ///     Attempts to dequeue a message from the specified queue and deserialize it to the specified object type.
        /// </summary>
        /// <typeparam name="T">The type to which the message should be deserialized.</typeparam>
        /// <param name="queueName">The name of the queue.</param>
        /// <returns>
        ///     A task that represents the asynchronous dequeue operation. The task result contains the deserialized object.
        ///     If the queue is empty, the result should default(T).
        /// </returns>
        Task<T> DequeueAsync<T>(string queueName) where T : class;

        /// <summary>
        ///     Checks if the specified queue exists.
        /// </summary>
        /// <param name="queueName">The name of the queue to check.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result is true if the queue exists, false otherwise.
        /// </returns>
        Task<bool> QueueExistsAsync(string queueName);

        /// <summary>
        ///     Creates a queue with the specified name.
        /// </summary>
        /// <param name="queueName">The name of the queue to create.</param>
        /// <returns>A task that represents the asynchronous queue creation operation.</returns>
        Task CreateQueueAsync(string queueName);

        /// <summary>
        ///     Deletes the specified queue.
        /// </summary>
        /// <param name="queueName">The name of the queue to delete.</param>
        /// <returns>A task that represents the asynchronous queue deletion operation.</returns>
        Task DeleteQueueAsync(string queueName);
        
        /// <summary>
        ///     Ensures that the specified queue exists.
        /// </summary>
        /// <param name="queueName">The name of the queue to ensure.</param>
        /// <returns>A task that represents the asynchronous queue creation operation if the queue did not exist.</returns>
        Task EnsureQueueExistsAsync(string queueName);
    }
}