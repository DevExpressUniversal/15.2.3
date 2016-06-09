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

#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Markup;
using DevExpress.Data.Mask;
using DevExpress.Xpf.Editors.Automation;
using System.Windows.Automation.Peers;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Data;
using DevExpress.Data.Async.Helpers;
#if !SL
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Printing;
using System.Windows.Interop;
using System.Collections;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
#endregion
namespace DevExpress.Xpf.Grid.LookUp {
	public class LookUpEditSettings : LookUpEditSettingsBase {
		public static readonly DependencyProperty IsPopupAutoWidthProperty;
		public static readonly DependencyProperty AutoPopulateColumnsProperty;
		static LookUpEditSettings() {
			Type ownerType = typeof(LookUpEditSettings);
			IsPopupAutoWidthProperty = DependencyPropertyManager.Register("IsPopupAutoWidth", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			AutoPopulateColumnsProperty = DependencyPropertyManager.Register("AutoPopulateColumns", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
#if !SL
			EditorSettingsProvider.Default.RegisterUserEditor2(typeof(LookUpEdit), typeof(LookUpEditSettings),
				optimized => optimized ? new InplaceBaseEdit() : (IBaseEdit)new LookUpEdit(), () => new LookUpEditSettings());
			PopupMinHeightProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(LookUpEdit.DefaultPopupMinHeight));
			PopupMinWidthProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(LookUpEdit.DefaultPopupMinWidth));
#else
			EditorSettingsProvider.Default.RegisterUserEditor(typeof(LookUpEdit), typeof(LookUpEditSettings), () => new LookUpEdit(), () => new LookUpEditSettings());
#endif
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("LookUpEditSettingsIsPopupAutoWidth")]
#endif
public bool IsPopupAutoWidth {
			get { return (bool)GetValue(IsPopupAutoWidthProperty); }
			set { SetValue(IsPopupAutoWidthProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("LookUpEditSettingsAutoPopulateColumns")]
#endif
public bool AutoPopulateColumns {
			get { return (bool)GetValue(AutoPopulateColumnsProperty); }
			set { SetValue(AutoPopulateColumnsProperty, value); }
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			LookUpEdit lookUp = edit as LookUpEdit;
			if(lookUp == null) return;
			SetValueFromSettings(IsPopupAutoWidthProperty, () => lookUp.IsPopupAutoWidth = IsPopupAutoWidth);
			SetValueFromSettings(AutoPopulateColumnsProperty, () => lookUp.AutoPopulateColumns = AutoPopulateColumns);
		}
	}
}
