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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfPostScriptCalculatorFunction : PdfCustomFunction {
		internal const int Number = 4;
		readonly byte[] code;
		PdfPostScriptInterpreter interpreter;
		IEnumerable<object> program;
		public byte[] Code { get { return code; } }
		protected override int FunctionType { get { return Number; } }
		internal PdfPostScriptCalculatorFunction(PdfReaderDictionary dictionary, byte[] code) : base(dictionary) {
			if (code == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			this.code = code;
		}
		protected internal override bool IsSame(PdfFunction function) {
			return base.IsSame(function) && code == ((PdfPostScriptCalculatorFunction)function).code;
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return new PdfCompressedStream(FillDictionary(objects), code);
		}
		protected override double[] PerformTransformation(double[] arguments) {
			if (program == null) {
				try {
					IList<object> operators = PdfPostScriptFileParser.Parse(code) as IList<object>;
					if (operators != null && operators.Count != 1)
						PdfDocumentReader.ThrowIncorrectDataException();
					program = operators[0] as IList<object>;
					if (program == null)
						PdfDocumentReader.ThrowIncorrectDataException();
				}
				catch {
					program = new object[0];
				}
				interpreter = new PdfPostScriptInterpreter();
			}
			PdfPostScriptStack stack = interpreter.Stack;
			foreach (double argument in arguments)
				stack.Push(argument);
			interpreter.Execute(program);
			int rangeSize = RangeSize;
			double[] result = new double[rangeSize];
			for (int i = rangeSize - 1; i >= 0; i--)
				result[i] = PdfDocumentReader.ConvertToDouble(stack.Pop());
			stack.Clear();
			return result;
		}
	}
}
