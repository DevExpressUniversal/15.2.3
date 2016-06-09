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
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region DefineNameViewModel
	public class DefineNameViewModel : ViewModelBase, IReferenceEditViewModel {
		#region Fields
		readonly ISpreadsheetControl control;
		readonly List<string> scopeList;
		string originalName = String.Empty;
		string name = String.Empty;
		string scope = String.Empty;
		string comment = String.Empty;
		string reference = String.Empty;
		int scopeIndex;
		bool isScopeChangeAllowed = true;
		bool newNameMode = true;
		#endregion
		public DefineNameViewModel(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.scopeList = new List<string>();
			PopulateScopeList();
		}
		#region Properties
		public ISpreadsheetControl Control { get { return control; } }
		protected internal DocumentModel DocumentModel { get { return control.InnerControl.DocumentModel; } }
		public string OriginalName {
			get { return originalName; }
			set {
				if (OriginalName == value)
					return;
				this.originalName = value;
				OnPropertyChanged("OriginalName");
			}
		}
		public string Name {
			get { return name; }
			set {
				if (Name == value)
					return;
				this.name = value;
				OnPropertyChanged("Name");
			}
		}
		public string Scope {
			get { return scope; }
			set {
				if (Scope == value)
					return;
				this.scope = value;
				this.scopeIndex = Math.Max(0, scopeList.IndexOf(value));
				OnPropertyChanged("Scope");
				OnPropertyChanged("ScopeIndex");
				OnPropertyChanged("SheetId");
			}
		}
		public int ScopeIndex {
			get { return scopeIndex; }
			set {
				if (ScopeIndex == value)
					return;
				this.scopeIndex = value;
				this.scope = scopeList[value];
				OnPropertyChanged("ScopeIndex");
				OnPropertyChanged("Scope");
				OnPropertyChanged("SheetId");
			}
		}
		public IList<string> ScopeDataSource { get { return scopeList; } }
		public string Comment {
			get { return comment; }
			set {
				if (Comment == value)
					return;
				this.comment = value;
				OnPropertyChanged("Comment");
			}
		}
		public string Reference {
			get { return reference; }
			set {
				if (Reference == value)
					return;
				this.reference = value;
				OnPropertyChanged("Reference");
			}
		}
		public bool IsScopeChangeAllowed {
			get { return isScopeChangeAllowed; }
			set {
				if (IsScopeChangeAllowed == value)
					return;
				this.isScopeChangeAllowed = value;
				OnPropertyChanged("IsScopeChangeAllowed");
			}
		}
		public bool NewNameMode {
			get { return newNameMode; }
			set {
				if (NewNameMode == value)
					return;
				this.newNameMode = value;
				OnPropertyChanged("NewNameMode");
				OnPropertyChanged("FormText");
			}
		}
		public string FormText { 
			get { 
				return NewNameMode ? 
					XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_NewDefinedNameFormTitle) : 
					XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_EditDefinedNameFormTitle); 
			} 
		}
		public int SheetId {
			get {
				if (ScopeIndex <= 0)
					return -1;
				int index = DocumentModel.Sheets.GetSheetIndexByName(Scope);
				return DocumentModel.Sheets[index].SheetId;
			}
		}
		public virtual bool IsReferenceChangeAllowed { get { return true; } }
		public virtual bool IsDeleteAllowed { get { return true; } }
		#endregion
		public virtual DefineNameViewModel Clone() {
			DefineNameViewModel clone = new DefineNameViewModel(Control);
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(DefineNameViewModel other) {
			OriginalName = other.OriginalName;
			Name = other.Name;
			ScopeIndex = other.ScopeIndex;
			Comment = other.Comment;
			Reference = other.Reference;
			IsScopeChangeAllowed = other.IsScopeChangeAllowed;
			NewNameMode = other.NewNameMode;
		}
		void PopulateScopeList() {
			scopeList.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Scope_Workbook));
			Model.WorksheetCollection sheets = DocumentModel.Sheets;
			int count = sheets.Count;
			for (int i = 0; i < count; i++)
				if (sheets[i].VisibleState == SheetVisibleState.Visible)
					scopeList.Add(sheets[i].Name);
		}
		public virtual bool Validate() {
			DefineNameCommand command = new DefineNameCommand(Control);
			return command.Validate(this);
		}
		public virtual void ApplyChanges() {
			DefineNameCommand command = new DefineNameCommand(Control);
			command.ApplyChanges(this);
		}
	}
	#endregion
	#region TableNameViewModel
	public class TableNameViewModel : DefineNameViewModel {
		public TableNameViewModel(ISpreadsheetControl control)
			: base(control) {
		}
		public override DefineNameViewModel Clone() {
 			TableNameViewModel result = new TableNameViewModel(Control);
			result.CopyFrom(this);
			return result;
		}
		public override bool Validate() {
			TableNameCommand command = new TableNameCommand(Control);
			return command.Validate(this);
		}
		public override void ApplyChanges() {
			TableNameCommand command = new TableNameCommand(Control);
			command.ApplyChanges(this);
		}
		public override bool IsReferenceChangeAllowed { get { return false; } }
		public override bool IsDeleteAllowed { get { return false; } }
	}
	#endregion
}
