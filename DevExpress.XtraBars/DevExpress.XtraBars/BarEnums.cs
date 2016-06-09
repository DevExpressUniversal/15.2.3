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
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Controls;
using DevExpress.XtraBars;
namespace DevExpress.XtraBars {
	public enum MenuDrawMode { Default, SmallImagesText, LargeImagesText, LargeImagesTextDescription }
	public enum BarItemLinkAlignment { Default, Left, Right }
	public enum AnimationType { System, None, Slide, Fade, Unfold, Random };
	[Flags]
	public enum BarOptionFlags {
		None = 0,
		AllowQuickCustomization = 1, 
		IsMainMenu = 2,
		MultiLine = 4,
		RotateWhenVertical = 8,
		Visible = 16,
		UseWholeRow = 32,
		DrawDragBorder = 64,
		Hidden = 128,
		DisableClose = 256,
		IsStatusBar = 512,
		DrawSizeGrip = 1024,
		DisableCustomization = 2048,
		AllowDelete = 4096
	}
	public enum BarMdiMenuMergeStyle {
		Always,
		WhenChildActivated,
		OnlyWhenChildMaximized,
		Never
	}
	public enum BarMenuMerge {
		Add,
		Replace,
		MergeItems,
		Remove
	}
	public enum BarStaticItemSize {
		None, Content, Spring
	}
	public enum BarDockStyle {
		None = 0, Left = 1, Top = 2, Right = 3, Bottom = 4, Standalone
	}
	[Flags]
	public enum BarCanDockStyle {
		Floating = 1, Left = 2, Top = 4, Right = 8, Bottom = 16, Standalone = 32, All = 1 | 2 | 4 | 8 | 16 | 32
	}
	public enum BarItemPaintStyle {
		Standard, Caption, CaptionInMenu, CaptionGlyph
	}
	public enum BarButtonStyle {
		Default, DropDown, Check, CheckDropDown
	}
	public enum BarItemBorderStyle {
		None, Single, Lowered, Raised
	}
	public enum BarItemCaptionAlignment {
		Left, Top, Right, Bottom
	}
	public enum BarItemVisibility {
		Always, Never, OnlyInCustomizing, OnlyInRuntime
	}
	public enum BarLinkProperty { Caption, ImageIndex, Visibility, Width, Enabled }
	public enum BarAutoPopupMode { Default, All, None, OnlyMenu }
	public class BarOptions : BaseBarManagerOptions {
		bool allowDelete, allowRename, allowQuickCustomization, disableClose, disableCustomization, 
			drawDragBorder, drawSizeGrip, hidden, multiLine, rotateWhenVertical, 
			useWholeRow, allowCollapse, allowCaptionWhenFloating, 
			drawBorder;
		BarState barState;
		BarAutoPopupMode autoPopupMode;
		int minHeight;
		public BarOptions() {
			this.allowDelete = false;
			this.allowRename = false;
			this.allowQuickCustomization = true;
			this.disableClose = false;
			this.disableCustomization = false;
			this.drawDragBorder = true;
			this.drawSizeGrip = false;
			this.hidden = false;
			this.multiLine = false;
			this.rotateWhenVertical = true;
			this.useWholeRow = false;
			this.allowCollapse = false;
			this.barState = BarState.Expanded;
			this.allowCaptionWhenFloating = true;
			this.drawBorder = true;
			this.autoPopupMode = BarAutoPopupMode.Default;
			this.minHeight = 0;
		}
		internal bool AllowCaptionWhenFloating {
			get { return allowCaptionWhenFloating; }
			set { allowCaptionWhenFloating = value; }
		}
		[XtraSerializableProperty, DefaultValue(0)]
		public virtual int MinHeight {
			get { return minHeight; }
			set {
				int prevValue = value;
				minHeight = value;
				OnChanged(new BaseOptionChangedEventArgs("MinHeight", prevValue, MinHeight));
			}
		}
		[XtraSerializableProperty(),  DefaultValue(BarAutoPopupMode.Default)]
		public virtual BarAutoPopupMode AutoPopupMode {
			get { return autoPopupMode; }
			set {
				if(AutoPopupMode == value)
					return;
				BarAutoPopupMode prevValue = AutoPopupMode;
				autoPopupMode = value;
				OnChanged(new BaseOptionChangedEventArgs("AutoPopupMode", prevValue, AutoPopupMode));
			}
		}
		[XtraSerializableProperty(), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarOptionsAllowCollapse"),
#endif
 DefaultValue(true)]
		public virtual bool DrawBorder {
			get { return drawBorder; }
			set {
				if(DrawBorder == value)
					return;
				bool prevValue = DrawBorder;
				drawBorder = value;
				OnChanged(new BaseOptionChangedEventArgs("DrawBorder", prevValue, DrawBorder));
			}
		}
		[XtraSerializableProperty(), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarOptionsAllowCollapse"),
#endif
 DefaultValue(false)]
		public virtual bool AllowCollapse {
			get { return allowCollapse; }
			set {
				if(AllowCollapse == value) return;
				bool prevValue = AllowCollapse;
				allowCollapse = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowCollapse", prevValue, AllowCollapse));
			}
		}
		[XtraSerializableProperty(), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarOptionsBarState"),
#endif
 DefaultValue(BarState.Expanded)]
		public virtual BarState BarState {
			get { return barState; }
			set {
				if(UseWholeRow) value = BarState.Expanded;
				if(BarState == value || !AllowCollapse) return;
				BarState prevValue = BarState;
				barState = value;
				OnChanged(new BaseOptionChangedEventArgs("BarState", prevValue, BarState));
			}
		}
		[XtraSerializableProperty(), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarOptionsAllowDelete"),
#endif
 DefaultValue(false)]
		public virtual bool AllowDelete {
			get { return allowDelete; }
			set {
				if(AllowDelete == value) return;
				bool prevValue = AllowDelete;
				allowDelete = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowDelete", prevValue, AllowDelete));
			}
		}
		[XtraSerializableProperty(), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarOptionsAllowRename"),
#endif
 DefaultValue(false)]
		public virtual bool AllowRename {
			get { return allowRename; }
			set {
				if(AllowRename == value) return;
				bool prevValue = AllowRename;
				allowRename = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowRename", prevValue, AllowRename));
			}
		}
		[XtraSerializableProperty(), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarOptionsAllowQuickCustomization"),
#endif
 DefaultValue(true)]
		public virtual bool AllowQuickCustomization {
			get { return allowQuickCustomization; }
			set {
				if(AllowQuickCustomization == value) return;
				bool prevValue = AllowQuickCustomization;
				allowQuickCustomization = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowQuickCustomization", prevValue, AllowQuickCustomization));
			}
		}
		[XtraSerializableProperty(), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarOptionsDisableClose"),
#endif
 DefaultValue(false)]
		public virtual bool DisableClose {
			get { return disableClose; }
			set {
				if(DisableClose == value) return;
				bool prevValue = DisableClose;
				disableClose = value;
				OnChanged(new BaseOptionChangedEventArgs("DisableClose", prevValue, DisableClose));
			}
		}
		[XtraSerializableProperty(), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarOptionsDisableCustomization"),
#endif
 DefaultValue(false)]
		public virtual bool DisableCustomization {
			get { return disableCustomization; }
			set {
				if(DisableCustomization == value) return;
				bool prevValue = DisableCustomization;
				disableCustomization = value;
				OnChanged(new BaseOptionChangedEventArgs("DisableCustomization", prevValue, DisableCustomization));
			}
		}
		[XtraSerializableProperty(), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarOptionsDrawDragBorder"),
#endif
 DefaultValue(true)]
		public virtual bool DrawDragBorder {
			get { return drawDragBorder; }
			set {
				if(DrawDragBorder == value) return;
				bool prevValue = DrawDragBorder;
				drawDragBorder = value;
				OnChanged(new BaseOptionChangedEventArgs("DrawDragBorder", prevValue, DrawDragBorder));
			}
		}
		[XtraSerializableProperty(), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarOptionsDrawSizeGrip"),
#endif
 DefaultValue(false)]
		public virtual bool DrawSizeGrip {
			get { return drawSizeGrip; }
			set {
				if(DrawSizeGrip == value) return;
				bool prevValue = DrawSizeGrip;
				drawSizeGrip = value;
				OnChanged(new BaseOptionChangedEventArgs("DrawSizeGrip", prevValue, DrawSizeGrip));
			}
		}
		[XtraSerializableProperty(), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarOptionsHidden"),
#endif
 DefaultValue(false)]
		public virtual bool Hidden {
			get { return hidden; }
			set {
				if(Hidden == value) return;
				bool prevValue = Hidden;
				hidden = value;
				OnChanged(new BaseOptionChangedEventArgs("Hidden", prevValue, Hidden));
			}
		}
		[XtraSerializableProperty(), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarOptionsMultiLine"),
#endif
 DefaultValue(false)]
		public virtual bool MultiLine {
			get { return multiLine; }
			set {
				if(MultiLine == value) return;
				bool prevValue = MultiLine;
				multiLine = value;
				OnChanged(new BaseOptionChangedEventArgs("MultiLine", prevValue, MultiLine));
			}
		}
		[XtraSerializableProperty(), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarOptionsRotateWhenVertical"),
#endif
 DefaultValue(true)]
		public virtual bool RotateWhenVertical {
			get { return rotateWhenVertical; }
			set {
				if(RotateWhenVertical == value) return;
				bool prevValue = RotateWhenVertical;
				rotateWhenVertical = value;
				OnChanged(new BaseOptionChangedEventArgs("RotateWhenVertical", prevValue, RotateWhenVertical));
			}
		}
		[XtraSerializableProperty(), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarOptionsUseWholeRow"),
#endif
 DefaultValue(false)]
		public virtual bool UseWholeRow {
			get { return useWholeRow; }
			set {
				if(UseWholeRow == value) return;
				bool prevValue = UseWholeRow;
				useWholeRow = value;
				BarState = BarState.Expanded;
				OnChanged(new BaseOptionChangedEventArgs("UseWholeRow", prevValue, UseWholeRow));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				BarOptions opt = options as BarOptions;
				if(opt == null) return;
				this.allowDelete = opt.AllowDelete;
				this.allowRename = opt.AllowRename;
				this.allowQuickCustomization = opt.AllowQuickCustomization;
				this.disableClose = opt.DisableClose;
				this.disableCustomization = opt.DisableCustomization;
				this.drawDragBorder = opt.DrawDragBorder;
				this.drawSizeGrip = opt.DrawSizeGrip;
				this.hidden = opt.Hidden;
				this.multiLine = opt.MultiLine;
				this.rotateWhenVertical = opt.RotateWhenVertical;
				this.useWholeRow = opt.UseWholeRow;
				this.drawBorder = opt.DrawBorder;
			}
			finally {
				EndUpdate();
			}
		}
		internal void ConvertFromFlags(BarOptionFlags flags) {
			AllowDelete = (flags & BarOptionFlags.AllowDelete) != 0;
			AllowQuickCustomization = (flags & BarOptionFlags.AllowQuickCustomization) != 0;
			DisableClose = (flags & BarOptionFlags.DisableClose) != 0;
			DisableCustomization = (flags & BarOptionFlags.DisableCustomization) != 0;
			DrawDragBorder = (flags & BarOptionFlags.DrawDragBorder) != 0;
			DrawSizeGrip = (flags & BarOptionFlags.DrawSizeGrip) != 0;
			Hidden = (flags & BarOptionFlags.Hidden) != 0;
			MultiLine = (flags & BarOptionFlags.MultiLine) != 0;
			RotateWhenVertical = (flags & BarOptionFlags.RotateWhenVertical) != 0;
			UseWholeRow = (flags & BarOptionFlags.UseWholeRow) != 0;
		}
	}
	public class BaseBarManagerOptions : BaseOptions {
		public BaseBarManagerOptions() {
		}
		internal event BaseOptionChangedEventHandler Changed {
			add { base.ChangedCore += value; }
			remove { base.ChangedCore -= value; }
		}
		protected internal bool ShouldSerializeCore() { return ShouldSerialize(); }
	}
}
