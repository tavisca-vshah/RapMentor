using Bogus;
using JPMC.Hackathon.RapMentor.Contract.Models;
using System;

namespace JPMC.Hackathon.RapMentor.Mock
{
    internal class FakeCourse : Faker<Course>
    {
        public FakeCourse()
        {
            var addressFaker = new Faker<Module>()
                .UseSeed(200)
                .RuleFor(a => a.Id, _ => Guid.NewGuid().ToString())
                .RuleFor(a => a.Title, f => f.Lorem.Sentence(2))
                .RuleFor(a => a.Summary, f => f.Lorem.Sentence(2))
                .RuleFor(m => m.Content, f => f.Lorem.Sentence(10));

            UseSeed(100)
                .RuleFor(c => c.Id, _ => Guid.NewGuid().ToString())
                .RuleFor(c => c.Title, f => f.Lorem.Sentence(2))
                .RuleFor(c => c.Description, f => f.Lorem.Paragraph(2))
                .RuleFor(c => c.Level, f => f.PickRandom<CourseCategory>().ToString())
                .RuleFor(x => x.Modules, _ => addressFaker.GenerateBetween(1, 10))
                .RuleFor(c => c.CourseStatus, f => f.PickRandom<CourseStatus>().ToString())
                .RuleFor(c => c.AuthorId, _ => Guid.NewGuid().ToString());
        }
    }
}
