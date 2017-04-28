using System;
using System.Globalization;
using Sprache;

namespace ApiPager.Core.FilterExpression
{
  public static class ExpressionParser
  {
    #region public methods
    public static Expression ParseExpression(string expressionString)
    {
      return new Expression()
      {
        RootClause = Clause.End().Parse(expressionString)
      };
    }

    public static bool TryParseExpression(string expressionString, out Expression expression)
    {
      IResult<AbstractExpressionClause> result = Clause.End().TryParse(expressionString);

      if (result.WasSuccessful)
      {
        expression = new Expression() { RootClause = result.Value };
        return true;
      }
      else
      {
        expression = null;
        return false;
      }
    }
    #endregion

    #region private simple definitions
    private static readonly Parser<string> UnquotedString = Parse.Letter.AtLeastOnce().Text().Token();

    private static readonly Parser<string> Decimal = Parse.Decimal.Token();

    private static readonly Parser<string> SingleQuotedString =
      (from lquot in Parse.Char('\'')
       from content in Parse.CharExcept('\'').Many().Text()
       from rquot in Parse.Char('\'')
       select content).Token();

    private static readonly Parser<string> DoubleQuotedString =
      (from lquot in Parse.Char('"')
       from content in Parse.CharExcept('"').Many().Text()
       from rquot in Parse.Char('"')
       select content).Token();

    private static readonly Parser<string> QuotedString =
      SingleQuotedString
        .Or(DoubleQuotedString);

    private static readonly Parser<string> SingleQuotedDate =
      (from lquot in Parse.Char('\'')
       from content in Parse.CharExcept('\'').Many().Text() where IsDateTime(content)
       from rquot in Parse.Char('\'')
       select content).Token();

    private static readonly Parser<string> DoubleQuotedDate =
      (from lquot in Parse.Char('"')
       from content in Parse.CharExcept('"').Many().Text() where IsDateTime(content)
       from rquot in Parse.Char('"')
       select content).Token();

    private static readonly Parser<string> QuotedDate =
      SingleQuotedDate
        .Or(DoubleQuotedDate);


    private static readonly Parser<ComparisonClause.ComparisonTypeEnum> EqualTo = 
      (from chars in Parse.String("eq").Token() select ComparisonClause.ComparisonTypeEnum.EqualTo)
      .Or(from chars in Parse.String("=").Token() select ComparisonClause.ComparisonTypeEnum.EqualTo);

    private static readonly Parser<ComparisonClause.ComparisonTypeEnum> NotEqualTo =
      (from chars in Parse.String("neq").Token() select ComparisonClause.ComparisonTypeEnum.NotEqualTo)
      .Or(from chars in Parse.String("!=").Token() select ComparisonClause.ComparisonTypeEnum.NotEqualTo);

    private static readonly Parser<ComparisonClause.ComparisonTypeEnum> LessThan = 
      (from chars in Parse.String("lt").Token() select ComparisonClause.ComparisonTypeEnum.LessThan)
      .Or(from chars in Parse.String("<").Token() select ComparisonClause.ComparisonTypeEnum.LessThan);

    private static readonly Parser<ComparisonClause.ComparisonTypeEnum> LessThanOrEqualTo = 
      (from chars in Parse.String("lte").Token() select ComparisonClause.ComparisonTypeEnum.LessThanOrEqualTo)
      .Or(from chars in Parse.String("<=").Token() select ComparisonClause.ComparisonTypeEnum.LessThanOrEqualTo);

    private static readonly Parser<ComparisonClause.ComparisonTypeEnum> GreaterThan = 
      (from chars in Parse.String("gt").Token() select ComparisonClause.ComparisonTypeEnum.GreaterThan)
      .Or(from chars in Parse.String(">").Token() select ComparisonClause.ComparisonTypeEnum.GreaterThan);

    private static readonly Parser<ComparisonClause.ComparisonTypeEnum> GreaterThanOrEqualTo = 
      (from chars in Parse.String("gte").Token() select ComparisonClause.ComparisonTypeEnum.GreaterThanOrEqualTo)
      .Or(from chars in Parse.String(">=").Token() select ComparisonClause.ComparisonTypeEnum.GreaterThanOrEqualTo);

    private static readonly Parser<ComparisonClause.ComparisonTypeEnum> Like =
      from chars in Parse.String("like").Token() select ComparisonClause.ComparisonTypeEnum.Like;

