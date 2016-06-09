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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Skins;
using DevExpress.XtraTab.Registrator;
using DevExpress.XtraLayout.Painting;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraDashboardLayout;
namespace DevExpress.XtraLayout.Registrator {
	public class LayoutPaintStyleCollection : CollectionBase {
		public int Add(LayoutPaintStyle style) { return List.Add(style); }
		public LayoutPaintStyle this[int index] { get { return List[index] as LayoutPaintStyle; } }
		public LayoutPaintStyle this[string name] {
			get {
				for(int n = 0; n < Count; n++) {
					if(this[n].PaintStyleName == name) return this[n];
				}
				return null;
			}
		}
		public LayoutPaintStyle GetPaintStyle(string paintStyleName, UserLookAndFeel lf) {
			if(paintStyleName == "Default") return this[lf];
			LayoutPaintStyle style = this[paintStyleName];
			if(style == null || !style.IsValid) style = this[0];
			return style;
		}
		public LayoutPaintStyle this[UserLookAndFeel lookAndFeel] { 
			get {
				if(lookAndFeel != null) {
					string name = "Flat";
					switch(lookAndFeel.ActiveStyle) {
						case ActiveLookAndFeelStyle.Flat: name = "Flat"; break;
						case ActiveLookAndFeelStyle.Style3D: name = "Style3D"; break;
						case ActiveLookAndFeelStyle.UltraFlat: name = "UltraFlat"; break;
						case ActiveLookAndFeelStyle.Skin : name = "Skin"; break;
						case ActiveLookAndFeelStyle.WindowsXP : name = "WindowsXP"; break;
						case ActiveLookAndFeelStyle.Office2003 : name = "Office2003"; break;
					}
					LayoutPaintStyle style = this[name];
					if(Count > 0 &&( style == null || !style.IsValid)) style = this[0];
					if(style == null) return LayoutPaintStyle.NullStyle;
					return style;
				}
				else
					throw new NullReferenceException("invalid argument");
			}
		}
	}
	public class LayoutPaintStyle {
		ISupportLookAndFeel owner;
		UserLookAndFeel lookAndFeel=null;
		public LayoutPaintStyle(ISupportLookAndFeel owner) {
			this.owner = owner;
		}
		[ThreadStatic]
		static LayoutPaintStyle nullStyle;
		public static LayoutPaintStyle NullStyle {
			get { 
				if(nullStyle == null) nullStyle = new LayoutPaintStyle(null);
				return nullStyle;
			}
		}
		public static void Reset() {
			nullStyle = null;
		}
		public void Destroy() {
			if(lookAndFeel != null) lookAndFeel.Dispose();
		}
		protected ISupportLookAndFeel Owner { get { return owner; } }
		public virtual string PaintStyleName { get { return "Flat"; } }
		public UserLookAndFeel LookAndFeel { 
			get { 
				if(lookAndFeel == null) lookAndFeel = CreateLookAndFeel();
				return lookAndFeel; 
			} 
		}
		public virtual bool IsValid { get { return true; } }
		public virtual BaseViewInfoRegistrator CreateTabInfo() {
			return DevExpress.XtraTab.Registrator.PaintStyleCollection.DefaultPaintStyles.GetView(LookAndFeel, BaseViewInfoRegistrator.DefaultViewName);
		}
		public virtual GroupObjectPainter CreateGroupPainter(IPanelControlOwner owner) {
			return new LCFlatGroupObjectPainter(owner);
		}
		protected LayoutControlItemSkinPainter layoutControlItemSkinPainter=null;
		protected LayoutControlItemPainter  layoutControlItemPainter=null;
		protected LayoutControlGroupPainter layoutControlGroupPainter=null;
		protected DashboardLayoutControlItemPainter dashboardLayoutItemPainter = null;
		protected DashboardLayoutControlItemInGroupPainter dashboardLayoutItemInGroupPainter = null;
		protected DashboardGroupPainter dashboardGroupPainter = null;
		protected TabbedGroupPainter tabbedGroupPainter = null;
		protected SplitterItemPainter splitterItemPainter=null;
		protected EmptySpaceItemPainter emptySpaceItemPainter = null;
		protected EmptySpaceItemPainter simpleSeparatorItemPainter = null;
		protected LayoutRepositoryItemPainter repositoryItemPainter = null;
		protected LayoutRepositoryItemSkinPainter repositoryItemSkinPainter = null;
		public virtual LayoutControlItemSkinPainter GetControlItemSkinPainter() {
			if(layoutControlItemSkinPainter==null) {
				layoutControlItemSkinPainter = new LayoutControlItemSkinPainter();
			}
			return layoutControlItemSkinPainter;
		}
		public virtual LayoutRepositoryItemPainter GetRepositoryItemPainter() {
			if(repositoryItemPainter==null) {
				repositoryItemPainter = new LayoutRepositoryItemPainter();
			}
			return repositoryItemPainter;
		}
		public virtual LayoutRepositoryItemSkinPainter GetRepositoryItemSkinPainter() {
			if(repositoryItemSkinPainter==null) {
				repositoryItemSkinPainter = new LayoutRepositoryItemSkinPainter();
			}
			return repositoryItemSkinPainter;
		}
		public virtual LayoutControlItemPainter GetControlItemPainter() {
			if(layoutControlItemPainter==null) {
				layoutControlItemPainter = new LayoutControlItemPainter();
			}
			return layoutControlItemPainter;
		}
			public virtual DashboardLayoutControlItemInGroupPainter GetDashboardLayoutItemInGroupPainter() {
				if(dashboardLayoutItemInGroupPainter == null) {
					dashboardLayoutItemInGroupPainter = new DashboardLayoutControlItemInGroupPainter();
			}
				return dashboardLayoutItemInGroupPainter;
		}
		public virtual DashboardLayoutControlItemPainter GetDashboardLayoutItemPainter() {
			if(dashboardLayoutItemPainter == null) {
				dashboardLayoutItemPainter = new DashboardLayoutControlItemPainter();
			}
			return dashboardLayoutItemPainter;
		}
		public virtual DashboardGroupPainter GetDashboardLayoutGroupPainter() {
			if(dashboardGroupPainter == null) {
				dashboardGroupPainter = new DashboardGroupPainter();
			}
			return dashboardGroupPainter;
		}
		public virtual LayoutControlGroupPainter GetControlGroupPainter() {
			if(layoutControlGroupPainter==null) {
				layoutControlGroupPainter  =new LayoutControlGroupPainter();
			}
			return layoutControlGroupPainter;
		}
		public virtual TabbedGroupPainter GetTabbedGroupPainter() {
			if(tabbedGroupPainter==null) {
				tabbedGroupPainter = new TabbedGroupPainter();
			}
			return tabbedGroupPainter;
		}
		public virtual SplitterItemPainter GetSplitterItemPainter() {
			if(splitterItemPainter==null) {
				splitterItemPainter = new SplitterItemPainter(LookAndFeel);
			}
			return splitterItemPainter;
		}
		public virtual EmptySpaceItemPainter GetEmptySpaceItemPainter() {
			if(emptySpaceItemPainter==null) {
				emptySpaceItemPainter = new EmptySpaceItemPainter();
			}
			return emptySpaceItemPainter;
		}
		protected virtual EmptySpaceItemPainter GetSimpleSeparatorItemPainter() {
			if(simpleSeparatorItemPainter==null) {
				simpleSeparatorItemPainter = new SkinSimpleSeparatorItemPainter();
			}
			return simpleSeparatorItemPainter;
		}
		public virtual BaseLayoutItemPainter GetPainter(BaseLayoutItem item) {
			LayoutClassificationArgs arg = LayoutClassifier.Default.Classify(item);
			if(arg.IsTabbedGroup) {
				return GetTabbedGroupPainter();
			}
			else if(arg.IsDashboardItem) {
				if((item as DashboardLayoutControlItemBase).ViewMode == DashboardViewMode.LayoutGroup) return GetDashboardLayoutItemPainter();
				else return GetDashboardLayoutItemInGroupPainter();
			} else if(arg.IsDashboardGroup) {
				return GetDashboardLayoutGroupPainter();
			} else if(arg.IsGroup) {
				return GetControlGroupPainter();
			} else if(arg.IsRepositoryItem) {
				switch(arg.RepositoryItem.GetPaintingType()) {
					case PaintingType.Normal: return GetRepositoryItemPainter();
					case PaintingType.Skinned: return GetRepositoryItemSkinPainter();
					case PaintingType.XP: return GetRepositoryItemPainter();
				}
				return GetRepositoryItemPainter();
			} else if(arg.IsSplitterItem) {
				return GetSplitterItemPainter();
			} else if(arg.IsEmptySpaceItem) {
				if(item is SimpleSeparator) return GetSimpleSeparatorItemPainter();
				else return GetEmptySpaceItemPainter();
			} else {
				LayoutControlItem controlItem = item as LayoutControlItem;
				if(controlItem!=null) {
					switch(controlItem.GetPaintingType()) {
						case PaintingType.Normal: return GetControlItemPainter();
						case PaintingType.Skinned: return GetControlItemSkinPainter();
						case PaintingType.XP: return GetControlItemPainter();
					}
				}
				return GetControlItemPainter();
			}
		}
		protected virtual UserLookAndFeel CreateLookAndFeel() {
			UserLookAndFeel lf = new EmbeddedLookAndFeel();
			lf.SetFlatStyle();
			return lf;
		}
		public virtual AppearanceDefault AppearanceCaption {
			get {
				return new AppearanceDefault(LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.WindowText), Color.Transparent, HorzAlignment.Near);
			}
		}
	}
	public class LayoutOffice2003PaintStyle : LayoutPaintStyle {
		public LayoutOffice2003PaintStyle(ISupportLookAndFeel owner) : base(owner) { }
		public override string PaintStyleName { get { return "Office2003"; } }
		public override GroupObjectPainter CreateGroupPainter(IPanelControlOwner owner) {
			return new GroupObjectPainter(owner);
		}
		protected override UserLookAndFeel CreateLookAndFeel() {
			UserLookAndFeel lf = new EmbeddedLookAndFeel();
			lf.SetOffice2003Style();
			return lf;
		}
	}
	public class Style3DPaintStyle : LayoutPaintStyle {
		public Style3DPaintStyle(ISupportLookAndFeel owner) : base(owner) { }
		public override string PaintStyleName { get { return "Style3D"; } }
		public override GroupObjectPainter CreateGroupPainter(IPanelControlOwner owner) {
			return new LCFlatGroupObjectPainter(owner);
		}
		protected override UserLookAndFeel CreateLookAndFeel() {
			UserLookAndFeel lf = new EmbeddedLookAndFeel();
			lf.SetStyle3D();
			return lf;
		}
	}
	public class UltraFlatPaintStyle : LayoutPaintStyle {
		public UltraFlatPaintStyle(ISupportLookAndFeel owner) : base(owner) { }
		public override string PaintStyleName { get { return "UltraFlat"; } }
		public override GroupObjectPainter CreateGroupPainter(IPanelControlOwner owner) {
			return new LCFlatGroupObjectPainter(owner);
		}
		protected override UserLookAndFeel CreateLookAndFeel() {
			UserLookAndFeel lf = new EmbeddedLookAndFeel();
			lf.SetUltraFlatStyle();
			return lf;
		}
	}
	public class FlatPaintStyle : LayoutPaintStyle {
		public FlatPaintStyle(ISupportLookAndFeel owner) : base(owner) { }
		public override string PaintStyleName { get { return "Flat"; } }
		public override GroupObjectPainter CreateGroupPainter(IPanelControlOwner owner) {
			return new LCFlatGroupObjectPainter(owner);
		}
		protected override UserLookAndFeel CreateLookAndFeel() {
			UserLookAndFeel lf = new EmbeddedLookAndFeel();
			lf.SetFlatStyle();
			return lf;
		}
		protected override EmptySpaceItemPainter GetSimpleSeparatorItemPainter() {
			return new FlatSimpleSeparatorItemPainter();
		}
	}
	public class LayoutSkinPaintStyle : LayoutPaintStyle {
		public LayoutSkinPaintStyle(ISupportLookAndFeel owner) : base(owner) { }
		public override string PaintStyleName { get { return "Skin"; } }
		public override GroupObjectPainter CreateGroupPainter(IPanelControlOwner owner) {
			return new SkinGroupObjectPainter(owner, LookAndFeel);
		}
		protected override UserLookAndFeel CreateLookAndFeel() {
			UserLookAndFeel lf = new SkinEmbeddedLookAndFeel(Owner.LookAndFeel.ActiveLookAndFeel); 
			return lf;
		}
	}
	public class LayoutWindowsXPPaintStyle : LayoutPaintStyle {
		public LayoutWindowsXPPaintStyle(ISupportLookAndFeel owner) : base(owner) { }
		public override bool IsValid { get { return DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled; } }
		public override string PaintStyleName { get { return "WindowsXP"; } }
		public override GroupObjectPainter CreateGroupPainter(IPanelControlOwner owner) {
			return new WindowsXPGroupObjectPainter(owner);
		}
		protected override UserLookAndFeel CreateLookAndFeel() {
			UserLookAndFeel lf = new EmbeddedLookAndFeel();
			lf.SetWindowsXPStyle();
			return lf;
		}
	}
}
