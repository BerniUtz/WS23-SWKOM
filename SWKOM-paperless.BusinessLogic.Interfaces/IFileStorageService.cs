using System.IO;
using System.Threading.Tasks;

namespace SWKOM_paperless.BusinessLogic.Interfaces
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Uploads a file to the configured bucket in the storage service.
        /// </summary>
        /// <param name="fileStream">The stream representing the file to upload.</param>
        /// <param name="filePath">The path within the bucket where the file will be stored, including the file name.</param>
        /// <returns>The URL to the uploaded file.</returns>
        Task<string> UploadFileAsync(Stream fileStream, string filePath);

        /// <summary>
        /// Retrieves a file from the configured bucket in the storage service.
        /// </summary>
        /// <param name="filePath">The path to the file within the bucket, including the file name.</param>
        /// <returns>The stream of the retrieved file.</returns>
        Task<Stream> GetFileAsync(string filePath);

        /// <summary>
        /// Deletes a file from the configured bucket in the storage service.
        /// </summary>
        /// <param name="filePath">The path to the file within the bucket, including the file name, to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteFileAsync(string filePath);

        /// <summary>
        /// Checks if a file exists in the configured bucket in the storage service.
        /// </summary>
        /// <param name="filePath">The path to the file within the bucket, including the file name, to check.</param>
        /// <returns>True if the file exists, otherwise false.</returns>
        Task<bool> FileExistsAsync(string filePath);
    }

}