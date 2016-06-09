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
using System.Security.Cryptography;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
namespace DevExpress.ExpressApp.Security {
	[DomainComponent]
	public class ResetPasswordParameters {
		private string password;
		public string Password {
			get { return password; }
			set { password = value; }
		}
	}
	public class CustomResetPasswordEventArgs : HandledEventArgs {
		public CustomResetPasswordEventArgs(object targetUser, ResetPasswordParameters resetPasswordParameters) {
			this.ResetPasswordParameters = resetPasswordParameters;
			this.TargetUser = targetUser;
		}
		public ResetPasswordParameters ResetPasswordParameters { get; private set; }
		public object TargetUser { get; private set; }
	}
	public abstract class BaseManagePasswordController : ObjectViewController {
		protected const string IsChangePasswordSupportedKey = "IsChangePasswordSupported";
		protected const string IsIAuthenticationStandardUserKey = "IsIAuthenticationStandardUser";
		protected const string ObjectSpaceIsNotModifiedKey = "ObjectSpaceIsNotModified";
		protected const string HasTargetObjectKey = "HasTargetObject";
		protected bool IsChangePasswordSupported { get; set; }
		protected static bool TryGetCanEditPasswordProperty(object targetObject, CollectionSourceBase collectionSource, IObjectSpace os, out bool result) {
			return TryGetCanEditProperty(PasswordFieldName, SecuritySystem.UserType, targetObject, collectionSource, os, out result);
		}
		protected static bool TryGetCanEditChangePasswordOnFirstLogonProperty(object targetObject, CollectionSourceBase collectionSource, IObjectSpace os, out bool result) {
			return TryGetCanEditProperty(ChangePasswordOnFirstLogonFieldName, SecuritySystem.UserType, targetObject, collectionSource, os, out result);
		}
		protected static bool TryGetCanEditProperty(string propertyName, Type targetType, object targetObject, CollectionSourceBase collectionSource, IObjectSpace os, out bool result) {
			result = false;
			if((SecuritySystem.Instance is IRequestSecurity) && (targetType != null)) {
				ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(targetType);
				IMemberInfo fieldInfo = typeInfo.FindMember(propertyName);
				if(fieldInfo != null) {
					result = DataManipulationRight.CanEdit(targetType, propertyName, targetObject, collectionSource, os);
					return true;
				}
			}
			return false;
		}
		protected override void OnActivated() {
			base.OnActivated();
			IsChangePasswordSupported = false;
			if(SecuritySystem.Instance is ISupportChangePasswordOption) {
				IsChangePasswordSupported = ((ISupportChangePasswordOption)SecuritySystem.Instance).IsSupportChangePassword;
			}
		}
		static BaseManagePasswordController() {
			ChangePasswordOnFirstLogonFieldName = "ChangePasswordOnFirstLogon";
			PasswordFieldName = "StoredPassword";
		}
		public static string PasswordFieldName { get; set; }
		public static string ChangePasswordOnFirstLogonFieldName { get; set; }
	}
	public class ResetPasswordController : BaseManagePasswordController {
		private void SetPasswordByAdmin_OnExecute(Object sender, PopupWindowShowActionExecuteEventArgs args) {
			ExecuteResetPassword(args.CurrentObject, (ResetPasswordParameters)args.PopupWindow.View.CurrentObject);
		}
		private void SetPasswordByAdmin_OnCustomizePopupWindowParams(Object sender, CustomizePopupWindowParamsEventArgs args) {
			CustomizeResetPasswordPopupWindowParams(args);
		}
		private void View_SelectionChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void ObjectSpace_ModifiedChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		protected virtual void UpdateActionState() {
			object targetUserObject = View.SelectedObjects.Count > 0 ? View.SelectedObjects[0] : null;
			if(ResetPasswordAction.Active[IsChangePasswordSupportedKey] && ResetPasswordAction.Enabled[IsIAuthenticationStandardUserKey]
				&& (targetUserObject != null)) {
				ResetPasswordAction.Enabled[ObjectSpaceIsNotModifiedKey] = !View.ObjectSpace.IsModified; 
				bool securityAllow = false;
				CollectionSourceBase collectionSource = LinkToListViewController.FindCollectionSource(Frame);
				if(SecuritySystem.Instance is IRequestSecurity) {
					securityAllow = DataManipulationRight.CanRead(targetUserObject.GetType(), null, targetUserObject, LinkToListViewController.FindCollectionSource(Frame), View.ObjectSpace);
					if(securityAllow) {
						bool canEditPasswordProperty = false;
						if(TryGetCanEditPasswordProperty(targetUserObject, collectionSource, View.ObjectSpace, out canEditPasswordProperty)) {
							securityAllow &= canEditPasswordProperty;
						}
						bool canEditChangePasswordOnFirstLogonProperty = false;
						if(TryGetCanEditChangePasswordOnFirstLogonProperty(targetUserObject, collectionSource, View.ObjectSpace, out canEditChangePasswordOnFirstLogonProperty)) {
							securityAllow &= canEditChangePasswordOnFirstLogonProperty;
						}
					}
				}
				else if(SecuritySystem.UserType != null) {
					securityAllow = SecuritySystem.UserType != null &&
						DataManipulationRight.CanRead(SecuritySystem.UserType, null, targetUserObject, collectionSource, View.ObjectSpace) &&
						DataManipulationRight.CanEdit(SecuritySystem.UserType, null, null, collectionSource, View.ObjectSpace);
				}
				ResetPasswordAction.Active.SetItemValue("SecurityAllow", securityAllow); 
			}
			if(ActionStateUpdated != null) {
				ActionStateUpdated(this, EventArgs.Empty);
			}
		}
		protected virtual void ExecuteResetPassword(object targetUserObject, ResetPasswordParameters resetPasswordParameters) {
			Guard.ArgumentNotNull(targetUserObject, "targetUserObject");
			Guard.ArgumentNotNull(resetPasswordParameters, "resetPasswordParameters");
			CustomResetPasswordEventArgs customResetPasswordEventArgs = new CustomResetPasswordEventArgs(targetUserObject, resetPasswordParameters);
			if(CustomResetPassword != null) {
				CustomResetPassword(this, customResetPasswordEventArgs);
			}
			if(!customResetPasswordEventArgs.Handled) {
				using(IObjectSpace objectSpace = Application.CreateObjectSpace(targetUserObject.GetType())) {
					IAuthenticationStandardUser user = (IAuthenticationStandardUser)objectSpace.GetObject(targetUserObject);
					user.SetPassword(resetPasswordParameters.Password);
					user.ChangePasswordOnFirstLogon = true;
					objectSpace.SetModified(user);
					if(SecuritySystem.CurrentUserId.Equals(ObjectSpace.GetKeyValue(targetUserObject))) {
						SecurityModule.CommitCurrentUserLogonParameters(objectSpace, resetPasswordParameters.Password, resetPasswordParameters);
					}
					else {
						objectSpace.CommitChanges();
					}
					if(!View.ObjectSpace.IsModified) {
						View.ObjectSpace.ReloadObject(targetUserObject); 
					}
				}
			}
		}
		protected virtual void CustomizeResetPasswordPopupWindowParams(CustomizePopupWindowParamsEventArgs args) {
			args.DialogController.AcceptAction.Caption = ResetPasswordAction.Caption;
			ResetPasswordParameters resetPasswordParameters = new ResetPasswordParameters();
			byte[] randomBytes = new byte[6];
			new RNGCryptoServiceProvider().GetBytes(randomBytes);
			resetPasswordParameters.Password = Convert.ToBase64String(randomBytes);
			IObjectSpace objectSpace = ObjectSpaceInMemory.CreateNew();
			args.View = Application.CreateDetailView(objectSpace, resetPasswordParameters);
			((DetailView)args.View).ViewEditMode = ViewEditMode.Edit;
			args.IsSizeable = false;
		}
		protected override void OnActivated() {
			base.OnActivated();
			ResetPasswordAction.Active[IsChangePasswordSupportedKey] = IsChangePasswordSupported;
			ResetPasswordAction.Enabled[IsIAuthenticationStandardUserKey] = typeof(IAuthenticationStandardUser).IsAssignableFrom(View.ObjectTypeInfo.Type);
			UpdateActionState();
			View.SelectionChanged += new EventHandler(View_SelectionChanged);
			View.ObjectSpace.ModifiedChanged += new EventHandler(ObjectSpace_ModifiedChanged);
		}
		protected override void OnDeactivated() {
			View.SelectionChanged -= new EventHandler(View_SelectionChanged);
			base.OnDeactivated();
		}
		public ResetPasswordController() {
			this.ResetPasswordAction = new PopupWindowShowAction(this, "ResetPassword", PredefinedCategory.Edit);
			this.ResetPasswordAction.ImageName = "Action_ResetPassword";
			this.ResetPasswordAction.ToolTip = "Generate a new password for the selected user";
			this.ResetPasswordAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
			this.ResetPasswordAction.Execute += new PopupWindowShowActionExecuteEventHandler(this.SetPasswordByAdmin_OnExecute);
			this.ResetPasswordAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(this.SetPasswordByAdmin_OnCustomizePopupWindowParams);
			TargetViewType = ViewType.DetailView;
			TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
			TargetObjectType = typeof(IAuthenticationStandardUser);
		}
		public PopupWindowShowAction ResetPasswordAction { get; private set; }
		public event EventHandler<CustomResetPasswordEventArgs> CustomResetPassword;
		public event EventHandler<EventArgs> ActionStateUpdated;
	}
}
