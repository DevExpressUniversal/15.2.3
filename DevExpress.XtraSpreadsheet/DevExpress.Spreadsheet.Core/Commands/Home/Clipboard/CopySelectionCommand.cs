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

using DevExpress.Office.Localization;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Office.Utils;
#if !DXPORTABLE
using PlatformIndependentDataObject = System.Windows.Forms.DataObject;
using System.Windows.Forms;
#else
using PlatformIndependentDataObject = DevExpress.Compatibility.System.Windows.Forms.DataObject;
using DevExpress.Compatibility.System.Windows.Forms;
#endif
namespace DevExpress.XtraSpreadsheet.Commands {
	public class CopySelectionCommand : SpreadsheetMenuItemSimpleCommand {
		public CopySelectionCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_CopySelection; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_CopySelectionDescription; } }
		protected override bool UseOfficeTextsAndImage { get { return true; } }
		public override string ImageName { get { return "Copy"; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.CopySelection; } }
		protected internal override void ExecuteCore() {
			if (InnerControl.IsInplaceEditorActive)
				InnerControl.InplaceEditor.Copy();
			else {
				if (DocumentModel.ActiveSheet.Selection.SelectedDrawingIndexes.Count > 0)
					CopyDrawings();
				else if (DocumentModel.ActiveSheet.Selection.SelectedRanges.Count > 0)
					CopySelectedRanges();
			}
		}
		void CopySelectedRanges() {
			CellRange range = DocumentModel.ActiveSheet.Selection.SelectedRanges[0];
			DocumentModel.CopyCutRangeToClipboard(range, false);
		}
		void CopyDrawings() {
#if !SL && !DXPORTABLE
			DocumentModel.ClearCopiedRange();
			Worksheet sheet = DocumentModel.ActiveSheet;
			List<int> indices = sheet.Selection.SelectedDrawingIndexes;
			int count = indices.Count;
			if (DocumentModel.RaiseShapesCopying())
				return;
			var clipboard = DocumentModel.GetService<IClipboardProvider>();
			IDataObject dataObject = new PlatformIndependentDataObject();
			for (int i = 0; i < count; i++) {
				int drawingObjectIndex = indices[i];
				IDrawingObject drawing = sheet.DrawingObjects[drawingObjectIndex];
				if (drawing.DrawingType == DrawingObjectType.Picture) {
					Picture picture = drawing as Picture;
					dataObject.SetData(OfficeDataFormats.Bitmap, picture.Image.NativeImage);
				}
			}
			clipboard.Clear();
			clipboard.SetDataObject(dataObject, true);
#endif
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			if (InnerControl.IsInplaceEditorActive) {
				state.Visible = true;
				state.Enabled = InnerControl.InplaceEditor.CanCopy();
				state.Checked = false;
			}
			else {
				bool isSingleSelection = DocumentModel.ActiveSheet.Selection.SelectedRanges.Count == 1 || DocumentModel.ActiveSheet.Selection.SelectedDrawingIndexes.Count == 1;
				state.Checked = false;
				ApplyCommandsRestriction(state, Options.InnerBehavior.Copy, isSingleSelection);
			}
		}
	}
}
