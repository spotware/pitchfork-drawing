using cAlgo.API;
using cAlgo.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace cAlgo.Patterns
{
    public class SchiffPitchforkPattern : PatternBase
    {
        private readonly Dictionary<double, ChartTrendLine> _horizontalTrendLines = new Dictionary<double, ChartTrendLine>();
        private readonly Dictionary<double, ChartTrendLine> _verticalTrendLines = new Dictionary<double, ChartTrendLine>();
        private readonly LineSettings _medianLineSettings;
        private readonly Dictionary<double, PercentLineSettings> _levelsSettings;
        private ChartTrendLine _medianLine;
        private ChartTrendLine _handleLine;
        private ChartTrendLine _schiffLine;

        public SchiffPitchforkPattern(PatternConfig config, LineSettings medianLineSettings, Dictionary<double, PercentLineSettings> levelsSettings) : base("Schiff Pitchfork", config)
        {
            _medianLineSettings = medianLineSettings;
            _levelsSettings = levelsSettings;
        }

        protected override void OnPatternChartObjectsUpdated(long id, ChartObject updatedChartObject, ChartObject[] patternObjects)
        {
            if (updatedChartObject.ObjectType != ChartObjectType.TrendLine) return;

            var trendLines = patternObjects.Where(iObject => iObject.ObjectType == ChartObjectType.TrendLine).Cast<ChartTrendLine>().ToArray();

            var schiffLine = trendLines.FirstOrDefault(iObject => iObject.Name.Split('_').Last().Equals("SchiffLine", StringComparison.InvariantCultureIgnoreCase));

            if (schiffLine == null) return;

            var handleLine = trendLines.FirstOrDefault(iObject => iObject.Name.Split('_').Last().Equals("HandleLine", StringComparison.InvariantCultureIgnoreCase));

            if (handleLine == null) return;

            var medianLine = trendLines.FirstOrDefault(iObject => iObject.Name.Split('_').Last().Equals("MedianLine", StringComparison.InvariantCultureIgnoreCase));

            if (medianLine == null) return;

            if (updatedChartObject != schiffLine && updatedChartObject != handleLine) return;

            if (updatedChartObject == schiffLine)
            {
                UpdateHandleLine(handleLine, schiffLine);
            }
            else if (updatedChartObject == handleLine)
            {
                UpdateSchiffLine(handleLine, schiffLine);
            }

            UpdateMedianLine(medianLine, schiffLine, handleLine);

            DrawPercentLevels(medianLine, handleLine, id);
        }

        protected override void OnDrawingStopped()
        {
            _medianLine = null;
            _handleLine = null;
            _schiffLine = null;

            _horizontalTrendLines.Clear();
            _verticalTrendLines.Clear();
        }

        protected override void OnMouseUp(ChartMouseEventArgs obj)
        {
            if (MouseUpNumber == 3)
            {
                FinishDrawing();

                return;
            }

            if (_schiffLine == null)
            {
                var name = GetObjectName("SchiffLine");

                _schiffLine = Chart.DrawTrendLine(name, obj.TimeValue, obj.YValue, obj.TimeValue, obj.YValue, _medianLineSettings.LineColor, _medianLineSettings.Thickness, _medianLineSettings.Style);

                _schiffLine.IsInteractive = true;
            }
            else if (_handleLine == null)
            {
                var name = GetObjectName("HandleLine");

                _handleLine = Chart.DrawTrendLine(name, obj.TimeValue, obj.YValue, obj.TimeValue, obj.YValue, _medianLineSettings.LineColor, _medianLineSettings.Thickness, _medianLineSettings.Style);

                _handleLine.IsInteractive = true;

                var medianLineName = GetObjectName("MedianLine");

                _medianLine = Chart.DrawTrendLine(medianLineName, _schiffLine.Time1, _schiffLine.Y1, _schiffLine.Time2, _handleLine.Y2, _medianLineSettings.LineColor, _medianLineSettings.Thickness, _medianLineSettings.Style);

                _medianLine.IsInteractive = true;
                _medianLine.IsLocked = true;
                _medianLine.ExtendToInfinity = true;
            }
        }

        protected override void OnMouseMove(ChartMouseEventArgs obj)
        {
            if (_schiffLine == null) return;

            if (_handleLine == null)
            {
                _schiffLine.Time2 = obj.TimeValue;
                _schiffLine.Y2 = obj.YValue;
            }
            else
            {
                _handleLine.Time2 = obj.TimeValue;
                _handleLine.Y2 = obj.YValue;

                UpdateMedianLine(_medianLine, _schiffLine, _handleLine);

                DrawPercentLevels(_medianLine, _handleLine, Id);
            }
        }

        protected override ChartObject[] GetFrontObjects()
        {
            return new ChartObject[] { _schiffLine, _handleLine };
        }

        private void DrawPercentLevels(ChartTrendLine medianLine, ChartTrendLine handleLine, long id)
        {
            var medianLineSecondBarIndex = Chart.Bars.GetBarIndex(medianLine.Time2, Chart.Symbol);
            var handleLineFirstBarIndex = Chart.Bars.GetBarIndex(handleLine.Time1, Chart.Symbol);

            var barsDelta = Math.Abs(medianLineSecondBarIndex - handleLineFirstBarIndex);
            var lengthInMinutes = Math.Abs((medianLine.Time2 - handleLine.Time1).TotalMinutes) * 2;
            var priceDelta = handleLine.GetPriceDelta() / 2;

            var handleLineSlope = handleLine.GetSlope();

            foreach (var levelSettings in _levelsSettings)
            {
                DrawLevel(medianLine, medianLineSecondBarIndex, barsDelta, lengthInMinutes, priceDelta, handleLineSlope, levelSettings.Value.Percent, levelSettings.Value.LineColor, id);
                DrawLevel(medianLine, medianLineSecondBarIndex, barsDelta, lengthInMinutes, priceDelta, handleLineSlope, -levelSettings.Value.Percent, levelSettings.Value.LineColor, id);
            }
        }

        private void DrawLevel(ChartTrendLine medianLine, double medianLineSecondBarIndex, double barsDelta, double lengthInMinutes, double priceDelta, double handleLineSlope, double percent, Color lineColor, long id)
        {
            var barsPercent = barsDelta * percent;

            var firstBarIndex = handleLineSlope > 0 ? medianLineSecondBarIndex + barsPercent : medianLineSecondBarIndex - barsPercent;
            var firstTime = Chart.Bars.GetOpenTime(firstBarIndex, Chart.Symbol);
            var firstPrice = medianLine.Y2 + priceDelta * percent;

            var secondTime = medianLine.Time1 > medianLine.Time2 ? firstTime.AddMinutes(-lengthInMinutes) : firstTime.AddMinutes(lengthInMinutes);

            var priceDistanceWithMediumLine = Math.Abs(medianLine.CalculateY(firstTime) - medianLine.CalculateY(secondTime));

            var secondPrice = medianLine.Y2 > medianLine.Y1 ? firstPrice + priceDistanceWithMediumLine : firstPrice - priceDistanceWithMediumLine;

            var name = GetObjectName(string.Format("Level_{0}", percent.ToString(CultureInfo.InvariantCulture)), id: id);

            var line = Chart.DrawTrendLine(name, firstTime, firstPrice, secondTime, secondPrice, lineColor);

            line.ExtendToInfinity = true;
            line.IsInteractive = true;
            line.IsLocked = true;
        }

        private void UpdateMedianLine(ChartTrendLine medianLine, ChartTrendLine schiffLine, ChartTrendLine handleLine)
        {
            medianLine.Time1 = schiffLine.Time1;
            medianLine.Y1 = schiffLine.GetBottomPrice() + schiffLine.GetPriceDelta() / 2;

            var handleLineStartBarIndex = handleLine.GetStartBarIndex(Chart.Bars, Chart.Symbol);
            var handleLineBarsNumber = handleLine.GetBarsNumber(Chart.Bars, Chart.Symbol);

            medianLine.Time2 = Chart.Bars.GetOpenTime(handleLineStartBarIndex + handleLineBarsNumber / 2, Chart.Symbol);
            medianLine.Y2 = handleLine.GetBottomPrice() + handleLine.GetPriceDelta() / 2;
        }

        private void UpdateHandleLine(ChartTrendLine handleLine, ChartTrendLine schiffLine)
        {
            handleLine.Time1 = schiffLine.Time2;
            handleLine.Y1 = schiffLine.Y2;
        }

        private void UpdateSchiffLine(ChartTrendLine handleLine, ChartTrendLine schiffLine)
        {
            schiffLine.Time2 = handleLine.Time1;
            schiffLine.Y2 = handleLine.Y1;
        }
    }
}