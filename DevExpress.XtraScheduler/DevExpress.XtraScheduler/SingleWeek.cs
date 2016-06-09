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
using System.Globalization;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler.Drawing {
	#region SingleWeekCellBase
	public abstract class SingleWeekCellBase : SchedulerViewCellBase {
		#region Fields
		SchedulerHeader header;
		Rectangle contentBounds;
		BaseHeaderAppearanceContainer headerAppearance;
		SchedulerHeaderPreliminaryLayoutResult headerPreliminaryResult;
		#endregion
		protected SingleWeekCellBase(BaseHeaderAppearanceContainer headerAppearance) {
			this.headerAppearance = headerAppearance;
			this.Interval = TimeInterval.Day;
			this.header = CreateHeader();
		}
		#region Properties
		public DateTime Date { get { return Interval.Start; } set { Interval.Start = value; } }
		public SchedulerHeader Header { get { return header; } }
		public Rectangle HeaderBounds { get { return Header != null ? Header.Bounds : Rectangle.Empty; } }
		public override Rectangle ContentBounds { get { return contentBounds; } }
		public SchedulerHeaderPreliminaryLayoutResult HeaderPreliminaryResult {
			get { return this.headerPreliminaryResult; }
			set { this.headerPreliminaryResult = value; }
		}
		public override bool Selected {
			get { return base.Selected; }
			set {
				if (value == base.Selected)
					return;
				base.Selected = value;
				Header.Selected = value;
			}
		}
		public override TimeInterval Interval {
			get {
				return base.Interval;
			}
			set {
				base.Interval = value;
				if (Header != null)
					Header.Interval = Interval;
			}
		}
		public override Resource Resource {
			get { return base.Resource; }
			set {
				if (value == base.Resource)
					return;
				base.Resource = value;
				if (Header != null)
					Header.Resource = Resource;
			}
		}
		#endregion
		public void CalcLayout(bool hideHeaderCaption) {
			this.contentBounds = RectUtils.CutFromTop(Bounds, Header.Bounds.Height);
			if (HasLeftBorder)
				LeftBorderBounds = RectUtils.GetLeftSideRect(Bounds, 1);
			if (HasBottomBorder)
				BottomBorderBounds = RectUtils.GetBottomSideRect(Bounds, 1);
			if (HasTopBorder)
				TopBorderBounds = RectUtils.GetTopSideRect(Bounds, 1);
			if (HasRightBorder)
				RightBorderBounds = RectUtils.GetRightSideRect(Bounds, 1);
			if (hideHeaderCaption)
				Header.Caption = String.Empty;
		}
		protected internal abstract void SetHeaderCaption(HeaderCaptionFormatProviderBase provider, GraphicsCache cache);
		protected internal virtual SchedulerHeader CreateHeader() {
			SchedulerHeader header = CreateHeaderInstance(headerAppearance);
			header.Interval = Interval;
			header.Selected = Selected;
			header.Resource = Resource;
			return header;
		}
		protected internal virtual Rectangle CalculateInitialHeaderBounds(int verticalOverlap) {
			return Bounds;
		}
		protected internal abstract DayHeader CreateHeaderInstance(BaseHeaderAppearance appearance);
	}
	#endregion
	#region HorizontalSingleWeekCell
	public class HorizontalSingleWeekCell : SingleWeekCellBase {
		bool firstVisible;
		public HorizontalSingleWeekCell(BaseHeaderAppearanceContainer headerAppearance)
			: base(headerAppearance) {
		}
		public bool FirstVisible { get { return firstVisible; } set { firstVisible = value; } }
		protected internal override void SetHeaderCaption(HeaderCaptionFormatProviderBase provider, GraphicsCache cache) {
			if (provider != null) {
				string format = provider.GetHorizontalWeekCellHeaderCaption(Header);
				if (!String.IsNullOrEmpty(format)) {
					Header.Caption = String.Format(CultureInfo.CurrentCulture, format, Date);
					return;
				}
			}
			if (IsLongFormatHeaderCaption()) {
				if (IsNewYearFormatHeaderCaption())
					SetNewYearFormatHeaderCaption(cache);
				else
					SetLongFormatHeaderCaption(cache);
			} else
				SetShortFormatHeaderCaption();
		}
		protected internal virtual bool IsLongFormatHeaderCaption() {
			return FirstVisible || Date.Day == 1;
		}
		protected internal virtual bool IsNewYearFormatHeaderCaption() {
			return Date.Day == 1 && Date.Month == 1;
		}
		protected internal virtual void SetLongFormatHeaderCaption(GraphicsCache cache) {
			Header.Caption = DateTimeFormatHelper.DateToStringWithoutYearAndWeekDayOptimal(cache.Graphics, Header.CaptionAppearance.Font, Date, Header.TextBounds.Width);
		}
		protected internal virtual void SetNewYearFormatHeaderCaption(GraphicsCache cache) {
			Header.Caption = DateTimeFormatHelper.DateToStringForNewYearOptimal(cache.Graphics, Header.CaptionAppearance.Font, Date, Header.TextBounds.Width);
		}
		protected internal virtual void SetShortFormatHeaderCaption() {
			Header.Caption = Date.Day.ToString();
		}
		protected internal override Rectangle CalculateInitialHeaderBounds(int verticalOverlap) {
			Rectangle headerBounds = Bounds;
			headerBounds.Y -= verticalOverlap;
			headerBounds.Height += verticalOverlap;
			return headerBounds;
		}
		protected internal override DayHeader CreateHeaderInstance(BaseHeaderAppearance appearance) {
			return new MonthViewTimeCellHeader(appearance);
		}
	}
	#endregion
	#region MonthSingleWeekCell
	public class MonthSingleWeekCell : HorizontalSingleWeekCell {
		public MonthSingleWeekCell(BaseHeaderAppearanceContainer headerAppearance)
			: base(headerAppearance) {
		}
		protected internal virtual bool Alternate { get { return Date.Month % 2 != 0; } }
		protected internal override Color CalculateAppearanceBackColor(SchedulerColorSchema colorSchema) {
			return Alternate ? colorSchema.Cell : colorSchema.CellLight;
		}
		protected internal override Color CalculateAppearanceBorderColor(SchedulerColorSchema colorSchema) {
			return Alternate ? colorSchema.CellBorderDark : colorSchema.CellBorder;
		}
	}
	#endregion
	#region VerticalSingleWeekCell
	public class VerticalSingleWeekCell : SingleWeekCellBase {
		public VerticalSingleWeekCell(BaseHeaderAppearanceContainer headerAppearance)
			: base(headerAppearance) {
		}
		protected internal override void SetHeaderCaption(HeaderCaptionFormatProviderBase provider, GraphicsCache cache) {
			if (provider != null) {
				string format = provider.GetVerticalWeekCellHeaderCaption(Header);
				if (!String.IsNullOrEmpty(format)) {
					Header.Caption = String.Format(CultureInfo.CurrentCulture, format, Date);
					return;
				}
			}
			Header.Caption = DateTimeFormatHelper.DateToStringWithoutYearOptimal(cache.Graphics, Header.CaptionAppearance.Font, Date, Header.TextBounds.Width);
		}
		protected internal override DayHeader CreateHeaderInstance(BaseHeaderAppearance appearance) {
			return new DayHeader(appearance);
		}
	}
	#endregion
	#region SingleWeekViewInfo (abstract class)
	public abstract class SingleWeekViewInfo : SchedulerViewCellContainer {
		#region Fields
		ISupportWeekCells viewInfo;
		bool firstVisible;
		#endregion
		protected SingleWeekViewInfo(ISupportWeekCells viewInfo, Resource resource, DateTime start, SchedulerColorSchema colorSchema)
			: base(colorSchema) {
			if (viewInfo == null)
				Exceptions.ThrowArgumentException("viewInfo", viewInfo);
			if (resource == null)
				Exceptions.ThrowArgumentException("resource", resource);
			this.viewInfo = viewInfo;
			Resource = resource;
			Interval.Start = start;
		}
		#region Properties
		public ISupportWeekCells ViewInfo { get { return viewInfo; } }
		public override SchedulerHitTest HitTestType { get { return SchedulerHitTest.SingleWeek; } }
		public bool FirstVisible { get { return firstVisible; } set { firstVisible = value; } }
		#endregion
		protected internal abstract bool CalculateCellHeaderAlternate(DateTime date);
		protected internal abstract int GetFirstDayIndex();
		protected internal override void InitializeCell(SchedulerViewCellBase cell, TimeInterval interval) {
			base.InitializeCell(cell, interval);
			SingleWeekCellBase singleWeekCell = (SingleWeekCellBase)cell;
			singleWeekCell.Header.Alternate = CalculateCellHeaderAlternate(singleWeekCell.Interval.Start.Date);
		}
		public virtual void CalcLayout(DateTime[] weekDates, Rectangle[] anchorBounds, GraphicsCache cache, SchedulerHeaderPainter painter) {
			Rectangle[] cellRects = CalcCellRectangles(weekDates, anchorBounds, painter.HorizontalOverlap);
			CalcLayoutCore(weekDates, cellRects, cache, painter);
		}
		public void CalcPreliminaryLayout(DateTime[] weekDates, GraphicsCache cache, SchedulerHeaderPainter painter, bool groupSeparatorBeforeWeek, bool groupSeparatorAfterWeek) {
			if (Cells.Count == 0)
				CreateCells(weekDates);
			else {
				HorizontalSingleWeekCell cell = (HorizontalSingleWeekCell)Cells[0];
				cell.FirstVisible = FirstVisible;
			}
			CalculateFinalAppearance(ViewInfo.PaintAppearance, ColorSchema);
			InitializeCellsBorders(groupSeparatorBeforeWeek, groupSeparatorAfterWeek);
			foreach (SingleWeekCellBase cell in Cells)
				cell.HeaderPreliminaryResult = cell.Header.CalculateHeaderPreliminaryLayout(cache, painter);
		}
		internal virtual void CalcLayoutCore(DateTime[] weekDates, Rectangle[] cellRects, GraphicsCache cache, SchedulerHeaderPainter painter) {
			for (int i = 0; i < cellRects.Length; i++) {
				Cells[i].Bounds = cellRects[i];
			}
			CalculateCellsHeaders(cache, painter);
			CalcCellsLayout();
		}
		protected virtual void CreateCells(DateTime[] weekDates) {
			int count = weekDates.Length;
			for (int i = 0; i < count; i++) {
				TimeInterval dayInterval = new TimeInterval(weekDates[i], DateTimeHelper.DaySpan);
				SchedulerViewCellBase cell = CreateCell(dayInterval);
				Cells.Add(cell);
			}
		}
		protected internal virtual void InitializeCellsBorders(bool groupSeparatorBeforeWeek, bool groupSeparatorAfterWeek) {
			int count = Cells.Count;
			for (int i = 0; i < count; i++) {
				SingleWeekCellBase cell = (SingleWeekCellBase)Cells[i];
				InitializeCellBorders(cell, groupSeparatorBeforeWeek, groupSeparatorAfterWeek);
			}
		}
		protected virtual void CalculateCellsHeaders(GraphicsCache cache, SchedulerHeaderPainter painter) {
			int count = Cells.Count;
			int verticalOverlap = painter.VerticalOverlap;
			for (int i = 0; i < count; i++) {
				SingleWeekCellBase cell = (SingleWeekCellBase)Cells[i];
				Rectangle headerBounds = cell.CalculateInitialHeaderBounds(verticalOverlap);
				SchedulerHeader header = cell.Header;
				header.Bounds = headerBounds; 
				cell.SetHeaderCaption(viewInfo.GetCaptionFormatProvider(), cache);
				SchedulerHeaderPreliminaryLayoutResult headerResult = cell.HeaderPreliminaryResult;
				headerResult = header.CalculateHeaderPreliminaryLayout(cache, painter);
				headerBounds.Height = headerResult.Size.Height;
				header.Bounds = headerBounds;
				header.CalcLayout(cache, headerResult);
				cell.SetHeaderCaption(viewInfo.GetCaptionFormatProvider(), cache);
				CacheHeaderSkinElementInfo(header, painter);
			}
		}
		protected internal abstract void CacheHeaderSkinElementInfo(SchedulerHeader header, SchedulerHeaderPainter painter);
		protected internal virtual Rectangle[] SeparateWeekendCell(Rectangle[] cellRects, DateTime[] dates) {
			XtraSchedulerDebug.Assert(cellRects.Length > 0);
			int splitModule = DateTimeHelper.FindDayOfWeekNumber(dates, DayOfWeek.Saturday);
			return SplitWeekCells(cellRects, splitModule);
		}
		public static Rectangle[] SplitWeekCells(Rectangle[] rects, int splitModule) {
			if (splitModule < 1 || splitModule > rects.Length)
				return rects;
			List<Rectangle> result = new List<Rectangle>();
			for (int i = 0; i < rects.Length; i++) {
				if ((i + 1) % 7 == splitModule) {
					result.AddRange(RectUtils.SplitVertically(rects[i], 2));
				} else
					result.Add(rects[i]);
			}
			return result.ToArray();
		}
		protected internal virtual void CalcCellsLayout() {
			int count = Cells.Count;
			for (int i = 0; i < count; i++) {
				SingleWeekCellBase cell = (SingleWeekCellBase)Cells[i];
				CalcCellLayoutCore(cell);
			}
		}
		protected internal virtual void CalcCellLayoutCore(SingleWeekCellBase cell) {
			bool hideHeaderCaption = ViewInfo.ShouldHideCellContent(cell);
			cell.CalcLayout(hideHeaderCaption);
		}
		internal void SetViewInfo(ISupportWeekCells viewInfo) {
			this.viewInfo = viewInfo;
		}
		protected internal abstract void InitializeCellBorders(SingleWeekCellBase cell, bool groupSeparatorBeforeWeek, bool groupSeparatorAfterWeek);
		protected internal abstract int CalculateCellColumnIndex(SingleWeekCellBase cell);
		protected internal abstract Rectangle[] CalcCellRectangles(DateTime[] dates, Rectangle[] anchorBounds, int horizontalOverlap);
	}
	#endregion
	#region HorizontalSingleWeekViewInfoBase (abstract class)
	public abstract class HorizontalSingleWeekViewInfoBase : SingleWeekViewInfo {
		protected HorizontalSingleWeekViewInfoBase(ISupportWeekCells viewInfo, Resource resource, DateTime start, SchedulerColorSchema colorSchema)
			: base(viewInfo, resource, start, colorSchema) {
		}
		protected override void CreateCells(DateTime[] weekDates) {
			base.CreateCells(weekDates);
			if (Cells.Count > 0) {
				HorizontalSingleWeekCell cell = (HorizontalSingleWeekCell)Cells[0];
				cell.FirstVisible = FirstVisible;
			}
		}
		protected internal override Rectangle[] CalcCellRectangles(DateTime[] dates, Rectangle[] anchorBounds, int horizontalOverlap) {
			List<Rectangle> cellsBounds = new List<Rectangle>();
			for (int i = 0; i < anchorBounds.Length; i++) {
				Rectangle bounds = Bounds;
				bounds.X = anchorBounds[i].Left;
				bounds.Width = anchorBounds[i].Width;
				cellsBounds.Add(bounds);
			}
			Rectangle[] cellRects = cellsBounds.ToArray();
			if (ShouldCompressWeekend())
				cellRects = SeparateWeekendCell(cellRects, dates);
			return cellRects;
		}
		protected internal override SchedulerViewCellBase CreateCellInstance() {
			return new HorizontalSingleWeekCell(ViewInfo.PaintAppearance.SingleWeekCellHeaderAppearance);
		}
		protected internal override int CalculateCellColumnIndex(SingleWeekCellBase cell) {
			if (!Cells.Contains(cell))
				return -1;
			int cellDayIndex = (int)cell.Interval.Start.DayOfWeek;
			int firstDayIndex = GetFirstDayIndex();
			int columnIndex = cellDayIndex - firstDayIndex;
			columnIndex = columnIndex < 0 ? columnIndex + 7 : columnIndex;
			if (!ViewInfo.CompressWeekend)
				return columnIndex;
			if (cellDayIndex >= firstDayIndex)
				return columnIndex;
			return columnIndex - 1;
		}
		protected internal override void InitializeCellBorders(SingleWeekCellBase cell, bool groupSeparatorBeforeWeek, bool groupSeparatorAfterWeek) {
			int columnIndex = CalculateCellColumnIndex(cell);
			cell.HasLeftBorder = columnIndex > 0;
			cell.HasRightBorder = false;
			cell.HasTopBorder = HasTopBorder;
			cell.HasBottomBorder = CalculateCellBottomBorder(cell);
			InitializeCellHeaderBorders(cell);
		}
		protected internal virtual bool CalculateCellBottomBorder(SingleWeekCellBase cell) {
			bool separatedCell = IsSeparatedCell(cell.Date.DayOfWeek);
			return separatedCell ? !HasTopBorder : HasBottomBorder;
		}
		protected internal virtual void InitializeCellHeaderBorders(SingleWeekCellBase cell) {
			cell.Header.HasLeftBorder = true;
			cell.Header.HasTopBorder = true;
			cell.Header.HasBottomBorder = true;
			cell.Header.HasRightBorder = true;
		}
		protected internal override void CacheHeaderSkinElementInfo(SchedulerHeader header, SchedulerHeaderPainter painter) {
		}
		protected internal abstract bool IsSeparatedCell(DayOfWeek cellDay);
		protected internal abstract bool ShouldCompressWeekend();
	}
	#endregion
	#region HorizontalSingleWeekViewInfo
	public class HorizontalSingleWeekViewInfo : HorizontalSingleWeekViewInfoBase {
		DateTime today;
		public HorizontalSingleWeekViewInfo(WeekViewInfo viewInfo, Resource resource, DateTime start, SchedulerColorSchema colorSchema)
			: base(viewInfo, resource, start, colorSchema) {
			TimeZoneHelper timeZoneEngine = ViewInfo.View.Control.InnerControl.TimeZoneHelper;
			this.today = timeZoneEngine.ToClientTime(DateTime.Now).Date;
		}
		public new WeekViewInfo ViewInfo { get { return (WeekViewInfo)base.ViewInfo; } }
		protected internal DateTime Today { get { return today; } set { today = value; } }
		protected internal override bool CalculateCellHeaderAlternate(DateTime date) {
			return (ViewInfo.View.HeaderAlternateEnabled) ? date == today : false;
		}
		protected internal override bool ShouldCompressWeekend() {
			return ViewInfo.CompressWeekend;
		}
		protected internal override bool IsSeparatedCell(DayOfWeek cellDay) {
			return ViewInfo.ShowWeekend && ViewInfo.CompressWeekend && cellDay == DayOfWeek.Saturday;
		}
		protected internal override int GetFirstDayIndex() {
			TimeIntervalCollection weekIntervals = ((WeekViewInfo)ViewInfo).VisibleIntervals;
			XtraSchedulerDebug.Assert(((WeekIntervalCollection)weekIntervals).FirstDayOfWeek == ViewInfo.FirstDayOfWeek);
			return (int)weekIntervals[0].Start.DayOfWeek;
		}
	}
	#endregion
	#region MonthSingleWeekViewInfo
	public class MonthSingleWeekViewInfo : HorizontalSingleWeekViewInfo {
		public MonthSingleWeekViewInfo(MonthViewInfo viewInfo, Resource resource, DateTime start, SchedulerColorSchema colorSchema)
			: base(viewInfo, resource, start, colorSchema) {
		}
		protected internal override SchedulerViewCellBase CreateCellInstance() {
			return new MonthSingleWeekCell(ViewInfo.PaintAppearance.SingleWeekCellHeaderAppearance);
		}
	}
	#endregion
	#region VerticalSingleWeekViewInfoBase (abstract class)
	public abstract class VerticalSingleWeekViewInfoBase : SingleWeekViewInfo {
		#region CellIndexes
		static int[,] cellColumnIndex = new int[7, 7] {  
			  {1, 0, 0, 0, 1, 1, 1 }, 
			  {1, 0, 0, 0, 1, 1, 1 }, 
			  {1, 1, 0, 0, 0, 1, 1 }, 
			  {1, 1, 1, 0, 0, 0, 1 }, 
			  {0, 1, 1, 1, 0, 0, 0 }, 
			  {0, 0, 1, 1, 1, 0, 0 }, 
			  {0, 0, 0, 1, 1, 1, 0 }
			};
		static int[,] cellRowIndex = new int[7, 7] {  
			  {0, 1, 2, 0, 1, 2, 0 }, 
			  {2, 0, 1, 2, 0, 1, 2 }, 
			  {1, 2, 0, 1, 2, 0, 1 }, 
			  {0, 1, 2, 0, 1, 2, 0 }, 
			  {2, 0, 1, 2, 0, 1, 2 }, 
			  {1, 2, 0, 1, 2, 0, 1 }, 
			  {0, 1, 2, 0, 1, 2, 0 }
			};
		#endregion
		protected VerticalSingleWeekViewInfoBase(ISupportWeekCells viewInfo, Resource resource, DateTime start, SchedulerColorSchema colorSchema)
			: base(viewInfo, resource, start, colorSchema) {
		}
		protected internal override SchedulerViewCellBase CreateCellInstance() {
			return new VerticalSingleWeekCell(ViewInfo.PaintAppearance.SingleWeekCellHeaderAppearance);
		}
		protected internal override Rectangle[] CalcCellRectangles(DateTime[] dates, Rectangle[] anchorBounds, int horizontalOverlap) {
			bool isSundayPresent = DateTimeHelper.FindDayOfWeekNumber(dates, DayOfWeek.Sunday) >= 0;
#if (DEBUG)
			bool isSaturdayPresent = DateTimeHelper.FindDayOfWeekNumber(dates, DayOfWeek.Saturday) >= 0;
			XtraSchedulerDebug.Assert(isSundayPresent == isSaturdayPresent);
#endif
			List<Rectangle> cells = new List<Rectangle>();
			if (IsHorizontalDivideRequired(dates)) {
				Rectangle[] columnRects = RectUtils.SplitHorizontally(Bounds, 2);
				columnRects[0].Width += horizontalOverlap;
				cells.AddRange(RectUtils.SplitVertically(columnRects[0], 3));
				cells.AddRange(RectUtils.SplitVertically(columnRects[1], 3));
			} else {
				int datesLength = dates.Length;
				cells.AddRange(RectUtils.SplitVertically(Bounds, isSundayPresent ? datesLength - 1 : datesLength));
			}
			Rectangle[] cellRects = cells.ToArray();
			if (ViewInfo.CompressWeekend)
				cellRects = SeparateWeekendCell(cellRects, dates);
			return cellRects;
		}
		private bool IsHorizontalDivideRequired(DateTime[] dates) {
			return dates.Length > 4;
		}
		protected internal override int CalculateCellColumnIndex(SingleWeekCellBase cell) {
			if (!Cells.Contains(cell))
				return -1;
			return cellColumnIndex[GetFirstDayIndex(), (int)cell.Interval.Start.DayOfWeek];
		}
		protected internal virtual int CalculateCellRowIndex(SingleWeekCellBase cell) {
			if (!Cells.Contains(cell))
				return -1;
			return cellRowIndex[GetFirstDayIndex(), (int)cell.Interval.Start.DayOfWeek];
		}
		protected internal override void InitializeCellBorders(SingleWeekCellBase cell, bool groupSeparatorBeforeWeek, bool groupSeparatorAfterWeek) {
			int columnIndex = CalculateCellColumnIndex(cell);
			bool isLastColumn = columnIndex > 0;
			cell.HasLeftBorder = isLastColumn || HasLeftBorder;
			cell.HasTopBorder = false;
			cell.HasRightBorder = isLastColumn && HasRightBorder;
			cell.HasBottomBorder = IsLastRow(cell) && HasBottomBorder;
			InitializeCellHeaderBorders(cell, groupSeparatorBeforeWeek, groupSeparatorAfterWeek, columnIndex);
		}
		protected internal virtual bool IsLastRow(SingleWeekCellBase cell) {
			if (cell.Date.DayOfWeek == DayOfWeek.Saturday)
				return false;
			return CalculateCellRowIndex(cell) == 2;
		}
		protected internal virtual bool IsFirstRow(SingleWeekCellBase cell) {
			return CalculateCellRowIndex(cell) == 0;
		}
		protected internal virtual void InitializeCellHeaderBorders(SingleWeekCellBase cell, bool groupSeparatorBeforeWeek, bool groupSeparatorAfterWeek, int columnIndex) {
			bool isFirstColumn = columnIndex == 0;
			bool isLastColumn = columnIndex > 0;
			bool isSunday = cell.Interval.Start.DayOfWeek == DayOfWeek.Sunday;
			cell.Header.HasLeftBorder = (isFirstColumn && !groupSeparatorBeforeWeek) || (isLastColumn && isSunday);
			cell.Header.HasTopBorder = true;
			cell.Header.HasBottomBorder = true;
			cell.Header.HasRightBorder = isLastColumn && !groupSeparatorAfterWeek;
			cell.Header.HasSeparator = isLastColumn && !isSunday;
		}
		protected internal override void CacheHeaderSkinElementInfo(SchedulerHeader header, SchedulerHeaderPainter painter) {
			if (header.Alternate)
				return;
			if (this.Resource == ResourceBase.Empty)
				return;
			if (painter.ShouldCacheSkinElementInfo)
				header.CachedSkinElementInfo = painter.PrepareCachedSkinElementInfo(header, ColorSchema.Cell);
		}
	}
	#endregion
	#region VerticalSingleWeekViewInfo
	public class VerticalSingleWeekViewInfo : VerticalSingleWeekViewInfoBase {
		DateTime today;
		public VerticalSingleWeekViewInfo(WeekViewInfo viewInfo, Resource resource, DateTime start, SchedulerColorSchema colorSchema)
			: base(viewInfo, resource, start, colorSchema) {
			TimeZoneHelper timeZoneEngine = ViewInfo.View.Control.InnerControl.TimeZoneHelper;
			this.today = timeZoneEngine.ToClientTime(DateTime.Now).Date;
		}
		public new WeekViewInfo ViewInfo { get { return (WeekViewInfo)base.ViewInfo; } }
		public DateTime Today { get { return today; } set { today = value; } }
		protected internal override bool CalculateCellHeaderAlternate(DateTime date) {
			return (ViewInfo.View.HeaderAlternateEnabled) ? date == today : false;
		}
		protected internal override int GetFirstDayIndex() {
			return (int)ViewInfo.FirstDayOfWeek;
		}
	}
	#endregion
}
