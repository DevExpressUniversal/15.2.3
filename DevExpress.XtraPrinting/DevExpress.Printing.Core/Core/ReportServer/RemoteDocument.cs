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
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.Xpf.Printing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Parameters;
using System.Linq;
using DevExpress.DocumentServices.ServiceModel.ServiceOperations;
using DevExpress.DocumentServices.ServiceModel.Native;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.ReportServer.ServiceModel.Native.RemoteOperations;
using DevExpress.ReportServer.Printing.Services;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports;
using DevExpress.DocumentServices.ServiceModel;
using DevExpress.XtraReports.Parameters.Native;
using System.Threading.Tasks;
using DevExpress.Data;
using DevExpress.XtraPrinting.Native.DrillDown;
namespace DevExpress.ReportServer.Printing {
	public class RemoteDocument : PrintingDocument {
		#region fields and properties
		DocumentId documentId;
		readonly IServiceContainer serviceContainer;
		AvailableExportModes availableExportModes;
		InstanceIdentity identity;
		CreateDocumentOperation operation;
		IParameterContainer parametersContainer;
		RemotePageList RemotePages { get { return (RemotePageList)Pages; } }
		public override int PageCount { get { return RemotePages.Count; } }
		IReportServiceClient Client { get { return serviceContainer.GetService<IReportServiceClient>(); } }
		RemotePrintingSystem RemotePrintingSystem { get { return ps as RemotePrintingSystem; } }
		IBrickPagePairFactory BrickPagePairFactory { get { return serviceContainer.GetService<IBrickPagePairFactory>(); } }
		public override bool IsEmpty { get { return Pages == null || Pages.Count == 0; } }
		internal override bool CanPerformContinuousExport { get { return !IsEmpty && !Pages.ContainsExternalPages; } }
		#endregion
		#region ctor
		public RemoteDocument(PrintingSystemBase ps)
			: base(ps, null, null) {
			serviceContainer = (IServiceContainer)ps;
		}
		#endregion
		#region methods
		protected override PageList CreatePageList() {
			IPageListService serv = ps.GetService<IPageListService>();
			return new RemotePageList(this, new RemoteInnerPageList(serv));
		}
		public void RequestDocumentAsync(InstanceIdentity reportId, IParameterContainer defaultValueParameters) {
			Clear();
			identity = reportId;
			state = DocumentState.Creating;
			this.parametersContainer = defaultValueParameters;
			isCreated = false;
			Start(true);
		}
		internal void SubmitParameters() {
			Clear();
			state = DocumentState.Creating;
			isCreated = false;
			Start(false);
		}
		protected override void Clear() {
			if(operation != null) {
				UnsubscribeOperationEvents();
				operation.Abort();
				operation = null;
			}
			base.Clear();
			if(documentId != null) {
				Client.ClearDocumentAsync(documentId, null);
			}
			documentId = null;
		}
		void Start(bool shouldRequestReportInformation) {
			ReportBuildArgs reportBuildArgs = Helper.CreateReportBuildArgs(parametersContainer, null, null, null);
			reportBuildArgs.DrillDownKeys = GetDrillDownKeys();
			operation = CreateOperation(Client, identity, reportBuildArgs, shouldRequestReportInformation, Helper.DefaultStatusUpdateInterval);
			operation.Progress += operation_Progress;
			operation.Completed += operation_Completed;
			operation.GetReportParameters += operation_GetReportParameters;
			operation.Started += operation_Started;
			ps.OnBeforeBuildPages(EventArgs.Empty);
			operation.Start();
		}
		void operation_GetReportParameters(object sender, CreateDocumentReportParametersEventArgs e) {
			var operation = (CreateDocumentOperation)sender;
			var reportParameters = new ClientParameterContainer(e.ReportParameters);
			if(parametersContainer is DefaultValueParameterContainer) {
				try {
					CopyParameters((DefaultValueParameterContainer)parametersContainer, reportParameters);
				} catch(Exception error) {
					ps.OnCreateDocumentException(new ExceptionEventArgs(error));
					return;
				}
			}
			parametersContainer = reportParameters;
			e.ReportParameters.Parameters = reportParameters.ToParameterStubs();
			IReportPrintTool printTool = GetPrintTool();
			if(printTool != null) {
				IEnumerable<Parameter> originalParameters = reportParameters.OriginalParameters.Where(param => param.Visible);
				printTool.ApproveParameters(originalParameters.ToArray(), e.ReportParameters.ShouldRequestParameters);
			}
			operation.SetParameters(parametersContainer);
			var lookUpValuesProvider = RemotePrintingSystem.GetService<ILookUpValuesProvider>();
			if(lookUpValuesProvider != null)
				RemotePrintingSystem.RemoveService(typeof(ILookUpValuesProvider));
			RemotePrintingSystem.AddService<ILookUpValuesProvider>(new RemoteLookUpValuesProvider((ClientParameterContainer)parametersContainer, identity, () => Client));
			RemotePrintingSystem.OnReportParametersApproved(reportParameters.Cast<ClientParameter>().ToList(), lookUpValuesProvider);
		}
		void CopyParameters(DefaultValueParameterContainer source, ClientParameterContainer dest) {
			Exception error;
			if(!source.CopyTo(dest, out error))
				throw error;
		}
		protected virtual CreateDocumentOperation CreateOperation(IReportServiceClient client, InstanceIdentity instanceId, ReportBuildArgs args, bool shouldRequestReportInformation, TimeSpan statusUpdateInterval) {
			return new CreateDocumentOperation(client, instanceId, args, shouldRequestReportInformation, statusUpdateInterval);
		}
		protected internal override void StopPageBuilding() {
			if(operation != null && operation.CanStop) {
				operation.Stop();
			}
		}
		void operation_Started(object sender, CreateDocumentStartedEventArgs e) {
			documentId = e.DocumentId;
			serviceContainer.RemoveService(typeof(RemoteOperationFactory));
			serviceContainer.AddService(typeof(RemoteOperationFactory), new RemoteOperationFactory(Client, documentId, Helper.DefaultStatusUpdateInterval));
		}
		void operation_Completed(object sender, CreateDocumentCompletedEventArgs e) {
			UnsubscribeOperationEvents();
			if(e.Error != null) {
				AfterBuild();
				PrintingSystem.OnCreateDocumentException(new ExceptionEventArgs(e.Error));
				return;
			}
			if(documentId == null) {
				AfterBuild();
				return;
			}
			operation = null;
			Client.GetDocumentDataCompleted += Client_GetDocumentDataCompleted;
			availableExportModes = null;
			Client.GetDocumentDataAsync(documentId, e.UserState);
		}
		IReportPrintTool GetPrintTool() {
			IReport report = ps.GetService<IReport>();
			return report != null ? report.PrintTool : null;
		}
		IDrillDownServiceBase GetDrillDownService() {
			IReport report = ps.GetService<IReport>();
			return report != null ? report.GetService<IDrillDownServiceBase>() : null;
		}
		void Client_GetDocumentDataCompleted(object sender, ScalarOperationCompletedEventArgs<DocumentData> e) {
			var client = sender as IReportServiceClient;
			if(client != null)
				client.GetDocumentDataCompleted -= Client_GetDocumentDataCompleted;
			if(e.Error != null) {
				AfterBuild();
				PrintingSystem.OnCreateDocumentException(new ExceptionEventArgs(e.Error));
				return;
			}
			ExportOptions exportOptions = new ExportOptions();
			var result = e.Result;
			Helper.DeserializeExportOptions(exportOptions, result.ExportOptions);
			RemotePrintingSystem.CustomizeExportOptions(exportOptions, result.HiddenOptions);
			XtraPageSettingsBase pageSettings = new XtraPageSettingsBase(PrintingSystem);
			Helper.DeserializePageSettings(pageSettings, result.SerializedPageData);
			RemotePrintingSystem.PageSettings.Assign(pageSettings.Margins, pageSettings.PaperKind, pageSettings.Landscape);
			availableExportModes = e.Result.AvailableExportModes;
			if(e.Result.DrillDownKeys != null)
				AssignDrillDownKeys(e.Result.DrillDownKeys);
			RestoreBookmarkNodes(result.DocumentMap, RootBookmark);
			AfterBuild();
		}
		void AssignDrillDownKeys(Dictionary<string, bool> drillDownKeys) {
			var drillDownService = GetDrillDownService();
			if(drillDownService == null)
				return;
			drillDownService.Keys.Clear();
			drillDownKeys.ForEach(x => drillDownService.Keys[DrillDownKey.Parse(x.Key)] = x.Value);
		}
		Dictionary<string, bool> GetDrillDownKeys() {
			var drillDownService = GetDrillDownService();
			if(drillDownService == null)
				return null;
			var result = new Dictionary<string, bool>();
			drillDownService.Keys.ForEach(x => result[x.Key.ToString()] = x.Value);
			return result;
		}
		void RestoreBookmarkNodes(DocumentMapTreeViewNode rootNode, BookmarkNode rootBookmarkNode) {
			if(rootNode != null && rootNode.Nodes.Count > 0) {
				foreach(DocumentMapTreeViewNode node in rootNode.Nodes) {
					int[] indexes = BrickPagePairHelper.ParseIndices(DocumentMapTreeViewNodeHelper.GetBrickIndicesByTag(node.AssociatedElementTag));
					BrickPagePair pair = BrickPagePairFactory.CreateBrickPagePair(indexes, node.PageIndex);
					BookmarkNode newNode = new BookmarkNode(node.Text, pair);
					rootBookmarkNode.Nodes.Add(newNode);
					RestoreBookmarkNodes(node, newNode);
				}
			}
		}
		protected internal override void AfterBuild() {
			OnContentChanged();
			state = DocumentState.Created;
			isCreated = true;
			SetModified(false);
			ps.ProgressReflector.MaximizeRange();
			PrintingSystem.OnAfterBuildPages(EventArgs.Empty);
		}
		void operation_Progress(object sender, CreateDocumentProgressEventArgs e) {
			RemotePages.SetCount(e.PageCount);
			ps.ProgressReflector.SetPosition(e.ProgressPosition);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected internal virtual void CloneDocumentData(RemoteDocument newDocument) {
			newDocument.parametersContainer = this.parametersContainer;
		}
		public override void Dispose() {
			UnsubscribeOperationEvents();
			Client.GetDocumentDataCompleted -= Client_GetDocumentDataCompleted;
			RemotePages.Dispose();
			base.Dispose();
		}
		void UnsubscribeOperationEvents() {
			if(operation != null) {
				operation.Progress -= operation_Progress;
				operation.Completed -= operation_Completed;
				operation.Started -= operation_Started;
				operation.GetReportParameters -= operation_GetReportParameters;
			}
		}
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		internal bool IsDocumentDisposed { get { return IsDisposed; } }
		protected override object[] GetAvailableExportModes(Type exportModeType) {
			return availableExportModes == null ? base.GetAvailableExportModes(exportModeType) : availableExportModes.GetExportModesByType(exportModeType);
		}
		#endregion
		#region PrintingDocument overrides
		protected internal override DocumentBand AddReportContainer() {
			throw new NotImplementedException();
		}
		protected internal override void BeginReport(DocumentBand docBand, PointF offset) {
			throw new NotImplementedException();
		}
		protected internal override void EndReport() {
			throw new NotImplementedException();
		}
		protected internal override void InsertPageBreak(float pos, CustomPageData nextPageData) {
			throw new NotImplementedException();
		}
		protected internal override void InsertPageBreak(float pos) {
			throw new NotImplementedException();
		}
		public override void ShowFromNewPage(Brick brick) {
			throw new NotImplementedException();
		}
		#endregion
	}
}
