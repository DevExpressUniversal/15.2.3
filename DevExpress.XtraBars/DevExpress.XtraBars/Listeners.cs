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
using System.Collections;
using System.Windows.Forms;
using DevExpress.XtraBars.MessageFilter;
namespace DevExpress.XtraBars {
	public interface IBarManagerListener {
		BarManagerHookResult PreFilterMessage(object owner, int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam);
		BarManagerHookResult InternalPreFilterMessage(object owner, int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam); 
	}
	public delegate BarManagerHookResult BarManagerListenerHandler(object owner, int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam);
	public class BarManagerListenerCollection : CollectionBase {
		public BarManagerListenerCollection() {
		}
		public int Add(IBarManagerListener listener) {
			return List.Add(new WeakReference(listener));
		}
		public void Remove(IBarManagerListener listener) {
			for(int i = 0; i < Count; i++) {
				WeakReference wr = RequestItemByIndex(i);
				IBarManagerListener item = wr.Target as IBarManagerListener;
				if(item != null && item == listener) {
					List.Remove(wr);
					break;
				}
			}
		}
		public IBarManagerListener this[int index] {
			get {
				WeakReference wr = RequestItemByIndex(index);
				return wr.Target as IBarManagerListener;
			}
		}
		public void Check() {
			for(int i = 0; i < Count; i++) {
				WeakReference wr = RequestItemByIndex(i);
				if(wr.Target == null)
					List.RemoveAt(i--);
			}
		}
		protected virtual WeakReference RequestItemByIndex(int index) {
			return (WeakReference)List[index];
		}
	}
}
