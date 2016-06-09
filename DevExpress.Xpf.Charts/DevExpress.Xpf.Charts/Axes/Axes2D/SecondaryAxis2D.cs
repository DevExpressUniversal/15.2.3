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
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using System.Collections.Specialized;
using System.Collections;
namespace DevExpress.Xpf.Charts {
	public class SecondaryAxisX2D : AxisX2D {
		protected override int LayoutPriority { get { return (int)ChartElementVisibilityPriority.AxisX; } }
		public SecondaryAxisX2D() {
			DefaultStyleKey = typeof(SecondaryAxisX2D);
		}		
	}
	public class SecondaryAxisY2D : AxisY2D {
		protected override int LayoutPriority { get { return (int)ChartElementVisibilityPriority.AxisY; } }
		public SecondaryAxisY2D() {
			DefaultStyleKey = typeof(SecondaryAxisY2D);
		}
	}
	public class SecondaryAxisXCollection : ChartElementCollection<SecondaryAxisX2D>, IEnumerable<IAxisData> {
		protected override ChartElementChange Change { get { return base.Change | ChartElementChange.UpdateXYDiagram2DItems; } }
		IEnumerator<IAxisData> IEnumerable<IAxisData>.GetEnumerator() {
			foreach (IAxisData axis in this)
				yield return axis;
		}
		protected override ChartUpdateInfoBase CreateUpdateInfo(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int newStartingIndex, int oldStartingIndex) {
			if ((newItems != null && newItems.Count > 1) || (oldItems != null && oldItems.Count > 1))
				return new AxisCollectionBatchUpdateInfo(this, GetOperation(action), GetCollection<IAxisData>(oldItems), oldStartingIndex, GetCollection<IAxisData>(newItems), newStartingIndex);
			IAxisData oldPoint = oldItems != null && oldItems.Count > 0 ? oldItems[0] as IAxisData : null;
			IAxisData newPoint = newItems != null && newItems.Count > 0 ? newItems[0] as IAxisData : null;
			return new AxisCollectionUpdateInfo(this, GetOperation(action), oldPoint, oldStartingIndex, newPoint, newStartingIndex);
		}
	}
	public class SecondaryAxisYCollection : ChartElementCollection<SecondaryAxisY2D>, IEnumerable<IAxisData> {
		protected override ChartElementChange Change { get { return base.Change | ChartElementChange.UpdateXYDiagram2DItems; } }
		IEnumerator<IAxisData> IEnumerable<IAxisData>.GetEnumerator() {
			foreach (IAxisData axis in this)
				yield return axis;
		}
		protected override ChartUpdateInfoBase CreateUpdateInfo(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int newStartingIndex, int oldStartingIndex) {
			if ((newItems != null && newItems.Count > 1) || (oldItems != null && oldItems.Count > 1))
				return new AxisCollectionBatchUpdateInfo(this, GetOperation(action), GetCollection<IAxisData>(oldItems), oldStartingIndex, GetCollection<IAxisData>(newItems), newStartingIndex);
			IAxisData oldPoint = oldItems != null && oldItems.Count > 0 ? oldItems[0] as IAxisData : null;
			IAxisData newPoint = newItems != null && newItems.Count > 0 ? newItems[0] as IAxisData : null;
			return new AxisCollectionUpdateInfo(this, GetOperation(action), oldPoint, oldStartingIndex, newPoint, newStartingIndex);
		}
	}
}
