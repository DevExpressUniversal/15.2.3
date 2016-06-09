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
using System.Text;
using System.Data.SqlTypes;
using System.Reflection;
namespace DevExpress.XtraReports.UI {
	public class OracleConverter {
		class Nested {
			static Nested() {
			}
			internal static readonly OracleConverter instance = new OracleConverter();
		}
		public static OracleConverter Instance {
			get {
				return Nested.instance;
			}
		}
		static readonly object padlock = new object();
		static readonly string[] names = new string[] {
			"System.Data.OracleClient.OracleNumber",
			"System.Data.OracleClient.OracleDateTime",
			"System.Data.OracleClient.OracleBinary",
			"System.Data.OracleClient.OracleBoolean",
			"System.Data.OracleClient.OracleString",
		};
		Dictionary<string, PropertyInfo> converters = new Dictionary<string,PropertyInfo>();
		MethodInfo method;
		public object Convert(object obj) {
			try {
				if(CanConvert(obj))
					return ConvertCore(obj, GetKey(obj));
			} catch { 
			}
			return obj;
		}
		object ConvertCore(object obj, string key) {
			lock(padlock) {
				PropertyInfo property;
				if(!converters.TryGetValue(key, out property)) {
					property = obj.GetType().GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);
					converters[key] = property;
				}
				try {
					if(property != null)
						return property.GetValue(obj, new object[0]);
				} catch {
					if(key == "System.Data.OracleClient.OracleNumber")
						return ConvertToDouble(obj);
				}
				return obj;
			}
		}
		object ConvertToDouble(object obj) {
			if(method == null) {
				MethodInfo[] methods = obj.GetType().GetMethods(BindingFlags.Static | BindingFlags.Public);
				method = FindMethod(methods, "op_Explicit", typeof(double), obj.GetType());
			}
			return method.Invoke(null, new object[] { obj });
		}
		static MethodInfo FindMethod(MethodInfo[] methods, string name, Type returnType, Type parameterType) {
			foreach(MethodInfo method in methods) {
				if(method.Name != name || method.ReturnType != returnType)
					continue;
				ParameterInfo[] parameters = method.GetParameters();
				if(parameters.Length > 0 && parameters[0].ParameterType == parameterType)
					return method;
			}
			return null;
		}
		static bool CanConvert(object obj) {
			if(obj is INullable && ((INullable)obj).IsNull)
				return false;
			return obj != null && Array.IndexOf(names, GetKey(obj)) >= 0;
		}
		static string GetKey(object obj) {
			return obj.GetType().FullName;
		}
	}
}
