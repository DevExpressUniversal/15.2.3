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
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.ExpressApp.Actions;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class ExitController : WindowController {
		public const string ExitActionId = "Exit";
		private System.ComponentModel.IContainer components;
		private DevExpress.ExpressApp.Actions.SimpleAction exitAction;
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.exitAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
			this.exitAction.Caption = "Exit";
			this.exitAction.Category = "Exit";
			this.exitAction.Id = ExitActionId;
			this.exitAction.ImageName = "MenuBar_Exit";
			this.exitAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.exitAction_OnExecute);
			this.TargetWindowType = DevExpress.ExpressApp.WindowType.Main;
		}
		public ExitController()
			: base() {
			InitializeComponent();
			RegisterActions(components);
		}
		private void exitAction_OnExecute(Object sender, SimpleActionExecuteEventArgs args) {
			Exit();
		}
		protected virtual void Exit() {
			Window.Close();
		}
		public DevExpress.ExpressApp.Actions.SimpleAction ExitAction {
			get { return exitAction; }
		}
	}
}
