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
using DevExpress.Office;
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
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraRichEdit.Design {
	#region TableSizeControl
	[DXToolboxItem(false)]
	public partial class TableSizeControl : UserControl, IBatchUpdateable, IBatchUpdateHandler {
		#region Fields
		BatchUpdateHelper batchUpdateHelper;
		bool deferredRaiseChanged;
		TableSizeProperties properties;
		#endregion
		public TableSizeControl() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.properties = new TableSizeProperties();
			InitializeComponent();
			SubscribeEvents();
			UpdateControl();
		}
		#region Properties
		public TableSizeProperties Properties { get { return properties; } }
		protected override CreateParams CreateParams {
			get {
				return DevExpress.XtraRichEdit.Native.RightToLeftHelper.PatchCreateParams(base.CreateParams, this);
			}
		}
		#endregion
		#region Events
		#region TableSizeControlChanged
		static readonly object tableSizeControlChanged = new object();
		public event EventHandler TableSizeControlChanged {
			add { Events.AddHandler(tableSizeControlChanged, value); }
			remove { Events.RemoveHandler(tableSizeControlChanged, value); }
		}
		protected internal virtual void RaiseTableSizeControlChanged() {
			EventHandler handler = (EventHandler)this.Events[tableSizeControlChanged];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal virtual void SubscribeEvents() {
			Properties.PropertiesChanged += OnPropertiesChanged;
			this.chkPreferredWidth.CheckStateChanged += OnChkPreferredWidthCheckStateChanged;
			this.spnPreferredWidth.ValueChanged += OnSpnPreferredWidthValueChanged;
			this.cbWidthType.SelectedValueChanged += OnCbWidthTypeSelectedValueChanged;
		}
		protected internal virtual void UnsubscribeEvents() {
			Properties.PropertiesChanged -= OnPropertiesChanged;
			this.chkPreferredWidth.CheckStateChanged -= OnChkPreferredWidthCheckStateChanged;
			this.spnPreferredWidth.ValueChanged -= OnSpnPreferredWidthValueChanged;
			this.cbWidthType.SelectedValueChanged -= OnCbWidthTypeSelectedValueChanged;
		}
		void UpdateControl() {
			UnsubscribeEvents();
			try {
				this.spnPreferredWidth.ValueUnitConverter = Properties.ValueUnitConverter;
				this.spnPreferredWidth.Properties.MinValue = Properties.MinValue;
				this.spnPreferredWidth.Properties.MaxValue = Properties.MaxValue;
				DocumentUnit defaultUnitType = Properties.UnitType;
				this.spnPreferredWidth.Properties.DefaultUnitType = defaultUnitType;
				this.cbWidthType.Properties.UnitType = defaultUnitType;
				bool? useDefaultWidth = Properties.UseDefaultValue;
				bool enabledControls = useDefaultWidth.HasValue && !useDefaultWidth.Value;
				UpdateEnabledControls(enabledControls);
				UpdatePreferredWidthCheckEdit();
				WidthUnitType? widthType = Properties.WidthType;
				WidthUnitType actualWidhtUnitType = (widthType.HasValue && widthType.Value == WidthUnitType.FiftiethsOfPercent) ? widthType.Value : WidthUnitType.ModelUnits;
				this.spnPreferredWidth.Properties.IsValueInPercent = actualWidhtUnitType == WidthUnitType.FiftiethsOfPercent;
				this.spnPreferredWidth.Value = Properties.Width;
				this.cbWidthType.Value = actualWidhtUnitType;
			}
			finally {
				SubscribeEvents();
			}
		}
		protected internal virtual void UpdatePreferredWidthCheckEdit() {
			bool? check = !Properties.UseDefaultValue;
			if (check == null) {
				chkPreferredWidth.Properties.AllowGrayed = true;
				chkPreferredWidth.CheckState = CheckState.Indeterminate;
				return;
			}
			chkPreferredWidth.Properties.AllowGrayed = false;
			if (check.Value)
				chkPreferredWidth.CheckState = CheckState.Checked;
			else
				chkPreferredWidth.CheckState = CheckState.Unchecked;
		}
		void OnPropertiesChanged(object sender, EventArgs e) {
			UpdateControl();
			OnTableSizeControlChanged();
		}
		void OnTableSizeControlChanged() {
			if (IsUpdateLocked)
				deferredRaiseChanged = true;
			else
				RaiseTableSizeControlChanged();
		}
		void OnChkPreferredWidthCheckStateChanged(object sender, EventArgs e) {
			bool? check = CheckStateToNullableBool(chkPreferredWidth.CheckState);
			Properties.UseDefaultValue = !check;
			bool enabledControls = check.HasValue && check.Value;
			UpdateEnabledControls(enabledControls);
		}
		void UpdateEnabledControls(bool enabled) {
			this.spnPreferredWidth.Enabled = enabled;
			this.lblMeasureIn.Enabled = enabled;
			this.cbWidthType.Enabled = enabled;
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
		void OnSpnPreferredWidthValueChanged(object sender, EventArgs e) {
			Properties.Width = spnPreferredWidth.Value;
		}
		void OnCbWidthTypeSelectedValueChanged(object sender, EventArgs e) {
			BeginUpdate();
			WidthUnitType widthUnitType = (WidthUnitType)((ComboBoxItem)(cbWidthType.SelectedItem)).Value;
			Properties.WidthType = widthUnitType;
			Properties.Width = CalculateActualWidth();
			EndUpdate();
		}
		int? CalculateActualWidth() {
			WidthUnitType? widthType = Properties.WidthType;
			if (!widthType.HasValue)
				return null;
			if (widthType.Value == WidthUnitType.FiftiethsOfPercent)
				return CalculateValueInPercent();
			else
				return CalculateValueInModelUnits();
		}
		protected internal virtual int? CalculateValueInPercent() {
			int? width = Properties.Width;
			int valueForPercent = Properties.ValueForPercent;
			if (!width.HasValue || valueForPercent == 0)
				return 0;
			return width.Value * 100 / valueForPercent;
		}
		protected internal virtual int? CalculateValueInModelUnits() {
			int? width = Properties.Width;
			if (!width.HasValue)
				return 0;
			return Properties.ValueForPercent * width.Value / 100;
		}
		#region IBatchUpdateable Members
		public void BeginUpdate() {
			Properties.BeginUpdate();
			this.spnPreferredWidth.BeginUpdate();
			this.batchUpdateHelper.BeginUpdate();
		}
		public void CancelUpdate() {
			this.batchUpdateHelper.CancelUpdate();
			this.spnPreferredWidth.CancelUpdate();
			Properties.CancelUpdate();
		}
		public void EndUpdate() {
			this.batchUpdateHelper.EndUpdate();
			this.spnPreferredWidth.EndUpdate();
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
				RaiseTableSizeControlChanged();
			SubscribeEvents();
		}
		#endregion
	}
	#endregion
}
