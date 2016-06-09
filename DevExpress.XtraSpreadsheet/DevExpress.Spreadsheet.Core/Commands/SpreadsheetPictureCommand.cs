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
using DevExpress.Utils.Commands;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using System.ComponentModel;
using DevExpress.Office.Localization;
using System.Diagnostics;
using System.Text.RegularExpressions;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region SpreadsheetDrawingCommand (abstract class)
	public abstract class SpreadsheetDrawingCommand : SpreadsheetMenuItemSimpleCommand {
		#region Fields
		protected internal const int OffsetInPixels = 1;
		protected internal const float LargeScale = 1.1f;
		protected internal const float SmallScale = 1.01f;
		const int largeAngleInDegrees = 15;
		const int smallAngleInDegrees = 1;
		#endregion
		protected SpreadsheetDrawingCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		protected abstract DocumentCapability Capability { get; }
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdate();
			try {
				List<IDrawingObject> selectedDrawings = GetSelectedDrawings();
				foreach (IDrawingObject drawing in selectedDrawings)
					ModifyDrawing(drawing);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal List<IDrawingObject> GetSelectedDrawings() {
			List<IDrawingObject> result = new List<IDrawingObject>();
			List<int> pictureIndexes = ActiveSheet.Selection.SelectedDrawingIndexes;
			int count = pictureIndexes.Count;
			for (int i = 0; i < count; i++) {
				int index = pictureIndexes[i];
				result.Add(ActiveSheet.DrawingObjects[index]);
			}
			return result;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestriction(state);
			ApplyActiveSheetProtection(state, !Protection.ObjectsLocked);
		}
		protected virtual void ApplyCommandRestriction(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Capability, !InnerControl.IsInplaceEditorActive && ActiveSheet.Selection.IsDrawingSelected);
		}
		protected internal virtual int GetLargeAngleInModelUnits() {
			return DocumentModel.UnitConverter.DegreeToModelUnits(largeAngleInDegrees);
		}
		protected internal virtual int GetSmallAngleInModelUnits() {
			return DocumentModel.UnitConverter.DegreeToModelUnits(smallAngleInDegrees);
		}
		protected internal abstract void ModifyDrawing(IDrawingObject drawing);
	}
	#endregion
	#region OpenHyperlinkContextMenuItemCommandBase
	public abstract class OpenHyperlinkContextMenuItemCommandBase : SpreadsheetMenuItemSimpleCommand {
		protected OpenHyperlinkContextMenuItemCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected int SelectedPictureCount { get { return ActiveSheet.Selection.SelectedDrawingIndexes.Count; } }
		protected abstract bool IsVisible { get; }
		protected abstract bool IsExternal { get; }
		protected abstract string TargetUri { get; }
		#region UpdateUIStateCore
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = IsVisible;
		}
		#endregion
		protected internal override void ExecuteCore() {
			HyperlinkClickEventArgs args = CreateHyperlinkClickEventArgs();
			if (InnerControl.RaiseHyperlinkClick(args))
				return;
			if(!IsExternal)
				TryOpenInternalHyperlink();
			else
				TryOpenExternalHyperlink();
		}
		HyperlinkClickEventArgs CreateHyperlinkClickEventArgs() {
			HyperlinkClickEventArgs result = new HyperlinkClickEventArgs(InnerControl, DevExpress.Utils.KeyboardHandler.KeyboardHandler.GetModifierKeys());
			result.IsExternal = IsExternal;
			result.TargetUri = TargetUri;
			result.ModelTargetRange = GetTargetRange();
			return result;
		}
		[System.Security.SecuritySafeCritical]
		void TryOpenExternalHyperlink() {
			try {
				string targetUri = TargetUri;
				if (HyperlinkExpressionParser.IsFilePath(targetUri)) {
					targetUri = HyperlinkExpressionParser.GetAbsoluteFilePath(targetUri, DocumentModel);
					int index = targetUri.IndexOf("#");
					if (index > 0)
						targetUri = targetUri.Remove(index);
				}
#if !DXPORTABLE
#if !SL
				using(Process process = new Process()) {
					process.StartInfo.FileName = targetUri;
					process.Start();
				}
#else
				new DevExpress.Xpf.Core.HyperlinkNavigator().Navigate(targetUri);
#endif
#endif
			}
			catch {
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_InvalidFilePath));
			}
		}
		void TryOpenInternalHyperlink() {
			try {
				Control.Document.Model.DocumentModel.BeginUpdate();
				CellRangeBase selectedRange = GetTargetRange();
				if(selectedRange == null)
					Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorInvalidReference));
				else {
					Control.Document.Model.DocumentModel.ActiveSheet = (Worksheet)selectedRange.Worksheet;
					ActiveSheet.Selection.SetSelection(selectedRange);
					Control.InnerControl.ScrollToTarget(selectedRange.TopLeft);
				}
			}
			catch(Exception) {
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorInvalidReference));
			}
			finally {
				Control.Document.Model.DocumentModel.EndUpdate();
			}
		}
		protected abstract CellRangeBase GetTargetRange();
	}
#endregion
#region OpenPictureHyperlinkContextMenuItemCommand
	public class OpenPictureHyperlinkContextMenuItemCommand : OpenHyperlinkContextMenuItemCommandBase {
		IHyperlinkViewInfo iModelHyperlink;
		public OpenPictureHyperlinkContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.OpenPictureHyperlinkContextMenuItem; } }
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_OpenHyperlink; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_OpenHyperlinkDescription; } }
		protected override bool UseOfficeTextsAndImage { get { return true; } }
		public IHyperlinkViewInfo IModelHyperlink { get { return iModelHyperlink; } set { iModelHyperlink = value; ; } }
		public override string ImageName { get { return "OpenHyperlink"; } }
		protected override bool IsVisible { get { return GetSelectedDrawingObject() != null && !String.IsNullOrEmpty(GetSelectedDrawingObject().TargetUri) && SelectedPictureCount < 2; } }
		protected override bool IsExternal { get { return GetSelectedDrawingObject().IsExternal; } }
		protected override string TargetUri { get { return GetSelectedDrawingObject().TargetUri; } }
#endregion
		IHyperlinkViewInfo GetSelectedDrawingObject() {
			if(iModelHyperlink == null && SelectedPictureCount != 0)
				iModelHyperlink = ActiveSheet.DrawingObjects[ActiveSheet.Selection.SelectedDrawingIndexes[SelectedPictureCount - 1]].DrawingObject;
			return iModelHyperlink;
		}
		protected override CellRangeBase GetTargetRange() {
			return HyperlinkExpressionParser.GetTargetRange(ActiveSheet.DataContext, iModelHyperlink.Expression, iModelHyperlink.IsExternal);
		}
	}
#endregion
}
