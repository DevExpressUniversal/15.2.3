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
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using DevExpress.XtraPrinting.Export.Pdf;
using System.IO;
using System.Drawing.Drawing2D;
namespace DevExpress.Printing.Core.PdfExport.Metafile {
	public enum CharacterSet {
		ANSI_CHARSET = 0x00000000,
		DEFAULT_CHARSET = 0x00000001,
		SYMBOL_CHARSET = 0x00000002,
		MAC_CHARSET = 0x0000004D,
		SHIFTJIS_CHARSET = 0x00000080,
		HANGUL_CHARSET = 0x00000081,
		JOHAB_CHARSET = 0x00000082,
		GB2312_CHARSET = 0x00000086,
		CHINESEBIG5_CHARSET = 0x00000088,
		GREEK_CHARSET = 0x000000A1,
		TURKISH_CHARSET = 0x000000A2,
		VIETNAMESE_CHARSET = 0x000000A3,
		HEBREW_CHARSET = 0x000000B1,
		ARABIC_CHARSET = 0x000000B2,
		BALTIC_CHARSET = 0x000000BA,
		RUSSIAN_CHARSET = 0x000000CC,
		THAI_CHARSET = 0x000000DE,
		EASTEUROPE_CHARSET = 0x000000EE,
		OEM_CHARSET = 0x000000FF
	}
}
