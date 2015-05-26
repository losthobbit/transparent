namespace Transparent.Data.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Security;
    using Transparent.Data.Models;
    using WebMatrix.WebData;

    internal sealed class Configuration : DbMigrationsConfiguration<UsersContext>
    {
        private const string BasicPsychologyTagName = "Basic Psychology";
        private const string ShelterTagName = "Shelter";
        private const string ArchitectureTagName = "Architecture";
        private const string PromotionTagName = "Promotion";
        private const string HealthTagName = "Health";
        private const string TechnologyTagName = "Technology";
        private const string EducationTagName = "Education";
        private const string EnvironmentTagName = "Environment";
        private const string FoodTagName = "Food";
        private const string WaterTagName = "Water";
        private const string SafetyTagName = "Safety";
        private const string PeaceTagName = "Peace";

        public Configuration()
        {
            Security.InitializeDatabase();
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(UsersContext context)
        {
            //  This method will be called after migrating to the latest version.
            AddTags(context);
            CreateRoles();
            var administrator = SetupAdministrator(context);
            if (administrator != null)
            {
                CreateTests(context, administrator);
            }
            SetupTestUser(context);
        }

        private void CreateRoles()
        {
            if (!Roles.RoleExists(Constants.AdministratorRole))
                Roles.CreateRole(Constants.AdministratorRole);
            if (!Roles.RoleExists(Constants.VolunteerRole))
                Roles.CreateRole(Constants.VolunteerRole);
        }

        private void CreateTests(UsersContext context, UserProfile testUser)
        {
            var criticalThinkingTag = context.Tags.Single(tag => tag.Name == Constants.CriticalThinkingTagName);
            var body = "Name a logical fallacy in the following sentence:\n\n" +
                    "I eat herbs because my grandfather ate herbs and lived for a hundred years.";
            if(!context.Tests.Any(test => test.Body == body))
                context.Tests.Add(new Test
                {
                    User = testUser,
                    CreatedDate = DateTime.Now,
                    Heading = "Name the logical fallacy",
                    Body = body,
                    TicketTags = new Collection<TicketTag> { new TicketTag { Tag = criticalThinkingTag } },
                    State = TicketState.Completed
                });
            body = "Name or explain a logical fallacy in the following:\n\n" +
                    "Either chemotherapy or a miracle cured his cancer.  He didn't have chemotherapy.  Therefore it was a miracle.";
            if (!context.Tests.Any(test => test.Body == body))
                context.Tests.AddOrUpdate(t => t.Body, new Test
                {
                    User = testUser,
                    CreatedDate = DateTime.Now,
                    Heading = "Name or explain the logical fallacy",
                    Body = body,
                    TicketTags = new Collection<TicketTag> { new TicketTag { Tag = criticalThinkingTag } },
                    State = TicketState.Completed
                });

            var promotionTag = context.Tags.Single(tag => tag.Name == PromotionTagName);
            body = "Write what you would consider to be the two most important characteristics of a video, in order to make it go viral.";
            if (!context.Tests.Any(test => test.Body == body))
                context.Tests.Add(new Test
                {
                    User = testUser,
                    CreatedDate = DateTime.Now,
                    Heading = "Viral videos",
                    Body = body,
                    TicketTags = new Collection<TicketTag> { new TicketTag { Tag = promotionTag } },
                    State = TicketState.Completed
                });
        }

        private UserProfile SetupAdministrator(UsersContext context)
        {
            const string adminEmail = "losthobbit@gmail.com";
            const string adminName = "Stephen Oberauer";
            var adminstrator = context.UserProfiles.SingleOrDefault(user => user.Email == adminEmail);

            if(adminstrator == null)
            {
                WebSecurity.CreateUserAndAccount(adminName, "password", new { Email = adminEmail });
                adminstrator = context.UserProfiles.SingleOrDefault(user => user.Email == adminEmail);
            }

            var criticalThinkingTag = context.Tags.Single(tag => tag.Name == Constants.CriticalThinkingTagName);
            AddOrUpdateUserTagPoints(context, adminstrator, criticalThinkingTag, 30);

            var applicationTag = context.Tags.Single(tag => tag.Name == Constants.ApplicationName);
            AddOrUpdateUserTagPoints(context, adminstrator, applicationTag, 40);

            var basicPsychologyTag = context.Tags.Single(tag => tag.Name == BasicPsychologyTagName);
            AddOrUpdateUserTagPoints(context, adminstrator, basicPsychologyTag, 20);

            if (!Roles.GetRolesForUser(adminName).Contains(Constants.AdministratorRole))
                Roles.AddUserToRole(adminName, Constants.AdministratorRole);

            return adminstrator;
        }

        private UserProfile SetupTestUser(UsersContext context)
        {
            const string testEmail = "nerdsguide@gmail.com";
            var testUser = context.UserProfiles.SingleOrDefault(user => user.Email == testEmail);

            if (testUser == null)
            {
                WebSecurity.CreateUserAndAccount("Nerd", "password", new { Email = testEmail });
                testUser = context.UserProfiles.SingleOrDefault(user => user.Email == testEmail);
            }

            var criticalThinkingTag = context.Tags.Single(tag => tag.Name == Constants.CriticalThinkingTagName);
            AddOrUpdateUserTagPoints(context, testUser, criticalThinkingTag, 30);

            var applicationTag = context.Tags.Single(tag => tag.Name == Constants.ApplicationName);
            AddOrUpdateUserTagPoints(context, testUser, applicationTag, 40);

            var basicPsychologyTag = context.Tags.Single(tag => tag.Name == BasicPsychologyTagName);
            AddOrUpdateUserTagPoints(context, testUser, basicPsychologyTag, 20);

            return testUser;
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
                                "<li><a href=\"https://www.youtube.com/watch?v=KayBys8gaJY\">Burden of proof</a></li>" +
                                "<li><a href=\"https://www.youtube.com/watch?v=8T_jwq9ph8k\">Why do people believe weird things</a></li>" +
                                "<li><a href=\"https://www.youtube.com/watch?v=hJmRbSX8Rqo\">Baloney detection kit</a></li>" +
                                "</ul>" +
                                "<p>Also, take a look at <a href=\"https://yourlogicalfallacyis.com/\">this website</a>, which lists 24 logical fallacies with examples.</p>",
            };

            var applicationTag = new Tag
            {
                Name = Constants.ApplicationName,
                Description = "<p>Use this tag for questions and suggestions related to this system.  " +
                                "Increase your points by using the site positively and volunteering.  " +
                                "Here are some of the ways that you can earn points:</p>" + 
                                "<ul>" + 
                                "<li>Create your first ticket</li>" + 
                                "<li>Answer your first test</li>" +
                                "<li>Mark your first test</li>" +
                                "<li>Write your first answer to a question or argument for a suggestion</li>" +
                                "<li>Create suggestion which are accepted and completed</li>" +
                                "<li>Volunteer</li>"
            };

            var shelterTag = new Tag
            {
                Name = ShelterTagName,
                Description = "Use this tag for suggestions relating to shelter."
            };

            var architectureTag = new Tag
            {
                Name = ArchitectureTagName,
                Description = "Use this tag for suggestions relating to architecture."
            };

            shelterTag.Children = new List<Tag> { architectureTag };

            var basicPsychologyTag = new Tag
            {
                Name = BasicPsychologyTagName,
                Description = "Use this tag for suggestions relating to basic psychology."
            };

            var subTags = new List<Tag>()
            {
                applicationTag,
                new Tag
                {
                    Name = PromotionTagName,
                    Description = "<p>Use this tag for suggestions related to promotion of the goals of " + Constants.ApplicationName + ".  " +
                        "In order to earn points about promotion you need to answer or mark tests related to promoting ideas.  " +
                        "I don't know of any particularly good books on the subject, so feel free to suggest some.  " +
                        "Until we have a specific list of books, and while the site is still immature, you can probably " +
                        "simply lookup the answers to the questions while you answer them.  If, however, you'd like to " + 
                        "be more prepared, you can read up on the internet about viral videos, and internet advertising " +
                        "options, like Google Adwords.</p>"
                },
                new Tag
                {
                    Name = HealthTagName,
                    Description = "Use this tag for suggestions relating to health."
                },
                new Tag
                {
                    Name = TechnologyTagName,
                    Description = "Use this tag for suggestions relating to technology."
                },
                new Tag
                {
                    Name = EducationTagName,
                    Description = "Use this tag for suggestions relating to education."
                },
                new Tag
                {
                    Name = EnvironmentTagName,
                    Description = "Use this tag for suggestions relating to education."
                },
                new Tag
                {
                    Name = FoodTagName,
                    Description = "Use this tag for suggestions relating to food."
                },
                new Tag
                {
                    Name = WaterTagName,
                    Description = "Use this tag for suggestions relating to water."
                },
                new Tag
                {
                    Name = SafetyTagName,
                    Description = "Use this tag for suggestions relating to safety."
                },
                new Tag
                {
                    Name = PeaceTagName,
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
