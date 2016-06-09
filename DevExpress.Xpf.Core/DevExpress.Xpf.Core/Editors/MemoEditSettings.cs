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
using System.Windows.Controls;
using System.ComponentModel;
#if !SL
using DevExpress.Xpf.Utils;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using Keyboard = DevExpress.Xpf.Editors.WPFCompatibility.SLKeyboard;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors.Settings {
	public partial class MemoEditSettings : PopupBaseEditSettings {
		public static readonly DependencyProperty MemoTextWrappingProperty;
		public static readonly DependencyProperty MemoHorizontalScrollBarVisibilityProperty;
		public static readonly DependencyProperty MemoVerticalScrollBarVisibilityProperty;
		public static readonly DependencyProperty MemoAcceptsReturnProperty;
#if !SL
		public static readonly DependencyProperty MemoAcceptsTabProperty;
#endif
		public static readonly DependencyProperty ShowIconProperty;
		static MemoEditSettings() {
			Type ownerType = typeof(MemoEditSettings);
			MemoTextWrappingProperty = DependencyPropertyManager.Register("MemoTextWrapping", typeof(TextWrapping), ownerType, new FrameworkPropertyMetadata(TextWrapping.NoWrap));
			MemoHorizontalScrollBarVisibilityProperty = DependencyPropertyManager.Register("MemoHorizontalScrollBarVisibility", typeof(ScrollBarVisibility), ownerType, new FrameworkPropertyMetadata(ScrollBarVisibility.Hidden));
			MemoVerticalScrollBarVisibilityProperty = DependencyPropertyManager.Register("MemoVerticalScrollBarVisibility", typeof(ScrollBarVisibility), ownerType, new FrameworkPropertyMetadata(ScrollBarVisibility.Hidden));
			MemoAcceptsReturnProperty = DependencyPropertyManager.Register("MemoAcceptsReturn", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ShowIconProperty = DependencyPropertyManager.Register("ShowIcon", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
#if !SL
			MemoAcceptsTabProperty = DependencyPropertyManager.Register("MemoAcceptsTab", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			IsTextEditableProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(false));
			PopupFooterButtonsProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(DevExpress.Xpf.Editors.PopupFooterButtons.OkCancel));
			ShowSizeGripProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true));
			PopupMinHeightProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(MemoEdit.popupMinHeightConstant));
#endif
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("MemoEditSettingsMemoTextWrapping"),
#endif
Category(EditSettingsCategories.Behavior)]
		public TextWrapping MemoTextWrapping {
			get { return (TextWrapping)GetValue(MemoTextWrappingProperty); }
			set { SetValue(MemoTextWrappingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("MemoEditSettingsMemoAcceptsReturn"),
#endif
Category(EditSettingsCategories.Behavior)]
		public bool MemoAcceptsReturn {
			get { return (bool)GetValue(MemoAcceptsReturnProperty); }
			set { SetValue(MemoAcceptsReturnProperty, value); }
		}
#if !SL
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("MemoEditSettingsMemoAcceptsTab"),
#endif
Category(EditSettingsCategories.Behavior)]
		public bool MemoAcceptsTab {
			get { return (bool)GetValue(MemoAcceptsTabProperty); }
			set { SetValue(MemoAcceptsTabProperty, value); }
		}
#endif
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("MemoEditSettingsMemoHorizontalScrollBarVisibility"),
#endif
Category(EditSettingsCategories.Behavior)]
		public ScrollBarVisibility MemoHorizontalScrollBarVisibility {
			get { return (ScrollBarVisibility)GetValue(MemoHorizontalScrollBarVisibilityProperty); }
			set { SetValue(MemoHorizontalScrollBarVisibilityProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("MemoEditSettingsMemoVerticalScrollBarVisibility"),
#endif
Category(EditSettingsCategories.Behavior)]
		public ScrollBarVisibility MemoVerticalScrollBarVisibility {
			get { return (ScrollBarVisibility)GetValue(MemoVerticalScrollBarVisibilityProperty); }
			set { SetValue(MemoVerticalScrollBarVisibilityProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("MemoEditSettingsShowIcon"),
#endif
Category(EditSettingsCategories.Behavior)]
		public bool ShowIcon {
			get { return (bool)GetValue(ShowIconProperty); }
			set { SetValue(ShowIconProperty, value); }
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			MemoEdit editor = edit as MemoEdit;
			if(editor == null)
				return;
			SetValueFromSettings(MemoTextWrappingProperty, () => editor.MemoTextWrapping = MemoTextWrapping);
			SetValueFromSettings(MemoAcceptsReturnProperty, () => editor.MemoAcceptsReturn = MemoAcceptsReturn);
			SetValueFromSettings(MemoHorizontalScrollBarVisibilityProperty, () => editor.MemoHorizontalScrollBarVisibility = MemoHorizontalScrollBarVisibility);
			SetValueFromSettings(MemoVerticalScrollBarVisibilityProperty, () => editor.MemoVerticalScrollBarVisibility = MemoVerticalScrollBarVisibility);
			SetValueFromSettings(ShowIconProperty, () => editor.ShowIcon = ShowIcon);
#if !SL
			SetValueFromSettings(MemoAcceptsTabProperty, () => editor.MemoAcceptsTab = MemoAcceptsTab);
#endif
		}
	}
}
#if !SL
namespace DevExpress.Xpf.Editors.Settings.Extension {
	public class MemoSettingsExtension : PopupBaseSettingsExtension {
		public TextWrapping MemoTextWrapping { get; set; }
		public bool MemoAcceptsReturn { get; set; }
		public bool MemoAcceptsTab { get; set; }
		public ScrollBarVisibility MemoHorizontalScrollBarVisibility { get; set; }
		public ScrollBarVisibility MemoVerticalScrollBarVisibility { get; set; }
		public bool ShowIcon { get; set; }
		public MemoSettingsExtension() {
			IsTextEditable = false;
			PopupFooterButtons = DevExpress.Xpf.Editors.PopupFooterButtons.OkCancel;
			ShowSizeGrip = true;
			PopupMinHeight = MemoEdit.popupMinHeightConstant;
			MemoTextWrapping = (TextWrapping)MemoEditSettings.MemoTextWrappingProperty.DefaultMetadata.DefaultValue;
			MemoAcceptsReturn = (bool)MemoEditSettings.MemoAcceptsReturnProperty.DefaultMetadata.DefaultValue;
			MemoAcceptsTab = (bool)MemoEditSettings.MemoAcceptsTabProperty.DefaultMetadata.DefaultValue;
			MemoHorizontalScrollBarVisibility = (ScrollBarVisibility)MemoEditSettings.MemoHorizontalScrollBarVisibilityProperty.DefaultMetadata.DefaultValue;
			MemoVerticalScrollBarVisibility = (ScrollBarVisibility)MemoEditSettings.MemoVerticalScrollBarVisibilityProperty.DefaultMetadata.DefaultValue;
			ShowIcon = (bool)MemoEditSettings.ShowIconProperty.DefaultMetadata.DefaultValue;
		}
		protected sealed override PopupBaseEditSettings CreatePopupBaseEditSettings() {
			MemoEditSettings settings = CreateMemoEditSettings();
			settings.MemoTextWrapping = MemoTextWrapping;
			settings.MemoAcceptsReturn = MemoAcceptsReturn;
			settings.MemoAcceptsTab = MemoAcceptsTab;
			settings.MemoHorizontalScrollBarVisibility = MemoHorizontalScrollBarVisibility;
			settings.MemoVerticalScrollBarVisibility = MemoVerticalScrollBarVisibility;
			settings.ShowIcon = ShowIcon;
			return settings;
		}
		protected virtual MemoEditSettings CreateMemoEditSettings() {
			return new MemoEditSettings();
		}
	}
}
#endif
