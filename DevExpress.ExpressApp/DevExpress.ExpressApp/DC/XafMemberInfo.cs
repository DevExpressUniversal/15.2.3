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
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.DC {
	public class XafMemberInfo : BaseInfo, IMemberInfo {
		private readonly String name;
		private TypeInfo owner;
		private Type memberType;
		private TypeInfo memberTypeInfo;
		private Boolean isPublic;
		private Boolean isVisible;
		private Boolean isReadOnly;
		private Boolean isKey;
		private Boolean isAutoGenerate;
		private Boolean isService;
		private Boolean isProperty;
		private Boolean isCustom;
		private Boolean isDelayed;
		private Boolean isAliased;
		private Boolean isPersistent;
		private Boolean isAssociation;
		private XafMemberInfo associatedMemberInfo;
		private Type associatedMemberOwner;
		private String associatedMemberName;
		private Boolean isList;
		private TypeInfo listElementTypeInfo;
		private Type listElementType;
		private String displayName;
		private Int32 size;
		private Int32 valueMaxLength;
		private Boolean isAggregated;
		private Boolean isReferenceToOwner;
		private Boolean isManyToMany;
		private String bindingName;
		private String expression;
		private Boolean skipRefreshOnAddAttribute;
		private ITypeInfoSource GetTypeInfoSource() {
			return (Source != null) ? Source : Owner.Source;
		}
		protected override void OnAddAttribute(Attribute attribute) {
			base.OnAddAttribute(attribute);
			if(!skipRefreshOnAddAttribute) {
				Refresh();
			}
		}
		public XafMemberInfo(String name, TypesInfo store)
			: base(store) {
			this.name = name;
			bindingName = name;
			isVisible = true;
		}
		public override string ToString() {
			string ownerName = "";
			if(Owner != null) {
				ownerName = Owner.ToString();
			}
			return ownerName + "-" + Name;
		}
		public override void Dispose() {
			base.Dispose();
			owner = null;
			memberTypeInfo = null;
			associatedMemberInfo = null;
			listElementTypeInfo = null;
		}
		public virtual void SetValue(Object obj, Object value) {
			if((obj != null) && (obj != DBNull.Value) && (owner != null) && owner.Type.IsAssignableFrom(obj.GetType())) {
				GetTypeInfoSource().SetValue(this, obj, value);
			}
		}
		public virtual Object GetValue(Object obj) {
			Object result = null;
			if((obj != null) && (obj != DBNull.Value) && (owner != null)) {
				if(owner.Type.IsAssignableFrom(obj.GetType())) {
					result = GetTypeInfoSource().GetValue(this, obj);
				}
				else if(obj is XafDataViewRecord) {
					XafDataViewRecord dataViewRecord = (XafDataViewRecord)obj;
					if(owner.Type.IsAssignableFrom(dataViewRecord.ObjectType) && dataViewRecord.ContainsMember(name)) {
						result = dataViewRecord[name];
					}
				}
			}
			return result;
		}
		public object GetOwnerInstance(object obj) {
			return obj;
		}
		public IList<IMemberInfo> GetPath() {
			return new IMemberInfo[] { this };
		}
		public string SerializeValue(object obj) {
			return Convert.ToString(GetValue(obj));
		}
		public object DeserializeValue(string value) {
			return ReflectionHelper.Convert(value, MemberType);
		}
		public void Refresh() {
			Source.RefreshMemberInfo(owner, this);
		}
		public void AddAttribute(Attribute attribute, Boolean skipRefresh) {
			skipRefreshOnAddAttribute = skipRefresh;
			base.AddAttribute(attribute);
		}
		public String Name {
			get { return name; }
		}
		public TypeInfo Owner {
			get { return owner; }
			set { owner = value; }
		}
		public Type MemberType {
			get { return memberType; }
			set { memberType = value; }
		}
		public TypeInfo MemberTypeInfo {
			get {
				if(memberTypeInfo == null) {
					memberTypeInfo = Store.FindTypeInfo(memberType);
				}
				return memberTypeInfo;
			}
		}
		public Boolean IsPublic {
			get { return isPublic; }
			set { isPublic = value; }
		}
		public Boolean IsProperty {
			get { return isProperty; }
			set { isProperty = value; }
		}
		public Boolean IsCustom {
			get { return isCustom; }
			set { isCustom = value; }
		}
		public Boolean IsVisible {
			get { return isVisible; }
			set { isVisible = value; }
		}
		public Boolean IsInStruct {
			get { return Name.Contains(".") || Owner.IsValueType; }
		}
		public Boolean IsReadOnly {
			get { return isReadOnly; }
			set { isReadOnly = value; }
		}
		public Boolean IsService {
			get { return isService; }
			set { isService = value; }
		}
		public Boolean IsKey {
			get { return isKey; }
			set { isKey = value; }
		}
		public Boolean IsAutoGenerate {
			get { return isAutoGenerate; }
			set { isAutoGenerate = value; }
		}
		public Boolean IsDelayed {
			get { return isDelayed; }
			set { isDelayed = value; }
		}
		public Boolean IsAliased {
			get { return isAliased; }
			set { isAliased = value; }
		}
		public Boolean IsPersistent {
			get { return isPersistent; }
			set { isPersistent = value; }
		}
		public Boolean IsAggregated {
			get { return isAggregated; }
			set { isAggregated = value; }
		}
		public Boolean IsAssociation {
			get { return isAssociation; }
			set { isAssociation = value; }
		}
		public Boolean IsReferenceToOwner {
			get { return isReferenceToOwner; }
			set { isReferenceToOwner = value; }
		}
		public XafMemberInfo AssociatedMemberInfo {
			get {
				if(associatedMemberInfo == null) {
					if(isAssociation) {
						TypeInfo ownerTypeInfo = Store.FindTypeInfo(associatedMemberOwner);
						associatedMemberInfo = ownerTypeInfo.FindMember(associatedMemberName);
					}
				}
				return associatedMemberInfo;
			}
			set { associatedMemberInfo = value; }
		}
		public Type AssociatedMemberOwner {
			get { return associatedMemberOwner; }
			set { associatedMemberOwner = value; }
		}
		public string AssociatedMemberName {
			get { return associatedMemberName; }
			set { associatedMemberName = value; }
		}
		public Boolean IsList {
			get { return isList; }
			set { isList = value; }
		}
		public TypeInfo ListElementTypeInfo {
			get {
				if(!isList) return null;
				if(listElementTypeInfo == null) {
					listElementTypeInfo = Store.FindTypeInfo(listElementType);
				}
				return listElementTypeInfo;
			}
		}
		public Type ListElementType {
			get { return listElementType; }
			set {
				listElementType = value;
				listElementTypeInfo = null;
			}
		}
		public String DisplayName {
			get { return displayName; }
			set { displayName = value; }
		}
		public Int32 Size {
			get { return size; }
			set { size = value; }
		}
		public Int32 ValueMaxLength {
			get { return valueMaxLength; }
			set { valueMaxLength = value; }
		}
		public bool IsManyToMany {
			get { return isManyToMany; }
			set { isManyToMany = value; }
		}
		public String BindingName {
			get { return bindingName; }
			set { bindingName = value; }
		}
		public String Expression {
			get { return expression; }
			set { expression = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Boolean HasValueConverter { get; set; }
		IMemberInfo IMemberInfo.AssociatedMemberInfo {
			get { return AssociatedMemberInfo; }
		}
		ITypeInfo IMemberInfo.ListElementTypeInfo {
			get { return ListElementTypeInfo; }
		}
		IMemberInfo IMemberInfo.LastMember {
			get { return this; }
		}
		ITypeInfo IMemberInfo.Owner {
			get { return Owner; }
		}
		ITypeInfo IMemberInfo.MemberTypeInfo {
			get { return MemberTypeInfo; }
		}
	}
}