    private static readonly Parser<ComparisonClause.ComparisonTypeEnum> ComparisonType =
      EqualTo
        .Or(NotEqualTo)
        .Or(LessThanOrEqualTo)
        .Or(LessThan)
        .Or(GreaterThanOrEqualTo)
        .Or(GreaterThan)
        .Or(Like);

    private static readonly Parser<BinaryLogicClause.LogicTypeEnum> And =
      from chars in Parse.String("and").Token() select BinaryLogicClause.LogicTypeEnum.And;

    private static readonly Parser<BinaryLogicClause.LogicTypeEnum> Or =
      from chars in Parse.String("or").Token() select BinaryLogicClause.LogicTypeEnum.Or;

    private static readonly Parser<UnaryLogicClause.LogicTypeEnum> Not =
      from chars in Parse.String("not").Token() select UnaryLogicClause.LogicTypeEnum.Not;

    private static readonly Parser<BinaryLogicClause.LogicTypeEnum> BinaryLogicOperator =
      And
      .Or(Or);

    private static readonly Parser<UnaryLogicClause.LogicTypeEnum> UnaryLogicOperator =
      Not;

    #endregion

    #region private combined definitions
    private static readonly Parser<AbstractOperand> FieldClause =
      (from content in UnquotedString select new Field() {FieldName = content}).Token();

    private static readonly Parser<AbstractConstant> ConstantClause =
      (from content in Decimal select MakeNumericConstant(content)).Token()
        .Or<AbstractConstant>((from content in QuotedDate select new DateConstant() {Value = GetAsDateTime(content).Value }).Token())
        .Or((from content in QuotedString select new StringConstant() {Value = content}).Token());


    private static readonly Parser<AbstractOperand> Operand =
      FieldClause
        .Or(ConstantClause);

    private static readonly Parser<AbstractExpressionClause> ComparisonLogicClause =
      (from lhs in Operand
       from logicOperator in ComparisonType
       from rhs in Operand
       select new ComparisonClause() { ComparisonType = logicOperator, LeftHandSide = lhs, RightHandSide = rhs }).Token();


    private static readonly Parser<AbstractExpressionClause> NestedClause =
      (from lparen in Parse.Char('(')
       from expr in Parse.Ref(() => Clause)
       from rparen in Parse.Char(')')
       select expr);

    private static readonly Parser<AbstractExpressionClause> InnerClause =
      NestedClause
      .XOr(ComparisonLogicClause);

    private static readonly Parser<AbstractExpressionClause> BinaryClause =
      Parse.ChainOperator(BinaryLogicOperator, InnerClause, MakeBinaryLogicClause);

    private static readonly Parser<AbstractExpressionClause> UnaryClause =
      (from logicOperator in UnaryLogicOperator
       from clause in InnerClause
       select new UnaryLogicClause() { LogicType = logicOperator, Clause = clause }).Token();

    private static readonly Parser<AbstractExpressionClause> Clause =
      BinaryClause
      .Or(UnaryClause)
      .Or(ComparisonLogicClause)
      .Or(NestedClause);

    #endregion

    #region private helper methods
    private static bool IsDateTime(string s)
    {
      return GetAsDateTime(s) != null;
    }

    private static DateTime? GetAsDateTime(string s)
    {
      string[] dateFormats = new string[]
      {
        "yyyyMMdd",
        "yyyyMMdd HHmmss",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-dd",
        "dd-MM-yyyy",
        "dd-MM-yyyy HH:mm:ss"
      };

      DateTime dateTime;
      bool success = DateTime.TryParseExact(s, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
      return success ? dateTime : (DateTime?) null;
    }

    private static AbstractConstant MakeNumericConstant(string s)
    {
      int intValue;
      double doubleValue;
      if (int.TryParse(s, out intValue))
        return new IntegerConstant() {Value = intValue};
      else if (double.TryParse(s, out doubleValue))
        return new RealConstant() {Value = doubleValue};
      else
        throw new Exception("Can't parse \""+s+"\" as either an integer or a double");
    }

    private static BinaryLogicClause MakeBinaryLogicClause(BinaryLogicClause.LogicTypeEnum logicOperator, AbstractExpressionClause lhs, AbstractExpressionClause rhs)
    {
      return new BinaryLogicClause() {LogicType = logicOperator, LeftHandSide = lhs, RightHandSide = rhs};
    }
    #endregion
  }
}
