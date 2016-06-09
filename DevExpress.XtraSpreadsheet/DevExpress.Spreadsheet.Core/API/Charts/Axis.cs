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
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Office;
using DevExpress.Spreadsheet.Drawings;
namespace DevExpress.Spreadsheet.Charts {
	public enum AxisGroup {
		Primary,
		Secondary
	}
	public enum AxisType {
		Category,
		Date,
		Value,
		Series
	}
	public enum AxisPosition {
		Bottom = DevExpress.XtraSpreadsheet.Model.AxisPosition.Bottom,
		Left = DevExpress.XtraSpreadsheet.Model.AxisPosition.Left,
		Right = DevExpress.XtraSpreadsheet.Model.AxisPosition.Right,
		Top = DevExpress.XtraSpreadsheet.Model.AxisPosition.Top
	}
	public enum AxisOrientation {
		MinMax = DevExpress.XtraSpreadsheet.Model.AxisOrientation.MinMax,
		MaxMin = DevExpress.XtraSpreadsheet.Model.AxisOrientation.MaxMin
	}
	public enum AxisTickMarks {
		Cross = DevExpress.XtraSpreadsheet.Model.TickMark.Cross,
		Inside = DevExpress.XtraSpreadsheet.Model.TickMark.Inside,
		None = DevExpress.XtraSpreadsheet.Model.TickMark.None,
		Outside = DevExpress.XtraSpreadsheet.Model.TickMark.Outside
	}
	public enum AxisTickLabelPosition {
		None = DevExpress.XtraSpreadsheet.Model.TickLabelPosition.None,
		High = DevExpress.XtraSpreadsheet.Model.TickLabelPosition.High,
		Low = DevExpress.XtraSpreadsheet.Model.TickLabelPosition.Low,
		NextTo = DevExpress.XtraSpreadsheet.Model.TickLabelPosition.NextTo
	}
	public enum AxisLabelAlignment {
		Center = DevExpress.XtraSpreadsheet.Model.LabelAlignment.Center,
		Left = DevExpress.XtraSpreadsheet.Model.LabelAlignment.Left,
		Right = DevExpress.XtraSpreadsheet.Model.LabelAlignment.Right
	}
	public enum AxisTimeUnits {
		Auto = DevExpress.XtraSpreadsheet.Model.TimeUnits.Auto,
		Days = DevExpress.XtraSpreadsheet.Model.TimeUnits.Days,
		Months = DevExpress.XtraSpreadsheet.Model.TimeUnits.Months,
		Years = DevExpress.XtraSpreadsheet.Model.TimeUnits.Years,
	}
	public enum AxisCrossPosition {
		BetweenTickMarks = DevExpress.XtraSpreadsheet.Model.AxisCrossBetween.Between,
		OnTickMarks = DevExpress.XtraSpreadsheet.Model.AxisCrossBetween.Midpoint
	}
	public interface AxisScaling {
		bool AutoMin { get; set; }
		bool AutoMax { get; set; }
		double Min { get; set; }
		double Max { get; set; }
		bool LogScale { get; set; }
		double LogBase { get; set; }
		AxisOrientation Orientation { get; set; }
	}
	public interface Axis : ShapeFormat, ShapeTextFormat {
		AxisType AxisType { get; }
		bool Visible { get; set; }
		AxisPosition Position { get; set; }
		AxisScaling Scaling { get; }
		NumberFormatOptions NumberFormat { get; }
		ChartTitleOptions Title { get; }
		ChartLineOptions MajorGridlines { get; }
		ChartLineOptions MinorGridlines { get; }
		AxisTickLabelPosition TickLabelPosition { get; set; }
		AxisTickMarks MajorTickMarks { get; set; }
		AxisTickMarks MinorTickMarks { get; set; }
		int TickLabelSkip { get; set; }
		int TickMarkSkip { get; set; }
		bool Auto { get; set; }
		AxisLabelAlignment LabelAlignment { get; set; }
		int LabelOffset { get; set; }
		bool NoMultilevelLabels { get; set; }
		bool AutoMajorUnit { get; }
		bool AutoMinorUnit { get; }
		double MajorUnit { get; set; }
		double MinorUnit { get; set; }
		void SetAutoMajorUnit();
		void SetAutoMinorUnit();
		AxisTimeUnits BaseTimeUnit { get; set; }
		AxisTimeUnits MajorTimeUnit { get; set; }
		AxisTimeUnits MinorTimeUnit { get; set; }
		DisplayUnitOptions DisplayUnits { get; }
		AxisCrossPosition CrossPosition { get; set; }
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.Office.API.Internal;
	#region NativeAxisScaling
	partial class NativeAxisScaling : NativeObjectBase, AxisScaling {
		readonly Model.AxisBase modelAxis;
		public NativeAxisScaling(Model.AxisBase modelAxis) {
			this.modelAxis = modelAxis;
		}
		Model.DocumentModel DocumentModel { get { return modelAxis.DocumentModel; } }
		#region AxisScaling Members
		#region AutoMin
		public bool AutoMin {
			get {
				CheckValid();
				return !modelAxis.Scaling.FixedMin;
			}
			set {
				CheckValid();
				modelAxis.Scaling.FixedMin = !value;
			}
		}
		#endregion
		#region AutoMax
		public bool AutoMax {
			get {
				CheckValid();
				return !modelAxis.Scaling.FixedMax;
			}
			set {
				CheckValid();
				modelAxis.Scaling.FixedMax = !value;
			}
		}
		#endregion
		#region Min
		public double Min {
			get {
				CheckValid();
				return modelAxis.Scaling.Min; 
			}
			set {
				CheckValid();
				modelAxis.Scaling.Min = value;
			}
		}
		#endregion
		#region Max
		public double Max {
			get {
				CheckValid();
				return modelAxis.Scaling.Max;
			}
			set {
				CheckValid();
				modelAxis.Scaling.Max = value;
			}
		}
		#endregion
		#region LogScale
		public bool LogScale {
			get {
				CheckValid();
				return modelAxis.Scaling.LogScale;
			}
			set {
				CheckValid();
				modelAxis.Scaling.LogScale = value;
			}
		}
		#endregion
		#region LogBase
		public double LogBase {
			get {
				CheckValid();
				return modelAxis.Scaling.LogBase;
			}
			set {
				CheckValid();
				modelAxis.Scaling.LogBase = value;
			}
		}
		#endregion
		#region Orientation
		public AxisOrientation Orientation {
			get {
				CheckValid();
				return (AxisOrientation)modelAxis.Scaling.Orientation;
			}
			set {
				if (Orientation == value)
					return;
				DocumentModel.BeginUpdate();
				try {
					modelAxis.Scaling.Orientation = (Model.AxisOrientation)value;
					SetupCrossesAxis();
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		#endregion
		#endregion
		void SetupCrossesAxis() {
			if (modelAxis.AxisType == Model.AxisDataType.Series)
				return;
			Model.AxisBase crossesAxis = modelAxis.CrossesAxis;
			if (crossesAxis == null || crossesAxis.Crosses == Model.AxisCrosses.AtValue)
				return;
			crossesAxis.Position = GetAxisPosition(crossesAxis.Crosses);
		}
		Model.AxisPosition GetAxisPosition(Model.AxisCrosses axisCrosses) {
			bool horiz = modelAxis.Position == Model.AxisPosition.Bottom || modelAxis.Position == Model.AxisPosition.Top;
			if (modelAxis.Scaling.Orientation == Model.AxisOrientation.MaxMin) {
				if (axisCrosses == Model.AxisCrosses.Max)
					return horiz ? Model.AxisPosition.Left : Model.AxisPosition.Bottom;
				return horiz ? Model.AxisPosition.Right : Model.AxisPosition.Top;
			}
			if (axisCrosses == Model.AxisCrosses.Max)
				return horiz ? Model.AxisPosition.Right : Model.AxisPosition.Top;
			return horiz ? Model.AxisPosition.Left : Model.AxisPosition.Bottom;
		}
	}
	#endregion
	#region NativeAxisBase
	abstract partial class NativeAxisBase : NativeShapeTextFormat, Axis {
		#region Fields
		readonly Model.AxisBase modelAxis;
		readonly NativeWorkbook nativeWorkbook;
		NativeShapeFormat nativeShapeFormat;
		NativeAxisScaling nativeScaling;
		NativeNumberFormatOptions nativeNumberFormat;
		NativeChartTitleOptions nativeTitle;
		NativeChartLineOptions nativeMajorGridlines;
		NativeChartLineOptions nativeMinorGridlines;
		#endregion
		protected NativeAxisBase(Model.AxisBase modelAxis, NativeWorkbook nativeWorkbook) 
			: base(modelAxis.TextProperties) {
			this.modelAxis = modelAxis;
			this.nativeWorkbook = nativeWorkbook;
		}
		Model.DocumentModel DocumentModel { get { return modelAxis.DocumentModel; } }
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (nativeShapeFormat != null)
				nativeShapeFormat.IsValid = value;
			if (nativeScaling != null)
				nativeScaling.IsValid = value;
			if (nativeNumberFormat != null)
				nativeNumberFormat.IsValid = value;
			if (nativeTitle != null)
				nativeTitle.IsValid = value;
			if (nativeMajorGridlines != null)
				nativeMajorGridlines.IsValid = value;
			if (nativeMinorGridlines != null)
				nativeMinorGridlines.IsValid = value;
		}
		protected NativeWorkbook NativeWorkbook { get { return nativeWorkbook; } }
		#region Axis Members
		public abstract AxisType AxisType { get; }
		#region Visible
		public bool Visible {
			get {
				CheckValid();
				return !modelAxis.Delete;
			}
			set {
				CheckValid();
				modelAxis.Delete = !value;
			}
		}
		#endregion
		#region AxisPosition
		public AxisPosition Position {
			get {
				CheckValid();
				return (AxisPosition)modelAxis.Position;
			}
			set {
				if (Position == value)
					return;
				DocumentModel.BeginUpdate();
				try {
					modelAxis.Position = (Model.AxisPosition)value;
					SetupCrossing();
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		void SetupCrossing() {
			if (modelAxis.AxisType == Model.AxisDataType.Series || modelAxis.Crosses == Model.AxisCrosses.AtValue)
				return;
			Model.AxisBase crossesAxis = modelAxis.CrossesAxis;
			if (crossesAxis == null)
				return;
			bool maxMin = crossesAxis.Scaling.Orientation == Model.AxisOrientation.MaxMin;
			switch (modelAxis.Position) {
				case Model.AxisPosition.Left:
				case Model.AxisPosition.Bottom:
					modelAxis.Crosses = maxMin ? Model.AxisCrosses.Max : Model.AxisCrosses.AutoZero;
					break;
				case Model.AxisPosition.Right:
				case Model.AxisPosition.Top:
					modelAxis.Crosses = maxMin ? Model.AxisCrosses.AutoZero : Model.AxisCrosses.Max;
					break;
			}
		}
		#endregion
		#region AxisScaling
		public AxisScaling Scaling {
			get {
				CheckValid();
				if (nativeScaling == null)
					nativeScaling = new NativeAxisScaling(modelAxis);
				return nativeScaling;
			}
		}
		#endregion
		#region NumberFormat
		public NumberFormatOptions NumberFormat {
			get {
				CheckValid();
				if (nativeNumberFormat == null)
					nativeNumberFormat = new NativeNumberFormatOptions(modelAxis.NumberFormat);
				return nativeNumberFormat;
			}
		}
		#endregion
		#region Title
		public ChartTitleOptions Title {
			get {
				CheckValid();
				if (nativeTitle == null)
					nativeTitle = new NativeChartTitleOptions(modelAxis.Title, nativeWorkbook);
				return nativeTitle;
			}
		}
		#endregion
		#region MajorGridlines
		public ChartLineOptions MajorGridlines {
			get {
				CheckValid();
				if (nativeMajorGridlines == null)
					nativeMajorGridlines = new NativeChartLineOptions(new MajorGridlinesAdapter(modelAxis), nativeWorkbook);
				return nativeMajorGridlines;
			}
		}
		#endregion
		#region MinorGridlines
		public ChartLineOptions MinorGridlines {
			get {
				CheckValid();
				if (nativeMinorGridlines == null)
					nativeMinorGridlines = new NativeChartLineOptions(new MinorGridlinesAdapter(modelAxis), nativeWorkbook);
				return nativeMinorGridlines;
			}
		}
		#endregion
		#region TickLabelPos
		public AxisTickLabelPosition TickLabelPosition {
			get {
				CheckValid();
				return (AxisTickLabelPosition)modelAxis.TickLabelPos;
			}
			set {
				CheckValid();
				modelAxis.TickLabelPos = (Model.TickLabelPosition)value;
			}
		}
		#endregion
		#region MajorTickMarks
		public AxisTickMarks MajorTickMarks {
			get {
				CheckValid();
				return (AxisTickMarks)modelAxis.MajorTickMark;
			}
			set {
				CheckValid();
				modelAxis.MajorTickMark = (Model.TickMark)value;
			}
		}
		#endregion
		#region MinorTickMarks
		public AxisTickMarks MinorTickMarks {
			get {
				CheckValid();
				return (AxisTickMarks)modelAxis.MinorTickMark;
			}
			set {
				CheckValid();
				modelAxis.MinorTickMark = (Model.TickMark)value;
			}
		}
		#endregion
		#region Unsupported Members
		#region TickLabelSkip
		public virtual int TickLabelSkip {
			get {
				CheckValid();
				return Model.AxisTickBase.DefaultTickSkip;
			}
			set {
				CheckValid();
			}
		}
		#endregion
		#region TickMarkSkip
		public virtual int TickMarkSkip {
			get {
				CheckValid();
				return Model.AxisTickBase.DefaultTickSkip;
			}
			set {
				CheckValid();
			}
		}
		#endregion
		#region Auto
		public virtual bool Auto {
			get {
				CheckValid();
				return true;
			}
			set {
				CheckValid();
			}
		}
		#endregion
		#region LabelAlign
		public virtual AxisLabelAlignment LabelAlignment {
			get {
				CheckValid();
				return AxisLabelAlignment.Center;
			}
			set {
				CheckValid();
			}
		}
		#endregion
		#region LabelOffset
		public virtual int LabelOffset {
			get {
				CheckValid();
				return Model.AxisInfo.DefaultInfo.LabelOffset;
			}
			set {
				CheckValid();
			}
		}
		#endregion
		#region NoMultilevelLabels
		public virtual bool NoMultilevelLabels {
			get {
				CheckValid();
				return false;
			}
			set {
				CheckValid();
			}
		}
		#endregion
		#region AutoMajorUnit
		public virtual bool AutoMajorUnit {
			get {
				CheckValid();
				return true;
			}
		}
		#endregion
		#region AutoMinorUnit
		public virtual bool AutoMinorUnit {
			get {
				CheckValid();
				return true;
			}
		}
		#endregion
		#region MajorUnit
		public virtual double MajorUnit {
			get {
				CheckValid();
				return 0;
			}
			set {
				CheckValid();
			}
		}
		#endregion
		#region MinorUnit
		public virtual double MinorUnit {
			get {
				CheckValid();
				return 0;
			}
			set {
				CheckValid();
			}
		}
		#endregion
		#region SetAutoMajor/MinorUnit
		public virtual void SetAutoMajorUnit() {
			CheckValid();
		}
		public virtual void SetAutoMinorUnit() {
			CheckValid();
		}
		#endregion
		#region BaseTimeUnit
		public virtual AxisTimeUnits BaseTimeUnit {
			get {
				CheckValid();
				return AxisTimeUnits.Auto;
			}
			set {
				CheckValid();
			}
		}
		#endregion
		#region MajorTimeUnit
		public virtual AxisTimeUnits MajorTimeUnit {
			get {
				CheckValid();
				return AxisTimeUnits.Auto;
			}
			set {
				CheckValid();
			}
		}
		#endregion
		#region MinorTimeUnit
		public virtual AxisTimeUnits MinorTimeUnit {
			get {
				CheckValid();
				return AxisTimeUnits.Auto;
			}
			set {
				CheckValid();
			}
		}
		#endregion
		#region DisplayUnits
		public virtual DisplayUnitOptions DisplayUnits {
			get {
				CheckValid();
				return null;
			}
		}
		#endregion
		#region CrossPosition
		public virtual AxisCrossPosition CrossPosition {
			get {
				CheckValid();
				return AxisCrossPosition.OnTickMarks;
			}
			set {
				CheckValid();
			}
		}
		#endregion
		#endregion
		#region ShapeFormat Members
		public ShapeFill Fill {
			get {
				CheckValid();
				CheckShapeFormat();
				return nativeShapeFormat.Fill;
			}
		}
		public ShapeOutline Outline {
			get {
				CheckValid();
				CheckShapeFormat();
				return nativeShapeFormat.Outline;
			}
		}
		public void ResetToMatchStyle() {
			CheckValid();
			CheckShapeFormat();
			nativeShapeFormat.ResetToMatchStyle();
		}
		void CheckShapeFormat() {
			if (nativeShapeFormat == null)
				nativeShapeFormat = new NativeShapeFormat(modelAxis.ShapeProperties, nativeWorkbook);
		}
		#endregion
		#endregion
	}
	#endregion
	#region NativeCategoryAxis
	partial class NativeCategoryAxis : NativeAxisBase {
		readonly Model.CategoryAxis modelAxis;
		public NativeCategoryAxis(Model.CategoryAxis modelAxis, NativeWorkbook nativeWorkbook)
			: base(modelAxis, nativeWorkbook) {
			this.modelAxis = modelAxis;
		}
		#region Axis Members
		#region AxisType
		public override AxisType AxisType {
			get {
				CheckValid();
				return AxisType.Category;
			}
		}
		#endregion
		#region TickLabelSkip
		public override int TickLabelSkip {
			get {
				CheckValid();
				return modelAxis.TickLabelSkip;
			}
			set {
				CheckValid();
				modelAxis.TickLabelSkip = value;
			}
		}
		#endregion
		#region TickMarkSkip
		public override int TickMarkSkip {
			get {
				CheckValid();
				return modelAxis.TickMarkSkip;
			}
			set {
				CheckValid();
				modelAxis.TickMarkSkip = value;
			}
		}
		#endregion
		#region Auto
		public override bool Auto {
			get {
				CheckValid();
				return modelAxis.Auto;
			}
			set {
				CheckValid();
				modelAxis.Auto = value;
			}
		}
		#endregion
		#region LabelAlign
		public override AxisLabelAlignment LabelAlignment {
			get {
				CheckValid();
				return (AxisLabelAlignment)modelAxis.LabelAlign;
			}
			set {
				CheckValid();
				modelAxis.LabelAlign = (Model.LabelAlignment)value;
			}
		}
		#endregion
		#region LabelOffset
		public override int LabelOffset {
			get {
				CheckValid();
				return modelAxis.LabelOffset;
			}
			set {
				CheckValid();
				modelAxis.LabelOffset = value;
			}
		}
		#endregion
		#region NoMultilevelLabels
		public override bool NoMultilevelLabels {
			get {
				CheckValid();
				return modelAxis.NoMultilevelLabels;
			}
			set {
				CheckValid();
				modelAxis.NoMultilevelLabels = value;
			}
		}
		#endregion
		#endregion
	}
	#endregion
	#region NativeValueAxis
	partial class NativeValueAxis : NativeAxisBase {
		#region Fields
		readonly Model.ValueAxis modelValueAxis;
		NativeDisplayUnitOptions nativeDisplayUnits;
		#endregion
		public NativeValueAxis(Model.ValueAxis modelValueAxis, NativeWorkbook nativeWorkbook)
			: base(modelValueAxis, nativeWorkbook) {
			this.modelValueAxis = modelValueAxis;
		}
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (nativeDisplayUnits != null)
				nativeDisplayUnits.IsValid = value;
		}
		#region Axis Members
		#region AxisType
		public override AxisType AxisType {
			get {
				CheckValid();
				return AxisType.Value;
			}
		}
		#endregion
		#region AutoMajorUnit
		public override bool AutoMajorUnit {
			get {
				CheckValid();
				return !modelValueAxis.FixedMajorUnit;
			}
		}
		#endregion
		#region AutoMinorUnit
		public override bool AutoMinorUnit {
			get {
				CheckValid();
				return !modelValueAxis.FixedMinorUnit;
			}
		}
		#endregion
		#region MajorUnit
		public override double MajorUnit {
			get {
				CheckValid();
				return modelValueAxis.MajorUnit;
			}
			set {
				CheckValid();
				modelValueAxis.MajorUnit = value;
			}
		}
		#endregion
		#region MinorUnit
		public override double MinorUnit {
			get {
				CheckValid();
				return modelValueAxis.MinorUnit;
			}
			set {
				CheckValid();
				modelValueAxis.MinorUnit = value;
			}
		}
		#endregion
		#region SetAutoMajor/MinorUnit
		public override void SetAutoMajorUnit() {
			CheckValid();
			modelValueAxis.MajorUnit = 0;
		}
		public override void SetAutoMinorUnit() {
			CheckValid();
			modelValueAxis.MinorUnit = 0;
		}
		#endregion
		#region DisplayUnits
		public override DisplayUnitOptions DisplayUnits {
			get {
				CheckValid();
				if (nativeDisplayUnits == null)
					nativeDisplayUnits = new NativeDisplayUnitOptions(modelValueAxis.DisplayUnit, NativeWorkbook);
				return nativeDisplayUnits;
			}
		}
		#endregion
		#region CrossPosition
		public override AxisCrossPosition CrossPosition {
			get {
				CheckValid();
				return (AxisCrossPosition)modelValueAxis.CrossBetween;
			}
			set {
				CheckValid();
				modelValueAxis.CrossBetween = (Model.AxisCrossBetween)value;
			}
		}
		#endregion
		#endregion
	}
	#endregion
	#region NativeDateAxis
	partial class NativeDateAxis : NativeAxisBase {
		readonly Model.DateAxis modelAxis;
		public NativeDateAxis(Model.DateAxis modelAxis, NativeWorkbook nativeWorkbook) 
			: base(modelAxis, nativeWorkbook) {
			this.modelAxis = modelAxis;
		}
		#region Axis Members
		#region AxisType
		public override AxisType AxisType {
			get {
				CheckValid();
				return AxisType.Date;
			}
		}
		#endregion
		#region Auto
		public override bool Auto {
			get {
				CheckValid();
				return modelAxis.Auto;
			}
			set {
				CheckValid();
				modelAxis.Auto = value;
			}
		}
		#endregion
		#region LabelOffset
		public override int LabelOffset {
			get {
				CheckValid();
				return modelAxis.LabelOffset;
			}
			set {
				CheckValid();
				modelAxis.LabelOffset = value;
			}
		}
		#endregion
		#region AutoMajorUnit
		public override bool AutoMajorUnit {
			get {
				CheckValid();
				return !modelAxis.FixedMajorUnit;
			}
		}
		#endregion
		#region AutoMinorUnit
		public override bool AutoMinorUnit {
			get {
				CheckValid();
				return !modelAxis.FixedMinorUnit;
			}
		}
		#endregion
		#region MajorUnit
		public override double MajorUnit {
			get {
				CheckValid();
				return modelAxis.MajorUnit;
			}
			set {
				CheckValid();
				modelAxis.MajorUnit = value;
			}
		}
		#endregion
		#region MinorUnit
		public override double MinorUnit {
			get {
				CheckValid();
				return modelAxis.MinorUnit;
			}
			set {
				CheckValid();
				modelAxis.MinorUnit = value;
			}
		}
		#endregion
		#region SetAutoMajor/MinorUnit
		public override void SetAutoMajorUnit() {
			CheckValid();
			modelAxis.MajorUnit = 0;
		}
		public override void SetAutoMinorUnit() {
			CheckValid();
			modelAxis.MinorUnit = 0;
		}
		#endregion
		#region BaseTimeUnit
		public override AxisTimeUnits BaseTimeUnit {
			get {
				CheckValid();
				return (AxisTimeUnits)modelAxis.BaseTimeUnit;
			}
			set {
				CheckValid();
				modelAxis.BaseTimeUnit = (Model.TimeUnits)value;
			}
		}
		#endregion
		#region MajorTimeUnit
		public override AxisTimeUnits MajorTimeUnit {
			get {
				CheckValid();
				return (AxisTimeUnits)modelAxis.MajorTimeUnit;
			}
			set {
				CheckValid();
				modelAxis.MajorTimeUnit = (Model.TimeUnits)value;
			}
		}
		#endregion
		#region MinorTimeUnit
		public override AxisTimeUnits MinorTimeUnit {
			get {
				CheckValid();
				return (AxisTimeUnits)modelAxis.MinorTimeUnit;
			}
			set {
				CheckValid();
				modelAxis.MinorTimeUnit = (Model.TimeUnits)value;
			}
		}
		#endregion
		#endregion
	}
	#endregion
	#region NativeSeriesAxis
	partial class NativeSeriesAxis : NativeAxisBase {
		readonly Model.SeriesAxis modelAxis;
		public NativeSeriesAxis(Model.SeriesAxis modelAxis, NativeWorkbook nativeWorkbook) 
			: base(modelAxis, nativeWorkbook) {
			this.modelAxis = modelAxis;
		}
		#region Axis Members
		#region AxisType
		public override AxisType AxisType {
			get {
				CheckValid();
				return AxisType.Series;
			}
		}
		#endregion
		#region TickLabelSkip
		public override int TickLabelSkip {
			get {
				CheckValid();
				return modelAxis.TickLabelSkip;
			}
			set {
				CheckValid();
				modelAxis.TickLabelSkip = value;
			}
		}
		#endregion
		#region TickMarkSkip
		public override int TickMarkSkip {
			get {
				CheckValid();
				return modelAxis.TickMarkSkip;
			}
			set {
				CheckValid();
				modelAxis.TickMarkSkip = value;
			}
		}
		#endregion
		#endregion
	}
	#endregion
}
