#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public class PdfShowTextWithGlyphPositioningCommand : PdfShowTextCommand {
		internal new const string Name = "TJ";
		readonly double[] glyphOffsets;
		public double[] GlyphOffsets { get { return glyphOffsets; } }
		public PdfShowTextWithGlyphPositioningCommand(byte[] text, double[] glyphOffsets)
			: base(text) {
			if (glyphOffsets.Length <= text.Length)
				throw new ArgumentException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectGlyphPosition));
			this.glyphOffsets = glyphOffsets;
		}
		internal PdfShowTextWithGlyphPositioningCommand(PdfOperands operands) {
			IList<object> data = operands.PopObject() as IList<object>;
			if (data == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			List<byte> text = new List<byte>();
			List<double> offsets = new List<double>();
			double currentGlyphOffset = 0.0;
			foreach (object value in data) {
				byte[] str = value as byte[];
				if (str == null)
					currentGlyphOffset += PdfDocumentReader.ConvertToDouble(value);
				else {
					int length = str.Length;
					if (length > 0) {
						text.AddRange(str);
						offsets.Add(currentGlyphOffset);
						for (int i = 1; i < length; i++)
							offsets.Add(0);
						currentGlyphOffset = 0.0;
					}
				}
			}
			offsets.Add(currentGlyphOffset);
			glyphOffsets = offsets.ToArray();
			Text = text.ToArray();
		}
		protected internal override void Write(PdfResources resources, PdfDocumentWritableStream writer) {
			writer.WriteSpace();
			writer.WriteOpenBracket();
			byte[] text = Text;
			int textLength = text.Length;
			for (int pos = 0; pos < textLength; ) {
				double glyphOffset = glyphOffsets[pos];
				if (glyphOffset != 0.0) {
					writer.WriteSpace();
					writer.WriteDouble(glyphOffset);
				}
				writer.WriteSpace();
				List<byte> str = new List<byte>() { text[pos] };
				for (++pos; pos < textLength && glyphOffsets[pos] == 0; pos++)
					str.Add(text[pos]);
				writer.WriteHexadecimalString(str.ToArray(), -1);
			}
			double lastOffset = glyphOffsets[glyphOffsets.Length - 1];
			if (lastOffset != 0.0) {
				writer.WriteSpace();
				writer.WriteDouble(lastOffset);
			}
			writer.WriteCloseBracket();
			writer.WriteSpace();
			writer.WriteString(Name);
		}
		protected internal override void Execute(PdfCommandInterpreter interpreter) {
			interpreter.DrawString(Text, glyphOffsets);
		}
	}
}
