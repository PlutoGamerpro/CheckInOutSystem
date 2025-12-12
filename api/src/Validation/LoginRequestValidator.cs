using System;
using TimeRegistration.Contracts.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Interfaces;
using TimeRegistration.Classes;
using TimeRegistration.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TimeRegistration.Data;
using Microsoft.AspNetCore.Http;
using TimeRegistration.Contracts.Results;

namespace TimeRegistration.Validation
{
    public static class LoginRequestValidator
    {
       
        public static string ValidateOrThrow(LoginRequest req)
        {
            if (req == null)
                throw new ArgumentNullException(nameof(req), "Login request is required.");

            var username = (req.Username ?? string.Empty).Trim();
            var password = (req.Password ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Username and password are required.");

            return username;
        }
        
        public static void VerifyPasswordOrThrow(string plainPassword, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new UnauthorizedAccessException("Invalid password");
            if (!BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash))
                throw new UnauthorizedAccessException("Invalid password");
        }
    }
    }

