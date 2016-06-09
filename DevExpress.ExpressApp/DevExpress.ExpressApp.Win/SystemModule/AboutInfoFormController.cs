#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Editors;
using System.Windows.Forms;
using DevExpress.XtraLayout;
using DevExpress.XtraEditors;
using System.Drawing;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class AboutInfoFormController : ViewController {
		private void popupForm_CustomizeMinimumSize(object sender, CustomSizeEventArgs e) {
			e.CustomSize = new Size(15, 15);
			e.Handled = true;
		}
		private void popupForm_KeyDown(object sender, KeyEventArgs e) {
			if((e.KeyCode == Keys.Escape) && !e.Control && !e.Shift && !e.Alt) {
				((Form)sender).Close();
				e.Handled = true;
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.ControlsCreated += new EventHandler(OnAboutInfoViewControlsCreated);
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			View.ControlsCreated -= new EventHandler(OnAboutInfoViewControlsCreated);
		}
		protected virtual void OnAboutInfoViewControlsCreated(object sender, EventArgs e) {
			StaticImageViewItem image = ((DetailView)View).FindItem("Logo") as StaticImageViewItem;
			WinWindow aboutWindow = Frame as WinWindow;
			if(aboutWindow != null) {
				PopupForm popupForm = aboutWindow.Form as PopupForm;
				if(popupForm != null) {
					if(image != null && !string.IsNullOrEmpty(image.ImageName)) {
						popupForm.CustomizeMinimumSize += new EventHandler<CustomSizeEventArgs>(popupForm_CustomizeMinimumSize);
					}
					popupForm.ShowInTaskbar = false;
					popupForm.KeyPreview = true;
					popupForm.KeyDown += new KeyEventHandler(popupForm_KeyDown);
				}
			}
			LayoutControl layoutControl = View.Control as LayoutControl;
			if(layoutControl != null) {
				if(image != null && string.IsNullOrEmpty(image.ImageName)) {
					layoutControl.HideItem(layoutControl.GetItemByControl((Control)image.Control));
				}
				layoutControl.AllowCustomization = false;
			}
			StaticTextViewItem text = ((DetailView)View).FindItem("AboutText") as StaticTextViewItem;
			LabelControl labelControl = text.Control as LabelControl;
			if(labelControl != null) {
				labelControl.AllowHtmlString = true;
			}
		}
		public AboutInfoFormController() : base() {
			TargetObjectType = typeof(AboutInfo);
			TargetViewType = ViewType.DetailView;
		}		
	}
}
