namespace Starnet.Projections.Tests
{
    [SubscribesToStream("$ce-Match")]
    public class TestProjection : Projection, IHandledBy<TestHandler> { }
}