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
using System.Runtime.Serialization;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.DC {
	[Serializable]
	public class IntermediateMemberIsNullException : InvalidOperationException {
		protected IntermediateMemberIsNullException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public IntermediateMemberIsNullException(string memberName)
			: base(string.Format("Cannot set value to the \"{0}\" property because the intermediate property is null", memberName)) { }
	}
	public class MemberPathInfo : IMemberInfo {
		private List<IMemberInfo> members = new List<IMemberInfo>();
		private IMemberInfo lastMember = null;
		private ITypeInfo owner = null;
		private String bindingName;
		private String path;
		public bool IsSubclassOf(Type baseType, Type type) {
			if(type == null) {
				throw new ArgumentNullException("type");
			}
			for(Type type2 = type.BaseType; type2 != null; type2 = type2.BaseType) {
				if(type2 == baseType) {
					return true;
				}
			}
			return ((type == typeof(object)) && (type != baseType));
		}
		protected internal void AddMember(IMemberInfo member) {
			if(lastMember == null) {
				owner = member.Owner;
			}
			else {
				if(!(
					lastMember.MemberType.IsAssignableFrom(member.Owner.Type) ||
					member.Owner.Type.IsAssignableFrom(lastMember.MemberType) ||
					lastMember.MemberType == member.Owner.Type ||
					IsSubclassOf(lastMember.MemberType, member.Owner.Type) ||
					IsSubclassOf(member.Owner.Type, lastMember.MemberType))) {
					throw new ArgumentException("new member doesn't fit the path", "member");
				}
			}
			members.Add(member);
			lastMember = member;
		}
		public MemberPathInfo(string path) {
			this.path = path;
		}
		public override string ToString() {
			return Owner.Type.FullName + " - " + Name;
		}
		public void SetValue(Object obj, Object value) {
			if(obj == null || obj == DBNull.Value) {
				return;
			}
			Object instance = GetOwnerInstance(obj);
			if(instance != null) {
				LastMember.SetValue(instance, value);
			}
			else {
				throw new IntermediateMemberIsNullException(Name);
			}
		}
		public Object GetValue(Object obj) {
			if(obj == null || obj == DBNull.Value) {
				return null;
			}
			Object instance = GetOwnerInstance(obj);
			if(instance != null) {
				return LastMember.GetValue(instance);
			}
			else {
				return null;
			}
		}
		public object GetOwnerInstance(object obj) {
			if(LastMember != null) {
				object result = obj;
				for(int i = 0; i < members.Count - 1; i++) {
					result = members[i].GetValue(result);
				}
				return result;
			}
			return null;
		}
		public IList<IMemberInfo> GetPath() {
			return members.AsReadOnly();
		}
		public string SerializeValue(object obj) {
			return Convert.ToString(GetValue(obj));
		}
		public object DeserializeValue(string value) {
			return ReflectionHelper.Convert(value, MemberType);
		}
		public AttributeType FindAttribute<AttributeType>(bool recursive) where AttributeType : Attribute {
			return LastMember.FindAttribute<AttributeType>(recursive);
		}
		public IEnumerable<AttributeType> FindAttributes<AttributeType>(bool recursive) where AttributeType : Attribute {
			return LastMember.FindAttributes<AttributeType>(recursive);
		}
		public void AddAttribute(Attribute attribute, Boolean skipRefresh) {
			LastMember.AddAttribute(attribute, skipRefresh);
		}
		public IMemberInfo AssociatedMemberInfo {
			get {
				return (LastMember == null) ? null : LastMember.AssociatedMemberInfo;
			}
		}
		public string Name {
			get { return Path; }
		}
		public ITypeInfo Owner {
			get {
				return owner;
			}
		}
		public Type MemberType {
			get { return LastMember.MemberType; }
		}
		public bool IsPublic {
			get { return LastMember.IsPublic; }
		}
		public bool IsProperty {
			get { return LastMember.IsProperty; }
		}
		public Boolean IsCustom {
			get { return LastMember.IsCustom; }
		}
		public bool IsVisible {
			get { return LastMember.IsVisible; }
		}
		public bool IsInStruct {
			get { return LastMember.IsInStruct; }
		}
		public bool IsReadOnly {
			get { return LastMember.IsReadOnly; }
		}
		public bool IsKey {
			get { return LastMember.IsKey; }
		}
		public bool IsAutoGenerate {
			get { return LastMember.IsAutoGenerate; }
		}
		public bool IsDelayed {
			get { return LastMember.IsDelayed; }
		}
		public bool IsAliased {
			get { return LastMember.IsAliased; }
		}
		public bool IsService {
			get { return LastMember.IsService; }
		}
		public bool IsPersistent {
			get {
				foreach(IMemberInfo member in members) {
					if(!member.IsPersistent) {
						return false;
					}
				}
				return true;
			}
		}
		public bool IsAggregated {
			get { return LastMember.IsAggregated; }
		}
		public bool IsAssociation {
			get { return LastMember.IsAssociation; }
		}
		public bool IsManyToMany {
			get { return LastMember.IsManyToMany; }
		}
		public bool IsReferenceToOwner {
			get { return LastMember.IsReferenceToOwner; }
		}
		public bool IsList {
			get { return LastMember.IsList; }
		}
		public ITypeInfo ListElementTypeInfo {
			get { return LastMember.ListElementTypeInfo; }
		}
		public Type ListElementType {
			get { return LastMember.ListElementType; }
		}
		public ITypeInfo MemberTypeInfo {
			get { return LastMember.MemberTypeInfo; }
		}
		public string DisplayName {
			get { return LastMember.DisplayName; }
		}
		public int Size {
			get { return LastMember.Size; }
		}
		public int ValueMaxLength {
			get { return LastMember.ValueMaxLength; }
		}
		public String BindingName {
			get {
				if(bindingName == null) {
					bindingName = Name;
					if(!bindingName.EndsWith(LastMember.BindingName)) {
						int lastMemberNameIndex = bindingName.LastIndexOf(LastMember.Name);
						if(lastMemberNameIndex != -1) {
							bindingName = bindingName.Remove(lastMemberNameIndex) + LastMember.BindingName;
						}
					}
				}
				return bindingName;
			}
		}
		public String Expression {
			get { return LastMember.Expression; }
		}
		public IMemberInfo LastMember {
			get { return lastMember; }
		}
		public string Path {
			get {
				if(path == "") {
					foreach(IMemberInfo member in members) {
						path += member.Name + ".";
					}
					path = path.TrimEnd('.');
				}
				return path;
			}
		}
		#region IBaseInfo Members
		public void AddAttribute(Attribute attribute) { }
		public AttributeType FindAttribute<AttributeType>() where AttributeType : Attribute {
			return LastMember.FindAttribute<AttributeType>();
		}
		public IEnumerable<AttributeType> FindAttributes<AttributeType>() where AttributeType : Attribute {
			return LastMember.FindAttributes<AttributeType>();
		}
		public T GetExtender<T>() {
			return LastMember.GetExtender<T>();
		}
		public void AddExtender<T>(T extender) {
			if(LastMember != null) {
				LastMember.AddExtender<T>(extender);
			}
		}
		#endregion
		#region Obsolete 12.2
		[Obsolete("Static members are not taken into account in the TypesInfo subsystem anymore.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsStatic { get { throw new NotSupportedException(); } }
		#endregion
	}
}
