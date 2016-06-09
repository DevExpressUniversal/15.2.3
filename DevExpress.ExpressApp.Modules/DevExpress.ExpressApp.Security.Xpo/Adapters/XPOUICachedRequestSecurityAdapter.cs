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
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.Security.Adapters;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
namespace DevExpress.ExpressApp.Security.Xpo.Adapters {
	public class XpoUICachedRequestSecurityAdapterProvider : ISecurityAdapterProvider, ISupportCreatePermissions {
		public XpoUICachedRequestSecurityAdapterProvider() {
		}
		public int GetScopeKey(IObjectSpace objectSpace) {
			XPObjectSpace xPObjectSpace = objectSpace as XPObjectSpace;
			SimpleObjectLayer simpleObjectLayer = null;
			if(xPObjectSpace != null) {
				if(xPObjectSpace.Session.ObjectLayer is SessionObjectLayer && !(xPObjectSpace.Session.ObjectLayer is SecuredSessionObjectLayer)) {
					simpleObjectLayer = ((SessionObjectLayer)xPObjectSpace.Session.ObjectLayer).ParentSession.ObjectLayer as SimpleObjectLayer;
				}
				else {
					simpleObjectLayer = xPObjectSpace.Session.ObjectLayer as SimpleObjectLayer;
				}
			}
			return simpleObjectLayer.GetHashCode();
		}
		public ISecurityAdapter CreateSecurityAdapter() {
			return new XPOUICachedRequestSecurityAdapter();
		}
		public bool CanHandle(IObjectSpace objectSpace) {
			XPObjectSpace xPObjectSpace = objectSpace as XPObjectSpace;
			if(xPObjectSpace != null) {
				if(xPObjectSpace.Session.ObjectLayer is SessionObjectLayer && !(xPObjectSpace.Session.ObjectLayer is SecuredSessionObjectLayer)) {
					if(((SessionObjectLayer)xPObjectSpace.Session.ObjectLayer).ParentSession.ObjectLayer is SimpleObjectLayer) {
						return true;
					}
				}
				if(xPObjectSpace.Session.ObjectLayer is SimpleObjectLayer) {
					return true;
				}
			}
			return false;
		}
		#region ISupportCreatePermissions Members
		public IPermissionDictionary CreatePermissionDictionary(IObjectSpace objectSpace, Type userType, object userID) {
			object user = objectSpace.GetObjectByKey(userType, userID);
			if(user == null) {
				return null;
			}
			IEnumerable<IOperationPermission> userPermissions = OperationPermissionProviderHelper.CollectPermissionsRecursive((IOperationPermissionProvider)user);
			return new PermissionDictionary(userPermissions);
		}
		#endregion
	}
	public class XPOUICachedRequestSecurityAdapter : ISecurityAdapter, ISupportPermissions {
		private IObjectSpace directObjectSpace = SecuritySystem.LogonObjectSpace;
		private Dictionary<string, bool> requestCache = new Dictionary<string, bool>();
		private object GetRealObject(object targetObject, IObjectSpace objectSpace, out IObjectSpace evaluatorObjectSpace) {
			evaluatorObjectSpace = objectSpace;
			if(objectSpace.IsModified) {
				if(objectSpace.IsNewObject(targetObject)) {
					return null;
				}
				CreateUnitOfWorkHandler createUnitOfWorkHandler = () => new UnitOfWork(((XPObjectSpace)objectSpace).Session.ObjectLayer);
				XPObjectSpace XPObjectSpaceHelper = new XPObjectSpace(objectSpace.TypesInfo, XpoTypesInfoHelper.GetXpoTypeInfoSource(), createUnitOfWorkHandler);				
				evaluatorObjectSpace = XPObjectSpaceHelper;
				return evaluatorObjectSpace.GetObject(targetObject);
			}
			return targetObject;
		}
		public XPOUICachedRequestSecurityAdapter() {
		}
		public IPermissionDictionary PermissionDictionary { get; set; }
		public bool IsGranted(IRequestSecurity requestSecurity, IObjectSpace objectSpace, Type objectType, object targetObject, string memberName, string operation, out bool result) {
			if(targetObject != null && !objectSpace.Contains(targetObject)) {
				result = false;
				return false;
			}
			string passedRequestHashCode = IsGrantedAdapterHelper.GetRequestHashCode(objectSpace, objectType, targetObject, memberName, operation);
			if(!requestCache.TryGetValue(passedRequestHashCode, out result)) {
				IObjectSpace evaluatorObjectSpace = objectSpace;
				object realObject = objectSpace.IsNewObject(targetObject) ? null : targetObject;
				if(realObject != null && !(realObject is XafDataViewRecord)) {
					realObject = GetRealObject(targetObject, objectSpace, out evaluatorObjectSpace);
				}
				ServerPermissionRequest serverPermissionRequest = new ServerPermissionRequest(objectType, realObject, memberName, operation, new SecurityExpressionEvaluator(objectSpace), PermissionDictionary);
				result = requestSecurity.IsGranted(serverPermissionRequest);
				requestCache.Add(passedRequestHashCode, result);
			}
			return true;
		}
		public void CreateObjectSpace(XPObjectSpace objectSpace) {
		}
	}
}
