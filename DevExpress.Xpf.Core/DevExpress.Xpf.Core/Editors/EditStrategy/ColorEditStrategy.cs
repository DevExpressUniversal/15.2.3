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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Automation.Peers;
using DevExpress.Xpf.Editors.Internal;
#if !SL
using DevExpress.Data.Access;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Services;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Utils;
using DevExpress.Data.Mask;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Automation;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Bars;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Data;
using DevExpress.Utils;
using PopupColorEditValidator = DevExpress.Xpf.Editors.Services.PopupColorEditValidator;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Services;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Editors.WPFCompatibility.Extensions;
using DevExpress.Data.Mask;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Automation;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Bars;
using System.Windows.Media;
#endif
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using DevExpress.Utils;
using PopupColorEditValidator = DevExpress.Xpf.Editors.Validation.Native.PopupColorEditValidator;
#endif
namespace DevExpress.Xpf.Editors {
	public class ColorEditStrategy : EditStrategyBase {
		public ColorEditStrategy(ColorEdit editor) : base(editor) { }
		protected new ColorEdit Editor { get { return (ColorEdit)base.Editor; } }
		public virtual object CoerceColor(Color color) {
			return CoerceValue(ColorEdit.ColorProperty, color);
		}
		public virtual void OnColorChanged(Color oldValue, Color newValue) {
			if(ShouldLockUpdate)
				return;
			SyncWithValue(ColorEdit.ColorProperty, oldValue, newValue);
		}
		public virtual void OnGalleryColorChanged(Color color) {
			ValueContainer.SetEditValue(color, UpdateEditorSource.ValueChanging);
		}
		public virtual void OnResetButtonClick() {
			ValueContainer.SetEditValue(Editor.DefaultColor, UpdateEditorSource.ValueChanging);
		}
		public virtual void OnNoColorButtonClick() {
			ValueContainer.SetEditValue(ColorEdit.EmptyColor, UpdateEditorSource.ValueChanging);
		}
		protected override void SyncWithValueInternal() {
			UpdateItemsState();
		}
		protected override void SyncEditCorePropertiesInternal() {
			base.SyncEditCorePropertiesInternal();
			UpdateItemsState();
		}
		public virtual void UpdateItemsState() {
			Gallery gallery = Editor.Gallery;
			if(gallery == null) return;
			Color color = Editor.Color;
			foreach(GalleryItemGroup group in gallery.Groups) {
				foreach(ColorGalleryItem item in group.Items) {
					item.IsChecked = (color == item.Color);
					item.Hint = Editor.GetColorNameCore(item.Color);
				}
			}
		}
		protected override void RegisterUpdateCallbacks() {
			base.RegisterUpdateCallbacks();
			PropertyUpdater.Register(ColorEdit.ColorProperty, baseValue => baseValue, baseValue => ColorEditHelper.GetColorFromValue(baseValue));
		}
		public override void RaiseValueChangedEvents(object oldValue, object newValue) {
			base.RaiseValueChangedEvents(oldValue, newValue);
			if(ShouldLockRaiseEvents)
				return;
			if(oldValue != newValue)
				Editor.RaiseColorChanged();
		}
	}
	public class PopupColorEditStrategy : PopupBaseEditStrategy {
		public PopupColorEditStrategy(PopupColorEdit edit)
			: base(edit) {
		}
		protected new PopupColorEdit Editor { get { return (PopupColorEdit)base.Editor; } }
		protected ColorEdit ColorEditControl { get { return Editor.ColorEditControl; } }
		protected override void SyncWithValueInternal() {
			SyncProperties();
			base.SyncWithValueInternal();
		}
		public virtual void SyncProperties() {
			if(ColorEditControl == null)
				return;
			ColorEditControl.Color = ColorEditHelper.GetColorFromValue(EditValue);
			ColorEditControl.ColumnCount = Editor.ColumnCount;
			ColorEditControl.RecentColors.Assign(Editor.RecentColors);
			ColorEditControl.DefaultColor = Editor.DefaultColor;
			ColorEditControl.ToolTipColorDisplayFormat = Editor.ColorDisplayFormat;
			ColorEditControl.ShowDefaultColorButton = Editor.ShowDefaultColorButton;
			ColorEditControl.ShowNoColorButton = Editor.ShowNoColorButton;
			ColorEditControl.ShowMoreColorsButton = Editor.ShowMoreColorsButton;
			ColorEditControl.DefaultColorButtonContent = Editor.DefaultColorButtonContent;
			ColorEditControl.NoColorButtonContent = Editor.NoColorButtonContent;
			ColorEditControl.MoreColorsButtonContent = Editor.MoreColorsButtonContent;
			ColorEditControl.ChipMargin = Editor.ChipMargin;
			ColorEditControl.ChipSize = Editor.ChipSize;
			ColorEditControl.ChipBorderBrush = Editor.ChipBorderBrush;
			ColorEditControl.Palettes = Editor.Palettes;
			ColorEditControl.IsReadOnly = Editor.IsReadOnly;
		}
		public virtual void OnColorChanged(Color oldValue, Color newValue) {
			if(ShouldLockUpdate)
				return;
			SyncWithValue(PopupColorEdit.ColorProperty, oldValue, newValue);
		}
		public virtual object CoerceColor(Color color) {
			return CoerceValue(PopupColorEdit.ColorProperty, color);
		}
		public virtual void AcceptPopupValue() {
			if(ColorEditControl == null || Editor.IsReadOnly)
				return;
			ValueContainer.SetEditValue(ColorEditControl.Color, UpdateEditorSource.ValueChanging);
			UpdateDisplayText();
		}
		protected override void RegisterUpdateCallbacks() {
			base.RegisterUpdateCallbacks();
			PropertyUpdater.Register(PopupColorEdit.ColorProperty, baseValue => baseValue, baseValue => ColorEditHelper.GetColorFromValue(baseValue));
		}
		protected internal override object ConvertEditValueForFormatDisplayText(object convertedValue) {
			if (string.IsNullOrEmpty(Editor.DisplayFormatString))
				return Editor.GetColorNameCore(ColorEditHelper.GetColorFromValue(convertedValue));
			return base.ConvertEditValueForFormatDisplayText(convertedValue);
		}
		public override void RaiseValueChangedEvents(object oldValue, object newValue) {
			base.RaiseValueChangedEvents(oldValue, newValue);
			if(ShouldLockRaiseEvents)
				return;
			if(oldValue != newValue)
				Editor.RaiseColorChanged();
		}
		public virtual void OnAddCustomColor(Color color) {
			Editor.RecentColors.Add(color);
			ValueContainer.SetEditValue(color, UpdateEditorSource.ValueChanging);
		}
		protected override EditorSpecificValidator CreateEditorValidatorService() {
			return new Services.PopupColorEditValidator(Editor);
		}
		public override bool ProvideEditValue(object value, out object provideValue, UpdateEditorSource updateSource) {
			provideValue = Validator.ProcessConversion(value);
			return true;
		}
		protected override void UpdateDisplayTextInternal() {
			if (!AllowEditing) {
				base.UpdateDisplayTextInternal();
				return;
			}
			CursorPositionSnapshot snapshot = new CursorPositionSnapshot(EditBox.SelectionStart, EditBox.SelectionLength, EditBox.Text, false);
			base.UpdateDisplayTextInternal();
			snapshot.ApplyToEdit(Editor);
		}
	}
}
