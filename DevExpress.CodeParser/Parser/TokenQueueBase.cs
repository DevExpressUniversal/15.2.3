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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  using Base;
	public class TokenQueueBase : ICollection, IEnumerable, ICloneable, ITokenReader
	{
		#region private fields...
		LinkedList _Queue;
		int _CurrentPos;
		Token _LastDequeuedToken;
		#endregion
		public TokenQueueBase()
		{
			_Queue = new LinkedList();
			_CurrentPos = 0;
		}
		object ICloneable.Clone()
		{
			return Clone();
		}
		public void Append(TokenQueueBase queue)
		{
			if (queue == null || queue._Queue == null)
				return;
			_Queue.AddRange(queue._Queue);
		}
		public void CopyTo(Array array, int index)
		{
			_Queue.CopyTo(array, index);
		}
		public bool Contains(Token token)
		{
			return _Queue.Contains(token);
		}
		public IEnumerator GetEnumerator()
		{
			return _Queue.GetEnumerator();
		}
		public TokenQueueBase Clone()
		{
			return CloneQueue();
		}
		public Token Peek()
		{			
			return Peek(CurrentPos);
		}
		public Token Peek(int index)
		{
			if (Count == 0)
				throw new ParserException("Token queue is empty.");
			return (Token)_Queue[index];
		}
		public void Clear()
		{
			_Queue.Clear();
			_CurrentPos = 0;
		}
		public Token Dequeue()
		{
			if (Count == 0)
				throw new ParserException("Token queue is empty.");
			Token lToken = Peek(_CurrentPos);
			_Queue.RemoveAt(_CurrentPos);
			_LastDequeuedToken = lToken;
			return lToken;
		}
		#region Enqueue
		public void Enqueue(Token token)
		{
			_Queue.Add(token);
		}
		#endregion
		#region DequeueToken
		public Token DequeueToken()
		{
			return (Token)Dequeue();
		}
		#endregion
		#region EnqueueToken
		public void EnqueueToken(Token token)
		{
			Enqueue(token);
		}
		#endregion
		#region TokenPosition
		public int TokenPosition(int token)
		{
			int i = 0;
			IEnumerator lEnumerator = GetEnumerator();
			while (lEnumerator.MoveNext())
			{
				Token lToken = (Token)lEnumerator.Current;
				if (lToken.Type == token)
					return i;
				i ++;
			}
			return -1;
		}
		#endregion
		#region LookUpToken(int pos)
		public Token LookUpToken(int pos)
		{
			int lRealPos = _CurrentPos + pos;
			if (lRealPos < 0 || lRealPos >= _Queue.Count)
				return null;
			return Peek(lRealPos);
		}
		#endregion
		#region MoveForward()
		public Token MoveForward()
		{
			Dequeue();
			return CurrentToken;
		}
		#endregion
		#region ContainsToken
		public bool ContainsToken(int token)
		{
			return TokenPosition(token) != -1;
		}
		#endregion      
		#region GetUnparentedTokenPosition
		public int GetUnparentedTokenPosition(int tokenType, int openType, int closeType)
		{
			int parenLevel = 0;
			int count = Count;
			for (int i = 0; i < count; i++)
			{
				Token token = _Queue[i] as Token;
				if (token == null)
					continue;
				if (token.Type == openType)
						parenLevel ++;
				if (token.Type == closeType)
					parenLevel --;
				else if (token.Type == tokenType && parenLevel == 0)
					return i;
			}
			return -1;
		}
		#endregion
		#region ContainsUnparentedToken
		public bool ContainsUnparentedToken(int token)
		{
			return GetUnparentedTokenPosition(token, TokenType.ParenOpen, TokenType.ParenClose) >= 0;
		}
		#endregion
		#region CloneQueue()
		public virtual TokenQueueBase CloneQueue()
		{
			TokenQueueBase lQueue = new TokenQueueBase();
			IEnumerator lEnumerator = GetEnumerator();
			while (lEnumerator.MoveNext())
				lQueue.Enqueue((Token)lEnumerator.Current);
			return lQueue;
		}
		#endregion
		public bool CurrentTokenMatches(int tokenType)
		{
			return CurrentToken != null && CurrentToken.Match(tokenType);
		}
		#region ToString
		public override string ToString()
		{
			StringBuilder lResult = new StringBuilder();
			IEnumerator lEnumerator = GetEnumerator();
			string lSpace = String.Empty;
			while (lEnumerator.MoveNext())
			{
				Token lToken = (Token)lEnumerator.Current;
				lResult.AppendFormat("{0}{1}", lSpace, lToken.EscapedValue);
				lSpace = " ";
			}
			return lResult.ToString();
		}
		#endregion
		#region SkipUpTo
		public void SkipUpTo(int tokenType)
		{
			SkipUpTo(tokenType, false);
		}
		#endregion
		#region SkipUpTo
		public void SkipUpTo(int tokenType, bool include)
		{
			if (Count == 0)
				return;
			Token lToken = Peek();
			while (Count > 0 && lToken.Type != tokenType)
			{
				Dequeue();
				if (Count > 0)
					lToken = Peek();
			}
			if (Count > 0 && include)
				Dequeue();
		}
		#endregion
		#region QueueUpTo
		public TokenQueueBase QueueUpTo(int tokenType)
		{
			return QueueUpTo(tokenType, true);
		}
		#endregion
		#region QueueUpTo
		public TokenQueueBase QueueUpTo(int tokenType, bool include)
		{
			TokenQueueBase lResult = new TokenQueueBase();
			Token lToken = Peek();
			while (Count > 0 && (lToken.Type != tokenType))
			{
				lResult.Enqueue(lToken);
				Dequeue();
				if (Count > 0)
					lToken = Peek();
			}
			if (Count > 0 && include)
			{
				lResult.Enqueue(lToken);
				Dequeue();
			}
			return lResult;
		}
		#endregion
		public void SetCurrentPos(int pos)
		{
			if (pos < 0)
				pos = 0;
			if (pos >= _Queue.Count)
				pos = _Queue.Count - 1;
			_CurrentPos = pos;
		}
		public int CurrentPos
		{
			get
			{
				return _CurrentPos;
			}
			set
			{
				SetCurrentPos(value);
			}
		}
		public int Count
		{
			get
			{
				return _Queue.Count - _CurrentPos;
			}
		}
		public bool IsSynchronized
		{
			get
			{
				return _Queue.IsSynchronized;
			}
		}
		public object SyncRoot
		{
			get
			{
				return _Queue.SyncRoot;
			}
		}
		#region CurrentToken
		public Token CurrentToken
		{
			get
			{
				if (Count > 0)
					return Peek();
				return null;
			}
		}
		#endregion
		#region NextToken
		public Token NextToken
		{
			get
			{
				if (Count > 1)
					return LookUpToken(1);
				return null;
			}
		}
		#endregion
		#region LastToken
		public Token LastToken
		{
			get
			{
				if (Count == 0)
					return null;
				return Peek(Count - 1);
			}
		}
		#endregion
		#region LastDequeuedToken
		public Token LastDequeuedToken
		{
			get
			{
				return _LastDequeuedToken;
			}
		}
		#endregion
		#region Eof
		public bool Eof
		{
			get
			{
				return CurrentToken == null;
			}
		}
		#endregion
	}
}
