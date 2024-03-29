﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace MockTracer.UI.Server;

public class ApiPrefixConvention : IApplicationModelConvention
{
  private readonly string prefix;
  private readonly Func<ControllerModel, bool> controllerSelector;
  private readonly AttributeRouteModel onlyPrefixRoute;
  private readonly AttributeRouteModel fullRoute;

  public ApiPrefixConvention(string prefix, Func<ControllerModel, bool> controllerSelector)
  {
    this.prefix = prefix;
    this.controllerSelector = controllerSelector;

    // Prepare AttributeRouteModel local instances, ready to be added to the controllers

    //  This one is meant to be combined with existing route attributes
    onlyPrefixRoute = new AttributeRouteModel(new RouteAttribute(prefix));

    //  This one is meant to be added as the route for api controllers that do not specify any route attribute
    fullRoute = new AttributeRouteModel(
        new RouteAttribute("api/[controller]"));
  }

  public void Apply(ApplicationModel application)
  {
    // Loop through any controller matching our selector
    foreach (var controller in application.Controllers.Where(controllerSelector))
    {
      // Either update existing route attributes or add a new one
      if (controller.Selectors.Any(x => x.AttributeRouteModel != null))
      {
        AddPrefixesToExistingRoutes(controller);
      }
      else
      {
        AddNewRoute(controller);
      }
    }
  }

  private void AddPrefixesToExistingRoutes(ControllerModel controller)
  {
    foreach (var selectorModel in controller.Selectors.Where(x => x.AttributeRouteModel != null).ToList())
    {
      // Merge existing route models with the api prefix
      var originalAttributeRoute = selectorModel.AttributeRouteModel;
      selectorModel.AttributeRouteModel =
          AttributeRouteModel.CombineAttributeRouteModel(onlyPrefixRoute, originalAttributeRoute);
    }
  }

  private void AddNewRoute(ControllerModel controller)
  {
    // The controller has no route attributes, lets add a default api convention 
    var defaultSelector = controller.Selectors.First(s => s.AttributeRouteModel == null);
    defaultSelector.AttributeRouteModel = fullRoute;
  }
}
