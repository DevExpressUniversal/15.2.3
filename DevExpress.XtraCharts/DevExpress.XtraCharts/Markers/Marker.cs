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

using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using System;
using DevExpress.Utils;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class Marker : SimpleMarker {
		static readonly Color defaultColor = Color.Empty;
		Color color = defaultColor;
		bool ActualVisible {
			get {
				RangeAreaSeriesView rangeAreaView = Owner as RangeAreaSeriesView;
				if (rangeAreaView != null) {
					if (this == rangeAreaView.Marker1)
						return rangeAreaView.ActualMarker1Visible;
					if (this == rangeAreaView.Marker2)
						return rangeAreaView.ActualMarker2Visible;
				}
				else {
					LineSeriesView view = Owner as LineSeriesView;
					if (view != null)
						return view.ActualMarkerVisible;
					else {
						RadarLineSeriesView radarView = Owner as RadarLineSeriesView;
						if (radarView != null)
							return radarView.ActualMarkerVisible;
						else {
							RangeBarSeriesView rangeBarView = Owner as RangeBarSeriesView;
							if (rangeBarView != null) {
								if (this == rangeBarView.MinValueMarker)
									return rangeBarView.ActualMinValueMarkerVisible;
								if (this == rangeBarView.MaxValueMarker)
									return rangeBarView.ActualMaxValueMarkerVisible;
							}
						}
					}
				}
				return false;
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("The Marker.Visible property is now obsolete. Use the corresponding marker visibility properties of series view objects that support series point markers instead."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public bool Visible {
			get { return ActualVisible; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				RangeAreaSeriesView rangeAreaView = Owner as RangeAreaSeriesView;
				if (rangeAreaView != null) {
					if (this == rangeAreaView.Marker1)
						rangeAreaView.Marker1Visibility = value ? DefaultBoolean.True : DefaultBoolean.False;
					if (this == rangeAreaView.Marker2)
						rangeAreaView.Marker2Visibility = value ? DefaultBoolean.True : DefaultBoolean.False;
				}
				else {
					LineSeriesView view = Owner as LineSeriesView;
					if (view != null)
						view.MarkerVisibility = value ? DefaultBoolean.True : DefaultBoolean.False;
					else {
						RadarLineSeriesView radarView = Owner as RadarLineSeriesView;
						if (radarView != null)
							radarView.MarkerVisibility = value ? DefaultBoolean.True : DefaultBoolean.False;
						else {
							RangeBarSeriesView rangeBarView = Owner as RangeBarSeriesView;
							if (rangeBarView != null) {
								if (this == rangeBarView.MinValueMarker)
									rangeBarView.MinValueMarkerVisibility = value ? DefaultBoolean.True : DefaultBoolean.False;
								if (this == rangeBarView.MaxValueMarker)
									rangeBarView.MaxValueMarkerVisibility = value ? DefaultBoolean.True : DefaultBoolean.False;
							}
						}
					}
				}
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("MarkerColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Marker.Color"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public Color Color {
			get { return this.color; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				this.color = value;
				RaiseControlChanged();
			}
		}
		internal Marker(ChartElement owner, int defaultSize) : base(owner, defaultSize) {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "Color")
				return ShouldSerializeColor();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeColor() {
			return !this.color.Equals(defaultColor);
		}
		void ResetColor() {
			Color = defaultColor;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeColor();
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new Marker(null, 0);
		}
		protected override internal IPolygon CalculatePolygon(GRealPoint2D origin, bool isSelected) {
			return base.CalculatePolygon(origin, isSelected);
		}
		protected override internal IPolygon CalculatePolygon(Rectangle bounds) {
			return base.CalculatePolygon(bounds);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			Marker marker = obj as Marker;
			if (marker == null)
				return;
			this.color = marker.color;
		}
		public override bool Equals(object obj) {
			Marker marker = obj as Marker;
			return marker != null &&
				base.Equals(marker) &&
				this.color.Equals(marker.color);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
