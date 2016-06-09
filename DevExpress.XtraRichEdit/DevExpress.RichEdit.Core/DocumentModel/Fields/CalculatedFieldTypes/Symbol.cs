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
using DevExpress.XtraRichEdit.Model;
using System.Globalization;
using System.Text;
namespace DevExpress.XtraRichEdit.Fields {
	public class SymbolField : CalculatedFieldBase {
		#region static
		public static readonly string FieldType = "SYMBOL";
		static readonly Dictionary<string, bool> switchesWithArgument = CreateSwitchesWithArgument("f", "s");
		public static CalculatedFieldBase Create() {
			return new SymbolField();
		}
		#endregion
		int characterCode;
		bool isAnsiCharacter;
		bool isUnicodeCharacter;
		string fontName;
		int doubleFontSize;
		public int CharacterCode { get { return characterCode; } }
		public bool IsAnsiCharacter { get { return isAnsiCharacter; } }
		public bool IsUnicodeCharacter { get { return isUnicodeCharacter; } }
		public string FontName { get { return fontName; } }
		public int DoubleFontSize { get { return doubleFontSize; } }
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return switchesWithArgument; } }
		protected override string FieldTypeName { get { return FieldType; } }
		public override void Initialize(PieceTable pieceTable, InstructionCollection switches) {
			base.Initialize(pieceTable, switches);
			string argument = switches.GetArgumentAsString(0);
			this.characterCode = GetCharacterCode(argument);
			this.fontName = switches.GetString("f");
			this.doubleFontSize = switches.GetInt("s")*2;
			this.isAnsiCharacter = switches.GetBool("a");
			this.isUnicodeCharacter = switches.GetBool("u");
		}
		int GetCharacterCode(string argument) {
			int fromBase = argument.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? 16 : 10;
			try {
				return Convert.ToInt32(argument, fromBase);
			}
			catch {
				return -1;
			}
		}
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			int unicodeCharacterCode = GetUnicodeCharacterCode();
			if (unicodeCharacterCode < 0)
				return CalculatedFieldValue.Null;
			char character = (char)unicodeCharacterCode;
			DocumentModel targetModel = sourcePieceTable.DocumentModel.GetFieldResultModel();
			targetModel.MainPieceTable.InsertText(DocumentLogPosition.Zero, new string(character, 1));
			TextRun run = targetModel.MainPieceTable.LastInsertedRunInfo.Run;
			if (!String.IsNullOrEmpty(FontName))
				run.FontName = FontName;
			if (DoubleFontSize > 0)
				run.DoubleFontSize = DoubleFontSize;
			return new CalculatedFieldValue(targetModel);
		}
#if !SL
		int GetUnicodeCharacterCode() {
			if (IsUnicodeCharacter) {
				if (CharacterCode < Char.MinValue || CharacterCode > Char.MaxValue)
					return -1;
				return CharacterCode;
			}
			else {
				if (CharacterCode < Byte.MinValue || CharacterCode > Byte.MaxValue)
					return -1;
				Encoding ansiEncoding = Encoding.GetEncoding(1252);
				byte[] bytes = Encoding.Convert(ansiEncoding, Encoding.UTF8, new byte[] { (byte)characterCode });
				char[] chars = Encoding.UTF8.GetChars(bytes);
				if (chars.Length != 1)
					return -1;
				return chars[0];
			}
		}
#else
		int GetUnicodeCharacterCode() {
			if (CharacterCode < Char.MinValue || CharacterCode > Char.MaxValue)
				return -1;
			return CharacterCode;
		}
#endif
		protected internal override UpdateFieldOperationType GetAllowedUpdateFieldTypes(FieldUpdateOnLoadOptions options) {
			return base.GetAllowedUpdateFieldTypes(options) | UpdateFieldOperationType.Load | UpdateFieldOperationType.Copy | UpdateFieldOperationType.CreateModelForExport;
		}
		protected override FieldResultOptions GetCharacterFormatFlag() {
			return base.GetCharacterFormatFlag() | FieldResultOptions.DoNotApplyFieldCodeFormatting;
		}
	}
}
