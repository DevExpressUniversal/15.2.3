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
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using DevExpress.Xpf.PivotGrid.Internal;
namespace DevExpress.Xpf.PivotGrid.Automation {
	public class PivotGridAutomationPeer : PivotGridAutomationPeerBase, ITableProvider {
		public PivotGridAutomationPeer(PivotGridControl pivotGrid, FrameworkElement element)
			: base(pivotGrid, element) { }
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> peers = new List<AutomationPeer>();
			if(PivotGrid.ShowFilterHeaders)
				peers.Add(new HeaderAreaAutomationPeer(PivotGrid, FieldArea.FilterArea));
			if(PivotGrid.ShowDataHeaders)
				peers.Add(new HeaderAreaAutomationPeer(PivotGrid, FieldArea.DataArea));
			if(PivotGrid.ShowColumnHeaders)
				peers.Add(new HeaderAreaAutomationPeer(PivotGrid, FieldArea.ColumnArea));
			if(PivotGrid.ShowRowHeaders)
				peers.Add(new HeaderAreaAutomationPeer(PivotGrid, FieldArea.RowArea));
			peers.Add(GetCellsPeer());
			peers.Add(new FieldValuesAutomationPeer(PivotGrid, false));
			peers.Add(new FieldValuesAutomationPeer(PivotGrid, true));
			return peers;
		}
		CellsAutomationPeer GetCellsPeer() {
			return new CellsAutomationPeer(PivotGrid);
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.DataGrid;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Table)
				return this;
			if(patternInterface == PatternInterface.Scroll)
				return GetCellsPeer();
			return base.GetPattern(patternInterface);
		}
		#region IGridProvider Members
		public int ColumnCount {
			get { return PivotGrid.ColumnCount; }
		}
		public int RowCount {
			get { return PivotGrid.RowCount; }
		}
		IRawElementProviderSimple IGridProvider.GetItem(int row, int column) {
			if(row >= RowCount || column >= ColumnCount)
				return null;
			throw new System.NotImplementedException();
		}
		#endregion
		#region ITableProvider Members
		IRawElementProviderSimple[] ITableProvider.GetColumnHeaders() {
			return GetHeaders(true);
		}
		IRawElementProviderSimple[] ITableProvider.GetRowHeaders() {
			return GetHeaders(false);
		}
		IRawElementProviderSimple[] GetHeaders(bool isColumn) {
			List<IRawElementProviderSimple> items = new List<IRawElementProviderSimple>();
			FieldValuesPresenter preseter = isColumn ? PivotGrid.PivotGridScroller.ColumnValues : PivotGrid.PivotGridScroller.RowValues;
			foreach(ScrollableAreaCell cell in preseter.Children)
				if(cell.Visibility == Visibility.Visible)
					ProviderFromPeer(((IAutomationPeerCreator)preseter).CreatePeer(cell));
			return items.ToArray();
		}
		RowOrColumnMajor ITableProvider.RowOrColumnMajor {
			get { return RowOrColumnMajor.Indeterminate; }
		}
		#endregion
	}
}
