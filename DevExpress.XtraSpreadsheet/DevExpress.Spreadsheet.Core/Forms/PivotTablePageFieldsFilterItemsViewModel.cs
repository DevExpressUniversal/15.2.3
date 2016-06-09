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

using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Forms {
	public class PivotTableFieldsFilterItemsViewModel : ViewModelBase {
		#region Fields
		ISpreadsheetControl control;
		PivotTable pivotTable;
		PivotField field;
		List<PivotItemFilterInfo> items;
		bool selectMultipleItems;
		#endregion
		public PivotTableFieldsFilterItemsViewModel(ISpreadsheetControl control) {
			this.control = control;
		}
		#region Events
		#region SelectMultipleItemsChanged
		EventHandler selectMultipleItemsChanged;
		public event EventHandler SelectMultipleItemsChanged { add { selectMultipleItemsChanged += value; } remove { selectMultipleItemsChanged -= value; } }
		protected virtual void RaiseSelectMultipleItemsChanged() {
			if (selectMultipleItemsChanged != null)
				selectMultipleItemsChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		#region Properties
		public ISpreadsheetControl Control { get { return control; } }
		public PivotTable PivotTable {
			get { return pivotTable; }
			set {
				if (PivotTable == value)
					return;
				pivotTable = value;
			}
		}
		public PivotField Field {
			get { return field; }
			set {
				if (Field == value)
					return;
				field = value;
				OnPropertyChanged("Field");
			}
		}
		public List<PivotItemFilterInfo> Items {
			get { return items; }
			set {
				if (Items == value)
					return;
				items = value;
				OnPropertyChanged("Items");
			}
		}
		public bool SelectMultipleItems {
			get { return selectMultipleItems; }
			set {
				if (SelectMultipleItems == value)
					return;
				selectMultipleItems = value;
				RaiseSelectMultipleItemsChanged();
				OnPropertyChanged("SelectMultipleItems");
				OnPropertyChanged("SelectMultipleItemsVisible");
			}
		}
		public PivotTableAxis Axis { get; set; }
		public bool SelectMultipleItemsVisible { get { return Axis == PivotTableAxis.Page; } }
		#endregion
		public bool Validate() {
			return Axis == PivotTableAxis.Page ?
				new PivotTablePageFieldsFilterItemsCommand(Control).Validate(this) :
				new PivotTableNonPageFieldsFilterItemsCommand(Control).Validate(this);
		}
		public void ApplyChanges() {
			if (Axis == PivotTableAxis.Page)
				new PivotTablePageFieldsFilterItemsCommand(Control).ApplyChanges(this);
			else
				new PivotTableNonPageFieldsFilterItemsCommand(Control).ApplyChanges(this);
		}
	}
	public class PivotItemFilterInfo {
		#region Fields
		string name;
		int index;
		bool isVisible;
		#endregion
		public PivotItemFilterInfo(string name, int index, bool isVisible) {
			this.name = name;
			this.index = index;
			this.isVisible = isVisible;
		}
		#region Properties
		public string Name {
			get { return name; }
			set {
				if (Name == value)
					return;
				name = value;
			}
		}
		public int Index {
			get { return index; }
			set {
				if (Index == value)
					return;
				index = value;
			}
		}
		public bool IsVisible {
			get { return isVisible; }
			set {
				if (IsVisible == value)
					return;
				isVisible = value;
			}
		}
		#endregion
		public override string ToString() {
			return name;
		}
	}
}
