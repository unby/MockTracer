using System.Collections;
using System.Data;
using System.Data.Common;

namespace MockTracer.UI.Server.Application.Watcher.Database;

public class MockerDataReader : DbDataReader, IDataReader
{
  private readonly IReadFinisher _dBConnectionTracer;
  private readonly DbDataReader _reader;

  internal DataSet CurrentResult;
  internal List<DataSet> Result;

  public MockerDataReader(DbDataReader dataReader, IReadFinisher dBConnectionTracer)
  {
    _dBConnectionTracer = dBConnectionTracer;
    _reader = dataReader;
    CurrentResult = new DataSet();
    Result = new List<DataSet>() { CurrentResult };
  }

  public override object this[int i]
  {
    get
    {
      var value = _reader[i];
      try
      {
        CurrentResult.Row[i] = value;
      }
      catch (IndexOutOfRangeException)
      {
        CurrentResult.RefreshtRow();
        CurrentResult.Row[i] = value;
      }
      return value;
    }
  }

  public override object this[string name]
  {
    get
    {
      var value = _reader[name];
      try
      {
        CurrentResult.Row[CurrentResult.GetIndexByName(name)] = value;
      }
      catch (IndexOutOfRangeException)
      {
        CurrentResult.RefreshtRow();
        CurrentResult.Row[CurrentResult.GetIndexByName(name)] = value;
      }
      return value;
    }
  }

  public override int Depth => _reader.Depth;

  public override bool IsClosed => _reader.IsClosed;

  public override int RecordsAffected => _reader.RecordsAffected;

  public override int FieldCount => _reader.FieldCount;

  public override bool HasRows => _reader.HasRows;

  public override void Close()
  {
    _reader.Close();
  }

  public void Dispose()
  {
    _reader?.Dispose();
    _dBConnectionTracer.AddResult(Result);
  }

  public override bool GetBoolean(int i)
  {
    return _reader.GetBoolean(i);
  }

  public override byte GetByte(int i)
  {
    return _reader.GetByte(i);
  }

  public override long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length)
  {
    return _reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
  }

  public override char GetChar(int i)
  {
    return _reader.GetChar(i);
  }

  public override long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length)
  {
    return _reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
  }

  public IDataReader GetData(int i)
  {
    return _reader.GetData(i);
  }

  public override string GetDataTypeName(int i)
  {
    return _reader.GetDataTypeName(i);
  }

  public override DateTime GetDateTime(int i)
  {
    return _reader.GetDateTime(i);
  }

  public override decimal GetDecimal(int i)
  {
    return _reader.GetDecimal(i);
  }

  public override double GetDouble(int i)
  {
    return _reader.GetDouble(i);
  }

  public override IEnumerator GetEnumerator()
  {
    return _reader.GetEnumerator();
  }

  public override Type GetFieldType(int i)
  {
    if (CurrentResult.Header.TryGetValue(i, out var info))
    {
      if (info.Type == null)
      {
        var type = _reader.GetFieldType(i);
        info.TypeFullName = type.FullName;
        info.Type = type;
      }

      return info.Type;
    }
    else
    {
      var type = _reader.GetFieldType(i);
      CurrentResult.Header.Add(i, new ColumnInfo() { Index = i, Type = type, TypeFullName = type.FullName });
      return type;
    }
  }

  public override float GetFloat(int i)
  {
    return _reader.GetFloat(i);
  }

  public override Guid GetGuid(int i)
  {
    return _reader.GetGuid(i);
  }

  public override short GetInt16(int i)
  {
    return _reader.GetInt16(i);
  }

  public override int GetInt32(int i)
  {
    return _reader.GetInt32(i);
  }

  public override long GetInt64(int i)
  {
    return _reader.GetInt64(i);
  }

  public override string GetName(int i)
  {
    if (CurrentResult.Header.TryGetValue(i, out var info))
    {
      if (info.Name == null)
      {
        var name = _reader.GetName(i);
        info.Name = name;
      }

      return info.Name;
    }
    else
    {
      var name = _reader.GetName(i);
      CurrentResult.Header.Add(i, new ColumnInfo() { Index = i, Name = name });
      return name;
    }
  }

  public override int GetOrdinal(string name)
  {
    return _reader.GetOrdinal(name);
  }

  public override DataTable? GetSchemaTable()
  {
    return _reader.GetSchemaTable();
  }

  public override string GetString(int i)
  {
    return _reader.GetString(i);
  }

  public override object GetValue(int i)
  {
    return _reader.GetValue(i);
  }

  public override int GetValues(object[] values)
  {
    return _reader.GetValues(values);
  }

  public override bool IsDBNull(int i)
  {
    return _reader.IsDBNull(i);
  }

  public override bool NextResult()
  {
    var hasNextResult = _reader.NextResult();
    if (hasNextResult)
    {
      CurrentResult = new DataSet();
      Result.Add(CurrentResult);
    }
    return hasNextResult;
  }

  public override bool Read()
  {
    var hasRow = _reader.Read();
    if (hasRow)
    {
      CurrentResult.NextRow();
    }

    return hasRow;
  }
}
