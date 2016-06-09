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
namespace DevExpress.Pdf {
	public class PdfSetTextRenderingModeCommand : PdfCommand {
		internal const string Name = "Tr";
		static readonly List<int> supportedModes;
		static PdfSetTextRenderingModeCommand() {
			Array array = Enum.GetValues(typeof(PdfTextRenderingMode));
			supportedModes = new List<int>(array.Length);
			foreach (PdfTextRenderingMode mode in array)
				supportedModes.Add((int)mode);
		}
		readonly PdfTextRenderingMode textRenderingMode;
		public PdfTextRenderingMode TextRenderingMode { get { return textRenderingMode; } }
		public PdfSetTextRenderingModeCommand(PdfTextRenderingMode textRenderingMode) {
			this.textRenderingMode = textRenderingMode;
		}
		internal PdfSetTextRenderingModeCommand(PdfOperands operands) {
			int modeIndex = operands.PopInt();
			if (!supportedModes.Contains(modeIndex))
				PdfDocumentReader.ThrowIncorrectDataException();
			textRenderingMode = (PdfTextRenderingMode)modeIndex;
		}
		protected internal override void Write(PdfResources resources, PdfDocumentWritableStream writer) {
			writer.WriteSpace();
			writer.WriteInt((int)textRenderingMode);
			writer.WriteSpace();
			writer.WriteString(Name);
		}
		protected internal override void Execute(PdfCommandInterpreter interpreter) {
			interpreter.SetTextRenderingMode(textRenderingMode);
		}
	}
}
