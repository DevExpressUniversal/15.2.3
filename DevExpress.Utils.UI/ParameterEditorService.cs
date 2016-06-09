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
using DevExpress.Utils.UI.Localization;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.Design {
	public class ParameterEditorService : InstanceProvider<Dictionary<Type, string>> {
		#region Fields & Properties
		public const string Guid = "ParameterEditorExtension";
		static readonly Dictionary<Type, string> defaultDictionary;
		static readonly Type defaultType = typeof(object);
		static readonly object padlock = new object();
		static readonly ParameterEditorService service = new ParameterEditorService();
		#endregion
		#region Constructors
		static ParameterEditorService() {
			defaultDictionary = CreateDictionary();
		}
		#endregion
		#region Methods
		public static Dictionary<Type, string> GetParameterTypes(string contextName) {
			lock(padlock) {
				return service.GetTypes(contextName);
			}
		}
		public static void AddParameterType(string contextName, Type type, string displayName) {
			lock(padlock) {
				service.AddType(contextName, type, displayName);
			}
		}
		public static Type GetParameterTypeByDisplayName(string contextName, string name) {
			lock(padlock) {
				return service.NameToType(contextName, name);
			}
		}
		public static string GetDisplayNameByParameterType(string contextName, Type type) {
			lock(padlock) {
				return service.TypeToName(contextName, type);
			}
		}
		static Dictionary<Type, string> CreateDictionary() {
			Dictionary<Type, string> dictionary = new Dictionary<Type, string>();
			dictionary[typeof(String)] = UtilsUILocalizer.GetString(UtilsUIStringId.Parameter_Type_String);
			dictionary[typeof(DateTime)] = UtilsUILocalizer.GetString(UtilsUIStringId.Parameter_Type_DateTime);
			dictionary[typeof(Int16)] = UtilsUILocalizer.GetString(UtilsUIStringId.Parameter_Type_Int16);
			dictionary[typeof(Int32)] = UtilsUILocalizer.GetString(UtilsUIStringId.Parameter_Type_Int32);
			dictionary[typeof(Int64)] = UtilsUILocalizer.GetString(UtilsUIStringId.Parameter_Type_Int64);
			dictionary[typeof(float)] = UtilsUILocalizer.GetString(UtilsUIStringId.Parameter_Type_Float);
			dictionary[typeof(double)] = UtilsUILocalizer.GetString(UtilsUIStringId.Parameter_Type_Double);
			dictionary[typeof(decimal)] = UtilsUILocalizer.GetString(UtilsUIStringId.Parameter_Type_Decimal);
			dictionary[typeof(Boolean)] = UtilsUILocalizer.GetString(UtilsUIStringId.Parameter_Type_Boolean);
			dictionary[typeof(Guid)] = UtilsUILocalizer.GetString(UtilsUIStringId.Parameter_Type_Guid);
			return dictionary;
		}
		static Type ResolveType(string name) {
			try {
				Type result = Type.GetType(name);
				return result ?? defaultType;
			} catch {
				return defaultType;
			}
		}
		Dictionary<Type, string> GetTypes(string contextName) {
			Dictionary<Type, string> dictionary = GetInstance(contextName) ?? defaultDictionary;
			return new Dictionary<Type, string>(dictionary);
		}
		void AddType(string contextName, Type type, string displayName) {
			Dictionary<Type, string> dictionary = GetInstance(contextName);
			if(dictionary == null) {
				dictionary = CreateDictionary();
				SetInstance(contextName, dictionary);
			}
			dictionary[type] = displayName;
		}
		Type NameToType(string contextName, string name) {
			Dictionary<Type, string> dictionary = GetInstance(contextName) ?? defaultDictionary;
			if(dictionary.ContainsValue(name))
				return dictionary.First(x => x.Value == name).Key;
			return ResolveType(name);
		}
		string TypeToName(string contextName, Type type) {
			if(type == null)
				return defaultType.FullName;
			Dictionary<Type, string> dictionary = GetInstance(contextName) ?? defaultDictionary;
			string name;
			return dictionary.TryGetValue(type, out name) ? name : type.FullName;
		}
		#endregion
	}
}
