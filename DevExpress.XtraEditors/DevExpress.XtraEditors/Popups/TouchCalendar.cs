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
using DevExpress.Data.Mask;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.XtraEditors.Popup;
using DevExpress.Utils.Drawing;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraEditors.ViewInfo;
using System.Drawing.Imaging;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.XtraEditors.Popups {
	[ToolboxItem(false)]
	public abstract class TouchCalendar : BaseStyleControl, IPickItemsClientBoundsProvider, IPickItemContainerHandler {
		public TouchCalendar() {
			InitializeTouchCalendar();
		}
		protected virtual void InitializeTouchCalendar() {
			SelectedDate = DateTime.Today;
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			PickContainer = CreatePickContainer();
			TotalProviders = 0;
			CreateProviders();
			FocusOnMouseDown = false;
			SetStyle(ControlStyles.Selectable, false);
			SetStyle(ControlStyles.UserMouse, false);
		}
		public int TotalProviders { get; set; }
		public DateTime SelectedDate { get; set; }
		protected virtual CalendarPickItemsContainer CreatePickContainer() {
			return new CalendarPickItemsContainer() { Owner = this };
		}
		public virtual void OnDateChanged() {
			DateTime newDate = GetDateTimeFromContainer();
			if(SelectedDate != null && SelectedDate.Equals(newDate))
				return;
			SelectedDate = newDate;
			if(PickContainer.IsReady) CheckSelectedDate();
			if(ShouldUpdatePanels) UpdatePanels();
		}
		public virtual bool RaiseDisableCalendarDate(DateTime date) {
			return false;
		}
		protected virtual void CheckSelectedDate() {
			if(!(this is DateEditTouchCalendar)) return;
			DateTime minValue = ((DateEditTouchCalendar)this).OwnerEdit.Properties.MinValue;
			DateTime maxValue = ((DateEditTouchCalendar)this).OwnerEdit.Properties.MaxValue;
			if(maxValue == DateTime.MinValue) maxValue = DateTime.MaxValue;
			for(DateTime date = SelectedDate; date <= maxValue && date < DateTime.MaxValue.AddDays(-1); date = date.AddDays(1)) {
				if(SelectEnabledDate(date)) return;
			}
			for(DateTime date = SelectedDate; date >= minValue && date >= DateTime.MinValue.AddDays(1); date = date.AddDays(-1)) {
				if(SelectEnabledDate(date)) return;
			}
		}
		protected bool SelectEnabledDate(DateTime date) {
			if(RaiseDisableCalendarDate(date)) return false;
			SelectedDate = date;
			SetDate();
			return true;
		}
		protected internal DateTime GetDateTimeFromContainer() {
			int year = GetValueFromContainer(typeof(YearItemsProvider));
			if(year == -1) year = SelectedDate.Year;
			else year += 1;
			int month = GetValueFromContainer(typeof(MonthItemsProvider));
			if(month == -1) month = SelectedDate.Month;
			else month += 1;
			PickItemsPanel panel = GetPanelFromType(typeof(DaysItemsProvider));
			int day = panel == null ? SelectedDate.Day : panel.SelectedItemIndex + 1;
			int daysInMonth = DateTime.DaysInMonth(year, month);
			if(panel != null) {
				if(day > daysInMonth)
					panel.SelectItem(daysInMonth - 1);
				panel.IsReady = false;
				if(panel.ItemsProvider.ItemsCount != daysInMonth)
					((DaysItemsProvider)panel.ItemsProvider).SetItemsCount(daysInMonth);
			}
			day = Math.Min(day, daysInMonth);
			int hour = GetValueFromContainer(typeof(HoursItemsProvider));
			int minute = GetValueFromContainer(typeof(MinsItemsProvider));
			int second = GetValueFromContainer(typeof(SecondsItemsProvider));
			if (hour == -1 && minute == -1 && second == -1)
				return new DateTime(year, month, day);
			if(hour == -1) hour = 0;
			int meridiem = GetValueFromContainer(typeof(MeridiemItemsProvider)); 
			if(meridiem >= 0 && ((meridiem == 1 && hour % 12 != 0) || (meridiem != 1 && hour % 12 == 0))) 
				hour = (hour + 12) % 24;
			if(minute == -1) minute = 0;
			if(second == -1) second = 0;
			return new DateTime(year, month, day, hour, minute, second);
		}
		public virtual DateTime GetDateFromIndex(IItemsProvider provider, int itemIndex) {
			DateTime date = GetDateTimeFromContainer();
			int index = itemIndex + 1;
			if(provider is YearItemsProvider) return new DateTime(index, 1, 1);
			if(provider is MonthItemsProvider) return new DateTime(date.Year, index, 1);
			if(provider is DaysItemsProvider) {
				if(index > DateTime.DaysInMonth(date.Year, date.Month)) return date;
				return new DateTime(date.Year, date.Month, index);
			}
			return date;
		}
		public DateTime[] GetDateRangeForProvider(IItemsProvider provider, DateTime date) {
			DateTime[] range = new DateTime[2];
			if(provider is MonthItemsProvider) {
				range[0] = new DateTime(date.Year, date.Month, 1);
				range[1] = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
				return range;
			}
			if(provider is YearItemsProvider) {
				range[0] = new DateTime(date.Year, 1, 1);
				range[1] = new DateTime(date.Year, 12, 31);
				return range;
			}
			range[0] = range[1] = date;
			return range;
		}
		protected virtual bool ShouldUpdatePanels {
			get {
				return false;
			}
		}
		protected internal abstract int GetMinuteIncrement();
		protected internal abstract int GetSecondIncrement();
		protected internal void UpdatePanels() {
			if(!PickContainer.IsReady) return;
			for(int i = 0; i < Providers.Count; i++) {
				if(!(Providers[i] is BaseCalendarItemsProvider)) continue;
				BaseCalendarItemsProvider prov = (BaseCalendarItemsProvider)Providers[i];
				if(prov == null) continue;
				PickItemsPanel panel = PickContainer.FindPanelByItem(PickContainer.Items[i]);
				if(panel == null || !panel.ItemIsSelected) 
					continue;
				int selectedItemIndex = panel.SelectedItemIndex;
				if(panel.ItemIsSelected) {
					UpdateMinMaxValue(prov);
					CheckPanelLoop(i);
					CheckSelectedItemIndex(Providers[i], panel, selectedItemIndex);
				}
			}
		}
		protected void CheckPanelLoop(int index) {
			if((Providers[index] is DaysItemsProvider)) {
				int daysInMonth = DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month);
				int daysInPanel = Providers[index].LastDrawingItemIndex - Providers[index].FirstDrawingItemIndex + 1;
				PickContainer.FindPanelByItem(PickContainer.Items[index]).Loop = daysInMonth == daysInPanel;
				return;
			}
			if((Providers[index] is MonthItemsProvider)) {
				int monthsInYear = 12;
				int monthsInPanel = Providers[index].LastDrawingItemIndex - Providers[index].FirstDrawingItemIndex + 1;
				PickContainer.FindPanelByItem(PickContainer.Items[index]).Loop = monthsInYear == monthsInPanel;
				return;
			}
			PickContainer.FindPanelByItem(PickContainer.Items[index]).Loop = false;
		}
		protected void UpdateMinMaxValue(BaseCalendarItemsProvider provider) {
			int startItemIndex = GetStartItemIndex(provider);
			int lastItemIndex = GetLastItemIndex(provider);
			provider.SetMinMaxValue(startItemIndex, lastItemIndex);
		}
		protected void CheckSelectedItemIndex(IItemsProvider provider, PickItemsPanel panel, int lastSelectedItem) {
			int maxIndex = provider.LastDrawingItemIndex - provider.FirstItemIndex;
			int minIndex = provider.FirstDrawingItemIndex - provider.FirstItemIndex;
			int rightItemIndex = Math.Max(Math.Min(lastSelectedItem, maxIndex), minIndex);
			panel.SelectItem(rightItemIndex);
		}
		protected int GetStartItemIndex(BaseCalendarItemsProvider provider) {
			DateTime minDateEdit = ((DateEditTouchCalendar)this).OwnerEdit.Properties.MinValue;
			if(provider is DaysItemsProvider) {
				return minDateEdit > new DateTime(SelectedDate.Year, SelectedDate.Month, 1) ? minDateEdit.Day : 1;
			}
			if(provider is MonthItemsProvider) {
				return minDateEdit > new DateTime(SelectedDate.Year, 1, 1) ? minDateEdit.Month : 1;
			}
			if(provider is YearItemsProvider) {
				return minDateEdit.Year > 1 ? minDateEdit.Year : 1;
			}
			return -1;
		}
		protected int GetLastItemIndex(BaseCalendarItemsProvider provider) {
			DateTime maxDateEdit = ((DateEditTouchCalendar)this).OwnerEdit.Properties.MaxValue;
			if(maxDateEdit == DateTime.MinValue) maxDateEdit = DateTime.MaxValue;
			if(provider is DaysItemsProvider) {
				int days = DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month);
				return maxDateEdit < new DateTime(SelectedDate.Year, SelectedDate.Month, DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month)) ? maxDateEdit.Day : days;
			}
			if(provider is MonthItemsProvider) {
				return maxDateEdit < new DateTime(SelectedDate.Year, 12, 1) ? maxDateEdit.Month : 12;
			}
			if(provider is YearItemsProvider) {
				return maxDateEdit.Year < 9999 ? maxDateEdit.Year : 9999;
			}
			return -1;
		}
		public IItemsProvider CreateNewProvider(DateTimeMaskFormatElementEditable editableFormat) {
			if(editableFormat is DateTimeMaskFormatElement_h12) {
				HoursItemsProvider hoursItemsProvider = new HoursItemsProvider(12);
				hoursItemsProvider.StartIndex = 1;
				return hoursItemsProvider;
			}
			if(editableFormat is DateTimeMaskFormatElement_H24)
				return new HoursItemsProvider(24);
			if(editableFormat is DateTimeMaskFormatElement_d)
				return new DaysItemsProvider(31);
			if(editableFormat is DateTimeMaskFormatElement_Min)
				return new MinsItemsProvider(60 / GetMinuteIncrement());
			if(editableFormat is DateTimeMaskFormatElement_Month)
				return new MonthItemsProvider(12);
			if(editableFormat is DateTimeMaskFormatElement_s)
				return new SecondsItemsProvider(60 / GetSecondIncrement());
			if(editableFormat is DateTimeMaskFormatElement_Year)
				return new YearItemsProvider(9999);
			if(editableFormat is DateTimeMaskFormatElement_AmPm)
				return new MeridiemItemsProvider(2);
			return null;
		}
		public void SetDate() {
			for(int i = 0; i < Providers.Count; i++) {
				int value = GetValueForPanel(Providers[i]);
				PickContainer.FindPanelByItem(PickContainer.Items[i]).SelectItem(value);
			}
			Invalidate();
		}
		protected virtual int GetValueFromContainer(Type type) {
			for(int i = 0; i < Providers.Count; i++)
				if(Providers[i].GetType() == type) {
					int itemIndex = PickContainer.FindPanelByItem(PickContainer.Items[i]).SelectedItemIndex;
					if(IsHoursItemsProvider(type)) return itemIndex + ((HoursItemsProvider)Providers[i]).StartIndex;
					if(IsMinsItemsProvider(type)) return itemIndex * GetMinuteIncrement();
					if(IsSecondsItemsProvider(type)) return itemIndex * GetSecondIncrement();
					return itemIndex;
				}
			return -1;
		}
		protected virtual bool IsHoursItemsProvider(Type type) { return type == typeof(HoursItemsProvider); }
		protected virtual bool IsMinsItemsProvider(Type type) { return type == typeof(MinsItemsProvider); }
		protected virtual bool IsSecondsItemsProvider(Type type) { return type == typeof(SecondsItemsProvider); }
		public abstract bool ShowTime();
		private PickItemsPanel GetPanelFromType(Type type) {
			for(int i = 0; i < Providers.Count; i++)
				if(Providers[i].GetType() == type)
					return PickContainer.FindPanelByItem(PickContainer.Items[i]);
			return null;
		}
		protected virtual int GetValueForPanel(IItemsProvider iItemsProvider) {
			if(iItemsProvider is HoursItemsProvider) {
				if(iItemsProvider.ItemsCount == 12)
					return (SelectedDate.Hour - 1) % 12;
				return SelectedDate.Hour;
			}
			if(iItemsProvider is DaysItemsProvider)
				return SelectedDate.Day - 1;
			if(iItemsProvider is MinsItemsProvider)
				return SelectedDate.Minute / GetMinuteIncrement();
			if(iItemsProvider is MonthItemsProvider)
				return SelectedDate.Month - 1;
			if(iItemsProvider is SecondsItemsProvider)
				return SelectedDate.Second / GetMinuteIncrement();
			if(iItemsProvider is YearItemsProvider)
				return SelectedDate.Year - 1;
			if(iItemsProvider is MeridiemItemsProvider)
				return SelectedDate.Hour / 12;
			return -1;
		}
		protected DateTimeMaskManager MaskManager { get; set; }
		public abstract void UpdateMaskManager();
		protected internal IEnumerable<DateTimeMaskFormatElement> Formats {
			get {
				if(MaskManager == null)
					return null;
				return DateTimeMaskManagerHelper.GetFormatInfo(MaskManager);
			}
		}
		protected virtual void InitializeItemsControl() {
			Providers.Clear();
			TotalProviders = 0;
			if(Formats == null)
				return;
			foreach(var format in Formats) {
				MaskManager.SetInitialEditValue(SelectedDate);
				if(!format.Editable)
					continue;
				var editableFormat = format as DateTimeMaskFormatElementEditable;
				if(editableFormat == null)
					continue;
				AddNewProvider(editableFormat);
			}
			AddCustomProviders();
		}
		protected virtual void AddCustomProviders() {
		}
		List<IItemsProvider> providers;
		protected List<IItemsProvider> Providers {
			get {
				if(providers == null)
					providers = new List<IItemsProvider>();
				return providers;
			}
		}
		public void CreateProviders() {
			UpdateMaskManager();
			InitializeItemsControl();
			PickContainer.Items.Clear();
			for(int i = 0; i < Providers.Count; i++) {
				PickContainer.Items.Add(Providers[i]);
				if(Providers[i] is MeridiemItemsProvider)
					PickContainer.FindPanelByItem(PickContainer.Items[i]).Loop = false;
			}
		}
		public abstract void AddNewProvider(DateTimeMaskFormatElementEditable editableFormat);
		CalendarPickItemsContainer pickContainer;
		public CalendarPickItemsContainer PickContainer {
			get { return pickContainer; }
			set {
				if(PickContainer == value)
					return;
				CalendarPickItemsContainer prevContainer = PickContainer;
				pickContainer = value;
				OnPickContainerChanged(prevContainer, PickContainer);
			}
		}
		protected internal TouchCalendarViewInfo TouchViewInfo { get { return (TouchCalendarViewInfo)ViewInfo; } }
		protected override void OnPaint(PaintEventArgs e) {
			CheckContainer();
			base.OnPaint(e);
		}
		protected abstract void CheckContainer();
		protected internal override void LayoutChanged() {
			base.LayoutChanged();
			if(Painter is TouchCalendarPainter)
				((TouchCalendarPainter)Painter).ShouldUpdateAppearance = true;
			PickContainer.OnPropertiesChanged();
		}
		protected virtual void OnPickContainerChanged(CalendarPickItemsContainer oldContainer, CalendarPickItemsContainer newContainer) {
			if(oldContainer != null)
				oldContainer.Owner = null;
			if(newContainer != null)
				newContainer.Owner = this;
		}
		protected override void WndProc(ref Message m) {
			if(PickContainer != null && PickContainer.ProcessTouchEvents(ref m))
				return;
			base.WndProc(ref m);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			PickContainer.OnMouseDown(e);
		}
		protected override void OnMouseClick(MouseEventArgs e) {
			base.OnMouseClick(e);
			PickContainer.OnMouseClick(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			PickContainer.OnMouseUp(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			PickContainer.OnMouseMove(e);
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			PickContainer.OnMouseWheel(e);
		}
		protected internal virtual void OnMouseWheelCore(MouseEventArgs e) {
			PickContainer.OnMouseWheel(e);
		}
		#region IPickItemsClientBoundsProvider Members
		Rectangle IPickItemsClientBoundsProvider.ClientRectangle {
			get {
				return TouchViewInfo.PickContainerClientBounds;
			}
		}
		#endregion
		protected override BaseControlPainter CreatePainter() {
			return new TouchCalendarPainter();
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new TouchCalendarViewInfo(this);
		}
		#region IPickItemsClientBoundsProvider Members
		public Rectangle ContentRectangle {
			get {
				return TouchViewInfo.PickContainerContentBounds;
			}
		}
		#endregion
	}
	public class PickItemsPainter {
		public void DrawItem(GraphicsCache cache, IPickItemsContainerDrawInfo drawInfo, PickItemInfo info, string firstString, string description) {
			if(info.IsImageReadyToRendering) {
				RenderItemToBitmap(cache, drawInfo, info, firstString);
				info.IsImageReadyToRendering = false;
			}
			DrawItemRenderImage(cache, info, firstString, description);
		}
		public string ConvertIntToString(int number, int numberOfChars) {
			string format = "d" + numberOfChars.ToString();
			return number.ToString(format);
		}
		public bool ShouldDrawDescription(PickItemInfo info) {
			if(info.ItemIndex == info.Panel.SelectedItemIndex)
				return true;
			if(((info.ItemIndex - 1 + info.Panel.ItemsProvider.ItemsCount) % info.Panel.ItemsProvider.ItemsCount + info.Panel.AddedIndex == info.Panel.SelectedItemIndex) && (info.Panel.CalcDeltaOffset() < 0))
				return true;
			return ((info.ItemIndex + 1 + info.Panel.ItemsProvider.ItemsCount) % info.Panel.ItemsProvider.ItemsCount + info.Panel.AddedIndex == info.Panel.SelectedItemIndex) && (info.Panel.CalcDeltaOffset() > 0);
		}
		public float DescriptionOpacity(PickItemInfo info) {
			if(info.ItemIndex == info.Panel.SelectedItemIndex)
				return 1 - (float)Math.Abs(info.Panel.CalcDeltaOffset()) / info.Panel.Container.OuterItemSize.Height;
			return (float)Math.Abs(info.Panel.CalcDeltaOffset()) / info.Panel.Container.OuterItemSize.Height;
		}
		protected internal virtual void DrawItemImage(Graphics graphics, PickItemInfo info) {
			TouchCalendarViewInfo viewInfo = GetCalendar(info).TouchViewInfo;
			using(GraphicsCache cache = new GraphicsCache(graphics)) {
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, viewInfo.GetSelectedItemInfo(info));
			}
		}
		protected virtual TouchCalendar GetCalendar(PickItemInfo info) {
			if(info.Panel.Container.Owner is DateEditTouchCalendar)
				return ((DateEditTouchCalendar)info.Panel.Container.Owner);
			if(info.Panel.Container.Owner is TimeEditTouchCalendar)
				return ((TimeEditTouchCalendar)info.Panel.Container.Owner);
			return null;
		}
		protected internal void RenderItemToBitmap(GraphicsCache cache, IPickItemsContainerDrawInfo drawInfo, PickItemInfo info, string s) {
			using(Graphics graphics = Graphics.FromImage(info.RenderImage)) {
				graphics.Clear(drawInfo.PanelBackColor);
				DrawItemImage(graphics, info);
				AppearanceObject paintAppearance = CreateAppearance(info);
				graphics.DrawString(s, paintAppearance.Font, paintAppearance.GetForeBrush(cache), new Rectangle(Point.Empty, info.ContentBounds.Size), paintAppearance.GetStringFormat());
			}
		}
		protected internal Image DescriptionImage(GraphicsCache cache, PickItemInfo info, string s) {
			Size size = new Size(info.ContentBounds.Width, info.ContentBounds.Height / 2);
			Point location = new Point(0, info.ContentBounds.Height / 2);
			Bitmap bitmap = new Bitmap(info.ContentBounds.Width, info.ContentBounds.Height);
			using(Graphics graphics = Graphics.FromImage(bitmap)) {
				graphics.DrawImage(info.RenderImage, new Rectangle(Point.Empty, info.ContentBounds.Size));
				AppearanceObject paintAppearance = CreateDescriptionAppearance(info);
				graphics.DrawString(s, paintAppearance.Font, paintAppearance.GetForeBrush(cache), new Rectangle(location, size), paintAppearance.GetStringFormat());
			}
			return bitmap;
		}
		protected AppearanceObject CreateAppearance(PickItemInfo info) {
			if(info.Disabled) return info.Panel.PaintAppearanceInactive;
			if(info.ItemIndex == info.Panel.SelectedItemIndex && info.Panel.State == PanelState.Expand && info.Panel.ItemIsSelected)
				return info.Panel.PaintSelectedItemAppearance;
			return info.Panel.PaintAppearance;
		}
		protected AppearanceObject CreateDescriptionAppearance(PickItemInfo info) {
			if(info.Disabled) return info.Panel.PaintDescriptionAppearanceInactive;
			if(info.ItemIndex == info.Panel.SelectedItemIndex && info.Panel.State == PanelState.Expand && info.Panel.ItemIsSelected)
				return info.Panel.PaintSelectedDescriptionAppearance;
			return info.Panel.PaintDescriptionAppearance;
		}
		static ColorMatrix opacityMatrix;
		static ColorMatrix GetOpacityMatrix(float opacity) {
			if(opacityMatrix == null)
				opacityMatrix = new ColorMatrix();
			opacityMatrix.Matrix33 = opacity;
			return opacityMatrix;
		}
		protected internal virtual void DrawItemRenderImage(GraphicsCache cache, PickItemInfo info, string firstString, string description) {
			if(info.Opacity == 255) {
				cache.Graphics.DrawImage(info.RenderImage, info.ContentBounds);
				DrawDescription(cache, info, description);
			}
			else {
				ImageAttributes attr = new ImageAttributes();
				float opacity = description == string.Empty ? info.Opacity / 255.0f : CalcOpacityFirstPicture(info);
				attr.SetColorMatrix(GetOpacityMatrix(opacity));
				cache.Graphics.DrawString(firstString, info.Panel.PaintAppearanceInactive.Font, info.Panel.PaintAppearanceInactive.GetForeBrush(cache), info.ContentBounds, info.Panel.PaintAppearanceInactive.GetStringFormat());
				if(info.Opacity > 0)
					cache.Graphics.DrawImage(info.RenderImage, info.ContentBounds, 0, 0, info.RenderImage.Width, info.RenderImage.Height, GraphicsUnit.Pixel, attr);
				DrawDescription(cache, info, description);
			}
		}
		protected float CalcOpacityFirstPicture(PickItemInfo info) {
			float totalOpacity = info.Opacity / 255.0f;
			return (1 - totalOpacity) / (CalcOpacitySecondPicture(info) - 1) + 1;
		}
		protected float CalcOpacitySecondPicture(PickItemInfo info) {
			return (info.Opacity / 255.0f) * DescriptionOpacity(info) * 0.5f;
		}
		protected internal virtual void DrawDescription(GraphicsCache cache, PickItemInfo info, string description) {
			if(description != string.Empty) {
				Rectangle rect = info.ContentBounds;
				rect.Height = rect.Height / 2;
				rect.Y += rect.Height;
				ImageAttributes attr = new ImageAttributes();
				Bitmap bmp = (Bitmap)DescriptionImage(cache, info, description);
				attr.SetColorMatrix(GetOpacityMatrix(CalcOpacitySecondPicture(info)));
				cache.Graphics.DrawImage(bmp, info.ContentBounds, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attr);
			}
		}
	}
	public class TouchCalendarViewInfo : BaseStyleControlViewInfo {
		public TouchCalendarViewInfo(TouchCalendar calendar)
			: base(calendar) {
			CalcConstants();
		}
		public int IndentBetweenItems { get; private set; }
		public int IndentBetweenItemAndBorder { get; private set; }
		public int IndentBetweenItemAndBottom { get; private set; }
		public int IndentBetweenItemAndTop { get; private set; }
		public int BorderSize { get; private set; }
		public Size DefaultCellSize { get; private set; }
		public TouchCalendar Calendar { get { return (TouchCalendar)base.OwnerControl; } }
		protected ISkinProvider Provider { get { return Calendar.LookAndFeel.ActiveLookAndFeel; } }
		public Rectangle PickContainerClientBounds { get; private set; }
		public Rectangle PickContainerContentBounds { get; private set; }
		protected override void CalcConstants() {
			base.CalcConstants();
			IndentBetweenItemAndBottom = IndentBetweenItemAndBorder = IndentBetweenItemAndTop = IndentBetweenItems = BorderSize = 1;
			DefaultCellSize = CalcDefaultCellSize();
		}
		protected virtual Size CalcDefaultCellSize() {
			Size defSize = new Size(90, 70);
			SizeF scale = ScaleUtils.GetScaleFactor();
			return new Size((int)(defSize.Width * scale.Width), (int)(defSize.Height * scale.Height));
		}
		protected internal Color GetActiveItemColor() {
			return EditorsSkins.GetSkin(Provider).Properties.GetColor(EditorsSkins.SkinCalendarTodayCellColor);
		}
		protected internal Color GetInactiveItemColor() {
			return EditorsSkins.GetSkin(Provider).Properties.GetColor(EditorsSkins.SkinCalendarInactiveCellColor);
		}
		protected internal int CalcBestWidth() {
			return IndentBetweenItemAndBorder * 2 + Calendar.TotalProviders * DefaultCellSize.Width + (Calendar.TotalProviders - 1) * IndentBetweenItems;
		}
		public override Size CalcBestFit(Graphics g) {
			int height = 3 * DefaultCellSize.Height + 2 * IndentBetweenItems + IndentBetweenItemAndBorder * 2;
			int width = CalcBestWidth();
			return new Size(width, height);
		}
		public PickItemsContainer PickContainer { get { return GetCalendar().PickContainer; } }
		public virtual TouchCalendar GetCalendar() {
			if(OwnerControl is DateEditTouchCalendar)
				return ((DateEditTouchCalendar)OwnerControl);
			if(OwnerControl is TimeEditTouchCalendar)
				return ((TimeEditTouchCalendar)OwnerControl);
			return null;
		}
		protected internal SkinElementInfo GetSelectedItemInfo(PickItemInfo info) {
			SkinElement element = CommonSkins.GetSkin(Provider)[CommonSkins.SkinHighlightedItem];
			SkinElementInfo elemInfo = new SkinElementInfo(element, new Rectangle(Point.Empty, info.ContentBounds.Size));
			if(info.Panel.State == PanelState.Expand && info.ItemIndex == info.Panel.SelectedItemIndex && info.Panel.ItemIsSelected)
				elemInfo.ImageIndex = 0;
			else
				elemInfo.ImageIndex = 1;
			return elemInfo;
		}
		protected override void CalcRects() {
			base.CalcRects();
			PickContainerClientBounds = CalcPickContainerClientBounds();
			PickContainerContentBounds = CalcContentRectangle();
			PickContainer.ItemSize = DefaultCellSize;
			PickContainer.OnPropertiesChanged();
		}
		protected Rectangle CalcContentRectangle() {
			Point location = new Point(IndentBetweenItemAndBorder, IndentBetweenItemAndBorder);
			Size size = new Size(PickContainerClientBounds.Width - 2 * IndentBetweenItemAndBorder, PickContainerClientBounds.Height - 2 * IndentBetweenItemAndBorder);
			return new Rectangle(location, size);
		}
		protected Form Form { get { return Calendar.FindForm(); } }
		protected internal virtual Rectangle CalcPickContainerClientBounds() {
			if(Form == null)
				return Rectangle.Empty;
			int height = 3 * DefaultCellSize.Height + 2 * IndentBetweenItems + 2 * IndentBetweenItemAndBorder;
			return new Rectangle(Form.ClientRectangle.Location, new Size(Form.ClientRectangle.Width, height));
		}
	}
	public class PickItemsContainerDrawInfo : IPickItemsContainerDrawInfo {
		ISkinProvider provider;
		Font font, smallFont;
		public PickItemsContainerDrawInfo(ISkinProvider provider) {
			this.provider = provider;
		}
		Font Font {
			get {
				if(font == null)
					font = CreateFont(14.0f);
				return font;
			}
		}
		Font SmallFont {
			get {
				if(smallFont == null)
					smallFont = CreateFont(8.0f);
				return smallFont;
			}
		}
		Font CreateFont(float size) {
			Font defaultFont = System.Drawing.SystemFonts.DefaultFont;
			return new Font(defaultFont.FontFamily, size, defaultFont.Style);
		}
		#region IPickItemsContainerPaintInfo Members
		public Color ContainerBackColor {
			get {
				return CommonSkins.GetSkin(Provider).GetSystemColor(SystemColors.Window);
			}
		}
		public Color PanelBackColor {
			get {
				return CommonSkins.GetSkin(Provider).GetSystemColor(SystemColors.Window);
			}
		}
		public AppearanceDefault CreateDefaultAppearance() {
			return new AppearanceDefault(CalendarTodayCellColor, Color.Empty, HorzAlignment.Center, VertAlignment.Center, Font);
		}
		public AppearanceDefault CreateDescriptionAppearanceInactive() {
			return new AppearanceDefault(CalendarInactiveCellColor, Color.Empty, HorzAlignment.Center, VertAlignment.Center, SmallFont);
		}
		public AppearanceDefault CreateDefaultAppearanceInactive() {
			return new AppearanceDefault(CalendarInactiveCellColor, Color.Empty, HorzAlignment.Center, VertAlignment.Center, Font);
		}
		public AppearanceDefault CreateDescriptionAppearance() {
			return new AppearanceDefault(CalendarTodayCellColor, Color.Empty, HorzAlignment.Center, VertAlignment.Center, SmallFont);
		}
		public AppearanceDefault CreateSelectedItemAppearance() {
			return new AppearanceDefault(CalendarSelectedCellColor, Color.Empty, HorzAlignment.Center, VertAlignment.Center, Font);
		}
		public AppearanceDefault CreateSelectedDescriptionAppearance() {
			return new AppearanceDefault(CalendarSelectedCellColor, Color.Empty, HorzAlignment.Center, VertAlignment.Center, SmallFont);
		}
		ISkinProvider Provider { get { return provider; } }
		#endregion
		public Color CalendarTodayCellColor {
			get {
				return EditorsSkins.GetSkin(Provider).Colors.GetColor(EditorsSkins.SkinCalendarTodayCellColor);
			}
		}
		public Color CalendarInactiveCellColor {
			get {
				return EditorsSkins.GetSkin(Provider).Colors.GetColor(EditorsSkins.SkinCalendarInactiveCellColor);
			}
		}
		public Color CalendarSelectedCellColor {
			get {
				return EditorsSkins.GetSkin(Provider).Colors.GetColor(EditorsSkins.SkinCalendarSelectedCellColor);
			}
		}
	}
	public class TouchCalendarPainter : BaseControlPainter {
		public TouchCalendarPainter() {
			ShouldUpdateAppearance = true;
		}
		PickItemsContainerDrawInfo drawInfo;
		PickItemsContainerDrawInfo DrawInfo {
			get {
				return drawInfo;
			}
		}
		public bool ShouldUpdateAppearance { get; set; }
		protected override void DrawBorder(ControlGraphicsInfoArgs info) {
		}
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			TouchCalendarViewInfo viewInfo = (TouchCalendarViewInfo)info.ViewInfo;
			TouchCalendar Calendar = (TouchCalendar)viewInfo.PickContainer.Owner;
			RectangleF clipRectangle = info.Graphics.ClipBounds;
			info.Graphics.SetClip(viewInfo.PickContainerContentBounds);
			try {
				if(drawInfo == null)
					drawInfo = new PickItemsContainerDrawInfo(Calendar.LookAndFeel.ActiveLookAndFeel);
				viewInfo.PickContainer.Draw(info.Cache, DrawInfo, ShouldUpdateAppearance);
				ShouldUpdateAppearance = false;
			}
			finally {
				info.Graphics.SetClip(clipRectangle);
			}
		}
	}
	public class CalendarPickItemInfo : PickItemInfo {
		public CalendarPickItemInfo(PickItemsPanel panel) : base(panel) { }
		public AppearanceObject PaintDescriptionAppearance { get; set; }
	}
	public class CalendarPickItemsPanel : PickItemsPanel {
		public CalendarPickItemsPanel(PickItemsContainer container, IItemsProvider provider)
			: base(container, provider) {
		}
		public CalendarPickItemsPanel(PickItemsContainer container, IItemsProvider provider, int index)
			: base(container, provider, index) {
		}
		protected override PickItemInfo CreateItemInfo() {
			return new CalendarPickItemInfo(this);
		}
	}
	public class CalendarPickItemsContainer : PickItemsContainer {
		protected override PickItemsPanel CreatePanel(IItemsProvider itemsProvider, int index) {
			return new CalendarPickItemsPanel(this, itemsProvider, index);
		}
	}
	[ToolboxItem(false)]
	public abstract class TouchPopupForm : CustomBlobPopupForm {
		Control separatorLine = null;
		public TouchPopupForm(PopupBaseEdit ownerEdit)
			: base(ownerEdit) {
			CreateTouchCalendar();
			Controls.Add(EmbeddedControl);
			SetDateInCalendar();
			IsCancel = false;
			CreateSeparatorControl();
			CreateProviders();
			OkButton.AllowFocus = false;
		}
		protected virtual void CreateProviders() {
			TouchCalendar.CreateProviders();
		}
		public override bool AllowSizing {
			get { return false; }
			set { }
		}
		protected void CreateSeparatorControl() {
			separatorLine = new PopupSeparatorLine(OwnerEdit.LookAndFeel);
			this.Controls.Add(separatorLine);
		}
		protected override void SetupButtons() {
			base.SetupButtons();
			this.fShowOkButton = true;
			this.fCloseButtonStyle = BlobCloseButtonStyle.Caption;
		}
		protected virtual bool IsNull(object value) {
			return value == null || value == DBNull.Value || value.Equals(DateTime.MinValue);
		}
		protected override Control SeparatorControl { get { return separatorLine; } }
		public TouchCalendar TouchCalendar {
			get;
			set;
		}
		public void SetDateInCalendar() {
			SetDate(OwnerEdit.EditValue);
		}
		protected abstract void CreateTouchCalendar();
		public abstract void SetDate(object val);
		protected override void OnOkButtonClick() {
			ClosePopupInNormalMode();
		}
		protected override void OnCloseButtonClick() {
			OwnerEdit.ClosePopup(PopupCloseMode.Cancel);
		}
		protected void ClosePopupInNormalMode() {
			IsCancel = false;
			OwnerEdit.ClosePopup(PopupCloseMode.Normal);
		}
		public bool IsCancel { get; set; }
		protected override Control EmbeddedControl { get { return TouchCalendar; } }
		public override object ResultValue {
			get {
				if(TouchCalendar == null) return null;
				if(IsCancel) return OldEditValue;
				return TouchCalendar.GetDateTimeFromContainer();
			}
		}
		public override void ShowPopupForm() {
			SetDateInCalendar();
			base.ShowPopupForm();
			IsCancel = true;
		}
		protected override Size CalcFormSizeCore() {
			return CalcFormSize(CalcBestSize());
		}
		protected virtual Size CalcBestSize() {
			return TouchCalendar.CalcBestSize();
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.Enter) ClosePopupInNormalMode();
			if(TouchCalendar != null) TouchCalendar.PickContainer.OnKeyDown(e);
			base.OnKeyDown(e);
		}
	}
}
