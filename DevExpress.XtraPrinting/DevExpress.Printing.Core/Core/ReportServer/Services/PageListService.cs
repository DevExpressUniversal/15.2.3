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
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.Data.Utils.ServiceModel;
using System.Collections;
using DevExpress.ReportServer.IndexedCache;
using DevExpress.ReportServer.ServiceModel.Native.RemoteOperations;
using DevExpress.ReportServer.ServiceModel.Native;
namespace DevExpress.ReportServer.Printing.Services {
	class PageListService : IndexedCache<RemotePageInfo>, IPageListService {
		#region field and properties
		readonly IServiceProvider serviceProvider;
		bool isRequestActive;
		int[] requestedPageIndexes;
		internal XtraPageSettingsBase DefaultPageSettings { get; set; }
		public event ExceptionEventHandler RequestPagesException;
		RemoteOperationFactory PageOperationFactory { get { return serviceProvider.GetService<RemoteOperationFactory>(); } }
		IEmptyPageFactory EmptyPageFactory { get { return serviceProvider.GetService<IEmptyPageFactory>(); } }
		IPageOwnerProvider PageOwnerProvider { get { return serviceProvider.GetService<IPageOwnerProvider>(); } }
		#endregion
		#region ctor
		public PageListService(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
		}
		#endregion
		#region methods
		protected override RemotePageInfo CreateFakeValue(int index, int count) {
			Page page = EmptyPageFactory.CreateEmptyPage(index, count);
			if(DefaultPageSettings != null)
				page.PageData = new PageData(DefaultPageSettings.MarginsF, DefaultPageSettings.PaperKind, DefaultPageSettings.Landscape);
			page.Owner = PageOwnerProvider.PageOwner;
			return new RemotePageInfo(page);
		}
		protected override void StartRequestIfNeeded() {
			if(isRequestActive)
				return;
			List<int> indexesToRequest = new List<int>();
			for(int i = 0; i < Capacity; i++)
				if(cache[i].State == IndexedCacheItemState.Requested)
					indexesToRequest.Add(i);
			if(indexesToRequest.Count == 0) {
				isRequestActive = false;
				return;
			}
			requestedPageIndexes = indexesToRequest.ToArray();
			RequestPagesOperation requestPageOperation = PageOperationFactory.CreateRequestPagesOperation(requestedPageIndexes,
				(IBrickPagePairFactory)serviceProvider.GetService(typeof(IBrickPagePairFactory)));
			requestPageOperation.RequestPagesException += requestPageOperation_RequestPagesException;
			requestPageOperation.OperationCompleted += requestPageOperation_OperationCompleted;
			isRequestActive = true;
			requestPageOperation.Start(); ;
		}
		void requestPageOperation_RequestPagesException(object sender, ExceptionEventArgs args) {
			((RequestPagesOperation)sender).RequestPagesException -= requestPageOperation_RequestPagesException;
			if (RequestPagesException != null)
				RequestPagesException(this, args);
		}
		public bool PagesShouldBeLoaded(params int[] pageIndexes) {
			return EnsureIndexes(pageIndexes).Length != 0;
		}
		public IEnumerator GetEnumerator() {
			return new PageListEnumerator(cache);
		}
		void requestPageOperation_OperationCompleted(object sender, ScalarOperationCompletedEventArgs<DeserializedPrintingSystem> e) {
			((RequestPagesOperation)sender).OperationCompleted -= requestPageOperation_OperationCompleted;
			DeserializedPrintingSystem ps = (DeserializedPrintingSystem)e.Result;
			Dictionary<int, RemotePageInfo> result = new Dictionary<int, RemotePageInfo>();
			if(IsDisposed)
				return;
			for(int i = 0; i < requestedPageIndexes.Length; i++) {
				var page = ps.Pages[i];
				page.Owner = PageOwnerProvider.PageOwner;
				page.AssignWatermarkReference(ps.Watermark);
				result[requestedPageIndexes[i]] = new RemotePageInfo(page, ps);
			}
			isRequestActive = false;
			requestedPageIndexes = null;
			OnRequestCompleted(result);
		}
		new public Page this[int index] { 
			get { return base[index].Page; }
			set { base[index].Page = value; }
		}
		#endregion
	}
}
