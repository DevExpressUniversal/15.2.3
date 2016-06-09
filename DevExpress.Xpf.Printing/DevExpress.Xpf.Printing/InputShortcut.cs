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
using System.Windows.Input;
using DevExpress.Utils;
namespace DevExpress.Xpf.Printing {
	public enum MouseWheelScrollingDirection { None, Up, Down };
	public enum MouseInputAction { None, LeftClick, RightClick, MiddleClick, WheelClick };
	public abstract class InputShortcut {
		readonly List<ModifierKeys> modifiers;
		public InputShortcut()
			: this(new ModifierKeys[] { }) {
		}
		public InputShortcut(ModifierKeys[] modifiers) {
			Guard.ArgumentNotNull(modifiers, "modifiers");
			modifiers = modifiers.Distinct().ToArray<ModifierKeys>();
			this.modifiers = new List<ModifierKeys>(modifiers);
			if(this.modifiers.Contains(ModifierKeys.None))
				this.modifiers.Remove(ModifierKeys.None);
		}
		public ModifierKeys[] Modifiers {
			get { return modifiers.ToArray(); }
		}
		public virtual string DisplayString {
			get { return Modifiers.Length != 0 ? string.Join<ModifierKeys>("+", Modifiers) : Enum.GetName(typeof(ModifierKeys), ModifierKeys.None); }
		}
		protected bool AreModifierArraysEqual(ModifierKeys[] first, ModifierKeys[] second) {
			if(object.ReferenceEquals(first, null) || object.ReferenceEquals(second, null))
				return false;
			if(first.Length != second.Length)
				return false;
			foreach(ModifierKeys item in first)
				if(!second.Contains(item))
					return false;
			return true;
		}
		public override int GetHashCode() {
			return HashCodeHelper.CalcHashCode(ArrayHelper.ConvertAll<char, int>(DisplayString.ToCharArray(), (chr) => (int)chr));
		}
	}
	public class KeyShortcut : InputShortcut {
		public KeyShortcut(Key key)
			: this(new ModifierKeys[] { }, key) {
		}
		public KeyShortcut(ModifierKeys modifier, Key key)
			: this(new ModifierKeys[] { modifier }, key) {
		}
		public KeyShortcut(ModifierKeys[] modifiers, Key key)
			: base(modifiers) {
			Key = key;
		}
		public Key Key {
			get;
			set;
		}
		public override string DisplayString {
			get {
				string modifiers = base.DisplayString;
				return String.Format("{0}+{1}", modifiers, Key);
			}
		}
		public override bool Equals(object obj) {
			KeyShortcut another = obj as KeyShortcut;
			return !Object.ReferenceEquals(another, null) &&
				AreModifierArraysEqual(this.Modifiers, another.Modifiers) &&
				this.Key == another.Key;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public class MouseShortcut : InputShortcut {
		public MouseShortcut(MouseInputAction mouseAction)
			: this(new ModifierKeys[] { }, mouseAction, MouseWheelScrollingDirection.None) {
		}
		public MouseShortcut(ModifierKeys modifier, MouseInputAction mouseAction)
			: this(new ModifierKeys[] { modifier }, mouseAction, MouseWheelScrollingDirection.None) {
		}
		public MouseShortcut(ModifierKeys[] modifiers, MouseInputAction mouseAction)
			: this(modifiers, mouseAction, MouseWheelScrollingDirection.None) {
		}
		public MouseShortcut(MouseInputAction mouseAction, MouseWheelScrollingDirection scrollingDirection)
			: this(new ModifierKeys[] { }, mouseAction, scrollingDirection) {
		}
		public MouseShortcut(ModifierKeys modifier, MouseInputAction mouseAction, MouseWheelScrollingDirection scrollingDirection)
			: this(new ModifierKeys[] { modifier }, mouseAction, scrollingDirection) {
		}
		public MouseShortcut(ModifierKeys[] modifiers, MouseInputAction mouseAction, MouseWheelScrollingDirection scrollingDirection)
			: base(modifiers) {
			MouseAction = mouseAction;
			ScrollingDirection = scrollingDirection;
		}
		public MouseInputAction MouseAction {
			get;
			set;
		}
		public MouseWheelScrollingDirection ScrollingDirection {
			get;
			set;
		}
		public override string DisplayString {
			get {
				string modifiers = base.DisplayString;
				return String.Format("{0}+{1}+Scroll {2}", modifiers, MouseAction, ScrollingDirection);
			}
		}
		public override bool Equals(object obj) {
			MouseShortcut another = obj as MouseShortcut;
			return !Object.ReferenceEquals(another, null) &&
				AreModifierArraysEqual(this.Modifiers, another.Modifiers) &&
				this.MouseAction == another.MouseAction &&
				this.ScrollingDirection == another.ScrollingDirection;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
