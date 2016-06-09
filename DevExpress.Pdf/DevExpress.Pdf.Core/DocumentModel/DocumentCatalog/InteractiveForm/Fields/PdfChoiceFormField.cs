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
	public class PdfChoiceFormField : PdfInteractiveFormField {
		internal const string Type = "Ch";
		const string topIndexDictionaryKey = "TI";
		const string optionsDictionaryKey = "Opt";
		const string selectedIndicesDictionaryKey = "I";
		const string selectedValuesDictionaryKey = "V";
		const string defaultValuesDictionaryKey = "DV";
		readonly PdfChoiceFormField valuesProvider;
		readonly IList<string> defaultValues;
		readonly IList<PdfOptionsFormFieldOption> options;
		int topIndex;
		IList<int> selectedIndices;
		IList<string> selectedValues;
		public IList<string> DefaultValues { get { return valuesProvider.defaultValues; } }
		public IList<PdfOptionsFormFieldOption> Options { get { return valuesProvider.options; } }
		public int TopIndex { get { return valuesProvider.topIndex; } }
		public IList<int> SelectedIndices { get { return valuesProvider.selectedIndices; } }
		public IList<string> SelectedValues { get { return valuesProvider.selectedValues; } }
		protected internal override object Value {
			get {
				IList<string> selectedValues = valuesProvider.selectedValues;
				if (selectedValues == null)
					return null;
				switch (selectedValues.Count) {
					case 0:
						return null;
					case 1:
						return selectedValues[0];
					default:
						return selectedValues;
				}
			}
		}
		protected internal override object DefaultValue { get { return valuesProvider.defaultValues; } }
		protected override string FieldType { get { return Type; } }
		internal PdfChoiceFormField(PdfInteractiveForm form, PdfInteractiveFormField parent, PdfReaderDictionary dictionary, PdfObjectReference valueReference)
				: base(form, parent, dictionary, valueReference) {
			topIndex = dictionary.GetInteger(topIndexDictionaryKey) ?? 0;
			bool shouldPresentSelectedIndices = false;
			List<string> values = new List<string>();
			IList<object> opt = dictionary.GetArray(optionsDictionaryKey);
			if (opt == null) {
				if (topIndex != 0)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			else {
				options = new List<PdfOptionsFormFieldOption>(opt.Count);
				List<string> exportedValues = new List<string>();
				foreach (object item in opt) {
					PdfOptionsFormFieldOption option = new PdfOptionsFormFieldOption(dictionary, item);
					values.Add(option.Text);
					string exportText = option.ExportText;
					if (exportedValues.Contains(exportText))
						shouldPresentSelectedIndices = true;
					else
						exportedValues.Add(exportText);
					options.Add(option);
				}
				if (topIndex < 0 || topIndex >= options.Count)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			bool isMultiSelect = (Flags & PdfInteractiveFormFieldFlags.MultiSelect) == PdfInteractiveFormFieldFlags.MultiSelect;
			IList<object> i = dictionary.GetArray(selectedIndicesDictionaryKey);
			if (i == null) {
				if (shouldPresentSelectedIndices && isMultiSelect)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			else {
				selectedIndices = new List<int>(i.Count);
				foreach (object item in i) {
					if (!(item is int))
						PdfDocumentReader.ThrowIncorrectDataException();
					int index = (int)item;
					if (index < 0)
						PdfDocumentReader.ThrowIncorrectDataException();
					selectedIndices.Add(index);
				}
			}
			selectedValues = GetValues(dictionary, selectedValuesDictionaryKey, values);
			if (isMultiSelect) {
				int selectedIndicesCount = selectedIndices == null ? 0 : selectedIndices.Count;
				int selectedValuesCount = selectedValues == null ? 0 : selectedValues.Count;
				if (selectedIndicesCount != selectedValuesCount)
					PdfDocumentReader.ThrowIncorrectDataException();
				for (int j = 0; j < selectedValuesCount; j++)
					if (values[selectedIndices[j]] != selectedValues[j])
						PdfDocumentReader.ThrowIncorrectDataException();
			}
			defaultValues = GetValues(dictionary, defaultValuesDictionaryKey, values);
			valuesProvider = ValuesProvider as PdfChoiceFormField ?? this;
		}
		internal void SetTopIndex(int topIndex, IList<Action> rebuildAppearanceActions) {
			int finalTopIndex = topIndex;
			IList<string> selectedValues = valuesProvider.selectedValues;
			IList<PdfOptionsFormFieldOption> options = valuesProvider.options;
			if (selectedValues != null && options != null) {
				int optionsCount = options.Count;
				for (int selIndex = 0; selIndex < optionsCount; selIndex++)
					if (selectedValues.Contains(options[selIndex].Text)) {
						double itemHeight = valuesProvider.TextState.FontSize + 1;
						double contentRectHeight = Widget != null ? Widget.ContentRectangle.Height : 0;
						int visibleItems = (int)(contentRectHeight / itemHeight) - 1;
						int startIndex = selIndex - visibleItems;
						if (startIndex < 0)
							startIndex = 0;
						int endIndex = selIndex <= visibleItems ? selIndex : selIndex + visibleItems;
						if (endIndex >= optionsCount)
							endIndex = optionsCount - 1;
						int valueTopIndex = topIndex;
						if (valueTopIndex < startIndex)
							finalTopIndex = startIndex;
						else if (valueTopIndex > endIndex)
							finalTopIndex = endIndex;
						break;
					}
			}
			if (valuesProvider.topIndex != finalTopIndex) {
				valuesProvider.topIndex = finalTopIndex;
				SetFormModifiedState(rebuildAppearanceActions);
			}
		}
		string ConvertToString(object value, IList<string> values) {
			string stringValue = value as string;
			if (stringValue == null) {
				byte[] bytes = value as byte[];
				if (bytes == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				stringValue = PdfDocumentReader.ConvertToString(bytes);
			}
			if ((Flags & PdfInteractiveFormFieldFlags.Combo) == 0 && !values.Contains(stringValue))
				PdfDocumentReader.ThrowIncorrectDataException();
			return stringValue;
		}
		IList<string> GetValues(PdfReaderDictionary dictionary, string key, IList<string> values) {
			object value;
			if (!dictionary.TryGetValue(key, out value))
				return null;
			value = dictionary.Objects.TryResolve(value);
			if (value == null)
				return null;
			List<string> result = new List<string>();
			IList<object> valueList = value as IList<object>;
			if (valueList == null)
				result.Add(ConvertToString(value, values));
			else
				foreach (object v in valueList)
					result.Add(ConvertToString(v, values));
			return result;
		}
		void WriteValues(PdfWriterDictionary dictionary, string key, IList<string> values) {
			if (values != null)
				if (values.Count == 1 && (Flags & PdfInteractiveFormFieldFlags.MultiSelect) == 0)
					dictionary.Add(key, values[0]);
				else
					dictionary.AddList(key, values, value => (string)value);
		}
		protected internal override void SetValue(object value, IEnumerable<Action> rebuildAppearanceActions) {
			base.SetValue(value, rebuildAppearanceActions);
			if (!AcceptValue(value))
				throw new ArgumentException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectChoiceFormFieldValue));
			string strValue = value as string;
			IList<string> newSelectedValues = strValue == null ? (value as IList<string>) : new string[] { strValue };
			IList<string> selectedValues = valuesProvider.selectedValues;
			bool modified;
			if (selectedValues == null)
				modified = newSelectedValues != null;
			else
				if (newSelectedValues == null) 
					modified = true;
				else {
					int valuesCount = newSelectedValues.Count;
					modified = valuesCount != selectedValues.Count;
					if (!modified)
						for (int i = 0; i < valuesCount; i++)
							if (!selectedValues.Contains(newSelectedValues[i]))
								modified = true;
				}
			if (modified) {
				selectedValues = new List<string>();
				IList<int> selectedIndices = new List<int>();
				if (newSelectedValues != null) {
					IList<PdfOptionsFormFieldOption> options = valuesProvider.options;
					int optionsCount = options == null ? 0 : options.Count;
					if (Flags.HasFlag(PdfInteractiveFormFieldFlags.Combo) && Flags.HasFlag(PdfInteractiveFormFieldFlags.Edit)) {
						if (strValue != null) {
							selectedValues.Add(strValue);
							for (int j = 0; j < optionsCount; j++) {
								if (options[j].Text == strValue) {
									selectedIndices.Add(j);
									break;
								}
							}
						}
					}
					else {
						int valuesCount = newSelectedValues.Count;
						for (int i = 0; i < valuesCount; i++) {
							string selectedValue = newSelectedValues[i];
							for (int j = 0; j < optionsCount; j++) {
								if (options[j].Text == selectedValue) {
									selectedValues.Add(selectedValue);
									selectedIndices.Add(j);
									break;
								}
							}
						}
					}
				}
				IList<string> oldValues = valuesProvider.selectedValues;
				PdfInteractiveFormFieldValueChangingEventArgs changingArgs = new PdfInteractiveFormFieldValueChangingEventArgs(FullName, oldValues, selectedValues);
				if (RaiseFieldChanging(changingArgs)) {
					valuesProvider.selectedValues = changingArgs.NewValue as IList<string>;
					valuesProvider.selectedIndices = selectedIndices;
					SetFormModifiedState(rebuildAppearanceActions);
					RaiseFieldChanged(oldValues, changingArgs.NewValue);
				}
			}
		}
		protected override bool AcceptValue(object value) {
			return base.AcceptValue(value) || value is IList<string>;
		}
		protected internal override void FillDictionary(PdfWriterDictionary dictionary) {
			base.FillDictionary(dictionary);
			dictionary.Add(topIndexDictionaryKey, topIndex, 0);
			dictionary.AddList(optionsDictionaryKey, options, value => ((PdfOptionsFormFieldOption)value).Write());
			dictionary.AddList(selectedIndicesDictionaryKey, selectedIndices, value => (int)value);
			WriteValues(dictionary, selectedValuesDictionaryKey, selectedValues);
			WriteValues(dictionary, defaultValuesDictionaryKey, defaultValues);
		}
	}
}
