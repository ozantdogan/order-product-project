﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OTD.Core.Models.Responses;
using OTD.Core.Models.SearchRequests;
using OTD.Repository.Abstract;
using OTD.ServiceLayer.Abstract;
using OTD.ServiceLayer.Concrete;

namespace OTD.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpGet("GetProducts")]
        public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetProducts([FromQuery] ProductSearchRequest request)
        {
            var response = await _service.List(request);
            return Ok(response);
        }
    }
}
