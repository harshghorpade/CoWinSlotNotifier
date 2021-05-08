// ===========================================
// CoWin Background Listener Application class 
// ===========================================

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using CoWinService.Domain.Interfaces;

namespace CoWinService.Application
{
    public class CoWinListener : BackgroundService
    {
        private ICoWinServer _coWinServer;
        List<string> pincodes;
        List<string> districtCodes;
        const int NUMBER_OF_DAYS = 2;
        // Constructor
        public CoWinListener(ICoWinServer coWinServer)
        {
            _coWinServer = coWinServer;
            pincodes = new List<string>();
            districtCodes = new List<string>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // prepare the pincode and districts data
                PreparePincodeAndDistrictData();
                var districts = districtCodes.ToArray();
                var pinCodes = pincodes.ToArray();
                while (!stoppingToken.IsCancellationRequested)
                {
                    DateTime dateTime = DateTime.UtcNow.Date;

                    // check for two days (today and next 1 day) vaccination slots from current date
                    // one can customize the number of days for search by changing value of NUMBER_OF_DAYS
                    // but if you change NUMBER_OF_DAYS be sure to calculate and increase the wait period below
                    // otherwise more frequently API calls will get rejected by CoWin API server
                    for (int day =0; day < NUMBER_OF_DAYS; day++)
                    {
                        var date = dateTime.AddDays(day);
                        await Task.WhenAll(districts.Select(district => Task.Run(async () => {
                            await _coWinServer.SearchByDistrict(district, date.ToString("ddMMyyyy"));
                        })));
                        // use _coWinServer.SearchByPincode(pincode,date); call if you want to search by specific pincode
                    }
                    // These APIs have a rate limit of 100 API calls per 5 minutes per IP
                    // so added wait for 45 seconds between two GET calls to put some more buffer
                    Thread.Sleep(45000);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error : {exception.Message}");
            }
        }

        private void PreparePincodeAndDistrictData()
        {
            // Add or remove more pincode here
            pincodes.Add("411038");     // Kothrud

            // Add or remove more districts here
            districtCodes.Add("363");   // pune
            districtCodes.Add("373");   // sangli
            // Some more district codes for reference
            // Satara   : 376
            // Mumbai   : 395
            // Thane    : 392
            // Kolhapur : 371
        }
    }
}
