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
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using DevExpress.Data.Utils;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native.DrillDown;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.DrillDown;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Native.ParametersPanel;
using ParameterValueTypeHelper = DevExpress.XtraPrinting.Native.ParameterValueTypeHelper;
using XRParameter = DevExpress.XtraReports.Parameters.Parameter;
namespace DevExpress.XtraReports.Web.Native {
	public class ReportWebMediator {
		const string ChangedParameterName = "$changed";
#if DEBUGTEST
		public static ReportWebMediator CreateWithoutConverter_TEST(XtraReport report, Hashtable parameters) {
			return new ReportWebMediator(report, parameters, null, x => { });
		}
#endif
		static readonly ClientParameterValueNormalizer clientParameterValueNormalizer = new ClientParameterValueNormalizer();
		readonly bool hasClientParametersValues;
		readonly Hashtable clientDrillDownKeys;
		XtraReport report;
		WebEventInfo webEventInfo;
		ASPxParameterInfo[] parametersInfo;
		public bool HasEventInfo {
			get { return webEventInfo != null; }
		}
		internal WebEventInfo Event {
			get { return webEventInfo; }
		}
		public bool FileImageCache {
			get { return HasEventInfo && Event.IsPrintEvent; }
		}
		internal bool IsParametersInfoAssigned { get; private set; }
		internal ASPxParameterInfo[] ParametersInfo {
			get {
				if(parametersInfo == null) {
					if(!IsParametersInfoAssigned) {
						if(report == null) {
							return new ASPxParameterInfo[0];
						}
						parametersInfo = new NestedParameterPathCollector()
							.EnumerateParameters(report)
							.Select(x => new ASPxParameterInfo(x, report))
							.ToArray();
					}
				}
				return parametersInfo;
			}
			set {
				parametersInfo = value;
				IsParametersInfoAssigned = true;
			}
		}
		public Dictionary<string, object> ClientParameterValues { get; private set; }
		public bool ClientParametersWereChanged { get; private set; }
		protected Hashtable ClientDrillDownKeys {
			get { return clientDrillDownKeys; }
		}
		public ReportWebMediator(XtraReport report, string eventArgument, Hashtable parameters, Hashtable drillDownKeys, Action<DeserializeClientParameterEventArgs> deserializeClientParameter)
			: this(report, eventArgument, parameters, drillDownKeys, deserializeClientParameter, null) {
		}
		public ReportWebMediator(XtraReport report, string eventArgument, Hashtable clientParameters, Hashtable clientDrillDownKeys, Action<DeserializeClientParameterEventArgs> deserializeClientParameter, Func<string, WebParameterTypeInfo> getParameterType)
			: this(report, clientParameters, clientDrillDownKeys, deserializeClientParameter, getParameterType) {
			InitEventInfo(eventArgument);
		}
		public ReportWebMediator(XtraReport report, Hashtable clientParameters, Hashtable clientDrillDownKeys, Action<DeserializeClientParameterEventArgs> deserializeClientParameter)
			: this(report, clientParameters, clientDrillDownKeys, deserializeClientParameter, null) {
		}
		public ReportWebMediator(XtraReport report, Hashtable clientParameters, Hashtable clientDrillDownKeys, Action<DeserializeClientParameterEventArgs> deserializeClientParameter, Func<string, WebParameterTypeInfo> getParameterType) {
			Guard.ArgumentNotNull(deserializeClientParameter, "deserializeClientParameter");
			if(getParameterType == null && report != null) {
				getParameterType = new LocalParameterTypeProvider(report).GetParameterType;
			}
			this.report = report ?? new XtraReport();
			hasClientParametersValues = clientParameters != null;
			ClientParameterValues = hasClientParametersValues
				? ConvertParameterValues(clientParameters, deserializeClientParameter, getParameterType)
				: new Dictionary<string, object>();
			object changedObject;
			if(ClientParameterValues.TryGetValue(ChangedParameterName, out changedObject)) {
				ClientParameterValues.Remove(ChangedParameterName);
				ClientParametersWereChanged = changedObject is bool && (bool)changedObject;
			}
			this.clientDrillDownKeys = clientDrillDownKeys;
		}
		public void InitEventInfo(string eventArgument) {
			webEventInfo = WebEventInfo.Create(eventArgument);
		}
		public virtual void AssignClientStateToReport() {
			AssignClientParametersToReport(report, ClientParameterValues, ParametersInfo);
			AssignClientDrillDownKeysToReport(report, clientDrillDownKeys);
		}
		public Dictionary<string, object> GetParameterValues() {
			return hasClientParametersValues
				? ClientParameterValues
				: ParametersInfo.ToDictionary(x => x.Path, x => x.Value);
		}
		public virtual Dictionary<string, bool> GetDrillDownKeys() {
			if(report == null) {
				return new Dictionary<string, bool>();
			}
			var drillDownService = report.GetService<IDrillDownService>();
			return drillDownService != null
				? drillDownService.GetSerializableState()
				: new Dictionary<string, bool>();
		}
		public Task<ExportStreamInfo> GetExportInfoAsync() {
			var documentFormat = Event.Format;
			var responseContentDisposition = Event.IsSaveToWindowEvent ? DispositionTypeNames.Inline : DispositionTypeNames.Attachment;
			var options = GenerateExportOptions(documentFormat);
			if(!string.IsNullOrEmpty(Event.ShowPrintDialog)) {
				var pdfExportOptions = (PdfExportOptions)options;
				pdfExportOptions.ShowPrintDialogOnOpen = true;
				responseContentDisposition = DispositionTypeNames.Inline;
				int pageNumber = 0;
				if(int.TryParse(Event.PageIndexString, out pageNumber)) {
					pageNumber += 1;
					if(pageNumber > 0) {
						pdfExportOptions.PageRange = pageNumber.ToString(CultureInfo.InvariantCulture);
					}
				}
			}
			return GenerateExportInfoAsync(options, documentFormat, responseContentDisposition);
		}
		internal void AssignReport(XtraReport report) {
			this.report = report;
		}
		protected virtual ExportOptionsBase GenerateExportOptions(string documentFormat) {
			return ExportStreamCache.CreateExportOptions(report.ExportOptions, documentFormat, report.PrintingSystem.Document.IsModified);
		}
		protected virtual Task<ExportStreamInfo> GenerateExportInfoAsync(ExportOptionsBase options, string documentFormat, string responseContentDisposition) {
			if(report == null) {
				throw new InvalidOperationException("Report instance is not assigned.");
			}
			var psProvider = new PrintingSystemProvider(report.PrintingSystem);
			var tcs = new TaskCompletionSource<ExportStreamInfo>();
			var result = ExportStreamCache.CreateExportStreamInfo(psProvider, options, documentFormat, responseContentDisposition);
			tcs.SetResult(result);
			return tcs.Task;
		}
		static Dictionary<string, object> ConvertParameterValues(Hashtable parametersValues, Action<DeserializeClientParameterEventArgs> deserializeClientParameter, Func<string, WebParameterTypeInfo> getParameterTypeInfo) {
			if(parametersValues == null || parametersValues.Count == 0) {
				return new Dictionary<string, object>();
			}
			return parametersValues
				.Cast<DictionaryEntry>()
				.ToDictionary(
					e => (string)e.Key,
					e => DeserializeClientParameterCore((string)e.Key, e.Value, deserializeClientParameter, getParameterTypeInfo));
		}
		static object DeserializeClientParameterCore(string path, object value, Action<DeserializeClientParameterEventArgs> deserializeClientParameter, Func<string, WebParameterTypeInfo> getParameterTypeInfo) {
			Guard.ArgumentNotNull(path, "path");
			Guard.ArgumentNotNull(deserializeClientParameter, "deserializeClientParameter");
			if(path == ChangedParameterName) { 
				return value;
			}
			var name = path.Split('.').LastOrDefault();
			var args = new DeserializeClientParameterEventArgs(name, path, value);
			deserializeClientParameter(args);
			value = args.Value;
			if(getParameterTypeInfo == null) {
				return value;
			}
			WebParameterTypeInfo parameterTypeInfo = getParameterTypeInfo(path);
			return clientParameterValueNormalizer.NormalizeSafe(value, parameterTypeInfo.Type, parameterTypeInfo.MultiValue);
		}
		static void AssignParameter(ASPxParameterInfo parameterInfo, object clientValue) {
			var parameter = parameterInfo.Parameter;
			parameter.Value = clientValue;
			if(ShouldAssignValueInfo(parameter.Type, clientValue, parameterInfo.Parameter.MultiValue)) {
				parameter.ValueInfo = parameterInfo.Parameter.MultiValue
					? EscapeAndJoinMultiValueString((IEnumerable)clientValue)
					: (string)clientValue;
			}
		}
		static string EscapeAndJoinMultiValueString(IEnumerable values) {
			const string MultiValueParameterSeparator = "|";
			var escapedStrings = values
				.Cast<string>()
				.Select(x => x.Replace("\\", "\\\\"))
				.Select(x => x.Replace(MultiValueParameterSeparator, "\\" + MultiValueParameterSeparator));
			return string.Join(MultiValueParameterSeparator, escapedStrings);
		}
		static void AssignClientParametersToReport(XtraReport report, Dictionary<string, object> clientParameterValues, ASPxParameterInfo[] parametersInfo) {
			if(report == null
				|| clientParameterValues == null || clientParameterValues.Count == 0
				|| parametersInfo == null || parametersInfo.Length == 0) {
				return;
			}
			Dictionary<string, XRParameter> parametersByPaths = new NestedParameterPathCollector().GetParametersAsDictionary(report);
			foreach(ASPxParameterInfo info in parametersInfo) {
				info.Update(parametersByPaths, report);
				object clientParameterValue;
				if(clientParameterValues.TryGetValue(info.Path, out clientParameterValue)) {
					AssignParameter(info, clientParameterValue);
				}
			}
		}
		static void AssignClientDrillDownKeysToReport(XtraReport report, Hashtable clientDrillDownKeys) {
			if(report == null || clientDrillDownKeys == null) {
				return;
			}
			var drillDownService = report.GetService<IDrillDownService>();
			if(drillDownService == null) {
				return;
			}
			foreach(DictionaryEntry pair in clientDrillDownKeys) {
				var key = DrillDownKey.Parse((string)pair.Key);
				drillDownService.Keys.Add(key, (bool)pair.Value);
			}
			if(clientDrillDownKeys.Count > 0) {
				drillDownService.IsDrillDowning = true;
			}
		}
		static bool ShouldAssignValueInfo(Type originalParameterType, object clientValue, bool multiValue) {
			return originalParameterType != typeof(string)
				&& originalParameterType != typeof(decimal)
				&& originalParameterType != typeof(Guid)
				&& Type.GetTypeCode(originalParameterType) == TypeCode.Object
				&& clientValue != null
				&& ParameterValueTypeHelper.GetValueType(clientValue, multiValue) == typeof(string);
		}
	}
}
