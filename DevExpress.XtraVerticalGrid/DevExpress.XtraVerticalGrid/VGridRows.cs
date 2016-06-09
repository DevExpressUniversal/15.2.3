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
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraVerticalGrid.ViewInfo;
using DevExpress.XtraVerticalGrid.Painters;
using DevExpress.XtraEditors.Repository;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using DevExpress.Data;
using System.Collections.Generic;
using DevExpress.XtraVerticalGrid.Data;
namespace DevExpress.XtraVerticalGrid.Rows {
	public enum RowSortOrder { None, Ascending, Descending }
	public enum RowFilterType { None, Custom, Value}
	public enum SeparatorKind { VertLine, String }
	public enum FixedStyle {
		None,
		Top,
		Bottom
	}
	public class RowProperties : IDisposable, IDataColumnInfo {
		const string DisconnectedName = "Disconnected RowProperties";
		internal const string BaseRowName = "row";
		internal const string BaseCategoryRowName = "category";
		protected FormatInfo fFormat;
		protected string fCaption;
		protected string fCustomizationCaption;
		protected string fToolTip;
		protected string fFieldName;
		protected string unboundFieldName;
		protected object fValue;
		protected bool? fReadOnly;
		internal IRowChangeListener sink;
		int imageIndex;
		UnboundColumnType unboundType;
		string unboundExpression;
		bool showUnboundExpressionMenu;
		internal RepositoryItem rowEdit;
		public RowProperties() : this(string.Empty) {}
		public RowProperties(string fFieldName) {
			this.fFormat = new RowFormatInfo(this);
			this.fFieldName = fFieldName;
			this.fCaption = fFieldName;
			this.rowEdit = null;
			this.fValue = null;
			this.imageIndex = -1;
			this.fCustomizationCaption = string.Empty;
			this.showUnboundExpressionMenu = false;
			this.unboundExpression = string.Empty;
			this.fToolTip = string.Empty;
		}
		public void AssignTo(RowProperties dest) {
			if(dest.Grid != null)
				dest.Grid.BeginUpdate();
			try {
				AssignToCore(dest);
			}
			finally {
				if(dest.Grid != null)
					dest.Grid.CancelUpdate();
			}
			dest.Changed(RowChangeTypeEnum.PropertiesAssigned);
		}
		protected virtual void AssignToCore(RowProperties dest) {
			dest.fFieldName = this.FieldName;
			dest.fFormat.Assign(this.Format);
			dest.fCaption = this.Caption;
			dest.rowEdit = this.RowEdit;
			dest.fValue = this.Value;
			dest.fReadOnly = this.ReadOnly;
			dest.imageIndex = this.ImageIndex;
			dest.unboundExpression = this.UnboundExpression;
			dest.showUnboundExpressionMenu = this.ShowUnboundExpressionMenu;
			dest.fToolTip = this.fToolTip;
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("RowPropertiesFormat"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		Category("Appearance")]
		public FormatInfo Format {
			get { return fFormat; }
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("RowPropertiesCaption"),
#endif
		DefaultValue(""),
		XtraSerializableProperty(),
		Localizable(true),
		Category("Appearance")]
		public string Caption { 
			get { return fCaption; }
			set {
				if(value == null) value = string.Empty;
				RowPropertiesChangingArgs args = new RowPropertiesChangingArgs(RowChangeTypeEnum.Caption, value, Caption);
				if(CanChange(args)) {
					fCaption = (string)args.Value;
					Changed(RowChangeTypeEnum.Caption);
				}
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("RowPropertiesCustomizationCaption"),
#endif
		DefaultValue(""),
		XtraSerializableProperty(),
		Localizable(true),
		Category("Appearance")]
		public string CustomizationCaption {
			get { return fCustomizationCaption; }
			set {
				if(value == null) value = string.Empty;
				RowPropertiesChangingArgs args = new RowPropertiesChangingArgs(RowChangeTypeEnum.Caption, value, CustomizationCaption);
				if(CanChange(args)) {
					fCustomizationCaption = (string)args.Value;
					Changed(RowChangeTypeEnum.Caption);
				}
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("RowPropertiesToolTip"),
#endif
 Category("Appearance"), DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		public virtual string ToolTip {
			get { return fToolTip; }
			set {
				if(value == null)
					value = string.Empty;
				RowPropertiesChangingArgs args = new RowPropertiesChangingArgs(RowChangeTypeEnum.ToolTip, value, ToolTip);
				if(CanChange(args)) {
					fToolTip = (string)args.Value;
					Changed(RowChangeTypeEnum.ToolTip);
				}
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("RowPropertiesFieldName"),
#endif
		DefaultValue(""),
		TypeConverter("DevExpress.XtraVerticalGrid.Design.Converters.FieldNameTypeConverter," + AssemblyInfo.SRAssemblyVertGridDesign),
		XtraSerializableProperty(),
		Category("Data"),
		RefreshProperties(RefreshProperties.All)]
		public virtual string FieldName {
			get { return fFieldName; }
			set {
				if(value == null) value = string.Empty;
				RowPropertiesChangingArgs args = new RowPropertiesChangingArgs(RowChangeTypeEnum.FieldName, value, FieldName);
				if(CanChange(args)) {
					fFieldName = (string)args.Value;
					Changed(RowChangeTypeEnum.FieldName);
				}
			}
		}
		internal int CellIndex {
			get {
				if(Row == null)
					return -1;
				int rowPropertiesCount = Row.RowPropertiesCount;
				for(int i = 0; i < rowPropertiesCount; i++)
					if(Equals(Row.GetRowProperties(i)))
						return i;
				return -1;
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("RowPropertiesShowUnboundExpressionMenu"),
#endif
		Category("Data"),
		DefaultValue(false),
		XtraSerializableProperty()]
		public bool ShowUnboundExpressionMenu {
			get { return showUnboundExpressionMenu; }
			set {
				if(showUnboundExpressionMenu == value)
					return;
				showUnboundExpressionMenu = value;
				Changed(RowChangeTypeEnum.ShowUnboundExpressionMenu);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("RowPropertiesUnboundType"),
#endif
		DefaultValue(UnboundColumnType.Bound),
		XtraSerializableProperty(),
		Category("Data")]
		public UnboundColumnType UnboundType {
			get { return unboundType; }
			set {
				if(unboundType == value)
					return;
				unboundType = value;
				Changed(RowChangeTypeEnum.UnboundType);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("RowPropertiesUnboundExpression"),
#endif
		Category("Data"),
		DefaultValue(""),
		XtraSerializableProperty(),
		Editor(typeof(DevExpress.XtraEditors.Design.ExpressionEditorBase), typeof(System.Drawing.Design.UITypeEditor))]
		public string UnboundExpression {
			get { return unboundExpression; }
			set {
				if(value == null) value = string.Empty;
				if(UnboundExpression == value) return;
				unboundExpression = value;
				Changed(RowChangeTypeEnum.UnboundExpression);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int RowHandle {
			get {
				if(Grid == null) return -1;
				return Grid.DataModeHelper.GetRowHandleByFieldName(this);
			}
		}
		[Browsable(false)]
		public Type RowType {
			get {
				if(Grid == null) return typeof(object);
				return Grid.DataModeHelper.GetDataType(this);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsLoading { get { return Row == null || Row.IsLoadingCore; } }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("RowPropertiesRowEdit"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		DefaultValue(null),
		Localizable(true),
		Editor("DevExpress.XtraVerticalGrid.Design.RowEditEditor, " + AssemblyInfo.SRAssemblyVertGridDesign, typeof(System.Drawing.Design.UITypeEditor)),
		Category("Data")]
		public virtual DevExpress.XtraEditors.Repository.RepositoryItem RowEdit {
			get { return rowEdit; }
			set {
				RowPropertiesChangingArgs args = new RowPropertiesChangingArgs(RowChangeTypeEnum.RowEdit, value, RowEdit);
				if(CanChange(args)) {
					SetRowEdit(args.Value as RepositoryItem);
					Changed(RowChangeTypeEnum.RowEdit);
				}
			}
		}
		void RowEdit_Appearance_Changed(object sender, EventArgs e) {
			Changed(RowChangeTypeEnum.RowEdit);
		}
		protected virtual void SetRowEdit(RepositoryItem newRowEdit) {
			DisconnectEditor();
			this.rowEdit = newRowEdit;
			ConnectEditor();
		}
		protected virtual void DisconnectEditor() {
			if(RowEdit != null) {
				RowEdit.Disconnect(this);
				RowEdit.Appearance.Changed -= new EventHandler(RowEdit_Appearance_Changed);
			}
		}
		protected virtual void ConnectEditor() {
			if(RowEdit != null) {
				RowEdit.Connect(this);
				RowEdit.Appearance.Changed += new EventHandler(RowEdit_Appearance_Changed);
			}
		}
		[Browsable(false), Localizable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty()]
		public virtual string RowEditName {
			get { return (RowEdit == null ? string.Empty : RowEdit.Name); }
			set {
				if(value == null) value = string.Empty;
				if(value == RowEditName || Grid == null) return;
				RowEdit = Grid.ContainerHelper.FindRepositoryItem(value);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("RowPropertiesValue"),
#endif
		DefaultValue(null),
		XtraSerializableProperty(),
		Localizable(true),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Category("Data")]
		public object Value {
			get {
				if(Grid == null) {
					return ValueCore;
				}
				return Grid.DataModeHelper.GetCellValue(this, 0);
			}
			set {
				RowPropertiesChangingArgs args = new RowPropertiesChangingArgs(RowChangeTypeEnum.Value, value, Value);
				if(CanChange(args)) {
					if(Grid == null) {
						ValueCore = args.Value;
					} else {
						sink.IsBlocked = true;
						Grid.DataModeHelper.SetCellValue(this, 0, args.Value, false);
						sink.IsBlocked = false;
					}
					Changed(RowChangeTypeEnum.Value);
				}
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool XtraShouldSerializeValue() {
			if(Grid == null)
				return false;
			return !Grid.DataModeHelper.IsBound;
		}
		internal object ValueCore { get { return fValue; } set { fValue = value; } }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("RowPropertiesImageIndex"),
#endif
		DefaultValue(-1),
		XtraSerializableProperty(),
		Localizable(true),
		Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(UITypeEditor)),
		DevExpress.Utils.ImageList("Images"),
		Category("Appearance")]
		public int ImageIndex {
			get { return imageIndex; }
			set {
				RowPropertiesChangingArgs args = new RowPropertiesChangingArgs(RowChangeTypeEnum.ImageIndex, value, ImageIndex);
				if(CanChange(args)) {
					imageIndex = (int)args.Value;
					Changed(RowChangeTypeEnum.ImageIndex);
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Images {
			get {
				if(Grid == null) return null;
				return Grid.ImageList;
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("RowPropertiesReadOnly"),
#endif
		DefaultValue(null),
		XtraSerializableProperty(),
		Category("Options")]
		public virtual bool? ReadOnly {
			get { return fReadOnly; }
			set {
				RowPropertiesChangingArgs args = new RowPropertiesChangingArgs(RowChangeTypeEnum.ReadOnly, value, ReadOnly);
				if(CanChange(args)) {
					fReadOnly = (bool?)args.Value;
					Changed(RowChangeTypeEnum.ReadOnly);
				}
			}
		}
		protected internal virtual bool RenderReadOnly { get; set; }
		protected internal virtual bool GetReadOnly() { return ReadOnly.HasValue ? ReadOnly.Value : IsSourceReadOnly; }
		[Browsable(false)]
		public virtual bool Bindable { get { return true; } }
		public bool IsSourceReadOnly {
			get {
				if(Grid == null)
					return false;
				return Grid.DataModeHelper.GetPredefinedReadOnlyByFieldName(this);
			}
		}
		protected virtual bool CanChange(RowPropertiesChangingArgs args) {
			if(sink == null) return true;
			args.Prop = this;
			sink.RowPropertiesChanging(this, args);
			return args.CanChange;
		}
		protected virtual void Changed(RowChangeTypeEnum changeType) {
			if(sink != null)
				sink.RowPropertiesChanged(this, changeType);
		}
		[Browsable(false)]
		public BaseRow Row { 
			get { 
				if(sink == null) return null;
				return sink.Row; 
			} 
		}
		VGridControlBase Grid { get { return (Row == null ? null : Row.Grid);} }
		class RowFormatInfo : FormatInfo {
			RowProperties props;
			public RowFormatInfo(RowProperties props) {
				this.props = props;
			}
			public override IFormatProvider Format { 
				get { return base.Format; }
				set {
					RowPropertiesChangingArgs args = new RowPropertiesChangingArgs(RowChangeTypeEnum.Format, value, FormatString);
					if(props.CanChange(args)) {
						base.Format = value;
						props.Changed(RowChangeTypeEnum.Format);
					}
				}
			}
			public override string FormatString { 
				get { return base.FormatString; }
				set {
					if(value == null) value = string.Empty;
					RowPropertiesChangingArgs args = new RowPropertiesChangingArgs(RowChangeTypeEnum.FormatString, value, FormatString);
					if(props.CanChange(args)) {
						base.FormatString = value;
						props.Changed(RowChangeTypeEnum.FormatString);
					}
				}
			}
			public override FormatType FormatType { 
				get { return base.FormatType; }
				set {
					RowPropertiesChangingArgs args = new RowPropertiesChangingArgs(RowChangeTypeEnum.FormatType, value, FormatType);
					if(props.CanChange(args)) {
						base.FormatType = value;
						props.Changed(RowChangeTypeEnum.FormatType);
					}
				}
			}
			protected override bool IsLoading { get { return props.IsLoading || base.IsLoading; } }
		}
		#region IDisposable Members
		void IDisposable.Dispose() {
			Dispose();
			GC.SuppressFinalize(this);
		}
		#endregion
#if DEBUGTEST
		public override string ToString() {
			return GetTextCaption();
		}
#endif
		protected virtual void Dispose() {
			this.SetRowEdit(null);
		}
		protected virtual internal string GetTextCaption() {
			if(!string.IsNullOrEmpty(Caption))
				return Caption;
			if(!string.IsNullOrEmpty(FieldName)) {
				return FieldName;
			}
			return Row == null ? DisconnectedName : Row.Name;
		}
		protected virtual internal string GetName() {
			string uniqueName = string.IsNullOrEmpty(Row.Name) ? Guid.NewGuid().ToString() : Row.Name;
			return uniqueName + CellIndex.ToString();
		}
		protected virtual internal string GetBaseName() {
			return BaseRowName + FieldName;
		}
		#region IDataColumnInfo Members
		string IDataColumnInfo.Caption { get { return GetTextCaption(); } }
		System.Collections.Generic.List<IDataColumnInfo> IDataColumnInfo.Columns {
			get {
				if(Grid == null)
					return null;
				GetRowPropertiesOperation operation = new GetRowPropertiesOperation(this);
				Grid.RowsIterator.DoOperation(operation);
				return new System.Collections.Generic.List<IDataColumnInfo>(operation.Properties.ToArray());
			}
		}
		DataControllerBase IDataColumnInfo.Controller { get { return Grid == null ? null : Grid.DataManager; } }
		string IDataColumnInfo.FieldName { get { return FieldName; } }
		Type IDataColumnInfo.FieldType { get { return RowType; }}
		string IDataColumnInfo.Name { get { return Row.Name; } }
		string IDataColumnInfo.UnboundExpression { get { return UnboundExpression; } }
		#endregion
	}
	public class CategoryRowProperties : RowProperties {
		public CategoryRowProperties() : this(string.Empty) {}
		public CategoryRowProperties(string fCaption) : base(fCaption) { }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool? ReadOnly {
			get { return true; }
			set {}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string FieldName {
			get { return string.Empty; }
			set {}
		}
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("CategoryRowPropertiesRowHandle")]
#endif
		public override int RowHandle { get { return -1; } }
		[Browsable(false)]
		public override DevExpress.XtraEditors.Repository.RepositoryItem RowEdit {
			get { return null; }
			set {}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("CategoryRowPropertiesRowEditName"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string RowEditName {
			get { return string.Empty; }
			set {}
		}
		[Browsable(false)]
		public override bool Bindable { get { return false; } }
		protected internal override string GetBaseName() {
			return BaseCategoryRowName + Caption;
		}
	}
	[DesignTimeVisible(false)]
	[ToolboxItem(false)]
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverterEx))]
	public class MultiEditorRowProperties : RowProperties, IComponent {
		int width, cellWidth;
		public const int MinWidth  = 15;
		const int defautlWidth = 20;
		public MultiEditorRowProperties() : this(string.Empty) {
		}
		public MultiEditorRowProperties(string fFieldName) : base (fFieldName) {
			this.width = defautlWidth;
			this.cellWidth = defautlWidth;
		}
		protected override void AssignToCore(RowProperties dest) {
			base.AssignToCore(dest);
			MultiEditorRowProperties mDest = dest as MultiEditorRowProperties;
			if(mDest != null) {
				mDest.width = this.Width;
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("MultiEditorRowPropertiesWidth"),
#endif
 DefaultValue(defautlWidth), XtraSerializableProperty(), Localizable(true)]
		public int Width {
			get { return width; }
			set {
				if(value < MinWidth)
					value = MinWidth;
				RowPropertiesChangingArgs args = new RowPropertiesChangingArgs(RowChangeTypeEnum.Width, value, Width);
				if(CanChange(args)) {
					SetWidth(Math.Max(MinWidth, (int)args.Value));
					Changed(RowChangeTypeEnum.Width);
				}
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("MultiEditorRowPropertiesCellWidth"),
#endif
 DefaultValue(defautlWidth), XtraSerializableProperty(), Localizable(true)]
		public int CellWidth {
			get { return cellWidth; }
			set {
				if(value < MinWidth)
					value = MinWidth;
				RowPropertiesChangingArgs args = new RowPropertiesChangingArgs(RowChangeTypeEnum.CellWidth, value, CellWidth);
				if(CanChange(args)) {
					SetCellWidth(Math.Max(MinWidth, (int)args.Value));
					Changed(RowChangeTypeEnum.CellWidth);
				}
			}
		}
		internal void SetCellWidth(int value) { this.cellWidth = value; }
		internal void SetWidth(int value) { this.width = value; }
		internal bool IsVisible { get { return !string.IsNullOrEmpty(Caption); } }
		protected override void Dispose() {
			lock(this) {
				if((this.site != null) && (this.site.Container != null)) {
					this.site.Container.Remove(this);
				}
				if(this.disposed != null) {
					this.disposed(this, EventArgs.Empty);
				}
			}
			base.Dispose();
		}
		#region IComponent Members
		ISite site;
		event EventHandler disposed;
		event EventHandler IComponent.Disposed {
			add { disposed += value; }
			remove { disposed -= value; }
		}
		ISite IComponent.Site { get { return site; } set { site = value; } }
		#endregion
		#region IDisposable Members
		void IDisposable.Dispose() {
			Dispose();
			GC.SuppressFinalize(this);
		}
		#endregion
	}
	[ListBindable(false)]
	public class MultiEditorRowPropertiesCollection : IList, IRowViewScaler {
		protected ArrayList List;
		internal MultiEditorRow gridRow;
		internal MultiEditorRowPropertiesCollection(MultiEditorRow gridRow) {
			this.List = new ArrayList();
			this.gridRow = gridRow;
		}
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("MultiEditorRowPropertiesCollectionItem")]
#endif
		public MultiEditorRowProperties this[int index] {
			get {
				if(index < 0 || index > Count - 1) return null;
				return List[index] as MultiEditorRowProperties;
			}
		}
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("MultiEditorRowPropertiesCollectionItem")]
#endif
		public MultiEditorRowProperties this[string fFieldName] {
			get {
				foreach(MultiEditorRowProperties mp in List) {
					if(mp.FieldName == fFieldName)
						return mp;
				}
				return null;
			}
		}
		object IList.this[int index] {
			get { return List[index]; }
			set {
				if(value is MultiEditorRowProperties) {
					if(List[index] != value) {
						MultiEditorRowProperties mp = (MultiEditorRowProperties)value;
						mp.sink = gridRow;
						List[index] = mp;
						Changed(mp, RowChangeTypeEnum.PropertiesReplaced);
					}
				}
			}
		}
		public MultiEditorRowProperties Add() {
			return AddProperties(string.Empty);
		}
		public MultiEditorRowProperties AddProperties(string fFieldName) {
			MultiEditorRowProperties mp = (MultiEditorRowProperties)gridRow.CreateRowProperties();
			mp.FieldName = fFieldName;
			mp.Caption = fFieldName;
			Add(mp);
			return mp;
		}
		public int Add(MultiEditorRowProperties rowProperties) {
			return ((IList)this).Add(rowProperties);
		}
		int IList.Add(object rowProperties) {
			if(rowProperties is MultiEditorRowProperties) {
				MultiEditorRowProperties mp = rowProperties as MultiEditorRowProperties;
				mp.sink = gridRow;
				AddToContainer(mp);
				int index = List.Add(mp);
				Changed(mp, RowChangeTypeEnum.PropertiesAdded);
				return index;
			}
			return -1;
		}
		void AddToContainer(MultiEditorRowProperties properties) {
			if(this.gridRow.IsConnected && this.gridRow.Grid.Container != null) {
				this.gridRow.Grid.Container.Add(properties);
			}
		}
		public void Insert(int index, MultiEditorRowProperties rowProperties) {
			((IList)this).Insert(index, rowProperties);
		}
		void IList.Insert(int index, object rowProperties) {
			if(rowProperties is MultiEditorRowProperties) {
				MultiEditorRowProperties mp = rowProperties as MultiEditorRowProperties;
				mp.sink = gridRow;
				List.Insert(index, rowProperties);
				Changed((MultiEditorRowProperties)rowProperties, RowChangeTypeEnum.PropertiesAdded);
			}
		}
		public void AddRange(MultiEditorRowProperties[] props) {
			if(props == null) return;
			for (int i = 0; i < props.Length; i++) {
				((IList)this).Add(props[i]);
			}
		}
		public void Clear() {
			for (int i = 0; i < Count; i++) {
				this[i].sink = null;
			}
			List.Clear();
			Changed(null, RowChangeTypeEnum.PropertiesCleared);
		}
		public void Remove(object rowProperties) {
			if(IndexOf(rowProperties) != -1) {
				(rowProperties as IDisposable).Dispose();
				List.Remove(rowProperties);
				Changed(null, RowChangeTypeEnum.PropertiesDeleted);
			}
		}
		public void RemoveAt(int index) {
			(List[index] as IDisposable).Dispose();
			List.RemoveAt(index);
			Changed(null, RowChangeTypeEnum.PropertiesDeleted);
		}
		public int IndexOf(object rowProperties) { return List.IndexOf(rowProperties); }
		public bool Contains(object rowProperties) { return List.Contains(rowProperties); }
		bool IList.IsFixedSize { get { return List.IsFixedSize; } }
		bool IList.IsReadOnly { get { return List.IsReadOnly; } }
		IEnumerator IEnumerable.GetEnumerator() { return List.GetEnumerator(); }
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("MultiEditorRowPropertiesCollectionCount")]
#endif
		public int Count { get { return List.Count; } }
		object ICollection.SyncRoot { get { return List.SyncRoot; } }
		bool ICollection.IsSynchronized { get { return List.IsSynchronized; } }
		void ICollection.CopyTo(Array array, int index) { List.CopyTo(array, index); }
		internal void AssignTo(MultiEditorRowPropertiesCollection rows) {
			if(rows == null) return;
			rows.List.Clear();
			foreach(RowProperties item in this) {
				RowProperties rowItem = gridRow.CreateRowProperties();
				item.AssignTo(rowItem);
				rows.List.Add(rowItem);
			}
		}
		protected virtual void Changed(MultiEditorRowProperties prop,  RowChangeTypeEnum changeType) {
			((IRowChangeListener)gridRow).RowPropertiesChanged(prop, changeType);
		}
		int IRowViewScaler.GetCellViewRectWidth(RowProperties props, int commonWidth, bool calcValue) {
			int propertiesIndex = GetVisibleIndex(props);
			if(!gridRow.IsConnected && propertiesIndex == -1)
				return 0;
			DevExpress.XtraVerticalGrid.Utils.RectScaleScrollerArgs args = new DevExpress.XtraVerticalGrid.Utils.RectScaleScrollerArgs(new Rectangle(Point.Empty, new Size(commonWidth, BaseRow.MinHeight)), gridRow, 0, 0, gridRow.Grid.ViewInfo.RC.separatorWidth, calcValue);
			DevExpress.XtraVerticalGrid.Utils.RectScaleScroller.Instance.CalcCellRects(args);
			if (args.Rects == null || args.Rects.Length - 1 < propertiesIndex)
				return 0;
			return args.Rects[propertiesIndex].Width;
		}
		int GetVisibleIndex(RowProperties props) {
			int i = 0;
			foreach (MultiEditorRowProperties properties in this) {
				if (this[i] == props)
					break;
				if (properties.IsVisible)
					i++;
			}
			if (this.Count <= i)
				return -1;
			return i;
		}
	}
	#region GridRow classes
	[DesignTimeVisible(false), ToolboxItem(false), 
	DefaultProperty("Properties")]
	public abstract class BaseRow : Component, IXtraSerializable, IRowChangeListener, IRowViewScaler, IServiceProvider, IAppearanceOwner, IXtraSerializableLayoutEx {
		internal const int
			MinRowHeight = 10,
			MaxRowHeight = -1;
		string styleName, name;
		VGridOptionsRow optionsRow;
		protected int fHeight, fMaxCaptionLineCount;
		protected bool fTabStop;
		VGridRows childRows;
		object tag;
		bool expanded;
		VGridRows rows;
		AppearanceObjectEx appearance;
		BaseRowHeaderInfo headerInfo;
		FixedStyle fixedStyle;
		internal int rowIndex;
		internal bool visible;
		protected BaseRow() {
			this.styleName = name = string.Empty;
			this.optionsRow = CreateOptionsRow();
			this.optionsRow.Changed += new BaseOptionChangedEventHandler(OnOptionsRowChanged); 
			this.rowIndex = -1;
			this.fHeight = -1;
			this.fMaxCaptionLineCount = 2;
			this.rows = null;
			this.appearance = new AppearanceObjectEx(this);
			this.appearance.Changed += new EventHandler(OnAppearanceChanged);
			this.childRows = CreateChildRows();
			this.expanded = true;
			this.visible = true;
			this.fTabStop = true;
			this.tag = null;
			this.headerInfo = CreateHeaderInfo();
			HeaderInfo.MakeDraft();
		}
		protected virtual VGridOptionsRow CreateOptionsRow() {
			return new VGridOptionsRow();
		}
		protected virtual VGridRows CreateChildRows() {
			return new VGridRows(null);
		}
		#region public
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ISite Site {
			get { return base.Site; }
			set {
				base.Site = value;
				if(Site == null)
					return;
				BaseRow row = Site.Component as BaseRow;
				if(row != null)
					row.Name = Site.Name;
			}
		}
		bool ShouldSerializeOptionsRow() { return OptionsRow.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("BaseRowOptionsRow"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public VGridOptionsRow OptionsRow { get { return optionsRow; } }
		protected virtual void OnOptionsRowChanged(object sender, BaseOptionChangedEventArgs e) {
			if(IsConnected)
				Grid.RowOptionsChanged(this, e);
			Changed(RowChangeTypeEnum.Options);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("BaseRowVisible"),
#endif
 DefaultValue(true), XtraSerializableProperty(), Localizable(true)]
		public bool Visible {
			get { return visible; }
			set {
				RowChangeArgs args = new RowChangeArgs(RowChangeTypeEnum.Visible, value, Visible);
				if(CanChangeRowProperty(args)) {
					visible = (bool)args.Value;
					Changed(RowChangeTypeEnum.Visible);
				}
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("BaseRowTabStop"),
#endif
 DefaultValue(true), XtraSerializableProperty(), Localizable(true)]
		public virtual bool TabStop {
			get { return fTabStop; }
			set {
				RowChangeArgs args = new RowChangeArgs(RowChangeTypeEnum.TabStop, value, TabStop);
				if(CanChangeRowProperty(args)) {
					fTabStop = (bool)args.Value;
					Changed(RowChangeTypeEnum.TabStop);
				}
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string StyleName {
			get { return styleName; }
			set { styleName = value; }
		}
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		void ResetAppearance() { Appearance.Reset(); }
		[XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		[XtraSerializablePropertyId(VGridOptionsLayout.AppearanceLayoutId)]
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("BaseRowAppearance"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObjectEx Appearance { get { return appearance; }	}
		[Browsable(false)]
		public virtual TreeButtonType TreeButtonType { get { return TreeButtonType.TreeViewButton; } }
		[Browsable(false), DefaultValue(""), XtraSerializableProperty(), Localizable(false)]
		public virtual string Name {
			get {
				if(Site != null)
					name = Site.Name;
				return name;
			}
			set {
				if(value == null)
					value = string.Empty;
				RowChangeArgs args = new RowChangeArgs(RowChangeTypeEnum.Name, value, Name);
				if(!CanChangeRowProperty(args))
					return;
				if(Site != null)
					Site.Name = (string)args.Value;
				name = (string)args.Value;
				Changed(RowChangeTypeEnum.Name);
			}
		}
		[Browsable(false)]
		public int VisibleIndex {
			get {
				if(!IsConnected)
					return -1;
				return Grid.VisibleRows.IndexOf(this);
			}
		}
		[Browsable(false)]
		public VGridControlBase Grid { 
			get {
				if(Rows == null) return null;
				return Rows.Grid;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(true, true, true)]
		public VGridRows ChildRows {
			get { return childRows; }
		}
		[Browsable(false)]
		public virtual bool HasChildren {
			get { return !ChildRows.IsEmpty; }
		}
		[Browsable(false)]
		public int Level {
			get {
				int level = 0;
				VGridRows _rows = Rows;
				while(_rows != null && _rows.ParentRow != null) {
					level++;
					_rows = _rows.ParentRow.Rows;
				}
				return level;
			}
		}
		[Browsable(false), XtraSerializableProperty()]
		public abstract int XtraRowTypeID { get ; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Index { 
			get { 
				if(IsConnected && !Rows.IsIndexesValid) {
					Rows.UpdateIndexes();
				}
				return rowIndex; 
			}
			set {
				Rows.SetRowIndex(this, value); 
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("BaseRowTag"),
#endif
 DefaultValue(null), Localizable(true),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public object Tag {
			get { return tag; }
			set {
				RowChangeArgs args = new RowChangeArgs(RowChangeTypeEnum.Tag, value, Tag);
				if(CanChangeRowProperty(args)) {
					tag = args.Value;
					Changed(RowChangeTypeEnum.Tag);
				}
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("BaseRowFixed"),
#endif
 DefaultValue(FixedStyle.None), Localizable(true)]
		public virtual FixedStyle Fixed {
			get {
				if(ParentRow != null)
					return ParentRow.Fixed;
				return fixedStyle;
			}
			set {
				RowChangeArgs args = new RowChangeArgs(RowChangeTypeEnum.Fixed, value, Fixed);
				if(CanChangeRowProperty(args)) {
					InternalFixed = (FixedStyle)args.Value;
					Changed(RowChangeTypeEnum.Fixed);
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty(), DefaultValue(FixedStyle.None)]
		public FixedStyle InternalFixed {
			get { return fixedStyle; }
			set { fixedStyle = value; }
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("BaseRowExpanded"),
#endif
 DefaultValue(true), XtraSerializableProperty(), Localizable(true)]
		public virtual bool Expanded {
			get { return expanded; }
			set {
				ChildRows.OnExpandedChanging(value);
				RowChangeArgs args = new RowChangeArgs(RowChangeTypeEnum.Expanded, value, Expanded);
				if(CanChangeRowProperty(args)) {
					expanded = (bool)args.Value;
					Changed(RowChangeTypeEnum.Expanded);
				}
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("BaseRowHeight"),
#endif
 DefaultValue(-1), XtraSerializableProperty(), Localizable(true)] 
		public virtual int Height {
			get { return fHeight; }
			set {
				if(value < MinHeight && value != -1) value = MinHeight;
				RowChangeArgs args = new RowChangeArgs(RowChangeTypeEnum.Height, value, Height);
				if(CanChangeRowProperty(args)) {
					value = (int)args.Value;
					if(value != -1)
						value = Math.Max(MinHeight, value);
					fHeight = value;
					Changed(RowChangeTypeEnum.Height);
				}
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("BaseRowMaxCaptionLineCount"),
#endif
 DefaultValue(2), XtraSerializableProperty(), Localizable(true)]
		public virtual int MaxCaptionLineCount {
			get { return fMaxCaptionLineCount; }
			set {
				if(!IsConnected || Grid.IsLoading) {
					fMaxCaptionLineCount = value;
					return;
				}
				if(value == MaxCaptionLineCount || Height != -1 || value < 0) return;
				RowChangeArgs args = new RowChangeArgs(RowChangeTypeEnum.MaxCaptionLineCount, value, MaxCaptionLineCount);
				if(CanChangeRowProperty(args)) {
					fMaxCaptionLineCount = (int)args.Value;
					Changed(RowChangeTypeEnum.MaxCaptionLineCount);
				}
			}
		}
		[Browsable(false)]
		public static int MinHeight { get { return MinRowHeight; } }
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("BaseRowProperties")]
#endif
		public virtual RowProperties Properties { get { return null; } }
		[Browsable(false)]
		public int RowPropertiesCount { get { return RowPropertiesCountCore; } }
		[Browsable(false)]
		public BaseRow ParentRow { 
			get {
				if(Rows == null) return null;
				return Rows.ParentRow;
			}
		}
		public bool HasAsParent(BaseRow gridRow) {
			BaseRow parent = ParentRow;
			while(parent != null) {
				if(gridRow == parent) return true;
				if(parent.Rows == null) return false;
				parent = parent.Rows.ParentRow;
			}
			return false;
		}
		public bool HasAsChild(BaseRow gridRow) {
			if(gridRow == null) return false;
			return gridRow.HasAsParent(this);
		}
		public RowProperties GetRowProperties(int index) {
			return GetRowPropertiesCore(index);
		}
		public abstract IEnumerable<RowProperties> GetRowProperties();
		#endregion public
		#region protected
		protected override void Dispose(bool disposing) {
			if(disposing) {
				ChildRows.DestroyRows();
				if(Rows != null) {
					Rows.Remove(this);
				}
				this.optionsRow.Changed -= new BaseOptionChangedEventHandler(OnOptionsRowChanged); 
				DisposeRowProperties();
			}
			base.Dispose(disposing);
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			Changed(RowChangeTypeEnum.StyleName);
		}
		protected virtual void Changed(RowChangeTypeEnum changeType) {
			((IRowChangeListener)this).RowPropertiesChanged(null, changeType);
		}
		protected virtual void DisposeRowProperties() {
			(Properties as IDisposable).Dispose();
		}
		protected internal virtual AppearanceObject GetRowHeaderStyle(VGridAppearanceCollection appearance) {
			return appearance.RowHeaderPanel; 
		}
		protected virtual int RowPropertiesCountCore { get { return 1; } }
		protected virtual RowProperties GetRowPropertiesCore(int index) {
			if(index != 0) return null;
			return Properties;
		}
		protected virtual bool CanChangeRowProperty(RowChangeArgs args) {
			if(!IsConnected)
				return true;
			if(args.OldValue == null) {
				if(args.Value == null) return false;
			}
			else if(args.OldValue.Equals(args.Value)) return false;
			return Grid.OnRowChanging(this, args);
		}
		object IServiceProvider.GetService(Type serviceType) {
			return GetRowService(serviceType);
		}
		protected virtual object GetRowService(Type serviceType) {
			if(serviceType.Equals(typeof(IRowViewScaler))) return this;
			return base.GetService(serviceType);
		}
		#endregion protected
		#region IXtraSerializable
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
		}
		void IXtraSerializable.OnStartSerializing() {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		protected virtual void XtraClearChildRows(XtraItemEventArgs e) {
			RowXtraDeserializer.ClearRows(e);
		}
		protected virtual object XtraCreateChildRowsItem(XtraItemEventArgs e) {
			return RowXtraDeserializer.CreateRowsItem(e, Grid);
		}
		protected virtual object XtraFindChildRowsItem(XtraItemEventArgs e) {
			return RowXtraDeserializer.FindRowsItem(e);
		}
		protected virtual void XtraSetIndexChildRowsItem(XtraSetItemIndexEventArgs e) {
			RowXtraDeserializer.SetItemIndex(e);
		}
		#endregion
		internal VGridRows Rows {
			get { return rows; }
			set {
				rows = value;
				ChildRows.ParentRow = value == null ? null : this;
			}
		}
		protected internal virtual bool IsConnected { get { return Grid != null; } }
		protected internal virtual bool IsDesignTime { get { return Site != null; } }
		public void AssignTo(BaseRow destRow) {
			if(destRow.IsConnected)
				destRow.Grid.BeginUpdate();
			try {
				AssignToCore(destRow);
			}
			finally {
				if(destRow.IsConnected)
					destRow.Grid.CancelUpdate();
			}
			destRow.Changed(RowChangeTypeEnum.RowAssigned);
		}
		protected virtual void AssignToCore(BaseRow destRow) {
			destRow.rowIndex = -1;
			destRow.visible = this.Visible;
			destRow.optionsRow.Assign(this.OptionsRow);
			destRow.styleName = this.StyleName;
			destRow.name = this.Name;
			destRow.tag = this.Tag;
			destRow.expanded = this.Expanded;
			destRow.fHeight = this.Height;
			destRow.fMaxCaptionLineCount = this.MaxCaptionLineCount;
			AssignPropertiesTo(destRow);
		}
		protected virtual void AssignPropertiesTo(BaseRow destRow) {
			if(this.Properties != null)
				this.Properties.AssignTo(destRow.Properties);
		}
		protected internal void FireChanged() {
			DevExpress.XtraVerticalGrid.Utils.DesignUtils.FireChanged(this, IsLoadingCore, DesignMode, GetService(typeof(IComponentChangeService)) as IComponentChangeService);
		}
		[Browsable(false)]
		public BaseRowHeaderInfo HeaderInfo { get { return headerInfo; } }
		protected internal abstract BaseRowViewInfo CreateViewInfo();
		protected internal abstract BaseRowHeaderInfo CreateHeaderInfo();
		protected internal abstract RowProperties CreateRowProperties();
		protected internal virtual int GetHeaderViewWidth(int headerViewWidth, int bandWidth) { return headerViewWidth; }
		void IRowChangeListener.RowPropertiesChanged(RowProperties prop, RowChangeTypeEnum changeType) {
			if(Rows != null && !this.isBlocked) 
				Rows.Changed(this, null, prop, changeType);
		}
		void IRowChangeListener.RowPropertiesChanging(RowProperties prop, RowPropertiesChangingArgs args) {
			args.CanChange = CanChangeRowProperty(args);
		}
		BaseRow IRowChangeListener.Row { get { return this; } }
		bool isBlocked;
		bool IRowChangeListener.IsBlocked { get { return isBlocked; } set { isBlocked = value; } }
		int IRowViewScaler.GetCellViewRectWidth(RowProperties props, int commonWidth, bool calcValue) { return commonWidth; }
		bool IAppearanceOwner.IsLoading { get { return IsLoadingCore; } }
		internal bool IsLoadingCore { get { return (!IsConnected || Grid.IsLoading); } }
		internal protected virtual void AppendHeaderText(System.Text.StringBuilder sb) {
			if(!IsConnected)
				return;
			sb.Append(Properties.Caption);
		}
		internal protected virtual void AppendValueText(System.Text.StringBuilder sb, int recordIndex) {
			if(!IsConnected)
				return;
			sb.Append(Grid.GetCellDisplayText(this, recordIndex));
		}
		internal protected virtual object GetData(int recordIndex) {
			if(!IsConnected)
				return null;
			return Grid.GetCellValue(this, recordIndex);
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			OnResetSerializationProperties(options);
		}
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return OnAllowSerializationProperty(options, propertyName, id);
		}
		protected virtual bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			VGridOptionsLayout opt = options as VGridOptionsLayout;
			if (opt == null)
				return true;
			if (id == VGridOptionsLayout.AppearanceLayoutId)
				return opt.StoreAppearance;
			return true;
		}
		protected virtual void OnResetSerializationProperties(OptionsLayoutBase options) {
			VGridOptionsLayout opt = options as VGridOptionsLayout;
			if (opt != null && opt.StoreAppearance) {
				Appearance.Reset();
			}
		}
#if DEBUGTEST
		public override string ToString() {
			return Properties.GetTextCaption();
		}
#endif
	}
	public class CategoryRow : BaseRow {
		RowProperties prop;
		public CategoryRow() : this(string.Empty) {}
		public CategoryRow(string fCaption) {
			this.prop = CreateRowProperties();
			this.prop.Caption = fCaption;
			this.prop.sink = this;
			this.fMaxCaptionLineCount = 1;
			this.fTabStop = false;
		}
		public override IEnumerable<RowProperties> GetRowProperties() {
			yield return Properties;
		}
		protected override void AssignToCore(BaseRow destRow) {
			base.AssignToCore(destRow);
			if(destRow is CategoryRow) {
				CategoryRow categoryRow  = destRow as CategoryRow;
			}
		}
		protected internal override BaseRowViewInfo CreateViewInfo() {
			return new CategoryRowViewInfo(this);
		}
		protected internal override BaseRowHeaderInfo CreateHeaderInfo() {
			return new CategoryRowHeaderInfo(this);
		}
		protected internal override RowProperties CreateRowProperties() {
			return new CategoryRowProperties();
		}
		protected internal override int GetHeaderViewWidth(int headerViewWidth, int bandWidth) { return bandWidth; }
		protected internal override AppearanceObject GetRowHeaderStyle(VGridAppearanceCollection appearance) {
			return new AppearanceObject(Appearance, appearance.Category);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("CategoryRowProperties"),
#endif
 TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public override RowProperties Properties {
			get { return prop; }
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("CategoryRowTabStop"),
#endif
 DefaultValue(false)]
		public override bool TabStop { get { return base.TabStop; } set { base.TabStop = value; } }
		[Browsable(false), XtraSerializableProperty()]
		public override int XtraRowTypeID { get { return RowTypeIdConsts.CategoryRowTypeId; } }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("CategoryRowMaxCaptionLineCount"),
#endif
 DefaultValue(1)]
		public override int MaxCaptionLineCount { get { return base.MaxCaptionLineCount; } set { base.MaxCaptionLineCount = value; } }
		[Browsable(false)]
		public override TreeButtonType TreeButtonType { 
			get {
				if(!IsConnected || Grid.TreeButtonStyle == TreeButtonStyle.TreeView) return TreeButtonType.TreeViewButton;
				if(this.Grid.Scroller.ScrollInfo.IsOverlapScrollbar) return TreeButtonType.TreeViewButton;
				if(Grid.TreeButtonStyle == TreeButtonStyle.ExplorerBar) return TreeButtonType.ExplorerBarButton;
				if(Grid.ElementsLookAndFeel.ActiveStyle == LookAndFeel.ActiveLookAndFeelStyle.WindowsXP ||
					Grid.ElementsLookAndFeel.ActiveStyle == LookAndFeel.ActiveLookAndFeelStyle.Skin) return TreeButtonType.ExplorerBarButton;
				return TreeButtonType.TreeViewButton;
			} 
		}
	}
	public class EditorRow : BaseRow {
		RowProperties prop;
		bool enabled;
		public EditorRow() : this(string.Empty) {}
		public EditorRow(string fFieldName) {
			this.prop = CreateRowProperties();
			this.prop.FieldName = fFieldName;
			this.prop.sink = this;
			this.enabled = true;
		}
		public override IEnumerable<RowProperties> GetRowProperties() {
			yield return Properties;
		}
		protected override void AssignToCore(BaseRow destRow) {
			base.AssignToCore(destRow);
			if(destRow is EditorRow) {
				EditorRow eeRow = destRow as EditorRow;
				eeRow.enabled = this.Enabled;
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("EditorRowEnabled"),
#endif
 DefaultValue(true), XtraSerializableProperty(), Localizable(true)]
		public bool Enabled {
			get { return enabled; }
			set {
				RowChangeArgs args = new RowChangeArgs(RowChangeTypeEnum.Enabled, value, Enabled);
				if(CanChangeRowProperty(args)) {
					enabled = (bool)args.Value;
					Changed(RowChangeTypeEnum.Enabled);
				}
			}
		}
		protected internal override BaseRowViewInfo CreateViewInfo() {
			return new EditorRowViewInfo(this);
		}
		protected internal override BaseRowHeaderInfo CreateHeaderInfo() {
			return new EditorRowHeaderInfo(this);
		}
		protected internal override RowProperties CreateRowProperties() {
			return new RowProperties();
		}
		protected internal override AppearanceObject GetRowHeaderStyle(VGridAppearanceCollection appearance) {
			return base.GetRowHeaderStyle(appearance);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("EditorRowProperties"),
#endif
 TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public override RowProperties Properties {
			get { return prop; }
		}
		[Browsable(false), XtraSerializableProperty()]
		public override int XtraRowTypeID { get { return RowTypeIdConsts.EditorRowTypeId; } }
	}
	[DefaultProperty("PropertiesCollection")]
	public class MultiEditorRow : EditorRow {
		MultiEditorRowPropertiesCollection propCollection;
		string separatorString;
		SeparatorKind separatorKind;
		public MultiEditorRow() {
			this.propCollection = new MultiEditorRowPropertiesCollection(this);
			this.separatorString = string.Empty;
			this.separatorKind = SeparatorKind.VertLine;
		}
		public override IEnumerable<RowProperties> GetRowProperties() {
			return PropertiesCollection.Cast<RowProperties>();
		}
		protected override void AssignToCore(BaseRow destRow) {
			base.AssignToCore(destRow);
			if(destRow is MultiEditorRow) {
				MultiEditorRow meRow = destRow as MultiEditorRow;
				meRow.separatorString = this.SeparatorString;
				meRow.separatorKind = this.SeparatorKind;
			}
		}
		protected override void AssignPropertiesTo(BaseRow destRow) {
			if(destRow is MultiEditorRow) {
				MultiEditorRow meRow = destRow as MultiEditorRow;
				this.PropertiesCollection.AssignTo(meRow.PropertiesCollection);
			}
			else base.AssignPropertiesTo(destRow);
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override RowProperties Properties {
			get { 
				if(PropertiesCollection.Count == 0) return null;
				return PropertiesCollection[0]; 
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("MultiEditorRowPropertiesCollection"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(true, true, true)]
		public MultiEditorRowPropertiesCollection PropertiesCollection {
			get { return propCollection; }
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("MultiEditorRowSeparatorString"),
#endif
 DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		public virtual string SeparatorString {
			get { return separatorString; }
			set {
				if(value == null) value = string.Empty;
				RowChangeArgs args = new RowChangeArgs(RowChangeTypeEnum.SeparatorString, value, SeparatorString);
				if(CanChangeRowProperty(args)) {
					separatorString = (string)args.Value;
					Changed(RowChangeTypeEnum.SeparatorString);
				}
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("MultiEditorRowSeparatorKind"),
#endif
 DefaultValue(SeparatorKind.VertLine), XtraSerializableProperty(), Localizable(true)]
		public virtual SeparatorKind SeparatorKind {
			get { return separatorKind; }
			set {
				RowChangeArgs args = new RowChangeArgs(RowChangeTypeEnum.SeparatorKind, value, SeparatorKind);
				if(CanChangeRowProperty(args)) {
					separatorKind = (SeparatorKind)args.Value;
					Changed(RowChangeTypeEnum.SeparatorKind);
				}
			}
		}
		[Browsable(false), XtraSerializableProperty()]
		public override int XtraRowTypeID { get { return RowTypeIdConsts.MultiEditorRowTypeId; } }
		protected override int RowPropertiesCountCore {
			get { return PropertiesCollection.Count; }
		}
		protected override RowProperties GetRowPropertiesCore(int index) {
			return PropertiesCollection[index];
		}
		protected internal override BaseRowViewInfo CreateViewInfo() {
			return new MultiEditorRowViewInfo(this);
		}
		protected internal override BaseRowHeaderInfo CreateHeaderInfo() {
			return new MultiEditorRowHeaderInfo(this);
		}
		protected internal override RowProperties CreateRowProperties() {
			return new MultiEditorRowProperties();
		}
		protected virtual void XtraClearPropertiesCollection(XtraItemEventArgs e) {
			PropertiesCollection.Clear();
		}
		protected virtual object XtraCreatePropertiesCollectionItem(XtraItemEventArgs e) {
			return CreateRowProperties();
		}
		protected virtual object XtraFindPropertiesCollectionItem(XtraItemEventArgs e) {
			return RowXtraDeserializer.FindRowProperties(e);
		}
		protected virtual void XtraSetIndexPropertiesCollectionItem(XtraSetItemIndexEventArgs e) {
			RowXtraDeserializer.SetIndexPropertiesCollectionItem(e);
		}
		protected override void DisposeRowProperties() {
			if(this.propCollection == null)
				return;
			foreach(RowProperties prop in PropertiesCollection) {
				(prop as IDisposable).Dispose();
			}
			PropertiesCollection.Clear();
			this.propCollection = null;
		}
		protected override object GetRowService(Type serviceType) {
			if(serviceType.Equals(typeof(IRowViewScaler))) return PropertiesCollection;
			return base.GetService(serviceType);
		}
		public override string ToString() {
			string result = string.Empty;
			foreach(RowProperties rowProperties in PropertiesCollection) {
				result += rowProperties.GetTextCaption() + "; ";
			}
			return result;
		}
		internal protected override void AppendHeaderText(System.Text.StringBuilder sb) {
			if(!IsConnected) return;
			if(RowPropertiesCount == 0) return;
			int lastIndex = RowPropertiesCount - 1;
			for(int i = 0; i < lastIndex; i++) {
				sb.Append(PropertiesCollection[i].Caption);
				sb.Append(GetSeparatorString());
			}
			sb.Append(PropertiesCollection[lastIndex].Caption);
		}
		internal protected override void AppendValueText(System.Text.StringBuilder sb, int recordIndex) {
			if(!IsConnected) return;
			if(RowPropertiesCount == 0) return;
			int lastIndex = RowPropertiesCount - 1;
			for(int i = 0; i < lastIndex; i++) {
				sb.Append(Grid.GetCellDisplayText(this, Grid.FocusedRecord, i));
				sb.Append(GetSeparatorString());
			}
			sb.Append(Grid.GetCellDisplayText(this, Grid.FocusedRecord, lastIndex));
		}
		string GetSeparatorString() {
			if(SeparatorKind == SeparatorKind.String)
				return " " + SeparatorString + " ";
			if(SeparatorKind == SeparatorKind.VertLine)
				return " ";
			return " ";
		}
		internal protected override object GetData(int recordIndex) {
			if(!IsConnected) return null;
			if(RowPropertiesCount == 0) return null;
			object[] objects = new object[RowPropertiesCount];
			for(int i = 0; i < RowPropertiesCount; i++) {
				objects[i] = Grid.GetCellValue(this, recordIndex, i);
			}
			return objects;
		}
		internal MultiEditorRowProperties GetNextVisibleProperties(int startIndex) {
			for (int i = startIndex; i < RowPropertiesCount; i++) {
				if (PropertiesCollection[i].IsVisible)
					return PropertiesCollection[i];
			}
			return null;
		}
		internal protected virtual IList<MultiEditorRowProperties> GetVisibleRowProperties() {
			List<MultiEditorRowProperties> list = new List<MultiEditorRowProperties>();
			foreach (MultiEditorRowProperties properties in PropertiesCollection) {
				if (properties.IsVisible)
					list.Add(properties);
			}
			if (list.Count == 0)
				list.Add(MultiEditorRowViewInfo.NullProperties);
			return list;
		}
	}
	#endregion GridRow classes
	#region class VGridRows
	[ListBindable(false)]
	public class VGridRows : CollectionBase, IEnumerable {
		VGridControlBase grid;
		BaseRow parent;
		internal VGridRows(VGridControlBase grid) {
			this.grid = grid;
			this.ParentRow = null;
		}
		[Browsable(false)]
		public VGridControlBase Grid { get { return ParentRow == null ? grid : ParentRow.Grid; } }
		internal BaseRow ParentRow {
			get { return parent; }
			set {
				parent = value;
				SetParentToChildRows(value);
			}
		}
		protected virtual void SetParentToChildRows(BaseRow parentRow) {
			foreach(BaseRow childRow in this)
				childRow.Rows = parentRow == null ? null : parentRow.ChildRows;
		}
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("VGridRowsItem")]
#endif
		public virtual bool IsEmpty {
			get {
				return IsLoaded ? Count == 0 : false;
			}
		}
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("VGridRowsCount")]
#endif
		public new virtual int Count { get { return base.Count; } }
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("VGridRowsItem")]
#endif
		public virtual BaseRow this[int index] { 
			get { 
				if(index < 0 || index > List.Count - 1) return null;
				return (BaseRow)List[index];  }  
		}
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("VGridRowsItem")]
#endif
		public virtual BaseRow this[string name] { 
			get {
				int c = List.Count;
				for(int n = 0; n < c; n++) {
					BaseRow row = this[n];
					if(row.Name == name) return row;
					if(row.HasChildren) {
						row = row.ChildRows[name];
						if(row != null) return row;
					}
				}
				return null;
			}
		}
		public virtual BaseRow GetRowByFieldName(string fieldName) {
			return GetRowByFieldName(fieldName, false);
		}
		internal BaseRow GetLoadedRowByFieldName(string fieldName, bool recursive) {
			return GetRowByFieldNameCore(fieldName, recursive);
		}
		protected virtual BaseRow GetRowByFieldNameCore(string fieldName, bool recursive) {
			if(Grid == null)
				return null;
			Grid.BeginUpdate();
			FindRowByFieldName operation = new FindRowByFieldName(fieldName, recursive);
			Grid.RowsIterator.DoLocalOperation(operation, this);
			Grid.CancelUpdate();
			return operation.Row;
		}
		public virtual BaseRow GetRowByFieldName(string fieldName, bool recursive) {
			return GetRowByFieldNameCore(fieldName, recursive);
		}
		public virtual int Add(BaseRow row) { 
			return List.Add(row); 
		}
		public virtual void AddRange(BaseRow[] rows) {
			if(rows == null) return;
			for (IEnumerator e = rows.GetEnumerator(); e.MoveNext();) {
				List.Add(e.Current);
			}
		}
		internal virtual bool IsLoadedCore { get { return true; } set { } }
		internal bool IsLoaded { get { return IsLoadedCore; } }
#if DEBUGTEST
		public override string ToString() {
			if(Grid == null)
				return "Disconnected";
			CountOperation count = new CountOperation();
			Grid.RowsIterator.DoLocalOperation(count, this);
			return "RowCount: " + count.RowCount.ToString();
		}
#endif
		public void Insert(BaseRow row, int index) {
			if(index < 0) index = 0;
			if(index >= Count)  {
				Add(row);
				return;
			}
			List.Insert(index, row);
		}
		public int IndexOf(BaseRow Row) {
			return List.IndexOf(Row);
		}
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("VGridRowsVisibleRowsCount")]
#endif
		public int VisibleRowsCount { 
			get {
				int count = 0;
				for(int i = 0; i < Count; i++)
					if(this[i].Visible) count++;
				return count;
			}
		}
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("VGridRowsVisibleRowsCount")]
#endif
		public virtual bool HasVisibleItems {
			get { return IsLoaded ? VisibleRowsCount > 0 : true; }
		}
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("VGridRowsFirstVisible")]
#endif
		public BaseRow FirstVisible {
			get {
				for(int i = 0; i < Count; i++)
					if(this[i].Visible) return this[i];
				return null;
			}
		}
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("VGridRowsLastVisible")]
#endif
		public BaseRow LastVisible {
			get {
				for(int i = Count - 1; i > -1; i--)
					if(this[i].Visible) return this[i];
				return null;
			}
		}
		internal void Sort(IComparer comparer) {
			InnerList.Sort(comparer);
		}
		public void Remove(BaseRow row) { List.Remove(row); }
		internal static void Move(BaseRow sourceRow, BaseRow destRow, VGridRows destCollection, bool before) {
			sourceRow.Rows.Remove(sourceRow);
			if(destRow == null)
				destCollection.Add(sourceRow);
			else {
				int index = before ? destRow.Index : destRow.Index + 1;
				destCollection.Insert(sourceRow, index);
			}
			destCollection.Changed(sourceRow, destCollection, null, RowChangeTypeEnum.Move);
		}
		internal bool IsIndexesValid { get; set; }
		internal void InvalidateIndexes() {
			IsIndexesValid = false;
		}
		internal void UpdateIndexes() {
			IsIndexesValid = true;
			for(int i = 0; i < Count; i++) {
				this[i].rowIndex = i;
			}
		}
		internal void SetRowIndex(BaseRow row, int index) {
			Remove(row);
			Insert(row, index);
			Changed(row, this, null, RowChangeTypeEnum.Index);
		}
		protected override void OnInsertComplete(int index, object value) {
			BaseRow row = value as BaseRow;
			if(row == null || row.Rows != null)
				return;
			row.Rows = this;
			if(Grid == null)
				return;
			InvalidateIndexes();
			Grid.RowsIterator.DoLocalOperation(new AddToContainer(Grid.Container), row);
			Grid.RowsIterator.DoLocalOperation(new RowChangedOperation(RowChangeTypeEnum.Add), row);
		}
		protected internal void FireChanged() {
			if (Grid == null) return;
			Grid.RowsIterator.DoOperation(new RowFireChanged());
		}
		protected override void OnRemove(int index, object value) {
			BaseRow row = value as BaseRow;
			if(row.Rows == this)
				row.Rows = null;
		}
		protected void DoRecursive(Action<BaseRow> Action) {
			for(int i = Count - 1; i > -1; i--) {
				BaseRow row = this[i];
				Action(row);
				row.ChildRows.DoRecursive(Action);
			}
		}
		protected override void OnRemoveComplete(int index, object value) {
			if(Grid == null)
				return;
			InvalidateIndexes();
			BaseRow row = (BaseRow)value;
			Grid.RowsIterator.DoLocalOperation(new DelegateRowOperation((executingRow) => { Grid.UniqueNames.RemoveName(executingRow.Name); }), row);
			Changed(row, this, null, RowChangeTypeEnum.Delete);
		}
		protected override void OnClear() {
			ClearRows(false);
		}
		protected internal virtual void DestroyRows() {
			ClearRows(true);
		}
		protected internal virtual void ClearRows(bool dispose) {
			for(int i = List.Count - 1; i > -1; i--) {
				BaseRow row = this[i];
				if(row.Rows == this && row.IsDesignTime || dispose) {
					row.Dispose();
				} else {
					RemoveAt(i);
				}
			}
		}
		protected override void OnValidate(object value) {
			if(value == null) base.OnValidate(value);
			if(!(value is BaseRow))throw new ArgumentException("Invalid argument type");
		}
		protected internal void Changed(BaseRow Row, VGridRows Rows, RowProperties prop, RowChangeTypeEnum rowProperty) {
			if(Grid != null) Grid.OnRowChanged(Row, Rows, prop, rowProperty);
		}
		protected internal virtual void OnExpandedChanging(bool value) { }
		#region IEnumerable Members
		public new virtual IEnumerator GetEnumerator() {
			return base.GetEnumerator();
		}
		#endregion
	}
	#endregion
	[ListBindable(false)]
	public class PGridVirtualRows : VGridRows {
		bool isLoadedCore = false;
		public PGridVirtualRows(PropertyGridControl grid)
			: base(grid) {
		}
		internal override bool IsLoadedCore {
			get {
				if (isLoadedCore)
					return true;
				if (ParentRow == null || PGrid == null)
					return false;
				DescriptorContext context = PGrid.DataModeHelper.GetDescriptorContext(ParentRow.Properties.FieldName);
				if (context == null)
					return false;
				return !context.IsGetPropertiesSupported; }
			set { isLoadedCore = value; }
		}
		protected PropertyGridControl PGrid { get { return (PropertyGridControl)Grid; } }
		public override int Count {
			get {
				EnsureIsLoaded();
				return base.Count;
			}
		}
		public override BaseRow this[string name] {
			get {
				EnsureIsLoaded();
				return base[name];
			}
		}
		public override BaseRow this[int index] {
			get {
				EnsureIsLoaded();
				return base[index];
			}
		}
		public override int Add(BaseRow row) {
			EnsureIsLoaded();
			return base.Add(row);
		}
		public override void AddRange(BaseRow[] rows) {
			EnsureIsLoaded();
			base.AddRange(rows);
		}
		protected override void SetParentToChildRows(BaseRow parentRow) {
			if(!IsLoaded)
				return;
			base.SetParentToChildRows(parentRow);
		}
		protected internal override void OnExpandedChanging(bool value) {
			if(value && PGrid != null) {
				PGrid.UnlockRowLoading();
				EnsureIsLoaded();
				PGrid.LockRowLoading();
			}
			base.OnExpandedChanging(value);
		}
		protected void EnsureIsLoaded() {
			if(IsLoaded || (Grid != null && (Grid.IsLoading || !PGrid.IsUnlockedRowLoading)))
				return;
			if(PGrid != null && ParentRow != null) {
				PGrid.RetrieveFieldsCore(this, false, ParentRow.Properties.FieldName, false, PGrid.AutoGenerateRows);
			}
		}
		public override BaseRow GetRowByFieldName(string fieldName, bool recursive) {
			PGrid.UnlockRowLoading();
			BaseRow row = GetRowByFieldNameCore(fieldName, recursive);
			PGrid.LockRowLoading();
			return row;
		}
		public override IEnumerator GetEnumerator() {
			EnsureIsLoaded();
			return base.GetEnumerator();
		}
	}
	class DefaultEditorConverter : TypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object val, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor) && val is DefaultEditor) {
				ConstructorInfo ctor = typeof(DefaultEditor).GetConstructor(new Type[] { typeof(Type), typeof(RepositoryItem) });
				if(ctor != null) {
					DefaultEditor edit = (DefaultEditor)val;
					return new InstanceDescriptor(ctor, new object[] { edit.EditingType, edit.Edit });
				}
			}
			return base.ConvertTo(context, culture, val, destinationType);
		}
	}
	class EditingTypeConverter : TypeConverter {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string type = value as string;
			if(type != null)
				return Type.GetType(type);
			return base.ConvertFrom(context, culture, value);
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection values = new StandardValuesCollection(new Type[] {
				typeof(Color), typeof(bool), typeof(DateTime), typeof(string),
				typeof(int), typeof(long), typeof(TimeSpan), typeof(byte),
				typeof(short), typeof(char), typeof(decimal), typeof(float),
				typeof(double),
			});
			return values;
		}
	}
	[TypeConverter(typeof(DefaultEditorConverter))]
	public class DefaultEditor {
		PropertyGridControl owner;
		RepositoryItem edit;
		Type editingType;
		public DefaultEditor() { }
		public DefaultEditor(Type editingType, RepositoryItem edit) {
			this.edit = edit;
			this.editingType = editingType;
		}
		[TypeConverter(typeof(EditingTypeConverter))]
		public Type EditingType {
			get {
				return editingType;
			}
			set {
				editingType = value;
			}
		}
		[DefaultValue(null), Localizable(true),
		Editor("DevExpress.XtraVerticalGrid.Design.DefaultEditEditor, " + AssemblyInfo.SRAssemblyVertGridDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual RepositoryItem Edit {
			get {
				return edit;
			}
			set {
				edit = value;
			}
		}
		public void SetOwner(PropertyGridControl owner) { this.owner = owner; }
		public PropertyGridControl GetOwner() { return owner; }
	}
	#region class PGridDefaultEditors
	[ListBindable(false)]
	public class PGridDefaultEditors : CollectionBase {
		internal PropertyGridControl grid;
		internal PGridDefaultEditors(PropertyGridControl grid) {
			this.grid = grid;
		}
		[Browsable(false)]
		public PropertyGridControl Grid { get { return grid; } }
		public DefaultEditor this[int index] {
			get {
				if(index < 0 || index > List.Count - 1) return null;
				return (DefaultEditor)List[index];
			}
		}
		public DefaultEditor this[Type type] {
			get {
				int c = List.Count;
				for(int n = 0; n < c; n++) {
					DefaultEditor edit = this[n];
					if(edit.EditingType == type) return edit;
				}
				return null;
			}
		}
		public int Add(DefaultEditor edit) {
			Grid.ConfigurePredefinedProperties(edit);
			return List.Add(edit); 
		}
		public int Add(Type editingType, RepositoryItem edit) { return Add(new DefaultEditor(editingType, edit)); }
		public virtual void AddRange(DefaultEditor[] editors) {
			if(editors == null) return;
			int count = editors.Length;
			for(int i = 0; i < count; i++) {
				Add(editors[i]);
			}
		}
		public void Remove(DefaultEditor edit) { List.Remove(edit); }
		protected override void OnInsertComplete(int index, object value) {
			DefaultEditor editor = value as DefaultEditor;
			if(editor == null) return;
			editor.SetOwner(grid);
			InvalidateGridLayout();
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			InvalidateGridLayout();
		}
		void InvalidateGridLayout() {
			grid.InvalidateUpdate();
		}
	}
	#endregion
	public interface IRetrievable {
		int RetrieveIndex { get; set; }
	}
	public class PGridEditorRow : EditorRow, IRetrievable {
		int retrieveIndex;
		public PGridEditorRow() : this(string.Empty) { }
		public PGridEditorRow(string fieldName) : base(fieldName) { }
		[DefaultValue(false), XtraSerializableProperty()]
		public virtual bool IsChildRowsLoaded {
			get { return ChildRows.IsLoadedCore; }
			set { ChildRows.IsLoadedCore = value; }
		}
		protected override void AssignToCore(BaseRow destRow) {
			base.AssignToCore(destRow);
			IRetrievable retrivable = destRow as IRetrievable;
			if(retrivable != null) {
				retrivable.RetrieveIndex = this.retrieveIndex;
			}
		}
		protected override VGridRows CreateChildRows() {
			return new PGridVirtualRows(null);
		}
		#region IRetrivable Members
		int IRetrievable.RetrieveIndex { get { return retrieveIndex; } set { retrieveIndex = value; } }
		#endregion
	}
}
