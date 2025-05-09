using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Services.IImplement;
using Microsoft.AspNetCore.Mvc;

namespace ConsultantDashboard.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaqController : ControllerBase
    {
        private readonly IFaqService _faqService;

        public FaqController(IFaqService faqService)
        {
            _faqService = faqService;
        }

        // GET: api/Faq
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FaqDTOs>>> GetFaqs()
        {
            var faqs = await _faqService.GetAllFaqsAsync();
            return Ok(faqs);
        }

        // GET: api/Faq/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FaqDTOs>> GetFaq(int id)
        {
            var faq = await _faqService.GetFaqByIdAsync(id);
            if (faq == null)
                return NotFound();

            return Ok(faq);
        }

        // POST: api/Faq
        [HttpPost]
        public async Task<ActionResult<FaqDTOs>> CreateFaq([FromBody] CreateFaqDTOs dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdFaq = await _faqService.CreateFaqAsync(dto);
            return CreatedAtAction(nameof(GetFaq), new { id = createdFaq.Id }, createdFaq);
        }

        // PUT: api/Faq/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFaq(int id, [FromBody] UpdateFaqDTOs dto)
        {
            if (id != dto.Id)
                return BadRequest("FAQ ID mismatch");

            var updatedFaq = await _faqService.UpdateFaqAsync(dto);
            if (updatedFaq == null)
                return NotFound();

            return Ok(updatedFaq);
        }

        // DELETE: api/Faq/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFaq(int id)
        {
            var deleted = await _faqService.DeleteFaqAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
