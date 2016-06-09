#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public class PdfButtonFormField : PdfInteractiveFormField {
		internal const string Type = "Btn";
		internal const string OffStateName = "Off";
		const string valueDictionaryKey = "V";
		const string defaultValuDictionaryKey = "DV";
		const string optionsDictionaryKey = "Opt";
		static string ReadState(PdfReaderDictionary dictionary, string key) {
			object value;
			if (!dictionary.TryGetValue(key, out value))
				return null;
			value = dictionary.Objects.TryResolve(value);
			string state;
			PdfName name = value as PdfName;
			if (name == null) {
				byte[] bytes = value as byte[];
				if (bytes == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				state = PdfDocumentReader.ConvertToString(bytes);
			}
			else 
				state = name.Name;
			return String.IsNullOrEmpty(state) ? null : state;
		}
		readonly PdfButtonFormField valuesProvider;
		readonly string defaultState;
		readonly string[] kidsState;
		string state;
		public string DefaultState { get { return valuesProvider.defaultState; } }
		public IList<string> KidsState { get { return valuesProvider.kidsState; } }
		public string State { get { return valuesProvider.state; } }
		internal string OnState {
			get {
				IList<PdfInteractiveFormField> kids = valuesProvider.Kids;
				string[] kidsState = valuesProvider.kidsState;
				if (kids != null && kidsState != null) {
					int index = kids.IndexOf(this);
					if (index >= 0)
						return kidsState[index];
				}
				PdfWidgetAnnotation widget = Widget;
				return widget == null ? OffStateName : widget.OnAppearanceName;
			}
		}
		protected internal override bool ShouldHighlight { get { return base.ShouldHighlight && !Flags.HasFlag(PdfInteractiveFormFieldFlags.PushButton); } }
		protected internal override object Value {
			get {
				string state = valuesProvider.state;
				string optValue = FindOption(state);
				return String.IsNullOrEmpty(optValue) ? (state ?? OffStateName) : optValue;
			}
		}
		protected internal override object DefaultValue { get { return valuesProvider.defaultState ?? OffStateName; } }
		protected override string FieldType { get { return Type; } }
		internal PdfButtonFormField(PdfInteractiveForm form, PdfInteractiveFormField parent, PdfReaderDictionary dictionary, PdfObjectReference valueReference) 
				: base(form, parent, dictionary, valueReference) {
			valuesProvider = (ValuesProvider as PdfButtonFormField) ?? this;
			state = ReadState(dictionary, valueDictionaryKey);
			defaultState = ReadState(dictionary, defaultValuDictionaryKey);
			IList<PdfInteractiveFormField> kids = Kids;
			IList<object> opt = dictionary.GetArray(optionsDictionaryKey);
			if (opt == null) {
				if (state != null || defaultState != null)
					if (kids == null) {
						if (Widget == null)
							PdfDocumentReader.ThrowIncorrectDataException();
					}
					else
						foreach (PdfInteractiveFormField kid in kids)
							if (kid.Widget == null)
								PdfDocumentReader.ThrowIncorrectDataException();
			}
			else {
				if (kids == null || (Flags & PdfInteractiveFormFieldFlags.PushButton) != 0)
					PdfDocumentReader.ThrowIncorrectDataException();
				int kidsCount = kids.Count;
				if (opt.Count < kidsCount)
					PdfDocumentReader.ThrowIncorrectDataException();
				bool found = false;
				kidsState = new string[kidsCount];
				for (int i = 0; i < kidsCount; i++) {
					string widgetState = opt[i] as string;
					if (widgetState == null) {
						byte[] widgetStateBytes = opt[i] as byte[];
						if (widgetStateBytes != null)
							widgetState = PdfDocumentReader.ConvertToString(widgetStateBytes);
					}
					PdfWidgetAnnotation widget = kids[i].Widget;
					if (widgetState == null || widget == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					kidsState[i] = widgetState;
					if ((state == null || state == widget.AppearanceName) && (defaultState == null || widget.Appearance.Names.Contains(defaultState)))
						found = true;
				}
				if (!found)
					foreach (PdfInteractiveFormField kid in kids) {
						string name = kid.Widget.AppearanceName;
						if (!String.IsNullOrEmpty(name) && name != "Off") {
							state = name;
							break;
						}
					}
			}
		}
		bool HasAppearance(string value) {
			PdfWidgetAnnotation widget = Widget;
			if (widget == null)
				return false;
			PdfAnnotationAppearances appearances = widget.Appearance;
			if (appearances == null)
				return false;
			PdfAnnotationAppearance appearance = appearances.Normal;
			return appearance != null && appearance.GetNames(null).Contains(value);
		}
		string FindOption(string onAppearanceName) {
			IList<PdfInteractiveFormField> kids = valuesProvider.Kids;
			string[] kidsState = valuesProvider.kidsState;
			if (kids != null && kidsState != null) {
				int kidsCount = kids.Count;
				for (int i = 0; i < kidsCount; i++) {
					PdfWidgetAnnotation widget = kids[i].Widget;
					if (widget != null && onAppearanceName == widget.OnAppearanceName)
						return kidsState[i];
				}
			}
			return String.Empty;
		}
		protected internal override void SetValue(object value, IEnumerable<Action> rebuildAppearanceActions) {
			base.SetValue(value, rebuildAppearanceActions);
			if (!AcceptValue(value))
				throw new ArgumentException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectButtonFormFieldValue));
			string strValue = value as string;
			IList<PdfInteractiveFormField> kids = valuesProvider.Kids;
			IList<string> kidsState = valuesProvider.kidsState;
			if (kids != null && kidsState != null) {
				string newValue = null;
				string option = kidsState.Contains(strValue) ? strValue : FindOption(strValue);
				if (!String.IsNullOrEmpty(option)) {
					int kidsCount = kidsState.Count;
					for (int i = 0; i < kidsCount; i++) {
						PdfWidgetAnnotation widget = kids[i].Widget;
						if (widget != null)
							if (option == kidsState[i]) {
								string newWidgetState = widget.OnAppearanceName;
								bool isMofidied = newWidgetState != valuesProvider.state;
								if (isMofidied && String.IsNullOrEmpty(newValue)) {
									PdfInteractiveFormFieldValueChangingEventArgs changingArgs = new PdfInteractiveFormFieldValueChangingEventArgs(FullName, valuesProvider.state, newWidgetState);
									if (RaiseFieldChanging(changingArgs))
										newValue = changingArgs.NewValue as string;
									else
										return;
								}
								widget.AppearanceName = newWidgetState;
								if (isMofidied)
									SetFormModifiedState(rebuildAppearanceActions);
							}
							else
								widget.AppearanceName = OffStateName;
					}
					if (!String.IsNullOrEmpty(newValue)) {
						string oldValue = valuesProvider.state;
						valuesProvider.state = newValue;
						RaiseFieldChanged(oldValue, newValue);
					}
					return;
				}
			}
			if (strValue != state) {
				string oldState = valuesProvider.state;
				string newState = strValue ?? OffStateName;
				bool isModified = newState != oldState;
				PdfInteractiveFormFieldValueChangingEventArgs changingArgs = new PdfInteractiveFormFieldValueChangingEventArgs(FullName, oldState, newState);
				if (RaiseFieldChanging(changingArgs)) {
					valuesProvider.state = changingArgs.NewValue as string;
					PdfWidgetAnnotation widget = Widget;
					if (widget != null)
						widget.AppearanceName = strValue;
					if (kids != null) {
						if (strValue != null && strValue != OffStateName) {
							bool found = false;
							foreach (PdfButtonFormField kid in kids)
								if (kid.HasAppearance(strValue)) {
									found = true;
									break;
								}
							if (!found) {
								SetValue(OffStateName, rebuildAppearanceActions);
								return;
							}
						}
						foreach (PdfButtonFormField kid in kids) {
							string newValue = kid.HasAppearance(strValue) ? strValue : OffStateName;
							PdfWidgetAnnotation kidWidget = kid.Widget;
							if (kidWidget != null)
								kidWidget.AppearanceName = newValue;
							kid.state = strValue;
						}
					}
					if (isModified) {
						SetFormModifiedState(rebuildAppearanceActions);
						RaiseFieldChanged(oldState, newState);
					}
				}
			}
		}
		protected internal override void FillDictionary(PdfWriterDictionary dictionary) {
			base.FillDictionary(dictionary);
			dictionary.AddName(valueDictionaryKey, state);
			dictionary.AddName(defaultValuDictionaryKey, defaultState);
			if (kidsState != null)
				dictionary.Add(optionsDictionaryKey, kidsState);
		}
	}
}
