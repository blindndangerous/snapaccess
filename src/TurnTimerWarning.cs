namespace SnapAccess;

/// <summary>The warning to announce for the current turn-timer reading.</summary>
public enum TurnTimerWarningLevel
{
    None,
    Warning,
    Urgent,
}

/// <summary>
/// Decides when to announce that the turn timer is running low: once at 15
/// seconds (warning) and once at 5 seconds (urgent), resetting when the timer
/// climbs back above 20 seconds (a new turn). Pure logic extracted from
/// BattlefieldHandler.CheckTurnTimer so it can be tested without the game.
/// </summary>
public sealed class TurnTimerWarning
{
    private bool _warningFired;
    private bool _urgentFired;

    /// <summary>
    /// Evaluates the remaining seconds and returns the warning to announce (or
    /// None). Mirrors the original inline branch order: the 15-second warning is
    /// checked before the 5-second urgent, and each fires at most once per turn.
    /// </summary>
    public TurnTimerWarningLevel Evaluate(float remainingSeconds)
    {
        TurnTimerWarningLevel level = TurnTimerWarningLevel.None;

        if (!_warningFired && remainingSeconds <= 15f && remainingSeconds > 0f)
        {
            _warningFired = true;
            level = TurnTimerWarningLevel.Warning;
        }
        else if (!_urgentFired && remainingSeconds <= 5f && remainingSeconds > 0f)
        {
            _urgentFired = true;
            level = TurnTimerWarningLevel.Urgent;
        }

        // Timer went back up — a new turn started, so allow warnings again.
        if (remainingSeconds > 20f)
        {
            _warningFired = false;
            _urgentFired = false;
        }

        return level;
    }

    /// <summary>Clears the fired state (e.g. on a turn change or new game).</summary>
    public void Reset()
    {
        _warningFired = false;
        _urgentFired = false;
    }
}
