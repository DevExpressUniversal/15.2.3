#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Data.Services.Providers;
using System.Reflection;
using DevExpress.Xpo.Helpers;
namespace DevExpress.Xpo {
	public class ActionFactory {
		readonly IDataServiceMetadataProvider metadata;
		static readonly Type[] primitives = new[] {
			typeof(bool),
			typeof(short),
			typeof(int),
			typeof(long),
			typeof(string),
			typeof(decimal),
			typeof(Guid),
			typeof(DateTime),
			typeof(char),
			typeof(float),
			typeof(double),
			typeof(bool?),
			typeof(short?),
			typeof(int?),
			typeof(long?),
			typeof(decimal?),
			typeof(Guid?),
			typeof(DateTime?),			
			typeof(float?),
			typeof(double?)
		};
		public static Type[] Primitives { get { return primitives; } }
		public ActionFactory(IDataServiceMetadataProvider metadata) {
			this.metadata = metadata;
		}
		public IEnumerable<ServiceAction> GetActions(Type typeWithActions, string methodName = null) {
			var actionInfos = ActionFinder.GetActionsFromType(typeWithActions);
			foreach (var actionInfo in actionInfos) {
				var method = actionInfo.ActionMethod;
				if(!string.IsNullOrEmpty(methodName) && method.Name != methodName)
					continue;
				string actionName = method.Name;
				ResourceType returnType = OperationHelper.GetResourceType(method.ReturnType, metadata);
				ResourceSet resourceSet = OperationHelper.GetResourceSet(returnType, metadata);
				var parameters = GetParameters(method, actionInfo.Binding != OperationParameterBindingKind.Never);
				ServiceAction action = new ServiceAction(
					actionName,
					returnType,
					resourceSet,
					actionInfo.Binding,
					parameters
				);
				action.CustomState = actionInfo;
				action.SetReadOnly();
				yield return action;
			}
		}
		private ResourceType GetBindingParameterResourceType(Type type) {
			var resourceType = OperationHelper.GetResourceType(type, metadata);
			if (resourceType.ResourceTypeKind == ResourceTypeKind.EntityType) {
				return resourceType;
			} else if (resourceType.ResourceTypeKind == ResourceTypeKind.EntityCollection) {
				if (type.GetGenericTypeDefinition() == typeof(IQueryable<>))
					return resourceType;
			}
			throw new Exception(string.Format("Type {0} is not a valid binding parameter", type.FullName));
		}		
		private IEnumerable<ServiceActionParameter> GetParameters(MethodInfo method, bool isBindable) {
			IEnumerable<ParameterInfo> parameters = method.GetParameters();
			if (isBindable) {
				var bindingParameter = parameters.First();
				yield return new ServiceActionParameter(
						bindingParameter.Name,
						GetBindingParameterResourceType(bindingParameter.ParameterType)
				);
				parameters = parameters.Skip(1);
			}
			foreach (var parameter in parameters) {
				yield return new ServiceActionParameter(
					parameter.Name,
					OperationHelper.GetParameterResourceType(parameter.ParameterType, metadata)
				);
			}
		}
	}
	public static class ActionFinder {
		public static ActionInfo[] GetActionsFromType(Type type) {
			var actionInfos = type
				.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Select(m => new {
					Method = m,
					Attribute = Attribute.GetCustomAttribute(m, typeof(ActionAttribute)) as ActionAttribute
				})
				.Where(u => u.Attribute != null)
				.Select(u => new ActionInfo(
					u.Method,
					u.Attribute.Binding,
					u.Attribute.AvailabilityMethodName)
				).ToArray();
			return actionInfos;
		}
	}
	class OperationHelper {
		public static ResourceType GetResourceType(Type type, IDataServiceMetadataProvider _metadata) {
			if (type == typeof(void)) return null;
			if (type.IsGenericType) {
				var typeDef = type.GetGenericTypeDefinition();
				if (typeDef.GetGenericArguments().Count() == 1) {
					if (typeDef == typeof(IEnumerable<>) || typeDef == typeof(IQueryable<>)) {
						var elementResource = GetResourceType(type.GetGenericArguments().Single(), _metadata);
						if ((elementResource.ResourceTypeKind | ResourceTypeKind.EntityType) == ResourceTypeKind.EntityType)
							return ResourceType.GetEntityCollectionResourceType(elementResource);
						else
							return ResourceType.GetCollectionResourceType(elementResource);
					}
				}
				throw new Exception(string.Format("Generic action parameter type {0} not supported", type.ToString()));
			}
			if (ActionFactory.Primitives.Contains(type))
				return ResourceType.GetPrimitiveResourceType(type);
			ResourceType resourceType;
			string typeName = TypeSystem.ProcessTypeName(_metadata.ContainerNamespace, type);
			if (!_metadata.TryResolveResourceType(typeName, out resourceType)) throw new Exception(string.Format("Generic action parameter type {0} not supported", type.ToString()));
			return resourceType;
		}
		public static ResourceType GetServiceOperationReturnType(Type type, IDataServiceMetadataProvider _metadata) {			
			if (type.IsGenericType) {
				var typeDef = type.GetGenericTypeDefinition();
				if (typeDef.GetGenericArguments().Count() == 1) {
					if (typeDef == typeof(IEnumerable<>) || typeDef == typeof(IQueryable<>)) {
						return GetResourceType(type.GetGenericArguments().Single(), _metadata);						
					}
				}
				throw new Exception(string.Format("Generic action parameter type {0} not supported", type.ToString()));
			}
			return GetResourceType(type, _metadata);
		}
		public static ResourceSet GetResourceSet(ResourceType type, IDataServiceMetadataProvider _metadata) {
			if (type == null) {
				return null;
			} else if (type.ResourceTypeKind == ResourceTypeKind.EntityCollection) {
				EntityCollectionResourceType ecType = type as EntityCollectionResourceType;
				return GetResourceSet(ecType.ItemType, _metadata);
			} else if (type.ResourceTypeKind == ResourceTypeKind.EntityType) {
				var set = _metadata.ResourceSets.SingleOrDefault(rs => rs.ResourceType == type);
				if (set != null) {
					return set;
				} else if (type.BaseType != null) {
					return GetResourceSet(type.BaseType, _metadata);
				}
			}
			return null;
		}
		public static ResourceType GetParameterResourceType(Type type, IDataServiceMetadataProvider _metadata) {
			var resourceType = OperationHelper.GetResourceType(type, _metadata);
			if (resourceType.ResourceTypeKind == ResourceTypeKind.EntityType) {
				throw new Exception(string.Format("Entity Types ({0}) MUST not be used as non-binding parameters.", type.FullName));
			} else if (resourceType.ResourceTypeKind == ResourceTypeKind.EntityCollection) {
				throw new Exception(string.Format("Entity Type Collections ({0}) MUST not be used as non-binding parameters.", type.FullName));
			} else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryable<>)) {
				throw new Exception("IQueryable<> is not supported for non-binding parameters");
			}
			return resourceType;
		}
		public static ServiceOperationResultKind GetOperationResultKind(Type type, IDataServiceMetadataProvider _metadata) {
			if (type == typeof(void)) return ServiceOperationResultKind.Void;
			if (type.IsGenericType) {
				var typeDef = type.GetGenericTypeDefinition();
				if (typeDef.GetGenericArguments().Count() == 1) {
					if (typeDef == typeof(IEnumerable<>) || typeDef == typeof(IQueryable<>)) {
						var elementResource = OperationHelper.GetResourceType(type.GetGenericArguments().Single(), _metadata);
						if ((elementResource.ResourceTypeKind | ResourceTypeKind.EntityType) == ResourceTypeKind.EntityType)
							return ServiceOperationResultKind.QueryWithMultipleResults;
						else
							return ServiceOperationResultKind.Enumeration;
					}
				}
				throw new Exception(string.Format("Generic action parameter type {0} not supported", type.ToString()));
			}
			if (ActionFactory.Primitives.Contains(type))
				return ServiceOperationResultKind.DirectValue;
			ResourceType resourceType;
			string typeName = TypeSystem.ProcessTypeName(_metadata.ContainerNamespace, type);
			if (!_metadata.TryResolveResourceType(typeName, out resourceType)) throw new Exception(string.Format("Generic action parameter type {0} not supported", type.ToString()));
			return ServiceOperationResultKind.QueryWithSingleResult;
		}
	}
}
