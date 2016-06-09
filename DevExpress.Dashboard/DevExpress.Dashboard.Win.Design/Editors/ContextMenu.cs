#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Drawing;
using DevExpress.DashboardWin.Native;
using DevExpress.Data.Utils;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Controls;
using PopupBarManager = DevExpress.XtraReports.Design.XtraContextMenuBase.PopupBarManager;
namespace DevExpress.DashboardWin.Design {
	public class ContextMenu : PopupMenu {
		protected static PopupBarManager MenuBarManager = new PopupBarManager() {
			Controller = new BarAndDockingController(),
			Images = new ImageCollection() { TransparentColor = Color.Magenta }
		};
		IServiceProvider provider;
		public ContextMenu(IServiceProvider provider)
			: base(MenuBarManager) {
			this.provider = provider;
		}
		protected override void OnBeforePopup(CancelEventArgs e) {
			MenuBarManager.Form = provider.GetService<SelectedContextService>().Designer;
			MenuBarManager.Controller.LookAndFeel.ParentLookAndFeel = provider.GetService<ILookAndFeelService>().LookAndFeel;
			base.OnBeforePopup(e);
		}
		protected override void OnCloseUp(CustomPopupBarControl prevControl) {
			base.OnCloseUp(prevControl);
			MenuBarManager.Form = null;
			MenuBarManager.Controller.LookAndFeel.ParentLookAndFeel = null;
		}
		public void AddItem(string caption, System.Drawing.Image image) {
			BarButtonItem item = new BarButtonItem();
			item.Caption = caption;
			this.AddItem(item, image);
		}
		protected void AddItem(BarButtonItem item, System.Drawing.Image image) {
			item.Manager = MenuBarManager;
			item.ImageIndex = (MenuBarManager.Images as ImageCollection).Images.Add(image);
			this.AddItem(item);
		}
	}
	public class ActionContextMenu : ContextMenu {
		class ActionButtonItem : BarButtonItem {
			Type type;
			EventHandler onClickHandler;
			public ActionButtonItem(string caption, Type type, EventHandler onClickHandler) : base() {
				this.Caption = caption;
				this.onClickHandler = onClickHandler;
				this.type = type;
			}
			protected override void OnClick(BarItemLink link) {
				base.OnClick(link);
				onClickHandler(type, EventArgs.Empty);
			}
		}
		public ActionContextMenu(IServiceProvider provider)
			: base(provider) {
		}
		public void AddItem(string caption, System.Drawing.Image image, Type type, EventHandler onClickHandler) {
			ActionButtonItem item = new ActionButtonItem(caption, type, onClickHandler);
			this.AddItem(item, image);
		}
	}
}
