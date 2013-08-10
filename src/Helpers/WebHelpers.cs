using System.Net;
using ServiceStack.Common.Web;

namespace BitHome.Helpers
{
    public class WebHelpers
    {
        public static object OkResponse
        {
            get
            {
                return new HttpResult() { StatusCode = HttpStatusCode.OK };
            }
        }

        public static object NotFoundResponse
        {
            get
            {
                return new HttpResult() { StatusCode = HttpStatusCode.NotFound };
            }
        }

        public static object BadRequestResponse
        {
            get
            {
                return new HttpResult() { StatusCode = HttpStatusCode.BadRequest };
            }
        }
    }
}
