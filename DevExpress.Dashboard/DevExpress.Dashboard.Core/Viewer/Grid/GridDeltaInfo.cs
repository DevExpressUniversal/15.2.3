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

using System.Drawing;
using DevExpress.DashboardCommon.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
namespace DevExpress.DashboardCommon.Viewer {
	public interface IDeltaColors {
		Color ActualValueColor { get; }
		Color Bad { get; }
		Color Good { get; }
		Color Neutral { get; }
		Color Warning { get; }
		Color GetActualValueColor(AppearanceObject appearanceObject, ObjectState objectState);
	}
	public class GridDeltaInfo {
		public const int ImageWidth = 22;
		public const int ImageHeight = 16;
		readonly IDeltaColors colors;
		readonly IndicatorPresenter presenter;
		public GridDeltaInfo(IDeltaColors colors) {
			this.colors = colors;
			this.presenter = new IndicatorPresenter(colors.Good, colors.Bad, colors.Warning);
		}
		public Image GetImage(IndicatorType? indicatorType, bool deltaIsGood) {
			return indicatorType != null && indicatorType != IndicatorType.None ? presenter.GetImage(new Size(ImageWidth, ImageHeight), (IndicatorType)indicatorType, deltaIsGood) : null;
		}
		public Color GetTextColor(IndicatorType? indicatorType, bool deltaIsGood, bool ignoreDeltaColor, AppearanceObject cellAppearance, ObjectState cellState) {
			if (indicatorType == null || ignoreDeltaColor)
				return colors.GetActualValueColor(cellAppearance, cellState);
			if(indicatorType == IndicatorType.None)
				return colors.Neutral;
			if(indicatorType == IndicatorType.Warning)
				return colors.Warning;
			return deltaIsGood ? colors.Good : colors.Bad;
		}
	}
}
