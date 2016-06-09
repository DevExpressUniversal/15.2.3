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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Web.Internal;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.DocumentViewer;
using DevExpress.XtraReports.Web.Localization;
using DevExpress.XtraReports.Web.Native.DocumentViewer.Remote;
using DevExpress.XtraReports.Web.Native.ParametersPanel;
namespace DevExpress.XtraReports.Web.Native.DocumentViewer {
	[ToolboxItem(false)]
	public class DocumentViewerReportViewer : ReportViewer {
		public const string RemoteKey = "remote";
		bool isParentControlCallback; 
		bool remoteMode;
		RSRemoteDocumentInformation rsRemoteDocumentInformation;
		ReportParameter[] remoteParameters;
		Dictionary<string, bool> remoteDrillDownKeys;
		internal IReportWebRemoteMediator ReportWebRemoteMediator { get; set; }
		internal override bool RemoteMode {
			get { return remoteMode; }
			set { remoteMode = value; }
		}
		internal bool HasRemoteDocumentInformation {
			get { return RemoteDocumentInformation != null; }
		}
		protected internal override bool AllowNullReport {
			get { return true; }
		}
		bool ShouldRestoreRSRemoteDocumentInfoFromPostData {
			get {
				return isParentControlCallback
					|| (Request != null && Request["__EVENTTARGET"] == UniqueID);
			}
		}
		RSRemoteDocumentInformation RSRemoteDocumentInformation {
			get {
				if(rsRemoteDocumentInformation == null) {
					if(ShouldRestoreRSRemoteDocumentInfoFromPostData) {
						var value = GetClientObjectStateValueString(RemoteKey);
						rsRemoteDocumentInformation = RSRemoteDocumentInformation.Deserialize(value);
					} else {
						rsRemoteDocumentInformation = new RSRemoteDocumentInformation();
					}
				}
				return rsRemoteDocumentInformation;
			}
		}
		RemoteDocumentInformation RemoteDocumentInformation {
			get { return RSRemoteDocumentInformation.DocumentInformation; }
			set { RSRemoteDocumentInformation.DocumentInformation = value; }
		}
		ActionBuilder ActionBuilder {
			get { return Content.ActionBuilder; }
		}
		public DocumentViewerReportViewer(bool isParentControlCallback) {
			ClientIDHelper.EnableClientIDGeneration(this);
			this.isParentControlCallback = isParentControlCallback;
		}
		protected override IDictionary<string, bool> GetDrillDownKeys() {
			return RemoteMode && remoteDrillDownKeys != null
				? remoteDrillDownKeys
				: base.GetDrillDownKeys();
		}
		internal void AssignRemoteParameters(ReportParameter[] parameters, bool isApplicationCallback) {
			this.remoteParameters = parameters; 
			if(!isApplicationCallback) {
				AssignRemoteParametersToState(ReportWebMediator, parameters);
			}
		}
		internal void AssignRemoteDrillDownKeys(Dictionary<string, bool> drillDownKeys) {
			this.remoteDrillDownKeys = drillDownKeys;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientDocumentViewerReportViewer";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(RemoteMode) {
				stb.AppendLine(localVarName + ".shouldDisableSearchButton = true;");
			}
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = base.GetClientObjectState();
			result.Add(RemoteKey, string.Empty);
			return result;
		}
		internal void RaiseCallbackEventCore(string callbackArgs) {
			RaiseCallbackEvent(callbackArgs);
		}
		internal object GetCallbackResultCore() {
			return GetCallbackResult();
		}
		internal ITokenStorage CreateClientTokenStorage() {
			return new ClientTokenStorage(
				RSRemoteDocumentInformation,
				_ => ActionBuilder.AddAction("setStateObjectKey", RemoteKey, RSRemoteDocumentInformation.Serialize())
			);
		}
		protected override string FetchEmptyContent() {
			ActionBuilder.AddAction("removeContentPaddings");
			return base.FetchEmptyContent();
		}
		#region Callbacks
		protected override void RegisterCallbacks(Dictionary<string, ReportViewerCallback> callbacks) {
			if(ReportWebRemoteMediator == null) {
				base.RegisterCallbacks(callbacks);
				return;
			}
			callbacks["page"] = CallbackRemotePage;
			callbacks["print"] = CallbackRemotePrint;
			callbacks["bookmark"] = CallbackRemoteBookmark;
			callbacks[Constants.ReportViewer.SubmitParametersCallbackName] = CallbackRemotePage;
		}
		internal void EnsureRemoteDocumentInformation() {
			if(RemoteDocumentInformation == null) {
				RemoteDocumentInformation = BuildRemoteReport();
				SetRemoteDocumentSourceHiddenFieldValue();
			}
		}
		internal protected override string CallbackRemotePage() {
			EnsureRemoteDocumentInformation();
			ActionBuilder.AddAction("onPageLoad", RemoteDocumentInformation.PageCount);
			if(RemoteDocumentInformation.PageCount == 0) {
				return FetchEmptyContent();
			}
			var pageInformation = ReportWebRemoteMediator.GetPage(this, RemoteDocumentInformation, CurrentPageIndex);
			var size = pageInformation.PageSize;
			if(size.IsEmpty) {
				ActionBuilder.AddAction("setViewSize");
			} else {
				ActionBuilder.AddAction("setViewSize", size.Width, size.Height);
			}
			return pageInformation.HtmlContent;
		}
		string CallbackRemotePrint() {
			string pageIndexString = ReportWebMediator.Event.PageIndexString;
			int pageIndex;
			if(int.TryParse(pageIndexString, out pageIndex)) {
				return ReportWebRemoteMediator.GetPage(this, RemoteDocumentInformation, pageIndex).HtmlContent;
			}
			return ReportWebRemoteMediator.GetPagesForPrinting(this, RemoteDocumentInformation);
		}
		string CallbackRemoteBookmark() {
			int pageIndex = CurrentPageIndex;
			Content.AddPageLoadScript(pageIndex);
			Content.ActionBuilder.AddAction("HighlightBookmark", ReportWebMediator.Event.Path);
			return CallbackRemotePage();
		}
		#endregion
		protected override void PrepareUserControl(Control userControl, Control parent, string id, bool builtInControl) {
			base.PrepareUserControl(userControl, parent, id, builtInControl);
			if(IsMvcRender()) {
				ForceOnInit(userControl);
				ForceOnLoad(userControl);
			}
		}
		protected internal override void ForcePSDocument() {
			if(ReportWebRemoteMediator == null) {
				base.ForcePSDocument();
			}
		}
		protected override ReportWebMediator CreateReportWebMediator(XtraReport report, Hashtable reportParameters, Hashtable clientDrillDownKeys, Action<DeserializeClientParameterEventArgs> deserializeClientParameter) {
			return ReportWebRemoteMediator != null
				? new RemoteReportWebMediator(ReportWebRemoteMediator, reportParameters, clientDrillDownKeys, remoteParameters, deserializeClientParameter, GetDocumentIdForCreateReportWebMediator)
				: base.CreateReportWebMediator(report, reportParameters, clientDrillDownKeys, deserializeClientParameter);
		}
		DocumentId GetDocumentIdForCreateReportWebMediator() {
			if(RemoteDocumentInformation == null || RemoteDocumentInformation.DocumentId == null)
				throw new InvalidOperationException(ASPxReportsLocalizer.GetString(ASPxReportsStringId.DocumentViewer_NoRemoteDocumentInformation_Error));
			return RemoteDocumentInformation.DocumentId;
		}
		RemoteDocumentInformation BuildRemoteReport() {
			IEnumerable<ReportParameter> parameters = ReportWebMediator.IsParametersInfoAssigned
				? ReportWebMediator.ParametersInfo
					.Select(x => new ReportParameter { Path = x.Path, Value = x.Value })
				: ReportWebMediator.ClientParameterValues
					.Select(x => new ReportParameter { Path = x.Key, Value = x.Value });
			var buildArgs = new ReportBuildArgs {
				Parameters = parameters.ToArray(),
				DrillDownKeys = ReportWebMediator.GetDrillDownKeys()
			};
			return ReportWebRemoteMediator.CreateDocument(buildArgs);
		}
		internal void SetRemoteDocumentSourceHiddenFieldValue() {
			ActionBuilder.AddAction("setStateObjectKey", RemoteKey, RSRemoteDocumentInformation.Serialize());
		}
		protected void ForceOnInit(Control control) {
			var element = control as IDialogFormElementRequiresLoad;
			if(element != null) {
				element.ForceInit();
			}
			foreach(Control childControl in control.Controls) {
				ForceOnInit(childControl);
			}
		}
		protected void ForceOnLoad(Control control) {
			var element = control as IDialogFormElementRequiresLoad;
			if(element != null) {
				element.ForceLoad();
			}
			foreach(Control childControl in control.Controls) {
				ForceOnLoad(childControl);
			}
		}
		static void AssignRemoteParametersToState(ReportWebMediator reportWebMediator, ReportParameter[] parameters) {
			reportWebMediator.ParametersInfo = parameters
				.Select(x => new ASPxParameterInfo(GenerateParameterPathFromRemoteParameter(x), null))
				.ToArray();
		}
		static ParameterPath GenerateParameterPathFromRemoteParameter(ReportParameter reportParameter) {
			var value = reportParameter.Value;
			var type = value != null ? value.GetType() : typeof(string);
			var parameter = new Parameter {
				Name = reportParameter.Name,
				Type = type,
				Value = value
			};
			return new ParameterPath(parameter, reportParameter.Path);
		}
	}
}
