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
using System.Diagnostics;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Security.Adapters;
namespace DevExpress.ExpressApp.Security {
	public interface IOperationPermissionProvider { 
		IEnumerable<IOperationPermission> GetPermissions(); 
		IEnumerable<IOperationPermissionProvider> GetChildren(); 
	}
	public interface ISecurityRole { 
		string Name { get; }
	} 
	public class OperationPermissionProviderHelper {
		private static void FillPermissionsRecursive(List<IOperationPermission> permissions, List<IOperationPermissionProvider> processedProviders, IEnumerable<IOperationPermissionProvider> targetProviders) {
			Guard.ArgumentNotNull(permissions, "permissions");
			Guard.ArgumentNotNull(processedProviders, "processedProviders");
			if(targetProviders == null) {
				return;
			}
			foreach(IOperationPermissionProvider provider in targetProviders) {
				if(!processedProviders.Contains(provider)) {
					processedProviders.Add(provider);
					permissions.AddRange(provider.GetPermissions());
					FillPermissionsRecursive(permissions, processedProviders, provider.GetChildren());
				}
			}
		}
		public static IEnumerable<IOperationPermission> CollectPermissionsRecursive(IOperationPermissionProvider permissionProvider) {
			DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(permissionProvider, "permissionProvider");
			List<IOperationPermission> result = new List<IOperationPermission>();
			FillPermissionsRecursive(result, new List<IOperationPermissionProvider>(), new IOperationPermissionProvider[] { permissionProvider });
			return result;
		}
	}
	public class CustomizeRequestProcessorsEventArgs : EventArgs {
		public CustomizeRequestProcessorsEventArgs(IDictionary<Type, IPermissionRequestProcessor> processors, IPermissionDictionary permissions) {
			this.Processors = processors;
			this.Permissions = permissions;
		}
		public IDictionary<Type, IPermissionRequestProcessor> Processors { get; private set; }
		public IPermissionDictionary Permissions { get; private set; }
	}
	[ToolboxItem(false)] 
	[ToolboxItemFilter("Xaf", ToolboxItemFilterType.Require)]
	public abstract class SecurityStrategy : SecurityStrategyBase, IDataServerSecurity, ISupportLogonEvent {
		private static TraceLevel traceLevel = TraceLevel.Off;
		public static string AdministratorRoleName = "Administrator";
		public const string TraceSwitchName = "InternalSecurityBehaviorDebug";
		[Obsolete("Use 'AdditionalSecuredTypes' instead.", true)]
		public static IList<Type> SecuredNonPersistentTypes {
			get { return AdditionalSecuredTypes; }
		}
		public static readonly IList<Type> AdditionalSecuredTypes; 
		private static bool IsSecuredTypeCore(DevExpress.ExpressApp.DC.ITypeInfo typeInfo) {
			Guard.ArgumentNotNull(typeInfo, "typeInfo");
			if(typeInfo.IsPersistent && typeInfo.Type != null
				&& typeInfo.Type.FullName != "DevExpress.Xpo.XPObjectType"
				&& !typeof(IModuleInfo).IsAssignableFrom(typeInfo.Type)) {
				return true;
			}
			return false;
		}
		public static bool DelayPermissionDictionaryLoading { get; set; }
		public static bool IsSecuredType(Type type) {
			Guard.ArgumentNotNull(type, "type");
			if(AdditionalSecuredTypes.Contains(type)) {
				return true;
			}
			DevExpress.ExpressApp.DC.ITypeInfo ti =XafTypesInfo.Instance.FindTypeInfo(type);
			if(ti == null) {
				return false;
			}
			return IsSecuredTypeCore(ti);
		}
		public static IEnumerable<Type>  GetSecuredTypes() {
			List<Type> result = new List<Type>();
			foreach(DevExpress.ExpressApp.DC.ITypeInfo classInfo in XafTypesInfo.Instance.PersistentTypes) {
				if(IsSecuredTypeCore(classInfo)) {
					result.Add(classInfo.Type);
				}
			}
			foreach(Type t in AdditionalSecuredTypes) {
				if(!result.Contains(t)) {
					result.Add(t);
				}
			}
			return result;
		}
		private ISelectDataSecurity currentSelectDataSecurity;
		private IList<IObjectSpace> GetObjectSpaces() {
			IList<IObjectSpace> result = objectSpaces;
			if(result.Count == 0) {
				result = new IObjectSpace[] { LogonObjectSpace };
			}
			return result;
		}
		protected override void ReloadPermissionsCore() {
			if(User != null) {
				if(LogonObjectSpace != null) {
					LogonObjectSpace.ReloadObject(User); 
				}
				currentSelectDataSecurity = null;
			}
		}
		static SecurityStrategy() {
			AdditionalSecuredTypes = new List<Type>();
			TraceSwitch serverProcessorDebugSwitch = new TraceSwitch(TraceSwitchName, "0 - Off, 1 - Error, 2 - Warning, 3 - Info", "0");
			if(serverProcessorDebugSwitch != null) {
				traceLevel = serverProcessorDebugSwitch.Level;
			}
		}
		public SecurityStrategy() {
		}
		public SecurityStrategy(Type userType, AuthenticationBase authentication)
			: base(userType, authentication) {
		}
		public override void Logon(object user) {
			Guard.ArgumentNotNull(user, "user");
			ISecurityUser securityUser = (ISecurityUser)user;  
			if(securityUser != null && !securityUser.IsActive) {
				throw new AuthenticationException(securityUser.UserName);
			}
			base.Logon(user);
			currentSelectDataSecurity = null;
		}
		public override void Logoff() {
			base.Logoff();
			currentSelectDataSecurity = null;
		}
		public virtual bool IsGranted(IPermissionRequest permissionRequest) {
			Guard.ArgumentNotNull(permissionRequest, "permissionRequest");		   
			if(currentSelectDataSecurity == null) { 
				currentSelectDataSecurity = CreateSelectDataSecurity(permissionRequest);
			}
			return currentSelectDataSecurity.IsGranted(permissionRequest);
		}
		public virtual IList<bool> IsGranted(IList<IPermissionRequest> permissionRequests) {
			Guard.ArgumentNotNull(permissionRequests, "permissionRequests");
			if(currentSelectDataSecurity == null) {
				currentSelectDataSecurity = CreateSelectDataSecurity();
			}
			return currentSelectDataSecurity.IsGranted(permissionRequests);
		}
		private ISelectDataSecurity CreateSelectDataSecurity(IPermissionRequest permissionRequest) {
			if(User == null || LogonObjectSpace == null) {
				return new EmptySelectDataSecurity();
			}
			IPermissionDictionary permissionsDictionary;
			if(DelayPermissionDictionaryLoading) {
				permissionsDictionary = new DelayedIPermissionDictionary((IOperationPermissionProvider)User);
			}
			else {
				IEnumerable<IOperationPermission> userPermissions = OperationPermissionProviderHelper.CollectPermissionsRecursive((IOperationPermissionProvider)User);
				permissionsDictionary = new PermissionDictionary(userPermissions);
			}
			IDictionary<Type, IPermissionRequestProcessor> processors = new Dictionary<Type, IPermissionRequestProcessor>();
			processors.Add(typeof(ModelOperationPermissionRequest), new ModelPermissionRequestProcessor(permissionsDictionary));
			IPermissionRequestProcessor serverPermissionRequestProcessor;
			if(traceLevel == TraceLevel.Off) {
				serverPermissionRequestProcessor = new ServerPermissionRequestProcessor(permissionsDictionary);
			}
			else {
				serverPermissionRequestProcessor = new ServerPermissionRequestProcessorLogger(permissionsDictionary, new FilterLogger(Logger.ConvertToLogLevel(traceLevel), Logger.Instance));
			}
			processors.Add(typeof(ServerPermissionRequest), serverPermissionRequestProcessor);
#pragma warning disable 0618
			processors.Add(typeof(ClientPermissionRequest), new ClientPermissionRequestProcessor(GetObjectSpaces(), processors));
#pragma warning restore 0618           
			CustomizeRequestProcessorsEventArgs args = new CustomizeRequestProcessorsEventArgs(processors, permissionsDictionary);
			if(CustomizeRequestProcessors != null) {
				CustomizeRequestProcessors(this, args);
				processors = args.Processors;
			}
			return new SelectDataSecurity(processors, permissionsDictionary);
		}
		public ISelectDataSecurity CreateSelectDataSecurity() { 
			return CreateSelectDataSecurity(null);
		}
		public override IList<Type> GetBusinessClasses() {
			List<Type> result = new List<Type>(base.GetBusinessClasses());
			if(UserType != null && !result.Contains(UserType)) {
				result.Add(UserType);
			}
			return result;
		}
		public override string UserName {
			get {
				return (User != null) ? ((ISecurityUser)User).UserName : "";
			}
		}
		public override object UserId {
			get {
				return (LogonObjectSpace == null || User == null) ? null : LogonObjectSpace.GetKeyValue(User); 
			}
		}
		[TypeConverter(typeof(BusinessClassTypeConverter<ISecurityUser>))]
		public Type UserType {
			get { return userType; }
			set {
				if(value != null && !typeof(ISecurityUser).IsAssignableFrom(value)) {
					throw new ArgumentException("UserType");
				}
				userType = value;
				if(Authentication != null) {
					Authentication.UserType = userType;
				}
			}
		}
		public static TraceLevel TraceLevel {
			get { return traceLevel; }
			set { traceLevel = value; }
		}
		void IDataServerSecurity.SetLogonParameters(object logonParameters) {
			Guard.ArgumentNotNull(Authentication, "Authentication");
			Authentication.SetLogonParameters(logonParameters);
		}
		public event EventHandler<CustomizeRequestProcessorsEventArgs> CustomizeRequestProcessors;
	}
}
