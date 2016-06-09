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
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.Localization;
using System.Windows.Forms.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using System.Collections.Generic;
using DevExpress.Data.Browsing;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraPrinting.Native.RichText;
using DevExpress.XtraReports.Design.Behaviours;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office.Internal;
using DevExpress.Utils.Internal;
namespace DevExpress.XtraReports.Design 
{
	public class XRLabelDesigner : XRTextControlDesigner
	{
		protected XRLabel XRLabel { get { return Component as XRLabel; } }
		public XRLabelDesigner() : base() {
		}
		protected override InplaceTextEditorBase GetInplaceEditor(string text, bool selectAll) {
			return new InplaceTextEditor(fDesignerHost, XRLabel, text, selectAll);
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			if (String.IsNullOrEmpty(XRLabel.Text))
				XRLabel.Text = Component.Site.Name;
			XRLabel.Padding = XRLabel.DefaultPadding;
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new XRLabelDesignerActionList1(this));
			list.Add(new XRLabelDesignerActionList2(this));
			list.Add(new XRControlBookmarkDesignerActionList(this));
			list.Add(new XRFormattingControlDesignerActionList(this));
			list.Add(new XRLabelDesignerActionList3(this));
		}
		protected override bool SetFormatStringCore(string propName, string formatString) {
			if(XRLabel.HasSummary) {
				XRLabel.Summary.FormatString = formatString;
				return true;
			}
			return base.SetFormatStringCore(propName, formatString);
		}
	}
	public abstract class XRRichTextBaseDesigner : XRTextControlDesigner {
		#region static
		static string SafelyGetRtfFromFile(string fileName) {
			DocumentModelHelper helper = new DocumentModelHelper();
			helper.Load(fileName);
			return helper.GetRtf();
		}
		#endregion
		public new InplaceRichTextEditorBase Editor { get { return (InplaceRichTextEditorBase)base.Editor; } }
		protected XRRichTextBaseDesigner()
			: base() {
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			Verbs.AddRange(new DesignerVerb[] {
												CreateXRDesignerVerb(DesignSR.Verb_RTFClear, ReportCommand.VerbRtfClear),
												CreateXRDesignerVerb(DesignSR.Verb_RTFLoad, ReportCommand.VerbRtfLoadFile)});
		}
		public void OnLoadFile(object sender, EventArgs e) {
			OpenFileDialog openFileDialog = new OpenFileDialog();
			XtraRichEditStringId[] filterIds = new XtraRichEditStringId[] { 
				XtraRichEditStringId.FileFilterDescription_RtfFiles, 
				XtraRichEditStringId.FileFilterDescription_OpenXmlFiles,
				XtraRichEditStringId.FileFilterDescription_TextFiles,
				XtraRichEditStringId.FileFilterDescription_HtmlFiles,
			};
			string[][] extentions = new string[][] { 
				new string[] { "rtf" }, 
				new string[] { "docx" },
				new string[] { "txt" },
				new string[] { "html", "htm" },
			};
			FileDialogFilterCollection filters = new FileDialogFilterCollection();
			for(int i = 0; i < filterIds.Length; i++) {
				FileDialogFilter filter = new FileDialogFilter(XtraRichEditLocalizer.GetString(filterIds[i]), extentions[i]);
				filters.Add(filter);
			}
			openFileDialog.Filter = filters.CreateFilterString();
			if(DialogRunner.ShowDialog(openFileDialog) == DialogResult.OK) {
				if(Editor != null)
					LoadFileInInpaceEditor(openFileDialog.FileName);
				else
					LoadFileInRichTextBox(openFileDialog.FileName);
			}
		}
		void LoadFileInInpaceEditor(string fileName) {
			Editor.Rtf = SafelyGetRtfFromFile(fileName);
		}
		void RaiseRtfContentChanged(string rtfText) {
			XRControlDesignerBase.RaiseComponentChanging(changeService, XRRichTextBase, "RtfText");
			XRRichTextBase.Rtf = rtfText;
			XRControlDesignerBase.RaiseComponentChanged(changeService, XRRichTextBase);
		}
		void LoadFileInRichTextBox(string fileName) {
			DesignerTransaction trans = DesignerHost.CreateTransaction(String.Format("Load file in {0}", XRRichTextBase.Name));
			CursorStorage.SetCursor(Cursors.WaitCursor);
			try {
				RaiseRtfContentChanged(SafelyGetRtfFromFile(fileName));
				trans.Commit();
			} catch {
				trans.Cancel();
			}
			finally {
				CursorStorage.RestoreCursor();
			}
		}
		public void OnClear(object sender, EventArgs e) {
			if(Editor != null)
				ClearInplaceEditor();
			else
				ClearRichTextBox();
		}
		private void ClearRichTextBox() {
			DesignerTransaction trans = DesignerHost.CreateTransaction(String.Format("Clear {0}", XRRichTextBase.Name));
			RichTextBox richTextBox = new RichTextBox();
			IntPtr ignore = richTextBox.Handle;
			try {
				RaiseRtfContentChanged(richTextBox.Rtf);
				trans.Commit();
			}
			catch {
				trans.Cancel();
			}
			finally {
				richTextBox.Dispose();
			}
		}
		private void ClearInplaceEditor() {
			Editor.Clear();
		}
		protected XRRichTextBase XRRichTextBase {
			get { return Component as XRRichTextBase; }
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			if (String.IsNullOrEmpty(XRRichTextBase.Text))
				XRRichTextBase.Text = Component.Site.Name;
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new XRRichTextBoxBaseDesignerActionList(this));
		}
	}
	public class XRRichTextBoxDesigner : XRRichTextBaseDesigner {
		protected override InplaceTextEditorBase GetInplaceEditor(string text, bool selectAll) {
			return new InplaceRichTextEditor(fDesignerHost, XRRichTextBase, text, selectAll);
		}
		protected override bool ShouldDrawReportExplorerImage {
			get { return true; }
		}
	}
	public class XRCheckBoxDesigner : XRTextControlDesigner
	{
		protected XRCheckBox XRCheckBox { get { return Component as XRCheckBox; } }
		public XRCheckBoxDesigner() : base() {
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			if (String.IsNullOrEmpty(XRCheckBox.Text))
				XRCheckBox.Text = Component.Site.Name;
		}
		public override void OnComponentChanged(ComponentChangedEventArgs e) {
			base.OnComponentChanged(e);
			try {
				if(e.Member != null && e.Member.Name == "Name" && e.OldValue.Equals(XRCheckBox.Text)) {
					XRCheckBox.Text = (string)e.NewValue;
				}
			} catch {}
		}
		protected override InplaceTextEditorBase GetInplaceEditor(string text, bool selectAll) {
			return new InplaceSingleTextEditor(fDesignerHost, XRCheckBox, text, selectAll);
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new XRCheckBoxDesignerActionList(this));
			list.Add(new XRControlTextDesignerActionList(this));
			list.Add(new XRControlBookmarkDesignerActionList(this));
			list.Add(new XRFormattingControlDesignerActionList(this));
		}
	}
	public class XRPageBreakDesigner : XRControlDesigner 
	{
		public XRPageBreakDesigner() : base () {
		}
		public override void SetBounds(RectangleF rect) {
		}
		public override void SetSize(SizeF value, bool raiseChanged) {
		}
		protected override SelectionRules GetSelectionRulesCore() {
			return SelectionRules.Moveable;
		}
		protected override string[] GetFilteredProperties() {
			List<string> names = new List<string>(XRComponentDesigner.stylePropertyNames);
			names.AddRange(XRComponentDesigner.filterPropertyNames);
			names.AddRange(new string[] {
									XRComponentPropertyNames.Size, XRComponentPropertyNames.Styles});
			return names.ToArray();
		}
	}
	public class WinControlContainerDesigner : XRControlDesigner 
	{
		bool winControlRemoved;
		protected WinControlContainer WinControlContainer { 
			get { return Component as WinControlContainer; }
		}
		protected Control WinControl { 
			get { return WinControlContainer.WinControl; }
		}
		public override ICollection AssociatedComponents {
			get {
				ArrayList comps = new ArrayList(base.AssociatedComponents);
				if(WinControl != null)
					comps.Add(WinControl);
				return comps;
			}
		}
		public override DesignerVerbCollection Verbs {
			get {
				ControlDesigner controlDesigner = GetWinControlDesigner();
				if(controlDesigner != null) {
					return controlDesigner.Verbs;
				}
				return base.Verbs;
			}
		}
		public override bool CanCutControl {
			get { return false; }
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				IIdleService serv = GetService(typeof(IIdleService)) as IIdleService;
				if(serv != null)
					serv.Idle -= new EventHandler(HandleGrabTick);
				changeService.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
				if(!winControlRemoved && WinControl != null && WinControl.Site != null) {
					winControlRemoved = true;
					if(!this.DesignerHost.Loading)
						this.DesignerHost.DestroyComponent(WinControl);
				}
			}
			base.Dispose(disposing);
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			changeService.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
			IIdleService serv = GetService(typeof(IIdleService)) as IIdleService;
			if(serv != null)
				serv.Idle += new EventHandler(HandleGrabTick);
		}
		private void OnComponentRemoved(object source, ComponentEventArgs e) {
			if(Comparer.Equals(e.Component, WinControl)) {
				winControlRemoved = true;
				WinControlContainer.Dispose();
			}
		}
		public override void SelectComponent() {
			object selObject = (WinControl == null || WinControl.Site == null) ? (object)XRControl : (object)WinControl;
			SelectComponents(new object[] { selObject }, GetSelectionTypes());
		}
		protected override string[] GetFilteredProperties() {
			return new string[] {
									XRComponentPropertyNames.ParentStyleUsing, 
									XRComponentPropertyNames.TextAlign,
									XRComponentPropertyNames.TextAlignment,
									XRComponentPropertyNames.Text,
									XRComponentPropertyNames.WordWrap };
		}
		protected override SelectionRules GetSelectionRulesCore() {
			if(WinControl != null) {
				ControlDesigner controlDesigner = GetWinControlDesigner();
				if(controlDesigner != null)
					return controlDesigner.SelectionRules;
			}
			return base.GetSelectionRulesCore();
		}
		ControlDesigner GetWinControlDesigner() {
			return WinControl != null ? fDesignerHost.GetDesigner(WinControl) as ControlDesigner : null;
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			ControlDesigner controlDesigner = GetWinControlDesigner();
			if(controlDesigner == null)
				return;
			try {
				DesignerActionListCollection winControlList = XRAccessor.GetProperty(controlDesigner, "ActionLists") as DesignerActionListCollection;
				if(winControlList == null)
					return;
				foreach(DesignerActionList item in winControlList)
					list.Add(item);
			} catch { }
		}
	}
	[DesignerBehaviour(typeof(PrintableComponentContainerBehaviour))]
	public class PrintableComponentContainerDesigner : WinControlContainerDesigner {
		public PrintableComponentContainerDesigner()
			: base() {
		}
		public override bool CanCutControl {
			get { return true; }
		}
	}
	public class XRLineDesigner : XRControlDesigner
	{
		public XRLineDesigner() : base() {
		}
		protected override string[] GetFilteredProperties() {
			return new string[] {
									XRComponentPropertyNames.Font, 
									XRComponentPropertyNames.TextAlign,
									XRComponentPropertyNames.TextAlignment,
									XRComponentPropertyNames.Text,
									XRComponentPropertyNames.WordWrap, 
									XRComponentPropertyNames.NavigateUrl, 
									XRComponentPropertyNames.Target };
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new XRLineDesignerActionList(this));
			list.Add(new XRControlAnchorVerticalDesignerActionList(this));
			list.Add(new XRFormattingControlDesignerActionList(this));
		}
	}
	public class XRPageInfoDesigner : XRControlDesigner {
		public XRPageInfoDesigner() : base() {
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			XRControl.Padding = XRPageInfo.DefaultPadding;
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new XRPageInfoDesignerActionList(this));
			list.Add(new XRControlAnchorVerticalDesignerActionList(this));
			list.Add(new XRFormattingControlDesignerActionList(this));
		}
	}
	[DesignerBehaviour(typeof(PanelDesignerBehaviour))]
	public class XRPanelDesigner : XRControlDesigner 
	{
		public XRPanelDesigner() : base() {
		}
		protected override string[] GetFilteredProperties() {
			return new string[] { XRComponentPropertyNames.Text,
									XRComponentPropertyNames.Font, 
									XRComponentPropertyNames.ForeColor, 
									XRComponentPropertyNames.TextAlign,
									XRComponentPropertyNames.TextAlignment,
									XRComponentPropertyNames.WordWrap }; 
		}
	}
	public class XRBarCodeDesigner : XRTextControlDesigner {
		#region innerClasses
		class PaddingPropertyDescriptor : WrappedPropertyDescriptor {
			public PaddingPropertyDescriptor(PropertyDescriptor parentPropertyDescriptor)
				: base(parentPropertyDescriptor) {
			}
			public override void SetValue(object component, object Value) {
				if(Value is int) {
					PaddingInfo paddingInfo = (int)Value;
					Value = paddingInfo;
				}
				oldPropertyDescriptor.SetValue(component, Value);
			}
			public override string Name { get { return oldPropertyDescriptor.Name; } }
			public override string DisplayName { get { return oldPropertyDescriptor.DisplayName; } }
			public override object GetValue(object component) { return oldPropertyDescriptor.GetValue(component); }
		}
		#endregion
		protected XRBarCode XRBarCode { get { return Component as XRBarCode; } }
		public XRBarCodeDesigner()
			: base() {
		}
		protected override InplaceTextEditorBase GetInplaceEditor(string text, bool selectAll) {
			return new InplaceSingleTextEditor(fDesignerHost, XRBarCode, text, selectAll);
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			string initialBarCodeText = XRBarCode.Text;
			base.InitializeNewComponent(defaultValues);
			XRBarCode.Text = initialBarCodeText;
			XRBarCode.Padding = XRBarCode.DefaultPadding;
		}
		internal override string GetBindablePropName(DataInfo data) {
			using(XRDataContext dataContext = new XRDataContext(null, true)) {
				Type type = dataContext.GetPropertyType(data.Source, data.Member);
				if(type.IsAssignableFrom(typeof(byte[])))
					return "BinaryData";
				return base.GetBindablePropName(data);
			}
		}
		protected override bool ShouldAddBindableProperty(PropertyDescriptor property) {
			if(property.Name == "BinaryData" && !(XRBarCode.Symbology is BarCode2DGenerator))
				return false;
			return base.ShouldAddBindableProperty(property);
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new XRBarCodeDesignerActionList1(this));
			list.Add(new XRBarCodeDesignerActionList2(this));
			list.Add(new XRControlTextDesignerActionList(this));
			list.Add(new XRControlBookmarkDesignerActionList(this));
			list.Add(new XRFormattingControlDesignerActionList(this));
			list.Add(new XRBarCodeDesignerActionList3(this));
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			AllowPaddingDeserializationFromInt(properties);
		}
		static void AllowPaddingDeserializationFromInt(IDictionary properties) {
			properties[XRComponentPropertyNames.Padding] = new PaddingPropertyDescriptor((PropertyDescriptor)properties[XRComponentPropertyNames.Padding]);
		}
	}
	public class XRZipCodeDesigner : XRTextControlDesigner {
		protected XRZipCode XRZipCode { get { return Component as XRZipCode; }
		}
		public XRZipCodeDesigner() : base() {
		}
		protected override InplaceTextEditorBase GetInplaceEditor(string text, bool selectAll) {
			return new InplaceSingleTextEditor(fDesignerHost, XRZipCode, text, selectAll);
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			string initialZipCodeText = XRZipCode.Text;
			base.InitializeNewComponent(defaultValues);
			XRZipCode.Text = initialZipCodeText;
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new XRZipCodeDesignerActionList(this));
			list.Add(new XRControlBookmarkDesignerActionList(this));
			list.Add(new XRFormattingControlDesignerActionList(this));
		}
		protected override string[] GetFilteredProperties() {
			return new string[] { XRComponentPropertyNames.Font };
		}
	}
}
