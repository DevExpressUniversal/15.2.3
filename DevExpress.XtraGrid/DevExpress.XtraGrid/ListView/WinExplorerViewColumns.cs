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
using System.Windows.Forms;
using DevExpress.XtraGrid.Columns;
namespace DevExpress.XtraGrid.Views.WinExplorer {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class WinExplorerViewColumns {
		GridColumn textColumn, checkColumn, groupColumn, descriptionColumn, enabledColumn;
		GridColumn extraLargeImageColumn, largeImageColumn, mediumImageColumn, smallImageColumn;
		GridColumn extraLargeImageIndexColumn, largeImageIndexColumn, mediumImageIndexColumn, smallImageIndexColumn;
		public WinExplorerViewColumns(WinExplorerView view) {
			View = view;
		}
		[DefaultValue(null), TypeConverter("DevExpress.XtraGrid.TypeConverters.WinExplorerViewColumnReferenceConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public GridColumn ExtraLargeImageColumn {
			get { return extraLargeImageColumn; }
			set {
				if(ExtraLargeImageColumn == value)
					return;
				extraLargeImageColumn = value;
				OnColumnsChanged();
			}
		}
		[DefaultValue(null), TypeConverter("DevExpress.XtraGrid.TypeConverters.WinExplorerViewColumnReferenceConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public GridColumn LargeImageColumn {
			get { return largeImageColumn; }
			set {
				if(LargeImageColumn == value)
					return;
				largeImageColumn = value;
				OnColumnsChanged();
			}
		}
		[DefaultValue(null), TypeConverter("DevExpress.XtraGrid.TypeConverters.WinExplorerViewColumnReferenceConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public GridColumn MediumImageColumn {
			get { return mediumImageColumn; }
			set {
				if(MediumImageColumn == value)
					return;
				mediumImageColumn = value;
				OnColumnsChanged();
			}
		}
		[DefaultValue(null), TypeConverter("DevExpress.XtraGrid.TypeConverters.WinExplorerViewColumnReferenceConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public GridColumn SmallImageColumn {
			get { return smallImageColumn; }
			set {
				if(SmallImageColumn == value)
					return;
				smallImageColumn = value;
				OnColumnsChanged();
			}
		}
		[DefaultValue(null), TypeConverter("DevExpress.XtraGrid.TypeConverters.WinExplorerViewColumnReferenceConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public GridColumn ExtraLargeImageIndexColumn {
			get { return extraLargeImageIndexColumn; }
			set {
				if(ExtraLargeImageIndexColumn == value)
					return;
				extraLargeImageIndexColumn = value;
				OnColumnsChanged();
			}
		}
		[DefaultValue(null), TypeConverter("DevExpress.XtraGrid.TypeConverters.WinExplorerViewColumnReferenceConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public GridColumn LargeImageIndexColumn {
			get { return largeImageIndexColumn; }
			set {
				if(LargeImageIndexColumn == value)
					return;
				largeImageIndexColumn = value;
				OnColumnsChanged();
			}
		}
		[DefaultValue(null), TypeConverter("DevExpress.XtraGrid.TypeConverters.WinExplorerViewColumnReferenceConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public GridColumn MediumImageIndexColumn {
			get { return mediumImageIndexColumn; }
			set {
				if(MediumImageIndexColumn == value)
					return;
				mediumImageIndexColumn = value;
				OnColumnsChanged();
			}
		}
		[DefaultValue(null), TypeConverter("DevExpress.XtraGrid.TypeConverters.WinExplorerViewColumnReferenceConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public GridColumn SmallImageIndexColumn {
			get { return smallImageIndexColumn; }
			set {
				if(SmallImageIndexColumn == value)
					return;
				smallImageIndexColumn = value;
				OnColumnsChanged();
			}
		}
		[DefaultValue(null), TypeConverter("DevExpress.XtraGrid.TypeConverters.WinExplorerViewColumnReferenceConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public GridColumn TextColumn {
			get { return textColumn; }
			set {
				if(TextColumn == value)
					return;
				textColumn = value;
				OnColumnsChanged();
			}
		}
		[DefaultValue(null), TypeConverter("DevExpress.XtraGrid.TypeConverters.WinExplorerViewColumnReferenceConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public GridColumn DescriptionColumn {
			get { return descriptionColumn; }
			set {
				if(DescriptionColumn == value)
					return;
				descriptionColumn = value;
				OnColumnsChanged();
			}
		}
		[DefaultValue(null), TypeConverter("DevExpress.XtraGrid.TypeConverters.WinExplorerViewColumnReferenceConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public GridColumn CheckBoxColumn {
			get { return checkColumn; }
			set {
				if(CheckBoxColumn == value)
					return;
				checkColumn = value;
				OnColumnsChanged();
			}
		}
		[DefaultValue(null), TypeConverter("DevExpress.XtraGrid.TypeConverters.WinExplorerViewColumnReferenceConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public GridColumn EnabledColumn {
			get { return enabledColumn; }
			set {
				if(EnabledColumn == value)
					return;
				enabledColumn = value;
				OnColumnsChanged();
			}
		}
		[DefaultValue(null), TypeConverter("DevExpress.XtraGrid.TypeConverters.WinExplorerViewColumnReferenceConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public GridColumn GroupColumn {
			get { return groupColumn; }
			set {
				if(GroupColumn == value)
					return;
				GridColumn prev = groupColumn;
				groupColumn = value;
				OnGroupColumnChanged(prev);
			}
		}
		public virtual void Assign(WinExplorerViewColumns columnSet) {
			ExtraLargeImageColumn = AssignColumn(columnSet.ExtraLargeImageColumn);
			LargeImageColumn = AssignColumn(columnSet.LargeImageColumn);
			MediumImageColumn = AssignColumn(columnSet.MediumImageColumn);
			SmallImageColumn = AssignColumn(columnSet.SmallImageColumn);
			ExtraLargeImageIndexColumn = AssignColumn(columnSet.ExtraLargeImageIndexColumn);
			LargeImageIndexColumn = AssignColumn(columnSet.LargeImageIndexColumn);
			MediumImageIndexColumn = AssignColumn(columnSet.MediumImageIndexColumn);
			SmallImageIndexColumn = AssignColumn(columnSet.SmallImageIndexColumn);
			TextColumn = AssignColumn(columnSet.TextColumn);
			DescriptionColumn = AssignColumn(columnSet.DescriptionColumn);
			CheckBoxColumn = AssignColumn(columnSet.CheckBoxColumn);
			GroupColumn = AssignColumn(columnSet.GroupColumn);
			EnabledColumn = AssignColumn(columnSet.EnabledColumn);
		}
		protected GridColumn AssignColumn(GridColumn sourceColumn) {
			if(sourceColumn == null)
				return null;
			return View.Columns[sourceColumn.FieldName];
		}
		protected virtual void OnGroupColumnChanged(GridColumn prev) {
			if(View != null && lockUpdate == 0) View.OnGroupColumnChanged(prev);
		}
		internal void SetGroupColumn(GridColumn column) {
			this.groupColumn = column;
		}
		int lockUpdate = 0;
		[Browsable(false)]
		public WinExplorerView View { get; private set; }
		protected virtual void OnColumnsChanged() {
			if(View != null && lockUpdate == 0)
				View.OnColumnSetChanged();
		}
		GridColumn prevGroupColumn = null;
		internal void BeginUpdate() {
			if(lockUpdate++ == 0) prevGroupColumn = GroupColumn;
		}
		internal void EndUpdate() {
			if(--lockUpdate == 0) {
				OnColumnsChanged();
				if(prevGroupColumn != GroupColumn) OnGroupColumnChanged(prevGroupColumn);
				prevGroupColumn = null;
			}
		}
		internal void Reset() {
			this.CheckBoxColumn =
				this.DescriptionColumn =
				this.ExtraLargeImageColumn =
				this.ExtraLargeImageIndexColumn =
				this.GroupColumn =
				this.LargeImageColumn =
				this.LargeImageIndexColumn =
				this.MediumImageColumn =
				this.MediumImageIndexColumn =
				this.SmallImageColumn =
				this.SmallImageIndexColumn =
				this.TextColumn = null;
		}
		internal bool ShouldSerialize() {
			return this.CheckBoxColumn != null ||
				this.DescriptionColumn != null ||
				this.ExtraLargeImageColumn != null ||
				this.ExtraLargeImageIndexColumn != null ||
				this.GroupColumn != null ||
				this.LargeImageColumn != null ||
				this.LargeImageIndexColumn != null ||
				this.MediumImageColumn != null ||
				this.MediumImageIndexColumn != null ||
				this.SmallImageColumn != null ||
				this.SmallImageIndexColumn != null ||
				this.TextColumn != null;
		}
		internal bool HasTextField { get { return TextColumn != null && !string.IsNullOrEmpty(TextColumn.FieldName); } }
		internal bool HasDescriptionField { get { return DescriptionColumn != null && !string.IsNullOrEmpty(DescriptionColumn.FieldName); } }
		internal bool HasExtraLargeImageField { get { return ExtraLargeImageColumn != null && !string.IsNullOrEmpty(ExtraLargeImageColumn.FieldName); } }
		internal bool HasLargeImageField { get { return LargeImageColumn != null && !string.IsNullOrEmpty(LargeImageColumn.FieldName); } }
		internal bool HasMediumImageField { get { return MediumImageColumn != null && !string.IsNullOrEmpty(MediumImageColumn.FieldName); } }
		internal bool HasSmallImageField { get { return SmallImageColumn != null && !string.IsNullOrEmpty(SmallImageColumn.FieldName); } }
		internal bool HasExtraLargeImageIndexField { get { return ExtraLargeImageIndexColumn != null && !string.IsNullOrEmpty(ExtraLargeImageIndexColumn.FieldName); } }
		internal bool HasLargeImageIndexField { get { return LargeImageIndexColumn != null && !string.IsNullOrEmpty(LargeImageIndexColumn.FieldName); } }
		internal bool HasMediumImageIndexField { get { return MediumImageIndexColumn != null && !string.IsNullOrEmpty(MediumImageIndexColumn.FieldName); } }
		internal bool HasSmallImageIndexField { get { return SmallImageIndexColumn != null && !string.IsNullOrEmpty(SmallImageIndexColumn.FieldName); } }
		internal bool HasCheckField { get { return CheckBoxColumn != null && !string.IsNullOrEmpty(CheckBoxColumn.FieldName); } }
		internal bool HasEnabledField { get { return EnabledColumn != null && !string.IsNullOrEmpty(EnabledColumn.FieldName); } }
		internal bool HasGroupField { get { return GroupColumn != null && !string.IsNullOrEmpty(GroupColumn.FieldName); } }
	}
}
