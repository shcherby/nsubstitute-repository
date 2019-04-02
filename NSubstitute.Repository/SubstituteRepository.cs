using NSubstitute.Core;
using NSubstitute.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute
{
    public class SubstituteRepository : IDisposable
    {
        private HashSet<Mock> mocks;

        public SubstituteRepository()
        {
            mocks = new HashSet<Mock>();
        }

        public Mock<TSubstitute> Create<TSubstitute>()
            where TSubstitute : class
        {
            var mock = new Mock<TSubstitute>(Substitute.For<TSubstitute>());
            mocks.Add(mock);
            return mock;
        }

        public void VerifyAll()
        {
            if (!mocks.Any())
            {
                return;
            }

            var query = new Query(SubstitutionContext.Current.CallSpecificationFactory);

            Action calls = () =>
            {
                mocks
                .SelectMany(callWrapper => callWrapper.RegisteredCalls)
                .ToList()
                .ForEach(callWrapper => callWrapper());
            };

            SubstitutionContext.Current.ThreadContext.RunInQueryContext(calls, query);

            ICall[] receivedCalls = mocks.SelectMany(mock => mock.ReceivedCalls).ToArray();
            mocks.Clear();

            new VerifyAllAssertion().Assert(query.Result(), receivedCalls);
        }

        public void Dispose()
        {
            mocks.Clear();
        }
    }
}
