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
using DevExpress.Office.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils.Commands;
using DevExpress.Spreadsheet;
using DevExpress.Office.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region PasteContentCommandBase (abstract class)
	public abstract class PasteCommandBase : SpreadsheetSelectionCommand {
		PasteSource pasteSource;
		ModelPasteSpecialOptions pasteOptions;
		protected PasteCommandBase(ISpreadsheetControl control)
			: base(control) {
			this.pasteSource = new EmptyPasteSource();
			this.pasteOptions = new ModelPasteSpecialOptions();
		}
		#region Properties
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_Paste; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_PasteDescription; } }
		public override string ImageName { get { return "Paste"; } }
		public abstract DocumentFormat Format { get; }
		public virtual PasteSource PasteSource { get { return pasteSource; } set { pasteSource = value; } }
		public virtual ModelPasteSpecialOptions PasteOptions { get { return pasteOptions; } set { pasteOptions = value; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
#if !SL
#endif
			bool enabled = !ActiveSheet.Selection.IsDrawingSelected && IsContentEditable && IsDataAvailable();
			state.Enabled = enabled;
			state.Visible = true;
			state.Checked = false;
		}
		protected internal abstract bool IsDataAvailable();
	}
	#endregion
}
