namespace Arad.Net.Core.Informix.System.Data.ProviderBase;

internal class TimeoutTimer
{
	private long _timerExpire;

	private bool _isInfiniteTimeout;

	internal static readonly long InfiniteTimeout;

	internal bool IsExpired
	{
		get
		{
			if (!IsInfinite)
			{
				return Common.ADP.TimerHasExpired(_timerExpire);
			}
			return false;
		}
	}

	internal bool IsInfinite => _isInfiniteTimeout;

	internal long LegacyTimerExpire
	{
		get
		{
			if (!_isInfiniteTimeout)
			{
				return _timerExpire;
			}
			return long.MaxValue;
		}
	}

	internal long MillisecondsRemaining
	{
		get
		{
			long num;
			if (_isInfiniteTimeout)
			{
				num = long.MaxValue;
			}
			else
			{
				num = System.Data.Common.ADP.TimerRemainingMilliseconds(_timerExpire);
				if (0 > num)
				{
					num = 0L;
				}
			}
			return num;
		}
	}

	internal static TimeoutTimer StartSecondsTimeout(int seconds)
	{
		TimeoutTimer timeoutTimer = new TimeoutTimer();
		timeoutTimer.SetTimeoutSeconds(seconds);
		return timeoutTimer;
	}

	internal static TimeoutTimer StartMillisecondsTimeout(long milliseconds)
	{
		TimeoutTimer timeoutTimer = new TimeoutTimer();
		timeoutTimer._timerExpire = checked(System.Data.Common.ADP.TimerCurrent() + milliseconds * 10000);
		timeoutTimer._isInfiniteTimeout = false;
		return timeoutTimer;
	}

	internal void SetTimeoutSeconds(int seconds)
	{
		if (InfiniteTimeout == seconds)
		{
			_isInfiniteTimeout = true;
			return;
		}
		_timerExpire = checked(System.Data.Common.ADP.TimerCurrent() + System.Data.Common.ADP.TimerFromSeconds(seconds));
		_isInfiniteTimeout = false;
	}
}
