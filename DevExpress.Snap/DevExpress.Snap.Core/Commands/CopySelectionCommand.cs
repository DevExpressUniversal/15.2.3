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
using DevExpress.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Snap.Core.Export;
using DevExpress.Snap.Core.Native;
#if !SL
using System.Windows.Forms;
using DevExpress.Snap.Core.Commands;
#else
using System.Windows;
#endif
namespace DevExpress.Snap.Core.Commands {
	#region SnapCopySelectionManager
	public class SnapCopySelectionManager : CopySelectionManager {
		public SnapCopySelectionManager(InnerRichEditDocumentServer documentServer)
			: base(documentServer) {
		}
		protected internal override void SetDataObject(PieceTable pieceTable, SelectionRangeCollection selectionCollection, IDataObject dataObject) {
			base.SetDataObject(pieceTable, selectionCollection, dataObject);
			byte[] snxBytes = GetSnxBytes(pieceTable, selectionCollection);
			if (snxBytes != null)
				dataObject.SetData(SnapDataFormats.Snx, snxBytes);
		}
		protected internal virtual byte[] GetSnxBytes(PieceTable pieceTable, SelectionRangeCollection selectionRanges) {
			return GetSnxBytes(pieceTable, selectionRanges, null);
		}
		protected internal virtual byte[] GetSnxBytes(PieceTable pieceTable, SelectionRangeCollection selectionRanges, SnapDocumentExporterOptions options) {
			using (SnapDocumentModel target = CreateDocumentModel(ParagraphNumerationCopyOptions.CopyAlways, FormattingCopyOptions.UseDestinationStyles,  pieceTable, selectionRanges, true, null) as SnapDocumentModel) {
				SubscribeTargetModelEvents(target);
				try {
					return target.InternalAPI.GetDocumentSnxContent(options);
				} 
				finally {
					UnsubscribeTargetModelEvents(target);
				}
			}
		}
	}
	#endregion
	#region CopyAndSaveSnapContentCommand
	public class CopyAndSaveSnapContentCommand : CopyAndSaveContentCommand {
		byte[] snxBytes;
		public CopyAndSaveSnapContentCommand(IRichEditControl control)
			: base(control) {
		}
		public byte[] SnxBytes { get { return snxBytes; } }
		protected internal override void CopySelectedContent(CopySelectionManager manager) {
			base.CopySelectedContent(manager);
			Selection selection = DocumentModel.Selection;
			snxBytes = ((SnapCopySelectionManager)manager).GetSnxBytes(selection.PieceTable, selection.GetSortedSelectionCollection());
		}
	}
	#endregion
	#region PasteSavedSnapContentCommand
	public class PasteSavedSnapContentCommand : PasteSnxCommand {
		readonly CopyAndSaveSnapContentCommand copyCommand;
		public PasteSavedSnapContentCommand(IRichEditControl control, CopyAndSaveSnapContentCommand copyCommand)
			: base(control) {
			Guard.ArgumentNotNull(copyCommand, "copyCommand");
			this.copyCommand = copyCommand;
			CreateInnerCommand();
		}
		protected internal override PieceTablePasteContentConvertedToDocumentModelCommandBase CreateInnerCommandCore() {
			if (copyCommand == null)
				return null;
			return new PieceTablePasteSavedSnapContentCommand((SnapPieceTable)ActivePieceTable, copyCommand);
		}
	}
	#endregion
	#region PieceTablePasteSavedSnapContentCommand
	public class PieceTablePasteSavedSnapContentCommand : PieceTablePasteSnxCommand {
		readonly CopyAndSaveSnapContentCommand copyCommand;
		readonly PieceTablePasteSavedContentCommand innerCommand;
		public PieceTablePasteSavedSnapContentCommand(SnapPieceTable pieceTable, CopyAndSaveSnapContentCommand copyCommand)
			: base(pieceTable) {
			Guard.ArgumentNotNull(copyCommand, "copyCommand");
			this.copyCommand = copyCommand;
			this.innerCommand = new PieceTablePasteSavedContentCommand(pieceTable, copyCommand);
		}
		protected internal override DocumentModel CreateSourceDocumentModel(string sizeCollection) {
			byte[] contentBytes = GetContentBytes();
			if (contentBytes != null)
				return CreateDocumentModelFromContentBytes(contentBytes, sizeCollection);
			return innerCommand.CreateSourceDocumentModel(sizeCollection);
		}
		protected internal override byte[] GetContentBytes() {
			return copyCommand.SnxBytes;
		}
		protected internal override string GetAdditionalContentString() {
			return copyCommand.SuppressStoreImageSizeCollection;
		}
		protected internal override bool IsDataAvailable() {
			return (copyCommand.SnxBytes != null) || !String.IsNullOrEmpty(copyCommand.RtfText);
		}
	}
	#endregion
}
