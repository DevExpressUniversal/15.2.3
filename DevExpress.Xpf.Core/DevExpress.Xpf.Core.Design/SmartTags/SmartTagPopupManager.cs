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
namespace DevExpress.Xpf.Core.Design {
	public class SmartTagPopupIsPopupOpenChangedEventArgs : EventArgs {
		public SmartTagPopupIsPopupOpenChangedEventArgs(ISmartTagPopup popup) {
			Popup = popup;
		}
		public ISmartTagPopup Popup { get; private set; }
	}
	public interface ISmartTagPopup {
		bool IsPopupOpen { get; set; }
		event EventHandler<SmartTagPopupIsPopupOpenChangedEventArgs> IsPopupOpenChanged;
	}
	public class SmartTagPopup : ISmartTagPopup {
		Func<bool> getIsPopupOpen;
		Action<bool> setIsPopupOpen;
		Action<EventHandler<SmartTagPopupIsPopupOpenChangedEventArgs>> subscribeToIsPopupOpenChanged;
		Action<EventHandler<SmartTagPopupIsPopupOpenChangedEventArgs>> unsubscribeFromIsPopupOpenChanged;
		public SmartTagPopup(Func<bool> getIsPopupOpen, Action<bool> setIsPopupOpen, Action<EventHandler<SmartTagPopupIsPopupOpenChangedEventArgs>> subscribeToIsPopupOpenChanged, Action<EventHandler<SmartTagPopupIsPopupOpenChangedEventArgs>> unsubscribeFromIsPopupOpenChanged) {
			this.getIsPopupOpen = getIsPopupOpen;
			this.setIsPopupOpen = setIsPopupOpen;
			this.subscribeToIsPopupOpenChanged = subscribeToIsPopupOpenChanged;
			this.unsubscribeFromIsPopupOpenChanged = unsubscribeFromIsPopupOpenChanged;
		}
		bool ISmartTagPopup.IsPopupOpen {
			get { return getIsPopupOpen(); }
			set { setIsPopupOpen(value); }
		}
		event EventHandler<SmartTagPopupIsPopupOpenChangedEventArgs> ISmartTagPopup.IsPopupOpenChanged {
			add { subscribeToIsPopupOpenChanged(value); }
			remove { unsubscribeFromIsPopupOpenChanged(value); }
		}
	}
	public static class SmartTagPopupManager {
		static List<WeakReference> popups = new List<WeakReference>();
		public static void RegisterPopup(ISmartTagPopup popup) {
			WeakReference popupReference = new WeakReference(popup);
			if(popups.Contains(popupReference)) return;
			popup.IsPopupOpenChanged += OnPopupIsPopupOpenChanged;
			popups.Add(popupReference);
		}
		public static void UnregisterPopup(ISmartTagPopup popup) {
			WeakReference popupReference = new WeakReference(popup);
			if(!popups.Contains(popupReference)) return;
			popups.Remove(popupReference);
			popup.IsPopupOpenChanged -= OnPopupIsPopupOpenChanged;
		}
		static void OnPopupIsPopupOpenChanged(object sender, SmartTagPopupIsPopupOpenChangedEventArgs e) {
			if(!e.Popup.IsPopupOpen) return;
			foreach(WeakReference popupReference in popups.ToArray()) {
				ISmartTagPopup popup = popupReference.Target as ISmartTagPopup;
				if(popup == null) {
					popups.Remove(popupReference);
					continue;
				}
				if(popup == e.Popup) continue;
				popup.IsPopupOpen = false;
			}
		}
	}
}
