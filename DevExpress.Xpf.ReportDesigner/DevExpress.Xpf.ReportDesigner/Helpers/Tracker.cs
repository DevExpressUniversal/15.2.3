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

using DevExpress.Xpf.Core.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using System.Windows;
using DevExpress.Diagram.Core;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public interface IObjectTracker {
		event PropertyChangedEventHandler ObjectPropertyChanged;
		object Object { get; }
	}
	public sealed class TrackablePropertyDescriptor : PropertyDescriptorWrapper {
		internal interface IObjectTrackerInternal : IObjectTracker {
			void AddPropertyChanged(string propertyName, EventHandler handler);
			void RemovePropertyChanged(string propertyName, EventHandler handler);
			void RaisePropertyChanged(string propertyName);
		}
		readonly List<IObjectTracker> trackers = new List<IObjectTracker>();
		public TrackablePropertyDescriptor(PropertyDescriptor baseDescriptor) : base(baseDescriptor) { }
		public override bool SupportsChangeEvents { get { return true; } }
		public override void AddValueChanged(object component, EventHandler handler) {
			Guard.ArgumentNotNull(component, "component");
			IObjectTracker tracker;
			Tracker.GetTracker(component, out tracker);
			trackers.Add(tracker);
			((IObjectTrackerInternal)tracker).AddPropertyChanged(Name, handler);
		}
		public override void RemoveValueChanged(object component, EventHandler handler) {
			Guard.ArgumentNotNull(component, "component");
			IObjectTracker tracker;
			Tracker.GetTracker(component, out tracker);
			((IObjectTrackerInternal)tracker).RemovePropertyChanged(Name, handler);
			trackers.Remove(tracker);
		}
		protected override void OnValueChanged(object component, EventArgs e) {
			IObjectTracker tracker;
			Tracker.GetTracker(component, out tracker);
			((IObjectTrackerInternal)tracker).RaisePropertyChanged(Name);
		}
		public override void SetValue(object component, object value) {
			base.SetValue(component, value);
			OnValueChanged(component, EventArgs.Empty);
		}
		public override void ResetValue(object component) {
			base.ResetValue(component);
			OnValueChanged(component, EventArgs.Empty);
		}
	}
	public static class Tracker {
		class ObjectTracker : TrackablePropertyDescriptor.IObjectTrackerInternal {
			readonly object obj;
			readonly Dictionary<string, EventHandler> handlers = new Dictionary<string, EventHandler>();
			public ObjectTracker(object obj) {
				this.obj = obj;
			}
			public object Object { get { return obj; } }
			public event PropertyChangedEventHandler ObjectPropertyChanged;
			public void RaisePropertyChanged(string propertyName) {
				if(ObjectPropertyChanged != null)
					ObjectPropertyChanged(obj, new PropertyChangedEventArgs(propertyName));
				EventHandler h;
				if(!handlers.TryGetValue(propertyName, out h)) return;
				if(h != null)
					h(this, EventArgs.Empty);
			}
			public void AddPropertyChanged(string propertyName, EventHandler handler) {
				EventHandler h;
				if(!handlers.TryGetValue(propertyName, out h))
					handlers.Add(propertyName, handler);
				else
					handlers[propertyName] = h + handler;
			}
			public void RemovePropertyChanged(string propertyName, EventHandler handler) {
				EventHandler h;
				if(!handlers.TryGetValue(propertyName, out h)) return;
				h = h - handler;
				if(h == null)
					handlers.Remove(propertyName);
				else
					handlers[propertyName] = h;
			}
		}
		static readonly WrappersManager<object, ObjectTracker> trackersManager = new WrappersManager<object, ObjectTracker>();
		public static void Set<T, TProperty>(T obj, Expression<Func<T, TProperty>> property, TProperty value) where T : class {
			var propertyName = ExpressionHelper.GetPropertyName(property);
			Set(obj, propertyName, value);
		}
		public static void Set<T, TProperty>(T obj, string propertyName, TProperty value) where T : class {
			TypeDescriptor.GetProperties(obj)[propertyName].SetValue(obj, value);
			GetTrackerCore(obj).RaisePropertyChanged(propertyName);
		}
		public static void GetTracker(object obj, out IObjectTracker objectTracker) { 
			objectTracker = GetTrackerCore(obj);
		}
		static ObjectTracker GetTrackerCore(object obj) {
			return trackersManager.Wrap(obj, x => new ObjectTracker(x));
		}
	}
}
