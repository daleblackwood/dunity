using UnityEngine;

namespace Behave
{
    public class BWaitTime : BNode
    {
        float seconds = 0.0f;
        float upperSeconds = 0.0f;
        float waitSeconds = 0.0f;
        bool shouldSucceed = true;

        public BWaitTime(bool shouldSucceed, float seconds, float upperSeconds = -1.0f)
            : base("wait-" + seconds + "s")
        {
            this.seconds = seconds;
            this.upperSeconds = upperSeconds < 0.0f ? seconds : upperSeconds;
            this.shouldSucceed = shouldSucceed;
        }

        protected override void OnBegin()
        {
            waitSeconds = Random.Range(seconds, upperSeconds);
        }

        protected override BResult OnTick()
        {
            if (time < waitSeconds)
                return BResult.Ticking;

            return shouldSucceed ? BResult.Succeeded : BResult.Failed;
        }
    }
}