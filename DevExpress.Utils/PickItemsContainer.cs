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

using System.Drawing;
using System.Collections;
using DevExpress.Utils.Drawing;
using System;
using DevExpress.Utils.Gesture;
using System.Windows.Forms;
using DevExpress.Utils;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Collections.Generic;
namespace DevExpress.XtraEditors {
	public interface IPickItemContainerHandler {
		bool RaiseDisableCalendarDate(DateTime date);
		DateTime GetDateFromIndex(IItemsProvider provider, int itemIndex);
		DateTime[] GetDateRangeForProvider(IItemsProvider provider, DateTime date);
	}
	public interface IItemCalculator {
		void Calculate(GraphicsInfo gInfo, PickItemInfo info, object itemData);
	}
	public interface IPickItemsContainerDrawInfo {
		Color ContainerBackColor { get; }
		Color PanelBackColor { get; }
		AppearanceDefault CreateDefaultAppearance();
		AppearanceDefault CreateDefaultAppearanceInactive();
		AppearanceDefault CreateSelectedItemAppearance();
		AppearanceDefault CreateDescriptionAppearance();
		AppearanceDefault CreateSelectedDescriptionAppearance();
		AppearanceDefault CreateDescriptionAppearanceInactive();
	}
	public interface IItemPainter {
		void Draw(GraphicsCache cache, PickItemInfo info, IPickItemsContainerDrawInfo drawInfo);
	}
	public interface IItemsProvider {
		int ItemsCount { get; }
		int MaxVisibleItemsCount { get; }
		int FirstItemIndex { get; }
		int FirstDrawingItemIndex { get; }
		int LastDrawingItemIndex { get; }
		object GetItemData(int itemIndex);
		IItemCalculator GetItemCalculator(int itemIndex);
		IItemPainter GetItemPainter(int itemIndex);
		bool IsDateProvider { get; }
	}
	public class PickItemInfo {
		int itemIndex;
		bool disabled;
		public PickItemInfo(PickItemsPanel panel) {
			Panel = panel;
			IsImageReadyToRendering = true;
			this.ItemIndex = -1;
			this.disabled = false;
		}
		public PickItemsPanel Panel { get; set; }
		public Rectangle Bounds { get; set; }
		public Rectangle ContentBounds { get; set; }
		public int ItemIndex {
			get {
				return itemIndex;
			}
			set {
				if(itemIndex == value)
					return;
				IsImageReadyToRendering = true;
				itemIndex = value;
			}
		}
		public object ItemViewInfo { get; set; }
		public int Opacity { get; set; }
		public Image RenderImage { get; internal set; }
		public bool IsImageReadyToRendering { get; set; }
		public bool Disabled {
			get { return disabled; }
			set {
				disabled = value;
				IsImageReadyToRendering = true;
			}
		}
	}
	public enum PanelState { None, Expanding, Collapsing, ForceExpanding, Expand }
	public class PickItemsPanel {
		bool isMoving;
		public bool IsMoving {
			get {
				return isMoving;
			}
			set {
				if(isMoving == value)
					return;
				if(FindItemByIndex(SelectedItemIndex) != null)
					FindItemByIndex(SelectedItemIndex).IsImageReadyToRendering = true;
				isMoving = value;
			}
		}
		bool isDragging;
		public bool IsDragging {
			get {
				return isDragging;
			}
			set {
				if(isDragging == value)
					return;
				if(FindItemByIndex(SelectedItemIndex) != null)
					FindItemByIndex(SelectedItemIndex).IsImageReadyToRendering = true;
				isDragging = value;
			}
		}
		public bool Loop { get; set; }
		public int Index { get; set; }
		public int CurrentItemsAlpha { get; set; }
		public int EndItemsAlpha { get; set; }
		public double Inertia { get; set; }
		public int NextItemIndex { get; set; }
		protected internal int SpeedMultiplier { 
			get { return speedMiltiplier; }
			set { speedMiltiplier = value; }
		}
		PanelState state = PanelState.None;
		public Rectangle Bounds { get; private set; }
		double offset, minOffset;
		public PickItemsContainer Container { get; private set; }
		public bool IsInInertia { get; set; }
		internal bool ShouldBreakAnimation { get; set; }
		public bool ItemIsSelected {
			get {
				return !IsMoving && !IsDragging;
			}
		}
		int speedMiltiplier;
		public PickItemsPanel(PickItemsContainer container, IItemsProvider provider, int index) {
			Index = index;
			ItemsProvider = provider;
			offset = 0;
			CurrentItemsAlpha = 0;
			EndItemsAlpha = 0;
			IsActive = false;
			Loop = ItemsProvider.ItemsCount > 3;
			Container = container;
			IsInInertia = false;
			NextItemIndex = -1;
			this.lastSelectedItem = -1;
			this.isMoving = false;
			this.IsDragging = false;
			this.speedMiltiplier = 0;
			ShouldBreakAnimation = false;
		}
		public PickItemsPanel(PickItemsContainer container, IItemsProvider provider)
			: this(container, provider, 0) {
		}
		public PickItemInfo FindItemByIndex(int index) {
			foreach(PickItemInfo info in Items)
				if(info.ItemIndex == index)
					return info;
			return null;
		}
		public Point GetCentralItemPosition {
			get {
				return new Point(Bounds.X, (Bounds.Height - Container.ItemSize.Height) / 2);
			}
		}
		PickItemsPanelCalculator calculator;
		protected internal PickItemsPanelCalculator Calculator {
			get {
				if(calculator == null) calculator = new PickItemsPanelCalculator(this);
				return calculator;
			}
		}
		public virtual int GetOffsetForItem() {
			return GetCentralItemPosition.Y - NextItemIndex * Container.OuterItemSize.Height;
		}
		protected internal virtual int GetNextItemIndex() {
			int itemIndex = 0;
			itemIndex = -(int)((Offset - GetCentralItemPosition.Y) / Container.OuterItemSize.Height);
			if(Loop && (Offset - GetCentralItemPosition.Y) > 0) itemIndex--;
			if(Inertia < 0) {
				if(!Loop)
					itemIndex = Math.Min(itemIndex + 1, ItemsProvider.ItemsCount - 1);
				else
					itemIndex++;
			}
			return itemIndex;
		}
		public void CheckOffset() {
			CheckOffset(false);
		}
		public void CheckOffset(bool isUp) {
			if(IsMoving) 
				return;
			SetOffset();
			if(!IsItemDisabled(SelectedItemIndex)) {
				Container.UpdateLayout();
				return;
			}
			Container.MakeMovePanel(Calculator.GetNextEnabledItemIndex(SelectedItemRealIndex, isUp), this, 0, 0);
		}
		public void SetOffset() {
			int selectedItemIndex = ((int)Offset - GetCentralItemPosition.Y - Container.OuterItemSize.Height / 2) / Container.OuterItemSize.Height;
			if(Loop && (int)Offset - GetCentralItemPosition.Y - Container.OuterItemSize.Height / 2 > 0) selectedItemIndex++;
			Offset = selectedItemIndex * Container.OuterItemSize.Height + GetCentralItemPosition.Y;
		}
		public double CalcDeltaOffset() {
			int selectedItemIndex = ((int)Offset - GetCentralItemPosition.Y - Container.OuterItemSize.Height / 2) / Container.OuterItemSize.Height;
			if(Loop && (int)Offset - GetCentralItemPosition.Y - Container.OuterItemSize.Height / 2 > 0) selectedItemIndex++;
			return Offset - (selectedItemIndex * Container.OuterItemSize.Height + GetCentralItemPosition.Y);
		}
		public PanelState State {
			get { return state; }
			set {
				if(State == value)
					return;
				if((value == PanelState.Expanding || value == PanelState.ForceExpanding) && State == PanelState.Expand)
					return;
				if(FindItemByIndex(SelectedItemIndex) != null)
					FindItemByIndex(SelectedItemIndex).IsImageReadyToRendering = true;
				state = value;
				OnPanelStateChanged();
			}
		}
		protected virtual void OnPanelStateChanged() {
			AddPanelStateAnimation();
			Container.CheckPanelsOffset();
		}
		protected virtual void AddPanelStateAnimation() {
			Container.AddPanelStateAnimation(this);
		}
		public int MaxVisibleItemsCount {
			get { return ItemsProvider.MaxVisibleItemsCount > 0 ? ItemsProvider.MaxVisibleItemsCount : Bounds.Height / Container.OuterItemSize.Height + 2; }
		}
		public IItemsProvider ItemsProvider { get; private set; }
		ItemsCollection items;
		public ItemsCollection Items {
			get {
				if(items == null)
					items = new ItemsCollection(this);
				return items;
			}
		}
		public bool IsReady { get; set; }
		public double Offset {
			get { return offset; }
			set {
				ConstrainOffset(value);
				IsReady = false;
			}
		}
		protected void ConstrainOffset(double value) {
			double maxOffset = -Container.OuterItemSize.Height * (ItemsProvider.ItemsCount - 1) + minOffset;
			if(!Loop)
				offset = Math.Min(Math.Max(maxOffset, value), minOffset);
			else {
				offset = value;
				offset = -((-offset + GetCentralItemPosition.Y) % (ItemsProvider.ItemsCount * Container.OuterItemSize.Height)) + GetCentralItemPosition.Y;
				if(offset >= minOffset + Container.OuterItemSize.Height) offset -= Container.OuterItemSize.Height * ItemsProvider.ItemsCount;
				if(offset < maxOffset) offset += Container.OuterItemSize.Height * ItemsProvider.ItemsCount;
			}
		}
		public int VisibleStartIndex {
			get {
				if(Offset > 0) return 0;
				return -(int)Offset / Container.OuterItemSize.Height;
			}
		}
		public int lastSelectedItem;
		void UpdateConst() {
			minOffset = (Bounds.Height - Container.OuterItemSize.Height + Container.IndentBetweenItems % 2) / 2;
		}
		public int SelectedItemIndex {
			get {
				if((-(int)Offset + GetCentralItemPosition.Y + Container.OuterItemSize.Height / 2) < 0)
					return ItemsProvider.ItemsCount - 1;
				return (-(int)Offset + GetCentralItemPosition.Y + Container.OuterItemSize.Height / 2) / Container.OuterItemSize.Height + AddedIndex;
			}
		}
		protected internal int SelectedItemRealIndex { get { return SelectedItemIndex - AddedIndex; } }
		protected internal int FirstItemIndex { get { return ItemsProvider.FirstDrawingItemIndex - ItemsProvider.FirstItemIndex; } }
		protected internal int LastItemIndex { get { return ItemsProvider.LastDrawingItemIndex - ItemsProvider.FirstItemIndex; } }
		public void LayoutPanel(Rectangle bounds) {
			if(IsReady)
				return;
			Bounds = bounds;			
			UpdateConst();
			CreateItems();
			LayoutItems();
			IsReady = true;
		}
		AppearanceDefault defaultAppearance;
		protected AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null)
					defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
			set {
				defaultAppearance = value;
			}
		}
		AppearanceDefault descriptionAppearance;
		protected AppearanceDefault DescriptionAppearance {
			get {
				if(descriptionAppearance == null)
					descriptionAppearance = CreateDescriptionAppearance();
				return descriptionAppearance;
			}
			set {
				descriptionAppearance = value;
			}
		}
		AppearanceDefault selectedItemAppearance;
		protected AppearanceDefault SelectedItemAppearance {
			get {
				if(selectedItemAppearance == null)
					selectedItemAppearance = CreateSelectedItemAppearance();
				return selectedItemAppearance;
			}
			set {
				selectedItemAppearance = value;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearance() {
			return new AppearanceDefault(Color.Black, Color.Empty, HorzAlignment.Center, VertAlignment.Center, new Font(new FontFamily("Times New Roman"), 16, FontStyle.Regular));
		}
		protected virtual AppearanceDefault CreateDescriptionAppearance() {
			return new AppearanceDefault(Color.Gray, Color.Empty, HorzAlignment.Center, VertAlignment.Center, new Font(new FontFamily("Times New Roman"), 16, FontStyle.Regular));
		}
		protected virtual AppearanceDefault CreateSelectedItemAppearance() {
			return new AppearanceDefault(Color.White, Color.Empty, HorzAlignment.Center, VertAlignment.Center, new Font(new FontFamily("Times New Roman"), 16, FontStyle.Regular));
		}
		AppearanceObject paintAppearance;
		public AppearanceObject PaintAppearance {
			get {
				if(paintAppearance == null)
					paintAppearance = new AppearanceObject();
				return paintAppearance;
			}
		}
		AppearanceObject paintSelectedItemAppearance;
		public AppearanceObject PaintSelectedItemAppearance {
			get {
				if(paintSelectedItemAppearance == null)
					paintSelectedItemAppearance = new AppearanceObject();
				return paintSelectedItemAppearance;
			}
		}
		AppearanceObject paintSelectedDescriptionAppearance;
		public AppearanceObject PaintSelectedDescriptionAppearance {
			get {
				if(paintSelectedDescriptionAppearance == null)
					paintSelectedDescriptionAppearance = new AppearanceObject();
				return paintSelectedDescriptionAppearance;
			}
		}
		AppearanceObject paintDescriptionAppearanceInactive;
		public AppearanceObject PaintDescriptionAppearanceInactive {
			get {
				if(paintDescriptionAppearanceInactive == null)
					paintDescriptionAppearanceInactive = new AppearanceObject();
				return paintDescriptionAppearanceInactive;
			}
		}
		AppearanceObject paintDescriptionAppearance;
		public AppearanceObject PaintDescriptionAppearance {
			get {
				if(paintDescriptionAppearance == null)
					paintDescriptionAppearance = new AppearanceObject();
				return paintDescriptionAppearance;
			}
		}
		AppearanceDefault defaultAppearanceInactive;
		protected AppearanceDefault DefaultAppearanceInactive {
			get {
				if(defaultAppearanceInactive == null)
					defaultAppearanceInactive = CreateDefaultAppearanceInactive();
				return defaultAppearanceInactive;
			}
			set {
				defaultAppearanceInactive = value;
			}
		}
		AppearanceDefault selectedDescriptionAppearance;
		protected AppearanceDefault SelectedDescriptionAppearance {
			get {
				if(selectedDescriptionAppearance == null)
					selectedDescriptionAppearance = CreateSelectedDescriptionAppearance();
				return selectedDescriptionAppearance;
			}
			set {
				selectedDescriptionAppearance = value;
			}
		}
		AppearanceObject paintAppearanceInactive;
		public AppearanceObject PaintAppearanceInactive {
			get {
				if(paintAppearanceInactive == null)
					paintAppearanceInactive = new AppearanceObject();
				return paintAppearanceInactive;
			}
		}
		AppearanceDefault descriptionAppearanceInactive;
		protected AppearanceDefault DescriptionAppearanceInactive {
			get {
				if(descriptionAppearanceInactive == null)
					DescriptionAppearanceInactive = CreateDescriptionAppearanceInactive();
				return descriptionAppearanceInactive;
			}
			set {
				descriptionAppearanceInactive = value;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearanceInactive() {
			return new AppearanceDefault(Color.Gray, Color.Empty, HorzAlignment.Center, VertAlignment.Center, new Font(new FontFamily("Times New Roman"), 16, FontStyle.Regular));
		}
		protected virtual AppearanceDefault CreateSelectedDescriptionAppearance() {
			return new AppearanceDefault(Color.Black, Color.Empty, HorzAlignment.Center, VertAlignment.Center, new Font(new FontFamily("Times New Roman"), 8, FontStyle.Regular));
		}
		protected virtual AppearanceDefault CreateDescriptionAppearanceInactive() {
			return new AppearanceDefault(Color.Gray, Color.Empty, HorzAlignment.Center, VertAlignment.Center, new Font(new FontFamily("Times New Roman"), 8, FontStyle.Regular));
		}
		protected virtual void UpdatePaintAppearance() {
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { }, DefaultAppearance);
			AppearanceHelper.Combine(PaintDescriptionAppearance, new AppearanceObject[] { }, DescriptionAppearance);
			AppearanceHelper.Combine(PaintSelectedItemAppearance, new AppearanceObject[] { }, SelectedItemAppearance);
			AppearanceHelper.Combine(PaintAppearanceInactive, new AppearanceObject[] { }, DefaultAppearanceInactive);
			AppearanceHelper.Combine(PaintSelectedDescriptionAppearance, new AppearanceObject[] { }, SelectedDescriptionAppearance);
			AppearanceHelper.Combine(PaintDescriptionAppearanceInactive, new AppearanceObject[] { }, DescriptionAppearanceInactive);
		}
		public int GetStartDrawItemIndex() {
			int startDrawItemIndex;
			int selectedItemIndex = SelectedItemIndex;
			startDrawItemIndex = -(int)Offset / Container.OuterItemSize.Height;
			if(Offset > 0) startDrawItemIndex -= 1;
			if(startDrawItemIndex + MaxVisibleItemsCount > ItemsProvider.ItemsCount)
				startDrawItemIndex = Math.Max(ItemsProvider.ItemsCount - MaxVisibleItemsCount, VisibleStartIndex);
			if(Loop) return startDrawItemIndex;
			return Math.Max(0, startDrawItemIndex);
		}
		protected GraphicsInfo GInfo { get; set; }
		protected internal bool HasActiveAlphaAnimation {
			get { return CurrentItemsAlpha != EndItemsAlpha; }
		}
		protected internal bool HasActiveInertiaAnimation {
			get { return Inertia != 0; }
		}
		protected virtual void LayoutItems() {
			int itemTopOffset = ((int)Math.Abs(Offset)) % Container.OuterItemSize.Height;
			int y = Bounds.Y + Offset < 0 ? -itemTopOffset : itemTopOffset;
			int x = Bounds.X + (Bounds.Width - Container.OuterItemSize.Width) / 2;
			if(Offset > 0) y += (int)Offset - itemTopOffset;
			y += (GetStartDrawItemIndex() - VisibleStartIndex) * Container.OuterItemSize.Height;
			for(int i = 0; i < VisibleCount; i++) {
				Items[i].Bounds = new Rectangle(new Point(x, y), Container.OuterItemSize);
				Items[i].ContentBounds = new Rectangle(new Point(x, y + (Container.IndentBetweenItems + 1) / 2), new Size(Container.OuterItemSize.Width, Container.OuterItemSize.Height - Container.IndentBetweenItems));
				UpdateItemRenderImage(Items[i]);
				IItemCalculator calc = ItemsProvider.GetItemCalculator(VisibleStartIndex + i);
				calc.Calculate(GInfo, Items[i], ItemsProvider.GetItemData(VisibleStartIndex + i));
				y += Container.OuterItemSize.Height;
			}
		}
		protected virtual void UpdateItemRenderImage(PickItemInfo info) {
			if(info.RenderImage != null) {
				if(info.RenderImage.Size == info.ContentBounds.Size)
					return;
				info.RenderImage.Dispose();
				info.RenderImage = null;
			}
			info.RenderImage = new Bitmap(info.ContentBounds.Width, info.ContentBounds.Height);
		}
		public int GetNumberOfInvalidItems() {
			if(Loop) return 0;
			int numbersOfInvalidItems = Math.Max((int)Offset / Container.OuterItemSize.Height, 0);
			if(Offset < -Container.OuterItemSize.Height * ItemsProvider.ItemsCount + GetCentralItemPosition.Y * 2)
				numbersOfInvalidItems = (-Container.OuterItemSize.Height * ItemsProvider.ItemsCount + GetCentralItemPosition.Y * 2 - (int)Offset) / Container.OuterItemSize.Height;
			return numbersOfInvalidItems;
		}
		public int GetMaxNumbersVisibleItems() {
			int firstItemOffset;
			if(!Loop) firstItemOffset = Offset > 0 ? 0 : (int)Math.Abs(Offset) % Container.OuterItemSize.Height;
			else {
				firstItemOffset = -(int)Offset % Container.OuterItemSize.Height;
				if(firstItemOffset < 0) firstItemOffset += Container.OuterItemSize.Height;
			}
			int NumbersVisibleItems = (Bounds.Height + firstItemOffset + Container.OuterItemSize.Height - 1) / (Container.OuterItemSize.Height) - GetNumberOfInvalidItems();
			if(Loop) return NumbersVisibleItems + (Offset > 0 ? 1 : 0);
			return Math.Min(NumbersVisibleItems, ItemsProvider.ItemsCount - VisibleStartIndex);
		}
		public bool IsActive;
		public int VisibleCount {
			get { return Math.Min(Math.Min(MaxVisibleItemsCount, ItemsProvider.ItemsCount), GetMaxNumbersVisibleItems()); }
		}
		public void SelectItem(int index) {
			Offset = GetCentralItemPosition.Y - (index - AddedIndex) * Container.OuterItemSize.Height;
			lastSelectedItem = SelectedItemIndex;
			LayoutPanel(Bounds);
		}
		public int GetRealItemIndex(int itemIndexFromOffset) {
			if(!Loop) 
				return itemIndexFromOffset;
			int realIndex = itemIndexFromOffset % ItemsProvider.ItemsCount;
			return realIndex >= 0 ? realIndex : ItemsProvider.ItemsCount + realIndex;
		}
		protected virtual void CreateItems() {
			if(Items.Count != VisibleCount)
				CreateItemsCore();
			UpdateItemsIndices();
		}
		private void CreateItemsCore() {
			if(Items.Count > VisibleCount)
				Items.Clear();
			for(int i = Items.Count; i < VisibleCount; i++) {
				Items.Add(CreateItemInfo());
			}
		}
		protected virtual void UpdateItemsIndices() {
			int start = GetStartDrawItemIndex();
			for(int i = 0; i < VisibleCount; i++) {
				Items[i].ItemIndex = (ItemsProvider.ItemsCount + start + i) % ItemsProvider.ItemsCount + AddedIndex;
			}
		}
		public int AddedIndex { get { return FirstItemIndex; } }
		protected virtual PickItemInfo CreateItemInfo() {
			return new PickItemInfo(this);
		}
		protected void UpdateAppearance(IPickItemsContainerDrawInfo drawInfo) {
			DefaultAppearance = drawInfo.CreateDefaultAppearance();
			DescriptionAppearance = drawInfo.CreateDescriptionAppearance();
			DefaultAppearanceInactive = drawInfo.CreateDefaultAppearanceInactive();
			SelectedItemAppearance = drawInfo.CreateSelectedItemAppearance();
			SelectedDescriptionAppearance = drawInfo.CreateSelectedDescriptionAppearance();
			DescriptionAppearanceInactive = drawInfo.CreateDescriptionAppearanceInactive();
		}
		public void OnAppearanceChanged(IPickItemsContainerDrawInfo drawInfo) {
			UpdateAppearance(drawInfo);
			UpdatePaintAppearance();
		}
		public void Draw(GraphicsCache cache , IPickItemsContainerDrawInfo drawInfo, bool shouldUpdateAppearance) {
			if(shouldUpdateAppearance) OnAppearanceChanged(drawInfo);
			if(!IsReady)
				LayoutPanel(Bounds);
			cache.FillRectangle(drawInfo.PanelBackColor, Bounds);
			int selectedItemIndex = SelectedItemIndex;
			foreach(PickItemInfo info in Items) {
				info.Disabled = IsItemDisabled(info.ItemIndex);
				IItemPainter painter = ItemsProvider.GetItemPainter(info.ItemIndex);
				if(lastSelectedItem != -1 && info.Panel.State != PanelState.Collapsing)
					info.Opacity = (info.ItemIndex == lastSelectedItem) ? 255 : CurrentItemsAlpha;
				else
					info.Opacity = (info.ItemIndex == selectedItemIndex) ? 255 : CurrentItemsAlpha;
				painter.Draw(cache, info, drawInfo);
			}
		}
		protected internal bool IsItemDisabled(int itemIndex) {
			if(!(Container.Owner is IPickItemContainerHandler) || !ItemsProvider.IsDateProvider) return false;
			IPickItemContainerHandler owner = (IPickItemContainerHandler)Container.Owner;
			DateTime date = owner.GetDateFromIndex(ItemsProvider, itemIndex);
			DateTime[] range = owner.GetDateRangeForProvider(ItemsProvider, date);
			for(DateTime dateTime = range[0]; dateTime <= range[1] && dateTime <= DateTime.MaxValue; dateTime = dateTime.AddDays(1)) {
				bool disabled = owner.RaiseDisableCalendarDate(dateTime);
				if(!disabled) return false;
				if(dateTime.Date.Equals(DateTime.MaxValue.Date)) break;
			}
			return true;
		}
	}
	public class PickItemsPanelCalculator {
		PickItemsPanel panel;
		public PickItemsPanelCalculator(PickItemsPanel panel) {
			this.panel = panel;
		}
		PickItemsPanel Panel { get { return panel; } }
		int FirstItemIndex { get { return Panel.FirstItemIndex; } }
		int LastItemIndex { get { return Panel.LastItemIndex; } }
		IItemsProvider Provider { get { return Panel.ItemsProvider; } }
		const int speedItertiaMultiplier = 1;
		protected int MaxSpeedMultiplier() {
			return (int)((Panel.Container.OuterItemSize.Height * 3 / 4) / Math.Abs(Panel.Inertia));
		}
		protected void CalcSpeedMultiplier(int currentIndex, int nextIndex) {
			currentIndex += FirstItemIndex;
			nextIndex += FirstItemIndex;
			int distance = CalcDistanceBetweenItems(currentIndex, nextIndex);
			Panel.SpeedMultiplier = Math.Min((distance + 1) * speedItertiaMultiplier, MaxSpeedMultiplier());
		}
		protected int CalcDistanceBetweenItems(int currentIndex, int nextIndex) {
			int firstIndex = FirstItemIndex;
			int lastIndex = LastItemIndex;
			if(currentIndex >= firstIndex && currentIndex <= lastIndex) {
				if(nextIndex >= firstIndex && nextIndex <= lastIndex) return Math.Abs(currentIndex - nextIndex);
			}
			int distanceToCurrent = CalcMinDistanceToItem(currentIndex, firstIndex, lastIndex);
			int distanceToNext = CalcMinDistanceToItem(nextIndex, firstIndex, lastIndex);
			return Math.Max(1, distanceToCurrent + distanceToNext + 1);
		}
		protected int CalcMinDistanceToItem(int itemIndex, int firstIndex, int lastIndex) {
			if(itemIndex >= lastIndex) return itemIndex - lastIndex;
			if(itemIndex <= firstIndex) return firstIndex - itemIndex;
			return Math.Min(lastIndex - itemIndex, itemIndex - firstIndex);
		}
		public bool ShouldEndInertia() {
			if(Panel.SpeedMultiplier == 0) {
				int nextItemIndex = Panel.GetNextItemIndex();
				int nextEnabledItemIndex = GetNextEnabledItemIndex(nextItemIndex, Panel.Inertia > 0);
				CalcSpeedMultiplier(nextEnabledItemIndex, nextItemIndex);
				Panel.NextItemIndex = GetNextEnabledItemRealIndex(nextItemIndex, Panel.Inertia > 0);
			}
			double nextOffset = Panel.Offset + Panel.Inertia * Panel.SpeedMultiplier;
			int itemOffset = Panel.GetOffsetForItem();
			if(Math.Sign(itemOffset) != Math.Sign(nextOffset) && itemOffset != 0 && nextOffset != 0)
				return false;
			if(Panel.Inertia < 0) {
				if(nextOffset > itemOffset)
					return false;
				return nextOffset <= itemOffset && nextOffset + Panel.Container.OuterItemSize.Height >= itemOffset;
			}
			if(nextOffset < itemOffset)
				return false;
			return nextOffset >= itemOffset && nextOffset - Panel.Container.OuterItemSize.Height <= itemOffset;
		}
		public int CheckIndex(int index) {
			if(index < 0)
				return LastItemIndex + 1 + index;
			if(index > LastItemIndex)
				return Provider.FirstDrawingItemIndex + index - Provider.LastDrawingItemIndex - 1;
			return index;
		}
		bool shouldInverseDirection = false;
		public int GetNextEnabledItemIndex(int nextItem, bool isUp) {
			shouldInverseDirection = false;
			int nextEnabledItemIndex = GetNextEnabledItemRealIndex(nextItem, isUp);
			if(shouldInverseDirection) isUp = !isUp;
			if(isUp && Panel.SelectedItemRealIndex < nextEnabledItemIndex) nextEnabledItemIndex -= Provider.ItemsCount;
			if(!isUp && Panel.SelectedItemRealIndex > nextEnabledItemIndex) nextEnabledItemIndex += Provider.ItemsCount;
			return nextEnabledItemIndex;
		}
		public int GetNextEnabledItemRealIndex(int itemIndex, bool isUp) {
			itemIndex += FirstItemIndex;
			if(itemIndex < FirstItemIndex) itemIndex = LastItemIndex - 1;
			if(itemIndex > LastItemIndex) itemIndex = FirstItemIndex + 1;
			if(!Panel.IsItemDisabled(itemIndex) || LastItemIndex == FirstItemIndex) return itemIndex - FirstItemIndex;
			if(isUp) return SearchPrevEnabledItem(itemIndex);
			return SearchNextEnabledItem(itemIndex);
		}
		protected int SearchPrevEnabledItem(int itemIndex) {
			int startItemIndex = itemIndex + 1;
			if(Panel.Loop || (itemIndex - 1) >= FirstItemIndex) {
				startItemIndex = (itemIndex - 1) >= FirstItemIndex ? (itemIndex - 1) : LastItemIndex;
				for(int i = startItemIndex; i != itemIndex; i = (i - 1) >= FirstItemIndex ? (i - 1) : LastItemIndex) {
					if(!Panel.IsItemDisabled(i)) return i - FirstItemIndex;
					if(!Panel.Loop && (i - 1) < FirstItemIndex) break;
				}
			}
			if(!Panel.Loop) {
				for(int i = startItemIndex; i <= LastItemIndex; i++) {
					if(!Panel.IsItemDisabled(i)) {
						Panel.Inertia = Panel.Inertia * -1;
						shouldInverseDirection = true;
						return i - FirstItemIndex;
					}
				}
			}
			return itemIndex - FirstItemIndex;
		}
		protected int SearchNextEnabledItem(int itemIndex) {
			int startItemIndex = itemIndex - 1;
			if(Panel.Loop || (itemIndex + 1) <= LastItemIndex) {
				startItemIndex = (itemIndex + 1) <= LastItemIndex ? (itemIndex + 1) : FirstItemIndex;
				for(int i = startItemIndex; i != itemIndex; i = (i + 1) <= LastItemIndex ? (i + 1) : FirstItemIndex) {
					if(!Panel.IsItemDisabled(i)) return i - FirstItemIndex;
					if(!Panel.Loop && (i + 1) > LastItemIndex) break;
				}
			}
			if(!Panel.Loop) {
				for(int i = startItemIndex; i >= FirstItemIndex; i--) {
					if(!Panel.IsItemDisabled(i)) {
						Panel.Inertia = Panel.Inertia * -1;
						shouldInverseDirection = true;
						return i - FirstItemIndex;
					}
				}
			}
			return itemIndex - FirstItemIndex;
		}
		public static int CalcMovingSpeedByItemsCount(int itemsCount){
			return 10 * itemsCount;
		}
	}
	public class PickItemsPanelCollection : CollectionBase {
		public PickItemsPanelCollection(PickItemsContainer owner) {
			Owner = owner;
		}
		public PickItemsContainer Owner { get; private set; }
		public int Add(PickItemsPanel panel) { return List.Add(panel); }
		public void Insert(int index, PickItemsPanel panel) { List.Insert(index, panel); }
		public void Remove(PickItemsPanel panel) { List.Remove(panel); }
		public bool Contains(PickItemsPanel panel) { return List.Contains(panel); }
		public PickItemsPanel this[int index] { get { return (PickItemsPanel)List[index]; } set { List[index] = value; } }
	}
	public class ItemsCollection : CollectionBase {
		public ItemsCollection(PickItemsPanel owner) {
			Owner = owner;
		}
		public PickItemsPanel Owner { get; private set; }
		public int Add(PickItemInfo item) { return List.Add(item); }
		public void Insert(int index, PickItemInfo item) { List.Insert(index, item); }
		public void Remove(PickItemInfo item) { List.Remove(item); }
		public bool Contains(PickItemInfo item) { return List.Contains(item); }
		public PickItemInfo this[int index] { get { return (PickItemInfo)List[index]; } set { List[index] = value; } }
	}
	public class ItemsProviderCollection : CollectionBase {
		public ItemsProviderCollection(PickItemsContainer owner) {
			Owner = owner;
		}
		public PickItemsContainer Owner { get; private set; }
		public int Add(IItemsProvider provider) { return List.Add(provider); }
		public void Insert(int index, IItemsProvider provider) { List.Insert(index, provider); }
		public void Remove(IItemsProvider provider) { List.Remove(provider); }
		public bool Contains(IItemsProvider provider) { return List.Contains(provider); }
		public IItemsProvider this[int index] { get { return (IItemsProvider)List[index]; } set { List[index] = value; } }
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			OnCollectionChanged();
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			OnCollectionChanged();
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			OnCollectionChanged();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			OnCollectionChanged();
		}
		protected virtual void OnCollectionChanged() {
			if(IsLockUpdate)
				return;
			if(Owner != null)
				Owner.OnItemsCollectionChanged();
		}
		int LockCount { get; set; }
		public void BeginUpdate() {
			LockCount++;
		}
		public bool IsLockUpdate {
			get { return LockCount > 0; }
		}
		public void EndUpdate() {
			if(!IsLockUpdate)
				return;
			LockCount--;
			if(!IsLockUpdate) {
				OnCollectionChanged();
			}
		}
	}
	public interface IPickItemsClientBoundsProvider {
		Rectangle ClientRectangle { get; }
		Rectangle ContentRectangle { get; }
	}
	public class PickItemsContainer : IPickItemsContainerKeyboardCommandInfo {
		PickItemsContainerKeyboardCommandManager keyboardCommandManager;
		int defineOffset;
		int distanceBetweenPanels;
		Size itemSize;
		bool IsMouseDown;
		public bool IsMouseClick;
		int startDistanceToMove;
		Point pointMouseDown;
		public PickItemsContainer() {
			this.keyboardCommandManager = new PickItemsContainerKeyboardCommandManager(this);
			this.itemSize = DefaultItemSize;
			this.distanceBetweenPanels = 1;
			this.IsMouseDown = false;
			this.IsMouseClick = true;
			this.startDistanceToMove = 5;
			this.isPanelsMovingByMouse = false;
			this.DefaultDistanceBetweenPanelAndBorder = new Point(1, 1);
			IsSpinning = false;
		}
		Control owner;
		public Control Owner {
			get { return owner; }
			set {
				if(Owner == value)
					return;
				owner = value;
				OnOwnerChanged();
			}
		}
		protected virtual void OnOwnerChanged() {
			UpdateLayout();
		}
		bool ShouldSerializeItemSize() { return ItemSize.Equals(DefaultItemSize); }
		void ResetItemSize() { ItemSize = DefaultItemSize; }
		public Size ItemSize {
			get { return itemSize; }
			set {
				if(ItemSize == value)
					return;
				itemSize = value;
				OnPropertiesChanged();
			}
		}
		int indentBetweenItems = 1;
		[DefaultValue(1)]
		public int IndentBetweenItems {
			get { return indentBetweenItems; }
			set {
				if(IndentBetweenItems == value)
					return;
				indentBetweenItems = value;
				OnPropertiesChanged();
			}
		}
		public virtual void OnPropertiesChanged() {
			foreach(PickItemsPanel panel in Panels)
				panel.IsReady = false;
			UpdateLayout();
		}
		public PickItemsPanel SelectedPanel {
			get {
				foreach(PickItemsPanel panel in Panels)
					if(panel.State == PanelState.Expand || panel.State == PanelState.Expanding || panel.State == PanelState.ForceExpanding)
						return panel;
				return null;
			}
		}
		protected bool isPanelsMovingByMouse;
		public bool IsReady {
			get {
				return !isPanelsMovingByMouse && !AnimationTimer.Enabled && !MoveTimer.Enabled && !IsSpinning && !IsDragging;
			}
		}
		protected bool IsSpinning { get; set; }
		protected bool IsDragging {
			get {
				foreach(PickItemsPanel panel in Panels)
					if(panel.IsDragging) return true;
				return false;
			}
		}
		public PickItemsPanel FindPanelByItem(IItemsProvider provider) {
			for(int i = 0; i < Items.Count; i++)
				if(Items[i] == provider) return Panels[i];
			return null;
		}
		public Size OuterItemSize {  get { return new Size(ItemSize.Width, ItemSize.Height + IndentBetweenItems); } }
		Size DefaultItemSize { get { return new Size(64, 64); } }
		[Obsolete("Use DefaultDistanceBetweenPanelAndBorder"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Point DefaultDistanceBeetweenPanelAndBorder { get { return DefaultDistanceBetweenPanelAndBorder; } set { DefaultDistanceBetweenPanelAndBorder = value; } }
		public Point DefaultDistanceBetweenPanelAndBorder { get; set; }
		public int DistanceBetweenPanels {
			get { return distanceBetweenPanels; }
			set {
				if(DistanceBetweenPanels == value)
					return;
				distanceBetweenPanels = value;
				OnPropertiesChanged();
			}
		}		
		ItemsProviderCollection items;
		public ItemsProviderCollection Items {
			get {
				if(items == null)
					items = CreateItemsCollection();
				return items;
			}
		}
		PickItemsPanelCollection panels;
		protected PickItemsPanelCollection Panels {
			get {
				if(panels == null)
					panels = CreatePanelsCollection();
				return panels;
			}
		}
		protected virtual PickItemsPanelCollection CreatePanelsCollection() {
			return new PickItemsPanelCollection(this);
		}
		protected virtual ItemsProviderCollection CreateItemsCollection() {
			return new ItemsProviderCollection(this);
		}
		protected internal virtual void OnItemsCollectionChanged() {
			UpdatePanelsCollection();
		}
		protected virtual void UpdatePanelsCollection() {
			Panels.Clear();
			for(int i = 0; i < Items.Count; i++) {
				Panels.Add(CreatePanel(Items[i], i));
			}
			if(Owner != null) UpdateLayout();
		}
		protected virtual PickItemsPanel CreatePanel(IItemsProvider itemsProvider, int index) {
			return new PickItemsPanel(this, itemsProvider, index);
		}
		static readonly double DecelerationCoefficient = 0.95;
		Timer animationTimer;
		protected Timer AnimationTimer {
			get {
				if(animationTimer == null)
					animationTimer = CreateAnimationTimer();
				return animationTimer;
			}
		}
		Timer mouseWheelTimer;
		protected Timer MouseWheelTimer {
			get {
				if(mouseWheelTimer == null)
					mouseWheelTimer = CreateMouseWheelTimer();
				return mouseWheelTimer;
			}
		}
		protected virtual Timer CreateAnimationTimer() {
			Timer timer = new Timer();
			timer.Interval = 1;
			timer.Tick += new EventHandler(OnAnimationTimer);
			return timer;
		}
		public static readonly int MouseWheelInactiveThreshold = 500;
		protected virtual Timer CreateMouseWheelTimer() {
			Timer timer = new Timer();
			timer.Interval = MouseWheelInactiveThreshold;
			timer.Tick += new EventHandler(OnMouseWheelCompleted);
			return timer;
		}
		PickItemsPanelCollection animatedPanels;
		protected PickItemsPanelCollection AnimatedPanels {
			get {
				if(animatedPanels == null)
					animatedPanels = new PickItemsPanelCollection(this);
				return animatedPanels;
			}
		}
		protected int GetAlphaKoef(PanelState state) {
			if(state == PanelState.Expanding) return 7;
			if(state == PanelState.Collapsing) return 7;
			if(state == PanelState.ForceExpanding) return 15;
			return 1;
		}
		protected int GetNewPanelAlpha(int alpha, PickItemsPanel panel) {
			alpha = alpha + Math.Sign(panel.EndItemsAlpha - panel.CurrentItemsAlpha) * GetAlphaKoef(panel.State);
			return Math.Min(Math.Max(0, alpha), 255);
		}
		protected void CalcAlpha(PickItemsPanel panel) {
			panel.CurrentItemsAlpha = GetNewPanelAlpha(panel.CurrentItemsAlpha, panel);
			if(panel.EndItemsAlpha == panel.CurrentItemsAlpha) {
				if(panel.CurrentItemsAlpha == 0) {
					panel.IsActive = false;
					panel.IsReady = false;
					panel.State = PanelState.None;
				}
				if(panel.CurrentItemsAlpha == 255) {
					panel.State = PanelState.Expand;
				}
				panel.lastSelectedItem = panel.SelectedItemIndex;
				return;
			}
		}
		static double TicksPerFrame = TimeSpan.TicksPerMillisecond * 20;
		long LastTicks { get; set; }
		protected void MovePanelToFinalPosition(PickItemsPanel panel) {
			if(panel.Calculator.ShouldEndInertia()) {
				panel.Offset = panel.GetOffsetForItem();
				panel.Inertia = 0;
				panel.IsInInertia = false;
				panel.IsDragging = false;
				panel.SpeedMultiplier = 0;
				panel.lastSelectedItem = panel.SelectedItemIndex;
				return;
			}
			double time = (DateTime.Now.Ticks - LastTicks) / TicksPerFrame;
			panel.Offset += panel.Inertia * panel.SpeedMultiplier * time;
			return;
		}
		protected bool CanDecreaseInertia(PickItemsPanel panel) { return Math.Abs(panel.Inertia) > 3; }
		protected void CalcInertia(PickItemsPanel panel) {
			if(!panel.IsInInertia)
				return;
			if(!CanDecreaseInertia(panel)) {
				MovePanelToFinalPosition(panel);
				return;
			}
			UpdateInertiaAndMovePanel(panel);
			panel.SpeedMultiplier = 0;
		}
		protected virtual void UpdateInertiaAndMovePanel(PickItemsPanel panel) {
			double time = (DateTime.Now.Ticks - LastTicks) / TicksPerFrame;
			double deltaOffset = panel.Inertia * time;
			panel.Inertia *= Math.Pow(DecelerationCoefficient, time);
			panel.Offset += deltaOffset;			
		}
		protected virtual void OnAnimationTimer(object sender, EventArgs e) {
			for(int panelIndex = 0; panelIndex < AnimatedPanels.Count; ) {
				PickItemsPanel panel = AnimatedPanels[panelIndex];
				if(panel.ShouldBreakAnimation) {
					AnimatedPanels.RemoveAt(panelIndex);
					return;
				}
				if(panel.State == PanelState.Collapsing && panel.HasActiveInertiaAnimation) {
					CalcInertia(panel);
				}
				else {
					if(panel.HasActiveAlphaAnimation)
						CalcAlpha(panel);
					if(panel.HasActiveInertiaAnimation)
						CalcInertia(panel);
				}
				if(!panel.HasActiveAlphaAnimation && !panel.HasActiveInertiaAnimation) {
					AnimatedPanels.RemoveAt(panelIndex);
				}
				else
					panelIndex++;
			}
			LastTicks = DateTime.Now.Ticks;
			if(AnimatedPanels.Count == 0) {
				AnimationTimer.Stop();
			}
			Owner.Invalidate();
		}
		protected Rectangle GetContentRectangle() {
			if(Owner is IPickItemsClientBoundsProvider)
				return ((IPickItemsClientBoundsProvider)Owner).ContentRectangle;
			return Owner.ClientRectangle;
		}
		public void UpdateLayout() {
			Rectangle rect = GetContentRectangle();
			Rectangle client = rect;
			bool isSizeChanged;
			int panelsWidth = Panels.Count * (DistanceBetweenPanels + OuterItemSize.Width) - DistanceBetweenPanels;
			for(int i = 0; i < Panels.Count; i++) {
				rect.X = client.X + (client.Width - panelsWidth - DefaultDistanceBetweenPanelAndBorder.X * 2) / 2 + (OuterItemSize.Width + DistanceBetweenPanels) * i;
				rect.Width = OuterItemSize.Width;
				isSizeChanged = rect != Panels[i].Bounds;
				Panels[i].LayoutPanel(rect);
				if(isSizeChanged)
					Panels[i].SetOffset();
			}
			Owner.Invalidate();
		}
		[Obsolete("Use MakeDynamicOffset"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void MakeDinamicOffset(double pickOffset, int yStart, int yEnd) {
			MakeDynamicOffset(pickOffset, yStart, yEnd);
		}
		public void MakeDynamicOffset(double pickOffset, int yStart, int yEnd) {
			SelectedPanel.Offset = pickOffset - yStart + yEnd;
			UpdateLayout();
		}
		bool isClickInItem;
		private int ClickedItem(Point p) {
			PickItemsPanel panel = SelectedPanel;
			if(panel.State == PanelState.None) {
				if(IsHitInItemArea(p, 0))
					return 0;
			}
			else {
				int nowSelectedItem = panel.SelectedItemRealIndex;
				int nowFirstVisibleItem = panel.GetStartDrawItemIndex();
				int nowLastVisibleItem = nowFirstVisibleItem + panel.VisibleCount - 1;
				for(int i = nowFirstVisibleItem - nowSelectedItem; i <= nowLastVisibleItem - nowSelectedItem; i++)
					if(IsHitInItemArea(p, i)) {
						isClickInItem = true;
						return i + nowSelectedItem;
					}
			}
			isClickInItem = false;
			return -1;
		}
		private bool IsHitInItemArea(Point p, int visibleIndex) {
			PickItemsPanel panel = SelectedPanel;
			if(p.X >= panel.GetCentralItemPosition.X && p.X <= panel.GetCentralItemPosition.X + OuterItemSize.Width) {
				int itemTop = panel.GetCentralItemPosition.Y + OuterItemSize.Height * visibleIndex;
				return p.Y >= itemTop && p.Y <= itemTop + OuterItemSize.Height;
			}
		return false;
		}
		protected PickItemsPanel GetPanelByPoint(Point pt) { 
			foreach(PickItemsPanel panel in Panels) { 
				if(panel.Bounds.Contains(pt)) { 
					return panel;
				}
			}
			return null;
		}
		protected int GetItemIndexByPoint(Point pt) {
			foreach(PickItemsPanel panel in Panels)
				foreach(PickItemInfo item in panel.Items)
					if(item.Bounds.Contains(pt))
						return item.ItemIndex;
			return -1;
		}
		protected bool DoMouseClick { get; set; }
		public void ActivatePanel(Point pt) {
			PickItemsPanel panel = GetPanelByPoint(pt);
			if(panel == null)
				return;
			if(SelectedPanel != panel) {
				DoMouseClick = false;
			}
			else
				DoMouseClick = true;
			if(SelectedPanel != null && SelectedPanel != panel && SelectedPanel.IsActive) {
				SelectedPanel.lastSelectedItem = (-(int)SelectedPanel.Offset + SelectedPanel.GetCentralItemPosition.Y) / OuterItemSize.Height;
				SelectedPanel.State = PanelState.Collapsing;
			}
			if(panel.State == PanelState.Expanding || panel.State == PanelState.ForceExpanding) {
				int clickedItem = GetItemIndexByPoint(pt);
				if(clickedItem == panel.SelectedItemIndex)
					panel.State = PanelState.Collapsing;
				else {
					panel.State = PanelState.ForceExpanding;
					if(isClickInItem)
						MakeMovePanel(ClickedItem(pt), panel);
				}
			}
			else panel.State = PanelState.Expanding;
		}
		protected int MovingSpeed { get; set; }
		protected int MoveStady { get; set; }
		protected int MoveEnd { get; set; }
		protected PickItemsPanel MovingPanel { get; set; }
		protected PickItemsPanel SpinningPanel { get; set; }
		Timer moveTimer;
		protected Timer MoveTimer {
			get {
				if(moveTimer == null)
					moveTimer = CreateMoveTimer();
				return moveTimer;
			}
		}
		protected virtual Timer CreateMoveTimer() {
			Timer timer = new Timer();
			timer.Interval = 1;
			timer.Tick += new EventHandler(OnMoveTimer);
			return timer;
		}
		protected int CheckClickedItemIndex(int index, PickItemsPanel panel) {
			return (index + panel.AddedIndex + panel.ItemsProvider.ItemsCount) % panel.ItemsProvider.ItemsCount;
		}
		protected internal virtual void MakeMovePanel(int clickedItem, PickItemsPanel panel) {
			if(panel.IsItemDisabled(CheckClickedItemIndex(clickedItem, panel))) return;
			MakeMovePanel(clickedItem, panel, 0, 0);
		}
		protected internal virtual void MakeMovePanel(int clickedItem, PickItemsPanel panel, int movingSpeed, int moveStady) {
			if(panel.IsInInertia || MoveTimer.Enabled)
				return;
			panel.IsMoving = true;
			MoveStady = moveStady;
			int currentToNextIndex = clickedItem - panel.SelectedItemRealIndex;
			MoveEnd = -currentToNextIndex * OuterItemSize.Height;
			if(movingSpeed == 0)
				MovingSpeed = 5 * Math.Abs(currentToNextIndex);
			else
				MovingSpeed = movingSpeed;
			if(MoveEnd < MoveStady) MovingSpeed = -MovingSpeed;
			MovingPanel = panel;
			MoveTimer.Start();
		}
		protected virtual void OnMoveTimer(object sender, EventArgs e) {
			int lastMoveStady = MoveStady;
			if(IsSpinning){
				int movingSpeed = PickItemsPanelCalculator.CalcMovingSpeedByItemsCount(CalcScrollItemsCount());
				MovingSpeed = movingSpeed * Math.Sign(MovingSpeed);
			}
			MoveStady += MovingSpeed;
			if(Math.Abs(MoveStady) > Math.Abs(MoveEnd))
				MoveStady = MoveEnd;
			if(Math.Abs(MoveStady) <= Math.Abs(MoveEnd)) {
				double newOffset = MovingPanel.Offset + MoveStady - lastMoveStady;
				MovingPanel.Offset = newOffset;
			}
			Owner.Invalidate();
			if(Math.Abs(MoveStady) == Math.Abs(MoveEnd)) {
				MoveTimer.Stop();
				MovingPanel.IsMoving = false;
				MovingPanel.CheckOffset(MovingSpeed >= 0);
			}
		}
		protected int CalcScrollItemsCount() {
			return (Math.Abs(MoveEnd - MoveStady) + ItemSize.Height - 1) / ItemSize.Height;
		}
		protected void ForceStopMoveTimer() {
			if(MovingPanel == null) return;
			MoveTimer.Stop();
			MovingPanel.IsMoving = false;
			double newOffset = MovingPanel.Offset + MoveEnd - MoveStady;
		}
		protected internal virtual void AddPanelInertiaAnimation(PickItemsPanel panel) {
			panel.IsInInertia = true;
			panel.SpeedMultiplier = 0;
			panel.NextItemIndex = -1;
			panel.ShouldBreakAnimation = false;
			AddPanelAnimation(panel);
		}
		protected virtual void AddPanelAnimation(PickItemsPanel panel) {
			if(AnimatedPanels.Contains(panel))
				return;
			AnimatedPanels.Add(panel);
			if(!AnimationTimer.Enabled) {
				AnimationTimer.Start();
				LastTicks = DateTime.Now.Ticks;
			}
		}
		protected internal void AddPanelStateAnimation(PickItemsPanel panel) {
			if(panel.State == PanelState.None)
				return;
			panel.IsActive = true;
			if(panel.State == PanelState.Collapsing)
				panel.EndItemsAlpha = 0;
			else if(panel.State == PanelState.Expanding || panel.State == PanelState.ForceExpanding)
				panel.EndItemsAlpha = 255;
			AddPanelAnimation(panel);
		}
		public void ShouldMouseClick(Point delta) {
			if(Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y) > startDistanceToMove) {
				IsMouseClick = false;
				if(SelectedPanel != null)
					SelectedPanel.IsDragging = true;
			}
		}
		public void OnMouseDown(MouseEventArgs e) {
			ResetKeyboardCommandManager();
			ActivatePanel(e.Location);
			PickItemsPanel panel = SelectedPanel;
			if(panel != null) {
				IsMouseDown = true;
				pointMouseDown = new Point(e.X, e.Y);
				defineOffset = (int)panel.Offset;
				IsMouseClick = true;
				isPanelsMovingByMouse = true;
			}
		}
		public void OnMouseMove(MouseEventArgs e) {
			if(IsMouseDown) {
				ShouldMouseClick(new Point(pointMouseDown.X - e.X, pointMouseDown.Y - e.Y));
				if(SelectedPanel != null) {
					SelectedPanel.State = PanelState.ForceExpanding;
					MakeDynamicOffset(defineOffset, pointMouseDown.Y, e.Y);
				}
			}
		}
		public void OnMouseUp(MouseEventArgs e) {
			PickItemsPanel panel = SelectedPanel;
			IsMouseDown = false;
			if(panel != null) {
				panel.CheckOffset();
				panel.IsDragging = false;
			}
			isPanelsMovingByMouse = false;
		}
		public void OnMouseClick(MouseEventArgs e) {
			ResetKeyboardCommandManager();
			if(!DoMouseClick)
				return;
			PickItemsPanel panel = SelectedPanel;
			if(!IsMouseClick || panel == null)
				return;
			int clickedItem = ClickedItem(new Point(e.X, e.Y));
			if(!isClickInItem)
				return;
			if(panel.IsActive) {
				int clickedItemIndex = (clickedItem + panel.ItemsProvider.ItemsCount) % panel.ItemsProvider.ItemsCount;
				if(clickedItemIndex != panel.SelectedItemIndex)
					MakeMovePanel(clickedItem, panel);
				else {
					panel.lastSelectedItem = (-(int)panel.Offset + panel.GetCentralItemPosition.Y) / OuterItemSize.Height;
					panel.State = PanelState.Collapsing;
				}
			}
			else {
				panel.State = PanelState.Expanding;
			}
		}
		int mouseDeltaSum = 0;
		const int wheelDelta = 120;
		public virtual void OnMouseWheel(MouseEventArgs e) {
			mouseDeltaSum += e.Delta;
			Point location = MousePositionToClient(e);
			if(GetPanelByPoint(location) != SelectedPanel) {
				AutoActivatePanel(location);
			}
			if(Math.Abs(mouseDeltaSum) < wheelDelta) return;
			int scrollItemsCount = 0;
			if(MoveTimer.Enabled) {
				ForceStopMoveTimer();
				scrollItemsCount = (Math.Abs(MoveEnd - MoveStady) + ItemSize.Height - 1) / ItemSize.Height;
			}
			OnMouseWheelCore(scrollItemsCount);
		}
		protected internal Point MousePositionToClient(MouseEventArgs e) {
			if(Owner == null) return e.Location;
			Point cursorPosition = Control.MousePosition;
			return Owner.PointToClient(cursorPosition);
		}
		protected virtual void OnMouseWheelCore(int scrollItemsCount) {
			MouseWheelTimer.Stop();
			if(SelectedPanel == null) {
				IsSpinning = false;
				mouseDeltaSum = 0;
				return;
			}
			IsSpinning = true;
			SpinningPanel = SelectedPanel;
			bool isUp = mouseDeltaSum > 0;
			int currentScrollItemsCount = Math.Abs(mouseDeltaSum) / wheelDelta;
			int sumScrollitemsCount = currentScrollItemsCount + scrollItemsCount;
			ActivateNextItemCore(isUp, sumScrollitemsCount);
			mouseDeltaSum += isUp ? -wheelDelta * currentScrollItemsCount : wheelDelta * currentScrollItemsCount;
			MouseWheelTimer.Start();
			UpdateLayout();
		}
		protected virtual void AutoActivatePanel(Point location) {
			if(MouseWheelTimer.Enabled) {
				MouseWheelTimer.Stop();
				IsSpinning = false;
				OnMouseWheelCompleted(this, EventArgs.Empty);
			}
			ActivatePanel(location);
		}
		protected virtual void OnMouseWheelCompleted(object sender, EventArgs e) {
			MouseWheelTimer.Stop();
			mouseDeltaSum = 0;
			IsSpinning = false;
			if(SpinningPanel == null) return;
			int centralItemOffset = Math.Abs((int)SpinningPanel.Offset) % OuterItemSize.Height;
			SpinningPanel.IsMoving = false;
			if(centralItemOffset == 0) {
				UpdateLayout();
			}
			MakeMovePanelCore(centralItemOffset);
		}
		protected void MakeMovePanelCore(int centralItemOffset) {
			int movingSpeed = Math.Sign(centralItemOffset - OuterItemSize.Height / 2);
			bool isUp = movingSpeed < 0;
			int movingStady = isUp ? (OuterItemSize.Height - centralItemOffset) : -centralItemOffset;
			int nextItem = isUp ? SpinningPanel.SelectedItemRealIndex - 1 : SpinningPanel.SelectedItemRealIndex + 1;
			MakeMovePanel(nextItem, SpinningPanel, 0, movingStady); 
		}
		protected internal void CheckPanelsOffset() {
			for(int i = 0; i < Panels.Count; i++) {
				PickItemsPanel panel = Panels[i];
				if(panel == SelectedPanel) continue;
				if(panel.ItemIsSelected) 
					panel.CheckOffset();
			}
		}
		public void OnKeyDown(KeyEventArgs e) {
			PickItemsContainerKeyCommandType commandType = PickItemsContainerKeyboardCommandManager.CheckKeyEvent(e);
			if(commandType != PickItemsContainerKeyCommandType.None)
				OnPerformKeyboardInput(commandType, e);
			e.Handled = true;
			if(commandType != PickItemsContainerKeyCommandType.Number && commandType != PickItemsContainerKeyCommandType.NumPadNumber) 
				ResetKeyboardCommandManager();
		}
		protected virtual void OnPerformKeyboardInput(PickItemsContainerKeyCommandType commandType, KeyEventArgs e) {
			switch(commandType) {
				case PickItemsContainerKeyCommandType.HorzShift:
					ActivateNextPanel(e.KeyCode == Keys.Left);
					break;
				case PickItemsContainerKeyCommandType.VertShift:
					ActivateNextItem(e.KeyCode == Keys.Up || e.KeyCode == Keys.PageUp);
					break;
				case PickItemsContainerKeyCommandType.Home:
					SelectFirstItem();
					break;
				case PickItemsContainerKeyCommandType.End:
					SelectLastItem();
					break;
				case PickItemsContainerKeyCommandType.Number:
					SelectItemByNumber(e.KeyCode - Keys.D0);
					break;
				case PickItemsContainerKeyCommandType.NumPadNumber:
					SelectItemByNumber(e.KeyCode - Keys.NumPad0);
					break;
			}
		}
		protected internal void ResetKeyboardCommandManager() {
			keyboardCommandManager.Clear();
		}
		protected virtual void SelectItemByNumber(int number){
			if(SelectedPanel == null) return;
			if(!keyboardCommandManager.AddChar((char)(number + '0'))) return;
			SelectedPanel.IsMoving = true;
			int selectedItemIndex = Math.Max(keyboardCommandManager.GetValue() - SelectedPanel.ItemsProvider.FirstItemIndex, 0);
			SelectedPanel.SelectItem(selectedItemIndex);
			UpdateLayout();
			SelectedPanel.IsMoving = false;
			if(keyboardCommandManager.IsReady()) {
				ResetKeyboardCommandManager();
				if(SelectedPanel.Index < Panels.Count - 1) {
					ActivateNextPanel(false);
				}
			}
		}
		protected void SelectFirstItem() {
			SelectItemCore(SelectedPanel.GetCentralItemPosition.Y);		
		}
		protected void SelectLastItem() {
			int offset = SelectedPanel.GetCentralItemPosition.Y - (SelectedPanel.ItemsProvider.ItemsCount - 1) * OuterItemSize.Height;
			SelectItemCore(offset);		
		}
		protected void SelectItemCore(int offset) {
			if(SelectedPanel == null) return;
			SelectedPanel.IsMoving = true;
			SelectedPanel.Offset = offset;
			UpdateLayout();
			SelectedPanel.IsMoving = false;
		}
		public virtual void Draw(GraphicsCache e, IPickItemsContainerDrawInfo drawInfo, bool shouldUpdateAppearance) {
			e.FillRectangle(drawInfo.ContainerBackColor, GetContentRectangle());
			foreach(PickItemsPanel panel in Panels)
				panel.Draw(e, drawInfo, shouldUpdateAppearance);
		}
		public bool ProcessTouchEvents(ref Message m){
			return GestureHelper.WndProc(ref m);
		}
		PickControlHandler handler;
		protected PickControlHandler Handler {
			get {
				if(handler != null)
					handler = CreateHandler();
				return handler;
			}
			set {
				if(Handler == value)
					return;
				handler = value;
			}
		}
		GestureHelper gestureHelper;
		protected internal GestureHelper GestureHelper {
			get {
				if(gestureHelper == null) {
					Handler = new PickControlHandler(this);
					gestureHelper = new GestureHelper(Handler);
					gestureHelper.PanWithGutter = false;
				}
				return gestureHelper;
			}
		}
		protected virtual PickControlHandler CreateHandler() {
			return new PickControlHandler(this);
		}
		public static readonly int DefaultMovingSpeed = 10;
		protected virtual void ActivateNextItem(bool isUp) {
			ActivateNextItemCore(isUp, 1);
		}
		protected virtual void ActivateNextItemCore(bool isUp, int scrollItemsCount) {
			if(!CanActivateNextItem(isUp)) return;
			int nextItemIndex = SelectedPanel.Calculator.CheckIndex(SelectedPanel.SelectedItemRealIndex + (isUp ? -scrollItemsCount : scrollItemsCount));
			int nextEnabledItemIndex = SelectedPanel.Calculator.GetNextEnabledItemIndex(nextItemIndex, isUp);
			MakeMovePanel(nextEnabledItemIndex, SelectedPanel, DefaultMovingSpeed * scrollItemsCount, 0);
		}
		protected virtual bool CanActivateNextItem(bool isUp) {
			if(SelectedPanel == null) return false;
			if(!SelectedPanel.Loop) {
				if(SelectedPanel.SelectedItemIndex == SelectedPanel.Items.Count - 1 && !isUp) return false;
				if(SelectedPanel.SelectedItemIndex == 0 && isUp) return false;
			}
			return true;
		}
		protected virtual void ActivateNextPanel(bool isLeft) {
			if(!CanActivateNextPanel()) return;
			if(SelectedPanel == null)
				Panels[0].State = PanelState.Expanding;
			else {
				if(Items.Count == 1) return;
				int nextPanelIndex = (Items.Count + SelectedPanel.Index + (isLeft ? -1 : 1)) % Items.Count;
				SelectedPanel.State = PanelState.Collapsing;
				Panels[nextPanelIndex].State = PanelState.Expanding;
			}
		}
		protected virtual bool CanActivateNextPanel() {
			return Items.Count > 0;
		}
		#region IKeyCharCommandInfo Members
		public int FirstItemIndex {
			get {
				if(SelectedPanel == null) return -1;
				return SelectedPanel.ItemsProvider.FirstItemIndex;
			}
		}
		public int ItemsCount {
			get {
				if(SelectedPanel == null) return -1;
				return SelectedPanel.ItemsProvider.ItemsCount;
			}
		}
		#endregion
	}
	public class PickControlHandler : IGestureClient {
		public double PickControlOffset;
		public PickControlHandler(PickItemsContainer container) {
			Container = container;
		}
		public PickItemsContainer Container { get; private set; }
		public Control Owner { get { return Container.Owner; } }
		#region IGestureClient Members
		public GestureAllowArgs[] CheckAllowGestures(Point point) {
			GestureAllowArgs p = new GestureAllowArgs() { GID = GID.PAN, AllowID = GestureHelper.GC_PAN_WITH_GUTTER | GestureHelper.GC_PAN_WITH_SINGLE_FINGER_VERTICALLY, BlockID = GestureHelper.GC_PAN_WITH_INERTIA };
			return new GestureAllowArgs[] { p, GestureAllowArgs.PressAndTap };
		}
		public IntPtr Handle {
			get { return Owner.IsHandleCreated ? Owner.Handle : IntPtr.Zero; }
		}
		public void OnBegin(GestureArgs info) {
			Container.ResetKeyboardCommandManager();
		}
		public void OnEnd(GestureArgs info) { }
		int prevTopIncrement = 0;
		public void OnPan(GestureArgs info, Point delta, ref Point overPan) {
			if(info.IsBegin) {
				Container.ActivatePanel(info.Start.Point);
				if(Container.SelectedPanel == null) 
					return;
				Container.SelectedPanel.ShouldBreakAnimation = true;
				Container.SelectedPanel.IsDragging = true;
				PickControlOffset = Container.SelectedPanel.Offset;
				return;
			}
			if(Container.SelectedPanel == null) 
				return;
			if(info.IsEnd) {
				if(ShouldRunInertiaAnimation)
					RunInertiaAnimation(Container.SelectedPanel);
				else {
					Container.SelectedPanel.CheckOffset();
					Container.SelectedPanel.IsDragging = false;
				}
				return;
			}
			this.prevTopIncrement = delta.Y;
			if(delta.Y != 0) {
				Container.SelectedPanel.Inertia = delta.Y;
			}
			Container.SelectedPanel.State = PanelState.ForceExpanding;
			Container.MakeDynamicOffset(PickControlOffset, info.Start.Y, info.Current.Y);
		}
		bool ShouldRunInertiaAnimation { get { return this.prevTopIncrement != 0; } }
		protected virtual void RunInertiaAnimation(PickItemsPanel panel) {
			Container.AddPanelInertiaAnimation(panel);
		}
		public void OnPressAndTap(GestureArgs info) {
		}
		public void OnRotate(GestureArgs info, Point center, double degreeDelta) {
		}
		public void OnTwoFingerTap(GestureArgs info) {
		}
		public void OnZoom(GestureArgs info, Point center, double zoomDelta) {
		}
		public IntPtr OverPanWindowHandle {
			get { return GestureHelper.FindOverpanWindow(Owner); }
		}
		public Point PointToClient(Point p) {
			return Owner.PointToClient(p);
		}
		#endregion
	}
	public interface IPickItemsContainerKeyboardCommandInfo {
		int FirstItemIndex { get; }
		int ItemsCount { get; }
	}
	public class PickItemsContainerKeyboardCommandManager {
		Queue<char> queue;
		IPickItemsContainerKeyboardCommandInfo container;
		PickItemsContainerKeyboardCommandManagerState state;
		public PickItemsContainerKeyboardCommandManager(IPickItemsContainerKeyboardCommandInfo container) {
			this.queue = new Queue<char>();
			this.container = container;
			state = PickItemsContainerKeyboardCommandManagerState.Read;
		}
		public bool IsReady() {
			return state == PickItemsContainerKeyboardCommandManagerState.Ready;
		}
		public bool AddChar(char c) {
			queue.Enqueue(c);
			return CheckQueue();
		}
		public void Clear() {
			queue.Clear();
			state = PickItemsContainerKeyboardCommandManagerState.Read;
		}
		public int GetValue() {
			if(queue.Count == 0) return 0;
			return int.Parse(new string(queue.ToArray()));
		}
		protected bool CheckQueue() {
			int lastItemIndex = Container.FirstItemIndex + Container.ItemsCount - 1;
			int maxSymbolsCount = lastItemIndex.ToString().Length;
			int maxSelectedValue = (GetValue() + 1) * (int)Math.Pow(10, maxSymbolsCount - queue.Count) - 1;
			if(maxSelectedValue < Container.FirstItemIndex) {
				Clear();
				return false;
			}
			if(GetValue() > lastItemIndex) queue.Dequeue();
			if(GetValue() > lastItemIndex) queue.Dequeue();
			if(GetValue() * 10 > lastItemIndex || queue.Count + 1 > maxSymbolsCount)
				state = PickItemsContainerKeyboardCommandManagerState.Ready;
			return true;
		}
		public static PickItemsContainerKeyCommandType CheckKeyEvent(KeyEventArgs e) {
			if(e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
				return PickItemsContainerKeyCommandType.HorzShift;
			if(e.KeyCode == Keys.Down || e.KeyCode == Keys.Up || e.KeyCode == Keys.PageDown || e.KeyCode == Keys.PageUp)
				return PickItemsContainerKeyCommandType.VertShift;
			if(e.KeyCode == Keys.Home) return PickItemsContainerKeyCommandType.Home;
			if(e.KeyCode == Keys.End) return PickItemsContainerKeyCommandType.End;
			if(e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
				return PickItemsContainerKeyCommandType.Number;
			if(e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)
				return PickItemsContainerKeyCommandType.NumPadNumber;
			return PickItemsContainerKeyCommandType.None;
		}
		public IPickItemsContainerKeyboardCommandInfo Container { get { return container; } }
		#region States
		enum PickItemsContainerKeyboardCommandManagerState { Read, Ready }
		#endregion
	}
	public enum PickItemsContainerKeyCommandType { HorzShift, Home, End, VertShift, Number, NumPadNumber, None }
}
