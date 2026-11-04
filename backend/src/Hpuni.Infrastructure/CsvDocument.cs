using System.Globalization;
using System.Text;

namespace Hpuni.Infrastructure;

public static class CsvDocument
{
    public static string Cell(object? value)
    {
        var s=value is decimal d?d.ToString("G29",CultureInfo.InvariantCulture):value is IFormattable f?f.ToString(null,CultureInfo.InvariantCulture):value?.ToString()??"";
        var first=s.FirstOrDefault(c=>!char.IsWhiteSpace(c)&&!char.IsControl(c)&&char.GetUnicodeCategory(c)!=UnicodeCategory.Format);
        if(first is '=' or '+' or '-' or '@')s="'"+s;
        return "\""+s.Replace("\"","\"\"")+"\"";
    }
    public static byte[] Create(IEnumerable<object?[]> rows)=>Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(string.Join("\r\n",rows.Select(r=>string.Join(",",r.Select(Cell))))+"\r\n")).ToArray();
}
