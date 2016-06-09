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
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Model;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.FontEffects.chkSuperscript")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.FontEffects.chkSubscript")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.FontEffects.chkAllCaps")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.FontEffects.chkStrikethrough")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.FontEffects.chkDoubleStrikethrough")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.FontEffects.chkHidden")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.FontEffects.chkUnderlineWordsOnly")]
#endregion
namespace DevExpress.XtraRichEdit.Design {
	[DXToolboxItem(false), ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public class FontEffects : DevExpress.XtraEditors.XtraUserControl, IBatchUpdateable, IBatchUpdateHandler {
		#region Fields
		protected DevExpress.XtraEditors.CheckEdit chkSuperscript;
		protected DevExpress.XtraEditors.CheckEdit chkSubscript;
		protected DevExpress.XtraEditors.CheckEdit chkAllCaps;
		protected DevExpress.XtraEditors.CheckEdit chkStrikethrough;
		protected DevExpress.XtraEditors.CheckEdit chkDoubleStrikethrough;
		protected DevExpress.XtraEditors.CheckEdit chkHidden;
		protected DevExpress.XtraEditors.CheckEdit chkUnderlineWordsOnly;
		bool? allCaps;
		StrikeoutType? strikeout;
		CharacterFormattingScript? script;
		bool? hidden;
		bool? underlineWordsOnly;
		CharacterFormattingDetailedOptions characterFormattingDetailedOptions;
		BatchUpdateHelper batchUpdateHelper;
		bool deferredRaiseChanged;
		#endregion
		public FontEffects() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.script = CharacterFormattingScript.Normal;
			this.strikeout = StrikeoutType.None;
			this.allCaps = false;
			this.underlineWordsOnly = false;
			this.hidden = false;
			characterFormattingDetailedOptions = null;
			InitializeComponent();
			SubscribeEvents();
			UpdateControl();
		}
		#region Properties
		protected override CreateParams CreateParams {
			get {
				return DevExpress.XtraRichEdit.Native.RightToLeftHelper.PatchCreateParams(base.CreateParams, this);
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public StrikeoutType? Strikeout {
			get { return strikeout; }
			set {
				if (Strikeout == value)
					return;
				strikeout = value;
				UpdateControl();
				OnEffectsChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CharacterFormattingScript? Script {
			get { return script; }
			set {
				if (Script == value)
					return;
				script = value;
				UpdateControl();
				OnEffectsChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool? AllCaps {
			get { return allCaps; }
			set {
				if (AllCaps == value)
					return;
				allCaps = value;
				UpdateControl();
				OnEffectsChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CharacterFormattingDetailedOptions CharacterFormattingDetailedOptions {
			get { return characterFormattingDetailedOptions; }
			set {
				if (CharacterFormattingDetailedOptions == value)
					return;
				characterFormattingDetailedOptions = value;
				UpdateControl();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool? Hidden {
			get { return hidden; }
			set {
				if (Hidden == value)
					return;
				hidden = value;
				UpdateControl();
				OnEffectsChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool? UnderlineWordsOnly {
			get { return underlineWordsOnly; }
			set {
				if (UnderlineWordsOnly == value)
					return;
				underlineWordsOnly = value;
				UpdateControl();
				OnEffectsChanged();
			}
		}
		#endregion
		#region Events
		#region EffectsChanged
		static readonly object effectsChanged = new object();
		public event EventHandler EffectsChanged {
			add { Events.AddHandler(effectsChanged, value); }
			remove { Events.RemoveHandler(effectsChanged, value); }
		}
		protected internal virtual void RaiseEffectsChanged() {
			EventHandler handler = (EventHandler)this.Events[effectsChanged];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FontEffects));
			this.chkStrikethrough = new DevExpress.XtraEditors.CheckEdit();
			this.chkDoubleStrikethrough = new DevExpress.XtraEditors.CheckEdit();
			this.chkSuperscript = new DevExpress.XtraEditors.CheckEdit();
			this.chkSubscript = new DevExpress.XtraEditors.CheckEdit();
			this.chkAllCaps = new DevExpress.XtraEditors.CheckEdit();
			this.chkHidden = new DevExpress.XtraEditors.CheckEdit();
			this.chkUnderlineWordsOnly = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.chkStrikethrough.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDoubleStrikethrough.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkSuperscript.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkSubscript.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAllCaps.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkHidden.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkUnderlineWordsOnly.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chkStrikethrough, "chkStrikethrough");
			this.chkStrikethrough.Name = "chkStrikethrough";
			this.chkStrikethrough.Properties.AccessibleName = resources.GetString("chkStrikethrough.Properties.AccessibleName");
			this.chkStrikethrough.Properties.AutoWidth = true;
			this.chkStrikethrough.Properties.Caption = resources.GetString("chkStrikethrough.Properties.Caption");
			resources.ApplyResources(this.chkDoubleStrikethrough, "chkDoubleStrikethrough");
			this.chkDoubleStrikethrough.Name = "chkDoubleStrikethrough";
			this.chkDoubleStrikethrough.Properties.AccessibleName = resources.GetString("chkDoubleStrikethrough.Properties.AccessibleName");
			this.chkDoubleStrikethrough.Properties.AutoWidth = true;
			this.chkDoubleStrikethrough.Properties.Caption = resources.GetString("chkDoubleStrikethrough.Properties.Caption");
			resources.ApplyResources(this.chkSuperscript, "chkSuperscript");
			this.chkSuperscript.Name = "chkSuperscript";
			this.chkSuperscript.Properties.AccessibleName = resources.GetString("chkSuperscript.Properties.AccessibleName");
			this.chkSuperscript.Properties.AutoWidth = true;
			this.chkSuperscript.Properties.Caption = resources.GetString("chkSuperscript.Properties.Caption");
			resources.ApplyResources(this.chkSubscript, "chkSubscript");
			this.chkSubscript.Name = "chkSubscript";
			this.chkSubscript.Properties.AccessibleName = resources.GetString("chkSubscript.Properties.AccessibleName");
			this.chkSubscript.Properties.AutoWidth = true;
			this.chkSubscript.Properties.Caption = resources.GetString("chkSubscript.Properties.Caption");
			resources.ApplyResources(this.chkAllCaps, "chkAllCaps");
			this.chkAllCaps.Name = "chkAllCaps";
			this.chkAllCaps.Properties.AutoWidth = true;
			this.chkAllCaps.Properties.Caption = resources.GetString("chkAllCaps.Properties.Caption");
			resources.ApplyResources(this.chkHidden, "chkHidden");
			this.chkHidden.Name = "chkHidden";
			this.chkHidden.Properties.AccessibleName = resources.GetString("chkHidden.Properties.AccessibleName");
			this.chkHidden.Properties.AutoWidth = true;
			this.chkHidden.Properties.Caption = resources.GetString("chkHidden.Properties.Caption");
			resources.ApplyResources(this.chkUnderlineWordsOnly, "chkUnderlineWordsOnly");
			this.chkUnderlineWordsOnly.Name = "chkUnderlineWordsOnly";
			this.chkUnderlineWordsOnly.Properties.AccessibleName = resources.GetString("chkUnderlineWordsOnly.Properties.AccessibleName");
			this.chkUnderlineWordsOnly.Properties.AutoWidth = true;
			this.chkUnderlineWordsOnly.Properties.Caption = resources.GetString("chkUnderlineWordsOnly.Properties.Caption");
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.chkUnderlineWordsOnly);
			this.Controls.Add(this.chkHidden);
			this.Controls.Add(this.chkAllCaps);
			this.Controls.Add(this.chkSubscript);
			this.Controls.Add(this.chkSuperscript);
			this.Controls.Add(this.chkDoubleStrikethrough);
			this.Controls.Add(this.chkStrikethrough);
			this.Name = "FontEffects";
			((System.ComponentModel.ISupportInitialize)(this.chkStrikethrough.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDoubleStrikethrough.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkSuperscript.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkSubscript.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAllCaps.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkHidden.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkUnderlineWordsOnly.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected internal virtual void SubscribeEvents() {
			chkStrikethrough.CheckStateChanged += new EventHandler(OnStrikethroughCheckedChanged);
			chkDoubleStrikethrough.CheckStateChanged += new EventHandler(OnDoubleStrikethroughCheckedChanged);
			chkSuperscript.CheckStateChanged += new EventHandler(OnSuperscriptCheckedChanged);
			chkSubscript.CheckStateChanged += new EventHandler(OnSubscriptCheckedChanged);
			chkAllCaps.CheckStateChanged += new EventHandler(OnAllCapsCheckedChanged);
			chkHidden.CheckStateChanged += new EventHandler(OnHiddenCheckedChanged);
			chkUnderlineWordsOnly.CheckStateChanged += new EventHandler(OnUnderlineWordsOnlyCheckedChanged);
		}
		protected internal virtual void UnsubscribeEvents() {
			chkStrikethrough.CheckStateChanged -= new EventHandler(OnStrikethroughCheckedChanged);
			chkDoubleStrikethrough.CheckStateChanged -= new EventHandler(OnDoubleStrikethroughCheckedChanged);
			chkSuperscript.CheckStateChanged -= new EventHandler(OnSuperscriptCheckedChanged);
			chkSubscript.CheckStateChanged -= new EventHandler(OnSubscriptCheckedChanged);
			chkAllCaps.CheckStateChanged -= new EventHandler(OnAllCapsCheckedChanged);
			chkHidden.CheckStateChanged -= new EventHandler(OnHiddenCheckedChanged);
			chkUnderlineWordsOnly.CheckStateChanged -= new EventHandler(OnUnderlineWordsOnlyCheckedChanged);
		}
		protected internal virtual void OnStrikethroughCheckedChanged(object sender, EventArgs e) {
			switch (chkStrikethrough.CheckState) {
				case CheckState.Unchecked:
					Strikeout = StrikeoutType.None;
					break;
				case CheckState.Checked:
					Strikeout = StrikeoutType.Single;
					break;
				case CheckState.Indeterminate:
					Strikeout = null;
					break;
			}
		}
		protected internal virtual void OnDoubleStrikethroughCheckedChanged(object sender, EventArgs e) {
			switch (chkDoubleStrikethrough.CheckState) {
				case CheckState.Unchecked:
					Strikeout = StrikeoutType.None;
					break;
				case CheckState.Checked:
					Strikeout = StrikeoutType.Double;
					break;
				case CheckState.Indeterminate:
					Strikeout = null;
					break;
			}
		}
		protected internal virtual void OnSuperscriptCheckedChanged(object sender, EventArgs e) {
			switch (chkSuperscript.CheckState) {
				case CheckState.Checked:
					Script = CharacterFormattingScript.Superscript;
					break;
				case CheckState.Unchecked:
					Script = CharacterFormattingScript.Normal;
					break;
				case CheckState.Indeterminate:
					Script = null;
					break;
			}
		}
		protected internal virtual void OnSubscriptCheckedChanged(object sender, EventArgs e) {
			switch (chkSubscript.CheckState) {
				case CheckState.Checked:
					Script = CharacterFormattingScript.Subscript;
					break;
				case CheckState.Unchecked:
					Script = CharacterFormattingScript.Normal;
					break;
				case CheckState.Indeterminate:
					Script = null;
					break;
			}
		}
		protected internal virtual void OnAllCapsCheckedChanged(object sender, EventArgs e) {
			AllCaps = CheckStateToNullableBool(this.chkAllCaps.CheckState);
		}
		protected internal virtual void OnUnderlineWordsOnlyCheckedChanged(object sender, EventArgs e) {
			UnderlineWordsOnly = CheckStateToNullableBool(chkUnderlineWordsOnly.CheckState);
		}
		protected internal virtual void OnHiddenCheckedChanged(object sender, EventArgs e) {
			Hidden = CheckStateToNullableBool(chkHidden.CheckState);
		}
		private bool? CheckStateToNullableBool(CheckState checkState) {
			switch (checkState) {
				case CheckState.Checked:
					return true;
				case CheckState.Unchecked:
					return false;
				case CheckState.Indeterminate:
					return null;
			}
			return null;
		}
		protected internal virtual void UpdateControl() {
			BeginUpdate();
			UpdateStrikethroughAndDoubleStrikethroughCheckEdits();
			UpdateSuperscriptAndSubscriptCheckEdits();
			UpdateAllCapsCheckEdit();
			UpdateCheckEdit(this.chkHidden, Hidden);
			UpdateCheckEdit(this.chkUnderlineWordsOnly, UnderlineWordsOnly);
			UpdateRestrictions();
			EndUpdate();
		}
		protected internal virtual void UpdateRestrictions() {
			if (CharacterFormattingDetailedOptions == null)
				return;
			this.chkAllCaps.Enabled = CharacterFormattingDetailedOptions.AllCapsAllowed;
			this.chkUnderlineWordsOnly.Enabled = CharacterFormattingDetailedOptions.UnderlineWordsOnlyAllowed;
			this.chkSubscript.Enabled = CharacterFormattingDetailedOptions.ScriptAllowed;
			this.chkSuperscript.Enabled = CharacterFormattingDetailedOptions.ScriptAllowed;
			this.chkHidden.Enabled = CharacterFormattingDetailedOptions.HiddenAllowed;
			this.chkStrikethrough.Enabled = CharacterFormattingDetailedOptions.FontStrikeoutAllowed;
			this.chkDoubleStrikethrough.Enabled = CharacterFormattingDetailedOptions.FontStrikeoutAllowed;
		}
		protected internal virtual void UpdateAllCapsCheckEdit() {
			if (AllCaps == null) {
				this.chkAllCaps.Properties.AllowGrayed = true;
				this.chkAllCaps.CheckState = CheckState.Indeterminate;
				return;
			}
			this.chkAllCaps.CheckState = (AllCaps == true) ? CheckState.Checked : CheckState.Unchecked;
		}
		protected internal virtual void UpdateCheckEdit(CheckEdit checkEdit, bool? state) {
			if (!state.HasValue) {
				checkEdit.Properties.AllowGrayed = true;
				checkEdit.CheckState = CheckState.Indeterminate;
				return;
			}
			checkEdit.CheckState = (state == true) ? CheckState.Checked : CheckState.Unchecked;
		}
		protected internal virtual void UpdateStrikethroughAndDoubleStrikethroughCheckEdits() {
			if (Strikeout == null) {
				this.chkStrikethrough.Properties.AllowGrayed = true;
				this.chkDoubleStrikethrough.Properties.AllowGrayed = true;
				this.chkStrikethrough.CheckState = CheckState.Indeterminate;
				this.chkDoubleStrikethrough.CheckState = CheckState.Indeterminate;
			}
			else if (Strikeout == StrikeoutType.Single) {
				this.chkStrikethrough.CheckState = CheckState.Checked;
				this.chkDoubleStrikethrough.CheckState = CheckState.Unchecked;
			}
			else
				if (Strikeout == StrikeoutType.Double) {
					this.chkStrikethrough.CheckState = CheckState.Unchecked;
					this.chkDoubleStrikethrough.CheckState = CheckState.Checked;
				}
				else if (Strikeout == StrikeoutType.None) {
					this.chkStrikethrough.CheckState = CheckState.Unchecked;
					this.chkDoubleStrikethrough.CheckState = CheckState.Unchecked;
				}
				else {
					this.chkStrikethrough.CheckState = CheckState.Indeterminate;
					this.chkDoubleStrikethrough.CheckState = CheckState.Indeterminate;
				}
		}
		protected internal virtual void UpdateSuperscriptAndSubscriptCheckEdits() {
			if (Script == null) {
				this.chkSuperscript.Properties.AllowGrayed = true;
				this.chkSubscript.Properties.AllowGrayed = true;
				this.chkSuperscript.CheckState = CheckState.Indeterminate;
				this.chkSubscript.CheckState = CheckState.Indeterminate;
			}
			else if (Script.Value == CharacterFormattingScript.Superscript) {
				this.chkSuperscript.CheckState = CheckState.Checked;
				this.chkSubscript.CheckState = CheckState.Unchecked;
			}
			else
				if (Script.Value == CharacterFormattingScript.Subscript) {
					this.chkSuperscript.CheckState = CheckState.Unchecked;
					this.chkSubscript.CheckState = CheckState.Checked;
				}
				else {
					this.chkSuperscript.CheckState = CheckState.Unchecked;
					this.chkSubscript.CheckState = CheckState.Unchecked;
				}
		}
		protected internal virtual void OnEffectsChanged() {
			if (IsUpdateLocked)
				deferredRaiseChanged = true;
			else
				RaiseEffectsChanged();
		}
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			UnsubscribeEvents();
			deferredRaiseChanged = false;
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if (deferredRaiseChanged)
				RaiseEffectsChanged();
			SubscribeEvents();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		#endregion
	}
}
