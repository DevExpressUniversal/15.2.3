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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
namespace DevExpress.XtraDiagram.Bars {
	public abstract class DiagramCommandDropDownButtonItem : DiagramCommandBarButtonItem {
		public DiagramCommandDropDownButtonItem() {
			ButtonStyle = BarButtonStyle.DropDown;
		}
		protected override void Initialize() {
			base.Initialize();
		}
		protected virtual void InitializePopupMenu(PopupMenu popupMenu) {
		}
		protected virtual void SubscribePopupMenu(PopupMenu popupMenu) {
			popupMenu.Popup += OnPopupMenuPopup;
		}
		protected virtual void UnsubscribePopupMenu(PopupMenu popupMenu) {
			popupMenu.Popup -= OnPopupMenuPopup;
		}
		protected virtual void OnPopupMenuPopup(object sender, EventArgs e) {
			ForEachPopupMenuItem(cmdItem => cmdItem.UpdateChecked());
		}
		protected void ForEachPopupMenuItem(Action<ICommandBarItem> action) {
			if(PopupMenu == null) return;
			foreach(BarItemLink link in PopupMenu.ItemLinks) {
				ICommandBarItem cmdItem = link.Item as ICommandBarItem;
				if(cmdItem != null)
					action(cmdItem);
			}
		}
		protected override void OnManagerChanged() {
			base.OnManagerChanged();
		}
		protected PopupMenu PopupMenu { get { return DropDownControl as PopupMenu; } }
		public override PopupControl DropDownControl {
			get { return base.DropDownControl; }
			set {
				PopupControl prevValue = DropDownControl;
				base.DropDownControl = value;
				if(prevValue != value) OnDropDownControlChanged(prevValue, DropDownControl);
			}
		}
		protected virtual void OnDropDownControlChanged(PopupControl prevValue, PopupControl newValue) {
			if(prevValue != null && prevValue is PopupMenu) {
				UnsubscribePopupMenu(((PopupMenu)prevValue));
			}
			if(newValue != null && newValue is PopupMenu) {
				SubscribePopupMenu(((PopupMenu)newValue));
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(PopupMenu != null) UnsubscribePopupMenu(PopupMenu);
			}
			base.Dispose(disposing);
		}
	}
}
