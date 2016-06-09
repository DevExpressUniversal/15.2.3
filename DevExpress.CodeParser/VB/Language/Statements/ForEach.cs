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

#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
  public class VBForEach : ForEach
	{
		Expression _NextExpression;
		LanguageElement _Initializer;
		public VBForEach()
		{
		}
	protected override void SetInitializer(LanguageElement value)
	{
	  base.SetInitializer(value);
	  LanguageElement oldInitializer = _Initializer;
	  if (oldInitializer != null)
		oldInitializer.RemoveFromParent();
	  _Initializer = value;
	  if (_Initializer != null)
		AddDetailNode(_Initializer); 
	}
	protected override void SetNextExpression(Expression value)
	{
	  base.SetNextExpression(value);
	  Expression oldExpression = _NextExpression;
	  if (oldExpression != null)
		oldExpression.RemoveFromParent();
	  _NextExpression = value;
	  if (_NextExpression != null)
		AddDetailNode(_NextExpression); 
	}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
	  if (_NextExpression == oldElement)
		_NextExpression = newElement as Expression;
	  else if (_Initializer == oldElement)
		_Initializer = newElement;
	  else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is VBForEach))
				return;
			VBForEach lSource = (VBForEach)source;
			if (lSource._NextExpression != null)
			{
		_NextExpression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._NextExpression) as Expression;
				if (_NextExpression == null)
					_NextExpression = lSource._NextExpression.Clone(options) as Expression;
			}
	  if (lSource._Initializer != null)
	  {
		_Initializer = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Initializer) as Expression;
		if (_Initializer == null)
		  _Initializer = lSource._Initializer.Clone(options) as Expression;
	  }
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			return "For Each";
		}
		#endregion
	public override void CleanUpOwnedReferences()
	{
	  base.CleanUpOwnedReferences();
	  if (_Initializer != null)
	  {
		_Initializer.CleanUpOwnedReferences();
		_Initializer = null;
	  }
	  if (_NextExpression != null)
	  {
		_NextExpression.CleanUpOwnedReferences();
		_NextExpression = null;
	  }
	}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			VBForEach lClone = new VBForEach();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
	#region Initializer
	public LanguageElement Initializer
		{
			get
			{
				return _Initializer;
			}
			set
			{
		SetInitializer(value);
			}
		}
		#endregion
		#region NextExpression
		public override Expression NextExpression
		{
			get
			{
				return _NextExpression;
			}
	  set
	  {
		SetNextExpression(value);
	  }
		}
		#endregion
	}
}
