using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;
using Transparent.Data.Tests.Generic;

namespace Transparent.Data.Tests.Helpers
{
    public class TestData
    {
        public FakeUsersContext UsersContext { get; private set; }

        #region User Profiles

        public UserProfile Stephen = new UserProfile { UserId = 1, UserName = "Stephen", Email = "stephen@gmail.com" };
        public UserProfile Joe = new UserProfile { UserId = 2, UserName = "Joe", Email = "joe@gmail.com" };
        public UserProfile Admin = new UserProfile { UserId = 3, UserName = "Admin", Email = "losthobbit@gmail.com" };

        #endregion User Profiles

        #region Tags

        public Tag CriticalThinkingTag = new Tag { Id = 1, Name = "Critical Thinking" };
        public Tag ScubaDivingTag = new Tag { Id = 2, Name = "Scuba Diving" };

        #endregion Tags

        #region Tickets

            #region Suggestions

            public Suggestion JoesCriticalThinkingSuggestion;
            public Suggestion JoesScubaDivingSuggestion;

            #endregion Suggestions

            #region Tests

            public Test CriticalThinkingTestThatJoeTook;
            public Test CriticalThinkingTestThatStephenTook;
            public Test ScubaDivingTestThatJoeTook;

            #endregion Tests

            #region Questions

            #endregion Questions

        #endregion Tickets

        #region UserTags

        public UserTag StephensCriticalThinkingTag;

        #endregion UserTags

        public TestData ()
	    {
            JoesCriticalThinkingSuggestion = new Suggestion { Id = 1, FkUserId = Joe.UserId, User = Joe, Heading = "Hello", Body = "My name is Joe" };
            JoesScubaDivingSuggestion = new Suggestion { Id = 2, FkUserId = Joe.UserId, User = Joe, Heading = "Scuba", Body = "I like to dive" };

            CriticalThinkingTestThatJoeTook = new Test { Id = 200, FkUserId = Admin.UserId, User = Admin, Heading = "Logical Fallacy", 
                Body = "Name the logical fallacy"};
            ScubaDivingTestThatJoeTook = new Test { Id = 201, FkUserId = Admin.UserId, User = Admin, Heading = "Breathing", 
                Body = "What do you breathe?"};
            CriticalThinkingTestThatStephenTook = new Test
            {
                Id = 203,
                FkUserId = Admin.UserId,
                User = Admin,
                Heading = "Another LF",
                Body = "Are you a logical fallacy?"
            };

            StephensCriticalThinkingTag = new UserTag
            {
                User = Stephen,
                FkUserId = Stephen.UserId,
                Tag = CriticalThinkingTag,
                FkTagId = CriticalThinkingTag.Id,
                TotalPoints = 10
            };
        }

        public static TestData Create()
        {
            var testData = new TestData();
            testData.UsersContext = new FakeUsersContext
            {
                Tags =
                {
                    testData.CriticalThinkingTag,
                    testData.ScubaDivingTag
                },
                UserProfiles =
                {
                    testData.Stephen,
                    testData.Joe,
                    testData.Admin
                },
                Tickets =
                {
                    // Suggestions
                    testData.JoesCriticalThinkingSuggestion,
                    testData.JoesScubaDivingSuggestion,
                    // Questions
                    // Tests
                    testData.CriticalThinkingTestThatJoeTook,
                    testData.CriticalThinkingTestThatStephenTook,
                    testData.ScubaDivingTestThatJoeTook
                },
                TicketTags =
                {
                    new TicketTag 
                    {
                        Ticket = testData.JoesCriticalThinkingSuggestion, FkTicketId = testData.JoesCriticalThinkingSuggestion.Id,
                        Tag = testData.CriticalThinkingTag, FkTagId = testData.CriticalThinkingTag.Id 
                    },
                    new TicketTag 
                    {
                        Ticket = testData.JoesScubaDivingSuggestion, FkTicketId = testData.JoesScubaDivingSuggestion.Id,
                        Tag = testData.ScubaDivingTag, FkTagId = testData.ScubaDivingTag.Id 
                    },
                    new TicketTag
                    {
                        Ticket = testData.CriticalThinkingTestThatJoeTook, FkTicketId = testData.CriticalThinkingTestThatJoeTook.Id,
                        Tag = testData.CriticalThinkingTag, FkTagId = testData.CriticalThinkingTag.Id
                    },
                    new TicketTag
                    {
                        Ticket = testData.ScubaDivingTestThatJoeTook, FkTicketId = testData.ScubaDivingTestThatJoeTook.Id,
                        Tag = testData.ScubaDivingTag, FkTagId = testData.ScubaDivingTag.Id
                    },
                    new TicketTag
                    {
                        Ticket = testData.CriticalThinkingTestThatStephenTook, FkTicketId = testData.CriticalThinkingTestThatStephenTook.Id,
                        Tag = testData.CriticalThinkingTag, FkTagId = testData.CriticalThinkingTag.Id
                    }
                },
                UserTags =
                {
                    testData.StephensCriticalThinkingTag,
                    new UserTag 
                    {
                        User = testData.Joe, FkUserId = testData.Joe.UserId,
                        Tag = testData.ScubaDivingTag, FkTagId = testData.ScubaDivingTag.Id,
                        TotalPoints = 10
                    }
                },
                UserPoints =
                {
                    new UserPoint
                    {
                        Id = 1, Answer = "I have no idea.", Quantity = -2, FkTagId = testData.CriticalThinkingTag.Id,
                        FkTestId = testData.CriticalThinkingTestThatJoeTook.Id, FkUserId = testData.Joe.UserId,
                        Tag = testData.CriticalThinkingTag, TestTaken = testData.CriticalThinkingTestThatJoeTook,
                        User = testData.Joe
                    },
                    new UserPoint
                    {
                        Id = 2, Answer = "Air.", Quantity = 1, FkTagId = testData.ScubaDivingTag.Id,
                        FkTestId = testData.ScubaDivingTestThatJoeTook.Id, FkUserId = testData.Joe.UserId,
                        Tag = testData.ScubaDivingTag, TestTaken = testData.ScubaDivingTestThatJoeTook,
                        User = testData.Joe
                    },
                    new UserPoint
                    {
                        Id = 2, Answer = "Yes.", Quantity = 1, FkTagId = testData.CriticalThinkingTag.Id,
                        FkTestId = testData.CriticalThinkingTestThatStephenTook.Id, FkUserId = testData.Stephen.UserId,
                        Tag = testData.CriticalThinkingTag, TestTaken = testData.CriticalThinkingTestThatStephenTook,
                        User = testData.Stephen
                    }
                }
            };
            testData.UsersContext.Suggestions = new FakeDbSet<Suggestion>(testData.UsersContext.Tickets.OfType<Suggestion>());
            testData.UsersContext.Questions = new FakeDbSet<Question>(testData.UsersContext.Tickets.OfType<Question>());
            testData.UsersContext.Tests = new FakeDbSet<Test>(testData.UsersContext.Tickets.OfType<Test>());
            foreach (var ticket in testData.UsersContext.Tickets)
            {
                ticket.TicketTags = testData.UsersContext.TicketTags.Where(ticketTag => ticketTag.Ticket == ticket).ToList();
            }

            return testData;
        }
    }
}
