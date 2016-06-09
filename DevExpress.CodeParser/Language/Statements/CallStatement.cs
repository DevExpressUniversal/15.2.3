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

using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public class CallStatement : Statement, ICallStatement
  {
	Expression _CalledExpression;
	public CallStatement()
	{
	}
	protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
	{
	  if (oldElement == _CalledExpression)
		_CalledExpression = newElement as Expression;
	  else
		base.ReplaceOwnedReference(oldElement, newElement);
	}
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  CallStatement callStatement = source as CallStatement;
	  if (callStatement == null)
		return;
	  if (callStatement._CalledExpression == null)
		return;
	  _CalledExpression = ParserUtils.GetCloneFromNodes(this, callStatement, callStatement._CalledExpression) as Expression;
	  if (_CalledExpression == null)
		_CalledExpression = callStatement._CalledExpression.Clone(options) as Expression;
	}
	public override void CleanUpOwnedReferences()
	{
	  _CalledExpression = null;
	  base.CleanUpOwnedReferences();
	}
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  CallStatement result = new CallStatement();
	  result.CloneDataFrom(this, options);
	  return result;
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override void OwnedReferencesTransfered()
	{
	  _CalledExpression = null;
	  base.OwnedReferencesTransfered();
	}
	public override string ToString()
	{
	  string calledStatementString = string.Empty;
	  if (_CalledExpression != null)
		calledStatementString = _CalledExpression.ToString();
	  return "Call " + calledStatementString;
	}
	public Expression CalledExpression
	{
	  get
	  {
		return _CalledExpression;
	  }
	  set
	  {
		ReplaceDetailNode(_CalledExpression, value);
		_CalledExpression = value;
	  }
	}
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.CallStatement;
	  }
	}
	IExpression ICallStatement.CalledExpression
	{
	  get { return _CalledExpression; }
	}
  }
}
