using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.DAL
{
    public class DocumentRepository
    {
        private readonly ApplicationDbContext<Document> _context;

        public DocumentRepository(ApplicationDbContext<Document> context)
        {
            _context = context;
        }

        public void AddDocument(Document document)
        {
            _context.Elements?.Add(document);
            _context.SaveChanges();
        }

        public IEnumerable<Document>? GetAllDocuments()
        {
            return _context.Elements?.ToList();
        }
        
        public Document? GetDocumentById(int id)
        {
            return _context.Elements?.Find(id);
        }
        
        public void DeleteDocument(int id)
        {
            var document = _context.Elements?.Find(id);
            if (document == null) return;
            
            _context.Elements?.Remove(document);
            _context.SaveChanges();
        }
    }
}