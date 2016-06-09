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

using DevExpress.XtraCharts.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using DevExpress.Utils.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	[HasOptionsControl]
	public abstract class SeriesViewModelBase : DesignerChartElementModelBase {
		readonly SeriesViewBase seriesView;
		SeriesBase Series { get { return Parent != null ? ((DesignerSeriesModelBase)Parent).SeriesBase : null; } }
		protected SeriesViewBase SeriesView { get { return seriesView; } }
		protected ScaleType[] ValueScaleTypes {
			get {
				return Series != null ? new ScaleType[] { Series.ValueScaleType } : new ScaleType[0];
			}
		}
		protected internal override bool HasOptionsControl { get { return true; } }
		protected internal override ChartElement ChartElement { get { return SeriesView; } }
		protected internal override string ChartTreeImageKey { get { return seriesView.ToString(); } }
		[PropertyForOptions(-1, "Appearance")]
		public Color Color {
			get { return seriesView.Color; }
			set { SetProperty("Color", value); }
		}
		protected SeriesViewModelBase(SeriesViewBase seriesView, CommandManager commandManager)
			: base(commandManager) {
			this.seriesView = seriesView;
		}
		public override List<DataMemberInfo> GetDataMembersInfo() {
			DesignerSeriesModelBase seriesModel = Parent as DesignerSeriesModelBase;
			List<DataMemberInfo> dataMembersInfo = new List<DataMemberInfo>();
			if (seriesModel != null)
				dataMembersInfo.Add(new DataMemberInfo("ValueDataMember", "Value", seriesModel.ValueDataMembers[0], ValueScaleTypes));
			return dataMembersInfo;
		}
	}
	public abstract class XYDiagram2DViewBaseModel : SeriesViewModelBase {
		readonly IndicatorCollectionModel indicatorModels;
		RangeControlOptionsModel rangeControlOptionsModel;
		protected new XYDiagram2DSeriesViewBase SeriesView { get { return (XYDiagram2DSeriesViewBase)base.SeriesView; } }
		[Browsable(false)] 
		public IndicatorCollectionModel Indicators { get { return indicatorModels; } }
		public RangeControlOptionsModel RangeControlOptions { get { return rangeControlOptionsModel; } }
		public XYDiagram2DViewBaseModel(XYDiagram2DSeriesViewBase seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
			this.indicatorModels = new IndicatorCollectionModel(SeriesView.Indicators, CommandManager, null);
		}
		protected override void AddChildren() {
			Children.Add(indicatorModels);
			if (rangeControlOptionsModel != null)
				Children.Add(rangeControlOptionsModel);
			base.AddChildren();
		}
		public override void Update() {
			indicatorModels.Update();
			this.rangeControlOptionsModel = new RangeControlOptionsModel(SeriesView.RangeControlOptions, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	public abstract class XYDiagram3DViewBaseModel : SeriesViewModelBase {
		protected new XYDiagram3DSeriesViewBase SeriesView { get { return (XYDiagram3DSeriesViewBase)base.SeriesView; } }
		public byte Transparency {
			get { return SeriesView.Transparency; }
			set { SetProperty("Transparency", value); }
		}
		public XYDiagram3DViewBaseModel(XYDiagram3DSeriesViewBase seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	public abstract class XYDiagramViewBaseModel : XYDiagram2DViewBaseModel {
		ShadowModel shadowModel;
		XYDiagramPaneBaseModel paneModel;
		AxisXBaseModel axisXModel;
		AxisYBaseModel axisYModel;
		protected new XYDiagramSeriesViewBase SeriesView { get { return (XYDiagramSeriesViewBase)base.SeriesView; } }
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public ShadowModel Shadow { get { return shadowModel; } }
		[Editor("DevExpress.XtraCharts.Designer.Native.PaneModelUITypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		PropertyForOptions("Layout",1),
		UseAsSimpleProperty
		]
		public XYDiagramPaneBaseModel Pane {
			get { return paneModel; }
			set { SetProperty("Pane", value); }
		}
		[Editor("DevExpress.XtraCharts.Designer.Native.AxisXModelUITypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		PropertyForOptions("Layout", 1),
		UseAsSimpleProperty
		]
		public AxisXBaseModel AxisX {
			get { return axisXModel; }
			set { SetProperty("AxisX", value); }
		}
		[Editor("DevExpress.XtraCharts.Designer.Native.AxisYModelUITypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		PropertyForOptions("Layout", 1),
		UseAsSimpleProperty
		]
		public AxisYBaseModel AxisY {
			get { return axisYModel; }
			set { SetProperty("AxisY", value); }
		}
		public XYDiagramViewBaseModel(XYDiagramSeriesViewBase seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		protected override void AddChildren() {
			if (shadowModel != null)
				Children.Add(shadowModel);
			base.AddChildren();
		}
		public override void Update() {
			this.shadowModel = new ShadowModel(SeriesView.Shadow, CommandManager);
			DesignerChartModel chartModel = FindParent<DesignerChartModel>();
			if (chartModel == null)
				return;
			if (SeriesView.Pane != null)
				this.paneModel = (XYDiagramPaneBaseModel)chartModel.FindElementModel(SeriesView.Pane);
			else
				this.paneModel = null;
			if (SeriesView.AxisX != null)
				this.axisXModel = (AxisXBaseModel)chartModel.FindElementModel(SeriesView.AxisX);
			else
				this.axisXModel = null;
			if (SeriesView.AxisY != null)
				this.axisYModel = (AxisYBaseModel)chartModel.FindElementModel(SeriesView.AxisY);
			else
				this.axisYModel = null; ClearChildren();
			AddChildren();
			base.Update();
		}
		protected override void ProcessMessage(ViewMessage message) {
			if (message.Name == "Pane")
				SetProperty(message.Name, ((XYDiagramPaneBaseModel)message.Value).Pane);
			else if (message.Name == "AxisX")
				SetProperty(message.Name, ((AxisXBaseModel)message.Value).Axis);
			else if (message.Name == "AxisY")
				SetProperty(message.Name, ((AxisYBaseModel)message.Value).Axis);
			else
				base.ProcessMessage(message);
		}
	}
	public abstract class ColorEachSupportViewBaseModel : XYDiagramViewBaseModel {
		protected new SeriesViewColorEachSupportBase SeriesView { get { return (SeriesViewColorEachSupportBase)base.SeriesView; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool ColorEach {
			get { return SeriesView.ColorEach; }
			set { SetProperty("ColorEach", value); }
		}
		[
		PropertyForOptions(-1, "Appearance"),
		DependentUpon("ColorEach")]
		public new Color Color {
			get { return base.Color; }
			set { base.Color = value; }
		}
		public ColorEachSupportViewBaseModel(SeriesViewColorEachSupportBase seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	public abstract class View3DColorEachSupportBaseModel : XYDiagram3DViewBaseModel {
		protected new SeriesView3DColorEachSupportBase SeriesView { get { return (SeriesView3DColorEachSupportBase)base.SeriesView; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool ColorEach {
			get { return SeriesView.ColorEach; }
			set { SetProperty("ColorEach", value); }
		}
		[
		PropertyForOptions(-1, "Appearance"),
		DependentUpon("ColorEach")]
		public new Color Color {
			get { return base.Color; }
			set { base.Color = value; }
		}
		public View3DColorEachSupportBaseModel(SeriesView3DColorEachSupportBase seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	public abstract class SwiftPlotViewBaseModel : XYDiagram2DViewBaseModel {
		XYDiagramPaneBaseModel paneModel;
		protected new SwiftPlotSeriesViewBase SeriesView { get { return (SwiftPlotSeriesViewBase)base.SeriesView; } }
		SwiftPlotDiagramAxisXBaseModel axisXModel;
		SwiftPlotDiagramAxisYBaseModel axisYModel;
		[Editor("DevExpress.XtraCharts.Designer.Native.PaneModelUITypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		PropertyForOptions("Layout", 1),
		UseAsSimpleProperty
		]
		public XYDiagramPaneBaseModel Pane {
			get { return paneModel; }
			set { SetProperty("Pane", value); }
		}
		[Editor("DevExpress.XtraCharts.Designer.Native.SwiftPlotAxisXModelUITypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		PropertyForOptions("Layout", 1),
		UseAsSimpleProperty
		]
		public SwiftPlotDiagramAxisXBaseModel AxisX {
			get { return axisXModel; }
			set { SetProperty("AxisX", value); }
		}
		[Editor("DevExpress.XtraCharts.Designer.Native.SwiftPlotAxisYModelUITypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		PropertyForOptions("Layout", 1),
		UseAsSimpleProperty
		]
		public SwiftPlotDiagramAxisYBaseModel AxisY {
			get { return axisYModel; }
			set { SetProperty("AxisY", value); }
		}
		public SwiftPlotViewBaseModel(SwiftPlotSeriesViewBase seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		public override void Update() {
			DesignerChartModel chartModel = FindParent<DesignerChartModel>();
			if (chartModel == null)
				return;
			if (SeriesView.Pane != null)
				this.paneModel = (XYDiagramPaneBaseModel)chartModel.FindElementModel(SeriesView.Pane);
			else
				this.paneModel = null;
			if (SeriesView.AxisX != null)
				this.axisXModel = (SwiftPlotDiagramAxisXBaseModel)chartModel.FindElementModel(SeriesView.AxisX);
			else
				this.axisXModel = null;
			if (SeriesView.AxisY != null)
				this.axisYModel = (SwiftPlotDiagramAxisYBaseModel)chartModel.FindElementModel(SeriesView.AxisY);
			else
				this.axisYModel = null; ClearChildren();
			base.Update();
		}
	}
}
