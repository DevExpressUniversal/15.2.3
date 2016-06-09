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
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using DevExpress.Xpf.PivotGrid.Internal;
namespace DevExpress.Xpf.PivotGrid.Automation {
	public abstract class ScrollableAreaItemAutomationPeer : PivotGridControlVirtualElementAutomationPeerBase, IGridItemProvider, IValueProvider, ISelectionItemProvider {
		protected ScrollableAreaItemAutomationPeer(PivotGridControl pivotGrid, ScrollableAreaCell item)
			: base(pivotGrid) {
			Item = item;
		}
		protected ScrollableAreaCell Item { get; set; }
		protected ScrollableAreaItemBase CellItem { get { return Item.ValueItem; } }
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Value)
				return this;
			if(patternInterface == PatternInterface.SelectionItem)
				return this;
			if(patternInterface == PatternInterface.GridItem)
				return this;
			return null;
		}
		protected override string GetLocalizedControlTypeCore() {
			return string.Empty;
		}
		protected override System.Collections.Generic.List<AutomationPeer> GetChildrenCore() {
			return new List<AutomationPeer>();
		}
		protected override System.Windows.FrameworkElement GetFrameworkElement() {
			return Item;
		}
		#region IGridItemProvider Members
		public int Column {
			get { return CellItem.MinIndex; }
		}
		public int ColumnSpan {
			get { return CellItem.MaxIndex - CellItem.MinIndex + 1; }
		}
		public IRawElementProviderSimple ContainingGrid {
			get { return null; } 
		}
		public int Row {
			get { return CellItem.MinLevel; }
		}
		public int RowSpan {
			get { return CellItem.MaxLevel - CellItem.MinLevel + 1; }
		}
		#endregion
		#region IValueProvider Members
		public bool IsReadOnly {
			get { return true; }
		}
		public void SetValue(string value) { }
		public string Value {
			get { return CellItem.DisplayText; }
		}
		#endregion
		#region ISelectionItemProvider Members
		public bool IsSelected {
			get { return CellItem.IsSelected; }
		}
		public IRawElementProviderSimple SelectionContainer {
			get { return ContainingGrid; }
		}
		public abstract void AddToSelection();
		public abstract void RemoveFromSelection();
		public abstract void Select();
		#endregion
	}
}
