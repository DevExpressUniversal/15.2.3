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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[DesignerSerializer("DevExpress.XtraCharts.Design.ChartCollectionSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					  "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")]
	public class StripCollection : ChartElementNamedCollection, IEnumerable<IStrip> {
		protected override string NamePrefix { 
			get { return ChartLocalizer.GetString(ChartStringId.StripPrefix); } 
		}
		internal Axis2D Axis { 
			get { return (Axis2D)base.Owner; } 
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("StripCollectionItem")]
#endif
		public Strip this[int index] { 
			get { return (Strip)List[index]; }
		}
		internal StripCollection(Axis2D axis)
			: base(axis) {
		}
		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<IStrip>)this).GetEnumerator();
		}
		#endregion
		#region IEnumerable<IStrip> implementation
		IEnumerator<IStrip> IEnumerable<IStrip>.GetEnumerator() {
			foreach (IStrip strip in this)
				yield return strip;
		}
		#endregion
		void CheckStripLimits(Strip strip) {
			if (Axis != null)
				AxisValueContainerHelper.UpdateAxisValue(strip.MinLimit, strip.MaxLimit, Axis.ScaleTypeMap);
			if (strip.MinLimit.AxisValue != null && strip.MaxLimit.AxisValue != null)
				strip.CorrectLimits(Axis);
		}
		protected override ChartUpdateInfoBase CreateUpdateInfo(ChartCollectionOperation operation, object oldItem, int oldIndex, object newItem, int newIndex) {
			return new AxisElementUpdateInfo(this, Axis);
		}
		protected override ChartUpdateInfoBase CreateBatchUpdateInfo(ChartCollectionOperation operation, ICollection oldItems, int oldIndex, ICollection newItems, int newIndex) {
			return new AxisElementUpdateInfo(this, Axis);
		}
		internal IList<ILegendItemData> GetLegendItems() {
			List<ILegendItemData> itemsList = new List<ILegendItemData>();
			foreach (Strip strip in this)
				if (strip.ShowInLegend && (CustomAxisElementsHelper.IsStripVisible(Axis.ScaleTypeMap, strip) || 
					Axis.ChartContainer.Chart.Legend.UseCheckBoxes && strip.Visible && strip.CheckableInLegend))
					itemsList.Add(strip);
			return itemsList;
		}
		public int Add(Strip strip) {
			CheckStripLimits(strip);
			return base.Add(strip);
		}
		public void AddRange(Strip[] coll) {
			foreach (Strip strip in coll)
				CheckStripLimits(strip);
			base.AddRange(coll);
		}
		public void Remove(Strip strip) {
			base.Remove(strip);
		}
		public void Insert(int index, Strip strip) {
			CheckStripLimits(strip);
			base.Insert(index, strip);
		}
		public bool Contains(Strip strip) {
			return base.Contains(strip);
		}
		public Strip GetStripByName(string name) {
			return (Strip)base.GetElementByName(name);
		}
	}
}
