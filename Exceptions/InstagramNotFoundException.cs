namespace InstagramScraper
{
    public class InstagramNotFoundException : System.Exception
    {
		public InstagramNotFoundException(string message) : base(message)
		{ }
	}
}