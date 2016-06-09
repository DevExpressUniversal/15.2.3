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
using DevExpress.Xpf.Editors.Flyout.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.ComponentModel;
using System.Windows;
using FlyoutStrategy = DevExpress.Xpf.Editors.Flyout.Native.FlyoutBase.FlyoutStrategy;
namespace DevExpress.Xpf.Editors.Flyout {
	public enum FlyoutPlacement {
		Left,
		Top,
		Right,
		Bottom
	}
	public abstract class FlyoutSettingsBase : DependencyObject, INotifyPropertyChanged {
		static FlyoutSettingsBase() {
			Type ownerType = typeof(FlyoutSettingsBase);
		}
		protected FlyoutSettingsBase() { }
		public virtual void Apply(FlyoutPositionCalculator calculator, FlyoutBase flyout) {
		}
		public virtual IndicatorDirection GetIndicatorDirection(FlyoutPlacement placement) {
			return IndicatorDirection.None;
		}
		public virtual void OnPropertyChanged(FlyoutBase flyout, PropertyChangedEventArgs e) {
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			RaisePropertyChanged(e.Property.Name);
		}
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChanged.Do(x => x(this, new PropertyChangedEventArgs(propertyName)));
		}
		public event PropertyChangedEventHandler PropertyChanged;
		public abstract FlyoutPositionCalculator CreatePositionCalculator();
		public abstract FlyoutStrategy CreateStrategy();
		public virtual FlyoutAnimatorBase CreateAnimator() {
			return new FlyoutAnimator();
		}
	}
	public class FlyoutSettings : FlyoutSettingsBase {
		public static readonly DependencyProperty ShowIndicatorProperty;
		public static readonly DependencyProperty PlacementProperty;
		public static readonly DependencyProperty IndicatorTargetProperty;
		public static readonly DependencyProperty IndicatorHorizontalAlignmentProperty;
		public static readonly DependencyProperty IndicatorVerticalAlignmentProperty;
		static FlyoutSettings() {
			Type ownerType = typeof(FlyoutSettings);
			ShowIndicatorProperty = DependencyPropertyManager.Register("ShowIndicator", typeof(bool), ownerType, new PropertyMetadata(false));
			PlacementProperty = DependencyPropertyManager.Register("Placement", typeof(FlyoutPlacement), ownerType, new PropertyMetadata(FlyoutPlacement.Bottom));
			IndicatorTargetProperty = DependencyPropertyManager.Register("IndicatorTarget", typeof(UIElement), ownerType, new PropertyMetadata(null));
			IndicatorHorizontalAlignmentProperty = DependencyPropertyManager.Register("IndicatorHorizontalAlignment", typeof(Nullable<HorizontalAlignment>), ownerType, new PropertyMetadata(null));
			IndicatorVerticalAlignmentProperty = DependencyPropertyManager.Register("IndicatorVerticalAlignment", typeof(Nullable<VerticalAlignment>), ownerType, new PropertyMetadata(null));
		}
		public FlyoutSettings() { }
		public FlyoutPlacement Placement {
			get { return (FlyoutPlacement)GetValue(PlacementProperty); }
			set { SetValue(PlacementProperty, value); }
		}
		public bool ShowIndicator {
			get { return (bool)GetValue(ShowIndicatorProperty); }
			set { SetValue(ShowIndicatorProperty, value); }
		}
		public UIElement IndicatorTarget {
			get { return (UIElement)GetValue(IndicatorTargetProperty); }
			set { SetValue(IndicatorTargetProperty, value); }
		}
		public Nullable<HorizontalAlignment> IndicatorHorizontalAlignment {
			get { return (Nullable<HorizontalAlignment>)GetValue(IndicatorHorizontalAlignmentProperty); }
			set { SetValue(IndicatorHorizontalAlignmentProperty, value); }
		}
		public Nullable<VerticalAlignment> IndicatorVerticalAlignment {
			get { return (Nullable<VerticalAlignment>)GetValue(IndicatorVerticalAlignmentProperty); }
			set { SetValue(IndicatorVerticalAlignmentProperty, value); }
		}
		public override void OnPropertyChanged(FlyoutBase flyout, PropertyChangedEventArgs e) {
			if (e.PropertyName == ShowIndicatorProperty.Name ||
				e.PropertyName == PlacementProperty.Name ||
				e.PropertyName == IndicatorTargetProperty.Name ||
				e.PropertyName == IndicatorHorizontalAlignmentProperty.Name ||
				e.PropertyName == IndicatorVerticalAlignmentProperty.Name) {
					flyout.InvalidateLocation();
			}
		}
		public override void Apply(FlyoutPositionCalculator calculator, FlyoutBase flyout) {
			calculator.Placement = Placement;
			calculator.ActualIndicatorDirection = flyout.IndicatorDirection;
			calculator.IndicatorHorizontalAlignment = IndicatorHorizontalAlignment.HasValue ? IndicatorHorizontalAlignment.Value : flyout.HorizontalAlignment;
			calculator.IndicatorVerticalAlignment = IndicatorVerticalAlignment.HasValue ? IndicatorVerticalAlignment.Value : flyout.VerticalAlignment;
			calculator.IndicatorTargetBounds = ScreenHelper.GetScaledRect(TranslateHelper.ToScreen(flyout.PlacementTarget, flyout.GetTargetBounds(flyout.PlacementTarget, IndicatorTarget, () => flyout.GetTargetBounds())));
		}
		public override IndicatorDirection GetIndicatorDirection(FlyoutPlacement placement) {
			if (!ShowIndicator)
				return IndicatorDirection.None;
			switch (placement) {
				case FlyoutPlacement.Left:
					return IndicatorDirection.Right;
				case FlyoutPlacement.Top:
					return IndicatorDirection.Bottom;
				case FlyoutPlacement.Right:
					return IndicatorDirection.Left;
				case FlyoutPlacement.Bottom:
					return IndicatorDirection.Top;
				default:
					return IndicatorDirection.None;
			}
		}
		public override FlyoutPositionCalculator CreatePositionCalculator() {
			return new FlyoutPositionCalculator();
		}
		public override FlyoutStrategy CreateStrategy() {
			return new FlyoutStrategy();
		}
	}
	public class FlyInSettings : FlyoutSettingsBase {
		public FlyInSettings()  { }
		public override FlyoutPositionCalculator CreatePositionCalculator() {
			return new FlyinPositionCalculator();
		}
		public override FlyoutStrategy CreateStrategy() {
			return new FlyoutBase.FlyinStrategy();
		}
	}
}
