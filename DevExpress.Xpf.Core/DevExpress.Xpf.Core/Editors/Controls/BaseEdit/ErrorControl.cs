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
using System.Windows.Markup;
using System.IO;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Documents;
using DevExpress.Utils;
using System.Collections;
using System.ComponentModel;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Printing;
using System.Windows.Media;
using DevExpress.Xpf.Printing.Native;
using System.Windows.Media.Animation;
#if !SL
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using IInputElement = System.Windows.UIElement;
using Keyboard = DevExpress.Xpf.Editors.WPFCompatibility.SLKeyboard;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors {
	public partial class ErrorControl : ContentControl {
		FrameworkElement contentPresenter;
		Storyboard showErrorStoryboard;
		Storyboard hideErrorStoryboard;
		static ErrorControl() {
			Type ownerType = typeof(ErrorControl);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			DataObjectBase.RaiseResetEventWhenObjectIsLoadedProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true));
		}
		public ErrorControl() {
			DataObjectBase.AddResetHandler(this, Reset);
			Loaded += new RoutedEventHandler(ErrorControl_Loaded);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			contentPresenter = GetTemplateChild("content") as FrameworkElement;
		}
		void ErrorControl_Loaded(object sender, RoutedEventArgs e) {
			UpdateError();
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if(e.Property == HasContentProperty)
				UpdateError();
		}
		void UpdateError() {
			if(HasContent)
				ShowError();
			else
				HideError();
		}
		void ShowError() {
			if(contentPresenter == null)
				return;
			showErrorStoryboard = ResourceHelper.FindResource(contentPresenter, "showErrorStoryboard") as Storyboard;
			if(showErrorStoryboard != null)
				showErrorStoryboard.Begin();
		}
		void HideError() {
			if(contentPresenter == null)
				return;
			hideErrorStoryboard = ResourceHelper.FindResource(contentPresenter, "hideErrorStoryboard") as Storyboard;
			if(hideErrorStoryboard != null)
				hideErrorStoryboard.Begin();
		}
		void Reset(object sender, RoutedEventArgs args) {
			if(showErrorStoryboard != null)
				showErrorStoryboard.SkipToFill();
			if(hideErrorStoryboard != null)
				hideErrorStoryboard.SkipToFill();
		}
	}
}
