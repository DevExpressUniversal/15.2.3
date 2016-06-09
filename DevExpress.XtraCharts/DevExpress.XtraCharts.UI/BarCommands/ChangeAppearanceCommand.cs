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
using DevExpress.XtraCharts.Native;
using DevExpress.Utils.Commands;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Native {
	public class ChartAppearanceInfo {
		int chartPaletteBaseColorNumber;
		ChartAppearance chartAppearance;
		public int ChartPaletteBaseColorNumber {
			get { return chartPaletteBaseColorNumber; }
			set { chartPaletteBaseColorNumber = value; }
		}
		public ChartAppearance ChartAppearance {
			get { return chartAppearance; }
			set { chartAppearance = value; }
		}
		public override bool Equals(object obj) {
			ChartAppearanceInfo chartInfo = obj as ChartAppearanceInfo;
			if (chartInfo != null &&
				ChartPaletteBaseColorNumber == chartInfo.ChartPaletteBaseColorNumber &&
				ChartAppearance.Name == chartInfo.ChartAppearance.Name)
				return true;
			else
				return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public ChartAppearanceInfo(ChartAppearance chartAppearance, int chartPaletteBaseColorNumber) {
			this.chartAppearance = chartAppearance;
			this.chartPaletteBaseColorNumber = chartPaletteBaseColorNumber;
		}
	}
}
namespace DevExpress.XtraCharts.Commands {
	public class ChangeAppearanceCommand : ChartCommand {
		public ChangeAppearanceCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartCommandId Id { get { return ChartCommandId.ChangeAppearance; } }
		protected override void ExecuteCore(ICommandUIState state) {
			DefaultValueBasedCommandUIState<ChartAppearanceInfo> uiState = state as DefaultValueBasedCommandUIState<ChartAppearanceInfo>;
			ChartAppearanceInfo info = uiState.Value != null ? uiState.Value as ChartAppearanceInfo : null;
			if (info != null) {
				Chart.Appearance = info.ChartAppearance;
				Chart.PaletteBaseColorNumber = info.ChartPaletteBaseColorNumber;
			}
		}
	}
}
