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
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.API.Internal;
using DevExpress.XtraSpreadsheet.Internal;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Model {
	public interface INameBoxControllerOwner {
		event EventHandler SelectedIndexChanged;
		int SelectedIndex { get; set; }
		object EditValue { get; set; }
		bool SelectionMode { get; set; }
		bool IsEnabled { get; set; }
	}
	public interface INameBoxControl {
		bool SelectionMode { get; set; }
	}
	public partial class NameBoxController : IDisposable {
		#region Fields
		readonly INameBoxControllerOwner owner;
		ISpreadsheetControl spreadsheetControl;
		List<DefinedNameBase> visibleDefinedNames;
		List<Table> tables;
		string ownersText;
		#endregion
		public NameBoxController(INameBoxControllerOwner owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
			this.visibleDefinedNames = new List<DefinedNameBase>();
			this.tables = new List<Table>();
			this.ownersText = String.Empty;
		}
		#region Properties
		protected internal virtual DocumentModel Workbook { get { return SpreadsheetControl != null ? SpreadsheetControl.InnerControl.DocumentModel : null; } }
		protected internal List<DefinedNameBase> VisibleDefinedNames { get { return visibleDefinedNames; } }
		protected internal List<Table> Tables { get { return tables; } }
		public ISpreadsheetControl SpreadsheetControl {
			get {
				return spreadsheetControl;
			}
			set {
				if (spreadsheetControl == value)
					return;
				UnsubscribeControlEvents();
				ResetFields();
				spreadsheetControl = value;
				if (value != null) {
					SubscribeControlEvents();
					UpdateVisibleDefinedNames(null);
					OwnerSelectionChanged();
				}
			}
		}
		public string OwnersText { get { return ownersText; } }
		public SheetViewSelection Selection { get { return Workbook.ActiveSheet.Selection; } }
		#endregion
		void SubscribeControlEvents() {
			owner.SelectedIndexChanged += OnNameBoxSelectedIndexChanged;
			if (SpreadsheetControl == null)
				return;
			SpreadsheetControl.DocumentLoaded += OnDocumentLoaded;
			SpreadsheetControl.EmptyDocumentCreated += OnEmptyDocumentCreated;
			Workbook.InnerActiveSheetChanged += OnActiveSheetChanged;
			Workbook.InnerSelectionChanged += OnDocumentSelectionChanged;
			Workbook.SchemaChanged += OnSchemaChanged;
			Workbook.EndDocumentUpdate += OnEndDocumentUpdate;
			if (SpreadsheetControl.InnerControl != null) {
				SpreadsheetControl.InnerControl.CellBeginEdit += OnCellBeginEdit;
				SpreadsheetControl.InnerControl.CellEndEdit += OnCellEndEdit;
				SpreadsheetControl.InnerControl.CellCancelEdit += OnCellCancelEdit;
			}
			InternalAPI internalAPI = Workbook.InternalAPI;
			internalAPI.TableAdd += OnTableAdd;
			internalAPI.TableCollectionClear += OnTableCollectionClear;
			internalAPI.TableRemoveAt += OnTableRemoveAt;
		}
		void UnsubscribeControlEvents() {
			owner.SelectedIndexChanged -= OnNameBoxSelectedIndexChanged;
			if (SpreadsheetControl == null)
				return;
			SpreadsheetControl.DocumentLoaded -= OnDocumentLoaded;
			SpreadsheetControl.EmptyDocumentCreated -= OnEmptyDocumentCreated;
			Workbook.InnerActiveSheetChanged -= OnActiveSheetChanged;
			Workbook.InnerSelectionChanged -= OnDocumentSelectionChanged;
			Workbook.SchemaChanged -= OnSchemaChanged;
			Workbook.EndDocumentUpdate -= OnEndDocumentUpdate;
			if (SpreadsheetControl.InnerControl != null) {
				SpreadsheetControl.InnerControl.CellBeginEdit -= OnCellBeginEdit;
				SpreadsheetControl.InnerControl.CellEndEdit -= OnCellEndEdit;
				SpreadsheetControl.InnerControl.CellCancelEdit -= OnCellCancelEdit;
			}
			InternalAPI internalAPI = Workbook.InternalAPI;
			internalAPI.TableAdd -= OnTableAdd;
			internalAPI.TableCollectionClear -= OnTableCollectionClear;
			internalAPI.TableRemoveAt -= OnTableRemoveAt;
		}
		void OnCellCancelEdit(object sender, SpreadsheetCellCancelEditEventArgs e) {
			owner.IsEnabled = SpreadsheetControl.InnerControl.IsEnabled;
		}
		void OnCellEndEdit(object sender, SpreadsheetCellValidatingEventArgs e) {
			owner.IsEnabled = SpreadsheetControl.InnerControl.IsEnabled;
		}
		void OnCellBeginEdit(object sender, SpreadsheetCellCancelEventArgs e) {
			owner.IsEnabled = false;
		}
		void ResetFields() {
			spreadsheetControl = null;
			visibleDefinedNames = new List<DefinedNameBase>();
			tables = new List<Table>();
			ownersText = String.Empty;
		}
		void OnTableRemoveAt(object sender, TableRemoveAtEventArgs e) {
			UpdateVisibleDefinedNames(null);
		}
		void OnTableCollectionClear(object sender, TableCollectionClearEventArgs e) {
			UpdateVisibleDefinedNames(null);
		}
		void OnTableAdd(object sender, TableAddEventArgs e) {
			UpdateVisibleDefinedNames(e.Table);
		}
		void OnDocumentSelectionChanged(object sender, EventArgs e) {
			OwnerSelectionChanged();
		}
		void OnEmptyDocumentCreated(object sender, EventArgs e) {
			UpdateVisibleDefinedNames(null);
			OwnerSelectionChanged();
		}
		void OnSchemaChanged(object sender, EventArgs e) {
			UpdateVisibleDefinedNames(null);
		}
		void OnEndDocumentUpdate(object sender, DocumentUpdateCompleteEventArgs e) {
			if ((e.DeferredChanges.ChangeActions & DocumentModelChangeActions.RaiseSchemaChanged) != 0)
				UpdateVisibleDefinedNames(null);
		}
		internal void UpdateVisibleDefinedNames(Table newTable) {
			VisibleDefinedNames.Clear();
			Tables.Clear();
			PopulateDefinedNameFromWorkbook();
			PopulateDefinedNameFromActiveWorksheet();
			PopulateDefinedNameFromTables(newTable);
			RaiseVisibleDefinedNamesChanged();
		}
		void OnDocumentLoaded(object sender, EventArgs e) {
			UpdateVisibleDefinedNames(null);
			OwnerSelectionChanged();
		}
		void OnActiveSheetChanged(object sender, ActiveSheetChangedEventArgs e) {
			UpdateVisibleDefinedNames(null);
			OwnerSelectionChanged();
		}
		void PopulateDefinedNameFromTables(Table newTable) {
			foreach (Worksheet sheet in Workbook.Sheets)
				foreach (Table table in sheet.Tables)
					Tables.Add(table);
		}
		void PopulateDefinedNameFromActiveWorksheet() {
			List<DefinedName> withLowestPriority = new List<DefinedName>();
			foreach (DefinedName name in Workbook.ActiveSheet.DefinedNames) {
				if (ShouldUseDefinedName(name)) {
					foreach (DefinedName bookName in VisibleDefinedNames)
						if (name.Name == bookName.Name) {
							withLowestPriority.Add(bookName);
							break;
						}
					VisibleDefinedNames.Add(name);
				}
			}
			foreach (DefinedName name in withLowestPriority)
				VisibleDefinedNames.Remove(name);
		}
		void PopulateDefinedNameFromWorkbook() {
			foreach (DefinedName name in Workbook.DefinedNames) {
				if (ShouldUseDefinedName(name))
					VisibleDefinedNames.Add(name);
			}
		}
		bool ShouldUseDefinedName(DefinedName name) {
			CellRangeBase nameRange = name.GetReferencedRange();
			if (name.IsHidden)
				return false;
			if (nameRange == null)
				return false;
			if (nameRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = (CellUnion)nameRange;
				return union.CheckSheetSameness() && union.InnerCellRanges[0].Worksheet is Worksheet;
			}
			else
				return nameRange.Worksheet is Worksheet;
		}
		public void RefreshSelection() {
			OwnerSelectionChanged();
		}
		void OwnerSelectionChanged() {
			owner.SelectedIndexChanged -= OnNameBoxSelectedIndexChanged;
			Workbook.InnerActiveSheetChanged -= OnActiveSheetChanged;
			try {
				OwnerSelectionChangedCore();
			}
			finally {
				owner.SelectedIndexChanged += OnNameBoxSelectedIndexChanged;
				Workbook.InnerActiveSheetChanged += OnActiveSheetChanged;
			}
		}
		void OwnerSelectionChangedCore() {
			CellRange firstSelectedRange = Selection.SelectedRanges[0];
			if (!Selection.IsSingleCell && Selection.IsSingleMergedCell()) {
				if (TryFindDefinedNameName(IsDefinedNameReferencedToMergedRangeTopLeft))
					return;
				ownersText = firstSelectedRange.TopLeft.ToString();
				RaiseSelectionChanged();
				return;
			}
			else if (Selection.IsMultiMergedCell()) {
				ownersText = Selection.ActiveRange.TopLeft.ToString();
				RaiseSelectionChanged();
				return;
			}
			if (owner.SelectionMode && !Selection.IsSingleCell) {
				ownersText = GetOwnerTextInSelectionMode();
				RaiseSelectionChanged();
				return;
			}
			else if (Selection.IsDrawingMultiSelection) {
				ownersText = String.Empty;
				RaiseSelectionChanged();
				return;
			}
			if (Selection.IsDrawingSelected) {
				ownersText = Workbook.ActiveSheet.DrawingObjects[Selection.SelectedDrawingIndexes[0]].DrawingObject.Properties.Name;
				RaiseSelectionChanged();
				return;
			}
			if (Selection.IsMultiSelection) {
				ownersText = GetActiveCellReference();
				RaiseSelectionChanged();
				return;
			}
			if (TryFindDefinedNameName((definedNameRange) => firstSelectedRange.EqualsPosition(definedNameRange)))
				return;
			if (TryFindTableName(Selection.ActiveRange))
				return;
			ownersText = GetActiveCellReference();
			RaiseSelectionChanged();
		}
		bool IsDefinedNameReferencedToMergedRangeTopLeft(CellRangeBase definedNameReferencedRange) {
			CellPosition topLeft = definedNameReferencedRange.TopLeft;
			CellRange mergedRange = Workbook.ActiveSheet.MergedCells.FindMergedCell(topLeft.Column, topLeft.Row);
			if (mergedRange == null)
				return false;
			return mergedRange.TopLeft.EqualsPosition(topLeft) && definedNameReferencedRange.CellCount == 1;
		}
		bool TryFindTableName(CellRange activeRange) {
			foreach (Table table in Tables) {
				if (!IsTableSelected(table, activeRange))
					continue;
				ownersText = table.Name;
				RaiseSelectionChanged();
				return true;
			}
			return false;
		}
		bool TryFindDefinedNameName(Predicate<CellRangeBase> definedNameReferencedRangeMeetsCondition) {
			CellRange selectedRange = Selection.ActiveRange;
			Worksheet activeSheet = Selection.Sheet;
			foreach (DefinedName name in VisibleDefinedNames) {
				CellRangeBase nameRange = name.GetReferencedRange(Selection.ActiveCell, activeSheet);
				if (nameRange == null || !nameRange.IsAbsolute() || !Object.ReferenceEquals(nameRange.Worksheet, activeSheet))
					continue;
				if (definedNameReferencedRangeMeetsCondition(nameRange)) { 
					ownersText = name.Name;
					RaiseSelectionChanged();
					return true;
				}
			}
			return false;
		}
		string GetOwnerTextInSelectionMode() {
			CellPosition bottomRight = Selection.ActiveRange.BottomRight;
			CellPosition topLeft = Selection.ActiveRange.TopLeft;
			int rowCount = bottomRight.Row - topLeft.Row + 1;
			int columnCount = bottomRight.Column - topLeft.Column + 1;
			return String.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.NameBox_SelectionModeFormat), rowCount, columnCount);
		}
		string GetActiveCellReference() {
			CellRange activeCell = new CellRange(Workbook.ActiveSheet, Selection.ActiveCell, Selection.ActiveCell);
			bool isUseR1C1 = Workbook.DataContext.UseR1C1ReferenceStyle;
			PositionType positionType = isUseR1C1 ? PositionType.Absolute : PositionType.Relative;
			string reference = CellRangeToString.GetReferenceCommon(activeCell, isUseR1C1, new CellPosition(0, 0), positionType, positionType, false);
			return reference;
		}
		bool IsTableSelected(Table table, CellRange selectedRange) {
			if (!Object.ReferenceEquals(table.Worksheet, Workbook.ActiveSheet))
				return false;
			CellRange dataRange = table.GetDataRange();
			if (dataRange == null)
				return false;
			return selectedRange.EqualsPosition(dataRange);
		}
		void OnNameBoxSelectedIndexChanged(object sender, EventArgs e) {
			if (owner.SelectedIndex == -1)
				return;
			CellRangeBase range = GetSelectedRangeOrNull();
			SetSelection(range);
		}
		CellRangeBase GetSelectedRangeOrNull() {
			int visibleDefinedNamesCount = VisibleDefinedNames.Count;
			if (owner.SelectedIndex < visibleDefinedNamesCount)
				return ((DefinedName)VisibleDefinedNames[owner.SelectedIndex]).GetReferencedRange(Selection.ActiveCell, Selection.Sheet);
			Table table = Tables[owner.SelectedIndex - visibleDefinedNamesCount];
			return table.GetDataRange();
		}
