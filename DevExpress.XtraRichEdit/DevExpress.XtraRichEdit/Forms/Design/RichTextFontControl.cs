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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Internal;
using DevExpress.Office.Model;
using DevExpress.Office.UI;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Design;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
#if !SL
using System.Drawing;
using DevExpress.XtraRichEdit.Forms;
#else
using System.Windows.Media;
#endif
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.RichEditFontControl.edtFont")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.RichEditFontControl.lblFont")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.RichEditFontControl.lblFontStyle")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.RichEditFontControl.lblFontSize")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.RichEditFontControl.lblFontColor")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.RichEditFontControl.lblUnderlineStyle")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.RichEditFontControl.lblUnderlineColor")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.RichEditFontControl.edtFontStyle")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.RichEditFontControl.cbForeColor")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.RichEditFontControl.cbUnderlineColor")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.RichEditFontControl.lblEffects")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.RichEditFontControl.fontEffects")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.RichEditFontControl.edtUnderlineStyle")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.RichEditFontControl.edtFontSize")]
#endregion
namespace DevExpress.XtraRichEdit.Design {
	[DXToolboxItem(false)]
	public class RichEditFontControl : DevExpress.XtraEditors.XtraUserControl, IBatchUpdateable, IBatchUpdateHandler {
		#region Fields
		RichEditControl richEditControl;
		protected DevExpress.XtraEditors.FontEdit edtFont;
		protected DevExpress.XtraEditors.LabelControl lblFont;
		protected DevExpress.XtraEditors.LabelControl lblFontStyle;
		protected DevExpress.XtraEditors.LabelControl lblFontSize;
		protected DevExpress.XtraEditors.LabelControl lblFontColor;
		protected DevExpress.XtraEditors.LabelControl lblUnderlineStyle;
		protected DevExpress.XtraEditors.LabelControl lblUnderlineColor;
		protected FontStyleEdit edtFontStyle;
		protected OfficeColorPickEdit cbUnderlineColor;
		protected DevExpress.XtraEditors.LabelControl lblEffects;
		protected FontEffects fontEffects;
		protected UnderlineStyleEdit edtUnderlineStyle;
		protected DevExpress.XtraRichEdit.Design.RichEditFontSizeEdit edtFontSize;
		BatchUpdateHelper batchUpdateHelper;
		bool defferedUpdateControl;
		protected OfficeColorPickEdit cbForeColor;
		bool defferedFontControlChanged;
		#endregion
		public RichEditFontControl() {
			batchUpdateHelper = new BatchUpdateHelper(this);
			InitializeComponent();
			SubscribeControlsEvents();
		}
		#region Properties
		#region RichEditControl
		protected override CreateParams CreateParams {
			get {
				return DevExpress.XtraRichEdit.Native.RightToLeftHelper.PatchCreateParams(base.CreateParams, this);
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RichEditControl RichEditControl {
			get { return richEditControl; }
			set {
				BeginUpdate();
				this.richEditControl = value;
				UpdateControl();
				OnFontControlChanged();
				EndUpdate();
			}
		}
		#endregion
		#region FontName
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string FontName {
			get { return edtFont.Text; }
			set {
				BeginUpdate();
				edtFont.Text = value;
				edtFontStyle.FontFamilyName = value;
				UpdateControl();
				OnFontControlChanged();
				EndUpdate();
			}
		}
		public bool FontNameAllowed {
			get { return edtFont.Enabled; }
			set {
				BeginUpdate();
				edtFont.Enabled = value;
				EndUpdate();
			}
		}
		#endregion
		#region FontBold
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool? FontBold {
			get { return edtFontStyle.FontBold; }
			set {
				BeginUpdate();
				edtFontStyle.FontBold = value;
				UpdateControl();
				OnFontControlChanged();
				EndUpdate();
			}
		}
		#endregion
		#region FontItalic
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool? FontItalic {
			get { return edtFontStyle.FontItalic; }
			set {
				BeginUpdate();
				edtFontStyle.FontItalic = value;
				UpdateControl();
				OnFontControlChanged();
				EndUpdate();
			}
		}
		#endregion
		#region FontForeColor
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Color? FontForeColor {
			get { return cbForeColor.Value; }
			set {
				BeginUpdate();
				cbForeColor.Value = value;
				UpdateControl();
				OnFontControlChanged();
				EndUpdate();
			}
		}
		#endregion
		#region FontUnderlineType
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public UnderlineType? FontUnderlineType {
			get { return edtUnderlineStyle.UnderlineType; }
			set {
				BeginUpdate();
				edtUnderlineStyle.UnderlineType = value;
				UpdateControl();
				OnFontControlChanged();
				EndUpdate();
			}
		}
		#endregion
		#region FontUnderlineColor
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Color? FontUnderlineColor {
			get { return cbUnderlineColor.Value; }
			set {
				BeginUpdate();
				cbUnderlineColor.Value = value;
				UpdateControl();
				OnFontControlChanged();
				EndUpdate();
			}
		}
		#endregion
		#region AllCaps
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool? AllCaps {
			get { return fontEffects.AllCaps; }
			set {
				BeginUpdate();
				fontEffects.AllCaps = value;
				UpdateControl();
				OnFontControlChanged();
				EndUpdate();
			}
		}
		#endregion
		#region Hidden
		public bool? Hidden {
			get { return fontEffects.Hidden; }
			set {
				BeginUpdate();
				fontEffects.Hidden = value;
				UpdateControl();
				OnFontControlChanged();
				EndUpdate();
			}
		}
		#endregion
		#region UnderlineWordsOnly
		public bool? UnderlineWordsOnly {
			get { return fontEffects.UnderlineWordsOnly; }
			set {
				BeginUpdate();
				fontEffects.UnderlineWordsOnly = value;
				UpdateControl();
				OnFontControlChanged();
				EndUpdate();
			}
		}
		#endregion
		#region Strikeout
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public StrikeoutType? Strikeout {
			get { return fontEffects.Strikeout; }
			set {
				BeginUpdate();
				fontEffects.Strikeout = value;
				UpdateControl();
				OnFontControlChanged();
				EndUpdate();
			}
		}
		#endregion
		#region Script
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CharacterFormattingScript? Script {
			get { return fontEffects.Script; }
			set {
				BeginUpdate();
				fontEffects.Script = value;
				UpdateControl();
				OnFontControlChanged();
				EndUpdate();
			}
		}
		#endregion
		#region FontSize
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]		
		public float? FontSize {
			get {
				int value;
				bool isValidValue = OfficeFontSizeEditHelper.TryGetHalfSizeValue(edtFontSize.EditValue, out value);
				if (isValidValue)
					return value/2f;
				else
					return null;
			}
			set {
				BeginUpdate();
				if (value == null)
					edtFontSize.EditValue = String.Empty;
				else
					edtFontSize.EditValue = value;
				UpdateControl();
				OnFontControlChanged();
				EndUpdate();
			}
		}
		#endregion
		#endregion
		#region Events
		#region FontControlChanged
		static readonly object fontControlChanged = new object();
		public event EventHandler FontControlChanged {
			add { Events.AddHandler(fontControlChanged, value); }
			remove { Events.RemoveHandler(fontControlChanged, value); }
		}
		protected internal virtual void RaiseFontControlChanged() {
			EventHandler handler = (EventHandler)this.Events[fontControlChanged];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		protected internal virtual void OnFontControlChanged() {
			if (IsUpdateLocked)
				defferedFontControlChanged = true;
			else
				RaiseFontControlChanged();
		}
		#endregion
		#endregion
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RichEditFontControl));
			this.edtFont = new DevExpress.XtraEditors.FontEdit();
			this.lblFont = new DevExpress.XtraEditors.LabelControl();
			this.lblFontStyle = new DevExpress.XtraEditors.LabelControl();
			this.lblFontSize = new DevExpress.XtraEditors.LabelControl();
			this.lblFontColor = new DevExpress.XtraEditors.LabelControl();
			this.lblUnderlineStyle = new DevExpress.XtraEditors.LabelControl();
			this.lblUnderlineColor = new DevExpress.XtraEditors.LabelControl();
			this.lblEffects = new DevExpress.XtraEditors.LabelControl();
			this.cbUnderlineColor = new OfficeColorPickEdit();
			this.cbForeColor = new OfficeColorPickEdit();
			this.edtFontSize = new DevExpress.XtraRichEdit.Design.RichEditFontSizeEdit();
			this.edtUnderlineStyle = new DevExpress.XtraRichEdit.Design.UnderlineStyleEdit();
			this.fontEffects = new DevExpress.XtraRichEdit.Design.FontEffects();
			this.edtFontStyle = new DevExpress.XtraRichEdit.Design.FontStyleEdit();
			((System.ComponentModel.ISupportInitialize)(this.edtFont.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbUnderlineColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbForeColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFontSize.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtUnderlineStyle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFontStyle.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.edtFont, "edtFont");
			this.edtFont.Name = "edtFont";
			this.edtFont.Properties.AccessibleName = resources.GetString("edtFont.Properties.AccessibleName");
			this.edtFont.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtFont.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtFont.Properties.Buttons"))))});
			resources.ApplyResources(this.lblFont, "lblFont");
			this.lblFont.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblFont.Name = "lblFont";
			resources.ApplyResources(this.lblFontStyle, "lblFontStyle");
			this.lblFontStyle.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblFontStyle.Name = "lblFontStyle";
			resources.ApplyResources(this.lblFontSize, "lblFontSize");
			this.lblFontSize.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblFontSize.Name = "lblFontSize";
			resources.ApplyResources(this.lblFontColor, "lblFontColor");
			this.lblFontColor.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblFontColor.Name = "lblFontColor";
			resources.ApplyResources(this.lblUnderlineStyle, "lblUnderlineStyle");
			this.lblUnderlineStyle.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblUnderlineStyle.Name = "lblUnderlineStyle";
			resources.ApplyResources(this.lblUnderlineColor, "lblUnderlineColor");
			this.lblUnderlineColor.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblUnderlineColor.Name = "lblUnderlineColor";
			resources.ApplyResources(this.lblEffects, "lblEffects");
			this.lblEffects.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblEffects.LineVisible = true;
			this.lblEffects.Name = "lblEffects";
			resources.ApplyResources(this.cbUnderlineColor, "cbUnderlineColor");
			this.cbUnderlineColor.Name = "cbUnderlineColor";
			this.cbUnderlineColor.Properties.AccessibleName = resources.GetString("cbUnderlineColor.Properties.AccessibleName");
			this.cbUnderlineColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbUnderlineColor.Properties.Buttons"))))});
			resources.ApplyResources(this.cbForeColor, "cbForeColor");
			this.cbForeColor.Name = "cbForeColor";
			this.cbForeColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbForeColor.Properties.Buttons"))))});
			this.cbForeColor.Properties.ColorAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.cbForeColor.Properties.ColorDialogOptions.AllowTransparency = false;
			this.cbForeColor.Properties.ShowSystemColors = false;
			this.cbForeColor.Properties.ShowWebColors = false;
			this.edtFontSize.Control = null;
			resources.ApplyResources(this.edtFontSize, "edtFontSize");
			this.edtFontSize.Name = "edtFontSize";
			this.edtFontSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtFontSize.Properties.Buttons"))))});
			this.edtFontSize.Properties.Control = null;
			resources.ApplyResources(this.edtUnderlineStyle, "edtUnderlineStyle");
			this.edtUnderlineStyle.Name = "edtUnderlineStyle";
			this.edtUnderlineStyle.Properties.AccessibleName = resources.GetString("edtUnderlineStyle.Properties.AccessibleName");
			this.edtUnderlineStyle.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtUnderlineStyle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtUnderlineStyle.Properties.Buttons"))))});
			resources.ApplyResources(this.fontEffects, "fontEffects");
			this.fontEffects.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
			this.fontEffects.Name = "fontEffects";
			resources.ApplyResources(this.edtFontStyle, "edtFontStyle");
			this.edtFontStyle.Name = "edtFontStyle";
			this.edtFontStyle.Properties.AccessibleName = resources.GetString("edtFontStyle.Properties.AccessibleName");
			this.edtFontStyle.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtFontStyle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtFontStyle.Properties.Buttons"))))});
			resources.ApplyResources(this, "$this");
			this.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("RichEditFontControl.Appearance.ForeColor")));
			this.Appearance.Options.UseForeColor = true;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.cbForeColor);
			this.Controls.Add(this.edtFontSize);
			this.Controls.Add(this.edtUnderlineStyle);
			this.Controls.Add(this.edtFont);
			this.Controls.Add(this.lblFont);
			this.Controls.Add(this.lblFontStyle);
			this.Controls.Add(this.lblFontSize);
			this.Controls.Add(this.lblFontColor);
			this.Controls.Add(this.fontEffects);
			this.Controls.Add(this.lblUnderlineStyle);
			this.Controls.Add(this.lblEffects);
			this.Controls.Add(this.lblUnderlineColor);
			this.Controls.Add(this.cbUnderlineColor);
			this.Controls.Add(this.edtFontStyle);
			this.Name = "RichEditFontControl";
			((System.ComponentModel.ISupportInitialize)(this.edtFont.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbUnderlineColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbForeColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFontSize.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtUnderlineStyle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFontStyle.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		#region SubscribeControlsEvents
		protected internal virtual void SubscribeControlsEvents() {
			this.edtFont.EditValueChanged += new EventHandler(OnSomeChildControlEditValueChanged);
			this.edtFontStyle.EditValueChanged += new EventHandler(OnSomeChildControlEditValueChanged);
			this.edtFontSize.Validating += new CancelEventHandler(OnFontSizeValidating);
			this.edtFontSize.Validated += new EventHandler(OnSomeChildControlEditValueChanged);
			this.edtUnderlineStyle.EditValueChanged += new EventHandler(OnSomeChildControlEditValueChanged);
			this.cbForeColor.EditValueChanged += new EventHandler(OnSomeChildControlEditValueChanged);
			this.cbUnderlineColor.EditValueChanged += new EventHandler(OnSomeChildControlEditValueChanged);
			this.fontEffects.EffectsChanged += new EventHandler(OnSomeChildControlEditValueChanged);
		}
		#endregion
		#region UnsubscribeControlsEvents
		protected internal virtual void UnsubscribeControlsEvents() {
			this.edtFont.EditValueChanged -= new EventHandler(OnSomeChildControlEditValueChanged);
			this.edtFontStyle.EditValueChanged -= new EventHandler(OnSomeChildControlEditValueChanged);
			this.edtFontSize.Validating -= new CancelEventHandler(OnFontSizeValidating);
			this.edtFontSize.Validated -= new EventHandler(OnSomeChildControlEditValueChanged);
			this.edtUnderlineStyle.EditValueChanged -= new EventHandler(OnSomeChildControlEditValueChanged);
			this.cbForeColor.EditValueChanged -= new EventHandler(OnSomeChildControlEditValueChanged);
			this.cbUnderlineColor.EditValueChanged -= new EventHandler(OnSomeChildControlEditValueChanged);
			this.fontEffects.EffectsChanged -= new EventHandler(OnSomeChildControlEditValueChanged);
		}
		#endregion
		protected internal virtual void OnFontSizeValidating(object sender, CancelEventArgs e) {
			RichEditFontSizeEdit edit = (RichEditFontSizeEdit)sender;
			e.Cancel = false;
			string text = String.Empty;	 
			if (edit.EditValue is String && String.IsNullOrEmpty(edit.EditValue as string))
				return;
			e.Cancel = !EditStyleHelper.IsFontSizeValid(edit.EditValue, out text);
			if (e.Cancel)
				edit.ErrorText = text;			
		}
		protected internal virtual void OnSomeChildControlEditValueChanged(object sender, EventArgs e) {
			UpdateControl();
			OnFontControlChanged();
		}
		protected internal virtual void UpdateControl() {
			if (IsUpdateLocked)
				defferedUpdateControl = true;
			else {
				UnsubscribeControlsEvents();
				UpdateControlCore();
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void UpdateControlCore() {
			this.edtFontSize.Control = RichEditControl;
			if (RichEditControl != null)
				this.edtUnderlineStyle.Properties.UnderlineRepository = RichEditControl.DocumentModel.UnderlineRepository;
			this.edtFontStyle.FontFamilyName = edtFont.Text;
			this.cbUnderlineColor.Enabled = !(edtUnderlineStyle.UnderlineType == UnderlineType.None);
			if (edtUnderlineStyle.UnderlineType == UnderlineType.None)
				cbUnderlineColor.Color = Color.Empty;
			if (RichEditControl != null) {
				CharacterFormattingDetailedOptions restrictions = RichEditControl.DocumentModel.DocumentCapabilities.CharacterFormattingDetailed;
				if (fontEffects.CharacterFormattingDetailedOptions == null)
					fontEffects.CharacterFormattingDetailedOptions = restrictions;
				cbForeColor.Enabled = restrictions.ForeColorAllowed;
				cbUnderlineColor.Enabled = restrictions.UnderlineColorAllowed;
				edtFontSize.Enabled = restrictions.FontSizeAllowed;
				edtFont.Enabled = restrictions.FontNameAllowed;
				edtUnderlineStyle.Enabled = restrictions.FontUnderlineAllowed;
			}
		}
		#region IBatchUpdateable Members
		public void BeginUpdate() {
			this.batchUpdateHelper.BeginUpdate();
			this.edtFontStyle.BeginUpdate();
			this.fontEffects.BeginUpdate();
		}
		public void CancelUpdate() {
			this.batchUpdateHelper.CancelUpdate();
			this.edtFontStyle.CancelUpdate();
			this.fontEffects.CancelUpdate();
		}
		public void EndUpdate() {
			this.edtFontStyle.EndUpdate();
			this.fontEffects.EndUpdate();
			this.batchUpdateHelper.EndUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler Members
		public void OnBeginUpdate() {
		}
		public void OnCancelUpdate() {
		}
		public void OnEndUpdate() {
		}
		public void OnFirstBeginUpdate() {
			UnsubscribeControlsEvents();
			this.defferedUpdateControl = false;
			this.defferedFontControlChanged = false;
		}
		public void OnLastCancelUpdate() {
			SubscribeControlsEvents();
		}
		public void OnLastEndUpdate() {
			if (this.defferedUpdateControl)
				UpdateControlCore();
			if (this.defferedFontControlChanged)
				RaiseFontControlChanged();
			SubscribeControlsEvents();
		}
		#endregion
	}
}
