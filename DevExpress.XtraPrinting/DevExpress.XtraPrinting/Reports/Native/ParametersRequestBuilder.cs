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
using System.Text;
using DevExpress.XtraEditors;
using System.Drawing.Design;
using DevExpress.XtraPrinting.Native.Lines;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Parameters;
using DevExpress.LookAndFeel;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraBars.Docking;
using System.Drawing;
using DevExpress.XtraPrinting.InternalAccess;
using DevExpress.XtraReports.Design;
using DevExpress.Skins;
using System.ComponentModel;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting;
using DevExpress.Services.Internal;
using DevExpress.XtraReports.Parameters.Native;
using System.Linq;
using DevExpress.Data.Browsing;
using DevExpress.XtraPrinting.Reports.Native;
using System.Collections;
namespace DevExpress.XtraReports.Native {
	public class ParametersRequestBuilder {
		#region static
		protected static string ParametersRequestCaption {
			get { return PreviewLocalizer.GetString(PreviewStringId.ParametersRequest_Caption); }
		}
		#endregion
		IReport report;
		IList<ParameterInfo> parametersInfo;
		protected IReport Report {
			get { return report; }
		}
		public IList<ParameterInfo> ParametersInfo {
			get { return parametersInfo; }
		}
		public ParametersRequestBuilder(IReport report, IList<ParameterInfo> parametersInfo) {
			this.report = report;
			this.parametersInfo = parametersInfo;
		}
	}
	public class ParametersFormBuilder : ParametersRequestBuilder {
		public ParametersFormBuilder(IReport report, IList<ParameterInfo> parametersInfo)
			: base(report, parametersInfo) {
		}
		public DialogResult ShowDialog(UserLookAndFeel lookAndFeel) {
			using(XtraForm form = new XtraForm()) {
				form.SuspendLayout();
				if(lookAndFeel != null)
					form.LookAndFeel.ParentLookAndFeel = lookAndFeel;
				form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
				form.TopLevel = true;
				form.ControlBox = false;
				form.ShowInTaskbar = false;
				form.StartPosition = FormStartPosition.CenterScreen;
				form.Text = ParametersRequestCaption;
				ParametersControl parametersControl = new ParametersControl(true);
				parametersControl.Update(Report, null, ParametersInfo);
				form.ClientSize = parametersControl.BestSize;
				form.Controls.Add(parametersControl);
				form.ResumeLayout(false);
				return form.ShowDialog();
			}
		}
	}
	public class ParametersPanelBuilder : ParametersRequestBuilder {
		PrintControl printControl;
		DockPanel dockPanel;
		DockPanel DockPanel {
			get {
				if(dockPanel == null)
					dockPanel = printControl.GetDockPanel(PrintingSystemCommand.Parameters);
				return dockPanel; 
			}
		}
		public ParametersPanelBuilder(IReport report, IList<ParameterInfo> parametersInfo)
			: base(report, parametersInfo) {
		}
		public void BuildParametersPanel(PrintControl printControl, bool showParametersPanel) {
			this.printControl = printControl;
			if(ParametersInfo.Count > 0)
				InitPanel(showParametersPanel);
			else {
				DockPanel.ControlContainer.Controls.Clear();
				SetPanelVisibility(false);
				this.printControl.HasParameters = false;
			}
		}
		void InitPanel(bool showParametersPanel) {
			DockPanel.Text = ParametersRequestCaption;
			ParametersControl parametersControl;
			if(DockPanel.ControlContainer.Controls.Count == 0) {
				parametersControl = new PanelParametersControl();
				DockPanel.ControlContainer.Controls.Add(parametersControl);
				parametersControl.LookAndFeel.ParentLookAndFeel = ((ISupportLookAndFeel)printControl).LookAndFeel;
			} else {
				parametersControl = DockPanel.ControlContainer.Controls[0] as ParametersControl;
				System.Diagnostics.Debug.Assert(parametersControl != null);
			}
			((PanelParametersControl)parametersControl).Initialize(Report, printControl, ParametersInfo);
			printControl.HasParameters = true;
			SetPanelVisibility(showParametersPanel);
		}
		void SetPanelVisibility(bool value) {
			printControl.ExecCommand(DevExpress.XtraPrinting.PrintingSystemCommand.Parameters, new object[] { value });
		}
	}
	public class PanelParametersControl : ParametersControl {
		IReport saveReport;
		PrintControl savedPrintControl;
		IList<ParameterInfo> saveParametersInfo;
		bool visibleChanged;
		public PanelParametersControl()
			: base(false) {
			submitButton.DialogResult = DialogResult.None;
		}
		protected override void OnVisibleChanged(EventArgs e) {
			visibleChanged = true;
			if(saveReport != null && !saveReport.IsDisposed && saveReport != Report) {
				Update(saveReport, savedPrintControl, saveParametersInfo);
				saveReport = null;
			}
			base.OnVisibleChanged(e);
		}
		public void Initialize(IReport report, PrintControl printControl, IList<ParameterInfo> parametersInfo) {
			if(visibleChanged) {
				Update(report, printControl, parametersInfo);
			} else {
				saveReport = report;
				savedPrintControl = printControl;
				saveParametersInfo = parametersInfo;
			}
		}
		protected override void OnSubmit() {
			if(Report == null) return;
			Report.RaiseParametersRequestSubmit(parametersInfo, true);
			ParametersRequestService.OnSubmit(parametersInfo, Report);
		}
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class ParametersControl : XtraUserControl, DevExpress.XtraPrinting.Control.Native.IContentBestSize {
		#region static
		static ParameterLineController[] GetLineControllers(IList<ParameterInfo> parametersInfo, UserLookAndFeel lookAndFeel) {
			List<ParameterLineController> controllers = new List<ParameterLineController>();
			foreach(ParameterInfo parameterInfo in parametersInfo) {
				ParameterLineController controller = new ParameterLineController(parameterInfo, parametersInfo);
				controllers.Add(controller);
			}
			return controllers.ToArray();
		}
		#endregion
		const int buttonTextPadding = 9;
		const int innerPadding = 4;
		const int padding = 11;
		const int buttonsHeight = 23;
		LinesContainer linesContainer;
		PanelControl buttonsPanel;
		PanelControl parametersPanel;
		protected SimpleButton submitButton;
		SimpleButton resetButton;
		SimpleButton cancelButton;
		LabelControl lineLabel;
		bool showCancelButton;
		protected IList<ParameterInfo> parametersInfo;
		IReport report;
		ParameterLineController[] lineControllers;
		int minParametersPanelHeight, minButtonsPanelHeight;
		ParameterEditorValueProvider valuesProvider;
		PrintControl printControl;
		protected IReport Report { get { return report != null && report.IsDisposed ? null : report; } }
		public Size BestSize {
			get {
				int bestWidth = linesContainer.Width + parametersPanel.Margin.Left + parametersPanel.Margin.Right;
				int bestHeight = linesContainer.Height + parametersPanel.Margin.Top + parametersPanel.Margin.Bottom + minButtonsPanelHeight;
				return new Size(bestWidth, bestHeight);
			}
		}
		public IButtonControl DefaultButton {
			get { return submitButton; }
		}
		public ParametersControl(bool showCancelButton) {
			linesContainer = new LinesContainer();
			buttonsPanel = new PanelControl();
			buttonsPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			buttonsPanel.CausesValidation = false;
			parametersPanel = new PanelControl();
			parametersPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			cancelButton = new SimpleButton();
			submitButton = new SimpleButton();
			resetButton = new SimpleButton();
			lineLabel = new LabelControl();
			linesContainer.LinesRefreshed += new EventHandler<RefreshLinesEventArgs>(linesContainer_LinesRefreshed);
			linesContainer.Location = new Point(0, 0);
			parametersPanel.AutoScroll = true;
			parametersPanel.Controls.Add(linesContainer);
			parametersPanel.Location = new Point(0, 0);
			parametersPanel.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			cancelButton.Text = PreviewLocalizer.GetString(PreviewStringId.Button_Cancel);
			cancelButton.TabIndex = 102;
			cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			cancelButton.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			this.showCancelButton = cancelButton.Visible = showCancelButton;
			submitButton.Text = PreviewLocalizer.GetString(PreviewStringId.ParametersRequest_Submit);
			submitButton.TabIndex = 100;
			submitButton.Click += new EventHandler(submitButton_Click);
			submitButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			submitButton.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			resetButton.CausesValidation = false;
			resetButton.TabIndex = 101;
			resetButton.Text = PreviewLocalizer.GetString(PreviewStringId.ParametersRequest_Reset);
			resetButton.Click += new EventHandler(resetButton_Click);
			resetButton.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			lineLabel.AutoSizeMode = LabelAutoSizeMode.None;
			lineLabel.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			lineLabel.LineVisible = true;
			lineLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			buttonsPanel.Controls.Add(lineLabel);
			buttonsPanel.Controls.Add(cancelButton);
			buttonsPanel.Controls.Add(submitButton);
			buttonsPanel.Controls.Add(resetButton);
			buttonsPanel.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			this.Controls.Add(parametersPanel);
			this.Controls.Add(buttonsPanel);
			this.Dock = DockStyle.Fill;
			minButtonsPanelHeight = buttonsHeight + 2 * padding;
		}
		void SubscribeEvents(PrintControl printControl) {
			if(printControl == null)
				return;
			printControl.CommandChanged += new EventHandler(PrintControl_CommandChanged);
		}
		void UnsubscribeEvents(PrintControl printControl) {
			if(printControl == null)
				return;
			printControl.CommandChanged -= new EventHandler(PrintControl_CommandChanged);
		}
		void PrintControl_CommandChanged(object sender, EventArgs e) {
			UpdateSubmitButtonEnable();
		}
		void UpdateSubmitButtonEnable() {
			bool enable = true;
			if(printControl != null) {
				CommandSetItem item = printControl.CommandSet[DevExpress.XtraPrinting.PrintingSystemCommand.SubmitParameters];
				enable = item.Enabled;
			}
			submitButton.Enabled = enable;
		}
		public void Update(IReport report, PrintControl printControl, IList<ParameterInfo> parametersInfo) {
			this.report = report;
			UnsubscribeEvents(this.printControl);
			this.printControl = printControl;
			SubscribeEvents(this.printControl);
			UpdateSubmitButtonEnable();
			this.parametersInfo = parametersInfo;
			valuesProvider = new ParameterEditorValueProvider(this.parametersInfo);
			FillWithLines(parametersInfo);
			minParametersPanelHeight = linesContainer.Bottom;
			UpadateInnerControlsSizes();
			UpdateButtonsLayout();
			resetButton.Enabled = false;
		}
		void FillWithLines(IList<ParameterInfo> parametersInfo) {
			UnsubscribeEditValueChanged();
			lineControllers = GetLineControllers(parametersInfo, this.LookAndFeel);
			this.SuspendLayout();
			parametersPanel.SuspendLayout();
			linesContainer.Controls.Clear();
			BaseLine[] lines = Array.ConvertAll<ILine, BaseLine>(BaseLineController.GetLines(lineControllers, null), delegate(ILine line) { return (BaseLine)line; });
			linesContainer.FillWithLines(lines, this.LookAndFeel, 100, innerPadding, padding);
			int maxWith = 0;
			foreach(BaseEditPropertyLine control in linesContainer.Controls) {
				control.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
				maxWith = Math.Max(control.LabelInitialWidth, maxWith);
			}
			foreach(BaseEditPropertyLine control in linesContainer.Controls)
				control.LabelMaxWidth = maxWith;
			this.Width = Math.Max(GetMinButtonsPanelWidth(), linesContainer.Width);
			SubscribeEditValueChanged();
			this.ResumeLayout(false);
			parametersPanel.ResumeLayout(false);
		}
		void linesContainer_LinesRefreshed(object sender, RefreshLinesEventArgs e) {
			if(Report == null) return;
			ParameterInfo changedParameterInfo = FindParameterInfoByLine(e.InitiatingLine);
			Report.RaiseParametersRequestValueChanged(parametersInfo, changedParameterInfo);
			ParametersRequestService.OnValueChanged(parametersInfo, changedParameterInfo, Report);
		}
		ParameterInfo FindParameterInfoByLine(BaseLine line) {
			BaseEditPropertyLine initiatingLine = line as BaseEditPropertyLine;
			if(initiatingLine != null) {
				Parameter parameter = (Parameter)((IContainerComponent)initiatingLine.Property).Component;
				foreach(ParameterInfo info in parametersInfo)
					if(info.Parameter == parameter) {
						return info;
					}
			}
			return null;
		}
		BaseEditPropertyLine FindPropertyLineByEditor(BaseEdit editor) {
			return linesContainer.Controls.OfType<BaseEditPropertyLine>().SingleOrDefault(x => x.Editor == editor);
		}
		ParameterInfo FindParameterInfoByEditor(BaseEdit editor) {
			foreach(ParameterInfo info in parametersInfo)
				if(info.Editor == editor) {
					return info;
				}
			return null;
		}
		void submitButton_Click(object sender, EventArgs e) {
			Submit();
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Enter) {
				this.DefaultButton.PerformClick();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		protected override void OnEnter(EventArgs e) {
			base.OnEnter(e);
			this.DefaultButton.NotifyDefault(true);
		}
		protected override void OnLeave(EventArgs e) {
			base.OnLeave(e);
			this.DefaultButton.NotifyDefault(false);
		}
#if DEBUGTEST
		public
#endif
		void Submit() {
			try {
				ParameterLineController.Commit(lineControllers);
				OnSubmit();
				resetButton.Enabled = false;
			} catch(Exception e) {
				if(Report != null)
					Report.PrintingSystemBase.OnCreateDocumentException(new ExceptionEventArgs(e));
			}
		}
		protected virtual void OnSubmit() {
			if(Report == null) return;
			Report.RaiseParametersRequestSubmit(parametersInfo, false);
			ParametersRequestService.OnSubmit(parametersInfo, Report);
		}
		void resetButton_Click(object sender, EventArgs e) {
			UnsubscribeEditValueChanged();
			SetReadonly(true);
			ParameterLineController.Reset(lineControllers);
			var enumerator = linesContainer.Lines.GetEnumerator();
			if (parametersInfo.Select(x=>x.Parameter).Any(x=> x.LookUpSettings!=null) && enumerator.MoveNext() )
				ResetCore(enumerator);
			else {
				linesContainer.RefreshLines(null);
				SetReadonly(false);
				SubscribeEditValueChanged();
				resetButton.Enabled = false;
			}
		}
		void ResetCore(IEnumerator enumerator) {
			var line = enumerator.Current as BaseLine;
			line.RefreshProperty();
			var changedParameterInfo = FindParameterInfoByLine(line);
			var lookUpValuesProvider = GetLookUpValuesProvider();
			var task = lookUpValuesProvider.GetLookUpValues(changedParameterInfo.Parameter, valuesProvider);
			task.ContinueWith(t => {
				if (t.Exception != null) {
					Invoke((Action)(() => {
						NotificationService.ShowException<PrintingSystemBase>(LookAndFeel, this.FindForm(), t.Exception);
						SubscribeEditValueChanged();
						SetReadonly(false);
					}));
					return;
				} else
					Invoke((Action)(() => UpdateLookUpEditors(t.Result)));
				if (enumerator.MoveNext())
					ResetCore(enumerator);
				else {
					BeginInvoke((Action)(() => {
						linesContainer.RefreshLines(null);
						SubscribeEditValueChanged();
						SetReadonly(false);
					}));
				}
			});
		}
		void Editor_EditValueChanged(object sender, EventArgs e) {
			if(Report == null) return;
			resetButton.Enabled = true;
			var editor = sender as BaseEdit;
			var changedInfo = FindParameterInfoByEditor(editor);
			ParametersRequestService.OnEditorValueChanged(parametersInfo, changedInfo, Report);
			var line = FindPropertyLineByEditor(editor);
			line.Value = editor.EditValue;
			if(changedInfo != null && parametersInfo.Any(x => x.Parameter.LookUpSettings != null)) {
				UpdateLookUps(changedInfo);
			}
		}
		void UpdateLookUps(ParameterInfo changedParameterInfo) {
			UnsubscribeEditValueChanged();
			SetReadonly(true);
			var lookUpValuesProvider = GetLookUpValuesProvider();
			var task = lookUpValuesProvider.GetLookUpValues(changedParameterInfo.Parameter, valuesProvider);
			task.ContinueWith(t => {
				if (t.Exception != null) {
					Invoke((Action)(() => {
						NotificationService.ShowException<PrintingSystemBase>(LookAndFeel, this.FindForm(), t.Exception);
					}));					
				} else 
				Invoke((Action)(() => {
					UpdateLookUpEditors(t.Result);
				}));
				BeginInvoke((Action)(() => {
					SubscribeEditValueChanged();
					SetReadonly(false);
				}));
			});
		}
		void UpdateLookUpEditors(IEnumerable<ParameterLookUpValuesContainer> lookUpValues) {
			foreach(var item in lookUpValues) {
				var parameterInfo = parametersInfo.FirstOrDefault(x => x.Parameter == item.Parameter);
				LookUpEditUpdaterBase updater = LookUpEditUpdater.CreateInstance(parameterInfo.Editor as BaseEdit);
				if(updater != null && updater.TryUpdateEditor(item.LookUpValues)) {
					var line = FindPropertyLineByEditor((BaseEdit)parameterInfo.Editor);
					line.Value = ((BaseEdit)parameterInfo.Editor).EditValue;
				}
			}
		}
		ILookUpValuesProvider GetLookUpValuesProvider() {
			var provider = report.PrintingSystemBase.GetService<ILookUpValuesProvider>();
			return provider ?? new LookUpValuesProvider(parametersInfo.Select(x => x.Parameter), Report.GetService<DevExpress.Data.Browsing.DataContext>());
		}
		void SetReadonly(bool isReadonly) {
			foreach(ParameterLineController controller in lineControllers)
				((BaseEdit)controller.ParameterInfo.Editor).Properties.ReadOnly = isReadonly;
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			UpadateInnerControlsSizes();
			UpdateButtonsLayout();
		}
		void UpadateInnerControlsSizes() {
			if(this.Height < minParametersPanelHeight + minButtonsPanelHeight) {
				buttonsPanel.Height = minButtonsPanelHeight;
				parametersPanel.Height = this.Height - minButtonsPanelHeight;
			} else {
				parametersPanel.Height = minParametersPanelHeight;
				buttonsPanel.Height = this.Height - minParametersPanelHeight;
			}
			buttonsPanel.Location = new System.Drawing.Point(0, parametersPanel.Bottom);
			parametersPanel.SuspendLayout();
			buttonsPanel.Width = parametersPanel.Width = this.Width;
			linesContainer.Width = parametersPanel.ClientSize.Width;
			parametersPanel.ResumeLayout(true);
		}
		void UpdateButtonsLayout() {
			lineLabel.Location = new Point(0, -1);
			lineLabel.Size = new Size(buttonsPanel.Width, 3);
			if(showCancelButton) {
				resetButton.Size = submitButton.Size = cancelButton.Size = CalcBestSize(resetButton, submitButton, cancelButton);
				SetLayout(submitButton, resetButton, cancelButton);
			} else {
				resetButton.Size = submitButton.Size = cancelButton.Size = CalcBestSize(resetButton, submitButton);
				SetLayout(resetButton, submitButton);
			}
		}
		int GetMinButtonsPanelWidth() {
			return CalcBestSize(resetButton, submitButton, cancelButton).Width * 3 + padding * 4 - parametersPanel.Margin.Left - parametersPanel.Margin.Right;
		}
		static Size CalcBestSize(params BaseControl[] controls) {
			int width = 0;
			foreach(var item in controls)
				width = Math.Max(width, item.CalcBestSize().Width);
			return new Size(width + 2 * buttonTextPadding, buttonsHeight);
		}
		void SetLayout(params BaseControl[] controls) {
			int x = buttonsPanel.Width - padding;
			for(int i = controls.Length - 1; i >= 0; i--) {
				BaseControl item = controls[i];
				item.Location = new Point(x - item.Width, padding + lineLabel.Height);
				x = item.Bounds.X - padding;
			}
		}
		void SubscribeEditValueChanged() {
			if(lineControllers == null)
				return;
			foreach(ParameterLineController controller in lineControllers) {
				UnsubscribeEditValueChanged(controller.ParameterInfo);
				((BaseEdit)controller.ParameterInfo.Editor).EditValueChanged += new EventHandler(Editor_EditValueChanged);
			}
		}
		void UnsubscribeEditValueChanged() {
			if(lineControllers == null)
				return;
			foreach(ParameterLineController controller in lineControllers)
				UnsubscribeEditValueChanged(controller.ParameterInfo);
		}
		void UnsubscribeEditValueChanged(ParameterInfo parameterInfo) {
			BaseEdit editor = parameterInfo.GetEditor(false) as BaseEdit;
			if(editor != null)
				editor.EditValueChanged -= new EventHandler(Editor_EditValueChanged);
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					linesContainer.LinesRefreshed -= new EventHandler<RefreshLinesEventArgs>(linesContainer_LinesRefreshed);
					submitButton.Click -= new EventHandler(submitButton_Click);
					resetButton.Click -= new EventHandler(resetButton_Click);
					UnsubscribeEditValueChanged();
					UnsubscribeEvents(printControl);
				}
			} finally {
				base.Dispose(disposing);
			}
		}
	}
	public class ParameterLineController : ParameterLineControllerBase {
		ParameterInfo parameterInfo;
		public ParameterInfo ParameterInfo {
			get { return parameterInfo; }
		}
		public ParameterLineController(ParameterInfo parameterInfo, object obj) 
			: base(parameterInfo.Parameter, obj) {
				this.parameterInfo = parameterInfo;
		}
		protected override ILine CreateLine(LineFactoryBase lineFactory) {
			return new BaseEditPropertyLine(CreateStringConverter(), (BaseEdit)parameterInfo.Editor, PropertyDescriptor, Obj);
		}		
	}
	public class BaseEditPropertyLine : EditorPropertyLineBase {
		BaseEdit editor;
		int labelInitialWidth;
		int labelMaxWidth = -1;
		public int LabelMaxWidth {
			set { labelMaxWidth = value; }
		}
		protected override bool ShouldUpdateValue {
			get { return base.ShouldUpdateValue || IsValidationDisabled(); }
		}
		public int LabelInitialWidth { 
			get { return labelInitialWidth; }
		}
		internal BaseEdit Editor { get { return editor; } }
		public BaseEditPropertyLine(IStringConverter converter, BaseEdit editor, PropertyDescriptor property, object obj)
			: base(converter, property, obj) {
			this.editor = editor;
		}
		protected override BaseEdit CreateEditor() {
			return editor;
		}
		protected override void Initialize() {
			base.Initialize();
			labelInitialWidth = label.Width;
			label.AutoSizeMode = LabelAutoSizeMode.None;
			label.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
			label.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
			baseEdit.Dock = DockStyle.None;
			this.SizeChanged += new EventHandler(BaseEditPropertyLine_SizeChanged);
		}
		protected override void IntializeEditor() {
			base.IntializeEditor();
			baseEdit.Validating += new CancelEventHandler(OnBaseEditValidating);
		}
		protected override void SetEditText(object val) {
			baseEdit.EditValue = val;
		}
		protected override void UpdateValueCore(BaseEdit edit) {
			String text = edit.EditValue as String;
			if(text != null && Property.PropertyType != typeof(string)) {
				object value = converter.ConvertFromString(text);
				if(!Object.Equals(Value, value))
					Value = value;
			}
			Value = edit.EditValue;
		}
		protected override void OnBaseEditValidating(object sender, CancelEventArgs e) {
			if(Parent == null) return;
			if(baseEdit.EditValue != null && Property.PropertyType.IsAssignableFrom(baseEdit.EditValue.GetType()))
				UpdateValueCore(baseEdit);
			else
				base.OnBaseEditValidating(sender, e);
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				this.SizeChanged -= new EventHandler(BaseEditPropertyLine_SizeChanged);
			base.Dispose(disposing);
	}
		void BaseEditPropertyLine_SizeChanged(object sender, EventArgs e) {
			if(string.IsNullOrEmpty(label.Text) || labelMaxWidth < 0)
				return;
			const int minEditWidth = 50;
			label.Width = Math.Min(labelMaxWidth, Math.Max(0, this.Width - minEditWidth));
			baseEdit.Left = label.Right;
			baseEdit.Width = this.Width - baseEdit.Left;
		}
		bool IsValidationDisabled() { 
			if(baseEdit == null) return false;
			ContainerControl containerControl = baseEdit.GetContainerControl() as ContainerControl;
			return containerControl != null && containerControl.AutoValidate == AutoValidate.Disable;
		}
	}
}
