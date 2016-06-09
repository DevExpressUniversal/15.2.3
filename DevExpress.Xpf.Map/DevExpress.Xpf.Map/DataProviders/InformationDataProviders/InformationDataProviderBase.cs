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

using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Windows;
namespace DevExpress.Xpf.Map {
	public enum RequestResultCode {
		Success,
		BadRequest,
		ServerError,
		Timeout
	}
	public abstract class RequestResultBase {
		readonly RequestResultCode statusCode;
		readonly string faultReason;
		public RequestResultCode ResultCode { get { return statusCode; } }
		public string FaultReason { get { return faultReason; } }
		public RequestResultBase(RequestResultCode statusCode, string faultReason) {
			this.statusCode = statusCode;
			this.faultReason = faultReason;
		}
	}
	public class RequestCompletedEventArgs : AsyncCompletedEventArgs {
		readonly MapItem[] items;
		public MapItem[] Items { get { return items; } }
		public RequestCompletedEventArgs(MapItem[] items, Exception error, bool cancelled, object userState) :  base(error, cancelled, userState) {
			this.items = items;
		}
	}
	public class LayerItemsGeneratingEventArgs : AsyncCompletedEventArgs {
		readonly MapItem[] items;
		public MapItem[] Items { get { return this.items; } }
		public LayerItemsGeneratingEventArgs(MapItem[] items, Exception error, bool cancelled, object userState) : base(error, cancelled, userState) {
			this.items = items;
		}
	}
	public delegate void LayerItemsGeneratingEventHandler(object sender, LayerItemsGeneratingEventArgs args);
	public abstract class InformationDataProviderBase : MapDependencyObject, IOwnedElement {
		public static readonly DependencyProperty GenerateLayerItemsProperty = DependencyPropertyManager.Register("GenerateLayerItems",
			typeof(bool), typeof(InformationDataProviderBase), new PropertyMetadata(true, NotifyPropertyChanged));
		public static readonly DependencyProperty CultureNameProperty = DependencyPropertyManager.Register("CultureName",
			typeof(string), typeof(InformationDataProviderBase), new PropertyMetadata(CultureInfo.InvariantCulture.Name));
		[Category(Categories.Behavior)]
		public bool GenerateLayerItems {
			get { return (bool)GetValue(GenerateLayerItemsProperty); }
			set { SetValue(GenerateLayerItemsProperty, value); }
		}
		[Category(Categories.Behavior)]
		public string CultureName {
			get { return (string)GetValue(CultureNameProperty); }
			set { SetValue(CultureNameProperty, value); }
		}
		object owner;
		protected bool HasRequestCompletedSubscribers { get { return RequestCompleted != null; } }
		protected internal InformationLayer Layer { get { return this.owner as InformationLayer; } }
		protected internal abstract int MaxVisibleResultCountInternal { get; }
		protected string ActualCultureName { get { return string.IsNullOrEmpty(CultureName) ? Thread.CurrentThread.CurrentUICulture.Name : CultureName; } }
		public abstract bool IsBusy { get; }
		#region IOwnerElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				object oldOwner = this.owner;
				this.owner = value;
				InfromationLayerChanged(oldOwner as InformationLayer, this.owner as InformationLayer);
			}
		}
		#endregion
		internal event EventHandler<RequestCompletedEventArgs> RequestCompleted;
		[SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
		public event LayerItemsGeneratingEventHandler LayerItemsGenerating;
		protected void RaiseLayerItemsGenerating(LayerItemsGeneratingEventArgs args) {
			if (LayerItemsGenerating != null)
				LayerItemsGenerating(this, args);
		}
		protected void RaiseRequestComplete(RequestCompletedEventArgs e) {
			if (HasRequestCompletedSubscribers)
				RequestCompleted(this, e);
		}
		virtual protected void InfromationLayerChanged(InformationLayer oldLayer, InformationLayer newLayer) { }
		public abstract void Cancel();
	}
}
