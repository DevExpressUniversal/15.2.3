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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region DataPoint
	public class DataPoint : ISupportsCopyFrom<DataPoint>, ISupportsInvertIfNegative {
		#region Fields
		readonly IChart parent;
		int index;
		bool bubble3D;
		bool invertIfNegative;
		int explosion;
		Marker marker;
		PictureOptions pictureOptions;
		ShapeProperties shapeProperties;
		#endregion
		public DataPoint(IChart parent, int index) {
			Guard.ArgumentNotNull(parent, "parent");
			Guard.ArgumentNonNegative(index, "index");
			this.parent = parent;
			this.index = index;
			this.bubble3D = false;
			this.invertIfNegative = false;
			this.explosion = -1;
			this.marker = new Marker(parent);
			this.pictureOptions = new PictureOptions(parent);
			this.shapeProperties = new ShapeProperties(DocumentModel) { Parent = parent };
		}
		#region Properties
		protected internal IChart Parent { get { return parent; } }
		protected internal DocumentModel DocumentModel { get { return parent.DocumentModel; } }
		#region Index
		public int Index { get { return index; } }
		protected internal void SetIndexCore(int value) {
			this.index = value;
		}
		#endregion
		#region Bubble3D
		public bool Bubble3D {
			get { return bubble3D; }
			set {
				if(bubble3D == value)
					return;
				SetBubble3D(value);
			}
		}
		void SetBubble3D(bool value) {
			DataPointBubble3DPropertyChangedHistoryItem historyItem = new DataPointBubble3DPropertyChangedHistoryItem(DocumentModel, this, bubble3D, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetBubble3DCore(bool value) {
			this.bubble3D = value;
			Parent.Invalidate();
		}
		#endregion
		#region InvertIfNegative
		public bool InvertIfNegative {
			get { return invertIfNegative; }
			set {
				if(invertIfNegative == value)
					return;
				SetInvertIfNegative(value);
			}
		}
		void SetInvertIfNegative(bool value) {
			InvertIfNegativePropertyChangedHistoryItem historyItem = new InvertIfNegativePropertyChangedHistoryItem(DocumentModel, this, invertIfNegative, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetInvertIfNegativeCore(bool value) {
			this.invertIfNegative = value;
			Parent.Invalidate();
		}
		#endregion
		public bool HasExplosion { 
			get { return explosion >= 0; }
			set {
				if (HasExplosion == value)
					return;
				SetExplosion(value ? 0 : -1);
			}
		}
		#region Explosion
		public int Explosion {
			get {
				return explosion < 0 ? 0 : explosion; 
			}
			set {
				Guard.ArgumentNonNegative(value, "Explosion");
				if(explosion == value)
					return;
				SetExplosion(value);
			}
		}
		void SetExplosion(int value) {
			DataPointExplosionPropertyChangedHistoryItem historyItem = new DataPointExplosionPropertyChangedHistoryItem(DocumentModel, this, explosion, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetExplosionCore(int value) {
			this.explosion = value;
			Parent.Invalidate();
		}
		#endregion
		public Marker Marker { get { return marker; } }
		public PictureOptions PictureOptions { get { return pictureOptions; } }
		public ShapeProperties ShapeProperties { get { return shapeProperties; } }
		#endregion
		#region ISupportsCopyFrom<DataPoint> Members
		public void CopyFrom(DataPoint value) {
			Guard.ArgumentNotNull(value, "value");
			Bubble3D = value.Bubble3D;
			InvertIfNegative = value.InvertIfNegative;
			Explosion = value.Explosion;
			this.marker.CopyFrom(value.marker);
			this.pictureOptions.CopyFrom(value.pictureOptions);
			this.shapeProperties.CopyFrom(value.shapeProperties);
		}
		#endregion
		public void ResetToStyle(MarkerStyle markerSymbol, bool keepOutline) {
			ShapeProperties.ResetToStyle(keepOutline);
			Marker.ResetToStyle(markerSymbol);
		}
	}
	#endregion
	#region DataPointCollection
	public class DataPointCollection : ChartUndoableCollectionSupportsCopyFrom<DataPoint> {
		public DataPointCollection(IChart parent)
			: base(parent) {
		}
		public DataPoint FindByIndex(int index) {
			foreach (DataPoint point in InnerList)
				if (point.Index == index)
					return point;
			return null;
		}
		public DataPoint CreateDataPoint(int index) {
			DataPoint result = FindByIndex(index);
			if (result == null) {
				result = new DataPoint(Parent, index);
				Add(result);
			}
			return result;
		}
		public void RemoveByIndex(int index) {
			for (int i = 0; i < InnerList.Count; i++) {
				if (InnerList[i].Index == index) {
					RemoveAt(i);
					return;
				}
			}
		}
		protected override DataPoint CreateNewItem(DataPoint source) {
			DataPoint result = new DataPoint(Parent, source.Index);
			result.CopyFrom(source);
			return result;
		}
	}
	#endregion
}
