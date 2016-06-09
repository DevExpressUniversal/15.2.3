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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.UI;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Localization;
namespace DevExpress.XtraScheduler.UI {
	#region RepositoryItemDuration
	[
	UserRepositoryItem("RegisterDurationEdit"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemDuration : RepositoryItemComboBox {
		static RepositoryItemDuration() { RegisterDurationEdit(); }
		bool showEmptyItem;
		public RepositoryItemDuration() {
			DisabledStateText = null;
			InitItems(TimeSpan.MaxValue);
		}
		public static void RegisterDurationEdit() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraScheduler.Bitmaps256.durationEdit.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(DurationEdit).Name, typeof(DurationEdit), typeof(RepositoryItemDuration), typeof(DevExpress.XtraEditors.ViewInfo.ComboBoxViewInfo), new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true, img, typeof(DevExpress.Accessibility.TextEditAccessible));
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		#region Properties
		internal static string InternalEditorTypeName { get { return typeof(DurationEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		#region Items
		[Localizable(false), Category(CategoryName.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraScheduler.Design.DurationItemCollectionEditor," + AssemblyInfo.SRAssemblySchedulerDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public override ComboBoxItemCollection Items { get { return base.Items; } }
		protected override bool ShouldSerializeItems() {
			DurationItemCollection defaultContent = new DurationItemCollection(this);
			PopulateItems(defaultContent, TimeSpan.MaxValue);
			int count = defaultContent.Count;
			if (count != Items.Count)
				return true;
			for (int i = 0; i < count; i++) {
				if (!defaultContent[i].Equals(Items[i]))
					return true;
			}
			return false;
		}
		#endregion
		public bool ShowEmptyItem {
			get { return showEmptyItem; }
			set {
				if (value == showEmptyItem)
					return;
				showEmptyItem = value;
				InitItems(TimeSpan.MaxValue);
			}
		}
		public string DisabledStateText { get; set; }
		#endregion
		protected override ComboBoxItemCollection CreateItemCollection() {
			return new DurationItemCollection(this);
		}
		protected internal virtual void InitItems(TimeSpan maxDuration) {
			BeginUpdate();
			try {
				PopulateItems(Items, maxDuration);
			} finally {
				EndUpdate();
			}
		}
		protected internal virtual void PopulateItems(ComboBoxItemCollection target, TimeSpan maxDuration) {
			target.Clear();
			int count = ReminderTimeSpans.ReminderTimeSpanValues.Length;
			if (ShowEmptyItem)
				target.Add(TimeSpan.MinValue);
			for (int i = 0; i < count; i++) {
				TimeSpan timeSpan = ReminderTimeSpans.ReminderTimeSpanValues[i];
				if (timeSpan <= maxDuration)
					target.Add(timeSpan);
			}
		}
		protected override ConvertEditValueEventArgs DoFormatEditValue(object val) {
			if (val is TimeSpan) {
				TimeSpan timeSpan = (TimeSpan)val;
				if (timeSpan == TimeSpan.MinValue && ShowEmptyItem)
					return new ConvertEditValueEventArgs(SchedulerLocalizer.GetString(SchedulerStringId.Caption_NoneReminder));
				return new ConvertEditValueEventArgs(HumanReadableTimeSpanHelper.ToString(timeSpan));
			}
			return base.DoFormatEditValue(val);
		}
		public override void BeginInit() {
			base.BeginInit();
			Items.Clear();
		}
		public override void EndInit() {
			base.EndInit();
			if (Items.Count <= 0)
				InitItems(TimeSpan.MaxValue);
		}
		public override void Assign(RepositoryItem item) {
			base.Assign(item);
			RepositoryItemDuration repositoryItemDuration = item as RepositoryItemDuration;
			if (repositoryItemDuration == null)
				return;
			this.ShowEmptyItem = repositoryItemDuration.ShowEmptyItem;
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			if (DisabledStateText == null || OwnerEdit.Enabled)
				return base.GetDisplayText(format, editValue);
			return DisabledStateText;
		}
	}
	#endregion
	#region DurationEdit
	[
	DXToolboxItem(DXToolboxItemKind.Regular),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerControl), DevExpress.Utils.ControlConstants.BitmapPath + "durationEdit.bmp"),
	System.Runtime.InteropServices.ComVisible(false),
	Designer("DevExpress.XtraScheduler.Design.XtraSchedulerSuiteComboBoxEditDesigner," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("A combo box control used to specify time intervals (durations).")
	]
	public class DurationEdit : ComboBoxEdit {
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeSpan Duration {
			get {
				if (EditValue is TimeSpan)
					return (TimeSpan)EditValue;
				else if (EditValue is String)
					return HumanReadableTimeSpanHelper.Parse((String)EditValue);
				else
					return TimeSpan.MinValue;
			}
			set {
				EditValue = value;
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DurationEditProperties"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemDuration Properties { get { return base.Properties as RepositoryItemDuration; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("DurationEditEditorTypeName")]
#endif
		public override string EditorTypeName { get { return GetType().Name; } }
		#endregion
		#region Events
		#region DurationChanged
		static object onDurationChanged = new object();
		public event EventHandler DurationChanged {
			add { Events.AddHandler(onDurationChanged, value); }
			remove { Events.RemoveHandler(onDurationChanged, value); }
		}
		void RaiseDurationChanged() {
			EventHandler handler = Events[onDurationChanged] as EventHandler;
			if (handler == null)
				return;
			handler(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		public void LoadDefaults() {
			Properties.InitItems(TimeSpan.MaxValue);
		}
		public void LoadDefaults(TimeSpan maxDuration) {
			Properties.InitItems(maxDuration);
		}
		static DurationEdit() {
			RepositoryItemDuration.RegisterDurationEdit();
		}
		protected override void OnValidated(EventArgs e) {
			TimeSpan duration = Duration;
			if (Duration != TimeSpan.MinValue)
				EditValue = duration;
			base.OnValidated(e);
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			RaiseDurationChanged();
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	#region DurationItemCollection
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1039:ListsAreStronglyTyped")]
	public class DurationItemCollection : ComboBoxItemCollection {
		public DurationItemCollection(RepositoryItemDuration properties)
			: base(properties) {
		}
		public new RepositoryItemDuration Properties { get { return (RepositoryItemDuration)base.Properties; } }
		public override string GetItemDescription(object item) {
			if (item is TimeSpan) {
				TimeSpan timeSpan = (TimeSpan)item;
				if (timeSpan == TimeSpan.MinValue && Properties.ShowEmptyItem)
					return SchedulerLocalizer.GetString(SchedulerStringId.Caption_NoneReminder);
				return HumanReadableTimeSpanHelper.ToString(timeSpan);
			} else
				return base.GetItemDescription(item);
		}
	}
	#endregion
}
