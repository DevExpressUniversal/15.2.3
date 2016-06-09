#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Drawing;
using DevExpress.Utils.Drawing;
using System.ComponentModel;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraReports;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using DevExpress.XtraScheduler.Reporting.Native;
using DevExpress.XtraScheduler.Printing.Native;
using DevExpress.XtraScheduler.Printing;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler.Reporting {
	public interface ISupportPrintableTimeInterval {
		TimeInterval GetPrintTimeInterval(PrintContentMode displayMode);
	}
	public interface ISupportPrintableResources {
		ResourceBaseCollection GetPrintResources();
	}
	public enum PrintInColumnMode { All, Odd, Even };
	public enum PrintContentMode { CurrentColumn, AllColumns};
	public enum TimeIntervalFormatType { Default, Daily, Weekly, Monthly, Timeline }
	[
	XRDesigner("DevExpress.XtraScheduler.Reporting.Design.DataDependentControlDesigner," + AssemblyInfo.SRAssemblySchedulerReportingExtensions),
	Designer("DevExpress.XtraScheduler.Reporting.Design.DataDependentControlDesigner_," + AssemblyInfo.SRAssemblySchedulerDesign)
	]
	public abstract class DataDependentControlBase : ReportRelatedControlBase {		
		PrintInColumnMode printInColumn;
		protected DataDependentControlBase() {
		}
		protected DataDependentControlBase(ReportViewBase view)
			: base(view) {
		}
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DataDependentControlBasePrintInColumn"),
#endif
DefaultValue(PrintInColumnMode.All), Category(SRCategoryNames.Layout)]
		public PrintInColumnMode PrintInColumn { get { return printInColumn; } set { printInColumn = value; } }
		protected override BaseHeaderAppearance CreateAppearance() {
			return new BaseHeaderAppearance();
		}		
		protected internal override void CalculateLayoutCore(ControlLayoutInfo info) {
			if (ShouldRecalculatePrintInfo())  {
				DisposePrintInfo();
				BeforeCalculatePrintInfo();
				PrintInfo = CalculatePrintInfo(info);
			}
		}
		protected virtual void BeforeCalculatePrintInfo() {
		}
		protected virtual bool ShouldRecalculatePrintInfo() {
			return PrintInfo == null;
		}
		protected override void OnSizeChanged(DevExpress.XtraReports.UI.ChangeEventArgs e) {
			DisposePrintInfo();
		}
		protected abstract ControlPrintInfo CalculatePrintInfo(ControlLayoutInfo info);
		protected internal override bool CanWriteContent() {			
			int columnIndex = GetPrintColumnIndex();
			bool canPrintInColumn = CanPrintInColumn(columnIndex);
			return canPrintInColumn && base.CanWriteContent();
		}
		protected int GetPrintColumnIndex() {
			return SchedulerReport != null ? SchedulerReport.GetPrintColumnIndex(this) : -1;
		}
		protected bool CanPrintInColumn(int columnIndex) {
			if (IsDesignMode || columnIndex < 0)
				return true;
			switch(PrintInColumn) {
				case PrintInColumnMode.Odd:
					return IsEvenColumn(columnIndex);
				case PrintInColumnMode.Even:
					return !IsEvenColumn(columnIndex);
				case PrintInColumnMode.All:
					return true;
				default:
					return true;
			}
		}
		protected bool IsEvenColumn(int columnIndex) {
			return columnIndex % 2 == 0;
		}		
	}
	public abstract class TextInfoControlBase : DataDependentControlBase {
		const float defaultFontHeight = 14.0f;
		const bool DefaultAutoScaleFont = true;
		Font adjustedFont;
		GraphicsInfoArgs graphicsInfoArgs;
		bool autoScaleFont = DefaultAutoScaleFont;
		protected TextInfoControlBase() {
		}
		protected TextInfoControlBase(ReportViewBase view)
			: base(view) {
		}
		#region Base Properties
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TextInfoControlBaseForeColor"),
#endif
Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Color ForeColor { get { return base.ForeColor; } set { base.ForeColor = value; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TextInfoControlBaseFont"),
#endif
Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new Font Font { get { return base.Font; } set { base.Font = value; } }
		#endregion
		#region Properties
		protected internal GraphicsInfoArgs GraphicsInfoArgs { get { return graphicsInfoArgs; } set { graphicsInfoArgs = value; } }
		protected new TextInfoControlPrintInfo PrintInfo { get { return (TextInfoControlPrintInfo)base.PrintInfo; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TextInfoControlBaseAutoScaleFont"),
#endif
DefaultValue(DefaultAutoScaleFont), Category(SRCategoryNames.Appearance)]
		public bool AutoScaleFont {
			get { return autoScaleFont; }
			set {
				if (autoScaleFont == value)
					return;
				autoScaleFont = value;
				OnAutoScaleFontChanged();
			}
		}
		protected virtual void OnAutoScaleFontChanged() {
			DisposePrintInfo();
		}
		[Category(SRCategoryNames.Scheduler), DefaultValue(null), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ReportViewBase View {
			get {
				return TimeCells != null ? TimeCells.View : null;
			}
			set { }
		}
		protected override BorderSide DefaultBorders { get { return BorderSide.None; } }
		#endregion
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (adjustedFont != null) {
					adjustedFont.Dispose();
					adjustedFont = null;
				}
				graphicsInfoArgs = null;
			}
			base.Dispose(disposing);
		}
		protected internal abstract float CalculateSecondLineTextSizeMultiplier();
		protected internal abstract MultilineText CalculateMultilineText();
		protected abstract TextCustomizingEventArgs CreateTextCustomizingEventArgs(string firstLineText, string secondLineText);
		#region Events
		static readonly object CustomizeTextEvent = new object();
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TextInfoControlBaseCustomizeText"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event TextCustomizingEventHandler CustomizeText
		{
			add { Events.AddHandler(CustomizeTextEvent, value); }
			remove { Events.RemoveHandler(CustomizeTextEvent, value); }
		}
		protected void RaiseCustomizeText(TextCustomizingEventArgs args) {
			TextCustomizingEventHandler handler = (TextCustomizingEventHandler)Events[CustomizeTextEvent];
			if (handler != null) handler(this, args);
		}
		#endregion
		protected internal Font GetAdjustedFont() {
			if (adjustedFont != null && !IsValidAdjustedFont()) {
				adjustedFont.Dispose();
			}
			adjustedFont = CreateAdjustedFont();
			return adjustedFont;
		}
		protected override void OnFontChanged() {
			base.OnFontChanged();
			DisposePrintInfo();
		}
		protected bool IsValidAdjustedFont() {
			return adjustedFont.FontFamily.Equals(Font.FontFamily) && adjustedFont.Style.Equals(Font.Style);
		}
		protected override void ApplyPrintColorSchema() {
			PrintColorConverter converter = GetColorConverter(PrintColorSchemas.Content);
			if (PrintInfo != null) {
				if (PrintInfo.FirstLineTextInfo != null)
					converter.ConvertAppearance(PrintInfo.FirstLineTextInfo.TextAppearance);
				if (PrintInfo.SecondLineTextInfo != null)
					converter.ConvertAppearance(PrintInfo.SecondLineTextInfo.TextAppearance);
			}
		}		
		protected virtual Font CreateAdjustedFont() {
			float fontSize = defaultFontHeight;
			if (!AutoScaleFont)
				fontSize = XRConvert.Convert(Font.Size, GraphicsDpi.UnitToDpi(Font.Unit), GraphicsDpi.Point);
			return new Font(Font.FontFamily, fontSize, Font.Style, GraphicsUnit.Point);
		}
		protected override void InitializeGraphicsInfoArgs(GraphicsInfoArgs info) {
			this.graphicsInfoArgs = info;
		}
		protected override void FinalizeGraphicsInfoArgs(GraphicsInfoArgs info) {
			this.graphicsInfoArgs = null;
		}
		protected override bool ShouldRecalculatePrintInfo() {
			if (base.ShouldRecalculatePrintInfo())
				return true;
			if (adjustedFont == null || !IsValidAdjustedFont())
				return true;
			if (TextInfoControlPrintInfo.Empty == PrintInfo)
				return false;
			if (PrintInfo.FirstLineTextInfo.TextAppearance.ForeColor != ForeColor)
				return true;
			return false;
		}
		protected override void OnSizeChanged(DevExpress.XtraReports.UI.ChangeEventArgs e) {
			DisposePrintInfo();
		}
		protected override ControlPrintInfo CalculatePrintInfo(ControlLayoutInfo info) {
			return CalculatePrintInfoCore(info);
		}		
		protected override bool CanCreateDesignBrick() {
			return base.CanCreateDesignBrick() || (TimeCells == null);
		}
		protected override string GetDesignBrickText() {
			if (TimeCells == null)
				return SchedulerLocalizer.GetString(SchedulerStringId.Reporting_NotAssigned_TimeCells);
			return base.GetDesignBrickText();
		}
		protected virtual ControlPrintInfo CalculatePrintInfoCore(ControlLayoutInfo info) {
			TextInfoLayoutCalculator calculator = CreateTextInfoLayoutCalculator();
			TextInfoLayoutCalculatorParameters parameters = CreateLayoutCalculatorParameters();
			Rectangle printBounds = info.ControlPrintBounds;
			InitializeCalculatorParameters(parameters, printBounds);
			if (String.IsNullOrEmpty(parameters.FirstLineText) && String.IsNullOrEmpty(parameters.SecondLineText))
				return TextInfoControlPrintInfo.Empty;
			TextInfoPreliminaryLayoutResult preliminaryResult = calculator.CalculatePreliminaryLayout(parameters);
			return calculator.Calculate(this, preliminaryResult, printBounds, parameters.SecondLineTextSizeMultiplier);
		}		
		protected internal virtual TextInfoLayoutCalculator CreateTextInfoLayoutCalculator() {
			return new TextInfoLayoutCalculator(Cache.Graphics, GetAdjustedFont(), ForeColor, AutoScaleFont);
		}
		protected abstract TextInfoLayoutCalculatorParameters CreateLayoutCalculatorParameters();
		protected virtual void InitializeCalculatorParameters(TextInfoLayoutCalculatorParameters parameters, Rectangle printBounds) {
			parameters.PrintBounds = printBounds;
			MultilineText lineText = CalculateMultilineText();
			TextCustomizingEventArgs args = CreateTextCustomizingEventArgs(lineText.FirstLineText, lineText.SecondLineText);
			RaiseCustomizeText(args);
			parameters.FirstLineText = args.Text;
			parameters.SecondLineText = args.SecondText;
			parameters.SecondLineTextSizeMultiplier = CalculateSecondLineTextSizeMultiplier();
		}
		protected internal override ControlPrintInfo CreatePrintInfo() {
			return TextInfoControlPrintInfo.Empty;
		}
	}
	#region TimeIntervalInfoBase
	public abstract class TimeIntervalInfoBase : TextInfoControlBase {
		const PrintContentMode DefaultPrintContentMode = PrintContentMode.CurrentColumn;
		TimeInterval printTimeInterval;
		TimeIntervalFormatterBase timeIntervalFormatter;
		PrintContentMode printContentMode;		
		protected TimeIntervalInfoBase() {
		}
		protected TimeIntervalInfoBase(ReportViewBase view)
			: base(view) {
		}
		#region Properties
		[Browsable(false)]
		public TimeInterval PrintTimeInterval { get { return printTimeInterval; } }
		protected internal TimeIntervalFormatterBase TimeIntervalFormatter { get { return timeIntervalFormatter; } }
		protected override int DefaultHeight { get { return 50; } }
		protected override int DefaultWidth { get { return 250; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimeIntervalInfoBasePrintContentMode"),
#endif
DefaultValue(DefaultPrintContentMode), Category(SRCategoryNames.Appearance)]
		public PrintContentMode PrintContentMode {
			get { return printContentMode; }
			set {
				if (printContentMode == value)
					return;
				printContentMode = value;
			}
		}
		#endregion
		protected internal override ViewInfoPainterBase CreatePainter() {
			return new ViewInfoPainterBase(); 
		}
		protected internal override ControlPrintControllerBase CreatePrintController() {
			return new DependentPrintController();
		}
		protected override void BeforeCalculatePrintInfo() {
			this.printTimeInterval = GetPrintTimeInterval();
			this.timeIntervalFormatter = GetTimeIntervalFormatter();
		}
		protected TimeInterval GetPrintTimeInterval() {
			ISupportPrintableTimeInterval support = TimeCells as ISupportPrintableTimeInterval;
			return support != null ? support.GetPrintTimeInterval(PrintContentMode) : TimeInterval.Empty;
		}
		protected override TextInfoLayoutCalculatorParameters CreateLayoutCalculatorParameters() {
			return new TimeIntervalLayoutCalculatorParameters();
		}
		protected internal override MultilineText CalculateMultilineText() {
			if (TimeInterval.Empty.Equals(PrintTimeInterval))
				return MultilineText.Empty;
			string firstLine = TimeIntervalFormatter.FormatFirstLineText(PrintTimeInterval);
			string secondLine = TimeIntervalFormatter.FormatSecondLineText(PrintTimeInterval);
			return new MultilineText(firstLine, secondLine);
		}
		protected internal abstract TimeIntervalFormatterBase GetTimeIntervalFormatter();
	}
	#endregion
	#region TimeIntervalInfo
	[DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "timeinfo.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraScheduler.Reporting.TimeIntervalInfo", "TimeIntervalInfo"),
	XRDesigner("DevExpress.XtraScheduler.Reporting.Design.TimeIntervalInfoDesigner," + AssemblyInfo.SRAssemblySchedulerReportingExtensions),
	Designer("DevExpress.XtraScheduler.Reporting.Design.TimeIntervalInfoDesigner_," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("An information control used to print a textual view of the time interval.")
	]
	public class TimeIntervalInfo : TimeIntervalInfoBase {		
		TimeIntervalFormatType formatType;
		public TimeIntervalInfo() {
		}
		public TimeIntervalInfo(ReportViewBase view)
			: base(view) {
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimeIntervalInfoFormatType"),
#endif
		DefaultValue(TimeIntervalFormatType.Default), Category(SRCategoryNames.Appearance)]
		public TimeIntervalFormatType FormatType {
			get { return formatType; }
			set {
				if(formatType == value)
					return;
				formatType = value;
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimeIntervalInfoTimeCells"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(null), Category(SRCategoryNames.Layout)]
		public new TimeCellsControlBase TimeCells {
			get { return base.TimeCells as TimeCellsControlBase; }
			set { base.TimeCells = value; }
		}
		protected override int DefaultHeight { get { return 100; } }
		protected override int DefaultWidth { get { return 300; } }
		#endregion
		protected internal override TimeIntervalFormatterBase GetTimeIntervalFormatter() {
			TimeIntervalFormatType type = GetActualTimeIntervalFormatType();
			TimeIntervalFormatterBase formatter = TimeIntervalFormatterBase.CreateInstance(type);
			formatter.Initialize(View);
			return formatter;
		}
		protected override bool ShouldRecalculatePrintInfo() {
			if (TimeIntervalFormatter == null)
				return true;
			if (TimeIntervalFormatter.Type != GetActualTimeIntervalFormatType())
				return true;
			if (!TimeInterval.Equals(PrintTimeInterval, GetPrintTimeInterval()))
				return true;
			return base.ShouldRecalculatePrintInfo();
		}
		protected override void InitializeCalculatorParameters(TextInfoLayoutCalculatorParameters parameters, Rectangle printBounds) {
			base.InitializeCalculatorParameters(parameters, printBounds);
			TimeIntervalLayoutCalculatorParameters intervalParams = (TimeIntervalLayoutCalculatorParameters)parameters;
			intervalParams.Interval = PrintTimeInterval;
		}
		protected internal override float CalculateSecondLineTextSizeMultiplier() {
			TimeIntervalFormatterBase formatter = GetTimeIntervalFormatter();
			return formatter.SecondLineTextSizeMultiplier;
		}
		protected override TextCustomizingEventArgs CreateTextCustomizingEventArgs(string firstLineText, string secondLineText) {
			return new TimeIntervalTextCustomizingEventArgs(firstLineText, secondLineText, PrintTimeInterval);
		}
		protected internal virtual TimeIntervalFormatType GetActualTimeIntervalFormatType() {
			if (FormatType != TimeIntervalFormatType.Default)
				return formatType;
			return View != null ? View.GetDefaultTimeIntervalFormatType() : FormatType;
		}
	}
	#endregion
	#region ResourceInfo
	[DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "resourceinfo.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraScheduler.Reporting.ResourceInfo", "ResourceInfo"),
	Description("An information control used for resource listing.")
	]
	public class ResourceInfo : TextInfoControlBase {
		const Char DefaultResourceDelimiterChar = ',';
		string resourceDelimiter = string.Empty;
		ResourceBaseCollection printResources;
		public ResourceInfo() {
		}
		public ResourceInfo(ReportViewBase view)
			: base(view) {
		}
		[Browsable(false)]
		public ResourceBaseCollection PrintResources { get { return printResources; } }
		protected override int DefaultHeight { get { return 50; } }
		protected override int DefaultWidth { get { return 250; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ResourceInfoResourceDelimiter"),
#endif
DefaultValue(""), Category(SRCategoryNames.Layout)]
		public string ResourceDelimiter {
			get {
				return resourceDelimiter;
			}
			set {
				string newVal = value != null ? value : string.Empty;
				if(resourceDelimiter == value)
					return;
				resourceDelimiter = newVal;
				OnResourceDelimiterChanged();
			}
		}
		protected virtual void OnResourceDelimiterChanged() {
			DisposePrintInfo();
		}
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ResourceInfoTimeCells"),
#endif
Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(null), Category(SRCategoryNames.Layout)]
		public new TimeCellsControlBase TimeCells {
			get { return base.TimeCells as TimeCellsControlBase; }
			set { base.TimeCells = value; }
		}
		protected internal override ViewInfoPainterBase CreatePainter() {
			return null;
		}
		protected internal override ControlPrintControllerBase CreatePrintController() {
			return new DependentPrintController();
		}
		protected override void BeforeCalculatePrintInfo() {
			this.printResources = GetPrintResources();
		}
		protected override bool ShouldRecalculatePrintInfo() {
			if (PrintResources == null)
				return true;
			if (!ResourceBase.InternalAreResourceCollectionsSame(PrintResources, GetPrintResources()))
				return true;
			return base.ShouldRecalculatePrintInfo();
		}
		protected ResourceBaseCollection GetPrintResources() {
			ISupportPrintableResources support = TimeCells as ISupportPrintableResources;
			return support != null ? support.GetPrintResources() : new ResourceBaseCollection();
		}
		protected override TextInfoLayoutCalculatorParameters CreateLayoutCalculatorParameters() {
			return new ResourceLayoutCalculatorParameters();
		}
		protected override void InitializeCalculatorParameters(TextInfoLayoutCalculatorParameters parameters, Rectangle printBounds) {
			base.InitializeCalculatorParameters(parameters, printBounds);
			ResourceLayoutCalculatorParameters resParams = (ResourceLayoutCalculatorParameters)parameters;
			resParams.Resources = PrintResources;
		}
		protected internal override float CalculateSecondLineTextSizeMultiplier() {
			return 1;
		}
		protected internal override MultilineText CalculateMultilineText() {
			return new MultilineText(FormatResourceString(PrintResources), string.Empty);
		}
		protected override TextCustomizingEventArgs CreateTextCustomizingEventArgs(string firstLineText, string secondLineText) {
			return new ResourceTextCustomizingEventArgs(firstLineText, PrintResources);
		}
		protected string FormatResourceString(ResourceBaseCollection resources) {
			string result = string.Empty;
			for (int i = 0; i < resources.Count; i++) {
				if (result.Length > 0)
					result += GetResourceDelimeterString();
				result += resources[i].Caption;
			}
			return result;
		}
		protected string GetResourceDelimeterString() {
			return !string.IsNullOrEmpty(resourceDelimiter) ? resourceDelimiter : String.Format("{0} ", DefaultResourceDelimiterChar);
		}
	}
	#endregion
	#region FormatTimeIntervalInfo
	[DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "formattimeinfo.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraScheduler.Reporting.FormatTimeIntervalInfo", "FormatTimeIntervalInfo"),
	XRDesigner("DevExpress.XtraScheduler.Reporting.Design.FormatTimeIntervalInfoDesigner," + AssemblyInfo.SRAssemblySchedulerReportingExtensions),
	Designer("DevExpress.XtraScheduler.Reporting.Design.FormatTimeIntervalInfoDesigner_," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("An information control used to print a textual view of the time interval, using the specified format.")
	]
	public class FormatTimeIntervalInfo : TimeIntervalInfoBase {
		string formatString = string.Empty; 
		bool autoFormat = true;
		public FormatTimeIntervalInfo() { 
		}
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("FormatTimeIntervalInfoAutoFormat"),
#endif
DefaultValue(true), Category(SRCategoryNames.Behavior)]
		public bool AutoFormat {
			get { return autoFormat; }
			set {
				if (autoFormat == value)
					return;
				autoFormat = value;
				DisposePrintInfo();
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("FormatTimeIntervalInfoFormatString"),
#endif
DefaultValue(""),
	   Category(SRCategoryNames.Behavior), RefreshProperties(RefreshProperties.All),
	   TypeConverter(typeof(DevExpress.Utils.Design.TimeIntervalFormatConverter))]
		public string FormatString {
			get { return formatString; }
			set {
				if (formatString == value)
					return;
				formatString = value;
				this.autoFormat = false;
				DisposePrintInfo();
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("FormatTimeIntervalInfoTimeCells"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(null), Category(SRCategoryNames.Layout)]
		public new TimeCellsControlBase TimeCells {
			get { return base.TimeCells as TimeCellsControlBase; }
			set { base.TimeCells = value; }
		}
		internal void ResetFormatString() {
			this.formatString = string.Empty;
			DisposePrintInfo();
		}
		protected internal override float CalculateSecondLineTextSizeMultiplier() {
			return 1;
		}
		protected override TextCustomizingEventArgs CreateTextCustomizingEventArgs(string firstLineText, string secondLineText) {
			return new TextCustomizingEventArgs(firstLineText);
		}
		protected override TextInfoLayoutCalculatorParameters CreateLayoutCalculatorParameters() {
			return new TimeIntervalLayoutCalculatorParameters();
		}
		protected internal override ControlPrintControllerBase CreatePrintController() {
			return new DependentPrintController();
		}
		protected internal override ViewInfoPainterBase CreatePainter() {
			return null;
		}
		protected internal override TimeIntervalFormatterBase GetTimeIntervalFormatter() {
			if (AutoFormat)
				return new AutomaticVisibleIntervalFormatter();
			return new SimpleVisibleIntervalFormatter();
		}
		protected override bool ShouldRecalculatePrintInfo() {
			if (!TimeInterval.Equals(PrintTimeInterval, GetPrintTimeInterval()))
				return true;
			return base.ShouldRecalculatePrintInfo();
		}
		protected override void BeforeCalculatePrintInfo() {
			base.BeforeCalculatePrintInfo();
			AutomaticVisibleIntervalFormatter formatter = TimeIntervalFormatter as AutomaticVisibleIntervalFormatter;
			if (formatter != null)
				formatter.FormatForMonth = CalculateFormatForMonth(PrintTimeInterval);
			SimpleVisibleIntervalFormatter simpleFormatter = TimeIntervalFormatter as SimpleVisibleIntervalFormatter;
			if (simpleFormatter != null) {
				simpleFormatter.FirstLineFormat = !String.IsNullOrEmpty(FormatString) ? FormatString : AutomaticVisibleIntervalFormatter.DefaultIntervalFormat;
			}
		}
		protected virtual bool CalculateFormatForMonth(TimeInterval interval) {
			if (View != null) {
				if (View is ReportMonthView) 
					return true;
				if (View is ReportTimelineView) {
					TimeScale baseScale = ((ReportTimelineView)View).GetBaseTimeScale();
					return baseScale.SortingWeight >= TimeSpan.FromDays(31);
				}
			}
			return false;
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Reporting.Native {
	#region TextInfoControlPrintInfo
	public class TextInfoControlPrintInfo : ControlPrintInfo {
		#region Fields
		HeaderTextInfo firstLineTextInfo;
		HeaderTextInfo secondLineTextInfo;
		static readonly ControlPrintInfo empty;
		#endregion
		#region Ctors
		static TextInfoControlPrintInfo() {
			empty = new EmptyTextControlPrintInfo();
		}
		public TextInfoControlPrintInfo() {
		}
		public TextInfoControlPrintInfo(TextInfoControlBase control, HeaderTextInfo firstLineTextInfo, HeaderTextInfo secondLineTextInfo)
			: base(control) {
			this.firstLineTextInfo = firstLineTextInfo;
			this.secondLineTextInfo = secondLineTextInfo;
		}
		#endregion
		#region Properties
		internal HeaderTextInfo FirstLineTextInfo { get { return firstLineTextInfo; } }
		internal HeaderTextInfo SecondLineTextInfo { get { return secondLineTextInfo; } }
		protected internal new TextInfoControlBase Control { get { return (TextInfoControlBase)base.Control; } }
		public static ControlPrintInfo Empty { get { return empty; } }
		#endregion        
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (firstLineTextInfo != null) {
						firstLineTextInfo.Dispose();
						firstLineTextInfo = null;
					}
					if (secondLineTextInfo != null) {
						secondLineTextInfo.Dispose();
						secondLineTextInfo = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		protected internal override ControlPrintInfo CloneCore() {
			HeaderTextInfo firstLine = FirstLineTextInfo.Clone();
			HeaderTextInfo secondLine = SecondLineTextInfo.Clone();
			TextInfoControlPrintInfo printInfo = new TextInfoControlPrintInfo(Control, firstLine, secondLine);
			return printInfo;
		}
		public override void Print(GraphicsCache cache) {
			if (FirstLineTextInfo != null)
				FirstLineTextInfo.Print(cache);
			if (SecondLineTextInfo != null)
				SecondLineTextInfo.Print(cache);
		}
	}
	#endregion
	public class EmptyTextControlPrintInfo : TextInfoControlPrintInfo {
		protected internal override ControlPrintInfo CloneCore() {
			return new EmptyTextControlPrintInfo();
		}
	}
}
