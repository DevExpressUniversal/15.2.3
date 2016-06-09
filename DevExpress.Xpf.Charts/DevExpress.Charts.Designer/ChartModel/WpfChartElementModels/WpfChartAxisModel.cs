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
using System.Windows.Input;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public abstract class WpfChartAxisModel : ChartModelElement {
		readonly WpfChartConstantLinesCollectionModel constantLinesCollectionModel;
		readonly WpfChartAxisCustomLabelCollectionModel customAxisLabelCollectionModel;
		readonly WpfChartStripCollectionModel stripCollectionModel;
		WpfChartAxisTitleModel title;
		WpfChartAxisLabelModel label;
		string axisName;
		public override IEnumerable<ChartModelElement> Children {
			get { return new ChartModelElement[] { ConstantLinesCollectionModel, stripCollectionModel, customAxisLabelCollectionModel }; }
		}
		public AxisBase Axis {
			get { return (AxisBase)ChartElement; }
		}
		public AxisAlignment Alignment {
			get {
				if (Axis is Axis2D)
					return ((Axis2D)Axis).Alignment;
				return AxisAlignment.Near;
			}
			set {
				if (Alignment != value) {
					if (Axis is Axis2D)
						((Axis2D)Axis).Alignment = value;
					OnPropertyChanged("Alignment");
				}
			}
		}
		public ScaleType ScaleType {
			get { return ChartDesignerPropertiesProvider.GetAxisScaleType(Axis); }
		}
		public WpfChartConstantLinesCollectionModel ConstantLinesCollectionModel {
			get { return constantLinesCollectionModel; }
		}
		public WpfChartAxisCustomLabelCollectionModel CustomLabelCollectionModel {
			get { return customAxisLabelCollectionModel; }
		}
		public WpfChartStripCollectionModel StripCollectionModel {
			get { return stripCollectionModel; }
		}
		public WpfChartAxisTitleModel Title {
			get { return title; }
			set {
				if (title != value) {
					title = value;
					OnPropertyChanged("Title");
				}
			}
		}
		public WpfChartAxisLabelModel Label {
			get { return label; }
			set {
				if (label != value) {
					label = value;
					OnPropertyChanged("Label");
				}
			}
		}
		public object WholeRangeMaxValue {
			get {
				if (Axis is Axis && ((Axis)Axis).WholeRange != null) {
					return ((Axis)Axis).WholeRange.MaxValue;
				}
				else if (Axis is PolarAxisY2D && ((PolarAxisY2D)Axis).WholeRange != null) {
					return ((PolarAxisY2D)Axis).WholeRange.MaxValue;
				}
				else if (Axis is RadarAxisX2D && ((RadarAxisX2D)Axis).WholeRange != null) {
					return ((RadarAxisX2D)Axis).WholeRange.MaxValue;
				}
				else if (Axis is RadarAxisY2D && ((RadarAxisY2D)Axis).WholeRange != null) {
					return ((RadarAxisY2D)Axis).WholeRange.MaxValue;
				}
				else
					return null;
			}
			set {
				if (Axis is Axis && ((Axis)Axis).WholeRange.MaxValue != value) {
					((Axis)Axis).WholeRange.MaxValue = value;
					OnPropertyChanged("WholeRangeMaxValue");
				}
				else if (Axis is PolarAxisY2D && ((PolarAxisY2D)Axis).WholeRange.MaxValue != value) {
					((PolarAxisY2D)Axis).WholeRange.MaxValue = value;
					OnPropertyChanged("WholeRangeMaxValue");
				}
				else if (Axis is RadarAxisX2D && ((RadarAxisX2D)Axis).WholeRange.MaxValue != value) {
					((RadarAxisX2D)Axis).WholeRange.MaxValue = value;
					OnPropertyChanged("WholeRangeMaxValue");
				}
				else if (Axis is RadarAxisX2D && ((RadarAxisY2D)Axis).WholeRange.MaxValue != value) {
					((RadarAxisY2D)Axis).WholeRange.MaxValue = value;
					OnPropertyChanged("WholeRangeMaxValue");
				}
			}
		}
		public object WholeRangeMinValue {
			get {
				if (Axis is Axis && ((Axis)Axis).WholeRange != null) {
					return ((Axis)Axis).WholeRange.MinValue;
				}
				else if (Axis is PolarAxisY2D && ((PolarAxisY2D)Axis).WholeRange != null) {
					return ((PolarAxisY2D)Axis).WholeRange.MinValue;
				}
				else if (Axis is RadarAxisX2D && ((RadarAxisX2D)Axis).WholeRange != null) {
					return ((RadarAxisX2D)Axis).WholeRange.MinValue;
				}
				else if (Axis is RadarAxisY2D && ((RadarAxisY2D)Axis).WholeRange != null) {
					return ((RadarAxisY2D)Axis).WholeRange.MinValue;
				}
				else
					return null;
			}
			set {
				if (Axis is Axis && ((Axis)Axis).WholeRange.MinValue != value) {
					((Axis)Axis).WholeRange.MinValue = value;
					OnPropertyChanged("WholeRangeMinValue");
				}
				else if (Axis is PolarAxisY2D && ((PolarAxisY2D)Axis).WholeRange.MinValue != value) {
					((PolarAxisY2D)Axis).WholeRange.MinValue = value;
					OnPropertyChanged("WholeRangeMinValue");
				}
				else if (Axis is RadarAxisX2D && ((RadarAxisX2D)Axis).WholeRange.MinValue != value) {
					((RadarAxisX2D)Axis).WholeRange.MinValue = value;
					OnPropertyChanged("WholeRangeMinValue");
				}
				else if (Axis is RadarAxisX2D && ((RadarAxisY2D)Axis).WholeRange.MinValue != value) {
					((RadarAxisY2D)Axis).WholeRange.MinValue = value;
					OnPropertyChanged("WholeRangeMinValue");
				}
			}
		}
		public object VisualRangeMinValue {
			get {
				if (Axis is Axis2D && ((Axis2D)Axis).VisualRange != null)
					return ((Axis2D)Axis).VisualRange.MinValue;
				else
					return null;
			}
			set {
				if (Axis is Axis2D && ((Axis2D)Axis).VisualRange.MinValue != value) {
					((Axis2D)Axis).VisualRange.MinValue = value;
					OnPropertyChanged("VisualRangeMinValue");
				}
				else
					ChartDebug.WriteWarning("WholeRange does not supported by " + Axis.GetType().Name + ".");
			}
		}
		public object VisualRangeMaxValue {
			get {
				if (Axis is Axis2D && ((Axis2D)Axis).VisualRange != null)
					return ((Axis2D)Axis).VisualRange.MaxValue;
				else
					return null;
			}
			set {
				if (Axis is Axis2D && ((Axis2D)Axis).VisualRange.MaxValue != value) {
					((Axis2D)Axis).VisualRange.MaxValue = value;
					OnPropertyChanged("VisualRangeMinValue");
				}
				else
					ChartDebug.WriteWarning("WholeRange does not supported by " + Axis.GetType().Name + ".");
			}
		}
		public string Name {
			get { return axisName; }
		}
		public int MinorCount {
			get { return Axis.MinorCount; }
			set {
				if (Axis.MinorCount != value) {
					Axis.MinorCount = value;
					OnPropertyChanged("MinorCount");
				}
			}
		}
		public bool IsVisible {
			get {
				if (Axis is Axis2D) {
					bool? visible = ((Axis2D)Axis).Visible;
					if (visible.HasValue)
						return visible.Value;
					return true;
				}
				if (Axis is CircularAxisY2D)
					return ((CircularAxisY2D)Axis).Visible;
				return true;
			}
			set {
				if (IsVisible != value) {
					if (Axis is Axis2D)
						((Axis2D)Axis).Visible = value;
					if (Axis is CircularAxisY2D)
						((CircularAxisY2D)Axis).Visible = value;
					OnPropertyChanged("IsVisible");
				}
			}
		}
		public bool Reverse {
			get {
				if (Axis is Axis2D)
					return ((Axis2D)Axis).Reverse;
				return false;
			}
			set {
				if (Reverse != value) {
					if (Axis is Axis2D)
						((Axis2D)Axis).Reverse = value;
					OnPropertyChanged("Reverse");
				}
			}
		}
		public bool GridLinesVisible {
			get { return Axis.GridLinesVisible; }
			set {
				if (Axis.GridLinesVisible != value) {
					Axis.GridLinesVisible = value;
					OnPropertyChanged("GridLinesVisible");
				}
			}
		}
		public bool GridLinesMinorVisible {
			get { return Axis.GridLinesMinorVisible; }
			set {
				if (Axis.GridLinesMinorVisible != value) {
					Axis.GridLinesMinorVisible = value;
					OnPropertyChanged("GridLinesMinorVisible");
				}
			}
		}
		public bool TickmarksVisible {
			get {
				if (Axis is Axis2D)
					return ((Axis2D)Axis).TickmarksVisible;
				if (Axis is CircularAxisY2D)
					return ((CircularAxisY2D)Axis).TickmarksVisible;
				return false;
			}
			set {
				if (Axis is Axis2D && ((Axis2D)Axis).TickmarksVisible != value) {
					((Axis2D)Axis).TickmarksVisible = value;
					OnPropertyChanged("TickmarksVisible");
				}
				if (Axis is CircularAxisY2D) {
					((CircularAxisY2D)Axis).TickmarksVisible = value;
					OnPropertyChanged("TickmarksVisible");
				}
			}
		}
		public bool TickmarksMinorVisible {
			get {
				if (Axis is Axis2D)
					return ((Axis2D)Axis).TickmarksMinorVisible;
				if (Axis is CircularAxisY2D)
					return ((CircularAxisY2D)Axis).TickmarksMinorVisible;
				return false;
			}
			set {
				if (Axis is Axis2D && ((Axis2D)Axis).TickmarksMinorVisible != value) {
					((Axis2D)Axis).TickmarksMinorVisible = value;
					OnPropertyChanged("TickmarksMinorVisible");
				}
				if (Axis is CircularAxisY2D) {
					((CircularAxisY2D)Axis).TickmarksMinorVisible = value;
					OnPropertyChanged("TickmarksMinorVisible");
				}
			}
		}
		public bool Interlaced {
			get { return Axis.Interlaced; }
			set {
				if (Axis.Interlaced != value) {
					Axis.Interlaced = value;
					OnPropertyChanged("Interlaced");
				}
			}
		}
		public bool IsArgumentAxis {
			get { return !IsValuesAxis; }
		}
		public bool IsValuesAxis {
			get { return ChartDesignerPropertiesProvider.IsValuesAxis(Axis); }
		}
		public bool IsPrimaryAxis {
			get {
				if (Parent is WpfChartDiagramModel)
					return true;
				else
					return false;
			}
		}
		public WpfChartAxisModel(ChartModelElement parent, AxisBase axis, string axisName)
			: base(parent, axis) {
			this.axisName = axisName;
			if (Axis is Axis2D) {
				this.constantLinesCollectionModel = new WpfChartConstantLinesCollectionModel(this, ((Axis2D)Axis).ConstantLinesInFront);
				this.customAxisLabelCollectionModel = new WpfChartAxisCustomLabelCollectionModel(this, ((Axis2D)Axis).CustomLabels);
				this.stripCollectionModel = new WpfChartStripCollectionModel(this, ((Axis2D)Axis).Strips);
			}
			UpdateTitle();
			UpdateLabel();
			WpfChartDiagramModel diagramModel = (WpfChartDiagramModel)GetParent<WpfChartDiagramModel>();
			PropertyGridModel = WpfChartAxisBasePropertyGridModel.CreatePropertyGridModelForAxis(this);
		}
		void UpdateTitle() {
			if (Axis is Axis && (title == null || ((Axis)Axis).Title != title.Title))
				Title = new WpfChartAxisTitleModel(this, ((Axis)Axis).Title);
		}
		void UpdateLabel() {
			if (label == null || Axis.Label != label.Label)
				Label = new WpfChartAxisLabelModel(this, Axis.Label);
		}
		protected override void UpdateChildren() {
			UpdateTitle();
			UpdateLabel();
		}
		public override string ToString() {
			return axisName;
		}
		public int GetSelfIndex() {
			if (Parent is WpfChartDiagramModel)
				return -1;
			else
				if (Parent is WpfChartSecondaryAxesCollectionModel)
					return ((WpfChartSecondaryAxesCollectionModel)Parent).IndexOf(this);
			return -2;
		}
	}
	public class WpfChartAxisModelX : WpfChartAxisModel {
		public static WpfChartAxisModelX CreateForDiagramPrimaryAxis(ChartModelElement parent, Diagram diagram) {
			if (diagram is PolarDiagram2D)
				return new WpfChartAxisModelX(parent, ((PolarDiagram2D)diagram).ActualAxisX, ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.PrimaryAxisXName));
			else if (diagram is RadarDiagram2D)
				return new WpfChartAxisModelX(parent, ((RadarDiagram2D)diagram).ActualAxisX, ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.PrimaryAxisXName));
			else if (diagram is XYDiagram2D)
				return new WpfChartAxisModelX(parent, ((XYDiagram2D)diagram).ActualAxisX, ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.PrimaryAxisXName));
			else if (diagram is XYDiagram3D)
				return new WpfChartAxisModelX(parent, ((XYDiagram3D)diagram).ActualAxisX, ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.PrimaryAxisXName));
			return null;
		}
		public WpfChartAxisModelX(ChartModelElement parent, AxisBase axis, string axisName) : base(parent, axis, axisName) { }
	}
	public class WpfChartAxisModelY : WpfChartAxisModel {
		public static WpfChartAxisModelY CreateForDiagramPrimaryAxis(ChartModelElement parent, Diagram diagram) {
			if (diagram is PolarDiagram2D)
				return new WpfChartAxisModelY(parent, ((PolarDiagram2D)diagram).ActualAxisY, ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.PrimaryAxisYName));
			else if (diagram is RadarDiagram2D)
				return new WpfChartAxisModelY(parent, ((RadarDiagram2D)diagram).ActualAxisY, ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.PrimaryAxisYName));
			else if (diagram is XYDiagram2D)
				return new WpfChartAxisModelY(parent, ((XYDiagram2D)diagram).ActualAxisY, ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.PrimaryAxisYName));
			else if (diagram is XYDiagram3D)
				return new WpfChartAxisModelY(parent, ((XYDiagram3D)diagram).ActualAxisY, ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.PrimaryAxisYName));
			return null;
		}
		public WpfChartAxisModelY(ChartModelElement parent, AxisBase axis, string axisName) : base(parent, axis, axisName) { }
	}
}