#if !SL
		public void OnNameBoxKeyDown(object sender, KeyEventArgs e) {
#else
		public void OnNameBoxKeyDown(object sender, DevExpress.Data.KeyEventArgs e) {
#endif
			if (e.KeyCode == Keys.Enter) {
				OnNameBoxEndEdit((string)owner.EditValue);
				SpreadsheetControl.Focus();
				SpreadsheetControl.InnerControl.Owner.Redraw();
			}
			else if (e.KeyCode == Keys.Escape) {
				OwnerSelectionChanged();
				SpreadsheetControl.Focus();
				SpreadsheetControl.InnerControl.Owner.Redraw();
			}
#if !SL
			e.SuppressKeyPress = (e.KeyCode == Keys.Return);
#endif
		}
		void OnNameBoxEndEdit(string reference) {
			if (TryNavigateToDefinedName(reference))
				return;
			if (TryCreateDefinedName(reference))
				return;
			TryNavigateToRange(reference);
		}
		void TryNavigateToRange(string reference) {
			CellRangeBase oldActiveRange = Selection.ActiveRange;
			ParsedExpression expression = HyperlinkExpressionParser.Parse(Workbook.DataContext, reference, false);
			if (expression == null) {
				SetSelection(oldActiveRange);
				ShowWarningMessage();
				return;
			}
			CellRangeBase range = HyperlinkExpressionParser.GetTargetRange(Workbook.DataContext, expression, false);
			if (range == null) {
				SetSelection(oldActiveRange);
				ShowWarningMessage();
				return;
			}
			SetSelection(range);
		}
		void ShowWarningMessage() {
			SpreadsheetControl.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorInvalidDefinedName_FormulaBar));
		}
		bool TryCreateDefinedName(string name) {
			if (!WorkbookDataContext.IsIdent(name))
				return false;
			if (Workbook.ActiveSheet.Properties.Protection.SheetLocked) {
				RefreshSelection();
				return true; 
			}
			if (Selection.IsDrawingMultiSelection)
				return false;
			if (Selection.IsDrawingSelected) {
				Workbook.ActiveSheet.DrawingObjects[Selection.SelectedDrawingIndexes[0]].DrawingObject.Properties.Name = name;
				return true;
			}
			Workbook.BeginUpdate();
			try {
				if (Workbook.ActiveSheet.DefinedNames.Contains(name))
					Workbook.ActiveSheet.RemoveDefinedName(name);
				if (Workbook.DefinedNames.Contains(name))
					Workbook.RemoveDefinedName(name);
				string reference = CellRangeToString.GetReferenceCommon(Selection.ActiveRange, Workbook.DataContext.UseR1C1ReferenceStyle, CellPosition.InvalidValue, PositionType.Absolute, PositionType.Absolute, true);
				Workbook.CreateDefinedName(name, reference);
			}
			finally {
				Workbook.EndUpdate();
			}
			return true;
		}
		bool TryNavigateToDefinedName(string reference) {
			foreach (DefinedName name in Workbook.DefinedNames)
				if (name.Name == reference) {
					SetSelection(name.GetReferencedRange(Selection.ActiveCell, Selection.Sheet));
					return true;
				}
			foreach (DefinedName name in Workbook.ActiveSheet.DefinedNames)
				if (name.Name == reference) {
					SetSelection(name.GetReferencedRange(Selection.ActiveCell, Selection.Sheet));
					return true;
				}
			foreach (Worksheet sheet in Workbook.Sheets)
				foreach (Table table in sheet.Tables)
					if (table.Name == reference) {
						SetSelection(table.Range);
						return true;
					}
			for (int i = 0; i < Workbook.ActiveSheet.DrawingObjects.Count; i++)
				if (Workbook.ActiveSheet.DrawingObjects[i].DrawingObject.Properties.Name == reference) {
					Selection.AddSelectedDrawingIndex(i);
					return true;
				}
			return false;
		}
		void SetSelection(CellRangeBase nameRange) {
			if (nameRange == null)
				return;
			SpreadsheetControl.BeginUpdate();
			try {
				Worksheet activeSheet = null;
				if (nameRange.RangeType == CellRangeType.UnionRange) {
					CellUnion union = (CellUnion)nameRange;
					if (union.CheckSheetSameness())
						activeSheet = union.InnerCellRanges[0].Worksheet as Worksheet;
				}
				else
					activeSheet = nameRange.Worksheet as Worksheet;
				if (activeSheet != null) {
					Workbook.ActiveSheet = activeSheet;
					Selection.SetSelection(nameRange);
					InnerSpreadsheetControl innerControl = SpreadsheetControl.InnerControl;
					if (innerControl.ActiveView != null && !innerControl.IsCellVisible(nameRange.TopLeft))
						Workbook.ActiveSheet.ScrollTo(nameRange.TopLeft.Row, nameRange.TopLeft.Column);
				}
			}
			finally {
				SpreadsheetControl.EndUpdate();
			}
		}
		public void Dispose() {
			UnsubscribeControlEvents();
			visibleDefinedNames = null;
			tables = null;
		}
	}
}
