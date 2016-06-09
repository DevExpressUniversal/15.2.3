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
using System.Reflection;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(SeriesPointAnchorPointTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SeriesPointAnchorPoint : AnnotationAnchorPoint, ISupportInitialize, IXtraSerializable {
		const int defaultID = -1;
		Series series = null;
		SeriesPoint point = null;
		int seriesPointID = defaultID;
		int seriesID = defaultID;
		bool loading = false;
		protected internal override bool Loading { get { return loading || base.Loading; } }
		Chart Chart { get { return ChartContainer.Chart; } }
		Series Series {
			get { if (series != null)
				return series;
				return Chart != null ? Chart.Series.GetSeriesByID(seriesID) : null; }
			set { series = value; }
		}
		protected internal override bool LabelModeSupported { 
			get {
				return Series != null && Series.View.AnnotationLabelModeSupported;
			} 
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public int SeriesID {
			get { return series != null ? series.SeriesID : seriesID; }
			set {
				if (!Loading) 
					throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgInternalPropertyChangeError));
				seriesID = value;
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public int SeriesPointID {
			get { return point != null ? point.SeriesPointID : seriesPointID; }
			set {
				if (!Loading) 
					throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgInternalPropertyChangeError));
				seriesPointID = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesPointAnchorPointSeriesPoint"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesPointAnchorPoint.SeriesPoint"),
		Editor("DevExpress.XtraCharts.Design.AnchorPointSeriesPointEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(typeof(ExpandableObjectConverter)),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public SeriesPoint SeriesPoint {
			get {
				if (point != null)
					return point;
				return Series != null ? Series.Points.GetByID(seriesPointID) : null;
			}
			set {
				CheckSeriesPoint(value);
				SendNotification(new ElementWillChangeNotification(this));
				SetSeriesPoint(value);
				RaiseControlChanged();
			}
		}
		public SeriesPointAnchorPoint() { 
		}
		public SeriesPointAnchorPoint(SeriesPoint point) {
			CheckSeriesPoint(point);
			SetSeriesPoint(point);
		}
		#region ISupportInitialize implementation
		void ISupportInitialize.BeginInit() {
			this.loading = true;
		}
		void ISupportInitialize.EndInit() {
			this.loading = false;
		}
		#endregion
		#region XtraSerializing
		void IXtraSerializable.OnStartSerializing() {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			((ISupportInitialize)this).BeginInit();
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			((ISupportInitialize)this).EndInit();
		}
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "SeriesPointID":
					return ShouldSerializeSeriesPointID();
				case "SeriesID":
					return ShouldSerializeSeriesID();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeSeriesID() {
			return SeriesID != defaultID;
		}
		bool ShouldSerializeSeriesPointID() {
			return SeriesPointID != defaultID;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeSeriesPointID() ||
				ShouldSerializeSeriesID();
		}
		#endregion
		void CheckSeriesPoint(SeriesPoint point) {
			if (point == null)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgNullAnchorPointSeriesPoint));
			if (point.Series == null || point.Series.Owner == null)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAnchorPointSeriesPoint));
		}
		protected override ChartElement CreateObjectForClone() {
			return new SeriesPointAnchorPoint();
		}
		protected internal override DiagramPoint GetAnchorPoint(ZPlaneRectangle viewport) {
			return new DiagramPoint(0, 0);
		}
		internal void SetSeriesPoint(SeriesPoint point) {
			this.point = point;
			Series = point.Series;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			SeriesPointAnchorPoint anchorPoint = obj as SeriesPointAnchorPoint;
			if (anchorPoint == null)
				return;
			seriesPointID = anchorPoint.SeriesPointID;
			seriesID = anchorPoint.SeriesID;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class AnchorPointSeriesPointItem : ICustomTypeDescriptor {
		#region Inner Class 
		class ArgumentPropertyDescriptor : PropertyDescriptor {
			public override Type ComponentType { get { return typeof(AnchorPointSeriesPointItem); } }
			public override Type PropertyType { get { return typeof(string); } }
			public override bool IsReadOnly { get { return true; } }
			public override bool IsBrowsable { get { return true; } }
			public ArgumentPropertyDescriptor() : base(ChartLocalizer.GetString(ChartStringId.ArgumentMember), new Attribute[] { }) {
			}
			public override object GetValue(object component) {
				AnchorPointSeriesPointItem item = (AnchorPointSeriesPointItem)component;
				Series series = item.Point.Series;
				if (series.ActualArgumentScaleType == ScaleType.DateTime) {
					string format = "G";
					if (series.Label != null)
						format = PatternUtils.GetArgumentFormat(series.Label.ActualTextPattern);
					return PatternUtils.Format(item.Point.DateTimeArgument, format);
				}
				return item.Point.Argument;
			}
			public override void SetValue(object component, object value) {			   
			}
			public override bool CanResetValue(object component) {
				return false;
			}
			public override void ResetValue(object component) {				
			}
			public override bool ShouldSerializeValue(object component) {
				return false;
			}
		}
		class ValuePropertyDescriptor : PropertyDescriptor {
			int index;
			public override Type ComponentType { get { return typeof(AnchorPointSeriesPointItem); } }
			public override Type PropertyType { get { return typeof(string); } }
			public override bool IsReadOnly { get { return true; } }
			public override bool IsBrowsable { get { return true; } }
			public ValuePropertyDescriptor(string name, int index) : base(name, new Attribute[] { }) {
				this.index = index;
			}
			public override object GetValue(object component) {
				AnchorPointSeriesPointItem item = (AnchorPointSeriesPointItem)component;
				return item.Point.IsEmpty ? String.Empty : item.Point.GetValueString(index);
			}
			public override void SetValue(object component, object value) {				
			}
			public override bool CanResetValue(object component) {
				return false;
			}
			public override void ResetValue(object component) {				
			}
			public override bool ShouldSerializeValue(object component) {
				return false;
			}
		}
		#endregion
		SeriesPoint point;
		public SeriesPoint Point { get { return point; } }
		public AnchorPointSeriesPointItem(SeriesPoint point) {
			this.point = point;
		}
		#region ICustomTypeDescriptor Members
		System.ComponentModel.AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return TypeDescriptor.GetAttributes(this, true);
		}
		System.ComponentModel.TypeConverter ICustomTypeDescriptor.GetConverter() {
			return TypeDescriptor.GetConverter(this, true);
		}
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			return GetProperties();
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return GetProperties();
		}
		string ICustomTypeDescriptor.GetClassName() {
			return GetType().Name;
		}
		string ICustomTypeDescriptor.GetComponentName() {
			return GetType().Name;
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
			return TypeDescriptor.GetEvents(this, true);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			return this;
		}
		#endregion
		PropertyDescriptorCollection GetProperties() {
			if (point == null || point.Series == null || point.Series.View == null)
				return new PropertyDescriptorCollection(new PropertyDescriptor[0]);
			SeriesViewBase view = point.Series.View;
			int dimension = view.PointDimension;
			PropertyDescriptor[] desc = new PropertyDescriptor[dimension + 1];
			desc[0] = new ArgumentPropertyDescriptor();
			for (int i = 0; i < dimension; i++)
				desc[i + 1] = new ValuePropertyDescriptor(view.GetValueCaption(i), i);		   
			return new PropertyDescriptorCollection(desc);
		}
		public override string ToString() {
			return "(SeriesPoint)";
		}		
	};
	public class SeriesPointPropertyDescriptor : PropertyDescriptor {
		public override Type ComponentType { get { return typeof(SeriesPointAnchorPoint); } }
		public override Type PropertyType { get { return typeof(SeriesPoint); } }
		public override bool IsReadOnly { get { return false; } }
		public override bool IsBrowsable { get { return true; } }
		public SeriesPointPropertyDescriptor(string name ,Attribute[] attributes) : base(name, attributes) {
		}
		public override object GetValue(object component) {
			SeriesPointAnchorPoint anchorPoint = component as SeriesPointAnchorPoint;
			return anchorPoint != null ? new AnchorPointSeriesPointItem(anchorPoint.SeriesPoint) : null;
		}
		public override void SetValue(object component, object value) {
			SeriesPointAnchorPoint anchorPoint = component as SeriesPointAnchorPoint;
			if (anchorPoint != null) 
				anchorPoint.SeriesPoint = value as SeriesPoint;								
		}
		public override bool CanResetValue(object component) {
			return false;
		}
		public override void ResetValue(object component) {			
		}
		public override bool ShouldSerializeValue(object component) {
			return false;
		}		
	}
	public abstract class AnnotationAnchorPointLayoutList : List<AnnotationLayout> {
		readonly RefinedSeriesData seriesData;
		public RefinedSeriesData SeriesData { get { return seriesData; } }
		public AnnotationAnchorPointLayoutList(RefinedSeriesData seriesData) {
			this.seriesData = seriesData;
		}
	}
	public class XYDiagramAnchorPointLayoutList : AnnotationAnchorPointLayoutList {
		readonly XYDiagramMappingContainer mappingContainer;
		public XYDiagramMappingContainer MappingContainer { get { return mappingContainer; } }
		public XYDiagram2DSeriesViewBase View { get { return (XYDiagram2DSeriesViewBase)SeriesData.Series.View; } }
		public XYDiagramAnchorPointLayoutList(RefinedSeriesData seriesData, XYDiagramMappingContainer mappingContainer) : base(seriesData) {
			this.mappingContainer = mappingContainer;
			View.CalculateAnnotationsAnchorPointsLayout(this);
		}
		public XYDiagramMappingBase GetMapping(double argument, double value, bool scrollingSupported) {
			return scrollingSupported && View.IsScrollingEnabled ? mappingContainer.MappingForScrolling : mappingContainer.GetMapping(argument, value);
		}
	}
	public class RadarDiagramAnchorPointLayoutList : AnnotationAnchorPointLayoutList {
		readonly RadarDiagramMapping diagramMapping;
		RadarSeriesViewBase View { get { return (RadarSeriesViewBase)SeriesData.Series.View; } }
		public RadarDiagramMapping DiagramMapping { get { return diagramMapping; } }		
		public RadarDiagramAnchorPointLayoutList(RefinedSeriesData seriesData, RadarDiagramMapping diagramMapping)
			: base(seriesData) {			
			this.diagramMapping = diagramMapping;
			View.CalculateAnnotationsAchorPointsLayout(this);
		}
	}
	public class XYDiagram3DAnchorPointLayoutList : AnnotationAnchorPointLayoutList {
		readonly IAxisRangeData axisRangeY;
		readonly XYDiagram3DCoordsCalculator coordsCalculator;
		XYDiagram3DSeriesViewBase View { get { return (XYDiagram3DSeriesViewBase)SeriesData.Series.View; } }
		public IAxisRangeData AxisRangeY { get { return axisRangeY; } }
		public XYDiagram3DCoordsCalculator CoordsCalculator { get { return coordsCalculator; } }
		public XYDiagram3DAnchorPointLayoutList(RefinedSeriesData seriesData, XYDiagram3DCoordsCalculator coordsCalculator, IAxisRangeData axisRangeY)
			: base(seriesData) {
			this.axisRangeY = axisRangeY;
			this.coordsCalculator = coordsCalculator;
			View.CalculateAnnotationsAnchorPointsLayout(this);
		}
	}
}
