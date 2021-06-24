using System;
using System.Runtime.Serialization;

namespace Api.Models.Constants
{
    public static class StringConstants
    {
        public static readonly string ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "Server=localhost;port=5432;database=eximmadb";
            

        public static readonly string JwtKey = "JwtKey";
        public static readonly string JwtKeyDefault = "PersonaJWTDeaultKey2020&411%#@3";
        public const string API_DESCRIBTION = "Eximma Backend API";
        public const string DEVELOPER_NAME = "Innovantics LTD";
        public const string DEVELOPER_EMAIL = "dev@innovantics.com";
#pragma warning disable S1075 // URIs should not be hardcoded
        public const string DEVELOPER_URL = "http://innovantics.com";
#pragma warning restore S1075 // URIs should not be hardcoded
        public const string API_TITTLE = "Eximma";

        #region Roles
        public const string SUPER_ADMIN = "superAdmin";
        #endregion
    }
}