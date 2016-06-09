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
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class PredefinedPalette : Palette {
		readonly ColorCollection predefinedColors = new ColorCollection();
		protected internal override ColorCollection ActualColors { get { return predefinedColors; } }
		PredefinedPalette() {
		}
		protected PredefinedPalette(Color[] colors) {
			foreach (Color color in colors)
				predefinedColors.Add(color);
		}
		protected override ChartDependencyObject CreateObject() {
			return new PredefinedPalette();
		}
	}
	public class ChameleonPalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChameleonPalettePaletteName")]
#endif
		public override string PaletteName { get { return "Chameleon"; } }
		public ChameleonPalette()
			: base(new Color[] {
			Color.FromArgb(0xFF, 0x00, 0x69, 0xBF),
			Color.FromArgb(0xFF, 0xA5, 0x8B, 0x47),
			Color.FromArgb(0xFF, 0xA0, 0x3C, 0x7F),
			Color.FromArgb(0xFF, 0x70, 0x9E, 0x25),
			Color.FromArgb(0xFF, 0x3E, 0x92, 0xD8),
			Color.FromArgb(0xFF, 0xC2, 0xAB, 0x69),
			Color.FromArgb(0xFF, 0xC3, 0x70, 0xA8),
			Color.FromArgb(0xFF, 0x95, 0xBB, 0x5D),
			Color.FromArgb(0xFF, 0x8E, 0xC6, 0xEF),
			Color.FromArgb(0xFF, 0xE0, 0xD1, 0xAB),
			Color.FromArgb(0xFF, 0xDE, 0xB0, 0xCF),
			Color.FromArgb(0xFF, 0xC2, 0xD8, 0x9D) }) {
		}
	}
	public class DXChartsPalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("DXChartsPalettePaletteName")]
#endif
		public override string PaletteName { get { return "DX Charts"; } }
		public DXChartsPalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0x47, 0xB2, 0x1C),
			Color.FromArgb(0xFF, 0x1C, 0x52, 0xB2),
			Color.FromArgb(0xFF, 0xB2, 0x1B, 0x1C),
			Color.FromArgb(0xFF, 0x98, 0xB2, 0x1C),
			Color.FromArgb(0xFF, 0x1C, 0xB2, 0x69),
			Color.FromArgb(0xFF, 0xB2, 0x1C, 0x6F),
			Color.FromArgb(0xFF, 0xB2, 0x76, 0x1C),
			Color.FromArgb(0xFF, 0xC2, 0x1C, 0xCC) }) {
		}
	}
	public class InAFogPalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("InAFogPalettePaletteName")]
#endif
		public override string PaletteName { get { return "In a Fog"; } }
		public InAFogPalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0x8E, 0xB3, 0xE5),
			Color.FromArgb(0xFF, 0xE5, 0x8E, 0x99),
			Color.FromArgb(0xFF, 0xC8, 0x92, 0xD4),
			Color.FromArgb(0xFF, 0x9A, 0x93, 0xDD),
			Color.FromArgb(0xFF, 0xE1, 0xB1, 0x87),
			Color.FromArgb(0xFF, 0xD9, 0xD3, 0x83),
			Color.FromArgb(0xFF, 0xA1, 0xDB, 0x89),
			Color.FromArgb(0xFF, 0x8A, 0xD2, 0xCC),
			Color.FromArgb(0xFF, 0x8A, 0xB6, 0xD2),
			Color.FromArgb(0xFF, 0xB3, 0x97, 0x85),
			Color.FromArgb(0xFF, 0xDE, 0x77, 0x85),
			Color.FromArgb(0xFF, 0x9C, 0xCD, 0x8B) }) {
		}
	}
	public class NatureColorsPalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("NatureColorsPalettePaletteName")]
#endif
		public override string PaletteName { get { return "Nature Colors"; } }
		public NatureColorsPalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0xCE, 0x92, 0x1E),
			Color.FromArgb(0xFF, 0x66, 0xA8, 0x09),
			Color.FromArgb(0xFF, 0xC8, 0x4C, 0x1B),
			Color.FromArgb(0xFF, 0x3A, 0xAD, 0xC8),
			Color.FromArgb(0xFF, 0x77, 0xC7, 0x46),
			Color.FromArgb(0xFF, 0x98, 0x3A, 0x17),
			Color.FromArgb(0xFF, 0x00, 0x88, 0x00),
			Color.FromArgb(0xFF, 0x9E, 0xA1, 0x00),
			Color.FromArgb(0xFF, 0x00, 0x9F, 0x71),
			Color.FromArgb(0xFF, 0xCE, 0xAC, 0x68),
			Color.FromArgb(0xFF, 0x73, 0xC0, 0xC0),
			Color.FromArgb(0xFF, 0xEA, 0x93, 0x73) }) {
		}
	}
	public class NorthernLightsPalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("NorthernLightsPalettePaletteName")]
