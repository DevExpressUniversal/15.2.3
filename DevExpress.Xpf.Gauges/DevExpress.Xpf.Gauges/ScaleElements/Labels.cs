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

using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public enum ArcScaleLabelOrientation { 
		Radial,
		Tangent,
		LeftToRight,
		BottomToTop,
		TopToBottom
	}
	public enum LinearScaleLabelOrientation {
		LeftToRight,
		BottomToTop,
		TopToBottom
	}
	public abstract class ScaleLabelOptions : GaugeDependencyObject {
		public static readonly DependencyProperty OffsetProperty = DependencyPropertyManager.Register("Offset",
			typeof(double), typeof(ScaleLabelOptions), new PropertyMetadata(-50.0, NotifyPropertyChanged));
		public static readonly DependencyProperty ShowFirstProperty = DependencyPropertyManager.Register("ShowFirst",
			typeof(bool), typeof(ScaleLabelOptions), new PropertyMetadata(true, NotifyPropertyChanged));
		public static readonly DependencyProperty ShowLastProperty = DependencyPropertyManager.Register("ShowLast",
			typeof(bool), typeof(ScaleLabelOptions), new PropertyMetadata(true, NotifyPropertyChanged));
		public static readonly DependencyProperty AddendProperty = DependencyPropertyManager.Register("Addend",
			typeof(double), typeof(ScaleLabelOptions), new PropertyMetadata(0.0, NotifyPropertyChanged));
		public static readonly DependencyProperty MultiplierProperty = DependencyPropertyManager.Register("Multiplier",
			typeof(double), typeof(ScaleLabelOptions), new PropertyMetadata(1.0, NotifyPropertyChanged));
		public static readonly DependencyProperty FormatStringProperty = DependencyPropertyManager.Register("FormatString",
			typeof(string), typeof(ScaleLabelOptions), new PropertyMetadata("{0:0}", NotifyPropertyChanged));
		public static readonly DependencyProperty ZIndexProperty = DependencyPropertyManager.Register("ZIndex",
			typeof(int), typeof(ScaleLabelOptions), new PropertyMetadata(30, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleLabelOptionsOffset"),
#endif
		Category(Categories.Layout)
		]
		public double Offset {
			get { return (double)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleLabelOptionsShowFirst"),
#endif
		Category(Categories.Behavior)
		]
		public bool ShowFirst {
			get { return (bool)GetValue(ShowFirstProperty); }
			set { SetValue(ShowFirstProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleLabelOptionsShowLast"),
#endif
		Category(Categories.Behavior)
		]
		public bool ShowLast {
			get { return (bool)GetValue(ShowLastProperty); }
			set { SetValue(ShowLastProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleLabelOptionsAddend"),
#endif
		Category(Categories.Data)
		]
		public double Addend {
			get { return (double)GetValue(AddendProperty); }
			set { SetValue(AddendProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleLabelOptionsMultiplier"),
#endif
		Category(Categories.Data)
		]
		public double Multiplier {
			get { return (double)GetValue(MultiplierProperty); }
			set { SetValue(MultiplierProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleLabelOptionsFormatString"),
#endif
		Category(Categories.Data)
		]
		public string FormatString {
			get { return (string)GetValue(FormatStringProperty); }
			set { SetValue(FormatStringProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleLabelOptionsZIndex"),
#endif
		Category(Categories.Layout)
		]
		public int ZIndex {
			get { return (int)GetValue(ZIndexProperty); }
			set { SetValue(ZIndexProperty, value); }
		}
		protected internal abstract double CorrectAngleByOrientation(double angle);
		internal ScaleElementLayout CalculateLayout(ScaleLabelInfo elementInfo, ScaleMapping mapping) {
			if((!elementInfo.Tickmark.IsFirstTick || ShowFirst) && (!elementInfo.Tickmark.IsLastTick || ShowLast) && !mapping.Layout.IsEmpty) {
				double angle = CorrectAngleByOrientation(mapping.GetAngleByPercent(elementInfo.Tickmark.Alpha));
				return new ScaleElementLayout(angle, mapping.GetPointByPercent(elementInfo.Tickmark.Alpha, Offset));
			}
			return null;
		}
	}
	public class ArcScaleLabelOptions : ScaleLabelOptions {
		public static readonly DependencyProperty OrientationProperty = DependencyPropertyManager.Register("Orientation",
			typeof(ArcScaleLabelOrientation), typeof(ArcScaleLabelOptions), new PropertyMetadata(ArcScaleLabelOrientation.LeftToRight, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleLabelOptionsOrientation"),
#endif
		Category(Categories.Layout)
		]
		public ArcScaleLabelOrientation Orientation {
			get { return (ArcScaleLabelOrientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ArcScaleLabelOptions();
		}
		protected internal override double CorrectAngleByOrientation(double angle) {
			double normalizedAngle = MathUtils.NormalizeDegree(angle);
			switch (Orientation) { 
				case ArcScaleLabelOrientation.LeftToRight:
					return 0;
				case ArcScaleLabelOrientation.TopToBottom:
					return 90;
				case ArcScaleLabelOrientation.BottomToTop:
					return 270;
				case ArcScaleLabelOrientation.Radial:
					return angle > 90 && angle <= 270 ? angle + 180 : angle;
				case ArcScaleLabelOrientation.Tangent:
					return angle >= 180 && angle <= 360 || angle == 0 ? angle + 90 : angle - 90;
				default:
					DebugHelper.Fail("Unknown the scale label orientation.");
					goto case ArcScaleLabelOrientation.Radial;
			}
		}
	}
	public class LinearScaleLabelOptions : ScaleLabelOptions {
		public static readonly DependencyProperty OrientationProperty = DependencyPropertyManager.Register("Orientation",
			typeof(LinearScaleLabelOrientation), typeof(LinearScaleLabelOptions), new PropertyMetadata(LinearScaleLabelOrientation.LeftToRight, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleLabelOptionsOrientation"),
#endif
		Category(Categories.Layout)
		]
		public LinearScaleLabelOrientation Orientation {
			get { return (LinearScaleLabelOrientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ArcScaleLabelOptions();
		}
		protected internal override double CorrectAngleByOrientation(double angle) {
			double normalizedAngle = MathUtils.NormalizeDegree(angle);
			switch (Orientation) {
				case LinearScaleLabelOrientation.LeftToRight:
				return 0;
				case LinearScaleLabelOrientation.TopToBottom:
				return 90;
				case LinearScaleLabelOrientation.BottomToTop:
				return 270;
				default:
				DebugHelper.Fail("Unknown the scale label orientation.");
				goto case LinearScaleLabelOrientation.LeftToRight;
			}
		}
	}
	[NonCategorized]
	public class ScaleLabelInfo : ScaleElementInfoBase {
		readonly MajorTickmarkInfo tickmark;
		string text;
		internal MajorTickmarkInfo Tickmark { get { return tickmark; } }
		public string Text {
			get { return text; }
			set {
				if (text != value) {
					text = value;
					RaisePropertyChanged("Text");
				}
			}
		}
		internal ScaleLabelInfo(PresentationControl presentationControl, PresentationBase presentation, MajorTickmarkInfo tickmark, string text)
			: base(presentationControl, presentation) {
			Text = text;
			this.tickmark = tickmark;
			if (presentationControl != null)
				presentationControl.DataContext = this;
		}
	}
}
