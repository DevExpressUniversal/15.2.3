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
using System.ComponentModel;
namespace DevExpress.ExpressApp.DC {
	public delegate void EnumMembersHandler(Object memberSource, String memberName);
	public interface ITypeInfoSource {
		Boolean TypeIsKnown(Type type);
		void InitTypeInfo(TypeInfo typeInfo);
		void InitAttributes(TypeInfo typeInfo);
		void EnumMembers(TypeInfo info, EnumMembersHandler handler);
		void InitMemberInfo(Object member, XafMemberInfo memberInfo);
		void RefreshMemberInfo(TypeInfo typeInfo, XafMemberInfo memberInfo);
		Boolean RegisterNewMember(ITypeInfo typeInfo, XafMemberInfo memberInfo);
		void UpdateMember(XafMemberInfo memberInfo);
		Boolean AddAttribute(IBaseInfo info, Attribute attribute);
		Boolean RemoveAttribute(IBaseInfo info, Type attributeType);
		Object GetValue(IMemberInfo memberInfo, Object obj);
		void SetValue(IMemberInfo memberInfo, Object obj, Object value);
	}
	#region Obsolete 15.2
	[Obsolete("Use the 'ITypeInfoSource.RemoveAttribute' method instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface ISupportRemoveAttribute {
		Boolean RemoveAttribute(IBaseInfo info, Type attributeType);
	}
	#endregion
}
