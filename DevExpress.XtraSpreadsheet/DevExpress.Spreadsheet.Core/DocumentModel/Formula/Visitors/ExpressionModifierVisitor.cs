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
using System.Diagnostics;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ExpressionModifierVisitor
	public abstract class ExpressionModifierVisitor : ReplaceThingsPRNVisitor {
		readonly WorkbookDataContext context;
		Stack<ThingPosition> stack;
		int currentThingPosition;
		int currentSpacesCount;
		ParsedExpression expression;
		List<MemThingPosition> memThingPositions;
		List<AttrThingPosition> attrOffsetThingPositions;
		protected ExpressionModifierVisitor(WorkbookDataContext context) {
			this.context = context;
		}
		public WorkbookDataContext Context { get { return context; } }
		protected Stack<ThingPosition> Stack { get { return stack; } }
		protected int CurrentSpacesCount { get { return currentSpacesCount; } set { currentSpacesCount = value; } }
		public override ParsedExpression Process(ParsedExpression expression) {
			this.stack = new Stack<ThingPosition>();
			if (expression == null)
				return null;
			this.expression = expression;
			memThingPositions = new List<MemThingPosition>();
			attrOffsetThingPositions = new List<AttrThingPosition>();
			currentSpacesCount = 0;
			currentThingPosition = 0;
			while (currentThingPosition < expression.Count) {
				IParsedThing thing = expression[currentThingPosition];
				thing.Visit(this);
				CheckOffsetThings();
				currentThingPosition++;
			}
			if (stack.Count != 1)
				DevExpress.Office.Utils.Exceptions.ThrowInternalException();
			return expression;
		}
		public override void VisitBinary(ParsedThingBase thing) {
			base.VisitBinary(thing);
			Debug.Assert(stack.Count >= 2);
			stack.Pop();
			ThingPosition left = stack.Pop();
			stack.Push(new ThingPosition(left.Start, currentThingPosition));
			currentSpacesCount = 0;
		}
		public override void VisitUnary(ParsedThingBase thing) {
			base.VisitUnary(thing);
			Debug.Assert(stack.Count >= 1);
			ThingPosition left = stack.Pop();
			stack.Push(new ThingPosition(left.Start, currentThingPosition));
			currentSpacesCount = 0;
		}
		public override void VisitOperand(ParsedThingBase thing) {
			base.VisitOperand(thing);
			stack.Push(new ThingPosition(currentThingPosition - currentSpacesCount, currentThingPosition));
			currentSpacesCount = 0;
		}
		#region Attributes
		bool ShouldTakeAttrSpaceIntoAccount(ParsedThingAttrSpaceBase thing) {
			return thing.SpaceType != ParsedThingAttrSpaceType.CarriageReturnBeforeCloseParentheses && thing.SpaceType != ParsedThingAttrSpaceType.SpaceBeforeCloseParentheses
							&& thing.SpaceType != ParsedThingAttrSpaceType.SpaceBeforeOpenParentheses && thing.SpaceType != ParsedThingAttrSpaceType.CarriageReturnBeforeOpenParentheses;
		}
		public override void Visit(ParsedThingAttrSpace thing) {
			base.Visit(thing);
			if (ShouldTakeAttrSpaceIntoAccount(thing))
				currentSpacesCount++;
		}
		public override void Visit(ParsedThingAttrSpaceSemi thing) {
			base.Visit(thing);
			currentSpacesCount++;
		}
		public override void Visit(ParsedThingAttrIf thing) {
			base.Visit(thing);
			ThingPosition position = new ThingPosition(currentThingPosition, currentThingPosition + thing.Offset);
			attrOffsetThingPositions.Add(new AttrIfThingPosition(thing, position));
		}
		public override void Visit(ParsedThingAttrChoose thing) {
			base.Visit(thing);
			throw new ArgumentException();
		}
		public override void Visit(ParsedThingAttrGoto thing) {
			base.Visit(thing);
			ThingPosition position = new ThingPosition(currentThingPosition, currentThingPosition + thing.Offset);
			attrOffsetThingPositions.Add(new AttrGotoThingPosition(thing, position));
		}
		#endregion
		public override void VisitMem(ParsedThingMemBase thing) {
			base.VisitMem(thing);
			ThingPosition position = new ThingPosition(currentThingPosition, currentThingPosition + thing.InnerThingCount);
			memThingPositions.Add(new MemThingPosition(thing, position));
		}
		void CheckOffsetThings() {
			while (memThingPositions.Count > 0) {
				MemThingPosition position = memThingPositions[memThingPositions.Count - 1];
				if (position.Position.End > currentThingPosition)
					break;
				memThingPositions.RemoveAt(memThingPositions.Count - 1);
				stack.Peek().Start = position.Position.Start;
			}
			for (int i = attrOffsetThingPositions.Count - 1; i >= 0; i--) {
				AttrThingPosition position = attrOffsetThingPositions[i];
				if (position.Position.End <= currentThingPosition) {
					attrOffsetThingPositions.RemoveAt(i);
				}
			}
		}
		public override void VisitFunction(ParsedThingFunc thing) {
			base.VisitFunction(thing);
			int paramCount = GetFuncParamCount(thing);
			int startPos = currentThingPosition;
			for (int i = 0; i < paramCount; i++)
				startPos = stack.Pop().Start;
			ThingPosition position = new ThingPosition(startPos, currentThingPosition);
			OnFunction(thing, position, paramCount);
		}
		int GetFuncParamCount(ParsedThingFunc thing) {
			ParsedThingFuncVar funcVar = thing as ParsedThingFuncVar;
			if (funcVar != null)
				return funcVar.ParamCount;
			return thing.Function.Parameters.Count;
		}
		public virtual void OnFunction(ParsedThingFunc thing, ThingPosition position, int paramCount) {
			int currentThing = position.End - 1;
			while (currentThing >= position.Start) {
				ParsedThingAttrSpaceBase attrSpace = expression[currentThing] as ParsedThingAttrSpaceBase;
				if (attrSpace != null) {
					if (ShouldTakeAttrSpaceIntoAccount(attrSpace))
						currentSpacesCount--;
				}
				else
					break;
				currentThing--;
			}
			stack.Push(position);
		}
		protected void ReplaceFunctionByCalculatedValue(ThingPosition position, int paramCount) {
			VariantValue value = CalculateSubExpression(position);
			ParsedThingList newExpression = BasicExpressionCreator.CreateExpressionForVariantValue(value, expression[position.End].DataType, Context);
			int movedAttributesCount = ReplaceRange(position, newExpression);
			ThingPosition thingPosition = new ThingPosition(position.Start, position.End + movedAttributesCount);
			if (paramCount == 0 && CurrentSpacesCount > 0) {
				thingPosition.Start--;
				CurrentSpacesCount--;
			}
			Stack.Push(thingPosition);
		}
		#region Replace range
		protected int ReplaceRange(ThingPosition range, ParsedThingList newThings) {
			Debug.Assert(range.End <= currentThingPosition);
			int rangeLength = range.End - range.Start + 1;
			ParsedThingList attributes = new ParsedThingList();
			if (expression[range.End] is ParsedThingFunc) {
				int currentThing = range.End - 1;
				while (currentThing >= range.Start) {
					ParsedThingAttrSpaceBase attrSpace = expression[currentThing] as ParsedThingAttrSpaceBase;
					if (attrSpace != null) {
						if (ShouldTakeAttrSpaceIntoAccount(attrSpace)) {
							attributes.Insert(0, attrSpace);
							currentSpacesCount--;
						}
					}
					else
						break;
					currentThing--;
				}
			}
			int positionDelta = newThings.Count - rangeLength + attributes.Count;
			expression.RemoveRange(range.Start, rangeLength);
			expression.InsertRange(range.Start, newThings);
			expression.InsertRange(range.Start, attributes);
			currentThingPosition += positionDelta;
			MoveMemThings(range.Start, rangeLength, positionDelta);
			MoveAttrThings(range.Start, rangeLength, positionDelta);
			FormulaChanged = true;
			return attributes.Count;
		}
		protected internal virtual void MoveMemThings(int startPosition, int rangeLength, int positionDelta) {
			for (int i = memThingPositions.Count - 1; i >= 0; i--) {
				MemThingPosition memPosition = memThingPositions[i];
				if (memPosition.Position.Start >= startPosition) {
					if (memPosition.Position.End < startPosition + rangeLength)
						throw new ArgumentException("Can not move mem thing.");
					else
						memThingPositions.RemoveAt(i);
				}
				else {
					memPosition.Thing.InnerThingCount += positionDelta;
					memPosition.Position.End += positionDelta;
				}
			}
		}
		protected internal virtual void MoveAttrThings(int startPosition, int rangeLength, int positionDelta) {
			for (int i = attrOffsetThingPositions.Count - 1; i >= 0; i--) {
				AttrThingPosition attrPosition = attrOffsetThingPositions[i];
				if (attrPosition.Position.Start >= startPosition) {
					if (attrPosition.Position.End < startPosition + rangeLength)
						throw new ArgumentException("Can not move attr thing.");
					else
						attrOffsetThingPositions.RemoveAt(i);
				}
				else {
					attrPosition.MoveOffcet(positionDelta);
					attrPosition.Position.End += positionDelta;
				}
			}
		}
		#endregion
		protected VariantValue CalculateSubExpression(ThingPosition range) {
			if (range.Length <= 0)
				return VariantValue.Missing;
			Debug.Assert(range.Start >= 0 && range.End < expression.Count);
			ParsedExpression innerExpression = new ParsedExpression();
			innerExpression.AddRange(expression.GetRange(range.Start, range.Length));
			return innerExpression.Evaluate(context);
		}
	}
	public class ThingPosition {
		public ThingPosition(int start, int end) {
			this.Start = start;
			this.End = end;
		}
		public int Start { get; set; }
		public int End { get; set; }
		public int Length { get { return End - Start + 1; } }
	}
	public class MemThingPosition {
		ParsedThingMemBase thing;
		ThingPosition position;
		public MemThingPosition(ParsedThingMemBase thing, ThingPosition position) {
			this.thing = thing;
			this.position = position;
		}
		public ParsedThingMemBase Thing { get { return thing; } set { thing = value; } }
		public ThingPosition Position { get { return position; } set { position = value; } }
	}
	public abstract class AttrThingPosition {
		ThingPosition position;
		protected AttrThingPosition(ThingPosition position) {
			this.position = position;
		}
		public ThingPosition Position { get { return position; } set { position = value; } }
		public abstract void MoveOffcet(int delta);
	}
	public class AttrIfThingPosition : AttrThingPosition {
		ParsedThingAttrIf thing;
		public AttrIfThingPosition(ParsedThingAttrIf thing, ThingPosition position)
			: base(position) {
			this.thing = thing;
		}
		public ParsedThingAttrIf Thing { get { return thing; } set { thing = value; } }
		public override void MoveOffcet(int delta) {
			thing.Offset += delta;
		}
	}
	public class AttrGotoThingPosition : AttrThingPosition {
		ParsedThingAttrGoto thing;
		public AttrGotoThingPosition(ParsedThingAttrGoto thing, ThingPosition position)
			: base(position) {
			this.thing = thing;
		}
		public ParsedThingAttrGoto Thing { get { return thing; } set { thing = value; } }
		public override void MoveOffcet(int delta) {
			thing.Offset += delta;
		}
	}
	#endregion
}
