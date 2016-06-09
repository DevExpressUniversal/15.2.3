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
using System.CodeDom.Compiler;
namespace DevExpress.XtraRichEdit.API.Word {
	#region WdTextureIndex
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdTextureIndex {
		wdTexture10Percent = 100,
		wdTexture12Pt5Percent = 0x7d,
		wdTexture15Percent = 150,
		wdTexture17Pt5Percent = 0xaf,
		wdTexture20Percent = 200,
		wdTexture22Pt5Percent = 0xe1,
		wdTexture25Percent = 250,
		wdTexture27Pt5Percent = 0x113,
		wdTexture2Pt5Percent = 0x19,
		wdTexture30Percent = 300,
		wdTexture32Pt5Percent = 0x145,
		wdTexture35Percent = 350,
		wdTexture37Pt5Percent = 0x177,
		wdTexture40Percent = 400,
		wdTexture42Pt5Percent = 0x1a9,
		wdTexture45Percent = 450,
		wdTexture47Pt5Percent = 0x1db,
		wdTexture50Percent = 500,
		wdTexture52Pt5Percent = 0x20d,
		wdTexture55Percent = 550,
		wdTexture57Pt5Percent = 0x23f,
		wdTexture5Percent = 50,
		wdTexture60Percent = 600,
		wdTexture62Pt5Percent = 0x271,
		wdTexture65Percent = 650,
		wdTexture67Pt5Percent = 0x2a3,
		wdTexture70Percent = 700,
		wdTexture72Pt5Percent = 0x2d5,
		wdTexture75Percent = 750,
		wdTexture77Pt5Percent = 0x307,
		wdTexture7Pt5Percent = 0x4b,
		wdTexture80Percent = 800,
		wdTexture82Pt5Percent = 0x339,
		wdTexture85Percent = 850,
		wdTexture87Pt5Percent = 0x36b,
		wdTexture90Percent = 900,
		wdTexture92Pt5Percent = 0x39d,
		wdTexture95Percent = 950,
		wdTexture97Pt5Percent = 0x3cf,
		wdTextureCross = -11,
		wdTextureDarkCross = -5,
		wdTextureDarkDiagonalCross = -6,
		wdTextureDarkDiagonalDown = -3,
		wdTextureDarkDiagonalUp = -4,
		wdTextureDarkHorizontal = -1,
		wdTextureDarkVertical = -2,
		wdTextureDiagonalCross = -12,
		wdTextureDiagonalDown = -9,
		wdTextureDiagonalUp = -10,
		wdTextureHorizontal = -7,
		wdTextureNone = 0,
		wdTextureSolid = 0x3e8,
		wdTextureVertical = -8
	}
	#endregion
	#region Shading
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Shading : IWordObject {
		WdColorIndex ForegroundPatternColorIndex { get; set; }
		WdColorIndex BackgroundPatternColorIndex { get; set; }
		WdTextureIndex Texture { get; set; }
		WdColor ForegroundPatternColor { get; set; }
		WdColor BackgroundPatternColor { get; set; }
	}
	#endregion
}
