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

using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	public abstract class XYDiagramSeriesViewBase : XYDiagram2DSeriesViewBase, IShadowSupportView {
		readonly Shadow shadow;
		XYDiagramPaneBase pane;
		AxisXBase axisX;
		AxisYBase axisY;
		protected internal override XYDiagramPaneBase ActualPane {
			get { return pane; }
			set { pane = value; }
		}
		protected internal override Axis2D ActualAxisX {
			get { return axisX; }
			set { axisX = value as AxisXBase; }
		}
		protected internal override Axis2D ActualAxisY {
			get { return axisY; }
			set { axisY = value as AxisYBase; }
		}
		protected internal override bool HitTestingSupported { get { return true; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramSeriesViewBaseShadow"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagramSeriesViewBase.Shadow"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public Shadow Shadow { get { return shadow; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramSeriesViewBasePane"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagramSeriesViewBase.Pane"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(typeof(PaneTypeConverter)),
		Editor("DevExpress.XtraCharts.Design.PaneTypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public XYDiagramPaneBase Pane {
			get { return pane; }
			set {
				if (value != pane) {
					PropertyUpdateInfo updateInfo = new PropertyUpdateInfo(base.Owner, "Pane");
					CheckPane(value, false);
					SendNotification(new ElementWillChangeNotification(this));
					pane = value;
					RaiseControlChanged(updateInfo);
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramSeriesViewBaseAxisX"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagramSeriesViewBase.AxisX"),
		Editor("DevExpress.XtraCharts.Design.SeriesViewAxisXTypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		TypeConverter(typeof(AxisTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public AxisXBase AxisX {
			get { return axisX; }
			set {
				if (value != axisX) {
					CheckAxisX(value, false);
					SendNotification(new ElementWillChangeNotification(this));
					PropertyUpdateInfo<IAxisData> updateInfo = new PropertyUpdateInfo<IAxisData>(this, "AxisX", axisX, value);
					axisX = value;
					RaiseControlChanged(updateInfo);					
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramSeriesViewBaseAxisY"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagramSeriesViewBase.AxisY"),
		TypeConverter(typeof(AxisTypeConverter)),
		Editor("DevExpress.XtraCharts.Design.SeriesViewAxisYTypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public AxisYBase AxisY {
			get { return axisY; }
			set {
				if (value != axisY) {
					CheckAxisY(value, false);
					SendNotification(new ElementWillChangeNotification(this));
					PropertyUpdateInfo<IAxisData> updateInfo = new PropertyUpdateInfo<IAxisData>(this, "AxisY", axisY, value);
					axisY = value;
					RaiseControlChanged(updateInfo);
				}
			}
		}
		protected XYDiagramSeriesViewBase()
			: base() {
			shadow = new Shadow(this);
		}
		bool ShouldSerializeShadow() {
			return shadow.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeShadow();
		}
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "Shadow" ? ShouldSerializeShadow() : base.XtraShouldSerialize(propertyName);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			XYDiagramSeriesViewBase view = obj as XYDiagramSeriesViewBase;
			if (view != null) {
				pane = view.pane;
				axisX = view.axisX;
				axisY = view.axisY;
			}
			IShadowSupportView shadowView = obj as IShadowSupportView;
			if (shadowView != null)
				shadow.Assign(shadowView.Shadow);
		}
		public override bool Equals(object obj) {
			return base.Equals(obj) && shadow.Equals(((XYDiagramSeriesViewBase)obj).shadow);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
