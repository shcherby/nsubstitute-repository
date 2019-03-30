using NSubstitute.Core;
using NSubstitute.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute
{
    public class SubstituteRepository : IDisposable
    {
        private static readonly Dictionary<object, List<Action>> SubstituteCalls = new Dictionary<object, List<Action>>();

        public TSubstitute Create<TSubstitute>()
            where TSubstitute : class
        {
            var substitute = Substitute.For<TSubstitute>();
            SubstituteCalls.Add(substitute, new List<Action>());
            return substitute;
        }

        public static void AddCall(object substitute, Action call)
        {
            if (SubstituteCalls.TryGetValue(substitute, out var calls))
            {
                calls.Add(call);
            }
            else
            {
                throw new Exception($"{substitute.GetType().Name} wasn't create in {nameof(SubstituteRepository)}.");
            }
        }

        public void VerifyAll()
        {
            if (!SubstituteCalls.Keys.Any())
            {
                return;
            }

            var query = new Query(SubstitutionContext.Current.CallSpecificationFactory);

            Action calls = () =>
            {
                SubstituteCalls.Values.SelectMany(x => x).ToList().ForEach(x => x());
            };

            SubstitutionContext.Current.ThreadContext.RunInQueryContext(calls, query);

            ICall[] receivedCalls = SubstituteCalls.Keys.SelectMany(substitute => substitute.ReceivedCalls()).ToArray();
            SubstituteCalls.Clear();

            new ReceivedCallsAssertion().Assert(query.Result(), receivedCalls);
        }

        public void Dispose()
        {
            SubstituteCalls.Clear();
        }
    }
}
