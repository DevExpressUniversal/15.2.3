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
using System.ComponentModel;
using DevExpress.Printing;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using System.Collections;
using System.Collections.Generic;
#if SL
using DevExpress.Xpf.ComponentModel;
#endif
namespace DevExpress.XtraReports.Parameters {
#if !SL
	[TypeConverter(typeof(EnumTypeConverter))]
#endif
	[ResourceFinder(typeof(ResFinder))]
	public enum ParameterType {
		String,
		DateTime,
		Int32,
		Int64,
		Float,
		Double,
		Decimal,
		Boolean,
	}
	[ToolboxItem(false)]
	[DesignTimeVisibleAttribute(false)]
	[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.Parameters.Parameter")]
#if !SL
	[TypeConverter("DevExpress.XtraReports.Design.ParameterValueEditorChangingConverter," + AssemblyInfo.SRAssemblyUtilsUI)]
	[Designer("DevExpress.XtraReports.Design.Designers.ParameterDesigner," + AssemblyInfo.SRAssemblyReportsExtensions)]
#endif
	public class Parameter : Component, IXtraSupportShouldSerialize, DevExpress.Data.IParameter, DevExpress.Data.IMultiValueParameter {
		#region Fields & Properties
		object value = null;
		Type type = null;
		string name = string.Empty;
		LookUpSettings lookUpSettings;
		bool multiValue;
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object RawValue {
			get { return value; }
		}
		[Obsolete("The ParameterType property is now obsolete. Use the Type property instead.")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public ParameterType ParameterType {
			get { return ParameterType.String; }
			set { Type = ToType(value); }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("ParameterVisible")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.Parameters.Parameter.Visible")]
		[TypeConverter(typeof(BooleanTypeConverter))]
		[DefaultValue(true)]
		[Category("Behavior")]
		[XtraSerializableProperty]
		public bool Visible {
			get;
			set;
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("ParameterDescription")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.Parameters.Parameter.Description")]
		[DefaultValue("")]
		[Localizable(true)]
		[Category("Data")]
		[XtraSerializableProperty]
#if !SL
		[Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
#endif
		public string Description {
			get;
			set;
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("ParameterLookUpSettings")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.Parameters.Parameter.LookUpSettings")]
		[XtraSerializableProperty(XtraSerializationVisibility.Reference)]
		[Localizable(true)]
		[DefaultValue(null)]
		[Category("Data")]
		[RefreshProperties(RefreshProperties.All)]
#if !SL
		[TypeConverter("DevExpress.XtraReports.Design.ParameterLookUpSettingsConverter," + AssemblyInfo.SRAssemblyUtilsUI)]
		[Editor("DevExpress.XtraReports.Design.ParameterLookUpSettingsEditor," + AssemblyInfo.SRAssemblyUtilsUI, typeof(System.Drawing.Design.UITypeEditor))]
#endif
		public virtual LookUpSettings LookUpSettings {
			get {
				SetOwnerParameter(lookUpSettings);
				return lookUpSettings;
			}
			set {
				lookUpSettings = value;
				SetOwnerParameter(lookUpSettings);
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("ParameterMultiValue")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.Parameters.Parameter.MultiValue")]
		[XtraSerializableProperty]
		[Localizable(true)]
		[DefaultValue(false)]
		[Category("Data")]
		[TypeConverter(typeof(BooleanTypeConverter))]
		public virtual bool MultiValue {
			get { return multiValue; }
			set {
				multiValue = value;
				ConvertValue();
			}
		}
#if !SL
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XtraSerializableProperty]
		public string ValueInfo {
			get;
			set;
		}
		TypeCode TypeCode {
			get { return Type.GetTypeCode(Type); }
		}
#endif
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XtraSerializableProperty(-1)]
		public string ObjectType {
			get { return GetType().AssemblyQualifiedName; }
		}
		internal bool IsLoading {
			get { return Owner == null || Owner.IsLoading; }
		}
		protected internal ParameterCollection Owner {
			get;
			set;
		}
		object DefaultValue {
			get { return ParameterHelper.GetDefaultValue(Type); }
		}
		#endregion
		#region Constructors
		public Parameter() {
			Description = string.Empty;
			Visible = true;
#if !SL
			ValueInfo = string.Empty;
#endif
		}
		#endregion
		#region Methods
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Owner != null)
					Owner.Remove(this);
			}
			base.Dispose(disposing);
		}
#if !SL
		void SetValueCore(object value) {
			this.value = value;
			ValueInfo = string.Empty;
		}
		void ResetValue() {
			SetValueCore(DefaultValue);
		}
		object GetValueCore() {
			object deserializedValue;
			if(Deserialize(ValueInfo, Type, out deserializedValue))
				SetValueCore(deserializedValue);
			return this.value ?? (multiValue ? Array.CreateInstance(Type, 0) : DefaultValue);
		}
		bool ShouldSerializeValueInfo() {
			if(string.IsNullOrEmpty(ValueInfo))
				ValueInfo = SerializeValue();
			return !string.IsNullOrEmpty(this.ValueInfo);
		}
		string SerializeValue() {
			if(MultiValue && Value is IEnumerable) { 
				return SerializeMultiValue((IEnumerable)Value);
			} 
		   return SerializeSingleValue(Value);
		}
		string SerializeMultiValue(IEnumerable value) {
			List<string> list = new List<string>();
			foreach(object element in value) {
				if(Type.IsAssignableFrom(element.GetType())) {
					list.Add(EscapeString(SerializeSingleValue(element)));
				}
			}
			return string.Join(separator, list.ToArray());
		}
		string SerializeSingleValue(object value) {
			if(TypeCode == TypeCode.Object && Type != typeof(Guid))
				return Owner != null ? Owner.Serialize(value) : string.Empty;
			else
				return ParameterHelper.ConvertValueToString(value);
		}
		bool ShouldSerializeValue() {
			return value != null;
		}
		bool ShouldSerializeType() {
			return Type != typeof(String);
		}
		bool Deserialize(string value, Type destType, out object result) {
			if(!string.IsNullOrEmpty(value)) {
				if(MultiValue)
					return DeserializeMultiValue(value, destType, out result);
				return DeserializeSingeValue(value, destType, out result);
			}
			result = null;
			return false;
		}
		bool DeserializeMultiValue(string value, System.Type destType, out object result) {
			ArrayList values = new ArrayList();
			foreach(string part in GetParts(value)) { 
				object singleResult = null;
				if(DeserializeSingeValue(UnescapeString(part), destType, out singleResult))
					values.Add(singleResult);
			}
			result = values.Count > 0 ? values.ToArray(destType) : null;
			return result != null;
		}
		bool DeserializeSingeValue(string value, System.Type destType, out object result) {
			if(Owner != null && Owner.Deserialize(value, destType.FullName, out result))
				return true;
			result = ParameterHelper.ConvertFrom(value, destType, null);
			return result != null;
		}
		const char separatorChar = '|';
		const string separator = "|";
		static string EscapeString(string source) {
			return source.Replace("\\", "\\\\").Replace(separator, "\\" + separator);
		}
		static string UnescapeString(string source) {
			return source.Replace("\\" + separator, separator).Replace("\\\\", "\\");
		}
		string[] GetParts(string value) {
			return System.Text.RegularExpressions.Regex.Split(value, @"(?<![^\\](\\\\)*\\)\|");
		}
		void ConvertValue() {
			if(IsLoading)
				return;
			object value = Value;
			if(TryConvertValue(ref value, DefaultValue, Type, MultiValue, System.Globalization.CultureInfo.InvariantCulture))
				SetValueCore(value);
		}
		internal static bool TryConvertValue(ref object value, object defaultValue, Type type, bool multiValue, System.Globalization.CultureInfo culture) {
			if(!multiValue) {
				if(ParameterHelper.ShouldConvertValue(value, type)) {
					value = ParameterHelper.ConvertFrom(value, type, defaultValue, culture);
					return true;
				}
				return false;
			}
			if(value == null)
				return false;
			ArrayList values = new ArrayList();
			if(value is string || !(value is IEnumerable)) {
				object newValue = ParameterHelper.ShouldConvertValue(value, type) ? ParameterHelper.ConvertFrom(value, type, defaultValue, culture) : value;
				values.Add(newValue);
			} else if(value is IEnumerable) {
				foreach(object element in (IEnumerable)value) {
					if(!ParameterHelper.ShouldConvertValue(element, type))
						values.Add(element);
				}
			}
			value = values.ToArray(type);
			return true;
		}
#else
		void ResetValue() {
			this.value = DefaultValue;
		}
		void SetValueCore(object value) {
			this.value = value;
		}
		object GetValueCore() {
			return this.value ?? DefaultValue;
		}
#endif
		static Type ToType(ParameterType value) {
			if(value == ParameterType.Boolean)
				return typeof(bool);
			if(value == ParameterType.DateTime)
				return typeof(DateTime);
			if(value == ParameterType.Decimal)
				return typeof(Decimal);
			if(value == ParameterType.Double)
				return typeof(double);
			if(value == ParameterType.Float)
				return typeof(float);
			if(value == ParameterType.Int32)
				return typeof(int);
			if(value == ParameterType.Int64)
				return typeof(long);
			if(value == ParameterType.String)
				return typeof(string);
			return typeof(object);
		}
		void SetOwnerParameter(LookUpSettings lookUpSettings) {
			if(lookUpSettings != null) {
				lookUpSettings.Parameter = this;
			}
		}
		object ValidateValue(object value) {
			return (value is string && this.Type != typeof(string))
				? ParameterHelper.ConvertFrom(value, this.Type, this.DefaultValue)
				: value;
		}
		System.Type GetTypeFromValue() {
			if(MultiValue || value == null || value == DBNull.Value)
				return null;
			return value.GetType();
		}
		#endregion
		#region IParameter Members
		[DefaultValue("")]
		[Browsable(false)]
		[XtraSerializableProperty]
		public string Name {
			get { return Site != null ? Site.Name : name; }
			set { name = value; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("ParameterType")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.Parameters.Parameter.Type")]
		[RefreshProperties(RefreshProperties.All)]
		[Category("Data")]
		[XtraSerializableProperty(XtraSerializationVisibility.Reference)]
#if !SL
		[TypeConverter("DevExpress.XtraReports.Design.ParameterTypeConverter," + AssemblyInfo.SRAssemblyUtilsUI)]
#endif
		public virtual Type Type {
			get { return type ?? GetTypeFromValue() ?? typeof(String); }
			set {
				type = value;
				ConvertValue();
				if(LookUpSettings != null) {
					LookUpSettings.SyncParameterType(type);
				}
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("ParameterValue")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.Parameters.Parameter.Value")]
		[Category("Data")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Value {
			get { return GetValueCore(); }
			set {
				if(IsLoading)
					this.value = value;
				else
					SetValueCore(ValidateValue(value));
			}
		}
		#endregion
		#region IXtraSupportShouldSerialize Members
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			return (propertyName == "ObjectType")
				? GetType() != typeof(Parameter)
				: true;
		}
		#endregion
	}
}
