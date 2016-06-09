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
	public class Catch: ConditionalParentingStatement, ICatchStatement
	{
		private const int INT_MaintainanceComplexity = 3;
	  public string CatchesException = String.Empty;
		LanguageElement _Exception;
		LanguageElement _Condition;
		#region Catch
		public Catch()
		{
		}
		public Catch(LanguageElement exception, LanguageElement condition)
		{
	  SetException(exception);
	  SetCondition(condition);
		}
		#endregion
	protected void SetException(LanguageElement expression)
	{
	  LanguageElement oldExpression = _Exception;
	  if (oldExpression != null)
		oldExpression.RemoveFromParent();
	  _Exception = expression;
	  if (_Exception != null)
		AddDetailNode(_Exception);
	}
	protected void SetCondition(LanguageElement expression)
	{
	  LanguageElement oldExpression = _Condition;
	  if (oldExpression != null)
		oldExpression.RemoveFromParent();
	  _Condition = expression;
	  if (_Condition != null)
		AddDetailNode(_Condition);
	}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Catch))
				return;
			Catch lSource = (Catch)source;
			CatchesException = lSource.CatchesException;
	  if (lSource._Condition != null)
	  {
		_Condition = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Condition) as LanguageElement;
		if (_Condition == null)
		  _Condition = lSource._Condition.Clone(options) as LanguageElement;
	  }
	  if (lSource._Exception != null)
	  {
		_Exception = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Exception) as LanguageElement;
		if (_Exception == null)
		  _Exception = lSource._Exception.Clone(options) as LanguageElement;
	  }
		}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.CatchBlock;	
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Catch lClone = new Catch();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
	#region GetFinally
	public Finally GetFinally()
	{
	  LanguageElement lNextCodeSibling = NextCodeSibling;
	  while (lNextCodeSibling != null && lNextCodeSibling.CompletesPrevious && lNextCodeSibling.ElementType != LanguageElementType.Finally)
		lNextCodeSibling = lNextCodeSibling.NextCodeSibling;
	  return lNextCodeSibling as Finally;
	}
	#endregion
	#region GetFinallyTarget
	public LanguageElement GetFinallyTarget()
	{
	  Finally lFinallyBlock = GetFinally();
	  if (lFinallyBlock == null)
		return null;
	  LanguageElement lTarget = lFinallyBlock.GetFirstCodeChild();
	  if (lTarget == null)
		lTarget = lFinallyBlock;
	  return lTarget;
	}
	#endregion
	#region ThisMaintenanceComplexity
	protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
	}
	#endregion
	#region ElementType
	public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Catch;
			}
	}
	#endregion
	#region CompletesPrevious
	public override bool CompletesPrevious
		{
			get 
			{
				return true;
			}
	}
	#endregion
	#region Exception
	public LanguageElement Exception
		{
			get
			{
				return _Exception;
			}
			set
			{
				SetException(value);
			}
	}
	#endregion
	#region Condition
	public LanguageElement Condition
		{
			get
			{
				return _Condition;
			}
			set
			{
				SetCondition(value);
			}
	}
	#endregion
	#region ICatchStatement Members
	IElement ICatchStatement.ExceptionVariable
		{
			get 
			{
				if (DetailNodeCount == 0)
					return null;
				return DetailNodes[0] as LanguageElement;
			}
		}
	IElement ICatchStatement.Condition
	{
	  get
	  {
		return _Condition;
	  }
	}
		#endregion
	}
}
