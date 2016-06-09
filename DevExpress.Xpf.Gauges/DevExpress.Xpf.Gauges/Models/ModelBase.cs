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
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public abstract class GaugeModelBase : GaugeElement, IWeakEventListener, INamedElement {
		public static readonly DependencyProperty InnerPaddingProperty = DependencyPropertyManager.Register("InnerPadding",
			typeof(Thickness), typeof(GaugeModelBase), new PropertyMetadata(new Thickness(0)));
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Thickness InnerPadding {
			get { return (Thickness)GetValue(InnerPaddingProperty); }
			set { SetValue(InnerPaddingProperty, value); }
		}
		protected static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			GaugeModelBase model = d as GaugeModelBase;
			if (model != null && model.ModelHolder != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as INotifyPropertyChanged, e.NewValue as INotifyPropertyChanged, d as IWeakEventListener);
				model.ModelHolder.UpdateModel();
			}
		}
		protected static void CollectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			GaugeModelBase model = d as GaugeModelBase;
			if (model != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as INotifyPropertyChanged, e.NewValue as INotifyPropertyChanged, d as IWeakEventListener);
				if (model.ModelHolder != null)
					model.ModelHolder.UpdateModel();
			}
		}
		IModelSupported ModelHolder { get { return Owner as IModelSupported; } }
		public abstract string ModelName { get; }
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return PerformWeakEvent(managerType, sender, e);
		}
		#endregion
		#region INamedElement implementation
		string INamedElement.Name { get { return ModelName; } }
		#endregion
		bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager)) {
				if (ModelHolder != null)
					ModelHolder.UpdateModel();
				success = true;
			}
			return success;
		}
		protected ModelBase GetModel(DependencyProperty dp, int index) {
			IList collection = GetValue(dp) as IList;
			if (collection != null && collection.Count > 0) {
				if (index < collection.Count)
					return collection[index] as ModelBase;
				else
					return collection[collection.Count - 1] as ModelBase;
			}
			return null;
		}
	}
	public abstract class ModelBase : GaugeDependencyObject {
	}
	public abstract class ModelCollection<T> : GaugeDependencyObjectCollection<T>, INotifyPropertyChanged, IWeakEventListener where T : ModelBase {
		public event PropertyChangedEventHandler PropertyChanged;
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return PerformWeakEvent(managerType, sender, e);
		}
		#endregion
		bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager)) {
				if (sender is ModelBase)
					NotifyPropertyChanged(sender, e);
				success = true;
			}
			return success;
		}
		protected override void RemoveChildren(IList children) {
			base.RemoveChildren(children);
			foreach (object child in children)
				if (child is INotifyPropertyChanged)
					PropertyChangedWeakEventManager.RemoveListener(child as INotifyPropertyChanged, this);
		}
		protected override void AddChildren(IList children) {
			base.AddChildren(children);
			foreach (object child in children)
				if (child is INotifyPropertyChanged)
					PropertyChangedWeakEventManager.AddListener(child as INotifyPropertyChanged, this);
		}
		protected void NotifyPropertyChanged(object sender, EventArgs e) {
			if (PropertyChanged != null && e is PropertyChangedEventArgs)
				PropertyChanged(sender, e as PropertyChangedEventArgs);
		}
		protected override void ClearItems() {
			RemoveChildren(this);
			base.ClearItems();
		}
		protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if (e.OldItems != null && (e.Action == NotifyCollectionChangedAction.Reset || e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace))
				RemoveChildren(e.OldItems);
			if (e.NewItems != null && (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace))
				AddChildren(e.NewItems);
			NotifyPropertyChanged(this, new PropertyChangedEventArgs("Collection"));
		}
	}
}
