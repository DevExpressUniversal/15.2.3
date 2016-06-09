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
using DevExpress.Office;
using DevExpress.Xpf.Core;
using System.Windows.Input;
using DevExpress.Xpf.Editors;
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.Office.Design.Internal;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.RichEdit.UI {
	[DXToolboxBrowsableAttribute(false)]
	public class RichTextIndentEdit : TextEdit, IDisposable {
		#region Fields
		readonly UIUnitConverter uiUnitConverter;
		DocumentModelUnitConverter valueUnitConverter;
		int? exactValue = 0;
		bool isValueInPercent;
		bool isUseValueInPercent;
		#endregion
		public RichTextIndentEdit() {
			this.uiUnitConverter = new UIUnitConverter(UnitPrecisionDictionary.DefaultPrecisions);
			this.valueUnitConverter = new DocumentModelUnitTwipsConverter();
			MinValue = -40000;
			MaxValue = 40000;
			DefaultUnitType = DocumentUnit.Inch;
			EditValueChanged += OnEditValueChanged;
			LostFocus += OnLostFocus;
		}
		public bool Visible { get { return this.GetVisible(); } }
		public bool Enabled { get { return IsEnabled; } }
		public int MinValue { get; set; }
		public int MaxValue { get; set; }
		public DocumentUnit DefaultUnitType { get; set; }
		public event EventHandler ValueChanged;
		public DocumentModelUnitConverter ValueUnitConverter { get { return valueUnitConverter; } set { valueUnitConverter = value; } }
		protected internal bool IsUseValueInPercent { get { return isUseValueInPercent; } }
		public bool IsValueInPercent {
			get {
				return isValueInPercent;
			}
			set {
				isValueInPercent = value;
				isUseValueInPercent |= value;
			}
		}
		bool IsWithinAllowedLimits(int value) {
			return value >= MinValue && value <= MaxValue;
		}
		public bool IsModified { get; set; } 
		protected internal float? CoreValue {
			get {
				if (String.IsNullOrEmpty(Text))
					return null;
				UIUnit unit = UIUnit.Create(Text, DefaultUnitType, UnitPrecisionDictionary.DefaultPrecisions, IsValueInPercent);
				int resultValue = uiUnitConverter.ToTwipsUnit(unit, IsValueInPercent);
				return resultValue;
			}
			set {
				if (value == null) {
					Text = String.Empty;
					return;
				}
				float resultValue = value.Value;
				UIUnit oldUnit = UIUnit.Create(Text, DefaultUnitType, UnitPrecisionDictionary.DefaultPrecisions);
				UIUnit unit = uiUnitConverter.ToUIUnitF(resultValue, oldUnit.UnitType, IsValueInPercent);
				if (!IsWithinAllowedLimits(uiUnitConverter.ToTwipsUnit(unit, IsValueInPercent))) {
					unit = uiUnitConverter.ToUIUnit(0, oldUnit.UnitType, IsValueInPercent);
					this.exactValue = 0;
				}
				if (!String.IsNullOrEmpty(Text) && oldUnit.Value == unit.Value && !IsUseValueInPercent)
					return;
				Text = unit.ToString();
				OnValueChanged();
			}
		}
		public int? Value {
			get {
				if (exactValue.HasValue)
					return exactValue.Value;
				float? result = CoreValue;
				if (result == null)
					return null;
				return ValueUnitConverter.TwipsToModelUnits((int)result.Value);
			}
			set {
				float? newValue = (value == null) ? ((float?)value) : (float?)ValueUnitConverter.ModelUnitsToTwipsF(value.Value);
				CoreValue = newValue;
				this.exactValue = value;
				if (!newValue.HasValue)
					return;
				if (!IsWithinAllowedLimits((int)Math.Round(newValue.Value)))
					this.exactValue = ValueUnitConverter.TwipsToModelUnits(MinValue);
			}
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			if (e.Key == Key.Enter)
				ApplyTextChanged();
			base.OnKeyUp(e);
		}
		void OnLostFocus(object sender, RoutedEventArgs e) {
			ApplyTextChanged();
		}
		protected virtual void ApplyTextChanged() {
			if (ValidateCore()) {
				exactValue = null;
				OnValueChanged();
			} else
				Value = 0;
		}
		void OnEditValueChanged(object sender, EditValueChangedEventArgs e) {
			ApplyTextChanged();
		}
		protected virtual void OnValueChanged() {
			exactValue = Value;
			RaiseValueChanged();
		}
		protected internal virtual void RaiseValueChanged() {
			if (ValueChanged != null) {
				ValueChanged(this, EventArgs.Empty);
			}
		}
		public override bool DoValidate() {
			bool result = ValidateCore();
			if (result) {
				exactValue = null;
			} else
				Value = 0;
			return true;
		}
		protected bool ValidateCore() {
			UIUnit result = null;
			bool isValid = UIUnit.TryParse(Text, DefaultUnitType, out result, IsValueInPercent);
			if (!isValid) return false;
			return IsWithinAllowedLimits(uiUnitConverter.ToTwipsUnit(result, IsValueInPercent));
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
		#endregion
	}
}
