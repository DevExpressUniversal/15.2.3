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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
#if !SL
using DevExpress.Xpf.Core;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors {
	public class EditorControl : Control {
		static EditorControl() {
			Type ownerType = typeof(EditorControl);
#if !SL
			DataContextProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(
				null, FrameworkPropertyMetadataOptions.Inherits, DataContextChanged, CoerceDataContext));
#endif
		}
#if !SL
		new static void DataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((EditorControl)d).DataContextChanged((DependencyObject)e.NewValue);
		}
		static object CoerceDataContext(DependencyObject d, object value) {
			if(value == null)
				return value;
			return value is DependencyObject ? value : new DependencyObject();
		}
#endif
		public EditorControl() {
			Focusable = false;
		}
		internal BaseEdit Owner { get; set; }
		StandaloneContentManagementStrategy ContentManagementStrategy { get { return Owner != null ? (StandaloneContentManagementStrategy)Owner.ContentManagementStrategy : null; } }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(ContentManagementStrategy != null) 
				ContentManagementStrategy.OnApplyContentTemplate(this);
		}
		internal FrameworkElement GetEditCore() {
			return GetTemplateChild("PART_Editor") as FrameworkElement;
		}
#if !SL
		new void DataContextChanged(DependencyObject dataContext) {
			if(Owner != null)
				Owner.UpdateDataContext(dataContext);
		}
		protected override Size MeasureOverride(Size constraint) {
			return MeasurePixelSnapperHelper.MeasureOverride(base.MeasureOverride(constraint), SnapperType.Ceil);
		}
#endif
	}
}
