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
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Xpf.Editors;
using DevExpress.Office;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.RichEdit.UI {
	public partial class ParagraphSpacingControl : UserControl, IBatchUpdateable, IBatchUpdateHandler, IDisposable {
		const float MinMultiplierValue = 0.06f; 
		const decimal MaxMultiplierValue = 132M; 
		ParagraphSpacingProperties properties;
		BatchUpdateHelper batchUpdateHelper;
		bool deferredRaiseChanged;
		internal float? shadowSpnAtSpacingValue;
		internal float? shadowSpnAtFloatSpacingValue;
		public ParagraphSpacingControl() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.properties = new ParagraphSpacingProperties();
			InitializeComponent();
			InitializeControls();
			SubscribeEvents();
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~ParagraphSpacingControl() {
			Dispose(false);
		}
		#endregion
		public ParagraphSpacingProperties Properties { get { return properties; } }
		#region Events
		#region ParagraphSpacingControlChanged
		public event EventHandler ParagraphSpacingControlChanged;
		protected internal virtual void RaiseParagraphSpacingControlChanged() {
			if (ParagraphSpacingControlChanged != null)
				ParagraphSpacingControlChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal void InitializeControls() {
			this.spnAtSpacing.Value = null;
			this.spnAtFloatSpacing.EditValue = null;
			this.shadowSpnAtSpacingValue = null;
			this.shadowSpnAtFloatSpacingValue = null;
			this.spnAtFloatSpacing.MinValue = 0;
			this.spnAtFloatSpacing.MaxValue = MaxMultiplierValue;
			UpdateControl();
		}
		protected internal virtual void SubscribeEvents() {
			Properties.PropertiesChanged += OnPropertiesChanged;
			this.edtLineSpacing.EditValueChanged += OnEdtLineSpacingEditValueChanged;
			this.spnSpacingAfter.ValueChanged += OnSpnSpacingAfterValueChanged;
			this.spnSpacingBefore.ValueChanged += OnSpnSpacingBeforeValueChanged;
			this.spnAtSpacing.ValueChanged += OnSpnAtSpacingValueChanged;
			this.spnAtFloatSpacing.EditValueChanged += OnSpnAtFloatSpacingValueChanged;
		}
		protected internal virtual void UnsubscribeEvents() {
			Properties.PropertiesChanged -= OnPropertiesChanged;
			this.edtLineSpacing.EditValueChanged -= OnEdtLineSpacingEditValueChanged;
			this.spnSpacingAfter.ValueChanged -= OnSpnSpacingAfterValueChanged;
			this.spnSpacingBefore.ValueChanged -= OnSpnSpacingBeforeValueChanged;
			this.spnAtSpacing.ValueChanged -= OnSpnAtSpacingValueChanged;
			this.spnAtFloatSpacing.EditValueChanged -= OnSpnAtFloatSpacingValueChanged;
		}
		protected internal virtual void OnEdtLineSpacingEditValueChanged(object sender, EditValueChangedEventArgs e) {
			OnEdtLineSpacingSelectedIndexChanged(sender, e);
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
						this.shadowSpnAtFloatSpacingValue = 3.0f;
					Properties.LineSpacing = this.shadowSpnAtFloatSpacingValue;
				}
				if (Properties.LineSpacingType == ParagraphLineSpacing.Exactly || Properties.LineSpacingType == ParagraphLineSpacing.AtLeast) {
					if (!this.shadowSpnAtSpacingValue.HasValue)
						this.shadowSpnAtSpacingValue = 240;
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
		protected internal virtual void OnSpnAtFloatSpacingValueChanged(object sender, EditValueChangedEventArgs e) {
			Properties.BeginUpdate();
			try {
				Properties.LineSpacingType = ParagraphLineSpacing.Multiple;
				object value = this.spnAtFloatSpacing.EditValue;
				if (value == null)
					Properties.LineSpacing = null;
				else
					Properties.LineSpacing = Math.Max(MinMultiplierValue, Convert.ToSingle(value));
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
			DocumentUnit unitType = Properties.UnitType;
			this.spnSpacingAfter.MinValue = 0;
			this.spnSpacingAfter.MaxValue = Properties.MaxSpacing;
			this.spnSpacingAfter.DefaultUnitType = unitType;
			this.spnSpacingBefore.MinValue = 0;
			this.spnSpacingBefore.MaxValue = Properties.MaxSpacing;
			this.spnSpacingBefore.DefaultUnitType = unitType;
			this.spnAtSpacing.MinValue = Properties.MinLineSpacing;
			this.spnAtSpacing.MaxValue = Properties.MaxLineSpacing;
			this.spnAtSpacing.DefaultUnitType = unitType;
			this.spnSpacingAfter.Value = Properties.SpacingAfter;
			this.spnSpacingBefore.Value = Properties.SpacingBefore;
			UpdateLineSpacing();
		}
		void UpdateLineSpacing() {
			this.edtLineSpacing.EditValue = Properties.LineSpacingType;
			bool isSpnAtFloatSpacingVisible = !Properties.LineSpacingType.HasValue ||
												Properties.LineSpacingType == ParagraphLineSpacing.Single ||
												Properties.LineSpacingType == ParagraphLineSpacing.Double ||
												Properties.LineSpacingType == ParagraphLineSpacing.Multiple ||
												Properties.LineSpacingType == ParagraphLineSpacing.Sesquialteral;
			spnAtFloatSpacing.SetVisible(this.spnAtFloatSpacing.IsEnabled = isSpnAtFloatSpacingVisible);
			spnAtSpacing.SetVisible(this.spnAtSpacing.IsEnabled = !isSpnAtFloatSpacingVisible);
			if (isSpnAtFloatSpacingVisible)
				this.spnAtFloatSpacing.EditValue = (decimal?)((Properties.LineSpacingType != ParagraphLineSpacing.Multiple) ? null : Properties.LineSpacing);
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
			this.batchUpdateHelper.BeginUpdate();
		}
		public void CancelUpdate() {
			this.batchUpdateHelper.CancelUpdate();
			Properties.CancelUpdate();
		}
		public void EndUpdate() {
			this.batchUpdateHelper.EndUpdate();
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
	}
}
