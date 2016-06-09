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
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  using Base;
	public class CastTargetExpression : TypeCastExpression
	{
		#region private fields...
		Token _CastToken;
		string _CastKeyword;
		string _CastTypeName;
		#endregion
		#region CastTargetExpression
		protected CastTargetExpression()
		{
		}
		#endregion		
		#region CastTargetExpression
		public CastTargetExpression(string castTypeName, string castKeyword,SourceRange castRange, Expression target)
		{
			SetTarget(target);
			_CastKeyword = castKeyword;
			_CastTypeName = castTypeName;
			SetStart(castRange);
			TypeReferenceExpression castTypeRef = new TypeReferenceExpression(castTypeName, castRange);
			SetTypeReference(castTypeRef);
		}
		public CastTargetExpression(Token castToken, Expression target)
		{
			SetTarget(target);
			_CastToken = castToken;
			SetStart(castToken.Range);
			SetTypeReference(GetTypeReference(castToken));
		}
	public CastTargetExpression(TypeReferenceExpression type, Expression target)
	  : base(type, target)
	{
	}
		#endregion
		#region GetTypeFromCastToken
		protected string GetTypeFromCastToken(Token token)
		{
			switch (token.Type)
			{
				case TokenType.CBool:
					return "Boolean";
				case TokenType.CByte:
					return "Byte";
				case TokenType.CChar:
					return "Char";
				case TokenType.CDate:
					return "Date";
				case TokenType.CDec:
					return "Decimal";
				case TokenType.CDbl:
					return "Double";
				case TokenType.Cint:
					return "Integer";
				case TokenType.Clng:
					return "Long";
				case TokenType.Cobj:
					return "Object";
				case TokenType.Cshort:
					return "Short";
				case TokenType.Csng:
					return "Single";
				case TokenType.Cstr:
					return "String";
			}
			return String.Empty;
		}
		#endregion
		#region GetTypeCastKeyword
		protected string GetTypeCastKeyword(Token token)
		{
			switch (token.Type)
			{
				case TokenType.CBool:
					return "CBool";
				case TokenType.CByte:
					return "CByte";
				case TokenType.CChar:
					return "CChar";
				case TokenType.CDate:
					return "CDate";
				case TokenType.CDec:
					return "CDec";
				case TokenType.CDbl:
					return "CDbl";
				case TokenType.Cint:
					return "CInt";
				case TokenType.Clng:
					return "CLng";
				case TokenType.Cobj:
					return "CObj";
				case TokenType.Cshort:
					return "CShort";
				case TokenType.Csng:
					return "CSng";
				case TokenType.Cstr:
					return "CStr";
			}
			return String.Empty;
		}
		#endregion
		#region GetTypeReference
		protected TypeReferenceExpression GetTypeReference(Token token)
		{
			string lTypeName = GetTypeFromCastToken(token);
			if (lTypeName.Length > 0)
				return new TypeReferenceExpression(lTypeName, token.Range);
			return null;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is CastTargetExpression))
				return;
			CastTargetExpression lSource = (CastTargetExpression)source;
			if (lSource == null)
		return;
	  if (lSource._CastToken != null)
	  {
		_CastToken = lSource._CastToken.Clone();			
	  }
	  _CastKeyword = lSource.CastKeyword;
	  _CastTypeName = lSource.CastTypeName;
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			string lResult = CastKeyword + "(";
			if (Target != null)
				lResult += Target.ToString();
			lResult += ")";
			return lResult;
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			CastTargetExpression lClone = new CastTargetExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region ElementType
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.CastTargetExpression;
			}
		}
		#endregion
		#region CastToken
		public Token CastToken
		{
			get
			{
				return _CastToken;
			}
		}
		#endregion
		#region CastTypeName
		public string CastTypeName
		{
			get
			{
				if (_CastTypeName != null && _CastTypeName != String.Empty)
					return _CastTypeName;
				if(CastToken != null)
					return GetTypeFromCastToken(CastToken);
				return String.Empty;
			}
			set
			{
				_CastTypeName = value;
			}
		}
		#endregion
		#region CastKeyword
		public string CastKeyword
		{
			get
			{
				if (_CastKeyword != null && _CastKeyword != String.Empty)
					return _CastKeyword;
				if(CastToken != null)
					GetTypeCastKeyword(CastToken);
				return String.Empty;
			}
			set
			{
				_CastKeyword = value;
			}		
		}
		#endregion
	}
}
