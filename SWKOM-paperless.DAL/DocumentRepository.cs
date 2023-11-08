using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.DAL
{
    public class DocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddDocument(Document document)
        {
            _context.Documents?.Add(document);
            _context.SaveChanges();
        }

        public IEnumerable<Document>? GetAllDocuments()
        {
            return _context.Documents?.ToList();
        }
        
        public Document? GetDocumentById(int id)
        {
            return _context.Documents?.Find(id);
        }
        
        public void DeleteDocument(int id)
        {
            var document = _context.Documents?.Find(id);
            if (document == null) return;
            
            _context.Documents?.Remove(document);
            _context.SaveChanges();
        }
    }
}