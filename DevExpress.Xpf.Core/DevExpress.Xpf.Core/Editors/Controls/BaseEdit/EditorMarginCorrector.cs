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
namespace DevExpress.Xpf.Editors.Helpers {
	public class EditorMarginCorrector : DependencyObject {
		public static readonly DependencyProperty ActualMarginProperty;
		public static readonly DependencyProperty CorrectorProperty;
		public static readonly DependencyProperty ErrorMarginProperty;
		public static readonly DependencyProperty MarginProperty;
		static readonly DependencyPropertyKey ActualMarginPropertyKey;
		static EditorMarginCorrector() {
			ActualMarginPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualMargin", typeof(Thickness), typeof(EditorMarginCorrector),
			  new PropertyMetadata((d, e) => ((EditorMarginCorrector)d).PropertyChangedActualMargin()));
			ActualMarginProperty = ActualMarginPropertyKey.DependencyProperty;
			CorrectorProperty = DependencyPropertyManager.RegisterAttached("Corrector", typeof(EditorMarginCorrector), typeof(EditorMarginCorrector), new PropertyMetadata(PropertyChangedCorrector));
			ErrorMarginProperty = DependencyPropertyManager.Register("ErrorMargin", typeof(Thickness), typeof(EditorMarginCorrector),
			  new PropertyMetadata((d, e) => ((EditorMarginCorrector)d).UpdateActualMargin()));
			MarginProperty = DependencyPropertyManager.Register("Margin", typeof(Thickness), typeof(EditorMarginCorrector),
			  new PropertyMetadata((d, e) => ((EditorMarginCorrector)d).UpdateActualMargin()));
		}
		public static EditorMarginCorrector GetCorrector(DependencyObject obj) {
			return (EditorMarginCorrector)obj.GetValue(CorrectorProperty);
		}
		public static void SetCorrector(DependencyObject obj, EditorMarginCorrector value) {
			obj.SetValue(CorrectorProperty, value);
		}
		static void PropertyChangedCorrector(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			EditorMarginCorrector corrector = (EditorMarginCorrector)e.NewValue;
			if(d is FrameworkElement) {
				corrector.Target = d as FrameworkElement;
				corrector.UpdateTargetMargin();
				(d as FrameworkElement).Loaded += TargetLoaded;
			}
		}
		static void TargetLoaded(object sender, System.Windows.RoutedEventArgs e) {
			FrameworkElement target = (FrameworkElement)sender;
			target.Loaded -= TargetLoaded;
			BaseEdit editor;
			if(!(target.DataContext is BaseEdit))
				editor = LayoutHelper.FindParentObject<BaseEdit>(target);
			else
				editor = (BaseEdit)target.DataContext;
			EditorMarginCorrector corrector = GetCorrector(target);
			if(editor != null) {
				corrector.editMode = editor.EditMode;
				corrector.hasValidationError = editor.HasValidationError;
				corrector.showBorder = editor.ShowBorder;
				corrector.showError = editor.ShowError;
				corrector.Editor = editor;
				editor.MarginCorrector = corrector;
			} else
				corrector.UpdateTargetMargin();
		}
		public EditorMarginCorrector() {
			UpdateActualMargin();
		}
		EditMode editMode;
		BaseEdit editor;
		bool hasValidationError;
		bool showBorder;
		bool showError;
		WeakReference target;
		public Thickness ActualMargin {
			get { return (Thickness)GetValue(ActualMarginProperty); }
			private set { this.SetValue(ActualMarginPropertyKey, value); }
		}
		public Thickness ErrorMargin {
			get { return (Thickness)GetValue(ErrorMarginProperty); }
			set { SetValue(ErrorMarginProperty, value); }
		}
		public Thickness Margin {
			get { return (Thickness)GetValue(MarginProperty); }
			set { SetValue(MarginProperty, value); }
		}
		internal EditMode EditMode {
			get { return editMode; }
			set {
				if(value == editMode) return;
				editMode = value;
				UpdateActualMargin();
			}
		}
		BaseEdit Editor {
			get { return editor; }
			set {
				if(value == editor) return;
				editor = value;
				UpdateActualMargin();
			}
		}
		internal bool HasValidationError {
			get { return hasValidationError; }
			set {
				if(value == hasValidationError) return;
				hasValidationError = value;
				UpdateActualMargin();
			}
		}
		internal bool ShowBorder {
			get { return showBorder; }
			set {
				if(value == showBorder) return;
				showBorder = value;
				UpdateActualMargin();
			}
		}
		internal bool ShowError {
			get { return showError; }
			set {
				if(value == showError) return;
				showError = value;
				UpdateActualMargin();
			}
		}
		FrameworkElement Target {
			get { return (target != null && target.IsAlive) ? (FrameworkElement)target.Target : null; }
			set { target = (value != null) ? new WeakReference(value) : null; }
		}
		void PropertyChangedActualMargin() {
			if(Target != null)
				UpdateTargetMargin();
		}
		void UpdateActualMargin() {
			if(Editor == null)
				ActualMargin = Margin;
			else
				if(EditMode == EditMode.Standalone && !ShowBorder)
					ActualMargin = new Thickness();
				else
					if(HasValidationError && (ShowError || EditMode != Editors.EditMode.Standalone))
						ActualMargin = ErrorMargin;
					else
						ActualMargin = Margin;
		}
		void UpdateTargetMargin() {
			if(Target.Margin != ActualMargin)
				Target.Margin = ActualMargin;
		}
	}
	public class EditorMarginHelper : DependencyObject {
		public static readonly DependencyProperty MarginProperty;
		static EditorMarginHelper() {
			MarginProperty = DependencyPropertyManager.RegisterAttached("Margin", typeof(string), typeof(EditorMarginHelper), new PropertyMetadata(PropertyChangedMargin));
		}
		public static string GetMargin(DependencyObject obj) {
			return (string)obj.GetValue(MarginProperty);
		}
		public static void SetMargin(DependencyObject obj, string value) {
			obj.SetValue(MarginProperty, value);
		}
		static void PropertyChangedMargin(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(!(e.NewValue is string)) return;
			string[] list = ((string)e.NewValue).Split(new char[] { ',' });
			EditorMarginCorrector corrector = new EditorMarginCorrector() {
				ErrorMargin = new Thickness(Double.Parse(list[4]), Double.Parse(list[5]), Double.Parse(list[6]), Double.Parse(list[7])),
				Margin = new Thickness(Double.Parse(list[0]), Double.Parse(list[1]), Double.Parse(list[2]), Double.Parse(list[3])),
			};
			EditorMarginCorrector.SetCorrector(d, corrector);
		}
	}
	public class ScrollContentPresenterMarginCorrector : IValueConverter {
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
#if !SL
			return value;
#else
			Thickness t = (Thickness)value;
			ThicknessHelper.Inc(ref t, new Thickness(2, 0, 2, 0));
			return t;
#endif
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
}
