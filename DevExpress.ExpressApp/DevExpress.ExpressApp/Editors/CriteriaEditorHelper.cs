#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors.Filtering;
namespace DevExpress.ExpressApp.Editors {
	[AttributeUsage(AttributeTargets.Property)]
	public class CriteriaOptionsAttribute : Attribute {
		public CriteriaOptionsAttribute(string objectTypeMemberName, string parametersMemberName) {
			this.ObjectTypeMemberName = objectTypeMemberName;
			this.ParametersMemberName = parametersMemberName;
		}
		public CriteriaOptionsAttribute(string objectTypeMemberName) : this(objectTypeMemberName, null) { }
		public string ObjectTypeMemberName { get; set; }
		public string ParametersMemberName { get; set; }
	}
	public class XafFilterParameter : IFilterParameter {
		string name;
		Type type;
		public XafFilterParameter(string name, Type type) {
			this.name = name;
			this.type = type;
		}
		public string Name { get { return name; } }
		public Type Type { get { return type; } }
	}
	public class CriteriaPropertyEditorHelper {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static IList<Type> IgnoredMemberTypes = new List<Type>() { typeof(Type) };
		private IMemberInfo criteriaObjectTypeMemberInfo;
		private IMemberInfo criteriaParametersMemberInfo;
		protected void Init(IMemberInfo criteriaObjectTypeMemberInfo, IMemberInfo criteriaParametersMemberInfo) {
			Guard.ArgumentNotNull(criteriaObjectTypeMemberInfo, "criteriaObjectTypeMemberInfo");
			this.criteriaObjectTypeMemberInfo = criteriaObjectTypeMemberInfo;
			Guard.TypeArgumentIs(typeof(Type), criteriaObjectTypeMemberInfo.MemberType, "CriteriaOptionsAttribute.ObjectTypeMemberName");
			this.criteriaParametersMemberInfo = criteriaParametersMemberInfo;
			if(criteriaParametersMemberInfo != null) {
				Guard.TypeArgumentIs(typeof(IEnumerable<IFilterParameter>), criteriaParametersMemberInfo.MemberType, "CriteriaOptionsAttribute.ParametersMemberName");
			}
		}
		protected CriteriaPropertyEditorHelper() { }
		public CriteriaPropertyEditorHelper(IMemberInfo filterStringMemberInfo) {
			Guard.ArgumentNotNull(filterStringMemberInfo, "filterStringMemberInfo");
			IMemberInfo criteriaObjectTypeMemberInfo = null;
			IMemberInfo criteriaParametersMemberInfo = null;
#pragma warning disable 0618
			CriteriaObjectTypeMemberAttribute sourceAttribute = filterStringMemberInfo.FindAttribute<CriteriaObjectTypeMemberAttribute>();
#pragma warning restore 0618
			if(sourceAttribute != null) {
				string memberName = sourceAttribute.MemberName;
				if(filterStringMemberInfo.Name.Contains(".")) {
					memberName = filterStringMemberInfo.Name.Substring(0, filterStringMemberInfo.Name.LastIndexOf(".") + 1) + memberName;
				}
				criteriaObjectTypeMemberInfo = filterStringMemberInfo.Owner.FindMember(memberName);
				if(criteriaObjectTypeMemberInfo == null) {
					throw new ArgumentException(string.Format("Can't find the type of the filtered objects. The {0} member is absent.", sourceAttribute.MemberName));
				}
			}
			else {
				CriteriaOptionsAttribute criteriaAttribute = filterStringMemberInfo.FindAttribute<CriteriaOptionsAttribute>();
				Guard.ArgumentNotNull(criteriaAttribute, "CriteriaOptionsAttribute");
				Guard.ArgumentNotNullOrEmpty(criteriaAttribute.ObjectTypeMemberName, "CriteriaOptionsAttribute.ObjectTypeMemberName");
				string memberPrefix = "";
				if(filterStringMemberInfo.Name.Contains(".")) {
					memberPrefix = filterStringMemberInfo.Name.Substring(0, filterStringMemberInfo.Name.LastIndexOf(".") + 1);
				}
				string objectTypeMemberName = memberPrefix + criteriaAttribute.ObjectTypeMemberName;
				criteriaObjectTypeMemberInfo = filterStringMemberInfo.Owner.FindMember(objectTypeMemberName);
				if(criteriaObjectTypeMemberInfo == null) {
					throw new MemberNotFoundException(filterStringMemberInfo.Owner.Type, objectTypeMemberName);
				}
				if(!string.IsNullOrEmpty(criteriaAttribute.ParametersMemberName)) {
					string parametersMemberName = memberPrefix + criteriaAttribute.ParametersMemberName;
					criteriaParametersMemberInfo = filterStringMemberInfo.Owner.FindMember(parametersMemberName);
					if(criteriaParametersMemberInfo == null) {
						throw new MemberNotFoundException(filterStringMemberInfo.Owner.Type, parametersMemberName);
					}
				}
			}
			Init(criteriaObjectTypeMemberInfo, criteriaParametersMemberInfo);
		}
		public Type GetCriteriaObjectType(object currentObject) {
			if(currentObject != null && criteriaObjectTypeMemberInfo != null) {
				return (Type)criteriaObjectTypeMemberInfo.GetValue(currentObject);
			}
			return null;
		}
		public IEnumerable<IFilterParameter> GetParametersList(object currentObject) {
			if(currentObject != null && criteriaParametersMemberInfo != null) {
				return (IEnumerable<IFilterParameter>)criteriaParametersMemberInfo.GetValue(currentObject);
			}
			return null;
		}
		public static object CreateFilterControlDataSource(Type itemType, XafApplication application) {
			return CreateFilterControlDataSource(itemType, application, null);
		}
		public static object CreateFilterControlDataSource(Type itemType, XafApplication application, IEnumerable<IFilterParameter> parameters) {
			if(itemType != null) {
				IObjectSpace tempObjectSpace = (application != null && application.ObjectSpaceProvider != null) ? application.CreateObjectSpace(itemType) : null;
				return CreateFilterControlDataSourceInternal(itemType, tempObjectSpace, parameters);
			}
			else {
				return null;
			}
		}
		internal static object CreateFilterControlDataSourceInternal(Type itemType, IObjectSpace objectSpace, IEnumerable<IFilterParameter> parameters) {
			if(itemType != null) {
				IList list = null;
				if(objectSpace != null) {
					list = objectSpace.CreateCollection(itemType, CollectionSourceBase.EmptyCollectionCriteria);
				}
				else if(typeof(Type).IsAssignableFrom(itemType)) {
					list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType)); 
				}
				else {
					list = new ArrayList(); 
				}
				ProxyCollection result = new ProxyCollection(null, XafTypesInfo.Instance.FindTypeInfo(itemType), list);
				if(parameters != null) {
					foreach(IFilterParameter parameter in parameters) {
						((IFilterParametersOwner)result).Parameters.Add(parameter);
					}
				}
				return result;
			}
			else {
				return null;
			}
		}
		public object CreateFilterControlDataSource(object currentObject, XafApplication application) {
			return CreateFilterControlDataSource(GetCriteriaObjectType(currentObject), application, GetParametersList(currentObject));
		}
		public string ConvertFromOldFormat(string criteriaString, object currentObject, IObjectSpace objectSpace) {
			Type collectionType = GetCriteriaObjectType(currentObject);
			if(collectionType != null) {
				return CriteriaStringHelper.ConvertFromOldFormat(criteriaString, objectSpace, XafTypesInfo.Instance.FindTypeInfo(collectionType));
			}
			return criteriaString;
		}
		public IList<string> MasterProperties {
			get {
				return (criteriaObjectTypeMemberInfo == null) ? new string[0] : new string[] { criteriaObjectTypeMemberInfo.Name };
			}
		}
		public IMemberInfo CriteriaObjectTypeMemberInfo {
			get { return criteriaObjectTypeMemberInfo; }
		}
		public IMemberInfo CriteriaParametersMemberInfo {
			get { return criteriaParametersMemberInfo; }
		}
	}
	#region Obsolete 11.2
	[Obsolete(ObsoleteMessages.ClassForInternalUseOnly)]
	public abstract class FilterColumnCollectionHelperBase {
		private XafApplication application;
		private IObjectSpace objectSpace;
		private bool isRoot = true;
		public FilterColumnCollectionHelperBase(XafApplication application, IObjectSpace objectSpace) {
			this.application = application;
			this.objectSpace = objectSpace;
		}
		#region ICriteriaEditorHelper Members
		public XafApplication Application {
			get { return application; }
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
			set { objectSpace = value; }
		}
		public abstract ITypeInfo FilteredTypeInfo {
			get;
		}
		public virtual bool IsRoot { get { return isRoot; } protected set { isRoot = value; } }
		public virtual bool UseColumnBindingName { get { return false; } }
		public string GetDefaultColumnFieldName() {
			if(FilteredTypeInfo != null && FilteredTypeInfo.DefaultMember != null) {
				return FilteredTypeInfo.DefaultMember.Name;
			}
			return string.Empty;
		}
		public IModelColumn CreateColumnInfoNode(string propertyName, out ITypeInfo typeInfo) {
			typeInfo = null;
			if(FilteredTypeInfo == null || application == null) return null;
			Guard.ArgumentNotNull(propertyName, "propertyName");
			IModelViews viewsNode = ((ModelNode)application.Model.Views).CreateNode() as IModelViews;
			typeInfo = GetFilteredTypeInfo(ref propertyName);
			IModelListView newListViewModel = viewsNode.AddNode<IModelListView>(propertyName);
			newListViewModel.ModelClass = application.Model.BOModel.GetClass(typeInfo.Type);
			IModelColumns modelColumns = newListViewModel.AddNode<IModelColumns>();
			IModelColumn columnNode = modelColumns.AddNode<IModelColumn>();
			columnNode.PropertyName = propertyName;
			columnNode.Caption = string.Empty;
			return columnNode;
		}
		ITypeInfo GetFilteredTypeInfo(ref string propertyName) {
			ITypeInfo typeInfo = FilteredTypeInfo;
			string[] properties = propertyName.Split('.');
			ITypeInfo memberTypeInfo = null;
			if(properties.Length > 1) {
				bool isList = false;
				memberTypeInfo = typeInfo;
				for(int i = 0; i < properties.Length - 1; i++) {
					IMemberInfo memberInfo = memberTypeInfo.FindMember(properties[i]);
					if(memberInfo == null) break;
					isList = isList || memberInfo.IsList;
					memberTypeInfo = memberInfo.IsList ? memberInfo.ListElementTypeInfo : memberInfo.MemberTypeInfo;
				}
				if(!isList) {
					memberTypeInfo = null;
				}
			}
			if(memberTypeInfo != null) {
				typeInfo = memberTypeInfo;
				propertyName = properties[properties.Length - 1];
			}
			return typeInfo;
		}
		public IModelColumn CreateColumnInfoNode(IMemberInfo memberInfo, out ITypeInfo typeInfo) {
			Guard.ArgumentNotNull(memberInfo, "memberInfo");
			return CreateColumnInfoNode(memberInfo.Name, out typeInfo);
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			if(objectSpace != null) {
				objectSpace = null;
			}
			if(application != null) {
				application = null;
			}
		}
		#endregion
	}
	[Obsolete(ObsoleteMessages.ClassForInternalUseOnly)]
	public class FilterColumnCollectionHelper : FilterColumnCollectionHelperBase {
		private ITypeInfo filteredTypeInfo;
		public FilterColumnCollectionHelper(XafApplication application, IObjectSpace objectSpace, ITypeInfo filteredTypeInfo)
			: base(application, objectSpace) {
			this.filteredTypeInfo = filteredTypeInfo;
		}
		public override ITypeInfo FilteredTypeInfo { get { return filteredTypeInfo; } }
		public override bool UseColumnBindingName { get { return true; } }
	}
	[Obsolete(ObsoleteMessages.ClassForInternalUseOnly)]
	public class CriteriaEditorWithoutApplicationHelper : FilterColumnCollectionHelperBase {
		ITypeInfo filteredTypeInfo;
		public CriteriaEditorWithoutApplicationHelper(ITypeInfo filteredTypeInfo)
			: base(null, null) {
			this.filteredTypeInfo = filteredTypeInfo;
		}
		public override ITypeInfo FilteredTypeInfo {
			get { return filteredTypeInfo; }
		}
	}
	[AttributeUsage(AttributeTargets.Property)]
	[Obsolete("Use 'CriteriaOptionsAttribute' instead.")]
	public class CriteriaObjectTypeMemberAttribute : Attribute {
		private string memberName;
		public string MemberName {
			get { return memberName; }
			set { memberName = value; }
		}
		public CriteriaObjectTypeMemberAttribute(string memberName) {
			this.memberName = memberName;
		}
	}
	#endregion
