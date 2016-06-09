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
using System.Windows;
#if !SL
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Core;
#else
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
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
namespace DevExpress.Xpf.Editors.Validation {
	public interface ISupportDXValidation {
		bool DoValidate();
		InvalidValueBehavior InvalidValueBehavior { get; }
		bool HasValidationError { get; }
		BaseValidationError ValidationError { get; }
	}
	public class MouseEventLockHelper : DependencyObject {
		public static readonly DependencyProperty ValidationLockInitializedProperty;
		static readonly DependencyPropertyKey ValidationLockInitializedPropertyKey;
		static MouseEventLockHelper() {
			Type ownerType = typeof(MouseEventLockHelper);
			ValidationLockInitializedPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ValidationLockInitialized", typeof(bool), ownerType, 
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None));
			ValidationLockInitializedProperty = ValidationLockInitializedPropertyKey.DependencyProperty;
		}
		public static bool GetValidationLockInitialized(DependencyObject d) {
			return (bool)d.GetValue(ValidationLockInitializedProperty);
		}
		internal static void SetValidationLockInitialized(DependencyObject d, bool value) {
			d.SetValue(ValidationLockInitializedPropertyKey, value);
		}
		static void SubscribeEventsForLock(FrameworkElement root) {
#if !SL
			root.PreviewMouseDown += root_PreviewMouseDown;
			root.PreviewMouseUp += root_PreviewMouseUp;
			root.PreviewMouseWheel += root_PreviewMouseWheel;
#endif
		}
		static void root_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e) {
			e.Handled = ShouldHandleEvent(e.OriginalSource as DependencyObject);
		}
		static void root_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			e.Handled = ShouldHandleEvent(e.OriginalSource as DependencyObject);
		}
		static void root_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			e.Handled = ShouldHandleEvent(e.OriginalSource as DependencyObject);
		}
		static bool ShouldHandleEvent(DependencyObject originalSource) {
			IBaseEdit focused = LayoutHelper.FindLayoutOrVisualParentObject<IBaseEdit>(FocusHelper.GetFocusedElement() as DependencyObject, true);
			if(focused == null || focused.InvalidValueBehavior == InvalidValueBehavior.AllowLeaveEditor)
				return false;
#if !SL
			DXWindow window = LayoutHelper.FindRoot(originalSource) as DXWindow;
			WindowContentHolder wch = LayoutHelper.FindRoot(originalSource) as WindowContentHolder;
			if(window != null || wch != null) if(!ShouldHandleDXWindowChild(originalSource)) return false;
#endif
			IBaseEdit target = LayoutHelper.FindLayoutOrVisualParentObject<IBaseEdit>(originalSource, true);
			bool hasValidationError = focused.EditMode != EditMode.Standalone && focused.HasValidationError || target != focused ? !focused.DoValidate() : false;
			if(target != focused && hasValidationError)
				return true;
			return false;
		}
#if !SL
		private static bool ShouldHandleDXWindowChild(DependencyObject originalSource) {
			DependencyObject currentParent;
			FrameworkElement currentParentNamed;
			DependencyObject current = originalSource;
			do {
				currentParent = LayoutHelper.GetParent(current, false);
				currentParentNamed = currentParent as FrameworkElement;
				if(current == currentParent) break;
				current = currentParent;
			}
			while(currentParent != null && (currentParentNamed != null ? currentParentNamed.Name != "PART_ContainerContent" : true));
			return currentParentNamed != null ? currentParentNamed.Name == "PART_ContainerContent" : false;
		}
#endif
		public static void SubscribeMouseEvents(DependencyObject d) {
			FrameworkElement root = LayoutHelper.FindRoot(d) as FrameworkElement;
			if(root == null)
				return;
			if(GetValidationLockInitialized(root))
				return;
			SetValidationLockInitialized(root, true);
			SubscribeEventsForLock(root);
		}
	}
}
