﻿using System;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api.V2;
using Microsoft.Rest;

class Program {

  static string aadAuthorizationEndpoint = "https://login.windows.net/common/oauth2/authorize";
  static string resourceUriPowerBi = "https://analysis.windows.net/powerbi/api";
  static string urlPowerBiRestApiRoot = "https://api.powerbi.com/";

  static string clientId = "79104967-3752-4645-98c4-b0360b7bb1dc";
  static string redirectUrl = "http://localhost/app1234";

  static string GetAccessToken() {

    // create new authentication context 
    var authenticationContext =
      new AuthenticationContext(aadAuthorizationEndpoint);

    // use authentication context to trigger user sign-in and return access token 
    var userAuthnResult =
      authenticationContext.AcquireTokenAsync(resourceUriPowerBi,
                                              clientId,
                                              new Uri(redirectUrl),
                                              new PlatformParameters(PromptBehavior.Auto)).Result;

    //var userAuthnResult =
    // authenticationContext.AcquireTokenAsync(resourceUriPowerBi,
    //                                         clientId,
    //                                         new UserPasswordCredential("ACCOUNT_NAME_OF_MASTER_USER", 
    //                                                                    "PASSWORD_OF_MASTER_USER")).Result;



    // return access token to caller
    return userAuthnResult.AccessToken;

  }

  static PowerBIClient GetPowerBiClient() {
    var tokenCredentials = new TokenCredentials(GetAccessToken(), "Bearer");
    return new PowerBIClient(new Uri(urlPowerBiRestApiRoot), tokenCredentials);
  }

  static void Main() {

    DisplayPersonalWorkspaceAssets();
    
  }

  static void DisplayPersonalWorkspaceAssets() {

    PowerBIClient pbiClient = GetPowerBiClient();

    Console.WriteLine("Datasets:");
    var datasets = pbiClient.Datasets.GetDatasets().Value;
    foreach (var dataset in datasets) {
      Console.WriteLine(" - " + dataset.Name + " [" + dataset.Id + "]");
    }

    Console.WriteLine();
    Console.WriteLine("Reports:");
    var reports = pbiClient.Reports.GetReports().Value;
    foreach (var report in reports) {
      Console.WriteLine(" - " + report.Name + " [" + report.Id + "]");
    }

    Console.WriteLine();
    Console.WriteLine("Dashboards:");
    var dashboards = pbiClient.Dashboards.GetDashboards().Value;
    foreach (var dashboard in dashboards) {
      Console.WriteLine(" - " + dashboard.DisplayName + " [" + dashboard.Id + "]");
    }

    Console.WriteLine();
  }

}

