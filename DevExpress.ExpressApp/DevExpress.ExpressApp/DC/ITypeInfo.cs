#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace DevExpress.ExpressApp.DC {
	public interface ITypeInfo : IBaseInfo {
		IMemberInfo FindMember(String name);
		IMemberInfo CreateMember(String memberName, Type memberType);
		IEnumerable<ITypeInfo> GetDependentTypes(Predicate<ITypeInfo> filter);
		IEnumerable<ITypeInfo> GetRequiredTypes(Predicate<ITypeInfo> filter);
		Boolean IsAssignableFrom(ITypeInfo from);
		Boolean Implements<InterfaceType>();
		String FullName { get; }
		String Name { get; }
		Type Type { get; }
		ITypeInfo Base { get; }
		IEnumerable<IMemberInfo> OwnMembers { get; }
		IEnumerable<IMemberInfo> Members { get; }
		Boolean IsVisible { get; }
		Boolean IsPersistent { get; }
		Boolean IsInterface { get; }
		Boolean IsDomainComponent { get; }
		Boolean IsValueType { get; }
		Boolean IsListType { get; }
		Boolean IsAbstract { get; }
		Boolean IsNullable { get; }
		IMemberInfo KeyMember { get; }
		ReadOnlyCollection<IMemberInfo> KeyMembers { get; }
		ITypeInfo UnderlyingTypeInfo { get; }
		Boolean HasPublicConstructor { get; }
		Boolean HasDescendants { get; }
		IAssemblyInfo AssemblyInfo { get; }
		IMemberInfo ReferenceToOwner { get; }
		IMemberInfo DeclaredDefaultMember { get; }
		IMemberInfo DefaultMember { get; }
		IEnumerable<ITypeInfo> ImplementedInterfaces { get; }
		IEnumerable<ITypeInfo> Implementors { get; }
		IEnumerable<ITypeInfo> Descendants { get; }
		Object CreateInstance(params Object[] args);
	}
}
