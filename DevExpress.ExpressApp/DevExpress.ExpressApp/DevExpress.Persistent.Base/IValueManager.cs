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
using System.Diagnostics;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.Persistent.Base {
	public interface IValueManagerBase {
		void Clear();
	}
	public interface IValueManager<ValueType> : IValueManagerBase {
		bool CanManageValue { get; }
		ValueType Value { get; set; }
	}
	public class MultiThreadValueManager<ValueType> : IValueManager<ValueType> {
		[ThreadStatic]
		private static ValueType value;
		public ValueType Value {
			get { return value; }
			set { MultiThreadValueManager<ValueType>.value = value; }
		}
		public void Clear() {
			value = default(ValueType);
		}
		public bool CanManageValue {
			get { return true; }
		}
		public MultiThreadValueManager(string key) {
		}
	}
	public class SimpleValueManager<ValueType> : IValueManager<ValueType> {
		private ValueType value;
		public ValueType Value {
			get { return value; }
			set { this.value = value; }
		}
		public void Clear() {
			value = default(ValueType);
		}
		public bool CanManageValue {
			get { return true; }
		}
		public SimpleValueManager(string key) {
		}
	}
	public class ValueManager {
		private static object lockObject = new object();
		private static Type valueManagerType;
		private static bool isValueManagerTypeUsed = false;
		private static Dictionary<Type, Dictionary<string, object>> createdValueManagers = new Dictionary<Type, Dictionary<string, object>>();
		public static IValueManager<ValueType> GetValueManager<ValueType>(string key) {
			object result = null;
			Dictionary<string, object> innerDictionary;
			lock(lockObject) {
				if(!createdValueManagers.TryGetValue(typeof(ValueType), out innerDictionary)) {
					if(innerDictionary == null) {
						result = CreateValueManager<ValueType>(key);
						innerDictionary = new Dictionary<string, object>();
						innerDictionary.Add(key, result);
						createdValueManagers.Add(typeof(ValueType), innerDictionary);
					}
				}
				else {
					if(innerDictionary.ContainsKey(key)) {
						result = innerDictionary[key];
					}
					else {
						result = CreateValueManager<ValueType>(key);
						innerDictionary.Add(key, result);
					}
				}
			}
			return result as IValueManager<ValueType>;
		}
		private static IValueManager<ValueType> CreateValueManager<ValueType>(string key) {
			if(ValueManagerType == null) {
				ValueManagerType = typeof(DevExpress.Persistent.Base.SimpleValueManager<>).GetGenericTypeDefinition();
				Tracing.Tracer.LogText("The ValueManagerType is initialized by the default value: SimpleValueManager.");
				StackTrace st = new StackTrace(true);
				Tracing.Tracer.LogText("Stack trace:\r\n" + st.ToString());
				isValueManagerTypeUsed = true;
			}
			Type type = ValueManagerType.MakeGenericType(new Type[] { typeof(ValueType) });
			IValueManager<ValueType> result = TypeHelper.CreateInstance(type, key) as IValueManager<ValueType>;
			return result;
		}
		public static void Clear() {
			lock(lockObject) {
				foreach(Dictionary<string, object> innerDictionary in createdValueManagers.Values) {
					foreach(object valueManager in innerDictionary.Values) {
						((IValueManagerBase)valueManager).Clear();
					}
				}
				createdValueManagers.Clear();
			}
		}
		public static Type ValueManagerType {
			get {
				return valueManagerType;
			}
			set {
				lock(lockObject) {
					if(valueManagerType == value) {
						return;
					}
					ReflectionHelper.CheckAssemblesConflict(valueManagerType, value, false);
					if(isValueManagerTypeUsed == true) {
						throw new InvalidOperationException(String.Format("Cannot change the ValueManager.ValueManagerType property after it's initialized.\r\nCurrent type: {0}\r\nNew type: {1}\r\n" +
							"This error may occur, for example, in an ASP.NET application when you try to access members of a static class before the ValueManager.ValueManagerType property is initialized by the value, which is specific for ASP.NET applications, in the constructor of the WebApplication class.\r\n" +
							"To avoid this error it's necessary to manually set the ValueManager.ValueManagerType property before accessing the members of a static class in your code. The best way to do this in ASP.NET applications is to put the following code within the Global.Application_Start method:\r\n" +
							"<code lang=\"cs\">ValueManager.ValueManagerType = typeof(ASPSessionValueManager<>).GetGenericTypeDefinition();</code>\r\n" +
							"<code lang=\"vb\">ValueManager.ValueManagerType = GetType(ASPSessionValueManager(Of )).GetGenericTypeDefinition()</code>\r\n" +
							"Please refer to the product's documentation for more detailed information on these classes and methods. If this doesn't help, please contact our Support Team at http://www.devexpress.com/Support/Center/",
							valueManagerType.AssemblyQualifiedName, value.AssemblyQualifiedName));
					}
					valueManagerType = value;
				}
			}
		}
#if DEBUG
		public static void Reset() {
			valueManagerType = null;
			isValueManagerTypeUsed = false;
		}
#endif
	}
}
