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
using System.Globalization;
using System.Text;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Data;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Fields {
	public enum CalculatedFieldValueState {
		RawValue,
		ValueConvertedToString
	}
	public class CalculatedFieldValue : IDisposable {
		static readonly CalculatedFieldValue nullValue = new CalculatedFieldValue(null);
		static readonly CalculatedFieldValue invalidValue = new CalculatedFieldValue(null);
		public static CalculatedFieldValue Null { get { return nullValue; } }
		public static CalculatedFieldValue InvalidValue { get { return invalidValue; } }
		object rawValue;
		string text;
		DocumentModel documentModel;
		FieldResultOptions options;				
		public CalculatedFieldValue(object rawValue) {
			SetRawValue(rawValue);
		}
		public FieldResultOptions Options { get { return options; } }
		public CalculatedFieldValue(object rawValue, FieldResultOptions options) : this(rawValue) {
			AddOptions(options);
		}
		public CalculatedFieldValue AddOptions(FieldResultOptions options) {
			this.options |= options;
			return this;
		}
		void SetRawValue(object value) {
			this.rawValue = value;
			this.text = value as String;
			this.documentModel = value as DocumentModel;
			if (this.text != null)
				ConvertedToString = true;
		}
		public bool ConvertedToString { get; private set; }
		public DocumentModel DocumentModel { get { return documentModel; } }
		public string Text { get { return text; } }
		public object RawValue { get { return rawValue; } }
		public bool IsDateTimeValue() {
			return rawValue is DateTime;
		}
		public bool IsDocumentModelValue() {
			return rawValue is DocumentModel;
		}
		internal void SetValueConvertedToString(string stringValue) {
			if (documentModel != null)
				documentModel.Dispose();
			documentModel = null;
			SetRawValue(stringValue);
			ConvertedToString = true;		  
		}
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);			
		}
		public virtual void Dispose(bool disposing) {
			if (disposing) {
				if (this.documentModel != null) {
					this.documentModel.Dispose();
					this.documentModel = null;
				}				
			}
		}
		~CalculatedFieldValue() {
			Dispose(false);
		}
		#endregion
	}
	public abstract class CalculatedFieldBase {
		protected enum FieldMailMergeType { NonMailMerge = 1, MailMerge = 2, Mixed = NonMailMerge | MailMerge }
		public static readonly string FrameworkStringFormatSwitch = "$";
		static internal readonly string MergeFormatKeyword = "MERGEFORMAT";
		static internal readonly string MergeFormatInetKeyword = "MERGEFORMATINET";
		static internal readonly string CharFormatKeyword = "CHARFORMAT";		
		string dateAndTimeFormatting;
		string numericFormatting;
		string[] generalFormatting;
		string commonStringFormat;
		InstructionCollection switches;
		bool useCurrentCultureDateTimeFormat;
		public string DateAndTimeFormatting { get { return dateAndTimeFormatting; } }
		public string NumericFormatting { get { return numericFormatting; } }
		public string FrameworkStringFormat { get { return commonStringFormat; } }
		public string[] GeneralFormatting { get { return generalFormatting; } }
		public InstructionCollection Switches { get { return switches; } }
		public virtual bool CanPrepare { get { return false; } }
		public virtual bool AllowAsyncUpdate { get { return true; } } 
		protected bool UseCurrentCultureDateTimeFormat { get {return useCurrentCultureDateTimeFormat;}}
		protected abstract string FieldTypeName { get; }
		protected virtual FieldMailMergeType MailMergeType() {
			return FieldMailMergeType.NonMailMerge;
		}
		protected internal virtual UpdateFieldOperationType GetAllowedUpdateFieldTypes(FieldUpdateOnLoadOptions options) {
			return UpdateFieldOperationType.Normal;
		}
		protected static string GetDefaultMailMergeFieldText(string fieldName) {
			return String.Format("<<{0}>>", fieldName);
		}
		protected static Dictionary<string, bool> CreateSwitchesWithArgument(params string[] switches) {
			Dictionary<string, bool> result = new Dictionary<string, bool>();
			int count = switches.Length;
			for (int i = 0; i < count; i++)
				result.Add(String.Format("\\{0}", switches[i]), true);
			return result;
		}
		public virtual bool IsSwitchWithArgument(string fieldSpecificSwitch) {
			return SwitchesWithArguments.ContainsKey(fieldSpecificSwitch);
		}
		public virtual void Initialize(PieceTable pieceTable, InstructionCollection switches) {
			this.switches = switches;
			dateAndTimeFormatting = switches.GetString("@");
			numericFormatting = switches.GetString("#");
			generalFormatting = switches.GetStringArray("*");
			if (pieceTable.SupportFieldCommonStringFormat)
				commonStringFormat = switches.GetString(FrameworkStringFormatSwitch);
			this.useCurrentCultureDateTimeFormat = pieceTable.DocumentModel.FieldOptions.UseCurrentCultureDateTimeFormat;
		}
		protected abstract Dictionary<string, bool> SwitchesWithArguments { get; }
		protected virtual bool InsertDefaultMailMergeText(PieceTable sourcePieceTable) {
			if ((MailMergeType() & FieldMailMergeType.NonMailMerge) != 0)
				return false;
			IFieldDataService fieldDataService = sourcePieceTable.DocumentModel.GetService<IFieldDataService>();
			return fieldDataService == null || !fieldDataService.BoundMode;
		}
		public virtual CalculatedFieldValue Update(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {			
			if (InsertDefaultMailMergeText(sourcePieceTable)) {				
				return new CalculatedFieldValue(GetDefaultMailMergeFieldText(FieldTypeName), GetCharacterFormatFlag());
			}
			CalculatedFieldValue value = GetCalculatedValueCore(sourcePieceTable, mailMergeDataMode, documentField);
			if (value == CalculatedFieldValue.InvalidValue) {				
				return new CalculatedFieldValue(GetDefaultMailMergeFieldText(FieldTypeName), GetCharacterFormatFlag());				
			}
			ApplyFieldFormatting(value, sourcePieceTable.DocumentModel.MailMergeOptions.CustomSeparators);
			Debug.Assert(value.IsDocumentModelValue() || value.ConvertedToString);
			FieldResultOptions characterFormatFlag = GetCharacterFormatFlag();
			if ((MailMergeType() & FieldMailMergeType.MailMerge) != 0)
				characterFormatFlag |= FieldResultOptions.MailMergeField;
			return value.AddOptions(characterFormatFlag);
		}
		protected virtual FieldResultOptions GetCharacterFormatFlag() {
			if (generalFormatting == null)
				return FieldResultOptions.None;
			int lastIndex = generalFormatting.Length - 1;
			for (int i = lastIndex; i >= 0; i--) {
				string value = generalFormatting[i].ToUpper(CultureInfo.InvariantCulture);
				if (value == MergeFormatKeyword || value == MergeFormatInetKeyword)
					return FieldResultOptions.MergeFormat;
				if (value == CharFormatKeyword)
					return FieldResultOptions.CharFormat;
			}
			return FieldResultOptions.None;
		}
		public virtual void ApplyFieldFormatting(CalculatedFieldValue value, MailMergeCustomSeparators customSeparators) {			
			TryApplyFormatting(value, customSeparators);
			if (GeneralFormatting != null) {
				int count = GeneralFormatting.Length;
				GeneralFieldFormatter generalFieldFormatter = new GeneralFieldFormatter();
				for (int i = 0; i < count; i++) {
					string generalFormattingSwitch = GeneralFormatting[i];
					string generalFormattingSwitchUpperCase = generalFormattingSwitch.ToUpper(CultureInfo.InvariantCulture);
					if (generalFormattingSwitchUpperCase == MergeFormatKeyword || generalFormattingSwitchUpperCase == MergeFormatInetKeyword || generalFormattingSwitchUpperCase == CharFormatKeyword)
						continue;
					if (!value.ConvertedToString && value.IsDocumentModelValue()) {
						if (generalFieldFormatter.IsGeneralDocumentModelStringFormatter(generalFormattingSwitch))
							generalFieldFormatter.FormatPieceTable(value.DocumentModel.MainPieceTable, generalFormattingSwitch);
						else {
							string actualValue = GetDocumentModelStringValue(value.DocumentModel);
							actualValue = generalFieldFormatter.Format(actualValue, (string)actualValue, generalFormattingSwitch);
							value.SetValueConvertedToString(actualValue);
						}
					}
					else {
						if (!value.ConvertedToString && !value.IsDocumentModelValue()) {
							value.SetValueConvertedToString(value.RawValue.ToString());							
						}
						Debug.Assert(value.ConvertedToString);
						value.SetValueConvertedToString(generalFieldFormatter.Format(value.Text, value.Text, generalFormattingSwitch));
					}
				}
			}
			if (!value.IsDocumentModelValue() && !value.ConvertedToString) {
				if (value.RawValue != null)
					value.SetValueConvertedToString(value.RawValue.ToString());
				else
					value.SetValueConvertedToString(String.Empty);
			}
			Debug.Assert(value.IsDocumentModelValue() || value.ConvertedToString);
		}
		void TryApplyFormatting(CalculatedFieldValue value, MailMergeCustomSeparators customSeparators) {
			if (!SholdApplyFormating)
				return;
			if (TryApplyCommonStringFormat(value))
				return;
			if (TryApplyDateAndTimeFormatting(value))
				return;
			TryApplyNumericFormatting(value, customSeparators);
		}
		public virtual void BeforeCalculateFields(PieceTable sourcePieceTable, Field documentField) {
		}
		public abstract CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField);
		protected virtual bool SholdApplyFormating {
			get { return true; }
		}
		protected bool TryApplyCommonStringFormat(CalculatedFieldValue value) {
			bool hasCommonStringFormat = this.commonStringFormat != null;
			bool shouldApplyFormatting = hasCommonStringFormat && value.RawValue != null;
			if(shouldApplyFormatting) {
				string formatString = MailMergeFieldInfo.MakeFormatString(commonStringFormat);
				value.SetValueConvertedToString(String.Format(formatString, value.RawValue));
				return true;
			}
			return false;
		}
		object GetObjectForFormat(CalculatedFieldValue value, string commonStringFormat) {
			DateTime dateTime;
			if (TryConvertToDateTime(value, out dateTime))
				return dateTime;
			double number;
			if(TryConvertToDouble(value, out number))
				return number;
			if (value.IsDocumentModelValue())
				return GetDocumentModelStringValue(value.DocumentModel);
			else   
				return value.RawValue;
		}
		protected virtual bool TryApplyDateAndTimeFormatting(CalculatedFieldValue value) {
			bool hasDateAndTimeFormatting = dateAndTimeFormatting != null;
			bool shouldApplyFormatting = hasDateAndTimeFormatting || value.IsDateTimeValue();
			if (!shouldApplyFormatting)
				return false;
			DateTime dateTime;
			if (!TryConvertToDateTime(value, out dateTime))
				return false;
			DateTimeFieldFormatter dateTimeFieldFormatter = CreateDateTimeFieldFormatter();
			value.SetValueConvertedToString(dateTimeFieldFormatter.Format(dateTime, DateAndTimeFormatting, hasDateAndTimeFormatting));
			return true;
		}
		protected internal virtual DateTimeFieldFormatter CreateDateTimeFieldFormatter() {
			return new DateTimeFieldFormatter() { UseCurrentCultureDateTimeFormat = this.useCurrentCultureDateTimeFormat };
		}
		protected virtual bool TryApplyNumericFormatting(CalculatedFieldValue value, MailMergeCustomSeparators customSeparators) {			
			bool hasNumericFormatting = numericFormatting != null;
			if (!hasNumericFormatting)
				return false;
			double doubleVal;
			if (!TryConvertToDouble(value, out doubleVal))
				return false;
			NumericFieldFormatter numericFieldFormatter = new NumericFieldFormatter();
			numericFieldFormatter.CustomSeparators.Assign(customSeparators);
			value.SetValueConvertedToString(numericFieldFormatter.Format(doubleVal, NumericFormatting, hasNumericFormatting));
			return true;
		}
		protected virtual bool TryConvertToDateTime(CalculatedFieldValue value, out DateTime result) {
			object rawValue = value.RawValue;
			if (value.IsDocumentModelValue()) {
				rawValue = GetDocumentModelStringValue(value.DocumentModel);
			}
			try {
				result = Convert.ToDateTime(rawValue);
				return true;
			}
			catch {
				result = DateTime.MinValue;
				return false;
			}
		}
		protected virtual bool TryConvertToDouble(CalculatedFieldValue value, out double result) {
			object rawValue = value.RawValue;
			if (value.IsDocumentModelValue()) {
				rawValue = GetDocumentModelStringValue(value.DocumentModel);
			}
			if (rawValue != null && !IsEmptyString(rawValue)) {
				try {
					result = Convert.ToDouble(rawValue);
					return true;
				}
				catch {
				}
			}
			result = 0;
			return false;
		}
		bool IsEmptyString(object value) {
			string str = value as string;
			if (str == null)
				return false;
			else
				return str.Length == 0;
		}
		protected virtual string GetDocumentModelStringValue(DocumentModel documentModel) {
			return documentModel.InternalAPI.GetDocumentPlainTextContent();
		}
		protected internal virtual string[] GetNativeSwithes() {
			return new string[] { };
		}
		protected internal virtual string[] GetInvariableSwitches() {
			return new string[] { };
		}
		public virtual bool IsSwitchArgumentField(string fieldSpecificSwitch) {
			return false;
		}
		public virtual bool CanUseSwitchWithoutArgument(string fieldSpecificSwitch) {
			return false;
		}
	}
	public class ExpressionCalculatedField : CalculatedFieldBase {
		static readonly Dictionary<string, bool> switchesWithArguments = new Dictionary<string, bool>();
		double result;
		public ExpressionCalculatedField(double result) {
			this.result = result;
		}
		protected override string FieldTypeName {
			get { throw new NotImplementedException(); }
		}
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return switchesWithArguments; } }
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			return new CalculatedFieldValue(result);
		}
	}
}
