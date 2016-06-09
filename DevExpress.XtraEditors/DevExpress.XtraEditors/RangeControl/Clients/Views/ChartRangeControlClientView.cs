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
using System.Drawing;
using System.Drawing.Design;
using DevExpress.Sparkline;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.Native;
namespace DevExpress.XtraEditors {
	public abstract class ChartRangeControlClientView {
		internal static Color DefaultColor = Color.Empty;
		internal delegate void ChangedDelegate();
		readonly SparklineViewBase sparklineView;
		internal ChangedDelegate Changed { get; set; }
		protected internal SparklineViewBase SparklineView {
			get { return sparklineView; }
		}
		[
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.ChartRangeControlClientView.Color"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ChartRangeControlClientViewColor"),
#endif
		Editor(typeof(ChartRangeControlClientTemplatedColorTypeEditor), typeof(UITypeEditor)),
		TypeConverter(typeof(ChartRangeControlClientTemplatedColorTypeConverter))
		]
		public Color Color {
			get { return sparklineView.Color; }
			set {
				if (sparklineView.Color != value) {
					sparklineView.Color = value;
					RaiseChanged();
				}
			}
		}
		public ChartRangeControlClientView(SparklineViewBase sparklineView) {
			this.sparklineView = sparklineView;
			sparklineView.EndPointColor = Color.Transparent;
			sparklineView.HighlightEndPoint = false;
			sparklineView.HighlightMaxPoint = false;
			sparklineView.HighlightMinPoint = false;
			sparklineView.HighlightStartPoint = false;
			sparklineView.MaxPointColor = Color.Transparent;
			sparklineView.MinPointColor = Color.Transparent;
			sparklineView.NegativePointColor = Color.Transparent;
			sparklineView.StartPointColor = Color.Transparent;
			sparklineView.Color = DefaultColor;
		}
		#region ShouldSerialize & Reset
		protected internal virtual bool ShouldSerialize() {
			return ShouldSerializeColor();
		}
		bool ShouldSerializeColor() {
			return Color != DefaultColor;
		}
		void ResetColor() {
			Color = DefaultColor;
		}
		#endregion
		protected void RaiseChanged() {
			if (Changed != null)
				Changed();
		}
		protected abstract ChartRangeControlClientView CreateInstance();
		protected virtual void ApplyPaletteCore(ChartRangeControlClientPaletteEntry paletteEntry) {
			if (sparklineView.Color == DefaultColor)
				sparklineView.Color = paletteEntry.Color;
		}
		protected virtual void Assign(ChartRangeControlClientView clone) {
			clone.Color = Color;
		}
		internal ChartRangeControlClientView Clone() {
			ChartRangeControlClientView clone = CreateInstance();
			Assign(clone);
			return clone;
		}
		internal void ApplyPalette(ChartRangeControlClientPalette palette, int paletteIndex) {
			ApplyPaletteCore(palette.GetRepeatingEntry(paletteIndex));
		}
		public override string ToString() {
			return "(" + this.GetType().Name + ")";
		}
	}
}
