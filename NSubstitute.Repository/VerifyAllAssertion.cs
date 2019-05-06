using System.Linq;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Core.SequenceChecking;
using NSubstitute.Exceptions;

namespace NSubstitute.Repository
{
    public class VerifyAllAssertion
    {
        private const string CallDelimiter = "\n    ";

        public void Assert(IQueryResults queryResult, ICall[] receivedCalls)
        {
            var querySpec = queryResult
                .QuerySpecification()
                .Where(x => IsNotPropertyGetterCall(x.CallSpecification.GetMethodInfo()))
                .ToArray();

            if (querySpec.Length != receivedCalls.Length)
            {
                throw new CallSequenceNotFoundException(GetExceptionMessage(querySpec, receivedCalls));
            }

            var notMatchedCalls = querySpec.Where(spec => receivedCalls.FirstOrDefault(call => Matches(call, spec)) == null).ToArray();

            if (notMatchedCalls.Any())
            {
                throw new CallSequenceNotFoundException(GetExceptionMessage(notMatchedCalls, receivedCalls));
            }
        }

        private bool Matches(ICall call, CallSpecAndTarget specAndTarget)
        {
            var callTarget = call.Target();
            return ReferenceEquals(callTarget, specAndTarget.Target)
                   && specAndTarget.CallSpecification.IsSatisfiedBy(call);
        }

        private bool IsNotPropertyGetterCall(MethodInfo methodInfo)
        {
            return methodInfo.GetPropertyFromGetterCallOrNull() == null;
        }

        private string GetExceptionMessage(CallSpecAndTarget[] querySpec, ICall[] matchingCallsInOrder)
        {
            var formatter = new SequenceFormatter(CallDelimiter, querySpec, matchingCallsInOrder);
            return string.Format(
                "\nActually not matched registered calls:\n{0}{1}\n\nReceived calls:\n{0}{2}\n\n{3}",
                CallDelimiter,
                formatter.FormatQuery(),
                formatter.FormatActualCalls(),
                "*** Note: calls to property getters are not considered part of the query. ***");
        }
    }
}
