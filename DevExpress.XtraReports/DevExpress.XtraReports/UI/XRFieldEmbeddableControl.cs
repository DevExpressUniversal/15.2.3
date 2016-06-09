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
using System.Drawing;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.XtraReports.UI {
	public class XRFieldEmbeddableControl : XRControl, IDisplayNamePropertyContainer {
		#region Fields & Properties
		MailMergeFieldInfoCollection embeddedFields;
		XRBinding textBinding;
		string savedText;
		bool restoreSavedTextNeeded;
		bool disposed;
		internal virtual bool HasDisplayDataBinding {
			get { return DataBindings[DisplayPropertyName] != null; }
		}
		protected virtual string DisplayProperty {
			get { return Text; }
			set { Text = value; }
		}
		protected virtual string DisplayPropertyName {
			get { return "Text"; }
		}
		protected override TextEditMode TextEditMode {
			get { return (DataBindings[XRComponentPropertyNames.Text] == null) ? TextEditMode.Plain : TextEditMode.None; }
		}
		bool HasEmbeddedFieldNames {
			get { return (embeddedFields != null) && embeddedFields.Count > 0; }
		}
		[TypeConverter(typeof(DevExpress.XtraReports.Design.TextPropertyTypeConverter))]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		#endregion
		#region Constructors
		public XRFieldEmbeddableControl() {
		}
		internal XRFieldEmbeddableControl(Rectangle bounds)
			: base(bounds) {
		}
		#endregion
		#region Methods
#if DEBUGTEST
		public
#else
		internal 
#endif
		string GetTextPropertyWithDisplayColumnNames() {
			return EmbeddedFieldsHelper.GetTextWithDisplayColumnNames(Report, PlainTextMailMergeFieldInfosCalculator.Instance, Text);
		}
#if DEBUGTEST
		public
#else
		internal 
#endif
		void SetTextFromTextWithDisplayColumnNames(string source) {
			Text = GetTextFromTextWithDisplayColumnNames(source, PlainTextMailMergeFieldInfosCalculator.Instance);
		}
		internal virtual void SetDisplayPropertyFromTextWithDisplayColumnNames(string source) {
			DisplayProperty = GetTextFromTextWithDisplayColumnNames(source, GetMailMergeFieldInfosCalculator());
		}
		internal string GetDisplayPropertyWithDisplayColumnNames() {
			return EmbeddedFieldsHelper.GetTextWithDisplayColumnNames(Report, GetMailMergeFieldInfosCalculator(), DisplayProperty);
		}
		string GetTextFromTextWithDisplayColumnNames(string source, MailMergeFieldInfosCalculator calculator) {
			return EmbeddedFieldsHelper.GetTextFromTextWithDisplayColumnNames(Report, calculator, source);
		}
		internal Dictionary<MailMergeFieldInfoValue, string> GetEmbeddedFieldValuesHT() {
			Dictionary<MailMergeFieldInfoValue, string> embeddedFieldsHT = new Dictionary<MailMergeFieldInfoValue, string>();
			foreach(MailMergeFieldInfo mailMergeFieldInfo in embeddedFields) {
				if(embeddedFieldsHT.ContainsKey(new MailMergeFieldInfoValue(mailMergeFieldInfo))) continue;
				string stringValue = string.Empty;
				object value = null;
				DataBrowser dataBrowser = Report.DataContext.GetDataBrowser(Report.DataSource, mailMergeFieldInfo.DataMember, true);
				if(dataBrowser != null) {
					try {
						value = dataBrowser.GetValue();
					} catch { }
				}
				if(value == null) {
					Parameter parameter = GetParameterByDataMember(mailMergeFieldInfo.DataMember);
					if(parameter != null)
						value = parameter.Value;
				}
				try {
					stringValue = GetStringValue(value, mailMergeFieldInfo);
				} catch { }
				embeddedFieldsHT[new MailMergeFieldInfoValue(mailMergeFieldInfo)] = stringValue;
			}
			return embeddedFieldsHT;
		}
		internal string GetSavedText() {
			return (String.IsNullOrEmpty(savedText) || HasDisplayDataBinding) ? DisplayProperty : savedText;
		}
		protected void SetDesignerBrickText(VisualBrick brick) {
			if(EmbeddedFieldsHelper.HasEmbeddedFieldNames(this)) {
				brick.Text = GetDisplayPropertyWithDisplayColumnNames();
			}
			if(ShowBindingString(brick.Text)) {
				brick.Text = GetDisplayPropertyBindingString();
				BrickStringFormat source = brick.Style.StringFormat;
				BrickStringFormat bsf = source.ChangeFormatFlags(source.FormatFlags | StringFormatFlags.NoWrap);
				brick.Style = BrickStyleHelper.ChangeStringFormat(brick.Style, bsf);
			}
		}
		protected internal override bool HasBindings() {
			return base.HasBindings() || EmbeddedFieldsHelper.HasEmbeddedFieldNames(this);
		}
		protected internal override void SaveInitialPropertyValues() {
			base.SaveInitialPropertyValues();
			savedText = DisplayProperty;
		}
		protected internal override void RestoreInitialPropertyValues() {
			base.RestoreInitialPropertyValues();
			if(restoreSavedTextNeeded) {
				DisplayProperty = savedText;
				restoreSavedTextNeeded = false;
			}
		}
		protected internal override object GetTextValue() {
			if(embeddedFields != null && embeddedFields.Count > 0)
				return null;
			return (textBinding != null) ? textBinding.ColumnValue : base.GetTextValue();
		}
		protected internal override string GetTextValueFormatString() {
			return (textBinding != null) ? textBinding.FormatString : base.GetTextValueFormatString();
		}
		protected internal virtual string GetDisplayPropertyBindingString() {
			return (DataBindings[DisplayPropertyName] != null) ? DataBindings[DisplayPropertyName].ToString() : Text;
		}
		protected internal virtual MailMergeFieldInfosCalculator GetMailMergeFieldInfosCalculator() {
			return PlainTextMailMergeFieldInfosCalculator.Instance;
		}
		protected virtual string ConvertValue(object val) {
			return Convert.ToString(val);
		}
		protected override void BeforeReportPrint() {
			base.BeforeReportPrint();
			embeddedFields = EmbeddedFieldsHelper.GetEmbeddedFieldInfos(this);
		}
		protected override void AfterReportPrint() {
			base.AfterReportPrint();
			DisposeTextBinding();
		}
		protected override void UpdateBindingCore(XRDataContext dataContext, ImagesContainer images) {
			base.UpdateBindingCore(dataContext, images);
			if(HasDisplayDataBinding)
				embeddedFields = EmbeddedFieldsHelper.GetEmbeddedFieldInfos(this);
			if(HasEmbeddedFieldNames) {
				Parameter parameter = GetParameterByDataMember(embeddedFields[0].DataMember);
				if(textBinding == null && parameter != null)
					textBinding = new XRBinding("Text", Report.RootReport.ParametersDataSource, parameter.Name);
				else if(textBinding == null)
					textBinding = new XRBinding("Text", Report.DataSource, embeddedFields[0].DataMember);
				textBinding.GetColumnValue(dataContext, images);
				UpdateDisplayProperty();
			}
		}
		protected bool ShowBindingString(string text) {
			return DesignMode && (string.IsNullOrEmpty(text) || DataBindings.Contains(DisplayPropertyName));
		}
		protected override void Dispose(bool disposing) {
			if(!disposed) {
				if(disposing) {
					DisposeTextBinding();
				}
				disposed = true;
			}
			base.Dispose(disposing);
		}
		void DisposeTextBinding() {
			if(textBinding != null) {
				textBinding.Dispose();
				textBinding = null;
			}
		}
		string GetStringWithEmbeddedFieldValues() {
			Dictionary<MailMergeFieldInfoValue, string> embeddedFieldsHT = GetEmbeddedFieldValuesHT();
			Collection<string> values = new Collection<string>();
			foreach(MailMergeFieldInfo mailMergeInfo in embeddedFields)
				values.Add(embeddedFieldsHT[new MailMergeFieldInfoValue(mailMergeInfo)]);
			return EmbeddedFieldsHelper.AssembleStringFromEmbeddedFields(GetSavedText(), embeddedFields, values);
		}
		string GetStringValue(object obj, MailMergeFieldInfo mailMergeFieldInfo) {
			object value = mailMergeFieldInfo.HasFormatStringInfo ? String.Format(mailMergeFieldInfo.FormatString, obj) : obj;
			return ConvertValue(value);
		}
		Parameter GetParameterByDataMember(string dataMember) {
			string parameterName = ParametersReplacer.GetParameterName(dataMember);
			return !string.IsNullOrEmpty(parameterName) ? RootReport.Parameters.GetByName(parameterName) : null;
		}
		void UpdateDisplayProperty() {
			restoreSavedTextNeeded = true;
			DisplayProperty = GetStringWithEmbeddedFieldValues();
		}
		#endregion
		#region IDisplayNamePropertyContainer Members
		bool IDisplayNamePropertyContainer.CanSetPropertyValue {
			get { return this.Report != null; }
		}
		string IDisplayNamePropertyContainer.GetDisplayPropertyValue(string source) {
			return GetDisplayPropertyWithDisplayColumnNames();
		}
		string IDisplayNamePropertyContainer.GetRealPropertyValue(string source) {
			return GetTextFromTextWithDisplayColumnNames(source, GetMailMergeFieldInfosCalculator());
		}
		string IDisplayNamePropertyContainer.GetDisplayName(string source) {
			return CriteriaFieldNameConverter.ToDisplayName(Report, Report.GetEffectiveDataSource(), Report.DataMember, source);
		}
		#endregion
	}
}
