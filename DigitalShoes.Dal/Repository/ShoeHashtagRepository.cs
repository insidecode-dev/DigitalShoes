using DigitalShoes.Dal.Context;
using DigitalShoes.Dal.Repository.Interfaces;
using DigitalShoes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Dal.Repository
{
    public class ShoeHashtagRepository : Repository<ShoeHashtag>, IShoeHashtagRepository
    {
        public ShoeHashtagRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
