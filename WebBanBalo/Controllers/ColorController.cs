using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebBanBalo.Dto;
using WebBanBalo.Interface;
using Color = WebBanBalo.Model.Color;

namespace WebBanBalo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IColorRepository _colorRepository;

        public ColorController(IMapper mapper, IColorRepository colorRepository) {
            _mapper = mapper;
            _colorRepository = colorRepository;
        
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id) {

            var color = _colorRepository.Get(id);
            if(color == null)
            {
                return NotFound(new { color=color, message="Khong tim thay color"});
            }
            _colorRepository.Delete(color);
            return Ok(new { color = color, message = "Xoa thanh cong" });
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var color = _colorRepository.GetAll();
            return Ok(_mapper.Map<List<ColorDto>>(color));

        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var color = _colorRepository.Get(id);
            if (color == null) return NotFound("San pham khong ton tai");
            return Ok(_mapper.Map<ColorDto>(color));

        }
        [HttpPost]
        public IActionResult Add([FromBody] ColorDto colorDto)
        { 
            var color = _colorRepository.Get(colorDto.Id);
            if (color != null) return BadRequest(new { color = color, message = "Color Exist" });
            return Ok(_colorRepository.Add(_mapper.Map<Color>(colorDto)));
        }

    }
}
