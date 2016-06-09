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
using System.Collections.ObjectModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Filtering {
	public enum SearchMode { SearchInProperty, SearchInObject }
	public enum SearchMemberMode { Unknown, Include, Exclude }
	public class SearchStringParser {
		private List<string> entries;
		public SearchStringParser(string valueToSearch) {
			entries = new List<string>();
			if(!string.IsNullOrEmpty(valueToSearch)) {
				string[] strongPhrases = valueToSearch.Split('"');
				for(int i = 0; i < strongPhrases.Length; i++) {
					if((i & 1) == 1) {
						if(!string.IsNullOrEmpty(strongPhrases[i]))
							entries.Add(strongPhrases[i]);
					}
					else {
						string text = strongPhrases[i];
						bool lastChance = (i == strongPhrases.Length - 2);
						if(lastChance)
							text = text + "\"" + strongPhrases[i + 1];
						string[] words = text.Split(' ');
						foreach(string word in words) {
							if(!string.IsNullOrEmpty(word))
								entries.Add(word);
						}
						if(lastChance)
							break;
					}
				}
			}
			if(entries.Count == 0)
				entries.Add(valueToSearch);
		}
		public IList<string> Entries { get { return entries; } }
		public int Count { get { return entries.Count; } }
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
	public class SearchMemberOptionsAttribute : Attribute {
		public static SearchMemberMode IncludeMemberInCriteriaByDefault = SearchMemberMode.Include;
		private SearchMemberMode includeMemberInCriteria = IncludeMemberInCriteriaByDefault;
		public SearchMemberOptionsAttribute(SearchMemberMode useMemberInCriteria) {
			this.includeMemberInCriteria = useMemberInCriteria;
		}
		internal static SearchMemberMode GetSearchMemberMode(IMemberInfo memberInfo) {
			if(memberInfo != null) {
				SearchMemberOptionsAttribute attr = memberInfo.FindAttribute<SearchMemberOptionsAttribute>();
				if(attr != null) {
					return attr.IncludeMemberInCriteria;
				}
			}
			return SearchMemberMode.Unknown;
		}
		internal static SearchMemberMode GetSearchMemberMode(ITypeInfo typeInfo) {
			if(typeInfo != null) {
				SearchClassOptionsAttribute classAttr = typeInfo.FindAttribute<SearchClassOptionsAttribute>(true);
				if(classAttr != null) {
					return classAttr.IncludeMembersInCriteria;
				}
			}
			return SearchMemberMode.Unknown;
		}
		public SearchMemberMode IncludeMemberInCriteria {
			get { return includeMemberInCriteria; }
		}
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true)]
	public class SearchClassOptionsAttribute : Attribute {
		private SearchMemberMode includeMembersInCriteria;
		public SearchClassOptionsAttribute(SearchMemberMode includeMembersInCriteria) {
			this.includeMembersInCriteria = includeMembersInCriteria;
		}
		public SearchMemberMode IncludeMembersInCriteria {
			get { return includeMembersInCriteria; }
		}
	}
	public interface ISearchCriteriaBuilder {
		CriteriaOperator BuildCriteria();
		void SetSearchProperties(IList<string> searchProperties);
		ITypeInfo TypeInfo { get; set; }
		ReadOnlyCollection<string> SearchProperties { get; }
		string SearchText { get; set; }
		SearchMode SearchMode { get; set; }
		bool SearchInStringPropertiesOnly { get; set; }
		bool IncludeNonPersistentMembers { get; set; }
	}
	public class SearchCriteriaBuilderBase : ISearchCriteriaBuilder {
		public static SearchMode SearchModeDefault = SearchMode.SearchInObject;
		private ITypeInfo typeInfo;
		private string searchText;
		private List<string> searchProperties = new List<String>();
		private SearchMode searchMode = SearchModeDefault;
		private bool searchInStringPropertiesOnly = false;
		private GroupOperatorType valuesGroupOperatorType = GroupOperatorType.And; 
		private bool includeNonPersistentMembers = false; 
		public SearchCriteriaBuilderBase() { }
		public virtual CriteriaOperator BuildCriteria() {
			return null;
		}
		public void SetSearchProperties(IList<string> properties) {
			this.searchProperties.Clear();
			AddSearchProperties(properties);
		}
		public void AddSearchProperties(IList<string> properties) {
			if(properties != null) {
				foreach(string property in properties) {
					if(!this.searchProperties.Contains(property)) {
						this.searchProperties.Add(property);
					}
				}
			}
		}
		public ITypeInfo TypeInfo {
			get { return typeInfo; }
			set { typeInfo = value; }
		}
		public ReadOnlyCollection<string> SearchProperties {
			get { return new ReadOnlyCollection<string>(searchProperties); }
		}
		public string SearchText {
			get { return searchText; }
			set { searchText = value; }
		}
		public SearchMode SearchMode {
			get { return searchMode; }
			set { searchMode = value; }
		}
		public bool SearchInStringPropertiesOnly {
			get { return searchInStringPropertiesOnly; }
			set { searchInStringPropertiesOnly = value; }
		}
		public bool IncludeNonPersistentMembers {
			get { return includeNonPersistentMembers; }
			set { includeNonPersistentMembers = value; }
		}
		public GroupOperatorType ValuesGroupOperatorType {
			get { return valuesGroupOperatorType; }
			set { valuesGroupOperatorType = value; }
		}
	}
	public class SearchCriteriaBuilder : SearchCriteriaBuilderBase {
		protected static void AddChildOperandSmart(GroupOperator mainGroupOperator, CriteriaOperator childOperator) {
			CriteriaOperator child = childOperator;
			if(childOperator is GroupOperator) {
				child = GetSimplifiedCriteria((GroupOperator)childOperator);
			}
			if(!object.ReferenceEquals(null, child)) {
				mainGroupOperator.Operands.Add(child);
			}
		}
		protected static CriteriaOperator GetSimplifiedCriteria(GroupOperator groupOperator) {
			if(groupOperator.Operands.Count > 1) {
				return groupOperator;
			}
			else if(groupOperator.Operands.Count == 1) {
				return groupOperator.Operands[0];
			}
			return null;
		}
		protected Boolean IsMemberPersistent(IMemberInfo memberInfo) {
			return (memberInfo.IsPersistent || memberInfo.IsAliased);
		}
		protected List<IMemberInfo> GetActualSearchProperties() {
			List<IMemberInfo> result = new List<IMemberInfo>();
			foreach(string propertyName in SearchProperties) {
				IMemberInfo mi = GetActualSearchProperty(propertyName.Trim());
				if(mi != null) {
					result.Add(mi);
				}
			}
			return result;
		}
		protected void AutoFillSearchPropertiesRecursive(string path, ITypeInfo typeInfo) {
			foreach(IMemberInfo memberInfo in typeInfo.Members) {
				string fullMemberName =
					!string.IsNullOrEmpty(path) ? (path + '.' + memberInfo.Name) : memberInfo.Name;
				if(TypeInfo.FindMember(fullMemberName) == null) {
					return;
				}
				if(memberInfo.IsAggregated) {
					bool isCycledRecurrence = false;
					foreach(IMemberInfo intermediateMemberInfo in TypeInfo.FindMember(fullMemberName).GetPath()) {
						if(intermediateMemberInfo.Owner.IsAssignableFrom(memberInfo.MemberTypeInfo)) {
							isCycledRecurrence = true;
							break;
						}
					}
					if(!isCycledRecurrence) {
						AutoFillSearchPropertiesRecursive(fullMemberName, memberInfo.MemberTypeInfo);
					}
				}
				IMemberInfo actualSearchMember = GetActualSearchProperty(fullMemberName);
				if(actualSearchMember != null) {
					AddSearchProperties(actualSearchMember.Name);
				}
			}
		}
		protected bool TryConvertStringTo(Type targetType, string valueToSearch, out object convertedValue) {
			convertedValue = null;
			if(valueToSearch == null) {
				return true;
			}
			string trimmedValue = valueToSearch.Trim();
			if(string.IsNullOrEmpty(trimmedValue)) {
				return true;
			}
			try {
				convertedValue = ReflectionHelper.Convert(trimmedValue, targetType);
				return true;
			}
			catch(InvalidCastException) { }
			catch(FormatException) { }
			catch(OverflowException) { }
			catch(ArgumentException) { }
			return false;
		}
		protected virtual bool AllowSearchForMember(IMemberInfo memberInfo) {
			Guard.ArgumentNotNull(memberInfo, "memberInfo");
			if(!memberInfo.Owner.IsAssignableFrom(TypeInfo)) {
				return false;
			}
			foreach(IMemberInfo currentMemberInfo in memberInfo.GetPath()) {
				if(SearchMemberOptionsAttribute.GetSearchMemberMode(currentMemberInfo) == SearchMemberMode.Include) {
					continue;
				}
				else if(SearchMemberOptionsAttribute.GetSearchMemberMode(currentMemberInfo) == SearchMemberMode.Exclude) {
					return false;
				}
				else if(!currentMemberInfo.IsPublic && !currentMemberInfo.IsPersistent) {
					return false;
				}
				else if(currentMemberInfo.IsService || !currentMemberInfo.IsVisible) {
					return false;
				}
				else if(currentMemberInfo is XafMemberInfo && ((XafMemberInfo)currentMemberInfo).HasValueConverter) {
					return false;
				}
				else if(SearchMemberOptionsAttribute.GetSearchMemberMode(currentMemberInfo.Owner) == SearchMemberMode.Exclude) {
					return false;
				}
				else if(SearchMemberOptionsAttribute.GetSearchMemberMode(currentMemberInfo.Owner) == SearchMemberMode.Include) {
					continue;
				}
				else if(!IncludeNonPersistentMembers && !IsMemberPersistent(currentMemberInfo)) {
					return false;
				}
				else if(SearchMemberOptionsAttribute.IncludeMemberInCriteriaByDefault == SearchMemberMode.Exclude) {
					return false;
				}
			}
			if(SearchInStringPropertiesOnly && memberInfo.MemberType != typeof(string)) {
				return false;
			}
			return true;
		}
		protected virtual IMemberInfo GetActualSearchProperty(string propertyName) {
			if(propertyName == "ObjectType" || propertyName.Contains("!")) {
				return null;
			}
			IMemberInfo result = TypeInfo.FindMember(propertyName);
			if(result == null) {
				return null;
			}
			if(result.LastMember == null) {
				return null;
			}
			if((result.LastMember.MemberTypeInfo != null) && result.LastMember.MemberTypeInfo.IsDomainComponent) {
				IMemberInfo defaultMember = result.LastMember.MemberTypeInfo.DefaultMember;
				if(defaultMember != null) {
					result = TypeInfo.FindMember(propertyName + "." + defaultMember.Name);
				}
				else {
					return null;
				}
			}
			if((result == null) || !AllowSearchForMember(result)) {
				return null;
			}
			return result;
		}
		protected virtual CriteriaOperator CreateCriteriaOperator(IMemberInfo memberInfo, string valueToSearch) {
			Guard.ArgumentNotNull(memberInfo, "memberInfo");
			if(string.IsNullOrEmpty(valueToSearch)) {
				return null;
			}
			string trimmedValueToSearch = valueToSearch.Trim();
			if(!string.IsNullOrEmpty(trimmedValueToSearch)) {
				if(memberInfo.MemberType == typeof(string)) {
					return new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty(memberInfo.Name), new OperandValue(trimmedValueToSearch));
				}
				else {
					object convertedValue = null;
					if(TryConvertStringTo(memberInfo.MemberType, trimmedValueToSearch, out convertedValue)) {
						return new BinaryOperator(memberInfo.Name, convertedValue, BinaryOperatorType.Equal);
					}
				}
				return null;
			}
			return null;
		}
		protected virtual IList<string> ParseSearchText() {
			SearchStringParser searchTextParser = new SearchStringParser(SearchText);
			return searchTextParser.Entries;
		}
		public SearchCriteriaBuilder() { }
		public SearchCriteriaBuilder(ITypeInfo typeInfo)
			: this(typeInfo, null, null, GroupOperatorType.And, false, SearchMode.SearchInObject) { }
		public SearchCriteriaBuilder(ITypeInfo typeInfo, ICollection<string> properties, string valueToSearch, GroupOperatorType valuesGroupOperatorType, bool includeNonPersistentMembers, SearchMode searchMode) {
			Guard.ArgumentNotNull(typeInfo, "typeInfo");
			this.TypeInfo = typeInfo;
			this.ValuesGroupOperatorType = valuesGroupOperatorType;
			this.SearchMode = searchMode;
			this.IncludeNonPersistentMembers = includeNonPersistentMembers;
			if(properties != null) {
				this.AddSearchProperties((IList<string>)properties);
			}
			this.SearchText = valueToSearch;
		}
		public SearchCriteriaBuilder(ITypeInfo typeInfo, ICollection<string> properties, string valueToSearch, GroupOperatorType valuesGroupOperatorType, bool includeNonPersistentMembers)
			: this(typeInfo, properties, valueToSearch, valuesGroupOperatorType, includeNonPersistentMembers, SearchMode.SearchInProperty) { }
		public SearchCriteriaBuilder(ITypeInfo typeInfo, ICollection<string> properties, string valueToSearch, GroupOperatorType valuesGroupOperatorType)
			: this(typeInfo, properties, valueToSearch, valuesGroupOperatorType, true, SearchMode.SearchInProperty) {
		}
		public override CriteriaOperator BuildCriteria() {
			IList<string> searchTextItems = ParseSearchText();
			if((searchTextItems.Count > 0) && (SearchProperties.Count > 0)) {
				List<IMemberInfo> actualSearchProperties = GetActualSearchProperties();
				GroupOperator rootCriteria = new GroupOperator();
				if(SearchMode == SearchMode.SearchInProperty) {
					rootCriteria.OperatorType = GroupOperatorType.Or;
					foreach(IMemberInfo searchMember in actualSearchProperties) {
						GroupOperator subCriteria = new GroupOperator();
						subCriteria.OperatorType = ValuesGroupOperatorType;
						foreach(string valueToSearch in searchTextItems) {
							AddChildOperandSmart(subCriteria, CreateCriteriaOperator(searchMember, valueToSearch));
						}
						AddChildOperandSmart(rootCriteria, subCriteria);
					}
				}
				else if(SearchMode == SearchMode.SearchInObject) {
					rootCriteria.OperatorType = GroupOperatorType.And;
					foreach(string valueToSearch in searchTextItems) {
						GroupOperator subCriteria = new GroupOperator();
						subCriteria.OperatorType = GroupOperatorType.Or;
						foreach(IMemberInfo searchMember in actualSearchProperties) {
							AddChildOperandSmart(subCriteria, CreateCriteriaOperator(searchMember, valueToSearch));
						}
						AddChildOperandSmart(rootCriteria, subCriteria);
					}
				}
				else {
					throw new ArgumentException("Unknown value: '" + SearchMode.ToString() + "'", "searchMode");
				}
				return GetSimplifiedCriteria(rootCriteria);
			}
			return null;
		}
		public void AddSearchProperties(params string[] properties) {
			AddSearchProperties((IList<string>)properties);
		}
		public void SetSearchProperties(params string[] properties) {
			SetSearchProperties((IList<string>)properties);
		}
		public void FillSearchProperties() {
			AutoFillSearchPropertiesRecursive(null, TypeInfo);
		}
	}
}
