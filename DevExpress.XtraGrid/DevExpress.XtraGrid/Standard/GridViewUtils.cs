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
using System.Collections;
using System.Windows.Forms;
using DevExpress.XtraGrid.Registrator;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using System.Drawing.Design;
namespace DevExpress.XtraGrid.Views.Grid {
	public interface IGridDesignTime {
		bool Enabled { get; }
		DragDropEffects DoDragDrop(object obj);
		bool ForceDesignMode { get; set; }
	}
	public interface IGridLookUp {
		void Setup();
		void SetDisplayFilter(string text);
		void Show(object editValue, string filterText);
	}
	[Flags]
	public enum ScrollStyleFlags { None, LiveVertScroll = 1, LiveHorzScroll = 2 };
	public enum DrawFocusRectStyle {None, RowFocus, CellFocus, RowFullFocus};
	public enum GridState {Normal, ColumnSizing, Editing, 
		ColumnDragging, ColumnDown, ColumnFilterDown, RowDetailSizing,
		FilterPanelCloseButtonPressed, ColumnButtonDown , RowSizing, IncrementalSearch, Selection, 
		FilterPanelActiveButtonPressed, FilterPanelTextPressed, FilterPanelMRUButtonPressed, FilterPanelCustomizeButtonPressed,
		CellSelection, Scrolling,
		Unknown};
	public enum RowVisibleState { Hidden, Visible, Partially };
	public enum GroupFooterShowMode { Hidden, VisibleIfExpanded, VisibleAlways };
	public enum NewItemRowPosition { None, Top, Bottom };
	public enum GroupDrawMode { Default, Standard, Office2003, Office };
	[DevExpress.Data.Access.DataPrimitive]
	public class FilterItem {
		public string Text;
		object val;
		public FilterItem(string text, object val) {
			this.val = val;
			this.Text = text;
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("FilterItemValue")]
#endif
		public object Value {
			get { return val; }
		}
		public override string ToString() { return Text; }
	}
	public class GridRowCollection : CollectionBase {
		Hashtable hash;
		public GridRowCollection() {
			hash = new Hashtable();
		}
		public GridRow this[int index] { 
			get { return List[index] as GridRow; }
		}
		public GridRow Add(int rowHandle, int vIndex, int level, int totalHeight, object rowKey, bool forcedRow) {
			GridRow row = new GridRow(rowHandle, vIndex, level, totalHeight, rowKey, forcedRow);
			List.Add(row);
			hash.Add(rowHandle, row);
			return row;
		}
		public GridRow RowByHandle(int rowHandle) {
			return (GridRow)hash[rowHandle];
		}
	}
	public class GridRow {
		public int VisibleIndex, RowHandle, Level;
		int TotalHeight; 
		public object RowKey;
		public bool ForcedRow, NextRowPrimaryChild = true, ForcedRowLight = false;
		public GridRow() : this(0, 0, 0, 0, null, false) { }
		public GridRow(int rowHandle, int visibleIndex, int level, int totalHeight, object rowKey, bool forcedRow) {
			this.ForcedRow = forcedRow;
			this.RowKey = rowKey;
			this.TotalHeight = totalHeight;
			this.VisibleIndex = visibleIndex;
			this.Level = level;
			this.RowHandle = rowHandle;
		}
	}
}
