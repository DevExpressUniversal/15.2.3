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
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	public interface IAutomatonSourceReader {
		char ScanChar();
	}
	public abstract class Automaton {
		public const char EOF = (char)0xFFFF;
		private IAutomatonSourceReader sourceReader;
		private char[] alphabet;
		private int[,] transitionMatrix;
		private int state;
		public Automaton(IAutomatonSourceReader sourceReader) {
			this.sourceReader = sourceReader;
			DefineAlphabet(out this.alphabet);
			DefineTransitionMatrix(out this.transitionMatrix);
		}
		public virtual bool IsActive {
			get { return this.state != FinalState; }
		}
#if DEBUG
		public IAutomatonSourceReader SourceReader {
			set { this.sourceReader = value; }
		}
#endif
		protected abstract int InitialState { get; }
		protected abstract int FinalState { get; }
		protected int State {
			get { return state; }
			set { state = value; }
		}
		public void Reset() {
			this.state = InitialState;
		}
		public char PerformAction() {
			char ch = this.sourceReader.ScanChar();
			if(ch == EOF) {
				this.state = FinalState;
				return EOF;
			}
			int chIndex = 0;
			for(int i = 0; i < this.alphabet.Length; i++) {
				if(this.alphabet[i] == ch) {
					chIndex = i + 1;
					break;
				}
			}
			this.state = this.transitionMatrix[chIndex, this.state];
			return ch;
		}
		protected abstract void DefineAlphabet(out char[] alphabet);
		protected abstract void DefineTransitionMatrix(out int[,] transitionMatrix);
	}
	public sealed class CssWalkAutomaton : Automaton {
		public CssWalkAutomaton(IAutomatonSourceReader sourceReader)
			: base(sourceReader) {
		}
		protected override int InitialState {
			get { return 0; }
		}
		protected override int FinalState {
			get { return 4; }
		}
		protected override void DefineAlphabet(out char[] alphabet) {
			alphabet = new char[] {
				'<', '/', '*'
			};
		}
		protected override void DefineTransitionMatrix(out int[,] transitionMatrix) {
			transitionMatrix = new int[,] {
				 { 0, 0, 2, 2, 4 },
				 { 4, 4, 2, 2, 4 },
				 { 1, 1, 2, 1, 4 },
				 { 0, 2, 3, 3, 4 }
			};
		}
	}
	public sealed class JsWalkAutomaton : Automaton {
		public JsWalkAutomaton(IAutomatonSourceReader sourceReader)
			: base(sourceReader) {
		}
		protected override int InitialState { get { return 0; } }
		protected override int FinalState { get { return 9; } }
		protected override void DefineAlphabet(out char[] alphabet) {
			alphabet = new char[] {
				'<', '\'', '\"', '/', '\n', '*', '\\'
			};
		}
		protected override void DefineTransitionMatrix(out int[,] transitionMatrix) {
			transitionMatrix = new int[,] {
				 { 0, 0, 2, 3, 3, 5, 6, 5, 6, 9 },
				 { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
				 { 6, 1, 2, 3, 3, 5, 0, 5, 6, 9 },
				 { 5, 1, 2, 3, 3, 0, 6, 5, 6, 9 },
				 { 1, 2, 2, 3, 0, 5, 6, 5, 6, 9 },
				 { 0, 0, 0, 3, 3, 5, 6, 5, 6, 9 },
				 { 0, 3, 0, 4, 4, 5, 6, 5, 6, 9 },
				 { 0, 1, 2, 3, 3, 7, 8, 5, 6, 9 }
			};
		}
	}
}
