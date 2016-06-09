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
using System.Windows;
using System.ComponentModel;
using DevExpress.Data.Browsing;
using System.Collections.Specialized;
namespace DevExpress.Xpf.Core.Native {
	public class ListChangedEventManager : WeakEventManager {
		public static void AddListener(IBindingList source, IWeakEventListener listener) {
			CurrentManager.ProtectedAddListener(source, listener);
		}
		public static void RemoveListener(IBindingList source, IWeakEventListener listener) {
			CurrentManager.ProtectedRemoveListener(source, listener);
		}
		static ListChangedEventManager CurrentManager {
			get {
				Type managerType = typeof(ListChangedEventManager);
				ListChangedEventManager currentManager = (ListChangedEventManager)WeakEventManager.GetCurrentManager(managerType);
				if(currentManager == null) {
					currentManager = new ListChangedEventManager();
					WeakEventManager.SetCurrentManager(managerType, currentManager);
				}
				return currentManager;
			}
		}
		ListChangedEventManager() { }
		void OnCollectionChanged(object sender, ListChangedEventArgs args) {
			base.DeliverEvent(sender, args);
		}
		protected override void StartListening(object source) {
			IBindingList changed = (IBindingList)source;
			changed.ListChanged += new ListChangedEventHandler(this.OnCollectionChanged);
		}
		protected override void StopListening(object source) {
			IBindingList changed = (IBindingList)source;
			changed.ListChanged -= new ListChangedEventHandler(this.OnCollectionChanged);
		}
	}
}
