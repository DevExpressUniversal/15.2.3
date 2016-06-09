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
using System.IO;
using System.Collections;
using System.Drawing;
namespace DevExpress.XtraPrinting.Export.Pdf {
	public class PdfPattern : PdfDocumentStreamObject {
		public Color ForegroundColor { get; private set; }
		public Color BackgroundColor { get; private set; }
		public string Name { get; private set; }
		public PdfPattern(bool compressed, Color foreColor, Color backColor, int index)
			: base(compressed) {
			this.ForegroundColor = foreColor;
			this.BackgroundColor = backColor;
			this.Name = string.Format("P{0}", index);
		}
		public override void FillUp() {
			Attributes.Add("PatternType", 1);
			Attributes.Add("PaintType", 1);
			Attributes.Add("TilingType", 2);
			Attributes.Add("BBox", new PdfArray() { 0, 0, 8, 1200 });
			Attributes.Add("Matrix", new PdfArray() { 0.6f, -0.6f, 0.6f, 0.6f, 0, 72 });
			Attributes.Add("XStep", 8);
			Attributes.Add("YStep", 1200);
			PdfDictionary resources = new PdfDictionary();
			PdfDictionary gst = null;
			Stream.SetStringLine("1 0 0 1 0 0 cm");
			DrawPattrnLine(ref gst, "GSF", ForegroundColor, 0, 3);
			DrawPattrnLine(ref gst, "GSB", BackgroundColor, 3, 8);
			resources.AddIfNotNull("ExtGState", gst);
			Attributes.Add("Resources", resources);
		}
		void DrawPattrnLine(ref PdfDictionary gst, string name, Color color, int s, int e) {
			if(color.A == 0)
				return;
			Stream.SetString("q ");
			if(color.A != 0xFF) {
				if(gst == null)
					gst = new PdfDictionary();
				PdfDictionary gs = new PdfDictionary();
				gs.Add("Type", "ExtGState");
				gs.Add("ca", new PdfDouble(ToPdfColorFloat(color.A)));
				gst.Add(name, gs);
				Stream.SetStringLine("/" + name + " gs");
			}
			Stream.SetStringLine(GetColorString(color) + " rg");
			Stream.SetStringLine(string.Format("{0} 0 m {1} 0 l {1} 1200 l {0} 1200 l h f Q", s, e));
		}
		static string GetColorString(System.Drawing.Color color) {
			return ToPdfColor(color.R) + " " + ToPdfColor(color.G) + " " + ToPdfColor(color.B);
		}
		static string ToPdfColor(byte p) {
			return Utils.ToString(ToPdfColorFloat(p));
		}
		static double ToPdfColorFloat(byte p) {
			return Math.Round((float)p / 255f, 3);
		}
		public override void AddToDictionary(PdfDictionary dictionary) {
			dictionary.Add(this.Name, this.Stream);
		}
	}
	public class PdfPatternCollection : PdfObjectCollection<PdfPattern> {
		bool compressed;
		public PdfPatternCollection(bool compressed) {
			this.compressed = compressed;
		}
		public PdfPattern CreateAddUnique(Color foreColor, Color backColor) {
			foreach(PdfPattern pattern in List) {
				if(pattern.ForegroundColor == foreColor && pattern.BackgroundColor == backColor)
					return pattern;
			}
			PdfPattern newShading = new PdfPattern(compressed, foreColor, backColor, List.Count);
			Add(newShading);
			return newShading;
		}
	}
}
