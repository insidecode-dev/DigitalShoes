using DigitalShoes.Dal.Context;
using DigitalShoes.Dal.Repository.Interfaces;
using DigitalShoes.Domain.Entities;



namespace DigitalShoes.Dal.Repository
{
    public class ImageRepository : Repository<Image>, IMageRepository
    {
        public ImageRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
