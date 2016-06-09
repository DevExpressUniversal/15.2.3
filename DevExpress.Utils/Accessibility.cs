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
using System.Resources;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.Accessibility {
	public enum ChildType { Text, Button, HScroll, VScroll, Item, Toolbar };
	public class AccessibleUserInfoHelper : IDXAccessibleUserInfo {
		Control control;
		public AccessibleUserInfoHelper(Control control) {
			this.control = control;
		}
		string IDXAccessibleUserInfo.DefaultAction { get { return control.AccessibleDefaultActionDescription; } }
		string IDXAccessibleUserInfo.AccessibleName { get { return control.AccessibleName; } }
		string IDXAccessibleUserInfo.AccessibleDescription { get { return control.AccessibleDescription; } }
		AccessibleRole IDXAccessibleUserInfo.AccessibleRole { get { return control.AccessibleRole; } }
	}
	[System.Runtime.InteropServices.ComVisible(false)]
	public interface IDXAccessibleUserInfo {
		string AccessibleName { get; }
		string DefaultAction { get; }
		string AccessibleDescription { get; }
		AccessibleRole AccessibleRole { get; }
	}
	[System.Runtime.InteropServices.ComVisible(false)]
	public interface IDXAccessible {
		void DoDefaultAction();
		void Select(AccessibleSelection flags);
		string AccessibleName { get; }
		string DefaultAction { get; }
		string AccessibleDescription { get; }
		string KeyboardShortcut { get; }
		Rectangle Bounds { get; }
		AccessibleRole AccessibleRole { get; }
		Control GetOwnerControl();
		bool RaiseQueryAccessibilityHelp(QueryAccessibilityHelpEventArgs e);
		int GetChildCount();
		AccessibleObject GetSelected();
		AccessibleObject GetFocused();
		AccessibleObject GetChild(int index);
		AccessibleObject HitTest(int x, int y);
		AccessibleObject Navigate(AccessibleNavigation nav);
		AccessibleObject Parent { get; }
		AccessibleStates State { get; }
		bool IsTopControl { get; }
		string Value { get; set; }
	}
	public class ChildrenInfo {
		Hashtable hash = null;
		int precalculated = -1;
		public ChildrenInfo() { }
		public ChildrenInfo(object kind, int count) {
			this[kind] = count;
		}
		public override int GetHashCode() { return base.GetHashCode(); 	}
		public override bool Equals(object obj) {
			ChildrenInfo info = (ChildrenInfo)obj;
			if(info.Count != Count) return false;
			if(Count == 0) return true;
			if(hash.Count != info.hash.Count) return false;
			foreach(DictionaryEntry entry in hash) {
				if((int)entry.Value != info[entry.Key]) return false;
			}
			return true;
		}
		public void Clear() {
			hash = null;
		}
		public int this[object kind] { 
			get { 
				if(hash == null) return 0;
				object val = hash[kind];
				if(val == null) return 0;
				return (int)val;
			}
			set {
				value = Math.Max(0, value);
				precalculated = -1;
				if(value == 0) {
					if(hash == null) return;
					if(hash.Contains(kind)) {
						hash.Remove(kind);
						if(hash.Count == 0) hash = null;
					}
					return;
				}
				if(hash == null) hash = new Hashtable();
				hash[kind] = value;
			}
		}
		public int Count {
			get {
				if(hash == null) return 0;
				if(precalculated != -1) return precalculated;
				CalculateChildren();
				return precalculated;
			}
		}
		protected void CalculateChildren() {
			precalculated = 0;
			if(hash == null) return;
			foreach(int count in hash.Values) {
				precalculated += count;
			}
			return;
		}
	}
	public class StandardAccessible : BaseAccessible {
		Control control;
		public StandardAccessible(Control control) {
			this.control = control;
			CustomInfo = new AccessibleUserInfoHelper(control);
		}
		protected Control Control { get { return control; } }
		public override AccessibleObject Accessible { 
			get { 
				return Control.AccessibilityObject;
			}
		}
		public override Control GetOwnerControl() { return Control; }
	}
	public class StandardAccessibleEx : StandardAccessible {
		AccessibleObject accessible;
		public StandardAccessibleEx(Control control, AccessibleObject accessible) : base(control) {
			this.accessible = accessible;
		}
		public override AccessibleObject Accessible { get { return accessible; } }
	}
	public class ContainerBaseAccessible : BaseAccessible {
		Control control;
		public ContainerBaseAccessible(Control control) {
			this.control = control;
			CustomInfo = new AccessibleUserInfoHelper(control);
		}
		protected Control Control { get { return control; } }
		public override Control GetOwnerControl() { return Control; }
		protected override AccessibleRole GetRole() { return AccessibleRole.Client; }
		protected override ChildrenInfo GetChildrenInfo() { return new ChildrenInfo("Controls", Control.Controls.Count); }
		protected override void OnChildrenCountChanged() {
			foreach(Control ctrl in Control.Controls) {
				AddChild(new StandardAccessible(ctrl));
			}
		}
	}
	public class BaseAccessible : IDXAccessible {
		IDXAccessibleUserInfo userInfo = null;
		BaseAccessibleObject accessible = null;
		BaseAccessibleCollection children;
		BaseAccessible parent = null;
		protected string GetString(AccStringId id) { return AccLocalizer.Active.GetLocalizedString(id); }
		public virtual AccessibleObject Accessible { 
			get { 
				if(accessible == null) accessible = new BaseAccessibleObject(this);
				return accessible; 
			} 
		}
		protected void SetAccessible(BaseAccessibleObject accessible) {
			this.accessible = accessible;
		}
		public virtual void Substitute(BaseAccessible baseAccessible) {
			if(baseAccessible == null || baseAccessible == this || baseAccessible.accessible == null) return;
			this.accessible = baseAccessible.accessible;
			this.accessible.DXOwner = this;
		}
		public string RemoveMnemonic(string text) {
			if(text == null || text.IndexOf('&') == -1) return text;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for(int n = 0; n < text.Length; n++) {
				if(text[n] == '&' && n < text.Length - 1) {
					if(text[n + 1] == '&') {
						sb.Append('&');
						n++;
						continue;
					}
					continue;
				}
				sb.Append(text[n]);
			}
			return sb.ToString();
		}
		public virtual bool IsTopControl { get { return false; } }
		public BaseAccessible ParentCore { 
			get { 
				return parent; 
			} 
			set { 
				parent = value; 
			} 
		} 
		protected virtual void CreateCollection() {
			if(this.children != null) return;
			this.children = CreateCollectionInstance();
		}
		protected virtual BaseAccessibleCollection CreateCollectionInstance() { return new BaseAccessibleCollection(this); }
		protected void ResetCollection() {
			if(this.children == null) return;
			this.children.Clear();
		}
		protected void AddChild(BaseAccessible accessible) {
			if(accessible == null) return;
			CreateCollection();
			children.Add(accessible);
		}
		public BaseAccessibleCollection Children { 
			get { 
				RequestChildren();
				return children; 
			} 
		}
		public int ChildCount { 
			get { 
				return RequestChildrenCount();
			} 
		}
		public bool HasChildren { 
			get { 
				int res = RequestChildrenCount();
				return res > 0;
			}
		}
		ChildrenInfo precalculatedChildrenInfo;
		protected virtual int RequestChildrenCount() {
			ChildrenInfo info = GetChildrenInfo();
			return info != null ? info.Count : 0;
		}
		protected virtual void ForceUpdateChildren() {
			ResetCollection();
			this.precalculatedChildrenInfo = null;
			RequestChildren();
		}
		protected virtual void RequestChildren() {
			ChildrenInfo info = GetChildrenInfo();
			if(info == null) {
				this.precalculatedChildrenInfo = null;
				return;
			}
			if(precalculatedChildrenInfo != null) {
				if(this.precalculatedChildrenInfo.Equals(info)) return;
			}
			this.precalculatedChildrenInfo = info;
			ResetCollection();
			OnChildrenCountChanged();
		}
		protected virtual void OnChildrenCountChanged() {
		}
		protected virtual ChildrenInfo GetChildrenInfo() { return null; }
		protected bool HasChildrenCore { get { return this.children != null && this.children.Count > 0; } }
		public IDXAccessibleUserInfo CustomInfo {
			get { return userInfo; }
			set { userInfo = value; }
		}
		void IDXAccessible.DoDefaultAction() { DoDefaultAction(); }
		void IDXAccessible.Select(AccessibleSelection flags) { Select(flags); }
		Rectangle IDXAccessible.Bounds { get { return ScreenBounds; } }
		string IDXAccessible.AccessibleName { 
			get { 
				string name = CustomInfo == null ? null : CustomInfo.AccessibleName;
				if(name != null && name.Length > 0) return name;
				return GetName();
			}
		}
		string IDXAccessible.KeyboardShortcut {
			get {
				return GetKeyboardShortcut();
			}
		}
		string IDXAccessible.DefaultAction { 
			get { 
				string res = CustomInfo == null ? null : CustomInfo.DefaultAction;
				if(res != null && res.Length > 0) return res;
				return GetDefaultAction();
			} 
		}
		string IDXAccessible.AccessibleDescription { 
			get { 
				string res = CustomInfo == null ? null : CustomInfo.AccessibleDescription;
				if(res != null && res.Length > 0) return res;
				return GetDescription();
			} 
		}
		AccessibleRole IDXAccessible.AccessibleRole { 
			get { 
				AccessibleRole res = CustomInfo == null ? AccessibleRole.Default : CustomInfo.AccessibleRole;
				if(res != AccessibleRole.Default) return res;
				return GetRole();
			} 
		}
		protected virtual Rectangle RectangleToScreen(Rectangle bounds) {
			if(bounds.IsEmpty || GetOwnerControl() == null) return bounds;
			if(GetOwnerControl().IsHandleCreated) return GetOwnerControl().RectangleToScreen(bounds);
			return bounds;
		}
		protected virtual Point PointToClient(Point point) { 
			if(GetOwnerControl() == null) return point;
			if(GetOwnerControl().IsHandleCreated) return GetOwnerControl().PointToClient(point);
			return point;
		}
		public virtual Rectangle ScreenBounds { get { return RectangleToScreen(ClientBounds); } }
		public virtual Rectangle ClientBounds { get { return IsVisible && GetOwnerControl() != null ? GetOwnerControl().ClientRectangle : Rectangle.Empty; } }
		public virtual bool IsVisible { 
			get { 
				if(GetOwnerControl() != null) return GetOwnerControl().Visible;
				return true; 
			} 
		}
		protected virtual string GetName() { return null; }
		protected virtual void DoDefaultAction() { }
		protected virtual void Select(AccessibleSelection flags) { }
		protected virtual AccessibleRole GetRole() { return AccessibleRole.None; }
		protected virtual string GetDescription() { return null; }
		protected virtual string GetDefaultAction() { return null; } 
		protected virtual BaseAccessible GetSelected() { return null; }
		protected virtual BaseAccessible GetFocused() {
			if(HasChildren) {
				foreach(BaseAccessible accessible in Children) {
					BaseAccessible focused = accessible.GetFocused();
					if(focused != null) return focused;
				}
			}
			if((GetState() & AccessibleStates.Focused) != 0) return this;
			return null; 
		}
		protected virtual string GetKeyboardShortcut() { return null; } 
		public virtual Control GetOwnerControl() { return ParentCore == null ? null : ParentCore.GetOwnerControl(); }
		public virtual bool RaiseQueryAccessibilityHelp(QueryAccessibilityHelpEventArgs e) { return false; }
		public virtual int GetChildCount() { return ChildCount; }
		public virtual AccessibleObject GetChild(int index) { 
			if(index < GetChildCount() && index > -1) return Children[index].Accessible;
			return null; 
		}
		public virtual BaseAccessible GetBaseAccessible(int x, int y) { return GetBaseAccessible(x, y, false); }
		public virtual BaseAccessible GetBaseAccessible(int x, int y, bool recursive) {
			if(HasChildren) {
				foreach(BaseAccessible accessible in Children) {
					if(accessible.IsVisible && accessible.ScreenBounds.Contains(x, y)) return accessible;
				}
				if(recursive) {
					foreach(BaseAccessible accessible in Children) {
						BaseAccessible res = null;
						if(accessible.HasChildren) res = accessible.GetBaseAccessible(x, y, true);
						if(res != null) return res;
					}
				}
			}
			if(IsVisible && ScreenBounds.Contains(x, y)) return this;
			return null;
		}
		public virtual AccessibleObject HitTest(int x, int y) { 
			BaseAccessible res = GetBaseAccessible(x, y);
			if(res != null) return res.Accessible;
			return null;
		}
		public virtual AccessibleObject Navigate(AccessibleNavigation nav) {
			switch(nav) {
				case AccessibleNavigation.FirstChild : 
					if(GetChildCount() > 0) return GetChild(0);
					break;
				case AccessibleNavigation.LastChild : 
					if(GetChildCount() > 0) return GetChild(GetChildCount() - 1);
					break;
				case AccessibleNavigation.Up: 
					return GetSibling(-1);
				case AccessibleNavigation.Down:
					return GetSibling(1);
			}
			return null;
		}
		protected virtual AccessibleObject GetSibling(int delta) {
			if(ParentCore == null || !ParentCore.HasChildren) return null;
			int index = ParentCore.Children.IndexOf(this);
			if(index == -1) return null;
			index = Math.Max(0, Math.Min(ParentCore.ChildCount - 1, index + delta));
			return ParentCore.Children[index].Accessible;
		}
		protected virtual AccessibleStates GetState() { 
			AccessibleStates state = AccessibleStates.None;
			if(!IsVisible) state |= AccessibleStates.Invisible;
			if(GetOwnerControl() != null) {
				if(!GetOwnerControl().Enabled) state |= AccessibleStates.Unavailable;
				if(GetOwnerControl().Focused) state |= AccessibleStates.Focused;
			}
			return state;
		}
		public virtual AccessibleObject Parent { 
			get { 
				if(ParentCore != null) return ParentCore.Accessible;
				if(GetOwnerControl() != null && GetOwnerControl().Parent != null) return GetOwnerControl().Parent.AccessibilityObject; 
				return null;
			}
		}
		AccessibleObject IDXAccessible.GetSelected() {
			BaseAccessible acc = GetSelected();
			if(acc != null) return acc.Accessible;
			return null;
		}
		AccessibleObject IDXAccessible.GetFocused() {
			BaseAccessible acc = GetFocused();
			if(acc != null) return acc.Accessible;
			return null;
		}
		AccessibleStates IDXAccessible.State { get { return GetState(); } }
		public virtual string Value { 
			get { return null; }
			set { }
		}
		const int OBJID_CLIENT = unchecked(unchecked((int)0xFFFFFFFC));
		public virtual void Notify(AccessibleEvents accEvent, int childIndex) {
			NotifyCore(accEvent, GetChildIndex(childIndex));
		}
		protected virtual void NotifyCore(AccessibleEvents accEvent, int index) {
			Control control = GetOwnerControl();
			if(control == null || !control.IsHandleCreated) return;
			NotifyWinEvent((int)accEvent, control.Handle, OBJID_CLIENT, index);
		}
		[DllImport("User32", ExactSpelling = true, CharSet = CharSet.Auto)]
		static extern void NotifyWinEvent(int winEvent, IntPtr hwnd, int objType, int objID);
		public int GetChildIndex(int childIndex) { 
			BaseAccessible baseAcc = ParentCore;
			while(baseAcc != null) {
				childIndex += baseAcc.Children.IndexOf(this);
				baseAcc = baseAcc.ParentCore;
			}
			return childIndex;
		}
	}
	public abstract class BaseAccessibleVirtualCollection : BaseAccessibleCollection {
		Hashtable childrenHash = null;
		public BaseAccessibleVirtualCollection(BaseAccessible owner) : base(owner) { }
		public override BaseAccessible this[int index] { 
			get { 
				return RequestChild(index);
			}
		}
		protected virtual bool AllowHash { get { return true; } }
		protected virtual bool AdvancedHash { get { return true; } }
		public override int Add(BaseAccessible accessible) { return -1; }
		protected Hashtable ChildrenHash { get { return childrenHash; } }
		protected virtual BaseAccessible RequestChild(int index) {
			BaseAccessible res = null;
			if(AllowHash && ChildrenHash != null) res = (BaseAccessible)ChildrenHash[index];
			if(res != null && !AdvancedHash) return res;
			BaseAccessible newChild = CreateChild(index);
			if(newChild != null) {
				if(AllowHash) {
					if(res != null && CompareChild(res, newChild)) return res;
					if(ChildrenHash == null) this.childrenHash = new Hashtable();
					ChildrenHash[index] = newChild;
				}
				OnInsertComplete(index, newChild);
			}
			return newChild;
		}
		protected virtual bool CompareChild(BaseAccessible acc1, BaseAccessible acc2) {
			if(acc1 == acc2) return true;
			if(acc1 == null || acc2 == null) return false;
			return acc1.ClientBounds == acc2.ClientBounds && ((IDXAccessible)acc1).State == ((IDXAccessible)acc2).State;
		}
		int count = 0;
		public virtual void SetCount(int count) {
			count = Math.Max(0, count);
			if(count != this.count) Clear();
			this.count = count;
		}
		protected abstract BaseAccessible CreateChild(int index);
		protected override BaseAccessible Find(AccessibleObject acc) { return null; } 
		public override void Clear() {
			if(this.childrenHash != null) this.childrenHash.Clear();
		}
		public override int Count { get { return count; } }
	}
	public class BaseAccessibleCollection : IEnumerable {
		BaseAccessible owner;
		ArrayList list;
		public BaseAccessibleCollection(BaseAccessible owner) {
			this.owner = owner;
		}
		protected ArrayList List { 
			get {
				if(list == null) list = new ArrayList();
				return list;
			}
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return List.GetEnumerator();
		}
		public BaseAccessible Owner { get { return owner; } }
		public virtual int Add(BaseAccessible accessible) {
			int res = List.Add(accessible);
			OnInsertComplete(res, accessible);
			return res;
		}
		public virtual void Insert(int index, BaseAccessible accessible) { List.Insert(index, accessible); }
		public void AddRange(BaseAccessible[] array) {
			foreach(BaseAccessible accessible in array) Add(accessible);
		}
		public virtual BaseAccessible this[int index] { get { return List[index] as BaseAccessible; } }
		public virtual BaseAccessible this[AccessibleObject acc] {
			get {
				if(acc == null) return null;
				BaseAccessibleObject bc = acc as BaseAccessibleObject;
				BaseAccessible res = null;
				if(bc != null) res = bc.DXOwner as BaseAccessible;
				if(res != null) {
					if(res.ParentCore == owner) return res;
					return null;
				}
				return Find(acc);
			}
		}
		protected virtual BaseAccessible Find(AccessibleObject acc) {
			for(int n = Count - 1; n >= 0; n--) {
				if(this[n].Accessible == acc) return this[n];
			}
			return null;
		}
		protected virtual void OnInsertComplete(int index, object item) {
			BaseAccessible acc = (BaseAccessible)item;
			acc.ParentCore = Owner;
		}
		public virtual int Count { get { return list == null ? 0 : list.Count; } }
		public virtual void Clear() {
			if(list != null) list.Clear();
		}
		public virtual int IndexOf(BaseAccessible accessible) {
			return List.IndexOf(accessible);
		}
	}
	[ComVisible(true)]
	public class BaseAccessibleObject : Control.ControlAccessibleObject, IDisposable {
		IDXAccessible owner;
		public BaseAccessibleObject(IDXAccessible owner) : base(owner.GetOwnerControl()) {
			this.owner = owner;
		}
		public virtual void Dispose() { 
		}
		protected internal IDXAccessible DXOwner { get { return owner; } set { owner = value; } }
		public override string DefaultAction {
			get {
				string defaultAction = DXOwner.DefaultAction;
				return defaultAction != null ? defaultAction : base.DefaultAction;
			}
		}
		public override void DoDefaultAction() {
			DXOwner.DoDefaultAction();
		}
		public override AccessibleRole Role {
			get {
				AccessibleRole role = DXOwner.AccessibleRole;
				return role != AccessibleRole.Default ? role : base.Role;
			}
		}
		public override void Select(AccessibleSelection flags) {
			DXOwner.Select(flags);
		}
		public override Rectangle Bounds { get { return DXOwner.Bounds; } }
		public override int GetChildCount() { return DXOwner.GetChildCount();  }
		public override AccessibleObject GetChild(int index) { return DXOwner.GetChild(index); }
		public override AccessibleObject HitTest(int x, int y) { return DXOwner.HitTest(x, y); }
		public override AccessibleObject Navigate(AccessibleNavigation nav) { return DXOwner.Navigate(nav); }
		public override AccessibleObject Parent { 
			get { 
				if(DXOwner.IsTopControl) return base.Parent;
				return DXOwner.Parent; 
			} 
		}
		public override AccessibleStates State { 
			get { 
				return DXOwner.State; 
			} 
		}
		public override AccessibleObject GetSelected() { return DXOwner.GetSelected(); }
		public override AccessibleObject GetFocused() { return DXOwner.GetFocused(); }
		public override string Value { 
			get { return DXOwner.Value; }
			set { DXOwner.Value = value; }
		}
		public override string Description {
			get {
				string description = DXOwner.AccessibleDescription;
				return description != null ? description : base.Description;
			}
		}
		public override string Help {
			get {
				QueryAccessibilityHelpEventArgs e = new QueryAccessibilityHelpEventArgs();
				if(!DXOwner.RaiseQueryAccessibilityHelp(e)) return base.Help;
				return e.HelpString;
			}
		}	 
		public override string KeyboardShortcut {
			get {
				string res = DXOwner.KeyboardShortcut;
				if(res != null) return res;
				Label previousLabel = PreviousLabel;
				if(GetMnemonic(previousLabel) != (char)0) {
					return "Alt+" + GetMnemonic(previousLabel);
				}
				string baseShortcut = base.KeyboardShortcut;
				if((baseShortcut == null || baseShortcut.Length == 0) && GetMnemonic(GetOwnerControl()) != (char)0) {
					return "Alt+" + GetMnemonic(GetOwnerControl());
				}
				return baseShortcut;
			}
		} 
		protected virtual Control GetOwnerControl() {
			return DXOwner.GetOwnerControl();
		}
		protected bool IsHandleCreated { get { return GetOwnerControl() != null ? GetOwnerControl().IsHandleCreated : false; } }
		public override string Name {
			get {
				string name = DXOwner.AccessibleName;
				if (name != null) {
					return name;
				}
				if(!IsHandleCreated) return null;
				string baseName = base.Name;
				if (baseName == null || baseName.Length == 0) {
					Label previousLabel = PreviousLabel;
					if (previousLabel != null) {
						return previousLabel.AccessibilityObject.Name;
					}
				}
				return baseName;
			}
		}
		private Label PreviousLabel {
			get {
				if (GetOwnerControl().Parent == null) return null;
				ContainerControl c = GetOwnerControl().Parent.GetContainerControl() as ContainerControl;
				if (c != null) {
					Label previous = c.GetNextControl(GetOwnerControl(), false) as Label;
					if (previous != null) return previous;
				}
				return null;
			}
		}
		public override int GetHelpTopic(out string fileName) {
			int topic = 0;
			QueryAccessibilityHelpEventArgs e = new QueryAccessibilityHelpEventArgs();
			if(DXOwner.RaiseQueryAccessibilityHelp(e)) {
				fileName = e.HelpNamespace;							 
				try {
					topic = Int32.Parse(e.HelpKeyword);
				}
				catch { }
				return topic;
			}
			return base.GetHelpTopic(out fileName);
		}
		[DllImport("Kernel32", CharSet=CharSet.Auto, SetLastError=true)]
		static extern IntPtr LoadLibrary(string libname);
		[DllImport("Kernel32", CharSet=CharSet.Auto)]
		static extern bool FreeLibrary(IntPtr hModule);
		[DllImport("User32", ExactSpelling=true, CharSet=CharSet.Auto)]
		static extern void NotifyWinEvent(int winEvent, IntPtr hwnd, int objType, int objID);
		char GetMnemonic(Control control) {
			char mnemonic = (char)0;
			string text = control == null ? null : control.Text;
			if(text == null) return mnemonic;
			for(int i = 0; i < text.Length - 1; i++) {
				if(text[i] == '&' && text[i+1] != '&') {
					mnemonic = Char.ToLower(text[i + 1], System.Globalization.CultureInfo.CurrentCulture);
					break;
				}
			}
			return mnemonic;
		}
	}
	public class BaseControlAccessible : BaseAccessible {
		Control control;
		public BaseControlAccessible(Control control) {
			this.control = control;
			if (control != null) CustomInfo = new AccessibleUserInfoHelper(control);
		}
		protected Control Control { get { return control; } }
		public override Control GetOwnerControl() { return Control; } 
	}
	public class ButtonAccessible : BaseAccessible {
		protected override AccessibleRole GetRole() { return AccessibleRole.PushButton; }
		protected override string GetDefaultAction() { return GetString(AccStringId.ActionPress); }
	}
	public class ScrollBarAccessible : BaseControlAccessible {
		public ScrollBarAccessible(IScrollBar scroll) : base(scroll as Control) {
		}
		protected override ChildrenInfo GetChildrenInfo() { 
			return new ChildrenInfo("Scroll", 5);
		}
		protected override void OnChildrenCountChanged() {
			AddChild(new ScrollChildButton(Scroll, GetIncName(), GetIncDesc(), new GetBoundsDelegate(GetIncButtonBounds), new MethodInvoker(DoInc)));
			AddChild(new ScrollChildButton(Scroll, GetIncAreaName(), GetIncAreaDesc(), new GetBoundsDelegate(GetIncAreaBounds), new MethodInvoker(DoAreaInc)));
			AddChild(new ScrollIndicator(Scroll, GetIndicatorName(), GetIndicatorDesc(), new GetBoundsDelegate(GetIndicatorBounds)));
			AddChild(new ScrollChildButton(Scroll, GetDecAreaName(), GetDecAreaDesc(), new GetBoundsDelegate(GetDecAreaBounds), new MethodInvoker(DoAreaDec)));
			AddChild(new ScrollChildButton(Scroll, GetDecName(), GetDecDesc(), new GetBoundsDelegate(GetDecButtonBounds), new MethodInvoker(DoDec)));
		}
		protected string GetIncName() { return Scroll.ScrollBarType == ScrollBarType.Horizontal ? GetString(AccStringId.NameScrollColumnLeft) : GetString(AccStringId.NameScrollLineUp); }
		protected string GetDecName() { return Scroll.ScrollBarType == ScrollBarType.Horizontal ? GetString(AccStringId.NameScrollColumnRight) : GetString(AccStringId.NameScrollLineDown); }
		protected string GetIncAreaName() { return Scroll.ScrollBarType == ScrollBarType.Horizontal ? GetString(AccStringId.NameScrollAreaLeft) : GetString(AccStringId.NameScrollAreaUp); }
		protected string GetDecAreaName() { return Scroll.ScrollBarType == ScrollBarType.Horizontal ? GetString(AccStringId.NameScrollAreaRight) : GetString(AccStringId.NameScrollAreaDown); }
		protected string GetIndicatorName() { return GetString(AccStringId.NameScrollIndicator); }
		protected string GetIncAreaDesc() { return Scroll.ScrollBarType == ScrollBarType.Horizontal ? GetString(AccStringId.DescScrollAreaLeft) : GetString(AccStringId.DescScrollAreaUp); }
		protected string GetDecAreaDesc() { return Scroll.ScrollBarType == ScrollBarType.Horizontal ? GetString(AccStringId.DescScrollAreaRight) : GetString(AccStringId.DescScrollAreaDown); }
		protected string GetIndicatorDesc() { return Scroll.ScrollBarType == ScrollBarType.Horizontal ? GetString(AccStringId.DescScrollHorzIndicator) : GetString(AccStringId.DescScrollVertIndicator); }
		protected string GetIncDesc() { return Scroll.ScrollBarType == ScrollBarType.Horizontal ? GetString(AccStringId.DescScrollColumnLeft) : GetString(AccStringId.DescScrollLineUp); }
		protected string GetDecDesc() { return Scroll.ScrollBarType == ScrollBarType.Horizontal ? GetString(AccStringId.DescScrollColumnRight) : GetString(AccStringId.DescScrollLineDown); }
		protected Rectangle GetIncButtonBounds() { return Scroll.ViewInfo.VisibleDecButtonBounds; }
		protected Rectangle GetIndicatorBounds() { return Scroll.ViewInfo.ThumbButtonBounds; }
		protected Rectangle GetDecButtonBounds() { return Scroll.ViewInfo.VisibleIncButtonBounds; }
		protected Rectangle GetIncAreaBounds() { return Scroll.ViewInfo.DecAreaBounds; }
		protected Rectangle GetDecAreaBounds() { return Scroll.ViewInfo.IncAreaBounds; }
		protected void DoInc() { Scroll.ChangeValueBasedByState(DevExpress.XtraEditors.ViewInfo.ScrollBarState.DecButtonPressed); }
		protected void DoDec() { Scroll.ChangeValueBasedByState(DevExpress.XtraEditors.ViewInfo.ScrollBarState.IncButtonPressed); }
		protected void DoAreaInc() { Scroll.ChangeValueBasedByState(DevExpress.XtraEditors.ViewInfo.ScrollBarState.DecAreaPressed); }
		protected void DoAreaDec() { Scroll.ChangeValueBasedByState(DevExpress.XtraEditors.ViewInfo.ScrollBarState.IncAreaPressed); }
		protected virtual IScrollBar Scroll { get { return Control as IScrollBar; } }
		protected override AccessibleRole GetRole() { return AccessibleRole.ScrollBar; }
		protected override string GetName() { return GetString(AccStringId.NameScroll); }
		public override string Value { get { return Scroll.Value.ToString(); } }
		protected class ScrollIndicator : ScrollChildButton {
			public ScrollIndicator(IScrollBar scroll, string name, string description, GetBoundsDelegate getBounds) :
				base(scroll, name, description, getBounds, null) { 
			}
			protected override AccessibleRole GetRole() { return AccessibleRole.Indicator; }
			protected override string GetDefaultAction() { return null; }
		}
		protected class ScrollChildButton : ButtonAccessible {
			string name, description;
			IScrollBar scroll;
			GetBoundsDelegate getBounds;
			MethodInvoker doAction;
			public ScrollChildButton(IScrollBar scroll, string name, string description, GetBoundsDelegate getBounds, MethodInvoker doAction) {
				this.scroll = scroll;
				this.name = name;
				this.description = description;
				this.getBounds = getBounds;
				this.doAction = doAction;
			}
			protected IScrollBar Scroll { get { return scroll; } }
			protected override string GetName() { return name; } 
			protected override string GetDescription() { return description; }
			public override Rectangle ClientBounds { get { return getBounds(); } }																				 
			protected override void DoDefaultAction() { 
				if(doAction != null) doAction();
			}
		}
	}
	public interface IAccessibleGridHeaderCell {
		Rectangle Bounds { get; }
		string GetDefaultAction();
		void DoDefaultAction();
		string GetName();
		AccessibleStates GetState();
	}
	public delegate Rectangle GetBoundsDelegate();
	[ToolboxItem(false)]
	public class AccLocalizer : XtraLocalizer<AccStringId> {
		#region static
		static AccLocalizer() {
			SetActiveLocalizerProvider(
					new DefaultActiveLocalizerProvider<AccStringId>(CreateDefaultLocalizer())
				);
		}
		public static XtraLocalizer<AccStringId> CreateDefaultLocalizer() {
			return new EditResAccLocalizer();
		}
		public static string GetString(AccStringId id) {
			return Active.GetLocalizedString(id);
		}
		public new static XtraLocalizer<AccStringId> Active { 
			get { return XtraLocalizer<AccStringId>.Active; }
			set { XtraLocalizer<AccStringId>.Active = value; }
		}
		#endregion static
		public override XtraLocalizer<AccStringId> CreateResXLocalizer() {
			return new EditResAccLocalizer();
		}
		protected override void PopulateStringTable() {
			#region AddString
			AddString(AccStringId.ActionPress, "Press");
			AddString(AccStringId.NameScroll, "scroll bar");
			AddString(AccStringId.NameScrollIndicator, "Position");
			AddString(AccStringId.NameScrollLineUp, "Line Up");
			AddString(AccStringId.NameScrollLineDown, "Line Down");
			AddString(AccStringId.NameScrollColumnLeft, "Column Left");
			AddString(AccStringId.NameScrollColumnRight, "Column Right");
			AddString(AccStringId.NameScrollAreaUp, "Page Up");
			AddString(AccStringId.NameScrollAreaDown, "Page Down");
			AddString(AccStringId.NameScrollAreaLeft, "Page Left");
			AddString(AccStringId.NameScrollAreaRight, "Page Right");
			AddString(AccStringId.DescScrollLineUp, "Moves the vertical position up one line");
			AddString(AccStringId.DescScrollLineDown, "Moves the vertical position down one line");
			AddString(AccStringId.DescScrollAreaUp, "Moves the vertical position up a couple of lines");
			AddString(AccStringId.DescScrollAreaDown, "Moves the vertical position down a couple of lines");
			AddString(AccStringId.DescScrollVertIndicator, "Indicates the current vertical position, and can be dragged to change it directly");
			AddString(AccStringId.DescScrollColumnLeft, "Moves the horizontal position left one column");
			AddString(AccStringId.DescScrollColumnRight, "Moves the horizontal position right one column");
			AddString(AccStringId.DescScrollAreaLeft, "Moves the horizontal position left a couple of columns");
			AddString(AccStringId.DescScrollAreaRight, "Moves the horizontal position right a couple of columns");
			AddString(AccStringId.DescScrollHorzIndicator, "Indicates the current horizontal position, and can be dragged to change it directly");
			AddString(AccStringId.ButtonPush, "Press");
			AddString(AccStringId.ButtonOpen, "Open");
			AddString(AccStringId.ButtonClose, "Close");
			AddString(AccStringId.MouseDoubleClick, "Double Click");
			AddString(AccStringId.OpenKeyboardShortcut, "Alt+Down");
			AddString(AccStringId.CheckEditCheck, "Check");
			AddString(AccStringId.CheckEditUncheck, "Uncheck");
			AddString(AccStringId.TabSwitch, "Switch");
			AddString(AccStringId.SpinBox, "Spinner");
			AddString(AccStringId.SpinUpButton, "Up");
			AddString(AccStringId.SpinDownButton, "Down");
			AddString(AccStringId.SpinLeftButton, "Left");
			AddString(AccStringId.SpinRightButton, "Right");
			AddString(AccStringId.GridNewItemRow, "NewItem Row");
			AddString(AccStringId.GridFilterRow, "Filter Row");
			AddString(AccStringId.GridHeaderPanel, "Header Panel");
			AddString(AccStringId.GridDataPanel, "Data Panel");
			AddString(AccStringId.GridCell, "cell");
			AddString(AccStringId.GridRow, "Row {0}");
			AddString(AccStringId.GridRowExpand, "Expand");
			AddString(AccStringId.GridRowCollapse, "Collapse");
			AddString(AccStringId.GridCardExpand, "Expand");
			AddString(AccStringId.GridCardCollapse, "Collapse");
			AddString(AccStringId.GridRowActivate, "Activate");
			AddString(AccStringId.GridCellEdit, "Edit");
			AddString(AccStringId.GridCellFocus, "Focus");
			AddString(AccStringId.GridDataRowExpand, "Expand detail");
			AddString(AccStringId.GridDataRowCollapse, "Collapse detail");
			AddString(AccStringId.GridColumnSortAscending, "Sort ascending");
			AddString(AccStringId.GridColumnSortDescending, "Sort descending");
			AddString(AccStringId.GridColumnSortNone, "Remove sorting");
			AddString(AccStringId.BarLinkCaption, "Item");
			AddString(AccStringId.BarLinkClick, "Press");
			AddString(AccStringId.BarLinkMenuOpen, "Open");
			AddString(AccStringId.BarLinkMenuClose, "Close");
			AddString(AccStringId.BarLinkStatic, "Static");
			AddString(AccStringId.BarLinkEdit, "Edit");
			AddString(AccStringId.BarDockControlTop, "Dock Top");
			AddString(AccStringId.BarDockControlLeft, "Dock Left");
			AddString(AccStringId.BarDockControlBottom, "Dock Bottom");
			AddString(AccStringId.BarDockControlRight, "Dock Right");
			AddString(AccStringId.NavBarGroupExpand, "Expand");
			AddString(AccStringId.NavBarGroupCollapse, "Collapse");
			AddString(AccStringId.NavBarItemClick, "Press");
			AddString(AccStringId.NavBarScrollUp, "Scroll Up");
			AddString(AccStringId.NavBarScrollDown, "Scroll Down");
			AddString(AccStringId.TreeListNodeCollapse, "Collapse");
			AddString(AccStringId.TreeListNodeExpand, "Expand");
			AddString(AccStringId.TreeListNode, "Node");
			AddString(AccStringId.TreeListNodeCell, "cell");
			AddString(AccStringId.TreeListColumnSortAscending, "Sort ascending"); 
			AddString(AccStringId.TreeListColumnSortDescending, "Sort descending");
			AddString(AccStringId.TreeListColumnSortNone, "Remove sorting");
			AddString(AccStringId.TreeListHeaderPanel, "Header Panel");
			AddString(AccStringId.TreeListDataPanel, "Data Panel");
			AddString(AccStringId.TreeListCellEdit, "Edit");
			AddString(AccStringId.TreelistRowActivate, "Activate"); 
			AddString(AccStringId.ScrollableControlDefaultAction, "Default Action"); 
			AddString(AccStringId.ScrollableControlDescription, "ScrollableControl"); 
			AddString(AccStringId.AboutSupportCenter, "Support Center");
			AddString(AccStringId.AboutChart, "Chat Online");
			AddString(AccStringId.AboutQuestions, "<b>Questions?");
			AddString(AccStringId.AboutSubscription, "You are using {0} of this Subscription.");
			AddString(AccStringId.AboutRegistrationCode, "Registration code");
			AddString(AccStringId.AboutVisit, "Visit our");
			AddString(AccStringId.AboutOr, "or");
			AddString(AccStringId.AboutExpiredVersion, "an expired version");
			AddString(AccStringId.AboutTrialVersion, "an eval version");
			AddString(AccStringId.AboutCopyToClipboard, "Copy to clipboard");
			AddString(AccStringId.AboutSubscribeOnline, "Buy Online");
			AddString(AccStringId.AboutRegisterYourProduct, "Enter Registration Code");
			AddString(AccStringId.AboutCompetitiveDiscounts, "Competitive Discounts");
			#endregion AddString
		}
	}
	public class EditResAccLocalizer : XtraResXLocalizer<AccStringId> {
		public EditResAccLocalizer() : base(new AccLocalizer()) {}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Utils.LocalizationRes", typeof(EditResAccLocalizer).Assembly);
		}
	}
	#region Enum
	public enum AccStringId {
		ActionPress,
		NameScroll,
		NameScrollIndicator,
		NameScrollLineUp,
		NameScrollLineDown,
		NameScrollColumnLeft,
		NameScrollColumnRight,
		NameScrollAreaUp,
		NameScrollAreaDown,
		NameScrollAreaLeft,
		NameScrollAreaRight,
		DescScrollLineUp,
		DescScrollLineDown,
		DescScrollAreaUp,
		DescScrollAreaDown,
		DescScrollVertIndicator,
		DescScrollColumnLeft,
		DescScrollColumnRight,
		DescScrollAreaLeft,
		DescScrollAreaRight,
		DescScrollHorzIndicator,
		ButtonPush,
		ButtonOpen,
		ButtonClose,
		MouseDoubleClick,
		OpenKeyboardShortcut,
		CheckEditCheck,
		CheckEditUncheck,
		TabSwitch,
		SpinBox,
		SpinUpButton,
		SpinDownButton,
		SpinLeftButton,
		SpinRightButton,
		GridNewItemRow,
		GridFilterRow,
		GridHeaderPanel,
		GridDataPanel,
		GridCell,
		GridRow,
		GridRowExpand,
		GridRowCollapse,
		GridCardExpand,
		GridCardCollapse,
		GridRowActivate,
		GridCellEdit,
		GridCellFocus,
		GridDataRowExpand,
		GridDataRowCollapse,
		GridColumnSortAscending,
		GridColumnSortDescending,
		GridColumnSortNone,
		BarLinkCaption,
		BarLinkClick,
		BarLinkMenuOpen,
		BarLinkMenuClose,
		BarLinkStatic,
		BarLinkEdit,
		BarDockControlTop,
		BarDockControlLeft,
		BarDockControlBottom,
		BarDockControlRight,
		NavBarGroupExpand,
		NavBarGroupCollapse,
		NavBarItemClick,
		NavBarScrollUp,
		NavBarScrollDown,
		TreeListNodeExpand,
		TreeListNodeCollapse,
		TreeListNode,
		TreeListNodeCell,
		TreeListColumnSortAscending,
		TreeListColumnSortDescending,
		TreeListColumnSortNone,
		TreeListHeaderPanel,
		TreeListDataPanel,
		TreeListCellEdit,
		TreelistRowActivate,
		ScrollableControlDescription,
		ScrollableControlDefaultAction,
		AboutSupportCenter,
		AboutChart,
		AboutQuestions,
		AboutSubscription, 
		AboutSubscriptionExp,
		AboutRegistrationCode,
		AboutVisit, 
		AboutOr, 
		AboutExpiredVersion,
		AboutTrialVersion,
		AboutCopyToClipboard,
		AboutSubscribeOnline, 
		AboutRegisterYourProduct, 
		AboutCompetitiveDiscounts
	}
	#endregion
}