#endif
		public override string PaletteName { get { return "Northern Lights"; } }
		public NorthernLightsPalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0x16, 0x57, 0xB8),
			Color.FromArgb(0xFF, 0xB4, 0x16, 0x84),
			Color.FromArgb(0xFF, 0x22, 0xA4, 0xF0),
			Color.FromArgb(0xFF, 0x9D, 0x0C, 0xAA),
			Color.FromArgb(0xFF, 0x69, 0x96, 0xD8),
			Color.FromArgb(0xFF, 0xD6, 0x6D, 0xB6),
			Color.FromArgb(0xFF, 0x75, 0xC1, 0xEE),
			Color.FromArgb(0xFF, 0xC6, 0x6D, 0xCE),
			Color.FromArgb(0xFF, 0x98, 0xBA, 0xED),
			Color.FromArgb(0xFF, 0xED, 0x9B, 0xD4),
			Color.FromArgb(0xFF, 0xA4, 0xD5, 0xF1),
			Color.FromArgb(0xFF, 0xDF, 0x9C, 0xE7) }) {
		}
	}
	public class OfficePalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("OfficePalettePaletteName")]
#endif
		public override string PaletteName { get { return "Office"; } }
		public OfficePalette()
			: base(new Color[] {
			Color.FromArgb(0xFF, 0x4F, 0x81, 0xBD),
			Color.FromArgb(0xFF, 0xC0, 0x50, 0x4D),
			Color.FromArgb(0xFF, 0x9B, 0xBB, 0x59),
			Color.FromArgb(0xFF, 0x80, 0x64, 0xA2),
			Color.FromArgb(0xFF, 0x4B, 0xAC, 0xC6),
			Color.FromArgb(0xFF, 0xF7, 0x96, 0x46) }) {
		}
	}
	public class PastelKitPalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("PastelKitPalettePaletteName")]
#endif
		public override string PaletteName { get { return "Pastel Kit"; } }
		public PastelKitPalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0xDC, 0x63, 0x4E),
			Color.FromArgb(0xFF, 0x2A, 0x84, 0xBE),
			Color.FromArgb(0xFF, 0xEE, 0xBD, 0x3A),
			Color.FromArgb(0xFF, 0x85, 0xCE, 0x59),
			Color.FromArgb(0xFF, 0x8F, 0xCC, 0xF4),
			Color.FromArgb(0xFF, 0xEA, 0x84, 0x41),
			Color.FromArgb(0xFF, 0x91, 0x71, 0xB8),
			Color.FromArgb(0xFF, 0xAC, 0xAC, 0xAC),
			Color.FromArgb(0xFF, 0x52, 0xBD, 0xDB),
			Color.FromArgb(0xFF, 0x2F, 0xD7, 0xA1),
			Color.FromArgb(0xFF, 0xC6, 0xAA, 0x7E),
			Color.FromArgb(0xFF, 0xCD, 0x83, 0xC6) }) {
		}
	}
	public class TerracottaPiePalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("TerracottaPiePalettePaletteName")]
#endif
		public override string PaletteName { get { return "Terracotta Pie"; } }
		public TerracottaPiePalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0xCD, 0x56, 0x33),
			Color.FromArgb(0xFF, 0xA2, 0x82, 0x44),
			Color.FromArgb(0xFF, 0xC2, 0x68, 0x0C),
			Color.FromArgb(0xFF, 0xF1, 0xA5, 0x8A),
			Color.FromArgb(0xFF, 0xDE, 0xCB, 0x7F),
			Color.FromArgb(0xFF, 0xEC, 0xB5, 0x52),
			Color.FromArgb(0xFF, 0xDF, 0x8A, 0x71),
			Color.FromArgb(0xFF, 0xB8, 0x9C, 0x66),
			Color.FromArgb(0xFF, 0xD4, 0x8E, 0x44),
			Color.FromArgb(0xFF, 0xFF, 0x8E, 0x44),
			Color.FromArgb(0xFF, 0xF4, 0xE7, 0xB6),
			Color.FromArgb(0xFF, 0xFA, 0xDC, 0xA5) }) {
		}
	}
	public class TheTreesPalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("TheTreesPalettePaletteName")]
