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
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Office;
namespace DevExpress.Spreadsheet.Charts {
	public interface AxisCollection : ISimpleCollection<Axis> {
		bool Contains(Axis axis);
		int IndexOf(Axis axis);
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	#region NativeAxisCollection
	partial class NativeAxisCollection : NativeChartCollectionBase<Axis, NativeAxisBase, Model.AxisBase>, AxisCollection {
		readonly NativeAxisFactory factory;
		public NativeAxisCollection(Model.AxisGroup modelCollection, NativeWorkbook nativeWorkbook)
			: base(modelCollection) {
			this.factory = new NativeAxisFactory(nativeWorkbook);
		}
		protected override NativeAxisBase CreateNativeObject(Model.AxisBase modelItem) {
			return factory.Create(modelItem);
		}
		public bool Contains(Axis item) {
			return IndexOf(item) != -1;
		}
		public int IndexOf(Axis item) {
			CheckValid();
			NativeAxisBase nativeItem = item as NativeAxisBase;
			if (nativeItem == null || !nativeItem.IsValid)
				return -1;
			return InnerList.IndexOf(nativeItem);
		}
	}
	#endregion
	#region NativeAxisFactory
	partial class NativeAxisFactory : Model.IAxisVisitor {
		readonly NativeWorkbook nativeWorkbook;
		NativeAxisBase nativeAxis;
		public NativeAxisFactory(NativeWorkbook nativeWorkbook) {
			this.nativeWorkbook = nativeWorkbook;
		}
		public NativeAxisBase Create(Model.AxisBase axis) {
			axis.Visit(this);
			return nativeAxis;
		}
		#region IAxisVisitor Members
		public void Visit(Model.CategoryAxis axis) {
			nativeAxis = new NativeCategoryAxis(axis, nativeWorkbook);
		}
		public void Visit(Model.ValueAxis axis) {
			nativeAxis = new NativeValueAxis(axis, nativeWorkbook);
		}
		public void Visit(Model.DateAxis axis) {
			nativeAxis = new NativeDateAxis(axis, nativeWorkbook);
		}
		public void Visit(Model.SeriesAxis axis) {
			nativeAxis = new NativeSeriesAxis(axis, nativeWorkbook);
		}
		#endregion
	}
	#endregion
}
