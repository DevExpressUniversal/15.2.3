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
using System.Collections;
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public abstract class AccessSpecifiedElement: DelimiterCapableBlock, IMemberElement, IGenericElement, IMemberElementModifier, IGenericElementModifier
	{
		bool _IsDefaultVisibility = true;
		MemberVisibility _Visibility = MemberVisibility.Illegal;
		MemberVisibility[] _ValidVisiblities;
		AccessSpecifiers _AccessSpecifiers;
		WeakReference _GenericTemplate;	
		String _StorageClassSpecifier = String.Empty;
		Expression _NameQualifier;
		GenericModifier _GenericModifier;
		TextRange _NameRange;
	bool IsProtectedOrInternal(MemberVisibility visibility)
	{
	  return visibility == MemberVisibility.Protected ||
		visibility == MemberVisibility.Internal;
	}
	bool CanSetProtectedInternalVisibility(MemberVisibility visibility)
	{
	  if (!IsProtectedOrInternal(_Visibility))
		return false;
	  if (!IsProtectedOrInternal(visibility))
		return false;
	  if (IsDefaultVisibility)
		return false;
	  return true;
	}
	#region EnsureAccessSpecifiers
	protected void EnsureAccessSpecifiers()
		{
			if (_AccessSpecifiers == null)
				_AccessSpecifiers = new AccessSpecifiers();
	}
	#endregion
	#region SetSealedRange
	[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetSealedRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetSealedRange(token);
		}
		#endregion
		#region SetReadOnlyRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetReadOnlyRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetReadOnlyRange(token);
		}
		#endregion
		#region SetVolatileRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetVolatileRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetVolatileRange(token);
		}
		#endregion
		#region SetNewRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetNewRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetNewRange(token);
		}
		#endregion
		#region SetVisibility
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetVisibility(MemberVisibility visibility, Token token)
		{
	  if (CanSetProtectedInternalVisibility(visibility))
			{
				_Visibility = MemberVisibility.ProtectedInternal;
				EnsureAccessSpecifiers();
				_AccessSpecifiers.SetVisibilityRangeEnd(token);
			}
			else
			{
				_Visibility = visibility;
				EnsureAccessSpecifiers();
				_AccessSpecifiers.SetVisibilityRange(token);
			}
			_IsDefaultVisibility = false;
		}
		#endregion
		#region SetVisibility
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetVisibility(MemberVisibility visibility)
		{
			_Visibility = visibility;
			_IsDefaultVisibility = false;
		}
		#endregion
		#region SetExternRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetExternRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetExternRange(token);
		}
		#endregion
		#region SetStaticRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetStaticRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetStaticRange(token);
		}
		#endregion
		#region SetVirtualOverrideAbstractRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetVirtualOverrideAbstractRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetVirtualOverrideAbstractRange(token);
		}
		#endregion
		#region SetVirtualRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetVirtualRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetVirtualRange(token);
		}
		#endregion
		#region SetOverrideRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetOverrideRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetOverrideRange(token);
		}
		#endregion
		#region SetAbstractRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetAbstractRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetAbstractRange(token);
		}
		#endregion
		#region SetUnsafeRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetUnsafeRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetUnsafeRange(token);
		}
		#endregion
		#region SetWithEventsRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetWithEventsRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetWithEventsRange(token);
		}
		#endregion
		#region SetWriteOnlyRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetWriteOnlyRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetWriteOnlyRange(token);
		}
		#endregion
		#region SetDefaultRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetDefaultRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetDefaultRange(token);
		}
		#endregion
		#region SetOverloadsRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetOverloadsRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetOverloadsRange(token);
		}
		#endregion
		#region SetAutoRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetAutoRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetAutoRange(token);
		}
		#endregion
		#region SetRegisterRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetRegisterRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetRegisterRange(token);
		}
		#endregion
		#region SetAutoRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetMutableRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetMutableRange(token);
		}
		#endregion
		#region SetInlineRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetInlineRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetInlineRange(token);
		}
		#endregion
		#region SetExplicitRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetExplicitRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetExplicitRange(token);
		}
		#endregion
		#region SetFriendRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetFriendRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetFriendRange(token);
		}
		#endregion
		#region SetTypeDefRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetTypeDefRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetTypeDefRange(token);
		}
		#endregion
		#region SetPartialRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetPartialRange(SourceRange range)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetPartialRange(range);
		}
		#endregion
		#region SetPartialRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetPartialRange(Token token)
		{
			EnsureAccessSpecifiers();
			_AccessSpecifiers.SetPartialRange(token);
		}
		#endregion
		#region SetAccessSpecifiers
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void SetAccessSpecifiers(AccessSpecifiers specifiers)
		{
			if (specifiers == null)
				return;
			_AccessSpecifiers = specifiers;
		}
		#endregion
	#region SetHistory
	public override void SetHistory(int index, DocumentHistorySlice history, bool isRecursive)
		{
			EnsureAccessSpecifiers();
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
	#region CleanUpOwnedReferences
	public override void CleanUpOwnedReferences()
		{
			_GenericModifier = null;
			_GenericTemplate = null;
			base.CleanUpOwnedReferences();
	}
	#endregion
	#region ReplaceOwnedReference
	protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_GenericModifier == oldElement && newElement is GenericModifier)
				_GenericModifier = (GenericModifier)newElement;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
	}
	#endregion
		#region GetCanBeDocumented
		protected internal override bool GetCanBeDocumented()
		{
			return true;
		}
		#endregion
		#region SetParent(LanguageElement parent)
		public override void SetParent(LanguageElement parent)
		{
			base.SetParent(parent);
			if (parent != null)
			{
				if (_Visibility == MemberVisibility.Illegal)
				{
					_Visibility = parent.GetDefaultVisibility();
					_IsDefaultVisibility = true;
				}
				_ValidVisiblities = parent.GetValidVisibilities();
			}
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is AccessSpecifiedElement))
				return;			
			AccessSpecifiedElement lSource = (AccessSpecifiedElement)source;
			if (options.KeepAccessSpecifierTemplate)
				GenericTemplate = lSource;
			_NameRange = lSource.NameRange;
			_IsDefaultVisibility = lSource._IsDefaultVisibility;
			_StorageClassSpecifier = lSource.StorageClassSpecifier;
			_Visibility = lSource._Visibility;
	  if (lSource._ValidVisiblities != null)
			{
				_ValidVisiblities = new MemberVisibility[lSource._ValidVisiblities.Length];
				for (int i = 0; i < lSource._ValidVisiblities.Length; i++)
					_ValidVisiblities[i] = lSource._ValidVisiblities[i];
			}
			if (lSource.HasAccessSpecifiers)
				_AccessSpecifiers = lSource._AccessSpecifiers.Clone();
			if (lSource.GenericModifier != null)
			{
				_GenericModifier = ParserUtils.GetCloneFromNodes(this, lSource, lSource.GenericModifier) as GenericModifier;
				if (_GenericModifier == null)
					_GenericModifier = lSource.GenericModifier.Clone(options) as GenericModifier;
			}
			if (lSource._NameQualifier != null)
			{
				_NameQualifier = ParserUtils.GetCloneFromNodes(this, lSource, lSource.NameQualifier) as Expression;
				if (_NameQualifier == null)
					_NameQualifier = lSource.NameQualifier.Clone(options) as Expression;
			}
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
	}
	#region GetImplementExpressions
		protected virtual IExpressionCollection GetImplementExpressions()
		{
			return null;
	}
	#endregion
	public void SetNameRange(SourceRange nameRange)
	{
	  _NameRange = nameRange;
	}
		#region GetVisibilityImageIndex
		public virtual int GetVisibilityImageIndex()
		{
			switch (Visibility)		
			{
				case MemberVisibility.Private:
					return ImageIndex.Private;
				case MemberVisibility.Protected:
					return ImageIndex.Protected;
				case MemberVisibility.ProtectedInternal:
					return ImageIndex.ProtectedOrInternal;
				case MemberVisibility.Internal:
					return ImageIndex.Internal;
				default:
					return ImageIndex.Public;
			}
		}
		#endregion
		#region SupportsVisibility
		public bool SupportsVisibility(MemberVisibility aMemberVisibility)
		{
			if (ValidVisibilities != null)
				foreach(MemberVisibility lSupportedVisibility in ValidVisibilities)
					if (lSupportedVisibility == aMemberVisibility)
						return true;
			return false;
		}
		#endregion
	#region SetGenericModifier
		public void SetGenericModifier(GenericModifier modifier)
		{
			ReplaceDetailNode(_GenericModifier, modifier);
			_GenericModifier = modifier;
	}
	#endregion
	#region GetUnusedDeclarations
		public LanguageElementCollection GetUnusedDeclarations() 
		{
			return StructuralParserServicesHolder.GetUnusedDeclarations(AllVariables);
	}
	#endregion
	#region IsVisibleFrom
		public override bool IsVisibleFrom(LanguageElement viewer)
		{
			if (base.IsVisibleFrom(viewer))
				return true;
			if (Visibility == MemberVisibility.Public)
				return true;
			if (HasInternalAccess && InSameProject(viewer))
				return true;
			if (HasProtectedAccess || Visibility == MemberVisibility.Private)
			{
				Class lViewerClass = viewer.GetDeclaringType();
				if (lViewerClass == null)
					return false;
				if (lViewerClass.DeclaresMember(this))
					return true;
				if (Visibility == MemberVisibility.Private)
					return false;
				if (lViewerClass.AncestorDeclaresMember(this))
					return true;
			}
			return false;
		}
		#endregion
		#region ValidVisibilities
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected MemberVisibility[] BaseValidVisibilities
		{
			get
			{
				return _ValidVisiblities;
			}
		}
		#endregion
	#region NameRange
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
	#endregion
	#region DeclaresIdentifier
	public override bool DeclaresIdentifier
		{
			get
			{
				return true;
			}
	}
	#endregion
	#region AccessSpecifiers
		public AccessSpecifiers AccessSpecifiers
		{
			get
			{
				return _AccessSpecifiers;
			}
			set
			{
				_AccessSpecifiers = value;
			}
		}
		#endregion
		#region AccessibleByDecendants
		public bool AccessibleByDecendants
		{
			get
			{
				return Visibility == MemberVisibility.Public || 
					Visibility == MemberVisibility.Protected || 
					Visibility == MemberVisibility.ProtectedInternal;
			}
		}
		#endregion
		#region HasInternalAccess
		public bool HasInternalAccess
		{
			get
			{
				return Visibility == MemberVisibility.Internal || 
					Visibility == MemberVisibility.ProtectedInternal;
			}
		}
		#endregion
		#region HasProtectedAccess
		public bool HasProtectedAccess
		{
			get
			{
				return Visibility == MemberVisibility.Protected || 
					Visibility == MemberVisibility.ProtectedInternal;
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
		#region ValidVisibilities
		public virtual MemberVisibility[] ValidVisibilities
		{
			get
			{
				return _ValidVisiblities;
			}
			set
			{
				_ValidVisiblities = value;
			}
		}
		#endregion		
		#region Visibility
		[Description("The visibility of this member (e.g., private, protected, internal, public, local, etc.).")]
		[Category("Access")]
		[DefaultValue(MemberVisibility.Public)]
		public virtual MemberVisibility Visibility
		{
			get
			{
				return _Visibility;
			}
			set 
			{
				SetVisibility(value);
			}
		}
		#endregion
		#region VisibilityIsFixed
		[Description("True if the visibility of this member can not be changed (e.g., illegal, param, or local).")]
		[Category("Access")]
		public virtual bool VisibilityIsFixed
		{
			get
			{
				return _Visibility == MemberVisibility.Illegal || _Visibility == MemberVisibility.Local || 
					_Visibility == MemberVisibility.Param;
			}
		}
		#endregion
		#region VisibilityRange
		[Category("Access")]
		public SourceRange VisibilityRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.VisibilityRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region VirtualOverrideAbstractRange
		[Category("Access")]
		public SourceRange VirtualOverrideAbstractRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.VirtualOverrideAbstractRange;
				return SourceRange.Empty;
			}
		}
		#endregion
	#region IteratorRange
	[Category("Access")]
	public SourceRange IteratorRange
	{
	  get
	  {
		if (HasAccessSpecifiers)
		  return _AccessSpecifiers.IteratorRange;
		return SourceRange.Empty;
	  }
	}
	#endregion
		#region VirtualRange
		[Category("Access")]
		public SourceRange VirtualRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.VirtualRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region OverrideRange
		[Category("Access")]
		public SourceRange OverrideRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.OverrideRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region VirtualOverrideAbstractRange
		[Category("Access")]
		public SourceRange AbstractRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.AbstractRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region StaticRange
		[Category("Access")]
		public SourceRange StaticRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.StaticRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region UnsafeRange
		[Category("Access")]
		public SourceRange UnsafeRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.UnsafeRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region NewRange
		[Category("Access")]
		public SourceRange NewRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.NewRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region SealedRange
		[Category("Access")]
		public SourceRange SealedRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.SealedRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region ReadOnlyRange
		[Category("Access")]
		public SourceRange ReadOnlyRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.ReadOnlyRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region WriteOnlyRange
		[Category("Access")]
		public SourceRange WriteOnlyRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.WriteOnlyRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region DefaultRange
		[Category("Access")]
		public SourceRange DefaultRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.DefaultRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region OverloadsRange
		[Category("Access")]
		public SourceRange OverloadsRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.OverloadsRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region ExternRange
		[Category("Access")]
		public SourceRange ExternRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.ExternRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region VolatileRange
		[Category("Access")]
		public SourceRange VolatileRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.VolatileRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region AutoRange
		[Category("Access")]
		public SourceRange AutoRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.AutoRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region RegisterRange
		[Category("Access")]
		public SourceRange RegisterRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.RegisterRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region MutableRange
		[Category("Access")]
		public SourceRange MutableRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.MutableRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region InlineRange
		[Category("Access")]
		public SourceRange InlineRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.InlineRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region ExplicitRange
		[Category("Access")]
		public SourceRange ExplicitRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.ExplicitRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region FriendRange
		[Category("Access")]
		public SourceRange FriendRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.FriendRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region TypeDefRange
		[Category("Access")]
		public SourceRange TypeDefRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.TypeDefRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region PartialRange
		[Category("Access")]
		public SourceRange PartialRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.PartialRange;
				return SourceRange.Empty;
			}
		}
		#endregion
		#region WithEventsRange
		[Category("Access")]
		public SourceRange WithEventsRange
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.WithEventsRange;
				return SourceRange.Empty;
			}
		}
		#endregion
	#region GenericModifier
		public GenericModifier GenericModifier
		{
			get { return _GenericModifier; }
	}
	#endregion
	#region IsActivatedGeneric
	public bool IsActivatedGeneric
		{
			get
			{
				return _GenericTemplate != null && _GenericTemplate.IsAlive;
			}
	}
	#endregion
	#region GenericTemplate
	public AccessSpecifiedElement GenericTemplate
		{
			get
			{
				if (IsActivatedGeneric)
					return _GenericTemplate.Target as AccessSpecifiedElement;
				return null;
			}
			set
			{
				if (value == null)
				{
					_GenericTemplate = null;
					return;
				}
				if (_GenericTemplate == null)
					_GenericTemplate = new WeakReference(value);
			}
	}
	#endregion
	#region IsDefaultVisibility
		[Description("True if the visibility for this member is derived from its parent's default visibility. False if a visibility keyword was specified (e.g., \"public\", \"protected\", \"private\", etc.) in the declaration of this member.")]
		[Category("Access")]
		[DefaultValue(true)]
		public virtual bool IsDefaultVisibility
		{
			get
			{
				return _IsDefaultVisibility;
			}
			set
			{
				if (_IsDefaultVisibility == value)
					return;
				_IsDefaultVisibility = value;
			}
		}
		#endregion
		#region IsAbstract
		[Description("True if this is an abstract member.")]
		[Category("Access")]
		[DefaultValue(false)]
		public bool IsAbstract
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsAbstract;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsAbstract = value;
			}
		}
		#endregion
	#region IsIterator
	[Description("True if this is an iterator member.")]
	[Category("Access")]
	[DefaultValue(false)]
	public bool IsIterator
	{
	  get
	  {
		if (HasAccessSpecifiers)
		  return _AccessSpecifiers.IsIterator;
		return false;
	  }
	  set
	  {
		EnsureAccessSpecifiers();
		_AccessSpecifiers.IsIterator = value;
	  }
	}
	#endregion
	#region IsGeneric
		public bool IsGeneric
		{
			get { return _GenericModifier != null; }
	}
	#endregion
	#region IsOverride
		[Description("True if this member overrides an ancestor member.")]
		[Category("Access")]
		[DefaultValue(false)]
		public bool IsOverride
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsOverride;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsOverride = value;
			}
		}
		#endregion
		#region IsReadOnly
		[Description("True if this is a read-only member.")]
		[Category("Modifiers")]
		public virtual bool IsReadOnly
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsReadOnly;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsReadOnly = value;
			}
		}
		#endregion
		#region IsWriteOnly
		[Description("True if this is a write-only member. (This construct is supported in VB).")]
		[Category("Modifiers")]
		[DefaultValue(false)]
		public bool IsWriteOnly
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsWriteOnly;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsWriteOnly = value;
			}
		}
		#endregion
		#region IsDefault
		[Description("True if this is a Default member (this construct is supported in VB).")]
		[Category("Access")]
		[DefaultValue(false)]
		public bool IsDefault
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsDefault;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsDefault = value;
			}
		}
		#endregion
		#region IsStatic
		[Description("True if this is a static member.")]
		[Category("Modifiers")]
		[DefaultValue(false)]
		public virtual bool IsStatic
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsStatic;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsStatic = value;
			}
		}
		#endregion
		#region IsExtern
		[Description("True if this is member is externally defined.")]
		[Category("Access")]
		[DefaultValue(false)]
		public bool IsExtern
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsExtern;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsExtern = value;
			}
		}
		#endregion
		#region IsVirtual
		[Description("True if this member is virtual.")]
		[Category("Access")]
		[DefaultValue(false)]
		public bool IsVirtual
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsVirtual;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsVirtual = value;
			}
		}
		#endregion
		#region IsVolatile
		[Description("True if this member can be modified in the running program by an external asynchronous source, such as the operating system, the hardware, or a concurrently executing thread.")]
		[Category("Modifiers")]
		[DefaultValue(false)]
		public virtual bool IsVolatile
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsVolatile;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsVolatile = value;
			}
		}
		#endregion
		#region IsConst
		public virtual bool IsConst
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsConst;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsConst = value;
			}
		}
		#endregion
		#region IsConstVolatile
		public virtual bool IsConstVolatile
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsConstVolatile;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsConstVolatile = value;
			}
		}
		#endregion
		#region ResetCVFlags
		public void ResetCVFlags()
		{
			if (_AccessSpecifiers != null)
				_AccessSpecifiers.ResetCVFlags();
		}
		#endregion
		#region IsNew
		[Description("True if this member explicitly hides an inherited member.")]
		[Category("Modifiers")]
		[DefaultValue(false)]
		public virtual bool IsNew
	{
	  get
	  {
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsNew;
				return false;
	  }
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsNew = value;
			}
	}
	#endregion
	#region IsSealed
	[Description("True if this is a sealed member.")]
	[Category("Access")]
	[DefaultValue(false)]
	public bool IsSealed
	{
	  get
	  {
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsSealed;
				return false;
	  }
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsSealed = value;
			}
	}
	#endregion
	#region IsUnsafe
	[Description("True if this is an unsafe member.")]
	[Category("Access")]
	[DefaultValue(false)]
	public bool IsUnsafe
	{
	  get
	  {
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsUnsafe;
				return false;
	  }
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsUnsafe = value;
			}
	}
	#endregion
		#region IsWithEvents
		[Description("True if this member refers to an instance of a class that can raise events (this construct is supported in VB).")]
		[Category("Access")]
		[DefaultValue(false)]
		public virtual bool IsWithEvents
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsWithEvents;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsWithEvents = value;
			}
		}
		#endregion
		#region IsOverloads
		[Description("True if this member has Overloads modifier (this construct is supported in VB).")]
		[Category("Access")]
		[DefaultValue(false)]
		public bool IsOverloads
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsOverloads;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsOverloads = value;
			}
		}
		#endregion
		#region IsAuto
	[DefaultValue(false)]
	public bool IsAuto
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsAuto;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsAuto = value;
			}
		}
		#endregion
		#region IsRegister
	[DefaultValue(false)]
	public bool IsRegister
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsRegister;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsRegister = value;
			}
		}
		#endregion
		#region IsMutable
	[DefaultValue(false)]
	public bool IsMutable
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsMutable;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsMutable = value;
			}
		}
		#endregion
		#region IsInline
	[DefaultValue(false)]
	public bool IsInline
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsInline;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsInline = value;
			}
		}
		#endregion
		#region IsExplicit
	[DefaultValue(false)]
	public bool IsExplicit
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsExplicit;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsExplicit = value;
			}
		}
		#endregion
		#region IsFriend
	[DefaultValue(false)]
	public bool IsFriend
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsFriend;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsFriend = value;
			}
		}
		#endregion
		#region IsTypeDef
	[DefaultValue(false)]
	public bool IsTypeDef
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsTypeDef;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsTypeDef = value;
			}
		}
		#endregion
		#region IsPartial
	[DefaultValue(false)]
	public bool IsPartial
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsPartial;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsPartial = value;
			}
		}
		#endregion
	#region IsExplicitInterfaceMember
		[Description("Returns true if this member is explicitly declared interface member.")]
		[Category("Details")]
		public virtual bool IsExplicitInterfaceMember
		{
			get
			{
				return false;
			}
			set
			{				
			}
	}
	#endregion
	#region PathSegment
	public override string PathSegment
		{
			get
			{
				return StructuralParserServicesHolder.GetSignaturePart(this);
			}
		}
		#endregion
	#region NameQualifier
	public Expression NameQualifier
		{
			get
			{
				return _NameQualifier;
			}
			set
			{				
				ReplaceDetailNode(_NameQualifier, value);
				_NameQualifier = value;
			}
	}
	#endregion
	#region AllVariables
		public virtual IEnumerable AllVariables
		{
			get
			{
				return new ElementEnumerable(this, typeof(Variable), true);
			}
	}
	#endregion
	#region AllStatements
		public virtual IEnumerable AllStatements
		{
			get
			{
				return new ElementEnumerable(this, typeof(Statement), true);
			}
	}
	#endregion
	#region AllFlowBreaks
		public virtual IEnumerable AllFlowBreaks
		{
			get
			{
				return new ElementEnumerable(this, typeof(FlowBreak), true);
			}
	}
	#endregion
	#region HasOuterRangeChildren
		protected override bool HasOuterRangeChildren
		{
			get { return IsGeneric; }
	}
	#endregion
	#region IGenericElement Members
	ITypeParameterCollection IGenericElement.TypeParameters
		{
			get
			{
				if (GenericModifier == null || 
					GenericModifier.TypeParameters == null)
					return EmptyLiteElements.EmptyITypeParameterCollection;
				return GenericModifier.TypeParameters;
			}			
		}
		IGenericElement IGenericElement.GenericTemplate
		{
			get
			{
				return GenericTemplate;
			}
		}
		#endregion
		#region IMemberElement Members
		bool IMemberElement.IsDefaultVisibility
		{
			get
			{
				return IsDefaultVisibility;
			}
		}
		public string GetOverrideCode()
		{
	  return StructuralParserServicesHolder.GetOverrideCode(this, false, String.Empty, String.Empty);
		}
		public string GetOverrideCode(bool callBase)
		{
	  return StructuralParserServicesHolder.GetOverrideCode(this, callBase, String.Empty, String.Empty);
		}
		public string GetOverrideCode(bool callBase, string codeBefore, string codeAfter)
		{
	  return StructuralParserServicesHolder.GetOverrideCode(this, callBase, codeBefore, codeAfter);
		}
		IAttributeElementCollection IHasAttributes.Attributes
		{
			get
			{
				if (Attributes == null)
					return EmptyLiteElements.EmptyIAttributeElementCollection;
				LiteAttributeElementCollection lAttribures = new LiteAttributeElementCollection();
				lAttribures.AddRange(Attributes);
				return lAttribures;
			}
		}
		IExpression IMemberElement.NameQualifier
		{
			get
			{
				return NameQualifier;
			}
		}
		string IMemberElement.Signature
		{
			get
			{
				return SignatureBuilder.GetSignature(this);
			}
		}
		IExpressionCollection IMemberElement.Implements
		{
			get
			{
				return GetImplementExpressions();
			}
		}
		#endregion
		#region StorageClassSpecifier
		public string StorageClassSpecifier
		{
			get
			{
				return _StorageClassSpecifier;
			}
			set
			{
				_StorageClassSpecifier = value;
			}
		}
		#endregion
	#region IsPureVirtual
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsPureVirtual
		{
			get
			{
				if (HasAccessSpecifiers)
					return _AccessSpecifiers.IsPureVirtual;
				return false;
			}
			set
			{
				EnsureAccessSpecifiers();
				_AccessSpecifiers.IsPureVirtual = value;
			}
	}
	#endregion
	#region IMemberElementModifier Members
	void IMemberElementModifier.SetIsAbstract(bool isAbstract)
	{
	  IsAbstract = isAbstract;
	}
	void IMemberElementModifier.SetIsStatic(bool isStatic)
	{
	  IsStatic = isStatic;
	}
	void IMemberElementModifier.SetIsOverride(bool isOverride)
	{
	  IsOverride = isOverride;
	}
	void IMemberElementModifier.SetIsExtern(bool isExtern)
	{
	  IsExtern = isExtern;
	}
	void IMemberElementModifier.SetIsVirtual(bool isVirtual)
	{
	  IsVirtual = isVirtual;
	}
	void IMemberElementModifier.SetIsReadOnly(bool isReadOnly)
	{
	  IsReadOnly = isReadOnly;
	}
	void IMemberElementModifier.SetIsSealed(bool isSealed)
	{
	  IsSealed = isSealed;
	}
	#endregion
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
	#region IGenericElementModifier Members
	void CreateGenericModifierIfNeeded()
	{
	  if (_GenericModifier == null)
		SetGenericModifier(new GenericModifier());
	}
	void IGenericElementModifier.AddTypeParameter(ITypeParameter typeParameter)
	{
	  CreateGenericModifierIfNeeded();
	  GenericModifier.AddTypeParameter(typeParameter as TypeParameter);
	}
	void IGenericElementModifier.InsertTypeParameter(int index, ITypeParameter typeParameter)
	{
	  CreateGenericModifierIfNeeded();
	  GenericModifier.InsertTypeParameter(index, typeParameter as TypeParameter);
	}
	void IGenericElementModifier.RemoveTypeParameter(ITypeParameter typeParameter)
	{
	  CreateGenericModifierIfNeeded();
	  GenericModifier.RemoveTypeParameter(typeParameter as TypeParameter);
	}
	#endregion
  }
}
