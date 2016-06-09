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
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Native {
	#region IPredicate<T>
	public interface IPredicate<T> {
		bool Calculate(T obj);
	}
	#endregion
	public class NotPredicate<T> : IPredicate<T> {
		readonly IPredicate<T> predicate;
		public NotPredicate(IPredicate<T> predicate) {
			Guard.ArgumentNotNull(predicate, "predicate");
			this.predicate = predicate;
		}
		public bool Calculate(T obj) {
			return !predicate.Calculate(obj);
		}
	}
	#region SchedulerPredicate
	public abstract class SchedulerPredicate<T> : IPredicate<T> {
		public abstract bool Calculate(T obj);
	}
	#endregion
	#region SchedulerEmptyPredicate
	public class SchedulerEmptyPredicate<T> : SchedulerPredicate<T> {
		public override bool Calculate(T obj) {
			return true;
		}
	}
	#endregion
	public class SchedulerPredicateList<T> : List<IPredicate<T>> {
	}
	#region SchedulerCompositePredicate<T>
	public class SchedulerCompositePredicate<T> : SchedulerPredicate<T> {
		readonly SchedulerPredicateList<T> items;
		public SchedulerCompositePredicate() {
			this.items = new SchedulerPredicateList<T>();
		}
		public SchedulerPredicateList<T> Items { get { return items; } }
		#region Calculate
		public override bool Calculate(T obj) {
			int count = Items.Count;
			for (int i = 0; i < count; i++) {
				IPredicate<T> item = items[i];
				if (!item.Calculate(obj))
					return false;
			}
			return true;
		}
		#endregion
	}
	#endregion
	#region FilterObjectViaStorageEventPredicate (abstract class)
	public abstract class FilterObjectViaStorageEventPredicate<T> : SchedulerPredicate<T> {
		readonly ISchedulerStorageBase storage;
		protected FilterObjectViaStorageEventPredicate(ISchedulerStorageBase storage) {
			if (storage == null)
				Exceptions.ThrowArgumentNullException("storage");
			this.storage = storage;
		}
		protected internal ISchedulerStorageBase Storage { get { return storage; } }
	}
	#endregion
	#region FilterObjectViaFilterCriteriaPredicate
	public class FilterObjectViaFilterCriteriaPredicate<T> : SchedulerPredicate<T> where T : IPersistentObject {
		ExpressionEvaluator evaluator;
		#region Ctors
		public FilterObjectViaFilterCriteriaPredicate(IPersistentObjectStorage<T> objectStorage, string filterString, bool caseSensitive)
		{
			if (objectStorage == null)
				Exceptions.ThrowArgumentNullException("objectStorage");
			Initialize(objectStorage, filterString, caseSensitive);
		}
		public FilterObjectViaFilterCriteriaPredicate(IPersistentObjectStorage<T> objectStorage) {
			if (objectStorage == null)
				Exceptions.ThrowArgumentNullException("objectStorage");
			Initialize(objectStorage, objectStorage.Filter, true);
		}
		#endregion
		internal ExpressionEvaluator Evaluator { get { return evaluator; } }
		protected internal void Initialize(IPersistentObjectStorage<T> objectStorage, string filterString, bool caseSensitive) {
			if (!String.IsNullOrEmpty(filterString)) {
				CriteriaOperator filter = CriteriaOperator.Parse(filterString);
				this.evaluator = new ExpressionEvaluator(new MappingsEvaluatorContextDescriptor(((IInternalPersistentObjectStorage<T>)objectStorage).ActualMappings), filter, caseSensitive);				
			}
		}
		#region Calculate
		public override bool Calculate(T obj) {
			if (Evaluator == null)
				return true;
			return Evaluator.Fit(obj);
		}
		#endregion
	}
	#endregion
	#region ProcessorBase (abstract class)
	public abstract class ProcessorBase<T> {
		readonly NotificationCollection<T> destinationCollection;
		protected ProcessorBase() {
			this.destinationCollection = CreateDestinationCollection();
			DestinationCollection.UniquenessProviderType = DXCollectionUniquenessProviderType.None;
		}
		public NotificationCollection<T> DestinationCollection { get { return destinationCollection; } }
		public virtual void Process(NotificationCollection<T> sourceCollection) {
			InitializeProcess();
			if (sourceCollection == null || sourceCollection.Count == 0)
				return;
			ProcessCore(sourceCollection);
		}
		protected internal virtual void InitializeProcess() {
			DestinationCollection.Clear();
		}
		protected internal abstract void ProcessCore(NotificationCollection<T> sourceCollection);
		protected internal abstract NotificationCollection<T> CreateDestinationCollection();
	}
	#endregion
	#region SimpleProcessorBase (abstract class)
	public abstract class SimpleProcessorBase<T> : ProcessorBase<T> {
		readonly IPredicate<T> predicate;
		protected SimpleProcessorBase(IPredicate<T> predicate) {
			if (predicate == null)
				Exceptions.ThrowArgumentNullException("predicate");
			this.predicate = predicate;
		}
		public IPredicate<T> Predicate { get { return predicate; } }
		protected internal override void ProcessCore(NotificationCollection<T> sourceCollection) {
			int count = sourceCollection.Count;
			for (int i = 0; i < count; i++) {
				T item = sourceCollection[i];
				if (predicate.Calculate(item))
					DestinationCollection.Add(item);
			}
		}
	}
	#endregion
}
