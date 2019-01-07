namespace Starnet.Projections.Tests
{
    [SubscribesToStream("$ce-Match")]
    public class TestProjection : Projection, IHandledBy<TestHandler> { }

    [SubscribesToStream("$ce-Match")]
    [InactiveProjection]
    public class InactiveTestProjection : Projection, IHandledBy<TestHandler> { }
}