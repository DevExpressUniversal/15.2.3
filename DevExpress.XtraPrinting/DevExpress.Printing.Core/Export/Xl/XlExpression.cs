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

using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.Export.Xl {
	#region XlCellReferenceMode
	public enum XlCellReferenceMode {
		Offset,
		Reference
	}
	#endregion
	#region XlCellReferenceStyle
	public enum XlCellReferenceStyle {
		A1,
		R1C1
	}
	#endregion
	#region XlFormulaType
	public enum XlExpressionStyle {
		Normal = 0,
		Shared = 1,
		Array = 2,
		DefinedName = 3,
	}
	#endregion
	#region IXlExpressionContext
	public interface IXlExpressionContext {
		XlCellPosition CurrentCell { get; set; }
		string CurrentSheetName { get; }
		XlCellReferenceMode ReferenceMode { get; set; }
		XlCellReferenceStyle ReferenceStyle { get; }
		int MaxColumnCount { get; }
		int MaxRowCount { get; }
		XlExpressionStyle ExpressionStyle { get; }
		CultureInfo Culture { get; }
	}
	#endregion
	#region XlExpressionContext
	class XlExpressionContext : IXlExpressionContext {
		public XlExpressionContext() {
			ReferenceMode = XlCellReferenceMode.Offset;
			MaxColumnCount = 16384;
			MaxRowCount = 1048576;
		}
		#region Properties
		public XlCellPosition CurrentCell { get; set; }
		public string CurrentSheetName { get; set; }
		public XlCellReferenceMode ReferenceMode { get; set; }
		public XlCellReferenceStyle ReferenceStyle { get { return XlCellReferenceStyle.A1; } }
		public int MaxColumnCount { get; set; }
		public int MaxRowCount { get; set; }
		public XlExpressionStyle ExpressionStyle { get; set; }
		public CultureInfo Culture { get { return CultureInfo.InvariantCulture; } }
		#endregion
	}
	#endregion
	#region XlExpression
	public class XlExpression : List<XlPtgBase> {
		public override string ToString() {
			return ToString(null);
		}
		public string ToString(IXlExpressionContext context) {
			XlExpressionStringBuilder builder = new XlExpressionStringBuilder();
			builder.Context = context;
			return builder.BuildString(this);
		}
	}
	#endregion
#region XlPtgTypeCode
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Spelling", "DF1000")]
	public static class XlPtgTypeCode {
		public const short Exp = 0x01;
		public const short Add = 0x03;
		public const short Sub = 0x04;
		public const short Mul = 0x05;
		public const short Div = 0x06;
		public const short Power = 0x07;
		public const short Concat = 0x08;
		public const short Lt = 0x09;
		public const short Le = 0x0a;
		public const short Eq = 0x0b;
		public const short Ge = 0x0c;
		public const short Gt = 0x0d;
		public const short Ne = 0x0e;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Spelling", "DF1000")]
		public const short Isect = 0x0f;
		public const short Union = 0x10;
		public const short Range = 0x11;
		public const short Uplus = 0x12;
		public const short Uminus = 0x13;
		public const short Percent = 0x14;
		public const short Paren = 0x15;
		public const short MissArg = 0x16;
		public const short Str = 0x17;
		public const short AttrSemi = 0x0119;
		public const short AttrIf = 0x0219;
		public const short AttrChoose = 0x0419;
		public const short AttrGoto = 0x0819;
		public const short AttrSpace = 0x4019;
		public const short Err = 0x1c;
		public const short Bool = 0x1d;
		public const short Int = 0x1e;
		public const short Num = 0x1f;
		public const short Array = 0x20;
		public const short Func = 0x01;
		public const short FuncVar = 0x02;
		public const short Ref3d = 0x1a;
		public const short Name = 0x03;
		public const short Ref = 0x04;
		public const short Area = 0x05;
		public const short MemFunc = 0x09;
		public const short Area3d = 0x1b;
		public const short RefN = 0x0c;
		public const short AreaN = 0x0d;
		public const short RefErr = 0x0a;
		public const short AreaErr = 0x0b;
		public const short RefErr3d = 0x1c;
		public const short AreaErr3d = 0x1d;
	}
#endregion
#region XlPtgDataType
	public enum XlPtgDataType {
		None = 0,
		Reference = 1,
		Value = 2,
		Array = 3
	}
#endregion
#region XlPtgAttrSpace
	public enum XlPtgAttrSpaceType {
		SpaceBeforeBaseExpression = 0,
		CarriageReturnBeforeBaseExpression = 1,
		SpaceBeforeOpenParentheses = 2,
		CarriageReturnBeforeOpenParentheses = 3,
		SpaceBeforeCloseParentheses = 4,
		CarriageReturnBeforeCloseParentheses = 5,
		SpaceBeforeExpression = 6
	}
#endregion
#region IXlPtgVisitor
	public interface IXlPtgVisitor {
		void Visit(XlPtgBinaryOperator ptg);
		void Visit(XlPtgUnaryOperator ptg);
		void Visit(XlPtgParen ptg);
		void Visit(XlPtgMissArg ptg);
		void Visit(XlPtgStr ptg);
		void Visit(XlPtgErr ptg);
		void Visit(XlPtgBool ptg);
		void Visit(XlPtgInt ptg);
		void Visit(XlPtgNum ptg);
		void Visit(XlPtgFunc ptg);
		void Visit(XlPtgFuncVar ptg);
		void Visit(XlPtgRef ptg);
		void Visit(XlPtgRef3d ptg);
		void Visit(XlPtgArea ptg);
		void Visit(XlPtgArea3d ptg);
		void Visit(XlPtgAttrSemi ptg);
		void Visit(XlPtgAttrSpace ptg);
		void Visit(XlPtgAttrIf ptg);
		void Visit(XlPtgAttrGoto ptg);
		void Visit(XlPtgAttrChoose ptg);
		void Visit(XlPtgRefN ptg);
		void Visit(XlPtgRefN3d ptg);
		void Visit(XlPtgAreaN ptg);
		void Visit(XlPtgAreaN3d ptg);
		void Visit(XlPtgRefErr ptg);
		void Visit(XlPtgAreaErr ptg);
		void Visit(XlPtgRefErr3d ptg);
		void Visit(XlPtgAreaErr3d ptg);
		void Visit(XlPtgArray ptg);
		void Visit(XlPtgName ptg);
		void Visit(XlPtgExp ptg);
		void Visit(XlPtgMemFunc ptg);
	}
#endregion
#region XlPtgBase
	public abstract class XlPtgBase {
		XlPtgDataType dataType;
		public abstract short TypeCode { get; }
		public XlPtgDataType DataType {
			get { return GetPtgDataType(this.dataType); }
			set { this.dataType = value; }
		}
		protected virtual XlPtgDataType GetPtgDataType(XlPtgDataType ptgDataType) {
			return XlPtgDataType.None;
		}
		protected internal short GetPtgCode() {
			ushort result = (ushort)(TypeCode & 0x7f1f);
			result |= (ushort)(((ushort)DataType & 0x03) << 5);
			return (short)result;
		}
		public abstract void Visit(IXlPtgVisitor visitor);
	}
#endregion
#region XlPtgOperator
	public abstract class XlPtgOperator : XlPtgBase {
		short typeCode;
		protected XlPtgOperator(short typeCode) {
			if (!IsValidTypeCode(typeCode))
				throw new ArgumentException("Invalid type code.");
			this.typeCode = typeCode;
		}
		public override short TypeCode { get { return typeCode; } }
		protected abstract bool IsValidTypeCode(short typeCode);
	}
#endregion
#region XlPtgWithDataType
	public abstract class XlPtgWithDataType : XlPtgBase {
		protected override XlPtgDataType GetPtgDataType(XlPtgDataType ptgDataType) {
			if (ptgDataType == XlPtgDataType.None)
				return XlPtgDataType.Reference;
			return ptgDataType;
		}
		protected XlPtgWithDataType() {
		}
		protected XlPtgWithDataType(XlPtgDataType dataType) {
			this.DataType = dataType;
		}
	}
#endregion
#region XlPtgBinaryOperator
	public class XlPtgBinaryOperator : XlPtgOperator {
		static readonly Dictionary<short, string> typeCodeToOperatorTextConversionTable = CreateTypeCodeToOperatorTextConvetsionTable();
		static Dictionary<short, string> CreateTypeCodeToOperatorTextConvetsionTable() {
			Dictionary<short, string> result = new Dictionary<short, string>();
			result.Add(XlPtgTypeCode.Add, "+");
			result.Add(XlPtgTypeCode.Sub, "-");
			result.Add(XlPtgTypeCode.Mul, "*");
			result.Add(XlPtgTypeCode.Div, "/");
			result.Add(XlPtgTypeCode.Power, "^");
			result.Add(XlPtgTypeCode.Concat, "&");
			result.Add(XlPtgTypeCode.Lt, "<");
			result.Add(XlPtgTypeCode.Le, "<=");
			result.Add(XlPtgTypeCode.Eq, "=");
			result.Add(XlPtgTypeCode.Ge, ">=");
			result.Add(XlPtgTypeCode.Gt, ">");
			result.Add(XlPtgTypeCode.Ne, "<>");
			result.Add(XlPtgTypeCode.Isect, " ");
			result.Add(XlPtgTypeCode.Union, ",");
			result.Add(XlPtgTypeCode.Range, ":");
			return result;
		}
		public XlPtgBinaryOperator(short typeCode)
			: base(typeCode) {
		}
		protected override bool IsValidTypeCode(short typeCode) {
			return typeCode >= XlPtgTypeCode.Add && typeCode <= XlPtgTypeCode.Range;
		}
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
		public string OperatorText { get { return typeCodeToOperatorTextConversionTable[TypeCode]; } }
	}
#endregion
#region XlPtgUnaryOperator
	public class XlPtgUnaryOperator : XlPtgOperator {
		public XlPtgUnaryOperator(short typeCode)
			: base(typeCode) {
		}
		protected override bool IsValidTypeCode(short typeCode) {
			return typeCode >= XlPtgTypeCode.Uplus && typeCode <= XlPtgTypeCode.Percent;
		}
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
		public string OperatorText { get { return GetOperatorText(); } }
		string GetOperatorText() {
			switch (TypeCode) {
				case XlPtgTypeCode.Uminus:
					return "-";
				case XlPtgTypeCode.Uplus:
					return "+";
				case XlPtgTypeCode.Percent:
					return "%";
				default:
					throw new InvalidOperationException("Invalid unary operator with typeCode + " + TypeCode);
			}
		}
	}
#endregion
#region XlPtgParen
	public class XlPtgParen : XlPtgBase {
		public override short TypeCode { get { return XlPtgTypeCode.Paren; } }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgMissArg
	public class XlPtgMissArg : XlPtgBase {
		public override short TypeCode { get { return XlPtgTypeCode.MissArg; } }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgStr
	public class XlPtgStr : XlPtgBase {
		string innerString = string.Empty;
		public XlPtgStr(string value) {
			this.Value = value;
		}
		public override short TypeCode { get { return XlPtgTypeCode.Str; } }
		public string Value {
			get { return innerString; }
			set {
				if (string.IsNullOrEmpty(value))
					value = string.Empty;
				if (value.Length > byte.MaxValue)
					throw new ArgumentException("String value too long");
				innerString = value;
			}
		}
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region Attributes
#region XlPtgAttrSemi
	public class XlPtgAttrSemi : XlPtgBase {
		public override short TypeCode { get { return XlPtgTypeCode.AttrSemi; } }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgAttrIf
	public class XlPtgAttrIf : XlPtgBase {
		int offset;
		public XlPtgAttrIf(int offset) {
			this.offset = offset;
		}
		public override short TypeCode { get { return XlPtgTypeCode.AttrIf; } }
		public int Offset {
			get { return offset; }
			set { this.offset = value; }
		}
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgAttrChoose
	public class XlPtgAttrChoose : XlPtgBase {
		IList<int> offsets;
		public XlPtgAttrChoose(List<int> offsets) {
			this.offsets = offsets;
		}
		public IList<int> Offsets { get { return offsets; } set { offsets = value; } }
		public override short TypeCode { get { return XlPtgTypeCode.AttrChoose; } }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgAttrGoto
	public class XlPtgAttrGoto : XlPtgBase {
		int offset;
		public XlPtgAttrGoto(int offset) {
			this.offset = offset;
		}
		public override short TypeCode { get { return XlPtgTypeCode.AttrGoto; } }
		public int Offset { get { return offset; } set { this.offset = value; } }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgAttrSpace
	public class XlPtgAttrSpace : XlPtgBase {
		int charCount;
		public XlPtgAttrSpace(XlPtgAttrSpaceType spaceType, int charCount) {
			SpaceType = spaceType;
			CharCount = charCount;
		}
		public override short TypeCode { get { return XlPtgTypeCode.AttrSpace; } }
		public XlPtgAttrSpaceType SpaceType { get; set; }
		public int CharCount {
			get { return charCount; }
			set {
				if (value < 0 || value > byte.MaxValue)
					throw new ArgumentOutOfRangeException("CharCount");
				this.charCount = value;
			}
		}
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#endregion
#region XlPtgErr
	public class XlPtgErr : XlPtgBase {
		public XlPtgErr(XlCellErrorType value) {
			this.Value = value;
		}
		public override short TypeCode { get { return XlPtgTypeCode.Err; } }
		public XlCellErrorType Value { get; set; }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgBool
	public class XlPtgBool : XlPtgBase {
		public XlPtgBool(bool value) {
			this.Value = value;
		}
		public override short TypeCode { get { return XlPtgTypeCode.Bool; } }
		public bool Value { get; set; }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgInt
	public class XlPtgInt : XlPtgBase {
		int value;
		public XlPtgInt(int value) {
			this.value = value;
		}
		public override short TypeCode { get { return XlPtgTypeCode.Int; } }
		public int Value {
			get { return this.value; }
			set {
				if (value < 0 || value > UInt16.MaxValue)
					throw new ArgumentOutOfRangeException(string.Format("Value {0} out of range 0..{1}.", value, UInt16.MaxValue));
				this.value = value;
			}
		}
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgNum
	public class XlPtgNum : XlPtgBase {
		double value = 0.0;
		public XlPtgNum(double value) {
			this.value = value;
		}
		public override short TypeCode { get { return XlPtgTypeCode.Num; } }
		public double Value {
			get { return this.value; }
			set {
				XtraExport.Xls.XNumChecker.CheckValue(value);
				this.value = value;
			}
		}
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgFunc
	public class XlPtgFunc : XlPtgWithDataType {
		int funcCode;
		public XlPtgFunc(int funcCode)
			: this(funcCode, XlPtgDataType.Reference) {
		}
		public XlPtgFunc(int funcCode, XlPtgDataType dataType)
			: base(dataType) {
			this.funcCode = funcCode;
		}
		public override short TypeCode { get { return XlPtgTypeCode.Func; } }
		public int FuncCode {
			get { return this.funcCode; }
			set {
				if (value < 0 || value > UInt16.MaxValue)
					throw new ArgumentOutOfRangeException(string.Format("FuncCode {0} out of range 0..{1}.", value, UInt16.MaxValue));
				this.funcCode = value;
			}
		}
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgFuncVar
	public class XlPtgFuncVar : XlPtgFunc {
		int paramCount;
		public XlPtgFuncVar(int funcCode, int paramCount)
			: this(funcCode, paramCount, XlPtgDataType.Reference) {
		}
		public XlPtgFuncVar(int funcCode, int paramCount, XlPtgDataType dataType)
			: base(funcCode, dataType) {
			this.paramCount = paramCount;
		}
		public override short TypeCode { get { return XlPtgTypeCode.FuncVar; } }
		public int ParamCount {
			get { return this.paramCount; }
			set {
				if (value < 0 || value > byte.MaxValue)
					throw new ArgumentOutOfRangeException(string.Format("ParamCount {0} out of range 0..{1}.", value, byte.MaxValue));
				this.paramCount = value;
			}
		}
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgRef
	public class XlPtgRef : XlPtgWithDataType {
		XlCellPosition cellPosition;
		public XlPtgRef(XlCellPosition cellPosition) {
			this.cellPosition = cellPosition;
		}
		public XlPtgRef(XlCellPosition cellPosition, XlPtgDataType dataType)
			: base(dataType) {
			this.cellPosition = cellPosition;
		}
		public override short TypeCode { get { return XlPtgTypeCode.Ref; } }
		public XlCellPosition CellPosition {
			get { return cellPosition; }
			set {
				cellPosition = value;
			}
		}
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgRef3d
	public class XlPtgRef3d : XlPtgRef {
		string sheetName;
		public XlPtgRef3d(XlCellPosition cellPosition, string sheetName)
			: base(cellPosition) {
			this.sheetName = sheetName;
		}
		public XlPtgRef3d(XlCellPosition cellPosition, string sheetName, XlPtgDataType dataType)
			: base(cellPosition, dataType) {
			this.sheetName = sheetName;
		}
		public override short TypeCode { get { return XlPtgTypeCode.Ref3d; } }
		public string SheetName { get { return this.sheetName; } set { this.sheetName = value; } }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgArea
	public class XlPtgArea : XlPtgWithDataType {
		XlCellPosition topLeft;
		XlCellPosition bottomRight;
		public XlPtgArea(XlCellPosition topLeft, XlCellPosition bottomRight) {
			this.topLeft = topLeft;
			this.bottomRight = bottomRight;
		}
		public XlPtgArea(XlCellPosition topLeft, XlCellPosition bottomRight, XlPtgDataType dataType)
			: base(dataType) {
			this.topLeft = topLeft;
			this.bottomRight = bottomRight;
		}
		public XlPtgArea(XlCellRange range)
			: this(range.TopLeft, range.BottomRight) {
		}
		public override short TypeCode { get { return XlPtgTypeCode.Area; } }
		public XlCellPosition TopLeft {
			get { return topLeft; }
			set {
				topLeft = value;
			}
		}
		public XlCellPosition BottomRight {
			get { return bottomRight; }
			set {
				bottomRight = value;
			}
		}
		public XlCellRange CellRange { get { return new XlCellRange(topLeft, bottomRight); } }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgArea3d
	public class XlPtgArea3d : XlPtgArea {
		string sheetName;
		public XlPtgArea3d(XlCellPosition topLeft, XlCellPosition bottomRight, string sheetName)
			: base(topLeft, bottomRight) {
			this.sheetName = sheetName;
		}
		public XlPtgArea3d(XlCellPosition topLeft, XlCellPosition bottomRight, string sheetName, XlPtgDataType dataType)
			: base(topLeft, bottomRight, dataType) {
			this.sheetName = sheetName;
		}
		public XlPtgArea3d(XlCellRange range, string sheetName)
			: base(range) {
			this.sheetName = sheetName;
		}
		public override short TypeCode { get { return XlPtgTypeCode.Area3d; } }
		public string SheetName { get { return this.sheetName; } set { this.sheetName = value; } }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgRefN
	public class XlPtgRefN : XlPtgWithDataType {
		public XlPtgRefN(XlCellOffset cellOffset) {
			CellOffset = cellOffset;
		}
		public XlPtgRefN(XlCellOffset cellOffset, XlPtgDataType dataType)
			: base(dataType) {
			CellOffset = cellOffset;
		}
		public override short TypeCode { get { return XlPtgTypeCode.RefN; } }
		public XlCellOffset CellOffset { get; set; }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgRefN3d
	public class XlPtgRefN3d : XlPtgRefN {
		string sheetName;
		public XlPtgRefN3d(XlCellOffset cellOffset, string sheetName)
			: base(cellOffset) {
			this.sheetName = sheetName;
		}
		public XlPtgRefN3d(XlCellOffset cellOffset, string sheetName, XlPtgDataType dataType)
			: base(cellOffset, dataType) {
			this.sheetName = sheetName;
		}
		public override short TypeCode { get { return XlPtgTypeCode.Ref3d; } }
		public string SheetName { get { return this.sheetName; } set { this.sheetName = value; } }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgAreaN
	public class XlPtgAreaN : XlPtgWithDataType {
		public XlPtgAreaN(XlCellOffset topLeft, XlCellOffset bottomRight) {
			TopLeft = topLeft;
			BottomRight = bottomRight;
		}
		public XlPtgAreaN(XlCellOffset topLeft, XlCellOffset bottomRight, XlPtgDataType dataType)
			: base(dataType) {
			TopLeft = topLeft;
			BottomRight = bottomRight;
		}
		public override short TypeCode { get { return XlPtgTypeCode.AreaN; } }
		public XlCellOffset TopLeft { get; set; }
		public XlCellOffset BottomRight { get; set; }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgAreaN
	public class XlPtgAreaN3d : XlPtgAreaN {
		string sheetName;
		public XlPtgAreaN3d(XlCellOffset topLeft, XlCellOffset bottomRight, string sheetName)
			: base(topLeft, bottomRight) {
			this.sheetName = sheetName;
		}
		public XlPtgAreaN3d(XlCellOffset topLeft, XlCellOffset bottomRight, string sheetName, XlPtgDataType dataType)
			: base(topLeft, bottomRight, dataType) {
			this.sheetName = sheetName;
		}
		public override short TypeCode { get { return XlPtgTypeCode.Area3d; } }
		public string SheetName { get { return this.sheetName; } set { this.sheetName = value; } }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgRefErr
	public class XlPtgRefErr : XlPtgWithDataType {
		public XlPtgRefErr() {
		}
		public XlPtgRefErr(XlPtgDataType dataType)
			: base(dataType) {
		}
		public override short TypeCode { get { return XlPtgTypeCode.RefErr; } }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgAreaErr
	public class XlPtgAreaErr : XlPtgWithDataType {
		public XlPtgAreaErr() {
		}
		public XlPtgAreaErr(XlPtgDataType dataType)
			: base(dataType) {
		}
		public override short TypeCode { get { return XlPtgTypeCode.AreaErr; } }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgRefErr3d
	public class XlPtgRefErr3d : XlPtgWithDataType {
		public XlPtgRefErr3d(string sheetName)
			: base() {
			SheetName = sheetName;
		}
		public XlPtgRefErr3d(string sheetName, XlPtgDataType dataType)
			: base(dataType) {
			SheetName = sheetName;
		}
		public override short TypeCode { get { return XlPtgTypeCode.RefErr3d; } }
		public string SheetName { get; set; }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgAreaErr3d
	public class XlPtgAreaErr3d : XlPtgWithDataType {
		public XlPtgAreaErr3d(string sheetName)
			: base() {
			SheetName = sheetName;
		}
		public XlPtgAreaErr3d(string sheetName, XlPtgDataType dataType)
			: base(dataType) {
			SheetName = sheetName;
		}
		public override short TypeCode { get { return XlPtgTypeCode.AreaErr3d; } }
		public string SheetName { get; set; }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgArray
	public class XlPtgArray : XlPtgWithDataType {
		readonly int width;
		readonly int height;
		readonly IList<XlVariantValue> values;
		public XlPtgArray(int width, int height, IList<XlVariantValue> values, XlPtgDataType dataType)
			: base(dataType) {
			this.width = width;
			this.height = height;
			this.values = values;
		}
		public int Width { get { return width; } }
		public int Height { get { return height; } }
		public IList<XlVariantValue> Values { get { return values; } }
		public override short TypeCode { get { return XlPtgTypeCode.Array; } }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgName
	public class XlPtgName : XlPtgWithDataType {
		public XlPtgName(int nameIndex, XlPtgDataType dataType)
			: base(dataType) {
			NameIndex = nameIndex;
		}
		public int NameIndex { get; private set; }
		public override short TypeCode { get { return XlPtgTypeCode.Name; } }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region XlPtgExp
	public class XlPtgExp : XlPtgBase {
		XlCellPosition cellPosition;
		public XlPtgExp(XlCellPosition cellPosition) {
			this.cellPosition = cellPosition;
		}
		public override short TypeCode { get { return XlPtgTypeCode.Exp; } }
		public XlCellPosition CellPosition {
			get { return cellPosition; }
			set { cellPosition = value; }
		}
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region Mem
#region XlPtgMemBase
	public abstract class XlPtgMemBase : XlPtgWithDataType {
#region Fields
		int innerPtgCount;
#endregion
		protected XlPtgMemBase(int innerPtgCount) {
			this.innerPtgCount = innerPtgCount;
		}
		protected XlPtgMemBase(int innerPtgCount, XlPtgDataType dataType)
			: base(dataType) {
			this.innerPtgCount = innerPtgCount;
		}
#region Properties
		public int InnerThingCount { get { return innerPtgCount; } set { innerPtgCount = value; } }
#endregion
	}
#endregion
#region XlPtgMemFunc
	public class XlPtgMemFunc : XlPtgMemBase {
		public XlPtgMemFunc(int innerPtgCount)
			: base(innerPtgCount) {
		}
		public XlPtgMemFunc(int innerPtgCount, XlPtgDataType dataType)
			: base(innerPtgCount, dataType) {
		}
		public override short TypeCode { get { return XlPtgTypeCode.MemFunc; } }
		public override void Visit(IXlPtgVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#endregion
#region IXlColumnNameConverter
	public interface IXlColumnPositionConverter {
		int GetColumnIndex(string name);
		int GetTopRowForColumn(string name); 
	}
#endregion
#region ExpressionConversionException
	public class ExpressionConversionException : Exception {
		public ExpressionConversionException(string message)
			: base(message) {
		}
	}
#endregion
#region CriteriaOperatorToXlExpressionConverter
	public class CriteriaOperatorToXlExpressionConverter : IClientCriteriaVisitor<CriteriaPriorityClass> {
		static readonly int functionNotCode = 0x0026;
		static readonly int functionIsBlankCode = 0x0081;
		static readonly int functionBitwiseOrCode = 0x402C; 
		static readonly int functionBitwiseAndCode = 0x402A;
		static readonly int functionBitwiseXorCode = 0x402E;
		static readonly int functionModCode = 0x0027;
		static readonly int functionAndCode = 0x0024;
		static readonly int functionOrCode = 0x0025;
		static Dictionary<Data.Filtering.FunctionOperatorType, IFunctionConverter> functionConvertersList;
		readonly IXlColumnPositionConverter columnNameConverter;
		XlExpression expression;
		XlExpressionContext context;
		public CriteriaOperatorToXlExpressionConverter(IXlColumnPositionConverter columnNameConverter) {
			this.columnNameConverter = columnNameConverter;
			this.context = new XlExpressionContext();
		}
		public IXlExpressionContext Context { get { return context; } }
		void ThrowConversionError(string message) {
			throw new ExpressionConversionException(message);
		}
		protected internal XlExpression TestConvert(string expression) {
			OperandValue[] criteriaParametersList = new OperandValue[0];
			CriteriaOperator criteriaOperator = CriteriaOperator.Parse(expression, out criteriaParametersList);
			return Execute(criteriaOperator);
		}
		public XlExpression Execute(CriteriaOperator criteriaOperator) {
			expression = new XlExpression();
			criteriaOperator.Accept(this);
			return ParsedExpression;
		}
		CriteriaPriorityClass Process(CriteriaOperator criteriaOperator) {
			return criteriaOperator.Accept(this);
		}
		public XlExpression ParsedExpression { get { return expression; } }
		static Dictionary<Data.Filtering.FunctionOperatorType, IFunctionConverter> FunctionConvertersList {
			get {
				if (functionConvertersList == null)
					functionConvertersList = CreateFunctionConvertersList();
				return functionConvertersList;
			}
		}
		static Dictionary<Data.Filtering.FunctionOperatorType, IFunctionConverter> CreateFunctionConvertersList() {
			Dictionary<Data.Filtering.FunctionOperatorType, IFunctionConverter> result = new Dictionary<Data.Filtering.FunctionOperatorType, IFunctionConverter>();
			result.Add(Data.Filtering.FunctionOperatorType.Abs, new FunctionDefaultConverter(0x0018, 1));
			result.Add(Data.Filtering.FunctionOperatorType.Acos, new FunctionDefaultConverter(0x0063, 1));
			result.Add(Data.Filtering.FunctionOperatorType.Asin, new FunctionDefaultConverter(0x0062, 1));
			result.Add(Data.Filtering.FunctionOperatorType.Atn, new FunctionDefaultConverter(0x0012, 1));
			result.Add(Data.Filtering.FunctionOperatorType.Atn2, new FunctionDefaultConverter(0x0061, 2)); 
			result.Add(Data.Filtering.FunctionOperatorType.Ceiling, new FunctionCeilingConverter());
			result.Add(Data.Filtering.FunctionOperatorType.Char, new FunctionDefaultConverter(0x006F, 1));
			result.Add(Data.Filtering.FunctionOperatorType.Concat, new FunctionVarDefaultConverter(0x0150, 1, 256));
			result.Add(Data.Filtering.FunctionOperatorType.Contains, new FunctionContainsTextConverter());
			result.Add(Data.Filtering.FunctionOperatorType.Cos, new FunctionDefaultConverter(0x0010, 1));
			result.Add(Data.Filtering.FunctionOperatorType.Cosh, new FunctionDefaultConverter(0x00E6, 1));
			result.Add(Data.Filtering.FunctionOperatorType.EndsWith, new FunctionEndsWithConverter());
			result.Add(Data.Filtering.FunctionOperatorType.Exp, new FunctionDefaultConverter(0x0015, 1));
			result.Add(Data.Filtering.FunctionOperatorType.Floor, new FunctionFloorConverter());
			result.Add(Data.Filtering.FunctionOperatorType.IsNull, new FunctionDefaultConverter(0x0081, 1));
			result.Add(Data.Filtering.FunctionOperatorType.IsNullOrEmpty, new FunctionIsNullOrEmptyConverter());
			result.Add(Data.Filtering.FunctionOperatorType.Len, new FunctionDefaultConverter(0x0020, 1));
			result.Add(Data.Filtering.FunctionOperatorType.Log, new FunctionVarDefaultConverter(0x006D, 1, 2));
			result.Add(Data.Filtering.FunctionOperatorType.Log10, new FunctionDefaultConverter(0x0017, 1));
			result.Add(Data.Filtering.FunctionOperatorType.Lower, new FunctionDefaultConverter(0x0070, 1));
			result.Add(Data.Filtering.FunctionOperatorType.Max, new FunctionVarDefaultConverter(0x0007, 1, 256));
			result.Add(Data.Filtering.FunctionOperatorType.Min, new FunctionVarDefaultConverter(0x0006, 1, 256));
			result.Add(Data.Filtering.FunctionOperatorType.Now, new FunctionDefaultConverter(0x004A, 0));
			result.Add(Data.Filtering.FunctionOperatorType.Power, new FunctionDefaultConverter(0x0063, 2));
			result.Add(Data.Filtering.FunctionOperatorType.Rnd, new FunctionDefaultConverter(0x003F, 1));
			result.Add(Data.Filtering.FunctionOperatorType.Round, new FunctionRoundConverter());
			result.Add(Data.Filtering.FunctionOperatorType.Sign, new FunctionDefaultConverter(0x001A, 1));
			result.Add(Data.Filtering.FunctionOperatorType.Sin, new FunctionDefaultConverter(0x000F, 1));
			result.Add(Data.Filtering.FunctionOperatorType.Sinh, new FunctionDefaultConverter(0x00E5, 1));
			result.Add(Data.Filtering.FunctionOperatorType.StartsWith, new FunctionStartsWithConverter());
			result.Add(Data.Filtering.FunctionOperatorType.Tan, new FunctionDefaultConverter(0x0011, 1));
			result.Add(Data.Filtering.FunctionOperatorType.Tanh, new FunctionDefaultConverter(0x00E7, 1));
			result.Add(Data.Filtering.FunctionOperatorType.Trim, new FunctionDefaultConverter(0x0076, 1));
			result.Add(Data.Filtering.FunctionOperatorType.Upper, new FunctionDefaultConverter(0x0071, 1));
			return result;
		}
		void ProcessOperandList(Data.Filtering.CriteriaOperatorCollection collection) {
			foreach (Data.Filtering.CriteriaOperator innerOperator in collection) {
				innerOperator.Accept(this);
			}
		}
#region Operator
		public CriteriaPriorityClass Visit(Data.Filtering.OperandValue theOperand) {
			XlPtgBase convertedOperand = ConvertOperand(theOperand.Value);
			if (convertedOperand == null)
				ThrowConversionError("Operand can not be converted.");
			expression.Add(convertedOperand);
			return CriteriaPriorityClass.Atom;
		}
		XlPtgBase ConvertOperand(object value) {
			if (value == null || DXConvert.IsDBNull(value))
				return null;	
			Type type = value.GetType();
			if (type == typeof(string))
				return new XlPtgStr((string)value);
			if (type == typeof(bool))
				return new XlPtgBool((bool)value);
			if (type == typeof(double) ||
				type == typeof(int) ||
				type == typeof(long) ||
				type == typeof(decimal) ||
				type == typeof(float) ||
				type == typeof(short) ||
				type == typeof(byte) ||
				type == typeof(ushort) ||
				type == typeof(uint))
				return CreateParsedThingForDouble(Convert.ToDouble(value));
			return new XlPtgStr(value.ToString());
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		bool TryConvertOperandToXlValue(object value, out XlVariantValue result) {
			result = XlVariantValue.Empty;
			if (value == null || DXConvert.IsDBNull(value))
				return true;
			Type type = value.GetType();
			if (type == typeof(string)) {
				result = (string)value;
				return true;
			}
			if (type == typeof(bool)) {
				result = (bool)value;
				return true;
			}
			if (type == typeof(double) ||
				type == typeof(int) ||
				type == typeof(long) ||
				type == typeof(decimal) ||
				type == typeof(float) ||
				type == typeof(short) ||
				type == typeof(byte) ||
				type == typeof(ushort) ||
				type == typeof(uint)) {
				result = Convert.ToDouble(value);
				return true;
			}
			return false; 
		}
		public XlPtgBase CreateParsedThingForDouble(double numericValue) {
			double truncated = Math.Truncate(numericValue);
			if (truncated == numericValue && truncated >= 0 && truncated <= UInt16.MaxValue)
				return new XlPtgInt((int)truncated);
			return new XlPtgNum(numericValue);
		}
#endregion
#region Function
		public CriteriaPriorityClass Visit(Data.Filtering.FunctionOperator theOperator) {
			IFunctionConverter converter = GetConverter(theOperator.OperatorType);
			if (converter == null)
				ThrowConversionError("Function " + theOperator.OperatorType.ToString() + " can not be converted.");
			if (!converter.ConvertFunction(theOperator.Operands, this, expression))
				ThrowConversionError("An error occurred while converting function " + theOperator.OperatorType.ToString() + ".");
			return CriteriaPriorityClass.Atom;
		}
		IFunctionConverter GetConverter(Data.Filtering.FunctionOperatorType functionType) {
			IFunctionConverter result;
			if (FunctionConvertersList.TryGetValue(functionType, out result))
				return result;
			return null;
		}
#endregion
#region GroupOperator
		public CriteriaPriorityClass Visit(Data.Filtering.GroupOperator theOperator) {
			int operandsCount = theOperator.Operands.Count;
			switch (operandsCount) {
				case 0:
					ThrowConversionError("GroupOperator without operands can not be converted");
					break;
				case 1:
					return Process(theOperator.Operands[0]);
			}
			int funcCode;
			switch (theOperator.OperatorType) {
				case Data.Filtering.GroupOperatorType.And:
					funcCode = functionAndCode;
					break;
				case Data.Filtering.GroupOperatorType.Or:
					funcCode = functionOrCode;
					break;
				default:
					ThrowConversionError("Invalid GroupOperatorType" + theOperator.OperatorType.ToString());
					return default(CriteriaPriorityClass);
			}
			ProcessOperandList(theOperator.Operands);
			expression.Add(new XlPtgFuncVar(funcCode, operandsCount, XlPtgDataType.Value));
			return CriteriaPriorityClass.Atom;
		}
#endregion
#region Unary
		public CriteriaPriorityClass Visit(Data.Filtering.UnaryOperator theOperator) {
			XlPtgBase convertedOperator;
			CriteriaPriorityClass priority;
			bool operatorReplacedByFunction = false;
			switch (theOperator.OperatorType) {
				case Data.Filtering.UnaryOperatorType.Minus:
					convertedOperator = new XlPtgUnaryOperator(XlPtgTypeCode.Uminus);
					priority = CriteriaPriorityClass.Neg;
					break;
				case Data.Filtering.UnaryOperatorType.Plus:
					convertedOperator = new XlPtgUnaryOperator(XlPtgTypeCode.Uplus);
					priority = CriteriaPriorityClass.Neg;
					break;
				case Data.Filtering.UnaryOperatorType.Not: 
					convertedOperator = new XlPtgFunc(functionNotCode, XlPtgDataType.Value);
					priority = CriteriaPriorityClass.Atom;
					operatorReplacedByFunction = true;
					break;
				case Data.Filtering.UnaryOperatorType.IsNull: 
					convertedOperator = new XlPtgFunc(functionIsBlankCode, XlPtgDataType.Value);
					priority = CriteriaPriorityClass.IsNull;
					priority = CriteriaPriorityClass.Atom;
					operatorReplacedByFunction = true;
					break;
				case Data.Filtering.UnaryOperatorType.BitwiseNot:
					convertedOperator = null;
					priority = CriteriaPriorityClass.BinaryNot;
					break;
				default:
					ThrowConversionError(string.Empty);
					return default(CriteriaPriorityClass);
			}
			CriteriaPriorityClass operandPriority = Process(theOperator.Operand);
			if (!operatorReplacedByFunction && operandPriority > priority)
				expression.Add(new XlPtgParen());
			expression.Add(convertedOperator);
			return priority;
		}
#endregion
#region Binary
		public CriteriaPriorityClass Visit(Data.Filtering.BinaryOperator theOperator) {
			XlPtgBase convertedOperator;
			CriteriaPriorityClass priority;
			bool operatorReplacedByFunction = false;
			switch (theOperator.OperatorType) {
				case Data.Filtering.BinaryOperatorType.BitwiseAnd:
					convertedOperator = new XlPtgFunc(functionBitwiseAndCode, XlPtgDataType.Value); 
					priority = CriteriaPriorityClass.Atom;
					operatorReplacedByFunction = true;
					break;
				case Data.Filtering.BinaryOperatorType.BitwiseOr:
					convertedOperator = new XlPtgFunc(functionBitwiseOrCode, XlPtgDataType.Value); 
					priority = CriteriaPriorityClass.Atom;
					operatorReplacedByFunction = true;
					break;
				case Data.Filtering.BinaryOperatorType.BitwiseXor:
					convertedOperator = new XlPtgFunc(functionBitwiseXorCode, XlPtgDataType.Value); 
					priority = CriteriaPriorityClass.Atom;
					operatorReplacedByFunction = true;
					break;
				case Data.Filtering.BinaryOperatorType.Divide:
					convertedOperator = new XlPtgBinaryOperator(XlPtgTypeCode.Div);
					priority = CriteriaPriorityClass.Mul;
					break;
				case Data.Filtering.BinaryOperatorType.Equal:
					convertedOperator = new XlPtgBinaryOperator(XlPtgTypeCode.Eq);
					priority = CriteriaPriorityClass.CmpEq;
					break;
				case Data.Filtering.BinaryOperatorType.Greater:
					convertedOperator = new XlPtgBinaryOperator(XlPtgTypeCode.Gt);
					priority = CriteriaPriorityClass.CmpGt;
					break;
				case Data.Filtering.BinaryOperatorType.GreaterOrEqual:
					convertedOperator = new XlPtgBinaryOperator(XlPtgTypeCode.Ge);
					priority = CriteriaPriorityClass.CmpGt;
					break;
				case Data.Filtering.BinaryOperatorType.Less:
					convertedOperator = new XlPtgBinaryOperator(XlPtgTypeCode.Lt);
					priority = CriteriaPriorityClass.CmpGt;
					break;
				case Data.Filtering.BinaryOperatorType.LessOrEqual:
					convertedOperator = new XlPtgBinaryOperator(XlPtgTypeCode.Le);
					priority = CriteriaPriorityClass.CmpGt;
					break;
				case Data.Filtering.BinaryOperatorType.Minus:
					convertedOperator = new XlPtgBinaryOperator(XlPtgTypeCode.Sub);
					priority = CriteriaPriorityClass.Add;
					break;
				case Data.Filtering.BinaryOperatorType.Modulo:
					convertedOperator = new XlPtgFunc(functionModCode, XlPtgDataType.Value); 
					priority = CriteriaPriorityClass.Atom;
					operatorReplacedByFunction = true;
					break;
				case Data.Filtering.BinaryOperatorType.Multiply:
					convertedOperator = new XlPtgBinaryOperator(XlPtgTypeCode.Mul);
					priority = CriteriaPriorityClass.Mul;
					break;
				case Data.Filtering.BinaryOperatorType.NotEqual:
					convertedOperator = new XlPtgBinaryOperator(XlPtgTypeCode.Ne);
					priority = CriteriaPriorityClass.CmpEq;
					break;
				case Data.Filtering.BinaryOperatorType.Plus:
					convertedOperator = new XlPtgBinaryOperator(XlPtgTypeCode.Add);
					priority = CriteriaPriorityClass.Add;
					break;
#pragma warning disable 618
				case Data.Filtering.BinaryOperatorType.Like:
#pragma warning restore 618
				default:
					ThrowConversionError(string.Empty);
					return default(CriteriaPriorityClass);
			}
			CriteriaPriorityClass leftOperandPriority = Process(theOperator.LeftOperand);
			if (!operatorReplacedByFunction && leftOperandPriority > priority)
				expression.Add(new XlPtgParen());
			CriteriaPriorityClass rightOperandPriority = Process(theOperator.RightOperand);
			if (!operatorReplacedByFunction && rightOperandPriority >= priority)
				expression.Add(new XlPtgParen());
			expression.Add(convertedOperator);
			return priority;
		}
#endregion
#region Between
		public CriteriaPriorityClass Visit(Data.Filtering.BetweenOperator theOperator) {
			CriteriaPriorityClass priority = CriteriaPriorityClass.CmpGt;
			CriteriaPriorityClass beginExpressionPriority = Process(theOperator.BeginExpression);
			if (beginExpressionPriority > priority)
				expression.Add(new XlPtgParen());
			CriteriaPriorityClass testExpressionPriority = Process(theOperator.TestExpression);
			if (testExpressionPriority >= priority)
				expression.Add(new XlPtgParen());
			expression.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Le));
			testExpressionPriority = Process(theOperator.TestExpression);
			if (testExpressionPriority > priority)
				expression.Add(new XlPtgParen());
			CriteriaPriorityClass endExpressionPriority = Process(theOperator.EndExpression);
			if (endExpressionPriority >= priority)
				expression.Add(new XlPtgParen());
			expression.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Le));
			expression.Add(new XlPtgFuncVar(functionAndCode, 2, XlPtgDataType.Value));
			return CriteriaPriorityClass.Atom;
		}
#endregion
		XlPtgArray TryConvertOperandsToArray(CriteriaOperatorCollection operands) {
			List<XlVariantValue> values = new List<XlVariantValue>();
			foreach (CriteriaOperator operand in operands) {
				OperandValue operandValue = operand as OperandValue;
				if (object.ReferenceEquals(operandValue, null))
					return null;
				XlVariantValue operandXlValue;
				if (!TryConvertOperandToXlValue(operandValue.Value, out operandXlValue))
					return null;
				values.Add(operandXlValue);
			}
			return new XlPtgArray(values.Count, 1, values, XlPtgDataType.Value);
		}
#region InOperator
		public CriteriaPriorityClass Visit(Data.Filtering.InOperator theOperator) {
			ThrowConversionError("The \"in\" operator can be converted to Excel-compatible expression only using array constants which are illegal for conditional formatting rules.");
			return CriteriaPriorityClass.Atom; 
		}
#endregion
#region Join
		public CriteriaPriorityClass Visit(JoinOperand theOperand) {
			ThrowConversionError("Join operand can not be converted to expression.");
			return default(CriteriaPriorityClass);
		}
#endregion
#region OperandProperty
		public CriteriaPriorityClass Visit(OperandProperty theOperand) {
			string columnName = theOperand.PropertyName;
			int columnIndex = columnNameConverter.GetColumnIndex(columnName);
			if (columnIndex < 0)
				ThrowConversionError("Property \"" + columnName + "\" can not be converted to column.");
			if (Context.ReferenceMode == XlCellReferenceMode.Offset) {
				XlCellOffset position = new XlCellOffset(columnIndex, 0, XlCellOffsetType.Position, XlCellOffsetType.Offset); 
				XlPtgRefN thing = new XlPtgRefN(position) { DataType = XlPtgDataType.Value };
				expression.Add(thing);
			}
			else {
				XlCellPosition position = new XlCellPosition(columnIndex, Context.CurrentCell.Row, XlPositionType.Relative, XlPositionType.Relative);
				XlPtgRef thing = new XlPtgRef(position, XlPtgDataType.Value);
				expression.Add(thing);
			}
			return CriteriaPriorityClass.Atom;
		}
#endregion
#region Aggregate
		public CriteriaPriorityClass Visit(AggregateOperand theOperand) {
			ThrowConversionError("Aggregate operand can not be converted to expression.");
			return default(CriteriaPriorityClass);
		}
#endregion
	}
	interface IFunctionConverter {
		bool ConvertFunction(CriteriaOperatorCollection operands, IClientCriteriaVisitor<CriteriaPriorityClass> visitor, XlExpression expression);
	}
#region FunctionConverter
	abstract class FunctionConverter : IFunctionConverter {
		protected FunctionConverter() {
		}
		protected void ProcessOperandList(Data.Filtering.CriteriaOperatorCollection collection, IClientCriteriaVisitor<CriteriaPriorityClass> visitor) {
			foreach (Data.Filtering.CriteriaOperator innerOperator in collection) {
				innerOperator.Accept(visitor);
			}
		}
		protected void ProcessOperand(Data.Filtering.CriteriaOperatorCollection collection, IClientCriteriaVisitor<CriteriaPriorityClass> visitor, int index) {
			collection[index].Accept(visitor);
		}
		public abstract bool ConvertFunction(Data.Filtering.CriteriaOperatorCollection operands, IClientCriteriaVisitor<CriteriaPriorityClass> visitor, XlExpression expression);
		protected XlPtgBase CreateFuncThing(int funcCode) {
			return new XlPtgFunc(funcCode, XlPtgDataType.Value); 
		}
		protected XlPtgBase CreateFuncVarThing(int funcCode, int paramCount) {
			return new XlPtgFuncVar(funcCode, paramCount, XlPtgDataType.Value); 
		}
	}
#endregion
#region FunctionDefaultConverter
	class FunctionDefaultConverter : FunctionConverter {
		readonly int funcCode;
		readonly int expectedParamCount;
		public FunctionDefaultConverter(int funcCode, int expectedParamCount) {
			this.funcCode = funcCode;
			this.expectedParamCount = expectedParamCount;
		}
		public int ExpectedParamCount { get { return expectedParamCount; } }
		public int FuncCode { get { return funcCode; } }
		public override bool ConvertFunction(Data.Filtering.CriteriaOperatorCollection operands, IClientCriteriaVisitor<CriteriaPriorityClass> visitor, XlExpression expression) {
			int actualParamsCount = operands.Count;
			if (!CheckCompliance(actualParamsCount))
				return false;
			ProcessOperandList(operands, visitor);
			expression.Add(PrepareFuncThing(actualParamsCount));
			return true;
		}
		protected virtual bool CheckCompliance(int actualParamsCount) {
			return actualParamsCount == expectedParamCount;
		}
		protected virtual XlPtgBase PrepareFuncThing(int actualParamsCount) {
			return CreateFuncThing(FuncCode);
		}
	}
#endregion
#region FunctionVarDefaultConverter
	class FunctionVarDefaultConverter : FunctionDefaultConverter {
		readonly int expectedMaxParamCount;
		public FunctionVarDefaultConverter(int funcCode, int expectedMinParamCount, int expectedMaxParamCount)
			: base(funcCode, expectedMinParamCount) {
			this.expectedMaxParamCount = expectedMaxParamCount;
		}
		public int ExpectedMaxParamCount { get { return expectedMaxParamCount; } }
		protected override bool CheckCompliance(int actualParamsCount) {
			return actualParamsCount >= ExpectedParamCount && actualParamsCount <= ExpectedMaxParamCount;
		}
		protected override XlPtgBase PrepareFuncThing(int actualParamsCount) {
			return CreateFuncVarThing(FuncCode, actualParamsCount);
		}
	}
#endregion
#region FunctionStartsWithConverter
	class FunctionStartsWithConverter : FunctionConverter {
		public override bool ConvertFunction(Data.Filtering.CriteriaOperatorCollection operands, IClientCriteriaVisitor<CriteriaPriorityClass> visitor, XlExpression expression) {
			if (!CheckCompliance(operands.Count))
				return false;
			ProcessOperandList(operands, visitor);
			expression.Add(CreateFuncThing(0x0020));
			expression.Add(CreateFuncVarThing(0x0073, 2));
			ProcessOperand(operands, visitor, 1);
			expression.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Eq));
			return true;
		}
		bool CheckCompliance(int actualParamsCount) {
			return actualParamsCount == 2;
		}
	}
#endregion
#region FunctionEndsWithConverter
	class FunctionEndsWithConverter : FunctionConverter {
		public override bool ConvertFunction(Data.Filtering.CriteriaOperatorCollection operands, IClientCriteriaVisitor<CriteriaPriorityClass> visitor, XlExpression expression) {
			if (!CheckCompliance(operands.Count))
				return false;
			ProcessOperandList(operands, visitor);
			expression.Add(CreateFuncThing(0x0020));
			expression.Add(CreateFuncVarThing(0x0074, 2));
			ProcessOperand(operands, visitor, 1);
			expression.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Eq));
			return true;
		}
		bool CheckCompliance(int actualParamsCount) {
			return actualParamsCount == 2;
		}
	}
#endregion
#region FunctionContainsTextConverter
	class FunctionContainsTextConverter : FunctionConverter {
		public override bool ConvertFunction(Data.Filtering.CriteriaOperatorCollection operands, IClientCriteriaVisitor<CriteriaPriorityClass> visitor, XlExpression expression) {
			if (!CheckCompliance(operands.Count))
				return false;
			ProcessOperand(operands, visitor, 1);
			ProcessOperand(operands, visitor, 0);
			expression.Add(CreateFuncVarThing(0x0052, 2));
			expression.Add(CreateFuncThing(0x0003));
			expression.Add(CreateFuncThing(0x0026));
			return true;
		}
		bool CheckCompliance(int actualParamsCount) {
			return actualParamsCount == 2;
		}
	}
#endregion
#region FunctionCeilinigConverter
	class FunctionCeilingConverter : FunctionConverter {
		public override bool ConvertFunction(Data.Filtering.CriteriaOperatorCollection operands, IClientCriteriaVisitor<CriteriaPriorityClass> visitor, XlExpression expression) {
			if (!CheckCompliance(operands.Count))
				return false;
			ProcessOperand(operands, visitor, 0);
			expression.Add(new XlPtgInt(1));
			expression.Add(CreateFuncThing(0x0120));
			return true;
		}
		bool CheckCompliance(int actualParamsCount) {
			return actualParamsCount == 1;
		}
	}
#endregion
#region FunctionFloorConverter
	class FunctionFloorConverter : FunctionConverter {
		public override bool ConvertFunction(Data.Filtering.CriteriaOperatorCollection operands, IClientCriteriaVisitor<CriteriaPriorityClass> visitor, XlExpression expression) {
			if (!CheckCompliance(operands.Count))
				return false;
			ProcessOperand(operands, visitor, 0);
			expression.Add(new XlPtgInt(1));
			expression.Add(CreateFuncThing(0x011D));
			return true;
		}
		bool CheckCompliance(int actualParamsCount) {
			return actualParamsCount == 1;
		}
	}
#endregion
#region FunctionRoundConverter
	class FunctionRoundConverter : FunctionConverter {
		public override bool ConvertFunction(Data.Filtering.CriteriaOperatorCollection operands, IClientCriteriaVisitor<CriteriaPriorityClass> visitor, XlExpression expression) {
			int operandsCount = operands.Count;
			if (!CheckCompliance(operandsCount))
				return false;
			ProcessOperandList(operands, visitor);
			if (operandsCount == 1)
				expression.Add(new XlPtgInt(0));
			expression.Add(CreateFuncThing(0x001B));
			return true;
		}
		bool CheckCompliance(int actualParamsCount) {
			return actualParamsCount >= 1 && actualParamsCount <= 2;
		}
	}
#endregion
#region FunctionIsNullOrEmptyConverter
	class FunctionIsNullOrEmptyConverter : FunctionConverter {
		public override bool ConvertFunction(Data.Filtering.CriteriaOperatorCollection operands, IClientCriteriaVisitor<CriteriaPriorityClass> visitor, XlExpression expression) {
			int operandsCount = operands.Count;
			if (!CheckCompliance(operands.Count))
				return false;
			ProcessOperandList(operands, visitor);
			expression.Add(new XlPtgStr(""));
			expression.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Eq));
			return true;
		}
		bool CheckCompliance(int actualParamsCount) {
			return actualParamsCount == 1;
		}
	}
#endregion
#endregion
#region ExpressionStringBuilder
	public class XlExpressionStringBuilder : IXlPtgVisitor {
		static readonly Dictionary<XlCellErrorType, string> errCodeToTextConversionTable = CreateErrorCodeToFunctionInfoConversionTable();
		static readonly Dictionary<int, XlFunctionInfo> functionCodeToFunctionInfoConversionTable = CreateFunctionCodeToNameConversionTable();
		static readonly string trueConstant = "TRUE";
		static readonly string falseConstant = "FALSE";
		static readonly string FUTURE_FUNCTION_PREFIX = "_xlfn.";
		static Dictionary<XlCellErrorType, string> CreateErrorCodeToFunctionInfoConversionTable() {
			Dictionary<XlCellErrorType, string> result = new Dictionary<XlCellErrorType, string>();
			result.Add(XlCellErrorType.DivisionByZero, "#DIV/0!");
			result.Add(XlCellErrorType.NotAvailable, "#N/A");
			result.Add(XlCellErrorType.Name, "#NAME?");
			result.Add(XlCellErrorType.Null, "#NULL!");
			result.Add(XlCellErrorType.Number, "#NUM!");
			result.Add(XlCellErrorType.Reference, "#REF!");
			result.Add(XlCellErrorType.Value, "#VALUE!");
			return result;
		}
		static Dictionary<int, XlFunctionInfo> CreateFunctionCodeToNameConversionTable() {
			Dictionary<int, XlFunctionInfo> result = new Dictionary<int, XlFunctionInfo>();
			result.Add(270, new XlFunctionInfo("BETADIST", 4, 5, XlFunctionProperty.Normal));
			result.Add(272, new XlFunctionInfo("BETAINV", 4, 5, XlFunctionProperty.Normal));
			result.Add(273, new XlFunctionInfo("BINOMDIST", 4, 4, XlFunctionProperty.Normal));
			result.Add(274, new XlFunctionInfo("CHIDIST", 2, 2, XlFunctionProperty.Normal));
			result.Add(275, new XlFunctionInfo("CHIINV", 2, 2, XlFunctionProperty.Normal));
			result.Add(306, new XlFunctionInfo("CHITEST", 2, 2, XlFunctionProperty.Normal));
			result.Add(277, new XlFunctionInfo("CONFIDENCE", 3, 3, XlFunctionProperty.Normal));
			result.Add(308, new XlFunctionInfo("COVAR", 2, 2, XlFunctionProperty.Normal));
			result.Add(278, new XlFunctionInfo("CRITBINOM", 3, 3, XlFunctionProperty.Normal));
			result.Add(280, new XlFunctionInfo("EXPONDIST", 3, 3, XlFunctionProperty.Normal));
			result.Add(281, new XlFunctionInfo("FDIST", 3, 3, XlFunctionProperty.Normal));
			result.Add(282, new XlFunctionInfo("FINV", 3, 3, XlFunctionProperty.Normal));
			result.Add(310, new XlFunctionInfo("FTEST", 2, 2, XlFunctionProperty.Normal));
			result.Add(286, new XlFunctionInfo("GAMMADIST", 4, 4, XlFunctionProperty.Normal));
			result.Add(287, new XlFunctionInfo("GAMMAINV", 3, 3, XlFunctionProperty.Normal));
			result.Add(289, new XlFunctionInfo("HYPGEOMDIST", 4, 4, XlFunctionProperty.Normal));
			result.Add(291, new XlFunctionInfo("LOGINV", 3, 3, XlFunctionProperty.Normal));
			result.Add(290, new XlFunctionInfo("LOGNORMDIST", 3, 3, XlFunctionProperty.Normal));
			result.Add(330, new XlFunctionInfo("MODE", 1, 255, XlFunctionProperty.Normal));
			result.Add(292, new XlFunctionInfo("NEGBINOMDIST", 3, 3, XlFunctionProperty.Normal));
			result.Add(293, new XlFunctionInfo("NORMDIST", 4, 4, XlFunctionProperty.Normal));
			result.Add(295, new XlFunctionInfo("NORMINV", 3, 3, XlFunctionProperty.Normal));
			result.Add(294, new XlFunctionInfo("NORMSDIST", 1, 1, XlFunctionProperty.Normal));
			result.Add(296, new XlFunctionInfo("NORMSINV", 1, 1, XlFunctionProperty.Normal));
			result.Add(328, new XlFunctionInfo("PERCENTILE", 2, 2, XlFunctionProperty.Normal));
			result.Add(329, new XlFunctionInfo("PERCENTRANK", 2, 3, XlFunctionProperty.Normal));
			result.Add(300, new XlFunctionInfo("POISSON", 3, 3, XlFunctionProperty.Normal));
			result.Add(327, new XlFunctionInfo("QUARTILE", 2, 2, XlFunctionProperty.Normal));
			result.Add(216, new XlFunctionInfo("RANK", 2, 3, XlFunctionProperty.Normal));
			result.Add(12, new XlFunctionInfo("STDEV", 1, 255, XlFunctionProperty.Normal));
			result.Add(193, new XlFunctionInfo("STDEVP", 1, 255, XlFunctionProperty.Normal));
			result.Add(301, new XlFunctionInfo("TDIST", 3, 3, XlFunctionProperty.Normal));
			result.Add(332, new XlFunctionInfo("TINV", 2, 2, XlFunctionProperty.Normal));
			result.Add(316, new XlFunctionInfo("TTEST", 4, 4, XlFunctionProperty.Normal));
			result.Add(46, new XlFunctionInfo("VAR", 1, 255, XlFunctionProperty.Normal));
			result.Add(194, new XlFunctionInfo("VARP", 1, 255, XlFunctionProperty.Normal));
			result.Add(302, new XlFunctionInfo("WEIBULL", 4, 4, XlFunctionProperty.Normal));
			result.Add(324, new XlFunctionInfo("ZTEST", 2, 3, XlFunctionProperty.Normal));
			result.Add(42, new XlFunctionInfo("DAVERAGE", 3, 3, XlFunctionProperty.Normal));
			result.Add(40, new XlFunctionInfo("DCOUNT", 3, 3, XlFunctionProperty.Normal));
			result.Add(199, new XlFunctionInfo("DCOUNTA", 3, 3, XlFunctionProperty.Normal));
			result.Add(235, new XlFunctionInfo("DGET", 3, 3, XlFunctionProperty.Normal));
			result.Add(44, new XlFunctionInfo("DMAX", 3, 3, XlFunctionProperty.Normal));
			result.Add(43, new XlFunctionInfo("DMIN", 3, 3, XlFunctionProperty.Normal));
			result.Add(189, new XlFunctionInfo("DPRODUCT", 3, 3, XlFunctionProperty.Normal));
			result.Add(45, new XlFunctionInfo("DSTDEV", 3, 3, XlFunctionProperty.Normal));
			result.Add(195, new XlFunctionInfo("DSTDEVP", 3, 3, XlFunctionProperty.Normal));
			result.Add(41, new XlFunctionInfo("DSUM", 3, 3, XlFunctionProperty.Normal));
			result.Add(47, new XlFunctionInfo("DVAR", 3, 3, XlFunctionProperty.Normal));
			result.Add(196, new XlFunctionInfo("DVARP", 3, 3, XlFunctionProperty.Normal));
			result.Add(65, new XlFunctionInfo("DATE", 3, 3, XlFunctionProperty.Normal));
			result.Add(140, new XlFunctionInfo("DATEVALUE", 1, 1, XlFunctionProperty.Normal));
			result.Add(67, new XlFunctionInfo("DAY", 1, 1, XlFunctionProperty.Normal));
			result.Add(16450, new XlFunctionInfo("DAYS", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(220, new XlFunctionInfo("DAYS360", 2, 3, XlFunctionProperty.Normal));
			result.Add(449, new XlFunctionInfo("EDATE", 2, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(450, new XlFunctionInfo("EOMONTH", 2, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(71, new XlFunctionInfo("HOUR", 1, 1, XlFunctionProperty.Normal));
			result.Add(16451, new XlFunctionInfo("ISOWEEKNUM", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(72, new XlFunctionInfo("MINUTE", 1, 1, XlFunctionProperty.Normal));
			result.Add(68, new XlFunctionInfo("MONTH", 1, 1, XlFunctionProperty.Normal));
			result.Add(472, new XlFunctionInfo("NETWORKDAYS", 2, 3, XlFunctionProperty.Excel2010Predefined));
			result.Add(16399, new XlFunctionInfo("NETWORKDAYS.INTL", 3, 4, XlFunctionProperty.Excel2010Future));
			result.Add(74, new XlFunctionInfo("NOW", 0, 0, XlFunctionProperty.Normal));
			result.Add(73, new XlFunctionInfo("SECOND", 1, 1, XlFunctionProperty.Normal));
			result.Add(66, new XlFunctionInfo("TIME", 3, 3, XlFunctionProperty.Normal));
			result.Add(141, new XlFunctionInfo("TIMEVALUE", 1, 1, XlFunctionProperty.Normal));
			result.Add(221, new XlFunctionInfo("TODAY", 0, 0, XlFunctionProperty.Normal));
			result.Add(70, new XlFunctionInfo("WEEKDAY", 1, 2, XlFunctionProperty.Normal));
			result.Add(465, new XlFunctionInfo("WEEKNUM", 1, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(471, new XlFunctionInfo("WORKDAY", 2, 3, XlFunctionProperty.Excel2010Predefined));
			result.Add(16398, new XlFunctionInfo("WORKDAY.INTL", 3, 4, XlFunctionProperty.Excel2010Future));
			result.Add(69, new XlFunctionInfo("YEAR", 1, 1, XlFunctionProperty.Normal));
			result.Add(451, new XlFunctionInfo("YEARFRAC", 2, 3, XlFunctionProperty.Excel2010Predefined));
			result.Add(351, new XlFunctionInfo("DATEDIF", 3, 3, XlFunctionProperty.Normal));
			result.Add(428, new XlFunctionInfo("BESSELI", 2, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(425, new XlFunctionInfo("BESSELJ", 2, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(426, new XlFunctionInfo("BESSELK", 2, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(427, new XlFunctionInfo("BESSELY", 2, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(393, new XlFunctionInfo("BIN2DEC", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(395, new XlFunctionInfo("BIN2HEX", 1, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(394, new XlFunctionInfo("BIN2OCT", 1, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(16426, new XlFunctionInfo("BITAND", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(16427, new XlFunctionInfo("BITLSHIFT", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(16428, new XlFunctionInfo("BITOR", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(16429, new XlFunctionInfo("BITRSHIFT", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(16430, new XlFunctionInfo("BITXOR", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(411, new XlFunctionInfo("COMPLEX", 2, 3, XlFunctionProperty.Excel2010Predefined));
			result.Add(468, new XlFunctionInfo("CONVERT", 3, 3, XlFunctionProperty.Excel2010Predefined));
			result.Add(387, new XlFunctionInfo("DEC2BIN", 1, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(388, new XlFunctionInfo("DEC2HEX", 1, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(389, new XlFunctionInfo("DEC2OCT", 1, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(418, new XlFunctionInfo("DELTA", 1, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(423, new XlFunctionInfo("ERF", 1, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(16401, new XlFunctionInfo("ERF.PRECISE", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(424, new XlFunctionInfo("ERFC", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(16402, new XlFunctionInfo("ERFC.PRECISE", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(419, new XlFunctionInfo("GESTEP", 1, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(384, new XlFunctionInfo("HEX2BIN", 1, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(385, new XlFunctionInfo("HEX2DEC", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(386, new XlFunctionInfo("HEX2OCT", 1, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(399, new XlFunctionInfo("IMABS", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(409, new XlFunctionInfo("IMAGINARY", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(407, new XlFunctionInfo("IMARGUMENT", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(408, new XlFunctionInfo("IMCONJUGATE", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(405, new XlFunctionInfo("IMCOS", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(16431, new XlFunctionInfo("IMCOSH", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(16432, new XlFunctionInfo("IMCOT", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(16433, new XlFunctionInfo("IMCSC", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(16434, new XlFunctionInfo("IMCSCH", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(397, new XlFunctionInfo("IMDIV", 2, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(406, new XlFunctionInfo("IMEXP", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(401, new XlFunctionInfo("IMLN", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(403, new XlFunctionInfo("IMLOG10", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(402, new XlFunctionInfo("IMLOG2", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(398, new XlFunctionInfo("IMPOWER", 2, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(413, new XlFunctionInfo("IMPRODUCT", 1, 255, XlFunctionProperty.Excel2010Predefined));
			result.Add(410, new XlFunctionInfo("IMREAL", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(16435, new XlFunctionInfo("IMSEC", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(16436, new XlFunctionInfo("IMSECH", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(404, new XlFunctionInfo("IMSIN", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(16437, new XlFunctionInfo("IMSINH", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(400, new XlFunctionInfo("IMSQRT", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(396, new XlFunctionInfo("IMSUB", 2, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(412, new XlFunctionInfo("IMSUM", 1, 255, XlFunctionProperty.Excel2010Predefined));
			result.Add(16438, new XlFunctionInfo("IMTAN", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(390, new XlFunctionInfo("OCT2BIN", 1, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(392, new XlFunctionInfo("OCT2DEC", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(391, new XlFunctionInfo("OCT2HEX", 1, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(470, new XlFunctionInfo("ACCRINTM", 4, 5, XlFunctionProperty.Excel2010Predefined));
			result.Add(452, new XlFunctionInfo("COUPDAYBS", 3, 4, XlFunctionProperty.Excel2010Predefined));
			result.Add(455, new XlFunctionInfo("COUPNCD", 3, 4, XlFunctionProperty.Excel2010Predefined));
			result.Add(456, new XlFunctionInfo("COUPNUM", 3, 4, XlFunctionProperty.Excel2010Predefined));
			result.Add(457, new XlFunctionInfo("COUPPCD", 3, 4, XlFunctionProperty.Excel2010Predefined));
			result.Add(448, new XlFunctionInfo("CUMIPMT", 6, 6, XlFunctionProperty.Excel2010Predefined));
			result.Add(447, new XlFunctionInfo("CUMPRINC", 6, 6, XlFunctionProperty.Excel2010Predefined));
			result.Add(247, new XlFunctionInfo("DB", 4, 5, XlFunctionProperty.Normal));
			result.Add(144, new XlFunctionInfo("DDB", 4, 5, XlFunctionProperty.Normal));
			result.Add(435, new XlFunctionInfo("DISC", 4, 5, XlFunctionProperty.Excel2010Predefined));
			result.Add(443, new XlFunctionInfo("DOLLARDE", 2, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(444, new XlFunctionInfo("DOLLARFR", 2, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(446, new XlFunctionInfo("EFFECT", 2, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(57, new XlFunctionInfo("FV", 4, 5, XlFunctionProperty.Normal));
			result.Add(476, new XlFunctionInfo("FVSCHEDULE", 2, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(433, new XlFunctionInfo("INTRATE", 4, 5, XlFunctionProperty.Excel2010Predefined));
			result.Add(167, new XlFunctionInfo("IPMT", 5, 6, XlFunctionProperty.Normal));
			result.Add(62, new XlFunctionInfo("IRR", 1, 2, XlFunctionProperty.Normal));
			result.Add(350, new XlFunctionInfo("ISPMT", 4, 4, XlFunctionProperty.Normal));
			result.Add(61, new XlFunctionInfo("MIRR", 3, 3, XlFunctionProperty.Normal));
			result.Add(445, new XlFunctionInfo("NOMINAL", 2, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(58, new XlFunctionInfo("NPER", 4, 5, XlFunctionProperty.Normal));
			result.Add(11, new XlFunctionInfo("NPV", 2, 255, XlFunctionProperty.Normal));
			result.Add(16442, new XlFunctionInfo("PDURATION", 3, 3, XlFunctionProperty.Excel2010Future));
			result.Add(59, new XlFunctionInfo("PMT", 4, 5, XlFunctionProperty.Normal));
			result.Add(168, new XlFunctionInfo("PPMT", 5, 6, XlFunctionProperty.Normal));
			result.Add(436, new XlFunctionInfo("PRICEDISC", 4, 5, XlFunctionProperty.Excel2010Predefined));
			result.Add(56, new XlFunctionInfo("PV", 4, 5, XlFunctionProperty.Normal));
			result.Add(60, new XlFunctionInfo("RATE", 5, 6, XlFunctionProperty.Normal));
			result.Add(434, new XlFunctionInfo("RECEIVED", 4, 5, XlFunctionProperty.Excel2010Predefined));
			result.Add(142, new XlFunctionInfo("SLN", 3, 3, XlFunctionProperty.Normal));
			result.Add(143, new XlFunctionInfo("SYD", 4, 4, XlFunctionProperty.Normal));
			result.Add(438, new XlFunctionInfo("TBILLEQ", 3, 3, XlFunctionProperty.Excel2010Predefined));
			result.Add(439, new XlFunctionInfo("TBILLPRICE", 3, 3, XlFunctionProperty.Excel2010Predefined));
			result.Add(440, new XlFunctionInfo("TBILLYIELD", 3, 3, XlFunctionProperty.Excel2010Predefined));
			result.Add(222, new XlFunctionInfo("VDB", 6, 7, XlFunctionProperty.Normal));
			result.Add(16443, new XlFunctionInfo("RRI", 3, 3, XlFunctionProperty.Excel2010Future));
			result.Add(430, new XlFunctionInfo("XNPV", 3, 3, XlFunctionProperty.Excel2010Predefined));
			result.Add(437, new XlFunctionInfo("YIELDDISC", 4, 5, XlFunctionProperty.Excel2010Predefined));
			result.Add(125, new XlFunctionInfo("CELL", 1, 2, XlFunctionProperty.Normal));
			result.Add(261, new XlFunctionInfo("ERROR.TYPE", 1, 1, XlFunctionProperty.Normal));
			result.Add(244, new XlFunctionInfo("INFO", 1, 1, XlFunctionProperty.Normal));
			result.Add(129, new XlFunctionInfo("ISBLANK", 1, 1, XlFunctionProperty.Normal));
			result.Add(126, new XlFunctionInfo("ISERR", 1, 1, XlFunctionProperty.Normal));
			result.Add(3, new XlFunctionInfo("ISERROR", 1, 1, XlFunctionProperty.Normal));
			result.Add(420, new XlFunctionInfo("ISEVEN", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(16447, new XlFunctionInfo("ISFORMULA", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(198, new XlFunctionInfo("ISLOGICAL", 1, 1, XlFunctionProperty.Normal));
			result.Add(2, new XlFunctionInfo("ISNA", 1, 1, XlFunctionProperty.Normal));
			result.Add(190, new XlFunctionInfo("ISNONTEXT", 1, 1, XlFunctionProperty.Normal));
			result.Add(128, new XlFunctionInfo("ISNUMBER", 1, 1, XlFunctionProperty.Normal));
			result.Add(421, new XlFunctionInfo("ISODD", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(105, new XlFunctionInfo("ISREF", 1, 1, XlFunctionProperty.Normal));
			result.Add(127, new XlFunctionInfo("ISTEXT", 1, 1, XlFunctionProperty.Normal));
			result.Add(131, new XlFunctionInfo("N", 1, 1, XlFunctionProperty.Normal));
			result.Add(10, new XlFunctionInfo("NA", 0, 0, XlFunctionProperty.Normal));
			result.Add(16448, new XlFunctionInfo("SHEET", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(16449, new XlFunctionInfo("SHEETS", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(86, new XlFunctionInfo("TYPE", 1, 1, XlFunctionProperty.Normal));
			result.Add(36, new XlFunctionInfo("AND", 1, 255, XlFunctionProperty.Normal));
			result.Add(35, new XlFunctionInfo("FALSE", 0, 0, XlFunctionProperty.Normal));
			result.Add(1, new XlFunctionInfo("IF", 2, 3, XlFunctionProperty.Normal));
			result.Add(480, new XlFunctionInfo("IFERROR", 2, 2, XlFunctionProperty.Excel2003Future));
			result.Add(16444, new XlFunctionInfo("IFNA", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(38, new XlFunctionInfo("NOT", 1, 1, XlFunctionProperty.Normal));
			result.Add(37, new XlFunctionInfo("OR", 1, 255, XlFunctionProperty.Normal));
			result.Add(34, new XlFunctionInfo("TRUE", 0, 0, XlFunctionProperty.Normal));
			result.Add(16445, new XlFunctionInfo("XOR", 1, 255, XlFunctionProperty.Excel2010Future));
			result.Add(219, new XlFunctionInfo("ADDRESS", 4, 5, XlFunctionProperty.Normal));
			result.Add(75, new XlFunctionInfo("AREAS", 1, 1, XlFunctionProperty.Normal));
			result.Add(100, new XlFunctionInfo("CHOOSE", 2, 255, XlFunctionProperty.Normal));
			result.Add(9, new XlFunctionInfo("COLUMN", 0, 1, XlFunctionProperty.Normal));
			result.Add(77, new XlFunctionInfo("COLUMNS", 1, 1, XlFunctionProperty.Normal));
			result.Add(16446, new XlFunctionInfo("FORMULATEXT", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(358, new XlFunctionInfo("GETPIVOTDATA", 2, 255, XlFunctionProperty.Normal));
			result.Add(101, new XlFunctionInfo("HLOOKUP", 3, 4, XlFunctionProperty.Normal));
			result.Add(359, new XlFunctionInfo("HYPERLINK", 1, 2, XlFunctionProperty.Normal));
			result.Add(29, new XlFunctionInfo("INDEX", 3, 4, XlFunctionProperty.Normal));
			result.Add(148, new XlFunctionInfo("INDIRECT", 1, 2, XlFunctionProperty.Normal));
			result.Add(28, new XlFunctionInfo("LOOKUP", 2, 3, XlFunctionProperty.Normal));
			result.Add(64, new XlFunctionInfo("MATCH", 2, 3, XlFunctionProperty.Normal));
			result.Add(78, new XlFunctionInfo("OFFSET", 4, 5, XlFunctionProperty.Normal));
			result.Add(8, new XlFunctionInfo("ROW", 0, 1, XlFunctionProperty.Normal));
			result.Add(76, new XlFunctionInfo("ROWS", 1, 1, XlFunctionProperty.Normal));
			result.Add(379, new XlFunctionInfo("RTD", 3, 255, XlFunctionProperty.Normal));
			result.Add(83, new XlFunctionInfo("TRANSPOSE", 1, 1, XlFunctionProperty.Normal));
			result.Add(102, new XlFunctionInfo("VLOOKUP", 3, 4, XlFunctionProperty.Normal));
			result.Add(24, new XlFunctionInfo("ABS", 1, 1, XlFunctionProperty.Normal));
			result.Add(99, new XlFunctionInfo("ACOS", 1, 1, XlFunctionProperty.Normal));
			result.Add(233, new XlFunctionInfo("ACOSH", 1, 1, XlFunctionProperty.Normal));
			result.Add(16405, new XlFunctionInfo("ACOT", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(16406, new XlFunctionInfo("ACOTH", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(16407, new XlFunctionInfo("ARABIC", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(98, new XlFunctionInfo("ASIN", 1, 1, XlFunctionProperty.Normal));
			result.Add(232, new XlFunctionInfo("ASINH", 1, 1, XlFunctionProperty.Normal));
			result.Add(18, new XlFunctionInfo("ATAN", 1, 1, XlFunctionProperty.Normal));
			result.Add(97, new XlFunctionInfo("ATAN2", 2, 2, XlFunctionProperty.Normal));
			result.Add(234, new XlFunctionInfo("ATANH", 1, 1, XlFunctionProperty.Normal));
			result.Add(16408, new XlFunctionInfo("BASE", 2, 3, XlFunctionProperty.Excel2010Future));
			result.Add(288, new XlFunctionInfo("CEILING", 2, 2, XlFunctionProperty.Normal));
			result.Add(16409, new XlFunctionInfo("CEILING.MATH", 2, 3, XlFunctionProperty.Excel2010Future));
			result.Add(16497, new XlFunctionInfo("CEILING.PRECISE", 1, 2, XlFunctionProperty.Excel2010Future));
			result.Add(276, new XlFunctionInfo("COMBIN", 2, 2, XlFunctionProperty.Normal));
			result.Add(16410, new XlFunctionInfo("COMBINA", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(16, new XlFunctionInfo("COS", 1, 1, XlFunctionProperty.Normal));
			result.Add(230, new XlFunctionInfo("COSH", 1, 1, XlFunctionProperty.Normal));
			result.Add(16411, new XlFunctionInfo("COT", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(16412, new XlFunctionInfo("COTH", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(16413, new XlFunctionInfo("CSC", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(16414, new XlFunctionInfo("CSCH", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(16415, new XlFunctionInfo("DECIMAL", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(343, new XlFunctionInfo("DEGREES", 1, 1, XlFunctionProperty.Normal));
			result.Add(279, new XlFunctionInfo("EVEN", 1, 1, XlFunctionProperty.Normal));
			result.Add(21, new XlFunctionInfo("EXP", 1, 1, XlFunctionProperty.Normal));
			result.Add(184, new XlFunctionInfo("FACT", 1, 1, XlFunctionProperty.Normal));
			result.Add(415, new XlFunctionInfo("FACTDOUBLE", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(285, new XlFunctionInfo("FLOOR", 2, 2, XlFunctionProperty.Normal));
			result.Add(16416, new XlFunctionInfo("FLOOR.MATH", 2, 3, XlFunctionProperty.Excel2010Future));
			result.Add(16499, new XlFunctionInfo("FLOOR.PRECISE", 1, 2, XlFunctionProperty.Excel2010Future));
			result.Add(473, new XlFunctionInfo("GCD", 1, 255, XlFunctionProperty.Excel2010Predefined));
			result.Add(25, new XlFunctionInfo("INT", 1, 1, XlFunctionProperty.Normal));
			result.Add(16417, new XlFunctionInfo("ISO.CEILING", 1, 2, XlFunctionProperty.Excel2010Future));
			result.Add(475, new XlFunctionInfo("LCM", 1, 255, XlFunctionProperty.Excel2010Predefined));
			result.Add(22, new XlFunctionInfo("LN", 1, 1, XlFunctionProperty.Normal));
			result.Add(109, new XlFunctionInfo("LOG", 1, 2, XlFunctionProperty.Normal));
			result.Add(23, new XlFunctionInfo("LOG10", 1, 1, XlFunctionProperty.Normal));
			result.Add(163, new XlFunctionInfo("MDETERM", 1, 1, XlFunctionProperty.Normal));
			result.Add(164, new XlFunctionInfo("MINVERSE", 1, 1, XlFunctionProperty.Normal));
			result.Add(165, new XlFunctionInfo("MMULT", 2, 2, XlFunctionProperty.Normal));
			result.Add(39, new XlFunctionInfo("MOD", 2, 2, XlFunctionProperty.Normal));
			result.Add(422, new XlFunctionInfo("MROUND", 2, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(16418, new XlFunctionInfo("MUNIT", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(474, new XlFunctionInfo("MULTINOMIAL", 1, 255, XlFunctionProperty.Excel2010Predefined));
			result.Add(298, new XlFunctionInfo("ODD", 1, 1, XlFunctionProperty.Normal));
			result.Add(19, new XlFunctionInfo("PI", 0, 0, XlFunctionProperty.Normal));
			result.Add(337, new XlFunctionInfo("POWER", 2, 2, XlFunctionProperty.Normal));
			result.Add(183, new XlFunctionInfo("PRODUCT", 1, 255, XlFunctionProperty.Normal));
			result.Add(417, new XlFunctionInfo("QUOTIENT", 2, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(342, new XlFunctionInfo("RADIANS", 1, 1, XlFunctionProperty.Normal));
			result.Add(63, new XlFunctionInfo("RAND", 0, 0, XlFunctionProperty.Normal));
			result.Add(464, new XlFunctionInfo("RANDBETWEEN", 2, 2, XlFunctionProperty.Excel2010Predefined));
			result.Add(354, new XlFunctionInfo("ROMAN", 1, 2, XlFunctionProperty.Normal));
			result.Add(27, new XlFunctionInfo("ROUND", 2, 2, XlFunctionProperty.Normal));
			result.Add(213, new XlFunctionInfo("ROUNDDOWN", 2, 2, XlFunctionProperty.Normal));
			result.Add(212, new XlFunctionInfo("ROUNDUP", 2, 2, XlFunctionProperty.Normal));
			result.Add(414, new XlFunctionInfo("SERIESSUM", 4, 4, XlFunctionProperty.Excel2010Predefined));
			result.Add(16419, new XlFunctionInfo("SEC", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(16420, new XlFunctionInfo("SECH", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(26, new XlFunctionInfo("SIGN", 1, 1, XlFunctionProperty.Normal));
			result.Add(15, new XlFunctionInfo("SIN", 1, 1, XlFunctionProperty.Normal));
			result.Add(229, new XlFunctionInfo("SINH", 1, 1, XlFunctionProperty.Normal));
			result.Add(20, new XlFunctionInfo("SQRT", 1, 1, XlFunctionProperty.Normal));
			result.Add(416, new XlFunctionInfo("SQRTPI", 1, 1, XlFunctionProperty.Excel2010Predefined));
			result.Add(344, new XlFunctionInfo("SUBTOTAL", 2, 255, XlFunctionProperty.Normal));
			result.Add(4, new XlFunctionInfo("SUM", 1, 255, XlFunctionProperty.Normal));
			result.Add(345, new XlFunctionInfo("SUMIF", 2, 3, XlFunctionProperty.Normal));
			result.Add(482, new XlFunctionInfo("SUMIFS", 256, 255, XlFunctionProperty.Excel2003Future));
			result.Add(228, new XlFunctionInfo("SUMPRODUCT", 1, 255, XlFunctionProperty.Normal));
			result.Add(321, new XlFunctionInfo("SUMSQ", 1, 255, XlFunctionProperty.Normal));
			result.Add(304, new XlFunctionInfo("SUMX2MY2", 2, 2, XlFunctionProperty.Normal));
			result.Add(305, new XlFunctionInfo("SUMX2PY2", 2, 2, XlFunctionProperty.Normal));
			result.Add(303, new XlFunctionInfo("SUMXMY2", 2, 2, XlFunctionProperty.Normal));
			result.Add(17, new XlFunctionInfo("TAN", 1, 1, XlFunctionProperty.Normal));
			result.Add(231, new XlFunctionInfo("TANH", 1, 1, XlFunctionProperty.Normal));
			result.Add(197, new XlFunctionInfo("TRUNC", 1, 2, XlFunctionProperty.Normal));
			result.Add(269, new XlFunctionInfo("AVEDEV", 1, 255, XlFunctionProperty.Normal));
			result.Add(5, new XlFunctionInfo("AVERAGE", 1, 255, XlFunctionProperty.Normal));
			result.Add(361, new XlFunctionInfo("AVERAGEA", 1, 255, XlFunctionProperty.Normal));
			result.Add(483, new XlFunctionInfo("AVERAGEIF", 2, 3, XlFunctionProperty.Excel2003Future));
			result.Add(484, new XlFunctionInfo("AVERAGEIFS", 256, 255, XlFunctionProperty.Excel2003Future));
			result.Add(16404, new XlFunctionInfo("BETA.DIST", 5, 6, XlFunctionProperty.Excel2010Future));
			result.Add(16465, new XlFunctionInfo("BETA.INV", 4, 5, XlFunctionProperty.Excel2010Future));
			result.Add(16495, new XlFunctionInfo("BINOM.DIST", 4, 4, XlFunctionProperty.Excel2010Future));
			result.Add(16421, new XlFunctionInfo("BINOM.DIST.RANGE", 3, 4, XlFunctionProperty.Excel2010Future));
			result.Add(16496, new XlFunctionInfo("BINOM.INV", 3, 3, XlFunctionProperty.Excel2010Future));
			result.Add(16485, new XlFunctionInfo("CHISQ.DIST", 3, 3, XlFunctionProperty.Excel2010Future));
			result.Add(16486, new XlFunctionInfo("CHISQ.DIST.RT", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(16487, new XlFunctionInfo("CHISQ.INV", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(16488, new XlFunctionInfo("CHISQ.INV.RT", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(16489, new XlFunctionInfo("CHISQ.TEST", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(16400, new XlFunctionInfo("CONFIDENCE.NORM", 3, 3, XlFunctionProperty.Excel2010Future));
			result.Add(16498, new XlFunctionInfo("CONFIDENCE.T", 3, 3, XlFunctionProperty.Excel2010Future));
			result.Add(307, new XlFunctionInfo("CORREL", 2, 2, XlFunctionProperty.Normal));
			result.Add(0, new XlFunctionInfo("COUNT", 1, 255, XlFunctionProperty.Normal));
			result.Add(169, new XlFunctionInfo("COUNTA", 1, 255, XlFunctionProperty.Normal));
			result.Add(347, new XlFunctionInfo("COUNTBLANK", 1, 1, XlFunctionProperty.Normal));
			result.Add(346, new XlFunctionInfo("COUNTIF", 2, 2, XlFunctionProperty.Normal));
			result.Add(481, new XlFunctionInfo("COUNTIFS", 255, 256, XlFunctionProperty.Excel2003Future));
			result.Add(16461, new XlFunctionInfo("COVARIANCE.P", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(16462, new XlFunctionInfo("COVARIANCE.S", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(318, new XlFunctionInfo("DEVSQ", 1, 255, XlFunctionProperty.Normal));
			result.Add(16500, new XlFunctionInfo("EXPON.DIST", 3, 3, XlFunctionProperty.Excel2010Future));
			result.Add(16468, new XlFunctionInfo("F.DIST", 4, 4, XlFunctionProperty.Excel2010Future));
			result.Add(16471, new XlFunctionInfo("F.DIST.RT", 3, 3, XlFunctionProperty.Excel2010Future));
			result.Add(16467, new XlFunctionInfo("F.INV", 3, 3, XlFunctionProperty.Excel2010Future));
			result.Add(16483, new XlFunctionInfo("F.INV.RT", 3, 3, XlFunctionProperty.Excel2010Future));
			result.Add(16470, new XlFunctionInfo("F.TEST", 2, 2, XlFunctionProperty.Normal));
			result.Add(283, new XlFunctionInfo("FISHER", 1, 1, XlFunctionProperty.Normal));
			result.Add(284, new XlFunctionInfo("FISHERINV", 1, 1, XlFunctionProperty.Normal));
			result.Add(309, new XlFunctionInfo("FORECAST", 3, 3, XlFunctionProperty.Normal));
			result.Add(252, new XlFunctionInfo("FREQUENCY", 2, 2, XlFunctionProperty.Normal));
			result.Add(16472, new XlFunctionInfo("GAMMA.DIST", 4, 4, XlFunctionProperty.Excel2010Future));
			result.Add(16484, new XlFunctionInfo("GAMMA.INV", 3, 3, XlFunctionProperty.Excel2010Future));
			result.Add(16422, new XlFunctionInfo("GAMMA", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(271, new XlFunctionInfo("GAMMALN", 1, 1, XlFunctionProperty.Normal));
			result.Add(16403, new XlFunctionInfo("GAMMALN.PRECISE", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(16502, new XlFunctionInfo("GAUSS", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(319, new XlFunctionInfo("GEOMEAN", 1, 255, XlFunctionProperty.Normal));
			result.Add(52, new XlFunctionInfo("GROWTH", 3, 4, XlFunctionProperty.Normal));
			result.Add(320, new XlFunctionInfo("HARMEAN", 1, 255, XlFunctionProperty.Normal));
			result.Add(16493, new XlFunctionInfo("HYPGEOM.DIST", 5, 5, XlFunctionProperty.Excel2010Future));
			result.Add(311, new XlFunctionInfo("INTERCEPT", 2, 2, XlFunctionProperty.Normal));
			result.Add(322, new XlFunctionInfo("KURT", 1, 255, XlFunctionProperty.Normal));
			result.Add(325, new XlFunctionInfo("LARGE", 2, 2, XlFunctionProperty.Normal));
			result.Add(49, new XlFunctionInfo("LINEST", 3, 4, XlFunctionProperty.Normal));
			result.Add(51, new XlFunctionInfo("LOGEST", 3, 4, XlFunctionProperty.Normal));
			result.Add(16393, new XlFunctionInfo("LOGNORM.DIST", 4, 4, XlFunctionProperty.Excel2010Future));
			result.Add(16395, new XlFunctionInfo("LOGNORM.INV", 3, 3, XlFunctionProperty.Excel2010Future));
			result.Add(7, new XlFunctionInfo("MAX", 1, 255, XlFunctionProperty.Normal));
			result.Add(362, new XlFunctionInfo("MAXA", 1, 255, XlFunctionProperty.Normal));
			result.Add(227, new XlFunctionInfo("MEDIAN", 1, 255, XlFunctionProperty.Normal));
			result.Add(6, new XlFunctionInfo("MIN", 1, 255, XlFunctionProperty.Normal));
			result.Add(363, new XlFunctionInfo("MINA", 1, 255, XlFunctionProperty.Normal));
			result.Add(16385, new XlFunctionInfo("MODE.MULT", 1, 255, XlFunctionProperty.Excel2010Future));
			result.Add(16386, new XlFunctionInfo("MODE.SNGL", 1, 255, XlFunctionProperty.Excel2010Future));
			result.Add(16492, new XlFunctionInfo("NEGBINOM.DIST", 4, 4, XlFunctionProperty.Excel2010Future));
			result.Add(16397, new XlFunctionInfo("NORM.DIST", 4, 4, XlFunctionProperty.Excel2010Future));
			result.Add(16396, new XlFunctionInfo("NORM.INV", 3, 3, XlFunctionProperty.Excel2010Future));
			result.Add(16392, new XlFunctionInfo("NORM.S.DIST", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(16394, new XlFunctionInfo("NORM.S.INV", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(312, new XlFunctionInfo("PEARSON", 2, 2, XlFunctionProperty.Normal));
			result.Add(16423, new XlFunctionInfo("PERMUTATIONA", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(16424, new XlFunctionInfo("PHI", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(16457, new XlFunctionInfo("PERCENTILE.EXC", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(16458, new XlFunctionInfo("PERCENTILE.INC", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(16455, new XlFunctionInfo("PERCENTRANK.EXC", 2, 3, XlFunctionProperty.Excel2010Future));
			result.Add(16456, new XlFunctionInfo("PERCENTRANK.INC", 2, 3, XlFunctionProperty.Excel2010Future));
			result.Add(299, new XlFunctionInfo("PERMUT", 2, 2, XlFunctionProperty.Normal));
			result.Add(16501, new XlFunctionInfo("POISSON.DIST", 3, 3, XlFunctionProperty.Excel2010Future));
			result.Add(317, new XlFunctionInfo("PROB", 3, 4, XlFunctionProperty.Normal));
			result.Add(16459, new XlFunctionInfo("QUARTILE.EXC", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(16460, new XlFunctionInfo("QUARTILE.INC", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(16463, new XlFunctionInfo("RANK.AVG", 2, 3, XlFunctionProperty.Excel2010Future));
			result.Add(16464, new XlFunctionInfo("RANK.EQ", 2, 3, XlFunctionProperty.Excel2010Future));
			result.Add(313, new XlFunctionInfo("RSQ", 2, 2, XlFunctionProperty.Normal));
			result.Add(323, new XlFunctionInfo("SKEW", 1, 255, XlFunctionProperty.Normal));
			result.Add(16425, new XlFunctionInfo("SKEW.P", 1, 255, XlFunctionProperty.Excel2010Future));
			result.Add(315, new XlFunctionInfo("SLOPE", 2, 2, XlFunctionProperty.Normal));
			result.Add(326, new XlFunctionInfo("SMALL", 2, 2, XlFunctionProperty.Normal));
			result.Add(297, new XlFunctionInfo("STANDARDIZE", 3, 3, XlFunctionProperty.Normal));
			result.Add(16390, new XlFunctionInfo("STDEV.P", 1, 255, XlFunctionProperty.Excel2010Future));
			result.Add(16389, new XlFunctionInfo("STDEV.S", 1, 255, XlFunctionProperty.Excel2010Future));
			result.Add(366, new XlFunctionInfo("STDEVA", 1, 255, XlFunctionProperty.Normal));
			result.Add(364, new XlFunctionInfo("STDEVPA", 1, 255, XlFunctionProperty.Normal));
			result.Add(314, new XlFunctionInfo("STEYX", 2, 2, XlFunctionProperty.Normal));
			result.Add(16469, new XlFunctionInfo("T.DIST", 3, 3, XlFunctionProperty.Excel2010Future));
			result.Add(16481, new XlFunctionInfo("T.DIST.2T", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(16480, new XlFunctionInfo("T.DIST.RT", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(16466, new XlFunctionInfo("T.INV", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(16482, new XlFunctionInfo("T.INV.2T", 2, 2, XlFunctionProperty.Excel2010Future));
			result.Add(50, new XlFunctionInfo("TREND", 3, 4, XlFunctionProperty.Normal));
			result.Add(331, new XlFunctionInfo("TRIMMEAN", 2, 2, XlFunctionProperty.Normal));
			result.Add(16473, new XlFunctionInfo("T.TEST", 4, 4, XlFunctionProperty.Excel2010Future));
			result.Add(16387, new XlFunctionInfo("VAR.P", 1, 255, XlFunctionProperty.Excel2010Future));
			result.Add(16388, new XlFunctionInfo("VAR.S", 1, 255, XlFunctionProperty.Excel2010Future));
			result.Add(367, new XlFunctionInfo("VARA", 1, 255, XlFunctionProperty.Normal));
			result.Add(365, new XlFunctionInfo("VARPA", 1, 255, XlFunctionProperty.Normal));
			result.Add(16491, new XlFunctionInfo("WEIBULL.DIST", 4, 4, XlFunctionProperty.Excel2010Future));
			result.Add(16490, new XlFunctionInfo("Z.TEST", 2, 3, XlFunctionProperty.Excel2010Future));
			result.Add(214, new XlFunctionInfo("ASC", 1, 1, XlFunctionProperty.Normal));
			result.Add(368, new XlFunctionInfo("BAHTTEXT", 1, 1, XlFunctionProperty.Normal));
			result.Add(111, new XlFunctionInfo("CHAR", 1, 1, XlFunctionProperty.Normal));
			result.Add(162, new XlFunctionInfo("CLEAN", 1, 1, XlFunctionProperty.Normal));
			result.Add(121, new XlFunctionInfo("CODE", 1, 1, XlFunctionProperty.Normal));
			result.Add(336, new XlFunctionInfo("CONCATENATE", 1, 255, XlFunctionProperty.Normal));
			result.Add(13, new XlFunctionInfo("DOLLAR", 1, 2, XlFunctionProperty.Normal));
			result.Add(117, new XlFunctionInfo("EXACT", 2, 2, XlFunctionProperty.Normal));
			result.Add(124, new XlFunctionInfo("FIND", 2, 3, XlFunctionProperty.Normal));
			result.Add(205, new XlFunctionInfo("FINDB", 2, 3, XlFunctionProperty.Normal));
			result.Add(14, new XlFunctionInfo("FIXED", 2, 3, XlFunctionProperty.Normal));
			result.Add(115, new XlFunctionInfo("LEFT", 1, 2, XlFunctionProperty.Normal));
			result.Add(208, new XlFunctionInfo("LEFTB", 1, 2, XlFunctionProperty.Normal));
			result.Add(32, new XlFunctionInfo("LEN", 1, 1, XlFunctionProperty.Normal));
			result.Add(211, new XlFunctionInfo("LENB", 1, 1, XlFunctionProperty.Normal));
			result.Add(112, new XlFunctionInfo("LOWER", 1, 1, XlFunctionProperty.Normal));
			result.Add(31, new XlFunctionInfo("MID", 3, 3, XlFunctionProperty.Normal));
			result.Add(210, new XlFunctionInfo("MIDB", 3, 3, XlFunctionProperty.Normal));
			result.Add(16439, new XlFunctionInfo("NUMBERVALUE", 2, 3, XlFunctionProperty.Excel2010Future));
			result.Add(360, new XlFunctionInfo("PHONETIC", 1, 1, XlFunctionProperty.Normal));
			result.Add(114, new XlFunctionInfo("PROPER", 1, 1, XlFunctionProperty.Normal));
			result.Add(119, new XlFunctionInfo("REPLACE", 4, 4, XlFunctionProperty.Normal));
			result.Add(207, new XlFunctionInfo("REPLACEB", 4, 4, XlFunctionProperty.Normal));
			result.Add(30, new XlFunctionInfo("REPT", 2, 2, XlFunctionProperty.Normal));
			result.Add(116, new XlFunctionInfo("RIGHT", 1, 2, XlFunctionProperty.Normal));
			result.Add(209, new XlFunctionInfo("RIGHTB", 1, 2, XlFunctionProperty.Normal));
			result.Add(82, new XlFunctionInfo("SEARCH", 2, 3, XlFunctionProperty.Normal));
			result.Add(206, new XlFunctionInfo("SEARCHB", 2, 3, XlFunctionProperty.Normal));
			result.Add(120, new XlFunctionInfo("SUBSTITUTE", 3, 4, XlFunctionProperty.Normal));
			result.Add(130, new XlFunctionInfo("T", 1, 1, XlFunctionProperty.Normal));
			result.Add(48, new XlFunctionInfo("TEXT", 2, 2, XlFunctionProperty.Normal));
			result.Add(118, new XlFunctionInfo("TRIM", 1, 1, XlFunctionProperty.Normal));
			result.Add(16441, new XlFunctionInfo("UNICODE", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(113, new XlFunctionInfo("UPPER", 1, 1, XlFunctionProperty.Normal));
			result.Add(33, new XlFunctionInfo("VALUE", 1, 1, XlFunctionProperty.Normal));
			result.Add(150, new XlFunctionInfo("CALL", 2, 255, XlFunctionProperty.Normal));
			result.Add(267, new XlFunctionInfo("REGISTER.ID", 2, 3, XlFunctionProperty.Normal));
			result.Add(16452, new XlFunctionInfo("ENCODEURL", 1, 1, XlFunctionProperty.Excel2010Future));
			result.Add(16640, new XlFunctionInfo("FIELD", 0, 1, XlFunctionProperty.Normal));
			result.Add(16641, new XlFunctionInfo("RANGE", 1, 1, XlFunctionProperty.Normal));
			result.Add(16642, new XlFunctionInfo("FIELDPICTURE", 5, 6, XlFunctionProperty.Normal));
			return result;
		}
		readonly Stack<int> stack;
		readonly StringBuilder builder;
		IXlExpressionContext context;
		public XlExpressionStringBuilder() {
			this.stack = new Stack<int>();
			this.builder = new StringBuilder();
			this.context = null;
		}
		public IXlExpressionContext Context {
			get { return context; }
			set { context = value; }
		}
		public string BuildString(XlExpression expression) {
			foreach (XlPtgBase ptg in expression) {
				ptg.Visit(this);
			}
			return builder.ToString();
		}
		public void Visit(XlPtgBinaryOperator ptg) {
			builder.Insert(stack.Pop(), ptg.OperatorText);
		}
		public void Visit(XlPtgUnaryOperator ptg) {
			if (ptg.TypeCode != XlPtgTypeCode.Percent)
				builder.Insert(stack.Peek(), ptg.OperatorText);
			else
				builder.Append(ptg.OperatorText);
		}
		public void Visit(XlPtgParen ptg) {
			int position = stack.Peek();
			builder.Insert(position, "(");
			builder.Append(")");
		}
		public void Visit(XlPtgMissArg ptg) {
			stack.Push(builder.Length);
		}
		public void Visit(XlPtgStr ptg) {
			stack.Push(builder.Length);
			builder.Append("\"");
			builder.Append(ptg.Value.Replace("\"", "\"\""));
			builder.Append("\"");
		}
		public void Visit(XlPtgErr ptg) {
			stack.Push(builder.Length);
			builder.Append(errCodeToTextConversionTable[ptg.Value]);
		}
		public void Visit(XlPtgBool ptg) {
			stack.Push(builder.Length);
			builder.Append(ptg.Value ? trueConstant : falseConstant);
		}
		public void Visit(XlPtgInt ptg) {
			stack.Push(builder.Length);
			builder.Append(ptg.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}
		string ConvertNumberToText(double value, System.Globalization.CultureInfo culture) {
			string result = value.ToString(culture);
			if (value > 1e+15 && value < 1e+16) {
				try {
					double parsedValue = double.Parse(result, culture);
					long intValue = (long)parsedValue;
					if (parsedValue == intValue) {
						string integerText = intValue.ToString(culture);
						if (integerText.Length < result.Length)
							return integerText;
					}
				}
				catch {
				}
			}
			return result;
		}
		public void Visit(XlPtgNum ptg) {
			stack.Push(builder.Length);
			builder.Append(ConvertNumberToText(ptg.Value, System.Globalization.CultureInfo.InvariantCulture));
		}
#region Function
		public void Visit(XlPtgFunc ptg) {
			XlFunctionInfo function = functionCodeToFunctionInfoConversionTable[ptg.FuncCode];
			string functionName = PrepareFunctionName(function);
			BuildFunctionString(function.MinArgumentsCount, functionName);
		}
		public void Visit(XlPtgFuncVar ptg) {
			XlFunctionInfo function = functionCodeToFunctionInfoConversionTable[ptg.FuncCode];
			string functionName = PrepareFunctionName(function);
			BuildFunctionString(ptg.ParamCount, functionName);
		}
		string PrepareFunctionName(XlFunctionInfo function) {
			if (function.Properties == XlFunctionProperty.Excel2010Future)
				return FUTURE_FUNCTION_PREFIX + function.Name;
			return function.Name;
		}
		void BuildFunctionString(int parametersCount, string functionName) {
			if (parametersCount > 0) {
				string separator = ",";
				for (int i = 0; i < parametersCount - 1; i++)
					builder.Insert(stack.Pop(), separator);
				int startPos = stack.Peek();
				builder.Insert(startPos, functionName + "(");
				builder.Append(")");
			}
			else {
				stack.Push(builder.Length);
				builder.Append(functionName);
				builder.Append("()");
			}
		}
#endregion
		public void Visit(XlPtgRef ptg) {
			stack.Push(builder.Length);
			builder.Append(ptg.CellPosition.ToString());
		}
		public void Visit(XlPtgRef3d ptg) {
			stack.Push(builder.Length);
			XlCellRange range = new XlCellRange(ptg.CellPosition);
			range.SheetName = ptg.SheetName;
			builder.Append(range.ToString());
		}
		public void Visit(XlPtgRefN3d ptg) {
			stack.Push(builder.Length);
			XlCellPosition position = ptg.CellOffset.ToCellPosition(Context);
			XlCellRange range = new XlCellRange(position);
			range.SheetName = ptg.SheetName;
			builder.Append(range.ToString());
		}
		public void Visit(XlPtgArea ptg) {
			stack.Push(builder.Length);
			builder.Append(ptg.CellRange.ToString());
		}
		public void Visit(XlPtgArea3d ptg) {
			stack.Push(builder.Length);
			XlCellRange range = ptg.CellRange;
			range.SheetName = ptg.SheetName;
			builder.Append(range.ToString());
		}
		public void Visit(XlPtgAreaN3d ptg) {
			stack.Push(builder.Length);
			XlCellPosition topLeft = ptg.TopLeft.ToCellPosition(Context);
			XlCellPosition bottomRight = ptg.BottomRight.ToCellPosition(Context);
			XlCellRange range = new XlCellRange(topLeft, bottomRight);
			range.SheetName = ptg.SheetName;
			builder.Append(range.ToString());
		}
		public void Visit(XlPtgAttrSemi ptg) {
		}
		public void Visit(XlPtgAttrSpace ptg) {
		}
		public void Visit(XlPtgAttrIf ptg) {
		}
		public void Visit(XlPtgAttrGoto ptg) {
		}
		public void Visit(XlPtgAttrChoose ptg) {
		}
		public void Visit(XlPtgRefN ptg) {
			stack.Push(builder.Length);
			XlCellPosition position = ptg.CellOffset.ToCellPosition(Context);
			builder.Append(position.ToString());
		}
		public void Visit(XlPtgAreaN ptg) {
			stack.Push(builder.Length);
			XlCellPosition topLeft = ptg.TopLeft.ToCellPosition(Context);
			XlCellPosition bottomRight = ptg.BottomRight.ToCellPosition(Context);
			XlCellRange range = new XlCellRange(topLeft, bottomRight);
			builder.Append(range.ToString());
		}
		public void Visit(XlPtgRefErr ptg) {
		}
		public void Visit(XlPtgAreaErr ptg) {
		}
		public void Visit(XlPtgRefErr3d ptg) {
		}
		public void Visit(XlPtgAreaErr3d ptg) {
		}
		public void Visit(XlPtgArray ptg) {
			stack.Push(builder.Length);
			ConvertArrayToString(ptg, builder);
		}
		public void Visit(XlPtgName ptg) {
		}
		public void Visit(XlPtgExp ptg) {
		}
		public void Visit(XlPtgMemFunc ptg) {
		}
		public void ConvertArrayToString(XlPtgArray ptg, StringBuilder builder) {
			char decimalSeparator = '.';
			char elementDelimiter = (decimalSeparator == ',') ? '\\' : ',';
			char rowsDelimiter = (decimalSeparator == ';') ? '\\' : ';';
			builder.Append('{');
			int index = 0;
			int width = ptg.Width;
			int height = ptg.Height;
			for (int h = 0; h < height; h++) {
				for (int w = 0; w < width; w++) {
					XlVariantValue value = ptg.Values[index];
					string element = value.ToText().TextValue;
					if (value.IsText)
						element = "\"" + element.Replace("\"", "\"\"") + "\"";
					builder.Append(element);
					if (w != width - 1)
						builder.Append(elementDelimiter);
					index++;
				}
				if (h != height - 1)
					builder.Append(rowsDelimiter);
			}
			builder.Append('}');
		}
	}
#endregion
#region XlFunctionProperty
	enum XlFunctionProperty {
		Normal = 0,
		Excel2010Predefined = 1,
		Excel2003Future = 2,
		Excel2010Future = 3,
	}
#endregion
#region XlFunctionInfo
	class XlFunctionInfo {
		readonly string name;
		readonly int minArgumentsCount;
		readonly int maxArgumentsCount;
		readonly XlFunctionProperty properties;
		public XlFunctionInfo(string name, int minArgumentsCount, int maxArgumentsCount, XlFunctionProperty properties) {
			this.maxArgumentsCount = maxArgumentsCount;
			this.minArgumentsCount = minArgumentsCount;
			this.name = name;
			this.properties = properties;
		}
		public bool HasFixedArgumentsCount { get { return minArgumentsCount == maxArgumentsCount; } }
		public string Name { get { return name; } }
		public int MinArgumentsCount { get { return minArgumentsCount; } }
		public int MaxArgumentsCount { get { return maxArgumentsCount; } }
		public XlFunctionProperty Properties { get { return properties; } }
		public bool IsExcel2010PredefinedFunction() {
			return properties == XlFunctionProperty.Excel2010Predefined || properties == XlFunctionProperty.Excel2003Future;
		}
		public bool IsExcel2010PredefinedNonFutureFunction() {
			return properties == XlFunctionProperty.Excel2010Predefined;
		}
		public bool IsExcel2010FutureFunction() {
			return properties == XlFunctionProperty.Excel2010Future;
		}
		public bool IsExcel2003FutureFunction() {
			return properties == XlFunctionProperty.Excel2003Future || IsExcel2010FutureFunction();
		}
	}
#endregion
}
