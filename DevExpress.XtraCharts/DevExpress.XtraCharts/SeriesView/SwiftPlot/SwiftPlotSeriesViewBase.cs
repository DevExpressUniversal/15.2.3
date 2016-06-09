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
using System.Drawing;
using System.Drawing.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	public abstract class SwiftPlotSeriesViewBase : XYDiagram2DSeriesViewBase {
		SwiftPlotDiagramAxisXBase axisX;
		SwiftPlotDiagramAxisYBase axisY;
		XYDiagramPaneBase pane;
		protected override CompatibleViewType CompatibleViewType { get { return CompatibleViewType.SwiftPlotView; } }
		protected internal override Axis2D ActualAxisX { get { return axisX; } set { axisX = value as SwiftPlotDiagramAxisXBase; } }
		protected internal override Axis2D ActualAxisY { get { return axisY; } set { axisY = value as SwiftPlotDiagramAxisYBase; } }
		protected internal override XYDiagramPaneBase ActualPane { get { return pane; } set { pane = value; } }
		protected internal override bool HitTestingSupported { get { return Chart.Container.DesignMode; } }
		protected internal override bool IsSupportedLabel { get { return false; } }
		protected internal override bool IsSupportedPointOptions { get { return false; } }
		protected internal override bool IsSupportedToolTips { get { return false; } }
		protected internal override bool DateTimeValuesSupported { get { return true; } }
		protected internal override bool ActualColorEach { get { return false; } }
		protected internal override bool ShouldCalculatePointsData { get { return false; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override Type DiagramType { get { return typeof(SwiftPlotDiagram); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SwiftPlotSeriesViewBaseAxisX"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SwiftPlotDiagramSeriesViewBase.AxisX"),
		TypeConverter(typeof(AxisTypeConverter)),
		Editor("DevExpress.XtraCharts.Design.SeriesViewAxisXTypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public SwiftPlotDiagramAxisXBase AxisX {
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
	DevExpressXtraChartsLocalizedDescription("SwiftPlotSeriesViewBaseAxisY"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SwiftPlotDiagramSeriesViewBase.AxisY"),
		TypeConverter(typeof(AxisTypeConverter)),
		Editor("DevExpress.XtraCharts.Design.SeriesViewAxisYTypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public SwiftPlotDiagramAxisYBase AxisY {
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
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SwiftPlotSeriesViewBasePane"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SwiftPlotDiagramSeriesViewBase.Pane"),
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
					CheckPane(value, false);
					SendNotification(new ElementWillChangeNotification(this));
					pane = value;
					RaiseControlChanged();
				}
			}
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return null;
		}
		protected internal override GraphicsCommand CreateGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			return null;
		}
		protected internal override void Render(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
		}
		protected internal override GraphicsCommand CreateShadowGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			return null;
		}
		protected internal override void RenderShadow(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
		}
		public override string GetValueCaption(int index) {
			if (index > 0)
				throw new IndexOutOfRangeException();
			return ChartLocalizer.GetString(ChartStringId.ValueMember);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			SwiftPlotSeriesViewBase view = obj as SwiftPlotSeriesViewBase;
			if (view == null)
				return;
			axisX = view.axisX;
			axisY = view.axisY;
			pane = view.pane;
		}
	}
}
