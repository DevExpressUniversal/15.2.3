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
using System.Web;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.Web.Native.ParametersPanel;
namespace DevExpress.XtraReports.Web.Native.DocumentViewer.ParametersPanel {
	public abstract class CallbackEventProcessorBase : ICallbackEventProcessor {
		readonly ClientParameterValueNormalizer clientParameterValueNormalizer = new ClientParameterValueNormalizer();
		#region ICallbackEventProcessor
		public IEnumerable<CallbackEventState> Process(string eventArgument, ASPxParameterInfo[] parametersInfo) {
			var eventArguments = eventArgument.Split(new[] { '=' }, 2);
			var command = eventArguments[0];
			var argument = HttpUtility.UrlDecode(eventArguments[1]);
			if(command == DocumentViewerReportParametersPanel.CascadeLookupsCallbackName) {
				return RaiseEditorsCascadeLookupsCallbackEventCore(argument, parametersInfo);
			}
			return Enumerable.Empty<CallbackEventState>();
		}
		#endregion
		IEnumerable<CallbackEventState> RaiseEditorsCascadeLookupsCallbackEventCore(string eventArgument, ASPxParameterInfo[] parametersInfo) {
			var args = HtmlConvertor.FromJSON<Hashtable>(eventArgument)
				.Cast<DictionaryEntry>()
				.ToDictionary(
					x => (string)x.Key,
					x => ConvertToCascadeLookupEventArgument((Hashtable)x.Value, (string)x.Key, parametersInfo));
			BeforeRaiseEditorsCascadeLookupsCallbackEventCore(args);
			var parameterEditorValueProvider = new WebParameterEditorValueProvider(parametersInfo, args);
			var result = new List<CallbackEventState>();
			foreach(var info in parametersInfo) {
				var autoCompleteBox = info.EditorInformation as ASPxAutoCompleteBoxBase;
				CascadeLookupEventArgument cascadeLookupEventArgument;
				if(autoCompleteBox == null || info.Parameter.LookUpSettings == null || !args.TryGetValue(info.Path, out cascadeLookupEventArgument) || string.IsNullOrEmpty(cascadeLookupEventArgument.CallbackEventArgument)) {
					continue;
				}
				LookUpValueCollection lookUpValues = GetLookUpValues(info, parameterEditorValueProvider);
				var values = lookUpValues.Select(x => x.Value);
				object value;
				if(info.Parameter.MultiValue) {
					value = cascadeLookupEventArgument.Value;
				} else {
					value = values.FirstOrDefault(x => CompareObjects(x, cascadeLookupEventArgument.Value, info.Parameter.Type))
						?? values.FirstOrDefault()
						?? cascadeLookupEventArgument.Value;
				}
				AssignData(autoCompleteBox, lookUpValues, value, info.Parameter.Type);
				ICallbackEventHandler callbackEventHandler = autoCompleteBox;
				callbackEventHandler.RaiseCallbackEvent("c0:" + cascadeLookupEventArgument.CallbackEventArgument);
				result.Add(new CallbackEventState(info.Path, callbackEventHandler));
			}
			return result;
		}
		protected virtual void BeforeRaiseEditorsCascadeLookupsCallbackEventCore(Dictionary<string, CascadeLookupEventArgument> args) {
		}
		protected abstract LookUpValueCollection GetLookUpValues(ASPxParameterInfo info, IParameterEditorValueProvider parameterEditorValueProvider);
		CascadeLookupEventArgument ConvertToCascadeLookupEventArgument(Hashtable hashtable, string path, ASPxParameterInfo[] parametersInfo) {
			object clientValue = hashtable["value"];
			string callbackEventArgument = hashtable["callbackEventArgument"] as string;
			ASPxParameterInfo info = parametersInfo.First(x => x.Path == path);
			object value = clientParameterValueNormalizer.NormalizeSafe(clientValue, info.Parameter.Type, info.Parameter.MultiValue);
			return new CascadeLookupEventArgument {
				Value = value,
				CallbackEventArgument = callbackEventArgument
			};
		}
		static bool CompareObjects(object serverValue, object clientValue, Type type) {
			if(serverValue == null && clientValue == null) {
				return true;
			}
			if(serverValue == null || clientValue == null) {
				return false;
			}
			if(clientValue is IConvertible) {
				clientValue = Convert.ChangeType(clientValue, type, CultureInfo.InvariantCulture);
			}
			try {
				return Comparer.DefaultInvariant.Compare(serverValue, clientValue) == 0;
			} catch(ArgumentException) {
				return false;
			}
		}
		static void AssignData(ASPxAutoCompleteBoxBase autoCompleteBox, LookUpValueCollection lookUpValues, object value, Type type) {
			autoCompleteBox.Items.Clear();
			var combobox = autoCompleteBox as ASPxComboBox;
			if(combobox != null) {
				AssignDataForComboBox(combobox, type, value, lookUpValues);
				return;
			}
			var tokenBox = autoCompleteBox as ASPxTokenBox;
			if(tokenBox != null) {
				MultiValueEditorFactory.FillItems(tokenBox.Items, tokenBox.Tokens, type, value as IEnumerable, lookUpValues, strictTokens: true);
				return;
			}
			autoCompleteBox.DataSource = lookUpValues;
			autoCompleteBox.TextField = LookUpValue.DescriptionPropertyName;
			autoCompleteBox.ValueField = LookUpValue.ValuePropertyName;
			autoCompleteBox.Value = value;
		}
		static void AssignDataForComboBox(ASPxComboBox combobox, Type type, object value, LookUpValueCollection lookUpValues) {
			var editorCreator = new ParametersASPxEditorCreator(bindOnLoad: false);
			editorCreator.ConfigureComboboxForLookUps(combobox, type, value, lookUpValues);
		}
	}
}
