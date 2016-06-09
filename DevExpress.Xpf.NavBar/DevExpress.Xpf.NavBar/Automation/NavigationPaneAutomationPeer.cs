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
namespace DevExpress.Xpf.NavBar.Automation {
	public class NavigationPaneViewAutomationPeer : NavBarViewAutomationPeer, IExpandCollapseProvider {
		NavigationPaneView view { get { return Owner as NavigationPaneView; } }
		public NavigationPaneViewAutomationPeer(NavigationPaneView view)
			: base(view) {			
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = new List<AutomationPeer>();
			if(view == null) return children;
			if (view.NavBar.ActiveGroup != null) {
				children.Add(UIElementAutomationPeer.CreatePeerForElement((UIElement)FindObjectInVisualTree(view, "activeGroup")));
			}
			if (view.ItemsControlGroupCount > 0) {
				var panel = (ItemsControl)FindObjectInVisualTree(view, "groupButtonPanel");
				if (panel != null)
					children.Add(new NavBarItemsControlAutomationPeer(panel));
			}
			if(view.IsOverflowPanelVisible){
				var panel = (ItemsControl)FindObjectInVisualTree(view, "overflowPanel");
				if (panel != null)
					children.Add(new NavBarItemsControlAutomationPeer(panel));
			}
			return children;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.ExpandCollapse)
				return this;
			return null;
		}
		#region IExpandCollapseProvider Members
		public void Collapse() {
			if(view == null) return;
			view.IsExpanded = false;			
		}
		public void Expand() {
			if(view == null) return;
			view.IsExpanded = true;			
		}
		public ExpandCollapseState ExpandCollapseState {
			get {
				if(view == null) return System.Windows.Automation.ExpandCollapseState.LeafNode;
				return view.IsExpanded ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
			}
		}
		#endregion
	}
}
