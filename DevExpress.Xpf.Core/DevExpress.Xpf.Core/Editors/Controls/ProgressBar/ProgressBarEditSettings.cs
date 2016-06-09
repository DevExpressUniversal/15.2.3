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
using System.Windows.Markup;
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
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors.Settings {
#if !SL
	public class ProgressBarEditSettingsExtension : BaseSettingsExtension {
		public bool IsPercent { get; set; }
		public BaseProgressBarStyleSettings StyleSettings { get; set; }
		public double SmallStep { get; set; }
		public double LargeStep { get; set; }
		public Orientation Orientation { get; set; }
		public double Minimum { get; set; }
		public double Maximum { get; set; }
		public ContentDisplayMode ContentDisplayMode { get; set; }
		public DataTemplate ContentTemplate { get; set; }
		public ProgressBarEditSettingsExtension() {
			IsPercent = (bool)ProgressBarEditSettings.IsPercentProperty.DefaultMetadata.DefaultValue;
			StyleSettings = (BaseProgressBarStyleSettings)ProgressBarEditSettings.StyleSettingsProperty.DefaultMetadata.DefaultValue;
			SmallStep = (double)ProgressBarEditSettings.SmallStepProperty.DefaultMetadata.DefaultValue;
			LargeStep = (double)ProgressBarEditSettings.LargeStepProperty.DefaultMetadata.DefaultValue;
			Orientation = (Orientation)ProgressBarEditSettings.OrientationProperty.DefaultMetadata.DefaultValue;
			Minimum = (double)ProgressBarEditSettings.MinimumProperty.DefaultMetadata.DefaultValue;
			Maximum = (double)ProgressBarEditSettings.MaximumProperty.DefaultMetadata.DefaultValue;
			ContentDisplayMode = (ContentDisplayMode)ProgressBarEditSettings.ContentDisplayModeProperty.DefaultMetadata.DefaultValue;
			ContentTemplate = (DataTemplate)ProgressBarEditSettings.ContentTemplateProperty.DefaultMetadata.DefaultValue;
		}
		protected override BaseEditSettings CreateEditSettings() {
			return new ProgressBarEditSettings() {
				IsPercent = this.IsPercent,
				StyleSettings = this.StyleSettings,
				SmallStep = this.SmallStep,
				LargeStep = this.LargeStep,
				Orientation = this.Orientation,
				Minimum = this.Minimum,
				Maximum = this.Maximum,
				ContentDisplayMode = this.ContentDisplayMode,
				ContentTemplate = this.ContentTemplate
			};
		}
	}
#endif
	public class ProgressBarEditSettings : RangeBaseEditSettings {
		public static readonly DependencyProperty IsPercentProperty;
		public static readonly DependencyProperty ContentDisplayModeProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
		static ProgressBarEditSettings() {
			Type ownerType = typeof(ProgressBarEditSettings);
			IsPercentProperty = DependencyPropertyManager.Register("IsPercent", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, OnSettingsPropertyChanged));
			ContentDisplayModeProperty = DependencyPropertyManager.Register("ContentDisplayMode", typeof(ContentDisplayMode), ownerType,
				new FrameworkPropertyMetadata(ContentDisplayMode.None, FrameworkPropertyMetadataOptions.None, OnSettingsPropertyChanged));
			ContentTemplateProperty = DependencyPropertyManager.Register("ContentTemplate", typeof(DataTemplate), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnSettingsPropertyChanged));
		}
		[Category(EditSettingsCategories.Behavior)]
		public bool IsPercent {
			get { return (bool)GetValue(IsPercentProperty); }
			set { SetValue(IsPercentProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public ContentDisplayMode ContentDisplayMode {
			get { return (ContentDisplayMode)GetValue(ContentDisplayModeProperty); }
			set { SetValue(ContentDisplayModeProperty, value); }
		}
		[Category(EditSettingsCategories.Appearance)]
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		public ProgressBarEditSettings() {
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			ProgressBarEdit editor = edit as ProgressBarEdit;
			if(editor == null)
				return;
			SetValueFromSettings(IsPercentProperty, () => editor.IsPercent = IsPercent);
			SetValueFromSettings(ContentDisplayModeProperty, () => editor.ContentDisplayMode = ContentDisplayMode);
			SetValueFromSettings(ContentTemplateProperty, () => editor.ContentTemplate = ContentTemplate);
		}
	}
}
