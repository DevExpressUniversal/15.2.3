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
using DevExpress.Spreadsheet;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public interface IReferenceEditControl {
	}
	public interface IReferenceEditControllerOwner {
		List<CellRange> Selection { get; }
		bool Activated { get; set; }
		string Text { get; set; }
		event EventHandler ActivatedChanged;
		event EventHandler EditValueChanged;
		event EventHandler CollapseButtonClick;
	}
	public partial class ReferenceEditController {
		#region Fields
		readonly IReferenceEditControllerOwner owner;
		readonly ISpreadsheetControl spreadsheetControl;
		bool includeSheetName;
		ReferenceEditControlViewState viewState;
		bool isCollapsed;
		#endregion
		public ReferenceEditController(IReferenceEditControllerOwner owner, ISpreadsheetControl spreadsheetControl) {
			Guard.ArgumentNotNull(owner, "owner");
			Guard.ArgumentNotNull(spreadsheetControl, "spreadsheetControl");
			this.owner = owner;
			this.spreadsheetControl = spreadsheetControl;
			this.viewState = ReferenceEditControlViewState.Expand;
			this.PositionType = Model.PositionType.Absolute;
			this.EditValuePrefix = "=";
			ClearSelection(); 
			SubscribeSpreadsheetEvents();
			SubscribeOwnerEvents();
		}
		#region Properties
		public ISpreadsheetControl Spreadsheet { get { return spreadsheetControl; } }
		public bool IncludeSheetName { get { return includeSheetName; } set { includeSheetName = value; } }
		protected WorkbookDataContext DataContext { get { return Spreadsheet.InnerDocumentServer.DocumentModel.DataContext; } }
		protected bool ActiveSheetAlreadyWasChanged { get { return includeSheetName; } }
		protected DocumentModel DocumentModel { get { return Spreadsheet.InnerDocumentServer != null ? Spreadsheet.InnerDocumentServer.DocumentModel : null; } }
		protected SheetViewSelection Selection { get { return DocumentModel != null ? DocumentModel.ActiveSheet.ReferenceEditSelection : null; } }
		protected internal ReferenceEditControlViewState ViewState {
			get { return viewState; }
			protected set {
				if (viewState == value)
					return;
				viewState = value;
				IsCollapsed = (viewState != ReferenceEditControlViewState.Expand);
			}
		}
		public bool IsCollapsed {
			get { return isCollapsed; }
			protected set {
				if (isCollapsed == value)
					return;
				isCollapsed = value;
				RaiseCollapsedChanged();
			}
		}
		public PositionType PositionType { get; set; }
		public string EditValuePrefix { get; set; }
		public bool SuppressActiveSheetChanging { get; set; }
		#endregion
		#region Subscribe / Unsubscribe Spreadsheet's events
		void SubscribeSpreadsheetEvents() {
			DocumentModel.InnerActiveSheetChanged += OnActiveSheetChanged;
			DocumentModel.ActiveSheetChanging += DocumentModel_ActiveSheetChanging;
			for (int i = 0; i < DocumentModel.SheetCount; i++) {
				SheetViewSelection sheetViewSelection = ((Worksheet)DocumentModel.GetSheetByIndex(i)).ReferenceEditSelection;
				sheetViewSelection.SelectionChanged += OnViewSelectionChanged;
				sheetViewSelection.BeginReferenceEditMode += OnViewBeginReferenceEditMode;
				sheetViewSelection.EndReferenceEditMode += OnViewEndReferenceEditMode;
			}
		}
		void UnsubscribeSpreadsheetEvents() {
			if (Spreadsheet != null) {
				DocumentModel.InnerActiveSheetChanged -= OnActiveSheetChanged;
				DocumentModel.ActiveSheetChanging -= DocumentModel_ActiveSheetChanging;
				if (Spreadsheet.InnerDocumentServer != null)
					for (int i = 0; i < DocumentModel.SheetCount; i++) {
						SheetViewSelection sheetViewSelection = ((Worksheet)DocumentModel.GetSheetByIndex(i)).ReferenceEditSelection;
						sheetViewSelection.SelectionChanged -= OnViewSelectionChanged;
						sheetViewSelection.BeginReferenceEditMode -= OnViewBeginReferenceEditMode;
						sheetViewSelection.EndReferenceEditMode -= OnViewEndReferenceEditMode;
					}
			}
		}
		#endregion
		#region Subscribe/Unsubscribe Owner's events
		void SubscribeOwnerEvents() {
			owner.ActivatedChanged += OnOwnerActivatedChanged;
			owner.EditValueChanged += OnOwnerEditValueChanged;
			owner.CollapseButtonClick += OnOwnerCollapseButtonClick;
		}
		void UnsubscribeOwnerEvents() {
			if (owner == null)
				return;
			owner.ActivatedChanged -= OnOwnerActivatedChanged;
			owner.EditValueChanged -= OnOwnerEditValueChanged;
			owner.CollapseButtonClick -= OnOwnerCollapseButtonClick;
		}
		#endregion
		void ClearSelection() {
			if (Selection == null)
				return;
			Selection.Clear();
		}
		void OnOwnerCollapseButtonClick(object sender, EventArgs e) {
			ViewState = IsCollapsed ? ReferenceEditControlViewState.Expand : ReferenceEditControlViewState.CollapsedAfterCollapsedButtonClick;
		}
		void OnOwnerEditValueChanged(object sender, EventArgs e) {
			SetNewOwnerSelection();
		}
		void OnOwnerActivatedChanged(object sender, EventArgs e) {
			if (owner.Activated)
				SetReferenceEditSelection();
			else
				Selection.SelectionChanged -= OnViewSelectionChanged;
		}
		void SetReferenceEditSelection() {
			if (owner.Selection.Count == 0) {
				Selection.SelectionChanged -= OnViewSelectionChanged;
				Selection.IsEmpty = true;
				Selection.SelectionChanged += OnViewSelectionChanged;
				return;
			}
			CellUnion unionRange = null;
			if (owner.Selection.Count > 1)
				unionRange = new CellUnion(owner.Selection.ToList<CellRangeBase>());
			CellRange range = owner.Selection[0];
			if (range.Worksheet != null && range.Worksheet.Name != DocumentModel.ActiveSheet.Name) {
				Worksheet rangeSheet = range.Worksheet as Worksheet;
				if (rangeSheet == null || SuppressActiveSheetChanging)
					return;
				DocumentModel.ActiveSheet = rangeSheet;
			}
			Selection.SelectionChanged -= OnViewSelectionChanged;
			try {
				if (unionRange == null)
					Selection.SetSelection(range);
				else
					Selection.SetSelection(unionRange);
			}
			finally {
				Selection.SelectionChanged += OnViewSelectionChanged;
			}
		}
		void SetNewOwnerSelection() {
			CellRangeBase range = CellRangeBase.TryParse(owner.Text, DataContext);
			if (range == null) {
				RaiseSelectionChanged(new List<CellRange>());
				DocumentModel.ShowReferenceSelection = false;
			}
			else {
				List<CellRange> ranges = new List<CellRange>();
				CellUnion union = range as CellUnion;
				if (union != null) {
					foreach (CellRange innerRange in union.InnerCellRanges)
						ranges.Add(innerRange);
				}
				else
					ranges.Add((CellRange)range);
				RaiseSelectionChanged(ranges);
				DocumentModel.ShowReferenceSelection = true;
			}
			SetReferenceEditSelection();
			Spreadsheet.InnerControl.Owner.Redraw();
		}
		void OnViewSelectionChanged(object sender, EventArgs e) {
			if (!owner.Activated)
				return;
			RaiseSelectionChanged(Selection.SelectedRanges);
			RaiseOwnerTextChanged(GetEditValue());
		}
		string GetEditValue() {
			return EditValuePrefix + RangeToString();
		}
		string GetReferenceCommon(CellRange range) {
			return CellRangeToString.GetReferenceCommon(range, DataContext.UseR1C1ReferenceStyle, CellPosition.InvalidValue, PositionType, PositionType, includeSheetName);
		}
		public string RangeToString() {
			char separator = DataContext.GetListSeparator();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < owner.Selection.Count; i++) {
				stringBuilder.Append(GetReferenceCommon((CellRange)owner.Selection[i]));
				stringBuilder.Append(separator);
			}
			if (stringBuilder.Length > 0)
				stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
			return stringBuilder.ToString();
		}
		void OnViewBeginReferenceEditMode(object sender, EventArgs e) {
			if (!owner.Activated || ViewState == ReferenceEditControlViewState.CollapsedAfterCollapsedButtonClick)
				return;
			ViewState = ReferenceEditControlViewState.CollapsedAfterContinueSelection;
		}
		void OnViewEndReferenceEditMode(object sender, EventArgs e) {
			if (!owner.Activated || ViewState == ReferenceEditControlViewState.CollapsedAfterCollapsedButtonClick)
				return;
			ViewState = ReferenceEditControlViewState.Expand;
		}
		void DocumentModel_ActiveSheetChanging(object sender, ActiveSheetChangingEventArgs e) {
			e.Cancel = SuppressActiveSheetChanging;
		}
		void OnActiveSheetChanged(object sender, ActiveSheetChangedEventArgs e) {
			if (!owner.Activated)
				return;
			string editValue;
			if (!ActiveSheetAlreadyWasChanged || owner.Selection.Count == 0) {
				editValue = GetWorksheetName();
				Selection.IsEmpty = true;
				RaiseSelectionChanged(new List<CellRange>());
			}
			else {
				CellRange newRange = new CellRange(DocumentModel.ActiveSheet, owner.Selection[0].TopLeft, owner.Selection[0].BottomRight);
				List<CellRange> selection = new List<CellRange>();
				selection.Add(newRange);
				Selection.SetSelection(newRange);
				RaiseSelectionChanged(selection);
				editValue = GetReferenceCommon(newRange);
			}
			if (!string.IsNullOrEmpty(EditValuePrefix))
				editValue = EditValuePrefix + editValue;
			RaiseOwnerTextChanged(editValue);
			includeSheetName = true;
		}
		void RaiseOwnerTextChanged(string editValue) {
			UnsubscribeOwnerEvents();
			try {
				RaiseTextChanged(editValue);
			}
			finally {
				SubscribeOwnerEvents();
			}
		}
		string GetWorksheetName() {
			string worksheetName = Spreadsheet.ActiveWorksheet.Name;
			bool addQuotes = SheetDefinition.ShouldAddQuotes(false, worksheetName, worksheetName);
			string format = (addQuotes) ? "'{0}'!" : "{0}!";
			return string.Format(format, worksheetName);
		}
		public void Dispose() {
			UnsubscribeSpreadsheetEvents();
			UnsubscribeOwnerEvents();
		}
	}
	#region ReferenceEditControlViewState
	public enum ReferenceEditControlViewState {
		Expand,
		CollapsedAfterContinueSelection,
		CollapsedAfterCollapsedButtonClick
	}
	#endregion
}
