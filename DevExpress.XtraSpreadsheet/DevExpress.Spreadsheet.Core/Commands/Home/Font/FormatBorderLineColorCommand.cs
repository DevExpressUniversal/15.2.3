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
using System.Drawing;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FormatBorderLineColorCommand
	public class FormatBorderLineColorCommand : SpreadsheetCommand {
		public FormatBorderLineColorCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatBorderLineColor; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatBorderColor; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatBorderColorDescription; } }
		public override string ImageName { get { return "PenColor"; } }
		protected override bool UseOfficeImage { get { return true; } }
		#endregion
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<Color>();
		}
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<Color> valueBasedState = state as IValueBasedCommandUIState<Color>;
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
		}
		protected internal virtual void ModifyDocumentModel(IValueBasedCommandUIState<Color> state) {
			DocumentModel documentModel = DocumentModel;
			documentModel.BeginUpdate();
			try {
				ModifyDocumentModelCore(state);
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		protected internal virtual void ModifyDocumentModelCore(IValueBasedCommandUIState<Color> state) {
			Color color = state.Value;
			if (DXColor.IsTransparentOrEmpty(color))
				color = DXColor.Black;
			DocumentModel.UiBorderInfoRepository.CurrentItem.Color = color;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default);
			ApplyActiveSheetProtection(state, !Protection.FormatCellsLocked);
			IValueBasedCommandUIState<Color> valueBasedState = state as IValueBasedCommandUIState<Color>;
			if (valueBasedState != null)
				valueBasedState.Value = DocumentModel.UiBorderInfoRepository.CurrentItem.Color;
		}
	}
	#endregion
}
