using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace phiplayerChartConverter
{
    public class Phigros
    {
        public static Chart.Root GetJsonToObject(string chartJson) =>
            JsonConvert.DeserializeObject<Chart.Root>(chartJson);

        public static string GetObjectToJson(Chart.Root chartObject) =>
            JsonConvert.SerializeObject(chartObject);

        /// <summary>
        /// Convert "formatVersion = 1" chart to "formatVersion = 3"
        /// </summary>
        /// <param name="chartObject">Any "formatVersion = 1" chart</param>
        /// <returns>"formatVersion = 1" chart</returns>
        public static Chart.Root Fv1ToFv3(Chart.Root chartObject)
        {
            var obj = new Chart.Root
            {
                formatVersion = 3,
                offset = chartObject.offset,
                numOfNotes = chartObject.numOfNotes,
                judgeLineList = new Chart.JudgeLine[chartObject.judgeLineList.Length]
            };
            for (int i = 0; i < chartObject.judgeLineList.Length; i++)
            {
                var sourceLine = chartObject.judgeLineList[i];
                var targetLine = obj.judgeLineList[i] = new Chart.JudgeLine
                {
                    bpm = sourceLine.bpm,
                    notesAbove = sourceLine.notesAbove,
                    notesBelow = sourceLine.notesBelow,
                    numOfNotesAbove = sourceLine.numOfNotesAbove,
                    numOfNotesBelow = sourceLine.numOfNotesBelow,
                    numOfNotes = sourceLine.numOfNotes,
                    speedEvents = new Chart.SpeedEvent[sourceLine.speedEvents.Length],
                    judgeLineMoveEvents = new Chart.JudgeLineEvent[sourceLine.judgeLineMoveEvents.Length],
                    judgeLineDisappearEvents = new Chart.JudgeLineEvent[sourceLine.judgeLineDisappearEvents.Length],
                    judgeLineRotateEvents = new Chart.JudgeLineEvent[sourceLine.judgeLineRotateEvents.Length]
                };
                for (int j = 0; j < sourceLine.speedEvents.Length; j++)
                {
                    var se = sourceLine.speedEvents[j];
                    targetLine.speedEvents[j] = new Chart.SpeedEvent
                    {
                        startTime = se.startTime,
                        endTime = se.endTime,
                        value = se.value,
                        floorPosition = GetFloorPosition(sourceLine.speedEvents, se.startTime, sourceLine.bpm)
                    };
                }
                for (int j = 0; j < sourceLine.judgeLineMoveEvents.Length; j++)
                {
                    var me = sourceLine.judgeLineMoveEvents[j];
                    targetLine.judgeLineMoveEvents[j] = new Chart.JudgeLineEvent
                    {
                        startTime = me.startTime,
                        endTime = me.endTime,
                        start = ConvertValue1ToValue3(me.start).x3,
                        end = ConvertValue1ToValue3(me.end).y3,
                        start2 = 0,
                        end2 = 0
                    };
                }
                for (int j = 0; j < sourceLine.judgeLineDisappearEvents.Length; j++)
                {
                    var de = sourceLine.judgeLineDisappearEvents[j];
                    targetLine.judgeLineDisappearEvents[j] = new Chart.JudgeLineEvent
                    {
                        startTime = de.startTime,
                        endTime = de.endTime,
                        start = de.start,
                        end = de.end,
                        start2 = 0,
                        end2 = 0
                    };
                }
                for (int j = 0; j < sourceLine.judgeLineRotateEvents.Length; j++)
                {
                    var re = sourceLine.judgeLineRotateEvents[j];
                    targetLine.judgeLineRotateEvents[j] = new Chart.JudgeLineEvent
                    {
                        startTime = re.startTime,
                        endTime = re.endTime,
                        start = re.start,
                        end = re.end,
                        start2 = 0,
                        end2 = 0
                    };
                }
            }
            return obj;
        }

        /// <summary>
        /// Convert Phigros Ver 100 chart to Phigros Ver 250+ chart
        /// </summary>
        /// <param name="chartObject">Any chart</param>
        /// <returns>"formatVersion = 3" chart</returns>
        public static Chart.Root Pgr100ToPgr250(Chart.Root chartObject)
        {
            var obj = new Chart.Root
            {
                formatVersion = 3,
                offset = chartObject.offset,
                numOfNotes = null,
                judgeLineList = new Chart.JudgeLine[chartObject.judgeLineList.Length]
            };
            for (int i = 0; i < chartObject.judgeLineList.Length; i++)
            {
                var sourceLine = chartObject.judgeLineList[i];
                var targetLine = obj.judgeLineList[i] = new Chart.JudgeLine
                {
                    bpm = sourceLine.bpm,
                    notesAbove = sourceLine.notesAbove,
                    notesBelow = sourceLine.notesBelow,
                    numOfNotesAbove = null,
                    numOfNotesBelow = null,
                    numOfNotes = null,
                    speedEvents = new Chart.SpeedEvent[sourceLine.speedEvents.Length],
                    judgeLineMoveEvents = new Chart.JudgeLineEvent[sourceLine.judgeLineMoveEvents.Length],
                    judgeLineDisappearEvents = new Chart.JudgeLineEvent[sourceLine.judgeLineDisappearEvents.Length],
                    judgeLineRotateEvents = new Chart.JudgeLineEvent[sourceLine.judgeLineRotateEvents.Length]
                };
                for (int j = 0; j < sourceLine.speedEvents.Length; j++)
                {
                    var se = sourceLine.speedEvents[j];
                    targetLine.speedEvents[j] = new Chart.SpeedEvent
                    {
                        startTime = se.startTime,
                        endTime = se.endTime,
                        value = se.value,
                        floorPosition = GetFloorPosition(sourceLine.speedEvents, se.startTime, sourceLine.bpm)
                    };
                }
                for (int j = 0; j < sourceLine.judgeLineMoveEvents.Length; j++)
                {
                    var me = sourceLine.judgeLineMoveEvents[j];
                    targetLine.judgeLineMoveEvents[j] = new Chart.JudgeLineEvent
                    {
                        startTime = me.startTime,
                        endTime = me.endTime,
                        start = me.start,
                        start2 = me.start2,
                        end = me.end,
                        end2 = me.end2.Value
                    };
                }
                for (int j = 0; j < sourceLine.judgeLineDisappearEvents.Length; j++)
                {
                    var de = sourceLine.judgeLineDisappearEvents[j];
                    targetLine.judgeLineDisappearEvents[j] = new Chart.JudgeLineEvent
                    {
                        startTime = de.startTime,
                        endTime = de.endTime,
                        start = de.start,
                        end = de.end,
                        start2 = null,
                        end2 = null
                    };
                }
                for (int j = 0; j < sourceLine.judgeLineRotateEvents.Length; j++)
                {
                    var re = sourceLine.judgeLineRotateEvents[j];
                    targetLine.judgeLineRotateEvents[j] = new Chart.JudgeLineEvent
                    {
                        startTime = re.startTime,
                        endTime = re.endTime,
                        start = re.start,
                        end = re.end,
                        start2 = null,
                        end2 = null
                    };
                }
            }
            return obj;
        }

        public static Base.Chart.Root PhigrosChartToBase(Chart.Root chartObject)
        {
            var obj = new Base.Chart.Root()
            {
                formatVersion = 5,
                offset = chartObject.offset
            };

            var judgeLineList = new List<Base.Chart.JudgeLine>();
            foreach (var line in chartObject.judgeLineList)
            {
                var notes = new List<Base.Chart.Note>();
                foreach (var note in line.notesAbove)
                {
                    notes.Add(new Base.Chart.Note
                    {
                        type = note.type,
                        above = true,
                        startTime = CTMF(new[] { note.time, 32 }),
                        speed = note.speed,
                        endTime = CTMF(new[] { note.time + note.holdTime, 32 }),
                        positionX = note.positionX
                    });
                }
                foreach (var note in line.notesBelow)
                {
                    notes.Add(new Base.Chart.Note
                    {
                        type = note.type,
                        above = false,
                        startTime = CTMF(new[] { note.time, 32 }),
                        speed = note.speed,
                        endTime = CTMF(new[] { note.time + note.holdTime, 32 }),
                        positionX = note.positionX
                    });
                }
                notes = notes.OrderBy(n => GetBeat(n.startTime)).ToList();

                var speedEvents = new List<Base.Chart.JudgeLineEvent>();
                foreach (var item in line.speedEvents)
                {
                    speedEvents.Add(new Base.Chart.JudgeLineEvent
                    {
                        startTime = CTMF(new[] { item.startTime, 32 }),
                        endTime = CTMF(new[] { item.endTime, 32 }),
                        start = item.value * (float)speedScale,
                        end = item.value * (float)speedScale
                    });
                }
                speedEvents = speedEvents.OrderBy(s => GetBeat(s.startTime)).ToList();

                var eventLayers = new List<Base.Chart.JudgeLineEventLayer>();
                var moveXEvents = new List<Base.Chart.JudgeLineEvent>();
                var moveYEvents = new List<Base.Chart.JudgeLineEvent>();
                var rotateEvents = new List<Base.Chart.JudgeLineEvent>();
                var disappearEvents = new List<Base.Chart.JudgeLineEvent>();
                foreach (var item in line.judgeLineMoveEvents)
                {
                    moveXEvents.Add(new Base.Chart.JudgeLineEvent()
                    {
                        start = item.start,
                        end = item.end,
                        startTime = new[] { item.startTime, 32 },
                        endTime = new[] { item.endTime, 32 }
                    });
                }
                moveXEvents = moveXEvents.OrderBy(s => GetBeat(s.startTime)).ToList();
                foreach (var item in line.judgeLineMoveEvents)
                {
                    moveYEvents.Add(new Base.Chart.JudgeLineEvent()
                    {
                        start = item.start2.Value,
                        end = item.end2.Value,
                        startTime = new[] { item.startTime, 32 },
                        endTime = new[] { item.endTime, 32 }
                    });
                }
                moveYEvents = moveYEvents.OrderBy(s => GetBeat(s.startTime)).ToList();
                foreach (var item in line.judgeLineRotateEvents)
                {
                    rotateEvents.Add(new Base.Chart.JudgeLineEvent()
                    {
                        start = item.start,
                        end = item.end,
                        startTime = new[] { item.startTime, 32 },
                        endTime = new[] { item.endTime, 32 }
                    });
                }
                rotateEvents = rotateEvents.OrderBy(s => GetBeat(s.startTime)).ToList();
                foreach (var item in line.judgeLineDisappearEvents)
                {
                    disappearEvents.Add(new Base.Chart.JudgeLineEvent()
                    {
                        start = item.start,
                        end = item.end,
                        startTime = new[] { item.startTime, 32 },
                        endTime = new[] { item.endTime, 32 }
                    });
                }
                disappearEvents = disappearEvents.OrderBy(s => GetBeat(s.startTime)).ToList();
                eventLayers.Add(new Base.Chart.JudgeLineEventLayer()
                {
                    judgeLineMoveXEvents = moveXEvents.ToArray(),
                    judgeLineMoveYEvents = moveYEvents.ToArray(),
                    judgeLineRotateEvents = rotateEvents.ToArray(),
                    judgeLineDisappearEvents = disappearEvents.ToArray()
                });

                judgeLineList.Add(new Base.Chart.JudgeLine()
                {
                    bpms = new Base.Chart.BpmItems[]
                    {
                        new Base.Chart.BpmItems()
                        {
                            time = new[] { 0, 0, 1 },
                            bpm = line.bpm
                        }
                    },
                    notes = notes.ToArray(),
                    speedEvents = speedEvents.ToArray(),
                    judgeLineEventLayers = eventLayers.ToArray(),
                });
            }

            obj.judgeLineList = judgeLineList.ToArray();
            return obj;
        }
        private static float GetBeat(int[] beatArray) => beatArray[0] + beatArray[1] / (float)beatArray[2];
        public static int[] CTMF(int[] f)
        {
            if (f[0] == 0) return new[] { 0, 0, 1 };

            int g = GCD(Math.Abs(f[0]), Math.Abs(f[1]));
            int n = f[0] / g, d = f[1] / g;

            if (d < 0) { n = -n; d = -d; }

            int i = n / d, r = n % d;
            if (n < 0 && r != 0) i--;

            int num = n - i * d;
            return new[] { i, num, num == 0 ? 1 : d };
        }
        private static int GCD(int a, int b)
        {
            while (b != 0) (a, b) = (b, a % b);
            return a;
        }
        public const double speedScale = 6d;
        public static float ConvertValue3ToValue1(float x3, float y3) => 1000 * x3 * 880 + y3 * 520;
        public static (float x3, float y3) ConvertValue1ToValue3(float value1) => ((float)Math.Truncate(value1) / 880000f, value1 % 880000f);

        /// <summary>
        /// Force integration without relying on any auxiliary values.
        /// </summary>
        /// <param name="speedEvents">Speed events in any version</param>
        /// <returns></returns>
        public static float GetFloorPosition(Chart.SpeedEvent[] speedEvents, float curTime, float bpm)
        {
            float position = 0f;
            for (int i = 0; i < speedEvents.Length; i++)
            {
                var item = speedEvents[i];
                if (curTime >= GetCurTime(item.startTime, bpm) && curTime <= GetCurTime(item.endTime, bpm))
                {
                    for (int j = 0; j < i + 1; j++)
                    {
                        position += j != i ? (speedEvents[j].value * (GetCurTime(speedEvents[j].endTime, bpm) - GetCurTime(speedEvents[j].startTime, bpm)))
                            : (speedEvents[j].value * (curTime - GetCurTime(speedEvents[j].startTime, bpm)));
                    }
                    break;
                }
            }
            return position;
        }

        /// <summary>
        /// Convert directly to real time based on 32nd notes and BPM.
        /// </summary>
        /// <param name="beatTime"></param>
        /// <param name="bpm"></param>
        /// <returns></returns>
        public static float GetCurTime(int beatTime, float bpm) => (15f * beatTime) / (2 * bpm);

        /// <summary>
        /// Phigros Any Chart Class. (Expect "formatVersion = 2")
        /// </summary>
        [Serializable]
        public class Chart
        {
            [Serializable]
            public class Root
            {
                public int formatVersion { get; set; }
                public float offset { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public int? numOfNotes { get; set; }
                public JudgeLine[] judgeLineList { get; set; }
            }

            [Serializable]
            public class JudgeLine
            {
                public float bpm { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public int? numOfNotes { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public int? numOfNotesAbove { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public int? numOfNotesBelow { get; set; }
                public Note[] notesAbove { get; set; }
                public Note[] notesBelow { get; set; }
                public SpeedEvent[] speedEvents { get; set; }
                public JudgeLineEvent[] judgeLineDisappearEvents { get; set; }
                public JudgeLineEvent[] judgeLineMoveEvents { get; set; }
                public JudgeLineEvent[] judgeLineRotateEvents { get; set; }
            }

            [Serializable]
            public class Note
            {
                public int type { get; set; }
                [JsonConverter(typeof(FloatToIntConverter))]
                public int time { get; set; }
                public float positionX { get; set; }

                [JsonConverter(typeof(FloatToIntConverter))]
                public int holdTime { get; set; }
                public float speed { get; set; }
                public float floorPosition { get; set; }
            }

            [Serializable]
            public class SpeedEvent
            {
                [JsonConverter(typeof(FloatToIntConverter))]
                public int startTime { get; set; }
                [JsonConverter(typeof(FloatToIntConverter))]
                public int endTime { get; set; }
                public float value { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public float? floorPosition { get; set; }
            }

            [Serializable]
            public class JudgeLineEvent
            {
                [JsonConverter(typeof(FloatToIntConverter))]
                public int startTime { get; set; }
                [JsonConverter(typeof(FloatToIntConverter))]
                public int endTime { get; set; }
                public float start { get; set; }
                public float end { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public float? start2 { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public float? end2 { get; set; }
            }
        }

        /// <summary>
        /// Has been deleted.
        /// </summary>
        [Serializable]
        public class ChartFv2
        {

        }
    }

    public class FloatToIntConverter : JsonConverter<int>
    {
        public override int ReadJson(JsonReader reader, Type objectType, int existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is double d)
                return (int)Math.Round(d);
            if (reader.Value is float f)
                return (int)Math.Round(f);
            if (reader.Value is long l)
                return (int)l;
            return 0;
        }

        public override void WriteJson(JsonWriter writer, int value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}