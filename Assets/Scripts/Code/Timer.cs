using System;

// Generates tick events in fixed intervals.
public interface ITimer
{
    // The defined interval has elapsed.
    event EventHandler<EventArgs> TimerTicked;
};

public class Timer : ITimer
{
    public float TickInterval { get; }

    public event EventHandler<EventArgs> TimerTicked;

    private float Elapsed;

    // Set tick interval for timer.
    // tickInterval: Interval in seconds.
    public Timer(float tickInterval)
    {
        TickInterval = tickInterval;
        Elapsed = 0.0f;
    }

    // Update the timer and raise a TimerTicked event if needed.
    // elapsed: Time since last update in seconds.
    public void Update(float elapsed)
    {
        Elapsed += elapsed;
        while (Elapsed >= TickInterval)
        {
            OnTimerTicked();
            Elapsed -= TickInterval;
        }
    }

    private void OnTimerTicked()
    {
        EventHandler<EventArgs> handler = TimerTicked;
        if (handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }
};

