using System.Collections;
using System.Data;
using System.Data.Common;

namespace MockTracer.UI.Server.Application.Watcher.Database;

/// <summary>
/// Tracer of SQL source
/// </summary>
public class MockerDataReader : DbDataReader, IDataReader
{
  private readonly IDataFlusher _dBConnectionTracer;
  private readonly DbDataReader _reader;

  internal DataSet CurrentResult;
  internal List<DataSet> Result;

  public MockerDataReader(DbDataReader dataReader, IDataFlusher dBConnectionTracer)
  {
    _dBConnectionTracer = dBConnectionTracer;
    _reader = dataReader;
    CurrentResult = new DataSet();
    Result = new List<DataSet>() { CurrentResult };
  }

  /// <inheritdoc/>
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

  /// <inheritdoc/>
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

  /// <inheritdoc/>
  public override int Depth => _reader.Depth;

  /// <inheritdoc/>
  public override bool IsClosed => _reader.IsClosed;

  /// <inheritdoc/>
  public override int RecordsAffected => _reader.RecordsAffected;

  /// <inheritdoc/>
  public override int FieldCount => _reader.FieldCount;

  /// <inheritdoc/>
  public override bool HasRows => _reader.HasRows;

  /// <inheritdoc/>
  public override void Close()
  {
    _reader.Close();
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    _reader?.Dispose();
    _dBConnectionTracer.FlushResult(Result);
  }

  /// <inheritdoc/>
  public override bool GetBoolean(int i)
  {
    return _reader.GetBoolean(i);
  }

  /// <inheritdoc/>
  public override byte GetByte(int i)
  {
    return _reader.GetByte(i);
  }

  /// <inheritdoc/>
  public override long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length)
  {
    return _reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
  }

  /// <inheritdoc/>
  public override char GetChar(int i)
  {
    return _reader.GetChar(i);
  }

  /// <inheritdoc/>
  public override long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length)
  {
    return _reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
  }

  /// <inheritdoc/>
  public IDataReader GetData(int i)
  {
    return _reader.GetData(i);
  }

  /// <inheritdoc/>
  public override string GetDataTypeName(int i)
  {
    return _reader.GetDataTypeName(i);
  }

  /// <inheritdoc/>
  public override DateTime GetDateTime(int i)
  {
    return _reader.GetDateTime(i);
  }

  /// <inheritdoc/>
  public override decimal GetDecimal(int i)
  {
    return _reader.GetDecimal(i);
  }

  /// <inheritdoc/>
  public override double GetDouble(int i)
  {
    return _reader.GetDouble(i);
  }

  /// <inheritdoc/>
  public override IEnumerator GetEnumerator()
  {
    return _reader.GetEnumerator();
  }

  /// <inheritdoc/>
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

  /// <inheritdoc/>
  public override float GetFloat(int i)
  {
    return _reader.GetFloat(i);
  }

  /// <inheritdoc/>
  public override Guid GetGuid(int i)
  {
    return _reader.GetGuid(i);
  }

  /// <inheritdoc/>
  public override short GetInt16(int i)
  {
    return _reader.GetInt16(i);
  }

  /// <inheritdoc/>
  public override int GetInt32(int i)
  {
    return _reader.GetInt32(i);
  }

  /// <inheritdoc/>
  public override long GetInt64(int i)
  {
    return _reader.GetInt64(i);
  }

  /// <inheritdoc/>
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

  /// <inheritdoc/>
  public override int GetOrdinal(string name)
  {
    return _reader.GetOrdinal(name);
  }

  /// <inheritdoc/>
  public override DataTable? GetSchemaTable()
  {
    return _reader.GetSchemaTable();
  }

  /// <inheritdoc/>
  public override string GetString(int i)
  {
    return _reader.GetString(i);
  }

  /// <inheritdoc/>
  public override object GetValue(int i)
  {
    return _reader.GetValue(i);
  }

  /// <inheritdoc/>
  public override int GetValues(object[] values)
  {
    return _reader.GetValues(values);
  }

  /// <inheritdoc/>
  public override bool IsDBNull(int i)
  {
    return _reader.IsDBNull(i);
  }

  /// <inheritdoc/>
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

  /// <inheritdoc/>
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
