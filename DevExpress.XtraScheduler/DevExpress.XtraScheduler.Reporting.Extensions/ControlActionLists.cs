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
using System.ComponentModel.Design;
using System.ComponentModel;
using DevExpress.XtraReports.Design;
using DevExpress.XtraScheduler.Reporting.Native;
namespace DevExpress.XtraScheduler.Reporting.Design {
	#region ReportViewControlBaseActionList
	public class ReportViewControlBaseActionList : XRControlBaseDesignerActionList {
		public ReportViewControlBaseActionList(ReportViewControlBaseDesigner designer)
			: base(designer) {
		}
		protected virtual bool IsPropertyAllowed(string propertyName) {
			return DesignUtils.IsPropertyAllowed(Component.GetType(), propertyName);
		}
		protected internal void OnPropertyChanged() {
			DevExpress.Utils.Design.EditorContextHelper.FireChanged(designer, Component);
		}
	}
	#endregion
	#region ReportViewControlActionList
	public class ReportViewControlActionList : ReportViewControlBaseActionList {
		public ReportViewControlActionList(ReportViewControlBaseDesigner designer)
			: base(designer) {
		}
		#region Properties
		protected ReportViewControlBase ViewControl { get { return (ReportViewControlBase)Component; } }
		protected new ReportViewControlBaseDesigner ControlDesigner { get { return (ReportViewControlBaseDesigner)base.ControlDesigner; } }
		public ReportViewBase View {
			get { return ViewControl.View; }
			set {
				ViewControl.View = value; OnPropertyChanged();
			}
		}
		public ControlContentLayoutType VerticalLayoutType {
			get { return ViewControl.LayoutOptionsVertical.LayoutType; }
			set {
				ViewControl.LayoutOptionsVertical.LayoutType = value;
				OnPropertyChanged();
			}
		}
		internal virtual bool IsViewAllowed { get { return true; } }
		#endregion
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			if (ControlDesigner.IsViewAllowed)
				AddPropertyItem(actionItems, DesignSR.View, DesignSR.View, Component);
			if (IsPropertyAllowed(DesignSR.VerticalLayoutType))
				AddPropertyItem(actionItems, DesignSR.VerticalLayoutType, DesignSR.VerticalLayoutType, ViewControl);
		}
	}
	#endregion
	#region ReportRelatedControlActionList
	public class ReportRelatedControlActionList : ReportViewControlBaseActionList {
		public ReportRelatedControlActionList(ReportViewControlBaseDesigner designer)
			: base(designer) {
		}
		protected ReportRelatedControlBase ViewControl { get { return (ReportRelatedControlBase)Component; } }
		protected new ReportRelatedControlDesigner ControlDesigner { get { return (ReportRelatedControlDesigner)base.ControlDesigner; } }
		#region Properties
		[TypeConverter(typeof(ReportRelatedControlConverter))]
		public HorizontalHeadersControlBase HorizontalHeaders {
			get { return ControlDesigner.HorizontalHeaders; }
			set {
				ControlDesigner.HorizontalHeaders = value;
				OnPropertyChanged();
			}
		}
		[TypeConverter(typeof(ReportRelatedControlConverter))]
		public VerticalHeadersControlBase VerticalHeaders {
			get { return ControlDesigner.VerticalHeaders; }
			set {
				ControlDesigner.VerticalHeaders = value;
				OnPropertyChanged();
			}
		}
		[TypeConverter(typeof(ReportRelatedControlConverter))]
		public TimeCellsControlBase TimeCells {
			get { return ControlDesigner.TimeCells; }
			set {
				ControlDesigner.TimeCells = value;
				OnPropertyChanged();
			}
		}
		#endregion
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			if (IsPropertyAllowed(DesignSR.HorizontalHeaders))
				AddPropertyItem(actionItems, DesignSR.HorizontalHeaders, "HorizontalHeaders", ViewControl);
			if (IsPropertyAllowed(DesignSR.VerticalHeaders))
				AddPropertyItem(actionItems, DesignSR.VerticalHeaders, "VerticalHeaders", ViewControl);
			if (IsPropertyAllowed(DesignSR.TimeCells))
				AddPropertyItem(actionItems, DesignSR.TimeCells, "TimeCells", ViewControl);
		}
	}
	#endregion
	#region DayViewControlActionList
	public class DayViewControlActionList : ReportViewControlActionList {
		public DayViewControlActionList(ReportViewControlBaseDesigner designer)
			: base(designer) {
		}
		public new ReportDayView View { get { return (ReportDayView)base.View; } set { base.View = value; } }
	}
	#endregion
	#region TimelineViewControlActionList
	public class TimelineViewControlActionList : ReportViewControlActionList {
		public TimelineViewControlActionList(ReportViewControlBaseDesigner designer)
			: base(designer) {
		}
		public new ReportTimelineView View { get { return (ReportTimelineView)base.View; } set { base.View = value; } }
	}
	#endregion
	#region WeekViewControlActionList
	public class WeekViewControlActionList : ReportViewControlActionList {
		public WeekViewControlActionList(ReportViewControlBaseDesigner designer)
			: base(designer) {
		}
		public new ReportWeekView View { get { return (ReportWeekView)base.View; } set { base.View = value; } }
	}
	#endregion
	#region DayViewTimeCellsActionList
	public class DayViewTimeCellsActionList : ReportViewControlBaseActionList {
		public DayViewTimeCellsActionList(ReportViewControlBaseDesigner designer)
			: base(designer) {
		}
		protected DayViewTimeCells TimeCells { get { return (DayViewTimeCells)base.Component; } }
		protected new DayViewTimeCellsDesigner ControlDesigner { get { return (DayViewTimeCellsDesigner)base.designer; } }
		public TimeSpan TimeScale {
			get { return TimeCells.TimeScale; }
			set {
				TimeCells.TimeScale = value;
				OnPropertyChanged();
			}
		}
		public TimeSpan VisibleTimeStart {
			get { return ControlDesigner.VisibleTimeStart; }
			set {
				ControlDesigner.VisibleTimeStart = value;
				OnPropertyChanged();
			}
		}
		public TimeSpan VisibleTimeEnd {
			get { return ControlDesigner.VisibleTimeEnd; }
			set {
				ControlDesigner.VisibleTimeEnd = value;
				OnPropertyChanged();
			}
		}
		public bool ShowWorkTimeOnly {
			get { return TimeCells.ShowWorkTimeOnly; }
			set {
				TimeCells.ShowWorkTimeOnly = value;
				OnPropertyChanged();
			}
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, DesignSR.TimeScale, "TimeScale", TimeCells);
			AddPropertyItem(actionItems, DesignSR.VisibleTimeStart, "VisibleTimeStart", ControlDesigner);
			AddPropertyItem(actionItems, DesignSR.VisibleTimeEnd, "VisibleTimeEnd", ControlDesigner);
			AddPropertyItem(actionItems, DesignSR.ShowWorkTimeOnly, "ShowWorkTimeOnly", TimeCells);
		}
	}
	#endregion
	#region HorizontalWeekActionList
	public class HorizontalWeekActionList : ReportViewControlBaseActionList {
		public HorizontalWeekActionList(ReportViewControlBaseDesigner designer)
			: base(designer) {
		}
		protected HorizontalWeek Week { get { return (HorizontalWeek)base.Component; } }
		public bool CompressWeekend {
			get { return Week.CompressWeekend; }
			set {
				Week.CompressWeekend = value;
				OnPropertyChanged();
			}
		}
		[
		Editor("DevExpress.XtraScheduler.Reporting.Design.WeekDaysEditor," + AssemblyInfo.SRAssemblySchedulerReportingExtensions, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.XtraScheduler.Reporting.Design.WeekDaysConverter))
		]
		public WeekDays VisibleWeekDays {
			get { return Week.VisibleWeekDays; }
			set {
				Week.VisibleWeekDays = value;
				OnPropertyChanged();
			}
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, DesignSR.VisibleWeekDays, "VisibleWeekDays", Week);
			AddPropertyItem(actionItems, DesignSR.CompressWeekend, "CompressWeekend", Week);
		}
	}
	#endregion
	#region DataDependentControlActionList
	public class DataDependentControlActionList : ReportViewControlBaseActionList {
		public DataDependentControlActionList(ReportViewControlBaseDesigner designer)
			: base(designer) {
		}
		protected DataDependentControlBase ViewControl { get { return (DataDependentControlBase)Component; } }
		protected new DataDependentControlDesigner ControlDesigner { get { return (DataDependentControlDesigner)base.designer; } }
		public PrintInColumnMode PrintInColumn {
			get { return ViewControl.PrintInColumn; }
			set {
				ViewControl.PrintInColumn = value;
				OnPropertyChanged();
			}
		}
		public PrintContentMode PrintContentMode {
			get { return ControlDesigner.PrintContentMode; }
			set {
				ControlDesigner.PrintContentMode = value;
				OnPropertyChanged();
			}
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, DesignSR.PrintInColumn, "PrintInColumn", ViewControl);
			if (ControlDesigner.IsPrintContentModeAllowed)
				AddPropertyItem(actionItems, DesignSR.PrintContentMode, "PrintContentMode", ViewControl);
		}
	}
	#endregion
	#region TimeIntervalInfoActionList
	public class TimeIntervalInfoActionList: DataDependentControlActionList {
		public TimeIntervalInfoActionList(ReportViewControlBaseDesigner designer)
			: base(designer) {
		}
		protected new TimeIntervalInfo ViewControl { get { return (TimeIntervalInfo)Component; } }
		public TimeIntervalFormatType FormatType {
			get { return ViewControl.FormatType; }
			set {
				ViewControl.FormatType = value;
				OnPropertyChanged();
			}
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, DesignSR.FormatType, "FormatType", ViewControl);
		}
	}
	#endregion
	#region FormatTimeIntervalInfoActionList
	public class FormatTimeIntervalInfoActionList : DataDependentControlActionList {
		public FormatTimeIntervalInfoActionList(ReportViewControlBaseDesigner designer)
			: base(designer) {
		}
		protected new FormatTimeIntervalInfo ViewControl { get { return (FormatTimeIntervalInfo)Component; } }
		public bool AutoFormat {
			get { return ViewControl.AutoFormat; }
			set {
				ViewControl.AutoFormat = value;
				OnPropertyChanged();
			}
		}
		[TypeConverter(typeof(DevExpress.Utils.Design.TimeIntervalFormatConverter))]
		public string FormatString {
			get { return ViewControl.FormatString; }
			set {
				ViewControl.FormatString = value;
				OnPropertyChanged();
			}
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, DesignSR.AutoFormat, "AutoFormat", ViewControl);
			AddPropertyItem(actionItems, DesignSR.FormatString, "FormatString", ViewControl);
		}
	}
	#endregion
	#region CorneredControlActionList
	public class CorneredControlActionList : XRControlBaseDesignerActionList {
		public CorneredControlActionList(ReportViewControlBaseDesigner designer)
			: base(designer) {
		}
		protected new CorneredControlDesigner ControlDesigner { get { return (CorneredControlDesigner)base.designer; } }
		public int TopCornerIndent {
			get { return ControlDesigner.TopCornerIndent; }
			set {
				ControlDesigner.TopCornerIndent = value;
				DevExpress.Utils.Design.EditorContextHelper.FireChanged(designer, Component);
			}
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, DesignSR.TopCornerIndent, "TopCornerIndent", ControlDesigner);
		}
	}
	#endregion
}
