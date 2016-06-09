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
using System.Linq;
using DevExpress.Data.Browsing;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.Web.Localization;
using DevExpress.XtraReports.Web.Native.ParametersPanel.MultiValue;
namespace DevExpress.XtraReports.Web.Native.ParametersPanel {
	public static class MultiValueEditorFactory {
		class TokenEntity {
			public object Value { get; private set; }
			public string Description { get; private set; }
			public bool IsSelected { get; set; }
			public TokenEntity(object value, string description) {
				this.Value = value;
				this.Description = description;
			}
		}
		const string JSPropertyMark = "cpReportParametersPanel_MultiValue";
		public static ASPxTokenBox Create(Parameter parameter) {
			return Create(parameter, null);
		}
		public static ASPxTokenBox Create(Parameter parameter, DataContext optionalDataContext) {
			if(!parameter.MultiValue) {
				throw new ArgumentException("Parameter is not MultiValued", "parameter");
			}
			var tokenbox = new ASPxTokenBox();
			ApplyMark(tokenbox);
			FillItems(tokenbox, parameter, optionalDataContext);
			bool hasLookUpSettings = parameter.LookUpSettings != null;
			tokenbox.AllowCustomTokens = !hasLookUpSettings;
			tokenbox.EnableCallbackMode = hasLookUpSettings && parameter.HasCascadeLookUpSettings();
			return tokenbox;
		}
		public static void ApplyMark(ASPxTokenBox tokenbox) {
			tokenbox.JSProperties[JSPropertyMark] = true;
		}
		public static bool HasMark(ASPxEditBase editor) {
			object value;
			return editor is ASPxTokenBox
				&& editor.JSProperties.TryGetValue(JSPropertyMark, out value)
				&& value is bool
				&& (bool)value;
		}
		public static void ApplyValidationSettings(ValidationSettings validationSettings, Type type) {
			var regexExpression = validationSettings.RegularExpression;
			if(WebReportParameterHelper.SignedDecimalTypes.Contains(type)) {
				regexExpression.ErrorText = ASPxReportsLocalizer.GetString(ASPxReportsStringId.ParametersPanel_GenericRegexValidationError);
				regexExpression.ValidationExpression = @"^-?\d+$";
			} else if(WebReportParameterHelper.UnsignedDecimalTypes.Contains(type)) {
				regexExpression.ErrorText = ASPxReportsLocalizer.GetString(ASPxReportsStringId.ParametersPanel_GenericRegexValidationError);
				regexExpression.ValidationExpression = @"^\d+$";
			} else if(WebReportParameterHelper.FixedPointTypes.Contains(type)) {
				regexExpression.ErrorText = ASPxReportsLocalizer.GetString(ASPxReportsStringId.ParametersPanel_GenericRegexValidationError);
				regexExpression.ValidationExpression = @"^-?\d+\.?\d*?$";
			} else if(type == typeof(Guid)) {
				regexExpression.ErrorText = ASPxReportsLocalizer.GetString(ASPxReportsStringId.ParametersPanel_GuidValidationError);
				regexExpression.ValidationExpression = @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$";
			} else if(type == typeof(DateTime)) {
				regexExpression.ErrorText = ASPxReportsLocalizer.GetString(ASPxReportsStringId.ParametersPanel_GenericRegexValidationError);
				regexExpression.ValidationExpression = @"^((((31\/(0?[13578]|1[02]))|((29|30)\/(0?[1,3-9]|1[0-2])))\/(1[6-9]|[2-9]\d)?\d{2})|(29\/0?2\/(((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))|(0?[1-9]|1\d|2[0-8])\/((0?[1-9])|(1[0-2]))\/((1[6-9]|[2-9]\d)?\d{2})) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d$";
			}
		}
		internal static void FillItems(ListEditItemCollection items, CustomCollection<string> tokens, Type type, IEnumerable values, LookUpSettings lookUpSettings, DataContext optionalDataContext = null, IParameterEditorValueProvider parameterValueProvider = null, bool strictTokens = false) {
			ITokenConverter converter = GetTokenConverter(type);
			LookUpValueCollection lookUpValues = lookUpSettings != null
				? LookUpHelper.GetLookUpValues(lookUpSettings, optionalDataContext, parameterValueProvider)
				: null;
			FillItems(items, tokens, converter, values, lookUpValues, strictTokens);
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public static void FillItems(ListEditItemCollection items, CustomCollection<string> tokens, Type type, IEnumerable values, LookUpValueCollection lookUpValues, bool strictTokens = false) {
			ITokenConverter converter = GetTokenConverter(type);
			FillItems(items, tokens, converter, values, lookUpValues, strictTokens);
		}
		static void FillItems(ListEditItemCollection items, CustomCollection<string> tokens, ITokenConverter converter, IEnumerable values, LookUpValueCollection lookUpValues, bool strictTokens = false) {
			TokenEntity[] generatedItems = GenerateTokenItems(converter, values, lookUpValues, strictTokens);
			items.Clear();
			tokens.Clear();
			foreach(var item in generatedItems) {
				items.Add(item.Description, item.Value);
				if(item.IsSelected) {
					tokens.Add(item.Description);
				}
			}
		}
		static ITokenConverter GetTokenConverter(Type type) {
			type = type ?? typeof(string);
			if(type == typeof(string)) {
				return StringTokenConverter.Instance;
			} else if(type == typeof(DateTime)) {
				return DateTimeTokenConverter.Instance;
			}
			return new SimpleTokenConverter(type);
		}
		static void FillItems(ASPxTokenBox tokenbox, Parameter parameter, DataContext optionalDataContext) {
			var parameterType = parameter.Type;
			var isDateTime = parameterType == typeof(DateTime);
			tokenbox.ItemValueType = isDateTime ? typeof(string) : parameterType;
			if(isDateTime) {
				ComboboxDateTimeLookupFix.ApplyMark(tokenbox);
			}
			FillItems(tokenbox.Items, tokenbox.Tokens, parameterType, parameter.Value as IEnumerable, parameter.LookUpSettings, optionalDataContext);
		}
		static TokenEntity[] GenerateTokenItems(ITokenConverter tokenConverter, IEnumerable values, LookUpValueCollection lookUpValues, bool strictTokens) {
			var dictionary = new Dictionary<object, TokenEntity>();
			if(lookUpValues != null) {
				FillLookUpValues(dictionary, tokenConverter, lookUpValues);
			}
			if(values != null) {
				FillValues(dictionary, tokenConverter, values, strictTokens);
			}
			var result = new TokenEntity[dictionary.Count];
			dictionary.Values.CopyTo(result, 0);
			return result;
		}
		static void FillLookUpValues(Dictionary<object, TokenEntity> dictionaryToFill, ITokenConverter tokenConverter, LookUpValueCollection lookUpValues) {
			foreach(LookUpValue lookUpValue in lookUpValues) {
				KeyValuePair<object, string> entity = tokenConverter.GetEntity(lookUpValue.Value);
				dictionaryToFill[entity.Key] = new TokenEntity(entity.Key, lookUpValue.RealDescription);
			}
		}
		static void FillValues(Dictionary<object, TokenEntity> dictionaryToFill, ITokenConverter tokenConverter, IEnumerable values, bool strictTokens) {
			foreach(object value in values) {
				KeyValuePair<object, string> valueEntity = tokenConverter.GetEntity(value);
				TokenEntity existingEntity;
				if(!dictionaryToFill.TryGetValue(valueEntity.Key, out existingEntity) && !strictTokens) {
					existingEntity = new TokenEntity(valueEntity.Key, valueEntity.Value);
					dictionaryToFill.Add(existingEntity.Value, existingEntity);
				}
				existingEntity.IsSelected = true;
			}
		}
	}
}
