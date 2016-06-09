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
using DevExpress.Utils;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Commands;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region FindReplaceViewModel
	public class FindReplaceViewModel : ViewModelBase {
		#region Fields
		readonly ISpreadsheetControl control;
		readonly Dictionary<string, SearchBy> searchByValues;
		readonly Dictionary<string, SearchIn> searchInValues;
		string findWhat = String.Empty;
		string replaceWith = String.Empty;
		bool matchCase;
		bool matchEntireCellContents;
		string searchBy;
		string searchIn;
		bool replaceMode;
		#endregion
		public FindReplaceViewModel(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.searchByValues = CreateSearchByValues();
			this.searchInValues = CreateSearchInValues();
			this.SearchBy = SearchByToString(Spreadsheet.SearchBy.Rows);
			this.SearchIn = SearchInToString(Spreadsheet.SearchIn.Formulas);
		}
		#region Properties
		public ISpreadsheetControl Control { get { return control; } }
		public string FindWhat {
			get { return findWhat; }
			set {
				if (FindWhat == value)
					return;
				this.findWhat = value;
				OnPropertyChanged("FindWhat");
			}
		}
		public string ReplaceWith {
			get { return replaceWith; }
			set {
				if (ReplaceWith == value)
					return;
				this.replaceWith = value;
				OnPropertyChanged("ReplaceWith");
			}
		}
		public bool MatchCase {
			get { return matchCase; }
			set {
				if (MatchCase == value)
					return;
				this.matchCase = value;
				OnPropertyChanged("MatchCase");
			}
		}
		public bool MatchEntireCellContents {
			get { return matchEntireCellContents; }
			set {
				if (MatchEntireCellContents == value)
					return;
				this.matchEntireCellContents = value;
				OnPropertyChanged("MatchEntireCellContents");
			}
		}
		public bool ReplaceMode {
			get { return replaceMode; }
			set {
				if (ReplaceMode == value)
					return;
				this.replaceMode = value;
				this.searchIn = SearchInToString(Spreadsheet.SearchIn.Formulas);
				OnPropertyChanged("ReplaceMode");
				OnPropertyChanged("SearchIn");
				OnPropertyChanged("SearchInDataSource");
			}
		}
		public string SearchBy {
			get { return searchBy; }
			set {
				if (SearchBy == value)
					return;
				this.searchBy = value;
				OnPropertyChanged("SearchBy");
			}
		}
		public string SearchIn {
			get { return searchIn; }
			set {
				if (SearchIn == value)
					return;
				this.searchIn = value;
				OnPropertyChanged("SearchIn");
			}
		}
		public IEnumerable<string> SearchByDataSource { get { return searchByValues.Keys; } }
		public IEnumerable<string> SearchInDataSource {
			get {
				if (ReplaceMode)
					return new string[] { SearchInToString(Spreadsheet.SearchIn.Formulas) };
				return searchInValues.Keys;
			}
		}
		public bool ReplaceEnabled {
			get { return !Control.InnerControl.IsReadOnly; }
		}
		#endregion
		Dictionary<string, SearchBy> CreateSearchByValues() {
			Dictionary<string, SearchBy> result = new Dictionary<string, SearchBy>();
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_SearchByRows), Spreadsheet.SearchBy.Rows);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_SearchByColumns), Spreadsheet.SearchBy.Columns);
			return result;
		}
		Dictionary<string, SearchIn> CreateSearchInValues() {
			Dictionary<string, SearchIn> result = new Dictionary<string, SearchIn>();
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_SearchInFormulas), Spreadsheet.SearchIn.Formulas);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_SearchInValues), Spreadsheet.SearchIn.Values);
			return result;
		}
		public string SearchByToString(SearchBy value) {
			foreach (string key in searchByValues.Keys)
				if (searchByValues[key] == value)
					return key;
			return String.Empty;
		}
		public SearchBy StringToSearchBy(string value) {
			SearchBy result;
			if (!searchByValues.TryGetValue(value, out result))
				return Spreadsheet.SearchBy.Rows;
			else
				return result;
		}
		public string SearchInToString(SearchIn value) {
			foreach (string key in searchInValues.Keys)
				if (searchInValues[key] == value)
					return key;
			return String.Empty;
		}
		public SearchIn StringToSearchIn(string value) {
			SearchIn result;
			if (!searchInValues.TryGetValue(value, out result))
				return Spreadsheet.SearchIn.Formulas;
			else
				return result;
		}
		public void FindNext() {
			FindCommand command = new FindCommand(Control);
			command.FindNext(this);
		}
		public void ReplaceNext() {
			ReplaceCommand command = new ReplaceCommand(Control);
			command.ReplaceNext(this);
		}
		public void ReplaceAll() {
			ReplaceCommand command = new ReplaceCommand(Control);
			command.ReplaceAll(this);
		}
	}
	#endregion
}
