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
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {
	public enum RequestResultCode {
		Success,
		BadRequest,
		ServerError,
		Timeout
	}
	public abstract class RequestResultBase {
		readonly RequestResultCode statusCode;
		readonly string faultReason;
#if !SL
	[DevExpressXtraMapLocalizedDescription("RequestResultBaseResultCode")]
#endif
		public RequestResultCode ResultCode { get { return statusCode; } }
#if !SL
	[DevExpressXtraMapLocalizedDescription("RequestResultBaseFaultReason")]
#endif
		public string FaultReason { get { return faultReason; } }
		protected RequestResultBase(RequestResultCode statusCode, string faultReason) {
			this.statusCode = statusCode;
			this.faultReason = faultReason;
		}
	}
	public class RequestCompletedEventArgs : AsyncCompletedEventArgs {
		readonly MapItem[] items;
		public MapItem[] Items { get { return items; } }
		public RequestCompletedEventArgs(MapItem[] items, Exception error, bool cancelled, object userState)
			: base(error, cancelled, userState) {
			this.items = items;
		}
	}
	public class LayerItemsGeneratingEventArgs : AsyncCompletedEventArgs {
		readonly MapItem[] items;
		public MapItem[] Items { get { return this.items; } }
		public LayerItemsGeneratingEventArgs(MapItem[] items, Exception error, bool cancelled, object userState)
			: base(error, cancelled, userState) {
			this.items = items;
		}
	}
	public delegate void LayerItemsGeneratingEventHandler(object sender, LayerItemsGeneratingEventArgs e);
	public abstract class InformationDataProviderBase : IOwnedElement {
		const bool DefaultGenerateLayerItems = true;
		object owner;
		bool generateLayerItems = DefaultGenerateLayerItems;
		protected internal abstract int MaxVisibleResultCountInternal { get; }
		[Browsable(false)]
		public abstract bool IsBusy { get; protected set; }
		protected InformationLayer Layer { get { return owner as InformationLayer; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("InformationDataProviderBaseGenerateLayerItems"),
#endif
		Category(SRCategoryNames.Behavior), DefaultValue(DefaultGenerateLayerItems)]
		public bool GenerateLayerItems {
			get { return generateLayerItems; }
			set { generateLayerItems = value; }
		}
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				if(owner == value)
					return;
				object oldOwner = owner;
				owner = value;
				OnOwnerChanged(oldOwner as InformationLayer, owner as InformationLayer);
			}
		}
		#endregion
		internal event EventHandler<RequestCompletedEventArgs> RequestCompleted;
		protected virtual void OnOwnerChanged(InformationLayer oldLayer, InformationLayer newLayer) {
			if(newLayer == null)
				Cancel();
		}
#if !SL
	[DevExpressXtraMapLocalizedDescription("InformationDataProviderBaseLayerItemsGenerating")]
#endif
		public event LayerItemsGeneratingEventHandler LayerItemsGenerating;
		protected void OnRequestComplete(RequestCompletedEventArgs e) {
			if (RequestCompleted != null) 
				RequestCompleted(this, e);
		}
		protected void RaiseLayerItemsGenerating(LayerItemsGeneratingEventArgs args) {
			if(LayerItemsGenerating != null)
				LayerItemsGenerating(this, args);
		}
		protected internal abstract void Cancel();
	}
}
