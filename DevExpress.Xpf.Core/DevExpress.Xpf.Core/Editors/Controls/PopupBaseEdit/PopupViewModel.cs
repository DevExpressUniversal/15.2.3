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
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading;
using System.Windows.Automation.Peers;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Internal;
#if !SL
using DevExpress.Xpf.Utils;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using System.Windows.Interop;
using DevExpress.Xpf.Editors.Automation;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DevExpress.Xpf.Editors.Automation;
#endif
#if !SL
using PopupFooterButtonsType = DevExpress.Xpf.Editors.PopupFooterButtons;
using System.Collections;
using DevExpress.Xpf.Editors.Native;
#else
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PopupFooterButtonsType = DevExpress.Xpf.Editors.PopupFooterButtons;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using DevExpress.Xpf.Editors.Native;
#endif
namespace DevExpress.Xpf.Editors {
	public class PopupViewModel : EditorViewModelBase {
		public static readonly DependencyProperty NullValueButtonVisibilityProperty;
		public static readonly DependencyProperty AddNewButtonVisibilityProperty;
		public static readonly DependencyProperty OkButtonVisibilityProperty;
		public static readonly DependencyProperty CancelButtonVisibilityProperty;
		public static readonly DependencyProperty CloseButtonVisibilityProperty;
		public static readonly DependencyProperty FooterVisibilityProperty;
		public static readonly DependencyProperty DropOppositeProperty;
		public static readonly DependencyProperty OkButtonIsEnabledProperty;
		static PopupViewModel() {
			Type ownerType = typeof(PopupViewModel);
			NullValueButtonVisibilityProperty = DependencyPropertyManager.Register("NullValueButtonVisibility", typeof(Visibility), ownerType, new PropertyMetadata(Visibility.Collapsed));
			AddNewButtonVisibilityProperty = DependencyPropertyManager.Register("AddNewButtonVisibility", typeof(Visibility), ownerType, new PropertyMetadata(Visibility.Collapsed));
			OkButtonVisibilityProperty = DependencyPropertyManager.Register("OkButtonVisibility", typeof(Visibility), ownerType, new PropertyMetadata(Visibility.Collapsed));
			CancelButtonVisibilityProperty = DependencyPropertyManager.Register("CancelButtonVisibility", typeof(Visibility), ownerType, new PropertyMetadata(Visibility.Collapsed));
			CloseButtonVisibilityProperty = DependencyPropertyManager.Register("CloseButtonVisibility", typeof(Visibility), ownerType, new PropertyMetadata(Visibility.Collapsed));
			FooterVisibilityProperty = DependencyPropertyManager.Register("FooterVisibility", typeof(Visibility), ownerType, new PropertyMetadata(Visibility.Collapsed));
			DropOppositeProperty = DependencyPropertyManager.Register("DropOpposite", typeof(bool), ownerType, new PropertyMetadata(false));
			OkButtonIsEnabledProperty = DependencyPropertyManager.Register("OkButtonIsEnabled", typeof(bool), ownerType, new PropertyMetadata(true));
		}
		new PopupBaseEdit OwnerEdit { get { return (PopupBaseEdit)base.OwnerEdit; } }
		new PopupBaseEditPropertyProvider PropertyProvider { get { return (PopupBaseEditPropertyProvider)base.PropertyProvider; } }
		public Visibility NullValueButtonVisibility {
			get { return (Visibility)GetValue(NullValueButtonVisibilityProperty); }
			set { SetValue(NullValueButtonVisibilityProperty, value); }
		}
		public Visibility AddNewButtonVisibility {
			get { return (Visibility)GetValue(AddNewButtonVisibilityProperty); }
			set { SetValue(AddNewButtonVisibilityProperty, value); }
		}
		public Visibility OkButtonVisibility {
			get { return (Visibility)GetValue(OkButtonVisibilityProperty); }
			set { SetValue(OkButtonVisibilityProperty, value); }
		}
		public Visibility CancelButtonVisibility {
			get { return (Visibility)GetValue(CancelButtonVisibilityProperty); }
			set { SetValue(CancelButtonVisibilityProperty, value); }
		}
		public Visibility CloseButtonVisibility {
			get { return (Visibility)GetValue(CloseButtonVisibilityProperty); }
			set { SetValue(CloseButtonVisibilityProperty, value); }
		}
		public Visibility FooterVisibility {
			get { return (Visibility)GetValue(FooterVisibilityProperty); }
			set { SetValue(FooterVisibilityProperty, value); }
		}
		public bool DropOpposite {
			get { return (bool)GetValue(DropOppositeProperty); }
			set { SetValue(DropOppositeProperty, value); }
		}
		public bool OkButtonIsEnabled {
			get { return (bool)GetValue(OkButtonIsEnabledProperty); }
			set { SetValue(OkButtonIsEnabledProperty, value); }
		}
		public PopupViewModel(BaseEdit editor) : base(editor) {
		}
		public virtual void UpdateDropOpposite() {
			DropOpposite = OwnerEdit.PopupSettings.PopupResizingStrategy.DropOpposite;
		}
		public virtual void Update() {
			bool okButtonVisible = PropertyProvider.GetPopupFooterButtons() == PopupFooterButtonsType.OkCancel;
			bool okCancelButtonVisible = PropertyProvider.GetPopupFooterButtons() == PopupFooterButtonsType.OkCancel;
			bool okCloseButtonVisible = PropertyProvider.GetPopupFooterButtons() == PopupFooterButtonsType.Close;
			bool nullValueButtonVisible = EditorPlacementToBoolean(PropertyProvider.GetNullValueButtonPlacement());
			bool addNewButtonVisible = EditorPlacementToBoolean(PropertyProvider.GetAddNewButtonPlacement());
			FooterVisibility = BooleanToVisibility(
				PropertyProvider.GetShowSizeGrip() ||
				okButtonVisible ||
				okCancelButtonVisible ||
				okCloseButtonVisible ||
				nullValueButtonVisible ||
				addNewButtonVisible);
			OkButtonVisibility = BooleanToVisibility(okButtonVisible);
			CancelButtonVisibility = BooleanToVisibility(okCancelButtonVisible);
			CloseButtonVisibility = BooleanToVisibility(okCloseButtonVisible);
			NullValueButtonVisibility = BooleanToVisibility(nullValueButtonVisible);
			AddNewButtonVisibility = BooleanToVisibility(addNewButtonVisible);
		}
		Visibility BooleanToVisibility(bool visible) {
			return visible ? Visibility.Visible : Visibility.Collapsed;
		}
		Visibility EditorPlacementToVisibility(EditorPlacement placement) {
			return placement == EditorPlacement.Popup ? Visibility.Visible : Visibility.Collapsed;
		}
		bool EditorPlacementToBoolean(EditorPlacement placement) {
			return placement == EditorPlacement.Popup ? true : false;
		}
	}
}
