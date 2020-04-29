namespace Wpf.Autocomplete
{
    public class SearchResult
    {
        public SearchResult(string[] results)
        {
            Results = results;
        }

        public string[] Results { get; }
    }
}