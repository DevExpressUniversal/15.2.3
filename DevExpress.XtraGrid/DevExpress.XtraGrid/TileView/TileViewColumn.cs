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

using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Tile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.XtraGrid.Columns {
	public class TileViewColumnCollection : GridColumnCollection {
		public TileViewColumnCollection(TileView view)
			: base(view) { 
		}
		protected internal override GridColumn CreateColumn() {
			return new TileViewColumn();
		}
	}
	public class TileViewColumn : GridColumn {
		protected override OptionsColumn CreateOptionsColumn() {
			return new TileViewOptionsColumn();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public int GroupIndex {
			get { return base.GroupIndex; }
			set { base.GroupIndex = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public FormatInfo GroupFormat {
			get { return base.GroupFormat; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public ColumnGroupInterval GroupInterval {
			get { return base.GroupInterval; }
			set { base.GroupInterval = value; }
		}
	}
	public class TileViewOptionsColumn : OptionsColumn {
		public TileViewOptionsColumn() {
			this.ShowCaption = false;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnShowCaption"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public new bool ShowCaption {
			get { return base.ShowCaption; }
			set { base.ShowCaption = value; }
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TileViewColumns {
		TileView view;
		[Browsable(false)]
		public TileView View { get { return view; } }
		public TileViewColumns(TileView view) {
			this.view = view;
		}
		protected virtual void OnColumnsChanged() {
			if(View != null)
				View.OnColumnSetChanged();
		}
		int lockUpdate = 0;
		internal void BeginUpdate() {
			lockUpdate++;
		}
		internal void EndUpdate() {
			if(--lockUpdate == 0) {
				OnColumnsChanged();
			}
		}
		GridColumn groupColumn;
		[DefaultValue(null)]
		public GridColumn GroupColumn {
			get { return groupColumn; }
			set {
				if(value == groupColumn) return;
				GridColumn prev = groupColumn;
				groupColumn = value;
				if(lockUpdate == 0)
					View.OnGroupColumnChanged(prev);
			}
		}
		GridColumn checkedColumn;
		[DefaultValue(null)]
		public GridColumn CheckedColumn {
			get { return checkedColumn; }
			set {
				if(value == checkedColumn) return;
				checkedColumn = value;
				OnColumnsChanged();
			}
		}
		GridColumn backGroundImageColumn;
		[DefaultValue(null)]
		public GridColumn BackgroundImageColumn {
			get { return backGroundImageColumn; }
			set {
				if(value == backGroundImageColumn) return;
				backGroundImageColumn = value;
				OnColumnsChanged();
			}
		}
		GridColumn enabledColumn;
		[DefaultValue(null)]
		public GridColumn EnabledColumn {
			get { return enabledColumn; }
			set {
				if(value == enabledColumn) return;
				enabledColumn = value;
				OnColumnsChanged();
			}
		}
		internal void Reset() {
			this.GroupColumn =
				this.BackgroundImageColumn =
				this.EnabledColumn =
				this.CheckedColumn = null;
		}
		internal bool ShouldSerialize() {
			return this.GroupColumn != null ||
				this.BackgroundImageColumn != null ||
				this.EnabledColumn != null ||
				this.CheckedColumn != null;
		}
		public virtual void Assign(TileViewColumns srcSet) {
			this.GroupColumn = srcSet.GroupColumn;
			this.CheckedColumn = srcSet.CheckedColumn;
			this.BackgroundImageColumn = srcSet.BackgroundImageColumn;
			this.EnabledColumn = srcSet.EnabledColumn;
		}
		protected internal void SetGroupColumn(GridColumn column) {
			this.groupColumn = column;
		}
	}
}
