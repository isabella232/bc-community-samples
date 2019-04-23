#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Text;

public static async Task<HttpResponseMessage> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("C# HTTP trigger function processed a request.");
    string name = req.Query["content"];

    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    log.LogInformation($"Message received {requestBody}");
    
    string base64Decoded = ASCIIEncoding.ASCII.GetString(System.Convert.FromBase64String(requestBody));
    log.LogInformation($"Message decoded {base64Decoded}");

    int start = base64Decoded.IndexOf("{");
    int end = base64Decoded.LastIndexOf("}");
    string result = base64Decoded.Substring(start, end - start + 1);
    log.LogInformation($"Message decoded as JSON {result}");

    dynamic data = JsonConvert.DeserializeObject(result);
    log.LogInformation($"DATA {data}");
    Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    log.LogInformation($"unixTimestamp {unixTimestamp}");
    data.time = unixTimestamp;
    data.temperature1 = (Int32)data.temperature1;
    data.temperature2 = (Int32)data.temperature2;
    data.humiditysample1 = (Int32)data.humiditysample1;
    data.humiditysample2 = (Int32)data.humiditysample2;
    
    log.LogInformation($"DATA {data}");
    string jsonToReturn = JsonConvert.SerializeObject(data);

    return new HttpResponseMessage(HttpStatusCode.OK) {
        Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
    };
}
