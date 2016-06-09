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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Diagram.Core.Themes {
	public class DiagramEffect {
		readonly DiagramItemLineSettings lineSettingsCore;
		readonly Func<EffectContext, DiagramItemBrush> getItemBrush;
		readonly DiagramFontEffects fontEffectsCore;
		public DiagramItemLineSettings LineSettings { get { return lineSettingsCore; } }
		public DiagramFontEffects FontEffects { get { return fontEffectsCore; } }
		public DiagramEffect(DiagramItemLineSettings lineSettings, Func<EffectContext, DiagramItemBrush> getItemBrush, DiagramFontEffects fontEffects) {
			this.lineSettingsCore = lineSettings;
			this.getItemBrush = getItemBrush;
			this.fontEffectsCore = fontEffects;
		}
		public DiagramItemBrush GetItemBrush(EffectContext context) {
			return getItemBrush(context);
		}
	}
	public class DiagramEffectCollection {
		readonly ReadOnlyCollection<DiagramEffect> themeEffects;
		readonly ReadOnlyCollection<DiagramEffect> variantEffects;
		readonly ReadOnlyCollection<DiagramEffect> connectorEffects;
		public ReadOnlyCollection<DiagramEffect> ThemeEffects { get { return themeEffects; } }
		public ReadOnlyCollection<DiagramEffect> VariantEffects { get { return variantEffects; } }
		public ReadOnlyCollection<DiagramEffect> ConnectorEffects { get { return connectorEffects; } }
		public DiagramEffectCollection(DiagramEffect[] variantEffects, DiagramEffect[] themeEffects, DiagramEffect[] connectorEffects) {
			this.variantEffects = new ReadOnlyCollection<DiagramEffect>(variantEffects);
			this.themeEffects = new ReadOnlyCollection<DiagramEffect>(themeEffects);
			this.connectorEffects = new ReadOnlyCollection<DiagramEffect>(connectorEffects);
		}
	}
	public class EffectContext {
		readonly Color accent;
		readonly Color light;
		readonly DiagramColorPalette palette;
		public Color Accent { get { return accent; } }
		public Color Light { get { return light; } }
		public Color Dark { get { return palette.Dark; } }
		public Color Accent1 { get { return palette.Accents[0]; } }
		public Color Accent2 { get { return palette.Accents[1]; } }
		public Color Accent3 { get { return palette.Accents[2]; } }
		public Color Accent4 { get { return palette.Accents[3]; } }
		public Color Accent5 { get { return palette.Accents[4]; } }
		public Color Accent6 { get { return palette.Accents[5]; } }
		public EffectContext(DiagramColorPalette palette, Color accent, Color light) {
			this.accent = accent;
			this.light = light;
			this.palette = palette;
		}
	}
	public class DiagramItemLineSettings {
		readonly double strokeThickness;
		readonly double[] strokeDashArray;
		public double StrokeThickness { get { return strokeThickness; } }
		public double[] StrokeDashArray { get { return strokeDashArray; } }
		public DiagramItemLineSettings(double strokeThickness, double[] strokeDashArray) {
			this.strokeThickness = strokeThickness;
			this.strokeDashArray = strokeDashArray;
		}
	}
}
