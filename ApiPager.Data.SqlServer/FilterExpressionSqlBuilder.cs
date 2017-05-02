using System;
using System.Data;
using System.Linq;
using System.Text;
using ApiPager.Core.FilterExpression;

namespace ApiPager.Data.SqlServer
{
  public class FilterExpressionSqlBuilder
  {
    #region private fields
    private IDbCommand command;
    private string[] filterColumns;
    #endregion

    #region public constructor
    public FilterExpressionSqlBuilder(IDbCommand command, string[] filterColumns)
    {
      this.command = command;
      this.filterColumns = filterColumns;
    }
    #endregion

    #region public methods
    public string CreateFilterExpression(Expression filterExpression)
    {
      if ((filterExpression == null) || (filterExpression.RootClause == null))
        return "";

      StringBuilder builder = new StringBuilder();
      Append(filterExpression.RootClause, builder);
      return builder.ToString();
    }
    #endregion

    #region private methods
    private void Append(AbstractExpressionClause clause, StringBuilder builder)
    {
      if (clause is BinaryLogicClause)
        Append((BinaryLogicClause)clause, builder);
      else if (clause is UnaryLogicClause)
        Append((UnaryLogicClause)clause, builder);
      else if (clause is ComparisonClause)
        Append((ComparisonClause)clause, builder);
      else
        throw new Exception("Unknown type of AbstractExpressionClause: "+(clause == null? "<null> : " : clause.GetType().ToString()));
    }

    private void Append(BinaryLogicClause clause, StringBuilder builder)
    {
      string logic;
      switch (clause.LogicType)
      {
        case BinaryLogicClause.LogicTypeEnum.And:
          logic = "AND";
          break;

        case BinaryLogicClause.LogicTypeEnum.Or:
          logic = "OR";
          break;

        default:
          throw new Exception("Unknown binary LogicTyoe: "+clause.LogicType);
      }

      builder.Append("(");
      Append(clause.LeftHandSide, builder);
      builder.Append(" ").Append(logic).Append(" ");
      Append(clause.RightHandSide, builder);
      builder.Append(")");
    }

    private void Append(UnaryLogicClause clause, StringBuilder builder)
    {
      string logic;
      switch (clause.LogicType)
      {
        case UnaryLogicClause.LogicTypeEnum.Not:
          logic = "NOT";
          break;

        default:
          throw new Exception("Unknown unary LogicType: " + clause.LogicType);
      }

      builder.Append("(");
      builder.Append(logic).Append(" ");
      Append(clause.Clause, builder);
      builder.Append(")");
    }

    private void Append(ComparisonClause clause, StringBuilder builder)
    {
      string comparison;
      switch (clause.ComparisonType)
      {
        case ComparisonClause.ComparisonTypeEnum.EqualTo:
          comparison = "=";
          break;

        case ComparisonClause.ComparisonTypeEnum.NotEqualTo:
          comparison = "<>";
          break;

        case ComparisonClause.ComparisonTypeEnum.LessThan:
          comparison = "<";
          break;

        case ComparisonClause.ComparisonTypeEnum.LessThanOrEqualTo:
          comparison = "<=";
          break;

        case ComparisonClause.ComparisonTypeEnum.GreaterThan:
          comparison = ">";
          break;

        case ComparisonClause.ComparisonTypeEnum.GreaterThanOrEqualTo:
          comparison = ">=";
          break;

        case ComparisonClause.ComparisonTypeEnum.Like:
          comparison = "LIKE";
          break;

        default:
          throw new Exception("Unknown ComparisonType: " + clause.ComparisonType);
      }

      builder.Append("(");
      Append(clause.LeftHandSide, builder, false);
      builder.Append(comparison).Append(" ");
      Append(clause.RightHandSide, builder, clause.ComparisonType == ComparisonClause.ComparisonTypeEnum.Like);
      builder.Append(")");
    }

