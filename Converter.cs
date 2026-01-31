using System;
using System.Collections.Generic;
using System.Text;

namespace phiplayerChartConverter
{
    public static class Converter
    {
        public static string ConvertChart(string json, ChartType type)
        {
            switch (type)
            {
                case ChartType.phigros100:
                    return Base.GetObjectToJson(Phigros.PhigrosChartToBase(Phigros.Fv1ToFv3(Phigros.GetJsonToObject(json))));
                case ChartType.phigros250:
                    return Base.GetObjectToJson(Phigros.PhigrosChartToBase(Phigros.GetJsonToObject(json)));
                case ChartType.rephiedit170:
                    return Base.GetObjectToJson(RePhiedit.RePhieditChartToBase(RePhiedit.GetJsonToObject(json)));
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        public enum ChartType
        {
            phigros100,
            phigros250,
            rephiedit170
        }
    }
}
