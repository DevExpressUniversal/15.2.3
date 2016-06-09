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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(SideBySideStackedBar3DSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SideBySideStackedBar3DSeriesView : StackedBar3DSeriesView, ISideBySideStackedBarSeriesView, ISupportStackedGroup {
		object stackedGroup;
		protected override bool NeedSeriesGroupsInteraction { get { return true; } }
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnSideBySideStackedBar3D); } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty,
		NonTestableProperty
		]
		public string StackedGroupSerializable {
			get { return ObjectToStringConversion.ObjectToString(StackedGroup); }
			set { StackedGroup = ObjectToStringConversion.StringToObject(value); }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SideBySideStackedBar3DSeriesViewStackedGroup"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SideBySideStackedBar3DSeriesView.StackedGroup"),
		TypeConverter(typeof(ObjectTypeConverter)),
		Editor("DevExpress.XtraCharts.Design.SeriesGroupTypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty
		]
		public object StackedGroup {
			get { return stackedGroup; }
			set {
				if (!object.Equals(stackedGroup, value)) {
					SendNotification(new ElementWillChangeNotification(this));
					stackedGroup = value;
					RaiseControlChanged(new PropertyUpdateInfo(this.Owner, "StackedGroup"));
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SideBySideStackedBar3DSeriesViewBarDistance"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SideBySideStackedBar3DSeriesView.BarDistance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public double BarDistance {
			get { return Chart != null ? Chart.SideBySideBarDistance : SideBySideBarDefaults.DefaultBarDistance; }
			set {
				if (Chart == null)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectBarDistancePropertyUsing));
				Chart.SideBySideBarDistance = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SideBySideStackedBar3DSeriesViewBarDistanceFixed"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SideBySideStackedBar3DSeriesView.BarDistanceFixed"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public int BarDistanceFixed {
			get { return Chart != null ? Chart.SideBySideBarDistanceFixed : SideBySideBarDefaults.DefaultBarDistanceFixed; }
			set {
				if (Chart == null)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectBarDistanceFixedPropertyUsing));
				Chart.SideBySideBarDistanceFixed = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SideBySideStackedBar3DSeriesViewEqualBarWidth"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SideBySideStackedBar3DSeriesView.EqualBarWidth"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public bool EqualBarWidth {
			get { return Chart != null ? Chart.SideBySideEqualBarWidth : SideBySideBarDefaults.DefaultEqualBarWidth; }
			set {
				if (Chart == null)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectEqualBarWidthPropertyUsing));
				Chart.SideBySideEqualBarWidth = value;
			}
		}
		#region ISupportSeriesGroups implementation
		object ISupportSeriesGroups.SeriesGroup { get { return StackedGroup; } set { StackedGroup = value; } }
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeStackedGroupSerializable() {
			return StackedGroupSerializable != string.Empty;
		}
		bool ShouldSerializeStackedGroup() {
			return this.stackedGroup != null;
		}
		void ResetStackedGroup() {
			StackedGroup = null;
		}
		bool ShouldSerializeBarDistance() {
			return BarDistance != SideBySideBarDefaults.DefaultBarDistance;
		}
		void ResetBarDistance() {
			BarDistance = SideBySideBarDefaults.DefaultBarDistance;
		}
		bool ShouldSerializeBarDistanceFixed() {
			return BarDistanceFixed != SideBySideBarDefaults.DefaultBarDistanceFixed;
		}
		void ResetBarDistanceFixed() {
			BarDistanceFixed = SideBySideBarDefaults.DefaultBarDistanceFixed;
		}
		bool ShouldSerializeEqualBarWidth() {
			return EqualBarWidth != SideBySideBarDefaults.DefaultEqualBarWidth;
		}
		void ResetEqualBarWidth() {
			EqualBarWidth = SideBySideBarDefaults.DefaultEqualBarWidth;
		}
		#endregion
		#region XtraShouldSerialize
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "StackedGroupSerializable")
				return ShouldSerializeStackedGroupSerializable();
			if (propertyName == "BarDistance")
				return ShouldSerializeBarDistance();
			if (propertyName == "BarDistanceFixed")
				return ShouldSerializeBarDistanceFixed();
			if (propertyName == "EqualBarWidth")
				return ShouldSerializeEqualBarWidth();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new SideBySideStackedBar3DSeriesView();
		}
		protected override double GetBarWidth(IXYPoint refinedPoint) {
			return ((ISideBySidePoint)refinedPoint).BarWidth;
		}
		protected override double GetDisplayOffset(IXYPoint refinedPoint) {
			return ((ISideBySidePoint)refinedPoint).Offset;
		}
		protected override int GetFixedOffset(IXYPoint refinedPoint) {
			return ((ISideBySidePoint)refinedPoint).FixedOffset;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ISideBySideStackedBarSeriesView view = obj as ISideBySideStackedBarSeriesView;
			if (view == null)
				return;
			stackedGroup = view.StackedGroup;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			SideBySideStackedBar3DSeriesView view = (SideBySideStackedBar3DSeriesView)obj;
			return object.Equals(stackedGroup, view.stackedGroup);
		}
	}
}
