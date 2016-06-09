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
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraCharts.Native;
using System.Collections.Generic;
using DevExpress.XtraCharts.Localization;
using System.Reflection;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartDictionarySerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class PaletteRepository : PaletteListBase, IPaletteRepository {
		readonly PredefinedPaletteList predefinedList;
		internal int PredefinedCount { get { return predefinedList.GetPaletteNames().Length; } }
		internal int TotalCount { get { return PredefinedCount + Count; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("PaletteRepositoryItem")]
#endif
		public override Palette this[string name] {
			get {
				Palette palette = GetPaletteByName(name);
				if (palette == null)
					throw new PaletteException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgPaletteNotFound), name));
				return palette;
			}
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("PaletteRepositoryPaletteNames")]
#endif
		public string[] PaletteNames {
			get {
				List<string> res = new List<string>();
				res.AddRange(predefinedList.GetPaletteNames());
				res.AddRange(GetPaletteNames());
				return res.ToArray();
			}
		}
		internal PaletteRepository(PredefinedPaletteList predefinedPalettes) {
			predefinedList = predefinedPalettes;
		}
		public PaletteRepository() : this (new PredefinedPaletteList()) {
		}
		internal void Assign(PaletteRepository repository) {
			Clear();
			foreach (Palette palette in repository.Values)
				RegisterPalette((Palette)palette.Clone());
		}
		internal Palette GetPaletteByName(string name) {
			Palette palette = predefinedList[name] as Palette;
			if (palette == null)
				palette = base[name] as Palette;
			return palette;
		}
		internal Palette GetAppearancePalette(string paletteName) {
			if (!String.IsNullOrEmpty(paletteName))
				try {
					return this[paletteName];
				}
				catch {
				}
			return null;
		}
		internal IList<Palette> GetPaletteList() {
			List<Palette> paletteList = new List<Palette>(TotalCount);
			foreach (string name in PaletteNames)
				paletteList.Add(this[name]);
			return paletteList;
		}
		internal void ReRegisterPalette(Palette palette) {
			if (palette == null)
				throw new ArgumentNullException("palette");
			int index = IndexOfValue(palette);
			if (index < 0)
				return;
			string name = (string)GetKey(index);
			if (name != palette.Name) {
				RegisterPalette(palette);
				Remove(name);
			}
		}
		public void Add(string name, Palette palette) {
			if (!String.IsNullOrEmpty(name))
				palette.SetName(name);
			RegisterPalette(palette);
		}
		public void RegisterPalette(Palette palette) {
			if (palette == null)
				throw new PaletteException(ChartLocalizer.GetString(ChartStringId.MsgEmptyArgument));
			if (String.IsNullOrEmpty(palette.Name))
				throw new PaletteException(ChartLocalizer.GetString(ChartStringId.MsgInvalidPaletteName));
			if (predefinedList[palette.Name] != null)
				throw new PaletteException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgAddExistingPaletteError), palette.Name));
			if (IndexOfKey(palette.Name) >= 0)
				this[palette.Name] = palette;
			else
				base.Add(palette.Name, palette);
		}
		[Obsolete("This method is now obsolete. Use the \"void RegisterPalette(Palette palette)\" constructor instead.")]
		public void RegisterPalette(string name, Palette palette) {
			Add(name, palette);
		}
		#region IPaletteRepository implementation
		int IPaletteRepository.PredefinedCount { get { return PredefinedCount; } }
		void IPaletteRepository.ReRegisterPalette(Palette palette) {
			ReRegisterPalette(palette);
		}
		#endregion
	}
}
