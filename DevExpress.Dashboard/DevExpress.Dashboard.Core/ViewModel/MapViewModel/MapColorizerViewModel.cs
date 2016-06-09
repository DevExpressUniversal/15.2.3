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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.DashboardCommon.ViewModel {
	public class MapColorizerViewModel {
		public bool UsePercentRangeStops { get; set; }
		public List<double> RangeStops { get; set; }
		public List<ColorViewModel> Colors { get; set; }
		public MapColorizerViewModel() {
		}
		public MapColorizerViewModel(MapPalette palette, MapScale scale) {
			UniformScale uniformScale = scale as UniformScale;
			if(uniformScale != null) {
				UsePercentRangeStops = true;
				RangeStops = ValueMapScaleHelper.GetPercentRangeStops(uniformScale.LevelsCount);
			}
			else {
				CustomScale customScale = scale as CustomScale;
				if(customScale != null) {
					UsePercentRangeStops = customScale.IsPercent;
					RangeStops = customScale.RangeStops.ToList();
				}
			}
			int rangesCount = RangeStops.Count;
			GradientPalette gradientPalette = palette as GradientPalette;
			if(gradientPalette != null) {
				Colors = new List<ColorViewModel>();
				for(int i = 0; i < rangesCount; i++)
					Colors.Add(new ColorViewModel(ValueMapScaleHelper.GetGradientColor(gradientPalette.StartColor, gradientPalette.EndColor, i, rangesCount)));
			}
			else {
				CustomPalette customPalette = palette as CustomPalette;
				if(customPalette != null)
					Colors = customPalette.Colors.Select(color => new ColorViewModel(color)).ToList();
			}
			if(Colors != null && Colors.Count > 0) {
				if(Colors.Count > rangesCount)
					Colors.RemoveRange(rangesCount, Colors.Count - rangesCount);
				if(Colors.Count < rangesCount) {
					int countToAdd = rangesCount - Colors.Count;
					for(int i = 0; i < countToAdd; i++)
						Colors.Add(Colors[Colors.Count - 1]);
				}
			}
		}
		public override bool Equals(object obj) {
			MapColorizerViewModel colorizer = obj as MapColorizerViewModel;
			if(colorizer == null)
				return false;
			return UsePercentRangeStops == colorizer.UsePercentRangeStops && Helper.DataEquals(RangeStops, colorizer.RangeStops) && Helper.DataEquals(Colors, colorizer.Colors);
		}
		public override int GetHashCode() {
			int hashCode = UsePercentRangeStops.GetHashCode();
			if(RangeStops != null)
				hashCode ^= Helper.GetDataHashCode(RangeStops);
			if(Colors != null)
				hashCode ^= Helper.GetDataHashCode(Colors);
			return hashCode;
		}
	}
	public class ColorViewModel {
		public byte R { get; set; }
		public byte G { get; set; }
		public byte B { get; set; }
		public ColorViewModel(Color color) {
			R = color.R;
			G = color.G;
			B = color.B;
		}
		public override bool Equals(object obj) {
			ColorViewModel color = obj as ColorViewModel;
			if(color == null)
				return false;
			return R == color.R && G == color.G && B == color.B;
		}
		public override int GetHashCode() {
			return R ^ G ^ B;
		}
	}
	public abstract class ChoroplethColorizerViewModel {
		public string AttributeName { get; set; }
		protected ChoroplethColorizerViewModel() {
		}
		protected ChoroplethColorizerViewModel(string attributeName)
			: base() {
			AttributeName = attributeName;
		}
		public virtual bool ShouldUpdateGeometry(ChoroplethColorizerViewModel viewModel) {
			return viewModel == null || AttributeName != viewModel.AttributeName;
		}
		public virtual bool ShouldUpdateLegend(ChoroplethColorizerViewModel viewModel) {
			return false;
		}
	}
	public class MapValueColorizerViewModel : ChoroplethColorizerViewModel {
		public string ValueName { get; set; }
		public string ValueId { get; set; }
		public MapColorizerViewModel Colorizer { get; set; }
		public MapValueColorizerViewModel()
			: base() {
		}
		public MapValueColorizerViewModel(ChoroplethMapDashboardItem dashboardItem, ValueMap map)
			: base(dashboardItem.AttributeName) {
			ValueName = map.ValueDisplayName;
			ValueId = map.Value.ActualId;
			Colorizer = new MapColorizerViewModel(map.Palette, map.Scale);
		}
		public override bool ShouldUpdateGeometry(ChoroplethColorizerViewModel viewModel) {
			MapValueColorizerViewModel model = viewModel as MapValueColorizerViewModel;
			return base.ShouldUpdateGeometry(viewModel) || model == null || ValueName != model.ValueName ||
				Colorizer != null && !Colorizer.Equals(model.Colorizer) || Colorizer == null && model.Colorizer != null;
		}
		public override bool ShouldUpdateLegend(ChoroplethColorizerViewModel viewModel) {
			return true; 
		}
	}
	public class MapDeltaColorizerViewModel : ChoroplethColorizerViewModel {
		public string ActualValueName { get; set; }
		public string DeltaValueName { get; set; }
		public string DeltaValueId { get; set; }
		public DeltaValueType DeltaValueType { get; set; }
		public MapDeltaColorizerViewModel()
			: base() {
		}
		public MapDeltaColorizerViewModel(ChoroplethMapDashboardItem dashboardItem, DeltaMap map)
			: base(dashboardItem.AttributeName) {
			ActualValueName = map.ActualValueDisplayName;
			Measure actualValue = map.ActualValue;
			DeltaValueId = actualValue.ActualId;
			DeltaValueName = map.DeltaDisplayName;
			DeltaValueType valueType = map.DeltaOptions.ValueType;
			DeltaValueType = map.DeltaOptions.ValueType;
		}
		public override bool ShouldUpdateGeometry(ChoroplethColorizerViewModel viewModel) {
			MapDeltaColorizerViewModel model = viewModel as MapDeltaColorizerViewModel;
			return base.ShouldUpdateGeometry(viewModel) || model == null || ActualValueName != model.ActualValueName || DeltaValueName != model.DeltaValueName;
		}
	}
}
