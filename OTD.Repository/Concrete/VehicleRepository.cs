using Microsoft.EntityFrameworkCore;
using OTD.Core.Entities;
using OTD.Repository.Abstract;

namespace OTD.Repository.Concrete
{
    public class VehicleRepository : BaseRepository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
