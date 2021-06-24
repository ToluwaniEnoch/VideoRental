using System;
using System.Runtime.Serialization;

namespace Api.Models.Constants
{
    public static class StringConstants
    {
        public static readonly string ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "Server=localhost;port=5432;database=VideoRentalsDB;User ID =postgres;Password=password123;IntegratedSecurity=true";
            

        public static readonly string JwtKey = "JwtKey";
        public static readonly string JwtKeyDefault = "PersonaJWTDeaultKey2020&411%#@3";
        public const string API_DESCRIBTION = "VideoRentals Backend API";
        public const string DEVELOPER_NAME = "Toluwani Idowu";
        public const string DEVELOPER_EMAIL = "toluademola74@gmail.com";
        public const string API_TITTLE = "VideoRentals";
        public const decimal REGULAR_RATE = 10;
        public const decimal CHILDREN_RATE = 8;
        public const decimal NEW_RELEASE_RATE = 15;

        #region Roles
        public const string SUPER_ADMIN = "superAdmin";
        #endregion
    }
}