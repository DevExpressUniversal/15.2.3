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

using DevExpress.Data.Utils;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using System.IO;
using System.Reflection;
using System.Windows.Media;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	#region AutoFilterImageCache
	public class AutoFilterImageCache {
		string[] imageNames = new string[] { "DropDown", "Filtered", "Ascending", "Descending", "FilteredAndAscending", "FilteredAndDescending" };
		ImageSource[] imageSources;
		public AutoFilterImageCache() {
			ResetCache();
		}
		public void ResetCache() {
			imageSources = new ImageSource[imageNames.Length];
		}
		public ImageSource GetImageSource(int imageIndex) {
			ImageSource result = imageSources[imageIndex];
			if (result != null)
				return result;
			string imageName = imageNames[imageIndex];
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.Xpf.Spreadsheet.Images.AutoFilter." + imageName + ".png");
			if (stream == null)
				return null;
			System.Drawing.Image image = ImageTool.ImageFromStream(stream);
			result = image.ToImageSource();
			imageSources[imageIndex] = result;
			return result;
		}
		public ImageSource GetImageSource(IFilterHotZone hotZone) {
			int index = 0;
			if (hotZone is AutoFilterColumnHotZone)
				index = CalculateAutoFilterImageIndex(hotZone as AutoFilterColumnHotZone);
			else
				index = CalculatePivotTableFilterImageIndex(hotZone as PivotTableFilterHotZone);
			return GetImageSource(index);
		}
		public int CalculateAutoFilterImageIndex(AutoFilterColumnHotZone hotZone) {
			SortCondition sortCondition = hotZone.SortCondition;
			if (hotZone.FilterColumn.IsNonDefault) {
				if (sortCondition == null)
					return 1;
				return sortCondition.Descending ? 5 : 4;
			}
			else {
				if (sortCondition == null)
					return 0;
				return sortCondition.Descending ? 3 : 2;
			}
		}
		public int CalculatePivotTableFilterImageIndex(PivotTableFilterHotZone hotZone) {
			if (hotZone.IsFilterApplied) {
				if (!hotZone.IsSortApplied)
					return 1;
				return hotZone.SortTypeDescending ? 5 : 4;
			}
			else {
				if (!hotZone.IsSortApplied)
					return 0;
				return hotZone.SortTypeDescending ? 3 : 2;
			}
		}
	}
	#endregion
}
