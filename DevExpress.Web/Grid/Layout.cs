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
using System.Text;
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public abstract class GridLayoutItemCollection : LayoutItemCollection {
		public GridLayoutItemCollection() : base() { }
		public GridLayoutItemCollection(IWebControlObject owner) : base(owner) { }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new LayoutItemBase Add(LayoutItemBase item) { return base.Add(item); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new T Add<T>(string caption) where T : LayoutItemBase, new() { return base.Add<T>(caption); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new T Add<T>(string caption, string name) where T : LayoutItemBase, new() { return base.Add<T>(caption, name); }
		public EmptyLayoutItem AddEmptyItem(EmptyLayoutItem emptyLayoutItem) {
			return (EmptyLayoutItem)base.Add(emptyLayoutItem);
		}
		protected ColumnLayoutItem AddColumnItem(ColumnLayoutItem item, string columnName, string caption) {
			item.ColumnName = columnName;
			if(caption != null)
				item.Caption = caption;
			Add(item);
			return item;
		}
	}
	public abstract class GridLayoutGroup : LayoutGroup {
		public GridLayoutGroup() : base() { }
		public GridLayoutGroup(string caption) : base(caption) { }
		protected internal GridLayoutGroup(FormLayoutProperties owner) : base(owner) { }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new LayoutItem FindItemByFieldName(string fieldName) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new Control FindNestedControlByFieldName(string fieldName) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new object GetNestedControlValueByFieldName(string fieldName) { return null; }
		public ColumnLayoutItem FindColumnItem(string Name_ColumnName) {
			return FindColumnItemInternal(Name_ColumnName);
		}
	}
	public abstract class GridTabbedLayoutGroup : TabbedLayoutGroup {
		public GridTabbedLayoutGroup() : base() { }
		public GridTabbedLayoutGroup(string caption) : base(caption) { }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new LayoutItem FindItemByFieldName(string fieldName) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new Control FindNestedControlByFieldName(string fieldName) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new object GetNestedControlValueByFieldName(string fieldName) { return null; }
		public ColumnLayoutItem FindColumnItem(string Name_ColumnName) {
			return FindColumnItemInternal(Name_ColumnName);
		}
	}
	public abstract class GridFormLayoutProperties : FormLayoutProperties, IFormLayoutOwner {
		const string SameColumnNamesException = "Several layout items with the same ColumnName were found. It is not allowed to associate several items with the same column.";
		public GridFormLayoutProperties(IPropertiesOwner owner) : base(owner) { }
		public GridFormLayoutProperties() : this(null) { }
		public LayoutItemBase FindItemOrGroupByName(string name) {
			return Root.FindItemOrGroupByName(name);
		}
		public ColumnLayoutItem FindColumnItem(string Name_ColumnName) {
			return Root.FindColumnItemInternal(Name_ColumnName);
		}
		protected internal void ValidateLayoutItemColumnNames() {
			var columnNames = new HashSet<string>();
			ForEach(item => {
				ColumnLayoutItem columnLayoutItem = item as ColumnLayoutItem;
				if(columnLayoutItem == null || string.IsNullOrEmpty(columnLayoutItem.ColumnName))
					return;
				if(columnNames.Contains(columnLayoutItem.ColumnName))
					throw new ArgumentException(SameColumnNamesException);
				columnNames.Add(columnLayoutItem.ColumnName);
			});
		}
		string[] IFormLayoutOwner.GetColumnNames() {
			return GetColumnNames();
		}
		object IFormLayoutOwner.FindColumnByName(string columnName) {
			return FindColumnByName(columnName);
		}
		FormLayoutProperties IFormLayoutOwner.GenerateDefaultLayout(bool fromControlDesigner) {
			return GenerateLayout(fromControlDesigner);
		}
		protected virtual string[] GetColumnNames() {
			return DataOwner != null ? DataOwner.GetColumnNames() : null;
		}
		protected virtual object FindColumnByName(string columnName) {
			return DataOwner != null ? DataOwner.FindColumnByName(columnName) : null;
		}
		protected virtual FormLayoutProperties GenerateLayout(bool fromControlDesigner) {
			return DataOwner != null ? DataOwner.GenerateDefaultLayout(fromControlDesigner) : null;
		}
	}
}
