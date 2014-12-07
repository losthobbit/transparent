using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;

namespace Transparent.Data.Tests.Helpers
{
    public class TestData
    {
        public FakeUsersContext UsersContext { get; private set; }

        #region User Profiles

        public UserProfile Stephen = new UserProfile { UserId = 1, UserName = "Stephen", Email = "stephen@gmail.com" };
        public UserProfile Joe = new UserProfile { UserId = 2, UserName = "Joe", Email = "joe@gmail.com" };

        #endregion User Profiles

        #region Tags

        public Tag CriticalThinkingTag = new Tag { Id = 1, Name = "Critical Thinking" };
        public Tag ScubaDivingTag = new Tag { Id = 2, Name = "Scuba Diving" };

        #endregion Tags

        #region Tickets

        public Ticket JoesCriticalThinkingTicket;
        public Ticket JoesScubaDivingTicket;

        #endregion Tickets

        #region UserTags

        public UserTag StephensCriticalThinkingTag;

        #endregion UserTags

        public TestData ()
	    {
            JoesCriticalThinkingTicket = new Ticket { Id = 1, FkUserId = Joe.UserId, User = Joe, Heading = "Hello", Body = "My name is Joe" };
            JoesScubaDivingTicket = new Ticket { Id = 2, FkUserId = Joe.UserId, User = Joe, Heading = "Scuba", Body = "I like to dive" };

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
                    testData.Joe
                },
                Tickets =
                {
                    testData.JoesCriticalThinkingTicket,
                    testData.JoesScubaDivingTicket
                },
                TicketTags =
                {
                    new TicketTag 
                    {
                        Ticket = testData.JoesCriticalThinkingTicket, FkTicketId = testData.JoesCriticalThinkingTicket.Id,
                        Tag = testData.CriticalThinkingTag, FkTagId = testData.CriticalThinkingTag.Id 
                    },
                    new TicketTag 
                    {
                        Ticket = testData.JoesScubaDivingTicket, FkTicketId = testData.JoesScubaDivingTicket.Id,
                        Tag = testData.ScubaDivingTag, FkTagId = testData.ScubaDivingTag.Id 
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
                }
            };
            return testData;
        }
    }
}
