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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using DevExpress.Utils.Design;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	[ModelOf(typeof(AxisLabel)), TypeConverter(typeof(ExpandableObjectConverter))]
	public class AxisLabelModel : TitleBaseModel {
		AxisLabelResolveOverlappingOptionsModel resolveOverlappingOptionsModel;
		RectangleFillStyleModel fillStyleModel;
		RectangularBorderModel borderModel;
		protected AxisLabel Label { get { return (AxisLabel)Title; } }
		[
		PropertyForOptions(1, "General"), TypeConverter(typeof(BooleanTypeConverter)),
		DevExpress.XtraCharts.Designer.Native.DesignerDisplayName("ShowLabel")]
		public new bool Visible {
			get { return Label.Visible; }
			set { SetProperty("Visible", value); }
		}
		public new Color TextColor {
			get { return Label.TextColor; }
			set { SetProperty("TextColor", value); }
		}
		public int Angle {
			get { return Label.Angle; }
			set { SetProperty("Angle", value); }
		}
		[TypeConverter(typeof(BooleanTypeConverter))]
		public bool Staggered {
			get { return Label.Staggered; }
			set { SetProperty("Staggered", value); }
		}
		public int MaxWidth {
			get { return Label.MaxWidth; }
			set { SetProperty("MaxWidth", value); }
		}
		public int MaxLineCount {
			get { return Label.MaxLineCount; }
			set { SetProperty("MaxLineCount", value); }
		}
		[TypeConverter(typeof(StringAlignmentTypeConvertor))]
		public StringAlignment TextAlignment {
			get { return Label.TextAlignment; }
			set { SetProperty("TextAlignment", value); }
		}
		public Color BackColor {
			get { return Label.BackColor; }
			set { SetProperty("BackColor", value); }
		}
		[Editor(typeof(AxisLabelModelPatternEditor), typeof(UITypeEditor))]
		public string TextPattern {
			get { return Label.TextPattern; }
			set { SetProperty("TextPattern", value); }
		}
		public AxisLabelResolveOverlappingOptionsModel ResolveOverlappingOptions { get { return resolveOverlappingOptionsModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public RectangleFillStyleModel FillStyle { get { return fillStyleModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public RectangularBorderModel Border { get { return borderModel; } }
		public AxisLabelModel(AxisLabel label, CommandManager commandManager)
			: base(label, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if (resolveOverlappingOptionsModel != null)
				Children.Add(resolveOverlappingOptionsModel);
			if (fillStyleModel != null)
				Children.Add(fillStyleModel);
			if (borderModel != null)
				Children.Add(borderModel);
			base.AddChildren();
		}
		public override void Update() {
			this.resolveOverlappingOptionsModel = new AxisLabelResolveOverlappingOptionsModel(Label.ResolveOverlappingOptions, CommandManager);
			this.fillStyleModel = new RectangleFillStyleModel(Label.FillStyle, CommandManager);
			this.borderModel = (RectangularBorderModel)ModelHelper.CreateBorderModelInstance(Label.Border, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(AxisLabel3D))]
	public class AxisLabel3DModel : AxisLabelModel {
		protected new AxisLabel3D Label { get { return (AxisLabel3D)base.Label; } }
		[
		PropertyForOptions(2, "General"),
		DevExpress.XtraCharts.Designer.Native.DesignerDisplayName("LabelPosition")]
		public AxisLabel3DPosition Position {
			get { return Label.Position; }
			set { SetProperty("Position", value); }
		}
		public AxisLabel3DModel(AxisLabel3D label, CommandManager commandManager)
			: base(label, commandManager) {
		}
	}
	[ModelOf(typeof(CustomAxisLabel))]
	public class CustomAxisLabelModel : ChartElementNamedModel {
		RectangleFillStyleModel fillStyleModel;
		RectangularBorderModel borderModel;
		protected CustomAxisLabel Label { get { return (CustomAxisLabel)base.ChartElementNamed; } }
		[Category(Categories.Behavior), PropertyForOptions("Behavior"), TypeConverter(typeof(BooleanTypeConverter))]
		public bool Visible {
			get { return Label.Visible; }
			set { SetProperty("Visible", value); }
		}
		[Category(Categories.Behavior), TypeConverter(typeof(AxisValueTypeConverter))]
		public object AxisValue {
			get { return Label.AxisValue; }
			set { SetProperty("AxisValue", value); }
		}
		[Category(Categories.Appearance), TypeConverter(typeof(FontTypeConverter))]
		public Font Font {
			get { return Label.Font; }
			set { SetProperty("Font", value); }
		}
		[Category(Categories.Appearance)]
		public Color TextColor {
			get { return Label.TextColor; }
			set { SetProperty("TextColor", value); }
		}
		[Category(Categories.Appearance)]
		public Color BackColor {
			get { return Label.BackColor; }
			set { SetProperty("BackColor", value); }
		}
		[Category(Categories.Appearance), TypeConverter(typeof(ExpandableObjectConverter))]
		public RectangleFillStyleModel FillStyle { get { return fillStyleModel; } }
		[Category(Categories.Appearance), TypeConverter(typeof(ExpandableObjectConverter))]
		public RectangularBorderModel Border { get { return borderModel; } }
		public CustomAxisLabelModel(CustomAxisLabel label, CommandManager commandManager)
			: base(label, commandManager) {
		}
		protected override void AddChildren() {
			if (fillStyleModel != null)
				Children.Add(fillStyleModel);
			if (borderModel != null)
				Children.Add(borderModel);
			base.AddChildren();
		}
		public override void Update() {
			this.fillStyleModel = new RectangleFillStyleModel(Label.FillStyle, CommandManager);
			this.borderModel = (RectangularBorderModel)ModelHelper.CreateBorderModelInstance(Label.Border, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(RadarAxisXLabel))]
	public class RadarAxisXLabelModel : AxisLabelModel {
		protected new RadarAxisXLabel Label { get { return (RadarAxisXLabel)base.Label; } }
		[Browsable(false)]
		public new int Angle { get; set; }
		[Browsable(false)]
		public new bool Staggered { get; set; }
		[
		PropertyForOptions(2, "General"),
		DevExpress.XtraCharts.Designer.Native.DesignerDisplayName("LabelDirection")]
		public RadarAxisXLabelTextDirection TextDirection {
			get { return Label.TextDirection; }
			set { SetProperty("TextDirection", value); }
		}
		public RadarAxisXLabelModel(RadarAxisXLabel label, CommandManager commandManager)
			: base(label, commandManager) {
		}
	}
	[ModelOf(typeof(RadarAxisYLabel))]
	public class RadarAxisYLabelModel : AxisLabelModel {
		protected new RadarAxisYLabel Label { get { return (RadarAxisYLabel)base.Label; } }
		[Browsable(false)]
		public new int Angle { get; set; }
		[Browsable(false)]
		public new bool Staggered { get; set; }
		public RadarAxisYLabelModel(RadarAxisYLabel label, CommandManager commandManager)
			: base(label, commandManager) {
		}
	}
	[ModelOf(typeof(GridLines))]
	public class GridLinesModel : DesignerChartElementModelBase {
		readonly GridLines gridLines;
		LineStyleModel lineStyleModel;
		LineStyleModel minorLineStyleModel;
		protected GridLines GridLines { get { return gridLines; } }
		protected internal override ChartElement ChartElement { get { return gridLines; } }
		[PropertyForOptions(0, "Appearance"),
		DevExpress.XtraCharts.Designer.Native.DesignerDisplayName("ShowMajorGridLines"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool Visible {
			get { return GridLines.Visible; }
			set { SetProperty("Visible", value); }
		}
		[PropertyForOptions(0, "Appearance"),
		DevExpress.XtraCharts.Designer.Native.DesignerDisplayName("ShowMinorGridLines"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool MinorVisible {
			get { return GridLines.MinorVisible; }
			set { SetProperty("MinorVisible", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public LineStyleModel LineStyle { get { return lineStyleModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public LineStyleModel MinorLineStyle { get { return minorLineStyleModel; } }
		public Color Color {
			get { return GridLines.Color; }
			set { SetProperty("Color", value); }
		}
		public Color MinorColor {
			get { return GridLines.MinorColor; }
			set { SetProperty("MinorColor", value); }
		}
		public GridLinesModel(GridLines gridLines, CommandManager commandManager)
			: base(commandManager) {
			this.gridLines = gridLines;
			Update();
		}
		protected override void AddChildren() {
			if (lineStyleModel != null)
				Children.Add(lineStyleModel);
			if (minorLineStyleModel != null)
				Children.Add(minorLineStyleModel);
			base.AddChildren();
		}
		public override void Update() {
			this.lineStyleModel = new LineStyleModel(GridLines.LineStyle, CommandManager);
			this.minorLineStyleModel = new LineStyleModel(GridLines.MinorLineStyle, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(GridLinesX))]
	public class GridLinesXModel : GridLinesModel {
		protected new GridLinesX GridLines { get { return (GridLinesX)base.GridLines; } }
		public GridLinesXModel(GridLinesX gridLines, CommandManager commandManager)
			: base(gridLines, commandManager) {
		}
	}
	[ModelOf(typeof(GridLinesY))]
	public class GridLinesYModel : GridLinesModel {
		protected new GridLinesY GridLines { get { return (GridLinesY)base.GridLines; } }
		public GridLinesYModel(GridLinesY gridLines, CommandManager commandManager)
			: base(gridLines, commandManager) {
		}
	}
	[ModelOf(typeof(SecondaryGridLinesX))]
	public class SecondaryGridLinesXModel : GridLinesModel {
		protected new SecondaryGridLinesX GridLines { get { return (SecondaryGridLinesX)base.GridLines; } }
		public SecondaryGridLinesXModel(SecondaryGridLinesX gridLines, CommandManager commandManager)
			: base(gridLines, commandManager) {
		}
	}
	[ModelOf(typeof(SecondaryGridLinesY))]
	public class SecondaryGridLinesYModel : GridLinesModel {
		protected new SecondaryGridLinesY GridLines { get { return (SecondaryGridLinesY)base.GridLines; } }
		public SecondaryGridLinesYModel(SecondaryGridLinesY gridLines, CommandManager commandManager)
			: base(gridLines, commandManager) {
		}
	}
	[ModelOf(typeof(RadarGridLinesX))]
	public class RadarGridLinesXModel : GridLinesModel {
		protected new RadarGridLinesX GridLines { get { return (RadarGridLinesX)base.GridLines; } }
		public RadarGridLinesXModel(RadarGridLinesX gridLines, CommandManager commandManager)
			: base(gridLines, commandManager) {
		}
	}
	[ModelOf(typeof(RadarGridLinesY))]
	public class RadarGridLinesYModel : GridLinesModel {
		protected new RadarGridLinesY GridLines { get { return (RadarGridLinesY)base.GridLines; } }
		public RadarGridLinesYModel(RadarGridLinesY gridLines, CommandManager commandManager)
			: base(gridLines, commandManager) {
		}
	}
	public abstract class TickmarksBaseModel : DesignerChartElementModelBase {
		readonly TickmarksBase tickmarks;
		protected TickmarksBase Tickmarks { get { return tickmarks; } }
		protected internal override ChartElement ChartElement { get { return tickmarks; } }
		[PropertyForOptions(0, "Appearance"),		
		DevExpress.XtraCharts.Designer.Native.DesignerDisplayName("ShowMajorTickmarks"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool Visible {
			get { return Tickmarks.Visible; }
			set { SetProperty("Visible", value); }
		}
		[PropertyForOptions(0, "Appearance"),		
		DevExpress.XtraCharts.Designer.Native.DesignerDisplayName("ShowMinorTickmarks"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool MinorVisible {
			get { return Tickmarks.MinorVisible; }
			set { SetProperty("MinorVisible", value); }
		}
		[TypeConverter(typeof(BooleanTypeConverter))]
		public bool CrossAxis {
			get { return Tickmarks.CrossAxis; }
			set { SetProperty("CrossAxis", value); }
		}
		public int Thickness {
			get { return Tickmarks.Thickness; }
			set { SetProperty("Thickness", value); }
		}
		public int MinorThickness {
			get { return Tickmarks.MinorThickness; }
			set { SetProperty("MinorThickness", value); }
		}
		public int Length {
			get { return Tickmarks.Length; }
			set { SetProperty("Length", value); }
		}
		public int MinorLength {
			get { return Tickmarks.MinorLength; }
			set { SetProperty("MinorLength", value); }
		}
		public TickmarksBaseModel(TickmarksBase tickmarks, CommandManager commandManager)
			: base(commandManager) {
			this.tickmarks = tickmarks;
		}
	}
	[ModelOf(typeof(Tickmarks))]
	public class TickmarksModel : TickmarksBaseModel {
		protected new Tickmarks Tickmarks { get { return (Tickmarks)base.Tickmarks; } }
		public TickmarksModel(Tickmarks tickmarks, CommandManager commandManager)
			: base(tickmarks, commandManager) {
		}
	}
	[ModelOf(typeof(TickmarksX))]
	public class TickmarksXModel : TickmarksModel {
		protected new TickmarksX Tickmarks { get { return (TickmarksX)base.Tickmarks; } }
		public TickmarksXModel(TickmarksX tickmarks, CommandManager commandManager)
			: base(tickmarks, commandManager) {
		}
	}
	[ModelOf(typeof(TickmarksY))]
	public class TickmarksYModel : TickmarksModel {
		protected new TickmarksY Tickmarks { get { return (TickmarksY)base.Tickmarks; } }
		public TickmarksYModel(TickmarksY tickmarks, CommandManager commandManager)
			: base(tickmarks, commandManager) {
		}
	}
	[ModelOf(typeof(RadarTickmarksY))]
	public class RadarTickmarksYModel : TickmarksBaseModel {
		protected new RadarTickmarksY Tickmarks { get { return (RadarTickmarksY)base.Tickmarks; } }
		public RadarTickmarksYModel(RadarTickmarksY tickmarks, CommandManager commandManager)
			: base(tickmarks, commandManager) {
		}
	}
	[
	ModelOf(typeof(Strip)),
	HasOptionsControl]
	public class StripModel : ChartElementNamedModel, ISupportModelVisibility {
		RectangleFillStyleModel fillStyleModel;
		MinStripLimitModel minLimitModel;
		MaxStripLimitModel maxLimitModel;
		protected Strip Strip { get { return (Strip)ChartElementNamed; } }
		protected internal override bool HasOptionsControl { get { return true; } }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.StripKey; } }
		[PropertyForOptions,
		Category("Appearance"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool Visible {
			get { return Strip.Visible; }
			set { SetProperty("Visible", value); }
		}
		[PropertyForOptions("Appearance"),
		Category("Appearance")]
		public Color Color {
			get { return Strip.Color; }
			set { SetProperty("Color", value); }
		}
		[PropertyForOptions("Behavior"),
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowInLegend {
			get { return Strip.ShowInLegend; }
			set { SetProperty("ShowInLegend", value); }
		}
		[
		PropertyForOptions("Behavior"),
		DependentUpon("ShowInLegend"),
		Category("Behavior")
		]
		public string LegendText {
			get { return Strip.LegendText; }
			set { SetProperty("LegendText", value); }
		}
		[PropertyForOptions("Behavior"),
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowAxisLabel {
			get { return Strip.ShowAxisLabel; }
			set { SetProperty("ShowAxisLabel", value); }
		}
		[
		PropertyForOptions("Behavior"),
		DependentUpon("ShowAxisLabel"),
		Category("Behavior")
		]
		public string AxisLabelText {
			get { return Strip.AxisLabelText; }
			set { SetProperty("AxisLabelText", value); }
		}
		[PropertyForOptions,
		AllocateToGroup("Min Limit", 0),
		Category("Behavior")]
		public MinStripLimitModel MinLimit { get { return minLimitModel; } }
		[PropertyForOptions,
		AllocateToGroup("Max Limit", 1),
		Category("Behavior")]
		public MaxStripLimitModel MaxLimit { get { return maxLimitModel; } }
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("Appearance"),
		Category("Appearance")]
		public RectangleFillStyleModel FillStyle { get { return fillStyleModel; } }
		[Category("Appearance"), TypeConverter(typeof(BooleanTypeConverter))]
		public bool CheckedInLegend {
			get { return Strip.CheckedInLegend; }
			set { SetProperty("CheckedInLegend", value); }
		}
		[Category("Appearance"), TypeConverter(typeof(BooleanTypeConverter))]
		public bool CheckableInLegend {
			get { return Strip.CheckableInLegend; }
			set { SetProperty("CheckableInLegend", value); }
		}
		public StripModel(Strip strip, CommandManager commandManager)
			: base(strip, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if (fillStyleModel != null)
				Children.Add(fillStyleModel);
			if (minLimitModel != null)
				Children.Add(minLimitModel);
			if (maxLimitModel != null)
				Children.Add(maxLimitModel);
			base.AddChildren();
		}
		public override void Update() {
			this.fillStyleModel = new RectangleFillStyleModel(Strip.FillStyle, CommandManager);
			this.minLimitModel = new MinStripLimitModel(Strip.MinLimit, CommandManager);
			this.maxLimitModel = new MaxStripLimitModel(Strip.MaxLimit, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeElement(this, parentCollection);
		}
	}
	[ModelOf(typeof(ConstantLine)),
	HasOptionsControl]
	public class ConstantLineModel : ChartElementNamedModel, ISupportModelVisibility {
		LineStyleModel lineStyleModel;
		ConstantLineTitleModel titleModel;
		protected ConstantLine ConstantLine { get { return (ConstantLine)ChartElementNamed; } }
		protected internal override bool HasOptionsControl { get { return true; } }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.ConstantLineKey; } }
		[PropertyForOptions,
		Category("Appearance"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool Visible {
			get { return ConstantLine.Visible; }
			set { SetProperty("Visible", value); }
		}
		[PropertyForOptions,
		Category("Behavior"),
		TypeConverter(typeof(AxisValueTypeConverter))]
		public object AxisValue {
			get { return ConstantLine.AxisValue; }
			set { SetProperty("AxisValue", value); }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("Appearance"),
		Category("Appearance")]
		public LineStyleModel LineStyle { get { return lineStyleModel; } }
		[PropertyForOptions("Appearance"),
		Category("Appearance")]
		public Color Color {
			get { return ConstantLine.Color; }
			set { SetProperty("Color", value); }
		}
		[PropertyForOptions("Behavior"),
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowBehind {
			get { return ConstantLine.ShowBehind; }
			set { SetProperty("ShowBehind", value); }
		}
		[PropertyForOptions("Behavior"),
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowInLegend {
			get { return ConstantLine.ShowInLegend; }
			set { SetProperty("ShowInLegend", value); }
		}
		[
		PropertyForOptions("Behavior"),
		DependentUpon("ShowInLegend"),
		Category("Behavior")
		]
		public string LegendText {
			get { return ConstantLine.LegendText; }
			set { SetProperty("LegendText", value); }
		}
		[Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool CheckedInLegend {
			get { return ConstantLine.CheckedInLegend; }
			set { SetProperty("CheckedInLegend", value); }
		}
		[Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool CheckableInLegend {
			get { return ConstantLine.CheckableInLegend; }
			set { SetProperty("CheckableInLegend", value); }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("Title"),
		Category("Elements")]
		public ConstantLineTitleModel Title { get { return titleModel; } }
		public ConstantLineModel(ConstantLine constantLine, CommandManager commandManager)
			: base(constantLine, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if (lineStyleModel != null)
				Children.Add(lineStyleModel);
			if (titleModel != null)
				Children.Add(titleModel);
			base.AddChildren();
		}
		public override void Update() {
			this.lineStyleModel = new LineStyleModel(ConstantLine.LineStyle, CommandManager);
			this.titleModel = new ConstantLineTitleModel(ConstantLine.Title, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeElement(this, parentCollection);
		}
	}
	[ModelOf(typeof(ScaleBreak)),
	HasOptionsControl]
	public class ScaleBreakModel : ChartElementNamedModel, ISupportModelVisibility {
		protected ScaleBreak ScaleBreak { get { return (ScaleBreak)ChartElementNamed; } }
		protected internal override bool HasOptionsControl { get { return true; } }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.ScaleBreakKey; } }
		[PropertyForOptions,
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool Visible {
			get { return ScaleBreak.Visible; }
			set { SetProperty("Visible", value); }
		}
		[
		TypeConverter(typeof(ScaleBreakEdgeTypeConverter)),
		PropertyForOptions,
		Category("Behavior")]
		public object Edge1 {
			get { return ScaleBreak.Edge1; }
			set { SetProperty("Edge1", value); }
		}
		[
		TypeConverter(typeof(ScaleBreakEdgeTypeConverter)),
		PropertyForOptions,
		Category("Behavior")]
		public object Edge2 {
			get { return ScaleBreak.Edge2; }
			set { SetProperty("Edge2", value); }
		}
		public ScaleBreakModel(ScaleBreak scaleBreak, CommandManager commandManager)
			: base(scaleBreak, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			base.AddChildren();
		}
		public override void Update() {
			ClearChildren();
			AddChildren();
			base.Update();
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeElement(this, parentCollection);
		}
	}
	[ModelOf(typeof(AutoScaleBreaks))]
	public class AutoScaleBreaksModel : DesignerChartElementModelBase {
		readonly AutoScaleBreaks autoScaleBreaks;
		protected AutoScaleBreaks AutoScaleBreaks { get { return autoScaleBreaks; } }
		protected internal override ChartElement ChartElement { get { return autoScaleBreaks; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool Enabled {
			get { return AutoScaleBreaks.Enabled; }
			set { SetProperty("Enabled", value); }
		}
		[PropertyForOptions]
		public int MaxCount {
			get { return AutoScaleBreaks.MaxCount; }
			set { SetProperty("MaxCount", value); }
		}
		public AutoScaleBreaksModel(AutoScaleBreaks autoScaleBreaks, CommandManager commandManager)
			: base(commandManager) {
			this.autoScaleBreaks = autoScaleBreaks;
		}
	}
	[ModelOf(typeof(KnownDate))]
	public class KnownDateModel : ChartElementNamedModel {
		protected KnownDate KnownDate { get { return (KnownDate)ChartElementNamed; } }
		[PropertyForOptions]
		public DateTime Date {
			get { return KnownDate.Date; }
			set { SetProperty("Date", value); }
		}
		public KnownDateModel(KnownDate knownDate, CommandManager commandManager)
			: base(knownDate, commandManager) {
		}
	}
}
