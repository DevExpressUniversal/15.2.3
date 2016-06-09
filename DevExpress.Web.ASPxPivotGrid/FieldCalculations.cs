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

using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.PivotGrid.Internal;
using System.Collections.Generic;
namespace DevExpress.Web.ASPxPivotGrid {
	public class PivotCalculationTargetCellInfo : DevExpress.PivotGrid.PivotCalculationTargetCellInfoBase {
		public PivotCalculationTargetCellInfo(PivotGridField field, PivotGridField crossAreaField)
			: base(field, crossAreaField) { }
		public PivotCalculationTargetCellInfo(PivotGridField field)
			: base(field) { }
		public PivotCalculationTargetCellInfo() { }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new PivotGridField ColumnField {
			get { return (PivotGridField)base.ColumnField; }
			set { base.ColumnField = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new PivotGridField RowField {
			get { return (PivotGridField)base.RowField; }
			set { base.RowField = value; }
		}
	}
	public class PivotRunningTotalCalculation : DevExpress.PivotGrid.PivotRunningTotalCalculationBase {
		PivotCalculationTargetCellCollection targetCells;
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)
		]
		public PivotCalculationTargetCellCollection TargetCells {
			get { return targetCells; }
		}
		protected override IList<DevExpress.PivotGrid.PivotCalculationTargetCellInfoBase> CreateTargetCells() {
			targetCells = new PivotCalculationTargetCellCollection();
			targetCells.Changed += OnCellsChanged;
			return new SafeToBaseIListWrapper<PivotCalculationTargetCellInfo, DevExpress.PivotGrid.PivotCalculationTargetCellInfoBase>(targetCells);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateTargetCellsItem(XtraItemEventArgs e) {
			PivotCalculationTargetCellInfo res = new PivotCalculationTargetCellInfo();
			TargetCells.Add(res);
			return res;
		}
	}
	public class PivotVariationCalculation : DevExpress.PivotGrid.PivotVariationCalculationBase {
		PivotCalculationTargetCellCollection targetCells;
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)
		]
		public PivotCalculationTargetCellCollection TargetCells {
			get { return targetCells; }
		}
		protected override IList<DevExpress.PivotGrid.PivotCalculationTargetCellInfoBase> CreateTargetCells() {
			targetCells = new PivotCalculationTargetCellCollection();
			targetCells.Changed += OnCellsChanged;
			return new SafeToBaseIListWrapper<PivotCalculationTargetCellInfo, DevExpress.PivotGrid.PivotCalculationTargetCellInfoBase>(targetCells);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateTargetCellsItem(XtraItemEventArgs e) {
			PivotCalculationTargetCellInfo res = new PivotCalculationTargetCellInfo();
			TargetCells.Add(res);
			return res;
		}
	}
	public class PivotPercentageCalculation : DevExpress.PivotGrid.PivotPercentageCalculationBase {
		PivotCalculationTargetCellCollection targetCells;
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)
		]
		public PivotCalculationTargetCellCollection TargetCells {
			get { return targetCells; }
		}
		protected override IList<DevExpress.PivotGrid.PivotCalculationTargetCellInfoBase> CreateTargetCells() {
			targetCells = new PivotCalculationTargetCellCollection();
			targetCells.Changed += OnCellsChanged;
			return new SafeToBaseIListWrapper<PivotCalculationTargetCellInfo, DevExpress.PivotGrid.PivotCalculationTargetCellInfoBase>(targetCells);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateTargetCellsItem(XtraItemEventArgs e) {
			PivotCalculationTargetCellInfo res = new PivotCalculationTargetCellInfo();
			TargetCells.Add(res);
			return res;
		}
	}
	public class PivotCalculationTargetCellCollection : DevExpress.PivotGrid.PivotCalculationTargetCellCollectionBase<PivotCalculationTargetCellInfo> {
	}
	public class PivotRankCalculation : DevExpress.PivotGrid.PivotRankCalculationBase {
		PivotCalculationTargetCellCollection targetCells;
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)
		]
		public PivotCalculationTargetCellCollection TargetCells {
			get { return targetCells; }
		}
		protected override IList<DevExpress.PivotGrid.PivotCalculationTargetCellInfoBase> CreateTargetCells() {
			targetCells = new PivotCalculationTargetCellCollection();
			targetCells.Changed += OnCellsChanged;
			return new SafeToBaseIListWrapper<PivotCalculationTargetCellInfo, DevExpress.PivotGrid.PivotCalculationTargetCellInfoBase>(targetCells);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateTargetCellsItem(XtraItemEventArgs e) {
			PivotCalculationTargetCellInfo res = new PivotCalculationTargetCellInfo();
			TargetCells.Add(res);
			return res;
		}
	}
	public class PivotIndexCalculation : DevExpress.PivotGrid.PivotIndexCalculationBase { }
	class PivotCalculationCreator : DevExpress.PivotGrid.IPivotCalculationCreator {
		#region IPivotCalculationCreator Members
		public DevExpress.PivotGrid.PivotIndexCalculationBase CreateIndexCalculation() {
			return new PivotIndexCalculation();
		}
		public DevExpress.PivotGrid.PivotPercentageCalculationBase CreatePercentageCalculation() {
			return new PivotPercentageCalculation();
		}
		public DevExpress.PivotGrid.PivotRankCalculationBase CreateRankCalculation() {
			return new PivotRankCalculation();
		}
		public DevExpress.PivotGrid.PivotRunningTotalCalculationBase CreateRunningTotalCalculation() {
			return new PivotRunningTotalCalculation();
		}
		public DevExpress.PivotGrid.PivotVariationCalculationBase CreateVariationCalculation() {
			return new PivotVariationCalculation();
		}
		#endregion
	}
}
