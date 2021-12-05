﻿using R.Systems.Auth.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace R.Systems.Auth.FunctionalTests.Models
{
    public class Users
    {
        public Dictionary<string, UserInfo> Data { get; } = new()
        {
            {
                "test@lukaszrydzkowski.pl",
                new UserInfo
                {
                    Id = 2,
                    Email = "test@lukaszrydzkowski.pl",
                    FirstName = "Testowy",
                    LastName = "Tester",
                    Password = "123123",
                    RoleKeys = new List<string>() { "admin" }
                }
            },
            {
                "test2@lukaszrydzkowski.pl",
                new UserInfo
                {
                    Id = 3,
                    Email = "test2@lukaszrydzkowski.pl",
                    FirstName = "Testowy 2",
                    LastName = "Tester 2",
                    RoleKeys = new List<string>() { "admin" }
                }
            },
            {
                "test3@lukaszrydzkowski.pl",
                new UserInfo
                {
                    Id = 4,
                    Email = "test3@lukaszrydzkowski.pl",
                    FirstName = "Testowy 3",
                    LastName = "Tester 3",
                    RefreshToken = "w2B/0+V2XNBB3V4yjKNNPU44fuGDtAg/foV37rtRjk/OhJYaPAodMc8saxVvCDbavo2yHKZWXpYSJ3XjCVo70A==",
                    RefreshTokenExpireDateTimeUtc = DateTime.UtcNow.AddMinutes(-10),
                    RoleKeys = new List<string>() { "admin" }
                }
            },
            {
                "test4@lukaszrydzkowski.pl",
                new UserInfo
                {
                    Id = 5,
                    Email = "test4@lukaszrydzkowski.pl",
                    FirstName = "Testowy 2",
                    LastName = "Tester 2",
                    Password = "e11e11"
                }
            }
        };

        public Users() { }

        public Users(IPasswordHasher passwordHasher)
        {
            foreach (KeyValuePair<string, UserInfo> element in Data)
            {
                element.Value.PasswordHash = passwordHasher.CreatePasswordHash(element.Value.Password);
            }
        }

        public List<UserInfo> Get()
        {
            return Data.Select(x => x.Value).ToList();
        }
    }
}
