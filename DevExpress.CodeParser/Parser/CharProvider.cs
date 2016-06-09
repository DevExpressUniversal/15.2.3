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
using System.IO;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  internal class CharReader : TextReader
  {
	private string _Str;
	private int _Position;
	public int Length { get { return _Str != null ? _Str.Length : 0; } }
	public int Position 
	{ 
	  get { return _Position; }
	  set { _Position = value; }
	}
	public CharReader(string s)
	{
	  _Str = s;
	  _Position = 0;
	}
	protected override void Dispose(bool disposing)
	{
	  _Str = null;
	  _Position = 0;
	  base.Dispose(disposing);
	}
	public override void Close()
	{
	  Dispose(true);
	}
	public override int Peek()
	{
	  if (_Str == null)
		return -1;
	  return (_Position >= 0 && _Position < Length) ? _Str[_Position] : -1;
	}
	public override int Read()
	{
	  if (_Str == null)
		return -1;
	  return (_Position >= 0 && _Position < Length) ? _Str[_Position++] : -1;
	}
	public override int Read(char[] buffer, int index, int count)
	{
	  if (buffer == null)
		throw new ArgumentNullException("buffer");
	  if (index < 0)
		throw new ArgumentOutOfRangeException("index");
	  if (count < 0)
		throw new ArgumentOutOfRangeException("count");
	  if (buffer.Length - index < count)
		throw new ArgumentException("Invalid offset & length");
	  if (_Str == null)
		return -1;
	  int n = Length - _Position;
	  if (n > 0)
	  {
		if (n > count)
		  n = count;
		_Str.CopyTo(_Position, buffer, index, n);
		_Position += n;
	  }
	  return n;
	}
	public override string ReadLine() 
	{
	  throw new NotImplementedException("ReadLine");
	}
	public override string ReadToEnd()
	{
	  throw new NotImplementedException("ReadToEnd");
	}
  }
  public class CharProvider 
	{
		const char EOF = (char)65535;
		const char EOL = '\n';
		class CharListItem
		{
			public char Value;
			public CharListItem Next = null;
		}
		TextReader  _Reader;		
		CharListItem _CharsQueueHead;
		CharListItem _PeekCharPosition;
	string _Str;
		public CharProvider(TextReader reader)
		{
	  _Str = reader.ReadToEnd();
	  _Reader = new CharReader(_Str);
		}
		protected char ReadChar()
		{
			char ch = (char)_Reader.Read();
	  if (ch == 1042)
	  {
		if (_Reader.Peek() == 160)
		{
		  _Reader.Read();
		  ch = ' ';
		}
	  }
			if (ch == '\r' && _Reader.Peek() != '\n') ch = EOL;
			return ch;
		}
		public char Get()
		{
			char result;
			if (_CharsQueueHead != null)
			{
				result = _CharsQueueHead.Value;
				if(_PeekCharPosition == _CharsQueueHead)
					_PeekCharPosition = _CharsQueueHead.Next;
				_CharsQueueHead = _CharsQueueHead.Next;
				return result;
			}
			else
			{
				result = ReadChar();
				return result;
			}
		}
	public void Seek(int position)
	{
	  ((CharReader)_Reader).Position = position;
	}
		public char Peek()
		{
			char result;
			if (_PeekCharPosition == null)
			{
				CharListItem newItem = new CharListItem();
				result = ReadChar();
				newItem.Value = result;
				_PeekCharPosition = newItem;
				_CharsQueueHead = newItem;
			}
			else 
				result = _PeekCharPosition.Value;
			if (_PeekCharPosition.Next == null)
			{
				CharListItem newItem = new CharListItem();
				newItem.Value = ReadChar();
				_PeekCharPosition.Next = newItem;
			}
			_PeekCharPosition = _PeekCharPosition.Next;
			return result;
		}
		public void ResetPeek()
		{
			_PeekCharPosition = _CharsQueueHead;
		}
		public void Close()
		{
			if (_Reader != null)
			{
				_Reader.Close();
				_Reader = null;
			}
		}
		public bool HasPeekedChars
		{
			get
			{
				return _CharsQueueHead != null && _CharsQueueHead.Next != null;
			}
		}	
	}
}
