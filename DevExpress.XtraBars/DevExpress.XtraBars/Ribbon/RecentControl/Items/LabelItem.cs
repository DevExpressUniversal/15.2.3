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

using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Ribbon {
	public enum RecentLabelStyles { Small, Medium, Large }
	public class RecentLabelItem : RecentTextGlyphItemBase {
		DefaultBoolean allowSelect;
		RecentLabelStyles style;
		public RecentLabelItem()
			: base() {
			this.allowSelect = DefaultBoolean.Default;
			this.style = RecentLabelStyles.Medium;
		}
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AllowSelect {
			get { return allowSelect; }
			set {
				if(AllowSelect == value) return;
				allowSelect = value;
				OnItemChanged();
			}
		}
		[DefaultValue(RecentLabelStyles.Medium)]
		public virtual RecentLabelStyles Style {
			get { return style; }
			set {
				if(Style == value) return;
				style = value;
				ViewInfo.SetAppearanceDirty();
				OnItemChanged();
			}
		}
		protected override RecentItemPainterBase CreateItemPainter() {
			return new RecentTextGlyphItemPainterBase();
		}
		protected override RecentItemViewInfoBase CreateItemViewInfo() {
			return new RecentLabelItemViewInfo(this);
		}
		protected override RecentItemHandlerBase CreateItemHandler() {
			return new RecentLabelItemHandler(this);
		}
	}
	public class RecentLabelItemAppearances : BaseAppearanceCollection {
		BaseRecentItemAppearanceCollection largeLabel, smallLabel, mediumLabel;
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.largeLabel = CreateBaseAppearanceCollection();
			this.smallLabel = CreateBaseAppearanceCollection();
			this.mediumLabel = CreateBaseAppearanceCollection();
			this.largeLabel.Changed += OnAppearanceChanged;
			this.smallLabel.Changed += OnAppearanceChanged;
			this.mediumLabel.Changed += OnAppearanceChanged;
		}
		BaseRecentItemAppearanceCollection CreateBaseAppearanceCollection() {
			return new BaseRecentItemAppearanceCollection();
		}
		void ResetLabel() { Label.Reset(); }
		bool ShouldSerializeLabel() { return Label.ShouldSerialize(); }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BaseRecentItemAppearanceCollection Label { get { return mediumLabel; } }
		void ResetLargeLabel() { LargeLabel.Reset(); }
		bool ShouldSerializeLargeLabel() { return LargeLabel.ShouldSerialize(); }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BaseRecentItemAppearanceCollection LargeLabel { get { return largeLabel; } }
		void ResetSmallLabel() { SmallLabel.Reset(); }
		bool ShouldSerializeSmallLabel() { return SmallLabel.ShouldSerialize(); }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BaseRecentItemAppearanceCollection SmallLabel { get { return smallLabel; } }
		public override string ToString() {
			return string.Empty;
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public class RecentLabelItemViewInfo : RecentTextGlyphItemViewInfoBase {
		public RecentLabelItemViewInfo(RecentLabelItem item) : base(item) {
		}
		protected virtual bool AllowHotTrack {
			get {
				if(LabelItem.AllowSelect == DefaultBoolean.True) return true;
				return false;
			}
		}
		protected override Padding ControlItemPadding { get { return Item.RecentControl.PaddingLabelItem; } }
		protected internal RecentLabelItem LabelItem { get { return Item as RecentLabelItem; } }
		protected override BaseRecentItemAppearanceCollection ControlAppearances { 
			get {
				if(LabelItem.Style == RecentLabelStyles.Large)
					return (Item.RecentControl.Appearances as RecentAppearanceCollection).LabelItem.LargeLabel;
				if(LabelItem.Style == RecentLabelStyles.Small)
					return (Item.RecentControl.Appearances as RecentAppearanceCollection).LabelItem.SmallLabel;
				return (Item.RecentControl.Appearances as RecentAppearanceCollection).LabelItem.Label; 
			} 
		}
		protected override AppearanceDefaultInfo[] GetAppearanceDefaultInfo() {
			AppearanceDefault appearance = new AppearanceDefault();
			ApplyBaseForeColor(appearance);
			if(LabelItem.Style == RecentLabelStyles.Large) {
				appearance.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular);
				appearance.FontStyleDelta = System.Drawing.FontStyle.Regular;
			}
			if(LabelItem.Style == RecentLabelStyles.Medium) {
				appearance.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
				appearance.FontStyleDelta = System.Drawing.FontStyle.Regular;
			}
			return new AppearanceDefaultInfo[]{
				new AppearanceDefaultInfo("ItemNormal", appearance),
				new AppearanceDefaultInfo("ItemHovered", appearance),
				new AppearanceDefaultInfo("ItemPressed", appearance)
			};
		}
		int CalcCaptionBestHeight() {
			int res;
			GraphicsInfo gInfo = new GraphicsInfo();
			gInfo.AddGraphics(null);
			try {
				res = GetPaintAppearance("ItemNormal").CalcTextSizeInt(gInfo.Graphics, Item.Caption, CaptionTextBounds.Width).Height;
			}
			finally {
				gInfo.ReleaseGraphics();
			}
			return res;
		}
		protected override ObjectState CalcItemState() {
			if(!AllowHotTrack) return ObjectState.Normal;
			return base.CalcItemState();
		}
		protected override void UpdateDefaults(AppearanceObject app) {
			base.UpdateDefaults(app);
			app.TextOptions.WordWrap = WordWrap.Wrap;
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.Handler {
	public class RecentLabelItemHandler : RecentItemHandlerBase {
		public RecentLabelItemHandler(RecentLabelItem item) : base(item) { }
	}
}
