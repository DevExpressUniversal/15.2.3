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
	public class Try: ParentingStatement, ITryStatement
	{
		private const int INT_MaintainanceComplexity = 3;		
		#region Try
		public Try()
		{
		}
		#endregion
		#region GetCyclomaticComplexity
		public override int GetCyclomaticComplexity()
		{
			return GetChildCyclomaticComplexity();
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.TryBlock;	
		}
		#endregion
		#region GetFinally
		public Finally GetFinally()
		{
			LanguageElement lNextCodeSibling = NextCodeSibling;
			while (lNextCodeSibling != null && lNextCodeSibling.CompletesPrevious && lNextCodeSibling.ElementType != LanguageElementType.Finally)
				lNextCodeSibling = lNextCodeSibling.NextCodeSibling;
			if (lNextCodeSibling is Finally)
				return (Finally)lNextCodeSibling;
			else
				return null;
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
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Try lClone = new Try();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Try;
			}
		}
	}
}
