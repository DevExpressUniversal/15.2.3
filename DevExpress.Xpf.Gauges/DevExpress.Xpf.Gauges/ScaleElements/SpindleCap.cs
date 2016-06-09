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
using System.Windows.Media;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public class SpindleCapOptions : GaugeDependencyObject {
		public static readonly DependencyProperty FactorWidthProperty = DependencyPropertyManager.Register("FactorWidth",
			typeof(double), typeof(SpindleCapOptions), new PropertyMetadata(1.0, NotifyPropertyChanged));
		public static readonly DependencyProperty FactorHeightProperty = DependencyPropertyManager.Register("FactorHeight",
			typeof(double), typeof(SpindleCapOptions), new PropertyMetadata(1.0, NotifyPropertyChanged));
		public static readonly DependencyProperty ZIndexProperty = DependencyPropertyManager.Register("ZIndex",
		   typeof(int), typeof(SpindleCapOptions), new PropertyMetadata(150, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("SpindleCapOptionsFactorWidth"),
#endif
		Category(Categories.Layout)
		]
		public double FactorWidth {
			get { return (double)GetValue(FactorWidthProperty); }
			set { SetValue(FactorWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("SpindleCapOptionsFactorHeight"),
#endif
		Category(Categories.Layout)
		]
		public double FactorHeight {
			get { return (double)GetValue(FactorHeightProperty); }
			set { SetValue(FactorHeightProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("SpindleCapOptionsZIndex"),
#endif
		Category(Categories.Layout)
		]
		public int ZIndex {
			get { return (int)GetValue(ZIndexProperty); }
			set { SetValue(ZIndexProperty, value); }
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SpindleCapOptions();
		}
	}
}
namespace DevExpress.Xpf.Gauges.Native {
	public class SpindleCap : ILayoutCalculator {
		readonly ArcScale scale;
		readonly LayerInfo info;
		SpindleCapPresentation Presentation { get { return scale.ActualSpindleCapPresentation; } }
		int ZIndex { get { return Options.ZIndex; } }
		SpindleCapOptions Options { get { return scale.ActualSpindleCapOptions; } }
		public LayerInfo ElementInfo { get { return info; } }
		public SpindleCap(ArcScale scale) {
			this.scale = scale;
			info = new LayerInfo(this, ZIndex, Presentation.CreateLayerPresentationControl(), Presentation);
		}
		#region ILayoutCalculator implementation
		ElementLayout ILayoutCalculator.CreateLayout(Size constraint) {
			if(scale.ActualShowSpindleCap && scale.Mapping != null && !scale.Mapping.Layout.IsEmpty)
				return new ElementLayout();
			else
				return null;
		}
		void ILayoutCalculator.CompleteLayout(ElementInfoBase elementInfo) {
			Point arrangePoint = scale.Mapping.Layout.EllipseCenter;
			Point offset = scale.GetLayoutOffset();
			arrangePoint.X += offset.X;
			arrangePoint.Y += offset.Y;
			ScaleTransform transform = new ScaleTransform() { ScaleX = Options.FactorWidth, ScaleY = Options.FactorHeight };
			elementInfo.Layout.CompleteLayout(arrangePoint, transform, null);
		}
		#endregion
		public void UpdateModel() {
			info.Presentation = Presentation;
			info.PresentationControl = Presentation.CreateLayerPresentationControl();
			info.ZIndex = ZIndex;
		}
	}
}
