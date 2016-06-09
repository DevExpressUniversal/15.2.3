﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
#if !SL
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Themes;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using FrameworkContentElement = System.Windows.FrameworkElement;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RepeatButton = DevExpress.Xpf.Editors.WPFCompatibility.SLRepeatButton;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors {
	public partial class TrackBarStyleSettings : BaseEditStyleSettings {
		public virtual bool IsMoveToPointEnabled { get { return false; } }
		protected virtual bool IsZoom { get { return false; } }
		protected virtual bool IsRange { get { return false; } }
		partial void ApplyPanelStyle(TrackBarEdit editor);
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			TrackBarEdit trackBarEditor = editor as TrackBarEdit;
			if(trackBarEditor == null)
				return;
			trackBarEditor.IsRange = IsRange;
			trackBarEditor.IsZoom = IsZoom;
			if(trackBarEditor.Panel == null)
				return;
			ApplyPanelStyle(trackBarEditor);
		}
	}
	public class TrackBarZoomStyleSettings : TrackBarStyleSettings {
		protected override bool IsZoom { get { return true; } }
	}
	public partial class TrackBarRangeStyleSettings : TrackBarStyleSettings {
		protected override bool IsRange { get { return true; } }
	}
	public class TrackBarZoomRangeStyleSettings : TrackBarRangeStyleSettings {
		protected override bool IsZoom { get { return true; } }
	}
}
