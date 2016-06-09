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
using DevExpress.Data.Browsing;
using DevExpress.Web;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Localization;
namespace DevExpress.XtraReports.Web.Native.ParametersPanel {
	public class ParametersASPxEditorCreator : ParametersEditorCreatorBase<ASPxEditBase> {
		readonly bool bindOnLoad;
		readonly DataContext optionalDataContext;
		readonly IExtensionsProvider extensionsProvider;
		public ParametersASPxEditorCreator(bool bindOnLoad)
			: this(bindOnLoad, null) {
		}
		public ParametersASPxEditorCreator(bool bindOnLoad, XtraReport report)
			: this(bindOnLoad, SafeGetService<DataContext>(report), report) {
		}
		public ParametersASPxEditorCreator(bool bindOnLoad, DataContext optionalDataContext, IExtensionsProvider extensionsProvider) {
			this.bindOnLoad = bindOnLoad;
			this.optionalDataContext = optionalDataContext;
			this.extensionsProvider = extensionsProvider;
		}
		public void ConfigureComboboxForLookUps(ASPxComboBox combobox, Type type, object value, LookUpValueCollection lookUpValues) {
			if(type == typeof(DateTime)) {
				ComboboxDateTimeLookupFix.InitDateTimeLookUpEditor(combobox, lookUpValues, value);
			} else {
				ConfigureComboboxForLookUpsCore(combobox, type, value, lookUpValues);
			}
			if(bindOnLoad) {
				combobox.Load += LookUpEdit_Load;
			} else {
				combobox.DataBind();
			}
		}
		protected override ASPxEditBase CreateMultiValueEditor(Parameter parameter) {
			ASPxTokenBox result = MultiValueEditorFactory.Create(parameter, optionalDataContext);
			WebReportParameterHelper.ConfigureValidationSettings(result.ValidationSettings, isFieldRequired: false);
			if(parameter.LookUpSettings == null) {
				MultiValueEditorFactory.ApplyValidationSettings(result.ValidationSettings, parameter.Type);
			}
			return result;
		}
		protected override ASPxEditBase CreateLookUpEditor(Parameter parameter) {
			var result = new ASPxComboBox();
			LookUpValueCollection lookUpValues = LookUpHelper.GetLookUpValues(parameter.LookUpSettings, optionalDataContext);
			ConfigureComboboxForLookUps(result, parameter.Type, parameter.Value, lookUpValues);
			return result;
		}
		protected override ASPxEditBase CreateBoolComboBox() {
			var result = new ASPxComboBox { ValueType = typeof(bool) };
			result.Items.Add(ASPxReportsLocalizer.GetString(ASPxReportsStringId.ParametersPanel_True), true);
			result.Items.Add(ASPxReportsLocalizer.GetString(ASPxReportsStringId.ParametersPanel_False), false);
			WebReportParameterHelper.ConfigureValidationSettings(result.ValidationSettings);
			return result;
		}
		protected override ASPxEditBase CreateGuidTextBox() {
			var result = CreateTextBoxCore(isFieldRequired: true);
			var regexExpression = result.ValidationSettings.RegularExpression;
			regexExpression.ErrorText = ASPxReportsLocalizer.GetString(ASPxReportsStringId.ParametersPanel_GuidValidationError);
			regexExpression.ValidationExpression = @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$";
			return result;
		}
		protected override ASPxEditBase CreateDateTimeEdit() {
			var result = new ASPxDateEdit { AllowNull = false };
			result.CalendarProperties.ShowClearButton = false;
			WebReportParameterHelper.ConfigureValidationSettings(result.ValidationSettings);
			result.ValidationSettings.RequiredField.ErrorText = ASPxReportsLocalizer.GetString(ASPxReportsStringId.ParametersPanel_DateTimeValueValidationError);
			return result;
		}
		protected override ASPxEditBase CreateNumericEditor(Type parameterType) {
			var result = new ASPxSpinEdit {
				NumberType = WebReportParameterHelper.GetSpinEditNumberType(parameterType),
				AllowNull = false
			};
			var onlyPositiveValues = WebReportParameterHelper.UnsignedDecimalTypes.Contains(parameterType);
			if(onlyPositiveValues) {
				result.MinValue = 0;
			}
			WebReportParameterHelper.ConfigureValidationSettings(result.ValidationSettings);
			return result;
		}
		protected override ASPxEditBase CreateEnumComboBox(Type type) {
			var result = new ASPxComboBox { ValueType = type };
			WebReportParameterHelper.FillListEditItems(result.Items, type);
			WebReportParameterHelper.ConfigureValidationSettings(result.ValidationSettings);
			return result;
		}
		protected override ASPxEditBase CreateTextBox() {
			return CreateTextBoxCore(isFieldRequired: false);
		}
		static T SafeGetService<T>(IServiceProvider serviceProvider)
			where T : class {
			return serviceProvider != null
				? (T)serviceProvider.GetService<T>()
				: null;
		}
		static ASPxTextBox CreateTextBoxCore(bool isFieldRequired) {
			var result = new ASPxTextBox();
			WebReportParameterHelper.ConfigureValidationSettings(result.ValidationSettings, isFieldRequired);
			const string EmptyJSFunction = "function(){}";
			result.ClientSideEvents.ValueChanged = EmptyJSFunction;
			return result;
		}
		static void LookUpEdit_Load(object sender, EventArgs e) {
			var combobox = (ASPxComboBox)sender;
			combobox.Load -= LookUpEdit_Load;
			combobox.DataBind();
		}
		void ConfigureComboboxForLookUpsCore(ASPxComboBox combobox, Type type, object value, LookUpValueCollection lookUpValues) {
			ListEditItemCollection items = combobox.Items;
			items.Clear();
			combobox.ValueType = type;
			bool hasValueInLookups = false;
			bool useSerializer = !type.IsPrimitive && type != typeof(string) && type != typeof(decimal) && type != typeof(Guid);
			foreach(LookUpValue lookUpValue in lookUpValues) {
				var currentValue = lookUpValue.Value;
				if(!hasValueInLookups && value != null && currentValue != null) {
					hasValueInLookups = value.Equals(currentValue);
				}
				if(useSerializer) {
					string result;
					if(SerializationService.SerializeObject(currentValue, out result, extensionsProvider)) {
						currentValue = result;
						combobox.ValueType = typeof(string);
					} else {
						useSerializer = false;
					}
				}
				items.Add(lookUpValue.RealDescription, currentValue);
			}
			if(value == null || (value is string && string.IsNullOrEmpty(value as string))) {
				return;
			}
			if(!hasValueInLookups) {
				if(useSerializer) {
					string result;
					if(SerializationService.SerializeObject(value, out result, extensionsProvider)) {
						value = result;
					}
					value = result;
				}
				items.Add(value.ToString(), value);
			}
			combobox.Value = value;
		}
	}
}
