namespace Transparent.Data.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Transparent.Data.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<UsersContext>
    {
        private const string BasicPsychologyTagName = "Basic Psychology";

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(UsersContext context)
        {
            //  This method will be called after migrating to the latest version.

            AddTags(context);

            SetupTestUser(context);
        }

        private void SetupTestUser(UsersContext context)
        {
            var stephen = context.UserProfiles.SingleOrDefault(user => user.Email == "losthobbit@gmail.com");

            if (stephen != null)
            {
                var criticalThinkingTag = context.Tags.Single(tag => tag.Name == Constants.CriticalThinkingTagName);
                AddOrUpdateUserTagPoints(context, stephen, criticalThinkingTag, 30);

                var applicationTag = context.Tags.Single(tag => tag.Name == Constants.ApplicationName);
                AddOrUpdateUserTagPoints(context, stephen, applicationTag, 40);

                var basicPsychologyTag = context.Tags.Single(tag => tag.Name == BasicPsychologyTagName);
                AddOrUpdateUserTagPoints(context, stephen, basicPsychologyTag, 20);
            }
        }

        private void AddOrUpdateUserTagPoints(UsersContext context, UserProfile user, Tag tag, int totalPoints)
        {
            var userTag = context.UserTags.SingleOrDefault(ut => ut.User.Email == user.Email && ut.FkTagId == tag.Id);
            if (userTag == null)
            {
                context.UserTags.Add(new UserTag { User = user, Tag = tag, TotalPoints = totalPoints });
            }
            else
                userTag.TotalPoints = totalPoints;
        }

        private void AddTags(UsersContext context)
        {
            var criticalThinkingTag = new Tag
            {
                Name = Constants.CriticalThinkingTagName,
                Description = "<p>The broad topic of critical thinking includes subjects like " +
                                "the scientific method, open mindedness, logical fallacies, etc.  " +
                                "Many books have been written on methods of thinking that have taken " +
                                "humans thousands of years to realize.  The application of critical " +
                                "thinking is essential to this site in order to keep the information and decisions " +
                                "as accurate and unbiased as possible.  All other tags are based upon this " +
                                "one, meaning that until a person has studied critical thinking, they cannot " +
                                "progress to any other topics within this site.</p>" +
                                "<p>If you have never read a book on critical thinking then I believe you are in " +
                                "for a fantastic surprise; Statistically, it is highly likely that the world, as you know it, is not exactly as you " +
                                "believe it to be.  After learning to apply critical thinking you will see things " +
                                "differently, realizing that the way you used to think was immature and fuzzy, and " +
                                "that what you thought was logic, relied too heavily on your emotions.</p>" +
                                "<p>In order to learn more about critical thinking, I'd recommend reading some (or all) of " +
                                "the following books:</p><ul>" +
                                "<li><a href=\"http://www.schoolofthinking.org/software.pdf\">Software for your Brain</a>, or the updated version, " +
                                "English Thinking: The Three Methods, by Michael Hewitt-Gleeson</li>" +
                                "<li>Your Deceptive Mind: A Scientific Guide to Critical Thinking Skills, by Steven Novella (audio book)</li>" +
                                "<li><a href=\"http://www.youtube.com/watch?v=hGkfs9WU98s\">The Demon Haunted World</a> by Carl Sagan</li>" +
                                "<li>Mistakes Were Made (but not by me) by Carol Tavris, Elliot Aronson</li>" +
                                "</ul>" +
                                "There are also numerous videos and articles related to critical thinking on the internet.  Here are some examples:<br/>" +
                                "Articles<br/>" +
                                "<ul>" +
                                "<li><a href=\"http://www.forbes.com/sites/emilywillingham/2012/11/08/10-questions-to-distinguish-real-from-fake-science/</li>\">10 questions to distinguish real from fake science.</a></li>" +
                                "<li><a href=\"http://www.usatoday.com/story/news/nation/2013/06/18/how-to-spot-a-quack/2429471/\">How to spot a quack</a></li>" +
                                "</ul>" +
                                "Videos<br/>" +
                                "<ul>" +
                                "<li><a href=\"https://www.youtube.com/watch?v=6OLPL5p0fMg\">Critical thinking</a></li>" +
                                "<li><a href=\"https://www.youtube.com/watch?v=T69TOuqaqXI\">Open mindedness</a></li>" +
                                "<li><a href=\"https://www.youtube.com/watch?v=KayBys8gaJY\">Burdon of proof</a></li>" +
                                "<li><a href=\"https://www.youtube.com/watch?v=8T_jwq9ph8k\">Why do people believe wierd things</a></li>" +
                                "<li><a href=\"https://www.youtube.com/watch?v=hJmRbSX8Rqo\">Baloney detection kit</a></li>" +
                                "</ul>" +
                                "<p>Also, take a look at <a href=\"https://yourlogicalfallacyis.com/\">this website</a>, which lists 24 logical fallacies with examples.</p>",
            };

            var applicationTag = new Tag
            {
                Name = Constants.ApplicationName,
                Description = "Use this tag for questions and suggestions related to this system.<br/>" +
                                "Increase your points by using the site positively and volunteering."
            };

            var shelterTag = new Tag
            {
                Name = "Shelter",
                Description = "Use this tag for suggestions relating to shelter."
            };

            var architectureTag = new Tag
            {
                Name = "Architecture",
                Description = "Use this tag for suggestions relating to architecture."
            };

            shelterTag.Children = new List<Tag> { architectureTag };

            var basicPsychologyTag = new Tag
            {
                Name = BasicPsychologyTagName,
                Description = "Use this tag for suggestions relating to basic psychology."
            };

            var socialSystemsTag = new Tag
            {
                Name = "Social Systems",
                Description = "Use this tag for suggestions to improve social systems."
            };

            basicPsychologyTag.Children = new List<Tag> { socialSystemsTag };

            var subTags = new List<Tag>()
            {
                applicationTag,
                new Tag
                {
                    Name = "Promotion",
                    Description = "Use this tag for suggestions related to promotion of the goals of " + Constants.ApplicationName + "."
                },
                new Tag
                {
                    Name = "Health",
                    Description = "Use this tag for suggestions relating to health."
                },
                new Tag
                {
                    Name = "Technology",
                    Description = "Use this tag for suggestions relating to technology."
                },
                new Tag
                {
                    Name = "Education",
                    Description = "Use this tag for suggestions relating to education."
                },
                new Tag
                {
                    Name = "Environment",
                    Description = "Use this tag for suggestions relating to education."
                },
                new Tag
                {
                    Name = "Food",
                    Description = "Use this tag for suggestions relating to food."
                },
                new Tag
                {
                    Name = "Water",
                    Description = "Use this tag for suggestions relating to water."
                },
                new Tag
                {
                    Name = "Safety",
                    Description = "Use this tag for suggestions relating to safety."
                },
                new Tag
                {
                    Name = "Peace",
                    Description = "Use this tag for suggestions relating to peace."
                },
                shelterTag,
                basicPsychologyTag
            };

            criticalThinkingTag.Children = subTags;

            context.Tags.AddOrUpdate(
                t => t.Name,
                criticalThinkingTag
            );

            context.SaveChanges();
        }
    }
}
