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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraLayout;
using DevExpress.XtraEditors;
using System.Collections.Generic;
namespace DevExpress.XtraGrid.Columns {
	public class LayoutViewColumnCollection : GridColumnCollection, IEnumerable<LayoutViewColumn>{
		public LayoutViewColumnCollection(LayoutView view)
			: base(view) {
		}
		protected internal override GridColumn CreateColumn() { 
			return new LayoutViewColumn(); 
		}
		public override void AddRange(GridColumn[] columns) {
			if(View != null && View.IsInitialized)
				View.BeginAddColumnsRange();
			base.AddRange(columns);
			if(View != null && View.IsInitialized)
				View.EndAddColumnsRange();
		}
		public new LayoutViewColumn Add() {
			return base.Add() as LayoutViewColumn;
		}
		public new LayoutViewColumn AddField(string fieldName) {
			return base.AddField(fieldName) as LayoutViewColumn;
		}
		[Browsable(false)]
		public new LayoutView View {
			get { return base.View as LayoutView; }
		}
		public new LayoutViewColumn this[string fieldName] {
			get { return ColumnByFieldName(fieldName); }
		}
		public new LayoutViewColumn this[int index] {
			get { return base[index] as LayoutViewColumn; }
		}
		public new LayoutViewColumn ColumnByName(string columnName) {
			return base.ColumnByName(columnName) as LayoutViewColumn;
		}
		public new LayoutViewColumn ColumnByFieldName(string fieldName) {
			return base.ColumnByFieldName(fieldName) as LayoutViewColumn;
		}
		public virtual void AddRange(LayoutViewColumn[] columns) {
			if(View != null && View.IsInitialized)
				View.BeginAddColumnsRange();
			foreach(LayoutViewColumn col in columns) List.Add(col);
			if(View != null && View.IsInitialized)
				View.EndAddColumnsRange();
		}
		IEnumerator<LayoutViewColumn> IEnumerable<LayoutViewColumn>.GetEnumerator() {
			foreach(LayoutViewColumn column in InnerList)
				yield return column;
		}
	}
	public class LayoutViewColumn : GridColumn {
		LayoutViewField layoutViewFieldCore;
		OptionsField optionsFieldCore;
		public LayoutViewColumn() {
			this.optionsFieldCore = CreateOptionsField();
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(View != null) {
						View.RemoveColumnFromLayout(this);
						View.Refresh();
					}
					if(layoutViewFieldCore != null) {
						using(new DevExpress.Utils.Design.UndoEngineHelper(View)) {
							layoutViewFieldCore.Dispose();
						}
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected virtual OptionsField CreateOptionsField() {
			return new OptionsField();
		}
		internal bool ShouldSerializeOptionsField() { 
			return OptionsField.ShouldSerializeCore(this); 
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewColumnOptionsField"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public OptionsField OptionsField { get { return optionsFieldCore; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewColumnLayoutViewField"),
#endif
TypeConverter(typeof(ExpandableObjectConverter))]
		public LayoutViewField LayoutViewField {
			get { return layoutViewFieldCore; }
			set { AssignLayoutViewField(value); }
		}
		internal bool IsFieldCreated {
			get { return layoutViewFieldCore != null; }
		}
		internal void AssignLayoutViewField(LayoutViewField value) {
			this.layoutViewFieldCore = value;
			if(IsFieldCreated) layoutViewFieldCore.SetLayoutViewColumn(this);
		}
		[Browsable(false)]
		public bool AllowHotTrack {
			get { return IsFieldCreated && layoutViewFieldCore.AllowHotTrack; }
		}
		[Browsable(false)]
		public new LayoutView View {
			get { return base.View as LayoutView; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewColumnImageIndex"),
#endif
XtraSerializableProperty(), DefaultValue(-1)]
		public override int ImageIndex {
			get { return IsFieldCreated ? layoutViewFieldCore.ImageIndex : -1; }
			set { if(IsFieldCreated) layoutViewFieldCore.ImageIndex = value; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewColumnImage"),
#endif
 DefaultValue(null)]
		public override Image Image {
			get { return IsFieldCreated ? layoutViewFieldCore.Image : null; }
			set { if(IsFieldCreated) layoutViewFieldCore.Image = value; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewColumnVisible"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Visible {
			get { return IsFieldCreated ? !layoutViewFieldCore.IsHidden : base.Visible; }
			set {
				if(!(View != null && View.IsInitialized) || !IsFieldCreated || Visible == value) return;
				if(View != null) {
					using(new SafeBaseLayoutItemChanger(layoutViewFieldCore)) {
						if(Visible) layoutViewFieldCore.HideToCustomization();
						else {
							layoutViewFieldCore.RestoreFromCustomization();
						}
					}
				}
				base.Visible = value;
			}
		}
		protected internal override void Assign(GridColumn column) {
			base.Assign(column);
			LayoutViewColumn lvColumn = column as LayoutViewColumn;
			if(lvColumn != null) {
				this.OptionsField.Assign(lvColumn.OptionsField);
			}
		}
		#region Hidden Properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override StringAlignment ImageAlignment {
			get { return base.ImageAlignment; }
			set { base.ImageAlignment = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(defaultColumnWidth), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdLayout), Localizable(true)]
		public override int Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(20), XtraSerializableProperty()]
		public override int MinWidth {
			get { return base.MinWidth; }
			set { base.MinWidth = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(0), XtraSerializableProperty()]
		public override int MaxWidth {
			get { return base.MaxWidth; }
			set { base.MaxWidth = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int VisibleIndex { get { return -1; } set { } }
		#endregion Hidden Properties
	}
}
