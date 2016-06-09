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
using DevExpress.Spreadsheet.Drawings;
namespace DevExpress.Spreadsheet.Charts {
	public enum LegendPosition {
		Bottom,
		Left,
		Right,
		Top,
		TopRight
	}
	public interface LegendEntry {
		int Index { get; }
		bool Hidden { get; set; }
		ShapeTextFont Font { get; }
	}
	public interface LegendEntryCollection : ISimpleCollection<LegendEntry> {
		LegendEntry Add(int index);
		bool Remove(LegendEntry entry);
		void RemoveAt(int index);
		void Clear();
		bool Contains(LegendEntry entry);
		int IndexOf(LegendEntry entry);
		LegendEntry GetEntryByIndex(int index);
	}
	public interface Legend : ShapeFormat {
		bool Visible { get; set; }
		bool Overlay { get; set; }
		LegendPosition Position { get; set; }
		LayoutOptions Layout { get; }
		LegendEntryCollection CustomEntries { get; }
		ShapeTextFont Font { get; }
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.Office.API.Internal;
	#region NativeLegendEntry
	partial class NativeLegendEntry : NativeObjectBase, LegendEntry {
		readonly Model.LegendEntry modelLegendEntry;
		NativeShapeTextFont font;
		public NativeLegendEntry(Model.LegendEntry modelLegendEntry)
			: base() {
			this.modelLegendEntry = modelLegendEntry;
		}
		#region LegendEntry Members
		public int Index {
			get {
				CheckValid();
				return modelLegendEntry.Index;
			}
		}
		public bool Hidden {
			get {
				CheckValid();
				return modelLegendEntry.Delete;
			}
			set {
				CheckValid();
				modelLegendEntry.Delete = value;
			}
		}
		public ShapeTextFont Font {
			get {
				CheckValid();
				if (font == null)
					font = new NativeShapeTextFont(modelLegendEntry.TextProperties);
				return font;
			}
		}
		#endregion
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (font != null)
				font.IsValid = value;
		}
		public override bool Equals(object obj) {
			if (!IsValid)
				return false;
			NativeLegendEntry other = obj as NativeLegendEntry;
			if (other == null || !other.IsValid)
				return false;
			return object.ReferenceEquals(modelLegendEntry, other.modelLegendEntry);
		}
		public override int GetHashCode() {
			if (!IsValid)
				return -1;
			return modelLegendEntry.Index;
		}
	}
	#endregion
	#region NativeLegendEntryCollection
	partial class NativeLegendEntryCollection : NativeChartCollectionBase<LegendEntry, NativeLegendEntry, Model.LegendEntry>, LegendEntryCollection {
		public NativeLegendEntryCollection(Model.LegendEntryCollection modelCollection)
			: base(modelCollection) {
		}
		Model.LegendEntryCollection ModelLegendEntries { get { return ModelCollection as Model.LegendEntryCollection; } }
		protected override NativeLegendEntry CreateNativeObject(Model.LegendEntry modelItem) {
			return new NativeLegendEntry(modelItem);
		}
		#region LegendEntryCollection Members
		public LegendEntry Add(int index) {
			LegendEntry entry = GetEntryByIndex(index);
			if (entry != null)
				return entry;
			Model.LegendEntry modelEntry = new Model.LegendEntry(ModelLegendEntries.Parent);
			modelEntry.Index = index;
			ModelLegendEntries.Add(modelEntry);
			return InnerList[Count - 1];
		}
		public bool Remove(LegendEntry entry) {
			int index = IndexOf(entry);
			if (index != -1)
				RemoveAt(index);
			return index != -1;
		}
		public bool Contains(LegendEntry entry) {
			return IndexOf(entry) != -1;
		}
		public int IndexOf(LegendEntry entry) {
			CheckValid();
			NativeLegendEntry nativeEntry = entry as NativeLegendEntry;
			if (nativeEntry != null)
				return InnerList.IndexOf(nativeEntry);
			return -1;
		}
		public LegendEntry GetEntryByIndex(int index) {
			CheckValid();
			int count = Count;
			for (int i = 0; i < count; i++) {
				LegendEntry item = InnerList[i];
				if (item.Index == index)
					return item;
			}
			return null;
		}
		#endregion
	}
	#endregion
	#region NativeLegend
	partial class NativeLegend : NativeShapeFormat, Legend {
		#region Fields
		readonly Model.Legend modelLegend;
		NativeLayoutOptions layout;
		NativeLegendEntryCollection entries;
		NativeShapeTextFont font;
		#endregion
		public NativeLegend(Model.Legend modelLegend, NativeWorkbook nativeWorkbook)
			: base(modelLegend.ShapeProperties, nativeWorkbook) {
			this.modelLegend = modelLegend;
		}
		#region Legend Members
		public bool Visible {
			get {
				CheckValid();
				return modelLegend.Visible;
			}
			set {
				CheckValid();
				modelLegend.Visible = value;
			}
		}
		public bool Overlay {
			get {
				CheckValid();
				return modelLegend.Overlay;
			}
			set {
				CheckValid();
				modelLegend.Overlay = value;
			}
		}
		public LegendPosition Position {
			get {
				CheckValid();
				return (LegendPosition)modelLegend.Position;
			}
			set {
				CheckValid();
				modelLegend.Position = (Model.LegendPosition)value;
			}
		}
		public LayoutOptions Layout {
			get {
				CheckValid();
				if (layout == null)
					layout = new NativeLayoutOptions(modelLegend.Layout);
				return layout;
			}
		}
		public LegendEntryCollection CustomEntries {
			get {
				CheckValid();
				if (entries == null)
					entries = new NativeLegendEntryCollection(modelLegend.Entries);
				return entries;
			}
		}
		public ShapeTextFont Font {
			get {
				CheckValid();
				if (font == null)
					font = new NativeShapeTextFont(modelLegend.TextProperties);
				return font;
			}
		}
		#endregion
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (layout != null)
				layout.IsValid = value;
			if (entries != null)
				entries.IsValid = value;
			if (font != null)
				font.IsValid = value;
		}
		public override void ResetToMatchStyle() {
			modelLegend.DocumentModel.BeginUpdate();
			try {
				modelLegend.ResetToStyle();
			}
			finally {
				modelLegend.DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
}
