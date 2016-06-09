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
using System.Collections;
using System.Collections.Specialized;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.Xml
#else
namespace DevExpress.CodeParser.Xml
#endif
{
	public class Tokens
	{
		public const int IDREF = 36;
		public const int EOF = 0;
		public const int PLUS = 31;
		public const int NMTOKENS = 41;
		public const int AMPERSAND = 51;
		public const int XMLTAGOPEN = 5;
		public const int ELEMENTDECL = 16;
		public const int IDREFS = 37;
		public const int NMTOKEN = 40;
		public const int DOCTYPE = 15;
		public const int CDATA = 54;
		public const int COMMENT = 2;
		public const int ENTITYDECL = 46;
		public const int IMPLIED = 44;
		public const int CDATASTART = 52;
		public const int PICHARS = 55;
		public const int PARENOPEN = 22;
		public const int TAGCLOSESTART = 50;
		public const int BITWISEOR = 25;
		public const int CDATATOKEN = 35;
		public const int ENCODING = 9;
		public const int SINGLEQUOTE = 7;
		public const int PARENCLOSE = 23;
		public const int ENTITY = 38;
		public const int ANYCONTENT = 21;
		public const int SYSTEM = 13;
		public const int EMPTYTAGCLOSE = 49;
		public const int CDATAEND = 53;
		public const int EQUALSTOKEN = 6;
		public const int PCDATA = 24;
		public const int COMMA = 28;
		public const int PERCENTSYMBOL = 26;
		public const int EMPTY = 20;
		public const int TAGCLOSE = 17;
		public const int SEMICOLON = 27;
		public const int NOTATIONDECl = 32;
		public const int ATTLISTDECL = 33;
		public const int QUESTTAGOPEN = 10;
		public const int PUBLIC = 14;
		public const int NOTATION = 42;
		public const int QUESTTAGCLOSE = 11;
		public const int BRACKETCLOSE = 19;
		public const int QUESTION = 29;
		public const int STAR = 30;
		public const int FIXED = 45;
		public const int STANDALONE = 12;
		public const int BRACKETOPEN = 18;
		public const int ENTITIES = 39;
		public const int REQUIRED = 43;
		public const int NDATA = 47;
		public const int VERSION = 8;
		public const int TAGOPEN = 48;
		public const int IDTOKEN = 34;
		public const int CHARREF = 4;
		public const int NAME = 1;
		public const int MaxTokens = 56;
		public static int[] Keywords = {
		};
	}
	public class NewXmlParser : XmlParserBase
	{
		public NewXmlParser() 
		{							
			parserErrors = new XmlParserErrors();		
			set = CreateSetArray();
			maxTokens = Tokens.MaxTokens;
		}
		protected override void HandlePragmas()
		{
		}			
			void Parser()
	{
		if (la.Type == Tokens.CDATA )
		{
			Text(true);
		}
		XmlScanner.ShouldCheckForXmlText = false;
		Prolog();
		if (la.Type == Tokens.TAGOPEN )
		{
			Element();
		}
		else if (la.Type == Tokens.CDATA )
		{
			Text(true);
		}
		else
			SynErr(57);
		XmlScanner.ShouldCheckForXmlText = false;
		while (la.Type == Tokens.TAGOPEN  || la.Type == Tokens.CDATA )
		{
			if (la.Type == Tokens.TAGOPEN )
			{
				Element();
			}
			else
			{
				Text(true);
			}
			XmlScanner.ShouldCheckForXmlText = false;
		}
		if (la.Type == Tokens.CDATA )
		{
			Get();
		}
		while (StartOf(1))
		{
			if (la.Type == Tokens.TAGOPEN  || la.Type == Tokens.CDATA )
			{
				XmlScanner.ShouldCheckForXmlText = true;
				if (la.Type == Tokens.TAGOPEN )
				{
					Element();
				}
				else if (la.Type == Tokens.CDATA )
				{
					Text(true);
				}
				else
					SynErr(58);
				XmlScanner.ShouldCheckForXmlText = false;
				if (la.Type == Tokens.CDATA )
				{
					Get();
				}
			}
			else
			{
				Misc();
			}
		}
	}
	void Text(bool useTrim)
	{
		XmlScanner.ShouldCheckForXmlText = false;
		Expect(Tokens.CDATA );
		string text = tToken.Value;
		if (useTrim)
		  text = text.Trim(' ', '\r', '\n');
		if (String.IsNullOrEmpty(text))
		  return;
		if (la.Type == Tokens.EOF && text == "*/")
		  return;
		XmlText xmlText = new XmlText();
		xmlText.Name = tToken.Value;
		xmlText.Text = tToken.Value;
		AddNode(xmlText);
		xmlText.SetRange(tToken.Range);
	}
	void Prolog()
	{
		if (la.Type == Tokens.XMLTAGOPEN )
		{
			XMLDecl();
		}
		while (la.Type == Tokens.COMMENT  || la.Type == Tokens.QUESTTAGOPEN )
		{
			Misc();
		}
		if (la.Type == Tokens.DOCTYPE )
		{
			DocTypeDecl();
		}
		while (la.Type == Tokens.COMMENT  || la.Type == Tokens.QUESTTAGOPEN )
		{
			Misc();
		}
	}
	void Element()
	{
		XmlScanner.ShouldCheckForXmlText = false;
		XmlAttribute attribute = null;
		XmlElement xmlElement = new XmlElement();
		OpenContext(xmlElement);
		xmlElement.SetRange(la.Range);
		Expect(Tokens.TAGOPEN );
		Name();
		xmlElement.Name = tToken.Value;
		while (StartOf(2))
		{
			Attribute(out attribute);
			if (attribute != null)
			xmlElement.AddDetailNode(attribute);
		}
		XmlScanner.ShouldCheckForXmlText = true;
		if (la.Type == Tokens.TAGCLOSE )
		{
			Get();
		}
		else if (la.Type == Tokens.EMPTYTAGCLOSE )
		{
			Get();
			CloseContext();
			xmlElement.HasCloseTag = false;
			xmlElement.SetRange(GetRange(xmlElement, tToken));
			return;
		}
		else
			SynErr(59);
		while (StartOf(3))
		{
			Content();
		}
		XmlScanner.ShouldCheckForXmlText = false; 
		if (la.Type != Tokens.TAGCLOSESTART)
			XmlScanner.ShouldCheckForXmlText = true;
		if (la.Type == Tokens.TAGCLOSESTART )
		{
			Get();
			Name();
			XmlScanner.ShouldCheckForXmlText = true;
			Expect(Tokens.TAGCLOSE );
		}
		xmlElement.SetRange(GetRange(xmlElement, tToken));
		CloseContext();
	}
	void Misc()
	{
		XmlProcessingInstruction processingInstruction = null;
		if (la.Type == Tokens.COMMENT )
		{
			Comment();
		}
		else if (la.Type == Tokens.QUESTTAGOPEN )
		{
			PI(out processingInstruction, true);
			if (processingInstruction != null)
			AddNode(processingInstruction);
		}
		else
			SynErr(60);
	}
	void XMLDecl()
	{
		XmlDecl xmlDecl = new XmlDecl();
		AddNode(xmlDecl);
		xmlDecl.SetRange(la.Range);
		Expect(Tokens.XMLTAGOPEN );
		VersionInfo(xmlDecl);
		if (la.Type == Tokens.ENCODING )
		{
			EncodingDecl(xmlDecl);
		}
		if (la.Type == Tokens.STANDALONE )
		{
			SDDecl(xmlDecl);
		}
		Expect(Tokens.QUESTTAGCLOSE );
		xmlDecl.SetRange(GetRange(xmlDecl, tToken));
	}
	void DocTypeDecl()
	{
		XmlDocTypeDecl docTypeDecl = new XmlDocTypeDecl();
		AddNode(docTypeDecl);
		docTypeDecl.SetRange(la.Range);
		NewExternalIDLink extID = null;
		Expect(Tokens.DOCTYPE );
		Name();
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
	void VersionInfo(XmlDecl xmlDecl)
	{
		Expect(Tokens.VERSION );
		Expect(Tokens.EQUALSTOKEN );
		Expect(3 );
		xmlDecl.Version = tToken.Value;
	}
	void EncodingDecl(XmlDecl xmlDecl)
	{
		Expect(Tokens.ENCODING );
		Expect(Tokens.EQUALSTOKEN );
		Expect(3 );
		xmlDecl.Encoding = tToken.Value;
	}
	void SDDecl(XmlDecl xmlDecl)
	{
		Expect(Tokens.STANDALONE );
		Expect(Tokens.EQUALSTOKEN );
		Expect(3 );
		xmlDecl.StandAlone = tToken.Value;
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
		case Tokens.VERSION : 
		{
			Get();
			break;
		}
		case Tokens.ENCODING : 
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
		default: SynErr(61); break;
		}
	}
	void ExternalID(out NewExternalIDLink extID)
	{
		extID = null;
		Token startToken = la;
		if (la.Type == Tokens.SYSTEM )
		{
			Get();
			Expect(3 );
			NewExternalIDSystemLink result = new NewExternalIDSystemLink();
			result.SystemURI = tToken.Value;
			extID = result;
		}
		else if (la.Type == Tokens.PUBLIC )
		{
			Get();
			NewExternalIDPublicLink result = new NewExternalIDPublicLink();
			extID = result;
			Expect(3 );
			result.PublicID = tToken.Value;
			if (la.Type == 3 )
			{
				Get();
				result.SystemURI = tToken.Value;
			}
		}
		else
			SynErr(62);
		if (extID != null)
		extID.SetRange(GetRange(startToken, tToken));
	}
	void IntSubset(XmlDocTypeDecl docTypeDecl)
	{
		while (StartOf(4))
		{
			if (StartOf(5))
			{
				MarkupDecl(docTypeDecl);
			}
			else
			{
				DeclSep();
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
		switch (la.Type)
		{
		case Tokens.ELEMENTDECL : 
		{
			ElementDecl(out elementDecl);
			if (docTypeDecl != null && elementDecl != null)
			docTypeDecl.AddNode(elementDecl);
			break;
		}
		case Tokens.ATTLISTDECL : 
		{
			AttlistDecl(out attListDecl);
			if (docTypeDecl != null && attListDecl != null)
			docTypeDecl.AddNode(attListDecl);
			break;
		}
		case Tokens.ENTITYDECL : 
		{
			EntityDecl(out entityDecl);
			if (docTypeDecl != null && entityDecl != null)
			docTypeDecl.AddNode(entityDecl);
			break;
		}
		case Tokens.NOTATIONDECl : 
		{
			NotationDecl(out notationDecl);
			if (docTypeDecl != null && notationDecl != null)
			docTypeDecl.AddNode(notationDecl);
			break;
		}
		case Tokens.QUESTTAGOPEN : 
		{
			PI(out  processingInstruction, false);
			if (docTypeDecl != null && processingInstruction != null)
			docTypeDecl.AddNode(processingInstruction);
			break;
		}
		case Tokens.COMMENT : 
		{
			Comment();
			break;
		}
		default: SynErr(63); break;
		}
	}
	void DeclSep()
	{
		XmlNameReference nameRef;
		PEReference(out nameRef);
		if (nameRef != null)
		AddNode(nameRef);
	}
	void ElementDecl(out XmlElementDecl elementDecl)
	{
		elementDecl = new XmlElementDecl();
		elementDecl.SetRange(la.Range);
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
		result.SetRange(la.Range);
		Expect(Tokens.ATTLISTDECL );
		Name();
		result.Name = tToken.Value; 
		while (StartOf(2))
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
		entityDecl.SetRange(la.Range);
		Expect(Tokens.ENTITYDECL );
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
		notationDecl.SetRange(la.Range);
		Expect(Tokens.NOTATIONDECl );
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
	void PI(out XmlProcessingInstruction processingInstruction, bool checkForText)
	{
		processingInstruction = new XmlProcessingInstruction();
		XmlScanner.ShouldCheckForXmlText = false;
		processingInstruction.SetRange(la.Range);
		Expect(Tokens.QUESTTAGOPEN );
		XmlScanner.ShouldReturnPIChars = true;
		Name();
		processingInstruction.Name = tToken.Value;
		Expect(Tokens.PICHARS );
		processingInstruction.InstructionText = tToken.Value;
		XmlScanner.ShouldCheckForXmlText = checkForText;
		Expect(Tokens.QUESTTAGCLOSE );
		processingInstruction.SetRange(GetRange(processingInstruction, tToken));
	}
	void Comment()
	{
		XmlScanner.ShouldCheckForXmlText = false;
		Comment comment = new Comment();
		comment.CommentType = CommentType.MultiLine;
		AddNode(comment);
		XmlScanner.ShouldCheckForXmlText = true;
		Expect(Tokens.COMMENT );
		comment.SetRange(tToken.Range);comment.Name = tToken.Value;
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
		Token startToken = la;
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
			else if (StartOf(6))
			{
				Children(out childContent);
				contentSpec = childContent;
			}
			else
				SynErr(64);
			if (contentSpec != null)
			contentSpec.SetRange(GetRange(startToken, tToken));
		}
		else
			SynErr(65);
	}
	void Mixed(out XmlMixedContentSpec contentSpec)
	{
		contentSpec = new XmlMixedContentSpec();
		XmlNameReference nameRef;
		Expect(Tokens.PCDATA );
		while (la.Type == Tokens.BITWISEOR )
		{
			Get();
			if (StartOf(2))
			{
				Name();
				XmlName name = new XmlName();
				name.Name = tToken.Value;
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
				SynErr(66);
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
		Token startToken = la;
		if (StartOf(2))
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
			SynErr(67);
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
			contentParticle.SetRange(GetRange(startToken, tToken));
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
		if (StartOf(7))
		{
			TokenizedType(attrDecl);
		}
		else if (la.Type == Tokens.PARENOPEN  || la.Type == Tokens.NOTATION )
		{
			Enumeration(attrDecl);
		}
		else
			SynErr(68);
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
		else if (la.Type == 3  || la.Type == Tokens.FIXED )
		{
			if (la.Type == Tokens.FIXED )
			{
				Get();
			}
			Expect(3 );
			attrDecl.DefaultAttributeValueType = DefaultAttributeValueType.Fixed;
			attrDecl.DefaultAttributeValue = tToken.Value;
		}
		else
			SynErr(69);
	}
	void TokenizedType(XmlAttributeDeclaration attrDecl)
	{
		switch (la.Type)
		{
		case Tokens.IDTOKEN : 
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
		default: SynErr(70); break;
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
	void EntityDef(XmlEntityDecl entityDecl)
	{
		NewExternalIDLink extID = null;
		if (la.Type == 3 )
		{
			Get();
			if (entityDecl != null)
			entityDecl.ImmediateValue = tToken.Value;
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
			SynErr(71);
	}
	void CDSect()
	{
		XmlCharacterData cData = new XmlCharacterData();
		AddNode(cData);
		XmlScanner.ShouldCheckForXmlText = false;
		cData.SetRange(la.Range);
		XmlScanner.ShouldReturnCharDataToken = true;
		Expect(Tokens.CDATASTART );
		Expect(Tokens.CDATA );
		cData.Name = tToken.Value;XmlScanner.ShouldCheckForXmlText = true; 
		Expect(Tokens.CDATAEND );
		cData.SetRange(GetRange(cData, tToken));
	}
	void Attribute(out XmlAttribute attribute)
	{
		attribute = new XmlAttribute();
		attribute.SetRange(la.Range);
		Name();
		attribute.Name = tToken.Value;
		attribute.SetNameRange(tToken.Range);
		Expect(Tokens.EQUALSTOKEN );
		Expect(3 );
		if (tToken.Value != null && tToken.Value.Length > 0)
		{
			int startPos = 0;
			int endPos = tToken.Value.Length;
			if (tToken.Value[0] == '"' || tToken.Value[0] == '\'')
				startPos = 1;
			if (tToken.Value[tToken.Value.Length -1] == '"' || tToken.Value[tToken.Value.Length -1] == '\'')
				endPos = tToken.Value.Length -1;
			attribute.Value = tToken.Value.Substring(startPos, endPos - startPos);	
		}
		attribute.SetValueRange(new SourceRange(tToken.Range.Start.Line, tToken.Range.Start.Offset + 1, tToken.Range.End.Line, tToken.Range.End.Offset - 1));
		attribute.SetRange(GetRange(attribute, tToken));
	}
	void Content()
	{
		XmlProcessingInstruction processingInstruction = null;
		switch (la.Type)
		{
		case Tokens.TAGOPEN : 
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
		{
			Comment();
			break;
		}
		case Tokens.CDATA : 
		{
			Text(false);
			break;
		}
		default: SynErr(72); break;
		}
	}
	void Reference()
	{
		XmlScanner.ShouldCheckForXmlText = false;
		if (la.Type == Tokens.AMPERSAND )
		{
			Get();
			XmlReference xmlReference = new XmlReference();AddNode(xmlReference);xmlReference.SetRange(tToken.Range);
			Name();
			xmlReference.SetRange(GetRange(xmlReference, la));
			xmlReference.Name = tToken.Value;
			XmlScanner.ShouldCheckForXmlText = true;
			Expect(Tokens.SEMICOLON );
		}
		else if (la.Type == Tokens.CHARREF )
		{
			Get();
			XmlCharReference xmlCharReference = new XmlCharReference();
			AddNode(xmlCharReference);
			xmlCharReference.SetRange(GetRange(tToken, la));
			xmlCharReference.Name = tToken.Value;
			XmlScanner.ShouldCheckForXmlText = true;
			Expect(Tokens.SEMICOLON );
		}
		else
			SynErr(73);
	}
		void Parse()
		{
			la = new Token();
			la.Value = "";
	  XmlScanner.ShouldCheckForXmlText = true;
			Get();
			Parser();
			Expect(0);
			if (Context != null)
				Context.SetRange(GetRange(Context, tToken));
			CloseContext();
		}
		protected override LanguageElement Parse(string code, int startLine, int startColumn)
		{
			ISourceReader stringReader = new SourceStringReader(code, startLine, startColumn);
			return Parse(stringReader);
		}
		protected override LanguageElement DoParse(ParserContext parserContext, ISourceReader reader)
		{
			LanguageElement context = parserContext.Context;
			if (context == null)
				return null;
			SetRootNode(context);
			if (context is SourceFile)
				((SourceFile)context).SetDocument(parserContext.Document);
			return Parse(reader);
		}
		public LanguageElement Parse(ISourceReader reader)
		{
			try
			{
				scanner = new XmlScanner(reader);
				if (!(RootNode is SourceFile))
					OpenContext(GetSourceFile("dsf"));
				Parse();
				return RootNode;
			}
			finally
			{
				CleanUpParser();
			}
		}
		protected override bool[,] CreateSetArray()
		{
			bool[,] set =
			{
			{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x},
		{x,T,x,x, x,x,x,x, T,T,x,x, x,T,T,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, T,x,T,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, T,T,x,x, x,T,T,x, x,x,x,x, T,T,T,x, x,x,T,x, x,x,x,x, x,x,x,T, T,T,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x}
			};
			return set;
		}
	} 
	public class XmlParserErrors : ParserErrorsBase
	{
		protected override string GetSyntaxErrorText(int n)
		{
			string s;
			switch (n)
			{
				case 0: s = "EOF expected"; break;
			case 1: s = "NAME expected"; break;
			case 2: s = "COMMENT expected"; break;
			case 3: s = "QUOTEDLITERAL expected"; break;
			case 4: s = "CHARREF expected"; break;
			case 5: s = "XMLTAGOPEN expected"; break;
			case 6: s = "EQUALS expected"; break;
			case 7: s = "SINGLEQUOTE expected"; break;
			case 8: s = "VERSION expected"; break;
			case 9: s = "ENCODING expected"; break;
			case 10: s = "QUESTTAGOPEN expected"; break;
			case 11: s = "QUESTTAGCLOSE expected"; break;
			case 12: s = "STANDALONE expected"; break;
			case 13: s = "SYSTEM expected"; break;
			case 14: s = "PUBLIC expected"; break;
			case 15: s = "DOCTYPE expected"; break;
			case 16: s = "ELEMENTDECL expected"; break;
			case 17: s = "TAGCLOSE expected"; break;
			case 18: s = "BRACKETOPEN expected"; break;
			case 19: s = "BRACKETCLOSE expected"; break;
			case 20: s = "EMPTY expected"; break;
			case 21: s = "ANYCONTENT expected"; break;
			case 22: s = "PARENOPEN expected"; break;
			case 23: s = "PARENCLOSE expected"; break;
			case 24: s = "PCDATA expected"; break;
			case 25: s = "BITWISEOR expected"; break;
			case 26: s = "PERCENTSYMBOL expected"; break;
			case 27: s = "SEMICOLON expected"; break;
			case 28: s = "COMMA expected"; break;
			case 29: s = "QUESTION expected"; break;
			case 30: s = "STAR expected"; break;
			case 31: s = "PLUS expected"; break;
			case 32: s = "NOTATIONDECl expected"; break;
			case 33: s = "ATTLISTDECL expected"; break;
			case 34: s = "IDTOKEN expected"; break;
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
			case 46: s = "ENTITYDECL expected"; break;
			case 47: s = "NDATA expected"; break;
			case 48: s = "TAGOPEN expected"; break;
			case 49: s = "EMPTYTAGCLOSE expected"; break;
			case 50: s = "TAGCLOSESTART expected"; break;
			case 51: s = "AMPERSAND expected"; break;
			case 52: s = "CDATASTART expected"; break;
			case 53: s = "CDATAEND expected"; break;
			case 54: s = "CDATA expected"; break;
			case 55: s = "PICHARS expected"; break;
			case 56: s = "??? expected"; break;
			case 57: s = "invalid Parser"; break;
			case 58: s = "invalid Parser"; break;
			case 59: s = "invalid Element"; break;
			case 60: s = "invalid Misc"; break;
			case 61: s = "invalid Name"; break;
			case 62: s = "invalid ExternalID"; break;
			case 63: s = "invalid MarkupDecl"; break;
			case 64: s = "invalid ContentSpec"; break;
			case 65: s = "invalid ContentSpec"; break;
			case 66: s = "invalid Mixed"; break;
			case 67: s = "invalid Cp"; break;
			case 68: s = "invalid AttType"; break;
			case 69: s = "invalid DefaultDecl"; break;
			case 70: s = "invalid TokenizedType"; break;
			case 71: s = "invalid EntityDef"; break;
			case 72: s = "invalid Content"; break;
			case 73: s = "invalid Reference"; break;
				default:
					s = "error " + n;
					break;
			}
			return s;
		}
	}
}
