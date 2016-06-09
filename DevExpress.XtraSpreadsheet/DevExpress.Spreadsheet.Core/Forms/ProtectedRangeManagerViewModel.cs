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
using System.Collections.ObjectModel;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region ProtectedRangeManagerViewModel
	public class ProtectedRangeManagerViewModel : ViewModelBase {
		readonly ISpreadsheetControl control;
		readonly DocumentModel documentModel;
		readonly ObservableCollection<ProtectedRangeViewModel> protectedRanges;
		int currentRangeIndex;
		bool hasPendingChanges;
		public ProtectedRangeManagerViewModel(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.documentModel = control.InnerControl.DocumentModel;
			this.protectedRanges = new ObservableCollection<ProtectedRangeViewModel>();
			UpdateProtectedRanges();
		}
		#region Properties
		DocumentModel DocumentModel { get { return documentModel; } }
		public ISpreadsheetControl Control { get { return control; } }
		public ObservableCollection<ProtectedRangeViewModel> ProtectedRangesDataSource { get { return protectedRanges; } }
		public int CurrentRangeIndex {
			get { return currentRangeIndex; }
			set {
				if (CurrentRangeIndex == value)
					return;
				this.currentRangeIndex = value;
				OnPropertyChanged("CurrentRangeIndex");
				OnPropertyChanged("IsCurrentRangeValid");
				OnPropertyChanged("CurrentRange");
			}
		}
		public bool IsCurrentRangeValid { get { return CurrentRangeIndex >= 0 && CurrentRangeIndex < protectedRanges.Count; } }
		public bool HasPendingChanges {
			get { return hasPendingChanges; }
			set {
				if (HasPendingChanges == value)
					return;
				this.hasPendingChanges = value;
				OnPropertyChanged("HasPendingChanges");
			}
		}
		public ProtectedRangeViewModel CurrentRange {
			get {
				if (IsCurrentRangeValid)
					return ProtectedRangesDataSource[CurrentRangeIndex];
				else
					return null;
			}
		}
		#endregion
		public void UpdateProtectedRanges() {
			protectedRanges.Clear();
			AppendActiveWorksheetProtectedRanges();
			OnPropertyChanged("RangesDataSource");
			OnPropertyChanged("CurrentRangeIndex");
			OnPropertyChanged("IsCurrentRangeValid");
			OnPropertyChanged("CurrentRange");
		}
		void AppendActiveWorksheetProtectedRanges() {
			ModelProtectedRangeCollection ranges = DocumentModel.ActiveSheet.ProtectedRanges;
			int count = ranges.Count;
			for (int i = 0; i < count; i++)
				this.protectedRanges.Add(CreateViewModel(ranges[i]));
		}
		public ProtectedRangeViewModel CreateNewRangeViewModel() {
			ProtectedRangeViewModel result = new ProtectedRangeViewModel(Control);
			result.IsNew = true;
			result.IsModified = false;
			result.IsDeleted = false;
			result.Title = GenerateNewProtectedRangeName();
			result.Reference = DefineNameCommand.CreateSelectionReferenceString(DocumentModel.ActiveSheet);
			result.SecurityDescriptor = String.Empty;
			result.Password = String.Empty;
			result.HasPassword = false;
			PopulateExistingProtectedRangeNames(result.ExistingProtectedRangeNames, result.Title);
			return result;
		}
		public ProtectedRangeViewModel CreateEditCurrentRangeViewModel() {
			ProtectedRangeViewModel result = CurrentRange.Clone();
			result.IsModified = true;
			PopulateExistingProtectedRangeNames(result.ExistingProtectedRangeNames, result.Title);
			return result;
		}
		void PopulateExistingProtectedRangeNames(IList<string> names, string excludeName) {
			names.Clear();
			int count = ProtectedRangesDataSource.Count;
			for (int i = 0; i < count; i++) {
				if (ProtectedRangesDataSource[i].Title != excludeName)
					names.Add(ProtectedRangesDataSource[i].Title);
			}
		}
		ProtectedRangeViewModel CreateViewModel(ModelProtectedRange range) {
			ProtectedRangeViewModel result = new ProtectedRangeViewModel(Control);
			result.ProtectedRange = range;
			result.Title = range.Name;
			result.Reference = range.CellRange.ToString(DocumentModel.DataContext);
			result.IsNew = false;
			result.IsModified = false;
			result.IsDeleted = false;
			result.Password = String.Empty;
			result.SecurityDescriptor = range.SecurityDescriptor;
			result.UpdateHasPassword();
			result.PasswordChanged = false;
			PopulateExistingProtectedRangeNames(result.ExistingProtectedRangeNames, result.Title);
			return result;
		}
		public string GenerateNewProtectedRangeName() {
			int index = 1;
			for (; ; index++) {
				string name = "Range" + index.ToString();
				if (LookupProtectedRangeByName(name) == null)
					return name;
			}
		}
		public ProtectedRangeViewModel LookupProtectedRangeByName(string name) {
			int count = ProtectedRangesDataSource.Count;
			for (int i = 0; i < count; i++)
				if (ProtectedRangesDataSource[i].Title == name)
					return ProtectedRangesDataSource[i];
			return null;
		}
		public void DeleteCurrentProtectedRange() {
			CurrentRange.IsDeleted = true;
			OnPropertyChanged("RangesDataSource");
			OnPropertyChanged("CurrentRangeIndex");
			OnPropertyChanged("IsCurrentRangeValid");
			OnPropertyChanged("CurrentRange");
		}
		public void ApplyChanges() {
			for (int i = ProtectedRangesDataSource.Count - 1; i >= 0; i--)
				if (ProcessProtectedRange(ProtectedRangesDataSource[i]))
					ProtectedRangesDataSource.RemoveAt(i);
		}
		public void ApplyRangeChange(ProtectedRangeViewModel rangeViewModel) {
			if (rangeViewModel.IsModified && IsCurrentRangeValid) {
				CurrentRange.CopyFrom(rangeViewModel);
				CurrentRange.UpdateHasPassword();
			}
			else if (rangeViewModel.IsNew) {
				ProtectedRangesDataSource.Add(rangeViewModel);
				rangeViewModel.UpdateHasPassword();
			}
			OnPropertyChanged("RangesDataSource");
			OnPropertyChanged("CurrentRangeIndex");
			OnPropertyChanged("IsCurrentRangeValid");
			OnPropertyChanged("CurrentRange");
		}
		bool ProcessProtectedRange(ProtectedRangeViewModel viewModel) {
			if (viewModel.IsDeleted)
				return !ProcessDeletedProtectedRange(viewModel);
			else if (viewModel.IsNew)
				return ProcessNewProtectedRange(viewModel);
			else if (viewModel.IsModified)
				return ProcessModifiedProtectedRange(viewModel);
			else
				return true;
		}
		bool ProcessNewProtectedRange(ProtectedRangeViewModel viewModel) {
			CellRangeBase range = CellRange.TryParse(viewModel.Reference, DocumentModel.DataContext);
			if (range == null)
				return false;
			ModelProtectedRange protectedRange = new ModelProtectedRange(viewModel.Title, range);
			ApplyCredentials(protectedRange, viewModel);
			protectedRange.SecurityDescriptor = viewModel.SecurityDescriptor;
			DocumentModel.ActiveSheet.ProtectedRanges.Add(protectedRange);
			viewModel.ProtectedRange = protectedRange;
			MarkAsApplied(viewModel);
			return true;
		}
		bool ProcessModifiedProtectedRange(ProtectedRangeViewModel viewModel) {
			CellRangeBase range = CellRange.TryParse(viewModel.Reference, DocumentModel.DataContext);
			if (range == null)
				return false;
			ModelProtectedRange protectedRange = viewModel.ProtectedRange;
			protectedRange.Name = viewModel.Title;
			protectedRange.CellRange = range;
			protectedRange.SecurityDescriptor = viewModel.SecurityDescriptor;
			if (viewModel.PasswordChanged)
				ApplyCredentials(protectedRange, viewModel);
			MarkAsApplied(viewModel);
			return true;
		}
		bool ProcessDeletedProtectedRange(ProtectedRangeViewModel viewModel) {
			if (viewModel.ProtectedRange == null)
				return true;
			int index = DocumentModel.ActiveSheet.ProtectedRanges.IndexOf(viewModel.ProtectedRange);
			if (index < 0)
				return true;
			DocumentModel.ActiveSheet.ProtectedRanges.RemoveAt(index);
			viewModel.ProtectedRange = null;
			MarkAsApplied(viewModel);
			return true;
		}
		void ApplyCredentials(ModelProtectedRange protectedRange, ProtectedRangeViewModel viewModel) {
			if (String.IsNullOrEmpty(viewModel.Password))
				protectedRange.Credentials = ProtectionCredentials.NoProtection;
			else {
				SpreadsheetProtectionOptions options = DocumentModel.ProtectionOptions;
				protectedRange.Credentials = new ProtectionCredentials(viewModel.Password, false, options.UseStrongPasswordVerifier, options.SpinCount);
			}
			viewModel.Password = String.Empty;
			viewModel.PasswordChanged = false;
		}
		void MarkAsApplied(ProtectedRangeViewModel viewModel) {
			viewModel.IsNew = false;
			viewModel.IsModified = false;
			viewModel.IsDeleted = false;
			viewModel.OriginalTitle = viewModel.Title;
			viewModel.UpdateHasPassword();
		}
		public void EditCurrentRangePermissions() {
			if (!IsCurrentRangeValid)
				return;
			ProtectedRangePermissionsViewModel permissionsViewModel = new ProtectedRangePermissionsViewModel(Control);
			permissionsViewModel.Name = CurrentRange.Title;
			permissionsViewModel.SecurityDescriptor = CurrentRange.SecurityDescriptor;
			Control.ShowProtectedRangePermissionsForm(permissionsViewModel);
			CurrentRange.IsModified = true;
			CurrentRange.SecurityDescriptor = permissionsViewModel.SecurityDescriptor;
		}
	}
	#endregion
}
