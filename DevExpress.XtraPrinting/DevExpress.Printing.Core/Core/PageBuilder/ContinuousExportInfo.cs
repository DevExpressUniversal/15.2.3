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
using System.Drawing;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Export;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Serializing;
using System.Collections;
#if SL
using DevExpress.Xpf.Drawing.Printing;
using DevExpress.Xpf.Collections;
#else
using System.Drawing.Printing;
#endif
namespace DevExpress.XtraPrinting.Native {
	public class BrickLayoutInfoEnumerator : IEnumerator {
		IEnumerator<KeyValuePair<Brick, RectangleDF>> en;
		public BrickLayoutInfoEnumerator(IEnumerator<KeyValuePair<Brick, RectangleDF>> en) {
			this.en = en;
			en.Reset();
		}
		object IEnumerator.Current { 
			get {
				return new BrickLayoutInfo(en.Current.Key, en.Current.Value); 
			} 
		}
		bool IEnumerator.MoveNext() { 
			return en.MoveNext(); 
		}
		void IEnumerator.Reset() { 
			en.Reset(); 
		}
	}
	public class BrickLayoutInfoCollection : ICollection {
		Dictionary<Brick, RectangleDF> dictionary;
		public BrickLayoutInfoCollection(Dictionary<Brick, RectangleDF> dictionary) {
			this.dictionary = dictionary;
		}
		void ICollection.CopyTo(Array array, int index) {
			ExceptionHelper.ThrowInvalidOperationException();
		}
		int ICollection.Count { get { return dictionary.Count; } }
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot { get { return null; } }
		IEnumerator IEnumerable.GetEnumerator() {
			return new BrickLayoutInfoEnumerator(dictionary.GetEnumerator());
		}
	}
	public class BrickLayoutInfo : IXtraSupportCreateContentPropertyValue {
		Brick brick;
		RectangleDF rect;
		[XtraSerializableProperty(XtraSerializationVisibility.Content, true, false, false, 3, XtraSerializationFlags.Cached)]
		public Brick Brick { get { return brick; } set { brick = value; } }
		[XtraSerializableProperty]
		public RectangleDF Rect { get { return rect; } set { rect = value; } }
		public BrickLayoutInfo() { 
		}
		public BrickLayoutInfo(Brick brick, RectangleDF rect) {
			this.brick = brick;
			this.rect = rect;
		}
		object IXtraSupportCreateContentPropertyValue.Create(XtraItemEventArgs e) {
			if(e.Item.Name == PrintingSystemSerializationNames.Brick)
				return BrickFactory.CreateBrick(e);
			return null;
		}
	}
	public class MultiColumnInfo {
		int columnCount;
		float start, end;
		[XtraSerializableProperty(0)]
		public int ColumnCount { get {return columnCount;} set {columnCount = value;} }
		[XtraSerializableProperty(1)]
		public float Start { get { return start; } set { start = value; } }
		[XtraSerializableProperty(2)]
		public float End { get { return end; } set { end = value; } }
	}
	public class ContinuousExportInfo : IXtraSortableProperties {
		#region static
		public static readonly ContinuousExportInfo Empty = new ContinuousExportInfo();
		#endregion
		ICollection bricks;
		Margins margins;
		Rectangle pageBounds;
		float bottomMarginOffset;
		ICollection pageBreakPositions;
		ICollection multiColumnInfo;
		[XtraSerializableProperty(0)]
		public Margins PageMargins { get { return margins; } set { margins = value; } }
		[XtraSerializableProperty(1)]
		public Rectangle PageBounds { get { return pageBounds; } set { pageBounds = value; } }
		[XtraSerializableProperty(2)]
		public float BottomMarginOffset { get { return bottomMarginOffset; } set { bottomMarginOffset = value; } }
		[XtraSerializableProperty(XtraSerializationVisibility.SimpleCollection, true, false, false, 3)]
		public ICollection PageBreakPositions { get { return pageBreakPositions; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 4)]
		public ICollection MultiColumnInfo { get { return multiColumnInfo; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 5)]
		public ICollection Bricks { get { return bricks; } }
		internal bool IsEmpty { get { return Bricks.Count == 0; } }
		protected ContinuousExportInfo(ICollection bricks, Margins margins, Rectangle pageBounds, float bottomMarginOffset,
			ICollection pageBreakPositions, ICollection multiColumnInfo) {
			this.bricks = bricks;
			this.margins = margins;
			this.pageBounds = pageBounds;
			this.bottomMarginOffset = bottomMarginOffset;
			this.pageBreakPositions = pageBreakPositions;
			this.multiColumnInfo = multiColumnInfo;
		}
		public ContinuousExportInfo()
			: this(new object[0], new Margins(0, 0, 0, 0), Rectangle.Empty, 0, new object[0], new object[0]) {
		}
		public ContinuousExportInfo(Dictionary<Brick, RectangleDF> bricks, Margins margins, Rectangle pageBounds, float bottomMarginOffset,
			ICollection pageBreakPositions, ICollection multiColumnInfo)
			: this(new BrickLayoutInfoCollection(bricks), margins, pageBounds, bottomMarginOffset, pageBreakPositions, multiColumnInfo) {
		}
		public void ExecuteExport(IBrickExportVisitor brickVisitor, IPrintingSystemContext context) {
			GetLayoutControls(bricks, brickVisitor, context);
		}
		protected virtual void PreprocessBrick(Brick brick, IPrintingSystemContext context) { 
		}
		void GetLayoutControls(ICollection bricks, IBrickExportVisitor brickVisitor, IPrintingSystemContext context) {
			if(bricks == null)
				return;
			foreach(BrickLayoutInfo item in bricks) {
				PreprocessBrick(item.Brick, context);
				RectangleDF rect = item.Rect;
				brickVisitor.ExportBrick(rect.Left, rect.Top, item.Brick);
			}
		}
		bool IXtraSortableProperties.ShouldSortProperties() {
			return true;
		}
	}
	public class DeserializedContinuousExportInfo : ContinuousExportInfo, IXtraSupportDeserializeCollectionItem {
		public DeserializedContinuousExportInfo()
			: base(new ArrayList(), new Margins(0, 0, 0, 0), Rectangle.Empty, 0, new ArrayList(), new ArrayList()) {
		}
		protected override void PreprocessBrick(Brick brick, IPrintingSystemContext context) {
			brick.PerformLayout(context);
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == PrintingSystemSerializationNames.Bricks)
				((ArrayList)Bricks).Add(e.Item.Value);
			else if(propertyName == PrintingSystemSerializationNames.MultiColumnInfo)
				((ArrayList)MultiColumnInfo).Add(e.Item.Value);
		}
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == PrintingSystemSerializationNames.Bricks)
				return new BrickLayoutInfo();
			else if(propertyName == PrintingSystemSerializationNames.MultiColumnInfo)
				return new MultiColumnInfo();
			return null;
		}
	}
}
