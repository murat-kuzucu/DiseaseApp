using DiseaseApp.Entity;


namespace DiseaseApp.Data.Abstract
{
    public interface IUserRepository
    {
        IQueryable<User> Users { get; }

        void CreateUser(User user);

        void UpdateUser(User user); //TODO: Implement this method
    }
}