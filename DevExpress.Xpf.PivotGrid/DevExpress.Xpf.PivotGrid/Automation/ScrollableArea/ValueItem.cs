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

using System.Drawing;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.Xpf.PivotGrid.Automation {
	public class ValueItem : ScrollableAreaItemAutomationPeer, IExpandCollapseProvider {
		public ValueItem(PivotGridControl pivotGrid, FieldValueCellElement cell)
			: base(pivotGrid, cell) { }
		protected new PivotFieldValueItem Item { get { return ((FieldValueItem)base.CellItem).Item; } }
		protected override string GetNameCore() {
			return string.Format("{0} FieldValue: {1}, Level: {2}, Index: {3}", Item.IsColumn ? "Column" : "Row", Item.DisplayText, Item.MinLastLevelIndex, Item.Level);
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.ExpandCollapse)
				return this;
			return base.GetPattern(patternInterface);
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.HeaderItem;
		}
		public override void AddToSelection() {
			PivotGrid.VisualItems.AddSelection(GetRect());
		}
		public override void RemoveFromSelection() {
			PivotGrid.VisualItems.SubtractSelection(GetRect());
		}
		protected Rectangle GetRect() {
			return new Rectangle(Item.IsColumn ? Column : 0, Item.IsColumn ? 0 : Row, Item.IsColumn ? 1 : PivotGrid.ColumnCount, Item.IsColumn ? PivotGrid.RowCount : 1);
		}
		public override void Select() {
			PivotGrid.VisualItems.PerformColumnRowSelection(Item);
		}
		#region IExpandCollapseProvider Members
		public void Collapse() {
			if(!Item.AllowExpand || Item.IsCollapsed)
				return;
			ChangeExpanded();
		}
		public void Expand() {
			if(!Item.AllowExpand || !Item.IsCollapsed)
				return;
			ChangeExpanded();
		}
		void ChangeExpanded() {
#if !SL
			PivotGrid.Commands.ChangeFieldValueExpanded.Execute(Item, PivotGrid);
#else
			PivotGrid.Commands.ChangeFieldValueExpanded.Execute(Item);
#endif
		}
		public ExpandCollapseState ExpandCollapseState {
			get {
				if(!Item.AllowExpand)
					return ExpandCollapseState.LeafNode;
				return Item.IsCollapsed ? ExpandCollapseState.Collapsed : ExpandCollapseState.Expanded;
			}
		}
		#endregion
	}
}
