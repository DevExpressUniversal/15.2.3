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
	public class ExitStatement : Exit
	{
		#region ExitStatement
		public ExitStatement()
		{
		}
		public ExitStatement(string exitKind, SourceRange range)
			: base(exitKind, range)
		{
		}
		#endregion
		public override ExitKind ResolveExitKind(string exitKind)
		{
			switch(exitKind.ToLower())
			{
				case "do": return ExitKind.Do;
				case "for": return ExitKind.For;
				case "while": return ExitKind.While;
				case "select": return ExitKind.Select;
				case "sub": return ExitKind.Sub;
				case "function": return ExitKind.Function;
				case "property": return ExitKind.Property;
				case "try": return ExitKind.Try;
			}
			return ExitKind.Try;
		}
		#region MatchesExitKind
		public override bool MatchesExitKind(LanguageElement target)
		{
			bool lResult = false;
			switch (ExitKind)
			{
				case ExitKind.Do:
	  		lResult = target is Do;
					break;
				case ExitKind.For:
					lResult = (target is VBFor) || (target is VBForEach);
					break;
				case ExitKind.While:
					lResult = target is While;
					break;
				case ExitKind.Select:
					lResult = target is Switch;
					break;
				case ExitKind.Sub:
				case ExitKind.Function:
					lResult = target is Method;
					break;
				case ExitKind.Property:
					lResult = target is PropertyAccessor;
					break;
				case ExitKind.Try:
					lResult = target is Try;
					break;
				default:
					lResult = false;
					break;
			}
			return lResult;
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ExitStatement lClone = new ExitStatement();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
	}
}
