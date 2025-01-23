using System.Net;

namespace Domain.Commons
{
    public class Response
    {
        public Response(HttpStatusCode statusCode, IEnumerable<string> errors = null, object data = null)
        {
            StatusCode = statusCode;
            Errors = errors?.ToList()?.AsReadOnly();
            Data = data;
        }

        public HttpStatusCode StatusCode { get; }
        public IReadOnlyCollection<string> Errors { get; }
        public object Data { get; }
    }
}
