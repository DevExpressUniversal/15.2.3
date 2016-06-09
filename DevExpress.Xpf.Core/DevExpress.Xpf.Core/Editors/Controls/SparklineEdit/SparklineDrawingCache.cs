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

using DevExpress.Xpf.Editors.Internal;
using System;
using System.Collections.Generic;
using System.Windows.Media;
namespace DevExpress.Xpf.Editors.Internal {
	public class SparklineDrawingCache : IDisposable {
		readonly Dictionary<Color, SolidColorBrush> brushes = new Dictionary<Color, SolidColorBrush>();
		readonly Dictionary<SolidColorBrush, List<Pen>> pens = new Dictionary<SolidColorBrush, List<Pen>>();
		public SolidColorBrush GetSolidBrush(Color color) {
			if(!brushes.ContainsKey(color))
				brushes.Add(color, new SolidColorBrush(color));
			return brushes[color];
		}
		public Pen GetPen(SolidColorBrush brush, int thinkness) {
			if (!pens.ContainsKey(brush)) {
				pens[brush] = new List<Pen>();
				Pen pen = new Pen(brush, thinkness) { EndLineCap = PenLineCap.Round, StartLineCap = PenLineCap.Round };
				pens[brush].Add(pen);
				return pen; 
			}
			else {
				foreach (var pen in pens[brush]) {
					if (pen.Thickness == thinkness)
						return pen;
				}
				Pen newPen = new Pen(brush, thinkness) { EndLineCap = PenLineCap.Round, StartLineCap = PenLineCap.Round };
				pens[brush].Add(newPen);
				return newPen; 
			}
		}
		public void Dispose() {
			brushes.Clear();
			pens.Clear();
		}
	}
}
