using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebhookGail.Data;
using WebhookGail.Middleware;
using WebhookGail.Models;
using WebhookGail.Services;

namespace WebhookGail.Controllers
{
    [ApiController]
    [Route("axia/instructions")]
    public class InstructionsController : ControllerBase
    {
        private readonly ILogger<DataOrbitController> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly WebhookGailContext _dbContext;
        private readonly Rapihogar _rapiInstance;

        public InstructionsController(ILogger<DataOrbitController> logger, IConfiguration configuration, HttpClient httpClient, WebhookGailContext dbContext, Rapihogar rapiInstance)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
            _dbContext = dbContext;
            _rapiInstance = rapiInstance;
        }

        [HttpGet]
        [ServiceFilter(typeof(TokenValidationMiddleware))]
        public async Task<IActionResult> GetInstructionsAsync()
        {
            try
            {
                var instructions = await _dbContext.Instructions.ToListAsync();

                if (instructions == null || !instructions.Any())
                {
                    return NotFound("No se encontraron instrucciones.");
                }

                return Ok(instructions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ServiceFilter(typeof(TokenValidationMiddleware))]
        public async Task<IActionResult> GetInstructionAsync([FromRoute] int id)
        {
            try
            {
                var instruction = await _dbContext.Instructions.FindAsync(id);

                if (instruction == null)
                {
                    return NotFound("No se encontro instruccion.");
                }

                return Ok(instruction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(TokenValidationMiddleware))]
        public async Task<IActionResult> PostInstructionsAsync([FromBody] Instructions instructions)
        {
            try
            {

                if (instructions == null)
                {
                    return NotFound("No se encontraron clientes.");
                }

                _dbContext.Instructions.Add(instructions);
                await _dbContext.SaveChangesAsync();
                return StatusCode(201, instructions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(TokenValidationMiddleware))]
        public async Task<IActionResult> UpdateInstruction([FromRoute] int id, [FromBody] Instructions updatedInstruction)
        {
            try
            {
                var instruction = await _dbContext.Instructions.FindAsync(id);
                if (instruction == null)
                    return NotFound("Instrucción no encontrada.");

                instruction.Business = updatedInstruction.Business;
                instruction.Name = updatedInstruction.Name;
                instruction.Instruction = updatedInstruction.Instruction;
                instruction.ScriptId = updatedInstruction.ScriptId;
                instruction.Variable = updatedInstruction.Variable;

                await _dbContext.SaveChangesAsync();
                return Ok("Instrucción actualizada con éxito.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(TokenValidationMiddleware))]
        public async Task<IActionResult> DeleteInstruction([FromRoute] int id)
        {
            try
            {
                Instructions? instructionToDelete = await _dbContext.Instructions.FindAsync(id);

                if (instructionToDelete == null)
                {
                    return NotFound($"Instrucción con ID {id} no encontrada.");
                }

                _dbContext.Instructions.Remove(instructionToDelete);
                await _dbContext.SaveChangesAsync();

                return Ok("Instrucción eliminada con éxito.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
