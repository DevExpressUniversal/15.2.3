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
using System.Text.RegularExpressions;
using System.Globalization;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Import.Html {
	#region AttributePattern
	public static class AttributePattern {
		const string attrNamePattern = @"(?<attrName>\w*)";
		const string valuePattern = "\"?\'?(?<attrValue>[^\"\']*)\"?\'?";
		const string eqPattern = @"(?<attrEq>=|\^=|\$=|~=|\*=|\|=)";
		const string spacePattern = @"\s*";
		static string pattern = String.Format("{0}{1}{0}({2}{0}{3}{0})?", spacePattern, attrNamePattern, eqPattern, valuePattern);
		public static Regex regex = new Regex(pattern);
	}
	#endregion
	#region SelectorAttribute
	public class SelectorAttribute {
		string attributeName;
		string attributeValue;
		string attributeConnector;
		public SelectorAttribute() {
			this.attributeName = String.Empty;
			this.attributeValue = String.Empty;
			this.attributeConnector = String.Empty;
		}
		public string AttributeName { get { return attributeName; } set { attributeName = value; } }
		public string AttributeValue { get { return attributeValue; } set { attributeValue = value; } }
		public string AttributeConnector { get { return attributeConnector; } set { attributeConnector = value; } }
	}
	#endregion
	#region Combinator
	public enum Combinator {
		None,
		WhiteSpace,
		RightAngle,
		PlusSign
	}
	#endregion
	#region SimpleSelector
	public class SimpleSelector {
		string name;
		StyleClasses classes;
		StyleClasses pseudoClasses;
		string id;
		List<SelectorAttribute> selectorAttributes;
		public SimpleSelector() {
			this.name = string.Empty;
			this.classes = new StyleClasses();
			this.pseudoClasses = new StyleClasses();
			this.id = String.Empty;
			this.selectorAttributes = new List<SelectorAttribute>();
		}
		public string Name { get { return name; } set { name = value; } }
		public StyleClasses Classes { get { return classes; } set { classes = value; } }
		public StyleClasses PseudoClasses { get { return pseudoClasses; } set { pseudoClasses = value; } }
		public string Id { get { return id; } set { id = value; } }
		public List<SelectorAttribute> SelectorAttributes { get { return selectorAttributes; } set { selectorAttributes = value; } }
	}
	#endregion
	#region SelectorElement
	public class SelectorElement {
		SimpleSelector simpleSelector;
		Combinator combinator;
		public SelectorElement() {
			this.simpleSelector = new SimpleSelector();
			this.combinator = new Combinator();
		}
		public SimpleSelector SimpleSelector { get { return simpleSelector; } set { simpleSelector = value; } }
		public Combinator Combinator { get { return combinator; } set { combinator = value; } }
	}
	#endregion
	#region Selector
	public class Selector {
		int specificity = -1;
		List<SelectorElement> elements;
		public Selector() {
			this.elements = new List<SelectorElement>();
		}
		public List<SelectorElement> Elements { get { return elements; } set { elements = value; } }
		public int Specifity { get {
				if (specificity == -1)
					specificity = CalculateSpecifityCore(specificity);
				return specificity; 
			} 
		}
		public void InvalidateSpecifity() {
			this.specificity = -1;
		}
		protected virtual int CalculateSpecifityCore(int specificity) {
			int count = elements.Count;
			int idCount = 0;
			int attributesCount = 0;
			int elementsCount = 0;
			for (int i = 0; i < count; i++) {
				SimpleSelector simpleSelector = elements[i].SimpleSelector;
				if (!String.IsNullOrEmpty(simpleSelector.Id))
					idCount++;
				attributesCount += simpleSelector.SelectorAttributes.Count;
				attributesCount += simpleSelector.PseudoClasses.Count;
				attributesCount += simpleSelector.Classes.Count;
				if (!String.IsNullOrEmpty(simpleSelector.Name))
					elementsCount++;
			}
			idCount = Math.Min(255, idCount);
			attributesCount = Math.Min(255, attributesCount);
			elementsCount = Math.Min(2555, elementsCount);
			return  (idCount << 16) + (attributesCount << 8) + elementsCount;
		}
	}
	#endregion
	#region SelectorParserState
	public enum SelectorParserState {
		StartState,
		ReadSelectorName,
		ReadClassName,
		ReadId,
		SkipWord,
		AutoSpace,
		ReadChildElement,
		ReadAttribute,
		ReadPseudoClass,
	}
	#endregion
	#region SelectorParser
	public class SelectorParser {
		#region Field
		string rawText;
		string className;
		string pseudoClassName;
		string attribute;
		string id;
		StringBuilder name;
		Selector selector;
		List<Selector> selectors;
		SelectorParserState state;
		SelectorElement element;
		#endregion
		public SelectorParser(string rawText) {
			this.rawText = rawText;
			this.selectors = new List<Selector>();
			this.className = String.Empty;
			this.pseudoClassName = String.Empty;
			this.attribute = String.Empty;
			this.id = String.Empty;
			this.state = new SelectorParserState();
			this.selector = new Selector();
			this.element = new SelectorElement();
			this.name = new StringBuilder();
		}
		protected internal virtual List<Selector> Parse() {
			rawText = rawText.Trim();
			if (String.IsNullOrEmpty(rawText)) {
				AddElementCore();
				selectors.Add(selector);
			}
			else {
				for (int i = 0; i < rawText.Length; i++) {
					char ch = rawText[i];
					switch (state) {
						case SelectorParserState.StartState:
							StartState(ch);
							break;
						case SelectorParserState.ReadSelectorName:
							ReadSelectorName(ch);
							break;
						case SelectorParserState.ReadId:
							ReadId(ch);
							break;
						case SelectorParserState.ReadClassName:
							ReadClassName(ch);
							break;
						case SelectorParserState.AutoSpace:
							AutoSpace(ch);
							break;
						case SelectorParserState.SkipWord:
							SkipWord(ch);
							break;
						case SelectorParserState.ReadPseudoClass:
							ReadPseudoClass(ch);
							break;
						case SelectorParserState.ReadAttribute:
							ReadAttribute(ch);
							break;
					}
				}
				if (state != SelectorParserState.StartState)
					AddSelector();
			}
			return selectors;
		}
		protected internal void AddSelector() {
			if (!String.IsNullOrEmpty(className)) {
				element.SimpleSelector.Classes.Add(className);
				className = String.Empty;
			}
			if (!String.IsNullOrEmpty(pseudoClassName)) {
				element.SimpleSelector.PseudoClasses.Add(pseudoClassName);
				pseudoClassName = String.Empty;
			}
			if (!String.IsNullOrEmpty(id)) {
				element.SimpleSelector.Id = id.ToUpper(CultureInfo.InvariantCulture);
				id = String.Empty;
			}
			AddElementCore();
			selectors.Add(selector);
			selector = new Selector();
		}
		protected internal void AddElementCore() {
			element.SimpleSelector.Name = name.ToString().ToUpper(CultureInfo.InvariantCulture);
			name.Length = 0;
			selector.Elements.Add(element);
			element = new SelectorElement();
		}
		protected internal void StartState(char ch) {
			if (Char.IsWhiteSpace(ch))
				return;
			if (!IsReadSelectorName(ch)) {
				if (Char.IsLetter(ch) || ch == '*' || ch == '@') {
					name.Append(ch);
					state = SelectorParserState.ReadSelectorName;
				}
				else
					state = SelectorParserState.SkipWord;
			}
		}
		protected internal void ReadSelectorName(char ch) {
			if (Char.IsWhiteSpace(ch)) {
				state = SelectorParserState.AutoSpace;
				return;
			}
			if (!IsReadSelectorName(ch) && !IsStartNewElement(ch))
				name.Append(ch);
		}
		protected internal bool IsReadSelectorName(char ch) {
			switch (ch) {
				case ':':
					state = SelectorParserState.ReadPseudoClass;
					return true;
				case '[':
					state = SelectorParserState.ReadAttribute;
					return true;
				case '.':
					state = SelectorParserState.ReadClassName;
					return true;
				case '#':
					state = SelectorParserState.ReadId;
					return true;
			}
			return false;
		}
		protected internal bool IsStartNewElement(char ch) {
			switch (ch) {
				case '+':
					AddElement(ch, Combinator.PlusSign);
					return true;
				case '>':
					AddElement(ch, Combinator.RightAngle);
					return true;
				case ',':
					AddSelector();
					state = SelectorParserState.StartState;
					return true;
			}
			return false;
		}
		protected internal void ReadId(char ch) {
			if (Char.IsWhiteSpace(ch)) {
				element.SimpleSelector.Id = id.ToUpper(CultureInfo.InvariantCulture);
				id = String.Empty;
				state = SelectorParserState.AutoSpace;
				return;
			}
			if (ch == ',') {
				AddSelector();
				state = SelectorParserState.StartState;
				return;
			}
			id += ch;
		}
		protected internal void ReadClassName(char ch) {
			if (Char.IsWhiteSpace(ch)) {
				element.SimpleSelector.Classes.Add(className);
				className = String.Empty;
				state = SelectorParserState.AutoSpace;
				return;
			}
			if (ch == ',') {
				AddSelector();
				state = SelectorParserState.StartState;
				return;
			}
			if (ch == '.') {
				if (!String.IsNullOrEmpty(className))
					element.SimpleSelector.Classes.Add(className);
				className = String.Empty;
				return;
			}
			className += ch;
		}
		protected internal void AutoSpace(char ch) {
			if (Char.IsWhiteSpace(ch))
				return;
			if (!IsStartNewElement(ch)) {
				AddElement(ch, Combinator.WhiteSpace);
				state = SelectorParserState.ReadSelectorName;
				ReadSelectorName(ch);
			}
		}
		protected internal void SkipWord(char ch) {
			if (Char.IsWhiteSpace(ch) || ch == ',')
				state = SelectorParserState.StartState;
		}
		protected internal void ReadPseudoClass(char ch) {
			if (Char.IsWhiteSpace(ch)) {
				element.SimpleSelector.PseudoClasses.Add(pseudoClassName);
				pseudoClassName = String.Empty;
				state = SelectorParserState.AutoSpace;
				return;
			}
			if (ch == ':') { 
				if (String.IsNullOrEmpty(pseudoClassName))
					element.SimpleSelector.PseudoClasses.Add(pseudoClassName);
				pseudoClassName = String.Empty;
				return;
			}
			if (ch == ',') {
				AddSelector();
				state = SelectorParserState.StartState;
				return;
			}
			pseudoClassName += ch;
		}
		protected internal void AddElement(char ch, Combinator combinator) {
			AddElementCore();
			element.Combinator = combinator;
			state = SelectorParserState.StartState;
		}
		protected internal void ReadAttribute(char ch) {
			if (ch == ']') {
				Match match = AttributePattern.regex.Match(attribute);
				SelectorAttribute selectorAttribute = new SelectorAttribute();
				selectorAttribute.AttributeName = match.Groups["attrName"].Value.ToUpper(CultureInfo.InvariantCulture);
				selectorAttribute.AttributeValue = match.Groups["attrValue"].Value;
				selectorAttribute.AttributeConnector = match.Groups["attrEq"].Value;
				element.SimpleSelector.SelectorAttributes.Add(selectorAttribute);
				attribute = String.Empty;
				state = SelectorParserState.ReadSelectorName; 
				return;
			}
			attribute += ch;
		}
	}
	#endregion
	#region StyleClasses
	public class StyleClasses {
		readonly List<string> innerList = new List<string>();
		internal List<string> InnerList { get { return innerList; } }
		public int Count { get { return InnerList.Count; } }
		public string this[int index] { get { return InnerList[index]; } set { InnerList[index] = value; } }
		public void Add(string className) {
			InnerList.Add(className);
		}
		public void Remove(string className) {
			InnerList.Remove(className);
		}
		public void Clear() {
			InnerList.Clear();
		}
		public bool Contains(string className) {
			int count = InnerList.Count;
			for (int i = 0; i < count; i++) {
				if (String.Equals(InnerList[i], className, StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}
	}
	#endregion
}
