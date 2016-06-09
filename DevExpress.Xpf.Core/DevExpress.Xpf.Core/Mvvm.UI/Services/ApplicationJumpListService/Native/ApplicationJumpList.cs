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

#if !FREE
using DevExpress.Xpf.Utils;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.UI.Interactivity.Internal;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Utils;
using DevExpress.Internal;
using System.Windows.Markup;
namespace DevExpress.Mvvm.UI.Native {
	public class ApplicationJumpList : IApplicationJumpList {
		IApplicationJumpListImplementation implementation;
		public ApplicationJumpList(IApplicationJumpListImplementation implementation) {
			if(implementation == null) throw new ArgumentNullException("implementation");
			this.implementation = implementation;
		}
		ApplicationJumpTaskInfo IApplicationJumpList.Find(string commandId) {
			return implementation.Find(commandId);
		}
		bool IApplicationJumpList.AddOrReplace(ApplicationJumpTaskInfo jumpTask) {
			return implementation.AddOrReplace(jumpTask);
		}
		void ICollection<ApplicationJumpItemInfo>.Add(ApplicationJumpItemInfo item) { implementation.AddItem(item); }
		void IList<ApplicationJumpItemInfo>.Insert(int index, ApplicationJumpItemInfo item) { implementation.InsertItem(index, item); }
		ApplicationJumpItemInfo IList<ApplicationJumpItemInfo>.this[int index] {
			get { return implementation.GetItem(index); }
			set { implementation.SetItem(index, value); }
		}
		bool ICollection<ApplicationJumpItemInfo>.Remove(ApplicationJumpItemInfo item) { return implementation.RemoveItem(item); }
		void IList<ApplicationJumpItemInfo>.RemoveAt(int index) { implementation.RemoveItemAt(index); }
		void ICollection<ApplicationJumpItemInfo>.Clear() { implementation.ClearItems(); }
		int IList<ApplicationJumpItemInfo>.IndexOf(ApplicationJumpItemInfo item) { return implementation.IndexOfItem(item); }
		bool ICollection<ApplicationJumpItemInfo>.Contains(ApplicationJumpItemInfo item) { return implementation.ContainsItem(item); }
		void ICollection<ApplicationJumpItemInfo>.CopyTo(ApplicationJumpItemInfo[] array, int arrayIndex) {
			foreach(ApplicationJumpItemInfo item in implementation.GetItems()) {
				array[arrayIndex] = item;
				++arrayIndex;
			}
		}
		int ICollection<ApplicationJumpItemInfo>.Count { get { return implementation.ItemsCount(); } }
		bool ICollection<ApplicationJumpItemInfo>.IsReadOnly { get { return false; } }
		IEnumerator<ApplicationJumpItemInfo> IEnumerable<ApplicationJumpItemInfo>.GetEnumerator() { return implementation.GetItems().GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return implementation.GetItems().GetEnumerator(); }
	}
}
