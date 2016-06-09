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
using System.ComponentModel;
using DevExpress.Office.Localization;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InsertHyperlinkUICommand
	public class InsertHyperlinkUICommand : SpreadsheetCommand {
		public InsertHyperlinkUICommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.InsertHyperlink; } }
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_Hyperlink; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_HyperlinkDescription; } }
		protected override bool UseOfficeTextsAndImage { get { return true; } }
		public override string ImageName { get { return "Hyperlink"; } }
		protected int SelectedPictureCount { get { return ActiveSheet.Selection.SelectedDrawingIndexes.Count; } }
		public SheetViewSelection Selection { get { return ActiveSheet.Selection ; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			DocumentModel.BeginUpdateFromUI();
			try {
				IValueBasedCommandUIState<IHyperlinkViewInfo> valueBasedState = state as IValueBasedCommandUIState<IHyperlinkViewInfo>;
				if (valueBasedState == null)
					return;
				if (SelectedPictureCount == 0 && !ActiveSheet.CanEditSelection()) {
					InnerControl.ShowReadOnlyObjectMessage();
					return;
				}
				InsertHyperlinkCommand command = (InsertHyperlinkCommand)valueBasedState.Value;
				if (InnerControl.AllowShowingForms)
					Control.ShowHyperlinkForm(command, ShowHyperlinkFormCallback);
				else
					ShowHyperlinkFormCallback(command);
			}
			finally {
				DocumentModel.EndUpdateFromUI();
				NotifyEndCommandExecution(state);
			}
		}
		protected internal void ShowHyperlinkFormCallback(IHyperlinkViewInfo changedHyperlink) {
			TryAddHyperlinkToCollection(changedHyperlink);
			if (!changedHyperlink.IsDrawingObject)
				SetTextToTopLeftCell(changedHyperlink);
		}
		void TryAddHyperlinkToCollection(IHyperlinkViewInfo changedHyperlink) {
			IHyperlinkViewInfo existingHyperlink;
			SheetViewSelection selection = changedHyperlink.Workbook.ActiveSheet.Selection;
			DocumentModel.BeginUpdate();
			if (!changedHyperlink.IsDrawingObject) {
				CellRange activeRange = selection.ActiveRange;
				existingHyperlink = changedHyperlink.Worksheet.Hyperlinks.GetHyperlink(selection.ActiveRange);
				if (existingHyperlink == null)
					AddHyperlinkToCollection(changedHyperlink);
				else if (activeRange.IsEqualSurfacesWith(changedHyperlink.Range))
					ApplyChangeToExistingHyperlink(existingHyperlink, changedHyperlink);
				else
					AddHyperlinkToCollection(changedHyperlink);
			}
			else {
				existingHyperlink = changedHyperlink.Worksheet.DrawingObjects[selection.SelectedDrawingIndexes[selection.SelectedDrawingIndexes.Count - 1]].DrawingObject;
				ApplyChangeToExistingHyperlink(existingHyperlink, changedHyperlink);
			}
			DocumentModel.EndUpdate();
		}
		void ApplyChangeToExistingHyperlink(IHyperlinkViewInfo existingHyperlink, IHyperlinkViewInfo changedHyperlink) {
			if (!changedHyperlink.IsDrawingObject && existingHyperlink.DisplayText != changedHyperlink.DisplayText)
				existingHyperlink.DisplayText = changedHyperlink.DisplayText;
			if (existingHyperlink.TooltipText != changedHyperlink.TooltipText)
				existingHyperlink.TooltipText = changedHyperlink.TooltipText;
			if (existingHyperlink.TargetUri != changedHyperlink.TargetUri)
				existingHyperlink.SetTargetUriWithoutHistory(changedHyperlink.TargetUri);
			if (existingHyperlink.IsExternal != changedHyperlink.IsExternal)
				existingHyperlink.IsExternal = changedHyperlink.IsExternal;
		}
		void AddHyperlinkToCollection(IHyperlinkViewInfo changedHyperlink) {
			InsertHyperlinkCommand command = new InsertHyperlinkCommand(ErrorHandler, changedHyperlink.Worksheet, changedHyperlink.Range, 
													changedHyperlink.TargetUri, changedHyperlink.IsExternal, changedHyperlink.DisplayText, changedHyperlink.IsDrawingObject);
			command.SetDisplayTextToTopLeftCell = true;
			command.SetHyperlinkStyleToRange = true;
			if (command.Execute()) {
				ModelHyperlink hyperlink = (command.Result as ModelHyperlink);
				hyperlink.TooltipText = changedHyperlink.TooltipText;
			}
		}
		void SetTextToTopLeftCell(IHyperlinkViewInfo changedHyperlink) {
				CellRange activeRange = changedHyperlink.Workbook.ActiveSheet.Selection.ActiveRange;
				SetTextToHyperlinkTopLeftCellCommand command = new SetTextToHyperlinkTopLeftCellCommand(changedHyperlink.Worksheet, activeRange, changedHyperlink.DisplayText, changedHyperlink.TargetUri);
				command.ShouldRaiseCellValueChanged = true;
				command.Execute();
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			IValueBasedCommandUIState<IHyperlinkViewInfo> state = new DefaultValueBasedCommandUIState<IHyperlinkViewInfo>();
			state.Value = GetHyperlinkFromSelection();
			return state;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !InnerControl.IsAnyInplaceEditorActive);
			bool insertHyperlinkAllowed = !Protection.InsertHyperlinksLocked;
			if (SelectedPictureCount > 0)
				insertHyperlinkAllowed &= !Protection.ObjectsLocked;
			ApplyActiveSheetProtection(state, insertHyperlinkAllowed);
			state.Enabled &= (!ActiveSheet.Selection.IsMultiSelection && SelectedPictureCount == 0 && IsNotIntervalRange()) || SelectedPictureCount == 1;
			state.Enabled &= !ActiveSheet.PivotTables.ContainsItemsInRange(ActiveSheet.Selection.ActiveRange, true);
		}
		protected internal bool IsHyperlinkSelected() {
			return ActiveSheet.Hyperlinks.ContainsHyperlink(ActiveSheet.Selection.ActiveRange);
		}
		protected bool IsNotIntervalRange() {
			return ((!ActiveSheet.Selection.ActiveRange.IsColumnRangeInterval() && !ActiveSheet.Selection.ActiveRange.IsRowRangeInterval()) || SelectedPictureCount > 0);
		}
		public IHyperlinkViewInfo GetHyperlinkFromSelection() {
			IHyperlinkViewInfo existingHyperlink;
			int SelectedPictureCount = Selection.SelectedDrawingIndexes.Count;
			if (SelectedPictureCount == 0)
				existingHyperlink = ActiveSheet.Hyperlinks.GetHyperlink(Selection.ActiveRange);
			else
				existingHyperlink = ActiveSheet.DrawingObjects[Selection.SelectedDrawingIndexes[SelectedPictureCount - 1]].DrawingObject;
			return GetNewOrCopyHyperlink(existingHyperlink);
		}
		IHyperlinkViewInfo GetNewOrCopyHyperlink(IHyperlinkViewInfo existingHyperlink) {
			InsertHyperlinkCommand hyperlink;
			if (existingHyperlink == null)
				hyperlink = new InsertHyperlinkCommand(ErrorHandler, ActiveSheet, Selection.ActiveRange, String.Empty, true, String.Empty, false);
			else {
				hyperlink = new InsertHyperlinkCommand(ErrorHandler, existingHyperlink.Worksheet, existingHyperlink.Range, existingHyperlink.TargetUri, existingHyperlink.IsExternal, existingHyperlink.DisplayText, existingHyperlink.IsDrawingObject);
				hyperlink.DisplayText = existingHyperlink.DisplayText;
				hyperlink.TooltipText = existingHyperlink.TooltipText;
			}
			return hyperlink;
		}
	}
	#endregion
}
