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
using System.ComponentModel;
using System.Text;
#if SL
using DXEncoding = DevExpress.Utils.DXEncoding;
#else
using DXEncoding = System.Text.Encoding;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class SourceStringReader : ISourceReader
	{
		string _String;
		int _StartLine = 1;
		int _StartColumn = 1;
		public SourceStringReader(string s)
		{
			if (s == null)
				throw new ArgumentNullException("s");
			_String = s;
		}
	public SourceStringReader(string filePath, Encoding encoding)
	{
	  using (FileStream fileStream = File.OpenRead(filePath))
	  {
		using (StreamReader streamReader = new StreamReader(fileStream, encoding))
		{
		   _String = streamReader.ReadToEnd();
		}
	  }
	}
		public SourceStringReader(string s, int line, int column)
			: this(s)
		{
			_StartLine = line;
			_StartColumn = column;
		}
	public void Dispose()
	{
	}
	public TextReader GetStream()
	{
	  return new StringReader(_String);
	}
	public void OffsetSubStream(int line, int column)
	{
	}
	public ISourceReader GetSubStream(int startPos, int length, int line, int column)
	{
	  string lSubString = _String.Substring(startPos, length);
	  return new SourceStringReader(lSubString, line, column);
	}
	public bool IsDisposing
	{
	  get { return false; }
	}
	public bool IsDisposed
	{
	  get { return false; }
	}
		#region StartLine
		public int StartLine
		{
			get
			{
				return _StartLine;
			}
		}
		#endregion
		#region StartColumn
		public int StartColumn
		{
			get
			{
				return _StartColumn;
			}
		}
		#endregion
		bool ISourceReader.IsDocumentReader 
		{
			get
			{
				return false;
			}
		}
  }
}
