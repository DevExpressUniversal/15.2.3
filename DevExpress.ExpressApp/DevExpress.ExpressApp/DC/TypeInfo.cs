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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.DC {
	[Serializable]
	public class ObjectCreatingException : InvalidOperationException {
		[Serializable]
		private struct ObjectCreatingExceptionState : ISafeSerializationData {
			public Type ObjectType { get; set; }
			void ISafeSerializationData.CompleteDeserialization(Object obj) {
				ObjectCreatingException exception = (ObjectCreatingException)obj;
				exception.state = this;
			}
		}
		[NonSerialized]
		private ObjectCreatingExceptionState state = new ObjectCreatingExceptionState();
		public ObjectCreatingException() { }
		public ObjectCreatingException(Type objectType, string message, Exception innerException)
			: base(message, innerException) {
			state.ObjectType = objectType;
			SerializeObjectState += (exception, eventArgs) => eventArgs.AddSerializedState(state);
		}
		public Type ObjectType {
			get { return state.ObjectType; }
		}
	}
	public class TypeInfo : BaseInfo, ITypeInfo {
		private const string defaultPropertyAttributeNotFind = @"The ""{0}"" type declared with the DefaultProperty value of ""{1}"" doesn't contain this property or the property is declared as private";
		private readonly Type type;
		private readonly Dictionary<String, MemberPathInfo> pathsHash;
		private readonly Dictionary<TypeInfo, Object> dependentTypes;
		private Type underlyingType;
		private Boolean isAbstract;
		private Boolean isNullable;
		private Boolean isInterface;
		private Boolean isVisible;
		private readonly Boolean isValueType;
		private Boolean isDomainComponent;
		private Boolean isListType;
		private Boolean isPersistent;
		private Boolean hasPublicConstructor;
		private List<IMemberInfo> keyMembers;
		private DcAssemblyInfo assemblyInfo;
		private IMemberInfo referenceToOwner;
		private Boolean isDeclaredDefaultMemberInitialized;
		private IMemberInfo declaredDefaultMember;
		private Boolean isDefaultMemberInitialized;
		private IMemberInfo defaultMember;
		private void DisposeOwnMembers() {
			foreach(XafMemberInfo member in ownMemberByName.Values) {
				member.Dispose();
			}
		}
		private IMemberInfo[] ownMembers__ {
			get { return Enumerator.ToArray<IMemberInfo>(OwnMembers); }
		}
		private IMemberInfo[] members__ {
			get { return Enumerator.ToArray<IMemberInfo>(Members); }
		}
		private Boolean isBaseTypeInfoInitialized;
		private TypeInfo baseTypeInfo;
		private TypeInfo GetBaseTypeInfo() {
			if(!isBaseTypeInfoInitialized) {
				isBaseTypeInfoInitialized = true;
				Type baseType = type.BaseType;
				if(baseType != null) {
					baseTypeInfo = Store.FindTypeInfo(baseType);
				}
			}
			return baseTypeInfo;
		}
		private Boolean isImplementedInterfacesInitialized;
		private readonly Dictionary<TypeInfo, ITypeInfo> implementedInterfaces;
		private List<TypeInfo> sortedImplementedInterfaces;
		private Dictionary<TypeInfo, ITypeInfo> GetImplementedInterfaces() {
			if(!isImplementedInterfacesInitialized) {
				isImplementedInterfacesInitialized = true;
				InitializeImplementedInterfaces();
			}
			return implementedInterfaces;
		}
		private void InitializeImplementedInterfaces() {
			Type[] interfaces = TypeHelper.GetInterfaces(type);
			foreach(Type interfaceType in interfaces) {
				AddImplementedInterface(interfaceType);
				AddGenericTypeDefinition(interfaceType);
			}
			if(type.IsInterface) {
				AddGenericTypeDefinition(type);
			}
		}
		private void AddImplementedInterface(Type interfaceType) {
			TypeInfo interfaceInfo = Store.FindTypeInfo(interfaceType);
			if(!implementedInterfaces.ContainsKey(interfaceInfo)) {
				implementedInterfaces.Add(interfaceInfo, interfaceInfo);
			}
		}
		private void AddGenericTypeDefinition(Type interfaceType) {
			if(interfaceType.IsGenericType && !interfaceType.IsGenericTypeDefinition) {
				Type genericTypeDefinition = interfaceType.GetGenericTypeDefinition();
				AddImplementedInterface(genericTypeDefinition);
			}
		}
		private IEnumerable<TypeInfo> GetSortedDomainComponentFirst() {
			if(sortedImplementedInterfaces == null) {
				sortedImplementedInterfaces = new List<TypeInfo>();
				List<TypeInfo> notDomainComponents = new List<TypeInfo>();
				foreach(TypeInfo typeInfo in ImplementedInterfaces) {
					if(typeInfo.FindAttribute<DomainComponentAttribute>() != null) {
						sortedImplementedInterfaces.Add(typeInfo);
					}
					else {
						notDomainComponents.Add(typeInfo);
					}
				}
				sortedImplementedInterfaces.AddRange(notDomainComponents);
			}
			return sortedImplementedInterfaces;
		}
		private Boolean isUnderlyingTypeInfoInitialized;
		private TypeInfo underlyingTypeInfo;
		private TypeInfo GetUnderlyingTypeInfo() {
			if(!isUnderlyingTypeInfoInitialized) {
				isUnderlyingTypeInfoInitialized = true;
				if(IsNullable) {
					underlyingTypeInfo = Store.FindTypeInfo(UnderlyingType);
				}
				else {
					underlyingTypeInfo = this;
				}
			}
			return underlyingTypeInfo;
		}
		private Boolean isOwnMembersInitialized;
		private readonly Dictionary<String, XafMemberInfo> ownMemberByName;
		private void EnsureOwnMembersIsInitialized() {
			if(!isOwnMembersInitialized) {
				lock(TypesInfo.lockObject) {
					if(!isOwnMembersInitialized) {
						isOwnMembersInitialized = true;
						RefreshMembers();
					}
				}
			}
		}
		private IEnumerable<XafMemberInfo> GetOwnMembersCore() {
			EnsureOwnMembersIsInitialized();
			return ownMemberByName.Values;
		}
		private XafMemberInfo FindOwnMemberCore(String name) {
			EnsureOwnMembersIsInitialized();
			XafMemberInfo result;
			if(ownMemberByName.TryGetValue(name, out result)) {
				return result;
			}
			return null;
		}
		private void AddOwnMemberCore(XafMemberInfo newMember) {
			isRequiredTypesInitialized = false;
			newMember.Owner = this;
			ownMemberByName.Add(newMember.Name, newMember);
		}
		private IMemberInfo FindDeclaredDefaultMember() {
			List<String> potentialDefaultPropertyNames = new List<String>();
			foreach(XafDefaultPropertyAttribute attribute in FindAttributes<XafDefaultPropertyAttribute>(true)) {
				potentialDefaultPropertyNames.Add(attribute.Name);
			}
			foreach(DefaultPropertyAttribute attribute in FindAttributes<DefaultPropertyAttribute>(true)) {
				potentialDefaultPropertyNames.Add(attribute.Name);
			}
			IMemberInfo result = null;
			foreach(String propertyName in potentialDefaultPropertyNames) {
				IMemberInfo memberInfo = ((ITypeInfo)this).FindMember(propertyName);
				if(memberInfo == null) {
					Tracing.Tracer.LogWarning(String.Format(defaultPropertyAttributeNotFind, FullName, propertyName));
				}
				else if(memberInfo.IsPublic && memberInfo.IsProperty && memberInfo.IsVisible) {
					result = memberInfo;
					break;
				}
			}
			return result;
		}
		private IMemberInfo CalcDefaultMember() {
			IMemberInfo result = DeclaredDefaultMember;
			if(result != null) {
				return result;
			}
			if(!Type.IsValueType) {
				result = FindMember("Name");
				if(result != null && result.IsPublic && result.IsProperty && result.IsVisible) {
					return result;
				}
				foreach(IMemberInfo member in OwnMembers) {
					if(member.IsPublic && member.IsProperty && member.IsVisible && member.Name.Contains("Name")) {
						return member;
					}
				}
			}
			FriendlyKeyPropertyAttribute fkAttr = FindAttribute<FriendlyKeyPropertyAttribute>();
			if(fkAttr != null) {
				result = FindMember(fkAttr.MemberName);
				if(result != null && result.IsPublic && result.IsProperty && result.IsVisible) {
					return result;
				}
			}
			return null;
		}
		private Boolean isRequiredTypesInitialized;
		private readonly Dictionary<TypeInfo, Object> requiredTypes;
		private Dictionary<TypeInfo, Object> GetRequiredTypes() {
			if(!isRequiredTypesInitialized) {
				isRequiredTypesInitialized = true;
				InitializeRequiredTypes();
			}
			return requiredTypes;
		}
		private void InitializeRequiredTypes() {
			AddRequiredType(Base);
			AddRequiredType(GetUnderlyingTypeInfo());
			foreach(TypeInfo implementerInterface in ImplementedInterfaces) {
				AddRequiredType(implementerInterface);
			}
			foreach(XafMemberInfo ownMember in GetOwnMembersCore()) {
				TypeInfo requiredType;
				requiredType = ownMember.MemberTypeInfo;
				AddRequiredType(requiredType);
				requiredType = ownMember.ListElementTypeInfo;
				AddRequiredType(requiredType);
			}
		}
		private void AddRequiredType(TypeInfo requiredType) {
			if(requiredType == null || requiredType == this) return;
			if(!requiredTypes.ContainsKey(requiredType)) {
				requiredTypes.Add(requiredType, null);
			}
			if(!requiredType.dependentTypes.ContainsKey(this)) {
				requiredType.dependentTypes.Add(this, null);
			}
		}
		private void FillRequired(Dictionary<ITypeInfo, Object> result, Dictionary<ITypeInfo, Object> processed, Predicate<ITypeInfo> filter) {
			if(processed.ContainsKey(this)) return;
			IEnumerable<TypeInfo> filteredRequiredTypes = GetFilteredTypes(RequiredTypes, filter);
			AddUniqueTypes(result, filteredRequiredTypes);
			processed.Add(this, null);
			foreach(TypeInfo type in filteredRequiredTypes) {
				type.FillRequired(result, processed, filter);
			}
		}
		private void FillDependents(Dictionary<ITypeInfo, Object> result, Dictionary<ITypeInfo, Object> processed, Predicate<ITypeInfo> filter) {
			if(processed.ContainsKey(this)) return;
			IEnumerable<TypeInfo> filteredDependentTypes = GetFilteredTypes(DependentTypes, filter);
			AddUniqueTypes(result, filteredDependentTypes);
			processed.Add(this, null);
			foreach(TypeInfo type in filteredDependentTypes) {
				type.FillDependents(result, processed, filter);
			}
		}
		private IEnumerable<TypeInfo> GetFilteredTypes(IEnumerable<TypeInfo> source, Predicate<ITypeInfo> filter) {
			IEnumerable<TypeInfo> result;
			if(filter == null) {
				result = source;
			}
			else {
				List<TypeInfo> filteredTypes = new List<TypeInfo>();
				foreach(TypeInfo typeInfo in source) {
					if(filter(typeInfo)) {
						filteredTypes.Add(typeInfo);
					}
				}
				result = filteredTypes;
			}
			return result;
		}
		private void AddUniqueTypes(Dictionary<ITypeInfo, Object> target, IEnumerable<TypeInfo> source) {
			foreach(TypeInfo type in source) {
				if(!target.ContainsKey(type)) {
					target.Add(type, null);
				}
			}
		}
		private Boolean IsMemberExists(TypeInfo currentTypeInfo, String currentMemberName, out TypeInfo typeInfo, out String memberName) {
			typeInfo = null;
			memberName = null;
			if(!String.IsNullOrEmpty(currentMemberName)) {
				if(currentMemberName[0] == '<') {
					Int32 closingBracketPos = currentMemberName.IndexOf('>');
					if(closingBracketPos > 0) {
						String upcastTypeName = currentMemberName.Substring(1, closingBracketPos - 1);
						if(!String.IsNullOrEmpty(upcastTypeName)) {
							if(IsSuitableTypeInfo(currentTypeInfo, upcastTypeName)) {
								typeInfo = currentTypeInfo;
							}
							else {
								typeInfo = FindDescendantTypeInfo(currentTypeInfo, upcastTypeName);
								if(typeInfo == null) {
									typeInfo = FindBaseTypeInfo(currentTypeInfo, upcastTypeName);
								}
							}
						}
					}
					if(typeInfo != null) {
						memberName = currentMemberName.Substring(closingBracketPos + 1);
						return true;
					}
				}
				else {
					typeInfo = currentTypeInfo;
					memberName = currentMemberName;
					return true;
				}
			}
			return false;
		}
		private Boolean IsSuitableTypeInfo(ITypeInfo typeInfo, String typeString) {
			return typeInfo.Name == typeString;
		}
		private TypeInfo FindDescendantTypeInfo(TypeInfo typeInfo, String typeString) {
			foreach(TypeInfo descendant in typeInfo.Descendants) {
				if(IsSuitableTypeInfo(descendant, typeString)) {
					return descendant;
				}
			}
			return null;
		}
		private TypeInfo FindBaseTypeInfo(TypeInfo typeInfo, String typeString) {
			TypeInfo baseTypeInfo = typeInfo.Base;
			while(baseTypeInfo != null) {
				if(IsSuitableTypeInfo(baseTypeInfo, typeString)) {
					return baseTypeInfo;
				}
				baseTypeInfo = baseTypeInfo.Base;
			}
			return null;
		}
		protected internal void AddMember(XafMemberInfo member) {
			AddOwnMemberCore(member);
		}
		public TypeInfo(Type type, TypesInfo store)
			: base(store) {
			this.type = type;
			isValueType = type.IsValueType;
			ownMemberByName = new Dictionary<String, XafMemberInfo>();
			pathsHash = new Dictionary<String, MemberPathInfo>();
			implementedInterfaces = new Dictionary<TypeInfo, ITypeInfo>();
			requiredTypes = new Dictionary<TypeInfo, Object>();
			dependentTypes = new Dictionary<TypeInfo, Object>();
			keyMembers = new List<IMemberInfo>();
			isVisible = true;
		}
		public override string ToString() {
			return FullName;
		}
		public override void Dispose() {
			base.Dispose();
			keyMembers.Clear();
			keyMembers = null;
			baseTypeInfo = null;
			assemblyInfo = null;
			underlyingTypeInfo = null;
			referenceToOwner = null;
			declaredDefaultMember = null;
			defaultMember = null;
			DisposeOwnMembers();
			ownMemberByName.Clear();
			pathsHash.Clear();
			if(sortedImplementedInterfaces != null) {
				sortedImplementedInterfaces.Clear();
			}
			implementedInterfaces.Clear();
			dependentTypes.Clear();
			requiredTypes.Clear();
		}
		public XafMemberInfo FindMember(String name) {
			if(!String.IsNullOrEmpty(name)) {
				String memberName = name.TrimEnd('!');
				XafMemberInfo member = FindOwnMember(memberName);
				if(member != null) {
					return member;
				}
				if(Base != null) {
					return Base.FindMember(memberName);
				}
				if(type.IsInterface) {
					foreach(TypeInfo implementedInterfaceTypeInfo in GetSortedDomainComponentFirst()) {
						XafMemberInfo memberInfo = implementedInterfaceTypeInfo.FindMember(name);
						if(memberInfo != null) {
							return memberInfo;
						}
					}
				}
			}
			return null;
		}
		public IMemberInfo CreateMember(String memberName, Type memberType, String expression) {
			lock(TypesInfo.lockObject) {
				XafMemberInfo memberInfo = new XafMemberInfo(memberName, Store);
				memberInfo.MemberType = memberType;
				memberInfo.Expression = expression;
				memberInfo.IsCustom = true;
				if(Source.RegisterNewMember(this, memberInfo)) {
					memberInfo.Source = Source;
					AddMember(memberInfo);
					return memberInfo;
				}
				return null;
			}
		}
		public IMemberInfo CreateMember(String memberName, Type memberType) {
			return CreateMember(memberName, memberType, null);
		}
		public void UpdateMember(IMemberInfo memberInfo, Type type, String expression) {
			XafMemberInfo xafMemberInfo = (XafMemberInfo)memberInfo;
			xafMemberInfo.MemberType = type;
			xafMemberInfo.Expression = expression;
			xafMemberInfo.IsCustom = true;
			Source.UpdateMember(xafMemberInfo);
		}
		public void ClearMembers() {
			keyMembers.Clear();
			DisposeOwnMembers();
			ownMemberByName.Clear();
		}
		public bool CanInstantiate() {
			return isPersistent && !isAbstract && (hasPublicConstructor || type.IsInterface);
		}
		public XafMemberInfo FindOwnMember(String name) {
			return FindOwnMemberCore(name);
		}
		public void Refresh() {
			Source.InitTypeInfo(this);
		}
		protected override void EnsureAttributesCore() {
			Source.InitAttributes(this);
		}
		public void RefreshMembers() {
			Source.EnumMembers(this,
				delegate(Object member, String memberName) {
					XafMemberInfo memInfo = FindOwnMember(memberName);
					if(memInfo == null) {
						memInfo = new XafMemberInfo(memberName, Store);
						memInfo.Source = Source;
						AddMember(memInfo);
					}
					Source.InitMemberInfo(member, memInfo);
				}
			);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void AddKeyMember(IMemberInfo keyMember) {
			keyMembers.Add(keyMember);
		}
		public String FullName { get { return Type.FullName; } }
		public String Name { get { return Type.Name; } }
		public Type Type {
			get { return type; }
		}
		public TypeInfo Base {
			get { return GetBaseTypeInfo(); }
		}
		public Type UnderlyingType {
			get { return underlyingType; }
			set { underlyingType = value; }
		}
		public IEnumerable<IMemberInfo> OwnMembers {
			get { return Enumerator.Convert<IMemberInfo, XafMemberInfo>(GetOwnMembersCore()); }
		}
		public IEnumerable<IMemberInfo> Members {
			get {
				if(Base != null) {
					return Enumerator.Combine<IMemberInfo>(OwnMembers, Base.Members);
				}
				else {
					if(isInterface) {
						List<IMemberInfo> result = new List<IMemberInfo>(OwnMembers);
						foreach(TypeInfo implementedInterface in GetSortedDomainComponentFirst()) {
							foreach(IMemberInfo member in implementedInterface.OwnMembers) {
								if(member != implementedInterface.KeyMember) {
									Boolean resultContainsMember = false;
									foreach(IMemberInfo existsMember in result) {
										if(member.Name == existsMember.Name) {
											resultContainsMember = true;
											break;
										}
									}
									if(!resultContainsMember) {
										result.Add(member);
									}
								}
							}
						}
						return result;
					}
					return OwnMembers;
				}
			}
		}
		public Boolean IsNullable {
			get { return isNullable; }
			set { isNullable = value; }
		}
		public Boolean IsAbstract {
			get { return isAbstract; }
			set { isAbstract = value; }
		}
		public Boolean IsInterface {
			get { return isInterface; }
			set { isInterface = value; }
		}
		public Boolean IsVisible {
			get { return isVisible; }
			set { isVisible = value; }
		}
		public Boolean IsPersistent {
			get { return isPersistent; }
			set { isPersistent = value; }
		}
		public Boolean IsDomainComponent {
			get { return isDomainComponent; }
			set { isDomainComponent = value; }
		}
		public Boolean IsListType {
			get { return isListType; }
			set { isListType = value; }
		}
		public IMemberInfo KeyMember {
			get {
				if(keyMembers.Count > 0) {
					return keyMembers[0];
				}
				else if(Base != null) {
					return Base.KeyMember;
				}
				else {
					return null;
				}
			}
			set {
				keyMembers.Clear();
				if(value != null) {
					keyMembers.Add(value);
				}
			}
		}
		public ReadOnlyCollection<IMemberInfo> KeyMembers {
			get {
				ReadOnlyCollection<IMemberInfo> result = keyMembers.AsReadOnly();
				if((result.Count == 0) && (Base != null)) {
					result = Base.KeyMembers;
				}
				return result;
			}
		}
		public DcAssemblyInfo AssemblyInfo {
			get { return assemblyInfo; }
			set {
				assemblyInfo = value;
				if(assemblyInfo != null) {
					assemblyInfo.AddTypeInfo(this);
				}
			}
		}
		private ICollection<TypeInfo> descendants;
		public ICollection<TypeInfo> Descendants {
			get {
				if(IsInterface) {
					return Implementors;
				}
				Type[] localDescendants = Store.TypeHierarchyHelper.GetDescendants(type);
				if(descendants == null || descendants.Count != localDescendants.Length) {
					descendants = new List<TypeInfo>();
					foreach(Type descendant in localDescendants) {
						descendants.Add(Store.FindTypeInfo(descendant));
					}
				}
				return descendants;
			}
		}
		public Boolean HasDescendants {
			get { return Descendants.Count > 0; }
		}
		public Boolean HasPublicConstructor {
			get { return hasPublicConstructor; }
			set { hasPublicConstructor = value; }
		}
		IEnumerable<ITypeInfo> ITypeInfo.Descendants {
			get { return Enumerator.Convert<ITypeInfo, TypeInfo>(Descendants); }
		}
		ITypeInfo ITypeInfo.Base {
			get { return Base; }
		}
		ITypeInfo ITypeInfo.UnderlyingTypeInfo {
			get { return GetUnderlyingTypeInfo(); }
		}
		IMemberInfo ITypeInfo.FindMember(String name) {
			IMemberInfo result = null;
			if(!String.IsNullOrEmpty(name)) {
				result = FindMember(name);
				if(result == null) {
					result = FindMemberPath(name);
				}
			}
			return result;
		}
		IAssemblyInfo ITypeInfo.AssemblyInfo {
			get { return AssemblyInfo; }
		}
		public MemberPathInfo FindMemberPath(String path) {
			MemberPathInfo result = null;
			if(pathsHash.ContainsKey(path)) {
				result = pathsHash[path];
			}
			else {
				lock(TypesInfo.lockObject) {
					if(pathsHash.ContainsKey(path)) {
						result = pathsHash[path];
					}
					else {
						result = CreateMemberPath(path);
						if(result != null) {
							pathsHash[path] = result;
						}
					}
				}
			}
			return result;
		}
		public MemberPathInfo CreateMemberPath(String path) {
			MemberPathInfo result = new MemberPathInfo(path);
			String[] elements = EvaluatorProperty.Split(path);
			TypeInfo currentType = this;
			Int32 pathLength = elements.Length;
			Int32 i = 0;
			while(i < pathLength) {
				TypeInfo typeInfo;
				String memberName;
				if(IsMemberExists(currentType, elements[i], out typeInfo, out memberName)) {
					XafMemberInfo memberInfo = typeInfo.FindMember(memberName);
					while(memberInfo != null && i < pathLength - 1) {
						if(memberInfo.MemberType == null) {
							string message = string.Format("memberInfo.MemberType == null for the {0} member of the {1} type.", memberInfo.Name, typeInfo.FullName);
							throw new InvalidOperationException(message);
						}
						if(!memberInfo.MemberType.IsValueType) {
							break;
						}
						String structMemberName = memberName + "." + elements[i + 1];
						XafMemberInfo structMemberInfo = typeInfo.FindMember(structMemberName);
						if(structMemberInfo != null) {
							memberName = structMemberName;
							memberInfo = structMemberInfo;
							i++;
						}
						else {
							break;
						}
					}
					if(memberInfo != null) {
						result.AddMember(memberInfo);
						currentType = memberInfo.MemberTypeInfo;
						i++;
					}
					else {
						result = null;
						break;
					}
				}
				else {
					result = null;
					break;
				}
			}
			return result;
		}
		public ICollection<TypeInfo> ImplementedInterfaces {
			get { return GetImplementedInterfaces().Keys; }
		}
		IEnumerable<ITypeInfo> ITypeInfo.ImplementedInterfaces {
			get { return GetImplementedInterfaces().Values; }
		}
		private ICollection<TypeInfo> implementors;
		public ICollection<TypeInfo> Implementors {
			get {
				lock (TypesInfo.lockObject) {
					Type[] localImplementors;
					if(type.IsInterface && type.IsGenericTypeDefinition) {
						localImplementors = Store.TypeHierarchyHelper.GetTypesByGenericTypeDefinition(type);
					}
					else {
						localImplementors = Store.TypeHierarchyHelper.GetImplementors(type);
					}
					if(implementors == null || implementors.Count != localImplementors.Length) {
						implementors = new List<TypeInfo>();
						foreach (Type implementor in localImplementors) {
							implementors.Add(Store.FindTypeInfo(implementor));
						}
					}
					return implementors;
				}
			}
		}
		public IEnumerable<TypeInfo> RequiredTypes {
			get { return GetRequiredTypes().Keys; }
		}
		public IEnumerable<TypeInfo> DependentTypes {
			get { return dependentTypes.Keys; }
		}
		IEnumerable<ITypeInfo> ITypeInfo.Implementors {
			get { return Enumerator.Convert<ITypeInfo, TypeInfo>(Implementors); }
		}
		IEnumerable<ITypeInfo> ITypeInfo.GetDependentTypes(Predicate<ITypeInfo> filter) {
			Dictionary<ITypeInfo, Object> result = new Dictionary<ITypeInfo, Object>();
			Dictionary<ITypeInfo, Object> processed = new Dictionary<ITypeInfo, Object>();
			FillDependents(result, processed, filter);
			result.Remove(this);
			return result.Keys;
		}
		IEnumerable<ITypeInfo> ITypeInfo.GetRequiredTypes(Predicate<ITypeInfo> filter) {
			Dictionary<ITypeInfo, Object> result = new Dictionary<ITypeInfo, Object>();
			Dictionary<ITypeInfo, Object> processed = new Dictionary<ITypeInfo, Object>();
			lock(TypesInfo.lockObject) {
				FillRequired(result, processed, filter);
				result.Remove(this);
			}
			return result.Keys;
		}
		public IMemberInfo ReferenceToOwner {
			get { return referenceToOwner; }
			set { referenceToOwner = value; }
		}
		public IMemberInfo DeclaredDefaultMember {
			get {
				if(!isDeclaredDefaultMemberInitialized) {
					isDeclaredDefaultMemberInitialized = true;
					declaredDefaultMember = FindDeclaredDefaultMember();
				}
				return declaredDefaultMember;
			}
			set {
				isDeclaredDefaultMemberInitialized = true;
				declaredDefaultMember = value;
			}
		}
		public IMemberInfo DefaultMember {
			get {
				if (!isDefaultMemberInitialized) {
					lock (TypesInfo.lockObject) {
						if (!isDefaultMemberInitialized) {
							defaultMember = CalcDefaultMember();
							isDefaultMemberInitialized = true;
						}
					}
				}
				return defaultMember;
			}
			set {
				isDefaultMemberInitialized = true;
				defaultMember = value;
			}
		}
		public bool IsValueType {
			get { return isValueType; }
		}
		public bool IsAssignableFrom(ITypeInfo from) {
			if(from == null)
				return false;
			return Type.IsAssignableFrom(from.Type);
		}
		public bool AssignableFrom<T>() {
			return Type.IsAssignableFrom(typeof(T));
		}
		public bool Implements<InterfaceType>() {
			return typeof(InterfaceType).IsAssignableFrom(Type);
		}
		public object CreateInstance(params object[] args) {
			return TypeHelper.CreateInstance(type, args);
		}
	}
}
