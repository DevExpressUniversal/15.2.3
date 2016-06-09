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

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using DevExpress.Xpf.PivotGrid.Internal;
namespace DevExpress.Xpf.PivotGrid.Automation {
	public class Celltem : ScrollableAreaItemAutomationPeer, ITableItemProvider, IScrollItemProvider, IAutomationPeerCreator {
		public Celltem(PivotGridControl pivotGrid, ScrollableAreaCell cell)
			: base(pivotGrid, cell) { }
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.TableItem)
				return this;
			if(patternInterface == PatternInterface.ScrollItem)
				return this;
			return base.GetPattern(patternInterface);
		}
		protected override string GetNameCore() {
			return string.Format("Cell: {0}, Column: {1}, Row: {2}", CellItem.DisplayText, Column, Row);
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.DataItem;
		}
		#region ITableItemProvider Members
		public IRawElementProviderSimple[] GetColumnHeaderItems() {
			return GetProviders(true);
		}
		public IRawElementProviderSimple[] GetRowHeaderItems() {
			return GetProviders(false);
		}
		IRawElementProviderSimple[] GetProviders(bool IsColumn) {
			List<IRawElementProviderSimple> peers = new List<IRawElementProviderSimple>();
			FieldValuesPresenter presenter = IsColumn ? PivotGrid.PivotGridScroller.ColumnValues : PivotGrid.PivotGridScroller.RowValues;
			int index = IsColumn ? Column : Row;
			foreach(FieldValueCellElement cell in presenter.Children) {
				if(cell.Visibility == System.Windows.Visibility.Visible && cell.Item.MinLastLevelIndex <= index && cell.Item.MaxLastLevelIndex >= index)
					peers.Add(ProviderFromPeer(CreatePeer(cell)));
			}
			return peers.ToArray();
		}
		#endregion
		public override void AddToSelection() {
			PivotGrid.VisualItems.AddSelection(new Rectangle(new Point(Column, Row), new Size(ColumnSpan, RowSpan)));
		}
		public override void RemoveFromSelection() {
			PivotGrid.VisualItems.SubtractSelection(new Rectangle(new Point(Column, Row), new Size(ColumnSpan, RowSpan)));
		}
		public override void Select() {
			PivotGrid.VisualItems.Selection = new Rectangle(new Point(Column, Row), new Size(ColumnSpan, RowSpan));
		}
		#region IScrollItemProvider Members
		public void ScrollIntoView() {
			PivotGrid.MakeCellVisible(new Point(Column, Row));
		}
		#endregion
		#region IAutomationPeerCreator Members
		public AutomationPeer CreatePeer(System.Windows.DependencyObject obj) {
			AutomationPeer peer = PivotGrid.PeerCache.GetPeer(obj);
			if(peer != null) return peer;
			peer = new ValueItem(PivotGrid, obj as FieldValueCellElement);
			PivotGrid.PeerCache.AddPeer(obj, peer, true);
			return peer;
		}
		#endregion
	}
}
