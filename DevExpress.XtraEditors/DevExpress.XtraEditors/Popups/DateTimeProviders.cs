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
using System.Linq;
using System.Text;
using System.ComponentModel;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Controls;
using System.Globalization;
namespace DevExpress.XtraEditors.Popups {
	[ToolboxItem(false)]
	public class BaseCalendarItemsProvider {
		protected internal int minIndex, maxIndex;
		public BaseCalendarItemsProvider() {
			minIndex = maxIndex = -1;
		}
		public void SetMinMaxValue(int min, int max) {
			this.maxIndex = max;
			this.minIndex = min;
		}
		protected internal bool MinMaxChanged {
			get {
				return minIndex != -1 && minIndex != -1;
			}
		}
	}
	[ToolboxItem(false)]
	public class MonthItemsCalculator : IItemCalculator {
		#region IItemCalculator Members
		void IItemCalculator.Calculate(DevExpress.Utils.Drawing.GraphicsInfo gInfo, PickItemInfo info, object itemData) {
		}
		#endregion
	}
	public abstract class ItemPainterBase : IItemPainter {
		public ItemPainterBase() {
		}
		protected TouchCalendar GetCalendar(PickItemInfo info) {
			if(info.Panel.Container.Owner is DateEditTouchCalendar)
				return (DateEditTouchCalendar)info.Panel.Container.Owner;
			if(info.Panel.Container.Owner is TimeEditTouchCalendar)
				return (TimeEditTouchCalendar)info.Panel.Container.Owner;
			return null;
		}
		#region IItemPainter
		void IItemPainter.Draw(GraphicsCache cache, PickItemInfo info, IPickItemsContainerDrawInfo drawInfo) {
			DrawCore(cache, info, drawInfo);
		}
		#endregion
		protected abstract int StringLength { get; }
		protected abstract void DrawCore(GraphicsCache cache, PickItemInfo info, IPickItemsContainerDrawInfo drawInfo);
	}
	[ToolboxItem(false)]
	public class MonthItemsPainter : ItemPainterBase {
		protected override void DrawCore(GraphicsCache cache, PickItemInfo info, IPickItemsContainerDrawInfo drawInfo) {
			PickItemsPainter painter = new PickItemsPainter();
			bool descriptionIsExist = GetCalendar(info).ShowTime();
			string firstString = GetCalendar(info).ShowTime() ? painter.ConvertIntToString(info.ItemIndex + 1, StringLength) : CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames[info.ItemIndex];
			string description = descriptionIsExist && painter.ShouldDrawDescription(info) ? CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[info.ItemIndex].ToLower() : string.Empty;
			painter.DrawItem(cache, drawInfo, info, firstString, description);
		}
		protected override int StringLength {
			get { return 2; }
		}
	}
	[ToolboxItem(false)]
	public class MonthItemsProvider : BaseCalendarItemsProvider, IItemsProvider {
		int itemsCount;
		public MonthItemsProvider(int count) : base() {
			itemsCount = count;
		}
		public void SetItemsCount(int count) {
			itemsCount = count;
		}
		#region IItemsProvider Members
		IItemCalculator IItemsProvider.GetItemCalculator(int itemIndex) {
			return new MonthItemsCalculator();
		}
		object IItemsProvider.GetItemData(int itemIndex) {
			return itemIndex;
		}
		IItemPainter IItemsProvider.GetItemPainter(int itemIndex) {
			return new MonthItemsPainter();
		}
		const int firstItemIndex = 1;
		int IItemsProvider.FirstItemIndex {
			get { return firstItemIndex; }
		}
		int IItemsProvider.ItemsCount {
			get {
				return LastDrawingItemIndex - FirstDrawingItemIndex + 1;
			}
		}
		int IItemsProvider.MaxVisibleItemsCount {
			get { return 0; }
		}
		public int FirstDrawingItemIndex {
			get {
				return MinMaxChanged ? minIndex : firstItemIndex;
			}
		}
		public int LastDrawingItemIndex {
			get {
				return MinMaxChanged ? maxIndex : itemsCount - 1 + firstItemIndex;
			}
		}
		public bool IsDateProvider {
			get { return true; }
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class YearItemsCalculator : IItemCalculator {
		#region IItemCalculator Members
		void IItemCalculator.Calculate(DevExpress.Utils.Drawing.GraphicsInfo gInfo, PickItemInfo info, object itemData) {
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class YearItemsPainter : ItemPainterBase {
		protected override void DrawCore(GraphicsCache cache, PickItemInfo info, IPickItemsContainerDrawInfo drawInfo) {
			PickItemsPainter painter = new PickItemsPainter();
			string firstString = painter.ConvertIntToString(info.ItemIndex + 1, StringLength);
			painter.DrawItem(cache, drawInfo, info, firstString, string.Empty);
		}
		protected override int StringLength {
			get { return 4; }
		}
	}
	[ToolboxItem(false)]
	public class YearItemsProvider : BaseCalendarItemsProvider, IItemsProvider {
		int itemsCount;
		public YearItemsProvider(int count) : base() {
			itemsCount = count;
		}
		public void SetItemsCount(int count) {
			itemsCount = count;
		}
		#region IItemsProvider Members
		IItemCalculator IItemsProvider.GetItemCalculator(int itemIndex) {
			return new YearItemsCalculator();
		}
		object IItemsProvider.GetItemData(int itemIndex) {
			return itemIndex;
		}
		IItemPainter IItemsProvider.GetItemPainter(int itemIndex) {
			return new YearItemsPainter();
		}
		const int firstItemIndex = 1;
		int IItemsProvider.FirstItemIndex {
			get { return firstItemIndex; }
		}
		int IItemsProvider.ItemsCount {
			get {
				return LastDrawingItemIndex - FirstDrawingItemIndex + 1;
			}
		}
		int IItemsProvider.MaxVisibleItemsCount {
			get { return 0; }
		}
		public int FirstDrawingItemIndex {
			get {
				return MinMaxChanged ? minIndex : firstItemIndex;
			}
		}
		public int LastDrawingItemIndex {
			get {
				return MinMaxChanged ? maxIndex : itemsCount - 1 + firstItemIndex;
			}
		}
		public bool IsDateProvider {
			get { return true; }
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class DaysItemsProvider : BaseCalendarItemsProvider, IItemsProvider {
		int itemsCount;
		public DaysItemsProvider(int count) {
			itemsCount = count;
		}
		public void SetItemsCount(int count) {
			itemsCount = count;
		}
		#region IItemsProvider Members
		IItemCalculator IItemsProvider.GetItemCalculator(int itemIndex) {
			return new DaysItemsCalculator();
		}
		object IItemsProvider.GetItemData(int itemIndex) {
			return itemIndex;
		}
		IItemPainter IItemsProvider.GetItemPainter(int itemIndex) {
			return CreatePainter();
		}
		protected virtual IItemPainter CreatePainter() {
			return new DaysItemsPainter();
		}
		int IItemsProvider.FirstItemIndex { get { return FirstItemIndexCore; } }
		protected virtual int FirstItemIndexCore { get { return 1; } }
		int IItemsProvider.ItemsCount {
			get {
				return LastDrawingItemIndex - FirstDrawingItemIndex + 1;
			}
		}
		int IItemsProvider.MaxVisibleItemsCount {
			get { return 0; }
		}
		public int FirstDrawingItemIndex {
			get {
				return MinMaxChanged ? minIndex : FirstItemIndexCore;
			}
		}
		public int LastDrawingItemIndex {
			get {
				return MinMaxChanged ? maxIndex : itemsCount - 1 + FirstItemIndexCore;
			}
		}
		public bool IsDateProvider {
			get { return true; }
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class DaysItemsCalculator : IItemCalculator {
		#region IItemCalculator Members
		void IItemCalculator.Calculate(DevExpress.Utils.Drawing.GraphicsInfo gInfo, PickItemInfo info, object itemData) {
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class DaysItemsPainter : ItemPainterBase {
		protected override void DrawCore(GraphicsCache cache, PickItemInfo info, IPickItemsContainerDrawInfo drawInfo) {
			PickItemsPainter painter = new PickItemsPainter();
			string firstString = painter.ConvertIntToString(info.ItemIndex + 1, StringLength);
			bool descriptionIsExist = GetCalendar(info).ShowTime();
			string description = descriptionIsExist && painter.ShouldDrawDescription(info) ? GetDayOfWeek(info).ToLower() : string.Empty;
			painter.DrawItem(cache, drawInfo, info, firstString, description);
		}
		protected string GetDayOfWeek(PickItemInfo info) {
			PickItemsContainer container = info.Panel.Container;
			DateTime date = GetCalendar(info).SelectedDate;
			int year = date.Year;
			int month = date.Month;
			int day = info.ItemIndex + 1;
			date = new DateTime(year, month, day);
			return CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int)date.DayOfWeek];
		}
		protected override int StringLength {
			get { return 2; }
		}
	}
	[ToolboxItem(false)]
	public class MinsItemsProvider : IItemsProvider {
		int itemsCount;
		public MinsItemsProvider(int count) {
			itemsCount = count;
		}
		public void SetItemsCount(int count) {
			itemsCount = count;
		}
		#region IItemsProvider Members
		IItemCalculator IItemsProvider.GetItemCalculator(int itemIndex) {
			return new MinsItemsCalculator();
		}
		object IItemsProvider.GetItemData(int itemIndex) {
			return itemIndex;
		}
		IItemPainter IItemsProvider.GetItemPainter(int itemIndex) {
			return CreatePainter(itemIndex);
		}
		protected virtual IItemPainter CreatePainter(int itemIndex) {
			return new MinsItemsPainter();
		}
		const int firstItemIndex = 0;
		int IItemsProvider.FirstItemIndex {
			get { return firstItemIndex; }
		}
		int IItemsProvider.ItemsCount {
			get { return itemsCount; }
		}
		int IItemsProvider.MaxVisibleItemsCount {
			get { return 0; }
		}
		public bool IsDateProvider {
			get { return false; }
		}
		#endregion
		public int FirstDrawingItemIndex {
			get { return firstItemIndex; }
		}
		public int LastDrawingItemIndex {
			get { return itemsCount - 1 + firstItemIndex; }
		}
	}
	[ToolboxItem(false)]
	public class MinsItemsCalculator : IItemCalculator {
		#region IItemCalculator Members
		void IItemCalculator.Calculate(DevExpress.Utils.Drawing.GraphicsInfo gInfo, PickItemInfo info, object itemData) {
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class MinsItemsPainter : ItemPainterBase {
		protected override void DrawCore(GraphicsCache cache, PickItemInfo info, IPickItemsContainerDrawInfo drawInfo) {
			PickItemsPainter painter = new PickItemsPainter();
			TouchCalendar calendar = (TouchCalendar)info.Panel.Container.Owner;
			int minuteIncrement = calendar == null ? 1 : calendar.GetMinuteIncrement();
			string firstString = painter.ConvertIntToString(info.ItemIndex * minuteIncrement, StringLength);
			string description = painter.ShouldDrawDescription(info) ? Localizer.Active.GetLocalizedString(StringId.Mins) : string.Empty;
			painter.DrawItem(cache, drawInfo, info, firstString, description);
		}
		protected override int StringLength {
			get { return 2; }
		}
	}
	[ToolboxItem(false)]
	public class HoursItemsProvider : IItemsProvider {
		int itemsCount;
		public HoursItemsProvider(int count) {
			itemsCount = count;
			this.StartIndex = 0;
		}
		public int StartIndex { get; set; }
		public void SetItemsCount(int count) {
			itemsCount = count;
		}
		#region IItemsProvider Members
		IItemCalculator IItemsProvider.GetItemCalculator(int itemIndex) {
			return new HoursItemsCalculator();
		}
		object IItemsProvider.GetItemData(int itemIndex) {
			return itemIndex;
		}
		IItemPainter IItemsProvider.GetItemPainter(int itemIndex) {
			return CreatePainter(itemIndex);
		}
		protected virtual IItemPainter CreatePainter(int itemIndex) {
			return new HoursItemsPainter(StartIndex);
		}
		int IItemsProvider.FirstItemIndex {
			get { return StartIndex; }
		}
		int IItemsProvider.ItemsCount {
			get { return itemsCount; }
		}
		int IItemsProvider.MaxVisibleItemsCount {
			get { return 0; }
		}
		public bool IsDateProvider {
			get {return false; }
		}
		#endregion
		public int FirstDrawingItemIndex {
			get { return StartIndex; }
		}
		public int LastDrawingItemIndex {
			get { return itemsCount - 1 + StartIndex; }
		}
	}
	[ToolboxItem(false)]
	public class HoursItemsCalculator : IItemCalculator {
		#region IItemCalculator Members
		void IItemCalculator.Calculate(DevExpress.Utils.Drawing.GraphicsInfo gInfo, PickItemInfo info, object itemData) {
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class HoursItemsPainter : ItemPainterBase {
		int startIndex;
		public HoursItemsPainter(int startIndex) {
			this.startIndex = startIndex;
		}
		protected override void DrawCore(GraphicsCache cache, PickItemInfo info, IPickItemsContainerDrawInfo drawInfo) {
			PickItemsPainter painter = new PickItemsPainter();
			string firstString = painter.ConvertIntToString(info.ItemIndex + startIndex, StringLength);
			string description = painter.ShouldDrawDescription(info) ? Localizer.Active.GetLocalizedString(StringId.Hours) : string.Empty;
			painter.DrawItem(cache, drawInfo, info, firstString, description);
		}
		protected override int StringLength {
			get { return 2; }
		}
	}
	[ToolboxItem(false)]
	public class SecondsItemsProvider : IItemsProvider {
		int itemsCount;
		public SecondsItemsProvider(int count) {
			itemsCount = count;
		}
		public void SetItemsCount(int count) {
			itemsCount = count;
		}
		#region IItemsProvider Members
		IItemCalculator IItemsProvider.GetItemCalculator(int itemIndex) {
			return new SecondsItemsCalculator();
		}
		object IItemsProvider.GetItemData(int itemIndex) {
			return itemIndex;
		}
		IItemPainter IItemsProvider.GetItemPainter(int itemIndex) {
			return CreatePainter(itemIndex);
		}
		protected virtual IItemPainter CreatePainter(int itemIndex) {
			return new SecondsItemsPainter();
		}
		const int firstItemIndex = 0;
		int IItemsProvider.FirstItemIndex {
			get { return firstItemIndex; }
		}
		int IItemsProvider.ItemsCount {
			get { return itemsCount; }
		}
		int IItemsProvider.MaxVisibleItemsCount {
			get { return 0; }
		}
		public bool IsDateProvider {
			get { return false; }
		}
		#endregion
		public int FirstDrawingItemIndex {
			get { return firstItemIndex; }
		}
		public int LastDrawingItemIndex {
			get { return itemsCount - 1 + firstItemIndex; }
		}
	}
	[ToolboxItem(false)]
	public class SecondsItemsCalculator : IItemCalculator {
		#region IItemCalculator Members
		void IItemCalculator.Calculate(DevExpress.Utils.Drawing.GraphicsInfo gInfo, PickItemInfo info, object itemData) {
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class SecondsItemsPainter : ItemPainterBase {
		protected override void DrawCore(GraphicsCache cache, PickItemInfo info, IPickItemsContainerDrawInfo drawInfo) {
			PickItemsPainter painter = new PickItemsPainter();
			TouchCalendar calendar = (TouchCalendar)info.Panel.Container.Owner;
			int secondIncrement = calendar == null ? 1 : calendar.GetSecondIncrement();
			string firstString = painter.ConvertIntToString(info.ItemIndex * secondIncrement, StringLength);
			string description = painter.ShouldDrawDescription(info) ? Localizer.Active.GetLocalizedString(StringId.Secs) : string.Empty;
			painter.DrawItem(cache, drawInfo, info, firstString, description);
		}
		protected override int StringLength {
			get { return 2; }
		}
	}
	[ToolboxItem(false)]
	public class MeridiemItemsCalculator : IItemCalculator {
		#region IItemCalculator Members
		void IItemCalculator.Calculate(DevExpress.Utils.Drawing.GraphicsInfo gInfo, PickItemInfo info, object itemData) {
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class MeridiemItemsPainter : ItemPainterBase {
		protected override void DrawCore(GraphicsCache cache, PickItemInfo info, IPickItemsContainerDrawInfo drawInfo) {
			PickItemsPainter painter = new PickItemsPainter();
			bool descriptionIsExist = GetCalendar(info).ShowTime();
			string firstString = info.ItemIndex == 0 ? "AM" : "PM";
			painter.DrawItem(cache, drawInfo, info, firstString, string.Empty);
		}
		protected override int StringLength {
			get { return -1; }
		}
	}
	[ToolboxItem(false)]
	public class MeridiemItemsProvider : IItemsProvider {
		int itemsCount;
		public MeridiemItemsProvider(int count) {
			itemsCount = count;
		}
		public void SetItemsCount(int count) {
			itemsCount = count;
		}
		#region IItemsProvider Members
		IItemCalculator IItemsProvider.GetItemCalculator(int itemIndex) {
			return new MeridiemItemsCalculator();
		}
		object IItemsProvider.GetItemData(int itemIndex) {
			return itemIndex;
		}
		IItemPainter IItemsProvider.GetItemPainter(int itemIndex) {
			return new MeridiemItemsPainter();
		}
		const int firstItemIndex = 0;
		int IItemsProvider.FirstItemIndex {
			get { return firstItemIndex; }
		}
		int IItemsProvider.ItemsCount {
			get { return itemsCount; }
		}
		int IItemsProvider.MaxVisibleItemsCount {
			get { return 0; }
		}
		public bool IsDateProvider {
			get { return false; }
		}
		#endregion
		public int FirstDrawingItemIndex {
			get { return firstItemIndex; }
		}
		public int LastDrawingItemIndex {
			get { return itemsCount - 1 + firstItemIndex; }
		}
	}
}
