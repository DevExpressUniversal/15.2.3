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
	#region Obsoleted...
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Use ElementFilters instead.")]
	public sealed class DefaultElementFilters
	{
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
		public static TypeDeclarationFilter Type
		{
			get { return ElementFilters.Type; }
		}
		public static NamespaceFilter Namespace
		{
			get { return ElementFilters.Namespace; }
		}
		public static TypeOrNamespaceFilter TypeOrNamespace
		{
			get { return ElementFilters.TypeOrNamespace; }
		}
		public static NonPrivateMemberFilter NonPrivateMember
		{
			get { return ElementFilters.NonPrivateMember; }
		}
		public static MemberFilter Member
		{
			get { return ElementFilters.Member; }
		}
		public static MethodFilter Method
		{
			get { return ElementFilters.Method; }
		}
		public static PropertyFilter Property
		{
			get { return ElementFilters.Property; }
		}
		public static EventFilter Event
		{
			get { return ElementFilters.Event; }
		}
		public static ConstructorFilter Constructor
		{
			get { return ElementFilters.Constructor; }
		}
	}
	#endregion
}
