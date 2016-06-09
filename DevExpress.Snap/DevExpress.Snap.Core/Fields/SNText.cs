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
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Model;
using System.Collections.Generic;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Data;
using DevExpress.Office.Utils;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.Snap.Core.API;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Import;
namespace DevExpress.Snap.Core.Fields {
	[ActionList("DevExpress.Snap.Extensions.Native.ActionLists.ContentTypeActionList," + AssemblyInfo.SRAssemblySnapExtensions, 0)]
	[ActionList("DevExpress.Snap.Extensions.Native.ActionLists.DataFieldNameActionList, " + AssemblyInfo.SRAssemblySnapExtensions, 1)]
	[ActionList("DevExpress.Snap.Extensions.Native.ActionLists.FormatStringActionList, " + AssemblyInfo.SRAssemblySnapExtensions, 2)]
	[ActionList("DevExpress.Snap.Extensions.Native.ActionLists.SummaryActionList, " + AssemblyInfo.SRAssemblySnapExtensions, 3)]
	[ActionList("DevExpress.Snap.Extensions.Native.ActionLists.TextFormatActionList, " + AssemblyInfo.SRAssemblySnapExtensions, 4)]
	public class SNTextField : SNMergeFieldSupportsEmptyFieldDataAlias {
		public static new readonly string FieldType = "SNTEXT";
		static readonly string NullTextFormat = "<<{0}>>";
		static readonly string NullSummaryValue = "N/A";
		public static readonly string SummaryRunningSwitch = "sr";
		public static readonly string SummaryFuncSwitch = "sf";
		public static readonly string ObsoleteSummariesIgnoreNullValuesSwitch = "sinv";
		public static readonly string SummariesIgnoreNullValuesSwitch = "suminv";
		public static readonly string ParameterSwitch = "p";
		public static readonly string TextFormatSwitch = "tf";
		public static readonly string KeepLastParagraphSwitch = "klp";
		public static readonly string AliasForEmptyFieldData = "<<NoData>>";
		internal static string GetSummaryFieldCode(string dataFieldName, string summaryRunning, SummaryItemType summaryItemType) {
			return String.Format("{0} {1} \\{2} {3} \\{4} {5}", SNTextField.FieldType, InstructionController.GetEscapedArgument(dataFieldName), SNTextField.SummaryRunningSwitch, summaryRunning, SNTextField.SummaryFuncSwitch, FieldsHelper.GetSummaryItemTypeCodeString(summaryItemType));
		}
		internal static string GetDisplaySummaryString(string displayDataFieldName, SummaryItemType summaryItemType) {
			return string.Format("{0}: {1} = ", displayDataFieldName, FieldsHelper.GetSummaryItemTypeDisplayString(summaryItemType));
		}
		public static new CalculatedFieldBase Create() {
			return new SNTextField();
		}
		delegate bool FieldValueSetter(DocumentModel targetModel, DocumentModel sourceModel, object content);
		static readonly Dictionary<string, bool> snTextSwitchesWithArgument;
		static readonly Dictionary<string, FieldValueSetter> fieldValueSetters;
		static SNTextField() {
			snTextSwitchesWithArgument = CreateSwitchesWithArgument(SummaryRunningSwitch, SummaryFuncSwitch, ObsoleteSummariesIgnoreNullValuesSwitch, TextFormatSwitch,EmptyFieldDataAliasSwitch);
			foreach (KeyValuePair<string, bool> sw in MergefieldField.SwitchesWithArgument)
				snTextSwitchesWithArgument.Add(sw.Key, sw.Value);
			fieldValueSetters = CreateFieldResultSetters();
		}
		static Dictionary<string, FieldValueSetter> CreateFieldResultSetters() {
			Dictionary<string, FieldValueSetter> result = new Dictionary<string, FieldValueSetter>();
			result.Add("rtf", InsertRftContent);
			result.Add("doc", InsertDocContent);
			result.Add("openxml", InsertOpenXmlContent);
			result.Add("html", InsertHtmlContent);
			result.Add("mht", InsertMhtContent);
			result.Add("wordml", InsertWordMLContent);
			result.Add("opendocument", InsertOpenDocumentContent);
			return result;
		}
		#region field result setters
		static bool InsertRftContent(DocumentModel targetModel, DocumentModel sourceModel, object content) {
			RtfDocumentImporterOptions options = new RtfDocumentImporterOptions();
			return InsertDocumentContent<string, RtfDocumentImporterOptions>(content, sourceModel, options, targetModel.InternalAPI.SetDocumentRtfContent);
		}
		static bool InsertDocContent(DocumentModel targetModel, DocumentModel sourceModel, object content) {
			DocDocumentImporterOptions options = new DocDocumentImporterOptions();
			return InsertDocumentContent<byte[], DocDocumentImporterOptions>(content, sourceModel, options, targetModel.InternalAPI.SetDocumentDocContent);
		}
		static bool InsertHtmlContent(DocumentModel targetModel, DocumentModel sourceModel, object content) {
			HtmlDocumentImporterOptions options = new HtmlDocumentImporterOptions();
			return InsertDocumentContent<string, HtmlDocumentImporterOptions>(content, sourceModel, options, targetModel.InternalAPI.SetDocumentHtmlContent);
		}
		static bool InsertOpenXmlContent(DocumentModel targetModel, DocumentModel sourceModel, object content) {
			OpenXmlDocumentImporterOptions options = new OpenXmlDocumentImporterOptions();
			return InsertDocumentContent<byte[], OpenXmlDocumentImporterOptions>(content, sourceModel, options, targetModel.InternalAPI.SetDocumentOpenXmlContent);
		}
		static bool InsertMhtContent(DocumentModel targetModel, DocumentModel sourceModel, object content) {
			MhtDocumentImporterOptions options = new MhtDocumentImporterOptions();
			return InsertDocumentContent<string, MhtDocumentImporterOptions>(content, sourceModel, options, targetModel.InternalAPI.SetDocumentMhtContent);
		}
		static bool InsertWordMLContent(DocumentModel targetModel, DocumentModel sourceModel, object content) {
			WordMLDocumentImporterOptions options = new WordMLDocumentImporterOptions();
			return InsertDocumentContent<string, WordMLDocumentImporterOptions>(content, sourceModel, options, targetModel.InternalAPI.SetDocumentWordMLContent);
		}
		static bool InsertOpenDocumentContent(DocumentModel targetModel, DocumentModel sourceModel, object content) {
			OpenDocumentImporterOptions options = new OpenDocumentImporterOptions();
			return InsertDocumentContent<byte[], OpenDocumentImporterOptions>(content, sourceModel, options, targetModel.InternalAPI.SetDocumentOpenDocumentContent);
		}
		static bool InsertDocumentContent<T, U>(object content, DocumentModel sourceModel, U options, Action<T, U> contentSetter)
			where T : class
			where U : DocumentImporterOptions {
			T typifiedContent = content as T;
			if (typifiedContent != null) {
				try {
					DocumentImporterOptions defaultOptions = sourceModel.DocumentImportOptions.GetOptions(options.Format);
					if (defaultOptions != null)
						options.CopyFrom(defaultOptions);
					contentSetter(typifiedContent, options);
					return true;
				}
				catch {
				}
			}
			return false;
		}
		#endregion
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return snTextSwitchesWithArgument; } }
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			if (IsParameter) {
				Parameter parameter = ((SnapDocumentModel)sourcePieceTable.DocumentModel).Parameters[DataFieldName];
				if (parameter != null)
					return TryFormatFieldValue(sourcePieceTable, new CalculatedFieldValue(parameter.Value));
			}
			if (SummaryRunning != Fields.SummaryRunning.None) {
				IFieldDataAccessService fieldDataAccessService = sourcePieceTable.DocumentModel.GetService<IFieldDataAccessService>();
				if (fieldDataAccessService == null)
					return null;
				int groupLevel = -1;
				if (SummaryRunning == Fields.SummaryRunning.Group)
					groupLevel = GetGroupLevel(sourcePieceTable, documentField);
				FieldDataValueDescriptor descriptor = fieldDataAccessService.GetFieldValueDescriptor((SnapPieceTable)sourcePieceTable, documentField, this.DataFieldName);
				ICalculationContext calculationContext = fieldDataAccessService.FieldContextService.BeginCalculation(((SnapDocumentModel)sourcePieceTable.DocumentModel).DataSourceDispatcher);
				try {
					CalculatedFieldValue summaryResult = new CalculatedFieldValue(calculationContext.GetSummaryValue(descriptor.ParentDataContext, descriptor.RelativePath, SummaryRunning, SummaryFunc, SummariesIgnoreNullValues, groupLevel));
					if (summaryResult.RawValue == null)
						return new CalculatedFieldValue(NullSummaryValue);
					return summaryResult;
				}
				finally {
					fieldDataAccessService.FieldContextService.EndCalculation(calculationContext);
				}
			}
			CalculatedFieldValue result = base.GetCalculatedValueCore(sourcePieceTable, mailMergeDataMode, documentField);
			if (result.RawValue == FieldNull.Value) {
				if (sourcePieceTable.DocumentModel.ModelForExport)
					Exceptions.ThrowInternalException();
				return new CalculatedFieldValue(String.Format(NullTextFormat, DataFieldName)); 
			}
			return TryFormatFieldValue(sourcePieceTable, result);
		}
		CalculatedFieldValue TryFormatFieldValue(PieceTable sourcePieceTable, CalculatedFieldValue result) {
			if (!String.IsNullOrEmpty(TextFormat) && fieldValueSetters.ContainsKey(TextFormat)) {
				DocumentModel targetModel = sourcePieceTable.DocumentModel.GetFieldResultModel();
				targetModel.MailMergeOptions.KeepLastParagraph = KeepLastParagraph;
				targetModel.InheritService<DevExpress.Office.Services.IUriStreamService>(sourcePieceTable.DocumentModel);
				FieldValueSetter setter = fieldValueSetters[TextFormat];
				if (setter(targetModel, sourcePieceTable.DocumentModel, result.RawValue))
					return new CalculatedFieldValue(targetModel, FieldResultOptions.DoNotApplyFieldCodeFormatting | FieldResultOptions.SuppressMergeUseFirstParagraphStyle);
			}
			return result;
		}
		protected override CalculatedFieldValue GetNullValue() {
			if (EnableEmptyFieldDataAlias)
				return new CalculatedFieldValue(EmptyFieldDataAlias);
			else
				return new CalculatedFieldValue(String.Empty);
		}
		int GetGroupLevel(PieceTable sourcePieceTable, Field documentField) {
			SnapTemplateInfo templateInfo = GetTemplateInfo(sourcePieceTable, documentField);
			if (templateInfo == null)
				return -1;
			SnapTemplateIntervalType snapTemplateIntervalType = templateInfo.TemplateType;
			if (snapTemplateIntervalType == SnapTemplateIntervalType.GroupHeader || snapTemplateIntervalType == SnapTemplateIntervalType.GroupFooter) {
				return templateInfo.LastGroupIndex;
			}
			return 0;
		}
		private SnapTemplateInfo GetTemplateInfo(PieceTable sourcePieceTable, Field documentField) {
			if (!sourcePieceTable.DocumentModel.ModelForExport) {
				SnapBookmarkController snapBookmarkController = new SnapBookmarkController((SnapPieceTable)sourcePieceTable);
				SnapBookmark snapBookmark = snapBookmarkController.FindInnermostTemplateBookmarkByPosition(documentField.FirstRunIndex);
				if (snapBookmark != null)
					return snapBookmark.TemplateInterval.TemplateInfo;
			}
			else {
				IExportService service = sourcePieceTable.DocumentModel.GetService<IExportService>();
				if (service != null)
					return service.TemplateInfo;
			}
			return null;
		}
		public SummaryRunning SummaryRunning { get; private set; }
		public SummaryItemType SummaryFunc { get; private set; }
		public bool SummariesIgnoreNullValues { get; private set; }
		public bool IsParameter { get; private set; }
		public string TextFormat { get; private set; }
		public bool KeepLastParagraph { get; private set; }
		public override void Initialize(PieceTable pieceTable, InstructionCollection instructions) {
			base.Initialize(pieceTable, instructions);
			SetSummaryRunning(instructions);
			SetSummaryFunc(instructions);
			SetSummariesIgnoreNullValues(instructions);
			SetIsParameter(instructions);
			SetTextFormat(instructions);
			SetKeepLastParagraph(instructions);
		}
		void SetKeepLastParagraph(InstructionCollection instructions) {
			KeepLastParagraph = instructions.GetBool(KeepLastParagraphSwitch);
		}
		void SetSummariesIgnoreNullValues(InstructionCollection instructions) {
			SummariesIgnoreNullValues = instructions.GetBool(SNTextField.SummariesIgnoreNullValuesSwitch);
			if (!SummariesIgnoreNullValues) {
				string summariesIgnoreNullValuesString = instructions.GetString(ObsoleteSummariesIgnoreNullValuesSwitch);
				if (!string.IsNullOrEmpty(summariesIgnoreNullValuesString))
					SummariesIgnoreNullValues = Convert.ToBoolean(summariesIgnoreNullValuesString);
			}
		}
		void SetSummaryRunning(InstructionCollection instructions) {
			string summaryRunningString = instructions.GetString(SummaryRunningSwitch);
			if (!string.IsNullOrEmpty(summaryRunningString))
				SummaryRunning = (SummaryRunning)Enum.Parse(typeof(SummaryRunning), summaryRunningString, true);
		}
		void SetSummaryFunc(InstructionCollection instructions) {
			string summaryFuncString = instructions.GetString(SummaryFuncSwitch);
			if (!string.IsNullOrEmpty(summaryFuncString))
				SummaryFunc = (SummaryItemType)Enum.Parse(typeof(SummaryItemType), summaryFuncString, true);
		}
		void SetIsParameter(InstructionCollection instructions) {
			IsParameter = instructions.Switches.ContainsKey("\\" + ParameterSwitch);
		}
		void SetTextFormat(InstructionCollection instructions) {
			string fieldResultFormatString = instructions.GetString(TextFormatSwitch);
			if (!String.IsNullOrEmpty(fieldResultFormatString))
				TextFormat = fieldResultFormatString.ToLowerInvariant();
		}
		protected internal override string[] GetNativeSwithes() {
			return new string[] {
				SummaryRunningSwitch,
				SummaryFuncSwitch,
				ObsoleteSummariesIgnoreNullValuesSwitch,
				ParameterSwitch,
				TextFormatSwitch,
				KeepLastParagraphSwitch,
				EmptyFieldDataAliasSwitch,
				EnableEmptyFieldDataAliasSwitch
			};
		}
		protected internal override string[] GetInvariableSwitches() {
			return new string[] {
				EmptyFieldDataAliasSwitch,
				EnableEmptyFieldDataAliasSwitch
			};
		}
	}
	public enum SummaryRunning {
		None = 0,
		Group = 1,
		Report = 2,
	}
}
