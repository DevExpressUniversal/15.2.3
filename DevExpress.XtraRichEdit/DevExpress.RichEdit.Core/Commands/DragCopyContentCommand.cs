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
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Windows.Forms;
#if !SL
using System.Windows.Forms;
#else
using System.Windows;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region DragCopyContentCommand
	public class DragCopyContentCommand : DragMoveContentCommandBase {
		public DragCopyContentCommand(IRichEditControl control, DocumentModelPosition targetPosition)
			: base(control, targetPosition) {
		}
		protected internal override bool CopyContent { get { return true; } }
	}
	#endregion
	#region DragCopyExternalContentCommand
	public class DragCopyExternalContentCommand : DragCopyContentCommand {
		public DragCopyExternalContentCommand(IRichEditControl control, DocumentModelPosition targetPosition, IDataObject dataObject)
			: base(control, targetPosition) {
			Guard.ArgumentNotNull(dataObject, "dataObject");
			PasteDataObjectCoreCommand command = (PasteDataObjectCoreCommand)PasteContentCommand;
			command.DataObject = dataObject;
		}
		protected internal override CopyAndSaveContentCommand CreateCopyContentCommand() {
			return null; 
		}
		protected internal override Command CreatePasteContentCommand() {
			return Control.InnerControl.CreatePasteDataObjectCommand(null);			
		}
		public override void UpdateUIState(ICommandUIState state) {
			UpdateUIStateCore(state, true);
		}
		public override void ForceExecute(ICommandUIState state) {
			DocumentModel documentModel = DocumentModel;
			DocumentLogPosition start = Anchor.Position.LogPosition;
			base.ForceExecute(state);
			documentModel.BeginUpdate();
			try {
				SetSelection(documentModel.Selection, start);
			} finally {
				documentModel.EndUpdate();
			}
		}
		protected internal virtual void SetSelection(Selection selection, DocumentLogPosition start) {
			selection.Start = start;
			selection.End = Anchor.Position.LogPosition;
		}
	}
	#endregion
}
