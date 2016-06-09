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
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Editors {
	public abstract class PopupBrushEditBase : PopupBaseEdit, IPopupBrushEdit {
		public static readonly DependencyProperty ChipBorderBrushProperty;
		static readonly DependencyPropertyKey BrushTypePropertyKey;
		public static readonly DependencyProperty BrushTypeProperty;
		public static readonly DependencyProperty AllowEditBrushTypeProperty;
		static PopupBrushEditBase() {
			Type ownerType = typeof(PopupBrushEditBase);
			FocusPopupOnOpenProperty.OverrideMetadata(typeof(PopupBrushEditBase), new FrameworkPropertyMetadata(true));
			ValidateOnTextInputProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(false));
			BrushTypePropertyKey = DependencyPropertyManager.RegisterReadOnly("BrushType", typeof(BrushType), ownerType,
				new FrameworkPropertyMetadata(BrushType.SolidColorBrush, FrameworkPropertyMetadataOptions.None, (o, args) => ((PopupBrushEditBase)o).BrushTypeChanged((BrushType)args.OldValue, (BrushType)args.NewValue)));
			BrushTypeProperty = BrushTypePropertyKey.DependencyProperty;
			ChipBorderBrushProperty = DependencyPropertyManager.Register("ChipBorderBrush", typeof(SolidColorBrush), ownerType, new FrameworkPropertyMetadata(null));
			AllowEditBrushTypeProperty = DependencyPropertyManager.Register("AllowEditBrushType", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
		}
		protected internal override FrameworkElement PopupElement { get { return VisualClient.InnerEditor; } }
		protected internal override Type StyleSettingsType { get { return typeof(PopupBrushEditStyleSettingsBase); } }
		new PopupBrushEditStrategy EditStrategy { get { return base.EditStrategy as PopupBrushEditStrategy; } }
		public bool AllowEditBrushType {
			get { return (bool)GetValue(AllowEditBrushTypeProperty); }
			set { SetValue(AllowEditBrushTypeProperty, value); }
		}
		public BrushType BrushType {
			get { return (BrushType)GetValue(BrushTypeProperty); }
			internal set { SetValue(BrushTypePropertyKey, value); }
		}
		public SolidColorBrush ChipBorderBrush {
			get { return (SolidColorBrush)GetValue(ChipBorderBrushProperty); }
			set { SetValue(ChipBorderBrushProperty, value); }
		}
		protected PopupBrushEditBase() {
			this.SetDefaultStyleKey(typeof(PopupBrushEditBase));
		}
		protected override EditStrategyBase CreateEditStrategy() {
			return new PopupBrushEditStrategy(this);
		}
		protected internal override BaseEditStyleSettings CreateStyleSettings() {
			return new PopupBrushEditStyleSettings();
		}
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new PopupBrushEditPropertyProvider(this);
		}
		protected virtual void BrushTypeChanged(BrushType oldValue, BrushType newValue) {
			EditStrategy.BrushTypeChanged(oldValue, newValue);
		}
		protected internal override TextInputSettingsBase CreateTextInputSettings() {
			return new TextInputColorEditAutoCompleteSettings(this);
		}
		protected internal override void DestroyPopupContent(EditorPopupBase popup) {
			base.DestroyPopupContent(popup);
			EditStrategy.DestroyPopup();
		}
		protected override bool IsClosePopupWithAcceptGesture(Key key, ModifierKeys modifiers) {
			return base.IsClosePopupWithAcceptGesture(key, modifiers) || EditStrategy.IsClosePopupWithAcceptGesture(key, modifiers);
		}
		protected override void AcceptPopupValue() {
			base.AcceptPopupValue();
			EditStrategy.AcceptPopupValue();
		}
		#region IPopupBrushEdit
		PopupBrushValue IPopupBrushEdit.GetPopupBrushValue(BrushType brushType) {
			return EditStrategy.GetPopupEditValue(brushType);
		}
		#endregion
	}
}
