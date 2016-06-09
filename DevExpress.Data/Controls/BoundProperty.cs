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
using DevExpress.Data;
namespace DevExpress.Data {
	public interface IDataColumnInfoProvider {
		IDataColumnInfo GetInfo(object arguments);
	}
	public interface IBoundProperty {
		string Name { get; }
		string DisplayName { get; }
		Type Type { get; }
		bool HasChildren { get; }
		List<IBoundProperty> Children { get; }
		bool IsAggregate { get; }
		bool IsList { get; }
		IBoundProperty Parent { get; }
	}
	public class BoundPropertyList {
		delegate bool BoundPropertyResolver(IBoundProperty property, string text);
		List<IBoundProperty> properties;
		public BoundPropertyList(List<IBoundProperty> properties) {
			this.properties = properties;
		}
		public IBoundProperty this[string fieldName] {
			get {
				IBoundProperty res = ResolveComplexProperty(fieldName, Properties, ResolveByName);
				return res != null ? res : ResolveComplexProperty(fieldName, ResolveByName);
			}
		}
		public IBoundProperty GetBoundPropertyByDisplayName(string displayName) {
			IBoundProperty res = ResolveComplexProperty(displayName, Properties, ResolveByDisplayName);
			return res != null ? res : ResolveComplexProperty(displayName, ResolveByDisplayName);
		}
		protected List<IBoundProperty> Properties { get { return properties; } }
		IBoundProperty ResolveComplexProperty(string name, BoundPropertyResolver resolver) {
			if(string.IsNullOrEmpty(name)) return null;
			IBoundProperty parent = GetParentProperty(ref name, properties, resolver);
			while(parent != null && !string.IsNullOrEmpty(name)) {
				IBoundProperty child = ResolveComplexProperty(name, parent.Children, resolver);
				if(child != null) return child;
				parent = GetParentProperty(ref name, parent.Children, resolver);
			}
			return null;
		}
		IBoundProperty ResolveComplexProperty(string name, List<IBoundProperty> list, BoundPropertyResolver resolver) {
			if(list == null || string.IsNullOrEmpty(name)) return null;
			IBoundProperty result = FindProperty(name, list, resolver);
			if (result == null && resolver == ResolveByName && name.EndsWith("!")) {
				result = FindProperty(name.TrimEnd('!'), list, resolver);
			}
			return result;
		}
		IBoundProperty FindProperty(string name, List<IBoundProperty> list, BoundPropertyResolver resolver) {
			foreach (IBoundProperty column in list) {
				if (resolver(column, name)) return column;
			}
			return null;
		}
		IBoundProperty GetParentProperty(ref string name, List<IBoundProperty> list, BoundPropertyResolver resolver) {
			string prefix = GetParentFieldName(ref name);
			return ResolveComplexProperty(prefix, list, resolver);
		}
		string GetParentFieldName(ref string name) {
			int pos = name.IndexOf('.');
			if(pos < 0) return null;
			string prefix = name.Substring(0, pos);
			name = name.Substring(pos + 1);
			return prefix;
		}
		protected static bool ResolveByDisplayName(IBoundProperty property, string displayName) {
			if(property == null || string.IsNullOrEmpty(displayName)) return false;
			return displayName.Equals(property.DisplayName, StringComparison.OrdinalIgnoreCase);
		}
		protected static bool ResolveByName(IBoundProperty property, string name) {
			if(property == null || string.IsNullOrEmpty(name)) return false;
			return property.Name == name;
		}
	}
}
namespace DevExpress.XtraEditors.Filtering {
	public static class IBoundPropertyExtension {
		const char ColumnSeparateChar = '.';
		public static string GetFullName(this IBoundProperty self) {
			return self.Parent != null && !self.Parent.IsList ? self.Parent.GetFullName() + ColumnSeparateChar + self.Name : self.Name;
		}
		public static string GetFullNameWithLists(this IBoundProperty self) {
			return self.Parent != null ? self.Parent.GetFullNameWithLists() + ColumnSeparateChar + self.Name : self.Name;
		}
		public static string GetFullDisplayName(this IBoundProperty self) {
			return self.Parent != null && !self.Parent.IsList ? self.Parent.GetFullDisplayName() + ColumnSeparateChar + self.DisplayName : self.DisplayName;
		}
		public static string GetFullDisplayNameWithLists(this IBoundProperty self) {
			return self.Parent != null ? self.Parent.GetFullDisplayNameWithLists() + ColumnSeparateChar + self.DisplayName : self.DisplayName;
		}
		public static int GetLevel(this IBoundProperty self) {
			return self.Parent != null ? self.Parent.GetLevel() + 1 : 0;
		}
	}
}
