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
#if SL
using DevExpress.Utils;
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CSharp
#else
namespace DevExpress.CodeParser.CSharp
#endif
{
#if DXCORE
using StructuralParser = DevExpress.CodeRush.StructuralParser;
#else
using StructuralParser = DevExpress.CodeParser;
#endif
	using PrimitiveExpression = CPrimitiveExpression;
	public  class Tokens
	{
		public const int LINE = 143;
		public const int SBYTE = 60;
		public const int LINETERMINATOR = 7;
		public const int EQ = 96;
		public const int SCOLON = 118;
		public const int REGION = 146;
		public const int ENDREG = 147;
		public const int STRUCT = 66;
		public const int BASE = 10;
		public const int CASE = 14;
		public const int RAZORCOMMENT = 126;
		public const int RBRACE = 115;
		public const int FINALLY = 32;
		public const int NEW = 48;
		public const int LBRACE = 100;
		public const int SHARPCOCODIRECTIVE = 6;
		public const int ARGLIST = 83;
		public const int GTEQ = 98;
		public const int ELIF = 140;
		public const int USINGKW = 78;
		public const int MINUSASSGN = 107;
		public const int LOCK = 45;
		public const int ENDIF = 142;
		public const int RBRACK = 116;
		public const int TYPEOF = 72;
		public const int EXPLICIT = 29;
		public const int IN = 40;
		public const int IDENT = 1;
		public const int LONG = 46;
		public const int REF = 58;
		public const int COMMA = 91;
		public const int STRINGCON = 5;
		public const int LBRACK = 101;
		public const int FLOAT = 34;
		public const int UNSAFE = 76;
		public const int REFVALUE = 84;
		public const int PRAGMADIR = 148;
		public const int IFCLAUSE = 38;
		public const int OUT = 51;
		public const int DO = 24;
		public const int ULONG = 74;
		public const int CHARCON = 4;
		public const int WHILE = 82;
		public const int DEC = 92;
		public const int PLUSASSGN = 113;
		public const int MODASSGN = 108;
		public const int TRY = 71;
		public const int ORASSGN = 111;
		public const int DBLQUEST = 127;
		public const int LAMBDA = 136;
		public const int THIS = 68;
		public const int SWITCH = 67;
		public const int EXTERN = 30;
		public const int CONST = 19;
		public const int DELEGATE = 23;
		public const int MINUS = 106;
		public const int WARNING = 145;
		public const int TRUE = 70;
		public const int CHAR = 16;
		public const int TILDE = 119;
		public const int VOID = 80;
		public const int TIMESASSGN = 121;
		public const int EVENT = 28;
		public const int DEFINE = 137;
		public const int BREAK = 12;
		public const int MULTILINEXML = 150;
		public const int UNDEF = 138;
		public const int DBLAND = 129;
		public const int PRIVATE = 54;
		public const int ATCOLON = 88;
		public const int BYTE = 13;
		public const int READONLY = 57;
		public const int DOUBLE = 25;
		public const int RETURN = 59;
		public const int TIMES = 120;
		public const int SEALED = 61;
		public const int LOWOREQ = 132;
		public const int LTLT = 105;
		public const int UNCHECKED = 75;
		public const int OR = 130;
		public const int MULTILINECOMMENT = 125;
		public const int DEFAULT = 22;
		public const int DBLCOLON = 95;
		public const int SINGLELINECOMMENT = 124;
		public const int XOR = 131;
		public const int NAMESPACE = 47;
		public const int CONTINUE = 20;
		public const int RSHASSGN = 152;
		public const int SIZEOF = 63;
		public const int ANDASSGN = 86;
		public const int ASPBLOCKEND = 154;
		public const int THROW = 69;
		public const int CATCH = 15;
		public const int LT = 104;
		public const int ENUM = 27;
		public const int FOR = 35;
		public const int AT = 89;
		public const int PROTECTED = 55;
		public const int AND = 85;
		public const int COLON = 90;
		public const int LSHASSGN = 103;
		public const int OPERATOR = 50;
		public const int INTCON = 2;
		public const int GT = 97;
		public const int ERROR = 144;
		public const int SINGLELINEXML = 149;
		public const int SHORT = 62;
		public const int DIV = 133;
		public const int ASPBLOCKSTART = 153;
		public const int MOD = 134;
		public const int IMPLICIT = 39;
		public const int ABSTRACT = 8;
		public const int CLASS = 18;
		public const int UINT = 73;
		public const int FALSE = 31;
		public const int STACKALLOC = 64;
		public const int GTGT = 151;
		public const int INTERFACE = 42;
		public const int POINTERTOMEMBER = 123;
		public const int ASSGN = 87;
		public const int INT = 41;
		public const int CHECKED = 17;
		public const int ELSE = 26;
		public const int USHORT = 77;
		public const int XORASSGN = 122;
		public const int GOTO = 37;
		public const int POINT = 135;
		public const int REALCON = 3;
		public const int NOT = 110;
		public const int NEQ = 109;
		public const int ASPCOMMENT = 155;
		public const int PLUS = 112;
		public const int DOT = 94;
		public const int FIXED = 33;
		public const int NULL = 49;
		public const int AS = 9;
		public const int VOLATILE = 81;
		public const int DIVASSGN = 93;
		public const int INC = 99;
		public const int ELSEDIR = 141;
		public const int QUESTION = 114;
		public const int BOOL = 11;
		public const int RPAR = 117;
		public const int OVERRIDE = 52;
		public const int EOF = 0;
		public const int STATIC = 65;
		public const int LPAR = 102;
		public const int IFDIR = 139;
		public const int VIRTUAL = 79;
		public const int FOREACH = 36;
		public const int IS = 44;
		public const int DECIMAL = 21;
		public const int PUBLIC = 56;
		public const int DBLOR = 128;
		public const int INTERNAL = 43;
		public const int PARAMS = 53;
		public const int MaxTokens = 156;
		public static int[] Keywords = {
		};
	}
	partial class CSharp30Parser
	{
		protected override void HandlePragmas()
		{
		}		
			void Parser()
	{
		while (IsExternAliasDirective())
		{
			ExternAliasDirective();
		}
		while (la.Type == Tokens.USINGKW )
		{
			UsingDirective();
		}
		while (IsGlobalAttrTarget())
		{
			GlobalAttributes();
		}
		while (StartOf(1))
		{
			NamespaceMemberDeclaration();
		}
	}
	void ExternAliasDirective()
	{
		SourceRange nameRange = SourceRange.Empty;
		SourceRange externAliasRange = la.Range;
		String name = String.Empty;
		Expect(Tokens.EXTERN );
		Expect(Tokens.IDENT );
		if (tToken.Value == "alias")
		 CorrectFormattingTokenType(tToken);
		else
		  Error("alias expected");
		Expect(Tokens.IDENT );
		name = tToken.Value;
		nameRange = tToken.Range;
		Expect(Tokens.SCOLON );
		externAliasRange = GetRange(externAliasRange, tToken);
		AddNode(new ExternAlias(name, externAliasRange, nameRange));
	}
	void UsingDirective()
	{
		ElementReferenceExpression elRef = null;
		SourceRange startRange = la.Range;
		NamespaceReference namespaceRef = null;
		String aliasName = String.Empty;
		ElementReferenceExpression aliasExpression = null;
		Expect(Tokens.USINGKW );
		if (IsAssignment())
		{
			Expect(Tokens.IDENT );
			aliasName = tToken.Value;
			aliasExpression = new ElementReferenceExpression(tToken.Value, tToken.Range);
			aliasExpression.SetRange(tToken.Range);
			Expect(Tokens.ASSGN );
		}
		ElementReferenceName(out elRef);
		Expect(Tokens.SCOLON );
		if (elRef != null)
		{
			if (aliasName != String.Empty)
				namespaceRef = new NamespaceReference(aliasName, GlobalStringStorage.Intern(elRef.ToString()));
			else
				namespaceRef = new NamespaceReference(GlobalStringStorage.Intern(elRef.ToString()));
			namespaceRef.NameRange = elRef.Range;
			if (aliasExpression != null)
			{
				namespaceRef.AddDetailNode(aliasExpression);
				namespaceRef.AliasNameRange = aliasExpression.NameRange;
			}
			namespaceRef.AddDetailNode(elRef);
			namespaceRef.SetRange(GetRange(startRange, tToken));
			AddNode(namespaceRef);
			SourceFile sourceFile = namespaceRef.FileNode;
			String namespaceRefName = GlobalStringStorage.Intern(elRef.ToString());
			if (sourceFile != null)
			{
				if (aliasName != String.Empty)
				{
					sourceFile.AliasList.Add(aliasName, namespaceRefName);
					if (!sourceFile.AliasHash.Contains(aliasName))
					{
						Expression clone = elRef.Clone() as Expression;
						sourceFile.AliasHash.Add(aliasName, clone);
					}
				}
				else
				{
					if (sourceFile.UsingList.IndexOfKey(namespaceRefName) < 0)
							sourceFile.UsingList.Add(namespaceRefName, namespaceRefName);
				}
			}
		}
	}
	void GlobalAttributes()
	{
		StructuralParser.Attribute attributeDef = null;
		AttributeSection attrSection = new AttributeSection();
		AttributeTargetType targetType = AttributeTargetType.None;
		SourceRange targetSpecifierRange = SourceRange.Empty;
		attrSection.SetRange(la.Range);
		Expect(Tokens.LBRACK );
		Expect(Tokens.IDENT );
		targetType = StructuralParser.Attribute.GetTargetTypeFromName(tToken.Value);
		targetSpecifierRange = tToken.Range;
		Expect(Tokens.COLON );
		Attribute(out attributeDef);
		if (attributeDef != null && attrSection != null)
		{				
			attrSection.AttributeCollection.Add(attributeDef);
			attrSection.AddDetailNode(attributeDef);
			attributeDef.TargetType = targetType;
			ProcessAttribute(attributeDef);
			if (targetSpecifierRange != SourceRange.Empty)
				attributeDef.SetRange(GetRange(targetSpecifierRange, attributeDef));
		}
		while (NotFinalComma())
		{
			Expect(Tokens.COMMA );
			Attribute(out attributeDef);
			if (attributeDef != null && attrSection != null)
			{
				ProcessAttribute(attributeDef);
				attrSection.AttributeCollection.Add(attributeDef);
				attrSection.AddDetailNode(attributeDef);
			}
		}
		if (la.Type == Tokens.COMMA )
		{
			Get();
		}
		Expect(Tokens.RBRACK );
		attrSection.SetRange(GetRange(attrSection, tToken));
		AddNode(attrSection);
	}
	void NamespaceMemberDeclaration()
	{
		Namespace namespaceDecl = null;
		SourceRange namespaceKWRange = SourceRange.Empty;
		SourceRange namespaceNameRange = SourceRange.Empty;
		StringBuilder builder = null;
		if (la.Type == Tokens.NAMESPACE )
		{
			Get();
			namespaceKWRange = tToken.Range; 
			Expect(Tokens.IDENT );
			namespaceNameRange = tToken.Range; 
			builder = new StringBuilder();
			builder.Append(tToken.Value);
			while (la.Type == Tokens.DOT )
			{
				Get();
				Expect(Tokens.IDENT );
				if (builder == null)
				builder = new StringBuilder();
				builder.AppendFormat(".{0}", tToken.Value);
			}
			String namespaceName = String.Empty;
			if (builder != null)
				namespaceName = GlobalStringStorage.Intern(builder.ToString());
			namespaceNameRange = GetRange(namespaceNameRange, tToken);
			namespaceDecl = new Namespace(namespaceName);
			OpenContext(namespaceDecl);
			namespaceDecl.NameRange = namespaceNameRange;
			namespaceDecl.SetRange(GetRange(namespaceKWRange, la));
			ReadBlockStart(la.Range);
			Expect(Tokens.LBRACE );
			while (IsExternAliasDirective())
			{
				ExternAliasDirective();
			}
			while (la.Type == Tokens.USINGKW )
			{
				UsingDirective();
			}
			while (StartOf(1))
			{
				NamespaceMemberDeclarations();
			}
			NamespaceMemberDeclarationEnd(namespaceDecl, true);
		}
		else if (IsAccessorDeclaration())
		{
			AccessorDeclarations();
		}
		else if (StartOf(2))
		{
			ClassMemberSeq();
		}
		else if (StartOf(3))
		{
			StatementSeq();
		}
		else
			SynErr(157);
	}
	void ElementReferenceName(out ElementReferenceExpression refExpr)
	{
		ElementReferenceExpression currentExpression = null;
		TypeReferenceExpressionCollection typeArguments = null;
		refExpr = null;
		Name();
		if (la.Type != Tokens.DBLCOLON)
		{
			currentExpression = new ElementReferenceExpression(tToken.Value, tToken.Range);
			currentExpression.NameRange = tToken.Range;
			refExpr = currentExpression;
		}
		else
		{
			currentExpression = new QualifiedAliasExpression(tToken.Value, tToken.Range);
			if (tToken.Value == "global")
				(currentExpression as QualifiedAliasExpression).IsGlobal = true;
		}
		if (la.Type == Tokens.DBLCOLON )
		{
			Get();
			Name();
			refExpr = new QualifiedElementReference(currentExpression, tToken.Value, tToken.Range);
			refExpr.SetRange(GetRange(currentExpression, tToken));
			refExpr.AddNode(currentExpression);
			currentExpression = refExpr;
		}
		if (IsGeneric())
		{
			TypeArgumentList(currentExpression, out typeArguments);
		}
		while (la.Type == Tokens.DOT )
		{
			Get();
			Name();
			refExpr = new QualifiedElementReference(currentExpression, tToken.Value, tToken.Range);
			refExpr.SetRange(GetRange(currentExpression, tToken));
			refExpr.AddNode(currentExpression);
			currentExpression = refExpr;
			if (IsGeneric())
			{
				TypeArgumentList(currentExpression, out typeArguments);
			}
		}
	}
	void NamespaceMemberDeclarations()
	{
		NamespaceMemberDeclaration();
		while (StartOf(1))
		{
			NamespaceMemberDeclaration();
		}
	}
	void NamespaceMemberDeclarationEnd(LanguageElement namespaceOrType, bool addToList)
	{
		Expect(Tokens.RBRACE );
		ReadBlockEnd(tToken.Range);
		if (la.Type == Tokens.SCOLON )
		{
			Get();
			SetHasEndingSemicolonIfNeeded(namespaceOrType); 
		}
		if (namespaceOrType != null)
		namespaceOrType.SetRange(GetRange(namespaceOrType, tToken));
		CloseContext();
		if (addToList && namespaceOrType is Namespace)
		 AddNamespaceToDeclaredList((Namespace)namespaceOrType);
	}
	void AccessorDeclarations()
	{
		AccessSpecifiers accessSpecifiers = null;
		MemberVisibility visibility = MemberVisibility.Illegal;
		PropertyAccessor accessor = null;
		SourceRange startRange = la.Range;
		LanguageElementCollection attributes = null;
		AttributeSections(out attributes);
		ModifierList(out accessSpecifiers, out visibility);
		startRange = la.Range;
		if (la.Type == Tokens.IDENT && la.Value == "get")
		{
			Expect(Tokens.IDENT );
			accessor = new Get();
			SetLastFormattingType(FormattingTokenType.Get);
		}
		else if (la.Type == Tokens.IDENT )
		{
			Get();
			if (tToken.Value != "set")
			return;
			accessor = new Set();
			SetLastFormattingType(FormattingTokenType.Set);
			AddImplicitParamToAccessor(Context as Member, accessor);
		}
		else
			SynErr(158);
		if (accessor != null)
		{
			CorrectFormattingTokenType(tToken);
			accessor.Name = GetAccessorName(tToken.Value + "_");
			if (accessSpecifiers != null)
				accessor.SetSpecifiers(accessSpecifiers.Specifiers);
			accessor.Visibility = visibility;
		   accessor.VisibilityRange = accessSpecifiers.VisibilityRange;
			OpenContext(accessor);
			accessor.SetRange(startRange);
			accessor.NameRange = tToken.Range;
			AddAccessSpecifiersRange(accessor, accessSpecifiers);
			if (attributes != null)
				accessor.SetAttributes(attributes);
		}
		if (la.Type == Tokens.LBRACE )
		{
			BlockCore(false);
		}
		else if (la.Type == Tokens.SCOLON )
		{
			Get();
		}
		else
			SynErr(159);
		if (accessor != null)
		{
			accessor.SetRange(GetRange(accessor, tToken));
			CloseContext();
		}
			Property parentProperty = Context as Property;
			if (parentProperty != null && !parentProperty.IsAbstract && !(parentProperty.Parent is Interface))
			{
				bool propertyHasImplementedGet = parentProperty.Getter != null && parentProperty.Getter.HasBlock;
				bool propertyHasImplementedSet = parentProperty.Setter != null && parentProperty.Setter.HasBlock;
				parentProperty.IsAutoImplemented = !(propertyHasImplementedGet || propertyHasImplementedSet); 
			}
	}
	void ClassMemberSeq()
	{
		AccessSpecifiers accessSpecifiers = null;
		MemberVisibility visibility = MemberVisibility.Illegal;
		LanguageElementCollection attributes = null;
		AttributeSections(out attributes);
		ModifierList(out accessSpecifiers, out visibility);
		ClassMemberDeclaration(accessSpecifiers, visibility, attributes);
		while (StartOf(2))
		{
			AttributeSections(out attributes);
			ModifierList(out accessSpecifiers, out visibility);
			ClassMemberDeclaration(accessSpecifiers, visibility, attributes);
		}
	}
	void StatementSeq()
	{
		Statement();
		while (StartOf(3))
		{
			if (_StatementNestingLevel > INT_MaxNestingLevel)
			 return;
			Token token = tToken;
			Statement();
			if (tToken == token)
			 Get();
		}
	}
	void AttributeSections(out LanguageElementCollection attributes)
	{
		attributes = null;
		AttributeSection attrSection = null;
		while (la.Type == Tokens.LBRACK )
		{
			Attributes(out attrSection);
			if (attrSection != null)
			{
			  if (attributes == null)
				attributes = new LanguageElementCollection();				
			  attributes.Add(attrSection);
			}
		}
	}
	void ModifierList(out AccessSpecifiers accessSpecifiers, out MemberVisibility visibility)
	{
		accessSpecifiers = new AccessSpecifiers();
		visibility = MemberVisibility.Illegal;
		bool wasProtectedInternal = false;
		bool wasInternalProtected = false;
		ModifierListCore(ref accessSpecifiers, ref visibility, ref wasProtectedInternal, ref wasInternalProtected);
		if (IsAsyncModifier())
		{
			Expect(Tokens.IDENT );
			accessSpecifiers.IsAsynchronous = true;
			accessSpecifiers.SetAsynchronousRange(tToken.Range);
			CorrectFormattingTokenType(tToken);
		}
		ModifierListCore(ref accessSpecifiers, ref visibility, ref wasProtectedInternal, ref wasInternalProtected);
		if (IsPartialModifier(la))
		{
			Expect(Tokens.IDENT );
			accessSpecifiers.IsPartial = true;
			accessSpecifiers.SetPartialRange(tToken.Range);
			CorrectFormattingTokenType(tToken);
		}
		ModifierListCore(ref accessSpecifiers, ref visibility, ref wasProtectedInternal, ref wasInternalProtected);
	}
	void ClassMemberDeclaration(AccessSpecifiers accessSpecifiers, MemberVisibility visibility, LanguageElementCollection attributes)
	{
		SourceRange startRange = la.Range;
		if (StartOf(4))
		{
			StructMemberDeclaration(accessSpecifiers, visibility, attributes);
		}
		else if (la.Type == Tokens.TILDE )
		{
			Get();
			Expect(Tokens.IDENT );
			Method destructor = CreateDestructor(tToken.Value, GetRange(startRange, tToken.Range));
			 OpenContext(destructor);
			if (attributes != null)
			{
				destructor.SetAttributes(attributes);
				if (attributes.Count > 0)
					destructor.SetRange(GetRange(attributes[0], destructor));
			}	
			destructor.SetRange(startRange);
			SetAccessSpecifiers(destructor, accessSpecifiers, visibility);
			Expect(Tokens.LPAR );
			SetMethodParensRanges(destructor, tToken.Range, la.Range);	
			Expect(Tokens.RPAR );
			if (la.Type == Tokens.LBRACE )
			{
				BlockCore(false);
			}
			else if (la.Type == Tokens.SCOLON )
			{
				Get();
				if (destructor != null)
				{
					destructor.SetRange(GetRange(destructor, tToken));
				}
			}
			else
				SynErr(160);
			CloseContext();
		}
		else
			SynErr(161);
	}
	void Statement()
	{
		Expression expression = null;
		LanguageElementCollection razorElements = null;
		if (Context is DelimiterCapableBlock && la.Type == Tokens.RBRACE)
			return;
		if (Context is AnonymousMethodExpression && la.Type == Tokens.RBRACE)
			return;
		 if (_StatementNestingLevel > INT_MaxNestingLevel)
		   return;
		try
		{
		_StatementNestingLevel++;
		while (!(StartOf(5)))
		{
			SynErr(162);
			Get();
		}
		if (IsRazorHtmlCode())
		{
			if (SetTokensCategory && SavedTokens.Count > 0)
			 SavedTokens.RemoveAt(SavedTokens.Count - 1);
			RazorHtmlCode(out razorElements);
		}
		else if (la.Type == Tokens.IDENT && scanner.Peek().Type == Tokens.COLON)
		{
			Expect(Tokens.IDENT );
			Label label = new Label();
			label.Name = tToken.Value;
			label.NameRange = tToken.Range;
			label.SetRange(GetRange(tToken, la));
			AddNode(label);
			Expect(Tokens.COLON );
			Statement();
		}
		else if (la.Type == Tokens.CONST )
		{
			ConstantDefinition(null, MemberVisibility.Local, null);
		}
		else if (IsLocalVarDecl())
		{
			LocalVariableDeclaration();
		}
		else if (la.Type == Tokens.LBRACE )
		{
			BlockCore(false);
		}
		else if (StartOf(6))
		{
			EmbeddedStatement();
		}
		else if (la.Type == Tokens.IFCLAUSE )
		{
			Get();
			If ifClause = new If();
			ifClause.SetRange(tToken.Range);
			OpenContext(ifClause);
			Token lParToken = la;
			Expect(Tokens.LPAR );
			Expression(out expression);
			if (expression != null && ifClause != null)
			ifClause.SetExpression(expression);
			SkipTo(Tokens.RPAR);
			if (expression != null)
			  expression.SetRange(GetRange(expression, tToken.Range));
			Expect(Tokens.RPAR );
			ifClause.SetParensRange(lParToken, tToken);
			Statement();
			CloseContextAndSetRange(ifClause);
			if (la.Type == Tokens.ELSE )
			{
				Get();
				Else elseClause = new Else();
				elseClause.SetRange(tToken.Range);
				OpenContext(elseClause);
				Statement();
				CloseContextAndSetRange(elseClause);
			}
		}
		else if (la.Type == Tokens.ELSE )
		{
			Get();
			Else elseClause = new Else();
			elseClause.SetRange(tToken.Range);
			OpenContext(elseClause);
			Statement();
			CloseContextAndSetRange(elseClause);			
		}
		else
			SynErr(163);
		if (_ParsingRazor)
		 return;
		while (!(StartOf(7)))
		{
			SynErr(164);
			Get();
		}
		}
		finally
		{
		_StatementNestingLevel--;
		}
	}
	void TypeDeclaration(AccessSpecifiers accessSpecifiers, MemberVisibility visibility, LanguageElementCollection attributes)
	{
		TypeReferenceExpression typeRef = null;
		TypeDeclaration typeDecl = null;
		SourceRange startRange = la.Range;
		GenericModifier genericModifier = null;
		Hashtable parameterAttributes = null;
		LanguageElementCollection parameters = null;
		if (StartOf(8))
		{
			if (la.Type == Tokens.CLASS  || la.Type == Tokens.INTERFACE  || la.Type == Tokens.STRUCT )
			{
				if (la.Type == Tokens.CLASS )
				{
					Get();
					typeDecl = new Class(la.Value); 
					OpenContext(typeDecl); 
					typeDecl.NameRange = la.Range;
					Expect(Tokens.IDENT );
					if (la.Type == Tokens.LT )
					{
						TypeParameterList(out genericModifier);
					}
					if (la.Type == Tokens.COLON )
					{
						ClassBase(typeDecl);
					}
					while (la.Type == Tokens.IDENT )
					{
						TypeParameterConstraintsClause(genericModifier);
					}
					InitializeTypeDeclaration(typeDecl, startRange, accessSpecifiers, visibility);
					ClassBody(typeDecl);
					if (la.Type == Tokens.SCOLON )
					{
						Get();
						SetHasEndingSemicolonIfNeeded(typeDecl); 
					}
				}
				else if (la.Type == Tokens.STRUCT )
				{
					Get();
					typeDecl = new Struct(la.Value); 
					OpenContext(typeDecl); 
					typeDecl.NameRange = la.Range;
					Expect(Tokens.IDENT );
					if (la.Type == Tokens.LT )
					{
						TypeParameterList(out genericModifier);
					}
					if (la.Type == Tokens.COLON )
					{
						ClassBase(typeDecl);
					}
					while (la.Type == Tokens.IDENT )
					{
						TypeParameterConstraintsClause(genericModifier);
					}
					InitializeTypeDeclaration(typeDecl, startRange, accessSpecifiers, visibility);
					StructBody(typeDecl);
					if (la.Type == Tokens.SCOLON )
					{
						Get();
						SetHasEndingSemicolonIfNeeded(typeDecl); 
					}
				}
				else if (la.Type == Tokens.INTERFACE )
				{
					Get();
					typeDecl = new Interface(la.Value); 
					OpenContext(typeDecl); 
					typeDecl.NameRange = la.Range;
					Expect(Tokens.IDENT );
					if (la.Type == Tokens.LT )
					{
						TypeParameterList(out genericModifier);
					}
					InitializeTypeDeclaration(typeDecl, startRange, accessSpecifiers, visibility);
					if (la.Type == Tokens.COLON )
					{
						ClassBase(typeDecl);
					}
					while (la.Type == Tokens.IDENT )
					{
						TypeParameterConstraintsClause(genericModifier);
					}
					ClassBody(typeDecl);
					if (la.Type == Tokens.SCOLON )
					{
						Get();
						SetHasEndingSemicolonIfNeeded(typeDecl); 
					}
				}
				else
					SynErr(165);
			}
			else
			{
				Get();
				typeDecl = new Enumeration(la.Value); 
				OpenContext(typeDecl); 
				typeDecl.NameRange = la.Range;
				Expect(Tokens.IDENT );
				if (la.Type == Tokens.COLON )
				{
					Get();
					IntegralType(out typeRef);
					Enumeration enumeration = typeDecl as Enumeration;
					if (enumeration != null && typeRef != null)
					{
						enumeration.UnderlyingType = GlobalStringStorage.Intern(typeRef.ToString());
						enumeration.UnderlyingTypeRange = typeRef.Range;
					}
				}
				InitializeTypeDeclaration(typeDecl, startRange, accessSpecifiers, visibility);
				EnumBody(typeDecl);
				if (la.Type == Tokens.SCOLON )
				{
					Get();
					SetHasEndingSemicolonIfNeeded(typeDecl); 
				}
			}
			if (typeDecl != null)
			{
						typeDecl.SetRange(GetRange(typeDecl, tToken));
						if (genericModifier != null)
							typeDecl.SetGenericModifier(genericModifier);
						if (attributes != null)
							typeDecl.SetAttributes(attributes);
					}
		}
		else if (la.Type == Tokens.DELEGATE )
		{
			Get();
			DelegateDefinition delegateDef = new DelegateDefinition();
			SourceRange lparRange = SourceRange.Empty;
			SourceRange rparRange = SourceRange.Empty;
			delegateDef.SetRange(tToken.Range);
			AddNode(delegateDef);
			Type(out typeRef, true);
			if (typeRef != null)
			{
				delegateDef.MemberType = typeRef.Name;
				delegateDef.MemberTypeReference = typeRef;
				delegateDef.AddDetailNode(typeRef);
			}
			Expect(Tokens.IDENT );
			delegateDef.Name = tToken.Value;
			delegateDef.NameRange = tToken.Range;
			if (la.Type == Tokens.LT )
			{
				TypeParameterList(out genericModifier);
			}
			Expect(Tokens.LPAR );
			lparRange = tToken.Range;
			if (delegateDef != null)
				delegateDef.SetRange(GetRange(delegateDef, tToken));
			if (StartOf(9))
			{
				FormalParameterList(out parameters, out parameterAttributes);
				SetParameters(delegateDef, parameters);
				SetAttributesForParameters(parameters, parameterAttributes);
			}
			while (!(la.Type == Tokens.EOF  || la.Type == Tokens.RPAR ))
			{
				SynErr(166);
				Get();
			}
			Expect(Tokens.RPAR );
			rparRange = tToken.Range;
			if (delegateDef != null)
				delegateDef.SetRange(GetRange(delegateDef, tToken));
			SetParensRanges(delegateDef, lparRange, rparRange);
			while (la.Type == Tokens.IDENT )
			{
				TypeParameterConstraintsClause(genericModifier);
			}
			if (genericModifier != null)
				delegateDef.SetGenericModifier(genericModifier);
			SetAccessSpecifiers(delegateDef, accessSpecifiers, visibility);
			Expect(Tokens.SCOLON );
			delegateDef.SetRange(GetRange(delegateDef, tToken));
			if (attributes != null)
				delegateDef.SetAttributes(attributes);
		}
		else
			SynErr(167);
	}
	void TypeParameterList(out GenericModifier genericModifier)
	{
		genericModifier = null;
		SourceRange startRange = la.Range;
		SourceRange startTypeParamRange = SourceRange.Empty;
		TypeParameter typeParam = null;
		LanguageElementCollection attributes = null;
		TypeParameterDirection typeArgumentDirection = TypeParameterDirection.None;
		Expect(Tokens.LT );
		startTypeParamRange = la.Range;
		AttributeSections(out attributes);
		TypeArgDirection(out typeArgumentDirection);
		Expect(Tokens.IDENT );
		TypeParameterCollection typeParams = new TypeParameterCollection();
		typeParam = new TypeParameter(tToken.Value, GetRange(startTypeParamRange, tToken));
		typeParam.Direction = typeArgumentDirection;
		typeParams.Add(typeParam);
		genericModifier = new GenericModifier(typeParams);
		typeParam.NameRange = tToken.Range;
		if (attributes != null)
			typeParam.SetAttributes(attributes);
		while (la.Type == Tokens.COMMA )
		{
			Get();
			startTypeParamRange = la.Range;
			AttributeSections(out attributes);
			TypeArgDirection(out typeArgumentDirection);
			Expect(Tokens.IDENT );
			typeParam = new TypeParameter(tToken.Value, GetRange(startTypeParamRange, tToken));
			typeParam.Direction = typeArgumentDirection;
			typeParam.NameRange = tToken.Range;
			genericModifier.TypeParameters.Add(typeParam);
			genericModifier.AddDetailNode(typeParam);
			if (attributes != null)
				typeParam.SetAttributes(attributes);
		}
		Expect(Tokens.GT );
		if (genericModifier != null)
		genericModifier.SetRange(GetRange(startRange, tToken.Range));
	}
	void ClassBase(TypeDeclaration typeDecl)
	{
		TypeReferenceExpression typeRef = null;
		Expect(Tokens.COLON );
		ClassType(out typeRef);
		if (typeDecl != null && typeRef != null)
		typeDecl.PrimaryAncestorType = typeRef;
		while (la.Type == Tokens.COMMA )
		{
			Get();
			TypeName(out typeRef);
			if (typeDecl != null && typeRef != null)
			typeDecl.AddSecondaryAncestorType(typeRef);
		}
	}
	void TypeParameterConstraintsClause(GenericModifier genericModifier)
	{
		TypeReferenceExpression typeRef = null;
		TypeParameter currentTypeParameter = null;
		TypeParameterConstraint constraint = null;
		SourceRange newRange = SourceRange.Empty;
		Expect(Tokens.IDENT );
		CorrectFormattingTokenType(tToken);
		Expect(Tokens.IDENT );
		currentTypeParameter = GetTypeParameter(genericModifier, tToken.Value);
		typeRef = new TypeReferenceExpression(tToken.Value, tToken.Range);
		typeRef.NameRange = tToken.Range;
		if (currentTypeParameter != null)
			currentTypeParameter.AddDetailNode(typeRef);
		Expect(Tokens.COLON );
		while (StartOf(10))
		{
			if (la.Type == Tokens.CLASS )
			{
				Get();
				constraint = new ClassTypeParameterConstraint(tToken.Value, tToken.Range);
				constraint.NameRange = tToken.Range;
			}
			else if (la.Type == Tokens.STRUCT )
			{
				Get();
				constraint = new StructTypeParameterConstraint(tToken.Value, tToken.Range);
				constraint.NameRange = tToken.Range;
			}
			else if (la.Type == Tokens.IDENT  || la.Type == Tokens.ARGLIST  || la.Type == Tokens.REFVALUE )
			{
				ClassType(out typeRef);
				if (typeRef != null)
				{
					constraint = new NamedTypeParameterConstraint(typeRef);
					constraint.Name = typeRef.Name;
					constraint.SetRange(typeRef.Range);
					constraint.NameRange = typeRef.Range;
				}
			}
			else
			{
				Get();
				newRange = tToken.Range;
				Expect(Tokens.LPAR );
				Expect(Tokens.RPAR );
				constraint = new NewTypeParameterConstraint("new()", GetRange(newRange, tToken));
				constraint.NameRange = constraint.Range;
			}
			if (constraint != null && currentTypeParameter != null)
			{
				currentTypeParameter.Constraints.Add(constraint);
				currentTypeParameter.AddDetailNode(constraint);
				constraint = null;
			}
			if (la.Type == Tokens.COMMA )
			{
				Get();
			}
			if (la.Value == "where")
			return;
		}
	}
	void ClassBody(TypeDeclaration classDecl)
	{
		AccessSpecifiers accessSpecifiers = null;
		MemberVisibility visibility = MemberVisibility.Illegal;
		LanguageElementCollection attributes = null;
		SourceRange blockStart = SourceRange.Empty;
		SourceRange blockEnd = SourceRange.Empty;
		Expect(Tokens.LBRACE );
		if (tToken.Type == Tokens.LBRACE)
		blockStart = tToken.Range;
		   if (classDecl != null)
		   {
		   classDecl.SetBlockStart(blockStart); 
			 classDecl.SetBlockType(DelimiterBlockType.Brace);
		   }
		while (StartOf(2))
		{
			AttributeSections(out attributes);
			ModifierList(out accessSpecifiers, out visibility);
			ClassMemberDeclaration(accessSpecifiers, visibility, attributes);
			attributes = null;
			while (!(StartOf(11)))
			{
				SynErr(168);
				Get();
			}
		}
		if (la.Type == Tokens.RBRACE)
		CloseContext();
		Expect(Tokens.RBRACE );
		if (tToken.Type == Tokens.RBRACE)
		blockEnd = tToken.Range;
		if (classDecl != null)
		classDecl.SetBlockEnd(blockEnd);
	}
	void StructBody(TypeDeclaration structDecl)
	{
		AccessSpecifiers accessSpecifiers = null;
		MemberVisibility visibility = MemberVisibility.Illegal;
		LanguageElementCollection attributes = null;
		Expect(Tokens.LBRACE );
		structDecl.SetBlockStart(tToken.Range); structDecl.SetBlockType(DelimiterBlockType.Brace);
		while (StartOf(12))
		{
			AttributeSections(out attributes);
			ModifierList(out accessSpecifiers, out visibility);
			StructMemberDeclaration(accessSpecifiers, visibility, attributes);
			attributes = null;
			while (!(StartOf(13)))
			{
				SynErr(169);
				Get();
			}
		}
		if (la.Type == Tokens.RBRACE)
		CloseContext();
		Expect(Tokens.RBRACE );
		structDecl.SetBlockEnd(tToken.Range);
	}
	void IntegralType(out TypeReferenceExpression type)
	{
		type = new TypeReferenceExpression(la.Value, la.Range);
		type.NameRange = la.Range;
		switch (la.Type)
		{
		case Tokens.SBYTE : 
		{
			Get();
			break;
		}
		case Tokens.BYTE : 
		{
			Get();
			break;
		}
		case Tokens.SHORT : 
		{
			Get();
			break;
		}
		case Tokens.USHORT : 
		{
			Get();
			break;
		}
		case Tokens.INT : 
		{
			Get();
			break;
		}
		case Tokens.UINT : 
		{
			Get();
			break;
		}
		case Tokens.LONG : 
		{
			Get();
			break;
		}
		case Tokens.ULONG : 
		{
			Get();
			break;
		}
		case Tokens.CHAR : 
		{
			Get();
			break;
		}
		default: SynErr(170); break;
		}
	}
	void EnumBody(TypeDeclaration enumDecl)
	{
		Expect(Tokens.LBRACE );
		enumDecl.SetBlockStart(tToken.Range); enumDecl.SetBlockType(DelimiterBlockType.Brace);
		if (la.Type == Tokens.IDENT  || la.Type == Tokens.LBRACK )
		{
			EnumMembers();
		}
		if (la.Type == Tokens.RBRACE)
		CloseContext();
		Expect(Tokens.RBRACE );
		enumDecl.SetBlockEnd(tToken.Range);
	}
	void Type(out TypeReferenceExpression type, bool ignoreTypeCastChecks)
	{
		type = null;
		TypeReferenceExpressionCollection typeArguments = null;
		SimpleType(out type, ignoreTypeCastChecks);
		if (la.Type == Tokens.LT )
		{
			TypeArgumentList(type, out typeArguments);
		}
		while (la.Type == Tokens.LBRACK  || la.Type == Tokens.TIMES )
		{
			PointerOrArray(ref type);
			if (la.Type == Tokens.LT )
			{
				TypeArgumentList(type, out typeArguments);
			}
		}
	}
	void FormalParameterList(out LanguageElementCollection parameters, out Hashtable parameterAttributes)
	{
		Param parameter = null;
		parameters = new LanguageElementCollection();
		LanguageElementCollection attributes = null;
		parameterAttributes = new Hashtable();
		ParameterDeclaration(out parameter, out attributes);
		if (parameter != null)
		{
		parameters.Add(parameter);
		if (attributes != null)
			parameterAttributes.Add(parameter, attributes);	
		}
		while (la.Type == Tokens.COMMA )
		{
			Get();
			ParameterDeclaration(out parameter, out attributes);
			if (parameter != null)
			{
			parameters.Add(parameter);
			if (attributes != null)
				parameterAttributes.Add(parameter, attributes);	
			}
		}
	}
	void ClassType(out TypeReferenceExpression type)
	{
		type = null;
		TypeName(out type);
	}
	void TypeName(out TypeReferenceExpression refExpr)
	{
		TypeReferenceExpression currentExpression = null;
		TypeReferenceExpressionCollection typeArguments = null;
		QualifiedAliasExpression qualAlias = null;
		refExpr = null;
		Name();
		if (la.Type != Tokens.DBLCOLON)
		{
			currentExpression = new TypeReferenceExpression(tToken.Value, tToken.Range);
			currentExpression.NameRange = tToken.Range;
			currentExpression.IsDynamic = tToken.Value == "dynamic";
			refExpr = currentExpression;
		}
		else
		{
			qualAlias = new QualifiedAliasExpression(tToken.Value, tToken.Range);
			if (tToken.Value == "global")
				qualAlias.IsGlobal = true;
		}
		if (la.Type == Tokens.DBLCOLON )
		{
			Get();
			Name();
			refExpr = new TypeReferenceExpression(tToken.Value, tToken.Range);
			refExpr.NameRange = tToken.Range;
			if (qualAlias != null)
			{
				refExpr.SetRange(GetRange(qualAlias, tToken));
				refExpr.Qualifier = qualAlias;
				refExpr.AddDetailNode(qualAlias);
			}
			currentExpression = refExpr;
		}
		if (la.Type == Tokens.LT )
		{
			TypeArgumentList(currentExpression, out typeArguments);
		}
		while (la.Type == Tokens.DOT )
		{
			Get();
			Name();
			refExpr = new TypeReferenceExpression(tToken.Value, tToken.Range, currentExpression);
			refExpr.SetRange(GetRange(currentExpression, tToken));
			refExpr.NameRange = tToken.Range;
			currentExpression = refExpr;
			if (la.Type == Tokens.LT )
			{
				TypeArgumentList(currentExpression, out typeArguments);
			}
		}
	}
	void StructMemberDeclaration(AccessSpecifiers accessSpecifiers, MemberVisibility visibility, LanguageElementCollection attributes)
	{
		TypeReferenceExpression typeRef = null;
		ElementReferenceExpression memberName = null;
		SourceRange fixedRange = SourceRange.Empty;
		if (la.Type == Tokens.FIXED )
		{
			Get();
			fixedRange = tToken.Range;
			Type(out typeRef, true);
			FixedBufferFieldDecl(typeRef, accessSpecifiers, visibility, attributes, fixedRange);
		}
		else if (la.Type == Tokens.CONST )
		{
			ConstantDefinition(accessSpecifiers, visibility, attributes);
		}
		else if (la.Type == Tokens.EVENT )
		{
			EventDeclaration(accessSpecifiers, visibility, attributes);
		}
		else if (IsConstructor())
		{
			ConstructorDeclaration(accessSpecifiers, visibility, attributes);
		}
		else if (StartOf(14))
		{
			Type(out typeRef, true);
			if (la.Type == Tokens.OPERATOR )
			{
				OverloadableOperator(typeRef, accessSpecifiers, visibility, attributes);
			}
			else if (IsFieldDecl())
			{
				FieldDecl(typeRef, accessSpecifiers, visibility, attributes);
			}
			else if (la.Type == Tokens.IDENT || la.Type == Tokens.LBRACE || la.Type == Tokens.LPAR)
			{
				MemberName(out memberName);
				if (la.Type == Tokens.LBRACE )
				{
					PropertyDeclaration(typeRef, memberName, accessSpecifiers, visibility, attributes);
				}
				else if (la.Type == Tokens.THIS )
				{
					IndexerDeclaration(typeRef, memberName, accessSpecifiers, visibility, attributes);
				}
				else if (la.Type == Tokens.LPAR  || la.Type == Tokens.LT )
				{
					MethodDeclaration(typeRef, memberName, accessSpecifiers, visibility, attributes);
				}
				else
					SynErr(171);
			}
			else if (la.Type == Tokens.THIS )
			{
				IndexerDeclaration(typeRef, null, accessSpecifiers, visibility, attributes);
			}
			else if (StartOf(15))
			{
			}
			else
				SynErr(172);
		}
		else if (la.Type == Tokens.EXPLICIT  || la.Type == Tokens.IMPLICIT )
		{
			CastOperator(accessSpecifiers, visibility, attributes);
		}
		else if (StartOf(16))
		{
			TypeDeclaration(accessSpecifiers, visibility, attributes);
		}
		else
			SynErr(173);
	}
	void EnumMembers()
	{
		EnumMemberDeclaration();
		while (NotFinalComma())
		{
			Expect(Tokens.COMMA );
			EnumMemberDeclaration();
		}
		if (la.Type == Tokens.COMMA )
		{
			Get();
		}
	}
	void EnumMemberDeclaration()
	{
		Expression expression = null;
		String name = String.Empty;
		SourceRange startRange = la.Range;
		SourceRange nameRange = SourceRange.Empty;
		SourceRange endRange = SourceRange.Empty;
		LanguageElementCollection attributes = null;
		AttributeSections(out attributes);
		Expect(Tokens.IDENT );
		startRange = tToken.Range;
		nameRange = tToken.Range;
		name = tToken.Value;
		if (la.Type == Tokens.ASSGN )
		{
			Get();
			Expression(out expression);
		}
		endRange = tToken.Range;
		EnumElement enumElement = null;
		if(!String.IsNullOrEmpty(name))
		  enumElement = CreateEnumElement(name, expression, nameRange, startRange, endRange);
		if (enumElement != null)
		{
		   enumElement.HasComma = la.Type == Tokens.COMMA;
		  AddNode(enumElement);
		  if (attributes != null)
		  	enumElement.SetAttributes(attributes);
		}
	}
	void BlockCore(bool isMethodBlock)
	{
		Block block = null;
			if ((Context != null && Context is IHasBlock && !(Context as IHasBlock).BlockStart.IsEmpty) || Context is Block || Context is Case)
			{
				block = new Block();
				OpenContext(block);
				block.SetRange(la.Range);
			}
			ReadBlockStart(la.Range); 
			if (Context != null && Context is ParentingStatement)
				(Context as ParentingStatement).HasBlock = true;
			Token firstBlockToken = null;
			Token lastBlockToken = null;
		if (ShouldSkipMethodBody)
		{
			Expect(Tokens.LBRACE );
			firstBlockToken = tToken;
			SkipMethodBody(); 
			lastBlockToken = la;
			Expect(Tokens.RBRACE );
			ReadBlockEnd(tToken.Range); 
			if (Context != null && _Reader != null)
			{
				Context.SetRange(GetRange(Context, tToken));
			}
			if (isMethodBlock)
				CloseContext();
		}
		else if (la.Type == Tokens.LBRACE )
		{
			Get();
			StatementSeq();
			ReadBlockEnd(la.Range); 
			if (block != null)
			{
				block.SetRange(GetRange(block, la.Range));
				CloseContext();
			}
			else if (_ParsingRazor && _RazorParsingMode != RazorParsingMode.Functions)
			{
				Context.SetRange(GetRange(Context, la));
				CloseContext();
			}
			if (Context != null && Context is Method && _ParsingPostponedTokens == false)
			{
				Context.SetRange(GetRange(Context, la));
			}
			if (isMethodBlock)
				CloseMemberContext(la);
			Expect(Tokens.RBRACE );
		}
		else
			SynErr(174);
	}
	void FixedBufferFieldDecl(TypeReferenceExpression typeRef, AccessSpecifiers accessSpecifiers, MemberVisibility visibility, LanguageElementCollection attributes, SourceRange startRange)
	{
		Expression  arraySize = null;
		string name = la.Value;
		SourceRange nameRange = la.Range;
		SourceRange varStartRange = startRange;
		Variable variable = null;
		Variable prevVariable = null;
		Variable firstVariable = null;
		if (typeRef != null)
			startRange = typeRef.Range;
		Expect(Tokens.IDENT );
		if (la.Type == Tokens.LBRACK )
		{
			Get();
			if (StartOf(17))
			{
				Expression(out arraySize);
			}
			Expect(Tokens.RBRACK );
		}
		variable = CreateVariable(name, typeRef, null, nameRange, varStartRange, true, SourceRange.Empty);
		if (arraySize != null)
			variable.FixedSize = arraySize;
		arraySize = null;
		AddNode(variable); 
		prevVariable = variable;
		firstVariable = variable;
		SetAccessSpecifiers(variable, accessSpecifiers, visibility);
		SetAttributes(variable, attributes);
		while (la.Type == Tokens.COMMA )
		{
			Get();
			if (variable != null)
			variable.SetRange(GetRange(variable, tToken));
			name = la.Value;
			nameRange = la.Range;
			varStartRange = la.Range;
			Expect(Tokens.IDENT );
			if (la.Type == Tokens.LBRACK )
			{
				Get();
				if (StartOf(17))
				{
					Expression(out arraySize);
				}
				Expect(Tokens.RBRACK );
			}
			variable = CreateVariable(name, typeRef, null, nameRange, varStartRange, false, SourceRange.Empty);
			variable.SetPreviousVariable(prevVariable);
			variable.SetAncestorVariable(firstVariable);
			prevVariable.SetNextVariable(variable);
			prevVariable = variable;
			if (arraySize != null)
				variable.FixedSize = arraySize;
			arraySize = null;
			AddNode(variable); 
			SetAccessSpecifiers(variable, accessSpecifiers, visibility, false);
		}
		Expect(Tokens.SCOLON );
		if (variable != null)
		variable.SetRange(GetRange(variable, tToken));
	}
	void Expression(out Expression expression)
	{
		expression = null;
		Expression rightPart = null;
		Token operatorToken = null;
		AssignmentOperatorType assignmentOperator = AssignmentOperatorType.None;
		Expression currentLeftPart = null;
		Expression trueExpr = null;
		Expression falseExpr = null;
		if (IsLambda())
		{
			LambdaExpression(out expression);
		}
		else if (StartOf(18))
		{
			if (IsSqlExpression())
			{
				SqlExpression(out expression);
			}
			else
			{
				Unary(out expression);
			}
		}
		else
			SynErr(175);
		currentLeftPart = expression;
		if (assgnOps[la.Type] || (la.Type == Tokens.GT && scanner.Peek().Type == Tokens.GTEQ))
		{
			AssignmentOperator(out operatorToken, out assignmentOperator);
			Argument(out rightPart, false);
			if (currentLeftPart is ElementReferenceExpression)
			(currentLeftPart as ElementReferenceExpression).IsModified = true;
			expression = GetAssignmentExpression(currentLeftPart, rightPart, assignmentOperator, operatorToken);
		}
		else if (StartOf(19))
		{
			NullCoalescingExpr(out expression, currentLeftPart);
			if (expression == null)
			expression = currentLeftPart;
			if (la.Type == Tokens.QUESTION )
			{
				Get();
				Expression(out trueExpr);
				Expect(Tokens.COLON );
				Expression(out falseExpr);
				Expression condition = expression;
				expression = new ConditionalExpression(condition, trueExpr, falseExpr);
				expression.Name = String.Empty;
				expression.SetRange(GetRange(condition, falseExpr));
			}
		}
		else
			SynErr(176);
	}
	void FieldDecl(TypeReferenceExpression typeRef, AccessSpecifiers accessSpecifiers, MemberVisibility visibility, LanguageElementCollection attributes)
	{
		Expression  initializer = null;
		string name = la.Value;
		SourceRange nameRange = la.Range;
		Variable variable = null;
		Variable prevVariable = null;
		Variable firstVariable = null;
		SourceRange startRange = SourceRange.Empty;
		if (typeRef != null)
			startRange = typeRef.Range;
		SourceRange operatorRange = SourceRange.Empty;
		Expect(Tokens.IDENT );
		if (la.Type == Tokens.ASSGN )
		{
			Get();
			operatorRange = tToken.Range;
			VariableInitializer(out initializer);
		}
		variable = CreateVariable(name, typeRef, initializer, nameRange, startRange, true, operatorRange);
		prevVariable = variable;
		firstVariable = variable;
		initializer = null;
		operatorRange = SourceRange.Empty;
		AddNode(variable); 
		SetAccessSpecifiers(variable, accessSpecifiers, visibility);
		SetAttributes(variable, attributes);
		while (la.Type == Tokens.COMMA )
		{
			Get();
			if (variable != null)
			variable.SetRange(GetRange(variable, tToken));
			name = la.Value;
			nameRange = la.Range;
			startRange = la.Range;
			Expect(Tokens.IDENT );
			if (la.Type == Tokens.ASSGN )
			{
				Get();
				operatorRange = tToken.Range;
				VariableInitializer(out initializer);
			}
			variable = CreateVariable(name, typeRef, initializer, nameRange, startRange, false, operatorRange);
			variable.SetPreviousVariable(prevVariable);
			variable.SetAncestorVariable(firstVariable);
			prevVariable.SetNextVariable(variable);
			prevVariable = variable;
			initializer = null;
			operatorRange = SourceRange.Empty;
			AddNode(variable); 
			SetAccessSpecifiers(variable, accessSpecifiers, visibility, false);
		}
		Expect(Tokens.SCOLON );
		if (variable != null)
		variable.SetRange(GetRange(variable, tToken));
	}
	void VariableInitializer(out Expression expression)
	{
		expression = null;
		ArrayInitializerExpression initializer = null;
		if (StartOf(17))
		{
			Argument(out expression, true);
		}
		else if (la.Type == Tokens.LBRACE )
		{
			ArrayInitializer(out initializer);
		}
		else
			SynErr(177);
		if (initializer != null)
		expression = initializer;
	}
	void FixedFieldDecl(TypeReferenceExpression typeRef)
	{
		Expression  initializer = null;
		string name = la.Value;
		SourceRange nameRange = la.Range;
		Variable variable = null;
		Variable prevVariable = null;
		Variable firstVariable = null;
		SourceRange startRange = SourceRange.Empty;
		if (typeRef != null)
			startRange = typeRef.Range;
		SourceRange operatorRange = SourceRange.Empty;
		Expect(Tokens.IDENT );
		if (la.Type == Tokens.ASSGN )
		{
			Get();
			operatorRange = tToken.Range;
			VariableInitializer(out initializer);
		}
		variable = CreateVariable(name, typeRef, initializer, nameRange, startRange, true, operatorRange);
		prevVariable = variable;
		firstVariable = variable;
		initializer = null;
		operatorRange = SourceRange.Empty;
		AddDetailNode(variable); 
		while (la.Type == Tokens.COMMA )
		{
			Get();
			if (variable != null)
			variable.SetRange(GetRange(variable, tToken));
			name = la.Value;
			nameRange = la.Range;
			startRange = la.Range;
			Expect(Tokens.IDENT );
			if (la.Type == Tokens.ASSGN )
			{
				Get();
				operatorRange = tToken.Range;
				VariableInitializer(out initializer);
			}
			variable = CreateVariable(name, typeRef, initializer, nameRange, startRange, false, operatorRange);
			variable.SetPreviousVariable(prevVariable);
			variable.SetAncestorVariable(firstVariable);
			prevVariable.SetNextVariable(variable);
			prevVariable = variable;
			initializer = null;
			operatorRange = SourceRange.Empty;
			AddDetailNode(variable); 
		}
		if (variable != null)
		variable.SetRange(GetRange(variable, tToken));
	}
	void ForFieldDecl(TypeReferenceExpression typeRef)
	{
		Expression  initializer = null;
		string name = la.Value;
		SourceRange nameRange = la.Range;
		Variable variable = null;
		Variable prevVariable = null;
		Variable firstVariable = null;
		SourceRange startRange = SourceRange.Empty;
		if (typeRef != null)
			startRange = typeRef.Range;
		For forStatement = Context as For;
		SourceRange operatorRange = SourceRange.Empty;
		Expect(Tokens.IDENT );
		if (la.Type == Tokens.ASSGN )
		{
			Get();
			operatorRange = tToken.Range;
			VariableInitializer(out initializer);
		}
		variable = CreateVariable(name, typeRef, initializer, nameRange, startRange, true, operatorRange);
		prevVariable = variable;
		firstVariable = variable;
		initializer = null;
		operatorRange = SourceRange.Empty;
		if (forStatement != null && variable != null)
		{
			forStatement.Initializers.Add(variable);
			forStatement.AddDetailNode(variable);
		}
		while (la.Type == Tokens.COMMA )
		{
			Get();
			if (variable != null)
			variable.SetRange(GetRange(variable, tToken));
			name = la.Value;
			nameRange = la.Range;
			startRange = la.Range;
			Expect(Tokens.IDENT );
			if (la.Type == Tokens.ASSGN )
			{
				Get();
				operatorRange = tToken.Range;
				VariableInitializer(out initializer);
			}
			variable = CreateVariable(name, typeRef, initializer, nameRange, startRange, false, operatorRange);
			variable.SetPreviousVariable(prevVariable);
			variable.SetAncestorVariable(firstVariable);
			prevVariable.SetNextVariable(variable);
			prevVariable = variable;
			initializer = null;
			operatorRange = SourceRange.Empty;
			if (forStatement != null && variable != null)
			{
				forStatement.Initializers.Add(variable);
				forStatement.AddDetailNode(variable);
				variable.Visibility = MemberVisibility.Local;
			}
		}
		Token endToken = tToken;
		if (la.Type == Tokens.SCOLON)
			endToken = la;
		if (variable != null)
			variable.SetRange(GetRange(variable, endToken));
	}
	void UsingFieldDecl(TypeReferenceExpression typeRef)
	{
		Expression  initializer = null;
		string name = la.Value;
		SourceRange nameRange = la.Range;
		Variable variable = null;
		Variable prevVariable = null;
		Variable firstVariable = null;
		SourceRange startRange = SourceRange.Empty;
		if (typeRef != null)
			startRange = typeRef.Range;
		UsingStatement usingStatement = Context as UsingStatement;
		SourceRange operatorRange = SourceRange.Empty;
		Expect(Tokens.IDENT );
		if (la.Type == Tokens.ASSGN )
		{
			Get();
			operatorRange = tToken.Range;
			VariableInitializer(out initializer);
		}
		variable = CreateVariable(name, typeRef, initializer, nameRange, startRange, true, operatorRange);
		prevVariable = variable;
		firstVariable = variable;
		initializer = null;
		operatorRange = SourceRange.Empty;
		if (usingStatement != null && variable != null)
		{
			usingStatement.Initializers.Add(variable);
			usingStatement.AddDetailNode(variable);
		}
		while (la.Type == Tokens.COMMA )
		{
			Get();
			if (variable != null)
			variable.SetRange(GetRange(variable, tToken));
			name = la.Value;
			nameRange = la.Range;
			startRange = la.Range;
			Expect(Tokens.IDENT );
			if (la.Type == Tokens.ASSGN )
			{
				Get();
				operatorRange = tToken.Range;
				VariableInitializer(out initializer);
			}
			variable = CreateVariable(name, typeRef, initializer, nameRange, startRange, false, operatorRange);
			variable.HasType = false;
			variable.SetPreviousVariable(prevVariable);
			variable.SetAncestorVariable(firstVariable);
			prevVariable.SetNextVariable(variable);
			prevVariable = variable;
			initializer = null;
			operatorRange = SourceRange.Empty;
			if (usingStatement != null && variable != null)
			{
				usingStatement.Initializers.Add(variable);
				usingStatement.AddDetailNode(variable);
			}
		}
		if (la.Type == Tokens.SCOLON )
		{
			Get();
		}
		if (variable != null)
		variable.SetRange(GetRange(variable, tToken));
	}
	void ImplicitForFieldDecl()
	{
		Expression  initializer = null;
		string name = String.Empty;
		SourceRange nameRange = SourceRange.Empty;
		ImplicitVariable variable = null;
		ImplicitVariable prevVariable = null;
		ImplicitVariable firstVariable = null;
		SourceRange startRange = la.Range;
		For forStatement = Context as For;
		SourceRange operatorRange = SourceRange.Empty;
		SourceRange varRange = la.Range;
		 CorrectFormattingTokenType(la);
		Expect(Tokens.IDENT );
		name = la.Value; nameRange = la.Range;
		Expect(Tokens.IDENT );
		Expect(Tokens.ASSGN );
		operatorRange = tToken.Range;
		VariableInitializer(out initializer);
		variable = CreateImplicitVariable(name, initializer, nameRange, startRange, true, operatorRange, varRange);
		prevVariable = variable;
		firstVariable = variable;
		initializer = null;
		operatorRange = SourceRange.Empty;
		if (forStatement != null && variable != null)
		{
			forStatement.Initializers.Add(variable);
			forStatement.AddDetailNode(variable);
		}
		while (la.Type == Tokens.COMMA )
		{
			Get();
			if (variable != null)
			variable.SetRange(GetRange(variable, tToken));
			name = la.Value;
			nameRange = la.Range;
			startRange = la.Range;
			Expect(Tokens.IDENT );
			Expect(Tokens.ASSGN );
			operatorRange = tToken.Range;
			VariableInitializer(out initializer);
			variable = CreateImplicitVariable(name, initializer, nameRange, startRange, false, operatorRange, varRange);
			variable.SetPreviousVariable(prevVariable);
			variable.SetAncestorVariable(firstVariable);
			prevVariable.SetNextVariable(variable);
			prevVariable = variable;
			initializer = null;
			operatorRange = SourceRange.Empty;
			if (forStatement != null && variable != null)
			{
				forStatement.Initializers.Add(variable);
				forStatement.AddDetailNode(variable);
			}
		}
		Token endToken = tToken;
		if (la.Type == Tokens.SCOLON)
			endToken = la;
		if (variable != null)
			variable.SetRange(GetRange(variable, endToken));
	}
	void ImplicitUsingFieldDecl()
	{
		Expression  initializer = null;
		string name = la.Value;
		SourceRange nameRange = la.Range;
		ImplicitVariable variable = null;
		ImplicitVariable prevVariable = null;
		ImplicitVariable firstVariable = null;
		SourceRange startRange = la.Range;
		UsingStatement usingStatement = Context as UsingStatement;
		SourceRange operatorRange = SourceRange.Empty;
		SourceRange varRange = la.Range;
		 CorrectFormattingTokenType(la);
		Expect(Tokens.IDENT );
		name = la.Value; nameRange = la.Range;
		Expect(Tokens.IDENT );
		Expect(Tokens.ASSGN );
		operatorRange = tToken.Range;
		VariableInitializer(out initializer);
		variable = CreateImplicitVariable(name, initializer, nameRange, startRange, true, operatorRange, varRange);
		prevVariable = variable;
		firstVariable = variable;
		initializer = null;
		operatorRange = SourceRange.Empty;
		if (usingStatement != null && variable != null)
		{
			usingStatement.Initializers.Add(variable);
			usingStatement.AddDetailNode(variable);
		}
		while (la.Type == Tokens.COMMA )
		{
			Get();
			if (variable != null)
			variable.SetRange(GetRange(variable, tToken));
			name = la.Value;
			nameRange = la.Range;
			startRange = la.Range;
			Expect(Tokens.IDENT );
			Expect(Tokens.ASSGN );
			operatorRange = tToken.Range;
			VariableInitializer(out initializer);
			variable = CreateImplicitVariable(name, initializer, nameRange, startRange, false, operatorRange, varRange);
			variable.SetPreviousVariable(prevVariable);
			variable.SetAncestorVariable(firstVariable);
			prevVariable.SetNextVariable(variable);
			prevVariable = variable;
			initializer = null;
			operatorRange = SourceRange.Empty;
			if (usingStatement != null && variable != null)
			{
				usingStatement.Initializers.Add(variable);
				usingStatement.AddDetailNode(variable);
			}
		}
		if (la.Type == Tokens.SCOLON )
		{
			Get();
		}
		if (variable != null)
		variable.SetRange(GetRange(variable, tToken));
	}
	void ConstantDefinition(AccessSpecifiers accessSpecifiers, MemberVisibility visibility, LanguageElementCollection attributes)
	{
		TypeReferenceExpression typeRef = null;
		Expression constValue = null;
		string constName = String.Empty; 
		SourceRange nameRange = SourceRange.Empty;
		Const constant = null;
		Const prevConst = null;
		Const firstConst = null;
		SourceRange startRange = la.Range;
		SourceRange operatorRange = SourceRange.Empty;
		Expect(Tokens.CONST );
		Type(out typeRef, true);
		Expect(Tokens.IDENT );
		constName = tToken.Value; nameRange = tToken.Range;
		Expect(Tokens.ASSGN );
		operatorRange = tToken.Range;
		VariableInitializer(out constValue);
		constant = CreateConstant(constName, typeRef, constValue, nameRange, startRange, true, operatorRange);
		prevConst = constant;
		firstConst = constant;
		operatorRange = SourceRange.Empty;
		AddNode(constant); 
		SetAccessSpecifiers(constant, accessSpecifiers, visibility);
		SetAttributes(constant, attributes);
		while (la.Type == Tokens.COMMA )
		{
			Get();
			if (constant != null)
			constant.SetRange(GetRange(constant, tToken));
			Expect(Tokens.IDENT );
			constName = tToken.Value; nameRange = tToken.Range; startRange = tToken.Range;
			Expect(Tokens.ASSGN );
			operatorRange = tToken.Range;
			VariableInitializer(out constValue);
			constant = CreateConstant(constName, typeRef, constValue, nameRange, startRange, false, operatorRange);
			constant.SetPreviousVariable(prevConst);
			constant.SetAncestorVariable(firstConst);
			prevConst.SetNextVariable(constant);
			prevConst = constant;
			operatorRange = SourceRange.Empty;
			AddNode(constant); 
			SetAccessSpecifiers(constant, accessSpecifiers, visibility, false);		
		}
		Expect(Tokens.SCOLON );
		if (constant != null)
		constant.SetRange(GetRange(constant, tToken));
	}
	void ConstructorDeclaration(AccessSpecifiers accessSpecifiers, MemberVisibility visibility, LanguageElementCollection attributes)
	{
		ExpressionCollection arguments = null;
		Method method = CreateConstructor(la.Value, la.Range);
		LanguageElementCollection parameters = null;
		SourceRange lparRange = SourceRange.Empty;
		SourceRange rparRange = SourceRange.Empty;
		Hashtable parameterAttributes = null;
		Expect(Tokens.IDENT );
		method.SetRange(tToken.Range);
		SetAccessSpecifiers(method, accessSpecifiers, visibility);
		lparRange = la.Range;
		OpenContext(method);
		Expect(Tokens.LPAR );
		if (method != null)
		 method.SetRange(GetRange(method, tToken));
		if (StartOf(9))
		{
			FormalParameterList(out parameters, out parameterAttributes);
			SetParameters(method, parameters);
			SetAttributesForParameters(parameters, parameterAttributes);
		}
		rparRange = la.Range;
		SetMethodParensRanges(method, lparRange, rparRange);
		while (!(la.Type == Tokens.EOF  || la.Type == Tokens.RPAR ))
		{
			SynErr(178);
			Get();
		}
		Expect(Tokens.RPAR );
		if (method != null)
		 method.SetRange(GetRange(method, tToken));
		if (la.Type == Tokens.COLON )
		{
			ConstructorInitializer constructorInit = new ConstructorInitializer();
			constructorInit.SetRange(la.Range);			   
			method.AddNode(constructorInit);
			Get();
			constructorInit.Name = la.Value;
			constructorInit.NameRange = la.Range;
			if (la.Type == Tokens.BASE )
			{
				Get();
				constructorInit.Target = InitializerTarget.Ancestor;
			}
			else if (la.Type == Tokens.THIS )
			{
				Get();
				constructorInit.Target = InitializerTarget.ThisClass;
			}
			else
				SynErr(179);
			Expect(Tokens.LPAR );
			lparRange = tToken.Range;
			if (StartOf(17))
			{
				ArgumentCollection(out arguments);
				if (arguments != null)
				{
					constructorInit.Arguments = arguments;
					for (int i = 0; i < arguments.Count; i++)
						constructorInit.AddDetailNode(arguments[i]);
				}
			}
			Expect(Tokens.RPAR );
			rparRange = tToken.Range;
			constructorInit.SetRange(GetRange(constructorInit, tToken));   
			constructorInit.ParensRange = GetRange(lparRange, rparRange);
		}
		if (la.Type == Tokens.LBRACE )
		{
			BlockCore(false);
		}
		else if (la.Type == Tokens.SCOLON )
		{
			Get();
			if (method != null)
			{
				method.SetRange(GetRange(method, tToken));
			}
		}
		else
			SynErr(180);
		CloseContext();
		SetAttributes(method, attributes);
	}
	void ArgumentCollection(out ExpressionCollection arguments)
	{
		Expression argument = null;
		arguments = new ExpressionCollection();
		Argument(out argument, true);
		if (argument != null)
		arguments.Add(argument);
		while (la.Type == Tokens.COMMA )
		{
			Get();
			Argument(out argument, true);
			if (argument != null)
			arguments.Add(argument);
		}
	}
	void OverloadableOperator(TypeReferenceExpression typeRef, AccessSpecifiers accessSpecifiers, MemberVisibility visibility, LanguageElementCollection attributes)
	{
		LanguageElementCollection parameters = null;
		String name = String.Empty;
		SourceRange nameRange = SourceRange.Empty;
		SourceRange lparRange = SourceRange.Empty;
		SourceRange rparRange = SourceRange.Empty;
		Method overloadableOperator = null;
		Hashtable parameterAttributes = null;
		OperatorType operatorType = OperatorType.None;
		Expect(Tokens.OPERATOR );
		OverloadableOp(out name, out nameRange, out operatorType);
		Expect(Tokens.LPAR );
		lparRange = tToken.Range;
		if (StartOf(9))
		{
			FormalParameterList(out parameters, out parameterAttributes);
		}
		Expect(Tokens.RPAR );
		rparRange = tToken.Range;
		if (parameters != null && operatorType == OperatorType.UnaryPlus && parameters.Count > 1)
		{
			name = "op_Addition";
			operatorType = OperatorType.Addition;
		}
		if (parameters != null && operatorType == OperatorType.UnaryNegation && parameters.Count > 1)
		{
			name = "op_Subtraction";
			operatorType = OperatorType.Subtraction;
		}
		if (parameters != null && operatorType == OperatorType.Multiply && parameters.Count == 1)
		{
			name = "op_PointerDereference";
			operatorType = OperatorType.PointerDereference;
		}
		if (parameters != null && operatorType == OperatorType.BitwiseAnd && parameters.Count == 1)
		{
			name = "op_AddressOf";
			operatorType = OperatorType.AddressOf;
		}
		overloadableOperator = CreateClassOperator(name, nameRange, typeRef, operatorType);
		if (overloadableOperator != null)
		{
			SetParameters(overloadableOperator, parameters);
			SetAttributesForParameters(parameters, parameterAttributes);
			SetMethodParensRanges(overloadableOperator, lparRange, rparRange);
			OpenContext(overloadableOperator);
			overloadableOperator.SetRange(typeRef.Range);
			SetAccessSpecifiers(overloadableOperator, accessSpecifiers, visibility);
		}
		if (la.Type == Tokens.LBRACE )
		{
			BlockCore(false);
		}
		else if (la.Type == Tokens.SCOLON )
		{
			Get();
			if (overloadableOperator != null)
			{
				overloadableOperator.SetRange(GetRange(overloadableOperator, tToken));
			}
		}
		else
			SynErr(181);
		if (overloadableOperator != null)
		{
			CloseContext();
		}
		SetAttributes(overloadableOperator, attributes);
	}
	void OverloadableOp(out String name, out SourceRange nameRange, out OperatorType operatorType)
	{
		name = la.Value;
		nameRange = la.Range;
		operatorType = OperatorType.None;
		switch (la.Type)
		{
		case Tokens.PLUS : 
		{
			Get();
			name = "op_UnaryPlus";
			operatorType = OperatorType.UnaryPlus;
			break;
		}
		case Tokens.MINUS : 
		{
			Get();
			name = "op_UnaryNegation";
			operatorType = OperatorType.UnaryNegation;
			break;
		}
		case Tokens.NOT : 
		{
			Get();
			name = "op_LogicalNot";
			operatorType = OperatorType.LogicalNot;
			break;
		}
		case Tokens.TILDE : 
		{
			Get();
			name = "op_OnesComplement";
			operatorType = OperatorType.OnesComplement;
			break;
		}
		case Tokens.INC : 
		{
			Get();
			name = "op_Increment";
			operatorType = OperatorType.Increment;
			break;
		}
		case Tokens.DEC : 
		{
			Get();
			name = "op_Decrement";
			operatorType = OperatorType.Decrement;
			break;
		}
		case Tokens.TRUE : 
		{
			Get();
			name = "op_True";
			operatorType = OperatorType.True;
			break;
		}
		case Tokens.FALSE : 
		{
			Get();
			name = "op_False";
			operatorType = OperatorType.False;
			break;
		}
		case Tokens.TIMES : 
		{
			Get();
			name = "op_Multiply";
			operatorType = OperatorType.Multiply;
			break;
		}
		case Tokens.DIV : 
		{
			Get();
			name = "op_Division";
			operatorType = OperatorType.Division;
			break;
		}
		case Tokens.MOD : 
		{
			Get();
			name = "op_Modulus";
			operatorType = OperatorType.Modulus;
			break;
		}
		case Tokens.AND : 
		{
			Get();
			name = "op_BitwiseAnd";
			operatorType = OperatorType.BitwiseAnd;
			break;
		}
		case Tokens.OR : 
		{
			Get();
			name = "op_BitwiseOr";
			operatorType = OperatorType.BitwiseOr;
			break;
		}
		case Tokens.XOR : 
		{
			Get();
			name = "op_ExclusiveOr";
			operatorType = OperatorType.ExclusiveOr;
			break;
		}
		case Tokens.LTLT : 
		{
			Get();
			name = "op_LeftShift";
			operatorType = OperatorType.LeftShift;
			break;
		}
		case Tokens.EQ : 
		{
			Get();
			name = "op_Equality";
			operatorType = OperatorType.Equality;
			break;
		}
		case Tokens.NEQ : 
		{
			Get();
			name = "op_Inequality";
			operatorType = OperatorType.Inequality;
			break;
		}
		case Tokens.GT : 
		{
			Get();
			name = "op_GreaterThan";
			operatorType = OperatorType.GreaterThan;
			if (la.Type == Tokens.GT  || la.Type == Tokens.GTEQ )
			{
				if (la.Type == Tokens.GT )
				{
					Get();
					name = "op_RightShift";
					operatorType = OperatorType.RightShift;
					nameRange = GetRange(nameRange, tToken);
				}
				else
				{
					Get();
					name = "op_RightShiftAssignment";
					operatorType = OperatorType.RightShiftAssignment;
					nameRange = GetRange(nameRange, tToken);
				}
			}
			break;
		}
		case Tokens.LT : 
		{
			Get();
			name = "op_LessThan";
			operatorType = OperatorType.LessThan;
			break;
		}
		case Tokens.GTEQ : 
		{
			Get();
			name = "op_GreaterThanOrEqual";
			operatorType = OperatorType.GreaterThanOrEqual;
			break;
		}
		case Tokens.LOWOREQ : 
		{
			Get();
			name = "op_LessThanOrEqual";
			operatorType = OperatorType.LessThanOrEqual;
			break;
		}
		case Tokens.ANDASSGN : 
		{
			Get();
			name = "op_BitwiseAndAssignment";
			operatorType = OperatorType.BitwiseAndAssignment;
			break;
		}
		case Tokens.COMMA : 
		{
			Get();
			name = "op_Comma";
			operatorType = OperatorType.Comma;
			break;
		}
		case Tokens.DIVASSGN : 
		{
			Get();
			name = "op_DivisionAssignment";
			operatorType = OperatorType.DivisionAssignment;
			break;
		}
		case Tokens.LSHASSGN : 
		{
			Get();
			name = "op_LeftShiftAssignment";
			operatorType = OperatorType.LeftShiftAssignment;
			break;
		}
		case Tokens.MINUSASSGN : 
		{
			Get();
			name = "op_SubtractionAssignment";
			operatorType = OperatorType.SubtractionAssignment;
			break;
		}
		case Tokens.MODASSGN : 
		{
			Get();
			name = "op_ModulusAssignment";
			operatorType = OperatorType.ModulusAssignment;
			break;
		}
		case Tokens.ORASSGN : 
		{
			Get();
			name = "op_BitwiseOrAssignment";
			operatorType = OperatorType.BitwiseOrAssignment;
			break;
		}
		case Tokens.PLUSASSGN : 
		{
			Get();
			name = "op_AdditionAssignment";
			operatorType = OperatorType.AdditionAssignment;
			break;
		}
		case Tokens.TIMESASSGN : 
		{
			Get();
			name = "op_MultiplicationAssignment";
			operatorType = OperatorType.MultiplicationAssignment;
			break;
		}
		case Tokens.XORASSGN : 
		{
			Get();
			name = "op_ExclusiveOrAssignment";
			operatorType = OperatorType.ExclusiveOrAssignment;
			break;
		}
		case Tokens.POINTERTOMEMBER : 
		{
			Get();
			name = "op_PointerToMemberSelection";
			operatorType = OperatorType.PointerToMemberSelection;
			break;
		}
		case Tokens.POINT : 
		{
			Get();
			name = "op_MemberSelection";
			operatorType = OperatorType.MemberSelection;
			break;
		}
		case Tokens.ASSGN : 
		{
			Get();
			name = "op_Assign";
			operatorType = OperatorType.Assign;
			break;
		}
		case Tokens.DBLOR : 
		{
			Get();
			name = "op_LogicalOr";
			operatorType = OperatorType.LogicalOr;
			break;
		}
		case Tokens.DBLAND : 
		{
			Get();
			name = "op_LogicalAnd";
			operatorType = OperatorType.LogicalAnd;
			break;
		}
		default: SynErr(182); break;
		}
	}
	void MethodDeclaration(TypeReferenceExpression typeRef, ElementReferenceExpression memberName, AccessSpecifiers accessSpecifiers, MemberVisibility visibility, LanguageElementCollection attributes)
	{
		GenericModifier genericModifier = null;
		LanguageElementCollection parameters = null;
		Method method = CreateMethod(typeRef, memberName);
		 bool oldIsAsyncContext = IsAsyncContext;
		 if (accessSpecifiers != null)
		   IsAsyncContext = accessSpecifiers.IsAsynchronous;
		 else
		   IsAsyncContext = false;
		if (method != null)
		{
			OpenContext(method);
			method.SetRange(typeRef.Range);
			SetAccessSpecifiers(method, accessSpecifiers, visibility);
		}
		SourceRange lparRange = SourceRange.Empty;
		SourceRange rparRange = SourceRange.Empty;
		Hashtable parameterAttributes = null;
		if (la.Type == Tokens.LT )
		{
			TypeParameterList(out genericModifier);
			if (method != null && genericModifier != null)
			method.SetGenericModifier(genericModifier);
		}
		Expect(Tokens.LPAR );
		lparRange = tToken.Range; 
		if (method != null)
			method.SetRange(GetRange(method, tToken));
		if (StartOf(9))
		{
			FormalParameterList(out parameters, out parameterAttributes);
			SetParameters(method, parameters);
			SetAttributesForParameters(parameters, parameterAttributes);
		}
		while (!(la.Type == Tokens.EOF  || la.Type == Tokens.RPAR ))
		{
			SynErr(183);
			Get();
		}
		Expect(Tokens.RPAR );
		rparRange = tToken.Range; 
		if (method != null)
			method.SetRange(GetRange(method, tToken));
		SetMethodParensRanges(method, lparRange, rparRange);
		while (la.Type == Tokens.IDENT )
		{
			TypeParameterConstraintsClause(genericModifier);
		}
		if (la.Type == Tokens.LBRACE )
		{
			BlockCore(true);
		}
		else if (la.Type == Tokens.SCOLON )
		{
			method.GenerateCodeBlock = false; 
			Get();
			CloseContext();
		}
		else
			SynErr(184);
		if (method != null)
			{
				method.SetRange(GetRange(method, tToken));
			}
		SetAttributes(method, attributes);
		   IsAsyncContext = oldIsAsyncContext;
	}
	void PropertyDeclaration(TypeReferenceExpression type, ElementReferenceExpression name, AccessSpecifiers accessSpecifiers, MemberVisibility visibility, LanguageElementCollection attributes)
	{
		Property property = CreateProperty(type, name);
		if (property != null)
		{
			OpenContext(property);
			property.SetRange(GetRange(type));
			SetAccessSpecifiers(property, accessSpecifiers, visibility);
			ReadBlockStart(la.Range);
		}
		Expect(Tokens.LBRACE );
		AccessorDeclarationsSeq();
		CloseMemberContext(la);
		Expect(Tokens.RBRACE );
		if (property != null)
		{
			property.SetBlockEnd(tToken.Range);
			property.SetRange(GetRange(property, tToken));
		}
		SetAttributes(property, attributes);
	}
	void AccessorDeclarationsSeq()
	{
		AccessorDeclarations();
		while (StartOf(20))
		{
			AccessorDeclarations();
		}
	}
	void IndexerDeclaration(TypeReferenceExpression type, ElementReferenceExpression qualifier, AccessSpecifiers accessSpecifiers, MemberVisibility visibility, LanguageElementCollection attributes)
	{
		LanguageElementCollection parameters = null;
		Property indexer = null;
		if (qualifier == null)
			indexer = CreateIndexer(type, "Item", la.Range);
		else
		{
			indexer = CreateIndexer(type, GlobalStringStorage.Intern(qualifier.ToString()) + ".Item", la.Range);
			indexer.IsExplicitInterfaceMember = true;
			indexer.AddImplementsExpression(qualifier);
		}
		if (indexer != null)
		{
			OpenContext(indexer);
			indexer.SetRange(type.Range);
			SetAccessSpecifiers(indexer, accessSpecifiers, visibility);
		}
		SourceRange openParensRange = SourceRange.Empty;
		SourceRange closeParensRange = SourceRange.Empty;
		Hashtable parameterAttributes = null;
		Expect(Tokens.THIS );
		Expect(Tokens.LBRACK );
		openParensRange = tToken.Range;
		FormalParameterList(out parameters, out parameterAttributes);
		while (!(la.Type == Tokens.EOF  || la.Type == Tokens.RBRACK ))
		{
			SynErr(185);
			Get();
		}
		Expect(Tokens.RBRACK );
		closeParensRange = tToken.Range;
		SetParameters(indexer, parameters);
		SetAttributesForParameters(parameters, parameterAttributes);
		SetParensRanges(indexer, openParensRange, closeParensRange);
		SetIndexRanges(indexer, openParensRange, closeParensRange);
		Expect(Tokens.LBRACE );
		ReadBlockStart(tToken.Range);
		AccessorDeclarations();
		AccessorDeclarations();
		Expect(Tokens.RBRACE );
		if (indexer != null)
		{
			ReadBlockEnd(tToken.Range);
			CloseContext();
			indexer.SetRange(GetRange(indexer, tToken));
			SetAttributes(indexer, attributes);		
		}
	}
	void CastOperator(AccessSpecifiers accessSpecifiers, MemberVisibility visibility, LanguageElementCollection attributes)
	{
		TypeReferenceExpression typeRef = null;
		LanguageElementCollection parameters = null;
		bool isExplicit = false;
		SourceRange startRange = la.Range;
		SourceRange lParRange = SourceRange.Empty;
		SourceRange rParRange = SourceRange.Empty;
		Method method = null;
		Hashtable parameterAttributes = null;
		if (la.Type == Tokens.IMPLICIT )
		{
			Get();
		}
		else if (la.Type == Tokens.EXPLICIT )
		{
			Get();
			isExplicit = true;
		}
		else
			SynErr(186);
		Expect(Tokens.OPERATOR );
		Type(out typeRef, true);
		method = CreateCastOperator(typeRef, isExplicit);
		if (method != null)
		{
			OpenContext(method);
			method.SetRange(startRange);
			lParRange = la.Range;
			SetAccessSpecifiers(method, accessSpecifiers, visibility);
		}
		Expect(Tokens.LPAR );
		FormalParameterList(out parameters, out parameterAttributes);
		while (!(la.Type == Tokens.EOF  || la.Type == Tokens.RPAR ))
		{
			SynErr(187);
			Get();
		}
		Expect(Tokens.RPAR );
		rParRange = tToken.Range;
		if (method != null)
		{
			SetParameters(method, parameters);
			SetMethodParensRanges(method, lParRange, rParRange);
		}
		if (la.Type == Tokens.LBRACE )
		{
			BlockCore(false);
		}
		else if (la.Type == Tokens.SCOLON )
		{
			Get();
		}
		else
			SynErr(188);
		if (method != null)
		{
			CloseContext();
			method.SetRange(GetRange(method, tToken));
		}
		SetAttributes(method, attributes);		
	}
	void EventDeclaration(AccessSpecifiers accessSpecifiers, MemberVisibility visibility, LanguageElementCollection attributes)
	{
		TypeReferenceExpression type = null;
		ElementReferenceExpression eventName = null;
		SourceRange startRange = la.Range;
		Event eventDecl = null;
		Expression initializer = null;
		Expect(Tokens.EVENT );
		Type(out type, true);
		if (IsFieldDecl())
		{
			Expect(Tokens.IDENT );
			eventDecl = CreateEvent(type, tToken.Value, tToken.Range);
			if (eventDecl != null)
			{
				eventDecl.IsInterfaceEvent = (Context != null) && (Context is Interface);
				OpenContext(eventDecl);
				eventDecl.SetRange(startRange);
				SetAccessSpecifiers(eventDecl, accessSpecifiers, visibility);
			}
			if (la.Type == Tokens.ASSGN )
			{
				Get();
				Expression(out initializer);
				if (initializer != null && eventDecl != null)
				eventDecl.Initializer = initializer;
			}
			while (la.Type == Tokens.COMMA )
			{
				Get();
				startRange = la.Range;
				if (eventDecl != null)
				{
					CloseContext();
					eventDecl.SetRange(GetRange(eventDecl, tToken));
				}
				Expect(Tokens.IDENT );
				eventDecl = CreateEvent(type, tToken.Value, tToken.Range);
				if (eventDecl != null)
				{
					eventDecl.IsInterfaceEvent = (Context != null) && (Context is Interface);
					OpenContext(eventDecl);
					eventDecl.SetRange(startRange);
					SetAccessSpecifiers(eventDecl, accessSpecifiers, visibility);
					eventDecl.SetRange(startRange);
				}
				if (la.Type == Tokens.ASSGN )
				{
					Get();
					Expression(out initializer);
				}
				if (initializer != null && eventDecl != null)
				eventDecl.Initializer = initializer;
			}
			Expect(Tokens.SCOLON );
			if (eventDecl != null)
			{
				CloseContext();
				eventDecl.SetRange(GetRange(eventDecl, tToken));
			}
		}
		else if (la.Type == Tokens.IDENT  || la.Type == Tokens.ARGLIST  || la.Type == Tokens.REFVALUE )
		{
			MemberName(out eventName);
			if (eventName != null)
			{
				eventDecl = CreateEvent(type, eventName);
				if (eventDecl != null)
				{
					OpenContext(eventDecl);
					eventDecl.SetRange(startRange);
					SetAccessSpecifiers(eventDecl, accessSpecifiers, visibility);
					ReadBlockStart(la.Range);
				}
			}
			Expect(Tokens.LBRACE );
			EventAccessorDeclarationsSeq();
			bool hasClosedBrace = la.Match(Tokens.RBRACE);
			Expect(Tokens.RBRACE );
			if (eventDecl != null)
			{
				ReadBlockEnd(tToken.Range);
				if (hasClosedBrace)
				  CloseContext();
				eventDecl.SetRange(GetRange(eventDecl, tToken));
			}
		}
		else
			SynErr(189);
		SetAttributes(eventDecl, attributes);
	}
	void MemberName(out ElementReferenceExpression refExpr)
	{
		ElementReferenceExpression currentExpression = null;
		TypeReferenceExpressionCollection typeArguments = null;
		refExpr = null;
		if (la.Type != Tokens.IDENT)
		{
			refExpr = new ElementReferenceExpression(String.Empty, SourceRange.Empty);
			refExpr.SetRange(SourceRange.Empty);
			return;
		}
		Name();
		if (la.Type != Tokens.DBLCOLON)
		{
			currentExpression = new ElementReferenceExpression(tToken.Value, tToken.Range);
			currentExpression.NameRange = tToken.Range;
			refExpr = currentExpression;
		}
		else
		{
			currentExpression = new QualifiedAliasExpression(tToken.Value, tToken.Range);
			if (tToken.Value == "global")
				(currentExpression as QualifiedAliasExpression).IsGlobal = true;
		}
		if (la.Type == Tokens.DBLCOLON )
		{
			Get();
			Name();
			refExpr = new QualifiedElementReference(currentExpression, tToken.Value, tToken.Range);
			refExpr.SetRange(GetRange(currentExpression, tToken));
			refExpr.AddNode(currentExpression);
			currentExpression = refExpr;
		}
		if (la.Type == Tokens.LT && IsPartOfMemberName())
		{
			TypeArgumentList(currentExpression, out typeArguments);
		}
		while (la.Type == Tokens.DOT )
		{
			Get();
			if (la.Type != Tokens.IDENT)
			return;
			Name();
			refExpr = new QualifiedElementReference(currentExpression, tToken.Value, tToken.Range);
			refExpr.SetRange(GetRange(currentExpression, tToken));
			refExpr.AddNode(currentExpression);
			currentExpression = refExpr;
			if (la.Type == Tokens.LT && IsPartOfMemberName())
			{
				TypeArgumentList(currentExpression, out typeArguments);
			}
		}
	}
	void EventAccessorDeclarationsSeq()
	{
		EventAccessorDeclarations();
		while (la.Type == Tokens.IDENT  || la.Type == Tokens.LBRACK )
		{
			EventAccessorDeclarations();
		}
	}
	void ImplicitLocalVariableDeclaration()
	{
		Expression  initializer = null;
		string name = String.Empty;
		SourceRange nameRange = SourceRange.Empty;
		ImplicitVariable variable = null;
		ImplicitVariable prevVariable = null;
		ImplicitVariable firstVariable = null;
		SourceRange startRange = la.Range;
		SourceRange operatorRange = SourceRange.Empty;
		SourceRange varRange = la.Range;
		 CorrectFormattingTokenType(la);
		Expect(Tokens.IDENT );
		Expect(Tokens.IDENT );
		name = tToken.Value; nameRange = tToken.Range;
		Expect(Tokens.ASSGN );
		operatorRange = tToken.Range;
		VariableInitializer(out initializer);
		variable = CreateImplicitVariable(name, initializer, nameRange, startRange, true, operatorRange, varRange);
		prevVariable = variable;
		firstVariable = variable;
		initializer = null;
		operatorRange = SourceRange.Empty;
		AddNode(variable); 
		while (la.Type == Tokens.COMMA )
		{
			Get();
			if (variable != null)
			variable.SetRange(GetRange(variable, tToken));
			name = la.Value;
			nameRange = la.Range;
			startRange = la.Range;
			Expect(Tokens.IDENT );
			Expect(Tokens.ASSGN );
			operatorRange = tToken.Range;
			VariableInitializer(out initializer);
			variable = CreateImplicitVariable(name, initializer, nameRange, startRange, false, operatorRange, varRange);
			variable.SetPreviousVariable(prevVariable);
			variable.SetAncestorVariable(firstVariable);
			prevVariable.SetNextVariable(variable);
			prevVariable = variable;
			initializer = null;
			operatorRange = SourceRange.Empty;
			AddNode(variable); 
		}
		Expect(Tokens.SCOLON );
		if (variable != null)
		variable.SetRange(GetRange(variable, tToken));
	}
	void ImplicitFixedFieldDecl()
	{
		Expression  initializer = null;
		string name = String.Empty;
		SourceRange nameRange = SourceRange.Empty;
		ImplicitVariable variable = null;
		ImplicitVariable prevVariable = null;
		ImplicitVariable firstVariable = null;
		SourceRange startRange = la.Range;
		SourceRange operatorRange = SourceRange.Empty;
		SourceRange varRange = la.Range;
		 CorrectFormattingTokenType(la);
		Expect(Tokens.IDENT );
		Expect(Tokens.IDENT );
		name = tToken.Value; nameRange = tToken.Range;
		Expect(Tokens.ASSGN );
		operatorRange = tToken.Range;
		VariableInitializer(out initializer);
		variable = CreateImplicitVariable(name, initializer, nameRange, startRange, true, operatorRange, varRange);
		prevVariable = variable;
		firstVariable = variable;
		initializer = null;
		operatorRange = SourceRange.Empty;
		AddDetailNode(variable); 
		while (la.Type == Tokens.COMMA )
		{
			Get();
			if (variable != null)
			variable.SetRange(GetRange(variable, tToken));
			name = la.Value;
			nameRange = la.Range;
			startRange = la.Range;
			Expect(Tokens.IDENT );
			Expect(Tokens.ASSGN );
			operatorRange = tToken.Range;
			VariableInitializer(out initializer);
			variable = CreateImplicitVariable(name, initializer, nameRange, startRange, false, operatorRange, varRange);
			variable.SetPreviousVariable(prevVariable);
			variable.SetAncestorVariable(firstVariable);
			prevVariable.SetNextVariable(variable);
			prevVariable = variable;
			initializer = null;
			operatorRange = SourceRange.Empty;
			AddDetailNode(variable); 
		}
		Expect(Tokens.SCOLON );
		if (variable != null)
		variable.SetRange(GetRange(variable, tToken));
	}
	void LocalVariableDeclaration()
	{
		TypeReferenceExpression typeRef = null;
		if (IsImplicitVariable())
		{
			ImplicitLocalVariableDeclaration();
		}
		else if (StartOf(14))
		{
			Type(out typeRef, true);
			FieldDecl(typeRef, null, MemberVisibility.Local, null);
		}
		else
			SynErr(190);
	}
	void ForVariableDeclaration()
	{
		TypeReferenceExpression typeRef = null;
		if (IsImplicitVariable())
		{
			ImplicitForFieldDecl();
		}
		else if (StartOf(14))
		{
			Type(out typeRef, true);
			ForFieldDecl(typeRef);
		}
		else
			SynErr(191);
	}
	void UsingVariableDeclaration()
	{
		TypeReferenceExpression typeRef = null;
		if (IsImplicitVariable())
		{
			ImplicitUsingFieldDecl();
		}
		else if (StartOf(14))
		{
			Type(out typeRef, true);
			UsingFieldDecl(typeRef);
		}
		else
			SynErr(192);
	}
	void Argument(out Expression expression, bool supportsNamedArgument)
	{
		expression = null;
		Expression argument = null;
		Expression init = null;
		SourceRange directionRange = la.Range;
		ArgumentDirection direction = ArgumentDirection.In;
		Token token = la;
		if (la.Type == Tokens.OUT  || la.Type == Tokens.REF )
		{
			if (la.Type == Tokens.REF )
			{
				Get();
				direction = ArgumentDirection.Ref;
			}
			else
			{
				Get();
				direction = ArgumentDirection.Out;
			}
		}
		Expression(out argument);
		if (la.Type == Tokens.COLON && supportsNamedArgument)
		{
			Expect(Tokens.COLON );
			token = tToken; 
			if (la.Type == Tokens.OUT  || la.Type == Tokens.REF )
			{
				if (la.Type == Tokens.REF )
				{
					Get();
					direction = ArgumentDirection.Ref;
				}
				else
				{
					Get();
					direction = ArgumentDirection.Out;
				}
			}
			Expression(out init);
		}
		if (init != null && argument != null)
		{
		  SourceRange range = GetRange(argument, init);
		  argument = new AttributeVariableInitializer(argument, token, init);
		  argument.SetRange(range);
		}
		if (direction != ArgumentDirection.In)
		{
		 if (argument != null)
		 {
		  expression = new ArgumentDirectionExpression(direction, argument);
		  expression.NameRange = directionRange;
		  expression.SetRange(GetRange(directionRange, argument));
		 }
		}
		else
		 expression = argument;
	}
	void ArrayInitializer(out ArrayInitializerExpression initializer)
	{
		initializer = new ArrayInitializerExpression();
		initializer.SetRange(la.Range);
		Expression expression = null;
		Expect(Tokens.LBRACE );
		if (StartOf(21))
		{
			VariableInitializer(out expression);
			if (expression != null)
			{
				initializer.Initializers.Add(expression);
				initializer.AddDetailNode(expression);
			}
			while (la.Type == Tokens.COMMA )
			{
				Get();
				VariableInitializer(out expression);
				if (expression != null)
				{
					initializer.Initializers.Add(expression);
					initializer.AddDetailNode(expression);
				}
			}
			if (la.Type == Tokens.COMMA )
			{
				Get();
			}
		}
		Expect(Tokens.RBRACE );
		initializer.SetRange(GetRange(initializer, tToken));
	}
	void ParameterDeclaration(out Param parameter, out LanguageElementCollection attributes)
	{
		TypeReferenceExpression typeRef = null;
		parameter = null;
		ArgumentDirection direction = ArgumentDirection.In;
		attributes = null;
		bool isExtensionMethodParameter = false;
		Expression defaultValue = null;
		bool isOptional = false;
		Token nameToken = la;
		AttributeSections(out attributes);
		SourceRange paramRange = la.Range;
		if (la.Type == Tokens.ARGLIST )
		{
			Get();
			direction = ArgumentDirection.ArgList;
		}
		else if (StartOf(22))
		{
			if (la.Type == Tokens.OUT  || la.Type == Tokens.PARAMS  || la.Type == Tokens.REF )
			{
				if (la.Type == Tokens.REF )
				{
					Get();
					direction = ArgumentDirection.Ref;
				}
				else if (la.Type == Tokens.OUT )
				{
					Get();
					direction = ArgumentDirection.Out;
				}
				else
				{
					Get();
					direction = ArgumentDirection.ParamArray;
				}
			}
			if (la.Type == Tokens.THIS )
			{
				Get();
				isExtensionMethodParameter = true;
			}
			Type(out typeRef, true);
			nameToken = la; 
			Name();
			if (la.Type == Tokens.ASSGN )
			{
				Get();
				Expression(out defaultValue);
				isOptional = true; 
			}
		}
		else
			SynErr(193);
		paramRange = GetRange(paramRange, tToken);
		if (isExtensionMethodParameter)
			parameter = CreateExtensionMethodParameter(nameToken, typeRef, direction, paramRange);
		else
			parameter = CreateParameter(nameToken, typeRef, direction, paramRange);
		if (parameter != null)
		{
		  parameter.IsOptional = isOptional;
		  if (isOptional)
		  {
			parameter.DefaultValueExpression = defaultValue;
			if (defaultValue != null)
			{
			   parameter.DefaultValue = defaultValue.ToString();
			   parameter.AddDetailNode(defaultValue);
			 }
		  }
		}
	}
	void Name()
	{
		if (la.Type == Tokens.IDENT )
		{
			Get();
			if (SetTokensCategory && tToken.Value == "value")
			{
			  LanguageElement context = Context;
			  while (context != null)
			  {
				LanguageElementType type = context.ElementType;
				if (type == LanguageElementType.PropertyAccessorSet ||
					type == LanguageElementType.EventAdd ||
					type == LanguageElementType.EventRemove)
				{
				  CorrectFormattingTokenType(tToken);
				  break;
				}
				context = context.Parent;
			  }
			}
			 else if (tToken.Value == "partial")
			   CorrectFormattingTokenType(tToken);
		}
		else if (la.Type == Tokens.ARGLIST )
		{
			Get();
		}
		else if (la.Type == Tokens.REFVALUE )
		{
			Get();
		}
		else
			SynErr(194);
	}
	void EventAccessorDeclarations()
	{
		EventAccessor accessor = null;
		SourceRange startRange = la.Range;
		LanguageElementCollection attributes = null;
		AttributeSections(out attributes);
		startRange = la.Range;
		if (la.Type == Tokens.IDENT && la.Value == "add")
		{
			Expect(Tokens.IDENT );
			accessor = new EventAdd();
			SetLastFormattingType(FormattingTokenType.Add);
			AddImplicitParamToAccessor(Context as Member, accessor);
		}
		else if (la.Type == Tokens.IDENT )
		{
			Get();
			accessor = new EventRemove();
			SetLastFormattingType(FormattingTokenType.Remove);
			AddImplicitParamToAccessor(Context as Member, accessor);
		}
		else
			SynErr(195);
		if (accessor != null)
		{
			CorrectFormattingTokenType(tToken);
			accessor.Name = GetAccessorName(tToken.Value + "_");
			accessor.NameRange = tToken.Range;
			OpenContext(accessor);
			if (attributes != null)
				accessor.SetAttributes(attributes);
			accessor.SetRange(startRange);
		}
		if (la.Type == Tokens.LBRACE )
		{
			BlockCore(false);
		}
		else if (la.Type == Tokens.SCOLON )
		{
			Get();
		}
		else
			SynErr(196);
		if (accessor != null)
		{
			accessor.SetRange(GetRange(accessor, tToken));
			CloseContext();
		}
	}
	void Attributes(out AttributeSection attrSection)
	{
		StructuralParser.Attribute attributeDef = null;
		attrSection = new AttributeSection();
		AttributeTargetType targetType = AttributeTargetType.None;
		SourceRange targetSpecifierRange = SourceRange.Empty;
		attrSection.SetRange(la.Range);
		Expect(Tokens.LBRACK );
		if (IsAttrTargSpec())
		{
			if (la.Type == Tokens.IDENT )
			{
				Get();
			}
			else if (StartOf(23))
			{
				Keyword();
			}
			else
				SynErr(197);
			targetType = StructuralParser.Attribute.GetTargetTypeFromName(tToken.Value);
			targetSpecifierRange = tToken.Range;
			Expect(Tokens.COLON );
		}
		Attribute(out attributeDef);
		if (attributeDef != null && attrSection != null)
		{
			attrSection.AttributeCollection.Add(attributeDef);
			attrSection.AddDetailNode(attributeDef);
			attributeDef.TargetType = targetType;
			if (targetSpecifierRange != SourceRange.Empty)
					attributeDef.SetRange(GetRange(targetSpecifierRange, attributeDef));
		}
		while (la.Type == Tokens.COMMA && Peek().Type != Tokens.RBRACK)
		{
			Expect(Tokens.COMMA );
			Attribute(out attributeDef);
			if (attributeDef != null && attrSection != null)
			{
				attrSection.AttributeCollection.Add(attributeDef);
				attrSection.AddDetailNode(attributeDef);
			}
		}
		if (la.Type == Tokens.COMMA )
		{
			Get();
		}
		Expect(Tokens.RBRACK );
		attrSection.SetRange(GetRange(attrSection, tToken));
	}
	void Attribute(out StructuralParser.Attribute attributeDef)
	{
		TypeReferenceExpression typeRef = null;
		attributeDef = null;
		TypeName(out typeRef);
		if (typeRef != null)
		{
			attributeDef = new StructuralParser.Attribute();
			attributeDef.Name = typeRef.Name;
			attributeDef.NameRange = typeRef.NameRange;
			attributeDef.Qualifier = typeRef.Qualifier;
			attributeDef.SetRange(typeRef.Range);
		}
		if (la.Type == Tokens.LPAR )
		{
			AttributeArguments(attributeDef);
		}
		if (attributeDef != null)
		attributeDef.SetRange(GetRange(attributeDef, tToken));
	}
	void Keyword()
	{
		switch (la.Type)
		{
		case Tokens.ABSTRACT : 
		{
			Get();
			break;
		}
		case Tokens.AS : 
		{
			Get();
			break;
		}
		case Tokens.BASE : 
		{
			Get();
			break;
		}
		case Tokens.BOOL : 
		{
			Get();
			break;
		}
		case Tokens.BREAK : 
		{
			Get();
			break;
		}
		case Tokens.BYTE : 
		{
			Get();
			break;
		}
		case Tokens.CASE : 
		{
			Get();
			break;
		}
		case Tokens.CATCH : 
		{
			Get();
			break;
		}
		case Tokens.CHAR : 
		{
			Get();
			break;
		}
		case Tokens.CHECKED : 
		{
			Get();
			break;
		}
		case Tokens.CLASS : 
		{
			Get();
			break;
		}
		case Tokens.CONST : 
		{
			Get();
			break;
		}
		case Tokens.CONTINUE : 
		{
			Get();
			break;
		}
		case Tokens.DECIMAL : 
		{
			Get();
			break;
		}
		case Tokens.DEFAULT : 
		{
			Get();
			break;
		}
		case Tokens.DELEGATE : 
		{
			Get();
			break;
		}
		case Tokens.DO : 
		{
			Get();
			break;
		}
		case Tokens.DOUBLE : 
		{
			Get();
			break;
		}
		case Tokens.ELSE : 
		{
			Get();
			break;
		}
		case Tokens.ENUM : 
		{
			Get();
			break;
		}
		case Tokens.EVENT : 
		{
			Get();
			break;
		}
		case Tokens.EXPLICIT : 
		{
			Get();
			break;
		}
		case Tokens.EXTERN : 
		{
			Get();
			break;
		}
		case Tokens.FALSE : 
		{
			Get();
			break;
		}
		case Tokens.FINALLY : 
		{
			Get();
			break;
		}
		case Tokens.FIXED : 
		{
			Get();
			break;
		}
		case Tokens.FLOAT : 
		{
			Get();
			break;
		}
		case Tokens.FOR : 
		{
			Get();
			break;
		}
		case Tokens.FOREACH : 
		{
			Get();
			break;
		}
		case Tokens.GOTO : 
		{
			Get();
			break;
		}
		case Tokens.IFCLAUSE : 
		{
			Get();
			break;
		}
		case Tokens.IMPLICIT : 
		{
			Get();
			break;
		}
		case Tokens.IN : 
		{
			Get();
			break;
		}
		case Tokens.INT : 
		{
			Get();
			break;
		}
		case Tokens.INTERFACE : 
		{
			Get();
			break;
		}
		case Tokens.INTERNAL : 
		{
			Get();
			break;
		}
		case Tokens.IS : 
		{
			Get();
			break;
		}
		case Tokens.LOCK : 
		{
			Get();
			break;
		}
		case Tokens.LONG : 
		{
			Get();
			break;
		}
		case Tokens.NAMESPACE : 
		{
			Get();
			break;
		}
		case Tokens.NEW : 
		{
			Get();
			break;
		}
		case Tokens.NULL : 
		{
			Get();
			break;
		}
		case Tokens.OPERATOR : 
		{
			Get();
			break;
		}
		case Tokens.OUT : 
		{
			Get();
			break;
		}
		case Tokens.OVERRIDE : 
		{
			Get();
			break;
		}
		case Tokens.PARAMS : 
		{
			Get();
			break;
		}
		case Tokens.PRIVATE : 
		{
			Get();
			break;
		}
		case Tokens.PROTECTED : 
		{
			Get();
			break;
		}
		case Tokens.PUBLIC : 
		{
			Get();
			break;
		}
		case Tokens.READONLY : 
		{
			Get();
			break;
		}
		case Tokens.REF : 
		{
			Get();
			break;
		}
		case Tokens.RETURN : 
		{
			Get();
			break;
		}
		case Tokens.SBYTE : 
		{
			Get();
			break;
		}
		case Tokens.SEALED : 
		{
			Get();
			break;
		}
		case Tokens.SHORT : 
		{
			Get();
			break;
		}
		case Tokens.SIZEOF : 
		{
			Get();
			break;
		}
		case Tokens.STACKALLOC : 
		{
			Get();
			break;
		}
		case Tokens.STATIC : 
		{
			Get();
			break;
		}
		case Tokens.STRUCT : 
		{
			Get();
			break;
		}
		case Tokens.SWITCH : 
		{
			Get();
			break;
		}
		case Tokens.THIS : 
		{
			Get();
			break;
		}
		case Tokens.THROW : 
		{
			Get();
			break;
		}
		case Tokens.TRUE : 
		{
			Get();
			break;
		}
		case Tokens.TRY : 
		{
			Get();
			break;
		}
		case Tokens.TYPEOF : 
		{
			Get();
			break;
		}
		case Tokens.UINT : 
		{
			Get();
			break;
		}
		case Tokens.ULONG : 
		{
			Get();
			break;
		}
		case Tokens.UNCHECKED : 
		{
			Get();
			break;
		}
		case Tokens.UNSAFE : 
		{
			Get();
			break;
		}
		case Tokens.USHORT : 
		{
			Get();
			break;
		}
		case Tokens.USINGKW : 
		{
			Get();
			break;
		}
		case Tokens.VIRTUAL : 
		{
			Get();
			break;
		}
		case Tokens.VOID : 
		{
			Get();
			break;
		}
		case Tokens.VOLATILE : 
		{
			Get();
			break;
		}
		case Tokens.WHILE : 
		{
			Get();
			break;
		}
		default: SynErr(198); break;
		}
	}
	void AttributeArguments(Attribute attributeDef)
	{
		ExpressionCollection arguments = null;
		SourceRange startParensRange = la.Range;
		Expect(Tokens.LPAR );
		if (StartOf(17))
		{
			ArgumentCollection(out arguments);
		}
		Expect(Tokens.RPAR );
		attributeDef.ParensRange = GetRange(startParensRange, tToken.Range);
		if (arguments == null || arguments.Count == 0 || attributeDef == null)
		  return;
		for (int i = 0; i < arguments.Count; i++)
		{
		  Expression argument = arguments[i];
		  if (argument == null)
			continue;
		  attributeDef.Arguments.Add(argument);
		attributeDef.AddDetailNode(argument);
		 }
	}
	void ModifierListCore(ref AccessSpecifiers accessSpecifiers, ref MemberVisibility visibility, ref bool wasProtectedInternal, ref bool wasInternalProtected)
	{
		while (StartOf(24))
		{
			switch (la.Type)
			{
			case Tokens.NEW : 
			{
				Get();
				accessSpecifiers.IsNew = true; accessSpecifiers.SetNewRange(tToken.Range);
				break;
			}
			case Tokens.PUBLIC : 
			{
				Get();
				visibility = MemberVisibility.Public; accessSpecifiers.SetVisibilityRange(tToken.Range);
				break;
			}
			case Tokens.PROTECTED : 
			{
				Get();
				if (visibility == MemberVisibility.Internal && !wasInternalProtected)
				{
				  visibility = MemberVisibility.ProtectedInternal; 
				  accessSpecifiers.SetVisibilityRange(GetRange(accessSpecifiers.VisibilityRange,tToken.Range));
				  wasInternalProtected = true;
				}
				else if (visibility != MemberVisibility.ProtectedInternal || wasInternalProtected)
				{
				  visibility = MemberVisibility.Protected; 
				  wasInternalProtected = false;
				  accessSpecifiers.SetVisibilityRange(tToken.Range);
				}
				break;
			}
			case Tokens.INTERNAL : 
			{
				Get();
				if (visibility == MemberVisibility.Protected && !wasProtectedInternal)
				{
				  visibility = MemberVisibility.ProtectedInternal; 
				  accessSpecifiers.SetVisibilityRange(GetRange(accessSpecifiers.VisibilityRange,tToken.Range));
				  wasProtectedInternal = true;
				}
				else if (visibility != MemberVisibility.ProtectedInternal || wasProtectedInternal)
				{
				  visibility = MemberVisibility.Internal; 
				  accessSpecifiers.SetVisibilityRange(tToken.Range);
				}
				break;
			}
			case Tokens.PRIVATE : 
			{
				Get();
				visibility = MemberVisibility.Private; accessSpecifiers.SetVisibilityRange(tToken.Range);
				break;
			}
			case Tokens.UNSAFE : 
			{
				Get();
				accessSpecifiers.IsUnsafe = true; accessSpecifiers.SetUnsafeRange(tToken.Range);
				break;
			}
			case Tokens.STATIC : 
			{
				Get();
				accessSpecifiers.IsStatic = true; accessSpecifiers.SetStaticRange(tToken.Range);
				break;
			}
			case Tokens.READONLY : 
			{
				Get();
				accessSpecifiers.IsReadOnly = true; accessSpecifiers.SetReadOnlyRange(tToken.Range);
				break;
			}
			case Tokens.VOLATILE : 
			{
				Get();
				accessSpecifiers.IsVolatile = true; accessSpecifiers.SetVolatileRange(tToken.Range);
				break;
			}
			case Tokens.VIRTUAL : 
			{
				Get();
				accessSpecifiers.IsVirtual = true; accessSpecifiers.SetVirtualRange(tToken.Range); accessSpecifiers.SetVirtualOverrideAbstractRange(tToken.Range);
				break;
			}
			case Tokens.SEALED : 
			{
				Get();
				accessSpecifiers.IsSealed = true; accessSpecifiers.SetSealedRange(tToken.Range);
				break;
			}
			case Tokens.OVERRIDE : 
			{
				Get();
				accessSpecifiers.IsOverride = true; accessSpecifiers.SetOverrideRange(tToken.Range); accessSpecifiers.SetVirtualOverrideAbstractRange(tToken.Range);
				break;
			}
			case Tokens.ABSTRACT : 
			{
				Get();
				accessSpecifiers.IsAbstract = true; accessSpecifiers.SetAbstractRange(tToken.Range); accessSpecifiers.SetVirtualOverrideAbstractRange(tToken.Range);
				break;
			}
			case Tokens.EXTERN : 
			{
				Get();
				accessSpecifiers.IsExtern = true; accessSpecifiers.SetExternRange(tToken.Range);
				break;
			}
			}
		}
	}
	void SimpleType(out TypeReferenceExpression type, bool ignoreTypeCastChecks)
	{
		type = null;
		if (StartOf(25))
		{
			PrimitiveLanguageType(out type);
		}
		else if (la.Type == Tokens.IDENT  || la.Type == Tokens.ARGLIST  || la.Type == Tokens.REFVALUE )
		{
			ClassType(out type);
		}
		else
			SynErr(199);
		if (la.Type == Tokens.QUESTION && (ignoreTypeCastChecks || !IsConditionalExpressionStart()))
		{
			Expect(Tokens.QUESTION );
			if (type != null)
			{
				TypeReferenceExpression oldType = type;
				type = new TypeReferenceExpression(oldType);
				type.NameRange = tToken.Range;
				type.IsNullable = true;
				type.SetRange(GetRange(oldType, tToken));
				type.TypeReferenceType = TypeReferenceType.None;
			}
		}
	}
	void PrimitiveLanguageType(out TypeReferenceExpression type)
	{
		type = new TypeReferenceExpression(la.Value, la.Range);
		type.NameRange = la.Range;
		switch (la.Type)
		{
		case Tokens.SBYTE : 
		{
			Get();
			SetLastFormattingType(FormattingTokenType.Ident); 
			break;
		}
		case Tokens.BYTE : 
		{
			Get();
			SetLastFormattingType(FormattingTokenType.Ident); 
			break;
		}
		case Tokens.SHORT : 
		{
			Get();
			SetLastFormattingType(FormattingTokenType.Ident); 
			break;
		}
		case Tokens.USHORT : 
		{
			Get();
			SetLastFormattingType(FormattingTokenType.Ident); 
			break;
		}
		case Tokens.INT : 
		{
			Get();
			SetLastFormattingType(FormattingTokenType.Ident); 
			break;
		}
		case Tokens.UINT : 
		{
			Get();
			SetLastFormattingType(FormattingTokenType.Ident); 
			break;
		}
		case Tokens.LONG : 
		{
			Get();
			SetLastFormattingType(FormattingTokenType.Ident); 
			break;
		}
		case Tokens.ULONG : 
		{
			Get();
			SetLastFormattingType(FormattingTokenType.Ident); 
			break;
		}
		case Tokens.CHAR : 
		{
			Get();
			SetLastFormattingType(FormattingTokenType.Ident); 
			break;
		}
		case Tokens.FLOAT : 
		{
			Get();
			SetLastFormattingType(FormattingTokenType.Ident); 
			break;
		}
		case Tokens.DOUBLE : 
		{
			Get();
			SetLastFormattingType(FormattingTokenType.Ident); 
			break;
		}
		case Tokens.DECIMAL : 
		{
			Get();
			SetLastFormattingType(FormattingTokenType.Ident); 
			break;
		}
		case Tokens.BOOL : 
		{
			Get();
			SetLastFormattingType(FormattingTokenType.Ident); 
			break;
		}
		case Tokens.VOID : 
		{
			Get();
			break;
		}
		default: SynErr(200); break;
		}
	}
	void TypeArgumentList(ReferenceExpressionBase reference, out TypeReferenceExpressionCollection typeArguments)
	{
		TypeReferenceExpression typeRef = null;
		typeArguments = new TypeReferenceExpressionCollection();
		int typeArity = 1;
		Expect(Tokens.LT );
		if (StartOf(14))
		{
			Type(out typeRef, true);
			if (typeRef != null)
			typeArguments.Add(typeRef);
		}
		while (la.Type == Tokens.COMMA )
		{
			Get();
			typeArity ++; 
			if (StartOf(14))
			{
				Type(out typeRef, true);
				if (typeRef != null)
				typeArguments.Add(typeRef);
			}
		}
		Expect(Tokens.GT );
		if (reference != null)
		{
			reference.SetRange(GetRange(reference, tToken));
			if (typeArguments != null && typeArguments.Count > 0)
				SetTypeArguments(reference, typeArguments);
			else if (reference is TypeReferenceExpression)
			{
				TypeReferenceExpression typeReference = (TypeReferenceExpression)reference;
				typeReference.IsUnbound = true;
				typeReference.TypeArity = typeArity;
			}
		}
	}
	void PointerOrArray(ref TypeReferenceExpression type)
	{
		TypeReferenceExpression baseType = type; 
		if (la.Type == Tokens.TIMES )
		{
			Pointer(ref type);
		}
		else if (la.Type == Tokens.LBRACK )
		{
			Get();
			int rank = 1; SourceRange openBracketRange = tToken.Range;
			while (la.Type == Tokens.COMMA )
			{
				Get();
				rank++;
			}
			Expect(Tokens.RBRACK );
			type = new TypeReferenceExpression(baseType, rank);
			type.NameRange = GetRange(openBracketRange, tToken.Range);
			type.SetRange(GetRange(openBracketRange, tToken.Range));
			  if (baseType != null)
			  type.SetRange(GetRange(baseType, type));
		}
		else
			SynErr(201);
	}
	void ResolvedType(out TypeReferenceExpression type)
	{
		type = null;
		SimpleType(out type, false);
		while (la.Type == Tokens.LBRACK  || la.Type == Tokens.TIMES )
		{
			PointerOrArray(ref type);
		}
	}
	void Pointer(ref TypeReferenceExpression type)
	{
		TypeReferenceExpression baseType = type; 
		Expect(Tokens.TIMES );
		type = new TypeReferenceExpression(TypeReferenceType.Pointer);
		type.NameRange = tToken.Range;
		type.SetRange(tToken.Range);
		if (baseType != null)
		{
			type.SetBaseTypeAfterCreation(baseType);
			type.SetRange(GetRange(baseType, type));
		}
	}
	void RazorHtmlCode(out LanguageElementCollection result)
	{
		result = null;
		if (!_ParsingRazor)
		{
		  Get();
		  return;
		}
		ParseRazorHtmlEmbedding(out result);
		if (la.Type == Tokens.LT )
		{
			Get();
		}
		else if (la.Type == Tokens.IDENT )
		{
			Get();
		}
		else if (la.Type == Tokens.AT )
		{
			Get();
		}
		else if (la.Type == Tokens.ATCOLON )
		{
			Get();
		}
		else
			SynErr(202);
	}
	void EmbeddedStatement()
	{
		TypeReferenceExpression typeRef = null;
		Expression expression = null;
		if (la.Type == Tokens.SCOLON )
		{
			Get();
			Statement empty = new Statement(tToken.Value);
			empty.SetRange(tToken.Range);
			AddNode(empty);
		}
		else if (IsYieldStatement())
		{
			Expect(Tokens.IDENT );
			CorrectFormattingTokenType(tToken);
			SourceRange yieldRange = tToken.Range;
			Yield yieldStatement = null;
			if (la.Type == Tokens.RETURN )
			{
				Get();
				Expression(out expression);
				YieldReturn yieldReturn = new YieldReturn();
				if (expression != null)
				yieldReturn.Expression = expression;
				yieldStatement = yieldReturn;
			}
			else if (la.Type == Tokens.BREAK )
			{
				Get();
				yieldStatement = new YieldBreak();
			}
			else
				SynErr(203);
			Expect(Tokens.SCOLON );
			if (yieldStatement != null)
			{
				yieldStatement.SetRange(GetRange(yieldRange, tToken));
				AddNode(yieldStatement);
			}
		}
		else if (la.Type == Tokens.CHECKED && scanner.Peek().Type == Tokens.LBRACE)
		{
			Expect(Tokens.CHECKED );
			Checked checkedBlock = new Checked();
			OpenContext(checkedBlock);
			checkedBlock.SetRange(tToken.Range);
			BlockCore(false);
			CloseContext();
			checkedBlock.SetRange(GetRange(checkedBlock, tToken.Range));
		}
		else if (la.Type == Tokens.UNCHECKED && scanner.Peek().Type == Tokens.LBRACE)
		{
			Expect(Tokens.UNCHECKED );
			Unchecked uncheckedBlock = new Unchecked();
			OpenContext(uncheckedBlock);
			uncheckedBlock.SetRange(tToken.Range);
			BlockCore(false);
			CloseContext();
			uncheckedBlock.SetRange(GetRange(uncheckedBlock, tToken.Range));
		}
		else if (StartOf(17))
		{
			StatementExpression();
		}
		else if (la.Type == Tokens.SWITCH )
		{
			Get();
			Switch switchClause = new Switch(); 
			switchClause.SetRange(tToken.Range);
			OpenContext(switchClause);
			Token lParToken = la;
			Expect(Tokens.LPAR );
			Expression(out expression);
			if (switchClause != null && expression != null)
			switchClause.Expression = expression;
			SkipTo(Tokens.RPAR);
			if (expression != null)
			  expression.SetRange(GetRange(expression, tToken.Range));
			Expect(Tokens.RPAR );
			switchClause.SetParensRange(lParToken, tToken);
			switchClause.HasBlock = true;
			ReadBlockStart(la.Range);
			Expect(Tokens.LBRACE );
			while (la.Type == Tokens.CASE  || la.Type == Tokens.DEFAULT )
			{
				SwitchSection();
			}
			Expect(Tokens.RBRACE );
			ReadBlockEnd(tToken.Range);
			if (la.Type == Tokens.SCOLON )
			{
				Get();
			}
			CloseContextAndSetRange(switchClause);			
		}
		else if (la.Type == Tokens.WHILE )
		{
			Get();
			While whileStatement = new While();
			whileStatement.SetRange(tToken.Range);
			OpenContext(whileStatement);
			Token lParToken = la;
			Expect(Tokens.LPAR );
			Expression(out expression);
			if (whileStatement != null && expression != null)
			whileStatement.SetCondition(expression);
			SkipTo(Tokens.RPAR);
			if (expression != null)
			  expression.SetRange(GetRange(expression, tToken.Range));
			Expect(Tokens.RPAR );
			whileStatement.SetParensRange(lParToken, tToken);
			Statement();
			CloseContextAndSetRange(whileStatement);
		}
		else if (la.Type == Tokens.DO )
		{
			Get();
			Do doStatement = new Do();
			doStatement.SetRange(tToken.Range);
			OpenContext(doStatement);
			Statement();
			Expect(Tokens.WHILE );
			Token lParToken = la;
			Expect(Tokens.LPAR );
			Expression(out expression);
			if (doStatement != null && expression != null)
			doStatement.SetCondition(expression);
			SkipTo(Tokens.RPAR);
			if (expression != null)
			  expression.SetRange(GetRange(expression, tToken.Range));
			Expect(Tokens.RPAR );
			doStatement.SetParensRange(lParToken, tToken);
			Expect(Tokens.SCOLON );
			CloseContextAndSetRange(doStatement);
		}
		else if (la.Type == Tokens.FOR )
		{
			Get();
			For forStatement = new For();
			OpenContext(forStatement);
			forStatement.SetRange(tToken.Range);
			Token lParToken = la;
			Expect(Tokens.LPAR );
			if (StartOf(17))
			{
				ForInitializer();
			}
			Expect(Tokens.SCOLON );
			if (StartOf(17))
			{
				Expression(out expression);
				if (expression != null && forStatement != null)
				{
					forStatement.Condition = expression;
					forStatement.AddDetailNode(expression);
				}
			}
			Expect(Tokens.SCOLON );
			if (StartOf(17))
			{
				ForIterator();
			}
			SkipTo(Tokens.RPAR);
			Expect(Tokens.RPAR );
			forStatement.SetParensRange(lParToken, tToken);
			Statement();
			CloseContextAndSetRange(forStatement);
		}
		else if (la.Type == Tokens.FOREACH )
		{
			Get();
			ForEach forEach = new ForEach();
			forEach.SetRange(tToken.Range);
			OpenContext(forEach);
			Token lParToken = la;
			string typeName = String.Empty;
			  bool isImplicitVar = false;
			Expect(Tokens.LPAR );
			typeName  = la.Value;
			isImplicitVar = IsImplicitVariable();
			if (isImplicitVar)
			  CorrectFormattingTokenType(la);
			Type(out typeRef, true);
			Expect(Tokens.IDENT );
			if (forEach != null && typeRef != null)
			{
				Variable variable = null;
				if (isImplicitVar)
				{
					variable = CreateImplicitVariable(tToken.Value, tToken.Range, typeRef.Range, true);
					variable.TypeRange = typeRef.Range;
					forEach.FieldType = "unknown";
				}
				else
				{
					variable = CreateVariable(tToken.Value, typeRef, null, tToken.Range, typeRef.Range, true, SourceRange.Empty);
					forEach.FieldType = GlobalStringStorage.Intern(typeRef.ToString());
				}
				variable.SetRange(GetRange(typeRef, tToken));
				forEach.SetLoopVariable(variable);
				variable.Visibility = MemberVisibility.Local;
				forEach.Field = tToken.Value;
			}
			Expect(Tokens.IN );
			Expression(out expression);
			if (forEach != null && expression != null)
			{
				forEach.Expression = expression;
				forEach.Collection = GlobalStringStorage.Intern(expression.ToString());
			}
			SkipTo(Tokens.RPAR);
			 if (expression != null)
			   expression.SetRange(GetRange(expression, tToken.Range));
			Expect(Tokens.RPAR );
			forEach.SetParensRange(lParToken, tToken);
			Statement();
			CloseContextAndSetRange(forEach);
		}
		else if (la.Type == Tokens.BREAK )
		{
			Get();
			SourceRange endRange = tToken.Range;
			if (la.Type == Tokens.SCOLON)
				endRange = la.Range;
			Break breakStatement = new Break();
			breakStatement.SetRange(GetRange(tToken, endRange));
			AddNode(breakStatement);
			Expect(Tokens.SCOLON );
		}
		else if (la.Type == Tokens.CONTINUE )
		{
			Get();
			SourceRange endRange = tToken.Range;
			if (la.Type == Tokens.SCOLON)
				endRange = la.Range;
			Continue continueStatement = new Continue();
			continueStatement.SetRange(GetRange(tToken, endRange));
			AddNode(continueStatement);
			Expect(Tokens.SCOLON );
		}
		else if (la.Type == Tokens.GOTO )
		{
			Get();
			Goto gotoStatement = new Goto();
			gotoStatement.SetRange(tToken.Range);
			if (la.Type == Tokens.IDENT )
			{
				Get();
				gotoStatement.Label = tToken.Value;
				gotoStatement.LabelRange = tToken.Range;
			}
			else if (la.Type == Tokens.CASE )
			{
				Get();
				Expression(out expression);
				gotoStatement.IsGotoCaseLabel = true;
				if (expression != null)
				{
					gotoStatement.Label = GlobalStringStorage.Intern(expression.ToString());
					gotoStatement.LabelRange = expression.Range;
				}
			}
			else if (la.Type == Tokens.DEFAULT )
			{
				Get();
				gotoStatement.Label = tToken.Value;
				gotoStatement.LabelRange = tToken.Range;
				gotoStatement.IsGotoCaseDefault = true;
			}
			else
				SynErr(204);
			Expect(Tokens.SCOLON );
			gotoStatement.SetRange(GetRange(gotoStatement, tToken));
			AddNode(gotoStatement);
		}
		else if (la.Type == Tokens.RETURN )
		{
			Get();
			Return returnStatement = new Return();
			returnStatement.SetRange(tToken.Range);
			AddNode(returnStatement);
			if (StartOf(17))
			{
				Expression(out expression);
				if (expression != null)
				{
					returnStatement.Expression = expression;
					returnStatement.AddDetailNode(expression);
				}
			}
			Expect(Tokens.SCOLON );
			returnStatement.SetRange(GetRange(returnStatement, tToken));
		}
		else if (la.Type == Tokens.THROW )
		{
			Get();
			Throw throwStatement = new Throw();
			throwStatement.SetRange(tToken.Range);
			AddNode(throwStatement);
			if (StartOf(17))
			{
				Expression(out expression);
				if (expression != null)
				{
					throwStatement.Expression = expression;
					throwStatement.AddDetailNode(expression);
				}
			}
			Expect(Tokens.SCOLON );
			throwStatement.SetRange(GetRange(throwStatement, tToken));
		}
		else if (la.Type == Tokens.TRY )
		{
			TryCatchFinallyBlock();
		}
		else if (la.Type == Tokens.LOCK )
		{
			Get();
			Lock lockStatement = new Lock();
			lockStatement.SetRange(tToken.Range);
			OpenContext(lockStatement);
			Token lParToken = la;
			Expect(Tokens.LPAR );
			Expression(out expression);
			if (expression != null && lockStatement != null)
			{
				lockStatement.Expression = expression;
				lockStatement.AddDetailNode(expression);
			}
			SkipTo(Tokens.RPAR);
			 if (expression != null)
			   expression.SetRange(GetRange(expression, tToken.Range));
			Expect(Tokens.RPAR );
			lockStatement.SetParensRange(lParToken, tToken);
			Statement();
			CloseContextAndSetRange(lockStatement);
		}
		else if (la.Type == Tokens.USINGKW )
		{
			Get();
			UsingStatement usingStatement = new UsingStatement();
			usingStatement.SetRange(tToken.Range);
			OpenContext(usingStatement);
			Token lParToken = la;
			Expect(Tokens.LPAR );
			ResourceAcquisition();
			SkipTo(Tokens.RPAR);
			Expect(Tokens.RPAR );
			usingStatement.SetParensRange(lParToken, tToken);
			Statement();
			CloseContextAndSetRange(usingStatement);		
		}
		else if (la.Type == Tokens.UNSAFE )
		{
			Get();
			UnsafeStatement unsafeStatement = new UnsafeStatement();
			unsafeStatement.SetRange(tToken.Range);
			OpenContext(unsafeStatement);
			BlockCore(false);
			CloseContextAndSetRange(unsafeStatement);		
		}
		else if (la.Type == Tokens.FIXED )
		{
			Get();
			Fixed fixedStatement = new Fixed();
			fixedStatement.SetRange(tToken.Range);
			OpenContext(fixedStatement);
			Token lParToken = la;
			Expect(Tokens.LPAR );
			if (IsImplicitVariable())
			{
				ImplicitFixedFieldDecl();
			}
			else if (StartOf(14))
			{
				Type(out typeRef, true);
				FixedFieldDecl(typeRef);
			}
			else
				SynErr(205);
			SkipTo(Tokens.RPAR);
			Expect(Tokens.RPAR );
			fixedStatement.SetParensRange(lParToken, tToken);
			Statement();
			CloseContextAndSetRange(fixedStatement);		
		}
		else
			SynErr(206);
	}
	void StatementExpression()
	{
		Expression expression = null;
		Statement statement = null;
		NestedMethod aspContext = Context as NestedMethod;
		Token testToken = tToken; 
		Expression(out expression);
		Expect(Tokens.SCOLON );
		if (expression != null)
		{
			if (expression is AssignmentExpression)
				statement = Assignment.FromAssignmentExpression(expression as AssignmentExpression);
			else 
				if (expression is MethodCallExpression)
					statement = MethodCall.FromMethodCallExpression(expression as MethodCallExpression);
			else
				statement = StructuralParser.Statement.FromExpression(expression);
			statement.SetRange(GetRange(expression, tToken));
		}
		else
		{
			if (tToken.Type == Tokens.SCOLON)
			{
				statement = new Statement(tToken.Value);
				statement.SetRange(tToken.Range);
			}
			else
			{
				Get();
			}
		}
		if (testToken == tToken)
			Get();
		if (statement != null)
		{
			if (aspContext != null)
				aspContext.AddNode(statement);
			else
				AddNode(statement);
		}
	}
	void SwitchSection()
	{
		SwitchLabel();
		while (la.Type == Tokens.CASE || (la.Type == Tokens.DEFAULT && scanner.Peek().Type == Tokens.COLON))
		{
			SwitchLabel();
		}
		Statement();
		while (la.Type != Tokens.EOF && IsNoSwitchLabelOrRBrace())
		{
			Token startToken = la;
			Statement();
			if (startToken == la)
			 Get();
		}
		if (Context != null && Context is Case)
		{
			Context.SetRange(GetRange(Context, tToken));
			CloseContext();
		}
	}
	void ForInitializer()
	{
		Expression expression = null;
		For forStatement = Context as For;
		if (IsLocalVarDecl())
		{
			ForVariableDeclaration();
		}
		else if (StartOf(17))
		{
			Expression(out expression);
			if (forStatement != null && expression != null)
			{
				forStatement.Initializers.Add(expression);
				forStatement.AddDetailNode(expression);
			}
			while (la.Type == Tokens.COMMA )
			{
				Get();
				Expression(out expression);
				if (forStatement != null && expression != null)
				{
					forStatement.Initializers.Add(expression);
					forStatement.AddDetailNode(expression);
				}
			}
		}
		else
			SynErr(207);
	}
	void ForIterator()
	{
		Expression expression = null;
		For forStatement = Context as For;
		Expression(out expression);
		if (forStatement != null && expression != null)
		{
			forStatement.Incrementors.Add(expression);
			forStatement.AddDetailNode(expression);
		}
		while (la.Type == Tokens.COMMA )
		{
			Get();
			Expression(out expression);
			if (forStatement != null && expression != null)
			{
				forStatement.Incrementors.Add(expression);
				forStatement.AddDetailNode(expression);
			}
		}
	}
	void TryCatchFinallyBlock()
	{
		Expect(Tokens.TRY );
		Try tryBlock = new Try();
		OpenContext(tryBlock);
		tryBlock.SetRange(tToken.Range);
		BlockCore(false);
		CloseContextAndSetRange(tryBlock);	
		while (la.Type == Tokens.CATCH  || la.Type == Tokens.FINALLY )
		{
			if (la.Type == Tokens.CATCH )
			{
				CatchClauses();
			}
			else
			{
				FinallyBlock();
			}
		}
	}
	void ResourceAcquisition()
	{
		Expression expression = null;
		UsingStatement usingStatement = Context as UsingStatement;
		if (IsLocalVarDecl())
		{
			UsingVariableDeclaration();
		}
		else if (StartOf(17))
		{
			Expression(out expression);
			if (expression != null && usingStatement != null)
			{
				usingStatement.AddDetailNode(expression);
				usingStatement.Initializers.Add(expression);
			}
		}
		else
			SynErr(208);
	}
	void CatchClauses()
	{
		TypeReferenceExpression type = null;
		Variable variable = null;
		String variableName = String.Empty;
		SourceRange variableNameRange = SourceRange.Empty;
		Expect(Tokens.CATCH );
		Catch catchBlock = new Catch();
		catchBlock.SetRange(tToken.Range);
		OpenContext(catchBlock);
		if (la.Type == Tokens.LBRACE )
		{
			BlockCore(false);
		}
		else if (la.Type == Tokens.LPAR )
		{
			Get();
			Token lParToken = tToken;
			ClassType(out type);
			if (la.Type == Tokens.IDENT )
			{
				Get();
				variableName = tToken.Value;
				variableNameRange = tToken.Range;
			}
			if (type != null)
			{
				catchBlock.CatchesException = GlobalStringStorage.Intern(type.ToString());
				variable = CreateVariable(variableName, type, null, variableNameRange, type.Range, true, SourceRange.Empty);
				variable.SetRange(GetRange(type, tToken));
				catchBlock.Exception = variable;
				catchBlock.AddDetailNode(variable);
				SkipTo(Tokens.RPAR);
			}
			Expect(Tokens.RPAR );
			catchBlock.SetParensRange(lParToken, tToken);
			BlockCore(false);
		}
		else
			SynErr(209);
		CloseContextAndSetRange(catchBlock);
		if (la.Type == Tokens.CATCH )
		{
			CatchClauses();
		}
	}
	void FinallyBlock()
	{
		Expect(Tokens.FINALLY );
		Finally finallyBlock = new Finally();
		OpenContext(finallyBlock);
		finallyBlock.SetRange(tToken.Range);
		BlockCore(false);
		CloseContextAndSetRange(finallyBlock);	
	}
	void SwitchLabel()
	{
		Expression expression = null;
		SourceRange startRange = la.Range;
		if (Context != null && Context is Case)
		{
			Context.SetRange(GetRange(Context, tToken));
			CloseContext();
		}
		Case caseStatement = new Case();
		if (la.Type == Tokens.CASE )
		{
			Get();
			Expression(out expression);
			Expect(Tokens.COLON );
			if (expression != null)
			{
				caseStatement.AddDetailNode(expression);
				caseStatement.Expression = expression;
			}
		}
		else if (la.Type == Tokens.DEFAULT )
		{
			Get();
			Expect(Tokens.COLON );
			caseStatement.IsDefault = true;
		}
		else
			SynErr(210);
		Switch sw = Context as Switch;
		if (sw != null)
			sw.AddCaseStatement(caseStatement);
		OpenContextWithoutAddNode(caseStatement);
		caseStatement.SetRange(GetRange(startRange, tToken));
	}
	void AssignmentOperator(out Token operatorToken, out AssignmentOperatorType assignmentOperator)
	{
		operatorToken = la;
		assignmentOperator = AssignmentOperatorType.None;
		switch (la.Type)
		{
		case Tokens.ASSGN : 
		{
			Get();
			assignmentOperator = AssignmentOperatorType.Assignment;
			break;
		}
		case Tokens.PLUSASSGN : 
		{
			Get();
			assignmentOperator = AssignmentOperatorType.PlusEquals;
			break;
		}
		case Tokens.MINUSASSGN : 
		{
			Get();
			assignmentOperator = AssignmentOperatorType.MinusEquals;
			break;
		}
		case Tokens.TIMESASSGN : 
		{
			Get();
			assignmentOperator = AssignmentOperatorType.StarEquals;
			break;
		}
		case Tokens.DIVASSGN : 
		{
			Get();
			assignmentOperator = AssignmentOperatorType.SlashEquals;
			break;
		}
		case Tokens.MODASSGN : 
		{
			Get();
			assignmentOperator = AssignmentOperatorType.PercentEquals;
			break;
		}
		case Tokens.ANDASSGN : 
		{
			Get();
			assignmentOperator = AssignmentOperatorType.BitAndEquals;
			break;
		}
		case Tokens.ORASSGN : 
		{
			Get();
			assignmentOperator = AssignmentOperatorType.BitOrEquals;
			break;
		}
		case Tokens.XORASSGN : 
		{
			Get();
			assignmentOperator = AssignmentOperatorType.XorEquals;
			break;
		}
		case Tokens.LSHASSGN : 
		{
			Get();
			assignmentOperator = AssignmentOperatorType.ShiftLeftEquals;
			break;
		}
		case Tokens.GT : 
		{
			Get();
			assignmentOperator = AssignmentOperatorType.ShiftRightEquals;
			operatorToken = new Token(tToken.StartPosition, la.EndPosition, tToken.Line, tToken.Column, la.EndLine, la.EndColumn, Tokens.GT, ">>=");
			Expect(Tokens.GTEQ );
			break;
		}
		default: SynErr(211); break;
		}
	}
	void LambdaExpression(out Expression result)
	{
		LambdaExpression lambdaExpr = new LambdaExpression();
		result  = lambdaExpr;
		lambdaExpr.SetRange(la.Range);
		 bool oldIsAsyncContext = IsAsyncContext;
		if (la.Match(Tokens.IDENT) && la.Value == "async")
		{
			Expect(Tokens.IDENT );
			CorrectFormattingTokenType(tToken);
			lambdaExpr.IsAsynchronous = true;
		}
		IsAsyncContext = lambdaExpr.IsAsynchronous;
		if (la.Type == Tokens.LPAR )
		{
			Get();
			lambdaExpr.ParamOpenRange = tToken.Range;
			if (StartOf(9))
			{
				LambdaParameterList(lambdaExpr);
			}
			Expect(Tokens.RPAR );
			lambdaExpr.ParamCloseRange = tToken.Range;
		}
		else if (StartOf(9))
		{
			LambdaParameterList(lambdaExpr);
		}
		else
			SynErr(212);
		while (!(la.Type == Tokens.EOF  || la.Type == Tokens.LAMBDA ))
		{
			SynErr(213);
			Get();
		}
		Expect(Tokens.LAMBDA );
		lambdaExpr.OperatorRange = tToken.Range; 
		LambdaExpressionBody(lambdaExpr);
		lambdaExpr.SetRange(GetRange(lambdaExpr, tToken));
		IsAsyncContext = oldIsAsyncContext;
	}
	void SqlExpression(out Expression sqlExpression)
	{
		QueryExpression oldSqlExpression = null;
		sqlExpression = new QueryExpression();
		sqlExpression.SetRange(la.Range);
		if (_ActiveSqlExpression != null)
			oldSqlExpression = _ActiveSqlExpression;
		_ActiveSqlExpression = sqlExpression as QueryExpression;
		FromClause();
		QueryBody();
		sqlExpression.SetRange(GetRange(sqlExpression, tToken));
		_ActiveSqlExpression = oldSqlExpression;
	}
	void Unary(out Expression expression)
	{
		TypeReferenceExpression typeRef = null;
		expression = null;
		Expression topLevelExpression = null;
		Expression currentExpression = null;
		SourceRange startRange = la.Range;
		ExpressionCollection expressions = new ExpressionCollection();
		Expression firstExpression = null;
		UnaryOperatorType typeOperator = UnaryOperatorType.None;
		while (unaryHead[la.Type] || IsTypeCast())
		{
			startRange = la.Range;
			if (StartOf(26))
			{
				if (StartOf(27))
				{
					if (la.Type == Tokens.PLUS )
					{
						Get();
						typeOperator = UnaryOperatorType.UnaryPlus; 
					}
					else if (la.Type == Tokens.MINUS )
					{
						Get();
						typeOperator = UnaryOperatorType.UnaryNegation; 
					}
					else if (la.Type == Tokens.TILDE )
					{
						Get();
						typeOperator = UnaryOperatorType.OnesComplement; 
					}
					else if (la.Type == Tokens.TIMES )
					{
						Get();
						typeOperator = UnaryOperatorType.PointerDereference; 
					}
					else
						SynErr(214);
					topLevelExpression = new UnaryOperatorExpression();
					(topLevelExpression as UnaryOperatorExpression).SetOperatorRange(tToken.Range);
					(topLevelExpression as UnaryOperatorExpression).UnaryOperator = typeOperator;
				}
				else if (la.Type == Tokens.INC )
				{
					Get();
					topLevelExpression = new UnaryIncrement();
					(topLevelExpression as UnaryOperatorExpression).SetOperatorRange(tToken.Range);
					(topLevelExpression as UnaryOperatorExpression).UnaryOperator = UnaryOperatorType.Increment;
				}
				else if (la.Type == Tokens.DEC )
				{
					Get();
					topLevelExpression = new UnaryDecrement();
					(topLevelExpression as UnaryOperatorExpression).SetOperatorRange(tToken.Range);
					(topLevelExpression as UnaryOperatorExpression).UnaryOperator = UnaryOperatorType.Decrement;
				}
				else if (la.Type == Tokens.NOT )
				{
					Get();
					topLevelExpression = new LogicalInversion();
					(topLevelExpression as UnaryOperatorExpression).SetOperatorRange(tToken.Range);
					(topLevelExpression as UnaryOperatorExpression).UnaryOperator = UnaryOperatorType.LogicalNot;
				}
				else
				{
					Get();
					topLevelExpression = new AddressOfExpression();
				}
				if (topLevelExpression != null)
				topLevelExpression.Name = tToken.Value;
				if (currentExpression != null && topLevelExpression != null)
				{
					if (currentExpression is UnaryOperatorExpression)
						(currentExpression as UnaryOperatorExpression).Expression = topLevelExpression;
					if (currentExpression is AddressOfExpression)
						(currentExpression as AddressOfExpression).Expression = topLevelExpression;
				}
			}
			else if (la.Type == Tokens.LPAR )
			{
				Expression sqlExpr = null; 
				Get();
				Type(out typeRef, false);
				Expect(Tokens.RPAR );
				if (IsSqlExpression())
				{
					SqlExpression(out sqlExpr);
				}
				topLevelExpression = new TypeCastExpression();
				if (typeRef != null)
				{
				  (topLevelExpression as TypeCastExpression).SetTypeReference(typeRef);
				  if (sqlExpr != null)
				  {
					(topLevelExpression as TypeCastExpression).SetTarget(sqlExpr);
					topLevelExpression.SetRange(GetRange(startRange, tToken.Range));
					expression = topLevelExpression;
					return;
				  }
				}
			}
			else
				SynErr(215);
			if (firstExpression == null)
			firstExpression = topLevelExpression;
			if (topLevelExpression != null && currentExpression != null)
			{
				if (currentExpression is UnaryOperatorExpression)
					 (currentExpression as UnaryOperatorExpression).Expression = topLevelExpression;
				if (currentExpression is AddressOfExpression)
						(currentExpression as AddressOfExpression).Expression = topLevelExpression;
				if (currentExpression is TypeCastExpression)
					(currentExpression as TypeCastExpression).SetTarget(topLevelExpression);		
			}
			if (topLevelExpression != null)
			{
				topLevelExpression.SetRange(GetRange(startRange, tToken)); 
				for (int i = 0; i < expressions.Count; i++)
					expressions[i].SetRange(GetRange(expressions[i], topLevelExpression));
				expressions.Add(topLevelExpression);
			}
			currentExpression = topLevelExpression;
		}
		Primary(out expression, typeOperator);
		if (currentExpression != null && expression != null)
		{
			if (currentExpression is UnaryOperatorExpression)
				(currentExpression as UnaryOperatorExpression).Expression = expression;
			if (currentExpression is AddressOfExpression)
				(topLevelExpression as AddressOfExpression).Expression = expression;
			if (currentExpression is TypeCastExpression)
				(currentExpression as TypeCastExpression).SetTarget(expression);
			currentExpression.SetRange(GetRange(currentExpression, expression));
			if (expression is ElementReferenceExpression && (currentExpression is UnaryIncrement || currentExpression is UnaryDecrement))
				(expression as ElementReferenceExpression).IsModified = true;
			for (int i = 0; i < expressions.Count; i++)
				expressions[i].SetRange(GetRange(expressions[i], expression));
			expression = firstExpression;
		}
	}
	void NullCoalescingExpr(out Expression expression, Expression leftPart)
	{
		expression = null;
		Expression rightPart = null;
		Expression currentLeftPart = leftPart;
		Token operatorToken = null;
		OrExpr(out expression, currentLeftPart);
		if (expression == null)
		expression = currentLeftPart;
		while (la.Type == Tokens.DBLQUEST )
		{
			Get();
			operatorToken = tToken;
			Unary(out currentLeftPart);
			OrExpr(out rightPart, currentLeftPart);
			if (rightPart == null)
			rightPart = currentLeftPart;
			expression = GetNullCoalescingExpression(expression, rightPart, operatorToken);
			currentLeftPart = expression;
		}
	}
	void FromClause()
	{
		InExpression inExpression = null;
		FromExpression fromExpression = new FromExpression();
		AddToSqlExpression(fromExpression);
		fromExpression.SetRange(la.Range);
		Expect(Tokens.IDENT );
		CorrectFormattingTokenType(tToken);
		InExpression(out inExpression);
		fromExpression.AddInExpression(inExpression);
		while (la.Type == Tokens.COMMA )
		{
			Get();
			InExpression(out inExpression);
			fromExpression.AddInExpression(inExpression);
		}
		fromExpression.SetRange(GetRange(fromExpression, tToken));
		while (la.Type == Tokens.IDENT && la.Value == "join")
		{
			JoinClause();
		}
	}
	void QueryBody()
	{
		while (la.Type == Tokens.IDENT && (la.Value == "from" || la.Value == "let" || la.Value == "where" || la.Value == "orderby" || la.Value == "join"))
		{
			FromLetWhereClause();
		}
		SelectOrGroupClause();
		if (la.Type == Tokens.IDENT && la.Value == "into")
		{
			QueryContinuation();
		}
	}
	void QueryIdent(out QueryIdent ident)
	{
		TypeReferenceExpression typeRef = null;
		ident = new QueryIdent();
		ident.SetRange(la.Range);
		if (QueryIdentHasType())
		{
			Type(out typeRef, true);
			Expect(Tokens.IDENT );
		}
		else if (la.Type == Tokens.IDENT )
		{
			Get();
		}
		else
			SynErr(216);
		ident.Name = tToken.Value;
		ident.NameRange = tToken.Range;
		ident.MemberType = String.Empty;
		if (typeRef != null)
		{
			ident.MemberTypeReference = typeRef;
			ident.MemberType = GlobalStringStorage.Intern(typeRef.ToString());
			ident.AddDetailNode(typeRef);
		}
		ident.SetRange(GetRange(ident, tToken));
	}
	void InExpression(out InExpression inExpression)
	{
		inExpression = new InExpression();
		inExpression.SetRange(la.Range);
		QueryIdent ident = null;
		Expression expression = null;
		QueryIdent(out ident);
		Expect(Tokens.IN );
		CorrectFormattingTokenType(tToken);
		Expression(out expression);
		inExpression.QueryIdent = ident;
		inExpression.Expression = expression;
		inExpression.SetRange(GetRange(inExpression, tToken));
	}
	void JoinClause()
	{
		JoinExpressionBase joinExpression = new JoinExpression();
		joinExpression.SetRange(la.Range);
		InExpression inExpression = null;
		EqualsExpression onExpression = null;
		QueryIdent intoName = null;
		Expect(Tokens.IDENT );
		CorrectFormattingTokenType(tToken);
		InExpression(out inExpression);
		joinExpression.SetInExpression(inExpression);
		Expect(Tokens.IDENT );
		CorrectFormattingTokenType(tToken);
		EqualsExpression(out onExpression);
		joinExpression.AddEqualsExpression(onExpression);
		if (la.Type == Tokens.IDENT && la.Value == "into")
		{
			Expect(Tokens.IDENT );
			CorrectFormattingTokenType(tToken);
			QueryIdent(out intoName);
			joinExpression = ConvertToJoinIntoExpression(joinExpression, intoName);
		}
		AddToSqlExpression(joinExpression);
		joinExpression.SetRange(GetRange(joinExpression, tToken));
	}
	void EqualsExpression(out EqualsExpression expression)
	{
		Expression left = null;
		Expression right = null;
		expression = null;
		Expression(out left);
		Expect(Tokens.IDENT );
		CorrectFormattingTokenType(tToken);
		Expression(out right);
		if (left != null && right != null)
		{
			expression = new EqualsExpression(left, right);
			expression.SetRange(GetRange(left, right));
		}
	}
	void FromLetWhereClause()
	{
		if (la.Type == Tokens.IDENT && la.Value == "from")
		{
			FromClause();
		}
		else if (la.Type == Tokens.IDENT )
		{
			if (la.Type == Tokens.IDENT && la.Value == "let")
			{
				LetClause();
			}
			else
			{
				if (la.Type == Tokens.IDENT && la.Value == "orderby")
				{
					OrderByClause();
				}
				else
				{
					if (la.Type == Tokens.IDENT && la.Value == "join")
					{
						JoinClause();
					}
					else
					{
						WhereClause();
					}
				}
			}
		}
		else
			SynErr(217);
	}
	void SelectOrGroupClause()
	{
		Expression selectExpression = null;
		Expression groupExpression = null;
		Expression byExpression = null;
		QueryExpressionBase result = null;
		if (la.Type == Tokens.IDENT && la.Value == "select")
		{
			Expect(Tokens.IDENT );
			CorrectFormattingTokenType(tToken);
			result = new SelectExpression();
			result.SetRange(tToken.Range);
			Expression(out selectExpression);
			(result as SelectExpression).AddReturnedExpression(selectExpression);
			result.SetRange(GetRange(result, tToken));
			AddToSqlExpression(result);
		}
		else if (StartOf(19))
		{
			if (la.Type == Tokens.IDENT && la.Value == "group")
			{
				Expect(Tokens.IDENT );
				CorrectFormattingTokenType(tToken);
				result = new GroupByExpression();
				result.SetRange(tToken.Range);
				Expression(out groupExpression);
				CorrectFormattingTokenType(tToken);
				if (groupExpression != null)
					(result as GroupByExpression).AddGroupElement(groupExpression);
				Expect(Tokens.IDENT );
				CorrectFormattingTokenType(tToken);
				Expression(out byExpression);
				(result as GroupByExpression).AddByElement(byExpression);
				result.SetRange(GetRange(result, tToken));
				AddToSqlExpression(result);
			}
			else if (StartOf(19))
			{
			}
			else
				SynErr(218);
		}
		else
			SynErr(219);
	}
	void QueryContinuation()
	{
		IntoExpression intoExpression = new IntoExpression();
		intoExpression.SetRange(la.Range);
		QueryIdent ident= null;
		AddToSqlExpression(intoExpression);
		Expect(Tokens.IDENT );
		CorrectFormattingTokenType(tToken);
		QueryIdent(out ident);
		intoExpression.SetIntoTarget(ident);
		intoExpression.SetRange(GetRange(intoExpression, tToken));
		while (la.Type == Tokens.IDENT && la.Value == "join")
		{
			JoinClause();
		}
		QueryBody();
	}
	void LetClause()
	{
		Expression expression = null;
		LetExpression letExpression = new LetExpression();
		AddToSqlExpression(letExpression);
		letExpression.SetRange(la.Range);
		Token nameToken = null;
		SourceRange operatorRange = SourceRange.Empty;
		Expect(Tokens.IDENT );
		CorrectFormattingTokenType(tToken);
		Expect(Tokens.IDENT );
		nameToken = tToken;
		operatorRange = la.Range;
		Expect(Tokens.ASSGN );
		Expression(out expression);
		letExpression.SetRange(GetRange(letExpression, tToken));
		letExpression.AddDeclaration(CreateQueryIdent(nameToken, operatorRange, expression));
	}
	void OrderByClause()
	{
		OrderByExpression orderBy = new OrderByExpression();
		AddToSqlExpression(orderBy);
		orderBy.SetRange(la.Range);
		Expect(Tokens.IDENT );
		CorrectFormattingTokenType(tToken);
		Orderings(orderBy);
		orderBy.SetRange(GetRange(orderBy, tToken));
	}
	void WhereClause()
	{
		WhereExpression whereExpression = null;
		if (la.Type != Tokens.IDENT || la.Value != "where")
			return;
		Expression expression = null;
		whereExpression = new WhereExpression();
		AddToSqlExpression(whereExpression);
		whereExpression.SetRange(la.Range);
		Expect(Tokens.IDENT );
		CorrectFormattingTokenType(tToken);
		Expression(out expression);
		whereExpression.SetRange(GetRange(whereExpression, tToken));
		whereExpression.SetWhereClause(expression);
	}
	void Orderings(OrderByExpression orderBy)
	{
		OrderingExpression ordering = null;
		Ordering(out ordering);
		if (orderBy != null)
		orderBy.AddOrdering(ordering);
		while (la.Type == Tokens.COMMA )
		{
			Get();
			Ordering(out ordering);
			if (orderBy != null)
			orderBy.AddOrdering(ordering);
		}
	}
	void Ordering(out OrderingExpression ordering)
	{
		Expression expression = null;
		ordering = new OrderingExpression();
		ordering.SetRange(la.Range);
		Expression(out expression);
		ordering.SetOrdering(expression);
		if (la.Type == Tokens.IDENT && (la.Value == "ascending" || la.Value == "descending"))
		{
			Expect(Tokens.IDENT );
			CorrectFormattingTokenType(tToken);
			if (tToken.Value == "ascending")
				ordering.Order = OrderingType.Ascending;
			else
				if (tToken.Value == "descending")
					ordering.Order = OrderingType.Descending;
		}
		ordering.SetRange(GetRange(ordering, tToken));
	}
	void LambdaParameterList(LambdaExpression lambdaExpression)
	{
		Param currentParam = null;
		LambdaParameter(out currentParam);
		if (currentParam != null)
		lambdaExpression.AddParameter(currentParam);
		while (la.Type == Tokens.COMMA )
		{
			Get();
			LambdaParameter(out currentParam);
			if (currentParam != null)
			lambdaExpression.AddParameter(currentParam);
		}
	}
	void LambdaExpressionBody(Expression parentLambdaExpression)
	{
		Expression expression = null;
		LanguageElement oldContext = Context;
		SetContext(parentLambdaExpression);
		if (StartOf(17))
		{
			Expression(out expression);
			if (expression != null)
			AddNode(expression);
		}
		else if (la.Type == Tokens.LBRACE )
		{
			BlockCore(false);
		}
		else
			SynErr(220);
		SetContext(oldContext);	
	}
	void LambdaParameter(out Param parameter)
	{
		parameter = null;
		LanguageElementCollection attributes = null;
		if (IsImplicitLambdaParameter())
		{
			Expect(Tokens.IDENT );
			parameter = new LambdaImplicitlyTypedParam(tToken.Value);
			parameter.SetRange(tToken.Range);
			parameter.NameRange = tToken.Range;
		}
		else if (StartOf(9))
		{
			ParameterDeclaration(out parameter, out attributes);
		}
		else
			SynErr(221);
	}
	void OrExpr(out Expression expression, Expression leftPart)
	{
		expression = null;
		Expression rightPart = null;
		Expression currentLeftPart = leftPart;
		Token operatorToken = null;
		LogicalOperator operatorType = LogicalOperator.ShortCircuitOr;
		AndExpr(out expression, currentLeftPart);
		if (expression == null)
		expression = currentLeftPart;
		while (la.Type == Tokens.DBLOR )
		{
			Get();
			operatorToken = tToken;
			Unary(out currentLeftPart);
			AndExpr(out rightPart, currentLeftPart);
			if (rightPart == null)
			rightPart = currentLeftPart;
			expression = GetLogicalOperation(expression, rightPart, operatorType, operatorToken);
			currentLeftPart = expression;
		}
	}
	void AndExpr(out Expression expression, Expression leftPart)
	{
		expression = null;
		Expression rightPart = null;
		Expression currentLeftPart = leftPart;
		Token operatorToken = null;
		LogicalOperator operatorType = LogicalOperator.ShortCircuitAnd;
		BitOrExpr(out expression, currentLeftPart);
		if (expression == null)
		expression = currentLeftPart;
		while (la.Type == Tokens.DBLAND )
		{
			Get();
			operatorToken = tToken;
			Unary(out currentLeftPart);
			BitOrExpr(out rightPart, currentLeftPart);
			if (rightPart == null)
			rightPart = currentLeftPart;
			expression = GetLogicalOperation(expression, rightPart, operatorType, operatorToken);
			currentLeftPart = expression;
		}
	}
	void BitOrExpr(out Expression expression, Expression leftPart)
	{
		expression = null;
		Expression rightPart = null;
		Expression currentLeftPart = leftPart;
		Token operatorToken = null;
		LogicalOperator operatorType = LogicalOperator.Or;
		BitXorExpr(out expression, currentLeftPart);
		if (expression == null)
		expression = currentLeftPart;
		while (la.Type == Tokens.OR )
		{
			Get();
			operatorToken = tToken;
			Unary(out currentLeftPart);
			BitXorExpr(out rightPart, currentLeftPart);
			if (rightPart == null)
			rightPart = currentLeftPart;
			expression = GetLogicalOperation(expression, rightPart, operatorType, operatorToken);
			currentLeftPart = expression;
		}
	}
	void BitXorExpr(out Expression expression, Expression leftPart)
	{
		expression = null;
		Expression rightPart = null;
		Expression currentLeftPart = leftPart;
		Token operatorToken = null;
		LogicalOperator operatorType = LogicalOperator.ExclusiveOr;
		BitAndExpr(out expression, currentLeftPart);
		if (expression == null)
		expression = currentLeftPart;
		while (la.Type == Tokens.XOR )
		{
			Get();
			operatorToken = tToken;
			Unary(out currentLeftPart);
			BitAndExpr(out rightPart, currentLeftPart);
			if (rightPart == null)
			rightPart = currentLeftPart;
			expression = GetLogicalOperation(expression, rightPart, operatorType, operatorToken);
			currentLeftPart = expression;
		}
	}
	void BitAndExpr(out Expression expression, Expression leftPart)
	{
		expression = null;
		Expression rightPart = null;
		Expression currentLeftPart = leftPart;
		Token operatorToken = null;
		LogicalOperator operatorType = LogicalOperator.And;
		EqlExpr(out expression, currentLeftPart);
		if (expression == null)
		expression = currentLeftPart;
		while (la.Type == Tokens.AND )
		{
			Get();
			operatorToken = tToken;
			Unary(out currentLeftPart);
			EqlExpr(out rightPart, currentLeftPart);
			if (rightPart == null)
			rightPart = currentLeftPart;
			expression = GetLogicalOperation(expression, rightPart, operatorType, operatorToken);
			currentLeftPart = expression;
		}
	}
	void EqlExpr(out Expression expression, Expression leftPart)
	{
		expression = null;
		Expression rightPart = null;
		Expression currentLeftPart = leftPart;
		Token operatorToken = null;
		RelationalOperator operatorType = RelationalOperator.None;
		RelExpr(out expression, currentLeftPart);
		while (la.Type == Tokens.EQ  || la.Type == Tokens.NEQ )
		{
			if (la.Type == Tokens.NEQ )
			{
				Get();
				operatorType = RelationalOperator.Inequality;
			}
			else
			{
				Get();
				operatorType = RelationalOperator.Equality;
			}
			operatorToken = tToken;
			Unary(out currentLeftPart);
			RelExpr(out rightPart, currentLeftPart);
			if (rightPart == null)
			rightPart = currentLeftPart;
			expression = GetRelationalOperation(expression, rightPart, operatorType, operatorToken);
			currentLeftPart = expression;
		}
	}
	void RelExpr(out Expression expression, Expression leftPart)
	{
		TypeReferenceExpression type = null;
		Expression fictiveType = null;
		expression = null;
		Expression rightPart = null;
		Expression currentLeftPart = leftPart;
		Token operatorToken = null;
		RelationalOperator operatorType = RelationalOperator.None;
		ShiftExpr(out expression, currentLeftPart);
		if (expression == null)
		expression = currentLeftPart;
		while (StartOf(28))
		{
			if (StartOf(29))
			{
				if (la.Type == Tokens.LT )
				{
					Get();
					operatorType = RelationalOperator.LessThan;
				}
				else if (la.Type == Tokens.GT )
				{
					Get();
					operatorType = RelationalOperator.GreaterThan;
				}
				else if (la.Type == Tokens.LOWOREQ )
				{
					Get();
					operatorType = RelationalOperator.LessOrEqual;
				}
				else if (la.Type == Tokens.GTEQ )
				{
					Get();
					operatorType = RelationalOperator.GreaterOrEqual;
				}
				else
					SynErr(222);
				operatorToken = tToken;
				Unary(out currentLeftPart);
				ShiftExpr(out rightPart, currentLeftPart);
				if (rightPart == null)
				rightPart = currentLeftPart;
				expression = GetRelationalOperation(expression, rightPart, operatorType, operatorToken);
			}
			else if (la.Type == Tokens.IS )
			{
				Get();
				operatorToken = tToken;
				if (StartOf(14))
				{
					ResolvedType(out type);
				}
				else if (StartOf(17))
				{
					Expression(out fictiveType);
				}
				else
					SynErr(223);
				if (type != null)
				fictiveType = type;
				expression = GetTypeCheck(expression, fictiveType, operatorToken);
			}
			else
			{
				Get();
				operatorToken = tToken;
				if (StartOf(14))
				{
					ResolvedType(out type);
				}
				else if (StartOf(17))
				{
					Expression(out fictiveType);
				}
				else
					SynErr(224);
				if (type != null)
				fictiveType = type;
				expression = GetConditionalTypeCast(expression, fictiveType, operatorToken);
			}
			currentLeftPart = expression;
		}
	}
	void ShiftExpr(out Expression expression, Expression leftPart)
	{
		expression = null;
		Expression rightPart = null;
		Expression currentLeftPart = leftPart;
		Token operatorToken = null;
		BinaryOperatorType operatorType = BinaryOperatorType.None;
		AddExpr(out expression, currentLeftPart);
		if (expression == null)
		expression = currentLeftPart;
		if (la.Type != Tokens.LTLT && la.Type != Tokens.GT)
			return;
		if (la.Type == Tokens.GT)
		{
			ResetPeek();
			Token nextToken = Peek();
			if (nextToken.Type != Tokens.GT)
				return;
		}
		while (la.Type == Tokens.GT  || la.Type == Tokens.LTLT )
		{
			if (la.Type == Tokens.LTLT )
			{
				Get();
				operatorType = BinaryOperatorType.ShiftLeft;
				operatorToken = tToken;
			}
			else
			{
				Get();
				operatorType = BinaryOperatorType.ShiftRight;
				operatorToken = new Token(tToken.StartPosition, la.EndPosition, tToken.Line, tToken.Column, la.EndLine, la.EndColumn, Tokens.GT, ">>");
				Expect(Tokens.GT );
			}
			Unary(out currentLeftPart);
			AddExpr(out rightPart, currentLeftPart);
			if (rightPart == null)
			rightPart = currentLeftPart;
			expression = GetBinaryOperatorExpression(expression, rightPart, operatorType, operatorToken);
			currentLeftPart = expression;
		}
	}
	void AddExpr(out Expression expression, Expression leftPart)
	{
		expression = null;
		Expression rightPart = null;
		Expression currentLeftPart = leftPart;
		Token operatorToken = null;
		BinaryOperatorType operatorType = BinaryOperatorType.None;
		MulExpr(out expression, currentLeftPart);
		if (expression == null)
		expression = currentLeftPart;
		while (la.Type == Tokens.MINUS  || la.Type == Tokens.PLUS )
		{
			if (la.Type == Tokens.PLUS )
			{
				Get();
				operatorType = BinaryOperatorType.Add;
			}
			else
			{
				Get();
				operatorType = BinaryOperatorType.Subtract;
			}
			operatorToken = tToken;
			Unary(out currentLeftPart);
			MulExpr(out rightPart, currentLeftPart);
			if (rightPart == null)
			rightPart = currentLeftPart;
			expression = GetBinaryOperatorExpression(expression, rightPart, operatorType, operatorToken);
			currentLeftPart = expression;
		}
	}
	void MulExpr(out Expression expression, Expression leftPart)
	{
		expression = null;
		Expression rightPart = null;
		Expression currentLeftPart = leftPart;
		Token operatorToken = null;
		BinaryOperatorType operatorType = BinaryOperatorType.None;
		while (la.Type == Tokens.TIMES  || la.Type == Tokens.DIV  || la.Type == Tokens.MOD )
		{
			if (la.Type == Tokens.TIMES )
			{
				Get();
				operatorType = BinaryOperatorType.Multiply;
			}
			else if (la.Type == Tokens.DIV )
			{
				Get();
				operatorType = BinaryOperatorType.Divide;
			}
			else
			{
				Get();
				operatorType = BinaryOperatorType.Modulus;
			}
			operatorToken = tToken;
			Unary(out rightPart);
			expression = GetBinaryOperatorExpression(currentLeftPart, rightPart, operatorType, operatorToken);
			currentLeftPart = expression;
		}
	}
	void Primary(out Expression expression, UnaryOperatorType typeOperator)
	{
		TypeReferenceExpression typeRef = null;
		ElementReferenceExpression elRef = null;
		expression = null;
		SourceRange startRange = la.Range;
		ExpressionCollection arguments = null;
		Expression source = null;
		 LanguageElementCollection razorExpressionElements = null;
		if (StartOf(30))
		{
			Literal(out expression, typeOperator);
		}
		else if (la.Type == Tokens.LPAR )
		{
			Get();
			Expression(out expression);
			Expect(Tokens.RPAR );
			ParenthesizedExpression parExpr = new ParenthesizedExpression(expression);
			parExpr.SetRange(GetRange(startRange, tToken));
			expression = parExpr;
		}
		else if (IsAsyncDelegate())
		{
			Expect(Tokens.IDENT );
			CorrectFormattingTokenType(tToken); 
			AnonymousMethodExpression(out expression, true);
		}
		else if (IsAwaitExpression())
		{
			AwaitExpression(out expression);
		}
		else if (StartOf(25))
		{
			PrimitiveLanguageType(out typeRef);
			expression = typeRef;
		}
		else if (la.Type == Tokens.IDENT && (la.Value == "string" || la.Value == "object"))
		{
			Expect(Tokens.IDENT );
			expression = new TypeReferenceExpression(tToken.Value, tToken.Range);
			expression.NameRange = tToken.Range;
		}
		else if (IsRazorInlineExpression())
		{
			SourcePoint startPoint = SourcePoint.Empty;
			if (la.Type == Tokens.AT )
			{
				startPoint = la.Range.Start;
				Get();
			}
			Expect(Tokens.IDENT );
			LanguageElement savedContext = Context;
			SetContext(null);
			RazorHtmlCode(out razorExpressionElements);
			 SetContext(savedContext);
			if (razorExpressionElements != null && razorExpressionElements.Count == 1)
			{
			   HtmlElement htmlElement = razorExpressionElements[0] as HtmlElement;
			   if (htmlElement != null)
			   {
				   expression = new RazorInlineHtmlExpression(htmlElement);
				   expression.SetRange(startPoint, htmlElement.Range.End);
				 }
			}	  
		}
		else if (la.Type == Tokens.IDENT  || la.Type == Tokens.ARGLIST  || la.Type == Tokens.REFVALUE )
		{
			ElementReferenceName(out elRef);
			expression = elRef;
		}
		else if (la.Type == Tokens.THIS )
		{
			Get();
			expression = new ThisReferenceExpression(tToken.Value, tToken.Range);
			expression.SetRange(tToken.Range);
		}
		else if (la.Type == Tokens.BASE )
		{
			Get();
			expression = new BaseReferenceExpression(tToken.Value, tToken.Range);
			expression.SetRange(tToken.Range);
		}
		else if (la.Type == Tokens.NEW  || la.Type == Tokens.STACKALLOC )
		{
			ObjectCreationExpression(out expression);
		}
		else if (la.Type == Tokens.DEFAULT  || la.Type == Tokens.SIZEOF  || la.Type == Tokens.TYPEOF )
		{
			TypeOfSizeOfDefaultExpression(out expression);
		}
		else if (la.Type == Tokens.CHECKED  || la.Type == Tokens.UNCHECKED )
		{
			CheckedUncheckedExpression(out expression);
		}
		else if (la.Type == Tokens.DELEGATE )
		{
			AnonymousMethodExpression(out expression, false);
		}
		else
			SynErr(225);
		source = expression;
		int prevTokenType = tToken.Type;
		while (StartOf(31))
		{
			switch (la.Type)
			{
			case Tokens.INC : 
			{
				Get();
				expression = new UnaryIncrement(source, true);
				 (expression as UnaryOperatorExpression).UnaryOperator = UnaryOperatorType.Increment;
				expression.Name = tToken.Value;
				if (source is ElementReferenceExpression)
					(source as ElementReferenceExpression).IsModified = true;
				(expression as OperatorExpression).SetOperatorRange(tToken.Range);
				break;
			}
			case Tokens.DEC : 
			{
				Get();
				expression = new UnaryDecrement(source, true);
				expression.Name = tToken.Value;
				  (expression as UnaryOperatorExpression).UnaryOperator = UnaryOperatorType.Decrement;
				if (source is ElementReferenceExpression)
					(source as ElementReferenceExpression).IsModified = true;
				(expression as OperatorExpression).SetOperatorRange(tToken.Range);
				break;
			}
			case Tokens.POINT : 
			{
				Get();
				ElementReferenceName(out elRef);
				if (elRef != null)
				{
					if (!(elRef is QualifiedElementReference))
					{
						expression = new  PointerElementReference(source, elRef.Name, elRef.NameRange);
						CopyFieldValuesFromElementReference(expression as ReferenceExpressionBase, elRef);
						expression.AddNode(source);		
					}
					else
					{
						AttachToPointerElementReference(source, elRef as QualifiedElementReference);
						expression = elRef;
					}
				}
				break;
			}
			case Tokens.DOT : 
			{
				Get();
				ElementReferenceName(out elRef);
				if (elRef != null)
				{
					if (!(elRef is QualifiedElementReference))
					{
						expression = new QualifiedElementReference(source, elRef.Name, elRef.NameRange);
						expression.AddNode(source);
						CopyFieldValuesFromElementReference(expression as ReferenceExpressionBase, elRef);
					}
					else
					{
						AttachToQualifiedElementReference(source, elRef as QualifiedElementReference);
						expression = elRef;
					}
				}
				break;
			}
			case Tokens.LPAR : 
			{
				Get();
				SourceRange lparRange = tToken.Range;
				if (StartOf(17))
				{
					if (prevTokenType != Tokens.REFVALUE)
					{
						ArgumentCollection(out  arguments);
					}
					else
					{
						RefValueArguments(out arguments);
					}
				}
				Expect(Tokens.RPAR );
				SourceRange rparRange = tToken.Range;
				if (source != null)
				{
					if (source is ElementReferenceExpression)
						source = ToMethodReference(source as ElementReferenceExpression);
					MethodCallExpression methodCall = new MethodCallExpression(source);
					methodCall.AddNode(source);
					methodCall.SetParensRange(GetRange(lparRange, rparRange));
					if (arguments != null)
					{
						methodCall.Arguments = new ExpressionCollection();
						for (int i = 0; i < arguments.Count; i++)
						{
							methodCall.Arguments.Add(arguments[i]);
							methodCall.AddDetailNode(arguments[i]);
						}
					}
					expression = methodCall;
				}	
				break;
			}
			case Tokens.LBRACK : 
			{
				Get();
				ArgumentCollection(out  arguments);
				Expect(Tokens.RBRACK );
				expression = new IndexerExpression(source);
				if (arguments != null && arguments.Count > 0 && expression is IndexerExpression)
				{
					(expression as IndexerExpression).Arguments = new ExpressionCollection();
					for (int i = 0; i < arguments.Count; i++)
					{
						(expression as IndexerExpression).Arguments.Add(arguments[i]);
						expression.AddDetailNode(arguments[i]);
					}
				}
				break;
			}
			}
			if (source != null && expression != null)
			expression.SetRange(GetRange(source, tToken));
			source = expression;
			arguments = null;
		}
	}
	void Literal(out Expression expression, UnaryOperatorType typeOperator)
	{
		PrimitiveExpression primitive = new PrimitiveExpression(la.Value, la.Range);
		expression = primitive;
		primitive.NameRange = la.Range;	
		switch (la.Type)
		{
		case Tokens.INTCON : 
		{
			Get();
			primitive.PrimitiveType = CSharpPrimitiveTypeUtils.ToPrimitiveType(tToken.Type, tToken.Value, typeOperator);
			primitive.PrimitiveValue = CSharpPrimitiveTypeUtils.ToPrimitiveValue(tToken.Type, tToken.Value);
			break;
		}
		case Tokens.REALCON : 
		{
			Get();
			primitive.PrimitiveType = CSharpPrimitiveTypeUtils.ToPrimitiveType(tToken.Type, tToken.Value, typeOperator);
			primitive.PrimitiveValue = CSharpPrimitiveTypeUtils.ToPrimitiveValue(tToken.Type, tToken.Value);
			break;
		}
		case Tokens.CHARCON : 
		{
			Get();
			primitive.PrimitiveType = PrimitiveType.Char;
			primitive.PrimitiveValue = CSharpPrimitiveTypeUtils.ToPrimitiveValue(tToken.Type, tToken.Value);
			break;
		}
		case Tokens.STRINGCON : 
		{
			Get();
			primitive.PrimitiveType = PrimitiveType.String;
			primitive.PrimitiveValue = CSharpPrimitiveTypeUtils.ToPrimitiveValue(tToken.Type, tToken.Value);
			primitive.IsVerbatimStringLiteral = tToken.Value.StartsWith("@");
			break;
		}
		case Tokens.TRUE : 
		{
			Get();
			primitive.PrimitiveType = PrimitiveType.Boolean;
			primitive.PrimitiveValue = true;
			break;
		}
		case Tokens.FALSE : 
		{
			Get();
			primitive.PrimitiveType = PrimitiveType.Boolean;
			primitive.PrimitiveValue = false;
			break;
		}
		case Tokens.NULL : 
		{
			Get();
			primitive.PrimitiveType = PrimitiveType.Void;
			primitive.PrimitiveValue = null;
			break;
		}
		default: SynErr(226); break;
		}
	}
	void AnonymousMethodExpression(out Expression expression, bool isAsync)
	{
		bool oldIsAsyncContext = IsAsyncContext;
		IsAsyncContext = isAsync;
		AnonymousMethodExpression anonymousMethod = new AnonymousMethodExpression();
		anonymousMethod.NameRange = la.Range;
		Hashtable parameterAttributes = null;
		LanguageElementCollection anonymousParameters = null;
		LanguageElement oldContext = Context;
		SetContext(anonymousMethod);
		if (isAsync)
		  anonymousMethod.SetRange(tToken.Range);
		else
		  anonymousMethod.SetRange(la.Range);
		anonymousMethod.IsAsynchronous = isAsync;
		Expect(Tokens.DELEGATE );
		if (la.Type != Tokens.LPAR)
		 anonymousMethod.ParameterListOmitted = true;
		if (la.Type == Tokens.LPAR )
		{
			Get();
			anonymousMethod.ParamOpenRange = tToken.Range;
			if (StartOf(9))
			{
				FormalParameterList(out anonymousParameters, out parameterAttributes);
				if (anonymousParameters != null && anonymousMethod != null)
				{
					anonymousMethod.AddParameters(anonymousParameters);
					SetAttributesForParameters(anonymousParameters, parameterAttributes);
				}
			}
			Expect(Tokens.RPAR );
			anonymousMethod.ParamCloseRange = tToken.Range;
		}
		BlockCore(false);
		SetContext(oldContext);
		anonymousMethod.SetRange(GetRange(anonymousMethod, tToken));
		expression = anonymousMethod;
		  IsAsyncContext = oldIsAsyncContext;
	}
	void AwaitExpression(out Expression result)
	{
		Expression expression = null;
		result = null;
		SourceRange startRange = la.Range;
		CorrectFormattingTokenType(la);
		Expect(Tokens.IDENT );
		Primary(out expression, UnaryOperatorType.None);
		AwaitExpression awaitExpression = new AwaitExpression(expression);
		awaitExpression.SetRange(GetRange(startRange, tToken.Range));
		result = awaitExpression;
	}
	void ObjectCreationExpression(out Expression expression)
	{
		expression = null;
		TypeReferenceExpression typeRef = null;
		ArrayInitializerExpression initializer = null;
		SourceRange startRange = la.Range;
		ExpressionCollection arguments = null;
		bool isStackAlloc = false;
		Expression objectInitializer = null;
		if (la.Type == Tokens.NEW )
		{
			Get();
		}
		else if (la.Type == Tokens.STACKALLOC )
		{
			Get();
			isStackAlloc = true;
		}
		else
			SynErr(227);
		if (la.Type != Tokens.LBRACE && la.Type != Tokens.LBRACK)
		 SimpleType(out typeRef, true);
		if (la.Type == Tokens.TIMES )
		{
			Pointer(ref typeRef);
		}
		if (la.Type == Tokens.LBRACE )
		{
			ObjectInitializer(out objectInitializer);
			ObjectCreationExpression objectCreation = new ObjectCreationExpression(typeRef);
			if (objectInitializer != null && objectCreation != null)
			{
				objectCreation.AddDetailNode(objectInitializer);
				objectCreation.ObjectInitializer = objectInitializer as ObjectInitializerExpression;
			}
			expression = objectCreation;
		}
		else if (la.Type == Tokens.LPAR )
		{
			Get();
			Token lParToken = tToken;
			if (StartOf(17))
			{
				ArgumentCollection(out arguments);
			}
			Expect(Tokens.RPAR );
			Token rParToken = tToken;
			if (la.Type == Tokens.LBRACE )
			{
				ObjectInitializer(out objectInitializer);
			}
			ObjectCreationExpression objectCreation = new ObjectCreationExpression(typeRef);
			objectCreation.SetParensRange(lParToken, rParToken);
			if (arguments != null && arguments.Count > 0)
			{
				objectCreation.Arguments = new ExpressionCollection();
				for (int i = 0; i < arguments.Count; i++)
				{
					objectCreation.Arguments.Add(arguments[i]);
					objectCreation.AddDetailNode(arguments[i]);
				}
			}
			if (objectInitializer != null && objectCreation != null)
			{
				objectCreation.AddDetailNode(objectInitializer);
				objectCreation.ObjectInitializer = objectInitializer as ObjectInitializerExpression;
			}
			expression = objectCreation;								  
		}
		else if (la.Type == Tokens.LBRACK  || la.Type == Tokens.TIMES )
		{
			while (IsDims())
			{
				PointerOrArray(ref typeRef);
			}
			if (IsLBRACK()) { 
			Expect(Tokens.LBRACK );
			ArgumentCollection(out  arguments);
			Expect(Tokens.RBRACK );
			} 
			while (IsDims())
			{
				PointerOrArray(ref typeRef);
			}
			if (la.Type == Tokens.LBRACE )
			{
				ArrayInitializer(out initializer);
			}
			ArrayCreateExpression arrayCreation = new ArrayCreateExpression(typeRef);
			arrayCreation.IsStackAlloc = isStackAlloc;
			if (arguments != null && arguments.Count > 0)
			{
				arrayCreation.Dimensions = new ExpressionCollection();
				for (int i = 0; i < arguments.Count; i++)
				{
					arrayCreation.Dimensions.Add(arguments[i]);
					arrayCreation.AddDetailNode(arguments[i]);
				}
			}
			if (initializer != null)
			{
				arrayCreation.Initializer = initializer;
				arrayCreation.AddDetailNode(initializer);
			}
			expression = arrayCreation;								
		}
		else
			SynErr(228);
		if (typeRef != null && expression != null)
		{
			expression.AddNode(typeRef);
			expression.NameRange = typeRef.NameRange;
		}
		if (expression != null)
			expression.SetRange(GetRange(startRange, tToken));
	}
	void TypeOfSizeOfDefaultExpression(out Expression expression)
	{
		expression = null;
		TypeReferenceExpression typeRef = null;
		SourceRange startRange = la.Range;
		if (la.Type == Tokens.TYPEOF )
		{
			Get();
			Expect(Tokens.LPAR );
			Type(out typeRef, false);
			Expect(Tokens.RPAR );
			expression = new TypeOfExpression(typeRef);
		}
		else if (la.Type == Tokens.SIZEOF )
		{
			Get();
			Expect(Tokens.LPAR );
			Type(out typeRef, false);
			Expect(Tokens.RPAR );
			expression = new SizeOfExpression(typeRef);
		}
		else if (la.Type == Tokens.DEFAULT )
		{
			Get();
			Expect(Tokens.LPAR );
			Type(out typeRef, false);
			Expect(Tokens.RPAR );
			expression = new DefaultValueExpression(typeRef);
		}
		else
			SynErr(229);
		if (typeRef != null && expression != null && !(expression is DefaultValueExpression))
		{
			expression.AddDetailNode(typeRef);
			typeRef = null;
		}
		if (expression != null)
			expression.SetRange(GetRange(startRange, tToken));
	}
	void CheckedUncheckedExpression(out Expression expression)
	{
		expression  = null;
		Expression oldExpression = null;
		SourceRange startRange = la.Range;
		if (la.Type == Tokens.CHECKED )
		{
			Get();
			Expect(Tokens.LPAR );
			Expression(out oldExpression);
			Expect(Tokens.RPAR );
			expression = new CheckedExpression(oldExpression);
		}
		else if (la.Type == Tokens.UNCHECKED )
		{
			Get();
			Expect(Tokens.LPAR );
			Expression(out oldExpression);
			Expect(Tokens.RPAR );
			expression = new UncheckedExpression(oldExpression);
		}
		else
			SynErr(230);
		if (oldExpression != null && expression != null)
		{
			expression.AddDetailNode(oldExpression);
			oldExpression = null;
		}
		if (expression != null)
			expression.SetRange(GetRange(startRange, tToken));
	}
	void RefValueArguments(out ExpressionCollection arguments)
	{
		arguments = new ExpressionCollection();
		Expression firstArgument = null;
		TypeReferenceExpression typeRef = null;
		Expression(out firstArgument);
		Expect(Tokens.COMMA );
		Type(out typeRef, true);
		if (firstArgument != null)
		 arguments.Add(firstArgument);
		if (typeRef != null)
		  arguments.Add(typeRef);
	}
	void ObjectInitializer(out Expression expression)
	{
		expression = new ObjectInitializerExpression();
		expression.SetRange(la.Range);
		Expect(Tokens.LBRACE );
		if (StartOf(21))
		{
			MemberInitializerList(expression as ObjectInitializerExpression);
		}
		Expect(Tokens.RBRACE );
		expression.SetRange(GetRange(expression, tToken));
	}
	void MemberInitializerList(ObjectInitializerExpression objectInitializer)
	{
		Expression expression = null;
		MemberInitializerExpression(out expression);
		if (expression != null && objectInitializer != null)
		{
			objectInitializer.AddDetailNode(expression);
			objectInitializer.Initializers.Add(expression);
		}
		while (StartOf(32))
		{
			if (la.Type == Tokens.COMMA )
			{
				Get();
			}
			MemberInitializerExpression(out expression);
			if (expression != null && objectInitializer != null)
			{
				objectInitializer.AddDetailNode(expression);
				objectInitializer.Initializers.Add(expression);
			}
		}
	}
	void MemberInitializerExpression(out Expression expression)
	{
		expression = null;
		if (IsMemberInit())
		{
			MemberInitializer(out expression);
		}
		else if (la.Type == Tokens.LBRACE )
		{
			ObjectInitializer(out expression);
		}
		else if (StartOf(17))
		{
			Expression(out expression);
		}
		else
			SynErr(231);
	}
	void MemberInitializer(out Expression result)
	{
		MemberInitializerExpression expression = new MemberInitializerExpression();
		result = expression;
		Expression initializer = null;
		expression.SetRange(la.Range);
		expression.Name = la.Value;
		expression.NameRange = la.Range;
		Expect(Tokens.IDENT );
		Expect(Tokens.ASSGN );
		InitializerValue(out initializer);
		if (expression != null)
		{
			expression.SetRange(GetRange(expression, tToken));
			if (initializer != null)
			{
				expression.Value = initializer;
				expression.AddDetailNode(initializer);
			}
		}
	}
	void InitializerValue(out Expression expression)
	{
		expression = null;
		if (StartOf(17))
		{
			Expression(out expression);
		}
		else if (la.Type == Tokens.LBRACE )
		{
			ObjectInitializer(out expression);
		}
		else
			SynErr(232);
	}
	void TypeArgDirection(out TypeParameterDirection typeArgumentDirection)
	{
		typeArgumentDirection = TypeParameterDirection.None;
		if (la.Type == Tokens.IN  || la.Type == Tokens.OUT )
		{
			if (la.Type == Tokens.IN )
			{
				Get();
				typeArgumentDirection = TypeParameterDirection.In; 
			}
			else
			{
				Get();
				typeArgumentDirection = TypeParameterDirection.Out; 
			}
		}
	}
		void Parse()
		{
				PreparePreprocessor();
				Comments.Clear();
				TextStrings.Clear();
				Regions.Clear();
		FirstGet();
				Parser();
			if (SetTokensCategory)
			  while (la != null && la.Type != 0)
				Get();
				Expect(0);
				if (Context != null)
					Context.SetRange(GetRange(Context, tToken));
				CloseContext();
				BindComments();
		}
		void SetLastFormattingType(FormattingTokenType type)
	{
	  if (LastFormattingParsingElement == null)
		return;
	  LastFormattingParsingElement.FormattingType = type;
	}
		protected override bool[,] CreateSetArray()
		{
			bool[,] set =
			{
			{T,T,T,T, T,T,x,x, T,x,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, x,T,T,T, x,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, T,T,x,x, T,x,x,x, x,x,x,T, T,T,T,x, T,x,T,x, x,x,T,x, T,x,x,T, T,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,x,x, T,x,T,T, T,T,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, x,T,T,T, x,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, T,T,x,x, T,x,x,x, x,x,x,T, T,T,T,x, T,x,T,x, x,x,T,x, T,x,x,x, x,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, T,x,x,T, x,T,x,x, T,x,T,T, x,T,x,T, x,T,x,T, T,T,T,x, x,T,T,x, x,x,x,T, x,T,T,T, x,x,T,x, T,x,x,x, T,x,T,T, T,T,x,x, T,T,T,x, x,T,T,x, x,x,x,x, x,T,T,x, T,T,x,T, T,T,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,x,x, x,x,T,T, T,T,x,x, T,T,x,T, T,T,T,T, T,T,T,x, x,x,x,T, x,T,T,T, T,T,T,x, x,T,x,x, x,T,T,x, T,T,x,T, x,T,x,x, x,x,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,T,x, T,x,T,T, T,T,x,x, T,T,x,x, T,x,x,x, x,x,x,T, T,T,T,x, T,x,T,x, x,x,T,x, T,x,x,x, x,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,T,T, x,T,x,T, x,T,x,T, T,T,x,x, x,T,T,x, x,x,x,T, x,T,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,T,x, x,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{T,T,T,T, T,T,x,x, x,x,T,T, T,T,x,x, T,T,x,T, T,T,T,T, T,T,T,x, x,x,x,T, x,T,T,T, T,T,T,x, x,T,x,x, x,T,T,x, T,T,x,T, x,T,x,x, x,x,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,T,x, T,x,T,T, T,T,x,x, T,T,x,x, T,x,x,x, x,x,x,T, T,T,T,x, T,x,T,x, x,x,T,x, T,x,x,x, x,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,x,x, x,x,T,T, T,T,x,x, T,T,x,x, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,T, T,T,x,x, x,T,x,x, x,T,T,x, T,T,x,T, x,T,x,x, x,x,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,T,x, T,x,T,T, T,T,x,x, x,T,x,x, T,x,x,x, x,x,x,T, x,T,T,x, x,x,T,x, x,x,T,x, T,x,x,x, x,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{T,T,T,T, T,T,x,x, T,x,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, x,T,T,T, x,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, T,T,x,x, T,x,x,x, x,x,x,T, T,T,T,x, T,x,T,x, x,x,T,x, T,x,x,T, x,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,T,x,x, x,x,T,x, T,x,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{T,T,x,x, x,x,x,x, T,x,x,T, x,T,x,x, T,x,T,T, x,T,x,T, x,T,x,T, T,T,T,x, x,T,T,x, x,x,x,T, x,T,T,T, x,x,T,x, T,x,x,x, T,x,T,T, T,T,x,x, T,T,T,x, x,T,T,x, x,x,x,x, x,T,T,x, T,T,x,T, T,T,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, T,x,x,T, x,T,x,x, T,x,T,T, x,T,x,T, x,T,x,T, T,T,T,x, x,T,T,x, x,x,x,T, x,T,T,T, x,x,T,x, T,x,x,x, T,x,T,T, T,T,x,x, T,T,T,x, x,T,T,x, x,x,x,x, x,T,T,x, T,T,x,T, T,T,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{T,T,x,x, x,x,x,x, T,x,x,T, x,T,x,x, T,x,T,T, x,T,x,T, x,T,x,T, T,T,T,x, x,T,T,x, x,x,x,T, x,T,T,T, x,x,T,x, T,x,x,x, T,x,T,T, T,T,x,x, T,T,T,x, x,T,T,x, x,x,x,x, x,T,T,x, T,T,x,T, T,T,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{T,T,T,T, T,T,x,x, T,x,T,T, T,T,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, x,T,T,T, x,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, T,T,x,x, T,x,x,x, x,x,x,T, T,T,T,x, T,x,T,x, x,x,T,x, T,x,x,T, x,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,x,x, x,x,T,T, x,T,x,x, T,T,x,x, x,T,T,T, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, T,T,x,T, x,T,x,x, x,x,T,x, T,x,T,T, T,x,x,x, T,x,T,x, T,T,T,T, x,T,x,x, T,x,x,T, T,T,x,x, x,T,x,x, T,x,x,x, x,x,x,T, x,T,T,x, x,x,T,x, x,x,T,x, T,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,x,x, x,x,T,T, x,T,x,x, T,T,x,x, x,T,T,T, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,x,x,x, T,x,T,x, T,T,T,T, x,T,x,x, T,x,x,T, T,T,x,x, x,T,x,x, T,x,x,x, x,x,x,T, x,x,T,x, x,x,T,x, x,x,T,x, T,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,x,x, x,T,T,T, x,T,x,x, T,T,x,x, x,T,T,T, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,T,x,x, T,x,T,x, T,T,x,T, x,T,x,x, x,x,T,x, T,x,T,T, T,x,x,x, T,x,T,x, T,T,T,T, x,T,x,x, T,x,x,T, T,T,T,T, x,T,T,T, T,T,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,T,T, T,T,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,x,x, x,x,T,T, x,T,x,x, T,T,x,x, x,T,T,T, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, T,T,x,T, x,T,x,x, x,x,T,x, T,x,T,T, T,x,x,x, T,x,T,x, T,T,T,T, x,T,x,x, T,x,x,T, T,T,x,x, x,T,x,x, T,x,x,x, x,x,x,T, T,T,T,x, x,x,T,x, x,x,T,x, T,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,T,x,x, x,x,T,x, T,x,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,T,T, T,T,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,T,x, T,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,x,x, x,x,T,T, x,T,x,x, T,T,x,x, x,T,T,T, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, T,T,x,T, x,T,x,x, x,x,T,x, T,x,T,T, T,x,x,x, T,x,T,x, T,T,T,T, x,T,x,x, T,x,x,T, T,T,x,x, x,T,x,T, T,x,x,x, x,x,x,T, T,T,T,x, x,x,T,x, x,x,T,x, T,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x}
			};
			return set;
		}
	}	
	public class CSharpErrors : ParserErrorsBase
	{
		protected override string GetSyntaxErrorText(int n)
		{
			string s;
			switch (n)
			{
				case 0: s = "EOF expected"; break;
			case 1: s = "IDENT expected"; break;
			case 2: s = "INTCON expected"; break;
			case 3: s = "REALCON expected"; break;
			case 4: s = "CHARCON expected"; break;
			case 5: s = "STRINGCON expected"; break;
			case 6: s = "SHARPCOCODIRECTIVE expected"; break;
			case 7: s = "LINETERMINATOR expected"; break;
			case 8: s = "ABSTRACT expected"; break;
			case 9: s = "AS expected"; break;
			case 10: s = "BASE expected"; break;
			case 11: s = "BOOL expected"; break;
			case 12: s = "BREAK expected"; break;
			case 13: s = "BYTE expected"; break;
			case 14: s = "CASE expected"; break;
			case 15: s = "CATCH expected"; break;
			case 16: s = "CHAR expected"; break;
			case 17: s = "CHECKED expected"; break;
			case 18: s = "CLASS expected"; break;
			case 19: s = "CONST expected"; break;
			case 20: s = "CONTINUE expected"; break;
			case 21: s = "DECIMAL expected"; break;
			case 22: s = "DEFAULT expected"; break;
			case 23: s = "DELEGATE expected"; break;
			case 24: s = "DO expected"; break;
			case 25: s = "DOUBLE expected"; break;
			case 26: s = "ELSE expected"; break;
			case 27: s = "ENUM expected"; break;
			case 28: s = "EVENT expected"; break;
			case 29: s = "EXPLICIT expected"; break;
			case 30: s = "EXTERN expected"; break;
			case 31: s = "FALSE expected"; break;
			case 32: s = "FINALLY expected"; break;
			case 33: s = "FIXED expected"; break;
			case 34: s = "FLOAT expected"; break;
			case 35: s = "FOR expected"; break;
			case 36: s = "FOREACH expected"; break;
			case 37: s = "GOTO expected"; break;
			case 38: s = "IFCLAUSE expected"; break;
			case 39: s = "IMPLICIT expected"; break;
			case 40: s = "IN expected"; break;
			case 41: s = "INT expected"; break;
			case 42: s = "INTERFACE expected"; break;
			case 43: s = "INTERNAL expected"; break;
			case 44: s = "IS expected"; break;
			case 45: s = "LOCK expected"; break;
			case 46: s = "LONG expected"; break;
			case 47: s = "NAMESPACE expected"; break;
			case 48: s = "NEW expected"; break;
			case 49: s = "NULL expected"; break;
			case 50: s = "OPERATOR expected"; break;
			case 51: s = "OUT expected"; break;
			case 52: s = "OVERRIDE expected"; break;
			case 53: s = "PARAMS expected"; break;
			case 54: s = "PRIVATE expected"; break;
			case 55: s = "PROTECTED expected"; break;
			case 56: s = "PUBLIC expected"; break;
			case 57: s = "READONLY expected"; break;
			case 58: s = "REF expected"; break;
			case 59: s = "RETURN expected"; break;
			case 60: s = "SBYTE expected"; break;
			case 61: s = "SEALED expected"; break;
			case 62: s = "SHORT expected"; break;
			case 63: s = "SIZEOF expected"; break;
			case 64: s = "STACKALLOC expected"; break;
			case 65: s = "STATIC expected"; break;
			case 66: s = "STRUCT expected"; break;
			case 67: s = "SWITCH expected"; break;
			case 68: s = "THIS expected"; break;
			case 69: s = "THROW expected"; break;
			case 70: s = "TRUE expected"; break;
			case 71: s = "TRY expected"; break;
			case 72: s = "TYPEOF expected"; break;
			case 73: s = "UINT expected"; break;
			case 74: s = "ULONG expected"; break;
			case 75: s = "UNCHECKED expected"; break;
			case 76: s = "UNSAFE expected"; break;
			case 77: s = "USHORT expected"; break;
			case 78: s = "USINGKW expected"; break;
			case 79: s = "VIRTUAL expected"; break;
			case 80: s = "VOID expected"; break;
			case 81: s = "VOLATILE expected"; break;
			case 82: s = "WHILE expected"; break;
			case 83: s = "ARGLIST expected"; break;
			case 84: s = "REFVALUE expected"; break;
			case 85: s = "AND expected"; break;
			case 86: s = "ANDASSGN expected"; break;
			case 87: s = "ASSGN expected"; break;
			case 88: s = "ATCOLON expected"; break;
			case 89: s = "AT expected"; break;
			case 90: s = "COLON expected"; break;
			case 91: s = "COMMA expected"; break;
			case 92: s = "DEC expected"; break;
			case 93: s = "DIVASSGN expected"; break;
			case 94: s = "DOT expected"; break;
			case 95: s = "DBLCOLON expected"; break;
			case 96: s = "EQ expected"; break;
			case 97: s = "GT expected"; break;
			case 98: s = "GTEQ expected"; break;
			case 99: s = "INC expected"; break;
			case 100: s = "LBRACE expected"; break;
			case 101: s = "LBRACK expected"; break;
			case 102: s = "LPAR expected"; break;
			case 103: s = "LSHASSGN expected"; break;
			case 104: s = "LT expected"; break;
			case 105: s = "LTLT expected"; break;
			case 106: s = "MINUS expected"; break;
			case 107: s = "MINUSASSGN expected"; break;
			case 108: s = "MODASSGN expected"; break;
			case 109: s = "NEQ expected"; break;
			case 110: s = "NOT expected"; break;
			case 111: s = "ORASSGN expected"; break;
			case 112: s = "PLUS expected"; break;
			case 113: s = "PLUSASSGN expected"; break;
			case 114: s = "QUESTION expected"; break;
			case 115: s = "RBRACE expected"; break;
			case 116: s = "RBRACK expected"; break;
			case 117: s = "RPAR expected"; break;
			case 118: s = "SCOLON expected"; break;
			case 119: s = "TILDE expected"; break;
			case 120: s = "TIMES expected"; break;
			case 121: s = "TIMESASSGN expected"; break;
			case 122: s = "XORASSGN expected"; break;
			case 123: s = "POINTERTOMEMBER expected"; break;
			case 124: s = "SINGLELINECOMMENT expected"; break;
			case 125: s = "MULTILINECOMMENT expected"; break;
			case 126: s = "RAZORCOMMENT expected"; break;
			case 127: s = "DBLQUEST expected"; break;
			case 128: s = "DBLOR expected"; break;
			case 129: s = "DBLAND expected"; break;
			case 130: s = "OR expected"; break;
			case 131: s = "XOR expected"; break;
			case 132: s = "LOWOREQ expected"; break;
			case 133: s = "DIV expected"; break;
			case 134: s = "MOD expected"; break;
			case 135: s = "POINT expected"; break;
			case 136: s = "LAMBDA expected"; break;
			case 137: s = "DEFINE expected"; break;
			case 138: s = "UNDEF expected"; break;
			case 139: s = "IFDIR expected"; break;
			case 140: s = "ELIF expected"; break;
			case 141: s = "ELSEDIR expected"; break;
			case 142: s = "ENDIF expected"; break;
			case 143: s = "LINE expected"; break;
			case 144: s = "ERROR expected"; break;
			case 145: s = "WARNING expected"; break;
			case 146: s = "REGION expected"; break;
			case 147: s = "ENDREG expected"; break;
			case 148: s = "PRAGMADIR expected"; break;
			case 149: s = "SINGLELINEXML expected"; break;
			case 150: s = "MULTILINEXML expected"; break;
			case 151: s = "GTGT expected"; break;
			case 152: s = "RSHASSGN expected"; break;
			case 153: s = "ASPBLOCKSTART expected"; break;
			case 154: s = "ASPBLOCKEND expected"; break;
			case 155: s = "ASPCOMMENT expected"; break;
			case 156: s = "??? expected"; break;
			case 157: s = "invalid NamespaceMemberDeclaration"; break;
			case 158: s = "invalid AccessorDeclarations"; break;
			case 159: s = "invalid AccessorDeclarations"; break;
			case 160: s = "invalid ClassMemberDeclaration"; break;
			case 161: s = "invalid ClassMemberDeclaration"; break;
			case 162: s = "this symbol not expected in Statement"; break;
			case 163: s = "invalid Statement"; break;
			case 164: s = "this symbol not expected in Statement"; break;
			case 165: s = "invalid TypeDeclaration"; break;
			case 166: s = "this symbol not expected in TypeDeclaration"; break;
			case 167: s = "invalid TypeDeclaration"; break;
			case 168: s = "this symbol not expected in ClassBody"; break;
			case 169: s = "this symbol not expected in StructBody"; break;
			case 170: s = "invalid IntegralType"; break;
			case 171: s = "invalid StructMemberDeclaration"; break;
			case 172: s = "invalid StructMemberDeclaration"; break;
			case 173: s = "invalid StructMemberDeclaration"; break;
			case 174: s = "invalid BlockCore"; break;
			case 175: s = "invalid Expression"; break;
			case 176: s = "invalid Expression"; break;
			case 177: s = "invalid VariableInitializer"; break;
			case 178: s = "this symbol not expected in ConstructorDeclaration"; break;
			case 179: s = "invalid ConstructorDeclaration"; break;
			case 180: s = "invalid ConstructorDeclaration"; break;
			case 181: s = "invalid OverloadableOperator"; break;
			case 182: s = "invalid OverloadableOp"; break;
			case 183: s = "this symbol not expected in MethodDeclaration"; break;
			case 184: s = "invalid MethodDeclaration"; break;
			case 185: s = "this symbol not expected in IndexerDeclaration"; break;
			case 186: s = "invalid CastOperator"; break;
			case 187: s = "this symbol not expected in CastOperator"; break;
			case 188: s = "invalid CastOperator"; break;
			case 189: s = "invalid EventDeclaration"; break;
			case 190: s = "invalid LocalVariableDeclaration"; break;
			case 191: s = "invalid ForVariableDeclaration"; break;
			case 192: s = "invalid UsingVariableDeclaration"; break;
			case 193: s = "invalid ParameterDeclaration"; break;
			case 194: s = "invalid Name"; break;
			case 195: s = "invalid EventAccessorDeclarations"; break;
			case 196: s = "invalid EventAccessorDeclarations"; break;
			case 197: s = "invalid Attributes"; break;
			case 198: s = "invalid Keyword"; break;
			case 199: s = "invalid SimpleType"; break;
			case 200: s = "invalid PrimitiveLanguageType"; break;
			case 201: s = "invalid PointerOrArray"; break;
			case 202: s = "invalid RazorHtmlCode"; break;
			case 203: s = "invalid EmbeddedStatement"; break;
			case 204: s = "invalid EmbeddedStatement"; break;
			case 205: s = "invalid EmbeddedStatement"; break;
			case 206: s = "invalid EmbeddedStatement"; break;
			case 207: s = "invalid ForInitializer"; break;
			case 208: s = "invalid ResourceAcquisition"; break;
			case 209: s = "invalid CatchClauses"; break;
			case 210: s = "invalid SwitchLabel"; break;
			case 211: s = "invalid AssignmentOperator"; break;
			case 212: s = "invalid LambdaExpression"; break;
			case 213: s = "this symbol not expected in LambdaExpression"; break;
			case 214: s = "invalid Unary"; break;
			case 215: s = "invalid Unary"; break;
			case 216: s = "invalid QueryIdent"; break;
			case 217: s = "invalid FromLetWhereClause"; break;
			case 218: s = "invalid SelectOrGroupClause"; break;
			case 219: s = "invalid SelectOrGroupClause"; break;
			case 220: s = "invalid LambdaExpressionBody"; break;
			case 221: s = "invalid LambdaParameter"; break;
			case 222: s = "invalid RelExpr"; break;
			case 223: s = "invalid RelExpr"; break;
			case 224: s = "invalid RelExpr"; break;
			case 225: s = "invalid Primary"; break;
			case 226: s = "invalid Literal"; break;
			case 227: s = "invalid ObjectCreationExpression"; break;
			case 228: s = "invalid ObjectCreationExpression"; break;
			case 229: s = "invalid TypeOfSizeOfDefaultExpression"; break;
			case 230: s = "invalid CheckedUncheckedExpression"; break;
			case 231: s = "invalid MemberInitializerExpression"; break;
			case 232: s = "invalid InitializerValue"; break;
				default:
					s = "error " + n;
					break;
			}
			return s;
		}
	}
}
