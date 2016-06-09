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
using DevExpress.ExpressApp.EF;
using DevExpress.ExpressApp.Security.Adapters;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Security.EF.Adapters {
	public class EFCachedRequestSecurityAdapterProvider : ISecurityAdapterProvider, ISupportCreatePermissions {
		public EFCachedRequestSecurityAdapterProvider() {
		}
		public int GetScopeKey(IObjectSpace objectSpace) {
			EFObjectSpace eFObjectSpace = objectSpace as EFObjectSpace;
			return eFObjectSpace.ObjectContext.GetHashCode();
		}
		public ISecurityAdapter CreateSecurityAdapter() {
			return new EFCachedRequestSecurityAdapter();
		}
		public bool CanHandle(IObjectSpace objectSpace) {
			return objectSpace is EFObjectSpace;
		}
		public IPermissionDictionary CreatePermissionDictionary(IObjectSpace objectSpace, Type userType, object userID) {
			object user = objectSpace.GetObjectByKey(userType, userID);
			if(user == null) {
				return null;
			}
			IEnumerable<IOperationPermission> userPermissions = OperationPermissionProviderHelper.CollectPermissionsRecursive((IOperationPermissionProvider)user);
			return new PermissionDictionary(userPermissions);
		}
	}
	public class EFCachedRequestSecurityAdapter : ISecurityAdapter, ISupportPermissions {
		protected Dictionary<string, bool> requestCache = new Dictionary<string, bool>();
		public IPermissionDictionary PermissionDictionary { get; set; }
		protected object GetRealObject(object targetObject, IObjectSpace objectSpace, out IObjectSpace evaluatorObjectSpace) {
			evaluatorObjectSpace = objectSpace;
			if(objectSpace == null) {
				throw new ArgumentException("objectSpace/objectSpaceHelper should not be null");
			}
			if(objectSpace.IsModified) {
				evaluatorObjectSpace = objectSpace.CreateNestedObjectSpace(); 
				return evaluatorObjectSpace.GetObject(targetObject);
			}
			return targetObject;
		}
		public EFCachedRequestSecurityAdapter() {
		}
		public bool IsGranted(IRequestSecurity requestSecurity, IObjectSpace objectSpace, Type objectType, object targetObject, string memberName, string operation, out bool result) {
			if(targetObject != null && !objectSpace.Contains(targetObject)) {
				result = false;
				return false;
			}
			string passedRequestHashCode = IsGrantedAdapterHelper.GetRequestHashCode(objectSpace, objectType, targetObject, memberName, operation);
			if(!requestCache.TryGetValue(passedRequestHashCode, out result)) {
				IObjectSpace evaluatorObjectSpace = objectSpace;
				object realObject = null;
				if(!objectSpace.IsNewObject(targetObject)) {
					realObject = targetObject;
					if(targetObject != null && !(targetObject is XafDataViewRecord) && objectSpace.IsModified) {
						realObject = GetRealObject(targetObject, objectSpace, out evaluatorObjectSpace);
					}
				}
				ServerPermissionRequest serverPermissionRequest = new ServerPermissionRequest(objectType, realObject, memberName, operation, new SecurityExpressionEvaluator(evaluatorObjectSpace), PermissionDictionary);
				result = requestSecurity.IsGranted(serverPermissionRequest);
				requestCache.Add(passedRequestHashCode, result);
			}
			return true;
		}
	}
}
