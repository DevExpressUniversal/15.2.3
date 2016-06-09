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

using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Editors.Validation.Native;
namespace DevExpress.Xpf.Editors.Services {
	public class ValueContainerService : BaseEditBaseService {
		public object EditValue { get { return ValueContainer.EditValue; } }
		public bool HasValueCandidate { get { return ValueContainer.HasValueCandidate; } }
		public bool IsLockedByValueChanging { get { return ValueContainer.IsLockedByValueChanging; } }
		public bool IsPostponedValueChanging { get { return ValueContainer.IsPostponedValueChanging; } }
		public UpdateEditorSource UpdateSource { get { return ValueContainer.UpdateSource; } }
		EditValueContainer ValueContainer { get; set; }
		EditorSpecificValidator Validator { get { return PropertyProvider.GetService<EditorSpecificValidator>(); } }
		public bool HasTempValue { get { return ValueContainer.HasTempValue; } }
		public ValueContainerService(BaseEdit editor) : base(editor) {
			ValueContainer = new EditValueContainer(editor);
		}
		public virtual void SetEditValue(object editValue, UpdateEditorSource updateSource) {
			ValueContainer.SetEditValue(editValue, updateSource);
		}
		public virtual bool ProvideEditValue(object value, out object provideValue, UpdateEditorSource updateSource) {
			provideValue = PropertyProvider.ValueTypeConverter.ConvertBack(value);
			return true;
		}
		public virtual object ProcessConversion(object value, UpdateEditorSource updateSource) {
			return Validator.ProcessConversion(value);
		}
		public void FlushEditValue() {
			ValueContainer.FlushEditValue();			
		}
		public void Reset() {
			ValueContainer.Reset();
		}
		public void UndoTempValue() {
			ValueContainer.UndoTempValue();
		}
		public void FlushEditValueCandidate(object editValue, UpdateEditorSource updateSource) {
			ValueContainer.FlushEditValueCandidate(editValue, updateSource);
		}
		public void UpdatePostMode() {
			ValueContainer.UpdatePostMode();
		}
		public virtual object ConvertEditValueForFormatDisplayText(object convertedValue) {
			return convertedValue;
		}
		public virtual object ConvertEditTextToEditValueCandidate(object editValue) {
			ValueTypeConverter converter = PropertyProvider.InputTextToEditValueConverter;
			object editValueCandidate = converter.Convert(editValue);
			if (converter.ValidationError != null)
				return new InvalidInputValue { DisplayValue = editValue, EditValue = ValueContainer.EditValue };
			return editValueCandidate;
		}
	}
	public class TextInputValueContainerService : ValueContainerService {
		new TextEditPropertyProvider PropertyProvider { get { return (TextEditPropertyProvider)base.PropertyProvider; } }
		TextInputSettingsBase TextInputSettings { get { return PropertyProvider.TextInputSettings; } }
		public TextInputValueContainerService(TextEditBase editor) : base(editor) {
		}
		public override bool ProvideEditValue(object value, out object provideValue, UpdateEditorSource updateSource) {
			object convertedValue = TextInputSettings.ProvideEditValue(value, updateSource);
			return base.ProvideEditValue(convertedValue, out provideValue, updateSource);
		}
		public override object ConvertEditValueForFormatDisplayText(object convertedValue) {
			object result = TextInputSettings.ConvertEditValueForFormatDisplayText(convertedValue);
			return base.ConvertEditValueForFormatDisplayText(result);
		}
		public override object ProcessConversion(object value, UpdateEditorSource updateSource) {
			object result = base.ProcessConversion(value, updateSource);
			return TextInputSettings.ProcessConversion(result, updateSource);
		}
	}
}
