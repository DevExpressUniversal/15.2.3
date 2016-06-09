#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardWin.Native;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraGauges.Win;
using DevExpress.XtraGrid;
using DevExpress.XtraMap;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraRichEdit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.DashboardWin {
	public class DashboardCreatingEventArgs : EventArgs {
		readonly Dashboard dashboard;
		public Dashboard Dashboard { get { return dashboard; } }
		public bool Handled { get; set; }
		public DashboardCreatingEventArgs(Dashboard dashboard) {
			this.dashboard = dashboard;
			Handled = false;
		}
	}
	public delegate void DashboardCreatingEventHandler (object sender, DashboardCreatingEventArgs e);
	public class DashboardOpeningEventArgs : EventArgs {
		public Dashboard Dashboard { get; set; }		
		public bool Handled { get; set; }
		public DashboardOpeningEventArgs() {   
		}
	}
	public delegate void DashboardOpeningEventHandler(object sender, DashboardOpeningEventArgs e);
	public class DashboardSavingEventArgs : EventArgs {		
		readonly Dashboard dashboard;
		readonly DashboardSaveCommand command;
		public Dashboard Dashboard { get { return dashboard; } }		
		public bool Handled { get; set; }
		public bool Saved { get; set; }
		public DashboardSaveCommand Command { get { return command; } }
		public DashboardSavingEventArgs(Dashboard dashboard, DashboardSaveCommand command) {
			this.dashboard = dashboard;			
			Saved = true;
			this.command = command;
		}
	}
	public enum DashboardSaveCommand  { Save, SaveAs }
	public delegate void DashboardSavingEventHandler(object sender, DashboardSavingEventArgs e);
	public class DashboardSavedEventArgs : EventArgs {
		readonly Dashboard dashboard;
		readonly string oldFileName;
		readonly string newFileName;
		public Dashboard Dashboard { get { return dashboard; } }
		public string OldFileName { get { return oldFileName; } }
		public string NewFileName { get { return newFileName; } }
		public DashboardSavedEventArgs(Dashboard dashboard, string oldFileName, string newFileName) {
			this.dashboard = dashboard;
			this.oldFileName = oldFileName;
			this.newFileName = newFileName;
		}
	}
	public delegate void DashboardSavedEventHandler(object sender, DashboardSavedEventArgs e);
	public class DashboardClosingEventArgs : EventArgs {
		readonly Dashboard dashboard;
		bool isDashboardModified;
		public Dashboard Dashboard { get { return dashboard; } }
		public bool IsDashboardModified { get { return isDashboardModified; } set { isDashboardModified = value; } }
		public DashboardClosingEventArgs(Dashboard dashboard, bool isDashboardModified) {
			this.dashboard = dashboard;
			this.isDashboardModified = isDashboardModified;
		}
	}
	public delegate void DashboardClosingEventHandler(object sender, DashboardClosingEventArgs e);
	public class DashboardBeforeExportEventArgs : EventArgs {
		const bool DefaultShowExportForm = true;
		public bool ShowExportForm { get; set; }
		public DashboardBeforeExportEventArgs() {
			ShowExportForm = DefaultShowExportForm;
		}
	}
	public delegate void DashboardBeforeExportEventHandler(object sender, DashboardBeforeExportEventArgs e);
	public class DashboardItemMouseActionEventArgs : DashboardItemMouseHitTestEventArgs {
		internal DashboardItemMouseActionEventArgs(DashboardItemViewer dashboardItemViewer)
			: base(dashboardItemViewer) {
		}
		internal DashboardItemMouseActionEventArgs(DashboardItemViewer dashboardItemViewer, Point location)
			: base(dashboardItemViewer, location) {
		}
	}
	public delegate void DashboardItemMouseActionEventHandler(object sender, DashboardItemMouseActionEventArgs e);
	public class DashboardItemMouseEventArgs : EventArgs {
		string dashboardItemName;
		public DashboardItemMouseEventArgs(string dashboardItemName) {
			this.dashboardItemName = dashboardItemName;
		}
		public string DashboardItemName { get { return dashboardItemName; } }
	}
	public delegate void DashboardItemMouseEventHandler(object sender, DashboardItemMouseEventArgs e);
	public class DashboardItemMouseHitTestEventArgs : DashboardItemMouseEventArgs {
		readonly DashboardItemViewer dashboardItemViewer;
		readonly Point location;
		DataPointInfo dataPointInfo;
		AxisPointTuple tuple;
		bool initialized = false;
		DataDashboardItemViewer DataDashboardItemViewer { get { return dashboardItemViewer as DataDashboardItemViewer; } }
		AxisPointTuple Tuple { 
			get {
				if(!initialized)
					Initialize();
				return tuple; 
			}
		}
		DataPointInfo DataPointInfo {
			get {
				if(!initialized)
					Initialize();
				return dataPointInfo; 
			} 
		}
		protected DashboardItemMouseHitTestEventArgs(DashboardItemViewer dashboardItemViewer)
			: base(dashboardItemViewer != null ? dashboardItemViewer.DashboardItemName : string.Empty) {
			this.dashboardItemViewer = dashboardItemViewer;
		}
		protected DashboardItemMouseHitTestEventArgs(DashboardItemViewer dashboardItemViewer, Point location)
			: this(dashboardItemViewer) {
			this.location = location;
		}
		public MultiDimensionalData Data {
			get {
				if(DataDashboardItemViewer != null) {
					return DataDashboardItemViewer.MultiDimensionalData;
				}
				return null;
			}
		}
		public AxisPoint GetAxisPoint(string axisName) {
			return Tuple.GetAxisPoint(axisName);
		}
		public AxisPoint GetAxisPoint() {
			return Tuple.GetAxisPoint();
		}
		public IList<MeasureDescriptor> GetMeasures() {
			List<MeasureDescriptor> descriptors = new List<MeasureDescriptor>();
			foreach(MeasureDescriptor descriptor in dashboardItemViewer.MultiDimensionalData.GetMeasures()) {
				if(DataPointInfo.Measures.Contains(descriptor.ID))
					descriptors.Add(descriptor);
			}
			return descriptors;
		}
		public IList<DeltaDescriptor> GetDeltas() {
			List<DeltaDescriptor> descriptors = new List<DeltaDescriptor>();
			foreach(DeltaDescriptor descriptor in dashboardItemViewer.MultiDimensionalData.GetDeltas()) {
				if(DataPointInfo.Deltas.Contains(descriptor.ID))
					descriptors.Add(descriptor);
			}
			return descriptors;
		}
		public MultiDimensionalData GetSlice(string axisName) {
			AxisPoint hierarchyItem = GetAxisPoint(axisName);
			if(hierarchyItem != null) {
				return Data.GetSlice(hierarchyItem);
			}
			return null;
		}
		public MultiDimensionalData GetSlice() {
			return GetSlice(DashboardDataAxisNames.DefaultAxis);
		}
		public DashboardUnderlyingDataSet GetUnderlyingData(string axisName, IList<string> dataMembers) {
			if(DataDashboardItemViewer != null) {
				if(DataPointInfo != null) {
					AxisPoint axisPoint = GetAxisPoint(axisName);
					return DataDashboardItemViewer.GetUnderlyingData(new List<AxisPoint>() { axisPoint }, dataMembers);
				}
			}
			return null;
		}
		public DashboardUnderlyingDataSet GetUnderlyingData(string axisName) {
			return GetUnderlyingData(axisName, null);
		}
		public DashboardUnderlyingDataSet GetUnderlyingData() {
			return GetUnderlyingData((IList<string>)null);
		}
		public DashboardUnderlyingDataSet GetUnderlyingData(IList<string> dataMembers) {
			if(DataDashboardItemViewer != null) {
				if(DataPointInfo != null) {
					return DataDashboardItemViewer.GetUnderlyingData(Tuple.AxisPoints, dataMembers);
				}
			}
			return null;
		}
		void Initialize() {
			InitializeDataPoint();
			InitializeAxisPoints();
			initialized = true;
		}
		void InitializeDataPoint() {
			dataPointInfo = ((IDataPointInfoProvider)dashboardItemViewer).GetInfo(location);
		}
		void InitializeAxisPoints() {
			IList<AxisPoint> axisPoints = new List<AxisPoint>();
			if(dataPointInfo != null && dataPointInfo.DimensionValues != null && dataPointInfo.DimensionValues.Count > 0) {
				foreach(KeyValuePair<string, IList> axisPointValue in dataPointInfo.DimensionValues) {
					axisPoints.Add(DataDashboardItemViewer.GetAxisPoint(axisPointValue.Key, axisPointValue.Value));
				}
			}
			tuple = new AxisPointTuple(axisPoints);
		}
	}
	public delegate void DashboardItemVisualInteractivityEventHandler(object sender, DashboardItemVisualInteractivityEventArgs e);
	public class DashboardItemVisualInteractivityEventArgs : EventArgs {
		DataDashboardItemViewer itemViewer;
		List<string> targetAxes;
		InteractivityOptions options;
		internal DashboardItemVisualInteractivityEventArgs(DataDashboardItemViewer viewer, InteractivityOptions options, List<string> targetAxes) {
			this.itemViewer = viewer;
			this.options = options;
			this.targetAxes = targetAxes;
		}
		public string DashboardItemName { get { return itemViewer.DashboardItemName; } }
		public DashboardSelectionMode SelectionMode { get { return options.SelectionMode; } set { options.SelectionMode = value; } }
		public bool EnableHighlighting { get { return options.HighlightEnable; } set { options.HighlightEnable = value; } }
		public List<string> TargetAxes { get { return targetAxes; } set { targetAxes = value; } }
		public MultiDimensionalData Data {
			get {
				if(itemViewer != null) {
					return itemViewer.MultiDimensionalData;
				}
				return null;
			}
		}
		internal InteractivityOptions InteractivityOptions { get { return options; } }
		public void SetDefaultSelection(AxisPoint axisPoint) {
			List<AxisPointTuple> tupleList = new List<AxisPointTuple>();
			tupleList.Add(new AxisPointTuple(axisPoint));
			SetDefaultSelection(tupleList);
		}
		public void SetDefaultSelection(List<AxisPoint> axisPoints) {
			List<AxisPointTuple> tupleList = new List<AxisPointTuple>();
			foreach(AxisPoint axisPoint in axisPoints)
				tupleList.Add(new AxisPointTuple(axisPoint));
			SetDefaultSelection(tupleList);
		}
		public void SetDefaultSelection(AxisPointTuple axisPointTuple) {
			SetDefaultSelection(new List<AxisPointTuple> { axisPointTuple });
		}
		public void SetDefaultSelection(List<AxisPointTuple> axisPointTuples) {
			options.DefaultSelection = axisPointTuples;
		}
	}
	public delegate void DashboardItemSelectionChangedEventHandler(object sender, DashboardItemSelectionChangedEventArgs e);
	public class DashboardItemSelectionChangedEventArgs : EventArgs {
		string dashboardItemName;
		List<AxisPointTuple> selection;
		public string DashboardItemName { get { return dashboardItemName; } }
		public List<AxisPointTuple> CurrentSelection { get { return selection; } }
		internal DashboardItemSelectionChangedEventArgs(string dashboardItemName, List<AxisPointTuple> selection) {
			this.dashboardItemName = dashboardItemName;
			this.selection = selection;
		}
	}
	public delegate void DashboardItemControlUpdatedEventHandler(object sender, DashboardItemControlEventArgs e);
	public delegate void DashboardItemControlCreatedEventHandler(object sender, DashboardItemControlEventArgs e);
	public delegate void DashboardItemBeforeControlDisposedEventHandler(object sender, DashboardItemControlEventArgs e);
	public class DashboardItemControlEventArgs : EventArgs {
		string dashboardItemName;
		Control control;
		public string DashboardItemName { get { return dashboardItemName; } }
		public ChartControl ChartControl { get { return control as ChartControl; } }
		public RichEditControl RichEditControl { get{ return control as RichEditControl; } }
		public PivotGridControl PivotGridControl { get { return control as PivotGridControl; } }
		public MapControl MapControl { get { return control as MapControl; } }
		public PictureEdit PictureEdit { get { return control as PictureEdit; } }
		public GridControl GridControl { get { return control as GridControl; } }
		public GaugeControl GaugeControl { get { return control as GaugeControl; } }
		internal DashboardItemControlEventArgs(string dashboardItemName, Control control) { 
			this.dashboardItemName = dashboardItemName;
			this.control = control;
		}
	}
	public delegate void DashboardItemElementCustomColorEventHandler(object sender, DashboardItemElementCustomColorEventArgs e);
	public class DashboardItemElementCustomColorEventArgs : EventArgs {
		string dashboardItemName;
		ElementCustomColorEventArgs baseEventArgs;
		MultiDimensionalData data;
		public MultiDimensionalData Data { get { return data; } }
		public AxisPointTuple TargetElement { get { return baseEventArgs.AxisPointTuple; } }
		public List<MeasureDescriptor> Measures { get { return baseEventArgs.Measures; } }
		public Color Color { get { return baseEventArgs.Color; } set { baseEventArgs.Color = value; } }
		public string DashboardItemName { get { return dashboardItemName; } }
		internal DashboardItemElementCustomColorEventArgs(string dashboardItemName, MultiDimensionalData data,  ElementCustomColorEventArgs baseEventArgs) {
			this.dashboardItemName = dashboardItemName;
			this.baseEventArgs = baseEventArgs;
			this.data = data;
		}
	}
	public delegate void DashboardLoadedEventHandler(object sender, DashboardLoadedEventArgs e);
	public class DashboardLoadedEventArgs : EventArgs {
		public Dashboard Dashboard { get; private set; }
		internal DashboardLoadedEventArgs(Dashboard dashboard) {
			Guard.ArgumentNotNull(dashboard, "dashboard");
			Dashboard = dashboard;
		}
	}
	public delegate void CustomizeDashboardTitleTextEventHandler(object sender, CustomizeDashboardTitleTextEventArgs e);
	public class CustomizeDashboardTitleTextEventArgs : EventArgs {
		public string Text { get; set; }
		internal CustomizeDashboardTitleTextEventArgs(string text) {
			Text = text;
		}
	}
}
