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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraLayout;
namespace DevExpress.XtraDataLayout {
	public class IconsHlper {
		[ThreadStatic]
		static Image bindingIcon;
		[ThreadStatic]
		static Image checkIcon;
		public static Image BindingIcon {
			get {
				if(bindingIcon == null) {
					bindingIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Images.binding.png", Assembly.GetExecutingAssembly());
				}
				return bindingIcon;
			}
		}
		public static Image CheckIcon {
			get {
				if(checkIcon == null) {
					checkIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Images.checked.png", Assembly.GetExecutingAssembly());
				}
				return checkIcon;
			}
		}
	}
	public interface DataLayoutControlDesignerMethods {
		ICollection GetFieldsList();
	}
	[ComplexBindingProperties("DataSource", "DataMember"),
	 Designer(LayoutControlConstants.DataLayoutControlDesignerName, typeof(System.ComponentModel.Design.IDesigner)),
	 ToolboxBitmap(typeof(LayoutControl), "Images.datalayoutcontrol.bmp"),
	 Description("Provides advanced capabilities to create, customize and maintain a consistent layout for editing a specific data source's fields."),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation), DXToolboxItem(DXToolboxItemKind.Free)
]
	public class DataLayoutControl :LayoutControl, DataLayoutControlDesignerMethods {
		object dataSource = null;
		string dataMember = "";
		DataController dataController;
		ControlsManager controlsManager;
		DefaultEditorsRepository defaultEditorsRepository;
		public DataLayoutControl() {
			UseLocalBindingContext = true;
			controlsManager = CreateControlsManager();
			SetupDataController();
			defaultEditorsRepository = new DefaultEditorsRepository() { SupportEditMaskFormPrimitiveTypes = true };
		}
		public override ISite Site {
			get {
				return base.Site;
			}
			set {
				if(value == null) {
					StackTrace st = new StackTrace(System.Threading.Thread.CurrentThread, false);
					if(st != null) {
						StackFrame sf = st.GetFrame(4);
						if(sf != null && sf.GetMethod().Name == "GenerateDataTable") return;
					}
				}
				base.Site = value;
			}
		}
		protected virtual void SetupDataController() {
			this.dataController = CreateDataController();
			this.dataController.ListSourceChanged += new EventHandler(OnDataController_DataSourceChanged);
			this.dataController.VisibleRowCountChanged += new EventHandler(OnDataController_VisibleRowCountChanged);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ControlsManager ControlsManager {
			get { return controlsManager; }
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(DataControllerCore != null) OnDataSourceChanged();
			UpdateCurrencyManagerSubscription();
			UpdateItemsByAttributies();
		}
		void UpdateItemsByAttributies() {
			LayoutElementsBindingInfo info = GetLayoutElementsBindingsInfo();
			UpdateItemsByAttributiesCore(info);
		}
		void UpdateItemsByAttributiesCore(LayoutElementsBindingInfo layoutElementsBindingInfo) {
			foreach(var item in layoutElementsBindingInfo.bindingsInfo) {
				if(item.InnerLayoutElementsBindingInfo != null) {
					UpdateItemsByAttributiesCore(item.InnerLayoutElementsBindingInfo);
				}
				UpdateItemByLayoutElementBindingInfo(item);
			}
		}
		void UpdateItemByLayoutElementBindingInfo(LayoutElementBindingInfo item) {
			foreach(BaseLayoutItem bli in Items) {
				LayoutControlItem lci = bli as LayoutControlItem;
				if(lci != null && lci.Control != null) {
					Control control = lci.Control;
					foreach(Binding binding in control.DataBindings) {
						if(binding.DataSource == DataSource) {
							if(item.DataMember == binding.BindingMemberInfo.BindingMember) {
								lci.itemAnnotationAttributes = item.AnnotationAttributes;
								lci.ComplexUpdate(true, true, true);
								return;
							}
						}
					}
				}
			}
		}
		protected override void OnHandleDestroyed(EventArgs e) {
			UnSubscribeCurrencyManagerEvents();
			base.OnHandleDestroyed(e);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ForceInitialize() {
			OnDataSourceChanged();
		}
		IDisposable subscription;
		protected virtual void SubscribeCurrencyManagerEvents() {
			subscription = new CurrencyManagerSubscriber(this);
		}
		protected virtual void UnSubscribeCurrencyManagerEvents() {
			if(subscription != null)
				subscription.Dispose();
			subscription = null;
		}
		void UpdateCurrencyManagerSubscription() {
			UnSubscribeCurrencyManagerEvents();
			SubscribeCurrencyManagerEvents();
		}
		class CurrencyManagerSubscriber :IDisposable {
			DataLayoutControl Owner;
			CurrencyManager Source;
			internal CurrencyManagerSubscriber(DataLayoutControl owner) {
				this.Owner = owner;
				if(!((ILayoutControl)owner).DesignMode) {
					if(owner.DataSource != null && owner.BindingContext != null)
						this.Source = (CurrencyManager)owner.BindingContext[owner.DataSource];
					if(Source != null)
						Source.PositionChanged += Owner.RaiseCurrentRowChanged;
				}
			}
			public void Dispose() {
				if(Source != null)
					Source.PositionChanged -= Owner.RaiseCurrentRowChanged;
				Source = null;
				Owner = null;
				GC.SuppressFinalize(this);
			}
		}
		protected override void OnBindingContextChanged() {
			try {
			base.OnBindingContextChanged();
			} catch {
				DestroyDataController();
				ArrayList localControls = new ArrayList(Controls);
				foreach(Control control in localControls) if(!((ILayoutDesignerMethods)this).IsInternalControl(control)) control.DataBindings.Clear();
				dataSource = null;
				dataMember = string.Empty;
				SetupDataController();
				if(DesignMode) 
					XtraMessageBox.Show("The data bindings of inner controls have been removed. The DataLayoutControl's DataSource and DataMember properties have been reset.",  
					DevExpress.XtraEditors.Controls.Localizer.Active.GetLocalizedString(DevExpress.XtraEditors.Controls.StringId.CaptionError), 
					MessageBoxButtons.OK, MessageBoxIcon.Error);; 
			}
			UpdateCurrencyManagerSubscription();
		}
		public event EventHandler CurrentRecordChanged;
		void RaiseCurrentRowChanged(object sender, EventArgs e) {
			if(CurrentRecordChanged != null) CurrentRecordChanged(sender, e);
		}
		protected virtual DataController CreateDataController() {
			return new DataController();
		}
		protected virtual void OnDataController_VisibleRowCountChanged(object sender, EventArgs e) {
		}
		protected virtual void OnDataController_DataSourceChanged(object sender, EventArgs e) {
		}
		internal ListSourceDataController DataControllerCore { get { return dataController; } }
		protected virtual void DestroyDataController() {
			if(DataControllerCore != null) {
				this.dataController.ListSourceChanged -= new EventHandler(OnDataController_DataSourceChanged);
				this.dataController.VisibleRowCountChanged -= new EventHandler(OnDataController_VisibleRowCountChanged);
				this.dataController.Dispose();
				this.dataController = null;
			}
		}
		protected internal virtual void RecreateDataController() {
			if(DataControllerCore == null) return;
			DestroyDataController();
		}
		protected override void Dispose(bool disposing) {
			if(!disposing) { base.Dispose(disposing); return; }
			UnSubscribeCurrencyManagerEvents();
			ArrayList localControls = new ArrayList(Controls);
			foreach(Control control in localControls) if(!((ILayoutDesignerMethods)this).IsInternalControl(control)) control.DataBindings.Clear();
			DestroyDataController();
			base.Dispose(disposing);
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("DataLayoutControlDataSource"),
#endif
		AttributeProvider(typeof(IListSource)),
		TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design"),
		DefaultValue(null), Category("Data")]
		public virtual object DataSource {
			get { return dataSource; }
			set {
				if(value == DataSource) return;
				if(value != null && DataSource != null && DataSource.Equals(value)) return;
				if(IsValidDataSource(value)) {
					SetDataSourceCore(value);
				}
			}
		}
		protected void SetDataSourceCore(object value) {
			this.dataSource = value;
			if(!IsUpdateLocked) OnDataSourceChanged();
			UpdateControlBindings(DataSource);
		}
		protected void UpdateControlBindings(object newDataSource) {
			if(newDataSource == null) return;
			List<string> columns = GetColumns();
			foreach(Control control in Controls) {
				if(((ILayoutDesignerMethods)this).IsInternalControl(control)) continue;
				ArrayList controlBindings = new ArrayList(control.DataBindings);
				foreach(Binding b in controlBindings) {
					bool fDataMemberValid = columns.Contains(b.BindingMemberInfo.BindingMember);
					if(b.DataSource != newDataSource && fDataMemberValid) {
						control.DataBindings.Remove(b);
						control.DataBindings.Add(
								b.PropertyName,
								newDataSource,
								b.BindingMemberInfo.BindingMember,
								b.FormattingEnabled,
								b.DataSourceUpdateMode,
								b.NullValue,
								b.FormatString,
								b.FormatInfo
							);
					}
				}
			}
		}
		List<string> GetColumns() {
			List<string> columns = new List<string>();
			LayoutElementsBindingInfo info = GetLayoutElementsBindingsInfo();
			GetColumnsCore(columns, info);
			return columns;
		}
		private static void GetColumnsCore(List<string> columns, LayoutElementsBindingInfo info) {
			foreach(LayoutElementBindingInfo column in info.GetAllBindings()) {
				if(column.InnerLayoutElementsBindingInfo != null) {
					GetColumnsCore(columns, column.InnerLayoutElementsBindingInfo);
					continue;
				}
				columns.Add(column.DataMember);
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("DataLayoutControlDataMember"),
#endif
 DefaultValue(""), Editor(ControlConstants.DataMemberEditor, typeof(System.Drawing.Design.UITypeEditor)), Localizable(true), Category("Data")]
		public string DataMember {
			get { return dataMember; }
			set {
				if(DataMember == value) return;
				dataMember = value;
				OnDataMemberChanged();
			}
		}
		ICollection DataLayoutControlDesignerMethods.GetFieldsList() {
			return this.DataControllerCore.Columns;
		}
		protected bool IsValidDataSource(object dataSource) {
			if(dataSource == null) return true;
			if(dataSource is IList) return true;
			if(dataSource is IListSource) return true;
			if(dataSource is DataSet) return true;
			if(dataSource is System.Data.DataView) {
				System.Data.DataView dv = dataSource as System.Data.DataView;
				if(dv.Table == null) return false;
				return true;
			}
			if(dataSource is System.Data.DataTable) return true;
			return false;
		}
		bool autoRetrieveFields = false;
		[ DefaultValue(false)]
		public bool AutoRetrieveFields {
			get {
				return autoRetrieveFields;
			}
			set {
				autoRetrieveFields = value;
			}
		}
		DefaultBoolean allowGeneratingNestedGroups = DefaultBoolean.Default;
		[ DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AllowGeneratingNestedGroups {
			get {
				return allowGeneratingNestedGroups;
			}
			set {
				allowGeneratingNestedGroups = value;
			}
		}
		protected virtual void OnDataSourceChanged() {
			try {
			DataControllerCore.SetListSource(BindingContext, DataSource, DataMember);
			} catch {
				DestroyDataController();
				ArrayList localControls = new ArrayList(Controls);
				foreach(Control control in localControls) if(!((ILayoutDesignerMethods)this).IsInternalControl(control)) control.DataBindings.Clear();
				dataSource = null;
				dataMember = string.Empty;
				SetupDataController();
				if(DesignMode)
					XtraMessageBox.Show("The data bindings of inner controls have been removed. The DataLayoutControl's DataSource and DataMember properties have been reset.",
					DevExpress.XtraEditors.Controls.Localizer.Active.GetLocalizedString(DevExpress.XtraEditors.Controls.StringId.CaptionError),
					MessageBoxButtons.OK, MessageBoxIcon.Error); ; 
				return;
			}
			if(AutoRetrieveFields && Root.Items.Count == 0 && !DesignMode) RetrieveFields();																			   
		}
		protected virtual void OnDataMemberChanged() {
			try {
			DataControllerCore.SetListSource(BindingContext, DataSource, DataMember);
			} catch {
				DestroyDataController();
				ArrayList localControls = new ArrayList(Controls);
				foreach(Control control in localControls) if(!((ILayoutDesignerMethods)this).IsInternalControl(control)) control.DataBindings.Clear();
				dataSource = null;
				dataMember = string.Empty;
				SetupDataController();
				if(DesignMode)
					XtraMessageBox.Show("The data bindings of inner controls have been removed. The DataLayoutControl's DataSource and DataMember properties have been reset.",
					DevExpress.XtraEditors.Controls.Localizer.Active.GetLocalizedString(DevExpress.XtraEditors.Controls.StringId.CaptionError),
					MessageBoxButtons.OK, MessageBoxIcon.Error); ; 
				return;
			}
			if(AutoRetrieveFields && Root.Items.Count == 0 && !DesignMode) RetrieveFields();
		}
		protected virtual ControlsManager CreateControlsManager() {
			return new ControlsManager();
		}
		public virtual void RetrieveFields() {
			Clear();
			LayoutElementsBindingInfo info = GetLayoutElementsBindingsInfo();
			LayoutCreator creator = CreateLayoutCreator();
			creator.CreateLayout(info);
			BestFit();
		}
		public virtual void RetrieveFields(RetrieveFieldsParameters parameters) {
			Clear();
			LayoutElementsBindingInfo info = GetLayoutElementsBindingsInfo();
			info.ColumnCount = parameters.ColumnCount;
			LayoutCreator creator = CreateLayoutCreator();
			creator.CreateLayout(info, info.ColumnCount > 1, parameters);
			BestFit();
		}
		public virtual LayoutElementsBindingInfo GetLayoutElementsBindingsInfo() {
			LayoutElementsBindingInfoHelper bindingHelper = new LayoutElementsBindingInfoHelper(this);
			LayoutElementsBindingInfo info = bindingHelper.CreateDataLayoutElementsBindingInfo();
			bindingHelper.FillWithSuggestedValues(info);
			return info;
		}
		public virtual LayoutCreator CreateLayoutCreator() {
			return new LayoutCreator(this);
		}
		public void AddToHiddenItems(BaseLayoutItem item) {
			if(!item.AllowHide || item.IsHidden) return;
			((ILayoutDesignerMethods)this).BeginChangeUpdate();
			item.Owner = this;
			if(item.Parent != null) item.Parent.Remove(item);
			item.Parent = null;
			HiddenItems.Add(item);
			(this as ILayoutControl).ShouldArrangeTextSize = true;
			((ILayoutDesignerMethods)this).EndChangeUpdate();
		}
		protected internal virtual RepositoryItem GetRepositoryItem(LayoutElementBindingInfo bi) {
			RepositoryItem displayRepositoryItem = defaultEditorsRepository.GetRepositoryItem(bi.DataInfo.Type, bi.AnnotationAttributes);
			RepositoryItem repositoryItemForEditing = defaultEditorsRepository.GetRepositoryItemForEditing(displayRepositoryItem, bi.DataInfo.Type, bi.AnnotationAttributes);
			if(bi.ColumnOptions.ReadOnly) repositoryItemForEditing.ReadOnly = true;
			if(bi.ColumnOptions.IsFarAlignedByDefault)
				repositoryItemForEditing.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
			return repositoryItemForEditing;
		}
		public event EventHandler<FieldRetrievedEventArgs> FieldRetrieved;
		internal void RaiseFieldRetrieved(Control createdControl, LayoutControlItem createdItem, string fieldName, RepositoryItem createdRespoitoryItem) {
			if(FieldRetrieved != null)
				FieldRetrieved(this, new FieldRetrievedEventArgs() { Control = createdControl, Item = createdItem, fieldNameCore = fieldName, RepositoryItem = createdRespoitoryItem });
		}
		public event EventHandler<FieldRetrievingEventArgs> FieldRetrieving;
		internal LayoutElementBindingInfo RaiseFieldRetrieving(LayoutElementBindingInfo bi) {
			if(FieldRetrieving == null) return bi;
			FieldRetrievingEventArgs ea = new FieldRetrievingEventArgs() { PropertyName = bi.BoundPropertyName, fieldNameCore = bi.DataMember, Visible = bi.Visible, EditorType = bi.EditorType, DataSourceUpdateMode = bi.DataSourceUpdateMode, DataSourceNullValue = bi.DataSourceNullValue, DataType = bi.DataInfo.GetDataType() };
			FieldRetrieving(this, ea);
			if(ea.Handled) {
				bi.BoundPropertyName = ea.PropertyName;
				bi.Visible = ea.Visible;
				bi.EditorType = ea.EditorType;
				bi.DataSourceUpdateMode = ea.DataSourceUpdateMode;
				bi.DataSourceNullValue = ea.DataSourceNullValue;
			}
			return bi;
		}
	}
	public class RetrieveFieldsParameters {
		public RetrieveFieldsParameters() { 
			ColumnCount = 1;
			CustomListParameters = new List<RetrieveFieldParameters>();
		}
		public int ColumnCount { get; set; }
		public DataSourceUpdateMode DataSourceUpdateMode { get; set; }
		public object DataSourceNullValue { get; set; }
		public List<RetrieveFieldParameters> CustomListParameters { get; set; }
	}
	public class RetrieveFieldParameters{
		public RetrieveFieldParameters() {
			GenerateField = true;
		}
		public Control ControlForField { get; set; }
		public string FieldName { get; set; }
		public bool GenerateField { get; set; }
		public bool CreateTabGroupForItem { get; set; }
	}
	public class FieldRetrievingEventArgs :EventArgs {
		public FieldRetrievingEventArgs() { Handled = false; }
		public bool Handled { get; set; }
		internal string fieldNameCore = String.Empty;
		public string FieldName { get { return fieldNameCore; } }
		public bool Visible { get; set; }
		public string PropertyName { get; set; }
		public DataSourceUpdateMode DataSourceUpdateMode { get; set; }
		public Type EditorType { get; set; }
		public object DataSourceNullValue { get; set; }
		public Type DataType { get; internal set; }
	}
	public class FieldRetrievedEventArgs :EventArgs {
		public LayoutControlItem Item { get; internal set; }
		public Control Control { get; internal set; }
		public RepositoryItem RepositoryItem { get; internal set; }
		internal string fieldNameCore = String.Empty;
		public string FieldName { get { return fieldNameCore; } }
	}
}
