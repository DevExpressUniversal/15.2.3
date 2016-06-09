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
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Themes;
#if !SL
using DevExpress.Data.Access;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Data.Mask;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Automation;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
#else
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Editors.WPFCompatibility.Extensions;
using DevExpress.Data.Mask;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Automation;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using SelectionChangedEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventArgs;
using SelectionChangedEventHandler = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventHandler;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
#endif
namespace DevExpress.Xpf.Editors {
	public class PopupBaseEditPropertyProvider : ButtonEditPropertyProvider {
		public static readonly DependencyProperty PopupTopAreaTemplateProperty;
		public static readonly DependencyProperty PopupBottomAreaTemplateProperty;
		public static readonly DependencyProperty PopupViewModelProperty;
		public static readonly DependencyProperty AddNewCommandProperty;
		public PopupBaseEditPropertyProvider(TextEdit editor)
			: base(editor) {
			PopupViewModel = CreatePopupViewModel();
			ResizeGripViewModel = CreateResizeGripViewModel();
		}
		static PopupBaseEditPropertyProvider() {
			Type ownerType = typeof(PopupBaseEditPropertyProvider);
			PopupTopAreaTemplateProperty = DependencyPropertyManager.Register("PopupTopAreaTemplate", typeof(ControlTemplate), ownerType, new PropertyMetadata(null));
			PopupBottomAreaTemplateProperty = DependencyPropertyManager.Register("PopupBottomAreaTemplate", typeof(ControlTemplate), ownerType, new PropertyMetadata(null));
			PopupViewModelProperty = DependencyPropertyManager.Register("PopupViewModel", typeof(PopupViewModel), ownerType, new PropertyMetadata(null));
			AddNewCommandProperty = DependencyPropertyManager.Register("AddNewCommand", typeof(ICommand), ownerType, new PropertyMetadata(null));
		}
		new PopupBaseEdit Editor {
			get { return (PopupBaseEdit)base.Editor; }
		}
		new PopupBaseEditStyleSettings StyleSettings {
			get { return (PopupBaseEditStyleSettings)base.StyleSettings; }
		}
		public virtual bool StaysPopupOpen { get { return Editor.StaysPopupOpen ?? StyleSettings.StaysPopupOpen(); } }
		public ControlTemplate PopupTopAreaTemplate {
			get { return (ControlTemplate)GetValue(PopupTopAreaTemplateProperty); }
			set { SetValue(PopupTopAreaTemplateProperty, value); }
		}
		public ControlTemplate PopupBottomAreaTemplate {
			get { return (ControlTemplate)GetValue(PopupBottomAreaTemplateProperty); }
			set { SetValue(PopupBottomAreaTemplateProperty, value); }
		}
		public PopupViewModel PopupViewModel {
			get { return (PopupViewModel)GetValue(PopupViewModelProperty); }
			set { SetValue(PopupViewModelProperty, value); }
		}
		public ICommand AddNewCommand {
			get { return (ICommand)GetValue(AddNewCommandProperty); }
			set { SetValue(AddNewCommandProperty, value); }
		}
		public bool FocusPopupOnOpen {
			get { return Editor.FocusPopupOnOpen ?? GetFocusPopupOnOpenInternal(); }
		}
		protected virtual bool GetFocusPopupOnOpenInternal() {
			return StyleSettings.ShouldFocusPopup;
		}
		public bool IgnorePopupSizeConstraints { get { return Editor.IgnorePopupSizeConstraints ?? GetIgnorePopupSizeConstraints(); } }
		protected virtual bool GetIgnorePopupSizeConstraints() {
			return false;
		}
		public ResizeGripViewModel ResizeGripViewModel { get; private set; }
		public virtual void UpdatePopupTemplates() {
			PopupTopAreaTemplate = GetPopupTopAreaTemplate();
			PopupBottomAreaTemplate = GetPopupBottomAreaTemplate();
		}
		protected virtual PopupViewModel CreatePopupViewModel() {
			return new PopupViewModel(Editor);
		}
		protected virtual ResizeGripViewModel CreateResizeGripViewModel() {
			return new ResizeGripViewModel(Editor);
		}
		public override PopupFooterButtons GetPopupFooterButtons() {
			return Editor.PopupFooterButtons ?? StyleSettings.GetPopupFooterButtons(Editor);
		}
		protected override ControlTemplate GetPopupTopAreaTemplate() {
			return Editor.PopupTopAreaTemplate ?? StyleSettings.GetPopupTopAreaTemplate(Editor);
		}
		protected override ControlTemplate GetPopupBottomAreaTemplate() {
			return Editor.PopupBottomAreaTemplate ?? StyleSettings.GetPopupBottomAreaTemplate(Editor);
		}
		protected internal virtual bool GetShowSizeGrip() {
			return Editor.ShowSizeGrip ?? StyleSettings.GetShowSizeGrip(Editor);
		}
		protected internal virtual PopupCloseMode GetClosePopupOnClickMode() {
			return Editor.ClosePopupOnClickMode ?? StyleSettings.GetClosePopupOnClickMode(Editor);
		}
	}
}
