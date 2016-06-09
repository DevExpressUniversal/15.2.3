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
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Alerter {
	class AlertPopupMenuButton : AlertPredefinedButton {
		PopupMenu menu;
		AlertButtonCollection collection;
		public AlertPopupMenuButton(AlertForm form, AlertButtonCollection collection, PopupMenu menu)
			: base(AlertControlHelper.WindowImages.Images[3]) {
			this.menu = menu;
			this.collection = collection;
			SetOwner(form);
			Bounds = form.ViewInfo.GetControlBoxElementRectangle(collection.PredefinedButtonCount, this.GetButtonSize());
			if(menu != null)
				menu.CloseUp += new EventHandler(menu_CloseUp);
		}
		protected override Image SkinImage { get { return GetSkinImage(0); } }
		public override void Dispose() {
			if(menu != null) 
				menu.CloseUp -= new EventHandler(menu_CloseUp);
			base.Dispose();
		}
		void menu_CloseUp(object sender, EventArgs e) {
			Owner.Pin = IsPinDown;
			actionElement = false;
			Invalidate();
		}
		bool IsPinDown {
			get {
				foreach(AlertButton item in collection) {
					if(item is AlertPinButton && item.Down)
						return true;
				}
				return false;
			}
		}
		void SetInfoRecursion(object container) {
			BarLinkContainerItem cont = container as BarLinkContainerItem;
			if(cont == null) return;
			foreach(BarItemLink link in cont.ItemLinks) {
				SetInfoRecursion(link.Item);
				link.Item.Tag = new AlertClickEventArgs(Owner.Info, Owner);
			}
		}
		public override void OnClick() {
			base.OnClick();
			Owner.Pin = true;
			actionElement = true;
			foreach(BarItemLink link in menu.ItemLinks) {
				SetInfoRecursion(link.Item);
				link.Item.Tag = new AlertClickEventArgs(Owner.Info, Owner);
			}
			this.menu.Activator = Owner;
			this.menu.ShowPopup(Cursor.Position);
		}
	}
}
