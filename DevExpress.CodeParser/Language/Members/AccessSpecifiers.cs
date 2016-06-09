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
  [Flags]
  public enum AccessSpecifiersFlags : int
  {
	None = 0x00,
	Abstract = 0x01,
	Override = 0x02,
	ReadOnly = 0x04,
	Static = 0x08,
	Extern = 0x10,
	Virtual = 0x20,
	New = 0x40,
	Volatile = 0x80,
	Sealed = 0x100,
	Unsafe = 0x200,
	WithEvents = 0x400,
	WriteOnly = 0x800,
	Default = 0x1000,
	Overloads = 0x2000,
	Auto = 0x4000,
	Register = 0x8000,
	Vutable = 0x10000,
	Inline = 0x20000,
	Explicit = 0x40000,
	Friend = 0x80000,
	TypeDef = 0x100000,
	Mutable = 0x200000,
	Const = 0x400000,
	ConstVolatile = 0x800000,
	Partial = 0x1000000,
	PureVirtual = 0x2000000,
	Async = 0x4000000,
	Iterator = 0x8000000
  }
  public class AccessSpecifiers : ICloneable
  {
	DocumentHistorySlice _History;
	TextRangeWrapper _VisibilityRange;
	TextRangeWrapper _VirtualOverrideAbstractRange;
	TextRangeWrapper _VirtualRange;
	TextRangeWrapper _OverrideRange;
	TextRangeWrapper _AbstractRange;
	TextRangeWrapper _StaticRange;
	TextRangeWrapper _UnsafeRange;
	TextRangeWrapper _NewRange;
	TextRangeWrapper _SealedRange;
	TextRangeWrapper _ReadOnlyRange;
	TextRangeWrapper _ExternRange;
	TextRangeWrapper _VolatileRange;
	TextRangeWrapper _WithEventsRange;
	TextRangeWrapper _WriteOnlyRange;
	TextRangeWrapper _DefaultRange;
	TextRangeWrapper _OverloadsRange;
	TextRangeWrapper _PartialRange;
	TextRangeWrapper _AsynchronousRange;
	TextRangeWrapper _IteratorRange;
	TextRangeWrapper _AutoRange;
	TextRangeWrapper _RegisterRange;
	TextRangeWrapper _MutableRange;
	TextRangeWrapper _InlineRange;
	TextRangeWrapper _ExplicitRange;
	TextRangeWrapper _FriendRange;
	TextRangeWrapper _TypeDefRange;
	AccessSpecifiersFlags _Specifiers = AccessSpecifiersFlags.None;
	void Set(ref TextRangeWrapper range, Token token)
	{
	  range = token.Range;
	}
	void Set(ref TextRangeWrapper range, SourceRange sourceRange)
	{
	  range = sourceRange;
	}
	void SetEnd(ref TextRangeWrapper range, SourcePoint point)
	{
	  if (range == null)
		range = new TextRangeWrapper(new TextRange(new TextPoint(), point));
	  else
		range.Range.SetEnd(point);
	}
	void SetEnd(ref TextRangeWrapper range, Token token)
	{
	  TextPoint lEnd = new TextPoint(token.Line, token.EndColumn);
	  if (range == null)
		range = new TextRangeWrapper(new TextRange(new TextPoint(), lEnd));
	  else
		range.Range.SetEnd(lEnd);
	}
	SourceRange Get(TextRangeWrapper range)
	{
	  SourceRange lRange = range;
	  if (_History == null)
		return lRange;
	  if (lRange == SourceRange.Empty)
		return lRange;
	  return _History.Transform(lRange);
	}
	#region AccessSpecifiers
	public AccessSpecifiers()
	{
	}
	#endregion
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetHistory(DocumentHistorySlice history)
	{
	  if (_History == null)
		_History = history;
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void ClearHistory()
	{
	  UpdateRanges();
	  _History = null;
	}
	#region SetSealedRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetSealedRange(Token token)
	{
	  ClearHistory();
	  Set(ref _SealedRange, token);
	}
	#endregion
	#region SetVisibilityRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetVisibilityRange(Token token)
	{
	  ClearHistory();
	  Set(ref _VisibilityRange, token);
	}
	#endregion
	#region SetVisibilityRangeEnd
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetVisibilityRangeEnd(Token token)
	{
	  ClearHistory();
	  SetEnd(ref _VisibilityRange, token);
	}
	#endregion
	#region SetReadOnlyRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetReadOnlyRange(Token token)
	{
	  ClearHistory();
	  Set(ref _ReadOnlyRange, token);
	}
	#endregion
	#region SetVolatileRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetVolatileRange(Token token)
	{
	  ClearHistory();
	  Set(ref _VolatileRange, token);
	}
	#endregion
	#region SetNewRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetNewRange(Token token)
	{
	  ClearHistory();
	  Set(ref _NewRange, token);
	}
	#endregion
	#region SetExternRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetExternRange(Token token)
	{
	  ClearHistory();
	  Set(ref _ExternRange, token);
	}
	#endregion
	#region SetStaticRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetStaticRange(Token token)
	{
	  ClearHistory();
	  Set(ref _StaticRange, token);
	}
	#endregion
	#region SetVirtualOverrideAbstractRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetVirtualOverrideAbstractRange(Token token)
	{
	  ClearHistory();
	  Set(ref _VirtualOverrideAbstractRange, token);
	}
	#endregion
	#region SetVirtualRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetVirtualRange(Token token)
	{
	  ClearHistory();
	  Set(ref _VirtualRange, token);
	}
	#endregion
	#region SetOverrideRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetOverrideRange(Token token)
	{
	  ClearHistory();
	  Set(ref _OverrideRange, token);
	}
	#endregion
	#region SetAbstractRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetAbstractRange(Token token)
	{
	  ClearHistory();
	  Set(ref _AbstractRange, token);
	}
	#endregion
	#region SetUnsafeRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetUnsafeRange(Token token)
	{
	  ClearHistory();
	  Set(ref _UnsafeRange, token);
	}
	#endregion
	#region SetWithEventsRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetWithEventsRange(Token token)
	{
	  ClearHistory();
	  Set(ref _WithEventsRange, token);
	}
	#endregion
	#region SetWriteOnlyRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetWriteOnlyRange(Token token)
	{
	  ClearHistory();
	  Set(ref _WriteOnlyRange, token);
	}
	#endregion
	#region SetDefaultRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetDefaultRange(Token token)
	{
	  ClearHistory();
	  Set(ref _DefaultRange, token);
	}
	#endregion
	#region SetOverloadsRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetOverloadsRange(Token token)
	{
	  ClearHistory();
	  Set(ref _OverloadsRange, token);
	}
	#endregion
	#region SetSealedRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetSealedRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _SealedRange, range);
	}
	#endregion
	#region SetVisibilityRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetVisibilityRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _VisibilityRange, range);
	}
	#endregion
	#region SetVisibilityRangeEnd
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetVisibilityRangeEnd(SourceRange range)
	{
	  ClearHistory();
	  SetEnd(ref _VisibilityRange, range.End);
	}
	#endregion
	#region SetReadOnlyRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetReadOnlyRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _ReadOnlyRange, range);
	}
	#endregion
	#region SetVolatileRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetVolatileRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _VolatileRange, range);
	}
	#endregion
	#region SetNewRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetNewRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _NewRange, range);
	}
	#endregion
	#region SetExternRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetExternRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _ExternRange, range);
	}
	#endregion
	#region SetStaticRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetStaticRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _StaticRange, range);
	}
	#endregion
	#region SetVirtualOverrideAbstractRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetVirtualOverrideAbstractRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _VirtualOverrideAbstractRange, range);
	}
	#endregion
	#region SetVirtualRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetVirtualRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _VirtualRange, range);
	}
	#endregion
	#region SetOverrideRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetOverrideRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _OverrideRange, range);
	}
	#endregion
	#region SetAbstractRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetAbstractRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _AbstractRange, range);
	}
	#endregion
	#region SetUnsafeRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetUnsafeRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _UnsafeRange, range);
	}
	#endregion
	#region SetWithEventsRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetWithEventsRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _WithEventsRange, range);
	}
	#endregion
	#region SetWriteOnlyRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetWriteOnlyRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _WriteOnlyRange, range);
	}
	#endregion
	#region SetDefaultRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetDefaultRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _DefaultRange, range);
	}
	#endregion
	#region SetOverloadsRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetOverloadsRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _OverloadsRange, range);
	}
	#endregion
	#region SetPartialRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetPartialRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _PartialRange, range);
	}
	#endregion
	#region SetPartialRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetPartialRange(Token token)
	{
	  ClearHistory();
	  Set(ref _PartialRange, token);
	}
	#endregion
	#region SetAsynchronousRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetAsynchronousRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _AsynchronousRange, range);
	}
	#endregion
	#region SetIteratorRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetIteratorRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _IteratorRange, range);
	}
	#endregion
	#region SetAsynchronousRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetAsynchronousRange(Token token)
	{
	  ClearHistory();
	  Set(ref _AsynchronousRange, token);
	}
	#endregion
	#region SetIteratorRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetIteratorRange(Token token)
	{
	  ClearHistory();
	  Set(ref _IteratorRange, token);
	}
	#endregion
	#region SetAutoRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetAutoRange(Token token)
	{
	  ClearHistory();
	  Set(ref _AutoRange, token);
	}
	#endregion
	#region SetAutoRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetAutoRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _AutoRange, range);
	}
	#endregion
	#region SetRegisterRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetRegisterRange(Token token)
	{
	  ClearHistory();
	  Set(ref _RegisterRange, token);
	}
	#endregion
	#region SetRegisterRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetRegisterRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _RegisterRange, range);
	}
	#endregion
	#region SetMutableRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetMutableRange(Token token)
	{
	  ClearHistory();
	  Set(ref _MutableRange, token);
	}
	#endregion
	#region SetMutableRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetMutableRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _MutableRange, range);
	}
	#endregion
	#region SetInlineRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetInlineRange(Token token)
	{
	  ClearHistory();
	  Set(ref _InlineRange, token);
	}
	#endregion
	#region SetInlineRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetInlineRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _InlineRange, range);
	}
	#endregion
	#region SetExplicitRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetExplicitRange(Token token)
	{
	  ClearHistory();
	  Set(ref _ExplicitRange, token);
	}
	#endregion
	#region SetExplicitRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetExplicitRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _ExplicitRange, range);
	}
	#endregion
	#region SetFriendRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetFriendRange(Token token)
	{
	  ClearHistory();
	  Set(ref _FriendRange, token);
	}
	#endregion
	#region SetFriendRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetFriendRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _FriendRange, range);
	}
	#endregion
	#region SetTypeDefRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetTypeDefRange(Token token)
	{
	  ClearHistory();
	  Set(ref _TypeDefRange, token);
	}
	#endregion
	#region SetTypeDefRange
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetTypeDefRange(SourceRange range)
	{
	  ClearHistory();
	  Set(ref _TypeDefRange, range);
	}
	#endregion
	#region ICloneable Members
	object ICloneable.Clone()
	{
	  return Clone();
	}
	public AccessSpecifiers Clone()
	{
	  AccessSpecifiers lClone = new AccessSpecifiers();
	  lClone.SetVirtualOverrideAbstractRange(VirtualOverrideAbstractRange);
	  lClone.SetVirtualRange(VirtualRange);
	  lClone.SetOverrideRange(OverrideRange);
	  lClone.SetAbstractRange(AbstractRange);
	  lClone.SetStaticRange(StaticRange);
	  lClone.SetUnsafeRange(UnsafeRange);
	  lClone.SetNewRange(NewRange);
	  lClone.SetSealedRange(SealedRange);
	  lClone.SetReadOnlyRange(ReadOnlyRange);
	  lClone.SetExternRange(ExternRange);
	  lClone.SetVolatileRange(VolatileRange);
	  lClone.SetWithEventsRange(WithEventsRange);
	  lClone.SetWriteOnlyRange(WriteOnlyRange);
	  lClone.SetDefaultRange(DefaultRange);
	  lClone.SetOverloadsRange(OverloadsRange);
	  lClone.SetVisibilityRange(VisibilityRange);
	  lClone.SetPartialRange(PartialRange);
	  lClone.SetAutoRange(AutoRange);
	  lClone.SetRegisterRange(RegisterRange);
	  lClone.SetMutableRange(MutableRange);
	  lClone.SetInlineRange(InlineRange);
	  lClone.SetExplicitRange(ExplicitRange);
	  lClone.SetFriendRange(FriendRange);
	  lClone.SetTypeDefRange(TypeDefRange);
	  lClone.SetAsynchronousRange(AsynchronousRange);
	  lClone._Specifiers = _Specifiers;
	  return lClone;
	}
	#endregion
	protected void UpdateRanges()
	{
	  _VisibilityRange = VisibilityRange;
	  _VirtualOverrideAbstractRange = VirtualOverrideAbstractRange;
	  _VirtualRange = VirtualRange;
	  _OverrideRange = OverrideRange;
	  _AbstractRange = AbstractRange;
	  _StaticRange = StaticRange;
	  _UnsafeRange = UnsafeRange;
	  _NewRange = NewRange;
	  _SealedRange = SealedRange;
	  _ReadOnlyRange = ReadOnlyRange;
	  _ExternRange = ExternRange;
	  _VolatileRange = VolatileRange;
	  _WithEventsRange = WithEventsRange;
	  _WriteOnlyRange = WriteOnlyRange;
	  _DefaultRange = DefaultRange;
	  _OverloadsRange = OverloadsRange;
	  _PartialRange = PartialRange;
	  _AsynchronousRange = AsynchronousRange;
	  _IteratorRange = IteratorRange;
	  _AutoRange = AutoRange;
	  _RegisterRange = RegisterRange;
	  _MutableRange = MutableRange;
	  _InlineRange = InlineRange;
	  _ExplicitRange = ExplicitRange;
	  _FriendRange = FriendRange;
	  _TypeDefRange = TypeDefRange;
	}
	public bool SpecifierIsSet(AccessSpecifiersFlags flag)
	{
	  return (_Specifiers & flag) == flag;
	}
	public void SetSpecifierValue(AccessSpecifiersFlags flag, bool value)
	{
	  _Specifiers = value ? _Specifiers | flag : _Specifiers & (~flag);
	}
	#region IsAbstract
	public bool IsAbstract
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Abstract);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Abstract, value);
	  }
	}
	#endregion
	#region IsOverride
	public bool IsOverride
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Override);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Override, value);
	  }
	}
	#endregion
	#region IsReadOnly
	public bool IsReadOnly
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.ReadOnly);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.ReadOnly, value);
	  }
	}
	#endregion
	#region IsStatic
	public bool IsStatic
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Static);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Static, value);
	  }
	}
	#endregion
	#region IsExtern
	public bool IsExtern
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Extern);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Extern, value);
	  }
	}
	#endregion
	#region IsVirtual
	public bool IsVirtual
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Virtual);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Virtual, value);
	  }
	}
	#endregion
	#region IsNew
	public bool IsNew
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.New);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.New, value);
	  }
	}
	#endregion
	#region IsVolatile
	public bool IsVolatile
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Volatile);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Volatile, value);
	  }
	}
	#endregion
	#region IsConst
	public bool IsConst
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Const);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Const, value);
	  }
	}
	#endregion
	#region IsConstVolatile
	public bool IsConstVolatile
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.ConstVolatile);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.ConstVolatile, value);
	  }
	}
	#endregion
	#region IsSealed
	public bool IsSealed
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Sealed);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Sealed, value);
	  }
	}
	#endregion
	#region IsUnsafe
	public bool IsUnsafe
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Unsafe);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Unsafe, value);
	  }
	}
	#endregion
	#region IsWithEvents
	public bool IsWithEvents
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.WithEvents);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.WithEvents, value);
	  }
	}
	#endregion
	#region IsWriteOnly
	public bool IsWriteOnly
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.WriteOnly);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.WriteOnly, value);
	  }
	}
	#endregion
	#region IsDefault
	public bool IsDefault
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Default);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Default, value);
	  }
	}
	#endregion
	#region IsOverloads
	public bool IsOverloads
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Overloads);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Overloads, value);
	  }
	}
	#endregion
	#region IsAuto
	public bool IsAuto
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Auto);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Auto, value);
	  }
	}
	#endregion
	#region IsRegister
	public bool IsRegister
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Register);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Register, value);
	  }
	}
	#endregion
	#region IsMutable
	public bool IsMutable
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Mutable);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Mutable, value);
	  }
	}
	#endregion
	#region IsInline
	public bool IsInline
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Inline);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Inline, value);
	  }
	}
	#endregion
	#region IsExplicit
	public bool IsExplicit
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Explicit);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Explicit, value);
	  }
	}
	#endregion
	#region IsFriend
	public bool IsFriend
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Friend);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Friend, value);
	  }
	}
	#endregion
	#region IsTypeDef
	public bool IsTypeDef
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.TypeDef);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.TypeDef, value);
	  }
	}
	#endregion
	#region IsPartial
	public bool IsPartial
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Partial);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Partial, value);
	  }
	}
	#endregion
	#region IsPureVirtual
	public bool IsPureVirtual
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.PureVirtual);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.PureVirtual, value);
	  }
	}
	#endregion
	#region IsAsynchronous
	public bool IsAsynchronous
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Async);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Async, value);
	  }
	}
	#endregion
	#region IsIterator
	public bool IsIterator
	{
	  get
	  {
		return SpecifierIsSet(AccessSpecifiersFlags.Iterator);
	  }
	  set
	  {
		SetSpecifierValue(AccessSpecifiersFlags.Iterator, value);
	  }
	}
	#endregion
	[EditorBrowsable(EditorBrowsableState.Never)]
	public AccessSpecifiersFlags Specifiers
	{
	  get
	  {
		return _Specifiers;
	  }
	}
	public void ResetCVFlags()
	{
	  _Specifiers = _Specifiers & ~AccessSpecifiersFlags.Const;
	  _Specifiers = _Specifiers & ~AccessSpecifiersFlags.ConstVolatile;
	  _Specifiers = _Specifiers & ~AccessSpecifiersFlags.Volatile;
	}
	public SourceRange VisibilityRange
	{
	  get
	  {
		return Get(_VisibilityRange);
	  }
	}
	public SourceRange VirtualOverrideAbstractRange
	{
	  get
	  {
		return Get(_VirtualOverrideAbstractRange);
	  }
	}
	public SourceRange VirtualRange
	{
	  get
	  {
		return Get(_VirtualRange);
	  }
	}
	public SourceRange OverrideRange
	{
	  get
	  {
		return Get(_OverrideRange);
	  }
	}
	public SourceRange AbstractRange
	{
	  get
	  {
		return Get(_AbstractRange);
	  }
	}
	public SourceRange StaticRange
	{
	  get
	  {
		return Get(_StaticRange);
	  }
	}
	public SourceRange UnsafeRange
	{
	  get
	  {
		return Get(_UnsafeRange);
	  }
	}
	public SourceRange NewRange
	{
	  get
	  {
		return Get(_NewRange);
	  }
	}
	public SourceRange SealedRange
	{
	  get
	  {
		return Get(_SealedRange);
	  }
	}
	public SourceRange ReadOnlyRange
	{
	  get
	  {
		return Get(_ReadOnlyRange);
	  }
	}
	public SourceRange ExternRange
	{
	  get
	  {
		return Get(_ExternRange);
	  }
	}
	public SourceRange VolatileRange
	{
	  get
	  {
		return Get(_VolatileRange);
	  }
	}
	public SourceRange WithEventsRange
	{
	  get
	  {
		return Get(_WithEventsRange);
	  }
	}
	public SourceRange WriteOnlyRange
	{
	  get
	  {
		return Get(_WriteOnlyRange);
	  }
	}
	public SourceRange DefaultRange
	{
	  get
	  {
		return Get(_DefaultRange);
	  }
	}
	public SourceRange OverloadsRange
	{
	  get
	  {
		return Get(_OverloadsRange);
	  }
	}
	public SourceRange PartialRange
	{
	  get
	  {
		return Get(_PartialRange);
	  }
	}
	public SourceRange AsynchronousRange
	{
	  get
	  {
		return Get(_AsynchronousRange);
	  }
	}
	public SourceRange IteratorRange
	{
	  get
	  {
		return Get(_IteratorRange);
	  }
	}
	public SourceRange AutoRange
	{
	  get
	  {
		return Get(_AutoRange);
	  }
	}
	public SourceRange RegisterRange
	{
	  get
	  {
		return Get(_RegisterRange);
	  }
	}
	public SourceRange MutableRange
	{
	  get
	  {
		return Get(_MutableRange);
	  }
	}
	public SourceRange InlineRange
	{
	  get
	  {
		return Get(_InlineRange);
	  }
	}
	public SourceRange ExplicitRange
	{
	  get
	  {
		return Get(_ExplicitRange);
	  }
	}
	public SourceRange FriendRange
	{
	  get
	  {
		return Get(_FriendRange);
	  }
	}
	public SourceRange TypeDefRange
	{
	  get
	  {
		return Get(_TypeDefRange);
	  }
	}
  }
}
