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
namespace DevExpress.XtraSpreadsheet.Forms {
	#region InsertPivotTableViewModel
	public class InsertPivotTableViewModel : ReferenceEditViewModel {
		#region Fields
		string location;
		string source;
		CellRange locationRange;
		PivotCacheSourceWorksheet pivotSource;
		bool newWorksheet;
		#endregion
		public InsertPivotTableViewModel(ISpreadsheetControl control) 
			: base(control) {
		}
		#region Properties
		public WorkbookDataContext DataContext { get { return Control.Document.Model.DocumentModel.DataContext; } }
		public string Source {
			get { return source; }
			set {
				if (Source == value)
					return;
				source = value;
			   OnPropertyChanged("Source");
			}
		}
		public PivotCacheSourceWorksheet PivotSource {
			get { return pivotSource; }
			set {
				if (PivotSource == value)
					return;
				pivotSource = value;
			}
		}
		public bool NewWorksheet {
			get { return newWorksheet; }
			set {
				if (NewWorksheet == value)
					return;
				newWorksheet = value;
				OnPropertyChanged("NewWorksheet");
				OnPropertyChanged("LocationEnabled");
			}
		}
		public string Location {
			get { return location; }
			set {
				if (Location == value)
					return;
				location = value;
				OnPropertyChanged("Location");
			}
		}
		public CellRange LocationRange {
			get { return locationRange; }
			set {
				if (LocationRange == value)
					return;
				locationRange = value;
			}
		}
		public bool LocationEnabled { get { return !NewWorksheet; } }
		#endregion
		public virtual bool Validate() {
			return CreateCommand().Validate(this);
		}
		public virtual void ApplyChanges() {
			CreateCommand().ApplyChanges(this);
		}
		InsertPivotTableCommand CreateCommand() {
			return new InsertPivotTableCommand(Control);
		}
	}
	#endregion
	#region MovePivotTableViewModel
	public class MovePivotTableViewModel : InsertPivotTableViewModel {
		#region Field
		PivotTable pivotTable;
		#endregion
		public MovePivotTableViewModel(ISpreadsheetControl control) 
			: base(control) {
		}
		#region Properties
		public PivotTable PivotTable {
			get { return pivotTable; }
			set {
				if (PivotTable == value)
					return;
				pivotTable = value;
			}
		}
		#endregion
		public override bool Validate() {
			return CreateCommand().Validate(this);
		}
		public override void ApplyChanges() {
			CreateCommand().ApplyChanges(this);
		}
		MovePivotTableCommand CreateCommand() {
			return new MovePivotTableCommand(Control);
		}
	}
	#endregion
	#region ChangeDataSourcePivotTableViewModel
	public class ChangeDataSourcePivotTableViewModel : InsertPivotTableViewModel {
		#region Field
		PivotTable pivotTable;
		#endregion
		public ChangeDataSourcePivotTableViewModel(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public PivotTable PivotTable {
			get { return pivotTable; }
			set {
				if (PivotTable == value)
					return;
				pivotTable = value;
			}
		}
		#endregion
		public override bool Validate() {
			return CreateCommand().Validate(this);
		}
		public override void ApplyChanges() {
			CreateCommand().ApplyChanges(this);
		}
		ChangeDataSourcePivotTableCommand CreateCommand() {
			return new ChangeDataSourcePivotTableCommand(Control);
		}
	}
	#endregion
}
