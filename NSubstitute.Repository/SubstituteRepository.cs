using NSubstitute.Core;
using NSubstitute.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute
{
    public class SubstituteRepository : IDisposable
    {
        private static Dictionary<object, HashSet<Action>> substituteCalls;

        public SubstituteRepository()
        {
            substituteCalls = new Dictionary<object, HashSet<Action>>();
        }

        public TSubstitute Create<TSubstitute>()
            where TSubstitute : class
        {
            var substitute = Substitute.For<TSubstitute>();
            substituteCalls.Add(substitute, new HashSet<Action>());
            return substitute;
        }

        public static void AddCall(object substitute, Action call)
        {
            if (substituteCalls.TryGetValue(substitute, out var calls))
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
            if (!substituteCalls.Keys.Any())
            {
                return;
            }

            var query = new Query(SubstitutionContext.Current.CallSpecificationFactory);

            Action calls = () =>
            {
                substituteCalls.Values
                .SelectMany(callWrapper => callWrapper)
                .ToList()
                .ForEach(callWrapper => callWrapper());
            };

            SubstitutionContext.Current.ThreadContext.RunInQueryContext(calls, query);

            ICall[] receivedCalls = substituteCalls.Keys.SelectMany(substitute => substitute.ReceivedCalls()).ToArray();
            substituteCalls.Clear();

            new VerifyAllAssertion().Assert(query.Result(), receivedCalls);
        }

        public void Dispose()
        {
            substituteCalls.Clear();
        }
    }
}
