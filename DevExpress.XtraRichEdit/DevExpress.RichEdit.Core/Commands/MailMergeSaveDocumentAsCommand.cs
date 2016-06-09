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
using System.Collections;
using System.ComponentModel;
using DevExpress.Office.Internal;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Internal;
namespace DevExpress.XtraRichEdit.Commands {
	public class MailMergeSaveDocumentAsCommand : RichEditMenuItemSimpleCommand {
		public MailMergeSaveDocumentAsCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("MailMergeSaveDocumentAsCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_MailMergeSaveDocumentAsCommand; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("MailMergeSaveDocumentAsCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_MailMergeSaveDocumentAsCommandDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("MailMergeSaveDocumentAsCommandImageName")]
#endif
		public override string ImageName { get { return "MailMerge"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("MailMergeSaveDocumentAsCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.MailMergeSaveDocumentAs; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = SourceCount > 0;
		}
		protected int SourceCount {
			get {
#if SL
				IList source = Options.MailMerge.DataSource as IList;
				if (source == null)
					return 0;
				return source.Count;
#else
				if (DocumentModel.MailMergeDataController.IsReady)
					return DocumentModel.MailMergeDataController.ListSourceRowCount;
				else
					return 0;
#endif
			}
		}
		protected internal override void ExecuteCore() {
			IDocumentExportManagerService exportManagerService = DocumentModel.GetService<IDocumentExportManagerService>();
			DocumentExportHelper exportHelper = new DocumentExportHelper(DocumentModel);
			ExportTarget<DocumentFormat, bool> target = exportHelper.InvokeExportDialog(Control, exportManagerService);
			if (target == null)
				return;
			try {
				DocumentServer.NativeDocument.MailMerge(target.Storage, target.Exporter.Format);
			}
			finally {
				object obj = target.Storage;
				IDisposable disposable = obj as IDisposable;
				if (disposable != null)
					disposable.Dispose();
			}
		}
	}
}
