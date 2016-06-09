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
using System.Runtime.InteropServices;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	[Serializable]
  public enum SourcePointBinding : byte
	{
		None,
		EditPoint,
		Anchor
	}
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	public struct TextPoint
  {
		public static readonly TextPoint Empty;
		[FieldOffset(0)]
		int _Line;
		[FieldOffset(4)]
		int _Offset;
		public int Offset
	{
			get { return _Offset; }
		}
		public int Line
		{
			get { return _Line; }
		}
		public TextPoint(int line, int offset) {
			_Line = line;
			_Offset = offset;
		}
		public void Set(int line, int offset) {
			_Line = line;
			_Offset = offset;
		}
		public void Set(SourcePoint point) {
			Set(point.Line, point.Offset);
		}
		public void Set(Token token) {
			Set(token.Line, token.Column);
		}
		public void SetOffset(int offset) {
			_Offset = offset;
		}					
		public override bool Equals(object obj)
		{
			if (obj is TextPoint)
				return Equals((TextPoint)obj);
			return false;
		}
		public bool Equals(TextPoint point)
		{
			return _Offset == point._Offset && Line == point.Line;
		}
		public override int GetHashCode()
		{
	  string s = ToString();
	  return s.GetHashCode();
		}
		public int CompareTo(SourcePoint point)
		{
			return SourcePoint.CompareTo(Line, Offset, point.Line, point.Offset);
		}
	public override string ToString()
	{
	  return string.Format("{0},{1}", _Line, _Offset);
	}
		public static implicit operator SourcePoint(TextPoint point) 
		{
			return new SourcePoint(point.Line, point.Offset);
		}
		public static implicit operator TextPoint(SourcePoint point) {
			return new TextPoint(point.Line, point.Offset);
		}
		public static bool operator ==(TextPoint first, TextPoint second)
		{
			return first.Equals(second);
		}
		public static bool operator !=(TextPoint first, TextPoint second)
		{
			return !first.Equals(second);
		}
		public bool IsEmpty
		{
			get { return _Line == 0 && _Offset == 0; }
		}
	}
  [Serializable]
	public struct SourcePoint: IComparable, ICloneable, IEquatable<SourcePoint>, IComparable<SourcePoint>
	{
		#region private fields...
		int _Line;
		int _Offset;
		SourcePointBinding _Binding;
		byte _IsAnchorPushable;
		internal IDisposableEditPoint _BindingPoint;
	#endregion
	#region public static readonly fields...
	public static readonly SourcePoint Empty = new SourcePoint(0, 0);
	#endregion
	#region SourcePoint(int line, int offset)
	public SourcePoint(int line, int offset)
	{
	  _Line = line;
	  _Offset = offset;
			_IsAnchorPushable = 0;
			_BindingPoint = null;
			_Binding = SourcePointBinding.None;
	}
	#endregion
		#region SourcePoint(SourcePoint point)
		public SourcePoint(SourcePoint point)
			: this(point.Line, point.Offset)
		{
		}
		#endregion
		#region NeedToAdjustForInsertion
		private static bool NeedToAdjustForInsertion(int line, int offset, int topLine, int topOffset, bool pushable)
		{
	  int comp = CompareTo(line, offset, topLine, topOffset);
			return comp > 0 || (pushable && comp == 0);
		}
		#endregion
	#region Equals(object obj)
	public override bool Equals(object obj)
	{
	  if (obj == null)
		return false;
	  if (obj is SourcePoint)
		return Equals((SourcePoint)obj);
	  return false;
	}
	#endregion
	#region Equals(SourcePoint point)
	public bool Equals(SourcePoint point)
	{
	  return Offset == point.Offset && Line == point.Line;
	}
	#endregion
	#region Equals(int line, int offset)
	public bool Equals(int line, int offset)
	{
	  return Offset == offset && Line == line;
	}
	#endregion
		#region GetHashCode
	public override int GetHashCode()
	{
	  string s = ToString();
	  return s.GetHashCode();
	}
	#endregion
	#region ToString
	public override string ToString()
	{
			return String.Format("{0},{1}", _Line, _Offset);
	}
	#endregion
		#region AdjustForInsertion
		public void AdjustForInsertion(SourceRange insertion)
		{
			if (IsBoundToCode)		
				return;
			SourcePoint lTopPoint = insertion.Top;
			int lTopLine = lTopPoint.Line;
			int lTopOffset = lTopPoint.Offset;
			SourcePoint lBottomPoint = insertion.Bottom;
			int lBottomLine = lBottomPoint.Line;
			int lBottomOffset = lBottomPoint.Offset;
			if (IsAnchored)
			{
				int lBindingLine = _BindingPoint.Line;
				int lBindingOffset = _BindingPoint.Offset;
				if (CompareTo(lBindingLine, lBindingOffset, lBottomLine, lBottomOffset) >= 0)
					return;
			}
			int lThisLine = this.Line;
			int lThisOffset = this.Offset;
			if (!NeedToAdjustForInsertion(lThisLine, lThisOffset, lTopLine, lTopOffset, IsAnchorPushable))
				return;
			if (lTopLine == lThisLine)
			{
				int lNewOffset = lThisOffset + lBottomOffset - lTopOffset;
				Line = lBottomLine;
				Offset = lNewOffset;
			}
			else
			{
				int lNumLinesInserted = lBottomLine - lTopLine;
				Line = lThisLine + lNumLinesInserted;
			}
		}
		#endregion
		#region AdjustForDeletion
		public void AdjustForDeletion(SourceRange deletion)
		{
			if (IsBoundToCode)		
				return;
			if (IsAnchored)
			{
		int lBindingLine = _BindingPoint.Line;
				int lBindingOffset = _BindingPoint.Offset;
				if (CompareTo(lBindingLine, lBindingOffset, deletion.Top.Line, deletion.Top.Offset) >= 0)
					return;
			}
			if (deletion.Contains(this))
			{
				Line = deletion.Top.Line;
				Offset = deletion.Top.Offset;
			}
			else if (this > deletion.Bottom)
			{
				if (deletion.Bottom.Line == this.Line)
				{
					int lNewOffset = this.Offset - deletion.Bottom.Offset + deletion.Top.Offset;
					Line = deletion.Top.Line;
					Offset = lNewOffset;
				}
				else
				{
					int lNumLinesDeleted = deletion.Bottom.Line - deletion.Top.Line;
					Line = Line - lNumLinesDeleted;
				}
			}
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetFast(int line, int offset)
		{
			_Line = line;
			_Offset = offset;
		}
		#region SetPoint
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("SetPoint is obsolete -- use Set method instead.")]
		public void SetPoint(int line, int offset)
		{
			Set(line, offset);
		}
		#endregion
		#region Set(int line, int offset)
		public void Set(int line, int offset)
		{
			if (IsBoundToCode)
				RemoveBinding();
			Line = line;
			Offset = offset;
		}
		#endregion
		#region Set(SourcePoint sourcePoint)
		public void Set(SourcePoint sourcePoint)
		{
			Set(sourcePoint.Line, sourcePoint.Offset);
		}
		#endregion
		#region Set(Token token)
		public void Set(Token token)
		{
			Set(token.Line, token.Column);
		}
		#endregion
		#region SetOffset
		public void SetOffset(int offset)
		{
			RemoveAllBindings();
			_Offset = offset;
		}
		#endregion
		#region BindToCode
		public void BindToCode(IDisposableEditPointFactory editPointFactory)
		{
			BindToCode(editPointFactory, false);
		}
		#endregion
		#region BindToCode
		public void BindToCode(IDisposableEditPointFactory editPointFactory, bool pushable)
		{
			if (_Line < 1 || _Offset < 1)
				return;		
			RemoveAllBindings();
	  string name = "SourcePoint binding" + (pushable ? " (pushable)" : String.Empty);
			_BindingPoint	= editPointFactory.CreateDisposableEditPoint(_Line, _Offset, name);
			_BindingPoint.IsPushable = pushable;
	  _BindingPoint.IsAnchorable = true;
			SetBinding(SourcePointBinding.EditPoint);
		}
		#endregion
		#region RemoveBinding
		public void RemoveBinding()
		{
			if (IsBoundToCode)
			{
				_Line = _BindingPoint.Line;
				_Offset = _BindingPoint.Offset;
				if (!_BindingPoint.IsDisposed)
					_BindingPoint.Dispose();
				_BindingPoint = null;
				SetBinding(SourcePointBinding.None);
			}
		}
		#endregion
		#region AnchorToSourcePoint
		public void AnchorToSourcePoint(SourcePoint sourcePoint)
		{
			AnchorToSourcePoint(sourcePoint, false);
		}
		#endregion
		#region AnchorToSourcePoint
		public void AnchorToSourcePoint(SourcePoint sourcePoint, bool pushable)
		{
			if (IsEmpty)	
				return;
			RemoveAllBindings();
			if (!sourcePoint.IsBoundToCode)
				throw new ArgumentException("sourcePoint must be bound to code before calling AnchorToSourcePoint.");
			IsAnchorPushable = pushable;
			_BindingPoint	= sourcePoint._BindingPoint;
			_Line = _Line - _BindingPoint.Line; 
			if (_Line == 0)
				_Offset = _Offset - _BindingPoint.Offset; 
			SetBinding(SourcePointBinding.Anchor);
		}
		#endregion
		#region HoistAnchor
		public void HoistAnchor()
		{
			if (IsAnchored)
			{
				if (_Line == 0)		
					_Offset = _BindingPoint.Offset + _Offset; 
				_Line = _BindingPoint.Line + _Line;
				_BindingPoint = null;
				_IsAnchorPushable = 0;
				SetBinding(SourcePointBinding.None);
			}
		}
		#endregion
		#region RemoveAllBindings()
		public void RemoveAllBindings()
		{
			if (IsBoundToCode)
				RemoveBinding();
			if (IsAnchored)
				HoistAnchor();
		}
		#endregion
		#region OffsetPoint
		public SourcePoint OffsetPoint(int lines, int columns)
		{
			int lNewLine = Line + lines;
			int lNewOffset = Offset + columns;
			return new SourcePoint(lNewLine, lNewOffset);
		}
		#endregion
		#region ExtractFromDocument
		public SourcePoint ExtractFromDocument(SourcePoint newOrigin)
		{
			int lNewLine = Line - (newOrigin.Line - 1);
			int lDeltaOffset = 0;
			if (Line == newOrigin.Line)
				lDeltaOffset = newOrigin.Offset - 1;
			int lNewOffset = Offset - lDeltaOffset;
			return new SourcePoint(lNewLine, lNewOffset);
		}
		#endregion
		#region RestoreToDocument
		public SourcePoint RestoreToDocument(SourcePoint customOrigin)
		{
			int lNewLine = Line + (customOrigin.Line - 1);
			int lDeltaOffset = 0;
			if (Line == 1)
				lDeltaOffset = customOrigin.Offset - 1;
			int lNewOffset = Offset + lDeltaOffset;
			return new SourcePoint(lNewLine, lNewOffset);
		}
		#endregion
		#region IsBoundToCode
		public bool IsBoundToCode
		{
			get
			{
				return _Binding == SourcePointBinding.EditPoint && HasBindingPoint;
			}
		}
		#endregion
		#region IsAnchored
		public bool IsAnchored
		{
			get
			{
				return _Binding == SourcePointBinding.Anchor && HasBindingPoint;
			}
		}
		#endregion
		#region IsEmpty
	public bool IsEmpty
	{
	  get
	  {
				return _Line == 0 && _Offset == 0 && _BindingPoint == null;
	  }
	}
	#endregion
		#region Line
		public int Line
		{
			get
			{
				if (IsBoundToCode)
					return _BindingPoint.Line;
				else if (IsAnchored)
					return _BindingPoint.Line + _Line; 
				else
					return _Line;
			}
			set
			{
				if (IsAnchored)
					_Line = value - _BindingPoint.Line; 
				else
					_Line = value;
			}
		}
		#endregion
		#region Offset
		public int Offset
		{
			get
			{
				if (IsBoundToCode)
					return _BindingPoint.Offset;
				else if (_Line == 0 && IsAnchored)
					return _BindingPoint.Offset + _Offset; 
				else
					return _Offset;
			}
			set
			{
		_Offset = value;
				if (_Line == 0 && IsAnchored)
					_Offset -= _BindingPoint.Offset; 
			}
		}
		#endregion
		#region Binding
		public SourcePointBinding Binding
		{
			get
			{
				return _Binding;
			}
		}
		#endregion
		#region BindingPoint
		public IDisposableEditPoint BindingPoint
		{
			get
			{
				return _BindingPoint;
			}
		}
		#endregion
		#region HasBindingPoint
		public bool HasBindingPoint
		{
			get
			{
				return _BindingPoint != null && !_BindingPoint.IsDisposed;
			}
		}
		#endregion
		#region IsAnchorPushable
		public bool IsAnchorPushable
		{
			get
			{
				return _IsAnchorPushable == 1;
			}
			set
			{
				_IsAnchorPushable = (byte)(value ? 1 : 0);
			}
		}
		#endregion
		#region SetBinding(SourcePointBinding binding)
		private void SetBinding(SourcePointBinding binding)
		{
			_Binding = binding;
		}
		#endregion
	#region operator ==
	public static bool operator==(SourcePoint point1, SourcePoint point2)
	{
	  return point1.Equals(point2);
	}
	#endregion
		#region operator !=
	public static bool operator!=(SourcePoint point1, SourcePoint point2)
	{
	  return !(point1 == point2);
	}
	#endregion
	#region operator >
	public static bool operator>(SourcePoint point1, SourcePoint point2)
	{
	  return (point1.CompareTo(point2) > 0);
	}
	#endregion
	#region operator <
	public static bool operator<(SourcePoint point1, SourcePoint point2)
	{
	  return (point1.CompareTo(point2) < 0);
	}
	#endregion
	#region operator >=
	public static bool operator>=(SourcePoint point1, SourcePoint point2)
	{
	  return (point1.CompareTo(point2) >= 0);
	}
	#endregion
	#region operator <=
	public static bool operator<=(SourcePoint point1, SourcePoint point2)
	{
	  return (point1.CompareTo(point2) <= 0);
	}
	#endregion
		#region CompareTo(object obj)
	public int CompareTo(object obj)
	{
	  if (obj == null)
		throw new ArgumentNullException("obj");
	  if (!(obj is SourcePoint))
		throw new ArgumentException(String.Format("obj must be of type {0}", typeof(SourcePoint).FullName));
	  return CompareTo((SourcePoint)obj);
	}
	#endregion
	#region CompareTo(SourcePoint point)
	public int CompareTo(SourcePoint point)
	{
			int lLine = Line;
			int lOffset = Offset;
			int lPointLine = point.Line;
			int lPointOffset = point.Offset;
			return CompareTo(lLine, lOffset, lPointLine, lPointOffset);
	}
	#endregion
	public int UnboundCompareTo(SourcePoint point)
	{
	  return CompareTo(_Line, _Offset, point._Line, point._Offset);
	}
		#region CompareTo
		public static int CompareTo(int firstLine, int firstOffset, int secondLine, int secondOffset)
		{
			if (firstLine > secondLine)
				return 1;
			if (firstLine < secondLine)
				return -1;
			if (firstOffset > secondOffset)
				return 1;
			if (firstOffset < secondOffset)
				return -1;
			return 0;
		}
		#endregion
		#region ICloneable Members
		object ICloneable.Clone()
		{
			return Clone();
		}
		public SourcePoint Clone()
		{
			return new SourcePoint(this);
		}
		#endregion
	}
}
