void Main()
{
	// Nuget JSON.Net
	// using System.Net.Http
	
	const string GetUrl = "http://84.200.109.239:5000/challenges/1/";
	const string PostUrl = "http://84.200.109.239:5000/solutions/1/";
	
	using (var clientGet = CreateClient())
	{
		var responseGet = clientGet.GetAsync(GetUrl).Result;
		var resultGet = responseGet.Content.ReadAsStringAsync().Result;
		resultGet.Dump("Get Response"); 	// Linqpad

		if (string.IsNullOrEmpty(resultGet) == false)
		{
			using (var client = CreateClient())
			{
				var solResult = new SolutionResult() { Token = resultGet };
				var responsePost = client.PostAsync(PostUrl, solResult.AsJson()).Result;
				var resultPost = responsePost.Content.ReadAsStringAsync().Result;
				resultPost.Dump("Post Response"); 	// Linqpad
			}
		}
	}
}

private class SolutionResult
{
	[JsonProperty("token")]
	public string Token { get; set; }
}

private static HttpClient CreateClient(string accessToken = "")
{
	var client = new HttpClient();
	if (!string.IsNullOrWhiteSpace(accessToken))
	{
		client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
	}
	
	return client;
}

public static class Extensions
{
	public static StringContent AsJson(this object o)
	 => new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");
}
