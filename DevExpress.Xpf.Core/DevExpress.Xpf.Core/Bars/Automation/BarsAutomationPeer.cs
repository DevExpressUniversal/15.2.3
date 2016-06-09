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
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows;
using DevExpress.Xpf.Bars;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Utils; 
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
using System.Collections.ObjectModel;
namespace DevExpress.Xpf.Bars.Automation {
	public static class BarsAutomationHelper {
		private class FocusDependentStates {
			public IInputElement oldFocusedElement = null;
			public bool oldFocusableValue = false;
			public Bar oldSelectedBar = null;
			public BarItemLinkControl oldSelectedLinkControl = null;
			public BarItemLinkControlAutomationPeer owner = null;
		}
		private static bool allowUIAutomationSupport=true;
		public static bool AllowUIAutomationSupport {
			get { return allowUIAutomationSupport; }
			set { allowUIAutomationSupport = value; }
		}
		public static string CreateAutomationID(DependencyObject obj, AutomationPeer peer, string targetValue = null) {
			if(AutomationIDs.ContainsKey(peer)) {
				if(targetValue != null)
					AutomationIDs[peer] = targetValue;
				return AutomationIDs[peer];
			}
			bool isTargetUnset = targetValue == null;
			if (isTargetUnset) {
				targetValue = CreateAutomationIDCore(obj);
			if(AutomationIDs.ContainsValue(targetValue)) {
				int indexer = 0;
					while (AutomationIDs.ContainsValue(targetValue + indexer++.ToString()))
						;
				targetValue += (indexer -1).ToString();				
			}
			}
			AutomationIDs.Add(peer, targetValue);
			return targetValue;
		}
		static string CreateAutomationIDCore(DependencyObject obj) {
			string targetValue = string.Empty;
			if(obj != null) {
				targetValue += obj.GetType().Name;
				targetValue += GetName(obj);  
			}
			return targetValue;
		}	 
		private static string GetName(DependencyObject obj) {
			if(obj is FrameworkElement && (obj as FrameworkElement).Name != string.Empty)
				return (obj as FrameworkElement).Name;
			if(obj is FrameworkContentElement && (obj as FrameworkContentElement).Name != string.Empty)
				return (obj as FrameworkContentElement).Name;
			return String.Empty;
		}
		static WeakReferenceDictionary<AutomationPeer, string> automationIDs;
		static WeakReferenceDictionary<AutomationPeer, string> AutomationIDs {
			get {
				if(automationIDs == null)
					automationIDs = new WeakReferenceDictionary<AutomationPeer, string>();
				return automationIDs; }
			set { automationIDs = value; }
		}
		public static DockPosition GetDockPosition(Bar bar) {
			BarContainerType containerType = bar.DockInfo.ContainerType;
			return (DockPosition)Enum.Parse(typeof(DockPosition), containerType.ToString().Replace("Floating", "None"), true);
		}
		public static void SetDockPosition(Bar bar, DockPosition position) {
			string positionString = position.ToString();
			if(positionString == "None") positionString = "Floating";
			else if(positionString == "Fill") positionString = "None";
			BarContainerType containerType = (BarContainerType)Enum.Parse(typeof(BarContainerType), positionString, true);
			bar.DockInfo.SetCurrentValue(BarDockInfo.ContainerTypeProperty, containerType);
		}
		public static void InvokeMethodWithFocus(BarItemLinkControlAutomationPeer peer, Action<BarItemLinkControlAutomationPeer> method) {
			var oldValue = peer.isFocused;
			peer.isFocused = true;
			peer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);	
			method(peer);			
			peer.isFocused = oldValue;
		}
		public static void GetChildrenRecursive(List<AutomationPeer> children, DependencyObject owner) {
			if(children == null || owner == null) return;
			AutomationPeer peer = null;
			TryToGetPeer(ref peer, owner);
			if(peer != null) {
				children.Add(peer);
				return;
			}
			int count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(owner);
			if(count == 0) return;
			for(int i = 0; i < count; i++) {
				var child = System.Windows.Media.VisualTreeHelper.GetChild(owner, i);
				GetChildrenRecursive(children, child);
			}
		}
		private static void TryToGetPeer(ref AutomationPeer peer, DependencyObject peerOwner) {
			if(peerOwner is UIElement)				
				peer = UIElementAutomationPeer.CreatePeerForElement(peerOwner as UIElement);
			if(peerOwner is ContentElement)
				peer = ContentElementAutomationPeer.CreatePeerForElement(peerOwner as ContentElement);
		}
		public static object TryGetAutomationPropertyValue(DependencyProperty attachedAutomationProperty, out bool succeeded, params Func<DependencyObject>[] getElementFunctions) {
			succeeded = false;
			foreach(Func<DependencyObject> getElementFunction in getElementFunctions) {				
				DependencyObject element = getElementFunction();
				if(element == null) continue;
				if (!Object.Equals(element.ReadLocalValue(attachedAutomationProperty), DependencyProperty.UnsetValue)) {
					succeeded = true;
					return element.GetValue(attachedAutomationProperty);
				}
			}
			return null;
		}
	}
