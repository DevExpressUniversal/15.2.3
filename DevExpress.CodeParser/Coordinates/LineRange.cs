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
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public struct LineRange
	{
		#region private fields...
	private int _NumLines;
		#endregion
		#region LineRange
		public LineRange(int numLines)
		{
		_NumLines = numLines;
		}
		#endregion
		#region LineRange -> int
		public static implicit operator int(LineRange lineRange)
		{
			return lineRange._NumLines;
		}
		#endregion
		#region int -> LineRange
		public static implicit operator LineRange(int numLines)
		{
			return new LineRange(numLines);
		}
		#endregion
		#region Equality override
		public static bool operator==(LineRange left, int right)
		{
			if ((object)left == null)
				return (object)right == null;
			else
				return left.Equals(right);
		}
		public static bool operator!=(LineRange left, int right)
		{
			return !(left == right);
		}
		public static bool operator==(int left, LineRange right)
		{
			if ((object)left == null)
				return (object)right == null;
			else
				return right.Equals(left);
		}
		public static bool operator!=(int left, LineRange right)
		{
			return !(left == right);
		}
		public override int GetHashCode()
		{
			return _NumLines.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			if (obj is LineRange)
				return Equals((LineRange)obj);
			else if (obj is int)
				return Equals((int)obj);
			else
				return base.Equals(obj);
		}
		public bool Equals(LineRange obj)
		{
			return (_NumLines == obj._NumLines);
		}
		public bool Equals(int obj)
		{
			return (_NumLines == obj);
		}
		#endregion
		#region NumLines
		public int NumLines
		{
			get
			{
				return _NumLines;
			}
		}
		#endregion
	}
}
