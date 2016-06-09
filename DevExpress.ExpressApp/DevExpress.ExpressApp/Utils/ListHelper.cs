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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp.Utils {
	public static class ListHelper {
		public static IList GetList(Object collection) {
			if(collection is IList) {
				return (IList)collection;
			}
			else if(collection is IListSource) {
				return ((IListSource)collection).GetList();
			}
			else {
				return null;
			}
		}
		public static IBindingList GetBindingList(Object collection) {
			if(collection is IBindingList) {
				return (IBindingList)collection;
			}
			else if(collection is IListSource) {
				return ((IListSource)collection).GetList() as IBindingList;
			}
			else {
				return null;
			}
		}
		public static Boolean IsGenericListSupported(Object list) {
			if (list != null) {
				foreach(Type interfaceType in list.GetType().GetInterfaces()) {
					if(interfaceType.IsGenericType && (interfaceType.GetGenericTypeDefinition() == typeof(IList<>))) {
						return true;
					}
				}
			}
			return false;
		}
		public static IList CreateListWrapperForGenericList(Object list) {
			if(list != null) {
				foreach(Type interfaceType in list.GetType().GetInterfaces()) {
					if(interfaceType.IsGenericType && (interfaceType.GetGenericTypeDefinition() == typeof(IList<>))) {
						Type wrapperType = typeof(Collection<>).MakeGenericType(interfaceType.GetGenericArguments()[0]);
						return (IList)TypeHelper.CreateInstance(wrapperType, list);
					}
				}
			}
			return null;
		}
	}
}
