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
	public class Abort: FlowBreak, IAbortStatement
	{
		#region Abort
		public Abort()
	{
	}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.Abort;
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			return "abort";
		}
		#endregion
		public override LanguageElement GetFlowParent()
		{
	  LanguageElement parent = ParentTryBlock;
	  if (parent == null)
		parent = GetParent(LanguageElementType.Catch);
	  if (parent == null)
		parent = GetParentMethodOrPropertyAccessor();
	  return parent;
		}
		#region FindTarget
		public override LanguageElement FindTarget()
		{
	  LanguageElement flowParent = GetFlowParent();
	  if (flowParent == null)
		return null;
	  LanguageElement parentBlock = GetParentMethodOrPropertyAccessor();
	  LanguageElement flowParentNextCodeSibling = flowParent.NextCodeSibling;
	  if (flowParentNextCodeSibling == null)
		return GetParentMethodOrPropertyAccessor();
	  if (flowParentNextCodeSibling.ElementType == LanguageElementType.Catch)
		return flowParentNextCodeSibling;
	  return parentBlock;
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Abort lClone = new Abort();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Abort;
			}
		}
	}
}
