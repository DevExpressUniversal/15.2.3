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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.Xpf.Editors;
using DevExpress.Office.Design.Internal;
namespace DevExpress.Xpf.RichEdit.UI {
	public partial class TabStopPositionEdit : UserControl, IBatchUpdateable, IBatchUpdateHandler {
		TabPositionEditControlProperties properties;
		BatchUpdateHelper batchUpdateHelper;
		UIUnitConverter unitConverter;
		bool deferredPropertiesChanged;
		bool deferredEditValueChanged;
		bool deferredSelectedIndexChanged;
		public TabStopPositionEdit() {
			this.properties = new TabPositionEditControlProperties();
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.unitConverter = new UIUnitConverter(UnitPrecisionDictionary.DefaultPrecisions);
			InitializeComponent();
			SubscribeEvents();
			UpdateControl();
		}
		public TabPositionEditControlProperties Properties { get { return properties; } }
		public string EditValue {
			get { return this.edtCurrentPosition.Text; }
			set { this.edtCurrentPosition.Text = value; }
		}
		public int TabStopPositionIndex {
			get {
				if (!TabStopPosition.HasValue)
					return -1;
				UIUnit unit = unitConverter.ToUIUnit(TabStopPosition.Value, Properties.UnitType);
				return IndexOf(this.lbPositions.Items, unit.ToString());
			}
		}
		public int? TabStopPosition {
			get {
				string editValueString = this.edtCurrentPosition.Text as String;
				if (String.IsNullOrEmpty(editValueString))
					return null;
				UIUnit result = null;
				bool isParsed = UIUnit.TryParse(editValueString, Properties.UnitType, out result);
				if (isParsed)
					return unitConverter.ToTwipsUnit(result);
				return null;
			}
			set {
				if (!value.HasValue) {
					this.edtCurrentPosition.Text = String.Empty;
					return;
				}
				UIUnit unit = unitConverter.ToUIUnit(value.Value, Properties.UnitType);
				string editValueString = this.edtCurrentPosition.Text;
				string newEditValueString = unit.ToString();
				if (TabStopPosition == value && TabStopPosition.HasValue && editValueString == newEditValueString)
					return;
				this.edtCurrentPosition.Text = unit.ToString();
				UpdateControl();
			}
		}
		#region Events
		public event EventHandler PropertiesChanged;
		void RaiseControlPropertiesChanged() {
			if (PropertiesChanged != null)
				PropertiesChanged(this, EventArgs.Empty);
		}
		public event EventHandler EditValueChanged;
		void RaiseEditValueChanged() {
			if (EditValueChanged != null)
				EditValueChanged(this, EventArgs.Empty);
		}
		public event EventHandler SelectedIndexChanged;
		void RaiseSelectedIndexChanged() {
			if (SelectedIndexChanged != null)
				SelectedIndexChanged(this, EventArgs.Empty);
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
			this.lbPositions.SelectionChanged += OnLbPositionsSelectedIndexChanged;
			this.edtCurrentPosition.EditValueChanged += OnEdtCurrentPositionEditValueChanged;
		}
		protected internal virtual void UnsubscribeEvents() {
			Properties.PropertiesChanged -= OnPropertiesChanged;
			this.edtCurrentPosition.KeyDown -= OnEdtCurrentPositionKeyDown;
			this.lbPositions.SelectionChanged -= OnLbPositionsSelectedIndexChanged;
			this.edtCurrentPosition.EditValueChanged -= OnEdtCurrentPositionEditValueChanged;
		}
		protected internal virtual void OnPropertiesChanged(object sender, EventArgs e) {
			UpdateControl();
			OnControlPropertiesChanged();
		}
		protected internal virtual void OnEdtCurrentPositionEditValueChanged(object sender, EditValueChangedEventArgs e) {
			OnEditValueChanged();
		}
		protected internal virtual void OnLbPositionsSelectedIndexChanged(object sender, SelectionChangedEventArgs e) {
			int currentPositionIndex = this.lbPositions.SelectedIndex;
			if (currentPositionIndex == -1)
				return;
			TabStopPosition = Properties.TabFormattingInfo[currentPositionIndex].Position;
			OnSelectedIndexChanged();
		}
		protected internal virtual void OnEdtCurrentPositionKeyDown(object sender, KeyEventArgs e) {
			e.Handled = true;
			if (e.Key == Key.Up) {
				if (this.lbPositions.SelectedIndex > 0)
					this.lbPositions.SelectedIndex--;
			}
			else if (e.Key == Key.Down) {
				if (this.lbPositions.SelectedIndex < this.lbPositions.Items.Count - 1)
					this.lbPositions.SelectedIndex++;
			}
			else
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
		static int IndexOf(ItemCollection ic, string elem) {
			int index = 0;
			foreach (string e in ic) {
				if (((string)e).Equals(elem)) return index;
				index++;
			}
			return -1;
		}
		void UpdateControlCore() {
			this.lbPositions.Items.Clear();
			if (Properties.TabFormattingInfo == null) {
				return;
			}
			int count = Properties.TabFormattingInfo.Count;
			if (count < 1) {
				this.edtCurrentPosition.Text = String.Empty;
				return;
			}
			for (int i = 0; i < count; i++) {
				TabInfo tab = Properties.TabFormattingInfo[i];
				this.lbPositions.Items.Add(GetStringRepresentationsOfUnit(tab.Position));
			}
			string tabStopPositionString = GetStringRepresentationsOfUnit(TabStopPosition);
			string correctedTabStopPositionString = (String.IsNullOrEmpty(tabStopPositionString)) ? this.lbPositions.Items[0] as String : tabStopPositionString;
			this.edtCurrentPosition.Text = correctedTabStopPositionString;
			this.lbPositions.SelectedIndex = IndexOf(this.lbPositions.Items, correctedTabStopPositionString);
			this.lbPositions.UpdateLayout();
		}
		string GetStringRepresentationsOfUnit(int? unitInDocuments) {
			if (unitInDocuments == null)
				return String.Empty;
			UIUnit unit = unitConverter.ToUIUnit(unitInDocuments.Value, Properties.UnitType);
			return unit.ToString();
		}
		public bool DoValidate() {
			UIUnit result = null;
			if (!UIUnit.TryParse(this.edtCurrentPosition.Text, Properties.UnitType, out result)) {
				this.edtCurrentPosition.Focus();
				return false;
			}
			int valueInDocuments = unitConverter.ToTwipsUnit(result);
			if (valueInDocuments < ParagraphFormDefaults.MinTabStopPositionByDefault || ParagraphFormDefaults.MaxTabStopPositionByDefault < valueInDocuments) {
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
