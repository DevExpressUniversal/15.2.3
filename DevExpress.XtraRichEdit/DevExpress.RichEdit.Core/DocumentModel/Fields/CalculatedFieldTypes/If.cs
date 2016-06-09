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
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Fields {
	public partial class IfField : CalculatedFieldBase {
		const string EqualOperator = "=";
		const string NotEqualOperator = "<>";
		const string LessOperator = "<";
		const string LessOrEqualOperator = "<=";
		const string GreaterOperator = ">";
		const string GreaterOrEqualOperator = ">=";
		#region FieldInitialization
		#region static
		public static readonly string FieldType = "IF";
		static readonly Dictionary<string, bool> switchesWithArgument = CreateSwitchesWithArgument();
		public static CalculatedFieldBase Create() {
			return new IfField();
		}
		#endregion
		string expression1;
		string cmpOperator;
		string expression2;
		DocumentLogInterval trueResult;
		DocumentLogInterval falseResult;
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return switchesWithArgument; } }
		protected override string FieldTypeName { get { return FieldType; } }
		public string Expression1 { get { return expression1; } }
		public string CmpOperator { get { return cmpOperator; } }
		public string Expression2 { get { return expression2; } }
		public DocumentLogInterval TrueResult { get { return trueResult; } }
		public DocumentLogInterval FalseResult { get { return falseResult; } }
		public override void Initialize(PieceTable pieceTable, InstructionCollection instructions) {
			base.Initialize(pieceTable, instructions);
			expression1 = instructions.GetArgumentAsString(0);
			cmpOperator = instructions.GetArgumentAsString(1);
			expression2 = instructions.GetArgumentAsString(2);
			trueResult = instructions.GetArgumentAsDocumentInterval(3);
			if (instructions.Arguments.Count >= 5)
				falseResult = instructions.GetArgumentAsDocumentInterval(4);
			else
				falseResult = null;
		}
		#endregion
		protected override FieldMailMergeType MailMergeType() {
			return FieldMailMergeType.Mixed;
		}
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			DocumentModel targetModel = sourcePieceTable.DocumentModel.GetFieldResultModel();
			bool compareResult = Compare();
			if (compareResult == true)
				InsertResult(sourcePieceTable, targetModel, trueResult);
			else if(falseResult != null)
				InsertResult(sourcePieceTable, targetModel, falseResult);
			return new CalculatedFieldValue(targetModel);
		}
		protected override FieldResultOptions GetCharacterFormatFlag() {
			FieldResultOptions result = base.GetCharacterFormatFlag();
			if (result == FieldResultOptions.None)
				return FieldResultOptions.DoNotApplyFieldCodeFormatting;
			else
				return result;
		}
		void InsertResult(PieceTable sourcePieceTable, DocumentModel targetModel, DocumentLogInterval sourceInterval) {
			DocumentModelCopyOptions options = new DocumentModelCopyOptions(sourceInterval.Start, sourceInterval.Length);
			options.CopyDocumentVariables = true;
			DocumentModelCopyCommand copyCommand = sourcePieceTable.DocumentModel.CreateDocumentModelCopyCommand(sourcePieceTable, targetModel, options);
			copyCommand.Execute();
		}
		protected virtual bool Compare() {
			float numericValue1 = GetNumericValue(expression1);
			if (float.IsNaN(numericValue1))
				return CompareText();
			float numericValue2 = GetNumericValue(expression2);
			if (float.IsNaN(numericValue2))
				return CompareText();
			return CompareNumericExpression(numericValue1, numericValue2);
		}
		float GetNumericValue(string expression) {
			float value;
			if (float.TryParse(expression, out value))
				return value;
			else
				return float.NaN;
		}
		bool CompareText() {
			if (expression2.IndexOfAny(new char[] { '*', '?' }) >= 0 && (CmpOperator == EqualOperator || CmpOperator == NotEqualOperator))
				return CompareTextUsingRegExp();
			return CompareExpressionCore(StringExtensions.CompareWithCultureInfoAndOptions(expression1, expression2, CultureInfo.InvariantCulture, CompareOptions.Ordinal));
		}
		bool CompareTextUsingRegExp() {
			return expression1 == expression2;
		}
		bool CompareNumericExpression(float value1, float value2) {
			return CompareExpressionCore(value1 - value2);
		}
		bool CompareExpressionCore(float compareResult) {
			switch (CmpOperator) {
				case EqualOperator:
					return compareResult == 0;
				case NotEqualOperator:
					return compareResult != 0;
				case GreaterOrEqualOperator:
					return compareResult >= 0;
				case GreaterOperator:
					return compareResult > 0;
				case LessOperator:
					return compareResult < 0;
				case LessOrEqualOperator:
					return compareResult <= 0;
				default:
					return false;
			}
		}
	}
}
