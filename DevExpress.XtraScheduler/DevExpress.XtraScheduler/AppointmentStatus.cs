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
using System.Drawing.Drawing2D;
using System.Linq;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler.Internal;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Drawing.Design;
namespace DevExpress.XtraScheduler {
	public abstract class UserInterfaceObjectWin : UserInterfaceObject {
		private bool isDisposed = false;
		protected UserInterfaceObjectWin(object id, string displayName)
			: base(id, displayName) {
		}
		protected UserInterfaceObjectWin(object id, string displayName, string menuCaption)
			: base(id, displayName, menuCaption) {
		}
		internal Brush Brush { get; set; }
		protected internal override void Assign(UserInterfaceObject source) {
			base.Assign(source);
			UserInterfaceObjectWin sourceObject = source as UserInterfaceObjectWin;
			if (sourceObject == null)
				return;
			this.Brush = (Brush)sourceObject.Brush.Clone();
		}
		public void Dispose() {
			if (Brush != null) {
				Brush.Dispose();
				Brush = null;
			}
			isDisposed = true;
		}
		[Browsable(false)]
		public bool IsDisposed {
			get { return isDisposed; }
		}
	}
	#region AppointmentLabel
	public class AppointmentLabel : UserInterfaceObjectWin, IAppointmentLabel {
		protected internal AppointmentLabel(object id, Color color, string displayName, string menuCaption)
			: base(id, displayName, menuCaption) {
			Color = color;
			Brush = new SolidBrush(color);
		}
		[Obsolete("Use AppointmentLabelCollection.CreateNewLabel instead.", false)]
		public AppointmentLabel()
			: base(null, String.Empty) {
		}
		[Obsolete("Use AppointmentLabelCollection.CreateNewLabel instead.", false)]
		public AppointmentLabel(string displayName)
			: base(null, displayName) {
		}
		[Obsolete("Use AppointmentLabelCollection.CreateNewLabel instead.", false)]
		public AppointmentLabel(string displayName, string menuCaption)
			: base(null, displayName, menuCaption) {
		}
		[Obsolete("Use AppointmentLabelCollection.CreateNewLabel instead.", false)]
		public AppointmentLabel(Color color, string displayName)
			: base(null, displayName) {
			Color = color;
		}
		[Obsolete("Use AppointmentLabelCollection.CreateNewLabel instead.", false)]
		public AppointmentLabel(Color color, string displayName, string menuCaption)
			: base(null, displayName, menuCaption) {
			Color = color;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		int IAppointmentLabel.ColorValue { get; set; }
		[XtraSerializableProperty]
		public Color Color {
			get { return Color.FromArgb(((IAppointmentLabel)this).ColorValue); }
			set {
				int newColor = value.ToArgb();
				if (((IAppointmentLabel)this).ColorValue == newColor)
					return;
				int oldValue = ((IAppointmentLabel)this).ColorValue;
				((IAppointmentLabel)this).ColorValue = value.ToArgb();
				OnChanged("Color", oldValue, ((IAppointmentLabel)this).ColorValue);
				Brush oldBrush = Brush;
				Brush = new SolidBrush(value);
				if (oldBrush != null)
					oldBrush.Dispose();
			}
		}
	}
	#endregion
	#region AppointmentLabelCollection
	[ListBindable(BindableSupport.No)]
	[GeneratedCode("Suppress FxCop check", "")]
	[DesignerSerializer("DevExpress.XtraScheduler.Design.AppointmentLabelCollectionCodeDomSerializer, " + AssemblyInfo.SRAssemblySchedulerDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	public class AppointmentLabelCollection : UserInterfaceObjectWinCollection<IAppointmentLabel>, IAppointmentLabelStorage {
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use the GetById or GetByIndex methods instead.")]
		public new AppointmentLabel this[int index] {
			get { return GetByIndex(index); }
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
		protected IAppointmentLabel CreateItem(object id, Color labelColor, string displayName, string menuCaption) {
			AppointmentLabel label = (AppointmentLabel)base.CreateItem(id, displayName, menuCaption);
			label.Color = labelColor;
			return label;
		}
		protected internal override UserInterfaceObjectCollection<IAppointmentLabel> CreateDefaultContent() {
			AppointmentLabelCollection defaultContent = new AppointmentLabelCollection();
			defaultContent.Add(defaultContent.CreateItem(null, AppointmentLabelColor.None, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_None), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelNone)));
			defaultContent.Add(defaultContent.CreateItem(null, AppointmentLabelColor.Important, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_Important), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelImportant)));
			defaultContent.Add(defaultContent.CreateItem(null, AppointmentLabelColor.Business, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_Business), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelBusiness)));
			defaultContent.Add(defaultContent.CreateItem(null, AppointmentLabelColor.Personal, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_Personal), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelPersonal)));
			defaultContent.Add(defaultContent.CreateItem(null, AppointmentLabelColor.Vacation, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_Vacation), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelVacation)));
			defaultContent.Add(defaultContent.CreateItem(null, AppointmentLabelColor.MustAttend, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_MustAttend), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelMustAttend)));
			defaultContent.Add(defaultContent.CreateItem(null, AppointmentLabelColor.TravelRequired, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_TravelRequired), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelTravelRequired)));
			defaultContent.Add(defaultContent.CreateItem(null, AppointmentLabelColor.NeedsPreparation, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_NeedsPreparation), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelNeedsPreparation)));
			defaultContent.Add(defaultContent.CreateItem(null, AppointmentLabelColor.Birthday, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_Birthday), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelBirthday)));
			defaultContent.Add(defaultContent.CreateItem(null, AppointmentLabelColor.Anniversary, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_Anniversary), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelAnniversary)));
			defaultContent.Add(defaultContent.CreateItem(null, AppointmentLabelColor.PhoneCall, SchedulerLocalizer.GetString(SchedulerStringId.AppointmentLabel_PhoneCall), SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_AppointmentLabelPhoneCall)));
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
			foreach (IAppointmentLabel item in this)
				item.Dispose();
		}
	}
	#endregion
	#region AppointmentStatus
	public class AppointmentStatus : UserInterfaceObjectWin, IAppointmentStatus {
		private static AppointmentStatus empty = new AppointmentStatus(null, Color.Empty, AppointmentStatusType.Free, String.Empty, String.Empty);
		public static AppointmentStatus Empty {
			get { return empty; }
		}
		AppointmentStatusType type;
		protected internal AppointmentStatus(object id, Brush brush, AppointmentStatusType type, string displayName, string menuCaption)
			: base(id, displayName, menuCaption) {
			Type = type;
			Brush = brush;
		}
		protected internal AppointmentStatus(object id, Color color, AppointmentStatusType type, string displayName, string menuCaption)
			: base(id, displayName, menuCaption) {
			Type = type;
			Brush = new SolidBrush(color);
		}
		[Obsolete("Use AppointmentStatusCollection.CreateNewStatus instead.", false)]
		public AppointmentStatus()
			: base(null, String.Empty) {
			Brush = new SolidBrush(Color.White);
			Type = AppointmentStatusType.Custom;
		}
		[Obsolete("Use AppointmentStatusCollection.CreateNewStatus instead.", false)]
		public AppointmentStatus(AppointmentStatusType type, string displayName, string menuCaption)
			: base(null, displayName, menuCaption) {
			Type = type;
			Brush = new SolidBrush(Color.White);
		}
		[Obsolete("Use AppointmentStatusCollection.CreateNewStatus instead.", false)]
		public AppointmentStatus(AppointmentStatusType type, string displayName)
			: base(type, displayName) {
			Type = type;
			Brush = new SolidBrush(Color.White);
		}
		[Obsolete("Use AppointmentStatusCollection.CreateNewStatus instead.", false)]
		public AppointmentStatus(AppointmentStatusType type, Color color, string displayName)
			: base(type, displayName) {
			Type = type;
			Brush = new SolidBrush(color);
		}
		[Obsolete("Use AppointmentStatusCollection.CreateNewStatus instead.", false)]
		public AppointmentStatus(AppointmentStatusType type, Color color, string displayName, string menuCaption)
			: base(null, displayName, menuCaption) {
			Type = type;
			Brush = new SolidBrush(color);
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
		[XtraSerializableProperty]
		[Editor("DevExpress.XtraScheduler.Design.AppointmentStatusBrushUITypeEditor, " + AssemblyInfo.SRAssemblySchedulerDesign, "System.Drawing.Design.UITypeEditor")]
		public new Brush Brush {
			get { return base.Brush; }
			set {
				if (base.Brush == value)
					return;
				Brush oldBrush = base.Brush;
				base.Brush = value;
				OnChanged("Brush", oldBrush, value);
				if (oldBrush != null)
					oldBrush.Dispose();
			}
		}
		[Obsolete("Use the Brush property instead."), Browsable(false)]
		public Color Color {
			get {
				return SchedulerBrushHelper.GetColorFromAppointmentStatusType(Type, Brush);
			}
			set {
				Brush = SchedulerBrushHelper.GetBrushFromAppointmentStatusType(Type, value);
			}
		}
	}
	#endregion
	[ListBindable(BindableSupport.No)]
	[GeneratedCode("Suppress FxCop check", "")]
	[DesignerSerializer(" DevExpress.XtraScheduler.Design.AppointmentStatusCollectionCodeDomSerializer, " + AssemblyInfo.SRAssemblySchedulerDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	public class AppointmentStatusCollection : UserInterfaceObjectWinCollection<IAppointmentStatus>, IAppointmentStatusStorage {
		protected internal override object ProvideDefaultId() {
			return Count;
		}
		[Obsolete("Use the GetById, GetByIndex, or GetByType methods instead."), Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new AppointmentStatus this[int index] {
			get { return base[index] as AppointmentStatus; }
		}
		public new AppointmentStatus GetById(object id) {
			return (AppointmentStatus)base.GetById(id);
		}
		public new AppointmentStatus GetByIndex(int index) {
			return (AppointmentStatus)base.GetItem(index);
		}
		protected internal override IAppointmentStatus CreateItemInstance(object id, string displayName, string menuCaption) {
			return new AppointmentStatus(id, Color.White, AppointmentStatusType.Custom, displayName, menuCaption);
		}
		protected internal IAppointmentStatus CreateDefaultStatusByType(AppointmentStatusType type) {
			Brush brush = null;
			string text = String.Empty;
			string menuCaption = String.Empty;
			Color color = Color.White;
			switch (type) {
				case AppointmentStatusType.Free:
					color = Color.White;
					text = SchedulerLocalizer.GetString(SchedulerStringId.Caption_Free);
					menuCaption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_Free);
					break;
				case AppointmentStatusType.OutOfOffice:
					color = Color.FromArgb(0xD9, 0x53, 0x53);
					text = SchedulerLocalizer.GetString(SchedulerStringId.Caption_OutOfOffice);
					menuCaption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_OutOfOffice);
					break;
				case AppointmentStatusType.Busy:
					color = Color.FromArgb(0x4A, 0x87, 0xE2);
					text = SchedulerLocalizer.GetString(SchedulerStringId.Caption_Busy);
					menuCaption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_Busy);
					break;
				case AppointmentStatusType.Tentative:
					color = Color.FromArgb(0x4A, 0x87, 0xE2);
					text = SchedulerLocalizer.GetString(SchedulerStringId.Caption_Tentative);
					menuCaption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_Tentative);
					break;
				case AppointmentStatusType.WorkingElsewhere:
					color = Color.FromArgb(0x93, 0x7B, 0xD1);
					text = SchedulerLocalizer.GetString(SchedulerStringId.Caption_WorkingElsewhere);
					menuCaption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_WorkingElsewhere);
					break;
				case AppointmentStatusType.Custom:
				default:
					brush = new SolidBrush(Color.White);
					break;
			}
			brush = SchedulerBrushHelper.GetBrushFromAppointmentStatusType(type, color);
			return new AppointmentStatus(ProvideDefaultId(), brush, type, text, menuCaption);
		}
		protected internal override IAppointmentStatus CloneItem(IAppointmentStatus item) {
			AppointmentStatus source = (AppointmentStatus)item;
			AppointmentStatus target = new AppointmentStatus(source.Id, source.Brush, source.Type, source.DisplayName, source.MenuCaption);
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
			return Add(null, AppointmentStatusType.Custom, displayName, menuCaption, new SolidBrush(color));
		}
		public int Add(AppointmentStatusType type, string displayName, string menuCaption, Brush brush) {
			return Add(null, type, displayName, menuCaption, brush);
		}
		public int Add(AppointmentStatusType type, string displayName, string menuCaption, Color color) {
			Brush brush = SchedulerBrushHelper.GetBrushFromAppointmentStatusType(type, color);
			return this.Add(null, type, displayName, menuCaption, brush);
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
			foreach (IAppointmentStatus item in this)
				item.Dispose();
		}
	}
	public abstract class UserInterfaceObjectWinCollection<T> : UserInterfaceObjectCollection<T> where T : IUserInterfaceObject {
		internal Brush GetObjectBrush(T uiObject) {
			UserInterfaceObjectWin winUiObject = uiObject as UserInterfaceObjectWin;
			if (winUiObject == null)
				return null;
			return winUiObject.Brush;
		}
		internal Brush GetObjectBrush(int objectIndex) {
			return GetObjectBrush(this[objectIndex]);
		}
		internal void SetObjectBrush(Brush brush, T uiObject) {
			UserInterfaceObjectWin winUiObject = uiObject as UserInterfaceObjectWin;
			if (winUiObject != null)
				winUiObject.Brush = brush;
		}
		internal void SetObjectBrush(Brush brush, int objectIndex) {
			SetObjectBrush(brush, this[objectIndex]);
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
		public static Brush GetBrush(this IAppointmentStatus status) {
			AppointmentStatus statusObject = status as AppointmentStatus;
			if (statusObject != null)
				return statusObject.Brush;
			IBrushProvider brushProvider = status as IBrushProvider;
			if (brushProvider != null)
				return brushProvider.GetBrush();
			return null;
		}
		public static void SetBrush(this IAppointmentStatus status, Brush brush) {
			AppointmentStatus statusObject = status as AppointmentStatus;
			if (statusObject != null)
				statusObject.Brush = brush;
		}
	}
}
namespace DevExpress.XtraScheduler.Internal {
	public static class SchedulerBrushHelper {
		public static Brush GetBrushFromAppointmentStatusType(AppointmentStatusType type, Color color) {
			switch (type) {
				case AppointmentStatusType.Tentative:
					return new HatchBrush(HatchStyle.WideUpwardDiagonal, color, Color.White);
				case AppointmentStatusType.WorkingElsewhere:
					return new HatchBrush(HatchStyle.Percent75, Color.White, color);
				default:
					return new SolidBrush(color);
			}
		}
		public static Color GetColorFromAppointmentStatusType(AppointmentStatusType type, Brush brush) {
			HatchBrush hatchBrush = brush as HatchBrush;
			SolidBrush solidBrush = brush as SolidBrush;
			switch (type) {
				case AppointmentStatusType.Tentative:
					if (hatchBrush != null)
						return hatchBrush.ForegroundColor;
					if (solidBrush != null)
						return solidBrush.Color;
					return Color.White;
				case AppointmentStatusType.WorkingElsewhere:
					if (hatchBrush != null)
						return hatchBrush.BackgroundColor;
					if (solidBrush != null)
						return solidBrush.Color;
					return Color.White;
				default:
					if (solidBrush != null)
						return solidBrush.Color;
					return Color.White;
			}
		}
		public static Color GetColor(Brush brush) {
			SolidBrush solidBrush = brush as SolidBrush;
			if (solidBrush != null)
				return solidBrush.Color;
			HatchBrush hatchBrush = brush as HatchBrush;
			if (hatchBrush != null)
				return hatchBrush.BackgroundColor;
			return Color.White;
		}
	}
}
