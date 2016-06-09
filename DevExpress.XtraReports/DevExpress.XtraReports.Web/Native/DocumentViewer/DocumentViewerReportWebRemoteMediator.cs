#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.ReportServer.ServiceModel.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Web.Native.DocumentViewer {
	public class DocumentViewerReportWebRemoteMediator : IReportWebRemoteMediator {
		readonly IDocumentViewerManagementService serviceManagement;
		readonly IReportServiceClient serviceClient;
		public InstanceIdentity InstanceIdentity { get; private set; }
		protected IReportServiceClient ServiceClient { get { return serviceClient; } }
		public DocumentViewerReportWebRemoteMediator(IServiceClientFactory<IReportServiceClient> clientFactory, InstanceIdentity instanceIdentity) {
			Guard.ArgumentNotNull(clientFactory, "clientFactory");
			Guard.ArgumentNotNull(instanceIdentity, "instanceIdentity");
			serviceClient = clientFactory.Create();
			serviceClient.SetSynchronizationContext(null);
			serviceManagement = new DocumentViewerManagementService(serviceClient);
			InstanceIdentity = instanceIdentity;
		}
		#region IReportWebRemoteMediator
		public ReportParameterContainer GetReportParameters() {
			Task<ReportParameterContainer> task = serviceManagement.GetReportParameters(InstanceIdentity);
			return task.Result;
		}
		public ParameterLookUpValues[] GetLookUpValues(ReportParameter[] parameterValues, string[] requiredParameterPaths) {
			Task<ParameterLookUpValues[]> task = serviceManagement.GetLookupValues(InstanceIdentity, parameterValues, requiredParameterPaths);
			return task.Result;
		}
		public RemoteDocumentInformation CreateDocument(ReportBuildArgs buildArgs) {
			var task = serviceManagement.BuildAsync(InstanceIdentity, buildArgs);
			return task.Result;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public void OnReceiveDocumentData(Action<DocumentMapTreeViewNode, Dictionary<string, bool>> receiveDocumentData) {
			serviceManagement.OnReceiveDocumentData(receiveDocumentData);
		}
		public PageInformation GetPage(ReportViewer viewer, RemoteDocumentInformation documentInformation, int pageIndex) {
			var bytes = serviceManagement.GetPageAsync(documentInformation.DocumentId, pageIndex).Result;
			return DocumentViewerRemoteHelper.DoWithRemoteDocument(bytes, pageIndex, documentInformation.PageCount, printingSystem => {
				var htmlContent = RenderPage(viewer, printingSystem, pageIndex);
				var pageSize = printingSystem.GetPageSize(pageIndex, !viewer.EnableReportMargins);
				return new PageInformation {
					HtmlContent = htmlContent,
					PageSize = pageSize
				};
			});
		}
		public string GetPagesForPrinting(ReportViewer viewer, RemoteDocumentInformation documentInformation) {
			const int PagesChunkSize = 40;
			var prnxPagesChunks = LoadPrnxPages(documentInformation, PagesChunkSize);
			using(var destPrintingSystem = new WebRemotePrintingSystem(documentInformation.PageCount)) {
				LoadDocument(prnxPagesChunks, destPrintingSystem);
				ClearDocumentBookmarks(destPrintingSystem.Document);
				var renderHelper = new RemoteReportRenderHelper(viewer, destPrintingSystem);
				return renderHelper.WriteForPrinting(-1);
			}
		}
		public Task<byte[]> ExportDocumentAsync(DocumentId documentId, ExportFormat exportFormat, ExportOptions exportOptions) {
			return serviceManagement.Export(documentId, exportFormat, exportOptions);
		}
		#endregion
		string RenderPage(ReportViewer viewer, PrintingSystemBase ps, int pageIndex) {
			var renderHelper = new RemoteReportRenderHelper(viewer, ps);
			return renderHelper.WritePage(pageIndex);
		}
		ICollection<byte[]> LoadPrnxPages(RemoteDocumentInformation documentInformation, int pageindexesChunkSize) {
			var result = new List<byte[]>();
			for(int i = 0; i < documentInformation.PageCount; i += pageindexesChunkSize) {
				var count = Math.Min(pageindexesChunkSize, documentInformation.PageCount - i);
				var task = serviceManagement.GetPagesAsync(documentInformation.DocumentId, Enumerable.Range(i, count).ToArray());
				result.Add(task.Result);
			}
			return result;
		}
		static void LoadDocument(ICollection<byte[]> prnxPagesChunks, WebRemotePrintingSystem destPrintingSystem) {
			var serializer = new PrintingSystemXmlSerializer();
			int pageIndex = 0;
			foreach(var prnxPagesChunk in prnxPagesChunks) {
				using(var stream = new MemoryStream(prnxPagesChunk))
				using(var sourcePrintingSystem = new DeserializedPrintingSystem()) {
					var brickPagePairFactory = new WebRemotePrintBrickPagePairFactory(sourcePrintingSystem, destPrintingSystem);
					sourcePrintingSystem.AddService<IBrickPagePairFactory>(brickPagePairFactory);
					sourcePrintingSystem.Document.Deserialize(stream, serializer);
					for(int i = 0; i < sourcePrintingSystem.PageCount; i++) {
						var page = sourcePrintingSystem.Pages[i];
						page.Owner = destPrintingSystem.Pages;
						page.AssignWatermarkReference(sourcePrintingSystem.Watermark);
						destPrintingSystem.Watermark.PageRange = sourcePrintingSystem.Watermark.PageRange;
						destPrintingSystem.Pages.RemoveAt(pageIndex);
						destPrintingSystem.Pages.Insert(pageIndex, page);
						brickPagePairFactory.ReadyPagesCount = ++pageIndex;
					}
				}
			}
		}
		static void ClearDocumentBookmarks(Document document) {
			document.RootBookmark.Nodes.Clear();
		}
	}
	public static class DocumentViewerRemoteHelper {
		public static T DoWithRemoteDocument<T>(byte[] bytes, int pageIndex, int pageCount, Func<PrintingSystemBase, T> func) {
			using(var stream = new MemoryStream(bytes))
			using(var sourcePrintingSystem = new DeserializedPrintingSystem())
			using(var destPrintingSystem = new WebRemotePrintingSystem(pageCount)) {
				var brickPagePairFactory = new WebRemoteBrickPagePairFactory(sourcePrintingSystem, destPrintingSystem, pageIndex);
				sourcePrintingSystem.AddService<IBrickPagePairFactory>(brickPagePairFactory);
				sourcePrintingSystem.Document.Deserialize(stream, new PrintingSystemXmlSerializer());
				var page = sourcePrintingSystem.Pages[0];
				page.Owner = destPrintingSystem.Pages;
				page.AssignWatermarkReference(sourcePrintingSystem.Watermark);
				destPrintingSystem.Watermark.PageRange = sourcePrintingSystem.Watermark.PageRange;
				destPrintingSystem.Graph.PageBackColor = sourcePrintingSystem.Graph.PageBackColor;
				destPrintingSystem.Pages.RemoveAt(pageIndex);
				destPrintingSystem.Pages.Insert(pageIndex, page);
				return func(destPrintingSystem);
			}
		}
	}
}
