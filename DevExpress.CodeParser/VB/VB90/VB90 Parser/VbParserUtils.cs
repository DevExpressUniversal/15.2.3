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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
	public partial class VB90Parser : FormattingParserBase
	{
		LanguageElementCollection _MissedAttributes;
		TokenQueueBase _MissedModifiers;
		void SetMissedAttributesAndModifiersIfNeeded(LanguageElementCollection missedAttributes, TokenQueueBase missedModifiers)
		{
			if (LaIsStartPropertyAccessor())
				return;
			if (missedAttributes != null && missedAttributes.Count != 0)
				_MissedAttributes = missedAttributes;
			if (missedModifiers != null && missedModifiers.Count != 0)
				_MissedModifiers = missedModifiers;
		}
		bool LaIsStartPropertyAccessor()
		{
			return (la.Type == Tokens.Get || la.Type == Tokens.Set);
		}
		bool GetMissedAttributesAndModifiers(out LanguageElementCollection missedAttributes, out TokenQueueBase missedModifiers)
		{
			missedAttributes = null;
			missedModifiers = null;
			if ((_MissedAttributes == null || _MissedAttributes.Count == 0) && (_MissedModifiers == null || _MissedModifiers.Count == 0))
				return false;
			missedAttributes = _MissedAttributes;
			missedModifiers = _MissedModifiers;
			ResetMissedAttributesAndModifiers();
			return true;
		}
		void ResetMissedAttributesAndModifiers()
		{
			_MissedAttributes = null;
			_MissedModifiers = null;
		}
		protected void ParseRootRule()
		{
			ParserRoot();
		}
	protected string GetAccessorName(string prefix)
	{
	  Member member = Context as Member;
	  if (member != null && member.ImplementsCount > 0)
	  {
		QualifiedElementReference reference = member.ImplementsExpressions[0] as QualifiedElementReference;
		if (reference != null && reference.Qualifier != null)
		  return reference.Qualifier.ToString() + '.' + prefix + member.GetNameWithoutImplementsQualifier();
	  }
	  return prefix + Context.Name;
	}
		protected void CallAppropriateParseRule(LanguageElement context)
		{
			if (context == null)
				return;
			LanguageElementCollection accessors = new LanguageElementCollection();
			StatementTerminatorCall();
			if (context is Member || context is Statement || context is Enumeration)
			{
				_DeclarationsCache.Reset();
				if (context.ElementType == LanguageElementType.Property)
				{
		  string aceessorName = Context.Name;
					if (aceessorName == null)
						aceessorName = String.Empty;
					SourceRange dummyRange = SourceRange.Empty;
					PropertyAccessorDeclarationList(accessors, aceessorName, ref dummyRange);
					SetPropertyAccessors(RootNode as VBProperty, accessors);
				}
				else if (context.ElementType == LanguageElementType.Event)
				{
					EventAccessorDeclarations();
				}
				else if (context.ElementType == LanguageElementType.Enum)
				{
					EnumBody();
				}
				else
				{
					SourceRange endRange = SourceRange.Empty;
					BlockRule(context.Range, BlockType.None, true, out endRange);
					ParseMemberEnd();
				}
			}
			else
			{
				ParserRoot();
			}
		}
		protected override LanguageElement DoParse(ParserContext parserContext, ISourceReader reader)
		{
			try
			{
		_ExpressionNestingLevel = 0;
		SaveFormat = parserContext.SaveUserFormat;
				LanguageElement context = parserContext.Context;
				if (context == null)
					return null;
				SetRootNode(context);
				if (context is SourceFile && parserContext != null)
				{
					((SourceFile)context).SetDocument(parserContext.Document);
				}
				scanner = new VB90Scanner(reader);
				PrepareParse(context);
				CallAppropriateParseRule(context);
				if (parserContext != null && Comments != null)
					parserContext.Comments.AddRange(Comments);
				if (parserContext != null)
					parserContext.ContextAfterParse = Context;
		FinishParse(RootNode);
				if (parserContext != null)
					parserContext.ContextAfterParse = Context;
				return RootNode;
			}
			finally
			{
				CleanUpParser();
		_ExpressionNestingLevel = 0;
				if (reader != null)
				{
					reader.Dispose();
					reader = null;
				}
			}
		}
		protected int ConstructedTypeNameTailBase(out ExpressionCollection arguments)
		{
			int arity = 0;
			ConstructedTypeNameTail(out arguments, out arity);
			return arity;
		}
		void QualifiedIdentifier(out QualifiedIdentifier identifier)
		{
			QualifiedIdentifier(out identifier, CreateElementType.ElementReferenceExpression);
		}
		void ObjectCreationExpressionCore(out Expression result, out VBDeclarator decl)
		{
			decl = new VBDeclarator(CreateElementType.ObjectCreationExpression);
			CreationExpressionCore(out result, ref decl);
		}
		void ArrayCreationExpressionCore(out Expression result)
		{
			VBDeclarator decl = new VBDeclarator(CreateElementType.ArrayCreationExpression);
			CreationExpressionCore(out result, ref decl);
		}
		void DelegateCreationExpressionCore(out Expression result)
		{
			VBDeclarator decl = new VBDeclarator(CreateElementType.DelegateCreationExpression);
			CreationExpressionCore(out result, ref decl);
		}
		public static string GetSimpleTypeName(string fullName)
		{
			return VB90Tokens.Instance.GetSimpleTypeName(fullName);
		}
		protected void StatementRuleBase(out Statement result)
		{
			StatementRule(out result);
		}
		protected TypeReferenceExpression ParseTypeExpressionRule(out TypeReferenceExpression result)
		{
			result = null;
			VBDeclarator decl = null;
			TypeName(out result, out decl);
			return result;
		}
		protected void ExpressionRuleBase(out Expression result)
		{
			WhitespaceLines();
			ExpressionRule(out result);
		}
		protected void LocalDeclarationStatementCoreBase(out BaseVariable result)
		{
			LanguageElement element = null;
			LocalDeclarationStatementCore(out element, false);
			result = element as BaseVariable;
		}
		protected void ClassMemberDeclarationBase(out Member result)
		{
			LanguageElement element = null;
			ElementMemberDeclaration(out element);
			StatementTerminator();
			result = element as Member;
		}
		protected void WhitespaceLinesBase()
		{
			WhitespaceLines();
		}
		void ParseMemberEnd()
		{
			if (Context == null)
				return;
			LanguageElement oldContext = Context;
			LanguageElementType elementType = oldContext.ElementType;
			if (elementType == LanguageElementType.Method)
				ParseMethodEnd();
			if (oldContext != Context)
				CallAppropriateParseRule(Context);
		}
		void ParseMethodEnd()
		{
			if (!IsDeclEnd())
				return;
			_DeclarationsCache.CloseScope();
			ReadBlockEnd(la.Range);
			SetBlockRangeForEmptyContext();
			Get();
			Get();
			StatementTerminatorCall();
			CloseContext();
		}
		void SetBlockRangeForEmptyContext()
		{
			IHasBlock bl = Context as IHasBlock;
			if (bl == null)
				return;
			SetBlockRangeForEmptyElement(Context, bl.BlockStart, la.Range);
		}
		bool HasContextElementType(params LanguageElementType[] elementTypes)
		{
			if (elementTypes == null || Context == null)
				return false;
			LanguageElementType contextType = Context.ElementType;
			foreach (LanguageElementType elementType in elementTypes)
				if (elementType == contextType)
					return true;
			return false;
		}
		bool IsCorruptedElement()
		{
			return IsNamespaceCorruptedEnd() || IsTypeCorruptedEnd() || IsMemberCorruptedEnd();
		}
		bool IsNamespaceCorruptedEnd()
		{
			if (la == null || la.Type != Tokens.EndToken)
				return false;
			if (!HasContextElementType(LanguageElementType.SourceFile))
				return false;
			if (IsPeekToken(Tokens.Namespace))
				return true;
			return false;
		}
		bool IsTypeCorruptedEnd()
		{
			if (la == null || la.Type != Tokens.EndToken)
				return false;
			if (!HasContextElementType(LanguageElementType.SourceFile, LanguageElementType.Namespace))
				return false;
			if (IsPeekToken(Tokens.Class, Tokens.Structure, Tokens.Enum, Tokens.Module, Tokens.Interface))
				return true;
			return false;
		}
		bool IsMemberCorruptedEnd()
		{
			if (la == null || la.Type != Tokens.EndToken)
				return false;
			if (!HasContextElementType(LanguageElementType.SourceFile, LanguageElementType.Namespace,
				LanguageElementType.Class, LanguageElementType.Struct, LanguageElementType.Enum,
				LanguageElementType.Module, LanguageElementType.Interface))
				return false;
			if (IsPeekToken(Tokens.Function, Tokens.Sub, Tokens.Operator,
				Tokens.RaiseEvent, Tokens.RemoveHandler, Tokens.AddHandler, Tokens.Event))
				return true;
			return false;
		}
		void SetCorruptedType(Token token, CorruptedLanguageElement element)
		{
			if (token == null || element == null)
				return;
			int type = token.Type;
			CorruptedType corruptedType = CorruptedType.Block;
			switch (type)
			{
				case Tokens.Class:
					corruptedType = CorruptedType.Class;
					break;
				case Tokens.Structure:
					corruptedType = CorruptedType.Struct;
					break;
				case Tokens.Module:
					corruptedType = CorruptedType.Module;
					break;
				case Tokens.Interface:
					corruptedType = CorruptedType.Interface;
					break;
				case Tokens.Event:
					corruptedType = CorruptedType.Event;
					break;
				case Tokens.Namespace:
					corruptedType = CorruptedType.Namespace;
					break;
				case Tokens.AddHandler:
					corruptedType = CorruptedType.AddHandler;
					break;
				case Tokens.RemoveHandler:
					corruptedType = CorruptedType.RemoveHandler;
					break;
				case Tokens.RaiseEvent:
					corruptedType = CorruptedType.RaiseEvent;
					break;
				case Tokens.Get:
					corruptedType = CorruptedType.Get;
					break;
				case Tokens.Set:
					corruptedType = CorruptedType.Set;
					break;
				case Tokens.Enum:
					corruptedType = CorruptedType.Enum;
					break;
				case Tokens.Property:
					corruptedType = CorruptedType.Property;
					break;
				case Tokens.Sub:
					corruptedType = CorruptedType.Sub;
					break;
				case Tokens.Function:
					corruptedType = CorruptedType.Function;
					break;
				case Tokens.Operator:
					corruptedType = CorruptedType.Operator;
					break;
			}
			element.CorruptedType = corruptedType;
		}
	}
}
