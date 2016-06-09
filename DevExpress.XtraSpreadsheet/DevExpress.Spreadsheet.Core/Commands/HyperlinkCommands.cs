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
using System.Reflection;
using System.Runtime.InteropServices;
using DevExpress.Services;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Utils.Localization;
using DevExpress.Office.Localization;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using System.Diagnostics;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InsertHyperlinkContextMenuItemCommand
	public class InsertHyperlinkContextMenuItemCommand : InsertHyperlinkUICommand {
		public InsertHyperlinkContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.InsertHyperlinkContextMenuItem; } }
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_InsertHyperlink; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_InsertHyperlinkDescription; } }
		#region UpdateUIStateCore
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if((!IsHyperlinkSelected() || SelectedSomeNonMergedCells()) && IsPictureSelected() && IsNotIntervalRange())
				state.Visible &= true;
			else
				state.Visible &= false;
		}
		#endregion
		bool SelectedSomeNonMergedCells() {
			return (!ActiveSheet.Selection.IsSingleCell && !ActiveSheet.Selection.ActiveRange.IsMerged && !MergedRangeInTopLeft());
		}
		bool MergedRangeInTopLeft() {
			CellPosition topLeft = ActiveSheet.Selection.ActiveRange.TopLeft;
			return ActiveSheet.MergedCells.GetMergedCellRange(topLeft.Column, topLeft.Row) != null;
		}
		bool IsPictureSelected() {
			return ((SelectedPictureCount == 1 && string.IsNullOrEmpty(ActiveSheet.DrawingObjects[ActiveSheet.Selection.SelectedDrawingIndexes[0]].DrawingObject.TargetUri)) || SelectedPictureCount != 1);
		}
	}
	#endregion
	#region EditHyperlinkContextMenuItemCommand
	public class EditHyperlinkContextMenuItemCommand : InsertHyperlinkUICommand {
		public EditHyperlinkContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.EditHyperlinkContextMenuItem; } }
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_EditHyperlink; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_EditHyperlinkDescription; } }
		#region UpdateUIStateCore
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Enabled &= true;
			if(IsSingleOrMergedCellSelectedAndHyperlinkSelected()|| IsSingleDrawingObjectHiperlinkSelected())
				state.Visible &= true;
			else
				state.Visible &= false;
		}
		#endregion
		bool IsSingleOrMergedCellSelectedAndHyperlinkSelected() {
			SheetViewSelection selection = ActiveSheet.Selection;
			return (selection.IsSingleCell || selection.ActiveRange.IsMerged || MergedRangeInTopLeft()) && IsHyperlinkSelected();
		}
		bool IsSingleDrawingObjectHiperlinkSelected() {
			if (SelectedPictureCount != 1)
				return false;
			IDrawingObject selectedDrawing = ActiveSheet.DrawingObjects[ActiveSheet.Selection.SelectedDrawingIndexes[0]];
			return !string.IsNullOrEmpty(selectedDrawing.DrawingObject.TargetUri);
		}
		bool MergedRangeInTopLeft() {
			CellPosition topLeft = ActiveSheet.Selection.ActiveRange.TopLeft;
			return ActiveSheet.MergedCells.GetMergedCellRange(topLeft.Column, topLeft.Row) != null;
		}
	}
	#endregion
	#region RemoveHyperlinkContextMenuItemCommand
	public class RemoveHyperlinkContextMenuItemCommand : SpreadsheetMenuItemSimpleCommand {
		public RemoveHyperlinkContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.RemoveHyperlinkContextMenuItem; } }
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_RemoveHyperlink; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_RemoveHyperlinkDescription; } }
		protected override bool UseOfficeTextsAndImage { get { return true; } }
		public override string ImageName { get { return "Delete_Hyperlink"; } }
		protected int SelectedPictureCount { get { return ActiveSheet.Selection.SelectedDrawingIndexes.Count; } }
		#region UpdateUIStateCore
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default);
			if(IsSingleCellSelectedAndHyperlinkSelected() || IsSingleDrawingObjectHiperlinkSelected())
				state.Visible &= true;
			else
				state.Visible &= false;
		}
		#endregion
		bool IsSingleCellSelectedAndHyperlinkSelected() {
			return (ActiveSheet.Selection.IsSingleCell && ActiveSheet.Hyperlinks.ContainsHyperlink(ActiveSheet.Selection.ActiveRange));
		}
		bool IsSingleDrawingObjectHiperlinkSelected() {
			return (SelectedPictureCount == 1 && !string.IsNullOrEmpty(ActiveSheet.DrawingObjects[ActiveSheet.Selection.SelectedDrawingIndexes[0]].DrawingObject.TargetUri));
		}
		protected internal override void ExecuteCore() {
			List<int> indexes = GetSelectedHyperlinkIndexes();
			if (indexes.Count > 0) {
				RemoveHyperlinks(indexes);
			}
			else {
				IHyperlinkViewInfo hyperlink = ActiveSheet.DrawingObjects[ActiveSheet.Selection.SelectedDrawingIndexes[0]].DrawingObject;
				hyperlink.SetTargetUriWithoutHistory(String.Empty);
				hyperlink.DisplayText = String.Empty;
				hyperlink.IsExternal = false;
				hyperlink.TooltipText = String.Empty;
			}
		}
		protected List<int> GetSelectedHyperlinkIndexes() {
			List<int> indexes = new List<int>();
			foreach (CellRange range in ActiveSheet.Selection.SelectedRanges) {
				Predicate<CellRange> match = (hlinkRange) => { return range.Includes(hlinkRange) || hlinkRange.Includes(range); };
				List<int> hyperlinkIndexesIsSelectedRange = ActiveSheet.Hyperlinks.GetIndexesInsideRangeSortedDescending(match);
				foreach (int index in hyperlinkIndexesIsSelectedRange)
					if (!indexes.Contains(index))
						indexes.Add(index);
			}
			InverseIntComparer compaper = new InverseIntComparer();
			indexes.Sort(compaper);
			return indexes;
		}
		protected void RemoveHyperlinks(List<int> indexes) {
			Control.BeginUpdate();
			try {
				for (int i = 0; i < indexes.Count; i++)
					ActiveSheet.RemoveHyperlinkAt(indexes[i]);
			}
			finally {
				Control.EndUpdate();
			}
		}
	}
	#endregion
	#region RemoveHyperlinksContextMenuItemCommand
	public class RemoveHyperlinksContextMenuItemCommand : RemoveHyperlinkContextMenuItemCommand {
		public RemoveHyperlinksContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.RemoveHyperlinksContextMenuItem; } }
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_RemoveHyperlinks; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_RemoveHyperlinksDescription; } }
		#region UpdateUIStateCore
		protected override void UpdateUIStateCore(ICommandUIState state) {
			if(!ActiveSheet.Selection.IsSingleCell && ActiveSheet.Hyperlinks.ContainsHyperlink(ActiveSheet.Selection.SelectedRanges))
				state.Visible = true;
			else
				state.Visible = false;
		}
		#endregion
		protected internal override void ExecuteCore() {
			List<int> indexes = GetSelectedHyperlinkIndexes();
			RemoveHyperlinks(indexes);
		}
	}
	#endregion
	#region OpenHyperlinkContextMenuItemCommand
	public class OpenHyperlinkContextMenuItemCommand : OpenHyperlinkContextMenuItemCommandBase {
		ModelHyperlink hyperlink;
		public OpenHyperlinkContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.OpenHyperlinkContextMenuItem; } }
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_OpenHyperlink; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_OpenHyperlinkDescription; } }
		protected override bool UseOfficeTextsAndImage { get { return true; } }
		protected override bool IsExternal { get { return GetSelectedHyperlink().IsExternal; } }
		protected override string TargetUri { get { return GetSelectedHyperlink().TargetUri; } }
		protected override bool IsVisible { get { return IsSingleOrMergedCellSelectedAndHyperlinkSelected(); } }
		bool IsSingleOrMergedCellSelectedAndHyperlinkSelected() {
			return (ActiveSheet.Selection.IsSingleCell || ActiveSheet.Selection.ActiveRange.IsMerged || MergedRangeInTopLeft()) && (ActiveSheet.Hyperlinks.ContainsHyperlink(ActiveSheet.Selection.ActiveRange));
		}
		bool MergedRangeInTopLeft() {
			CellPosition topLeft = ActiveSheet.Selection.ActiveRange.TopLeft;
			return ActiveSheet.MergedCells.GetMergedCellRange(topLeft.Column, topLeft.Row) != null;
		}
		ModelHyperlink GetSelectedHyperlink() {
			if(hyperlink == null)
				hyperlink = ActiveSheet.Hyperlinks.GetHyperlink(ActiveSheet.Selection.ActiveRange);
			return hyperlink;
		}
		protected override CellRangeBase GetTargetRange() {
			return GetSelectedHyperlink().GetTargetRange();
		}
	}
	#endregion
	#region InverseIntComparer
	public class InverseIntComparer : IComparer<int> {
		public int Compare(int x, int y) {
			if(x > y)
				return -1;
			if(x < y)
				return 1;
			return 0;
		}
	}
	#endregion
}
