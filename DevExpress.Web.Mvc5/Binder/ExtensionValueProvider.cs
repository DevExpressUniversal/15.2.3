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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.ASPxHtmlEditor;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.BinderSettings;
namespace DevExpress.Web.Mvc.BinderSettings {
	public class UploadControlBinderSettings : BinderSettingsBase {
		public UploadControlBinderSettings() {
			ValidationSettings = new UploadControlValidationSettings();
			AzureSettings = new UploadControlAzureSettings();
			AmazonSettings = new UploadControlAmazonSettings();
			DropboxSettings = new UploadControlDropboxSettings();
		}
		public UploadControlUploadStorage UploadStorage { get; set; }
		public UploadControlValidationSettings ValidationSettings { get; private set; }
		public UploadControlAzureSettings AzureSettings { get; private set; }
		public UploadControlAmazonSettings AmazonSettings { get; private set; }
		public UploadControlDropboxSettings DropboxSettings { get; private set; }
		public EventHandler<FileUploadCompleteEventArgs> FileUploadCompleteHandler { get; set; }
	}
	public class HtmlEditorBinderSettings : BinderSettingsBase {
		public HtmlEditorBinderSettings() {
			HtmlEditingSettings = new ASPxHtmlEditorHtmlEditingSettings();
			ValidationSettings = new HtmlEditorValidationSettings();
		}
		public ASPxHtmlEditorHtmlEditingSettings HtmlEditingSettings { get; private set; }
		public HtmlEditorValidationSettings ValidationSettings { get; private set; }
		public EventHandler<HtmlEditorValidationEventArgs> ValidationHandler { get; set; }
		public EventHandler<HtmlCorrectingEventArgs> HtmlCorrectingHandler { get; set; }
	}
	public abstract class BinderSettingsBase { }
}
namespace DevExpress.Web.Mvc.Internal {
	public class ExtensionValueProvidersFactory {
		static List<ExtensionValueProviderBase> valueProviders;
		protected static List<ExtensionValueProviderBase> ValueProviders {
			get {
				if(valueProviders == null)
					valueProviders = CreateRegisteredProvidersCollection();
				return valueProviders;
			}
		}
		public static bool TryGetValue(ModelBindingContext bindingContext, out object value) {
			var provider = ValueProviders.FirstOrDefault(p => p.CanGetValue(bindingContext, bindingContext.ModelName));
			bool canGetValue = provider != null;
			value = canGetValue ? provider.GetValue(bindingContext, null) : null;
			return canGetValue;
		}
		public ExtensionValueProviderBase GetValueProvider(ModelBindingContext bindingContext, string propertyName) {
			return ValueProviders.FirstOrDefault(p => p.CanGetValue(bindingContext, propertyName));
		}
		public BinderSettingsBase GetValueProviderSettings(Type providerType) {
			ExtensionValueProviderBase provider = ValueProviders.FirstOrDefault(p => Type.Equals(p.GetType(), providerType));
			return provider != null ? provider.BinderSettings : null;
		}
		static List<ExtensionValueProviderBase> CreateRegisteredProvidersCollection() {
			var providers = new List<ExtensionValueProviderBase>();
			providers.Add(new UploadControlValueProvider());
			providers.Add(new GridValueProvider());
			providers.Add(new BinaryImageValueProvider());
			providers.Add(new EditorValueProvider());
			if(HasAssemblyInDomain(HtmlEditorValueProvider.MarkerAssemblyName))
				providers.Add(new HtmlEditorValueProvider());
			if(HasAssemblyInDomain(ReportDesignerValueProvider.MarkerAssemblyName))
				providers.Add(new ReportDesignerValueProvider());
			if(HasAssemblyInDomain(TreeListValueProvider.MarkerAssemblyName))
				providers.Add(new TreeListValueProvider());
			return providers;
		}
		static bool HasAssemblyInDomain(string assemblyName) {
			string fullAssamblyName = assemblyName + AssemblyInfo.FullAssemblyVersionExtension;
			return AppDomain.CurrentDomain.GetAssemblies().Any(assembly => assembly.FullName.Equals(fullAssamblyName, StringComparison.InvariantCultureIgnoreCase));
		}
	}
	public class UploadControlValueProvider : ExtensionValueProviderBase {
		public UploadControlValueProvider() {
			BinderSettings = new UploadControlBinderSettings();
		}
		public override string PropertyPostfix { get { return "_DXMVCUploadControl"; } }
		public new UploadControlBinderSettings BinderSettings {
			get { return (UploadControlBinderSettings)base.BinderSettings; }
			set { base.BinderSettings = value; }
		}
		public override object GetValue(ModelBindingContext bindingContext, ControllerContext controllerContext) {
			UploadControlSettings settings = CreateSettings(bindingContext.ModelName);
			return UploadControlExtension.GetUploadedFiles(settings, BinderSettings.FileUploadCompleteHandler);
		}
		UploadControlSettings CreateSettings(string name) {
			UploadControlSettings settings = new UploadControlSettings { Name = name };
			settings.UploadStorage = BinderSettings.UploadStorage;
			settings.ValidationSettings.Assign(BinderSettings.ValidationSettings);
			settings.AzureSettings.Assign(BinderSettings.AzureSettings);
			settings.AmazonSettings.Assign(BinderSettings.AmazonSettings);
			return settings;
		}
	}
	public class HtmlEditorValueProvider : ExtensionValueProviderBase {
		public HtmlEditorValueProvider() {
			BinderSettings = new HtmlEditorBinderSettings();
		}
		public static string MarkerAssemblyName { get { return AssemblyInfo.SRAssemblyHtmlEditorWeb; } }
		public override string PropertyPostfix { get { return "_DXMVCHtmlEditor"; } }
		public new HtmlEditorBinderSettings BinderSettings {
			get { return (HtmlEditorBinderSettings)base.BinderSettings; }
			set { base.BinderSettings = value; }
		}
		public override object GetValue(ModelBindingContext bindingContext, ControllerContext controllerContext) {
			bool isValid;
			var html = HtmlEditorExtension.GetHtml(bindingContext.ModelName, BinderSettings.HtmlEditingSettings, BinderSettings.ValidationSettings,
				BinderSettings.ValidationHandler, BinderSettings.HtmlCorrectingHandler, out isValid);
			foreach(var error in HtmlEditorExtension.ErrorTexts) {
				bindingContext.ModelState.AddModelError(error.Key, error.Value);
			}
			return ExtensionValueProviderBase.ConvertValue(html, bindingContext.ModelType);
		}
	}
	public class ReportDesignerValueProvider : ExtensionValueProviderBase {
		public static string MarkerAssemblyName { get { return AssemblyInfo.SRAssemblyReportsWeb; } }
		public static string Postfix = "_DXReportDesigner";
		public override string PropertyPostfix { get { return Postfix; } }
		public override object GetValue(ModelBindingContext bindingContext, ControllerContext controllerContext) {
			var reportXmlLayout = ReportDesignerExtension.GetReportXml(bindingContext.ModelName);
			if(bindingContext.ModelType == typeof(byte[]))
				return reportXmlLayout;
			if(bindingContext.ModelType == typeof(string))
				return Encoding.UTF8.GetString(reportXmlLayout);
			return ConvertValue(reportXmlLayout, bindingContext.ModelType);
		}
	}
	public class TreeListValueProvider : EditFormOwnerValueProvider<TreeListValueProvider> {
		public static string MarkerAssemblyName { get { return AssemblyInfo.SRAssemblyTreeListWeb; } }
		public const string FieldListRequestKeyConst = "DXMVCTreeListEditFields";
		protected override string FieldListRequestKey { get { return FieldListRequestKeyConst; } }
	}
	public class GridValueProvider : EditFormOwnerValueProvider<GridValueProvider> {
		public const string FieldListRequestKeyConst = "DXMVCGridEditFields";
		protected override string FieldListRequestKey { get { return FieldListRequestKeyConst; } }
		public override bool CanGetValue(ModelBindingContext bindingContext, string propertyName) {
			return CanGetBatchEditValue(bindingContext) || base.CanGetValue(bindingContext, propertyName);
		}
		public override object GetValue(ModelBindingContext bindingContext, ControllerContext controllerContext) {
			if(CanGetBatchEditValue(bindingContext))
				return ConvertValue(GetUnvalidatedValue(bindingContext.ValueProvider, bindingContext.ModelName), bindingContext.ModelType);
			return base.GetValue(bindingContext, controllerContext);
		}
		bool CanGetBatchEditValue(ModelBindingContext bindingContext) {
			string modelName = bindingContext.ModelName;
			var valueProvider = bindingContext.ValueProvider;
			var dxValueProvider = bindingContext.ValueProvider as MvcPostDataCollection;
			return dxValueProvider != null && dxValueProvider.IsBatchEditCollection && GetUnvalidatedValue(valueProvider, modelName) != null;
		}
		public static List<T> GetBatchInsertValues<T>(string name) {
			var insertState = new Dictionary<int, Dictionary<string, string>>();
			GridBatchEditHelper.LoadClientState(GetBatchState(), null, insertState, null, null);
			return GetInsertValuesCore<T>(name, insertState);
		}
		public static Dictionary<S, T> GetBatchUpdateValues<S, T>(string fieldName) {
			var updateState = new Dictionary<string, Dictionary<string, string>>();
			GridBatchEditHelper.LoadClientState(GetBatchState(), null, null, updateState, null);
			return GetUpdateValuesCore<S, T>(fieldName, updateState);
		}
		public static List<T> GetBatchDeleteKeys<T>() {
			var deleteState = new List<string>();
			GridBatchEditHelper.LoadClientState(GetBatchState(), null, null, null, deleteState);
			return GetDeleteKeysCore<T>(deleteState);
		}
		static Hashtable GetBatchState() {
			var key = HttpUtils.GetValueFromRequest(MVCxGridBatchEditHelperAdapter.MVCBatchEditingValuesRequestKey);
			return MVCxGridView.GetGridClientObjectBatchState(key, HttpUtils.GetValueFromRequest);
		}
		static List<T> GetInsertValuesCore<T>(string fieldName, Dictionary<int, Dictionary<string, string>> state) {
			if(state.Count == 0)
				return null;
			var insertValues = new List<T>();
			var sortedKeys = state.Keys.OrderBy(k => k).ToList();
			for(var i = 0; i < sortedKeys.Count; i++) {
				var rowValues = state[sortedKeys[i]];
				if(rowValues.ContainsKey(fieldName))
					insertValues.Add((T)ConvertValue(rowValues[fieldName], typeof(T)));
			}
			return insertValues;
		}
		static Dictionary<S, T> GetUpdateValuesCore<S, T>(string fieldName, Dictionary<string, Dictionary<string, string>> state) {
			if(state.Count == 0)
				return null;
			var updateValues = new Dictionary<S, T>();
			var sortedKeys = state.Keys.OrderBy(k => k).ToList();
			for(var i = 0; i < sortedKeys.Count; i++) {
				var rowValues = state[sortedKeys[i]];
				if(rowValues.ContainsKey(fieldName))
					updateValues.Add((S)ConvertValue(sortedKeys[i], typeof(S)), (T)ConvertValue(rowValues[fieldName], typeof(T)));
			}
			return updateValues;
		}
		static List<T> GetDeleteKeysCore<T>(List<string> state) {
			var deleteKeys = new List<T>();
			foreach(var key in state) {
				deleteKeys.Add((T)ConvertValue(key, typeof(T)));
			}
			return deleteKeys;
		}
	}
	public abstract class EditFormOwnerValueProvider<InheritorType> : EditorValueProvider<InheritorType> where InheritorType : EditFormOwnerValueProvider<InheritorType> {
		protected abstract string FieldListRequestKey { get; }
		public override object GetValue(ModelBindingContext bindingContext, ControllerContext controllerContext) {
			if(IsBinaryType(bindingContext.ModelType)) {
				string key = GetEditorValues(bindingContext.ValueProvider)[bindingContext.ModelName] as string;
				return BinaryImageValueProvider.GetValueByKey(key);
			}
			return base.GetValue(bindingContext, controllerContext);
		}
		static bool IsBinaryType(Type type) {
			return type == typeof(byte[]);
		}
		public override IDictionary GetEditorValues(string key) {
			Hashtable editorValues = new Hashtable();
			IList fieldList = GetFieldList();
			foreach(string fieldName in fieldList) {
				string jsonValue = HttpUtils.GetValueFromRequest(fieldName);
				if(!string.IsNullOrEmpty(jsonValue)) {
					var fieldValue = HtmlConvertor.FromJSON(jsonValue);
					if(fieldValue == null)
						continue;
					editorValues.Add(fieldName, fieldValue);
				}
			}
			return editorValues;
		}
		public override IDictionary GetEditorValues(System.Web.Mvc.IValueProvider valueProvider) {
			Hashtable editorValues = new Hashtable();
			IList fieldList = GetFieldList(valueProvider);
			foreach(string fieldName in fieldList) {
				if(!valueProvider.ContainsPrefix(fieldName))
					continue;
				string jsonValue = GetUnvalidatedValue(valueProvider, fieldName);
				object fieldValue = !string.IsNullOrEmpty(jsonValue) ? HtmlConvertor.FromJSON(jsonValue) : null;
				if(fieldValue == null)
					continue;
				editorValues.Add(fieldName, fieldValue);
			}
			return editorValues;
		}
		protected IList GetFieldList() {
			string jsonFieldList = HttpUtils.GetValueFromRequest(FieldListRequestKey);
			return GetFieldListCore(jsonFieldList);
		}
		protected IList GetFieldList(System.Web.Mvc.IValueProvider valueProvider) {
			string jsonFieldList = GetUnvalidatedValue(valueProvider, FieldListRequestKey);
			return GetFieldListCore(jsonFieldList);
		}
		static IList GetFieldListCore(string jsonFieldList) {
			if(string.IsNullOrEmpty(jsonFieldList))
				return new List<string>();
			return HtmlConvertor.FromJSON<IList>(jsonFieldList);
		}
	}
	public class BinaryImageValueProvider : EditorValueProvider {
		const string BinaryImageEditModePostfix = "BIEM";
		public override string PropertyPostfix { get { return MVCxBinaryImage.UploadControlPostfix + "_DXMVCUploadControl"; } }
		public override object GetValue(ModelBindingContext bindingContext, ControllerContext controllerContext) {
			string key = GetEditorValues(bindingContext.ValueProvider)[bindingContext.ModelName] as string;
			return GetValueByKey(key);
		}
		public static object GetValueByKey(string key){
			if(string.IsNullOrEmpty(key))
				return null;
			BinaryStorageMode storageMode = key.EndsWith(BinaryImageEditModePostfix) ? BinaryStorageMode.Cache : BinaryStorageMode.Session;
			BinaryStorageData storageData = BinaryStorage.GetResourceData(null, storageMode, key);
			return storageData != null ? storageData.Content : null;
		}
	}
	public class EditorValueProvider : EditorValueProvider<EditorValueProvider> { }
	public class EditorValueProvider<InheritorType> : ExtensionValueProviderBase where InheritorType : EditorValueProvider<InheritorType> {
		public const string CombinedValueParamName = "DXMVCEditorsValues";
		protected static IDictionary DefaultValueCollection = new Hashtable();
		public static T GetValue<T>(string name) {
			object rawValue = GetValue(name);
			if(typeof(T) == typeof(byte[]) && !(rawValue is ICollection)) 
				return (T)BinaryImageValueProvider.GetValueByKey(rawValue as string);
			return (T)ConvertValue(rawValue, typeof(T));
		}
		public static object GetValue(string name) {
			var provider = Activator.CreateInstance<InheritorType>();
			var rawValues = provider.GetEditorValues(CombinedValueParamName);
			if(rawValues.Contains(name))
				return rawValues[name];
			return HttpUtils.GetValueFromRequest(name);
		}
		public override bool CanGetValue(ModelBindingContext bindingContext, string propertyName) {
			IDictionary rawValues = GetEditorValues(bindingContext.ValueProvider);
			return rawValues.Contains(propertyName) && base.CanGetValue(bindingContext, propertyName);
		}
		public override object GetValue(ModelBindingContext bindingContext, ControllerContext controllerContext) {
			object rawValue = GetEditorValues(bindingContext.ValueProvider)[bindingContext.ModelName];
			if(rawValue is string && !SkipValidation(bindingContext, controllerContext))
				DevExpressRequestValidator.ValidateRequestString(rawValue.ToString());
			try {
				return ConvertValue(rawValue, bindingContext.ModelType);
			} catch(Exception exc) {
				AddExceptionToModelState(bindingContext, exc);
			}
			return null;
		}
		bool SkipValidation(ModelBindingContext bindingContext, ControllerContext controllerContext) {
			return bindingContext != null && controllerContext != null && !(controllerContext.Controller.ValidateRequest && bindingContext.ModelMetadata.RequestValidationEnabled);
		}
		void AddExceptionToModelState(ModelBindingContext bindingContext, Exception exception) {
			if(exception == null)
				return;
			ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			bindingContext.ModelState.AddModelError(bindingContext.ModelName, exception);
			bindingContext.ModelState.SetModelValue(bindingContext.ModelName, new ValueProviderResult(null, valueProviderResult.AttemptedValue, valueProviderResult.Culture));
		}
		public virtual IDictionary GetEditorValues(string key) {
			string jsonValue = HttpUtils.GetValueFromRequest(key);
			return GetEditorValuesCore(jsonValue);
		}
		public virtual IDictionary GetEditorValues(System.Web.Mvc.IValueProvider valueProvider) {
			if(valueProvider == null)
				return GetEditorValues(CombinedValueParamName);
			string jsonValue = GetUnvalidatedValue(valueProvider, CombinedValueParamName);
			return GetEditorValuesCore(jsonValue);
		}
		static IDictionary GetEditorValuesCore(string jsonValue) {
			if(string.IsNullOrEmpty(jsonValue))
				return DefaultValueCollection;
			IDictionary rawValues = HtmlConvertor.FromJSON<IDictionary>(jsonValue);
			return rawValues != null ? new Hashtable(rawValues, StringComparer.InvariantCultureIgnoreCase) : DefaultValueCollection;
		}
		protected static string GetUnvalidatedValue(System.Web.Mvc.IValueProvider valueProvider, string key) {
			IUnvalidatedValueProvider unvalidatedValueProvider = valueProvider as IUnvalidatedValueProvider;
			ValueProviderResult valueProviderResult = unvalidatedValueProvider != null ? unvalidatedValueProvider.GetValue(key, true) : valueProvider.GetValue(key);
			return valueProviderResult != null ? valueProviderResult.AttemptedValue : null;
		}
	}
	public abstract class ExtensionValueProviderBase {
		public virtual string PropertyPostfix { get { return null; } }
		public BinderSettingsBase BinderSettings { get; protected set; }
		public virtual bool CanGetValue(ModelBindingContext bindingContext, string propertyName) {
			return bindingContext.ValueProvider != null && bindingContext.ValueProvider.ContainsPrefix(propertyName + PropertyPostfix);
		}
		public abstract object GetValue(ModelBindingContext bindingContext, ControllerContext controllerContext);
		public static object ConvertValue(object rawValue, Type type) {
			if(IsCollectionType(type))
				return ConvertValueToCollectionType(rawValue, type);
			rawValue = GetFirstElementInMultipleValue(rawValue, type);
			if(rawValue == null || DBNull.Value == rawValue || IsNotRequiredEmptyStringValue(rawValue, type))
				return null;
			Type resultType = Nullable.GetUnderlyingType(type) ?? type;
			TypeConverter converter = TypeDescriptor.GetConverter(resultType);
			rawValue = FixFloatingPoint(rawValue, resultType);
			if(converter != null && converter.CanConvertFrom(rawValue.GetType()))
				return converter.ConvertFrom(null, CultureInfo.InvariantCulture, rawValue);
			if(resultType.IsEnum && resultType.IsEnumDefined(rawValue))
				return Enum.ToObject(resultType, rawValue);
			return Convert.ChangeType(rawValue, resultType, CultureInfo.InvariantCulture);
		}
		static bool IsNotRequiredEmptyStringValue(object value, Type type) {
			string stringValue = value as string;
			return stringValue != null && type != typeof(String) && String.IsNullOrWhiteSpace(stringValue);
		}
		static object FixFloatingPoint(object value, Type type) {
			if(DataUtils.IsFloatType(type) && value is string)
				return DataUtils.FixFloatingPoint(value.ToString(), CultureInfo.InvariantCulture);
			return value;
		}
		static bool IsCollectionType(Type type) {
			return type != typeof(string) && ReflectionUtils.IsGenericIEnumerable(type);
		}
		static object ConvertValueToCollectionType(object rawValue, Type collectionType) {
			if(rawValue == null)
				return null;
			object instance = CreateInstance(collectionType);
			IList collection = instance as IList;
			if(collection == null)
				return instance;
			FillCollection(collection, rawValue);
			return collectionType.IsArray ? CreateArrayFromCollection(collection) : collection;
		}
		static object CreateInstance(Type requiredType) {
			Type typeToCreate = ResolveInterfaceAndArrayTypes(requiredType);
			return Activator.CreateInstance(typeToCreate);
		}
		static Type ResolveInterfaceAndArrayTypes(Type collectionType) {
			Type elementType = GetCollectionElementType(collectionType);
			return IsSupportedInterfaceForList(collectionType) || collectionType.IsArray ? typeof(List<>).MakeGenericType(elementType) : collectionType;
		}
		static Type GetCollectionElementType(Type collectionType) {
			return collectionType.IsArray ? collectionType.GetElementType() : ReflectionUtils.ExtractGenericIEnumerable(collectionType).GetGenericArguments().First();
		}
		static bool IsSupportedInterfaceForList(Type type) {
			Type[] supportedInterfacesForList = new Type[] { typeof(IEnumerable<>), typeof(ICollection<>), typeof(IList<>) };
			return type.IsGenericType && supportedInterfacesForList.Contains(type.GetGenericTypeDefinition());
		}
		static void FillCollection(IList collection, object rawValue) {
			ArrayList rawArray = rawValue is ICollection ? new ArrayList(rawValue as ICollection) : new ArrayList() { rawValue };
			Type elementType = GetCollectionElementType(collection.GetType());
			foreach(var rawItem in rawArray) {
				collection.Add(ConvertValue(rawItem, elementType));
			}
		}
		static object CreateArrayFromCollection(IList elements) {
			var array = Array.CreateInstance(GetCollectionElementType(elements.GetType()), elements.Count);
			elements.CopyTo(array, 0);
			return array;
		}
		static object GetFirstElementInMultipleValue(object value, Type type) {
			var arrayListValue = value as ArrayList;
			if(arrayListValue == null)
				return value;
			if(arrayListValue.Count > 0)
				return arrayListValue[0];
			return type.GetConstructor(Type.EmptyTypes) != null ? Activator.CreateInstance(type) : null;
		}
	}
}
