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
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
namespace DevExpress.XtraScheduler.Native {
	#region DataFieldInfo
	public class DataFieldInfo {
		#region Fields
		string name;
		Type type;
		#endregion
		public DataFieldInfo(string name, Type type) {
			if (type == null)
				Exceptions.ThrowArgumentNullException("type");
			this.type = type;
			this.name = name;
		}
		#region Properties
		public string Name { get { return name; } }
		public Type Type { get { return type; } }
		public string TypeName { get { return type.Name; } }
		#endregion
	}
	#endregion
	#region DataFieldInfoCollection
	public class DataFieldInfoCollection : List<DataFieldInfo> {
	}
	#endregion
	#region MappingsTokenInfos
	public class MappingsTokenInfos : Dictionary<string, TokenInfoDictionary> {
		#region CreateSimpleTokenInfoDictionary
		protected internal TokenInfoDictionary CreateSimpleTokenInfoDictionary(string token) {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo info = new TokenInfo();
			result.Add(token, info);
			return result;
		}
		#endregion
	}
	#endregion
	#region AppointmentMappingsTokenInfos
	public class AppointmentMappingsTokenInfos : MappingsTokenInfos {
		public AppointmentMappingsTokenInfos() {
			Add("Id", CreateIdTokenInfoDictionary());
			Add("Start", CreateStartTokenInfoDictionary());
			Add("End", CreateEndTokenInfoDictionary());
			Add("AllDay", CreateAllDayTokenInfoDictionary());
			Add("Status", CreateStatusTokenInfoDictionary());
			Add("Label", CreateLabelTokenInfoDictionary());
			Add("Description", CreateDescriptionTokenInfoDictionary());
			Add("Location", CreateLocationTokenInfoDictionary());
			Add("RecurrenceInfo", CreateRecurrenceInfoTokenInfoDictionary());
			Add("ReminderInfo", CreateReminderInfoTokenInfoDictionary());
			Add("ResourceId", CreateResourceIdTokenInfoDictionary());
			Add("Subject", CreateSubjectTokenInfoDictionary());
			Add("Type", CreateTypeTokenInfoDictionary());
			Add("TimeZoneId", CreateTimeZoneIdTokenInfoDictionary());
			AddPercentCompleteTokenInfoDictionary();
		}
		protected internal virtual void AddPercentCompleteTokenInfoDictionary() {
			Add("PercentComplete", CreatePercentCompleteTokenInfoDictionary());
		}
		#region AppendAppointmentSynonymsAsAttendants (helper method)
		protected void AppendAppointmentSynonymsAsAttendants(TokenInfo info) {
			info.AddAttendant("event", 0);
			info.AddAttendant("appointment", 0);
			info.AddAttendant("apt", 0);
			info.AddAttendant("task", 0);
			info.AddAttendant("meeting", 0);
		}
		#endregion
		#region CreateIdTokenInfoDictionary
		protected internal TokenInfoDictionary CreateIdTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo info = new TokenInfo();
			info.AddSynonym("key", 0);
			AppendAppointmentSynonymsAsAttendants(info);
			result.Add("id", info);
			return result;
		}
		#endregion
		#region CreateStartTokenInfoDictionary
		protected internal TokenInfoDictionary CreateStartTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo info = new TokenInfo();
			info.AddSynonym("begin", 0);
			info.AddAttendant("time", 0);
			info.AddAttendant("date", 0);
			AppendAppointmentSynonymsAsAttendants(info);
			result.Add("start", info);
			return result;
		}
		#endregion
		#region CreateEndTokenInfoDictionary
		protected internal TokenInfoDictionary CreateEndTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo info = new TokenInfo();
			info.AddSynonym("finish", 0);
			info.AddSynonym("stop", 2);
			info.AddAttendant("time", 0);
			info.AddAttendant("date", 0);
			AppendAppointmentSynonymsAsAttendants(info);
			result.Add("end", info);
			return result;
		}
		#endregion
		#region CreateAllDayTokenInfoDictionary
		protected internal TokenInfoDictionary CreateAllDayTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo allInfo = new TokenInfo();
			allInfo.AddSynonym("entire", 0);
			allInfo.AddSynonym("whole", 0);
			allInfo.AddSynonym("full", 0);
			result.Add("all", allInfo);
			TokenInfo dayInfo = new TokenInfo();
			result.Add("day", dayInfo);
			return result;
		}
		#endregion
		#region CreateDescriptionTokenInfoDictionary
		protected internal TokenInfoDictionary CreateDescriptionTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo info = new TokenInfo();
			info.AddSynonym("details", 0);
			info.AddSynonym("detail", 1);
			info.AddSynonym("notes", 0);
			info.AddSynonym("note", 0);
			AppendAppointmentSynonymsAsAttendants(info);
			result.Add("description", info);
			return result;
		}
		#endregion
		#region CreateLocationTokenInfoDictionary
		protected internal TokenInfoDictionary CreateLocationTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo info = new TokenInfo();
			info.AddSynonym("city", 1);
			info.AddSynonym("country", 1);
			info.AddSynonym("room", 0);
			info.AddSynonym("office", 0);
			info.AddSynonym("apartment", 2);
			AppendAppointmentSynonymsAsAttendants(info);
			result.Add("location", info);
			return result;
		}
		#endregion
		#region CreateRecurrenceInfoTokenInfoDictionary
		protected internal TokenInfoDictionary CreateRecurrenceInfoTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo recurrenceInfo = new TokenInfo();
			recurrenceInfo.AddSynonym("reoccurrence", 0);
			recurrenceInfo.AddSynonym("repeat", 1);
			recurrenceInfo.AddSynonym("recourse", 1);
			recurrenceInfo.AddSynonym("repetition", 2);
			recurrenceInfo.AddAttendant("rule", 0);
			recurrenceInfo.AddAttendant("pattern", 0);
			recurrenceInfo.AddAttendant("settings", 1);
			recurrenceInfo.AddAttendant("properties", 2);
			recurrenceInfo.AddAttendant("parameters", 2);
			AppendAppointmentSynonymsAsAttendants(recurrenceInfo);
			result.Add("recurrence", recurrenceInfo);
			TokenInfo infoInfo = new TokenInfo();
			infoInfo.Cost = 3;
			infoInfo.AddSynonym("rule", 0);
			infoInfo.AddSynonym("pattern", 0);
			infoInfo.AddSynonym("settings", 1);
			infoInfo.AddSynonym("properties", 2);
			infoInfo.AddSynonym("parameters", 2);
			AppendAppointmentSynonymsAsAttendants(infoInfo);
			result.Add("info", infoInfo);
			return result;
		}
		#endregion
		#region CreateReminderInfoTokenInfoDictionary
		protected internal TokenInfoDictionary CreateReminderInfoTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo reminderInfo = new TokenInfo();
			reminderInfo.AddSynonym("alarm", 0);
			reminderInfo.AddSynonym("rule", 2);
			reminderInfo.AddAttendant("settings", 2);
			reminderInfo.AddAttendant("properties", 1);
			reminderInfo.AddAttendant("parameters", 1);
			AppendAppointmentSynonymsAsAttendants(reminderInfo);
			result.Add("reminder", reminderInfo);
			TokenInfo infoInfo = new TokenInfo();
			infoInfo.Cost = 3;
			infoInfo.AddSynonym("rule", 0);
			infoInfo.AddSynonym("settings", 1);
			infoInfo.AddSynonym("properties", 2);
			infoInfo.AddSynonym("parameters", 2);
			AppendAppointmentSynonymsAsAttendants(infoInfo);
			result.Add("info", infoInfo);
			return result;
		}
		#endregion
		#region CreateSubjectTokenInfoDictionary
		protected internal TokenInfoDictionary CreateSubjectTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo info = new TokenInfo();
			info.AddSynonym("text", 0);
			info.AddSynonym("caption", 1);
			info.AddSynonym("title", 2);
			info.AddSynonym("topic", 5);
			AppendAppointmentSynonymsAsAttendants(info);
			result.Add("subject", info);
			return result;
		}
		#endregion
		#region CreateTypeTokenInfoDictionary
		protected internal TokenInfoDictionary CreateTypeTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo info = new TokenInfo();
			info.AddSynonym("kind", 0);
			info.AddSynonym("sort", 1);
			AppendAppointmentSynonymsAsAttendants(info);
			result.Add("type", info);
			return result;
		}
		#endregion
		#region CreatePercentCompleteTokenInfoDictionary
		protected internal TokenInfoDictionary CreatePercentCompleteTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo percentInfo = new TokenInfo();
			percentInfo.AddSynonym("rate", 0);
			percentInfo.AddSynonym("percentage", 1);
			AppendAppointmentSynonymsAsAttendants(percentInfo);
			result.Add("percent", percentInfo);
			TokenInfo completeInfo = new TokenInfo();
			completeInfo.AddSynonym("completed", 0);
			completeInfo.AddSynonym("perfected", 0);
			AppendAppointmentSynonymsAsAttendants(completeInfo);
			result.Add("complete", completeInfo);
			return result;
		}
		#endregion
		#region CreateResourceIdTokenInfoDictionary
		protected internal TokenInfoDictionary CreateResourceIdTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo resourceInfo = new TokenInfo();
			resourceInfo.Cost = 5;
			resourceInfo.AddSynonym("car", 3);
			resourceInfo.AddSynonym("user", 3);
			resourceInfo.AddSynonym("employee", 3);
			resourceInfo.AddSynonym("customer", 3);
			resourceInfo.AddSynonym("client", 3);
			resourceInfo.AddSynonym("room", 3);
			resourceInfo.AddSynonym("office", 3);
			resourceInfo.AddSynonym("patient", 3);
			resourceInfo.AddSynonym("person", 3);
			resourceInfo.AddAttendant("key", 0);
			AppendAppointmentSynonymsAsAttendants(resourceInfo);
			result.Add("resource", resourceInfo);
			TokenInfo idInfo = new TokenInfo();
			idInfo.Cost = 3;
			idInfo.AddSynonym("key", 0);
			idInfo.AddAttendant("car", 3);
			idInfo.AddAttendant("user", 3);
			idInfo.AddAttendant("employee", 3);
			idInfo.AddAttendant("customer", 3);
			idInfo.AddAttendant("client", 3);
			idInfo.AddAttendant("room", 3);
			idInfo.AddAttendant("office", 3);
			idInfo.AddAttendant("patient", 3);
			idInfo.AddAttendant("person", 3);
			AppendAppointmentSynonymsAsAttendants(idInfo);
			result.Add("id", idInfo);
			return result;
		}
		#endregion
		#region CreateLabelTokenInfoDictionary
		protected internal TokenInfoDictionary CreateLabelTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo info = new TokenInfo();
			info.AddSynonym("category", 0);
			AppendAppointmentSynonymsAsAttendants(info);
			result.Add("label", info);
			return result;
		}
		#endregion
		#region CreateStatusTokenInfoDictionary
		protected internal TokenInfoDictionary CreateStatusTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo info = new TokenInfo();
			info.AddSynonym("category", 5);
			info.AddSynonym("show", 4);
			AppendAppointmentSynonymsAsAttendants(info);
			info.AddAttendant("busy", 3);
			result.Add("status", info);
			return result;
		}
		#endregion
		#region CreateTimeZoneIdTokenInfoDictionary
		TokenInfoDictionary CreateTimeZoneIdTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo time = new TokenInfo();
			time.AddSynonym("timezone", 0);
			time.AddSynonym("tzid", 1);
			time.AddSynonym("zid", 2);
			time.AddSynonym("zoneid", 3);
			time.AddSynonym("zone", 4);
			result.Add("timezoneid", time);
			return result;
		}
		#endregion
	}
	#endregion
	#region ResourceMappingsTokenInfos
	public class ResourceMappingsTokenInfos : MappingsTokenInfos {
		public ResourceMappingsTokenInfos() {
			AddIdTokenInfoDictionary();
			Add("Caption", CreateCaptionTokenInfoDictionary());
			Add("Image", CreateImageTokenInfoDictionary());
			Add("Color", CreateColorTokenInfoDictionary());
			AddParentIdTokenInfoDictionary();
		}
		protected internal virtual void AddIdTokenInfoDictionary() {
			Add("Id", CreateIdTokenInfoDictionary());
		}
		protected internal virtual void AddParentIdTokenInfoDictionary() {
			Add("ParentId", CreateParentIdTokenInfoDictionary());
		}
		#region AppendResourceSynonymsAsAttendants (helper method)
		protected internal void AppendResourceSynonymsAsAttendants(TokenInfo info) {
			info.AddAttendant("resource", 0);
			info.AddAttendant("person", 0);
			info.AddAttendant("employee", 0);
			info.AddAttendant("car", 0);
		}
		#endregion
		#region CreateIdTokenInfoDictionary
		protected internal virtual TokenInfoDictionary CreateIdTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo info = new TokenInfo();
			info.AddSynonym("oid", 0);
			AppendResourceSynonymsAsAttendants(info);
			result.Add("id", info);
			return result;
		}
		#endregion
		#region CreateCaptionTokenInfoDictionary
		protected internal TokenInfoDictionary CreateCaptionTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo info = new TokenInfo();
			info.AddSynonym("text", 0);
			info.AddSynonym("name", 0);
			info.AddSynonym("title", 0);
			AppendResourceSynonymsAsAttendants(info);
			result.Add("caption", info);
			return result;
		}
		#endregion
		#region CreateImageTokenInfoDictionary
		protected internal TokenInfoDictionary CreateImageTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo info = new TokenInfo();
			info.AddSynonym("picture", 0);
			info.AddSynonym("photo", 0);
			AppendResourceSynonymsAsAttendants(info);
			result.Add("image", info);
			return result;
		}
		#endregion
		#region CreateColorTokenInfoDictionary
		protected internal TokenInfoDictionary CreateColorTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo info = new TokenInfo();
			AppendResourceSynonymsAsAttendants(info);
			result.Add("color", info);
			return result;
		}
		#endregion
		#region CreateParentIdTokenInfoDictionary
		protected internal TokenInfoDictionary CreateParentIdTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo info = new TokenInfo();
			AppendResourceSynonymsAsAttendants(info);
			result.Add("parent", info);
			result.Add("parentId", info);
			result.Add("id", info);
			return result;
		}
		#endregion
	}
	#endregion
	#region AppointmentDependencyMappingsTokenInfos
	public class AppointmentDependencyMappingsTokenInfos : MappingsTokenInfos {
		public AppointmentDependencyMappingsTokenInfos() {
			Add("Type", CreateTypeTokenInfoDictionary());
			Add("ParentId", CreateParentIdTokenInfoDictionary());
			Add("DependentId", CreateDependentIdTokenInfoDictionary());
		}
		#region AppendAppointmentDependencySynonymsAsAttendants (helper method)
		void AppendAppointmentDependencySynonymsAsAttendants(TokenInfo info) {
			info.AddAttendant("id", 0);
			info.AddAttendant("oid", 0);
		}
		#endregion
		#region CreateTypeTokenInfoDictionary
		protected internal TokenInfoDictionary CreateTypeTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo info = new TokenInfo();
			info.AddSynonym("kind", 0);
			info.AddSynonym("sort", 1);
			AppendAppointmentDependencySynonymsAsAttendants(info);
			result.Add("type", info);
			return result;
		}
		#endregion
		#region CreateParentIdTokenInfoDictionary
		protected internal TokenInfoDictionary CreateParentIdTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo info = new TokenInfo();
			info.AddSynonym("key", 0);
			AppendAppointmentDependencySynonymsAsAttendants(info);
			result.Add("parent", info);
			result.Add("parentId", info);
			result.Add("id", info);
			return result;
		}
		#endregion
		#region CreateDependentIdTokenInfoDictionary
		protected internal TokenInfoDictionary CreateDependentIdTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo info = new TokenInfo();
			info.AddSynonym("key", 0);
			AppendAppointmentDependencySynonymsAsAttendants(info);
			result.Add("dependent", info);
			result.Add("dependentId", info);
			result.Add("depend", info);
			result.Add("id", info);
			return result;
		}
		#endregion
	}
	#endregion
	public class MappingsAutoPicker<T> where T : IPersistentObject {
		#region Fields
		MappingInfoBase<T> mappingInfo;
		MappingCollection mappings;
		DataFieldInfoCollection fields;
		MappingsTokenInfos tokenInfos;
		List<string> mappingsToIgnore;
		#endregion
		public MappingsAutoPicker(MappingInfoBase<T> mappingInfo, DataFieldInfoCollection fields, MappingsTokenInfos tokenInfos) {
			if (mappingInfo == null)
				Exceptions.ThrowArgumentNullException("mappings");
			if (fields == null)
				Exceptions.ThrowArgumentNullException("fields");
			if (tokenInfos == null)
				Exceptions.ThrowArgumentNullException("tokenInfos");
			this.mappingInfo = mappingInfo;
			this.fields = fields;
			this.tokenInfos = tokenInfos;
			this.mappings = new MappingCollection();
			this.mappingsToIgnore = new List<string>();
		}
		#region Properties
		protected internal MappingInfoBase<T> MappingInfo { get { return mappingInfo; } }
		protected internal MappingCollection Mappings { get { return mappings; } }
		protected internal DataFieldInfoCollection Fields { get { return fields; } }
		protected internal MappingsTokenInfos TokenInfos { get { return tokenInfos; } }
		List<string> MappingsToIgnore { get { return mappingsToIgnore; } }
		#endregion
		public virtual bool Execute() {
			return Execute(false);
		}
		public virtual bool Execute(bool forceAutoPick) {
			mappings.Clear();
			mappingInfo.AddMappings(mappings, false);
			if (!forceAutoPick && AreMappingsAssigned())
				return false;
			RemoveUnusedMappings();
			ExecuteCore();
			return true;
		}
		void RemoveUnusedMappings() {
			MappingCollection mappingsToRemove = new MappingCollection();
			foreach (MappingBase mapping in this.mappings) {
				if (!IsMappingFit(mapping))
					mappingsToRemove.Add(mapping);
			}
			foreach (MappingBase mapping in mappingsToRemove)
				this.mappings.Remove(mapping);
		}
		protected bool IsMappingFit(MappingBase mapping) {
			return !MappingsToIgnore.Contains(mapping.Name);
		}
		public void SetupMappingsToIgnore(List<string> mappingsToIgnore) {
			MappingsToIgnore.Clear();
			MappingsToIgnore.AddRange(mappingsToIgnore);
		}
		protected internal virtual bool AreMappingsAssigned() {
			int count = mappings.Count;
			for (int i = 0; i < count; i++)
				if (!String.IsNullOrEmpty(mappings[i].Member))
					return true;
			return false;
		}
		protected internal virtual void ExecuteCore() {
			DataFieldInfoCollection bestFitFields = CalculateBestMemberValues();
			int count = mappings.Count;
			for (int i = 0; i < count; i++) {
				DataFieldInfo field = bestFitFields[i];
				if (field != null)
					mappingInfo.SetMappingMember(mappings[i].Name, field.Name);
			}
		}
		protected internal virtual DataFieldInfoCollection CalculateBestMemberValues() {
			List<MappingFieldMatchCollection> matches = new List<MappingFieldMatchCollection>();
			PopulateMatches(matches);
			PrepareMatches(matches);
			return CalculateBestMemberValuesCore(matches);
		}
		protected internal virtual void PopulateMatches(List<MappingFieldMatchCollection> matches) {
			int count = mappings.Count;
			for (int i = 0; i < count; i++) {
				MappingFieldMatchCollection mappingMatch = new MappingFieldMatchCollection();
				PopulateMappingMatches(mappings[i], mappingMatch);
				matches.Add(mappingMatch);
			}
		}
		protected internal virtual void PopulateMappingMatches(MappingBase mapping, MappingFieldMatchCollection match) {
			int count = fields.Count;
			AutoMappingPicker picker = CreateMappingPicker(mapping);
			for (int i = 0; i < count; i++) {
				DataFieldInfo field = fields[i];
				if (picker.IsAssignableFrom(field)) {
					int weight = picker.CalculateMatchWeight(field);
					match.Add(new MappingFieldMatch(field, weight, i));
				}
			}
		}
		AutoMappingPicker CreateMappingPicker(MappingBase mapping) {
			if (mapping is ResourceColorMapping)
				return new ResourceColorMappingPicker(mapping, TokenInfos);
			if (mapping is AppointmentTimeZoneInfoMapping)
				return new TimeZoneIdMappingPicker(mapping, TokenInfos);
			return new AutoMappingPicker(mapping, TokenInfos);
		}
		const int ExactTypeMatchWeight = 3000;
		const int ExactNameMatchWeight = 3000;
		const int IgnoreCaseNameMatch = 2000;
		const int StartsWithNameMatchWeight = 1000;
		const int StartsWithNameIgnoreCaseMatchWeight = 500;
		const int MaxLevenshteinWeight = 500;
		protected internal virtual int CalculateMatchWeight(MappingBase mapping, DataFieldInfo field) {
			int result = 0;
			if (mapping.Type == field.Type || mapping.Type == typeof(Object))
				result = ExactTypeMatchWeight;
			if (String.Compare(mapping.Name, field.Name, StringComparison.InvariantCulture) == 0)
				result += ExactNameMatchWeight;
			if (String.Compare(mapping.Name, field.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
				result += IgnoreCaseNameMatch;
			if (field.Name.StartsWith(mapping.Name, StringComparison.InvariantCulture))
				result += StartsWithNameMatchWeight;
			if (field.Name.StartsWith(mapping.Name, StringComparison.InvariantCultureIgnoreCase))
				result += StartsWithNameIgnoreCaseMatchWeight;
			result += CalculateMatchWeightByName(mapping.Name, field.Name);
			return result;
		}
		protected internal virtual int CalculateMatchWeightByName(string mappingName, string fieldName) {
			StringTokenizer tokenizer = new StringTokenizer();
			StringCollection mappingNameTokens = tokenizer.Tokenize(mappingName);
			StringCollection fieldNameTokens = tokenizer.Tokenize(fieldName);
			TokenSequencesWithSynonymsLevenshteinDistanceCalculator calc;
			calc = new TokenSequencesWithSynonymsLevenshteinDistanceCalculator(TokenInfos[mappingName]);
			int weight = calc.Calculate(mappingNameTokens, fieldNameTokens);
			return MaxLevenshteinWeight - weight;
		}
		protected internal virtual void PrepareMatches(List<MappingFieldMatchCollection> matches) {
			matches.ForEach(SortMatchesByWeight);
		}
		protected internal virtual void SortMatchesByWeight(MappingFieldMatchCollection matches) {
			MappingFieldMatchComparer comparer = new MappingFieldMatchComparer();
			matches.Sort(comparer);
		}
		protected internal virtual DataFieldInfoCollection CalculateBestMemberValuesCore(List<MappingFieldMatchCollection> matches) {
			DataFieldInfoCollection result = new DataFieldInfoCollection();
			PopulateInitialMemberValues(matches, result);
			CalculateBestMemberValuesCore(matches, result);
			return result;
		}
		protected internal virtual void PopulateInitialMemberValues(List<MappingFieldMatchCollection> matches, DataFieldInfoCollection result) {
			int count = matches.Count;
			for (int i = 0; i < count; i++)
				result.Add(null);
		}
		protected internal virtual void CalculateBestMemberValuesCore(List<MappingFieldMatchCollection> matches, DataFieldInfoCollection result) {
			for (; ; ) {
				int index = LookupBestFitIndex(matches);
				if (index < 0)
					break;
				result[index] = matches[index][0].Field;
				RemoveUsedFieldFromConsideration(matches, index);
			}
		}
		protected internal virtual int LookupBestFitIndex(List<MappingFieldMatchCollection> matches) {
			int result = -1;
			int maxWeight = int.MinValue;
			int count = matches.Count;
			for (int i = 0; i < count; i++) {
				MappingFieldMatchCollection m = matches[i];
				if (m.Count > 0) {
					if (m[0].Weight > maxWeight) {
						maxWeight = m[0].Weight;
						result = i;
					}
				}
			}
			return result;
		}
		protected internal virtual void RemoveUsedFieldFromConsideration(List<MappingFieldMatchCollection> matches, int usedMatchIndex) {
			DataFieldInfo usedField = matches[usedMatchIndex][0].Field;
			matches[usedMatchIndex].Clear();
			int count = matches.Count;
			for (int i = 0; i < count; i++) {
				int index = CalculateFieldIndex(matches[i], usedField);
				if (index >= 0)
					matches[i].RemoveAt(index);
			}
		}
		protected internal virtual int CalculateFieldIndex(MappingFieldMatchCollection matches, DataFieldInfo field) {
			int count = matches.Count;
			for (int i = 0; i < count; i++)
				if (Object.ReferenceEquals(matches[i].Field, field))
					return i;
			return -1;
		}
	}
	#region MappingFieldMatch
	public class MappingFieldMatch {
		#region Fields
		DataFieldInfo field;
		int weight;
		int ordinal;
		#endregion
		public MappingFieldMatch(DataFieldInfo field, int weight, int ordinal) {
			if (field == null)
				Exceptions.ThrowArgumentNullException("field");
			this.field = field;
			this.weight = weight;
			this.ordinal = ordinal; 
		}
		#region Properties
		public DataFieldInfo Field { get { return field; } }
		public int Weight { get { return weight; } set { weight = value; } }
		public int Ordinal { get { return ordinal; } }
		#endregion
	}
	#endregion
	#region MappingFieldMatchCollection
	public class MappingFieldMatchCollection : List<MappingFieldMatch> {
	}
	#endregion
	#region MappingFieldMatchComparer
	public class MappingFieldMatchComparer : IComparer<MappingFieldMatch>, IComparer {
		public int Compare(MappingFieldMatch x, MappingFieldMatch y) {
			return CompareCore(x, y);
		}
		int IComparer.Compare(object x, object y) {
			return CompareCore((MappingFieldMatch)x, (MappingFieldMatch)y);
		}
		protected internal virtual int CompareCore(MappingFieldMatch x, MappingFieldMatch y) {
			int result = -x.Weight.CompareTo(y.Weight);
			if (result == 0)
				result = x.Ordinal.CompareTo(y.Ordinal);
			return result;
		}
	}
	#endregion
	#region StringSimilarityCalculator (abstract class)
	public abstract class StringSimilarityCalculator {
		public abstract float Calculate(string x, string y);
	}
	#endregion
	#region StringTokenizer
	public class StringTokenizer {
		#region Fields
		char[] separators;
		bool tokenizeByCaps = true;
		string tokenSeparators;
		#endregion
		public StringTokenizer() {
			this.TokenSeparators = @" _.,;:/\""'[]{}|<>`~!@#$%^&?*()-+=";
		}
		#region Properties
		public string TokenSeparators {
			get { return tokenSeparators; }
			set {
				if (tokenSeparators == value)
					return;
				tokenSeparators = value;
				UpdateSparators();
			}
		}
		public bool TokenizeByCaps { get { return tokenizeByCaps; } set { tokenizeByCaps = value; } }
		internal char[] Separators { get { return separators; } }
		#endregion
		protected internal virtual void UpdateSparators() {
			this.separators = tokenSeparators.ToCharArray();
		}
		public virtual StringCollection Tokenize(string str) {
			StringCollection result = PerformTokenizeBySeparators(str);
			if (TokenizeByCaps)
				PerformTokenizeByCaps(result);
			return result;
		}
		protected internal virtual StringCollection PerformTokenizeBySeparators(string str) {
			StringCollection result = new StringCollection();
			string[] tokens = str.Split(separators);
			int count = tokens.Length;
			for (int i = 0; i < count; i++) {
				string token = tokens[i];
				if (!String.IsNullOrEmpty(token))
					result.Add(token);
			}
			return result;
		}
		protected internal virtual void PerformTokenizeByCaps(StringCollection strings) {
			int count = strings.Count;
			for (int i = count - 1; i >= 0; i--) {
				StringCollection tokens = TokenizeByCapsCore(strings[i]);
				if (tokens.Count > 1)
					ReplaceStringWithTokens(strings, i, tokens);
			}
		}
		protected internal virtual StringCollection TokenizeByCapsCore(string str) {
			StringCollection result = new StringCollection();
			if (String.IsNullOrEmpty(str))
				return result;
			bool previousLower = true;
			int tokenStart = 0;
			int count = str.Length;
			for (int i = 0; i < count; i++) {
				if (Char.IsUpper(str[i])) {
					if (previousLower) {
						string token = str.Substring(tokenStart, i - tokenStart);
						if (!String.IsNullOrEmpty(token)) {
							result.Add(token);
							tokenStart = i;
						}
					}
					previousLower = false;
				} else
					previousLower = true;
			}
			string lastToken = str.Substring(tokenStart, count - tokenStart);
			if (!String.IsNullOrEmpty(lastToken))
				result.Add(lastToken);
			return result;
		}
		protected internal virtual void ReplaceStringWithTokens(StringCollection strings, int replacedStringIndex, StringCollection tokens) {
			strings.RemoveAt(replacedStringIndex);
			int count = tokens.Count;
			for (int i = 0; i < count; i++) {
				strings.Insert(replacedStringIndex, tokens[i]);
				replacedStringIndex++;
			}
		}
	}
	#endregion
	public abstract class LevenshteinDistanceCalculator<T> {
		public virtual int Calculate(T x, T y) {
			return CalcAlignmentMatrix(x, y)[GetLength(x), GetLength(y)];
		}
		protected internal virtual int[,] CalcAlignmentMatrix(T x, T y) {
			int m = GetLength(x);
			int n = GetLength(y);
			int[,] result = new int[m + 1, n + 1];
			result[0, 0] = 0;
			for (int j = 0; j < n; j++)
				result[0, j + 1] = result[0, j] + InsertCost(y, j);
			for (int i = 0; i < m; i++) {
				result[i + 1, 0] = result[i, 0] + DeleteCost(x, i);
				for (int j = 0; j < n; j++) {
					result[i + 1, j + 1] = Min(
						result[i, j] + SubstitutionCost(x, i, y, j),
						result[i, j + 1] + DeleteCost(x, i),
						result[i + 1, j] + InsertCost(y, j));
				}
			}
			return result;
		}
		protected internal virtual int Min(int x1, int x2, int x3) {
			return Math.Min(Math.Min(x1, x2), x3);
		}
		protected internal abstract int GetLength(T obj);
		protected internal abstract int DeleteCost(T x, int xi);
		protected internal abstract int InsertCost(T y, int yi);
		protected internal abstract int SubstitutionCost(T x, int pos1, T y, int pos2);
	}
	public class TokenInfo {
		int cost = 10;
		Dictionary<string, int> synonyms = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
		Dictionary<string, int> attendants = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
		public void AddSynonym(string synonym, int cost) {
			synonyms.Add(synonym, cost);
		}
		public void AddAttendant(string attendant, int cost) {
			attendants.Add(attendant, cost);
		}
		public int Cost { get { return cost; } set { cost = value; } }
		public int CalcSynonymSubstitutionCost(string synonym) {
			int result;
			if (synonyms.TryGetValue(synonym, out result))
				return result;
			else
				return 10;
		}
		public int CalcAttendantInsertionCost(string attendant) {
			int result;
			if (attendants.TryGetValue(attendant, out result))
				return result;
			else
				return 10;
		}
	}
	public class TokenInfoDictionary : Dictionary<string, TokenInfo> {
		public TokenInfoDictionary()
			: base(StringComparer.InvariantCultureIgnoreCase) {
		}
		public int CalcAttendantInsertionCost(string attendant) {
			int result = int.MaxValue;
			foreach (string key in this.Keys)
				result = Math.Min(result, this[key].CalcAttendantInsertionCost(attendant));
			return result;
		}
	}
	public class TokenSequencesWithSynonymsLevenshteinDistanceCalculator : LevenshteinDistanceCalculator<StringCollection> {
		TokenInfoDictionary tokenInfoDictionary;
		public TokenSequencesWithSynonymsLevenshteinDistanceCalculator(TokenInfoDictionary tokenInfoDictionary) {
			if (tokenInfoDictionary == null)
				Exceptions.ThrowArgumentNullException("tokenInfoDictionary");
			this.tokenInfoDictionary = tokenInfoDictionary;
		}
		protected internal TokenInfoDictionary TokenInfoDictionary { get { return tokenInfoDictionary; } }
		protected internal override int GetLength(StringCollection obj) {
			return obj.Count;
		}
		protected internal override int DeleteCost(StringCollection x, int xi) {
			return tokenInfoDictionary[x[xi]].Cost;
		}
		protected internal override int InsertCost(StringCollection y, int yi) {
			return tokenInfoDictionary.CalcAttendantInsertionCost(y[yi]);
		}
		protected internal override int SubstitutionCost(StringCollection x, int pos1, StringCollection y, int pos2) {
			if (String.Compare(x[pos1], y[pos2]) == 0)
				return 0;
			else
				return tokenInfoDictionary[x[pos1]].CalcSynonymSubstitutionCost(y[pos2]);
		}
	}
	public class StringLevenshteinDistanceCalculator : LevenshteinDistanceCalculator<string> {
		CultureInfo culture;
		public StringLevenshteinDistanceCalculator(CultureInfo culture) {
			this.culture = culture;
		}
		protected internal override int GetLength(string obj) {
			return obj.Length;
		}
		protected internal override int DeleteCost(string x, int xi) {
			return 1;
		}
		protected internal override int InsertCost(string y, int yi) {
			return 1;
		}
		protected internal override int SubstitutionCost(string x, int pos1, string y, int pos2) {
			int result = char.ToLower(x[pos1], culture) == char.ToLower(y[pos2], culture) ? 0 : 1;
			return result;
		}
	}
	public class ResourceColorMappingPicker : AutoMappingPicker {
		public ResourceColorMappingPicker(MappingBase mapping, MappingsTokenInfos tokenInfos)
			: base(mapping, tokenInfos) {
		}
		public override int CalculateMatchWeight(DataFieldInfo field) {
			int result = 0;
			if (String.Compare(Mapping.Name, field.Name, StringComparison.InvariantCulture) == 0)
				result += ExactNameMatchWeight;
			if (String.Compare(Mapping.Name, field.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
				result += IgnoreCaseNameMatch;
			if (field.Name.StartsWith(Mapping.Name, StringComparison.InvariantCulture))
				result += StartsWithNameMatchWeight;
			if (field.Name.StartsWith(Mapping.Name, StringComparison.InvariantCultureIgnoreCase))
				result += StartsWithNameIgnoreCaseMatchWeight;
			return result;
		}
		public override bool IsAssignableFrom(DataFieldInfo field) {
			return CalculateMatchWeight(field) != 0;
		}
	}
	public class TimeZoneIdMappingPicker : AutoMappingPicker {
		public TimeZoneIdMappingPicker(MappingBase mapping, MappingsTokenInfos tokenInfos)
			: base(mapping, tokenInfos) {
		}
		protected override int CalculateMatchWeightByName(string mappingName, string fieldName) {
			StringTokenizer tokenizer = new StringTokenizer();
			StringCollection mappingNameTokens = new StringCollection();
			mappingNameTokens.Add("TimeZoneId");
			StringCollection fieldNameTokens = new StringCollection();
			fieldNameTokens.Add(fieldName);
			TokenSequencesWithSynonymsLevenshteinDistanceCalculator calc;
			calc = new TokenSequencesWithSynonymsLevenshteinDistanceCalculator(TokenInfos[mappingName]);
			int weight = calc.Calculate(mappingNameTokens, fieldNameTokens);
			return MaxLevenshteinWeight - weight;
		}
		public override bool IsAssignableFrom(DataFieldInfo field) {
			int width = CalculateMatchWeightByName(Mapping.Name, field.Name);
			return width > 495 && Mapping.Type.IsAssignableFrom(field.Type);
		}
	}
	public class AutoMappingPicker {
		protected const int ExactTypeMatchWeight = 3000;
		protected const int ExactNameMatchWeight = 3000;
		protected const int IgnoreCaseNameMatch = 2000;
		protected const int StartsWithNameMatchWeight = 1000;
		protected const int StartsWithNameIgnoreCaseMatchWeight = 500;
		protected const int MaxLevenshteinWeight = 500;
		MappingsTokenInfos tokenInfos;
		MappingBase mapping;
		public AutoMappingPicker(MappingBase mapping, MappingsTokenInfos tokenInfos) {
			this.mapping = mapping;
			this.tokenInfos = tokenInfos;
		}
		protected internal MappingsTokenInfos TokenInfos { get { return tokenInfos; } }
		public MappingBase Mapping { get { return mapping; } }
		public virtual int CalculateMatchWeight(DataFieldInfo field) {
			int result = 0;
			if (Mapping.Type == field.Type || Mapping.Type == typeof(Object))
				result = ExactTypeMatchWeight;
			if (String.Compare(Mapping.Name, field.Name, StringComparison.InvariantCulture) == 0)
				result += ExactNameMatchWeight;
			if (String.Compare(Mapping.Name, field.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
				result += IgnoreCaseNameMatch;
			if (field.Name.StartsWith(Mapping.Name, StringComparison.InvariantCulture))
				result += StartsWithNameMatchWeight;
			if (field.Name.StartsWith(Mapping.Name, StringComparison.InvariantCultureIgnoreCase))
				result += StartsWithNameIgnoreCaseMatchWeight;
			result += CalculateMatchWeightByName(Mapping.Name, field.Name);
			return result;
		}
		public virtual bool IsAssignableFrom(DataFieldInfo field) {
			return Mapping.Type.IsAssignableFrom(field.Type);
		}
		protected virtual int CalculateMatchWeightByName(string mappingName, string fieldName) {
			StringTokenizer tokenizer = new StringTokenizer();
			StringCollection mappingNameTokens = tokenizer.Tokenize(mappingName);
			StringCollection fieldNameTokens = tokenizer.Tokenize(fieldName);
			TokenSequencesWithSynonymsLevenshteinDistanceCalculator calc;
			calc = new TokenSequencesWithSynonymsLevenshteinDistanceCalculator(tokenInfos[mappingName]);
			int weight = calc.Calculate(mappingNameTokens, fieldNameTokens);
			return MaxLevenshteinWeight - weight;
		}
	}
}
