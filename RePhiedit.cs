using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace phiplayerChartConverter
{
    public class RePhiedit
    {
        public static Chart.Root GetJsonToObject(string chartJson) =>
            JsonConvert.DeserializeObject<Chart.Root>(chartJson);

        public static string GetObjectToJson(Chart.Root chartObject) =>
            JsonConvert.SerializeObject(chartObject);

        /// <summary>
        /// Convert RePhiedit charts to Base chart format.
        /// </summary>
        /// <param name="chartObject">RePhiedit charts (170-)</param>
        /// <returns>Base chart format.(formatVersion = 5)</returns>
        public static Base.Chart.Root RePhieditChartToBase(Chart.Root chartObject)
        {
            var obj = new Base.Chart.Root()
            {
                formatVersion = 5,
                offset = (chartObject.META.offset / 1000f) * (-1f)
            };
            var bpmList = new List<Base.Chart.BpmItems>();
            foreach (var item in chartObject.BPMList)
            {
                bpmList.Add(new Base.Chart.BpmItems()
                {
                    bpm = item.bpm,
                    time = item.startTime
                });
            }

            var judgeLineList = new List<Base.Chart.JudgeLine>();
            foreach (var line in chartObject.judgeLineList)
            {
                var notes = new List<Base.Chart.Note>();
                foreach (var item in line.notes)
                {
                    notes.Add(new Base.Chart.Note()
                    {
                        type = RpeNoteTypeToBase(item.type),
                        above = item.above == 1,
                        isFake = item.isFake != 0,
                        tint = new int[4] { item.color[0], item.color[1], item.color[2], item.alpha },
                        startTime = item.startTime,
                        endTime = item.endTime,
                        positionX = item.positionX * 0.013169f,
                        size = item.size,
                        speed = item.speed,
                        hitFXtint = item.tintHitEffects,
                        key = 0
                    });
                }
                notes = notes.OrderBy(n => GetBeat(n.startTime)).ToList();
                var speedEvents = new List<Base.Chart.JudgeLineEvent>();
                foreach (var item in line.eventLayers[0].speedEvents)
                {
                    speedEvents.Add(new Base.Chart.JudgeLineEvent()
                    {
                        start = item.start * rpeToRhiSpeedScale,
                        end = item.end * rpeToRhiSpeedScale,
                        startTime = item.startTime,
                        endTime = item.endTime,
                        bezierPoints = item.bezier == 1 ? item.bezierPoints : Array.Empty<float>(),
                        easing = RpeEasingTypeToBase(item.easingType),
                        easingLeft = item.easingLeft,
                        easingRight = item.easingRight
                    });
                }
                speedEvents = speedEvents.OrderBy(s => GetBeat(s.startTime)).ToList();
                var eventLayers = new List<Base.Chart.JudgeLineEventLayer>();
                foreach (var layer in line.eventLayers)
                {
                    var moveXEvents = new List<Base.Chart.JudgeLineEvent>();
                    var moveYEvents = new List<Base.Chart.JudgeLineEvent>();
                    var rotateEvents = new List<Base.Chart.JudgeLineEvent>();
                    var disappearEvents = new List<Base.Chart.JudgeLineEvent>();
                    foreach (var item in layer.moveXEvents)
                    {
                        moveXEvents.Add(new Base.Chart.JudgeLineEvent()
                        {
                            start = item.start / 1350f,
                            end = item.end / 1350f,
                            startTime = item.startTime,
                            endTime = item.endTime,
                            bezierPoints = item.bezier == 1 ? item.bezierPoints : Array.Empty<float>(),
                            easing = RpeEasingTypeToBase(item.easingType),
                            easingLeft = item.easingLeft,
                            easingRight = item.easingRight
                        });
                    }
                    moveXEvents = moveXEvents.OrderBy(s => GetBeat(s.startTime)).ToList();
                    foreach (var item in layer.moveYEvents)
                    {
                        moveYEvents.Add(new Base.Chart.JudgeLineEvent()
                        {
                            start = item.start / 900f,
                            end = item.end / 900f,
                            startTime = item.startTime,
                            endTime = item.endTime,
                            bezierPoints = item.bezier == 1 ? item.bezierPoints : Array.Empty<float>(),
                            easing = RpeEasingTypeToBase(item.easingType),
                            easingLeft = item.easingLeft,
                            easingRight = item.easingRight
                        });
                    }
                    moveYEvents = moveYEvents.OrderBy(s => GetBeat(s.startTime)).ToList();
                    foreach (var item in layer.rotateEvents)
                    {
                        rotateEvents.Add(new Base.Chart.JudgeLineEvent()
                        {
                            start = -item.start,
                            end = -item.end,
                            startTime = item.startTime,
                            endTime = item.endTime,
                            bezierPoints = item.bezier == 1 ? item.bezierPoints : Array.Empty<float>(),
                            easing = RpeEasingTypeToBase(item.easingType),
                            easingLeft = item.easingLeft,
                            easingRight = item.easingRight
                        });
                    }
                    rotateEvents = rotateEvents.OrderBy(s => GetBeat(s.startTime)).ToList();
                    foreach (var item in layer.alphaEvents)
                    {
                        disappearEvents.Add(new Base.Chart.JudgeLineEvent()
                        {
                            start = item.start / 255f,
                            end = item.end / 255f,
                            startTime = item.startTime,
                            endTime = item.endTime,
                            bezierPoints = item.bezier == 1 ? item.bezierPoints : Array.Empty<float>(),
                            easing = RpeEasingTypeToBase(item.easingType),
                            easingLeft = item.easingLeft,
                            easingRight = item.easingRight
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
                }

                judgeLineList.Add(new Base.Chart.JudgeLine()
                {
                    fatherLineIndex = line.father,
                    localPosition = line.father != -1,
                    localEulerAngles = line.rotateWithFather,
                
                    bpms = bpmList.ToArray(),
                    notes = notes.ToArray(),
                    speedEvents = speedEvents.ToArray(),
                    judgeLineEventLayers = eventLayers.ToArray(),
                });
            }

            obj.judgeLineList = judgeLineList.ToArray();

            return obj;
        }
        public static float SecToBeat(Chart.BPMItem[] bpmList, float t, float bpmFactor)
        {
            float beat = 0.0f;
            for (int i = 0; i < bpmList.Length; i++)
            {
                var e = bpmList[i];
                float bpmv = e.bpm / bpmFactor;
                float currentBpmStartTime = GetBeat(e.startTime);

                if (i != bpmList.Length - 1)
                {
                    float nextBpmStartTime = GetBeat(bpmList[i + 1].startTime);
                    float etBeat = nextBpmStartTime - currentBpmStartTime;
                    float etSec = etBeat * (60 / bpmv);

                    if (t >= etSec)
                    {
                        beat += etBeat;
                        t -= etSec;
                    }
                    else
                    {
                        beat += t / (60 / bpmv);
                        break;
                    }
                }
                else
                {
                    beat += t / (60 / bpmv);
                }
            }
            return beat;
        }
        public static float BeatToSec(Chart.BPMItem[] bpmList, float t, float bpmFactor)
        {
            float sec = 0.0f;
            for (int i = 0; i < bpmList.Length; i++)
            {
                var e = bpmList[i];
                float bpmv = e.bpm / bpmFactor;
                float currentBpmStartTime = GetBeat(e.startTime);

                if (i != bpmList.Length - 1)
                {
                    float nextBpmStartTime = GetBeat(bpmList[i + 1].startTime);
                    float etBeat = nextBpmStartTime - currentBpmStartTime;

                    if (t >= etBeat)
                    {
                        sec += etBeat * (60 / bpmv);
                        t -= etBeat;
                    }
                    else
                    {
                        sec += t * (60 / bpmv);
                        break;
                    }
                }
                else
                {
                    sec += t * (60 / bpmv);
                }
            }
            return sec;
        }
        private static int[] GetBeatArray(float beat, double precision = 1e-6)
        {
            if (beat < 0)
            {
                var positive = GetBeatArray(-beat, precision);
                positive[0] = -positive[0];
                return positive;
            }

            int integerPart = (int)Math.Floor(beat);
            double fractionalPart = beat - integerPart;

            if (Math.Abs(fractionalPart) < precision)
            {
                return new[] { integerPart, 0, 1 };
            }

            double x = fractionalPart;
            int a = (int)Math.Floor(x);
            int h1 = 1, k1 = 0;
            int h2 = a, k2 = 1;
            double error;

            while (true)
            {
                x = 1 / (x - a);
                a = (int)Math.Floor(x);

                int h = a * h2 + h1;
                int k = a * k2 + k1;

                error = Math.Abs((double)h / k - fractionalPart);
                if (error < precision)
                {
                    int gcd = GCD(h, k);
                    return new[] { integerPart, h / gcd, k / gcd };
                }

                h1 = h2;
                k1 = k2;
                h2 = h;
                k2 = k;

                if (k > 1000000)
                {
                    int gcd = GCD(h2, k2);
                    return new[] { integerPart, h2 / gcd, k2 / gcd };
                }
            }
        }
        private static int GCD(int a, int b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }
        private static float GetBeat(int[] beatArray) => beatArray[0] + beatArray[1] / (float)beatArray[2];
        public static int RpeNoteTypeToBase(int i)
        {
            switch (i)
            {
                case 1:
                    return 1;
                case 2:
                    return 3;
                case 3:
                    return 4;
                case 4:
                    return 2;
                default:
                    return 1;
            }
        }
        public static int RpeEasingTypeToBase(int i)
        {
            switch (i)
            {
                case 1:
                    return 1;
                case 2:
                    return 3;
                case 3:
                    return 2;
                case 4:
                    return 6;
                case 5:
                    return 5;
                case 6:
                    return 4;
                case 7:
                    return 7;
                case 8:
                    return 9;
                case 9:
                    return 8;
                case 10:
                    return 12;
                case 11:
                    return 11;
                case 12:
                    return 10;
                case 13:
                    return 13;
                case 14:
                    return 15;
                case 15:
                    return 14;
                case 16:
                    return 18;
                case 17:
                    return 17;
                case 18:
                    return 21;
                case 19:
                    return 20;
                case 20:
                    return 24;
                case 21:
                    return 23;
                case 22:
                    return 22;
                case 23:
                    return 25;
                case 24:
                    return 27;
                case 25:
                    return 26;
                case 26:
                    return 30;
                case 27:
                    return 29;
                case 28:
                    return 31;
                case 29:
                    return 28;
                default:
                    return 1;
            }
        }

        public const float rpeToRhiSpeedScale = 1.3333333f;

        /// <summary>
        /// RePhiedit Chart Object
        /// </summary>
        [Serializable]
        public class Chart
        {
            [Serializable]
            public class BPMItem
            {
                public float bpm { get; set; }
                public int[] startTime { get; set; }
            }
            [Serializable]
            public class META
            {
                public int RPEVersion { get; set; }
                public int offset { get; set; }
            }
            [Serializable]
            public class JudgeLine
            {
                public string Texture { get; set; }
                public EventLayers[] eventLayers { get; set; }
                public Extended extended { get; set; }
                public Note[] notes { get; set; } = Array.Empty<Note>();
                public bool rotateWithFather { get; set; } = false;
                public int father { get; set; } = -1;
                public int zOrder { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public string attachUI { get; set; } = null;
                public PosControl[] posControl { get; set; } = Array.Empty<PosControl>();
                public sizeControls[] sizeControls { get; set; } = Array.Empty<sizeControls>();
                public YControl[] yControl { get; set; } = Array.Empty<YControl>();
                public AlphaControl[] alphaControl { get; set; } = Array.Empty<AlphaControl>();
            }
            [Serializable]
            public class AlphaControl
            {
                public int easing { get; set; }
                public float alpha { get; set; }
                public float x { get; set; }
            }
            [Serializable]
            public class sizeControls
            {
                public int easing { get; set; }
                public float size { get; set; }
                public float x { get; set; }
            }
            [Serializable]
            public class PosControl
            {
                public int easing { get; set; }
                public float pos { get; set; }
                public float x { get; set; }
            }
            [Serializable]
            public class YControl
            {
                public int easing { get; set; }
                public float y { get; set; }
                public float x { get; set; }
            }
            [Serializable]
            public class Note
            {
                public int above { get; set; }
                public int alpha { get; set; } = 1;

                [JsonProperty(PropertyName = "tint")]
                public int[] color { get; set; } = new int[3] { 255, 255, 255 };
                public int[] endTime { get; set; }
                public int isFake { get; set; }
                public float positionX { get; set; }
                public float size { get; set; }
                public float speed { get; set; }
                public int[] startTime { get; set; }
                public int type { get; set; }
                public float visibleTime { get; set; } = 999999;
                public float yOffset { get; set; } = 0;
                public float judgeArea { get; set; } = 1;
                public int[] tintHitEffects { get; set; } = Array.Empty<int>();
            }
            [Serializable]
            public class EventLayers
            {
                public Event[] alphaEvents { get; set; } = Array.Empty<Event>();
                public Event[] moveXEvents { get; set; } = Array.Empty<Event>();
                public Event[] moveYEvents { get; set; } = Array.Empty<Event>();
                public Event[] rotateEvents { get; set; } = Array.Empty<Event>();
                public Event[] speedEvents { get; set; } = Array.Empty<Event>();
            }
            [Serializable]
            public class Event
            {
                public int bezier { get; set; } = 0;
                public float[] bezierPoints { get; set; } = Array.Empty<float>();
                public float easingLeft { get; set; } = 0.0f;
                public float easingRight { get; set; } = 1.0f;
                public int easingType { get; set; } = 1;
                public float end { get; set; }
                public int[] endTime { get; set; }
                public float start { get; set; }
                public int[] startTime { get; set; }
            }
            [Serializable]
            public class ColorEvent
            {
                public int bezier { get; set; } = 0;
                public float[] bezierPoints { get; set; } = Array.Empty<float>();
                public float easingLeft { get; set; } = 0.0f;
                public float easingRight { get; set; } = 1.0f;
                public int easingType { get; set; } = 1;
                public int[] end { get; set; }
                public int[] endTime { get; set; }
                public int[] start { get; set; }
                public int[] startTime { get; set; }
            }
            [Serializable]
            public class TextEvent
            {
                public int bezier { get; set; } = 0;
                public float[] bezierPoints { get; set; } = Array.Empty<float>();
                public float easingLeft { get; set; } = 0.0f;
                public float easingRight { get; set; } = 1.0f;
                public int easingType { get; set; } = 1;
                public string end { get; set; }
                public int[] endTime { get; set; }
                public string start { get; set; }
                public int[] startTime { get; set; }
            }
            [Serializable]
            public class Extended
            {
                public Event[] scaleXEvents { get; set; } = Array.Empty<Event>();
                public Event[] scaleYEvents { get; set; } = Array.Empty<Event>();
                public ColorEvent[] colorEvents { get; set; } = Array.Empty<ColorEvent>();
                public TextEvent[] textEvents { get; set; } = Array.Empty<TextEvent>();
            }
            [Serializable]
            public class Root
            {
                public BPMItem[] BPMList { get; set; }
                public META META { get; set; }
                public JudgeLine[] judgeLineList { get; set; }
            }
        }
    }
}
