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
using System.Drawing.Design;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Forms;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Objects;
using DevExpress.Utils.Serializing;
using DevExpress.Utils;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraBars.Helpers.Docking;
namespace DevExpress.XtraBars {
	[ToolboxItem(false), Obsolete()]
	public class BarDockWindow : BarDockWindowCore, ISupportInitialize {
		public virtual void BeginInit() { }
		public virtual void EndInit() { }
	}
	[ToolboxItem(false)]
	public class BarDockWindowCore : Panel {
		BarDockInfo dockInfo;
		Guid groupId;
		BarDockWindowCollection innerWindows;
		BarManager manager;
		int initializing, lockUpdate, selectedInnerWindow, lockUpdateInnerWindow, suspendDocking;
		string dockCaption, tabCaption;
		int tabImageIndex;
		Image tabImage;
		BarCanDockStyle canDockStyle;
		bool dockWindowVisible, useWholeRow, stretchWhenDocked, hidden, showCloseButton;
		Point floatMousePosition;
		internal Point floatLocation;
		internal Size dockedSize, floatSize;
		public const int MinimumWidth = 40, MinimumHeight = 40;
		PopupControl dropDownControl;
		Color dockedBorderColor;
		BarItemBorderStyle dockedBorderStyle;
		static Size defaultSize = new Size(200, 100);
		public event EventHandler DockWindowVisibleChanged;
		public BarDockWindowCore() {
			if(DockWindowVisibleChanged != null)  
				DockWindowVisibleChanged = null;
			this.dockInfo = new BarDockInfo(BarDockStyle.None, 0, 0, 0);
			groupId = Guid.Empty;
			innerWindows = new BarDockWindowCollection();
			selectedInnerWindow = 0;
			dockedBorderStyle = BarItemBorderStyle.Single;
			dockedBorderColor = SystemColors.ControlDark;
			showCloseButton = true;
			dropDownControl = null;
			canDockStyle = BarCanDockStyle.Left | BarCanDockStyle.Right | BarCanDockStyle.Bottom | BarCanDockStyle.Floating;
			dockWindowVisible = true;
			stretchWhenDocked = true;
			hidden = false;
			tabCaption = "";
			dockCaption = "DockWindow";
			tabImageIndex = -1;
			tabImage = null;
			floatSize = dockedSize = defaultSize;
			floatMousePosition = floatLocation = Point.Empty;
			useWholeRow = true;
			manager = null;
			suspendDocking = lockUpdateInnerWindow = lockUpdate = initializing = 0;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				Manager = null;
			}
			base.Dispose(disposing);
		}
		[Category("DockWindow"), Browsable(false), DefaultValue(""), XtraSerializableProperty()]
		public new virtual string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		internal bool ShouldSerializeGroupId() { return !GroupId.Equals(Guid.Empty); }
		[Browsable(false), XtraSerializableProperty()]
		public virtual Guid GroupId { get { return groupId; } set { groupId = value; } }
		[Category("DockWindow"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.NameCollection, false, true, true)]
		public virtual BarDockWindowCollection InnerWindows { get { return innerWindows; } }
		[Category("DockWindow"), Browsable(false), DefaultValue(0), XtraSerializableProperty()]
		public virtual int SelectedInnerWindowIndex {
			get { return (InnerWindows.Count == 0 || selectedInnerWindow < 0)? 0 : selectedInnerWindow; 
			}
			set {
				if(value < 0) value = 0;
				selectedInnerWindow = value;
			}
		}
		[ Category("DockWindow"), DefaultValue(true), XtraSerializableProperty()]
		public virtual bool StretchWhenDocked { 
			get { return stretchWhenDocked; }
			set { 
				if(StretchWhenDocked == value) return;
				stretchWhenDocked = value;
			}
		}
		[ Category("DockWindow"), DefaultValue(BarItemBorderStyle.Single), XtraSerializableProperty()]
		public virtual BarItemBorderStyle DockedBorderStyle { 
			get { return dockedBorderStyle; }
			set { 
				if(DockedBorderStyle == value) return;
				dockedBorderStyle = value;
			}
		}
		[ Category("DockWindow"), DefaultValue(BarItemBorderStyle.Single), XtraSerializableProperty()]
		public virtual Color DockedBorderColor { 
			get { return dockedBorderColor; }
			set { dockedBorderColor = value; }
		}
		[ Category("DockWindow"), DefaultValue(false)]
		public virtual bool Hidden { 
			get { return hidden; }
			set {  hidden = value; }
		}
		[ Category("DockWindow"), DefaultValue(""), XtraSerializableProperty()]
		public virtual string TabCaption { 
			get { return tabCaption; }
			set { tabCaption = value; }
		}
		[ Category("DockWindow"), DefaultValue(-1), XtraSerializableProperty(),
		Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(UITypeEditor)),
		DevExpress.Utils.ImageList("Images")]
		public virtual int TabImageIndex { 
			get { return tabImageIndex; }
			set { tabImageIndex = value; }
		}
		[ Category("DockWindow"), DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image TabImage { 
			get { return tabImage; }
			set { tabImage = value; }
		}
		[ Category("DockWindow"), DefaultValue("DockWindow"), XtraSerializableProperty(), Localizable(true)]
		public virtual string DockCaption { 
			get { return dockCaption; }
			set { dockCaption = value; }
		}
		[ Category("DockWindow"), DefaultValue(true), XtraSerializableProperty()]
		public virtual bool UseWholeRow { 
			get { return useWholeRow; }
			set { useWholeRow = value; }
		}
		[ Category("DockWindow"), DefaultValue(true), XtraSerializableProperty()]
		public virtual bool DockWindowVisible { 
			get { return dockWindowVisible; }
			set {  dockWindowVisible = value; }
		}
		public virtual Size DockedSize {
			get { return dockedSize; }
			set {
				if(value.Width < MinimumWidth) value.Width = MinimumWidth;
				if(value.Height < MinimumHeight) value.Height = MinimumHeight;
				if(value == DockedSize) return;
				dockedSize = value;
			}
		}
		public virtual Size FloatSize {
			get { return floatSize; }
			set {
				if(value.Width < MinimumWidth) value.Width = MinimumWidth;
				if(value.Height < MinimumHeight) value.Height = MinimumHeight;
				floatSize = value;
			}
		}
		public virtual Point FloatLocation { 
			get { return floatLocation; }
			set {floatLocation = value; }
		}
		public BarDockInfo DockInfo { get { return dockInfo; } set { dockInfo = value; }}
		[Category("DockWindow"), DefaultValue(0), Browsable(false),  XtraSerializableProperty()]
		public virtual int Offset { 
			get { return DockInfo.Offset; }
			set { DockInfo.Offset = value; 
			}
		}
		[Category("DockWindow"), DefaultValue(-1), Browsable(false),  XtraSerializableProperty()]
		public virtual int DockCol { 
			get { return DockInfo.DockCol; }
			set { 
				DockInfo.DockCol = value;
			} 
		}
		[Category("DockWindow"), DefaultValue(-1), Browsable(false),  XtraSerializableProperty()]
		public virtual int DockRow { 
			get { 
				int res = DockInfo.DockRow;
				if(IsDesignMode && DockStyle == BarDockStyle.Bottom) res --;
				return Math.Max(-1, res);
			}
			set { 
				DockInfo.DockRow = value;
			}
		}
		[ Category("DockWindow"), DefaultValue(null)]
		public BarManager Manager { 
			get { return manager; }
			set {
				if(Manager == value) return;
				ClearManager();
				manager = value;
				if(Manager != null) {
					Manager.dockWindows.Add(this);
				}
			}
		}
		internal void ClearManager() {
			BarManager man = Manager;
			if(man == null) return;
			manager = null;
			man.dockWindows.Remove(this);
			Dispose();
		}
		[ Category("DockWindow"), DefaultValue(BarCanDockStyle.Left | BarCanDockStyle.Right | BarCanDockStyle.Bottom | BarCanDockStyle.Floating), System.ComponentModel.Editor(typeof(DevExpress.Utils.Editors.AttributesEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public BarCanDockStyle CanDockStyle { 
			get { return canDockStyle; }
			set { canDockStyle = value; }
		}
		[ Category("DockWindow"), DefaultValue(BarDockStyle.None),  XtraSerializableProperty()]
		public BarDockStyle DockStyle {
			get { return DockInfo.DockStyle; }
			set {
				if(DockStyle == value) return;
				DockInfo.DockStyle = value;
				DockInfo = new BarDockInfo(value, DockInfo.DockRow, DockInfo.DockCol, DockInfo.Offset);
			}
		}
		[ Category("DockWindow"), DefaultValue(null)]
		public PopupControl DropDownControl {
			get { return dropDownControl; }
			set { dropDownControl = value; }
		}
		[ Category("DockWindow"), DefaultValue(true)]
		public bool ShowCloseButton {
			get { return showCloseButton; }
			set { showCloseButton = value; }
		}
		public virtual void LayoutChanged() { }
		public override string ToString() { return DockCaption; } 
		public virtual void ShowDockWindow() { }
		public void HideDockWindows() { }
		public virtual void HideDockWindow() { }
		public virtual bool IsDesignMode {
			get { return DesignMode; }
		}
		public virtual bool IsLoading {
			get { return initializing != 0; }
		}
	}	
	public class BarDockWindowCollection : ArrayList {	}
}
