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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.PropertyGrid;
using DevExpress.Mvvm.Native;
namespace DevExpress.Charts.Designer.Native {
	public class AxisItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartAxisBasePropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewAxis)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartAxisBasePropertyGridModel();
		}
	}
	public abstract class SecondaryAxisPropertyGridModelCollection : PropertyGridModelCollectionBase {
		WpfChartModel chartModel;
		protected WpfChartModel ChartModel { get { return chartModel; } }
		protected abstract bool IsAxisX { get; }
		public SecondaryAxisPropertyGridModelCollection(WpfChartModel chartModel) {
			this.chartModel = chartModel;
		}
		protected abstract void ExecuteCommand();
		protected abstract bool CanRemoveAxis(WpfChartAxisBasePropertyGridModel axis);
		protected override void InsertItem(int index, PropertyGridModelBase item) {
			if (((WpfChartAxisBasePropertyGridModel)item).IsFake)
				base.InsertItem(index, WpfChartAxisBasePropertyGridModel.CreatePropertyGridModelForAxis(ChartModel.DiagramModel, IsAxisX));
			else
				base.InsertItem(index, item);
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					foreach (WpfChartAxisBasePropertyGridModel newAxis in e.NewItems)
						if (newAxis.AxisModel == null) {
							ExecuteCommand();
							break;
						}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (WpfChartAxisBasePropertyGridModel oldAxis in e.OldItems) {
						if (CanRemoveAxis(oldAxis)) {
							RemoveSecondaryAxisCommand command = new RemoveSecondaryAxisCommand(chartModel);
							((ICommand)command).Execute(oldAxis.AxisModel.Axis);
							break;
						}
					}
					break;
				default:
					break;
			}
		}
	}
	public class SecondaryAxisXPropertyGridModelCollection : SecondaryAxisPropertyGridModelCollection {
		protected override bool IsAxisX { get { return true; } }
		public SecondaryAxisXPropertyGridModelCollection(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override void ExecuteCommand() {
			AddSecondaryAxisX command = new AddSecondaryAxisX(ChartModel);
			((ICommand)command).Execute(null);
		}
		protected override bool CanRemoveAxis(WpfChartAxisBasePropertyGridModel axis) {
			return ChartModel.DiagramModel.SecondaryAxesCollectionModelX.ModelCollection.Contains(axis.AxisModel);
		}
	}
	public class SecondaryAxisYPropertyGridModelCollection : SecondaryAxisPropertyGridModelCollection {
		protected override bool IsAxisX { get { return false; } }
		public SecondaryAxisYPropertyGridModelCollection(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override bool CanRemoveAxis(WpfChartAxisBasePropertyGridModel axis) {
			return ChartModel.DiagramModel.SecondaryAxesCollectionModelY.ModelCollection.Contains(axis.AxisModel);
		}
		protected override void ExecuteCommand() {
			AddSecondaryAxisY command = new AddSecondaryAxisY(ChartModel);
			((ICommand)command).Execute(null);
		}
	}
	public class WpfChartAxisBasePropertyGridModel : PropertyGridModelBase {
		public static WpfChartAxisBasePropertyGridModel CreatePropertyGridModelForAxis(WpfChartDiagramModel diagramModel, bool isAxisX) {
			if (diagramModel.Diagram is XYDiagram2D)
				if (isAxisX)
					return new WpfChartAxisX2DPropertyGridModel(null);
				else
					return new WpfChartAxisY2DPropertyGridModel(null);
			if (diagramModel.Diagram is XYDiagram3D)
				if (isAxisX)
					return new WpfChartAxisX3DPropertyGridModel(null);
				else
					return new WpfChartAxisY3DPropertyGridModel(null);
			if (diagramModel.Diagram is RadarDiagram2D)
				if (isAxisX)
					return new WpfChartRadarAxisX2DPropertyGridModel(null);
				else
					return new WpfChartCircularAxisY2DPropertyGridModel(null);
			if (diagramModel.Diagram is PolarDiagram2D)
				if (isAxisX)
					return new WpfChartCircularAxisX2DPropertyGridModel(null);
				else
					return new WpfChartCircularAxisY2DPropertyGridModel(null);
			return null;
		}
		public static WpfChartAxisBasePropertyGridModel CreatePropertyGridModelForAxis(WpfChartAxisModel axisModel) {
			if (axisModel.Axis is AxisX3D)
				return new WpfChartAxisX3DPropertyGridModel(axisModel);
			if (axisModel.Axis is AxisY3D)
				return new WpfChartAxisY3DPropertyGridModel(axisModel);
			if (axisModel.Axis is AxisX2D)
				return new WpfChartAxisX2DPropertyGridModel(axisModel);
			if (axisModel.Axis is AxisY2D)
				return new WpfChartAxisY2DPropertyGridModel(axisModel);
			if (axisModel.Axis is RadarAxisX2D)
				return new WpfChartRadarAxisX2DPropertyGridModel(axisModel);
			if (axisModel.Axis is CircularAxisY2D)
				return new WpfChartCircularAxisY2DPropertyGridModel(axisModel);
			if (axisModel.Axis is PolarAxisX2D)
				return new WpfChartCircularAxisX2DPropertyGridModel(axisModel);
			return null;
		}
		SetAxisPropertyCommand setPropertyCommand;
		WpfChartLineStylePropertyGridModel gridLinesLineStyle;
		WpfChartLineStylePropertyGridModel gridLinesMinorLineStyle;
		WpfChartAxisLabelPropertyGridModel label;
		internal bool IsFake { get { return AxisModel == null; } }
		protected override ICommand SetObjectPropertyCommand { get { return setPropertyCommand; } }
		protected internal WpfChartAxisModel AxisModel { get { return ModelElement as WpfChartAxisModel; } }
		protected AxisBase Axis { get { return AxisModel.Axis; } }
		[Category(Categories.Behavior)]
		public int MinorCount {
			get { return Axis.MinorCount; }
			set { SetProperty("MinorCount", value); }
		}
		[Category(Categories.Behavior)]
		public bool GridLinesVisible {
			get { return Axis.GridLinesVisible; }
			set { SetProperty("GridLinesVisible", value); }
		}
		[Category(Categories.Behavior)]
		public bool GridLinesMinorVisible {
			get { return Axis.GridLinesMinorVisible; }
			set { SetProperty("GridLinesMinorVisible", value); }
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Presentation)
		]
		public WpfChartLineStylePropertyGridModel GridLinesLineStyle {
			get { return gridLinesLineStyle; }
			set { SetProperty("GridLinesLineStyle", new LineStyle()); }
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Presentation)
		]
		public WpfChartLineStylePropertyGridModel GridLinesMinorLineStyle {
			get { return gridLinesMinorLineStyle; }
			set { SetProperty("GridLinesMinorLineStyle", new LineStyle()); }
		}
		[Category(Categories.Brushes)]
		public Brush GridLinesBrush {
			get { return Axis.GridLinesBrush; }
			set { SetProperty("GridLinesBrush", value); }
		}
		[Category(Categories.Brushes)]
		public Brush GridLinesMinorBrush {
			get { return Axis.GridLinesMinorBrush; }
			set { SetProperty("GridLinesMinorBrush", value); }
		}
		[Category(Categories.Behavior)]
		public bool Interlaced {
			get { return Axis.Interlaced; }
			set { SetProperty("Interlaced", value); }
		}
		[Category(Categories.Brushes)]
		public Brush InterlacedBrush {
			get { return Axis.InterlacedBrush; }
			set { SetProperty("InterlacedBrush", value); }
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Elements)
		]
		public WpfChartAxisLabelPropertyGridModel Label {
			get { return label; }
			set { SetProperty("Label", new AxisLabel()); }
		}
		public WpfChartAxisBasePropertyGridModel()
			: this(null) {
		}
		public WpfChartAxisBasePropertyGridModel(WpfChartAxisModel axisModel)
			: base(axisModel) {
			if (axisModel != null)
				UpdateInternal();
		}
		protected virtual WpfChartAxisLabelPropertyGridModel CreateLabelModel(AxisLabel label) {
			return new WpfChartAxisLabelPropertyGridModel(this.AxisModel, label);
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (AxisModel == null || AxisModel.Axis == null)
				return;
			if (Axis.GridLinesLineStyle != null) {
				if (gridLinesLineStyle != null && Axis.GridLinesLineStyle != gridLinesLineStyle.LineStyle || gridLinesLineStyle == null)
					gridLinesLineStyle = new WpfChartLineStylePropertyGridModel(AxisModel, Axis.GridLinesLineStyle, setPropertyCommand, "GridLinesLineStyle.");
			}
			else
				gridLinesLineStyle = null;
			if (Axis.GridLinesMinorLineStyle != null) {
				if (gridLinesMinorLineStyle != null && Axis.GridLinesMinorLineStyle != gridLinesMinorLineStyle.LineStyle || gridLinesMinorLineStyle == null)
					gridLinesMinorLineStyle = new WpfChartLineStylePropertyGridModel(AxisModel, Axis.GridLinesMinorLineStyle, setPropertyCommand, "GridLinesMinorLineStyle.");
			}
			else
				gridLinesMinorLineStyle = null;
			if (Axis.Label != null) {
				if (label != null && Axis.Label != label.Label || label == null)
					label = CreateLabelModel(Axis.Label);
			}
			else
				label = Axis.ActualLabel.Visible ? CreateLabelModel(Axis.ActualLabel) : null;
		}
		protected override void UpdateCommands() {
			base.UpdateCommands();
			setPropertyCommand = new SetAxisPropertyCommand(ChartModel);
		}
	}
	public abstract class WpfChartAxisPropertyGridModel : WpfChartAxisBasePropertyGridModel {
		WpfChartAxisRangePropertyGridModel wholeRange;
		WpfChartAxisTitlePropertyGridModel title;
		new protected Axis Axis { get { return base.Axis as Axis; } }
		[Category(Categories.Behavior)]
		public bool Logarithmic {
			get { return Axis.Logarithmic; }
			set { SetProperty("Logarithmic", value); }
		}
		[Category(Categories.Behavior)]
		public double LogarithmicBase {
			get { return Axis.LogarithmicBase; }
			set { SetProperty("LogarithmicBase", value); }
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Common)
		]
		public WpfChartAxisRangePropertyGridModel WholeRange {
			get { return wholeRange; }
			set { SetProperty("WholeRange", new Range()); }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DefaultValue(null),
		Category(Categories.Elements)
		]
		public WpfChartAxisTitlePropertyGridModel Title {
			get { return title; }
			set {
				AxisTitle newValue = value != null ? new AxisTitle() : null;
				SetProperty("Title", newValue);
			}
		}
		public WpfChartAxisPropertyGridModel(WpfChartAxisModel axisModel)
			: base(axisModel) {
		}
		protected virtual WpfChartAxisRangePropertyGridModel CreateRangeModel(Range range, string propertyPath) {
			return new WpfChartAxisRangePropertyGridModel(AxisModel, range, propertyPath);
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Axis.WholeRange != null) {
				if (wholeRange != null && Axis.WholeRange != wholeRange.Range || wholeRange == null)
					wholeRange = CreateRangeModel(Axis.WholeRange, "WholeRange.");
			}
			else
				wholeRange = null;
			if (Axis.Title != null) {
				if (title != null && Axis.Title != title.Title || title == null)
					title = new WpfChartAxisTitlePropertyGridModel(AxisModel, Axis.Title);
			}
			else
				title = null;
		}
	}
	public abstract class WpfChartAxis2DPropertyGridModel : WpfChartAxisPropertyGridModel {
		WpfChartAxisRangePropertyGridModel visualRange;
		WpfChartCrosshairAxisLabelOptionsPropertyGridModel crosshairAxisLabelOptions;
		new protected Axis2D Axis { get { return base.Axis as Axis2D; } }
		[Category(Categories.Behavior)]
		public bool? Visible {
			get { return Axis.Visible; }
			set { SetProperty("Visible", value); }
		}
		[Category(Categories.Layout)]
		public AxisAlignment Alignment {
			get { return Axis.Alignment; }
			set { SetProperty("Alignment", value); }
		}
		[Category(Categories.Behavior)]
		public bool Reverse {
			get { return Axis.Reverse; }
			set { SetProperty("Reverse", value); }
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Common)
		]
		public WpfChartAxisRangePropertyGridModel VisualRange {
			get { return visualRange; }
			set { SetProperty("VisualRange", new Range()); }
		}
		[Category(Categories.Presentation)]
		public int Thickness {
			get { return Axis.Thickness; }
			set { SetProperty("Thickness", value); }
		}
		[Category(Categories.Brushes)]
		public Brush Brush {
			get { return Axis.Brush; }
			set { SetProperty("Brush", value); }
		}
		[Category(Categories.Behavior)]
		public bool TickmarksVisible {
			get { return Axis.TickmarksVisible; }
			set { SetProperty("TickmarksVisible", value); }
		}
		[Category(Categories.Behavior)]
		public bool TickmarksMinorVisible {
			get { return Axis.TickmarksMinorVisible; }
			set { SetProperty("TickmarksMinorVisible", value); }
		}
		[Category(Categories.Presentation)]
		public int TickmarksThickness {
			get { return Axis.TickmarksThickness; }
			set { SetProperty("TickmarksThickness", value); }
		}
		[Category(Categories.Presentation)]
		public int TickmarksMinorThickness {
			get { return Axis.TickmarksMinorThickness; }
			set { SetProperty("TickmarksMinorThickness", value); }
		}
		[Category(Categories.Presentation)]
		public int TickmarksLength {
			get { return Axis.TickmarksLength; }
			set { SetProperty("TickmarksLength", value); }
		}
		[Category(Categories.Presentation)]
		public int TickmarksMinorLength {
			get { return Axis.TickmarksMinorLength; }
			set { SetProperty("TickmarksMinorLength", value); }
		}
		[Category(Categories.Presentation)]
		public bool TickmarksCrossAxis {
			get { return Axis.TickmarksCrossAxis; }
			set { SetProperty("TickmarksCrossAxis", value); }
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Appearance)
		]
		public WpfChartCrosshairAxisLabelOptionsPropertyGridModel CrosshairAxisLabelOptions {
			get { return crosshairAxisLabelOptions; }
			set { SetProperty("CrosshairAxisLabelOptions", new CrosshairAxisLabelOptions()); }
		}
		public WpfChartAxis2DPropertyGridModel(WpfChartAxisModel axisModel)
			: base(axisModel) {
		}
		protected override WpfChartAxisLabelPropertyGridModel CreateLabelModel(AxisLabel label) {
			return new WpfChartAxis2DLabelPropertyGridModel(AxisModel, label);
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Axis.VisualRange != null) {
				if (visualRange != null && Axis.VisualRange != visualRange.Range || visualRange == null)
					visualRange = CreateRangeModel(Axis.VisualRange, "VisualRange.");
			}
			else
				visualRange = null;
			if (Axis.CrosshairAxisLabelOptions != null) {
				if (crosshairAxisLabelOptions != null && Axis.CrosshairAxisLabelOptions != crosshairAxisLabelOptions.CrosshairAxisLabelOptions || crosshairAxisLabelOptions == null)
					crosshairAxisLabelOptions = new WpfChartCrosshairAxisLabelOptionsPropertyGridModel(AxisModel, Axis.CrosshairAxisLabelOptions, "CrosshairAxisLabelOptions.");
			}
			else
				crosshairAxisLabelOptions = null;
		}
	}
	public class WpfChartAxisX2DPropertyGridModel : WpfChartAxis2DPropertyGridModel {
		WpfChartNumericScaleOptionsBasePropertyGridModel numericScaleOptions;
		WpfChartDateTimeScaleOptionsBasePropertyGridModel dateTimeScaleOptions;
		new protected AxisX2D Axis { get { return base.Axis as AxisX2D; } }
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartNumericScaleOptionsBasePropertyGridModel NumericScaleOptions {
			get { return numericScaleOptions; }
			set {
				if (value != null)
					SetProperty("NumericScaleOptions", value.CreateScaleOptions());
				else
					SetProperty("NumericScaleOptions", null);
			}
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartDateTimeScaleOptionsBasePropertyGridModel DateTimeScaleOptions {
			get { return dateTimeScaleOptions; }
			set {
				if (value != null)
					SetProperty("DateTimeScaleOptions", value.CreateScaleOptions());
				else
					SetProperty("DateTimeScaleOptions", null);
			}
		}
		public WpfChartAxisX2DPropertyGridModel(WpfChartAxisModel axisModel)
			: base(axisModel) {
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Axis.NumericScaleOptions != null)
				numericScaleOptions = WpfChartNumericScaleOptionsBasePropertyGridModel.CreatePropertyGridModel(AxisModel, Axis.NumericScaleOptions, "NumericScaleOptions.");
			else
				numericScaleOptions = null;
			if (Axis.DateTimeScaleOptions != null)
				dateTimeScaleOptions = WpfChartDateTimeScaleOptionsBasePropertyGridModel.CreatePropertyGridModel(AxisModel, Axis.DateTimeScaleOptions, "DateTimeScaleOptions.");
			else
				dateTimeScaleOptions = null;
		}
	}
	public class WpfChartAxisY2DPropertyGridModel : WpfChartAxis2DPropertyGridModel {
		WpfChartContinuousNumericScaleOptionsPropertyGridModel numericScaleOptions;
		WpfChartContinuousDateTimeScaleOptionsPropertyGridModel dateTimeScaleOptions;
		new AxisY2D Axis { get { return base.Axis as AxisY2D; } }
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartContinuousNumericScaleOptionsPropertyGridModel NumericScaleOptions {
			get { return numericScaleOptions; }
			set {
				if (value != null)
					SetProperty("NumericScaleOptions", value.CreateScaleOptions());
				else
					SetProperty("NumericScaleOptions", null);
			}
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartContinuousDateTimeScaleOptionsPropertyGridModel DateTimeScaleOptions {
			get { return dateTimeScaleOptions; }
			set {
				if (value != null)
					SetProperty("DateTimeScaleOptions", value.CreateScaleOptions());
				else
					SetProperty("DateTimeScaleOptions", null);
			}
		}
		public WpfChartAxisY2DPropertyGridModel(WpfChartAxisModel axisModel)
			: base(axisModel) {
		}
		protected override WpfChartAxisRangePropertyGridModel CreateRangeModel(Range range, string propertyPath) {
			return new WpfChartAxisY2DRangePropertyGridModel(AxisModel, range, propertyPath);
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Axis.NumericScaleOptions != null)
				numericScaleOptions = (WpfChartContinuousNumericScaleOptionsPropertyGridModel)WpfChartNumericScaleOptionsBasePropertyGridModel.CreatePropertyGridModel(AxisModel, Axis.NumericScaleOptions, "NumericScaleOptions.");
			else
				numericScaleOptions = null;
			if (Axis.DateTimeScaleOptions != null)
				dateTimeScaleOptions = (WpfChartContinuousDateTimeScaleOptionsPropertyGridModel)WpfChartDateTimeScaleOptionsBasePropertyGridModel.CreatePropertyGridModel(AxisModel, Axis.DateTimeScaleOptions, "DateTimeScaleOptions.");
			else
				dateTimeScaleOptions = null;
		}
	}
	public class WpfChartAxisX3DPropertyGridModel : WpfChartAxisPropertyGridModel {
		WpfChartNumericScaleOptionsBasePropertyGridModel numericScaleOptions;
		WpfChartDateTimeScaleOptionsBasePropertyGridModel dateTimeScaleOptions;
		new protected AxisX3D Axis { get { return base.Axis as AxisX3D; } }
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartNumericScaleOptionsBasePropertyGridModel NumericScaleOptions {
			get { return numericScaleOptions; }
			set {
				if (value != null)
					SetProperty("NumericScaleOptions", value.CreateScaleOptions());
				else
					SetProperty("NumericScaleOptions", null);
			}
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartDateTimeScaleOptionsBasePropertyGridModel DateTimeScaleOptions {
			get { return dateTimeScaleOptions; }
			set {
				if (value != null)
					SetProperty("DateTimeScaleOptions", value.CreateScaleOptions());
				else
					SetProperty("DateTimeScaleOptions", null);
			}
		}
		public WpfChartAxisX3DPropertyGridModel(WpfChartAxisModel axisModel)
			: base(axisModel) {
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Axis.NumericScaleOptions != null)
				numericScaleOptions = WpfChartNumericScaleOptionsBasePropertyGridModel.CreatePropertyGridModel(AxisModel, Axis.NumericScaleOptions, "NumericScaleOptions.");
			else
				numericScaleOptions = null;
			if (Axis.DateTimeScaleOptions != null)
				dateTimeScaleOptions = WpfChartDateTimeScaleOptionsBasePropertyGridModel.CreatePropertyGridModel(AxisModel, Axis.DateTimeScaleOptions, "DateTimeScaleOptions.");
			else
				dateTimeScaleOptions = null;
		}
	}
	public class WpfChartAxisY3DPropertyGridModel : WpfChartAxisPropertyGridModel {
		WpfChartContinuousNumericScaleOptionsPropertyGridModel numericScaleOptions;
		WpfChartContinuousDateTimeScaleOptionsPropertyGridModel dateTimeScaleOptions;
		new AxisY3D Axis { get { return base.Axis as AxisY3D; } }
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartContinuousNumericScaleOptionsPropertyGridModel NumericScaleOptions {
			get { return numericScaleOptions; }
			set {
				if (value != null)
					SetProperty("NumericScaleOptions", value.CreateScaleOptions());
				else
					SetProperty("NumericScaleOptions", null);
			}
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartContinuousDateTimeScaleOptionsPropertyGridModel DateTimeScaleOptions {
			get { return dateTimeScaleOptions; }
			set {
				if (value != null)
					SetProperty("DateTimeScaleOptions", value.CreateScaleOptions());
				else
					SetProperty("DateTimeScaleOptions", null);
			}
		}
		public WpfChartAxisY3DPropertyGridModel(WpfChartAxisModel axisModel)
			: base(axisModel) {
		}
		protected override WpfChartAxisRangePropertyGridModel CreateRangeModel(Range range, string propertyPath) {
			return new WpfChartAxisY3DRangePropertyGridModel(AxisModel, range, propertyPath);
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Axis.NumericScaleOptions != null)
				numericScaleOptions = (WpfChartContinuousNumericScaleOptionsPropertyGridModel)WpfChartNumericScaleOptionsBasePropertyGridModel.CreatePropertyGridModel(AxisModel, Axis.NumericScaleOptions, "NumericScaleOptions.");
			else
				numericScaleOptions = null;
			if (Axis.DateTimeScaleOptions != null)
				dateTimeScaleOptions = (WpfChartContinuousDateTimeScaleOptionsPropertyGridModel)WpfChartDateTimeScaleOptionsBasePropertyGridModel.CreatePropertyGridModel(AxisModel, Axis.DateTimeScaleOptions, "DateTimeScaleOptions.");
			else
				dateTimeScaleOptions = null;
		}
	}
	public class WpfChartCircularAxisY2DPropertyGridModel : WpfChartAxisBasePropertyGridModel {
		WpfChartAxisRangePropertyGridModel wholeRange;
		WpfChartContinuousNumericScaleOptionsPropertyGridModel numericScaleOptions;
		WpfChartContinuousDateTimeScaleOptionsPropertyGridModel dateTimeScaleOptions;
		new protected CircularAxisY2D Axis { get { return base.Axis as CircularAxisY2D; } }
		[Category(Categories.Behavior)]
		public bool Logarithmic {
			get { return Axis.Logarithmic; }
			set { SetProperty("Logarithmic", value); }
		}
		[Category(Categories.Behavior)]
		public double LogarithmicBase {
			get { return Axis.LogarithmicBase; }
			set { SetProperty("LogarithmicBase", value); }
		}
		[Category(Categories.Presentation)]
		public int Thickness {
			get { return Axis.Thickness; }
			set { SetProperty("Thickness", value); }
		}
		[Category(Categories.Brushes)]
		public Brush Brush {
			get { return Axis.Brush; }
			set { SetProperty("Brush", value); }
		}
		[Category(Categories.Behavior)]
		public bool TickmarksVisible {
			get { return Axis.TickmarksVisible; }
			set { SetProperty("TickmarksVisible", value); }
		}
		[Category(Categories.Behavior)]
		public bool TickmarksMinorVisible {
			get { return Axis.TickmarksMinorVisible; }
			set { SetProperty("TickmarksMinorVisible", value); }
		}
		[Category(Categories.Presentation)]
		public int TickmarksThickness {
			get { return Axis.TickmarksThickness; }
			set { SetProperty("TickmarksThickness", value); }
		}
		[Category(Categories.Presentation)]
		public int TickmarksMinorThickness {
			get { return Axis.TickmarksMinorThickness; }
			set { SetProperty("TickmarksMinorThickness", value); }
		}
		[Category(Categories.Presentation)]
		public int TickmarksLength {
			get { return Axis.TickmarksLength; }
			set { SetProperty("TickmarksLength", value); }
		}
		[Category(Categories.Presentation)]
		public int TickmarksMinorLength {
			get { return Axis.TickmarksMinorLength; }
			set { SetProperty("TickmarksMinorLength", value); }
		}
		[Category(Categories.Presentation)]
		public bool TickmarksCrossAxis {
			get { return Axis.TickmarksCrossAxis; }
			set { SetProperty("TickmarksCrossAxis", value); }
		}
		[Category(Categories.Behavior)]
		public bool Visible {
			get { return Axis.Visible; }
			set { SetProperty("Visible", value); }
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Common)
		]
		public WpfChartAxisRangePropertyGridModel WholeRange {
			get { return wholeRange; }
			set { SetProperty("WholeRange", new Range()); }
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartContinuousNumericScaleOptionsPropertyGridModel NumericScaleOptions {
			get { return numericScaleOptions; }
			set {
				if (value != null)
					SetProperty("NumericScaleOptions", value.CreateScaleOptions());
				else
					SetProperty("NumericScaleOptions", null);
			}
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartContinuousDateTimeScaleOptionsPropertyGridModel DateTimeScaleOptions {
			get { return dateTimeScaleOptions; }
			set {
				if (value != null)
					SetProperty("DateTimeScaleOptions", value.CreateScaleOptions());
				else
					SetProperty("DateTimeScaleOptions", null);
			}
		}
		public WpfChartCircularAxisY2DPropertyGridModel(WpfChartAxisModel axisModel)
			: base(axisModel) {
		}
		WpfChartAxisRangePropertyGridModel CreateRangeModel(Range range, string propertyPath) {
			return new WpfChartCircularAxisY2DRangePropertyGridModel(AxisModel, range, propertyPath);
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Axis.WholeRange != null) {
				if (wholeRange != null && Axis.WholeRange != wholeRange.Range || wholeRange == null)
					wholeRange = CreateRangeModel(Axis.WholeRange, "WholeRange.");
			}
			else
				wholeRange = null;
			if (Axis.NumericScaleOptions != null)
				numericScaleOptions = (WpfChartContinuousNumericScaleOptionsPropertyGridModel)WpfChartNumericScaleOptionsBasePropertyGridModel.CreatePropertyGridModel(AxisModel, Axis.NumericScaleOptions, "NumericScaleOptions.");
			else
				numericScaleOptions = null;
			if (Axis.DateTimeScaleOptions != null)
				dateTimeScaleOptions = (WpfChartContinuousDateTimeScaleOptionsPropertyGridModel)WpfChartDateTimeScaleOptionsBasePropertyGridModel.CreatePropertyGridModel(AxisModel, Axis.DateTimeScaleOptions, "DateTimeScaleOptions.");
			else
				dateTimeScaleOptions = null;
		}
	}
	public class WpfChartCircularAxisX2DPropertyGridModel : WpfChartAxisBasePropertyGridModel {
		WpfChartNumericScaleOptionsBasePropertyGridModel numericScaleOptions;
		new protected CircularAxisX2D Axis { get { return base.Axis as CircularAxisX2D; } }
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartNumericScaleOptionsBasePropertyGridModel NumericScaleOptions {
			get { return numericScaleOptions; }
			set {
				if (value != null)
					SetProperty("NumericScaleOptions", value.CreateScaleOptions());
				else
					SetProperty("NumericScaleOptions", null);
			}
		}
		public WpfChartCircularAxisX2DPropertyGridModel(WpfChartAxisModel axisModel)
			: base(axisModel) {
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Axis.NumericScaleOptions != null)
				numericScaleOptions = WpfChartNumericScaleOptionsBasePropertyGridModel.CreatePropertyGridModel(AxisModel, Axis.NumericScaleOptions, "NumericScaleOptions.");
			else
				numericScaleOptions = null;
		}
	}
	public class WpfChartRadarAxisX2DPropertyGridModel : WpfChartCircularAxisX2DPropertyGridModel {
		WpfChartAxisRangePropertyGridModel wholeRange;
		WpfChartDateTimeScaleOptionsBasePropertyGridModel dateTimeScaleOptions;
		new protected RadarAxisX2D Axis { get { return base.Axis as RadarAxisX2D; } }
		[Category(Categories.Behavior)]
		public bool Logarithmic {
			get { return Axis.Logarithmic; }
			set { SetProperty("Logarithmic", value); }
		}
		[Category(Categories.Behavior)]
		public double LogarithmicBase {
			get { return Axis.LogarithmicBase; }
			set { SetProperty("LogarithmicBase", value); }
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Common)
		]
		public WpfChartAxisRangePropertyGridModel WholeRange {
			get { return wholeRange; }
			set { SetProperty("WholeRange", new Range()); }
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartDateTimeScaleOptionsBasePropertyGridModel DateTimeScaleOptions {
			get { return dateTimeScaleOptions; }
			set {
				if (value != null)
					SetProperty("DateTimeScaleOptions", value.CreateScaleOptions());
				else
					SetProperty("DateTimeScaleOptions", null);
			}
		}
		public WpfChartRadarAxisX2DPropertyGridModel(WpfChartAxisModel axisModel)
			: base(axisModel) {
		}
		WpfChartAxisRangePropertyGridModel CreateRangeModel(Range range, string propertyPath) {
			return new WpfChartAxisRangePropertyGridModel(AxisModel, range, propertyPath);
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Axis.WholeRange != null) {
				if (wholeRange != null && Axis.WholeRange != wholeRange.Range || wholeRange == null)
					wholeRange = CreateRangeModel(Axis.WholeRange, "WholeRange.");
			}
			else
				wholeRange = null;
			if (Axis.DateTimeScaleOptions != null)
				dateTimeScaleOptions = WpfChartDateTimeScaleOptionsBasePropertyGridModel.CreatePropertyGridModel(AxisModel, Axis.DateTimeScaleOptions, "DateTimeScaleOptions.");
			else
				dateTimeScaleOptions = null;
		}
	}
	public class WpfChartAxisTitlePropertyGridModel : WpfChartTitleBasePropertyGridModel {
		readonly SetAxisPropertyCommand setPropertyCommand;
		internal new AxisTitle Title { get { return base.Title as AxisTitle; } }
		[Category(Categories.Layout)]
		public TitleAlignment Alignment {
			get { return Title.Alignment; }
			set { SetProperty("Alignment", value); }
		}
		protected override ICommand SetObjectPropertyCommand {
			get { return setPropertyCommand; }
		}
		public WpfChartAxisTitlePropertyGridModel() : this(null, null) { }
		public WpfChartAxisTitlePropertyGridModel(ChartModelElement modelElement, AxisTitle title)
			: base(modelElement, title, "Title.") {
			setPropertyCommand = new SetAxisPropertyCommand(ChartModel);
		}
	}
}
