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
namespace DevExpress.CodeRush.StructuralParser.Html
#else
namespace DevExpress.CodeParser.Html
#endif
{
	using Xml;
	public  class Tokens
	{
		public const int NOTATION = 42;
		public const int CDATATOKEN = 35;
		public const int EMPTYTAGCLOSE = 48;
		public const int NDATA = 46;
		public const int TAGCLOSESTART = 49;
		public const int CHARREF = 4;
		public const int TAGCLOSE = 18;
		public const int RAZORMODEL = 60;
		public const int BRACKETOPEN = 19;
		public const int NAME = 1;
		public const int EMPTY = 21;
		public const int BRACKETCLOSE = 20;
		public const int DECL = 15;
		public const int QUESTION = 30;
		public const int DOCTYPE = 16;
		public const int ASPSCRIPTTAGSTART = 10;
		public const int PLUS = 32;
		public const int PCDATA = 25;
		public const int SEMICOLON = 28;
		public const int IMPLIED = 44;
		public const int TAGOPEN = 47;
		public const int PARENOPEN = 23;
		public const int FIXED = 45;
		public const int ENTITY = 38;
		public const int SINGLEQUOTE = 6;
		public const int RAZORSTARTCHAR = 56;
		public const int QUESTTAGOPEN = 8;
		public const int COMMENT = 2;
		public const int CDATAEND = 52;
		public const int AMPERSAND = 50;
		public const int STAR = 31;
		public const int EQUALSTOKEN = 5;
		public const int SYSTEM = 13;
		public const int ID = 34;
		public const int QUOTEDLITERAL = 57;
		public const int UNQUOTEDATTRIBUTEVALUE = 55;
		public const int RAZORINHERITS = 61;
		public const int SERVERCOMMENT = 3;
		public const int RAZORSTARTCHARCOLON = 58;
		public const int PICHARS = 54;
		public const int PERCENTSYMBOL = 27;
		public const int IDREF = 36;
		public const int BITWISEOR = 26;
		public const int DOUBLEQUOTE = 7;
		public const int RAZORSECTION = 62;
		public const int ELEMENTDECL = 17;
		public const int PARENCLOSE = 24;
		public const int RAZORFUNCTIONS = 64;
		public const int ANYCONTENT = 22;
		public const int QUESTTAGCLOSE = 9;
		public const int NMTOKEN = 40;
		public const int PUBLIC = 14;
		public const int ENTITIES = 39;
		public const int IDREFS = 37;
		public const int CDATA = 53;
		public const int ASPSCRIPTTAGCLOSE = 11;
		public const int NMTOKENS = 41;
		public const int COMMA = 29;
		public const int RAZORHELPER = 63;
		public const int ASPDIRECTIVETAGSTART = 12;
		public const int CDATASTART = 51;
		public const int RAZORCOMMENT = 59;
		public const int EOF = 0;
		public const int REQUIRED = 43;
		public const int ATTLISTDECL = 33;
		public const int MaxTokens = 65;
		public static int[] Keywords = {
		};
	}
	partial class HtmlParser
	{
		protected override void HandlePragmas()
		{
		}
			void Parser()
	{
		while (la != null && la.Type != Tokens.EOF)
		{
		  Token startToken = la;
		ParserCore();
		if (la == startToken)
		 Get();
		}
	}
	void ParserCore()
	{
		while (!(StartOf(1)))
		{
			SynErr(66);
			Get();
		}
		while (la.Type == Tokens.COMMENT  || la.Type == Tokens.SERVERCOMMENT  || la.Type == Tokens.RAZORCOMMENT )
		{
			Comment();
		}
		while (StartOf(2))
		{
			if (StartOf(3))
			{
				Element();
			}
			else
			{
				HtmlText();
			}
		}
		HtmlScanner.ShouldCheckForXmlText = false;
		Prolog();
		HtmlScanner.ShouldCheckForXmlText = false;
		while (StartOf(4))
		{
			if (StartOf(3))
			{
				Element();
			}
			else if (la.Type == Tokens.TAGCLOSESTART )
			{
				HtmlScanner.ShouldCheckForXmlText = false;
				Get();
				Name();
				SetCategoryIfNeeded(TokenCategory.HtmlElementName);
				if (Context != null)
				   Context.HasErrors = true;
				Expect(Tokens.TAGCLOSE );
			}
			else
			{
				HtmlText();
				Comment();
			}
		}
		while (StartOf(5))
		{
			Misc();
		}
	}
	void Comment()
	{
		HtmlScanner.ShouldCheckForXmlText = false;
		Comment comment = new Comment();
		comment.CommentType = CommentType.MultiLine;
		AddComment(comment);
		HtmlScanner.ShouldCheckForXmlText = true;
		if (la.Type == Tokens.COMMENT )
		{
			Get();
		}
		else if (la.Type == Tokens.SERVERCOMMENT )
		{
			Get();
		}
		else if (la.Type == Tokens.RAZORCOMMENT )
		{
			Get();
		}
		else
			SynErr(67);
		comment.SetRange(tToken.Range);comment.Name = tToken.Value;
	}
	void Element()
	{
		if (la.Type == Tokens.EOF)
		return;
		HtmlScanner.ShouldCheckForXmlText = false;
		HtmlElement htmlElement;
		SourceRange contentRange = SourceRange.Empty;
		AspCodeEmbedding codeEmbedding = null;
		XmlProcessingInstruction processingInstruction = null;
		bool shouldReturn = false;
		switch (la.Type)
		{
		case Tokens.TAGOPEN : 
		{
			ElementStart(out htmlElement, out shouldReturn);
			if (TopLevelReturnCount > 0 || ShouldPreventCycling())
			{
				TopLevelReturnCount--;
				ElementNestingLevel--;
				return;
			}
			if (shouldReturn)
				return;
			while (StartOf(6))
			{
				switch (la.Type)
				{
				case Tokens.ASPSCRIPTTAGSTART : 
case Tokens.ASPDIRECTIVETAGSTART : 
case Tokens.TAGOPEN : 
case Tokens.RAZORSTARTCHAR : 
case Tokens.RAZORSTARTCHARCOLON : 
case Tokens.RAZORMODEL : 
case Tokens.RAZORINHERITS : 
case Tokens.RAZORSECTION : 
case Tokens.RAZORHELPER : 
case Tokens.RAZORFUNCTIONS : 
				{
					Element();
					break;
				}
				case Tokens.CHARREF : 
case Tokens.AMPERSAND : 
				{
					Reference();
					break;
				}
				case Tokens.CDATASTART : 
				{
					CDSect();
					break;
				}
				case Tokens.QUESTTAGOPEN : 
				{
					PI(out processingInstruction, true);
					if (processingInstruction != null)
					AddNode(processingInstruction);
					break;
				}
				case Tokens.COMMENT : 
case Tokens.SERVERCOMMENT : 
case Tokens.RAZORCOMMENT : 
				{
					Comment();
					break;
				}
				case Tokens.CDATA : 
				{
					HtmlText();
					break;
				}
				}
			}
			if (Context == htmlElement)
			htmlElement.InnerRange = GetRange(htmlElement.InnerRange, new SourceRange(la.Range.Start.Line, la.Range.Start.Offset));
			HtmlScanner.ShouldCheckForXmlText = false; 
			if (la.Type != Tokens.TAGCLOSESTART)
				HtmlScanner.ShouldCheckForXmlText = true;
			if (la.Type == Tokens.TAGCLOSESTART )
			{
				ElementEnd(htmlElement, out shouldReturn);
				if (shouldReturn)
				return;
			}
			break;
		}
		case Tokens.ASPDIRECTIVETAGSTART : 
		{
			AspDirectiveDef();
			break;
		}
		case Tokens.ASPSCRIPTTAGSTART : 
		{
			AspCodeEmbedding(out codeEmbedding);
			if (codeEmbedding != null)
			AddNode(codeEmbedding);
			break;
		}
		case Tokens.RAZORSTARTCHAR : 
		{
			RazorEmbedding();
			break;
		}
		case Tokens.RAZORSTARTCHARCOLON : 
		{
			RazorText();
			break;
		}
		case Tokens.RAZORMODEL : 
		{
			RazorModel();
			break;
		}
		case Tokens.RAZORINHERITS : 
		{
			RazorInherits();
			break;
		}
		case Tokens.RAZORSECTION : 
		{
			RazorSection();
			break;
		}
		case Tokens.RAZORHELPER : 
		{
			RazorHelper();
			break;
		}
		case Tokens.RAZORFUNCTIONS : 
		{
			RazorFunctions();
			break;
		}
		default: SynErr(68); break;
		}
	}
	void HtmlText()
	{
		HtmlScanner.ShouldCheckForXmlText = false;
		Expect(Tokens.CDATA );
		if (tToken.Value.Length > 0)
		{
			HtmlText htmlText = new HtmlText();
			htmlText.Name = tToken.Value;
			htmlText.Text = tToken.Value;
			AddNode(htmlText);
			htmlText.SetRange(tToken.Range);
		}
	}
	void Prolog()
	{
		XmlProcessingInstruction processingInstruction = null;
		while (StartOf(7))
		{
			if (la.Type == Tokens.QUESTTAGOPEN )
			{
				PI(out  processingInstruction, false);
				if (processingInstruction != null)
				{
					AddNode(processingInstruction);
				}
			}
			else if (la.Type == Tokens.DECL )
			{
				DocTypeDecl();
			}
			else
			{
				Misc();
			}
		}
	}
	void Name()
	{
		switch (la.Type)
		{
		case Tokens.NAME : 
		{
			Get();
			break;
		}
		case Tokens.DOCTYPE : 
		{
			Get();
			break;
		}
		case Tokens.ELEMENTDECL : 
		{
			Get();
			break;
		}
		case Tokens.ATTLISTDECL : 
		{
			Get();
			break;
		}
		case Tokens.SYSTEM : 
		{
			Get();
			break;
		}
		case Tokens.PUBLIC : 
		{
			Get();
			break;
		}
		case Tokens.EMPTY : 
		{
			Get();
			break;
		}
		case Tokens.ANYCONTENT : 
		{
			Get();
			break;
		}
		case Tokens.CDATATOKEN : 
		{
			Get();
			break;
		}
		case Tokens.IDREF : 
		{
			Get();
			break;
		}
		case Tokens.IDREFS : 
		{
			Get();
			break;
		}
		case Tokens.ENTITY : 
		{
			Get();
			break;
		}
		case Tokens.ENTITIES : 
		{
			Get();
			break;
		}
		case Tokens.NMTOKEN : 
		{
			Get();
			break;
		}
		case Tokens.NMTOKENS : 
		{
			Get();
			break;
		}
		case Tokens.NOTATION : 
		{
			Get();
			break;
		}
		case Tokens.NDATA : 
		{
			Get();
			break;
		}
		case Tokens.ID : 
		{
			Get();
			break;
		}
		default: SynErr(69); break;
		}
	}
	void Misc()
	{
		XmlProcessingInstruction processingInstruction = null;
		if (la.Type == Tokens.COMMENT  || la.Type == Tokens.SERVERCOMMENT  || la.Type == Tokens.RAZORCOMMENT )
		{
			Comment();
		}
		else if (la.Type == Tokens.QUESTTAGOPEN )
		{
			PI(out processingInstruction, false);
			if (processingInstruction != null)
			AddNode(processingInstruction);
		}
		else
			SynErr(70);
	}
	void PI(out XmlProcessingInstruction processingInstruction, bool insideElementContent)
	{
		processingInstruction = new XmlProcessingInstruction();
		HtmlScanner.ShouldCheckForXmlText = false;
		processingInstruction.SetRange(la.Range);
		bool hasAttributes	= false;
		Expect(Tokens.QUESTTAGOPEN );
		hasAttributes = la.Value == "xml";
		HtmlScanner.ShouldReturnPIChars = !hasAttributes;
		Name();
		processingInstruction.Name = tToken.Value;
		SetCategoryIfNeeded(TokenCategory.HtmlElementName);
		if (hasAttributes)
		{
		  HtmlAttribute attribute = null;	  
		while (StartOf(8))
		{
			Attribute(out attribute);
			if (attribute != null)
			{
			  processingInstruction.AddDetailNode(attribute);		  
			}
		}
		}
		else
		{
		Expect(Tokens.PICHARS );
		processingInstruction.InstructionText = tToken.Value;
		HtmlScanner.ShouldCheckForXmlText = insideElementContent;
		}
		Expect(Tokens.QUESTTAGCLOSE );
		processingInstruction.SetRange(GetRange(processingInstruction, tToken));
	}
	void DocTypeDecl()
	{
		XmlDocTypeDecl docTypeDecl = new XmlDocTypeDecl();
		AddNode(docTypeDecl);
		docTypeDecl.SetRange(la.Range);
		NewExternalIDLink extID = null;
		HtmlScanner.ShouldCheckForXmlText = false;
		Expect(Tokens.DECL );
		if (la.Type == Tokens.DOCTYPE )
		{
			Get();
		}
		else if (StartOf(8))
		{
			Name();
		}
		else
			SynErr(71);
		SetCategoryIfNeeded(TokenCategory.HtmlElementName); 
		Name();
		SetCategoryIfNeeded(TokenCategory.HtmlAttributeName);
		docTypeDecl.Name = tToken.Value;
		if (la.Type == Tokens.SYSTEM  || la.Type == Tokens.PUBLIC )
		{
			ExternalID(out extID);
			if (extID != null) docTypeDecl.AddDetailNode(extID);
		}
		if (la.Type == Tokens.BRACKETOPEN )
		{
			Get();
			IntSubset(docTypeDecl);
			Expect(Tokens.BRACKETCLOSE );
		}
		Expect(Tokens.TAGCLOSE );
		docTypeDecl.SetRange(GetRange(docTypeDecl, tToken));
	}
	void ExternalID(out NewExternalIDLink extID)
	{
		extID = null;
		SourceRange startRange = la.Range;
		if (la.Type == Tokens.SYSTEM )
		{
			Get();
			Token stringToken = null;
			QuotedLiteral(out stringToken);
			NewExternalIDSystemLink result = new NewExternalIDSystemLink();
			result.SystemURI = stringToken.Value;
			extID = result;
		}
		else if (la.Type == Tokens.PUBLIC )
		{
			Get();
			SetCategoryIfNeeded(TokenCategory.HtmlAttributeName);
			NewExternalIDPublicLink result = new NewExternalIDPublicLink();
			extID = result;
			Token stringToken = null;
			QuotedLiteral(out stringToken);
			result.PublicID = stringToken.Value; SetCategoryIfNeeded(TokenCategory.HtmlAttributeValue);
			if (la.Type == Tokens.SINGLEQUOTE  || la.Type == Tokens.DOUBLEQUOTE )
			{
				QuotedLiteral(out stringToken);
				result.SystemURI = stringToken.Value; SetCategoryIfNeeded(TokenCategory.HtmlAttributeValue);
			}
		}
		else
			SynErr(72);
		if (extID != null)
		extID.SetRange(GetRange(startRange, tToken));
	}
	void IntSubset(XmlDocTypeDecl docTypeDecl)
	{
		while (StartOf(9))
		{
			if (StartOf(7))
			{
				MarkupDecl(docTypeDecl);
			}
			else
			{
				DeclSep(docTypeDecl);
			}
		}
	}
	void MarkupDecl(XmlDocTypeDecl docTypeDecl)
	{
		XmlElementDecl elementDecl = null;
		XmlNotationDecl notationDecl = null;
		XmlAttributeListDeclaration attListDecl = null;
		XmlEntityDecl entityDecl = null;
		XmlProcessingInstruction processingInstruction = null;
		if (la.Type == Tokens.DECL )
		{
			Get();
			if (la.Type == Tokens.ELEMENTDECL )
			{
				ElementDecl(out elementDecl);
				if (docTypeDecl != null && elementDecl != null)
				docTypeDecl.AddNode(elementDecl);
			}
			else if (la.Type == Tokens.ATTLISTDECL )
			{
				AttlistDecl(out attListDecl);
				if (docTypeDecl != null && attListDecl != null)
				docTypeDecl.AddNode(attListDecl);
			}
			else if (la.Type == Tokens.ENTITY )
			{
				EntityDecl(out entityDecl);
				if (docTypeDecl != null && entityDecl != null)
				docTypeDecl.AddNode(entityDecl);
			}
			else if (la.Type == Tokens.NOTATION )
			{
				NotationDecl(out notationDecl);
				if (docTypeDecl != null && notationDecl != null)
				docTypeDecl.AddNode(notationDecl);
			}
			else
				SynErr(73);
		}
		else if (la.Type == Tokens.QUESTTAGOPEN )
		{
			PI(out  processingInstruction, false);
			if (docTypeDecl != null && processingInstruction != null)
			docTypeDecl.AddNode(processingInstruction);
		}
		else if (la.Type == Tokens.COMMENT  || la.Type == Tokens.SERVERCOMMENT  || la.Type == Tokens.RAZORCOMMENT )
		{
			Comment();
		}
		else
			SynErr(74);
	}
	void DeclSep(XmlDocTypeDecl docTypeDecl)
	{
		XmlNameReference nameRef;
		PEReference(out nameRef);
		if (nameRef != null && docTypeDecl != null)
		docTypeDecl.AddNode(nameRef);
	}
	void ElementDecl(out XmlElementDecl elementDecl)
	{
		elementDecl = new XmlElementDecl();
		elementDecl.SetRange(tToken.Range);
		XmlBaseContentSpec contentSpec = null;
		Expect(Tokens.ELEMENTDECL );
		Name();
		elementDecl.Name = tToken.Value;
		ContentSpec(out contentSpec);
		if (contentSpec != null)
		elementDecl.AddDetailNode(contentSpec);
		Expect(Tokens.TAGCLOSE );
		elementDecl.SetRange(GetRange(elementDecl, tToken));
	}
	void AttlistDecl(out XmlAttributeListDeclaration result)
	{
		result = new XmlAttributeListDeclaration();
		XmlAttributeDeclaration attrDecl = null;
		result.SetRange(tToken.Range);
		Expect(Tokens.ATTLISTDECL );
		Name();
		result.Name = tToken.Value; 
		while (StartOf(8))
		{
			AttDef(out attrDecl);
			if (attrDecl != null)
			{
				result.AddNode(attrDecl);
				result.AttributesDecl.Add(attrDecl);
			}
		}
		Expect(Tokens.TAGCLOSE );
		result.SetRange(GetRange(result, tToken));
	}
	void EntityDecl(out XmlEntityDecl entityDecl)
	{
		entityDecl = new XmlEntityDecl();
		entityDecl.SetRange(tToken.Range);
		Expect(Tokens.ENTITY );
		if (la.Type == Tokens.PERCENTSYMBOL )
		{
			Get();
			entityDecl.IsParameterEntity = true;
		}
		Name();
		entityDecl.Name = tToken.Value;
		EntityDef(entityDecl);
		Expect(Tokens.TAGCLOSE );
		entityDecl.SetRange(GetRange(entityDecl, tToken));
	}
	void NotationDecl(out XmlNotationDecl notationDecl)
	{
		NewExternalIDLink extID = null;
		notationDecl = new XmlNotationDecl();
		notationDecl.SetRange(tToken.Range);
		Expect(Tokens.NOTATION );
		Name();
		notationDecl.Name = tToken.Value;
		ExternalID(out extID);
		if (extID != null) 
		{
			notationDecl.NotationLink = extID; 
			notationDecl.AddDetailNode(extID);
		}
		Expect(Tokens.TAGCLOSE );
		notationDecl.SetRange(GetRange(notationDecl, tToken));
	}
	void PEReference(out XmlNameReference nameRef)
	{
		nameRef = new XmlNameReference();
		nameRef.SetRange(la.Range);
		Expect(Tokens.PERCENTSYMBOL );
		Name();
		nameRef.Name = tToken.Value; 
		Expect(Tokens.SEMICOLON );
		nameRef.SetRange(GetRange(nameRef, tToken));
	}
	void ContentSpec(out XmlBaseContentSpec contentSpec)
	{
		contentSpec = null;
		XmlMixedContentSpec mixedContent = null;
		XmlChildrenContentSpec childContent = null;
		SourceRange startRange = la.Range;
		if (la.Type == Tokens.EMPTY )
		{
			Get();
			contentSpec = new XmlEmptyContentSpec();	
		}
		else if (la.Type == Tokens.ANYCONTENT )
		{
			Get();
			contentSpec = new XmlAnyContentSpec();	
		}
		else if (la.Type == Tokens.PARENOPEN )
		{
			Get();
			if (la.Type == Tokens.PCDATA )
			{
				Mixed(out mixedContent);
				contentSpec = mixedContent;
			}
			else if (StartOf(10))
			{
				Children(out childContent);
				contentSpec = childContent;
			}
			else
				SynErr(75);
			if (contentSpec != null)
			contentSpec.SetRange(GetRange(startRange, tToken));
		}
		else
			SynErr(76);
	}
	void Mixed(out XmlMixedContentSpec contentSpec)
	{
		contentSpec = new XmlMixedContentSpec();
		XmlNameReference nameRef;
		Expect(Tokens.PCDATA );
		while (la.Type == Tokens.BITWISEOR )
		{
			Get();
			if (StartOf(8))
			{
				Name();
				XmlName name = new XmlName();
				name.Name = tToken.Value;
				name.SetRange(GetRange(tToken));
				contentSpec.Names.Add(name);
				contentSpec.AddDetailNode(name);
			}
			else if (la.Type == Tokens.PERCENTSYMBOL )
			{
				PEReference(out nameRef);
				if (nameRef != null)
				{
					contentSpec.Names.Add(nameRef);
					contentSpec.AddDetailNode(nameRef);
				}
			}
			else
				SynErr(77);
		}
		Expect(Tokens.PARENCLOSE );
		if (la.Type == Tokens.STAR )
		{
			Get();
		}
	}
	void Children(out XmlChildrenContentSpec children)
	{
		XmlContentParticle contentParticle = null;
		children = new XmlChildrenContentSpec();
		RepeatCount repCount = RepeatCount.Once;
		CpSequence(out contentParticle);
		if (la.Type == Tokens.QUESTION  || la.Type == Tokens.STAR  || la.Type == Tokens.PLUS )
		{
			if (la.Type == Tokens.QUESTION )
			{
				Get();
				repCount = RepeatCount.ZeroOrOnce;
			}
			else if (la.Type == Tokens.STAR )
			{
				Get();
				repCount = RepeatCount.ZeroOrMore;
			}
			else
			{
				Get();
				repCount = RepeatCount.OnceOrMore;
			}
		}
		if (contentParticle != null)
		{
			contentParticle.RepeatCount = repCount;
			children.Source = contentParticle;
			children.AddDetailNode(contentParticle);
		}
	}
	void CpSequence(out XmlContentParticle choiceCP)
	{
		choiceCP = null;
		XmlContentParticle currentCP = null;
		XmlSequencedContentParticle result = null;
		Cp(out currentCP);
		while (la.Type == Tokens.BITWISEOR  || la.Type == Tokens.COMMA )
		{
			if (la.Type == Tokens.BITWISEOR )
			{
				Get();
				if (result == null)
				{
					result = new XmlChoiceContentParticle();
					if (currentCP != null)
					{
						result.Particles.Add(currentCP);
						result.AddDetailNode(currentCP);
					}
				}
			}
			else
			{
				Get();
				if (result == null)
				{
					result = new XmlSequenceContentParticle();
					if (currentCP != null)
					{
						result.Particles.Add(currentCP);
						result.AddDetailNode(currentCP);
					}
				}
			}
			Cp(out currentCP);
			if (currentCP != null)
			{
				result.Particles.Add(currentCP);
				result.AddDetailNode(currentCP);
			}
		}
		Expect(Tokens.PARENCLOSE );
		choiceCP = result;
	}
	void Cp(out XmlContentParticle contentParticle)
	{
		XmlNameReference nameRef = null;
		contentParticle = null;
		RepeatCount repCount = RepeatCount.Once;
		SourceRange startRange = la.Range;
		if (StartOf(8))
		{
			Name();
			XmlName name = new XmlName();
			name.Name = tToken.Value;
			contentParticle = new XmlNamedContentParticle();
			(contentParticle as XmlNamedContentParticle).ParticleName = name;
			contentParticle.AddDetailNode(name);
		}
		else if (la.Type == Tokens.PERCENTSYMBOL )
		{
			PEReference(out nameRef);
			if (nameRef != null)
			{
				contentParticle = new XmlNamedContentParticle();
				(contentParticle as XmlNamedContentParticle).ParticleName = nameRef;
				contentParticle.AddDetailNode(nameRef);
			}
		}
		else if (la.Type == Tokens.PARENOPEN )
		{
			Get();
			CpSequence(out contentParticle);
		}
		else
			SynErr(78);
		if (la.Type == Tokens.QUESTION  || la.Type == Tokens.STAR  || la.Type == Tokens.PLUS )
		{
			if (la.Type == Tokens.QUESTION )
			{
				Get();
				repCount = RepeatCount.ZeroOrOnce;
			}
			else if (la.Type == Tokens.STAR )
			{
				Get();
				repCount = RepeatCount.ZeroOrMore;
			}
			else
			{
				Get();
				repCount = RepeatCount.OnceOrMore;
			}
		}
		if (contentParticle != null)
		{
			contentParticle.RepeatCount = repCount;
			contentParticle.SetRange(GetRange(startRange, tToken));
		}
	}
	void AttDef(out XmlAttributeDeclaration attrDecl)
	{
		attrDecl = new XmlAttributeDeclaration();
		attrDecl.SetRange(la.Range);
		Name();
		attrDecl.Name = tToken.Value;
		AttType(attrDecl);
		DefaultDecl(attrDecl);
		attrDecl.SetRange(GetRange(attrDecl, tToken));
	}
	void AttType(XmlAttributeDeclaration attrDecl)
	{
		if (StartOf(11))
		{
			TokenizedType(attrDecl);
		}
		else if (la.Type == Tokens.PARENOPEN  || la.Type == Tokens.NOTATION )
		{
			Enumeration(attrDecl);
		}
		else
			SynErr(79);
	}
	void DefaultDecl(XmlAttributeDeclaration attrDecl)
	{
		if (la.Type == Tokens.REQUIRED )
		{
			Get();
			attrDecl.DefaultAttributeValueType = DefaultAttributeValueType.Required;
		}
		else if (la.Type == Tokens.IMPLIED )
		{
			Get();
			attrDecl.DefaultAttributeValueType = DefaultAttributeValueType.Implied;
		}
		else if (la.Type == Tokens.SINGLEQUOTE  || la.Type == Tokens.DOUBLEQUOTE  || la.Type == Tokens.FIXED )
		{
			if (la.Type == Tokens.FIXED )
			{
				Get();
			}
			Token stringToken = null;
			QuotedLiteral(out stringToken);
			attrDecl.DefaultAttributeValueType = DefaultAttributeValueType.Fixed;
			attrDecl.DefaultAttributeValue = stringToken.Value;
		}
		else
			SynErr(80);
	}
	void TokenizedType(XmlAttributeDeclaration attrDecl)
	{
		switch (la.Type)
		{
		case Tokens.ID : 
		{
			Get();
			attrDecl.AttributeType = AttributeType.Id;
			break;
		}
		case Tokens.CDATATOKEN : 
		{
			Get();
			attrDecl.AttributeType = AttributeType.CData;
			break;
		}
		case Tokens.IDREF : 
		{
			Get();
			attrDecl.AttributeType = AttributeType.IdRef;
			break;
		}
		case Tokens.IDREFS : 
		{
			Get();
			attrDecl.AttributeType = AttributeType.IdRefs;
			break;
		}
		case Tokens.ENTITY : 
		{
			Get();
			attrDecl.AttributeType = AttributeType.Entity;
			break;
		}
		case Tokens.ENTITIES : 
		{
			Get();
			attrDecl.AttributeType = AttributeType.Entities;
			break;
		}
		case Tokens.NMTOKEN : 
		{
			Get();
			attrDecl.AttributeType = AttributeType.NmToken;
			break;
		}
		case Tokens.NMTOKENS : 
		{
			Get();
			attrDecl.AttributeType = AttributeType.NmTokens;
			break;
		}
		default: SynErr(81); break;
		}
	}
	void Enumeration(XmlAttributeDeclaration attrDecl)
	{
		attrDecl.AttributeType = AttributeType.Enumeration;
		if (la.Type == Tokens.NOTATION )
		{
			Get();
			attrDecl.AttributeType = AttributeType.Notation;
		}
		Expect(Tokens.PARENOPEN );
		Name();
		attrDecl.EnumerationMembers.Add(tToken.Value);
		while (la.Type == Tokens.BITWISEOR )
		{
			Get();
			Name();
			attrDecl.EnumerationMembers.Add(tToken.Value);
		}
		Expect(Tokens.PARENCLOSE );
	}
	void QuotedLiteral(out Token stringToken)
	{
		stringToken = HtmlScanner.ReadStringLiteral(la.Type, this);
		if (la.Type == Tokens.SINGLEQUOTE )
		{
			Get();
		}
		else if (la.Type == Tokens.DOUBLEQUOTE )
		{
			Get();
		}
		else
			SynErr(82);
		if(tToken.Type == Tokens.SINGLEQUOTE || tToken.Type == Tokens.DOUBLEQUOTE)
		 {
		  tToken = ConcatTokens(tToken, stringToken, Tokens.QUOTEDLITERAL, tToken.Value + stringToken.Value);
		}
		   stringToken = tToken;
	}
	void EntityDef(XmlEntityDecl entityDecl)
	{
		NewExternalIDLink extID = null;
		if (la.Type == Tokens.SINGLEQUOTE  || la.Type == Tokens.DOUBLEQUOTE )
		{
			Token stringToken = null;
			QuotedLiteral(out stringToken);
			if (entityDecl != null)
			entityDecl.ImmediateValue = stringToken.Value;
		}
		else if (la.Type == Tokens.SYSTEM  || la.Type == Tokens.PUBLIC )
		{
			ExternalID(out extID);
			if (entityDecl != null && extID != null)
			{
				entityDecl.ExternalLinkValue = extID;
				entityDecl.AddDetailNode(extID);
			}
			if (la.Type == Tokens.NDATA )
			{
				Get();
				Name();
				if (entityDecl != null)
				entityDecl.NDataValue = tToken.Value;
			}
		}
		else
			SynErr(83);
	}
	void Attribute(out HtmlAttribute attribute)
	{
		attribute = null;
		InsideAttribute = true;
		AttributeCore(out attribute);
		InsideAttribute = false;
	}
	void CDSect()
	{
		XmlCharacterData cData = new XmlCharacterData();
		AddNode(cData);
		HtmlScanner.ShouldCheckForXmlText = false;
		cData.SetRange(la.Range);
		HtmlScanner.ShouldReturnCharDataToken = true;
		Expect(Tokens.CDATASTART );
		Expect(Tokens.CDATA );
		cData.Name = tToken.Value;HtmlScanner.ShouldCheckForXmlText = true; 
		Expect(Tokens.CDATAEND );
		cData.SetRange(GetRange(cData, tToken));
	}
	void AspDirectiveDef()
	{
		SplitAspDirectiveTagStartIfNeeded();
		Expect(Tokens.ASPDIRECTIVETAGSTART );
		AspDirective directive = null;
		if (String.Compare(la.Value, "Page", StringComparison.CurrentCultureIgnoreCase) == 0)
		{
			directive = new PageDirective();
		}
		else
		  if (String.Compare(la.Value, "Register", StringComparison.CurrentCultureIgnoreCase) == 0)
		  {
			directive = new RegisterDirective();
		  }
		else
		  if (String.Compare(la.Value, "Control", StringComparison.CurrentCultureIgnoreCase) == 0)
		  {
			directive = new ControlDirective();
		  }
		 else
		  if (String.Compare(la.Value, "Master", StringComparison.CurrentCultureIgnoreCase) == 0)
		  {
			directive = new MasterDirective();
		  }
		else
		  if (String.Compare(la.Value, "Import", StringComparison.CurrentCultureIgnoreCase) == 0)
		  {
			directive = new AspImportDirective();
		  }
		else
			directive = new AspDirective();
		directive.SetRange(tToken.Range);
		HtmlAttribute attribute = null;
		AddNode(directive);
		Name();
		SetCategoryIfNeeded(TokenCategory.HtmlElementName);
		directive.Name = tToken.Value;
		directive.NameRange = tToken.Range;
		while (StartOf(8))
		{
			Attribute(out attribute);
			if (attribute != null)
			{
				directive.AddDetailNode(attribute);
				directive.Attributes.Add(attribute);
			}
		}
		HtmlScanner.ShouldCheckForXmlText = true;
		Expect(Tokens.ASPSCRIPTTAGCLOSE );
		directive.HasCloseTag = false;
		directive.SetRange(GetRange(directive, tToken));
		DotNetLanguageType language = GetDefaultLanguage(directive);
		SourceFile rootFile = RootNode as SourceFile;
		if (rootFile != null)
		  {
			if (directive is PageDirective || directive is ControlDirective || directive is RazorModelDirective || IsWebHandlerOrWebService(directive))
			  SetSourceFileProperties(rootFile, directive, language);
			if (directive is AspImportDirective)
				AddToUsingList(rootFile, directive as AspImportDirective);
			string masterPageFile = GetMasterPageFile(directive);
			if (!string.IsNullOrEmpty(masterPageFile))
			  SetMasterPageFile(rootFile, masterPageFile);
		  }
		while (la.Type == Tokens.COMMENT  || la.Type == Tokens.SERVERCOMMENT  || la.Type == Tokens.RAZORCOMMENT )
		{
			Comment();
		}
	}
	void AspCodeEmbedding(out AspCodeEmbedding codeEmbedding)
	{
		codeEmbedding = new AspCodeEmbedding();
		Token codeEmbeddingToken = HtmlScanner.GetCodeEmbeddingText();
		string codeEmbeddingText = String.Empty;
		if (codeEmbeddingToken != null)
		{
			codeEmbeddingText = codeEmbeddingToken.Value;
			codeEmbedding.CodeRange = codeEmbeddingToken.Range;
			codeEmbedding.CodeEmbeddingToken = codeEmbeddingToken;
		}
		if (SetTokensCategory)
				{
					if (SavedTokens != null)
						SavedTokens.Add(codeEmbeddingToken);
				}
		codeEmbedding.Code = codeEmbeddingText;
		codeEmbedding.Name = codeEmbeddingText;
		Expect(Tokens.ASPSCRIPTTAGSTART );
		if (!InsideAttribute && !AspEmbCodeIsName)
		{
			HtmlScanner.ShouldCheckForXmlText = true;
		}
		codeEmbedding.HasCloseTag = false;
		codeEmbedding.SetRange(GetRange(tToken,la));
		Expect(Tokens.ASPSCRIPTTAGCLOSE );
	}
	void RazorEmbedding()
	{
		ParseRazorEmbedding();
		if (!InsideAttribute && !AspEmbCodeIsName)
		{
		   HtmlScanner.ShouldCheckForXmlText = true;
		 }
		Expect(Tokens.RAZORSTARTCHAR );
	}
	void ElementStart(out HtmlElement htmlElement, out bool shouldReturn)
	{
		htmlElement = null;
		shouldReturn = true;
		HtmlAttribute attribute = null;
		AspCodeEmbedding codeEmbedding = null;
		SourceRange tagOpenRange = la.Range;
		SourceRange lastTagRange = SourceRange.Empty;
		if (tToken != null)
			lastTagRange = tToken.Range;
		Expect(Tokens.TAGOPEN );
		htmlElement = CreateElement(la.Value, la.Range, tToken.Range); 
		if (StartOf(8))
		{
			Name();
		}
		else if (la.Type == Tokens.ASPSCRIPTTAGSTART )
		{
			AspEmbCodeIsName = true;
			AspCodeEmbedding(out codeEmbedding);
			AspEmbCodeIsName = false;	HandleCodeEmbedding(htmlElement, codeEmbedding);
		}
		else if (la.Type == Tokens.RAZORSTARTCHAR )
		{
			RazorEmbedding();
		}
		else
			SynErr(84);
		SetContextForHtmlElement(htmlElement, lastTagRange, tagOpenRange);
		while (StartOf(8))
		{
			Attribute(out attribute);
			if (attribute != null)
			{
				htmlElement.Attributes.Add(attribute);
				htmlElement.AddDetailNode(attribute);
			}
		}
		TryToHandleScriptOrStyleSheets(htmlElement);
		if (la.Type == Tokens.TAGCLOSE )
		{
			Get();
			HandleTagClose(htmlElement, out shouldReturn);
			if (shouldReturn)
				return;
		}
		else if (la.Type == Tokens.EMPTYTAGCLOSE )
		{
			Get();
			HanldeEmptyTagClose(htmlElement);
			return;
		}
		else
			SynErr(85);
		shouldReturn = false;
	}
	void ElementEnd(HtmlElement htmlElement, out bool shouldReturn)
	{
		SourceRange tagCloseRange = SourceRange.Empty;
		shouldReturn = true;
		HandleTagCloseStart(htmlElement, out tagCloseRange, out shouldReturn);
		if (shouldReturn)
			return;
		Expect(Tokens.TAGCLOSESTART );
		if (StartOf(8))
		{
			Name();
		}
		string closeTagElementName = String.Empty;
		HandleClosingNameToken(htmlElement, tagCloseRange, out closeTagElementName);
		Expect(Tokens.TAGCLOSE );
		HandleEndingTagClose(htmlElement, tagCloseRange, out shouldReturn);
		if (shouldReturn)
			return;
		shouldReturn = false;
	}
	void Reference()
	{
		HtmlScanner.ShouldCheckForXmlText = false;
		if (la.Type == Tokens.AMPERSAND )
		{
			Get();
			XmlReference xmlReference = new XmlReference();
			AddNode(xmlReference);
			xmlReference.SetRange(tToken.Range);
			SetCategoryIfNeeded(TokenCategory.HtmlEntity);
			Name();
			xmlReference.SetRange(GetRange(xmlReference, la));
			xmlReference.Name = tToken.Value;
			HtmlScanner.ShouldCheckForXmlText = true;
			SetCategoryIfNeeded(TokenCategory.HtmlEntity);
			Expect(Tokens.SEMICOLON );
		}
		else if (la.Type == Tokens.CHARREF )
		{
			Get();
			XmlCharReference xmlCharReference = new XmlCharReference();
			AddNode(xmlCharReference);
			xmlCharReference.SetRange(GetRange(tToken, la));
			xmlCharReference.Name = tToken.Value;
			HtmlScanner.ShouldCheckForXmlText = true;
			SetCategoryIfNeeded(TokenCategory.HtmlEntity);
			Expect(Tokens.SEMICOLON );
		}
		else
			SynErr(86);
		SetCategoryIfNeeded(TokenCategory.HtmlEntity);
	}
	void RazorText()
	{
		SourceRange lineRange = SourceRange.Empty;
		string razorTextLine = HtmlScanner.ReadSingleLine(out lineRange);
		ParseRazorText(razorTextLine, lineRange);
		Expect(Tokens.RAZORSTARTCHARCOLON );
	}
	void RazorModel()
	{
		HtmlScanner.ShouldCheckForAttribute = true;
		RazorModelDirective directive = new RazorModelDirective();
		directive.SetRange(la.Range);
		SplitRazorDerectiveTokenIfNeeded();
		Expect(Tokens.RAZORMODEL );
		Expect(Tokens.UNQUOTEDATTRIBUTEVALUE );
		directive.Model = tToken.Value.Trim();
		directive.SetRange(GetRange(directive, tToken));
		directive.ModelRange = tToken.Range;
		SourceFile fileNode = Context as SourceFile;
		if (fileNode != null)
		  fileNode.SetModelTypeName(directive.Model);
		AddNode(directive);
	}
	void RazorInherits()
	{
		HtmlScanner.ShouldCheckForAttribute = true;
		RazorInheritsDirective directive = new RazorInheritsDirective();
		directive.SetRange(la.Range);
		SplitRazorDerectiveTokenIfNeeded();
		Expect(Tokens.RAZORINHERITS );
		Expect(Tokens.UNQUOTEDATTRIBUTEVALUE );
		directive.Model = tToken.Value.Trim();
		directive.SetRange(GetRange(directive, tToken));
		directive.ModelRange = tToken.Range;
		AddNode(directive);
	}
	void RazorSection()
	{
		HtmlScanner.ShouldCheckForAttribute = true;
		SplitRazorDerectiveTokenIfNeeded();
		Expect(Tokens.RAZORSECTION );
		ParseRazorEmbedding();
		Expect(Tokens.NAME );
	}
	void RazorHelper()
	{
		if (SetTokensCategory)
		{
		  CategorizedToken catToken = la as CategorizedToken;
		  catToken.Category = TokenCategory.HtmlServerSideScript;
		}
		ParseRazorHelper();
		Expect(Tokens.RAZORHELPER );
	}
	void RazorFunctions()
	{
		if (SetTokensCategory)
		{
		  CategorizedToken catToken = la as CategorizedToken;
		  catToken.Category = TokenCategory.HtmlServerSideScript;
		}
		ParseRazorFunctions();
		Expect(Tokens.RAZORFUNCTIONS );
	}
	void AttributeCore(out HtmlAttribute attribute)
	{
		attribute = new HtmlAttribute();
		attribute.SetRange(la.Range);
		AspCodeEmbedding codeEmbedding = null;
		Name();
		attribute.Name = tToken.Value;
		attribute.SetNameRange(tToken.Range);
		SetCategoryIfNeeded(TokenCategory.HtmlAttributeName);
		if (la.Type == Tokens.EQUALSTOKEN )
		{
			HtmlScanner.ShouldCheckForAttribute = true;
			Get();
			if (la.Type == Tokens.SINGLEQUOTE  || la.Type == Tokens.DOUBLEQUOTE )
			{
				Token stringToken = null;
				QuotedLiteral(out stringToken);
				if (stringToken.Value != null && stringToken.Value.Length > 0)
				{
					int startPos = 0;
					int endPos = stringToken.Value.Length;
					char firstSign = stringToken.Value[0];
					if (firstSign == '"' || firstSign == '\'')
					{
						startPos = 1;
						if (firstSign == '"')
							attribute.AttributeQuoteType = QuoteType.DoubleQuote;
						else if (firstSign == '\'')
							attribute.AttributeQuoteType = QuoteType.SingleQuote;
					}
					if (stringToken.Value[stringToken.Value.Length -1] == '"' || stringToken.Value[stringToken.Value.Length -1] == '\'')
						endPos = stringToken.Value.Length -1;
					attribute.Value = stringToken.Value.Substring(startPos, endPos - startPos);	
				}
				attribute.SetValueRange(new SourceRange(stringToken.Range.Start.Line, stringToken.Range.Start.Offset + 1, stringToken.Range.End.Line, stringToken.Range.End.Offset - 1));
				attribute.SetRange(GetRange(attribute, stringToken));
				  CheckForInlineExpression(attribute);
				CheckForEventHandlerScriptCode(attribute);
			}
			else if (la.Type == Tokens.UNQUOTEDATTRIBUTEVALUE )
			{
				Get();
				attribute.Value = tToken.Value;
				attribute.SetValueRange(tToken.Range);
				attribute.SetRange(GetRange(attribute, tToken));
			}
			else if (la.Type == Tokens.ASPSCRIPTTAGSTART )
			{
				AspCodeEmbedding(out codeEmbedding);
				if (codeEmbedding != null && attribute != null)
				{
					attribute.Value = codeEmbedding.Code;
					attribute.SetValueRange(codeEmbedding.CodeRange);
					attribute.AddDetailNode(codeEmbedding);
					attribute.SetRange(GetRange(attribute, tToken));
					attribute.ValueIsAspCodeEmbedding = true;
				}
			}
			else
				SynErr(87);
			SetCategoryIfNeeded(TokenCategory.HtmlAttributeValue);
			SplitInlineExpressionIfNeeded();
		}
		ParseAttributeStyle(attribute);		
	}
		void Parse()
		{
			la = new Token();
			if(InlineExpressions != null)
				InlineExpressions.Clear();
			la.Value = "";
			Get();
					Parser();
			Expect(0);
			if (Context != null)
				Context.SetRange(GetRange(Context, tToken));
			CloseContext();
		}
		protected override bool[,] CreateSetArray()
		{
			bool[,] set =
			{
					{T,x,T,T, x,x,x,x, T,x,T,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,T,x,x, T,x,T,T, T,T,T,T, T,x,x},
		{T,x,T,T, x,x,x,x, T,x,T,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,T,x,x, T,x,T,T, T,T,T,T, T,x,x},
		{x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,T,x, T,T,T,T, T,x,x},
		{x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,T,x, T,T,T,T, T,x,x},
		{x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,T,x,x, T,x,T,x, T,T,T,T, T,x,x},
		{x,x,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x},
		{x,x,T,T, T,x,x,x, T,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,T, x,T,x,x, T,x,T,T, T,T,T,T, T,x,x},
		{x,x,T,T, x,x,x,x, T,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, T,T,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,T,T, x,x,x,x, T,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, x,T,T,T, x,x,x,T, x,x,x,x, x,T,T,T, T,T,T,T, T,T,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x}
			};
			return set;
		}				
	}
	public class HtmlParserErrors : ParserErrorsBase
	{
		protected override string GetSyntaxErrorText(int n)
		{
			string s;
			switch (n)
			{
				case 0: s = "EOF expected"; break;
			case 1: s = "NAME expected"; break;
			case 2: s = "COMMENT expected"; break;
			case 3: s = "SERVERCOMMENT expected"; break;
			case 4: s = "CHARREF expected"; break;
			case 5: s = "EQUALS expected"; break;
			case 6: s = "SINGLEQUOTE expected"; break;
			case 7: s = "DOUBLEQUOTE expected"; break;
			case 8: s = "QUESTTAGOPEN expected"; break;
			case 9: s = "QUESTTAGCLOSE expected"; break;
			case 10: s = "ASPSCRIPTTAGSTART expected"; break;
			case 11: s = "ASPSCRIPTTAGCLOSE expected"; break;
			case 12: s = "ASPDIRECTIVETAGSTART expected"; break;
			case 13: s = "SYSTEM expected"; break;
			case 14: s = "PUBLIC expected"; break;
			case 15: s = "DECL expected"; break;
			case 16: s = "DOCTYPE expected"; break;
			case 17: s = "ELEMENTDECL expected"; break;
			case 18: s = "TAGCLOSE expected"; break;
			case 19: s = "BRACKETOPEN expected"; break;
			case 20: s = "BRACKETCLOSE expected"; break;
			case 21: s = "EMPTY expected"; break;
			case 22: s = "ANYCONTENT expected"; break;
			case 23: s = "PARENOPEN expected"; break;
			case 24: s = "PARENCLOSE expected"; break;
			case 25: s = "PCDATA expected"; break;
			case 26: s = "BITWISEOR expected"; break;
			case 27: s = "PERCENTSYMBOL expected"; break;
			case 28: s = "SEMICOLON expected"; break;
			case 29: s = "COMMA expected"; break;
			case 30: s = "QUESTION expected"; break;
			case 31: s = "STAR expected"; break;
			case 32: s = "PLUS expected"; break;
			case 33: s = "ATTLISTDECL expected"; break;
			case 34: s = "ID expected"; break;
			case 35: s = "CDATATOKEN expected"; break;
			case 36: s = "IDREF expected"; break;
			case 37: s = "IDREFS expected"; break;
			case 38: s = "ENTITY expected"; break;
			case 39: s = "ENTITIES expected"; break;
			case 40: s = "NMTOKEN expected"; break;
			case 41: s = "NMTOKENS expected"; break;
			case 42: s = "NOTATION expected"; break;
			case 43: s = "REQUIRED expected"; break;
			case 44: s = "IMPLIED expected"; break;
			case 45: s = "FIXED expected"; break;
			case 46: s = "NDATA expected"; break;
			case 47: s = "TAGOPEN expected"; break;
			case 48: s = "EMPTYTAGCLOSE expected"; break;
			case 49: s = "TAGCLOSESTART expected"; break;
			case 50: s = "AMPERSAND expected"; break;
			case 51: s = "CDATASTART expected"; break;
			case 52: s = "CDATAEND expected"; break;
			case 53: s = "CDATA expected"; break;
			case 54: s = "PICHARS expected"; break;
			case 55: s = "UNQUOTEDATTRIBUTEVALUE expected"; break;
			case 56: s = "RAZORSTARTCHAR expected"; break;
			case 57: s = "QUOTEDLITERAL expected"; break;
			case 58: s = "RAZORSTARTCHARCOLON expected"; break;
			case 59: s = "RAZORCOMMENT expected"; break;
			case 60: s = "RAZORMODEL expected"; break;
			case 61: s = "RAZORINHERITS expected"; break;
			case 62: s = "RAZORSECTION expected"; break;
			case 63: s = "RAZORHELPER expected"; break;
			case 64: s = "RAZORFUNCTIONS expected"; break;
			case 65: s = "??? expected"; break;
			case 66: s = "this symbol not expected in ParserCore"; break;
			case 67: s = "invalid Comment"; break;
			case 68: s = "invalid Element"; break;
			case 69: s = "invalid Name"; break;
			case 70: s = "invalid Misc"; break;
			case 71: s = "invalid DocTypeDecl"; break;
			case 72: s = "invalid ExternalID"; break;
			case 73: s = "invalid MarkupDecl"; break;
			case 74: s = "invalid MarkupDecl"; break;
			case 75: s = "invalid ContentSpec"; break;
			case 76: s = "invalid ContentSpec"; break;
			case 77: s = "invalid Mixed"; break;
			case 78: s = "invalid Cp"; break;
			case 79: s = "invalid AttType"; break;
			case 80: s = "invalid DefaultDecl"; break;
			case 81: s = "invalid TokenizedType"; break;
			case 82: s = "invalid QuotedLiteral"; break;
			case 83: s = "invalid EntityDef"; break;
			case 84: s = "invalid ElementStart"; break;
			case 85: s = "invalid ElementStart"; break;
			case 86: s = "invalid Reference"; break;
			case 87: s = "invalid AttributeCore"; break;
				default:
					s = "error " + n;
					break;
			}
			return s;
		}
	}
}
