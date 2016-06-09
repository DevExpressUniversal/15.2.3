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
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Utils.Design;
using DevExpress.DashboardWin.Native;
namespace DevExpress.DashboardWin.Design {
	static class ActionNames {
		public const string AddDropDown = "AddDropDown";
		public const string DataSource = "DataSource";
		public const string Dimension = "Dimension";
		public const string Measure = "Measure";
		public const string Delta = "Delta";
		public const string Sparkline = "Sparkline";
		public const string Value = "Value";
		public const string Cards = "Cards";
		public const string Gauges = "Gauges";
		public const string Pies = "Pies";
		public const string Chart = "Chart";
		public const string Scatter = "ScatterChart";
		public const string Grid = "Grid";
		public const string Pivot = "Pivot";
		public const string RangeFilter = "RangeFilter";
		public const string ChoroplethMap = "ChoroplethMap";
		public const string GeoPointMap = "GeoPointMap";
		public const string BubbleMap = "BubbleMap";
		public const string PieMap = "PieMap";
		public const string TextBox = "TextBox";
		public const string Image = "Image";
		public const string Group = "Group";
		public const string ComboBox = "ComboBox";
		public const string ListBox = "ListBox";
		public const string TreeView = "TreeView";
	}
	static class BitmapStorage {
		const string BitmapExt = ".png";
		const string BitmapPath = "Images.";
		static bool useColors = false;
		static Dictionary<string, Bitmap> dic = new Dictionary<string, Bitmap>();
		public static Bitmap LoadBitmap(string name) {
			return new Bitmap(typeof(ResFinder), BitmapPath + name + BitmapExt);
		}
		static Bitmap LoadCommonBitmap(string name) {
			return new Bitmap(typeof(DevExpress.DashboardWin.ResFinder), BitmapPath + name + BitmapExt);
		}
		static Bitmap LoadDashboardItemBitmap(string name) {
			return useColors ? LoadCommonBitmap("Bars.Insert" + name + "_16x16") : LoadBitmap("DashboardItems." + name);
		}
		public static bool UseColors { get { return useColors; } }
		public static Bitmap GetBitmap(string actionName) {
			Bitmap bmp = null;
			dic.TryGetValue(actionName, out bmp);
			return bmp;
		}
		public static void Initialize(IServiceProvider serviceProvider) {
			if(dic.Count > 0)
				return;
			Version version = VSVersions.GetVersion(serviceProvider);
			useColors = version == VSVersions.VS2010;
			ChartSeriesGalleryControl.UseColors = useColors;
			dic.Add(ActionNames.AddDropDown, LoadBitmap("AddDropDown"));
			dic.Add(ActionNames.Dimension, LoadCommonBitmap("Groups.GridDimensionColumn"));
			dic.Add(ActionNames.Measure, LoadCommonBitmap("Groups.GridMeasureColumn"));
			dic.Add(ActionNames.Delta, LoadCommonBitmap("Groups.GridDeltaColumn"));
			dic.Add(ActionNames.Sparkline, LoadCommonBitmap("Groups.GridSparklineColumn"));
			dic.Add(ActionNames.Value, LoadCommonBitmap("Groups.GridMeasureColumn"));
			dic.Add(ActionNames.Cards, LoadDashboardItemBitmap(ActionNames.Cards));
			dic.Add(ActionNames.Gauges, LoadDashboardItemBitmap(ActionNames.Gauges));
			dic.Add(ActionNames.Pies, LoadDashboardItemBitmap(ActionNames.Pies));
			dic.Add(ActionNames.Chart, LoadDashboardItemBitmap(ActionNames.Chart));
			dic.Add(ActionNames.Scatter, LoadDashboardItemBitmap(ActionNames.Scatter));
			dic.Add(ActionNames.Grid, LoadDashboardItemBitmap(ActionNames.Grid));
			dic.Add(ActionNames.Pivot, LoadDashboardItemBitmap(ActionNames.Pivot));
			dic.Add(ActionNames.ChoroplethMap, LoadDashboardItemBitmap(ActionNames.ChoroplethMap));
			dic.Add(ActionNames.GeoPointMap, LoadDashboardItemBitmap(ActionNames.GeoPointMap));
			dic.Add(ActionNames.BubbleMap, LoadDashboardItemBitmap(ActionNames.BubbleMap));
			dic.Add(ActionNames.PieMap, LoadDashboardItemBitmap(ActionNames.PieMap));
			dic.Add(ActionNames.RangeFilter, LoadDashboardItemBitmap(ActionNames.RangeFilter));
			dic.Add(ActionNames.TextBox, LoadDashboardItemBitmap(ActionNames.TextBox));
			dic.Add(ActionNames.Image, LoadDashboardItemBitmap(ActionNames.Image));
			dic.Add(ActionNames.Group, LoadDashboardItemBitmap(ActionNames.Group));
			dic.Add(ActionNames.ComboBox, LoadDashboardItemBitmap(ActionNames.ComboBox));
			dic.Add(ActionNames.ListBox, LoadDashboardItemBitmap(ActionNames.ListBox));
			dic.Add(ActionNames.TreeView, LoadDashboardItemBitmap(ActionNames.TreeView));
		}
		public static string GetResourceName(string imageName) {
			return GetResourceName(imageName, true);
		}
		static string GetResourceName(string imageName, bool isFullName) {
			string name = useColors ? imageName + "_Color" : imageName;
			return isFullName ? BitmapPath + name + BitmapExt : name;
		}
	}
}
