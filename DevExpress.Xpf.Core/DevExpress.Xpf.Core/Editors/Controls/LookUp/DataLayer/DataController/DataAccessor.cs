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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using DevExpress.Data.Access;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Editors.Native;
namespace DevExpress.Xpf.Editors.Helpers {
	public class DataAccessor {
		[ThreadStatic]
		static ReflectionHelper helper;
		public static ReflectionHelper Helper { get { return helper ?? (helper = new ReflectionHelper()); } }
		readonly Dictionary<string, PropertyDescriptor> descriptors = new Dictionary<string, PropertyDescriptor>();
		readonly List<PropertyDescriptor> searchDescriptors = new List<PropertyDescriptor>();
		public IEnumerable<PropertyDescriptor> Descriptors { get { return searchDescriptors; } }
		public string ValueMember { get; private set; }
		public bool HasValueMember { get; private set; }
		public bool HasDisplayMember { get; private set; }
		public string DisplayMember { get; private set; }
		public string DisplayPropertyName { get; private set; }
		public string ValuePropertyName { get; private set; }
		public Type ElementType { get; private set; }
		readonly Locker endInitLocker = new Locker();
		Func<object, object> getValueHandler;
		Func<object, object> getDisplayValueHandler;
		Action<object, object>[] setHandlers;
		Func<object, object>[] getHandlers;
		Func<object, object>[] getFromSourceHandlers;
		public DataAccessor() {
		}
		public void BeginInit() {
			endInitLocker.Lock();
		}
		public void EndInit() {
			endInitLocker.Unlock();
			var descr = descriptors.Values.Select(CalcGetValueFromDescriptor).ToList();
			ElementType = CreateProxyType(descr);
			InitializeGetValueHandlers(descr);
			InitializeSetValueHandlers(descr);
			InitializeGetDisplayAndValueHandlers();
			InitializeSearchDescriptors(descr);
			InitializeGetFromSourceHandlers(descr);
		}
		void InitializeGetFromSourceHandlers(List<PropertyDescriptor> coll) {
			getFromSourceHandlers = new Func<object, object>[coll.Count];
			for (int i = 0; i < coll.Count; i++)
				getFromSourceHandlers[i] = FilterDBNull(coll[i].GetValue);
		}
		Func<object, object> FilterDBNull(Func<object, object> func) {
			return x => {
				object result = func(x);
				return result == DBNull.Value ? null : result;
			};
		}
		void InitializeGetDisplayAndValueHandlers() {
			getValueHandler = Helper.GetInstanceMethodHandler<Func<object, object>>(null, "get_" + ValuePropertyName,
				BindingFlags.Public | BindingFlags.Instance, ElementType);
			getDisplayValueHandler = Helper.GetInstanceMethodHandler<Func<object, object>>(null, "get_" + DisplayPropertyName,
				BindingFlags.Public | BindingFlags.Instance, ElementType);
		}
		void InitializeSearchDescriptors(IList<PropertyDescriptor> coll) {
			searchDescriptors.Clear();
			for (int i = 0; i < coll.Count; i++)
				searchDescriptors.Add(new DataProxySearchDescriptor(coll[i].Name, getHandlers[i], setHandlers[i]));
		}
		void InitializeGetValueHandlers(IList<PropertyDescriptor> coll) {
			getHandlers = new Func<object, object>[descriptors.Count];
			for (int i = 0; i < coll.Count; i++) {
				getHandlers[i] = Helper.GetInstanceMethodHandler<Func<object, object>>(null, "get_" + coll[i].Name, BindingFlags.Public | BindingFlags.Instance, ElementType);
			}
		}
		void InitializeSetValueHandlers(IList<PropertyDescriptor> coll) {
			setHandlers = new Action<object, object>[descriptors.Count];
			for (int i = 0; i < coll.Count; i++) {
				setHandlers[i] = Helper.GetInstanceMethodHandler<Action<object, object>>(null, "set_" + coll[i].Name, BindingFlags.Public | BindingFlags.Instance, ElementType);
			}
		}
		PropertyDescriptor CalcGetValueFromDescriptor(PropertyDescriptor descriptor) {
			return DataListDescriptor.GetFastProperty(descriptor);
		}
		public void GenerateDefaultDescriptors(string valueMember, string displayMember, Func<string, PropertyDescriptor> getDescriptorHandler) {
			if (!endInitLocker.IsLocked)
				throw new ArgumentException("lock");
			ValueMember = valueMember;
			HasValueMember = !string.IsNullOrEmpty(valueMember);
			string valueDescriptorName = HasValueMember ? valueMember : DataControllerData.ValueColumn;
			var vd = getDescriptorHandler(valueDescriptorName) ??
				new LookUpPropertyDescriptor(LookUpPropertyDescriptorType.Value, valueDescriptorName, HasValueMember ? valueDescriptorName : string.Empty);
			descriptors.Add(valueDescriptorName, vd);
			ValuePropertyName = valueDescriptorName;
			HasDisplayMember = !string.IsNullOrEmpty(displayMember);
			string displayDescriptorName = HasDisplayMember ? displayMember : DataControllerData.DisplayColumn;
			if (!descriptors.ContainsKey(displayDescriptorName)) {
				var dd = getDescriptorHandler(displayDescriptorName) ??
					new LookUpPropertyDescriptor(LookUpPropertyDescriptorType.Display, displayDescriptorName, HasDisplayMember ? displayDescriptorName : string.Empty);
				descriptors.Add(displayDescriptorName, dd);
			}
			DisplayMember = displayMember;
			DisplayPropertyName = displayDescriptorName;
		}
		public DataProxy CreateProxy(object component, int listSourceIndex) {
			var proxy = (DataProxy)Activator.CreateInstance(ElementType);
			proxy.f_component = component;
			proxy.f_visibleIndex = listSourceIndex;
			if (component != null) {
				for (int i = 0; i < getFromSourceHandlers.Length; i++) {
					SetValue(proxy, i, getFromSourceHandlers[i](component));
				}
			}
			return proxy;
		}
		void SetValue(DataProxy proxy, int i, object value) {
			var setValueHandler = setHandlers[i];
			setValueHandler(proxy, value);
		}
		public object GetValue(DataProxy proxy) {
			return getValueHandler(proxy);
		}
		public object GetDisplayValue(DataProxy proxy) {
			var value = getDisplayValueHandler(proxy);
			return LookUpPropertyDescriptorBase.UnsetValue == value ? null : value;
		}
		Type CreateProxyType(IEnumerable<PropertyDescriptor> coll) {
			Dictionary<string, Type> dict = descriptors.ToDictionary(pair => pair.Key, pair => GetPropertryTypeFromDescriptor(pair.Value));
			var type = DynamicTypeBuilder.GetDynamicType(dict, typeof(DataProxy), null);
			return type;
		}
		private Type GetPropertryTypeFromDescriptor(PropertyDescriptor desc) {
			bool isDataColumnDescriptor = desc.GetType().FullName == "System.Data.DataColumnPropertyDescriptor";
			return !isDataColumnDescriptor ? desc.PropertyType : typeof(object);
		}
		public void Fetch(string fieldName) {
			if (!endInitLocker.IsLocked)
				throw new ArgumentException("lock");
			PropertyDescriptor pd;
			if (!descriptors.TryGetValue(fieldName, out pd)) {
				pd = new EditorsDataControllerWrappedDescriptor(LookUpPropertyDescriptorType.Value, fieldName, fieldName);
				descriptors.Add(fieldName, pd);
			}
		}
		public void ResetDescriptors() {
			descriptors.Clear();
			ElementType = null;
		}
		public bool IsSame(DataAccessor dataAccessor) {
			return dataAccessor != null && object.Equals(ElementType, dataAccessor.ElementType);
		}
	}
}
