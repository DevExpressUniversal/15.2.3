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
#if SL
using DevExpress.Utils;
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
	public enum DeclaratorType
	{
		Const,
		Dim,
		Static,
		QueryIdent,
		CanAggregateFunction,
		CanAggregateElement,
		None
	}
  public class VariableListHelper
  {
	TypeReferenceExpression _Type = null;
	Expression _Initializer = null;
	SourceRange _OperatorRange = SourceRange.Empty;
	SourceRange _AsRange = SourceRange.Empty;
	bool _IsObjectCreation = false;
	bool _AddToContext = false;
	public VariableListHelper(bool addToContext)
	{
	  _AddToContext = addToContext;
	}
	public bool AddToContext
	{
	  get
	  {
		return _AddToContext;
	  }
	  set
	  {
		_AddToContext = value;
	  }
	}
	public SourceRange AsRange
	{
	  get
	  {
		return _AsRange;
	  }
	  set
	  {
		_AsRange = value;
	  }
	}
	public Expression Initializer
	{
	  get
	  {
		return _Initializer;
	  }
	  set
	  {
		_Initializer = value;
	  }
	}
	public bool IsObjectCreation
	{
	  get
	  {
		return _IsObjectCreation;
	  }
	  set
	  {
		_IsObjectCreation = value;
	  }
	}
	public SourceRange OperatorRange
	{
	  get
	  {
		return _OperatorRange;
	  }
	  set
	  {
		_OperatorRange = value;
	  }
	}
	public TypeReferenceExpression Type
	{
	  get
	  {
		return _Type;
	  }
	  set
	  {
		_Type = value;
	  }
	}
  }
	public class DeclarationsCache
	{			
		ArrayList _Scopes;
		Hashtable _VisibleDeclarations;
		public DeclarationsCache()
		{
			_Scopes = new ArrayList();
			_VisibleDeclarations = new Hashtable();
		}
		Hashtable ActiveScope
		{
			get
			{
				if (_Scopes == null || _Scopes.Count == 0)
					return null;
				int lastIndex = _Scopes.Count - 1;
				Hashtable	activeScope = _Scopes[lastIndex] as Hashtable;
				return activeScope;
			}
		}
		public DeclarationsCache GetClone()
		{
			DeclarationsCache clone = new DeclarationsCache();
			clone._Scopes = _Scopes.Clone() as ArrayList;
			clone._VisibleDeclarations = _VisibleDeclarations.Clone() as Hashtable;
			return clone;
		}
		public void AddDeclaration(LanguageElement element)
		{
			if (element == null)
				return;
			Hashtable activeScope = ActiveScope;
			if (activeScope == null)
				return;
			activeScope[element.Name] = element;
		}
		public void OpenScope()
		{
			if (_Scopes == null)
				return;
			Hashtable hash = HashtableUtils.CreateHashtable(false);
			_Scopes.Add(hash);
		}
		public void CloseScope()
		{
			if (_Scopes == null || _Scopes.Count == 0)
				return;
			int lastIndex = _Scopes.Count - 1;
			_Scopes.RemoveAt(lastIndex);
		}
		public LanguageElement GetDeclaration(string name)
		{
			if (name == null || name.Length == 0)
				return null;
			int lastIndex = _Scopes.Count - 1;
			for(int i = lastIndex; i >= 0 ; i--)
			{
				Hashtable currentHashtable = _Scopes[i] as Hashtable;
					if (currentHashtable != null && currentHashtable.Contains(name))
					{
						return currentHashtable[name] as LanguageElement;
					}
			}
			return null;
		}
		public bool HasDelclaration(string name)
		{
			return  GetDeclaration(name) != null;
		}
		public void Reset()
		{
			_Scopes.Clear();
			_VisibleDeclarations.Clear();
		}
		public void Remove(LanguageElement element)
		{
			Hashtable scope = ActiveScope;
			if (scope == null || element == null || element.Name == null || element.Name == String.Empty)
				return;
			scope.Remove(element.Name);
		}
	}
	public enum BlockType
	{
		None,
		IfSingleLineStatement
	}
	public class MethodHelper
	{
		public bool IsMustInherit = false;
		TokenQueueBase _Modifiers;
		public MethodHelper(TokenQueueBase modifiers)
		{
			_Modifiers = modifiers;
		}
		public bool WithoutBody()
		{
			if (_Modifiers == null || _Modifiers.Count == 0)
				return false;
			int count = _Modifiers.Count;
			for (int i = 0; i < count; i++)
			{
				Token token = _Modifiers.Peek(i);
				if (token.Type == Tokens.MustOverride)
					return true;
			}		
			return false;
		}
	}	
	public class OnDemandParsingParameters
	{
		Token _StartBlockToken = null;
		Token _EndBlockToken = null;
		public OnDemandParsingParameters(Token startBlockToken, Token endBlockToken)
		{
			_StartBlockToken = startBlockToken;
			_EndBlockToken = endBlockToken;
		}
		public Token StartBlockToken
		{
			get
			{
				return _StartBlockToken;
			}
		}
		public Token EndBlockToken
		{
			get
			{
				return _EndBlockToken;
			}
		}
	}
	public class VariableNameCollection: CollectionBase
	{
		class NestedVarColl
		{
			public VBDeclarator Declarator = null;
			public string Name = null;
			public SourceRange Range = SourceRange.Empty;
	  public SourceRange NameRange = SourceRange.Empty;
			public NestedVarColl(VBDeclarator decl, string name, SourceRange range, SourceRange nameRange)
			{
				Declarator = decl;
				Name = name;
				Range = range;
		NameRange = nameRange;
			}		
		}		
		public void Add(string name, VBDeclarator decl, SourceRange range, SourceRange nameRange)
		{
			NestedVarColl element = new NestedVarColl(decl, name, range, nameRange);
			InnerList.Add(element);
		}
		public new int Count 
		{
			get
			{
				if (InnerList == null)
					return 0;
				return InnerList.Count;
			}
		}
		public VBDeclarator GetDeclarator(int index)
		{
			if (InnerList == null)
				return null;
			NestedVarColl element = InnerList[index] as NestedVarColl;
			if (element == null)
				return null;
			return element.Declarator;
		}
		public SourceRange GetRange(int index)
		{
			if (InnerList == null)
				return SourceRange.Empty;
			NestedVarColl element = InnerList[index] as NestedVarColl;
			if (element == null)
				return SourceRange.Empty;
			return element.Range;
		}
	public SourceRange GetNameRange(int index)
	{
	  if (InnerList == null)
		return SourceRange.Empty;
	  NestedVarColl element = InnerList[index] as NestedVarColl;
	  if (element == null)
		return SourceRange.Empty;
	  return element.NameRange;
	}
		public string this[int index]
		{
			get
			{
				if (InnerList == null)
					return null;
				NestedVarColl element = InnerList[index] as NestedVarColl;
				if (element == null)
					return null;
				return element.Name;
			}
		}
	}
	public class VBDeclarator
	{
		string _FullTypeName;
		CreateElementType _CreateElementType;
		LanguageElementCollection _ArrayModifiers = null;
		NullableTypeModifier _NullableModifier = null;
		TypeReferenceExpression _CharacterType = null;
		public VBDeclarator()
		{
			_FullTypeName = String.Empty;
		}
		public VBDeclarator(CreateElementType createElementType)
			: this()
		{			
			_CreateElementType = createElementType;
		}
		public void AddArrayModifier(ArrayNameModifier modifier)
		{
			if (modifier == null)
				return;
			if (_ArrayModifiers == null)
				_ArrayModifiers = new LanguageElementCollection();
			_ArrayModifiers.Add(modifier);
		}
		public LanguageElementCollection ArrayModifiers
		{
			get
			{
				return _ArrayModifiers;
			}
		}
		public NullableTypeModifier NullableModifier
		{
			get
			{
				return _NullableModifier;
			}
			set
			{
				_NullableModifier = value;
			}
		}
		public CreateElementType CreateElementType
		{
			set
			{
				_CreateElementType	= value;
			}
			get
			{
				return _CreateElementType;
			}
		}
		public string FullTypeName
		{
			set
			{
				_FullTypeName = value;
			}
			get
			{
				return _FullTypeName;
			}
		}
		public TypeReferenceExpression CharacterType
		{
			get
			{
				return _CharacterType;
			}
			set
			{
				_CharacterType = value;
			}
		}
	}
	public class SubMemberData
	{
		public Token NameToken;
		public LanguageElementCollection ParamCollection = null;
		public TypeReferenceExpression MemberTypeReference = null;
		public SourceRange TypeRange = SourceRange.Empty;
		public GenericModifier GenericModifier;
		public SourceRange NameRange = SourceRange.Empty;
		ExpressionCollection _HandlesExpressions = null;
		ExpressionCollection _ImplementCollection = null;
		Expression _Initializer = null;
		string _LibString = String.Empty;
		string _AliasString = String.Empty;
		SourceRange _ParamOpenRange = SourceRange.Empty;
		SourceRange _ParamCloseRange = SourceRange.Empty;
	SourceRange _AsRange = SourceRange.Empty;
		public Expression Initializer
		{
			get
			{
				return _Initializer;
			}
			set
			{
				_Initializer = value;
			}
		}
	public SourceRange AsRange
	{
	  get
	  {
		return _AsRange;
	  }
	  set
	  {
		_AsRange = value;
	  }
	}
	public SourceRange ParamOpenRange
		{
			set
			{
				_ParamOpenRange = value;
			}
			get
			{
				return _ParamOpenRange;
			}
		}
		public SourceRange ParamCloseRange
		{
			set
			{
				_ParamCloseRange = value;
			}
			get
			{
				return _ParamCloseRange;
			}
		}
		public string Name
		{
			get
			{
				if (NameToken == null)
					return String.Empty;
				return NameToken.Value;
			}
		}
		public string LibString
		{
			get
			{
				return _LibString;
			}
			set
			{
				_LibString = value;
			}
		}
		public string AliasString
		{
			get
			{
				return _AliasString;
			}
			set
			{
				_AliasString = value;
			}
		}
		public void AddImplement(ElementReferenceExpression element)
		{
			if (_ImplementCollection == null)
				_ImplementCollection = new ExpressionCollection();
			_ImplementCollection.Add(element);
		}
		public ExpressionCollection ImplementsCollection
		{
			get
			{
				return _ImplementCollection;
			}
		}
		public ExpressionCollection HandlesExpressions
		{
			get
			{
				if (_HandlesExpressions == null)
					_HandlesExpressions = new ExpressionCollection();
				return _HandlesExpressions;
			}
			set
			{
				_HandlesExpressions = value;
			}
		}
	}
	public enum TypeDeclarationEnum
	{
		Class,
		Struct,
		Interface,
		Module
	}
	public class LocalVarArrayCollection
	{
		StringCollection _VarNames;
		public LocalVarArrayCollection()
		{			
		}
		public void AddVarArrayName(BaseVariable var)
		{
			if (var == null || var.Name == null || var.Name == String.Empty)
				return;
			TypeReferenceExpression type = var.MemberTypeReference;
			if (type == null)
				return;
			if (!type.IsArrayType)
			{
				LanguageElementCollection coll = var.ArrayNameModifiers;
				if (coll == null && coll.Count ==0)
					return;
			}
			string name = var.Name;
			if (_VarNames == null)
			{
				_VarNames	= new StringCollection();
			}
			_VarNames.Add(name);
		}
		public bool IsVarArrayName(string name)
		{
			return false;
		}
		public void Clear()
		{
			if (_VarNames != null)
				_VarNames.Clear();
		}
	}
}
