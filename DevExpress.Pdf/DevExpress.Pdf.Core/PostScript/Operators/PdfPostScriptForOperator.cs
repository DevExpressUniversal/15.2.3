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

using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfPostScriptForOperator : PdfPostScriptOperator {
		public const string Token = "for";
		public override void Execute(PdfPostScriptInterpreter interpreter) {
			PdfPostScriptStack stack = interpreter.Stack;
			IList<object> procedure = stack.Pop() as IList<object>;
			if (procedure == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			object limitValue = stack.Pop();
			object incrementValue = stack.Pop();
			object initialValue = stack.Pop();
			if ((initialValue is double) || (incrementValue is double) || (limitValue is double)) {
				double initial = PdfDocumentReader.ConvertToDouble(initialValue);
				double increment = PdfDocumentReader.ConvertToDouble(incrementValue);
				double limit = PdfDocumentReader.ConvertToDouble(limitValue);
				if (increment == 0)
					PdfDocumentReader.ThrowIncorrectDataException();
				else if (increment > 0)
					for (double value = initial; value <= limit; value += increment) {
						stack.Push(value);
						interpreter.Execute(procedure);
					}
				else
					for (double value = initial; value >= limit; value += increment) {
						stack.Push(value);
						interpreter.Execute(procedure);
					}
			}
			else {
				if (!(initialValue is int) || !(incrementValue is int) || !(limitValue is int))
					PdfDocumentReader.ThrowIncorrectDataException();
				int initial = (int)initialValue;
				int increment = (int)incrementValue;
				int limit = (int)limitValue;
				if (increment == 0)
					PdfDocumentReader.ThrowIncorrectDataException();
				else if (increment > 0)
					for (int value = initial; value <= limit; value += increment) {
						stack.Push(value);
						interpreter.Execute(procedure);
					}
				else
					for (int value = initial; value >= limit; value += increment) {
						stack.Push(value);
						interpreter.Execute(procedure);
					}
			}
		}
	}
}