#endif
		public override string PaletteName { get { return "The Trees"; } }
		public TheTreesPalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0x61, 0xAF, 0x20),
			Color.FromArgb(0xFF, 0xA8, 0x64, 0x00),
			Color.FromArgb(0xFF, 0x24, 0xA8, 0x11),
			Color.FromArgb(0xFF, 0x7D, 0xC4, 0x00),
			Color.FromArgb(0xFF, 0x00, 0xA8, 0x49),
			Color.FromArgb(0xFF, 0xA2, 0x87, 0x49),
			Color.FromArgb(0xFF, 0x9D, 0x2A, 0x00),
			Color.FromArgb(0xFF, 0xA5, 0xCA, 0x55),
			Color.FromArgb(0xFF, 0x49, 0xBD, 0x7C),
			Color.FromArgb(0xFF, 0xBE, 0x93, 0x37),
			Color.FromArgb(0xFF, 0xD7, 0xC7, 0x35) }) {
		}
	}
	public class Office2013Palette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("Office2013PalettePaletteName")]
#endif
		public override string PaletteName { get { return "Office 2013"; } }
		public Office2013Palette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0x5B, 0x9B, 0xD5),
			Color.FromArgb(0xFF, 0xED, 0x7D, 0x31),
			Color.FromArgb(0xFF, 0xA5, 0xA5, 0xA5),
			Color.FromArgb(0xFF, 0xFF, 0xC0, 0x00),
			Color.FromArgb(0xFF, 0x44, 0x72, 0xC4),
			Color.FromArgb(0xFF, 0x70, 0xAD, 0x47),
			}) {
		}
	}
	public class BlueWarmPalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("BlueWarmPalettePaletteName")]
#endif
		public override string PaletteName { get { return "Blue Warm"; } }
		public BlueWarmPalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0x4A, 0x66, 0xAC),
			Color.FromArgb(0xFF, 0x62, 0x9D, 0xD1),
			Color.FromArgb(0xFF, 0x29, 0x7F, 0xD5),
			Color.FromArgb(0xFF, 0x7F, 0x8F, 0xA9),
			Color.FromArgb(0xFF, 0x5A, 0xA2, 0xAE),
			Color.FromArgb(0xFF, 0x9D, 0x90, 0xA0),
			}) {
		}
	}
	public class BluePalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("BluePalettePaletteName")]
#endif
		public override string PaletteName { get { return "Blue"; } }
		public BluePalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0x0F, 0x6F, 0xC6),
			Color.FromArgb(0xFF, 0x00, 0x9D, 0xD9),
			Color.FromArgb(0xFF, 0x0B, 0xD0, 0xD9),
			Color.FromArgb(0xFF, 0x10, 0xCF, 0x9B),
			Color.FromArgb(0xFF, 0x7C, 0xCA, 0x62),
			Color.FromArgb(0xFF, 0xA5, 0xC2, 0x49),
			}) {
		}
	}
	public class BlueIIPalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("BlueIIPalettePaletteName")]
#endif
		public override string PaletteName { get { return "Blue II"; } }
		public BlueIIPalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0x1C, 0xAD, 0xE4),
			Color.FromArgb(0xFF, 0x26, 0x83, 0xC6),
			Color.FromArgb(0xFF, 0x27, 0xCE, 0xD7),
			Color.FromArgb(0xFF, 0x42, 0xBA, 0x97),
			Color.FromArgb(0xFF, 0x3E, 0x88, 0x53),
			Color.FromArgb(0xFF, 0x62, 0xA3, 0x9F),
			}) {
		}
	}
	public class BlueGreenPalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("BlueGreenPalettePaletteName")]
