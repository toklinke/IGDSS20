using NUnit.Framework;

namespace Tests
{
    public class TestTimer
    {
        public readonly struct TimerTickedTestCase
        {
            public TimerTickedTestCase(
                string description,
                float tickInterval,
                float[] updateDeltas,
                int expectedTimerTickedCount
            )
            {
                Description = description;
                TickInterval = tickInterval;
                UpdateDeltas = updateDeltas;
                ExpectedTimerTickedCount = expectedTimerTickedCount;
            }

            public string Description { get; }

            // Tick interval of timer.
            public float TickInterval { get; }
            // Time deltas to pass to Timer.Update()
            public float[] UpdateDeltas { get; }
            // Expected number of TimerTicked events raised.
            public int ExpectedTimerTickedCount { get; }

            public override string ToString()
            {
                return Description;
            }
        }

        private static TimerTickedTestCase[]
            TimerTickedTestCases = (
            new TimerTickedTestCase[] {
                new TimerTickedTestCase(
                    description: "no updates -> no events",
                    tickInterval: 1.0f,
                    updateDeltas: new float[] {},
                    expectedTimerTickedCount: 0
                ),
                new TimerTickedTestCase(
                    description: "tick interval elapsed exactly",
                    tickInterval: 1.0f,
                    updateDeltas: new float[] {1.0f},
                    expectedTimerTickedCount: 1
                ),
                new TimerTickedTestCase(
                    description: "elapsed more time than tick interval",
                    tickInterval: 1.0f,
                    updateDeltas: new float[] {1.1f},
                    expectedTimerTickedCount: 1
                ),
                new TimerTickedTestCase(
                    description: "elapsed less than tick interval",
                    tickInterval: 1.0f,
                    updateDeltas: new float[] {0.1f},
                    expectedTimerTickedCount: 0
                ),
                new TimerTickedTestCase(
                    description: (
                        "tick interval elapsed in multiple updates"
                    ),
                    tickInterval: 1.0f,
                    updateDeltas: new float[] {0.5f, 0.2f, 0.3f},
                    expectedTimerTickedCount: 1
                ),
                new TimerTickedTestCase(
                    description: (
                        "tick interval elapsed multiple times"
                    ),
                    tickInterval: 1.0f,
                    updateDeltas: new float[] {1.5f, 0.3f, 0.6f},
                    expectedTimerTickedCount: 2
                ),
                new TimerTickedTestCase(
                    description: (
                        "interval elapsed multiple times in single update"
                    ),
                    tickInterval: 1.0f,
                    updateDeltas: new float[] {2.5f},
                    expectedTimerTickedCount: 2
                ),
            }
        );

        [Test, TestCaseSource("TimerTickedTestCases")]
        public void TestTimerTicked(
            TimerTickedTestCase testCase
        )
        {
            var timer = new Timer(testCase.TickInterval);

            int timerTickedCount = 0;
            timer.TimerTicked += (sender, args) => {
                ++timerTickedCount;
            };

            foreach (var updateDelta in testCase.UpdateDeltas)
            {
                timer.Update(updateDelta);
            }

            Assert.That(
                timerTickedCount,
                Is.EqualTo(testCase.ExpectedTimerTickedCount)
            );
        }
    }
}


