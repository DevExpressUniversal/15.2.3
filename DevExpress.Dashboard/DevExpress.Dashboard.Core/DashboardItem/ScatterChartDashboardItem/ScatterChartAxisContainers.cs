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

namespace DevExpress.DashboardCommon.Native {
	public abstract class ScatterChartAxisContainer : IChartAxisContainer {
		readonly ScatterChartDashboardItem scatter;
		protected abstract Measure Measure { get; }
		protected ScatterChartDashboardItem Scatter { get { return scatter; } }
		protected ScatterChartAxisContainer(ScatterChartDashboardItem scatter) {
			this.scatter = scatter;
		}
		bool IChartAxisContainer.isReverseRequiredForContinuousScale {
			get { return false; }
		}
		void IChartAxisContainer.OnChanged(ChangeReason reason, object context) {
			scatter.OnChanged(reason, context);
		}
		string IChartAxisContainer.GetTitle(bool isSecondary) {
			return Measure != null ? Measure.DisplayName : string.Empty;
		}
	}
	public class ScatterChartAxisXContainer : ScatterChartAxisContainer {
		protected override Measure Measure {
			get { return Scatter.AxisXMeasure; }
		}
		public ScatterChartAxisXContainer(ScatterChartDashboardItem scatter)
			: base(scatter) {
		}
	}
	public class ScatterChartAxisYContainer : ScatterChartAxisContainer {
		protected override Measure Measure {
			get { return Scatter.AxisYMeasure; }
		}
		public ScatterChartAxisYContainer(ScatterChartDashboardItem scatter)
			: base(scatter) {
		}
	}
}
