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
	public interface DataPoint : ShapeFormat {
		int Index { get; }
		bool Bubble3D { get; set; }
		bool InvertIfNegative { get; set; }
		int Explosion { get; set; }
		Marker Marker { get; }
	}
	public interface DataPointCollection : ISimpleCollection<DataPoint> {
		DataPoint Add(int itemIndex);
		bool Remove(DataPoint dataPoint);
		void RemoveAt(int index);
		void Clear();
		bool Contains(DataPoint dataPoint);
		int IndexOf(DataPoint dataPoint);
		DataPoint FindByIndex(int itemIndex);
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.Office.API.Internal;
	#region NativeDataPoint
	partial class NativeDataPoint : NativeShapeFormat, DataPoint, ISupportIndex {
		#region Fields
		readonly Model.DataPoint modelDataPoint;
		NativeMarker nativeMarker;
		#endregion
		public NativeDataPoint(Model.DataPoint modelDataPoint, NativeWorkbook nativeWorkbook)
			: base(modelDataPoint.ShapeProperties, nativeWorkbook) {
			this.modelDataPoint = modelDataPoint;
		}
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (nativeMarker != null)
				nativeMarker.IsValid = value; 
		}
		#region DevExpress.Spreadsheet.DataPoint Members
		public int Index {
			get {
				CheckValid();
				return modelDataPoint.Index;
			}
		}
		public bool Bubble3D {
			get {
				CheckValid();
				return modelDataPoint.Bubble3D; 
			}
			set {
				CheckValid();
				modelDataPoint.Bubble3D = value; 
			}
		}
		public bool InvertIfNegative {
			get {
				CheckValid();
				return modelDataPoint.InvertIfNegative; 
			}
			set {
				CheckValid();
				modelDataPoint.InvertIfNegative = value; 
			}
		}
		public int Explosion {
			get {
				CheckValid();
				return modelDataPoint.Explosion;
			}
			set {
				CheckValid();
				modelDataPoint.Explosion = value;
			}
		}
		public Marker Marker {
			get {
				CheckValid();
				if (nativeMarker == null)
					nativeMarker = new NativeMarker(modelDataPoint.Marker, NativeWorkbook);
				return nativeMarker;
			}
		}
		#endregion
	}
	#endregion
	#region NativeDataPointCollection
	partial class NativeDataPointCollection : NativeChartIndexedCollectionBase<DataPoint, NativeDataPoint, Model.DataPoint, Model.DataPointCollection>, DataPointCollection {
		public NativeDataPointCollection(Model.DataPointCollection modelCollection, NativeWorkbook nativeWorkbook)
			: base(modelCollection, nativeWorkbook) {
		}
		#region NativeChartUndoableCollectionBase<DataPoint, NativeDataPoint, Model.DataPoint, Model.DataPointCollection> Members
		protected override NativeDataPoint CreateNativeObject(Model.DataPoint modelItem) {
			return new NativeDataPoint(modelItem, NativeWorkbook);
		}
		protected override Model.DataPoint CreateModelObject(int modelItemIndex) {
			return new Model.DataPoint(ModelChartCollection.Parent, modelItemIndex);
		}
		#endregion
	}
	#endregion 
}
