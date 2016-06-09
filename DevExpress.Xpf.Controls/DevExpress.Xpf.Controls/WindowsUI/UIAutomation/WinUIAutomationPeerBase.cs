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
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
namespace DevExpress.Xpf.WindowsUI.UIAutomation {
	public abstract class WinUIAutomationPeerBase : DevExpress.Xpf.Bars.Automation.BaseNavigationAutomationPeer {
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
		protected WinUIAutomationPeerBase(FrameworkElement owner)
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
		protected override string GetClassNameCore() {
			return Owner.GetType().Name;
		}
	}
	public abstract class WinUIAutomationPeerBase<T> : WinUIAutomationPeerBase where T : FrameworkElement {
		protected WinUIAutomationPeerBase(T owner)
			: base(owner) {
		}
		public new T Owner { get { return (T)base.Owner; } }
	}
	public abstract class HeaderedControlAutomationPeerBase<T> : WinUIAutomationPeerBase<T> where T : FrameworkElement {
		protected abstract object Header { get; }
		protected HeaderedControlAutomationPeerBase(T owner)
			: base(owner) {
		}
		protected override string GetAutomationIdImpl() {
			string result = base.GetAutomationIdImpl();
			if(string.IsNullOrEmpty(result)) {
				return Header != null ? Header.ToString() : string.Empty;
			}
			return result;
		}
		protected override string GetNameImpl() {
			string result = base.GetNameImpl();
			if(string.IsNullOrEmpty(result)) {
				return Header != null ? Header.ToString() : string.Empty;
			}
			return result;
		}
	}
}
