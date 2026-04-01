using Abstracciones.Interfaces.API;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductoController : ControllerBase, IProductoController
    {
        private readonly IProductoFlujo _productoFlujo;
        private ILogger<ProductoController> _logger;

        public ProductoController(IProductoFlujo productoFlujo, ILogger<ProductoController> logger)
        {
            _productoFlujo = productoFlujo;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Agregar(Producto.ProductoRequest producto)
        {
            var resultado = await _productoFlujo.Agregar(producto);
            return CreatedAtAction(nameof(Obtener), new {Id=resultado}, null);
        }

        [HttpPut("{Id}")]
        [Authorize]
        public async Task<IActionResult> Editar(Guid Id, Producto.ProductoRequest producto)
        {
            var resultado = await _productoFlujo.Editar(Id, producto);
            return Ok(resultado);
        }

        [HttpDelete("{Id}")]
        [Authorize]
        public async Task<IActionResult> Eliminar(Guid Id)
        {
            var resultado = await _productoFlujo.Eliminar(Id);
            return NoContent();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Obtener()
        {
            var resultado = await _productoFlujo.Obtener();
            if (!resultado.Any())
                return NoContent();

            return Ok(resultado);

        }

        [HttpGet("{Id}")]
        [Authorize]
        public async Task<IActionResult> Obtener(Guid Id)
        {
            var resultado = await _productoFlujo.Obtener(Id);

            if (resultado == null)
                return NotFound();



            return Ok(resultado);
        }
    }
}
