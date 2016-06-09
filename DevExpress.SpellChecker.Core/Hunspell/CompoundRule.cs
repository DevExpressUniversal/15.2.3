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
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;
namespace DevExpress.XtraSpellChecker.Hunspell {
	#region CompoundRuleElement (abstract class)
	public abstract class CompoundRuleElement {
		HunspellFlag flag;
		CompoundRuleElement nextElement;
		protected CompoundRuleElement(HunspellFlag flag, CompoundRuleElement nextElement) {
			this.flag = flag;
			this.nextElement = nextElement;
		}
		public HunspellFlag Flag { get { return flag; } }
		public CompoundRuleElement NextElement { get { return nextElement; } }
		public abstract bool CanSaveState { get; }
		public abstract bool IsFinish { get; }
		protected internal bool IsMatch(WordEntry wordEntry) {
			return wordEntry.ContainsFlag(Flag);
		}
		public abstract CompoundRuleElement MoveNext(WordEntry wordEntry);
	}
	#endregion
	#region ZeroOrOneElement
	public class ZeroOrOneElement : CompoundRuleElement {
		public ZeroOrOneElement(HunspellFlag flag, CompoundRuleElement nextElement)
			: base(flag, nextElement) {
		}
		public override bool CanSaveState { get { return true; } }
		public override bool IsFinish { get { return NextElement.IsFinish; } }
		public override CompoundRuleElement MoveNext(WordEntry wordEntry) {
			if (IsMatch(wordEntry))
				return NextElement;
			else if (NextElement != null)
				return NextElement.MoveNext(wordEntry);
			return null;
		}
	}
	#endregion
	#region ZeroOrMoreElement
	public class ZeroOrMoreElement : CompoundRuleElement {
		public ZeroOrMoreElement(HunspellFlag flag, CompoundRuleElement nextElement)
			: base(flag, nextElement) {
		}
		public override bool CanSaveState { get { return true; } }
		public override bool IsFinish { get { return NextElement.IsFinish; } }
		public override CompoundRuleElement MoveNext(WordEntry wordEntry) {
			if (IsMatch(wordEntry))
				return this;
			else if (NextElement != null)
				return NextElement.MoveNext(wordEntry);
			return null;
		}
	}
	#endregion
	#region SingleElement
	public class SingleElement : CompoundRuleElement {
		public SingleElement(HunspellFlag flag, CompoundRuleElement nextElement)
			: base(flag, nextElement) {
		}
		public override bool CanSaveState { get { return false; } }
		public override bool IsFinish { get { return false; } }
		public override CompoundRuleElement MoveNext(WordEntry wordEntry) {
			if (IsMatch(wordEntry))
				return NextElement;
			return null;
		}
	}
	#endregion
	#region FinishElement
	public class FinishElement : CompoundRuleElement {
		public FinishElement()
			: base(HunspellFlag.Empty, null) {
		}
		public override bool CanSaveState { get { return false; } }
		public override bool IsFinish { get { return true; } }
		public override CompoundRuleElement MoveNext(WordEntry wordEntry) {
			return null;
		}
	}
	#endregion
	public enum CompoundRuleCheckResult {
		Success,
		PartiallyMatch,
		Failure
	}
	#region CompoundRule
	public class CompoundRule {
		#region State (inner class)
		protected internal class State {
			int wordEntryIndex;
			CompoundRuleElement element;
			public State(int index, CompoundRuleElement element) {
				this.wordEntryIndex = index;
				this.element = element;
			}
			public int WordEntryIndex { get { return wordEntryIndex; } }
			public CompoundRuleElement Element { get { return element; } }
		}
		#endregion
		CompoundRuleElement rootElement;
		string rule;
		public CompoundRule(string rule, CompoundRuleElement rootElement) {
			this.rule = rule;
			this.rootElement = rootElement;
		}
		public virtual CompoundRuleCheckResult Validate(List<WordInfo> wordEntries) {
			CompoundRuleElement currentElement = rootElement;
			Stack<State> returnStack = new Stack<State>();
			int index = 0;
			while (true) {
				CompoundRuleElement lastElement = ProcessWordEntries(wordEntries, index, currentElement, returnStack);
				if (lastElement == null)
					return CompoundRuleCheckResult.Failure;
				if (lastElement != null && lastElement.IsFinish)
					return CompoundRuleCheckResult.Success;
				else if (returnStack.Count == 0)
					return  CompoundRuleCheckResult.PartiallyMatch;
				State state = returnStack.Pop();
				index = state.WordEntryIndex;
				currentElement = state.Element.NextElement;
			}
		}
		CompoundRuleElement ProcessWordEntries(List<WordInfo> wordInfos, int index, CompoundRuleElement currentElement, Stack<State> returnStack) {
			int lastIndex = wordInfos.Count - 1;
			while (index <= lastIndex) {
				WordEntry wordEntry = wordInfos[index].Root;
				CompoundRuleElement nextElement = currentElement.MoveNext(wordEntry);
				if (nextElement == null) {
					if (returnStack.Count > 0) {
						State state = returnStack.Pop();
						index = state.WordEntryIndex;
						currentElement = state.Element.NextElement;
					}
					else
						return null;
				}
				else {
					if (currentElement.CanSaveState)
						returnStack.Push(new State(index, currentElement));
					currentElement = nextElement;
					index++;
				}
			}
			return currentElement;
		}
		public override string ToString() {
			return rule;
		}
		public virtual bool IsWordInfoFitInWithRule(WordInfo wordInfo) {
			CompoundRuleElement element = rootElement;
			while (element != null) {
				if (element.IsMatch(wordInfo.Root))
					return true;
				element = element.NextElement;
			}
			return false;
		}
	}
	#endregion
}
