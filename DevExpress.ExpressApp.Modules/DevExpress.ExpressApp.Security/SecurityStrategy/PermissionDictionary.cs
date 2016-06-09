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
using System.Collections;
using System.Collections.Generic;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Security {
	public enum CompareTypeMode { Equals, IsAssignable }
	public interface IPermissionDictionary { 
		T FindFirst<T>() where T : IOperationPermission;
		IEnumerable<T> GetPermissions<T>() where T : IOperationPermission;
		T FindFirst<T>(Type targetObjectType, string operation) where T : ITypeOperationPermission;
		IEnumerable<T> GetPermissions<T>(Type targetObjectType, string operation) where T : ITypeOperationPermission;
		IEnumerable<T> GetPermissions<T>(Type targetObjectType, string operation, CompareTypeMode compareTypeMode) where T : ITypeOperationPermission;
	}
	public class DelayedIPermissionDictionary : IPermissionDictionary {
		private IOperationPermissionProvider user;
		private PermissionDictionary innerDictionary;
		private PermissionDictionary InnerDictionary {
			get {
				if(innerDictionary == null) {
					IEnumerable<IOperationPermission> userPermissions = OperationPermissionProviderHelper.CollectPermissionsRecursive(user);
					innerDictionary = new PermissionDictionary(userPermissions);
				}
				return innerDictionary;
			}
		}
		public DelayedIPermissionDictionary(IOperationPermissionProvider user) {
			this.user = user;
		}
		#region IPermissionDictionary Members
		public T FindFirst<T>() where T : IOperationPermission {
			return InnerDictionary.FindFirst<T>();
		}
		public IEnumerable<T> GetPermissions<T>() where T : IOperationPermission {
			return InnerDictionary.GetPermissions<T>();
		}
		public T FindFirst<T>(Type targetObjectType, string operation) where T : ITypeOperationPermission {
			return InnerDictionary.FindFirst<T>(targetObjectType, operation);
		}
		public IEnumerable<T> GetPermissions<T>(Type targetObjectType, string operation) where T : ITypeOperationPermission {
			return InnerDictionary.GetPermissions<T>(targetObjectType, operation);
		}
		public IEnumerable<T> GetPermissions<T>(Type targetObjectType, string operation, CompareTypeMode compareTypeMode) where T : ITypeOperationPermission {
			return InnerDictionary.GetPermissions<T>(targetObjectType, operation, compareTypeMode);
		}
		#endregion
	}
	public class PermissionDictionary : IPermissionDictionary {
		private readonly IEnumerable<IOperationPermission> sourcePermissions;
		private readonly IDictionary<Type, IList> operationPermissions;
		private readonly IDictionary<Type, IDictionary<Type, IDictionary<string, IList>>> ownTypeOperationPermissions;
		private readonly IDictionary<Type, IDictionary<Type, IDictionary<string, IList>>> ownAndInheritedTypeOperationPermissions;
		private bool IsOperationPermissionsInitialized<T>() where T : IOperationPermission {
			return operationPermissions.ContainsKey(typeof(T));
		}
		private void InitializeOperationPermissions<T>() where T : IOperationPermission {
			IList permissions = new List<T>();
			operationPermissions.Add(typeof(T), permissions);
			foreach(IOperationPermission permission in sourcePermissions) {
				if(permission is T) {
					permissions.Add(permission);
				}
			}
		}
		private IEnumerable<T> GetOperationPermissions<T>() where T : IOperationPermission {
			return (IEnumerable<T>)operationPermissions[typeof(T)];
		}
		private bool IsTypeOperationPermissionsInitialized<T>() where T : ITypeOperationPermission {
			return ownTypeOperationPermissions.ContainsKey(typeof(T));
		}
		private void InitializeTypeOperationPermissions<T>() where T : ITypeOperationPermission {
			IDictionary<Type, IDictionary<string, IList>> typeOperationPermissionsByTargetType = new Dictionary<Type, IDictionary<string, IList>>();
			ownTypeOperationPermissions.Add(typeof(T), typeOperationPermissionsByTargetType);
			foreach(T permission in GetPermissions<T>()) {
				IDictionary<string, IList> typeOperationPermissionsByOperation;
				if(!typeOperationPermissionsByTargetType.TryGetValue(permission.ObjectType, out typeOperationPermissionsByOperation)) {
					typeOperationPermissionsByOperation = new Dictionary<string, IList>();
					typeOperationPermissionsByTargetType.Add(permission.ObjectType, typeOperationPermissionsByOperation);
				}
				IList permissions;
				if(!typeOperationPermissionsByOperation.TryGetValue(permission.Operation, out permissions)) {
					permissions = new List<T>();
					typeOperationPermissionsByOperation.Add(permission.Operation, permissions);
				}
				permissions.Add(permission);
			}
		}
		private IEnumerable<T> GetTypeOperationPermissions<T>(Type targetType, string operation) where T : ITypeOperationPermission {
			IDictionary<Type, IDictionary<string, IList>> typeOperationPermissionsByTargetType = ownTypeOperationPermissions[typeof(T)];
			IDictionary<string, IList> typeOperationPermissionsByOperation;
			if(typeOperationPermissionsByTargetType.TryGetValue(targetType, out typeOperationPermissionsByOperation)) {
				IList permissions;
				if(typeOperationPermissionsByOperation.TryGetValue(operation, out permissions)) {
					return (IEnumerable<T>)permissions;
				}
			}
			return new T[0];
		}
		private bool IsTypeOperationPermissionsInitializedForIsAssignableCompareTypeMode<T>(Type targetType, string operation) where T : ITypeOperationPermission {
			IDictionary<Type, IDictionary<string, IList>> typeOperationPermissionsByTargetType;
			if(ownAndInheritedTypeOperationPermissions.TryGetValue(typeof(T), out typeOperationPermissionsByTargetType)) {
				IDictionary<string, IList> typeOperationPermissionsByOperation;
				if(typeOperationPermissionsByTargetType.TryGetValue(targetType, out typeOperationPermissionsByOperation)) {
					return typeOperationPermissionsByOperation.ContainsKey(operation);
				}
			}
			return false;
		}
		private void InitializeTypeOperationPermissionsForIsAssignableCompareTypeMode<T>(Type targetType, string operation) where T : ITypeOperationPermission {
			IDictionary<Type, IDictionary<string, IList>> typeOperationPermissionsByTargetType;
			if(!ownAndInheritedTypeOperationPermissions.TryGetValue(typeof(T), out typeOperationPermissionsByTargetType)) {
				typeOperationPermissionsByTargetType = new Dictionary<Type, IDictionary<string, IList>>();
				ownAndInheritedTypeOperationPermissions.Add(typeof(T), typeOperationPermissionsByTargetType);
			}
			IDictionary<string, IList> typeOperationPermissionsByOperation;
			if(!typeOperationPermissionsByTargetType.TryGetValue(targetType, out typeOperationPermissionsByOperation)) {
				typeOperationPermissionsByOperation = new Dictionary<string, IList>();
				typeOperationPermissionsByTargetType.Add(targetType, typeOperationPermissionsByOperation);
			}
			List<T> permissions = new List<T>();
			typeOperationPermissionsByOperation.Add(operation, permissions);
			Type currentType = targetType;
			while(currentType != null) {
				permissions.AddRange(GetPermissions<T>(currentType, operation, CompareTypeMode.Equals));
				currentType = currentType.BaseType;
			}
			foreach(Type implementedInterface in targetType.GetInterfaces()) {
				permissions.AddRange(GetPermissions<T>(implementedInterface, operation, CompareTypeMode.Equals));
			}
		}
		private IEnumerable<T> GetTypeOperationPermissionsForIsAssignableCompareTypeMode<T>(Type targetType, string operation) where T : ITypeOperationPermission {
			return (IEnumerable<T>)ownAndInheritedTypeOperationPermissions[typeof(T)][targetType][operation];
		}
		private T FindFirst<T>(IEnumerable<T> source) {
			using(IEnumerator<T> enumerator = source.GetEnumerator()) {
				if(enumerator.MoveNext()) {
					return enumerator.Current;
				}
			}
			return default(T);
		}
		public PermissionDictionary(IEnumerable<IOperationPermission> permissions) {
			sourcePermissions = permissions;
			operationPermissions = new Dictionary<Type, IList>();
			ownTypeOperationPermissions = new Dictionary<Type, IDictionary<Type, IDictionary<string, IList>>>();
			ownAndInheritedTypeOperationPermissions = new Dictionary<Type, IDictionary<Type, IDictionary<string, IList>>>();
		}
		public T FindFirst<T>() where T : IOperationPermission {
			IEnumerable<T> items = GetPermissions<T>();
			return FindFirst<T>(items);
		}
		public IEnumerable<T> GetPermissions<T>() where T : IOperationPermission {
			if(!IsOperationPermissionsInitialized<T>()) {
				InitializeOperationPermissions<T>();
			}
			return GetOperationPermissions<T>();
		}
		public T FindFirst<T>(Type targetObjectType, string operation) where T : ITypeOperationPermission {
			IEnumerable<T> items = GetPermissions<T>(targetObjectType, operation);
			return FindFirst<T>(items);
		}
		public IEnumerable<T> GetPermissions<T>(Type targetObjectType, string operation) where T : ITypeOperationPermission {
			return GetPermissions<T>(targetObjectType, operation, CompareTypeMode.Equals);
		}
		public IEnumerable<T> GetPermissions<T>(Type targetObjectType, string operation, CompareTypeMode compareTypeMode) where T : ITypeOperationPermission {
			Guard.ArgumentNotNull(targetObjectType, "targetObjectType");
			Guard.ArgumentNotNull(operation, "operation");
			IEnumerable<T> result;
			if(compareTypeMode == CompareTypeMode.Equals) {
				if(!IsTypeOperationPermissionsInitialized<T>()) {
					InitializeTypeOperationPermissions<T>();
				}
				result = GetTypeOperationPermissions<T>(targetObjectType, operation);
			}
			else {
				if(!IsTypeOperationPermissionsInitializedForIsAssignableCompareTypeMode<T>(targetObjectType, operation)) {
					InitializeTypeOperationPermissionsForIsAssignableCompareTypeMode<T>(targetObjectType, operation);
				}
				result = GetTypeOperationPermissionsForIsAssignableCompareTypeMode<T>(targetObjectType, operation);
			}
			return result;
		}
	}
}
