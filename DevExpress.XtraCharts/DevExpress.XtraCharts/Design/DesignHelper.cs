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
using System.Reflection;
using DevExpress.Data.Native;
using DevExpress.XtraCharts.Native;
using Microsoft.Win32;
namespace DevExpress.XtraCharts.Design {
	public class ChartDesignHelper {
		static void SelectHitTestable(IChartContainer container, ChartElement element) {
			for (ChartElement elem = element; elem != null; elem = elem.Owner) {
				IHitTest hitTest = elem as IHitTest;
				if (hitTest != null) {
					SelectObject(container.Chart, elem);
					return;
				}
			}
		}
		public static void SelectHitTestable(IChartContainer container, ChartCollectionBase collection) {
			SelectHitTestable(container, collection.Owner);
		}
		public static void SelectOwnerHitTestable(IChartContainer container, ChartElement element) {
			SelectHitTestable(container, element.Owner);
		}
		public static T GetOwner<T>(ChartElement element) where T : ChartElement {
			if (element == null)
				return default(T);
			ChartElement owner = element;
			do {
				owner = owner.Owner;
				if (owner is T)
					return (T)owner;
			}
			while (owner != null);
			return null;
		}
 		public static object GetField(Type type, object obj, string name) {
			FieldInfo fi = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
			if (fi == null)
				throw new MemberAccessException(name);
			return fi.GetValue(obj);
		}
		public static object InvokeMethod(object obj, string name, object[] parameters) {
			MethodInfo mi = obj.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance);
			if (mi == null)
				throw new MemberAccessException(name);
			return mi.Invoke(obj, parameters);		
		}
		public static void SelectObject(Chart chart, object obj) {
			if (chart == null)
				return;
			if (obj == null || obj == chart || !chart.Contains(obj)) {
				chart.SelectHitElement(chart);
				obj = chart.Container;
			}
			else
				chart.SelectHitElement((IHitTest)obj);
			chart.ContainerAdapter.OnObjectSelected(new HotTrackEventArgs(obj, null, null));
		}
		public static void PopulateDataSource(Chart chart) {
			using (VS2005ConnectionStringHelper helper = new VS2005ConnectionStringHelper()) {
				helper.PatchConnectionString(chart.DataContainer.DataAdapter);
				chart.FillDataSource();
			}
		}
		public static void InitializeDefaultGanttScaleType(SeriesBase series) {
			if(series.View is GanttSeriesView)
				series.ValueScaleType = ScaleType.DateTime;
		}
	}
}
