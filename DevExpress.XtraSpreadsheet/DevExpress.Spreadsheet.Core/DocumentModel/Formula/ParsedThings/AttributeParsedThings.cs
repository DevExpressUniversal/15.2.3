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

using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FormulaProperties
	public enum FormulaProperties {
		None = 0,
		HasVolatileFunction = 0x1,
		HasCustomFunction = 0x2,
		HasInternalFunction = 0x4,
		HasFunctionRTD = 0x8,
	}
	#endregion
	#region Attribute tokens
	public abstract class ParsedThingAttrBase : ParsedThingBase {
		public override bool IsEqual(IParsedThing obj) {
			return GetType() == obj.GetType();
		}
	}
	public class ParsedThingAttrSemi : ParsedThingAttrBase {
		FormulaProperties formulaProperties;
		public ParsedThingAttrSemi() { }
		public ParsedThingAttrSemi(FormulaProperties formulaProperties) {
			this.formulaProperties = formulaProperties;
		}
		public FormulaProperties FormulaProperties { get { return formulaProperties; } }
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Clone() {
			return this;
		}
		public override bool IsEqual(IParsedThing obj) {
			ParsedThingAttrSemi anotherPtg = obj as ParsedThingAttrSemi;
			if (anotherPtg == null)
				return false;
			return this.formulaProperties == anotherPtg.formulaProperties;
		}
		public void AddProperty(FormulaProperties property) {
			formulaProperties |= property;
		}
		public bool HasProperty(FormulaProperties property) {
			if ((formulaProperties & property) == property)
				return true;
			return false;
		}
	}
	public class ParsedThingAttrIf : ParsedThingAttrBase {
		int offset;
		public int Offset {
			get { return offset; }
			set {
				ValueChecker.CheckValue((int)value, 0, ushort.MaxValue, "Offset");
				this.offset = value;
			}
		}
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Clone() {
			ParsedThingAttrIf clone = new ParsedThingAttrIf();
			clone.offset = offset;
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			ParsedThingAttrIf anotherPtg = obj as ParsedThingAttrIf;
			if (anotherPtg == null)
				return false;
			return this.offset == anotherPtg.offset;
		}
	}
	public class ParsedThingAttrChoose : ParsedThingAttrBase {
		List<int> offsets = new List<int>();
		public IList<int> Offsets { get { return offsets; } }
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Clone() {
			ParsedThingAttrChoose clone = new ParsedThingAttrChoose();
			clone.offsets.AddRange(offsets);
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			ParsedThingAttrChoose anotherPtg = obj as ParsedThingAttrChoose;
			if (anotherPtg == null)
				return false;
			int count = offsets.Count;
			if (count != anotherPtg.offsets.Count)
				return false;
			for (int i = 0; i < count; i++) {
				if (offsets[i] != anotherPtg.offsets[i])
					return false;
			}
			return true;
		}
	}
	public class ParsedThingAttrGoto : ParsedThingAttrBase {
		int offset;
		public ParsedThingAttrGoto() {
		}
		public ParsedThingAttrGoto(int offset) {
			this.offset = offset;
		}
		public int Offset {
			get { return offset; }
			set {
				ValueChecker.CheckValue((int)value, 1, ushort.MaxValue, "Offset");
				this.offset = value;
			}
		}
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Clone() {
			ParsedThingAttrGoto clone = new ParsedThingAttrGoto();
			clone.offset = offset;
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			ParsedThingAttrGoto anotherPtg = obj as ParsedThingAttrGoto;
			if (anotherPtg == null)
				return false;
			return this.offset == anotherPtg.offset;
		}
	}
	public class ParsedThingAttrSum : ParsedThingAttrBase {
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Clone() {
			return this;
		}
	}
	public class ParsedThingAttrBaxcel : ParsedThingAttrBase {
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Clone() {
			return this;
		}
	}
	public class ParsedThingAttrBaxcelVolatile : ParsedThingAttrBase {
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Clone() {
			return this;
		}
	}
	public enum ParsedThingAttrSpaceType {
		SpaceBeforeBaseExpression = 0,
		CarriageReturnBeforeBaseExpression = 1,
		SpaceBeforeOpenParentheses = 2,
		CarriageReturnBeforeOpenParentheses = 3,
		SpaceBeforeCloseParentheses = 4,
		CarriageReturnBeforeCloseParentheses = 5,
		SpaceBeforeExpression = 6
	}
	public abstract class ParsedThingAttrSpaceBase : ParsedThingAttrBase {
		#region Fields
		int charCount;
		#endregion
		protected ParsedThingAttrSpaceBase() {
		}
		protected ParsedThingAttrSpaceBase(ParsedThingAttrSpaceType spaceType, int charCount) {
			this.SpaceType = spaceType;
			this.charCount = charCount;
		}
		#region Properties
		public ParsedThingAttrSpaceType SpaceType { get; set; }
		public int CharCount {
			get { return charCount; }
			set {
				ValueChecker.CheckValue(value, 0, byte.MaxValue, "CharCount");
				this.charCount = value;
			}
		}
		#endregion
		public override void BuildExpressionString(Stack<int> stack, StringBuilder builder, StringBuilder spacesBuilder, WorkbookDataContext context) {
			string spacesString;
			if (SpaceType == ParsedThingAttrSpaceType.CarriageReturnBeforeBaseExpression ||
				SpaceType == ParsedThingAttrSpaceType.CarriageReturnBeforeCloseParentheses ||
				SpaceType == ParsedThingAttrSpaceType.CarriageReturnBeforeOpenParentheses) {
				StringBuilder spacesStringBuilder = new StringBuilder();
				for (int i = 0; i < charCount; i++)
					spacesStringBuilder.Append("\r\n");
				spacesString = spacesStringBuilder.ToString();
			}
			else
				spacesString = new String(' ', charCount);
			if (SpaceType == ParsedThingAttrSpaceType.SpaceBeforeCloseParentheses || SpaceType == ParsedThingAttrSpaceType.CarriageReturnBeforeCloseParentheses)
				builder.Append(spacesString);
			else
				spacesBuilder.Append(spacesString);
		}
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingAttrSpaceBase anotherPtg = obj as ParsedThingAttrSpaceBase;
			if (anotherPtg == null)
				return false;
			return this.charCount == anotherPtg.charCount;
		}
	}
	public class ParsedThingAttrSpace : ParsedThingAttrSpaceBase {
		public ParsedThingAttrSpace()
			: base() {
		}
		public ParsedThingAttrSpace(ParsedThingAttrSpaceType spaceType, int charCount)
			: base(spaceType, charCount) {
		}
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Clone() {
			ParsedThingAttrSpace clone = new ParsedThingAttrSpace();
			clone.CharCount = CharCount;
			return clone;
		}
	}
	public class ParsedThingAttrSpaceSemi : ParsedThingAttrSpaceBase {
		public ParsedThingAttrSpaceSemi()
			: base() {
		}
		public ParsedThingAttrSpaceSemi(ParsedThingAttrSpaceType spaceType, int charCount)
			: base(spaceType, charCount) {
		}
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Clone() {
			ParsedThingAttrSpaceSemi clone = new ParsedThingAttrSpaceSemi();
			clone.CharCount = CharCount;
			return clone;
		}
	}
	#endregion
}
