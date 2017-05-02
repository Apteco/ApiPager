using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using ApiPager.Core;
using ApiPager.Core.FilterExpression;

namespace ApiPager.Data.SqlServer
{
  public static class ApiPagerSqlServerExtensions
  {
    #region public methods
    public static IDataReader ExecuteReader(this IDbCommand command, FilterPageAndSortInfo filterPageAndSortInfo, string[] filterColumns, string candidateKeyColumn)
    {
      return ExecuteReader(command, filterPageAndSortInfo, filterColumns, new string[] {candidateKeyColumn});
    }

    public static IDataReader ExecuteReader(this IDbCommand command, FilterPageAndSortInfo filterPageAndSortInfo, string[] filterColumns, string[] candidateKeyColumns)
    {
      if (filterPageAndSortInfo == null)
        return command.ExecuteReader();

      ApplyFilterPageAndSortSql(command, filterPageAndSortInfo, filterColumns, candidateKeyColumns);
      return new FilterPageAndSortDataReader(command.ExecuteReader(), filterPageAndSortInfo);
    }

    public static async Task<IDataReader> ExecuteReaderAsync(this IDbCommand command, FilterPageAndSortInfo filterPageAndSortInfo, string[] filterColumns, string candidateKeyColumn)
    {
      return await ExecuteReaderAsync(command, filterPageAndSortInfo, filterColumns, new string[] { candidateKeyColumn });
    }

    public static async Task<IDataReader> ExecuteReaderAsync(this IDbCommand command, FilterPageAndSortInfo filterPageAndSortInfo, string[] filterColumns, string[] candidateKeyColumns)
    {
      if (filterPageAndSortInfo == null)
      {
        if (command is SqlCommand)
          return await ((SqlCommand)command).ExecuteReaderAsync();

        return await Task.Run(() => command.ExecuteReader());
      }

      ApplyFilterPageAndSortSql(command, filterPageAndSortInfo, filterColumns, candidateKeyColumns);

      if (command is SqlCommand)
        return new FilterPageAndSortDataReader(await ((SqlCommand)command).ExecuteReaderAsync(), filterPageAndSortInfo);

      return new FilterPageAndSortDataReader(await Task.Run(() => command.ExecuteReader()), filterPageAndSortInfo);
    }
    #endregion

    #region private methods
    private static void ApplyFilterPageAndSortSql(IDbCommand command, FilterPageAndSortInfo filterPageAndSortInfo, string[] filterColumns, string[] candidateKeyColumns)
    {
      command.CommandText = "SELECT " + CreateSqlTopClause(filterPageAndSortInfo.Count) + " *" + Environment.NewLine +
                            "FROM (SELECT *, (ROW_NUMBER() OVER(ORDER BY " + CreateOrderByClause(filterPageAndSortInfo, candidateKeyColumns) + ") - 1) AS RowNum, COUNT(*) OVER () As TotalRows" + Environment.NewLine +
                            "      FROM (" + Environment.NewLine +
                            "            SELECT *" + Environment.NewLine +
                            "            FROM (" + Environment.NewLine +
                            command.CommandText + Environment.NewLine +
                            "                 ) AS Query" + Environment.NewLine +
                            CreateSqlFilterClause(command, filterPageAndSortInfo, filterColumns) +
                            "           ) AS FilteredQuery" + Environment.NewLine +
                            "     ) AS RowConstrainedFilteredQuery" + Environment.NewLine +
                            "WHERE RowNum >= " + filterPageAndSortInfo.Offset + Environment.NewLine +
                            "ORDER BY RowNum";
    }

    private static string CreateSqlTopClause(int count)
    {
      if (count >= 0)
        return "TOP " + count;
      else
        return "";
    }

    private static string CreateOrderByClause(FilterPageAndSortInfo filterPageAndSortInfo, string[] candidateKeyColumns)
    {
      if (string.IsNullOrEmpty(filterPageAndSortInfo.OrderByColumn) && (candidateKeyColumns.Length == 0))
        throw new ArgumentException("Must specify either an OrderByColumn in the pageAndSortInfo or at least one candidateKeyColumn");

      StringBuilder orderByClause = new StringBuilder();
      if (!string.IsNullOrEmpty(filterPageAndSortInfo.OrderByColumn))
      {
        orderByClause.Append(filterPageAndSortInfo.OrderByColumn);
        orderByClause.Append(" ").Append(filterPageAndSortInfo.Order == SortOrderEnum.Ascending ? "ASC" : "DESC").Append(", ");
      }

      orderByClause.Append(string.Join(", ", candidateKeyColumns));
      return orderByClause.ToString();
    }

    private static string CreateSqlFilterClause(IDbCommand command, FilterPageAndSortInfo filterPageAndSortInfo, string[] filterColumns)
    {
      if (string.IsNullOrEmpty(filterPageAndSortInfo.Filter))
        return "";

      StringBuilder builder = new StringBuilder();
      builder.Append("WHERE ");

      Expression expression = ExpressionParser.ParseExpression(filterPageAndSortInfo.Filter);
      string errorMessages;
      if (!expression.AreFieldsValid(filterColumns, out errorMessages))
        throw new Exception("The filter expression is not valid: "+errorMessages);

      builder.Append(new FilterExpressionSqlBuilder(command, filterColumns).CreateFilterExpression(expression));
      return builder.ToString();
    }
    #endregion
  }
}
