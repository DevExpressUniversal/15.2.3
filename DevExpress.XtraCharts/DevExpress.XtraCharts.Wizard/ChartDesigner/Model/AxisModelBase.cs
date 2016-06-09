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

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	[HasOptionsControl, TypeConverter(typeof(AxisBaseTypeConverter))]
	public abstract class AxisModelBase : ChartElementNamedModel {
		VisualRangeModel visualRangeModel;
		WholeRangeModel wholeRangeModel;
		GridLinesModel gridLinesModel;
		NumericScaleOptionsModel numericScaleOptionsModel;
		DateTimeScaleOptionsModel dateTimeScaleOptionsModel;
		protected internal override bool HasOptionsControl { get { return true; } }
		internal AxisBase Axis { get { return (AxisBase)base.ChartElementNamed; } }
		[PropertyForOptions(1, "General", 0),
		Category("Appearance"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool Interlaced {
			get { return Axis.Interlaced; }
			set { SetProperty("Interlaced", value); }
		}
		[PropertyForOptions(4, "General", 0),
		DependentUpon("ScaleType"),
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool Logarithmic {
			get { return Axis.Logarithmic; }
			set { SetProperty("Logarithmic", value); }
		}
		[Category("Misc")]
		public int MinorCount {
			get { return Axis.MinorCount; }
			set { SetProperty("MinorCount", value); }
		}
		[Browsable(false)]
		public ActualScaleType ScaleType {
			get { return ((IAxisData)Axis).AxisScaleTypeMap.ScaleType; }
		}
		[Category("Appearance")]
		public Color InterlacedColor {
			get { return Axis.InterlacedColor; }
			set { SetProperty("InterlacedColor", value); }
		}
		[PropertyForOptions(4, "General", 0),
		DependentUpon("Logarithmic"),
		Category("Behavior")
		]
		public double LogarithmicBase {
			get { return Axis.LogarithmicBase; }
			set { SetProperty("LogarithmicBase", value); }
		}
		[PropertyForOptions,
		AllocateToGroup("Visual Range"),
		Category("Behavior")]
		public VisualRangeModel VisualRange { get { return visualRangeModel; } }
		[PropertyForOptions,
		AllocateToGroup("Whole Range"),
		Category("Behavior")]
		public WholeRangeModel WholeRange { get { return wholeRangeModel; } }
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions, AllocateToGroup("General"),
		Category("Elements")]
		public GridLinesModel GridLines { get { return gridLinesModel; } }
		[Category("Behavior")]
		public NumericScaleOptionsModel NumericScaleOptions { get { return numericScaleOptionsModel; } }
		[Category("Behavior")]
		public DateTimeScaleOptionsModel DateTimeScaleOptions { get { return dateTimeScaleOptionsModel; } }
		public AxisModelBase(AxisBase axis, CommandManager commandManager)
			: base(axis, commandManager) {
		}
		protected override void AddChildren() {
			if (visualRangeModel != null)
				Children.Add(visualRangeModel);
			if (wholeRangeModel != null)
				Children.Add(wholeRangeModel);
			if (gridLinesModel != null)
				Children.Add(gridLinesModel);
			if (dateTimeScaleOptionsModel != null)
				Children.Add(dateTimeScaleOptionsModel);
			if (numericScaleOptionsModel != null)
				Children.Add(numericScaleOptionsModel);
			base.AddChildren();
		}
		protected override void ProcessMessage(ViewMessage message) {
			if (!message.SimpleName) {
				if (message.Name == "MinValue" || message.Name == "MaxValue") {
					object nativeValue = EditorValueToNative(message.Value);
					base.ProcessMessage(new ViewMessage(message.FullName, nativeValue));
					return;
				}
			}
			base.ProcessMessage(message);
		}
		public override void Update() {
			this.visualRangeModel = new VisualRangeModel(Axis.VisualRange, CommandManager);
			this.wholeRangeModel = new WholeRangeModel(Axis.WholeRange, CommandManager);
			this.gridLinesModel = new GridLinesModel(Axis.GridLines, CommandManager);
			this.dateTimeScaleOptionsModel = new DateTimeScaleOptionsModel(Axis.DateTimeScaleOptions, CommandManager);
			this.numericScaleOptionsModel = new NumericScaleOptionsModel(Axis.NumericScaleOptions, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeElement(this);
		}
		public object EditorValueToNative(object editorValue) {
			var culture = CultureInfo.CurrentCulture;
			AxisScaleTypeMap map = ((IAxisData)Axis).AxisScaleTypeMap;
			object nativeValue = map.ConvertValue(editorValue, culture);
			return nativeValue;
		}
	}
	public abstract class Axis2DModel : AxisModelBase, ISupportModelVisibility {
		RectangleFillStyleModel interlacedFillStyleModel;
		TickmarksModel tickmarksModel;
		AxisTitleModel titleModel;
		StripCollectionModel stripsModel;
		ConstantLineCollectionModel constantLinesModel;
		AxisLabelModel labelModel;
		CrosshairAxisLabelOptionsModel crosshairAxisLabelOptionsModel;
		readonly CustomAxisLabelCollectionModel customLabelsModel;
		protected new Axis2D Axis { get { return (Axis2D)base.Axis; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Behavior")]
		public RectangleFillStyleModel InterlacedFillStyle { get { return interlacedFillStyleModel; } }
		[PropertyForOptions(-1, "General", 0),
		Category("Behavior"),
		TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean Visibility {
			get { return Axis.Visibility; }
			set { SetProperty("Visibility", value); }
		}
		[PropertyForOptions(-1, "General", 0),
		Category("Behavior"),
		TypeConverter(typeof(AxisAlignmentTypeConverter))]
		public AxisAlignment Alignment {
			get { return Axis.Alignment; }
			set { SetProperty("Alignment", value); }
		}
		[Category("Appearance")]
		public int Thickness {
			get { return Axis.Thickness; }
			set { SetProperty("Thickness", value); }
		}
		[Category("Appearance")]
		public Color Color {
			get { return Axis.Color; }
			set { SetProperty("Color", value); }
		}
		[Editor(typeof(AxisVisibilityInPanesModelEditor), typeof(UITypeEditor)),
		Category("Behavior")]
		public IDictionary VisibilityInPanes {
			get { return Axis.VisibilityInPanes; }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("General"),
		Category("Elements")]
		public TickmarksModel Tickmarks { get { return tickmarksModel; } }
		[TypeConverter(typeof(AxisTitleTypeConverter)),
		PropertyForOptions,
		AllocateToGroup("Title"),
		Category("Elements")]
		public AxisTitleModel Title { get { return titleModel; } }
		[Browsable(false)]
		public StripCollectionModel Strips { get { return stripsModel; } }
		[Browsable(false)]
		public ConstantLineCollectionModel ConstantLines { get { return constantLinesModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("General"),
		Category("Elements")]
		public AxisLabelModel Label { get { return labelModel; } }
		[Category("Elements"), TypeConverter(typeof(EnumTypeConverter))]
		public AxisLabelVisibilityMode LabelVisibilityMode {
			get { return Axis.LabelVisibilityMode; }
			set { SetProperty("LabelVisibilityMode", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Behavior")]
		public CrosshairAxisLabelOptionsModel CrosshairAxisLabelOptions { get { return crosshairAxisLabelOptionsModel; } }
		[Category("Behavior"),
		Editor(typeof(CustomAxisLabelModelCollectionEditor), typeof(UITypeEditor)),
		TypeConverter(typeof(CollectionTypeConverter))]
		public CustomAxisLabelCollectionModel CustomLabels { get { return customLabelsModel; } }
		public Axis2DModel(Axis2D axis, CommandManager commandManager)
			: base(axis, commandManager) {
			this.customLabelsModel = new CustomAxisLabelCollectionModel(Axis.CustomLabels, CommandManager, null);
			Update();
		}
		#region ISupportModelVisibility implementation
		bool ISupportModelVisibility.Visible {
			get { return Visibility != DefaultBoolean.False; }
			set { Visibility = value ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		#endregion
		protected override void AddChildren() {
			if (interlacedFillStyleModel != null)
				Children.Add(interlacedFillStyleModel);
			if (tickmarksModel != null)
				Children.Add(tickmarksModel);
			if (titleModel != null)
				Children.Add(titleModel);
			if (stripsModel != null)
				Children.Add(stripsModel);
			if (constantLinesModel != null)
				Children.Add(constantLinesModel);
			if (labelModel != null)
				Children.Add(labelModel);
			if (crosshairAxisLabelOptionsModel != null)
				Children.Add(crosshairAxisLabelOptionsModel);
			if (customLabelsModel != null)
				Children.Add(customLabelsModel);
			base.AddChildren();
		}
		public override void Update() {
			this.interlacedFillStyleModel = new RectangleFillStyleModel(Axis.InterlacedFillStyle, CommandManager);
			this.tickmarksModel = new TickmarksModel(Axis.Tickmarks, CommandManager);
			this.titleModel = new AxisTitleModel(Axis.Title, CommandManager);
			this.stripsModel = new StripCollectionModel(Axis.Strips, CommandManager, null);
			this.constantLinesModel = new ConstantLineCollectionModel(Axis.ConstantLines, CommandManager, null);
			this.labelModel = new AxisLabelModel(Axis.Label, CommandManager);
			this.crosshairAxisLabelOptionsModel = new CrosshairAxisLabelOptionsModel(Axis.CrosshairAxisLabelOptions, CommandManager);
			this.customLabelsModel.Update();
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	public abstract class AxisModel : Axis2DModel {
		ScaleBreakCollectionModel scaleBreaksModel;
		ScaleBreakOptionsModel scaleBreakOptionsModel;
		AutoScaleBreaksModel autoScaleBreaksModel;
		protected new Axis Axis { get { return (Axis)base.Axis; } }
		[Category("Behavior"), TypeConverter(typeof(BooleanTypeConverter))]
		public bool Reverse {
			get { return Axis.Reverse; }
			set { SetProperty("Reverse", value); }
		}
		[Browsable(false)]
		public ScaleBreakCollectionModel ScaleBreaks { get { return scaleBreaksModel; } }
		[Category("Behavior")]
		public ScaleBreakOptionsModel ScaleBreakOptions { get { return scaleBreakOptionsModel; } }
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Behavior")]
		public AutoScaleBreaksModel AutoScaleBreaks { get { return autoScaleBreaksModel; } }
		public AxisModel(Axis axis, CommandManager commandManager)
			: base(axis, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if (scaleBreaksModel != null)
				Children.Add(scaleBreaksModel);
			if (scaleBreakOptionsModel != null)
				Children.Add(scaleBreakOptionsModel);
			if (autoScaleBreaksModel != null)
				Children.Add(autoScaleBreaksModel);
			base.AddChildren();
		}
		public override void Update() {
			this.scaleBreaksModel = new ScaleBreakCollectionModel(Axis.ScaleBreaks, CommandManager, null);
			this.scaleBreakOptionsModel = new ScaleBreakOptionsModel(Axis.ScaleBreakOptions, CommandManager);
			this.autoScaleBreaksModel = new AutoScaleBreaksModel(Axis.AutoScaleBreaks, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	public abstract class AxisXBaseModel : AxisModel {
		protected new AxisXBase Axis { get { return (AxisXBase)base.Axis; } }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.AxisXKey; } }
		public AxisXBaseModel(AxisXBase axis, CommandManager commandManager)
			: base(axis, commandManager) {
		}
		public override string ToString() {
			return "Axis X";
		}
	}
	[ModelOf(typeof(AxisX)), TypeConverter(typeof(AxisXTypeConverter))]
	public class AxisXModel : AxisXBaseModel {
		protected new AxisX Axis { get { return (AxisX)base.Axis; } }
		[Browsable(false)]
		public new string Name { get; set; }
		public AxisXModel(AxisX axis, CommandManager commandManager)
			: base(axis, commandManager) {
		}
	}
	public abstract class AxisYBaseModel : AxisModel {
		protected new AxisYBase Axis { get { return (AxisYBase)base.Axis; } }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.AxisYKey; } }
		public AxisYBaseModel(AxisYBase axis, CommandManager commandManager)
			: base(axis, commandManager) {
		}
		public override string ToString() {
			return "Axis Y";
		}
	}
	[ModelOf(typeof(AxisY))]
	public class AxisYModel : AxisYBaseModel {
		protected new AxisY Axis { get { return (AxisY)base.Axis; } }
		[Browsable(false)]
		public new string Name { get; set; }
		public AxisYModel(AxisY axis, CommandManager commandManager)
			: base(axis, commandManager) {
		}
	}
	public abstract class SwiftPlotDiagramAxisModel : Axis2DModel {
		protected new SwiftPlotDiagramAxis Axis { get { return (SwiftPlotDiagramAxis)base.Axis; } }
		public SwiftPlotDiagramAxisModel(SwiftPlotDiagramAxis axis, CommandManager commandManager)
			: base(axis, commandManager) {
		}
	}
	public abstract class SwiftPlotDiagramAxisXBaseModel : SwiftPlotDiagramAxisModel {
		protected new SwiftPlotDiagramAxisXBase Axis { get { return (SwiftPlotDiagramAxisXBase)base.Axis; } }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.AxisXKey; } }
		public SwiftPlotDiagramAxisXBaseModel(SwiftPlotDiagramAxisXBase axis, CommandManager commandManager)
			: base(axis, commandManager) {
		}
	}
	public abstract class SwiftPlotDiagramAxisYBaseModel : SwiftPlotDiagramAxisModel {
		protected new SwiftPlotDiagramAxisYBase Axis { get { return (SwiftPlotDiagramAxisYBase)base.Axis; } }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.AxisYKey; } }
		public SwiftPlotDiagramAxisYBaseModel(SwiftPlotDiagramAxisYBase axis, CommandManager commandManager)
			: base(axis, commandManager) {
		}
	}
	[ModelOf(typeof(SwiftPlotDiagramAxisX))]
	public class SwiftPlotDiagramAxisXModel : SwiftPlotDiagramAxisXBaseModel {
		protected new SwiftPlotDiagramAxisX Axis { get { return (SwiftPlotDiagramAxisX)base.Axis; } }
		[Browsable(false)]
		protected new string Name { get; set; }
		public SwiftPlotDiagramAxisXModel(SwiftPlotDiagramAxisX axis, CommandManager commandManager)
			: base(axis, commandManager) {
		}
		public override string ToString() {
			return "Axis X";
		}
	}
	[ModelOf(typeof(SwiftPlotDiagramAxisY))]
	public class SwiftPlotDiagramAxisYModel : SwiftPlotDiagramAxisYBaseModel {
		protected new SwiftPlotDiagramAxisY Axis { get { return (SwiftPlotDiagramAxisY)base.Axis; } }
		[Browsable(false)]
		public new string Name { get; set; }
		public SwiftPlotDiagramAxisYModel(SwiftPlotDiagramAxisY axis, CommandManager commandManager)
			: base(axis, commandManager) {
		}
		public override string ToString() {
			return "Axis Y";
		}
	}
	[ModelOf(typeof(SwiftPlotDiagramSecondaryAxisX)), TypeConverter(typeof(SwiftPlotDiagramSecondaryAxisXTypeConverter))]
	public class SwiftPlotDiagramSecondaryAxisXModel : SwiftPlotDiagramAxisXBaseModel {
		protected new SwiftPlotDiagramSecondaryAxisX Axis { get { return (SwiftPlotDiagramSecondaryAxisX)base.Axis; } }
		public SwiftPlotDiagramSecondaryAxisXModel(SwiftPlotDiagramSecondaryAxisX axis, CommandManager commandManager)
			: base(axis, commandManager) {
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeElement(this, parentCollection);
		}
		public override string ToString() {
			return "Secondary Axis X";
		}
	}
	[ModelOf(typeof(SwiftPlotDiagramSecondaryAxisY)), TypeConverter(typeof(SwiftPlotDiagramSecondaryAxisYTypeConverter))]
	public class SwiftPlotDiagramSecondaryAxisYModel : SwiftPlotDiagramAxisYBaseModel {
		protected new SwiftPlotDiagramSecondaryAxisY Axis { get { return (SwiftPlotDiagramSecondaryAxisY)base.Axis; } }
		public SwiftPlotDiagramSecondaryAxisYModel(SwiftPlotDiagramSecondaryAxisY axis, CommandManager commandManager)
			: base(axis, commandManager) {
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeElement(this, parentCollection);
		}
		public override string ToString() {
			return "Secondary Axis Y";
		}
	}
	public abstract class RadarAxisModel : AxisModelBase {
		PolygonFillStyleModel interlacedFillStyleModel;
		protected new RadarAxis Axis { get { return (RadarAxis)base.Axis; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public PolygonFillStyleModel InterlacedFillStyle { get { return interlacedFillStyleModel; } }
		public RadarAxisModel(RadarAxis axis, CommandManager commandManager)
			: base(axis, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if (interlacedFillStyleModel != null)
				Children.Add(interlacedFillStyleModel);
			base.AddChildren();
		}
		public override void Update() {
			this.interlacedFillStyleModel = new PolygonFillStyleModel(Axis.InterlacedFillStyle, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(RadarAxisX))]
	public class RadarAxisXModel : RadarAxisModel {
		RadarAxisXLabelModel labelModel;
		protected new RadarAxisX Axis { get { return (RadarAxisX)base.Axis; } }
		[Browsable(false)]
		public new string Name { get; set; }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.AxisXKey; } }
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("General"),
		Category("Elements")]
		public RadarAxisXLabelModel Label { get { return labelModel; } }
		public RadarAxisXModel(RadarAxisX axis, CommandManager commandManager)
			: base(axis, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if (labelModel != null)
				Children.Add(labelModel);
			base.AddChildren();
		}
		public override void Update() {
			this.labelModel = new RadarAxisXLabelModel(Axis.Label, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
		public override string ToString() {
			return "Radar Axis X";
		}
	}
	[ModelOf(typeof(RadarAxisY))]
	public class RadarAxisYModel : RadarAxisModel, ISupportModelVisibility {
		RadarTickmarksYModel tickmarksModel;
		RadarAxisYLabelModel labelModel;
		protected new RadarAxisY Axis { get { return (RadarAxisY)base.Axis; } }
		[Browsable(false)]
		public new string Name { get; set; }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.AxisYKey; } }
		[PropertyForOptions(-1, "General"),
		Category("Appearance"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool Visible {
			get { return Axis.Visible; }
			set { SetProperty("Visible", value); }
		}
		[Category("Behavior"), TypeConverter(typeof(BooleanTypeConverter))]
		public bool TopLevel {
			get { return Axis.TopLevel; }
			set { SetProperty("TopLevel", value); }
		}
		[Category("Appearance")]
		public int Thickness {
			get { return Axis.Thickness; }
			set { SetProperty("Thickness", value); }
		}
		[Category("Appearance")]
		public Color Color {
			get { return Axis.Color; }
			set { SetProperty("Color", value); }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("General"),
		Category("Elements")]
		public RadarTickmarksYModel Tickmarks { get { return tickmarksModel; } }
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("General"),
		Category("Elements")]
		public RadarAxisYLabelModel Label { get { return labelModel; } }
		public RadarAxisYModel(RadarAxisY axis, CommandManager commandManager)
			: base(axis, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if (tickmarksModel != null)
				Children.Add(tickmarksModel);
			if (labelModel != null)
				Children.Add(labelModel);
			base.AddChildren();
		}
		public override void Update() {
			this.tickmarksModel = new RadarTickmarksYModel(Axis.Tickmarks, CommandManager);
			this.labelModel = new RadarAxisYLabelModel(Axis.Label, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
		public override string ToString() {
			return "Axis Y";
		}
	}
	public abstract class Axis3DModel : AxisModelBase {
		RectangleFillStyle3DModel interlacedFillStyleModel;
		AxisLabel3DModel labelModel;
		protected new Axis3D Axis { get { return (Axis3D)base.Axis; } }
		[Browsable(false)]
		public new string Name { get; set; }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public RectangleFillStyle3DModel InterlacedFillStyle { get { return interlacedFillStyleModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("General"),
		Category("Elements")]
		public AxisLabel3DModel Label { get { return labelModel; } }
		public Axis3DModel(Axis3D axis, CommandManager commandManager)
			: base(axis, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if (interlacedFillStyleModel != null)
				Children.Add(interlacedFillStyleModel);
			if (labelModel != null)
				Children.Add(labelModel);
			base.AddChildren();
		}
		public override void Update() {
			this.interlacedFillStyleModel = new RectangleFillStyle3DModel(Axis.InterlacedFillStyle, CommandManager);
			this.labelModel = new AxisLabel3DModel(Axis.Label, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(AxisX3D))]
	public class AxisX3DModel : Axis3DModel {
		protected new AxisX3D Axis { get { return (AxisX3D)base.Axis; } }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.AxisXKey; } }
		public AxisX3DModel(AxisX3D axis, CommandManager commandManager)
			: base(axis, commandManager) {
		}
		public override string ToString() {
			return "Axis X 3D";
		}
	}
	[ModelOf(typeof(AxisY3D))]
	public class AxisY3DModel : Axis3DModel {
		protected new AxisY3D Axis { get { return (AxisY3D)base.Axis; } }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.AxisYKey; } }
		public AxisY3DModel(AxisY3D axis, CommandManager commandManager)
			: base(axis, commandManager) {
		}
		public override string ToString() {
			return "Axis Y 3D";
		}
	}
	[ModelOf(typeof(SecondaryAxisX)), TypeConverter(typeof(SecondaryAxisXTypeConverter))]
	public class SecondaryAxisXModel : AxisXBaseModel {
		protected new SecondaryAxisX Axis { get { return (SecondaryAxisX)base.Axis; } }
		public SecondaryAxisXModel(SecondaryAxisX axis, CommandManager commandManager)
			: base(axis, commandManager) {
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeElement(this, parentCollection);
		}
		public override string ToString() {
			return "Secondary Axis X";
		}
	}
	[ModelOf(typeof(SecondaryAxisY)), TypeConverter(typeof(SecondaryAxisYTypeConverter))]
	public class SecondaryAxisYModel : AxisYBaseModel {
		protected new SecondaryAxisY Axis { get { return (SecondaryAxisY)base.Axis; } }
		public SecondaryAxisYModel(SecondaryAxisY axis, CommandManager commandManager)
			: base(axis, commandManager) {
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeElement(this, parentCollection);
		}
		public override string ToString() {
			return "Secondary Axis Y";
		}
	}
	[ModelOf(typeof(GanttAxisX))]
	public class GanttAxisXModel : AxisXModel {
		protected new GanttAxisX Axis { get { return (GanttAxisX)base.Axis; } }
		public GanttAxisXModel(GanttAxisX axis, CommandManager commandManager)
			: base(axis, commandManager) {
		}
	}
	[ModelOf(typeof(PolarAxisX))]
	public class PolarAxisXModel : RadarAxisXModel {
		protected new PolarAxisX Axis { get { return (PolarAxisX)base.Axis; } }
		[Browsable(false)]
		public new bool Logarithmic { get; set; }
		[Browsable(false)]
		public new double LogarithmicBase { get; set; }
		[Browsable(false)]
		public new VisualRangeModel VisualRange { get { return null; } }
		[Browsable(false)]
		public new WholeRangeModel WholeRange { get { return null; } }
		public PolarAxisXModel(PolarAxisX axis, CommandManager commandManager)
			: base(axis, commandManager) {
		}
		public override string ToString() {
			return "Polar Axis X";
		}
	}
}
