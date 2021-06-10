using cAlgo.API;
using cAlgo.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace cAlgo.Patterns
{
    public class PitchfanPattern : FanPatternBase
    {
        private ChartTrendLine _handleLine;

        public PitchfanPattern(PatternConfig config, SideFanSettings[] sideFanSettings, FanSettings mainFanSettings) : base("Pitchfan", config, sideFanSettings, mainFanSettings)
        {
            CallStopDrawing = false;
        }

        protected override void OnDrawingStopped()
        {
            _handleLine = null;

            base.OnDrawingStopped();
        }

        protected override void OnMouseUp(ChartMouseEventArgs obj)
        {
            if (MouseUpNumber < 2)
            {
                base.OnMouseUp(obj);

                return;
            }

            if (MouseUpNumber == 2)
            {
                var name = GetObjectName("HandleLine");

                _handleLine = Chart.DrawTrendLine(name, obj.TimeValue, obj.YValue, obj.TimeValue, obj.YValue, MainFanSettings.Color, MainFanSettings.Thickness, MainFanSettings.Style);

                _handleLine.IsInteractive = true;

                base.OnMouseUp(obj);
            }
            else
            {
                FinishDrawing();
            }
        }

        protected override void OnMouseMove(ChartMouseEventArgs obj)
        {
            if (MouseUpNumber < 2 || _handleLine == null)
            {
                base.OnMouseMove(obj);

                return;
            }

            _handleLine.Time2 = obj.TimeValue;
            _handleLine.Y2 = obj.YValue;

            var handleLineBarsNumber = _handleLine.GetBarsNumber(Chart.Bars, Chart.Symbol);

            var mainFanLineSecondBarIndex = _handleLine.GetStartBarIndex(Chart.Bars, Chart.Symbol) + handleLineBarsNumber / 2;

            MainFanLine.Time2 = Chart.Bars.GetOpenTime(mainFanLineSecondBarIndex, Chart.Symbol);
            MainFanLine.Y2 = _handleLine.GetBottomPrice() + _handleLine.GetPriceDelta() / 2;

            DrawSideFans(MainFanLine);
        }

        protected override void DrawSideFans(ChartTrendLine mainFan)
        {
            if (MouseUpNumber < 2) return;

            var endBarIndex = Chart.Bars.GetBarIndex(mainFan.Time2, Chart.Symbol);

            var barsDelta = _handleLine.GetBarsNumber(Chart.Bars, Chart.Symbol) / 2;
            var priceDelta = _handleLine.GetPriceDelta() / 2;

            var slope = _handleLine.GetSlope();

            for (var iFan = 0; iFan < SideFanSettings.Length; iFan++)
            {
                var fanSettings = SideFanSettings[iFan];

                var secondBarIndex = slope > 0 ? endBarIndex + barsDelta * fanSettings.Percent : endBarIndex - barsDelta * fanSettings.Percent;

                var secondTime = Chart.Bars.GetOpenTime(secondBarIndex, Chart.Symbol);

                var secondPrice = mainFan.Y2 + priceDelta * fanSettings.Percent;

                var objectName = GetObjectName(string.Format("SideFan_{0}", fanSettings.Percent));

                var trendLine = Chart.DrawTrendLine(objectName, mainFan.Time1, mainFan.Y1, secondTime, secondPrice, fanSettings.Color, fanSettings.Thickness, fanSettings.Style);

                trendLine.IsInteractive = true;
                trendLine.IsLocked = true;
                trendLine.ExtendToInfinity = true;

                SideFanLines[fanSettings.Percent] = trendLine;
            }
        }

        protected override void OnPatternChartObjectsUpdated(long id, ChartObject updatedChartObject, ChartObject[] patternObjects)
        {
            if (updatedChartObject.ObjectType != ChartObjectType.TrendLine) return;

            var trendLines = patternObjects.Where(iObject => iObject.ObjectType == ChartObjectType.TrendLine).Cast<ChartTrendLine>().ToArray();

            var mainFan = trendLines.First(iLine => iLine.Name.IndexOf("MainFan", StringComparison.OrdinalIgnoreCase) > -1);

            var handleLine = trendLines.First(iLine => iLine.Name.IndexOf("HandleLine", StringComparison.OrdinalIgnoreCase) > -1);

            if ((mainFan == null || handleLine == null) || (updatedChartObject != mainFan && updatedChartObject != handleLine)) return;

            if (updatedChartObject == mainFan)
            {
                UpdateHandleLine(handleLine, mainFan);
            }
            else if (updatedChartObject == handleLine)
            {
                UpdateMainFan(handleLine, mainFan);
            }

            var fans = trendLines.Where(iLine => iLine.Name.IndexOf("SideFan", StringComparison.OrdinalIgnoreCase) > -1).ToDictionary(iLine => double.Parse(iLine.Name.Split('_').Last(), CultureInfo.InvariantCulture));

            if (fans.Count > 0) UpdateFans(mainFan, handleLine, fans);
        }

        private void UpdateHandleLine(ChartTrendLine handleLine, ChartTrendLine mainFan)
        {
            var mainFanSecondBarIndex = Chart.Bars.GetBarIndex(mainFan.Time2, Chart.Symbol);
            var handleLineHalfBarsNumber = handleLine.GetBarsNumber(Chart.Bars, Chart.Symbol) / 2;

            var firstBarIndex = mainFanSecondBarIndex - handleLineHalfBarsNumber;
            var secondBarIndex = mainFanSecondBarIndex + handleLineHalfBarsNumber;

            handleLine.Time1 = Chart.Bars.GetOpenTime(firstBarIndex, Chart.Symbol);
            handleLine.Time2 = Chart.Bars.GetOpenTime(secondBarIndex, Chart.Symbol);

            var handleLineHalfPriceDelta = handleLine.GetPriceDelta() / 2;

            var handleLineSlope = handleLine.GetSlope();

            handleLine.Y1 = handleLineSlope > 0 ? mainFan.Y2 - handleLineHalfPriceDelta : mainFan.Y2 + handleLineHalfPriceDelta;
            handleLine.Y2 = handleLineSlope > 0 ? mainFan.Y2 + handleLineHalfPriceDelta : mainFan.Y2 - handleLineHalfPriceDelta;
        }

        private void UpdateMainFan(ChartTrendLine handleLine, ChartTrendLine mainFan)
        {
            var handleLineBarsNumber = handleLine.GetBarsNumber(Chart.Bars, Chart.Symbol);

            var mainFanLineSecondBarIndex = handleLine.GetStartBarIndex(Chart.Bars, Chart.Symbol) + handleLineBarsNumber / 2;

            mainFan.Time2 = Chart.Bars.GetOpenTime(mainFanLineSecondBarIndex, Chart.Symbol);
            mainFan.Y2 = handleLine.GetBottomPrice() + handleLine.GetPriceDelta() / 2;
        }

        private void UpdateFans(ChartTrendLine mainFan, ChartTrendLine handleLine, Dictionary<double, ChartTrendLine> fans)
        {
            var endBarIndex = Chart.Bars.GetBarIndex(mainFan.Time2, Chart.Symbol);

            var barsDelta = handleLine.GetBarsNumber(Chart.Bars, Chart.Symbol) / 2;
            var priceDelta = handleLine.GetPriceDelta() / 2;

            var slope = handleLine.GetSlope();

            foreach (var fan in fans)
            {
                var fanSettings = SideFanSettings.FirstOrDefault(iSettings => iSettings.Percent == fan.Key);

                if (fanSettings == null) continue;

                var secondBarIndex = slope > 0 ? endBarIndex + barsDelta * fanSettings.Percent : endBarIndex - barsDelta * fanSettings.Percent;

                var secondTime = Chart.Bars.GetOpenTime(secondBarIndex, Chart.Symbol);

                var secondPrice = mainFan.Y2 + priceDelta * fanSettings.Percent;

                var fanLine = fan.Value;

                fanLine.Time1 = mainFan.Time1;
                fanLine.Time2 = secondTime;

                fanLine.Y1 = mainFan.Y1;
                fanLine.Y2 = secondPrice;
            }
        }

        protected override ChartObject[] GetFrontObjects()
        {
            return new ChartObject[] { MainFanLine, _handleLine };
        }
    }
}