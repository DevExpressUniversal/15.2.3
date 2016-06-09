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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using System.ComponentModel;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Internal.Implementations;
using System.Collections;
using System.Threading;
namespace DevExpress.Web.ASPxScheduler.Drawing {
	#region AppointmentViewInfo
	public class AppointmentViewInfo : IAppointmentViewInfo {
		#region Fields
		bool hasTopBorder;
		bool hasBottomBorder;
		bool hasLeftBorder;
		bool hasRightBorder;
		TimeInterval interval;
		Appointment appointment;
		XtraScheduler.Resource resource;
		AppointmentViewInfoOptions options;
		IAppointmentStatus status;
		TimeInterval appointmentInterval;
		AppearanceStyleBase appointmentStyle;
		#endregion
		public AppointmentViewInfo(Appointment appointment) {
			if (appointment == null)
				Exceptions.ThrowArgumentException("appointment", appointment);
			this.appointment = appointment;
			StatusBackgroundColor = AppointmentStatus.Empty.Color;
			StatusColor = AppointmentStatus.Empty.Color;
			Initialize();
		}
		#region Properties
		public Appointment Appointment { get { return appointment; } }
		public virtual TimeInterval AppointmentInterval { get { return appointmentInterval; } }
		public Resource Resource {
			get { return resource; }
			set {
				if (value == null)
					Exceptions.ThrowArgumentException("resource", resource);
				else
					resource = value;
			}
		}
		public TimeInterval Interval {
			get { return interval; }
			set {
				if (value == null)
					Exceptions.ThrowArgumentException("interval", interval);
				else
					interval = value;
			}
		}
		#region Border Options
		public bool HasTopBorder { get { return hasTopBorder; } set { hasTopBorder = value; } }
		public bool HasBottomBorder { get { return hasBottomBorder; } set { hasBottomBorder = value; } }
		public bool HasLeftBorder { get { return hasLeftBorder; } set { hasLeftBorder = value; } }
		public bool HasRightBorder { get { return hasRightBorder; } set { hasRightBorder = value; } }
		#endregion
		#region ViewInfo Options
		public AppointmentViewInfoOptions Options { get { return options; } }
		public bool ShowBell { get { return Options.ShowBell; } set { Options.ShowBell = value; } }
		public bool ShowEndTime { get { return Options.ShowEndTime; } set { Options.ShowEndTime = value; } }
		public bool ShowRecurrence { get { return Options.ShowRecurrence; } set { Options.ShowRecurrence = value; } }
		public bool ShowStartTime { get { return Options.ShowStartTime; } set { Options.ShowStartTime = value; } }
		public bool ShowTimeAsClock { get { return Options.ShowTimeAsClock; } set { Options.ShowTimeAsClock = value; } }
		public AppointmentStatusDisplayType StatusDisplayType { get { return Options.StatusDisplayType; } set { Options.StatusDisplayType = value; } }
		#endregion
		public AppearanceStyleBase AppointmentStyle { get { return appointmentStyle; } set { appointmentStyle = value; } }
		public IAppointmentStatus Status { get { return status; } set { status = value; } }
		public Color StatusBackgroundColor { get; set; }
		public Color StatusColor { get; set; }
		[Obsolete("You should use the 'AppointmentStyle.BackColor' instead", false), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public Color BackColor { get { return AppointmentStyle.BackColor; } set { AppointmentStyle.BackColor = value; } }
		protected internal virtual bool SameDay { get { return AppointmentInterval.SameDay; } }
		protected internal virtual bool LongerThanADay { get { return AppointmentInterval.LongerThanADay; } }
		#endregion
		protected internal virtual void Initialize() {
			this.resource = ResourceBase.Empty;
			this.interval = TimeInterval.Empty;
			this.options = new AppointmentViewInfoOptions();
			this.status = AppointmentStatus.Empty;
			this.appointmentInterval = GetAppointmentInterval();
			this.appointmentStyle = new AppearanceStyleBase();
		}
		public virtual bool IsLongTime() {
			return LongerThanADay || !SameDay;
		}
		protected internal virtual TimeInterval GetAppointmentInterval() {
			TimeZoneHelper tze = ASPxScheduler.ActiveControl.InnerControl.TimeZoneHelper;
			return tze.ToClientTime(((IInternalAppointment)Appointment).GetInterval(), Appointment.TimeZoneId, true);
		}
	}
	#endregion
	#region TimeLineAppointmentViewInfo
	public class TimeLineAppointmentViewInfo : AppointmentViewInfo {
		public TimeLineAppointmentViewInfo(Appointment appointment)
			: base(appointment) {
		}
		public override bool IsLongTime() {
			return true;
		}
	}
	#endregion
	#region AppointmentControl
	public abstract class AppointmentControl : ASPxInternalWebControl {
		readonly AppointmentIntermediateViewInfo intermediateViewInfo;
		readonly WebControl mainDiv;
		AppointmentTemplateItems templateItems;
		string templateContainerId;
		bool isPermanentAppointment = true;
		protected AppointmentControl(AppointmentIntermediateViewInfo aptViewInfo) {
			Guard.ArgumentNotNull(aptViewInfo, "aptViewInfo");
			this.intermediateViewInfo = aptViewInfo;
			this.mainDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			this.templateItems = CreateTemplateItems();
		}
		public AppointmentViewInfo ViewInfo { get { return intermediateViewInfo.ViewInfo; } }
		public AppointmentIntermediateViewInfo IntermediateViewInfo { get { return intermediateViewInfo; } }
		public Appointment Appointment { get { return ViewInfo.Appointment; } }
		public WebControl MainDiv { get { return mainDiv; } }
		protected internal virtual string TemplateContainerId { get { return templateContainerId; } set { templateContainerId = value; } }
		public AppointmentTemplateItems TemplateItems { get { return templateItems; } set { templateItems = value; } }
		internal bool IsPermanentAppointment { get { return isPermanentAppointment; } set { isPermanentAppointment = value; } }
		public virtual void AssignId(int aptIndex) {
			if (!IsPermanentAppointment) {
				MainDiv.ID = String.Format("{0}_{1}", SchedulerIdHelper.GenerateAppointmentDivId(aptIndex), SchedulerIdHelper.NonPermanentAppointmentDivSuffix);
				return;
			}
			MainDiv.ID = SchedulerIdHelper.GenerateAppointmentDivId(aptIndex);
			TemplateContainerId = SchedulerIdHelper.GenerateAppointmentTemplateContainerId(aptIndex);
			TemplateItems.StatusControl.AssignId(aptIndex);
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			Controls.Add(MainDiv);
			if (IsPermanentAppointment) {
				AppointmentTemplateContainer container = CreateTemplateContainer();
				container.ID = TemplateContainerId;
				MainDiv.Controls.Add(container);
				container.DataBind();
			}
		}
		protected override void PrepareControlHierarchy() {
			EnableViewState = false;
			PrepareMainDiv();
		}
		protected internal virtual void PrepareMainDiv() {
			RenderUtils.AppendDefaultDXClassName(MainDiv, "dxscApt");
		}
		protected internal virtual String GetCreateAppointmentCommonArgs(string clientAppointmentID) {
			string startTime = HtmlConvertor.ToScript(ViewInfo.Interval.Start, typeof(DateTime));
			string aptArgs = String.Format("\"{0}\", \"{1}\", {2}, {3}, {4}, {5}, \"{6}\", \"{7}\"", IntermediateViewInfo.FirstCell.Id, IntermediateViewInfo.LastCell.Id, startTime, ViewInfo.Interval.Duration.TotalMilliseconds, IntermediateViewInfo.StartRelativeOffset, IntermediateViewInfo.EndRelativeOffset, MainDiv.ID, clientAppointmentID);
			string statusInfoArgs = CreateStatusInfoArgs();
			string[] result = new string[] { aptArgs, statusInfoArgs };
			return String.Join(", ", result);
		}
		protected internal virtual string CreateStatusInfoArgs() {
			if (!IsPermanentAppointment)
				return "\"\", \"\", 0, 0";
			AppointmentStatusControl statusControl = TemplateItems.StatusControl;
			return String.Format("\"{0}\", \"{1}\", {2}, {3}", statusControl.BackgroundId, statusControl.ForegroundId, statusControl.StartOffset, statusControl.EndOffset);
		}
		public abstract void GenerateCreateAppointmentScript(StringBuilder sb, string localVarName, string clientAppointmentID);
		protected internal abstract AppointmentTemplateContainer CreateTemplateContainer();
		protected internal abstract AppointmentTemplateItems CreateTemplateItems();
	}
	#endregion
	public class VerticalAppointmentControl : AppointmentControl {
		public VerticalAppointmentControl(AppointmentIntermediateViewInfo aptViewInfo)
			: base(aptViewInfo) {
		}
		public new VerticalAppointmentTemplateItems TemplateItems { get { return (VerticalAppointmentTemplateItems)base.TemplateItems; } set { base.TemplateItems = value; } }
		public override void GenerateCreateAppointmentScript(StringBuilder sb, string localVarName, string clientAppointmentID) {
			String commonArgs = GetCreateAppointmentCommonArgs(clientAppointmentID);
			String args = String.Format("{0}, {1}, {2}, {3}, {4}, {5}", commonArgs, IntermediateViewInfo.FirstIndexPosition, IntermediateViewInfo.LastIndexPosition, IntermediateViewInfo.MaxIndexInGroup, HtmlConvertor.ToScript(ViewInfo.HasTopBorder), HtmlConvertor.ToScript(ViewInfo.HasBottomBorder));
			sb.AppendFormat("{0}.AddVerticalAppointment({1});\n", localVarName, args);
		}
		protected internal override AppointmentTemplateContainer CreateTemplateContainer() {
			TemplatesHelper helper = TemplatesHelper.Create(ASPxScheduler.ActiveControl.ActiveView);
			return helper.GetVerticalAppointmentTemplateContainer(ViewInfo, TemplateItems);
		}
		protected internal override AppointmentTemplateItems CreateTemplateItems() {
			return new VerticalAppointmentTemplateItems();
		}
	}
	#region HorizontalAppointmentControl
	public class HorizontalAppointmentControl : AppointmentControl {
		public new HorizontalAppointmentTemplateItems TemplateItems { get { return (HorizontalAppointmentTemplateItems)base.TemplateItems; } set { base.TemplateItems = value; } }
		public HorizontalAppointmentControl(AppointmentIntermediateViewInfo aptViewInfo)
			: base(aptViewInfo) {
		}
		public override void GenerateCreateAppointmentScript(StringBuilder sb, string localVarName, string clientAppointmentID) {
			String commonArgs = GetCreateAppointmentCommonArgs(clientAppointmentID);
			String args = String.Format("{0}, {1}, {2}", commonArgs, HtmlConvertor.ToScript(ViewInfo.HasLeftBorder), HtmlConvertor.ToScript(ViewInfo.HasRightBorder));
			sb.AppendFormat("{0}.AddHorizontalAppointment({1});\n", localVarName, args);
		}
		protected internal override AppointmentTemplateContainer CreateTemplateContainer() {
			TemplatesHelper helper = TemplatesHelper.Create(ASPxScheduler.ActiveControl.ActiveView);
			if (ViewInfo.IsLongTime())
				return helper.GetHorizontalAppointmentTemplateContainer(ViewInfo, TemplateItems);
			else
				return helper.GetHorizontalSameDayAppointmentTemplateContainer(ViewInfo, TemplateItems);
		}
		protected internal override AppointmentTemplateItems CreateTemplateItems() {
			return new HorizontalAppointmentTemplateItems();
		}
	}
	#endregion
	#region AppointmentControlCollection
	public class AppointmentControlCollection : DXCollection<AppointmentControl>, IAppointmentViewInfoCollection {
		public void AddRange(IAppointmentViewInfoCollection value) {
			base.AddRange((AppointmentControlCollection)value);
		}
	}
	#endregion
	#region WebAppointmentsLayoutResult
	public class WebAppointmentsLayoutResult : IAppointmentsLayoutResult {
		AppointmentControlCollection appointmentControls = new AppointmentControlCollection();
		public AppointmentControlCollection AppointmentControls { get { return appointmentControls; } }
		IAppointmentViewInfoCollection IAppointmentsLayoutResult.AppointmentViewInfos { get { return AppointmentControls; } }
		public virtual void Merge(IAppointmentsLayoutResult baseLayoutResult) {
			WebAppointmentsLayoutResult layoutResult = (WebAppointmentsLayoutResult)baseLayoutResult;
			AppointmentControls.AddRange(layoutResult.AppointmentControls);
		}
	}
	#endregion
	#region AppointmentIntermediateViewInfo
	public class AppointmentIntermediateViewInfo : AppointmentIntermediateViewInfoCore {
		IWebTimeCell firstCell;
		IWebTimeCell lastCell;
		public AppointmentIntermediateViewInfo(AppointmentViewInfo viewInfo)
			: base(viewInfo) {
		}
		public new AppointmentViewInfo ViewInfo { get { return (AppointmentViewInfo)base.ViewInfo; } }
		protected internal IWebTimeCell FirstCell { get { return firstCell; } set { firstCell = value; } }
		protected internal IWebTimeCell LastCell { get { return lastCell; } set { lastCell = value; } }
	}
	#endregion
	#region AppointmentIntermediateViewInfoComparer
	public class AppointmentIntermediateViewInfoComparer : AppointmentIntermediateViewInfoCoreComparer<AppointmentIntermediateViewInfo> {
		public AppointmentIntermediateViewInfoComparer(AppointmentBaseComparer aptComparer)
			: base(aptComparer) {
		}
	}
	#endregion
	#region AppointmentIntermediateViewInfoCollection
	public class AppointmentIntermediateViewInfoCollection : DXCollection<AppointmentIntermediateViewInfo>, IAppointmentIntermediateViewInfoCoreCollection {
		XtraScheduler.Resource resource;
		TimeInterval interval;
		ManualResetEventSlim signal = new ManualResetEventSlim();
		public AppointmentIntermediateViewInfoCollection() {
		}
		public AppointmentIntermediateViewInfoCollection(XtraScheduler.Resource resource, TimeInterval interval) {
			this.resource = resource;
			this.interval = interval;
		}
		public Resource Resource { get { return this.resource; } }
		public TimeInterval Interval { get { return this.interval; } }
		public List<IAppointmentIntermediateLayoutViewInfoCoreCollection> GroupedViewInfos { get { return new List<IAppointmentIntermediateLayoutViewInfoCoreCollection>(); } }
		#region IAppointmentIntermediateViewInfoCoreCollection Members
		AppointmentIntermediateViewInfoCore IAppointmentIntermediateViewInfoCoreCollection.this[int index] { get { return base[index]; } }
		ManualResetEventSlim IAppointmentIntermediateViewInfoCoreCollection.Signal {
			get { return this.signal; }
		}
		bool IAppointmentIntermediateViewInfoCoreCollection.Remove(AppointmentIntermediateViewInfoCore value) {
			return Remove((AppointmentIntermediateViewInfo)value);
		}
		int IAppointmentIntermediateViewInfoCoreCollection.Add(AppointmentIntermediateViewInfoCore value) {
			return Add((AppointmentIntermediateViewInfo)value);
		}
		void IAppointmentIntermediateViewInfoCoreCollection.Sort(AppointmentBaseComparer aptComparer) {
			Sort(new AppointmentIntermediateViewInfoComparer(aptComparer));
		}
		#endregion
	}
	#endregion
	public abstract class AppointmentBaseLayoutCalculator : AppointmentLayoutCalculatorCore<AppointmentIntermediateViewInfoCollection, WebVisuallyContinuousCellsInfo, WebAppointmentsLayoutResult, AppointmentControlCollection> {
		readonly AppointmentContentLayoutCalculator contentCalculator;
		protected AppointmentBaseLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, AppointmentContentLayoutCalculator contentCalculator)
			: base(viewInfo) {
			if (contentCalculator == null)
				Exceptions.ThrowArgumentNullException("contentCalculator");
			this.contentCalculator = contentCalculator;
		}
		protected internal new ISchedulerWebViewInfoBase ViewInfo { get { return (ISchedulerWebViewInfoBase)base.ViewInfo; } }
		protected internal AppointmentContentLayoutCalculator ContentCalculator { get { return contentCalculator; } }
		protected internal abstract AppointmentControl CreateAppointmentControlInstance(AppointmentIntermediateViewInfo aptViewInfo);
		protected internal override TimeInterval GetAppointmentInterval(Appointment appointment) {
			return ViewInfo.View.Control.InnerControl.TimeZoneHelper.ToClientTime(((IInternalAppointment)appointment).CreateInterval(), appointment.TimeZoneId, true);
		}
		protected internal override void PreliminaryContentLayout(AppointmentIntermediateViewInfoCollection intermediateResul, WebVisuallyContinuousCellsInfo cellsInfo) {
		}
		protected internal override void FinalContentLayout(AppointmentControlCollection aptControls) {
			ContentCalculator.CalculateContentLayout(aptControls);
		}
		protected internal override WebAppointmentsLayoutResult CalculateLayoutCoreSingleCellsInfo(AppointmentBaseCollection appointments, WebVisuallyContinuousCellsInfo cellsInfo, bool isTwoPassLayout) {
			AppointmentIntermediateLayoutCalculatorCore intermediateCalculator = CreateIntermediateLayoutCalculator();
			AppointmentIntermediateViewInfoCollection intermediateResult = (AppointmentIntermediateViewInfoCollection)intermediateCalculator.CreateIntermediateViewInfoCollection((XtraScheduler.Resource)cellsInfo.Resource, cellsInfo.Interval);
			CalculateIntermediateViewInfos(intermediateResult, appointments, cellsInfo);
			PreliminaryContentLayout(intermediateResult, cellsInfo);
			LayoutViewInfos(cellsInfo, intermediateResult, isTwoPassLayout);
			WebAppointmentsLayoutResult result = SnapToCells(intermediateResult, cellsInfo);
			FinalContentLayout(result.AppointmentControls);
			return result;
		}
		protected internal override WebAppointmentsLayoutResult SnapToCells(AppointmentIntermediateViewInfoCollection intermediateResult, WebVisuallyContinuousCellsInfo cellsInfo) {
			WebAppointmentsLayoutResult result = new WebAppointmentsLayoutResult();
			int count = intermediateResult.Count;
			for (int i = 0; i < count; i++) {
				AppointmentControl aptControl = SnapToCellsCore(intermediateResult[i], cellsInfo);
				result.AppointmentControls.Add(aptControl);
			}
			return result;
		}
		protected internal virtual AppointmentControl SnapToCellsCore(AppointmentIntermediateViewInfo intermediateViewInfo, WebVisuallyContinuousCellsInfo cellsInfo) {
			CalculateViewInfoProperties(intermediateViewInfo, cellsInfo);
			return CreateAppointmentControlInstance(intermediateViewInfo);
		}
		protected internal override void CalculateViewInfoProperties(AppointmentIntermediateViewInfoCore intermediateResult, WebVisuallyContinuousCellsInfo cellsInfo) {
			base.CalculateViewInfoProperties(intermediateResult, cellsInfo);
			AppointmentIntermediateViewInfo viewInfo = (AppointmentIntermediateViewInfo)intermediateResult;
			viewInfo.FirstCell = cellsInfo.Cells[intermediateResult.FirstCellIndex];
			viewInfo.LastCell = cellsInfo.Cells[intermediateResult.LastCellIndex];
		}
	}
	public abstract class HorizontalAppointmentLayoutCalculator : AppointmentBaseLayoutCalculator {
		protected internal abstract bool ShouldShowBorders(IAppointmentViewInfo aptViewInfo);
		protected HorizontalAppointmentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, AppointmentContentLayoutCalculator contentCalculator)
			: base(viewInfo, contentCalculator) {
		}
		protected internal override AppointmentIntermediateLayoutCalculatorCore CreateIntermediateLayoutCalculator() {
			return new HorizontalAppointmentIntermediateLayoutCalculator(Scale, AppointmentsSnapToCells, ViewInfo.View.Control.InnerControl.TimeZoneHelper);
		}
		protected internal override AppointmentControl CreateAppointmentControlInstance(AppointmentIntermediateViewInfo aptViewInfo) {
			return new HorizontalAppointmentControl(aptViewInfo);
		}
		protected internal override TimeScale CreateScale() {
			return new TimeScaleDay();
		}
		protected internal override void LayoutViewInfos(WebVisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateViewInfoCollection intermediateResult, bool isTwoPassLayout) {
		}
		protected internal override void CalculateViewInfoBorders(AppointmentIntermediateViewInfoCore intermediateResult) {
			IAppointmentViewInfo viewInfo = intermediateResult.ViewInfo;
			bool showBorders = ShouldShowBorders(viewInfo);
			viewInfo.HasTopBorder = true;
			viewInfo.HasBottomBorder = true;
			viewInfo.HasLeftBorder = intermediateResult.HasStartBorder && showBorders;
			viewInfo.HasRightBorder = intermediateResult.HasEndBorder && showBorders;
		}
	}
	public class VerticalAppointmentLayoutCalculator : AppointmentBaseLayoutCalculator {
		public VerticalAppointmentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, AppointmentContentLayoutCalculator contentCalculator)
			: base(viewInfo, contentCalculator) {
		}
		protected internal override AppointmentIntermediateLayoutCalculatorCore CreateIntermediateLayoutCalculator() {
			return new VerticalAppointmentIntermediateLayoutCalculator((TimeScaleFixedInterval)Scale, AppointmentsSnapToCells, ViewInfo.View.Control.InnerControl.TimeZoneHelper);
		}
		protected internal override AppointmentControl CreateAppointmentControlInstance(AppointmentIntermediateViewInfo aptViewInfo) {
			return new VerticalAppointmentControl(aptViewInfo);
		}
		protected internal override TimeScale CreateScale() {
			return new TimeScaleFixedInterval(((DayView)ViewInfo.View).TimeScale);
		}
		protected internal override void LayoutViewInfos(WebVisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateViewInfoCollection intermediateResult, bool isTwoPassLayout) {
			VerticalAppointmentLayoutCalculatorHelper helper = new VerticalAppointmentLayoutCalculatorHelper();
			helper.LayoutViewInfos(CancellationToken.None, cellsInfo, intermediateResult);
		}
		protected internal override void CalculateViewInfoBorders(AppointmentIntermediateViewInfoCore intermediateResult) {
			IAppointmentViewInfo viewInfo = intermediateResult.ViewInfo;
			viewInfo.HasRightBorder = true;
			viewInfo.HasLeftBorder = true;
			viewInfo.HasTopBorder = intermediateResult.HasStartBorder;
			viewInfo.HasBottomBorder = intermediateResult.HasEndBorder;
		}
	}
	#region DayViewShortAppointmentLayoutCalculator
	public class DayViewShortAppointmentLayoutCalculator : VerticalAppointmentLayoutCalculator {
		public DayViewShortAppointmentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, AppointmentContentLayoutCalculator contentCalculator)
			: base(viewInfo, contentCalculator) {
		}
	}
	#endregion
	#region DayViewLongAppointmentLayoutCalculator
	public class DayViewLongAppointmentLayoutCalculator : HorizontalAppointmentLayoutCalculator {
		public DayViewLongAppointmentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, AppointmentContentLayoutCalculator contentCalculator)
			: base(viewInfo, contentCalculator) {
		}
		protected internal override bool ShouldShowBorders(IAppointmentViewInfo aptViewInfo) {
			if (aptViewInfo.AppointmentInterval.SameDay)
				return ((WeekViewAppointmentDisplayOptions)ViewInfo.AppointmentDisplayOptions).ShowBordersForSameDayAppointments;
			else
				return true;
		}
		protected internal override AppointmentSnapToCellsMode AppointmentsSnapToCells { get { return AppointmentSnapToCellsMode.Always; } }
	}
	#endregion
	#region WorkWeekViewShortAppointmentLayoutCalculator
	public class WorkWeekViewShortAppointmentLayoutCalculator : DayViewShortAppointmentLayoutCalculator {
		public WorkWeekViewShortAppointmentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, AppointmentContentLayoutCalculator contentCalculator)
			: base(viewInfo, contentCalculator) {
		}
	}
	#endregion
	#region WorkWeekViewLongAppointmentLayoutCalculator
	public class WorkWeekViewLongAppointmentLayoutCalculator : DayViewLongAppointmentLayoutCalculator {
		public WorkWeekViewLongAppointmentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, AppointmentContentLayoutCalculator contentCalculator)
			: base(viewInfo, contentCalculator) {
		}
		protected internal override AppointmentSnapToCellsMode AppointmentsSnapToCells { get { return AppointmentSnapToCellsMode.Always; } }
	}
	#endregion
	#region WeekViewAppointmentLayoutCalculator
	public class WeekViewAppointmentLayoutCalculator : HorizontalAppointmentLayoutCalculator {
		public WeekViewAppointmentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, AppointmentContentLayoutCalculator contentCalculator)
			: base(viewInfo, contentCalculator) {
		}
		protected internal override AppointmentBaseComparer CreateAppointmentComparer() {
			return ViewInfo.AppointmentComparerProvider.CreateWeekViewAppointmentComparer();
		}
		protected internal override bool ShouldShowBorders(IAppointmentViewInfo aptViewInfo) {
			if (aptViewInfo.AppointmentInterval.SameDay)
				return ((WeekViewAppointmentDisplayOptions)ViewInfo.AppointmentDisplayOptions).ShowBordersForSameDayAppointments;
			else
				return true;
		}
	}
	#endregion
	#region MonthViewAppointmentLayoutCalculator
	public class MonthViewAppointmentLayoutCalculator : WeekViewAppointmentLayoutCalculator {
		public MonthViewAppointmentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, AppointmentContentLayoutCalculator contentCalculator)
			: base(viewInfo, contentCalculator) {
		}
	}
	#endregion
	#region TimeLineAppointmentLayoutCalculator
	public class TimeLineAppointmentLayoutCalculator : HorizontalAppointmentLayoutCalculator {
		public TimeLineAppointmentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, AppointmentContentLayoutCalculator contentCalculator)
			: base(viewInfo, contentCalculator) {
		}
		protected internal override AppointmentIntermediateLayoutCalculatorCore CreateIntermediateLayoutCalculator() {
			return new TimeLineAppointmentIntermediateLayoutCalculator(Scale, AppointmentsSnapToCells, ViewInfo.View.Control.InnerControl.TimeZoneHelper);
		}
		protected internal override TimeScale CreateScale() {
			return ((TimelineView)ViewInfo.View).GetBaseTimeScale();
		}
		protected internal override bool ShouldShowBorders(IAppointmentViewInfo viewInfo) {
			return true;
		}
	}
	#endregion
	#region HorizontalAppointmentIntermediateLayoutCalculator
	public class HorizontalAppointmentIntermediateLayoutCalculator : HorizontalAppointmentIntermediateLayoutCalculatorCore {
		public HorizontalAppointmentIntermediateLayoutCalculator(TimeScale scale, AppointmentSnapToCellsMode snapToCells, TimeZoneHelper timeZoneEngine)
			: base(scale, snapToCells, timeZoneEngine) {
		}
		protected internal override AppointmentIntermediateViewInfoCore CreateAppointmentIntermediateViewInfo(Appointment apt, IVisuallyContinuousCellsInfoCore cellsInfo) {
			return new AppointmentIntermediateViewInfo(new AppointmentViewInfo(apt));
		}
		protected internal override IAppointmentIntermediateViewInfoCoreCollection CreateIntermediateViewInfoCollection() {
			return new AppointmentIntermediateViewInfoCollection();
		}
		public override IAppointmentIntermediateViewInfoCoreCollection CreateIntermediateViewInfoCollection(XtraScheduler.Resource resource, TimeInterval interval) {
			return new AppointmentIntermediateViewInfoCollection(resource, interval);
		}
	}
	#endregion
	#region VerticalAppointmentIntermediateLayoutCalculator
	public class VerticalAppointmentIntermediateLayoutCalculator : VerticalAppointmentIntermediateLayoutCalculatorCore {
		public VerticalAppointmentIntermediateLayoutCalculator(TimeScaleFixedInterval scale, AppointmentSnapToCellsMode snapToCells, TimeZoneHelper timeZoneEngine)
			: base(scale, snapToCells, timeZoneEngine) {
		}
		protected internal override AppointmentIntermediateViewInfoCore CreateAppointmentIntermediateViewInfo(Appointment apt, IVisuallyContinuousCellsInfoCore cellsInfo) {
			return new AppointmentIntermediateViewInfo(new AppointmentViewInfo(apt));
		}
		protected internal override IAppointmentIntermediateViewInfoCoreCollection CreateIntermediateViewInfoCollection() {
			return new AppointmentIntermediateViewInfoCollection();
		}
		public override IAppointmentIntermediateViewInfoCoreCollection CreateIntermediateViewInfoCollection(XtraScheduler.Resource resource, TimeInterval interval) {
			return new AppointmentIntermediateViewInfoCollection(resource, interval);
		}
	}
	#endregion
	#region TimeLineAppointmentIntermediateLayoutCalculator
	public class TimeLineAppointmentIntermediateLayoutCalculator : HorizontalAppointmentIntermediateLayoutCalculator {
		public TimeLineAppointmentIntermediateLayoutCalculator(TimeScale scale, AppointmentSnapToCellsMode snapToCells, TimeZoneHelper timeZoneEngine)
			: base(scale, snapToCells, timeZoneEngine) {
		}
		protected internal override AppointmentIntermediateViewInfoCore CreateAppointmentIntermediateViewInfo(Appointment apt, IVisuallyContinuousCellsInfoCore cellsInfo) {
			return new AppointmentIntermediateViewInfo(new TimeLineAppointmentViewInfo(apt));
		}
	}
	#endregion
	#region VerticalAppointmentLayoutCalculatorHelper
	public class VerticalAppointmentLayoutCalculatorHelper : VerticalAppointmentLayoutCalculatorHelperCore<WebVisuallyContinuousCellsInfo> {
	}
	#endregion
}
