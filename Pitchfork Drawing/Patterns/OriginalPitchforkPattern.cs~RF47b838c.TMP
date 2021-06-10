using cAlgo.API;
using cAlgo.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace cAlgo.Patterns
{
    public class OriginalPitchforkPattern : PatternBase
    {
        private readonly Dictionary<double, ChartTrendLine> _horizontalTrendLines = new Dictionary<double, ChartTrendLine>();
        private readonly Dictionary<double, ChartTrendLine> _verticalTrendLines = new Dictionary<double, ChartTrendLine>();
        private readonly LineSettings _medianLineSettings;
        private readonly Dictionary<double, PercentLineSettings> _levelsSettings;
        private ChartTrendLine _medianLine;
        private ChartTrendLine _controllerLine;

        public OriginalPitchforkPattern(PatternConfig config, LineSettings medianLineSettings, Dictionary<double, PercentLineSettings> levelsSettings) : base("Original Pitchfork", config)
        {
            _medianLineSettings = medianLineSettings;
            _levelsSettings = levelsSettings;
        }

        protected override void OnPatternChartObjectsUpdated(long id, ChartObject updatedChartObject, ChartObject[] patternObjects)
        {
            if (updatedChartObject.ObjectType != ChartObjectType.TrendLine) return;

            var trendLines = patternObjects.Where(iObject => iObject.ObjectType == ChartObjectType.TrendLine).Cast<ChartTrendLine>().ToArray();

            var medianLine = trendLines.FirstOrDefault(iObject => iObject.Name.Split('_').Last().Equals("MedianLine", StringComparison.InvariantCultureIgnoreCase));

            if (medianLine == null) return;

            var controllerLine = trendLines.FirstOrDefault(iObject => iObject.Name.Split('_').Last().Equals("ControllerLine", StringComparison.InvariantCultureIgnoreCase));

            if (controllerLine == null) return;

            UpdateMedianLine(medianLine, controllerLine);

            DrawPercentLevels(medianLine, controllerLine, id);
        }

        protected override void OnDrawingStopped()
        {
            _medianLine = null;
            _controllerLine = null;

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

            if (_medianLine == null)
            {
                var name = GetObjectName("MedianLine");

                _medianLine = Chart.DrawTrendLine(name, obj.TimeValue, obj.YValue, obj.TimeValue, obj.YValue, _medianLineSettings.LineColor, _medianLineSettings.Thickness, _medianLineSettings.Style);

                _medianLine.IsInteractive = true;
                _medianLine.ExtendToInfinity = true;
            }
            else if (_controllerLine == null)
            {
                var name = GetObjectName("ControllerLine");

                _controllerLine = Chart.DrawTrendLine(name, obj.TimeValue, obj.YValue, obj.TimeValue, obj.YValue, _medianLineSettings.LineColor, _medianLineSettings.Thickness, _medianLineSettings.Style);

                _controllerLine.IsInteractive = true;
            }
        }

        protected override void OnMouseMove(ChartMouseEventArgs obj)
        {
            if (_medianLine == null) return;

            if (_controllerLine == null)
            {
                _medianLine.Time2 = obj.TimeValue;
                _medianLine.Y2 = obj.YValue;
            }
            else
            {
                _controllerLine.Time2 = obj.TimeValue;
                _controllerLine.Y2 = obj.YValue;

                UpdateMedianLine(_medianLine, _controllerLine);

                DrawPercentLevels(_medianLine, _controllerLine, Id);
            }
        }

        protected override ChartObject[] GetFrontObjects()
        {
            return new ChartObject[] { _medianLine, _controllerLine };
        }

        private void DrawPercentLevels(ChartTrendLine medianLine, ChartTrendLine controllerLine, long id)
        {
            var timeDeltaInMinutes = Math.Abs((medianLine.Time2 - controllerLine.Time1).TotalMinutes);
            var priceDelta = controllerLine.GetPriceDelta() / 2;

            var controllerLineSlope = controllerLine.GetSlope();

            foreach (var levelSettings in _levelsSettings)
            {
                DrawLevel(medianLine, timeDeltaInMinutes, priceDelta, controllerLineSlope, levelSettings.Value.Percent, levelSettings.Value.LineColor, id);
                DrawLevel(medianLine, timeDeltaInMinutes, priceDelta, controllerLineSlope, -levelSettings.Value.Percent, levelSettings.Value.LineColor, id);
            }
        }

        private void DrawLevel(ChartTrendLine medianLine, double timeDeltaInMinutes, double priceDelta, double controllerLineSlope, double percent, Color lineColor, long id)
        {
            var firstTime = controllerLineSlope > 0 ? medianLine.Time2.AddMinutes(timeDeltaInMinutes * percent) : medianLine.Time2.AddMinutes(-timeDeltaInMinutes * percent);
            var firstPrice = medianLine.Y2 + priceDelta * percent;

            var secondTime = medianLine.Time1 > medianLine.Time2 ? firstTime.AddMinutes(-timeDeltaInMinutes) : firstTime.AddMinutes(timeDeltaInMinutes);

            var priceDistanceWithMediumLine = Math.Abs(medianLine.CalculateY(firstTime) - medianLine.CalculateY(secondTime));

            var secondPrice = medianLine.Y2 > medianLine.Y1 ? firstPrice + priceDistanceWithMediumLine : firstPrice - priceDistanceWithMediumLine;

            var name = GetObjectName(string.Format("Level_{0}", percent.ToString(CultureInfo.InvariantCulture)), id: id);

            var line = Chart.DrawTrendLine(name, firstTime, firstPrice, secondTime, secondPrice, lineColor);

            line.ExtendToInfinity = true;
            line.IsInteractive = true;
            line.IsLocked = true;
        }

        private void UpdateMedianLine(ChartTrendLine medianLine, ChartTrendLine controllerLine)
        {
            medianLine.Time2 = controllerLine.GetStartTime().AddTicks(controllerLine.GetTimeDelta().Ticks / 2);
            medianLine.Y2 = controllerLine.GetBottomPrice() + controllerLine.GetPriceDelta() / 2;
        }
    }
}