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
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.XtraCharts;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public class PaletteItem {
		public PaletteItem(string name, Palette palette) {
			this.name = name;
			this.palette = new Tuple<Brush, Brush, Brush, Brush, Brush, Brush>(
				new SolidColorBrush(Color.FromArgb(palette[0].Color.A, palette[0].Color.R, palette[0].Color.G, palette[0].Color.B)),
				new SolidColorBrush(Color.FromArgb(palette[1].Color.A, palette[1].Color.R, palette[1].Color.G, palette[1].Color.B)),
				new SolidColorBrush(Color.FromArgb(palette[2].Color.A, palette[2].Color.R, palette[2].Color.G, palette[2].Color.B)),
				new SolidColorBrush(Color.FromArgb(palette[3].Color.A, palette[3].Color.R, palette[3].Color.G, palette[3].Color.B)),
				new SolidColorBrush(Color.FromArgb(palette[4].Color.A, palette[4].Color.R, palette[4].Color.G, palette[4].Color.B)),
				new SolidColorBrush(Color.FromArgb(palette[5].Color.A, palette[5].Color.R, palette[5].Color.G, palette[5].Color.B))
			);
		}
		readonly string name;
		public string Name { get { return name; } }
		readonly Tuple<Brush, Brush, Brush, Brush, Brush, Brush> palette;
		public Tuple<Brush, Brush, Brush, Brush, Brush, Brush> Palette { get { return palette; } }
	}
	public static class PaletteHelper {
		static PaletteHelper() {
			var paletteRepository = new PaletteRepository();
			palettes = paletteRepository.PaletteNames.Select(x => new PaletteItem(x, paletteRepository[x])).AsEnumerable();
		}
		readonly static IEnumerable<PaletteItem> palettes;
		public static IEnumerable<PaletteItem> Palettes { get { return palettes; } }
	}
}
