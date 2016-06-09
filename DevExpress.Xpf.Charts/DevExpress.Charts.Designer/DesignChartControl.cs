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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
using System;
namespace DevExpress.Charts.Designer.Native {
	internal class DesignChartControl : Control {
		public static readonly DependencyProperty ChartControlProperty = DependencyPropertyManager.Register("ChartControl", typeof(ChartControl), typeof(DesignChartControl), new PropertyMetadata(ChartControlChanged));
		public static readonly DependencyProperty SelectedObjectProperty = DependencyPropertyManager.Register("SelectedObject", typeof(object), typeof(DesignChartControl), new PropertyMetadata(SelectedObjectChanged));
		static void SelectedObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DesignChartControl designChart = d as DesignChartControl;
			if (designChart != null)
				designChart.SetNewSelectionInChart(e.OldValue as IInteractiveElement, e.NewValue as IInteractiveElement);
		}
		static void ChartControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DesignChartControl designChart = d as DesignChartControl;
			if (designChart != null) {
				designChart.SelectedObject = null;
				designChart.ChartControl.BorderBrush = null;
			}
		}
		public ChartControl ChartControl {
			get { return (ChartControl)GetValue(ChartControlProperty); }
			set { SetValue(ChartControlProperty, value);}
		}
		public Object SelectedObject {
			get { return (object)GetValue(SelectedObjectProperty); }
			set { SetValue(SelectedObjectProperty, value); }
		}
		public DesignChartControl() {
			DefaultStyleKey = typeof(DesignChartControl);
		}
		object GetSelectedObject(ChartHitInfo hitInfo) {
			if (hitInfo.Indicator != null)
				return hitInfo.Indicator;
			if (hitInfo.Legend != null)
				return hitInfo.Legend;
			if (hitInfo.ConstantLine != null)
				return hitInfo.ConstantLine;
			if (hitInfo.Title != null)
				return hitInfo.Title;
			if (hitInfo.Series != null)
				return hitInfo.Series;
			if (hitInfo.Strip != null)
				return hitInfo.Strip;
			if (hitInfo.Axis != null)
				return hitInfo.Axis;
			if (hitInfo.AxisLabel != null)
				return (AxisBase)(((IOwnedElement)hitInfo.AxisLabel).Owner);
			if (hitInfo.Pane != null)
				return hitInfo.Pane;
			if (hitInfo.Diagram != null)
				return hitInfo.Diagram;
			return ChartControl;
		}
		void SetNewSelectionInChart(IInteractiveElement oldSelectedObject, IInteractiveElement newSelectedObject) {
			if (oldSelectedObject != null)
				oldSelectedObject.IsSelected = false;
			if (newSelectedObject != null)
				newSelectedObject.IsSelected = true;
		}
		public override void OnApplyTemplate() {
			if (ChartControl!=null)
				ChartControl.SelectionMode = ElementSelectionMode.None;
		}
		protected override void OnPreviewMouseDown(MouseButtonEventArgs e) {
			base.OnPreviewMouseDown(e);
			ChartHitInfo hitInfo = ChartControl.CalcHitInfo(e.GetPosition(ChartControl), 3.0);
			SelectedObject = GetSelectedObject(hitInfo);
		}	   
	}
}
