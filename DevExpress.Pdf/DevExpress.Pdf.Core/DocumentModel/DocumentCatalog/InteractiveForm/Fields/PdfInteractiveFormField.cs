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
namespace DevExpress.Pdf {
	[Flags]
	public enum PdfInteractiveFormFieldFlags {
		None = 0x0000000,
		ReadOnly = 0x0000001,
		Required = 0x0000002,
		NoExport = 0x0000004,
		Multiline = 0x0001000,
		Password = 0x0002000,
		NoToggleToOff = 0x0004000,
		Radio = 0x0008000,
		PushButton = 0x0010000,
		Combo = 0x0020000,
		Edit = 0x0040000,
		Sort = 0x0080000,
		FileSelect = 0x0100000,
		MultiSelect = 0x0200000,
		DoNotSpellCheck = 0x0400000,
		DoNotScroll = 0x0800000,
		Comb = 0x1000000,
		RichText = 0x2000000,
		RadiosInUnison = 0x2000000,
		CommitOnSelChange = 0x4000000
	}
	public class PdfInteractiveFormField : PdfObject {
		internal const string ParentDictionaryKey = "Parent";
		const string formTypeDictionaryKey = "FT";
		const string kidsDictionaryKey = "Kids";
		const string nameDictionaryKey = "T";
		const string alternateNameDictionaryKey = "TU";
		const string mappingNameDictionaryKey = "TM";
		const string flagsDictionaryKey = "Ff";
		const string actionsDictionaryKey = "AA";
		const string defaultStyleDictionaryKey = "DS";
		const string richTextDataDictionaryKey = "RV";
		internal static PdfInteractiveFormField Parse(PdfInteractiveForm form, PdfInteractiveFormField parent, PdfReaderDictionary dictionary, PdfObjectReference reference) {
			if (dictionary == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			string type = dictionary.GetName(formTypeDictionaryKey);
			if (type == null && parent != null)
				type = parent.FieldType;
			if (type == null)
				return dictionary.ContainsKey(kidsDictionaryKey) ? new PdfInteractiveFormField(form, parent, dictionary, reference) : null;
			switch (type) {
				case PdfButtonFormField.Type:
					return new PdfButtonFormField(form, parent, dictionary, reference);
				case PdfTextFormField.Type:
					return new PdfTextFormField(form, parent, dictionary, reference);
				case PdfChoiceFormField.Type:
					return new PdfChoiceFormField(form, parent, dictionary, reference);
				case PdfSignatureFormField.Type:
					return new PdfSignatureFormField(form, parent, dictionary, reference);
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					return null;
			}
		}
		readonly PdfInteractiveFormField parent;
		readonly IList<PdfInteractiveFormField> kids;
		readonly PdfWidgetAnnotation widget;
		readonly string alternateName;
		readonly string mappingName;
		readonly PdfInteractiveFormFieldFlags? flags;
		readonly PdfAdditionalActions actions;
		readonly PdfCommandList appearanceCommands;
		readonly PdfTextJustification textJustification;
		readonly string defaultStyle;
		readonly string richTextData;
		readonly PdfInteractiveForm form;
		readonly PdfInteractiveFormField valuesProvider;
		PdfInteractiveFormFieldTextState textState;
		string name;
		public PdfInteractiveFormField Parent { get { return parent; } }
		public IList<PdfInteractiveFormField> Kids { get { return kids; } }
		public PdfInteractiveForm Form { get { return form; } }
		public PdfWidgetAnnotation Widget { get { return widget; } }
		public string Name { 
			get { return name; } 
			internal set { name = value; } 
		}
		public string AlternateName { get { return valuesProvider.alternateName; } }
		public string MappingName { get { return valuesProvider.mappingName; } }
		public PdfInteractiveFormFieldFlags Flags {
			get {
				if (flags.HasValue)
					return flags.Value;
				return parent == null ? PdfInteractiveFormFieldFlags.None : parent.Flags;
			}
		}
		public PdfInteractiveFormFieldActions Actions { get { return actions == null ? null : actions.InteractiveFormFieldActions; } }
		public IEnumerable<PdfCommand> AppearanceCommands { get { return appearanceCommands; } }
		public PdfTextJustification TextJustification { get { return textJustification; } }
		public string DefaultStyle { get { return defaultStyle; } }
		public string RichTextData { get { return valuesProvider.richTextData; } }
		internal string FullName {
			get {
				if (parent == null)
					return name;
				string parentName = parent.FullName;
				if (String.IsNullOrEmpty(parentName))
					return name;
				return String.IsNullOrEmpty(name) ? parentName : (parentName  + "." + name);
			}
		}
		internal PdfInteractiveFormFieldTextState TextState { 
			get { 
				if (textState == null)
					textState = new PdfInteractiveFormFieldTextState(this);
				return textState;
			} 
		}
		protected PdfInteractiveFormField ValuesProvider { get { return valuesProvider; } }
		protected virtual string FieldType { get { return null; } }
		protected internal virtual object DefaultValue { get { return null; } }
		protected internal virtual object Value { get { return null; } }
		protected internal virtual bool ShouldHighlight { get { return !Flags.HasFlag(PdfInteractiveFormFieldFlags.ReadOnly); } }
		internal PdfInteractiveFormField(PdfInteractiveForm form, PdfInteractiveFormField parent, PdfReaderDictionary dictionary, PdfObjectReference valueReference) : base(dictionary.Number) {
			this.parent = parent;
			this.form = form;
			PdfObjectCollection objects = dictionary.Objects;
			int? intFlags = dictionary.GetInteger(flagsDictionaryKey);
			flags = intFlags.HasValue ? (PdfInteractiveFormFieldFlags?)intFlags.Value : null;
			IList<object> kidsArray = dictionary.GetArray(kidsDictionaryKey);
			if (kidsArray == null) {
				if (dictionary.GetName(PdfDictionary.DictionaryTypeKey) != PdfAnnotation.DictionaryType && dictionary.GetName(PdfDictionary.DictionarySubtypeKey) != PdfWidgetAnnotation.Type)
					PdfDocumentReader.ThrowIncorrectDataException();
				PdfDocumentCatalog documentCatalog = objects.DocumentCatalog;
				widget = documentCatalog.FindWidget(dictionary.Number);
				if (widget == null) {
					PdfPage page = null;
					object pageValue;
					if (dictionary.TryGetValue(PdfAnnotation.PageDictionaryKey, out pageValue))
						page = documentCatalog.FindPage(pageValue);
					widget = objects.GetAnnotation(page, valueReference) as PdfWidgetAnnotation;
					if (widget == null)
						PdfDocumentReader.ThrowIncorrectDataException();
				}
				widget.InteractiveFormField = this;
			}
			else {
				kids = new List<PdfInteractiveFormField>(kidsArray.Count);
				foreach (object kid in kidsArray)
					kids.Add(objects.GetInteractiveFormField(form, this, kid));
			}
			name = dictionary.GetString(nameDictionaryKey);
			alternateName = dictionary.GetString(alternateNameDictionaryKey);
			mappingName = dictionary.GetString(mappingNameDictionaryKey);
			actions = dictionary.GetAdditionalActions(widget);
			appearanceCommands = dictionary.GetAppearance(form == null ? null : form.Resources);
			textJustification = dictionary.GetTextJustification();
			defaultStyle = dictionary.GetString(defaultStyleDictionaryKey);
			richTextData = dictionary.GetStringAdvanced(richTextDataDictionaryKey);
			valuesProvider = (name == null && parent != null) ? parent : this;
		}
		protected void SetFormModifiedState(IEnumerable<Action> rebuildAppearanceActions) {
			if (form != null)
				form.Modified = true;
			if (rebuildAppearanceActions != null)
				foreach (Action rebuildAppearanceAction in rebuildAppearanceActions)
					rebuildAppearanceAction();
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = widget == null ? null : (widget.ToWritableObject(objects) as PdfWriterDictionary);
			if (dictionary == null) {
				dictionary = new PdfWriterDictionary(objects);
				FillDictionary(dictionary);
			}
			return dictionary;
		}
		protected internal virtual void FillDictionary(PdfWriterDictionary dictionary) {
			dictionary.Add(ParentDictionaryKey, parent);
			dictionary.AddName(formTypeDictionaryKey, FieldType);
			dictionary.AddList(kidsDictionaryKey, kids);
			dictionary.Add(nameDictionaryKey, dictionary.Objects.GetFormFieldName(name), null);
			dictionary.Add(alternateNameDictionaryKey, alternateName, null);
			dictionary.Add(mappingNameDictionaryKey, mappingName, null);
			if (flags.HasValue)
				dictionary.Add(flagsDictionaryKey, (int)flags, (int)PdfInteractiveFormFieldFlags.None);
			if (!dictionary.ContainsKey(PdfAdditionalActions.DictionaryAdditionalActionsKey))
				dictionary.Add(PdfAdditionalActions.DictionaryAdditionalActionsKey, actions);
			if (appearanceCommands != null)
				dictionary.Add(PdfDictionary.DictionaryAppearanceKey, appearanceCommands.ToByteArray(form.Resources));
			if (textJustification != PdfTextJustification.LeftJustified)
				dictionary.Add(PdfDictionary.DictionaryJustificationKey, PdfEnumToValueConverter.Convert(textJustification));
			dictionary.Add(defaultStyleDictionaryKey, defaultStyle, null);
			if (richTextData != null)
				if (richTextData.Length == 0)
					dictionary.Add(richTextDataDictionaryKey, String.Empty);
				else 
					dictionary.AddStream(richTextDataDictionaryKey, richTextData);
		}
		protected internal virtual void SetValue(object value, IEnumerable<Action> rebuildAppearanceActions) {
		}
		protected bool RaiseFieldChanging(PdfInteractiveFormFieldValueChangingEventArgs args) {
			return form == null || form.RaiseFormFieldValueChanging(args);
		}
		protected void RaiseFieldChanged(object oldValue, object newValue) {
			if (form != null)
				form.RaiseFormFieldValueChanged(FullName, oldValue, newValue);
		}
		protected virtual bool AcceptValue(object value) {
			return value == null || (value is string);
		}
	}
}
