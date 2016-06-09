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

using System.Collections.Generic;
using DevExpress.Sparkline;
using DevExpress.Sparkline.Core;
using System;
namespace DevExpress.ChartRangeControlClient.Core {
	public interface IClientSeries : ISparklineExtendedData, ISparklineSettings { }
	public delegate void DataChangedDelegate(IClientDataProvider provider);
	public interface IClientDataProvider {
		IList<IClientSeries> Series { get; }
		void SetDataChangedDelegate(DataChangedDelegate dataChangedDelegate);
	}
	public interface IBindingSourceDelegate {
		void BindingChanged();
		void AdjustSeries(object dataSourceValue, BindingSourceClientSeries series, int seriesCounter);
	}
	public interface IChartCoreClientDelegate {
		void InteractionUpdated();
	}
	public interface IChartCoreClientGridOptions {
		bool Auto { get; }
		GridUnit GridUnit { get; }
		GridUnit SnapUnit { get; }
		double PixelPerUnit { get; }
		IChartCoreClientGridMapping GridMapping { get; }
	}
	public interface IChartCoreClientGridMapping {
		GridUnit SelectGridUnit(double unit);
		double CeilValue(GridUnit unit, double value);
		double FloorValue(GridUnit unit, double value);
		double GetGridValue(GridUnit unit, double index);
	}
}
