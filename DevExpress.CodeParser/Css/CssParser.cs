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
using System.Collections;
using System.Collections.Specialized;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.Css
#else
namespace DevExpress.CodeParser.Css
#endif
{
	using Xml;
	public  class Tokens
	{
		public const int TIME = 41;
		public const int AT = 1;
		public const int EOF = 0;
		public const int BEGINSWITH = 24;
		public const int EMS = 38;
		public const int IDENT = 10;
		public const int CDO = 2;
		public const int LENGTH = 37;
		public const int GREATER = 14;
		public const int PLUS = 13;
		public const int FUNCTION = 28;
		public const int MULTILINECOMMENT = 44;
		public const int ANGLE = 40;
		public const int PERCENTAGE = 36;
		public const int PARENOPEN = 29;
		public const int AND = 34;
		public const int IMPORTANTSYM = 31;
		public const int NUMBER = 35;
		public const int STRING = 4;
		public const int STAR = 19;
		public const int SINGLELINECOMMENT = 43;
		public const int COLON = 11;
		public const int EQUALSIGN = 21;
		public const int SEMICOLON = 5;
		public const int CURLYOPEN = 8;
		public const int PARENCLOSE = 30;
		public const int EXS = 39;
		public const int COMMA = 7;
		public const int SLASH = 12;
		public const int CONTAINS = 26;
		public const int HASH = 17;
		public const int TILDE = 16;
		public const int FREQ = 42;
		public const int CURLYCLOSE = 9;
		public const int INCLUDES = 22;
		public const int ONLY = 33;
		public const int BRACKETCLOSE = 27;
		public const int MINUS = 15;
		public const int DOT = 18;
		public const int BRACKETOPEN = 20;
		public const int URI = 6;
		public const int DASHMATCH = 23;
		public const int NOT = 32;
		public const int ENDSWITH = 25;
		public const int CDC = 3;
		public const int MaxTokens = 45;
		public static int[] Keywords = {
		};
	}
	partial class CssParser
	{
		protected override void HandlePragmas()
		{
		}
			void Parser()
	{
		CssStyleSheet styleSheet = new CssStyleSheet();
		styleSheet.SetRange(la.Range);
		OpenContext(styleSheet);
		if (IsSpecialName("charset"))
		{
			Expect(Tokens.AT );
			Expect(Tokens.IDENT );
			SetKeywordTokenCategory(tToken); 
			CssCharsetDeclaration charsetDecl = new CssCharsetDeclaration(); 
			charsetDecl.SetRange(tToken.Range);
			charsetDecl.Charset = la.Value;
			charsetDecl.Name = la.Value;
			charsetDecl.NameRange = la.Range;
			Expect(Tokens.STRING );
			Expect(Tokens.SEMICOLON );
			charsetDecl.SetRange(GetRange(charsetDecl, tToken));
			AddNode(charsetDecl);
		}
		while (la.Type == Tokens.CDO  || la.Type == Tokens.CDC )
		{
			HtmlCommentTags();
		}
		while (IsSpecialName("import"))
		{
			Import();
			while (la.Type == Tokens.CDO  || la.Type == Tokens.CDC )
			{
				HtmlCommentTags();
			}
		}
		while (StartOf(1))
		{
			if (la.Type == Tokens.AT )
			{
				Get();
				if(IsSpecialNameWithoutAt("font-face"))
				{
				FontFace();
				}
				if(IsSpecialNameWithoutAt("media"))
				{
				Media();
				}
				if(IsSpecialNameWithoutAt("page"))
				{
				Page();
				}
			}
			else
			{
				RuleSet();
			}
			while (la.Type == Tokens.CDO  || la.Type == Tokens.CDC )
			{
				HtmlCommentTags();
			}
		}
		CloseContext();
		styleSheet.SetRange(GetRange(styleSheet, tToken));
	}
	void HtmlCommentTags()
	{
		CssCommentTag commentTag = new CssCommentTag();
		commentTag.Name = la.Value;
		commentTag.SetRange(la.Range);
		AddNode(commentTag);
		if (la.Type == Tokens.CDO )
		{
			Get();
			commentTag.Kind = CommentTagKind.Open;
		}
		else if (la.Type == Tokens.CDC )
		{
			Get();
			commentTag.Kind = CommentTagKind.Close;
		}
		else
			SynErr(46);
	}
	void Import()
	{
		CssImportDirective importDirective = new CssImportDirective();
		AddNode(importDirective);
		importDirective.SetRange(la.Range);
		CssMediaQueryCollection queris;
		Expect(Tokens.AT );
		Expect(Tokens.IDENT );
		SetKeywordTokenCategory(tToken); 
		if (la.Type == Tokens.STRING )
		{
			Get();
		}
		else if (la.Type == Tokens.URI )
		{
			Get();
		}
		else
			SynErr(47);
		importDirective.Source = tToken.Value; 
		MediaQueryList(out queris);
		importDirective.SetQueries(queris); 
		if (la.Type == Tokens.IDENT )
		{
			IdentifierList(importDirective.SupportedMediaTypes);
		}
		Expect(Tokens.SEMICOLON );
		importDirective.SetRange(GetRange(importDirective, tToken)); 
	}
	void FontFace()
	{
		Expect(Tokens.AT );
		SetKeywordTokenCategory(la); 
		RuleSet();
	}
	void Media()
	{
		CssMediaDirective mediaDirective = new CssMediaDirective();
		mediaDirective.SetRange(tToken.Range);
		OpenContext(mediaDirective);
		CssMediaQueryCollection queris;
		Expect(Tokens.IDENT );
		SetKeywordTokenCategory(tToken); 
		MediaQueryList(out queris);
		mediaDirective.SetQueries(queris); 
		Expect(Tokens.CURLYOPEN );
		while (StartOf(2))
		{
			RuleSet();
		}
		Expect(Tokens.CURLYCLOSE );
		mediaDirective.SetRange(GetRange(mediaDirective, tToken));
		CloseContext();
	}
	void Page()
	{
		CssPropertyDeclaration propertyDecl = null;
		CssPageStyle page = new CssPageStyle();
		page.SetRange(tToken.Range);
		AddNode(page);
		Expect(Tokens.IDENT );
		SetKeywordTokenCategory(tToken); 
		if (la.Type == Tokens.COLON )
		{
			Get();
			Expect(Tokens.IDENT );
			page.Name = tToken.Value; 
		}
		Expect(Tokens.CURLYOPEN );
		Declaration(out propertyDecl);
		if (la.Type == Tokens.SEMICOLON )
		{
			Get();
		}
		if (propertyDecl != null)
		{
		  page.Properties.Add(propertyDecl);
		  page.AddNode(propertyDecl);
		  if (tToken.Type == Tokens.SEMICOLON)
			propertyDecl.SetRange(GetRange(propertyDecl, tToken));
		}
		while (la.Type == Tokens.IDENT  || la.Type == Tokens.STAR )
		{
			Declaration(out propertyDecl);
			if (la.Type == Tokens.SEMICOLON )
			{
				Get();
			}
			if (propertyDecl != null)
			{
			  page.Properties.Add(propertyDecl);
			  page.AddNode(propertyDecl);
			  if (tToken.Type == Tokens.SEMICOLON)
			  {
				propertyDecl.SetRange(GetRange(propertyDecl, tToken));
			  }
			} 
		}
		Expect(Tokens.CURLYCLOSE );
		page.SetRange(GetRange(page, tToken)); 
	}
	void RuleSet()
	{
		CssStyleRule rule = new CssStyleRule();
		OpenContext(rule);
		if (tToken.Type == Tokens.AT)
		  rule.SetRange(tToken.Range);
		else
		  rule.SetRange(la.Range);
		CssSelector currentSelector = null;
		CssPropertyDeclaration propertyDecl = null;
		Selector(out currentSelector);
		if (currentSelector != null)
		{
		  rule.Selectors.Add(currentSelector);
		  rule.AddNode(currentSelector);
		}
		while (la.Type == Tokens.COMMA )
		{
			Get();
			Selector(out currentSelector);
			if (currentSelector != null)
			{
			  rule.Selectors.Add(currentSelector);
			  rule.AddNode(currentSelector);
			}
		}
		Expect(Tokens.CURLYOPEN );
		rule.SetBlockStart(tToken.Range); 
		if (la.Type == Tokens.IDENT  || la.Type == Tokens.STAR )
		{
			Declaration(out propertyDecl);
			if (la.Type == Tokens.SEMICOLON )
			{
				Get();
			}
			if (propertyDecl != null)
			{
			  rule.Properties.Add(propertyDecl);
			  rule.AddNode(propertyDecl);
			  if (tToken.Type == Tokens.SEMICOLON)
				propertyDecl.SetRange(GetRange(propertyDecl, tToken));
			}
			while (la.Type == Tokens.IDENT  || la.Type == Tokens.STAR )
			{
				Declaration(out propertyDecl);
				if (la.Type == Tokens.SEMICOLON )
				{
					Get();
				}
				if (propertyDecl != null)
				{
				  rule.Properties.Add(propertyDecl);
				  rule.AddNode(propertyDecl);
				  if (tToken.Type == Tokens.SEMICOLON)
				  {
					propertyDecl.SetRange(GetRange(propertyDecl, tToken));
				  }
				}
			}
		}
		Expect(Tokens.CURLYCLOSE );
		rule.SetBlockEnd(tToken.Range); 
		CloseContext();
		rule.SetRange(GetRange(rule, tToken));
	}
	void IdentifierList(StringCollection stringCollection)
	{
		Expect(Tokens.IDENT );
		if(stringCollection != null)
		 stringCollection.Add(tToken.Value);
		while (la.Type == Tokens.COMMA )
		{
			Get();
			Expect(Tokens.IDENT );
			if(stringCollection != null)
			 stringCollection.Add(tToken.Value);
		}
	}
	void MediaQueryList(out CssMediaQueryCollection result)
	{
		result = new CssMediaQueryCollection();
		CssMediaQuery query;
		MediaQuery(out query);
		result.Add(query); 
		while (la.Type == Tokens.COMMA )
		{
			Get();
			MediaQuery(out query);
			result.Add(query); 
		}
	}
	void MediaQuery(out CssMediaQuery result)
	{
		result = new CssMediaQuery();
		result.SetRange(la.Range);
		CssMediaExpression expr;
		if (la.Type == Tokens.IDENT  || la.Type == Tokens.NOT  || la.Type == Tokens.ONLY )
		{
			if (la.Type == Tokens.NOT  || la.Type == Tokens.ONLY )
			{
				if (la.Type == Tokens.ONLY )
				{
					Get();
					result.Prefix = CssMediaQueryPrefix.Only; 
				}
				else
				{
					Get();
					result.Prefix = CssMediaQueryPrefix.Not; 
				}
			}
			Expect(Tokens.IDENT );
			result.Name = tToken.Value;
			result.NameRange = tToken.Range;
		}
		else if (la.Type == Tokens.PARENOPEN )
		{
			MediaExpr(out expr);
			result.AddExpression(expr); 
		}
		else
			SynErr(48);
		while (la.Type == Tokens.AND )
		{
			Get();
			MediaExpr(out expr);
			result.AddExpression(expr); 
		}
		result.SetRange(GetRange(result.Range, tToken.Range));
	}
	void MediaExpr(out CssMediaExpression result)
	{
		result = new CssMediaExpression();
		result.SetRange(la.Range);
		CssTerm term;
		Expect(Tokens.PARENOPEN );
		result.Name = la.Value;
		result.NameRange = la.Range;
		Expect(Tokens.IDENT );
		if (la.Type == Tokens.COLON )
		{
			Get();
			Term(out term);
			result.Expression = term; 
		}
		Expect(Tokens.PARENCLOSE );
		result.SetRange(GetRange(result.Range, tToken.Range)); 
	}
	void Term(out CssTerm result)
	{
		result = null;
		PrecedingSign sign = PrecedingSign.None;
		CssMeasureUnit unit = CssMeasureUnit.None;
		CssFunctionReference functionReference = null;
		Object value = null;
		switch (la.Type)
		{
		case Tokens.PLUS : 
case Tokens.MINUS : 
case Tokens.NUMBER : 
case Tokens.PERCENTAGE : 
case Tokens.LENGTH : 
case Tokens.EMS : 
case Tokens.EXS : 
case Tokens.ANGLE : 
case Tokens.TIME : 
case Tokens.FREQ : 
		{
			if (la.Type == Tokens.PLUS  || la.Type == Tokens.MINUS )
			{
				if (la.Type == Tokens.MINUS )
				{
					Get();
					sign = PrecedingSign.Minus;
				}
				else
				{
					Get();
					sign = PrecedingSign.Plus;
				}
			}
			if (la.Type == Tokens.NUMBER )
			{
				Get();
				result = new CssNumberLiteral();
				result.Name = tToken.Value;
				result.SetRange(tToken.Range);
				int len = tToken.Value.Length;
				if (tToken.Value[len - 1] == '%')
				{
				  (result as CssMeasuredNumber).Unit = CssMeasureUnit.Percent;
				  len--;
				}
				(result as CssNumberLiteral).Value = GetValueFromString(tToken.Value, tToken.Value.Length);
			}
			else if (la.Type == Tokens.PERCENTAGE )
			{
				Get();
				result = new CssPercentLiteral();
				result.Name = tToken.Value;
				result.SetRange(tToken.Range);
				(result as CssMeasuredNumber).Value = GetValueFromString(tToken.Value, tToken.Value.Length - 1);
			}
			else if (StartOf(3))
			{
				if (la.Type == Tokens.LENGTH  || la.Type == Tokens.EMS  || la.Type == Tokens.EXS )
				{
					if (la.Type == Tokens.LENGTH )
					{
						Get();
					}
					else if (la.Type == Tokens.EMS )
					{
						Get();
					}
					else if (la.Type == Tokens.EXS )
					{
						Get();
					}
					else
						SynErr(49);
					result = new CssLengthLiteral(); 
				}
				else if (la.Type == Tokens.ANGLE )
				{
					Get();
					result = new CssAngleLiteral(); 
				}
				else if (la.Type == Tokens.TIME )
				{
					Get();
					result = new CssTimeLiteral(); 
				}
				else
				{
					Get();
					result = new CssFrequencyLiteral(); 
				}
				result.Name = tToken.Value;
				result.SetRange(tToken.Range);
				GetMeasuredLiteralValue(tToken.Value, out value, out unit);
				(result as CssMeasuredNumber).Value = value;
				(result as CssMeasuredNumber).Unit = unit;
			}
			else
				SynErr(50);
			if (result != null)
			result.Sign = sign;
			break;
		}
		case Tokens.STRING : 
		{
			Get();
			result = new CssStringLiteral();
			result.Name = tToken.Value;
			(result as CssStringLiteral).Value = tToken.Value;
			result.SetRange(tToken.Range);
			break;
		}
		case Tokens.IDENT : 
case Tokens.DOT : 
		{
			if (la.Type == Tokens.DOT )
			{
				Get();
				SetCategoryIfNeeded(TokenCategory.CssPropertyValue); 
			}
			Expect(Tokens.IDENT );
			result = new CssIdentifierReference();
			result.Name = tToken.Value;
			result.SetRange(tToken.Range);
			while (la.Type == Tokens.COLON  || la.Type == Tokens.DOT )
			{
				if (la.Type == Tokens.DOT )
				{
					Get();
				}
				else
				{
					Get();
				}
				SetCategoryIfNeeded(TokenCategory.CssPropertyValue); 
				if (la.Type == Tokens.IDENT )
				{
					Get();
					SetCategoryIfNeeded(TokenCategory.CssPropertyValue);
					CssIdentifierReference newReference = new CssIdentifierReference();
					newReference.Name = tToken.Value;
					newReference.SetRange(GetRange(result, tToken));
					newReference.AddDetailNode(result);
					result = newReference;
				}
				else if (la.Type == Tokens.FUNCTION )
				{
					Function(out functionReference);
					functionReference.AddDetailNode(result);
					result = functionReference;
				}
				else
					SynErr(51);
			}
			break;
		}
		case Tokens.URI : 
		{
			Get();
			result = new CssURIReference();
			result.Name = tToken.Value;
			result.SetRange(tToken.Range);
			(result as CssURIReference).URI = tToken.Value;
			break;
		}
		case Tokens.HASH : 
		{
			Get();
			result = new CssColorReference();
			result.Name = tToken.Value;
			result.SetRange(tToken.Range);
			break;
		}
		case Tokens.FUNCTION : 
		{
			Function(out functionReference);
			result = functionReference; 
			break;
		}
		default: SynErr(52); break;
		}
	}
	void Declaration(out CssPropertyDeclaration propertyDecl)
	{
		propertyDecl = new CssPropertyDeclaration();
		CssTermCollection terms = null;
		propertyDecl.SetRange(la.Range);
		propertyDecl.Name = la.Value;
		propertyDecl.NameRange = la.Range;
		ElementName();
		SetCategoryIfNeeded(TokenCategory.CssPropertyName); 
		if (la.Type == Tokens.COLON )
		{
			Get();
		}
		else if (la.Type == Tokens.EQUALSIGN )
		{
			Get();
			propertyDecl.IsEqual = true;
		}
		else
			SynErr(53);
		Expr(out terms);
		if (la.Type == Tokens.IMPORTANTSYM )
		{
			Get();
			propertyDecl.IsImportant = true;
			Expect(Tokens.IDENT );
		}
		propertyDecl.SetRange(GetRange(propertyDecl, tToken));
		if (terms != null && terms.Count > 0)
		{
		  for (int i = 0; i < terms.Count; i++)
		  {
			propertyDecl.Values.Add(terms[i]);
			propertyDecl.AddDetailNode(terms[i]);
		  }
		}
	}
	void Combinator(out CssSelectorType combinatorType)
	{
		combinatorType = CssSelectorType.Name;
		if (la.Type == Tokens.PLUS )
		{
			Get();
			combinatorType = CssSelectorType.CombinedSibling;
			SetCategoryIfNeeded(TokenCategory.CssSelector);
		}
		else if (la.Type == Tokens.GREATER )
		{
			Get();
			combinatorType = CssSelectorType.CombinedChild;
			SetCategoryIfNeeded(TokenCategory.CssSelector);
		}
		else if (la.Type == Tokens.TILDE )
		{
			Get();
			combinatorType = CssSelectorType.CombinedGeneralSibling;
			SetCategoryIfNeeded(TokenCategory.CssSelector);
		}
		else
			SynErr(54);
	}
	void Selector(out CssSelector selector)
	{
		CssSelector currentSelector = null;
		CssSelectorType combinatorType = CssSelectorType.Name;
		CssSelector firstSelector = null;
		selector = null;
		SimpleSelector(out firstSelector);
		currentSelector = firstSelector; 
		while (la.Type == Tokens.PLUS  || la.Type == Tokens.GREATER  || la.Type == Tokens.TILDE )
		{
			Combinator(out combinatorType);
			SimpleSelector(out selector);
			if (currentSelector != null && selector != null)
			{
			  selector.SelectorType = combinatorType;
			  ConnectSelectors(currentSelector, selector);	
			}
			currentSelector = selector;
			if (firstSelector != null)
			  firstSelector.SetRange(GetRange(firstSelector, currentSelector));
		}
		selector = firstSelector; 
	}
	void SimpleSelector(out CssSelector selector)
	{
		selector = new CssSelector();
		CssSelector currentSelector = null;
		if (tToken.Type == Tokens.AT)
		  selector.SetRange(tToken.Range);
		else
		  selector.SetRange(la.Range);
		CssAttributeSelector attrSelector = null;
		CssPseudoSelector pseudoSelector = null;
		CssSelector firstSelector = null;
		while (StartOf(4))
		{
			if (la.Type == Tokens.IDENT  || la.Type == Tokens.STAR )
			{
				ElementName();
				selector = new CssSelector();
				selector.Name = tToken.Value;
				selector.NameRange = tToken.Range;
				selector.SetRange(tToken.Range);
				ConnectSelectors(currentSelector, selector);
				SetCategoryIfNeeded(TokenCategory.CssSelector);
				if (firstSelector == null)
				  firstSelector = selector;
				currentSelector = selector;
			}
			else if (la.Type == Tokens.HASH )
			{
				Get();
				selector = new CssSelector();
				selector.SetRange(tToken.Range);
				selector.SelectorType = CssSelectorType.Hash;
				SetCategoryIfNeeded(TokenCategory.CssSelector);
				if (tToken.Value.Length > 1)
				{
				  selector.Name = tToken.Value.Substring(1);
				  SourceRange hashRange = tToken.Range;
				  selector.NameRange = new SourceRange(hashRange.Top.Line, hashRange.Top.Offset + 1, hashRange.End.Line, hashRange.End.Offset);
				}
				ConnectSelectors(currentSelector, selector);
				if (firstSelector == null)
				  firstSelector = selector;
				currentSelector = selector;
			}
			else if (la.Type == Tokens.DOT )
			{
				Class(out selector);
				ConnectSelectors(currentSelector, selector);
				if (firstSelector == null)
				  firstSelector = selector;
				currentSelector = selector;
			}
			else if (la.Type == Tokens.BRACKETOPEN )
			{
				Attrib(out attrSelector);
				ConnectSelectors(currentSelector, attrSelector);
				if (firstSelector == null)
				  firstSelector = selector;
				currentSelector = attrSelector;
			}
			else
			{
				Pseudo(out pseudoSelector);
				ConnectSelectors(currentSelector, pseudoSelector);
				if (firstSelector == null)
				  firstSelector = selector;
				currentSelector = pseudoSelector;
			}
		}
		selector = firstSelector;  
	}
	void ElementName()
	{
		if (la.Type == Tokens.IDENT )
		{
			Get();
		}
		else if (la.Type == Tokens.STAR )
		{
			Get();
			if (la.Type == Tokens.IDENT )
			{
				Get();
			}
		}
		else
			SynErr(55);
	}
	void Class(out CssSelector selector)
	{
		selector = new CssSelector();
		selector.SelectorType = CssSelectorType.Class;
		Expect(Tokens.DOT );
		SetCategoryIfNeeded(TokenCategory.CssSelector);
		selector.SetRange(GetRange(tToken, la));
		selector.Name = la.Value;
		selector.NameRange = la.Range;
		Expect(Tokens.IDENT );
		SetCategoryIfNeeded(TokenCategory.CssSelector); 
	}
	void Attrib(out CssAttributeSelector attrSelector)
	{
		attrSelector = new CssAttributeSelector();
		attrSelector.SetRange(la.Range);
		Expect(Tokens.BRACKETOPEN );
		SetCategoryIfNeeded(TokenCategory.CssSelector); 
		Expect(Tokens.IDENT );
		attrSelector.Name = tToken.Value; attrSelector.NameRange = tToken.Range;
		SetCategoryIfNeeded(TokenCategory.CssSelector);
		if (StartOf(5))
		{
			switch (la.Type)
			{
			case Tokens.EQUALSIGN : 
			{
				Get();
				attrSelector.EqualityType = AttributeSelectorEqualityType.ExactMatch; 
				break;
			}
			case Tokens.INCLUDES : 
			{
				Get();
				attrSelector.EqualityType = AttributeSelectorEqualityType.SpaceSeparatedMatch; 
				break;
			}
			case Tokens.DASHMATCH : 
			{
				Get();
				attrSelector.EqualityType = AttributeSelectorEqualityType.HyphenSeparatedMatch; 
				break;
			}
			case Tokens.BEGINSWITH : 
			{
				Get();
				attrSelector.EqualityType = AttributeSelectorEqualityType.BeginsWith; 
				break;
			}
			case Tokens.ENDSWITH : 
			{
				Get();
				attrSelector.EqualityType = AttributeSelectorEqualityType.EndsWith; 
				break;
			}
			case Tokens.CONTAINS : 
			{
				Get();
				attrSelector.EqualityType = AttributeSelectorEqualityType.Contains; 
				break;
			}
			}
			if (la.Type == Tokens.IDENT )
			{
				Get();
			}
			else if (la.Type == Tokens.STRING )
			{
				Get();
			}
			else
				SynErr(56);
			attrSelector.Value = tToken.Value;
			SetCategoryIfNeeded(TokenCategory.CssSelector);
		}
		Expect(Tokens.BRACKETCLOSE );
		SetCategoryIfNeeded(TokenCategory.CssSelector);
		attrSelector.SetRange(GetRange(attrSelector, tToken));
	}
	void Pseudo(out CssPseudoSelector pseudoSelector)
	{
		Token startToken = la;
		pseudoSelector = null;
		Expect(Tokens.COLON );
		SetCategoryIfNeeded(TokenCategory.CssSelector); 
		if (la.Type == Tokens.IDENT )
		{
			Get();
			pseudoSelector = new CssPseudoSelector();
			pseudoSelector.Name = tToken.Value;
			pseudoSelector.NameRange = tToken.Range;
			pseudoSelector.PseudoClassType = GetPseudoClassType(tToken.Value);
			SetCategoryIfNeeded(TokenCategory.CssSelector);
		}
		else if (la.Type == Tokens.FUNCTION )
		{
			Get();
			SetCategoryIfNeeded(TokenCategory.CssSelector);
			pseudoSelector = new CssPseudoFunctionSelector();
			pseudoSelector.Name = tToken.Value.Substring(0, tToken.Value.Length - 1);
			SourceRange hashRange = tToken.Range;
			pseudoSelector.NameRange = new SourceRange(hashRange.Top.Line, hashRange.Top.Offset, hashRange.End.Line, hashRange.End.Offset - 1);
			pseudoSelector.PseudoClassType = GetPseudoClassType(pseudoSelector.Name);
			if (StartOf(6))
			{
				if (la.Type == Tokens.IDENT  || la.Type == Tokens.DOT )
				{
					if (la.Type == Tokens.DOT )
					{
						Get();
						SetCategoryIfNeeded(TokenCategory.CssSelector); 
					}
					Expect(Tokens.IDENT );
					if (la.Type == Tokens.PLUS  || la.Type == Tokens.MINUS )
					{
						if (la.Type == Tokens.PLUS )
						{
							Get();
						}
						else
						{
							Get();
						}
						Expect(Tokens.NUMBER );
					}
					(pseudoSelector as CssPseudoFunctionSelector).Param = tToken.Value;
					SetCategoryIfNeeded(TokenCategory.CssSelector);
				}
				else if (la.Type == Tokens.NUMBER )
				{
					Get();
					if (la.Type == Tokens.IDENT )
					{
						Get();
						if (la.Type == Tokens.PLUS  || la.Type == Tokens.MINUS )
						{
							if (la.Type == Tokens.PLUS )
							{
								Get();
							}
							else
							{
								Get();
							}
							Expect(Tokens.NUMBER );
						}
					}
				}
				else
				{
					if (la.Type == Tokens.PLUS )
					{
						Get();
					}
					else if (la.Type == Tokens.MINUS )
					{
						Get();
					}
					else
						SynErr(57);
					if (la.Type == Tokens.NUMBER )
					{
						Get();
						if (la.Type == Tokens.IDENT )
						{
							Get();
							if (la.Type == Tokens.PLUS  || la.Type == Tokens.MINUS )
							{
								if (la.Type == Tokens.PLUS )
								{
									Get();
								}
								else
								{
									Get();
								}
								Expect(Tokens.NUMBER );
							}
						}
					}
					else if (la.Type == Tokens.IDENT )
					{
						Get();
						if (la.Type == Tokens.PLUS  || la.Type == Tokens.MINUS )
						{
							if (la.Type == Tokens.PLUS )
							{
								Get();
							}
							else
							{
								Get();
							}
							Expect(Tokens.NUMBER );
						}
					}
					else
						SynErr(58);
				}
			}
			Expect(Tokens.PARENCLOSE );
			SetCategoryIfNeeded(TokenCategory.CssSelector); 
		}
		else
			SynErr(59);
		if (pseudoSelector != null)
		 pseudoSelector.SetRange(GetRange(startToken, tToken));
	}
	void Expr(out CssTermCollection terms)
	{
		CssTerm term = null;
		terms = new CssTermCollection();
		ExpressionDelimiter currentDelimiter = ExpressionDelimiter.None;
		Term(out term);
		if (term != null)
		{
		  term.Delimiter = ExpressionDelimiter.None;
		  terms.Add(term);
		}
		while (StartOf(7))
		{
			currentDelimiter = ExpressionDelimiter.Whitespace; 
			if (la.Type == Tokens.COMMA  || la.Type == Tokens.SLASH  || la.Type == Tokens.EQUALSIGN )
			{
				if (la.Type == Tokens.SLASH )
				{
					Get();
					currentDelimiter = ExpressionDelimiter.Slash; 
				}
				else if (la.Type == Tokens.COMMA )
				{
					Get();
					currentDelimiter = ExpressionDelimiter.Comma; 
				}
				else
				{
					Get();
					currentDelimiter = ExpressionDelimiter.Equals; 
				}
				SetCategoryIfNeeded(TokenCategory.CssPropertyValue); 
			}
			Term(out term);
			if (term != null)
			{
			  term.Delimiter = currentDelimiter;
			  terms.Add(term);
			}
		}
	}
	void Function(out CssFunctionReference functionReference)
	{
		functionReference = new CssFunctionReference() ;
		functionReference.SetRange(la.Range);
		CssTermCollection terms = null;
		Expect(Tokens.FUNCTION );
		functionReference.Name = tToken.Value.Substring(0, tToken.Value.Length - 1);
		SetCategoryIfNeeded(TokenCategory.CssPropertyValue);
		Expr(out terms);
		Expect(Tokens.PARENCLOSE );
		SetCategoryIfNeeded(TokenCategory.CssPropertyValue);
		functionReference.SetRange(GetRange(functionReference, tToken));
		if (terms != null && terms.Count > 0)
		{
		  for (int  i = 0; i < terms.Count; i++)
		  {
			functionReference.Parameters.Add(terms[i]);
			functionReference.AddDetailNode(terms[i]);
		  }
		}
	}
		void Parse()
		{
			la = new Token();
			la.Value = "";
			Get();
	  		Parser();
	  if (SetTokensCategory)
			while (la != null && la.Type != 0)
			  Get();
	  Expect(0);
		}
		protected override bool[,] CreateSetArray()
		{
			bool[,] set =
			{
	  		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,T,x,x, x,x,x,T, T,x,T,T, x,T,T,x, T,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,T, T,x,T,T, x,T,T,x, T,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, T,x,T,T, x,x,T,x, T,T,x,T, x,T,T,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,T, T,T,T,T, T,T,T,x, x,x,x}
			};
			return set;
		}
	} 
	public class CssParserErrors : ParserErrorsBase
	{
		protected override string GetSyntaxErrorText(int n)
		{
			string s;
			switch (n)
			{
					case 0: s = "EOF expected"; break;
			case 1: s = "AT expected"; break;
			case 2: s = "CDO expected"; break;
			case 3: s = "CDC expected"; break;
			case 4: s = "STRING expected"; break;
			case 5: s = "SEMICOLON expected"; break;
			case 6: s = "URI expected"; break;
			case 7: s = "COMMA expected"; break;
			case 8: s = "CURLYOPEN expected"; break;
			case 9: s = "CURLYCLOSE expected"; break;
			case 10: s = "IDENT expected"; break;
			case 11: s = "COLON expected"; break;
			case 12: s = "SLASH expected"; break;
			case 13: s = "PLUS expected"; break;
			case 14: s = "GREATER expected"; break;
			case 15: s = "MINUS expected"; break;
			case 16: s = "TILDE expected"; break;
			case 17: s = "HASH expected"; break;
			case 18: s = "DOT expected"; break;
			case 19: s = "STAR expected"; break;
			case 20: s = "BRACKETOPEN expected"; break;
			case 21: s = "EQUALSIGN expected"; break;
			case 22: s = "INCLUDES expected"; break;
			case 23: s = "DASHMATCH expected"; break;
			case 24: s = "BEGINSWITH expected"; break;
			case 25: s = "ENDSWITH expected"; break;
			case 26: s = "CONTAINS expected"; break;
			case 27: s = "BRACKETCLOSE expected"; break;
			case 28: s = "FUNCTION expected"; break;
			case 29: s = "PARENOPEN expected"; break;
			case 30: s = "PARENCLOSE expected"; break;
			case 31: s = "IMPORTANTSYM expected"; break;
			case 32: s = "NOT expected"; break;
			case 33: s = "ONLY expected"; break;
			case 34: s = "AND expected"; break;
			case 35: s = "NUMBER expected"; break;
			case 36: s = "PERCENTAGE expected"; break;
			case 37: s = "LENGTH expected"; break;
			case 38: s = "EMS expected"; break;
			case 39: s = "EXS expected"; break;
			case 40: s = "ANGLE expected"; break;
			case 41: s = "TIME expected"; break;
			case 42: s = "FREQ expected"; break;
			case 43: s = "SINGLELINECOMMENT expected"; break;
			case 44: s = "MULTILINECOMMENT expected"; break;
			case 45: s = "??? expected"; break;
			case 46: s = "invalid HtmlCommentTags"; break;
			case 47: s = "invalid Import"; break;
			case 48: s = "invalid MediaQuery"; break;
			case 49: s = "invalid Term"; break;
			case 50: s = "invalid Term"; break;
			case 51: s = "invalid Term"; break;
			case 52: s = "invalid Term"; break;
			case 53: s = "invalid Declaration"; break;
			case 54: s = "invalid Combinator"; break;
			case 55: s = "invalid ElementName"; break;
			case 56: s = "invalid Attrib"; break;
			case 57: s = "invalid Pseudo"; break;
			case 58: s = "invalid Pseudo"; break;
			case 59: s = "invalid Pseudo"; break;
				default:
					s = "error " + n;
					break;
			}
			return s;
		}
	}
}
