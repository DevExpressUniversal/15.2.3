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

using DevExpress.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.Diagram.Core {
	public static class PropertyActions {
		class PropertyState<T> {
			internal readonly IItemFinder<T> Finder;
			internal readonly PropertyDescriptor Property;
			internal readonly Func<T, IItemFinder<T>> GetFinder;
			public PropertyState(IItemFinder<T> finder, PropertyDescriptor property, Func<T, IItemFinder<T>> getFinder) {
				this.Finder = finder;
				this.Property = property;
				this.GetFinder = getFinder;
			}
		}
		class SetPropertyState<T> : PropertyState<T> {
			internal readonly object Value;
			public SetPropertyState(object value, IItemFinder<T> finder, PropertyDescriptor property, Func<T, IItemFinder<T>> getFinder)
				: base(finder, property, getFinder) {
				this.Value = value;
			}
		}
		public static void ResetProperty<T>(this Transaction transaction, T item, PropertyDescriptor property, Func<T, IItemFinder<T>> getFinder) {
			transaction.Execute(new PropertyState<T>(getFinder(item), property, getFinder), ResetProperty, SetProperty);
		}
		public static void SetProperty<T>(this Transaction transaction, object value, T item, PropertyDescriptor property, Func<T, IItemFinder<T>> getFinder) {
			transaction.Execute(new SetPropertyState<T>(value, getFinder(item), property, getFinder), SetProperty, SetProperty, (x1, x2) => Equals(x1.Finder, x2.Finder) && Equals(x1.Property, x2.Property) ? x1 : null);
		}
		public static void ResetMultipleItemsPropertyValues<TRoot, T>(this Transaction transaction, IEnumerable<TRoot> items, PropertyDescriptor property, Func<TRoot, IItemFinder<TRoot>> getRootFinder, Func<TRoot, T> getComponent) {
			items.ForEach(item => {
				transaction.ResetProperty(getComponent(item), property, CreateCompositeFinder(getRootFinder, getComponent, item));
			});
		}
		public static void SetMultipleItemsPropertyValues<TRoot, T>(this Transaction transaction, IEnumerable<TRoot> items, PropertyDescriptor property, IEnumerable<object> values, Func<TRoot, IItemFinder<TRoot>> getRootFinder, Func<TRoot, T> getComponent) {
			items.ForEach(values, (item, value) => {
				if(!object.Equals(property.GetValue(getComponent(item)), value))
					transaction.SetProperty(value, getComponent(item), property, CreateCompositeFinder(getRootFinder, getComponent, item));
			});
		}
		static Func<T, IItemFinder<T>> CreateCompositeFinder<TRoot, T>(Func<TRoot, IItemFinder<TRoot>> getRootFinder, Func<TRoot, T> getComponent, TRoot item) {
			var finder = getRootFinder(item);
			return x => new CompositeFinder<TRoot, T>(finder, getComponent);
		}
		static SetPropertyState<T> ResetProperty<T>(PropertyState<T> x) {
			var foundItem = x.Finder.FindItem();
			if(!x.Property.CanResetValue(foundItem))
				throw new InvalidOperationException();
			var value = x.Property.GetValue(foundItem);
			x.Property.ResetValue(foundItem);
			return new SetPropertyState<T>(value, x.GetFinder(foundItem), x.Property, x.GetFinder);
		}
		static SetPropertyState<T> SetProperty<T>(SetPropertyState<T> x) {
			var foundItem = x.Finder.FindItem();
			var oldValue = x.Property.GetValue(foundItem);
			x.Property.SetValue(foundItem, x.Value);
			return new SetPropertyState<T>(oldValue, x.GetFinder(foundItem), x.Property, x.GetFinder);
		}
	}
	public static class MergeTokenActions {
		public static void AddMergeToken<T>(this Transaction transaction, T token) {
			transaction.Execute(new { Token = token }, x => x, x => x, (x, y) => Equals(x.Token, y.Token) ? x : null);
		}
	}
	public interface IItemFinder<T> {
		T FindItem();
	}
	public sealed class FakeFinder<T> : EquatableObject<FakeFinder<T>>, IItemFinder<T> {
		readonly T item;
		public FakeFinder(T item) {
			this.item = item;
		}
		T IItemFinder<T>.FindItem() {
			return item;
		}
		protected override bool Equals(FakeFinder<T> other) {
			return object.Equals(item, other.item);
		}
	}
	public sealed class CompositeFinder<TItem, T> : EquatableObject<CompositeFinder<TItem, T>>, IItemFinder<T> {
		readonly IItemFinder<TItem> finder;
		readonly Func<TItem, T> getRealItem;
		public CompositeFinder(IItemFinder<TItem> finder, Func<TItem, T> getRealItem) {
			this.finder = finder;
			this.getRealItem = getRealItem;
		}
		T IItemFinder<T>.FindItem() {
			return getRealItem(finder.FindItem());
		}
		protected override bool Equals(CompositeFinder<TItem, T> other) {
			return Equals(finder, other.finder) && Equals(getRealItem, other.getRealItem);
		}
	}
}
