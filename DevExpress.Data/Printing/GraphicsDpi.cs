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
using System.Text;
using System.Drawing;
#if !DXPORTABLE
using DevExpress.XtraPrinting.Native;
#endif
using DevExpress.Compatibility.System.Drawing;
#if SL
using DevExpress.XtraPrinting.Stubs;
using DevExpress.Xpf.Security;
#else
#if !DXPORTABLE
using System.Security.Permissions;
using DevExpress.Data.Helpers;
#endif
#endif
namespace DevExpress.XtraPrinting {
	public class GraphicsDpi {
		public const float Display = 75f,
			Inch = 1f,
			Document = 300f,
			Millimeter = 25.4f,
			Point = 72f,
			HundredthsOfAnInch = 100f,
			TenthsOfAMillimeter = 254f,
			Twips = 1440f,
			EMU = 914400f,
			DeviceIndependentPixel = 96f;
		public static readonly float Pixel = 96f;
		static GraphicsDpi() {
#if !SL && !DXPORTABLE
			if(!PSNativeMethods.AspIsRunning)
				using(Bitmap bmp = new Bitmap(1,1))
				using(Graphics graph = Graphics.FromImage(bmp))
					Pixel = graph.DpiX;
#endif
		}
#if !DXPORTABLE
		public static float GetGraphicsDpi(Graphics gr) {
			if (gr.PageUnit == GraphicsUnit.Display)
				return gr.DpiX;
			return UnitToDpi(gr.PageUnit);
		}
#endif
		public static float UnitToDpi(GraphicsUnit unit) {
			switch (unit) {
				case GraphicsUnit.Display:
					return Display;
				case GraphicsUnit.Inch:
					return Inch;
				case GraphicsUnit.Document:
					return Document;
				case GraphicsUnit.Millimeter:
					return Millimeter;
				case GraphicsUnit.Pixel:
				case GraphicsUnit.World:
					return Pixel;
				case GraphicsUnit.Point:
					return Point;
			}
			throw new ArgumentException("unit");
		}
		public static float UnitToDpiI(GraphicsUnit unit) {
			switch(unit) {
				case GraphicsUnit.Display:
					return Display;
				case GraphicsUnit.Inch:
					return Inch;
				case GraphicsUnit.Document:
					return Document;
				case GraphicsUnit.Millimeter:
					return Millimeter;
				case GraphicsUnit.Pixel:
				case GraphicsUnit.World:
					return DeviceIndependentPixel;
				case GraphicsUnit.Point:
					return Point;
			}
			throw new ArgumentException("unit");
		}
		public static GraphicsUnit DpiToUnit(float dpi) {
			if (dpi.Equals(Display))
				return GraphicsUnit.Display;
			if (dpi.Equals(Inch))
				return GraphicsUnit.Inch;
			if (dpi.Equals(Document))
				return GraphicsUnit.Document;
			if (dpi.Equals(Millimeter))
				return GraphicsUnit.Millimeter;
			if (dpi.Equals(Pixel))
				return GraphicsUnit.Pixel;
			if (dpi.Equals(Point))
				return GraphicsUnit.Point;
			throw new ArgumentException("dpi");
		}
		public static string UnitToString(GraphicsUnit unit) {
			switch(unit) {
				case GraphicsUnit.Millimeter:
					return "mm";
				case GraphicsUnit.Inch:
					return "in";
				case GraphicsUnit.Display:
				case GraphicsUnit.Document:
				case GraphicsUnit.Pixel:
				case GraphicsUnit.World:
				case GraphicsUnit.Point:
					return string.Empty;
			}
			throw new ArgumentException("unit");
		}
	}
}
