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
using System.Linq;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.XtraReports.Web.Native.ParametersPanel {
	static class ComboboxDateTimeLookupFix {
		const string JSPropertyMark = "cpReportParametersPanel_DateTimeLookup";
		public static bool HasMark(ASPxEditBase editor) {
			object value;
			return editor.JSProperties.TryGetValue(JSPropertyMark, out value) && value is bool && (bool)value;
		}
		public static void ApplyMark(ASPxEditBase editor) {
			editor.JSProperties[JSPropertyMark] = true;
		}
		public static void InitDateTimeLookUpEditor(ASPxComboBox combobox, LookUpValueCollection lookUpValues, object value) {
			var valueNotNullAndNotPresent = value != null && !lookUpValues.Any(x => value.Equals(x.Value));
			if(valueNotNullAndNotPresent) {
				lookUpValues.Insert(0, new LookUpValue(value, null));
			}
			foreach(var lookupValue in lookUpValues) {
				FixLookup(lookupValue);
			}
			combobox.DataSource = lookUpValues;
			combobox.TextField = LookUpValue.DescriptionPropertyName;
			combobox.ValueField = LookUpValue.ValuePropertyName;
			ApplyMark(combobox);
			if(value != null) {
				combobox.Value = ToJSON(value);
			}
		}
		public static string ToJSON(object value) {
			return HtmlConvertor.ToJSON(value, false, false, true);
		}
		static void FixLookup(LookUpValue lookupValue) {
			var value = lookupValue.Value;
			if(lookupValue.Description == null && IsTimeless(value)) {
				lookupValue.Description = ((DateTime)value).ToShortDateString();
			}
			lookupValue.Value = ToJSON(value);
		}
		static bool IsTimeless(object value) {
			if(value == null || !(value is DateTime)) {
				return false;
			}
			var dateValue = (DateTime)value;
			return dateValue == dateValue.Date;
		}
	}
}
