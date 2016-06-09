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
using System.Text;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.XtraRichEdit.Mouse;
using DevExpress.XtraRichEdit.Services;
using DevExpress.Services;
using DevExpress.XtraRichEdit.Utils;
#if !SL
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office.Utils;
#endif
namespace DevExpress.XtraRichEdit.Forms {
	#region SimpleRichEditControl
#if !DEBUGTEST
	[DXToolboxItem(false), ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
#endif
	public class SimpleRichEditControl : RichEditControl {
		AbstractNumberingList abstractList;
		public SimpleRichEditControl() 
			: base() {
			DocumentModel.Sections.First.Page.Width = Int32.MaxValue;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AbstractNumberingList AbstractList { get { return abstractList; } set { abstractList = value; } }
		protected internal override InnerRichEditControl CreateInnerControl() {
			return new SimpleInnerRichEditControl(this);
		}
		protected internal override void AddServices() {
			AddService(typeof(IRichEditCommandFactoryService), new SimpleRichEditCommandFactoryService(this));
		}
	}
	#endregion
	public class SimpleInnerRichEditControl : InnerRichEditControl {
		public SimpleInnerRichEditControl(IInnerRichEditControlOwner owner)
			: base(owner) {
		}
		protected internal override void InitializeDefaultViewKeyboardHandlers(DevExpress.XtraRichEdit.Keyboard.NormalKeyboardHandler keyboardHandler, DevExpress.Utils.KeyboardHandler.IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, RichEditCommandId.PreviousCharacter);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, RichEditCommandId.NextCharacter);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Shift, RichEditCommandId.ExtendPreviousCharacter);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Shift, RichEditCommandId.ExtendNextCharacter);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Control, RichEditCommandId.PreviousWord);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Control, RichEditCommandId.NextWord);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Control | Keys.Shift, RichEditCommandId.ExtendPreviousWord);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Control | Keys.Shift, RichEditCommandId.ExtendNextWord);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.None, RichEditCommandId.StartOfLine);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.None, RichEditCommandId.EndOfLine);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.Shift, RichEditCommandId.ExtendStartOfLine);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.Shift, RichEditCommandId.ExtendEndOfLine);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.Control, RichEditCommandId.StartOfDocument);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.Control, RichEditCommandId.EndOfDocument);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.Control | Keys.Shift, RichEditCommandId.ExtendStartOfDocument);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.Control | Keys.Shift, RichEditCommandId.ExtendEndOfDocument);
			keyboardHandler.RegisterKeyHandler(provider, Keys.V, Keys.Control, RichEditCommandId.PasteSelection);
			keyboardHandler.RegisterKeyHandler(provider, Keys.V, Keys.Control | Keys.Alt, RichEditCommandId.ShowPasteSpecialForm);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Insert, Keys.Shift, RichEditCommandId.PasteSelection);
			keyboardHandler.RegisterKeyHandler(provider, Keys.C, Keys.Control, RichEditCommandId.CopySelection);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Insert, Keys.Control, RichEditCommandId.CopySelection);
			keyboardHandler.RegisterKeyHandler(provider, Keys.X, Keys.Control, RichEditCommandId.CutSelection);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Delete, Keys.Shift, RichEditCommandId.CutSelection);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Delete, Keys.None, RichEditCommandId.Delete);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Back, Keys.None, RichEditCommandId.BackSpaceKey);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Back, Keys.Shift, RichEditCommandId.BackSpaceKey);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Delete, Keys.Control, RichEditCommandId.DeleteWord);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Back, Keys.Control, RichEditCommandId.DeleteWordBack);
			keyboardHandler.RegisterKeyHandler(provider, Keys.A, Keys.Control, RichEditCommandId.SelectAll);
			keyboardHandler.RegisterKeyHandler(provider, Keys.NumPad5, Keys.Control, RichEditCommandId.SelectAll);
#if !SL
			keyboardHandler.RegisterKeyHandler(provider, Keys.Clear, Keys.Control, RichEditCommandId.SelectAll);
#endif
			keyboardHandler.RegisterKeyHandler(provider, Keys.Z, Keys.Control, RichEditCommandId.Undo);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Y, Keys.Control, RichEditCommandId.Redo);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Back, Keys.Alt, RichEditCommandId.Undo);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Back, Keys.Alt | Keys.Shift, RichEditCommandId.Redo);
			keyboardHandler.RegisterKeyHandler(provider, Keys.C, Keys.Control | Keys.Alt, RichEditCommandId.InsertCopyrightSymbol);
			keyboardHandler.RegisterKeyHandler(provider, Keys.R, Keys.Control | Keys.Alt, RichEditCommandId.InsertRegisteredTrademarkSymbol);
			keyboardHandler.RegisterKeyHandler(provider, Keys.T, Keys.Control | Keys.Alt, RichEditCommandId.InsertTrademarkSymbol);
		}
	}
	#region SimpleRichEditCommandFactoryService
	public class SimpleRichEditCommandFactoryService : RichEditCommandFactoryService {
		public SimpleRichEditCommandFactoryService(IRichEditControl control)
			: base(control) {
		}
		protected internal override void PopulateConstructorTable(RichEditCommandConstructorTable table) {
			base.PopulateConstructorTable(table);
			table.Remove(RichEditCommandId.PasteSelection);
			AddCommandConstructor(table, RichEditCommandId.PasteSelection, typeof(SimplePasteSelectionCommand));
		}
	}
	#endregion
	#region SimplePasteSelectionCommand
	public class SimplePasteSelectionCommand : PasteSelectionCommand {
		public SimplePasteSelectionCommand(IRichEditControl control) 
			: base(control) { 
		}
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new SimplePasteSelectionCoreCommand(Control);
		}
	}
	#endregion
	#region SimplePasteSelectionCoreCommand
	public class SimplePasteSelectionCoreCommand : PasteSelectionCoreCommand {
		public SimplePasteSelectionCoreCommand(IRichEditControl control)
			: base(control, new ClipboardPasteSource()) { 
		}
		protected internal override void CreateCommands() {
			AddCommand(new SimplePastePlainTextCommand(Control));
		}
	}
	#endregion
	#region SimplePastePlainTextCommand
	public class SimplePastePlainTextCommand : PastePlainTextCommand {
		public SimplePastePlainTextCommand(IRichEditControl control)
			: base(control) {
		}
		public override DocumentFormat Format { get { return DocumentFormat.PlainText; } }
		protected internal override void ModifyModel() {
			string text = GetTextData();
			if (text == null)
				return;
			text = GetFirstLine(text);			
			DocumentLogPosition position = DocumentModel.Selection.End;
			DocumentModel.MainPieceTable.InsertPlainText(position, text);
		}
		string GetFirstLine(string text) {
			StringBuilder firstLine = new StringBuilder();
			for (int i = 0; i < text.Length; i++) {
				if (text[i] != '\n' && text[i] != '\r')
					firstLine.Append(text[i]);
				else
					return firstLine.ToString();
			}
			return firstLine.ToString();
		}
		protected internal override bool IsDataAvailable() {
			return Clipboard.ContainsData(OfficeDataFormats.UnicodeText) ||
				Clipboard.ContainsData(OfficeDataFormats.Text) ||
				Clipboard.ContainsData(OfficeDataFormats.OemText);
		}
	}
	#endregion 
}
