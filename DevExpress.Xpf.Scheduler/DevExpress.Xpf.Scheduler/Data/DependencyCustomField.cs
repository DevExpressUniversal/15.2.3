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
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using DevExpress.XtraScheduler;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections;
using DevExpress.Xpf.Core;
#if SL
using PlatformIndependentColor = System.Windows.Media.Color;
#else
using PlatformIndependentColor = System.Drawing.Color;
using System.Windows.Media;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Xpf.Scheduler.Native;
using System.Windows.Markup;
using DevExpress.Utils;
using System.CodeDom.Compiler;
#endif
namespace DevExpress.Xpf.Scheduler {
	#region SchedulerCustomFieldMapping
	public class SchedulerCustomFieldMapping : DependencyObject, INotifyPropertyChanged {
		public SchedulerCustomFieldMapping() {
		}
		public SchedulerCustomFieldMapping(string name, string member) {
			Name = name;
			Member = member;
		}
		#region Name
		public string Name {
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty = CreateNameProperty();
		static DependencyProperty CreateNameProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerCustomFieldMapping, string>("Name", string.Empty, (d, e) => d.OnNameChanged(e.OldValue, e.NewValue), null);
		}
		private void OnNameChanged(string oldValue, string newValue) {
			RaiseOnPropertyChanged("Name");
		}
		#endregion
		#region Member
		public string Member {
			get { return (string)GetValue(MemberProperty); }
			set { SetValue(MemberProperty, value); }
		}
		public static readonly DependencyProperty MemberProperty = CreateMemberProperty();
		static DependencyProperty CreateMemberProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerCustomFieldMapping, string>("Member", string.Empty, (d, e) => d.OnMemberChanged(e.OldValue, e.NewValue), null);
		}
		private void OnMemberChanged(string oldValue, string newValue) {
			RaiseOnPropertyChanged("Member");
		}
		#endregion
		#region ValueType
		public FieldValueType ValueType {
			get { return (FieldValueType)GetValue(ValueTypeProperty); }
			set { SetValue(ValueTypeProperty, value); }
		}
		public static readonly DependencyProperty ValueTypeProperty = CreateValueTypeProperty();
		static DependencyProperty CreateValueTypeProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerCustomFieldMapping, FieldValueType>("ValueType", FieldValueType.Object, (d, e) => d.OnValueTypeChanged(e.OldValue, e.NewValue), null);
		}
		private void OnValueTypeChanged(FieldValueType oldValue, FieldValueType newValue) {
			RaiseOnPropertyChanged("ValueType");
		}
		#endregion
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged {
			add { onPropertyChanged += value; }
			remove { onPropertyChanged -= value; }
		}
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { onPropertyChanged += value; }
			remove { onPropertyChanged -= value; }
		}
		protected void RaiseOnPropertyChanged(string propertyName) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
	#endregion
	public static class WpfAppointmentLabelColor {
		public static readonly Color None = Colors.White;
		public static readonly Color Important = Color.FromArgb(0xFF, 0xFF, 0xC2, 0xBE);
		public static readonly Color Business = Color.FromArgb(0xFF, 0xA8, 0xD5, 0xFF);
		public static readonly Color Personal = Color.FromArgb(0xFF, 0xC1, 0xF4, 0x9C);
		public static readonly Color Vacation = Color.FromArgb(0xFF, 0xF3, 0xE4, 0xC7);
		public static readonly Color MustAttend = Color.FromArgb(0xFF, 0xF4, 0xCE, 0x93);
		public static readonly Color TravelRequired = Color.FromArgb(0xFF, 0xC7, 0xF4, 0xFF);
		public static readonly Color NeedsPreparation = Color.FromArgb(0xFF, 0xCF, 0xDB, 0x98);
		public static readonly Color Birthday = Color.FromArgb(0xFF, 0xE0, 0xCF, 0xE9);
		public static readonly Color Anniversary = Color.FromArgb(0xFF, 0x8D, 0xE9, 0xDF);
		public static readonly Color PhoneCall = Color.FromArgb(0xFF, 0xFF, 0xF7, 0xA5);
	}
	#region AppointmentLabelCollection
	[GeneratedCode("Suppress FxCop check", "")]
	public class AppointmentLabelCollection : UserInterfaceObjectCollection<IAppointmentLabel>, IAppointmentLabelStorage {
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use the GetById or GetByIndex methods instead")]
		public new AppointmentLabel this[int index] {
			get { return GetById(index); }
		}
		public new AppointmentLabel GetById(object id) {
			return (AppointmentLabel)base.GetById(id);
		}
		public new AppointmentLabel GetByIndex(int index) {
			return (AppointmentLabel)base.GetItem(index);
		}
		protected internal override IAppointmentLabel CreateItemInstance(object id, string displayName, string menuCaption) {
			return new AppointmentLabel(id, Colors.White, displayName, menuCaption);
		}
		protected internal override object ProvideDefaultId() {
			return Count;
		}
		protected IAppointmentLabel CreateItem(Color labelColor, string displayName, string menuCaption) {
			AppointmentLabel label = (AppointmentLabel)base.CreateItem(null, displayName, menuCaption);
			label.Color = labelColor;
			return label;
		}
		protected internal override UserInterfaceObjectCollection<IAppointmentLabel> CreateDefaultContent() {
			AppointmentLabelCollection defaultContent = new AppointmentLabelCollection();
			defaultContent.Add(defaultContent.CreateItem(WpfAppointmentLabelColor.None, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_None), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelNone)));
			defaultContent.Add(defaultContent.CreateItem(WpfAppointmentLabelColor.Important, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_Important), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelImportant)));
			defaultContent.Add(defaultContent.CreateItem(WpfAppointmentLabelColor.Business, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_Business), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelBusiness)));
			defaultContent.Add(defaultContent.CreateItem(WpfAppointmentLabelColor.Personal, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_Personal), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelPersonal)));
			defaultContent.Add(defaultContent.CreateItem(WpfAppointmentLabelColor.Vacation, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_Vacation), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelVacation)));
			defaultContent.Add(defaultContent.CreateItem(WpfAppointmentLabelColor.MustAttend, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_MustAttend), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelMustAttend)));
			defaultContent.Add(defaultContent.CreateItem(WpfAppointmentLabelColor.TravelRequired, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_TravelRequired), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelTravelRequired)));
			defaultContent.Add(defaultContent.CreateItem(WpfAppointmentLabelColor.NeedsPreparation, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_NeedsPreparation), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelNeedsPreparation)));
			defaultContent.Add(defaultContent.CreateItem(WpfAppointmentLabelColor.Birthday, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_Birthday), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelBirthday)));
			defaultContent.Add(defaultContent.CreateItem(WpfAppointmentLabelColor.Anniversary, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_Anniversary), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelAnniversary)));
			defaultContent.Add(defaultContent.CreateItem(WpfAppointmentLabelColor.PhoneCall, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_PhoneCall), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelPhoneCall)));
			return defaultContent;
		}
		protected internal override IAppointmentLabel CloneItem(IAppointmentLabel item) {
			AppointmentLabel source = (AppointmentLabel)item;
			AppointmentLabel target = new AppointmentLabel(source.Id, source.Brush.Clone(), source.MenuCaption, source.DisplayName);
			target.Assign(source);
			return target;
		}
		public int Add(Color color, string displayName) {
			return Add(color, displayName, displayName);
		}
		public int Add(Color color, string displayName, string menuCaption) {
			return Add(null, displayName, menuCaption, color);
		}
		public virtual int Add(AppointmentLabel value) {
			return base.Add(value);
		}
		public int Add(object id, string displayName, string menuCaption, Color labelColor) {
			AppointmentLabel label = (AppointmentLabel)CreateItem(id, displayName, menuCaption);
			label.Color = labelColor;
			return Add(label);
		}
		public AppointmentLabel CreateNewLabel(string displayName) {
			return CreateNewLabel(null, displayName, displayName);
		}
		public AppointmentLabel CreateNewLabel(object id, string displayName) {
			return CreateNewLabel(id, displayName, displayName);
		}
		public AppointmentLabel CreateNewLabel(object id, string displayName, string menuCaption) {
			return (AppointmentLabel)CreateItem(id, displayName, menuCaption);
		}
		public AppointmentLabel CreateNewLabel(object id, string displayName, string menuCaption, Color color) {
			AppointmentLabel label = (AppointmentLabel)CreateItem(id, displayName, menuCaption);
			label.Color = color;
			return label;
		}
		IAppointmentLabel IAppointmentLabelStorage.CreateNewLabel(string displayName) {
			return CreateNewLabel(displayName);
		}
		IAppointmentLabel IAppointmentLabelStorage.CreateNewLabel(object id, string displayName) {
			return CreateNewLabel(id, displayName);
		}
		IAppointmentLabel IAppointmentLabelStorage.CreateNewLabel(object id, string displayName, string menuCaption) {
			return CreateNewLabel(id, displayName, menuCaption);
		}
		public void Dispose() {
			foreach (IAppointmentLabel item in this) {
				IDisposable disposableItem = item as IDisposable;
				if (disposableItem != null)
					disposableItem.Dispose();
			}
		}
	}
	#endregion
	#region AppointmentStatusCollection
	[GeneratedCode("Suppress FxCop check", "")]
	public class AppointmentStatusCollection : UserInterfaceObjectCollection<IAppointmentStatus>, IAppointmentStatusStorage {
		[Obsolete("Use the GetById, GetByIndex, or GetByType methods instead"), Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new AppointmentStatus this[int index] {
			get { return GetByIndex(index); }
		}
		public new AppointmentStatus GetById(object id) {
			return (AppointmentStatus)base.GetById(id);
		}
		public new AppointmentStatus GetByIndex(int index) {
			return (AppointmentStatus)base.GetItem(index);
		}
		protected internal override object ProvideDefaultId() {
			return Count;
		}
		protected internal override IAppointmentStatus CreateItemInstance(object id, string displayName, string menuCaption) {
			return new AppointmentStatus(id, null, AppointmentStatusType.Custom, displayName, menuCaption);
		}
		protected internal IAppointmentStatus CreateDefaultStatusByType(AppointmentStatusType type) {
			Brush brush = null;
			string text = String.Empty;
			string menuCaption = String.Empty;
			switch (type) {
				case AppointmentStatusType.Free:
					brush = new SolidColorBrush(Colors.White);
					text = SchedulerLocalizer.GetString(SchedulerStringId.Caption_Free);
					menuCaption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_Free);
					break;
				case AppointmentStatusType.OutOfOffice:
					brush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD9, 0x53, 0x53));
					text = SchedulerLocalizer.GetString(SchedulerStringId.Caption_OutOfOffice);
					menuCaption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_OutOfOffice);
					break;
				case AppointmentStatusType.Busy:
					brush = new SolidColorBrush(Color.FromArgb(0xFF, 0x4A, 0x87, 0xE2));
					text = SchedulerLocalizer.GetString(SchedulerStringId.Caption_Busy);
					menuCaption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_Busy);
					break;
				case AppointmentStatusType.Tentative:
					brush = BrushHelper.CreateHatchBrush(Color.FromArgb(0xFF, 0x4A, 0x87, 0xE2), 3);
					text = SchedulerLocalizer.GetString(SchedulerStringId.Caption_Tentative);
					menuCaption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_Tentative);
					break;
				case AppointmentStatusType.WorkingElsewhere:
					brush = BrushHelper.CreateHatchBrushPercent75(Color.FromArgb(0xFF, 0x93, 0x7B, 0xD1), 1);
					text = SchedulerLocalizer.GetString(SchedulerStringId.Caption_WorkingElsewhere);
					menuCaption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_WorkingElsewhere);
					break;
				case AppointmentStatusType.Custom:
				default:
					brush = new SolidColorBrush(Colors.White);
					break;
			}
			AppointmentStatus status = new AppointmentStatus(ProvideDefaultId(), brush, type, text, menuCaption);
			return status;
		}
		protected internal override IAppointmentStatus CloneItem(IAppointmentStatus item) {
			AppointmentStatus source = (AppointmentStatus)item;
			AppointmentStatus target = new AppointmentStatus(source.Id, source.Brush.Clone(), source.Type, source.DisplayName, source.MenuCaption);
			target.Assign(source);
			return target;
		}
		protected internal override UserInterfaceObjectCollection<IAppointmentStatus> CreateDefaultContent() {
			AppointmentStatusCollection defaultContent = new AppointmentStatusCollection();
			IList types = Enum.GetValues(typeof(AppointmentStatusType));
			int count = types.Count;
			for (int i = 0; i < count; i++) {
				AppointmentStatusType itemType = (AppointmentStatusType)types[i];
				if (itemType != AppointmentStatusType.Custom)
					defaultContent.Add(defaultContent.CreateDefaultStatusByType(itemType));
			}
			return defaultContent;
		}
		public int Add(Color color, string displayName) {
			return Add(color, displayName, displayName);
		}
		public int Add(Color color, string displayName, string menuCaption) {
			return Add(null, AppointmentStatusType.Custom, displayName, menuCaption, new SolidColorBrush(color));
		}
		public int Add(AppointmentStatusType type, string displayName, string menuCaption, Brush brush) {
			return Add(null, type, displayName, menuCaption, brush);
		}
		public int Add(object id, AppointmentStatusType type, string displayName, string menuCaption, Brush brush) {
			AppointmentStatus status = (AppointmentStatus)CreateItem(id, displayName, menuCaption);
			status.Type = type;
			status.Brush = brush;
			return Add(status);
		}
		public AppointmentStatus GetByType(AppointmentStatusType type) {
			IAppointmentStatus status = this.FirstOrDefault(s => s.Type == type);
			if (status == null)
				status = AppointmentStatus.Empty;
			return (AppointmentStatus)status;
		}
		IAppointmentStatus IAppointmentStatusStorage.GetByType(AppointmentStatusType type) {
			return GetByType(type);
		}
		public AppointmentStatus CreateNewStatus(string displayName) {
			return CreateNewStatus(null, displayName, displayName);
		}
		public AppointmentStatus CreateNewStatus(object id, string displayName) {
			return CreateNewStatus(id, displayName, displayName);
		}
		public AppointmentStatus CreateNewStatus(object id, string displayName, string menuCaption) {
			AppointmentStatus status = (AppointmentStatus)CreateItem(id, displayName, menuCaption);
			status.Type = AppointmentStatusType.Custom;
			return status;
		}
		public AppointmentStatus CreateNewStatus(object id, string displayName, string menuCaption, Brush brush) {
			AppointmentStatus status = (AppointmentStatus)CreateItem(id, displayName, menuCaption);
			status.Brush = brush;
			return status;
		}
		IAppointmentStatus IAppointmentStatusStorage.CreateNewStatus(string displayName) {
			return CreateNewStatus(displayName);
		}
		IAppointmentStatus IAppointmentStatusStorage.CreateNewStatus(object id, string displayName) {
			return CreateNewStatus(id, displayName);
		}
		IAppointmentStatus IAppointmentStatusStorage.CreateNewStatus(object id, string displayName, string menuCaption) {
			return CreateNewStatus(id, displayName, menuCaption);
		}
		public void Dispose() {
			foreach (IAppointmentStatus item in this) {
				IDisposable disposableItem = item as IDisposable;
				if (disposableItem != null)
					disposableItem.Dispose();
			}
		}
	}
	#endregion
	public class SchedulerCustomFieldMappingCollection : LockableCollection<SchedulerCustomFieldMapping> {
		public SchedulerCustomFieldMappingCollection() {
		}
		protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if (e.Action == NotifyCollectionChangedAction.Add) {
				SubscribeItemsEvents(e.NewItems);
			}
			if (e.Action == NotifyCollectionChangedAction.Remove) {
				UnsubscribeItemsEvents(e.NewItems);
			}
			base.OnCollectionChanged(e);
		}
		protected virtual void SubscribeItemsEvents(IList items) {
			for (int i = 0; i < items.Count; i++) {
				INotifyPropertyChanged item = items[i] as INotifyPropertyChanged;
				if (item != null)
					item.PropertyChanged += OnItemPropertyChanged;
			}
		}
		protected virtual void UnsubscribeItemsEvents(IList items) {
			for (int i = 0; i < items.Count; i++) {
				INotifyPropertyChanged item = items[i] as INotifyPropertyChanged;
				if (item != null)
					item.PropertyChanged -= OnItemPropertyChanged;
			}
		}
		protected virtual void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
			base.OnPropertyChanged(new PropertyChangedEventArgs("Items"));
		}
	}
}
