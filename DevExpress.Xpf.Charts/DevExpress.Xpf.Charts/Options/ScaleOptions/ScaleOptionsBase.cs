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
using System.Windows;
using DevExpress.Charts.Native;
using DevExpress.Charts.NotificationCenter;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public enum ScaleMode {
		Automatic = ScaleModeNative.Automatic,
		Manual = ScaleModeNative.Manual,
		Continuous = ScaleModeNative.Continuous
	}
	public abstract class ScaleOptionsBase : ChartDependencyObject, IChartElement, IScaleOptionsBase {
		protected static void MeasureUnitsCalculatorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ScaleOptionsBase obj = d as ScaleOptionsBase;
			if (obj != null)
				ChartElementHelper.Update(d, new DataAggregationUpdate(obj.AxisData));
		}
		internal static ValueChangeInfo<D> CreateInfo<D>(D value) {
			return new ValueChangeInfo<D>(value);
		}
		internal static ValueChangeInfo<D> CreateInfo<D>(D oldValue, D newValue) {
			return new ValueChangeInfo<D>(oldValue, newValue);
		}
		internal static ValueChangeInfo<D> GetInfo<D>(ValueChangeInfo<D> info, D currentValue) {
			if (info == null)
				return new ValueChangeInfo<D>(currentValue);
			return info;
		}
		Diagram Diagram {
			get {
				IOwnedElement axis = Axis as IOwnedElement;
				return axis != null ? axis.Owner as Diagram : null;
			}
		}
		protected IAxisData AxisData { get { return owner as IAxisData; } }
		protected AxisBase Axis { get { return owner as AxisBase; } }
		protected ChartControl ChartControl { get { return Diagram != null ? Diagram.ChartControl : null; } }
		protected abstract AggregateFunction AggregateFunctionImp { get; }
		protected abstract ScaleModeNative ScaleModeImp { get; }
		protected abstract bool GridSpasingAutoImp { get; }
		protected abstract double GridSpacingImp { get; set; }
		protected abstract double GridOffsetImp { get; }
		protected internal abstract bool AutoGridImp { get; }
		#region IChartElement
		IChartElement owner;
		IChartElement IOwnedElement.Owner {
			get { return owner; }
			set { owner = value; }
		}
		ViewController INotificationOwner.Controller {
			get { return owner == null ? null : owner.Controller; }
		}
		bool IChartElement.Changed(ChartUpdate updateInfo) {
			if (owner != null)
				return owner.Changed(updateInfo);
			return true;
		}
		void IChartElement.AddChild(object child) {
		}
		void IChartElement.RemoveChild(object child) {
		}
		#endregion
		#region IScaleOptionsBase
		AggregateFunctionNative IScaleOptionsBase.AggregateFunction { get { return (AggregateFunctionNative)AggregateFunctionImp; } }
		ScaleModeNative IScaleOptionsBase.ScaleMode { get { return ScaleModeImp; } }
		ProcessMissingPointsModeNative IScaleOptionsBase.ProcessMissingPoints { get { return ProcessMissingPointsModeNative.Skip; } }
		bool IScaleOptionsBase.GridSpacingAuto { get { return GridSpasingAutoImp; } }
		double IScaleOptionsBase.GridSpacing { get { return GridSpacingImp; } set { GridSpacingImp = value; } }
		double IScaleOptionsBase.GridOffset { get { return GridOffsetImp; } }
		#endregion
		protected IEnumerable<Series> EnumerateSeries(IEnumerable<ISeries> sourceSeries) {
			foreach (ISeries series in sourceSeries) {
				yield return (Series)series;
			}
		}
		internal void SetOwner(IChartElement owner) {
			this.owner = owner;
		}
	}
}
