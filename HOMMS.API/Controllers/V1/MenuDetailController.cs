using AutoMapper;
using HOMMS.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HOMMS.API.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuDetailController : ControllerBase
    {
        private readonly IMenuDetailService _menuDetailService;

        public MenuDetailController(IMenuDetailService menuDetailService)
        {
            _menuDetailService = menuDetailService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok();
        }
    }
}
