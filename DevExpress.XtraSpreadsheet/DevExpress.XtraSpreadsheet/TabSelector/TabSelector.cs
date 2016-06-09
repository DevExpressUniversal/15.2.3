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
using System.Diagnostics;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using System.ComponentModel;
using DevExpress.Spreadsheet;
using DevExpress.Utils.Commands;
using System.Collections.Generic;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraSpreadsheet.Menu;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Commands;
using ModelWorksheet = DevExpress.XtraSpreadsheet.Model.Worksheet;
namespace DevExpress.XtraSpreadsheet.TabSelector {
	#region SpreadsheetTabSelector
	[DXToolboxItem(false)]
	public class SpreadsheetTabSelector : TabNavigator, ITabSelector {
		#region Fieds
		readonly SpreadsheetControl spreadsheetControl;
		bool cancelUpdate;
		#endregion
		public SpreadsheetTabSelector(SpreadsheetControl spreadsheetControl) {
			Guard.ArgumentNotNull(spreadsheetControl, "spreadsheetControl");
			this.spreadsheetControl = spreadsheetControl;
			PopulateButtons();
			SubscribeInternalEvents();
			SubscribeDocumentEvents();
		}
		#region Properties
		internal DocumentModel DocumentModel { get { return spreadsheetControl.InnerControl.DocumentModel; } }
		public override bool ForbidOperation { get { return spreadsheetControl.InnerControl.IsInplaceEditorActive; } }
		#endregion
		protected internal void SubscribeInternalEvents() {
			this.ActiveButtonChanged += OnActiveButtonChanged;
			this.AddWorksheet += OnAddWorksheet;
			this.RenameWorksheet += OnRenameWorksheet;
			SelectedButtons.ButtonInserted += OnButtonSelected;
			SelectedButtons.ButtonRemoved += OnButtonUnselected;
		}
		protected internal void UnsubscribeInternalEvents() {
			this.ActiveButtonChanged -= OnActiveButtonChanged;
			this.AddWorksheet -= OnAddWorksheet;
			this.RenameWorksheet -= OnRenameWorksheet;
			SelectedButtons.ButtonInserted -= OnButtonSelected;
			SelectedButtons.ButtonRemoved -= OnButtonUnselected;
		}
		void OnActiveButtonChanged(object sender, BaseButtonEventArgs e) {
			DocumentModel.BeginUpdateFromUI();
			try {
				DocumentModel.ActiveSheet = DocumentModel.Sheets[e.Button.Properties.Caption];
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		void OnAddWorksheet(object sender, WorksheetEventArgs args) {
			ExecuteActionWithoutInnerUpdate(AddWorksheetCore);
		}
		void AddWorksheetCore() {
			SpreadsheetCommand command = CreateAddNewWorksheetCommand();
			command.Execute();
			MoveLast();
		}
		SpreadsheetCommand CreateAddNewWorksheetCommand() {
			return spreadsheetControl.CreateCommand(SpreadsheetCommandId.AddNewWorksheet);
		}
		void OnRenameWorksheet(object sender, WorksheetEventArgs args) {
			SpreadsheetCommand command = spreadsheetControl.CreateCommand(SpreadsheetCommandId.RenameSheet);
			command.Execute();
		}
		void OnButtonSelected(object sender, WorksheetEventArgs args) {
			DocumentModel.Sheets[args.WorksheetName].IsSelected = true;
		}
		void OnButtonUnselected(object sender, WorksheetEventArgs args) {
			DocumentModel.Sheets[args.WorksheetName].IsSelected = false;
		}
		protected internal void SubscribeDocumentEvents() {
			DocumentModel.InnerActiveSheetChanged += OnActiveSheetChanged;
			DocumentModel.InnerActiveSheetTryChange += OnActiveSheetTryChange;
			DocumentModel.InnerSheetRenamed += OnSheetRenamed;
			DocumentModel.SheetVisibleStateChanged += OnSheetVisibleStateChanged;
			DocumentModel.SheetTabColorChanged += OnSheetTabColorChanged;
			DocumentModel.InternalAPI.AfterSheetRemoved += OnSheetRemoved;
			DocumentModel.InternalAPI.AfterSheetInserted += OnAfterSheetInserted;
			DocumentModel.InternalAPI.AfterWoksheetMoved += OnAfterWoksheetMoved;
			DocumentModel.ContentSetted += OnContentSetted;
			spreadsheetControl.InnerControl.UpdateUI += OnUpdateUI;
			spreadsheetControl.EnabledChanged += OnEnabledChanged;
		}
		protected internal void UnsubscribeDocumentEvents() {
			DocumentModel.InnerActiveSheetChanged -= OnActiveSheetChanged;
			DocumentModel.InnerActiveSheetTryChange -= OnActiveSheetTryChange;
			DocumentModel.InnerSheetRenamed -= OnSheetRenamed;
			DocumentModel.SheetVisibleStateChanged -= OnSheetVisibleStateChanged;
			DocumentModel.SheetTabColorChanged -= OnSheetTabColorChanged;
			DocumentModel.InternalAPI.AfterSheetRemoved -= OnSheetRemoved;
			DocumentModel.InternalAPI.AfterSheetInserted -= OnAfterSheetInserted;
			DocumentModel.InternalAPI.AfterWoksheetMoved -= OnAfterWoksheetMoved;
			DocumentModel.ContentSetted -= OnContentSetted;
			spreadsheetControl.InnerControl.UpdateUI -= OnUpdateUI;
			spreadsheetControl.EnabledChanged -= OnEnabledChanged;
		}
		void OnActiveSheetChanged(object sender, ActiveSheetChangedEventArgs e) {
			SetActiveButton();
		}
		void OnActiveSheetTryChange(object sender, EventArgs e) {
			BeginUpdate();
			try {
				MakeVisibleButton(DocumentModel.ActiveSheet.Name);
			}
			finally {
				EndUpdate();
			}
		}
		void OnSheetRenamed(object sender, SheetRenamedEventArgs e) {
			if (!String.IsNullOrEmpty(e.OldName))
				Buttons[e.OldName].Caption = e.NewName;
		}
		void OnSheetVisibleStateChanged(object sender, SheetVisibleStateChangedEventArgs e) {
			if (e.OldValue == SheetVisibleState.Visible || e.NewValue == SheetVisibleState.Visible)
				PopulateButtons();
		}
		void OnSheetTabColorChanged(object sender, SheetTabColorChangedEventArgs e) {
			TabNavigationButton button = Buttons[e.SheetName];
			Color newColor = e.NewValue;
			if (button.Color == newColor)
				return;
			button.Color = newColor;
			Refresh();
		}
		void OnSheetRemoved(object sender, EventArgs e) {
			PopulateButtons();
		}
		void OnAfterWoksheetMoved(object sender, WorksheetMovedEventArgs e) {
			PopulateButtons();
		}
		void OnAfterSheetInserted(object sender, WorksheetCollectionChangedEventArgs e) {
			PopulateButtons();
		}
		void OnContentSetted(object sender, DocumentContentSettedEventArgs e) {
			DocumentModelChangeType changeType = e.ChangeType;
			if (changeType == DocumentModelChangeType.LoadNewDocument) {
				ResetImageCache();
				PopulateButtons();
			}
			else if (changeType == DocumentModelChangeType.CreateEmptyDocument) {
				ResetImageCache();
				ExecuteActionWithoutInnerUpdate(EmptyDocumentCreatedCore);
			}
		}
		void EmptyDocumentCreatedCore() {
			PopulateButtons();
			MoveFirst();
		}
		void OnUpdateUI(object sender, EventArgs e) {
			UpdateAddNewButtonState();
		}
		void UpdateAddNewButtonState() {
			SpreadsheetCommand command = CreateAddNewWorksheetCommand();
			if (command == null)
				return;
			ICommandUIState state = command.CreateDefaultCommandUIState();
			command.UpdateUIState(state);
			IButtonProperties properties = GetAddNewWorksheetButton().Properties;
			properties.Enabled = state.Enabled;
			properties.Visible = state.Visible;
		}
		void OnEnabledChanged(object sender, EventArgs e) {
			foreach (IBaseButton button in this.Buttons)
				button.Properties.Enabled = spreadsheetControl.Enabled;
			UpdateAddNewButtonState();
		}
		IBaseButton GetAddNewWorksheetButton() {
			IBaseButton result = Buttons[Buttons.Count - 1];
			Debug.Assert(result is NewTabButton);
			return result;
		}
		protected internal void PopulateButtons() {
			BeginUpdate();
			try {
				EnsureButtons(GetTabButtonInfos());
				UpdateAddNewButtonState();
				SetActiveButton();
			}
			finally {
				if (cancelUpdate)
					CancelUpdate();
				else
					EndUpdate();
			}
		}
		protected internal List<ITabButtonInfo> GetTabButtonInfos() {
			List<ITabButtonInfo> result = new List<ITabButtonInfo>();
			foreach (ModelWorksheet sheet in DocumentModel.GetVisibleSheets()) {
				result.Add(CreateTabButtonInfo(sheet));
			}
			return result;
		}
		TabButtonInfo CreateTabButtonInfo(ModelWorksheet sheet) {
			return new TabButtonInfo(sheet);
		}
		protected override void OnPopupMenu(Point point) {
			point = spreadsheetControl.PointToClient(PointToScreen(point));
			spreadsheetControl.ShowPopupMenu(SpreadsheetMenuType.SheetTab, point);
		}
		protected internal void SetActiveButton() {
			string sheetName = DocumentModel.ActiveSheet.Name;
			MakeVisibleButton(sheetName); 
			SelectActiveButton(sheetName); 
		}
		void ExecuteActionWithoutInnerUpdate(ActionDelegate action) {
			BeginUpdate();
			try {
				this.cancelUpdate = true;
				action();
			}
			finally {
				this.cancelUpdate = false;
				EndUpdate();
			}
		}
		delegate void ActionDelegate();
		protected override void Dispose(bool disposing) {
			if (disposing) {
				UnsubscribeInternalEvents();
				UnsubscribeDocumentEvents();
			}
			base.Dispose(disposing);
		}
	}
	#endregion
	#region TabButtonInfo
	public class TabButtonInfo : ITabButtonInfo {
		#region Fields
		readonly ModelWorksheet sheet;
		#endregion
		public TabButtonInfo(ModelWorksheet sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
		}
		#region Properties
		public string Caption { get { return sheet.Name; } }
		public Color Color { get { return sheet.GetTabColor(); } }
		public bool Checked { get { return sheet.IsSelected; } }
		#endregion
	}
	#endregion
}
