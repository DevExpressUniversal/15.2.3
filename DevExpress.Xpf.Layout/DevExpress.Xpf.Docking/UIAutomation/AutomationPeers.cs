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
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using DevExpress.Xpf.Docking.Platform;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Docking.VisualElements;
#if !SILVERLIGHT
using SWC = System.Windows.Controls;
using System.Windows.Controls;
#else
#endif
namespace DevExpress.Xpf.Docking.UIAutomation {
	static class TransformProviderHelper {
#if !SILVERLIGHT
		public static Point PointToScreen(FrameworkElement target, Point point) {
			return target.PointToScreen(point);
		}
		public static Point PointFromScreen(FrameworkElement target, Point point) {
			return target.PointFromScreen(point);
		}
#else
		public static Point PointToScreen(FrameworkElement target, Point point) {
			return CoordinateHelper.PointToScreen(target, point);
		}
		public static Point PointFromScreen(FrameworkElement target, Point point) {
			return CoordinateHelper.PointFromScreen(target, point);
		}
#endif
	}
	public interface IFloatingPane {
		DockLayoutManager Manager { get; }
		FloatGroup FloatGroup { get; }
	}
	static class AutomationIdHelper {
		public static string GetId(string prefix, object owner) {
			return string.Format("{0}_{1}", prefix, GetId(owner));
		}
		public static string GetId(object owner) {
			return string.Format("{0}({1})", owner.GetType().Name, owner.GetHashCode());
		}
		public static string GetIdByLayoutItem(DependencyObject dObject) {
			BaseLayoutItem item = (dObject as BaseLayoutItem) ?? DockLayoutManager.GetLayoutItem(dObject);
			if(item == null) return GetId(dObject);
			string automationId = item.ReadLocalValue(AutomationProperties.AutomationIdProperty) as string;
			if(!string.IsNullOrEmpty(automationId))
				return automationId;
			if(!string.IsNullOrEmpty(item.Name))
				return item.Name;
			if(!string.IsNullOrEmpty(item.ActualCaption))
				return item.ActualCaption;
			return GetId(dObject.GetType().Name, item);
		}
		public static string GetLayoutItemName(BaseLayoutItem item, string defaultName) {
			if(item == null) return defaultName;
			string automationName = item.ReadLocalValue(AutomationProperties.NameProperty) as string;
			if(!string.IsNullOrEmpty(automationName))
				return automationName;
			if(!string.IsNullOrEmpty(item.ActualCaption))
				return item.ActualCaption;
			if(!string.IsNullOrEmpty(item.Name))
				return item.Name;
			return defaultName;
		}
	}
	public class BaseLayoutItemAutomationPeer : FrameworkElementAutomationPeer  {
		public BaseLayoutItemAutomationPeer(BaseLayoutItem item)
			: base(item) {
		}
		public new BaseLayoutItem Owner {
			get { return (BaseLayoutItem)base.Owner; }
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Group;
		}
		protected override string GetClassNameCore() {
			return Owner.GetType().Name;
		}
		protected override string GetNameCore() {
			return AutomationIdHelper.GetLayoutItemName(Owner, Owner.GetType().Name);
		}
		protected override void SetFocusCore() {
			Owner.Manager.Activate(Owner);
		}
		protected override string GetAutomationIdCore() {
			return AutomationIdHelper.GetIdByLayoutItem(Owner);
		}
	}
	public class TabbedItemAutomationPeer : BaseLayoutItemAutomationPeer, ISelectionItemProvider {
		public TabbedItemAutomationPeer(BaseLayoutItem item)
			: base(item) {
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.SelectionItem)
				return this;
			return base.GetPattern(patternInterface);
		}
		#region ISelectionItemProvider Members
		public void AddToSelection() {
			throw new NotSupportedException();
		}
		public bool IsSelected {
			get { return Owner.IsSelectedItem; }
		}
		public void RemoveFromSelection() {
			throw new NotSupportedException();
		}
		public void Select() {
			SetFocusCore();
		}
		public System.Windows.Automation.Provider.IRawElementProviderSimple SelectionContainer {
			get {
				AutomationPeer parent = GetParent();
				return ProviderFromPeer(parent);
			}
		}
		#endregion
	}
	public class AutoHideItemAutomationPeer : BaseLayoutItemAutomationPeer, IExpandCollapseProvider {
		public AutoHideItemAutomationPeer(BaseLayoutItem frameworkElement)
			: base(frameworkElement) {
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.ExpandCollapse)
				return this;
			return base.GetPattern(patternInterface);
		}
		#region IExpandCollapseProvider Members
		public void Collapse() {
			AutoHideTray autoHideTray = GetAutoHideTray();
			if(autoHideTray.Items.Count == 0)
				throw new InvalidOperationException();
			autoHideTray.DoCollapse();
		}
		public void Expand() {
			AutoHideTray autoHideTray = GetAutoHideTray();
			if(autoHideTray.Items.Count == 0)
				throw new InvalidOperationException();
			autoHideTray.DoExpand(Owner);
		}
		public System.Windows.Automation.ExpandCollapseState ExpandCollapseState {
			get {
				AutoHideTray autoHideTray = GetAutoHideTray();
				if(autoHideTray.Items.Count == 0)
					return System.Windows.Automation.ExpandCollapseState.LeafNode;
				if(autoHideTray.IsAnimated)
					return System.Windows.Automation.ExpandCollapseState.PartiallyExpanded;
				if(autoHideTray.IsExpanded)
					return System.Windows.Automation.ExpandCollapseState.Expanded;
				return System.Windows.Automation.ExpandCollapseState.Collapsed;
			}
		}
		protected internal void RaiseExpandCollapseAutomationEvent(bool value) {
			RaisePropertyChangedEvent(System.Windows.Automation.ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
				!value ? System.Windows.Automation.ExpandCollapseState.Expanded : System.Windows.Automation.ExpandCollapseState.Collapsed,
				value ? System.Windows.Automation.ExpandCollapseState.Expanded : System.Windows.Automation.ExpandCollapseState.Collapsed);
		}
		#endregion
		AutoHideTray GetAutoHideTray() {
			return ((AutoHidePaneHeaderItem)Owner.Manager.GetUIElement(Owner)).HeadersGroup.Tray;
		}
	}
	public class MDIDocumentAutomationPeer : BaseLayoutItemAutomationPeer, IWindowProvider, ITransformProvider {
		public MDIDocumentAutomationPeer(BaseLayoutItem frameworkElement)
			: base(frameworkElement) {
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Window)
				return this;
			if(patternInterface == PatternInterface.Transform)
				return this;
			return base.GetPattern(patternInterface);
		}
		public DocumentGroup DocumentGroup { get { return Owner.Parent as DocumentGroup; } }
		protected internal DockLayoutManager Manager { get { return Owner.Manager; } }
		public new DocumentPanel Owner { get { return base.Owner as DocumentPanel; } }
		#region IWindowProvider Members
		public void Close() {
			Manager.DockController.Close(Owner);
		}
		public System.Windows.Automation.WindowInteractionState InteractionState {
			get { throw new NotImplementedException(); }
		}
		public bool IsModal {
			get { return false; }
		}
		public bool IsTopmost {
			get { return false; }
		}
		public bool Maximizable {
			get { return true; }
		}
		public bool Minimizable {
			get { return true; }
		}
		public void SetVisualState(System.Windows.Automation.WindowVisualState state) {
			switch(state) {
				case System.Windows.Automation.WindowVisualState.Maximized:
					Manager.MDIController.Maximize(Owner);
					break;
				case System.Windows.Automation.WindowVisualState.Minimized:
					Manager.MDIController.Minimize(Owner);
					break;
				case System.Windows.Automation.WindowVisualState.Normal:
					Manager.MDIController.Restore(Owner);
					break;
			}
		}
		public System.Windows.Automation.WindowVisualState VisualState {
			get {
				if(Owner.IsMaximized) return System.Windows.Automation.WindowVisualState.Maximized;
				if(Owner.IsMinimized) return System.Windows.Automation.WindowVisualState.Minimized;
				return System.Windows.Automation.WindowVisualState.Normal;
			}
		}
		public bool WaitForInputIdle(int milliseconds) {
			throw new NotImplementedException();
		}
		#endregion
		#region ITransformProvider Members
		public bool CanMove {
			get { return true; }
		}
		public bool CanResize {
			get { return true; }
		}
		public bool CanRotate {
			get { return false; }
		}
		protected override Rect GetBoundingRectangleCore() {
			var panel = Core.Native.LayoutHelper.FindParentObject<MDIPanel>(Owner);
			if(panel != null) {
				Point mdiLocation = DocumentPanel.GetMDILocation(Owner);
				Size mdiSize = DocumentPanel.GetMDISize(Owner);
				Point location = TransformProviderHelper.PointToScreen(panel, mdiLocation);
				Size size = Layout.Core.MathHelper.IsEmpty(mdiSize) ? Owner.RenderSize : mdiSize;
				return new Rect(location, size);
			}
			return base.GetBoundingRectangleCore();
		}
		public void Move(double x, double y) {
			var panel = Core.Native.LayoutHelper.FindParentObject<MDIPanel>(Owner);
			Point location = TransformProviderHelper.PointFromScreen(panel, new Point(x, y));
			DocumentPanel.SetMDILocation(Owner, location);
		}
		public void Resize(double width, double height) {
			DocumentPanel.SetMDISize(Owner, new Size(width, height));
		}
		public void Rotate(double degrees) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class ClosedPanelItemAutomationPeer : BaseLayoutItemAutomationPeer {
		public ClosedPanelItemAutomationPeer(BaseLayoutItem item)
			: base(item) {
		}
		protected override void SetFocusCore() {
			Owner.Manager.DockController.Restore(Owner);
		}
	}
	public class AutoHideGroupAutomationPeer : LayoutGroupAutomationPeer, IDockProvider {
		public AutoHideGroupAutomationPeer(BaseLayoutItem element)
			: base(element) {
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Dock)
				return this;
			return base.GetPattern(patternInterface);
		}
		protected override void SetFocusCore() {
			if(Owner.SelectedItem != null) {
				Owner.Manager.Activate(Owner.SelectedItem);
				return;
			}
			if(Owner.Items.Count > 0) {
				Owner.Manager.Activate(Owner.Items[0]);
				return;
			}
			base.SetFocusCore();
		}
		#region IDockProvider Members
		public System.Windows.Automation.DockPosition DockPosition {
			get {
				var dockType = ((AutoHideGroup)Owner).DockType;
				switch(dockType) {
					case Dock.Left:
						return System.Windows.Automation.DockPosition.Left;
					case Dock.Top:
						return System.Windows.Automation.DockPosition.Top;
					case Dock.Right:
						return System.Windows.Automation.DockPosition.Right;
					case Dock.Bottom:
						return System.Windows.Automation.DockPosition.Bottom;
				}
				return System.Windows.Automation.DockPosition.None;
			}
		}
		public void SetDockPosition(System.Windows.Automation.DockPosition dockPosition) {
			AutoHideGroup autoHideGroup = (AutoHideGroup)Owner;
			if(autoHideGroup == null) return;
			switch(dockPosition) {
				case System.Windows.Automation.DockPosition.Left:
					autoHideGroup.DockType = Dock.Left;
					break;
				case System.Windows.Automation.DockPosition.Top:
					autoHideGroup.DockType = Dock.Top;
					break;
				case System.Windows.Automation.DockPosition.Right:
					autoHideGroup.DockType = Dock.Right;
					break;
				case System.Windows.Automation.DockPosition.Bottom:
					autoHideGroup.DockType = Dock.Bottom;
					break;
				default:
					throw new InvalidOperationException();
			}
		}
		#endregion
	}
	public class LayoutGroupAutomationPeer : FrameworkElementAutomationPeer, ISelectionProvider {
		public LayoutGroupAutomationPeer(BaseLayoutItem frameworkElement)
			: base(frameworkElement) {
#if !SILVERLIGHT
			LayoutGroup group = frameworkElement as LayoutGroup;
			if(group != null) {
				weakItemsChangedHandler = new EventHandler(group_WeakItemsChangedHandler);
				group.WeakItemsChanged += weakItemsChangedHandler;
			}
#endif
		}
#if !SILVERLIGHT
		readonly EventHandler weakItemsChangedHandler;
		void ResetHeadersPanel() {
			if(Owner == null) return;
			BaseHeadersPanel headersPanel = LayoutItemsHelper.GetTemplateChild<BaseHeadersPanel>(Owner);
			if(headersPanel != null) {
				AutomationPeer createPeerForElement = CreatePeerForElement(headersPanel);
				if(createPeerForElement != null) createPeerForElement.ResetChildrenCache();
			}
		}
		void group_WeakItemsChangedHandler(object sender, EventArgs e) {
			this.ResetChildrenCache();
			Dispatcher.BeginInvoke(new Action(ResetHeadersPanel));
		}
#endif
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> result = new List<AutomationPeer>();
			LayoutGroup group = Owner as LayoutGroup;
			foreach(BaseLayoutItem item in group.Items) {
				if(item is BaseLayoutItem)
					result.Add(CreatePeerForElement(item));
			}
			BaseHeadersPanel headersPanel = LayoutItemsHelper.GetTemplateChild<BaseHeadersPanel>(Owner);
			if(headersPanel != null)
				result.Add(CreatePeerForElement(headersPanel));
			GroupBoxControlBoxControl groupControlBox = LayoutItemsHelper.GetTemplateChild<GroupBoxControlBoxControl>(Owner);
			if(groupControlBox != null) {
				if(groupControlBox.PartExpandButton != null)
					result.Add(CreatePeerForElement(groupControlBox.PartExpandButton));
			}
			TabHeaderControlBoxControl headerControlBox = LayoutItemsHelper.GetTemplateChild<TabHeaderControlBoxControl>(Owner);
			if(headerControlBox != null) {
				if(headerControlBox.PartCloseButton != null)
					result.Add(CreatePeerForElement(headerControlBox.PartCloseButton));
				if(headerControlBox.PartScrollPrevButton != null)
					result.Add(CreatePeerForElement(headerControlBox.PartScrollPrevButton));
				if(headerControlBox.PartScrollNextButton != null)
					result.Add(CreatePeerForElement(headerControlBox.PartScrollNextButton));
				if(headerControlBox.PartDropDownButton != null)
					result.Add(CreatePeerForElement(headerControlBox.PartDropDownButton));
				if(headerControlBox.PartRestoreButton != null)
					result.Add(CreatePeerForElement(headerControlBox.PartRestoreButton));
			}
			return result;
		}
		public new LayoutGroup Owner {
			get { return (LayoutGroup)base.Owner; }
		}
		public LayoutGroupAutomationPeer(FloatPanePresenter.FloatingContentPresenter frameworkElement)
			: base(frameworkElement) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return Owner.IsTabHost  ? AutomationControlType.Tab :AutomationControlType.Group;
		}
		protected override string GetClassNameCore() {
			return Owner.GetType().Name;
		}
		protected override string GetNameCore() {
			return Owner.GetType().Name;
		}
		protected override void SetFocusCore() {
			Owner.Manager.Activate(Owner);
		}
		protected override string GetAutomationIdCore() {
			return AutomationIdHelper.GetIdByLayoutItem(Owner);
		}
		#region ISelectionProvider Members
		public bool CanSelectMultiple {
			get { return false; }
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(Owner.IsTabHost) {
				if(patternInterface == PatternInterface.Selection)
					return this;
			}
			return base.GetPattern(patternInterface);
		}
		public IRawElementProviderSimple[] GetSelection() {
			TabbedGroup tabbedGroup = (TabbedGroup)Owner;
			AutomationPeer peerForSelectedItem = CreatePeerForElement(tabbedGroup.SelectedItem);
			return new System.Windows.Automation.Provider.IRawElementProviderSimple[] { ProviderFromPeer(peerForSelectedItem) };
		}
		public bool IsSelectionRequired {
			get { return true; }
		}
		#endregion
	}
	public class DockLayoutManagerAutomationPeer : FrameworkElementAutomationPeer {
		public DockLayoutManagerAutomationPeer(psvControl control)
			: base(control) {
		}
		protected override string GetNameCore() {
			return "DockLayoutManager";
		}
		protected override string GetAutomationIdCore() {
			if(!Object.Equals(Owner.ReadLocalValue(AutomationProperties.AutomationIdProperty), DependencyProperty.UnsetValue)) {
				return (string)Owner.GetValue(AutomationProperties.AutomationIdProperty);
			}
			return AutomationIdHelper.GetIdByLayoutItem(Owner);
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Group;
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> result = new List<AutomationPeer>();
			DockLayoutManager manager = Owner as DockLayoutManager;
			if(manager == null || manager.IsDisposing) return result;
			foreach(BaseLayoutItem item in manager.AutoHideGroups) {
				if(item is LayoutPanel || item is LayoutGroup)
					result.Add(CreatePeerForElement(item));
			}
			foreach(BaseLayoutItem item in manager.ClosedPanels) {
				if(item is LayoutPanel || item is LayoutGroup)
					result.Add(CreatePeerForElement(item));
			}
#if SILVERLIGHT
			IUIElement[] children = ((IUIElement)manager).Children.GetElements();
			foreach(IUIElement e in children ) {
				FloatPanePresenter floatPanePresenter = e as FloatPanePresenter;
				if(floatPanePresenter!=null) { 
					result.Add(CreatePeerForElement(floatPanePresenter.Presenter));
				}
			}
#endif
			if(manager.LayoutRoot != null)
				result.Add(CreatePeerForElement(manager.LayoutRoot));
			return result;
		}
	}
	public class BasePanePresenterAutomationPeer<TVisual, TLogical> : FrameworkElementAutomationPeer
		where TVisual : DependencyObject, IDisposable
		where TLogical : BaseLayoutItem {
		public BasePanePresenterAutomationPeer(BasePanePresenter<TVisual, TLogical> presenter)
			: base(presenter as FrameworkElement) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Group;
		}
		protected override string GetClassNameCore() {
			return typeof(TVisual).Name;
		}
		protected override string GetNameCore() {
			return ((BasePanePresenter<TVisual, TLogical>)Owner).Name;
		}
		protected override string GetAutomationIdCore() {
			return AutomationIdHelper.GetIdByLayoutItem(Owner);
		}
	}
	public class ControlBoxAutomationPeer : FrameworkElementAutomationPeer {
		public ControlBoxAutomationPeer(BaseControlBoxControl controlBoxControl)
			: base(controlBoxControl) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Group;
		}
		protected override string GetClassNameCore() {
			return "BaseControlBoxControl";
		}
		protected override string GetNameCore() {
			return ((BaseControlBoxControl)Owner).Name;
		}
	}
	public class ControlBoxButtonPresenterAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider {
		public ControlBoxButtonPresenterAutomationPeer(ControlBoxButtonPresenter controlBoxButtonPresenter)
			: base(controlBoxButtonPresenter) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Button;
		}
		protected override string GetClassNameCore() {
			return "ControlBoxButtonPresenter";
		}
		protected override string GetNameCore() {
			return ((ControlBoxButtonPresenter)Owner).Name;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Invoke) {
				return this;
			}
			return base.GetPattern(patternInterface);
		}
		protected override void SetFocusCore() {
			DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(Owner);
			BaseLayoutItem item = DockLayoutManager.GetLayoutItem(Owner);
			if(item != null)
				manager.DockController.Activate(item);
		}
		void System.Windows.Automation.Provider.IInvokeProvider.Invoke() {
			if(!base.IsEnabled()) {
				throw new System.Windows.Automation.ElementNotEnabledException();
			}
			InvokeHelper.BeginInvoke(Owner, new Action(InvokeClick), InvokeHelper.Priority.Input);
		}
		protected override bool IsEnabledCore() {
			return ((ControlBoxButtonPresenter)Owner).PartButton != null;
		}
		void InvokeClick() {
			if(((ControlBoxButtonPresenter)Owner).AutomationClick())
				RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
		}
		protected override string GetAutomationIdCore() {
			return AutomationIdHelper.GetIdByLayoutItem(Owner);
		}
	}
	public class CaptionControlAutomationPeer : FrameworkElementAutomationPeer {
		public CaptionControlAutomationPeer(CaptionControl captionControl)
			: base(captionControl) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Group;
		}
		protected override string GetClassNameCore() {
			return "CaptionControl";
		}
		protected override string GetNameCore() {
			return ((CaptionControl)Owner).Name;
		}
		protected override void SetFocusCore() {
			throw new InvalidOperationException();
		}
		protected override string GetAutomationIdCore() {
			return AutomationIdHelper.GetIdByLayoutItem(Owner);
		}
	}
	public class BaseHeadersPanelAutomationPeer : FrameworkElementAutomationPeer {
		public BaseHeadersPanelAutomationPeer(BaseHeadersPanel headersPanel)
			: base(headersPanel) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Group;
		}
		protected override string GetClassNameCore() {
			return "BaseHeadersPanel";
		}
		protected override string GetNameCore() {
			return ((BaseHeadersPanel)Owner).Name;
		}
	}	
	public class TabbedPaneItemAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider {
		public TabbedPaneItemAutomationPeer(TabbedPaneItem paneItem)
			: base(paneItem) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Button;
		}
		protected override string GetClassNameCore() {
			return "TabbedPaneItem";
		}
		protected override string GetNameCore() {
			BaseLayoutItem item = DockLayoutManager.GetLayoutItem(Owner);
			return AutomationIdHelper.GetLayoutItemName(item, string.Empty) + "TabButton";
		}
		protected override string GetAutomationIdCore() {
			return AutomationIdHelper.GetIdByLayoutItem(Owner) + "TabButtonId";
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Invoke) 
				return this;
			return base.GetPattern(patternInterface);
		}
		protected override void SetFocusCore() {
			DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(Owner);
			BaseLayoutItem item = DockLayoutManager.GetLayoutItem(Owner);
			if(item != null)
				manager.DockController.Activate(item);
		}
		void System.Windows.Automation.Provider.IInvokeProvider.Invoke() {
			if(!base.IsEnabled()) {
				throw new System.Windows.Automation.ElementNotEnabledException();
			}
			InvokeHelper.BeginInvoke(Owner, new Action(InvokeClick), InvokeHelper.Priority.Input);
		}
		protected override bool IsEnabledCore() {
			BaseLayoutItem item = DockLayoutManager.GetLayoutItem(Owner);
			return item != null && item.IsEnabled;
		}
		void InvokeClick() {
			if(((TabbedPaneItem)Owner).AutomationClick())
				RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
		}
	}
	#region FloatPane
	public class BaseFloatingPaneAutomationPeer : FrameworkElementAutomationPeer, IWindowProvider, ITransformProvider {
		public BaseFloatingPaneAutomationPeer(FrameworkElement element)
			: base(element) {
		}
		protected internal FloatGroup FloatGroup { get { return ((IFloatingPane)Owner).FloatGroup; } }
		protected internal DockLayoutManager Manager { get { return ((IFloatingPane)Owner).Manager; } }
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Window)
				return this;
			if(patternInterface == PatternInterface.Transform)
				return this;
			return base.GetPattern(patternInterface);
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Window;
		}
		protected override string GetAutomationIdCore() {
			return AutomationIdHelper.GetIdByLayoutItem(Owner);
		}
		protected override string GetClassNameCore() {
			return FloatGroup.GetType().Name;
		}
		protected override string GetNameCore() {
			var floatGroup = this.FloatGroup;
			if(floatGroup != null && floatGroup.Items.Count != 0) {
				return AutomationIdHelper.GetLayoutItemName(floatGroup.Items[0], "FloatPaneWindow");
			}
			return string.Empty;
		}
		protected override void SetFocusCore() {
			Manager.Activate(FloatGroup);
		}
		#region IWindowProvider Members
		public void Close() {
			Manager.DockController.Close(FloatGroup);
		}
		public WindowInteractionState InteractionState {
			get { return WindowInteractionState.Running; }
		}
		public bool IsModal {
			get { return false; }
		}
		public bool IsTopmost {
			get { return false; }
		}
		public bool Maximizable {
			get { return FloatGroup.IsMaximizable; }
		}
		public bool Minimizable {
			get { return false; }
		}
		public void SetVisualState(WindowVisualState state) {
			if(FloatGroup.IsMaximizable && state != WindowVisualState.Minimized) {
				if(FloatGroup.IsMaximized)
					Manager.MDIController.Restore((DocumentPanel)FloatGroup.Items[0]);
				else
					Manager.MDIController.Maximize((DocumentPanel)FloatGroup.Items[0]);
			}
			else throw new InvalidOperationException();
		}
		public WindowVisualState VisualState {
			get { return WindowVisualState.Normal; }
		}
		public bool WaitForInputIdle(int milliseconds) {
			throw new NotImplementedException();
		}
		#endregion
		#region ITransformProvider Members
		protected override Rect GetBoundingRectangleCore() {
			Point location = TransformProviderHelper.PointToScreen(Manager, FloatGroup.FloatLocation);
			return new Rect(location, FloatGroup.FloatSize);
		}
		public bool CanMove {
			get { return true; }
		}
		public bool CanResize {
			get { return true; }
		}
		public bool CanRotate {
			get { return false; }
		}
		public void Move(double x, double y) {
			FloatGroup.FloatLocation = TransformProviderHelper.PointFromScreen(Manager, new Point(x, y));
		}
		public void Resize(double width, double height) {
			FloatGroup.FloatSize = new Size(width, height);
		}
		public void Rotate(double degrees) {
			throw new InvalidOperationException();
		}
		#endregion
	}
#if !SILVERLIGHT
	public class FloatingPaneWindowAutomationPeer : BaseFloatingPaneAutomationPeer {
		public FloatingPaneWindowAutomationPeer(FloatingPaneWindow floatingPaneWindow)
			: base(floatingPaneWindow) {
		}
	}
	public class FloatingPaneAdornerElementAutomationPeer : BaseFloatingPaneAutomationPeer {
		public FloatingPaneAdornerElementAutomationPeer(FloatingPaneAdornerElement floatingPaneAdornerElement)
			: base(floatingPaneAdornerElement) {
		}
	}
#else
	public class FloatingContentPresenterAutomationPeer : BaseFloatingPaneAutomationPeer {
		public FloatingContentPresenterAutomationPeer(FloatPanePresenter.FloatingContentPresenter floatingContentPresenter)
			: base(floatingContentPresenter) {
		}
	}
#endif
	#endregion
}
