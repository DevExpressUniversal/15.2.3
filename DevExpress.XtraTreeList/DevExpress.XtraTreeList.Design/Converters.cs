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
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.StyleFormatConditions;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Data;
namespace DevExpress.XtraTreeList.Design.TypeConverters {
	public sealed class ColumnEditConverter : ComponentConverter {
		public ColumnEditConverter(Type t) : base(t) { }
		TreeListColumn GetColumn(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null) return null;
			TreeListColumn col = context.Instance as TreeListColumn;
			if(context.Instance is Array) 
				col = (context.Instance as Array).GetValue(0) as TreeListColumn;
			return col;
		}
		TreeList GetTreeList(ITypeDescriptorContext context) {
			TreeListColumn col = GetColumn(context);
			if(col != null) return col.TreeList;
			if(context == null || context.Instance == null) return null;
			return (context.Instance as TreeList);
		}
		protected override bool IsValueAllowed(ITypeDescriptorContext context, object value) {
			bool bResult = base.IsValueAllowed(context, value);
			if(!bResult || value == null) return bResult;
			if(value is RepositoryItem) {
				RepositoryItem edit = value as RepositoryItem;
				TreeList tl = GetTreeList(context);
				if(tl == null) return false;
				if(tl.ExternalRepository == null) return false;
				return true;
			}
			return false;
		}
	}
	class FieldNameTypeConverter : StringConverter {
		protected virtual TreeList GetTreeList(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null) return null;
			if(context.Instance is TreeList) return context.Instance as TreeList;
			TreeListColumn column = null;
			if(context.Instance is TreeListColumn) column = context.Instance as TreeListColumn;
			if(context.Instance is Array) 
				column = (context.Instance as Array).GetValue(0) as TreeListColumn;
			TreeListColumnDesigner.TreeListColumnDesignerActionList columnAction = context.Instance as TreeListColumnDesigner.TreeListColumnDesignerActionList;
			if(columnAction != null)
				column = columnAction.Component as TreeListColumn;
			if(column != null) {
				return column.TreeList;
			}
			return null;
		}
		internal static string[] GetFieldList(TreeList treelist) {
			if(treelist == null) return null; 
			DataColumnInfoCollection columns = treelist.InternalGetService(typeof(DataColumnInfoCollection)) as DataColumnInfoCollection;
			if(columns == null || columns.Count == 0 || treelist.IsUnboundMode) return null;
			string[] fields = new string[columns.Count];
			for(int n = 0; n < columns.Count; n++) {
				fields[n] = columns[n].ColumnName;
			}
			return fields;
		}
		protected virtual StandardValuesCollection GetFieldList(ITypeDescriptorContext context) {
			TreeList tl = GetTreeList(context);
			string[] fields = GetFieldList(tl);
			if(fields != null)
				return new StandardValuesCollection(fields);
			return null;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return GetFieldList(context);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return GetFieldList(context) != null;
		}
		public override bool IsValid(ITypeDescriptorContext context, object value) {
			return true;
		}
	}
}
namespace DevExpress.XtraTreeList.Design {
	[CLSCompliant(false)]
	public class ColumnEditEditor : EditFieldEditor {
		protected virtual TreeListColumn GetColumn(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null) return null;
			object instance = context.Instance is IDXObjectWrapper ? DXObjectWrapper.GetInstance(context) : context.Instance;
			TreeListColumnDesigner.TreeListColumnDesignerActionList columnAction = instance as TreeListColumnDesigner.TreeListColumnDesignerActionList;
			if(columnAction != null)
				return columnAction.Component as TreeListColumn;
			TreeListColumn column = instance as TreeListColumn;
			if(instance is Array)
				column = (instance as Array).GetValue(0) as TreeListColumn;
			return column;
		}
		protected virtual TreeList GetTreeList(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null) return null;
			object instance = context.Instance is IDXObjectWrapper ? DXObjectWrapper.GetInstance(context) : context.Instance;
			if(instance is TreeList) return instance as TreeList;
			TreeListColumn column = GetColumn(context);
			if(column != null) return column.TreeList;
			return null;
		}
		protected override RepositoryItemCollection[] GetRepository(ITypeDescriptorContext context) {
			TreeList treeList = GetTreeList(context);
			if(treeList != null) {
				RepositoryItemCollection[] res = null;
				if(treeList.ExternalRepository != null) {
					res = new RepositoryItemCollection[2];
					res[1] = treeList.ExternalRepository.Items;
				}
				else {
					res = new RepositoryItemCollection[1];
				}
				res[0] = treeList.RepositoryItems;
				return res;
			}
			return null;
		}
	}
	#region ColumnReferenceConverter
	public class ColumnReferenceConverter : System.ComponentModel.TypeConverter 
	{
		protected virtual string None { get { return "(none)"; } }
		protected virtual string GetColumnName(TreeListColumn column) 
		{
			IComponent comp = column as IComponent;
			if(comp.Site != null)
				return comp.Site.Name;
			return "TreeListColumn" + column.AbsoluteIndex.ToString();
		}
		protected virtual TreeList GetTreeList(ITypeDescriptorContext context) 
		{ 
			object obj1 = DXObjectWrapper.GetInstance(context);
			if (obj1 != null)
			{
				TreeList control1 = obj1 as TreeList;
				if (control1 != null)
				{
					return control1;
				}
				StyleFormatCondition condition1 = obj1 as StyleFormatCondition;
				if (condition1 != null)
				{
					return ((StyleFormatConditionCollection)condition1.Collection).TreeListControl;
				}
				FilterCondition condition2 = obj1 as FilterCondition;
				if(condition2 != null) {
					if((FilterConditionCollection)condition2.Collection == null) return null;
					return ((FilterConditionCollection)condition2.Collection).TreeListControl;
				}
				TreeListFormatRule rule = obj1 as TreeListFormatRule;
				if(rule != null)
					return rule.TreeList; 
			}
			return null;	
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) 
		{
			if(destinationType.Equals(typeof(string))) 
			{
				if(value == null) return None;
				if(value is TreeListColumn) 
				{
					return GetColumnName(value as TreeListColumn);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type type) 
		{
			if(GetTreeList(context) != null) 
			{
				if(type != null && type.Equals(typeof(string))) 
				{
					return true;
				}
			}
			return base.CanConvertFrom(context, type);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) 
		{
			if(value == null) return null;
			if(value is string) 
			{
				string source = value.ToString();
				if(source == None) return null;
				TreeList treeList = GetTreeList(context);
				if(treeList != null) 
				{
					foreach(TreeListColumn col in treeList.Columns) 
					{
						if(source == GetColumnName(col)) return col;
					}
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) 
		{
			if(context == null || context.Instance == null) return null;
			ArrayList array = new ArrayList();
			array.Add(null);
			TreeList treeList = GetTreeList(context);
			if(treeList != null) 
			{
				foreach(TreeListColumn col in treeList.Columns) 
				{
					array.Add(col);
				}
			}
			return new StandardValuesCollection(array);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) 
		{
			return true;
		}
	}
	#endregion ColumnReferenceConverter
}
