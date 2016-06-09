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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.XtraReports.UI;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Native {
	public static class EmbeddedFieldsHelper {
		public delegate string ProcessDataMember(string dataMember, string trimmedFieldName);
		public static bool HasEmbeddedFieldNames(XRFieldEmbeddableControl control) {
			return (control == null) ? false : GetEmbeddedFieldInfos(control).Count > 0;
		}
		public static MailMergeFieldInfoCollection GetEmbeddedFieldInfos(XRFieldEmbeddableControl control) {
			return (control == null || control.Report == null)
				? new MailMergeFieldInfoCollection()
				: GetEmbeddedFieldInfos(control.Report, control.GetMailMergeFieldInfosCalculator(), control.GetSavedText());
		}
		public static string GetTextFromTextWithDisplayColumnNames(XtraReportBase report, MailMergeFieldInfosCalculator calculator, string source) {
			return GetTextFromTextWithDisplayColumnNames(report, report.GetEffectiveDataSource(), report.DataMember, calculator, source);
		}
		public static string GetTextFromTextWithDisplayColumnNames(XtraReportBase report, object dataSource, string dataMember, MailMergeFieldInfosCalculator calculator, string source) {
			if(report == null)
				return string.Empty;
			using(DataContextProvider provider = new DataContextProvider(report)) {
				MailMergeFieldInfoCollection embeddedFields = GetEmbeddedFieldInfosByDisplayNames(provider.DataContext, dataSource, dataMember, calculator, source);
				return AssembleStringFromEmbeddedFields(source, embeddedFields, PrepareNamesAsValues(dataMember, calculator, embeddedFields));
			}
		}
		static Collection<string> PrepareNamesAsValues(string dataMember, MailMergeFieldInfosCalculator calculator, MailMergeFieldInfoCollection embeddedFileds) {
			Collection<string> values = new Collection<string>();
			foreach(MailMergeFieldInfo mailMergeFieldInfo in embeddedFileds) {
				string wrappedColumnName = GetWrappedColumnName(dataMember, mailMergeFieldInfo);
				string preparedWrappedColumnName = calculator.PrepareString(wrappedColumnName);
				values.Add(preparedWrappedColumnName);
			}
			return values;
		}
		public static string GetColumnName(string reportDataMember, string dataMember) {
			if(string.IsNullOrEmpty(reportDataMember) || !string.IsNullOrEmpty(ParametersReplacer.GetParameterName(dataMember)))
				return dataMember;
			else {
				string prefix = reportDataMember + ".";
				if(dataMember.StartsWith(prefix))
					return dataMember.Remove(0, prefix.Length);
			}
			return string.Empty;
		}
		public static string GetTextWithDisplayColumnNames(XtraReportBase report, MailMergeFieldInfosCalculator calculator, string text) {
			if(report == null)
				return string.Empty;
			MailMergeFieldInfoCollection embeddedFields = GetEmbeddedFieldInfos(report, calculator, text);
			return GetTextWithDisplayColumnNames(text, embeddedFields, calculator);
		}
		public static string GetTextWithDisplayColumnNames(XtraReportBase report, object dataSource, string dataMember, MailMergeFieldInfosCalculator calculator, string text) {
			MailMergeFieldInfoCollection embeddedFields = GetEmbeddedFieldInfos(report, dataSource, dataMember, calculator, text, (dataMember1, trimmedFieldName) => GetDataMember(dataMember1, trimmedFieldName));
			return GetTextWithDisplayColumnNames(text, embeddedFields, calculator);
		}
		public static string GetDataMember(string dataMember, string fieldName) {
			return string.IsNullOrEmpty(dataMember)
				? fieldName
				: string.Format("{0}.{1}", dataMember, fieldName);
		}
		public static bool IsFieldNameValid(XtraReportBase report, string fieldName, object dataSource, string dataMember) {
			using(DataContextProvider provider = new DataContextProvider(report)) {
				return provider.DataContext.IsDataMemberValid(dataSource, GetDataMember(dataMember, fieldName));
			}
		}
		public static string GetFieldDisplayName(XtraReportBase report, string dataMember, object dataSource, string fieldName) {
			using(DataContextProvider provider = new DataContextProvider(report)) {
				dataMember = string.IsNullOrEmpty(dataMember) ? report.DataMember : dataMember;
				return GetFieldDisplayName(provider.DataContext, dataSource, dataMember, fieldName);
			}
		}
		static string GetFieldDisplayName(XRDataContextBase dataContext, object dataSource, string dataMember, string fieldName) {
			string fieldDisplayName = dataContext.GetDataMemberDisplayName(dataSource, dataMember, fieldName);
			return string.IsNullOrEmpty(fieldDisplayName) ? fieldName : fieldDisplayName;
		}
		public static string GetShortFieldDisplayName(XRDataContextBase dataContext, object dataSource, string dataMember, string fieldName) {
			string fullFieldName = DevExpress.Data.Native.BindingHelper.JoinStrings(".", dataMember, fieldName);
			string fieldDisplayName = dataContext.GetDataMemberDisplayName(dataSource, dataMember, fullFieldName);
			return string.IsNullOrEmpty(fieldDisplayName) ? fieldName : fieldDisplayName;
		}
		public static string GetShortFieldRealName(XRDataContextBase dataContext, object dataSource, string dataMember, string displayName) {
			string realName = dataContext.GetDataMemberFromDisplayName(dataSource, dataMember, displayName);
			if(string.IsNullOrEmpty(realName))
				return displayName;
			return EmbeddedFieldsHelper.GetColumnName(dataMember, realName);
		}
		public static string AssembleStringFromEmbeddedFields(string source, MailMergeFieldInfoCollection embeddedFields, IList<string> values) {
			Guard.ArgumentNotNull(source, "source");
			StringBuilder resultBuilder = new StringBuilder();
			MailMergeFieldInfo mailMergeFieldInfo;
			int previousColumnEndIndex = 0;
			for(int i = 0; i < embeddedFields.Count; i++) {
				mailMergeFieldInfo = embeddedFields[i];
				resultBuilder.Append(source.Substring(previousColumnEndIndex, mailMergeFieldInfo.StartPosition - previousColumnEndIndex));
				resultBuilder.Append(values[i]);
				previousColumnEndIndex = mailMergeFieldInfo.EndPosition + 1;
			}
			resultBuilder.Append(source.Substring(previousColumnEndIndex));
			return resultBuilder.ToString();
		}
		public static void GetSelectionProperties(string text, int charIndex, out int selectionStart, out int selectionLength) {
			MailMergeFieldInfoCollection bracketPairs = PlainTextMailMergeFieldInfosCalculator.Instance.CalcMailMergeFieldInfos(text);
			foreach(MailMergeFieldInfo pair in bracketPairs) {
				if(pair.StartPosition <= charIndex && charIndex <= pair.EndPosition) {
					selectionStart = pair.StartPosition;
					selectionLength = pair.EndPosition - pair.StartPosition + 1;
					return;
				}
			}
			selectionStart = charIndex;
			selectionLength = 0;
		}
		public static void ChangeBinding(XRFieldEmbeddableControl control, string dataMember) {
			MailMergeFieldInfoCollection columns = EmbeddedFieldsHelper.GetEmbeddedFieldInfos(control);
			if(columns.Count > 1)
				return;
			string displayColumnName = GetDisplayColumnName(control.Report, dataMember);
			displayColumnName = (columns[0].ToString().IndexOf(".") > 0 && dataMember.LastIndexOf(".") > 0)
				? dataMember.Substring(0, dataMember.LastIndexOf(".") + 1) + displayColumnName
				: displayColumnName;
			string columnInfoInBrackets = MailMergeFieldInfo.WrapColumnInfoInBrackets(displayColumnName, string.Empty);
			control.Text = (columns.Count == 0)
				? columnInfoInBrackets
				: control.Text.Replace(columns[0].ToString(), columnInfoInBrackets);
		}
		public static void ChangeFormatString(XRFieldEmbeddableControl control, string formatString) {
			MailMergeFieldInfoCollection columns = EmbeddedFieldsHelper.GetEmbeddedFieldInfos(control);
			if(columns.Count != 1)
				return;
			string clearFormatString = GetClearFormatString(formatString);
			string columnInfoInBrackets = MailMergeFieldInfo.WrapColumnInfoInBrackets(columns[0].DisplayName, clearFormatString);
			control.Text = control.Text.Replace(columns[0].ToString(), columnInfoInBrackets);
		}
		public static string GetClearFormatString(string formatString) {
			return String.IsNullOrEmpty(formatString)
				? string.Empty
				: formatString.Substring(3, formatString.Length - 4);
		}
		public static string GetDisplayColumnName(XtraReportBase report, string dataMember) {
			ObjectNameCollection displayColumnNames = GetDisplayColumnNames(report.ReportCalculatedFields, report);
			return GetDisplayColumnName(displayColumnNames, dataMember);
		}
		internal static MailMergeFieldInfoCollection GetEmbeddedFieldInfos(XtraReportBase report, MailMergeFieldInfosCalculator calculator, string text) {
			return report == null ? 
				new MailMergeFieldInfoCollection() : 
				GetEmbeddedFieldInfos(report, report.GetEffectiveDataSource(), report.DataMember, calculator, text, (dataMember, trimmedFieldName) => trimmedFieldName);
		}
		internal static MailMergeFieldInfoCollection GetEmbeddedFieldInfos(XtraReportBase report, object dataSource, string dataMember, MailMergeFieldInfosCalculator calculator, string text, ProcessDataMember method) {
			MailMergeFieldInfoCollection mailMergeFieldInfosFromString = calculator.CalcMailMergeFieldInfos(text);
			MailMergeFieldInfoCollection result = new MailMergeFieldInfoCollection();
			foreach(MailMergeFieldInfo mailMergeFieldInfo in mailMergeFieldInfosFromString) {
				ProcessMailMergeFieldInfo(report, result, mailMergeFieldInfo, dataMember, method(dataMember, mailMergeFieldInfo.TrimmedFieldName), dataSource);
			}
			return result;
		}
		static string GetWrappedColumnName(string reportDataMember, MailMergeFieldInfo mailMergeFieldInfo) {
			string columnName = GetColumnName(reportDataMember, mailMergeFieldInfo.DataMember);
			if(string.IsNullOrEmpty(columnName) && mailMergeFieldInfo.DataMember.Contains("."))
				columnName = mailMergeFieldInfo.DataMember;
			string formatString = mailMergeFieldInfo.HasFormatStringInfo ? EmbeddedFieldsHelper.GetClearFormatString(mailMergeFieldInfo.FormatString) : string.Empty;
			return MailMergeFieldInfo.WrapColumnInfoInBrackets(columnName, formatString);
		}
		static string GetTextWithDisplayColumnNames(string text, MailMergeFieldInfoCollection embeddedFields, MailMergeFieldInfosCalculator calculator) {
			Collection<string> values = new Collection<string>();
			foreach(MailMergeFieldInfo mailMergeFieldInfo in embeddedFields) {
				string preparedMailMergeFieldInfo = calculator.PrepareString(mailMergeFieldInfo.ToString());
				values.Add(preparedMailMergeFieldInfo);
			}
			return AssembleStringFromEmbeddedFields(text, embeddedFields, values);
		}
		static void ProcessMailMergeFieldInfo(XtraReportBase report, MailMergeFieldInfoCollection result, MailMergeFieldInfo mailMergeFieldInfo, string dataMember, string fieldName, object dataSource) {
			if(IsFieldNameValid(report, mailMergeFieldInfo.TrimmedFieldName, dataSource, dataMember)) {
				fieldName = GetDataMember(dataMember, mailMergeFieldInfo.TrimmedFieldName);
				mailMergeFieldInfo.DisplayName = GetFieldDisplayName(report, dataMember, dataSource, fieldName);
			} else if(!string.IsNullOrEmpty(ParametersReplacer.GetParameterName(fieldName))) {
				fieldName = mailMergeFieldInfo.TrimmedFieldName;
			} else
				fieldName = report.GetDataMemberFromDisplayName(mailMergeFieldInfo.DisplayName);
			if(!string.IsNullOrEmpty(fieldName)) {
				mailMergeFieldInfo.DataMember = fieldName;
				result.Add(mailMergeFieldInfo);
			}
		}
		static MailMergeFieldInfoCollection GetEmbeddedFieldInfosByDisplayNames(XRDataContextBase dataContext, object dataSource, string dataMember, MailMergeFieldInfosCalculator calculator, string text) {
			MailMergeFieldInfoCollection mailMergeFieldInfosFromString = calculator.CalcMailMergeFieldInfos(text);
			MailMergeFieldInfoCollection result = new MailMergeFieldInfoCollection();
				foreach(MailMergeFieldInfo mailMergeFieldInfo in mailMergeFieldInfosFromString) {
					string dataMemberFromDisplayName = dataContext.GetDataMemberFromDisplayName(dataSource, dataMember, mailMergeFieldInfo.TrimmedFieldName);
					if(!string.IsNullOrEmpty(dataMemberFromDisplayName)) {
						mailMergeFieldInfo.DataMember = dataMemberFromDisplayName;
						result.Add(mailMergeFieldInfo);
				}
			}
			return result;
		}
		static ObjectNameCollection GetDisplayColumnNames(CalculatedFieldCollection calculatedFields, IDataContainerBase dataContainer) {
			using(XRDataContextBase dataContext = new XRDataContextBase(calculatedFields, true)) {
				return dataContext.GetItemDisplayNames(dataContainer.DataSource, dataContainer.DataMember);
			}
		}
		static string GetDisplayColumnName(ObjectNameCollection objectNames, string dataMember) {
			string columnName = (dataMember != null) ? dataMember.Substring(dataMember.LastIndexOf(".") + 1) : null;
			int index = objectNames.IndexOfByName(columnName);
			return (index >= 0) ? objectNames[index].DisplayName : string.Empty;
		}
	}
}
