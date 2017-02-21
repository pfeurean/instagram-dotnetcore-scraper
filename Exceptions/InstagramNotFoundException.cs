namespace Instagram.Scraper
{
    public class InstagramNotFoundException : System.Exception
    {
		public InstagramNotFoundException(string message) : base(message)
		{ }
	}
}