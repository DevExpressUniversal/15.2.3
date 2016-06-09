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
namespace DevExpress.ExpressApp.Utils.Reflection {
	public class TypeHierarchyHelper {
		private readonly HashSet<Type> registeredTypes;
		private readonly Dictionary<Type, Type[]> descendantsByClass;
		private readonly Dictionary<Type, Type[]> implementorsByInterface;
		private readonly Dictionary<Type, Type[]> typesByGenericTypeDefinition;
		public TypeHierarchyHelper() {
			registeredTypes = new HashSet<Type>();
			descendantsByClass = new Dictionary<Type, Type[]>();
			implementorsByInterface = new Dictionary<Type, Type[]>();
			typesByGenericTypeDefinition = new Dictionary<Type, Type[]>();
		}
		public void Register(Type type) {
			Guard.ArgumentNotNull(type, "type");
			if(!registeredTypes.Contains(type)) {
				if(type.IsClass) {
					Type baseType = type.BaseType;
					while(baseType != null) {
						Add(descendantsByClass, baseType, type);
						ProcessGenericTypeDefinition(baseType, type);
						baseType = baseType.BaseType;
					}
				}
				foreach(Type implementedInterface in TypeHelper.GetInterfaces(type)) {
					Add(implementorsByInterface, implementedInterface, type);
					ProcessGenericTypeDefinition(implementedInterface, type);
				}
				ProcessGenericTypeDefinition(type, type);
				registeredTypes.Add(type);
			}
		}
		private void ProcessGenericTypeDefinition(Type genericTypeCandidate, Type specificType) {
			if(genericTypeCandidate.IsGenericType && !genericTypeCandidate.IsGenericTypeDefinition) {
				Type genericTypeDefinition = genericTypeCandidate.GetGenericTypeDefinition();
				Add(typesByGenericTypeDefinition, genericTypeDefinition, specificType);
			}
		}
		private void Add(Dictionary<Type, Type[]> cache, Type forType, Type typeToAdd) {
			Register(forType);
			Type[] array;
			Type[] newArray;
			if(cache.TryGetValue(forType, out array)) {
				newArray = new Type[array.Length + 1];
				array.CopyTo(newArray, 0);
				newArray[array.Length] = typeToAdd;
			}
			else {
				newArray = new Type[] { typeToAdd };
			}
			cache[forType] = newArray;
		}
		public Type[] GetDescendants(Type type) {
			CheckTypeArgument(type);
			Type[] result = Type.EmptyTypes;
			if(type.IsClass && descendantsByClass.ContainsKey(type)) {
				result = descendantsByClass[type];
			}
			return result;
		}
		public Type[] GetImplementors(Type type) {
			CheckTypeArgument(type);
			Type[] result = Type.EmptyTypes;
			if(type.IsInterface && implementorsByInterface.ContainsKey(type)) {
				result = implementorsByInterface[type];
			}
			return result;
		}
		public Type[] GetTypesByGenericTypeDefinition(Type type) {
			CheckTypeArgument(type);
			if(!type.IsGenericTypeDefinition) {
				throw new ArgumentException(string.Format("The {0} type is not generic type definition.", type.FullName), "type");
			}
			Type[] result = Type.EmptyTypes;
			if(type.IsGenericTypeDefinition && typesByGenericTypeDefinition.ContainsKey(type)) {
				result = typesByGenericTypeDefinition[type];
			}
			return result;
		}
		private void CheckTypeArgument(Type type) {
			Guard.ArgumentNotNull(type, "type");
			if(!registeredTypes.Contains(type)) {
				throw new ArgumentException(string.Format("The {0} type is not registered.", type.FullName), "type");
			}
		}
	}
}
