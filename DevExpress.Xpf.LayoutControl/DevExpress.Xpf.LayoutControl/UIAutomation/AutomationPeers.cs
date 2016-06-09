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

using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
namespace DevExpress.Xpf.LayoutControl.UIAutomation {
	public abstract class BaseLayoutAutomationPeer : DevExpress.Xpf.Bars.Automation.BaseNavigationAutomationPeer {
		protected delegate bool IteratorCallback(AutomationPeer peer);
		protected delegate bool FilterCallback(object obj);
		protected static bool Iterate(DependencyObject parent, IteratorCallback callback, FilterCallback filterCallback) {
			bool done = false;
			if(parent != null) {
				AutomationPeer peer = null;
				int count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
				for(int i = 0; i < count && !done; i++) {
					DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
					if(filterCallback(child) 
						&& (peer = CreatePeerForElement((UIElement)child)) != null) {
						done = callback(peer);
					}
					else if(child != null
						&& child is UIElement3D
						&& (peer = UIElement3DAutomationPeer.CreatePeerForElement(((UIElement3D)child))) != null) {
						done = callback(peer);
					}
					else {
						done = Iterate(child, callback, filterCallback);
					}
				}
			}
			return done;
		}
		protected BaseLayoutAutomationPeer(FrameworkElement owner)
			: base(owner) {
		}
		protected sealed override string GetNameCore() {
			string name = GetNameImpl();
			return string.IsNullOrEmpty(name) ? Owner.GetType().Name : name;
		}
		protected virtual string GetNameImpl() {
			string result = base.GetNameCore();
			if(string.IsNullOrEmpty(result)) {
				bool useAttachedValue;
				object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
				if(useAttachedValue) return (string)value;
			}
			return result;
		}
		protected sealed override string GetAutomationIdCore() {
			string id = GetAutomationIdImpl();
			return string.IsNullOrEmpty(id) ? Owner.GetType().Name : id;
		}
		protected virtual string GetAutomationIdImpl() {
			string result = base.GetAutomationIdCore();
			if(string.IsNullOrEmpty(result)) {
				bool useAttachedValue;
				object value = TryGetAutomationPropertyValue(AutomationProperties.AutomationIdProperty, out useAttachedValue);
				if(useAttachedValue) return (string)value;
			}
			return result;
		}
	}
	public abstract class BaseLayoutAutomationPeer<T> : BaseLayoutAutomationPeer where T : FrameworkElement {
		protected BaseLayoutAutomationPeer(T owner)
			: base(owner) {
		}
		public new T Owner { get { return (T)base.Owner; } }
	}
	public abstract class LayoutControlBaseAutomationPeer<T> : BaseLayoutAutomationPeer<T> where T : LayoutControlBase {
		protected LayoutControlBaseAutomationPeer(T owner)
			: base(owner) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Group;
		}
		protected override string GetClassNameCore() {
			return Owner.GetType().Name;
		}
		protected virtual bool IncludeInternalElements { get { return false; } }
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = new List<AutomationPeer>();
			IteratorCallback iteratorCallback = (IteratorCallback)delegate (AutomationPeer peer) {
				children.Add(peer);
				return (false);
			};
			FilterCallback filterCallback = (FilterCallback)delegate (object obj) {
				return obj != null && obj is UIElement;
			};
			Iterate(Owner, iteratorCallback, filterCallback);
			return children;
		}
	}
	public abstract class ScrollablePanelAutomationPeer<T> : LayoutControlBaseAutomationPeer<T>, IScrollProvider
		where T : LayoutControlBase {
		LayoutControllerBase Controller { get { return Owner.Controller; } }
		protected ScrollablePanelAutomationPeer(T owner)
			: base(owner) {
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Scroll)
				return this;
			return base.GetPattern(patternInterface);
		}
		#region IScrollProvider Members
		double IScrollProvider.HorizontalScrollPercent {
			get { return Controller.HorzScrollParams.RelativePosition * 100; }
		}
		double IScrollProvider.HorizontalViewSize {
			get { return Controller.HorzScrollParams.RelativePageSize * 100; }
		}
		bool IScrollProvider.HorizontallyScrollable {
			get { return Controller.HorzScrollParams.Enabled; }
		}
		IScrollProvider IScrollProvider { get { return (IScrollProvider)this; } }
		void IScrollProvider.Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount) {
			if(!IsEnabled()) throw new ElementNotEnabledException();
			bool scrollHorizontally = (horizontalAmount != ScrollAmount.NoAmount);
			bool scrollVertically = (verticalAmount != ScrollAmount.NoAmount);
			if(scrollHorizontally && !IScrollProvider.HorizontallyScrollable || scrollVertically && !IScrollProvider.VerticallyScrollable) {
				throw new InvalidOperationException("OperationCannotBePerformed");
			}
			double horzOffset = Controller.HorzScrollParams.PageSize;
			double vertOffset = Controller.VertScrollParams.PageSize;
			switch(horizontalAmount) {
				case ScrollAmount.LargeDecrement: horzOffset = -horzOffset; break;
				case ScrollAmount.LargeIncrement: break;
				case ScrollAmount.SmallDecrement: horzOffset = -16; ; break;
				case ScrollAmount.SmallIncrement: horzOffset = -16; ; break;
				case ScrollAmount.NoAmount: horzOffset = 0; break;
				default:
					throw new InvalidOperationException("OperationCannotBePerformed");
			}
			switch(verticalAmount) {
				case ScrollAmount.LargeDecrement: vertOffset = -vertOffset; break;
				case ScrollAmount.LargeIncrement: break;
				case ScrollAmount.SmallDecrement: vertOffset = -16; ; break;
				case ScrollAmount.SmallIncrement: vertOffset = -16; ; break;
				case ScrollAmount.NoAmount: vertOffset = 0; break;
				default:
					throw new InvalidOperationException("OperationCannotBePerformed");
			}
			if(scrollHorizontally || scrollVertically)
				Owner.SetOffset(new Point(Owner.Offset.X + horzOffset, Owner.Offset.Y + vertOffset));
		}
		void IScrollProvider.SetScrollPercent(double horizontalPercent, double verticalPercent) {
			ScrollParams horz = Controller.HorzScrollParams;
			ScrollParams vert = Controller.VertScrollParams;
			Controller.Scroll(Orientation.Horizontal, (horz.Max - horz.PageSize) * horizontalPercent * 0.01);
			Controller.Scroll(Orientation.Vertical, (vert.Max - vert.PageSize) * verticalPercent * 0.01);
		}
		double IScrollProvider.VerticalScrollPercent {
			get { return Controller.VertScrollParams.RelativePosition * 100; }
		}
		double IScrollProvider.VerticalViewSize {
			get { return Controller.VertScrollParams.RelativePageSize * 100; }
		}
		bool IScrollProvider.VerticallyScrollable {
			get { return Controller.VertScrollParams.Enabled; }
		}
		#endregion
	}
	public class LayoutGroupAutomationPeer : LayoutControlBaseAutomationPeer<LayoutGroup>, IExpandCollapseProvider, ISelectionProvider {
		public LayoutGroupAutomationPeer(LayoutGroup owner)
			: base(owner) {
		}
		bool IsCollapsible { get { return Owner.View == LayoutGroupView.GroupBox && Owner.IsCollapsible; } }
		protected override bool IncludeInternalElements {
			get { return base.IncludeInternalElements || Owner.View == LayoutGroupView.Tabs; }
		}
		protected override string GetNameImpl() {
			string result = base.GetNameImpl();
			if(string.IsNullOrEmpty(result)) {
				return Owner.Header != null ? Owner.Header.ToString() : string.Empty;
			}
			return result;
		}
		protected override string GetAutomationIdImpl() {
			string result = base.GetAutomationIdImpl();
			if(string.IsNullOrEmpty(result)) {
				return Owner.Header != null ? Owner.Header.ToString() : string.Empty;
			}
			return result;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.ExpandCollapse && IsCollapsible)
				return this;
			if(patternInterface == PatternInterface.Selection && Owner.View == LayoutGroupView.Tabs) {
				return this;
			}
			return base.GetPattern(patternInterface);
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> result = base.GetChildrenCore();
			if(Owner.View == LayoutGroupView.GroupBox) {
				var groupBox = Owner.GetChildren(true, false, true).Where(x => x is GroupBox).FirstOrDefault();
				if(groupBox != null) {
					var button = DevExpress.Xpf.Core.Native.LayoutHelper.FindElementByName(groupBox, "MinimizeElement") as GroupBoxButton;
					if(button != null) {
						AutomationPeer peer = CreatePeerForElement(button);
						if(peer != null) result.Add(peer);
					}
				}
			}
			return result;
		}
		#region IExpandCollapseProvider Members
		void IExpandCollapseProvider.Collapse() {
			if(Owner.IsCollapsible) Owner.IsCollapsed = true;
		}
		void IExpandCollapseProvider.Expand() {
			if(Owner.IsCollapsible) Owner.IsCollapsed = false;
		}
		ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState {
			get { return Owner.IsCollapsible && Owner.IsActuallyCollapsed ? ExpandCollapseState.Collapsed : ExpandCollapseState.Expanded; }
		}
		#endregion
		#region ISelectionProvider Members
		bool ISelectionProvider.CanSelectMultiple {
			get { return false; }
		}
		IRawElementProviderSimple[] ISelectionProvider.GetSelection() {
			AutomationPeer peerForSelectedItem = CreatePeerForElement(Owner.SelectedTabChild);
			return new System.Windows.Automation.Provider.IRawElementProviderSimple[] { ProviderFromPeer(peerForSelectedItem) };
		}
		bool ISelectionProvider.IsSelectionRequired {
			get { return true; }
		}
		#endregion
	}
	public class LayoutItemAutomationPeer : BaseLayoutAutomationPeer<LayoutItem> {
		public LayoutItemAutomationPeer(LayoutItem owner)
			: base(owner) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.ListItem;
		}
		protected override string GetClassNameCore() {
			return Owner.GetType().Name;
		}
		protected override string GetNameImpl() {
			string result = base.GetNameImpl();
			if(string.IsNullOrEmpty(result)) {
				return Owner.Label != null ? Owner.Label.ToString() : string.Empty;
			}
			return result;
		}
		protected override string GetAutomationIdImpl() {
			string result = base.GetAutomationIdImpl();
			if(string.IsNullOrEmpty(result)) {
				return Owner.Label != null ? Owner.Label.ToString() : string.Empty;
			}
			return result;
		}
	}
	public class DockLayoutControlAutomationPeer : LayoutControlBaseAutomationPeer<DockLayoutControl> {
		public DockLayoutControlAutomationPeer(DockLayoutControl owner)
			: base(owner) {
		}
	}
	public class LayoutControlAutomationPeer : ScrollablePanelAutomationPeer<LayoutControl> {
		public LayoutControlAutomationPeer(LayoutControl owner)
			: base(owner) {
		}
	}
	public class FlowLayoutControlAutomationPeer : ScrollablePanelAutomationPeer<FlowLayoutControl> {
		public FlowLayoutControlAutomationPeer(FlowLayoutControl owner)
			: base(owner) {
		}
	}
	public class TileLayoutControlAutomationPeer : FlowLayoutControlAutomationPeer {
		public TileLayoutControlAutomationPeer(TileLayoutControl owner)
			: base(owner) {
		}
	}
	public class GroupBoxButtonAutomationPeer : BaseLayoutAutomationPeer<GroupBoxButton>, IInvokeProvider {
		public GroupBoxButtonAutomationPeer(GroupBoxButton element)
			: base(element) {
		}
		protected override string GetClassNameCore() {
			return Owner.GetType().Name;
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Button;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Invoke)
				return this;
			return base.GetPattern(patternInterface);
		}
		void IInvokeProvider.Invoke() {
			if(!base.IsEnabled()) {
				throw new ElementNotEnabledException();
			}
			Owner.Dispatcher.BeginInvoke(new Action(() => {
				Owner.Controller.InvokeClick();
			}), System.Windows.Threading.DispatcherPriority.Input);
		}
	}
	public class TileAutomationPeer : BaseLayoutAutomationPeer<Tile>, IInvokeProvider {
		public TileAutomationPeer(Tile element)
			: base(element) {
		}
		protected override string GetClassNameCore() {
			return Owner.GetType().Name;
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.ListItem;
		}
		protected override string GetNameImpl() {
			string result = base.GetNameImpl();
			if(string.IsNullOrEmpty(result)) {
				return Owner.Header != null ? Owner.Header.ToString() : string.Empty;
			}
			return result;
		}
		protected override string GetAutomationIdImpl() {
			string result = base.GetAutomationIdImpl();
			if(string.IsNullOrEmpty(result)) {
				return Owner.Header != null ? Owner.Header.ToString() : string.Empty;
			}
			return result;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Invoke)
				return this;
			return base.GetPattern(patternInterface);
		}
		void IInvokeProvider.Invoke() {
			if(!base.IsEnabled()) {
				throw new ElementNotEnabledException();
			}
			Owner.Dispatcher.BeginInvoke(new Action(() => {
				Owner.Controller.InvokeClick();
			}), System.Windows.Threading.DispatcherPriority.Input);
		}
	}
	public class GroupBoxAutomationPeer : BaseLayoutAutomationPeer<GroupBox>, IExpandCollapseProvider {
		public GroupBoxAutomationPeer(GroupBox owner)
			: base(owner) {
		}
		bool IsCollapsible { get { return true; } }
		protected override string GetNameImpl() {
			string result = base.GetNameImpl();
			if(string.IsNullOrEmpty(result)) {
				return Owner.Header != null ? Owner.Header.ToString() : string.Empty;
			}
			return result;
		}
		protected override string GetAutomationIdImpl() {
			string result = base.GetAutomationIdImpl();
			if(string.IsNullOrEmpty(result)) {
				return Owner.Header != null ? Owner.Header.ToString() : string.Empty;
			}
			return result;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.ExpandCollapse && IsCollapsible)
				return this;
			return base.GetPattern(patternInterface);
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = null;
			IteratorCallback iteratorCallback = (IteratorCallback)delegate (AutomationPeer peer) {
				if(children == null)
					children = new List<AutomationPeer>();
				children.Add(peer);
				return (false);
			};
			FilterCallback filterCallback = (FilterCallback)delegate (object obj) {
				return obj != null && obj is UIElement && !(obj is LayoutGroup);
			};
			Iterate(Owner, iteratorCallback, filterCallback);
			return children;
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Group;
		}
		#region IExpandCollapseProvider Members
		void IExpandCollapseProvider.Collapse() {
			Owner.State = GroupBoxState.Minimized;
		}
		void IExpandCollapseProvider.Expand() {
			Owner.State = GroupBoxState.Normal;
		}
		ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState {
			get { return Owner.State == GroupBoxState.Minimized ? ExpandCollapseState.Collapsed : ExpandCollapseState.Expanded; }
		}
		#endregion
	}
}
