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
using System.Text.RegularExpressions;
namespace DevExpress.ExpressApp.DC {
	public sealed class EntitiesToGenerateInfo : DCInterfaceWithUniqueValue<String> {
		private const String IdentifierPattern = @"^(_|\p{L}|\p{Nl})(\p{L}|\p{Nl}|\p{Nd}|\p{Pc}|\p{Mn}|\p{Mc}|\p{Cf})*$";
		private readonly Regex identifierRegex;
		private readonly Dictionary<String, Type> baseClassByEntityName;
		private readonly HashSet<Type> sharedPartCache;
		public EntitiesToGenerateInfo() {
			baseClassByEntityName = new Dictionary<String, Type>();
			sharedPartCache = new HashSet<Type>();
			identifierRegex = new Regex(IdentifierPattern);
		}
		private void CheckEntityNameParameter(String entityName) {
			if(String.IsNullOrEmpty(entityName)) throw new ArgumentException("Value cannot be empty.", "entityName");
			if(!IsValidIdentifier(entityName)) throw new ArgumentException(String.Format("'{0}' is not a valid identifier.", entityName), "entityName");
		}
		private Boolean IsValidIdentifier(String value) {
			if(String.IsNullOrEmpty(value)) return false;
			return identifierRegex.IsMatch(value.Normalize());
		}
		private void CheckImplementedInterfaces(Type interfaceType, String entityName) {
			CheckImplementedInterface(interfaceType, entityName);
			foreach(Type implementedInterface in interfaceType.GetInterfaces()) {
				CheckImplementedInterface(implementedInterface, entityName);
			}
		}
		private void CheckImplementedInterface(Type interfaceType, String entityName) {
			if(interfaceType.IsGenericType) {
				throw new ArgumentException(String.Format("Cannot register the '{0}' entity because it implements the '{1}' interface. Entities cannot implement generic interfaces.", entityName, interfaceType.FullName));
			}
		}
		private void CheckBaseClassParameter(Type baseClass) {
			if(baseClass == null) throw new ArgumentNullException("baseClass");
			if(!baseClass.IsClass) throw new ArgumentException(String.Format("The '{0}' type is not a class.", baseClass.FullName));
			if(baseClass.IsSealed) throw new ArgumentException(String.Format("The '{0}' class is sealed.", baseClass.FullName));
		}
		protected override void OnAdd(Type interfaceType, String entityName) {
			base.OnAdd(interfaceType, entityName);
			CheckEntityNameParameter(entityName);
			CheckImplementedInterfaces(interfaceType, entityName);
		}
		protected override void ClearCore() {
			base.ClearCore();
			baseClassByEntityName.Clear();
			sharedPartCache.Clear();
		}
		public void AddEntity(String entityName, Type interfaceType) {
			AddInternal(interfaceType, entityName);
		}
		public String[] GetEntityNames() {
			return GetValuesInternal();
		}
		public Boolean ContainsEntityName(String entityName) {
			return ContainsValueInternal(entityName);
		}
		public String GetEntityName(Type interfaceType) {
			return GetValueInternal(interfaceType);
		}
		public Type[] GetEntityInterfaces() {
			return GetInterfacesInternal();
		}
		public Boolean ContainsEntityInterface(Type interfaceType) {
			return ContainsInterfaceInternal(interfaceType);
		}
		public Type GetEntityInterface(String entityName) {
			return GetInterfaceInternal(entityName);
		}
		public void AddBaseClass(String entityName, Type baseClass) {
			if(!ContainsValueInternal(entityName)) throw new ArgumentException(String.Format("Unknown entityName '{0}'.", entityName), "entityName");
			Type existsBaseClass;
			if(baseClassByEntityName.TryGetValue(entityName, out existsBaseClass)) {
				if(baseClass != existsBaseClass) {
					throw new ArgumentException(String.Format("The '{0}' entity already have other base class.", entityName), "baseClass");
				}
			}
			else {
				CheckBaseClassParameter(baseClass);
				baseClassByEntityName.Add(entityName, baseClass);
			}
		}
		public Boolean HasBaseClass(String entityName) {
			return baseClassByEntityName.ContainsKey(entityName);
		}
		public Type GetBaseClass(String entityName) {
			if(!HasBaseClass(entityName)) throw new ArgumentException(String.Format("The '{0}' entity has no base class.", entityName));
			return baseClassByEntityName[entityName];
		}
		public void AddSharedPart(Type interfaceType) {
			if(!ContainsSharedPart(interfaceType)) {
				if(interfaceType == null) throw new ArgumentNullException("interfaceType");
				if(!interfaceType.IsInterface) throw new ArgumentException(String.Format("The {0} type is not an interface.", interfaceType.FullName), "interfaceType");
				if(!IsDomainComponent(interfaceType)) throw new ArgumentException(String.Format("The {0} interface is not an domain component.", interfaceType.FullName), "interfaceType");
				if(!IsPersistent(interfaceType)) throw new ArgumentException(String.Format("The {0} interface is non persistent.", interfaceType.FullName), "interfaceType");
				sharedPartCache.Add(interfaceType);
			}
		}
		public Boolean ContainsSharedPart(Type interfaceType) {
			return sharedPartCache.Contains(interfaceType);
		}
		public Type[] GetSharedParts() {
			Type[] sharedParts = new Type[sharedPartCache.Count];
			sharedPartCache.CopyTo(sharedParts, 0);
			return sharedParts;
		}
	}
}
