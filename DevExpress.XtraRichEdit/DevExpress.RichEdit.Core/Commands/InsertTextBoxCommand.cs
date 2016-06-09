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
using System.Drawing;
using System.IO;
using DevExpress.Office.Import;
using DevExpress.Office.Internal;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Import;
using DevExpress.Office.Utils;
using DevExpress.Office.Services.Implementation;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Commands {
	#region InsertFloatingObjectCommandBase (abstract class)
	public abstract class InsertFloatingObjectCommandBase : TransactedInsertObjectCommand {
		protected InsertFloatingObjectCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal override Command CreateDeleteCommand() {
			return new MoveSelectionToBeginOfParagraph(Control);
		}
	}
	#endregion
	#region InsertTextBoxCommand
	public class InsertTextBoxCommand : InsertFloatingObjectCommandBase {
		public InsertTextBoxCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertTextBoxCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertTextBox; } }
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new InsertFloatingObjectTextBoxCoreCommand(Control);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (!Control.InnerControl.DocumentModel.ActivePieceTable.CanContainCompositeContent()) 
				state.Enabled = false;
		}
	}
	#endregion
	#region InsertFloatingObjectPictureCommand
	public class InsertFloatingObjectPictureCommand : InsertFloatingObjectCommandBase {
		public InsertFloatingObjectPictureCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertFloatingObjectPictureCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertFloatingPicture; } }
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new InsertFloatingObjectPictureCoreCommand(Control);
		}
		protected internal override void ForceExecuteCore(ICommandUIState state) {
			PictureFormatsManagerService importManagerService = new PictureFormatsManagerService();
			RichEditImageImportHelper importHelper = new RichEditImageImportHelper(DocumentModel);
			ImportSource<OfficeImageFormat, OfficeImage> importSource = importHelper.InvokeImportDialog(Control, importManagerService);
			if (importSource == null)
				return;
			InsertFloatingObjectPictureCoreCommand insertCommand = (InsertFloatingObjectPictureCoreCommand)Commands[1];
			insertCommand.ImportSource = importSource;
			if (insertCommand.EnsureImageLoaded())
				base.ForceExecuteCore(state);
			else
				Control.ShowErrorMessage(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_InvalidImageFile));
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (!Control.InnerControl.DocumentModel.ActivePieceTable.CanContainCompositeContent()) 
				state.Enabled = false;
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region InsertFloatingObjectCoreCommandBase (abstract class)
	public abstract class InsertFloatingObjectCoreCommandBase : InsertObjectCommandBase {
		protected InsertFloatingObjectCoreCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.Page; } }
		protected internal override void ModifyModel() {
			Selection selection = DocumentModel.Selection;
			DocumentLogPosition pos = ActivePieceTable.Paragraphs[selection.Interval.End.ParagraphIndex].LogPosition;
			ActivePieceTable.InsertFloatingObjectAnchor(pos);
			FloatingObjectAnchorRun run = ActivePieceTable.LastInsertedFloatingObjectAnchorRunInfo.Run;
			FloatingObjectContent content = CreateDefaultFloatingObjectContent(run);
			run.SetContent(content);
			Shape shape = run.Shape;
			shape.BeginInit();
			try {
				SetupShape(shape, run);
			}
			finally {
				shape.EndInit();
			}
			FloatingObjectProperties floatingObjectProperties = run.FloatingObjectProperties;
			floatingObjectProperties.BeginInit();
			try {
				SetupFloatingObjectProperties(floatingObjectProperties, run);
			}
			finally {
				floatingObjectProperties.EndInit();
			}
		}
		protected int GetZOrder() {
			List<IZOrderedObject> floatingObjectList = ActivePieceTable.GetFloatingObjectList();
			if (floatingObjectList.Count == 0)
				return 251658240;
			else {
				int maxIndex = floatingObjectList.Count - 1;
				return floatingObjectList[maxIndex].ZOrder + 1024;
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (state.Enabled)
				state.Enabled = !ActivePieceTable.IsTextBox;
#if !DEBUGTEST
#endif
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.FloatingObjects, state.Enabled);
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
		protected internal abstract void SetupFloatingObjectProperties(FloatingObjectProperties properties, FloatingObjectAnchorRun run);
		protected internal abstract void SetupShape(Shape shape, FloatingObjectAnchorRun run);
		protected internal abstract FloatingObjectContent CreateDefaultFloatingObjectContent(FloatingObjectAnchorRun run);
	}
	#endregion
	#region InsertFloatingObjectTextBoxCoreCommand
	public class InsertFloatingObjectTextBoxCoreCommand : InsertFloatingObjectCoreCommandBase {
		public InsertFloatingObjectTextBoxCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTextBox; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTextBoxDescription; } }
		public override string ImageName { get { return "InsertTextBox"; } }
		#endregion
		protected internal override void SetupFloatingObjectProperties(FloatingObjectProperties properties, FloatingObjectAnchorRun run) {
			properties.ActualSize = DocumentModel.UnitConverter.DocumentsToModelUnits(new Size(600, 300));
			properties.HorizontalPositionType = FloatingObjectHorizontalPositionType.Column;
			properties.HorizontalPositionAlignment = FloatingObjectHorizontalPositionAlignment.Center;
			properties.VerticalPositionType = FloatingObjectVerticalPositionType.Paragraph;
			properties.ZOrder = GetZOrder();
		}
		protected internal override void SetupShape(Shape shape, FloatingObjectAnchorRun run) {
			shape.FillColor = DXColor.White;
			shape.OutlineColor = DXColor.Black;
			shape.OutlineWidth = DocumentModel.UnitConverter.TwipsToModelUnits(10);
		}
		protected internal override FloatingObjectContent CreateDefaultFloatingObjectContent(FloatingObjectAnchorRun run) {
			TextBoxContentType textBoxContentType = new TextBoxContentType(DocumentModel);
			TextBoxFloatingObjectContent content = new TextBoxFloatingObjectContent(run, textBoxContentType);
			content.TextBoxProperties.ResizeShapeToFitText = true;
			IThreadSyncService service = DocumentModel.GetService<IThreadSyncService>();
			if (service != null) {
				Action action = delegate() {
					ChangeActivePieceTableCommand command = new ChangeActivePieceTableCommand(Control, textBoxContentType.PieceTable, null, CaretPosition.PreferredPageIndex);
					command.Execute();
					command = null;
				};
				service.EnqueueInvokeInUIThread(action);
			}
			return content;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (!Control.InnerControl.DocumentModel.ActivePieceTable.CanContainCompositeContent())
				state.Enabled = false;
		}
	}
	#endregion
	#region InsertFloatingObjectPictureCoreCommand
	public class InsertFloatingObjectPictureCoreCommand : InsertFloatingObjectCoreCommandBase {
		ImportSource<OfficeImageFormat, OfficeImage> importSource;
		OfficeImage image;
		public InsertFloatingObjectPictureCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertFloatingObjectPicture; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertFloatingObjectPictureDescription; } }
		public override string ImageName { get { return "InsertFloatingObjectImage"; } }
		protected internal ImportSource<OfficeImageFormat, OfficeImage> ImportSource { get { return importSource; } set { importSource = value; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.Column; } }
		#endregion
		protected internal override void ModifyModel() {
			if (importSource == null)
				return;
			if(EnsureImageLoaded())
				base.ModifyModel();
		}
		protected internal override void SetupFloatingObjectProperties(FloatingObjectProperties properties, FloatingObjectAnchorRun run) {
			PictureFloatingObjectContent content = (PictureFloatingObjectContent)run.Content;
			IRectangularScalableObject rectangularObject = run;
			int scale = CalculateImageScale(content.Image);
			rectangularObject.ScaleX = scale;
			rectangularObject.ScaleY = scale;
			properties.HorizontalPositionType = FloatingObjectHorizontalPositionType.Column;
			properties.HorizontalPositionAlignment = FloatingObjectHorizontalPositionAlignment.Center;
			properties.VerticalPositionType = FloatingObjectVerticalPositionType.Paragraph;
			properties.LockAspectRatio = true;
			properties.ZOrder = GetZOrder();
		}
		private int CalculateImageScale(OfficeImage image) {
			if(ActiveView is SimpleView)
				return 100;
			return InsertPictureCoreCommand.CalculateImageScale(image, CaretPosition.LayoutPosition);
		}
		protected internal override void SetupShape(Shape shape, FloatingObjectAnchorRun run) {
		}
		protected internal virtual bool EnsureImageLoaded() {
			if (importSource == null)
				return false;
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
				return true;
			}
			catch {
				return false;
			}
		}
		protected internal override FloatingObjectContent CreateDefaultFloatingObjectContent(FloatingObjectAnchorRun run) {			
			return new PictureFloatingObjectContent(run, image);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (!Control.InnerControl.DocumentModel.ActivePieceTable.CanContainCompositeContent())
				state.Enabled = false;
		}
	}
	#endregion
	#region MoveSelectionToBeginOfParagraph
	public class MoveSelectionToBeginOfParagraph : RichEditSelectionCommand {
		public MoveSelectionToBeginOfParagraph(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		#endregion
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return ActivePieceTable.Paragraphs[pos.ParagraphIndex].LogPosition;
		}
	}
	#endregion
	#region ShrinkSelectionToStart
	public class ShrinkSelectionToStartCommand : RichEditSelectionCommand {
		public ShrinkSelectionToStartCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return true; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		#endregion
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
	}
	#endregion
}
