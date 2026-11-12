using System.ComponentModel;
using System.Text.Json;
using GenerativeAI_dotnet.Configurations;
using Microsoft.Extensions.Configuration;

namespace GenerativeAI_dotnet.Tools;

public class DeviceSpecsTools
{
    private readonly HttpClient _httpClient;
    private readonly DeviceSpecsApiSettings _deviceSpecsApiSettings;

    public DeviceSpecsTools(HttpClient httpClient, IConfiguration configuration, string environment)
    {
        _httpClient = httpClient;

        _deviceSpecsApiSettings = configuration
            .GetSection(nameof(DeviceSpecsApiSettings))
            .Get<DeviceSpecsApiSettings>()
            ?? throw new InvalidOperationException($"{nameof(DeviceSpecsApiSettings)} section is missing in appsettings.{environment}.json");
    }

    /// <summary>
    /// Helper function/tool for AI to access device specification information.
    /// </summary>
    /// <param name="deviceName">The name of the device.</param>
    /// <returns>The specifications of the device.</returns>
    [Description("Get the specifications of a device")]
    public async Task<string> GetDeviceSpecsAsync(string deviceName)
    {
        try
        {
            using HttpResponseMessage response = await _httpClient.GetAsync(
                $"{_deviceSpecsApiSettings.BaseUrl}/devices/search/?name={deviceName}&page=1&limit=1&key={_deviceSpecsApiSettings.ApiKey}");

            var deviceSearch = JsonSerializer.Deserialize<DeviceSearch>(
                await response.Content.ReadAsStringAsync(),
                JsonDefaults.SnakeCaseLower);

            return
                $"""
                Device Name: {deviceSearch.Devices?[0].Name}
                Manufacturer: {deviceSearch.Devices?[0].ManufacturerName}
                Device Type: {deviceSearch.Devices?[0].DeviceType}
                Description: {deviceSearch.Devices?[0].Description}
                Colors: {deviceSearch.Devices?[0].Colors}
                Storage: {deviceSearch.Devices?[0].Storage}
                Screen Resolution: {deviceSearch.Devices?[0].ScreenResolution}
                Weight: {deviceSearch.Devices?[0].Weight}
                Thickness: {deviceSearch.Devices?[0].Thickness}
                Release Date: {deviceSearch.Devices?[0].ReleaseDate}
                Camera: {deviceSearch.Devices?[0].Camera}
                Battery Capacity: {deviceSearch.Devices?[0].BatteryCapacity}
                Hardware: {deviceSearch.Devices?[0].Hardware}
                Model Numbers: {deviceSearch.Devices?[0].ModelNumbers}
                """;
        }
        catch (Exception)
        {
            return "The device specifications cannot be retrieved at this time. Please try again later.";
        }
    }
}
