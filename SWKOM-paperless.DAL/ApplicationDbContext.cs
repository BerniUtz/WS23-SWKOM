using Microsoft.EntityFrameworkCore;
using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Correspondent>? Correspondents { get; set; }
        public DbSet<DocTag>? DocTags { get; set; }
        public DbSet<Document>? Documents { get; set; }
        public DbSet<DocumentType>? DocumentTypes { get; set; }
        public DbSet<NewCorrespondent>? NewCorrespondents { get; set; }
        public DbSet<NewDocumentType>? NewDocumentTypes { get; set; }
        public DbSet<NewTag>? NewTags { get; set; }
        public DbSet<UserInfo>? UserInfos { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}