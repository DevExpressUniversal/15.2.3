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
	public class SourceLine
	{
		SourcePoint _Start;
		string _Text;
		#region SourceLine
		public SourceLine()
		{
			_Text = String.Empty;
			_Start = SourcePoint.Empty;
		}
		#endregion
		#region SourceLine
		public SourceLine(int line, int offset) : this (line, offset, String.Empty)
		{
		}
		#endregion
		#region SourceLine
		public SourceLine(int line, int offset, string text)
		{
			_Text = text;
			_Start = new SourcePoint(line, offset);
		}
		#endregion
		#region SourceLine
		public SourceLine(SourcePoint point, string text)
		{
			_Text = text;
			_Start = point;
		}
		#endregion
		#region Start
		public SourcePoint Start
		{
			get
			{
				return _Start;
			}
			set
			{
				if (_Start == value)
					return;
				_Start = value;
			}
		}
		#endregion
		#region Text
		public string Text
		{
			get
			{
				return _Text;
			}
			set
			{
				if (_Text == value)
					return;
				_Text = value;
			}
		}
		#endregion
	}
}
