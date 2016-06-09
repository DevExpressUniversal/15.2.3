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
using System.ComponentModel;
using System.Linq;
using System.Collections.Specialized;
using System.Reflection;
using System.Collections;
using DevExpress.Data.Utils;
using DevExpress.Utils;
namespace DevExpress.Mvvm.UI.Native.ViewGenerator.Model {
	public abstract class ModelItemBase : IModelItem {
		public static object GetUnderlyingObject(object value) {
			if(value is IModelItem)
				value = ((IModelItem)value).View.PlatformObject;
			return value;
		}
		public readonly object element;
		readonly EditingContextBase context;
		readonly IModelItem parent;
		readonly IViewItem view;
		readonly IModelPropertyCollection properties;
		readonly PropertyChangedWeakEventHandler<ModelItemBase> propertyChangedHandler;
		PropertyChangedEventHandler propertyChanged;
		public ModelItemBase(EditingContextBase context, object element, IModelItem parent) {
			Guard.ArgumentNotNull(context, "context");
			this.parent = parent;
			this.context = context;
			this.element = element;
			this.properties = context.CreateModelPropertyCollection(element, this);
			this.view = context.CreateViewItem(this);
			this.propertyChangedHandler = new PropertyChangedWeakEventHandler<ModelItemBase>(this, (item, sender, e) => item.OnPropertyChanged(sender, e));
			if(element is INotifyPropertyChanged) {
				((INotifyPropertyChanged)element).PropertyChanged += propertyChangedHandler.Handler;
			}
		}
		public IModelPropertyCollection Properties { get { return properties; } }
		IModelEditingScope IModelItem.BeginEdit(string description) {
			return context.CreateEditingScope(description);
		}
		IEditingContext IModelItem.Context { get { return context; } }
		IViewItem IModelItem.View { get { return view; } }
		Type IModelItem.ItemType { get { return element.GetType(); } }
		IModelItem IModelItem.Root { get { return context.Services.GetService<IModelService>().Root; } }
		IModelSubscribedEvent IModelItem.SubscribeToPropertyChanged(EventHandler handler) {
			return ModelSubscribedEvent<PropertyChangedEventHandler>.Subscribe((s, e) => handler(s, e), h => propertyChanged += h);
		}
		void IModelItem.UnsubscribeFromPropertyChanged(IModelSubscribedEvent e) {
			ModelSubscribedEvent<PropertyChangedEventHandler>.Unsubscribe(e, h => propertyChanged -= h);   
		}
		protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(propertyChanged != null)
				propertyChanged(sender, e);
		}
		object IModelItem.GetCurrentValue() {
			return element;
		}
		public IEnumerable<object> GetAttributes(Type attributeType) {
			return element.GetType().GetCustomAttributes(attributeType, true);
		}
		public virtual string Name {
			get { return null; }
			set { }
		}
		IModelItem IModelItem.Parent {
			get { return parent; }
		}
	}
	public struct ModelSubscribedEvent<TEventHandler> : IModelSubscribedEvent {
		public static IModelSubscribedEvent Subscribe(TEventHandler handler, Action<TEventHandler> subscribeAction) {
			ModelSubscribedEvent<TEventHandler> e = new ModelSubscribedEvent<TEventHandler>(handler);
			subscribeAction(e.handler);
			return e;
		}
		public static void Unsubscribe(IModelSubscribedEvent e, Action<TEventHandler> unsubscribeAction) {
			if(e == null) return;
			ModelSubscribedEvent<TEventHandler> p = (ModelSubscribedEvent<TEventHandler>)e;
			unsubscribeAction(p.handler);
		}
		ModelSubscribedEvent(TEventHandler handler) { this.handler = handler; }
		readonly TEventHandler handler;
	}
}
