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

using DevExpress.Xpf.WindowsUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
namespace DevExpress.Xpf.WindowsUI.UIAutomation {
	public class PageViewAutomationPeer : WinUIAutomationPeerBase<PageView>, ISelectionProvider {
		public PageViewAutomationPeer(PageView owner) :
			base(owner) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Tab;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Selection)
				return this;
			return base.GetPattern(patternInterface);
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = new List<AutomationPeer>();
			for(int i = 0; i < Owner.Items.Count; i++) {
				object obj = Owner.ItemContainerGenerator.ContainerFromIndex(i);
				if(obj as FrameworkElement != null) {
					AutomationPeer peer = CreatePeerForElement(obj as FrameworkElement);
					if(peer != null)
						children.Add(peer);
				}
			}
			if(Owner.BackCommand != null) {
				if(Owner.PartNavigationHeader != null)
					children.Add(CreatePeerForElement(Owner.PartNavigationHeader.PartBackButton));
			}
			return children;
		}
		#region ISelectionProvider Members
		public bool CanSelectMultiple {
			get { return false; }
		}
		public IRawElementProviderSimple[] GetSelection() {
			if(Owner.SelectedItem == null) return null;
			UIElement selectedItem = Owner.ItemContainerGenerator.ContainerFromItem(Owner.SelectedItem) as UIElement;
			if(selectedItem == null) return null;
			AutomationPeer selectedTabItemPeer = CreatePeerForElement(selectedItem);
			if(selectedTabItemPeer == null) return null;
			IRawElementProviderSimple provider = ProviderFromPeer(selectedTabItemPeer);
			if(provider == null) return null;
			return new IRawElementProviderSimple[] { provider };
		}
		public bool IsSelectionRequired {
			get { return Owner.Items.Count != 0;  }
		}
		#endregion
	}
	public class PageViewItemAutomationPeer : HeaderedControlAutomationPeerBase<PageViewItem>, ISelectionItemProvider {
		public PageViewItemAutomationPeer(PageViewItem owner)
			: base(owner) {
		}
		protected override object Header {
			get { return Owner.Header; }
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.TabItem;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.SelectionItem)
				return this;
			return base.GetPattern(patternInterface);
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> headerChildren = base.GetChildrenCore();
			if(Owner != null && Owner.IsSelected) {
				PageView parentTabControl = Owner.Owner as PageView;
				if(parentTabControl != null) {
					var contentHost = parentTabControl.PartPageViewContentPanel;
					if(contentHost != null) {
						AutomationPeer contentHostPeer = new FrameworkElementAutomationPeer(contentHost);
						List<AutomationPeer> contentChildren = contentHostPeer.GetChildren();
						if(contentChildren != null) {
							if(headerChildren == null)
								headerChildren = contentChildren;
							else
								headerChildren.AddRange(contentChildren);
						}
					}
				}
			}
			return headerChildren;
		}
		#region ISelectionItemProvider Members
		public void AddToSelection() {
			Select();
		}
		public bool IsSelected {
			get { return Owner.IsSelected; }
		}
		public void RemoveFromSelection() {
		}
		public void Select() {
			Owner.Owner.Select(Owner);
		}
		public IRawElementProviderSimple SelectionContainer {
			get { return ProviderFromPeer(CreatePeerForElement((UIElement)Owner.Owner)); }
		}
		#endregion
	}
}
