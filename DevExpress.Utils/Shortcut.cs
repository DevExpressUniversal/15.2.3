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
using System.Drawing.Drawing2D;
using System.Collections;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
namespace DevExpress.Utils {
	[TypeConverter("DevExpress.Utils.Design.Serialization.KeyShortcutTypeConverter, " + AssemblyInfo.SRAssemblyDesign),
	DesignerSerializer("DevExpress.Utils.Design.Serialization.KeyShortcutCodeDomSerializer, " + AssemblyInfo.SRAssemblyDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	public class KeyShortcut {
		Keys key;
		public KeyShortcut(Shortcut shortcut) : this((Keys)shortcut) {
		}
		public KeyShortcut() : this(Keys.None) {
		}
		public KeyShortcut(Keys key) {
			this.key = CheckKey(key, false);
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("KeyShortcutKey")]
#endif
		public virtual Keys Key { get { return key; } }
		public override string ToString() {
			if(this == Empty) return "(none)";
			if(!IsExist) return "";
			string res = GetKeyDisplayText(Key);
			return res;
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("KeyShortcutIsExist")]
#endif
		public virtual bool IsExist {
			get {
				if(Key == Keys.None || !IsValidShortcut(Key)) return false;
				return true;
			}
		}
		protected virtual Keys CheckKey(Keys key, bool isSecond) {
			Keys v = IsValidShortcut(key) ? key : Keys.None;
			if(isSecond) {
				if(Key == Keys.None) v = Keys.None;
			}
			return v;
		}
		protected virtual bool IsValidShortcut(Keys key) {
			if(key == Keys.None) return false;
			key = key & (~Keys.Modifiers);
			if(key == Keys.None || key == Keys.ControlKey || key == Keys.ShiftKey || key == Keys.Alt) return false;
			return true;
		}
		public static string GetKeyDisplayText(Keys key) {
			string res = "";
			if(key == Keys.None) return res;
			if((key & Keys.Control) != 0 || key == Keys.ControlKey) res = ControlKeyName;
			if((key & Keys.Shift) != 0 || key == Keys.ShiftKey) res += (res.Length > 0 ? "+" : "") + ShiftKeyName;
			if((key & Keys.Alt) != 0 || key == Keys.Alt) res += (res.Length > 0 ? "+" : "") + AltKeyName;
			key = key & (~Keys.Modifiers);
			if(key != Keys.None) res += (res.Length > 0 ? "+" : "") + key.ToString();
			return res;
		}
		static string GetModifierKeyName(Keys key) {
			string keyName = new KeysConverter().ConvertToString(key);
			int index = keyName.IndexOf("+");
			if(index == -1) return keyName;
			return keyName.Substring(0, index);
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("KeyShortcutAltKeyName")]
#endif
		public static string AltKeyName { get { return GetModifierKeyName(Keys.Alt); } }
#if !SL
	[DevExpressUtilsLocalizedDescription("KeyShortcutShiftKeyName")]
#endif
		public static string ShiftKeyName { get { return GetModifierKeyName(Keys.Shift); } }
#if !SL
	[DevExpressUtilsLocalizedDescription("KeyShortcutControlKeyName")]
#endif
		public static string ControlKeyName { get { return GetModifierKeyName(Keys.Control); } }
		public static KeyShortcut Empty = new KeyShortcut();
		public static bool operator ==(KeyShortcut left, KeyShortcut right) {
			if(Object.ReferenceEquals(left, right)) return true;
			if(Object.ReferenceEquals(left, null)) return false;
			if(Object.ReferenceEquals(right, null)) return false;
			return (left.Key == right.Key);
		}
		public static bool operator !=(KeyShortcut left, KeyShortcut right) {
			return !(left == right);
		}
		public override bool Equals(object value) {
			KeyShortcut shcut = value as KeyShortcut;
			if(shcut == null) return false;
			return this.key == shcut.Key;
		}
		public override int GetHashCode() { return base.GetHashCode(); }
	}
}
