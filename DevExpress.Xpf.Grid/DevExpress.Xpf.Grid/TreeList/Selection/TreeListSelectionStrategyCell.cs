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

using System.Windows;
using System;
using DevExpress.Xpf.Grid.Native;
using System.Windows.Input;
using DevExpress.Xpf.Editors.Helpers;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
using DevExpress.Utils;
#endif
namespace DevExpress.Xpf.Grid.TreeList {
	public class TreeListSelectionStrageryCell : SelectionStrategyCellBase {
		TreeListView TreeListView { get { return DataView as TreeListView; } }
		public TreeListSelectionStrageryCell(TreeListView treeListView)
			: base(treeListView) {
		}
		public override void CopyToClipboard() {
			TreeListView.CopySelectedCellsToClipboard();
		}
		protected override CellBase CreateCell(int rowHandle, ColumnBase column) {
			return ((ITableView)TreeListView).CreateGridCell(rowHandle, column);
		}
	}
	public class TreeListCell : CellBase {
		public TreeListCell(int rowHandle, ColumnBase column) : base(rowHandle, column) { }
		public int RowHandle { get { return RowHandleCore; } }
		public ColumnBase Column { get { return ColumnCore; } }
	}
}
