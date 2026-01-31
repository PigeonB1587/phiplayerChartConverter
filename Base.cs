using Newtonsoft.Json;
using System;
using System.Linq;

namespace phiplayerChartConverter
{
    public class Base
    {
        public static Chart.Root GetJsonToObject(string chartJson) =>
            JsonConvert.DeserializeObject<Chart.Root>(chartJson);

        public static string GetObjectToJson(Chart.Root chartObject) =>
            JsonConvert.SerializeObject(chartObject);

        /// <summary>
        /// PhiPlayer Chart Classes.
        /// </summary>
        [Serializable]
        public class Chart
        {
            [Serializable]
            public class Root
            {
                public int formatVersion { get; set; }
                public float offset { get; set; }
                public JudgeLine[] judgeLineList { get; set; }
            }

            [Serializable]
            public class JudgeLineEvent
            {
                public int[] startTime { get; set; }
                public int[] endTime { get; set; }
                public float start { get; set; }
                public float end { get; set; }
                public int easing { get; set; } = 1;
                public float easingLeft { get; set; } = 0.0f;
                public float easingRight { get; set; } = 1.0f;
                public float[] bezierPoints { get; set; } = Array.Empty<float>();
            }

            [Serializable]
            public class JudgeLineEventLayer
            {
                public JudgeLineEvent[] judgeLineMoveXEvents { get; set; } = Array.Empty<JudgeLineEvent>();
                public JudgeLineEvent[] judgeLineMoveYEvents { get; set; } = Array.Empty<JudgeLineEvent>();
                public JudgeLineEvent[] judgeLineRotateEvents { get; set; } = Array.Empty<JudgeLineEvent>();
                public JudgeLineEvent[] judgeLineDisappearEvents { get; set; } = Array.Empty<JudgeLineEvent>();
            }

            [Serializable]
            public class BpmItems
            {
                public int[] time { get; set; }
                public float bpm { get; set; }
            }

            [Serializable]
            public class Note
            {
                public int type { get; set; }
                public bool isFake { get; set; } = false;
                public bool above { get; set; }
                public int[] startTime { get; set; }
                public float speed { get; set; } = 1;
                public float size { get; set; } = 1;
                public int[] endTime { get; set; }
                public float positionX { get; set; }
                public int[] tint { get; set; } = new int[4] { 255, 255, 255, 255 };
                public int[] hitFXtint { get; set; } = Array.Empty<int>();
                public int key { get; set; } = 0;
            }

            [Serializable]
            public class JudgeLine
            {
                public BpmItems[] bpms { get; set; }
                public Note[] notes { get; set; }
                public JudgeLineEvent[] speedEvents { get; set; }
                public JudgeLineEventLayer[] judgeLineEventLayers { get; set; }
                public int fatherLineIndex { get; set; } = -1;
                public bool localPosition { get; set; } = false;
                public bool localEulerAngles { get; set; } = false;
            }
        }
    }
}
