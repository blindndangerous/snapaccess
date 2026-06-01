using FluentAssertions;
using SnapAccess;
using Xunit;

namespace SnapAccess.Core.Tests;

public class TurnTimerWarningTests
{
    [Fact]
    public void AboveFifteenSeconds_NoWarning()
    {
        new TurnTimerWarning().Evaluate(30f).Should().Be(TurnTimerWarningLevel.None);
    }

    [Fact]
    public void NormalCountdown_WarnsAtFifteenThenUrgentAtFive_EachOnce()
    {
        var w = new TurnTimerWarning();
        w.Evaluate(16f).Should().Be(TurnTimerWarningLevel.None);
        w.Evaluate(15f).Should().Be(TurnTimerWarningLevel.Warning);
        w.Evaluate(12f).Should().Be(TurnTimerWarningLevel.None); // already warned
        w.Evaluate(5f).Should().Be(TurnTimerWarningLevel.Urgent);
        w.Evaluate(3f).Should().Be(TurnTimerWarningLevel.None);  // already urgent
        w.Evaluate(1f).Should().Be(TurnTimerWarningLevel.None);
    }

    [Fact]
    public void Zero_OrNegative_NeverWarns()
    {
        var w = new TurnTimerWarning();
        w.Evaluate(0f).Should().Be(TurnTimerWarningLevel.None);
        w.Evaluate(-3f).Should().Be(TurnTimerWarningLevel.None);
    }

    [Fact]
    public void TimerGoingBackUpAboveTwenty_ResetsForNextTurn()
    {
        var w = new TurnTimerWarning();
        w.Evaluate(15f).Should().Be(TurnTimerWarningLevel.Warning);
        w.Evaluate(25f).Should().Be(TurnTimerWarningLevel.None); // new turn, resets
        w.Evaluate(15f).Should().Be(TurnTimerWarningLevel.Warning); // warns again
    }

    [Fact]
    public void Reset_ClearsFiredState()
    {
        var w = new TurnTimerWarning();
        w.Evaluate(15f);
        w.Reset();
        w.Evaluate(15f).Should().Be(TurnTimerWarningLevel.Warning);
    }

    // Characterization of existing behavior: if the timer is first seen already
    // at/under 5s (a jump past the 15s window), the warning branch fires first
    // because it only checks <= 15. Preserved as-is by this refactor.
    [Fact]
    public void FirstSeenUnderFive_FiresWarningFirst_ThenUrgent()
    {
        var w = new TurnTimerWarning();
        w.Evaluate(4f).Should().Be(TurnTimerWarningLevel.Warning);
        w.Evaluate(4f).Should().Be(TurnTimerWarningLevel.Urgent);
    }
}
