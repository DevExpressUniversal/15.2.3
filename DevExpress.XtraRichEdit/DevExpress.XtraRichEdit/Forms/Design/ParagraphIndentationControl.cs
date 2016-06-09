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
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.ParagraphIndentationControl.lblLeft")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.ParagraphIndentationControl.lblRight")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.ParagraphIndentationControl.lblSpecial")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.ParagraphIndentationControl.lblBy")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.ParagraphIndentationControl.spnLeftIndent")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.ParagraphIndentationControl.spnRightIndent")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.ParagraphIndentationControl.spnSpecial")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Design.ParagraphIndentationControl.edtFirstLineIndent")]
#endregion
namespace DevExpress.XtraRichEdit.Design {
	[DXToolboxItem(false)]
	public partial class ParagraphIndentationControl : UserControl, IBatchUpdateable, IBatchUpdateHandler {
		readonly BatchUpdateHelper batchUpdateHelper;
		readonly ParagraphIndentationProperties properties;
		bool deferredRaiseChanged;
		public ParagraphIndentationControl() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.properties = new ParagraphIndentationProperties();
			InitializeComponent();
			SubscribeEvents();
			UpdateControl();
		}
		public ParagraphIndentationProperties Properties { get { return properties; } }
		protected override CreateParams CreateParams {
			get {
				return DevExpress.XtraRichEdit.Native.RightToLeftHelper.PatchCreateParams(base.CreateParams, this);
			}
		}
		protected override void OnParentRightToLeftChanged(EventArgs e) {
			base.OnParentRightToLeftChanged(e);
		}
		#region Events
		#region ParagraphIndentationControlChanged
		static readonly object paragraphIndentControlChanged = new object();
		public event EventHandler ParagraphIndentControlChanged {
			add { Events.AddHandler(paragraphIndentControlChanged, value); }
			remove { Events.RemoveHandler(paragraphIndentControlChanged, value); }
		}
		protected internal virtual void RaiseParagraphIndentControlChanged() {
			EventHandler handler = (EventHandler)this.Events[paragraphIndentControlChanged];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal virtual void SubscribeEvents() {
			Properties.PropertiesChanged += new EventHandler(OnPropertiesChanged);
			this.edtFirstLineIndent.SelectedIndexChanged += new EventHandler(OnEdtFirstLineIndentSelectedIndexChanged);
			this.spnLeftIndent.ValueChanged += new EventHandler(OnSpnLeftIndentValueChanged);
			this.spnRightIndent.ValueChanged += new EventHandler(OnSpnRightIndentValueChanged);
			this.spnSpecial.ValueChanged += new EventHandler(OnSpnSpecialValueChanged);
			this.spnSpecial.EditValueChanged += new EventHandler(OnSpnSpecialEditValueChanged);
		}
		protected internal virtual void UnsubscribeEvents() {
			Properties.PropertiesChanged -= new EventHandler(OnPropertiesChanged);
			this.edtFirstLineIndent.SelectedIndexChanged -= new EventHandler(OnEdtFirstLineIndentSelectedIndexChanged);
			this.spnLeftIndent.ValueChanged -= new EventHandler(OnSpnLeftIndentValueChanged);
			this.spnRightIndent.ValueChanged -= new EventHandler(OnSpnRightIndentValueChanged);
			this.spnSpecial.ValueChanged -= new EventHandler(OnSpnSpecialValueChanged);
			this.spnSpecial.EditValueChanged -= new EventHandler(OnSpnSpecialEditValueChanged);
		}
		protected internal virtual void OnEdtFirstLineIndentSelectedIndexChanged(object sender, EventArgs e) {
			ParagraphFirstLineIndent? oldValue = Properties.FirstLineIndentType;
			ParagraphFirstLineIndent? value = edtFirstLineIndent.Value;
			BeginUpdate();
			Properties.FirstLineIndentType = value;
			if (value == ParagraphFirstLineIndent.None) {
				Properties.FirstLineIndent = null;
				spnSpecial.Properties.AllowNullInput = DefaultBoolean.True;
			}
			else {
				if (oldValue == ParagraphFirstLineIndent.None || !oldValue.HasValue)
					Properties.FirstLineIndent = Properties.ValueUnitConverter.TwipsToModelUnits(ParagraphFormDefaults.DefaultFirstLineIndent);
				spnSpecial.Properties.AllowNullInput = DefaultBoolean.False;
			}
			EndUpdate();
		}
		protected internal virtual void OnSpnLeftIndentValueChanged(object sender, EventArgs e) {
			Properties.LeftIndent = this.spnLeftIndent.Value;
		}
		protected internal virtual void OnSpnRightIndentValueChanged(object sender, EventArgs e) {
			Properties.RightIndent = this.spnRightIndent.Value;
		}
		protected internal virtual void OnSpnSpecialValueChanged(object sender, EventArgs e) {
			Properties.FirstLineIndent = this.spnSpecial.Value;
		}
		protected internal virtual void OnSpnSpecialEditValueChanged(object sender, EventArgs e) {
			DevExpress.XtraRichEdit.Model.ParagraphFirstLineIndent? value = this.edtFirstLineIndent.Value;
			if (value == ParagraphFirstLineIndent.None || !value.HasValue) {
				BeginUpdate();
				Properties.FirstLineIndentType = ParagraphFirstLineIndent.Indented;
				Properties.FirstLineIndent = this.spnSpecial.Value;
				EndUpdate();
			}
		}
		protected internal virtual void OnPropertiesChanged(object sender, EventArgs e) {
			UpdateControl();
			OnParagraphIndentControlChanged();
		}
		private void UpdateControl() {
			UnsubscribeEvents();
			try {
				this.spnLeftIndent.ValueUnitConverter = Properties.ValueUnitConverter;
				this.spnRightIndent.ValueUnitConverter = Properties.ValueUnitConverter;
				this.spnSpecial.ValueUnitConverter = Properties.ValueUnitConverter;
				this.spnLeftIndent.Properties.MinValue = ParagraphFormDefaults.MinIndentByDefault;
				this.spnLeftIndent.Properties.MaxValue = ParagraphFormDefaults.MaxIndentByDefault;
				this.spnRightIndent.Properties.MinValue = ParagraphFormDefaults.MinIndentByDefault;
				this.spnRightIndent.Properties.MaxValue = ParagraphFormDefaults.MaxIndentByDefault;
				this.spnSpecial.Properties.MinValue = 0;
				this.spnSpecial.Properties.MaxValue = ParagraphFormDefaults.MaxIndentByDefault;
				this.spnLeftIndent.Properties.DefaultUnitType = Properties.UnitType;
				this.spnRightIndent.Properties.DefaultUnitType = Properties.UnitType;
				this.spnSpecial.Properties.DefaultUnitType = Properties.UnitType;
				this.spnRightIndent.Value = Properties.RightIndent;
				this.spnLeftIndent.Value = Properties.LeftIndent;
				this.edtFirstLineIndent.Value = Properties.FirstLineIndentType;
				this.spnSpecial.Properties.AllowNullInput = (!this.edtFirstLineIndent.Value.HasValue || this.edtFirstLineIndent.Value.Value == ParagraphFirstLineIndent.None) ? DefaultBoolean.True : DefaultBoolean.False;
				this.spnSpecial.Value = (Properties.FirstLineIndentType == ParagraphFirstLineIndent.None || !Properties.FirstLineIndentType.HasValue) ? null : Properties.FirstLineIndent;
			}
			finally {
				SubscribeEvents();
			}
		}
		protected internal virtual void OnParagraphIndentControlChanged() {
			if (IsUpdateLocked)
				deferredRaiseChanged = true;
			else
				RaiseParagraphIndentControlChanged();
		}
		#region IBatchUpdateable Members
		public void BeginUpdate() {
			this.spnLeftIndent.BeginUpdate();
			this.spnRightIndent.BeginUpdate();
			this.spnSpecial.BeginUpdate();
			Properties.BeginUpdate();
			this.batchUpdateHelper.BeginUpdate();
		}
		public void CancelUpdate() {
			this.batchUpdateHelper.CancelUpdate();
			Properties.CancelUpdate();
			this.spnLeftIndent.CancelUpdate();
			this.spnRightIndent.CancelUpdate();
			this.spnSpecial.CancelUpdate();
		}
		public void EndUpdate() {
			this.batchUpdateHelper.EndUpdate();
			Properties.EndUpdate();
			spnLeftIndent.EndUpdate();
			spnRightIndent.EndUpdate();
			spnSpecial.EndUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler Members
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			UnsubscribeEvents();
			deferredRaiseChanged = false;
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			SubscribeEvents();
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if (deferredRaiseChanged)
				RaiseParagraphIndentControlChanged();
			SubscribeEvents();
		}
		#endregion
	}
}
