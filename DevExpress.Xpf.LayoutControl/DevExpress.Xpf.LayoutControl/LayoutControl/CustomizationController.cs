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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
#if !SILVERLIGHT
using System.Windows.Interop;
#endif
namespace DevExpress.Xpf.LayoutControl {
	public interface ILayoutControlCustomizableItem {
		FrameworkElement AddNewItem();
		bool CanAddNewItems { get; }
		bool HasHeader { get; }
		object Header { get; set; }
		bool IsLocked { get; }
	}
	public class LayoutControlNewItemInfo {
		public LayoutControlNewItemInfo(string label, object data) {
			Label = label;
			Data = data;
		}
		public object Data { get; private set; }
		public string Label { get; private set; }
	}
	public class LayoutControlNewItemsInfo : List<LayoutControlNewItemInfo> { }
	public class LayoutControlSelectionChangedEventArgs : EventArgs {
		public LayoutControlSelectionChangedEventArgs(FrameworkElements selectedElements) {
			SelectedElements = selectedElements;
		}
		public FrameworkElements SelectedElements { get; private set; }
	}
	public class LayoutControlStartDragAndDropEventArgs<T> : EventArgs {
		public LayoutControlStartDragAndDropEventArgs(T data, MouseEventArgs mouseEventArgs, FrameworkElement source) {
			Data = data;
			MouseEventArgs = mouseEventArgs;
			Source = source;
		}
		public T Data { get; private set; }
		public MouseEventArgs MouseEventArgs { get; private set; }
		public FrameworkElement Source { get; private set; }
	}
	public class LayoutControlCustomizationController {
		private Style _CustomizationControlStyle;
		private FrameworkElement _CustomizationToolbarElement;
		private TransparentPopup _CustomizationToolbarPopup;
		private Style _ItemCustomizationToolbarStyle;
		private Style _ItemParentIndicatorStyle;
		private Style _ItemSelectionIndicatorStyle;
#if !SILVERLIGHT
		private Window _RootVisual;
#endif
		public LayoutControlCustomizationController(LayoutControlController controller) {
			Controller = controller;
		}
		public void CheckSelectedElementsAreInVisualTree() {
			if (SelectedElements == null)
				return;
			var selectedElements = new List<FrameworkElement>(SelectedElements.Where<FrameworkElement>(element => element.IsInVisualTree()));
			SelectedElements.Assign(selectedElements);
		}
		public virtual IEnumerable<UIElement> GetInternalElements() {
			if (CustomizationCover != null)
				yield return CustomizationCover;
			if (CustomizationCanvas != null)
				yield return CustomizationCanvas;
			if (CustomizationControl != null)
				yield return CustomizationControl;
#if SILVERLIGHT
			if (_CustomizationToolbarPopup != null)
				yield return _CustomizationToolbarPopup.Popup;
#endif
		}
		public virtual FrameworkElement GetSelectableItem(Point p) {
			FrameworkElement result = Controller.GetItem(p, false, Control.IsInDesignTool());
			if (Control.IsInDesignTool() && result is LayoutItem)
				result = ((LayoutItem)result).GetSelectableElement(Control.MapPoint(p, result));
			if (!Control.IsInDesignTool() && result == Control)
				result = null;
			return result;
		}
		public virtual void OnMeasure(Size availableSize) {
			if (CustomizationCover != null)
				CustomizationCover.Measure(availableSize);
			if (CustomizationCanvas != null)
				CustomizationCanvas.Measure(availableSize);
		}
		public virtual void OnArrange(Size finalSize) {
			if (CustomizationCover != null)
				CustomizationCover.Arrange(ILayoutControl.ClientBounds);
			if (CustomizationCanvas != null) {
				CustomizationCanvas.Arrange(ILayoutControl.ClientBounds);
				CustomizationCanvas.ClipToBounds();
			}
			if (CustomizationControl != null) {
				CustomizationControl.Measure(ILayoutControl.ClientBounds.Size());
				CustomizationControl.Arrange(ILayoutControl.ClientBounds);
			}
		}
		public FrameworkElement Control { get { return Controller.Control; } }
		public Canvas CustomizationCanvas { get; private set; }
		public Style CustomizationControlStyle {
			get { return _CustomizationControlStyle; }
			set {
				if (CustomizationControlStyle == value)
					return;
				_CustomizationControlStyle = value;
				if (CustomizationControl != null)
					CustomizationControl.Style = CustomizationControlStyle;
			}
		}
		public ILayoutControl ILayoutControl { get { return Controller.ILayoutControl; } }
		public Style ItemCustomizationToolbarStyle {
			get { return _ItemCustomizationToolbarStyle; }
			set {
				if (ItemCustomizationToolbarStyle == value)
					return;
				_ItemCustomizationToolbarStyle = value;
				if (CustomizationToolbar != null)
					CustomizationToolbar.Style = ItemCustomizationToolbarStyle;
			}
		}
		public Style ItemParentIndicatorStyle {
			get { return _ItemParentIndicatorStyle; }
			set {
				if (ItemParentIndicatorStyle == value)
					return;
				_ItemParentIndicatorStyle = value;
				if (ParentIndicator != null)
					ParentIndicator.Style = ItemParentIndicatorStyle;
			}
		}
		public Style ItemSelectionIndicatorStyle {
			get { return _ItemSelectionIndicatorStyle; }
			set {
				if (ItemSelectionIndicatorStyle == value)
					return;
				_ItemSelectionIndicatorStyle = value;
				if (SelectionIndicators != null)
					SelectionIndicators.ItemStyle = ItemSelectionIndicatorStyle;
			}
		}
		public FrameworkElements SelectedElements { get; private set; }
		public event EventHandler<LayoutControlSelectionChangedEventArgs> SelectionChanged;
		protected internal virtual void BeginCustomization() {
			SelectedElements = new FrameworkElements();
			SelectedElements.CollectionChanged += (sender, e) => OnSelectedElementsChanged(e);
			if (!Control.IsInDesignTool())
				ShowCustomizationCover();
			ShowCustomizationCanvas();
			if (!Control.IsInDesignTool()) {
				SelectionIndicators = CreateSelectionIndicators(CustomizationCanvas);
				SelectionIndicators.ItemStyle = ItemSelectionIndicatorStyle;
				ShowCustomizationControl();
#if !SILVERLIGHT
				if (Controller.IsLoaded)
					RootVisual = Control.FindElementByTypeInParents<Window>(null);
#endif
			}
			ILayoutControl.AvailableItems.CollectionChanged += OnAvailableItemsChanged;
		}
		protected internal virtual void EndCustomization() {
			ILayoutControl.AvailableItems.CollectionChanged -= OnAvailableItemsChanged;
#if !SILVERLIGHT
			RootVisual = null;
#endif
			SelectedElements.Clear();
			if (CustomizationToolbar != null)
				HideCustomizationToolbar(true);
			if (!Control.IsInDesignTool())
				HideCustomizationControl();
			SelectionIndicators = null;
			HideCustomizationCanvas();
			if (!Control.IsInDesignTool())
				HideCustomizationCover();
			SelectedElements = null;
		}
		protected virtual Canvas CreateCustomizationCanvas() {
			return new Canvas();
		}
		protected virtual LayoutControlCustomizationControl CreateCustomizationControl() {
			return new LayoutControlCustomizationControl();
		}
		protected virtual LayoutItemCustomizationToolbar CreateCustomizationToolbar() {
			return new LayoutItemCustomizationToolbar();
		}
		protected virtual LayoutItemParentIndicator CreateParentIndicator() {
			return new LayoutItemParentIndicator();
		}
		protected virtual LayoutItemSelectionIndicators CreateSelectionIndicators(Panel container) {
			return new LayoutItemSelectionIndicators(container, ILayoutControl);
		}
		protected virtual void AddNewItem(FrameworkElement container) {
			var customizableItem = container as ILayoutControlCustomizableItem;
			if (customizableItem == null || !customizableItem.CanAddNewItems)
				return;
			FrameworkElement item = customizableItem.AddNewItem();
			ILayoutControl.InitNewElement(item);
			ProcessSelection(item, true);
#if !SILVERLIGHT
			Control.UpdateLayout();
#endif
			ShowCustomizationToolbar();
		}
		protected virtual bool CanSetCustomizationToolbarElement(FrameworkElement element) {
			if (Control.IsInDesignTool())
				return false;
			if (element != null) {
				if (CustomizationControl != null && CustomizationControl.IsAvailableItemsListOpen)
					return false;
				if (Controller.IsDragAndDrop)
					return false;
#if !SILVERLIGHT
				if (RootVisual != null &&
					(!RootVisual.IsActive || !BrowserInteropHelper.IsBrowserHosted && RootVisual.WindowState == WindowState.Minimized))
					return false;
#endif
			}
			return true;
		}
		protected virtual void ChangeSelectedGroupPadding(bool isHorizontalChange, bool isIncrease) {
			var padding = SelectedGroup.Padding;
			var value = isHorizontalChange ? padding.Left : padding.Top;
			if (isIncrease)
				value++;
			else
				value--;
			value = Math.Max(0, value);
			if (isHorizontalChange)
				padding.Left = padding.Right = value;
			else
				padding.Top = padding.Bottom = value;
			if (SelectedGroup.Padding != padding) {
				SelectedGroup.Padding = padding;
				Controller.OnModelChanged(new LayoutControlModelPropertyChangedEventArgs(SelectedGroup, "Padding", LayoutControlBase.PaddingProperty));
			}
		}
		protected virtual void DeleteAvailableItem(FrameworkElement item) {
			FocusControl();
			ILayoutControl.DeleteAvailableItem(item);
		}
		protected void FocusControl() {
			if (CustomizationControl != null)
				CustomizationControl.Focus();
		}
		protected virtual Geometry GetCustomizationCoverHitTestClip() {
			var result = new PathGeometry();
			result.Figures.Add(GraphicsHelper.CreateRectFigure(CustomizationCover.GetSize().ToRect()));
			var areas = new List<Rect>();
			ILayoutControl.GetLiveCustomizationAreas(areas, CustomizationCover);
			foreach (var area in areas)
				result.Figures.Add(GraphicsHelper.CreateRectFigure(area));
			return result;
		}
		protected FrameworkElement GetCustomizationToolbarElement(Point p) {
			if (!ILayoutControl.ClientBounds.Contains(p))
				return null;
			foreach (FrameworkElement element in SelectedElements)
				if (element != Control &&
#if !SILVERLIGHT
					element.IsInVisualTree() &&
#endif
					GetCustomizationToolbarElementBounds(element).Contains(p))
					return element;
			return null;
		}
		protected virtual Rect GetCustomizationToolbarElementBounds(FrameworkElement element) {
			return element.GetBounds(Control);
		}
		protected virtual Point GetCustomizationToolbarOffset(Rect elementBounds, Size toolbarSize) {
			var result = RectHelper.New(toolbarSize);
			RectHelper.AlignHorizontally(ref result, elementBounds, HorizontalAlignment.Center);
			result.Y = elementBounds.Bottom;
			return result.Location();
		}
		protected virtual LayoutControlNewItemsInfo GetNewItemsInfo() {
			return new LayoutControlNewItemsInfo {
				new LayoutControlNewItemInfo(LocalizationRes.LayoutControl_Customization_NewGroupBox, LayoutGroupView.GroupBox),
				new LayoutControlNewItemInfo(LocalizationRes.LayoutControl_Customization_NewTabbedGroup, LayoutGroupView.Tabs)
			};
		}
		protected virtual void InitCustomizationControl() {
			CustomizationControl.AvailableItems = ILayoutControl.VisibleAvailableItems;
			CustomizationControl.SetBinding(LayoutControlCustomizationControl.AvailableItemsUIVisibilityProperty,
				new Binding("ActualAllowAvailableItemsDuringCustomization") { Source = Control, Converter = new BoolToVisibilityConverter() });
			CustomizationControl.NewItemsInfo = GetNewItemsInfo();
			CustomizationControl.SetBinding(LayoutControlCustomizationControl.NewItemsUIVisibilityProperty,
				new Binding("AllowNewItemsDuringCustomization") { Source = Control, Converter = new BoolToVisibilityConverter() });
			CustomizationControl.Style = CustomizationControlStyle;
			CustomizationControl.DeleteAvailableItem += DeleteAvailableItem;
			CustomizationControl.IsAvailableItemsListOpenChanged += OnIsAvailableItemsListOpenChanged;
			CustomizationControl.StartAvailableItemDragAndDrop += (o, e) => StartAvailableItemDragAndDrop(e);
			CustomizationControl.StartNewItemDragAndDrop += (o, e) => StartNewItemDragAndDrop(e);
		}
		protected virtual void FinalizeCustomizationControl() {
			CustomizationControl.IsAvailableItemsListOpenChanged -= OnIsAvailableItemsListOpenChanged;
			CustomizationControl.AvailableItems = null;
			CustomizationControl.NewItemsInfo = null;
		}
		protected virtual void InitCustomizationToolbar() {
			CustomizationToolbar.SetBinding(LayoutItemCustomizationToolbar.AvailableItemsUIVisibilityProperty,
				new Binding("ActualAllowAvailableItemsDuringCustomization") { Source = Control, Converter = new BoolToVisibilityConverter() });
			CustomizationToolbar.Style = ItemCustomizationToolbarStyle;
			UpdateCustomizationToolbarValues();
			CustomizationToolbar.AddNewItem += () => AddNewItem(CustomizationToolbarElement);
			CustomizationToolbar.ItemHeaderChanged +=
				() => ((ILayoutControlCustomizableItem)CustomizationToolbarElement).Header = CustomizationToolbar.ItemHeader;
			CustomizationToolbar.ItemHorizontalAlignmentChanged +=
				() => SelectedElementsHorizontalAlignment = CustomizationToolbar.ItemHorizontalAlignment;
			CustomizationToolbar.ItemVerticalAlignmentChanged +=
				() => SelectedElementsVerticalAlignment = CustomizationToolbar.ItemVerticalAlignment;
			CustomizationToolbar.MoveItem += (forward) => MoveItem(CustomizationToolbarElement, forward);
			CustomizationToolbar.MoveItemToAvailableItems += () => MoveItemsToAvailableItems(SelectedElements);
			CustomizationToolbar.ReturnFocus += () => FocusControl();
			CustomizationToolbar.SelectItemParent += () => MoveSelectionToParent(CustomizationToolbarElement);
			CustomizationToolbar.ShowItemParentIndicator += () => ShowParentIndicator(CustomizationToolbarElement);
			CustomizationToolbar.HideItemParentIndicator += () => HideParentIndicator();
		}
		protected virtual void MoveItem(FrameworkElement item, bool forward) {
			var parent = item.Parent as ILayoutGroup;
			if (parent == null)
				return;
			FrameworkElements visibleChildren = parent.GetLogicalChildren(true);
			int index = visibleChildren.IndexOf(item);
			if (forward)
				index++;
			else
				index--;
			if (index < 0 || index > visibleChildren.Count - 1)
				return;
			UIElement placementItem = visibleChildren[index];
			parent.Children.Remove(item);
			index = parent.Children.IndexOf(placementItem);
			if (forward)
				index++;
			parent.Children.Insert(index, item);
			ILayoutControl.MakeControlVisible(item);
		}
		protected virtual void MoveItemsToAvailableItems(FrameworkElements items) {
			if (items.Count == 1)
				ILayoutControl.AvailableItems.Add(items[0]);
			else {
				ILayoutControl.AvailableItems.BeginUpdate();
				try {
					foreach (FrameworkElement item in items)
						ILayoutControl.AvailableItems.Add(item);
				}
				finally {
					ILayoutControl.AvailableItems.EndUpdate();
				}
			}
			ILayoutControl.OptimizeLayout(true);
		}
		protected virtual void MoveSelectionToParent(FrameworkElement child) {
			SelectedElements.BeginUpdate();
			try {
				if (child != null) {
					if (child == Control)
						return;
					SelectedElements.Clear();
					SelectedElements.Add((FrameworkElement)child.Parent);
				}
				else
					if (SelectedElements.Count > 1) {
						var selectedElement = SelectedElements[SelectedElements.Count - 1];
						SelectedElements.Clear();
						SelectedElements.Add(selectedElement);
					}
					else
						if (SelectedElements.Count == 1 && SelectedElements[0] != Control)
							SelectedElements[0] = (FrameworkElement)SelectedElements[0].Parent;
			}
			finally {
				SelectedElements.EndUpdate();
			}
			ShowCustomizationToolbar();
		}
		protected virtual void OnAvailableItemsChanged(NotifyCollectionChangedEventArgs args) {
			var selectedElements = SelectedElements.Where<FrameworkElement>(element => !ILayoutControl.AvailableItems.Contains(element));
			SelectedElements.Assign(new List<FrameworkElement>(selectedElements));
		}
		protected virtual void OnAvailableItemsListOpened() {
			CustomizationToolbarElement = null;
		}
		protected virtual void OnAvailableItemsListClosed() {
			ShowCustomizationToolbar();
		}
		protected internal virtual void OnControlVisibilityChanged(FrameworkElement control) {
			if (control.Visibility == Visibility.Visible) {
				if (SelectedElements.Count == 1) {
					Control.UpdateLayout();
					ILayoutControl.MakeControlVisible(SelectedElements[0]);
				}
			}
			else
				SelectedElements.Remove(control);
		}
		protected internal virtual void OnGroupCollapsed(ILayoutGroup group) {
			foreach (FrameworkElement element in SelectedElements)
				if (element.FindIsInParents(group.Control)) {
					ProcessSelection(group.Control, true);
					break;
				}
		}
		protected internal virtual void OnLayoutUpdated() {
			UpdateCustomizationCoverHitTestClip();
			if (SelectionIndicators != null)
				SelectionIndicators.UpdateBounds();
			if (ParentIndicator != null)
				ParentIndicator.UpdateBounds();
			UpdateCustomizationToolbarValues();
			UpdateCustomizationToolbarBounds();
		}
		protected internal virtual void OnLoaded() {
#if !SILVERLIGHT
			RootVisual = Control.FindElementByTypeInParents<Window>(null);
#endif
		}
		protected virtual void OnSelectedElementsChanged(NotifyCollectionChangedEventArgs e) {
			if (CustomizationToolbarElement != null && !SelectedElements.Contains(CustomizationToolbarElement))
				CustomizationToolbarElement = null;
			if (SelectedElements.Count == 1) {
				ILayoutControl.MakeControlVisible(SelectedElements[0]);
				Control.UpdateLayout();
			}
			if (SelectionIndicators != null)
				SelectionIndicators.Update(SelectedElements);
			FocusControl();
			if (SelectionChanged != null)
				SelectionChanged(Control, new LayoutControlSelectionChangedEventArgs(SelectedElements));
		}
		protected internal virtual void OnTabClicked(ILayoutGroup group, FrameworkElement selectedTabChild) {
			if (group.IsLocked)
				return;
			ProcessSelection(selectedTabChild ?? group.Control, true);
			ShowCustomizationToolbar();
		}
		protected internal virtual void ProcessSelection(FrameworkElement element, bool clearExistingSelection) {
			if (element == null)
				return;
			SelectedElements.BeginUpdate();
			try {
				if (clearExistingSelection)
					if (SelectedElements.Count == 1)
						SelectedElements[0] = element;
					else {
						SelectedElements.Clear();
						SelectedElements.Add(element);
					}
				else
					if (!SelectedElements.Contains(element)) {
						SelectedElements.Remove(Control);
						SelectedElements.Add(element);
					}
					else {
						SelectedElements.Remove(element);
						if (Control.IsInDesignTool() && SelectedElements.Count == 0)
							SelectedElements.Add(Control);
					}
			}
			finally {
				SelectedElements.EndUpdate();
			}
		}
		protected virtual void SetSelectedGroupItemSpace(double itemSpace) {
			if (SelectedGroup.ItemSpace == itemSpace)
				return;
			SelectedGroup.ItemSpace = itemSpace;
			Controller.OnModelChanged(new LayoutControlModelPropertyChangedEventArgs(SelectedGroup, "ItemSpace", LayoutControlBase.ItemSpaceProperty));
		}
		protected void ShowCustomizationToolbar() {
			if (SelectedElements.Count != 0)
				CustomizationToolbarElement = SelectedElements[0];
		}
		protected void ShowCustomizationToolbar(Point p) {
			FrameworkElement element = GetCustomizationToolbarElement(p);
			if (element != null)
				CustomizationToolbarElement = element;
		}
		protected virtual void ShowLayoutXML() {
			var sb = new StringBuilder();
			using (var xml = XmlWriter.Create(sb, new XmlWriterSettings { Indent = true }))
				ILayoutControl.WriteToXML(xml);
			var textBox = new TextBox { FontFamily = new FontFamily("Courier New"), FontSize = 12, IsReadOnly = true };
			textBox.KeyDown +=
				delegate(object sender, KeyEventArgs e) {
					if (e.Key == Key.Escape) {
						((Popup)textBox.Parent).IsOpen = false;
						e.Handled = true;
					}
				};
			textBox.LostFocus += (o, e) => ((Popup)textBox.Parent).IsOpen = false;
			textBox.Text = sb.ToString();
			textBox.SelectAll();
			var popup = new Popup();
			popup.Child = textBox;
			popup.Closed += (o, e) => FocusControl();
			popup.IsOpen = true;
			textBox.Focus();
		}
		protected void UpdateCustomizationCoverHitTestClip() {
			if (CustomizationCover != null)
				CustomizationCover.HitTestClip = GetCustomizationCoverHitTestClip();
		}
		protected void UpdateCustomizationToolbarBounds() {
			if (CustomizationToolbarElement != null)
				UpdateCustomizationToolbarBounds(CustomizationToolbarElement);
		}
		protected void UpdateCustomizationToolbarValues() {
			if (CustomizationToolbarElement == null)
				return;
			CustomizationToolbar.IsInitializing = true;
			try {
				UpdateCustomizationToolbarValuesCore();
			}
			finally {
				CustomizationToolbar.IsInitializing = false;
			}
		}
		protected virtual void UpdateCustomizationToolbarValuesCore() {
			CustomizationToolbar.ItemHorizontalAlignment = SelectedElementsHorizontalAlignment;
			CustomizationToolbar.ItemVerticalAlignment = SelectedElementsVerticalAlignment;
			if (CustomizationToolbarElementParent.HasUniformLayout)
				CustomizationToolbar.SetBinding(LayoutItemCustomizationToolbar.ItemMovingUIVisibilityProperty,
					new Binding("AllowItemMovingDuringCustomization") { Source = Control, Converter = new BoolToVisibilityConverter() });
			else
				CustomizationToolbar.ItemMovingUIVisibility = Visibility.Collapsed;
			FrameworkElements itemParentChildren = CustomizationToolbarElementParent.GetLogicalChildren(true);
			CustomizationToolbar.CanMoveItemBackward = CustomizationToolbarElement != itemParentChildren[0];
			CustomizationToolbar.CanMoveItemForward = CustomizationToolbarElement != itemParentChildren[itemParentChildren.Count - 1];
			CustomizationToolbar.ItemMovingDirection = CustomizationToolbarElementParent.VisibleOrientation;
			var customizableItem = CustomizationToolbarElement as ILayoutControlCustomizableItem;
			if (customizableItem != null && !customizableItem.IsLocked &&
				customizableItem.HasHeader && (customizableItem.Header == null || customizableItem.Header is string)) {
				CustomizationToolbar.ItemHeader = customizableItem.Header;
				CustomizationToolbar.SetBinding(LayoutItemCustomizationToolbar.ItemRenamingUIVisibilityProperty,
					new Binding("AllowItemRenamingDuringCustomization") { Source = Control, Converter = new BoolToVisibilityConverter() });
			}
			else
				CustomizationToolbar.ItemRenamingUIVisibility = Visibility.Collapsed;
			if (customizableItem != null && !customizableItem.IsLocked && customizableItem.CanAddNewItems)
				CustomizationToolbar.SetBinding(LayoutItemCustomizationToolbar.NewItemsUIVisibilityProperty,
					new Binding("AllowNewItemsDuringCustomization") { Source = Control, Converter = new BoolToVisibilityConverter() });
			else
				CustomizationToolbar.NewItemsUIVisibility = Visibility.Collapsed;
		}
		protected T? GetSelectedElementsProperty<T>(Func<FrameworkElement, T> getElementProperty) where T : struct {
			T? result = null;
			foreach (var element in SelectedElements)
				if (element != Control)
					if (result == null)
						result = getElementProperty(element);
					else
						if (!result.Equals(getElementProperty(element)))
							return null;
			return result;
		}
		protected void SetSelectedElementsProperty<T>(Action<FrameworkElement, T> setElementProperty, T value) {
			foreach (var element in SelectedElements)
				if (element != Control)
					setElementProperty(element, value);
		}
		protected LayoutControlController Controller { get; private set; }
		protected LayoutControlCustomizationControl CustomizationControl { get; private set; }
		protected LayoutControlCustomizationCover CustomizationCover { get; private set; }
		protected LayoutItemCustomizationToolbar CustomizationToolbar { get; private set; }
		protected FrameworkElement CustomizationToolbarElement {
			get { return _CustomizationToolbarElement; }
			set {
				if (value == Control)
					value = null;
				if (CustomizationToolbarElement == value || !CanSetCustomizationToolbarElement(value))
					return;
				FrameworkElement oldCustomizationToolbarElement = CustomizationToolbarElement;
				_CustomizationToolbarElement = value;
				if (oldCustomizationToolbarElement != null)
					HideCustomizationToolbar(false);
				if (CustomizationToolbarElement != null)
					ShowCustomizationToolbar(CustomizationToolbarElement);
			}
		}
		protected ILayoutGroup CustomizationToolbarElementParent { get { return CustomizationToolbarElement.Parent as ILayoutGroup; } }
		protected LayoutItemParentIndicator ParentIndicator { get; private set; }
#if !SILVERLIGHT
		protected Window RootVisual {
			get { return _RootVisual; }
			private set {
				if (RootVisual == value)
					return;
				if (RootVisual != null) {
					RootVisual.Activated -= RootVisualActivated;
					RootVisual.Deactivated -= RootVisualDeactivated;
					RootVisual.LocationChanged -= RootVisualLocationChanged;
					RootVisual.StateChanged -= RootVisualStateChanged;
				}
				_RootVisual = value;
				if (RootVisual != null) {
					RootVisual.Activated += RootVisualActivated;
					RootVisual.Deactivated += RootVisualDeactivated;
					RootVisual.LocationChanged += RootVisualLocationChanged;
					RootVisual.StateChanged += RootVisualStateChanged;
				}
			}
		}
#endif
		protected HorizontalAlignment? SelectedElementsHorizontalAlignment {
			get {
				return GetSelectedElementsProperty<HorizontalAlignment>(
					element => (element.Parent as ILayoutGroup).GetItemHorizontalAlignment(element));
			}
			set {
				if (value != null)
					SetSelectedElementsProperty<HorizontalAlignment>(
						(element, alignment) => (element.Parent as ILayoutGroup).SetItemHorizontalAlignment(element, alignment, true),
						value.Value);
			}
		}
		protected VerticalAlignment? SelectedElementsVerticalAlignment {
			get {
				return GetSelectedElementsProperty<VerticalAlignment>(
					element => (element.Parent as ILayoutGroup).GetItemVerticalAlignment(element));
			}
			set {
				if (value != null)
					SetSelectedElementsProperty<VerticalAlignment>(
						(element, alignment) => (element.Parent as ILayoutGroup).SetItemVerticalAlignment(element, alignment, true),
						value.Value);
			}
		}
		protected LayoutGroup SelectedGroup {
			get {
				if (SelectedElements.Count == 1 && (SelectedElements[0] == Control || SelectedElements[0].IsLayoutGroup()))
					return (LayoutGroup)SelectedElements[0];
				else
					return null;
			}
		}
		protected LayoutItemSelectionIndicators SelectionIndicators { get; private set; }
		private void ShowCustomizationCanvas() {
			CustomizationCanvas = CreateCustomizationCanvas();
			CustomizationCanvas.SetZIndex(PanelBase.HighZIndex);
			ILayoutControl.Children.Add(CustomizationCanvas);
		}
		private void HideCustomizationCanvas() {
			ILayoutControl.Children.Remove(CustomizationCanvas);
			CustomizationCanvas = null;
		}
		private void ShowCustomizationControl() {
			CustomizationControl = CreateCustomizationControl();
			InitCustomizationControl();
			CustomizationControl.SetZIndex(PanelBase.HighZIndex);
			ILayoutControl.Children.Add(CustomizationControl);
		}
		private void HideCustomizationControl() {
			ILayoutControl.Children.Remove(CustomizationControl);
			FinalizeCustomizationControl();
			CustomizationControl = null;
		}
		private void ShowCustomizationCover() {
			CustomizationCover = new LayoutControlCustomizationCover();
			CustomizationCover.SetZIndex(PanelBase.HighZIndex);
			ILayoutControl.Children.Add(CustomizationCover);
		}
		private void HideCustomizationCover() {
			ILayoutControl.Children.Remove(CustomizationCover);
			CustomizationCover = null;
		}
		private void ShowCustomizationToolbar(FrameworkElement element) {
			if (CustomizationToolbar == null) {
				CustomizationToolbar = CreateCustomizationToolbar();
				InitCustomizationToolbar();
				_CustomizationToolbarPopup = new TransparentPopup();
				_CustomizationToolbarPopup.Child = CustomizationToolbar;
#if SILVERLIGHT
				ILayoutControl.Children.Add(_CustomizationToolbarPopup.Popup);
#else
				_CustomizationToolbarPopup.PlacementTarget = Control;
#endif
			}
			else
				UpdateCustomizationToolbarValues();
			UpdateCustomizationToolbarBounds(element);
		}
		private void HideCustomizationToolbar(bool remove) {
			_CustomizationToolbarPopup.IsOpen = false;
			if (remove) {
#if SILVERLIGHT
				ILayoutControl.Children.Remove(_CustomizationToolbarPopup.Popup);
#endif
				_CustomizationToolbarPopup.Child = null;
				_CustomizationToolbarPopup = null;
				CustomizationToolbar.OnHide();
				CustomizationToolbar = null;
			}
		}
		private void UpdateCustomizationToolbarBounds(FrameworkElement element) {
			Rect elementBounds = element.GetBounds(Control);
			elementBounds.Intersect(ILayoutControl.ClientBounds);
			_CustomizationToolbarPopup.IsOpen = Control.IsVisible() && !elementBounds.IsEmpty;
			if (!_CustomizationToolbarPopup.IsOpen)
				return;
			CustomizationToolbar.Measure(SizeHelper.Infinite);
			Point toolbarOffset = GetCustomizationToolbarOffset(elementBounds, CustomizationToolbar.DesiredSize);
			_CustomizationToolbarPopup.MakeVisible(toolbarOffset, elementBounds);
		}
		private void ShowParentIndicator(FrameworkElement element) {
			ParentIndicator = CreateParentIndicator();
			ParentIndicator.Style = ItemParentIndicatorStyle;
			CustomizationCanvas.Children.Add(ParentIndicator);
			ParentIndicator.LayoutControl = ILayoutControl;
			ParentIndicator.Element = (FrameworkElement)element.Parent;
		}
		private void HideParentIndicator() {
			CustomizationCanvas.Children.Remove(ParentIndicator);
			ParentIndicator = null;
		}
		private void OnAvailableItemsChanged(object sender, NotifyCollectionChangedEventArgs e) {
			OnAvailableItemsChanged(e);
		}
		private void OnIsAvailableItemsListOpenChanged(object sender, EventArgs e) {
			if (CustomizationControl.IsAvailableItemsListOpen)
				OnAvailableItemsListOpened();
			else
				OnAvailableItemsListClosed();
		}
#if !SILVERLIGHT
		private void RootVisualActivated(object sender, EventArgs e) {
			ShowCustomizationToolbar();
		}
		private void RootVisualDeactivated(object sender, EventArgs e) {
			CustomizationToolbarElement = null;
		}
		private void RootVisualLocationChanged(object sender, EventArgs e) {
			UpdateCustomizationToolbarBounds();
			if (_CustomizationToolbarPopup != null)
				_CustomizationToolbarPopup.UpdatePosition();
		}
		private void RootVisualStateChanged(object sender, EventArgs e) {
			if (RootVisual.WindowState == WindowState.Minimized)
				CustomizationToolbarElement = null;
			else
				ShowCustomizationToolbar();
		}
#endif
		#region Keyboard and Mouse Handling
		protected internal virtual void OnKeyDown(DXKeyEventArgs e) {
			switch (e.Key) {
				case Key.Escape:
					MoveSelectionToParent(null);
					e.Handled = true;
					break;
				case Key.D0:
				case Key.D1:
				case Key.D2:
				case Key.D3:
				case Key.D4:
				case Key.D5:
				case Key.D6:
				case Key.D7:
				case Key.D8:
				case Key.D9:
					if (SelectedGroup != null && !SelectedGroup.IsLocked) {
						SetSelectedGroupItemSpace(e.Key - Key.D0);
						e.Handled = true;
					}
					break;
				case Key.Left:
				case Key.Right:
				case Key.Up:
				case Key.Down:
					if (SelectedGroup != null && !SelectedGroup.IsLocked) {
						ChangeSelectedGroupPadding(e.Key == Key.Left || e.Key == Key.Right, e.Key == Key.Right || e.Key == Key.Up);
						e.Handled = true;
					}
					break;
				case Key.F2:
					if (!Control.IsInDesignTool()) {
						ShowLayoutXML();
						e.Handled = true;
					}
					break;
			}
		}
		protected internal virtual void OnMouseMove(DXMouseEventArgs e) {
			if (LayoutControlController.MouseCaptureOwner == null)
				ShowCustomizationToolbar(e.GetPosition(Control));
		}
		protected internal virtual void OnMouseLeftButtonDown(DXMouseButtonEventArgs e) {
			ProcessSelection(GetSelectableItem(e.GetPosition(Control)), Keyboard.Modifiers == ModifierKeys.None);
		}
		protected internal virtual void OnMouseLeftButtonUp(DXMouseButtonEventArgs e) {
			ShowCustomizationToolbar(e.GetPosition(Control));
		}
		protected internal virtual void OnMouseRightButtonDown(DXMouseButtonEventArgs e) {
			FrameworkElement element = GetSelectableItem(e.GetPosition(Control));
			if (!SelectedElements.Contains(element))
				ProcessSelection(element, true);
		}
		#endregion Keyboard and Mouse Handling
		#region Drag&Drop
		protected virtual FrameworkElement CreateNewItem(LayoutControlNewItemInfo itemInfo) {
			if (!(itemInfo.Data is LayoutGroupView))
				return null;
			LayoutGroup item = ILayoutControl.CreateGroup();
			item.Header = itemInfo.Label;
			item.View = (LayoutGroupView)itemInfo.Data;
			InitNewItem(item);
			return item;
		}
		protected virtual void InitNewItem(LayoutGroup group) {
			if (group.View != LayoutGroupView.Tabs)
				return;
			group.Children.Add(ILayoutControl.CreateGroup());
			group.Children.Add(ILayoutControl.CreateGroup());
			foreach (FrameworkElement tab in group.GetLogicalChildren(false))
				ILayoutControl.InitNewElement(tab);
		}
		protected internal virtual void OnDropElement(FrameworkElement element) {
			ProcessSelection(element, true);
		}
		protected internal virtual void OnStartDragAndDrop() {
			if (SelectionIndicators != null)
				SelectionIndicators.IsVisible = false;
			CustomizationToolbarElement = null;
		}
		protected internal virtual void OnEndDragAndDrop(bool accept) {
			Control.UpdateLayout();
			if (accept && SelectedElements.Count == 1)
				ILayoutControl.MakeControlVisible(SelectedElements[0]);
			if (SelectionIndicators != null)
				SelectionIndicators.IsVisible = true;
			ShowCustomizationToolbar();
			FocusControl();
		}
		protected void StartAvailableItemDragAndDrop(LayoutControlStartDragAndDropEventArgs<FrameworkElement> arguments) {
			Controller.StartItemDragAndDrop(arguments.Data, arguments.MouseEventArgs, arguments.Source);
		}
		protected void StartNewItemDragAndDrop(LayoutControlStartDragAndDropEventArgs<LayoutControlNewItemInfo> arguments) {
			FrameworkElement item = CreateNewItem(arguments.Data);
			if (item == null)
				return;
			ILayoutControl.InitNewElement(item);
			Controller.StartItemDragAndDrop(item, arguments.MouseEventArgs, arguments.Source);
		}
		#endregion
	}
	public class LayoutControlCustomizationCover : Canvas {
		public LayoutControlCustomizationCover() {
			Background = new SolidColorBrush(Colors.Transparent);
		}
#if SILVERLIGHT
		public Geometry HitTestClip {
			get { return Clip; }
			set { Clip = value; }
		}
#else
		public Geometry HitTestClip { get; set; }
		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) {
			if (HitTestClip == null || HitTestClip.FillContains(hitTestParameters.HitPoint))
				return base.HitTestCore(hitTestParameters);
			else
				return null;
		}
#endif
	}
	[TemplatePart(Name = LeftSizingElementName, Type = typeof(ElementSizer))]
	[TemplatePart(Name = RightSizingElementName, Type = typeof(ElementSizer))]
	[TemplatePart(Name = TopSizingElementName, Type = typeof(ElementSizer))]
	[TemplatePart(Name = BottomSizingElementName, Type = typeof(ElementSizer))]
	public class LayoutItemSelectionIndicator : ControlBase {
		#region Dependency Properties
		public static readonly DependencyProperty AllowSizingProperty =
			DependencyProperty.Register("AllowSizing", typeof(bool), typeof(LayoutItemSelectionIndicator), null);
		public static readonly DependencyProperty ElementProperty =
			DependencyProperty.Register("Element", typeof(FrameworkElement), typeof(LayoutItemSelectionIndicator),
				new PropertyMetadata((o, e) => ((LayoutItemSelectionIndicator)o).OnElementChanged()));
		public static readonly DependencyProperty ElementSizeProperty =
			DependencyProperty.Register("ElementSize", typeof(object), typeof(LayoutItemSelectionIndicator), null);
		public static readonly DependencyProperty IsSizingProperty =
			DependencyProperty.Register("IsSizing", typeof(bool), typeof(LayoutItemSelectionIndicator), null);
		public static readonly DependencyProperty IsHorizontalSizingProperty =
			DependencyProperty.Register("IsHorizontalSizing", typeof(bool), typeof(LayoutItemSelectionIndicator), null);
		public static readonly DependencyProperty IsVerticalSizingProperty =
			DependencyProperty.Register("IsVerticalSizing", typeof(bool), typeof(LayoutItemSelectionIndicator), null);
		#endregion Dependency Properties
		private ILayoutControl _LayoutControl;
		public LayoutItemSelectionIndicator() {
			DefaultStyleKey = typeof(LayoutItemSelectionIndicator);
		}
		public void UpdateBounds() {
			if (Element != null && Parent != null)
				this.SetBounds(CalculateBounds());
		}
		public bool AllowSizing {
			get { return (bool)GetValue(AllowSizingProperty); }
		}
		public FrameworkElement Element {
			get { return (FrameworkElement)GetValue(ElementProperty); }
			set { SetValue(ElementProperty, value); }
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Code Defects", "DXCA001")]
		public double ElementSize {
			get { return (double)GetValue(ElementSizeProperty); }
			protected set { SetValue(ElementSizeProperty, value); }
		}
		public bool IsSizing {
			get { return (bool)GetValue(IsSizingProperty); }
			protected set { SetValue(IsSizingProperty, value); }
		}
		public bool IsHorizontalSizing {
			get { return (bool)GetValue(IsHorizontalSizingProperty); }
			protected set { SetValue(IsHorizontalSizingProperty, value); }
		}
		public bool IsVerticalSizing {
			get { return (bool)GetValue(IsVerticalSizingProperty); }
			protected set { SetValue(IsVerticalSizingProperty, value); }
		}
		public ILayoutControl LayoutControl {
			get { return _LayoutControl; }
			internal set {
				if (LayoutControl == value)
					return;
				_LayoutControl = value;
				SetAllowSizing();
			}
		}
		#region Template
		const string LeftSizingElementName = "LeftSizingElement";
		const string RightSizingElementName = "RightSizingElement";
		const string TopSizingElementName = "TopSizingElement";
		const string BottomSizingElementName = "BottomSizingElement";
		public override void OnApplyTemplate() {
			DetachEventHandlers(LeftSizingElement);
			DetachEventHandlers(RightSizingElement);
			DetachEventHandlers(TopSizingElement);
			DetachEventHandlers(BottomSizingElement);
			base.OnApplyTemplate();
			LeftSizingElement = GetTemplateChild(LeftSizingElementName) as ElementSizer;
			RightSizingElement = GetTemplateChild(RightSizingElementName) as ElementSizer;
			TopSizingElement = GetTemplateChild(TopSizingElementName) as ElementSizer;
			BottomSizingElement = GetTemplateChild(BottomSizingElementName) as ElementSizer;
			AttachEventHandlers(LeftSizingElement);
			AttachEventHandlers(RightSizingElement);
			AttachEventHandlers(TopSizingElement);
			AttachEventHandlers(BottomSizingElement);
		}
		protected ElementSizer LeftSizingElement { get; private set; }
		protected ElementSizer RightSizingElement { get; private set; }
		protected ElementSizer TopSizingElement { get; private set; }
		protected ElementSizer BottomSizingElement { get; private set; }
		private void AttachEventHandlers(ElementSizer sizer) {
			if (sizer == null)
				return;
			sizer.ElementSizeChanging += OnElementSizeChanging;
			sizer.IsSizingChanged += OnIsSizingChanged;
		}
		private void DetachEventHandlers(ElementSizer sizer) {
			if (sizer == null)
				return;
			sizer.ElementSizeChanging -= OnElementSizeChanging;
			sizer.IsSizingChanged -= OnIsSizingChanged;
		}
		private void OnElementSizeChanging(object sender, EventArgs e) {
			OnElementSizeChanging(((ElementSizer)sender).ElementSize);
		}
		private void OnIsSizingChanged(object sender, EventArgs e) {
			var sizer = (ElementSizer)sender;
			OnIsSizingChanged(sizer.IsSizing, sizer.Side, sizer.ElementSize);
		}
		#endregion Template
		protected virtual Rect CalculateBounds() {
			if (Element == LayoutControl)
				return Element.MapRect(((ILayoutControl)Element).ContentBounds, (FrameworkElement)Parent);
			else
				return Element.GetVisualBounds((FrameworkElement)Parent, false);
		}
		protected virtual void OnElementChanged() {
			SetAllowSizing();
			UpdateBounds();
		}
		protected virtual void OnElementSizeChanging(double elementSize) {
			ElementSize = elementSize;
		}
		protected virtual void OnIsSizingChanged(bool isSizing, Side side, double elementSize) {
			IsSizing = isSizing;
			IsHorizontalSizing = isSizing && (side == Side.Left || side == Side.Right);
			IsVerticalSizing = isSizing && (side == Side.Top || side == Side.Bottom);
			ElementSize = elementSize;
		}
		protected virtual void SetAllowSizing() {
			if (LayoutControl != null && Element != LayoutControl)
				SetBinding(AllowSizingProperty, new Binding("AllowItemSizingDuringCustomization") { Source = LayoutControl });
			else
				ClearValue(AllowSizingProperty);
		}
	}
	public class LayoutItemSelectionIndicators : ElementPool<LayoutItemSelectionIndicator> {
		private bool _IsVisible = true;
		public LayoutItemSelectionIndicators(Panel container, ILayoutControl layoutControl)
			: base(container) {
			LayoutControl = layoutControl;
		}
		public void Update(FrameworkElements selectedElements) {
			MarkItemsAsUnused();
			foreach (var element in selectedElements)
				Add().Element = element;
			DeleteUnusedItems();
		}
		public void UpdateBounds() {
			if (IsVisible)
				Items.ForEach(item => item.UpdateBounds());
		}
		public bool IsVisible {
			get { return _IsVisible; }
			set {
				if (IsVisible == value)
					return;
				_IsVisible = value;
				Items.ForEach(item => item.SetVisible(value));
				if (IsVisible)
					UpdateBounds();
			}
		}
		public ILayoutControl LayoutControl { get; private set; }
		protected override LayoutItemSelectionIndicator CreateItem() {
			var result = base.CreateItem();
			result.LayoutControl = LayoutControl;
			result.SetVisible(IsVisible);
			return result;
		}
		protected override void DeleteItem(LayoutItemSelectionIndicator item) {
			item.LayoutControl = null;
			base.DeleteItem(item);
		}
	}
	public class LayoutItemParentIndicator : LayoutItemSelectionIndicator {
		public LayoutItemParentIndicator() {
			DefaultStyleKey = typeof(LayoutItemParentIndicator);
		}
	}
	[TemplatePart(Name = AddNewItemElementName, Type = typeof(Button))]
	[TemplatePart(Name = HorizontalAlignmentElementName, Type = typeof(LayoutItemAlignmentControl))]
	[TemplatePart(Name = VerticalAlignmentElementName, Type = typeof(LayoutItemAlignmentControl))]
	[TemplatePart(Name = MoveItemBackwardElementName, Type = typeof(Button))]
	[TemplatePart(Name = MoveItemForwardElementName, Type = typeof(Button))]
	[TemplatePart(Name = MoveToAvailableItemsElementName, Type = typeof(Button))]
	[TemplatePart(Name = RenameElementName, Type=typeof(Button))]
	[TemplatePart(Name = RenamingEditElementName, Type = typeof(TextBox))]
	[TemplatePart(Name = SelectParentElementName, Type = typeof(Button))]
	public class LayoutItemCustomizationToolbar : ControlBase {
		#region Dependency Properties
		public static readonly DependencyProperty AvailableItemsUIVisibilityProperty =
			DependencyProperty.Register("AvailableItemsUIVisibility", typeof(Visibility), typeof(LayoutItemCustomizationToolbar), null);
		public static readonly DependencyProperty CanMoveItemBackwardProperty =
			DependencyProperty.Register("CanMoveItemBackward", typeof(bool), typeof(LayoutItemCustomizationToolbar), null);
		public static readonly DependencyProperty CanMoveItemForwardProperty =
			DependencyProperty.Register("CanMoveItemForward", typeof(bool), typeof(LayoutItemCustomizationToolbar), null);
		public static readonly DependencyProperty ItemHeaderProperty =
			DependencyProperty.Register("ItemHeader", typeof(object), typeof(LayoutItemCustomizationToolbar),
				new PropertyMetadata((o, e) => ((LayoutItemCustomizationToolbar)o).OnItemHeaderChanged()));
		public static readonly DependencyProperty ItemMovingDirectionProperty =
			DependencyProperty.Register("ItemMovingDirection", typeof(Orientation), typeof(LayoutItemCustomizationToolbar),
				new PropertyMetadata((o, e) => ((LayoutItemCustomizationToolbar)o).OnItemMovingDirectionChanged()));
		public static readonly DependencyProperty ItemMovingBackwardDirectionProperty =
			DependencyProperty.Register("ItemMovingBackwardDirection", typeof(Side), typeof(LayoutItemCustomizationToolbar), null);
		public static readonly DependencyProperty ItemMovingForwardDirectionProperty =
			DependencyProperty.Register("ItemMovingForwardDirection", typeof(Side), typeof(LayoutItemCustomizationToolbar), null);
		public static readonly DependencyProperty ItemMovingUIVisibilityProperty =
			DependencyProperty.Register("ItemMovingUIVisibility", typeof(Visibility), typeof(LayoutItemCustomizationToolbar), null);
		public static readonly DependencyProperty ItemRenamingUIVisibilityProperty =
			DependencyProperty.Register("ItemRenamingUIVisibility", typeof(Visibility), typeof(LayoutItemCustomizationToolbar), null);
		public static readonly DependencyProperty NewItemsUIVisibilityProperty =
			DependencyProperty.Register("NewItemsUIVisibility", typeof(Visibility), typeof(LayoutItemCustomizationToolbar), null);
		#endregion Dependency Properties
		private HorizontalAlignment? _ItemHorizontalAlignment;
		private VerticalAlignment? _ItemVerticalAlignment;
		public LayoutItemCustomizationToolbar() {
			DefaultStyleKey = typeof(LayoutItemCustomizationToolbar);
			OnItemMovingDirectionChanged();
		}
		public Visibility AvailableItemsUIVisibility {
			get { return (Visibility)GetValue(AvailableItemsUIVisibilityProperty); }
			set { SetValue(AvailableItemsUIVisibilityProperty, value); }
		}
		public bool CanMoveItemBackward {
			get { return (bool)GetValue(CanMoveItemBackwardProperty); }
			set { SetValue(CanMoveItemBackwardProperty, value); }
		}
		public bool CanMoveItemForward {
			get { return (bool)GetValue(CanMoveItemForwardProperty); }
			set { SetValue(CanMoveItemForwardProperty, value); }
		}
		public bool IsInitializing { get; set; }
		public object ItemHeader {
			get { return GetValue(ItemHeaderProperty); }
			set { SetValue(ItemHeaderProperty, value); }
		}
		public HorizontalAlignment? ItemHorizontalAlignment {
			get { return _ItemHorizontalAlignment; }
			set {
				if (ItemHorizontalAlignment == value)
					return;
				_ItemHorizontalAlignment = value;
				UpdateTemplate();
				OnItemHorizontalAlignmentChanged();
			}
		}
		public VerticalAlignment? ItemVerticalAlignment {
			get { return _ItemVerticalAlignment; }
			set {
				if (ItemVerticalAlignment == value)
					return;
				_ItemVerticalAlignment = value;
				UpdateTemplate();
				OnItemVerticalAlignmentChanged();
			}
		}
		public Orientation ItemMovingDirection {
			get { return (Orientation)GetValue(ItemMovingDirectionProperty); }
			set { SetValue(ItemMovingDirectionProperty, value); }
		}
		public Side ItemMovingBackwardDirection {
			get { return (Side)GetValue(ItemMovingBackwardDirectionProperty); }
			set { SetValue(ItemMovingBackwardDirectionProperty, value); }
		}
		public Side ItemMovingForwardDirection {
			get { return (Side)GetValue(ItemMovingForwardDirectionProperty); }
			set { SetValue(ItemMovingForwardDirectionProperty, value); }
		}
		public Visibility ItemMovingUIVisibility {
			get { return (Visibility)GetValue(ItemMovingUIVisibilityProperty); }
			set { SetValue(ItemMovingUIVisibilityProperty, value); }
		}
		public Visibility ItemRenamingUIVisibility {
			get { return (Visibility)GetValue(ItemRenamingUIVisibilityProperty); }
			set { SetValue(ItemRenamingUIVisibilityProperty, value); }
		}
		public Visibility NewItemsUIVisibility {
			get { return (Visibility)GetValue(NewItemsUIVisibilityProperty); }
			set { SetValue(NewItemsUIVisibilityProperty, value); }
		}
		public event Action AddNewItem;
		public event Action ItemHeaderChanged;
		public event Action ItemHorizontalAlignmentChanged;
		public event Action ItemVerticalAlignmentChanged;
		public event Action<bool> MoveItem;
		public event Action MoveItemToAvailableItems;
		public event Action ReturnFocus;
		public event Action SelectItemParent;
		public event Action ShowItemParentIndicator;
		public event Action HideItemParentIndicator;
		#region Template
		const string AddNewItemElementName = "AddNewItemElement";
		const string HorizontalAlignmentElementName = "HorizontalAlignmentElement";
		const string VerticalAlignmentElementName = "VerticalAlignmentElement";
		const string MoveItemBackwardElementName = "MoveItemBackwardElement";
		const string MoveItemForwardElementName = "MoveItemForwardElement";
		const string MoveToAvailableItemsElementName = "MoveToAvailableItemsElement";
		const string RenameElementName = "RenameElement";
		const string RenamingEditElementName = "RenamingEditElement";
		const string SelectParentElementName = "SelectParentElement";
		public override void OnApplyTemplate() {
			if (AddNewItemElement != null)
				AddNewItemElement.Click -= OnAddNewItemElementClick;
			if (HorizontalAlignmentElement != null)
				HorizontalAlignmentElement.AlignmentChanged -= OnHorizontalAlignmentElementAlignmentChanged;
			if (VerticalAlignmentElement != null)
				VerticalAlignmentElement.AlignmentChanged -= OnVerticalAlignmentElementAlignmentChanged;
			if (MoveItemBackwardElement != null)
				MoveItemBackwardElement.Click -= OnMoveItemBackwardElementClick;
			if (MoveItemForwardElement != null)
				MoveItemForwardElement.Click -= OnMoveItemForwardElementClick;
			if (MoveToAvailableItemsElement != null)
				MoveToAvailableItemsElement.Click -= OnMoveToAvailableItemsElementClick;
			if (RenameElement != null)
				RenameElement.Click -= OnRenameElementClick;
			if (RenamingEditElement != null) {
				RenamingEditElement.KeyDown -= OnRenamingEditElementKeyDown;
				RenamingEditElement.LostFocus -= OnRenamingEditElementLostFocus;
				RenamingEditElement.TextChanged -= OnRenamingEditElementTextChanged;
			}
			if (SelectParentElement != null) {
				SelectParentElement.Click -= OnSelectParentElementClick;
				SelectParentElement.MouseEnter -= OnSelectParentElementMouseEnter;
				SelectParentElement.MouseLeave -= OnSelectParentElementMouseLeave;
			}
			base.OnApplyTemplate();
			AddNewItemElement = GetTemplateChild(AddNewItemElementName) as Button;
			HorizontalAlignmentElement = GetTemplateChild(HorizontalAlignmentElementName) as LayoutItemAlignmentControl;
			VerticalAlignmentElement = GetTemplateChild(VerticalAlignmentElementName) as LayoutItemAlignmentControl;
			MoveItemBackwardElement = GetTemplateChild(MoveItemBackwardElementName) as Button;
			MoveItemForwardElement = GetTemplateChild(MoveItemForwardElementName) as Button;
			MoveToAvailableItemsElement = GetTemplateChild(MoveToAvailableItemsElementName) as Button;
			RenameElement = GetTemplateChild(RenameElementName) as Button;
			RenamingEditElement = GetTemplateChild(RenamingEditElementName) as TextBox;
			SelectParentElement = GetTemplateChild(SelectParentElementName) as Button;
			if (AddNewItemElement != null)
				AddNewItemElement.Click += OnAddNewItemElementClick;
			if (HorizontalAlignmentElement != null)
				HorizontalAlignmentElement.AlignmentChanged += OnHorizontalAlignmentElementAlignmentChanged;
			if (VerticalAlignmentElement != null)
				VerticalAlignmentElement.AlignmentChanged += OnVerticalAlignmentElementAlignmentChanged;
			if (MoveItemBackwardElement != null)
				MoveItemBackwardElement.Click += OnMoveItemBackwardElementClick;
			if (MoveItemForwardElement != null)
				MoveItemForwardElement.Click += OnMoveItemForwardElementClick;
			if (MoveToAvailableItemsElement != null)
				MoveToAvailableItemsElement.Click += OnMoveToAvailableItemsElementClick;
			if (RenameElement != null)
				RenameElement.Click += OnRenameElementClick;
			if (RenamingEditElement != null) {
				RenamingEditElement.KeyDown += OnRenamingEditElementKeyDown;
				RenamingEditElement.LostFocus += OnRenamingEditElementLostFocus;
				RenamingEditElement.TextChanged += OnRenamingEditElementTextChanged;
			}
			if (SelectParentElement != null) {
				SelectParentElement.Click += OnSelectParentElementClick;
				SelectParentElement.MouseEnter += OnSelectParentElementMouseEnter;
				SelectParentElement.MouseLeave += OnSelectParentElementMouseLeave;
			}
			UpdateTemplate();
		}
		protected virtual void UpdateTemplate() {
			if (HorizontalAlignmentElement != null)
				if (ItemHorizontalAlignment == null)
					HorizontalAlignmentElement.Alignment = null;
				else
					HorizontalAlignmentElement.Alignment = ItemHorizontalAlignment.Value.GetItemAlignment();
			if (VerticalAlignmentElement != null)
				if (ItemVerticalAlignment == null)
					VerticalAlignmentElement.Alignment = null;
				else
					VerticalAlignmentElement.Alignment = ItemVerticalAlignment.Value.GetItemAlignment();
		}
		protected Button AddNewItemElement { get; private set; }
		protected LayoutItemAlignmentControl HorizontalAlignmentElement { get; private set; }
		protected LayoutItemAlignmentControl VerticalAlignmentElement { get; private set; }
		protected Button MoveItemBackwardElement { get; private set; }
		protected Button MoveItemForwardElement { get; private set; }
		protected Button MoveToAvailableItemsElement { get; private set; }
		protected Button RenameElement { get; private set; }
		protected TextBox RenamingEditElement { get; private set; }
		protected Button SelectParentElement { get; private set; }
		private object _OriginalItemHeader;
		private object _StoredRenameElementVisibility;
		private void OnAddNewItemElementClick(object sender, RoutedEventArgs e) {
			OnAddNewItem();
		}
		private void OnHorizontalAlignmentElementAlignmentChanged() {
			if (HorizontalAlignmentElement.Alignment == null)
				ItemHorizontalAlignment = null;
			else
				ItemHorizontalAlignment = HorizontalAlignmentElement.Alignment.Value.GetHorizontalAlignment();
		}
		private void OnVerticalAlignmentElementAlignmentChanged() {
			if (VerticalAlignmentElement.Alignment == null)
				ItemVerticalAlignment = null;
			else
				ItemVerticalAlignment = VerticalAlignmentElement.Alignment.Value.GetVerticalAlignment();
		}
		private void OnMoveItemBackwardElementClick(object sender, RoutedEventArgs e) {
			OnMoveItem(false);
		}
		private void OnMoveItemForwardElementClick(object sender, RoutedEventArgs e) {
			OnMoveItem(true);
		}
		private void OnMoveToAvailableItemsElementClick(object sender, RoutedEventArgs e) {
			OnMoveItemToAvailableItems();
		}
		private void OnRenameElementClick(object sender, RoutedEventArgs e) {
			OnRenameItem();
		}
		private void OnRenamingEditElementKeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter || e.Key == Key.Escape) {
				HideRenamingEdit(e.Key == Key.Enter);
				OnReturnFocus();
			}
		}
		private void OnRenamingEditElementLostFocus(object sender, RoutedEventArgs e) {
			HideRenamingEdit(true);
		}
		private void OnRenamingEditElementTextChanged(object sender, TextChangedEventArgs e) {
			BindingExpression binding = RenamingEditElement.GetBindingExpression(TextBox.TextProperty);
			if (binding != null)
				binding.UpdateSource();
		}
		private void OnSelectParentElementClick(object sender, RoutedEventArgs e) {
			OnHideItemParentIndicator();
			OnSelectItemParent();
		}
		private void OnSelectParentElementMouseEnter(object sender, MouseEventArgs e) {
			OnShowItemParentIndicator();
		}
		private void OnSelectParentElementMouseLeave(object sender, MouseEventArgs e) {
			OnHideItemParentIndicator();
		}
		private void ShowRenamingEdit() {
			if (RenamingEditElement == null || RenamingEditElement.Visibility == Visibility.Visible)
				return;
			_OriginalItemHeader = ItemHeader;
			RenamingEditElement.SetBinding(TextBox.TextProperty,
				new Binding("ItemHeader") { Source = this, Mode = BindingMode.TwoWay });
			RenamingEditElement.SelectionStart = RenamingEditElement.Text.Length;
			if (RenameElement != null) {
				_StoredRenameElementVisibility = RenameElement.StorePropertyValue(UIElement.VisibilityProperty);
				RenameElement.Visibility = Visibility.Collapsed;
			}
			RenamingEditElement.Visibility = Visibility.Visible;
			RenamingEditElement.Focus();
		}
		private void HideRenamingEdit(bool accept) {
			if (RenamingEditElement == null || RenamingEditElement.Visibility == Visibility.Collapsed)
				return;
			RenamingEditElement.Visibility = Visibility.Collapsed;
			RenamingEditElement.ClearValue(TextBox.TextProperty);
			if (RenameElement != null && _StoredRenameElementVisibility != null) {
				RenameElement.RestorePropertyValue(UIElement.VisibilityProperty, _StoredRenameElementVisibility);
				_StoredRenameElementVisibility = null;
			}
			if (!accept)
				ItemHeader = _OriginalItemHeader;
		}
		#endregion Template
		protected virtual void OnAddNewItem() {
			if (AddNewItem != null)
				AddNewItem();
		}
		protected virtual void OnItemHeaderChanged() {
			if (!IsInitializing && ItemHeaderChanged != null)
				ItemHeaderChanged();
		}
		protected virtual void OnItemHorizontalAlignmentChanged() {
			if (!IsInitializing && ItemHorizontalAlignmentChanged != null)
				ItemHorizontalAlignmentChanged();
		}
		protected virtual void OnItemVerticalAlignmentChanged() {
			if (!IsInitializing && ItemVerticalAlignmentChanged != null)
				ItemVerticalAlignmentChanged();
		}
		protected virtual void OnItemMovingDirectionChanged() {
			ItemMovingBackwardDirection = ItemMovingDirection == Orientation.Horizontal ? Side.Left : Side.Top;
			ItemMovingForwardDirection = ItemMovingDirection == Orientation.Horizontal ? Side.Right : Side.Bottom;
		}
		protected internal virtual void OnHide() {
			OnHideItemParentIndicator();
		}
		protected virtual void OnMoveItem(bool forward) {
			OnReturnFocus();
			if (MoveItem != null)
				MoveItem(forward);
		}
		protected virtual void OnMoveItemToAvailableItems() {
			if (MoveItemToAvailableItems != null)
				MoveItemToAvailableItems();
		}
		protected virtual void OnRenameItem() {
			ShowRenamingEdit();
		}
		protected virtual void OnReturnFocus() {
			if (ReturnFocus != null)
				ReturnFocus();
		}
		protected virtual void OnSelectItemParent() {
			if (SelectItemParent != null)
				SelectItemParent();
		}
		protected virtual void OnShowItemParentIndicator() {
			if (ShowItemParentIndicator != null)
				ShowItemParentIndicator();
		}
		protected virtual void OnHideItemParentIndicator() {
			if (HideItemParentIndicator != null)
				HideItemParentIndicator();
		}
	}
	public enum LayoutItemAlignmentButtonKind { Left, HorizontalCenter, Right, HorizontalStretch, Top, VerticalCenter, Bottom, VerticalStretch }
	[TemplatePart(Name = RootElementName, Type = typeof(Border))]
	[TemplatePart(Name = GlyphLeftName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = GlyphHorizontalCenterName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = GlyphRightName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = GlyphHorizontalStretchName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = GlyphTopName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = GlyphVerticalCenterName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = GlyphBottomName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = GlyphVerticalStretchName, Type = typeof(FrameworkElement))]
	public class LayoutItemAlignmentButton : RadioButton {
		#region Dependency Properties
		public static readonly DependencyProperty CornerRadiusProperty =
			DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(LayoutItemAlignmentButton), null);
		public static readonly DependencyProperty KindProperty =
			DependencyProperty.Register("Kind", typeof(LayoutItemAlignmentButtonKind), typeof(LayoutItemAlignmentButton),
				new PropertyMetadata((o, e) => ((LayoutItemAlignmentButton)o).OnKindChanged()));
		#endregion Dependency Properties
		public LayoutItemAlignmentButton() {
			DefaultStyleKey = typeof(LayoutItemAlignmentButton);
		}
		public CornerRadius CornerRadius {
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}
		public LayoutItemAlignmentButtonKind Kind {
			get { return (LayoutItemAlignmentButtonKind)GetValue(KindProperty); }
			set { SetValue(KindProperty, value); }
		}
		#region Template
		const string RootElementName = "RootElement";
		const string GlyphBaseName = "Glyph";
		const string GlyphLeftName = GlyphBaseName + "Left";
		const string GlyphHorizontalCenterName = GlyphBaseName + "HorizontalCenter";
		const string GlyphRightName = GlyphBaseName + "Right";
		const string GlyphHorizontalStretchName = GlyphBaseName + "HorizontalStretch";
		const string GlyphTopName = GlyphBaseName + "Top";
		const string GlyphVerticalCenterName = GlyphBaseName + "VerticalCenter";
		const string GlyphBottomName = GlyphBaseName + "Bottom";
		const string GlyphVerticalStretchName = GlyphBaseName + "VerticalStretch";
		public override void OnApplyTemplate() {
			if (RootElement != null)
				BorderExtensions.SetClipChild(RootElement, false);
			base.OnApplyTemplate();
			RootElement = GetTemplateChild(RootElementName) as Border;
			if (Glyphs == null)
				Glyphs = new Dictionary<LayoutItemAlignmentButtonKind, FrameworkElement>();
			for (var kind = LayoutItemAlignmentButtonKind.Left; kind <= LayoutItemAlignmentButtonKind.VerticalStretch; kind++)
				Glyphs[kind] = GetTemplateChild(GlyphBaseName + kind.ToString()) as FrameworkElement;
			if (RootElement != null)
				BorderExtensions.SetClipChild(RootElement, true);
			UpdateTemplate();
		}
		protected virtual void UpdateTemplate() {
			if (Glyphs == null)
				return;
			for (var kind = LayoutItemAlignmentButtonKind.Left; kind <= LayoutItemAlignmentButtonKind.VerticalStretch; kind++) {
				FrameworkElement glyph = Glyphs[kind];
				if (glyph != null)
					glyph.SetVisible(kind == Kind);
			}
		}
		protected Dictionary<LayoutItemAlignmentButtonKind, FrameworkElement> Glyphs { get; private set; }
		protected Border RootElement { get; private set; }
		#endregion Template
		protected virtual void OnKindChanged() {
			UpdateTemplate();
		}
	}
	[TemplatePart(Name = HorizontalRootElementName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = HorizontalStartElementName, Type = typeof(RadioButton))]
	[TemplatePart(Name = HorizontalCenterElementName, Type = typeof(RadioButton))]
	[TemplatePart(Name = HorizontalEndElementName, Type = typeof(RadioButton))]
	[TemplatePart(Name = HorizontalStretchElementName, Type = typeof(RadioButton))]
	[TemplatePart(Name = VerticalRootElementName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = VerticalStartElementName, Type = typeof(RadioButton))]
	[TemplatePart(Name = VerticalCenterElementName, Type = typeof(RadioButton))]
	[TemplatePart(Name = VerticalEndElementName, Type = typeof(RadioButton))]
	[TemplatePart(Name = VerticalStretchElementName, Type = typeof(RadioButton))]
	public class LayoutItemAlignmentControl : ControlBase {
		#region Dependency Properties
		public static readonly DependencyProperty AlignmentProperty =
			DependencyProperty.Register("Alignment", typeof(ItemAlignment?), typeof(LayoutItemAlignmentControl),
				new PropertyMetadata((o, e) => ((LayoutItemAlignmentControl)o).OnAlignmentChanged()));
		#endregion Dependency Properties
		private Orientation _Orientation;
		public LayoutItemAlignmentControl() {
			DefaultStyleKey = typeof(LayoutItemAlignmentControl);
		}
		public ItemAlignment? Alignment {
			get { return (ItemAlignment?)GetValue(AlignmentProperty); }
			set { SetValue(AlignmentProperty, value); }
		}
		public Orientation Orientation {
			get { return _Orientation; }
			set {
				if (Orientation == value)
					return;
				_Orientation = value;
				UpdateTemplate();
			}
		}
		public event Action AlignmentChanged;
		#region Template
		const string HorizontalRootElementName = "HorizontalRootElement";
		const string HorizontalStartElementName = "HorizontalStartElement";
		const string HorizontalCenterElementName = "HorizontalCenterElement";
		const string HorizontalEndElementName = "HorizontalEndElement";
		const string HorizontalStretchElementName = "HorizontalStretchElement";
		const string VerticalRootElementName = "VerticalRootElement";
		const string VerticalStartElementName = "VerticalStartElement";
		const string VerticalCenterElementName = "VerticalCenterElement";
		const string VerticalEndElementName = "VerticalEndElement";
		const string VerticalStretchElementName = "VerticalStretchElement";
		public override void OnApplyTemplate() {
			DetachEventHandlers(HorizontalStartElement);
			DetachEventHandlers(HorizontalCenterElement);
			DetachEventHandlers(HorizontalEndElement);
			DetachEventHandlers(HorizontalStretchElement);
			DetachEventHandlers(VerticalStartElement);
			DetachEventHandlers(VerticalCenterElement);
			DetachEventHandlers(VerticalEndElement);
			DetachEventHandlers(VerticalStretchElement);
			base.OnApplyTemplate();
			HorizontalRootElement = GetTemplateChild(HorizontalRootElementName) as FrameworkElement;
			HorizontalStartElement = GetTemplateChild(HorizontalStartElementName) as RadioButton;
			HorizontalCenterElement = GetTemplateChild(HorizontalCenterElementName) as RadioButton;
			HorizontalEndElement = GetTemplateChild(HorizontalEndElementName) as RadioButton;
			HorizontalStretchElement = GetTemplateChild(HorizontalStretchElementName) as RadioButton;
			VerticalRootElement = GetTemplateChild(VerticalRootElementName) as FrameworkElement;
			VerticalStartElement = GetTemplateChild(VerticalStartElementName) as RadioButton;
			VerticalCenterElement = GetTemplateChild(VerticalCenterElementName) as RadioButton;
			VerticalEndElement = GetTemplateChild(VerticalEndElementName) as RadioButton;
			VerticalStretchElement = GetTemplateChild(VerticalStretchElementName) as RadioButton;
			AttachEventHandlers(HorizontalStartElement);
			AttachEventHandlers(HorizontalCenterElement);
			AttachEventHandlers(HorizontalEndElement);
			AttachEventHandlers(HorizontalStretchElement);
			AttachEventHandlers(VerticalStartElement);
			AttachEventHandlers(VerticalCenterElement);
			AttachEventHandlers(VerticalEndElement);
			AttachEventHandlers(VerticalStretchElement);
			UpdateTemplate();
		}
		protected virtual void UpdateTemplate() {
			if (HorizontalRootElement != null)
				HorizontalRootElement.SetVisible(Orientation == Orientation.Horizontal);
			if (VerticalRootElement != null)
				VerticalRootElement.SetVisible(Orientation == Orientation.Vertical);
			UpdateIsChecked(HorizontalStartElement, HorizontalCenterElement, HorizontalEndElement, HorizontalStretchElement);
			UpdateIsChecked(VerticalStartElement, VerticalCenterElement, VerticalEndElement, VerticalStretchElement);
		}
		protected FrameworkElement HorizontalRootElement { get; private set; }
		protected RadioButton HorizontalStartElement { get; private set; }
		protected RadioButton HorizontalCenterElement { get; private set; }
		protected RadioButton HorizontalEndElement { get; private set; }
		protected RadioButton HorizontalStretchElement { get; private set; }
		protected FrameworkElement VerticalRootElement { get; private set; }
		protected RadioButton VerticalStartElement { get; private set; }
		protected RadioButton VerticalCenterElement { get; private set; }
		protected RadioButton VerticalEndElement { get; private set; }
		protected RadioButton VerticalStretchElement { get; private set; }
		private void AttachEventHandlers(RadioButton element) {
			if (element != null)
				element.Checked += ElementChecked;
		}
		private void DetachEventHandlers(RadioButton element) {
			if (element != null)
				element.Checked -= ElementChecked;
		}
		private void ElementChecked(object sender, RoutedEventArgs e) {
			if (sender == HorizontalStartElement || sender == VerticalStartElement)
				Alignment = ItemAlignment.Start;
			if (sender == HorizontalCenterElement || sender == VerticalCenterElement)
				Alignment = ItemAlignment.Center;
			if (sender == HorizontalEndElement || sender == VerticalEndElement)
				Alignment = ItemAlignment.End;
			if (sender == HorizontalStretchElement || sender == VerticalStretchElement)
				Alignment = ItemAlignment.Stretch;
		}
		private void UpdateIsChecked(RadioButton start, RadioButton center, RadioButton end, RadioButton stretch) {
			UpdateIsChecked(start, ItemAlignment.Start);
			UpdateIsChecked(center, ItemAlignment.Center);
			UpdateIsChecked(end, ItemAlignment.End);
			UpdateIsChecked(stretch, ItemAlignment.Stretch);
		}
		private void UpdateIsChecked(RadioButton element, ItemAlignment alignment) {
			if (element != null)
				element.IsChecked = Alignment == alignment;
		}
		#endregion Template
		protected virtual void OnAlignmentChanged() {
			UpdateTemplate();
			if (AlignmentChanged != null)
				AlignmentChanged();
		}
	}
	public class LayoutItemCustomizationToolbarButton : Button {
		#region Dependency Properties
		public static readonly DependencyProperty CornerRadiusProperty =
			DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(LayoutItemCustomizationToolbarButton), null);
		#endregion Dependency Properties
		public LayoutItemCustomizationToolbarButton() {
			DefaultStyleKey = typeof(LayoutItemCustomizationToolbarButton);
		}
		public CornerRadius CornerRadius {
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}
	}
	public class LayoutItemCustomizationToolbarButtonArrow : Control {
		#region Dependency Properties
		public static readonly DependencyProperty DefaultDirectionProperty =
			DependencyProperty.Register("DefaultDirection", typeof(Side), typeof(LayoutItemCustomizationToolbarButtonArrow),
				new PropertyMetadata((o, e) => ((LayoutItemCustomizationToolbarButtonArrow)o).OnDefaultDirectionChanged()));
		public static readonly DependencyProperty DirectionProperty =
			DependencyProperty.Register("Direction", typeof(Side), typeof(LayoutItemCustomizationToolbarButtonArrow),
				new PropertyMetadata((o, e) => ((LayoutItemCustomizationToolbarButtonArrow)o).OnDirectionChanged()));
		public static readonly DependencyProperty RotateTransformProperty =
			DependencyProperty.Register("RotateTransform", typeof(RotateTransform), typeof(LayoutItemCustomizationToolbarButtonArrow), null);
		#endregion Dependency Properties
		public LayoutItemCustomizationToolbarButtonArrow() {
			DefaultStyleKey = typeof(LayoutItemCustomizationToolbarButtonArrow);
		}
		public Side DefaultDirection {
			get { return (Side)GetValue(DefaultDirectionProperty); }
			set { SetValue(DefaultDirectionProperty, value); }
		}
		public Side Direction {
			get { return (Side)GetValue(DirectionProperty); }
			set { SetValue(DirectionProperty, value); }
		}
		public RotateTransform RotateTransform {
			get { return (RotateTransform)GetValue(RotateTransformProperty); }
			set { SetValue(RotateTransformProperty, value); }
		}
		protected virtual void OnAngleChanged(){
			UpdateRotateTransform();
		}
		protected virtual void OnDefaultDirectionChanged() {
			OnAngleChanged();
		}
		protected virtual void OnDirectionChanged() {
			OnAngleChanged();
		}
		protected void UpdateRotateTransform() {
			RotateTransform = new RotateTransform { Angle = this.Angle };
		}
		protected double Angle { get { return ((int)Direction - (int)DefaultDirection) * 90; } }
	}
	[TemplatePart(Name = AvailableItemsElementName, Type = typeof(LayoutControlAvailableItemsControl))]
	public class LayoutControlCustomizationControl : ControlBase {
		#region Dependency Properties
		public static readonly DependencyProperty AvailableItemsProperty =
			DependencyProperty.Register("AvailableItems", typeof(FrameworkElements), typeof(LayoutControlCustomizationControl), null);
		public static readonly DependencyProperty AvailableItemsUIVisibilityProperty =
			DependencyProperty.Register("AvailableItemsUIVisibility", typeof(Visibility), typeof(LayoutControlCustomizationControl), null);
		public static readonly DependencyProperty NewItemsInfoProperty =
			DependencyProperty.Register("NewItemsInfo", typeof(LayoutControlNewItemsInfo), typeof(LayoutControlCustomizationControl), null);
		public static readonly DependencyProperty NewItemsUIVisibilityProperty =
			DependencyProperty.Register("NewItemsUIVisibility", typeof(Visibility), typeof(LayoutControlCustomizationControl), null);
		#endregion Dependency Properties
		public LayoutControlCustomizationControl() {
			DefaultStyleKey = typeof(LayoutControlCustomizationControl);
#if !SILVERLIGHT
			FocusVisualStyle = null;
#endif
		}
		public FrameworkElements AvailableItems {
			get { return (FrameworkElements)GetValue(AvailableItemsProperty); }
			set { SetValue(AvailableItemsProperty, value); }
		}
		public Visibility AvailableItemsUIVisibility {
			get { return (Visibility)GetValue(AvailableItemsUIVisibilityProperty); }
			set { SetValue(AvailableItemsUIVisibilityProperty, value); }
		}
		public bool IsAvailableItemsListOpen {
			get { return AvailableItemsElement != null && AvailableItemsElement.IsListOpen; }
		}
		public LayoutControlNewItemsInfo NewItemsInfo {
			get { return (LayoutControlNewItemsInfo)GetValue(NewItemsInfoProperty); }
			set { SetValue(NewItemsInfoProperty, value); }
		}
		public Visibility NewItemsUIVisibility {
			get { return (Visibility)GetValue(NewItemsUIVisibilityProperty); }
			set { SetValue(NewItemsUIVisibilityProperty, value); }
		}
		public event Action<FrameworkElement> DeleteAvailableItem;
		public event EventHandler IsAvailableItemsListOpenChanged;
		public event EventHandler<LayoutControlStartDragAndDropEventArgs<FrameworkElement>> StartAvailableItemDragAndDrop;
		public event EventHandler<LayoutControlStartDragAndDropEventArgs<LayoutControlNewItemInfo>> StartNewItemDragAndDrop;
		#region Template
		const string AvailableItemsElementName = "AvailableItemsElement";
		public override void OnApplyTemplate() {
			if (AvailableItemsElement != null) {
				AvailableItemsElement.DeleteAvailableItem -= OnDeleteAvailableItem;
				AvailableItemsElement.IsListOpenChanged -= OnIsAvailableItemsListOpenChanged;
				AvailableItemsElement.StartAvailableItemDragAndDrop -= OnAvailableItemsStartAvailableItemDragAndDrop;
				AvailableItemsElement.StartNewItemDragAndDrop -= OnAvailableItemsStartNewItemDragAndDrop;
			}
			base.OnApplyTemplate();
			AvailableItemsElement = GetTemplateChild(AvailableItemsElementName) as LayoutControlAvailableItemsControl;
			if (AvailableItemsElement != null) {
				AvailableItemsElement.DeleteAvailableItem += OnDeleteAvailableItem;
				AvailableItemsElement.IsListOpenChanged += OnIsAvailableItemsListOpenChanged;
				AvailableItemsElement.StartAvailableItemDragAndDrop += OnAvailableItemsStartAvailableItemDragAndDrop;
				AvailableItemsElement.StartNewItemDragAndDrop += OnAvailableItemsStartNewItemDragAndDrop;
			}
		}
		protected LayoutControlAvailableItemsControl AvailableItemsElement { get; private set; }
		private void OnAvailableItemsStartAvailableItemDragAndDrop(object sender, LayoutControlStartDragAndDropEventArgs<FrameworkElement> e) {
			OnStartAvailableItemDragAndDrop(e);
		}
		private void OnAvailableItemsStartNewItemDragAndDrop(object sender, LayoutControlStartDragAndDropEventArgs<LayoutControlNewItemInfo> e) {
			OnStartNewItemDragAndDrop(e);
		}
		private void OnIsAvailableItemsListOpenChanged(object sender, EventArgs e) {
			OnIsAvailableItemsListOpenChanged();
		}
		#endregion Template
		protected virtual void OnDeleteAvailableItem(FrameworkElement item) {
			if (DeleteAvailableItem != null)
				DeleteAvailableItem(item);
		}
		protected virtual void OnIsAvailableItemsListOpenChanged() {
			if (IsAvailableItemsListOpenChanged != null)
				IsAvailableItemsListOpenChanged(this, EventArgs.Empty);
		}
		protected virtual void OnStartAvailableItemDragAndDrop(LayoutControlStartDragAndDropEventArgs<FrameworkElement> args) {
			if (StartAvailableItemDragAndDrop != null)
				StartAvailableItemDragAndDrop(this, args);
		}
		protected virtual void OnStartNewItemDragAndDrop(LayoutControlStartDragAndDropEventArgs<LayoutControlNewItemInfo> args) {
			if (StartNewItemDragAndDrop != null)
				StartNewItemDragAndDrop(this, args);
		}
	}
	public interface ILayoutControlAvailableItemsControl : IControl {
		bool IsListOpen { get; set; }
	}
	[TemplatePart(Name = AvailableItemsListElementName, Type = typeof(ListBox))]
	[TemplatePart(Name = NewItemsListElementName, Type = typeof(ListBox))]
	[TemplateVisualState(Name = ListClosedLayoutState, GroupName = LayoutStates)]
	[TemplateVisualState(Name = ListOpenLayoutState, GroupName = LayoutStates)]
	public class LayoutControlAvailableItemsControl : ControlBase, ILayoutControlAvailableItemsControl {
		#region Dependency Properties
		public static readonly DependencyProperty AvailableItemsProperty =
			DependencyProperty.Register("AvailableItems", typeof(FrameworkElements), typeof(LayoutControlAvailableItemsControl),
				new PropertyMetadata((o, e) => ((LayoutControlAvailableItemsControl)o).OnAvailableItemsChanged((FrameworkElements)e.OldValue)));
		public static readonly DependencyProperty IsListOpenProperty =
			DependencyProperty.Register("IsListOpen", typeof(bool), typeof(LayoutControlAvailableItemsControl),
				new PropertyMetadata((o, e) => ((LayoutControlAvailableItemsControl)o).OnIsListOpenChanged()));
		public static readonly DependencyProperty LayoutGroupItemTemplateProperty =
			DependencyProperty.Register("LayoutGroupItemTemplate", typeof(DataTemplate), typeof(LayoutControlAvailableItemsControl), null);
		public static readonly DependencyProperty NewItemsInfoProperty =
			DependencyProperty.Register("NewItemsInfo", typeof(LayoutControlNewItemsInfo), typeof(LayoutControlAvailableItemsControl),
				new PropertyMetadata((o, e) => ((LayoutControlAvailableItemsControl)o).OnNewItemsInfoChanged()));
		public static readonly DependencyProperty NewItemsUIVisibilityProperty =
			DependencyProperty.Register("NewItemsUIVisibility", typeof(Visibility), typeof(LayoutControlAvailableItemsControl), null);
		#endregion Dependency Properties
		public LayoutControlAvailableItemsControl() {
			DefaultStyleKey = typeof(LayoutControlAvailableItemsControl);
		}
		public FrameworkElements AvailableItems {
			get { return (FrameworkElements)GetValue(AvailableItemsProperty); }
			set { SetValue(AvailableItemsProperty, value); }
		}
		public bool IsListOpen {
			get { return (bool)GetValue(IsListOpenProperty); }
			set { SetValue(IsListOpenProperty, value); }
		}
		public DataTemplate LayoutGroupItemTemplate {
			get { return (DataTemplate)GetValue(LayoutGroupItemTemplateProperty); }
			set { SetValue(LayoutGroupItemTemplateProperty, value); }
		}
		public LayoutControlNewItemsInfo NewItemsInfo {
			get { return (LayoutControlNewItemsInfo)GetValue(NewItemsInfoProperty); }
			set { SetValue(NewItemsInfoProperty, value); }
		}
		public Visibility NewItemsUIVisibility {
			get { return (Visibility)GetValue(NewItemsUIVisibilityProperty); }
			set { SetValue(NewItemsUIVisibilityProperty, value); }
		}
		public event Action<FrameworkElement> DeleteAvailableItem;
		public event EventHandler IsListOpenChanged;
		public event EventHandler<LayoutControlStartDragAndDropEventArgs<FrameworkElement>> StartAvailableItemDragAndDrop;
		public event EventHandler<LayoutControlStartDragAndDropEventArgs<LayoutControlNewItemInfo>> StartNewItemDragAndDrop;
		#region Template
		const string AvailableItemsListElementName = "AvailableItemsListElement";
		const string NewItemsListElementName = "NewItemsListElement";
		const string LayoutStates = "LayoutStates";
		const string ListClosedLayoutState = "ListClosed";
		const string ListOpenLayoutState = "ListOpen";
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			AvailableItemsListElement = GetTemplateChild(AvailableItemsListElementName) as ListBox;
			NewItemsListElement = GetTemplateChild(NewItemsListElementName) as ListBox;
			UpdateAvailableItemsListElement(null);
			UpdateNewItemsListElement();
		}
		protected ListBox AvailableItemsListElement { get; private set; }
		protected ListBox NewItemsListElement { get; private set; }
		#endregion Template
		#region Keyboard and Mouse Handling
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			e.Handled = true;
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			e.Handled = true;
		}
		#endregion Keyboard and Mouse Handling
		protected override ControlControllerBase CreateController() {
			return new LayoutControlAvailableItemsController(this);
		}
		protected virtual AvailableItemInfo CreateAvailableItemInfo(FrameworkElement item) {
			return new AvailableItemInfo(item, GetAvailableItemLabel(item));
		}
		protected virtual LayoutControlAvailableListBoxItem CreateListItem() {
			return new LayoutControlAvailableListBoxItem(this);
		}
		protected LayoutControlAvailableListBoxItem CreateListItem(AvailableItemInfo itemInfo) {
			LayoutControlAvailableListBoxItem result = CreateListItem();
			InitListItem(result, itemInfo);
			result.Item = itemInfo.Item;
			return result;
		}
		protected virtual string GetAvailableItemDefaultLabel(FrameworkElement item) {
			return LayoutControl.GetCustomizationDefaultLabel(item);
		}
		protected AvailableItemInfo GetAvailableItemInfo(ListBoxItem listItem) {
			int itemIndex = AvailableItemsListElement.Items.IndexOf(listItem);
			return itemIndex != -1 ? AvailableItemsInfo[itemIndex] : null;
		}
		protected int GetAvailableItemInfoIndex(FrameworkElement item) {
			for (int i = 0; i < AvailableItemsInfo.Count; i++)
				if (AvailableItemsInfo[i].Item == item)
					return i;
			return -1;
		}
		protected virtual string GetAvailableItemLabel(FrameworkElement item) {
			string result = LayoutControl.GetCustomizationLabel(item) as string;
			if (string.IsNullOrEmpty(result))
				result = GetAvailableItemDefaultLabel(item);
			return result;
		}
		protected virtual List<AvailableItemInfo> GetAvailableItemsInfo() {
			var result = new List<AvailableItemInfo>();
			foreach (FrameworkElement item in AvailableItems)
				result.Add(CreateAvailableItemInfo(item));
			result.Sort(AvailableItemInfo.Compare);
			return result;
		}
		protected LayoutControlNewItemInfo GetNewItemInfo(ListBoxItem listItem) {
			int itemIndex = NewItemsListElement.Items.IndexOf(listItem);
			return itemIndex != -1 ? NewItemsInfo[itemIndex] : null;
		}
		protected int GetListItemIndex(FrameworkElement item) {
			for (int i = 0; i < AvailableItemsListElement.Items.Count; i++)
				if (((LayoutControlAvailableListBoxItem)AvailableItemsListElement.Items[i]).Item == item)
					return i;
			return -1;
		}
		protected virtual void InitListItem(LayoutControlAvailableListBoxItem listItem, AvailableItemInfo itemInfo) {
			listItem.Content = itemInfo.Label;
			if (itemInfo.Item.IsLayoutGroup() && LayoutGroupItemTemplate != null)
				listItem.ContentTemplate = LayoutGroupItemTemplate;
			if (LayoutControl.GetIsUserDefined(itemInfo.Item))
				listItem.SetBinding(LayoutControlAvailableListBoxItem.DeleteElementVisibilityProperty,
					new Binding("NewItemsUIVisibility") { Source = this });
			listItem.DeleteElementClick += (o, e) => OnDeleteAvailableItem(itemInfo.Item);
		}
		protected virtual void OnAvailableItemsChanged(NotifyCollectionChangedEventArgs args) {
			if (AvailableItems == null)
				AvailableItemsInfo = null;
			else
				if (args != null && (args.Action == NotifyCollectionChangedAction.Add || args.Action == NotifyCollectionChangedAction.Remove) &&
					AvailableItemsInfo != null)
					if (args.Action == NotifyCollectionChangedAction.Add)
						foreach (FrameworkElement item in args.NewItems) {
							AvailableItemInfo itemInfo = CreateAvailableItemInfo(item);
							AvailableItemsInfo.Insert(AvailableItemInfo.GetInsertionIndex(AvailableItemsInfo, itemInfo), itemInfo);
						}
					else
						foreach (FrameworkElement item in args.OldItems)
							AvailableItemsInfo.RemoveAt(GetAvailableItemInfoIndex(item));
				else
					AvailableItemsInfo = GetAvailableItemsInfo();
			UpdateAvailableItemsListElement(args);
		}
		protected virtual void OnDeleteAvailableItem(FrameworkElement item) {
			if (DeleteAvailableItem != null)
				DeleteAvailableItem(item);
		}
		protected virtual void OnIsListOpenChanged() {
			UpdateState(true);
			if (IsListOpenChanged != null)
				IsListOpenChanged(this, EventArgs.Empty);
		}
		protected virtual void OnNewItemsInfoChanged() {
			UpdateNewItemsListElement();
		}
		protected virtual void OnStartAvailableItemDragAndDrop(FrameworkElement item, MouseEventArgs mouseEventArgs, FrameworkElement source) {
			OnStartItemDragAndDrop();
			if (StartAvailableItemDragAndDrop != null)
				StartAvailableItemDragAndDrop(this, new LayoutControlStartDragAndDropEventArgs<FrameworkElement>(item, mouseEventArgs, source));
		}
		protected virtual void OnStartItemDragAndDrop() {
			IsListOpen = false;
			Controller.IsMouseEntered = false;
		}
		protected internal virtual void OnStartItemDragAndDrop(ListBoxItem listItem, MouseEventArgs arguments) {
			AvailableItemInfo availableItemInfo = GetAvailableItemInfo(listItem);
			if (availableItemInfo != null)
				OnStartAvailableItemDragAndDrop(availableItemInfo.Item, arguments, listItem);
			else {
				LayoutControlNewItemInfo newItemInfo = GetNewItemInfo(listItem);
				if (newItemInfo != null)
					OnStartNewItemDragAndDrop(newItemInfo, arguments, listItem);
			}
		}
		protected virtual void OnStartNewItemDragAndDrop(LayoutControlNewItemInfo itemInfo, MouseEventArgs mouseEventArgs, FrameworkElement source) {
			OnStartItemDragAndDrop();
			if (StartNewItemDragAndDrop != null)
				StartNewItemDragAndDrop(this, new LayoutControlStartDragAndDropEventArgs<LayoutControlNewItemInfo>(itemInfo, mouseEventArgs, source));
		}
		protected void UpdateAvailableItemsListElement(NotifyCollectionChangedEventArgs args) {
			if (AvailableItemsListElement == null)
				return;
			if (AvailableItemsInfo == null) {
				AvailableItemsListElement.Items.Clear();
				return;
			}
			if (args != null && (args.Action == NotifyCollectionChangedAction.Add || args.Action == NotifyCollectionChangedAction.Remove))
				if (args.Action == NotifyCollectionChangedAction.Add)
					foreach (FrameworkElement item in args.NewItems) {
						int index = GetAvailableItemInfoIndex(item);
						AvailableItemsListElement.Items.Insert(index, CreateListItem(AvailableItemsInfo[index]));
					}
				else
					foreach (FrameworkElement item in args.OldItems)
						AvailableItemsListElement.Items.RemoveAt(GetListItemIndex(item));
			else {
				AvailableItemsListElement.Items.Clear();
				foreach (AvailableItemInfo itemInfo in AvailableItemsInfo)
					AvailableItemsListElement.Items.Add(CreateListItem(itemInfo));
			}
		}
		protected void UpdateNewItemsListElement() {
			if (NewItemsListElement == null)
				return;
			NewItemsListElement.Items.Clear();
			if (NewItemsInfo == null)
				return;
			foreach (LayoutControlNewItemInfo itemInfo in NewItemsInfo) {
				ListBoxItem listItem = CreateListItem();
				listItem.Content = itemInfo.Label;
				NewItemsListElement.Items.Add(listItem);
			}
		}
		protected override void UpdateState(bool useTransitions) {
			base.UpdateState(useTransitions);
			GoToState(IsListOpen ? ListOpenLayoutState : ListClosedLayoutState, useTransitions);
		}
		protected List<AvailableItemInfo> AvailableItemsInfo { get; private set; }
		private void OnAvailableItemsChanged(FrameworkElements oldValue) {
			if (oldValue != null)
				oldValue.CollectionChanged -= OnAvailableItemsChanged;
			if (AvailableItems != null)
				AvailableItems.CollectionChanged += OnAvailableItemsChanged;
			OnAvailableItemsChanged(args: null);
		}
		private void OnAvailableItemsChanged(object sender, NotifyCollectionChangedEventArgs e) {
			OnAvailableItemsChanged(e);
		}
		protected class AvailableItemInfo {
			public static int Compare(AvailableItemInfo info1, AvailableItemInfo info2) {
				return string.Compare(info1.Label, info2.Label);
			}
			public static int GetInsertionIndex(List<AvailableItemInfo> sortedList, AvailableItemInfo info) {
				for (int i = 0; i < sortedList.Count; i++)
					if (Compare(sortedList[i], info) > 0)
						return i;
				return sortedList.Count;
			}
			public AvailableItemInfo(FrameworkElement item, string label) {
				Item = item;
				Label = label;
			}
			public FrameworkElement Item { get; private set; }
			public string Label { get; private set; }
		}
	}
	public class LayoutControlAvailableItemsController : ControlControllerBase {
		public LayoutControlAvailableItemsController(ILayoutControlAvailableItemsControl control) : base(control) { }
		public new ILayoutControlAvailableItemsControl IControl { get { return (ILayoutControlAvailableItemsControl)base.IControl; } }
		#region Keyboard and Mouse Handling
		protected override void OnIsMouseEnteredChanged() {
			base.OnIsMouseEnteredChanged();
			IControl.IsListOpen = IsMouseEntered;
		}
		#endregion Keyboard and Mouse Handling
	}
	[TemplatePart(Name = DeleteElementName, Type = typeof(Button))]
	public class LayoutControlAvailableListBoxItem : ListBoxItem {
		#region Dependency properties
		public static readonly DependencyProperty DeleteElementVisibilityProperty =
			DependencyProperty.Register("DeleteElementVisibility", typeof(Visibility), typeof(LayoutControlAvailableListBoxItem),
				new PropertyMetadata(Visibility.Collapsed));
		#endregion Dependency Properties
		private Point? _StartDragPoint;
		public LayoutControlAvailableListBoxItem(LayoutControlAvailableItemsControl owner) {
			Owner = owner;
		}
		public Visibility DeleteElementVisibility {
			get { return (Visibility)GetValue(DeleteElementVisibilityProperty); }
			set { SetValue(DeleteElementVisibilityProperty, value); }
		}
		public event EventHandler DeleteElementClick;
		#region Template
		const string DeleteElementName = "DeleteElement";
		public override void OnApplyTemplate() {
			if (DeleteElement != null)
				DeleteElement.Click -= OnDeleteElementClick;
			base.OnApplyTemplate();
			DeleteElement = GetTemplateChild(DeleteElementName) as Button;
			if (DeleteElement != null)
				DeleteElement.Click += OnDeleteElementClick;
		}
		protected Button DeleteElement { get; private set; }
		private void OnDeleteElementClick(object sender, RoutedEventArgs e) {
			OnDeleteElementClick();
		}
		#endregion Template
		protected virtual void OnDeleteElementClick() {
			if (DeleteElementClick != null)
				DeleteElementClick(this, EventArgs.Empty);
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			StartDragPoint = null;
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			StartDragPoint = e.GetPosition(this);
			e.Handled = true;
			base.OnMouseLeftButtonDown(e);
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			StartDragPoint = null;
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if (StartDragPoint != null) {
				StartDragPoint = null;
				VisualStateManager.GoToState(this, "Normal", false);
				StartDragAndDrop(e);
			}
		}
		protected void StartDragAndDrop(MouseEventArgs arguments) {
			if (Owner != null)
				Owner.OnStartItemDragAndDrop(this, arguments);
		}
		protected LayoutControlAvailableItemsControl Owner { get; private set; }
		protected Point? StartDragPoint {
			get { return _StartDragPoint; }
			private set {
				if (StartDragPoint == value)
					return;
				_StartDragPoint = value;
				if (StartDragPoint != null)
					CaptureMouse();
				else
					ReleaseMouseCapture();
			}
		}
		internal FrameworkElement Item { get; set; }
	}
}
