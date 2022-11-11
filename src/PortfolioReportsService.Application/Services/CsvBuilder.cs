using System;
using System.Collections.Generic;
using CsvHelper;

namespace PortfolioReportsService.Application.Services;

public class CsvBuilder<T>
{
    private readonly List<(string, Func<T, string?>, int)> _fields = new();

    public CsvBuilder<T> AddWithValue(string field, Func<T, string?> map, int maxLength = 50)
    {
        _fields.Add((field, map, maxLength));
        return this;
    }

    public CsvBuilder<T> AddBlank(string field, int maxLength = 50) => AddWithValue(field, _ => string.Empty, maxLength);

    public CsvBuilder<T> AddConst(string field, string value, int maxLength = 50) => AddWithValue(field, _ => value, maxLength);

    public void Write(CsvWriter writer, IReadOnlyCollection<T> data)
    {
        foreach (var (fieldName, _, _) in _fields)
            writer.WriteField(fieldName);

        writer.NextRecord();
        foreach (var entry in data)
        {
            foreach (var (_, fieldMapping, maxLength) in _fields)
            {
                var value = fieldMapping(entry) ?? string.Empty;
                writer.WriteField(value.Substring(0, Math.Min(value.Length, maxLength)));
            }
                
            writer.NextRecord();
        }
    }
}