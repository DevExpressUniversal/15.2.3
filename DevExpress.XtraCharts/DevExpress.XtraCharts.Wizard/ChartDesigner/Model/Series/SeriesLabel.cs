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
using DevExpress.XtraCharts.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using DevExpress.XtraCharts.Design;
using DevExpress.Utils.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	[HasOptionsControl, TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class SeriesLabelBaseModel : DesignerChartElementModelBase, ISupportModelVisibility {
		readonly SeriesLabelBase seriesLabel;
		ShadowModel shadowModel;
		RectangularBorderModel borderModel;
		RectangleFillStyleModel fillStyleModel;
		LineStyleModel lineStyleModel;
		protected SeriesLabelBase SeriesLabel { get { return seriesLabel; } }
		protected internal override bool HasOptionsControl { get { return true; } }
		protected internal override ChartElement ChartElement { get { return seriesLabel; } }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.SeriesLabelKey; } }
		internal override string ChartTreeText { get { return "Label"; } }
		internal DesignerSeriesModelBase SeriesModel { get; set; }
		[
		Browsable(false),
		DevExpress.XtraCharts.Designer.Native.DesignerDisplayName("Visibility"),
		PropertyForOptions(-1, "General", -1)]
		public DefaultBoolean LabelsVisibility {
			get { return SeriesModel.LabelsVisibility; }
			set { SeriesModel.LabelsVisibility = value; }
		}
		[Category("Behavior")]
		public int MaxWidth {
			get { return SeriesLabel.MaxWidth; }
			set { SetProperty("MaxWidth", value); }
		}
		[Category("Behavior")]
		public int MaxLineCount {
			get { return SeriesLabel.MaxLineCount; }
			set { SetProperty("MaxLineCount", value); }
		}
		[
		PropertyForOptions("Appearance"),
		DependentUpon("LabelsVisibility"),
		Category("Appearance")]
		public Color TextColor {
			get { return SeriesLabel.TextColor; }
			set { SetProperty("TextColor", value); }
		}
		[
		PropertyForOptions("Appearance"),
		DependentUpon("LabelsVisibility"),
		Category("Appearance")]
		public Color BackColor {
			get { return SeriesLabel.BackColor; }
			set { SetProperty("BackColor", value); }
		}
		[Category("Appearance"), TypeConverter(typeof(FontTypeConverter))]
		public Font Font {
			get { return SeriesLabel.Font; }
			set { SetProperty("Font", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public ShadowModel Shadow { get { return shadowModel; } }
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("Border"),
		DependentUpon("LabelsVisibility"),
		Category("Appearance")]
		public RectangularBorderModel Border { get { return borderModel; } }
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("Appearance"),
		DependentUpon("LabelsVisibility"),
		Category("Appearance")]
		public RectangleFillStyleModel FillStyle { get { return fillStyleModel; } }
		[Category("Appearance"), TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean EnableAntialiasing {
			get { return SeriesLabel.EnableAntialiasing; }
			set { SetProperty("EnableAntialiasing", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("LabelsVisibility"),
		Category("Appearance"),
		TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean LineVisibility {
			get { return SeriesLabel.LineVisibility; }
			set { SetProperty("LineVisibility", value); }
		}
		[
		PropertyForOptions("Text Options", 0),
		UseEditor(typeof(PatternControl), typeof(PatternControlAdapter)),
		DependentUpon("LabelsVisibility"),
		Editor(typeof(SeriesLabelTextModelPatternEditor), typeof(UITypeEditor)),
		Category("Behavior")
		]
		public string TextPattern {
			get { return SeriesLabel.TextPattern; }
			set { SetProperty("TextPattern", value); }
		}
		[
		PropertyForOptions("Text Options", 0),
		DependentUpon("LabelsVisibility"),
		Category("Behavior")]
		public TextOrientation TextOrientation {
			get { return SeriesLabel.TextOrientation; }
			set { SetProperty("TextOrientation", value); }
		}
		[
		PropertyForOptions("Text Options", 0),
		DependentUpon("LabelsVisibility"),
		Category("Behavior"),
		TypeConverter(typeof(StringAlignmentTypeConvertor))]
		public StringAlignment TextAlignment {
			get { return SeriesLabel.TextAlignment; }
			set { SetProperty("TextAlignment", value); }
		}
		[Category("Behavior"), TypeConverter(typeof(ResolveOverlappingModeTypeConverter))]
		public ResolveOverlappingMode ResolveOverlappingMode {
			get { return SeriesLabel.ResolveOverlappingMode; }
			set { SetProperty("ResolveOverlappingMode", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("LabelsVisibility"),
		Category("Appearance")]
		public int LineLength {
			get { return SeriesLabel.LineLength; }
			set { SetProperty("LineLength", value); }
		}
		[Category("Appearance")]
		public Color LineColor {
			get { return SeriesLabel.LineColor; }
			set { SetProperty("LineColor", value); }
		}
		[Category("Behavior")]
		public int ResolveOverlappingMinIndent {
			get { return SeriesLabel.ResolveOverlappingMinIndent; }
			set { SetProperty("ResolveOverlappingMinIndent", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public LineStyleModel LineStyle { get { return lineStyleModel; } }
		public SeriesLabelBaseModel(SeriesLabelBase seriesLabel, CommandManager commandManager)
			: base(commandManager) {
			this.seriesLabel = seriesLabel;
			Update();
		}
		#region ISupportModelVisibility implementation
		bool ISupportModelVisibility.Visible {
			get { return SeriesModel.SeriesBase.ActualLabelsVisibility; }
			set { SeriesModel.LabelsVisibility = value ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		#endregion
		protected override void ProcessMessage(ViewMessage message) {
			if (message.SimpleName && message.Name == "LabelsVisibility")
				this.LabelsVisibility = (DefaultBoolean)message.Value;
			else
				base.ProcessMessage(message);
		}
		protected override void AddChildren() {
			if (shadowModel != null)
				Children.Add(shadowModel);
			if (borderModel != null)
				Children.Add(borderModel);
			if (fillStyleModel != null)
				Children.Add(fillStyleModel);
			if (lineStyleModel != null)
				Children.Add(lineStyleModel);
			base.AddChildren();
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeSeriesLabelElement(this);
		}
		public override void Update() {
			this.shadowModel = new ShadowModel(SeriesLabel.Shadow, CommandManager);
			this.borderModel = (RectangularBorderModel)ModelHelper.CreateBorderModelInstance(SeriesLabel.Border, CommandManager);
			this.fillStyleModel = new RectangleFillStyleModel(SeriesLabel.FillStyle, CommandManager);
			this.lineStyleModel = new LineStyleModel(SeriesLabel.LineStyle, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(RangeAreaSeriesLabel)), TypeConverter(typeof(RangeAreaSeriesLabelTypeConverter))]
	public class RangeAreaSeriesLabelModel : SeriesLabelBaseModel {
		protected new RangeAreaSeriesLabel SeriesLabel { get { return (RangeAreaSeriesLabel)base.SeriesLabel; } }
		[
		PropertyForOptions,
		DependentUpon("LabelsVisibility"),
		Category("Behavior")]
		public RangeAreaLabelKind Kind {
			get { return SeriesLabel.Kind; }
			set { SetProperty("Kind", value); }
		}
		[Category("Behavior")]
		public int MinValueAngle {
			get { return SeriesLabel.MinValueAngle; }
			set { SetProperty("MinValueAngle", value); }
		}
		[Category("Behavior")]
		public int MaxValueAngle {
			get { return SeriesLabel.MaxValueAngle; }
			set { SetProperty("MaxValueAngle", value); }
		}
		public RangeAreaSeriesLabelModel(RangeAreaSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(RangeArea3DSeriesLabel)), TypeConverter(typeof(RangeArea3DSeriesLabelTypeConverter))]
	public class RangeArea3DSeriesLabelModel : RangeAreaSeriesLabelModel {
		protected new RangeArea3DSeriesLabel SeriesLabel { get { return (RangeArea3DSeriesLabel)base.SeriesLabel; } }
		[Browsable(false)]
		public new int MinValueAngle { get; set; }
		[Browsable(false)]
		public new int MaxValueAngle { get; set; }
		[Browsable(false)]
		public new ShadowModel Shadow { get; set; }
		public RangeArea3DSeriesLabelModel(RangeArea3DSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(PointSeriesLabel)), TypeConverter(typeof(PointSeriesLabelTypeConverter))]
	public class PointSeriesLabelModel : SeriesLabelBaseModel {
		protected new PointSeriesLabel SeriesLabel { get { return (PointSeriesLabel)base.SeriesLabel; } }
		[Category("Behavior")]
		public int Angle {
			get { return SeriesLabel.Angle; }
			set { SetProperty("Angle", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("LabelsVisibility"),
		Category("Behavior")]
		public PointLabelPosition Position {
			get { return SeriesLabel.Position; }
			set { SetProperty("Position", value); }
		}
		public PointSeriesLabelModel(PointSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(BubbleSeriesLabel)), TypeConverter(typeof(BubbleSeriesLabelTypeConverter))]
	public class BubbleSeriesLabelModel : PointSeriesLabelModel {
		protected new BubbleSeriesLabel SeriesLabel { get { return (BubbleSeriesLabel)base.SeriesLabel; } }
		[Browsable(false)]
		public new int LineLength { get; set; }
		[
		PropertyForOptions("Appearance"),
		DependentUpon("LabelsVisibility"),
		Category("Appearance")]
		public double IndentFromMarker {
			get { return SeriesLabel.IndentFromMarker; }
			set { SetProperty("IndentFromMarker", value); }
		}
		public BubbleSeriesLabelModel(BubbleSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(FunnelSeriesLabel)), TypeConverter(typeof(FunnelSeriesLabelTypeConverter))]
	public class FunnelSeriesLabelModel : SeriesLabelBaseModel {
		protected new FunnelSeriesLabel SeriesLabel { get { return (FunnelSeriesLabel)base.SeriesLabel; } }
		[Browsable(false)]
		public new TextOrientation TextOrientation { get; set; }
		[
		PropertyForOptions,
		DependentUpon("LabelsVisibility"),
		Category("Behavior")]
		public FunnelSeriesLabelPosition Position {
			get { return SeriesLabel.Position; }
			set { SetProperty("Position", value); }
		}
		public FunnelSeriesLabelModel(FunnelSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(Funnel3DSeriesLabel)), TypeConverter(typeof(Funnel3DSeriesLabelTypeConverter))]
	public class Funnel3DSeriesLabelModel : FunnelSeriesLabelModel {
		protected new Funnel3DSeriesLabel SeriesLabel { get { return (Funnel3DSeriesLabel)base.SeriesLabel; } }
		[Browsable(false)]
		public new ShadowModel Shadow { get; set; }
		public Funnel3DSeriesLabelModel(Funnel3DSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(Line3DSeriesLabel)), TypeConverter(typeof(Line3DSeriesLabelTypeConverter))]
	public class Line3DSeriesLabelModel : SeriesLabelBaseModel {
		protected new Line3DSeriesLabel SeriesLabel { get { return (Line3DSeriesLabel)base.SeriesLabel; } }
		[Browsable(false)]
		public new ShadowModel Shadow { get; set; }
		public Line3DSeriesLabelModel(Line3DSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(StackedLine3DSeriesLabel)), TypeConverter(typeof(StackedLine3DSeriesLabelTypeConverter))]
	public class StackedLine3DSeriesLabelModel : Line3DSeriesLabelModel {
		protected new StackedLine3DSeriesLabel SeriesLabel { get { return (StackedLine3DSeriesLabel)base.SeriesLabel; } }
		public StackedLine3DSeriesLabelModel(StackedLine3DSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(StackedLineSeriesLabel)), TypeConverter(typeof(StackedLineSeriesLabelTypeConverter))]
	public class StackedLineSeriesLabelModel : PointSeriesLabelModel {
		protected new StackedLineSeriesLabel SeriesLabel { get { return (StackedLineSeriesLabel)base.SeriesLabel; } }
		public StackedLineSeriesLabelModel(StackedLineSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(PieSeriesLabel)), TypeConverter(typeof(PieSeriesLabelTypeConverter))]
	public class PieSeriesLabelModel : SeriesLabelBaseModel {
		protected new PieSeriesLabel SeriesLabel { get { return (PieSeriesLabel)base.SeriesLabel; } }
		[Browsable(false)]
		public new TextOrientation TextOrientation { get; set; }
		[
		PropertyForOptions,
		DependentUpon("LabelsVisibility"),
		Category("Behavior"),
		TypeConverter(typeof(PieSeriesLabelPositionTypeConverter))]
		public PieSeriesLabelPosition Position {
			get { return SeriesLabel.Position; }
			set { SetProperty("Position", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("LabelsVisibility"),
		Category("Behavior")]
		public int ColumnIndent {
			get { return SeriesLabel.ColumnIndent; }
			set { SetProperty("ColumnIndent", value); }
		}
		public PieSeriesLabelModel(PieSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(DoughnutSeriesLabel)), TypeConverter(typeof(DoughnutSeriesLabelTypeConverter))]
	public class DoughnutSeriesLabelModel : PieSeriesLabelModel {
		protected new DoughnutSeriesLabel SeriesLabel { get { return (DoughnutSeriesLabel)base.SeriesLabel; } }
		public DoughnutSeriesLabelModel(DoughnutSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(NestedDoughnutSeriesLabel)), TypeConverter(typeof(NestedDoughnutSeriesLabelTypeConverter))]
	public class NestedDoughnutSeriesLabelModel : DoughnutSeriesLabelModel {
		protected new NestedDoughnutSeriesLabel SeriesLabel { get { return (NestedDoughnutSeriesLabel)base.SeriesLabel; } }
		public NestedDoughnutSeriesLabelModel(NestedDoughnutSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(Area3DSeriesLabel)), TypeConverter(typeof(Area3DSeriesLabelTypeConverter))]
	public class Area3DSeriesLabelModel : Line3DSeriesLabelModel {
		protected new Area3DSeriesLabel SeriesLabel { get { return (Area3DSeriesLabel)base.SeriesLabel; } }
		public Area3DSeriesLabelModel(Area3DSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	public abstract class BarSeriesLabelModel : SeriesLabelBaseModel {
		protected new BarSeriesLabel SeriesLabel { get { return (BarSeriesLabel)base.SeriesLabel; } }
		[Category("Behavior"), TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowForZeroValues {
			get { return SeriesLabel.ShowForZeroValues; }
			set { SetProperty("ShowForZeroValues", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("LabelsVisibility"),
		Category("Behavior"),
		TypeConverter(typeof(BarSeriesLabelPositionTypeConverter))]
		public BarSeriesLabelPosition Position {
			get { return SeriesLabel.Position; }
			set { SetProperty("Position", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("LabelsVisibility"),
		Category("Behavior")]
		public int Indent {
			get { return SeriesLabel.Indent; }
			set { SetProperty("Indent", value); }
		}
		public BarSeriesLabelModel(BarSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(Bar3DSeriesLabel)), TypeConverter(typeof(Bar3DSeriesLabelTypeConverter))]
	public class Bar3DSeriesLabelModel : BarSeriesLabelModel {
		protected new Bar3DSeriesLabel SeriesLabel { get { return (Bar3DSeriesLabel)base.SeriesLabel; } }
		[Browsable(false)]
		public new BarSeriesLabelPosition Position { get; set; }
		[Browsable(false)]
		public new int Indent { get; set; }
		[Browsable(false)]
		public new ShadowModel Shadow { get; set; }
		public Bar3DSeriesLabelModel(Bar3DSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(Pie3DSeriesLabel)), TypeConverter(typeof(Pie3DSeriesLabelTypeConverter))]
	public class Pie3DSeriesLabelModel : PieSeriesLabelModel {
		protected new Pie3DSeriesLabel SeriesLabel { get { return (Pie3DSeriesLabel)base.SeriesLabel; } }
		[Browsable(false)]
		public new ShadowModel Shadow { get; set; }
		public Pie3DSeriesLabelModel(Pie3DSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(Doughnut3DSeriesLabel)), TypeConverter(typeof(Doughnut3DSeriesLabelTypeConverter))]
	public class Doughnut3DSeriesLabelModel : Pie3DSeriesLabelModel {
		protected new Doughnut3DSeriesLabel SeriesLabel { get { return (Doughnut3DSeriesLabel)base.SeriesLabel; } }
		public Doughnut3DSeriesLabelModel(Doughnut3DSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(StackedArea3DSeriesLabel)), TypeConverter(typeof(StackedArea3DSeriesLabelTypeConverter))]
	public class StackedArea3DSeriesLabelModel : SeriesLabelBaseModel {
		protected new StackedArea3DSeriesLabel SeriesLabel { get { return (StackedArea3DSeriesLabel)base.SeriesLabel; } }
		[Browsable(false)]
		public new ShadowModel Shadow { get; set; }
		[Browsable(false)]
		public new int LineLength { get; set; }
		[Browsable(false)]
		public new Color LineColor { get; set; }
		[Browsable(false)]
		public new LineStyleModel LineStyle { get; set; }
		public StackedArea3DSeriesLabelModel(StackedArea3DSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(FullStackedArea3DSeriesLabel)), TypeConverter(typeof(FullStackedArea3DSeriesLabelTypeConverter))]
	public class FullStackedArea3DSeriesLabelModel : StackedArea3DSeriesLabelModel {
		protected new FullStackedArea3DSeriesLabel SeriesLabel { get { return (FullStackedArea3DSeriesLabel)base.SeriesLabel; } }
		public FullStackedArea3DSeriesLabelModel(FullStackedArea3DSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(FullStackedAreaSeriesLabel)), TypeConverter(typeof(FullStackedAreaSeriesLabelTypeConverter))]
	public class FullStackedAreaSeriesLabelModel : SeriesLabelBaseModel {
		protected new FullStackedAreaSeriesLabel SeriesLabel { get { return (FullStackedAreaSeriesLabel)base.SeriesLabel; } }
		[Browsable(false)]
		public new int LineLength { get; set; }
		[Browsable(false)]
		public new Color LineColor { get; set; }
		[Browsable(false)]
		public new LineStyleModel LineStyle { get; set; }
		public FullStackedAreaSeriesLabelModel(FullStackedAreaSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(RadarPointSeriesLabel)), TypeConverter(typeof(RadarPointSeriesLabelTypeConverter))]
	public class RadarPointSeriesLabelModel : PointSeriesLabelModel {
		protected new RadarPointSeriesLabel SeriesLabel { get { return (RadarPointSeriesLabel)base.SeriesLabel; } }
		public RadarPointSeriesLabelModel(RadarPointSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(RangeBarSeriesLabel)), TypeConverter(typeof(RangeBarSeriesLabelTypeConverter))]
	public class RangeBarSeriesLabelModel : SeriesLabelBaseModel {
		protected new RangeBarSeriesLabel SeriesLabel { get { return (RangeBarSeriesLabel)base.SeriesLabel; } }
		[Browsable(false)]
		public new int LineLength { get; set; }
		[Browsable(false)]
		public new Color LineColor { get; set; }
		[Browsable(false)]
		public new LineStyleModel LineStyle { get; set; }
		[
		PropertyForOptions,
		DependentUpon("LabelsVisibility"),
		Category("Behavior")]
		public RangeBarLabelKind Kind {
			get { return SeriesLabel.Kind; }
			set { SetProperty("Kind", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("LabelsVisibility"),
		Category("Behavior")]
		public RangeBarLabelPosition Position {
			get { return SeriesLabel.Position; }
			set { SetProperty("Position", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("LabelsVisibility"),
		Category("Behavior")]
		public int Indent {
			get { return SeriesLabel.Indent; }
			set { SetProperty("Indent", value); }
		}
		public RangeBarSeriesLabelModel(RangeBarSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(SideBySideBarSeriesLabel)), TypeConverter(typeof(SideBySideBarSeriesLabelTypeConverter))]
	public class SideBySideBarSeriesLabelModel : BarSeriesLabelModel {
		protected new SideBySideBarSeriesLabel SeriesLabel { get { return (SideBySideBarSeriesLabel)base.SeriesLabel; } }
		public SideBySideBarSeriesLabelModel(SideBySideBarSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(StackedBar3DSeriesLabel)), TypeConverter(typeof(StackedBar3DSeriesLabelTypeConverter))]
	public class StackedBar3DSeriesLabelModel : BarSeriesLabelModel {
		protected new StackedBar3DSeriesLabel SeriesLabel { get { return (StackedBar3DSeriesLabel)base.SeriesLabel; } }
		[Browsable(false)]
		public new BarSeriesLabelPosition Position { get; set; }
		[Browsable(false)]
		public new int Indent { get; set; }
		[Browsable(false)]
		public new ShadowModel Shadow { get; set; }
		[Browsable(false)]
		public new int LineLength { get; set; }
		[Browsable(false)]
		public new Color LineColor { get; set; }
		[Browsable(false)]
		public new LineStyleModel LineStyle { get; set; }
		public StackedBar3DSeriesLabelModel(StackedBar3DSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(FullStackedBar3DSeriesLabel)), TypeConverter(typeof(FullStackedBar3DSeriesLabelTypeConverter))]
	public class FullStackedBar3DSeriesLabelModel : StackedBar3DSeriesLabelModel {
		protected new FullStackedBar3DSeriesLabel SeriesLabel { get { return (FullStackedBar3DSeriesLabel)base.SeriesLabel; } }
		public FullStackedBar3DSeriesLabelModel(FullStackedBar3DSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(StackedBarSeriesLabel)), TypeConverter(typeof(StackedBarSeriesLabelTypeConverter))]
	public class StackedBarSeriesLabelModel : BarSeriesLabelModel {
		protected new StackedBarSeriesLabel SeriesLabel { get { return (StackedBarSeriesLabel)base.SeriesLabel; } }
		[Browsable(false)]
		public new int LineLength { get; set; }
		[Browsable(false)]
		public new Color LineColor { get; set; }
		[Browsable(false)]
		public new LineStyleModel LineStyle { get; set; }
		public StackedBarSeriesLabelModel(StackedBarSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(FullStackedBarSeriesLabel)), TypeConverter(typeof(FullStackedBarSeriesLabelTypeConverter))]
	public class FullStackedBarSeriesLabelModel : StackedBarSeriesLabelModel {
		protected new FullStackedBarSeriesLabel SeriesLabel { get { return (FullStackedBarSeriesLabel)base.SeriesLabel; } }
		public FullStackedBarSeriesLabelModel(FullStackedBarSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(StockSeriesLabel)), TypeConverter(typeof(StockSeriesLabelTypeConverter))]
	public class StockSeriesLabelModel : SeriesLabelBaseModel {
		protected new StockSeriesLabel SeriesLabel { get { return (StockSeriesLabel)base.SeriesLabel; } }
		[Browsable(false)]
		public new int LineLength { get; set; }
		[Browsable(false)]
		public new Color LineColor { get; set; }
		[Browsable(false)]
		public new LineStyleModel LineStyle { get; set; }
		public StockSeriesLabelModel(StockSeriesLabel seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
}
