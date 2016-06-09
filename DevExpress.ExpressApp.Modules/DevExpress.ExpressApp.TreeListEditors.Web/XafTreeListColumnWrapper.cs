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
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxTreeList;
namespace DevExpress.ExpressApp.TreeListEditors.Web {
	public class XafTreeListColumnWrapper : WebColumnBaseColumnWrapper {
		private TreeListDataColumn column;
		private DevExpress.ExpressApp.Web.Editors.ASPx.IDataColumnInfo treeListDataColumnInfo;
		public XafTreeListColumnWrapper(TreeListDataColumn column, DevExpress.ExpressApp.Web.Editors.ASPx.IDataColumnInfo treeListDataColumnInfo) : base(column) {
			Guard.ArgumentNotNull(column, "column");
			Guard.ArgumentNotNull(treeListDataColumnInfo, "treeListDataColumnInfo");
			this.column = column;
			this.treeListDataColumnInfo = treeListDataColumnInfo;
		}
		public new TreeListDataColumn Column {
			get { return column; }
		}
		public override string Id {
			get { return treeListDataColumnInfo.Model.Id; }
		}
		public override string PropertyName {
			get { return treeListDataColumnInfo.Model.PropertyName; }
		}
		public override int SortIndex {
			get { return column.SortIndex; }
			set { column.SortIndex = value; }
		}
		public override ColumnSortOrder SortOrder {
			get { return column.SortOrder; }
			set { column.SortOrder = value; }
		}
		public override bool AllowSortingChange {
			get { return DefaultBooleanConverter.ConvertToBoolean(column.AllowSort); }
			set { column.AllowSort = DefaultBooleanConverter.ConvertToDefaultBoolean(value); }
		}
		public override int VisibleIndex {
			get {
				if(!column.Visible) {
					return -1;
				}
				return column.VisibleIndex;
			}
			set { column.VisibleIndex = value; }
		}
		public override string Caption {
			get {
				return column.Caption;
			}
			set {
				column.Caption = value;
			}
		}
		public override string ToolTip {
			get {
				return column.ToolTip;
			}
			set {
				column.ToolTip = value;
			}
		}
		public override bool ShowInCustomizationForm {
			get { return column.ShowInCustomizationForm; }
			set { column.ShowInCustomizationForm = value; }
		}
		public override void ApplyModel(IModelColumn columnInfo) {
			base.ApplyModel(columnInfo);
			treeListDataColumnInfo.ApplyModel(column);
		}
		public override void SynchronizeModel() {
			base.SynchronizeModel();
			treeListDataColumnInfo.SynchronizeModel(column);
		}
		public override bool Visible {
			get {
				return column.Visible;
			}
		}
	}
}
