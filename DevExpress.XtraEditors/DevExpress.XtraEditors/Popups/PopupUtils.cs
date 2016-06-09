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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraEditors {
	[ToolboxItem(false)]
	public class PopupEditActivator : Component {
		private static readonly object queryCloseUp = new object();
		private static readonly object queryPopUp = new object();
		private static readonly object closeUp = new object();
		RepositoryItemPopupBase _popupItem;
		PopupBaseEdit _popupEdit;
		Control owner, focusedControl;
		Rectangle ownerFakeBounds;
		bool forceTopMost;
		bool reuseItem;
		public PopupEditActivator() : this(false) { }
		public PopupEditActivator(bool forceTopMost) : this(forceTopMost, false) { }
		public PopupEditActivator(bool forceTopMost, bool reuseItem) {
			this.reuseItem = reuseItem;
			this.focusedControl = null;
			this._popupItem = null;
			this._popupEdit = null;
			this.owner = null;
			this.forceTopMost = forceTopMost;
			this.ownerFakeBounds = Rectangle.Empty;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DestroyPopup();
			}
			base.Dispose(disposing);
		}
		protected internal Rectangle OwnerFakeBounds {
			get { return ownerFakeBounds; }
			set { ownerFakeBounds = value; }
		}
		[DXCategory(CategoryName.Behavior), DefaultValue(null)]
		public virtual Control Owner {
			get { return owner; }
			set {
				if(Owner == value) return;
				DestroyPopup();
				this.owner = value;
			}
		}
		[DXCategory(CategoryName.Behavior), DefaultValue(null)]
		public virtual RepositoryItemPopupBase PopupItem {
			get { return _popupItem; }
			set {
				if(PopupItem == value) return;
				if(PopupItem != null) {
					DestroyPopup();
				}
				this._popupItem = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false)]
		public virtual bool IsPopupOpen { get { return PopupEdit == null ? false : PopupEdit.IsPopupOpen; } }
		public virtual void ShowPopup(int x, int y) {
			DoShowPopup(new Rectangle(x, y, 0, 0));
		}
		public virtual void ShowPopup(Rectangle ownerBounds) {
			DoShowPopup(ownerBounds);
		}
		public virtual void ClosePopup() {
			if(PopupEdit != null) {
				PopupEdit.ClosePopup();
				DoClosePopup();
			}
		}
		public virtual void CancelPopup() {
			if(PopupEdit != null) {
				PopupEdit.CancelPopup();
				DoClosePopup();
			}
		}
		[Browsable(false)]
		public virtual bool ContainsFocus { get { return PopupEdit != null && PopupEdit.EditorContainsFocus; } }
		[Browsable(false)]
		public virtual PopupBaseEdit PopupEdit { get { return _popupEdit; } }
		protected virtual void DoShowPopup(Rectangle ownerBounds) {
			if(PopupItem != null) CreatePopupEdit();
			if(Owner == null || PopupEdit == null) return;
			this.focusedControl = Form.ActiveForm == null ? null : Form.ActiveForm.ActiveControl;
			Owner.Controls.Add(PopupEdit);
			int height = ownerBounds.Height;
			OwnerFakeBounds = ownerBounds;
			PopupEdit.AltBounds = ownerBounds;
			PopupEdit.Location = ownerBounds.Location;
			PopupEdit.BackColor = Color.Transparent;
			PopupEdit.Size = new Size(ownerBounds.Width, ownerBounds.Height);
			PopupEdit.Visible = true;
			PopupEdit.Focus();
			PopupEdit.PopupAllowClick += new PopupAllowClickEventHandler(OnPopup_AllowMouseClick);
			PopupEdit.CloseUp += new CloseUpEventHandler(OnPopup_CloseUp);
			PopupEdit.QueryCloseUp += new CancelEventHandler(OnPopup_QueryCloseUp);
			PopupEdit.QueryPopUp += new CancelEventHandler(OnPopup_QueryPopUp);
			PopupEdit.ShowPopup();
			if(this.forceTopMost) if(PopupEdit.PopupForm != null) PopupEdit.PopupForm.TopMost = true;
		}
		protected virtual void DoClosePopup() {
			if(PopupEdit == null) return;
			PopupEdit.PopupAllowClick -= new PopupAllowClickEventHandler(OnPopup_AllowMouseClick);
			PopupEdit.CloseUp -= new CloseUpEventHandler(OnPopup_CloseUp);
			PopupEdit.QueryCloseUp -= new CancelEventHandler(OnPopup_QueryCloseUp);
			PopupEdit.QueryPopUp -= new CancelEventHandler(OnPopup_QueryPopUp);
			bool focused = PopupEdit.EditorContainsFocus;
			PopupEdit.Visible = false;
			if(focused) {
				if(this.focusedControl != null) this.focusedControl.Focus();
			}
			this.focusedControl = null;
		}
		protected virtual void OnPopup_AllowMouseClick(object sender, PopupAllowClickEventArgs e) {
			if(e.Control == Owner && Owner != null && Owner.IsHandleCreated) {
				Point p = Owner.PointToClient(e.MousePosition);
				if(OwnerFakeBounds.Contains(p)) e.Allow = true;
			}
		}
		protected virtual void OnPopup_QueryCloseUp(object sender, CancelEventArgs e) {
			RaiseQueryCloseUp(e);
		}
		protected virtual void OnPopup_QueryPopUp(object sender, CancelEventArgs e) {
			RaiseQueryPopUp(e);
		}
		protected virtual void OnPopup_CloseUp(object sender, CloseUpEventArgs e) {
			RaiseCloseUp(e);
			DoClosePopup();
		}
		public virtual void DestroyPopup() {
			if(PopupEdit != null) {
				if(reuseItem) {
					if(PopupItem != null) {
						RepositoryItem item = (RepositoryItem)PopupItem.Clone();
						PopupItem.SetOwnerEdit(null);
						item.SetOwnerEdit(PopupEdit);
					}
				}
				this._popupEdit.Dispose();
				this._popupEdit = null;
			}
			this.focusedControl = null;
		}
		public virtual void RefreshPopup() {
			if(PopupEdit != null && PopupEdit.IsPopupOpen) PopupEdit.RefreshPopup();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void RefreshPopup(PopupBaseEdit editor) {
			editor.RefreshPopup();
		}
		protected virtual void CreatePopupEdit() {
			if(PopupEdit != null) return;
			if(PopupItem == null || Owner == null) return;
			this._popupEdit = PopupItem.CreateEditor() as PopupBaseEdit;
			this._popupEdit.Visible = false;
			if(reuseItem) {
				RepositoryItem oldProperties = this._popupEdit.Properties;
				oldProperties.SetOwnerEdit(null);
				this.PopupItem.SetOwnerEdit(this._popupEdit);
				oldProperties.Dispose();
			}
			else {
				this._popupEdit.Properties.Assign(PopupItem);
			}
			this._popupEdit.Properties.TextEditStyle = TextEditStyles.HideTextEditor;
			this._popupEdit.Properties.Buttons.Clear();
			this._popupEdit.Properties.BorderStyle = BorderStyles.NoBorder;
			this._popupEdit.Properties.AutoHeight = false;
			this._popupEdit.Size = Size.Empty;
		}
		protected virtual void OnPropertiesChanged() {
			DestroyPopup();
		}
		[DXCategory(CategoryName.Events)]
		public event CancelEventHandler QueryCloseUp {
			add { this.Events.AddHandler(queryCloseUp, value); }
			remove { this.Events.RemoveHandler(queryCloseUp, value); }
		}
		[DXCategory(CategoryName.Events)]
		public event CancelEventHandler QueryPopUp {
			add { this.Events.AddHandler(queryPopUp, value); }
			remove { this.Events.RemoveHandler(queryPopUp, value); }
		}
		[DXCategory(CategoryName.Events)]
		public event CloseUpEventHandler CloseUp {
			add { this.Events.AddHandler(closeUp, value); }
			remove { this.Events.RemoveHandler(closeUp, value); }
		}
		protected internal virtual void RaiseQueryCloseUp(CancelEventArgs e) {
			CancelEventHandler handler = (CancelEventHandler)this.Events[queryCloseUp];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseQueryPopUp(CancelEventArgs e) {
			CancelEventHandler handler = (CancelEventHandler)this.Events[queryPopUp];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCloseUp(CloseUpEventArgs e) {
			CloseUpEventHandler handler = (CloseUpEventHandler)this.Events[closeUp];
			if(handler != null) handler(this, e);
		}
	}
	public static class EnumDisplayTextHelper {
		internal static bool IsEnum(Type type) {
			return type.IsEnum || (IsNullable(type) && Nullable.GetUnderlyingType(type).IsEnum);
		}
		static System.Collections.Hashtable enumDisplayTexts = new System.Collections.Hashtable();
		public static void ResetDisplayTextsCache(Type enumType) {
			var enumValues = GetEnumValues(enumType);
			foreach(object value in enumValues)
				enumDisplayTexts.Remove(value);
		}
		public static string GetCachedDisplayText(object value) {
			if(!enumDisplayTexts.ContainsKey(value)) 
				enumDisplayTexts[value] = GetDisplayText(value);
			return enumDisplayTexts[value] as string;
		}
		[System.Security.SecuritySafeCritical]
		public static string GetDisplayText(object value) {
			if(value == null) 
				return string.Empty;
			string result = value.ToString();
			Type enumType = value.GetType();
			MemberInfo[] info = enumType.GetMember(result);
			if(info.Length == 0)
				return result;
			object[] attributes = info[0].GetCustomAttributes(false);
			for(int i = 0; i < attributes.Length; i++) {
				Type attributeType = attributes[i].GetType();
				if(typeof(DisplayNameAttribute).IsAssignableFrom(attributeType))
					return ((DisplayNameAttribute)attributes[i]).DisplayName;
				if(attributeType == typeof(System.ComponentModel.DataAnnotations.DisplayAttribute))
					return ((System.ComponentModel.DataAnnotations.DisplayAttribute)attributes[i]).GetName();
				if(typeof(DescriptionAttribute).IsAssignableFrom(attributeType))
					return ((DescriptionAttribute)attributes[i]).Description;
			}
			TypeConverter converter = TypeDescriptor.GetConverter(enumType);
			if(converter != null)
				return converter.ConvertToString(value);
			return result;
		}
		internal static Array GetEnumValues(Type enumType) {
			if(IsNullable(enumType))
				enumType = enumType.GetGenericArguments()[0];
			if(enumType.IsEnum)
				return System.Enum.GetValues(enumType);
			return Array.CreateInstance(enumType, 0);
		}
		internal static TEnum[] GetEnumValues<TEnum>() {
			Type enumType = typeof(TEnum);
			bool nullable = IsNullable(enumType);
			if(nullable)
				enumType = enumType.GetGenericArguments()[0];
			if(enumType.IsEnum) {
				var values = System.Enum.GetValues(enumType);
				if(nullable) {
					TEnum[] nullableValues = new TEnum[values.Length];
					for(int i = 0; i < nullableValues.Length; i++)
						nullableValues[i] = (TEnum)values.GetValue(i);
					return nullableValues;
				}
				return (TEnum[])values;
			}
			return new TEnum[] { };
		}
		static bool IsNullable(Type sourceType) {
			return sourceType.IsGenericType && sourceType.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
		internal static object GetEnumValue(bool addEnumeratorIntegerValues, object val, Type enumType) {
			if(addEnumeratorIntegerValues) {
				Type enumValueType = Enum.GetUnderlyingType(enumType);
				try { return Convert.ChangeType(val, enumValueType); }
				catch { }
			}
			return val;
		}
		internal static int GetEnumUnderlyingValue(Enum enumValue) {
			return (int)Convert.ChangeType(enumValue, enumValue.GetTypeCode());
		}
		internal static bool IsCompositeValue(Enum enumValue) {
			int bitsSet = 0;
			int value = GetEnumUnderlyingValue(enumValue);
			int bitsCount = Marshal.SizeOf(value) * 8;
			for(int i = 0; i < bitsCount && value != 0; i++) {
				if((value & 0x1) != 0) bitsSet++;
				value >>= 1;
			}
			return bitsSet > 1;
		}
		internal static object GetNoneEnumValue(Type enumType) {
			object val = null;
			try {
				val = Enum.ToObject(enumType, 0);
			}
			catch { }
			return val;
		}
		internal static bool IsNoneEnumValue(Enum enumValue) {
			return GetEnumUnderlyingValue(enumValue) == 0;
		}
	}
}
