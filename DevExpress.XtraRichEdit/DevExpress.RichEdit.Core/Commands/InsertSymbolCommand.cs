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
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Commands {
	public interface IInsertSymbolCommand {
		SymbolProperties SymbolInfo { get; set; }
		bool CanExecute();
		void ForceExecute(ICommandUIState state);
	}
	#region InsertSymbolCommand
	public class InsertSymbolCommand : TransactedInsertObjectCommand, IInsertSymbolCommand {
		public InsertSymbolCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertSymbol; } }
		public SymbolProperties SymbolInfo {
			get {
				InsertSymbolCoreCommand command = (InsertSymbolCoreCommand)InsertObjectCommand;
				return command.SymbolInfo;
			}
			set {
				InsertSymbolCoreCommand command = (InsertSymbolCoreCommand)InsertObjectCommand;
				command.SymbolInfo = value;
			}
		}
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new InsertSymbolCoreCommand(Control);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region InsertSymbolCoreCommand
	public class InsertSymbolCoreCommand : InsertTextCoreBaseCommand {
		SymbolProperties symbolInfo;
		public InsertSymbolCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertSymbol; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertSymbolDescription; } }
		public override string ImageName { get { return "Symbol"; } }
		protected internal override bool AllowAutoCorrect { get { return false; } }
		public SymbolProperties SymbolInfo { get { return symbolInfo; } set { symbolInfo = value; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			DefaultValueBasedCommandUIState<SymbolProperties> valueState = state as DefaultValueBasedCommandUIState<SymbolProperties>;
			if (valueState == null || valueState.Value == null)
				return;
			this.symbolInfo = valueState.Value;
			base.ForceExecute(state);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			DefaultValueBasedCommandUIState<SymbolProperties> valueState = state as DefaultValueBasedCommandUIState<SymbolProperties>;
			if (valueState == null) {
				state.Enabled = false;
				return;
			}
			base.UpdateUIStateCore(state);
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<SymbolProperties>();
		}
		protected internal override string GetInsertedText() {
			Debug.Assert(symbolInfo != null);
			return new String(symbolInfo.UnicodeChar, 1);
		}
		protected internal override void ModifyModel() {
			Selection selection = DocumentModel.Selection;
			DocumentLogPosition selectionEnd = selection.End;
			base.ModifyModel();
			if (selectionEnd == selection.End) 
				return;
			if (String.IsNullOrEmpty(symbolInfo.FontName))
				return;
			selectionEnd = selection.End;
			bool usePreviousBoxBounds = selection.UsePreviousBoxBounds;
			selection.Start = selectionEnd - 1;
			ChangeFontNameCommand command = new ChangeFontNameCommand(Control);
			if (command.CanExecute()) {
				DefaultValueBasedCommandUIState<string> state = new DefaultValueBasedCommandUIState<string>();
				state.Value = symbolInfo.FontName;
				command.ModifyDocumentModelCore(state);
			}
			selection.BeginUpdate();
			try {
				selection.Start = selectionEnd;
				selection.End = selectionEnd;
				selection.UsePreviousBoxBounds = usePreviousBoxBounds;
			}
			finally {
				selection.EndUpdate();
			}
		}
	}
	#endregion
}
