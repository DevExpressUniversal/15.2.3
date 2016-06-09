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

using System.Collections;
using DevExpress.CodeParser;
using System.Collections.Generic;
namespace DevExpress.XtraReports.Design {
	class TokenRangeComparer : IComparer<Token>, IComparer {
		protected virtual bool EqualTokenRanges(Token tokenx, Token tokeny) {
			return tokeny.Range.Contains(tokenx.Range);
		}
		bool TokenLessThan(Token tokenx, Token tokeny) {
			if(tokenx.Line < tokeny.Line)
				return true;
			if(tokenx.Line == tokeny.Line && tokenx.Column < tokeny.Column)
				return true;
			return false;
		}
		#region IComparer Members
		public int Compare(object x, object y) {
			return Compare((Token)x, (Token)y);
		}
		public int Compare(Token tokenx, Token tokeny) {
			if(EqualTokenRanges(tokenx, tokeny))
				return 0;
			else if(TokenLessThan(tokenx, tokeny))
				return -1;
			return 1;
		}
		#endregion
	}
	class TokenErrorsRangeComparer : TokenRangeComparer {
		protected override bool EqualTokenRanges(Token tokenx, Token tokeny) {
			return tokeny.Range.Contains(tokenx.Range) || tokenx.Range.Contains(tokeny.Range);
		}
	}
	class TokenBracketComparer : TokenRangeComparer {
		protected override bool EqualTokenRanges(Token tokenx, Token tokeny) {
			return base.EqualTokenRanges(tokenx, tokeny) && BracketSearcher.TokenIsBracket(tokeny.Value);
		}
	}
}
