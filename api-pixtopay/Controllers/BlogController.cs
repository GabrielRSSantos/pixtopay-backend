using api_pixtopay.Database;
using api_pixtopay.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace api_pixtopay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly SmtpSettings _smtpSettings;

        public BlogController(DatabaseContext context, IOptions<SmtpSettings> smtpSettings)
        {
            _context = context;
            _smtpSettings = smtpSettings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var blogs = await _context.blog.ToListAsync();
            return Ok(blogs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var blog = await _context.blog.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return Ok(blog);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Encontra o blog pelo ID
            var blog = await _context.blog.FindAsync(id);
            if (blog == null)
            {
                // Retorna um erro 404 se o blog não for encontrado
                return NotFound();
            }

            // Remove o blog do contexto
            _context.blog.Remove(blog);

            // Salva as mudanças no banco de dados
            await _context.SaveChangesAsync();

            // Retorna uma resposta 204 No Content indicando que a exclusão foi bem-sucedida
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> Post(string nome, string description, IFormFile image, IFormFile imageBackground)
        {
            if (string.IsNullOrEmpty(nome))
            {
                return BadRequest("Blog name is required.");
            }

            if (image == null || image.Length == 0)
            {
                return BadRequest("Image is missing.");
            }

            if (imageBackground == null || imageBackground.Length == 0)
            {
                return BadRequest("Background image is missing.");
            }

            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            byte[] imageBackgroundBytes;
            using (var memoryStream = new MemoryStream())
            {
                await imageBackground.CopyToAsync(memoryStream);
                imageBackgroundBytes = memoryStream.ToArray();
            }

            // Cria o blog
            var blog = new Blog
            {
                Nome = nome,
                Description = description,
                Image = imageBytes,
                ImageBackground = imageBackgroundBytes
            };

            // Adiciona o blog ao contexto do banco de dados
            _context.blog.Add(blog);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = blog.Id }, blog);
        }

        [HttpPost("contato")]
        public async Task<IActionResult> PostForm([FromBody] Contato contato)
        {
            if (contato == null)
            {
                return BadRequest();
            }

            var message = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                Subject = "Novo Contato",
                Body = $"Nome: {contato.Nome}\nEmail: {contato.Email}\nTelefone: {contato.Tel}\nEmpresa: {contato.Company}\nMensagem: {contato.Message}",
                IsBodyHtml = false
            };
            message.To.Add(new MailAddress("gabrielsantos.new@gmail.com")); 

            using (var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
            {
                client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                client.EnableSsl = true;
                await client.SendMailAsync(message);
            }

            return Ok(contato);
        }
    }
}
