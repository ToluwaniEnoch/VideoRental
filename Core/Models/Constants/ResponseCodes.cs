namespace Api.Models.Constants
{
    public static class ResponseCodes
    {
        public const int Success = 0;
        public const int Unauthenticated = 49;
        public const int BadRequest = 89;
        public const int NoPermission = 79;
        public const int ResourceAlreadyExist = 60;
        public const int ResourceNoLongerInuse = 65;
        public const int NoData = 69;
        public const int ServiceError = 99;
        public const int InvalidToken = 40;
    }
}