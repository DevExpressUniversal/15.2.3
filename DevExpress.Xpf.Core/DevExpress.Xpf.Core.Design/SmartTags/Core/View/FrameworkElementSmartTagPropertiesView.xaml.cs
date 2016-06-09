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

using DevExpress.Design.SmartTags;
using Microsoft.Windows.Design.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace DevExpress.Design.SmartTags {
	public partial class FrameworkElementSmartTagPropertiesView : UserControl, IPopupOwner {
		public static IPopupOwner GetPopupOwner(DependencyObject obj) {
			return (IPopupOwner)obj.GetValue(PopupOwnerProperty);
		}
		public static void SetPopupOwner(DependencyObject obj, IPopupOwner value) {
			obj.SetValue(PopupOwnerProperty, value);
		}
		public static readonly DependencyProperty PopupOwnerProperty =
			DependencyProperty.RegisterAttached("PopupOwner", typeof(IPopupOwner), typeof(FrameworkElementSmartTagPropertiesView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnPopupOwnerChanged)));
		static void OnPopupOwnerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(!(d is Popup))
				return;
			Popup popup = (Popup)d;
			if(e.OldValue != null) {
				popup.Opened -= ((IPopupOwner)e.OldValue).OnPopupOpen;
				popup.Closed -= ((IPopupOwner)e.OldValue).OnPopupClose;
			}
			if(e.NewValue != null) {
				popup.Opened += ((IPopupOwner)e.NewValue).OnPopupOpen;
				popup.Closed += ((IPopupOwner)e.NewValue).OnPopupClose;
			}
		}
		public FrameworkElementSmartTagPropertiesView() {
			SetPopupOwner(this, this);
			InitializeComponent();
		}
		void AddPopup(Popup popup) {
			for(int i = OpenedPopups.Count; --i >= 0; ) {
				Popup openedPopup = OpenedPopups[i].Target as Popup;
				if(openedPopup == popup) return;
				if(openedPopup == null)
					OpenedPopups.RemoveAt(i);
			}
			OpenedPopups.Add(new WeakReference(popup));
		}
		void RemovePopup(Popup popup) {
			for(int i = OpenedPopups.Count; --i >= 0; ) {
				Popup openedPopup = OpenedPopups[i].Target as Popup;
				if(openedPopup == null || openedPopup == popup)
					OpenedPopups.RemoveAt(i);
			}
		}
		List<WeakReference> openedPopups;
		List<WeakReference> OpenedPopups {
			get {
				if(openedPopups == null)
					openedPopups = new List<WeakReference>();
				return openedPopups;
			}
		}
		IEnumerable<Popup> IPopupOwner.OpenedPopups {
			get {
				for(int i = OpenedPopups.Count; --i >= 0; ) {
					Popup openedPopup = OpenedPopups[i].Target as Popup;
					if(openedPopup == null)
						OpenedPopups.RemoveAt(i);
					else
						yield return openedPopup;
				}
			}
		}
		void IPopupOwner.OnPopupOpen(object sender, EventArgs e) {
			AddPopup((Popup)sender);
		}
		void IPopupOwner.OnPopupClose(object sender, EventArgs e) {
			RemovePopup((Popup)sender);
		}
	}
	public interface IPopupOwner {
		void OnPopupOpen(object sender, EventArgs e);
		void OnPopupClose(object sender, EventArgs e);
		IEnumerable<Popup> OpenedPopups { get; }
	}
}
