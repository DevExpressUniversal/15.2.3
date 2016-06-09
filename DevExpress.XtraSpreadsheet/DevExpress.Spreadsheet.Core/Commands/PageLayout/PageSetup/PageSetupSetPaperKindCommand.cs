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
using DevExpress.Data;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.Compatibility.System.Drawing.Printing;
#if SL
using DevExpress.Xpf.Core.Native;
using DevExpress.Utils;
#else
using System.Drawing.Printing;
#endif
namespace DevExpress.XtraSpreadsheet.Commands {
	#region PageSetupSetPaperKindCommand
	public class PageSetupSetPaperKindCommand : SpreadsheetMenuItemSimpleCommand {
		static readonly List<PaperKind> defaultPaperKindList = CreateDefaultPaperKindList();
		static readonly List<PaperKind> fullPaperKindList = CreateFullPaperKindList();
		public static IList<PaperKind> DefaultPaperKindList { get { return defaultPaperKindList; } }
		public static IList<PaperKind> FullPaperKindList { get { return fullPaperKindList; } }
		static List<PaperKind> CreateDefaultPaperKindList() {
			List<PaperKind> result = new List<PaperKind>();
			result.Add(PaperKind.Letter);
			result.Add(PaperKind.Legal);
			result.Add(PaperKind.Folio);
			result.Add(PaperKind.A4);
			result.Add(PaperKind.B5);
			result.Add(PaperKind.Executive);
			result.Add(PaperKind.A5);
			result.Add(PaperKind.A6);
			return result;
		}
		static List<PaperKind> CreateFullPaperKindList() {
			List<PaperKind> result = new List<PaperKind>();
#if SL
			foreach (PaperKind paperKind in EnumExtensions.GetValues(typeof(PaperKind)))
#else
			foreach (PaperKind paperKind in Enum.GetValues(typeof(PaperKind)))
#endif
				if (paperKind != PaperKind.Custom)
					result.Add(paperKind);
			return result;
		}
		PaperKind paperKind = PaperKind.Letter;
		public PageSetupSetPaperKindCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public PageSetupSetPaperKindCommand(ISpreadsheetControl control, PaperKind paperKind)
			: base(control) {
			this.paperKind = paperKind;
		}
		#region Properties
		public PaperKind PaperKind { get { return paperKind; } set { paperKind = value; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PageSetupSetPaperKind; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PageSetupSetPaperKindDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PageSetupSetPaperKind; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<PaperKind> valueBasedState = state as IValueBasedCommandUIState<PaperKind>;
			if (valueBasedState != null) {
				if (valueBasedState.Value != PaperKind.Custom)
					PaperKind = valueBasedState.Value;
			}
			base.ForceExecute(state);
		}
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdate();
			try {
				ActiveSheet.PrintSetup.PaperKind = PaperKind;
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable && !InnerControl.IsAnyInplaceEditorActive;
			state.Visible = true;
			IValueBasedCommandUIState<PaperKind> valueBasedState = state as IValueBasedCommandUIState<PaperKind>;
			if (valueBasedState != null)
				state.Checked = (ActiveSheet.PrintSetup.PaperKind == valueBasedState.Value);
			else
				state.Checked = false;
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<PaperKind> result = new DefaultValueBasedCommandUIState<PaperKind>();
			result.Value = PaperKind;
			return result;
		}
	}
	#endregion
}
