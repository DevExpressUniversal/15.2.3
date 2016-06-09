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
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Actions;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class CloseWindowController : WindowController {
		private SimpleAction closeAction;
		private System.ComponentModel.IContainer components;
		private void UpdateActions() {
			closeAction.Active.SetItemValue("View is null", Window.View != null);
		}
		private void Window_ViewChanged(object sender, ViewChangedEventArgs e) {
			UpdateActions();
		}
		private void closeAction_OnExecute(Object sender, SimpleActionExecuteEventArgs args) {
			Close();
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.closeAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
			this.closeAction.Caption = "Close";
			this.closeAction.Category = "Close";
			this.closeAction.ConfirmationMessage = "";
			this.closeAction.Id = "Close";
			this.closeAction.ImageName = "MenuBar_Close";
			this.closeAction.Application = null;
			this.closeAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.closeAction_OnExecute);
			this.TargetWindowType = DevExpress.ExpressApp.WindowType.Child;
		}
		protected virtual void Close() {
			Window.Close();
		}
		protected override void OnDeactivated() {
			Window.ViewChanged -= new EventHandler<ViewChangedEventArgs>(Window_ViewChanged);
			base.OnDeactivated();
		}
		protected override void OnActivated() {
			base.OnActivated();
			Window.ViewChanged += new EventHandler<ViewChangedEventArgs>(Window_ViewChanged);
			if(Window.View != null) {
				UpdateActions();
			}
		}
		public CloseWindowController()
			: base() {
			InitializeComponent();
			RegisterActions(components);
		}
		public SimpleAction CloseAction {
			get { return closeAction; }
		}
	}
}
