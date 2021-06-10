using cAlgo.API;
using cAlgo.Controls;
using cAlgo.Helpers;
using cAlgo.Patterns;
using System.Collections.Generic;
using System.Linq;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class PitchforkDrawing : Indicator
    {
        private StackPanel _mainButtonsPanel;

        private StackPanel _groupButtonsPanel;

        private StackPanel _mainPanel;

        private Color _buttonsBackgroundDisableColor;

        private Color _buttonsBackgroundEnableColor;

        private Style _buttonsStyle;

        private readonly List<Button> _buttons = new List<Button>();

        private Button _expandButton;

        #region Patterns color parameters

        [Parameter("Color", DefaultValue = "Red", Group = "Patterns Color")]
        public string PatternsColor { get; set; }

        [Parameter("Alpha", DefaultValue = 100, MinValue = 0, MaxValue = 255, Group = "Patterns Color")]
        public int PatternsColorAlpha { get; set; }

        #endregion Patterns color parameters

        #region Patterns Label parameters

        [Parameter("Show", DefaultValue = true, Group = "Patterns Label")]
        public bool PatternsLabelShow { get; set; }

        [Parameter("Color", DefaultValue = "Yellow", Group = "Patterns Label")]
        public string PatternsLabelColor { get; set; }

        [Parameter("Alpha", DefaultValue = 100, MinValue = 0, MaxValue = 255, Group = "Patterns Label")]
        public int PatternsLabelColorAlpha { get; set; }

        [Parameter("Locked", DefaultValue = true, Group = "Patterns Label")]
        public bool PatternsLabelLocked { get; set; }

        [Parameter("Link Style", DefaultValue = true, Group = "Patterns Label")]
        public bool PatternsLabelLinkStyle { get; set; }

        #endregion Patterns Label parameters

        #region Container Panel parameters

        [Parameter("Orientation", DefaultValue = Orientation.Vertical, Group = "Container Panel")]
        public Orientation PanelOrientation { get; set; }

        [Parameter("Horizontal Alignment", DefaultValue = HorizontalAlignment.Left, Group = "Container Panel")]
        public HorizontalAlignment PanelHorizontalAlignment { get; set; }

        [Parameter("Vertical Alignment", DefaultValue = VerticalAlignment.Top, Group = "Container Panel")]
        public VerticalAlignment PanelVerticalAlignment { get; set; }

        [Parameter("Margin", DefaultValue = 3, Group = "Container Panel")]
        public double PanelMargin { get; set; }

        #endregion Container Panel parameters

        #region Buttons parameters

        [Parameter("Disable Color", DefaultValue = "#FFCCCCCC", Group = "Buttons")]
        public string ButtonsBackgroundDisableColor { get; set; }

        [Parameter("Enable Color", DefaultValue = "Red", Group = "Buttons")]
        public string ButtonsBackgroundEnableColor { get; set; }

        [Parameter("Text Color", DefaultValue = "Blue", Group = "Buttons")]
        public string ButtonsForegroundColor { get; set; }

        [Parameter("Margin", DefaultValue = 1, Group = "Buttons")]
        public double ButtonsMargin { get; set; }

        [Parameter("Transparency", DefaultValue = 0.5, MinValue = 0, MaxValue = 1, Group = "Buttons")]
        public double ButtonsTransparency { get; set; }

        #endregion Buttons parameters

        #region TimeFrame Visibility parameters

        [Parameter("Enable", DefaultValue = false, Group = "TimeFrame Visibility")]
        public bool IsTimeFrameVisibilityEnabled { get; set; }

        [Parameter("TimeFrame", Group = "TimeFrame Visibility")]
        public TimeFrame VisibilityTimeFrame { get; set; }

        [Parameter("Only Buttons", Group = "TimeFrame Visibility")]
        public bool VisibilityOnlyButtons { get; set; }

        #endregion TimeFrame Visibility parameters

        #region Original Pitchfork parameters

        [Parameter("Median Thickness", DefaultValue = 1, MinValue = 1, Group = "Original Pitchfork")]
        public int OriginalPitchforkMedianThickness { get; set; }

        [Parameter("Median Style", DefaultValue = LineStyle.Solid, Group = "Original Pitchfork")]
        public LineStyle OriginalPitchforkMedianStyle { get; set; }

        [Parameter("Median Color", DefaultValue = "Blue", Group = "Original Pitchfork")]
        public string OriginalPitchforkMedianColor { get; set; }

        [Parameter("Show 1st Level", DefaultValue = true, Group = "Original Pitchfork")]
        public bool ShowFirstOriginalPitchfork { get; set; }

        [Parameter("1st Level Percent", DefaultValue = 0.25, Group = "Original Pitchfork")]
        public double FirstOriginalPitchforkPercent { get; set; }

        [Parameter("1st Level Color", DefaultValue = "Gray", Group = "Original Pitchfork")]
        public string FirstOriginalPitchforkColor { get; set; }

        [Parameter("1st Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Original Pitchfork")]
        public int FirstOriginalPitchforkAlpha { get; set; }

        [Parameter("1st Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Original Pitchfork")]
        public int FirstOriginalPitchforkThickness { get; set; }

        [Parameter("1st Level Style", DefaultValue = LineStyle.Solid, Group = "Original Pitchfork")]
        public LineStyle FirstOriginalPitchforkStyle { get; set; }

        [Parameter("Show 2nd Level", DefaultValue = true, Group = "Original Pitchfork")]
        public bool ShowSecondOriginalPitchfork { get; set; }

        [Parameter("2nd Level Percent", DefaultValue = 0.382, Group = "Original Pitchfork")]
        public double SecondOriginalPitchforkPercent { get; set; }

        [Parameter("2nd Level Color", DefaultValue = "Red", Group = "Original Pitchfork")]
        public string SecondOriginalPitchforkColor { get; set; }

        [Parameter("2nd Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Original Pitchfork")]
        public int SecondOriginalPitchforkAlpha { get; set; }

        [Parameter("2nd Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Original Pitchfork")]
        public int SecondOriginalPitchforkThickness { get; set; }

        [Parameter("2nd Level Style", DefaultValue = LineStyle.Solid, Group = "Original Pitchfork")]
        public LineStyle SecondOriginalPitchforkStyle { get; set; }

        [Parameter("Show 3rd Level", DefaultValue = true, Group = "Original Pitchfork")]
        public bool ShowThirdOriginalPitchfork { get; set; }

        [Parameter("3rd Level Percent", DefaultValue = 0.5, Group = "Original Pitchfork")]
        public double ThirdOriginalPitchforkPercent { get; set; }

        [Parameter("3rd Level Color", DefaultValue = "GreenYellow", Group = "Original Pitchfork")]
        public string ThirdOriginalPitchforkColor { get; set; }

        [Parameter("3rd Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Original Pitchfork")]
        public int ThirdOriginalPitchforkAlpha { get; set; }

        [Parameter("3rd Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Original Pitchfork")]
        public int ThirdOriginalPitchforkThickness { get; set; }

        [Parameter("3rd Level Style", DefaultValue = LineStyle.Solid, Group = "Original Pitchfork")]
        public LineStyle ThirdOriginalPitchforkStyle { get; set; }

        [Parameter("Show 4th Level", DefaultValue = true, Group = "Original Pitchfork")]
        public bool ShowFourthOriginalPitchfork { get; set; }

        [Parameter("4th Level Percent", DefaultValue = 0.618, Group = "Original Pitchfork")]
        public double FourthOriginalPitchforkPercent { get; set; }

        [Parameter("4th Level Color", DefaultValue = "DarkGreen", Group = "Original Pitchfork")]
        public string FourthOriginalPitchforkColor { get; set; }

        [Parameter("4th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Original Pitchfork")]
        public int FourthOriginalPitchforkAlpha { get; set; }

        [Parameter("4th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Original Pitchfork")]
        public int FourthOriginalPitchforkThickness { get; set; }

        [Parameter("4th Level Style", DefaultValue = LineStyle.Solid, Group = "Original Pitchfork")]
        public LineStyle FourthOriginalPitchforkStyle { get; set; }

        [Parameter("Show 5th Level", DefaultValue = true, Group = "Original Pitchfork")]
        public bool ShowFifthOriginalPitchfork { get; set; }

        [Parameter("5th Level Percent", DefaultValue = 0.75, Group = "Original Pitchfork")]
        public double FifthOriginalPitchforkPercent { get; set; }

        [Parameter("5th Level Color", DefaultValue = "BlueViolet", Group = "Original Pitchfork")]
        public string FifthOriginalPitchforkColor { get; set; }

        [Parameter("5th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Original Pitchfork")]
        public int FifthOriginalPitchforkAlpha { get; set; }

        [Parameter("5th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Original Pitchfork")]
        public int FifthOriginalPitchforkThickness { get; set; }

        [Parameter("5th Level Style", DefaultValue = LineStyle.Solid, Group = "Original Pitchfork")]
        public LineStyle FifthOriginalPitchforkStyle { get; set; }

        [Parameter("Show 6th Level", DefaultValue = true, Group = "Original Pitchfork")]
        public bool ShowSixthOriginalPitchfork { get; set; }

        [Parameter("6th Level Percent", DefaultValue = 1, Group = "Original Pitchfork")]
        public double SixthOriginalPitchforkPercent { get; set; }

        [Parameter("6th Level Color", DefaultValue = "AliceBlue", Group = "Original Pitchfork")]
        public string SixthOriginalPitchforkColor { get; set; }

        [Parameter("6th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Original Pitchfork")]
        public int SixthOriginalPitchforkAlpha { get; set; }

        [Parameter("6th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Original Pitchfork")]
        public int SixthOriginalPitchforkThickness { get; set; }

        [Parameter("6th Level Style", DefaultValue = LineStyle.Solid, Group = "Original Pitchfork")]
        public LineStyle SixthOriginalPitchforkStyle { get; set; }

        [Parameter("Show 7th Level", DefaultValue = true, Group = "Original Pitchfork")]
        public bool ShowSeventhOriginalPitchfork { get; set; }

        [Parameter("7th Level Percent", DefaultValue = 1.5, Group = "Original Pitchfork")]
        public double SeventhOriginalPitchforkPercent { get; set; }

        [Parameter("7th Level Color", DefaultValue = "Bisque", Group = "Original Pitchfork")]
        public string SeventhOriginalPitchforkColor { get; set; }

        [Parameter("7th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Original Pitchfork")]
        public int SeventhOriginalPitchforkAlpha { get; set; }

        [Parameter("7th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Original Pitchfork")]
        public int SeventhOriginalPitchforkThickness { get; set; }

        [Parameter("7th Level Style", DefaultValue = LineStyle.Solid, Group = "Original Pitchfork")]
        public LineStyle SeventhOriginalPitchforkStyle { get; set; }

        [Parameter("Show 8th Level", DefaultValue = true, Group = "Original Pitchfork")]
        public bool ShowEighthOriginalPitchfork { get; set; }

        [Parameter("8th Level Percent", DefaultValue = 1.75, Group = "Original Pitchfork")]
        public double EighthOriginalPitchforkPercent { get; set; }

        [Parameter("8th Level Color", DefaultValue = "Azure", Group = "Original Pitchfork")]
        public string EighthOriginalPitchforkColor { get; set; }

        [Parameter("8th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Original Pitchfork")]
        public int EighthOriginalPitchforkAlpha { get; set; }

        [Parameter("8th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Original Pitchfork")]
        public int EighthOriginalPitchforkThickness { get; set; }

        [Parameter("8th Level Style", DefaultValue = LineStyle.Solid, Group = "Original Pitchfork")]
        public LineStyle EighthOriginalPitchforkStyle { get; set; }

        [Parameter("Show 9th Level", DefaultValue = true, Group = "Original Pitchfork")]
        public bool ShowNinthOriginalPitchfork { get; set; }

        [Parameter("9th Level Percent", DefaultValue = 2, Group = "Original Pitchfork")]
        public double NinthOriginalPitchforkPercent { get; set; }

        [Parameter("9th Level Color", DefaultValue = "Aqua", Group = "Original Pitchfork")]
        public string NinthOriginalPitchforkColor { get; set; }

        [Parameter("9th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Original Pitchfork")]
        public int NinthOriginalPitchforkAlpha { get; set; }

        [Parameter("9th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Original Pitchfork")]
        public int NinthOriginalPitchforkThickness { get; set; }

        [Parameter("9th Level Style", DefaultValue = LineStyle.Solid, Group = "Original Pitchfork")]
        public LineStyle NinthOriginalPitchforkStyle { get; set; }

        #endregion Original Pitchfork parameters

        #region Schiff Pitchfork parameters

        [Parameter("Median Thickness", DefaultValue = 1, MinValue = 1, Group = "Schiff Pitchfork")]
        public int SchiffPitchforkMedianThickness { get; set; }

        [Parameter("Median Style", DefaultValue = LineStyle.Solid, Group = "Schiff Pitchfork")]
        public LineStyle SchiffPitchforkMedianStyle { get; set; }

        [Parameter("Median Color", DefaultValue = "Blue", Group = "Schiff Pitchfork")]
        public string SchiffPitchforkMedianColor { get; set; }

        [Parameter("Show 1st Level", DefaultValue = true, Group = "Schiff Pitchfork")]
        public bool ShowFirstSchiffPitchfork { get; set; }

        [Parameter("1st Level Percent", DefaultValue = 0.25, Group = "Schiff Pitchfork")]
        public double FirstSchiffPitchforkPercent { get; set; }

        [Parameter("1st Level Color", DefaultValue = "Gray", Group = "Schiff Pitchfork")]
        public string FirstSchiffPitchforkColor { get; set; }

        [Parameter("1st Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Schiff Pitchfork")]
        public int FirstSchiffPitchforkAlpha { get; set; }

        [Parameter("1st Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Schiff Pitchfork")]
        public int FirstSchiffPitchforkThickness { get; set; }

        [Parameter("1st Level Style", DefaultValue = LineStyle.Solid, Group = "Schiff Pitchfork")]
        public LineStyle FirstSchiffPitchforkStyle { get; set; }

        [Parameter("Show 2nd Level", DefaultValue = true, Group = "Schiff Pitchfork")]
        public bool ShowSecondSchiffPitchfork { get; set; }

        [Parameter("2nd Level Percent", DefaultValue = 0.382, Group = "Schiff Pitchfork")]
        public double SecondSchiffPitchforkPercent { get; set; }

        [Parameter("2nd Level Color", DefaultValue = "Red", Group = "Schiff Pitchfork")]
        public string SecondSchiffPitchforkColor { get; set; }

        [Parameter("2nd Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Schiff Pitchfork")]
        public int SecondSchiffPitchforkAlpha { get; set; }

        [Parameter("2nd Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Schiff Pitchfork")]
        public int SecondSchiffPitchforkThickness { get; set; }

        [Parameter("2nd Level Style", DefaultValue = LineStyle.Solid, Group = "Schiff Pitchfork")]
        public LineStyle SecondSchiffPitchforkStyle { get; set; }

        [Parameter("Show 3rd Level", DefaultValue = true, Group = "Schiff Pitchfork")]
        public bool ShowThirdSchiffPitchfork { get; set; }

        [Parameter("3rd Level Percent", DefaultValue = 0.5, Group = "Schiff Pitchfork")]
        public double ThirdSchiffPitchforkPercent { get; set; }

        [Parameter("3rd Level Color", DefaultValue = "GreenYellow", Group = "Schiff Pitchfork")]
        public string ThirdSchiffPitchforkColor { get; set; }

        [Parameter("3rd Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Schiff Pitchfork")]
        public int ThirdSchiffPitchforkAlpha { get; set; }

        [Parameter("3rd Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Schiff Pitchfork")]
        public int ThirdSchiffPitchforkThickness { get; set; }

        [Parameter("3rd Level Style", DefaultValue = LineStyle.Solid, Group = "Schiff Pitchfork")]
        public LineStyle ThirdSchiffPitchforkStyle { get; set; }

        [Parameter("Show 4th Level", DefaultValue = true, Group = "Schiff Pitchfork")]
        public bool ShowFourthSchiffPitchfork { get; set; }

        [Parameter("4th Level Percent", DefaultValue = 0.618, Group = "Schiff Pitchfork")]
        public double FourthSchiffPitchforkPercent { get; set; }

        [Parameter("4th Level Color", DefaultValue = "DarkGreen", Group = "Schiff Pitchfork")]
        public string FourthSchiffPitchforkColor { get; set; }

        [Parameter("4th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Schiff Pitchfork")]
        public int FourthSchiffPitchforkAlpha { get; set; }

        [Parameter("4th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Schiff Pitchfork")]
        public int FourthSchiffPitchforkThickness { get; set; }

        [Parameter("4th Level Style", DefaultValue = LineStyle.Solid, Group = "Schiff Pitchfork")]
        public LineStyle FourthSchiffPitchforkStyle { get; set; }

        [Parameter("Show 5th Level", DefaultValue = true, Group = "Schiff Pitchfork")]
        public bool ShowFifthSchiffPitchfork { get; set; }

        [Parameter("5th Level Percent", DefaultValue = 0.75, Group = "Schiff Pitchfork")]
        public double FifthSchiffPitchforkPercent { get; set; }

        [Parameter("5th Level Color", DefaultValue = "BlueViolet", Group = "Schiff Pitchfork")]
        public string FifthSchiffPitchforkColor { get; set; }

        [Parameter("5th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Schiff Pitchfork")]
        public int FifthSchiffPitchforkAlpha { get; set; }

        [Parameter("5th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Schiff Pitchfork")]
        public int FifthSchiffPitchforkThickness { get; set; }

        [Parameter("5th Level Style", DefaultValue = LineStyle.Solid, Group = "Schiff Pitchfork")]
        public LineStyle FifthSchiffPitchforkStyle { get; set; }

        [Parameter("Show 6th Level", DefaultValue = true, Group = "Schiff Pitchfork")]
        public bool ShowSixthSchiffPitchfork { get; set; }

        [Parameter("6th Level Percent", DefaultValue = 1, Group = "Schiff Pitchfork")]
        public double SixthSchiffPitchforkPercent { get; set; }

        [Parameter("6th Level Color", DefaultValue = "AliceBlue", Group = "Schiff Pitchfork")]
        public string SixthSchiffPitchforkColor { get; set; }

        [Parameter("6th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Schiff Pitchfork")]
        public int SixthSchiffPitchforkAlpha { get; set; }

        [Parameter("6th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Schiff Pitchfork")]
        public int SixthSchiffPitchforkThickness { get; set; }

        [Parameter("6th Level Style", DefaultValue = LineStyle.Solid, Group = "Schiff Pitchfork")]
        public LineStyle SixthSchiffPitchforkStyle { get; set; }

        [Parameter("Show 7th Level", DefaultValue = true, Group = "Schiff Pitchfork")]
        public bool ShowSeventhSchiffPitchfork { get; set; }

        [Parameter("7th Level Percent", DefaultValue = 1.5, Group = "Schiff Pitchfork")]
        public double SeventhSchiffPitchforkPercent { get; set; }

        [Parameter("7th Level Color", DefaultValue = "Bisque", Group = "Schiff Pitchfork")]
        public string SeventhSchiffPitchforkColor { get; set; }

        [Parameter("7th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Schiff Pitchfork")]
        public int SeventhSchiffPitchforkAlpha { get; set; }

        [Parameter("7th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Schiff Pitchfork")]
        public int SeventhSchiffPitchforkThickness { get; set; }

        [Parameter("7th Level Style", DefaultValue = LineStyle.Solid, Group = "Schiff Pitchfork")]
        public LineStyle SeventhSchiffPitchforkStyle { get; set; }

        [Parameter("Show 8th Level", DefaultValue = true, Group = "Schiff Pitchfork")]
        public bool ShowEighthSchiffPitchfork { get; set; }

        [Parameter("8th Level Percent", DefaultValue = 1.75, Group = "Schiff Pitchfork")]
        public double EighthSchiffPitchforkPercent { get; set; }

        [Parameter("8th Level Color", DefaultValue = "Azure", Group = "Schiff Pitchfork")]
        public string EighthSchiffPitchforkColor { get; set; }

        [Parameter("8th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Schiff Pitchfork")]
        public int EighthSchiffPitchforkAlpha { get; set; }

        [Parameter("8th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Schiff Pitchfork")]
        public int EighthSchiffPitchforkThickness { get; set; }

        [Parameter("8th Level Style", DefaultValue = LineStyle.Solid, Group = "Schiff Pitchfork")]
        public LineStyle EighthSchiffPitchforkStyle { get; set; }

        [Parameter("Show 9th Level", DefaultValue = true, Group = "Schiff Pitchfork")]
        public bool ShowNinthSchiffPitchfork { get; set; }

        [Parameter("9th Level Percent", DefaultValue = 2, Group = "Schiff Pitchfork")]
        public double NinthSchiffPitchforkPercent { get; set; }

        [Parameter("9th Level Color", DefaultValue = "Aqua", Group = "Schiff Pitchfork")]
        public string NinthSchiffPitchforkColor { get; set; }

        [Parameter("9th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Schiff Pitchfork")]
        public int NinthSchiffPitchforkAlpha { get; set; }

        [Parameter("9th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Schiff Pitchfork")]
        public int NinthSchiffPitchforkThickness { get; set; }

        [Parameter("9th Level Style", DefaultValue = LineStyle.Solid, Group = "Schiff Pitchfork")]
        public LineStyle NinthSchiffPitchforkStyle { get; set; }

        #endregion Schiff Pitchfork parameters

        #region Modified Schiff Pitchfork parameters

        [Parameter("Median Thickness", DefaultValue = 1, MinValue = 1, Group = "Modified Schiff Pitchfork")]
        public int ModifiedSchiffPitchforkMedianThickness { get; set; }

        [Parameter("Median Style", DefaultValue = LineStyle.Solid, Group = "Modified Schiff Pitchfork")]
        public LineStyle ModifiedSchiffPitchforkMedianStyle { get; set; }

        [Parameter("Median Color", DefaultValue = "Blue", Group = "Modified Schiff Pitchfork")]
        public string ModifiedSchiffPitchforkMedianColor { get; set; }

        [Parameter("Show 1st Level", DefaultValue = true, Group = "Modified Schiff Pitchfork")]
        public bool ShowFirstModifiedSchiffPitchfork { get; set; }

        [Parameter("1st Level Percent", DefaultValue = 0.25, Group = "Modified Schiff Pitchfork")]
        public double FirstModifiedSchiffPitchforkPercent { get; set; }

        [Parameter("1st Level Color", DefaultValue = "Gray", Group = "Modified Schiff Pitchfork")]
        public string FirstModifiedSchiffPitchforkColor { get; set; }

        [Parameter("1st Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Modified Schiff Pitchfork")]
        public int FirstModifiedSchiffPitchforkAlpha { get; set; }

        [Parameter("1st Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Modified Schiff Pitchfork")]
        public int FirstModifiedSchiffPitchforkThickness { get; set; }

        [Parameter("1st Level Style", DefaultValue = LineStyle.Solid, Group = "Modified Schiff Pitchfork")]
        public LineStyle FirstModifiedSchiffPitchforkStyle { get; set; }

        [Parameter("Show 2nd Level", DefaultValue = true, Group = "Modified Schiff Pitchfork")]
        public bool ShowSecondModifiedSchiffPitchfork { get; set; }

        [Parameter("2nd Level Percent", DefaultValue = 0.382, Group = "Modified Schiff Pitchfork")]
        public double SecondModifiedSchiffPitchforkPercent { get; set; }

        [Parameter("2nd Level Color", DefaultValue = "Red", Group = "Modified Schiff Pitchfork")]
        public string SecondModifiedSchiffPitchforkColor { get; set; }

        [Parameter("2nd Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Modified Schiff Pitchfork")]
        public int SecondModifiedSchiffPitchforkAlpha { get; set; }

        [Parameter("2nd Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Modified Schiff Pitchfork")]
        public int SecondModifiedSchiffPitchforkThickness { get; set; }

        [Parameter("2nd Level Style", DefaultValue = LineStyle.Solid, Group = "Modified Schiff Pitchfork")]
        public LineStyle SecondModifiedSchiffPitchforkStyle { get; set; }

        [Parameter("Show 3rd Level", DefaultValue = true, Group = "Modified Schiff Pitchfork")]
        public bool ShowThirdModifiedSchiffPitchfork { get; set; }

        [Parameter("3rd Level Percent", DefaultValue = 0.5, Group = "Modified Schiff Pitchfork")]
        public double ThirdModifiedSchiffPitchforkPercent { get; set; }

        [Parameter("3rd Level Color", DefaultValue = "GreenYellow", Group = "Modified Schiff Pitchfork")]
        public string ThirdModifiedSchiffPitchforkColor { get; set; }

        [Parameter("3rd Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Modified Schiff Pitchfork")]
        public int ThirdModifiedSchiffPitchforkAlpha { get; set; }

        [Parameter("3rd Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Modified Schiff Pitchfork")]
        public int ThirdModifiedSchiffPitchforkThickness { get; set; }

        [Parameter("3rd Level Style", DefaultValue = LineStyle.Solid, Group = "Modified Schiff Pitchfork")]
        public LineStyle ThirdModifiedSchiffPitchforkStyle { get; set; }

        [Parameter("Show 4th Level", DefaultValue = true, Group = "Modified Schiff Pitchfork")]
        public bool ShowFourthModifiedSchiffPitchfork { get; set; }

        [Parameter("4th Level Percent", DefaultValue = 0.618, Group = "Modified Schiff Pitchfork")]
        public double FourthModifiedSchiffPitchforkPercent { get; set; }

        [Parameter("4th Level Color", DefaultValue = "DarkGreen", Group = "Modified Schiff Pitchfork")]
        public string FourthModifiedSchiffPitchforkColor { get; set; }

        [Parameter("4th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Modified Schiff Pitchfork")]
        public int FourthModifiedSchiffPitchforkAlpha { get; set; }

        [Parameter("4th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Modified Schiff Pitchfork")]
        public int FourthModifiedSchiffPitchforkThickness { get; set; }

        [Parameter("4th Level Style", DefaultValue = LineStyle.Solid, Group = "Modified Schiff Pitchfork")]
        public LineStyle FourthModifiedSchiffPitchforkStyle { get; set; }

        [Parameter("Show 5th Level", DefaultValue = true, Group = "Modified Schiff Pitchfork")]
        public bool ShowFifthModifiedSchiffPitchfork { get; set; }

        [Parameter("5th Level Percent", DefaultValue = 0.75, Group = "Modified Schiff Pitchfork")]
        public double FifthModifiedSchiffPitchforkPercent { get; set; }

        [Parameter("5th Level Color", DefaultValue = "BlueViolet", Group = "Modified Schiff Pitchfork")]
        public string FifthModifiedSchiffPitchforkColor { get; set; }

        [Parameter("5th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Modified Schiff Pitchfork")]
        public int FifthModifiedSchiffPitchforkAlpha { get; set; }

        [Parameter("5th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Modified Schiff Pitchfork")]
        public int FifthModifiedSchiffPitchforkThickness { get; set; }

        [Parameter("5th Level Style", DefaultValue = LineStyle.Solid, Group = "Modified Schiff Pitchfork")]
        public LineStyle FifthModifiedSchiffPitchforkStyle { get; set; }

        [Parameter("Show 6th Level", DefaultValue = true, Group = "Modified Schiff Pitchfork")]
        public bool ShowSixthModifiedSchiffPitchfork { get; set; }

        [Parameter("6th Level Percent", DefaultValue = 1, Group = "Modified Schiff Pitchfork")]
        public double SixthModifiedSchiffPitchforkPercent { get; set; }

        [Parameter("6th Level Color", DefaultValue = "AliceBlue", Group = "Modified Schiff Pitchfork")]
        public string SixthModifiedSchiffPitchforkColor { get; set; }

        [Parameter("6th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Modified Schiff Pitchfork")]
        public int SixthModifiedSchiffPitchforkAlpha { get; set; }

        [Parameter("6th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Modified Schiff Pitchfork")]
        public int SixthModifiedSchiffPitchforkThickness { get; set; }

        [Parameter("6th Level Style", DefaultValue = LineStyle.Solid, Group = "Modified Schiff Pitchfork")]
        public LineStyle SixthModifiedSchiffPitchforkStyle { get; set; }

        [Parameter("Show 7th Level", DefaultValue = true, Group = "Modified Schiff Pitchfork")]
        public bool ShowSeventhModifiedSchiffPitchfork { get; set; }

        [Parameter("7th Level Percent", DefaultValue = 1.5, Group = "Modified Schiff Pitchfork")]
        public double SeventhModifiedSchiffPitchforkPercent { get; set; }

        [Parameter("7th Level Color", DefaultValue = "Bisque", Group = "Modified Schiff Pitchfork")]
        public string SeventhModifiedSchiffPitchforkColor { get; set; }

        [Parameter("7th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Modified Schiff Pitchfork")]
        public int SeventhModifiedSchiffPitchforkAlpha { get; set; }

        [Parameter("7th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Modified Schiff Pitchfork")]
        public int SeventhModifiedSchiffPitchforkThickness { get; set; }

        [Parameter("7th Level Style", DefaultValue = LineStyle.Solid, Group = "Modified Schiff Pitchfork")]
        public LineStyle SeventhModifiedSchiffPitchforkStyle { get; set; }

        [Parameter("Show 8th Level", DefaultValue = true, Group = "Modified Schiff Pitchfork")]
        public bool ShowEighthModifiedSchiffPitchfork { get; set; }

        [Parameter("8th Level Percent", DefaultValue = 1.75, Group = "Modified Schiff Pitchfork")]
        public double EighthModifiedSchiffPitchforkPercent { get; set; }

        [Parameter("8th Level Color", DefaultValue = "Azure", Group = "Modified Schiff Pitchfork")]
        public string EighthModifiedSchiffPitchforkColor { get; set; }

        [Parameter("8th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Modified Schiff Pitchfork")]
        public int EighthModifiedSchiffPitchforkAlpha { get; set; }

        [Parameter("8th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Modified Schiff Pitchfork")]
        public int EighthModifiedSchiffPitchforkThickness { get; set; }

        [Parameter("8th Level Style", DefaultValue = LineStyle.Solid, Group = "Modified Schiff Pitchfork")]
        public LineStyle EighthModifiedSchiffPitchforkStyle { get; set; }

        [Parameter("Show 9th Level", DefaultValue = true, Group = "Modified Schiff Pitchfork")]
        public bool ShowNinthModifiedSchiffPitchfork { get; set; }

        [Parameter("9th Level Percent", DefaultValue = 2, Group = "Modified Schiff Pitchfork")]
        public double NinthModifiedSchiffPitchforkPercent { get; set; }

        [Parameter("9th Level Color", DefaultValue = "Aqua", Group = "Modified Schiff Pitchfork")]
        public string NinthModifiedSchiffPitchforkColor { get; set; }

        [Parameter("9th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Modified Schiff Pitchfork")]
        public int NinthModifiedSchiffPitchforkAlpha { get; set; }

        [Parameter("9th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Modified Schiff Pitchfork")]
        public int NinthModifiedSchiffPitchforkThickness { get; set; }

        [Parameter("9th Level Style", DefaultValue = LineStyle.Solid, Group = "Modified Schiff Pitchfork")]
        public LineStyle NinthModifiedSchiffPitchforkStyle { get; set; }

        #endregion Modified Schiff Pitchfork parameters

        #region Pitchfan parameters

        [Parameter("Median Thickness", DefaultValue = 1, MinValue = 1, Group = "Pitchfan")]
        public int PitchfanMedianThickness { get; set; }

        [Parameter("Median Style", DefaultValue = LineStyle.Solid, Group = "Pitchfan")]
        public LineStyle PitchfanMedianStyle { get; set; }

        [Parameter("Median Color", DefaultValue = "Blue", Group = "Pitchfan")]
        public string PitchfanMedianColor { get; set; }

        [Parameter("Show 1st Level", DefaultValue = true, Group = "Pitchfan")]
        public bool ShowFirstPitchfan { get; set; }

        [Parameter("1st Level Percent", DefaultValue = 0.25, Group = "Pitchfan")]
        public double FirstPitchfanPercent { get; set; }

        [Parameter("1st Level Color", DefaultValue = "Gray", Group = "Pitchfan")]
        public string FirstPitchfanColor { get; set; }

        [Parameter("1st Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Pitchfan")]
        public int FirstPitchfanAlpha { get; set; }

        [Parameter("1st Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Pitchfan")]
        public int FirstPitchfanThickness { get; set; }

        [Parameter("1st Level Style", DefaultValue = LineStyle.Solid, Group = "Pitchfan")]
        public LineStyle FirstPitchfanStyle { get; set; }

        [Parameter("Show 2nd Level", DefaultValue = true, Group = "Pitchfan")]
        public bool ShowSecondPitchfan { get; set; }

        [Parameter("2nd Level Percent", DefaultValue = 0.382, Group = "Pitchfan")]
        public double SecondPitchfanPercent { get; set; }

        [Parameter("2nd Level Color", DefaultValue = "Red", Group = "Pitchfan")]
        public string SecondPitchfanColor { get; set; }

        [Parameter("2nd Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Pitchfan")]
        public int SecondPitchfanAlpha { get; set; }

        [Parameter("2nd Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Pitchfan")]
        public int SecondPitchfanThickness { get; set; }

        [Parameter("2nd Level Style", DefaultValue = LineStyle.Solid, Group = "Pitchfan")]
        public LineStyle SecondPitchfanStyle { get; set; }

        [Parameter("Show 3rd Level", DefaultValue = true, Group = "Pitchfan")]
        public bool ShowThirdPitchfan { get; set; }

        [Parameter("3rd Level Percent", DefaultValue = 0.5, Group = "Pitchfan")]
        public double ThirdPitchfanPercent { get; set; }

        [Parameter("3rd Level Color", DefaultValue = "GreenYellow", Group = "Pitchfan")]
        public string ThirdPitchfanColor { get; set; }

        [Parameter("3rd Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Pitchfan")]
        public int ThirdPitchfanAlpha { get; set; }

        [Parameter("3rd Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Pitchfan")]
        public int ThirdPitchfanThickness { get; set; }

        [Parameter("3rd Level Style", DefaultValue = LineStyle.Solid, Group = "Pitchfan")]
        public LineStyle ThirdPitchfanStyle { get; set; }

        [Parameter("Show 4th Level", DefaultValue = true, Group = "Pitchfan")]
        public bool ShowFourthPitchfan { get; set; }

        [Parameter("4th Level Percent", DefaultValue = 0.618, Group = "Pitchfan")]
        public double FourthPitchfanPercent { get; set; }

        [Parameter("4th Level Color", DefaultValue = "DarkGreen", Group = "Pitchfan")]
        public string FourthPitchfanColor { get; set; }

        [Parameter("4th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Pitchfan")]
        public int FourthPitchfanAlpha { get; set; }

        [Parameter("4th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Pitchfan")]
        public int FourthPitchfanThickness { get; set; }

        [Parameter("4th Level Style", DefaultValue = LineStyle.Solid, Group = "Pitchfan")]
        public LineStyle FourthPitchfanStyle { get; set; }

        [Parameter("Show 5th Level", DefaultValue = true, Group = "Pitchfan")]
        public bool ShowFifthPitchfan { get; set; }

        [Parameter("5th Level Percent", DefaultValue = 0.75, Group = "Pitchfan")]
        public double FifthPitchfanPercent { get; set; }

        [Parameter("5th Level Color", DefaultValue = "BlueViolet", Group = "Pitchfan")]
        public string FifthPitchfanColor { get; set; }

        [Parameter("5th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Pitchfan")]
        public int FifthPitchfanAlpha { get; set; }

        [Parameter("5th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Pitchfan")]
        public int FifthPitchfanThickness { get; set; }

        [Parameter("5th Level Style", DefaultValue = LineStyle.Solid, Group = "Pitchfan")]
        public LineStyle FifthPitchfanStyle { get; set; }

        [Parameter("Show 6th Level", DefaultValue = true, Group = "Pitchfan")]
        public bool ShowSixthPitchfan { get; set; }

        [Parameter("6th Level Percent", DefaultValue = 1, Group = "Pitchfan")]
        public double SixthPitchfanPercent { get; set; }

        [Parameter("6th Level Color", DefaultValue = "AliceBlue", Group = "Pitchfan")]
        public string SixthPitchfanColor { get; set; }

        [Parameter("6th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Pitchfan")]
        public int SixthPitchfanAlpha { get; set; }

        [Parameter("6th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Pitchfan")]
        public int SixthPitchfanThickness { get; set; }

        [Parameter("6th Level Style", DefaultValue = LineStyle.Solid, Group = "Pitchfan")]
        public LineStyle SixthPitchfanStyle { get; set; }

        [Parameter("Show 7th Level", DefaultValue = true, Group = "Pitchfan")]
        public bool ShowSeventhPitchfan { get; set; }

        [Parameter("7th Level Percent", DefaultValue = 1.5, Group = "Pitchfan")]
        public double SeventhPitchfanPercent { get; set; }

        [Parameter("7th Level Color", DefaultValue = "Bisque", Group = "Pitchfan")]
        public string SeventhPitchfanColor { get; set; }

        [Parameter("7th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Pitchfan")]
        public int SeventhPitchfanAlpha { get; set; }

        [Parameter("7th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Pitchfan")]
        public int SeventhPitchfanThickness { get; set; }

        [Parameter("7th Level Style", DefaultValue = LineStyle.Solid, Group = "Pitchfan")]
        public LineStyle SeventhPitchfanStyle { get; set; }

        [Parameter("Show 8th Level", DefaultValue = true, Group = "Pitchfan")]
        public bool ShowEighthPitchfan { get; set; }

        [Parameter("8th Level Percent", DefaultValue = 1.75, Group = "Pitchfan")]
        public double EighthPitchfanPercent { get; set; }

        [Parameter("8th Level Color", DefaultValue = "Azure", Group = "Pitchfan")]
        public string EighthPitchfanColor { get; set; }

        [Parameter("8th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Pitchfan")]
        public int EighthPitchfanAlpha { get; set; }

        [Parameter("8th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Pitchfan")]
        public int EighthPitchfanThickness { get; set; }

        [Parameter("8th Level Style", DefaultValue = LineStyle.Solid, Group = "Pitchfan")]
        public LineStyle EighthPitchfanStyle { get; set; }

        [Parameter("Show 9th Level", DefaultValue = true, Group = "Pitchfan")]
        public bool ShowNinthPitchfan { get; set; }

        [Parameter("9th Level Percent", DefaultValue = 2, Group = "Pitchfan")]
        public double NinthPitchfanPercent { get; set; }

        [Parameter("9th Level Color", DefaultValue = "Aqua", Group = "Pitchfan")]
        public string NinthPitchfanColor { get; set; }

        [Parameter("9th Level Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Pitchfan")]
        public int NinthPitchfanAlpha { get; set; }

        [Parameter("9th Level Thickness", DefaultValue = 1, MinValue = 0, Group = "Pitchfan")]
        public int NinthPitchfanThickness { get; set; }

        [Parameter("9th Level Style", DefaultValue = LineStyle.Solid, Group = "Pitchfan")]
        public LineStyle NinthPitchfanStyle { get; set; }

        #endregion Pitchfan parameters

        #region Overridden methods

        protected override void Initialize()
        {
            _mainPanel = new StackPanel
            {
                HorizontalAlignment = PanelHorizontalAlignment,
                VerticalAlignment = PanelVerticalAlignment,
                Orientation = PanelOrientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal,
                BackgroundColor = Color.Transparent,
            };

            _mainButtonsPanel = new StackPanel
            {
                Orientation = PanelOrientation,
                Margin = PanelMargin
            };

            _mainPanel.AddChild(_mainButtonsPanel);

            _groupButtonsPanel = new StackPanel
            {
                Orientation = PanelOrientation,
                Margin = PanelMargin,
                IsVisible = false
            };

            _mainPanel.AddChild(_groupButtonsPanel);

            _buttonsBackgroundDisableColor = ColorParser.Parse(ButtonsBackgroundDisableColor);
            _buttonsBackgroundEnableColor = ColorParser.Parse(ButtonsBackgroundEnableColor);

            _buttonsStyle = new Style();

            _buttonsStyle.Set(ControlProperty.Margin, ButtonsMargin);
            _buttonsStyle.Set(ControlProperty.BackgroundColor, _buttonsBackgroundDisableColor);
            _buttonsStyle.Set(ControlProperty.ForegroundColor, ColorParser.Parse(ButtonsForegroundColor));
            _buttonsStyle.Set(ControlProperty.HorizontalContentAlignment, HorizontalAlignment.Center);
            _buttonsStyle.Set(ControlProperty.VerticalContentAlignment, VerticalAlignment.Center);
            _buttonsStyle.Set(ControlProperty.Opacity, ButtonsTransparency);

            var patternsColor = ColorParser.Parse(PatternsColor, PatternsColorAlpha);
            var patternsLabelsColor = ColorParser.Parse(PatternsLabelColor, PatternsLabelColorAlpha);

            var patternConfig = new PatternConfig(Chart, patternsColor, PatternsLabelShow, patternsLabelsColor, PatternsLabelLocked, PatternsLabelLinkStyle, new Logger(this.GetType().Name, Print));

            _expandButton = new Button
            {
                Style = _buttonsStyle,
                Text = "Expand Patterns"
            };

            _expandButton.Click += ExpandButton_Click;

            _mainButtonsPanel.AddChild(_expandButton);

            AddPatternButton(new OriginalPitchforkPattern(patternConfig, new LineSettings
            {
                LineColor = ColorParser.Parse(OriginalPitchforkMedianColor),
                Style = OriginalPitchforkMedianStyle,
                Thickness = OriginalPitchforkMedianThickness
            }, GetOriginalPitchforkLevels()));
            AddPatternButton(new SchiffPitchforkPattern(patternConfig, new LineSettings
            {
                LineColor = ColorParser.Parse(SchiffPitchforkMedianColor),
                Style = SchiffPitchforkMedianStyle,
                Thickness = SchiffPitchforkMedianThickness
            }, GetSchiffPitchforkLevels()));
            AddPatternButton(new ModifiedSchiffPitchforkPattern(patternConfig, new LineSettings
            {
                LineColor = ColorParser.Parse(ModifiedSchiffPitchforkMedianColor),
                Style = ModifiedSchiffPitchforkMedianStyle,
                Thickness = ModifiedSchiffPitchforkMedianThickness
            }, GetModifiedSchiffPitchforkLevels()));
            AddPatternButton(new PitchfanPattern(patternConfig, GetPitchfanSideFanSettings(), new FanSettings
            {
                Color = ColorParser.Parse(PitchfanMedianColor),
                Style = PitchfanMedianStyle,
                Thickness = PitchfanMedianThickness
            }));

            var showHideButton = new Controls.ToggleButton()
            {
                Style = _buttonsStyle,
                OnColor = _buttonsBackgroundEnableColor,
                OffColor = _buttonsBackgroundDisableColor,
                Text = "Hide",
                IsVisible = false
            };

            showHideButton.TurnedOn += ShowHideButton_TurnedOn;
            showHideButton.TurnedOff += ShowHideButton_TurnedOff;

            _mainButtonsPanel.AddChild(showHideButton);
            _buttons.Add(showHideButton);

            var saveButton = new PatternsSaveButton(Chart)
            {
                Style = _buttonsStyle,
                IsVisible = false
            };

            _mainButtonsPanel.AddChild(saveButton);
            _buttons.Add(saveButton);

            var loadButton = new PatternsLoadButton(Chart)
            {
                Style = _buttonsStyle,
                IsVisible = false
            };

            _mainButtonsPanel.AddChild(loadButton);
            _buttons.Add(loadButton);

            var removeAllButton = new PatternsRemoveAllButton(Chart)
            {
                Style = _buttonsStyle,
                IsVisible = false
            };

            _mainButtonsPanel.AddChild(removeAllButton);
            _buttons.Add(removeAllButton);

            var collapseButton = new Button
            {
                Style = _buttonsStyle,
                Text = "Collapse",
                IsVisible = false
            };

            collapseButton.Click += CollapseButton_Click;

            _mainButtonsPanel.AddChild(collapseButton);
            _buttons.Add(collapseButton);

            Chart.AddControl(_mainPanel);

            CheckTimeFrameVisibility();
        }

        public override void Calculate(int index)
        {
        }

        #endregion Overridden methods

        private void CollapseButton_Click(ButtonClickEventArgs obj)
        {
            _buttons.ForEach(iButton => iButton.IsVisible = false);

            _groupButtonsPanel.IsVisible = false;

            _expandButton.IsVisible = true;
        }

        private void ExpandButton_Click(ButtonClickEventArgs obj)
        {
            _buttons.ForEach(iButton => iButton.IsVisible = true);

            obj.Button.IsVisible = false;
        }

        private void ShowHideButton_TurnedOff(Controls.ToggleButton obj)
        {
            Chart.ChangePatternsVisibility(false);

            obj.Text = "Hide";
        }

        private void ShowHideButton_TurnedOn(Controls.ToggleButton obj)
        {
            Chart.ChangePatternsVisibility(true);

            obj.Text = "Show";
        }

        private void AddPatternButton(IPattern pattern)
        {
            var button = new PatternButton(pattern)
            {
                Style = _buttonsStyle,
                OnColor = _buttonsBackgroundEnableColor,
                OffColor = _buttonsBackgroundDisableColor,
                IsVisible = false
            };

            _buttons.Add(button);

            _mainButtonsPanel.AddChild(button);

            pattern.Initialize();
        }

        private void CheckTimeFrameVisibility()
        {
            if (IsTimeFrameVisibilityEnabled)
            {
                if (TimeFrame != VisibilityTimeFrame)
                {
                    _mainButtonsPanel.IsVisible = false;

                    if (!VisibilityOnlyButtons) Chart.ChangePatternsVisibility(true);
                }
                else if (!VisibilityOnlyButtons)
                {
                    Chart.ChangePatternsVisibility(false);
                }
            }
        }

        private Dictionary<double, PercentLineSettings> GetOriginalPitchforkLevels()
        {
            var originalPitchforkLevels = new Dictionary<double, PercentLineSettings>();

            if (ShowFirstOriginalPitchfork)
            {
                originalPitchforkLevels.Add(FirstOriginalPitchforkPercent, new PercentLineSettings
                {
                    Percent = FirstOriginalPitchforkPercent,
                    LineColor = ColorParser.Parse(FirstOriginalPitchforkColor),
                    Style = FirstOriginalPitchforkStyle,
                    Thickness = FirstOriginalPitchforkThickness,
                });
            }

            if (ShowSecondOriginalPitchfork)
            {
                originalPitchforkLevels.Add(SecondOriginalPitchforkPercent, new PercentLineSettings
                {
                    Percent = SecondOriginalPitchforkPercent,
                    LineColor = ColorParser.Parse(SecondOriginalPitchforkColor),
                    Style = SecondOriginalPitchforkStyle,
                    Thickness = SecondOriginalPitchforkThickness,
                });
            }

            if (ShowThirdOriginalPitchfork)
            {
                originalPitchforkLevels.Add(ThirdOriginalPitchforkPercent, new PercentLineSettings
                {
                    Percent = ThirdOriginalPitchforkPercent,
                    LineColor = ColorParser.Parse(ThirdOriginalPitchforkColor),
                    Style = ThirdOriginalPitchforkStyle,
                    Thickness = ThirdOriginalPitchforkThickness,
                });
            }

            if (ShowFourthOriginalPitchfork)
            {
                originalPitchforkLevels.Add(FourthOriginalPitchforkPercent, new PercentLineSettings
                {
                    Percent = FourthOriginalPitchforkPercent,
                    LineColor = ColorParser.Parse(FourthOriginalPitchforkColor),
                    Style = FourthOriginalPitchforkStyle,
                    Thickness = FourthOriginalPitchforkThickness,
                });
            }

            if (ShowFifthOriginalPitchfork)
            {
                originalPitchforkLevels.Add(FifthOriginalPitchforkPercent, new PercentLineSettings
                {
                    Percent = FifthOriginalPitchforkPercent,
                    LineColor = ColorParser.Parse(FifthOriginalPitchforkColor),
                    Style = FifthOriginalPitchforkStyle,
                    Thickness = FifthOriginalPitchforkThickness,
                });
            }

            if (ShowSixthOriginalPitchfork)
            {
                originalPitchforkLevels.Add(SixthOriginalPitchforkPercent, new PercentLineSettings
                {
                    Percent = SixthOriginalPitchforkPercent,
                    LineColor = ColorParser.Parse(SixthOriginalPitchforkColor),
                    Style = SixthOriginalPitchforkStyle,
                    Thickness = SixthOriginalPitchforkThickness,
                });
            }

            if (ShowSeventhOriginalPitchfork)
            {
                originalPitchforkLevels.Add(SeventhOriginalPitchforkPercent, new PercentLineSettings
                {
                    Percent = SeventhOriginalPitchforkPercent,
                    LineColor = ColorParser.Parse(SeventhOriginalPitchforkColor),
                    Style = SeventhOriginalPitchforkStyle,
                    Thickness = SeventhOriginalPitchforkThickness,
                });
            }

            if (ShowEighthOriginalPitchfork)
            {
                originalPitchforkLevels.Add(EighthOriginalPitchforkPercent, new PercentLineSettings
                {
                    Percent = EighthOriginalPitchforkPercent,
                    LineColor = ColorParser.Parse(EighthOriginalPitchforkColor),
                    Style = EighthOriginalPitchforkStyle,
                    Thickness = EighthOriginalPitchforkThickness,
                });
            }

            if (ShowNinthOriginalPitchfork)
            {
                originalPitchforkLevels.Add(NinthOriginalPitchforkPercent, new PercentLineSettings
                {
                    Percent = NinthOriginalPitchforkPercent,
                    LineColor = ColorParser.Parse(NinthOriginalPitchforkColor),
                    Style = NinthOriginalPitchforkStyle,
                    Thickness = NinthOriginalPitchforkThickness,
                });
            }

            return originalPitchforkLevels;
        }

        private Dictionary<double, PercentLineSettings> GetSchiffPitchforkLevels()
        {
            var schiffPitchforkLevels = new Dictionary<double, PercentLineSettings>();

            if (ShowFirstSchiffPitchfork)
            {
                schiffPitchforkLevels.Add(FirstSchiffPitchforkPercent, new PercentLineSettings
                {
                    Percent = FirstSchiffPitchforkPercent,
                    LineColor = ColorParser.Parse(FirstSchiffPitchforkColor),
                    Style = FirstSchiffPitchforkStyle,
                    Thickness = FirstSchiffPitchforkThickness,
                });
            }

            if (ShowSecondSchiffPitchfork)
            {
                schiffPitchforkLevels.Add(SecondSchiffPitchforkPercent, new PercentLineSettings
                {
                    Percent = SecondSchiffPitchforkPercent,
                    LineColor = ColorParser.Parse(SecondSchiffPitchforkColor),
                    Style = SecondSchiffPitchforkStyle,
                    Thickness = SecondSchiffPitchforkThickness,
                });
            }

            if (ShowThirdSchiffPitchfork)
            {
                schiffPitchforkLevels.Add(ThirdSchiffPitchforkPercent, new PercentLineSettings
                {
                    Percent = ThirdSchiffPitchforkPercent,
                    LineColor = ColorParser.Parse(ThirdSchiffPitchforkColor),
                    Style = ThirdSchiffPitchforkStyle,
                    Thickness = ThirdSchiffPitchforkThickness,
                });
            }

            if (ShowFourthSchiffPitchfork)
            {
                schiffPitchforkLevels.Add(FourthSchiffPitchforkPercent, new PercentLineSettings
                {
                    Percent = FourthSchiffPitchforkPercent,
                    LineColor = ColorParser.Parse(FourthSchiffPitchforkColor),
                    Style = FourthSchiffPitchforkStyle,
                    Thickness = FourthSchiffPitchforkThickness,
                });
            }

            if (ShowFifthSchiffPitchfork)
            {
                schiffPitchforkLevels.Add(FifthSchiffPitchforkPercent, new PercentLineSettings
                {
                    Percent = FifthSchiffPitchforkPercent,
                    LineColor = ColorParser.Parse(FifthSchiffPitchforkColor),
                    Style = FifthSchiffPitchforkStyle,
                    Thickness = FifthSchiffPitchforkThickness,
                });
            }

            if (ShowSixthSchiffPitchfork)
            {
                schiffPitchforkLevels.Add(SixthSchiffPitchforkPercent, new PercentLineSettings
                {
                    Percent = SixthSchiffPitchforkPercent,
                    LineColor = ColorParser.Parse(SixthSchiffPitchforkColor),
                    Style = SixthSchiffPitchforkStyle,
                    Thickness = SixthSchiffPitchforkThickness,
                });
            }

            if (ShowSeventhSchiffPitchfork)
            {
                schiffPitchforkLevels.Add(SeventhSchiffPitchforkPercent, new PercentLineSettings
                {
                    Percent = SeventhSchiffPitchforkPercent,
                    LineColor = ColorParser.Parse(SeventhSchiffPitchforkColor),
                    Style = SeventhSchiffPitchforkStyle,
                    Thickness = SeventhSchiffPitchforkThickness,
                });
            }

            if (ShowEighthSchiffPitchfork)
            {
                schiffPitchforkLevels.Add(EighthSchiffPitchforkPercent, new PercentLineSettings
                {
                    Percent = EighthSchiffPitchforkPercent,
                    LineColor = ColorParser.Parse(EighthSchiffPitchforkColor),
                    Style = EighthSchiffPitchforkStyle,
                    Thickness = EighthSchiffPitchforkThickness,
                });
            }

            if (ShowNinthSchiffPitchfork)
            {
                schiffPitchforkLevels.Add(NinthSchiffPitchforkPercent, new PercentLineSettings
                {
                    Percent = NinthSchiffPitchforkPercent,
                    LineColor = ColorParser.Parse(NinthSchiffPitchforkColor),
                    Style = NinthSchiffPitchforkStyle,
                    Thickness = NinthSchiffPitchforkThickness,
                });
            }

            return schiffPitchforkLevels;
        }

        private Dictionary<double, PercentLineSettings> GetModifiedSchiffPitchforkLevels()
        {
            var modifiedSchiffPitchforkLevels = new Dictionary<double, PercentLineSettings>();

            if (ShowFirstModifiedSchiffPitchfork)
            {
                modifiedSchiffPitchforkLevels.Add(FirstModifiedSchiffPitchforkPercent, new PercentLineSettings
                {
                    Percent = FirstModifiedSchiffPitchforkPercent,
                    LineColor = ColorParser.Parse(FirstModifiedSchiffPitchforkColor),
                    Style = FirstModifiedSchiffPitchforkStyle,
                    Thickness = FirstModifiedSchiffPitchforkThickness,
                });
            }

            if (ShowSecondModifiedSchiffPitchfork)
            {
                modifiedSchiffPitchforkLevels.Add(SecondModifiedSchiffPitchforkPercent, new PercentLineSettings
                {
                    Percent = SecondModifiedSchiffPitchforkPercent,
                    LineColor = ColorParser.Parse(SecondModifiedSchiffPitchforkColor),
                    Style = SecondModifiedSchiffPitchforkStyle,
                    Thickness = SecondModifiedSchiffPitchforkThickness,
                });
            }

            if (ShowThirdModifiedSchiffPitchfork)
            {
                modifiedSchiffPitchforkLevels.Add(ThirdModifiedSchiffPitchforkPercent, new PercentLineSettings
                {
                    Percent = ThirdModifiedSchiffPitchforkPercent,
                    LineColor = ColorParser.Parse(ThirdModifiedSchiffPitchforkColor),
                    Style = ThirdModifiedSchiffPitchforkStyle,
                    Thickness = ThirdModifiedSchiffPitchforkThickness,
                });
            }

            if (ShowFourthModifiedSchiffPitchfork)
            {
                modifiedSchiffPitchforkLevels.Add(FourthModifiedSchiffPitchforkPercent, new PercentLineSettings
                {
                    Percent = FourthModifiedSchiffPitchforkPercent,
                    LineColor = ColorParser.Parse(FourthModifiedSchiffPitchforkColor),
                    Style = FourthModifiedSchiffPitchforkStyle,
                    Thickness = FourthModifiedSchiffPitchforkThickness,
                });
            }

            if (ShowFifthModifiedSchiffPitchfork)
            {
                modifiedSchiffPitchforkLevels.Add(FifthModifiedSchiffPitchforkPercent, new PercentLineSettings
                {
                    Percent = FifthModifiedSchiffPitchforkPercent,
                    LineColor = ColorParser.Parse(FifthModifiedSchiffPitchforkColor),
                    Style = FifthModifiedSchiffPitchforkStyle,
                    Thickness = FifthModifiedSchiffPitchforkThickness,
                });
            }

            if (ShowSixthModifiedSchiffPitchfork)
            {
                modifiedSchiffPitchforkLevels.Add(SixthModifiedSchiffPitchforkPercent, new PercentLineSettings
                {
                    Percent = SixthModifiedSchiffPitchforkPercent,
                    LineColor = ColorParser.Parse(SixthModifiedSchiffPitchforkColor),
                    Style = SixthModifiedSchiffPitchforkStyle,
                    Thickness = SixthModifiedSchiffPitchforkThickness,
                });
            }

            if (ShowSeventhModifiedSchiffPitchfork)
            {
                modifiedSchiffPitchforkLevels.Add(SeventhModifiedSchiffPitchforkPercent, new PercentLineSettings
                {
                    Percent = SeventhModifiedSchiffPitchforkPercent,
                    LineColor = ColorParser.Parse(SeventhModifiedSchiffPitchforkColor),
                    Style = SeventhModifiedSchiffPitchforkStyle,
                    Thickness = SeventhModifiedSchiffPitchforkThickness,
                });
            }

            if (ShowEighthModifiedSchiffPitchfork)
            {
                modifiedSchiffPitchforkLevels.Add(EighthModifiedSchiffPitchforkPercent, new PercentLineSettings
                {
                    Percent = EighthModifiedSchiffPitchforkPercent,
                    LineColor = ColorParser.Parse(EighthModifiedSchiffPitchforkColor),
                    Style = EighthModifiedSchiffPitchforkStyle,
                    Thickness = EighthModifiedSchiffPitchforkThickness,
                });
            }

            if (ShowNinthModifiedSchiffPitchfork)
            {
                modifiedSchiffPitchforkLevels.Add(NinthModifiedSchiffPitchforkPercent, new PercentLineSettings
                {
                    Percent = NinthModifiedSchiffPitchforkPercent,
                    LineColor = ColorParser.Parse(NinthModifiedSchiffPitchforkColor),
                    Style = NinthModifiedSchiffPitchforkStyle,
                    Thickness = NinthModifiedSchiffPitchforkThickness,
                });
            }

            return modifiedSchiffPitchforkLevels;
        }

        private SideFanSettings[] GetPitchfanSideFanSettings()
        {
            var result = new List<SideFanSettings>();

            if (ShowFirstPitchfan)
            {
                result.Add(new SideFanSettings
                {
                    Percent = FirstPitchfanPercent,
                    Color = ColorParser.Parse(FirstPitchfanColor),
                    Style = FirstPitchfanStyle,
                    Thickness = FirstPitchfanThickness,
                });
            }

            if (ShowSecondPitchfan)
            {
                result.Add(new SideFanSettings
                {
                    Percent = SecondPitchfanPercent,
                    Color = ColorParser.Parse(SecondPitchfanColor),
                    Style = SecondPitchfanStyle,
                    Thickness = SecondPitchfanThickness,
                });
            }

            if (ShowThirdPitchfan)
            {
                result.Add(new SideFanSettings
                {
                    Percent = ThirdPitchfanPercent,
                    Color = ColorParser.Parse(ThirdPitchfanColor),
                    Style = ThirdPitchfanStyle,
                    Thickness = ThirdPitchfanThickness,
                });
            }

            if (ShowFourthPitchfan)
            {
                result.Add(new SideFanSettings
                {
                    Percent = FourthPitchfanPercent,
                    Color = ColorParser.Parse(FourthPitchfanColor),
                    Style = FourthPitchfanStyle,
                    Thickness = FourthPitchfanThickness,
                });
            }

            if (ShowFifthPitchfan)
            {
                result.Add(new SideFanSettings
                {
                    Percent = FifthPitchfanPercent,
                    Color = ColorParser.Parse(FifthPitchfanColor),
                    Style = FifthPitchfanStyle,
                    Thickness = FifthPitchfanThickness,
                });
            }

            if (ShowSixthPitchfan)
            {
                result.Add(new SideFanSettings
                {
                    Percent = SixthPitchfanPercent,
                    Color = ColorParser.Parse(SixthPitchfanColor),
                    Style = SixthPitchfanStyle,
                    Thickness = SixthPitchfanThickness,
                });
            }

            if (ShowSeventhPitchfan)
            {
                result.Add(new SideFanSettings
                {
                    Percent = SeventhPitchfanPercent,
                    Color = ColorParser.Parse(SeventhPitchfanColor),
                    Style = SeventhPitchfanStyle,
                    Thickness = SeventhPitchfanThickness,
                });
            }

            if (ShowEighthPitchfan)
            {
                result.Add(new SideFanSettings
                {
                    Percent = EighthPitchfanPercent,
                    Color = ColorParser.Parse(EighthPitchfanColor),
                    Style = EighthPitchfanStyle,
                    Thickness = EighthPitchfanThickness,
                });
            }

            if (ShowNinthPitchfan)
            {
                result.Add(new SideFanSettings
                {
                    Percent = NinthPitchfanPercent,
                    Color = ColorParser.Parse(NinthPitchfanColor),
                    Style = NinthPitchfanStyle,
                    Thickness = NinthPitchfanThickness,
                });
            }

            result.ToList().ForEach(iSettings => result.Add(new SideFanSettings
            {
                Percent = -iSettings.Percent,
                Color = iSettings.Color,
                Style = iSettings.Style,
                Thickness = iSettings.Thickness,
            }));

            return result.ToArray();
        }
    }
}