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
	public sealed class ElementFilters
	{
		ElementFilters(){}
		static ElementFilters()
		{
			Type = new TypeDeclarationFilter();
			Namespace = new NamespaceFilter();
			TypeOrNamespace = new TypeOrNamespaceFilter();
			Member = new MemberFilter();
			NonPrivateMember = new NonPrivateMemberFilter();
			Method = new MethodFilter();
			Property = new PropertyFilter();
			Event = new EventFilter();
			Constructor = new ConstructorFilter();
			Destructor = new DestructorFilter();
			Field = new FieldFilter();
			Local = new LocalFilter();
			PublicParameterLessMethod = new PublicParameterLessMethodFilter();
		}
		public static readonly TypeDeclarationFilter Type;
		public static readonly NamespaceFilter Namespace;
		public static readonly TypeOrNamespaceFilter TypeOrNamespace;
		public static readonly NonPrivateMemberFilter NonPrivateMember;
		public static readonly MemberFilter Member;
		public static readonly MethodFilter Method;
		public static readonly PropertyFilter Property;
		public static readonly EventFilter Event;
		public static readonly ConstructorFilter Constructor;
		public static readonly DestructorFilter Destructor;
		public static readonly FieldFilter Field;
		public static readonly LocalFilter Local;
		public static readonly PublicParameterLessMethodFilter PublicParameterLessMethod;
	public static bool IsVoidTypeRef(ITypeReferenceExpression type)
	{
	  return type != null &&
		(type.Name == "void" ||
		type.Name == "System.Void");
	}
	public static bool IsVoidType(ITypeElement type)
	{
	  return type != null &&
		type.FullName == "System.Void";
	}
		public static bool IsType(IElement element)
		{
			return Type.Apply(element);
		}
		public static bool IsNamespace(IElement element)
		{
			return Namespace.Apply(element);
		}
		public static bool IsTypeOrNamespace(IElement element)
		{
			return TypeOrNamespace.Apply(element);
		}
		public static bool IsNonPrivateMember(IElement element)
		{
			return NonPrivateMember.Apply(element);
		}
		public static bool IsMember(IElement element)
		{
			return Member.Apply(element);
		}
	public static bool IsExternMember(IElement element)
		{
	  IMemberElement member = element as IMemberElement;
	  return member != null && member.IsExtern;
		}
		public static bool IsMethod(IElement element)
		{
			return Method.Apply(element);
		}
		public static bool IsProperty(IElement element)
		{
			return Property.Apply(element);
		}
		public static bool IsEvent(IElement element)
		{
			return Event.Apply(element);
		}
		public static bool IsConstructor(IElement element)
		{
			return Constructor.Apply(element);
		}
		public static bool IsField(IElement element)
		{
			return Field.Apply(element);
		}
		public static bool IsLocal(IElement element)
		{
			return Local.Apply(element);
		}
		public static bool IsPublicParameterLessMethod(IElement element)
		{
			return PublicParameterLessMethod.Apply(element);
		}
	}
	public class ElementRangeFilter : ElementFilterBase
	{
		SourceRange _Range;
		public ElementRangeFilter(SourceRange range)
		{
			_Range = range;
		}
		public override bool Apply(IElement element)
		{
			SourceRange range = SourceRange.Empty;
			if (element.Ranges != null && element.Ranges.Count > 0)
				range = element.Ranges[0];
			if (range.IsEmpty && element.NameRanges != null && element.NameRanges.Count > 0)
				range = element.NameRanges[0];
			return _Range.Contains(range);
		}
	}
	public class ReferenceFilter : ElementFilterBase
	{
		ElementRangeFilter _RangeFilter;
		bool _HasThisOrBaseReferences;
		public ReferenceFilter(SourceRange range)
		{
			_RangeFilter = new ElementRangeFilter(range);
		}
		public override bool Apply(IElement element)
		{
			if (_RangeFilter != null && !_RangeFilter.Apply(element))
				return false;
			if (element is IThisReferenceExpression)
				_HasThisOrBaseReferences = true;
			if (element is IBaseReferenceExpression)
				_HasThisOrBaseReferences = true;
			return DeclarationTester.IsReference(element);
		}
		public bool HasThisOrBaseReferences
		{
			get
			{
				return _HasThisOrBaseReferences;
			}
		}
	}
}
