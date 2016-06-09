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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Security.Adapters;
namespace DevExpress.ExpressApp.Security.ClientServer {
	public interface IClientInfoProvider {
		IClientInfo CreateClientInfo(); 
	}
	public class ServerSecurityClient : IRequestSecurityStrategy, ISupportLogonEvent, IRoleTypeProvider, IClientInfoProvider, ISupportChangePasswordOption {
		private struct ValueHolder<T> {
			private T value;
			private bool hasValue;
			public bool HasValue {
				get { return hasValue; }
			}
			public T Value {
				get { return HasValue ? value : default(T); }
				set { this.value = value; hasValue = true; }
			}
		}
		private IServerSecurity serverSecurity;
		private IClientInfo clientInfo;
		private IClientInfoFactory clientInfoFactory;
		private ValueHolder<Type> userType;
		private ValueHolder<Type> roleType;
		private bool? needLogonParameters;
		private bool? isLogoffEnabled;
		private ValueHolder<object> logonParameters;
		private ValueHolder<object> user;
		private ValueHolder<string> userName;
		private ValueHolder<object> userId;
		private IObjectSpace logonObjectSpace;
		private bool isAnonymous = true;
		private RequestGrantedCache requestGrantedCache = new RequestGrantedCache();
		[DefaultValue(true)]
		[Browsable(false)]
		public static bool CanUseCache { get; set; }
		public event EventHandler<LoggingOnEventArgs> LoggingOn;
		public event EventHandler<EventArgs> LoggingOff;
		public ServerSecurityClient(IServerSecurity serverSecurity, IClientInfoFactory clientInfoFactory) {
			Guard.ArgumentNotNull(serverSecurity, "serverSecurity");
			Guard.ArgumentNotNull(clientInfoFactory, "clientInfoFactory");
			this.serverSecurity = serverSecurity;
			this.clientInfoFactory = clientInfoFactory;
			this.clientInfo = clientInfoFactory.CreateClientInfo(AnonymousLogonParameters.Instance);
		}
		public ServerSecurityClient(IServerSecurity serverSecurity) : this(serverSecurity, new ClientInfoFactory()) { }
		#region ISecurityStrategyBase Members
		public void Logon(IObjectSpace objectSpace) {
			IClientInfo probeClientInfo = clientInfoFactory.CreateClientInfo(LogonParameters);
			serverSecurity.Logon(probeClientInfo);
			this.clientInfo = probeClientInfo;
			logonObjectSpace = objectSpace; 
			requestGrantedCache.Clear();
			user = new ValueHolder<object>();
			userName = new ValueHolder<string>();
			userId = new ValueHolder<object>();
			isAnonymous = false;
			if(LoggingOn != null) {
				LoggingOnEventArgs loggingOnArgs = new LoggingOnEventArgs(logonObjectSpace, UserType, UserId);
				LoggingOn(this, loggingOnArgs);
			}
		}
		public void Logoff() {
			isAnonymous = true;
			clientInfo = clientInfoFactory.CreateClientInfo(AnonymousLogonParameters.Instance);
			requestGrantedCache.Clear();
			logonParameters = new ValueHolder<object>();
			user = new ValueHolder<object>();
			userName = new ValueHolder<string>();
			userId = new ValueHolder<object>();
			logonObjectSpace = null;
			if(LoggingOff != null) {
				LoggingOff(this, EventArgs.Empty);
			}
		}
		public void ClearSecuredLogonParameters() {
		}
		public void ReloadPermissions() { 
			requestGrantedCache.Clear();
		}
		public bool IsAuthenticated { 
			get { return false; }
		}
		public IObjectSpace LogonObjectSpace {
			get { return logonObjectSpace; }
		}
		public virtual object User {
			get {
				if(isAnonymous) {
					return null;
				}
				if(logonObjectSpace != null && !user.HasValue && UserId != null) {
					user.Value = logonObjectSpace.GetObjectByKey(UserType, UserId);
				}
				if(user.HasValue) {
					return user.Value;
				}
				else {
					return null;
				}
			}
		}
		public virtual string UserName {
			get {
				if(isAnonymous) {
					return null;
				}
				if(!userName.HasValue) {
					userName.Value = serverSecurity.GetUserName(clientInfo);
				}
				return userName.Value;
			}
		}
		public virtual object UserId {
			get {
				if(isAnonymous) {
					return null;
				}
				if(!userId.HasValue) {
					userId.Value = serverSecurity.GetUserId(clientInfo);
				}
				return userId.Value;
			}
		}
		public virtual object LogonParameters {
			get {
				if(!logonParameters.HasValue) {
					logonParameters.Value = serverSecurity.GetLogonParameters(clientInfo);
				}
				return logonParameters.Value;
			}
		}
		public virtual bool NeedLogonParameters {
			get {
				if(!needLogonParameters.HasValue) {
					needLogonParameters = serverSecurity.GetNeedLogonParameters(clientInfo);
				}
				return needLogonParameters.Value;
			}
		}
		public virtual bool IsLogoffEnabled {
			get {
				if(!isLogoffEnabled.HasValue) {
					isLogoffEnabled = serverSecurity.GetIsLogoffEnabled(clientInfo);
				}
				return isLogoffEnabled.Value;
			}
		}
		public virtual Type GetModuleType() {
			return null;
		}
		public virtual IList<Type> GetBusinessClasses() { 
			return new Type[0];
		}
		public virtual Type UserType {
			get {
				if(!userType.HasValue) {
					userType.Value = serverSecurity.GetUserType(clientInfo);
				}
				return userType.Value;
			}
		}
		#endregion
		#region IRequestSecurityStrategy Members
		bool IsGrantedBatch(IPermissionRequest permissionRequest) {
			CustomGetBatchRequestsEventArgs customGetBatchRequestsEventArgs = new CustomGetBatchRequestsEventArgs(permissionRequest);
			if(CustomGetBatchRequests != null) {
				CustomGetBatchRequests(this, customGetBatchRequestsEventArgs);
			}
			if(!customGetBatchRequestsEventArgs.Handled) {
				SerializablePermissionRequest serializablePermissionRequest = permissionRequest as SerializablePermissionRequest;
				if(serializablePermissionRequest != null) {
					List<IPermissionRequest> innerRequestList = new List<IPermissionRequest>();
					foreach(string operation in SecurityOperations.FullAccess.Split(SecurityOperations.Delimiter)) {
						innerRequestList.Add(new SerializablePermissionRequest(
							serializablePermissionRequest.ObjectType, null, serializablePermissionRequest.TargetObjectHandle, operation.Trim()));
					}
					ITypeInfo ti = XafTypesInfo.Instance.FindTypeInfo(serializablePermissionRequest.ObjectType);
					if(ti != null && SecurityStrategy.IsSecuredType(ti.Type)) {
						IList<string> checkedMemberNames = new List<string>();
						foreach(IMemberInfo mi in ti.Members) {
							if(mi.IsVisible && mi.IsProperty && !checkedMemberNames.Contains(mi.Name)) { 
								innerRequestList.Add(new SerializablePermissionRequest(
									serializablePermissionRequest.ObjectType, mi.Name, serializablePermissionRequest.TargetObjectHandle, SecurityOperations.Read));
								innerRequestList.Add(new SerializablePermissionRequest(
									serializablePermissionRequest.ObjectType, mi.Name, serializablePermissionRequest.TargetObjectHandle, SecurityOperations.Write));
								checkedMemberNames.Add(mi.Name);
							}
						}
					}
					customGetBatchRequestsEventArgs.RequestList.AddRange(innerRequestList);
				}
			}
			IList<bool> innerResult = IsGranted(customGetBatchRequestsEventArgs.RequestList);
			return innerResult[0];
		}
		public IList<bool> IsGranted(IList<IPermissionRequest> permissionRequests) {
			Guard.ArgumentNotNull(permissionRequests, "permissionRequests");
			List<bool> result = new List<bool>(permissionRequests.Count);
			for(int i = 0; i < permissionRequests.Count; i++) {
				result.Add(false);
			}
			if(isAnonymous) { 
				for(int i = 0; i < permissionRequests.Count; i++) {
					result[i] = true; 
				}
			}
			else {
				IList<int> clientRequestIndex = new List<int>();
				IList<IPermissionRequest> requestList = new List<IPermissionRequest>();
				for(int i = 0; i < permissionRequests.Count; i++) {
					bool cacheResult;
					if(!CanUseCache && (requestGrantedCache.TryGetValue(permissionRequests[i], out cacheResult))) {
						result[i] = cacheResult;
					}
					else {
						clientRequestIndex.Add(i);
						requestList.Add(permissionRequests[i]);
					}
				}
				if(clientRequestIndex.Count > 0) {
					IList<bool> serverRequestResult = serverSecurity.IsGranted(clientInfo, requestList);
					for(int i = 0; i < serverRequestResult.Count; i++) {
						requestGrantedCache.Set(requestList[i], serverRequestResult[i]);
						result[clientRequestIndex[i]] = serverRequestResult[i];
					}
				}
			}
			return result;
		}
		public bool IsGranted(IPermissionRequest permissionRequest) {
			Guard.ArgumentNotNull(permissionRequest, "permissionRequest");
			IPermissionRequest realPermissionRequest;
			if(permissionRequest is PermissionRequest) {
				PermissionRequest request = (PermissionRequest)permissionRequest;
				string objectHandle = DataManipulationRight.GetTargetObjectHandle(request.ObjectSpace, request.TargetObject);
				realPermissionRequest = new SerializablePermissionRequest(request.ObjectType, request.MemberName, objectHandle, request.Operation);
			}
			else if(permissionRequest is ServerPermissionRequest) {
				ServerPermissionRequest request = (ServerPermissionRequest)permissionRequest;
				string objectHandle = DataManipulationRight.GetTargetObjectHandle(request.ObjectSpace, request.TargetObject);
				realPermissionRequest = new SerializablePermissionRequest(request.ObjectType, request.MemberName, objectHandle, request.Operation);
			}
			else {
				realPermissionRequest = permissionRequest;
			}
			bool cacheResult;
			if(requestGrantedCache.TryGetValue(realPermissionRequest, out cacheResult)) {
				return cacheResult;
			}
			return IsGrantedBatch(realPermissionRequest);
		}
		#endregion
		#region IClientInfoProvider2 Members
		public IClientInfo CreateClientInfo() {
			return clientInfoFactory.CreateClientInfo(clientInfo.LogonParameters);
		}
		#endregion
		#region IRoleTypeProvider Members
		public virtual Type RoleType {
			get {
				if(!roleType.HasValue) {
					roleType.Value = serverSecurity.GetRoleType(clientInfo);
				}
				return roleType.Value;
			}
		}
		#endregion
		#region ISupportChangePasswordOption Members
		bool ISupportChangePasswordOption.IsSupportChangePassword { get { return GetIsSupportChangePassword(); } }
		protected virtual bool GetIsSupportChangePassword() {
			return IsSupportChangePassword;
		}
		[DefaultValue(false)]
		public bool IsSupportChangePassword { get; set; }
		#endregion
		public event EventHandler<CustomGetBatchRequestsEventArgs> CustomGetBatchRequests;
	}
	public class CustomGetBatchRequestsEventArgs : HandledEventArgs {
		private List<IPermissionRequest> requestList = new List<IPermissionRequest>();
		public CustomGetBatchRequestsEventArgs(IPermissionRequest permissionRequest) {
			requestList.Add(permissionRequest);
		}
		public List<IPermissionRequest> RequestList {
			get { return requestList; }
		}
	}
}
