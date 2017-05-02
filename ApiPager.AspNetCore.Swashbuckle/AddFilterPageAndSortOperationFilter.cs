using System.Collections.Generic;
using System.Linq;
using ApiPager.Core;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiPager.AspNetCore.Swashbuckle
{
  public class AddFilterPageAndSortOperationFilter : IOperationFilter
  {
    #region private constants
    private const string IntegerParameterType = "integer";
    private const string StringParameterType = "string";
    private const string ArrayParameterType = "array";
    #endregion

    #region public methods
    public void Apply(Operation operation, OperationFilterContext context)
    {
      CanFilterPageAndSortAttribute filterPageAndSort = context.ApiDescription.ActionAttributes().OfType<CanFilterPageAndSortAttribute>().FirstOrDefault();

      if (filterPageAndSort != null)
      {
        if (operation.Parameters == null)
          operation.Parameters = new List<IParameter>();

        string listOfFieldsDescription = "";
        if (filterPageAndSort.AvailableFields.Length > 0)
          listOfFieldsDescription = "The available list of fields are "+string.Join(", ", filterPageAndSort.AvailableFields);

        operation.Parameters.Add(CreateQueryParameter("filter", StringParameterType, "Filter the list of items using a simple expression language.  " + listOfFieldsDescription));
        operation.Parameters.Add(CreateQueryParameter("orderBy", StringParameterType, "Order the items by a given field (in ascending order unless the field is preceeded by a \"-\" character).  " + listOfFieldsDescription));
        operation.Parameters.Add(CreateQueryParameter("offset", IntegerParameterType, "The number of items to skip in the (potentially filtered) result set before returning subsequent items."));
        operation.Parameters.Add(CreateQueryParameter("count", IntegerParameterType, "The maximum number of items to show from the (potentially filtered) result set."));
      }
    }
    #endregion

    #region private methods

    private IParameter CreateQueryParameter(string name, string type, string description)
    {
      NonBodyParameter parameter = new NonBodyParameter()
      {
        In = "query",
        Name = name,
        Description = description,
        Type = type,
        Required = false,
      };

      if (type == IntegerParameterType)
        parameter.Minimum = 0;
      else if (type == ArrayParameterType)
      {
        parameter.Items = new PartialSchema() {Type = "string"};
        parameter.CollectionFormat = "multi";
      }

      return parameter;
    }
    #endregion
  }
}
