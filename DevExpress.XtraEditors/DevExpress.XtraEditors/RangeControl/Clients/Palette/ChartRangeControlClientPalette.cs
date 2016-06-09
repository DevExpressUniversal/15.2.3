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

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
namespace DevExpress.XtraEditors.Native {
	public class ChartRangeControlClientPalette {
		public const string ResourceName = "DevExpress.XtraEditors.RangeControl.Clients.Palette.Palettes.xml";
		static List<ChartRangeControlClientPalette> predefinedPalettes;
		static XmlElement OpenResource() {
			XmlDocument doc = new XmlDocument();
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName);
			doc.Load(stream);
			return doc["Palettes"];
		}
		static ChartRangeControlClientPalette CreateFromXml(XmlElement paletteNode) {
			string paletteName = paletteNode.Attributes["Name"].Value;
			string paletteDisplayName = paletteNode.Attributes["DisplayName"].Value;
			ChartRangeControlClientPalette palette = new ChartRangeControlClientPalette(paletteName, paletteDisplayName);
			foreach (XmlElement entryNode in paletteNode["Items"].ChildNodes)
				palette.AddEntry(entryNode["Color"].InnerText);
			return palette;
		}
		public static List<ChartRangeControlClientPalette> PredefinedPalettes {
			get {
				if (predefinedPalettes == null) {
					XmlElement palettes = OpenResource();
					predefinedPalettes = new List<ChartRangeControlClientPalette>();
					foreach (XmlElement element in palettes.ChildNodes)
						predefinedPalettes.Add(ChartRangeControlClientPalette.CreateFromXml(element));
				}
				return predefinedPalettes;
			}
		}
		public static ChartRangeControlClientPalette GetPalette(string name) {
			for (int i = 0; i < PredefinedPalettes.Count; i++) {
				ChartRangeControlClientPalette palette = PredefinedPalettes[i];
				if (palette.Name == name)
					return palette;
			}
			return null;
		}
		readonly string name;
		readonly List<ChartRangeControlClientPaletteEntry> entries;
		readonly string displayName;
		public string Name {
			get { return name; }
		}
		public string DisplayName {
			get { return displayName; }
		}
		public int Count {
			get { return entries.Count; }
		}
		public ChartRangeControlClientPaletteEntry this[int index] {
			get { return entries[index]; }
		}
		ChartRangeControlClientPalette(string name, string displayName) {
			this.name = name;
			this.displayName = displayName;
			this.entries = new List<ChartRangeControlClientPaletteEntry>();
		}
		ChartRangeControlClientPaletteEntry AddEntry(string color) {
			ChartRangeControlClientPaletteEntry entry = new ChartRangeControlClientPaletteEntry(this, color);
			entries.Add(entry);
			return entry;
		}
		public ChartRangeControlClientPaletteEntry GetRepeatingEntry(int index) {
			if (index >= entries.Count)
				index = index % entries.Count;
			return this[index];
		}
		public override string ToString() {
			return DisplayName;
		}
	}
}
