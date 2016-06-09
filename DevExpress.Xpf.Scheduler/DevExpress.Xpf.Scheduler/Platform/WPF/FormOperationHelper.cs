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
using DevExpress.Xpf.Core;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using System.Windows;
using DevExpress.Xpf.Scheduler.Internal;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Scheduler.Drawing;
namespace DevExpress.Xpf.Scheduler {
	[Obsolete(ObsoleteText.SRFormOperationHelper)]
	public class FormOperationHelper {
		public static void CloseDialog(UserControl dialog, bool dialogResult) {
			PlatformIndependentFormHelper.Close(dialog, dialogResult);
		}
		public static void SetFormCaption(UserControl dialog, string caption) {
			PlatformIndependentFormHelper.SetFormTitle(dialog, caption);
		}
	}
}
namespace DevExpress.Xpf.Scheduler.Internal {
	public class PlatformIndependentFormHelper {
		internal static void Show(UserControl dialog) {
			if (dialog == null)
				return;
			FloatingContainer container = FloatingContainer.GetFloatingContainer(dialog);
			if (container != null) {
				container.ShowActivated = true;
				return;
			}
			WindowContentHolder holder = LayoutHelper.GetRoot(dialog) as WindowContentHolder;
			if (holder != null)
				holder.ShowDialog();
		}
		public static void Close(UserControl dialog, bool dialogResult) {
			if (dialog == null)
				return;
			FloatingContainer container = FloatingContainer.GetFloatingContainer(dialog);
			if (container != null) {
				container.CloseDialog(dialogResult);
				return;
			}
			WindowContentHolder holder = LayoutHelper.GetRoot(dialog) as WindowContentHolder;
			if (holder != null)
				holder.Close();
			SchedulerPopup.Close(dialog);
		}
		public static void ShowDialog(FrameworkElement content, FrameworkElement owner, FloatingContainerParameters param) {
			SchedulerControl control = SchedulerFormBehavior.GetSchedulerControl(content);
#if !SL
			if (PresentationSource.FromVisual(owner) == null) 
				owner = GetParentWithPresentationSource(owner);
#endif
#if DEBUGTEST
			if (DevExpress.Xpf.Scheduler.Tests.TestFormHelper.ForceNonModalMode)
				control = null;
			FloatingContainer container = FloatingContainer.ShowDialog(content, owner, Size.Empty, param, control);
#else            
			FloatingContainer container = FloatingContainer.ShowDialog(content, owner, Size.Empty, param, control);
#endif
			OnDialogShown(container);
		}
 #if !SL
		static FrameworkElement GetParentWithPresentationSource(FrameworkElement element) {
			DependencyObject parent = element;
			do {
				parent = LayoutHelper.GetParent(parent, true);
			} while (parent != null && PresentationSource.FromVisual(parent as Visual) == null);
			return parent as FrameworkElement;
		}
#endif
		public static void ShowDialogContent(FrameworkElement content, FrameworkElement owner, FloatingContainerParameters param) {
			FloatingContainer container = FloatingContainer.ShowDialogContent(content, owner, Size.Empty, param);
			OnDialogShown(container);
		}
		protected static void OnDialogShown(FloatingContainer container) {
			container.SizeToContent = SizeToContent.WidthAndHeight;
			KeyboardNavigation.SetTabNavigation((DependencyObject)container.Content, KeyboardNavigationMode.Cycle); 
		}
		public static void SetFormTitle(UserControl dialog, string caption) {
			SetFormTitleCore(dialog, caption);
		}
		internal static bool SetFormTitleCore(DependencyObject d, string title) {
			if (d == null)
				return false;
			FloatingContainer container = FloatingContainer.GetFloatingContainer(d);
			if (container == null)
				return false;
			container.Caption = title;
			return true;
		}
		public static IDialogOwner GetDialogOwner(UserControl form) {
			return FloatingContainer.GetFloatingContainer(form);
		}
		public static Size GetSize(FrameworkElement form) {
			FloatingContainer container = FloatingContainer.GetFloatingContainer(form);
			if (container != null)
				return container.GetSize();
			WindowContentHolder holder = LayoutHelper.GetRoot(form) as WindowContentHolder;
			if (holder != null)
				return holder.GetSize();
			return Size.Empty;
		}
		public static void SetSize(FrameworkElement form, Size size) {
			FloatingContainer container = FloatingContainer.GetFloatingContainer(form);
			if (container != null)
				container.SetSize(size);
			WindowContentHolder holder = LayoutHelper.GetRoot(form) as WindowContentHolder;
			if (holder != null)
				holder.SetSize(size);
		}
	}
}