    private void Append(AbstractOperand operand, StringBuilder builder, bool performLike)
    {
      if (operand is Field)
        Append((Field)operand, builder, performLike);
      else if (operand is AbstractConstant)
        Append((AbstractConstant)operand, builder, performLike);
      else
        throw new Exception("Unknown type of AbstractOperand: " + (operand == null ? "<null> : " : operand.GetType().ToString()));
    }

    private void Append(Field field, StringBuilder builder, bool performLike)
    {
      if (performLike)
        throw new Exception("Can't perform a wildcard operation on field \""+field.FieldName+"\"");

      string filterColumn = filterColumns.FirstOrDefault(c => string.Equals(c, field.FieldName, StringComparison.OrdinalIgnoreCase));
      if (filterColumn == null)
        filterColumn = field.FieldName;

      builder.Append("[").Append(filterColumn).Append("]");
    }

    private void Append(AbstractConstant constant, StringBuilder builder, bool performLike)
    {
      if (constant is DateConstant)
        Append((DateConstant)constant, builder, performLike);
      else if (constant is IntegerConstant)
        Append((IntegerConstant)constant, builder, performLike);
      else if (constant is RealConstant)
        Append((RealConstant)constant, builder, performLike);
      else if (constant is StringConstant)
        Append((StringConstant)constant, builder, performLike);
      else
        throw new Exception("Unknown type of AbstractOperand: " + (constant == null ? "<null> : " : constant.GetType().ToString()));
    }

    private void Append(DateConstant constant, StringBuilder builder, bool performLike)
    {
      if (performLike)
        throw new Exception("Can't perform a wildcard operation on the date constant \"" + constant.Value + "\"");

      builder.Append(AddParameter(constant.Value));
    }

    private void Append(IntegerConstant constant, StringBuilder builder, bool performLike)
    {
      if (performLike)
        throw new Exception("Can't perform a wildcard operation on the integer constant \"" + constant.Value + "\"");

      builder.Append(AddParameter(constant.Value));
    }

    private void Append(RealConstant constant, StringBuilder builder, bool performLike)
    {
      if (performLike)
        throw new Exception("Can't perform a wildcard operation on the real constant \"" + constant.Value + "\"");

      builder.Append(AddParameter(constant.Value));
    }

    private void Append(StringConstant constant, StringBuilder builder, bool performLike)
    {
      if (performLike)
      {
        builder.Append("'%");
        builder.Append(EscapeForUseInSql(constant.Value));
        builder.Append("%'");
      }
      else
      {
        builder.Append(AddParameter(constant.Value));
      }
    }

    private string AddParameter(double doubleValue)
    {
      IDbDataParameter parameter = CreateAndAddParameter(doubleValue, DbType.Double);
      return parameter.ParameterName;
    }

    private string AddParameter(int intValue)
    {
      IDbDataParameter parameter = CreateAndAddParameter(intValue, DbType.Int32);
      return parameter.ParameterName;
    }

    private string AddParameter(DateTime dateTimeValue)
    {
      IDbDataParameter parameter = CreateAndAddParameter(dateTimeValue, DbType.DateTime);
      return parameter.ParameterName;
    }

    private string AddParameter(string stringValue)
    {
      IDbDataParameter parameter = CreateAndAddParameter(stringValue, DbType.String);
      parameter.Size = stringValue == null ? 0 : stringValue.Length;
      return parameter.ParameterName;
    }

    private IDbDataParameter CreateAndAddParameter(object value, DbType type)
    {
      string paramName = "@PARAM" + (command.Parameters.Count + 1);

      IDbDataParameter parameter = command.CreateParameter();
      parameter.ParameterName = paramName;
      parameter.DbType = type;
      parameter.Value = value ?? DBNull.Value;
      command.Parameters.Add(parameter);
      return parameter;
    }

    private static string EscapeForUseInSql(string s)
    {
      return s.Replace("'", "''");
    }
    #endregion
  }
}
