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
using System.Linq;
using System.Reflection;
using DevExpress.DataAccess.Native.ObjectBinding;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Presenters;
namespace DevExpress.DataAccess.Wizard.Native {
	public static class ObjectBindingPagesRouter {
		delegate bool Condition(Type objType, ObjectMember member, ConstructorInfo ctor, OperationMode mode);
		struct PageInfo {
			public readonly Type PageType;
			public readonly Condition Condition;
			public PageInfo(Type pageType, Condition condition) {
				PageType = pageType;
				Condition = condition;
			}
		}
		public static Type AfterChooseObjectAssemblyPage<TModel>() where TModel : IObjectDataSourceModel {
			return GetNextPage<TModel>(typeof(ChooseObjectAssemblyPage<>), null, null, null, 0);
		}
		public static Type AfterChooseObjectTypePage<TModel>(Type objType, OperationMode mode) where TModel : IObjectDataSourceModel {
			return GetNextPage<TModel>(typeof(ChooseObjectTypePage<>), objType, null, null, mode);
		}
		public static Type AfterChooseObjectMemberPage<TModel>(Type objType, ObjectMember member, ConstructorInfo ctor, OperationMode mode) where TModel : IObjectDataSourceModel {
			return GetNextPage<TModel>(typeof(ChooseObjectMemberPage<>), objType, member, ctor, mode);
		}
		public static Type AfterObjectMemberParametersPage<TModel>(Type objType, ObjectMember member, ConstructorInfo ctor, OperationMode mode) where TModel : IObjectDataSourceModel {
			return GetNextPage<TModel>(typeof(ObjectMemberParametersPage<>), objType, member, ctor, mode);
		}
		public static Type AfterChooseObjectBindingModePage<TModel>(Type objType, ConstructorInfo ctor, OperationMode mode) where TModel : IObjectDataSourceModel {
			return GetNextPage<TModel>(typeof(ChooseObjectBindingModePage<>), objType, null, ctor, mode);
		}
		public static Type AfterChooseObjectConstructorPage<TModel>(ConstructorInfo ctor, OperationMode mode) where TModel : IObjectDataSourceModel {
			return GetNextPage<TModel>(typeof(ChooseObjectConstructorPage<>), null, null, ctor, mode);
		}
		static Type GetNextPage<TModel>(Type pageType, Type objType, ObjectMember member, ConstructorInfo ctor, OperationMode mode) where TModel : IObjectDataSourceModel {
			Type pageTypeDef =
				pages.SkipWhile(info => info.PageType != pageType)
					.Skip(1)
					.FirstOrDefault(info => info.Condition(objType, member, ctor, mode))
					.PageType;
			return pageTypeDef == null ? null :
				pageTypeDef.MakeGenericType(typeof(TModel));
		}
		static readonly PageInfo[] pages = {
			new PageInfo(typeof(ChooseObjectAssemblyPage<>), Always),
			new PageInfo(typeof(ChooseObjectTypePage<>), Always),
			new PageInfo(typeof(ChooseObjectMemberPage<>), TypeHasBindableMembers),
			new PageInfo(typeof(ObjectMemberParametersPage<>), MemberHasParams),
			new PageInfo(typeof(ChooseObjectBindingModePage<>), AllowedAndTypeHasCtors), 
			new PageInfo(typeof(ChooseObjectConstructorPage<>), AllowedAndMultipleCtors),
			new PageInfo(typeof(ObjectConstructorParametersPage<>), AllowedAndCtorHasParams)
		};
		#region Conditions
		static bool Always(Type objType, ObjectMember member, ConstructorInfo ctor, OperationMode mode) { return true; }
		static bool TypeHasBindableMembers(Type objType, ObjectMember member, ConstructorInfo ctor, OperationMode mode) { return TypeHasBindableMembers(objType); }
		static bool MemberHasParams(Type objType, ObjectMember member, ConstructorInfo ctor, OperationMode mode) { return MemberHasParams(member); }
		static bool AllowedAndTypeHasCtors(Type objType, ObjectMember member, ConstructorInfo ctor, OperationMode mode) { return ChooseObjectBindingModeAllowed(mode) && TypeHasCtors(objType, member, mode); }
		static bool AllowedAndMultipleCtors(Type objType, ObjectMember member, ConstructorInfo ctor, OperationMode mode) { return ChooseObjectConstructorAllowed(mode) && MultipleCtors(objType, ctor); }
		static bool AllowedAndCtorHasParams(Type objType, ObjectMember member, ConstructorInfo ctor, OperationMode mode) { return ChooseObjectConstructorAllowed(mode) && CtorHasParams(ctor); }
		#endregion
		public static bool TypeHasBindableMembers(Type objType) {
			try {
				object instance = ObjectDataSourceFillHelper.CreateInstanceForSchema(objType);
				return ObjectDataSourceFillHelper.GetItemMembers(instance).Any();
			}
			catch {
				return false;
			}
		}
		public static bool MemberHasParams(ObjectMember member) { return member != null && member.HasParams; }
		public static bool ChooseObjectBindingModeAllowed(OperationMode mode) {
			return (mode & OperationMode.Both) == OperationMode.Both;
		}
		public static bool TypeHasCtors(Type objType, ObjectMember member, OperationMode mode) {
			if(objType.IsAbstract && objType.IsSealed || (member != null && member.IsStatic))
				return false;
			ConstructorInfo[] constructorInfos = objType.GetConstructors();
			if(constructorInfos.Length > 0)
				return true;
			return false;
		}
		public static bool ChooseObjectConstructorAllowed(OperationMode mode) {
			return (mode & OperationMode.DataOnly) == OperationMode.DataOnly;
		}
		public static bool MultipleCtors(Type objType, ConstructorInfo ctor) {
			return ctor != null && objType.GetConstructors().Length > 1;
		}
		public static bool CtorHasParams(ConstructorInfo ctor) {
			return ctor != null && ctor.GetParameters().Length > 0;
		}
	}
}
