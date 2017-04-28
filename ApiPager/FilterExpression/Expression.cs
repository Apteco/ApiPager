using System;
using System.Text;

namespace ApiPager.Core.FilterExpression
{
  public class Expression
  {
    #region public properties
    public AbstractExpressionClause RootClause { get; set; }
    #endregion

    #region public methods
    public override string ToString()
    {
      return RootClause == null? "" : RootClause.ToString();
    }

    public bool AreFieldsValid(string[] filterColumns, out string errorMessages)
    {
      if (RootClause == null)
      {
        errorMessages = null;
        return true;
      }

      StringBuilder errorMessagesBuilder = new StringBuilder();
      RootClause.AddFieldValidationMessages(filterColumns, errorMessagesBuilder);

      errorMessages = errorMessagesBuilder.Length == 0? null : errorMessagesBuilder.ToString();
      return string.IsNullOrEmpty(errorMessages);
    }
    #endregion
  }
}
