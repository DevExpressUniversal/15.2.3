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

using System.Text;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public abstract class ChartAxisSettings {
		readonly ChartAxis axis;
		readonly bool reverse;
		readonly bool titleVisible;
		readonly string title;
		readonly bool visible;
		protected ChartAxis ChartAxis { get { return axis; } }
		protected ChartAxisSettings(ChartAxis axis) {
			this.axis = axis;
			reverse = axis.Reverse;
			visible = axis.Visible;
			titleVisible = axis.TitleVisible;
			title = axis.Title;
		}
		public virtual void Apply() {
			axis.Reverse = reverse;
			axis.Visible = visible;
			axis.TitleVisible = titleVisible;
			axis.Title = title;
		}
	}
	public class ChartAxisXSettings : ChartAxisSettings {
		readonly bool enableZooming;
		readonly bool limitVisiblePoints;
		readonly int visiblePointsCount;
		public ChartAxisX ChartAxisX { get { return (ChartAxisX)ChartAxis; } }
		public bool IsContinuousArgumentScale { get; private set; }
		public ChartAxisXSettings(ChartAxisX axis, bool isContinuousArgumentScale)
			: base(axis) {
			enableZooming = axis.EnableZooming;
			limitVisiblePoints = axis.LimitVisiblePoints;
			visiblePointsCount = axis.VisiblePointsCount;
			IsContinuousArgumentScale = isContinuousArgumentScale;
		}
		public override void Apply() {
			base.Apply();
			ChartAxisX.EnableZooming = enableZooming;
			ChartAxisX.LimitVisiblePoints = limitVisiblePoints;
			ChartAxisX.VisiblePointsCount = visiblePointsCount;
		}
	}
	public class ChartAxisYSettings : ChartAxisSettings {
		readonly bool alwaysShowZeroLevel;
		readonly bool logarithmic;
		readonly LogarithmicBase logarithmicBase;
		readonly bool showAxisType;
		readonly string name;
		protected virtual string AxisTypeName { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.ChartPrimaryAxisTypeName); } }
		protected string Name { get { return name; } }
		public ChartAxisY ChartAxisY { get { return (ChartAxisY)ChartAxis; } }
		public ChartAxisYSettings(string name, ChartAxisY axisY, bool showAxisType)
			: base(axisY) {
			alwaysShowZeroLevel = axisY.AlwaysShowZeroLevel;
			logarithmic = axisY.Logarithmic;
			logarithmicBase = axisY.LogarithmicBase;
			this.showAxisType = showAxisType;
			this.name = name;
		}
		public override void Apply() {
			base.Apply();
			ChartAxisY.AlwaysShowZeroLevel = alwaysShowZeroLevel;
			ChartAxisY.Logarithmic = logarithmic;
			ChartAxisY.LogarithmicBase = logarithmicBase;
		}
		public override string ToString() {
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Name);
			if(showAxisType)
				stringBuilder.Append(string.Format(" ({0})", AxisTypeName));
			return stringBuilder.ToString();
		}
		public virtual ChartAxisYSettings GenerateAxisYSettings(ChartAxisY axisY) {
			return new ChartAxisYSettings(name, axisY, false);
		}
	}
	public class ChartSecondaryAxisYSettings : ChartAxisYSettings {
		protected override string AxisTypeName { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.ChartSecondaryAxisTypeName); } }
		public ChartSecondaryAxisYSettings(string name, ChartAxisY axisY)
			: base(name, axisY, true) {
		}
		public override ChartAxisYSettings GenerateAxisYSettings(ChartAxisY axisY) {
			return new ChartSecondaryAxisYSettings(Name, axisY);
		}
	}
}
