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

using DevExpress.Xpf.Core.Native;
using System.Collections.Generic;
using DevExpress.XtraScheduler.UI;
using DevExpress.Xpf.Scheduler.UI;
using System.Collections.ObjectModel;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Xpf.Core;
using System.Windows;
using System;
using System.Windows.Media;
using System.Windows.Controls;
using DevExpress.Xpf.Scheduler.Native;
#if WPF
using PlatformIndependentPropertyChangedCallback = System.Windows.PropertyChangedCallback;
using PlatformIndependentDependencyPropertyChangedEventArgs = System.Windows.DependencyPropertyChangedEventArgs;
using DevExpress.Xpf.Scheduler.Internal;
using System.Windows.Input;
#else
using PlatformIndependentDependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PlatformIndependentPropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Xpf.Scheduler.Internal;
#endif
namespace DevExpress.Xpf.Scheduler {
	public class SchedulerFormBehavior : DependencyObject {
		#region Title
		public static readonly DependencyProperty TitleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterAttachedPropertyCore<SchedulerFormBehavior, string>("Title", String.Empty, FrameworkPropertyMetadataOptions.None, new PlatformIndependentPropertyChangedCallback(OnTitlePropertyChanged));
		static void OnTitlePropertyChanged(DependencyObject d, PlatformIndependentDependencyPropertyChangedEventArgs e) {
			string newValue = e.NewValue as string;
			if (!PlatformIndependentFormHelper.SetFormTitleCore(d, newValue)) {
				FrameworkElement element = (FrameworkElement)d;
				element.Loaded += OnElementLoaded;
			}
		}
		static void OnElementLoaded(object sender, RoutedEventArgs e) {
			FrameworkElement element = (FrameworkElement)sender;
			PlatformIndependentFormHelper.SetFormTitleCore(element, GetTitle(element));
			element.Loaded -= OnElementLoaded;
		}
		public static void SetTitle(DependencyObject o, string value) {
			o.SetValue(TitleProperty, value);
		}
		public static string GetTitle(DependencyObject o) {
			return (string)o.GetValue(TitleProperty);
		}
		#endregion
		#region SchedulerControl
		public static readonly DependencyProperty SchedulerControlProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterAttachedPropertyCore<SchedulerFormBehavior, SchedulerControl>("SchedulerControl", null, FrameworkPropertyMetadataOptions.None, null);
		public static SchedulerControl GetSchedulerControl(DependencyObject o) {
			return (SchedulerControl)o.GetValue(SchedulerControlProperty);
		}
		public static void SetSchedulerControl(DependencyObject o, SchedulerControl value) {
			o.SetValue(SchedulerControlProperty, value);
		}
		#endregion
		internal static bool IsFormElement(DependencyObject element) {
			DependencyObject parent = element;
			while (parent != null) {
				if (FloatingContainer.GetDialogOwner(parent) != null)
					return true;
				parent = VisualTreeHelper.GetParent(parent);
			}
			return false;
		}
		public static void ShowDialog(FrameworkElement content, FrameworkElement owner, FloatingContainerParameters param) {
#if DEBUGTEST
			if (ForceAllFormIsNonModal) {
#if SL
				PlatformIndependentFormHelper.Show(content, owner, param);
				return;
#else
				param.ShowModal = false;
#endif
			}
#endif
			PlatformIndependentFormHelper.ShowDialog(content, owner, param);
		}
		public static void ShowDialogContent(FrameworkElement content, FrameworkElement owner, FloatingContainerParameters param) {
#if DEBUGTEST
			if (ForceAllFormIsNonModal) {
#if SL
				PlatformIndependentFormHelper.Show(content, owner, param);
				return;
#else
				param.ShowModal = false;
#endif
			}
#endif
			PlatformIndependentFormHelper.ShowDialogContent(content, owner, param);
		}
#if SL
		public static void Show(FrameworkElement content, FrameworkElement owner, FloatingContainerParameters param) {
			PlatformIndependentFormHelper.Show(content, owner, param);
		}		
#endif
		public static void Close(UserControl form, bool dialogResult) {
			if (form == null)
				return;
			PlatformIndependentFormHelper.Close(form, dialogResult);
		}
		[Obsolete(ObsoleteText.SRSchedulerFormBehaviorSetFormTitle)]
		public static void SetFormTitle(UserControl dialog, string title) {
			PlatformIndependentFormHelper.SetFormTitleCore(dialog, title);
		}
		public static Size GetSize(FrameworkElement form) {
			return PlatformIndependentFormHelper.GetSize(form);
		}
		public static void SetSize(FrameworkElement form, Size size) {
			PlatformIndependentFormHelper.SetSize(form, size);
		}
#if DEBUGTEST
		public static bool ForceAllFormIsNonModal = false;
#endif
	}
}
