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
	public class ConstantLineCollection : ChartElementNamedCollection, IEnumerable<IConstantLine> {
		protected override string NamePrefix { 
			get { return ChartLocalizer.GetString(ChartStringId.ConstantLinePrefix); } 
		}
		internal Axis2D Axis { 
			get { return (Axis2D)base.Owner; } 
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("ConstantLineCollectionItem")]
#endif
		public ConstantLine this[int index] {
			get { return (ConstantLine)List[index]; } 
		}
		internal ConstantLineCollection(Axis2D axis)
			: base(axis) {
		}
		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<IConstantLine>)this).GetEnumerator();
		}
		#endregion
		#region IEnumerable<IConstantLine> Implementation
		IEnumerator<IConstantLine> IEnumerable<IConstantLine>.GetEnumerator() {
			foreach (IConstantLine constantLine in this)
				yield return constantLine;
		}
		#endregion
		void UpdateAxisValue(ConstantLine constantLine) {
			if (Axis != null)
				AxisValueContainerHelper.UpdateAxisValue(constantLine, Axis.ScaleTypeMap);
		}
		protected override ChartUpdateInfoBase CreateUpdateInfo(ChartCollectionOperation operation, object oldItem, int oldIndex, object newItem, int newIndex) {
			return new AxisElementUpdateInfo(this, Axis);
		}
		protected override ChartUpdateInfoBase CreateBatchUpdateInfo(ChartCollectionOperation operation, ICollection oldItems, int oldIndex, ICollection newItems, int newIndex) {
			return new AxisElementUpdateInfo(this, Axis);
		}
		internal IList<ConstantLine> GetSubsetByShowBehindProperty(bool showBehind) {
			List<ConstantLine> list = new List<ConstantLine>();
			foreach (ConstantLine constantLine in this)
				if (constantLine.ShowBehind == showBehind)
					list.Add(constantLine);
			return list;
		}
		internal IList<ILegendItemData> GetLegendItems() {
			List<ILegendItemData> itemsList = new List<ILegendItemData>();
			foreach (ConstantLine line in this)
				if (line.ActualShowInLegend)
					itemsList.Add(line);
			return itemsList;
		}
		public int Add(ConstantLine constantLine) {
			UpdateAxisValue(constantLine);
			return base.Add(constantLine);
		}
		public void AddRange(ConstantLine[] coll) {
			foreach (ConstantLine line in coll)
				UpdateAxisValue(line);
			base.AddRange(coll);
		}
		public void Remove(ConstantLine constantLine) {
			base.Remove(constantLine);
		}
		public void Insert(int index, ConstantLine constantLine) {
			UpdateAxisValue(constantLine);
			base.Insert(index, constantLine);
		}
		public bool Contains(ConstantLine constantLine) {
			return base.Contains(constantLine);
		}
		public ConstantLine GetConstantLineByName(string name) {
			return (ConstantLine)base.GetElementByName(name);
		}
	}
}
