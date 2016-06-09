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

using DevExpress.Data;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Core {
	public class DXWindowBackgroundPanel : BackgroundPanel {
		public DXWindowBackgroundPanel() {
			ContentControlHelper.SetContentIsNotLogical(this, true);
		}
	}
	public class BackgroundPanel : ContentControl {
		static BackgroundPanel() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BackgroundPanel), new FrameworkPropertyMetadata(typeof(BackgroundPanel)));
			ContentControl.ContentProperty.AddOwner(typeof(BackgroundPanel), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnUpdateProperty)));
			TextBlock.ForegroundProperty.AddOwner(typeof(BackgroundPanel), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnUpdateProperty)));
			TextBlock.FontFamilyProperty.AddOwner(typeof(BackgroundPanel), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnUpdateProperty)));
			TextBlock.FontSizeProperty.AddOwner(typeof(BackgroundPanel), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnUpdateProperty)));
		}
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty PropertyWasModifiedByBackgroundPanelProperty = DependencyProperty.RegisterAttached("PropertyWasModifiedByBackgroundPanel", typeof(bool), typeof(BackgroundPanel), new UIPropertyMetadata(false));
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			CorrectProperty();
		}
		List<Type> stopList = new List<Type>(new Type[] { typeof(DXTabControl), typeof(RichTextBox), typeof(TextEditBase)});
		private void CorrectProperty() {
			this.DelayedExecute(() => {
				FrameworkElement contentPresenter = LayoutHelper.FindElementByName(this, "contentPresenter");
				if(contentPresenter == null) return;
				FrameworkElement child = LayoutHelper.FindElement(contentPresenter, (e) => (e is FrameworkElement) && e != contentPresenter);
				if(child == null) return;
				bool isPropertyCorrectionAllowed = true;
				foreach(Type ttype in stopList) {
					if(child.GetType().IsAssignableFrom(ttype)) {
						isPropertyCorrectionAllowed = false;
						break;
					}
				}
				if(!isPropertyCorrectionAllowed) return;
				FrameworkElement parent = Parent as FrameworkElement;
				if(parent == null) SetOwnPropertyValues(child);
				else {
					bool res1, res2, res3;
					res1 = CorrectPropertyCore(child, parent, TextBlock.ForegroundProperty);
					res2 = CorrectPropertyCore(child, parent, TextBlock.FontFamilyProperty);
					res3 = CorrectPropertyCore(child, parent, TextBlock.FontSizeProperty);
					if(!res1 && !res2 && !res3) child.ClearValue(PropertyWasModifiedByBackgroundPanelProperty);
					else child.SetValue(PropertyWasModifiedByBackgroundPanelProperty, true);
				}
			});
		}
		protected virtual void SetValue(DependencyObject target, DependencyProperty dp, object value) {
			target.SetValue(dp, value);
		}
		protected virtual void ClearValue(DependencyObject target, DependencyProperty dp) {
			target.ClearValue(dp);
		}
		private bool CorrectPropertyCore(FrameworkElement child, FrameworkElement parent, DependencyProperty dp) {
			bool result = false;
			if(dp == TextBlock.ForegroundProperty) {
				if(CanSetParentPropertyValue(parent, child, dp)) {
					SetValue(child, dp, parent.GetValue(dp));
					result = true;
				} else result = SetOwnPropertyValuesCore(child, dp);
			} else {
				object defaultPropertyValue = dp.GetMetadata(parent).DefaultValue;
				object currentParentPropertyValue = parent.GetValue(dp);
				object currentChildPropertyValue = child.GetValue(dp);
				ValueSource valueSource = System.Windows.DependencyPropertyHelper.GetValueSource(parent, dp);
				if(ComparePropertyValues(dp, defaultPropertyValue, currentChildPropertyValue) || valueSource.BaseValueSource == BaseValueSource.DefaultStyle) {
					if(!ComparePropertyValues(dp, defaultPropertyValue, currentParentPropertyValue))
						SetValue(child, dp, parent.GetValue(dp));
					else
						SetValue(child, dp, GetValue(dp));
					result = true;
				} else {
					if((bool)child.GetValue(PropertyWasModifiedByBackgroundPanelProperty)) ClearValue(child, dp);
				}
			}
			return result;
		}
		private void SetOwnPropertyValues(FrameworkElement child) {
			bool res1, res2, res3;
			res1 = SetOwnPropertyValuesCore(child, TextBlock.ForegroundProperty);
			res2 = SetOwnPropertyValuesCore(child, TextBlock.FontFamilyProperty);
			res3 = SetOwnPropertyValuesCore(child, TextBlock.FontSizeProperty);
			if(!res1 && !res2 && !res3) child.ClearValue(PropertyWasModifiedByBackgroundPanelProperty);
			else child.SetValue(PropertyWasModifiedByBackgroundPanelProperty, true);
		}
		private bool SetOwnPropertyValuesCore(FrameworkElement child, DependencyProperty dp) {
			bool result = false;
			if(GetValue(dp) != null && !(child is TextBox)) {
				SetValue(child, dp, GetValue(dp));
				result = true;
			} else ClearValue(child, dp);
			return result;
		}
		bool ComparePropertyValues(DependencyProperty dp, object object1, object object2) {
			if(object2 == object1) return true;
			if(dp == TextBlock.FontSizeProperty) return (double)object1 == (double)object2;
			if(dp == TextBlock.FontFamilyProperty) {
				FontFamily ff1 = object1 as FontFamily, ff2 = object2 as FontFamily;
				if(ff1 != null && ff2 != null && ff1.Source == ff2.Source) return true;
			}
			if(dp == TextBlock.ForegroundProperty) {
				SolidColorBrush fc1 = object1 as SolidColorBrush, fc2 = object2 as SolidColorBrush;
				if(fc1 != null && fc2 != null && fc1.Color == fc2.Color) return true;
			}
			return false;
		}
		private bool CanSetParentPropertyValue(FrameworkElement parent, FrameworkElement child, DependencyProperty dp) {
			bool areEquals = false;
			object defaultPropertyValue = dp.GetMetadata(parent).DefaultValue;
			object currentPropertyValue = parent.GetValue(dp);
			if(currentPropertyValue != null && defaultPropertyValue != null)
				areEquals = ComparePropertyValues(dp, defaultPropertyValue, currentPropertyValue);
			return parent.GetValue(dp) != null && !(child is TextBox) && !areEquals;
		}
		protected static void OnUpdateProperty(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BackgroundPanel bp = d as BackgroundPanel;
			if(bp != null) bp.CorrectProperty();
		}
	}
	[Obsolete("use the standard UserControl instead")]
	public class ThemedUserControl : UserControl {
		static ThemedUserControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ThemedUserControl), new FrameworkPropertyMetadata(typeof(ThemedUserControl)));
		}
	}
}
namespace DevExpress.Xpf.Core.Native {
	public static class ContentControlHelper {
		static readonly Action<ContentControl, bool> setContentInNotLogical =
			setContentInNotLogical = Internal.ReflectionHelper.CreateInstanceMethodHandler<Action<ContentControl, bool>>(null, "set_ContentIsNotLogical",
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, typeof(ContentControl));
		public static readonly DependencyProperty ContentIsNotLogicalProperty = DependencyProperty.RegisterAttached("ContentIsNotLogical", typeof(bool?), typeof(ContentControlHelper), new PropertyMetadata(null, OnContentIsNotLogicalChanged));
		public static bool? GetContentIsNotLogical(ContentControl obj) { return (bool?)obj.GetValue(ContentIsNotLogicalProperty); }
		public static void SetContentIsNotLogical(ContentControl obj, bool value) { obj.SetValue(ContentIsNotLogicalProperty, value); }
		static void OnContentIsNotLogicalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			setContentInNotLogical((ContentControl)d, (bool?)e.NewValue ?? false);
		}
	}
}
