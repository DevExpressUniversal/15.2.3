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
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region LegendPosition
	public enum LegendPosition {
		Bottom,
		Left,
		Right,
		Top,
		TopRight
	}
	#endregion
	#region Legend
	public class Legend : ISupportsCopyFrom<Legend> {
		#region Fields
		readonly IChart parent;
		readonly LegendEntryCollection entries;
		readonly LayoutOptions layout;
		readonly ShapeProperties shapeProperties;
		readonly TextProperties textProperties;
		bool visible;
		bool overlay;
		LegendPosition position;
		#endregion
		public Legend(IChart parent) {
			this.parent = parent;
			this.entries = new LegendEntryCollection(parent);
			this.layout = new LayoutOptions(parent);
			this.shapeProperties = new ShapeProperties(DocumentModel) { Parent = parent };
			this.textProperties = new TextProperties(DocumentModel) { Parent = parent };
			this.visible = false;
			this.overlay = true;
			this.position = LegendPosition.Right;
		}
		#region Properties
		protected internal IChart Parent { get { return parent; } }
		protected internal DocumentModel DocumentModel { get { return parent.DocumentModel; } }
		public LegendEntryCollection Entries { get { return entries; } }
		public LayoutOptions Layout { get { return layout; } }
		public bool Visible {
			get { return visible; }
			set {
				if(visible == value)
					return;
				SetVisible(value);
			}
		}
		public bool Overlay {
			get { return overlay; }
			set {
				if (overlay == value)
					return;
				SetOverlay(value);
			}
		}
		public LegendPosition Position { 
			get { return position; } 
			set {
				if(position == value)
					return;
				SetPosition(value); 
			} 
		}
		public ShapeProperties ShapeProperties { get { return shapeProperties; } }
		public TextProperties TextProperties { get { return textProperties; } }
		#endregion
		#region Visible
		public void SetVisible(bool value) {
			LegendVisiblePropertyChangedHistoryItem historyItem = new LegendVisiblePropertyChangedHistoryItem(DocumentModel, this, visible, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetVisibleCore(bool value) {
			this.visible = value;
			Parent.Invalidate();
		}
		#endregion
		#region Overlay
		public void SetOverlay(bool value) {
			LegendOvelayPropertyChangedHistoryItem historyItem = new LegendOvelayPropertyChangedHistoryItem(DocumentModel, this, overlay, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetOverlayCore(bool value) {
			overlay = value;
			Parent.Invalidate();
		} 
		#endregion
		#region Position
		public void SetPosition(LegendPosition value) {
			LegendPositionPropertyChangedHistoryItem historyItem = new LegendPositionPropertyChangedHistoryItem(DocumentModel, this, position, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetPositionCore(LegendPosition value) {
			position= value;
			Parent.Invalidate();
		}
		#endregion
		#region ISupportsCopyFrom<Legend> Members
		public void CopyFrom(Legend value) {
			Visible = value.Visible;
			Overlay = value.Overlay;
			Position = value.Position;
			this.layout.CopyFrom(value.layout);
			this.shapeProperties.CopyFrom(value.shapeProperties);
			this.textProperties.CopyFrom(value.textProperties);
			this.entries.Clear();
			foreach (LegendEntry source in value.entries) {
				LegendEntry item = new LegendEntry(Parent);
				item.CopyFrom(source);
				this.entries.Add(item);
			}
		}
		#endregion
		public Legend CloneTo(IChart parent) {
			Legend legend = new Legend(parent);
			legend.CopyFrom(this);
			return legend;
		}
		public void ResetToStyle() {
			ShapeProperties.ResetToStyle();
			TextProperties.ResetToStyle();
			int count = Entries.Count;
			for (int i = count - 1; i >= 0; i--) {
				LegendEntry entry = Entries[i];
				if (!entry.Delete)
					Entries.RemoveAt(i);
			}
		}
	}
	#endregion
	#region LegendEntry
	public class LegendEntry : ISupportsCopyFrom<LegendEntry> {
		#region Fields
		readonly IChart parent;
		readonly TextProperties textProperties;
		int index;
		bool delete;
		#endregion
		public LegendEntry(IChart parent) {
			this.parent = parent;
			this.textProperties = new TextProperties(DocumentModel) { Parent = parent };
		}
		#region Properties
		protected internal IChart Parent { get { return parent; } }
		protected internal DocumentModel DocumentModel { get { return parent.DocumentModel; } }
		public int Index {
			get { return index; }
			set {
				if (value < 0)
					throw new ArgumentOutOfRangeException("Index value must be equal or greater than zero");
				index = value;
			}
		}
		#region Delete
		public bool Delete { 
			get { return delete; } 
			set {
				if (delete == value)
					return;
				SetDelete(value); 
			} 
		}
		void SetDelete(bool value) {
			LegendEntryDeletePropertyChangedHistoryItem historyItem = new LegendEntryDeletePropertyChangedHistoryItem(DocumentModel, this, delete, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetDeleteCore(bool value) {
			this.delete = value;
			Parent.Invalidate();
		}
		#endregion
		public TextProperties TextProperties { get { return textProperties; } }
		#endregion
		#region ISupportsCopyFrom<LegendEntry> Members
		public void CopyFrom(LegendEntry value) {
			Guard.ArgumentNotNull(value, "value");
			Index = value.Index;
			Delete = value.Delete;
			this.textProperties.CopyFrom(value.textProperties);
		}
		#endregion
	}
	#endregion
	#region LegendEntryCollection
	public class LegendEntryCollection : ChartUndoableCollection<LegendEntry> {
		public LegendEntryCollection(IChart parent)
			: base(parent) {
		}
		public LegendEntry FindByIndex(int index) {
			foreach (LegendEntry item in this) {
				if (item.Index == index)
					return item;
			}
			return null;
		}
	}
	#endregion
}
