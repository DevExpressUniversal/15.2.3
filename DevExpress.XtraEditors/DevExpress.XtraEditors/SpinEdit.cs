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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemSpinEdit : RepositoryItemBaseSpinEdit {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemSpinEdit Properties { get { return this; } }
		private static object valueChanged = new object();
		decimal _minValue, _maxValue, _increment;
		bool _isFloatValue;
		public RepositoryItemSpinEdit() {
			this._isFloatValue = true;
			this._minValue = this._maxValue = Decimal.Zero;
			this._increment = 1;
		}
		protected override MaskProperties CreateMaskProperties() {
			return new NumericMaskProperties();
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemSpinEdit source = item as RepositoryItemSpinEdit;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this._increment = source.Increment;
				this._isFloatValue = source.IsFloatValue;
				this._maxValue = source.MaxValue;
				this._minValue = source.MinValue;
			} finally {
				EndUpdate();
			}
			Events.AddHandler(valueChanged, source.Events[valueChanged]);
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "SpinEdit"; } }
		protected new SpinEdit OwnerEdit { get { return base.OwnerEdit as SpinEdit; } }
		bool ShouldSerializeMinValue() { return MinValue != Decimal.Zero; }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemSpinEditMinValue"),
#endif
 RefreshProperties(System.ComponentModel.RefreshProperties.All), SmartTagProperty("MinValue", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public decimal MinValue {
			get { return _minValue; }
			set {
				if(MinValue == value) return;
				_minValue = value;
				if(MaxValue < MinValue) MaxValue = MinValue;
				OnMinMaxChanged();
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializeMaxValue() { return MaxValue != Decimal.Zero; }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemSpinEditMaxValue"),
#endif
 RefreshProperties(System.ComponentModel.RefreshProperties.All), SmartTagProperty("MaxValue", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public decimal MaxValue {
			get { return _maxValue; }
			set {
				if(MaxValue == value) return;
				_maxValue = value;
				if(MinValue > MaxValue) MinValue = MaxValue;
				OnMinMaxChanged();
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializeIncrement() { return Increment != 1; }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemSpinEditIncrement"),
#endif
 SmartTagProperty("Increment", "")]
		public decimal Increment {
			get { return _increment; }
			set {
				if(Increment == value) return;
				_increment = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemSpinEditIsFloatValue"),
#endif
 DefaultValue(true)]
		[RefreshProperties(RefreshProperties.All), SmartTagProperty("Allow Float Value", "", 0)]
		public bool IsFloatValue {
			get { return _isFloatValue; }
			set {
				if(IsFloatValue == value) return;
				_isFloatValue = value;
				OnPropertiesChanged();
				if(!IsLoading) {
					if(IsFloatValue) {
						if(Mask.EditMask == "N00")
							Mask.EditMask = string.Empty;
					} else {
						if(Mask.EditMask == string.Empty)
							Mask.EditMask = "N00";
					}
				}
			}
		}
		protected virtual void OnMinMaxChanged() {
			if(OwnerEdit != null) OwnerEdit.OnMinMaxChanged();
		}
		protected internal decimal CorrectValue(decimal val) {
			if(!this.IsFloatValue)
				val = Math.Round(val);
			if(this.MaxValue != Decimal.Zero || this.MinValue != Decimal.Zero) {
				if(val < this.MinValue)
					val = this.MinValue;
				if(val > this.MaxValue)
					val = this.MaxValue;
			}
			return val;
		}
		protected internal override void RaiseEditValueChanged(EventArgs e) {
			if(OwnerEdit != null && !IsNullValue(OwnerEdit.EditValue)) {
				decimal currentValue = OwnerEdit.Value;
				decimal checkedValue = CorrectValue(currentValue);
				if(currentValue != checkedValue)
					return;
			}
			base.RaiseEditValueChanged(e);
		}
		protected override bool IsNeededKeyCore(Keys keyData) {
			if(OwnerEdit != null && OwnerEdit.InplaceType == InplaceType.Bars && (keyData == Keys.Up || keyData == Keys.Down))
				return true;
			return base.IsNeededKeyCore(keyData);
		}
		protected override bool AllowFormatEditValue { get { return false; } }
		protected override bool AllowParseEditValue { get { return true; } }
		[Browsable(false)]
		public override HorzAlignment DefaultAlignment { get { return HorzAlignment.Far; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemSpinEditEditMask"),
#endif
 DXCategory(CategoryName.Format),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All)
		]
		public string EditMask {
			get { return Mask.EditMask; }
			set { Mask.EditMask = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemSpinEditValueChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler ValueChanged {
			add { this.Events.AddHandler(valueChanged, value); }
			remove { this.Events.RemoveHandler(valueChanged, value); }
		}
		protected override void RaiseEditValueChangedCore(EventArgs e) {
			base.RaiseEditValueChangedCore(e);
			RaiseValueChanged(e);
		}
		protected internal virtual void RaiseValueChanged(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[valueChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
	}
}
namespace DevExpress.XtraEditors {
	[DXToolboxItem(DXToolboxItemKind.Free), DefaultBindingPropertyEx("Value"),
	 Description("Allows an end-user to edit numeric values and provides up and down buttons for incrementing and decrementing values."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "SpinEdit")
	]
	public class SpinEdit : BaseSpinEdit {
		public SpinEdit() {
			this.fEditValue = Decimal.Zero;
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "SpinEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SpinEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemSpinEdit Properties { get { return base.Properties as RepositoryItemSpinEdit; } }
		[Bindable(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		decimal ConvertToDecimal(object val) {
			if(val == null)
				return Decimal.Zero;
			decimal res = Decimal.Zero;
			try { res = Convert.ToDecimal(val); } catch { }
			return res;
		}
		object ConvertFromDecimal(decimal val) {
			if(!Properties.IsFloatValue) {
				try {
					val = Math.Round(val);
					return (Int32)val;
				} catch { }
				try { return (UInt32)val; } catch { }
				try { return (Int64)val; } catch { }
				try { return (UInt64)val; } catch { }
			}
			return val;
		}
		[Bindable(ControlConstants.NonObjectBindable), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SpinEditValue"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), SmartTagProperty("Value", "")]
		public virtual decimal Value {
			get {
				FlushPendingEditActions();
				return ConvertToDecimal(EditValue);
			}
			set {
				EditValue = ConvertFromDecimal(value);
			}
		}
		[Browsable(false)]
		public override object EditValue {
			get {
				return base.EditValue;
			}
			set {
				if(Properties.IsNullValue(value))
					value = null;
				else
					value = ConvertToDecimal(value);
				base.EditValue = value;
			}
		}
		protected override object ExtractParsedValue(ConvertEditValueEventArgs e) {
			if(e.Handled)
				return e.Value;
			if(Properties.IsNullInputAllowed && Properties.IsNullValue(e.Value))
				return null;
			return Properties.CorrectValue(ConvertToDecimal(e.Value));
		}
		protected internal virtual void OnMinMaxChanged() {
			if(IsLoading) return;
			if(Properties.IsNullInputAllowed && Properties.IsNullValue(this.EditValue)) {
			} else {
				Value = Properties.CorrectValue(Value);
			}
		}
		decimal Increment(decimal val, decimal inc) {
			if(Properties.ReadOnly) return val;
			try {
				return Properties.CorrectValue(val + inc);
			} catch {
				return inc > Decimal.Zero ? Properties.MaxValue : Properties.MinValue;
			}
		}
		protected override void OnSpin(SpinEventArgs e) {
			if(Properties.ReadOnly) return;
			if(Properties.Increment == Decimal.Zero) {
				base.OnSpin(e);
			} else {
				Properties.RaiseSpin(e);
				if(e.Handled)
					return;
				decimal delta = e.IsSpinUp ? Properties.Increment : -Properties.Increment;
				Value = Increment(Value, delta);
				e.Handled = true;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SpinEditValueChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler ValueChanged {
			add { Properties.ValueChanged += value; }
			remove { Properties.ValueChanged -= value; }
		}
	}
}
