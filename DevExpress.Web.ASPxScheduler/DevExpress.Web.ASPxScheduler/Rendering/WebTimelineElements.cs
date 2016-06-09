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
using System.Web.UI;
using DevExpress.Web.ASPxScheduler.Drawing;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxScheduler.Internal;
using System.Globalization;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.Web.ASPxScheduler.Services;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.Web.ASPxScheduler.Rendering {
	#region WebTimelineHeader
	public class WebTimelineHeader : VerticalMergingContainer {
		#region Fields
		WebTimeScaleHeaderLevelCollection headerLevels;
		WebTimeScaleHeaderLevel baseLevel;
		TimeIntervalCollection intervals;
		WebEmptyTableCellCollection upperHeaders;
		InternalSchedulerCell lastBindCell;
		#endregion
		public WebTimelineHeader(TimeScaleCollection scales, DateTime start, int intervalCount) {
			if (scales == null)
				Exceptions.ThrowArgumentNullException("scales");
			if (scales.Count == 0)
				Exceptions.ThrowArgumentException("scales.Count", scales.Count);
			TimeScaleLevelsCalculator calculator = CreateCalculator();
			TimeScaleLevelCollection levels = calculator.Calculate(scales, start, intervalCount);
			this.baseLevel = CreateBaseLevel(levels);
			this.intervals = baseLevel.Level.Intervals;
			this.upperHeaders = new WebEmptyTableCellCollection();
			WebTimeScaleHeaderLevelCollection upperLevels = CreateUpperLevels(levels, BaseLevel);
			this.headerLevels = CreateHeaderLevels(BaseLevel, upperLevels);
			CreateContent(WebObjects, HeaderLevels);
			CalculateHeaderLevelsBordersVisibility(HeaderLevels);
		}
		#region Properties
		public WebTimeScaleHeaderLevelCollection HeaderLevels { get { return headerLevels; } }
		public WebTimeScaleHeaderLevel BaseLevel { get { return baseLevel; } }
		public TimeIntervalCollection Intervals { get { return intervals; } }
		public WebEmptyTableCellCollection UpperHeaders { get { return upperHeaders; } }
		internal InternalSchedulerCell LastBindCell { get { return lastBindCell; } }
		#endregion
		public void BindToCells(WebTimelineCellCollection cells) {
			int count = cells.Count;
			XtraSchedulerDebug.Assert(count == baseLevel.Headers.Count);
			XtraSchedulerDebug.Assert(count > 0);
			for (int i = 0; i < count; i++) {
				WebTimeScaleHeader header = baseLevel.Headers[i];
				header.Offset.LinkCell = cells[i];
			}
			this.lastBindCell = cells[count - 1];
		}
		protected internal virtual TimeScaleLevelsCalculator CreateCalculator() {
			return new TimeScaleLevelsCalculator();
		}
		protected internal virtual WebTimeScaleHeaderLevelCollection CreateHeaderLevels(WebTimeScaleHeaderLevel baseLevel, WebTimeScaleHeaderLevelCollection upperLevels) {
			WebTimeScaleHeaderLevelCollection result = new WebTimeScaleHeaderLevelCollection();
			result.AddRange(upperLevels);
			result.Add(baseLevel);
			return result;
		}
		protected internal virtual WebTimeScaleHeaderLevel CreateBaseLevel(TimeScaleLevelCollection levels) {
			int count = levels.Count;
			XtraSchedulerDebug.Assert(count != 0);
			return new WebTimeScaleHeaderLevel(levels[count - 1]);
		}
		protected internal virtual WebTimeScaleHeaderLevelCollection CreateUpperLevels(TimeScaleLevelCollection levels, WebTimeScaleHeaderLevel baseHeaderLevel) {
			WebTimeScaleHeaderLevelCollection result = new WebTimeScaleHeaderLevelCollection();
			int count = levels.Count;
			for (int i = 0; i < count - 1; i++) {
				WebTimeScaleHeaderLevel upperHeader = new WebTimeScaleHeaderLevel(levels[i]);
				upperHeader.CalculateRelativeOffset(baseHeaderLevel);
				result.Add(upperHeader);
			}
			return result;
		}
		protected internal virtual void CreateContent(SchedulerWebViewInfoCollection webObjects, WebTimeScaleHeaderLevelCollection headerLevels) {
			int count = headerLevels.Count;
			if (count == 0)
				return;
			WebTimeScaleHeaderLevel baseHeaderLevel = headerLevels[count - 1];
			for (int i = 0; i < count - 1; i++) {
				WebTimeScaleHeaderLevel headerLevel = headerLevels[i];
				if (headerLevel.Level.Scale.Visible) {
					WebEmptyTableCell cell = new WebEmptyTableCell();
					cell.InnerTables.Add(CreateHeaderLevelContentTable(headerLevel));
					UpperHeaders.Add(cell);
					webObjects.Add(cell);
					((ISchedulerCell)cell).ColumnSpan = baseHeaderLevel.Headers.Count;
				}
			}
			if (baseHeaderLevel.Level.Scale.Visible)
				webObjects.Add(baseHeaderLevel);
		}
		private SchedulerTable CreateHeaderLevelContentTable(WebTimeScaleHeaderLevel headerLevel) {
			SchedulerTable table = headerLevel.CreateTable();
			table.InnerStyles.Add(HtmlTextWriterStyle.BorderCollapse, "separate");
			table.InnerStyles.Add("table-layout", "fixed");
			table.InnerStyles.Add(HtmlTextWriterStyle.Width, "100%");
			return table;
		}
		protected internal virtual void CalculateHeaderLevelsBordersVisibility(WebTimeScaleHeaderLevelCollection headerLevels) {
			int count = headerLevels.Count;
			bool isLastLevel = true;
			int firstVisibleLevelIndex = count - 1;
			for (int i = count - 1; i >= 0; i--) {
				if (headerLevels[i].Level.Scale.Visible) {
					CalculateSingleHeaderLevelBordersVisibility(headerLevels[i], isLastLevel);
					isLastLevel = false;
					firstVisibleLevelIndex = i;
				}
			}
			CorrectFirstSingleHeaderLevelBordersVisibility(headerLevels[firstVisibleLevelIndex]);
		}
		protected internal virtual void CalculateSingleHeaderLevelBordersVisibility(WebTimeScaleHeaderLevel headerLevel, bool isLastLevel) {
			WebTimeScaleHeaderCollection headers = headerLevel.Headers;
			int count = headers.Count;
			if (count == 0)
				return;
			IgnoreBorderSide bottomBorder = isLastLevel ? 0 : IgnoreBorderSide.Bottom;
			for (int i = 0; i < count - 1; i++)
				headers[i].IgnoreBorderSide = bottomBorder;
			headers[count - 1].IgnoreBorderSide = bottomBorder | IgnoreBorderSide.Right;
		}
		protected internal virtual void CorrectFirstSingleHeaderLevelBordersVisibility(WebTimeScaleHeaderLevel headerLevel) {
		}
	}
	#endregion
	#region WebEmptyTableCell
	public class WebEmptyTableCell : InternalSchedulerCell {
		public WebEmptyTableCell() {
		}
		public override WebElementType CellType { get { return WebElementType.None; } }
		#region IWebViewInfo Members
		public override SchedulerTable CreateTable() {
			return TableHelper.CreateTableWithOneCell(this);
		}
		#endregion
		protected internal override void SetDefaultContent() {
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			this.Style.Add(HtmlTextWriterStyle.Overflow, "hidden");
		}
	}
	#endregion
	#region WebEmptyTableCellCollection
	public class WebEmptyTableCellCollection : List<WebEmptyTableCell> {
	}
	#endregion
	#region WebTimeScaleHeaderLevelCollection
	public class WebTimeScaleHeaderLevelCollection : List<WebTimeScaleHeaderLevel> {
	}
	#endregion
	#region WebTimeScaleHeaderCollection
	public class WebTimeScaleHeaderCollection : List<WebTimeScaleHeader> {
	}
	#endregion
	#region WebTimeScaleHeaderLevel
	public class WebTimeScaleHeaderLevel : HorizontalMergingContainer {
		TimeScaleLevel level;
		WebTimeScaleHeaderCollection headers;
		public WebTimeScaleHeaderLevel(TimeScaleLevel level) {
			if (level == null)
				Exceptions.ThrowArgumentNullException("level");
			this.level = level;
			this.headers = CreateHeaders(Level);
			CreateContent(WebObjects, Headers);
		}
		public TimeScaleLevel Level { get { return level; } }
		public WebTimeScaleHeaderCollection Headers { get { return headers; } }
		public void CalculateRelativeOffset(WebTimeScaleHeaderLevel baseLevel) {
			WebTimeScaleHeaderCollection baseHeadersCollection = baseLevel.Headers;
			int startIndex = 0;
			int headerCount = Headers.Count;
			for (int headerIndex = 0; headerIndex < headerCount; headerIndex++) {
				WebTimeScaleHeader header = Headers[headerIndex];
				startIndex = FindHeaderIndexByDate(baseHeadersCollection, header.Interval.Start, startIndex);
				WebTimeScaleHeader baseHeader = baseHeadersCollection[startIndex];
				header.Offset = CreateHeaderCellOffset(header, baseHeader);
			}
		}
		WebSingleTimelineHeaderCellOffset CreateHeaderCellOffset(WebTimeScaleHeader header, WebTimeScaleHeader baseHeader) {
			TimeInterval headerInterval = header.Interval;
			TimeInterval baseHeaderInterval = baseHeader.Interval;
			int relativeOffset = (int)(100 * (headerInterval.Start.Ticks - baseHeaderInterval.Start.Ticks) / (baseHeaderInterval.Duration.Ticks));
			if (relativeOffset < 0)
				relativeOffset = 0;
			WebSingleTimelineHeaderCellOffset offset = new WebSingleTimelineHeaderCellOffset(baseHeader, relativeOffset);
			return offset;
		}
		protected int FindHeaderIndexByDate(WebTimeScaleHeaderCollection headers, DateTime date, int startIndex) {
			for (int i = startIndex; i < headers.Count; i++) {
				TimeInterval interval = headers[i].Interval;
				if (interval.Contains(date) && (interval.End != date)) 
					return i;
			}
			return 0;
		}
		protected internal virtual WebTimeScaleHeaderCollection CreateHeaders(TimeScaleLevel level) {
			WebTimeScaleHeaderCollection headerCollection = new WebTimeScaleHeaderCollection();
			TimeIntervalCollection intervals = level.Intervals;
			int count = intervals.Count;
			for (int i = 0; i < count; i++) {
				TimeInterval interval = intervals[i];
				WebTimeScaleHeader header = new WebTimeScaleHeader(interval, level.Scale);
				headerCollection.Add(header);
			}
			return headerCollection;
		}
		protected internal virtual void CreateContent(SchedulerWebViewInfoCollection webObjects, WebTimeScaleHeaderCollection headers) {
			int count = headers.Count;
			for (int i = 0; i < count; i++)
				webObjects.Add(headers[i]);
		}
	}
	#endregion
	#region WebTimeScaleHeader
	public class WebTimeScaleHeader : InternalSchedulerCell {
		#region Fields
		TimeInterval interval;
		TimeScale scale;
		WebSingleTimelineHeaderCellOffset offset;
		bool isAlternate;
		#endregion
		public WebTimeScaleHeader(TimeInterval interval, TimeScale scale) {
			if (interval == null)
				Exceptions.ThrowArgumentNullException("interval");
			if (scale == null)
				Exceptions.ThrowArgumentNullException("scale");
			this.interval = interval;
			this.scale = scale;
			this.offset = new WebSingleTimelineHeaderCellOffset();
			this.isAlternate = IsAlternateHeader();
		}
		#region Properties
		public TimeInterval Interval { get { return interval; } }
		public TimeScale Scale { get { return scale; } }
		public WebSingleTimelineHeaderCellOffset Offset {
			get { return offset; }
			set {
				if (value == null)
					Exceptions.ThrowArgumentNullException("Offset");
				offset = value;
			}
		}
		public InternalSchedulerCell Cell { get { return this; } }
		public override WebElementType CellType { get { return WebElementType.TimelineDateHeader; } }
		public override bool IsAlternate { get { return isAlternate; } }
		#endregion
		#region IsAlternateHeader
		protected internal virtual bool IsAlternateHeader() {
			if (ASPxScheduler.ActiveControl == null)
				return false;
			DateTime scaleStart = Scale.Floor(Interval.Start);
			TimeInterval interval = new TimeInterval(scaleStart, scale.GetNextDate(scaleStart));
			DateTime clientNow = DateTime.Now;
			if (ASPxScheduler.ActiveControl != null) 
				clientNow = ASPxScheduler.ActiveControl.InnerControl.TimeZoneHelper.ToClientTime(DateTime.Now);
			return interval.IntersectsWithExcludingBounds(new TimeInterval(clientNow, TimeSpan.Zero));
		}
		#endregion
		#region IWebViewInfo Members
		public override SchedulerTable CreateTable() {
			return TableHelper.CreateTableWithOneCell(Cell);
		}
		#endregion
		#region SetDefaultContent
		protected internal override void SetDefaultContent() {
			this.Text = CalculateHeaderCaption();
			this.ToolTip = CalculateHeaderToolTip();
		}
		protected internal virtual string CalculateHeaderCaption() {
			IHeaderCaptionService captionFormatProvider = (IHeaderCaptionService)ASPxScheduler.ActiveControl.GetService(typeof(IHeaderCaptionService));
			if (captionFormatProvider != null) {
				string format = captionFormatProvider.GetTimeScaleHeaderCaption(this);
				if (!String.IsNullOrEmpty(format))
					return String.Format(CultureInfo.CurrentCulture, format, interval.Start, interval.End);
			}
			return Scale.FormatCaption(interval.Start, interval.End);
		}
		protected internal virtual string CalculateHeaderToolTip() {
			IHeaderToolTipService toolTipFormatProvider = (IHeaderToolTipService)ASPxScheduler.ActiveControl.GetService(typeof(IHeaderToolTipService));
			if (toolTipFormatProvider != null) {
				string format = toolTipFormatProvider.GetTimeScaleHeaderToolTip(this);
				if (!String.IsNullOrEmpty(format))
					return String.Format(CultureInfo.CurrentCulture, format, interval.Start, interval.End);
			}
			return Scale.FormatCaption(interval.Start, interval.End);
		}
		#endregion
	}
	#endregion
	#region WebSingleTimelineHeaderCellOffset
	public class WebSingleTimelineHeaderCellOffset {
		InternalSchedulerCell linkCell;
		int relativeOffset;
		public WebSingleTimelineHeaderCellOffset(InternalSchedulerCell linkCell, int relativeOffset) {
			if (linkCell == null)
				Exceptions.ThrowArgumentNullException("linkCell");
			this.linkCell = linkCell;
			this.relativeOffset = relativeOffset;
		}
		public WebSingleTimelineHeaderCellOffset() {
			this.linkCell = null;
			this.relativeOffset = 0;
		}
		public InternalSchedulerCell LinkCell {
			get {
				WebTimeScaleHeader headerCell = linkCell as WebTimeScaleHeader;
				if (headerCell != null && headerCell.Offset.LinkCell != null)
					return headerCell.Offset.LinkCell;
				return linkCell;
			}
			set {
				if (value == null)
					Exceptions.ThrowArgumentNullException("LinkCell");
				linkCell = value;
			}
		}
		public int RelativeOffset { get { return relativeOffset; } }
	}
	#endregion
	#region WebSelectionBarCell
	public class WebSelectionBarCell : WebSingleTimelineCell {
		public WebSelectionBarCell(TimeInterval interval)
			: base(interval, ResourceBase.Empty) {
		}
		public override WebElementType CellType { get { return WebElementType.SelectionBar; } }
	}
	#endregion
	#region WebSingleTimelineCell
	public class WebSingleTimelineCell : WebTimeCell {
		public WebSingleTimelineCell(TimeInterval interval, XtraScheduler.Resource resource)
			: base(interval, resource) {
		}
		public override WebElementType CellType { get { return WebElementType.TimelineCellBody; } }
	}
	#endregion
	#region WebTimelineCellCollection
	public class WebTimelineCellCollection : SchedulerViewCellBaseCollectionCore<WebSingleTimelineCell>, IWebTimeIntervalCollection {
		#region ITimeIntervalCollection Members
		public override ITimeIntervalCollection CreateEmptyCollection() {
			return new WebTimelineCellCollection();
		}
		#endregion
		#region IWebTimeIntervalCollection Members
		public string GetIdByIndex(int index) {
			if (index >= Count)
				Exceptions.ThrowArgumentException("index", index);
			return this[index].Cell.ID;
		}
		IWebTimeCell IWebTimeIntervalCollection.this[int index] { get { return this[index]; } }
		#endregion
	}
	#endregion
	public abstract class WebTimelineCellsBase : HorizontalMergingContainer, IWebCellContainer {
		#region Fields
		readonly TimeIntervalCollection intervals;
		readonly WebTimelineCellCollection cells;
		readonly XtraScheduler.Resource resource;
		#endregion
		protected WebTimelineCellsBase(TimeIntervalCollection intervals, XtraScheduler.Resource resource) {
			if (intervals == null)
				Exceptions.ThrowArgumentNullException("intervals");
			if (intervals.Count == 0)
				Exceptions.ThrowArgumentException("intervals.Count", intervals.Count);
			if (resource == null)
				Exceptions.ThrowArgumentNullException("resource");
			this.intervals = intervals;
			this.resource = resource;
			this.cells = new WebTimelineCellCollection();
		}
		#region Properties
		public TimeIntervalCollection Intervals { get { return intervals; } }
		public WebTimelineCellCollection Cells { get { return cells; } }
		public Resource Resource { get { return resource; } }
		#region IWebCellContainer Members
		int IWebCellContainer.CellCount { get { return Cells.Count; } }
		CellContainerType IWebCellContainer.ContainerType { get { return CellContainerType.Horizontal; } }
		IWebTimeCell IWebCellContainer.this[int index] { get { return Cells[index]; } }
		#endregion
		#endregion
		public virtual void Initialize() {
			FillCells(Cells, Intervals, Resource);
			CreateContent(cells);
			CalculateInvisibleBorders(cells);
		}
		protected internal virtual void FillCells(WebTimelineCellCollection cells, TimeIntervalCollection intervals, XtraScheduler.Resource resource) {
			int count = intervals.Count;
			for (int i = 0; i < count; i++) {
				WebSingleTimelineCell cell = CreateTimeCell(intervals[i], resource);
				InitializeCell(cell);
				cells.Add(cell);
			}
		}
		protected virtual WebSingleTimelineCell CreateTimeCell(TimeInterval interval, XtraScheduler.Resource resource) {
			WebSingleTimelineCell cell = new WebSingleTimelineCell(interval, resource);
			return cell;
		}
		protected internal virtual void CreateContent(WebTimelineCellCollection cells) {
			int count = cells.Count;
			for (int i = 0; i < count; i++)
				WebObjects.Add(cells[i]);
		}
		protected internal  void CalculateInvisibleBorders(WebTimelineCellCollection cells) {
			if (cells.Count == 0)
				return;
			int count = cells.Count;
			cells[count - 1].IgnoreBorderSide = IgnoreBorderSide.Right;
		}
		protected internal abstract void InitializeCell(WebSingleTimelineCell cell);
	}
	public class WebTimelineCells : WebTimelineCellsBase {
		TimelineWorkTimeCalculator workTimeCalculator;
		public WebTimelineCells(TimeIntervalCollection intervals, XtraScheduler.Resource resource, WorkDaysCollection workDays, InnerTimelineView innerView)
			: base(intervals, resource) {
			if (workDays == null)
				Exceptions.ThrowArgumentNullException("workDays");
			if (innerView == null)
				Exceptions.ThrowArgumentNullException("innerView");
			this.workTimeCalculator = new TimelineWorkTimeCalculator(innerView, workDays);
		}
		public TimelineWorkTimeCalculator WorkTimeCalculator { get { return workTimeCalculator; } }
		protected internal override void InitializeCell(WebSingleTimelineCell cell) {
			cell.IsWorkTime = workTimeCalculator.CalcIsWorkTime(cell);
		}
	}
	#region WebSelectionBar
	public class WebSelectionBar : WebTimelineCellsBase {
		public WebSelectionBar(TimeIntervalCollection intervals)
			: base(intervals, ResourceBase.Empty) {
		}
		protected override WebSingleTimelineCell CreateTimeCell(TimeInterval interval, XtraScheduler.Resource resource) {
			return new WebSelectionBarCell(interval);
		}
		protected internal override void InitializeCell(WebSingleTimelineCell cell) {
		}
	}
	#endregion
	#region WebTimelineViewInfoBase
	public abstract class WebTimelineViewInfoBase : WebViewInfoBase<WebTimelineCells> {
		#region Fields
		WebTimelineHeader header;
		WebSelectionBar selectionBar;
		#endregion
		protected WebTimelineViewInfoBase(TimelineView view)
			: base(view) {
		}
		#region Properties
		public new TimelineView View { get { return (TimelineView)base.View; } }
		protected internal int IntervalCount { get { return View.IntervalCount; } }
		public WebTimelineHeader Header { get { return header; } }
		public WebSelectionBar SelectionBar { get { return selectionBar; } }
		#endregion
		#region CreateAdditionalViewElements
		protected internal override IWebViewInfo CreateAdditionalViewElements() {
			VerticalMergingContainer container = new VerticalMergingContainer();
			this.header = new WebTimelineHeader(View.InnerView.ActualScales, View.InnerVisibleIntervals.Start, IntervalCount);
			container.WebObjects.Add(header);
			if (View.ShowSelectionBar) {
				this.selectionBar = new WebSelectionBar(Header.Intervals);
				SelectionBar.Initialize();
				container.WebObjects.Add(selectionBar);
			}
			return container;
		}
		#endregion
		#region Create
		public override void Create() {
			base.Create();
			Header.BindToCells(this.ResourcesCellContainers[0][0].Cells);
		}
		#endregion
		#region CreateResourceHeaderAndCellContainersMergingContainer
		protected internal override MergingContainerBase CreateResourceHeaderAndCellContainersMergingContainer() {
			return new HorizontalMergingContainer();
		}
		#endregion
		#region CreateCellContainersMergingContainer
		protected internal override MergingContainerBase CreateCellContainersMergingContainer() {
			return new HorizontalMergingContainer();
		}
		#endregion
		#region CreateMergingContainer
		protected internal override MergingContainerBase CreateMergingContainer() {
			return new VerticalMergingContainer();
		}
		#endregion
		#region CreateResourceCellContainers
		protected internal override List<WebTimelineCells> CreateResourceCellContainers(XtraScheduler.Resource resource) {
			List<WebTimelineCells> result = new List<WebTimelineCells>();
			result.Add(CreateCellContainer(View.InnerVisibleIntervals[0], resource));
			return result;
		}
		#endregion
		#region CreateCellContainer
		protected internal override WebTimelineCells CreateCellContainer(TimeInterval interval, XtraScheduler.Resource resource) {
			WebTimelineCells cells = new WebTimelineCells(Header.Intervals, resource, View.Control.WorkDays, View.InnerView);
			cells.Initialize();
			return cells;
		}
		#endregion
	}
	#endregion
	#region WebTimelineViewGroupByNone
	public class WebTimelineViewGroupByNone : WebTimelineViewInfoBase {
		public WebTimelineViewGroupByNone(TimelineView view) : base(view) {
		}
		public override ResourceBaseCollection Resources { get { return WebWeekViewInfoBase.EmptyResourceCollection; } }
		protected internal override IWebViewInfo CreateResourceHeader(XtraScheduler.Resource resource) {
			return new EmptyWebViewInfo();
		}
	}
	#endregion
	public interface ISchedulerCellProvider {
		InternalSchedulerCell GetCell(WebElementType elementType, int index);
	}
	#region WebTimelineViewGroupByDate
	public class WebTimelineViewGroupByDate : WebTimelineViewInfoBase, ISchedulerCellProvider {
		List<WebVerticalResourceHeader> resourceHeadersCollection;
		public WebTimelineViewGroupByDate(TimelineView view) : base(view) {
			this.resourceHeadersCollection = new List<WebVerticalResourceHeader>();
		}
		public List<WebVerticalResourceHeader> ResourceHeadersCollection { get { return resourceHeadersCollection; } }
		protected internal override IWebViewInfo CreateAdditionalViewElements() {
			IWebViewInfo headersWithSelectionBar = base.CreateAdditionalViewElements();
			int headerCount = CalcVisibleScaleCount();
			WebLeftTopCorner corner = new WebLeftTopCorner(String.Empty, headerCount);
			corner.IgnoreBorderSide = IgnoreBorderSide.Bottom;
			HorizontalMergingContainer result = new HorizontalMergingContainer();
			if(headerCount != 0)
				result.WebObjects.Add(corner);
			result.WebObjects.Add(headersWithSelectionBar);
			return result;
		}
		protected internal virtual int CalcVisibleScaleCount() {
			int scalesCount = 0;
			foreach(TimeScale scale in View.InnerView.ActualScales)
				if(scale.Visible)
					scalesCount++;
			if(View.ShowSelectionBar)
				scalesCount++;
			return scalesCount;
		}
		protected internal override IWebViewInfo CreateResourceHeader(XtraScheduler.Resource resource) {
			WebVerticalResourceHeader header = new WebVerticalResourceHeader(VisibleIntervals, resource);
			ResourceHeadersCollection.Add(header);
			return header;
		}
		#region ISchedulerCellProvider Members
		InternalSchedulerCell ISchedulerCellProvider.GetCell(WebElementType elementType, int index) {
			if(elementType == WebElementType.VerticalResourceHeader) {
				if(index < ResourceHeadersCollection.Count) 
					return resourceHeadersCollection[index];
			}
			return null;
		}
		#endregion
	}
	#endregion
	#region WebTimelineViewGroupByResource
	public class WebTimelineViewGroupByResource : WebTimelineViewGroupByDate {
		public WebTimelineViewGroupByResource(TimelineView view) : base(view) {
		}
	}
	#endregion
}
