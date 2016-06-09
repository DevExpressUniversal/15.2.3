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
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.Office.Drawing;
using DevExpress.Snap.Localization;
using DevExpress.Snap.Core.Native.LayoutUI;
namespace DevExpress.Snap.Core.Native {
	public class SparklineBoxHotZonePainter : BoxHotZonePainter, ISparklineDropZoneVisitor {
		public SparklineBoxHotZonePainter(Painter painter) : base(painter) { }
		public static readonly string UIString_DropValues = SnapLocalizer.GetString(SnapStringId.HotZonePainter_DropValues);
		protected override string GetLongestString() {
			using(Font font = new Font(SystemFonts.DefaultFont.FontFamily, DefaultFontSize)) {
				float uiStringDropValuesWidth = MeasureString(UIString_DropValues, font).Width;
				float uiStringSecondLineWidth = MeasureString(UIString_SecondLine, font).Width;
				return uiStringSecondLineWidth > uiStringDropValuesWidth ? UIString_SecondLine : UIString_DropValues;
			}
		}
		void ISparklineDropZoneVisitor.Visit(DropValuesSparklineHotZone hotZone) {
			DrawHotZone(hotZone, SparklineBoxHotZonePainter.UIString_DropValues);
		}
	}
}
