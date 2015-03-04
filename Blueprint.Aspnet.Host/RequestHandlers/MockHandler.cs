using System.Collections.Specialized;
using DeepEqual.Syntax;
using Newtonsoft.Json;
using snowcrashCLR;
using System;
using System.Linq;
using Blueprint.Aspnet.Host.Extensions;

namespace Blueprint.Aspnet.Host.RequestHandlers
{
    public class MockHandler : IRequestHandler
    {
        private readonly Resource _resource;
        public MockHandler(Resource resource, IRoute route)
        {
            _resource = resource;
            Route = route;
        }

        public IRoute Route { get; private set; }

        public void Handle(IRequestWrapper request, IResponseWrapper response)
        {
            var action = _resource.GetActionsCs().FirstOrDefault(a => a.method.EqualsIgnoreCase(request.HttpMethod));
            if (action != null)
            {
                var examples = action.GetTransactionExamplesCs();
                foreach (var example in examples)
                {
                    var match = MatchExample(request, example);
                    if (match)
                    {
                        WriteResponse(response, example);
                        response.End();
                        break;
                    }
                }
            }
        }


        private void WriteResponse(IResponseWrapper actualResponse, TransactionExample example)
        {
            actualResponse.Clear();

            var exampleResponse = example.GetResponsesCs().First();

            // write status code
            if (!string.IsNullOrWhiteSpace(exampleResponse.name))
            {
                int statusCode;
                if (int.TryParse(exampleResponse.name, out statusCode))
                {
                    actualResponse.StatusCode = statusCode;
                }
            }

            // write headers
            exampleResponse
                .GetHeadersCs()
                .ForEach(h => actualResponse.AppendHeader(h.Item1, h.Item2));

            //write body
            actualResponse.Write(exampleResponse.body);
        }


        private bool MatchExample(IRequestWrapper request, TransactionExample example)
        {
            var payloads = example.GetRequestsCs();
            if (!payloads.Any() || request.HttpMethod == "GET")
                return true;

            var matchingPayloads =
                from payload in payloads
                let isMatch = MatchPayload(request, payload)
                where isMatch
                select payload;

            return matchingPayloads.Any();
        }

        private bool MatchPayload(IRequestWrapper actualRequest, Payload payload)
        {
            if (!MatchHeaders(actualRequest.Headers, payload.Headers()))
                return false;

            if (!MatchBody(actualRequest, payload))
                return false;

            return true;
        }


        private bool MatchHeaders(NameValueCollection actualRequestHeaders, NameValueCollection payloadHeaders)
        {
            return actualRequestHeaders.Contains(payloadHeaders);
        }

        private bool MatchBody(IRequestWrapper request, Payload payload)
        {
            // check if content type matches, only when blueprint has a content type header.
            var payloadContentTypeHeader = payload
                .Headers()
                .Get("content-type");

            if (!string.IsNullOrWhiteSpace(payloadContentTypeHeader))
                if (!request.ContentType.EqualsIgnoreCase(payloadContentTypeHeader))
                    return false;

            if (request.ContentType.EqualsIgnoreCase("application/json"))
                return MatchJson(request.Body, payload.body);

            // compare body as a string ignoring whitespace (space, tab, line ending, carriage return)
            return request
                .Body
                .EqualsIgnoreWhitespace(payload.body, StringComparison.OrdinalIgnoreCase);
        }

        private bool MatchJson(string requestBody, string payloadBody)
        {
            var requestObject = JsonConvert.DeserializeObject(requestBody);
            var payloadObject = JsonConvert.DeserializeObject(payloadBody);

            return requestObject.IsDeepEqual(payloadObject);
        }



    }
}