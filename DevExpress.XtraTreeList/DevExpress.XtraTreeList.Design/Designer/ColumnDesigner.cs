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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Design;
using DevExpress.XtraEditors.Frames;
namespace DevExpress.XtraTreeList.Frames {
	[ToolboxItem(false)]
	public class ColumnDesigner : DevExpress.XtraEditors.Frames.ColumnDesignerBase	{
		private System.ComponentModel.IContainer components = null;
		public ColumnDesigner()	{
			InitializeComponent();
		}
		PropertyDescriptor columnDescriptor = null;
		protected override PropertyDescriptor ColumnsDescriptor {
			get {
				if(columnDescriptor == null) {
					columnDescriptor = TypeDescriptor.GetProperties(ColumnsOwner)["Columns"];
				}
				return columnDescriptor;
			}
		}
		public virtual TreeList TreeList { get { return EditingObject as TreeList; } }
		protected override Component ColumnsOwner { get {return TreeList; } }
		protected override CollectionBase Columns { get { return TreeList != null ? TreeList.Columns : null; } }
		protected override ColumnObject CreateColumnObject(object column) { return new TreeListColumnObject(column); }
		protected override void RetrieveColumnsCore() {
			if(TreeList == null) return;
			TreeList.PopulateColumns();
		}
		protected override object CreateNewColumn(string fieldName, int index) {
			if(TreeList == null) return null;
			TreeListColumn column = AddVisibleColumn(TreeList.Columns, fieldName, index);
			column.AbsoluteIndex = index; 
			return column;
		}
		protected override void OnColumnAdded(object column, int visibleIndex) {
			base.OnColumnAdded(column, visibleIndex);
			if(TreeList != null) TreeList.FireChanged(); 
		}
		protected internal static TreeListColumn AddVisibleColumn(TreeListColumnCollection columns, string fieldName, int visibleIndex) {
			TreeListColumn result = columns.AddField(fieldName);
			if(fieldName == "")
				result.Caption = result.Site != null ? result.Site.Name : "NewColumn";
			result.VisibleIndex = visibleIndex;
			return result;
		}
		protected override string[] GetDataFieldList(){
			return DevExpress.XtraTreeList.Design.TypeConverters.FieldNameTypeConverter.GetFieldList(TreeList);
		}
		protected override object GetNestedPropertyGridObject(object obj) {
			TreeListColumn treeListColumn = obj as TreeListColumn;
			if(treeListColumn != null && SelectedTabPage > 0) {
				return treeListColumn.OptionsColumn;
			}
			return obj; 
		}
		protected override string[] GetTabsCaption() { return new string[] {"Column properties", "Column options"}; }
		protected override object InternalGetService(Type type) { 
			if(TreeList == null) return null;
			return TreeList.InternalGetService(type);  
		}
		public override void InitComponent() {
			base.InitComponent();
			if(TreeList != null)
				TreeList.ColumnChanged += new ColumnChangedEventHandler(EditingViewColumnChanged);
		}
		protected override void  DeInitComponent() {
			base.DeInitComponent();
			if(TreeList != null)
				TreeList.ColumnChanged -= new ColumnChangedEventHandler(EditingViewColumnChanged);
		}
		void EditingViewColumnChanged(object sender, ColumnChangedEventArgs e) {
			UpdateColumnData();
		}
		protected override void Dispose( bool disposing ) {
			if( disposing )	{
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Designer generated code
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
	public class TreeListColumnObject : ColumnObject {
		public TreeListColumnObject(object column) : base(column) {}
		public override string FieldName { get { return TreeListColumn.FieldName; } }
		public override string Caption { get { return TreeListColumn.Caption; } }
		public override int AbsoluteIndex { get { return TreeListColumn.AbsoluteIndex; } set { TreeListColumn.AbsoluteIndex = value; } }
		public override bool Visible { get { return TreeListColumn.Visible; } }
		protected TreeListColumn TreeListColumn { get { return Column as TreeListColumn; } }
	}
}
