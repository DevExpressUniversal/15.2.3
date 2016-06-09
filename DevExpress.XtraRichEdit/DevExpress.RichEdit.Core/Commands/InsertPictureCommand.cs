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
using System.Drawing;
using System.IO;
using DevExpress.Office;
using DevExpress.Office.Import;
using DevExpress.Office.Internal;
using DevExpress.Office.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Import;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Windows.Forms;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region InsertPictureCommand
	public class InsertPictureCommand : TransactedInsertObjectCommand {
		public InsertPictureCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertPictureCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertPicture; } }
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new InsertPictureCoreCommand(Control);
		}
		protected internal override void ForceExecuteCore(ICommandUIState state) {
			PictureFormatsManagerService importManagerService = new PictureFormatsManagerService();
			RichEditImageImportHelper importHelper = new RichEditImageImportHelper(DocumentModel);
			ImportSource<OfficeImageFormat, OfficeImage> importSource = importHelper.InvokeImportDialog(Control, importManagerService);
			if (importSource == null)
				return;
			InsertPictureCoreCommand insertCommand = (InsertPictureCoreCommand)Commands[1];
			insertCommand.ImportSource = importSource;
			if (insertCommand.EnsureImageLoaded())
				base.ForceExecuteCore(state);
			else
				Control.ShowErrorMessage(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_InvalidImageFile));
		}
		public override void UpdateUIState(ICommandUIState state) {
			base.UpdateUIState(state);
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.InlinePictures, state.Enabled);
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region InsertPictureCoreCommand
	public class InsertPictureCoreCommand : InsertObjectCommandBase {
		ImportSource<OfficeImageFormat, OfficeImage> importSource;
		OfficeImage image;
		public InsertPictureCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertPicture; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertPictureDescription; } }
		public override string ImageName { get { return "InsertImage"; } }
		protected internal ImportSource<OfficeImageFormat, OfficeImage> ImportSource { get { return importSource; } set { importSource = value; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.Column; } }
		#endregion
		protected internal bool EnsureImageLoaded() {
			if (image != null)
				return true;
			try {
				IImporterOptions importerOptions = importSource.Importer.SetupLoading();
				using (Stream stream = importSource.GetStream()) {
					image = importSource.Importer.LoadDocument(DocumentModel, stream, importerOptions);
#if !SL
					image.Uri = importSource.FileName;
#endif
				}
			}
			catch {
				return false;
			}
			return true;
		}
		protected internal override void ModifyModel() {
			if (importSource == null)
				return;
			if (!EnsureImageLoaded())
				return;
			InsertPicture();
		}
		void InsertPicture() {
			InsertPictureInnerCommand command = Control.CreateCommand(RichEditCommandId.InsertPictureInner) as InsertPictureInnerCommand;
			command.StartPosition = DocumentModel.Selection.End;
			command.NewImage = image;
			command.Scale = CalculateImageScale(image); 
			command.Execute();
		}
		protected internal int CalculateImageScale(OfficeImage image) {
			if(ActiveView is SimpleView)
				return 100;
			return CalculateImageScale(image, CaretPosition.LayoutPosition);
		}
		protected internal static int CalculateImageScale(OfficeImage image, DocumentLayoutPosition pos) {
			if (!pos.IsValid(DocumentLayoutDetailsLevel.Column))
				return 100;
			DocumentModelUnitToLayoutUnitConverter unitConverter = pos.DocumentModel.ToDocumentLayoutUnitConverter;
			Rectangle columnBounds = pos.Column.Bounds;
			if (!pos.PieceTable.IsMain)
				columnBounds.Height = Math.Max(columnBounds.Height, pos.Page.ClientBounds.Height / 3);
			Size imageSize = image.SizeInTwips;
			int scaleX = 100 * unitConverter.ToModelUnits(columnBounds.Width) / Math.Max(1, imageSize.Width);
			int scaleY = 100 * unitConverter.ToModelUnits(columnBounds.Height) / Math.Max(1, imageSize.Height);
			int scale = Math.Min(scaleX, scaleY);
			return Math.Max(1, Math.Min(scale, 100));
		}
	}
	#endregion
	#region InsertPictureInnerCommand
	public class InsertPictureInnerCommand : RichEditMenuItemSimpleCommand {
		public InsertPictureInnerCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertPictureInner; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public virtual DocumentLogPosition StartPosition { get; set; }
		public virtual OfficeImage NewImage { get; set; }
		public virtual int Scale { get; set; }
		#endregion
		protected internal override void ExecuteCore() {
			ActivePieceTable.InsertInlinePicture(StartPosition, NewImage, Scale, Scale, GetForceVisible());
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable;
			state.Visible = true;
		}
	}
	#endregion
}
