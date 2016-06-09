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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Drawing2D;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.Utils;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(RadarLineSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RadarLineSeriesView : RadarPointSeriesView, ILineSeriesView, IGeometryStripCreator, IGeometryHolder, ICustomTypeDescriptor {
		const bool DefaultLineAntialiasing = true;
		const int DefaultMarkerSize = 10;
		const bool DefaultClosed = true;
		const DefaultBoolean DefaultMarkerVisibility = DefaultBoolean.Default;
		LineStyle lineStyle;
		bool closed = DefaultClosed;
		DefaultBoolean markerVisibility = DefaultMarkerVisibility;
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnRadarLine); } }
		protected internal virtual int DefaultLineThickness { get { return 2; } }
		protected internal override bool ActualMarkerVisible {
			get {
				if (MarkerVisibility == DefaultBoolean.Default)
					return true;
				else
					return MarkerVisibility == DefaultBoolean.True;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarLineSeriesViewLineStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarLineSeriesView.LineStyle"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public LineStyle LineStyle { get { return lineStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarLineSeriesViewClosed"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarLineSeriesView.Closed"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool Closed {
			get { return closed; }
			set {
				if (closed == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				closed = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarLineSeriesViewLineMarkerOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.LineSeriesView.LineMarkerOptions"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public Marker LineMarkerOptions { get { return (Marker)base.PointMarkerOptions; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty(),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new SimpleMarker PointMarkerOptions { get { return base.PointMarkerOptions; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarLineSeriesViewMarkerVisibility"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarLineSeriesView.MarkerVisibility"),
		TypeConverter(typeof(DefaultBooleanConverter)),
		Category(Categories.Appearance),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public DefaultBoolean MarkerVisibility {
			get { return markerVisibility; }
			set {
				if (value != markerVisibility) {
					SendNotification(new ElementWillChangeNotification(this));
					markerVisibility = value;
					RaiseControlChanged();
				}
			}
		}
		public RadarLineSeriesView() {
			lineStyle = new LineStyle(this, DefaultLineThickness, DefaultLineAntialiasing);
		}
		#region ICustomTypeDescriptor implementation
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
			return new LineSeriesViewPropertyDescriptorCollection(TypeDescriptor.GetProperties(this, true));
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return new LineSeriesViewPropertyDescriptorCollection(TypeDescriptor.GetProperties(this, true));
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
		class LineSeriesViewPropertyDescriptorCollection : PropertyDescriptorCollection {
			public LineSeriesViewPropertyDescriptorCollection(ICollection descriptors) : base(new PropertyDescriptor[] { }) {
				foreach (PropertyDescriptor pd in descriptors)
					if (pd.DisplayName == "PointMarkerOptions")
						Add(new CustomPropertyDescriptor(pd, false));
					else
						Add(pd);
			}
		}
		#endregion
		#region ILineSeriesView implementation
		LineStyle ILineSeriesView.LineStyle { get { return LineStyle; } }
		bool ILineSeriesView.MarkerVisible { get { return ActualMarkerVisible; } }
		#endregion
		#region IGeometryStripCreator implementation
		IGeometryStrip IGeometryStripCreator.CreateStrip() {
			return new LineStrip();
		}
		#endregion
		#region IGeometryHolder
		GeometryStripCreator IGeometryHolder.CreateStripCreator() {
			return CreateStripCreator();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "LineStyle":
					return ShouldSerializeLineStyle();
				case "Closed":
					return ShouldSerializeClosed();
				case "LineMarkerOptions":
					return ShouldSerializeLineMarkerOptions();
				case "MarkerVisibility":
					return ShouldSerializeMarkerVisibility();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeLineStyle() {
			return LineStyle.ShouldSerialize();
		}
		bool ShouldSerializeClosed() {
			return closed != DefaultClosed;
		}
		void ResetClosed() {
			Closed = DefaultClosed;
		}
		bool ShouldSerializeLineMarkerOptions() {
			return LineMarkerOptions.ShouldSerialize();
		}
		bool ShouldSerializeMarkerVisibility() {
			return markerVisibility != DefaultMarkerVisibility;
		}
		void ResetMarkerVisibility() {
			MarkerVisibility = DefaultMarkerVisibility;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeLineStyle() ||
				ShouldSerializeClosed() ||
				ShouldSerializeLineMarkerOptions() ||
				ShouldSerializeMarkerVisibility();
		}
		#endregion
		protected override PointSeriesViewPainter CreatePainter() {
			return new LineSeriesViewPainter(this);
		}
		protected override ChartElement CreateObjectForClone() {
			return new RadarLineSeriesView();
		}
		protected override SimpleMarker CreateMarker() {
			return new Marker(this, DefaultMarkerSize);
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new LineDrawOptions(this);
		}
		protected virtual GeometryStripCreator CreateStripCreator() {
			return new LineGeometryStripCreator(Closed);
		}
		protected int GetLineThickness(LineDrawOptions drawOptions, SelectionState selectionState) {
			int lineThickness = drawOptions.LineStyle.Thickness;
			if (selectionState != SelectionState.Normal)
				lineThickness += 2;
			return lineThickness;
		}
		internal override SeriesHitTestState CreateHitTestState() {
			return new SeriesHitTestState();
		}
		protected internal override WholeSeriesViewData CalculateWholeSeriesViewData(RefinedSeriesData seriesData, GeometryCalculator geometryCalculator) {
			return new LineAndAreaWholeSeriesViewData(geometryCalculator.CreateStrips(seriesData.RefinedSeries));
		}
		protected internal override WholeSeriesLayout CalculateWholeSeriesLayout(RadarDiagramMapping diagramMapping, RadarDiagramSeriesLayout seriesLayout) {
			LineAndAreaWholeSeriesViewData wholeViewData = (LineAndAreaWholeSeriesViewData)seriesLayout.WholeViewData;
			List<IGeometryStrip> mappedStrips = StripsUtils.MapLineStrips(diagramMapping, wholeViewData.Strips);
			return new RadarLineWholeSeriesLayout(seriesLayout, mappedStrips, GetLineThickness((LineDrawOptions)seriesLayout.SeriesData.DrawOptions, seriesLayout.SeriesData.SelectionState), Closed);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			RadarLineSeriesView radarLineView = obj as RadarLineSeriesView;
			if (radarLineView != null) {
				closed = radarLineView.Closed;
				markerVisibility = radarLineView.markerVisibility;
			}
			ILineSeriesView view = obj as ILineSeriesView;
			if (view != null) {
				lineStyle.Assign(view.LineStyle);
				lineStyle.SetAntialiasing(DefaultLineAntialiasing);
			}
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			RadarLineSeriesView view = (RadarLineSeriesView)obj;
			return closed == view.Closed && view.LineStyle.Equals(view.LineStyle);
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class RadarLineWholeSeriesLayout : LineWholeSeriesLayout {
		readonly bool closed;
		public RadarLineWholeSeriesLayout(SeriesLayout seriesLayout, List<IGeometryStrip> strips, int lineThickness, bool closed) : base(seriesLayout, strips, lineThickness, Rectangle.Empty, true) {
			this.closed = closed;
		}
		protected override void AddStrips(GraphicsPath path) {
			foreach (LineStrip strip in Strips) {
				LineStrip uniqueStrip = strip.CreateUniqueStrip();
				if (!uniqueStrip.IsEmpty)
					using (GraphicsPath stripPath = new GraphicsPath()) {
						FillPath(uniqueStrip, stripPath);
						path.AddPath(stripPath, false);
					}
			}
		}
		protected override void FillPath(LineStrip uniqueStrip, GraphicsPath stripPath) {
			base.FillPath(uniqueStrip, stripPath);
			if (closed)
				stripPath.AddLine(new PointF((float)uniqueStrip[0].X, (float)uniqueStrip[0].Y),
					new PointF((float)uniqueStrip[uniqueStrip.Count - 1].X, (float)uniqueStrip[uniqueStrip.Count - 1].Y));
		}
	}
}
