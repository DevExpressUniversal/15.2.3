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
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Security.Strategy.PermissionMatrix;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
namespace DevExpress.ExpressApp.Security {
	public class CustomUpdateLogonParametersEventArgs : HandledEventArgs {
		public CustomUpdateLogonParametersEventArgs(string newPassword, object sourceObject) {
			this.NewPassword = newPassword;
			this.SourceObject = sourceObject;
		}
		public string NewPassword { get; private set; }
		public object SourceObject { get; private set; }
	}
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Supplies various security system strategies and authentication implementations. Allows you to assign different permissions to different users or user groups. Supports custom Security strategies and authentication implementations if they use the Business Class Library's interfaces.")]
	[ToolboxBitmap(typeof(DevExpress.ExpressApp.Security.SecurityModule), "Resources.Toolbox_Module_Security.ico")]
	public sealed class SecurityModule : ModuleBase {
		private const bool CanHandleObjectFormatterCustomGetValueEventDefaultValue = false; 
		public static bool StrictSecurityStrategyBehavior = true;
		public static UsedExportedTypes UsedExportedTypes = UsedExportedTypes.XPObjects;
		[DefaultValue(CanHandleObjectFormatterCustomGetValueEventDefaultValue)]
		public static bool CanHandleObjectFormatterCustomGetValueEvent { get; set; }
		private const string shouldChangePwdKeyName = "user should change password on first logon";
		private PopupWindowShowAction changePasswordOnLogon;
		private Type userType;
		private bool isChangePasswordOnLogonExecuted = false;
		private void changePasswordOnLogon_OnCancel(object sender, EventArgs e) {
			changePasswordOnLogon.Application.Exit();
			isChangePasswordOnLogonExecuted = true;
		}
		private void View_Closing(object sender, EventArgs e) {
			if(!isChangePasswordOnLogonExecuted) {
				changePasswordOnLogon.Application.Exit();
			}
		}
		private void changePasswordOnLogon_OnCustomizePopupWindowParams(Object sender, CustomizePopupWindowParamsEventArgs args) {
			IObjectSpace objectSpace = ObjectSpaceInMemory.CreateNew();
			args.View = changePasswordOnLogon.Application.CreateDetailView(objectSpace, new ChangePasswordOnLogonParameters());
			args.View.Closing += View_Closing;
			((DetailView)args.View).ViewEditMode = ViewEditMode.Edit;
			args.Context = TemplateContext.PopupWindow;
			Type applicationType = Application.GetType();
			if(applicationType != null) {
				while(applicationType != typeof(object)) {
					if(applicationType.Name == "WebApplication") {
						args.DialogController.CancelAction.Active["NoCancelInChangePasswordView"] = false;
						break;
					}
					applicationType = applicationType.BaseType;
				}
			}
		}
		private void changePasswordOnLogon_OnExecute(Object sender, PopupWindowShowActionExecuteEventArgs args) {
			Guard.ArgumentNotNull(args.PopupWindow, "args.PopupWindow");
			Guard.ArgumentNotNull(args.PopupWindow, "args.PopupWindow.View");
			ChangePasswordOnLogon((ChangePasswordOnLogonParameters)args.PopupWindow.View.CurrentObject);
			isChangePasswordOnLogonExecuted = true;
			changePasswordOnLogon.Active.SetItemValue(shouldChangePwdKeyName, false);
		}
		private void ObjectSpace_ObjectDeleting(object sender, ObjectsManipulatingEventArgs e) {
		}
		private void ObjectSpace_ObjectSaving(object sender, ObjectManipulatingEventArgs e) {
		}
		private void ChangePasswordOnLogon(ChangePasswordOnLogonParameters logonPasswordParameters) {
			Guard.ArgumentNotNull(logonPasswordParameters, "logonPasswordParameters");
			Guard.ArgumentNotNull(Application, "Application");
			Guard.ArgumentNotNull(SecuritySystem.CurrentUser, "SecuritySystem.CurrentUser");
			CustomChangePasswordOnLogonEventArgs args = new CustomChangePasswordOnLogonEventArgs(logonPasswordParameters, Application);
			if(CustomChangePasswordOnLogon != null) {
				CustomChangePasswordOnLogon(this, args);
			}
			if(!args.Handled) {
				using(IObjectSpace objectSpace = Application.CreateObjectSpace(SecuritySystem.CurrentUser.GetType())) {
					IAuthenticationStandardUser user = (IAuthenticationStandardUser)objectSpace.GetObject(SecuritySystem.CurrentUser);
					if(user == null) {
						throw new InvalidOperationException(SecurityExceptionLocalizer.GetExceptionMessage(SecurityExceptionId.UnableToReadCurrentUserData, ((IAuthenticationStandardUser)SecuritySystem.CurrentUser).UserName));
					}
					if(logonPasswordParameters.NewPassword != logonPasswordParameters.ConfirmPassword) {
						throw new Exception(SecurityExceptionLocalizer.GetExceptionMessage(SecurityExceptionId.PasswordsAreDifferent) + " " + SecurityExceptionLocalizer.GetExceptionMessage(SecurityExceptionId.RetypeTheInformation));
					}
					if(user.ComparePassword(logonPasswordParameters.NewPassword)) {
						throw new Exception(SecurityExceptionLocalizer.GetExceptionMessage(SecurityExceptionId.NewPasswordIsEqualToOldPassword) + " " + SecurityExceptionLocalizer.GetExceptionMessage(SecurityExceptionId.RetypeTheInformation));
					}
					user.SetPassword(logonPasswordParameters.NewPassword);
					user.ChangePasswordOnFirstLogon = false;
					objectSpace.SetModified(user);
					SecurityModule.CommitCurrentUserLogonParameters(objectSpace, logonPasswordParameters.NewPassword, logonPasswordParameters);
				}
			}
		}
		private void application_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e) {
			e.ObjectSpace.ObjectSaving += new EventHandler<ObjectManipulatingEventArgs>(ObjectSpace_ObjectSaving);
			e.ObjectSpace.ObjectDeleting += new EventHandler<ObjectsManipulatingEventArgs>(ObjectSpace_ObjectDeleting);
		}
		private void application_LoggedOn(object sender, LogonEventArgs e) {
			if(SecuritySystem.CurrentUser is IUserWithRoles || SecuritySystem.CurrentUser is ISecurityUserWithRoles) {
				IsCurrentUserInRoleOperator.Register();
			}
		}
		private void ObjectFormatter_CustomGetValue(object sender, CustomGetValueEventArgs e) {
			if(e.Object != null) {
				ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(e.Object.GetType());
				if(typeInfo != null) {
					IMemberInfo memberInfo = typeInfo.FindMember(e.MemberPath);
					IObjectSpace objectSpace = null;
					if(UsedExportedTypes == UsedExportedTypes.XPObjects) {
						objectSpace = ObjectSpaceHelper.GetObjectSpace(e.Object);
					}
					if(memberInfo != null && !DataManipulationRight.CanRead(memberInfo.Owner.Type, memberInfo.Name, e.Object, null, objectSpace)) {
						e.Value = (Application != null && Application.Model != null) ? Application.Model.ProtectedContentText : EditorsFactory.ProtectedContentDefaultText;
						e.Handled = true;
					}
				}
			}
		}
		private bool userTypeValueWasUsed = false;
		public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
			base.AddGeneratorUpdaters(updaters);
			updaters.Add(new NavigationItemsMyDetailsNodeUpdater(UserType));
			userTypeValueWasUsed = true;
			updaters.Add(new NavigationItemComplexSecurityUpdater());
			updaters.Add(new ModelLocalizationGroupUpdater());
		}
		protected override void Dispose(bool disposing) {
			if(Application != null) {
				Application.ObjectSpaceCreated -= new EventHandler<ObjectSpaceCreatedEventArgs>(application_ObjectSpaceCreated);
				ObjectFormatter.CustomGetValue -= new EventHandler<CustomGetValueEventArgs>(ObjectFormatter_CustomGetValue);
			}
			base.Dispose(disposing);
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) { }
		protected override IEnumerable<Type> GetRegularTypes() {
			return null;
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return new Type[] {
				typeof(ChangePasswordOnLogonParameters),
				typeof(ChangePasswordParameters),
				typeof(ResetPasswordParameters),
				typeof(AuthenticationStandardLogonParameters),
				typeof(ObjectAccessPermission),
				typeof(EditModelPermission),
			};
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			List<Type> result = new List<Type>(){
				typeof(DevExpress.ExpressApp.Security.ChangePasswordController),
				typeof(DevExpress.ExpressApp.Security.HasRightsToModifyMemberController),
				typeof(DevExpress.ExpressApp.Security.LastAdminController),
				typeof(DevExpress.ExpressApp.Security.MyDetailsController),
				typeof(DevExpress.ExpressApp.Security.PermissionsController),
				typeof(DevExpress.ExpressApp.Security.RefreshSecurityController),
				typeof(DevExpress.ExpressApp.Security.ResetPasswordController),
				typeof(ListViewProcessMatrixTypePermissionObjectController),
				typeof(DevExpress.ExpressApp.Security.OwnerInitializingController)};
			if(UsedExportedTypes == UsedExportedTypes.XPObjects) {
				result.Add(typeof(PermissionMatrixGrantDenyController));
				result.Add(typeof(DevExpress.ExpressApp.Security.MemberPermissionsViewItemStateController));
				result.Add(typeof(MembersPermissionMatrixDisableLinkUnlinkController));
				result.Add(typeof(ObjectsPermissionMatrixDisableLinkUnlinkController));
			}
			return result;
		}
		static SecurityModule() {
			CanHandleObjectFormatterCustomGetValueEvent = CanHandleObjectFormatterCustomGetValueEventDefaultValue;
		}
		public SecurityModule() {
			SecurityExceptionResourceLocalizer.Register(typeof(SecurityExceptionResourceLocalizer));
			changePasswordOnLogon = new PopupWindowShowAction(null, "ChangePasswordOnLogon", "");
			changePasswordOnLogon.Execute += new PopupWindowShowActionExecuteEventHandler(changePasswordOnLogon_OnExecute);
			changePasswordOnLogon.Cancel += new EventHandler(changePasswordOnLogon_OnCancel);
			changePasswordOnLogon.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(changePasswordOnLogon_OnCustomizePopupWindowParams);
		}
		public override void Setup(XafApplication application) {
			base.Setup(application);
			if(application != null) {
				application.ObjectSpaceCreated += new EventHandler<ObjectSpaceCreatedEventArgs>(application_ObjectSpaceCreated);
				application.LoggedOn += new EventHandler<LogonEventArgs>(application_LoggedOn);
				if(CanHandleObjectFormatterCustomGetValueEvent) { 
					ObjectFormatter.CustomGetValue += new EventHandler<CustomGetValueEventArgs>(ObjectFormatter_CustomGetValue);
				}
			}
		}
		public override IList<PopupWindowShowAction> GetStartupActions() {
			SecuritySystem.ReloadPermissions();
			bool isChangePasswordSupported = SecuritySystem.Instance is ISupportChangePasswordOption ? ((ISupportChangePasswordOption)SecuritySystem.Instance).IsSupportChangePassword : false;
			changePasswordOnLogon.Active.SetItemValue("Change password option is supported by the security", isChangePasswordSupported);
			IAuthenticationStandardUser standardUser = SecuritySystem.CurrentUser as IAuthenticationStandardUser;
			bool doChangePasswordOnFirstLogon = (standardUser != null) && standardUser.ChangePasswordOnFirstLogon;
			changePasswordOnLogon.Active.SetItemValue(shouldChangePwdKeyName, doChangePasswordOnFirstLogon);
			return new PopupWindowShowAction[] { changePasswordOnLogon };
		}
		public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
			base.CustomizeTypesInfo(typesInfo);
			ITypeInfo typeInfo;
			if(UsedExportedTypes == UsedExportedTypes.XPObjects) {
				typeInfo = typesInfo.FindTypeInfo(typeof(TypePermissionMatrixItem));
				((TypeInfo)typeInfo).KeyMember = typeInfo.FindMember("Id");
			}
			if((ModuleManager != null) && (ModuleManager.Security != null)) {
				if(ModuleManager.Security.UserType != null) {
					typeInfo = typesInfo.FindTypeInfo(ModuleManager.Security.UserType);
					if(typeInfo.FindAttribute<NavigationItemAttribute>() == null) {
						typeInfo.AddAttribute(new NavigationItemAttribute());
					}
					if(typeInfo.FindAttribute<VisibleInReportsAttribute>() == null) {
						typeInfo.AddAttribute(new VisibleInReportsAttribute());
					}
				}
				IRoleTypeProvider roleTypeProvider = ModuleManager.Security as IRoleTypeProvider;
				if(roleTypeProvider != null && (roleTypeProvider.RoleType != null)) {
					ITypeInfo roleTypeInfo = typesInfo.FindTypeInfo(roleTypeProvider.RoleType);
					if(roleTypeInfo.FindAttribute<NavigationItemAttribute>() == null) {
						roleTypeInfo.AddAttribute(new NavigationItemAttribute());
					}
					if(roleTypeInfo.FindAttribute<VisibleInReportsAttribute>() == null) {
						roleTypeInfo.AddAttribute(new VisibleInReportsAttribute());
					}
				}
			}
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
		public override ICollection<Type> GetXafResourceLocalizerTypes() {
			ICollection<Type> result = new List<Type>();
			result.Add(typeof(ServerDataLogLocalizer));
			return result;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Type UserType {
			get {
				if(userType == null) {
					return SecurityModule.GetUserType(Application, ModuleManager);
				}
				else {
					return userType;
				}
			}
			set {
				if(userType == value) {
					return;
				}
				if(userTypeValueWasUsed) {
					throw new InvalidOperationException(); 
				}
				userType = value;
			}
		}
		public PopupWindowShowAction ChangePasswordOnLogonAction {
			get { return changePasswordOnLogon; }
		}
		public event EventHandler<CustomChangePasswordOnLogonEventArgs> CustomChangePasswordOnLogon;
		public static void CommitCurrentUserLogonParameters(IObjectSpace objectSpace, string password, object sourceObject = null) {
			objectSpace.ModifiedChanged += (sender, args) => {
				if(!objectSpace.IsModified) { 
					SecurityModule.TryUpdateLogonParameters(password, sourceObject);
				}
			};
			objectSpace.CommitChanges();
		}
		public static bool TryUpdateLogonParameters(string password, object sourceObject = null) {
			CustomUpdateLogonParametersEventArgs args = new CustomUpdateLogonParametersEventArgs(password, sourceObject);
			if(CustomUpdateLogonParameters != null) {
				CustomUpdateLogonParameters(null, args);
			}
			if(args.Handled) {
				return true;
			}
			else {
				AuthenticationStandardLogonParameters stdLogonParams = SecuritySystem.LogonParameters as AuthenticationStandardLogonParameters;
				if(stdLogonParams != null) {
					stdLogonParams.Password = password;
					return true;
				}
				return false;
			}
		}
		public static event EventHandler<CustomUpdateLogonParametersEventArgs> CustomUpdateLogonParameters;
		public static Type GetUserType(XafApplication application, ApplicationModulesManager modulesManager) {
			Type result = null;
			if(application != null && application.Security != null) {
				result = application.Security.UserType;
			}
			else if(modulesManager != null && modulesManager.Security != null) {
				result = modulesManager.Security.UserType;
			}
			else if(SecuritySystem.Instance != null) {
				result = SecuritySystem.Instance.UserType;
			}
			return result;
		}
	}
	public class CustomChangePasswordOnLogonEventArgs : HandledEventArgs {
		public CustomChangePasswordOnLogonEventArgs(ChangePasswordOnLogonParameters logonPasswordParameters, XafApplication application) {
			this.LogonPasswordParameters = logonPasswordParameters;
			this.Application = application;
		}
		public ChangePasswordOnLogonParameters LogonPasswordParameters { get; private set; }
		public XafApplication Application { get; private set; }
	}
	public class NavigationItemComplexSecurityUpdater : ModelNodesGeneratorUpdater<NavigationItemNodeGenerator> {
		public override void UpdateNode(ModelNode node) {
			if(node["Default"] != null && SecuritySystem.Instance != null && (SecuritySystem.Instance is ISecurityComplex)) {
				IModelNavigationItem defaultItemNode = node["Default"] as IModelNavigationItem;
				if(defaultItemNode.Items["Role_ListView"] == null) {
					IModelNavigationItem rolesNode = defaultItemNode.AddNode<IModelNavigationItem>();
					rolesNode.View = node.Application.Views["Role_ListView"];
					rolesNode.Caption = "Roles";
					rolesNode.ImageName = "BO_Role";
					rolesNode.Index = -1;
				}
				if(defaultItemNode.Items["User_ListView"] == null) {
					IModelNavigationItem usersNode = defaultItemNode.AddNode<IModelNavigationItem>();
					usersNode.View = node.Application.Views["User_ListView"];
					usersNode.Caption = "Users";
					usersNode.ImageName = "BO_User";
					usersNode.Index = -1;
				}
			}
		}
	}
	public class ModelLocalizationGroupUpdater : ModelNodesGeneratorUpdater<ModelLocalizationGroupGenerator> {
		public override void UpdateNode(ModelNode node) {
			if(node.Id == "Enums") {
				if(node[typeof(ObjectAccess).FullName] == null) {
					EnumDescriptor.GenerateDefaultCaptions(node.AddNode<IModelLocalizationGroup>(typeof(ObjectAccess).FullName), typeof(ObjectAccess), CompoundNameConvertStyle.None);
				}
			}
			if(node.Id == PermissionTargetBusinessClassListConverter.SecurityLocalizationGroupName) {
				IModelLocalizationItem item1 = node.AddNode<IModelLocalizationItem>(PermissionTargetBusinessClassListConverter.CommonBaseObjectTypeItemName);
				item1.Value = PermissionTargetBusinessClassListConverter.CommonBaseObjectTypeCaption;
				IModelLocalizationItem item2 = node.AddNode<IModelLocalizationItem>(PermissionTargetBusinessClassListConverter.PersistentBaseObjectTypeItemName);
				item2.Value = PermissionTargetBusinessClassListConverter.PersistentBaseObjectTypeCaption;
			}
		}
	}
	public interface ISupportChangePasswordOption {
		bool IsSupportChangePassword { get; }
	}
	[DomainComponent]
	public class ChangePasswordOnLogonParameters : INotifyPropertyChanged {
		private string newPassword;
		private string confirmPassword;
		protected void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		public virtual void ClearValues() {
			ConfirmPassword = "";
			NewPassword = "";
		}
		[ModelDefault("IsPassword", "True")]
		public string NewPassword {
			get { return newPassword; }
			set {
				newPassword = value;
				RaisePropertyChanged("NewPassword");
			}
		}
		[ModelDefault("IsPassword", "True")]
		public string ConfirmPassword {
			get { return confirmPassword; }
			set {
				confirmPassword = value;
				RaisePropertyChanged("ConfirmPassword");
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
	internal static class ObjectSpaceHelper {
		public static IObjectSpace GetObjectSpace(object obj) {
			return DevExpress.ExpressApp.Xpo.XPObjectSpace.FindObjectSpaceByObject(obj);
		}
	}
}
