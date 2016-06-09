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
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Automation.Peers;
using System.Windows.Media;
#if !SL
using DevExpress.Data.Access;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Utils;
using DevExpress.Data.Mask;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Automation;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Editors.Validation.Native;
using System.Threading;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Editors.WPFCompatibility.Extensions;
using DevExpress.Data.Mask;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Automation;
#endif
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Utils;
#endif
namespace DevExpress.Xpf.Editors {
	public static class ColorHelper {
		public static Color ColorFromHex(string hex) {
			if(hex.Length != 9) {
				return new Color();
			}
			string a = hex.Substring(1, 2);
			string r = hex.Substring(3, 2);
			string g = hex.Substring(5, 2);
			string b = hex.Substring(7, 2);
			Color color = new Color();
			try {
				int ai = int.Parse(a, NumberStyles.HexNumber);
				int ri = int.Parse(r, NumberStyles.HexNumber);
				int gi = int.Parse(g, NumberStyles.HexNumber);
				int bi = int.Parse(b, NumberStyles.HexNumber);
				color = Color.FromArgb(byte.Parse(ai.ToString()), byte.Parse(ri.ToString()), byte.Parse(gi.ToString()), byte.Parse(bi.ToString()));
			}
			catch {
				return new Color();
			}
			return color;
		}
		public static ColorPalette CreateGradientPalette(string name, ColorCollection source) {
			List<ColorCollection> colorColumns = new List<ColorCollection>();
			for(int i = 0; i < source.Count; i++)
				colorColumns.Add(GenerateColumnFromColor(source[i]));
			return new CustomPalette(name, GetColorsFromColumns(colorColumns), true);
		}
		static ColorCollection GetColorsFromColumns(List<ColorCollection> columns) {
			int columnHeight = 5;
			ColorCollection colors = new ColorCollection();
			for(int i = 0; i < columnHeight; i++)
				for(int j = 0; j < columns.Count; j++)
					colors.Add(columns[j][i]);
			return colors;
		}
		static ColorCollection GenerateColumnFromColor(Color color) {
			ColorCollection paletteRow = new ColorCollection();
			int[] lightColorMod = new int[] { -6, -15, -30, -45, -75 };
			int[] darkColorMod = new int[] { 50, 35, 25, -15, -30 };
			int modLoops = lightColorMod.Length;
			List<Color> whitePallette = new List<Color>();
			whitePallette.Add(ColorHelper.ColorFromHex("#FFF2F2F2"));
			whitePallette.Add(ColorHelper.ColorFromHex("#FFD8D8D8"));
			whitePallette.Add(ColorHelper.ColorFromHex("#FFBFBFBF"));
			whitePallette.Add(ColorHelper.ColorFromHex("#FFA5A5A5"));
			whitePallette.Add(ColorHelper.ColorFromHex("#FF7F7F7F"));
			List<Color> blackPallette = new List<Color>();
			blackPallette.Add(ColorHelper.ColorFromHex("#FF7F7F7F"));
			blackPallette.Add(ColorHelper.ColorFromHex("#FF595959"));
			blackPallette.Add(ColorHelper.ColorFromHex("#FF3F3F3F"));
			blackPallette.Add(ColorHelper.ColorFromHex("#FF262626"));
			blackPallette.Add(ColorHelper.ColorFromHex("#FF0C0C0C"));
			for(int i = 0; i < modLoops; i++) {
				if(color.Equals(Colors.White))
					paletteRow.Add(whitePallette[i]);
				else if(color.Equals(Colors.Black))
					paletteRow.Add(blackPallette[i]);
				else {
					int[] currentModArr;
					if(IsLight(color))
						currentModArr = lightColorMod;
					else
						currentModArr = darkColorMod;
					int modification = currentModArr[i];
					paletteRow.Add(ChangeBrightness(color, modification));
				}
			}
			return paletteRow;
		}
		static bool IsLight(Color color) {
			return GetBrightness(color) > 150;
		}
		static int GetBrightness(Color c) {
			return (int)Math.Sqrt((((c.R * c.R) * 0.241) + ((c.G * c.G) * 0.691)) + ((c.B * c.B) * 0.068));
		}
		static Color ChangeBrightness(Color color, int modification) {
			int r = color.R;
			int g = color.G;
			int b = color.B;
			int defaultDelta = (150 * modification) / 100;
			r += defaultDelta;
			if(r < 0)
				r = 0;
			if(r > 0xff)
				r = 0xff;
			g += defaultDelta;
			if(g < 0)
				g = 0;
			if(g > 0xff)
				g = 0xff;
			b += defaultDelta;
			if(b < 0)
				b = 0;
			if(b > 0xff)
				b = 0xff;
			return Color.FromArgb(0xff, (byte)r, (byte)g, (byte)b);
		}
	}
	public class ColorCollection : List<Color> {
		internal ColorCollection() : base() { }
		public ColorCollection(IEnumerable<Color> source) : base(source) { }
	}
	public abstract class ColorPalette {
		readonly ColorCollection colors = new ColorCollection();
		readonly string name = null;
		readonly bool calcBorder = false;
		protected ColorPalette() { }
		protected ColorPalette(IEnumerable<Color> source) {
			CopyToColors(source);
		}
		protected ColorPalette(string name, IEnumerable<Color> source)
			: this(source) {
			this.name = name;
		}
		protected ColorPalette(string name, IEnumerable<Color> source, bool calcBorder)
			: this(name, source) {
			this.calcBorder = calcBorder;
		}
		public static ColorPalette CreateGradientPalette(string name, ColorCollection source) {
			return ColorHelper.CreateGradientPalette(name, source);
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ColorPaletteName")]
#endif
		public string Name { get { return name; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ColorPaletteColors")]
#endif
		public IList<Color> Colors { get { return colors.AsReadOnly(); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ColorPaletteCalcBorder")]
#endif
		public bool CalcBorder { get { return calcBorder; } }
		void CopyToColors(IEnumerable<Color> source) {
			colors.Clear();
			if(source != null)
				foreach(Color color in source)
					colors.Add(color);
		}
	}
	public sealed class NullPalette : ColorPalette {
		static readonly NullPalette instance = new NullPalette();
		public static NullPalette Instance { get { return instance; } }
		NullPalette() : base() { }
	}
	public class CustomPalette : ColorPalette {
		public CustomPalette(string name, IEnumerable<Color> colors) : base(name, colors) { }
		public CustomPalette(string name, IEnumerable<Color> colors, bool calcBorder) : base(name, colors, calcBorder) { }
	}
	public class PaletteCollection : List<ColorPalette> {
		public PaletteCollection(string name, params ColorPalette[] palettes) : base(palettes) { this.name = name; }
		public PaletteCollection(PaletteCollection collection) : this(collection.Name, collection.ToArray()) {
		}
		new public void Add(ColorPalette newPalette) {
			if(newPalette != null && newPalette.Name != null && FindPaletteByName(newPalette.Name) != NullPalette.Instance)
				throw new ArgumentException("Palette has been already added", newPalette.Name);
			base.Add(newPalette ?? NullPalette.Instance);
		}
		public ColorPalette this[string name] {
			get { return FindPaletteByName(name); }
		}
		readonly string name;
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PaletteCollectionName")]
#endif
		public string Name { get { return name; } }
		ColorPalette FindPaletteByName(string name) {
			foreach(ColorPalette palette in this)
				if(name != null && name == palette.Name)
					return palette;
			return NullPalette.Instance;
		}
	}
	public static class PredefinedColorCollections {
		static readonly ColorCollection standard = new ColorCollection(
				new Color[] {
			ColorEditDefaultColors.DarkRed,
						ColorEditDefaultColors.Red,
						ColorEditDefaultColors.Orange,
						ColorEditDefaultColors.Yellow,
						ColorEditDefaultColors.LightGreen,
						ColorEditDefaultColors.Green,
						ColorEditDefaultColors.LightBlue,
						ColorEditDefaultColors.Blue,
						ColorEditDefaultColors.DarkBlue,
						ColorEditDefaultColors.Purple
				}
		);
		static readonly ColorCollection apex = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFC9C2D1"),
				ColorHelper.ColorFromHex("#FF69676D"),
				ColorHelper.ColorFromHex("#FFCEB966"),
				ColorHelper.ColorFromHex("#FF9CB084"),
				ColorHelper.ColorFromHex("#FF6BB1C9"),
				ColorHelper.ColorFromHex("#FF6585CF"),
				ColorHelper.ColorFromHex("#FF7E6BC9"),
				ColorHelper.ColorFromHex("#FFA379BB")
			});
		static readonly ColorCollection aspect = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFE3DED1"),
				ColorHelper.ColorFromHex("#FF323232"),
				ColorHelper.ColorFromHex("#FFF07F09"),
				ColorHelper.ColorFromHex("#FF9F2936"),
				ColorHelper.ColorFromHex("#FF1B587C"),
				ColorHelper.ColorFromHex("#FF4E8542"),
				ColorHelper.ColorFromHex("#FF604878"),
				ColorHelper.ColorFromHex("#FFC19859")
			});
		static readonly ColorCollection civic = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFC5D1D7"),
				ColorHelper.ColorFromHex("#FF646B86"),
				ColorHelper.ColorFromHex("#FFD16349"),
				ColorHelper.ColorFromHex("#FFCCB400"),
				ColorHelper.ColorFromHex("#FF8CADAE"),
				ColorHelper.ColorFromHex("#FF8C7B70"),
				ColorHelper.ColorFromHex("#FF8FB08C"),
				ColorHelper.ColorFromHex("#FFD19049")
			});
		static readonly ColorCollection concource = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFDEF5FA"),
				ColorHelper.ColorFromHex("#FF464646"),
				ColorHelper.ColorFromHex("#FF2DA2BF"),
				ColorHelper.ColorFromHex("#FFDA1F28"),
				ColorHelper.ColorFromHex("#FFEB641B"),
				ColorHelper.ColorFromHex("#FF39639D"),
				ColorHelper.ColorFromHex("#FF474B78"),
				ColorHelper.ColorFromHex("#FF7D3C4A")
			});
		static readonly ColorCollection equality = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFDEF5FA"),
				ColorHelper.ColorFromHex("#FF464646"),
				ColorHelper.ColorFromHex("#FF2DA2BF"),
				ColorHelper.ColorFromHex("#FFDA1F28"),
				ColorHelper.ColorFromHex("#FFEB641B"),
				ColorHelper.ColorFromHex("#FF39639D"),
				ColorHelper.ColorFromHex("#FF474B78"),
				ColorHelper.ColorFromHex("#FF7D3C4A")
			});
		static readonly ColorCollection flow = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFDBF5F9"),
				ColorHelper.ColorFromHex("#FF04617B"),
				ColorHelper.ColorFromHex("#FF0F6FC6"),
				ColorHelper.ColorFromHex("#FF009DD9"),
				ColorHelper.ColorFromHex("#FF0BD0D9"),
				ColorHelper.ColorFromHex("#FF10CF9B"),
				ColorHelper.ColorFromHex("#FF7CCA62"),
				ColorHelper.ColorFromHex("#FFA5C249")
			});
		static readonly ColorCollection foundry = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFEAEBDE"),
				ColorHelper.ColorFromHex("#FF676A55"),
				ColorHelper.ColorFromHex("#FF72A376"),
				ColorHelper.ColorFromHex("#FFB0CCB0"),
				ColorHelper.ColorFromHex("#FFA8CDD7"),
				ColorHelper.ColorFromHex("#FFC0BEAF"),
				ColorHelper.ColorFromHex("#FFCEC597"),
				ColorHelper.ColorFromHex("#FFE8B7B7")
			});
		static readonly ColorCollection grayscale = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFDDDDDD"),
				ColorHelper.ColorFromHex("#FFB2B2B2"),
				ColorHelper.ColorFromHex("#FF969696"),
				ColorHelper.ColorFromHex("#FF808080"),
				ColorHelper.ColorFromHex("#FF5F5F5F"),
				ColorHelper.ColorFromHex("#FF4D4D4D"),
			});
		static readonly ColorCollection median = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFEBDDC3"),
				ColorHelper.ColorFromHex("#FF775F55"),
				ColorHelper.ColorFromHex("#FF94B6D2"),
				ColorHelper.ColorFromHex("#FFDD8047"),
				ColorHelper.ColorFromHex("#FFA5AB81"),
				ColorHelper.ColorFromHex("#FFD8B25C"),
				ColorHelper.ColorFromHex("#FF7BA79D"),
				ColorHelper.ColorFromHex("#FF968C8C"),
			});
		static readonly ColorCollection metro = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFD6ECFF"),
				ColorHelper.ColorFromHex("#FF4E5B6F"),
				ColorHelper.ColorFromHex("#FF7FD13B"),
				ColorHelper.ColorFromHex("#FFEA157A"),
				ColorHelper.ColorFromHex("#FFFEB80A"),
				ColorHelper.ColorFromHex("#FF00ADDC"),
				ColorHelper.ColorFromHex("#FF738AC8"),
				ColorHelper.ColorFromHex("#FF1AB39F"),
			});
		static readonly ColorCollection module = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFD4D4D6"),
				ColorHelper.ColorFromHex("#FF5A6378"),
				ColorHelper.ColorFromHex("#FFF0AD00"),
				ColorHelper.ColorFromHex("#FF60B5CC"),
				ColorHelper.ColorFromHex("#FFE66C7D"),
				ColorHelper.ColorFromHex("#FF6BB76D"),
				ColorHelper.ColorFromHex("#FFE88651"),
				ColorHelper.ColorFromHex("#FFC64847"),
			});
		static readonly ColorCollection office = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFEEECE1"),
				ColorHelper.ColorFromHex("#FF1F497D"),
				ColorHelper.ColorFromHex("#FF4F81BD"),
				ColorHelper.ColorFromHex("#FFC0504D"),
				ColorHelper.ColorFromHex("#FF9BBB59"),
				ColorHelper.ColorFromHex("#FF8064A2"),
				ColorHelper.ColorFromHex("#FF4BACC6"),
				ColorHelper.ColorFromHex("#FFF79646"),
			});
		static readonly ColorCollection opulent = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFF4E7ED"),
				ColorHelper.ColorFromHex("#FFB13F9A"),
				ColorHelper.ColorFromHex("#FFB83D68"),
				ColorHelper.ColorFromHex("#FFAC66BB"),
				ColorHelper.ColorFromHex("#FFDE6C36"),
				ColorHelper.ColorFromHex("#FFF9B639"),
				ColorHelper.ColorFromHex("#FFCF6DA4"),
				ColorHelper.ColorFromHex("#FFFA8D3D"),
			});
		static readonly ColorCollection oriel = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFFFF39D"),
				ColorHelper.ColorFromHex("#FF575F6D"),
				ColorHelper.ColorFromHex("#FFFE8637"),
				ColorHelper.ColorFromHex("#FF7598D9"),
				ColorHelper.ColorFromHex("#FFB32C16"),
				ColorHelper.ColorFromHex("#FFF5CD2D"),
				ColorHelper.ColorFromHex("#FFAEBAD5"),
				ColorHelper.ColorFromHex("#FF777C84"),
			});
		static readonly ColorCollection origin = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFDDE9EC"),
				ColorHelper.ColorFromHex("#FF464653"),
				ColorHelper.ColorFromHex("#FF727CA3"),
				ColorHelper.ColorFromHex("#FF9FB8CD"),
				ColorHelper.ColorFromHex("#FFD2DA7A"),
				ColorHelper.ColorFromHex("#FFFADA7A"),
				ColorHelper.ColorFromHex("#FFB88472"),
				ColorHelper.ColorFromHex("#FF8E736A"),
			});
		static readonly ColorCollection paper = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFFEFAC9"),
				ColorHelper.ColorFromHex("#FF444D26"),
				ColorHelper.ColorFromHex("#FFA5B592"),
				ColorHelper.ColorFromHex("#FFF3A447"),
				ColorHelper.ColorFromHex("#FFE7BC29"),
				ColorHelper.ColorFromHex("#FFD092A7"),
				ColorHelper.ColorFromHex("#FF9C85C0"),
				ColorHelper.ColorFromHex("#FF809EC2"),
			});
		static readonly ColorCollection solstice = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFE7DEC9"),
				ColorHelper.ColorFromHex("#FF4F271C"),
				ColorHelper.ColorFromHex("#FF4F271C"),
				ColorHelper.ColorFromHex("#FFFEB80A"),
				ColorHelper.ColorFromHex("#FFE7BC29"),
				ColorHelper.ColorFromHex("#FF84AA33"),
				ColorHelper.ColorFromHex("#FF964305"),
				ColorHelper.ColorFromHex("#FF475A8D"),
			});
		static readonly ColorCollection trek = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFFBEEC9"),
				ColorHelper.ColorFromHex("#FF4E3B30"),
				ColorHelper.ColorFromHex("#FFF0A22E"),
				ColorHelper.ColorFromHex("#FFA5644E"),
				ColorHelper.ColorFromHex("#FFB58B80"),
				ColorHelper.ColorFromHex("#FFC3986D"),
				ColorHelper.ColorFromHex("#FFA19574"),
				ColorHelper.ColorFromHex("#FFC17529"),
			});
		static readonly ColorCollection urban = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFDEDEDE"),
				ColorHelper.ColorFromHex("#FF424456"),
				ColorHelper.ColorFromHex("#FF53548A"),
				ColorHelper.ColorFromHex("#FF438086"),
				ColorHelper.ColorFromHex("#FFA04DA3"),
				ColorHelper.ColorFromHex("#FFC4652D"),
				ColorHelper.ColorFromHex("#FF8B5D3D"),
				ColorHelper.ColorFromHex("#FF5C92B5"),
			});
		static readonly ColorCollection verve = new ColorCollection(
			new Color[] {
				ColorHelper.ColorFromHex("#FFFFFFFF"),
				ColorHelper.ColorFromHex("#FF000000"),
				ColorHelper.ColorFromHex("#FFD2D2D2"),
				ColorHelper.ColorFromHex("#FF666666"),
				ColorHelper.ColorFromHex("#FFFF388C"),
				ColorHelper.ColorFromHex("#FFE40059"),
				ColorHelper.ColorFromHex("#FF9C007F"),
				ColorHelper.ColorFromHex("#FF68007F"),
				ColorHelper.ColorFromHex("#FF005BD3"),
				ColorHelper.ColorFromHex("#FF00349E"),
			});
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsApex")]
#endif
		public static ColorCollection Apex { get { return apex; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsAspect")]
#endif
		public static ColorCollection Aspect { get { return aspect; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsCivic")]
#endif
		public static ColorCollection Civic { get { return civic; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsConcourse")]
#endif
		public static ColorCollection Concourse { get { return concource; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsEquality")]
#endif
		public static ColorCollection Equality { get { return equality; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsFlow")]
#endif
		public static ColorCollection Flow { get { return flow; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsFoundry")]
#endif
		public static ColorCollection Foundry { get { return foundry; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsGrayscale")]
#endif
		public static ColorCollection Grayscale { get { return grayscale; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsMedian")]
#endif
		public static ColorCollection Median { get { return median; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsMetro")]
#endif
		public static ColorCollection Metro { get { return metro; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsModule")]
#endif
		public static ColorCollection Module { get { return module; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsOffice")]
#endif
		public static ColorCollection Office { get { return office; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsOpulent")]
#endif
		public static ColorCollection Opulent { get { return opulent; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsOriel")]
#endif
		public static ColorCollection Oriel { get { return oriel; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsOrigin")]
#endif
		public static ColorCollection Origin { get { return origin; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsPaper")]
#endif
		public static ColorCollection Paper { get { return paper; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsSolstice")]
#endif
		public static ColorCollection Solstice { get { return solstice; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsStandard")]
#endif
		public static ColorCollection Standard { get { return standard; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsTrek")]
#endif
		public static ColorCollection Trek { get { return trek; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsUrban")]
#endif
		public static ColorCollection Urban { get { return urban; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedColorCollectionsVerve")]
#endif
		public static ColorCollection Verve { get { return verve; } }
	}
	public static class PredefinedPaletteCollections {
		static readonly string themeColorsName = EditorLocalizer.GetString(EditorStringId.ColorEdit_ThemeColorsCaption);
		static readonly string gradientColorsName = null;
		static readonly string standardColorsName = EditorLocalizer.GetString(EditorStringId.ColorEdit_StandardColorsCaption);
		static PaletteCollection apex = new PaletteCollection(
			"Apex",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Apex),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Apex),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static PaletteCollection aspect = new PaletteCollection(
			"Aspect",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Aspect),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Aspect),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static PaletteCollection civic = new PaletteCollection(
			"Civic",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Civic),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Civic),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static PaletteCollection concourse = new PaletteCollection(
			"Concourse",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Concourse),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Concourse),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static PaletteCollection equality = new PaletteCollection(
			"Equality",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Equality),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Equality),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static PaletteCollection flow = new PaletteCollection(
			"Flow",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Flow),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Flow),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static PaletteCollection foundry = new PaletteCollection(
			"Foundry",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Foundry),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Foundry),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static PaletteCollection grayscale = new PaletteCollection(
			"Grayscale",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Grayscale),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Grayscale),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static PaletteCollection median = new PaletteCollection(
			"Median",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Median),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Median),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static PaletteCollection metro = new PaletteCollection(
			"Metro",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Metro),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Metro),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static PaletteCollection module = new PaletteCollection(
			"Module",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Module),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Module),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static PaletteCollection office = new PaletteCollection(
			"Office",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Office),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Office),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static PaletteCollection opulent = new PaletteCollection(
			"Opulent",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Opulent),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Opulent),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static PaletteCollection oriel = new PaletteCollection(
			"Oriel",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Oriel),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Oriel),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static PaletteCollection origin = new PaletteCollection(
			"Origin",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Origin),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Origin),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static PaletteCollection paper = new PaletteCollection(
			"Paper",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Paper),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Paper),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static PaletteCollection solstice = new PaletteCollection(
			"Solstice",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Solstice),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Solstice),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static PaletteCollection trek = new PaletteCollection(
			"Trek",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Trek),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Trek),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static PaletteCollection urban = new PaletteCollection(
			"Urban",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Urban),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Urban),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static PaletteCollection verve = new PaletteCollection(
			"Verve",
			new CustomPalette(themeColorsName, PredefinedColorCollections.Verve),
			ColorPalette.CreateGradientPalette(gradientColorsName, PredefinedColorCollections.Verve),
			new CustomPalette(standardColorsName, PredefinedColorCollections.Standard)
			);
		static ReadOnlyCollection<PaletteCollection> CreatePaletteCollections() {
			return new List<PaletteCollection>(
				new PaletteCollection[] { 
					Apex, 
					Aspect, 
					Civic, 
					Concourse, 
					Equality, 
					Flow, 
					Foundry, 
					Grayscale, 
					Median, 
					Metro, 
					Module, 
					Office, 
					Opulent, 
					Oriel, 
					Origin, 
					Paper, 
					Solstice, 
					Trek, 
					Urban, 
					Verve 
				}
				).AsReadOnly();
		}
		static ReadOnlyCollection<PaletteCollection> collections = CreatePaletteCollections();
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsApex")]
#endif
		public static PaletteCollection Apex { get { return new PaletteCollection(apex); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsAspect")]
#endif
		public static PaletteCollection Aspect { get { return new PaletteCollection(aspect); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsCivic")]
#endif
		public static PaletteCollection Civic { get { return new PaletteCollection(civic); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsConcourse")]
#endif
		public static PaletteCollection Concourse { get { return new PaletteCollection(concourse); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsEquality")]
#endif
		public static PaletteCollection Equality { get { return new PaletteCollection(equality); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsFlow")]
#endif
		public static PaletteCollection Flow { get { return new PaletteCollection(flow); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsFoundry")]
#endif
		public static PaletteCollection Foundry { get { return new PaletteCollection(foundry); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsGrayscale")]
#endif
		public static PaletteCollection Grayscale { get { return new PaletteCollection(grayscale); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsMedian")]
#endif
		public static PaletteCollection Median { get { return new PaletteCollection(median); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsMetro")]
#endif
		public static PaletteCollection Metro { get { return new PaletteCollection(metro); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsModule")]
#endif
		public static PaletteCollection Module { get { return new PaletteCollection(module); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsOffice")]
#endif
		public static PaletteCollection Office { get { return new PaletteCollection(office); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsOpulent")]
#endif
		public static PaletteCollection Opulent { get { return new PaletteCollection(opulent); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsOriel")]
#endif
		public static PaletteCollection Oriel { get { return new PaletteCollection(oriel); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsOrigin")]
#endif
		public static PaletteCollection Origin { get { return new PaletteCollection(origin); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsPaper")]
#endif
		public static PaletteCollection Paper { get { return new PaletteCollection(paper); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsSolstice")]
#endif
		public static PaletteCollection Solstice { get { return new PaletteCollection(solstice); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsTrek")]
#endif
		public static PaletteCollection Trek { get { return new PaletteCollection(trek); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsUrban")]
#endif
		public static PaletteCollection Urban { get { return new PaletteCollection(urban); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsVerve")]
#endif
		public static PaletteCollection Verve { get { return new PaletteCollection(verve); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PredefinedPaletteCollectionsCollections")]
#endif
		public static ReadOnlyCollection<PaletteCollection> Collections { get { return collections; } }
	}
	public class ColorEditDefaultColors {
		static Dictionary<Color, string> predefinedColors = new Dictionary<Color, string>();
		static ColorEditDefaultColors() {
			predefinedColors.Add(White, EditorLocalizer.GetString(EditorStringId.ColorEdit_DefaultColors_White));
			predefinedColors.Add(Black, EditorLocalizer.GetString(EditorStringId.ColorEdit_DefaultColors_Black));
			predefinedColors.Add(DarkRed, EditorLocalizer.GetString(EditorStringId.ColorEdit_DefaultColors_DarkRed));
			predefinedColors.Add(Red, EditorLocalizer.GetString(EditorStringId.ColorEdit_DefaultColors_Red));
			predefinedColors.Add(Orange, EditorLocalizer.GetString(EditorStringId.ColorEdit_DefaultColors_Orange));
			predefinedColors.Add(Yellow, EditorLocalizer.GetString(EditorStringId.ColorEdit_DefaultColors_Yellow));
			predefinedColors.Add(LightGreen, EditorLocalizer.GetString(EditorStringId.ColorEdit_DefaultColors_LightGreen));
			predefinedColors.Add(Green, EditorLocalizer.GetString(EditorStringId.ColorEdit_DefaultColors_Green));
			predefinedColors.Add(LightBlue, EditorLocalizer.GetString(EditorStringId.ColorEdit_DefaultColors_LightBlue));
			predefinedColors.Add(Blue, EditorLocalizer.GetString(EditorStringId.ColorEdit_DefaultColors_Blue));
			predefinedColors.Add(DarkBlue, EditorLocalizer.GetString(EditorStringId.ColorEdit_DefaultColors_DarkBlue));
			predefinedColors.Add(Purple, EditorLocalizer.GetString(EditorStringId.ColorEdit_DefaultColors_Purple));
		}
		public static readonly Color White = Colors.White;
		public static readonly Color Black = Colors.Black;
		public static readonly Color DarkRed = Color.FromArgb(0xFF, 0x8B, 0x00, 0x00);
		public static readonly Color Red = Colors.Red;
		public static readonly Color Orange = Colors.Orange;
		public static readonly Color Yellow = Colors.Yellow;
		public static readonly Color LightGreen = Color.FromArgb(0xFF, 0x90, 0xEE, 0x90);
		public static readonly Color Green = Colors.Green;
		public static readonly Color LightBlue = Color.FromArgb(0xFF, 0xAD, 0xD8, 0xE6);
		public static readonly Color Blue = Colors.Blue;
		public static readonly Color DarkBlue = Color.FromArgb(0xFF, 0x00, 0x00, 0x8B);
		public static readonly Color Purple = Colors.Purple;
		public static string GetColorName(Color color) {
			string colorName;
			if(predefinedColors.TryGetValue(color, out colorName))
				return colorName;
			return null;
		}
	}
}
