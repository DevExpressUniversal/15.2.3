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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms.VisualStyles;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[TypeConverter(typeof(ExpandableObjectConverter)), AutoFormatUrlPropertyClass]
	public class PanelPart : PropertiesBase, IPropertiesOwner {
		private BackgroundImage fBackgroundImage = null;
		public PanelPart()
			: this(null) {
		}
		public PanelPart(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelPartBackColor"),
#endif
		DefaultValue(typeof(Color), ""), NotifyParentProperty(true), TypeConverter(typeof(WebColorConverter)),
		AutoFormatEnable]
		public Color BackColor {
			get { return GetColorProperty("BackColor", Color.Empty); }
			set {
				SetColorProperty("BackColor", Color.Empty, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelPartBackgroundImage"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public BackgroundImage BackgroundImage {
			get {
				if(fBackgroundImage == null)
					fBackgroundImage = new BackgroundImage();
				return fBackgroundImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelPartIsEmpty"),
#endif
 Browsable(false)]
		public bool IsEmpty {
			get { return BackColor.IsEmpty && BackgroundImage.IsEmpty; }
		}
		public override string ToString() {
			if(BackgroundImage.IsEmpty && BackColor.IsEmpty)
				return "";
			string s = "";
			if(!BackgroundImage.IsEmpty)
				s += BackgroundImage.ImageUrl + " ";
			if(!BackColor.IsEmpty)
				s += HtmlConvertor.ToHtml(BackColor) + " ";
			return s;
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				if(source is PanelPart) {
					PanelPart src = source as PanelPart;
					BackColor = src.BackColor;
					BackgroundImage.Assign(src.BackgroundImage);
				}
			} finally {
				EndUpdate();
			}
		}
		public void CopyFrom(PanelPart source) {
			if(!source.BackColor.IsEmpty)
				BackColor = source.BackColor;
			BackgroundImage.CopyFrom(source.BackgroundImage);
		}
		public void Reset() {
			BackColor = Color.Empty;
			BackgroundImage.Reset();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { BackgroundImage };
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	public class PanelCornerPart : ImageProperties {
		public PanelCornerPart()
			: this(null) {
		}
		public PanelCornerPart(ASPxWebControl owner)
			: base(owner) {
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string AlternateText {
			get { return base.AlternateText; }
			set { base.AlternateText = value; }
		}
		protected internal override string SpriteUrl {
			get {
				if(string.IsNullOrEmpty(base.SpriteUrl) && (Owner != null))
					return (Owner as ASPxWebControl).SpriteImageUrlInternal;
				return base.SpriteUrl;
			}
			set { base.SpriteUrl = value; }
		}
	}
	public class RoundPanelParts : PropertiesBase, IPropertiesOwner {
		protected internal PanelCornerPart TopLeftCornerInternal { get; private set; }
		protected internal PanelCornerPart TopRightCornerInternal { get; private set; }
		protected internal PanelCornerPart NoHeaderTopLeftCornerInternal { get; private set; }
		protected internal PanelCornerPart NoHeaderTopRightCornerInternal { get; private set; }
		protected internal PanelCornerPart BottomRightCornerInternal { get; private set; }
		protected internal PanelCornerPart BottomLeftCornerInternal { get; private set; }
		protected internal PanelPart TopEdgeInternal { get; private set; }
		protected internal PanelPart NoHeaderTopEdgeInternal { get; private set; }
		protected internal PanelPart BottomEdgeInternal { get; private set; }
		protected internal PanelPart LeftEdgeInternal { get; private set; }
		protected internal PanelPart RightEdgeInternal { get; private set; }
		protected internal PanelPart HeaderLeftEdgeInternal { get; private set; }
		protected internal PanelPart HeaderRightEdgeInternal { get; private set; }
		public RoundPanelParts(ASPxWebControl owner)
			: base(owner) {
			TopLeftCornerInternal = new PanelCornerPart(owner);
			TopRightCornerInternal = new PanelCornerPart(owner);
			NoHeaderTopLeftCornerInternal = new PanelCornerPart(owner);
			NoHeaderTopRightCornerInternal = new PanelCornerPart(owner);
			BottomRightCornerInternal = new PanelCornerPart(owner);
			BottomLeftCornerInternal = new PanelCornerPart(owner);
			TopEdgeInternal = new PanelPart(owner);
			NoHeaderTopEdgeInternal = new PanelPart(owner);
			BottomEdgeInternal = new PanelPart(owner);
			LeftEdgeInternal = new PanelPart(owner);
			RightEdgeInternal = new PanelPart(owner);
			Content = new PanelPart(owner);
			HeaderLeftEdgeInternal = new PanelPart(owner);
			HeaderContent = new PanelPart(owner);
			HeaderRightEdgeInternal = new PanelPart(owner);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelPartsShowDefaultImages"),
#endif
 Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."),
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable, DefaultValue(false)]
		public bool ShowDefaultImages {
			get { return false; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelPartsTopLeftCorner"),
#endif
 Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."),
		Category("Parts"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public PanelCornerPart TopLeftCorner { get { return TopLeftCornerInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelPartsNoHeaderTopLeftCorner"),
#endif
 Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."),
		Category("Parts"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public PanelCornerPart NoHeaderTopLeftCorner { get { return NoHeaderTopLeftCornerInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelPartsTopRightCorner"),
#endif
 Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."),
		Category("Parts"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public PanelCornerPart TopRightCorner { get { return TopRightCornerInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelPartsNoHeaderTopRightCorner"),
#endif
 Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."),
		Category("Parts"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public PanelCornerPart NoHeaderTopRightCorner { get { return NoHeaderTopRightCornerInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelPartsBottomRightCorner"),
#endif
 Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."),
		Category("Parts"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public PanelCornerPart BottomRightCorner { get { return BottomRightCornerInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelPartsBottomLeftCorner"),
#endif
 Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."),
		Category("Parts"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public PanelCornerPart BottomLeftCorner { get { return BottomLeftCornerInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelPartsHeaderLeftEdge"),
#endif
 Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."),
		Category("Parts"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public PanelPart HeaderLeftEdge { get { return HeaderLeftEdgeInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelPartsHeaderContent"),
#endif
		Category("Parts"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public PanelPart HeaderContent { get; private set; }
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelPartsHeaderRightEdge"),
#endif
 Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."),
		Category("Parts"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public PanelPart HeaderRightEdge { get { return HeaderRightEdgeInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelPartsNoHeaderTopEdge"),
#endif
 Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."),
		Category("Parts"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public PanelPart NoHeaderTopEdge { get { return NoHeaderTopEdgeInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelPartsTopEdge"),
#endif
 Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."),
		Category("Parts"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public PanelPart TopEdge { get { return TopEdgeInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelPartsRightEdge"),
#endif
 Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."),
		Category("Parts"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public PanelPart RightEdge { get { return RightEdgeInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelPartsBottomEdge"),
#endif
 Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."),
		Category("Parts"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public PanelPart BottomEdge { get { return BottomEdgeInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelPartsLeftEdge"),
#endif
 Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."),
		Category("Parts"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public PanelPart LeftEdge { get { return LeftEdgeInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelPartsContent"),
#endif
		Category("Parts"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public PanelPart Content { get; private set; }
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				if(source is RoundPanelParts) {
					RoundPanelParts src = source as RoundPanelParts;
					TopLeftCornerInternal.Assign(src.TopLeftCornerInternal);
					TopRightCornerInternal.Assign(src.TopRightCornerInternal);
					Content.Assign(src.Content);
					NoHeaderTopLeftCornerInternal.Assign(src.NoHeaderTopLeftCornerInternal);
					NoHeaderTopRightCornerInternal.Assign(src.NoHeaderTopRightCornerInternal);
					BottomLeftCornerInternal.Assign(src.BottomLeftCornerInternal);
					BottomRightCornerInternal.Assign(src.BottomRightCornerInternal);
					TopEdgeInternal.Assign(src.TopEdgeInternal);
					NoHeaderTopEdgeInternal.Assign(src.NoHeaderTopEdgeInternal);
					BottomEdgeInternal.Assign(src.BottomEdgeInternal);
					LeftEdgeInternal.Assign(src.LeftEdgeInternal);
					RightEdgeInternal.Assign(src.RightEdgeInternal);
					HeaderLeftEdgeInternal.Assign(src.HeaderLeftEdgeInternal);
					HeaderRightEdgeInternal.Assign(src.HeaderRightEdgeInternal);
					HeaderContent.Assign(src.HeaderContent);
				}
			} finally {
				EndUpdate();
			}
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] {Content, TopLeftCornerInternal, NoHeaderTopLeftCornerInternal, TopRightCornerInternal, 
				NoHeaderTopRightCornerInternal, BottomLeftCornerInternal, BottomRightCornerInternal, 
					TopEdgeInternal, NoHeaderTopEdgeInternal, BottomEdgeInternal, LeftEdgeInternal, RightEdgeInternal, HeaderContent, HeaderLeftEdgeInternal, HeaderRightEdgeInternal};
		}
	}
}
