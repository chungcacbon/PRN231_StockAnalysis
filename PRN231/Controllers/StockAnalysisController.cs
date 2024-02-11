using Microsoft.AspNetCore.Mvc;
using PRN231.Models;
using System.Text;
using Newtonsoft.Json;
using PRN231.Models.Requests;

namespace PRN231.Controllers;
[Route("[controller]/api")]
[ApiController]
public class StockAnalysisController : ControllerBase
{
    private readonly IHttpClientFactory _clientFactory;

    public StockAnalysisController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet("export/{id}")]
    public async Task<IActionResult> ExportAsync(string id)
    {
        void WriteToExcel(MemoryStream stream, List<Stock> stocks)
        {
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            {
                writer.WriteLine("Id,Symbol,MC,C,F,LastPrice,LastVolume,Lot,ot,ChangePc,AvePrice,HighPrice,LowPrice,fBVol,fBValue,fSVolume,fSValue,fRoom,g1,g2,g3,g4,g5,g6,g7,mp,CWUnderlying,CWIssuerName,CWType,CWMaturityDate,CWLastTradingDate,CWExcersisePrice,CWExerciseRatio,CWListedShare,sType,sBenefit");

                foreach (var stock in stocks)
                {
                    writer.WriteLine($"{stock.Id},{stock.Sym},{stock.Mc}," +
                        $"{stock.C},{stock.F},{stock.LastPrice},{stock.LastVolume}," +
                        $"{stock.Lot},{stock.Ot},{stock.ChangePc},{stock.AvePrice},{stock.HighPrice}," +
                        $"{stock.LowPrice},{stock.FBVol},{stock.FBValue},{stock.FSVolume},{stock.FSValue}," +
                        $"{stock.FRoom},{stock.G1},{stock.G2},{stock.G3},{stock.G4},{stock.G5},{stock.G6}" +
                        $",{stock.G7},{stock.Mp},{stock.CWUnderlying},{stock.CWIssuerName},{stock.CWType}" +
                        $",{stock.CWMaturityDate},{stock.CWLastTradingDate},{stock.CWExcersisePrice},{stock.CWExerciseRatio},{stock.CWListedShare},{stock.SType},{stock.SBenefit}");
                }
            }
        }
        var client = _clientFactory.CreateClient();
        var response = await client.GetAsync($"https://bgapidatafeed.vps.com.vn/getliststockdata/{id}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var stocks = JsonConvert.DeserializeObject<Stock[]>(content);
            if (stocks == null || stocks.Length == 0)
            {
                return BadRequest("id not found");
            }
            MemoryStream stream = new MemoryStream();

            WriteToExcel(stream, stocks.ToList());

            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "stock_data.csv");
        }
        else
        {
            return StatusCode((int)response.StatusCode, "Failed to get data from the API.");
        }
    }

    [HttpPost("compare")]
    public async Task<IActionResult> Compare(CompareRequest request)
    {
        var client = _clientFactory.CreateClient();
        var response = await client.GetAsync($"https://bgapidatafeed.vps.com.vn/getliststockdata/{request.FirstCode},{request.SecondCode}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var stocks = JsonConvert.DeserializeObject<Stock[]>(content);
            if (stocks == null || stocks.Length == 0)
            {
                return BadRequest("id not found");
            }
            return Ok(stocks);
        }
        else
        {
            return StatusCode((int)response.StatusCode, "Failed to get data from the API.");
        }
    }
}
