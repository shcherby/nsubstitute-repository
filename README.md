# NSubstitute SubstituteRepository

You can create and verify all mocks in a single place by using a SubstituteRepository, it ensure that EVERY single call to the mocked object has to have expectations set on it. If we make an additional call and we don't set the expectation, our test will fail.

```csharp
[Fact]
public void Check_call_for_substitute_was_received_and_registered()
{
    var substituteRepository = new SubstituteRepository();

    var car = substituteRepository.Create<ICar>();
    car.Setup(m => m.Rev());

    car.Object.Rev();

    substituteRepository.VerifyAll();
}
```
