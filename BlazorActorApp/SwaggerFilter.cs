using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace BlazorActorApp
{
    public class SwaggerFilter
    {
    }

    public class ApiExplorerIgnores : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            if (action.Controller.ControllerName.Equals("Pwa"))
                action.ApiExplorer.IsVisible = false;
        }
    }
}
