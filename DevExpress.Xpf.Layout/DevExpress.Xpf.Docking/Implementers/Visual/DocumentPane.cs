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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.VisualElements {
	[TemplatePart(Name = "PART_Content", Type = typeof(DocumentPaneContentPresenter))]
	public class DocumentPane : psvContentControl {
		#region static
		public static readonly DependencyProperty TabbedTemplateProperty;
		public static readonly DependencyProperty MDITemplateProperty;
		static DocumentPane() {
			var dProp = new DependencyPropertyRegistrator<DocumentPane>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("TabbedTemplate", ref TabbedTemplateProperty, (DataTemplate)null);
			dProp.Register("MDITemplate", ref MDITemplateProperty, (DataTemplate)null);
		}
		#endregion static
		public IUIElement GetRootUIScope() {
			return LayoutItem != null ? LayoutItem.GetRootUIScope() : null;
		}
		public DocumentPane() {
		}
		protected override void OnDispose() {
			ClearValue(TabbedTemplateProperty);
			ClearValue(MDITemplateProperty);
			if(PartContent != null) {
				PartContent.Dispose();
				PartContent = null;
			}
			base.OnDispose();
		}
		public DataTemplate TabbedTemplate {
			get { return (DataTemplate)GetValue(TabbedTemplateProperty); }
			set { SetValue(TabbedTemplateProperty, value); }
		}
		public DataTemplate MDITemplate {
			get { return (DataTemplate)GetValue(MDITemplateProperty); }
			set { SetValue(MDITemplateProperty, value); }
		}
		public DocumentPaneContentPresenter PartContent { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(PartContent != null && !LayoutItemsHelper.IsTemplateChild(PartContent, this))
				PartContent.Dispose();
			PartContent = GetTemplateChild("PART_Content") as DocumentPaneContentPresenter;
			if(PartContent != null)
				PartContent.EnsureOwner(this);
		}
	}
	[TemplatePart(Name = "PART_ItemsControl", Type = typeof(psvItemsControl))]
	public class DocumentPaneContentPresenter : BasePanePresenter<DocumentPane, DocumentGroup> {
		#region static
		public static readonly DependencyProperty MDIStyleProperty;
		static DocumentPaneContentPresenter() {
			var dProp = new DependencyPropertyRegistrator<DocumentPaneContentPresenter>();
			dProp.Register("MDIStyle", ref MDIStyleProperty, MDIStyle.Default,
				(dObj, e) => ((DocumentPaneContentPresenter)dObj).OnStylePropertyChanged());
		}
		#endregion static
		protected override void OnDispose() {
			if(PartItemsContainer != null) {
				PartItemsContainer.Dispose();
				PartItemsContainer = null;
			}
			base.OnDispose();
		}
		public MDIStyle MDIStyle {
			get { return (MDIStyle)GetValue(MDIStyleProperty); }
			set { SetValue(MDIStyleProperty, value); }
		}
		protected override bool CanSelectTemplate(DocumentGroup group) {
			return group.ActualGroupTemplateSelector != null;
		}
		protected override DataTemplate SelectTemplateCore(DocumentGroup group) {
			return group.ActualGroupTemplateSelector.SelectTemplate(group, this);
		}
		public psvItemsControl PartItemsContainer { get; private set; }
		public override void OnApplyTemplate() {
			if(PartItemsContainer != null && !LayoutItemsHelper.IsTemplateChild(PartItemsContainer, this))
				PartItemsContainer.Dispose();
			base.OnApplyTemplate();
			PartItemsContainer = GetTemplateChild("PART_ItemsControl") as psvItemsControl;
		}
		protected override void OnStylePropertyChanged() {
			MDIControllerHelper.UnMergeMDITitles(this);
			DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(this);
			using(new LogicalTreeLocker(manager, new BaseLayoutItem[] { ConvertToLogicalItem(Content) })) {
				base.OnStylePropertyChanged();
			}
			MDIControllerHelper.UnMergeMDIMenuItems(this);
		}
		protected override DocumentGroup ConvertToLogicalItem(object content) {
			return LayoutItemData.ConvertToBaseLayoutItem(content) as DocumentGroup ?? base.ConvertToLogicalItem(content);
		}
	}
	public class DocumentTabContainer : LayoutTabControl {
		#region static
		public static readonly DependencyProperty ThemeDependentBackgroundProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty BackgroundInternalProperty;
		public static readonly DependencyProperty ActualBackgroundProperty;
		public static readonly DependencyProperty TabColorProperty;
		public static readonly DependencyProperty HasTabColorProperty;
		readonly static DependencyPropertyKey HasTabColorPropertyKey;
		static DocumentTabContainer() {
			var dProp = new DependencyPropertyRegistrator<DocumentTabContainer>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.OverrideMetadata(ShowTabForSinglePageProperty, true);
			dProp.Register("ThemeDependentBackground", ref ThemeDependentBackgroundProperty, (Brush)null,
				(dObj, e) => ((DocumentTabContainer)dObj).OnBackgroundChanged((Brush)e.NewValue));
			dProp.Register("BackgroundInternal", ref BackgroundInternalProperty, (Brush)null,
				(dObj, e) => ((DocumentTabContainer)dObj).OnBackgroundChanged((Brush)e.NewValue));
			dProp.Register("ActualBackground", ref ActualBackgroundProperty, (Brush)null, null,
				(dObj, value) => ((DocumentTabContainer)dObj).CoerceActualBackground((Brush)value));
			dProp.Register("TabColor", ref TabColorProperty, Colors.Transparent,
				(dObj, e) => ((DocumentTabContainer)dObj).OnTabColorChanged((Color)e.OldValue, (Color)e.NewValue));
			dProp.RegisterReadonly("HasTabColor", ref HasTabColorPropertyKey, ref HasTabColorProperty, false);
		}
		#endregion static
		public DocumentTabContainer() {
			this.StartListen(BackgroundInternalProperty, "Background");
		}
		protected override void OnDispose() {
			base.OnDispose();
			UnsubscribeItem(SelectedContent);
		}
		protected override psvSelectorItem CreateSelectorItem() {
			return new DocumentPaneItem();
		}
		protected override IView GetView(DockLayoutManager container) {
			DocumentPane documentPane = null;
			DocumentPaneContentPresenter presenter = TemplatedParent as DocumentPaneContentPresenter;
			if(presenter != null) documentPane = presenter.Owner;
			return (documentPane != null) ? container.GetView(documentPane.GetRootUIScope()) : null;
		}
		void SubscribeItem(BaseLayoutItem item) {
			if(item != null) {
				item.VisualChanged += OnItemVisualChanged;
				item.Forward(this, TabColorProperty, "ActualTabBackgroundColor");
			}
		}
		private void UnsubscribeItem(BaseLayoutItem item) {
			if(item != null) {
				item.VisualChanged -= OnItemVisualChanged;
				ClearValue(TabColorProperty);
			}
		}
		void OnItemVisualChanged(object sender, EventArgs e) {
			UpdateVisualState();
		}
		protected override void OnSelectedContentChanged(BaseLayoutItem newValue, BaseLayoutItem oldValue) {
			base.OnSelectedContentChanged(newValue, oldValue);
			UnsubscribeItem(oldValue);
			SubscribeItem(newValue);
			UpdateVisualState();
		}
		protected override void OnHasItemsChanged(bool hasItems) {
			base.OnHasItemsChanged(hasItems);
			UpdateVisualState();
		}
		protected override void UpdateVisualState() {
			base.UpdateVisualState();
			VisualStateManager.GoToState(this, "EmptyActiveState", false);
			if(HasItems) {
				if(SelectedContent != null) {
					if(SelectedContent.IsActive)
						VisualStateManager.GoToState(this, "Active", false);
					else
						VisualStateManager.GoToState(this, "Inactive", false);
				}
			}
			else
				VisualStateManager.GoToState(this, "Empty", false);
			UpdateLocationState();
		}
		protected virtual void UpdateLocationState() {
			string state = "EmptyLocationState";
			VisualStateManager.GoToState(this, state, false);
			state = (CaptionLocation == CaptionLocation.Default ? CaptionLocation.Top : CaptionLocation).ToString();
			VisualStateManager.GoToState(this, state, false);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateVisualState();
		}
		protected override void OnCaptionLocationChanged(CaptionLocation captionLocation) {
			base.OnCaptionLocationChanged(captionLocation);
			UpdateVisualState();
		}
		protected virtual void OnBackgroundChanged(Brush newValue) {
			CoerceValue(ActualBackgroundProperty);
		}
		protected virtual void OnTabColorChanged(Color oldValue, Color newValue) {
			SetValue(HasTabColorPropertyKey, newValue != Colors.Transparent);
		}
		protected virtual object CoerceActualBackground(Brush value) {
			return Background ?? ThemeDependentBackground;
		}
		public Brush ThemeDependentBackground {
			get { return (Brush)GetValue(ThemeDependentBackgroundProperty); }
			set { SetValue(ThemeDependentBackgroundProperty, value); }
		}
		public Brush ActualBackground {
			get { return (Brush)GetValue(ActualBackgroundProperty); }
			set { SetValue(ActualBackgroundProperty, value); }
		}
		public Color TabColor {
			get { return (Color)GetValue(TabColorProperty); }
			set { SetValue(TabColorProperty, value); }
		}
		public bool HasTabColor {
			get { return (bool)GetValue(HasTabColorProperty); }
		}
	}
	[TemplatePart(Name = "PART_MDIPanel", Type = typeof(MDIPanel))]
	public class DocumentMDIContainer : psvItemsControl {
		#region static
		public static readonly DependencyProperty LayoutItemProperty;
		static readonly DependencyPropertyKey HasMaximizedDocumentsPropertyKey;
		public static readonly DependencyProperty HasMaximizedDocumentsProperty;
		static readonly DependencyPropertyKey IsHeaderVisiblePropertyKey;
		public static readonly DependencyProperty IsHeaderVisibleProperty;
		public static readonly DependencyProperty ThemeDependentBackgroundProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty BackgroundInternalProperty;
		public static readonly DependencyProperty ActualBackgroundProperty;
		static DocumentMDIContainer() {
			var dProp = new DependencyPropertyRegistrator<DocumentMDIContainer>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("LayoutItem", ref LayoutItemProperty, (BaseLayoutItem)null,
				(dObj, e) => ((DocumentMDIContainer)dObj).OnLayoutItemChanged((BaseLayoutItem)e.OldValue, (BaseLayoutItem)e.NewValue));
			dProp.RegisterReadonly("HasMaximizedDocuments", ref HasMaximizedDocumentsPropertyKey, ref HasMaximizedDocumentsProperty, false,
					(dObj, e) => ((DocumentMDIContainer)dObj).OnHasMaximizedDocumentsChanged((bool)e.NewValue),
					(dObj, value) => ((DocumentMDIContainer)dObj).CoerceHasMaximizedDocumentsVisible((bool)value));
			dProp.RegisterReadonly("IsHeaderVisible", ref IsHeaderVisiblePropertyKey, ref IsHeaderVisibleProperty, false);
			dProp.Register("ThemeDependentBackground", ref ThemeDependentBackgroundProperty, (Brush)null,
				(dObj, e) => ((DocumentMDIContainer)dObj).OnBackgroundChanged((Brush)e.NewValue));
			dProp.Register("BackgroundInternal", ref BackgroundInternalProperty, (Brush)null,
				(dObj, e) => ((DocumentMDIContainer)dObj).OnBackgroundChanged((Brush)e.NewValue));
			dProp.Register("ActualBackground", ref ActualBackgroundProperty, (Brush)null, null,
				(dObj, value) => ((DocumentMDIContainer)dObj).CoerceActualBackground((Brush)value));
		}
		#endregion static
		public DocumentMDIContainer() {
			this.StartListen(BackgroundInternalProperty, "Background");
		}
		protected override void OnDispose() {
			if(PartMDIPanel != null)
				PartMDIPanel.RequestBringIntoView -= PartMDIPanel_RequestBringIntoView;
			ClearValue(LayoutItemProperty);
			base.OnDispose();
		}
		protected virtual void OnLayoutItemChanged(BaseLayoutItem oldItem, BaseLayoutItem item) {
			OnLayoutGroupChanged(oldItem as LayoutGroup, item as LayoutGroup);
		}
		protected virtual void OnLayoutGroupChanged(LayoutGroup oldGroup, LayoutGroup group) {
			if(oldGroup != null)
				oldGroup.SelectedItemChanged -= OnSelectedDocumentChanged;
			if(group == null) {
				ClearValue(DockLayoutManager.LayoutItemProperty);
				ClearValue(ItemsSourceProperty);
			}
			else {
				SetValue(DockLayoutManager.LayoutItemProperty, group);
				SetValue(ItemsSourceProperty, group.Items);
				group.SelectedItemChanged += OnSelectedDocumentChanged;
			}
		}
		protected virtual void OnSelectedDocumentChanged(object sender, Base.SelectedItemChangedEventArgs e) {
			OnHasMaximizedDocumentsChanged(HasMaximizedDocuments);
		}
		public BaseLayoutItem LayoutItem {
			get { return (BaseLayoutItem)GetValue(LayoutItemProperty); }
			set { SetValue(LayoutItemProperty, value); }
		}
		public Brush ThemeDependentBackground {
			get { return (Brush)GetValue(ThemeDependentBackgroundProperty); }
			set { SetValue(ThemeDependentBackgroundProperty, value); }
		}
		public Brush ActualBackground {
			get { return (Brush)GetValue(ActualBackgroundProperty); }
			set { SetValue(ActualBackgroundProperty, value); }
		}
		public LayoutGroup LayoutGroup { get { return LayoutItem as LayoutGroup; } }
		public bool HasMaximizedDocuments {
			get { return (bool)GetValue(HasMaximizedDocumentsProperty); }
		}
		public bool IsHeaderVisible {
			get { return (bool)GetValue(IsHeaderVisibleProperty); }
			private set { SetValue(IsHeaderVisiblePropertyKey, value); }
		}
		protected virtual void OnBackgroundChanged(Brush newValue) {
			CoerceValue(ActualBackgroundProperty);
		}
		protected virtual object CoerceActualBackground(Brush value) {
			return Background ?? ThemeDependentBackground;
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is BaseLayoutItem || item is MDIDocument;
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new MDIDocument();
		}
		protected virtual void OnHasMaximizedDocumentsChanged(bool hasMaximizedDocuments) {
			DockLayoutManager manager = DockLayoutManager.Ensure(this);
			IsHeaderVisible = false;
			if(hasMaximizedDocuments) {
				InvokeHelper.BeginInvoke(this, new Action(()=>{
					MDIControllerHelper.MergeMDITitles(GetActiveDocument());
				}));				
				if(!MDIControllerHelper.MergeMDIMenuItems(this))
					IsHeaderVisible = true;
			}
			else {
				MDIControllerHelper.UnMergeMDITitles(this);				
				MDIControllerHelper.UnMergeMDIMenuItems(this);
			}
			UpdateVisualState();
		}
		private void UpdateVisualState() {
			if (IsHeaderVisible)
				VisualStateManager.GoToState(this, "HeaderVisible", false);
			else
				VisualStateManager.GoToState(this, "HeaderHidden", false);
			VisualStateManager.GoToState(this, HasMaximizedDocuments ?  "Maximized" : "EmptyMaximizedState", false);
		}
		protected virtual bool CoerceHasMaximizedDocumentsVisible(bool baseValue) {
			foreach(DependencyObject item in Items) {
				if(MDIStateHelper.GetMDIState(item) == MDIState.Maximized) return true;
			}
			return false;
		}
		protected override void PrepareContainer(DependencyObject element, object item) {
			base.PrepareContainer(element, item);
			if(item is BaseLayoutItem){
				((BaseLayoutItem)item).SelectTemplate();
			}
		}
		protected override void ClearContainer(DependencyObject element) {
			if(element is BaseLayoutItem) {
				((BaseLayoutItem)element).ClearTemplate();
			}
			CoerceValue(HasMaximizedDocumentsProperty);
			base.ClearContainer(element);
		}
		public MDIPanel PartMDIPanel { get; private set; }
		public ItemsPresenter PartMDIPanelPresenter { get; private set; }
		public ScrollViewer PartScroller { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartMDIPanelPresenter = GetTemplateChild("PART_MDIPanelPresenter") as ItemsPresenter;
			PartScroller = GetTemplateChild("scroller") as ScrollViewer;
			if(PartMDIPanelPresenter != null)
				PartMDIPanelPresenter.Loaded += PartMDIPanelPresenter_Loaded;
			if(PartScroller != null) {
				PartScroller.Focusable = false;
				PartScroller.CanContentScroll = true;
				PartScroller.SetBinding(DevExpress.Xpf.Core.ScrollBarExtensions.AllowMouseScrollingProperty, new System.Windows.Data.Binding() { Source = LayoutItem, Path = new PropertyPath(DevExpress.Xpf.Core.ScrollBarExtensions.AllowMouseScrollingProperty) });
			}
			CoerceValue(HasMaximizedDocumentsProperty);
		}
		void PartMDIPanelPresenter_Loaded(object sender, RoutedEventArgs e) {
			PartMDIPanelPresenter.Loaded -= PartMDIPanelPresenter_Loaded;
			PartMDIPanel = LayoutItemsHelper.GetTemplateChild<MDIPanel>(PartMDIPanelPresenter);
			if(PartMDIPanel != null) {
				PartMDIPanel.RequestBringIntoView += PartMDIPanel_RequestBringIntoView;
				if(HasMaximizedDocuments) MDIControllerHelper.MergeMDITitles(GetActiveDocument());
			}
		}
		void PartMDIPanel_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e) {
			e.Handled = true;
		}
		protected override void OnActualSizeChanged(Size value) {
			base.OnActualSizeChanged(value);
			if(IsDisposing) return;
			if(PartMDIPanel != null)
				PartMDIPanel.InvalidateMeasure();
			DocumentGroup dGroup = DockLayoutManager.GetLayoutItem(this) as DocumentGroup;
			if(dGroup != null) dGroup.MDIAreaSize = value;
		}
		public UIElement GetActiveDocument() {
			return PartMDIPanel != null ? PartMDIPanel.GetActiveDocument() : null;
		}
	}
	public class MDIPanel : ScrollablePanel {
		#region static
		static MDIPanel() {
			var dProp = new DependencyPropertyRegistrator<MDIPanel>();
			dProp.OverrideMetadataNotDataBindable(IsItemsHostProperty, true, null);
		}
		#endregion static
		public MDIDocument GetActiveDocument() {
			foreach(UIElement element in InternalChildren) {
				if(element == null) continue;
				var document = element as BaseLayoutItem ?? DockLayoutManager.GetLayoutItem(element);
				if(document == null || document.Parent == null) continue;
				if(document.Parent.SelectedItem == document)
					return element as MDIDocument ?? LayoutItemsHelper.GetVisualChild<MDIDocument>(document);
			}
			return null;
		}
		protected override Size MeasureOverride(Size avalaibleSize) {
			Rect allArea = new Rect(0, 0, 0, 0);
			Size constraints = CheckConstraints(avalaibleSize);
			foreach(UIElement element in InternalChildren) {
				if(element == null) continue;
				Size mdiSize = CheckMDISize(MDIStateHelper.GetMDIState(element), DocumentPanel.GetMDISize(element), constraints);
				element.Measure(mdiSize);
				Rect elementRect = GetElementRect(constraints, element);
				allArea.Union(elementRect);
			}
			VerifyScrollData(avalaibleSize, allArea.GetSize());
			return Viewport;
		}
		Size CheckMDISize(MDIState state, Size mdiSize, Size constraint) {
			if(double.IsNaN(mdiSize.Width))
				mdiSize.Width = double.PositiveInfinity;
			if(double.IsNaN(mdiSize.Height))
				mdiSize.Height = double.PositiveInfinity;
			if(state == MDIState.Maximized) {
				mdiSize = constraint;
			}
			return mdiSize;
		}
		Size CheckConstraints(Size constraint) {
			var scrollContainer = LayoutHelper.FindParentObject<ScrollContentPresenter>(this);
			if(double.IsInfinity(constraint.Width))
				constraint.Width = (scrollContainer != null) ? scrollContainer.ViewportWidth : 1000;
			if(double.IsInfinity(constraint.Height))
				constraint.Height = (scrollContainer != null) ? scrollContainer.ViewportHeight : 1000;
			return constraint;
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			Rect allArea = new Rect(0, 0, 0, 0);
			foreach(UIElement element in InternalChildren) {
				if(element == null) continue;
				Rect elementRect = GetElementRect(arrangeSize, element);
				allArea.Union(elementRect);
				elementRect.X -= HorizontalOffset;
				elementRect.Y -= VerticalOffset;
				element.Arrange(elementRect);
			}
			VerifyScrollData(arrangeSize, allArea.GetSize());
			return arrangeSize;
		}
		Rect GetElementRect(Size constraint, UIElement element) {
			var document = element as BaseLayoutItem ?? DockLayoutManager.GetLayoutItem(element);
			MDIState state = MDIStateHelper.GetMDIState(element);
			Point mdiLocation = DocumentPanel.GetMDILocation(element);
			Size mdiSize = state == MDIState.Minimized ? new Size(double.NaN, double.NaN) : DocumentPanel.GetMDISize(element);
			if(double.IsNaN(mdiSize.Height) || double.IsNaN(mdiSize.Width)) {
				Size min = document.ActualMinSize; Size max = document.ActualMaxSize;
				double w = mdiSize.Width;
				double h = mdiSize.Height;
				if(double.IsNaN(w))
					w = MathHelper.MeasureDimension(min.Width, max.Width, element.DesiredSize.Width);
				if(double.IsNaN(h))
					h = MathHelper.MeasureDimension(min.Height, max.Height, element.DesiredSize.Height);
				mdiSize = new Size(w, h);
			}
			if(state == MDIState.Maximized) {
				mdiLocation = new Point(0, 0);
				mdiSize = constraint;
			}
			return new Rect(mdiLocation, mdiSize);
		}
		void CheckInternalChildren() {
			foreach(UIElement element in InternalChildren) {
				if(element == null) continue;
				if(MDIStateHelper.GetMDIState(element) == MDIState.Maximized) {
					InvalidateMeasure();
					break;
				}
			}
		}
		protected override void OnActualSizeChanged(Size value) {
			base.OnActualSizeChanged(value);
			CheckInternalChildren();
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			CheckInternalChildren();
		}
	}
}
