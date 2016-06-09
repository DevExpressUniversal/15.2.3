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
using System.Linq;
using System.Text;
using DevExpress.Xpf.Grid.Native;
using System.Windows;
namespace DevExpress.Xpf.Grid.TreeList {
	public class TreeListViewHitInfo : GridViewHitInfoBase, ITableViewHitInfo {
		internal static TreeListViewHitInfo CalcHitInfo(DependencyObject d, ITableView view) {
			return new TreeListViewHitInfo(DataViewBase.GetStartHitTestObject(d, view.ViewBase), view);
		}
		internal TreeListViewHitInfo(DependencyObject d, ITableView view)
			: base(d, view != null ? view.ViewBase : null) {
		}
		internal static readonly TreeListViewHitInfo Instance = new TreeListViewHitInfo(null, null);
		TreeListViewHitTest? hitTest;
		protected override GridViewHitTestVisitorBase CreateDefaultVisitor() {
			return new TreeListViewHitTestVisitor(this);
		}
		internal override void SetHitTest(TableViewHitTest hitTest) {
			SetTreeListHitTest(ConvertToTreeListViewHitTest(hitTest));
		}
		internal void SetHitTest(TreeListViewHitTest hitTest) {
			SetTreeListHitTest(hitTest);
		}
		internal void SetTreeListHitTest(TreeListViewHitTest hitTest) {
			if(this.hitTest == null)
				this.hitTest = hitTest;
		}
		protected override TableViewHitTest GetTableViewHitTest() {
			return ConvertToTableViewHitTest(HitTest);
		}
		public TreeListViewHitTest HitTest {
			get { return hitTest ?? TreeListViewHitTest.None; }
		}
		public bool InNodeIndent { 
			get {
				return HitInfoInArea(HitTest,
					TreeListViewHitTest.NodeIndent);
			} 
		}
		public bool InNodeExpandButton {
			get {
				return HitInfoInArea(HitTest,
					TreeListViewHitTest.ExpandButton);
			}
		}
		[Obsolete("Use the InNodeImage property instead")]
		public bool InTreeListNodeImage {
			get {
				return InNodeImage;
			}
		}
		public bool InNodeImage {
			get {
				return HitInfoInArea(HitTest,
						TreeListViewHitTest.NodeImage);
			}
		}
		public bool InNodeCheckbox {
			get {
				return HitInfoInArea(HitTest,
					TreeListViewHitTest.NodeCheckbox);
			}
		}
		public bool InRowCell {
			get {
				return HitInfoInArea(HitTest,
					TreeListViewHitTest.RowCell);
			}
		}
		public override bool InRow {
			get {
				return HitInfoInArea(HitTest,
					TreeListViewHitTest.RowCell,
					TreeListViewHitTest.Row,
					TreeListViewHitTest.ExpandButton,
					TreeListViewHitTest.NodeImage,
					TreeListViewHitTest.NodeCheckbox,
					TreeListViewHitTest.NodeIndent,
					TreeListViewHitTest.RowIndicator);
			}
		}
		public new GridColumnBase Column { get { return columnCore as GridColumnBase; } }
		#region ITableViewHitInfo implementation
		bool ITableViewHitInfo.IsRowIndicator { get { return HitTest == TreeListViewHitTest.RowIndicator; } }
		internal override bool IsRowCellCore() {
			return HitTest == TreeListViewHitTest.RowCell;
		}
		#endregion
	}
	public enum TreeListViewHitTest {
		#region common
		None,
		RowCell,
		Row,
		ColumnHeaderPanel,
		ColumnHeader,
		ColumnHeaderFilterButton,
		BandHeaderPanel,
		BandHeader,
		VerticalScrollBar,
		HorizontalScrollBar,
		FilterPanel,
		FilterPanelCloseButton,
		FilterPanelCustomizeButton,
		FilterPanelActiveButton,
		FilterPanelText,
		MRUFilterListComboBox,
		TotalSummaryPanel,
		TotalSummary,
		FixedTotalSummary,
		DataArea,
		#endregion
		ColumnButton,
		BandButton,
		ColumnEdge,
		BandEdge,
		FixedLeftDiv,
		FixedRightDiv,
		RowIndicator,
		NodeIndent,
		ExpandButton,
		NodeImage,
		NodeCheckbox,
	}
}