#endif
		public override string PaletteName { get { return "Blue Green"; } }
		public BlueGreenPalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0x34, 0x94, 0xBA),
			Color.FromArgb(0xFF, 0x58, 0xB6, 0xC0),
			Color.FromArgb(0xFF, 0x75, 0xBD, 0xA7),
			Color.FromArgb(0xFF, 0x7A, 0x8C, 0x8E),
			Color.FromArgb(0xFF, 0x84, 0xAC, 0xB6),
			Color.FromArgb(0xFF, 0x26, 0x83, 0xC6),
			}) {
		}
	}
	public class GreenPalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("GreenPalettePaletteName")]
#endif
		public override string PaletteName { get { return "Green"; } }
		public GreenPalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0x54, 0x9E, 0x39),
			Color.FromArgb(0xFF, 0x8A, 0xB8, 0x33),
			Color.FromArgb(0xFF, 0xC0, 0xCF, 0x3A),
			Color.FromArgb(0xFF, 0x02, 0x96, 0x76),
			Color.FromArgb(0xFF, 0x4A, 0xB5, 0xC4),
			Color.FromArgb(0xFF, 0x09, 0x89, 0xB1),
			}) {
		}
	}
	public class GreenYellowPalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("GreenYellowPalettePaletteName")]
#endif
		public override string PaletteName { get { return "Green Yellow"; } }
		public GreenYellowPalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0x99, 0xCB, 0x38),
			Color.FromArgb(0xFF, 0x63, 0xA5, 0x37),
			Color.FromArgb(0xFF, 0x37, 0xA7, 0x6F),
			Color.FromArgb(0xFF, 0x44, 0xC1, 0xA3),
			Color.FromArgb(0xFF, 0x4E, 0xB3, 0xCF),
			Color.FromArgb(0xFF, 0x51, 0xC3, 0xF9),
			}) {
		}
	}
	public class YellowPalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("YellowPalettePaletteName")]
#endif
		public override string PaletteName { get { return "Yellow"; } }
		public YellowPalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0xFF, 0xCA, 0x08),
			Color.FromArgb(0xFF, 0xF8, 0x93, 0x1D),
			Color.FromArgb(0xFF, 0xCE, 0x8D, 0x3E),
			Color.FromArgb(0xFF, 0xEC, 0x70, 0x16),
			Color.FromArgb(0xFF, 0xE6, 0x48, 0x23),
			Color.FromArgb(0xFF, 0x9C, 0x6A, 0x6A),
			}) {
		}
	}
	public class YellowOrangePalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("YellowOrangePalettePaletteName")]
#endif
		public override string PaletteName { get { return "Yellow Orange"; } }
		public YellowOrangePalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0xF0, 0xA2, 0x2E),
			Color.FromArgb(0xFF, 0xA5, 0x64, 0x4E),
			Color.FromArgb(0xFF, 0xB5, 0x8B, 0x80),
			Color.FromArgb(0xFF, 0xC3, 0x98, 0x6D),
			Color.FromArgb(0xFF, 0xA1, 0x95, 0x74),
			Color.FromArgb(0xFF, 0xC1, 0x75, 0x29),
			}) {
		}
	}
	public class OrangePalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("OrangePalettePaletteName")]
#endif
		public override string PaletteName { get { return "Orange"; } }
		public OrangePalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0xE4, 0x83, 0x12),
			Color.FromArgb(0xFF, 0xBD, 0x58, 0x2C),
			Color.FromArgb(0xFF, 0x86, 0x56, 0x40),
			Color.FromArgb(0xFF, 0x9B, 0x83, 0x57),
			Color.FromArgb(0xFF, 0xC2, 0xBC, 0x80),
			Color.FromArgb(0xFF, 0x94, 0xA0, 0x88),
			}) {
		}
	}
	public class OrangeRedPalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("OrangeRedPalettePaletteName")]
#endif
		public override string PaletteName { get { return "Orange Red"; } }
		public OrangeRedPalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0xD3, 0x48, 0x17),
			Color.FromArgb(0xFF, 0x9B, 0x2D, 0x1F),
			Color.FromArgb(0xFF, 0xA2, 0x8E, 0x6A),
			Color.FromArgb(0xFF, 0x95, 0x62, 0x51),
			Color.FromArgb(0xFF, 0x91, 0x84, 0x85),
			Color.FromArgb(0xFF, 0x85, 0x5D, 0x5D),
			}) {
		}
	}
	public class RedOrangePalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("RedOrangePalettePaletteName")]
