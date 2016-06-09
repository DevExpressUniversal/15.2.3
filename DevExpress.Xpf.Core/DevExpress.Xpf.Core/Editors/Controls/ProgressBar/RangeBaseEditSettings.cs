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
using System.ComponentModel;
using System.Collections;
using System.Windows.Controls;
#if !SL
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Popups;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Popups;
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
namespace DevExpress.Xpf.Editors.Settings {
	public abstract class RangeBaseEditSettings : BaseEditSettings {
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty MinimumProperty;
		public static readonly DependencyProperty MaximumProperty;
		public static readonly DependencyProperty SmallStepProperty;
		public static readonly DependencyProperty LargeStepProperty;
		static RangeBaseEditSettings() {
			Type ownerType = typeof(RangeBaseEditSettings);
			SmallStepProperty = DependencyPropertyManager.Register("SmallStep", typeof(double), ownerType, new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.None, OnSettingsPropertyChanged));
			LargeStepProperty = DependencyPropertyManager.Register("LargeStep", typeof(double), ownerType, new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.None, OnSettingsPropertyChanged));
			OrientationProperty = DependencyPropertyManager.Register("Orientation", typeof(Orientation), ownerType,
				new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.None, OnSettingsPropertyChanged));
			MinimumProperty = DependencyPropertyManager.Register("Minimum", typeof(double), ownerType, new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.None,
				OnSettingsPropertyChanged));
			MaximumProperty = DependencyPropertyManager.Register("Maximum", typeof(double), ownerType, new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.None,
				OnSettingsPropertyChanged));
		}
		[Category(EditSettingsCategories.Behavior)]
		public double SmallStep {
			get { return (double)GetValue(SmallStepProperty); }
			set { SetValue(SmallStepProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public double LargeStep {
			get { return (double)GetValue(LargeStepProperty); }
			set { SetValue(LargeStepProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public double Minimum {
			get { return (double)GetValue(MinimumProperty); }
			set { SetValue(MinimumProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public double Maximum {
			get { return (double)GetValue(MaximumProperty); }
			set { SetValue(MaximumProperty, value); }
		}
		public RangeBaseEditSettings() {
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			RangeBaseEdit editor = edit as RangeBaseEdit;
			if(editor == null)
				return;
			SetValueFromSettings(SmallStepProperty, () => editor.SmallStep = SmallStep);
			SetValueFromSettings(LargeStepProperty, () => editor.LargeStep = LargeStep);
			SetValueFromSettings(OrientationProperty, () => editor.Orientation = Orientation);
			SetValueFromSettings(MinimumProperty, () => editor.Minimum = Minimum);
			SetValueFromSettings(MaximumProperty, () => editor.Maximum = Maximum);
		}
	}
}
