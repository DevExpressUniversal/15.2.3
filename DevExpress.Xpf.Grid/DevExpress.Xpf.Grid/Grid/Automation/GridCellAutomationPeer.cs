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

using System.Windows.Automation.Provider;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Collections.Generic;
using System.Windows.Controls;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Grid.Automation {
	public class GridCellAutomationPeer : GridControlVirtualElementAutomationPeerBase, IGridItemProvider, IValueProvider, ISelectionItemProvider {
		protected int RowHandle { get; set; }
		protected int ColumnIndex { get; set; }
		protected ColumnBase Column { get { return DataControl.viewCore.VisibleColumnsCore[ColumnIndex]; } }
		public GridCellAutomationPeer(int rowHandle, int columnIndex, DataControlBase dataControl)
			: base(dataControl) {
			RowHandle = rowHandle;
			ColumnIndex = columnIndex;
		}
		protected override FrameworkElement GetFrameworkElement() {
			return DataControl.viewCore.GetCellElementByRowHandleAndColumn(RowHandle, Column);
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			return new List<AutomationPeer>();
		}
		protected override bool HasKeyboardFocusCore() {
			return DataControl.DataView.NavigationStyle == GridViewNavigationStyle.Cell &&
				DataControl.DataView.FocusedRowHandle == RowHandle &&
				DataControl.CurrentColumn == Column;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Value)
				return this;
			if(patternInterface == PatternInterface.GridItem)
				return this;
			if(patternInterface == PatternInterface.SelectionItem)
				return this;
			return null;
		}
		protected override string GetNameCore() {
			return string.Format(GridControlLocalizer.GetString(GridControlStringId.CellPeerName), DataControl.DataProviderBase.GetRowValue(RowHandle), ColumnIndex, ((IValueProvider)this).Value);
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Custom;
		}
#if SL
		protected override string GetLocalizedControlTypeCore() {
			return string.Empty;
		}
#endif
		#region IValueProvider Members
		bool IValueProvider.IsReadOnly {
			get { return Column.ReadOnly; }
		}
		void IValueProvider.SetValue(string value) {
			if(!Column.ReadOnly)
				DataControl.SetCellValueCore(RowHandle, Column.FieldName, value);
		}
		string IValueProvider.Value {
			get { return DataControl.DataView.GetColumnDisplayText(DataControl.GetCellValue(RowHandle, Column.FieldName), Column); }
		}
		#endregion
		#region IGridItemProvider Members
		int IGridItemProvider.Column {
			get { return ColumnIndex; }
		}
		int IGridItemProvider.ColumnSpan {
			get { return 1; }
		}
		IRawElementProviderSimple IGridItemProvider.ContainingGrid {
#if !SL
			get { return DataControl.AutomationPeer; }
#else
			get { return base.ProviderFromPeer(DataControl.AutomationPeer); }
#endif
		}
		int IGridItemProvider.Row {
			get { return DataControl.GetRowVisibleIndexByHandleCore(RowHandle); }
		}
		int IGridItemProvider.RowSpan {
			get { return 1; }
		}
		#endregion
		#region ISelectionItemProvider Members
		void ISelectionItemProvider.AddToSelection() {
			DataControl.DataView.SelectionStrategy.SelectCell(RowHandle, Column);
		}
		bool ISelectionItemProvider.IsSelected {
			get {
				if(DataControl.SelectionMode == MultiSelectMode.Cell)
					return DataControl.DataView.SelectionStrategy.IsCellSelected(RowHandle, Column);
				return DataControl.DataView.FocusedRowHandle == RowHandle && DataControl.CurrentColumn == Column;
			}
		}
		void ISelectionItemProvider.RemoveFromSelection() {
			DataControl.DataView.SelectionStrategy.UnselectCell(RowHandle, Column);
		}
		void ISelectionItemProvider.Select() {
			DataControl.CurrentColumn = Column;
			DataControl.DataView.SetFocusedRowHandle(RowHandle);
			if(DataControl.SelectionMode == MultiSelectMode.Cell) {
				DataControl.BeginSelection();
				DataControl.UnselectAll();
				DataControl.DataView.SelectionStrategy.SelectCell(RowHandle, Column);
				DataControl.EndSelection();
			}
		}
		IRawElementProviderSimple ISelectionItemProvider.SelectionContainer {
#if !SL
			get { return DataControl.AutomationPeer; }
#else
			get { return base.ProviderFromPeer(DataControl.AutomationPeer); }
#endif
		}
		#endregion
	}
}
