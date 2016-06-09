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
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.DemoBase.Helpers {
	public static class LinkedDefinition {
		#region Dependency Properties
		public static readonly DependencyProperty UseFixedValueProperty;
		public static readonly DependencyProperty FixedValueProperty;
		public static readonly DependencyProperty AdjustmentProperty;
		public static readonly DependencyProperty FactorProperty;
		public static readonly DependencyProperty SourceElementProperty;
		static LinkedDefinition() {
			Type ownerType = typeof(LinkedDefinition);
			SourceElementProperty = DependencyProperty.RegisterAttached("SourceElement", typeof(FrameworkElement), ownerType, new PropertyMetadata(null, RaiseSourceElementChanged));
			UseFixedValueProperty = DependencyProperty.RegisterAttached("UseFixedValue", typeof(bool), ownerType, new PropertyMetadata(false, RaiseValuablePropertyChanged));
			FixedValueProperty = DependencyProperty.RegisterAttached("FixedValue", typeof(double), ownerType, new PropertyMetadata(0.0, RaiseValuablePropertyChanged));
			AdjustmentProperty = DependencyProperty.RegisterAttached("Adjustment", typeof(double), ownerType, new PropertyMetadata(0.0, RaiseValuablePropertyChanged));
			FactorProperty = DependencyProperty.RegisterAttached("Factor", typeof(double), ownerType, new PropertyMetadata(1.0, RaiseValuablePropertyChanged));
		}
		#endregion
		public static void SetSourceElement(DependencyObject column, FrameworkElement element) { column.SetValue(SourceElementProperty, element); }
		public static FrameworkElement GetSourceElement(DependencyObject column) { return (FrameworkElement)column.GetValue(SourceElementProperty); }
		public static void SetUseFixedValue(DependencyObject column, bool value) { column.SetValue(UseFixedValueProperty, value); }
		public static bool GetUseFixedValue(DependencyObject column) { return (bool)column.GetValue(UseFixedValueProperty); }
		public static void SetFixedValue(DependencyObject column, double value) { column.SetValue(FixedValueProperty, value); }
		public static double GetFixedValue(DependencyObject column) { return (double)column.GetValue(FixedValueProperty); }
		public static void SetAdjustment(DependencyObject column, double value) { column.SetValue(AdjustmentProperty, value); }
		public static double GetAdjustment(DependencyObject column) { return (double)column.GetValue(AdjustmentProperty); }
		public static void SetFactor(DependencyObject column, double value) { column.SetValue(FactorProperty, value); }
		public static double GetFactor(DependencyObject column) { return (double)column.GetValue(FactorProperty); }
		static void RaiseSourceElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FrameworkElement oldValue = (FrameworkElement)e.OldValue;
			FrameworkElement newValue = (FrameworkElement)e.NewValue;
			if(oldValue != null)
				oldValue.SizeChanged -= (s, ea) => OnSourceElementSizeChanged(d, ea);
			if(newValue != null)
				newValue.SizeChanged += (s, ea) => OnSourceElementSizeChanged(d, ea);
			UpdateSize(d);
		}
		static void RaiseValuablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { UpdateSize(d); }
		static void OnSourceElementSizeChanged(DependencyObject d, SizeChangedEventArgs e) { UpdateSize(d); }
		static void UpdateSize(DependencyObject d) {
			UpdateColumnWidth(d as ColumnDefinition);
			UpdateRowHeight(d as RowDefinition);
		}
		static void UpdateColumnWidth(ColumnDefinition column) {
			if(column == null) return;
			FrameworkElement SourceElement = GetSourceElement(column);
			if(SourceElement == null) return;
			double adjustment = GetAdjustment(column);
			column.Width = new GridLength(GetUseFixedValue(column) ? GetFixedValue(column) : adjustment + GetFactor(column) * (SourceElement.ActualWidth - adjustment));
		}
		static void UpdateRowHeight(RowDefinition row) {
			if(row == null) return;
			FrameworkElement heightSourceElement = GetSourceElement(row);
			if(heightSourceElement == null) return;
			double adjustment = GetAdjustment(row);
			row.Height = new GridLength(GetUseFixedValue(row) ? GetFixedValue(row) : adjustment + GetFactor(row) * (heightSourceElement.ActualHeight - adjustment));
		}
	}
	public enum TranslationDirection { Up, Down, Left, Right };
	public static class LinkedTranslation {
		#region Dependency Properties
		public static readonly DependencyProperty DirectionProperty;
		public static readonly DependencyProperty AdjustmentProperty;
		public static readonly DependencyProperty FactorProperty;
		public static readonly DependencyProperty SourceElementProperty;
		static LinkedTranslation() {
			Type ownerType = typeof(LinkedTranslation);
			SourceElementProperty = DependencyProperty.RegisterAttached("SourceElement", typeof(FrameworkElement), ownerType, new PropertyMetadata(null, RaiseSourceElementChanged));
			DirectionProperty = DependencyProperty.RegisterAttached("Direction", typeof(TranslationDirection), ownerType, new PropertyMetadata(TranslationDirection.Left, RaiseValuablePropertyChanged));
			AdjustmentProperty = DependencyProperty.RegisterAttached("Adjustment", typeof(double), ownerType, new PropertyMetadata(0.0, RaiseValuablePropertyChanged));
			FactorProperty = DependencyProperty.RegisterAttached("Factor", typeof(double), ownerType, new PropertyMetadata(1.0, RaiseValuablePropertyChanged));
		}
		#endregion
		public static void SetSourceElement(DependencyObject column, FrameworkElement element) { column.SetValue(SourceElementProperty, element); }
		public static FrameworkElement GetSourceElement(DependencyObject column) { return (FrameworkElement)column.GetValue(SourceElementProperty); }
		public static void SetDirection(DependencyObject column, TranslationDirection value) { column.SetValue(DirectionProperty, value); }
		public static TranslationDirection GetDirection(DependencyObject column) { return (TranslationDirection)column.GetValue(DirectionProperty); }
		public static void SetAdjustment(DependencyObject column, double value) { column.SetValue(AdjustmentProperty, value); }
		public static double GetAdjustment(DependencyObject column) { return (double)column.GetValue(AdjustmentProperty); }
		public static void SetFactor(DependencyObject column, double value) { column.SetValue(FactorProperty, value); }
		public static double GetFactor(DependencyObject column) { return (double)column.GetValue(FactorProperty); }
		static void RaiseSourceElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FrameworkElement oldValue = (FrameworkElement)e.OldValue;
			FrameworkElement newValue = (FrameworkElement)e.NewValue;
			if(oldValue != null)
				oldValue.SizeChanged -= (s, ea) => OnSourceElementSizeChanged(d, ea);
			if(newValue != null)
				newValue.SizeChanged += (s, ea) => OnSourceElementSizeChanged(d, ea);
			UpdateTranslation(d as FrameworkElement);
		}
		static void RaiseValuablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { UpdateTranslation(d as FrameworkElement); }
		static void OnSourceElementSizeChanged(DependencyObject d, SizeChangedEventArgs e) { UpdateTranslation(d as FrameworkElement); }
		static void UpdateTranslation(FrameworkElement translatingElement) {
			if(translatingElement == null) return;
			if(translatingElement.RenderTransform == null) {
				translatingElement.RenderTransform = new TranslateTransform();
			}
			TranslationDirection direction = GetDirection(translatingElement);
			translatingElement.RenderTransform = ((TranslateTransform)translatingElement.RenderTransform).Clone();
			FrameworkElement sourceElement = GetSourceElement(translatingElement);
			if(sourceElement == null) return;
			if(direction == TranslationDirection.Left || direction == TranslationDirection.Right) {
				double translationValue = GetTranslationValue(sourceElement.ActualWidth, GetFactor(translatingElement), GetAdjustment(translatingElement));
				((TranslateTransform)translatingElement.RenderTransform).X = direction == TranslationDirection.Left ? -translationValue : translationValue;
			} else {
				double translationValue = GetTranslationValue(sourceElement.ActualHeight, GetFactor(translatingElement), GetAdjustment(translatingElement));
				((TranslateTransform)translatingElement.RenderTransform).Y = direction == TranslationDirection.Up ? -translationValue : translationValue;
			}
		}
		static double GetTranslationValue(double sourceDimensionValue, double factor, double adjustment) {
			return adjustment + factor * (sourceDimensionValue - adjustment);
		}
	}
	public static class LinkedMaxValueHelper {
		#region Dependency Properties
		public static readonly DependencyProperty AdjustmentProperty;
		public static readonly DependencyProperty FactorProperty;
		public static readonly DependencyProperty SourceElementProperty;
		public static readonly DependencyProperty AllowBindHeightProperty;
		public static readonly DependencyProperty AllowBindWidthProperty;
		static LinkedMaxValueHelper() {
			Type ownerType = typeof(LinkedMaxValueHelper);
			SourceElementProperty = DependencyProperty.RegisterAttached("SourceElement", typeof(FrameworkElement), ownerType, new PropertyMetadata(null, RaiseSourceElementChanged));
			AdjustmentProperty = DependencyProperty.RegisterAttached("Adjustment", typeof(double), ownerType, new PropertyMetadata(0.0, RaiseValuablePropertyChanged));
			FactorProperty = DependencyProperty.RegisterAttached("Factor", typeof(double), ownerType, new PropertyMetadata(1.0, RaiseValuablePropertyChanged));
			AllowBindHeightProperty = DependencyProperty.RegisterAttached("AllowBindHeight", typeof(bool), ownerType, new PropertyMetadata(true, RaiseValuablePropertyChanged));
			AllowBindWidthProperty = DependencyProperty.RegisterAttached("AllowBindWidth", typeof(bool), ownerType, new PropertyMetadata(true, RaiseValuablePropertyChanged));
		}
		#endregion
		public static void SetSourceElement(DependencyObject column, FrameworkElement element) { column.SetValue(SourceElementProperty, element); }
		public static FrameworkElement GetSourceElement(DependencyObject column) { return (FrameworkElement)column.GetValue(SourceElementProperty); }
		public static void SetAdjustment(DependencyObject column, double value) { column.SetValue(AdjustmentProperty, value); }
		public static double GetAdjustment(DependencyObject column) { return (double)column.GetValue(AdjustmentProperty); }
		public static void SetFactor(DependencyObject column, double value) { column.SetValue(FactorProperty, value); }
		public static double GetFactor(DependencyObject column) { return (double)column.GetValue(FactorProperty); }
		public static void SetAllowBindHeight(DependencyObject column, bool value) { column.SetValue(AllowBindHeightProperty, value); }
		public static bool GetAllowBindHeight(DependencyObject column) { return (bool)column.GetValue(AllowBindHeightProperty); }
		public static void SetAllowBindWidth(DependencyObject column, bool value) { column.SetValue(AllowBindWidthProperty, value); }
		public static bool GetAllowBindWidth(DependencyObject column) { return (bool)column.GetValue(AllowBindWidthProperty); }
		static void RaiseSourceElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FrameworkElement oldValue = (FrameworkElement)e.OldValue;
			FrameworkElement newValue = (FrameworkElement)e.NewValue;
			if(oldValue != null)
				oldValue.SizeChanged -= (s, ea) => OnSourceElementSizeChanged(d, ea);
			if(newValue != null)
				newValue.SizeChanged += (s, ea) => OnSourceElementSizeChanged(d, ea);
			UpdateSize(d as FrameworkElement);
		}
		static void RaiseValuablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { UpdateSize(d as FrameworkElement); }
		static void OnSourceElementSizeChanged(DependencyObject d, SizeChangedEventArgs e) { UpdateSize(d as FrameworkElement); }
		static void UpdateSize(FrameworkElement d) {
			if(d == null) return;
			FrameworkElement sourceElement = GetSourceElement(d);
			if(sourceElement == null) return;
			d.MaxWidth = GetAllowBindWidth(d) ? GetTranslationValue(double.IsNaN(sourceElement.ActualWidth) ? 0.0 : sourceElement.ActualWidth, GetFactor(d), GetAdjustment(d)) : Double.PositiveInfinity;
			d.MaxHeight = GetAllowBindHeight(d) ? GetTranslationValue(double.IsNaN(sourceElement.ActualHeight) ? 0.0 : sourceElement.ActualHeight, GetFactor(d), GetAdjustment(d)) : Double.PositiveInfinity;
		}
		static double GetTranslationValue(double sourceDimensionValue, double factor, double adjustment) {
			return adjustment + factor * (sourceDimensionValue - adjustment);
		}
	}
}
