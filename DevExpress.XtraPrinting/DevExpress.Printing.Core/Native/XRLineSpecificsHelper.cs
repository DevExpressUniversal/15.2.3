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
using DevExpress.XtraReports.UI;
#if SL
using DevExpress.Xpf.Drawing.Drawing2D;
#else
using System.Drawing.Drawing2D;
#endif
namespace DevExpress.XtraPrinting.Native {
	public class XRLineSpecificsHelper {
		const char Separator = ';';
#if !SL
		public static string Combine(LineDirection lineDirection, DashStyle dashStyle, int lineWidth, int colorARGB) {
			return string.Join(Separator.ToString(), new object[] { lineDirection, dashStyle, lineWidth, colorARGB});
		}
#endif
		public static LineSpecifics Split(string combinedLineSpecifics) {
			string[] parts = combinedLineSpecifics.Split(Separator);
			if(parts.Length < 4) {
				return new LineSpecifics();
			}
			LineDirection lineDirection;
			Enum.TryParse<LineDirection>(parts[0], out lineDirection);
			DashStyle lineStyle;
			Enum.TryParse<DashStyle>(parts[1], out lineStyle);
			int linewidth;
			int colorARGB;
			int.TryParse(parts[2], out linewidth);
			int.TryParse(parts[3], out colorARGB);
			return new LineSpecifics() {
				LineDirection = lineDirection,
				LineStyle = lineStyle,
				LineWidth = linewidth,
				ColorARGB = colorARGB
			};
		}
	}
}
