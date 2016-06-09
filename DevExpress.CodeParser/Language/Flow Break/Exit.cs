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
	public enum ExitKind
	{
		Do,
		For,
		While,
		Select,
		Sub,
		Function,
		Property,
		Try
	}
	public class Exit : Break
	{
		ExitKind _ExitKind;
		#region Exit
		public Exit()
		{
		}
		public Exit(String exitKind, SourceRange range)
		{
			SetRange(range);
			_ExitKind = ResolveExitKind(exitKind);
		}		
		#endregion  
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Exit))
				return;
			Exit lSource = (Exit)source;
			_ExitKind = lSource._ExitKind; 			
		}
		#endregion
		public virtual ExitKind  ResolveExitKind(string exitKind)
		{
			return ExitKind.Sub;
		}		
		#region ToString
		public override string ToString()
		{
			return "Exit " + _ExitKind;
		}
		#endregion
		#region MatchesExitKind
		public virtual bool MatchesExitKind(LanguageElement target)
		{
			return false;			
		}
		#endregion
		#region FindTarget
		public override LanguageElement FindTarget()
		{
			LanguageElement lTargetElement = null;
			LanguageElement lParent = ParentLoop;
			LanguageElement lOuterLoop = null;
			LanguageElement lOuterTryBlock = null;
			if (lParent != null)
			{
				lOuterTryBlock = lParent.ParentTryBlock;
				lOuterLoop = lParent.ParentLoop;
			}
			bool lFound = false;
			while (lTargetElement == null)
			{
				if (lParent == null)
					break;
				if (!lFound)
				{
					if (MatchesExitKind(lParent))
					{
						lFound = true;
						lOuterLoop = lParent.ParentLoop;
						lOuterTryBlock = lParent.ParentTryBlock;
						continue;
					}
				}
				else
				{
					if (lParent is Method)
						lTargetElement = lParent;
					else if (lParent is Property)
						lTargetElement = lParent;
					else if (lParent is PropertyAccessor)
						lTargetElement = lParent;
					else if (lParent == lOuterLoop && !(lParent is Try))
						lTargetElement = lOuterLoop;
					else if (lParent == lOuterTryBlock)
						lTargetElement = NextStandaloneCodeSibling(lOuterTryBlock);
					else
						lTargetElement = NextStandaloneCodeSibling(lParent);
				}
				lParent = lParent.Parent;
			}
			if (lTargetElement == null)
				lTargetElement = GetParentMethodOrPropertyAccessor();
			return lTargetElement;
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Exit lClone = new Exit();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region ExitKind
		public ExitKind ExitKind
		{
			get
			{
				return _ExitKind;
			}
			set
			{
				_ExitKind = value;
			}
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.Exit;
			}			
		}
	}
}
