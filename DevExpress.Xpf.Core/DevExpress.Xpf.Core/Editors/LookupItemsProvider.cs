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
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using DevExpress.Utils;
#if !SL
using DevExpress.Data.Access;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using System.Windows;
using DevExpress.Xpf.Core.Native;
#else
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.WPFCompatibility;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using SelectionChangedEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using DevExpress.Data.Browsing;
using DevExpress.Data.Access;
#endif
namespace DevExpress.Xpf.Editors.Helpers {
	public class LookupItemsProvider : IWeakEventListener {
		IItemsProviderOwner owner;
		IList itemSource;
		public LookupItemsProvider(IItemsProviderOwner owner) {
			this.owner = owner;
			this.itemSource = Owner.Items;
			Reset();
		}
		protected IItemsProviderOwner Owner { get { return owner; } }
		public void Reset() {
		}
		public virtual object GetDisplayValueFromItem(object item) {
			return null;
		}
		public virtual object GetDisplayValue(object value) {
			return null;
		}
		public object GetItem(object value) {
			return null;
		}
		object GetDisplayValueCore(object value) {
			return null;
		}
		protected internal virtual object GetValueFromItem(object itemValue) {
			return null;
		}
		public int Count { get { return itemSource.Count; } }
		public object this[int index] { get { return null; } }
		public int IndexOfValue(object editValue) {
			return -1;
		}
		public object FindItem(string description, bool toLower) {
			return null;
		}
		public int IndexOf(object item) {
			return -1;
		}
		void OnListChanged() {
		}
		public static bool AreEqual(IList list1, IList list2) {
			if(list1 == null && list2 == null)
				return true;
			if(list1 == null && list2 != null)
				return list2.Count == 0;
			if(list2 == null && list1 != null)
				return list1.Count == 0;
			if(list1.Count != list2.Count)
				return false;
			for(int i = 0; i < list1.Count; i++) {
				if(!list2.Contains(list1[i]))
					return false;
			}
			return true;
		}
		public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if(managerType == typeof(ListChangedEventManager)) {
				OnListChanged();
				return true;
			}
			return false;
		}
	}
}
