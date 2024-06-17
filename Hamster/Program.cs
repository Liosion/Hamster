using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json; // Ensure you have Newtonsoft.Json NuGet package installed

class Program
{
    private static string bot_Token = "Bot_token"; // Add Bot token from @BotFather
    private static long chat_Id = -1001668422549; // Add Chat ID
    private static string hamsterAuthHash = ""; // Add your Hamster Combat Auth Hash
    static async Task Main(string[] args)
    {
        // Define the API endpoint
        string url = "https://api.hamsterkombat.io/clicker/config";

        // Create an instance of HttpClient
        using (HttpClient client = new HttpClient())
        {
            // Set up headers
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", hamsterAuthHash);
            client.DefaultRequestHeaders.Host = "api.hamsterkombat.io";

            try
            {
                // Send POST request with an empty content
                HttpResponseMessage response = await client.PostAsync(url, null);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // Read the response content as a string
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse JSON response to dynamic object
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);

                // wait for next cipher

                int reminSeconds = jsonResponse.dailyCipher.remainSeconds + 5;

                int milliseconds = 0 * 1000;

                // Wait for the remaining time

                await Task.Delay(milliseconds);

                string lastCypher = jsonResponse.dailyCipher.cipher;

                while (true)
                {
                    // Wait for the remaining time
                    await Task.Delay(1000);

                    // Send POST request with an empty content
                    response = await client.PostAsync(url, null);

                    // Ensure the request was successful
                    response.EnsureSuccessStatusCode();

                    // Read the response content as a string
                    responseBody = await response.Content.ReadAsStringAsync();

                    // Parse JSON response to dynamic object
                    jsonResponse = JsonConvert.DeserializeObject(responseBody);

                    // Check cipher
                    string newCypher = jsonResponse.dailyCipher.cipher;

                    if (newCypher != lastCypher)
                    {
                        break;
                    }
                }
                // Close wait for next cipher
                

                // Extract the cipher value from the response
                string cipherBase64 = jsonResponse.dailyCipher.cipher;

                // Decode the Base64 encoded cipher
                string decodedCipher = DecodeBase64(cipherBase64);

                // Convert the decoded cipher to Morse code
                string morseCode = ConvertToMorseCode(decodedCipher);

                // Format the message to send to Telegram
                string message = $@"
***
🚨 Hamster Combat Daily Cipher🚨

😳🔈 First Channel In The World 🔈😳
—————————
Cipher:
**
👉👉👉 {decodedCipher} 👈👈👈
**
—————————
Morse Code:

{morseCode}
—————————
Join Fastest Daily Channel: @EzAccess
Director: @LiosionQQ
***
";
                

                // Send message to Telegram
                await SendTelegramMessage(message);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    static string DecodeBase64(string base64EncodedData)
    {
        // Custom manipulation: remove the 4th character
        string manipulatedStr = $"{base64EncodedData.Substring(0, 3)}{base64EncodedData.Substring(4)}";

        // Decode the manipulated Base64 string
        byte[] decodedBytes = Convert.FromBase64String(manipulatedStr);
        string decodedStr = System.Text.Encoding.UTF8.GetString(decodedBytes);

        // Print the decoded string
        Console.WriteLine(decodedStr);
        return decodedStr;
    }

    static string ConvertToMorseCode(string input)
    {
        // Define Morse code mappings (you can expand this as needed)
        Dictionary<char, string> morseCodeMappings = new Dictionary<char, string>()
        {
            {'A', "o-"}, {'B', "-ooo"}, {'C', "-o-o"}, {'D', "-oo"},
            {'E', "o"}, {'F', "oo-o"}, {'G', "--o"}, {'H', "oooo"},
            {'I', "oo"}, {'J', "o---"}, {'K', "-o-"}, {'L', "o-oo"},
            {'M', "--"}, {'N', "-o"}, {'O', "---"}, {'P', "o--o"},
            {'Q', "--o-"}, {'R', "o-o"}, {'S', "ooo"}, {'T', "-"},
            {'U', "oo-"}, {'V', "ooo-"}, {'W', "o--"}, {'X', "-oo-"},
            {'Y', "-o--"}, {'Z', "--oo"},
            {'0', "-----"}, {'1', "o----"}, {'2', "oo---"}, {'3', "ooo--"},
            {'4', "oooo-"}, {'5', "ooooo"}, {'6', "-oooo"}, {'7', "--ooo"},
            {'8', "---oo"}, {'9', "----o"}
        };

        // Convert input to uppercase for simplicity
        input = input.ToUpper();

        StringBuilder morseCodeBuilder = new StringBuilder();

        foreach (char c in input)
        {
            if (morseCodeMappings.TryGetValue(c, out string morse))
            {
                morseCodeBuilder.Append(morse).Append(" \n");
            }
            else if (c == ' ')
            {
                morseCodeBuilder.Append("/ "); // Use / for space in Morse code representation
            }
            else
            {
                morseCodeBuilder.Append(c); // Keep unrecognized characters as they are
            }
        }

        return morseCodeBuilder.ToString().Trim();
    }

    static async Task SendTelegramMessage(string message)
    {
        // Replace with your Telegram bot token and chat ID
        string botToken = bot_Token;
        long chatId = chat_Id;

        string url = $"https://api.telegram.org/bot{botToken}/sendMessage?chat_id={chatId}&parse_mode=markdown&text={WebUtility.UrlEncode(message)}";

        using (var client = new HttpClient())
        {
            try
            {
                // Send POST request to Telegram API
                HttpResponseMessage response = await client.GetAsync(url);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // Read the response content (optional)
                string responseBody = await response.Content.ReadAsStringAsync();

                // Print response (optional)
                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Telegram request error: {e.Message}");
            }
        }
    }
}
