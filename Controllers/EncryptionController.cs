using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using EncryptionApi.Services; // Asegúrate que el namespace coincida.

namespace EncryptionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncryptionController : ControllerBase
    {
        private readonly EncryptionService _encryptionService;

        public EncryptionController() // Podrías inyectar el servicio si lo configuras en Startup.cs o Program.cs
        {
            _encryptionService = new EncryptionService();
        }

        [HttpPost("encrypt")]
        public async Task<IActionResult> EncryptFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                byte[] fileData;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    fileData = memoryStream.ToArray();
                }

                byte[] encryptedData = _encryptionService.EncryptFile(fileData);

                // Devuelve el archivo encriptado. Podrías guardarlo o manejarlo como necesites.
                // Para este ejemplo, lo devolvemos con un nombre modificado.
                return File(encryptedData, "application/octet-stream", $"{Path.GetFileNameWithoutExtension(file.FileName)}_encrypted{Path.GetExtension(file.FileName)}");
            }
            catch (Exception ex)
            {
                // Loggear el error (no implementado aquí por brevedad)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Endpoint de desencriptación para pruebas o si tu compañero también usa la API
        [HttpPost("decrypt")]
        public async Task<IActionResult> DecryptFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                byte[] fileData;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    fileData = memoryStream.ToArray();
                }

                byte[] decryptedData = _encryptionService.DecryptFile(fileData);

                return File(decryptedData, "application/octet-stream", $"{Path.GetFileNameWithoutExtension(file.FileName).Replace("_encrypted", "")}_decrypted{Path.GetExtension(file.FileName)}");
            }
            catch (Exception ex)
            {
                // Loggear el error
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
