using Microsoft.EntityFrameworkCore;

namespace FYPDraft.Models
{
    public class SchedulerContext : DbContext
    {
        public SchedulerContext(DbContextOptions<SchedulerContext> options)
            : base(options)
        {
        }
        public DbSet<SchedulerEvent> Event { get; set; }
    }
}
