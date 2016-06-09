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
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Editors {
	public interface ICalculatorViewOwner {
		void AddToHistory(string text);
		void AnimateButtonClick(object buttonID);
		void GetCustomErrorText(ref string errorText);
		void SetDisplayText(string value);
		void SetHasError(bool value);
		void SetMemory(decimal value);
		void SetValue(decimal value);
	}
	public abstract class CalculatorViewBase : DependencyObject {
		string displayText;
		decimal displayValue;
		decimal memory;
		int precision;
		decimal result;
		CalcStatus status;
		decimal value;
		Locker initLocker;
		Locker ownerValueSynchronizationLocker;
		Locker valueSynchronizationLocker;
		public CalculatorViewBase(ICalculatorViewOwner owner) {
			initLocker = new Locker();
			ownerValueSynchronizationLocker = new Locker();
			valueSynchronizationLocker = new Locker();
			Owner = owner;
			Strategy = CreateStrategy();
		}
		public CultureInfo Culture {
			get {
				return CultureInfo.CurrentCulture;
			}
		}
		public string DisplayText {
			get { return displayText; }
			set {
				if (value == displayText) return;
				displayText = value;
				if (Owner != null)
					Owner.SetDisplayText(value);
			}
		}
		public decimal DisplayValue {
			get { return displayValue; }
			set {
				if (value == displayValue) return;
				displayValue = value;
				SynchronizeValue();
			}
		}
		public string HistoryString { get; set; }
		public bool IsModified { get; set; }
		public decimal Memory {
			get { return memory; }
			set {
				if (value == memory) return;
				memory = value;
				if (Owner != null)
					Owner.SetMemory(value);
			}
		}
		public int Precision {
			get {
				return precision;
			}
			set {
				if (value == precision) return;
				precision = value;
				UpdateFormatting();
			}
		}
		public decimal Result {
			get { return result; }
			set {
				if (value == result) return;
				result = value;
			}
		}
		public CalcStatus Status {
			get { return status; }
			set {
				if (value == status) return;
				status = value;
				if (Owner != null)
					Owner.SetHasError(value == CalcStatus.Error);
			}
		}
		public decimal Value {
			get { return value; }
			set {
				if (ownerValueSynchronizationLocker.IsLocked || value == this.value) return;
				this.value = value;
				SynchronizeOwnerValue(value);
				if (valueSynchronizationLocker.IsLocked) return;
				initLocker.Lock();
				try {
					Init(value, false);
				} finally {
					initLocker.Unlock();
				}
			}
		}
		protected ICalculatorViewOwner Owner { get; private set; }
		protected CalculatorStrategyBase Strategy { get; private set; }
		public void ResetDisplayValue() {
			ownerValueSynchronizationLocker.DoLockedAction(() => DisplayValue = decimal.MinValue);
		}
		public virtual void Init(decimal value, bool resetMemory) {
			Strategy.Init(value, resetMemory);
		}
		public virtual void OnKeyDown(KeyEventArgs e) {
			object buttonID = GetButtonIDByKey(e);
			e.Handled = buttonID != null;
			if (e.Handled)
				ProcessButtonClickByKeyboard(buttonID);
		}
		public virtual void OnTextInput(TextCompositionEventArgs e) {
			object buttonID = GetButtonIDByTextInput(e);
			e.Handled = buttonID != null;
			if (e.Handled)
				ProcessButtonClickByKeyboard(buttonID);
		}
		public virtual void ProcessButtonClick(object buttonID) {
			IsModified = true;
			Strategy.ProcessButtonClick(buttonID);
		}
		public virtual void UpdateFormatting() {
			Strategy.UpdateFormatting();
		}
		protected internal void AddToHistory(string text) {
			if (Owner != null)
				Owner.AddToHistory(text);
		}
		protected internal void GetCustomErrorText(ref string errorText) {
			if (Owner != null)
				Owner.GetCustomErrorText(ref errorText);
		}
		protected internal void SynchronizeValue() {
			if (initLocker.IsLocked) return;
			valueSynchronizationLocker.Lock();
			try {
				Value = Decimal.Round(DisplayValue, Precision);
			} finally {
				valueSynchronizationLocker.Unlock();
			}
		}
		protected abstract CalculatorStrategyBase CreateStrategy();
		protected abstract object GetButtonIDByKey(KeyEventArgs e);
		protected abstract object GetButtonIDByTextInput(TextCompositionEventArgs e);
		protected virtual void ProcessButtonClickByKeyboard(object buttonID) {
			if (Owner != null)
				Owner.AnimateButtonClick(buttonID);
			ProcessButtonClick(buttonID);
		}
		void SynchronizeOwnerValue(decimal value) {
			if (Owner != null) {
				ownerValueSynchronizationLocker.Lock();
				try {
					Owner.SetValue(value);
				} finally {
					ownerValueSynchronizationLocker.Unlock();
				}
			}
		}
		#region Clipboard
		protected internal virtual bool CanCopy() {
			return true;
		}
		protected internal virtual bool CanPaste() {
			return Status != CalcStatus.Error;
		}
		protected internal virtual void Copy() {
			DXClipboard.SetText(DisplayText);
		}
		protected internal virtual void Paste() {
			if (DXClipboard.ContainsText())
				Strategy.SetDisplayText(DXClipboard.GetText());
		}
		#endregion
	}
	public abstract class CalculatorStrategyBase {
		public CalculatorStrategyBase(CalculatorViewBase view) {
			View = view;
		}
		protected CultureInfo Culture {
			get { return View.Culture; }
		}
		protected string DisplayText {
			get { return View.DisplayText; }
			set { View.DisplayText = value; }
		}
		protected decimal DisplayValue {
			get { return View.DisplayValue; }
			set { View.DisplayValue = value; }
		}
		public string HistoryString {
			get { return View.HistoryString; }
			set { View.HistoryString = value; }
		}
		protected bool IsModified {
			get { return View.IsModified; }
			set { View.IsModified = value; }
		}
		protected decimal Memory {
			get { return View.Memory; }
			set { View.Memory = value; }
		}
		protected int Precision {
			get { return View.Precision; }
		}
		protected object PrevButtonID { get; private set; }
		protected decimal Result {
			get { return View.Result; }
			set { View.Result = value; }
		}
		protected CalcStatus Status {
			get { return View.Status; }
			set { View.Status = value; }
		}
		protected CalculatorViewBase View { get; private set; }
		public virtual void Init(decimal value, bool resetMemory) {
			IsModified = false;
			PrevButtonID = null;
		}
		public virtual void ProcessButtonClick(object buttonID) {
			try {
				try {
					ProcessButtonClickInternal(buttonID);
				} catch {
					Error(EditorLocalizer.GetString(EditorStringId.CalculatorError));
				}
			} finally {
				PrevButtonID = buttonID;
			}
		}
		public virtual void SetDisplayText(string text) { }
		public virtual void UpdateFormatting() { }
		protected void AddToHistory(string text) {
			View.AddToHistory(text);
		}
		protected virtual void Error(string message) {
			GetCustomErrorText(ref message);
			DisplayText = message;
			DisplayValue = 0;
			Status = CalcStatus.Error;
			Result = 0;
		}
		protected void GetCustomErrorText(ref string errorText) {
			View.GetCustomErrorText(ref errorText);
		}
		protected abstract void ProcessButtonClickInternal(object buttonID);
		protected void SynchronizeValue() {
			View.SynchronizeValue();
		}
	}
}
