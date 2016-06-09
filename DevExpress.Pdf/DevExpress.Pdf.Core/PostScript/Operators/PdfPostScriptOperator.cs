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

namespace DevExpress.Pdf.Native {
	public abstract class PdfPostScriptOperator {
		public static object Parse(object obj) {
			PdfPostScriptOperator op = obj as PdfPostScriptOperator;
			if (op != null)
				return op;
			PdfComment comment = obj as PdfComment;
			if (comment != null)
				return new PdfPostScriptComment(comment.Text);
			string token = obj as string;
			if (token == null)
				return obj;
			switch (token) {
				case PdfPostScriptAddOperator.Token:
					return new PdfPostScriptAddOperator();
				case PdfPostScriptAndOperator.Token:
					return new PdfPostScriptAndOperator();
				case PdfPostScriptArrayOperator.Token:
					return new PdfPostScriptArrayOperator();
				case PdfPostScriptBeginOperator.Token:
					return new PdfPostScriptBeginOperator();
				case PdfPostScriptCurrentDictOperator.Token:
					return new PdfPostScriptCurrentDictOperator();
				case PdfPostScriptCurrentFileOperator.Token:
					return new PdfPostScriptCurrentFileOperator();
				case PdfPostScriptCvrOperator.Token:
					return new PdfPostScriptCvrOperator();
				case PdfPostScriptDefOperator.Token:
					return new PdfPostScriptDefOperator();
				case PdfPostScriptDictOperator.Token:
					return new PdfPostScriptDictOperator();
				case PdfPostScriptDupOperator.Token:
					return new PdfPostScriptDupOperator();
				case PdfPostScriptEexecOperator.Token:
					return new PdfPostScriptEexecOperator();
				case PdfPostScriptEndOperator.Token:
					return new PdfPostScriptEndOperator();
				case PdfPostScriptEqOperator.Token:
					return new PdfPostScriptEqOperator();
				case PdfPostScriptExchOperator.Token:
					return new PdfPostScriptExchOperator();
				case PdfPostScriptExecOperator.Token:
					return new PdfPostScriptExecOperator();
				case PdfPostScriptExecuteOnlyOperator.Token:
					return new PdfPostScriptExecuteOnlyOperator();
				case PdfPostScriptFindFontOperator.Token:
					return new PdfPostScriptFindFontOperator();
				case PdfPostScriptFontDirectoryOperator.Token:
					return new PdfPostScriptFontDirectoryOperator();
				case PdfPostScriptForOperator.Token:
					return new PdfPostScriptForOperator();
				case PdfPostScriptGetOperator.Token:
					return new PdfPostScriptGetOperator();
				case PdfPostScriptGtOperator.Token:
					return new PdfPostScriptGtOperator();
				case PdfPostScriptIfOperator.Token:
					return new PdfPostScriptIfOperator();
				case PdfPostScriptIfElseOperator.Token:
					return new PdfPostScriptIfElseOperator();
				case PdfPostScriptIndexOperator.Token:
					return new PdfPostScriptIndexOperator();
				case PdfPostScriptKnownOperator.Token:
					return new PdfPostScriptKnownOperator();
				case PdfPostScriptMulOperator.Token:
					return new PdfPostScriptMulOperator();
				case PdfPostScriptNoAccessOperator.Token:
					return new PdfPostScriptNoAccessOperator();
				case PdfPostScriptNotOperator.Token:
					return new PdfPostScriptNotOperator();
				case PdfPostScriptPopOperator.Token:
					return new PdfPostScriptPopOperator();
				case PdfPostScriptPutOperator.Token:
					return new PdfPostScriptPutOperator();
				case PdfPostScriptReadonlyOperator.Token:
					return new PdfPostScriptReadonlyOperator();
				case PdfPostScriptReadStringOperator.Token:
					return new PdfPostScriptReadStringOperator();
				case PdfPostScriptRollOperator.Token:
					return new PdfPostScriptRollOperator();
				case PdfPostScriptSaveOperator.Token:
					return new PdfPostScriptSaveOperator();
				case PdfPostScriptStandardEncodingOperator.Token:
					return new PdfPostScriptStandardEncodingOperator();
				case PdfPostScriptStringOperator.Token:
					return new PdfPostScriptStringOperator();
				case PdfPostScriptSubOperator.Token:
					return new PdfPostScriptSubOperator();
				case PdfPostScriptSystemDictOperator.Token:
					return new PdfPostScriptSystemDictOperator();
				case PdfPostScriptType1FontOperator.RDToken:
				case PdfPostScriptType1FontOperator.NDToken:
				case PdfPostScriptType1FontOperator.NPToken:
					return new PdfPostScriptType1FontOperator(token);
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					return null;
			}
		}
		protected PdfPostScriptOperator() {
		}
		public abstract void Execute(PdfPostScriptInterpreter interpreter);
	}
}