#if DEBUGTEST
	public class BarsAutomationTesterHelper {
		private static List<AutomationProperty> propertychanges;
		public static List<AutomationProperty> PropertyChanges {
			get {
				if(propertychanges == null) propertychanges = new List<AutomationProperty>();
				return propertychanges; }
			set { propertychanges = value; }
		}
		private static List<AutomationEvents> eventInvokations;
		public static List<AutomationEvents> EventInvokations {
			get {
				if(eventInvokations == null) eventInvokations = new List<AutomationEvents>();
				return eventInvokations; }
			set { eventInvokations = value; }
		}
	}
#endif
	public abstract class BaseNavigationAutomationPeer : FrameworkElementAutomationPeer {		
		public BaseNavigationAutomationPeer(FrameworkElement element)
			: base(element) {				
				ClearAutomationEventsHelper.ClearAutomationEvents();			
		}
		protected object TryGetAutomationPropertyValue(DependencyProperty attachedAutomationProperty, out bool succeeded) {
			return BarsAutomationHelper.TryGetAutomationPropertyValue(attachedAutomationProperty, out succeeded, GetAttachedAutomationPropertySource());
		}
		protected virtual Func<DependencyObject>[] GetAttachedAutomationPropertySource() {
			return new Func<DependencyObject>[] { () => Owner };
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			return base.GetNameCore();
		}
		protected override string GetAutomationIdCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.AutomationIdProperty, out useAttachedValue);
			return DevExpress.Xpf.Bars.Automation.BarsAutomationHelper.CreateAutomationID(Owner, this, useAttachedValue ? (string)value : base.GetAutomationIdCore());
		}		
		protected override string GetAcceleratorKeyCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.AcceleratorKeyProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			return base.GetAcceleratorKeyCore();
		}
		protected override string GetAccessKeyCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.AccessKeyProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			return base.GetAccessKeyCore();
		}
		protected override string GetHelpTextCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.HelpTextProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			return base.GetHelpTextCore();
		}
		protected override bool IsRequiredForFormCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.IsRequiredForFormProperty, out useAttachedValue);
			if(useAttachedValue)
				return (bool)value;
			return base.IsRequiredForFormCore();
		}
		protected override string GetItemStatusCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.ItemStatusProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			return base.GetItemStatusCore();
		}
		protected override string GetItemTypeCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.ItemTypeProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			return base.GetItemTypeCore();
		}
		protected override AutomationPeer GetLabeledByCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.LabeledByProperty, out useAttachedValue);
			if(useAttachedValue)
				return (AutomationPeer)value;
			return base.GetLabeledByCore();
		}
		protected override string GetClassNameCore() {
			return Owner==null ? base.GetClassNameCore() : Owner.GetType().Name;
		}
	}
	public class BarManagerAutomationPeer : BaseNavigationAutomationPeer {
		private WeakReference barManagerWR = new WeakReference(null);
		public BarManager Manager {
			get { return (BarManager)barManagerWR.Target; }
			set { barManagerWR = new WeakReference(value); }
		}
		public BarManagerAutomationPeer(BarManager manager) : base(manager) {
			this.Manager = manager;
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Group;
		}
		protected override string GetAutomationIdCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.AutomationIdProperty, out useAttachedValue);
			return BarsAutomationHelper.CreateAutomationID(Manager, this, useAttachedValue ? (string)value : null);
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			if (Manager != null) {
				return "BarManager" + Manager.Name;
			} else {
				return "BarManager";
			}
		}
		protected override string GetClassNameCore() {
			return typeof(BarManager).Name;			
		}
		protected override bool IsEnabledCore() {
			return Manager==null ? false : Manager.IsEnabled;
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = base.GetChildrenCore();
			var manager = Manager;
			if(children == null || Manager==null)
				return null;			
			if(Manager.Containers!=null && Manager.Containers.Count(i=>i is FloatingBarContainerControl)>0)
				children.Add(CreatePeerForElement(Manager.Containers.First(i => i is FloatingBarContainerControl)));
			return children;
		}
		protected override Func<DependencyObject>[] GetAttachedAutomationPropertySource() {
			var manager = Manager;
			return new Func<DependencyObject>[]{
				 () => manager
			};
		}
	}
	public class BarContainerControlAutomationPeer : BaseNavigationAutomationPeer {
		public BarContainerControlAutomationPeer(BarContainerControl control)
			: base(control) {			
		}		
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Group;
		}
		protected override string GetAutomationIdCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.AutomationIdProperty, out useAttachedValue);
			return BarsAutomationHelper.CreateAutomationID(Owner, this, useAttachedValue ? (string)value : null);
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			BarContainerControl ctrl = (Owner as BarContainerControl);
			return ctrl.Name == String.Empty ? ctrl.ContainerType.ToString() +" "+ ctrl.GetType().Name : ctrl.Name;
		}
		protected override string GetClassNameCore() {
			return Owner.GetType().Name;
		}
		protected override Func<DependencyObject>[] GetAttachedAutomationPropertySource() {
			return new Func<DependencyObject>[]{
				 () => Owner
			};
		}
	}
	public class BarControlAutomationPeer : BaseNavigationAutomationPeer, IDockProvider, IExpandCollapseProvider, ITransformProvider {
		WeakReference barWR = new WeakReference(null);
		WeakReference managerPeerWR = new WeakReference(null);
		private WeakReference itemsControlAutomationPeerWR = new WeakReference(null);
		internal ItemsControlAutomationPeer ItemsControlAutomationPeer {
			get { return (ItemsControlAutomationPeer)itemsControlAutomationPeerWR.Target; }
			set { itemsControlAutomationPeerWR = new WeakReference(value); }
		}
		internal BarManagerAutomationPeer ManagerPeer {
			get { return (BarManagerAutomationPeer)managerPeerWR.Target; }
			set { managerPeerWR = new WeakReference(value); }
		}
		internal Bar Bar {
			get { return (Bar)barWR.Target; }
			set { barWR = new WeakReference(value); }
		}
		EventHandler onBarContainerTypeChangedHandler;
		public BarControlAutomationPeer(BarControl control)
			: base(control)  {
				this.Bar = control.Bar;
				ItemsControlAutomationPeer = GetParent() as ItemsControlAutomationPeer; 
				if(ItemsControlAutomationPeer != null)
					this.ManagerPeer = GetParent().GetParent() as BarManagerAutomationPeer;
			onBarContainerTypeChangedHandler = new EventHandler(OnBarContainerTypeChanged);
			Bar.DockInfo.ContainerTypeChanged += onBarContainerTypeChangedHandler;
		}
		void OnBarContainerTypeChanged(object sender, EventArgs e) {
			var tempObject = ItemsControlAutomationPeer;
			var tempOBject1 = ManagerPeer;
			if(ItemsControlAutomationPeer != null) {
				ItemsControlAutomationPeer.ResetChildrenCache();
				ItemsControlAutomationPeer.RaiseAutomationEvent(AutomationEvents.StructureChanged);
#if DEBUGTEST
				BarsAutomationTesterHelper.EventInvokations.Add(AutomationEvents.StructureChanged);
#endif
			}
			if(ManagerPeer != null) {
				ManagerPeer.ResetChildrenCache();
				ManagerPeer.RaiseAutomationEvent(AutomationEvents.StructureChanged);
#if DEBUGTEST
				BarsAutomationTesterHelper.EventInvokations.Add(AutomationEvents.StructureChanged);
#endif
			}
			if(positionRaised == true) positionRaised = false;
			else {
				RaisePropertyChangedEvent(DockPatternIdentifiers.DockPositionProperty, DockPosition.None, DockPosition);
#if DEBUGTEST
				BarsAutomationTesterHelper.PropertyChanges.Add(DockPatternIdentifiers.DockPositionProperty);
#endif
			}
		}	 
		protected override string GetNameCore() {
			var bar = Bar;
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue || bar==null)
				return (string)value;
			return String.IsNullOrEmpty(Bar.Caption) ? BarsAutomationHelper.CreateAutomationID(Bar, this, null) : Bar.Caption;
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.ToolBar;
		}
		protected override string GetAutomationIdCore() {
			var bar = Bar;
			if (bar == null)
				return "";
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.AutomationIdProperty, out useAttachedValue);
			return BarsAutomationHelper.CreateAutomationID(Bar, this, useAttachedValue? (string)value : null);
		}
		protected override string GetClassNameCore() {			
			return typeof(Bar).Name;			
		}
		protected override void SetFocusCore() {			
			var bar = Bar;
			if(bar==null)
				return;
			Bar.Focus();			
		}
		#region IDockProvider
		public DockPosition DockPosition {
			get { return BarsAutomationHelper.GetDockPosition(Bar); }
		}
		bool positionRaised = false;
		public void SetDockPosition(DockPosition dockPosition) {
			DockPosition oldValue = DockPosition;
			positionRaised = true;
			BarsAutomationHelper.SetDockPosition(Bar, dockPosition);			
			RaisePropertyChangedEvent(DockPatternIdentifiers.DockPositionProperty, oldValue, DockPosition);
#if DEBUGTEST
			BarsAutomationTesterHelper.PropertyChanges.Add(DockPatternIdentifiers.DockPositionProperty);
#endif
		}
		#endregion
		#region IExpandCollapseProvider
		public void Collapse() {			
			var bar = Bar;
			if (bar == null)
				return;
			ExpandCollapseState oldValue = ExpandCollapseState;
			Bar.IsCollapsed = true;			
			RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue, ExpandCollapseState);
