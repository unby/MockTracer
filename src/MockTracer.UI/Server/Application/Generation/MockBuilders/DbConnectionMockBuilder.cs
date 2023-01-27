using System.Data;
using System.Text;
using System.Text.Json;
using MockTracer.UI.Server.Application.Generation.Common;
using MockTracer.UI.Server.Application.Watcher.Database;
using MockTracer.UI.Shared.Entity;
using DataSet = MockTracer.UI.Server.Application.Watcher.Database.DataSet;

namespace MockTracer.UI.Server.Application.Generation.MockBuilders;

public class DbConnectionMockBuilder : MockPointBuilderBase
{
  private readonly DumpOptions _arraySharpOptions = new DumpOptions()
  {
    TrimTrailingColonName = true,
    TrimInitialVariableName = true,
    DumpStyle = DumpStyle.CSharp,
    IgnoreDefaultValues = true,
    IgnoreIndexers = true,
    LineBreakChar = string.Empty,
  };
  
  public DbConnectionMockBuilder(VariableNameReslover nameReslover)
    : base(nameReslover)
  {
  }

  public override IEnumerable<LineFragment> BuildFragments(StackRow row)
  {
    var result = new List<LineFragment>(8);
    try
    {
      result.Add(BuildingConstans.Using.Line($"using Apps72.Dev.Data.DbMocker;"));
      var variable = NameReslover.CheckName($"mockDbConnection");
      var input = JsonSerializer.Deserialize<DbCommandInput>(row.Input.First().Json);

      result.Add(BuildingConstans.Prepare.Line($"var {variable} = new MockDbConnection();"));
      var outputParametr = row.Output.AddInfo != null ? JsonSerializer.Deserialize<List<MockParameter>>(row.Output.AddInfo) : null;
      var dataSet = row.Output.Name != "dataset" ? new List<DataSet>(0) : JsonSerializer.Deserialize<List<DataSet>>(row.Output.Json) ?? new List<DataSet>(0);
      if (row.Input[0].ClassName != null)
      {
        result.Add(BuildingConstans.Configure.Line($"s.SetTestDBConnectionProvider<{row.Input[0].ClassName}>({variable})"));
        result.Add(BuildingConstans.Using.Line($"using {row.Input[0].Namespace};"));
      }
      else
      {
        result.Add(BuildingConstans.Configure.Line($"/* set mockDbConnection */"));
      }

      var sb = new StringBuilder();
      var commandText = ResolveName(input.CommandText, result, "commandText");
      sb.AppendLine($"{variable}.Mocks.When(w => w.CommandText.Contains({commandText}))");

      if (outputParametr != null && outputParametr.Any())
      {
        result.Add(BuildingConstans.Using.Line($"using System.Data;"));
        outputParametr.ForEach(x => { sb.Append($".SetParameterValue({x.Name}, {x.Value}, {x.ParameterDirection})"); });
      }

      if (dataSet.Count > 1)
      {
        sb.AppendLine(".ReturnsDataset(");
        var count = 1;
        foreach (var item in dataSet)
        {
          sb.AppendLine($"\t\t\tMockTable.WithColumns({string.Join(", ", item.Header.OrderBy(o => o.Value.Index).Select(s => $"\"{s.Value.Name}\""))})");
          foreach (var dataRow in item.Data)
          {
            sb.Append(".AddRow(");
            sb.Append(ObjectDumper.Dump(dataRow, _arraySharpOptions).Trim());
            sb.Append(")");
          }
          if (count++ != dataSet.Count)
          {
            sb.Append(",");
            sb.AppendLine();
          }
        }
        sb.Append(");");
      }
      else
      {
        if (dataSet.Count == 1)
        {
          sb.Append(".ReturnsTable(");
          var item = dataSet.First();

          sb.Append($"MockTable.WithColumns(");
          sb.Append(string.Join(", ", item.Header.OrderBy(o => o.Value.Index).Select(s => $"\"{s.Value.Name}\"")));
          sb.Append(")");
          foreach (var dataRow in item.Data)
          {
            sb.Append(".AddRow(");
           sb.Append(ObjectDumper.Dump(dataRow, _arraySharpOptions));
            sb.Append(")");
          }

          sb.Append(");");
        }
        else
        {
          sb.Append($".ReturnsScalar({row.Output.ShortView});");
        }
      }

      result.Add(BuildingConstans.Prepare.Line(sb.ToString()));
    }
    catch (Exception ex)
    {
      result.Add(BuildingConstans.Prepare.Line("// faild prepare", ex));
    }

    return result;
  }
}
