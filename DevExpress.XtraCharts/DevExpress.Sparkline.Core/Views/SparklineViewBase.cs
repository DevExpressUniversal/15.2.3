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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Sparkline.Core;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Sparkline {
	public interface ISparklineViewVisitor {
		void Visit(LineSparklineView view);
		void Visit(AreaSparklineView view);
		void Visit(BarSparklineView view);
		void Visit(WinLossSparklineView view);
	}
	public abstract class SparklineViewBase : IXtraSupportShouldSerialize {
		public static SparklineViewBase CreateView(SparklineViewType viewType) {
			switch (viewType) {
				case SparklineViewType.Line:
					return new LineSparklineView();
				case SparklineViewType.Area:
					return new AreaSparklineView();
				case SparklineViewType.Bar:
					return new BarSparklineView();
				case SparklineViewType.WinLoss:
					return new WinLossSparklineView();
				default:
					return null;
			}
		}
		const bool defaultHighlightMaxPoint = false;
		const bool defaultHighlightMinPoint = false;
		const bool defaultHighlightStartPoint = false;
		const bool defaultHighlightEndPoint = false;
		bool highlightMaxPoint = defaultHighlightMaxPoint;
		bool highlightMinPoint = defaultHighlightMinPoint;
		bool highlightStartPoint = defaultHighlightStartPoint;
		bool highlightEndPoint = defaultHighlightEndPoint;
		Color color;
		Color maxPointColor;
		Color minPointColor;
		Color startPointColor;
		Color endPointColor;
		Color negativePointColor;
		ISparklineAppearanceProvider appearanceProvider;
		[Browsable(false)]
		public event EventHandler PropertiesChanged;
		protected internal ISparklineAppearanceProvider AppearanceProvider { get { return appearanceProvider; } set { appearanceProvider = value; } }
		[Browsable(false)]
		public Color ActualColor { get { return color.IsEmpty ? AppearanceProvider != null ? AppearanceProvider.Color : Color.Empty : color; } }
		[Browsable(false)]
		public Color ActualMaxPointColor { get { return maxPointColor.IsEmpty ? AppearanceProvider != null ? AppearanceProvider.MaxPointColor : Color.Empty : maxPointColor; } }
		[Browsable(false)]
		public Color ActualMinPointColor { get { return minPointColor.IsEmpty ? AppearanceProvider != null ? AppearanceProvider.MinPointColor : Color.Empty : minPointColor; } }
		[Browsable(false)]
		public Color ActualStartPointColor { get { return startPointColor.IsEmpty ? AppearanceProvider != null ? AppearanceProvider.StartPointColor : Color.Empty : startPointColor; } }
		[Browsable(false)]
		public Color ActualEndPointColor { get { return endPointColor.IsEmpty ? AppearanceProvider != null ? AppearanceProvider.EndPointColor : Color.Empty : endPointColor; } }
		[Browsable(false)]
		public Color ActualNegativePointColor { get { return negativePointColor.IsEmpty ? AppearanceProvider != null ? AppearanceProvider.NegativePointColor : Color.Empty : negativePointColor; } }
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("SparklineViewBaseHighlightMaxPoint"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.SparklineViewBase.HighlightMaxPoint"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool HighlightMaxPoint {
			get { return highlightMaxPoint; }
			set {
				if (highlightMaxPoint != value) {
					highlightMaxPoint = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("SparklineViewBaseHighlightMinPoint"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.SparklineViewBase.HighlightMinPoint"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool HighlightMinPoint {
			get { return highlightMinPoint; }
			set {
				if (highlightMinPoint != value) {
					highlightMinPoint = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("SparklineViewBaseHighlightStartPoint"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.SparklineViewBase.HighlightStartPoint"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool HighlightStartPoint {
			get { return highlightStartPoint; }
			set {
				if (highlightStartPoint != value) {
					highlightStartPoint = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("SparklineViewBaseHighlightEndPoint"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.SparklineViewBase.HighlightEndPoint"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool HighlightEndPoint {
			get { return highlightEndPoint; }
			set {
				if (highlightEndPoint != value) {
					highlightEndPoint = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("SparklineViewBaseColor"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.SparklineViewBase.Color")
		]
		public Color Color {
			get { return color; }
			set {
				if (color != value) {
					color = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("SparklineViewBaseMaxPointColor"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.SparklineViewBase.MaxPointColor")
		]
		public Color MaxPointColor {
			get { return maxPointColor; }
			set {
				if (maxPointColor != value) {
					maxPointColor = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("SparklineViewBaseMinPointColor"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.SparklineViewBase.MinPointColor")
		]
		public Color MinPointColor {
			get { return minPointColor; }
			set {
				if (minPointColor != value) {
					minPointColor = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("SparklineViewBaseStartPointColor"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.SparklineViewBase.StartPointColor")
		]
		public Color StartPointColor {
			get { return startPointColor; }
			set {
				if (startPointColor != value) {
					startPointColor = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("SparklineViewBaseEndPointColor"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.SparklineViewBase.EndPointColor")
		]
		public Color EndPointColor {
			get { return endPointColor; }
			set {
				if (endPointColor != value) {
					endPointColor = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("SparklineViewBaseNegativePointColor"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.SparklineViewBase.NegativePointColor")
		]
		public Color NegativePointColor {
			get { return negativePointColor; }
			set {
				if (negativePointColor != value) {
					negativePointColor = value;
					OnPropertiesChanged();
				}
			}
		}
		protected internal abstract bool ActualShowNegativePoint { get; }
		[XtraSerializableProperty, Browsable(false)]
		public abstract SparklineViewType Type { get; }
		protected SparklineViewBase() {
		}
		#region ShouldSerialize
		bool ShouldSerializeHighlightMaxPoint() {
			return highlightMaxPoint != defaultHighlightMaxPoint;
		}
		bool ShouldSerializeHighlightMinPoint() {
			return highlightMinPoint != defaultHighlightMinPoint;
		}
		bool ShouldSerializeHighlightStartPoint() {
			return highlightStartPoint != defaultHighlightStartPoint;
		}
		bool ShouldSerializeHighlightEndPoint() {
			return highlightEndPoint != defaultHighlightEndPoint;
		}
		bool ShouldSerializeColor() {
			return !color.IsEmpty;
		}
		bool ShouldSerializeMaxPointColor() {
			return !maxPointColor.IsEmpty;
		}
		bool ShouldSerializeMinPointColor() {
			return !minPointColor.IsEmpty;
		}
		bool ShouldSerializeStartPointColor() {
			return !startPointColor.IsEmpty;
		}
		bool ShouldSerializeEndPointColor() {
			return !endPointColor.IsEmpty;
		}
		bool ShouldSerializeNegativePointColor() {
			return !negativePointColor.IsEmpty;
		}
		protected virtual bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Type":
					return true;
				case "HighlightMaxPoint":
					return ShouldSerializeHighlightMaxPoint();
				case "HighlightMinPoint":
					return ShouldSerializeHighlightMinPoint();
				case "HighlightStartPoint":
					return ShouldSerializeHighlightStartPoint();
				case "HighlightEndPoint":
					return ShouldSerializeHighlightEndPoint();
				case "Color":
					return ShouldSerializeColor();
				case "MaxPointColor":
					return ShouldSerializeMaxPointColor();
				case "MinPointColor":
					return ShouldSerializeMinPointColor();
				case "StartPointColor":
					return ShouldSerializeStartPointColor();
				case "EndPointColor":
					return ShouldSerializeEndPointColor();
				case "NegativePointColor":
					return ShouldSerializeNegativePointColor();
				default:
					return false;
			}
		}
		#endregion
		#region Reset
		void ResetHighlightMaxPoint() {
			highlightMaxPoint = defaultHighlightMaxPoint;
		}
		void ResetHighlightMinPoint() {
			highlightMinPoint = defaultHighlightMinPoint;
		}
		void ResetHighlightStartPoint() {
			highlightStartPoint = defaultHighlightStartPoint;
		}
		void ResetHighlightEndPoint() {
			highlightEndPoint = defaultHighlightEndPoint;
		}
		void ResetColor() {
			color = Color.Empty;
		}
		void ResetMaxPointColor() {
			maxPointColor = Color.Empty;
		}
		void ResetMinPointColor() {
			minPointColor = Color.Empty;
		}
		void ResetStartPointColor() {
			startPointColor = Color.Empty;
		}
		void ResetEndPointColor() {
			endPointColor = Color.Empty;
		}
		void ResetNegativePointColor() {
			negativePointColor = Color.Empty;
		}
		#endregion
		#region IXtraSupportShouldSerialize Members
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			return XtraShouldSerialize(propertyName);
		}
		#endregion
		protected void OnPropertiesChanged() {
			if (PropertiesChanged != null)
				PropertiesChanged(this, EventArgs.Empty);
		}
		protected abstract string GetViewName();
		protected internal virtual BaseSparklinePainter CreatePainter() {
			return SparklineViewPainterFactory.Create(this);
		}
		public virtual void Assign(SparklineViewBase view) {
			if (view != null) {
				highlightMaxPoint = view.highlightMaxPoint;
				highlightMinPoint = view.highlightMinPoint;
				highlightStartPoint = view.highlightStartPoint;
				highlightEndPoint = view.highlightEndPoint;
				color = view.color;
				maxPointColor = view.maxPointColor;
				minPointColor = view.minPointColor;
				startPointColor = view.startPointColor;
				endPointColor = view.endPointColor;
				negativePointColor = view.negativePointColor;
				appearanceProvider = view.appearanceProvider;
			}
		}
		public override string ToString() {
			return GetViewName();
		}
		public abstract void Visit(ISparklineViewVisitor visitor);
	}
}
