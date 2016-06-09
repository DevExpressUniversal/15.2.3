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
using System.Windows.Forms;
namespace DevExpress.Utils {
	public class MouseWheelScrollClientArgs : DXMouseEventArgs {
		public MouseWheelScrollClientArgs(DXMouseEventArgs e, bool horz, bool inPixels, int distance, bool allowSystemLinesCount) : base(e) {
			this.ishMouseWheel = e.ishMouseWheel;
			this.Horizontal = horz;
			this.InPixels = inPixels;
			this.Distance = distance;
			this.AllowSystemLinesCount = allowSystemLinesCount;
		}
		public bool Horizontal { get; private set; }
		public int Distance { get; private set; }
		public bool InPixels { get; private set; }
		public bool AllowSystemLinesCount { get; private set; }
	}
	public interface IMouseWheelScrollClient {
		void OnMouseWheel(MouseWheelScrollClientArgs e);
		bool PixelModeHorz { get; }
		bool PixelModeVert { get; }
	}
	public class MouseWheelScrollHelper {
		int direction = 0;
		int distance = 0;
		int distanceLineCounter = 0;
		bool? horizontal = null;
		DateTime lastEvent = DateTime.MinValue;
		const int SkipInterval = 400;
		const int PixelLineHeight = 120;
		IMouseWheelScrollClient client;
		public MouseWheelScrollHelper(IMouseWheelScrollClient client) {
			this.client = client;
		}
		bool allowLog = false;
		bool LineMode {
			get {
				if (!horizontal.HasValue || client == null) return true;
				return !(horizontal.Value ? client.PixelModeHorz : client.PixelModeVert);
			}
		}
		public void OnMouseWheel(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			if (DateTime.Now.Subtract(lastEvent).TotalMilliseconds > SkipInterval) {
				this.direction = 0;
			}
			if (!horizontal.HasValue || horizontal.Value != ee.IsHMouseWheel) {
				this.direction = 0;
				this.horizontal = ee.IsHMouseWheel;
			}
			this.lastEvent = DateTime.Now;
			if (direction != GetDirection(e)) {
				ResetDistance();
				this.direction = GetDirection(e);
			}
			int delta = Math.Abs(e.Delta);
			if (delta % 120 == 0 && distance == 0) {
				OnScrollLine(ee, delta / 120, true);
				return;
			}
			distance += delta;
			if (distance / PixelLineHeight > distanceLineCounter) {
				int x = (distance / PixelLineHeight) - distanceLineCounter;
				distanceLineCounter = distance / PixelLineHeight;
				if (LineMode) OnScrollLine(ee, x, false);
			}
			if (!LineMode) {
				OnScrollPixel(ee, delta);
			}
		}
		void ResetDistance() {
			if (allowLog) Console.WriteLine("** Reset");
			this.distance = 0;
			this.distanceLineCounter = 0;
		}
		int counter = 0;
		void OnScrollLine(DXMouseEventArgs e, int linesCount, bool allowSystemLinesCount) {
			if(client != null) client.OnMouseWheel(new MouseWheelScrollClientArgs(e, horizontal.HasValue && horizontal.Value, false, linesCount * direction, allowSystemLinesCount));
			if (allowLog) Console.WriteLine("{1}: scroll line: {0}", linesCount * direction, counter++);
		}
		void OnScrollPixel(DXMouseEventArgs e, int delta) {
			if (client != null) client.OnMouseWheel(new MouseWheelScrollClientArgs(e, horizontal.HasValue && horizontal.Value, true, delta * direction, false));
			if (allowLog) Console.WriteLine("{1}: scroll pixels: {0}", delta * direction, counter++);
		}
		int GetDirection(MouseEventArgs e) {
			return e.Delta > 0 ? -1 : 1;
		}
	}
}
