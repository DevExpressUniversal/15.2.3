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
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.Utils;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Internal;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.Web.ASPxScheduler {
	public abstract class UserInterfaceObjectAsp : UserInterfaceObject {
		private bool isDisposed = false;
		protected UserInterfaceObjectAsp(object id, string displayName)
			: base(id, displayName) {
		}
		protected UserInterfaceObjectAsp(object id, string displayName, string menuCaption)
			: base(id, displayName, menuCaption) {
		}
		internal Color Color { get; set; }
		[Browsable(false)]
		public bool IsDisposed {
			get { return isDisposed; }
		}
		protected internal override void Assign(UserInterfaceObject source) {
			base.Assign(source);
			UserInterfaceObjectAsp sourceObject = source as UserInterfaceObjectAsp;
			if (sourceObject == null)
				return;
			this.Color = sourceObject.Color;
		}
	}
	public class AppointmentLabel : UserInterfaceObjectAsp, IAppointmentLabel {
		protected internal AppointmentLabel(object id, Color color, string displayName, string menuCaption)
			: base(id, displayName, menuCaption) {
			Color = color;
		}
		public AppointmentLabel()
			: base(null, String.Empty) {
		}
		[Obsolete("Use CreateNewLabel instead.", false)]
		public AppointmentLabel(string displayName)
			: base(null, displayName) {
		}
		[Obsolete("Use CreateNewLabel instead.", false)]
		public AppointmentLabel(string displayName, string menuCaption)
			: base(null, displayName, menuCaption) {
		}
		[Obsolete("Use CreateNewLabel instead.", false)]
		public AppointmentLabel(Color color, string displayName)
			: base(null, displayName) {
			Color = color;
		}
		[Obsolete("Use CreateNewLabel instead.", false)]
		public AppointmentLabel(Color color, string displayName, string menuCaption)
			: base(null, displayName, menuCaption) {
			Color = color;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		int IAppointmentLabel.ColorValue { get; set; }
		public new Color Color {
			get { return Color.FromArgb(((IAppointmentLabel)this).ColorValue); }
			set {
				int newColor = value.ToArgb();
				if (((IAppointmentLabel)this).ColorValue == newColor)
					return;
				int oldValue = ((IAppointmentLabel)this).ColorValue;
				((IAppointmentLabel)this).ColorValue = value.ToArgb();
				OnChanged("Color", oldValue, ((IAppointmentLabel)this).ColorValue);
			}
		}
		public void Dispose() {
		}
	}
	[ListBindable(BindableSupport.No)]
	[GeneratedCode("Suppress FxCop check", "")]
	public class AppointmentLabelCollection : UserInterfaceObjectCollection<IAppointmentLabel>, IAppointmentLabelStorage {
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use the GetById or GetByIndex methods instead.")]
		public new AppointmentLabel this[int index] {
			get { return GetByIndex(index); }
		}
		protected override bool IsIndexCacheEnabled {
			get { return true; }
		}
		public new AppointmentLabel GetById(object id) {
			return (AppointmentLabel)base.GetById(id);
		}
		public new AppointmentLabel GetByIndex(int index) {
			return (AppointmentLabel)base.GetItem(index);
		}
		protected internal override IAppointmentLabel CreateItemInstance(object id, string displayName, string menuCaption) {
			return new AppointmentLabel(id, Color.White, displayName, menuCaption);
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
			defaultContent.Add(defaultContent.CreateItem(AppointmentLabelColor.None, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_None), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelNone)));
			defaultContent.Add(defaultContent.CreateItem(AppointmentLabelColor.Important, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_Important), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelImportant)));
			defaultContent.Add(defaultContent.CreateItem(AppointmentLabelColor.Business, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_Business), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelBusiness)));
			defaultContent.Add(defaultContent.CreateItem(AppointmentLabelColor.Personal, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_Personal), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelPersonal)));
			defaultContent.Add(defaultContent.CreateItem(AppointmentLabelColor.Vacation, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_Vacation), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelVacation)));
			defaultContent.Add(defaultContent.CreateItem(AppointmentLabelColor.MustAttend, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_MustAttend), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelMustAttend)));
			defaultContent.Add(defaultContent.CreateItem(AppointmentLabelColor.TravelRequired, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_TravelRequired), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelTravelRequired)));
			defaultContent.Add(defaultContent.CreateItem(AppointmentLabelColor.NeedsPreparation, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_NeedsPreparation), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelNeedsPreparation)));
			defaultContent.Add(defaultContent.CreateItem(AppointmentLabelColor.Birthday, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_Birthday), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelBirthday)));
			defaultContent.Add(defaultContent.CreateItem(AppointmentLabelColor.Anniversary, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_Anniversary), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelAnniversary)));
			defaultContent.Add(defaultContent.CreateItem(AppointmentLabelColor.PhoneCall, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_PhoneCall), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelPhoneCall)));
			return defaultContent;
		}
		protected internal override IAppointmentLabel CloneItem(IAppointmentLabel item) {
			AppointmentLabel source = (AppointmentLabel)item;
			AppointmentLabel target = new AppointmentLabel(source.Id, source.Color, source.DisplayName, source.MenuCaption);
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
		public int Add(string displayName, string menuCaption, Color labelColor) {
			return Add(null, displayName, menuCaption, labelColor);
		}
		public int Add(object id, string displayName, string menuCaption, Color labelColor) {
			AppointmentLabel label = (AppointmentLabel)CreateItem(id, displayName, menuCaption);
			label.Color = labelColor;
			return Add(label);
		}
		public AppointmentLabel CreateNewLabel(string displayName) {
			return CreateNewLabel(displayName, displayName);
		}
		public AppointmentLabel CreateNewLabel(object id, string displayName) {
			return CreateNewLabel(null, displayName, displayName);
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
	public class AppointmentStatus : UserInterfaceObjectAsp, IAppointmentStatus, IBrushProvider {
		private static AppointmentStatus empty = new AppointmentStatus(null);
		public static AppointmentStatus Empty {
			get { return empty; }
		}
		AppointmentStatusType type;
		Brush colorBrush = null;
		public AppointmentStatus()
			: base(null, string.Empty) {
			this.type = AppointmentStatusType.Custom;
		}
		internal AppointmentStatus(object id)
			: this(id, AppointmentStatusType.Custom, string.Empty) {
		}
		internal AppointmentStatus(object id, AppointmentStatusType type, string displayName)
			: this(id, type, displayName, displayName) {
		}
		internal AppointmentStatus(object id, AppointmentStatusType type, string displayName, string menuCaption)
			: base(id, displayName, menuCaption) {
			Type = type;
			Color = Color.White;
		}
		protected internal override void Assign(UserInterfaceObject source) {
			base.Assign(source);
			AppointmentStatus sourceStatus = source as AppointmentStatus;
			if (sourceStatus == null)
				return;
			this.Type = sourceStatus.Type;
		}
		[Browsable(false)]
		public AppointmentStatusType Type {
			get { return type; }
			set {
				if (type == value)
					return;
				AppointmentStatusType oldType = type;
				type = value;
				OnChanged("Type", oldType, value);
			}
		}
		public new Color Color {
			get { return base.Color; }
			set {
				if (base.Color == value)
					return;
				Color oldColor = base.Color;
				base.Color = value;
				OnChanged("Color", oldColor, value);
			}
		}
		public void Dispose() {
			if (colorBrush != null) {
				colorBrush.Dispose();
				colorBrush = null;
			}
		}
		Brush IBrushProvider.GetBrush() {
			if (colorBrush == null)
				colorBrush = new SolidBrush(Color);
			return colorBrush;
		}
	}
	[ListBindable(BindableSupport.No)]
	[GeneratedCode("Suppress FxCop check", "")]
	public class AppointmentStatusCollection : UserInterfaceObjectCollection<IAppointmentStatus>, IAppointmentStatusStorage {
		[Obsolete("Use the GetById, GetByIndex, or GetByType methods instead"), Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new AppointmentStatus this[int index] {
			get { return GetByIndex(index); }
		}
		protected override bool IsIndexCacheEnabled {
			get { return true; }
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
			return new AppointmentStatus(id, AppointmentStatusType.Custom, displayName, menuCaption);
		}
		protected internal IAppointmentStatus CreateDefaultStatusByType(AppointmentStatusType type) {
			Color color = Color.Empty;
			string text = String.Empty;
			string menuCaption = String.Empty;
			switch (type) {
				case AppointmentStatusType.Free:
					color = DXColor.White;
					text = SchedulerLocalizer.GetString(SchedulerStringId.Caption_Free);
					menuCaption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_Free);
					break;
				case AppointmentStatusType.OutOfOffice:
					color = DXColor.FromArgb(0xD9, 0x53, 0x53);
					text = SchedulerLocalizer.GetString(SchedulerStringId.Caption_OutOfOffice);
					menuCaption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_OutOfOffice);
					break;
				case AppointmentStatusType.Busy:
					color = DXColor.FromArgb(0x4A, 0x87, 0xE2);
					text = SchedulerLocalizer.GetString(SchedulerStringId.Caption_Busy);
					menuCaption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_Busy);
					break;
				case AppointmentStatusType.Tentative:
					color = DXColor.FromArgb(0x63, 0xC6, 0x4C);
					text = SchedulerLocalizer.GetString(SchedulerStringId.Caption_Tentative);
					menuCaption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_Tentative);
					break;
				case AppointmentStatusType.WorkingElsewhere:
					color = DXColor.FromArgb(0x93, 0x7B, 0xD1);
					text = SchedulerLocalizer.GetString(SchedulerStringId.Caption_WorkingElsewhere);
					menuCaption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_WorkingElsewhere);
					break;
				case AppointmentStatusType.Custom:
				default:
					break;
			}
			AppointmentStatus status = new AppointmentStatus(ProvideDefaultId(), type, text, menuCaption);
			status.Color = color;
			return status;
		}
		protected internal override IAppointmentStatus CloneItem(IAppointmentStatus item) {
			AppointmentStatus source = (AppointmentStatus)item;
			AppointmentStatus target = new AppointmentStatus(source.Id);
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
			return Add(null, AppointmentStatusType.Custom, displayName, menuCaption, color);
		}
		public int Add(AppointmentStatusType type, string displayName, string menuCaption, Color color) {
			return Add(null, type, displayName, menuCaption, color);
		}
		public int Add(object id, AppointmentStatusType type, string displayName, string menuCaption, Color color) {
			AppointmentStatus status = (AppointmentStatus)CreateItem(id, displayName, menuCaption);
			status.Type = type;
			status.Color = color;
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
			return CreateNewStatus(displayName, displayName);
		}
		public AppointmentStatus CreateNewStatus(string displayName, string menuCaption) {
			return CreateNewStatus(null, displayName, menuCaption);
		}
		public AppointmentStatus CreateNewStatus(object id, string displayName, string menuCaption) {
			AppointmentStatus status = (AppointmentStatus)CreateItem(id, displayName, menuCaption);
			status.Type = AppointmentStatusType.Custom;
			return status;
		}
		public AppointmentStatus CreateNewStatus(object id, string displayName, string menuCaption, Color color) {
			AppointmentStatus status = (AppointmentStatus)CreateItem(id, displayName, menuCaption);
			status.Color = color;
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
		public IAppointmentStatus CreateNewStatus(object id, string displayName) {
			return CreateNewStatus(id, displayName, displayName);
		}
		public void Dispose() {
			foreach (IAppointmentStatus item in this) {
				IDisposable disposableItem = item as IDisposable;
				if (disposableItem != null)
					disposableItem.Dispose();
			}
		}
	}
	public static class AppointmentLabelExtension {
		public static Color GetColor(this IAppointmentLabel label) {
			AppointmentLabel labelObject = label as AppointmentLabel;
			if (labelObject != null)
				return labelObject.Color;
			return Color.FromArgb(label.ColorValue);
		}
		public static void SetColor(this IAppointmentLabel label, Color color) {
			AppointmentLabel labelObject = label as AppointmentLabel;
			if (labelObject != null)
				labelObject.Color = color;
			else
				label.ColorValue = color.ToArgb();
		}
	}
	public static class AppointmentStatusExtension {
		public static Color GetColor(this IAppointmentStatus status) {
			AppointmentStatus statusObject = status as AppointmentStatus;
			if (statusObject == null)
				return Color.Empty;
			return statusObject.Color;
		}
		public static void SetColor(this IAppointmentStatus status, Color color) {
			AppointmentStatus statusObject = status as AppointmentStatus;
			if (statusObject != null)
				statusObject.Color = color;
		}
	}
}
