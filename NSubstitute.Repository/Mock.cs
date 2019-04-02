using NSubstitute.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NSubstitute.Repository
{
    public class Mock<TSubstitute> : Mock
    {
        public TSubstitute Object { get; }

        public Mock(TSubstitute substitute) : base(substitute)
        {
            Object = substitute;
        }

        public void Setup(Action<TSubstitute> substituteCall)
        {
            AddCall(() => substituteCall(Object));
        }

        public TResult Setup<TResult>(Func<TSubstitute, TResult> substituteCall)
        {
            AddCall(() => substituteCall(Object));

            return substituteCall(Object);
        }
    }

    public class Mock
    {
        private readonly HashSet<Action> calls;
        private readonly object substitute;

        public Mock(object substitute)
        {
            calls = new HashSet<Action>();
            this.substitute = substitute;
        }

        public IReadOnlyCollection<Action> RegisteredCalls
            => new ReadOnlyCollection<Action>(calls.ToList());

        public IReadOnlyCollection<ICall> ReceivedCalls
            => new ReadOnlyCollection<ICall>(substitute.ReceivedCalls().ToList());

        protected void AddCall(Action call)
        {
            calls.Add(call);
        }
    }
}
