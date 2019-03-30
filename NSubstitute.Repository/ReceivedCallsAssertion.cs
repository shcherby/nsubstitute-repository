using NSubstitute.Core;
using NSubstitute.Core.SequenceChecking;
using NSubstitute.Exceptions;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Repository
{
    public class ReceivedCallsAssertion
    {
        public void Assert(IQueryResults queryResult, ICall[] receivedCalls)
        {
            CallSpecAndTarget[] querySpec = queryResult
                .QuerySpecification()
                .Where(x => IsNotPropertyGetterCall(x.CallSpecification.GetMethodInfo()))
                .ToArray();

            if (querySpec.Length != receivedCalls.Length)
            {
                throw new CallSequenceNotFoundException(GetExceptionMessage(querySpec, receivedCalls));
            }

            var callsAndSpecs = receivedCalls
               .Zip(querySpec, (call, specAndTarget) =>
                               new
                               {
                                   Call = call,
                                   Spec = specAndTarget.CallSpecification,
                                   IsMatch = Matches(call, specAndTarget)
                               }
               );

            if (callsAndSpecs.Any(x => !x.IsMatch))
            {
                throw new CallSequenceNotFoundException(GetExceptionMessage(querySpec, receivedCalls));
            }
        }

        private bool Matches(ICall call, CallSpecAndTarget specAndTarget)
        {
            return ReferenceEquals(call.Target(), specAndTarget.Target)
                   && specAndTarget.CallSpecification.IsSatisfiedBy(call);
        }

        private bool IsNotPropertyGetterCall(MethodInfo methodInfo)
        {
            return methodInfo.GetPropertyFromGetterCallOrNull() == null;
        }

        private string GetExceptionMessage(CallSpecAndTarget[] querySpec, ICall[] matchingCallsInOrder)
        {
            const string callDelimiter = "\n    ";
            var formatter = new SequenceFormatter(callDelimiter, querySpec, matchingCallsInOrder);
            return string.Format("\nActually registered calls:\n{0}{1}\n" +
                                 "\nReceived calls:\n{0}{2}\n\n{3}",
                                 callDelimiter,
                                 formatter.FormatQuery(),
                                 formatter.FormatActualCalls(),
                                 "*** Note: calls to property getters are not considered part of the query. ***");
        }
    }
}
