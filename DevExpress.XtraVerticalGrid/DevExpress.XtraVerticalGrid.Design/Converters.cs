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
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraVerticalGrid.Design.Converters {
	public class FieldNameTypeConverter : StringConverter {
		protected virtual VGridControlBase GetGrid(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null) return null;
			if(context.Instance is VGridControlBase) return context.Instance as VGridControlBase;
			BaseRow row = GetRow(context);
			if(row != null) return row.Grid;
			return null;
		}
		protected virtual BaseRow GetRow(ITypeDescriptorContext context) {
			BaseRow row = null;
			if(context.Instance is BaseRow) row = context.Instance as BaseRow;
			if(context.Instance is RowProperties) row = ((RowProperties)context.Instance).Row;
			if(context.Instance is Array) row = (context.Instance as Array).GetValue(0) as BaseRow;
			return row;
		}
		protected virtual StandardValuesCollection GetFieldList(ITypeDescriptorContext context) {
			VGridControlBase grid = GetGrid(context);
			if(grid == null) return null;
			DevExpress.Data.DataColumnInfoCollection columns = grid.InternalGetService(typeof(DevExpress.Data.DataColumnInfoCollection)) as DevExpress.Data.DataColumnInfoCollection;
			if(columns == null || columns.Count == 0) return null;
			string[] fields = new string[columns.Count];
			for(int n = 0; n < columns.Count; n++) {
				fields[n] = columns[n].Name;
			}
			return new StandardValuesCollection(fields);
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return GetFieldList(context);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			BaseRow row = GetRow(context);
			if(row is CategoryRow) return false;
			return GetFieldList(context) != null;
		}
		public override bool IsValid(ITypeDescriptorContext context, object value) {
			return true;
		}
	}
	public sealed class RowEditConverter : ComponentConverter {
		public RowEditConverter(Type t) : base(t) { }
		VGridControlBase GetGrid(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null) return null;
			if(context is VGridControlBase) return (VGridControlBase)context;
			if(context is RowProperties) {
				RowProperties p = context as RowProperties;
				if(p.Row == null) return null;
				return p.Row.Grid;
			}
			return null;
		}
		protected override bool IsValueAllowed(ITypeDescriptorContext context, object value) {
			bool bResult = base.IsValueAllowed(context, value);
			if(!bResult || value == null) return bResult;
			if(value is DevExpress.XtraEditors.Repository.RepositoryItem) {
				DevExpress.XtraEditors.Repository.RepositoryItem edit = value as DevExpress.XtraEditors.Repository.RepositoryItem;
				VGridControlBase grid = GetGrid(context);
				if(grid == null) return false;
				return true;
			}
			return false;
		}
	}
}
namespace DevExpress.XtraVerticalGrid.Design {
	public class DefaultEditEditor : RowEditEditor {
		protected override VGridControlBase GetGrid(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null) return null;
			DefaultEditor editor = context.Instance as DefaultEditor;
			if(editor == null) return null;
			return editor.GetOwner();
		}
	}
	public class RowEditEditor : EditFieldEditor {
		protected virtual BaseRow GetRow(ITypeDescriptorContext context) {
			BaseRow row = null;
			if(context.Instance is BaseRow) row = context.Instance as BaseRow;
			if(context.Instance is RowProperties) row = ((RowProperties)context.Instance).Row;
			if(context.Instance is Array) row = (context.Instance as Array).GetValue(0) as BaseRow;
			return row;
		}
		protected virtual VGridControlBase GetGrid(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null) return null;
			if(context.Instance is VGridControlBase) return context.Instance as VGridControlBase;
			BaseRow row = GetRow(context);
			if(row != null) return row.Grid;
			return null;
		}
		protected override RepositoryItemCollection[] GetRepository(ITypeDescriptorContext context) {
			VGridControlBase grid = GetGrid(context);
			if(grid != null) {
				RepositoryItemCollection[] res = null;
				if(grid.ExternalRepository != null) {
					res = new RepositoryItemCollection[2];
					res[1] = grid.ExternalRepository.Items;
				} else {
					res = new RepositoryItemCollection[1];
				}
				res[0] = grid.RepositoryItems;
				return res;
			}
			return null;
		}
	}
}
