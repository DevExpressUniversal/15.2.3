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
using DevExpress.ExpressApp.Security.Adapters;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
namespace DevExpress.ExpressApp.Security.Xpo.Adapters {
	public class XpoIntegratedCachedRequestSecurityAdapterProvider : ISecurityAdapterProvider, ISupportCreatePermissions {
		public XpoIntegratedCachedRequestSecurityAdapterProvider() {
		}
		public int GetScopeKey(IObjectSpace objectSpace) {
			XPObjectSpace xPObjectSpace = objectSpace as XPObjectSpace;
			SecuredSessionObjectLayer securedSessionObjectLayer = null;
			if(xPObjectSpace != null) {
				if(xPObjectSpace.Session.ObjectLayer is SessionObjectLayer && !(xPObjectSpace.Session.ObjectLayer is SecuredSessionObjectLayer)) {
					securedSessionObjectLayer = ((SessionObjectLayer)xPObjectSpace.Session.ObjectLayer).ParentSession.ObjectLayer as SecuredSessionObjectLayer;
				}
				else {
					securedSessionObjectLayer = xPObjectSpace.Session.ObjectLayer as SecuredSessionObjectLayer;
				}
			}
			return securedSessionObjectLayer.GetHashCode();
		}
		public bool CanHandle(IObjectSpace objectSpace) {
			XPObjectSpace xPObjectSpace = objectSpace as XPObjectSpace;
			if(xPObjectSpace != null) {
				if(xPObjectSpace.Session.ObjectLayer is SessionObjectLayer) {
					if(((SessionObjectLayer)xPObjectSpace.Session.ObjectLayer).ParentSession.ObjectLayer is SecuredSessionObjectLayer) {
						return true;
					}
				}
				return xPObjectSpace.Session.ObjectLayer is SecuredSessionObjectLayer;
			}
			return false;
		}
		public ISecurityAdapter CreateSecurityAdapter() {
			return new XPOIntegratedCachedRequestSecurityAdapter();
		}
		#region ISupportCreatePermissions Members
		private object GetObjectByKeyInParentSession(IObjectSpace objectSpace, Type objectType, object objectKey) {
			DevExpress.Xpo.Session session = ((DevExpress.ExpressApp.Xpo.XPObjectSpace)objectSpace).Session;
			DevExpress.Xpo.Session parentSession = ((DevExpress.Xpo.SessionObjectLayer)session.ObjectLayer).ParentSession;
			return parentSession.GetObjectByKey(objectType, objectKey);
		}
		public IPermissionDictionary CreatePermissionDictionary(IObjectSpace objectSpace, Type userType, object userID) {
			object user = GetObjectByKeyInParentSession(objectSpace, userType, userID);
			if(user == null) {
				return null;
			}
			IEnumerable<IOperationPermission> userPermissions = OperationPermissionProviderHelper.CollectPermissionsRecursive((IOperationPermissionProvider)user);
			return new PermissionDictionary(userPermissions);
		}
		#endregion
	}
	public class XPOIntegratedCachedRequestSecurityAdapter : ISecurityAdapter, ISupportPermissions {   
		private object GetObjectByKeyInParentSession(IObjectSpace objectSpace, Type objectType, object objectKey) {
			DevExpress.Xpo.Session session = ((DevExpress.ExpressApp.Xpo.XPObjectSpace)objectSpace).Session;
			DevExpress.Xpo.Session parentSession = ((DevExpress.Xpo.SessionObjectLayer)session.ObjectLayer).ParentSession;
			return parentSession.GetObjectByKey(objectType, objectKey);
		}
		private Dictionary<string, bool> requestCache = new Dictionary<string, bool>();
		private object GetRealObject(object targetObject, IObjectSpace objectSpace) {
			object keyObject = objectSpace.GetKeyValue(targetObject);
			if(keyObject == null) {
				return targetObject;
			}
			return GetObjectByKeyInParentSession(objectSpace, targetObject.GetType(), keyObject);
		}
		public IPermissionDictionary PermissionDictionary { get; set; }
		public bool IsGranted(IRequestSecurity requestSecurity, IObjectSpace objectSpace, Type objectType, object targetObject, string memberName, string operation, out bool result) {
			Guard.ArgumentNotNull(PermissionDictionary, "permissionDictionary");
			if(targetObject != null && !objectSpace.Contains(targetObject)) {
				result = false;
				return false;
			}
			string passedRequestHashCode = IsGrantedAdapterHelper.GetRequestHashCode(objectSpace, objectType, targetObject, memberName, operation);
			if(!requestCache.TryGetValue(passedRequestHashCode, out result)) {
				object realObject = objectSpace.IsNewObject(targetObject) ? null : targetObject;
				if(realObject != null && !(realObject is XafDataViewRecord)) {
					realObject = (targetObject is XafDataViewRecord) ? targetObject : GetRealObject(targetObject, objectSpace);
				}
				ServerPermissionRequest serverPermissionRequest = new ServerPermissionRequest(objectType, realObject, memberName, operation, new SecurityExpressionEvaluator(objectSpace), PermissionDictionary);
				result = requestSecurity.IsGranted(serverPermissionRequest);
				requestCache.Add(passedRequestHashCode, result);
			}
			return true;
		}
	}
}
