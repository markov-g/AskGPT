using System.Net.Mime;
using System.Text;
using Newtonsoft.Json;

if (args.Length > 0)
{
    HttpClient instanceClient = new HttpClient();
    instanceClient.DefaultRequestHeaders.Add(
        "authorization", "Bearer <<sk-XXXX>>" //TODO Add user secret
        );
    var content = new StringContent("{\"model\": \"text-davinci-001\", \"prompt\": \""+ args[0] +"\",\"temperature\": 1,\"max_tokens\": 100}",
        Encoding.UTF8, "application/json");
    HttpResponseMessage response = await instanceClient.PostAsync("https://api.openai.com/v1/completions", content);
    string responseString = await response.Content.ReadAsStringAsync();

    try
    {
        // this is horrible stuff --> use Microsoft's Library instead
        var dyData = JsonConvert.DeserializeObject<dynamic>(responseString);
        string guess = GuessCommand(dyData.choices[0].text);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"---> My guess at the command prompt is: {guess}");
        Console.ResetColor();
    }
    catch (Exception e)
    {
        Console.WriteLine($"---> Could not deserialize the JSON: {e.Message}");
        throw;
    }
    
    // Console.WriteLine(responseString);
}
else
{
    Console.WriteLine("---> You need to provide some search input");
}

static string GuessCommand(string raw)
{
    Console.WriteLine("---> GPT3 API returned text: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine(raw);
    Console.ResetColor();

    // return only the last line --> this is very naive, but seems to work ~70% of the time
    var lastIndex = raw.LastIndexOf("\n");
    string guess = raw.Substring(lastIndex + 1);
    
    TextCopy.ClipboardService.SetText(guess);

    return guess;
}