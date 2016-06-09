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
using System.ComponentModel;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
namespace DevExpress.ExpressApp.Security {
	public class CustomChangePasswordEventArgs : HandledEventArgs {
		public CustomChangePasswordEventArgs(ChangePasswordParameters changePasswordParameters) {
			this.ChangePasswordParameters = changePasswordParameters;
		}
		public ChangePasswordParameters ChangePasswordParameters { get; private set; }
	}
	public class ChangePasswordController : BaseManagePasswordController {
		private const string ViewCurrentObjectRepresentsCurrentUserKey = "ViewCurrentObjectRepresentsCurrentUser";
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void ObjectSpace_ModifiedChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void ChangeMyPassword_OnCustomizePopupWindowParams(Object sender, CustomizePopupWindowParamsEventArgs args) {
			IObjectSpace objectSpace = ObjectSpaceInMemory.CreateNew();
			args.View = Application.CreateDetailView(objectSpace, new ChangePasswordParameters());
			((DetailView)args.View).ViewEditMode = ViewEditMode.Edit;
		}
		private void ChangeMyPassword_OnExecute(Object sender, PopupWindowShowActionExecuteEventArgs args) {
			Guard.ArgumentNotNull(args.PopupWindow, "args.PopupWindow");
			Guard.ArgumentNotNull(args.PopupWindow, "args.PopupWindow.View");
			ChangePassword((ChangePasswordParameters)args.PopupWindow.View.CurrentObject);
		}
		private bool IsCurrentUser(IObjectSpace objectSpace, object obj) {
			if((obj == null) || (SecuritySystem.CurrentUser == null)) {
				return false;
			}
			string currentUserHandle = objectSpace.GetObjectHandle(SecuritySystem.CurrentUser);
			string currentObjectHandle = objectSpace.GetObjectHandle(obj);
			return (currentUserHandle == currentObjectHandle);
		}
		private void UpdateActionState() {
			bool result = true;
			ChangeMyPasswordAction.Active[IsChangePasswordSupportedKey] = IsChangePasswordSupported;
			result &= IsChangePasswordSupported;
			object targetUserObject = SecuritySystem.CurrentUser;
			ChangeMyPasswordAction.Active[HasTargetObjectKey] = (targetUserObject != null);
			result &= (targetUserObject != null);
			ChangeMyPasswordAction.Active[IsIAuthenticationStandardUserKey] = targetUserObject is IAuthenticationStandardUser;
			result &= (targetUserObject is IAuthenticationStandardUser);
			if(result) {
				bool isCurrentUser = IsCurrentUser(View.ObjectSpace, View.CurrentObject);
				ChangeMyPasswordAction.Active.SetItemValue(ViewCurrentObjectRepresentsCurrentUserKey, isCurrentUser);
				if(isCurrentUser) {
					ChangeMyPasswordAction.Enabled[ObjectSpaceIsNotModifiedKey] = !View.ObjectSpace.IsModified; 
					bool canEditPasswordProperty = false;
					if(ResetPasswordController.TryGetCanEditPasswordProperty(View.CurrentObject, null, View.ObjectSpace, out canEditPasswordProperty)) {
						ChangeMyPasswordAction.Active.SetItemValue("ResetPasswordController.TryGetCanEditPasswordProperty", canEditPasswordProperty);
					}
				}
			}
			if(ActionStateUpdated != null) {
				ActionStateUpdated(this, EventArgs.Empty);
			}
		}
		protected virtual void ChangePassword(ChangePasswordParameters parameters) {
			Guard.ArgumentNotNull(parameters, "parameters");
			try {
				CustomChangePasswordEventArgs customChangePasswordEventArgs = new CustomChangePasswordEventArgs(parameters);
				if(CustomChangePassword != null) {
					CustomChangePassword(this, customChangePasswordEventArgs);
				}
				if(!customChangePasswordEventArgs.Handled) {
					using(IObjectSpace objectSpace = Application.CreateObjectSpace((SecuritySystem.CurrentUser != null) ? SecuritySystem.CurrentUser.GetType() : null)) {
						IAuthenticationStandardUser user = (IAuthenticationStandardUser)objectSpace.GetObject(SecuritySystem.CurrentUser);
						if(!user.ComparePassword(parameters.OldPassword)) {
							throw new Exception(SecurityExceptionLocalizer.GetExceptionMessage(SecurityExceptionId.OldPasswordIsWrong) + " " + SecurityExceptionLocalizer.GetExceptionMessage(SecurityExceptionId.RetypeTheInformation));
						}
						if(parameters.NewPassword != parameters.ConfirmPassword) {
							throw new Exception(SecurityExceptionLocalizer.GetExceptionMessage(SecurityExceptionId.PasswordsAreDifferent) + " " + SecurityExceptionLocalizer.GetExceptionMessage(SecurityExceptionId.RetypeTheInformation));
						}
						if(user.ComparePassword(parameters.NewPassword)) {
							throw new Exception(SecurityExceptionLocalizer.GetExceptionMessage(SecurityExceptionId.NewPasswordIsEqualToOldPassword) + " " + SecurityExceptionLocalizer.GetExceptionMessage(SecurityExceptionId.RetypeTheInformation));
						}
						user.SetPassword(parameters.NewPassword);
						objectSpace.SetModified(user);
						SecurityModule.CommitCurrentUserLogonParameters(objectSpace, parameters.NewPassword, parameters);
						if(!View.ObjectSpace.IsModified) {
							bool isCurrentUser = IsCurrentUser(View.ObjectSpace, View.CurrentObject);
							if(isCurrentUser) {
								View.ObjectSpace.ReloadObject(View.CurrentObject); 
							}
						}
					}
				}
			}
			finally {
				parameters.ClearValues();
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
			View.ObjectSpace.ModifiedChanged += new EventHandler(ObjectSpace_ModifiedChanged);
			UpdateActionState();
		}
		protected override void OnDeactivated() {
			View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
			View.ObjectSpace.ModifiedChanged -= ObjectSpace_ModifiedChanged;
			base.OnDeactivated();
		}
		public ChangePasswordController()
			: base() {
			this.ChangeMyPasswordAction = new PopupWindowShowAction(this, "ChangePasswordByUser", PredefinedCategory.Edit);
			this.ChangeMyPasswordAction.Caption = "Change My Password";
			this.ChangeMyPasswordAction.ImageName = "Security";
			this.ChangeMyPasswordAction.Execute += new PopupWindowShowActionExecuteEventHandler(this.ChangeMyPassword_OnExecute);
			this.ChangeMyPasswordAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(this.ChangeMyPassword_OnCustomizePopupWindowParams);
			TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
			TargetViewType = ViewType.DetailView; 
			TargetObjectType = typeof(IAuthenticationStandardUser);
		}
		public PopupWindowShowAction ChangeMyPasswordAction { get; private set; }
		public event EventHandler<CustomChangePasswordEventArgs> CustomChangePassword;
		public event EventHandler<EventArgs> ActionStateUpdated;
	}
	[DomainComponent]
	public class ChangePasswordParameters : ChangePasswordOnLogonParameters {
		private string oldPassword;
		[ModelDefault("IsPassword", "True")]
		public string OldPassword {
			get { return oldPassword; }
			set {
				oldPassword = value;
				RaisePropertyChanged("OldPassword");
			}
		}
		public override void ClearValues() {
			base.ClearValues();
			OldPassword = "";
		}
	}
}
