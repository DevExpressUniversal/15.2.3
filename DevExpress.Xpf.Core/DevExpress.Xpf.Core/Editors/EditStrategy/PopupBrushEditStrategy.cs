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

using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Services;
using PopupBrushEditValidator = DevExpress.Xpf.Editors.Services.PopupBrushEditValidator;
using DevExpress.Xpf.Editors.Validation.Native;
using System;
namespace DevExpress.Xpf.Editors {
	public class PopupBrushEditStrategy : PopupBaseEditStrategy {
		new PopupBrushEditPropertyProvider PropertyProvider { get { return base.PropertyProvider as PopupBrushEditPropertyProvider; } }
		VisualClientOwner VisualClient { get { return Editor.VisualClient; } }
		new PopupBrushEditBase Editor { get { return base.Editor as PopupBrushEditBase; } }
		PostponedAction AcceptPopupValueAction { get; set; }
		public PopupBrushEditStrategy(PopupBrushEditBase editor)
			: base(editor) {
			AcceptPopupValueAction = new PostponedAction(() => Editor.IsPopupOpen);
		}
		protected override void SyncWithValueInternal() {
			base.SyncWithValueInternal();
			PropertyProvider.Brush = BrushConverter.ToBrushType(ValueContainer.EditValue, Editor.BrushType);
		}
		protected override EditorSpecificValidator CreateEditorValidatorService() {
			return new PopupBrushEditValidator(Editor);
		}
		protected override BaseEditingSettingsService CreateTextInputSettingsService() {
			return new PopupBrushEditSettingsService(Editor);
		}
		public virtual void AcceptPopupValue() {
			AcceptPopupValueAction.PerformPostpone(() => AcceptPopupValueInternal(VisualClient.GetSelectedItem()));
		}
		protected virtual void AcceptPopupValueInternal(object editValue) {
			var value = editValue as PopupBrushValue;
			if (value == null)
				return;
			UpdateStyleSettings(value.BrushType);
			var brush = BrushConverter.ToBrushType(value.Brush, value.BrushType);
			ValueContainer.SetEditValue(brush, UpdateEditorSource.ValueChanging);
			UpdateDisplayText();
			SelectAll();
		}
		public virtual void DestroyPopup() {
			AcceptPopupValueAction.PerformForce();
		}
		public virtual bool IsClosePopupWithAcceptGesture(Key key, ModifierKeys modifiers) {
			return key == Key.Enter;
		}
		public virtual PopupBrushValue GetPopupEditValue(BrushType brushType) {
			BrushType actualBrushType = BrushConverter.GetBrushType(ValueContainer.EditValue, brushType);
			object brush = BrushConverter.ToBrushType(ValueContainer.EditValue, actualBrushType);
			if (actualBrushType == BrushType.SolidColorBrush)
				return new PopupSolidColorBrushValue() { BrushType = actualBrushType, Brush = brush };
			if (actualBrushType == BrushType.LinearGradientBrush)
				return new PopupLinearGradientBrushValue() { BrushType = actualBrushType, Brush = brush };
			if (actualBrushType == BrushType.RadialGradientBrush)
				return new PopupRadialGradientBrushValue() { BrushType = actualBrushType, Brush = brush };
			return new PopupBrushValue() { BrushType = actualBrushType, Brush = brush};
		}
		protected virtual void UpdateStyleSettings(BrushType brushType) {
			if (Editor.BrushType == BrushType.AutoDetect)
				return;
			switch (brushType) {
				case BrushType.None:
				case BrushType.SolidColorBrush:
					Editor.StyleSettings = new PopupSolidColorBrushEditStyleSettings();
					break;
				case BrushType.LinearGradientBrush:
					Editor.StyleSettings = new PopupLinearGradientBrushEditStyleSettings();
					break;
				case BrushType.RadialGradientBrush:
					Editor.StyleSettings = new PopupRadialGradientBrushEditStyleSettings();
					break;
			}
		}
		public void BrushTypeChanged(BrushType oldValue, BrushType newValue) {
			SyncWithValue();
		}
	}
}
