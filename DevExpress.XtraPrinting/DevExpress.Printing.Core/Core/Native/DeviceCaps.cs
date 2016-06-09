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
using System.Drawing;
using System.Drawing.Printing;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraPrinting.Native {
	public class DeviceCaps : IDisposable {
		const int
			HORZSIZE = 4,
			VERTSIZE = 6,
			PHYSICALWIDTH = 110,
			PHYSICALHEIGHT = 111,
			PHYSICALOFFSETX = 112,
			PHYSICALOFFSETY = 113;
		[System.Runtime.InteropServices.DllImportAttribute("gdi32.dll"), System.Security.SuppressUnmanagedCodeSecurity]
		static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
		public static Margins GetMinMargins(Graphics graph) {
			using(DeviceCaps devCaps = new DeviceCaps(graph)) {
				Size hvSize = devCaps.GetHVSize();
				Size physSize = devCaps.GetPhysSize();
				Point physOffset = devCaps.GetPhysOffset();
				return new Margins(physOffset.X, Math.Max(0, physSize.Width - physOffset.X - hvSize.Width),
								   physOffset.Y, Math.Max(0, physSize.Height - physOffset.Y - hvSize.Height));
			}
		}
		public static int CompareMargins(Margins m1, Margins m2) {
			return (m1.Top >= m2.Top && m1.Right >= m2.Right && m1.Bottom >= m2.Bottom && m1.Left >= m2.Left) ? 1 : -1;
		}
		Graphics graph;
		IntPtr hdc;
		float dpiX;
		float dpiY;
		DeviceCaps(Graphics graph) {
			this.graph = graph;
			dpiX = graph.DpiX;
			dpiY = graph.DpiY;
			hdc = graph.GetHdc();
		}
		public void Dispose() {
			graph.ReleaseHdc(hdc);
		}
		[System.Security.SecuritySafeCritical]
		Size GetHVSize() {
			int hSize = GetDeviceCaps(hdc, HORZSIZE);
			hSize = (int)GraphicsUnitConverter.Convert(hSize, GraphicsDpi.Millimeter, GraphicsDpi.HundredthsOfAnInch);
			int vSize = GetDeviceCaps(hdc, VERTSIZE);
			vSize = (int)GraphicsUnitConverter.Convert(vSize, GraphicsDpi.Millimeter, GraphicsDpi.HundredthsOfAnInch);
			return new Size(hSize, vSize);
		}
		[System.Security.SecuritySafeCritical]
		Size GetPhysSize() {
			int width = GetDeviceCaps(hdc, PHYSICALWIDTH);
			width = (int)GraphicsUnitConverter.Convert(width, dpiX, GraphicsDpi.HundredthsOfAnInch);
			int height = GetDeviceCaps(hdc, PHYSICALHEIGHT);
			height = (int)GraphicsUnitConverter.Convert(height, dpiY, GraphicsDpi.HundredthsOfAnInch);
			return new Size(width, height);
		}
		[System.Security.SecuritySafeCritical]
		Point GetPhysOffset() {
			int x = GetDeviceCaps(hdc, PHYSICALOFFSETX);
			x = (int)GraphicsUnitConverter.Convert(x, dpiX, GraphicsDpi.HundredthsOfAnInch);
			int y = GetDeviceCaps(hdc, PHYSICALOFFSETY);
			y = (int)GraphicsUnitConverter.Convert(y, dpiY, GraphicsDpi.HundredthsOfAnInch);
			return new Point(x, y);
		}
	}
}
