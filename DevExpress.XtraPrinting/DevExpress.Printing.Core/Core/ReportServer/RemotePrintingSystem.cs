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
using DevExpress.ReportServer.Printing.Services;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.XtraReports.Parameters;
using DevExpress.DocumentServices.ServiceModel;
using DevExpress.XtraReports.Parameters.Native;
using System.ComponentModel;
using DevExpress.XtraPrinting.Native.DrillDown;
namespace DevExpress.ReportServer.Printing {
	public class RemotePrintingSystem : PrintingSystemBase {
		#region fields and properties
		IReportServiceClient client;
		readonly IPageListService pageListService;
		object padlock = new object();
		System.Threading.Timer pageTimer;
		Dictionary<object, int[]> servicePageIndexes = new Dictionary<object, int[]>();
		internal event GetRemoteParametersCompletedEventHandler GetRemoteParametersCompleted;
		#endregion
		protected internal RemotePrintingSystem() {
			pageListService = new PageListService(this) { DefaultPageSettings = this.PageSettings };
			pageListService.RequestPagesException += pageListService_RequestPagesException;
			AddService(typeof(IPageListService), pageListService);
			AddService(typeof(IUpdateDrillDownReportStrategy), new RemoteUpdateDrillDownReportStrategy());
			AddService(typeof(IEmptyPageFactory), new EmptyPageFactory());
			AddService(typeof(IPageOwnerProvider), new PageOwnerProvider(Pages));
			AddService(typeof(IBrickPagePairFactory), new BrickPagePairFactory(pageListService));
			AddService(typeof(IRemotePrintService), new RemotePrintService(pageListService, InvalidatePages));
		}
		void pageListService_RequestPagesException(object sender, ExceptionEventArgs args) {
			args.Handled = true;
			OnCreateDocumentException(new ExceptionEventArgs(args.Exception));
		}
		public RemotePrintingSystem(IReportServiceClient client) : this() {
			this.client = client;
			AddService(typeof(IReportServiceClient), client);
		}
		protected internal void EnsureClient(IReportServiceClient client) {
			this.client = client;
			if(this.GetService<IReportServiceClient>() != null) {
				Document.ClearContent();
				this.RemoveService(typeof(IReportServiceClient));
			}
			AddService(typeof(IReportServiceClient), client);
		}
		protected internal override void EnsureBrickOnPage(BrickPagePair pair, Action<BrickPagePair> onEnsured) {
			if (pageListService.PagesShouldBeLoaded(pair.PageIndex)) {
				pageListService.Ensure(new int[] { pair.PageIndex }, indexes => {
					System.Diagnostics.Debug.Assert(!IsDisposed && !IsDisposing);
					onEnsured(pair);
					InvalidatePages(indexes);
				});
			} else
				onEnsured(pair);
		}
		protected override void AfterDrawPagesCore(object syncObj, int[] pageIndices) {
			base.AfterDrawPagesCore(syncObj, pageIndices);
			lock(padlock) {
				servicePageIndexes[syncObj] = pageIndices;
				if(pageTimer != null) pageTimer.Dispose();
				pageTimer = new System.Threading.Timer(OnTimerCallback, null, 500, System.Threading.Timeout.Infinite);
			}
		}
		void OnTimerCallback(object state) {
			lock(padlock) {
				List<int> indexes = new List<int>();
				foreach(int[] item in servicePageIndexes.Values)
					indexes.AddRange(item);
				pageListService.Ensure(indexes.ToArray(), InvalidatePages);
				servicePageIndexes.Clear();
			}
		}
		void InvalidatePages(int[] indexes) {
			OnDocumentChanged(EventArgs.Empty);
		}
		[DXHelpExclude(true), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		virtual public void RequestRemoteDocument(InstanceIdentity reportId, IParameterContainer parameters) {
			((RemoteDocument)Document).RequestDocumentAsync(reportId, parameters);
		}
		[DXHelpExclude(true), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		virtual public void SubmitParameters() {
			((RemoteDocument)Document).SubmitParameters();
		}
		internal void CustomizeExportOptions(ExportOptions exportOptions, ExportOptionKind hiddenOptions) {
			ExportOptions.Assign(exportOptions);
			HideExportOptions(hiddenOptions);
		}
		void HideExportOptions(ExportOptionKind hiddenOptions) {
			var exportOptionKinds = (ExportOptionKind[])Enum.GetValues(typeof(ExportOptionKind));
			long mask = 0;
			long flag = (long)hiddenOptions;
			foreach(var exportOptionKind in exportOptionKinds) {
				mask = (long)exportOptionKind;
				if((mask & flag) != 0)
					ExportOptions.SetOptionVisibility(exportOptionKind, false);
			}
		}
		protected override PrintingDocument CreateDocument() {
			return new RemoteDocument(this);
		}
		protected override void Dispose(bool disposing) {
			if(pageTimer != null) {
				pageTimer.Dispose();
				pageTimer = null;
			}
			pageListService.Dispose();
			base.Dispose(disposing);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal void OnReportParametersApproved(IList<ClientParameter> parameters, ILookUpValuesProvider lookUpValuesProvider) {
			if(GetRemoteParametersCompleted != null)
				GetRemoteParametersCompleted(this, new GetRemoteParametersCompletedEventArgs(parameters, lookUpValuesProvider));
		}
	}
}
