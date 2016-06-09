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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public class LinearScaleLevelBarOptions : GaugeDependencyObject {
		public static readonly DependencyProperty OffsetProperty = DependencyPropertyManager.Register("Offset",
			typeof(double), typeof(LinearScaleLevelBarOptions), new PropertyMetadata(0.0, NotifyPropertyChanged));
		public static readonly DependencyProperty FactorThicknessProperty = DependencyPropertyManager.Register("FactorThickness",
			typeof(double), typeof(LinearScaleLevelBarOptions), new PropertyMetadata(1.0, NotifyPropertyChanged), FactorValidation);
		public static readonly DependencyProperty ZIndexProperty = DependencyPropertyManager.Register("ZIndex",
		   typeof(int), typeof(LinearScaleLevelBarOptions), new PropertyMetadata(50, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleLevelBarOptionsOffset"),
#endif
		Category(Categories.Layout)
		]
		public double Offset {
			get { return (double)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleLevelBarOptionsFactorThickness"),
#endif
		Category(Categories.Layout)
		]
		public double FactorThickness {
			get { return (double)GetValue(FactorThicknessProperty); }
			set { SetValue(FactorThicknessProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleLevelBarOptionsZIndex"),
#endif
		Category(Categories.Layout)
		]
		public int ZIndex {
			get { return (int)GetValue(ZIndexProperty); }
			set { SetValue(ZIndexProperty, value); }
		}
		static bool FactorValidation(object value) {
			return (double)value > 0;
		}
		protected override GaugeDependencyObject CreateObject() {
			return new LinearScaleLevelBarOptions();
		}
	}
	public class LinearScaleLevelBar : LinearScaleIndicator {
		public static readonly DependencyProperty PresentationProperty = DependencyPropertyManager.Register("Presentation",
			typeof(LinearScaleLevelBarPresentation), typeof(LinearScaleLevelBar), new PropertyMetadata(null, PresentationPropertyChanged));
		public static readonly DependencyProperty OptionsProperty = DependencyPropertyManager.Register("Options",
			typeof(LinearScaleLevelBarOptions), typeof(LinearScaleLevelBar), new PropertyMetadata(OptionsPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleLevelBarPresentation"),
#endif
		Category(Categories.Presentation)
		]
		public LinearScaleLevelBarPresentation Presentation {
			get { return (LinearScaleLevelBarPresentation)GetValue(PresentationProperty); }
			set { SetValue(PresentationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleLevelBarOptions"),
#endif
		Category(Categories.Presentation)
		]
		public LinearScaleLevelBarOptions Options {
			get { return (LinearScaleLevelBarOptions)GetValue(OptionsProperty); }
			set { SetValue(OptionsProperty, value); }
		}
		static void OptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			LinearScaleLevelBar level = d as LinearScaleLevelBar;
			if (level != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as LinearScaleLevelBarOptions, e.NewValue as LinearScaleLevelBarOptions, level);
				level.OnOptionsChanged();
			}
		}
#if !SL
	[DevExpressXpfGaugesLocalizedDescription("LinearScaleLevelBarPredefinedPresentations")]
#endif
		public static List<PredefinedElementKind> PredefinedPresentations {
			get { return PredefinedLinearScaleLevelBarPresentations.PresentationKinds; }
		}
		readonly ValueIndicatorInfo foregroundInfo;
		LinearScaleLevelBarModel Model { get { return Gauge != null && Gauge.ActualModel != null ? Gauge.ActualModel.GetLevelBarModel(Scale.LevelBars.IndexOf(this)) : null; } }
		protected override int ActualZIndex { get { return ActualOptions.ZIndex; } }
		protected override ValueIndicatorPresentation ActualPresentation {
			get {
				if (Presentation != null)
					return Presentation;
				if (Model != null && Model.Presentation != null)
					return Model.Presentation;
				return new DefaultLinearScaleLevelBarPresentation();
			}
		}
		protected internal override IEnumerable<ValueIndicatorInfo> Elements {
			get {
				yield return base.ElementInfo;
				yield return foregroundInfo;
			}
		}
		internal ValueIndicatorInfo ForegroundInfo { get { return foregroundInfo; } }
		internal LinearScaleLevelBarOptions ActualOptions {
			get {
				if (Options != null)
					return Options;
				if (Model != null && Model.Options != null)
					return Model.Options;
				return new LinearScaleLevelBarOptions();
			}
		}
		public LinearScaleLevelBar()
			: base() {
			LinearScaleLevelBarPresentation levelBarPresentation = ActualPresentation as LinearScaleLevelBarPresentation;
			foregroundInfo = levelBarPresentation != null ? new ValueIndicatorInfo(this, ActualZIndex, levelBarPresentation.CreateForegroundPresentationControl(), levelBarPresentation) : null;
		}
		double GetClipHeightByScaleLayoutMode(ElementInfoBase elementInfo) {
			double clipHeight = elementInfo.Layout.Height.Value;
			Rect relativeLevelBarRect = LayoutHelper.GetRelativeElementRect(ElementInfo.PresentationControl, Gauge);
			switch (Scale.LayoutMode) {
				case LinearScaleLayoutMode.BottomToTop:
					clipHeight += Gauge.ActualHeight - relativeLevelBarRect.Bottom;
					break;
				case LinearScaleLayoutMode.TopToBottom:
					clipHeight += relativeLevelBarRect.Top;
					break;
				case LinearScaleLayoutMode.LeftToRight:
					clipHeight += relativeLevelBarRect.Left;
					break;
				case LinearScaleLayoutMode.RightToLeft:
					clipHeight += Gauge.ActualWidth - relativeLevelBarRect.Right;
					break;
			}
			return clipHeight;
		}
		protected override GaugeDependencyObject CreateObject() {
			return new LinearScaleLevelBar();
		}
		protected override void UpdatePresentation() {
			base.UpdatePresentation();
			LinearScaleLevelBarPresentation levelBarPresentation = ActualPresentation as LinearScaleLevelBarPresentation;
			if (ForegroundInfo != null && levelBarPresentation != null) {
				ForegroundInfo.Presentation = levelBarPresentation;
				ForegroundInfo.PresentationControl = levelBarPresentation.CreateForegroundPresentationControl();
			}
		}
		protected override ElementLayout CreateLayout(Size constraint) {
			if (Scale != null && Scale.Mapping != null && !Scale.Mapping.Layout.IsEmpty) {
				double levelBarWidth;
				double levelBarHeight;
				if (Scale.LayoutMode == LinearScaleLayoutMode.BottomToTop || Scale.LayoutMode == LinearScaleLayoutMode.TopToBottom) {
					levelBarWidth = constraint.Width;
					levelBarHeight = Math.Abs(Scale.Mapping.Layout.ScaleVector.Y);
				}
				else {
					levelBarWidth = constraint.Height;
					levelBarHeight = Math.Abs(Scale.Mapping.Layout.ScaleVector.X);
				}
				return new ElementLayout(levelBarWidth, levelBarHeight);
			}
			return null;
		}
		protected override void CompleteLayout(ElementInfoBase elementInfo) {
			Point arrangePoint = Scale.Mapping.Layout.AnchorPoint;
			if (Scale.LayoutMode == LinearScaleLayoutMode.BottomToTop || Scale.LayoutMode == LinearScaleLayoutMode.TopToBottom)
				arrangePoint.X += ActualOptions.Offset;
			else
				arrangePoint.Y += ActualOptions.Offset;
			Point offset = Scale.GetLayoutOffset();
			arrangePoint.X += offset.X;
			arrangePoint.Y += offset.Y;			
			double rotationAngle = GetRotationAngleByScaleLayoutMode();
			TransformGroup transform = new TransformGroup();
			transform.Children.Add(new ScaleTransform() { ScaleX = ActualOptions.FactorThickness });
			transform.Children.Add(new RotateTransform() { Angle = rotationAngle });
			RectangleGeometry clipGeometry = null;
			if (elementInfo == foregroundInfo) {
				Point valuePoint = new Point(0.0, GetPointYByScaleLayoutMode(ActualValue));
				Point clipBottomPoint = new Point(elementInfo.Layout.Width.Value, GetClipHeightByScaleLayoutMode(elementInfo));
				clipGeometry = new RectangleGeometry();
				clipGeometry.Rect = new Rect(clipBottomPoint, valuePoint);
			}
			elementInfo.Layout.CompleteLayout(arrangePoint, transform, clipGeometry);
		}
	}
	public class LinearScaleLevelBarCollection : LinearScaleIndicatorCollection<LinearScaleLevelBar> {
		public LinearScaleLevelBarCollection(LinearScale scale)
			: base(scale) {
		}
	}
}
