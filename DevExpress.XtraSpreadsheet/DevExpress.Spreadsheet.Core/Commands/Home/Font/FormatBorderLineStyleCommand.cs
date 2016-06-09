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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Export.Xl;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FormatBorderLineStyleCommand
	public class FormatBorderLineStyleCommand : SpreadsheetCommand {
		public FormatBorderLineStyleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatBorderLineStyle; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatBorderLineStyle; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatBorderLineStyleDescription; } }
		public override string ImageName { get { return "BorderLineStyle"; } }
		#endregion
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<XlBorderLineStyle>();
		}
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<XlBorderLineStyle> valueBasedState = state as IValueBasedCommandUIState<XlBorderLineStyle>;
			if (valueBasedState == null)
				return;
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				ModifyDocumentModel(valueBasedState);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
			DocumentModel.UiBorderInfoRepository.RaiseUpdateUI();
			InnerControl.RaiseUpdateUI();
		}
		protected internal virtual void ModifyDocumentModel(IValueBasedCommandUIState<XlBorderLineStyle> state) {
			DocumentModel documentModel = DocumentModel;
			documentModel.BeginUpdate();
			try {
				ModifyDocumentModelCore(state);
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		protected internal virtual void ModifyDocumentModelCore(IValueBasedCommandUIState<XlBorderLineStyle> state) {
			DocumentModel.UiBorderInfoRepository.CurrentItem.LineStyle = state.Value;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default);
			ApplyActiveSheetProtection(state, !Protection.FormatCellsLocked);
			IValueBasedCommandUIState<XlBorderLineStyle> valueBasedState = state as IValueBasedCommandUIState<XlBorderLineStyle>;
			if (valueBasedState != null)
				valueBasedState.Value = DocumentModel.UiBorderInfoRepository.CurrentItem.LineStyle;
		}
	}
	#endregion
}
