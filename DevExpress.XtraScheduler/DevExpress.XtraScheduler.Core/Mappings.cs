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
using System.Drawing;
using System.Globalization;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Data;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Xml;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Internal;
#if !SL
using System.Drawing.Design;
using DevExpress.XtraScheduler.Design;
#else
using System.Windows.Media;
using System.Windows.Controls;
#endif
namespace DevExpress.XtraScheduler {
	public enum FieldValueType { Integer = 1, Decimal = 2, DateTime = 3, String = 4, Boolean = 5, Object = 6 };
	#region MappingBase (abstract class)
	public abstract class MappingBase {
		#region Fields
		string member = string.Empty;
		#endregion
		protected MappingBase() {
			this.member = Name;
		}
		#region Properties
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("MappingBaseName")]
#endif
		public abstract string Name { get; set; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public abstract Type Type { get; }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("MappingBaseMember")]
#endif
		public virtual string Member { get { return member; } set { member = value; } }
		protected internal virtual bool CommitToDataSource { get { return true; } }
		#endregion
		public object GetValue(IPersistentObject obj) {
			try {
				return GetValueCore(obj);
			} catch {
				return null;
			}
		}
		public void SetValue(IPersistentObject obj, object val) {
			try {
				SetValueCore(obj, val);
			} catch {
			}
		}
		protected internal virtual void Assign(MappingBase source) {
			if (source == null)
				return;
			Name = source.Name;
			Member = source.Member;
		}
		public abstract object GetValueCore(IPersistentObject obj);
		public abstract void SetValueCore(IPersistentObject obj, object val);
	}
	#endregion
	#region StringPropertyMapping (abstract class)
	public abstract class StringPropertyMapping : MappingBase {
		public override Type Type { get { return typeof(string); } }
		protected internal static string ValueToString(object val) {
			if (val is DBNull)
				return String.Empty;
			else
				return Convert.ToString(val, CultureInfo.InvariantCulture);
		}
	}
	#endregion
	#region MappingCollection
	[ListBindable(BindableSupport.No)]
	public class MappingCollection : NamedItemNotificationCollection<MappingBase> {
		protected override string GetItemName(MappingBase item) {
			return item.Name;
		}
		protected internal virtual void Assign(MappingCollection source) {
			if (source == null)
				return;
			BeginUpdate();
			try {
				Clear();
				for (int i = 0; i < source.Count; i++) {
					Add(CloneMapping(source[i]));
				}
			} finally {
				EndUpdate();
			}
		}
		protected MappingBase CloneMapping(MappingBase mapping) {
			MappingBase result = (MappingBase)Activator.CreateInstance(mapping.GetType());
			result.Assign(mapping);
			return result;
		}
	}
	#endregion
	#region MappingInfoBase<T> (abstract class)
#if !SL
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	public abstract class MappingInfoBase<T> : IPersistentObjectStorageProvider<T> where T : IPersistentObject {
		Dictionary<string, string> memberDictionary;
		protected MappingInfoBase() {
			this.memberDictionary = new Dictionary<string, string>();
			PopuplateMemberDictionary();
		}
		#region Events
		#region Changed
		EventHandler onChanged;
		internal event EventHandler Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected internal virtual void RaiseChanged() {
			if (onChanged != null)
				onChanged(this, EventArgs.Empty);
		}
		#endregion
		#region QueryPersistentObjectStorage
		QueryPersistentObjectStorageEventHandler<T> onQueryPersistentObjectStorage;
		internal event QueryPersistentObjectStorageEventHandler<T> QueryPersistentObjectStorage { add { onQueryPersistentObjectStorage += value; } remove { onQueryPersistentObjectStorage -= value; } }
		protected internal virtual IPersistentObjectStorage<T> RaiseQueryPersistentObjectStorage() {
			if (onQueryPersistentObjectStorage != null) {
				QueryPersistentObjectStorageEventArgs<T> args = new QueryPersistentObjectStorageEventArgs<T>();
				onQueryPersistentObjectStorage(this, args);
				return args.ObjectStorage;
			}
			return null;
		}
		#endregion
		#endregion
		#region Properties
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("You should not use the this property.", true), EditorBrowsable(EditorBrowsableState.Never)
		]
		public DataManager<T> DataManager { get { return null; } }
		protected internal Dictionary<string, string> MemberDictionary { get { return memberDictionary; } }
		#endregion
		protected internal virtual string GetMappingMember(string mappingName) {
			return MemberDictionary[mappingName];
		}
		protected internal virtual void SetMappingMember(string mappingName, string value) {
			string oldValue = GetMappingMember(mappingName);
			if (oldValue == value)
				return;
			MemberDictionary[mappingName] = value;
			RaiseChanged();
		}
		public abstract string[] GetRequiredMappingNames();
		protected internal abstract void AddMappingsCore(MappingCollection mappings, Dictionary<object, string> memberHash, bool ignoreEmptyMember);
		protected internal abstract Dictionary<object, string> CreateDefaultMemberHash();
		protected internal abstract Dictionary<object, string> CreateMemberHash();
		protected internal abstract void PopuplateMemberDictionary();
		protected internal abstract MappingsTokenInfos GetMappingsTokenInfos();
		protected internal void AddDefaultMappings(MappingCollection mappings, bool ignoreEmptyMember) {
			AddMappingsCore(mappings, CreateDefaultMemberHash(), ignoreEmptyMember);
		}
		protected internal void AddMappings(MappingCollection mappings, bool ignoreEmptyMember) {
			AddMappingsCore(mappings, CreateMemberHash(), ignoreEmptyMember);
		}
		protected internal static void AddMapping(MappingCollection target, MappingBase mapping, string member, bool ignoreEmptyMember) {
			if (ignoreEmptyMember && String.IsNullOrEmpty(member))
				return;
			Guard.ArgumentNotNull(target, "target");
			Guard.ArgumentNotNull(mapping, "mapping");
			mapping.Member = member;
			target.Add(mapping);
		}
		protected internal virtual void Assign(MappingInfoBase<T> source) {
		}
		#region IPersistentObjectStorageProvider<T> implementation
		IPersistentObjectStorage<T> IPersistentObjectStorageProvider<T>.ObjectStorage { get { return RaiseQueryPersistentObjectStorage(); } }
		#endregion
	}
	#endregion
	#region ResourceMappingInfo
#if !SL
	[Editor("DevExpress.XtraScheduler.Design.ResourceMappingInfoTypeEditor," + AssemblyInfo.SRAssemblySchedulerDesign, typeof(UITypeEditor))]
