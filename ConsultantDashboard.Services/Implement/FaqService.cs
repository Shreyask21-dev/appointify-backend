using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Core.Entities;
using ConsultantDashboard.Infrastructure.Data;
using ConsultantDashboard.Services.IImplement;
using Microsoft.EntityFrameworkCore;

namespace ConsultantDashboard.Services
{
    public class FaqService : IFaqService
    {
        private readonly ApplicationDbContext _context;

        public FaqService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FaqDTOs>> GetAllFaqsAsync()
        {
            var faqs = await _context.Faqs.ToListAsync();

            return faqs.Select(f => new FaqDTOs
            {
                Id = f.Id,
                Question = f.Question,
                Answer = f.Answer
            }).ToList();
        }

        public async Task<FaqDTOs> GetFaqByIdAsync(int id)
        {
            var faq = await _context.Faqs.FindAsync(id);
            if (faq == null) return null;

            return new FaqDTOs
            {
                Id = faq.Id,
                Question = faq.Question,
                Answer = faq.Answer
            };
        }

        public async Task<FaqDTOs> CreateFaqAsync(CreateFaqDTOs dto)
        {
            var faq = new Faq
            {
                Question = dto.Question,
                Answer = dto.Answer
            };

            _context.Faqs.Add(faq);
            await _context.SaveChangesAsync();

            return new FaqDTOs
            {
                Id = faq.Id,
                Question = faq.Question,
                Answer = faq.Answer
            };
        }

        public async Task<FaqDTOs> UpdateFaqAsync(UpdateFaqDTOs dto)
        {
            var faq = await _context.Faqs.FindAsync(dto.Id);
            if (faq == null) return null;

            faq.Question = dto.Question;
            faq.Answer = dto.Answer;

            _context.Faqs.Update(faq);
            await _context.SaveChangesAsync();

            return new FaqDTOs
            {
                Id = faq.Id,
                Question = faq.Question,
                Answer = faq.Answer
            };
        }

        public async Task<bool> DeleteFaqAsync(int id)
        {
            var faq = await _context.Faqs.FindAsync(id);
            if (faq == null) return false;

            _context.Faqs.Remove(faq);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
