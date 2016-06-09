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
using System.Text;
using System.Collections;
#if SL
using DevExpress.Utils;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public abstract class TokenizerBase : ITokenReader
	{
		ISourceReader _SourceReader;
		ScannerExtension _ScannerExtension;
		Token _CurrentToken = null;
		Token _NextToken = null;
		TextStringCollection _TextStrings;
		CommentCollection _Comments;
		LanguageTokensBase _Tokens;
		Queue _TokenQueue;
		string[] emptyString = new string[] { String.Empty };
		#region TokenizerBase
		public TokenizerBase(ISourceReader reader)
			: this (reader, null)
		{
		}
		public TokenizerBase(ISourceReader reader, ScannerExtension extension)
		{
			SourceReader = reader;
			_Comments = new CommentCollection();
			_ScannerExtension = extension;
			Prepare();
		}
		#endregion
		protected abstract void CreateTokens();
		protected abstract Token GotoNext();
		protected virtual Token GotoNextWithScannerExtension()
		{
			if (_TokenQueue != null && _TokenQueue.Count > 0)
				return _TokenQueue.Dequeue() as Token;
			Token lexerToken = GetNextTokenFromLexer();
			ScannerExtension extension = this.ScannerExtension;
			if (extension != null)
				return GetNextTokenWithScannerExtension(extension, lexerToken);
			return lexerToken;
		}
		protected virtual Token GetNextTokenFromLexer()
		{
			return null;
		}
		protected virtual Token GetNextTokenWithScannerExtension(ScannerExtension extension, Token lexerToken)
		{
			if (extension == null)
				return null;
			Token token = null;			
			if (lexerToken == null)
			{				
				TokenCollection tokens = extension.GetTailTokens();
				if (tokens != null)
				{
					foreach (Token tail in tokens)
						_TokenQueue.Enqueue(tail);
					if (_TokenQueue.Count > 0)
						return _TokenQueue.Dequeue() as Token;
				}
				return null;
			}
			token = extension.Scan(lexerToken.Range.Start);
			while (token != null)
			{
				_TokenQueue.Enqueue(token);
				token = extension.Scan(lexerToken.Range.Start);
			}
			_TokenQueue.Enqueue(lexerToken);
			return _TokenQueue.Dequeue() as Token;
		}		
		protected virtual void Prepare()
		{
			_TokenQueue = new Queue();
			CreateTokens();
			MoveForward();
			MoveForward();
		}
		protected string GetEmptyString(int length) 
		{
			if (emptyString.Length > length)
				return emptyString[length];
			string[] temp = new string[length + 1];
			emptyString.CopyTo(temp, 0);
			int i = emptyString.Length;
			emptyString = temp;
			while(i <= length)
			{
				emptyString[i] = new String(' ', i);
				i++;
			}
			return emptyString[length];
		}	  
		public abstract Token GetCodeBlock(Token token);
		public virtual void Reset(ISourceReader sourceReader)
		{
			SourceReader = sourceReader;
			_Comments = new CommentCollection();
			_TokenQueue = new Queue();
		}
		public virtual Token MoveForward()
		{
			CurrentToken = NextToken;
			NextToken = GotoNext();
			return CurrentToken;
		}	  
		protected LanguageTokensBase Tokens
		{
			get
			{
				return _Tokens;
			}
			set
			{
				_Tokens = value;
			}
		}
		protected Queue TokenQueue
		{
			get
			{
				return _TokenQueue;
			}
		}
		public ISourceReader SourceReader
		{
			get
			{
				return _SourceReader;
			}
			set
			{
				_SourceReader = value;
			}
		}
		public ScannerExtension ScannerExtension
		{
			get
			{
				return _ScannerExtension;
			}
			set
			{
				_ScannerExtension = value;
			}
		}
		public Token CurrentToken
		{
			get
			{
				return _CurrentToken;
			}
			set
			{
				_CurrentToken = value;
			}
		}
		public Token NextToken
		{
			get
			{
				return _NextToken;
			}
			set
			{
				_NextToken = value;
			}
		}
		public CommentCollection Comments
		{
			get
			{
				return _Comments;
			}
			set
			{
				_Comments = value;
			}
		}
		public TextStringCollection TextStrings
		{
			get
			{
				return _TextStrings;
			}
			set
			{
				_TextStrings = value;
			}
		}
		public virtual bool Eof
		{
			get
			{
				return CurrentToken == null;
			}
		}
	}
}
