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
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Forms.Design {
	#region TableRowHeightControl
	[DXToolboxItem(false)]
	public partial class TableRowHeightControl : UserControl, IBatchUpdateable, IBatchUpdateHandler {
		#region Fields
		BatchUpdateHelper batchUpdateHelper;
		bool deferredRaiseChanged;
		TableRowHeightProperties properties;
		#endregion
		public TableRowHeightControl() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.properties = new TableRowHeightProperties();
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
		public TableRowHeightProperties Properties { get { return properties; } }
		#endregion
		#region Events
		#region TableRowHeightControlChanged
		static readonly object tableRowHeightControlChanged = new object();
		public event EventHandler TableRowHeightControlChanged {
			add { Events.AddHandler(tableRowHeightControlChanged, value); }
			remove { Events.RemoveHandler(tableRowHeightControlChanged, value); }
		}
		protected internal virtual void RaiseTableRowHeightControlChanged() {
			EventHandler handler = (EventHandler)this.Events[tableRowHeightControlChanged];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal virtual void SubscribeEvents() {
			Properties.PropertiesChanged += OnPropertiesChanged;
			this.chkSpecifyHeight.CheckStateChanged += OnChkSpecifyHeightCheckStateChanged;
			this.spnHeight.ValueChanged += OnSpnHeightValueChanged;
			this.edtRowHeightType.SelectedIndexChanged += OnEdtHeightTypeSelectedIndexChanged;
		}
		protected internal virtual void UnsubscribeEvents() {
			Properties.PropertiesChanged -= OnPropertiesChanged;
			this.chkSpecifyHeight.CheckStateChanged -= OnChkSpecifyHeightCheckStateChanged;
			this.spnHeight.ValueChanged -= OnSpnHeightValueChanged;
			this.edtRowHeightType.SelectedIndexChanged -= OnEdtHeightTypeSelectedIndexChanged;
		}
		void UpdateControl() {
			UnsubscribeEvents();
			try {
				this.spnHeight.ValueUnitConverter = Properties.ValueUnitConverter;
				this.spnHeight.Properties.DefaultUnitType = Properties.UnitType;
				this.spnHeight.Properties.MinValue = Properties.MinValue;
				this.spnHeight.Properties.MaxValue = Properties.MaxValue;
				bool? useDefaultHeight = Properties.UseDefaultValue;
				bool enabledControls = useDefaultHeight.HasValue && !useDefaultHeight.Value;
				UpdateEnabledControls(enabledControls);
				UpdateSpecifyHeightCheckEdit();
				this.spnHeight.Value = Properties.Height;
				HeightUnitType? heightType = Properties.HeightType;
				this.edtRowHeightType.Value = (heightType.HasValue && heightType.Value == HeightUnitType.Exact) ? heightType : HeightUnitType.Minimum;
			}
			finally {
				SubscribeEvents();
			}
		}
		protected internal virtual void UpdateSpecifyHeightCheckEdit() {
			bool? check = !Properties.UseDefaultValue;
			if (check == null) {
				chkSpecifyHeight.Properties.AllowGrayed = true;
				chkSpecifyHeight.CheckState = CheckState.Indeterminate;
				return;
			}
			chkSpecifyHeight.Properties.AllowGrayed = false;
			if (check.Value)
				chkSpecifyHeight.CheckState = CheckState.Checked;
			else
				chkSpecifyHeight.CheckState = CheckState.Unchecked;
		}
		void OnPropertiesChanged(object sender, EventArgs e) {
			UpdateControl();
			OnTableRowHeightControlChanged();
		}
		void OnTableRowHeightControlChanged() {
			if (IsUpdateLocked)
				deferredRaiseChanged = true;
			else
				RaiseTableRowHeightControlChanged();
		}
		void OnChkSpecifyHeightCheckStateChanged(object sender, EventArgs e) {
			bool? check = CheckStateToNullableBool(chkSpecifyHeight.CheckState);
			Properties.UseDefaultValue = !check;
			bool enabledControls = check.HasValue && check.Value;
			UpdateEnabledControls(enabledControls);
		}
		void UpdateEnabledControls(bool enabled) {
			this.spnHeight.Enabled = enabled;
			this.lblHeightIn.Enabled = enabled;
			this.edtRowHeightType.Enabled = enabled;
		}
		bool? CheckStateToNullableBool(CheckState checkState) {
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
		void OnSpnHeightValueChanged(object sender, EventArgs e) {
			Properties.Height = spnHeight.Value;
		}
		void OnEdtHeightTypeSelectedIndexChanged(object sender, EventArgs e) {
			Properties.HeightType = edtRowHeightType.Value;
		}
		#region IBatchUpdateable Members
		public void BeginUpdate() {
			Properties.BeginUpdate();
			this.spnHeight.BeginUpdate();
			this.batchUpdateHelper.BeginUpdate();
		}
		public void CancelUpdate() {
			this.batchUpdateHelper.CancelUpdate();
			this.spnHeight.CancelUpdate();
			Properties.CancelUpdate();
		}
		public void EndUpdate() {
			this.batchUpdateHelper.EndUpdate();
			this.spnHeight.EndUpdate();
			Properties.EndUpdate();
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
				RaiseTableRowHeightControlChanged();
			SubscribeEvents();
		}
		#endregion
	}
	#endregion
}
