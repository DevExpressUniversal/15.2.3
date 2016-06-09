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
using System.Runtime.Serialization;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	[Serializable]
	public class EmptyLiteElements
	{
		EmptyLiteElements() {}
		public static readonly IElementCollection EmptyIElementCollection = new IElementCollection();
		public static readonly LiteTypeElementCollection EmptyITypeElementCollection = new LiteTypeElementCollection();
		public static readonly LiteNamespaceElementCollection EmptyINamespaceElementCollection = new LiteNamespaceElementCollection();
		public static readonly LiteExpressionCollection EmptyIExpressionCollection = new LiteExpressionCollection();
		public static readonly LiteTypeReferenceExpressionCollection EmptyITypeReferenceExpressionCollection = new LiteTypeReferenceExpressionCollection();
		public static readonly LiteMemberElementCollection EmptyIMemberElementCollection = new LiteMemberElementCollection();
		public static readonly LiteTypeParameterCollection EmptyITypeParameterCollection = new LiteTypeParameterCollection();
		public static readonly LiteVariableDeclarationStatementCollection EmptyIVariableDeclarationStatementCollection = new LiteVariableDeclarationStatementCollection();
		public static readonly LiteCaseClauseCollection EmptyICaseClauseCollection = new LiteCaseClauseCollection();
		public static readonly LiteCaseStatementCollection EmptyICaseStatementCollection = new LiteCaseStatementCollection();
		public static readonly LiteParameterElementCollection EmptyIParameterElementCollection = new LiteParameterElementCollection();
		public static readonly LiteAttributeElementCollection EmptyIAttributeElementCollection = new LiteAttributeElementCollection();
		public static readonly LiteTypeParameterConstraintCollection EmptyITypeParameterConstraintCollection = new LiteTypeParameterConstraintCollection();
		public static readonly LiteSourceFileCollection EmptyISourceFileCollection = new LiteSourceFileCollection();
		public static readonly LiteProjectElementCollection EmptyIProjectElementCollection = new LiteProjectElementCollection();
		public static readonly LiteQueryIdentCollection EmptyIQueryIndentCollection = new LiteQueryIdentCollection();
		public static readonly LiteJoinExpressionCollection EmptyIJoinExpressionCollection = new LiteJoinExpressionCollection();
		public static readonly LiteXmlAttributeDeclarationCollection EmptyIXmlAttributeDeclarationCollection = new LiteXmlAttributeDeclarationCollection();
		public static readonly LiteHtmlAttributeCollection EmptyIHtmlAttributeCollection = new LiteHtmlAttributeCollection();
		public static readonly LiteXmlContentParticleCollection EmptyIXmlContentParticleCollection = new LiteXmlContentParticleCollection();		
		public static readonly TextRangeCollection EmptyTextRangeCollection = new TextRangeCollection();
		public static readonly int[] EmptyIntArray = new int[0];
	}
}
