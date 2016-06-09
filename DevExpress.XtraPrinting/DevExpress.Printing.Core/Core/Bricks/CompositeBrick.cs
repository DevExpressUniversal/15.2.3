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

using DevExpress.XtraPrinting.Native;
using System;
using System.Collections;
using System.Drawing;
using DevExpress.Utils.Serializing;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.Utils.StoredObjects;
namespace DevExpress.XtraPrinting {
	static class BrickMapConst {
		public const int Graduation = 200;
	}
#if !SL
	[
	BrickExporter(typeof(CompositeBrickExporter))
	]
#endif
	public class CompositeBrick : BrickBase, IEnumerable {
		PointF offset;
		IList<Brick> innerBricks;
		internal IList<MapItem> BrickMap { get; private set; }
		internal void ForceBrickMap() {
			if(BrickMap == null || BrickMap.Count == 0)
				BrickMap = CreateBrickMap();
		}
		List<MapItem> CreateBrickMap() {
			List<MapItem> result = new List<MapItem>();
			for(int i = 0; i < InnerBricks.Count; i++) {
				RectangleF rect = InnerBricks[i].Rect;
				int index1 = i;
				int index2 = i;
				for(int j = 0; j + i < InnerBricks.Count && j < BrickMapConst.Graduation; j++) {
					RectangleF rect2 = InnerBricks[j + i].Rect;
					rect = RectangleF.Union(rect, rect2);
					index2 = j + i;
				}
				result.Add(new MapItem(rect, index1, index2));
				i = index2;
			}
			return result;
		}
		internal override IList InnerBrickList {
			get { return (IList)innerBricks; }
		}
		internal override PointF InnerBrickListOffset { get { return offset; } }
		[
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 0, XtraSerializationFlags.Cached)]
		public virtual IList<Brick> InnerBricks {
			get { return innerBricks; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("CompositeBrickOffset"),
#endif
		XtraSerializableProperty]
		public PointF Offset { get { return offset; } set { offset = value; } }
		public CompositeBrick() : base() {
			innerBricks = new List<Brick>();
		}
		public CompositeBrick(IList<Brick> innerBricks, PointF offset)
			: base(RectangleF.Empty) {
			this.innerBricks = innerBricks;
			this.offset = offset;
		}
		protected override void StoreValues(System.IO.BinaryWriter writer, IRepositoryProvider provider) {
			base.StoreValues(writer, provider);
			writer.WriteStoredObjects<Brick, BrickBase>(InnerBricks, provider);
			writer.WritePointF(offset);
		}
		protected override void RestoreValues(System.IO.BinaryReader reader, IRepositoryProvider provider) {
			base.RestoreValues(reader, provider);
			reader.ReadStoredObjects<Brick, BrickBase>(InnerBricks, provider);
			offset = reader.ReadPointF();
		}
		#region serialization
		protected override void SetIndexCollectionItemCore(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == PrintingSystemSerializationNames.InnerBricks)
				innerBricks.Add((Brick)e.Item.Value);
			base.SetIndexCollectionItemCore(propertyName, e);
		}
		protected override object CreateCollectionItemCore(string propertyName, XtraItemEventArgs e) {
			if(propertyName == PrintingSystemSerializationNames.InnerBricks)
				return CreateInnerBricksItem(e);
			return base.CreateCollectionItemCore(propertyName, e);
		}
		protected virtual object CreateInnerBricksItem(XtraItemEventArgs e) {
			return BrickFactory.CreateBrick(e);
		}
		#endregion
		IEnumerator IEnumerable.GetEnumerator() {
			return this.InnerBricks.GetEnumerator();
		}
		public BrickEnumerator GetEnumerator() {
			return new BrickEnumerator(this);
		}
		internal protected override bool AfterPrintOnPage(IList<int> indices, int pageIndex, int pageCount, Action<BrickBase> callback) {
			base.AfterPrintOnPage(indices, pageIndex, pageCount, callback);
			return InnerBrickList.Count > 0;
		}
		protected override bool ShouldSerializeCore(string propertyName) {
			if(propertyName == PrintingSystemSerializationNames.Offset)
				return !Offset.IsEmpty;
			return base.ShouldSerializeCore(propertyName);
		}
	}
}
