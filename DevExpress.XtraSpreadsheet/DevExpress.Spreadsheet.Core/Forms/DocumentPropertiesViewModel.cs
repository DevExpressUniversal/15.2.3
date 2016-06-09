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
using System.Collections.ObjectModel;
using System.IO;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region DocumentPropertiesViewModel
	public class DocumentPropertiesViewModel : ViewModelBase {
		#region Fields
		readonly ISpreadsheetControl control;
		string application = String.Empty;
		string manager = String.Empty;
		string company = String.Empty;
		string version = String.Empty;
		string title = String.Empty;
		string subject = String.Empty;
		string creator = String.Empty;
		string keywords = String.Empty;
		string description = String.Empty;
		string lastModifiedBy = String.Empty;
		string category = String.Empty;
		string created = String.Empty;
		string modified = String.Empty;
		string printed = String.Empty;
		string fileCreated = String.Empty;
		string fileAccessed = String.Empty;
		string fileModified = String.Empty;
		string fileSize = String.Empty;
		string fileName = String.Empty;
		string filePath = String.Empty;
		string fileShortName = String.Empty;
		bool isFileAttributeReadonly;
		bool isFileAttributeArchive;
		bool isFileAttributeHidden;
		bool isFileAttributeSystem;
		int propertyIndex = -1;
		string propertyName;
		string propertyType;
		string propertyValue;
		bool propertyYesNoValue = true;
		readonly ObservableCollection<CustomDocumentPropertyViewModel> customProperties = new ObservableCollection<CustomDocumentPropertyViewModel>();
		readonly List<CustomDocumentPropertyViewModel> predefinedCustomProperties = new List<CustomDocumentPropertyViewModel>();
		readonly Dictionary<string, CustomDocumentPropertyType> propertyTypeValues = new Dictionary<string,CustomDocumentPropertyType>();
		#endregion
		public DocumentPropertiesViewModel(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			PopulatePropertyTypeValues();
			PropertyType = CustomDocumentPropertyTypeToString(CustomDocumentPropertyType.Text);
			PopulatePredefinedCustomProperties();
			PopulateCustomProperties();
			UpdateFileInfo();
		}
		private void PopulatePropertyTypeValues() {
			propertyTypeValues.Clear();
			propertyTypeValues.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_CustomPropertyTypeText), CustomDocumentPropertyType.Text);
			propertyTypeValues.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_CustomPropertyTypeDateTime), CustomDocumentPropertyType.DateTime);
			propertyTypeValues.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_CustomPropertyTypeNumber), CustomDocumentPropertyType.Number);
			propertyTypeValues.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_CustomPropertyTypeBoolean), CustomDocumentPropertyType.Boolean);
		}
		#region Properties
		public ISpreadsheetControl Control { get { return control; } }
		protected internal DocumentModel DocumentModel { get { return control.InnerControl.DocumentModel; } }
		public string Application {
			get { return application; }
			set {
				if (Application == value)
					return;
				this.application = value;
				OnPropertyChanged("Application");
			}
		}
		public string Manager {
			get { return manager; }
			set {
				if (Manager == value)
					return;
				this.manager = value;
				OnPropertyChanged("Manager");
			}
		}
		public string Company {
			get { return company; }
			set {
				if (Company == value)
					return;
				this.company = value;
				OnPropertyChanged("Company");
			}
		}
		public string Version {
			get { return version; }
			set {
				if (Version == value)
					return;
				this.version = value;
				OnPropertyChanged("Version");
			}
		}
		public string Title {
			get { return title; }
			set {
				if (Title == value)
					return;
				this.title = value;
				OnPropertyChanged("Title");
			}
		}
		public string Subject {
			get { return subject; }
			set {
				if (Subject == value)
					return;
				this.subject = value;
				OnPropertyChanged("Subject");
			}
		}
		public string Creator {
			get { return creator; }
			set {
				if (Creator == value)
					return;
				this.creator = value;
				OnPropertyChanged("Creator");
			}
		}
		public string Keywords {
			get { return keywords; }
			set {
				if (Keywords == value)
					return;
				this.keywords = value;
				OnPropertyChanged("Keywords");
			}
		}
		public string Description {
			get { return description; }
			set {
				if (Description == value)
					return;
				this.description = value;
				OnPropertyChanged("Description");
			}
		}
		public string LastModifiedBy {
			get { return lastModifiedBy; }
			set {
				if (LastModifiedBy == value)
					return;
				this.lastModifiedBy = value;
				OnPropertyChanged("LastModifiedBy");
			}
		}
		public string Category {
			get { return category; }
			set {
				if (Category == value)
					return;
				this.category = value;
				OnPropertyChanged("Category");
			}
		}
		public string Created {
			get { return created; }
			set {
				if (Created == value)
					return;
				this.created = value;
				OnPropertyChanged("Created");
			}
		}
		public string Modified {
			get { return modified; }
			set {
				if (Modified == value)
					return;
				this.modified = value;
				OnPropertyChanged("Modified");
			}
		}
		public string Printed {
			get { return printed; }
			set {
				if (Printed == value)
					return;
				this.printed = value;
				OnPropertyChanged("Printed");
			}
		}
		public string FileCreated { get { return fileCreated; } }
		public string FileAccessed { get { return fileAccessed; } }
		public string FileModified { get { return fileModified; } }
		public string FileSize { get { return fileSize; } }
		public string FileName { get { return fileName; } }
		public string FilePath { get { return filePath; } }
		public string FileShortName { get { return fileShortName; } }
		public bool IsFileAttributeReadonly { get { return isFileAttributeReadonly; } }
		public bool IsFileAttributeArchive { get { return isFileAttributeArchive; } }
		public bool IsFileAttributeHidden { get { return isFileAttributeHidden; } }
		public bool IsFileAttributeSystem { get { return isFileAttributeSystem; } }
		public int PropertyIndex {
			get { return propertyIndex; }
			set {
				if (PropertyIndex == value)
					return;
				this.propertyIndex = value;
				OnPropertyChanged("PropertyIndex");
				if (IsPropertyIndexValid) {
					CustomDocumentPropertyViewModel property = CustomPropertiesDataSource[PropertyIndex];
					PropertyName = property.Name;
					PropertyType = CustomDocumentPropertyTypeToString(property.Type);
					PropertyValue = property.Text;
				}
				else
					ResetCurrentPropertyValues();
			}
		}
		public bool IsPropertyIndexValid { get { return PropertyIndex >= 0 && PropertyIndex < CustomPropertiesDataSource.Count; } }
		public string PropertyName {
			get { return propertyName; }
			set {
				if (PropertyName == value)
					return;
				this.propertyName = value;
				OnPropertyChanged("PropertyName");
				NotifyButtonStateChange();
			}
		}
		public string PropertyType {
			get { return propertyType; }
			set {
				if (PropertyType == value)
					return;
				this.propertyType = value;
				OnPropertyChanged("PropertyType");
				NotifyButtonStateChange();
				OnPropertyChanged("ShowYesNoSelector");
				OnPropertyChanged("ShowValueEditor");
			}
		}
		public string PropertyValue {
			get { return propertyValue; }
			set {
				if (PropertyValue == value)
					return;
				this.propertyValue = value;
				this.propertyYesNoValue = (value != XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_CustomPropertyNo));
				OnPropertyChanged("PropertyValue");
				OnPropertyChanged("PropertyYesNoValue");
				OnPropertyChanged("PropertyYesNoIndex");
				NotifyButtonStateChange();
			}
		}
		public bool PropertyYesNoValue {
			get { return propertyYesNoValue; }
			set {
				if (PropertyYesNoValue == value)
					return;
				this.propertyYesNoValue = value;
				if (value)
					this.propertyValue = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_CustomPropertyYes);
				else
					this.propertyValue = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_CustomPropertyNo);
				OnPropertyChanged("PropertyValue");
				OnPropertyChanged("PropertyYesNoValue");
				OnPropertyChanged("PropertyYesNoIndex");
				NotifyButtonStateChange();
			}
		}
		public int PropertyYesNoIndex {
			get { return PropertyYesNoValue ? 0 : 1; }
			set { PropertyYesNoValue = (value == 0); }
		}
		protected void NotifyButtonStateChange() {
			OnPropertyChanged("CanDeleteProperty");
			OnPropertyChanged("CanAddProperty");
			OnPropertyChanged("CanModifyProperty");
			OnPropertyChanged("ShowAddPropertyButton");
			OnPropertyChanged("ShowModifyPropertyButton");
		}
		public ObservableCollection<CustomDocumentPropertyViewModel> CustomPropertiesDataSource { get { return customProperties; } }
		public List<CustomDocumentPropertyViewModel> PredefinedCustomPropertiesDataSource { get { return predefinedCustomProperties; } }
		public IEnumerable<string> PropertyTypesDataSource { get { return propertyTypeValues.Keys; } }
		public bool CanDeleteProperty { get { return IsExistingProperty(PropertyName); } }
		public bool CanAddProperty { get { return !String.IsNullOrEmpty(PropertyName) && (!String.IsNullOrEmpty(PropertyValue) || ShowYesNoSelector) && !IsExistingProperty(PropertyName); } }
		public bool CanModifyProperty {
			get {
				if (!IsExistingProperty(PropertyName) || !IsPropertyIndexValid)
					return false;
				CustomDocumentPropertyViewModel property = CustomPropertiesDataSource[PropertyIndex];
				return PropertyName != property.Name ||
					PropertyType != CustomDocumentPropertyTypeToString(property.Type) ||
					PropertyValue != property.Text;
			}
		}
		public bool ShowAddPropertyButton { get { return String.IsNullOrEmpty(PropertyName) || String.IsNullOrEmpty(PropertyValue) || !IsExistingProperty(PropertyName); } }
		public bool ShowModifyPropertyButton { get { return !ShowAddPropertyButton; } }
		public bool ShowYesNoSelector { get { return StringToCustomDocumentPropertyType(PropertyType) == CustomDocumentPropertyType.Boolean; } }
		public bool ShowValueEditor { get { return !ShowYesNoSelector; } }
		#endregion
		void UpdateFileInfo() {
			string currentFileName = DocumentModel.DocumentSaveOptions.CurrentFileName;
			if (!File.Exists(currentFileName))
				return;
			this.fileName = Path.GetFileName(currentFileName);
			try {
				this.filePath = Path.GetDirectoryName(Path.GetFullPath(currentFileName));
			}
			catch {
				this.filePath = string.Empty;
			}
			try {
				FileInfo info = new FileInfo(currentFileName);
				this.fileCreated = ShowDocumentPropertiesCommand.DateTimeToString(info.CreationTime);
				this.fileAccessed = ShowDocumentPropertiesCommand.DateTimeToString(info.LastAccessTime);
				this.fileModified = ShowDocumentPropertiesCommand.DateTimeToString(info.LastWriteTime);
				this.fileSize = FileSizeToString(info.Length);
				this.isFileAttributeReadonly = (info.Attributes & FileAttributes.ReadOnly) != 0;
				this.isFileAttributeArchive = (info.Attributes & FileAttributes.Archive) != 0;
				this.isFileAttributeHidden = (info.Attributes & FileAttributes.Hidden) != 0;
				this.isFileAttributeSystem = (info.Attributes & FileAttributes.System) != 0;
#if !SL && !DXPORTABLE
				this.fileShortName = Path.GetFileName(DevExpress.Office.PInvoke.Win32.GetShortPathName(currentFileName));
#endif
			}
			catch {
			}
		}
		string FileSizeToString(long value) {
			return FileSizeToStringOptimal(value) + String.Format(" ({0:#,##0} {1})", value, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_Bytes));
		}
		string FileSizeToStringOptimal(long value) {
			string[] sizes = { "b", "kB", "MB", "GB", "TB", "PB", "EB" };
			int order = 0;
			double length = value;
			while (length >= 1024 && order + 1 < sizes.Length) {
				order++;
				length /= 1024.0;
			}
			return String.Format("{0:0.##} {1}", length, sizes[order]);
		}
		protected void PopulateCustomProperties() {
			customProperties.Clear();
			ModelDocumentCustomProperties properties = DocumentModel.DocumentCustomProperties;
			foreach (string name in properties.Names)
				customProperties.Add(CreateCustomPropertyViewModel(name, properties[name]));
		}
		protected void PopulatePredefinedCustomProperties() {
			predefinedCustomProperties.Clear();
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyCheckedBy, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyClient, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyDateCompleted, CustomDocumentPropertyType.DateTime);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyDepartment, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyDestination, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyDisposition, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyDivision, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyDocumentNumber, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyEditor, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyForwardTo, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyGroup, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyLanguage, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyMailstop, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyMatter, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyOffice, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyOwner, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyProject, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyPublisher, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyPurpose, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyReceivedFrom, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyRecordedBy, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyRecordedDate, CustomDocumentPropertyType.DateTime);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyReference, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertySource, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyStatus, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyTelephoneNumber, CustomDocumentPropertyType.Text);
			AddPredefinedCustomProperty(XtraSpreadsheetStringId.Caption_CustomPropertyTypist, CustomDocumentPropertyType.Text);
			predefinedCustomProperties.Sort(new CustomDocumentPropertyViewModelNameComparer());
		}
		protected CustomDocumentPropertyViewModel CreateCustomPropertyViewModel(string name, Spreadsheet.CellValue value) {
			CustomDocumentPropertyViewModel result = new CustomDocumentPropertyViewModel();
			result.Name = name;
			result.Value = value.Clone();
			result.Type = ObtainValueType(value);
			result.TypeText = CustomDocumentPropertyTypeToString(result.Type);
			result.UpdateText(DocumentModel.Culture);
			return result;
		}
		protected void AddPredefinedCustomProperty(XtraSpreadsheetStringId nameId, CustomDocumentPropertyType type) {
			CustomDocumentPropertyViewModel property = CreateCustomPropertyViewModel(XtraSpreadsheetLocalizer.GetString(nameId), type);
			predefinedCustomProperties.Add(property);
		}
		protected CustomDocumentPropertyViewModel CreateCustomPropertyViewModel(string name, CustomDocumentPropertyType type) {
			CustomDocumentPropertyViewModel result = new CustomDocumentPropertyViewModel();
			result.Name = name;
			result.Type = type;
			result.TypeText = CustomDocumentPropertyTypeToString(type);
			result.UpdateText(DocumentModel.Culture);
			return result;
		}
		protected CustomDocumentPropertyType ObtainValueType(Spreadsheet.CellValue value) {
			switch (value.Type) {
				case Spreadsheet.CellValueType.DateTime:
					return CustomDocumentPropertyType.DateTime;
				case Spreadsheet.CellValueType.Numeric:
					return CustomDocumentPropertyType.Number;
				case Spreadsheet.CellValueType.Boolean:
					return CustomDocumentPropertyType.Boolean;
				case Spreadsheet.CellValueType.Text:
				default:
					return CustomDocumentPropertyType.Text;
			}
		}
		protected int CalculatePropertyIndex(string name) {
			int count = CustomPropertiesDataSource.Count;
			for (int i = 0; i < count; i++)
				if (CustomPropertiesDataSource[i].Name == name)
					return i;
			return -1;
		}
		protected bool IsExistingProperty(string name) {
			return CalculatePropertyIndex(name) >= 0;
		}
		public void DeleteProperty() {
			int index = CalculatePropertyIndex(PropertyName);
			if (index >= 0) {
				CustomPropertiesDataSource.RemoveAt(index);
				PropertyIndex = -1;
				ResetCurrentPropertyValues();
				OnPropertyChanged("CustomPropertiesDataSource");
			}
		}
		public void AddProperty() {
			CustomDocumentPropertyViewModel property = CreateProperty();
			if (property != null) {
				CustomPropertiesDataSource.Add(property);
				PropertyIndex = -1;
				ResetCurrentPropertyValues();
				OnPropertyChanged("CustomPropertiesDataSource");
			}
		}
		public void ModifyProperty() {
			int index = CalculatePropertyIndex(PropertyName);
			if (index >= 0) {
				CustomDocumentPropertyViewModel property = CreateProperty();
				if (property != null) {
					CustomPropertiesDataSource[index] = property;
					PropertyIndex = -1;
					ResetCurrentPropertyValues();
					OnPropertyChanged("CustomPropertiesDataSource");
				}
			}
		}
		protected void ResetCurrentPropertyValues() {
			PropertyName = String.Empty;
			PropertyType = CustomDocumentPropertyTypeToString(CustomDocumentPropertyType.Text);
			PropertyValue = String.Empty;
			PropertyYesNoValue = true;
		}
		protected CustomDocumentPropertyViewModel CreateProperty() {
			CustomDocumentPropertyViewModel result = new CustomDocumentPropertyViewModel();
			result.Name = PropertyName;
			result.Type = StringToCustomDocumentPropertyType(PropertyType);
			result.TypeText = PropertyType;
			if (result.Type == CustomDocumentPropertyType.Boolean)
				result.Value = PropertyYesNoValue;
			else {
				if (string.IsNullOrEmpty(PropertyValue))
					return null;
				Spreadsheet.CellValue value = ParsePropertyValue(PropertyValue, StringToCustomDocumentPropertyType(PropertyType));
				if (value == null) {
					value = PropertyValue;
					result.Type = CustomDocumentPropertyType.Text;
					result.TypeText = CustomDocumentPropertyTypeToString(result.Type);
					Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorInvalidCustomPropertyValue));
				}
				result.Value = value;
			}
			result.UpdateText(DocumentModel.Culture);
			return result;
		}
		public CustomDocumentPropertyType StringToCustomDocumentPropertyType(string value) {
			CustomDocumentPropertyType result;
			if (!propertyTypeValues.TryGetValue(value, out result))
				return CustomDocumentPropertyType.Text;
			else
				return result;
		}
		public string CustomDocumentPropertyTypeToString(CustomDocumentPropertyType value) {
			foreach (string key in propertyTypeValues.Keys)
				if (propertyTypeValues[key] == value)
					return key;
			return String.Empty;
		}
		protected Spreadsheet.CellValue ParsePropertyValue(string text, CustomDocumentPropertyType type) {
			if (type == CustomDocumentPropertyType.Text)
				return text;
			VariantValue formattedValue = DocumentModel.DataContext.ConvertTextToVariantValueWithCaching(text);
			Spreadsheet.CellValue value = new Spreadsheet.CellValue(formattedValue, type == CustomDocumentPropertyType.DateTime);
			if (type != ObtainValueType(value))
				return null;
			else
				return value;
		}
		public void ApplyPredefinedProperty(int predefiendPropertyIndex) {
			if (predefiendPropertyIndex < 0 || predefiendPropertyIndex >= predefinedCustomProperties.Count) {
				ResetCurrentPropertyValues();
				return;
			}
			CustomDocumentPropertyViewModel property = predefinedCustomProperties[predefiendPropertyIndex];
			PropertyIndex = CalculatePropertyIndex(property.Name);
			PropertyName = property.Name;
			PropertyType = CustomDocumentPropertyTypeToString(property.Type);
			PropertyValue = String.Empty;
		}
		public bool Validate() {
			ShowDocumentPropertiesCommand command = new ShowDocumentPropertiesCommand(Control);
			return command.Validate(this);
		}
		public void ApplyChanges() {
			if (CanAddProperty)
				AddProperty();
			else if (CanModifyProperty)
				ModifyProperty();
			ShowDocumentPropertiesCommand command = new ShowDocumentPropertiesCommand(Control);
			command.ApplyChanges(this);
		}
	}
	#endregion
	#region CustomDocumentPropertyType
	public enum CustomDocumentPropertyType {
		Text,
		Number,
		DateTime,
		Boolean,
	}
	#endregion
	#region CustomDocumentPropertyViewModel
	public class CustomDocumentPropertyViewModel : ViewModelBase {
		#region Fields
		CustomDocumentPropertyType type;
		string name;
		string text;
		string typeText;
		DevExpress.Spreadsheet.CellValue value;
		#endregion
		#region Properties
		public CustomDocumentPropertyType Type {
			get { return type; }
			set {
				if (Type == value)
					return;
				this.type = value;
				OnPropertyChanged("Type");
			}
		}
		public string Name {
			get { return name; }
			set {
				if (Name == value)
					return;
				this.name = value;
				OnPropertyChanged("Name");
			}
		}
		public string Text {
			get { return text; }
			set {
				if (Text == value)
					return;
				this.text = value;
				OnPropertyChanged("Text");
			}
		}
		public string TypeText {
			get { return typeText; }
			set {
				if (TypeText == value)
					return;
				this.typeText = value;
				OnPropertyChanged("TypeText");
			}
		}
		public DevExpress.Spreadsheet.CellValue Value {
			get { return value; }
			set {
				if (Value == value)
					return;
				this.value = value;
				OnPropertyChanged("Value");
			}
		}
		#endregion
		public void UpdateText(CultureInfo provider) {
			if (Value == null) {
				Text = String.Empty;
				return;
			}
			if (Value.Type == Spreadsheet.CellValueType.DateTime) {
				DateTime dateTime = Value.DateTimeValue;
				if (dateTime.TimeOfDay == TimeSpan.Zero)
					Text = dateTime.ToString(provider.DateTimeFormat.ShortDatePattern);
				else if (dateTime.Ticks == dateTime.TimeOfDay.Ticks)
					Text = dateTime.ToString(provider.DateTimeFormat.ShortTimePattern);
				else
					Text = Value.ToString(provider);
			}
			else if (Value.Type == Spreadsheet.CellValueType.Boolean) {
				if (Value.BooleanValue)
					Text = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_CustomPropertyYes);
				else
					Text = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_CustomPropertyNo);
			}
			else
				Text = Value.ToString(provider);
		}
	}
	#endregion
	public class CustomDocumentPropertyViewModelNameComparer : IComparer<CustomDocumentPropertyViewModel> {
		public int Compare(CustomDocumentPropertyViewModel x, CustomDocumentPropertyViewModel y) {
			return Comparer<string>.Default.Compare(x.Name, y.Name);
		}
	}
}
