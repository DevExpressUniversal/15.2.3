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

using System.Collections.Generic;
using DevExpress.CodeParser;
namespace DevExpress.XtraReports.Design {
	abstract class BracketSearcher {
		#region inner classes
		class ForwardBracketSearcher : BracketSearcher {
			protected override sbyte Step {
				get { return 1; }
			}
			protected override string EndValue {
				get { return endBrackets[startBrackets.IndexOf(token.Value)]; }
			}
			public ForwardBracketSearcher(Token token)
				: base(token) {
			}
			protected override bool NotEndOfSearch(int index, TokenCollection tokens) {
				return index < tokens.Count;
			}
		}
		class BackwardBracketSearcher : BracketSearcher {
			protected override sbyte Step {
				get { return -1; ; }
			}
			protected override string EndValue {
				get { return startBrackets[endBrackets.IndexOf(token.Value)]; }
			}
			public BackwardBracketSearcher(Token token)
				: base(token) {
			}
			protected override bool NotEndOfSearch(int index, TokenCollection tokens) {
				return index >= 0;
			}
		}
		#endregion
		#region static
		static List<string> startBrackets = new List<string>(new string[] { "{", "[", "(" });
		static List<string> endBrackets = new List<string>(new string[] { "}", "]", ")" });
		public static bool TokenIsBracket(string token) {
			return TokenIsEndBracket(token) || TokenIsStartBracket(token);
		}
		static bool TokenIsStartBracket(string token) {
			return startBrackets.Contains(token);
		}
		static bool TokenIsEndBracket(string token) {
			return endBrackets.Contains(token);
		}
		public static BracketSearcher CreateInstance(Token token) {
			if(TokenIsStartBracket(token.Value))
				return new ForwardBracketSearcher(token);
			return new BackwardBracketSearcher(token);
		}
		#endregion
		Token token;
		public BracketSearcher(Token token) {
			this.token = token;
		}
		protected abstract sbyte Step { get; }
		protected abstract string EndValue { get; }
		protected abstract bool NotEndOfSearch(int index, TokenCollection tokens);
		public Token FindClosingBracket(TokenCollection tokens) {
			int countOfPairBrackets = 1;
			int start = tokens.IndexOf(token) + Step;
			string endValue = EndValue;
			for(int i = start; NotEndOfSearch(i, tokens); i += Step) {
				if(!TokenIsBracket(tokens[i].Value))
					continue;
				if(tokens[i].Value == endValue)
					countOfPairBrackets--;
				if(tokens[i].Value == token.Value)
					countOfPairBrackets++;
				if(countOfPairBrackets == 0)
					return tokens[i];
			}
			return null;
		}
	}
}
