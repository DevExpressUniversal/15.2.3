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

using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.XtraVerticalGrid.Data;
using DevExpress.Xpf.Editors.Helpers;
using System;
using System.Linq;
using System.Collections;
using DevExpress.Data.Utils;
using DevExpress.Mvvm.Native;
using System.Collections.Specialized;
namespace DevExpress.Xpf.PropertyGrid.Internal {
	public class PropertyDescriptorValueChangedEventHandler : WeakEventHandler<DescriptorContext, EventArgs, EventHandler> {
		WeakReference weakDescriptor;
		static Action<WeakEventHandler<DescriptorContext, EventArgs, EventHandler>, object> onDetachAction = (h, o) => {
			PropertyDescriptor descriptor = ((PropertyDescriptorValueChangedEventHandler)h).weakDescriptor.Target as PropertyDescriptor;
			if (descriptor == null)
				return;
			descriptor.RemoveValueChanged(o, h.Handler);
		};
		static Func<WeakEventHandler<DescriptorContext, EventArgs, EventHandler>, EventHandler> createHandlerFunction = h => h.OnEvent;
		public PropertyDescriptorValueChangedEventHandler(DescriptorContext context, Action<DescriptorContext, object, EventArgs> onEventAction) :
			base(context, onEventAction, onDetachAction, createHandlerFunction) {
			weakDescriptor = new WeakReference(context.PropertyDescriptor);
		}
	}
	public class INotifyPropertyChangedEventHandler : WeakEventHandler<DescriptorContext, PropertyChangedEventArgs, PropertyChangedEventHandler> {
		static Action<WeakEventHandler<DescriptorContext, PropertyChangedEventArgs, PropertyChangedEventHandler>, object> onDetachAction = (h, o) => {
			(o as INotifyPropertyChanged).Do(x => x.PropertyChanged -= h.Handler);
		};
		static Func<WeakEventHandler<DescriptorContext, PropertyChangedEventArgs, PropertyChangedEventHandler>, PropertyChangedEventHandler> createHandlerFunction = h => h.OnEvent;
		public INotifyPropertyChangedEventHandler(DescriptorContext context, Action<DescriptorContext, object, PropertyChangedEventArgs> onEventAction) :
			base(context, onEventAction, onDetachAction, createHandlerFunction) {
		}
	}
	public class INotifyCollectionChangedEventHandler : WeakEventHandler<DescriptorContext, NotifyCollectionChangedEventArgs, NotifyCollectionChangedEventHandler> {
		static Action<WeakEventHandler<DescriptorContext, NotifyCollectionChangedEventArgs, NotifyCollectionChangedEventHandler>, object> onDetachAction = (h, o) => {
			(o as INotifyCollectionChanged).Do(x => x.CollectionChanged -= h.Handler);
		};
		static Func<WeakEventHandler<DescriptorContext, NotifyCollectionChangedEventArgs, NotifyCollectionChangedEventHandler>, NotifyCollectionChangedEventHandler> createHandlerFunction = h => h.OnEvent;
		public INotifyCollectionChangedEventHandler(DescriptorContext context, Action<DescriptorContext, object, NotifyCollectionChangedEventArgs> onEventAction) :
			base(context, onEventAction, onDetachAction, createHandlerFunction) {
		}
	}
}