#pragma warning disable 0618
	public class CriteriaEditorHelper : FilterColumnCollectionHelperBase {
#pragma warning restore 0618
		private ITypeInfo objectTypeInfo;
		private IMemberInfo criteriaStringMemberInfo;
		private CriteriaPropertyEditorHelper criteriaPropertyEditorHelper;
		private object owner;
		public CriteriaEditorHelper(XafApplication application, IObjectSpace objectSpace, IMemberInfo criteriaStringMemberInfo, ITypeInfo objectTypeInfo)
			: base(application, objectSpace) {
			this.objectTypeInfo = objectTypeInfo;
			this.criteriaStringMemberInfo = criteriaStringMemberInfo;
			this.criteriaPropertyEditorHelper = new CriteriaPropertyEditorHelper(criteriaStringMemberInfo);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string ConvertFromOldFormat(string criteriaString, object currentObject) {
			return criteriaPropertyEditorHelper.ConvertFromOldFormat(criteriaString, currentObject, ObjectSpace);
		}
		protected virtual ITypeInfo GetFilteredTypeInfo() {
			Type type = criteriaPropertyEditorHelper.GetCriteriaObjectType(Owner);
			return XafTypesInfo.Instance.FindTypeInfo(type);
		}
		public DataColumnInfo[] GetDataColumnInfos() {
			if(FilteredTypeInfo != null) {
				List<DataColumnInfo> result = new List<DataColumnInfo>();
				object dataSource = CriteriaPropertyEditorHelper.CreateFilterControlDataSourceInternal(criteriaPropertyEditorHelper.GetCriteriaObjectType(Owner), ObjectSpace, criteriaPropertyEditorHelper.GetParametersList(Owner));
				foreach(DataColumnInfo columnInfo in new MasterDetailHelper().GetDataColumnInfo(dataSource)) {
					if(columnInfo.Browsable && columnInfo.Visible && !columnInfo.Name.EndsWith("!")) {
						result.Add(columnInfo);
					}
				}
				return result.ToArray();
			}
			else {
				return new DataColumnInfo[0];
			}
		}
		public string GetLocalizedCriteriaMemberCaption() {
			return CaptionHelper.GetFullMemberCaption(ObjectTypeInfo, CriteriaStringMemberInfo.BindingName);
		}
		public void SetupObjectSpace(IObjectSpace objectSpace) {
			ObjectSpace = objectSpace;
		}
		public object Owner {
			get { return owner; }
			set { owner = value; }
		}
		public IMemberInfo CriteriaStringMemberInfo {
			get { return criteriaStringMemberInfo; }
		}
		public IMemberInfo CriteriaObjectTypeMemberInfo {
			get { return criteriaPropertyEditorHelper.CriteriaObjectTypeMemberInfo; }
		}
		public ITypeInfo ObjectTypeInfo {
			get { return objectTypeInfo; }
		}
		public override ITypeInfo FilteredTypeInfo {
			get { return GetFilteredTypeInfo(); }
		}
		public static LocalizedCriteriaWrapper GetCriteriaWrapper(CriteriaOperator criteria, Type objectsType, IObjectSpace objectSpace) {
			LocalizedCriteriaWrapper wrapper = new LocalizedCriteriaWrapper(objectsType, criteria);
			wrapper.Validate();
			wrapper.UpdateParametersValues();
			return wrapper;
		}
		public static LocalizedCriteriaWrapper GetCriteriaWrapper(String criteria, Type objectsType, IObjectSpace objectSpace) {
			string convertedCriteriaString = CriteriaStringHelper.ConvertFromOldFormat(criteria, objectSpace, XafTypesInfo.Instance.FindTypeInfo(objectsType));
			CriteriaOperator rowCriteria = null;
			if(objectSpace == null) {
				rowCriteria = CriteriaOperator.Parse(convertedCriteriaString);
			}
			else {
				rowCriteria = objectSpace.ParseCriteria(convertedCriteriaString);
			}
			return GetCriteriaWrapper(rowCriteria, objectsType, objectSpace);
		}
		public static CriteriaOperator GetCriteriaOperator(String criteria, Type objectsType, IObjectSpace objectSpace) {
			return GetCriteriaWrapper(criteria, objectsType, objectSpace).CriteriaOperator;
		}
	}
	public class CriteriaParameterListProvider {
		private Dictionary<Type, IList<String>> parameterByDataTypeMap = new Dictionary<Type, IList<String>>();
		public IList<string> GetParameterNamesByDataType(Type dataType) {
			IList<string> result = null;
			if(!parameterByDataTypeMap.TryGetValue(dataType, out result)) {
				result = new List<string>();
				foreach(string parameterName in ParametersFactory.GetRegisteredParameterNames()) {
					DevExpress.Persistent.Base.IParameter parameter = ParametersFactory.CreateParameter(parameterName);
					if((SimpleTypes.IsSimpleType(dataType) && (parameter.Type == dataType)) ||
						(SimpleTypes.IsClass(dataType) && parameter.Type.IsAssignableFrom(dataType))) {
						result.Add(parameterName);
					}
				}
				parameterByDataTypeMap[dataType] = result;
			}
			return result;
		}
	}
}
