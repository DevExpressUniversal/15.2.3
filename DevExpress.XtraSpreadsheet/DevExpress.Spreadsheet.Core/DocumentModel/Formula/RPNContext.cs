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

namespace DevExpress.XtraSpreadsheet.Model {
	#region IRPNContext
	public interface IRPNContext {
		WorkbookDataContext WorkbookContext { get; }
		SheetDefinition GetSheetDefinition(int index);
		int IndexOfSheetDefinition(SheetDefinition sheetDefinition);
		byte[] ExpressionToBinary(ParsedExpression expression);
		ParsedExpression BinaryToExpression(byte[] data);
		ParsedExpression BinaryToExpression(byte[] data, int startIndex);
		string BuildExpressionString(byte[] data, int startIndex);
		VariantValue EvaluateBinaryExpression(byte[] binaryFormula, int startIndex, WorkbookDataContext context);
	}
	#endregion
	#region RPNContext
	public class RPNContext : IRPNContext {
		#region Fields
		WorkbookDataContext context;
		BinaryRPNReaderBase binaryRPNReader;
		BinaryRPNWriterBase binaryRPNWriter;
		BinaryRPNStringBuilder binaryRPNStringBuilder;
		#endregion
		public RPNContext(WorkbookDataContext context) {
			this.context = context;
			this.binaryRPNReader = new BinaryRPNReaderBase(this, context);
			this.binaryRPNWriter = new BinaryRPNWriterBase(this);
			this.binaryRPNStringBuilder = new BinaryRPNStringBuilder(this, context);
		}
		#region IRPNContext Members
		public WorkbookDataContext WorkbookContext { get { return context; } }
		public SheetDefinition GetSheetDefinition(int index) {
			return context.CurrentWorkbook.SheetDefinitions[index];
		}
		public int IndexOfSheetDefinition(SheetDefinition sheetDefinition) {
			return context.CurrentWorkbook.SheetDefinitions.GetIndex(sheetDefinition);
		}
		public byte[] ExpressionToBinary(ParsedExpression expression) {
			return binaryRPNWriter.GetBinary(expression);
		}
		public ParsedExpression BinaryToExpression(byte[] data) {
			return binaryRPNReader.FromBinary(data);
		}
		public ParsedExpression BinaryToExpression(byte[] data, int startIndex) {
			return binaryRPNReader.FromBinary(data, startIndex);
		}
		#endregion
		public string BuildExpressionString(byte[] data, int startIndex) {
			return binaryRPNStringBuilder.BuildExpressionString(data, startIndex);
		}
		public VariantValue EvaluateBinaryExpression(byte[] binaryFormula, int startIndex, WorkbookDataContext context) {
			BinaryRPNCalculator calculator = new BinaryRPNCalculator(this, context);
			return calculator.Evaluate(binaryFormula, startIndex);
		}
	 }
	#endregion
}
