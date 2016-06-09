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
using System.Windows.Automation.Peers;
using DevExpress.Xpf.Bars;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;
namespace DevExpress.Xpf.Ribbon.Automation {
	public class BackstageViewControlAutomationPeer : DevExpress.Xpf.Bars.Automation.BaseNavigationAutomationPeer, ISelectionProvider {
		public BackstageViewControlAutomationPeer(BackstageViewControl control) : base(control) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Pane;			
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = new List<AutomationPeer>();
			BackstageViewControl control = Owner as BackstageViewControl;
			if(control == null) return new List<AutomationPeer>();
			if (control.Host != null && control.Host.GlyphElement != null) {
				var peer = new RibbonCheckedBorderControlAutomationPeer(control.Host.GlyphElement);
				peer.parent = this;
				children.Add(peer);
			}
			for (int i = 0; i < control.Items.Count; i++) {
				UIElement element = control.ItemContainerGenerator.ContainerFromIndex(i) as UIElement;
				if (element == null) continue;
				AutomationPeer peer = CreatePeerForElement(element);
				if (peer != null)
					children.Add(peer);
				else
					DevExpress.Xpf.Bars.Automation.BarsAutomationHelper.GetChildrenRecursive(children, element);			
			}
			DevExpress.Xpf.Bars.Automation.BarsAutomationHelper.GetChildrenRecursive(children, control.ActualControlPane as DependencyObject);			
			return children;			
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			return "Backstage View";
		}		
		#region ISelectionProvider
		public bool CanSelectMultiple {
			get { return false; }
		}
		public IRawElementProviderSimple[] GetSelection() {
			return new IRawElementProviderSimple[] { };
		}
		public bool IsSelectionRequired {
			get { return true; }
		}		
		#endregion
		public override object GetPattern(PatternInterface patternInterface) {
			return patternInterface == PatternInterface.Selection ? this : null;
		}
	}
	public class BackstageItemControlAutomationPeer : DevExpress.Xpf.Bars.Automation.BaseNavigationAutomationPeer {
		public BackstageItemControlAutomationPeer(BackstageItem item) : base(item) {
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			return new List<AutomationPeer>();
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			BackstageItem item = Owner as BackstageItem;
			if(item == null)
				return string.Empty;
			return Convert.ToString(item.Content);
		}
	}
	public class BackstageButtonItemControlAutomationPeer : BackstageItemControlAutomationPeer, IInvokeProvider {
		public BackstageButtonItemControlAutomationPeer(BackstageButtonItem item) :base(item) {
			item.Click += new EventHandler(OnClick);
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Button;
		}
		#region IInvokeProvider
		public void Invoke() {
			(Owner as BackstageButtonItem).OnClick();
		}
		void OnClick(object sender, EventArgs e) {
			RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
			RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
		}
		#endregion
		public override object GetPattern(PatternInterface patternInterface) {
			return patternInterface == PatternInterface.Invoke ? this : null;
		}
	}
	public class BackstageTabItemControlAutomationPeer : BackstageItemControlAutomationPeer, ISelectionItemProvider {
		public BackstageTabItemControlAutomationPeer(BackstageTabItem item) : base(item) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.TabItem;
		}
		#region ISelectionItemProvider
		public void AddToSelection() {
			Select();
		}
		public bool IsSelected {
			get { return (Owner as BackstageTabItem).IsSelected; }
		}
		public void RemoveFromSelection() {			
		}
		public void Select() {
			(Owner as BackstageTabItem).Backstage.SelectedTab = Owner as BackstageTabItem;
			RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
		}
		public IRawElementProviderSimple SelectionContainer {
			get { return null; }
		}
		#endregion
		public override object GetPattern(PatternInterface patternInterface) {
			return patternInterface == PatternInterface.SelectionItem ? this : null;
		}
	}
	public class BackstageSeparatorItemControlAutomationPeer : DevExpress.Xpf.Bars.Automation.BaseNavigationAutomationPeer {
		public BackstageSeparatorItemControlAutomationPeer(BackstageSeparatorItem item) : base(item) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Separator;
		}
	}	
}
