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
	public abstract class Accessor : ParentingStatement, IMemberElementModifier
	{
		BaseVariable _ImplicitVariable;
		TextRange _NameRange;
		bool __IsNewContext;
		MemberVisibility _Visibility = MemberVisibility.Illegal;
		AccessSpecifiersFlags _Specifiers = AccessSpecifiersFlags.None;
		AccessSpecifiers _AccessSpecifiers = new AccessSpecifiers();
	bool _GenerateCodeBlock = true;
	bool _HasEndingSemicolon = false;
		private const int INT_MaintainanceComplexity = 3;
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
		}
		#region Accessor()
		public Accessor()
		{
			__IsNewContext = true;
			IsBreakable = false;
		}
		#endregion
		ITypeReferenceExpression GetTypeRef()
		{
			IHasType hasType = (IHasType)this;
			if (hasType != null)
				return null;
			return hasType.Type;
		}
		#region GetAccessorName
		protected virtual string GetAccessorName()
		{
			return String.Empty;
		}
		#endregion
   	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Accessor))
				return;
			Accessor lSource = (Accessor)source;
			_ImplicitVariable = ParserUtils.GetCloneFromNodes(this, lSource, lSource._ImplicitVariable) as BaseVariable;
			_NameRange = lSource.NameRange;
			__IsNewContext = lSource.__IsNewContext;
			_Visibility = lSource._Visibility;
			_Specifiers = lSource._Specifiers;
	  if (lSource._AccessSpecifiers != null)
			  _AccessSpecifiers = lSource._AccessSpecifiers.Clone();
	  _GenerateCodeBlock = lSource.GenerateCodeBlock;
	  _HasEndingSemicolon = lSource.HasEndingSemicolon;
		}
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
	}
	#region SetHistory
	public override void SetHistory(int index, DocumentHistorySlice history, bool isRecursive)
	{
	  if (_AccessSpecifiers != null)
		_AccessSpecifiers.SetHistory(history);
	  base.SetHistory(index, history, isRecursive);
	}
	#endregion
	public override void ClearHistory()
	{
	  if (History == null)
		return;
	  if (_AccessSpecifiers != null)
		_AccessSpecifiers.ClearHistory();
	  base.ClearHistory();
	}
		#region GetDefaultVisibility
		public override MemberVisibility GetDefaultVisibility()
		{
			return MemberVisibility.Local;
		}
		#endregion
		#region RangeIsClean
		public override bool RangeIsClean(SourceRange sourceRange)
		{
			return (sourceRange.Top > BlockStart.Bottom) && (sourceRange.Bottom < BlockEnd.Top);
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetSpecifiers(AccessSpecifiersFlags specifiers)
		{
				_Specifiers = specifiers;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool SpecifierIsSet(AccessSpecifiersFlags flag)
		{
			return (_Specifiers & flag) == flag;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetSpecifierValue(AccessSpecifiersFlags flag, bool value)
		{
			_Specifiers = value ? _Specifiers | flag : _Specifiers & (~flag);
		}
		public bool Is(string fullTypeName)
		{
			ITypeReferenceExpression typeRef = GetTypeRef();
			if (typeRef == null)
				return false;
			return typeRef.Is(fullTypeName);
		}
		public bool Is(ITypeElement type)
		{
			ITypeReferenceExpression typeRef = GetTypeRef();
			if (typeRef == null)
				return false;
			return typeRef.Is(type);
		}
		public bool Is(Type type)
		{
			ITypeReferenceExpression typeRef = GetTypeRef();
			if (typeRef == null)
				return false;
			return typeRef.Is(type);
		}
		public bool Is(ISourceTreeResolver resolver, string fullTypeName)
		{
			ITypeReferenceExpression typeRef = GetTypeRef();
			if (typeRef == null)
				return false;
			return typeRef.Is(resolver, fullTypeName);
		}
		public BaseVariable ImplicitVariable
		{
			get
			{
				return _ImplicitVariable;
			}
			set
			{
				_ImplicitVariable = value;
			}
		}
		public bool HasImplicitVariable
		{
			get
			{
				return _ImplicitVariable != null;
			}
		}
		public override SourceRange NameRange
		{
			get
			{
				return GetTransformedRange(_NameRange);
			}
			set
			{
		ClearHistory();
				_NameRange = value;
			}
		}
		public override bool CanContainCode 
		{
			get 
			{
				return true;
			}
		}
		public override bool IsNewContext
		{
			get 
			{
				return __IsNewContext;
			}
		}
		public MemberVisibility Visibility
		{
			get
			{
				return _Visibility;
			}
			set
			{
				_Visibility = value;
			}
		}
	public SourceRange VisibilityRange
	{
	  get
	  {
		if (HasAccessSpecifiers)
		  return _AccessSpecifiers.VisibilityRange;
		return SourceRange.Empty;
	  }
	  set
	  {
		EnsureAccessSpecifiers();
		_AccessSpecifiers.SetVisibilityRange(value);
	  }
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
		#region HasAccessSpecifiers
		public bool HasAccessSpecifiers
		{
			get
			{
				return _AccessSpecifiers != null;
			}
		}
		#endregion
		void EnsureAccessSpecifiers()
		{
			if (_AccessSpecifiers == null)
				_AccessSpecifiers = new AccessSpecifiers();
		}
		#region OverrideRange
		public SourceRange OverrideRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.OverrideRange;
				return SourceRange.Empty;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.SetOverrideRange(value);
			}
		}
		#endregion
		#region AbstractRange
		public SourceRange AbstractRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.AbstractRange;
				return SourceRange.Empty;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.SetAbstractRange(value);
			}
		}
		#endregion
		#region VirtualRange
		public SourceRange VirtualRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.VirtualRange;
				return SourceRange.Empty;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.SetVirtualRange(value);
			}
		}
		#endregion
	public override string  PathSegment
	  {
	  get
	  {
		return StructuralParserServicesHolder.GetSignaturePart(this);
	  }
	}
	public bool HasEndingSemicolon
	{
	  get
	  {
		return _HasEndingSemicolon;
	  }
	  set
	  {
		_HasEndingSemicolon = value;
	  }
	}
	public bool GenerateCodeBlock
	{
	  get
	  {
		return _GenerateCodeBlock;
	  }
	  set
	  {
		_GenerateCodeBlock = value;
	  }
	}
	#region IHasAttributesModifier Members
	AttributeSection CreateAttributeSection(IAttributeElement attribute)
	{
	  AttributeSection section = new AttributeSection();
	  section.AddAttribute(attribute as Attribute);
	  return section;
	}
	void IHasAttributesModifier.AddAttribute(IAttributeElement attribute)
	{
	  AddAttributeSection(CreateAttributeSection(attribute));
	}
	void IHasAttributesModifier.RemoveAttribute(IAttributeElement attribute)
	{
	  if (AttributeSections == null)
		return;
	  foreach (AttributeSection section in AttributeSections)
	  {
		if (section == null)
		  continue;
		if (section.AttributeCollection.Contains(attribute))
		{
		  section.RemoveAttribute(attribute as Attribute);
		  return;
		}
	  }
	}
	#endregion
	#region IMemberElementModifier Members
	void IMemberElementModifier.SetIsAbstract(bool isAbstract)
	{
	  IsAbstract = isAbstract;
	}
	void IMemberElementModifier.SetIsExtern(bool isExtern)
	{
	  IsExtern = isExtern;
	}
	void IMemberElementModifier.SetIsOverride(bool isOverride)
	{
	  IsOverride = isOverride;
	}
	void IMemberElementModifier.SetIsReadOnly(bool isReadOnly)
	{
	  IsReadOnly = isReadOnly;
	}
	void IMemberElementModifier.SetIsSealed(bool isSealed)
	{
	  IsSealed = isSealed;
	}
	void IMemberElementModifier.SetIsStatic(bool isStatic)
	{
		IsStatic = isStatic;
	}
	void IMemberElementModifier.SetIsVirtual(bool isVirtual)
	{
	  IsVirtual = isVirtual;
	}
	void IMemberElementModifier.SetVisibility(MemberVisibility visibility)
	{
	  Visibility = visibility;
	}
	#endregion
  }
}