#if DEBUGTEST
			BarsAutomationTesterHelper.PropertyChanges.Add(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty);
#endif
		}
		public void Expand() {			
			var bar = Bar;
			if (bar == null)
				return;
			ExpandCollapseState oldValue = ExpandCollapseState;
			Bar.IsCollapsed = false;
			RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue, ExpandCollapseState);
#if DEBUGTEST
			BarsAutomationTesterHelper.PropertyChanges.Add(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty);
#endif
		}
		public ExpandCollapseState ExpandCollapseState {
			get {
				var bar = Bar;
				if (bar == null)
					return System.Windows.Automation.ExpandCollapseState.LeafNode;
				return Bar.IsCollapsed == true ? ExpandCollapseState.Collapsed : ExpandCollapseState.Expanded;
			}
		}
		#endregion
		#region ITransformProvider
		public bool CanMove {
			get { return Bar.DockInfo.ContainerType == BarContainerType.Floating; }
		}
		public bool CanResize {
			get { return Bar.DockInfo.ContainerType == BarContainerType.Floating; }
		}
		public bool CanRotate {
			get { return false; }
		}
		public void Move(double x, double y) {
			if(Bar.DockInfo.ContainerType == BarContainerType.None || Bar.DockInfo.ContainerType == BarContainerType.Floating) {
				Bar.DockInfo.FloatBarOffset = new Point(x, y);
			} else {
				if(Bar.DockInfo.ContainerType == BarContainerType.Bottom || Bar.DockInfo.ContainerType == BarContainerType.Top)
					Bar.DockInfo.Offset = x;
				else
					Bar.DockInfo.Offset = y;
			}			
			Bar.DockInfo.BarControl.UpdateLayout();
		}
		public void Resize(double width, double height) {
			Bar.DockInfo.FloatBarWidth = width;
			Bar.DockInfo.BarControl.UpdateLayout();
		}
		public void Rotate(double degrees) {
			return;
		}
		#endregion
		public override object GetPattern(PatternInterface patternInterface) {
			var bar = Bar;
			switch(patternInterface){
				case PatternInterface.Dock:
					return this;
				case PatternInterface.ExpandCollapse:
					return Bar!=null && Bar.AllowCollapse == true ? this : null;
				case PatternInterface.Transform:
					return this;
				default:
					return null;
			}
		}
		protected override Func<DependencyObject>[] GetAttachedAutomationPropertySource() {
			return new Func<DependencyObject>[]{
				 () => Bar
			};
		}
	}
	public class BarItemLinkControlAutomationPeer : BaseNavigationAutomationPeer, IInvokeProvider {
		ItemClickEventHandler onBarItemClickHandler;
		ValueChangedEventHandler<bool> onIsHighlightedChangedHandler;
		public BarItemLinkControlAutomationPeer(BarItemLinkControl control)
			: base(control) {
				if(control.Link == null || control.Link.Item == null)
					return;
			BarItem item = control.Link.Item as BarItem;
			this.Dispatcher.BeginInvoke(new Action(() => {
				this.RaisePropertyChangedEvent(AutomationElementIdentifiers.AccessKeyProperty, null, GetAccessKey());
				this.RaisePropertyChangedEvent(AutomationElementIdentifiers.AcceleratorKeyProperty, null, GetAcceleratorKey());
			}));
			onBarItemClickHandler = new ItemClickEventHandler(OnBarItemClick);
			onIsHighlightedChangedHandler = new ValueChangedEventHandler<bool>(OnIsHighlightedChanged);
			item.WeakItemClick += onBarItemClickHandler;
			control.WeakIsHighlightedChanged += onIsHighlightedChangedHandler;
		}
		internal bool isFocused = false;
		void OnIsHighlightedChanged(object sender, ValueChangedEventArgs<bool> e) {
			var Manager = LinkControl.Manager;
			if (Manager == null)
				return;
			isFocused = NavigationTree.CurrentElement!=null && e.NewValue;
			var edit = (sender as BarEditItemLinkControl).With(x => x.Edit as DevExpress.Xpf.Editors.PopupBaseEdit);
			isFocused &= edit==null ? true : !edit.If(x => x.IsPopupOpen).ReturnSuccess();
			if (NavigationTree.CurrentElement!=null) {
				if (isFocused) {
					SetFocus();
				} else {
					RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
				}
			}
		}
		protected override bool HasKeyboardFocusCore() {
			return isFocused || base.HasKeyboardFocusCore();
		}
		#region IInvokeProvider
		bool justInvoked = false;
		public virtual void OnBarItemClick(object sender, ItemClickEventArgs e) {
			if(justInvoked == true) {
				justInvoked = false;
			} else {
				BarsAutomationHelper.InvokeMethodWithFocus(this, (peer) => { peer.RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
#if DEBUGTEST
				BarsAutomationTesterHelper.EventInvokations.Add(AutomationEvents.InvokePatternOnInvoked);
#endif
				});
			}
		}
		public virtual void Invoke() {
			if (IsEnabled() == false)
				throw new ElementNotEnabledException(); 
			Dispatcher.BeginInvoke(new Action(InvokeAsync));
		}
		void InvokeAsync() {
			BarItem item = (Owner as BarItemLinkControl).Link.Item;
			if (item.CanExecute) {
				justInvoked = true;
				item.OnItemClick((Owner as BarItemLinkControl).Link);
				RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
				RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
#if DEBUGTEST
				BarsAutomationTesterHelper.EventInvokations.Add(AutomationEvents.InvokePatternOnInvoked);
#endif
			}
		}
		#endregion
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Button;
		}
		protected BarItemLinkControl LinkControl {
			get { return Owner as BarItemLinkControl; }
		}
		protected override string GetAutomationIdCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.AutomationIdProperty, out useAttachedValue);
			var id = BarsAutomationHelper.CreateAutomationID((Owner as BarItemLinkControl).Link, this, useAttachedValue ? (string)value : null);
			if(useAttachedValue)
				return id;
			return id + (((Owner as BarItemLinkControl).Link != null && (Owner as BarItemLinkControl).Link.Item != null) ? (Owner as BarItemLinkControl).Link.Item.Name : String.Empty);
		}
		protected override string GetAccessKeyCore() {
			var baseValue = base.GetAccessKeyCore();
			if(!string.IsNullOrEmpty(baseValue) ||LinkControl==null || LinkControl.Manager==null || LinkControl.LinksControl==null)
				return baseValue;
			var stringContent = LinkControl.ActualContent as string;			
			if (stringContent == null)
				return baseValue;
			var result = Convert.ToString(stringContent.Select((current, index) => {
				if (index != (stringContent.Length - 1) && current == '_' && stringContent[index + 1] != '_' && (index == 0 || stringContent[index - 1] != '_')) {
					return stringContent[index + 1];
				}
				return (char)0;
			}).FirstOrDefault(current => current != (char)0));
			if (result != null) {
				result = result.ToUpper();
				if(LinkControl.ContainerType== LinkContainerType.MainMenu)
					result = "Alt+" + result;
			}
			AutomationProperties.SetAccessKey(Owner, result);
			return result;
		}
		protected override string GetAcceleratorKeyCore() {
			var baseValue = base.GetAcceleratorKeyCore();
			if(LinkControl==null || !string.IsNullOrEmpty(baseValue))
				return baseValue;
			AutomationProperties.SetAcceleratorKey(Owner, LinkControl.ActualKeyGestureText);
			return LinkControl.ActualKeyGestureText;
		}
		protected override bool IsKeyboardFocusableCore() {
			return true;
		}
		protected override bool IsEnabledCore() {
			return (Owner as BarItemLinkControl).IsEnabled;
		}
		protected override string GetClassNameCore() {
			return Owner.GetType().Name;
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return ((string)value);
			if(LinkControl == null || LinkControl.Link == null || LinkControl.Link.Item == null) return String.Empty;			
			if (LinkControl.ActualContent == null) {
			if(!String.IsNullOrEmpty(LinkControl.Link.Item.Name))
				return LinkControl.Link.Item.Name;
			}
			return Convert.ToString(LinkControl.ActualContent).Replace("_", "");
		}
		protected override void SetFocusCore() {
		}
		protected override Func<DependencyObject>[] GetAttachedAutomationPropertySource() {
			return new Func<DependencyObject>[]{
				 () => (Owner as BarItemLinkControl).LinkBase,
				() => ((Owner as BarItemLinkControl).Link != null ? (Owner as BarItemLinkControl).Link.Item : null)
			};
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if (patternInterface == PatternInterface.Invoke && LinkControl != null && LinkControl.Link != null)
				return this;
			return base.GetPattern(patternInterface);
		}
	}
	public class BarButtonItemLinkControlAutomationPeer : BarItemLinkControlAutomationPeer {
		public BarButtonItemLinkControlAutomationPeer(BarButtonItemLinkControl control) : base(control){			
		}				
		public override object GetPattern(PatternInterface patternInterface) {
			var baseValue = base.GetPattern(patternInterface);
			if (baseValue != null)
				return baseValue;
			return patternInterface == PatternInterface.Invoke ? this : null;
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			return new List<AutomationPeer>();
		}
	}
	public class BarCheckItemLinkControlAutomationPeer : BarItemLinkControlAutomationPeer, IToggleProvider {
		public BarCheckItemLinkControlAutomationPeer(BarCheckItemLinkControl control)
			: base(control) {
				BarCheckItem bchi = control.Link != null ? control.Link.Item as BarCheckItem : null;
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.CheckBox;
		}
		public override void OnBarItemClick(object sender, ItemClickEventArgs e) {
			BarsAutomationHelper.InvokeMethodWithFocus(this, (peer) => {
				peer.RaisePropertyChangedEvent(
					TogglePatternIdentifiers.ToggleStateProperty,
					ToggleState == ToggleState.Off ? ToggleState.On : ToggleState.Off,
					ToggleState);
#if DEBUGTEST
				BarsAutomationTesterHelper.PropertyChanges.Add(TogglePatternIdentifiers.ToggleStateProperty);
#endif
			});			
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			return new List<AutomationPeer>();
		}
		#region IToggleProvider        
		public virtual void Toggle() {			
			ToggleState oldState = ToggleState;
			if((Owner as BarCheckItemLinkControl).Link == null || (Owner as BarCheckItemLinkControl).Link.Item == null)
				return;
			((Owner as BarCheckItemLinkControl).Link.Item as BarCheckItem).Toggle();
			RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, oldState, ToggleState);
#if DEBUGTEST
			BarsAutomationTesterHelper.PropertyChanges.Add(TogglePatternIdentifiers.ToggleStateProperty);
#endif
		}
		public virtual ToggleState ToggleState {
			get {
				if((Owner as BarCheckItemLinkControl).Link == null || (Owner as BarCheckItemLinkControl).Link.Item == null)
					return ToggleState.Indeterminate;
				return ((Owner as BarCheckItemLinkControl).Link.Item as BarCheckItem).IsChecked == true ? ToggleState.On : ToggleState.Off; }
		}
		#endregion
		public override object GetPattern(PatternInterface patternInterface) {
			var baseValue = base.GetPattern(patternInterface);
			if (baseValue != null)
				return baseValue;
			return patternInterface == PatternInterface.Toggle ? this : null;
		}	   
	}
	public class BarSubItemLinkControlAutomationPeer : BarButtonItemLinkControlAutomationPeer, IExpandCollapseProvider {
		public BarSubItemLinkControlAutomationPeer(BarSubItemLinkControl control)
			: base(control) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Menu;
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = base.GetChildrenCore();
			if(children == null)
				return null;
			BarSubItemLinkControl subItemLinkControl = Owner as BarSubItemLinkControl;
			AutomationPeer peer = subItemLinkControl != null && subItemLinkControl.Popup != null && subItemLinkControl.Popup.PopupContent != null ? CreatePeerForElement(subItemLinkControl.Popup.PopupContent as UIElement) : null;
			if(peer != null)
				children.Add(peer);
			if(subItemLinkControl.SubItem == null || subItemLinkControl.SubItem.ItemLinks == null)
				return children;
			foreach (BarItemLinkBase link in ((Owner as BarSubItemLinkControl).SubItem.ItemLinks)) {
				if((link is BarItemLink) && (link as BarItemLink).LinkControl != null) {
					AutomationPeer itemLinkControlPeer = CreatePeerForElement((link as BarItemLink).LinkControl);
					if(itemLinkControlPeer != null)
						children.Add(itemLinkControlPeer);
				}
			}
			return children;
		}
		#region IExpandCollapseProvider
		public void Collapse() {
			BarSubItemLinkControl control = (Owner as BarSubItemLinkControl);
			if(control.IsPressed) {
				ExpandCollapseState oldValue = ExpandCollapseState;
				control.IsPressed = false;
				RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue, ExpandCollapseState);
#if DEBUGTEST
				BarsAutomationTesterHelper.PropertyChanges.Add(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty);
#endif
			}
		}
		public void Expand() {
			BarSubItemLinkControl control = (Owner as BarSubItemLinkControl);
			if(!control.IsPressed) {
				ExpandCollapseState oldValue = ExpandCollapseState;
				control.IsPressed = true;
				RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue, ExpandCollapseState);
#if DEBUGTEST
				BarsAutomationTesterHelper.PropertyChanges.Add(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty);
#endif
			}
		}
		public ExpandCollapseState ExpandCollapseState {
			get { return (Owner as BarSubItemLinkControl).IsPopupOpen == true ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed; }
		}
		#endregion
		public override object GetPattern(PatternInterface patternInterface) {
			var baseValue = base.GetPattern(patternInterface);
			if (baseValue != null)
				return baseValue;
			switch(patternInterface){
				case PatternInterface.Invoke:
					return this;
				case PatternInterface.ExpandCollapse:
					return this;
				default:
					return null;
			}
		}
	}
	public class BarSplitButtonItemLinkControlAutomationPeer : BarButtonItemLinkControlAutomationPeer, IExpandCollapseProvider {
		public BarSplitButtonItemLinkControlAutomationPeer(BarSplitButtonItemLinkControl control) : base(control) {
		}
		#region IExpandCollapseProvider
		public void Collapse() {
			BarSplitButtonItemLinkControl control = (Owner as BarSplitButtonItemLinkControl);
			if(control.IsPopupOpen) {
				ExpandCollapseState oldValue = ExpandCollapseState;
				control.ClosePopup();				
				RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue, ExpandCollapseState);
#if DEBUGTEST
				BarsAutomationTesterHelper.PropertyChanges.Add(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty);
#endif
			}
		}
		public void Expand() {
			BarSplitButtonItemLinkControl control = (Owner as BarSplitButtonItemLinkControl);
			if(!control.IsPopupOpen) {
				ExpandCollapseState oldValue = ExpandCollapseState;
				control.ShowPopup(); 
				RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue, ExpandCollapseState);
#if DEBUGTEST
				BarsAutomationTesterHelper.PropertyChanges.Add(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty);
#endif
			}
		}
		public ExpandCollapseState ExpandCollapseState {
			get { return (Owner as BarSplitButtonItemLinkControl).IsPopupOpen == true ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed; }
		}
		#endregion
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Button;
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = base.GetChildrenCore();
			if(children == null)
				return null;
			int count = children.Count;
			if((Owner as BarSplitButtonItemLinkControl).Link == null || (Owner as BarSplitButtonItemLinkControl).Link.Item == null)
				return children;
			if(((Owner as BarSplitButtonItemLinkControl).Link.Item as BarSplitButtonItem).PopupControl != null) {
				IPopupControl popupControl = ((Owner as BarSplitButtonItemLinkControl).Link.Item as BarSplitButtonItem).PopupControl;
				if(popupControl is UIElement && CreatePeerForElement(popupControl as UIElement)!=null)
					children.Add(CreatePeerForElement(popupControl as UIElement));
				if(children.Count == count) {
					object obj = BarManagerHelper.GetItemLinkControlPopup(Owner as BarSplitButtonItemLinkControl);
					if(obj is ILinksHolder) {
						foreach(BarItemLinkBase link in (obj as ILinksHolder).Links) {
							if(link != null && link.LinkInfos.Count > 0 && link.LinkInfos[0] != null && link.LinkInfos[0].LinkControl != null) {
								AutomationPeer peer = CreatePeerForElement(link.LinkInfos[0].LinkControl);
								if(peer != null)
									children.Add(peer);
							}
						}
					}
				}
				if(popupControl is ContentElement && ContentElementAutomationPeer.CreatePeerForElement(popupControl as ContentElement) != null)
					children.Add(ContentElementAutomationPeer.CreatePeerForElement(popupControl as ContentElement));				
			}
			return children;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			var baseValue = base.GetPattern(patternInterface);
			if (baseValue != null)
				return baseValue;
			switch(patternInterface) {
				case PatternInterface.Invoke:
					return this;
				case PatternInterface.ExpandCollapse:
					return this;
				default:
					return null;
			}
		}
	}
	public class BarSplitCheckItemLinkControlAutomationPeer : BarSplitButtonItemLinkControlAutomationPeer, IToggleProvider {
		public BarSplitCheckItemLinkControlAutomationPeer(BarSplitCheckItemLinkControl control)
			: base(control) {
		}
		public override void OnBarItemClick(object sender, ItemClickEventArgs e) {
			base.OnBarItemClick(sender, e);
			RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, ToggleState == ToggleState.Off ? ToggleState.On : ToggleState.Off, ToggleState);
#if DEBUGTEST
			BarsAutomationTesterHelper.PropertyChanges.Add(TogglePatternIdentifiers.ToggleStateProperty);
#endif
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.CheckBox;
		}
		#region IToggleProvider
		public void Toggle() {
			ToggleState oldState = ToggleState;
			if((Owner as BarSplitCheckItemLinkControl).Link == null || (Owner as BarSplitCheckItemLinkControl).Link.Item == null)
				return;
			((Owner as BarSplitCheckItemLinkControl).Link.Item as BarSplitCheckItem).Toggle();
			RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, oldState, ToggleState);
