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
using System.IO;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Export.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsColumnFormulaInfo
	public class XlsColumnFormulaInfo {
		#region Static Members
		public static XlsColumnFormulaInfo FromColumnFormula(Formula formula, IXlsRPNContext xlsRPNContext) {
			XlsColumnFormulaInfo result = new XlsColumnFormulaInfo();
			result.SetFormula(formula, xlsRPNContext);
			return result;
		}
		#endregion
		#region Fields
		internal const short fixedPartSize = 2;
		byte[] formulaBytes;
		ParsedExpression parsedExpression;
		#endregion
		#region Properties
		public ParsedExpression ParsedExpression { get { return parsedExpression; } protected set { parsedExpression = value; } }
		protected byte[] FormulaBytes { get { return formulaBytes; } set { formulaBytes = value; } }
		protected int FormulaBytesSize { get; set; }
		#endregion
		public void SetParsedExpression(ParsedExpression parsedExpression, IRPNContext context) {
			ParsedExpression = parsedExpression;
			FormulaBytes = context.ExpressionToBinary(ParsedExpression);
		}
		public ParsedExpression GetFormulaExpression(XlsContentBuilder contentBuilder) {
			return XlsParsedThingConverter.ToModelExpression(ParsedExpression, contentBuilder.RPNContext);
		}
		protected void SetFormula(Formula formula, IXlsRPNContext xlsRPNContext) {
			ParsedExpression xlsExpression = XlsParsedThingConverter.ToXlsExpression(formula.Expression, xlsRPNContext);
			SetParsedExpression(xlsExpression, xlsRPNContext);
		}
		public virtual void Read(BinaryReader reader, XlsRPNContext context) {
			FormulaBytesSize = reader.ReadUInt16();
			if(FormulaBytesSize > 0) {
				FormulaBytes = reader.ReadBytes(FormulaBytesSize);
				ParsedExpression = context.BinaryToExpression(FormulaBytes);
			}
		}
		public virtual void Write(BinaryWriter writer) {
			if (formulaBytes != null) {
				writer.Write((ushort)FormulaBytes.Length);
				writer.Write(FormulaBytes);
			} else
				writer.Write((ushort)0);
		}
		public virtual short GetSize() {
			int result = fixedPartSize;
			if (FormulaBytes != null)
				result += FormulaBytes.Length;
			return (short)result;
		}
	}
	#endregion
	#region XlsTotalColumnFormulaInfo
	public class XlsTotalColumnFormulaInfo : XlsColumnFormulaInfo {
		#region Static Members
		public static XlsTotalColumnFormulaInfo FromTotalColumnFormula(Formula formula, IXlsRPNContext xlsRPNContext) {
			XlsTotalColumnFormulaInfo result = new XlsTotalColumnFormulaInfo(formula is ArrayFormula);
			result.SetFormula(formula, xlsRPNContext);
			return result;
		}
		#endregion
		readonly bool isArray;
		public XlsTotalColumnFormulaInfo(bool isArray) {
			this.isArray = isArray;
		}
		public bool IsArray { get { return isArray; } }
		public override void Read(BinaryReader reader, XlsRPNContext context) {
			FormulaBytesSize = reader.ReadUInt16();
			if(FormulaBytesSize > 0) {
				FormulaBytes = reader.ReadBytes(FormulaBytesSize);
				ParsedExpression = context.BinaryToExpression(FormulaBytes, reader);
			}
		}
		public override void Write(BinaryWriter writer) {
			if(FormulaBytes != null)
				writer.Write(FormulaBytes);
			else
				writer.Write((ushort)0);
		}
		public override short GetSize() {
			if(FormulaBytes == null)
				return 2;
			return (short)FormulaBytes.Length;
		}
	}
	#endregion
}
