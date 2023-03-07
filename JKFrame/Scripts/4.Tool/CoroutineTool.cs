using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JKFrame
{
    /// <summary>
    /// 协程工具，避免GC
    /// </summary>
    public static class CoroutineTool
    {
        private static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        private static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        private static YieldInstruction yieldInstruction = new YieldInstruction();
        public static WaitForEndOfFrame WaitForEndOfFrame()
        {
            return waitForEndOfFrame;
        }
        public static WaitForFixedUpdate WaitForFixedUpdate()
        {
            return waitForFixedUpdate;
        }
        public static IEnumerator WaitForSeconds(float time)
        {
            float currTime = 0;
            while (currTime < time)
            {
                currTime += Time.deltaTime;
                yield return yieldInstruction;
            }
        }

        public static IEnumerator WaitForSecondsRealtime(float time)
        {
            float currTime = 0;
            while (currTime < time)
            {
                currTime += Time.unscaledDeltaTime;
                yield return yieldInstruction;
            }
        }

        public static IEnumerator WaitForFrames(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                yield return yieldInstruction;
            }
        }

    }

}
