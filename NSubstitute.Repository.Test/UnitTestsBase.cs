using System;
using System.Collections.Generic;
using System.Text;

namespace NSubstitute.Repository.Test
{
    public abstract class UtitTestsBase : IDisposable
    {
        protected readonly SubstituteRepository SubstituteRepository;

        public UtitTestsBase()
        {
            SubstituteRepository = new SubstituteRepository();
        }

        public void Dispose()
        {
            SubstituteRepository.VerifyAll();
        }
    }
}
