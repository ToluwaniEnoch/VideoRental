using Api.Models.Payloads;
using Api.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services
{

    public interface IPriceCalculatorService
    {
        Task<ApiResponse<PriceCalculatorResponse>> CalculatePrice(PriceCalculatorPayload payload);
    }
}
