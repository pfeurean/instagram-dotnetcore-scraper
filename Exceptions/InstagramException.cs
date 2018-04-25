using System.Net;

namespace InstagramScraper
{
    public class InstagramException : System.Exception
    {
        public HttpStatusCode StatusCode { get; set; }

		public InstagramException(string message) : this(0, message)
		{ }

        public InstagramException(HttpStatusCode statusCode, string message) : base(message)
        {
            this.StatusCode = statusCode;
        }
	}
}