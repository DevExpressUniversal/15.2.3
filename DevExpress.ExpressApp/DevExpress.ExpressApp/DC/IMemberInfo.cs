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
namespace DevExpress.ExpressApp.DC {
	public interface IMemberInfo : IBaseInfo {
		void SetValue(Object obj, Object value);
		Object GetValue(Object obj);
		object GetOwnerInstance(object obj);
		IList<IMemberInfo> GetPath();
		string SerializeValue(object obj);
		object DeserializeValue(string value);
		void AddAttribute(Attribute attribute, Boolean skipRefresh);
		String Name { get; }
		ITypeInfo Owner { get; }
		Type MemberType { get; }
		IMemberInfo AssociatedMemberInfo { get; }
		Boolean IsPublic { get; }
		Boolean IsProperty { get; }
		Boolean IsCustom { get; }
		Boolean IsVisible { get; }
		Boolean IsInStruct { get; }
		Boolean IsReadOnly { get; }
		Boolean IsKey { get; }
		Boolean IsAutoGenerate { get; }
		Boolean IsDelayed { get; }
		Boolean IsAliased { get; }
		Boolean IsService { get; }
		Boolean IsPersistent { get; }
		Boolean IsAggregated { get; }
		Boolean IsAssociation { get; }
		Boolean IsManyToMany { get; }
		Boolean IsReferenceToOwner { get; }
		Boolean IsList { get; }
		ITypeInfo ListElementTypeInfo { get; }
		Type ListElementType { get; }
		ITypeInfo MemberTypeInfo { get;}
		String DisplayName { get; }
		Int32 Size { get; }
		Int32 ValueMaxLength { get; }
		IMemberInfo LastMember { get; }
		String BindingName { get; }
		String Expression { get; }
	}
}
