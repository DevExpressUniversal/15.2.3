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
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Security.ClientServer;
namespace DevExpress.ExpressApp.Security {
	public interface ISelectDataSecurityProvider {
		ISelectDataSecurity CreateSelectDataSecurity();
	}
	public interface ISelectDataSecurityProvider2 {
		ISelectDataSecurity CreateSelectDataSecurityByDictionary(IPermissionDictionary permissionDictionary);
	}
	public interface ISelectDataSecurity : IRequestSecurity, ISecurityCriteriaProvider2 { 
	}
	public interface IReadPermissionVersionProvider {
		Dictionary<string, int> CompleteVersion(Dictionary<string, int> initial);
		bool CompareReadPermissionVersion(List<string> targetTypes, Dictionary<string, int> initial, Dictionary<string, int> current);
	}
	public interface ISecurityCriteriaProvider2 {
		IList<string> GetObjectCriteria(Type type);
		IList<string> GetMemberCriteria(Type type, string memberName);
	}
	public interface ISecurityCriteriaProvider3 {
		IList<string> GetObjectCriteria(Type type, IPermissionDictionary permissionDictionary);
		IList<string> GetMemberCriteria(Type type, string memberName, IPermissionDictionary permissionDictionary);
	}
	class EmptySelectDataSecurity : ISelectDataSecurity {
		public IList<string> GetObjectCriteria(Type type) {
			return new string[0];
		}
		public IList<string> GetMemberCriteria(Type type, string memberName) {
			return new string[0];
		}
		public bool IsGranted(IPermissionRequest permissionRequest) {
			return true;
		}
		public IList<bool> IsGranted(IList<IPermissionRequest> permissionRequests) {
			Guard.ArgumentNotNull(permissionRequests, "permissionRequests");
			List<bool> result = new List<bool>(permissionRequests.Count);
			for(int i = 0; i < permissionRequests.Count; i++) {
				result.Add(IsGranted(permissionRequests[i]));
			}
			return result;
		}
	}
	public class SelectDataSecurity : ISelectDataSecurity, IReadPermissionVersionProvider, ISecurityCriteriaProvider3 {
		private IDictionary<Type, IPermissionRequestProcessor> processors;
		private IPermissionDictionary permissions = null;
		private IList<string> GetObjectCriteria(Type type, IPermissionDictionary permissionDictionary) {
			Guard.ArgumentNotNull(permissionDictionary, "permissionDictionary");
			if(permissionDictionary.FindFirst<IsAdministratorPermission>() != null) {
				return new string[0];
			}
			IPermissionRequestProcessor processor = processors[typeof(ServerPermissionRequest)];
			if(processor is ISecurityCriteriaProvider3) {
				return ((ISecurityCriteriaProvider3)processor).GetObjectCriteria(type, permissionDictionary);
			}
			else if(processor is ISecurityCriteriaProvider2) {
				return ((ISecurityCriteriaProvider2)processor).GetObjectCriteria(type);
			}
			return new string[0];
		}
		private IList<string> GetMemberCriteria(Type type, string memberName, IPermissionDictionary permissionDictionary) {
			Guard.ArgumentNotNull(permissionDictionary, "permissionDictionary");
			if(permissionDictionary.FindFirst<IsAdministratorPermission>() != null) {
				return new string[0];
			}
			IPermissionRequestProcessor processor = processors[typeof(ServerPermissionRequest)];
			if(processor is ISecurityCriteriaProvider3) {
				return ((ISecurityCriteriaProvider3)processor).GetMemberCriteria(type, memberName, permissionDictionary);
			}
			else if(processor is ISecurityCriteriaProvider2) {
				return ((ISecurityCriteriaProvider2)processor).GetMemberCriteria(type, memberName);
			}
			return new string[0];
		}
		public SelectDataSecurity(IDictionary<Type, IPermissionRequestProcessor> processors, IPermissionDictionary permissions) {
			Guard.ArgumentNotNull(processors, "processors");
			Guard.ArgumentNotNull(permissions, "permissions"); 
			this.processors = processors;
			this.permissions = permissions;
		}
		public IList<string> GetObjectCriteria(Type type) {
			return GetObjectCriteria(type, permissions);
		}
		public IList<string> GetMemberCriteria(Type type, string memberName) {
			return GetMemberCriteria(type, memberName, permissions);
		}
		public bool IsGranted(IPermissionRequest permissionRequest) {
			Guard.ArgumentNotNull(permissionRequest, "permissionRequest");
			IPermissionRequest realPermissionRequest = null;
			if(permissionRequest is PermissionRequest) {
				PermissionRequest request = (PermissionRequest)permissionRequest;
				realPermissionRequest = new ServerPermissionRequest(request.ObjectType, request.TargetObject, request.MemberName, request.Operation, new SecurityExpressionEvaluator(request.ObjectSpace));
			}
			else {
				if(permissionRequest is SerializablePermissionRequest) {
					SerializablePermissionRequest request = (SerializablePermissionRequest)permissionRequest;
					object targetObject = (string.IsNullOrEmpty(request.TargetObjectHandle)) ? null : SecuritySystem.LogonObjectSpace.GetObjectByHandle(request.TargetObjectHandle);
					realPermissionRequest = new ServerPermissionRequest(request.ObjectType, targetObject, request.MemberName, request.Operation, new SecurityExpressionEvaluator(SecuritySystem.LogonObjectSpace));
				}
				else {
					realPermissionRequest = permissionRequest;
				}
			}
			if(permissions.FindFirst<IsAdministratorPermission>() != null) {
				return true;
			}
			IPermissionRequestProcessor processor = processors[realPermissionRequest.GetType()];
			if(processor != null) {
				return processor.IsGranted(realPermissionRequest);
			}
			return false;
		}
		public IList<bool> IsGranted(IList<IPermissionRequest> permissionRequests) {
			Guard.ArgumentNotNull(permissionRequests, "permissionRequests");
			List<bool> result = new List<bool>(permissionRequests.Count);
			for(int i = 0; i < permissionRequests.Count; i++) {
				result.Add(IsGranted(permissionRequests[i]));
			}
			return result;
		}
		private Dictionary<string, int> readPermissionVersion;
		public Dictionary<string, int> CompleteVersion(Dictionary<string, int> initial) {
			if(readPermissionVersion == null) {
				foreach(IPermissionRequestProcessor processor in processors.Values) {
					IReadPermissionVersionProvider readPermissionVersionProvider = processor as IReadPermissionVersionProvider;
					if(readPermissionVersionProvider != null) {
						readPermissionVersion = readPermissionVersionProvider.CompleteVersion(initial);
					}
				}
			}
			return readPermissionVersion;
		}
		public bool CompareReadPermissionVersion(List<string> targetTypes, Dictionary<string, int> initial, Dictionary<string, int> current) {
			bool result = true;
			foreach(IPermissionRequestProcessor processor in processors.Values) {
				IReadPermissionVersionProvider readPermissionVersionProvider = processor as IReadPermissionVersionProvider;
				if(readPermissionVersionProvider != null) {
					result &= readPermissionVersionProvider.CompareReadPermissionVersion(targetTypes, initial, current);
					if(!result) {
						return false;
					}
				}
			}
			return true;
		}
		#region ISecurityCriteriaProvider3 Members
		IList<string> ISecurityCriteriaProvider3.GetObjectCriteria(Type type, IPermissionDictionary permissionDictionary) {
			return GetObjectCriteria(type, permissionDictionary);
		}
		IList<string> ISecurityCriteriaProvider3.GetMemberCriteria(Type type, string memberName, IPermissionDictionary permissionDictionary) {
			return GetMemberCriteria(type, memberName, permissionDictionary);
		}
		#endregion
	}
}
