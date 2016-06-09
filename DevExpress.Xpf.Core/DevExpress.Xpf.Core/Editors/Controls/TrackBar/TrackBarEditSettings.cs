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
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Editors.Settings.Extension;
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
using FrameworkContentElement = System.Windows.FrameworkElement;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RepeatButton = DevExpress.Xpf.Editors.WPFCompatibility.SLRepeatButton;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors.Settings {
#if !SL
	public class TrackBarEditSettingsExtension : BaseSettingsExtension {
		public TrackBarStyleSettings StyleSettings { get; set; }
		public double SmallStep { get; set; }
		public double LargeStep { get; set; }
		public Orientation Orientation { get; set; }
		public double Minimum { get; set; }
		public double Maximum { get; set; }
		public TickPlacement TickPlacement { get; set; }
		public double TickFrequency { get; set; }
		public TrackBarEditSettingsExtension() {
			StyleSettings = (TrackBarStyleSettings)TrackBarEditSettings.StyleSettingsProperty.DefaultMetadata.DefaultValue;
			SmallStep = (double)TrackBarEditSettings.SmallStepProperty.DefaultMetadata.DefaultValue;
			LargeStep = (double)TrackBarEditSettings.LargeStepProperty.DefaultMetadata.DefaultValue;
			Orientation = (Orientation)TrackBarEditSettings.OrientationProperty.DefaultMetadata.DefaultValue;
			Minimum = (double)TrackBarEditSettings.MinimumProperty.DefaultMetadata.DefaultValue;
			Maximum = (double)TrackBarEditSettings.MaximumProperty.DefaultMetadata.DefaultValue;
			TickFrequency = (double)TrackBarEditSettings.TickFrequencyProperty.DefaultMetadata.DefaultValue;
			TickPlacement = (TickPlacement)TrackBarEditSettings.TickPlacementProperty.DefaultMetadata.DefaultValue;
		}
		protected override BaseEditSettings CreateEditSettings() {
			return new TrackBarEditSettings() {
				StyleSettings = this.StyleSettings,
				SmallStep = this.SmallStep,
				LargeStep = this.LargeStep,
				Orientation = this.Orientation,
				Minimum = this.Minimum,
				Maximum = this.Maximum,
				TickPlacement = this.TickPlacement,
				TickFrequency = this.TickFrequency,
			};
		}
	}
#endif
	public class TrackBarEditSettings : RangeBaseEditSettings {
		public static readonly DependencyProperty TickPlacementProperty;
		public static readonly DependencyProperty TickFrequencyProperty;
		static TrackBarEditSettings() {
			Type ownerType = typeof(TrackBarEditSettings);
#if !SL
			TickPlacementProperty = DependencyProperty.Register("TickPlacement", typeof(TickPlacement), ownerType, new FrameworkPropertyMetadata(TickPlacement.BottomRight));
			TickFrequencyProperty = DependencyProperty.Register("TickFrequency", typeof(double), ownerType, new FrameworkPropertyMetadata(5d));
#endif
		}
#if !SL
		[Category(EditSettingsCategories.Behavior)]
		public TickPlacement TickPlacement {
			get { return (TickPlacement)GetValue(TickPlacementProperty); }
			set { SetValue(TickPlacementProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public double TickFrequency {
			get { return (double)GetValue(TickFrequencyProperty); }
			set { SetValue(TickFrequencyProperty, value); }
		}
#endif
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			TrackBarEdit editor = edit as TrackBarEdit;
			if(editor == null)
				return;
#if !SL
			SetValueFromSettings(TickFrequencyProperty, () => editor.TickFrequency = this.TickFrequency);
			SetValueFromSettings(TickPlacementProperty, () => editor.TickPlacement = this.TickPlacement);
#endif
		}
	}
}
