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
using DevExpress.Utils.Commands;
using System.Collections.Generic;
using DevExpress.Office.Localization;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	public class PasteSpecialCommand : PasteSelectionCommand {
		public PasteSpecialCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ShowPasteSpecialForm; } }
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_ShowPasteSpecialForm; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_ShowPasteSpecialFormDescription; } }
		public override string ImageName { get { return "PasteSpecial"; } }
		public override bool ShowsModalDialog { get { return true; } }
		#endregion
		protected internal override void ForceExecuteCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				if (DocumentModel.IsCopyCutMode) {
					ModelPasteSpecialOptions options = new ModelPasteSpecialOptions();
					Control.ShowPasteSpecialLocalForm(options, ShowPasteSpecialLocalFormCallback, state);
				}
				else {
					IValueBasedCommandUIState<PasteSpecialInfo> valueBasedState = state as IValueBasedCommandUIState<PasteSpecialInfo>;
					ShowPasteSpecialForm(valueBasedState.Value, ShowPasteSpecialFormCallback, state);
				}
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal virtual void ShowPasteSpecialLocalFormCallback(ModelPasteSpecialOptions options){
			this.PasteOptions = options;
			DefaultCommandUIState state = new DefaultCommandUIState();
			state.Enabled = true;
			state.Visible = true;
			RestoreCopiedRange();
			base.ForceExecuteCore(state);
		}
		protected internal virtual void ShowPasteSpecialFormCallback(PasteSpecialInfo properties, object callbackData) {
			if (properties == null)
				return;
			PasteCommandBase command = properties.CreateCommand(Control);
			if (command == null)
				return;
			this.Format = command.Format;
			command.PasteOptions = new ModelPasteSpecialOptions(ModelPasteSpecialFlags.All); 
			Commands[0] = command;
			DefaultCommandUIState state = new DefaultCommandUIState();
			state.Enabled = true;
			state.Visible = true;
			base.ForceExecuteCore(state);
		}
		internal virtual void ShowPasteSpecialForm(PasteSpecialInfo properties, ShowPasteSpecialFormCallback callback, object callbackData) {
			Control.ShowPasteSpecialForm(properties, callback, callbackData);
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<PasteSpecialInfo> state = new DefaultValueBasedCommandUIState<PasteSpecialInfo>();
			state.Value = new PasteSpecialInfo();
			return state;
		}
		public override void UpdateUIState(ICommandUIState state) {
			base.UpdateUIState(state);
			if (state.Enabled)
				state.Enabled = !InnerControl.IsInplaceEditorActive && !DocumentModel.CopiedRangeProvider.IsCut;
		}
	}
}