#endif
		public override string PaletteName { get { return "Red Orange"; } }
		public RedOrangePalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0xE8, 0x4C, 0x22),
			Color.FromArgb(0xFF, 0xFF, 0xBD, 0x47),
			Color.FromArgb(0xFF, 0xB6, 0x49, 0x26),
			Color.FromArgb(0xFF, 0xFF, 0x84, 0x27),
			Color.FromArgb(0xFF, 0xCC, 0x99, 0x00),
			Color.FromArgb(0xFF, 0xB2, 0x26, 0x00),
			}) {
		}
	}
	public class RedPalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("RedPalettePaletteName")]
#endif
		public override string PaletteName { get { return "Red"; } }
		public RedPalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0xA5, 0x30, 0x0F),
			Color.FromArgb(0xFF, 0xD5, 0x58, 0x16),
			Color.FromArgb(0xFF, 0xE1, 0x98, 0x25),
			Color.FromArgb(0xFF, 0xB1, 0x9C, 0x7D),
			Color.FromArgb(0xFF, 0x7F, 0x5F, 0x52),
			Color.FromArgb(0xFF, 0xB2, 0x7D, 0x49),
			}) {
		}
	}
	public class RedVioletPalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("RedVioletPalettePaletteName")]
#endif
		public override string PaletteName { get { return "Red Violet"; } }
		public RedVioletPalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0xE3, 0x2D, 0x91),
			Color.FromArgb(0xFF, 0xC8, 0x30, 0xCC),
			Color.FromArgb(0xFF, 0x4E, 0xA6, 0xDC),
			Color.FromArgb(0xFF, 0x47, 0x75, 0xE7),
			Color.FromArgb(0xFF, 0x89, 0x71, 0xE1),
			Color.FromArgb(0xFF, 0xD5, 0x47, 0x73),
			}) {
		}
	}
	public class VioletPalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("VioletPalettePaletteName")]
#endif
		public override string PaletteName { get { return "Violet"; } }
		public VioletPalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0xAD, 0x84, 0xC6),
			Color.FromArgb(0xFF, 0x87, 0x84, 0xC7),
			Color.FromArgb(0xFF, 0x5D, 0x73, 0x9A),
			Color.FromArgb(0xFF, 0x69, 0x97, 0xAF),
			Color.FromArgb(0xFF, 0x84, 0xAC, 0xB6),
			Color.FromArgb(0xFF, 0x6F, 0x81, 0x83),
			}) {
		}
	}
	public class VioletIIPalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("VioletIIPalettePaletteName")]
#endif
		public override string PaletteName { get { return "Violet II"; } }
		public VioletIIPalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0x92, 0x27, 0x8F),
			Color.FromArgb(0xFF, 0x9B, 0x57, 0xD3),
			Color.FromArgb(0xFF, 0x75, 0x5D, 0xD9),
			Color.FromArgb(0xFF, 0x66, 0x5E, 0xB8),
			Color.FromArgb(0xFF, 0x45, 0xA5, 0xED),
			Color.FromArgb(0xFF, 0x59, 0x82, 0xDB),
			}) {
		}
	}
	public class MarqueePalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("MarqueePalettePaletteName")]
#endif
		public override string PaletteName { get { return "Marquee"; } }
		public MarqueePalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0x41, 0x8A, 0xB3),
			Color.FromArgb(0xFF, 0xA6, 0xB7, 0x27),
			Color.FromArgb(0xFF, 0xF6, 0x92, 0x00),
			Color.FromArgb(0xFF, 0x83, 0x83, 0x83),
			Color.FromArgb(0xFF, 0xFE, 0xC3, 0x06),
			Color.FromArgb(0xFF, 0xDF, 0x53, 0x27),
			}) {
		}
	}
	public class SlipstreamPalette : PredefinedPalette {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SlipstreamPalettePaletteName")]
#endif
		public override string PaletteName { get { return "Slipstream"; } }
		public SlipstreamPalette()
			: base(new Color[] { 
			Color.FromArgb(0xFF, 0x4E, 0x67, 0xC8),
			Color.FromArgb(0xFF, 0x5E, 0xCC, 0xF3),
			Color.FromArgb(0xFF, 0xA7, 0xEA, 0x52),
			Color.FromArgb(0xFF, 0x5D, 0xCE, 0xAF),
			Color.FromArgb(0xFF, 0xFF, 0x80, 0x21),
			Color.FromArgb(0xFF, 0xF1, 0x41, 0x24),
			}) {
		}
	}
}
