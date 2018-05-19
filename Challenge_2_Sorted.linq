<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <DisableMyExtensions>true</DisableMyExtensions>
</Query>

void Main()
{
	const string GetUrl = "http://84.200.109.239:5000/challenges/2/sorted/";
	const string PostUrl = "http://84.200.109.239:5000/solutions/2/";

	var counter = 100;
	double gesamt = 0.0;
	var httpClient = new HttpClient();
	var handler = new HttpClientHandler(httpClient);
	var stopwatch = new Stopwatch();
	for (int i = 0; i < counter; i++)
	{
		var responseGet = handler.Get(GetUrl);
		var resultGet = responseGet.Content.ReadAsStringAsync().Result;
		
		// Benchmark
		stopwatch.Restart();
		var resDataGet = JsonConvert.DeserializeObject<Data>(resultGet);
		var indexToPost = Array.BinarySearch(resDataGet.Werte, resDataGet.Key);
		stopwatch.Stop();
		gesamt += stopwatch.Elapsed.TotalMilliseconds;
		
		if ((indexToPost < 0) == false)
		{
			var solResult = new SolutionResult() { Token = indexToPost };
			var responsePost = handler.Post(PostUrl, solResult.AsJson());
			var resultPost = responsePost.Content.ReadAsStringAsync().Result;
//			resultPost.Dump("Post Response");			
		}
		else { "Fehler!!".Dump("Post Response"); }		
	}

	$"{gesamt} ms".Dump();
	$"Durchschnitt bei {counter} Durchläufe: { gesamt / counter} ms".Dump();
	/* 24,3325 ms, Durchschnitt bei 100 Durchläufe: 0,243325 ms  */

}

public class Data
{
	[JsonProperty("k")]
	public long Key { get; set; }
	[JsonProperty("list")]
	public  long[] Werte { get; set;}
}

public class SolutionResult
{
	[JsonProperty("token")]
	public int Token { get; set; }
}

public class HttpClientHandler : IHttpHandler
{
	private readonly HttpClient client;

	public HttpClientHandler(HttpClient client)
	{
		this.client = client;
	}

	public HttpResponseMessage Get(string url)
	{
		return GetAsync(url).Result;
	}

	public HttpResponseMessage Post(string url, HttpContent content)
	{
		return PostAsync(url, content).Result;
	}

	public async Task<HttpResponseMessage> GetAsync(string url)
	{
		return await client.GetAsync(url);
	}

	public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
	{
		return await client.PostAsync(url, content);
	}
}

public interface IHttpHandler
{
	HttpResponseMessage Get(string url);
	HttpResponseMessage Post(string url, HttpContent content);
	Task<HttpResponseMessage> GetAsync(string url);
	Task<HttpResponseMessage> PostAsync(string url, HttpContent content);
}

public static class Extensions
{
	public static StringContent AsJson(this object o)
	 => new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");

	public static int IndexOf<T>(this IEnumerable<T> obj, T value, IEqualityComparer<T> comparer)
	{
		comparer = comparer ?? EqualityComparer<T>.Default;
		var found = obj
			.Select((a, i) => new { a, i })
			.FirstOrDefault(x => comparer.Equals(x.a, value));
		return found == null ? -1 : found.i;
	}
}