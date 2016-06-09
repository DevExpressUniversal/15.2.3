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

using System.Drawing;
using DevExpress.Data.Browsing;
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Snap.Core.Native.Data;
namespace DevExpress.Snap.Core.Native.LayoutUI {
	public interface IChartDropZoneVisitor : IHotZoneVisitor {
		void Visit(DropValuesHotZone hotZone);
		void Visit(DropArgumentsHotZone hotZone);
	}
	public abstract class DropFieldChartHotZone : DropFieldRoundHotZone {
		protected DropFieldChartHotZone(PointF center, float radius)
			: base(center, radius) {
		}
		protected override void AcceptCore(IHotZoneVisitor visitor) {
			Accept((IChartDropZoneVisitor)visitor);
		}
		public void DoDragDrop(IDataObject data, SNChart chart) {
			SNDataInfo[] dataInfo = SnapDataHelper.GetDataInfo(data);
			if (dataInfo == null) return;
			DropDataInfo(dataInfo, chart);
		}
		protected abstract void DropDataInfo(SNDataInfo[] dataInfo, SNChart chart);
		protected abstract void Accept(IChartDropZoneVisitor visitor);
	}
	public class DropValuesHotZone : DropFieldChartHotZone {
		public DropValuesHotZone(PointF center, float radius)
			: base(center, radius) {
		}
		protected override void Accept(IChartDropZoneVisitor visitor) {
			visitor.Visit(this);
		}
		protected override void DropDataInfo(SNDataInfo[] dataInfo, SNChart chart) {
			chart.AddValues(dataInfo);
		}
	}
	public class DropArgumentsHotZone : DropFieldChartHotZone {
		public DropArgumentsHotZone(PointF center, float radius)
			: base(center, radius) {
		}
		protected override void Accept(IChartDropZoneVisitor visitor) {
			visitor.Visit(this);
		}
		protected override void DropDataInfo(SNDataInfo[] dataInfo, SNChart chart) {
			chart.AddArgument(dataInfo[0]);
		}
	}
}
