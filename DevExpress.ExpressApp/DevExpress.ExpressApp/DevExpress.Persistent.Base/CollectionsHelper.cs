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
using System.ComponentModel;
using System.Reflection;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp;
namespace DevExpress.Persistent.Base {
	public static class CollectionsHelper {
		private static readonly int defaultCapacity;
		static CollectionsHelper() {
			ArrayList list = new ArrayList();
			list.Add(new object());
			defaultCapacity = list.Capacity;
		}
		private static IList<T> IntersectCollectionsUsingHash<T>(ICollection<T> first, ICollection<T> second) {
			ICollection<T> immediate;
			ICollection<T> toCheck;
			if(first.Count > second.Count) {
				immediate = first;
				toCheck = second;
			}
			else {
				immediate = second;
				toCheck = first;
			}
			Dictionary<T, object> hash = new Dictionary<T, object>(immediate.Count);
			foreach(T obj in immediate)
				hash.Add(obj, null);
			List<T> result = new List<T>();
			foreach(T obj in toCheck)
				if(hash.ContainsKey(obj))
					result.Add(obj);
			return result;
		}
		private static IList<T> IntersectCollectionsUsingList<T>(ICollection<T> first, ICollection<T> second) {
			List<T> result = new List<T>();
			foreach(T obj in first)
				if(second.Contains(obj))
					result.Add(obj);
			return result;
		}
		private static IList MergeListsUsingHash(ICollection first, ICollection second, int initialCapacity) {
			Hashtable hash = new Hashtable(initialCapacity);
			foreach(object obj in first)
				hash[obj] = null;
			foreach(object obj in second)
				hash[obj] = null;
			return new ArrayList(hash.Keys);
		}
		private static IList MergeListsUsingList(ICollection first, ICollection second, int initialCapacity) {
			ArrayList result = new ArrayList(initialCapacity);
			result.AddRange(first);
			foreach(object obj in second)
				if(!result.Contains(obj))
					result.Add(obj);
			return result;
		}
		private static IList<T> MergeListsUsingHash<T>(ICollection<T> first, ICollection<T> second, int initialCapacity) {
			Dictionary<T, object> hash = new Dictionary<T, object>(initialCapacity);
			foreach(T obj in first)
				hash[obj] = null;
			foreach(T obj in second)
				hash[obj] = null;
			return new List<T>(hash.Keys);
		}
		private static IList<T> MergeListsUsingList<T>(ICollection<T> first, ICollection<T> second, int initialCapacity) {
			List<T> result = new List<T>(initialCapacity);
			result.AddRange(first);
			foreach(T obj in second)
				if(!result.Contains(obj))
					result.Add(obj);
			return result;
		}
		public static IList MergeCollections(ICollection first, ICollection second) {
			int initialCapacity = Math.Max(first.Count, second.Count);
			if(initialCapacity < 70)
				return MergeListsUsingList(first, second, initialCapacity);
			else
				return MergeListsUsingHash(first, second, initialCapacity);
		}
		public static IList<T> MergeCollections<T>(ICollection<T> first, ICollection<T> second) {
			int initialCapacity = Math.Max(first.Count, second.Count);
			if(initialCapacity < 70)
				return MergeListsUsingList<T>(first, second, initialCapacity);
			else
				return MergeListsUsingHash<T>(first, second, initialCapacity);
		}
		public static IList<T> IntersectCollections<T>(ICollection<T> first, ICollection<T> second) {
			if(first.Count * second.Count > 900)
				return IntersectCollectionsUsingHash<T>(first, second);
			else
				return IntersectCollectionsUsingList<T>(first, second);
		}
		private abstract class InnerComparerBase : IComparer, ICloneable {
			private IMemberInfo sortingMemberDescriptor;
			private bool descending;
			private Dictionary<object, object> valuesCache;
			private IComparer valuesComparer;
			private object GetValue(object obj) {
				object result;
				if(!valuesCache.TryGetValue(obj, out result)) {
					result = sortingMemberDescriptor.GetValue(obj);
					valuesCache.Add(obj, result);
				}
				return result;
			}
			public InnerComparerBase(IMemberInfo sortingMemberDescriptor, IComparer valuesComparer) {
				this.sortingMemberDescriptor = sortingMemberDescriptor;
				this.valuesComparer = valuesComparer;
				valuesCache = new Dictionary<object, object>();
			}
			public int Compare(object x, object y) {
				int result = valuesComparer.Compare(GetValue(x), GetValue(y));
				if(descending)
					result = -result;
				return result;
			}
			public abstract object Clone();
			public IMemberInfo SortingMemberDescriptor {
				get { return sortingMemberDescriptor; }
			}
			public bool Descending {
				get { return descending; }
				set { descending = value; }
			}
			public IComparer ValuesComparer {
				get { return valuesComparer; }
			}
		}
		private class InnerComparer<T> : InnerComparerBase, IComparer<T> {
			public InnerComparer(IMemberInfo sortingMemberDescriptor, IComparer valuesComparer) : base(sortingMemberDescriptor, valuesComparer) { }
			int IComparer<T>.Compare(T x, T y) {
				return Compare(x, y);
			}
			public override object Clone() {
				return new InnerComparer<T>(SortingMemberDescriptor, ValuesComparer);
			}
		}
		private static Dictionary<Type, IComparer> valueComparers = new Dictionary<Type, IComparer>();
		private static Dictionary<string, InnerComparerBase> comparers = new Dictionary<string, InnerComparerBase>();
		private static InnerComparerBase CreateInnerComparer(Type itemType, IMemberInfo sortingMemberDescriptor, ListSortDirection direction) {
			string comparerKey = itemType.FullName + "\\" + sortingMemberDescriptor.Name;
			InnerComparerBase cached;
			Tracing.Tracer.LogLockedSectionEntering(typeof(CollectionsHelper), "CreateInnerComparer", comparers);
			lock(comparers) {
				Tracing.Tracer.LogLockedSectionEntered();
				if(!comparers.TryGetValue(comparerKey, out cached)) {
					IComparer valueComparer;
					Type memberType = sortingMemberDescriptor.MemberType;
					if(!valueComparers.TryGetValue(memberType, out valueComparer)) {
						valueComparer = (IComparer)typeof(Comparer<>).MakeGenericType(memberType).GetProperty("Default", BindingFlags.Static | BindingFlags.Public).GetValue(null, null);
						valueComparers.Add(memberType, valueComparer);
					}
					cached = (InnerComparerBase)Activator.CreateInstance(
								typeof(InnerComparer<>).MakeGenericType(itemType),
								sortingMemberDescriptor,
								valueComparer);
					comparers.Add(comparerKey, cached);
				}
			}
			InnerComparerBase result = (InnerComparerBase)cached.Clone();
			result.Descending = direction == ListSortDirection.Descending;
			return result;
		}
		public static bool TrySort(ICollection collection, string propertyName, ListSortDirection direction) {
			if(collection == null || string.IsNullOrEmpty(propertyName))
				return false;
			bool result = false;
			Tracing.Tracer.LogVerboseText("->CollectionsHelper.TrySort");
			Type collectionType = collection.GetType();
			Type itemType = null;
			ITypedList typedList = collection as ITypedList;
			if(typedList != null) {
				PropertyDescriptor propertyDescriptor = typedList.GetItemProperties(null).Find(propertyName, false);
				if(propertyDescriptor != null)
					itemType = propertyDescriptor.ComponentType;
			}
			if(itemType == null) {
				Type genericEnumerableImpl = ReflectionHelper.GetInterfaceImplementation(collectionType, typeof(IEnumerable<>));
				if(genericEnumerableImpl != null) {
					itemType = genericEnumerableImpl.GetGenericArguments()[0];
				}
			}
			if(itemType != null) {
				ITypeInfo itemTypeInfo = XafTypesInfo.Instance.FindTypeInfo(itemType);
				IMemberInfo sortingMemberDescriptor = itemTypeInfo.FindMember(propertyName);
				IBindingList bindingList = collection as IBindingList;
				if(bindingList != null && typedList != null) {
					bindingList.ApplySort(typedList.GetItemProperties(null).Find(sortingMemberDescriptor.BindingName, false), direction);
					result = true;
				}
				else {
					if(collectionType.IsArray) {
						Array.Sort((Array)collection, CreateInnerComparer(itemType, sortingMemberDescriptor, direction));
						result = true;
					}
					else {
						MethodInfo sortMethod = collectionType.GetMethod("Sort", new Type[] { typeof(IComparer<>).MakeGenericType(itemType) });
						if(sortMethod == null)
							sortMethod = collectionType.GetMethod("Sort", new Type[] { typeof(IComparer) });
						if(sortMethod != null) {
							sortMethod.Invoke(collection, new object[] { CreateInnerComparer(itemType, sortingMemberDescriptor, direction) });
							result = true;
						}
					}
				}
			}
			Tracing.Tracer.LogVerboseText("<-CollectionsHelper.TrySort");
			return result;
		}
		public static bool TrySort(ICollection collection, string propertyName) {
			return TrySort(collection, propertyName, ListSortDirection.Ascending);
		}
		private static int ApproximateCapacity(IList list, int startIndex, Predicate<object> condition) {
			const int margin = 100;
			int result;
			int srcCount = list.Count - startIndex;
			if(srcCount < margin) {
				result = srcCount;
			}
			else {
				int srcCount1 = srcCount - 1;
				int sampleCount = 0;
				if(condition(list[startIndex])) {
					sampleCount++;
				}
				if(condition(list[list.Count - 1])) {
					sampleCount++;
				}
				if(condition(list[startIndex + srcCount1 >> 1])) {
					sampleCount++;
				}
				result = (int)(srcCount * sampleCount / 3);
				if(result < margin) {
					result = margin;
				}
			}
			return result < defaultCapacity ? defaultCapacity : result;
		}
		public static IList ConditionalCopy(IList source, Predicate<object> condition) {
			ArrayList result = new ArrayList(ApproximateCapacity(source, 0, condition));
			for(int i = 0; i < source.Count; i++) {
				object obj = source[i];
				if(condition(obj)) {
					result.Add(obj);
					if(result.Capacity == result.Count) {
						result.Capacity += ApproximateCapacity(source, i + 1, condition);
					}
				}
			}
			return result;
		}
	}
}
