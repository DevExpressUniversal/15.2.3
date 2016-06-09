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
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Automation;
using DevExpress.Xpf.WindowsUI.Base;
namespace DevExpress.Xpf.WindowsUI.UIAutomation {
	public class FlipViewAutomationPeer : WinUIAutomationPeerBase<FlipView>, ISelectionProvider, IScrollProvider {
		public FlipViewAutomationPeer(FlipView owner)
			: base(owner) {
			Owner.SelectionChanged += (o, e) => RaiseAutomationEvent(AutomationEvents.StructureChanged);
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Tab;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Selection || patternInterface == PatternInterface.Scroll)
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
			get { return Owner.Items.Count != 0; }
		}
		#endregion
		#region IScrollProvider Members
		public double HorizontalScrollPercent {
			get { return 100d * Owner.SelectedIndex / (Owner.Items.Count - 1); }
		}
		public double HorizontalViewSize {
			get { return 100d / Owner.Items.Count; }
		}
		public bool HorizontallyScrollable {
			get { return Owner.HasItems; }
		}
		public void Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount) {
			if(verticalAmount != ScrollAmount.NoAmount)
				throw new InvalidOperationException("Vertical Scrolling is not supported");
			if(horizontalAmount == ScrollAmount.NoAmount)
				return;
			if(horizontalAmount == ScrollAmount.LargeIncrement || horizontalAmount == ScrollAmount.SmallIncrement) {
				if(Owner.SelectedIndex != Owner.Items.Count - 1) {
					Owner.Select((ISelectorItem)Owner.Items[Owner.SelectedIndex + 1]);
				}
			}
			else {
				if(Owner.SelectedIndex != 0) {
					Owner.Select((ISelectorItem)Owner.Items[Owner.SelectedIndex - 1]);
				}
			}
		}
		public void SetScrollPercent(double horizontalPercent, double verticalPercent) {
			if(verticalPercent != -1)
				throw new InvalidOperationException("Vertical Scrolling is not supported");
			if(horizontalPercent < 0 || horizontalPercent > 100)
				throw new ArgumentOutOfRangeException("Horizontal percent may not be " + horizontalPercent);
			Owner.Select((ISelectorItem)Owner.Items[(int)Math.Floor(horizontalPercent / Owner.Items.Count)]);
		}
		public double VerticalScrollPercent {
			get { return -1; }
		}
		public double VerticalViewSize {
			get { return 100; }
		}
		public bool VerticallyScrollable {
			get { return false; }
		}
		#endregion
	}
	public class FlipViewItemAutomationPeer : WinUIAutomationPeerBase<FlipViewItem>, ISelectionItemProvider {
		public FlipViewItemAutomationPeer(FlipViewItem owner)
			: base(owner) {
		}
		protected override string GetNameImpl() {
			FlipView flipView = Owner.Owner as FlipView;
			if(flipView == null) return base.GetNameImpl();
			return string.Format("{0}, {1}, {2}", Owner.Content.ToString(),
			   flipView.ItemContainerGenerator.IndexFromContainer(Owner) + 1,
			   flipView.Items.Count);
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.TabItem;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.SelectionItem)
				return this;
			return base.GetPattern(patternInterface);
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
			get {
				if(Owner.Owner == null) return null;
				return ProviderFromPeer(CreatePeerForElement((UIElement)Owner.Owner));
			}
		}
		#endregion
	}
}
