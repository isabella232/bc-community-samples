using System;
namespace WeatherInsurance.Domain.Model
{
    public enum Platform
    {
        Ethereum
    }

    public enum ContractType
    {
        HighTemperatureInsurance,
        LowTemperatureInsurance,
        HighPrecipitationInsurance,
        LowPrecipitationInsurance,
        HighHumidityInsurance,
        LowHumidityInsurance
    }

}
