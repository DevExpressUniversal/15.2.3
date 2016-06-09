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
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security.Adapters;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
namespace DevExpress.ExpressApp.Security.Xpo.Adapters {
	public class XpoClientServerCachedRequestSecurityAdapterProvider : ISecurityAdapterProvider {
		public XpoClientServerCachedRequestSecurityAdapterProvider() {
		}
		public ISecurityAdapter CreateSecurityAdapter() {
			return new XPOClientServerCachedRequestSecurityAdapter();
		}
		public int GetScopeKey(IObjectSpace objectSpace) {
			XPObjectSpace xPObjectSpace = objectSpace as XPObjectSpace;
			SerializableObjectLayerClient serializableObjectLayerClient = null;
			if(xPObjectSpace != null) {
				if(xPObjectSpace.Session.ObjectLayer is SessionObjectLayer && !(xPObjectSpace.Session.ObjectLayer is SecuredSessionObjectLayer)) {
					serializableObjectLayerClient = ((SessionObjectLayer)xPObjectSpace.Session.ObjectLayer).ParentSession.ObjectLayer as SerializableObjectLayerClient;
				}
				else {
					serializableObjectLayerClient = xPObjectSpace.Session.ObjectLayer as SerializableObjectLayerClient;
				}
			}
			return serializableObjectLayerClient.GetHashCode();
		}
		public bool CanHandle(IObjectSpace objectSpace) {
			XPObjectSpace xPObjectSpace = objectSpace as XPObjectSpace;
			if(xPObjectSpace != null) {
				if(xPObjectSpace.Session.ObjectLayer is SessionObjectLayer && !(xPObjectSpace.Session.ObjectLayer is SecuredSessionObjectLayer)) {
					if(((SessionObjectLayer)xPObjectSpace.Session.ObjectLayer).ParentSession.ObjectLayer is SerializableObjectLayerClient) {
						return true;
					}
				}
				return xPObjectSpace.Session.ObjectLayer is SerializableObjectLayerClient;
			}
			return false;
		}
	}
	public class XPOClientServerCachedRequestSecurityAdapter : ISecurityAdapter, ISupportExternalRequestCache {
		private Dictionary<string, bool> requestCache = new Dictionary<string, bool>();
		private IList<SerializablePermissionRequest> CreateClientServerPermissionRequests(IObjectSpace objectSpace, Type objectType, object targetObject, string memberName, string operation) {
			string targetObjectHandle = targetObject == null ? null : objectSpace.GetObjectHandle(targetObject);
			SerializablePermissionRequest serializablePermissionRequest = new SerializablePermissionRequest(objectType, memberName, targetObjectHandle, operation);
			List<SerializablePermissionRequest> permissionRequestList = new List<SerializablePermissionRequest>() { serializablePermissionRequest };
			foreach(string securityOperation in SecurityOperations.FullAccess.Split(SecurityOperations.Delimiter)) {
				SerializablePermissionRequest serializablePermissionRequestType = new SerializablePermissionRequest(objectType, null, targetObjectHandle, operation.Trim());
				if(IsGrantedAdapterHelper.GetRequestHashCode(objectType, null, targetObjectHandle, operation.Trim()) != IsGrantedAdapterHelper.GetRequestHashCode(objectType, memberName, targetObjectHandle, operation)) {
					permissionRequestList.Add(serializablePermissionRequestType);
				}
			}
			ITypeInfo ti = XafTypesInfo.Instance.FindTypeInfo(objectType);
			if(ti != null && SecurityStrategy.IsSecuredType(ti.Type)) {
				IList<string> checkedMemberNames = new List<string>();
				foreach(IMemberInfo mi in ti.Members) {
					if(mi.IsVisible && mi.IsProperty && !checkedMemberNames.Contains(mi.Name)) {
						SerializablePermissionRequest serializablePermissionRequestRead = new SerializablePermissionRequest(objectType, memberName, targetObjectHandle, SecurityOperations.Read);
						if(IsGrantedAdapterHelper.GetRequestHashCode(objectType, memberName, targetObjectHandle, operation) != IsGrantedAdapterHelper.GetRequestHashCode(objectType, memberName, targetObjectHandle, SecurityOperations.Read)) {
							permissionRequestList.Add(serializablePermissionRequestRead);
						}
						SerializablePermissionRequest serializablePermissionRequestWrite = new SerializablePermissionRequest(objectType, memberName, targetObjectHandle, SecurityOperations.Write);
						if(IsGrantedAdapterHelper.GetRequestHashCode(objectType, memberName, targetObjectHandle, operation) != IsGrantedAdapterHelper.GetRequestHashCode(objectType, memberName, targetObjectHandle, SecurityOperations.Write)) {
							permissionRequestList.Add(serializablePermissionRequestWrite);
						}
						checkedMemberNames.Add(mi.Name);
					}
				}
			}
			return permissionRequestList;
		}
		public void SetRequestCache(Dictionary<string, bool> requestCache) {
			this.requestCache = requestCache;
		}
		public XPOClientServerCachedRequestSecurityAdapter() {
					}
		public bool IsGranted(IRequestSecurity requestSecurity, IObjectSpace objectSpace, Type objectType, object targetObject, string memberName, string operation, out bool result) {
			result = false;
			object targeRealtObject = targetObject;
			if(targetObject != null) {
				if(objectSpace.IsNewObject(targetObject)) {
					targeRealtObject = null;
				}
				if(!objectSpace.Contains(targetObject)) {
					return false;
				}
			}
		   string passedRequestHashCode = IsGrantedAdapterHelper.GetRequestHashCode(objectSpace, objectType, targeRealtObject, memberName, operation);
			if(!requestCache.TryGetValue(passedRequestHashCode, out result)) {
				IList<SerializablePermissionRequest> permissionRequests = CreateClientServerPermissionRequests(objectSpace, objectType, targeRealtObject, memberName, operation);
				IList<bool> isGrantedResults = requestSecurity.IsGranted(permissionRequests.OfType<IPermissionRequest>().ToList());
				for(int i = 0; i < permissionRequests.Count; i++) {
					string requestHashCode = IsGrantedAdapterHelper.GetRequestHashCode(objectSpace, permissionRequests[i].ObjectType, permissionRequests[i].TargetObjectHandle, permissionRequests[i].MemberName, permissionRequests[i].Operation);
					if(!requestCache.TryGetValue(requestHashCode, out result)) {
						requestCache.Add(requestHashCode, isGrantedResults[i]); 
					}
				}
				result = isGrantedResults[0];
			}
			return true;
		}
	}
}
