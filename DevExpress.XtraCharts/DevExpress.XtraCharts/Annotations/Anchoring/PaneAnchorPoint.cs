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
using System.Globalization;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	public abstract class AxisCoordinate : ChartElement {
		Axis2D axis;
		object axisValue;
		string deserializedAxisName;
		string deserializedAxisValue;
		internal PaneAnchorPoint AnchorPoint { get { return (PaneAnchorPoint)Owner; } }
		internal Axis2D ActualAxis { 
			get { return axis; } 
			set { 
				if (value != axis) {
					CheckAxis(value);
					SendNotification(new ElementWillChangeNotification(this));
					axis = value;
					RaiseControlChanged();
				}
			} 
		}
		internal bool Visible { get { return CustomAxisElementsHelper.IsAxisValueVisible((IAxisData)axis, axisValue); } }
		internal double Value { get { return axis.ScaleTypeMap.NativeToInternal(axisValue); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisCoordinateAxisValue"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisCoordinate.AxisValue"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(typeof(AxisValueTypeConverter)),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public object AxisValue {
			get { return axisValue; }
			set {
				if (value == null)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAxisCoordinateAxisValue));
				if (value != axisValue) {
					SendNotification(new ElementWillChangeNotification(this));
					SetAxisValue(axis, value);
					RaiseControlChanged();
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string AxisValueSerializable {
			get { return SerializingUtils.ConvertToSerializable(AxisValue); }
			set { 
				deserializedAxisValue = value;
				AxisValue = value; 
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string AxisName {
			get {
				if (axis != null)
					return axis.Name;
				Axis2D defaultAxis = GetDefaultAxis();
				return defaultAxis == null ? String.Empty : defaultAxis.Name;
			}
			set { deserializedAxisName = value; }
		}
		protected AxisCoordinate(PaneAnchorPoint owner) : base(owner) {
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeAxisName() {
			return axis != null && axis != GetDefaultAxis();
		}
		protected internal override bool ShouldSerialize() {
			return true;
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "AxisName":
					return ShouldSerializeAxisName();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		internal void SetAxisValue(Axis2D axis, object value) {
			this.axis = axis;
			axisValue = (axis == null || Loading) ? value : axis.ConvertBasedOnScaleType(value);
		}
		internal void OnEndLoading() {
			XYDiagram2D diagram = AnchorPoint.XYDiagram;
			if (diagram != null && !String.IsNullOrEmpty(deserializedAxisName)) {
				axis = FindAxisByName(deserializedAxisName);
				deserializedAxisName = String.Empty;
			}
			if (axis == null)
				axis = GetDefaultAxis();			
		}
		internal void UpdateAxisValue() {
			if (axis != null) {
				CultureInfo culture = (deserializedAxisValue != null && deserializedAxisValue.Equals(axisValue)) ? 
					CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;
				axisValue = ((IAxisData)axis).AxisScaleTypeMap.ConvertValue(axisValue, culture);
			}
		}
		protected abstract void CheckAxis(Axis2D axis);
		protected abstract Axis2D FindAxisByName(string axisName);
		protected abstract Axis2D GetDefaultAxis();
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			AxisCoordinate axisCoordinate = obj as AxisCoordinate;
			if (axisCoordinate != null) {
				deserializedAxisName = axisCoordinate.AxisName;
				axisValue = axisCoordinate.axisValue;
				deserializedAxisValue = axisCoordinate.deserializedAxisValue;
			}
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class AxisXCoordinate : AxisCoordinate {
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisXCoordinateAxis"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisXCoordinate.Axis"),
		DXDisplayNameIgnore(IgnoreRecursionOnly=true),
		TypeConverter(typeof(AxisXCoordinateAxisTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public Axis2D Axis {
			get { return ActualAxis; }
			set { ActualAxis = value; }
		}
		public AxisXCoordinate(PaneAnchorPoint owner) : base(owner) {
		}
		protected override ChartElement CreateObjectForClone() {
			return new AxisXCoordinate(null);
		}
		protected override void CheckAxis(Axis2D axis) {
			if (axis == null)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgNullAxisXCoordinateAxis));
			XYDiagram2D diagram = AnchorPoint.XYDiagram;
			if (diagram != null && axis != diagram.ActualAxisX && !diagram.ActualSecondaryAxesX.ContainsInternal(axis))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAxisXCoordinateAxis));
		}
		protected override Axis2D FindAxisByName(string axisName) {
			XYDiagram2D diagram = AnchorPoint.XYDiagram;
			return diagram == null ? null : diagram.FindAxisXByName(axisName);
		}
		protected override Axis2D GetDefaultAxis() {
			XYDiagram2D diagram = AnchorPoint.XYDiagram;
			return diagram == null ? null : diagram.ActualAxisX;
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class AxisYCoordinate : AxisCoordinate {
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisYCoordinateAxis"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisYCoordinate.Axis"),
		DXDisplayNameIgnore(IgnoreRecursionOnly = true),
		TypeConverter(typeof(AxisYCoordinateAxisTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public Axis2D Axis {
			get { return ActualAxis; }
			set { ActualAxis = value; }
		}
		public AxisYCoordinate(PaneAnchorPoint owner) : base(owner) {
		}
		protected override ChartElement CreateObjectForClone() {
			return new AxisYCoordinate(null);
		}
		protected override void CheckAxis(Axis2D axis) {
			if (axis == null)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgNullAxisYCoordinateAxis));
			XYDiagram2D diagram = AnchorPoint.XYDiagram;
			if (diagram != null && axis != diagram.ActualAxisY && !diagram.ActualSecondaryAxesY.ContainsInternal(axis))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAxisYCoordinateAxis));
		}
		protected override Axis2D FindAxisByName(string axisName) {
			XYDiagram2D diagram = AnchorPoint.XYDiagram;
			return diagram == null ? null : diagram.FindAxisYByName(axisName);
		}
		protected override Axis2D GetDefaultAxis() {
			XYDiagram2D diagram = AnchorPoint.XYDiagram;
			return diagram == null ? null : diagram.ActualAxisY;
		}
	}
	[
	TypeConverter(typeof(PaneAnchorPointTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class PaneAnchorPoint : AnnotationAnchorPoint {
		readonly AxisXCoordinate axisXCoordinate;
		readonly AxisYCoordinate axisYCoordinate;
		XYDiagramPaneBase pane;
		string deserializedPaneName;
		internal XYDiagram2D XYDiagram {
			get {
				Annotation annotation = Annotation;
				if (annotation == null)
					return null;
				Chart chart = annotation.Owner as Chart;
				return chart == null ? null : chart.Diagram as XYDiagram2D;
			}
		}
		protected internal override bool LabelModeSupported { get { return true; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PaneAnchorPointAxisXCoordinate"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PaneAnchorPoint.AxisXCoordinate"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(ExpandableObjectConverter)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public AxisXCoordinate AxisXCoordinate { get { return axisXCoordinate; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PaneAnchorPointAxisYCoordinate"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PaneAnchorPoint.AxisYCoordinate"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(ExpandableObjectConverter)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public AxisYCoordinate AxisYCoordinate { get { return axisYCoordinate; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PaneAnchorPointPane"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PaneAnchorPoint.Pane"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(typeof(PaneAnchorPointPaneTypeConverter)),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public XYDiagramPaneBase Pane {
			get { return pane; }
			set {
				if (value != pane) {
					SendNotification(new ElementWillChangeNotification(this));
					SetPane(value);
					RaiseControlChanged();
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string PaneName {
			get {
				if (pane != null)
					return pane.Name;
				XYDiagram2D diagram = XYDiagram;
				return diagram == null ? String.Empty : diagram.DefaultPane.Name;
			}
			set { deserializedPaneName = value; }
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeAxisXCoordinate() {
			return axisXCoordinate.ShouldSerialize();
		}
		bool ShouldSerializeAxisYCoordinate() {
			return axisYCoordinate.ShouldSerialize();
		}
		bool ShouldSerializePaneName() {
			if (pane == null)
				return false;
			XYDiagram2D diagram = XYDiagram;
			return diagram == null || pane != diagram.DefaultPane;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeAxisXCoordinate() || ShouldSerializeAxisYCoordinate() || ShouldSerializePaneName();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "AxisXCoordinate":
					return ShouldSerializeAxisXCoordinate();
				case "AxisYCoordinate":
					return ShouldSerializeAxisYCoordinate();
				case "PaneName":
					return ShouldSerializePaneName();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		public PaneAnchorPoint() {
			axisXCoordinate = new AxisXCoordinate(this);
			axisYCoordinate = new AxisYCoordinate(this);
		}
		internal void SetPane(XYDiagramPaneBase pane) {
			if (pane == null)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgNullPaneAnchorPointPane));
			XYDiagram2D diagram = XYDiagram;
			if (diagram != null && pane != diagram.DefaultPane && !diagram.Panes.Contains(pane))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPaneAnchorPointPane));
			this.pane = pane;
		}
		internal void SetPosition(XYDiagramPaneBase pane, Axis2D axisX, Axis2D axisY, object axisXValue, object axisYValue) {
			this.pane = pane;
			axisXCoordinate.SetAxisValue(axisX, axisXValue);
			axisYCoordinate.SetAxisValue(axisY, axisYValue);
		}
		protected override ChartElement CreateObjectForClone() {
			return new PaneAnchorPoint();
		}
		protected internal override void OnEndLoading() {
			base.OnEndLoading();
			XYDiagram2D diagram = XYDiagram;
			if (diagram != null) {
				if (!String.IsNullOrEmpty(deserializedPaneName)) {
					pane = diagram.FindPaneByName(deserializedPaneName);
					deserializedPaneName = String.Empty;
				}
				if (pane == null)
					pane = diagram.DefaultPane;
			}
			axisXCoordinate.OnEndLoading();
			axisYCoordinate.OnEndLoading();
		}
		protected internal override DiagramPoint GetAnchorPoint(ZPlaneRectangle viewport) {
			return new DiagramPoint(0, 0);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			PaneAnchorPoint anchorPoint = obj as PaneAnchorPoint;
			if (anchorPoint != null) {
				axisXCoordinate.Assign(anchorPoint.axisXCoordinate);
				axisYCoordinate.Assign(anchorPoint.axisYCoordinate);
				PaneName = anchorPoint.PaneName;
			}
		}
	}
}
