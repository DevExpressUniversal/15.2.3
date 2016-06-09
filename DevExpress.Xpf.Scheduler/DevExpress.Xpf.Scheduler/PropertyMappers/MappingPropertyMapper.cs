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
using System.Windows;
using DevExpress.Utils;
using DevExpress.XtraScheduler;
namespace DevExpress.Xpf.Scheduler.Native {
	#region AppointmentMappingPropertySyncManager
	public class AppointmentMappingPropertySyncManager : DependencyPropertySyncManager {
		AppointmentMapping mapping;
		public AppointmentMappingPropertySyncManager(AppointmentMapping mapping) {
			Guard.ArgumentNotNull(mapping, "mapping");
			this.mapping = mapping;
		}
		public AppointmentMapping Mapping { get { return mapping; } }
		public override void Register() {
			PropertyMapperTable.RegisterPropertyMapper(AppointmentMapping.StartProperty, new AppointmentMappingStartPropertyMapper(AppointmentMapping.StartProperty, Mapping));
			PropertyMapperTable.RegisterPropertyMapper(AppointmentMapping.EndProperty, new AppointmentMappingEndPropertyMapper(AppointmentMapping.EndProperty, Mapping));
			PropertyMapperTable.RegisterPropertyMapper(AppointmentMapping.AppointmentIdProperty, new AppointmentMappingAppointmentIdPropertyMapper(AppointmentMapping.AppointmentIdProperty, Mapping));
			PropertyMapperTable.RegisterPropertyMapper(AppointmentMapping.AllDayProperty, new AppointmentMappingAllDayPropertyMapper(AppointmentMapping.AllDayProperty, Mapping));
			PropertyMapperTable.RegisterPropertyMapper(AppointmentMapping.DescriptionProperty, new AppointmentMappingDescriptionPropertyMapper(AppointmentMapping.DescriptionProperty, Mapping));
			PropertyMapperTable.RegisterPropertyMapper(AppointmentMapping.LabelProperty, new AppointmentMappingLabelPropertyMapper(AppointmentMapping.LabelProperty, Mapping));
			PropertyMapperTable.RegisterPropertyMapper(AppointmentMapping.LocationProperty, new AppointmentMappingLocationPropertyMapper(AppointmentMapping.LocationProperty, Mapping));
			PropertyMapperTable.RegisterPropertyMapper(AppointmentMapping.RecurrenceInfoProperty, new AppointmentMappingRecurrenceInfoPropertyMapper(AppointmentMapping.RecurrenceInfoProperty, Mapping));
			PropertyMapperTable.RegisterPropertyMapper(AppointmentMapping.ReminderInfoProperty, new AppointmentMappingReminderInfoPropertyMapper(AppointmentMapping.ReminderInfoProperty, Mapping));
			PropertyMapperTable.RegisterPropertyMapper(AppointmentMapping.ResourceIdProperty, new AppointmentMappingResourceIdPropertyMapper(AppointmentMapping.ResourceIdProperty, Mapping));
			PropertyMapperTable.RegisterPropertyMapper(AppointmentMapping.StatusProperty, new AppointmentMappingStatusPropertyMapper(AppointmentMapping.StatusProperty, Mapping));
			PropertyMapperTable.RegisterPropertyMapper(AppointmentMapping.SubjectProperty, new AppointmentMappingSubjectPropertyMapper(AppointmentMapping.SubjectProperty, Mapping));
			PropertyMapperTable.RegisterPropertyMapper(AppointmentMapping.TypeProperty, new AppointmentMappingTypePropertyMapper(AppointmentMapping.TypeProperty, Mapping));
			PropertyMapperTable.RegisterPropertyMapper(AppointmentMapping.TimeZoneIdProperty, new AppointmentMappingTimeZoneIdPropertyMapper(AppointmentMapping.TimeZoneIdProperty, Mapping));
		}
	}
	#endregion
	#region ResourceMappingPropertySyncManager
	public class ResourceMappingPropertySyncManager : DependencyPropertySyncManager {
		ResourceMapping mapping;
		public ResourceMappingPropertySyncManager(ResourceMapping mapping) {
			Guard.ArgumentNotNull(mapping, "mapping");
			this.mapping = mapping;
		}
		public ResourceMapping Mapping { get { return mapping; } }
		public override void Register() {
			PropertyMapperTable.RegisterPropertyMapper(ResourceMapping.CaptionProperty, new ResourceMappingCaptionPropertyMapper(ResourceMapping.CaptionProperty, Mapping));
			PropertyMapperTable.RegisterPropertyMapper(ResourceMapping.ColorProperty, new ResourceMappingColorPropertyMapper(ResourceMapping.ColorProperty, Mapping));
			PropertyMapperTable.RegisterPropertyMapper(ResourceMapping.IdProperty, new ResourceMappingIdPropertyMapper(ResourceMapping.IdProperty, Mapping));
			PropertyMapperTable.RegisterPropertyMapper(ResourceMapping.ImageProperty, new ResourceMappingImagePropertyMapper(ResourceMapping.ImageProperty, Mapping));
		}
	}
	#endregion
	#region Mappings mappers
	public abstract class PersistentObjectMappingPropertyMapper<T, U> : DependencyPropertyMapperBase where T : IPersistentObjectStorage<U> where U : IPersistentObject {
		readonly MappingInfoBase<U> innerMappingInfo;
		readonly PersistentObjectMapping<T, U> mappingContainer;
		protected PersistentObjectMappingPropertyMapper(DependencyProperty property, PersistentObjectMapping<T, U> mappingContainer)
			: base(property, mappingContainer) {
			Guard.ArgumentNotNull(mappingContainer, "mappingContainer");
			this.mappingContainer = mappingContainer;
			this.innerMappingInfo = mappingContainer.InnerMappingInfo;
		}
		protected MappingInfoBase<U> InnerMappingInfo { get { return innerMappingInfo; } }
		protected PersistentObjectMapping<T, U> MappingContainer { get { return mappingContainer; } }
	}
	public abstract class AppointmentMappingPropertyMapper : PersistentObjectMappingPropertyMapper<IAppointmentStorageBase, Appointment> {
		protected AppointmentMappingPropertyMapper(DependencyProperty property, AppointmentMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		public AppointmentMappingInfo InnerAppointmentMappingInfo { get { return (AppointmentMappingInfo)InnerMappingInfo; } }
	}
	public abstract class ResourceMappingPropertyMapper : PersistentObjectMappingPropertyMapper<IResourceStorageBase, Resource> {
		protected ResourceMappingPropertyMapper(DependencyProperty property, ResourceMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		public ResourceMappingInfo InnerResourceMappingInfo { get { return (ResourceMappingInfo)InnerMappingInfo; } }
	}
	public class AppointmentMappingStartPropertyMapper : AppointmentMappingPropertyMapper {
		public AppointmentMappingStartPropertyMapper(DependencyProperty property, AppointmentMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerAppointmentMappingInfo.Start = Convert.ToString(newValue);
		}
		public override object GetInnerPropertyValue() {
			return InnerAppointmentMappingInfo.Start;
		}
	}
	public class AppointmentMappingAppointmentIdPropertyMapper : AppointmentMappingPropertyMapper {
		public AppointmentMappingAppointmentIdPropertyMapper(DependencyProperty property, AppointmentMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerAppointmentMappingInfo.AppointmentId = Convert.ToString(newValue);
		}
		public override object GetInnerPropertyValue() {
			return InnerAppointmentMappingInfo.AppointmentId;
		}
	}
	public class AppointmentMappingAllDayPropertyMapper : AppointmentMappingPropertyMapper {
		public AppointmentMappingAllDayPropertyMapper(DependencyProperty property, AppointmentMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerAppointmentMappingInfo.AllDay = Convert.ToString(newValue);
		}
		public override object GetInnerPropertyValue() {
			return InnerAppointmentMappingInfo.AllDay;
		}
	}
	public class AppointmentMappingDescriptionPropertyMapper : AppointmentMappingPropertyMapper {
		public AppointmentMappingDescriptionPropertyMapper(DependencyProperty property, AppointmentMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerAppointmentMappingInfo.Description = Convert.ToString(newValue);
		}
		public override object GetInnerPropertyValue() {
			return InnerAppointmentMappingInfo.Description;
		}
	}
	public class AppointmentMappingEndPropertyMapper : AppointmentMappingPropertyMapper {
		public AppointmentMappingEndPropertyMapper(DependencyProperty property, AppointmentMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerAppointmentMappingInfo.End = Convert.ToString(newValue);
		}
		public override object GetInnerPropertyValue() {
			return InnerAppointmentMappingInfo.End;
		}
	}
	public class AppointmentMappingLabelPropertyMapper : AppointmentMappingPropertyMapper {
		public AppointmentMappingLabelPropertyMapper(DependencyProperty property, AppointmentMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerAppointmentMappingInfo.Label = Convert.ToString(newValue);
		}
		public override object GetInnerPropertyValue() {
			return InnerAppointmentMappingInfo.Label;
		}
	}
	public class AppointmentMappingLocationPropertyMapper : AppointmentMappingPropertyMapper {
		public AppointmentMappingLocationPropertyMapper(DependencyProperty property, AppointmentMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerAppointmentMappingInfo.Location = Convert.ToString(newValue);
		}
		public override object GetInnerPropertyValue() {
			return InnerAppointmentMappingInfo.Location;
		}
	}
	public class AppointmentMappingRecurrenceInfoPropertyMapper : AppointmentMappingPropertyMapper {
		public AppointmentMappingRecurrenceInfoPropertyMapper(DependencyProperty property, AppointmentMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerAppointmentMappingInfo.RecurrenceInfo = Convert.ToString(newValue);
		}
		public override object GetInnerPropertyValue() {
			return InnerAppointmentMappingInfo.RecurrenceInfo;
		}
	}
	public class AppointmentMappingReminderInfoPropertyMapper : AppointmentMappingPropertyMapper {
		public AppointmentMappingReminderInfoPropertyMapper(DependencyProperty property, AppointmentMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerAppointmentMappingInfo.ReminderInfo = Convert.ToString(newValue);
		}
		public override object GetInnerPropertyValue() {
			return InnerAppointmentMappingInfo.ReminderInfo;
		}
	}
	public class AppointmentMappingResourceIdPropertyMapper : AppointmentMappingPropertyMapper {
		public AppointmentMappingResourceIdPropertyMapper(DependencyProperty property, AppointmentMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerAppointmentMappingInfo.ResourceId = Convert.ToString(newValue);
		}
		public override object GetInnerPropertyValue() {
			return InnerAppointmentMappingInfo.ResourceId;
		}
	}
	public class AppointmentMappingStatusPropertyMapper : AppointmentMappingPropertyMapper {
		public AppointmentMappingStatusPropertyMapper(DependencyProperty property, AppointmentMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerAppointmentMappingInfo.Status = Convert.ToString(newValue);
		}
		public override object GetInnerPropertyValue() {
			return InnerAppointmentMappingInfo.Status;
		}
	}
	public class AppointmentMappingSubjectPropertyMapper : AppointmentMappingPropertyMapper {
		public AppointmentMappingSubjectPropertyMapper(DependencyProperty property, AppointmentMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerAppointmentMappingInfo.Subject = Convert.ToString(newValue);
		}
		public override object GetInnerPropertyValue() {
			return InnerAppointmentMappingInfo.Subject;
		}
	}
	public class AppointmentMappingTypePropertyMapper : AppointmentMappingPropertyMapper {
		public AppointmentMappingTypePropertyMapper(DependencyProperty property, AppointmentMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerAppointmentMappingInfo.Type = Convert.ToString(newValue);
		}
		public override object GetInnerPropertyValue() {
			return InnerAppointmentMappingInfo.Type;
		}
	}
	public class AppointmentMappingTimeZoneIdPropertyMapper : AppointmentMappingPropertyMapper {
		public AppointmentMappingTimeZoneIdPropertyMapper(DependencyProperty property, AppointmentMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		public override object GetInnerPropertyValue() {
			return InnerAppointmentMappingInfo.TimeZoneId;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerAppointmentMappingInfo.TimeZoneId = Convert.ToString(newValue);
		}
	}
	public class ResourceMappingCaptionPropertyMapper : ResourceMappingPropertyMapper {
		public ResourceMappingCaptionPropertyMapper(DependencyProperty property, ResourceMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerResourceMappingInfo.Caption = Convert.ToString(newValue);
		}
		public override object GetInnerPropertyValue() {
			return InnerResourceMappingInfo.Caption;
		}
	}
	public class ResourceMappingColorPropertyMapper : ResourceMappingPropertyMapper {
		public ResourceMappingColorPropertyMapper(DependencyProperty property, ResourceMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerResourceMappingInfo.Color = Convert.ToString(newValue);
		}
		public override object GetInnerPropertyValue() {
			return InnerResourceMappingInfo.Color;
		}
	}
	public class ResourceMappingIdPropertyMapper : ResourceMappingPropertyMapper {
		public ResourceMappingIdPropertyMapper(DependencyProperty property, ResourceMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerResourceMappingInfo.Id = Convert.ToString(newValue);
		}
		public override object GetInnerPropertyValue() {
			return InnerResourceMappingInfo.Id;
		}
	}
	public class ResourceMappingImagePropertyMapper : ResourceMappingPropertyMapper {
		public ResourceMappingImagePropertyMapper(DependencyProperty property, ResourceMapping mappingContainer)
			: base(property, mappingContainer) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerResourceMappingInfo.Image = Convert.ToString(newValue);
		}
		public override object GetInnerPropertyValue() {
			return InnerResourceMappingInfo.Image;
		}
	}
	#endregion
}
