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
using System.Windows.Controls;
using System.Windows;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if !FREE
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.Core {
#else
namespace DevExpress.Mvvm.UI {
#endif
	public class WaitIndicator : ContentControl {
		public static readonly DependencyProperty DeferedVisibilityProperty;
		public static readonly DependencyProperty ActualContentProperty;
		public static readonly DependencyProperty ShowShadowProperty;
		internal static readonly DependencyPropertyKey ActualContentPropertyKey;
		public static readonly DependencyProperty ContentPaddingProperty;
		static WaitIndicator() {
			Type ownerType = typeof(WaitIndicator);
			DeferedVisibilityProperty = DependencyProperty.Register("DeferedVisibility", typeof(bool), ownerType, new PropertyMetadata(false, OnDeferedVisibilityPropertyChanged));
			ShowShadowProperty = DependencyProperty.Register("ShowShadow", typeof(bool), ownerType, new PropertyMetadata(true));
			ContentPaddingProperty = DependencyProperty.Register("ContentPadding", typeof(Thickness), ownerType, new PropertyMetadata(new Thickness()));
			ActualContentPropertyKey = DependencyProperty.RegisterReadOnly("ActualContent", typeof(object), ownerType, new FrameworkPropertyMetadata(null));
			ActualContentProperty = ActualContentPropertyKey.DependencyProperty;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			OnDeferedVisibilityChanged();
			ChangeContentIfNeed(Content);
		}
		static void OnDeferedVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((WaitIndicator)d).OnDeferedVisibilityChanged();
		}
		public WaitIndicator() {
#if !FREE
			this.SetDefaultStyleKey(typeof(WaitIndicator));
#else
			DefaultStyleKey = typeof(WaitIndicator);
#endif
		}
		protected override void OnContentChanged(object oldContent, object newContent) {
			base.OnContentChanged(oldContent, newContent);
			ChangeContentIfNeed(newContent);
		}
		void ChangeContentIfNeed(object newContent) {
#if !FREE
			ActualContent = newContent ?? EditorLocalizer.Active.GetLocalizedString(EditorStringId.WaitIndicatorText);
#else
			ActualContent = newContent ?? "Loading...";
#endif
		}
		void OnDeferedVisibilityChanged() {
			if(DeferedVisibility)
				VisualStateManager.GoToState(this, "Visible", true);
			else
				VisualStateManager.GoToState(this, "Collapsed", true);
		}
		public bool DeferedVisibility {
			get { return (bool)GetValue(DeferedVisibilityProperty); }
			set { SetValue(DeferedVisibilityProperty, value); }
		}
		public object ActualContent {
			get { return (object)GetValue(ActualContentProperty); }
			internal set { this.SetValue(ActualContentPropertyKey, value); }
		}
		public bool ShowShadow {
			get { return (bool)GetValue(ShowShadowProperty); }
			set { SetValue(ShowShadowProperty, value); }
		}
		public Thickness ContentPadding {
			get { return (Thickness)GetValue(ContentPaddingProperty); }
			set { SetValue(ContentPaddingProperty, value); }
		}
	}
	public class WaitIndicatorContainer : ContentControl {
		public WaitIndicatorContainer() {
#if FREE
			DefaultStyleKey = typeof(WaitIndicatorContainer);
		}
	}
#else
			this.SetDefaultStyleKey(typeof(WaitIndicatorContainer));
		}
	}
	public class ColumnWaitIndicator : ProgressBar {
		public ColumnWaitIndicator() {
			this.SetDefaultStyleKey(typeof(ColumnWaitIndicator));
		}
	}
	public enum WaitIndicatorThemeKeys {
		WaitIndicatorTemplate,
		WaitIndicatorContentTemplate,
		WaitIndicatorContainerTemplate
	}
	public class WaitIndicatorThemeKeyExtension : ThemeKeyExtensionBase<WaitIndicatorThemeKeys> {
	}
#endif
}