#if DEBUGTEST
			BarsAutomationTesterHelper.PropertyChanges.Add(TogglePatternIdentifiers.ToggleStateProperty);
#endif
		}
		public ToggleState ToggleState {
			get {
				if((Owner as BarSplitCheckItemLinkControl).Link == null || (Owner as BarSplitCheckItemLinkControl).Link.Item == null)
					return ToggleState.Indeterminate;
				return ((Owner as BarSplitCheckItemLinkControl).Link.Item as BarSplitCheckItem).IsChecked == true ? ToggleState.On : ToggleState.Off; }
		}
		#endregion
		public override object GetPattern(PatternInterface patternInterface) {
			var baseValue = base.GetPattern(patternInterface);
			if (baseValue != null)
				return baseValue;
			switch(patternInterface) {
				case PatternInterface.Toggle:
					return this;
				case PatternInterface.ExpandCollapse:
					return this;
				default:
					return null;
			}
		}
	}
	public class BarStaticItemLinkControlAutomationPeer : BarItemLinkControlAutomationPeer {
		public BarStaticItemLinkControlAutomationPeer(BarStaticItemLinkControl control)
			: base(control) {
		}
		public override object GetPattern(PatternInterface patternInterface) {
			var baseValue = base.GetPattern(patternInterface);
			if (baseValue != null)
				return baseValue;
			if(patternInterface == PatternInterface.Invoke)
				return this;
			return null;
		}
	}
	public class BarEditItemLinkControlAutomationPeer : BarItemLinkControlAutomationPeer {
		public BarEditItemLinkControlAutomationPeer(BarEditItemLinkControl control) : base(control) {
		}
	}
	public class PopupMenuAutomationPeer : FrameworkElementAutomationPeer {
		private WeakReference ownerCoreWR = new WeakReference(null);
		delegate void SetFocusDelegate();
		public BarPopupBorderControl OwnerCore {
			get { return (BarPopupBorderControl)ownerCoreWR.Target; }
			set { ownerCoreWR = new WeakReference(value); }
		}
		public PopupMenuAutomationPeer(BarPopupBorderControl ownerCore)
			: base(ownerCore) {
			OwnerCore = ownerCore;
			if (OwnerCore != null && OwnerCore.Popup != null) {
				if (OwnerCore.Popup.IsOpen) {
					Dispatcher.BeginInvoke(new SetFocusDelegate(SetFocus), null);
				}
			}
		}
		protected override void SetFocusCore() {
			var ownercore = OwnerCore;
			if(OwnerCore == null || OwnerCore.Popup == null) {
				base.SetFocusCore();
				return;
			}
			OwnerCore.Popup.Focus();
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Menu;
		}
		protected override string GetClassNameCore() {
			var ownerCore = OwnerCore;
			if (ownerCore == null)
				return "";
			return OwnerCore.Popup.GetType().Name;
		}
	}
	public class NavigationAutomationPeersCreator : ObjectCreator<AutomationPeer> {
		static NavigationAutomationPeersCreator defaultCreator;
		static object olock = new object();
		public static NavigationAutomationPeersCreator Default {
			get {
				lock (olock) {
				if(defaultCreator == null) defaultCreator = new NavigationAutomationPeersCreator();
				}
				return defaultCreator;
			}
		}
		protected override void RegisterObjects() { }
		public bool AllowPeerCreation {
			get { return BarsAutomationHelper.AllowUIAutomationSupport; }
			set { BarsAutomationHelper.AllowUIAutomationSupport = value; }
		}
		public override AutomationPeer Create(Type baseType, object arg) {
			if(!AllowPeerCreation) return null;
			return base.Create(baseType, arg);
		}
		public virtual AutomationPeer Create(object owner) {
			if(owner==null) return null;
			return Create(owner.GetType(), owner);
		}
		public void RegisterObject(Type ownerType, Type peerType, CreateObjectMethod<AutomationPeer> linkControlCreateMethod) {
			RegisterObject(ownerType, new BarItemClassInfo<AutomationPeer>() { ItemType = peerType, CreateMethod = linkControlCreateMethod });
		}
		public bool HasPeer(Type type, bool checkBaseTypes = true) {
			if(Storage.ContainsKey(type)) return true;
			while(type != null) {
				type = type.BaseType;
				if(type != null && Storage.ContainsKey(type)) return true;
			}
			return false;
		}
	}
	public class WeakReferenceDictionary<TKey, TValue> where TValue : class {
		class Record<TRecordKey, TRecordValue> {
			public Record(TRecordKey key, TRecordValue value) {
				Key = key;
				Value = value;
			}
			public TRecordKey Key { get; set; }
			public TRecordValue Value { get; set; }
		}
		List<Record<WeakReference, WeakReference>> internalDict = new List<Record<WeakReference, WeakReference>>();		
		public TValue this[TKey key] {
			get {
				return GetUnwrappedList().FirstOrDefault(x => Equals(x.Key, key)).With(x => x.Value);				
			}
			set {
				Set(key, value, false);
			}
		}
		public bool ContainsKey(TKey key) {
			return GetUnwrappedList().Any(x => Equals(x.Key, key));
		}
		public bool ContainsValue(TValue value) {
			return GetUnwrappedList().Any(x => Equals(x.Value, value));			
		}
		ReadOnlyCollection<Record<TKey, TValue>> GetUnwrappedList() {
			return internalDict
				.Select(x => new {
				Key = x.Key.With(k => k.Target),
				Value = x.Value.With(v => v.Target),
				IsAlive =
						x.Key.Return(k => k.IsAlive, () => false) &&
						x.Value.Return(v => v.IsAlive, () => false)
			})
				.Where(x => x.IsAlive)
				.Select(x => new Record<TKey, TValue>((TKey)x.Key, (TValue)x.Value))
				.ToList().AsReadOnly();
		}
		public void Add(TKey key, TValue value) {
			Set(key, value, true);
		}
		void Set(TKey key, TValue value, bool add) {
			var uw = GetUnwrappedList();
			if (add && ContainsKey(key))
				throw new ArgumentException("key");
			uw = GetUnwrappedList();
			if (!ContainsKey(key))
				internalDict.Add(new Record<WeakReference, WeakReference>(new WeakReference(key), null));
			var pair = internalDict.FirstOrDefault(x => Equals(x.Key.Target, key));
			pair.Value = new WeakReference(value);
		}
	}
}
