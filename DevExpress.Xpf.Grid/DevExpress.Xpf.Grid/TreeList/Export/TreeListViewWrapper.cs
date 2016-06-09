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

using System.Linq;
using DevExpress.XtraExport.Helpers;
using DevExpress.Data.Summary;
using DevExpress.Data;
using DevExpress.Xpf.Data;
using DevExpress.XtraPrinting;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.ConditionalFormatting.Printing;
using DevExpress.Data.Helpers;
using DevExpress.Data.Filtering;
using System.Reflection;
using DevExpress.Data.Linq;
using System.ComponentModel;
using DevExpress.Data.Async;
using DevExpress.Xpf.Core;
using System.Threading;
using DevExpress.Xpf.Grid.Printing;
using System.Collections.Generic;
namespace DevExpress.Xpf.Grid.TreeList {
	public abstract class TreeListViewExportHelperBase : DataViewExportHelperBase<ColumnWrapper, TreeListNodeWrapper> {
		protected TreeListViewExportHelperBase(TreeListView view, ExportTarget target) : base(view, target) {  }
		public override bool ShowGroupedColumns { get { return false; } }
		public override System.Collections.Generic.IEnumerable<ISummaryItemEx> GridGroupSummaryItemCollection { get { return new List<ISummaryItemEx>(); }  }
		public override System.Collections.Generic.IEnumerable<ISummaryItemEx> GroupHeaderSummaryItems { get { return new List<ISummaryItemEx>(); } }
		public override System.Collections.Generic.IEnumerable<ColumnWrapper> GetGroupedColumns() { return null; }
		public override bool GetAllowMerge(ColumnWrapper col) { return false; }
		public new TreeListView View { get { return base.View as TreeListView; } }
		protected override long RowCountCore { get { return View.TreeListDataProvider.VisibleCount; } }
		protected override FormatConditionCollection FormatConditionsCore { get { return View.FormatConditions; } }
		public override System.Collections.Generic.IEnumerable<TreeListNodeWrapper> GetAllRows() {
			return View.Nodes.Where(node => node.IsVisible).Select(node => { return new TreeListNodeWrapper(node); });
		}
	}
	public class TreeListNodeWrapper : IRowBase, IGroupRow<TreeListNodeWrapper>, IClipboardGroupRow<TreeListNodeWrapper> {
		public TreeListNodeWrapper(TreeListNode node){
			Node = node;
		}
		public TreeListNode Node { get; private set; }
		#region IRowBase
		public bool IsDataAreaRow { get { return true; } }
		public int DataSourceRowIndex { get { return Node.Id; } }
		public FormatSettings FormatSettings { get { return null; } }
		public int GetRowLevel() { return Node.Level; }
		public virtual bool IsGroupRow { get { return Node.Nodes.Count > 0; } }
		public int LogicalPosition { get { return Node.RowHandle; } }
		#endregion
		#region IGroupRow
		public IEnumerable<TreeListNodeWrapper> GetAllRows() {
			return Node.Nodes.Where(node => node.IsVisible).Select(node => { return new TreeListNodeWrapper(node); });
		}
		public string GetGroupRowHeader() { return string.Empty; }
		public bool IsCollapsed { get { return !Node.IsExpanded; } }
		#endregion
		#region IClipboardGroupRow
		public IEnumerable<TreeListNodeWrapper> GetSelectedRows() {
			return Node.Nodes.Where(node => node.IsVisible && node.DataProvider.TreeListSelection.GetSelected(node)).Select(node => { return new TreeListNodeWrapper(node); });
		}
		public bool IsTreeListGroupRow() { return true; }
		#endregion
	}
}
