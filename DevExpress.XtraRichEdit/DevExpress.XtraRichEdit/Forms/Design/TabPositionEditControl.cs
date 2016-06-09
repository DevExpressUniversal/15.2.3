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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.Office;
using DevExpress.Office.Design.Internal;
namespace DevExpress.XtraRichEdit.Design {
#if DEBUGTEST
	[DXToolboxItem(true)]
#else
	[DXToolboxItem(false)]
#endif // DEBUGTEST
	public partial class TabStopPositionEdit : UserControl, IBatchUpdateable, IBatchUpdateHandler{
		readonly BatchUpdateHelper batchUpdateHelper;
		readonly UIUnitConverter unitConverter;
		readonly TabPositionEditControlProperties properties;
		bool deferredPropertiesChanged;
		bool deferredEditValueChanged;
		bool deferredSelectedIndexChanged;
		DocumentModelUnitConverter valueUnitConverter;
		public TabStopPositionEdit() {
			this.unitConverter = new UIUnitConverter(UnitPrecisionDictionary.DefaultPrecisions);
			this.properties = new TabPositionEditControlProperties();
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			InitializeComponent();
			this.valueUnitConverter = new DocumentModelUnitTwipsConverter();
			SubscribeEvents();
			UpdateControl();
		}
		protected override CreateParams CreateParams {
			get {
				return DevExpress.XtraRichEdit.Native.RightToLeftHelper.PatchCreateParams(base.CreateParams, this);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DocumentModelUnitConverter ValueUnitConverter { get { return valueUnitConverter; } set { valueUnitConverter = value; } }
		public TabPositionEditControlProperties Properties { get { return properties; } }
		public string EditValue {
			get { return this.edtCurrentPosition.EditValue as String; }
			set { this.edtCurrentPosition.EditValue = value; }
		}
		public int TabStopPositionIndex {
			get {
				if (!TabStopPosition.HasValue)
					return -1;
				UIUnit unit = unitConverter.ToUIUnit(TabStopPosition.Value, Properties.UnitType);
				return this.lbPositions.Items.IndexOf(unit.ToString());
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int? TabStopPosition {
			get {
				int? coreValue = TabStopPositionCore;
				if(coreValue == null)
					return null;
				return ValueUnitConverter.TwipsToModelUnits(coreValue.Value);
			}
			set {
				if(!value.HasValue) {
					this.edtCurrentPosition.EditValue = String.Empty;
					return;
				}
				TabStopPositionCore = ValueUnitConverter.ModelUnitsToTwips(value.Value);
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int? TabStopPositionCore {
			get {
				string editValueString = this.edtCurrentPosition.EditValue as String;
				if (String.IsNullOrEmpty(editValueString))
					return null;
				UIUnit result = null;
				bool isParsed = UIUnit.TryParse(editValueString, Properties.UnitType, out result);
				if (isParsed)
					return unitConverter.ToTwipsUnit(result);
				return null;
			}
			set {
				UIUnit unit = unitConverter.ToUIUnit(value.Value, Properties.UnitType);
				string editValueString = this.edtCurrentPosition.EditValue as string;
				string newEditValueString = unit.ToString();
				if (TabStopPosition == value && TabStopPosition.HasValue && editValueString == newEditValueString)
					return;
				this.edtCurrentPosition.EditValue = unit.ToString();
				UpdateControl();
			}
		}
		#region Events
		static readonly object onPropertiesChanged = new Object();
		public event EventHandler PropertiesChanged {
			add { Events.AddHandler(onPropertiesChanged, value); }
			remove { Events.RemoveHandler(onPropertiesChanged, value); }
		}
		void RaiseControlPropertiesChanged() {
			EventHandler handler = (EventHandler)Events[onPropertiesChanged];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		static readonly object onEditValueChanged = new Object();
		public event EventHandler EditValueChanged {
			add { Events.AddHandler(onEditValueChanged, value); }
			remove { Events.RemoveHandler(onEditValueChanged, value); }
		}
		void RaiseEditValueChanged() {
			EventHandler handler = (EventHandler)Events[onEditValueChanged];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		static readonly object onSelectedIndexChanged = new object();
		public event EventHandler SelectedIndexChanged {
			add { Events.AddHandler(onSelectedIndexChanged, value); }
			remove { Events.RemoveHandler(onSelectedIndexChanged, value); }
		}
		void RaiseSelectedIndexChanged() {
			EventHandler handler = (EventHandler)Events[onSelectedIndexChanged];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		void OnControlPropertiesChanged() {
			if (IsUpdateLocked)
				this.deferredPropertiesChanged = true;
			else
				RaiseControlPropertiesChanged();
		}
		void OnEditValueChanged() {
			if (IsUpdateLocked)
				this.deferredEditValueChanged = true;
			else
				RaiseEditValueChanged();
		}
		void OnSelectedIndexChanged() {
			if (IsUpdateLocked)
				this.deferredSelectedIndexChanged = true;
			else
				RaiseSelectedIndexChanged();
		}
		protected internal virtual void SubscribeEvents() {
			Properties.PropertiesChanged += OnPropertiesChanged;
			this.edtCurrentPosition.KeyDown += OnEdtCurrentPositionKeyDown;
			this.lbPositions.SelectedIndexChanged += OnLbPositionsSelectedIndexChanged;
			this.edtCurrentPosition.EditValueChanged += OnEdtCurrentPositionEditValueChanged;
		}
		protected internal virtual void UnsubscribeEvents() {
			Properties.PropertiesChanged -= OnPropertiesChanged;
			this.edtCurrentPosition.KeyDown -= OnEdtCurrentPositionKeyDown;
			this.lbPositions.SelectedIndexChanged -= OnLbPositionsSelectedIndexChanged;
			this.edtCurrentPosition.EditValueChanged -= OnEdtCurrentPositionEditValueChanged;
		}
		protected internal virtual void OnPropertiesChanged(object sender, EventArgs e) {
			UpdateControl();
			OnControlPropertiesChanged();
		}
		protected internal virtual void OnEdtCurrentPositionEditValueChanged(object sender, EventArgs e) {
			this.edtCurrentPosition.ErrorText = String.Empty;
			OnEditValueChanged();
		}
		protected internal virtual void OnLbPositionsSelectedIndexChanged(object sender, EventArgs e) {
			int currentPositionIndex = this.lbPositions.SelectedIndex;
			if (currentPositionIndex == -1)
				return;
			TabStopPosition = Properties.TabFormattingInfo[currentPositionIndex].Position;
			OnSelectedIndexChanged();
		}
		protected internal virtual void OnEdtCurrentPositionKeyDown(object sender, KeyEventArgs e) {
			e.Handled = true;
			if (e.KeyCode == Keys.Up) {
				if (this.lbPositions.SelectedIndex > 0)
					this.lbPositions.SelectedIndex--;
			} else if (e.KeyCode == Keys.Down) {
				this.lbPositions.SelectedIndex++;
			} else
				e.Handled = false;   
		}
		protected internal virtual void UpdateControl() {
			BeginUpdate();
			try {
				UpdateControlCore();
			}
			finally {
				CancelUpdate();
			}
		}
		void UpdateControlCore() {
			this.lbPositions.Items.Clear();
			if (Properties.TabFormattingInfo == null) {
				return;
			}
			int count = Properties.TabFormattingInfo.Count;
			if (count < 1) {
				this.edtCurrentPosition.EditValue = String.Empty;
				return;
			}
			for (int i = 0; i < count; i++) {
				TabInfo tab = Properties.TabFormattingInfo[i];
				int positionInTwips = ValueUnitConverter.ModelUnitsToTwips(tab.Position);
				this.lbPositions.Items.Add(GetStringRepresentationsOfUnit(positionInTwips));
			}
			string tabStopPositionString = GetStringRepresentationsOfUnit(TabStopPositionCore);
			string correctedTabStopPositionString = (String.IsNullOrEmpty(tabStopPositionString)) ? this.lbPositions.Items[0] as String : tabStopPositionString;
			this.edtCurrentPosition.EditValue = correctedTabStopPositionString;
			this.lbPositions.SelectedIndex = this.lbPositions.Items.IndexOf(correctedTabStopPositionString);
		}
		string GetStringRepresentationsOfUnit(int? unitInTwips) {
			if (unitInTwips == null)
				return String.Empty;
			UIUnit unit = unitConverter.ToUIUnit(unitInTwips.Value, Properties.UnitType);
			return unit.ToString();
		}
		public bool DoValidate() {
			UIUnit result = null;
			if (!UIUnit.TryParse(this.edtCurrentPosition.Text, Properties.UnitType, out result)) {
				this.edtCurrentPosition.ErrorText = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_InvalidTabStop);
				this.edtCurrentPosition.Focus();
				return false;
			}
			float valueInTwips = result.Value;
			if(valueInTwips < ParagraphFormDefaults.MinTabStopPositionByDefault || ParagraphFormDefaults.MaxTabStopPositionByDefault < valueInTwips) {
				string stringFormat = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_InvalidValueRange);
				string minValue = unitConverter.ToUIUnit(ParagraphFormDefaults.MinTabStopPositionByDefault, Properties.UnitType).ToString();
				string maxValue = unitConverter.ToUIUnit(ParagraphFormDefaults.MaxTabStopPositionByDefault, Properties.UnitType).ToString();
				this.edtCurrentPosition.ErrorText = String.Format(stringFormat, minValue, maxValue);
				this.edtCurrentPosition.Focus();
				return false;
			}
			return true;
		}
		#region IBatchUpdateable Members
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		public bool IsUpdateLocked { get { return BatchUpdateHelper.IsUpdateLocked; } }
		BatchUpdateHelper BatchUpdateHelper { get { return batchUpdateHelper; } }
		public void BeginUpdate() {
			Properties.BeginUpdate();
			BatchUpdateHelper.BeginUpdate();
		}
		public void CancelUpdate() {
			BatchUpdateHelper.EndUpdate();
			Properties.CancelUpdate();
		}
		public void EndUpdate() {
			BatchUpdateHelper.EndUpdate();
			Properties.EndUpdate();
		}
		#endregion
		#region IBatchUpdateHandler Members
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			this.deferredPropertiesChanged = false;
			this.deferredEditValueChanged = false;
			this.deferredSelectedIndexChanged = false;
			UnsubscribeEvents();
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			SubscribeEvents();
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			SubscribeEvents();
			if (this.deferredPropertiesChanged)
				RaiseControlPropertiesChanged();
			if (this.deferredEditValueChanged)
				RaiseEditValueChanged();
			if (this.deferredSelectedIndexChanged)
				RaiseSelectedIndexChanged();
		}
		#endregion
	}
}
