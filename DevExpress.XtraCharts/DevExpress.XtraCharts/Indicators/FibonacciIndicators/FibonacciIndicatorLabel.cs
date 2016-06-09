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

using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class FibonacciIndicatorLabel : TitleBase, ITextPropertiesProvider {
		static readonly Color DefaultBaseLevelColor = Color.Empty;
		Color baseLevelTextColor = DefaultBaseLevelColor;
		protected override bool DefaultAntialiasing { get { return false; } }
		protected override bool DefaultVisible { get { return true; } }
		protected override bool Rotated { get { return ((FibonacciIndicator)Owner).Kind == FibonacciIndicatorKind.FibonacciFans; } }
		protected override Font DefaultFont { get { return DefaultFonts.Tahoma8; } }
		internal FibonacciIndicator FibonacciIndicator { get { return (FibonacciIndicator)Owner; } }
		internal Color ActualBaseLevelTextColor { 
			get { 
				return baseLevelTextColor.IsEmpty ? 
					((FibonacciIndicatorBehavior)FibonacciIndicator.IndicatorBehavior).BaseLevelColor : baseLevelTextColor; 
			} 
		}
		protected internal override Color ActualTextColor { 
			get { 
				Color textColor = TextColor;
				return textColor.IsEmpty ? FibonacciIndicator.IndicatorBehavior.Color : textColor; 
			} 
		}
		protected internal override ChartElement BackElement { get { return null; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FibonacciIndicatorLabelBaseLevelTextColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FibonacciIndicatorLabel.BaseLevelTextColor"),
		Category("Appearance"),
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All)
		]
		public Color BaseLevelTextColor {
			get { return baseLevelTextColor; }
			set {
				if (value != baseLevelTextColor) {
					SendNotification(new ElementWillChangeNotification(this));
					baseLevelTextColor = value;
					RaiseControlChanged();
				}
			}
		}
		internal FibonacciIndicatorLabel(FibonacciIndicator fibonacciIndicator) : base(fibonacciIndicator) {
		}
		#region IHitTest implementation
		object IHitTest.Object { get { return ((IHitTest)FibonacciIndicator).Object; } }
		HitTestState IHitTest.State { get { return ((IHitTest)FibonacciIndicator).State; } }
		#endregion
		#region ITextPropertiesProvider implementation
		RectangleFillStyle ITextPropertiesProvider.FillStyle { get { return RectangleFillStyle.Empty; } }
		RectangularBorder ITextPropertiesProvider.Border { get { return null; } }
		Shadow ITextPropertiesProvider.Shadow { get { return null; } }
		StringAlignment ITextPropertiesProvider.Alignment { get { return StringFormat.GenericDefault.Alignment; } }
		bool ITextPropertiesProvider.ChangeSelectedBorder { get { return true; } }
		bool ITextPropertiesProvider.CorrectBoundsByBorder { get { return false; } }
		Color ITextPropertiesProvider.BackColor { get { return Color.Empty; } }
		Color ITextPropertiesProvider.GetTextColor(Color color) { return color; }
		Color ITextPropertiesProvider.GetBorderColor(Color color) { return Color.Empty; }
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeBaseLevelTextColor() {
			return baseLevelTextColor != DefaultBaseLevelColor;
		}
		void ResetBaseLevelTextColor() {
			BaseLevelTextColor = DefaultBaseLevelColor;
		}
		protected internal override bool ShouldSerialize() {
			return ShouldSerializeBaseLevelTextColor() || base.ShouldSerialize();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "BaseLevelTextColor" ? ShouldSerializeBaseLevelTextColor() : base.XtraShouldSerialize(propertyName);
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new FibonacciIndicatorLabel(null);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			FibonacciIndicatorLabel label = obj as FibonacciIndicatorLabel;
			if (label != null)
				this.baseLevelTextColor = label.baseLevelTextColor;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class FibonacciIndicatorLabelsLayout {
		readonly FibonacciIndicatorLabel label;
		readonly IEnumerable<RotatedTextPainterNearCircleTangent> painters;
		readonly IEnumerable<RotatedTextPainterNearCircleTangent> baseLevelPainters;
		public FibonacciIndicatorLabelsLayout(FibonacciIndicatorLabel label, IEnumerable<RotatedTextPainterNearCircleTangent> painters, IEnumerable<RotatedTextPainterNearCircleTangent> baseLevelPainters) {
			this.label = label;
			this.painters = painters;
			this.baseLevelPainters = baseLevelPainters;
		}
		void Render(IRenderer renderer, IEnumerable<RotatedTextPainterNearCircleTangent> painters, Color color) {
			HitTestController hitTestController = label.FibonacciIndicator.HitTestController;
			foreach (RotatedTextPainterNearCircleTangent painter in painters)
				painter.Render(renderer, hitTestController, null, color);
		}
		public void Render(IRenderer renderer) {
			Color color = label.ActualTextColor;
			Color baseLevelColor = label.ActualBaseLevelTextColor;
			Render(renderer, painters, color);
			Render(renderer, baseLevelPainters, baseLevelColor);
		}		
	}
}
