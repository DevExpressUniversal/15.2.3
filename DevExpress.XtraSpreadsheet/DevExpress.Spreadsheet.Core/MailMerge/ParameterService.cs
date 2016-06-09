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
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using DevExpress.Data;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet {
	public class MailMergeParametersCollection : SimpleCollection<SpreadsheetParameter> {
		public void AddRange(IEnumerable<IParameter> parameters) {
			foreach(SpreadsheetParameter parameter in parameters) {
				this.Add(parameter);
			}
		}
		public SpreadsheetParameter FindByName(string parameterName) {
			foreach(SpreadsheetParameter parameter in this) {
				if(parameter.Name == parameterName)
					return parameter;
			}
			return null;
		}
		public override int Add(SpreadsheetParameter item) {
			int index = base.Add(item);
			RaiseParameterAdded(item);
			return index;
		}
		public override void RemoveAt(int index) {
			SpreadsheetParameter removed = InnerList[index];
			RaiseParameterRemoved(removed);
			base.RemoveAt(index);
		}
		public override void Clear() {
			RaiseCollectionCleared();
			base.Clear();
		}
		public override void Insert(int index, SpreadsheetParameter item) {
			base.Insert(index, item);
			RaiseParameterInserted(index, item);
		}
		public void Add(string name, object value) {
			SpreadsheetParameter parameter = new SpreadsheetParameter {Name = name, Type = value.GetType(), Value = value};
			this.Add(parameter);
		}
		public void Add(string name) {
			SpreadsheetParameter parameter = new SpreadsheetParameter {Name = name};
			this.Add(parameter);
		}
		public void Add(string name, Type type, object value) {
			SpreadsheetParameter parameter = new SpreadsheetParameter {Name = name, Type = type, Value = value};
			this.Add(parameter);
		}
		public IParameterService GetParameterService() {
			IParameterService result = new ParameterService(this);
			return result;
		}
		public void UpdateFromParameterService(IParameterService parameterService) {
			UpdateFromList(parameterService.Parameters);
		}
		public void UpdateFromList(IEnumerable<IParameter> otherParameters) {
			this.Clear();
			this.AddRange(otherParameters);
		}
		#region Events
		#region ParameterAdded event
		ParametersCollectionChangedEventHandler onParameterAdded;
		internal event ParametersCollectionChangedEventHandler ParameterAdded { add { onParameterAdded += value; } remove { onParameterAdded -= value; } }
		protected internal virtual void RaiseParameterAdded(SpreadsheetParameter parameter) {
			if(onParameterAdded != null) {
				ParametersCollectionChangedEventArgs args = new ParametersCollectionChangedEventArgs(parameter);
				onParameterAdded(this, args);
			}
		}
		#endregion
		#region ParameterInserted event
		ParameterInsertedEventHandler onParameterInserted;
		internal event ParameterInsertedEventHandler ParameterInserted { add { onParameterInserted += value; } remove { onParameterInserted -= value; } }
		protected internal virtual void RaiseParameterInserted(int index, SpreadsheetParameter newParameter) {
			if(onParameterInserted != null) {
				ParameterInsertedEventArgs args = new ParameterInsertedEventArgs(index, newParameter);
				onParameterInserted(this, args);
			}
		}
		#endregion
		#region ParameterRemoved event
		ParametersCollectionChangedEventHandler onParameterRemoved;
		internal event ParametersCollectionChangedEventHandler ParameterRemoved { add { onParameterRemoved += value; } remove { onParameterRemoved -= value; } }
		protected internal virtual void RaiseParameterRemoved(SpreadsheetParameter parameter) {
			if(onParameterRemoved != null) {
				ParametersCollectionChangedEventArgs args = new ParametersCollectionChangedEventArgs(parameter);
				onParameterRemoved(this, args);
			}
		}
		#endregion
		#region Clear event
		EventHandler onCollectionClear;
		internal event EventHandler CollectionCleared { add { onCollectionClear += value; } remove { onCollectionClear -= value; } }
		protected internal virtual void RaiseCollectionCleared() {
			if(onCollectionClear != null) {
				EventArgs args = new EventArgs();
				onCollectionClear(this, args);
			}
		}
		#endregion
		#endregion
	}
	internal class ParameterService : IParameterService {
		#region Fields
		readonly List<IParameter> parameters;
		#endregion
		public ParameterService(MailMergeParametersCollection parametersCollection) {
			parameters = new List<IParameter>();
			parameters.AddRange(parametersCollection.InnerList);
		}
		#region IParameterService Members
		public string AddParameterString { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.NewSpreadsheetMailMergeParameter_Text); } }
		public string CreateParameterString { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.EditSpreadsheetMailMergeParameter_Text); } }
		public bool CanCreateParameters { get { return true; } }
		public void AddParameter(IParameter parameter) {
			parameters.Add(parameter);
		}
		public IParameter CreateParameter(Type type) {
			SpreadsheetParameter parameter = new SpreadsheetParameter {Type = type, Name = CreateName()};
			return parameter;
		}
		public IEnumerable<IParameter> Parameters { get { return parameters; } }
		#endregion
		public string CreateName() {
			const string name = "parameter";
			HashSet<string> existingNames = new HashSet<string>(DXListExtensions.ConvertAll<IParameter, string>(parameters, p => p.Name));
			int number = 1;
			while(existingNames.Contains(name + number))
				number++;
			return name + number;
		}
	}
	[TypeConverter("DevExpress.XtraReports.Design.ParameterValueEditorChangingConverter," + AssemblyInfo.SRAssemblyUtilsUI)]
	public class SpreadsheetParameter : IParameter, INotifyPropertyChanged {
		#region Fields
		Type type;
		string name;
		object value;
		#endregion
		#region Properties
		[Browsable(false)]
		[DefaultValue("")]
		public string Name {
			get { return name; }
			set {
				if(name == value)
					return;
				name = value;
				OnPropertyChanged("Name");
			}
		}
		[TypeConverter("DevExpress.XtraReports.Design.ParameterTypeConverter," + AssemblyInfo.SRAssemblyUtilsUI),
		 RefreshProperties(RefreshProperties.All)]
		public Type Type {
			get { return type ?? typeof(String); }
			set {
				type = value;
				if(ParameterHelper.ShouldConvertValue(this.Value, this.Type))
					this.value = ParameterHelper.ConvertFrom(this.Value, this.Type, this.DefaultValue);
				OnPropertyChanged("Type");
			}
		}
		public object Value {
			get { return value ?? DefaultValue; }
			set {
				object validatedValue = ValidateValue(value);
				if(this.Value == validatedValue)
					return;
				this.value = validatedValue;
				OnPropertyChanged("Value");
			}
		}
		object DefaultValue { get { return ParameterHelper.GetDefaultValue(Type); } }
		#endregion
		public SpreadsheetParameter() {
			type = null;
			name = String.Empty;
			value = null;
		}
		object ValidateValue(object newValue) {
			if(newValue==null || newValue.GetType() == Type || Type.IsInstanceOfType(newValue))
				return newValue;
			return ParameterHelper.ConvertFrom(newValue, Type, Value);
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName) {
			PropertyChangedEventHandler handler = PropertyChanged;
			if(handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	internal static class ParameterHelper {
		static object CreateDefaultValue(Type type) {
			if(type.IsClass() || type.IsInterface()) {
				return null;
			}
			if(type.IsEnum()) {
				return EnumExtensions.GetValues(type).GetValue(0);
			}
			return Activator.CreateInstance(type);
		}
		public static object GetDefaultValue(Type type) {
			var typeCode = DXTypeExtensions.GetTypeCode(type);
			switch(typeCode) {
				case TypeCode.String:
					return string.Empty;
				case TypeCode.DateTime:
					return DateTime.MinValue;
				default:
					return CreateDefaultValue(type);
			}
		}
		public static bool ShouldConvertValue(object value, Type type) {
			return value == null || value.GetType() != type;
		}
		public static object ConvertFrom(object value, Type type, object defaultValue) {
			try {
				TypeConverter converterTo = TypeDescriptor.GetConverter(value.GetType());
				if(converterTo.CanConvertTo(null, type))
					return converterTo.ConvertTo(null, CultureInfo.InvariantCulture, value, type);
				else {
					TypeConverter converterFrom = TypeDescriptor.GetConverter(type);
					if(converterFrom.CanConvertFrom(null, value.GetType())) {
						return converterFrom.ConvertFrom(null, CultureInfo.InvariantCulture, value);
					}
				}
			}
			catch {}
			return defaultValue;
		}
		public static string ConvertValueToString(object value) {
			try {
				if(value != null) {
					TypeConverter converter = TypeDescriptor.GetConverter(value.GetType());
					if(converter != null && converter.CanConvertTo(null, typeof(string)))
						return (string) converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(string));
				}
			}
			catch {}
			return string.Empty;
		}
	}
}
