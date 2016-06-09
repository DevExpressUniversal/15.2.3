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
using System.Collections.Generic;
namespace DevExpress.Xpf.Charts {
	public abstract class PredefinedElementKind {
		readonly Type type;
		readonly string name;
		public Type Type { get { return type; } }
		public string Name { get { return name; } }
		protected PredefinedElementKind(Type type, string name) {
			this.type = type;
			this.name = name;
		}
		public override string ToString() {
			return name;
		}
	}
	public class PaletteKind : PredefinedElementKind {
		internal static readonly List<PaletteKind> List = new List<PaletteKind>();
		static PaletteKind() {
			Add(typeof(NatureColorsPalette));
			Add(typeof(PastelKitPalette));
			Add(typeof(InAFogPalette));
			Add(typeof(TerracottaPiePalette));
			Add(typeof(NorthernLightsPalette));
			Add(typeof(ChameleonPalette));
			Add(typeof(TheTreesPalette));
			Add(typeof(OfficePalette));
			Add(typeof(DXChartsPalette));
			Add(typeof(Office2013Palette));
			Add(typeof(BlueWarmPalette));
			Add(typeof(BluePalette));
			Add(typeof(BlueIIPalette));
			Add(typeof(BlueGreenPalette));
			Add(typeof(GreenPalette));
			Add(typeof(GreenYellowPalette));
			Add(typeof(YellowPalette));
			Add(typeof(YellowOrangePalette));
			Add(typeof(OrangePalette));
			Add(typeof(OrangeRedPalette));
			Add(typeof(RedOrangePalette));
			Add(typeof(RedPalette));
			Add(typeof(RedVioletPalette));
			Add(typeof(VioletPalette));
			Add(typeof(VioletIIPalette));
			Add(typeof(MarqueePalette));
			Add(typeof(SlipstreamPalette));
		}
		static void Add(Type type) {
			List.Add(new PaletteKind(type, ((Palette)Activator.CreateInstance(type)).PaletteName));
		}
		public PaletteKind(Type type, string name) : base(type, name) { 
		}
	}
	public class IndicatorsPaletteKind : PredefinedElementKind {
		internal static readonly List<IndicatorsPaletteKind> List = new List<IndicatorsPaletteKind>();
		static IndicatorsPaletteKind() {
			Add(typeof(DefaultIndicatorsPalette));
		}
		static void Add(Type type) {
			List.Add(new IndicatorsPaletteKind(type, ((IndicatorsPalette)Activator.CreateInstance(type)).PaletteName));
		}
		public IndicatorsPaletteKind(Type type, string name)
			: base(type, name) {
		}
	}
	public class Bar2DKind : PredefinedElementKind {
		internal static readonly List<Bar2DKind> List = new List<Bar2DKind>();
		static Bar2DKind() {
			Add(typeof(OutsetBar2DModel));
			Add(typeof(GradientBar2DModel));
			Add(typeof(BorderlessGradientBar2DModel));
			Add(typeof(SimpleBar2DModel));
			Add(typeof(BorderlessSimpleBar2DModel));
			Add(typeof(FlatBar2DModel));
			Add(typeof(FlatGlassBar2DModel));
			Add(typeof(SteelColumnBar2DModel));
			Add(typeof(TransparentBar2DModel));
			Add(typeof(Quasi3DBar2DModel));
			Add(typeof(GlassCylinderBar2DModel));
		}
		static void Add(Type type) {
			List.Add(new Bar2DKind(type, ((Bar2DModel)Activator.CreateInstance(type)).ModelName));
		}
		public Bar2DKind(Type type, string name) : base(type, name) { 
		}
	}
	public class RangeBar2DKind : PredefinedElementKind {
		internal static readonly List<RangeBar2DKind> List = new List<RangeBar2DKind>();
		static RangeBar2DKind() {
			Add(typeof(OutsetRangeBar2DModel));
			Add(typeof(GradientRangeBar2DModel));
			Add(typeof(BorderlessGradientRangeBar2DModel));
			Add(typeof(SimpleRangeBar2DModel));
			Add(typeof(BorderlessSimpleRangeBar2DModel));
			Add(typeof(FlatRangeBar2DModel));
			Add(typeof(FlatGlassRangeBar2DModel));
			Add(typeof(TransparentRangeBar2DModel));
		}
		static void Add(Type type) {
			List.Add(new RangeBar2DKind(type, ((RangeBar2DModel)Activator.CreateInstance(type)).ModelName));
		}
		public RangeBar2DKind(Type type, string name) : base(type, name) { 
		}
	}
	public class CandleStick2DKind : PredefinedElementKind {
		internal static readonly List<CandleStick2DKind> List = new List<CandleStick2DKind>();
		static CandleStick2DKind() {
			Add(typeof(SimpleCandleStick2DModel));
			Add(typeof(BorderCandleStick2DModel));
			Add(typeof(ThinCandleStick2DModel));
			Add(typeof(FlatCandleStick2DModel));
			Add(typeof(GlassCandleStick2DModel));
		}
		static void Add(Type type) {
			List.Add(new CandleStick2DKind(type, ((CandleStick2DModel)Activator.CreateInstance(type)).ModelName));
		}
		public CandleStick2DKind(Type type, string name) : base(type, name) { 
		}
	}
	public class Stock2DKind : PredefinedElementKind {
		internal static readonly List<Stock2DKind> List = new List<Stock2DKind>();
		static Stock2DKind() {
			Add(typeof(ThinStock2DModel));
			Add(typeof(FlatStock2DModel));
			Add(typeof(ArrowsStock2DModel));
			Add(typeof(DropsStock2DModel));
		}
		static void Add(Type type) {
			List.Add(new Stock2DKind(type, ((Stock2DModel)Activator.CreateInstance(type)).ModelName));
		}
		public Stock2DKind(Type type, string name) : base(type, name) { 
		}
	}
	public class Marker2DKind : PredefinedElementKind {
		internal static readonly List<Marker2DKind> List = new List<Marker2DKind>();
		static Marker2DKind() {
			Add(typeof(CircleMarker2DModel));
			Add(typeof(CrossMarker2DModel));
			Add(typeof(DollarMarker2DModel));
			Add(typeof(PolygonMarker2DModel));
			Add(typeof(RingMarker2DModel));
			Add(typeof(SquareMarker2DModel));
			Add(typeof(StarMarker2DModel));
			Add(typeof(TriangleMarker2DModel));
		}
		static void Add(Type type) {
			List.Add(new Marker2DKind(type, ((Marker2DModel)Activator.CreateInstance(type)).ModelName));
		}
		public Marker2DKind(Type type, string name) : base(type, name) { 
		}
	}
	public class Pie2DKind : PredefinedElementKind {
		internal static readonly List<Pie2DKind> List = new List<Pie2DKind>();
		static Pie2DKind() {
			Add(typeof(SimplePie2DModel));
			Add(typeof(FlatPie2DModel));
			Add(typeof(BorderlessFlatPie2DModel));
			Add(typeof(GlarePie2DModel));
			Add(typeof(GlassPie2DModel));
			Add(typeof(GlossyPie2DModel));
			Add(typeof(CupidPie2DModel));
		}
		static void Add(Type type) {
			List.Add(new Pie2DKind(type, ((Pie2DModel)Activator.CreateInstance(type)).ModelName));
		}
		public Pie2DKind(Type type, string name) : base(type, name) { 
		}
	}
	public class AnimationKind : PredefinedElementKind {
		public AnimationKind(Type type, string name) : base(type, name) { 
		}
		internal AnimationKind(Type type) : this(type, ((AnimationBase)Activator.CreateInstance(type)).AnimationName) { 
		}
	}
	public class Bar3DKind : PredefinedElementKind {
		internal static readonly List<Bar3DKind> List = new List<Bar3DKind>();
		static Bar3DKind() {
			Add(typeof(BoxBar3DModel));
			Add(typeof(ConeBar3DModel));
			Add(typeof(CylinderBar3DModel));
			Add(typeof(HexagonBar3DModel));
			Add(typeof(PyramidBar3DModel));
		}
		static void Add(Type type) {
			List.Add(new Bar3DKind(type, ((Bar3DModel)Activator.CreateInstance(type)).ModelName));
		}
		public Bar3DKind(Type type, string name) : base(type, name) { 
		}
	}
	public class Marker3DKind : PredefinedElementKind {
		internal static readonly List<Marker3DKind> List = new List<Marker3DKind>();
		static Marker3DKind() {
			Add(typeof(CapsuleMarker3DModel));
			Add(typeof(ConeMarker3DModel));
			Add(typeof(CubeMarker3DModel));
			Add(typeof(CylinderMarker3DModel));
			Add(typeof(HexagonMarker3DModel));
			Add(typeof(PyramidMarker3DModel));
			Add(typeof(RoundedCubeMarker3DModel));
			Add(typeof(SphereMarker3DModel));
			Add(typeof(StarMarker3DModel));
		}
		static void Add(Type type) {
			List.Add(new Marker3DKind(type, ((Marker3DModel)Activator.CreateInstance(type)).ModelName));
		}
		public Marker3DKind(Type type, string name) : base(type, name) { 
		}
	}
	public class Pie3DKind : PredefinedElementKind {
		internal static readonly List<Pie3DKind> List = new List<Pie3DKind>();
		static Pie3DKind() {
			Add(typeof(CirclePie3DModel));
			Add(typeof(RectanglePie3DModel));
			Add(typeof(PentagonPie3DModel));
			Add(typeof(HexagonPie3DModel));
			Add(typeof(RoundedRectanglePie3DModel));
			Add(typeof(SemiCirclePie3DModel));
			Add(typeof(SemiRectanglePie3DModel));
			Add(typeof(SemiPentagonPie3DModel));
			Add(typeof(SemiHexagonPie3DModel));
			Add(typeof(SemiRoundedRectanglePie3DModel));
		}
		static void Add(Type type) {
			List.Add(new Pie3DKind(type, ((Pie3DModel)Activator.CreateInstance(type)).ModelName));
		}
		public Pie3DKind(Type type, string name) : base(type, name) { 
		}
	}
}
