#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.DashboardCommon.Printing {
	public class DashboardItemServerData {
		public string Type { get; set; }
		public DashboardItemViewModel ViewModel { get; set; }
		public ConditionalFormattingModel ConditionalFormattingModel { get; set; }
		public object Data { get { return null; } }
		public IList SelectedValues { get; set; }
		public IList DrillDownState { get; set; }
		public IList<DimensionFilterValues> DrillDownValues { get; set; }
		public IList<DimensionFilterValues> FilterValues { get; set; }
		public MultidimensionalDataDTO MultiDimensionalData { get; set; }
		public HierarchicalMetadata Metadata { get; set; }
	}
	public class DimensionFilterValues {
		public string Name { get; set; }
		public bool Truncated { get; set; }
		public IList<FormattableValue> Values { get; set; }
	}
	public class DashboardItemExportData {
		public string Name { get; set; }
		public DashboardItemServerData ServerData { get; set; }
		public ItemViewerClientState ViewerClientState { get; set; }
		public DashboardFontInfo FontInfo { get; set; }
	}
	public class DashboardExportData {
		public IList<DashboardItemExportData> ItemsData { get; set; }
		public TitleLayoutViewModel TitleLayoutModel { get; set; }
		public Rectangle TitleBounds { get; set; }
		public Size ClientSize { get; set; }
		public IList<DimensionFilterValues> MasterFilterValues { get; set; }
		public DashboardFontInfo FontInfo { get; set; }
		public DashboardExportData() { }
		public DashboardExportData(IList<DashboardItemExportData> itemsData) { 
			ItemsData = itemsData;
		}
	}
	public class DashboardFontInfo {
		public string Name { get; set; }
		public byte? GdiCharSet { get; set; }
	}
	public static class FontHelper {
		public static Font GetFont(Font baseFont, DashboardFontInfo fontInfo) {
			if(HasValue(fontInfo)) {
				string newFontName = fontInfo.Name == null ? baseFont.Name : fontInfo.Name;
				byte newGdiCharSet = fontInfo.GdiCharSet.GetValueOrDefault(baseFont.GdiCharSet);
				return new Font(new FontFamily(newFontName), baseFont.Size, baseFont.Style, baseFont.Unit, newGdiCharSet);
			}
			return baseFont;
		}
		public static bool HasValue(DashboardFontInfo fontInfo) {
			return fontInfo != null && (fontInfo.Name != null || fontInfo.GdiCharSet.HasValue);
		}
	}
}
