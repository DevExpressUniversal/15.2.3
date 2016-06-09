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

using DevExpress.Data.Helpers;
using DevExpress.Entity.Model;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using System;
using System.Globalization;
using System.Windows;
namespace DevExpress.Mvvm.UI.Native.ViewGenerator {
	public static class AttributesApplier {
		public static void ApplyDisplayFormatAttributes(IEdmPropertyInfo propertyInfo,
			Action<string> setNullText,
			Action<string> setDisplayFormat,
			Action setNotConvertEmptyStringsToNull) {
			if(!string.IsNullOrEmpty(propertyInfo.Attributes.NullDisplayText)) setNullText.Do(x => x(propertyInfo.Attributes.NullDisplayText));
			if(!propertyInfo.Attributes.ConvertEmptyStringToNull) setNotConvertEmptyStringsToNull.Do(x => x());
			if(!string.IsNullOrEmpty(propertyInfo.Attributes.DataFormatString)) setDisplayFormat.Do(x => x(propertyInfo.Attributes.DataFormatString));
		}
		public static void ApplyMaskAttributes(MaskInfo maskInfo,
		   Action<string> setMask,
		   Action setUseMaskAsDisplayFormat,
		   Action<CultureInfo> setCulture,
		   Action setAutomaticallyAdvanceCaret,
		   Action<char> setPlaceHolder,
		   Action setNotSaveLiteral,
		   Action setNotShowPlaceHolders,
		   Action setNotIgnoreBlank) {
			if(!maskInfo.IsDefaultMask) setMask.Do(x => x(maskInfo.Mask));
			if(maskInfo.UseAsDisplayFormat != MaskInfo.DefaultUseAsDisplayFormatValue) setUseMaskAsDisplayFormat.Do(x => x());
			if(maskInfo.Culture != null) setCulture.Do(x => x(maskInfo.Culture));
			if(maskInfo.AutomaticallyAdvanceCaret != MaskInfo.DefaultAutomaticallyAdvanceCaretValue) setAutomaticallyAdvanceCaret.Do(x => x());
			if(maskInfo.PlaceHolder != MaskInfo.DefaultPlaceHolderValue) setPlaceHolder.Do(x => x(maskInfo.PlaceHolder));
			if(maskInfo.SaveLiteral != MaskInfo.DefaultSaveLiteralValue) setNotSaveLiteral.Do(x => x());
			if(maskInfo.ShowPlaceHolders != MaskInfo.DefaultShowPlaceHoldersValue) setNotShowPlaceHolders.Do(x => x());
			if(maskInfo.IgnoreBlank != MaskInfo.DefaultIgnoreBlankValue) setNotIgnoreBlank.Do(x => x());
		}
		public static void ApplyBaseAttributes(IEdmPropertyInfo propertyInfo,
			Action<string> setDisplayMember,
			Action<string> setDisplayName,
			Action<string> setDisplayShortName,
			Action<string> setDescription,
			Action setReadOnly,
			Action<bool?> setEditable,
			Action setInvisible,
			Action setHidden,
			Action setRequired) {
			string caption = propertyInfo.HasDisplayAttribute() ? propertyInfo.GetDisplayName() : null;
			string shortName = propertyInfo.Attributes.ShortName ?? caption;
			string name = propertyInfo.Attributes.Name ?? shortName;
			if(shortName != null) {
				setDisplayShortName.Do(x => x(shortName));
				setDisplayName.Do(x => x(name));
			}
			setDisplayMember.Do(x => x(propertyInfo.Name));
			var description = propertyInfo.Attributes.Description;
			if(!string.IsNullOrEmpty(description)) setDescription.Do(x => x(description));
			var readOnly = propertyInfo.Attributes.IsReadOnly.HasValue ? propertyInfo.Attributes.IsReadOnly.Value : propertyInfo.IsReadOnly;
			if(readOnly) setReadOnly.Do(x => x());
			var editable = propertyInfo.Attributes.AllowEdit;
			setEditable.Do(x => x(editable));
			int visibleIndex = propertyInfo.Attributes.Order.HasValue ? propertyInfo.Attributes.Order.Value : int.MaxValue;
			if(visibleIndex < 0) setInvisible.Do(x => x());
			if(propertyInfo.Attributes.Hidden()) setHidden.Do(x => x());
			if(propertyInfo.Attributes.Required()) setRequired.Do(x => x());
		}
		public static void ApplyDisplayFormatAttributesForEditor(IEdmPropertyInfo propertyInfo, Func<IModelItem> getOrCreateEditor, Action<IModelItem, DependencyProperty, object> setValueMethod = null) {
			var setValue = GetMethodForSettingValue(setValueMethod);
			ApplyDisplayFormatAttributes(propertyInfo: propertyInfo,
				setNullText: x => setValue(getOrCreateEditor(), BaseEdit.NullTextProperty, x),
				setDisplayFormat: x => setValue(getOrCreateEditor(), BaseEdit.DisplayFormatStringProperty, x),
				setNotConvertEmptyStringsToNull: () => setValue(getOrCreateEditor(), BaseEdit.ShowNullTextForEmptyValueProperty, false));
		}
		public static void ApplyDisplayFormatAttributesForEditSettings(IEdmPropertyInfo propertyInfo, Func<IModelItem> getOrCreateEditSettings, Action<IModelItem, DependencyProperty, object> setValueMethod = null) {
			var setValue = GetMethodForSettingValue(setValueMethod);
			ApplyDisplayFormatAttributes(propertyInfo: propertyInfo,
				setNullText: x => setValue(getOrCreateEditSettings(), BaseEditSettings.NullTextProperty, x),
				setDisplayFormat: x => setValue(getOrCreateEditSettings(), BaseEditSettings.DisplayFormatProperty, x),
				setNotConvertEmptyStringsToNull: null);
		}
		public static void ApplyMaskAttributesForEditor(MaskInfo maskInfo, Func<IModelItem> getOrCreateEditor, Action<IModelItem, DependencyProperty, object> setValueMethod = null) {
			var setValue = GetMethodForSettingValue(setValueMethod);
			ApplyMaskAttributes(maskInfo: maskInfo,
				setMask: x => setValue(getOrCreateEditor(), TextEdit.MaskProperty, x),
				setUseMaskAsDisplayFormat: () => setValue(getOrCreateEditor(), TextEdit.MaskUseAsDisplayFormatProperty, true),
				setCulture: x => setValue(getOrCreateEditor(), TextEdit.MaskCultureProperty, x),
				setAutomaticallyAdvanceCaret: () => setValue(getOrCreateEditor(), TextEdit.MaskTypeProperty, MaskType.DateTimeAdvancingCaret),
				setPlaceHolder: x => setValue(getOrCreateEditor(), TextEdit.MaskPlaceHolderProperty, x),
				setNotSaveLiteral: () => setValue(getOrCreateEditor(), TextEdit.MaskSaveLiteralProperty, false),
				setNotIgnoreBlank: () => setValue(getOrCreateEditor(), TextEdit.MaskIgnoreBlankProperty, false),
				setNotShowPlaceHolders: () => setValue(getOrCreateEditor(), TextEdit.MaskShowPlaceHoldersProperty, false));
		}
		public static void ApplyMaskAttributesForEditSettings(MaskInfo maskInfo, Func<IModelItem> getOrCreateEditSettings, Action<IModelItem, DependencyProperty, object> setValueMethod = null) {
			var setValue = GetMethodForSettingValue(setValueMethod);
			ApplyMaskAttributes(maskInfo: maskInfo,
				setMask: x => setValue(getOrCreateEditSettings(), TextEditSettings.MaskProperty, x),
				setUseMaskAsDisplayFormat: () => setValue(getOrCreateEditSettings(), TextEditSettings.MaskUseAsDisplayFormatProperty, true),
				setCulture: x => setValue(getOrCreateEditSettings(), TextEditSettings.MaskCultureProperty, x),
				setAutomaticallyAdvanceCaret: () => setValue(getOrCreateEditSettings(), TextEditSettings.MaskTypeProperty, MaskType.DateTimeAdvancingCaret),
				setPlaceHolder: x => setValue(getOrCreateEditSettings(), TextEditSettings.MaskPlaceHolderProperty, x),
				setNotSaveLiteral: () => setValue(getOrCreateEditSettings(), TextEditSettings.MaskSaveLiteralProperty, false),
				setNotIgnoreBlank: () => setValue(getOrCreateEditSettings(), TextEditSettings.MaskIgnoreBlankProperty, false),
				setNotShowPlaceHolders: () => setValue(getOrCreateEditSettings(), TextEditSettings.MaskShowPlaceHoldersProperty, false));
		}
		public static void ApplyBaseAttributesForLayoutItem(IEdmPropertyInfo propertyInfo, IModelItem layoutItem, IModelItem editor, Func<string, string> getLabelByPropertyName = null) {
			if(getLabelByPropertyName == null) getLabelByPropertyName = x => x;
			var editorSetValue = GetMethodForSettingValue((modelItem, dp, v) => {
				if(modelItem != null) modelItem.SetValue(dp, v, false);
			});
			ApplyBaseAttributes(propertyInfo: propertyInfo,
				setDisplayMember: x => layoutItem.Properties["Label"].SetValueIfNotSet(getLabelByPropertyName(x)),
				setDisplayName: x => layoutItem.Properties["Label"].SetValueIfNotSet(x),
				setDisplayShortName: null,
				setDescription: x => layoutItem.Properties["ToolTip"].SetValueIfNotSet(x),
				setReadOnly: () => editorSetValue(editor, BaseEdit.IsReadOnlyProperty, true),
				setEditable: x => { if(x != null && !x.Value) editorSetValue(editor, BaseEdit.IsReadOnlyProperty, true); },
				setInvisible: null,
				setHidden: null,
				setRequired: () => layoutItem.Properties["IsRequired"].SetValue(true));
		}
		public static void ApplyBaseAttributesForFilterColumn(IEdmPropertyInfo propertyInfo, IModelItem filterColumn) {
			ApplyBaseAttributes(propertyInfo: propertyInfo,
				setDisplayMember: x => filterColumn.Properties["FieldName"].SetValue(x),
				setDisplayName: x => filterColumn.Properties["ColumnCaption"].SetValue(x),
				setDisplayShortName: null,
				setDescription: null,
				setReadOnly: null,
				setEditable: null,
				setInvisible: null,
				setHidden: null,
				setRequired: null);
		}
		static Action<IModelItem, DependencyProperty, object> GetMethodForSettingValue(Action<IModelItem, DependencyProperty, object> customSetValueMethod) {
			return customSetValueMethod ??
				((modelItem, dp, v) => { if(modelItem != null) modelItem.SetValueIfNotSet(dp, v, false); });
		}
#if DEBUGTEST
		internal static string ApplyDisplayFormatAttributesForTests(IEdmPropertyInfo propertyInfo) {
			string log = string.Empty;
			ApplyDisplayFormatAttributes(propertyInfo: propertyInfo,
				setNullText: x => log += string.Format("{{NullText:{0}}}", x),
				setDisplayFormat: x => log += string.Format("{{DisplayFormat:{0}}}", x),
				setNotConvertEmptyStringsToNull: () => log += "{NotConvertEmptyStringsToNull}");
			return log;
		}
		internal static string ApplyMaskAttributesForEditorForTests(MaskInfo maskInfo, string defaultMask, bool logMaskUseAsDisplayFormat) {
			string log = string.Empty;
			ApplyMaskAttributes(maskInfo: maskInfo,
				setMask: x => log += x != defaultMask ? "{Mask:" + x + "}" : string.Empty,
				setUseMaskAsDisplayFormat: () => { if(logMaskUseAsDisplayFormat) log += "{MaskUseAsDisplayFormat}"; },
				setCulture: x => log += string.Format("{{MaskCulture:{0}}}", x.Name),
				setAutomaticallyAdvanceCaret: () => log += "{MaskAutomaticallyAdvanceCaret}",
				setPlaceHolder: x => log += string.Format("{{MaskPlaceHolder:{0}}}", x),
				setNotSaveLiteral: () => log += "{MaskNotSaveLiteral}",
				setNotIgnoreBlank: () => log += "{MaskNotIgnoreBlank}",
				setNotShowPlaceHolders: () => log += "{MaskNotShowPlaceHolders}");
			return log;
		}
		internal static string ApplyBaseAttributesForTests(IEdmPropertyInfo propertyInfo) {
			string log = string.Empty;
			string shortName = null;
			string name = null;
			ApplyBaseAttributes(propertyInfo: propertyInfo,
				setDisplayName: x => name = x,
				setDisplayShortName: x => shortName = x,
				setDisplayMember: x => {
					log += x;
					if(!string.IsNullOrEmpty(name)) {
						log += string.Format("{{DisplayName:{0}}}", name);
						if(name != shortName) log += string.Format("{{DisplayShortName:{0}}}", shortName);
					}
				},
				setDescription: x => log += string.Format("{{Description:{0}}}", x),
				setReadOnly: () => log += "{ReadOnly}",
				setEditable: x => { if(x != null && !x.Value) log += "{NonEditable}"; },
				setInvisible: () => log += "{Invisible}",
				setHidden: () => log += "{Hidden}",
				setRequired: () => log += "{Required}");
			return log;
		}
#endif
	}
	public interface IDisplayFormatAttributesApplier {
		void SetNullText(string nullText);
		void SetDisplayFormat(string format);
		void SetNotConvertEmptyStringsToNull();
	}
	public interface IEditorAttributesApplier : IDisplayFormatAttributesApplier {
		void SetDisplayMember(string propertyName);
		void SetDisplayName(string shortName, string name);
		void SetDescription(string description);
		void SetReadOnly();
		void SetEditable(bool? editable);
		void SetInvisible();
		void SetHidden();
		void SetRequired();
	}
}
