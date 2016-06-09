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
	public class PdfShading : PdfDocumentDictionaryObject {
		public Color StartColor { get; private set; }
		public Color EndColor { get; private set; }
		public string Name { get; private set; }
		public PdfShading(bool compressed, Color startColor, Color endColor, int index)
			: base(compressed) {
			this.StartColor = startColor;
			this.EndColor = endColor;
			this.Name = string.Format("Sh{0}", index);
		}
		public override void FillUp() {
			Dictionary.Add("ShadingType", 2);
			Dictionary.Add("ColorSpace", "DeviceRGB");
			Dictionary.Add("Extend", new PdfArray() { true, true });
			Dictionary.Add("Coords", new PdfArray() { 0, 0, 1, 0 });
			PdfDictionary function = new PdfDictionary();
			function.Add("FunctionType", 2);
			function.Add("Domain", new PdfArray() { 0, 1 });
			function.Add("C0", GetColorArray(StartColor));
			function.Add("C1", GetColorArray(EndColor));
			function.Add("N", 1);
			Dictionary.Add("Function", function);
		}
		static PdfArray GetColorArray(System.Drawing.Color color) {
			PdfArray array = new PdfArray();
			array.Add(ToPdfColor(color.R));
			array.Add(ToPdfColor(color.G));
			array.Add(ToPdfColor(color.B));
			return array;
		}
		static float ToPdfColor(byte p) {
			return (float)p / 255f;
		}
		public override void AddToDictionary(PdfDictionary dictionary) {
			dictionary.Add(this.Name, this.Dictionary);
		}
	}
	public class PdfShadingCollection : PdfObjectCollection<PdfShading> {
		bool compressed;
		public PdfShadingCollection(bool compressed) {
			this.compressed = compressed;
		}
		public PdfShading CreateAddUnique(Color startColor, Color endColor) {
			foreach(PdfShading shading in List) {
				if(shading.StartColor == startColor && shading.EndColor == endColor)
					return shading;
			}
			PdfShading newShading = new PdfShading(compressed, startColor, endColor, List.Count);
			Add(newShading);
			return newShading;
		}
	}
}
