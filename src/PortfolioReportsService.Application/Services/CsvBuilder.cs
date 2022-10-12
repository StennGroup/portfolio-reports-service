using System;
using System.Collections.Generic;
using CsvHelper;

namespace PortfolioReportsService.Application.Services;

public class CsvBuilder<T>
{
    private readonly List<(string, Func<T, string>)> _fields = new();

    public CsvBuilder<T> AddWithValue(string field, Func<T, string> map)
    {
        _fields.Add((field, map));
        return this;
    }

    public CsvBuilder<T> AddBlank(string field) => AddWithValue(field, _ => string.Empty);

    public CsvBuilder<T> AddConst(string field, string value) => AddWithValue(field, _ => value);

    public void Write(CsvWriter writer, IReadOnlyCollection<T> data)
    {
        foreach (var (fieldName, _) in _fields)
            writer.WriteField(fieldName);

        writer.NextRecord();
        foreach (var entry in data)
        {
            foreach (var (_, fieldMapping) in _fields)
                writer.WriteField(fieldMapping(entry));
            writer.NextRecord();
        }
    }
}