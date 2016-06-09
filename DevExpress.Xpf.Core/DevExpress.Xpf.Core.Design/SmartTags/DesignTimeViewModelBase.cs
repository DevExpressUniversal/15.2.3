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

extern alias Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.Model;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Windows.Design.Services;
using Microsoft.Windows.Design;
using System.Linq.Expressions;
using DevExpress.Design.SmartTags;
using System.Windows;
using Platform::DevExpress.Data.Utils;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System.Windows.Threading;
namespace DevExpress.Xpf.Core.Design {
	public abstract class DesignTimeViewModelBase : INotifyPropertyChanged, IDisposable {
		IModelSubscribedEvent onSelectedItemPropertyChangedSubscribedEvent;
		public IModelItem RuntimeSelectedItem {
			get { return runtimeSelectedItem; }
			private set {
				if(runtimeSelectedItem == value) return;
				var oldValue = runtimeSelectedItem;
				runtimeSelectedItem = value;
				OnSelectedItemChanged(oldValue);
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		public DesignTimeViewModelBase(IModelItem selectedItem) {
			if(selectedItem == null)
				throw new ArgumentNullException("selectedItem");
			RuntimeSelectedItem = selectedItem;
		}		
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			RuntimeSelectedItem = null;
		}
		protected void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		protected internal void SetPropertyValue(string propertyName, object value) {
			ModelPropertyHelper.SetPropertyValue(RuntimeSelectedItem, propertyName, value);
		}
		protected internal object GetPropertyValue(string propertyName) {
			return ModelPropertyHelper.GetPropertyValue(RuntimeSelectedItem, propertyName);
		}
		protected virtual void OnSelectedItemChanged(IModelItem oldSelectedItem) {
			UnsubscribeEvents(oldSelectedItem);
			SubscribeEvents();
		}
		protected virtual void OnSelectedItemPropertyChanged(string propertyName) {
			RaisePropertyChanged(propertyName);
		}
		protected virtual void SubscribeEvents() {
			if(RuntimeSelectedItem != null) {
				onSelectedItemPropertyChangedSubscribedEvent = RuntimeSelectedItem.SubscribeToPropertyChanged(InvokeOnSelectedItemPropertyChanged);
				IModelService service = RuntimeSelectedItem.Context.Services.GetService<IModelService>();
				if(service != null)
					SubscribeModelChanged(service);
			}
		}
		void InvokeOnSelectedItemPropertyChanged(object sender, EventArgs e) {
			PropertyChangedEventArgs args = (PropertyChangedEventArgs)e;
			Dispatcher.CurrentDispatcher.BeginInvoke((Action<string>)OnSelectedItemPropertyChanged, args.PropertyName);
		}
		protected virtual void UnsubscribeEvents(IModelItem oldSelectedItem) {
			if(oldSelectedItem != null) {
				oldSelectedItem.UnsubscribeFromPropertyChanged(onSelectedItemPropertyChangedSubscribedEvent);
				IModelService service = oldSelectedItem.Context.Services.GetService<IModelService>();
				if(service != null)
					UnsubscribeModelChanged(service);
			}
		}
		protected virtual void SubscribeModelChanged(IModelService service) { }
		protected virtual void UnsubscribeModelChanged(IModelService service) { }
		IModelItem runtimeSelectedItem;
	}
}
