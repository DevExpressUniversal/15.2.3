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
using System.Windows.Input;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Data.Utils;
using DevExpress.Xpf.Bars.Native;
using System.Linq;
namespace DevExpress.Xpf.Docking.VisualElements {
	[TemplatePart(Name = "PART_ContentPresenter", Type = typeof(DocumentContentPresenter))]
	public class BaseDocument : psvContentControl, Bars.IMDIChildHost, IMDIMergeStyleListener, IDockLayoutManagerListener, IMergingClient {
		#region static
		public static readonly DependencyProperty IsSelectedProperty;
		public static readonly DependencyProperty ControlHostTemplateProperty;
		public static readonly DependencyProperty LayoutHostTemplateProperty;
		public static readonly DependencyProperty DataHostTemplateProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty LayoutItemContentProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty IsActiveProperty;
		static BaseDocument() {
			var dProp = new DependencyPropertyRegistrator<BaseDocument>();
			dProp.Register("ControlHostTemplate", ref ControlHostTemplateProperty, (DataTemplate)null);
			dProp.Register("LayoutHostTemplate", ref LayoutHostTemplateProperty, (DataTemplate)null);
			dProp.Register("DataHostTemplate", ref DataHostTemplateProperty, (DataTemplate)null);
			dProp.Register("IsSelected", ref IsSelectedProperty, false,
				(dObj, e) => ((BaseDocument)dObj).OnIsSelectedChanged((bool)e.NewValue));
			dProp.Register("LayoutItemContent", ref LayoutItemContentProperty, (object)null,
				(dObj, e) => ((BaseDocument)dObj).OnLayoutItemContentChanged(e.OldValue, e.NewValue));
			dProp.Register("IsActive", ref IsActiveProperty, false,				
				(dObj, e) => ((BaseDocument)dObj).OnIsActiveChanged((bool)e.OldValue, (bool)e.NewValue));
		}
		#endregion static
		public BaseDocument() {
			LayoutUpdated += OnLayoutUpdated;
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			if(fMergingRequested)
				MergingHelper.DoMerging();
		}
		protected override void OnDispose() {
			UnMerge();
			ClearValue(ControlHostTemplateProperty);
			ClearValue(LayoutHostTemplateProperty);
			ClearValue(DataHostTemplateProperty);
			ClearValue(IsSelectedProperty);
			ClearValue(LayoutItemContentProperty);
			if(PartContentPresenter != null) {
				PartContentPresenter.Dispose();
				PartContentPresenter = null;
			}
			isChildMenuVisibleChangedHandlers.Clear();
			base.OnDispose();
		}
		public DataTemplate ControlHostTemplate {
			get { return (DataTemplate)GetValue(ControlHostTemplateProperty); }
			set { SetValue(ControlHostTemplateProperty, value); }
		}
		public DataTemplate LayoutHostTemplate {
			get { return (DataTemplate)GetValue(LayoutHostTemplateProperty); }
			set { SetValue(LayoutHostTemplateProperty, value); }
		}
		public DataTemplate DataHostTemplate {
			get { return (DataTemplate)GetValue(DataHostTemplateProperty); }
			set { SetValue(DataHostTemplateProperty, value); }
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public bool IsActive {
			get { return (bool)GetValue(IsActiveProperty); }
			set { SetValue(IsActiveProperty, value); }
		}
		public DocumentPanel DocumentPanel {
			get { return LayoutItem as DocumentPanel; }
		}
		BarManager parentBarManager;
		protected BarManager ParentBarManager {
			get {
				if(parentBarManager == null) {
					parentBarManager = LayoutHelper.FindParentObject<BarManager>(Container);
				}
				return parentBarManager;
			}
		}
		public DocumentContentPresenter PartContentPresenter { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(PartContentPresenter != null && !LayoutItemsHelper.IsTemplateChild(PartContentPresenter, this))
				PartContentPresenter.Dispose();
			PartContentPresenter = GetTemplateChild("PART_ContentPresenter") as DocumentContentPresenter;
			if(PartContentPresenter != null) {
				PartContentPresenter.EnsureOwner(this);
				BindingHelper.SetBinding(PartContentPresenter, DocumentContentPresenter.IsControlItemsHostProperty, LayoutItem, "IsControlItemsHost");
				BindingHelper.SetBinding(PartContentPresenter, DocumentContentPresenter.IsDataBoundProperty, LayoutItem, "IsDataBound");
			}
		}
		protected override void OnLayoutItemChanged(BaseLayoutItem item, BaseLayoutItem oldItem) {
			base.OnLayoutItemChanged(item, oldItem);
			if(item != null) {
				SetBinding(IsSelectedProperty, new System.Windows.Data.Binding("IsSelectedItem") { Source = item });
				SetBinding(VisibilityProperty, new System.Windows.Data.Binding("Visibility") { Source = item });
				SetBinding(LayoutItemContentProperty, new System.Windows.Data.Binding("Content") { Source = item });
				SetBinding(IsActiveProperty, new System.Windows.Data.Binding("IsActive") { Source = item });
				if (PartContentPresenter != null) {
					BindingHelper.SetBinding(PartContentPresenter, DocumentContentPresenter.IsControlItemsHostProperty, LayoutItem, "IsControlItemsHost");
					BindingHelper.SetBinding(PartContentPresenter, DocumentContentPresenter.IsDataBoundProperty, LayoutItem, "IsDataBound");
				}
				var documentPanel = item as DocumentPanel;
				if(documentPanel != null) documentPanel.MergingClient = this;
			}
			else {
				BindingHelper.ClearBinding(this, IsSelectedProperty);
				BindingHelper.ClearBinding(this, VisibilityProperty);
				BindingHelper.ClearBinding(this, LayoutItemContentProperty);
				BindingHelper.ClearBinding(this, IsActiveProperty);
				var documentPanel = oldItem as DocumentPanel;
				if(documentPanel != null) documentPanel.MergingClient = null;
			}
			CheckIsChildMenuVisible();
		}		
		protected virtual bool GetIsChildMenuVisible() {
			return true;
		}
		protected virtual bool GetIsChildMenuVisibleForFloatingElement(bool isFloating) {
			if(DocumentPanel == null
				|| DesignerProperties.GetIsInDesignMode(this)
				|| !BarNameScope.GetService<IElementRegistratorService>(this).GetElements<IMergingSupport>(ScopeSearchSettings.Ancestors).Any(x => x is IBar || x is DevExpress.Xpf.Ribbon.IRibbonControl))
				return true;
			MDIMergeStyle mergingStyle = MDIControllerHelper.GetActualMDIMergeStyle(DocumentPanel.GetDockLayoutManager(), DocumentPanel);
			bool disableMerging = isFloating || mergingStyle == MDIMergeStyle.Never;
			return disableMerging;
		}
		protected virtual void OnLayoutItemContentChanged(object oldContent, object newContent) {
			if(oldContent != null) UnMerge();
			ProcessMergeActions(LayoutItem);
		}
		protected virtual void OnIsActiveChanged(bool oldValue, bool newValue) {
			ProcessMergeActions(LayoutItem);
		}
		protected virtual void ProcessMergeActions(BaseLayoutItem item) { }
		protected virtual void OnIsSelectedChanged(bool selected) {
			ProcessMergeActions(DocumentPanel);
		}
		protected virtual void CheckIsChildMenuVisible() {
			IsChildMenuVisibleCore = GetIsChildMenuVisible();			
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			CheckIsChildMenuVisible();
		}
		#region MDI Merging
		bool fMergingRequested;
		protected void UnMerge() {
			if(fMergingRequested)
				fMergingRequested = false;
			UnmergeCore();
		}
		protected void Merge() {
			if(!fMergingRequested || DesignerProperties.GetIsInDesignMode(this)) return;
			MergeCore();
			fMergingRequested = false;
		}
		protected void BeginMerge() {
			if(fMergingRequested) return;
			fMergingRequested = true;
			MergingHelper.AddMergingClient(this);
			InvalidateArrange();
		}
		protected virtual void UnmergeCore() {
			MDIControllerHelper.DoUnMerge(this);
		}
		protected virtual void MergeCore() {
			MDIControllerHelper.DoMerge(this);
		}
		#endregion merging
		#region IMDIChildHost Members
		bool _IsChildMenuVisibleCore = true;
		protected bool IsChildMenuVisibleCore {
			get { return _IsChildMenuVisibleCore; }
			set {
				if(_IsChildMenuVisibleCore == value) return;
				_IsChildMenuVisibleCore = value;
				NotifyListeners();
			}
		}
		protected virtual void NotifyListeners() {
			foreach(EventHandler handler in isChildMenuVisibleChangedHandlers) {
				if(handler != null) handler(this, EventArgs.Empty);
			}
		}
		protected DevExpress.Xpf.Bars.Native.WeakList<EventHandler> isChildMenuVisibleChangedHandlers = new DevExpress.Xpf.Bars.Native.WeakList<EventHandler>();
		bool Bars.IMDIChildHost.IsChildMenuVisible {
			get { return _IsChildMenuVisibleCore; }
		}
		event EventHandler Bars.IMDIChildHost.IsChildMenuVisibleChanged {
			add { isChildMenuVisibleChangedHandlers.Add(value); }
			remove { isChildMenuVisibleChangedHandlers.Remove(value); }
		}
		#endregion
		#region IMDIMergeStyleListener Members
		void IMDIMergeStyleListener.OnMDIMergeStyleChanged(MDIMergeStyle oldValue, MDIMergeStyle newValue) {
			if(oldValue.IsDefault() && newValue.IsDefault()) return;
			CheckIsChildMenuVisible();
			ProcessMergeActions(DocumentPanel);
		}
		#endregion
		#region IDockLayoutManagerListener Members
		void OnMDIMergeStyleChanged(object sender, PropertyChangedEventArgs e) {
			if(Container.MDIMergeStyle.IsDefault() && DocumentPanel.GetMDIMergeStyle(this).IsDefault()) return;
			CheckIsChildMenuVisible();
			ProcessMergeActions(DocumentPanel);
		}
		MDIMergeStylePropertyChangedWeakEventHandler<BaseDocument> mergeStylePropertyChangedHandler;
		MDIMergeStylePropertyChangedWeakEventHandler<BaseDocument> MDIMergeStylePropertyChangedHandler {
			get {
				if(mergeStylePropertyChangedHandler == null) {
					mergeStylePropertyChangedHandler = new MDIMergeStylePropertyChangedWeakEventHandler<BaseDocument>(this, 
						(owner, o, e) => { owner.OnMDIMergeStyleChanged(o, e); });
				}
				return mergeStylePropertyChangedHandler;
			}
		}
		void IDockLayoutManagerListener.Subscribe(DockLayoutManager manager) {
			manager.MDIMergeStyleChanged += MDIMergeStylePropertyChangedHandler.Handler;
		}
		void IDockLayoutManagerListener.Unsubscribe(DockLayoutManager manager) {
			manager.MDIMergeStyleChanged -= MDIMergeStylePropertyChangedHandler.Handler;
		}
		class MDIMergeStylePropertyChangedWeakEventHandler<TOwner> : WeakEventHandler<TOwner, PropertyChangedEventArgs, PropertyChangedEventHandler> where TOwner : class {
		 static Action<WeakEventHandler<TOwner, PropertyChangedEventArgs, PropertyChangedEventHandler>, object> action = (h, o) => ((DockLayoutManager)o).MDIMergeStyleChanged -= h.Handler;
		 static Func<WeakEventHandler<TOwner, PropertyChangedEventArgs, PropertyChangedEventHandler>, PropertyChangedEventHandler> create = h => new PropertyChangedEventHandler(h.OnEvent);
			public MDIMergeStylePropertyChangedWeakEventHandler(TOwner owner, Action<TOwner, object, PropertyChangedEventArgs> onEventAction)
				: base(owner, onEventAction, action, create) {
			}
		}
		#endregion
		#region IMergingClient Members
		void IMergingClient.Merge() {
			Merge();
		}
		void IMergingClient.QueueMerge() {
			ProcessMergeActions(DocumentPanel);
		}
		void IMergingClient.QueueUnmerge() {
			UnMerge();
		}
		#endregion
	}
	public class DocumentContentPresenter : DockItemContentPresenter<BaseDocument, DocumentPanel> {
		#region static
		static DocumentContentPresenter() {
			var dProp = new DependencyPropertyRegistrator<DocumentContentPresenter>();
		}
		#endregion
		class DefaultContentTemplateSelector : DataTemplateSelector {
			public override DataTemplate SelectTemplate(object item, DependencyObject container) {
				DocumentContentPresenter presenter = container as DocumentContentPresenter;
				DocumentPanel panel = item as DocumentPanel;
				if(panel != null && presenter != null && presenter.Owner != null) {
					return panel.IsControlItemsHost ?
						presenter.Owner.LayoutHostTemplate :
						panel.IsDataBound ? presenter.Owner.DataHostTemplate : presenter.Owner.ControlHostTemplate;
				}
				return null;
			}
		}
		DataTemplateSelector defaultContentTemplateSelector;
		DataTemplateSelector _DefaultContentTemplateSelector {
			get {
				if(defaultContentTemplateSelector == null)
					defaultContentTemplateSelector = new DefaultContentTemplateSelector();
				return defaultContentTemplateSelector;
			}
		}
		protected override void OnDispose() {
			if(PartBarContainer != null) {
				PartBarContainer.Dispose();
				PartBarContainer = null;
			}
			if(PartControl != null) {
				PartControl.Dispose();
				PartControl = null;
			}
			if(PartLayout != null) {
				PartLayout.Dispose();
				PartLayout = null;
			}
			if(PartContent != null) {
				PartContent.Dispose();
				PartContent = null;
			}
			base.OnDispose();
		}
		protected override bool CanSelectTemplate(DocumentPanel document) {
			return _DefaultContentTemplateSelector != null;
		}
		protected override DataTemplate SelectTemplateCore(DocumentPanel document) {
			return _DefaultContentTemplateSelector.SelectTemplate(document, this);
		}
		public DockBarContainerControl PartBarContainer { get; private set; }
		public UIElementPresenter PartControl { get; private set; }
		public psvContentPresenter PartLayout { get; private set; }
		public psvContentPresenter PartContent { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(PartBarContainer != null && !LayoutItemsHelper.IsTemplateChild(PartBarContainer, this))
				PartBarContainer.Dispose();
			PartBarContainer = LayoutItemsHelper.GetTemplateChild<DockBarContainerControl>(this);
			if(PartControl != null && !LayoutItemsHelper.IsTemplateChild(PartControl, this))
				PartControl.Dispose();
			PartControl = LayoutItemsHelper.GetTemplateChild<UIElementPresenter>(this);
			if(PartLayout != null && !LayoutItemsHelper.IsTemplateChild(PartLayout, this))
				PartLayout.Dispose();
			ScrollViewer scrollViewer = LayoutItemsHelper.GetTemplateChild<ScrollViewer>(this);
			if(scrollViewer != null) {
				PartLayout = scrollViewer.Content as psvContentPresenter;
			}
			if(PartContent != null && !LayoutItemsHelper.IsTemplateChild(PartContent, this))
				PartContent.Dispose();
			PartContent = LayoutItemsHelper.GetTemplateChild<psvContentPresenter>(this, false);
		}
	}
	public class Document : BaseDocument {
		#region static
		static Document() {
			var dProp = new DependencyPropertyRegistrator<Document>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion static
		public Document() {
		}
		protected override bool GetIsChildMenuVisible() {
			return GetIsChildMenuVisibleForFloatingElement(DocumentPanel == null ? true : DocumentPanel.IsFloating);
		}
		protected override void ProcessMergeActions(BaseLayoutItem item) {
			if(item == null || (!item.IsFloating && !item.IsSelectedItem))
				UnMerge();
			else {
				if(!item.IsFloating) {
					MDIMergeStyle mergeStyle = MDIControllerHelper.GetActualMDIMergeStyle(item.GetDockLayoutManager(), item);
					switch(mergeStyle) {
						case MDIMergeStyle.Default:
						case MDIMergeStyle.Always:
							BeginMerge();
							break;
						case MDIMergeStyle.Never:
							UnMerge();
							break;
						default:
							if(item.IsActive)
								BeginMerge();
							else {
								UnMerge();
							}
							break;
					}
				}
			}
		}
		protected override void Subscribe(BaseLayoutItem item) {
			base.Subscribe(item);
			BindingHelper.SetBinding(this, DocumentPanel.MDIMergeStyleProperty, item, DocumentPanel.MDIMergeStyleProperty);
		}
	}
	[TemplatePart(Name = "PART_Header", Type = typeof(UIElement))]
	[TemplatePart(Name = "PART_Content", Type = typeof(UIElement))]
	[TemplatePart(Name = "PART_InactiveLayer", Type = typeof(UIElement))]
	public class MDIDocument : BaseDocument {
		#region static
		public static readonly DependencyProperty IsMaximizedProperty;
		static MDIDocument() {
			var dProp = new DependencyPropertyRegistrator<MDIDocument>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("IsMaximized", ref IsMaximizedProperty, false,
				(dObj, e) => ((MDIDocument)dObj).OnIsMaximizedChanged((bool)e.OldValue, (bool)e.NewValue));
			EventManager.RegisterClassHandler(typeof(MDIDocument), System.Windows.Input.AccessKeyManager.AccessKeyPressedEvent,
				new System.Windows.Input.AccessKeyPressedEventHandler(OnAccessKeyPressed));
		}
		static void OnAccessKeyPressed(object sender, System.Windows.Input.AccessKeyPressedEventArgs e) {
			MDIDocument document = sender as MDIDocument;
			if(document != null) {
				BaseLayoutItem item = DockLayoutManager.GetLayoutItem(document);
				if(item != null && !item.IsActive) {
					if(Core.Native.LayoutHelper.IsChildElement(document, e.Target))
						e.Target = null;
				}
			}
		}
		#endregion static
		public MDIDocument() {
			KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Cycle);
			KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Cycle);
		}
		protected override void OnDispose() {
			UnSubscribeInactiveLayer();
			ClearValue(DocumentPanel.MDILocationProperty);
			ClearValue(DocumentPanel.MDISizeProperty);
			ClearValue(MDIStateHelper.MDIStateProperty);
			ClearValue(MDIPanel.ZIndexProperty);
			base.OnDispose();
		}
		public bool IsMaximized {
			get { return (bool)GetValue(IsMaximizedProperty); }
			set { SetValue(IsMaximizedProperty, value); }
		}
		protected UIElement PartHeader { get; private set; }
		protected UIElement PartContent { get; private set; }
		protected UIElement InactiveLayer { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetConstraintBindings();
			PartHeader = GetTemplateChild("PART_Header") as UIElement;
			PartContent = GetTemplateChild("PART_Content") as UIElement;
			InactiveLayer = GetTemplateChild("PART_InactiveLayer") as UIElement;
			SubscribeInactiveLayer();
			UpdateVisualState();
		}
		void SetConstraintBindings() {
			SetBinding(MinWidthProperty, new System.Windows.Data.Binding("ActualMinSize") { Converter = new MinSizeConverter(true) });
			SetBinding(MinHeightProperty, new System.Windows.Data.Binding("ActualMinSize") { Converter = new MinSizeConverter(false) });
			SetBinding(MaxWidthProperty, new System.Windows.Data.Binding("ActualMaxSize") { Converter = new MaxSizeConverter(true) });
			SetBinding(MaxHeightProperty, new System.Windows.Data.Binding("ActualMaxSize") { Converter = new MaxSizeConverter(false) });
		}
		void SetBindings(DocumentPanel document) {
			if(document == null) return;
			BindingHelper.SetBinding(this, DocumentPanel.MDILocationProperty, document, DocumentPanel.MDILocationProperty, System.Windows.Data.BindingMode.TwoWay);
			BindingHelper.SetBinding(this, DocumentPanel.MDISizeProperty, document, DocumentPanel.MDISizeProperty, System.Windows.Data.BindingMode.TwoWay);
			BindingHelper.SetBinding(this, MDIStateHelper.MDIStateProperty, document, MDIStateHelper.MDIStateProperty, System.Windows.Data.BindingMode.TwoWay);
			BindingHelper.SetBinding(this, DocumentPanel.MDIMergeStyleProperty, document, DocumentPanel.MDIMergeStyleProperty);
			BindingHelper.SetBinding(this, IsMaximizedProperty, document, DocumentPanel.IsMaximizedProperty);
			BindingHelper.SetBinding(this, MDIPanel.ZIndexProperty, document, MDIPanel.ZIndexProperty, System.Windows.Data.BindingMode.TwoWay);
		}
		protected override void Subscribe(BaseLayoutItem item) {
			base.Subscribe(item);
			if(item != null) {
				item.VisualChanged += OnItemVisualChanged;
				SetBindings(item as DocumentPanel);
			}
		}
		protected override void Unsubscribe(BaseLayoutItem item) {
			base.Unsubscribe(item);
			if(item != null)
				item.VisualChanged -= OnItemVisualChanged;
		}
		void OnItemVisualChanged(object sender, EventArgs e) {
			UpdateVisualState();
		}
		protected virtual void UpdateVisualState() {
			if(DocumentPanel != null) {
				if(DocumentPanel.IsActive)
					VisualStateManager.GoToState(this, "Active", false);
				else
					VisualStateManager.GoToState(this, "Inactive", false);
				if(DocumentPanel.IsMaximized)
					VisualStateManager.GoToState(this, "Maximized", false);
				else
					if(DocumentPanel.IsMinimized)
						VisualStateManager.GoToState(this, "Minimized", false);
					else
						VisualStateManager.GoToState(this, "EmptyMDIState", false);
			}
		}
		void SubscribeInactiveLayer() {
			if(InactiveLayer == null) return;
			InactiveLayer.AllowDrop = true;
			InactiveLayer.MouseDown += InactiveLayer_MouseDown;
			InactiveLayer.DragEnter += InactiveLayer_DragEnter;
			if(DocumentPanel != null)
				BindingHelper.SetBinding(InactiveLayer, UIElement.IsHitTestVisibleProperty, DocumentPanel, 
					DocumentPanel.EnableMouseHoverWhenInactiveProperty, new Core.BoolInverseConverter());
		}
		void UnSubscribeInactiveLayer() {
			if(InactiveLayer == null) return;
			InactiveLayer.MouseDown -= InactiveLayer_MouseDown;
			InactiveLayer.DragEnter -= InactiveLayer_DragEnter;
			BindingHelper.ClearBinding(InactiveLayer, UIElement.IsHitTestVisibleProperty);
		}
		void InactiveLayer_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			if(PartContent == null) return;
			IInputElement inputElement = PartContent.InputHitTest(e.GetPosition(InactiveLayer));
			if(inputElement != null)
				inputElement.RaiseEvent(e);
		}
		void InactiveLayer_DragEnter(object sender, DragEventArgs e) {
			if(Container != null) {
				Container.Activate(LayoutItem);
			}
		}
		protected override void OnActualSizeChanged(Size value) {
			base.OnActualSizeChanged(value);
			if(IsDisposing) return;
			DocumentPanel dPanel = DockLayoutManager.GetLayoutItem(this) as DocumentPanel;
			if(dPanel != null) {
				dPanel.MDIDocumentSize = value;
				DocumentGroup dGroup = dPanel.Parent as DocumentGroup;
				if(dGroup != null && PartHeader != null) {
					dGroup.MDIDocumentHeaderHeight = Math.Max(
							PartHeader.RenderSize.Height, dGroup.MDIDocumentHeaderHeight
						);
				}
			}
		}		
		protected override void ProcessMergeActions(BaseLayoutItem item) {
			if(item == null || !((DocumentPanel)item).IsMaximized || !item.IsSelectedItem)
				UnMerge();
			else {
				if(((DocumentPanel)item).IsMaximized && item.IsSelectedItem) {
					MDIMergeStyle mergeStyle = MDIControllerHelper.GetActualMDIMergeStyle(item.GetDockLayoutManager(), item);
					switch(mergeStyle) {
						case MDIMergeStyle.Always:
							BeginMerge();
							break;
						case MDIMergeStyle.Never:
							UnMerge();
							break;
						default:
							if(item.IsActive)
								BeginMerge();
							else {
								UnMerge();
							}
							break;
					}
				}
			}
		}
		protected virtual void OnIsMaximizedChanged(bool oldValue, bool newValue) {
			CheckIsChildMenuVisible();
			ProcessMergeActions(DocumentPanel);
			UpdateMDITitles();
		}
		protected override bool GetIsChildMenuVisible() {
			return GetIsChildMenuVisibleForFloatingElement(!IsMaximized);
		}
		protected override void OnIsActiveChanged(bool oldValue, bool newValue) {
			base.OnIsActiveChanged(oldValue, newValue);
			UpdateMDITitles();
		}
		void UpdateMDITitles() {
			if(IsActive && IsMaximized) MDIControllerHelper.MergeMDITitles(this);
		}
	}
	public class FloatDocument : Document {
		static FloatDocument() {
			var dProp = new DependencyPropertyRegistrator<FloatDocument>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		public FloatDocument() {
		}
	}
}
