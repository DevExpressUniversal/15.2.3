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
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp.DC {
	public abstract class DCInterfaceWithUniqueValue<T> where T : class {
		private readonly Dictionary<Type, T> valueByInterface;
		private readonly Dictionary<T, Type> interfaceByValue;
		protected DCInterfaceWithUniqueValue() {
			valueByInterface = new Dictionary<Type, T>();
			interfaceByValue = new Dictionary<T, Type>();
		}
		private void CheckInterfaceTypeParameter(Type interfaceType) {
			if(interfaceType == null) throw new ArgumentNullException("interfaceType");
			if(!interfaceType.IsInterface) throw new ArgumentException(String.Format("The {0} type is not an interface.", interfaceType.FullName), "interfaceType");
			if(!IsDomainComponent(interfaceType)) throw new ArgumentException(String.Format("The {0} interface is not an domain component.", interfaceType.FullName), "interfaceType");
			if(!IsPersistent(interfaceType)) throw new ArgumentException(String.Format("The {0} interface is non persistent.", interfaceType.FullName), "interfaceType");
		}
		private void CheckValueParameter(T value) {
			if(value == null) throw new ArgumentNullException("value");
		}
		private void CheckParameters(Type interfaceType, T value) {
			T existsValue;
			if(valueByInterface.TryGetValue(interfaceType, out existsValue) && !Object.Equals(value, existsValue)) {
				throw new ArgumentException(String.Format("The {0} interface has already been added.", interfaceType.FullName));
			}
			Type existsInterfaceType;
			if(interfaceByValue.TryGetValue(value, out existsInterfaceType) && interfaceType != existsInterfaceType) {
				throw new ArgumentException("Same value has already been added.", "value");
			}
		}
		protected virtual void OnAdd(Type interfaceType, T value) {
			CheckInterfaceTypeParameter(interfaceType);
			CheckValueParameter(value);
			CheckParameters(interfaceType, value);
		}
		protected virtual void ClearCore() {
			valueByInterface.Clear();
			interfaceByValue.Clear();
		}
		protected Boolean IsDomainComponent(Type interfaceType) {
			return AttributeHelper.GetAttributes<DomainComponentAttribute>(interfaceType, false).Length > 0;
		}
		protected Boolean IsPersistent(Type interfaceType) {
			if(!IsDomainComponent(interfaceType)) {
				return false;
			}
			return AttributeHelper.GetAttributes<NonPersistentDcAttribute>(interfaceType, false).Length == 0;
		}
		protected void AddInternal(Type interfaceType, T value) {
			if(!ContainsInternal(interfaceType, value)) {
				OnAdd(interfaceType, value);
				valueByInterface.Add(interfaceType, value);
				interfaceByValue.Add(value, interfaceType);
			}
		}
		protected Boolean ContainsInternal(Type interfaceType, T value) {
			T existsValue;
			return valueByInterface.TryGetValue(interfaceType, out existsValue) && Object.Equals(value, existsValue);
		}
		protected T[] GetValuesInternal() {
			T[] values = new T[interfaceByValue.Keys.Count];
			interfaceByValue.Keys.CopyTo(values, 0);
			return values;
		}
		protected Boolean ContainsValueInternal(T value) {
			return interfaceByValue.ContainsKey(value);
		}
		protected T GetValueInternal(Type interfaceType) {
			if(!ContainsInterfaceInternal(interfaceType)) throw new ArgumentException(String.Format("Unknown interface {0}.", interfaceType.FullName), "interfaceType");
			return valueByInterface[interfaceType];
		}
		protected Type[] GetInterfacesInternal() {
			Type[] interfaces = new Type[valueByInterface.Keys.Count];
			valueByInterface.Keys.CopyTo(interfaces, 0);
			return interfaces;
		}
		protected Boolean ContainsInterfaceInternal(Type interfaceType) {
			return valueByInterface.ContainsKey(interfaceType);
		}
		protected Type GetInterfaceInternal(T value) {
			if(!ContainsValueInternal(value)) throw new ArgumentException("Unknown value.", "value");
			return interfaceByValue[value];
		}
		public void Clear() {
			ClearCore();
		}
	}
}
