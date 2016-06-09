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
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace DevExpress.Charts.Designer.Native {
	public static class GlyphUtils {
		public const string Series = "Series/";
		public const string BarItemImages = "BarItemImages/";
		public const string GalleryItemImages = "GalleryItemImages/";
		public const string TreeImages = "Tree/";
		public const string ElementsPageImages = "BarItemImages/Elements/";
		const string smallGlyphSuffix = "_16";
		const string largeGlyphSuffix = "_32";
		const string glyphExtension = ".png";
		readonly static string asmName;
		static GlyphUtils() {
			asmName = (typeof(GlyphUtils)).Assembly.GetName().Name;
		}
		static string GetFullPath(string glyph) {
			return "pack://application:,,,/" + asmName + ";component/Images/" + glyph;
		}
		public static string GetSeriesGlyph(Type seriesType) {
			return Series + seriesType.Name;
		}
		public static ImageSource GetGlyphByPath(string glyphPath) {
			if (string.IsNullOrEmpty(glyphPath))
				return null;
			return new BitmapImage(new Uri(GetFullPath(glyphPath) + smallGlyphSuffix + glyphExtension, UriKind.Absolute));
		}
		public static ImageSource GetLargeGlyphByPath(string glyphPath) {
			if (string.IsNullOrEmpty(glyphPath))
				return null;
			return new BitmapImage(new Uri(GetFullPath(glyphPath) + largeGlyphSuffix + glyphExtension, UriKind.Absolute));
		}
		public static ImageSource GetGalleryItemGlyphByPath(string glyphPath) {
			if (string.IsNullOrEmpty(glyphPath))
				return null;
			return new BitmapImage(new Uri(GetFullPath(glyphPath) + glyphExtension, UriKind.Absolute));
		}
		public static string GetTreeDiagramTypeGlyph(Type diagramType) {
			return TreeImages + diagramType.Name;
		}
	}
}
