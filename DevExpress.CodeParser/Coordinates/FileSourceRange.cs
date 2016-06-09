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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class FileSourceRange
	{
		SourceFile _File;
		SourceRange _Range;
		object _Data;
		public FileSourceRange(SourceFile file, SourceRange range)
		{
			_File = file;
			_Range = range;
			_Data = null;
		}
	public FileSourceRange(IElement element, SourceRange range)
	  :this(GetSourceFile(element), range)
	{
	}
	static SourceFile GetSourceFile(IElement element)
	{
	  if (element == null)
		return null;
	  SourceFile file = element as SourceFile;
	  if (file != null)
		return file;
	  return element.FirstFile as SourceFile;
	}
		public SourceFile File
		{
			get
			{
				return _File;
			}
		}
		public SourceRange Range
		{
			get
			{
				return _Range;
			}
			set
			{
				_Range = value;
			}
		}
		public Object Data
		{
			get
			{
				return _Data;
			}
			set
			{
				_Data = value;
			}
		}
		public override bool Equals(object obj)
		{
			FileSourceRange fileSourceRange = obj as FileSourceRange;
			if (fileSourceRange == null)
				return false;
		return fileSourceRange.File == _File && fileSourceRange.Range == _Range;
		}
		public override int GetHashCode()
		{
	  string s = _File.Name + " " + _Range.ToString();
	  return s.GetHashCode();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool BindToCode()
		{
			if (_File == null)
				return false;
			_Range.BindToCode(_File.Document);
			return _Range.IsBoundToCode;
		}
	}
}
