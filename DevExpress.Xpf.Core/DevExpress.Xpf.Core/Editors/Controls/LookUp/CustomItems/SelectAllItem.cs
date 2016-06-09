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
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Utils.Native;
using DevExpress.Xpf.Core.Native;
using System.Globalization;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Internal;
#if !SL
using DevExpress.Xpf.Editors.Helpers;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
#else
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Core.WPFCompatibility;
using System.Windows.Media;
using System.Runtime.Serialization;
using System.IO;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using ListBox = DevExpress.Xpf.Editors.WPFCompatibility.SLListBox;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors {
	public sealed class SelectAllItem : CustomItem {
		protected internal override bool ShouldFilter { get { return true; } }
		SelectionViewModel SelectionViewModel {
			get {
				ISelectorEdit editor = OwnerEdit;
				if (editor == null)
					return null;
				return ((ISelectorEditPropertyProvider)ActualPropertyProvider.GetProperties((DependencyObject)editor)).SelectionViewModel;
			}
		}
		public bool? SelectAll { get { return SelectionViewModel.SelectAll; } }
		protected override Style GetItemStyleInternal() {
			FrameworkElement editor = (FrameworkElement)OwnerEdit;
			if (editor == null)
				return null;
			return (Style)editor.FindResource(new CustomItemThemeKeyExtension {
				ResourceKey = CustomItemThemeKeys.SelectAllItemContainerStyle,
			});
		}
		protected override string GetDisplayTextInternal() {
			return EditorLocalizer.GetString(EditorStringId.SelectAll);
		}
		protected override object GetEditValueInternal() {
			return SelectAll;
		}
	}
}
