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

using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking.Platform;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.VisualElements {
	[DevExpress.Xpf.Core.DXToolboxBrowsable(false)]
	[TemplatePart(Name = "PART_DocumentSelectorPreview", Type = typeof(DocumentSelectorPreview))]
	[TemplatePart(Name = "PART_PanelsListBox", Type = typeof(DocumentSelectorListBox))]
	[TemplatePart(Name = "PART_DocumentsListBox", Type = typeof(DocumentSelectorListBox))]
	[TemplatePart(Name = "PART_PanelsListBoxCaption", Type = typeof(UIElement))]
	[TemplatePart(Name = "PART_DocumentsListBoxCaption", Type = typeof(UIElement))]
	public class DocumentSelector : psvControl, IControlHost {
		#region static
		public static readonly DependencyProperty PanelIndexProperty;
		public static readonly DependencyProperty DocumentIndexProperty;
		public static readonly DependencyProperty PanelsCaptionProperty;
		public static readonly DependencyProperty DocumentsCaptionProperty;
		static readonly DependencyPropertyKey SelectedItemPropertyKey;
		public static readonly DependencyProperty SelectedItemProperty;
		public static readonly DependencyProperty BindableDocumentIndexProperty;
		public static readonly DependencyProperty BindablePanelIndexProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty IsActiveProperty;
		static DocumentSelector() {
			var dProp = new DependencyPropertyRegistrator<DocumentSelector>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("PanelsCaption", ref PanelsCaptionProperty, string.Empty);
			dProp.Register("DocumentsCaption", ref DocumentsCaptionProperty, string.Empty);
			dProp.Register("PanelIndex", ref PanelIndexProperty, -1,
					(dObj, e) => ((DocumentSelector)dObj).OnPanelIndexChanged(),
					(dObj, value) => ((DocumentSelector)dObj).CoercePanelIndex(value));
			dProp.Register("DocumentIndex", ref DocumentIndexProperty, -1,
					(dObj, e) => ((DocumentSelector)dObj).OnDocumentIndexChanged(),
					(dObj, value) => ((DocumentSelector)dObj).CoerceDocumentIndex(value));
			dProp.RegisterReadonly("SelectedItem", ref SelectedItemPropertyKey, ref SelectedItemProperty, (LayoutPanel)null,
					(dObj, e) => ((DocumentSelector)dObj).OnSelectedItemChanged((LayoutPanel)e.NewValue),
					(dObj, value) => ((DocumentSelector)dObj).CoerceSelectedItem((LayoutPanel)value));
			dProp.Register("BindableDocumentIndex", ref BindableDocumentIndexProperty, -1,
				(dObj, e) => ((DocumentSelector)dObj).OnBindableDocumentIndexChanged((int)e.OldValue, (int)e.NewValue));
			dProp.Register("BindablePanelIndex", ref BindablePanelIndexProperty, -1,
				(dObj, e) => ((DocumentSelector)dObj).OnBindablePanelIndexChanged((int)e.OldValue, (int)e.NewValue));
			dProp.Register("IsActive", ref IsActiveProperty, false,
				(dObj, e) => ((DocumentSelector)dObj).OnIsActiveChanged((bool)e.OldValue, (bool)e.NewValue));
		}
		#endregion
		public DocumentSelector() {
			Panels = new ObservableCollection<LayoutPanel>();
			Documents = new ObservableCollection<LayoutPanel>();
			Documents.CollectionChanged += OnDocumentsCollectionChanged;
			Panels.CollectionChanged += OnPanelsCollectionChanged;
			SelectedItems = Documents;
		}
		protected override void OnDispose() {
			Documents.CollectionChanged -= OnDocumentsCollectionChanged;
			Panels.CollectionChanged -= OnPanelsCollectionChanged;
			if(PartPanelsListBox != null)
				PartPanelsListBox.MouseLeftButtonUp -= SelectorListBox_MouseLeftButtonUp;
			if(PartDocumentsListBox != null)
				PartDocumentsListBox.MouseLeftButtonUp -= SelectorListBox_MouseLeftButtonUp;
			base.OnDispose();
		}
		protected virtual void OnSelectedItemChanged(LayoutPanel value) {
			if(VisualContainer != null)
				DockLayoutManager.SetLayoutItem(VisualContainer, value);
			DockLayoutManager.SetLayoutItem(this, value);
		}
		protected virtual LayoutPanel CoerceSelectedItem(LayoutPanel value) {
			if(SelectedItems == null || SelectedItems.Count == 0 || SelectedIndex == -1) return null;
			return SelectedItems[SelectedIndex];
		}
		protected virtual void OnBindableDocumentIndexChanged(int oldValue, int newValue) {
			if(!IsDocumentIndexLocked) {
				if(newValue != -1) {
					DocumentIndex = newValue;
				}
			}
		}
		protected virtual void OnBindablePanelIndexChanged(int oldValue, int newValue) {
			if(!IsPanelIndexLocked) {
				if(newValue != -1) {
					PanelIndex = newValue;
				}
			}
		}
		public int SelectedIndex {
			get { return (SelectedItems == Panels) ? PanelIndex : DocumentIndex; }
			internal set {
				if(SelectedItems == Panels)
					PanelIndex = value;
				else
					DocumentIndex = value;
			}
		}
		public int BindableDocumentIndex {
			get { return (int)GetValue(BindableDocumentIndexProperty); }
			set { SetValue(BindableDocumentIndexProperty, value); }
		}
		public int BindablePanelIndex {
			get { return (int)GetValue(BindablePanelIndexProperty); }
			set { SetValue(BindablePanelIndexProperty, value); }
		}
		public int PanelIndex {
			get { return (int)GetValue(PanelIndexProperty); }
			set { SetValue(PanelIndexProperty, value); }
		}
		public int DocumentIndex {
			get { return (int)GetValue(DocumentIndexProperty); }
			set { SetValue(DocumentIndexProperty, value); }
		}
		public string PanelsCaption {
			get { return (string)GetValue(PanelsCaptionProperty); }
			set { SetValue(PanelsCaptionProperty, value); }
		}
		public string DocumentsCaption {
			get { return (string)GetValue(DocumentsCaptionProperty); }
			set { SetValue(DocumentsCaptionProperty, value); }
		}
		public LayoutPanel SelectedItem {
			get { return (LayoutPanel)GetValue(SelectedItemProperty); }
		}
		public bool HasItemsToShow {
			get { return Panels.Count + Documents.Count > 1; }
		}
		ObservableCollection<LayoutPanel> selectedItemsCore;
		public ObservableCollection<LayoutPanel> SelectedItems {
			get { return selectedItemsCore; }
			internal set { selectedItemsCore = value; }
		}
		public ObservableCollection<LayoutPanel> Panels { get; internal set; }
		public ObservableCollection<LayoutPanel> Documents { get; internal set; }
		public DocumentSelectorPreview PartDocumentSelectorPreview { get; private set; }
		public DocumentSelectorListBox PartPanelsListBox { get; private set; }
		public DocumentSelectorListBox PartDocumentsListBox { get; private set; }
		public UIElement PartPanelsListBoxCaption { get; private set; }
		public UIElement PartDocumentsListBoxCaption { get; private set; }
		protected internal bool IsCollectionChanging { get; set; }
		protected FloatingContainer FloatingContainer { get; set; }
		protected DependencyObject VisualContainer { get; set; }
		int selectedItemLock;
		bool IsSelectedItemLocked { get { return selectedItemLock > 0; } }
		protected virtual void OnIsActiveChanged(bool oldValue, bool newValue) {
			try {
				selectedItemLock++;
				if(!newValue) {
					CloseContainer();
				}
			}
			finally {
				selectedItemLock--;
			}
		}
		void OnPanelsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			UpdatePanelsVisibility();
		}
		void UpdatePanelsVisibility() {
			if(PartPanelsListBoxCaption != null)
				PartPanelsListBoxCaption.Visibility = GetVisibility(Panels);
			if(PartPanelsListBox != null)
				PartPanelsListBox.Visibility = GetVisibility(Panels);
		}
		void OnDocumentsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			UpdateDocumentsVisibility();
		}
		protected void UpdateDocumentsVisibility() {
			if(PartDocumentsListBox != null)
				PartDocumentsListBox.Visibility = GetVisibility(Documents);
			if(PartDocumentsListBoxCaption != null)
				PartDocumentsListBoxCaption.Visibility = GetVisibility(Documents);
		}
		protected override void OnLoaded() {
			FrameworkElement fe = VisualContainer as FrameworkElement;
			if(fe != null) {
				fe.PreviewKeyDown -= OnVisualContainerPreviewKeyDown;
				fe.PreviewKeyDown -= OnVisualContainerPreviewKeyUp;
			}
			ClearValue(IsActiveProperty);
			VisualContainer = DevExpress.Xpf.Core.Native.LayoutHelper.FindRoot(this);
			if(VisualContainer != null) {
				DockLayoutManager.SetLayoutItem(VisualContainer, SelectedItem);
				fe = VisualContainer as FrameworkElement;
				if(fe != null) {
					fe.PreviewKeyDown += OnVisualContainerPreviewKeyDown;
					fe.PreviewKeyUp += OnVisualContainerPreviewKeyUp;
				}
				SetBinding(IsActiveProperty, new System.Windows.Data.Binding() { Path = new PropertyPath(FloatingContainer.IsActiveProperty), Source = VisualContainer, Mode = System.Windows.Data.BindingMode.OneWay });
			}
			DockLayoutManager.SetLayoutItem(this, SelectedItem);
		}
		void OnVisualContainerPreviewKeyDown(object sender, KeyEventArgs e) {
			OnPreviewKeyDownCore(e);
		}
		void OnVisualContainerPreviewKeyUp(object sender, KeyEventArgs e) {
			OnPreviewKeyUpCore(e);
		}
		bool keyLeftCtrl, keyRightCtrl;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			keyLeftCtrl = Keyboard.IsKeyDown(Key.LeftCtrl);
			keyRightCtrl = Keyboard.IsKeyDown(Key.RightCtrl);
			FloatingContainer = FloatingContainer.GetFloatingContainer(this);
			PartDocumentSelectorPreview = GetTemplateChild("PART_DocumentSelectorPreview") as DocumentSelectorPreview;
			PartPanelsListBox = GetTemplateChild("PART_PanelsListBox") as DocumentSelectorListBox;
			if(PartPanelsListBox != null) {
				PartPanelsListBox.MouseLeftButtonUp += SelectorListBox_MouseLeftButtonUp;
			}
			PartDocumentsListBox = GetTemplateChild("PART_DocumentsListBox") as DocumentSelectorListBox;
			if(PartDocumentsListBox != null) {
				PartDocumentsListBox.MouseLeftButtonUp += SelectorListBox_MouseLeftButtonUp;
			}
			PartPanelsListBoxCaption = GetTemplateChild("PART_PanelsListBoxCaption") as UIElement;
			PartDocumentsListBoxCaption = GetTemplateChild("PART_DocumentsListBoxCaption") as UIElement;
			UpdateDocumentsVisibility();
			UpdatePanelsVisibility();
			SelectContent();
		}
		static Visibility GetVisibility(ICollection collection) {
			return collection.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
		}
		void SelectorListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			CloseContainer();
		}
		void CloseContainer() {
			BaseLayoutItem selected = SelectedItem;
			ClearValue(DocumentIndexProperty);
			ClearValue(PanelIndexProperty);
			if(PartDocumentSelectorPreview != null)
				PartDocumentSelectorPreview.OnClosing();
			if(FloatingContainer != null) {
				FloatingContainer.IsOpen = false;
				Container.RemoveFromLogicalTree(FloatingContainer, this);
			}
			Documents.Clear();
			Panels.Clear();
			if(!IsSelectedItemLocked) {
				Dispatcher.BeginInvoke(new System.Action(() =>
				{
					Container.Activate(selected);
					Container.Update();
				}));
			}
		}
		public void Close() {
			CloseContainer();
		}
		void OnPreviewKeyUpCore(KeyEventArgs e) {
			if((e.Key == Key.LeftCtrl && keyLeftCtrl) || (e.Key == Key.RightCtrl && keyRightCtrl))
				CloseContainer();
		}
		void OnPreviewKeyDownCore(KeyEventArgs e) {
			keyLeftCtrl = Keyboard.IsKeyDown(Key.LeftCtrl);
			keyRightCtrl = Keyboard.IsKeyDown(Key.RightCtrl);
			if(e.Key == Key.Tab && (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
				SelectPrevItemWrap();
			else if(e.Key == Key.Tab && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
				SelectNextItemWrap();
			else if(e.Key == Key.Up)
				SelectPrevItem();
			else if(e.Key == Key.Down)
				SelectNextItem();
			else if(e.Key == Key.Left || e.Key == Key.Right)
				ToggleActiveItems();
		}
		public void SelectNextItem() {
			SelectItemCore(false, true);
		}
		public void SelectPrevItem() {
			SelectItemCore(false, false);
		}
		public void SelectNextItemWrap() {
			SelectItemCore(true, true);
		}
		public void SelectPrevItemWrap() {
			SelectItemCore(true, false);
		}
		public void ToggleActiveItems() {
			ToggleActiveCollectionCore();
		}
		protected internal void SetSelectedItem() {
			SelectedItems = Documents.Count > 0 ? Documents : Panels;
			SelectActiveItem();
			if(KeyHelper.IsShiftPressed)
				SelectPrevItemWrap();
			else
				SelectNextItemWrap();
		}
		protected internal void InitializeItems(DockLayoutManager manager) {
			InitializeItemsCore(manager);
		}
		void InitializeItemsCore(DockLayoutManager manager) {
			Panels.Clear();
			Documents.Clear();
			CollectItemsByType(manager.GetItems());
		}
		void SelectActiveItem() {
			DockLayoutManager container = Container ?? DockLayoutManager.Ensure(this);
			if(Panels.Count != 0 || Documents.Count != 0) {
				if(Documents.Count > 0) {
					DocumentPanel active = container.ActiveDockItem as DocumentPanel;
					SelectedIndex = active != null ? Documents.IndexOf(active) : 0;
					return;
				}
				if(Panels.Count > 0) {
					LayoutPanel active = container.ActiveDockItem as LayoutPanel;
					if(active != null) {
						SelectedIndex = Panels.IndexOf(active);
						return;
					}
				}
				SelectedIndex = 0;
			}
		}
		protected void CollectItemsByType(BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items) {
				LayoutPanel panel = item as LayoutPanel;
				if(panel == null) continue;
				if(panel.IsClosed || !panel.IsVisible || !panel.ShowInDocumentSelector) continue;
				if(panel.ItemType == LayoutItemType.Document)
					Documents.Add(panel);
				else if(panel.ItemType == LayoutItemType.Panel)
					Panels.Add(panel);
			}
		}
		protected void SelectContent() {
			if(PartDocumentSelectorPreview != null)
				PartDocumentSelectorPreview.Target = SelectedItem;
		}
		protected internal void SelectItemCore(bool changeCollection, bool isNext) {
			if(SelectedItems.Count == 0) {
				SelectedIndex = -1;
				return;
			}
			IsCollectionChanging = CalcIsCollectionChanging(changeCollection);
			SelectedIndex += (isNext ? 1 : -1);
		}
		protected bool CalcIsCollectionChanging(bool isChanging) {
			return (!isChanging && NotActiveItemsAreEmpty()) ? true : isChanging;
		}
		protected bool NotActiveItemsAreEmpty() {
			return NotActivePanelsAreEmpty() || NotActiveDocumentsAreEmpty();
		}
		protected bool NotActivePanelsAreEmpty() {
			return SelectedItems == Documents && Panels.Count == 0;
		}
		protected bool NotActiveDocumentsAreEmpty() {
			return SelectedItems == Panels && Documents.Count == 0;
		}
		protected void SelectActiveItems(ObservableCollection<LayoutPanel> activeItems) {
			if(SelectedItems == activeItems || activeItems.Count == 0)
				return;
			SelectActiveItemsIndex(activeItems);
			SelectedItems = activeItems;
		}
		protected void SelectActiveItemsIndex(ObservableCollection<LayoutPanel> activeItems) {
			if(activeItems == Documents) {
				if(PanelIndex >= Documents.Count)
					PanelIndex = Documents.Count - 1;
				DocumentIndex = PanelIndex;
			}
			else {
				if(DocumentIndex >= Panels.Count)
					DocumentIndex = Panels.Count - 1;
				PanelIndex = DocumentIndex;
			}
		}
		protected void ToggleActiveCollectionCore() {
			IsCollectionChanging = false;
			SelectActiveItems(SelectedItems == Panels ? Documents : Panels);
			SelectContent();
		}
		int lockDocumentIndex;
		int lockPanelIndex;
		bool IsDocumentIndexLocked { get { return lockDocumentIndex > 0; } }
		bool IsPanelIndexLocked { get { return lockPanelIndex > 0; } }
		protected void OnPanelIndexChanged() {
			OnIndexPropertyChangedCore(true);
			try {
				lockPanelIndex++;
				BindablePanelIndex = PanelIndex;
			}
			finally {
				lockPanelIndex--;
			}
		}
		protected void OnDocumentIndexChanged() {
			OnIndexPropertyChangedCore(false);
			try {
				lockDocumentIndex++;
				BindableDocumentIndex = DocumentIndex;
			}
			finally {
				lockDocumentIndex--;
			}
		}
		protected void OnIndexPropertyChangedCore(bool isPanelIndex) {
			DependencyProperty indexProperty = null;
			if(PanelIndex != -1 || DocumentIndex != -1) {
				if(isPanelIndex) {
					SelectedItems = (PanelIndex == -1) ? Documents : Panels;
					indexProperty = DocumentIndexProperty;
				}
				else {
					SelectedItems = (DocumentIndex == -1) ? Panels : Documents;
					indexProperty = PanelIndexProperty;
				}
			}
			if(PanelIndex != -1 && DocumentIndex != -1 && indexProperty != null) {
				CoerceValue(indexProperty);
			}
			CoerceValue(SelectedItemProperty);
			SelectContent();
		}
		protected object CoercePanelIndex(object baseValue) {
			return CoerceIndexCore(true, (int)baseValue);
		}
		protected object CoerceDocumentIndex(object baseValue) {
			return CoerceIndexCore(false, (int)baseValue);
		}
		protected int CoerceIndexCore(bool isPanel, int baseValue) {
			if(PanelIndex != -1 && DocumentIndex != -1) return -1;
			return CalcIndex(isPanel, baseValue);
		}
		protected int CalcIndex(bool isPanel, int baseValue) {
			int count = isPanel ? Panels.Count : Documents.Count;
			if(baseValue >= count)
				return CalcIndexGreaterOrEqual(isPanel);
			else if(baseValue < 0)
				return CalcIndexLess(isPanel);
			else
				return baseValue;
		}
		protected int CalcIndexGreaterOrEqual(bool isPanelIndex) {
			int result = IsCollectionChanging ? 0 : -1;
			if(result == -1)
				AdjustIndex(isPanelIndex, 0);
			return result;
		}
		protected int CalcIndexLess(bool isPanel) {
			int count = isPanel ? (Panels.Count - 1) : (Documents.Count - 1);
			int result = IsCollectionChanging ? count : -1;
			if(result == -1)
				AdjustIndex(isPanel, isPanel ? (Documents.Count - 1) : (Panels.Count - 1));
			return result;
		}
		protected void AdjustIndex(bool isPanel, int index) {
			if(isPanel)
				AdjustDocumentIndex(index);
			else
				AdjustPanelIndex(index);
		}
		protected void AdjustDocumentIndex(int index) {
			if(Documents.Count != 0)
				DocumentIndex = index;
		}
		protected void AdjustPanelIndex(int index) {
			if(Panels.Count != 0)
				PanelIndex = index;
		}
		public FrameworkElement[] GetChildren() {
			return new FrameworkElement[] { PartPanelsListBox, PartDocumentsListBox, PartDocumentSelectorPreview };
		}
	}
}
