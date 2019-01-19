using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Text;

static HttpClient httpClient = new HttpClient();

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    try
    {
        FormModel data = await req.Content.ReadAsAsync<FormModel>();
        var FormContent = new FormUrlEncodedContent(new[]{
            new KeyValuePair<string, string>("code", data.Code),
            new KeyValuePair<string,string>("redirect_uri", data.Redirect_uri),
            new KeyValuePair<string,string>("client_id", data.Client_id),
            new KeyValuePair<string,string>("client_secret", System.Environment.GetEnvironmentVariable("CLIENT_SECRET")),
            new KeyValuePair<string,string>("scope", ""),
            new KeyValuePair<string,string>("grant_type", "authorization_code")
        });
        var request = new HttpRequestMessage(){
            RequestUri = new Uri("https://accounts.google.com/o/oauth2/token"),
            Method = HttpMethod.Post,
            Content = FormContent
        };
        request.Content.Headers.ContentType= new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        request.Headers.Add("X-Requested-With", "XMLHttpRequest");

        var response = await httpClient.SendAsync(request);
        var result = await response.Content.ReadAsAsync<object>();

        log.Info(result.ToString());

        return result == null
            ? req.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong.")
            : req.CreateResponse(HttpStatusCode.OK, result);
    }
    catch(Exception ex)
    {
        log.Info(ex.Message);
        return req.CreateResponse(HttpStatusCode.InternalServerError, "There was an issue with the server.");
    }
    
}

public class FormModel{
    public string Code{get;set;}
    public string Redirect_uri{get;set;}
    public string Client_id {get;set;}
}
