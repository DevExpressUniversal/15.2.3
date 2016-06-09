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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data.Design;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPrinting.Native.Lines;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.XtraReports.Design {
	public partial class NewParameterEditorForm : ReportsEditorFormBase, INewParameterEditorView, IDataContainer {
		class EditItem<T> {
			public T Value { get; private set; }
			public string DisplayText { get; private set; }
			public EditItem(T value, string displayText) {
				if(value == null)
					throw new ArgumentNullException("value");
				if(string.IsNullOrEmpty(displayText))
					throw new ArgumentException("displayText");
				Value = value;
				DisplayText = displayText;
			}
			public override string ToString() {
				return DisplayText;
			}
			public override bool Equals(object obj) {
				return Equals(obj, Value);
			}
			public override int GetHashCode() {
				return Value.GetHashCode();
			}
		}
		public event EventHandler Submit;
		public event EventHandler StandardValuesSupportedChanged;
		public event EventHandler ShowAtParametersPanelChanged;
		public event EventHandler MultiValueChanged;
		public event EventHandler<ValidationEventArgs> ValidateName;
		public event EventHandler TypeChanged;
		public event EventHandler DataSourceChanged;
		public event EventHandler ValueMemberChanged;
		public event EventHandler DisplayMemberChanged;
		public event EventHandler FilterStringChanged;
		public event EventHandler LookUpValuesChanged;
		public event EventHandler ActiveTabChanged;
		BaseEdit defaultValueEditor;
		IDesignerHost designerHost;
		object dataSource;
		object dataAdapter;
		string dataMember = string.Empty;
		string valueMember;
		string displayMember;
		string filterString;
		public NewParameterEditorForm(IDesignerHost designerHost)
			: base(designerHost) {
			InitializeComponent();
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlConstants.DoubleBuffer, true);
			this.designerHost = designerHost;
			ceShowAtParametersPanel.CheckedChanged += ceShowAtParametersPanel_CheckedChanged;
			ceStandardValues.CheckedChanged += ceStandardValues_CheckedChanged;
			ceMultiValue.CheckedChanged += ceMultiValue_CheckedChanged;
			teName.Validating += teName_Validating;
			cbType.EditValueChanged += cbType_EditValueChanged;
			dataNavigator1.ButtonClick += lookUpValuesChanged;
			tabControl.SelectedPageChanged += tabControl_SelectedPageChanged;
			FillLinesContainer();
			defaultValueEditor = teDefaultValue;
			teDefaultValue.LocationChanged += teDefaultValue_LocationChanged;
		}
		void teDefaultValue_LocationChanged(object sender, EventArgs e) {
		}
		public NewParameterEditorForm() {
			InitializeComponent();
		}
		void FillLinesContainer() {
			string[] propertyNames = GetDynamicLookUpParameterPropertyNames();
			EditorPropertyLine[] lines = CreateEditorLines(propertyNames).ToArray();
			LinesContainer linesContainer = new LinesContainer();
			linesContainer.FillWithLines(lines, null, 331, 4, 11);
			linesContainer.Dock = DockStyle.Fill;
			foreach(Control control in linesContainer.Controls)
				control.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			linesContainer.Parent = pageDynamic;
		}
		protected virtual string[] GetDynamicLookUpParameterPropertyNames() {
			return new[] { "DataSource", "DataAdapter", "DataMember", "ValueMember", "DisplayMember", "FilterString" };
		}
		IEnumerable<EditorPropertyLine> CreateEditorLines(IEnumerable<string> propertyNames) {
			PropertyDescriptorCollection dynamicListLookUpSettingsProperties = TypeDescriptor.GetProperties(typeof(DynamicListLookUpSettings));
			PropertyDescriptorCollection formProperties = TypeDescriptor.GetProperties(this);
			List<EditorPropertyLine> editorLines = new List<EditorPropertyLine>();
			foreach(string propertyName in propertyNames) {
				PropertyDescriptor property = dynamicListLookUpSettingsProperties[propertyName];
				if(propertyName == null || property.Attributes.Contains(BrowsableAttribute.No))
					continue;
				string lineText = TryGetDisplayName(property, propertyName);
				EditorPropertyLine editorLine = CreateEditorLine(formProperties[propertyName], lineText);
				editorLines.Add(editorLine);
			}
			return editorLines;
		}
		EditorPropertyLine CreateEditorLine(PropertyDescriptor formProperty, string lineText) {
			IStringConverter stringConverter = new TypeStringConverter(formProperty.Converter, new RuntimeTypeDescriptorContext(formProperty, this));
			EditorPropertyLine editorLine = new EditorPropertyLine(designerHost, this, stringConverter, formProperty, this) { Name = lineText };
			editorLine.SetText(lineText);
			return editorLine;
		}
		static string TryGetDisplayName(PropertyDescriptor property, string defaultDisplayName) {
			DXDisplayNameAttribute displayNameAttribute = property.Attributes[typeof(DXDisplayNameAttribute)] as DXDisplayNameAttribute;
			return (displayNameAttribute != null) ? displayNameAttribute.DisplayName : defaultDisplayName;
		}
		string INewParameterEditorView.Name {
			get {
				return teName.Text;
			}
			set {
				teName.Text = value;
			}
		}
		string INewParameterEditorView.Description {
			get {
				return teDescription.Text;
			}
			set {
				teDescription.Text = value;
			}
		}
		object INewParameterEditorView.DefaultValue {
			get {
				return defaultValueEditor.EditValue;
			}
			set {
				defaultValueEditor.EditValue = value;
			}
		}
		Type INewParameterEditorView.Type {
			get {
				return cbType.EditValue != null ? ((EditItem<Type>)cbType.EditValue).Value : NewParameterEditorPresenter.DefaultParameterType;
			}
			set {
				if(value == null)
					return;
				foreach(EditItem<Type> item in cbType.Properties.Items) 
					if(item.Equals(value)) {
						cbType.EditValue = item;
						return;
					}
				cbType.EditValue = new EditItem<Type>(value, value.Name);
			}
		}
		bool INewParameterEditorView.ShowAtParametersPanel {
			get {
				return ceShowAtParametersPanel.Checked;
			}
			set {
				ceShowAtParametersPanel.Checked = value;
			}
		}
		bool INewParameterEditorView.StandardValuesSupported {
			get {
				return ceStandardValues.Checked;
			}
			set {
				ceStandardValues.Checked = value;
			}
		}
		bool INewParameterEditorView.MultiValue {
			get {
				return ceMultiValue.Checked;
			}
			set {
				ceMultiValue.Checked = value;
			}
		}
		LookUpSettingsTab INewParameterEditorView.LookUpSettingsActiveTab {
			get {
				return tabControl.SelectedTabPageIndex == 0 ? LookUpSettingsTab.DynamicList : LookUpSettingsTab.StaticList;
			}
			set {
				tabControl.SelectedTabPageIndex = value == LookUpSettingsTab.DynamicList ? 0 : 1;
			}
		}
		[Editor(typeof(DataSourceEditor), typeof(UITypeEditor)), TypeConverter(typeof(DataSourceConverter))]
		public virtual object DataSource {
			get {
				return dataSource;
			}
			set {
				dataSource = value;
				if(DataSourceChanged != null)
					DataSourceChanged(this, EventArgs.Empty);
			}
		}
		[Editor(typeof(DataAdapterEditor), typeof(UITypeEditor)), TypeConverter(typeof(DataAdapterConverter))]
		public virtual object DataAdapter {
			get {
				return dataAdapter;
			}
			set {
				dataAdapter = value;
			}
		}
		[Editor(typeof(DataContainerDataMemberEditor), typeof(UITypeEditor)), TypeConverter(typeof(DataMemberTypeConverter))]
		public string DataMember {
			get {
				return dataMember;
			}
			set {
				dataMember = value;
			}
		}
		[Editor(typeof(DataContainerFieldNameEditor), typeof(UITypeEditor)), TypeConverter(typeof(FieldNameConverter))]
		public string ValueMember {
			get {
				return valueMember;
			}
			set {
				valueMember = value;
				if(ValueMemberChanged != null)
					ValueMemberChanged(this, EventArgs.Empty);
			}
		}
		[Editor(typeof(DataContainerFieldNameEditor), typeof(UITypeEditor)), TypeConverter(typeof(FieldNameConverter))]
		public string DisplayMember {
			get {
				return displayMember;
			}
			set {
				displayMember = value;
				if(DisplayMemberChanged != null)
					DisplayMemberChanged(this, EventArgs.Empty);
			}
		}
		[Editor(typeof(LookUpSettingsFilterStringEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TextPropertyTypeConverter))]
		public string FilterString {
			get {
				return filterString;
			}
			set {
				filterString = value;
				if(FilterStringChanged != null)
					FilterStringChanged(this, EventArgs.Empty);
			}
		}
		IList INewParameterEditorView.LookUpValues {
			get {
				BindingSource bindingSource = (BindingSource)staticItemsTreeList.DataSource;
				return (IList)bindingSource.DataSource;
			}
			set {
				BindingSource bindingSource = new BindingSource();
				bindingSource.DataSource = value;
				staticItemsTreeList.DataSource = bindingSource;
				dataNavigator1.DataSource = bindingSource;
			}
		}
		void INewParameterEditorView.EnableLookUpSettings(bool enable, bool enableStandardValuesSupported) {
			panelLookUpSettings.Enabled = enable;
			ceStandardValues.Enabled = enableStandardValuesSupported;
		}
		void INewParameterEditorView.EnableSubmit(bool enable) {
			btOK.Enabled = enable;
		}
		void INewParameterEditorView.PopulateTypes(Dictionary<Type, string> availableTypes) {
			cbType.Properties.Items.Clear();
			foreach(var item in availableTypes)
				cbType.Properties.Items.Add(new EditItem<Type>(item.Key, item.Value));
			cbType.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
		}
		void INewParameterEditorView.SetEditType(Type type, bool multiValue) {
			BaseEdit editor = CreateEditorByType(type, multiValue);
			AddEditorToForm(editor);
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			if(DialogResult == DialogResult.OK) {
				if(Submit != null) {
					Submit(this, EventArgs.Empty);
				}
			}
		}
		void ceShowAtParametersPanel_CheckedChanged(object sender, EventArgs e) {
			if(ShowAtParametersPanelChanged != null) {
				ShowAtParametersPanelChanged(this, EventArgs.Empty);
			}
		}
		void ceStandardValues_CheckedChanged(object sender, EventArgs e) {
			if(StandardValuesSupportedChanged != null) {
				StandardValuesSupportedChanged(this, EventArgs.Empty);
			}
		}
		void ceMultiValue_CheckedChanged(object sender, EventArgs e) {
			if(MultiValueChanged != null) {
				MultiValueChanged(this, EventArgs.Empty);
			}
		}
		void teName_Validating(object sender, CancelEventArgs e) {
			ValidationEventArgs args = new ValidationEventArgs();
			if(ValidateName != null) {
				ValidateName(this, args);
			}
			e.Cancel = !args.IsValid;
		}
		void cbType_EditValueChanged(object sender, EventArgs e) {
			if(TypeChanged != null) {
				TypeChanged(this, EventArgs.Empty);
			}
		}
		void lookUpValuesChanged(object sender, EventArgs e) {
			if(LookUpValuesChanged != null)
				LookUpValuesChanged(this, EventArgs.Empty);
		}
		void tabControl_SelectedPageChanged(object sender, XtraTab.TabPageChangedEventArgs e) {
			if(ActiveTabChanged != null)
				ActiveTabChanged(this, EventArgs.Empty);
		}
		BaseEdit CreateEditorByType(Type type, bool multiValue) {
			IExtensionsProvider extensionProvider = (IExtensionsProvider)designerHost.RootComponent;			
			EditingContext editingContext = new EditingContext(extensionProvider.Extensions[DataEditorService.Guid], extensionProvider);
			var repositoryItem = DataEditorService.GetRepositoryItem(type, new Parameter(), editingContext) ?? 
				(multiValue ? DataEditorService.GetMultiValueRepositoryItem(type) : DataEditorService.GetRepositoryItem(type));
			var editor = repositoryItem.CreateEditor();
			editor.Properties.Assign(repositoryItem);
			return editor;
		}
		void AddEditorToForm(BaseEdit editor) {
			editor.Anchor = defaultValueEditor.Anchor;
			editor.Location = defaultValueEditor.Location;
			editor.Size = defaultValueEditor.Size;
			editor.TabIndex = defaultValueEditor.TabIndex;
			editor.Name = defaultValueEditor.Name;
			Controls.Remove(defaultValueEditor);
			defaultValueEditor = editor;
			editor.Parent = this;
		}
		#region IDataContainer Members
		public object GetEffectiveDataSource() {
			return DataSource;
		}
		public object GetSerializableDataSource() {
			return DataSource;
		}
		#endregion
		protected override void OnLayout(LayoutEventArgs levent) {
			base.OnLayout(levent);
			if(levent.AffectedProperty == "Visible") {
				DevExpress.Utils.LayoutHelper.DoLabelsEditorsLayout(
					new BaseControl[] { lbName, lbDescription, lbType, lbDefaultValue },
					new BaseControl[] { teName, teDescription, cbType, defaultValueEditor ?? teDefaultValue }
					);
				int w = Math.Max(Math.Max(ceShowAtParametersPanel.CalcBestSize().Width, ceStandardValues.CalcBestSize().Width), ceMultiValue.CalcBestSize().Width);
				if(teName.Left + w < teName.Right) {
					ceShowAtParametersPanel.Left = ceStandardValues.Left = ceMultiValue.Left = teName.Left;
					ceShowAtParametersPanel.Width = ceStandardValues.Width = ceMultiValue.Width = w; 
				}
			}
		}
	}
}
