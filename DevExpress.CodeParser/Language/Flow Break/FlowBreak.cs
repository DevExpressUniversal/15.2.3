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
	public class FlowBreak : Statement, IFlowBreakStatement
	{
		#region FlowBreak
		public FlowBreak()
		{
		}
		#endregion
		protected LanguageElement FindReturnTarget()
		{
			LanguageElement target = null;
			Try parentTryBlock = ParentTryBlock;
			if (parentTryBlock != null)
				target = parentTryBlock.GetFinallyTarget();
	  if (target == null)
	  {
		Catch parentCatchBlock = GetParent(LanguageElementType.Catch) as Catch;
		if (parentCatchBlock != null)
		  target = parentCatchBlock.GetFinallyTarget();
	  }
			if (target == null)
				target = GetParentAnonymousExpression();
			if (target == null)
				target = GetParentMethodOrPropertyAccessor();
			return target;
		}
		protected LanguageElement FindBreakTarget()
		{
			return FindBreakTarget(false);
		}
		protected LanguageElement FindBreakTarget(bool canBreakOutOfMember)
		{
			LanguageElement targetElement = GetParentLoopOrFinallyTarget();
			LanguageElement parentLoop = ParentLoop;
			if (parentLoop == null && canBreakOutOfMember)
				parentLoop = GetParentMethodOrAccessor();
			if (parentLoop != null && targetElement == parentLoop)
			{
				targetElement = NextStandaloneCodeSibling(parentLoop);
				if (targetElement == null)		
				{
					LanguageElement parent = parentLoop;
					LanguageElement outerLoop = parentLoop.ParentLoop;
					Try outerTryBlock = parentLoop.ParentTryBlock;
					if (parent != null)
						while (targetElement == null)
						{
							parent = parent.Parent;
							if (parent == null || parent is Method || parent is PropertyAccessor)
								break;
							if (parent == outerLoop)
								targetElement = outerLoop;
							else if (parent == outerTryBlock)
								targetElement = NextStandaloneCodeSibling(outerTryBlock);
							else
								targetElement = NextStandaloneCodeSibling(parent);
						}
					if (targetElement == null)
						targetElement = GetParentMethodOrPropertyAccessor();
				}
			}
			return targetElement;
		}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.Break;
		}
		#endregion
		#region FindTarget
		public virtual LanguageElement FindTarget()
		{
			return null;
		}
		#endregion
		#region GetFlowParent
		public virtual LanguageElement GetFlowParent()
		{
			return null;
		}
		#endregion		
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			FlowBreak lClone = new FlowBreak();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.FlowBreak;
			}
		}
	}
}
