namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
using Transparent.Data.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<UsersContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(UsersContext context)
        {
            //  This method will be called after migrating to the latest version.

            context.Tags.AddOrUpdate(
                t => t.Name,
                new Tag
                {
                    Name = "Democratic Intelligence",
                    Description = "Use this tag for suggestions related to this site.<br/>" +
                                    "Increase your points simply by using the site."
                },
                new Tag
                {
                    Name = "Critical Thinking",
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
                                    "</ul>"+
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
                                    "<p>Also, take a look at <a href=\"https://yourlogicalfallacyis.com/\">this website</a>, which lists 24 logical fallacies with examples.</p>"

                }
            );
        }
    }
}
