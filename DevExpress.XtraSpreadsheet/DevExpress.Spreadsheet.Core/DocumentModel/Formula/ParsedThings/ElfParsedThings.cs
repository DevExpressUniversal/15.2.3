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
using System.Text;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraExport.Xls;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region Elf translation
	public interface IParsedThingElf {
		IParsedThing Translate(WorkbookDataContext context);
	}
	#endregion
	#region Elf base classes
	public abstract class ParsedThingElfBase : ParsedThingBase, IParsedThingElf {
		readonly List<CellPosition> positions = new List<CellPosition>();
		public bool IsRelative { get; set; }
		public IList<CellPosition> Positions { get { return positions; } }
		#region IParsedThingTranslate Members
		public virtual IParsedThing Translate(WorkbookDataContext context) {
			return this;
		}
		#endregion
	}
	public abstract class ParsedThingPositionElfBase : ParsedThingPositionBase, IParsedThingElf {
		public bool Quoted { get; set; }
		#region IParsedThingTranslate Members
		public virtual IParsedThing Translate(WorkbookDataContext context) {
			return this;
		}
		#endregion
	}
	public abstract class ParsedThingElfLelBase : ParsedThingBase, IParsedThingElf {
		int index;
		#region Properties
		public int Index {
			get { return index; }
			set {
				ValueChecker.CheckValue(value, 0, XlsDefs.MaxLelIndex, "Index");
				index = value;
			}
		}
		public bool Quoted { get; set; }
		#endregion
		#region IParsedThingTranslate Members
		public virtual IParsedThing Translate(WorkbookDataContext context) {
			return this;
		}
		#endregion
	}
	#endregion
	#region Elf tokens (Natural language formula)
	public class ParsedThingElfLel : ParsedThingElfLelBase {
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Translate(WorkbookDataContext context) {
			return new ParsedThingError() { Value = VariantValue.ErrorName };
		}
		public override IParsedThing Clone() {
			ParsedThingElfLel clone = new ParsedThingElfLel();
			clone.Index = Index;
			return clone;
		}
	}
	public class ParsedThingElfRw : ParsedThingPositionElfBase {
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Translate(WorkbookDataContext context) {
			CellPosition position = new CellPosition(context.CurrentColumnIndex, Position.Row, Position.ColumnType, Position.RowType);
			return new ParsedThingRef() { DataType = OperandDataType.Reference, Position = position };
		}
		public override IParsedThing Clone() {
			ParsedThingElfRw clone = new ParsedThingElfRw();
			clone.Quoted = Quoted;
			clone.Position= Position;
			return clone;
		}
	}
	public class ParsedThingElfCol : ParsedThingPositionElfBase {
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Translate(WorkbookDataContext context) {
			CellPosition position = new CellPosition(Position.Column, context.CurrentRowIndex, Position.ColumnType, Position.RowType);
			return new ParsedThingRef() { DataType = OperandDataType.Reference, Position = position };
		}
		public override IParsedThing Clone() {
			ParsedThingElfCol clone = new ParsedThingElfCol();
			clone.Quoted = Quoted;
			clone.Position = Position;
			return clone;
		}
	}
	public class ParsedThingElfRwV : ParsedThingPositionElfBase {
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Translate(WorkbookDataContext context) {
			CellPosition position = new CellPosition(context.CurrentColumnIndex, Position.Row, Position.ColumnType, Position.RowType);
			return new ParsedThingRef() { DataType = OperandDataType.Value, Position = position };
		}
		public override IParsedThing Clone() {
			ParsedThingElfRwV clone = new ParsedThingElfRwV();
			clone.Quoted = Quoted;
			clone.Position = Position;
			return clone;
		}
	}
	public class ParsedThingElfColV : ParsedThingPositionElfBase {
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Translate(WorkbookDataContext context) {
			CellPosition position = new CellPosition(Position.Column, context.CurrentRowIndex, Position.ColumnType, Position.RowType);
			return new ParsedThingRef() { DataType = OperandDataType.Value, Position = position };
		}
		public override IParsedThing Clone() {
			ParsedThingElfColV clone = new ParsedThingElfColV();
			clone.Quoted = Quoted;
			clone.Position = Position;
			return clone;
		}
	}
	public class ParsedThingElfRadical : ParsedThingPositionElfBase {
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Translate(WorkbookDataContext context) {
			return null;
		}
		public override IParsedThing Clone() {
			ParsedThingElfRadical clone = new ParsedThingElfRadical();
			clone.Quoted = Quoted;
			clone.Position = Position;
			return clone;
		}
	}
	public class ParsedThingElfRadicalS : ParsedThingElfBase {
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Clone() {
			ParsedThingElfRadicalS clone = new ParsedThingElfRadicalS();
			clone.IsRelative = IsRelative;
			foreach (CellPosition position in Positions)
				clone.Positions.Add(position);
			return clone;
		}
	}
	public class ParsedThingElfColS : ParsedThingElfBase {
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Clone() {
			ParsedThingElfColS clone = new ParsedThingElfColS();
			clone.IsRelative = IsRelative;
			foreach (CellPosition position in Positions)
				clone.Positions.Add(position);
			return clone;
		}
	}
	public class ParsedThingElfColSV : ParsedThingElfBase {
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Clone() {
			ParsedThingElfColSV clone = new ParsedThingElfColSV();
			clone.IsRelative = IsRelative;
			foreach (CellPosition position in Positions)
				clone.Positions.Add(position);
			return clone;
		}
	}
	public class ParsedThingElfRadicalLel : ParsedThingElfLelBase {
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Translate(WorkbookDataContext context) {
			return null;
		}
		public override IParsedThing Clone() {
			ParsedThingElfRadicalLel clone = new ParsedThingElfRadicalLel();
			clone.Index = Index;
			clone.Quoted = Quoted;
			return clone;
		}
	}
	public class ParsedThingSxName : ParsedThingBase {
		ItemPairCollection itemPairs = new ItemPairCollection();
		int index;
		#region Properties
		public int Index {
			get { return index; }
			set {
				index = value;
			}
		}
		public ItemPairCollection ItemPairs { get { return itemPairs; } }
		public bool IsValid { get { return index != -1; } }
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override void BuildExpressionString(Stack<int> stack, StringBuilder builder, StringBuilder spacesBuilder, WorkbookDataContext context) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			builder.Append(CellErrorFactory.GetErrorName(NameError.Instance, context));
		}
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			stack.Push(VariantValue.ErrorName);
		}
		public override IParsedThing Clone() {
			ParsedThingSxName clone = new ParsedThingSxName();
			clone.Index = Index;
			return clone;
		}
	}
	#endregion
	#region ItemPair
	public class ItemPairCollection : List<ItemPair> {
	}
	public class ItemPair {
		#region Fields
		int fieldIndex;
		int itemIndex;
		bool isCalculatedItem;
		bool isPosition;
		bool isRelative;
		#endregion
		public ItemPair(int fieldIndex, int itemIndex) {
			this.fieldIndex = fieldIndex;
			this.itemIndex = itemIndex;
		}
		#region Properties
		public int FieldIndex { get { return fieldIndex; } set { fieldIndex = value; } }
		public int ItemIndex { get { return itemIndex; } set { itemIndex = value; } }
		public bool IsCalculatedItem { get { return isCalculatedItem; } set { isCalculatedItem = value; } }
		public bool IsPosition { get { return isPosition; } set { isPosition = value; } }
		public bool IsRelative { get { return isRelative; } set { isRelative = value; } }
		#endregion
	}
	#endregion
}
