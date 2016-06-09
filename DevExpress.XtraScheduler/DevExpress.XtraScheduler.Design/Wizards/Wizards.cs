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
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Text;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Design.Wizards {
	#region SetupMappingsBaseWizard<T> (abstract class)
	public abstract class SetupMappingsBaseWizard<T> where T : IPersistentObject {
		#region Fields
		readonly IDesignerHost host;
		readonly IPersistentObjectStorage<T> objectStorage;
		readonly IDataFieldsProvider dataFieldsProvider;
		#endregion
		protected SetupMappingsBaseWizard(IDesignerHost host, IPersistentObjectStorage<T> objectStorage, IDataFieldsProvider dataFieldsProvider) {
			this.host = host;
			this.objectStorage = objectStorage;
			this.dataFieldsProvider = dataFieldsProvider;
		}
		#region Properties
		public IDesignerHost Host { get { return host; } }
		public IPersistentObjectStorage<T> ObjectStorage { get { return objectStorage; } }
		protected IDataFieldsProvider DataFieldsProvider { get { return dataFieldsProvider; } }
		protected bool UnboundMode { get { return dataFieldsProvider.UnboundMode; } }
		#endregion
		protected internal virtual DataFieldInfoCollection GetDataFields() {
			if (dataFieldsProvider.UnboundMode)
				return new DataFieldInfoCollection();
			else
				return dataFieldsProvider.GetDataFields();
		}
	}
	#endregion
	#region SetupMappingsWizard<T>
	public class SetupMappingsWizard<T> : SetupMappingsBaseWizard<T> where T : IPersistentObject {
		IMappingWizardExtensionLogic wizardExtensionLogic;
		public SetupMappingsWizard(IDesignerHost host, IPersistentObjectStorage<T> objectStorage, IDataFieldsProvider dataFieldsProvider)
			: base(host, objectStorage, dataFieldsProvider) {
		}
		public virtual void GenerateMappingsInitially() {
			MappingCollection mappings = new MappingCollection();
			ObjectStorage.Mappings.AddMappings(mappings, true);
			if (mappings.Count <= 0)
				GenerateMappings();
		}
		public virtual void GenerateMappings() {
			DataFieldInfoCollection dataFields = GetDataFields();
			MappingsAutoPicker<T> picker = new MappingsAutoPicker<T>(ObjectStorage.Mappings, dataFields, ObjectStorage.Mappings.GetMappingsTokenInfos());
			if (this.wizardExtensionLogic != null)
				picker.SetupMappingsToIgnore(this.wizardExtensionLogic.GetMappingsToIgnore());
			picker.Execute(true);
		}
		public virtual List<string> GetRequiredMappingNames() {
			return new List<string>(ObjectStorage.Mappings.GetRequiredMappingNames());
		}
		public virtual void ClearMappings() {
			MappingInfoBase<T> mappingInfo = ObjectStorage.Mappings;
			MappingCollection mappings = new MappingCollection();
			mappingInfo.AddMappings(mappings, true);
			int count = mappings.Count;
			for (int i = 0; i < count; i++)
				mappingInfo.SetMappingMember(mappings[i].Name, String.Empty);
		}
		public virtual bool ValidateMappings(StringBuilder errorMessage) {
			MappingCollection mappings = new MappingCollection();
			if (UnboundMode)
				((IInternalPersistentObjectStorage<T>)ObjectStorage).AppendDefaultMappings(mappings);
			else
				((IInternalPersistentObjectStorage<T>)ObjectStorage).AppendMappings(mappings);
			return ValidateMappingsCore(errorMessage, mappings);
		}
		protected internal virtual bool ValidateMappingsCore(StringBuilder errorMessage, MappingCollection mappings) {
			StringCollection errors = new StringCollection();
			DataFieldInfoCollection fields = GetDataFields();
			((IInternalPersistentObjectStorage<T>)ObjectStorage).ValidateDataSourceCore(errors, fields, mappings);
			if (errors.Count <= 0)
				return true;
			else {
				errorMessage.Append(CreateInvalidMappingsErrorMessage(errors));
				return false;
			}
		}
		protected internal virtual string CreateInvalidMappingsErrorMessage(StringCollection errors) {
			StringBuilder sb = new StringBuilder();
			sb.Append(SchedulerLocalizer.GetString(SchedulerStringId.Msg_IncorrectMappingsQuestion));
			int count = errors.Count;
			for (int i = 0; i < count; i++)
				sb.AppendFormat("{0}\r\n", errors[i]);
			return sb.ToString();
		}
		public void InitializeWizardExtension(IMappingWizardExtensionView wiazardExtensionView, Action refreshHandler) {
			if (this.wizardExtensionLogic == null)
				return;
			this.wizardExtensionLogic.AttachView(wiazardExtensionView);
			this.wizardExtensionLogic.RefreshHandler = refreshHandler;
		}
		public void SetupExtension(IMappingWizardExtensionLogic wizardExtensionLogic) {
			this.wizardExtensionLogic = wizardExtensionLogic;
		}
		internal bool CheckAdditionalRestriction(string mappingName) {
			if (this.wizardExtensionLogic == null)
				return false;
			return this.wizardExtensionLogic.CheckRestriction(mappingName);
		}
		internal void ClearNotAllowedMappings() {
			if (this.wizardExtensionLogic == null)
				return;
			MappingInfoBase<T> mappingInfo = ObjectStorage.Mappings;
			MappingCollection mappings = new MappingCollection();
			mappingInfo.AddMappings(mappings, true);
			List<string> mappingNames = this.wizardExtensionLogic.GetMappingsToIgnore();
			int count = mappings.Count;
			for (int i = 0; i < count; i++) {
				string mappingName = mappings[i].Name;
				if (mappingNames.Contains(mappingName))
					mappingInfo.SetMappingMember(mappingName, String.Empty);
			}
		}
	}
	#endregion
	#region SetupCustomFieldMappingsWizard<T> (abstract class)
	public abstract class SetupCustomFieldMappingsWizard<T> : SetupMappingsBaseWizard<T> where T : IPersistentObject {
		readonly DataFieldInfoCollection availableFields;
		readonly DataFieldInfoCollection usedFields;
		readonly CustomFieldMappingCollectionBase<T> customFieldMappings;
		bool autoCorrectCustomFieldName;
		protected SetupCustomFieldMappingsWizard(IDesignerHost host, IPersistentObjectStorage<T> objectStorage, IDataFieldsProvider dataFieldsProvider)
			: base(host, objectStorage, dataFieldsProvider) {
			this.availableFields = new DataFieldInfoCollection();
			this.usedFields = new DataFieldInfoCollection();
			this.customFieldMappings = new CustomFieldMappingCollectionBase<T>();
			this.autoCorrectCustomFieldName = true;
		}
		public DataFieldInfoCollection AvailableFields { get { return availableFields; } }
		public CustomFieldMappingCollectionBase<T> CustomFields { get { return customFieldMappings; } }
		public bool AutoCorrectCustomFieldName {
			get { return autoCorrectCustomFieldName; }
			set {
				if (autoCorrectCustomFieldName == value)
					return;
				autoCorrectCustomFieldName = value;
			}
		}
		protected internal virtual void Initialize() {
			this.availableFields.Clear();
			this.usedFields.Clear();
			this.customFieldMappings.Clear();
			DataFieldInfoCollection availableFieldsCollection = GetDataFields();
			DataFieldInfoCollection userFieldsCollection = new DataFieldInfoCollection();
			userFieldsCollection.AddRange(availableFieldsCollection);
			MappingCollection mappings = new MappingCollection();
			ObjectStorage.Mappings.AddMappings(mappings, true);
			int count = mappings.Count;
			for (int i = 0; i < count; i++)
				RemoveFieldByName(userFieldsCollection, mappings[i].Member);
			mappings.AddRange(ObjectStorage.CustomFieldMappings);
			count = mappings.Count;
			for (int i = 0; i < count; i++)
				RemoveFieldByName(availableFieldsCollection, mappings[i].Member);
			availableFields.AddRange(availableFieldsCollection);
			usedFields.AddRange(userFieldsCollection);
			customFieldMappings.AddRange(ObjectStorage.CustomFieldMappings);
		}
		protected internal virtual void RemoveFieldByName(DataFieldInfoCollection fields, string name) {
			DataFieldInfo field = LookupFieldByName(fields, name);
			if (field != null)
				fields.Remove(field);
		}
		protected internal virtual DataFieldInfo LookupFieldByName(DataFieldInfoCollection fields, string name) {
			int count = fields.Count;
			for (int i = 0; i < count; i++)
				if (fields[i].Name == name)
					return fields[i];
			return null;
		}
		public void ConvertFieldInfoIntoCustomFieldMapping(DataFieldInfo fieldInfo) {
			CustomFieldMappingBase<T> mapping = CreateCustomFieldMapping();
			mapping.Member = fieldInfo.Name;
			mapping.Name = MakeCustomFieldName(fieldInfo.Name, customFieldMappings);
			availableFields.Remove(fieldInfo);
			usedFields.Add(fieldInfo);
			customFieldMappings.Add(mapping);
			ObjectStorage.CustomFieldMappings.Add(mapping);
		}
		protected internal abstract CustomFieldMappingBase<T> CreateCustomFieldMapping();
		protected internal virtual string MakeCustomFieldName(string member, CustomFieldMappingCollectionBase<T> customFields) {
			string initialName = MakeCustomFieldName(member);
			string name = initialName;
			for (int i = 2; ; i++) {
				if (customFields[name] == null)
					return name;
				name = String.Format("{0}{1}", name, i);
			}
		}
		protected internal virtual string MakeCustomFieldName(string member) {
			if (!AutoCorrectCustomFieldName)
				return member;
			StringTokenizer tokenizer = new StringTokenizer();
			StringCollection tokens = tokenizer.Tokenize(member);
			StringBuilder result = new StringBuilder();
			int count = tokens.Count;
			for (int i = 0; i < count; i++)
				result.Append(Capitalize(tokens[i]));
			return result.ToString();
		}
		protected internal virtual string Capitalize(string str) {
			if (String.IsNullOrEmpty(str))
				return str;
			char first = Char.ToUpper(str[0]);
			return first + str.Substring(1);
		}
		public void RemoveCustomFieldMapping(CustomFieldMappingBase<T> mapping) {
			DataFieldInfo field = LookupFieldByName(usedFields, mapping.Member);
			if (field != null) {
				usedFields.Remove(field);
				availableFields.Add(field);
			}
			customFieldMappings.Remove(mapping);
			ObjectStorage.CustomFieldMappings.Remove(mapping);
		}
		public bool ValidateMappings(StringBuilder errors) {
			StringBuilder sb = new StringBuilder();
			StringCollection duplicatedMappingNames = new StringCollection();
			int count = customFieldMappings.Count;
			for (int i = 0; i < count; i++) {
				CustomFieldMappingBase<T> mapping = customFieldMappings[i];
				if (!duplicatedMappingNames.Contains(mapping.Name) && !CheckCustomFieldMappingNameUnique(customFieldMappings, mapping)) {
					sb.AppendFormat("{0}\r\n", mapping.Name);
					duplicatedMappingNames.Add(mapping.Name);
				}
			}
			string errorDescription = sb.ToString();
			if (!String.IsNullOrEmpty(errorDescription)) {
				errors.Append(String.Format(SchedulerLocalizer.GetString(SchedulerStringId.Msg_DuplicateCustomFieldMappings), errorDescription));
				return false;
			} else
				return true;
		}
		protected internal virtual bool CheckCustomFieldMappingNameUnique(CustomFieldMappingCollectionBase<T> customFieldMappings, CustomFieldMappingBase<T> customFieldMapping) {
			int count = customFieldMappings.Count;
			for (int i = 0; i < count; i++) {
				CustomFieldMappingBase<T> mapping = customFieldMappings[i];
				if (mapping.Name == customFieldMapping.Name && !Object.ReferenceEquals(mapping, customFieldMapping))
					return false;
			}
			return true;
		}
		public void ApplyChanges() {
			ObjectStorage.Storage.BeginUpdate();
			try {
				ObjectStorage.CustomFieldMappings.BeginUpdate();
				try {
					ObjectStorage.CustomFieldMappings.Clear();
					ObjectStorage.CustomFieldMappings.AddRange(customFieldMappings);
				} finally {
					ObjectStorage.CustomFieldMappings.EndUpdate();
				}
			} finally {
				ObjectStorage.Storage.EndUpdate();
			}
		}
	}
	#endregion
	#region SetupAppointmentCustomFieldMappingsWizard
	public class SetupAppointmentCustomFieldMappingsWizard : SetupCustomFieldMappingsWizard<Appointment> {
		public SetupAppointmentCustomFieldMappingsWizard(IDesignerHost host, IPersistentObjectStorage<Appointment> objectStorage, IDataFieldsProvider dataFieldsProvider)
			: base(host, objectStorage, dataFieldsProvider) {
		}
		protected internal override CustomFieldMappingBase<Appointment> CreateCustomFieldMapping() {
			return new AppointmentCustomFieldMapping();
		}
	}
	#endregion
	#region SetupResourceCustomFieldMappingsWizard
	public class SetupResourceCustomFieldMappingsWizard : SetupCustomFieldMappingsWizard<Resource> {
		public SetupResourceCustomFieldMappingsWizard(IDesignerHost host, IPersistentObjectStorage<Resource> objectStorage, IDataFieldsProvider dataFieldsProvider)
			: base(host, objectStorage, dataFieldsProvider) {
		}
		protected internal override CustomFieldMappingBase<Resource> CreateCustomFieldMapping() {
			return new ResourceCustomFieldMapping();
		}
	}
	#endregion
	#region SetupAppointmentDependencyCustomFieldMappingsWizard
	public class SetupAppointmentDependencyCustomFieldMappingsWizard : SetupCustomFieldMappingsWizard<AppointmentDependency> {
		public SetupAppointmentDependencyCustomFieldMappingsWizard(IDesignerHost host, IPersistentObjectStorage<AppointmentDependency> objectStorage, IDataFieldsProvider dataFieldsProvider)
			: base(host, objectStorage, dataFieldsProvider) {
		}
		protected internal override CustomFieldMappingBase<AppointmentDependency> CreateCustomFieldMapping() {
			return new AppointmentDependencyCustomFieldMapping();
		}
	}
	#endregion
}
