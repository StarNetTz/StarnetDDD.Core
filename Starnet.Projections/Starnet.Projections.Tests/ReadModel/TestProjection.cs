﻿namespace Starnet.Projections.Tests
{
    [SubscribesToStream("$ce-Match")]
    public class TestProjection : Projection, IHandledBy<TestProjectionHandler> { }

    [SubscribesToStream("$ce-Match")]
    [InactiveProjection]
    public class InactiveTestProjection : Projection, IHandledBy<TestProjectionHandler> { }
}