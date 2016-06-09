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
using DevExpress.Web;
using DevExpress.XtraReports.Web.Native.ParametersPanel;
namespace DevExpress.Web.Mvc {
	public class MVCxDocumentViewerParameter {
		MVCxDocumentViewerParameterEditorType parameterType;
		internal Func<object, object> JsObjectToPoco { get; private set; }
		public string ParameterName { get; set; }
		public EditPropertiesBase PropertiesEdit { get; set; }
		public string EditorID { get; set; }
		public MVCxDocumentViewerParameterEditorType ParameterType {
			get {
				return parameterType;
			}
			set {
				parameterType = value;
				PropertiesEdit = CreateEditProperties(value);
			}
		}
		public void SetConvertBack(Func<object, object> jsObjectToPoco) {
			JsObjectToPoco = jsObjectToPoco;
		}
		static EditPropertiesBase CreateEditProperties(MVCxDocumentViewerParameterEditorType parameterType) {
			switch(parameterType) {
				case MVCxDocumentViewerParameterEditorType.CheckBox:
				case MVCxDocumentViewerParameterEditorType.ComboBox:
					var comboboxProperties = new MVCxComboBoxProperties();
					WebReportParameterHelper.ConfigureValidationSettings(comboboxProperties.ValidationSettings);
					return comboboxProperties;
				case MVCxDocumentViewerParameterEditorType.DateEdit:
					var dateEditProperties = new MVCxDateEditProperties { AllowNull = false };
					WebReportParameterHelper.ConfigureValidationSettings(dateEditProperties.ValidationSettings);
					return dateEditProperties;
				case MVCxDocumentViewerParameterEditorType.SpinEdit:
					var spinEditProperties = new MVCxSpinEditProperties { AllowNull = false };
					WebReportParameterHelper.ConfigureValidationSettings(spinEditProperties.ValidationSettings);
					return spinEditProperties;
				case MVCxDocumentViewerParameterEditorType.TimeEdit:
					var timeEditProperties = new MVCxTimeEditProperties();
					WebReportParameterHelper.ConfigureValidationSettings(timeEditProperties.ValidationSettings);
					return timeEditProperties;
				case MVCxDocumentViewerParameterEditorType.DropDownEdit:
					var dropDownEditProperties = new MVCxDropDownEditProperties();
					WebReportParameterHelper.ConfigureValidationSettings(dropDownEditProperties.ValidationSettings);
					return dropDownEditProperties;
				case MVCxDocumentViewerParameterEditorType.TokenBox:
					var tokenBoxProperties = new MVCxTokenBoxProperties();
					WebReportParameterHelper.ConfigureValidationSettings(tokenBoxProperties.ValidationSettings);
					return tokenBoxProperties;
				case MVCxDocumentViewerParameterEditorType.TextBox:
				case MVCxDocumentViewerParameterEditorType.Default:
				default:
					var textBoxProperties = new MVCxTextBoxProperties();
					WebReportParameterHelper.ConfigureValidationSettings(textBoxProperties.ValidationSettings);
					return textBoxProperties;
			}
		}
	}
	public enum MVCxDocumentViewerParameterEditorType {
		Default,
		TextBox,
		CheckBox,
		ComboBox,
		DateEdit,
		SpinEdit,
		TimeEdit,
		DropDownEdit,
		TokenBox
	}
}
