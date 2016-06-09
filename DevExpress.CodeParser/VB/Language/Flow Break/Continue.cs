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
	public enum ContinueKind : byte
	{
		Unknown,
		Do,
		For,
		While
	};
	public class VBContinue : Continue
	{
		ContinueKind _ContinueKind;
		public VBContinue()
		{
		}
		public VBContinue(ContinueKind continueKind)
		{
			_ContinueKind = continueKind;
		}
		public VBContinue(Token continueKind, SourceRange range)
		{
			if(continueKind != null)_ContinueKind = ToContinueKind(continueKind.Type);
			SetRange(range);
		}
		ContinueKind ToContinueKind(int tokenType)
		{
			switch (tokenType)
			{
				case TokenType.Do:
					 return ContinueKind.Do;
				case TokenType.For:
					return ContinueKind.For;
				case TokenType.While:
					return ContinueKind.While;
			}
			return ContinueKind.Unknown;
		}
		bool MatchesContinueKind(LanguageElement element)
		{
			if (element == null)
				return false;
			switch (ContinueKind)
			{
				case ContinueKind.Do:
					return element is Do;
				case ContinueKind.For:
					return element is VBFor || element is VBForEach;
				case ContinueKind.While:
					return element is While;
			}
			return false;
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is VBContinue))
				return;
			VBContinue lSource = (VBContinue)source;
			_ContinueKind = lSource._ContinueKind;
		}
		#endregion
		public override string ToString()
		{
			return "Continue";
		}
		public override int GetImageIndex()
		{
			return ImageIndex.Continue;
		}
		public override LanguageElement FindTarget()
		{
			LanguageElement target = null;
			LanguageElement parent = ParentLoop;
			if (ContinueKind == ContinueKind.Unknown)
				return parent;
			while (target == null)
			{
				if (parent == null)
					break;
				if (MatchesContinueKind(parent))
				{
					target = parent;
					continue;
				}
				if (ElementFilters.IsMember(parent))
					target = parent;
				parent = parent.Parent;
			}
			if (target == null)
				target = ParentLoop;
			return target;
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			VBContinue lClone = new VBContinue();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Continue;
			}
		}
		public ContinueKind ContinueKind
		{
			get
			{
				return _ContinueKind;
			}
		}
	}
}
