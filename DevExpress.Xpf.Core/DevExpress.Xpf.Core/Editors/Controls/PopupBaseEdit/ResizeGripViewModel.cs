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
	public class ResizeGripViewModel : EditorViewModelBase {
		public static readonly DependencyProperty IsDownProperty;
		static ResizeGripViewModel() {
			Type ownerType = typeof(ResizeGripViewModel);
			IsDownProperty = DependencyPropertyManager.Register("IsDown", typeof(bool), ownerType, new PropertyMetadata(true));
		}
		public ResizeGripViewModel(BaseEdit editor) : base(editor) {
		}
		public bool IsDown {
			get { return (bool)GetValue(IsDownProperty); }
			set { SetValue(IsDownProperty, value); }
		}
		bool DropOpposite { get { return OwnerEdit.PopupSettings.PopupResizingStrategy.DropOpposite; } }
		bool IsRTL { get { return OwnerEdit.PopupSettings.PopupResizingStrategy.IsRTL; } }
		new PopupBaseEditPropertyProvider PropertyProvider { get { return (PopupBaseEditPropertyProvider)base.PropertyProvider; } }
		new PopupBaseEdit OwnerEdit { get { return (PopupBaseEdit)base.OwnerEdit; } }
		public void Update() {
			Visibility = PropertyProvider.GetShowSizeGrip() ? Visibility.Visible : Visibility.Collapsed;
			IsDown = !DropOpposite;
			Cursor = GetCursor();
			VerticalAlignment = GetVerticalAlignment();
		}
		VerticalAlignment GetVerticalAlignment() {
			if (DropOpposite)
				return VerticalAlignment.Top;
			return VerticalAlignment.Bottom;
		}
		Cursor GetCursor() {
			if (DropOpposite ^ IsRTL)
				return Cursors.SizeNESW;
			return Cursors.SizeNWSE;
		}
	}
}
