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
using DevExpress.Utils;
using System.Windows.Markup;
using System.Windows.Data;
using System.Collections;
using System.Windows;
using System.Collections.Generic;
using DevExpress.Utils.Localization;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls.Primitives;
using DevExpress.Utils.Localization.Internal;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.IO;
using System.Reflection;
using System.Windows.Interop;
using System.Windows.Input;
using DevExpress.Xpf.Utils;
#if SL
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.WPFCompatibility;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
namespace DevExpress.Xpf.Core {
	public interface INotifyContentChanged {
		event EventHandler ContentChanged;
	}
	public enum ColumnTextAlignment {
		Default,
		Left,
		Right,
		Center,
		Justify
	}
	public interface IDataObjectReset {
		void Reset();
	}
	public class DataObjectBase : DependencyObject, INotifyContentChanged, INotifyPropertyChanged {
		public static readonly DependencyProperty DataObjectProperty;
		public static readonly DependencyProperty NeedsResetEventProperty;
		public static readonly DependencyProperty RaiseResetEventWhenObjectIsLoadedProperty;
		public static readonly RoutedEvent ResetEvent;
		static DataObjectBase() {
			DataObjectProperty = DependencyPropertyManager.RegisterAttached("DataObject", typeof(DataObjectBase), typeof(DataObjectBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, OnDataObjectChanged));
			NeedsResetEventProperty = DependencyPropertyManager.RegisterAttached("NeedsResetEvent", typeof(bool), typeof(DataObjectBase), new PropertyMetadata(false, OnNeedsResetEventChanged));
			RaiseResetEventWhenObjectIsLoadedProperty = DependencyPropertyManager.RegisterAttached("RaiseResetEventWhenObjectIsLoaded", typeof(bool), typeof(DataObjectBase), new PropertyMetadata(false));
			ResetEvent = EventManager.RegisterRoutedEvent("Reset", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(DataObjectBase));
		}
		public static void AddResetHandler(DependencyObject dObj, RoutedEventHandler handler) {
#if !SL
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(ResetEvent, handler, false);
#endif
		}
		public static void RemoveResetHandler(DependencyObject dObj, RoutedEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(ResetEvent, handler);
		}
		protected static void OnDataObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(!GetNeedsResetEvent(d))
				return;
			if(e.OldValue != null)
				((DataObjectBase)e.OldValue).elements.Remove((FrameworkElement)d);
			if(e.NewValue != null)
				((DataObjectBase)e.NewValue).elements.Add((FrameworkElement)d);
		}
		protected static void OnNeedsResetEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DataObjectBase dataObject = GetDataObject(d);
			if(dataObject == null)
				return;
			if(GetNeedsResetEvent(d))
				dataObject.elements.Add((FrameworkElement)d);
			else
				dataObject.elements.Remove((FrameworkElement)d);
		}
		public static void SetDataObject(DependencyObject element, DataObjectBase value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(DataObjectProperty, value);
		}
		public static DataObjectBase GetDataObject(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (DataObjectBase)element.GetValue(DataObjectProperty);
		}
		public static void SetNeedsResetEvent(DependencyObject element, bool value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(NeedsResetEventProperty, value);
		}
		public static bool GetNeedsResetEvent(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (bool)element.GetValue(NeedsResetEventProperty);
		}
		public static void SetRaiseResetEventWhenObjectIsLoaded(DependencyObject element, bool value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(RaiseResetEventWhenObjectIsLoadedProperty, value);
		}
		public static bool GetRaiseResetEventWhenObjectIsLoaded(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (bool)element.GetValue(RaiseResetEventWhenObjectIsLoadedProperty);
		}
		static void RaiseResetEvent(FrameworkElement item) {
#if !SL
			item.RaiseEvent(new RoutedEventArgs() { RoutedEvent = ResetEvent });
#endif
			IDataObjectReset elem = item as IDataObjectReset;
			if(elem != null) {
				elem.Reset();
			}
		}
		List<FrameworkElement> elements = new List<FrameworkElement>();
		public void RaiseResetEvents() {
			foreach(FrameworkElement item in elements) {
				if(GetRaiseResetEventWhenObjectIsLoaded(item) && !FrameworkElementHelper.GetIsLoaded(item)) {
					item.Loaded += new RoutedEventHandler(OnElementLoaded);
				} else {
					RaiseResetEvent(item);
				}
			}
		}
		void OnElementLoaded(object sender, System.Windows.RoutedEventArgs e) {
			FrameworkElement element = sender as FrameworkElement;
			if(element != null) {
				element.Loaded -= new RoutedEventHandler(OnElementLoaded);
				RaiseResetEvent(element);
			}
		}
		#region INotifyContentChanged Members
		public event EventHandler ContentChanged;
		protected virtual void RaiseContentChanged() {
			if(ContentChanged != null)
				ContentChanged(this, EventArgs.Empty);
		}
		#endregion
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			var handler = PropertyChanged;
			if(handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
