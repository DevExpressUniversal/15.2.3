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
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.XtraScheduler.Services;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.Web.ASPxScheduler.Rendering {
	public class WebTimeCell : InternalSchedulerCell, IWebTimeCell, ISupportAnchors {
		#region Fields
		TimeInterval interval;
		XtraScheduler.Resource resource;
		bool isWorkTime;
		bool endOfHour;
		#endregion
		public WebTimeCell(TimeInterval interval, XtraScheduler.Resource resource) {
			if (interval == null)
				Exceptions.ThrowArgumentNullException("interval");
			if (resource == null)
				Exceptions.ThrowArgumentNullException("resource");
			this.interval = interval;
			this.resource = resource;
		}
		#region Properties
		public TimeInterval Interval { get { return interval; } }
		public InternalSchedulerCell Cell { get { return this; } }
		public Resource Resource { get { return resource; } }
		public override WebElementType CellType { get { return WebElementType.TimeCellBody; } }
		public bool IsWorkTime { get { return isWorkTime; } set { isWorkTime = value; } }
		public bool EndOfHour { get { return endOfHour; } set { endOfHour = value; } }
		public TableCell ContentCell { get { return this; } }
		#endregion
		#region IWebViewInfo Members
		public override SchedulerTable CreateTable() {
			return TableHelper.CreateTableWithOneCell(Cell);
		}
		#endregion
		#region ISupportAnchors Members
		public void CreateLeftTopAnchor(AnchorCollection anchors) {
			anchors.Add(new Anchor(Cell, Resource, AnchorType.Left));
		}
		public void CreateRightBottomAnchor(AnchorCollection anchors) {
			anchors.Add(new Anchor(Cell, Resource, AnchorType.Right));
		}
		#endregion
		protected internal override ITimeCell GetTimeCell() {
			return this;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			ASPxScheduler.ActiveControl.RaiseHtmlTimeCellPrepared(this, this, ASPxScheduler.ActiveControl.ActiveView);
		}
		#region IWebTimeCell Members
		string IWebTimeCell.Id { get { return base.Id; } }
		#endregion
	}
	public class WebTimeCellContainer : VerticalMergingContainer, IWebCellContainer, IWorkTimeInfo {
		#region Fields
		WebTimeCellCollection cells;
		TimeInterval interval;
		XtraScheduler.Resource resource;
		bool isWorkDay;
		TimeOfDayIntervalCollection workTimes;
		#endregion
		public WebTimeCellContainer(DayView view, TimeInterval interval, XtraScheduler.Resource resource) {
			Guard.ArgumentNotNull(view, "view");
			Guard.ArgumentNotNull(interval, "interval");
			Guard.ArgumentNotNull(resource, "resource");
			if (!DateTimeHelper.IsExactlyOneDay(interval))
				Exceptions.ThrowArgumentException("interval", interval);
			this.interval = interval;
			this.resource = resource;
			DayViewWorkTimeInfoCalculator workTimeInfoCalculator = new DayViewWorkTimeInfoCalculator((InnerDayView)view.InnerView);
			WorkTimeInfo workTimeInfo = workTimeInfoCalculator.CalcWorkTimeInfo(this.Interval, (XtraScheduler.Resource)this.Resource);
			this.workTimes = workTimeInfo.WorkTimes;
			this.IsWorkDay = workTimeInfo.IsWorkDay;
			this.cells = CreateCellCollection(view.ActualVisibleTime, view.TimeScale);
			CreateContent(Cells);
		}
		#region Properties
		public WebTimeCellCollection Cells { get { return cells; } }
		public TimeInterval Interval { get { return interval; } }
		public Resource Resource { get { return resource; } }
		public bool IsWorkDay { get { return isWorkDay; } set { isWorkDay = value; } }
		public TimeOfDayIntervalCollection WorkTimes { get { return workTimes; } }
		#endregion
		void CreateContent(WebTimeCellCollection cells) {
			int count = cells.Count;
			for (int i = 0; i < count; i++)
				WebObjects.Add(cells[i]);
			if (count > 0)
				cells[count - 1].IgnoreBorderSide = IgnoreBorderSide.Bottom;
		}
		protected internal virtual WebTimeCellCollection CreateCellCollection(TimeOfDayInterval visibleTime, TimeSpan timeScale) {
			TimeOfDayIntervalCollection timeOfDayIntervalCollection = CalculateActualIntervals(visibleTime, timeScale);
			WebTimeCellCollection webTimeCellCollection = new WebTimeCellCollection();
			int count = timeOfDayIntervalCollection.Count;
			for (int i = 0; i < count; i++) {
				TimeOfDayInterval adjustedTimeOfDayInterval = DayViewCellsCalculatorHelper.CreateAlignedVisibleTime(timeOfDayIntervalCollection[i], timeScale, false); ;
				TimeSpan start = adjustedTimeOfDayInterval.Start;
				TimeInterval cellInterval = new TimeInterval(Interval.Start + start, adjustedTimeOfDayInterval.Duration);
				WebTimeCell cell = new WebTimeCell(cellInterval, Resource);
				InitializeCell(cell, Interval);
				webTimeCellCollection.Add(cell);
			}
			return webTimeCellCollection;
		}
		static TimeOfDayIntervalCollection CalculateActualIntervals(TimeOfDayInterval visibleTime, TimeSpan timeScale) {
			TimeOfDayInterval adjustedVisibleTime = DayViewCellsCalculatorHelper.CreateAlignedVisibleTime(visibleTime, timeScale, false);
			return TableHelper.SplitInterval(adjustedVisibleTime, timeScale);
		}
		protected internal virtual void InitializeCell(WebTimeCell cell, TimeInterval interval) {
			TimeInterval cellInterval = cell.Interval;
			cell.IsWorkTime = DayViewTimeCellHelper.IsWorkTime(this, cellInterval, interval);
			cell.EndOfHour = DateTimeHelper.IsBeginOfHour(cellInterval.End);
		}
		public void SetId(int containerIndex) {
			int count = Cells.Count;
			string id = GenerateId(containerIndex);
			for (int i = 0; i < count; i++)
				Cells[i].Cell.ID = String.Format("{0}_{1}", id, i);
		}
		public string GenerateId(int containerIndex) {
			return String.Format("v{0}", containerIndex);
		}
		#region IWebCellContainer Members
		int IWebCellContainer.CellCount { get { return Cells.Count; } }
		IWebTimeCell IWebCellContainer.this[int index] { get { return Cells[index]; } }
		CellContainerType IWebCellContainer.ContainerType { get { return CellContainerType.Vertical; } }
		#endregion
		protected internal virtual void SetIsLastColumn() {
			CalculateCellsBordersVisibility(Cells);
		}
		protected internal virtual void CalculateCellsBordersVisibility(WebTimeCellCollection cells) {
			int count = cells.Count;
			for (int i = 0; i < count; i++)
				cells[i].IgnoreBorderSide |= IgnoreBorderSide.Right;
		}
	}
	public class WebAllDayAreaCellsContainter : IWebCellContainer {
		WebAllDayAreaCellCollection cells;
		public WebAllDayAreaCellsContainter(WebAllDayAreaCellCollection cells) {
			if (cells == null)
				Exceptions.ThrowArgumentNullException("cells");
			this.cells = cells;
		}
		#region IWebCellContainer members
		public int CellCount { get { return cells.Count; } }
		public Resource Resource {
			get {
				if (CellCount == 0)
					return ResourceBase.Empty;
				else
					return cells[0].Resource;
			}
		}
		public CellContainerType ContainerType { get { return CellContainerType.Horizontal; } }
		public IWebTimeCell this[int index] { get { return cells[index]; } }
		#endregion
	}
	public class WebAllDayAreaCell : InternalSchedulerCell, IWebTimeCell {
		#region Fields
		TimeInterval interval;
		XtraScheduler.Resource resource;
		#endregion
		public WebAllDayAreaCell(TimeInterval interval, XtraScheduler.Resource resource) {
			if (interval == null)
				Exceptions.ThrowArgumentNullException("interval");
			if (!DateTimeHelper.IsExactlyOneDay(interval))
				Exceptions.ThrowArgumentException("interval", interval);
			if (resource == null)
				Exceptions.ThrowArgumentNullException("resource");
			this.interval = interval;
			this.resource = resource;
		}
		#region Properties
		public InternalSchedulerCell Cell { get { return this; } }
		public Resource Resource { get { return resource; } }
		public TimeInterval Interval { get { return interval; } }
		public override WebElementType CellType { get { return WebElementType.AllDayArea; } }
		public TableCell ContentCell { get { return this; } }
		#endregion
		#region IWebViewInfo Members
		public override SchedulerTable CreateTable() {
			Cell.Text = "&nbsp;";
			return TableHelper.CreateTableWithOneCell(Cell);
		}
		#endregion
		#region IWebTimeCell Members
		string IWebTimeCell.Id { get { return base.Id; } }
		#endregion
	}
	public class WebDayViewColumn : IWebViewInfo, ISupportAnchors {
		#region Fields
		WebAllDayAreaCell allDayAreaCell;
		WebTimeCellContainer cells;
		TimeInterval interval;
		XtraScheduler.Resource resource;
		MergingContainerBase mergingContainer;
		bool showAllDayArea;
		#endregion
		public WebDayViewColumn(DayView view, TimeInterval interval, XtraScheduler.Resource resource) {
			this.interval = interval;
			this.resource = resource;
			this.allDayAreaCell = new WebAllDayAreaCell(Interval, Resource);
			this.cells = new WebTimeCellContainer(view, interval, resource);
			this.showAllDayArea = view.ShowAllDayArea;
			mergingContainer = new VerticalMergingContainer();
			if (view.ShowAllDayArea)
				mergingContainer.WebObjects.Add(AllDayAreaCell);
			mergingContainer.WebObjects.Add(Cells);
		}
		#region Properties
		public Resource Resource { get { return resource; } }
		public TimeInterval Interval { get { return interval; } }
		protected internal WebAllDayAreaCell AllDayAreaCell { get { return allDayAreaCell; } }
		protected internal WebTimeCellContainer Cells { get { return cells; } }
		protected internal MergingContainerBase MergingContainer { get { return mergingContainer; } }
		public bool ShowAllDayArea { get { return showAllDayArea; } }
		#endregion
		#region IWebViewInfo Members
		public SchedulerTable CreateTable() {
			return MergingContainer.CreateTable();
		}
		#endregion
		#region ISupportAnchors Members
		public void CreateLeftTopAnchor(AnchorCollection anchors) {
			MergingContainer.CreateLeftTopAnchor(anchors);
		}
		public void CreateRightBottomAnchor(AnchorCollection anchors) {
			MergingContainer.CreateRightBottomAnchor(anchors);
		}
		#endregion
		internal void SetIsLastColumn() {
			Cells.SetIsLastColumn();
		}
	}
	public abstract class WebDayViewInfoBase : WebViewInfoBase<WebDayViewColumn> {
		List<WebTimeRuler> timeRulers;
		protected WebDayViewInfoBase(DayView view)
			: base(view) {
			this.timeRulers = new List<WebTimeRuler>();
		}
		public new DayView View { get { return (DayView)base.View; } }
		public List<WebTimeRuler> TimeRulers { get { return timeRulers;} }
		protected internal virtual int TimeRulerHeaderSpan {
			get {
				int result = 0;
				if (View.ShowAllDayArea)
					result++;
				if (View.ShowDayHeaders)
					result++;
				return result;
			}
		}
		protected internal override IWebViewInfo CreateAdditionalViewElements() {
			HorizontalMergingContainer result = new HorizontalMergingContainer();
			this.timeRulers = CreateTimeRulers();
			int count = TimeRulers.Count;
			for (int i = 0; i < count; i++) {
				WebTimeRuler ruler = TimeRulers[i];
				result.WebObjects.Add(ruler);
			}
			return result;
		}
		List<WebTimeRuler> CreateTimeRulers() {
			ITimeRulerFormatStringService formatStringProvider = (ITimeRulerFormatStringService)View.Control.GetService(typeof(ITimeRulerFormatStringService));
			TimeZoneHelper timeZoneEngine = View.Control.InnerControl.TimeZoneHelper;
			int timeRulerHeaderSpan = this.TimeRulerHeaderSpan;
			TimeOfDayInterval interval = View.ActualVisibleTime;
			TimeSpan scale = View.TimeScale;
			List<WebTimeRuler> result = new List<WebTimeRuler>();
			TimeRulerCollection rulers = View.InnerView.GetVisibleTimeRulers();
			int count = rulers.Count;
			for (int i = 0; i < count; i++) {
				TimeRuler timeRuler = rulers[i];
				TimeFormatInfo timeFormatInfo = new TimeFormatInfo();
				timeFormatInfo.Initialize(timeRuler, formatStringProvider);
				WebTimeRuler ruler = new WebTimeRuler(View.VisibleStart, timeRuler, interval, timeZoneEngine, scale, i == 0, timeRulerHeaderSpan, timeFormatInfo);
				result.Add(ruler);
			}
			return result;
		}
		protected internal override MergingContainerBase CreateMergingContainer() {
			return new HorizontalMergingContainer();
		}
		protected internal override List<WebDayViewColumn> CreateResourceCellContainers(XtraScheduler.Resource resource) {
			List<WebDayViewColumn> result = base.CreateResourceCellContainers(resource);
			int count = result.Count;
			if (count > 0)
				result[count - 1].SetIsLastColumn();
			return result;
		}
		protected internal override WebDayViewColumn CreateCellContainer(TimeInterval interval, XtraScheduler.Resource resource) {
			return new WebDayViewColumn(View, interval, resource);
		}
		protected internal override IWebViewInfo CreateResourceCellContainerViewInfo(WebDayViewColumn column) {
			VerticalMergingContainer result = new VerticalMergingContainer();
			IWebViewInfo header = CreateColumnHeader(column);
			result.WebObjects.Add(header);
			result.WebObjects.Add(column);
			return result;
		}
		protected internal override MergingContainerBase CreateResourceHeaderAndCellContainersMergingContainer() {
			return new VerticalMergingContainer();
		}
		protected internal override MergingContainerBase CreateCellContainersMergingContainer() {
			return new HorizontalMergingContainer();
		}
		protected internal virtual IWebViewInfo CreateColumnHeader(WebDayViewColumn column) {
			if (!View.ShowDayHeaders)
				return new EmptyWebViewInfo();
			WebDateHeader header = new WebDateHeader(column.Interval, column.Resource);
			HeaderFormatSeparator.Add(header);
			return header;
		}
		public override void CreateLeftTopAnchor(AnchorCollection anchors) {
			int count = ResourcesCellContainers.Count;
			for (int i = 0; i < count; i++) {
				ResourcesCellContainers[i][0].CreateLeftTopAnchor(anchors);
			}
		}
		#region ISchedulerWebViewInfoBase Members
		protected internal override WebCellContainerCollection GetWebCellContainers(List<WebDayViewColumn> resourceCellContainers) {
			WebCellContainerCollection result = base.GetWebCellContainers(resourceCellContainers);
			if (View.ShowAllDayArea)
				AddAllDayCellsContainers(result, resourceCellContainers);
			return result;
		}
		protected internal virtual void AddAllDayCellsContainers(WebCellContainerCollection target, List<WebDayViewColumn> resourceCellContainers) {
			List<WebAllDayAreaCellsContainter> allDayCellContainers = GetAllDayCellsContainers(resourceCellContainers);
			int count = allDayCellContainers.Count;
			for (int i = 0; i < count; i++)
				target.Add(allDayCellContainers[i]);
		}
		protected internal virtual List<WebAllDayAreaCellsContainter> GetAllDayCellsContainers(List<WebDayViewColumn> resourceCellContainers) {
			WebAllDayAreaCellCollection allDayAreaCells = new WebAllDayAreaCellCollection();
			int count = resourceCellContainers.Count;
			for (int j = 0; j < count; j++) {
				WebAllDayAreaCell allDayCell = ((WebDayViewColumn)resourceCellContainers[j]).AllDayAreaCell;
				allDayAreaCells.Add(allDayCell);
			}
			List<WebAllDayAreaCellsContainter> result = new List<WebAllDayAreaCellsContainter>();
			result.Add(new WebAllDayAreaCellsContainter(allDayAreaCells));
			return result;
		}
		protected internal override IWebCellContainer GetWebCellContainer(WebDayViewColumn resourceContainer) {
			return resourceContainer.Cells;
		}
		#endregion
		protected override HeaderFormatSeparatorBase CreateHeadersFormatSeparator() {
			return new DayViewHeaderFormatSeparator();
		}
	}
	public class WebDayViewGroupByNone : WebDayViewInfoBase {
		public WebDayViewGroupByNone(DayView view)
			: base(view) {
		}
		public override ResourceBaseCollection Resources { get { return WebWeekViewInfoBase.EmptyResourceCollection; } }
		protected internal override IWebViewInfo CreateResourceHeader(XtraScheduler.Resource resource) {
			return new EmptyWebViewInfo();
		}
		public override int GetResourceColorIndex(XtraScheduler.Resource resource) {
			return 0;
		}
	}
	public class WebDayViewGroupByResource : WebDayViewInfoBase {
		#region Fields
		GroupSeparatorDayViewHelper groupSeparatorDayViewHelper;
		#endregion
		public WebDayViewGroupByResource(DayView view)
			: base(view) {
			this.groupSeparatorDayViewHelper = new GroupSeparatorDayViewHelper();
		}
		#region Properties
		protected internal override int TimeRulerHeaderSpan { get { return base.TimeRulerHeaderSpan + 1; } }
		internal GroupSeparatorDayViewHelper GroupSeparatorDayViewHelper { get { return groupSeparatorDayViewHelper; } }
		#endregion
		#region CreateGroupSeparator
		protected internal override void CreateGroupSeparator(SchedulerWebViewInfoCollection webObjects, IWebViewInfo resourceView) {
			HideRightBorder(resourceView);
			GroupSeparatorDayViewHelper.CreateGroupSeparator(View, webObjects, resourceView);
		}
		#endregion
		#region HideRightBorder
		protected internal virtual void HideRightBorder(IWebViewInfo resourceView) {
			VerticalMergingContainer singleResourceView = (VerticalMergingContainer)resourceView;
			WebHorizontalResourceHeader resourceHeader = (WebHorizontalResourceHeader)singleResourceView.WebObjects[0];
			HorizontalMergingContainer cellContainers = (HorizontalMergingContainer)singleResourceView.WebObjects[1];
			VerticalMergingContainer dayContainer = (VerticalMergingContainer)cellContainers.WebObjects[cellContainers.WebObjects.Count - 1];
			WebDateHeader dateHeader = dayContainer.WebObjects[0] as WebDateHeader; 
			WebDayViewColumn dayViewColumn = (WebDayViewColumn)dayContainer.WebObjects[1];
			resourceHeader.IgnoreBorderSide |= IgnoreBorderSide.Right;
			if (dateHeader != null)
				dateHeader.IgnoreBorderSide |= IgnoreBorderSide.Right;
			dayViewColumn.AllDayAreaCell.IgnoreBorderSide = IgnoreBorderSide.Right;
		}
		#endregion
		#region CreateResourceHeader
		protected internal override IWebViewInfo CreateResourceHeader(XtraScheduler.Resource resource) {
			WebHorizontalResourceHeader header = new WebHorizontalResourceHeader(VisibleIntervals, resource);
			header.IgnoreBorderSide = IgnoreBorderSide.Top;
			return header;
		}
		#endregion
	}
	public class WebDayViewGroupByDate : WebDayViewInfoBase {
		#region Fields
		GroupSeparatorDayViewHelper groupSeparatorDayViewHelper;
		#endregion
		public WebDayViewGroupByDate(DayView view)
			: base(view) {
			this.groupSeparatorDayViewHelper = new GroupSeparatorDayViewHelper();
		}
		protected internal override int TimeRulerHeaderSpan { get { return base.TimeRulerHeaderSpan + 1; } }
		#region Properties
		internal GroupSeparatorDayViewHelper GroupSeparatorDayViewHelper { get { return groupSeparatorDayViewHelper; } }
		#endregion
		protected internal override IWebViewInfo CreateResourcesView() {
			MergingContainerBase result = CreateMergingContainer();
			InitializeResourceColumns();
			int count = VisibleIntervals.Count;
			for (int i = 0; i < count; i++) {
				IWebViewInfo resourceView = CreateSingleDateView(VisibleIntervals[i]);
				result.WebObjects.Add(resourceView);
				if (i + 1 < count)
					CreateGroupSeparator(result.WebObjects, resourceView);
			}
			return result;
		}
		protected internal override void CreateGroupSeparator(SchedulerWebViewInfoCollection webObjects, IWebViewInfo resourceView) {
			GroupSeparatorDayViewHelper.CreateGroupSeparator(View, webObjects, resourceView);
			HideRightBorder(resourceView);
		}
		#region HideRightBorder
		protected internal virtual void HideRightBorder(IWebViewInfo resourceView) {
			VerticalMergingContainer singleResourceView = (VerticalMergingContainer)resourceView;
			WebDateHeader dateHeader = (WebDateHeader)singleResourceView.WebObjects[0];
			HorizontalMergingContainer cellContainers = (HorizontalMergingContainer)singleResourceView.WebObjects[1];
			VerticalMergingContainer dayContainer = (VerticalMergingContainer)cellContainers.WebObjects[cellContainers.WebObjects.Count - 1];
			WebHorizontalResourceHeader resourceHeader = (WebHorizontalResourceHeader)dayContainer.WebObjects[0];
			WebDayViewColumn dayViewColumn = (WebDayViewColumn)dayContainer.WebObjects[1];
			resourceHeader.IgnoreBorderSide |= IgnoreBorderSide.Right;
			dateHeader.IgnoreBorderSide |= IgnoreBorderSide.Right;
			dayViewColumn.AllDayAreaCell.IgnoreBorderSide = IgnoreBorderSide.Right;
		}
		#endregion
		protected internal virtual void InitializeResourceColumns() {
			int count = Resources.Count;
			for (int i = 0; i < count; i++)
				ResourcesCellContainers.Add(new WebDayViewColumnCollection());
		}
		protected internal virtual IWebViewInfo CreateSingleDateView(TimeInterval interval) {
			IWebViewInfo dateHeader = CreateDateHeader(interval);
			MergingContainerBase columns = CreateDateColumns(interval);
			VerticalMergingContainer result = new VerticalMergingContainer();
			result.WebObjects.Add(dateHeader);
			result.WebObjects.Add(columns);
			return result;
		}
		protected internal virtual IWebViewInfo CreateDateHeader(TimeInterval interval) {
			if (!View.ShowDayHeaders)
				return new EmptyWebViewInfo();
			WebDateHeader header = new WebDateHeader(interval, Resources);
			HeaderFormatSeparator.Add(header);
			return header;
		}
		protected internal virtual MergingContainerBase CreateDateColumns(TimeInterval interval) {
			MergingContainerBase result = CreateMergingContainer();
			int count = Resources.Count;
			for (int i = 0; i < count; i++) {
				WebDayViewColumn column = CreateCellContainer(interval, Resources[i]);
				if (i == count - 1)
					column.SetIsLastColumn();
				ResourcesCellContainers[i].Add(column);
				IWebViewInfo columnViewInfo = CreateResourceCellContainerViewInfo(column);
				result.WebObjects.Add(columnViewInfo);
			}
			return result;
		}
		protected internal override IWebViewInfo CreateColumnHeader(WebDayViewColumn column) {
			return new WebHorizontalResourceHeader(column.Interval, column.Resource);
		}
		public override void CreateLeftTopAnchor(AnchorCollection anchors) {
			int count = ResourcesCellContainers.Count;
			for (int i = 0; i < count; i++)
				ResourcesCellContainers[i][0].CreateLeftTopAnchor(anchors);
		}
		public override void CreateRightBottomAnchor(AnchorCollection anchors) {
			int count = ResourcesCellContainers.Count;
			for (int i = 0; i < count; i++) {
				int containerCount = ResourcesCellContainers[i].Count;
				ResourcesCellContainers[i][containerCount - 1].CreateRightBottomAnchor(anchors);
			}
		}
		protected internal override List<WebAllDayAreaCellsContainter> GetAllDayCellsContainers(List<WebDayViewColumn> resourceCellContainers) {
			List<WebAllDayAreaCellsContainter> result = new List<WebAllDayAreaCellsContainter>();
			int count = resourceCellContainers.Count;
			for (int j = 0; j < count; j++) {
				WebAllDayAreaCellCollection allDayAreaCells = new WebAllDayAreaCellCollection();
				WebAllDayAreaCell allDayCell = ((WebDayViewColumn)resourceCellContainers[j]).AllDayAreaCell;
				allDayAreaCells.Add(allDayCell);
				result.Add(new WebAllDayAreaCellsContainter(allDayAreaCells));
			}
			return result;
		}
	}
	public class WebWorkWeekViewGroupByNone : WebDayViewGroupByNone {
		bool showFullWeek;
		public WebWorkWeekViewGroupByNone(WorkWeekView view)
			: base(view) {
			this.showFullWeek = view.ShowFullWeek;
		}
		public bool ShowFullWeek { get { return showFullWeek; } }
	}
	public class WebWorkWeekViewGroupByDate : WebDayViewGroupByDate {
		bool showFullWeek;
		public WebWorkWeekViewGroupByDate(WorkWeekView view)
			: base(view) {
			this.showFullWeek = view.ShowFullWeek;
		}
		public bool ShowFullWeek { get { return showFullWeek; } }
	}
	public class WebWorkWeekViewGroupByResource : WebDayViewGroupByResource {
		bool showFullWeek;
		public WebWorkWeekViewGroupByResource(WorkWeekView view)
			: base(view) {
			this.showFullWeek = view.ShowFullWeek;
		}
		public bool ShowFullWeek { get { return showFullWeek; } }
	}
	public static class DayViewCellsCalculatorHelper {
		public static TimeOfDayInterval CreateAlignedVisibleTime(TimeOfDayInterval visibleTime, TimeSpan timeScale, bool visibleTimeSnapMode) {
			TimeSpan start, end;
			if (!visibleTimeSnapMode) {
				start = DateTimeHelper.Floor(visibleTime.Start, timeScale);
				end = DateTimeHelper.Ceil(visibleTime.End, timeScale);
			}
			else {
				start = visibleTime.Start;
				end = DateTimeHelper.Ceil(visibleTime.End, timeScale, visibleTime.Start);
			}
			return new TimeOfDayInterval(start, end);
		}
	}
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	public class GroupSeparatorDayViewHelper {
		const int ResourceHeaderRowCount = 1;
		public virtual void CreateGroupSeparator(DayView view, SchedulerWebViewInfoCollection webObjects, IWebViewInfo viewInfo) {
			VerticalMergingContainer container = (VerticalMergingContainer)viewInfo;
			WebDayViewColumn dayViewColumn = GetDayViewColumn(container);
			int scrolRowCount = GetTimeCellCount(dayViewColumn);
			int headerRowCount = GetHeaderRowCount(view, dayViewColumn);
			WebGroupSeparator headerSeparator = new WebGroupSeparatorVertical(true, headerRowCount);
			WebGroupSeparator bodySeparator = new WebGroupSeparatorVertical(false, scrolRowCount);
			VerticalMergingContainer separatorContainer = new VerticalMergingContainer();
			separatorContainer.WebObjects.Add(headerSeparator);
			separatorContainer.WebObjects.Add(bodySeparator);
			webObjects.Add(separatorContainer);
		}
		protected internal virtual WebDayViewColumn GetDayViewColumn(VerticalMergingContainer container) {
			HorizontalMergingContainer resourceColumns = (HorizontalMergingContainer)container.WebObjects[1];
			VerticalMergingContainer resourceColumn = (VerticalMergingContainer)resourceColumns.WebObjects[0];
			WebDayViewColumn dayViewColumn = (WebDayViewColumn)resourceColumn.WebObjects[1];
			return dayViewColumn;
		}
		protected internal virtual int GetTimeCellCount(WebDayViewColumn dayViewColumn) {
			return dayViewColumn.Cells.Cells.Count;
		}
		protected internal virtual int GetHeaderRowCount(DayView view, WebDayViewColumn dayViewColumn) {
			return ResourceHeaderRowCount + (view.ShowDayHeaders ? 1 : 0) + ((dayViewColumn.ShowAllDayArea) ? 1 : 0);
		}
	}
}