#endif
	public class ResourceMappingInfo : MappingInfoBase<Resource> {
		#region Fields
		ColorSavingType colorSaving = ColorSavingType.OleColor;
		#endregion
		#region Properties
		#region UseParentId
		[DefaultValue(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal virtual bool UseParentId { get { return true; } }
		#endregion
		#region SRId
		[DefaultValue(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal virtual string SRId { get { return ResourceSR.Id; } }
		#endregion
		#region Id
#if !SL
		[TypeConverter(typeof(ResourceColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("ResourceMappingInfoId"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string Id { get { return GetMappingMember(SRId); } set { SetMappingMember(SRId, value); } }
		#endregion
		#region ParentId
#if !SL
		[TypeConverter(typeof(ResourceColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("ResourceMappingInfoParentId"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string ParentId { get { return GetMappingMember(ResourceSR.ParentId); } set { SetMappingMember(ResourceSR.ParentId, value); } }
		#endregion
		#region Caption
#if !SL
		[TypeConverter(typeof(ResourceColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("ResourceMappingInfoCaption"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string Caption { get { return GetMappingMember(ResourceSR.Caption); } set { SetMappingMember(ResourceSR.Caption, value); } }
		#endregion
		#region Color
#if !SL
		[TypeConverter(typeof(ResourceColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("ResourceMappingInfoColor"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string Color { get { return GetMappingMember(ResourceSR.Color); } set { SetMappingMember(ResourceSR.Color, value); } }
		#endregion
		#region Image
#if !SL
		[TypeConverter(typeof(ResourceColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("ResourceMappingInfoImage"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string Image { get { return GetMappingMember(ResourceSR.Image); } set { SetMappingMember(ResourceSR.Image, value); } }
		#endregion
		#region ColorSaving
		[DefaultValue(ColorSavingType.OleColor), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal ColorSavingType ColorSaving {
			get {
				return colorSaving;
			}
			set {
				if (colorSaving == value)
					return;
				colorSaving = value;
				RaiseChanged();
			}
		}
		#endregion
		#endregion
		protected internal override void PopuplateMemberDictionary() {
			MemberDictionary.Add(SRId, String.Empty);
			MemberDictionary.Add(ResourceSR.Caption, String.Empty);
			MemberDictionary.Add(ResourceSR.Color, String.Empty);
			MemberDictionary.Add(ResourceSR.Image, String.Empty);
			if (UseParentId)
				MemberDictionary.Add(ResourceSR.ParentId, String.Empty);
		}
		internal MappingBase CreateMapping(ResourceMappingType type) {
			switch (type) {
				case ResourceMappingType.Caption:
					return new ResourceCaptionMapping();
				case ResourceMappingType.Color:
					ResourceColorMapping result = CreateResourceColorMapping();
					result.ColorSaving = ColorSaving;
					return result;
				case ResourceMappingType.Id:
					return GetResourceIdMapping();
				case ResourceMappingType.Image:
					return new ResourceImageMapping();
				case ResourceMappingType.ParentId:
					return new ResourceParentIdMapping();
			}
			return null;
		}
		protected internal virtual ResourceColorMapping CreateResourceColorMapping() {
			return new ResourceColorMapping();
		}
		protected internal virtual ResourceIdMapping GetResourceIdMapping() {
			return new ResourceIdMapping();
		}
		public override string[] GetRequiredMappingNames() {
			return new string[] { SRId };
		}
		protected internal override Dictionary<object, string> CreateDefaultMemberHash() {
			Dictionary<object, string> result = new Dictionary<object, string>();
			result.Add(ResourceMappingType.Id, SRId);
			result.Add(ResourceMappingType.Caption, ResourceSR.Caption);
			result.Add(ResourceMappingType.Color, ResourceSR.Color);
			result.Add(ResourceMappingType.Image, ResourceSR.Image);
			if (UseParentId)
				result.Add(ResourceMappingType.ParentId, ResourceSR.ParentId);
			return result;
		}
		protected internal override Dictionary<object, string> CreateMemberHash() {
			Dictionary<object, string> result = new Dictionary<object, string>();
			result.Add(ResourceMappingType.Id, Id);
			result.Add(ResourceMappingType.Caption, Caption);
			result.Add(ResourceMappingType.Color, Color);
			result.Add(ResourceMappingType.Image, Image);
			if (UseParentId)
				result.Add(ResourceMappingType.ParentId, ParentId);
			return result;
		}
		protected internal override void AddMappingsCore(MappingCollection mappings, Dictionary<object, string> memberHash, bool ignoreEmptyMember) {
			AddResourceMapping(mappings, ResourceMappingType.Id, memberHash[ResourceMappingType.Id], ignoreEmptyMember);
			AddResourceMapping(mappings, ResourceMappingType.Caption, memberHash[ResourceMappingType.Caption], ignoreEmptyMember);
			AddResourceMapping(mappings, ResourceMappingType.Color, memberHash[ResourceMappingType.Color], ignoreEmptyMember);
			AddResourceMapping(mappings, ResourceMappingType.Image, memberHash[ResourceMappingType.Image], ignoreEmptyMember);
#if !SL && !WPF
			if (UseParentId)
				AddResourceMapping(mappings, ResourceMappingType.ParentId, memberHash[ResourceMappingType.ParentId], ignoreEmptyMember);
#endif
		}
		void AddResourceMapping(MappingCollection target, ResourceMappingType type, object member, bool ignoreEmptyMember) {
			AddMapping(target, CreateMapping(type), Convert.ToString(member), ignoreEmptyMember);
		}
		protected internal override MappingsTokenInfos GetMappingsTokenInfos() {
			return new ResourceMappingsTokenInfos();
		}
		protected internal override void Assign(MappingInfoBase<Resource> source) {
			base.Assign(source);
			ResourceMappingInfo mappingInfo = source as ResourceMappingInfo;
			if (mappingInfo != null) {
				Id = mappingInfo.Id;
				if (UseParentId)
					ParentId = mappingInfo.ParentId;
				Caption = mappingInfo.Caption;
				Color = mappingInfo.Color;
				Image = mappingInfo.Image;
			}
		}
	}
	#endregion
	#region AppointmentMappingInfo
#if !SL
	[Editor("DevExpress.XtraScheduler.Design.AppointmentMappingInfoTypeEditor," + AssemblyInfo.SRAssemblySchedulerDesign, typeof(UITypeEditor))]
#endif
	public class AppointmentMappingInfo : MappingInfoBase<Appointment> {
		#region Fields
		bool commitIdToDataSource = true;
		bool resourceSharing;
		#endregion
		#region Properties
		#region UsePercentComplete
		[DefaultValue(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal virtual bool UsePercentComplete { get { return true; } }
		#endregion
		#region CommitIdToDataSource
		[DefaultValue(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool CommitIdToDataSource {
			get { return commitIdToDataSource; }
			set { commitIdToDataSource = value; RaiseChanged(); }
		}
		#endregion
		#region AppointmentId
#if !SL
		[TypeConverter(typeof(AppointmentColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentMappingInfoAppointmentId"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string AppointmentId { get { return GetMappingMember(AppointmentSR.Id); } set { SetMappingMember(AppointmentSR.Id, value); } }
		#endregion
		#region AllDay
#if !SL
		[TypeConverter(typeof(AppointmentColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentMappingInfoAllDay"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string AllDay { get { return GetMappingMember(AppointmentSR.AllDay); } set { SetMappingMember(AppointmentSR.AllDay, value); } }
		#endregion
		#region Status
#if !SL
		[TypeConverter(typeof(AppointmentColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentMappingInfoStatus"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string Status { get { return GetMappingMember(AppointmentSR.Status); } set { SetMappingMember(AppointmentSR.Status, value); } }
		#endregion
		#region Label
#if !SL
		[TypeConverter(typeof(AppointmentColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentMappingInfoLabel"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string Label { get { return GetMappingMember(AppointmentSR.Label); } set { SetMappingMember(AppointmentSR.Label, value); } }
		#endregion
		#region Description
#if !SL
		[TypeConverter(typeof(AppointmentColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentMappingInfoDescription"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true), AutoFormatDisable()]
		public virtual string Description { get { return GetMappingMember(AppointmentSR.Description); } set { SetMappingMember(AppointmentSR.Description, value); } }
		#endregion
		#region End
#if !SL
		[TypeConverter(typeof(AppointmentColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentMappingInfoEnd"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string End { get { return GetMappingMember(AppointmentSR.End); } set { SetMappingMember(AppointmentSR.End, value); } }
		#endregion
		#region Location
#if !SL
		[TypeConverter(typeof(AppointmentColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentMappingInfoLocation"),
#endif
	   DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string Location { get { return GetMappingMember(AppointmentSR.Location); } set { SetMappingMember(AppointmentSR.Location, value); } }
		#endregion
#if !SL && !WPF
		#region PercentComplete
#if !SL
		[TypeConverter(typeof(AppointmentColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentMappingInfoPercentComplete"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string PercentComplete { get { return GetMappingMember(AppointmentSR.PercentComplete); } set { SetMappingMember(AppointmentSR.PercentComplete, value); } }
		#endregion
#endif
		#region RecurrenceInfo
#if !SL
		[TypeConverter(typeof(AppointmentColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentMappingInfoRecurrenceInfo"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string RecurrenceInfo { get { return GetMappingMember(AppointmentSR.RecurrenceInfo); } set { SetMappingMember(AppointmentSR.RecurrenceInfo, value); } }
		#endregion
		#region ReminderInfo
#if !SL
		[TypeConverter(typeof(AppointmentColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentMappingInfoReminderInfo"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string ReminderInfo { get { return GetMappingMember(AppointmentSR.ReminderInfo); } set { SetMappingMember(AppointmentSR.ReminderInfo, value); } }
		#endregion
		#region ResourceId
#if !SL
		[TypeConverter(typeof(AppointmentColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentMappingInfoResourceId"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string ResourceId { get { return GetMappingMember(AppointmentSR.ResourceId); } set { SetMappingMember(AppointmentSR.ResourceId, value); } }
		#endregion
		#region Start
#if !SL
		[TypeConverter(typeof(AppointmentColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentMappingInfoStart"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string Start { get { return GetMappingMember(AppointmentSR.Start); } set { SetMappingMember(AppointmentSR.Start, value); } }
		#endregion
		#region Subject
#if !SL
		[TypeConverter(typeof(AppointmentColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentMappingInfoSubject"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string Subject { get { return GetMappingMember(AppointmentSR.Subject); } set { SetMappingMember(AppointmentSR.Subject, value); } }
		#endregion
		#region Type
#if !SL
		[TypeConverter(typeof(AppointmentColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentMappingInfoType"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string Type { get { return GetMappingMember(AppointmentSR.Type); } set { SetMappingMember(AppointmentSR.Type, value); } }
		#endregion
		#region TimeZoneId
		[TypeConverter(typeof(AppointmentColumnNameConverter))]
		[DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string TimeZoneId { get { return GetMappingMember(AppointmentSR.TimeZoneId); } set { SetMappingMember(AppointmentSR.TimeZoneId, value); } }
		#endregion
		#region ResourceSharing
		[DefaultValue(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool ResourceSharing {
			get { return resourceSharing; }
			set {
				if (resourceSharing == value)
					return;
				resourceSharing = value;
				RaiseChanged();
			}
		}
		#endregion
		#region DataManager
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("You should not use the this property.", true), EditorBrowsable(EditorBrowsableState.Never)
		]
		public new AppointmentDataManager DataManager { get { return null; } }
		#endregion
		#endregion
		protected internal override void PopuplateMemberDictionary() {
			MemberDictionary.Add(AppointmentSR.TimeZoneId, String.Empty);
			MemberDictionary.Add(AppointmentSR.Id, String.Empty);
			MemberDictionary.Add(AppointmentSR.AllDay, String.Empty);
			MemberDictionary.Add(AppointmentSR.Status, String.Empty);
			MemberDictionary.Add(AppointmentSR.Label, String.Empty);
			MemberDictionary.Add(AppointmentSR.Description, String.Empty);
			MemberDictionary.Add(AppointmentSR.Location, String.Empty);
			MemberDictionary.Add(AppointmentSR.RecurrenceInfo, String.Empty);
			MemberDictionary.Add(AppointmentSR.ReminderInfo, String.Empty);
			MemberDictionary.Add(AppointmentSR.End, String.Empty);
			MemberDictionary.Add(AppointmentSR.Start, String.Empty);
			MemberDictionary.Add(AppointmentSR.ResourceId, String.Empty);
			MemberDictionary.Add(AppointmentSR.Type, String.Empty);
			MemberDictionary.Add(AppointmentSR.Subject, String.Empty);
			if (UsePercentComplete)
				MemberDictionary.Add(AppointmentSR.PercentComplete, String.Empty);
		}
		protected internal virtual MappingBase CreateMapping(AppointmentMappingType type) {
			switch (type) {
#if !SL && !WPF
				case AppointmentMappingType.Id:
					return new AppointmentIdMapping(CommitIdToDataSource);
#endif
				case AppointmentMappingType.AllDay:
					return new AppointmentAllDayMapping();
				case AppointmentMappingType.Status:
					return new AppointmentStatusMapping();
				case AppointmentMappingType.Label:
					return new AppointmentLabelMapping();
				case AppointmentMappingType.End: {
						return new AppointmentEndMapping();
					}
				case AppointmentMappingType.Description:
					return new AppointmentDescriptionMapping();
				case AppointmentMappingType.Location:
					return new AppointmentLocationMapping();
				case AppointmentMappingType.RecurrenceInfo: 
					return new AppointmentRecurrenceInfoMapping();
				case AppointmentMappingType.ReminderInfo:
					return new AppointmentReminderInfoMapping();
				case AppointmentMappingType.ResourceId:
					if (this.ResourceSharing)
						return new AppointmentSharedResourceIdMapping();
					else
						return new AppointmentResourceIdMapping();
				case AppointmentMappingType.Start: {
						return new AppointmentStartMapping();
					}
				case AppointmentMappingType.Subject:
					return new AppointmentSubjectMapping();
				case AppointmentMappingType.Type:
					return new AppointmentTypeMapping();
#if !SL && !WPF
				case AppointmentMappingType.PercentComplete:
					return new AppointmentPercentCompleteMapping();
#endif
				case AppointmentMappingType.TimeZoneId:
					return new AppointmentTimeZoneInfoMapping();
			}
			return null;
		}
		public override string[] GetRequiredMappingNames() {
			return new string[] { AppointmentSR.Start, AppointmentSR.End };
		}
		protected internal override Dictionary<object, string> CreateDefaultMemberHash() {
			Dictionary<object, string> result = new Dictionary<object, string>();
			result.Add(AppointmentMappingType.TimeZoneId, AppointmentSR.TimeZoneId);
			result.Add(AppointmentMappingType.Id, AppointmentSR.Id);
			result.Add(AppointmentMappingType.Type, AppointmentSR.Type);
			result.Add(AppointmentMappingType.Start, AppointmentSR.Start);
			result.Add(AppointmentMappingType.End, AppointmentSR.End);
			result.Add(AppointmentMappingType.AllDay, AppointmentSR.AllDay);
			result.Add(AppointmentMappingType.Status, AppointmentSR.Status);
			result.Add(AppointmentMappingType.Label, AppointmentSR.Label);
			result.Add(AppointmentMappingType.Description, AppointmentSR.Description);
			result.Add(AppointmentMappingType.Location, AppointmentSR.Location);
			result.Add(AppointmentMappingType.RecurrenceInfo, AppointmentSR.RecurrenceInfo);
			result.Add(AppointmentMappingType.ReminderInfo, AppointmentSR.ReminderInfo);
			result.Add(AppointmentMappingType.ResourceId, AppointmentSR.ResourceId);
			result.Add(AppointmentMappingType.Subject, AppointmentSR.Subject);
			if (UsePercentComplete)
				result.Add(AppointmentMappingType.PercentComplete, AppointmentSR.PercentComplete);			
			return result;
		}
		protected internal override Dictionary<object, string> CreateMemberHash() {
			Dictionary<object, string> result = new Dictionary<object, string>();
			result.Add(AppointmentMappingType.Id, AppointmentId);
			result.Add(AppointmentMappingType.Type, Type);
			result.Add(AppointmentMappingType.Start, Start);
			result.Add(AppointmentMappingType.End, End);
			result.Add(AppointmentMappingType.AllDay, AllDay);
			result.Add(AppointmentMappingType.Status, Status);
			result.Add(AppointmentMappingType.Label, Label);
			result.Add(AppointmentMappingType.Description, Description);
			result.Add(AppointmentMappingType.Location, Location);
			result.Add(AppointmentMappingType.RecurrenceInfo, RecurrenceInfo);
			result.Add(AppointmentMappingType.ReminderInfo, ReminderInfo);
			result.Add(AppointmentMappingType.ResourceId, ResourceId);
			result.Add(AppointmentMappingType.Subject, Subject);
#if !SL && !WPF
			if (UsePercentComplete)
				result.Add(AppointmentMappingType.PercentComplete, PercentComplete);
#endif
			result.Add(AppointmentMappingType.TimeZoneId, TimeZoneId);
			return result;
		}
		protected internal override void AddMappingsCore(MappingCollection mappings, Dictionary<object, string> memberHash, bool ignoreEmptyMember) {
			AddAppointmentMapping(mappings, AppointmentMappingType.Type, memberHash[AppointmentMappingType.Type], ignoreEmptyMember);
			AddAppointmentMapping(mappings, AppointmentMappingType.TimeZoneId, memberHash[AppointmentMappingType.TimeZoneId], ignoreEmptyMember);
			AddAppointmentMapping(mappings, AppointmentMappingType.Start, memberHash[AppointmentMappingType.Start], ignoreEmptyMember);
			AddAppointmentMapping(mappings, AppointmentMappingType.End, memberHash[AppointmentMappingType.End], ignoreEmptyMember);
#if !SL && !WPF
			AddAppointmentMapping(mappings, AppointmentMappingType.Id, memberHash[AppointmentMappingType.Id], ignoreEmptyMember);
#endif
			AddAppointmentMapping(mappings, AppointmentMappingType.AllDay, memberHash[AppointmentMappingType.AllDay], ignoreEmptyMember);
			AddAppointmentMapping(mappings, AppointmentMappingType.Status, memberHash[AppointmentMappingType.Status], ignoreEmptyMember);
			AddAppointmentMapping(mappings, AppointmentMappingType.Label, memberHash[AppointmentMappingType.Label], ignoreEmptyMember);
			AddAppointmentMapping(mappings, AppointmentMappingType.Description, memberHash[AppointmentMappingType.Description], ignoreEmptyMember);
			AddAppointmentMapping(mappings, AppointmentMappingType.Location, memberHash[AppointmentMappingType.Location], ignoreEmptyMember);
			AddAppointmentMapping(mappings, AppointmentMappingType.RecurrenceInfo, memberHash[AppointmentMappingType.RecurrenceInfo], ignoreEmptyMember);
			AddAppointmentMapping(mappings, AppointmentMappingType.ReminderInfo, memberHash[AppointmentMappingType.ReminderInfo], ignoreEmptyMember);
			AddAppointmentMapping(mappings, AppointmentMappingType.ResourceId, memberHash[AppointmentMappingType.ResourceId], ignoreEmptyMember);
			AddAppointmentMapping(mappings, AppointmentMappingType.Subject, memberHash[AppointmentMappingType.Subject], ignoreEmptyMember);
#if !SL && !WPF
			if (UsePercentComplete)
				AddAppointmentMapping(mappings, AppointmentMappingType.PercentComplete, memberHash[AppointmentMappingType.PercentComplete], ignoreEmptyMember);
#endif            
		}
		protected void AddAppointmentMapping(MappingCollection target, AppointmentMappingType type, object member, bool ignoreEmptyMember) {
			AddMapping(target, CreateMapping(type), Convert.ToString(member), ignoreEmptyMember);
		}
		protected internal override MappingsTokenInfos GetMappingsTokenInfos() {
			return new AppointmentMappingsTokenInfos();
		}
		protected internal override void Assign(MappingInfoBase<Appointment> source) {
			base.Assign(source);
			AppointmentMappingInfo mappingInfo = source as AppointmentMappingInfo;
			if (mappingInfo != null) {
				AllDay = mappingInfo.AllDay;
				AppointmentId = mappingInfo.AppointmentId;
				Status = mappingInfo.Status;
				Label = mappingInfo.Label;
				Description = mappingInfo.Description;
				End = mappingInfo.End;
				Location = mappingInfo.Location;
#if !SL && !WPF
				if (UsePercentComplete)
					PercentComplete = mappingInfo.PercentComplete;
#endif
				RecurrenceInfo = mappingInfo.RecurrenceInfo;
				ReminderInfo = mappingInfo.ReminderInfo;
				ResourceId = mappingInfo.ResourceId;
				Start = mappingInfo.Start;
				Subject = mappingInfo.Subject;
				Type = mappingInfo.Type;
				TimeZoneId = mappingInfo.TimeZoneId;
			}
		}
	}
	#endregion
	#region AppointmentDependencyMappingInfo
#if !SL
	[Editor("DevExpress.XtraScheduler.Design.AppointmentDependencyMappingInfoTypeEditor," + AssemblyInfo.SRAssemblySchedulerDesign, typeof(UITypeEditor))]
#endif
	public class AppointmentDependencyMappingInfo : MappingInfoBase<AppointmentDependency> {
		#region Properties
		#region Type
#if !SL
		[TypeConverter(typeof(AppointmentDependencyColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDependencyMappingInfoType"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string Type { get { return GetMappingMember(AppointmentDependencySR.Type); } set { SetMappingMember(AppointmentDependencySR.Type, value); } }
		#endregion
		#region ParentId
#if !SL
		[TypeConverter(typeof(AppointmentDependencyColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDependencyMappingInfoParentId"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string ParentId { get { return GetMappingMember(AppointmentDependencySR.ParentId); } set { SetMappingMember(AppointmentDependencySR.ParentId, value); } }
		#endregion
		#region ParentId
#if !SL
		[TypeConverter(typeof(AppointmentDependencyColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDependencyMappingInfoDependentId"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		AutoFormatDisable()]
		public virtual string DependentId { get { return GetMappingMember(AppointmentDependencySR.DependentId); } set { SetMappingMember(AppointmentDependencySR.DependentId, value); } }
		#endregion
		#endregion
		protected internal override void PopuplateMemberDictionary() {
			MemberDictionary.Add(AppointmentDependencySR.Type, String.Empty);
			MemberDictionary.Add(AppointmentDependencySR.ParentId, String.Empty);
			MemberDictionary.Add(AppointmentDependencySR.DependentId, String.Empty);
		}
		internal MappingBase CreateMapping(AppointmentDependencyMappingType type) {
			switch (type) {
				case AppointmentDependencyMappingType.Type:
					return new AppointmentDependencyTypeMapping();
				case AppointmentDependencyMappingType.ParentId:
					return new AppointmentDependencyParentIdMapping();
				case AppointmentDependencyMappingType.DependentId:
					return new AppointmentDependencyDependentIdMapping();
			}
			return null;
		}
		public override string[] GetRequiredMappingNames() {
			return new string[] { AppointmentDependencySR.ParentId, AppointmentDependencySR.DependentId };
		}
		protected internal override Dictionary<object, string> CreateDefaultMemberHash() {
			Dictionary<object, string> result = new Dictionary<object, string>();
			result.Add(AppointmentDependencyMappingType.Type, AppointmentDependencySR.Type);
			result.Add(AppointmentDependencyMappingType.ParentId, AppointmentDependencySR.ParentId);
			result.Add(AppointmentDependencyMappingType.DependentId, AppointmentDependencySR.DependentId);
			return result;
		}
		protected internal override Dictionary<object, string> CreateMemberHash() {
			Dictionary<object, string> result = new Dictionary<object, string>();
			result.Add(AppointmentDependencyMappingType.Type, Type);
			result.Add(AppointmentDependencyMappingType.ParentId, ParentId);
			result.Add(AppointmentDependencyMappingType.DependentId, DependentId);
			return result;
		}
		protected internal override void AddMappingsCore(MappingCollection mappings, Dictionary<object, string> memberHash, bool ignoreEmptyMember) {
			AddAppointmentDependencyMapping(mappings, AppointmentDependencyMappingType.Type, memberHash[AppointmentDependencyMappingType.Type], ignoreEmptyMember);
			AddAppointmentDependencyMapping(mappings, AppointmentDependencyMappingType.ParentId, memberHash[AppointmentDependencyMappingType.ParentId], ignoreEmptyMember);
			AddAppointmentDependencyMapping(mappings, AppointmentDependencyMappingType.DependentId, memberHash[AppointmentDependencyMappingType.DependentId], ignoreEmptyMember);
		}
		protected void AddAppointmentDependencyMapping(MappingCollection target, AppointmentDependencyMappingType type, object member, bool ignoreEmptyMember) {
			AddMapping(target, CreateMapping(type), Convert.ToString(member), ignoreEmptyMember);
		}
		protected internal override MappingsTokenInfos GetMappingsTokenInfos() {
			return new AppointmentDependencyMappingsTokenInfos();
		}
	}
	#endregion
	#region CustomFieldMappingBase<T>
#if !SL
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverter))]
#endif
	public class CustomFieldMappingBase<T> : MappingBase, IPersistentObjectStorageProvider<T> where T : IPersistentObject {
		#region Fields
		string name = String.Empty;
		FieldValueType valueType = FieldValueType.Object;
		#endregion
		#region Events
		#region Changing
		EventHandler onChanging;
		internal event EventHandler Changing { add { onChanging += value; } remove { onChanging -= value; } }
		protected internal virtual void RaiseChanging() {
			if (onChanging != null)
				onChanging(this, EventArgs.Empty);
		}
		#endregion
		#region Changed
		EventHandler onChanged;
		internal event EventHandler Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected internal virtual void RaiseChanged() {
			if (onChanged != null)
				onChanged(this, EventArgs.Empty);
		}
		#endregion
		#region QueryPersistentObjectStorage
		QueryPersistentObjectStorageEventHandler<T> onQueryPersistentObjectStorage;
		internal event QueryPersistentObjectStorageEventHandler<T> QueryPersistentObjectStorage { add { onQueryPersistentObjectStorage += value; } remove { onQueryPersistentObjectStorage -= value; } }
		protected internal virtual IPersistentObjectStorage<T> RaiseQueryPersistentObjectStorage() {
			if (onQueryPersistentObjectStorage != null) {
				QueryPersistentObjectStorageEventArgs<T> args = new QueryPersistentObjectStorageEventArgs<T>();
				onQueryPersistentObjectStorage(this, args);
				return args.ObjectStorage;
			}
			return null;
		}
		#endregion
		#endregion
		protected internal CustomFieldMappingBase() {
		}
		protected internal CustomFieldMappingBase(string name, string member) {
			this.name = name;
			this.Member = member;
		}
		protected internal CustomFieldMappingBase(string name, string member, FieldValueType valueType) {
			this.name = name;
			this.Member = member;
			this.valueType = valueType;
		}
		#region Properties
		#region Name
		[DefaultValue(""),
		NotifyParentProperty(true)]
		public override string Name {
			get { return name; }
			set {
				if (name == value)
					return;
				RaiseChanging();
				name = value;
				RaiseChanged();
			}
		}
		#endregion
		#region Member
		[DefaultValue(""),
		NotifyParentProperty(true)]
		public override string Member {
			get { return base.Member; }
			set {
				if (base.Member == value)
					return;
				RaiseChanging();
				base.Member = value;
				RaiseChanged();
			}
		}
		#endregion
		#region ValueType
		[DefaultValue(FieldValueType.Object), NotifyParentProperty(true)]
		public FieldValueType ValueType {
			get { return valueType; }
			set {
				if (ValueType == value)
					return;
				RaiseChanging();
				valueType = value;
				RaiseChanged();
			}
		}
		#endregion
		#region Type
		public override Type Type { get { return GetType(ValueType); } }
		#endregion
		#endregion
		protected internal override void Assign(MappingBase source) {
			base.Assign(source);
			CustomFieldMappingBase<T> customFieldMapping = source as CustomFieldMappingBase<T>;
			if (customFieldMapping != null)
				ValueType = customFieldMapping.ValueType;
		}
		public override object GetValueCore(IPersistentObject obj) {
			return obj.CustomFields[Name];
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			if (val is DBNull)
				obj.CustomFields[Name] = null;
			else
				obj.CustomFields[Name] = val;
		}
		protected virtual Type GetType(FieldValueType valueType) {
			return new UnboundColumnInfo(Name, (UnboundColumnType)valueType, false).DataType;
		}
		#region IPersistentObjectStorageProvider implementation
		IPersistentObjectStorage<T> IPersistentObjectStorageProvider<T>.ObjectStorage { get { return RaiseQueryPersistentObjectStorage(); } }
		#endregion
	}
	#endregion
	#region ResourceCustomFieldMapping
	public class ResourceCustomFieldMapping : CustomFieldMappingBase<Resource> {
		public ResourceCustomFieldMapping() {
		}
		public ResourceCustomFieldMapping(string name, string member)
			: base(name, member) {
		}
		public ResourceCustomFieldMapping(string name, string member, FieldValueType valueType)
			: base(name, member, valueType) {
		}
		#region Member
#if !SL
		[TypeConverter(typeof(ResourceColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("ResourceCustomFieldMappingMember"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true)]
		public override string Member { get { return base.Member; } set { base.Member = value; } }
		#endregion
	}
	#endregion
	#region AppointmentCustomFieldMapping
	public class AppointmentCustomFieldMapping : CustomFieldMappingBase<Appointment> {
		public AppointmentCustomFieldMapping() {
		}
		public AppointmentCustomFieldMapping(string name, string member)
			: base(name, member) {
		}
		public AppointmentCustomFieldMapping(string name, string member, FieldValueType valueType)
			: base(name, member, valueType) {
		}
		#region Member
#if  !SL
		[TypeConverter(typeof(AppointmentColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentCustomFieldMappingMember"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true)]
		public override string Member { get { return base.Member; } set { base.Member = value; } }
		#endregion
	}
	#endregion
	#region AppointmentDependencyCustomFieldMapping
	public class AppointmentDependencyCustomFieldMapping : CustomFieldMappingBase<AppointmentDependency> {
		public AppointmentDependencyCustomFieldMapping() {
		}
		public AppointmentDependencyCustomFieldMapping(string name, string member)
			: base(name, member) {
		}
		public AppointmentDependencyCustomFieldMapping(string name, string member, FieldValueType valueType)
			: base(name, member, valueType) {
		}
		#region Member
#if !SL
		[TypeConverter(typeof(AppointmentDependencyColumnNameConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDependencyCustomFieldMappingMember"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true)]
		public override string Member { get { return base.Member; } set { base.Member = value; } }
		#endregion
	}
	#endregion
	#region CustomFieldMappingCollectionBase<T>
#if !SL
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
#endif
	public class CustomFieldMappingCollectionBase<T> : MappingCollection, IPersistentObjectStorageProvider<T> where T : IPersistentObject {
		public new CustomFieldMappingBase<T> this[int index] { get { return (CustomFieldMappingBase<T>)base[index]; } }
		public new CustomFieldMappingBase<T> this[string name] { get { return base[name] as CustomFieldMappingBase<T>; } }
		#region Events
		#region QueryPersistentObjectStorage
		QueryPersistentObjectStorageEventHandler<T> onQueryPersistentObjectStorage;
		internal event QueryPersistentObjectStorageEventHandler<T> QueryPersistentObjectStorage { add { onQueryPersistentObjectStorage += value; } remove { onQueryPersistentObjectStorage -= value; } }
		protected internal virtual IPersistentObjectStorage<T> RaiseQueryPersistentObjectStorage() {
			if (onQueryPersistentObjectStorage != null) {
				QueryPersistentObjectStorageEventArgs<T> args = new QueryPersistentObjectStorageEventArgs<T>();
				onQueryPersistentObjectStorage(this, args);
				return args.ObjectStorage;
			}
			return null;
		}
		#endregion
		#endregion
		protected override void OnInsertComplete(int index, MappingBase value) {
			base.OnInsertComplete(index, value);
			CustomFieldMappingBase<T> mapping = (CustomFieldMappingBase<T>)value;
			SubscribeMappingEvents(mapping);
		}
		protected override void OnRemoveComplete(int index, MappingBase value) {
			base.OnRemoveComplete(index, value);
			CustomFieldMappingBase<T> mapping = (CustomFieldMappingBase<T>)value;
			UnsubscribeMappingEvents(mapping);
		}
		protected internal virtual void SubscribeMappingEvents(CustomFieldMappingBase<T> mapping) {
			mapping.Changing += new EventHandler(OnMappingChanging);
			mapping.Changed += new EventHandler(OnMappingChanged);
			mapping.QueryPersistentObjectStorage += new QueryPersistentObjectStorageEventHandler<T>(OnQueryPersistentObjectStorage);
		}
		protected internal virtual void UnsubscribeMappingEvents(CustomFieldMappingBase<T> mapping) {
			mapping.Changing -= new EventHandler(OnMappingChanging);
			mapping.Changed -= new EventHandler(OnMappingChanged);
			mapping.QueryPersistentObjectStorage -= new QueryPersistentObjectStorageEventHandler<T>(OnQueryPersistentObjectStorage);
		}
		protected override bool OnClear() {
			int count = Count;
			for (int i = 0; i < count; i++)
				UnsubscribeMappingEvents(this[i]);
			return base.OnClear();
		}
		protected virtual void OnMappingChanged(object sender, EventArgs e) {
			CustomFieldMappingBase<T> mapping = (CustomFieldMappingBase<T>)sender;
			NameHash.Add(mapping.Name, mapping);
			OnCollectionChanged(new CollectionChangedEventArgs<MappingBase>(CollectionChangedAction.Changed, mapping));
		}
		protected virtual void OnMappingChanging(object sender, EventArgs e) {
			CustomFieldMappingBase<T> mapping = (CustomFieldMappingBase<T>)sender;
			NameHash.Remove(mapping.Name);
			OnCollectionChanging(new CollectionChangingEventArgs<MappingBase>(CollectionChangedAction.Changed, mapping));
		}
		public int Add(CustomFieldMappingBase<T> mapping) {
			return base.Add(mapping);
		}
		protected internal virtual void OnQueryPersistentObjectStorage(object sender, QueryPersistentObjectStorageEventArgs<T> e) {
			e.ObjectStorage = RaiseQueryPersistentObjectStorage();
		}
		#region IPersistentObjectStorageProvider implementation
		IPersistentObjectStorage<T> IPersistentObjectStorageProvider<T>.ObjectStorage { get { return RaiseQueryPersistentObjectStorage(); } }
		#endregion
	}
	#endregion
	#region AppointmentCustomFieldMappingCollection (compatibility class)
	public class AppointmentCustomFieldMappingCollection : CustomFieldMappingCollectionBase<Appointment> {
		public new AppointmentCustomFieldMapping this[int index] { get { return (AppointmentCustomFieldMapping)base[index]; } }
		public new AppointmentCustomFieldMapping this[string name] { get { return (AppointmentCustomFieldMapping)base[name]; } }
		public int Add(AppointmentCustomFieldMapping mapping) {
			return base.Add(mapping);
		}
	}
	#endregion
	#region ResourceCustomFieldMappingCollection (compatibility class)
	public class ResourceCustomFieldMappingCollection : CustomFieldMappingCollectionBase<Resource> {
		public new ResourceCustomFieldMapping this[int index] { get { return (ResourceCustomFieldMapping)base[index]; } }
		public new ResourceCustomFieldMapping this[string name] { get { return (ResourceCustomFieldMapping)base[name]; } }
		public int Add(ResourceCustomFieldMapping mapping) {
			return base.Add(mapping);
		}
	}
	#endregion
	#region AppointmentDependencyCustomFieldMappingCollection (compatibility class)
	public class AppointmentDependencyCustomFieldMappingCollection : CustomFieldMappingCollectionBase<AppointmentDependency> {
		public new AppointmentDependencyCustomFieldMapping this[int index] { get { return (AppointmentDependencyCustomFieldMapping)base[index]; } }
		public new AppointmentDependencyCustomFieldMapping this[string name] { get { return (AppointmentDependencyCustomFieldMapping)base[name]; } }
		public int Add(AppointmentDependencyCustomFieldMapping mapping) {
			return base.Add(mapping);
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	#region ResourceMappingType
	public enum ResourceMappingType {
		Id,
		Caption,
		Color,
		Image,
		ParentId
	};
	#endregion
	#region ResourceCaptionMapping
	public class ResourceCaptionMapping : StringPropertyMapping {
		public ResourceCaptionMapping() {
		}
		public override string Name { get { return ResourceSR.Caption; } set { } }
		public override object GetValueCore(IPersistentObject obj) {
			return ((Resource)obj).Caption;
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			((Resource)obj).Caption = ValueToString(val);
		}
	}
	#endregion
	#region ResourceColorMapping
	public class ResourceColorMapping : MappingBase {
		#region Fields
		ColorSavingType colorSaving = ColorSavingType.OleColor;
		#endregion
		public ResourceColorMapping() {
		}
		#region Properties
		public override string Name { get { return ResourceSR.Color; } set { } }
		public override Type Type { get { return typeof(int); } }
		public ColorSavingType ColorSaving { get { return colorSaving; } set { colorSaving = value; } }
		#endregion
		#region GetValueCore
		public override object GetValueCore(IPersistentObject obj) {
			string colorValue = ((Resource)obj).ColorValue;
			Color color = WinColorToStringSerializer.StringToColor(colorValue);
			switch (ColorSaving) {
				case ColorSavingType.ArgbColor:
					return color.ToArgb();
				case ColorSavingType.Color:
					return color;
				default:
					return DXColor.ToOle(color);
			}
		}
		#endregion
		#region SetValueCore
		public override void SetValueCore(IPersistentObject obj, object val) {
			Resource resource = (Resource)obj;
			if (val == null || (val is DBNull)) {
				resource.ColorValue = null;
				return;
			}
			int intColor;
			if (Int32.TryParse(val.ToString(), out intColor) && intColor == 0) {
				resource.ColorValue = null;
				return;
			}
			resource.ColorValue = WinColorToStringSerializer.ColorToString(GetColorFromValue(val));
		}
		#endregion
		#region GetColorFromValue
		protected internal virtual Color GetColorFromValue(object val) {
			if (ColorSaving == ColorSavingType.Color)
				return (Color)val;
			int int32Val = unchecked((int)(Convert.ToInt64(val, CultureInfo.InvariantCulture)));
			switch (ColorSaving) {
				case ColorSavingType.ArgbColor:
					return DXColor.FromArgb(int32Val);
				default:
					return DXColor.FromOle(int32Val);
			}
		}
		#endregion
	}
	#endregion
	#region ResourceIdMapping
	public class ResourceIdMapping : MappingBase {
		public ResourceIdMapping() {
		}
		public override string Name { get { return ResourceSR.Id; } set { } }
		public override Type Type { get { return typeof(object); } }
		public override object GetValueCore(IPersistentObject obj) {
			return ((Resource)obj).Id;
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			((Resource)obj).SetId(val);
		}
	}
	#endregion
	#region ResourceParentIdMapping
	public class ResourceParentIdMapping : MappingBase {
		public ResourceParentIdMapping() {
		}
		public override string Name { get { return ResourceSR.ParentId; } set { } }
		public override Type Type { get { return typeof(object); } }
		public override object GetValueCore(IPersistentObject obj) {
			return ((Resource)obj).ParentId;
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			IInternalResource res = (IInternalResource)obj;
			if (val is DBNull)
				res.SetParentId(null);
			else
				res.SetParentId(val);
		}
	}
	#endregion
	#region ResourceImageMapping
	public class ResourceImageMapping : MappingBase {
		public ResourceImageMapping() {
		}
		public override string Name {
			get { return ResourceSR.Image; }
			set { }
		}
		public override Type Type {
			get { return typeof(byte[]); }
		}
		public override object GetValueCore(IPersistentObject obj) {
			return ((Resource)obj).ImageBytes;
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			byte[] bytes = val as byte[];
			Resource resource = (Resource)obj;
			if (bytes != null)
				resource.ImageBytes = (byte[])bytes.Clone();
			else {
				Image image = val as Image;
				if (image != null)
					resource.ImageBytes = SchedulerImageHelper.GetImageBytes(image);
				else
					resource.ImageBytes = null;
			}
		}
	}
	#endregion
	#region AppointmentMappingType
	public enum AppointmentMappingType {
		AllDay,
		Description,
		End,
		Location,
		PercentComplete,
		RecurrenceInfo,
		ResourceId,
		ReminderInfo,
		Start,
		Status,
		Label,
		Subject,
		Type,
		Id,
		TimeZoneId
	}
	#endregion
	#region AppointmentSubjectMapping
	public class AppointmentSubjectMapping : StringPropertyMapping {
		public AppointmentSubjectMapping() {
		}
		public override string Name { get { return AppointmentSR.Subject; } set { } }
		public override object GetValueCore(IPersistentObject obj) {
			return ((Appointment)obj).Subject;
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			((Appointment)obj).Subject = ValueToString(val);
		}
	}
	#endregion
	#region AppointmentDescriptionMapping
	public class AppointmentDescriptionMapping : StringPropertyMapping {
		public AppointmentDescriptionMapping() {
		}
		public override string Name { get { return AppointmentSR.Description; } set { } }
		public override object GetValueCore(IPersistentObject obj) {
			return ((Appointment)obj).Description;
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			((Appointment)obj).Description = ValueToString(val);
		}
	}
	#endregion
	#region AppointmentLocationMapping
	public class AppointmentLocationMapping : StringPropertyMapping {
		public AppointmentLocationMapping() {
		}
		public override string Name { get { return AppointmentSR.Location; } set { } }
		public override object GetValueCore(IPersistentObject obj) {
			return ((Appointment)obj).Location;
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			((Appointment)obj).Location = ValueToString(val);
		}
	}
	#endregion
	#region AppointmentTypeMapping
	public class AppointmentTypeMapping : MappingBase {
		public AppointmentTypeMapping() {
		}
		public override string Name { get { return AppointmentSR.Type; } set { } }
		public override Type Type { get { return typeof(int); } }
		public override object GetValueCore(IPersistentObject obj) {
			return (int)((Appointment)obj).Type;
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			AppointmentType type;
			if (val is DBNull)
				type = AppointmentType.Normal;
			else
				type = (AppointmentType)Convert.ToInt32(val, CultureInfo.InvariantCulture);
			((IInternalAppointment)obj).SetTypeCore(type);
		}
	}
	#endregion
#if !SL && !WPF
	#region AppointmentPercentCompleteMapping
	public class AppointmentPercentCompleteMapping : MappingBase {
		public AppointmentPercentCompleteMapping() {
		}
		public override string Name { get { return AppointmentSR.PercentComplete; } set { } }
		public override Type Type { get { return typeof(int); } }
		public override object GetValueCore(IPersistentObject obj) {
			return (int)((Appointment)obj).PercentComplete;
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			Appointment apt = (Appointment)obj;
			if (val is DBNull)
				apt.PercentComplete = 0;
			else
				apt.PercentComplete = Convert.ToInt32(val, CultureInfo.InvariantCulture);
		}
	}
	#endregion
#endif
	#region AppointmentIdMapping
	public class AppointmentIdMapping : MappingBase {
		bool commitToDataSource;
		public AppointmentIdMapping()
			: this(true) {
		}
		public AppointmentIdMapping(bool commitToDataSource) {
			this.commitToDataSource = commitToDataSource;
		}
		public override string Name { get { return AppointmentSR.Id; } set { } }
		public override Type Type { get { return typeof(object); } }
		protected internal override bool CommitToDataSource { get { return commitToDataSource; } }
		public override object GetValueCore(IPersistentObject obj) {
			return ((Appointment)obj).Id;
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			Appointment apt = (Appointment)obj;
			if (val is DBNull)
				apt.SetId(null);
			else
				apt.SetId(val);
		}
	}
	#endregion
	#region AppointmentAllDayMapping
	public class AppointmentAllDayMapping : MappingBase {
		public AppointmentAllDayMapping() {
		}
		public override string Name { get { return AppointmentSR.AllDay; } set { } }
		public override Type Type { get { return typeof(bool); } }
		public override object GetValueCore(IPersistentObject obj) {
			return ((Appointment)obj).AllDay;
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			Appointment apt = (Appointment)obj;
			if (val is DBNull)
				apt.AllDay = false;
			else
				apt.AllDay = Convert.ToBoolean(val, CultureInfo.InvariantCulture);
		}
	}
	#endregion
	#region AppointmentResourceIdMapping
	public class AppointmentResourceIdMapping : MappingBase {
		public AppointmentResourceIdMapping() {
		}
		public override string Name { get { return AppointmentSR.ResourceId; } set { } }
		public override Type Type { get { return typeof(object); } }
		public override object GetValueCore(IPersistentObject obj) {
			object resourceId = ((Appointment)obj).ResourceId;
			if (ResourceBase.IsEmptyResourceId(resourceId))
				resourceId = null;
			return resourceId;
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			IInternalAppointment apt = (IInternalAppointment)obj;
			if (val is DBNull)
				apt.SetResourceId(EmptyResourceId.Id);
			else
				apt.SetResourceId(val);
		}
	}
	#endregion
	#region AppointmentSharedResourceIdMapping
	public class AppointmentSharedResourceIdMapping : StringPropertyMapping {
		public AppointmentSharedResourceIdMapping() {
		}
		public override string Name { get { return AppointmentSR.ResourceId; } set { } }
		public override object GetValueCore(IPersistentObject obj) {
			Appointment apt = (Appointment)obj;
			if (apt.ResourceIds.Count == 1 && ResourceBase.IsEmptyResourceId(apt.ResourceIds[0]))
				return null;
			AppointmentResourceIdCollectionXmlPersistenceHelper helper = new AppointmentResourceIdCollectionXmlPersistenceHelper(apt.ResourceIds);
			return helper.ToXml();
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			Appointment apt = (Appointment)obj;
			if (apt.ResourceIds is AppointmentResourceIdReadOnlyCollection)
				return;
			string s = ValueToString(val);
			AppointmentResourceIdCollectionXmlPersistenceHelper.ObjectFromXml(apt.ResourceIds, s);
		}
	}
	#endregion
	#region AppointmentDateMappingBase (abstract class)
	public abstract class AppointmentDateMappingBase : MappingBase {
		protected AppointmentDateMappingBase() {
			EnableDateSaving = true;
		}
		public override Type Type { get { return typeof(DateTime); } }
		protected bool EnableDateSaving { get; set; }
		protected internal DateTime GetDateValue(DateTime val, TimeZoneEngine timeZoneEngine, string timeZoneId) {
			timeZoneEngine = ValidateTimeZoneEngine(timeZoneEngine);
			if (timeZoneEngine.UseDefaultAppointmentTimeZone)
				return timeZoneEngine.FromOperationTimeToDefaultAppointmentTimeZone(val);
			else if (!String.IsNullOrEmpty(timeZoneId) && EnableDateSaving)
				return timeZoneEngine.FromOperationTime(val, timeZoneId);
			return val;
		}
		protected internal DateTime SetDateValue(DateTime val, TimeZoneEngine timeZoneEngine, string timeZoneId) {
			timeZoneEngine = ValidateTimeZoneEngine(timeZoneEngine);
			if (timeZoneEngine.UseDefaultAppointmentTimeZone)
				return timeZoneEngine.ToOperationTimeFromDefaultAppointmentTimeZone(val);
			else if (!String.IsNullOrEmpty(timeZoneId) && EnableDateSaving)
				return timeZoneEngine.ToOperationTime(val, timeZoneId);
			return val;
		}
		TimeZoneEngine ValidateTimeZoneEngine(TimeZoneEngine timeZoneEngine) {
			return timeZoneEngine ?? new TimeZoneEngine();
		}
	}
	#endregion
	#region AppointmentStartMapping
	public class AppointmentStartMapping : AppointmentDateMappingBase {
		public override string Name { get { return AppointmentSR.Start; } set { } }
		public override object GetValueCore(IPersistentObject obj) {
			Appointment apt = (Appointment)obj;
			try {
				EnableDateSaving = !apt.AllDay;
				return GetDateValue(apt.Start, ((IInternalAppointment)apt).TimeZoneEngine, apt.TimeZoneId);
			} finally {
				EnableDateSaving = true;
			}
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			Appointment apt = (Appointment)obj;
			try {
				EnableDateSaving = !apt.AllDay;
				if (val is DBNull)
					apt.Start = DateTime.MinValue;
				else
					apt.Start = SetDateValue(Convert.ToDateTime(val, CultureInfo.InvariantCulture), ((IInternalAppointment)apt).TimeZoneEngine, apt.TimeZoneId);
			} finally {
				EnableDateSaving = true;
			}
		}
	}
	#endregion
	#region AppointmentEndMapping
	public class AppointmentEndMapping : AppointmentDateMappingBase {
		public override string Name { get { return AppointmentSR.End; } set { } }
		public override object GetValueCore(IPersistentObject obj) {
			try {
				Appointment apt = (Appointment)obj;
				EnableDateSaving = !apt.AllDay;
				return GetDateValue(apt.End, ((IInternalAppointment)apt).TimeZoneEngine, apt.TimeZoneId);
			} finally {
				EnableDateSaving = true;
			}
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			Appointment apt = (Appointment)obj;
			try {
				EnableDateSaving = !apt.AllDay;
				if (val == null || val is DBNull)
					apt.Duration = TimeSpan.Zero;
				else
					apt.End = SetDateValue(Convert.ToDateTime(val, CultureInfo.InvariantCulture), ((IInternalAppointment)apt).TimeZoneEngine, apt.TimeZoneId);
			} finally {
				EnableDateSaving = true;
			}
		}
	}
	#endregion
	#region AppointmentStatusMapping
	public class AppointmentStatusMapping : MappingBase {
		public AppointmentStatusMapping() {
		}
		public override string Name { get { return AppointmentSR.Status; } set { } }
		public override Type Type { get { return typeof(int); } }
		public override object GetValueCore(IPersistentObject obj) {
			return ((Appointment)obj).StatusKey;
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			Appointment apt = (Appointment)obj;
			if (val is DBNull)
				apt.StatusKey = null;
			else
				apt.StatusKey = val;
		}
	}
	#endregion
	#region AppointmentLabelMapping
	public class AppointmentLabelMapping : MappingBase {
		public AppointmentLabelMapping() {
		}
		public override string Name { get { return AppointmentSR.Label; } set { } }
		public override Type Type { get { return typeof(int); } }
		public override object GetValueCore(IPersistentObject obj) {
			return ((Appointment)obj).LabelKey;
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			Appointment apt = (Appointment)obj;
			if (val is DBNull)
				apt.LabelKey = null;
			else
				apt.LabelKey = val;
		}
	}
	#endregion
	#region AppointmentRecurrenceInfoMapping
	public class AppointmentRecurrenceInfoMapping : StringPropertyMapping {
		public override string Name { get { return AppointmentSR.RecurrenceInfo; } set { } }
		public override object GetValueCore(IPersistentObject obj) {
			Appointment apt = (Appointment)obj;
			if (apt.Type == AppointmentType.Pattern) {
				TimeZoneEngine timeZoneEngine = ValidateTimeZoneEngine(((IInternalAppointment)apt).TimeZoneEngine);
				RecurrenceInfoXmlPersistenceHelper helper = new RecurrenceInfoXmlPersistenceHelper(apt.RecurrenceInfo, timeZoneEngine, apt.TimeZoneId);
				return helper.ToXml();
			} else if (apt.IsException) {
				PatternReference reference = new PatternReference(apt.RecurrenceInfo.Id, apt.RecurrenceIndex);
				PatternReferenceXmlPersistenceHelper helper = new PatternReferenceXmlPersistenceHelper(reference);
				return helper.ToXml();
			} else
				return null;
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			string s = ValueToString(val);
			Appointment apt = (Appointment)obj;
			IInternalAppointment internalApt = (IInternalAppointment)apt;
			if (apt.Type == AppointmentType.Pattern) {
				TimeZoneEngine timeZoneEngine = ValidateTimeZoneEngine(internalApt.TimeZoneEngine);
				IRecurrenceInfo recurrenceInfo = RecurrenceInfoXmlPersistenceHelper.ObjectFromXml(s, timeZoneEngine, apt.TimeZoneId);
				((IInternalRecurrenceInfo)recurrenceInfo).SetTimeZoneId(apt.TimeZoneId);
				if (recurrenceInfo != null)
					internalApt.SetRecurrenceInfo(recurrenceInfo);
			} else if (apt.IsException) {
				PatternReference reference = PatternReferenceXmlPersistenceHelper.ObjectFromXml(s);
				if (reference != null) {
					internalApt.SetRecurrenceIndex(reference.RecurrenceIndex);
				}
			}
		}
		TimeZoneEngine ValidateTimeZoneEngine(TimeZoneEngine timeZoneEngine) {
			return timeZoneEngine ?? new TimeZoneEngine();
		}
	}
	#endregion
	#region AppointmentReminderInfoMapping
	public class AppointmentReminderInfoMapping : StringPropertyMapping {
		public override string Name { get { return AppointmentSR.ReminderInfo; } set { } }
		public override object GetValueCore(IPersistentObject obj) {
			Appointment apt = (Appointment)obj;
			TimeZoneEngine timeZoneEngine = ValidateTimeZoneEngine(((IInternalAppointment)apt).TimeZoneEngine);
			ReminderCollectionXmlPersistenceHelper helper = ReminderCollectionXmlPersistenceHelper.CreateSaveInstance(apt, timeZoneEngine);
			return helper.ToXml();
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			Appointment apt = (Appointment)obj;
			apt.HasReminder = false;
			string s = ValueToString(val);
			TimeZoneEngine timeZoneEngine = ValidateTimeZoneEngine(((IInternalAppointment)apt).TimeZoneEngine);
			ReminderCollectionXmlPersistenceHelper.ObjectFromXml(apt, s, timeZoneEngine);
		}
		TimeZoneEngine ValidateTimeZoneEngine(TimeZoneEngine timeZoneEngine) {
			return timeZoneEngine ?? new TimeZoneEngine();
		}
	}
	#endregion
	#region AppointmentDependencyMappingType
	public enum AppointmentDependencyMappingType {
		Type,
		ParentId,
		DependentId
	}
	#endregion
	#region AppointmentDependencyTypeMapping
	public class AppointmentDependencyTypeMapping : MappingBase {
		public AppointmentDependencyTypeMapping() {
		}
		public override string Name { get { return AppointmentDependencySR.Type; } set { } }
		public override Type Type { get { return typeof(int); } }
		public override object GetValueCore(IPersistentObject obj) {
			return (int)((AppointmentDependency)obj).Type;
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			AppointmentDependencyType type;
			if (val is DBNull)
				type = AppointmentDependencyType.FinishToStart;
			else
				type = (AppointmentDependencyType)Convert.ToInt32(val, CultureInfo.InvariantCulture);
			((AppointmentDependency)obj).Type = type;
		}
	}
	#endregion
	#region AppointmentDependencyParentIdMapping
	public class AppointmentDependencyParentIdMapping : MappingBase {
		public AppointmentDependencyParentIdMapping() {
		}
		public override string Name { get { return AppointmentDependencySR.ParentId; } set { } }
		public override Type Type { get { return typeof(object); } }
		public override object GetValueCore(IPersistentObject obj) {
			return ((AppointmentDependency)obj).ParentId;
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			IInternalAppointmentDependency aptDepend = (IInternalAppointmentDependency)obj;
			if (val is DBNull)
				aptDepend.SetParentId(null);
			else
				aptDepend.SetParentId(val);
		}
	}
	#endregion
	#region AppointmentDependencyDependentIdMapping
	public class AppointmentDependencyDependentIdMapping : MappingBase {
		public AppointmentDependencyDependentIdMapping() {
		}
		public override string Name { get { return AppointmentDependencySR.DependentId; } set { } }
		public override Type Type { get { return typeof(object); } }
		public override object GetValueCore(IPersistentObject obj) {
			return ((AppointmentDependency)obj).DependentId;
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			IInternalAppointmentDependency aptDepend = (IInternalAppointmentDependency)obj;
			if (val is DBNull)
				aptDepend.SetDependentId(null);
			else
				aptDepend.SetDependentId(val);
		}
	}
	#endregion
	public class AppointmentTimeZoneInfoMapping : StringPropertyMapping {
		public AppointmentTimeZoneInfoMapping() {
		}
		public override string Name { get { return AppointmentSR.TimeZoneId; } set { } }
		public string TimeZoneId { get; set; }
		public override object GetValueCore(IPersistentObject obj) {
			Appointment apt = (Appointment)obj;
			return apt.TimeZoneId;
		}
		public override void SetValueCore(IPersistentObject obj, object val) {
			Appointment apt = (Appointment)obj;
			apt.TimeZoneId = (string)val;
		}
	}
}
