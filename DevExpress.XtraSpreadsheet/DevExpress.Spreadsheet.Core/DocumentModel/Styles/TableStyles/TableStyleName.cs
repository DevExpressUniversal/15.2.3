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
using DevExpress.Office.Utils;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public struct TableStyleName {
		#region Helper class
		class TableStyleNameEqualityComparer : IEqualityComparer<TableStyleName> {
			public bool Equals(TableStyleName x, TableStyleName y) {
				return x.IsEquals(y);
			}
			public int GetHashCode(TableStyleName obj) {
				return obj.GetHashCode();
			}
		}
		#endregion
		#region Static Members
		internal static bool CompareStrings(string x, string y) {
			return StringExtensions.CompareInvariantCultureIgnoreCase(x, y) == 0;
		}
		internal static IEqualityComparer<TableStyleName> Comparer { get { return new TableStyleNameEqualityComparer(); } }
		internal static TableStyleName DefaultStyleName = CreateDefaultStyleName();
		static TableStyleName CreateDefaultStyleName() {
			TableStyleName result = new TableStyleName();
			result.id = DefaultStyleId;
			result.name = DefaultStyleNameString;
			return result;
		}
		internal static Dictionary<string, int> PredefinedPivotNamesTable = CreatePredefinedPivotNamesTable();
		static Dictionary<string, int> CreatePredefinedPivotNamesTable() {
			Dictionary<string, int> result = new Dictionary<string, int>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			Array values = Enum.GetValues(typeof(PredefinedPivotStyleId));
			foreach (PredefinedPivotStyleId id in values)
				result.Add(GetName(id), (int)id);
			return result;
		}
		internal static Dictionary<string, int> PredefinedTableNamesTable = CreatePredefinedTableNamesTable();
		static Dictionary<string, int> CreatePredefinedTableNamesTable() {
			Dictionary<string, int> result = new Dictionary<string, int>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			Array values = Enum.GetValues(typeof(PredefinedTableStyleId));
			foreach (PredefinedTableStyleId id in values)
				result.Add(GetName(id), (int)id);
			return result;
		}
		public static TableStyleName CreatePivotName(string name) {
			if (DefaultStyleName.IsEquals(name))
				return DefaultStyleName;
			TableStyleName result = new TableStyleName();
			result.name = name;
			if (PredefinedPivotNamesTable.ContainsKey(name))
				result.id = (int)PredefinedPivotNamesTable[name];
			return result;
		}
		public static TableStyleName CreateTableName(string name) {
			if (DefaultStyleName.IsEquals(name))
				return DefaultStyleName;
			TableStyleName result = new TableStyleName();
			result.name = name;
			if (PredefinedTableNamesTable.ContainsKey(name))
				result.id = (int)PredefinedTableNamesTable[name];
			return result;
		}
		public static TableStyleName CreatePredefined(PredefinedPivotStyleId id) {
			TableStyleName result = new TableStyleName();
			result.id = (int)id;
			result.name = GetName(id);
			return result;
		}
		public static TableStyleName CreatePredefined(PredefinedTableStyleId id) {
			TableStyleName result = new TableStyleName();
			result.id = (int)id;
			result.name = GetName(id);
			return result;
		}
		public static TableStyleName CreateCustom(string name) {
			if (String.IsNullOrEmpty(name) || CheckPredefinedName(name) || CheckDefaultStyleName(name))
				Exceptions.ThrowInternalException();
			TableStyleName result = new TableStyleName();
			result.name = name;
			return result;
		}
		public static bool CheckPredefinedTableStyleName(string name) {
			return PredefinedTableNamesTable.ContainsKey(name);
		}
		public static bool CheckPredefinedPivotStyleName(string name) {
			return PredefinedPivotNamesTable.ContainsKey(name);
		}
		public static bool CheckDefaultStyleName(TableStyleName name) {
			return name.id.HasValue && CheckDefaultStyleId(name.id.Value) && CheckDefaultStyleName(name.name);
		}
		public static bool CheckDefaultStyleId(int id) {
			return id == DefaultStyleId;
		}
		public static bool CheckDefaultStyleName(string name) {
			return CompareStrings(DefaultStyleNameString, name);
		}
		public static bool CheckPredefinedName(string name) {
			return CheckPredefinedTableStyleName(name) || CheckPredefinedPivotStyleName(name);
		}
		public static string GetName(PredefinedPivotStyleId id) {
			return Enum.GetName(typeof(PredefinedPivotStyleId), id);
		}
		public static string GetName(PredefinedTableStyleId id) {
			return Enum.GetName(typeof(PredefinedTableStyleId), id);
		}
		public static TableStyleCategory GetTableCategory(string styleName) {
			if (!CheckPredefinedName(styleName))
				return TableStyleCategory.Custom;
			int id = (int)PredefinedTableNamesTable[styleName];
			if (id >= 0 && id < 22)
				return TableStyleCategory.Light;
			if (id > 21 && id < 50)
				return TableStyleCategory.Medium;
			return TableStyleCategory.Dark;
		}
		public static TableStyleCategory GetPivotCategory(string styleName) {
			if (!CheckPredefinedName(styleName))
				return TableStyleCategory.Custom;
			int index = ((int)PredefinedPivotNamesTable[styleName] - 1) / 28 + 1;
			return (TableStyleCategory)index;
		}
		#endregion
		#region Fields
		const int DefaultStyleId = 0;
		const string DefaultStyleNameString = "None";
		int? id;
		string name;
		#endregion
		#region Properties
		public int? Id { get { return id; } }
		public string Name { get { return name; } }
		public bool IsPredefined { get { return id.HasValue; } }
		public bool IsDefault { get { return CheckDefaultStyleName(this); } }
		#endregion
		public bool IsEquals(TableStyleName other) {
			return IsEquals(other.name);
		}
		public bool IsEquals(string other) {
			return CompareStrings(name, other);
		}
	}
} 
