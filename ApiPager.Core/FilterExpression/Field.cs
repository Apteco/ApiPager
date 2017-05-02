using System;
using System.Linq;
using System.Text;

namespace ApiPager.Core.FilterExpression
{
  public class Field : AbstractOperand
  {
    #region public properties
    public string FieldName { get; set; }
    #endregion

    #region public methods
    public override void AddFieldValidationMessages(string[] filterColumns, StringBuilder errorMessagesBuilder)
    {
      if (!filterColumns.Any(c => string.Equals(c, FieldName, StringComparison.OrdinalIgnoreCase)))
        errorMessagesBuilder.Append("The field \"").Append(FieldName).Append("\" in this expression is not available to be filtered on.").Append(Environment.NewLine);
    }

    public override string ToString()
    {
      return FieldName;
    }
    #endregion
  }
}
