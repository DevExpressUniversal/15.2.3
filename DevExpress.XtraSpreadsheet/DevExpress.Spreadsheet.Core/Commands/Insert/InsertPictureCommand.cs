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
using DevExpress.Office.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Office.Internal;
using DevExpress.Office.Localization;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Office.Import;
using System.IO;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InsertPictureCommand
	public class InsertPictureCommand : SpreadsheetCommand {
		public InsertPictureCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.InsertPicture; } }
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_InsertFloatingObjectPicture; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_InsertFloatingObjectPictureDescription; } }
		protected override bool UseOfficeTextsAndImage { get { return true; } }
		public override string ImageName { get { return "InsertFloatingObjectImage"; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<InsertPictureCommandParameters> valueBasedState = state as IValueBasedCommandUIState<InsertPictureCommandParameters>;
				if (valueBasedState == null)
					return;
				OfficeImage image;
				if (InnerControl.AllowShowingForms)
					image = ShowOpenFileDialog();
				else
					image = ImportImage(valueBasedState.Value);
				if (image != null)
					InsertPicture(image);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		OfficeImage ShowOpenFileDialog() {
			PictureFormatsManagerService importManagerService = new PictureFormatsManagerService();
			SpreadsheetImageImportHelper importHelper = new SpreadsheetImageImportHelper(DocumentModel);
			ImportSource<OfficeImageFormat, OfficeImage> importSource = importHelper.InvokeImportDialog(Control, importManagerService);
			if (importSource == null)
				return null;
			IImporterOptions importerOptions = importSource.Importer.SetupLoading();
			OfficeImage image;
			using (Stream stream = importSource.GetStream()) {
				image = importSource.Importer.LoadDocument(DocumentModel, stream, importerOptions);
#if !SL
				image.Uri = importSource.FileName;
#endif
			}
			return image;
		}
		OfficeImage ImportImage(InsertPictureCommandParameters parameters) {
			Stream stream = parameters.Stream;
			string fileName = parameters.FileName;
			if (parameters.Stream != null)
				return ImportFromStream(stream, fileName);
			return ImportFromFile(fileName);
		}
		OfficeImage ImportFromStream(Stream stream, string fileName) {
			PictureFormatsManagerService importManagerService = new PictureFormatsManagerService();
			SpreadsheetImageImportHelper importHelper = new SpreadsheetImageImportHelper(DocumentModel);
			IImporter<OfficeImageFormat, OfficeImage> importer = importHelper.AutodetectImporter(fileName, importManagerService);
			return importHelper.Import(stream, String.Empty, importer, null);
		}
		OfficeImage ImportFromFile(string fileName) {
			PictureFormatsManagerService importManagerService = new PictureFormatsManagerService();
			SpreadsheetImageImportHelper importHelper = new SpreadsheetImageImportHelper(DocumentModel);
			OfficeImage image = importHelper.ImportFromFileAutodetectFormat(fileName, importManagerService);
#if !SL
			image.Uri = fileName;
#endif
			return image;
		}
		protected internal void InsertPicture(OfficeImage image) {
			DocumentModel.BeginUpdateFromUI();
			try {
				CellPosition activeCell = ActiveSheet.Selection.ActiveCell;
				Picture picture = ActiveSheet.InsertPicture(image, new CellKey(ActiveSheet.SheetId, activeCell.Column, activeCell.Row));
				ActiveSheet.Selection.SetSelectedDrawingIndex(picture.IndexInCollection);
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<InsertPictureCommandParameters>();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, InnerControl.DocumentModel.DocumentCapabilities.Pictures, !InnerControl.IsAnyInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.ObjectsLocked);
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region InsertPictureCommandParameters
	public class InsertPictureCommandParameters {
		public InsertPictureCommandParameters() {
		}
		public InsertPictureCommandParameters(Stream stream, string fileName) {
			Stream = stream;
			FileName = fileName;
		}
		#region Properties
		public Stream Stream { get; set; }
		public string FileName { get; set; }
		#endregion
	}
	#endregion
}
