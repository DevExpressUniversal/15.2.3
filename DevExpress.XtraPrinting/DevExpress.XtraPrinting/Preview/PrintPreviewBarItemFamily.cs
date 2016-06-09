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
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraPrinting.Preview.Native;
using DevExpress.XtraEditors;
using System.Collections.Generic;
using DevExpress.LookAndFeel;
namespace DevExpress.XtraPrinting.Native {
	using DevExpress.XtraPrinting.Preview;
	using System.Collections.Generic;
	using System.Collections;
	public static class ContextHelper {
		static bool ItemHasSameContext<T>(object item, object contextSpecifier) {
			return typeof(T).IsInstanceOfType(item) && item is ISupportContextSpecifier && ((ISupportContextSpecifier)item).HasSameContext(contextSpecifier);
		}
		public static IEnumerable<T> GetSameContextEnumerable<T>(ICollection items, object contextSpecifier) {
			if(items == null)
				yield break;
			foreach(object item in items) {
				if(ItemHasSameContext<T>(item, contextSpecifier))
					yield return (T)item;
			}
		}
	}
}
namespace DevExpress.XtraPrinting.Preview
{
	public interface IStatusPanel {
		StatusPanelID StatusPanelID { get ; }
	}
	public interface ISupportContextSpecifier {
		object ContextSpecifier { get; set; }
		bool HasSameContext(object contextSpecifier);
	}
	public interface ISupportPrintingSystemCommand : ISupportContextSpecifier {
		PrintingSystemCommand Command { get; }
	}
	public interface ISupportParametrizedPrintingSystemCommand : ISupportPrintingSystemCommand {
		object CommandParameter { get; }
	}
	public class PreviewBar : Bar {
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public override string Text { get { return base.Text; } set { base.Text = value; }
		}
	}
	public class PrintPreviewBarCheckItem : DevExpress.XtraBars.BarCheckItem, ISupportPrintingSystemCommand {
		PrintingSystemCommand command = PrintingSystemCommand.None;
		#region ISupportContextSpecifier stub
		bool ISupportContextSpecifier.HasSameContext(object contextSpecifier) {
			return true;
		}
		object ISupportContextSpecifier.ContextSpecifier {
			get { return null; }
			set {}
		}
		#endregion
		public PrintPreviewBarCheckItem(PrintingSystemCommand command)
			: this() {
			this.command = command;
		}
		public PrintPreviewBarCheckItem(string caption, PrintingSystemCommand command): this(command) {
			this.Caption = caption;
		}
		public PrintPreviewBarCheckItem(string caption, PrintingSystemCommand command, bool check): this(caption, command) {
			this.Checked = check;
		}
		public PrintPreviewBarCheckItem(): base() {
		}
		[
		DefaultValue(PrintingSystemCommand.None)
		]
		public virtual PrintingSystemCommand Command { get { return command; } set { command = value; } 
		}
	}
#if DEBUGTEST
	[System.Diagnostics.DebuggerDisplay(@"\{{GetType().FullName,nq}, Command = {Command}}")]
#endif
	public class PrintPreviewBarItem : DevExpress.XtraBars.BarButtonItem, ISupportParametrizedPrintingSystemCommand {
		#region ISupportContextSpecifier
		object contextSpecifier;
		bool ISupportContextSpecifier.HasSameContext(object contextSpecifier) {
			return this.ContextSpecifier == null || this.ContextSpecifier == contextSpecifier;
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null)]
		public object ContextSpecifier {
			get { return contextSpecifier; }
			set { contextSpecifier = value; }
		}
		#endregion
		PrintingSystemCommand command = PrintingSystemCommand.None;
		object commandParameter;
		[
		DefaultValue(PrintingSystemCommand.None)
		]
		public virtual PrintingSystemCommand Command {
			get { return command; }
			set { command = value;
			if(command == PrintingSystemCommand.Find)
				this.ButtonStyle = BarButtonStyle.Check;
			}
		}
		object ISupportParametrizedPrintingSystemCommand.CommandParameter { get { return commandParameter; } } 
		public PrintPreviewBarItem(PrintingSystemCommand command): this() {
			this.command = command; 
		}
		public PrintPreviewBarItem(string caption, PrintingSystemCommand command): this(command) {
			this.Caption = caption;
		}
		internal PrintPreviewBarItem(string caption, PrintingSystemCommand command, object commandParameter): this(caption, command) {
			this.commandParameter = commandParameter;
		}
		public PrintPreviewBarItem(): base() {
		}
	}
	public class RuntimePrintPreviewBarItem : PrintPreviewBarItem {
		internal RuntimePrintPreviewBarItem(string caption, PrintingSystemCommand command, object commandParameter)
			: base(caption, command, commandParameter) {
		}
	}
	public class PrintPreviewSubItem : DevExpress.XtraBars.BarSubItem, ISupportPrintingSystemCommand {
		#region ISupportContextSpecifier
		object contextSpecifier;
		bool ISupportContextSpecifier.HasSameContext(object contextSpecifier) {
			return this.ContextSpecifier == null || this.ContextSpecifier == contextSpecifier;
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null)]
		public object ContextSpecifier {
			get { return contextSpecifier; }
			set { contextSpecifier = value; }
		}
		#endregion
		PrintingSystemCommand command = PrintingSystemCommand.None;
		public PrintPreviewSubItem(PrintingSystemCommand command)
			: this() {
			this.command = command;
		}
		public PrintPreviewSubItem() : base() {
		}
		[
		DefaultValue(PrintingSystemCommand.None)
		]
		public virtual PrintingSystemCommand Command {
			get { return command; }
			set { command = value; }
		}
	}
	public class PrintPreviewStaticItem : DevExpress.XtraBars.BarStaticItem, IStatusPanel {
		private StatusPanelID statusPanelID = StatusPanelID.None;
		[
		TypeConverter("DevExpress.XtraPrinting.Design.PreviewStatusPanelIDConverter," + AssemblyInfo.SRAssemblyPrintingDesign),
		DefaultValue(StatusPanelID.None),
		]
		public string Type {
			get { return statusPanelID.ToString(); }
			set {
				try {
					statusPanelID = (StatusPanelID)Enum.Parse(typeof(StatusPanelID), value);
				} catch {
					statusPanelID = StatusPanelID.None;
				}
			}
		}
		internal protected StatusPanelID StatusPanelID { get { return statusPanelID; } }
		public PrintPreviewStaticItem(string type) : base() {
			Type = type;
		}
		public PrintPreviewStaticItem(StatusPanelID statusPanelID) : base() {
			this.statusPanelID = statusPanelID;
		}
		public PrintPreviewStaticItem()	: base() {
		}
		#region IStatusPanel Members
		StatusPanelID IStatusPanel.StatusPanelID {
			get { return statusPanelID; }
		}
		#endregion
	}
	public class ZoomComboBoxItemBase {
		private string text;
		public ZoomComboBoxItemBase(string text) {
			this.text = text;
		}
		public override string ToString() {
			return text;
		}
	}
	public class ProgressBarEditItem : DevExpress.XtraBars.BarEditItem, ISupportContextSpecifier, IStatusPanel {
		public static ProgressBarEditItem CreateInstance(int width, int editHeight, BarItemVisibility visibility) {
			ProgressBarEditItem barItem = new ProgressBarEditItem();
			barItem.EditHeight = editHeight;
			barItem.Width = width;
			barItem.Visibility = visibility;
			RepositoryItemProgressBar repositoryItem = new RepositoryItemProgressBar();
			((System.ComponentModel.ISupportInitialize)(repositoryItem)).BeginInit();
			barItem.Edit = repositoryItem;
			((System.ComponentModel.ISupportInitialize)(repositoryItem)).EndInit();
			return barItem;
		}
		internal RepositoryItemProgressBar RepositoryItem {
			get { return this.Edit as RepositoryItemProgressBar; }
		}
		public ProgressBarEditItem() {
		}
		#region ISupportContextSpecifier
		object contextSpecifier;
		bool ISupportContextSpecifier.HasSameContext(object contextSpecifier) {
			return this.ContextSpecifier == null || this.ContextSpecifier == contextSpecifier;
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null)]
		public object ContextSpecifier {
			get { return contextSpecifier; }
			set { contextSpecifier = value; }
		}
		#endregion
		#region IStatusPanel Members
		StatusPanelID IStatusPanel.StatusPanelID {
			get { return StatusPanelID.Progress; }
		}
		#endregion
	}
	public class ZoomTrackBarEditItem : DevExpress.XtraBars.BarEditItem, ISupportPrintingSystemCommand {
		public static readonly int[] DefaultRange = new int[] {10, 500};
		public static ZoomTrackBarEditItem CreateInstance(int width) {
			ZoomTrackBarEditItem barItem = new ZoomTrackBarEditItem();
			RepositoryItemZoomTrackBar repositoryItem = new RepositoryItemZoomTrackBar();
			((System.ComponentModel.ISupportInitialize)(repositoryItem)).BeginInit();
			repositoryItem.ScrollThumbStyle = DevExpress.XtraEditors.Repository.ScrollThumbStyle.ArrowDownRight;
			repositoryItem.Maximum = 180;
			repositoryItem.AllowFocused = false;
			repositoryItem.BorderStyle = BorderStyles.NoBorder;
			repositoryItem.Alignment = DevExpress.Utils.VertAlignment.Center;
			barItem.Alignment = BarItemLinkAlignment.Right;
			barItem.Edit = repositoryItem;
			((System.ComponentModel.ISupportInitialize)(repositoryItem)).EndInit();
			barItem.Width = width;
			return barItem;
		}
		bool locked;
		internal bool Locked {
			get { return locked; }
			set { locked = value; }
		}
		CommandExecuteDelegate commandExecuter;
		int[] range = DefaultRange;
		public int[] Range {
			get { return range; }
			set {
				if(value.Length != 2 || value[0] >= value[1])
					throw new ArgumentException("value");
				range = value;
			}
		}
		internal CommandExecuteDelegate CommandExecuter {
			get { return commandExecuter; }
			set { commandExecuter = value; }
		}
		internal RepositoryItemZoomTrackBar RepositoryItem {
			get { return this.Edit as RepositoryItemZoomTrackBar; }
		}
		public ZoomTrackBarEditItem() {
		}
		protected override void OnItemChanged(bool onlyInvalidate) {
			base.OnItemChanged(onlyInvalidate);
			if(this.RepositoryItem != null) {
				this.RepositoryItem.ValueChanged -= OnEditValueChanged;
				this.RepositoryItem.ValueChanged += OnEditValueChanged;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.RepositoryItem != null) {
					this.RepositoryItem.ValueChanged -= OnEditValueChanged;
				}
			}
			base.Dispose(disposing);
		}
		protected override void OnEditValueChanged() {
			if(allowSetZoomBarValueForCodedUI) { 
				OnEditValueChangedCore((int)EditValue);
			}
			base.OnEditValueChanged();
		}
		void OnEditValueChanged(object sender, EventArgs e) {
			OnEditValueChangedCore(((ZoomTrackBarControl)sender).Value);
		}
		private void OnEditValueChangedCore(int value) {
			if(commandExecuter == null)
				return;
			Locked = true;
			try {
				float zoomFactor = ValueToZoom(value) / 100f;
				commandExecuter(PrintingSystemCommand.ZoomTrackBar, new object[] { zoomFactor });
			}
			finally {
				Locked = false;
			}
		}
		bool allowSetZoomBarValueForCodedUI = true;
		public void ApplyZoom(float zoom) {
			try {
				allowSetZoomBarValueForCodedUI = false;
				if(this.RepositoryItem != null && !Locked)
					this.EditValue = ZoomToValue(zoom * 100);
			}
			finally {
				allowSetZoomBarValueForCodedUI = true;
			}
		}
		int ZoomToValue(float zoom) {
			float result = zoom <= 100 ? (zoom - range[0]) * 90 / (100 - range[0]):
				90 + ((zoom - 100) * 90 / (range[1] - 100));
			return Math.Max(this.RepositoryItem.Minimum, Math.Min(this.RepositoryItem.Maximum, Convert.ToInt32(result)));
		}
		float ValueToZoom(float value) {
			return value <= 90 ? range[0] + value * (100f - range[0]) / 90f :
				100f + (value - 90f) * (range[1] - 100f) / 90f;
		}
		#region ISupportContextSpecifier
		object contextSpecifier;
		bool ISupportContextSpecifier.HasSameContext(object contextSpecifier) {
			return this.ContextSpecifier == null || this.ContextSpecifier == contextSpecifier;
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null)]
		public object ContextSpecifier {
			get { return contextSpecifier; }
			set { contextSpecifier = value; }
		}
		#endregion
		#region ISupportPrintingSystemCommand Members
		PrintingSystemCommand ISupportPrintingSystemCommand.Command {
			get { return PrintingSystemCommand.ZoomTrackBar; }
		}
		#endregion
	}
	public abstract class ZoomBarEditItemBase : DevExpress.XtraBars.BarEditItem {
		#region static
		static bool IsNativeZoomItem(object item) {
			return item is ZoomComboBoxItem;
		}
		#endregion
		bool showMessageOnInvalidInput = true;
		ZoomEditController editController;
		bool isMessageBoxShown;
		protected abstract int MinZoomFactor { get; }
		protected abstract int MaxZoomFactor { get; }
		protected abstract string ZoomStringFormat { get; }
		protected abstract float CurrentZoom { get; }
		protected ZoomBarEditItemBase() {
			editController = new ZoomEditController(MinZoomFactor, MaxZoomFactor, ZoomStringFormat);
		}
		public override object EditValue {
			get { return base.EditValue; }
			set {
				if(!CanExecZoomCommand()) 
					return;
				if(IsNativeZoomItem(value)) {
					SetBaseEditValue(value);
					return;
				}
				string message = string.Empty;
				if(!editController.IsValidZoomValue(value, ref message)) {
					if(!isMessageBoxShown) {
						isMessageBoxShown = true;
						ShowMessageBox(message);
						isMessageBoxShown = false;
					}
					return;
				}
				ExecZoomCommand(value);
				SetBaseEditValue(editController.GetDigits(value) + "%");
			}	
		}
		public void Init() {
			if(Edit is RepositoryItemComboBox) {
				RepositoryItemComboBox comboBox = (RepositoryItemComboBox)Edit;
				if(comboBox.Items.Count > 0) return;
				comboBox.Items.AddRange(CreateItems());
				comboBox.DropDownRows = comboBox.Items.Count;
			}
		}
		internal void SetBaseEditValue(object value) {
			base.EditValue = value;
		}
		protected abstract void ApplyNewEditValue();
		protected abstract bool CanExecZoomCommand();
		protected abstract void ExecZoomCommandCore(float zoomFactor);
		protected abstract object[] CreateItems();
		protected void ExecZoomCommand(object input) {
			if(CanExecZoomCommand())
				ExecZoomCommandCore(editController.GetZoomValue(input));
		}
		protected object GetEditValue() {
			return CanExecZoomCommand() ? string.Format(ZoomStringFormat, CurrentZoom * 100) :
				string.Empty;
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			if(CanExecZoomCommand())
				ApplyNewEditValue();
		}
		void ShowMessageBox(string text) {
			if(showMessageOnInvalidInput) {
				Form form = Manager.Form.FindForm();
				string caption = form != null ? form.Text : PreviewStringId.Msg_Caption.GetString();
				NotificationService.ShowMessage<PrintingSystemBase>(((ISupportLookAndFeel)Manager).LookAndFeel, form, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}
	}
	public class ZoomComboBoxItem : ZoomComboBoxItemBase, ISupportPrintingSystemCommand {
		private PrintingSystemCommand command;
		#region ISupportContextSpecifier stub
		bool ISupportContextSpecifier.HasSameContext(object contextSpecifier) {
			return true;
		}
		object ISupportContextSpecifier.ContextSpecifier {
			get { return null; }
			set { }
		}
		#endregion
		public PrintingSystemCommand Command {
			get { return command; }
		}
		public ZoomComboBoxItem(string text, PrintingSystemCommand command) : base(text) {
			this.command = command;
		}
	}
	public class ZoomBarEditItem : ZoomBarEditItemBase, ISupportPrintingSystemCommand {
		#region ISupportContextSpecifier
		object contextSpecifier;
		public bool HasSameContext(object contextSpecifier) {
			return this.ContextSpecifier == null || this.ContextSpecifier == contextSpecifier;
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null)]
		public object ContextSpecifier {
			get { return contextSpecifier; }
			set { contextSpecifier = value; }
		}
		#endregion
		PrintControl printControl;
		protected override int MinZoomFactor {
			get { return 10; } 
		}
		protected override int MaxZoomFactor { get { return 500; } 
		}
		protected override string ZoomStringFormat { get { return PreviewItemsLogicBase.ZoomStringFormat; } 
		}
		protected override float CurrentZoom { get { return printControl.Zoom; } 
		}
		PrintingSystemCommand ISupportPrintingSystemCommand.Command { get { return PrintingSystemCommand.Zoom; }
		}
		public ZoomBarEditItem() {
		}
		protected override object[] CreateItems() {
			return new object[] {			
									new ZoomComboBoxItem("500%", PrintingSystemCommand.Zoom),	
									new ZoomComboBoxItem("200%", PrintingSystemCommand.Zoom),	
									new ZoomComboBoxItem("150%", PrintingSystemCommand.Zoom),	
									new ZoomComboBoxItem("100%", PrintingSystemCommand.Zoom),	
									new ZoomComboBoxItem("75%", PrintingSystemCommand.Zoom),
									new ZoomComboBoxItem("50%", PrintingSystemCommand.Zoom),
									new ZoomComboBoxItem("25%", PrintingSystemCommand.Zoom),
									new ZoomComboBoxItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_ZoomPageWidth), PrintingSystemCommand.ZoomToPageWidth),
									new ZoomComboBoxItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_ZoomTextWidth), PrintingSystemCommand.ZoomToTextWidth),
									new ZoomComboBoxItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_ZoomWholePage), PrintingSystemCommand.ZoomToWholePage),
									new ZoomComboBoxItem(PreviewLocalizer.GetString(PreviewStringId.MenuItem_ZoomTwoPages), PrintingSystemCommand.ZoomToTwoPages)
								};
		}
		internal void Init(PrintControl printControl) {
			Init();
			this.printControl = printControl;
			this.EditValue = GetEditValue();
		}
 		internal void OnZoomChanged(object sender, EventArgs e) {
			SetBaseEditValue(GetEditValue());
		}
		protected override void ExecZoomCommandCore(float zoomFactor) {
			printControl.ExecCommand(PrintingSystemCommand.Zoom, new object[] { zoomFactor });
		}
		protected override bool CanExecZoomCommand() {
			return printControl != null;
		}
		protected override void ApplyNewEditValue() {
			if(EditValue is ISupportPrintingSystemCommand) {
				PrintingSystemCommand command =((ISupportPrintingSystemCommand)EditValue).Command;
				if(command == ((ISupportPrintingSystemCommand)this).Command)
					ExecZoomCommand(EditValue);
				else
					printControl.ExecCommand( ((ISupportPrintingSystemCommand)EditValue).Command );
			}
		}
	}
	public class RuntimeZoomBarEditItem : ZoomBarEditItem, ISupportParametrizedPrintingSystemCommand {
		#region ISupportParametrizedPrintingSystemCommand Members
		public object CommandParameter {
			get { return CurrentZoom; }
		}
		#endregion
	}
	[
	ToolboxItem(false),
	DesignTimeVisible(false),
	]
	public class PrintPreviewRepositoryItemComboBox : RepositoryItemComboBox {
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public override ComboBoxItemCollection Items { get { return base.Items; }
		}
	}
}
