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
using System.Collections;
using DevExpress.Utils;
using DevExpress.Printing;
#if SL
using System.Windows.Media;
using DevExpress.Xpf.Drawing;
using Brush = DevExpress.Xpf.Drawing.Brush;
#endif
namespace DevExpress.XtraPrinting.Native
{
	public class GdiHashtable : IDisposable
	{
		private Hashtable brushHT;
		private Hashtable penHT;
		public GdiHashtable()
		{
			brushHT = new Hashtable();
			penHT = new Hashtable();
		}
		public virtual void Dispose() {
			if(penHT != null) {
				foreach(Pen pen in penHT.Values) pen.Dispose();
				penHT.Clear();
				penHT = null;
			}
			if(brushHT != null) {
				foreach(SolidBrush brush in brushHT.Values) brush.Dispose();
				brushHT.Clear();
				brushHT = null;
			}
		}
		public SolidBrush GetBrush(Color color) {
			int key = color.GetHashCode();
			SolidBrush brush = (SolidBrush)brushHT[key];
			if(brush == null) {
				brush = new SolidBrush(color);
				brushHT.Add(key, brush);
			}
			return brush;
		}
		private int GetHashCode(Color color, float value) {
			return color.GetHashCode() ^ value.GetHashCode();
		}
		public Pen GetPen(Color color, float width) {
			int key = GetHashCode(color, width);
			Pen pen = (Pen)penHT[key];
			if(pen == null) {
				pen = new Pen(color);
				pen.Width = width;
				penHT.Add(key, pen);
			}
			return pen;
		}
	}
}
