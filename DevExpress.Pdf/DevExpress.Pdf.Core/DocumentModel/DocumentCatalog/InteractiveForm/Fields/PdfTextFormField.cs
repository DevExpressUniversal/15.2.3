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
	public class PdfTextFormField : PdfInteractiveFormField {
		internal const string Type = "Tx";
		const string textDictionaryKey = "V";
		const string defaultTextDictionaryKey = "DV";
		const string maxLenDictionaryKey = "MaxLen";
		const PdfInteractiveFormFieldFlags zeroLengthFlagsCheck = PdfInteractiveFormFieldFlags.Multiline | 
			PdfInteractiveFormFieldFlags.Password | PdfInteractiveFormFieldFlags.FileSelect | PdfInteractiveFormFieldFlags.Comb;
		readonly PdfTextFormField valuesProvider;
		readonly string defaultText;
		readonly int? maxLen;
		string text = String.Empty;
		public string DefaultText { get { return valuesProvider.defaultText; } }
		public int? MaxLen { get { return valuesProvider.maxLen; } }
		public string Text { get { return valuesProvider.text; } }
		protected internal override object Value { get { return valuesProvider.text; } }
		protected internal override object DefaultValue { get { return valuesProvider.defaultText; } }
		protected override string FieldType { get { return Type; } }
		internal PdfTextFormField(PdfInteractiveForm form, PdfInteractiveFormField parent, PdfReaderDictionary dictionary, PdfObjectReference valueReference) 
				: base(form, parent, dictionary, valueReference) {
			object value;
			if (dictionary.TryGetValue(textDictionaryKey, out value)) {
				PdfName name = value as PdfName;
				if (name == null) {
					byte[] bytes = dictionary.Objects.TryResolve(value) as byte[];
					if (bytes == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					text = PdfDocumentReader.ConvertToString(bytes);
				}
				else
					text = name.Name;
			}
			defaultText = dictionary.GetString(defaultTextDictionaryKey);
			maxLen = dictionary.GetInteger(maxLenDictionaryKey);
			if (maxLen.HasValue) {
				int maxLenValue = maxLen.Value;
				if (maxLenValue < 0 || (maxLenValue == 0 && (Flags & zeroLengthFlagsCheck) == PdfInteractiveFormFieldFlags.Comb))
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			valuesProvider = ValuesProvider as PdfTextFormField ?? this;
		}
		protected internal override void SetValue(object value, IEnumerable<Action> rebuildAppearanceActions) {
			base.SetValue(value, rebuildAppearanceActions);
			if (!AcceptValue(value))
				throw new ArgumentException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectTextFormFieldValue));
			string[] strings;
			string strValue = value as string;
			if (strValue == null) {
				IList<string> list = value as IList<string>;
				if (list == null)
					strings = null;
				else {
					int count = list.Count;
					strings = new string[count];
					for (int i = 0; i < count; i++)
						strings[i] = list[i];
				}
			}
			else
				strings = strValue.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			if (strings != null) {
				if (Flags.HasFlag(PdfInteractiveFormFieldFlags.Password) && strings.Length > 0) {
					int count = strings.Length;
					string[] passwords = new string[count];
					for (int i = 0; i < count; i++)
						passwords[i] = new string('*', strings[i].Length);
					strings = passwords;
				}
				strValue = String.Join("\r", strings);
			}
			if (strValue == null)
				strValue = String.Empty;
			string text = valuesProvider.text;
			PdfInteractiveFormFieldValueChangingEventArgs changingArgs = new PdfInteractiveFormFieldValueChangingEventArgs(FullName, text, strValue);
			if (text != strValue && !String.Equals(text, value) && RaiseFieldChanging(changingArgs)) {
				string newObject = changingArgs.NewValue as string;
				valuesProvider.text = newObject;
				SetFormModifiedState(rebuildAppearanceActions);
				RaiseFieldChanged(text, newObject);
			}
		}
		protected override bool AcceptValue(object value) {
			return base.AcceptValue(value) || (value is IList<string>);
		}
		protected internal override void FillDictionary(PdfWriterDictionary dictionary) {
			base.FillDictionary(dictionary);
			dictionary.Add(textDictionaryKey, text, null);
			dictionary.Add(defaultTextDictionaryKey, defaultText, null);
			dictionary.AddNullable(maxLenDictionaryKey, maxLen);
		}
	}
}
