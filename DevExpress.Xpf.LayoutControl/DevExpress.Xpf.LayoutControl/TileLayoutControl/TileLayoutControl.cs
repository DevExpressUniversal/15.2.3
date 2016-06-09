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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.LayoutControl {
	public class TileClickEventArgs : EventArgs {
		public TileClickEventArgs(Tile tile) {
			Tile = tile;
		}
		public Tile Tile { get; private set; }
	}
	public interface ITileLayoutControl : IFlowLayoutControl {
		void OnTileSizeChanged(ITile tile);
		void StopGroupHeaderEditing();
		void TileClick(ITile tile);
	}
	[StyleTypedProperty(Property = "GroupHeaderStyle", StyleTargetType = typeof(TileGroupHeader))]
	[DXToolboxBrowsable]
	public class TileLayoutControl : FlowLayoutControl, ITileLayoutControl {
		public static readonly Brush DefaultBackground = new SolidColorBrush(Colors.Transparent);
		public const double DefaultGroupHeaderSpace = 11;
		public new const double DefaultItemSpace = 10;
		public new const double DefaultLayerSpace = 70;
		public static readonly new Thickness DefaultPadding = new Thickness(120, 110, 120, 110);
		#region Dependency Properties
		public static readonly DependencyProperty AllowGroupHeaderEditingProperty =
			DependencyProperty.Register("AllowGroupHeaderEditing", typeof(bool), typeof(TileLayoutControl),
				new PropertyMetadata((o, e) => ((TileLayoutControl)o).OnAllowGroupHeaderEditingChanged()));
		public static readonly DependencyProperty GroupHeaderProperty =
			DependencyProperty.RegisterAttached("GroupHeader", typeof(object), typeof(TileLayoutControl),
				new PropertyMetadata(OnAttachedPropertyChanged));
		public static readonly DependencyProperty GroupHeaderSpaceProperty =
			DependencyProperty.Register("GroupHeaderSpace", typeof(double), typeof(TileLayoutControl),
				new PropertyMetadata(DefaultGroupHeaderSpace, (o, e) => ((TileLayoutControl)o).OnGroupHeaderSpaceChanged()));
		public static readonly DependencyProperty GroupHeaderStyleProperty =
			DependencyProperty.Register("GroupHeaderStyle", typeof(Style), typeof(TileLayoutControl),
				new PropertyMetadata((o, e) => ((TileLayoutControl)o).GroupHeaders.ItemStyle = (Style)e.NewValue));
		public static readonly DependencyProperty GroupHeaderTemplateProperty =
			DependencyProperty.Register("GroupHeaderTemplate", typeof(DataTemplate), typeof(TileLayoutControl),
				new PropertyMetadata((o, e) => ((TileLayoutControl)o).GroupHeaders.ItemContentTemplate = (DataTemplate)e.NewValue));
		public static readonly DependencyProperty TileClickCommandProperty =
			DependencyProperty.Register("TileClickCommand", typeof(ICommand), typeof(TileLayoutControl), null);
#if SILVERLIGHT
		private static readonly DependencyProperty BackgroundListener = RegisterPropertyListener("Background");
#endif
		public static object GetGroupHeader(UIElement element) {
			return element.GetValue(GroupHeaderProperty);
		}
		public static void SetGroupHeader(UIElement element, object value) {
			element.SetValue(GroupHeaderProperty, value);
		}
		#endregion Dependency Properties
#if !SILVERLIGHT
		static TileLayoutControl() {
			DevExpress.Mvvm.UI.ViewInjection.StrategyManager.Default.RegisterStrategy
				<TileLayoutControl, DevExpress.Mvvm.UI.ViewInjection.TileLayoutControlStrategy>();
			AllowAddFlowBreaksDuringItemMovingProperty.OverrideMetadata(typeof(TileLayoutControl), new PropertyMetadata(true));
			AllowItemMovingProperty.OverrideMetadata(typeof(TileLayoutControl), new PropertyMetadata(true));
			AnimateItemMovingProperty.OverrideMetadata(typeof(TileLayoutControl), new PropertyMetadata(true));
			BackgroundProperty.OverrideMetadata(typeof(TileLayoutControl), new FrameworkPropertyMetadata(DefaultBackground));
			ItemSpaceProperty.OverrideMetadata(typeof(TileLayoutControl), new PropertyMetadata(DefaultItemSpace));
			LayerSpaceProperty.OverrideMetadata(typeof(TileLayoutControl), new PropertyMetadata(DefaultLayerSpace));
			PaddingProperty.OverrideMetadata(typeof(TileLayoutControl), new PropertyMetadata(DefaultPadding));
		}
#endif
		public TileLayoutControl() {
			GroupHeaders = new TileGroupHeaders(this);
#if SILVERLIGHT
			this.EnsureDefaultValue(BackgroundProperty, DefaultBackground, false);
			AttachPropertyListener("Background", BackgroundListener);
#endif
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("TileLayoutControlAllowGroupHeaderEditing")]
#endif
		public bool AllowGroupHeaderEditing {
			get { return (bool)GetValue(AllowGroupHeaderEditingProperty); }
			set { SetValue(AllowGroupHeaderEditingProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("TileLayoutControlGroupHeaderSpace")]
#endif
		public double GroupHeaderSpace {
			get { return (double)GetValue(GroupHeaderSpaceProperty); }
			set { SetValue(GroupHeaderSpaceProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("TileLayoutControlGroupHeaderStyle")]
#endif
		public Style GroupHeaderStyle {
			get { return (Style)GetValue(GroupHeaderStyleProperty); }
			set { SetValue(GroupHeaderStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("TileLayoutControlGroupHeaderTemplate")]
#endif
		public DataTemplate GroupHeaderTemplate {
			get { return (DataTemplate)GetValue(GroupHeaderTemplateProperty); }
			set { SetValue(GroupHeaderTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("TileLayoutControlTileClickCommand")]
#endif
		public ICommand TileClickCommand {
			get { return (ICommand)GetValue(TileClickCommandProperty); }
			set { SetValue(TileClickCommandProperty, value); }
		}
		public event EventHandler<TileClickEventArgs> TileClick;
		#region Children
		protected override FrameworkElement CreateItem() {
			return new Tile();
		}
		protected override void InitItem(FrameworkElement item) {
			item.SetBinding(Tile.ContentProperty, new Binding());
		}
		protected override bool IsTempChild(UIElement child) {
			return base.IsTempChild(child) || GroupHeaders.IsItem(child);
		}
		#endregion Children
		#region Layout
		protected override Size OnArrange(Rect bounds) {
			GroupHeaders.MarkItemsAsUnused();
			Size result = base.OnArrange(bounds);
			GroupHeaders.DeleteUnusedItems();
			return result;
		}
		protected override LayoutProviderBase CreateLayoutProvider() {
			return new TileLayoutProvider(this);
		}
		protected override LayoutParametersBase CreateLayoutProviderParameters() {
			return new TileLayoutParameters(ItemSpace, LayerSpace, GroupHeaderSpace, BreakFlowToFit, GroupHeaders);
		}
		#endregion Layout
		#region XML Storage
		protected override void ReadCustomizablePropertiesFromXML(FrameworkElement element, XmlReader xml) {
			base.ReadCustomizablePropertiesFromXML(element, xml);
			element.ReadPropertyFromXML(xml, GroupHeaderProperty, "GroupHeader", typeof(object));
		}
		protected override void WriteCustomizablePropertiesToXML(FrameworkElement element, XmlWriter xml) {
			base.WriteCustomizablePropertiesToXML(element, xml);
			element.WritePropertyToXML(xml, GroupHeaderProperty, "GroupHeader");
		}
		#endregion XML Storage
		protected override PanelControllerBase CreateController() {
			return new TileLayoutController(this);
		}
#if SILVERLIGHT
		protected override bool GetDefaultAllowAddFlowBreaksDuringItemMoving() {
			return true;
		}
		protected override bool GetDefaultAllowItemMoving() {
			return true;
		}
		protected override bool GetDefaultAnimateItemMoving() {
			return true;
		}
		protected override double GetDefaultItemSpace() {
			return DefaultItemSpace;
		}
		protected override double GetDefaultLayerSpace() {
			return DefaultLayerSpace;
		}
		protected override Thickness GetDefaultPadding() {
			return DefaultPadding;
		}
		protected override void OnPropertyChanged(DependencyProperty propertyListener, object oldValue, object newValue) {
			base.OnPropertyChanged(propertyListener, oldValue, newValue);
			if (propertyListener == BackgroundListener)
				this.EnsureDefaultValue(BackgroundProperty, DefaultBackground, true);
		}
#endif
		protected virtual void OnAllowGroupHeaderEditingChanged() {
#if SILVERLIGHT
			if (this.IsInDesignTool())
				return;
#endif
			GroupHeaders.AreEditable = AllowGroupHeaderEditing;
		}
		protected override void OnAttachedPropertyChanged(FrameworkElement child, DependencyProperty property, object oldValue, object newValue) {
			base.OnAttachedPropertyChanged(child, property, oldValue, newValue);
			if (property == GroupHeaderProperty)
				OnGroupHeaderChanged(child);
		}
		protected virtual void OnGroupHeaderChanged(FrameworkElement child) {
			InvalidateArrange();
		}
		protected virtual void OnGroupHeaderSpaceChanged() {
			InvalidateArrange();
		}
		protected virtual void OnTileClick(Tile tile) {
			if (TileClick != null)
				TileClick(this, new TileClickEventArgs(tile));
			if (TileClickCommand != null && TileClickCommand.CanExecute(tile))
				TileClickCommand.Execute(tile);
		}
		protected TileGroupHeaders GroupHeaders { get; private set; }
		#region ITileLayoutControl
		void ITileLayoutControl.OnTileSizeChanged(ITile tile) {
			InvalidateMeasure();
		}
		void ITileLayoutControl.StopGroupHeaderEditing() {
			GroupHeaders.StopEditing();
		}
		void ITileLayoutControl.TileClick(ITile tile) {
			if (tile is Tile)
				OnTileClick((Tile)tile);
		}
		#endregion ITileLayoutControl
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new DevExpress.Xpf.LayoutControl.UIAutomation.TileLayoutControlAutomationPeer(this);
		}
		#endregion
	}
	public enum TileGroupHeaderState { NonEditable, Editable, Editing }
	[TemplatePart(Name = EditorElementName, Type = typeof(TextBox))]
	[TemplateVisualState(GroupName = EditingStates, Name = NonEditableState)]
	[TemplateVisualState(GroupName = EditingStates, Name = EditableState)]
	[TemplateVisualState(GroupName = EditingStates, Name = EditingState)]
	public class TileGroupHeader : ContentControlBase {
		public TileGroupHeader() {
			DefaultStyleKey = typeof(TileGroupHeader);
		}
		private TileGroupHeaderState _State;
		public TileGroupHeaderState State {
			get { return _State; }
			set {
				if (State == value)
					return;
				_State = value;
				OnStateChanged();
			}
		}
		#region Template
		const string EditorElementName = "EditorElement";
		const string EditingStates = "EditingStates";
		const string NonEditableState = "NonEditable";
		const string EditableState = "Editable";
		const string EditingState = "Editing";
		public override void OnApplyTemplate() {
			if (EditorElement != null) {
				EditorElement.GotFocus -= OnEditorElementGotFocus;
				EditorElement.KeyDown -= OnEditorElementKeyDown;
				EditorElement.LostFocus -= OnEditorElementLostFocus;
			}
			base.OnApplyTemplate();
			EditorElement = GetTemplateChild(EditorElementName) as TextBox;
			if (EditorElement != null) {
				EditorElement.GotFocus += OnEditorElementGotFocus;
				EditorElement.KeyDown += OnEditorElementKeyDown;
				EditorElement.LostFocus += OnEditorElementLostFocus;
			}
			UpdateEditor();
		}
		protected TextBox EditorElement { get; private set; }
		private string GetStateName(TileGroupHeaderState state) {
			switch (state) {
				case TileGroupHeaderState.NonEditable:
					return NonEditableState;
				case TileGroupHeaderState.Editable:
					return EditableState;
				case TileGroupHeaderState.Editing:
					return EditingState;
				default:
					return null;
			}
		}
		private void OnEditorElementGotFocus(object sender, RoutedEventArgs e) {
			State = TileGroupHeaderState.Editing;
		}
		private void OnEditorElementKeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter || e.Key == Key.Escape)
				StopEditing(e.Key == Key.Enter);
		}
		private void OnEditorElementLostFocus(object sender, RoutedEventArgs e) {
			StopEditing(true);
		}
		internal void StopEditing(bool accept) {
			if (State != TileGroupHeaderState.Editing)
				return;
			if (accept)
				Content = EditorElement.Text == "" ? null : EditorElement.Text;
			State = TileGroupHeaderState.Editable;
			EditorElement.Select(0, 0);
#if SILVERLIGHT
			object focusedElement = FocusManager.GetFocusedElement();
			if (focusedElement == EditorElement)
				this.GetRootVisual().Focus();
#else
			DependencyObject focusScope = FocusManager.GetFocusScope(this);
			object focusedElement = FocusManager.GetFocusedElement(focusScope);
			if (focusedElement == EditorElement) {
				FocusManager.SetFocusedElement(focusScope, null);
				Keyboard.ClearFocus();
			}
#endif
		}
		private void UpdateEditor() {
			if (EditorElement == null || State == TileGroupHeaderState.NonEditable)
				return;
			var text = Content as string;
			if (string.IsNullOrEmpty(text))
				EditorElement.Text = State == TileGroupHeaderState.Editable ? LocalizationRes.TileLayoutControl_GroupHeader_Empty : "";
			else
				EditorElement.Text = text;
		}
		#endregion Template
		protected override void OnContentChanged(object oldValue, object newValue) {
			base.OnContentChanged(oldValue, newValue);
			UpdateEditor();
		}
		protected virtual void OnStateChanged() {
			UpdateState(false);
			UpdateEditor();
		}
		protected override void UpdateState(bool useTransitions) {
			base.UpdateState(useTransitions);
			GoToState(GetStateName(State), useTransitions);
		}
#if !SILVERLIGHT
		protected override bool IsContentInLogicalTree { get { return false; } }
#endif
	}
	public class TileGroupHeaders : ElementPool<TileGroupHeader> {
		private bool _AreEditable;
		private DataTemplate _ItemContentTemplate;
		public TileGroupHeaders(Panel container) : base(container) { }
		public void StopEditing() {
			if (!AreEditable)
				return;
			foreach (TileGroupHeader item in Items)
				item.StopEditing(true);
		}
		public bool AreEditable {
			get { return _AreEditable; }
			set {
				if (AreEditable == value)
					return;
				StopEditing();
				_AreEditable = value;
				Items.ForEach(UpdateItemState);
			}
		}
		public DataTemplate ItemContentTemplate {
			get { return _ItemContentTemplate; }
			set {
				if (ItemContentTemplate == value)
					return;
				_ItemContentTemplate = value;
				OnItemContentTemplateChanged();
			}
		}
		protected override TileGroupHeader CreateItem() {
			TileGroupHeader result = base.CreateItem();
			if (ItemContentTemplate != null)
				UpdateItemContentTemplate(result);
			UpdateItemState(result);
			return result;
		}
		protected virtual void OnItemContentTemplateChanged() {
			Items.ForEach(UpdateItemContentTemplate);
		}
		private void UpdateItemContentTemplate(TileGroupHeader item) {
			item.SetValueIfNotDefault(ContentControlBase.ContentTemplateProperty, ItemContentTemplate);
		}
		private void UpdateItemState(TileGroupHeader item) {
			item.State = AreEditable ? TileGroupHeaderState.Editable : TileGroupHeaderState.NonEditable;
		}
	}
	public class TileLayoutParameters : FlowLayoutParameters {
		public TileLayoutParameters(double itemSpace, double layerSpace, double groupHeaderSpace, bool breakFlowToFit, TileGroupHeaders groupHeaders) :
			base(itemSpace, layerSpace, breakFlowToFit, false, false, null) {
			GroupHeaders = groupHeaders;
			GroupHeaderSpace = groupHeaderSpace;
		}
		public TileGroupHeaders GroupHeaders { get; private set; }
		public double GroupHeaderSpace { get; private set; }
	}
	public class TileLayoutProvider : FlowLayoutProvider {
		public TileLayoutProvider(IFlowLayoutModel model) : base(model) { }
		public override double GetLayerSpace(bool isHardFlowBreak) {
			return isHardFlowBreak ? base.GetLayerSpace(isHardFlowBreak) : Parameters.ItemSpace;
		}
		public virtual bool ShowGroupHeaders {
			get { return Model.MaximizedElement == null && Orientation == Orientation.Vertical; }
		}
		protected override Size OnArrange(FrameworkElements items, Rect bounds, Rect viewPortBounds) {
			Size result = base.OnArrange(items, bounds, viewPortBounds);
			if (ShowGroupHeaders)
				AddGroupHeaders();
			return result;
		}
		protected override void CalculateLayoutForSlotWithMultipleItems(FrameworkElements items, ref int itemIndex,
			FlowLayoutItemPosition slotPosition, ref FlowLayoutItemSize slotSize, MeasureItem measureItem, ArrangeItem arrangeItem) {
			List<int> itemPositionsInSlot = GetItemPositionsInSlot(items, itemIndex);
			FlowLayoutItemPosition halfSlotPosition = slotPosition;
			FlowLayoutItemSize halfSlotSize = slotSize;
			FlowLayoutItemPosition prevItemPosition = slotPosition;
			FlowLayoutItemSize prevItemSize = slotSize;
			for (int i = itemIndex; i < itemIndex + itemPositionsInSlot.Count; i++) {
				FrameworkElement item = items[i];
				FlowLayoutItemSize itemSize = i == itemIndex ? prevItemSize : GetItemSize(measureItem(item));
				FlowLayoutItemPosition itemPosition = prevItemPosition;
				switch (itemPositionsInSlot[i - itemIndex]) {
					case 1:
					case 3:
					case 5:
					case 7:
						itemPosition.LayerOffset += prevItemSize.Width + Parameters.ItemSpace;
						break;
					case 2:
					case 6:
						itemPosition.LayerOffset = halfSlotPosition.LayerOffset;
						itemPosition.ItemOffset += prevItemSize.Length + Parameters.ItemSpace;
						break;
					case 4:
						halfSlotPosition.LayerOffset += GetHalfSlotMaxWidth(items[itemIndex], slotSize) + Parameters.ItemSpace;
						itemPosition = halfSlotPosition;
						slotSize.Length = halfSlotSize.Length;
						halfSlotSize = itemSize;
						break;
				}
				halfSlotSize.Width = Math.Max(halfSlotSize.Width, itemPosition.LayerOffset + itemSize.Width - halfSlotPosition.LayerOffset);
				halfSlotSize.Length = Math.Max(halfSlotSize.Length, itemPosition.ItemOffset + itemSize.Length - halfSlotPosition.ItemOffset);
				if (arrangeItem != null)
					arrangeItem(item, ref itemPosition, ref itemSize);
				prevItemPosition = itemPosition;
				prevItemSize = itemSize;
			}
			itemIndex += itemPositionsInSlot.Count - 1;
			slotSize.Width = halfSlotPosition.LayerOffset + halfSlotSize.Width - slotPosition.LayerOffset;
			slotSize.Length = Math.Max(slotSize.Length, halfSlotSize.Length);
		}
		protected virtual Rect GetHalfSlotBounds(FlowLayoutLayerInfo layerInfo, int slotIndex, bool isFirstHalf) {
			Rect result = GetSlotBounds(layerInfo, slotIndex);
			FrameworkElement slotFirstItem = LayoutItems[layerInfo.SlotFirstItemIndexes[slotIndex]];
			double halfSlotMaxWidth = GetHalfSlotMaxWidth(slotFirstItem, GetItemSize(slotFirstItem.GetSize()));
			if (isFirstHalf)
				result.Width = Math.Min(result.Width, halfSlotMaxWidth);
			else
				RectHelper.IncLeft(ref result, halfSlotMaxWidth + Parameters.ItemSpace);
			return result;
		}
		protected virtual void GetHalfSlotFirstAndLastItemIndexes(FlowLayoutLayerInfo layerInfo, int slotIndex, bool isFirstHalf,
			out int firstItemIndex, out int lastItemIndex) {
			int slotFirstItemIndex = layerInfo.SlotFirstItemIndexes[slotIndex];
			List<int> itemPositionsInSlot = GetItemPositionsInSlot(LayoutItems, slotFirstItemIndex);
			firstItemIndex = slotFirstItemIndex;
			lastItemIndex = slotFirstItemIndex + itemPositionsInSlot.Count - 1;
			for (int i = 0; i < itemPositionsInSlot.Count; i++)
				if (itemPositionsInSlot[i] == 4) {
					if (isFirstHalf)
						lastItemIndex = slotFirstItemIndex + i - 1;
					else
						firstItemIndex = slotFirstItemIndex + i;
					break;
				}
		}
		protected virtual int GetHalfSlotItemPlaceIndex(FlowLayoutLayerInfo layerInfo, int slotIndex, bool isFirstHalf,
			FrameworkElement item, Rect bounds, Point p, out bool isBeforeItemPlace) {
			Rect halfSlotBounds = GetHalfSlotBounds(layerInfo, slotIndex, isFirstHalf);
			if (!GetElementHitTestBounds(halfSlotBounds, bounds, slotIndex == 0, slotIndex == layerInfo.SlotCount - 1).Contains(p)) {
				isBeforeItemPlace = false;
				return -1;
			}
			if (NeedsHalfSlot(item))
				isBeforeItemPlace = GetLayerOffset(p) < GetLayerCenter(halfSlotBounds);
			else
				isBeforeItemPlace = false;
			int firstItemIndex, lastItemIndex;
			GetHalfSlotFirstAndLastItemIndexes(layerInfo, slotIndex, isFirstHalf, out firstItemIndex, out lastItemIndex);
			return isBeforeItemPlace ? firstItemIndex : lastItemIndex;
		}
		protected virtual double GetHalfSlotMaxWidth(FrameworkElement item, FlowLayoutItemSize itemSize) {
			if (((ITile)item).Size == TileSize.ExtraSmall)
				return 2 * itemSize.Width + Parameters.ItemSpace;
			else
				return itemSize.Width;
		}
		protected virtual List<int> GetItemPositionsInSlot(FrameworkElements items, int slotFirstItemIndex) {
			var result = new List<int>();
			int prevItemPositionInSlot = 0;
			result.Add(prevItemPositionInSlot);
			for (int i = slotFirstItemIndex + 1; i < items.Count; i++) {
				if (NeedsFullSlot(items[i]) || FlowLayoutControl.GetIsFlowBreak(items[i]))
					break;
				int itemPositionInSlot;
				if (((ITile)items[i - 1]).Size == TileSize.Small)
					itemPositionInSlot = prevItemPositionInSlot + 4;
				else
					if (((ITile)items[i]).Size == TileSize.Small)
						itemPositionInSlot = prevItemPositionInSlot - prevItemPositionInSlot % 4 + 4;
					else
						itemPositionInSlot = prevItemPositionInSlot + 1;
				if (itemPositionInSlot >= 2 * 4)
					break;
				result.Add(itemPositionInSlot);
				prevItemPositionInSlot = itemPositionInSlot;
			}
			return result;
		}
		protected override int GetSlotItemPlaceIndex(FlowLayoutLayerInfo layerInfo, int slotIndex, FrameworkElement item,
			Rect bounds, Point p, out bool isBeforeItemPlace) {
			int result = base.GetSlotItemPlaceIndex(layerInfo, slotIndex, item, bounds, p, out isBeforeItemPlace);
			if (result != -1)
				return result;
			if (!NeedsHalfSlot(item))
				for (int i = layerInfo.SlotFirstItemIndexes[slotIndex]; i <= GetSlotLastItemIndex(layerInfo, slotIndex); i++) {
					FrameworkElement slotItem = LayoutItems[i];
					Rect slotItemBounds = slotItem.GetBounds();
					bool isBack = slotIndex == layerInfo.SlotCount - 1;
					if (isBack && !NeedsHalfSlot(slotItem)) {
						List<int> itemPositionsInSlot = GetItemPositionsInSlot(LayoutItems, layerInfo.SlotFirstItemIndexes[slotIndex]);
						int itemPositionInSlot = itemPositionsInSlot[i - layerInfo.SlotFirstItemIndexes[slotIndex]];
						isBack = itemPositionInSlot == 2 || itemPositionInSlot == 3 || itemPositionInSlot == 6 || itemPositionInSlot == 7;
					}
					if (GetElementHitTestBounds(slotItemBounds, bounds, slotIndex == 0, isBack).Contains(p)) {
						isBeforeItemPlace = GetLayerOffset(p) < GetLayerCenter(slotItemBounds);
						return i;
					}
				}
			result = GetHalfSlotItemPlaceIndex(layerInfo, slotIndex, true, item, bounds, p, out isBeforeItemPlace);
			if (result == -1)
				result = GetHalfSlotItemPlaceIndex(layerInfo, slotIndex, false, item, bounds, p, out isBeforeItemPlace);
			return result;
		}
		protected override double GetSlotLength(FrameworkElements items, int itemIndex, FlowLayoutItemSize itemSize) {
			double result = base.GetSlotLength(items, itemIndex, itemSize);
			double maxLength = GetSlotMaxLength(items[itemIndex], itemSize);
			if (result < maxLength)
				foreach (int itemPositionInSlot in GetItemPositionsInSlot(items, itemIndex))
					if (itemPositionInSlot > 1)
						return maxLength;
			return result;
		}
		protected override double GetSlotMaxLength(FrameworkElement item, FlowLayoutItemSize itemSize) {
			if (!NeedsFullSlot(item) && ((ITile)item).Size == TileSize.ExtraSmall)
				return 2 * itemSize.Length + Parameters.ItemSpace;
			else
				return base.GetSlotMaxLength(item, itemSize);
		}
		protected override double GetSlotMaxWidth(FrameworkElement item, FlowLayoutItemSize itemSize) {
			if (NeedsFullSlot(item))
				return base.GetSlotMaxWidth(item, itemSize);
			else
				return 2 * GetHalfSlotMaxWidth(item, itemSize) + Parameters.ItemSpace;
		}
		protected override bool NeedsFullSlot(FrameworkElement item) {
			var tile = item as ITile;
			return item == Model.MaximizedElement ||
				!(Orientation == Orientation.Vertical && tile != null && (tile.Size == TileSize.ExtraSmall || tile.Size == TileSize.Small));
		}
		protected virtual bool NeedsHalfSlot(FrameworkElement item) {
			return item is ITile && ((ITile)item).Size == TileSize.Small;
		}
		protected void AddGroupHeader(object headerSource, Rect headerAreaBounds) {
			TileGroupHeader groupHeader = Parameters.GroupHeaders.Add();
			groupHeader.SetBinding(TileGroupHeader.ContentProperty,
				new Binding { Source = headerSource, Path = new PropertyPath(TileLayoutControl.GroupHeaderProperty), Mode = BindingMode.TwoWay });
#if !SILVERLIGHT
			groupHeader.InvalidateParentsOfModifiedChildren();
#endif
			groupHeader.Measure(headerAreaBounds.Size());
			groupHeader.Arrange(GetGroupHeaderBounds(headerAreaBounds, groupHeader.DesiredSize));
		}
		protected virtual void AddGroupHeaders() {
			for (int i = 0; i < LayerInfos.Count; i++) {
				FlowLayoutLayerInfo layerInfo = LayerInfos[i];
				if (layerInfo.IsHardFlowBreak)
					AddGroupHeader(LayoutItems[layerInfo.FirstItemIndex], GetGroupHeaderAreaBounds(i));
			}
		}
		protected virtual Rect GetGroupBounds(int groupFirstLayerInfoIndex) {
			Rect result = Rect.Empty;
			for (int i = groupFirstLayerInfoIndex; i < LayerInfos.Count; i++) {
				FlowLayoutLayerInfo layerInfo = LayerInfos[i];
				if (i > groupFirstLayerInfoIndex && layerInfo.IsHardFlowBreak)
					break;
				result.Union(GetLayerBounds(layerInfo));
			}
			return result;
		}
		protected virtual Rect GetGroupHeaderAreaBounds(int groupFirstLayerInfoIndex) {
			Rect result = GetGroupBounds(groupFirstLayerInfoIndex);
			result.Y -= Parameters.GroupHeaderSpace;
			result.Height = double.PositiveInfinity;
			return result;
		}
		protected virtual Rect GetGroupHeaderBounds(Rect headerAreaBounds, Size headerSize) {
			Rect result = headerAreaBounds;
			result.Y -= headerSize.Height;
			result.Height = headerSize.Height;
			return result;
		}
		protected new TileLayoutParameters Parameters { get { return base.Parameters as TileLayoutParameters; } }
	}
	public class TileLayoutController : FlowLayoutController {
		public TileLayoutController(IFlowLayoutControl control) : base(control) { }
		public new ITileLayoutControl ILayoutControl { get { return base.ILayoutControl as ITileLayoutControl; } }
		#region Keyboard and Mouse Handling
		protected override void OnMouseLeftButtonUp(DXMouseButtonEventArgs e) {
			bool stopGroupHeaderEditing = IsMouseLeftButtonDown && !IsDragAndDrop;
			base.OnMouseLeftButtonUp(e);
			if (stopGroupHeaderEditing)
				ILayoutControl.StopGroupHeaderEditing();
		}
		#endregion Keyboard and Mouse Handling
		#region Drag&Drop
		protected override DragAndDropController CreateItemDragAndDropControler(Point startDragPoint, FrameworkElement dragControl) {
			return new TileLayoutItemDragAndDropController(this, startDragPoint, dragControl);
		}
		#endregion Drag&Drop
	}
	public class TileLayoutItemDragAndDropController : FlowLayoutItemDragAndDropController {
		public TileLayoutItemDragAndDropController(Controller controller, Point startDragPoint, FrameworkElement dragControl) :
			base(controller, startDragPoint, dragControl) { }
		public override void EndDragAndDrop(bool accept) {
			if (accept)
				TileLayoutControl.SetGroupHeader(DragControl, TileLayoutControl.GetGroupHeader(DragControlPlaceHolder));
			else
				RestoreGroupHeaderOriginalValues();
			base.EndDragAndDrop(accept);
		}
		protected override FrameworkElement CreateDragControlPlaceHolder() {
			if (DragControl is ITile)
				return new TilePlaceHolder((ITile)DragControl);
			else
				return base.CreateDragControlPlaceHolder();
		}
		protected override Point GetItemPlacePoint(Point p) {
			Point relativeOffset = PointHelper.Subtract(StartDragRelativePoint, new Point(0.5, 0.5));
			Point offset = PointHelper.Multiply(DragImageSize.ToPoint(), relativeOffset);
			return PointHelper.Subtract(p, offset);
		}
		protected override void InitDragControlPlaceHolder() {
			Controller.ILayoutControl.StopGroupHeaderEditing();
			base.InitDragControlPlaceHolder();
			TileLayoutControl.SetGroupHeader(DragControlPlaceHolder, TileLayoutControl.GetGroupHeader(DragControl));
		}
		protected override void OnGroupFirstItemChanged(FrameworkElement oldValue, FrameworkElement newValue) {
			base.OnGroupFirstItemChanged(oldValue, newValue);
			MoveGroupHeaderAndStoreOriginalValues(oldValue, newValue);
		}
		protected void MoveGroupHeaderAndStoreOriginalValues(UIElement from, UIElement to) {
			if (GroupHeaderOriginalValues == null)
				GroupHeaderOriginalValues = new Dictionary<UIElement, object>();
			object groupHeader = TileLayoutControl.GetGroupHeader(from);
			if (!GroupHeaderOriginalValues.ContainsKey(from))
				GroupHeaderOriginalValues.Add(from, groupHeader);
			TileLayoutControl.SetGroupHeader(from, DependencyProperty.UnsetValue);
			if (to != null) {
				if (!GroupHeaderOriginalValues.ContainsKey(to))
					GroupHeaderOriginalValues.Add(to, TileLayoutControl.GetGroupHeader(to));
				TileLayoutControl.SetGroupHeader(to, groupHeader);
			}
		}
		protected void RestoreGroupHeaderOriginalValues() {
			if (GroupHeaderOriginalValues == null)
				return;
			foreach (KeyValuePair<UIElement, object> originalValue in GroupHeaderOriginalValues)
				TileLayoutControl.SetGroupHeader(originalValue.Key, originalValue.Value);
		}
		protected new TileLayoutController Controller { get { return (TileLayoutController)base.Controller; } }
		private Dictionary<UIElement, object> GroupHeaderOriginalValues { get; set; }
		private class TilePlaceHolder : Canvas, ITile {
			public TilePlaceHolder(ITile tile) {
				Size = tile.Size;
			}
			public TileSize Size { get; private set; }
			#region ITile
			void ITile.Click() { }
			#endregion ITile
		}
	}
}
