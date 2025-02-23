using Microsoft.AspNetCore.Mvc;
using OTD.Repository.Abstract;
using OTD.ServiceLayer.Concrete;

namespace OTD.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly VehicleService _vehicleBusiness;
        public VehicleController(IVehicleRepository vehicleRepository)
        {
            _vehicleBusiness = new VehicleService(vehicleRepository);
        }

        //[HttpGet("List")]
        //[Authorize]
        //public async Task<ActionResult<ApiResponse>> List()
        //{
        //    var response = await _vehicleBusiness.List();
        //    if (!response.Success)
        //        return BadRequest(response);

        //    return Ok(response);
        //}

        //[HttpGet("Get")]
        //[Authorize]
        //public async Task<ActionResult<ApiResponse>> GetHero(Guid id)
        //{
        //    var response = await _vehicleBusiness.Get(id);
        //    if (!response.Success)
        //        return BadRequest(response);

        //    return Ok(response);
        //}

        //[HttpPost]
        //[Authorize]
        //public async Task<ActionResult<ApiResponse>> AddHero(VehicleDto dto)
        //{
        //    var response = await _vehicleBusiness.Add(dto);
        //    if (!response.Success)
        //        return BadRequest(response);

        //    return Ok(response);
        //}

        //[HttpPut]
        //[Authorize]
        //public async Task<ActionResult<ApiResponse>> UpdateHero(VehicleDto dto)
        //{
        //    var response = await _vehicleBusiness.Update(dto);
        //    if(!response.Success)
        //        return BadRequest(response);

        //    return Ok(response);
        //}

        //[HttpDelete]
        //[Authorize]
        //public async Task<ActionResult<ApiResponse>> DeleteHero(VehicleDto dto)
        //{
        //    var response = await _vehicleBusiness.Delete(dto);
        //    if (!response.Success)
        //        return BadRequest(response);

        //    return Ok(response);
        //}
    }
}
