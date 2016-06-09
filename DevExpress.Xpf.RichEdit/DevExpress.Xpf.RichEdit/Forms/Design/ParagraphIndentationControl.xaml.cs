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
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.RichEdit.UI {
	public partial class ParagraphIndentationControl : UserControl, IBatchUpdateable, IBatchUpdateHandler, IDisposable {
		BatchUpdateHelper batchUpdateHelper;
		bool deferredRaiseChanged;
		ParagraphIndentationProperties properties;
		public ParagraphIndentationControl() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.properties = new ParagraphIndentationProperties();
			InitializeComponent();
			SubscribeEvents();
			UpdateControl();
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~ParagraphIndentationControl() {
			Dispose(false);
		}
		#endregion
		public ParagraphIndentationProperties Properties { get { return properties; } }
		#region Events
		#region ParagraphIndentationControlChanged
		public event EventHandler ParagraphIndentControlChanged;
		protected internal virtual void RaiseParagraphIndentControlChanged() {
			if(ParagraphIndentControlChanged != null)
				ParagraphIndentControlChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal virtual void SubscribeEvents() {
			Properties.PropertiesChanged += OnPropertiesChanged;
			this.edtFirstLineIndent.EditValueChanged += OnEdtFirstLineIndentValueChanged;
			this.spnLeftIndent.ValueChanged += OnSpnLeftIndentValueChanged;
			this.spnRightIndent.ValueChanged += OnSpnRightIndentValueChanged;
			this.spnSpecial.ValueChanged += OnSpnSpecialValueChanged;
			this.spnSpecial.EditValueChanged += OnSpnSpecialEditValueChanged;
		}
		protected internal virtual void UnsubscribeEvents() {
			Properties.PropertiesChanged -= OnPropertiesChanged;
			this.edtFirstLineIndent.EditValueChanged -= OnEdtFirstLineIndentValueChanged;
			this.spnLeftIndent.ValueChanged -= OnSpnLeftIndentValueChanged;
			this.spnRightIndent.ValueChanged -= OnSpnRightIndentValueChanged;
			this.spnSpecial.ValueChanged -= OnSpnSpecialValueChanged;
			this.spnSpecial.EditValueChanged -= OnSpnSpecialEditValueChanged;
		}
		protected internal virtual void OnEdtFirstLineIndentValueChanged(object sender, EditValueChangedEventArgs e) {
			OnEdtFirstLineIndentSelectedIndexChanged(sender, e);
		}
		protected internal virtual void OnEdtFirstLineIndentSelectedIndexChanged(object sender, EventArgs e) {
			ParagraphFirstLineIndent? oldValue = Properties.FirstLineIndentType;
			ParagraphFirstLineIndent? value = edtFirstLineIndent.Value;
			BeginUpdate();
			Properties.FirstLineIndentType = value;
			if (value == ParagraphFirstLineIndent.None)
				Properties.FirstLineIndent = null;
			else {
				if (oldValue == ParagraphFirstLineIndent.None || !oldValue.HasValue)
					Properties.FirstLineIndent = ParagraphFormDefaults.DefaultFirstLineIndent;
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
			if(value == ParagraphFirstLineIndent.None || !value.HasValue) {
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
				this.spnLeftIndent.MinValue = ParagraphFormDefaults.MinIndentByDefault;
				this.spnLeftIndent.MaxValue = ParagraphFormDefaults.MaxIndentByDefault;
				this.spnRightIndent.MinValue = ParagraphFormDefaults.MinIndentByDefault;
				this.spnRightIndent.MaxValue = ParagraphFormDefaults.MaxIndentByDefault;
				this.spnSpecial.MinValue = 0;
				this.spnSpecial.MaxValue = ParagraphFormDefaults.MaxIndentByDefault;
				this.spnLeftIndent.DefaultUnitType = Properties.UnitType;
				this.spnRightIndent.DefaultUnitType = Properties.UnitType;
				this.spnSpecial.DefaultUnitType = Properties.UnitType;
				this.spnRightIndent.Value = Properties.RightIndent;
				this.spnLeftIndent.Value = Properties.LeftIndent;
				this.edtFirstLineIndent.EditValue = Properties.FirstLineIndentType;
				this.spnSpecial.Value = (Properties.FirstLineIndentType == ParagraphFirstLineIndent.None || !Properties.FirstLineIndentType.HasValue) ? null : Properties.FirstLineIndent;
			} finally {
				SubscribeEvents();
			}
		}
		protected internal virtual void OnParagraphIndentControlChanged() {
			if(IsUpdateLocked)
				deferredRaiseChanged = true;
			else
				RaiseParagraphIndentControlChanged();
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
			if(deferredRaiseChanged)
				RaiseParagraphIndentControlChanged();
			SubscribeEvents();
		}
		#endregion
	}
}
