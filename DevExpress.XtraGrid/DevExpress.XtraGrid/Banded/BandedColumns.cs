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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
namespace DevExpress.XtraGrid.Views.BandedGrid {
	public class BandedGridColumnLookUp : BandedGridColumn {
		public override ISite Site { get { return null; } set { } }
	}
	[DesignTimeVisible(false), ToolboxItem(false)]
	public class BandedGridColumn : GridColumn {
		GridBand ownerBand;
		bool autoFillDown;
		int rowCount, rowIndex;
		int colIndex;
		public BandedGridColumn() {
			this.colIndex = -1;
			this.rowIndex = 0;
			this.ownerBand = null;
			this.autoFillDown = false;
			this.rowCount = 1;
		}
		protected override void OnResetSerializationProperties(OptionsLayoutBase options) {
			base.OnResetSerializationProperties(options);
			OptionsLayoutGrid optGrid = options as OptionsLayoutGrid;
			if(optGrid == null || optGrid.Columns.StoreLayout) {
				this.colIndex = -1;
				this.autoFillDown = false;
				this.rowCount = 1;
				this.rowIndex = 0;
			}
		}
		protected internal override void Assign(GridColumn column) {
			base.Assign(column);
			BandedGridColumn bc = column as BandedGridColumn;
			if(bc == null) return;
			this.ownerBand = null;
			this.colIndex = bc.ColIndex;
			this.autoFillDown = bc.AutoFillDown;
			this.rowCount = bc.RowCount;
			this.rowIndex = bc.RowIndex;
		}
		[Browsable(false)]
		public new BandedGridView View { get { return base.View as BandedGridView; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int VisibleIndex { 
			get { return base.VisibleIndex; }
			set { 
				if(IsLoading) {
					base.VisibleIndex = value;
					return;
				}
				if(value < 0) value = -1;
				SetVisibleCore(value > -1, value);
				View.OnColumnVisibleIndexChanged(this);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public override int MaxWidth {
			get { return 0; }
			set { }
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridColumnRowCount"),
#endif
 DefaultValue(1), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdLayout)]
		public virtual int RowCount {
			get { return rowCount; }
			set {
				if(value < 1) value = 1;
				if(RowCount == value) return;
				rowCount = value;
				OnChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridColumnAutoFillDown"),
#endif
 DefaultValue(false), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdLayout)]
		public virtual bool AutoFillDown {
			get { return autoFillDown; }
			set {
				if(AutoFillDown == value) return;
				autoFillDown = value;
				OnChanged();
			}
		}
		internal void SeColIndexCore(int value) { this.colIndex = value; } 
		internal int InternalColIndexCore { get { return InternalColIndex; } }
		[Browsable(false)]
		protected virtual int InternalColIndex {
			get {
				if(OwnerBand == null || View == null || View.IsLoading) return -1;
				if(colIndex == -1) {
					colIndex = OwnerBand.Columns.IndexOf(this);
				}
				return colIndex;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ColVIndex {
			get { return View == null ? -1 : View.GetColumnVisibleIndex(this); }
			set { 
				if(View == null) return;
				View.SetColumnVisibleIndex(this, value);
			}
		}
		[Browsable(false)]
		public virtual int ColIndex {
			get {
				if(View == null) return -1;
				return View.CalcBandColIndex(this);
			}
		}
		internal void SetRowIndexCore(int value) { this.rowIndex = value; } 
		[Browsable(false), DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridColumnRowIndex"),
#endif
 DefaultValue(0), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdLayout)]
		public virtual int RowIndex {
			get { return rowIndex; }
			set {
				if(value < 0) value = 0;
				rowIndex = value;
				if(View == null) return;
				View.OnColumnRowIndexChanged(this);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual GridBand OwnerBand { 
			get { return ownerBand; }
			set {
				if(OwnerBand == value) return;
				GridBand oldBand = OwnerBand;
				this.ownerBand = null;
				if(oldBand != null) oldBand.Columns.Remove(this);
				if(value != null) value.Columns.Add(this);
				this.ownerBand = value;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override FixedStyle Fixed {
			get { return FixedStyle.None; }
			set {
			}
		}
		internal void SetOwnerBandCore(GridBand band) { this.ownerBand = band; this.colIndex = -1;}
		protected override void UnGroupCore(int proposedVisibleIndex) {
			GroupIndex = -1;
			Visible = true;
		}
	}
	public class BandedGridColumnReadOnlyCollection : GridColumnReadOnlyCollection {
#if !SL
	[DevExpressXtraGridLocalizedDescription("BandedGridColumnReadOnlyCollectionItem")]
#endif
		public new BandedGridColumn this[int index] { get { return base[index] as BandedGridColumn; } }
		public BandedGridColumnReadOnlyCollection(ColumnView view) : base(view) {
		}
	}
	[ToolboxItem(false)]
	public class BandedGridColumnCollection : GridColumnCollection {
		public BandedGridColumnCollection(ColumnView view) : base(view) {
		}
		public new BandedGridColumn Add() { return base.AddField("") as BandedGridColumn; }
		public new BandedGridColumn AddField(string fieldName) { return base.AddField(fieldName) as BandedGridColumn; }
		[Browsable(false)]
		public new BandedGridView View { get { return base.View as BandedGridView; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("BandedGridColumnCollectionItem")]
#endif
		public new BandedGridColumn this[string fieldName] { get { return ColumnByFieldName(fieldName); } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("BandedGridColumnCollectionItem")]
#endif
		public new BandedGridColumn this[int index] { get { return base[index] as BandedGridColumn; }  }
		public new BandedGridColumn ColumnByName(string columnName) {
			return base.ColumnByName(columnName) as BandedGridColumn;
		}
		public new BandedGridColumn ColumnByFieldName(string fieldName) {
			return base.ColumnByFieldName(fieldName) as BandedGridColumn;
		}
		public virtual void AddRange(BandedGridColumn[] columns) {
			foreach(BandedGridColumn col in columns) {
				List.Add(col);
			}
		}
		protected internal override GridColumn CreateColumn() { return new BandedGridColumn(); }
	}
}
