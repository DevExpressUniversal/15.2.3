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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.API.Internal;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region DefinedNameViewModelList
	public class DefinedNameViewModelList : List<DefineNameViewModel>, INotifyCollectionChanged {
		public DefinedNameViewModelList()
			: base() {
		}
		#region INotifyCollectionChanged Members
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		#endregion
		protected internal void RaiseCollectionChanged() {
			if(CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
	}
	#endregion
	#region NameManagerViewModel
	public class NameManagerViewModel : ViewModelBase, IReferenceEditViewModel {
		readonly ISpreadsheetControl control;
		readonly DefinedNameViewModelList definedNameList;
		string reference = String.Empty;
		int currentNameIndex;
		DefinedName targetName = null;
		public NameManagerViewModel(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.definedNameList = new DefinedNameViewModelList();
			UpdateDefinedNames();
		}
		#region Properties
		public ISpreadsheetControl Control { get { return control; } }
		DocumentModel DocumentModel { get { return control.InnerControl.DocumentModel; } }
		public DefinedNameViewModelList NamesDataSource { get { return definedNameList; } }
		public int CurrentNameIndex {
			get { return currentNameIndex; }
			set {
				if (CurrentNameIndex == value)
					return;
				this.currentNameIndex = value;
				UpdateReference();
				OnPropertyChanged("CurrentNameIndex");
				OnPropertyChanged("Reference");
				OnPropertyChanged("IsCurrentNameValid");
				OnPropertyChanged("IsReferenceChanged");
				OnPropertyChanged("IsDeleteAllowed");
				OnPropertyChanged("IsReferenceChangeAllowed");
			}
		}
		public bool IsCurrentNameValid { get { return CurrentNameIndex >= 0 && CurrentNameIndex < definedNameList.Count; } }
		public bool IsDeleteAllowed { get { return IsCurrentNameValid && definedNameList[CurrentNameIndex].IsDeleteAllowed; } }
		public bool IsReferenceChangeAllowed { get { return IsCurrentNameValid && definedNameList[CurrentNameIndex].IsReferenceChangeAllowed; } }
		public string Reference {
			get { return reference; }
			set {
				if (Reference == value)
					return;
				this.reference = value;
				OnPropertyChanged("Reference");
				OnPropertyChanged("IsReferenceChanged");
			}
		}
		public bool IsReferenceChanged { get { return IsCurrentNameValid ? Reference != definedNameList[CurrentNameIndex].Reference : false; } }
		#endregion
		void UpdateReference() {
			if (IsCurrentNameValid)
				this.reference = definedNameList[CurrentNameIndex].Reference;
			else
				this.reference = String.Empty;
		}
		public void SubscribeDefinedNameEvents() {
			InternalAPI internalAPI = DocumentModel.InternalAPI;
			internalAPI.AfterDefinedNameRenamed += OnDefinedNameRenamed;
			internalAPI.DefinedNameWorkbookAdd += OnDefinedNameWorkbookAdd;
			internalAPI.DefinedNameWorksheetAdd += OnDefinedNameWorksheetAdd;
		}
		public void UnsubscribeDefinedNameEvents() {
			InternalAPI internalAPI = DocumentModel.InternalAPI;
			internalAPI.AfterDefinedNameRenamed -= OnDefinedNameRenamed;
			internalAPI.DefinedNameWorkbookAdd -= OnDefinedNameWorkbookAdd;
			internalAPI.DefinedNameWorksheetAdd -= OnDefinedNameWorksheetAdd;
		}
		void OnDefinedNameRenamed(object sender, AfterDefinedNameRenamedEventArgs e) {
			targetName = e.DefinedName;
		}
		void OnDefinedNameWorkbookAdd(object sender, DefinedNameWorkbookAddEventArgs e) {
			targetName = e.DefinedName;
		}
		void OnDefinedNameWorksheetAdd(object sender, DefinedNameWorksheetAddEventArgs e) {
			targetName = e.DefinedName;
		}
		public void UpdateDefinedNames() {
			int targetNameIndex = this.currentNameIndex;
			definedNameList.Clear();
			AppendWorkbookDefinedNames();
			AppendWorksheetsDefinedNames();
			AppendWorksheetsTables();
			DefineNameComparer defineNameComparer = new DefineNameComparer();
			definedNameList.Sort(defineNameComparer);
			if (targetName != null)
				targetNameIndex = definedNameList.FindIndex(MatchToTargetName);
			else
				targetNameIndex = Math.Min(targetNameIndex, definedNameList.Count - 1);
			definedNameList.RaiseCollectionChanged();
			this.currentNameIndex = targetNameIndex;
			UpdateReference();
			OnPropertyChanged("CurrentNameIndex");
			OnPropertyChanged("Reference");
			OnPropertyChanged("IsCurrentNameValid");
			OnPropertyChanged("IsReferenceChanged");
			OnPropertyChanged("IsDeleteAllowed");
			OnPropertyChanged("IsReferenceChangeAllowed");
			targetName = null;
		}
		bool MatchToTargetName(DefineNameViewModel viewModel) {
			return targetName.Name == viewModel.Name && targetName.ScopedSheetId == viewModel.SheetId;
		}
		void AppendWorkbookDefinedNames() {
			foreach (DefinedName name in DocumentModel.DefinedNames)
				if (!name.IsHidden)
					definedNameList.Add(CreateDefinedNameViewModel(name));
		}
		void AppendWorksheetsDefinedNames() {
			DocumentModel.Sheets.InnerList.ForEach(AppendWorksheetDefinedNames);
		}
		void AppendWorksheetDefinedNames(Worksheet sheet) {
			foreach (DefinedName name in sheet.DefinedNames)
				if (!name.IsHidden)
					definedNameList.Add(CreateDefinedNameViewModel(name));
		}
		void AppendWorksheetsTables() {
			DocumentModel.Sheets.InnerList.ForEach(AppendWorksheetTables);
		}
		void AppendWorksheetTables(Worksheet sheet) {
			sheet.Tables.ForEach(AppendWorksheetTable);
		}
		void AppendWorksheetTable(Table table) {
			definedNameList.Add(CreateDefinedNameViewModel(table));
		}
		DefineNameViewModel CreateDefinedNameViewModel(DefinedName name) {
			CellPosition activeCell = DocumentModel.ActiveSheet.Selection.ActiveCell;
			DefineNameViewModel result = new DefineNameViewModel(Control);
			result.OriginalName = name.Name;
			result.Name = name.Name;
			result.Reference = GetDisplayReference(name.GetReference(activeCell.Column, activeCell.Row));
			result.Scope = name.ScopedSheetId < 0 ?
				XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Scope_Workbook)
				: DocumentModel.Sheets.GetById(name.ScopedSheetId).Name;
			result.Comment = name.Comment;
			return result;
		}
		DefineNameViewModel CreateDefinedNameViewModel(Table table) {
			TableNameViewModel result = new TableNameViewModel(Control);
			result.OriginalName = table.Name;
			result.Name = table.Name;
			result.Reference = GetDisplayReference(table.Range.Worksheet.Name + "!" + table.Range.GetWithModifiedPositionType(PositionType.Absolute).ToString(DocumentModel.DataContext));
			result.Scope = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Scope_Workbook);
			result.Comment = table.Comment;
			return result;
		}
		string GetDisplayReference(string reference) {
			if (string.IsNullOrEmpty(reference) || reference[0] == '=')
				return reference;
			return string.Format("={0}", reference);
		}
		public void ApplyReferenceChange() {
			DefineNameCommand command = new DefineNameCommand(Control);
			DefineNameViewModel viewModel = definedNameList[CurrentNameIndex].Clone();
			viewModel.Reference = GetDisplayReference(command.ValidateReference(this.Reference));
			viewModel.NewNameMode = false;
			if (command.Validate(viewModel)) {
				DefinedName definedName = command.FindDefinedName(viewModel);
				command.ApplyChanges(viewModel);
				CellPosition activeCell = DocumentModel.ActiveSheet.Selection.ActiveCell;
				string updateReference = GetDisplayReference(definedName.GetReference(activeCell.Column, activeCell.Row));
				this.reference = updateReference;
				definedNameList[CurrentNameIndex].Reference = updateReference;
				definedNameList.RaiseCollectionChanged();
				OnPropertyChanged("IsReferenceChanged");
			}
		}
		public void CancelReferenceChange() {
			if (IsCurrentNameValid)
				this.Reference = definedNameList[CurrentNameIndex].Reference;
		}
		public void DeleteDefinedName() {
			DefineNameViewModel viewModel = NamesDataSource[CurrentNameIndex];
			if (!viewModel.IsDeleteAllowed)
				return;
			if (Control.InnerControl.RaiseDefinedNameDeleting(viewModel.Name, viewModel.Scope, viewModel.ScopeIndex, viewModel.Reference, viewModel.Comment))
				return;
			string message = String.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ConfirmDeleteDefinedName), viewModel.Name);
			if (!Control.ShowYesNoMessage(message))
				return;
			int sheetId = viewModel.SheetId;
			if (sheetId < 0) {
				DefinedNameBase definedName;
				if (DocumentModel.DefinedNames.TryGetItemByName(viewModel.Name, out definedName)) {
					DefinedNameWorkbookRemoveCommand command = new DefinedNameWorkbookRemoveCommand(DocumentModel, (DefinedName)definedName);
					command.Execute();
				}
			}
			else {
				Worksheet sheet = DocumentModel.Sheets.GetById(sheetId);
				DefinedNameBase definedName;
				if (sheet.DefinedNames.TryGetItemByName(viewModel.Name, out definedName)) {
					DefinedNameWorksheetRemoveCommand command = new DefinedNameWorksheetRemoveCommand(sheet, (DefinedName)definedName);
					command.Execute();
				}
			}
			UpdateDefinedNames();
		}
		public IList<string> GetAvailableDefinedNameList(Worksheet sheet) {
			List<string> result = new List<string>();
			int count = NamesDataSource.Count;
			for (int i = 0; i < count; i++) {
				DefineNameViewModel item = NamesDataSource[i];
				if (item.SheetId < 0 || item.SheetId == sheet.SheetId)
					result.Add(item.Name);
			}
			return result;
		}
	}
	#endregion
	#region DefineNameComparer
	class DefineNameComparer : IComparer<DefineNameViewModel> {
		public int Compare(DefineNameViewModel firstDefineName, DefineNameViewModel secondDefineName) {
			return String.Compare(firstDefineName.Name, secondDefineName.Name);
		}
	}
	#endregion
}
