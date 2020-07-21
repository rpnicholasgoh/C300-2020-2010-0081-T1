using System;
using System.Collections.Generic;
using System.Linq;

namespace FYPDraft.Models
{
    public class SchedulerSeeder
    {
        public static void Seed(SchedulerContext context)
        {
            if (context.Event.Any())
            {
                return;   // DB has been seeded
            }

            var events = new List<SchedulerEvent>()
            {
                new SchedulerEvent()
                {
                    Title = "Hiring Employees",
                    Description = "Taking the first step to growing your company and establishing reputation",
                    Venue = "Tribe Accelerator - 6, #03-308 Raffles Blvd, Marina Square, 039594",
                    StartDate = new DateTime(2020, 1, 20, 0, 0, 0),
                    EndDate = new DateTime(2020, 1, 22, 0, 0, 0),
                    Type = "Startup"
                }
            };

            events.ForEach(s => context.Event.Add(s));
            context.SaveChanges();
        }
    }
}
