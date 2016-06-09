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
using System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.Data;
namespace DevExpress.XtraPrinting {
#if !WINRT && !DXRESTRICTED
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
#endif
	public enum RtfExportMode {
		SingleFile,
		SingleFilePageByPage
	}
#if !WINRT && !DXRESTRICTED
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
#endif
	public enum HtmlExportMode {
		SingleFile,
		SingleFilePageByPage,
		DifferentFiles
	}
#if !WINRT && !DXRESTRICTED
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
#endif
	public enum ImageExportMode {
		SingleFile,
		SingleFilePageByPage,
		DifferentFiles
	}
#if !WINRT && !DXRESTRICTED
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
#endif
	public enum XlsExportMode {
		SingleFile,
		DifferentFiles
	}
#if !WINRT  && !DXRESTRICTED
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
#endif
	public enum WorkbookColorPaletteCompliance {
		AdjustColorsToDefaultPalette,
		ReducePaletteForExactColors
	}
#if !WINRT && !DXRESTRICTED
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
#endif
	public enum XlsxExportMode {
		SingleFile,
		SingleFilePageByPage,
		DifferentFiles
	}
#if !WINRT && !DXRESTRICTED
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
#endif
	public enum TextExportMode {
		Value,
		Text
	}
#if !WINRT && !DXRESTRICTED
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
#endif
	public enum PdfJpegImageQuality {
		Lowest = 10,
		Low = 25,
		Medium = 50,
		High = 75,
		Highest = 100
	}
#if !WINRT && !DXRESTRICTED
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
#endif
	public enum XpsCompressionOption {
		NotCompressed = -1,
		Normal = 0,
		Maximum = 1,
		Fast = 2,
		SuperFast = 3
	}
#if !WINRT && !DXRESTRICTED
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
#endif
	public enum EncodingType {
		Default,
		ASCII,
		Unicode,
		BigEndianUnicode,
		UTF7,
		UTF8,
		UTF32
	}
}
