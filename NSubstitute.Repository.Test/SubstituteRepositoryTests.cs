using NSubstitute.Exceptions;
using System;
using Xunit;

namespace NSubstitute.Repository.Test
{
    public class SubstituteRepositoryTests : UtitTestsBase
    {
        private ICar _car;
        private IBus _bus;
        const int Rpm = 7000;
        private static readonly object[] Luggage = new[] { new object(), new object() };
        private static readonly DateTime[] ServiceDates = new[] { new DateTime(2001, 01, 01), new DateTime(2002, 02, 02) };

        public SubstituteRepositoryTests()
        {
            _car = SubstituteRepository.Create<ICar>();
            _bus = SubstituteRepository.Create<IBus>();
        }

        [Fact]
        public void Check_when_calls_from_several_substitutes_was_received()
        {
            _car.Setup(m => m.Rev());
            _bus.Setup(m => m.RevBus());

            _bus.RevBus();
            _car.Rev();
        }

        [Fact]
        public void Check_when_call_was_received()
        {
            _car.Setup(m => m.Rev());

            _car.Rev();
        }

        [Fact]
        public void Throw_when_expected_call_was_not_received()
        {
            Assert.Throws<CallSequenceNotFoundException>(() =>
            {
                _car.Idle();
                SubstituteRepository.VerifyAll();
            });
        }

        [Fact]
        public void Check_call_was_received_with_expected_argument()
        {
            _car.Setup(m => m.RevAt(Rpm));

            _car.RevAt(Rpm);
        }

        [Fact]
        public void Check_call_was_received_with_return_value()
        {
            float expected = 10;
            _car.Setup(m => m.GetCapacityInLitres()).Returns(expected);

            var actual = _car.GetCapacityInLitres();

            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Throw_when_expected_call_was_received_with_different_argument()
        {
            _car.Setup(m => m.RevAt(Rpm));

            Assert.Throws<CallSequenceNotFoundException>(() =>
            {
                _car.RevAt(Rpm + 2);
                SubstituteRepository.VerifyAll();
            });
        }

        [Fact]
        public void Check_that_a_call_was_not_received()
        {
            _car.Setup(m => m.RevAt(Rpm));

            Assert.Throws<CallSequenceNotFoundException>(() =>
            {
                _car.Rev();
                SubstituteRepository.VerifyAll();
            });
        }

        [Fact]
        public void Check_that_a_call_was_run_not_from_substitute_repository()
        {
            var car = Substitute.For<ICar>();

            var exception = Assert.Throws<Exception>(() =>
             {
                 car.Setup(m => m.Rev());
             });

            Assert.Equal("ObjectProxy wasn't create in SubstituteRepository.", exception.Message);
        }
    }

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

    public interface IBus
    {
        void StartBus();
        void RevBus();
        void StopBus();
        void IdleBus();
        void RevAtBus(int rpm);
        void FillPetrolTankToBus(int percent);
        void StoreLuggageBus(params object[] luggage);
        void RecordServiceDatesBus(params DateTime[] serviceDates);
        float GetCapacityInLitresBus();
        event Action StartedBus;
    }

    public interface ICar
    {
        void Start();
        void Rev();
        void Stop();
        void Idle();
        void RevAt(int rpm);
        void FillPetrolTankTo(int percent);
        void StoreLuggage(params object[] luggage);
        void RecordServiceDates(params DateTime[] serviceDates);
        float GetCapacityInLitres();
        event Action Started;
    }
}
