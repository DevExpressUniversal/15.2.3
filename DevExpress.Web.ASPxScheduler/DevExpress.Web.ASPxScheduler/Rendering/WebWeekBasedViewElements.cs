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
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using System.Web.UI;
using DevExpress.Web.Internal;
using DevExpress.XtraScheduler.Services.Internal;
using System.Globalization;
using DevExpress.Web.ASPxScheduler.Services;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.Web.ASPxScheduler.Rendering {
	public enum AnchorType {
		Left = 0,
		Right = 1
	}
	#region WebWeekBase (abstract class)
	public abstract class WebWeekBase : IWebViewInfo, IWebCellContainer, ISupportAnchors {
		#region Fields
		TimeInterval interval;
		XtraScheduler.Resource resource;
		WebDateCellCollection cells;
		MergingContainerBase mergingContainer;
		WeekInvisibleCellBorderCalculator weekInvisibleCellBorderCalculator;
		#endregion
		protected WebWeekBase(TimeInterval interval, XtraScheduler.Resource resource, DateTime[] dates, bool compressWeekend) {
			if(interval == null)
				Exceptions.ThrowArgumentNullException("interval");
			if(resource == null)
				Exceptions.ThrowArgumentNullException("resource");
			if(dates == null)
				Exceptions.ThrowArgumentNullException("dates");
			this.interval = interval;
			this.resource = resource;
			this.cells = CreateCells(dates);
			SchedulerWebViewInfoCollection compressedCells = PrepareCells(compressWeekend);
			this.mergingContainer = LayoutCells(compressedCells);
			this.weekInvisibleCellBorderCalculator = CreateWeekInvisibleCellBorderCalculator();
			WeekInvisibleCellBorderCalculator.Calculate(compressedCells);
		}
		#region Properties
		public MergingContainerBase MergingContainer { get { return mergingContainer; } }
		public WebDateCellCollection Cells { get { return cells; } }
		public Resource Resource { get { return resource; } }
		public TimeInterval Interval { get { return interval; } }
		internal virtual WeekInvisibleCellBorderCalculator WeekInvisibleCellBorderCalculator { get { return weekInvisibleCellBorderCalculator; } }
		public bool FirstVisible { get { return Cells[0].Header.FirstVisible; } set { Cells[0].Header.FirstVisible = value; } }
		#endregion
		protected internal virtual WebDateCellCollection CreateCells(DateTime[] dates) {
			int count = dates.Length;
			WebDateCellCollection result = new WebDateCellCollection();
			for(int i = 0; i < count; i++) {
				TimeInterval interval = new TimeInterval(dates[i], DateTimeHelper.DaySpan);
				result.Add(CreateDateCell(interval, resource));
			}
			return result;
		}
		protected internal abstract WebDateCell CreateDateCell(TimeInterval interval, XtraScheduler.Resource resource);
		protected internal virtual SchedulerWebViewInfoCollection PrepareCells(bool compressWeekend) {
			if(compressWeekend)
				return CompressCells(Cells);
			else {
				SchedulerWebViewInfoCollection result = new SchedulerWebViewInfoCollection();
				int count = Cells.Count;
				for(int i = 0; i < count; i++)
					result.Add(Cells[i]);
				return result;
			}
		}
		protected internal virtual SchedulerWebViewInfoCollection CompressCells(WebDateCellCollection cells) {
			SchedulerWebViewInfoCollection result = new SchedulerWebViewInfoCollection();
			int count = cells.Count;
			for(int i = 0; i < count; i++) {
				DayOfWeek dayOfWeek = cells[i].Interval.Start.DayOfWeek;
				if(dayOfWeek == DayOfWeek.Sunday)
					continue;
				if(dayOfWeek == DayOfWeek.Saturday) {
					WebDateCell saturdayCell = cells[i];
					WebDateCell sundayCell = cells[i + 1];
					saturdayCell.IsCompressedCell = true;
					sundayCell.IsCompressedCell = true;
					VerticalMergingContainer weekends = new VerticalMergingContainer();
					weekends.WebObjects.Add(saturdayCell);
					weekends.WebObjects.Add(sundayCell);
					result.Add(weekends);
				}
				else
					result.Add(cells[i]);
			}
			return result;
		}
		protected internal abstract MergingContainerBase LayoutCells(SchedulerWebViewInfoCollection cells);
		protected internal abstract WeekInvisibleCellBorderCalculator CreateWeekInvisibleCellBorderCalculator();
		#region IWebViewInfo Members
		public virtual SchedulerTable CreateTable() {
			return MergingContainer.CreateTable();
		}
		#endregion
		#region IWebCellContainer Members
		public int CellCount { get { return Cells.Count; } }
		public CellContainerType ContainerType { get { return CellContainerType.Horizontal; } }
		public IWebTimeCell this[int index] { get { return Cells[index]; } }
		#endregion
		#region ISupportAnchors Members
		public virtual void CreateLeftTopAnchor(AnchorCollection anchors) {
			MergingContainer.CreateLeftTopAnchor(anchors);
		}
		public virtual void CreateRightBottomAnchor(AnchorCollection anchors) {
			MergingContainer.CreateRightBottomAnchor(anchors);
		}
		#endregion
	}
	#endregion
	#region WebHorizontalWeek
	public class WebHorizontalWeek : WebWeekBase {
		public WebHorizontalWeek(TimeInterval interval, XtraScheduler.Resource resource, DateTime[] dates, bool compressWeekend)
			: base(interval, resource, dates, compressWeekend) {
		}
		protected internal override MergingContainerBase LayoutCells(SchedulerWebViewInfoCollection cells) {
			MergingContainerBase result = new HorizontalMergingContainer();
			int count = cells.Count;
			for(int i = 0; i < count; i++)
				result.WebObjects.Add(cells[i]);
			return result;
		}
		protected internal override WeekInvisibleCellBorderCalculator CreateWeekInvisibleCellBorderCalculator() {
			return new HorizontalWeekInvisibleCellBorderCalculator();
		}
		protected internal override WebDateCell CreateDateCell(TimeInterval interval, XtraScheduler.Resource resource) {
			return new WebDateCell(interval, resource, false);
		}
	}
	#endregion
	#region WebVerticalWeek
	public class WebVerticalWeek : WebWeekBase {
		public WebVerticalWeek(TimeInterval interval, XtraScheduler.Resource resource, DateTime[] dates)
			: base(interval, resource, dates, true) {
		}
		protected internal override MergingContainerBase LayoutCells(SchedulerWebViewInfoCollection cells) {
			MergingContainerBase result = new VerticalMergingContainer();
			int count = cells.Count;
			XtraSchedulerDebug.Assert(count == 6);
			int halfCount = count / 2;
			for(int i = 0; i < halfCount; i++) {
				HorizontalMergingContainer container = new HorizontalMergingContainer();
				container.WebObjects.Add(cells[i]);
				container.WebObjects.Add(cells[i + halfCount]);
				result.WebObjects.Add(container);
			}
			return result;
		}
		protected internal override WeekInvisibleCellBorderCalculator CreateWeekInvisibleCellBorderCalculator() {
			return new VerticalWeekInvisibleCellBorderCalculator();
		}
		protected internal override WebDateCell CreateDateCell(TimeInterval interval, XtraScheduler.Resource resource) {
			return new WebDateCell(interval, resource, true);
		}
	}
	#endregion
	#region WebDateCellHeader
	public class WebDateCellHeader : WebDateHeader {
		bool vertical;
		public WebDateCellHeader(TimeInterval interval, XtraScheduler.Resource resource, bool vertical)
			: base(interval, resource) {
			this.vertical = vertical;
		}
		public WebDateCellHeader(TimeInterval interval, ResourceBaseCollection resources, bool vertical)
			: base(interval, resources) {
			this.vertical = vertical;
		}
		#region Properties
		public override WebElementType CellType { get { return WebElementType.DateCellHeader; } }
		public bool Vertical { get { return vertical; } }
		#endregion
		#region CalculateHeaderCaption
		protected internal override string CalculateHeaderCaption() {
			IHeaderCaptionService formatProvider = (IHeaderCaptionService)ASPxScheduler.ActiveControl.GetService(typeof(IHeaderCaptionService));
			if(formatProvider != null) {
				string format = Vertical ? formatProvider.GetVerticalWeekCellHeaderCaption(this) : formatProvider.GetHorizontalWeekCellHeaderCaption(this);
				formatProvider.GetDayColumnHeaderCaption(this);
				if(!String.IsNullOrEmpty(format))
					return String.Format(CultureInfo.CurrentCulture, format, Interval.Start);
			}
			if(DesignMode) {
				if(vertical)
					return DesignTimeDateHeaderFormatter.FormatHeaderDate(this);
				return DesignTimeDateHeaderFormatter.FormatMonthHeaderDate(this);
			}
			return String.Empty;
		}
		#endregion
		#region CalculateHeaderToolTip
		protected internal override string CalculateHeaderToolTip() {
			IHeaderToolTipService formatProvider = (IHeaderToolTipService)ASPxScheduler.ActiveControl.GetService(typeof(IHeaderToolTipService));
			if(formatProvider != null) {
				string format = Vertical ? formatProvider.GetVerticalWeekCellHeaderToolTip(this) : formatProvider.GetHorizontalWeekCellHeaderToolTip(this);
				if(!String.IsNullOrEmpty(format))
					return String.Format(CultureInfo.CurrentCulture, format, Interval.Start);
			}
			return String.Empty;
		}
		#endregion
	}
	#endregion
	#region WebDateCell
	public class WebDateCell : VerticalMergingContainer, IWebTimeCell {
		#region Fields
		WebDateCellHeader header;
		WebDateContent content;
		TimeInterval interval;
		XtraScheduler.Resource resource;
		bool isCompressedCell;
		#endregion
		public WebDateCell(TimeInterval interval, XtraScheduler.Resource resource, bool vertical) {
			if(interval == null)
				Exceptions.ThrowArgumentNullException("interval");
			if(resource == null)
				Exceptions.ThrowArgumentNullException("resource");
			this.interval = interval;
			this.resource = resource;
			this.header = new WebDateCellHeader(Interval, Resource, vertical);
			this.content = new WebDateContent(Interval, Resource);
			WebObjects.Add(Header);
			WebObjects.Add(Content);
		}
		#region Properties
		public WebDateHeader Header { get { return header; } }
		public WebDateContent Content { get { return content; } }
		public TimeInterval Interval { get { return interval; } }
		public Resource Resource { get { return resource; } }
		public string Id { get { return content.Id; } }
		public bool FirstVisible { get { return Header.FirstVisible; } set { Header.FirstVisible = value; } }
		public bool IsCompressedCell { get { return isCompressedCell; } set { isCompressedCell = value; } }
		#endregion
		#region IWebTimeCell Members
		public void AssignId(string id) {
			content.AssignId(id);
		}
		Control IWebTimeCell.Parent { get { return content.Parent; } }
		TableCell IWebTimeCell.ContentCell { get { return content; } }
		#endregion
	}
	#endregion
	#region WebDateContent
	public class WebDateContent : InternalSchedulerCell, ISupportAnchors, IWebTimeCell {
		#region Fields
		TimeInterval interval;
		XtraScheduler.Resource resource;
		#endregion
		public WebDateContent(TimeInterval interval, XtraScheduler.Resource resource) {
			this.interval = interval;
			this.resource = resource;
		}
		#region Properties
		public InternalSchedulerCell Cell { get { return this; } }
		public TimeInterval Interval { get { return interval; } }
		public Resource Resource { get { return resource; } }
		public override WebElementType CellType { get { return WebElementType.DateCellBody; } }
		public TableCell ContentCell { get { return this; } }
		internal string ToolTipFormatString { get { return "{0:D}"; } }
		#endregion
		public override SchedulerTable CreateTable() {
			return TableHelper.CreateTableWithOneCell(Cell);
		}
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
		public override HorizontalAlign HorizontalAlign {
			get { return ((CellBodyStyle)ControlStyle).HorizontalAlign; }
			set { ((CellBodyStyle)base.ControlStyle).HorizontalAlign = value; }
		}
		public override VerticalAlign VerticalAlign {
			get { return ((CellBodyStyle)ControlStyle).VerticalAlign; }
			set { ((CellBodyStyle)base.ControlStyle).VerticalAlign = value; }
		}
		protected override Style CreateControlStyle() {
			return new CellBodyStyle();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			ASPxScheduler.ActiveControl.RaiseHtmlTimeCellPrepared(this, this, ASPxScheduler.ActiveControl.ActiveView);
		}
		#region IWebTimeCell Members
		string IWebTimeCell.Id { get { return Id; } }
		#endregion
	}
	#endregion
	#region WebWeekViewInfoBase
	public abstract class WebWeekViewInfoBase : WebViewInfoBase<WebWeekBase> {
		#region Fields
		DayOfWeek[] weekDays;
		public new WeekView View { get { return (WeekView)base.View; } }
		#endregion
		protected WebWeekViewInfoBase(WeekView view)
			: base(view) {
		}
		#region Properties
		protected internal DayOfWeek[] WeekDays { get { return weekDays; } }
		#endregion
		protected internal override IWebViewInfo CreateAdditionalViewElements() {
			return CreateDayOfWeekHeaders();
		}
		protected internal override void Initialize() {
			base.Initialize();
			this.weekDays = CreateWeekDays();
		}
		protected internal virtual DayOfWeek[] CreateWeekDays() {
			DateTime[] dates = GetVisibleDates(VisibleIntervals[0]);
			int count = dates.Length;
			DayOfWeek[] result = new DayOfWeek[count];
			for(int i = 0; i < count; i++)
				result[i] = dates[i].DayOfWeek;
			return result;
		}
		protected internal virtual DateTime[] GetVisibleDates(TimeInterval interval) {
			List<DateTime> dates = new List<DateTime>();
			DateTime date = interval.Start;
			while(date < interval.End) {
				if(IsDayOfWeekVisible(date.DayOfWeek))
					dates.Add(date);
				date += DateTimeHelper.DaySpan;
			}
			return dates.ToArray();
		}
		protected internal virtual bool IsDayOfWeekVisible(DayOfWeek dayOfWeek) {
			if(View.InnerView.ShowWeekendInternal)
				return true;
			else
				return dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday;
		}
		protected internal override MergingContainerBase CreateCellContainersMergingContainer() {
			return new VerticalMergingContainer();
		}
		protected internal override WebWeekBase CreateCellContainer(TimeInterval interval, XtraScheduler.Resource resource) {
			DateTime[] dates = GetVisibleDates(interval);
			WebWeekBase week = CreateWeekCore(interval, resource, dates);
			if(this.VisibleIntervals.Interval.Start == interval.Start)
				week.FirstVisible = true;
			WebDateCellCollection weekCells = week.Cells;
			int count = weekCells.Count;
			for(int i = 0; i < count; i++) {
				WebDateHeader header = weekCells[i].Header;
				HeaderFormatSeparator.Add(header);
			}
			return week;
		}
		protected internal abstract WebWeekBase CreateWeekCore(TimeInterval interval, XtraScheduler.Resource resource, DateTime[] dates);
		protected internal abstract IWebViewInfo CreateDayOfWeekHeaders();
	}
	#endregion
	#region WebWeekViewGroupByNone
	public class WebWeekViewGroupByNone : WebWeekViewInfoBase {
		public WebWeekViewGroupByNone(WeekView view)
			: base(view) {
		}
		public override ResourceBaseCollection Resources { get { return WebWeekViewInfoBase.EmptyResourceCollection; } }
		protected internal override IWebViewInfo CreateResourceHeader(XtraScheduler.Resource resource) {
			return new EmptyWebViewInfo();
		}
		protected internal override MergingContainerBase CreateMergingContainer() {
			return new VerticalMergingContainer();
		}
		protected internal override MergingContainerBase CreateResourceHeaderAndCellContainersMergingContainer() {
			return new VerticalMergingContainer();
		}
		protected internal override WebWeekBase CreateWeekCore(TimeInterval interval, XtraScheduler.Resource resource, DateTime[] dates) {
			return new WebVerticalWeek(interval, resource, dates);
		}
		protected internal override IWebViewInfo CreateDayOfWeekHeaders() {
			return new EmptyWebViewInfo();
		}
		public override int GetResourceColorIndex(XtraScheduler.Resource resource) {
			return 0;
		}
		protected override HeaderFormatSeparatorBase CreateHeadersFormatSeparator() {
			return new WeekViewGroupByNoneHeaderFormatSeparator();
		}
	}
	#endregion
	#region WebWeekViewGroupByDate
	public class WebWeekViewGroupByDate : WebWeekViewInfoBase {
		public WebWeekViewGroupByDate(WeekView view)
			: base(view) {
		}
		protected internal override MergingContainerBase CreateMergingContainer() {
			return new VerticalMergingContainer();
		}
		protected internal override MergingContainerBase CreateResourceHeaderAndCellContainersMergingContainer() {
			return new HorizontalMergingContainer();
		}
		protected internal override WebWeekBase CreateWeekCore(TimeInterval interval, XtraScheduler.Resource resource, DateTime[] dates) {
			return new WebHorizontalWeek(interval, resource, dates, true);
		}
		protected internal override IWebViewInfo CreateDayOfWeekHeaders() {
			WebLeftTopCorner corner = new WebLeftTopCorner();
			corner.IgnoreBorderSide = IgnoreBorderSide.Bottom;
			WebDayHeaderContainer headers = new WebDayHeaderContainer(WeekDays, View, ResourceBase.Empty);
			HeaderFormatSeparator.Add(headers.DayHeaders);
			HorizontalMergingContainer result = new HorizontalMergingContainer();
			result.WebObjects.Add(corner);
			result.WebObjects.Add(headers);
			return result;
		}
		protected override HeaderFormatSeparatorBase CreateHeadersFormatSeparator() {
			return new WeekViewGroupByDateHeaderFormatSeparator();
		}
		protected internal override IWebViewInfo CreateResourceHeader(XtraScheduler.Resource resource) {
			return new WebVerticalResourceHeader(VisibleIntervals, resource);
		}
	}
	#endregion
	#region WebWeekViewGroupByResource
	public class WebWeekViewGroupByResource : WebWeekViewInfoBase {
		public WebWeekViewGroupByResource(WeekView view)
			: base(view) {
		}
		protected internal override MergingContainerBase CreateMergingContainer() {
			return new HorizontalMergingContainer();
		}
		protected internal override MergingContainerBase CreateResourceHeaderAndCellContainersMergingContainer() {
			return new VerticalMergingContainer();
		}
		protected internal override WebWeekBase CreateWeekCore(TimeInterval interval, XtraScheduler.Resource resource, DateTime[] dates) {
			return new WebVerticalWeek(interval, resource, dates);
		}
		protected internal override IWebViewInfo CreateDayOfWeekHeaders() {
			return new EmptyWebViewInfo();
		}
		protected internal override void CreateGroupSeparator(SchedulerWebViewInfoCollection webObjects, IWebViewInfo resourceView) {
			WebGroupSeparatorVertical separator = new WebGroupSeparatorVertical();
			webObjects.Add(separator);
		}
		protected internal override IWebViewInfo CreateResourceHeader(XtraScheduler.Resource resource) {
			IWebViewInfo result = base.CreateResourceHeader(resource);
			WebHorizontalResourceHeader resourceHeader = result as WebHorizontalResourceHeader;
			if (resourceHeader != null) {
				resourceHeader.IgnoreBorderSide = IgnoreBorderSide.Right;
				if(RenderUtils.Browser.IsIE)
					resourceHeader.IgnoreBorderSide |= IgnoreBorderSide.Bottom;
			}
			return result;
		}
		protected override HeaderFormatSeparatorBase CreateHeadersFormatSeparator() {
			return new WeekViewGroupByResourceHeaderFormatSeparator();
		}
	}
	#endregion
	#region WebMonthViewGroupByNone
	public class WebMonthViewGroupByNone : WebWeekViewGroupByNone {
		public WebMonthViewGroupByNone(MonthView view)
			: base(view) {
		}
		protected internal override IWebViewInfo CreateDayOfWeekHeaders() {
			WebDayHeaderContainer headers = new WebDayHeaderContainer(WeekDays, View, ResourceBase.Empty);
			HeaderFormatSeparator.Add(headers.DayHeaders);
			return headers;
		}
		protected internal override WebWeekBase CreateWeekCore(TimeInterval interval, XtraScheduler.Resource resource, DateTime[] dates) {
			return new WebHorizontalWeek(interval, resource, dates, ((MonthView)View).CompressWeekend);
		}
		protected override HeaderFormatSeparatorBase CreateHeadersFormatSeparator() {
			return new MonthViewHeaderFormatSeparatorBase();
		}
	}
	#endregion
	#region WebMonthViewGroupByDate
	public class WebMonthViewGroupByDate : WebWeekViewGroupByDate {
		public WebMonthViewGroupByDate(MonthView view)
			: base(view) {
		}
		protected internal override WebWeekBase CreateWeekCore(TimeInterval interval, XtraScheduler.Resource resource, DateTime[] dates) {
			return new WebHorizontalWeek(interval, resource, dates, ((MonthView)View).CompressWeekend);
		}
		protected override HeaderFormatSeparatorBase CreateHeadersFormatSeparator() {
			return new MonthViewHeaderFormatSeparatorBase();
		}
	}
	#endregion
	#region WebMonthViewGroupByResource
	public class WebMonthViewGroupByResource : WebWeekViewGroupByResource {
		public WebMonthViewGroupByResource(MonthView view)
			: base(view) {
		}
		protected internal override IWebViewInfo CreateDayOfWeekHeaders() {
			return new EmptyWebViewInfo();
		}
		protected internal override WebWeekBase CreateWeekCore(TimeInterval interval, XtraScheduler.Resource resource, DateTime[] dates) {
			return new WebHorizontalWeek(interval, resource, dates, ((MonthView)View).CompressWeekend);
		}
		protected internal override IWebViewInfo CreateResourceHeader(XtraScheduler.Resource resource) {
			IWebViewInfo resourceHeader = base.CreateResourceHeader(resource);
			WebDayHeaderContainer headers = new WebDayHeaderContainer(WeekDays, View, resource);
			VerticalMergingContainer result = new VerticalMergingContainer();
			HeaderFormatSeparator.Add(headers.DayHeaders);
			result.WebObjects.Add(resourceHeader);
			result.WebObjects.Add(headers);
			return result;
		}
		public override void CreateLeftTopAnchor(AnchorCollection anchors) {
			int count = ResourcesCellContainers.Count;
			for(int i = 0; i < count; i++)
				ResourcesCellContainers[i][0].CreateLeftTopAnchor(anchors);
		}
		public override void CreateRightBottomAnchor(AnchorCollection anchors) {
			int count = ResourcesCellContainers.Count;
			for(int i = 0; i < count; i++) {
				int containerCount = ResourcesCellContainers[i].Count;
				ResourcesCellContainers[i][containerCount - 1].CreateRightBottomAnchor(anchors);
			}
		}
		protected override HeaderFormatSeparatorBase CreateHeadersFormatSeparator() {
			return new MonthViewHeaderFormatSeparatorBase();
		}
	}
	#endregion
	#region WeekInvisibleCellBorderCalculator
	public abstract class WeekInvisibleCellBorderCalculator {
		public abstract void Calculate(SchedulerWebViewInfoCollection cells);
	}
	#endregion
	#region HorizontalWeekInvisibleCellBorderCalculator
	public class HorizontalWeekInvisibleCellBorderCalculator : WeekInvisibleCellBorderCalculator {
		protected internal enum RelativePosition { First, Middle, Last }
		public override void Calculate(SchedulerWebViewInfoCollection cells) {
			int count = cells.Count;
			for(int i = 0; i < count; i++) {
				RelativePosition position = CalculateCellRelativePosition(count, i);
				IWebViewInfo cell = cells[i];
				HideCellBorders(cell, position);
			}
		}
		protected internal virtual void HideCellBorders(IWebViewInfo cell, RelativePosition position) {
			WebDateCell dateCell = cell as WebDateCell;
			if(dateCell != null)
				HideDateCellBorders(position, dateCell);
			else
				HideWeekendCellBorders(cell, position);
		}
		protected internal virtual void HideWeekendCellBorders(IWebViewInfo cell, RelativePosition position) {
			if(position == RelativePosition.First)
				HideWeekendBordersCore(cell, IgnoreBorderSide.Left, IgnoreBorderSide.Left);
			else if(position == RelativePosition.Middle)
				HideWeekendBordersCore(cell, IgnoreBorderSide.Left, IgnoreBorderSide.None);
			else if(position == RelativePosition.Last)
				HideWeekendBordersCore(cell, IgnoreBorderSide.Left | IgnoreBorderSide.Right, IgnoreBorderSide.Right);
		}
		protected internal virtual void HideDateCellBorders(RelativePosition position, WebDateCell dateCell) {
			WebDateHeader header = dateCell.Header;
			WebDateContent content = dateCell.Content;
			if(position != RelativePosition.Last)
				header.IgnoreBorderSide = IgnoreBorderSide.Left;
			else
				header.IgnoreBorderSide = IgnoreBorderSide.Left | IgnoreBorderSide.Right;
			content.IgnoreBorderSide = header.IgnoreBorderSide;
		}
		protected internal virtual RelativePosition CalculateCellRelativePosition(int count, int index) {
			RelativePosition position = (index == 0) ? RelativePosition.First : ((index == count - 1) ? RelativePosition.Last : RelativePosition.Middle);
			return position;
		}
		protected virtual void HideWeekendBordersCore(IWebViewInfo item, IgnoreBorderSide saturdayIgnoreBorders, IgnoreBorderSide sundayIgnoreBorders) {
			VerticalMergingContainer weekend = item as VerticalMergingContainer;
			if(weekend == null)
				return;
			if(weekend.WebObjects.Count != 2)
				return;
			WebDateCell saturday = weekend.WebObjects[0] as WebDateCell;
			WebDateCell sunday = weekend.WebObjects[1] as WebDateCell;
			saturday.Header.IgnoreBorderSide |= saturdayIgnoreBorders;
			saturday.Content.IgnoreBorderSide |= saturdayIgnoreBorders;
			sunday.Header.IgnoreBorderSide |= sundayIgnoreBorders;
			sunday.Content.IgnoreBorderSide |= sundayIgnoreBorders;
		}
	}
	#endregion
	#region VerticalWeekInvisibleCellBorderCalculator
	public class VerticalWeekInvisibleCellBorderCalculator : WeekInvisibleCellBorderCalculator {
		public override void Calculate(SchedulerWebViewInfoCollection cells) {
			int count = cells.Count;
			if(RenderUtils.Browser.IsFirefox)
				HideTopHeadersTopBorders(cells, count);
			HideBordersForLeftCells(cells, count);
			HideBordersForRightCells(cells, count);
		}
		protected internal virtual void HideBordersForRightCells(SchedulerWebViewInfoCollection cells, int count) {
			for(int i = count / 2; i < count; i++) {
				WebDateCell cell = cells[i] as WebDateCell;
				if(cell != null) {
					cell.Header.IgnoreBorderSide |= IgnoreBorderSide.Right | IgnoreBorderSide.Left;
					cell.Content.IgnoreBorderSide |= cell.Header.IgnoreBorderSide;
				}
				else
					HideWeekendBordersCore(cells[i], IgnoreBorderSide.Left | IgnoreBorderSide.Right, IgnoreBorderSide.Right);
			}
		}
		protected internal virtual void HideBordersForLeftCells(SchedulerWebViewInfoCollection cells, int count) {
			for(int i = 0; i < count / 2; i++) {
				WebDateCell cell = cells[i] as WebDateCell;
				if(cell != null) {
					cell.Header.IgnoreBorderSide |= IgnoreBorderSide.Left;
					cell.Content.IgnoreBorderSide |= cell.Header.IgnoreBorderSide;
				}
				else
					HideWeekendBordersCore(cells[i], IgnoreBorderSide.Left, IgnoreBorderSide.Left);
			}
		}
		protected internal virtual void HideTopHeadersTopBorders(SchedulerWebViewInfoCollection cells, int count) {
			for(int i = 0; i < count; i += 3) {
				WebDateCell cell = cells[i] as WebDateCell;
				if(cell != null)
					cell.Header.IgnoreBorderSide = IgnoreBorderSide.Top;
				else
					HideWeekendBordersCore(cells[i], IgnoreBorderSide.Top, IgnoreBorderSide.None);
			}
		}
		protected virtual void HideWeekendBordersCore(IWebViewInfo item, IgnoreBorderSide saturdayIgnoreBorders, IgnoreBorderSide sundayIgnoreBorders) {
			VerticalMergingContainer weekend = item as VerticalMergingContainer;
			if(weekend == null)
				return;
			if(weekend.WebObjects.Count != 2)
				return;
			WebDateCell saturday = weekend.WebObjects[0] as WebDateCell;
			WebDateCell sunday = weekend.WebObjects[1] as WebDateCell;
			saturday.Header.IgnoreBorderSide |= saturdayIgnoreBorders;
			saturday.Content.IgnoreBorderSide |= saturdayIgnoreBorders;
			sunday.Header.IgnoreBorderSide |= sundayIgnoreBorders;
			sunday.Content.IgnoreBorderSide |= sundayIgnoreBorders;
		}
	}
	#endregion
}
