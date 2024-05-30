using DiseaseApp.Concrete.EfCore;
using DiseaseApp.Data.Abstract;
using DiseaseApp.Entity;
namespace DiseaseApp.Data.Concrete.EfCore
{
    public class EfUserRepository : IUserRepository
    {
        private DiseaseContext _context;
        public EfUserRepository(DiseaseContext context)
        {
            _context = context;
        }
        public IQueryable<User> Users => _context.Users;

        public void CreateUser(User User)
        {
            _context.Users.Add(User);
            _context.SaveChanges();
        }
        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        
    }
}