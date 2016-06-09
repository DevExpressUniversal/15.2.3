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
using System.Text;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler;
using System.Collections.ObjectModel;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public interface ISupportCopyFrom<T> {
		void CopyFrom(T source);
	}
	public static class CollectionCopyHelper {
		public static void CopyDateFormats(ObservableStringCollection destination, System.Collections.Specialized.StringCollection source) {
			destination.Clear();
			int count = source.Count;
			for(int i = 0; i < count; i++)
				destination.Add(source[i]);
		}
		class ListAccessor<T> {
			IList<T> source;
			public ListAccessor(IList<T> source) {
				this.source = source;
			}
			public T GetItem(int index) {
				return source[index];
			}
		}
		public class CellContainerAccessor {
			ICellContainer source;
			public CellContainerAccessor(ICellContainer source) {
				this.source = source;
			}
			public ITimeCell GetItem(int index) {
				return source[index];
			}
		}
		public class SingleResourceViewInfoAccessor {
			IList<SingleResourceViewInfo> source;
			public SingleResourceViewInfoAccessor(IList<SingleResourceViewInfo> source) {
				this.source = source;
			}
			public ICellContainer GetItem(int index) {
				XtraSchedulerDebug.Assert(((DayBasedSingleResourceViewInfo)source[index]).VerticalCellContainers.Count == 1);
				return ((DayBasedSingleResourceViewInfo)source[index]).VerticalCellContainers[0];
			}
		}
		public class AllDayAreaResourceViewInfoAccessor {
			IList<SingleResourceViewInfo> source;
			public AllDayAreaResourceViewInfoAccessor(IList<SingleResourceViewInfo> source) {
				this.source = source;
			}
			public ICellContainer GetItem(int index) {
				return ((DayBasedSingleResourceViewInfo)source[index]).HorizontalCellContainer;
			}
		}
		public class SingleResourceViewInfoResourceHeaderAccessor {
			IList<SingleResourceViewInfo> source;
			public SingleResourceViewInfoResourceHeaderAccessor(IList<SingleResourceViewInfo> source) {
				this.source = source;
			}
			public ResourceHeaderBase GetItem(int index) {
				return source[index].ResourceHeader;
			}
		}
		static T CreateNewItem<T>() where T : new() {
			return new T();
		}
		public static void Copy<T, U>(ObservableCollection<T> destination, IList<U> source) where T : ISupportCopyFrom<U>, new() {
			Copy(destination, source, CreateNewItem<T>);
		}
		public delegate T CreaeteItemDelegate<T>();
		public delegate T GetItemDelegate<T>(int index);
		public delegate void CopyItemDelegate<T, U>(ObservableCollection<T> destination, int index, U item);
		public static void Copy<T, U>(ObservableCollection<T> destination, IList<U> source, CreaeteItemDelegate<T> createItemDelegate) where T : ISupportCopyFrom<U> {
			ListAccessor<U> accessor = new ListAccessor<U>(source);
			Copy(destination, accessor.GetItem, source.Count, createItemDelegate);
		}
		public static void Copy<T>(ObservableCollection<T> destination, ICellContainer source, CreaeteItemDelegate<T> createItemDelegate) where T : ISupportCopyFrom<ITimeCell> {
			CellContainerAccessor accessor = new CellContainerAccessor(source);
			Copy(destination, accessor.GetItem, source.CellCount, createItemDelegate);
		}
		public static void Copy(VisualSimpleResourceIntervalCollection destination, IList<SingleResourceViewInfo> source, CreaeteItemDelegate<VisualSimpleResourceInterval> createItemDelegate) {
			SingleResourceViewInfoAccessor accessor = new SingleResourceViewInfoAccessor(source);
			Copy(destination, accessor.GetItem, source.Count, createItemDelegate);
		}
		public static void Copy(VisualResourceHeaderBaseContentCollection destination, IList<SingleResourceViewInfo> source, CreaeteItemDelegate<VisualResourceHeaderBaseContent> createItemDelegate) {
			SingleResourceViewInfoResourceHeaderAccessor accessor = new SingleResourceViewInfoResourceHeaderAccessor(source);
			Copy(destination, accessor.GetItem, source.Count, createItemDelegate);
		}
		public static void MemberwiseCopy<T, U>(ObservableCollection<T> destination, int index, U sourceItem) where T : ISupportCopyFrom<U> {
			destination[index].CopyFrom(sourceItem);
		}
		public static VisualResource GetResource(VisualResource source) {
			return source;
		}
		public static void AssignAllDayAreaContainer(VisualResourceAllDayAreaCollection destiantion, int index, VisualResource source) {
			VisualResourceAllDayArea allDayArea = ((VisualDayViewResource)source).ResourceAllDayArea;
			if(!Object.ReferenceEquals(destiantion[index], allDayArea))
				destiantion[index] = allDayArea;
		}
		public static void Copy(VisualResourceAllDayAreaCollection destination, IList<SingleResourceViewInfo> source, CreaeteItemDelegate<VisualResourceAllDayArea> createItemDelegate) {
			AllDayAreaResourceViewInfoAccessor accessor = new AllDayAreaResourceViewInfoAccessor(source);
			List<ICellContainer> allDayAreaContainers = ObtainAllDayAreaContainers(accessor, source.Count);
			Copy(destination, allDayAreaContainers, createItemDelegate);
		}
		public static void Copy(VisualResourceAllDayAreaCollection destination, VisualResourcesCollection source) {
			VisualResourceAllDayAreaCollection allDayAreas = CollectionCopyHelper.ObtainAllDayAreaCollection(source);
			CollectionCopyHelper.Copy(destination, allDayAreas);
		}
		static VisualResourceAllDayAreaCollection ObtainAllDayAreaCollection(VisualResourcesCollection source) {
			VisualResourceAllDayAreaCollection result = new VisualResourceAllDayAreaCollection();
			int count = source.Count;
			for(int i = 0; i < count; i++) {
				VisualResourceAllDayArea allDayArea = ((VisualDayViewResource)source[i]).ResourceAllDayArea;
				if(allDayArea != null)
					result.Add(allDayArea);
			}
			return result;
		}
		static List<ICellContainer> ObtainAllDayAreaContainers(AllDayAreaResourceViewInfoAccessor accessor, int count) {
			List<ICellContainer> result = new List<ICellContainer>();
			for(int i = 0; i < count; i++) {
				ICellContainer allDayAreaContainer = accessor.GetItem(i);
				if(allDayAreaContainer != null)
					result.Add(allDayAreaContainer);
			}
			return result;
		}
		static void Copy(VisualResourceAllDayAreaCollection destination, VisualResourceAllDayAreaCollection source) {
			int sourceCount = source.Count;
			if(sourceCount < destination.Count) {
				for(int i = destination.Count - 1; i >= sourceCount; i--)
					destination.RemoveAt(i);
			}
			for(int i = 0; i < destination.Count; i++) {
				if(!Object.ReferenceEquals(destination[i], source[i]))
					destination[i] = source[i];
			}
			if(sourceCount > destination.Count) {
				for(int i = destination.Count; i < sourceCount; i++)
					destination.Add(source[i]);
			}
		}
		public static void Copy<T, U>(ObservableCollection<T> destination, GetItemDelegate<U> getSourceItem, int sourceCount, CreaeteItemDelegate<T> createItemDelegate) where T : ISupportCopyFrom<U> {
			if(sourceCount < destination.Count) {
				for(int i = destination.Count - 1; i >= sourceCount; i--)
					destination.RemoveAt(i);
			}
			for(int i = 0; i < destination.Count; i++) {
				destination[i].CopyFrom(getSourceItem(i));
			}
			if(sourceCount > destination.Count) {
				for(int i = destination.Count; i < sourceCount; i++) {
					T newItem = createItemDelegate();
					newItem.CopyFrom(getSourceItem(i));
					destination.Add(newItem);
				}
			}
		}
	}
}
