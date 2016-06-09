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
using System.ComponentModel;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class Statement : DelimiterCapableBlock, IStatement
	{
		Expression _SourceExpression = null;
		#region Statement
		public Statement(): base()
		{
		}
		#endregion
		#region Statement
		public Statement(string code): base()
		{
			InternalName = code;
		}
		#endregion
	public IEnumerable<IElement> GetBlockChildren()
	{
	  foreach (object nodeObj in Nodes)
	  {
		IElement node = nodeObj as IElement;
		if (node != null)
		  yield return node;
	  }
	}
		#region GetImageIndex 
		public override int GetImageIndex() 
		{
			return ImageIndex.Statement;
		}
		#endregion
		#region FromExpression
		public static Statement FromExpression(Expression expression)
		{
			Statement lStatement = null;
			if (expression != null)
			{
				lStatement = new Statement(expression.ToString());
				lStatement.AddDetailNode(expression);
				expression.IsStatement = true;
				lStatement.MacroCall = expression.MacroCall;
				lStatement._SourceExpression = expression;
			}
			return lStatement;
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Statement lClone = new Statement();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
	public override MemberVisibility GetDefaultVisibility()
	{
	  return MemberVisibility.Local;
	}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Statement))
				return;			
			Statement lSource = (Statement)source;
	  if (lSource._SourceExpression != null)
	  {
		_SourceExpression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._SourceExpression) as Expression;
		if (_SourceExpression == null)
		  _SourceExpression = lSource._SourceExpression.Clone(options) as Expression;
	  }
		}
		#endregion
	protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
	{
	  if(_SourceExpression == oldElement)
		_SourceExpression = newElement as Expression;
	  base.ReplaceOwnedReference(oldElement, newElement);
	}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Statement;
			}
		}
		public override bool CanContainCode 
		{
			get 
			{
				return true;
			}
		}
		public Expression SourceExpression
		{
			get
			{
				return _SourceExpression;
			}
	  set
	  {
		_SourceExpression = value;
	  }
		}
	#region IStatement Members
	public new bool HasDelimitedBlock
	{
	  get
	  {
		return base.HasDelimitedBlock;
	  }
	}
	#endregion
  }
}
