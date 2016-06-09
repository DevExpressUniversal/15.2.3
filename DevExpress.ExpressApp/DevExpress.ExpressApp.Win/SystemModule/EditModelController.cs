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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class EditModelController : WindowController {
		private System.ComponentModel.IContainer components;
		private SimpleAction editModelAction;
		public SimpleAction EditModelAction {
			get { return editModelAction; }
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			editModelAction = new SimpleAction(components);
			editModelAction.Id = "EditModel";
			editModelAction.Caption = "Edit Model";
			editModelAction.Category = "Tools";
			editModelAction.ImageName = "MenuBar_EditModel";
			editModelAction.Shortcut = "CtrlShiftF1";
			editModelAction.Execute += new SimpleActionExecuteEventHandler(action_OnExecute);
			this.TargetWindowType = WindowType.Main;
		}
		private void action_OnExecute(Object sender, SimpleActionExecuteEventArgs args) {
			EditModel();
		}
		private void UpdateActivity() {
			if(SecuritySystem.Instance is IRequestSecurity) {
				Active.SetItemValue("Security", ((IRequestSecurity)SecuritySystem.Instance).IsGranted(new ModelOperationPermissionRequest()));
			}
			else {
				Active.SetItemValue("Security", SecuritySystem.IsGranted(new EditModelPermission(ModelAccessModifier.Allow)));
			}
		}
		protected virtual void EditModel() {
			SecuritySystem.ReloadPermissions();
			UpdateActivity();
			if(SecuritySystem.Instance is IRequestSecurity) {
				SecuritySystem.Demand(new ModelOperationPermissionRequest());
			}
			else {
				SecuritySystem.Demand(new EditModelPermission(ModelAccessModifier.Allow));
			}
			((WinApplication)Application).EditModel();
		}
		protected override void OnWindowChanging(Window window) {
			base.OnWindowChanging(window);
			UpdateActivity();
		}
		public EditModelController() : base() {
			InitializeComponent();
			RegisterActions(components);
		}
	}
}
