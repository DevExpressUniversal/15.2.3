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
using System.Security;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using System.ComponentModel;
namespace DevExpress.ExpressApp {
	public class PermissionAuditResult {
		private bool? isGranted;
		public bool? IsGranted {
			get { return isGranted; }
			set { isGranted = value; }
		}
	}
	public interface IPermissionAuditor {
		void IsGranted(IPermission permission, bool isGrantedDefault, PermissionAuditResult result);
	}
	public static class SecuritySystem {
		[Obsolete("Use 'IsGrantedAdapter.Enable()' instead.", true)]
		public static bool CanRefreshPermissions { get; set; }
		private static bool allowReloadPermissions = true;
		private static IValueManager<List<IPermissionAuditor>> permissionAuditorsValueManager;
		private static object lockObject = new object();
		private static List<IPermissionAuditor> permissionAuditors { 
			get {
				permissionAuditorsValueManager = ValueManager.GetValueManager<List<IPermissionAuditor>>("SecuritySystem_IPermissionAuditor");
				if(permissionAuditorsValueManager.Value == null) {
					permissionAuditorsValueManager.Value = new List<IPermissionAuditor>();
				}
				return permissionAuditorsValueManager.Value;
			}
		}
		static SecuritySystem() {
		}
		public static void SetInstance(ISecurityStrategyBase instance) {
			ValueManager.GetValueManager<ISecurityStrategyBase>("SecuritySystem_ISecurityStrategyBase").Value = instance;
		}
		public static bool IsGranted(IPermissionRequest permissionRequest) {
			IRequestSecurity instanceEx = Instance as IRequestSecurity;			
			if(instanceEx != null) {
				PermissionRequest customPermissionRequest = permissionRequest as PermissionRequest;
				if(customPermissionRequest != null) {
					if(CustomIsGranted != null) {
						CustomHasPermissionToEventArgs args = new CustomHasPermissionToEventArgs(customPermissionRequest.ObjectType, customPermissionRequest.MemberName, customPermissionRequest.TargetObject, customPermissionRequest.ObjectSpace, customPermissionRequest.Operation);
						CustomIsGranted(null, args);
						if(args.Handled) {
							return args.Result;
						}
					}					
				}
				return instanceEx.IsGranted(permissionRequest);
			}
			return false;
		}
		public static bool IsGranted(IObjectSpace objectSpace, Type objectType, string operation, object targetObject, string memberName) {
			Type targetObjectType = objectType;
			if(!(targetObject is XafDataViewRecord)) {
				targetObjectType = DataManipulationRight.GetTargetObjectType(objectType, targetObject);
			}
			PermissionRequest permissionRequest = new PermissionRequest(objectSpace, targetObjectType, operation, targetObject, memberName);
			return IsGranted(permissionRequest);
		}
		public static bool IsGranted(IPermission permission) {
			bool result = true;
			ISecurity iSecurityInstance = Instance as ISecurity;
			if(iSecurityInstance != null) {
				result = iSecurityInstance.IsGranted(permission);
			}
			IPermissionAuditor hasOpinionAuditor = null;
			bool hasOpinionAuditorResult = false;
			bool isGrantedByAuditors = true;
			if(permissionAuditors != null) {
				foreach(IPermissionAuditor currentAuditor in permissionAuditors) {
					PermissionAuditResult currentAuditorResult = new PermissionAuditResult();
					currentAuditor.IsGranted(permission, result, currentAuditorResult);
					if(currentAuditorResult.IsGranted.HasValue) {
						isGrantedByAuditors &= currentAuditorResult.IsGranted.Value;
						if(hasOpinionAuditor == null) {
							hasOpinionAuditorResult = currentAuditorResult.IsGranted.Value;
							hasOpinionAuditor = currentAuditor;
						}
						if((hasOpinionAuditor != null) && (hasOpinionAuditor != currentAuditor) && (hasOpinionAuditorResult != currentAuditorResult.IsGranted)) {
							Tracing.Tracer.LogWarning("A conflict occurs when auditing the '{0}' permission: '{1}' auditor {2} it, '{3}' auditor {4} it. The permission is denied.",
								permission.ToString(),
								hasOpinionAuditor, hasOpinionAuditorResult ? "grants" : "denies",
								currentAuditor, currentAuditorResult.IsGranted.Value ? "grants" : "denies");
						}
					}
				}
			}
			else {
				result = false;
				isGrantedByAuditors = false;
			}
			return result && isGrantedByAuditors;
		}
		public static void Demand(IPermissionRequest permissionRequest) {
			if(!((IRequestSecurity)SecuritySystem.Instance).IsGranted(permissionRequest)) {
				throw new SecurityException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.PermissionIsDenied));
			}
		}
		public static void Demand(IPermission permission) {
			if(!IsGranted(permission)) {
				throw new SecurityException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.PermissionIsDenied));
			}
		}
		public static void ReloadPermissions() {
			if(Instance != null && AllowReloadPermissions) {
				Instance.ReloadPermissions();
			}
		}
		public static bool AllowReloadPermissions {
			get { return allowReloadPermissions; }
			set { allowReloadPermissions = value; }
		}
		public static bool IsAuthenticated {
			get { return Instance == null ? true : Instance.IsAuthenticated; }
		}
		public static void Logon(IObjectSpace objectSpace) {
			if(!Instance.IsAuthenticated) {
				Instance.Logon(objectSpace);
			}
		}
		public static void AddPermissionAuditor(IPermissionAuditor auditor) { 
			permissionAuditors.Add(auditor);
		}
		public static ISecurityStrategyBase Instance {
			get { return ValueManager.GetValueManager<ISecurityStrategyBase>("SecuritySystem_ISecurityStrategyBase").Value; }
		}
		public static object CurrentUser { get { return Instance == null ? null : Instance.User; } }
		public static object LogonParameters { get { return Instance == null ? null : Instance.LogonParameters; } }
		public static string CurrentUserName { get { return Instance == null ? "" : Instance.UserName; } }
		public static object CurrentUserId { get { return Instance == null ? "" : Instance.UserId; } }
		public static Type UserType { get { return Instance == null ? null : Instance.UserType; } }
		public static bool IsLogoffEnabled { get { return Instance == null ? false : Instance.IsLogoffEnabled; } }
		public static IObjectSpace LogonObjectSpace { get { return Instance == null ? null : Instance.LogonObjectSpace; } }
		public static event EventHandler<CustomHasPermissionToEventArgs> CustomIsGranted;
	}
	public class CustomHasPermissionToEventArgs : HandledEventArgs {
		public CustomHasPermissionToEventArgs(Type objectType, String memberName, Object targetObject, IObjectSpace objectSpace, string operation) {
			TargetObject = targetObject;
			ObjectType = objectType;
			MemberName = memberName;
			Operation = operation;
			ObjectSpace = objectSpace;
		}
		public object TargetObject;
		public Type ObjectType;
		public string MemberName;
		public string Operation;
		public IObjectSpace ObjectSpace;
		public bool Result { get; set; }
	}
}
