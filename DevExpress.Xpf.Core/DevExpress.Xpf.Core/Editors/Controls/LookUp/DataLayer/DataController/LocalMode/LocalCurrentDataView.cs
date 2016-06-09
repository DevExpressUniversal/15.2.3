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
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Editors.Helpers {
	public abstract class LocalCurrentDataView : CurrentDataView {
		DefaultDataViewAsListWrapper Wrapper { get; set; }
		new DefaultDataView ListSource {
			get { return base.ListSource as DefaultDataView; }
		}
		protected LocalCurrentDataView(object view, object handle, string valueMember, string displayMember)
			: base(view, handle, valueMember, displayMember) {
		}
		public override bool ProcessAddItem(int index) {
			DataProxy proxy = DataAccessor.CreateProxy(Wrapper[index], -1);
			View.Add(index, proxy);
			ItemsCache.UpdateItemOnAdding(index);
			return true;
		}
		public override bool ProcessChangeItem(int index) {
			DataProxy proxy = DataAccessor.CreateProxy(ListSource.GetItemByIndex(index), -1);
			View.Replace(index, proxy);
			ItemsCache.UpdateItem(index);
			return true;
		}
		public override bool ProcessDeleteItem(int index) {
			View.Remove(index);
			ItemsCache.UpdateItemOnDeleting(index);
			return true;
		}
		public override bool ProcessMoveItem(int oldIndex, int newIndex) {
			DataProxy proxy = View[oldIndex];
			View.Remove(oldIndex);
			View.Add(newIndex, proxy);
			ItemsCache.UpdateItemOnMoving(oldIndex, newIndex);
			return true;
		}
		public override bool ProcessReset() {
			ItemsCache.Reset();
			Initialize();
			return true;
		}
		protected override void InitializeView(object source) {
			Wrapper = new DefaultDataViewAsListWrapper((DefaultDataView)source);
			SetView(new LocalDataProxyViewCache(DataAccessor, Wrapper.Cast<object>().Select(x => DataAccessor.CreateProxy(x, -1))));
		}
		protected override object CreateVisibleListWrapper() {
			Type sourceGenericType = GetSourceGenericType();
			if (sourceGenericType == null)
				sourceGenericType = typeof(object);
			Type type = typeof(LocalVisibleListWrapper<>).MakeGenericType(new[] { sourceGenericType });
			return Activator.CreateInstance(type, new object[] { this }) as LocalVisibleListWrapper;
		}
		Type GetSourceGenericType() {
			Type sourceType = null;
			var listSource = ListSource.ListSource;
			BindingListAdapter adapter = listSource as BindingListAdapter;
			if (adapter != null && adapter.OriginalDataSource != null)
				sourceType = adapter.OriginalDataSource.GetType();
			else if (listSource != null)
				sourceType = listSource.GetType();
			return sourceType != null ? FindGenericType(sourceType) : null;
		}
		public Type FindGenericType(Type sourceType) {
			IEnumerable<Type> typeHierarchy = GetTypeHierarchy(sourceType);
			foreach (Type type in typeHierarchy) {
				Type[] interfaces = type.GetInterfaces();
				Type genericCollectionInterfaceType = GetCollectionLikeGenericTypeFromInterfaces(interfaces);
				if (genericCollectionInterfaceType != null)
					return genericCollectionInterfaceType;
			}
			if (!sourceType.IsGenericType)
				return null;
			Type[] genericTypes = sourceType.GetGenericArguments();
			return genericTypes.Length == 1 ? genericTypes[0] : null;
		}
		Type GetCollectionLikeGenericTypeFromInterfaces(IEnumerable<Type> interfaces) {
			foreach (Type interf in interfaces) {
				if (!interf.IsGenericType)
					continue;
				Type[] arguments = interf.GetGenericArguments();
				if (arguments.Length > 1)
					continue;
				if (typeof(IEnumerable<>).MakeGenericType(arguments) == interf)
					return arguments[0];
			}
			return null;
		}
		IEnumerable<Type> GetTypeHierarchy(Type type) {
			IList<Type> hierarchy = new List<Type>();
			Type currentType = type;
			while (currentType.BaseType != null) {
				hierarchy.Add(currentType);
				currentType = currentType.BaseType;
			}
			return hierarchy.Reverse();
		}
	}
}
