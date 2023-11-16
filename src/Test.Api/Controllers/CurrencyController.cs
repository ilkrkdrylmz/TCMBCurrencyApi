using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Xml;
using Test.Api.Models.Currency;

namespace Test.Api.Controllers;

[ApiController]
[Route("api/v1/currencies")]
public class CurrencyController : ControllerBase
{
    private readonly ILogger<CurrencyController> _logger;

    public CurrencyController(ILogger<CurrencyController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetCurrenciesResponse), 200)]
    public async Task<IActionResult> Get()
    {
        var response = new GetCurrenciesResponse()
        {
            Currencies = new()
        };

        try
        {
            XmlTextReader rdr = new XmlTextReader("https://www.tcmb.gov.tr/kurlar/today.xml");
            DataSet ds = new DataSet();
            ds.ReadXml(rdr);
            var currencies = new List<string>()
            {
                "USD",
                "EUR",
                "GBP"
            };
            DataTable dt = ds.Tables["Currency"];
            foreach (var item in currencies)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][10].ToString() == item)
                    {
                        response.Currencies.Add(new GetCurrencyResponse
                        {
                            Code = item,
                            SalePrice = Convert.ToDecimal(dt.Rows[i][4].ToString().Replace(".", ",")),
                            PurchasePrice = Convert.ToDecimal(dt.Rows[i][3].ToString().Replace(".", ","))
                        });
                    }
                }
            }
        }
        catch (XmlException xml)
        {
            return BadRequest();
        }

        return Ok(response);
    }
}
