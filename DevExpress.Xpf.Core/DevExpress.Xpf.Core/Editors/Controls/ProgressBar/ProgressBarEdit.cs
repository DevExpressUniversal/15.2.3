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
using System.Windows.Markup;
using System.Windows.Automation.Peers;
using System.Windows.Media;
using DevExpress.Xpf.Editors.Automation;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Printing;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using SelectionChangedEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventArgs;
using SelectionChangedEventHandler = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventHandler;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors {
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	[ContentProperty("Content")]
	public class ProgressBarEdit : RangeBaseEdit, IProgressBarExportSettings {
		public static readonly DependencyProperty IsIndeterminateProperty;
		static readonly DependencyPropertyKey IsIndeterminatePropertyKey;
		public static readonly DependencyProperty IsPercentProperty;
		public static readonly DependencyProperty AdditionalForegroundProperty;
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty ContentDisplayModeProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
#if SL
		public static readonly DependencyProperty ContentDisplayModeNoneTemplateProperty;
		public static readonly DependencyProperty ContentDisplayModeContentTemplateProperty;
#endif
		static ProgressBarEdit() {
			Type ownerType = typeof(ProgressBarEdit);
			IsIndeterminatePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsIndeterminate", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			IsIndeterminateProperty = IsIndeterminatePropertyKey.DependencyProperty;
			IsPercentProperty = DependencyPropertyManager.Register("IsPercent", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(IsPercentPropertyChanged)));
			AdditionalForegroundProperty = DependencyPropertyManager.Register("AdditionalForeground", typeof(SolidColorBrush), ownerType, new FrameworkPropertyMetadata(null));
			ContentProperty = DependencyPropertyManager.Register("Content", typeof(object), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
			ContentDisplayModeProperty = DependencyPropertyManager.Register("ContentDisplayMode", typeof(ContentDisplayMode), ownerType, new FrameworkPropertyMetadata(ContentDisplayMode.None, FrameworkPropertyMetadataOptions.None, (d, e) => ((ProgressBarEdit)d).ContentDisplayModeChanged((ContentDisplayMode)e.NewValue)));
			ContentTemplateProperty = DependencyPropertyManager.Register("ContentTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
#if SL
			ContentDisplayModeNoneTemplateProperty = DependencyPropertyManager.Register("ContentDisplayModeNoneTemplate", typeof(DataTemplate), ownerType, 
				new FrameworkPropertyMetadata(null, (o, value) => ((ProgressBarEdit)o).UpdateContentTemplate()));
			ContentDisplayModeContentTemplateProperty = DependencyPropertyManager.Register("ContentDisplayModeContentTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null));
#endif
		}
		static void IsPercentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ProgressBarEdit)d).IsPercentChanged((bool)e.NewValue);
		}
		protected internal override BaseEditStyleSettings CreateStyleSettings() {
			return new ProgressBarStyleSettings();
		}
		public SolidColorBrush AdditionalForeground {
			get { return (SolidColorBrush)GetValue(AdditionalForegroundProperty); }
			set { SetValue(AdditionalForegroundProperty, value); }
		}
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public ContentDisplayMode ContentDisplayMode {
			get { return (ContentDisplayMode)GetValue(ContentDisplayModeProperty); }
			set { SetValue(ContentDisplayModeProperty, value); }
		}
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ProgressBarEditIsIndeterminate")]
#endif
		public bool IsIndeterminate {
			get { return (bool)GetValue(IsIndeterminateProperty); }
			internal set { this.SetValue(IsIndeterminatePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ProgressBarEditIsPercent")]
#endif
		public bool IsPercent {
			get { return (bool)GetValue(IsPercentProperty); }
			set { SetValue(IsPercentProperty, value); }
		}
#if SL
		public DataTemplate ContentDisplayModeNoneTemplate {
			get { return (DataTemplate)GetValue(ContentDisplayModeNoneTemplateProperty); }
			set { SetValue(ContentDisplayModeNoneTemplateProperty, value); }
		}
		public DataTemplate ContentDisplayModeContentTemplate {
			get { return (DataTemplate)GetValue(ContentDisplayModeContentTemplateProperty); }
			set { SetValue(ContentDisplayModeContentTemplateProperty, value); }
		}
#endif
		[Category(EditSettingsCategories.Behavior)]
		[Browsable(true)]
		new public BaseEditStyleSettings StyleSettings { get { return base.StyleSettings; } set { base.StyleSettings = value; } }
		protected new ProgressBarEditStrategy EditStrategy { get { return (ProgressBarEditStrategy)base.EditStrategy; } }
		protected internal override Type StyleSettingsType { get { return typeof(BaseProgressBarStyleSettings); } }
		public ProgressBarEdit() {
			Type ownerType = typeof(ProgressBarEdit);
			this.SetDefaultStyleKey(ownerType);
		}
		protected virtual void IsPercentChanged(bool value) {
			EditStrategy.IsPercentChanged(value);
		}
		protected override EditStrategyBase CreateEditStrategy() {
			return new ProgressBarEditStrategy(this);
		}
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new ProgressBarEditPropertyProvider(this);
		}
		protected virtual void ContentDisplayModeChanged(ContentDisplayMode value) {
			EditStrategy.ContentDisplayModeChanged(value);
		}
#if SL
		protected virtual void UpdateContentTemplate() {
			EditStrategy.UpdateContentTemplate(ContentDisplayMode);
		}
#endif
#if !SL
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new ProgressBarEditAutomationPeer(this);
		}
#endif
		#region IProgressBarExportSettings Members
		int IProgressBarExportSettings.Position {
			get { return ToInt(Value); }
		}
		Color IExportSettings.Background {
			get { return Colors.White; }
		}
		#endregion
	}
	public class ProgressBarEditPropertyProvider : ActualPropertyProvider {
		public ProgressBarEditPropertyProvider(ProgressBarEdit editor)
			: base(editor) {
		}
		public override bool CalcSuppressFeatures() {
			return false;
		}
	}
}
