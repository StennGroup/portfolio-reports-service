using System;
using System.Globalization;
using System.Text;
using CsvHelper.Configuration;

namespace PortfolioReportsService.Application.Services;

public static class AtradiusFileConfig
{
    public static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false);

    public const string DateFormat = "YYYYMMDD";

    public static readonly CsvConfiguration Config = new (CultureInfo.InvariantCulture)
    {
        Delimiter = "\t",
        Encoding = Utf8WithoutBom,
    };
}