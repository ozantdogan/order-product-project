using OTD.Core.Entities;
using OTD.Repository.Abstract;
using OTD.ServiceLayer.Abstract;
using OTD.ServiceLayer.Helper;
using OTD.Core.Models.Responses;

namespace OTD.ServiceLayer.Concrete
{
    public class VehicleService : BaseService
    {
        private readonly IVehicleRepository _repository;

        public VehicleService(IVehicleRepository vehicleRepository)
        {
            _repository = vehicleRepository;
        }

        //public async Task<ApiResponse> Add(VehicleDto dto)
        //{
        //    var vehicle = new Vehicle()
        //    {
        //        Manufacturer = dto.Manufacturer,
        //        Model = dto.Model,
        //        Year = dto.Year,
        //        Color = dto.Color,
        //        Horsepower = dto.Horsepower
        //    };

        //    _repository.Add(vehicle);
        //    var success = await _repository.SaveChanges();
        //    if(success == false)
        //        return GenerateResponse<ApiResponse>(success, ErrorCode.Failed, null);

        //    return GenerateResponse(success, ErrorCode.Success, dto);
        //}

        //public ApiResponse Update(VehicleDto dto)
        //{
        //    var vehicle = _repository.Get(x => x.VehicleId == dto.VehicleId && x.DeleteFlag == false);

        //    if (vehicle == null)
        //        return GenerateResponse<ApiResponse>(false, ErrorCode.Failed, null);

        //    vehicle.Manufacturer = dto.Manufacturer;
        //    vehicle.Model = dto.Model;
        //    vehicle.Year = dto.Year;
        //    vehicle.Color = dto.Color;
        //    vehicle.Horsepower = dto.Horsepower;
        //    vehicle.ModifiedOn = DateTime.Now;

        //    var success = _repository.Update(vehicle);
        //    if (success == false)
        //        return GenerateResponse<ApiResponse>(success, ErrorCode.Failed, null);

        //    return GenerateResponse(success, ErrorCode.Success, dto);
        //}

        //public ApiResponse Delete(VehicleDto dto)
        //{
        //    var vehicle = _repository.Get(x => x.VehicleId == dto.VehicleId && x.DeleteFlag == false);

        //    if (vehicle == null)
        //        return GenerateResponse(false, ErrorCode.Failed, dto);

        //    vehicle.DeleteFlag = true;

        //    _repository.Update(vehicle);
        //    var success = _repository.SaveChanges();
        //    if (success == false)
        //        return GenerateResponse<ApiResponse>(success, ErrorCode.Failed, null);

        //    return GenerateResponse(success, ErrorCode.Failed, dto);
        //}

        //public ApiResponse Get(Guid vehicleId)
        //{
        //    var vehicle = _repository.Get(x => x.VehicleId == vehicleId && x.DeleteFlag == false);

        //    if (vehicle == null)
        //        return GenerateResponse<ApiResponse>(false, ErrorCode.Failed, null);

        //    return GenerateResponse(true, ErrorCode.Success, vehicle);
        //}

        //public async Task ApiResponse List()
        //{
        //    var response = new ApiResponse();

        //    var vehicles = await _repository.List(x => x.DeleteFlag == false);

        //    if (vehicles == null)
        //        return GenerateResponse<ApiResponse>(false, ErrorCode.Failed, null);

        //    return GenerateResponse(true, ErrorCode.Success, vehicles);
        //}
    }
}
