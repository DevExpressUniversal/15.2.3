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

using System.Windows.Automation.Provider;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.NavBar.Automation {
	public class NavBarGroupControlAutomationPeer : NavBarImageTextAutomationPeerBase, IInvokeProvider, IExpandCollapseProvider, IRawElementProviderSimple {
		internal NavBarGroupControl groupControl { get { return Owner as NavBarGroupControl; } }
		internal NavBarGroup group { get { return groupControl != null ? groupControl.DataContext as NavBarGroup : null; } }
		public NavBarGroupControlAutomationPeer(NavBarGroupControl groupControl)
			: base(groupControl) {			
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			if(groupControl == null || group == null) return new List<AutomationPeer>();
			List<AutomationPeer> children = CreateChildren();			
			bool isViewExplorerBar = group.NavBar.View.NavBarViewKind == NavBarViewKind.ExplorerBar;
			Button button = (Button)FindObjectInVisualTree(groupControl, isViewExplorerBar ? "explorerBarExpandButton" : "navPaneExpandButton");
			if(button != null)
				children.Add(new NavBarButtonAutomationPeer(button));
			if(group.DisplaySource == DisplaySource.Items) {
				ItemsControl itemsControl = (ItemsControl)FindObjectInVisualTree(groupControl, "groupItemsPresenter");
				if(itemsControl != null)
					children.Add(new NavBarGroupItemsControlAutomationPeer(itemsControl));
			}
			else {
				DXContentPresenter contentPresenter = (DXContentPresenter)FindObjectInVisualTree(groupControl, "groupContainerPresenter");
				if (contentPresenter != null && contentPresenter.Content as FrameworkElement != null)
					children.Add(new NavBarGroupContentAutomationPeer((FrameworkElement)contentPresenter.Content));
			}
			return children;
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			return group != null && group.Header != null ? group.Header.ToString() : string.Empty;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Invoke && group!=null && group.NavBar!=null && group.NavBar.View!=null && group.NavBar.View.NavBarViewKind != NavBarViewKind.ExplorerBar)
				return this;
			if (patternInterface == PatternInterface.ExpandCollapse && group!=null && group.NavBar!=null && group.NavBar.View!=null && group.NavBar.View.NavBarViewKind == NavBarViewKind.ExplorerBar)
				return this;
			return null;
		}
		#region IInvokeProvider Members
		void IInvokeProvider.Invoke() {
			if (group != null && group.NavBar != null)
				group.NavBar.SelectionStrategy.SelectGroup(group);
		}
		#endregion
		#region IExpandCollapseProvider Members
		void IExpandCollapseProvider.Collapse() {
			if(group != null)
				group.IsExpanded = false;
		}
		void IExpandCollapseProvider.Expand() {
			if(group != null)
				group.IsExpanded = true;
		}
		ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState {
			get {
				if(group == null) return ExpandCollapseState.LeafNode;
				return group.IsExpanded ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
			}
		}
		#endregion
		#region IRawElementProviderSimple Members
		public object GetPatternProvider(int patternId) {
			return null;
		}
		public object GetPropertyValue(int propertyId) {
			if(propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return "";
			else if(propertyId == AutomationElementIdentifiers.ClassNameProperty.Id)
				return "NavBarGroup";
			return null;
		}
		public IRawElementProviderSimple HostRawElementProvider {
			get { return null; }
		}
		public ProviderOptions ProviderOptions {
			get { return ProviderOptions.ClientSideProvider; }
		}
		#endregion
		protected override System.Func<DependencyObject>[] GetAttachedAutomationPropertySource() {
			return new System.Func<DependencyObject>[]{
				()=>group,
				()=>groupControl
			};
		}
	}
	class NavBarButtonAutomationPeer : ButtonAutomationPeer {
		public NavBarButtonAutomationPeer(Button button)
			: base(button) {
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			return null;
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = DevExpress.Xpf.Bars.Automation.BarsAutomationHelper.TryGetAutomationPropertyValue
				(AutomationProperties.NameProperty,
				out useAttachedValue,
				()=>Owner);
			if(useAttachedValue)
				return (string)value;
			return ((FrameworkElement)Owner).Name;
		}
		protected override string GetAutomationIdCore() {
			bool useAttachedValue;
			object value = DevExpress.Xpf.Bars.Automation.BarsAutomationHelper.TryGetAutomationPropertyValue
				(AutomationProperties.AutomationIdProperty,
				out useAttachedValue,
				() => Owner);
			if(useAttachedValue)
				return (string)value;
			return base.GetAutomationIdCore();
		}
	}
}
