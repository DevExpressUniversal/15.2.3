﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using System.Drawing;
namespace DevExpress.Pdf.Drawing {
	public class PdfDashedHorizontalHatchPatternConstructor : PdfHatchPatternConstructor {
		protected override PdfLineCapStyle LineCapStyle { get { return PdfLineCapStyle.Butt; } }
		public PdfDashedHorizontalHatchPatternConstructor(Color foreColor, Color backColor)
			: base(foreColor, backColor) {
		}
		protected override void GetCommands() {
			base.GetCommands();
			Constructor.SetLineCapStyle(PdfLineCapStyle.Butt);
			double tileCenter = LineStep / 2;
			Constructor.SetLineStyle(PdfLineStyle.CreateDashed(tileCenter, tileCenter, 0));
			double lineOffset = LineWidth / 2;
			Constructor.DrawLine(new PdfPoint(0, lineOffset), new PdfPoint(LineStep, lineOffset));
			Constructor.SetLineStyle(PdfLineStyle.CreateDashed(tileCenter, tileCenter, tileCenter));
			Constructor.DrawLine(new PdfPoint(0, lineOffset + tileCenter), new PdfPoint(LineStep, lineOffset + tileCenter));
		}
	}
}
