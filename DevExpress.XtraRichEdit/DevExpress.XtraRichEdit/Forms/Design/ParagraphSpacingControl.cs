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
using System.ComponentModel;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.XtraEditors;
using System.Globalization;
using DevExpress.XtraEditors.Repository;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.ParagraphSpacingControl.lblBefore")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.ParagraphSpacingControl.lblAfter")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.ParagraphSpacingControl.lblLineSpacing")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.ParagraphSpacingControl.lblAt")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.ParagraphSpacingControl.edtLineSpacing")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.ParagraphSpacingControl.spnSpacingAfter")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.ParagraphSpacingControl.spnAtSpacing")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.ParagraphSpacingControl.spnSpacingBefore")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.ParagraphSpacingControl.spnAtFloatSpacing")]
#endregion
namespace DevExpress.XtraRichEdit.Design {
	[DXToolboxItem(false)]
	public partial class ParagraphSpacingControl : UserControl, IBatchUpdateable, IBatchUpdateHandler {
		const decimal MinMultiplierValue = 0.06M; 
		const decimal MaxMultiplierValue = 132M; 
		const int DefaultLineSpacingExactlyValue = 240; 
		const float DefaultLineSpacingMultipleValue = 3.0f; 
		ParagraphSpacingProperties properties;
		BatchUpdateHelper batchUpdateHelper;
		bool deferredRaiseChanged;
		float? shadowSpnAtSpacingValue;
		float? shadowSpnAtFloatSpacingValue;
		public ParagraphSpacingControl() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.properties = new ParagraphSpacingProperties();
			InitializeComponent();
			InitializeControls();
			SubscribeEvents();
		}
		public ParagraphSpacingProperties Properties { get { return properties; } }
		protected override CreateParams CreateParams {
			get {
				return DevExpress.XtraRichEdit.Native.RightToLeftHelper.PatchCreateParams(base.CreateParams, this);
			}
		}
		#region Events
		#region ParagraphSpacingControlChanged
		static readonly object paragraphSpacingControlChanged = new object();
		public event EventHandler ParagraphSpacingControlChanged {
			add { Events.AddHandler(paragraphSpacingControlChanged, value); }
			remove { Events.RemoveHandler(paragraphSpacingControlChanged, value); }
		}
		protected internal virtual void RaiseParagraphSpacingControlChanged() {
			EventHandler handler = (EventHandler)this.Events[paragraphSpacingControlChanged];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal void InitializeControls() {
			this.spnAtSpacing.Value = null;
			this.spnAtFloatSpacing.EditValue = null;
			this.shadowSpnAtSpacingValue = null;
			this.shadowSpnAtFloatSpacingValue = null;
			this.spnAtFloatSpacing.Properties.MinValue = MinMultiplierValue;
			this.spnAtFloatSpacing.Properties.MaxValue = MaxMultiplierValue;
			UpdateControl();
		}
		protected internal virtual void SubscribeEvents() {
			Properties.PropertiesChanged += new EventHandler(OnPropertiesChanged);
			this.edtLineSpacing.SelectedIndexChanged += new EventHandler(OnEdtLineSpacingSelectedIndexChanged);
			this.spnSpacingAfter.ValueChanged += new EventHandler(OnSpnSpacingAfterValueChanged);
			this.spnSpacingBefore.ValueChanged += new EventHandler(OnSpnSpacingBeforeValueChanged);
			this.spnAtSpacing.ValueChanged += new EventHandler(OnSpnAtSpacingValueChanged);
			this.spnAtFloatSpacing.ValueChanged += new EventHandler(OnSpnAtFloatSpacingValueChanged);
		}
		protected internal virtual void UnsubscribeEvents() {
			Properties.PropertiesChanged -= new EventHandler(OnPropertiesChanged);
			this.edtLineSpacing.SelectedIndexChanged -= new EventHandler(OnEdtLineSpacingSelectedIndexChanged);
			this.spnSpacingAfter.ValueChanged -= new EventHandler(OnSpnSpacingAfterValueChanged);
			this.spnSpacingBefore.ValueChanged -= new EventHandler(OnSpnSpacingBeforeValueChanged);
			this.spnAtSpacing.ValueChanged -= new EventHandler(OnSpnAtSpacingValueChanged);
			this.spnAtFloatSpacing.ValueChanged -= new EventHandler(OnSpnAtFloatSpacingValueChanged);
		}
		protected internal virtual void OnEdtLineSpacingSelectedIndexChanged(object sender, EventArgs e) {
			Properties.BeginUpdate();
			try {
				Properties.LineSpacingType = edtLineSpacing.LineSpacing;
				if (IsLineSpacingInsignificant()) {
					Properties.LineSpacing = null;
					return;
				}
				if (Properties.LineSpacingType == ParagraphLineSpacing.Multiple) {
					if (!this.shadowSpnAtFloatSpacingValue.HasValue)
						this.shadowSpnAtFloatSpacingValue = DefaultLineSpacingMultipleValue;
					Properties.LineSpacing = this.shadowSpnAtFloatSpacingValue;
				}
				if (Properties.LineSpacingType == ParagraphLineSpacing.Exactly || Properties.LineSpacingType == ParagraphLineSpacing.AtLeast) {
					if (!this.shadowSpnAtSpacingValue.HasValue)
						this.shadowSpnAtSpacingValue = Properties.ValueUnitConverter.TwipsToModelUnits(DefaultLineSpacingExactlyValue);
					Properties.LineSpacing = this.shadowSpnAtSpacingValue;
				}
			}
			finally {
				Properties.EndUpdate();
			}
		}
		bool IsLineSpacingInsignificant() {
			return !Properties.LineSpacingType.HasValue ||
				Properties.LineSpacingType == ParagraphLineSpacing.Single ||
				Properties.LineSpacingType == ParagraphLineSpacing.Double ||
				Properties.LineSpacingType == ParagraphLineSpacing.Sesquialteral;
		}
		protected internal virtual void OnSpnAtSpacingValueChanged(object sender, EventArgs e) {
			Properties.BeginUpdate();
			try {
				Properties.LineSpacing = this.spnAtSpacing.Value;
				this.shadowSpnAtSpacingValue = Properties.LineSpacing;
			}
			finally {
				Properties.EndUpdate();
			}
		}
		protected internal virtual void OnSpnAtFloatSpacingValueChanged(object sender, EventArgs e) {
			Properties.BeginUpdate();
			try {
				Properties.LineSpacingType = ParagraphLineSpacing.Multiple;
				Properties.LineSpacing = (float)this.spnAtFloatSpacing.Value;
				this.shadowSpnAtFloatSpacingValue = Properties.LineSpacing;
			}
			finally {
				Properties.EndUpdate();
			}
		}
		protected internal virtual void OnSpnSpacingBeforeValueChanged(object sender, EventArgs e) {
			Properties.SpacingBefore = this.spnSpacingBefore.Value;
		}
		protected internal virtual void OnSpnSpacingAfterValueChanged(object sender, EventArgs e) {
			Properties.SpacingAfter = this.spnSpacingAfter.Value;
		}
		protected internal virtual void OnPropertiesChanged(object sender, EventArgs e) {
			UnsubscribeEvents();
			try {
				UpdateControl();
			}
			finally {
				SubscribeEvents();
			}
			OnParagraphSpacingControlChanged();
		}
		void UpdateControl() {
			this.spnAtSpacing.ValueUnitConverter = Properties.ValueUnitConverter;
			this.spnSpacingAfter.ValueUnitConverter = Properties.ValueUnitConverter;
			this.spnSpacingBefore.ValueUnitConverter = Properties.ValueUnitConverter;
			this.spnSpacingAfter.Properties.MinValue = 0;
			this.spnSpacingAfter.Properties.MaxValue = Properties.MaxSpacing;
			this.spnSpacingBefore.Properties.MinValue = 0;
			this.spnSpacingBefore.Properties.MaxValue = Properties.MaxSpacing;
			this.spnSpacingAfter.Properties.DefaultUnitType = Properties.UnitType;
			this.spnSpacingBefore.Properties.DefaultUnitType = Properties.UnitType;
			this.spnAtSpacing.Properties.DefaultUnitType = Properties.UnitType;
			this.spnAtSpacing.Properties.MaxValue = Properties.MaxLineSpacing;
			this.spnAtSpacing.Properties.MinValue = Properties.MinLineSpacing;
			this.spnSpacingAfter.Value = Properties.SpacingAfter;
			this.spnSpacingBefore.Value = Properties.SpacingBefore;
			UpdateLineSpacing();
		}
		void UpdateLineSpacing() {
			this.edtLineSpacing.LineSpacing = Properties.LineSpacingType;
			bool isSpnAtFloatSpacingVisible = !Properties.LineSpacingType.HasValue ||
												Properties.LineSpacingType == ParagraphLineSpacing.Single ||
												Properties.LineSpacingType == ParagraphLineSpacing.Double ||
												Properties.LineSpacingType == ParagraphLineSpacing.Multiple ||
												Properties.LineSpacingType == ParagraphLineSpacing.Sesquialteral;
			this.spnAtFloatSpacing.Enabled = spnAtFloatSpacing.Visible = isSpnAtFloatSpacingVisible;
			this.spnAtSpacing.Enabled = spnAtSpacing.Visible = !isSpnAtFloatSpacingVisible;
			if (isSpnAtFloatSpacingVisible) {
				double? value = null;
				if (Properties.LineSpacingType == ParagraphLineSpacing.Multiple) {
					if (Properties.LineSpacing.HasValue) {
						value = Math.Round(Properties.LineSpacing.Value, 2);
					}
				}
				this.spnAtFloatSpacing.EditValue = value;
			}
			else
				this.spnAtSpacing.Value = (int?)Properties.LineSpacing;
		}
		protected internal virtual void OnParagraphSpacingControlChanged() {
			if (IsUpdateLocked)
				deferredRaiseChanged = true;
			else
				RaiseParagraphSpacingControlChanged();
		}
		#region IBatchUpdateable Members
		public void BeginUpdate() {
			Properties.BeginUpdate();
			this.spnSpacingAfter.BeginUpdate();
			this.spnAtSpacing.BeginUpdate();
			this.spnSpacingBefore.BeginUpdate();
			this.batchUpdateHelper.BeginUpdate();
		}
		public void CancelUpdate() {
			this.batchUpdateHelper.CancelUpdate();
			this.spnSpacingBefore.CancelUpdate();
			this.spnAtSpacing.CancelUpdate();
			this.spnSpacingAfter.CancelUpdate();
			Properties.CancelUpdate();
		}
		public void EndUpdate() {
			this.batchUpdateHelper.EndUpdate();
			this.spnSpacingBefore.EndUpdate();
			this.spnAtSpacing.EndUpdate();
			this.spnSpacingAfter.EndUpdate();
			Properties.EndUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		public bool IsUpdateLocked {
			get { return batchUpdateHelper.IsUpdateLocked; }
		}
		#endregion
		#region IBatchUpdateHandler Members
		public void OnBeginUpdate() {
		}
		public void OnCancelUpdate() {
		}
		public void OnEndUpdate() {
		}
		public void OnFirstBeginUpdate() {
			UnsubscribeEvents();
			deferredRaiseChanged = false;
		}
		public void OnLastCancelUpdate() {
			SubscribeEvents();
		}
		public void OnLastEndUpdate() {
			if (deferredRaiseChanged)
				RaiseParagraphSpacingControlChanged();
			SubscribeEvents();
		}
		#endregion
		private void OnSpnAtFloatSpacingSpin(object sender, DevExpress.XtraEditors.Controls.SpinEventArgs e) {
			SpinEdit spinEdit = sender as SpinEdit;
			if (spinEdit == null)
				return;
			e.Handled = true;
			decimal newValue = spinEdit.Properties.MinValue;
			if (e.IsSpinUp)
				newValue = OnSpnAtFloatSpacingSpinUp(spinEdit);
			else
				newValue = OnSpnAtFloatSpacingSpinDown(spinEdit);
			newValue = ApplyLimitationOnSpacingMultiplier(newValue, spinEdit.Properties);
			spinEdit.Value = newValue;
		}
		decimal ApplyLimitationOnSpacingMultiplier(decimal value, RepositoryItemSpinEdit properties) {
			decimal actualMinValue = Math.Max(properties.Increment, properties.MinValue);
			decimal actualValue = value;
			if (value < actualMinValue)
				actualValue = actualMinValue;
			else if (value > properties.MaxValue)
				actualValue = properties.MaxValue;
			return actualValue;
		}
		decimal OnSpnAtFloatSpacingSpinUp(SpinEdit spinEdit) {
			decimal oldValue = spinEdit.Value;
			decimal testValue = Math.Truncate(oldValue + 0.5M);
			decimal truncatedOldValue = Math.Truncate(oldValue);
			if (testValue == truncatedOldValue)
				return truncatedOldValue + 0.5M;
			return testValue;
		}
		decimal OnSpnAtFloatSpacingSpinDown(SpinEdit spinEdit) {
			decimal oldValue = spinEdit.Value;
			decimal truncatedOldValue = Math.Ceiling(oldValue);
			decimal testValue = Math.Ceiling(oldValue - 0.5M);
			if (testValue == truncatedOldValue)
				return truncatedOldValue - 0.5M;
			return testValue;
		}
	}
}
