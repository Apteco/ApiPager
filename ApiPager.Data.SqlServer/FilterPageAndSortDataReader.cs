using System;
using System.Data;
using ApiPager.Core;

namespace ApiPager.Data.SqlServer
{
  public class FilterPageAndSortDataReader : IDataReader
  {
    #region private fields
    private IDataReader innerReader;
    private FilterPageAndSortInfo filterPageAndSortInfo;
    #endregion

    #region public properties
    public int FieldCount
    {
      get { return innerReader.FieldCount; }
    }

    object IDataRecord.this[string name]
    {
      get { return innerReader[name]; }
    }

    object IDataRecord.this[int i]
    {
      get { return innerReader[i]; }
    }

    public int Depth
    {
      get { return innerReader.Depth; }
    }

    public bool IsClosed
    {
      get { return innerReader.IsClosed; }
    }

    public int RecordsAffected
    {
      get { return innerReader.RecordsAffected; }
    }

    public FilterPageAndSortInfo FilterPageAndSortInfo
    {
      get { return filterPageAndSortInfo;}
    }
    #endregion

    #region public constructor
    public FilterPageAndSortDataReader(IDataReader innerReader, FilterPageAndSortInfo filterPageAndSortInfo)
    {
      this.innerReader = innerReader;
      this.filterPageAndSortInfo = filterPageAndSortInfo;
    }
    #endregion

    #region public methods
    public bool GetBoolean(int i)
    {
      return innerReader.GetBoolean(i);
    }

    public byte GetByte(int i)
    {
      return innerReader.GetByte(i);
    }

    public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
    {
      return innerReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
    }

    public char GetChar(int i)
    {
      return innerReader.GetChar(i);
    }

    public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
    {
      return innerReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
    }

    public IDataReader GetData(int i)
    {
      return innerReader.GetData(i);
    }

    public string GetDataTypeName(int i)
    {
      return innerReader.GetDataTypeName(i);
    }

    public DateTime GetDateTime(int i)
    {
      return innerReader.GetDateTime(i);
    }

    public decimal GetDecimal(int i)
    {
      return innerReader.GetDecimal(i);
    }

    public double GetDouble(int i)
    {
      return innerReader.GetDouble(i);
    }

    public Type GetFieldType(int i)
    {
      return innerReader.GetFieldType(i);
    }

    public float GetFloat(int i)
    {
      return innerReader.GetFloat(i);
    }

    public Guid GetGuid(int i)
    {
      return innerReader.GetGuid(i);
    }

    public short GetInt16(int i)
    {
      return innerReader.GetInt16(i);
    }

    public int GetInt32(int i)
    {
      return innerReader.GetInt32(i);
    }

    public long GetInt64(int i)
    {
      return innerReader.GetInt64(i);
    }

    public string GetName(int i)
    {
      return innerReader.GetName(i);
    }

    public int GetOrdinal(string name)
    {
      return innerReader.GetOrdinal(name);
    }

    public string GetString(int i)
    {
      return innerReader.GetString(i);
    }

    public object GetValue(int i)
    {
      return innerReader.GetValue(i);
    }

    public int GetValues(object[] values)
    {
      return innerReader.GetValues(values);
    }

    public bool IsDBNull(int i)
    {
      return innerReader.IsDBNull(i);
    }

    public DataTable GetSchemaTable()
    {
      return innerReader.GetSchemaTable();
    }

    public bool NextResult()
    {
      return innerReader.NextResult();
    }

    public bool Read()
    {
      bool readSuccessful = innerReader.Read();

      if ((filterPageAndSortInfo != null) && (filterPageAndSortInfo.TotalCount == null))
      {
        if (readSuccessful)
        {
          filterPageAndSortInfo.TotalCount = innerReader.GetInt32(innerReader.FieldCount - 1);
        }
        else
        {
          filterPageAndSortInfo.TotalCount = 0;
        }
      }

      return readSuccessful;
    }

    public void Dispose()
    {
      innerReader.Dispose();
    }

    public void Close()
    {
      innerReader.Close();
    }
    #endregion
  }
}
