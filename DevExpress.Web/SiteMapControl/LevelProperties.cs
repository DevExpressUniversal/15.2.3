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
using DevExpress.Web;
using DevExpress.Utils;
namespace DevExpress.Web {
	[TypeConverter(typeof(ExpandableObjectConverter)), AutoFormatUrlPropertyClass]
	public class LevelProperties : CollectionItem {
		private ImageProperties fImage = null;
		private ImageProperties fParentImage = null;
		private Paddings fChildNodesPaddings = null;
		private Paddings fNodePaddings = null;
		private AppearanceSelectedStyle fCurrentNodeStyle = null;
		private AppearanceStyleBase fStyle = null;
		private ITemplate fNodeTemplate = null;
		private ITemplate fNodeTextTemplate = null;
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesBackColor"),
#endif
		Category("Appearance"), AutoFormatEnable, DefaultValue(typeof(Color), ""), NotifyParentProperty(true),
		TypeConverter(typeof(WebColorConverter))]
		public Color BackColor {
			get { return Style.BackColor; }
			set { Style.BackColor = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesBackgroundImage"),
#endif
		Category("Appearance"), AutoFormatEnable, NotifyParentProperty(true), DefaultValue(""),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public BackgroundImage BackgroundImage {
			get { return Style.BackgroundImage; }
			set { Style.BackgroundImage.CopyFrom(value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesBorder"),
#endif
		Category("Appearance"), AutoFormatEnable, NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public BorderWrapper Border {
			get { return Style.Border; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesBorderBottom"),
#endif
		Category("Appearance"), AutoFormatEnable, DefaultValue(typeof(BorderBottom), ""), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public Border BorderBottom {
			get { return Style.BorderBottom; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesBorderLeft"),
#endif
		Category("Appearance"), AutoFormatEnable, DefaultValue(typeof(BorderLeft), ""), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public Border BorderLeft {
			get { return Style.BorderLeft; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesBorderRight"),
#endif
		Category("Appearance"), AutoFormatEnable, DefaultValue(typeof(BorderRight), ""), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public Border BorderRight {
			get { return Style.BorderRight; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesBorderTop"),
#endif
		Category("Appearance"), AutoFormatEnable, DefaultValue(typeof(BorderTop), ""), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public Border BorderTop {
			get { return Style.BorderTop; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesCssClass"),
#endif
		Category("Appearance"), AutoFormatEnable, DefaultValue(""), Localizable(false), NotifyParentProperty(true)]
		public string CssClass {
			get { return Style.CssClass; }
			set { Style.CssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesFont"),
#endif
		Category("Appearance"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)]
		public FontInfo Font {
			get { return Style.Font; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesForeColor"),
#endif
		Category("Appearance"), AutoFormatEnable, DefaultValue(typeof(Color), ""),
		NotifyParentProperty(true), TypeConverter(typeof(WebColorConverter))]
		public Color ForeColor {
			get { return Style.ForeColor; }
			set { Style.ForeColor = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesHorizontalAlign"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(HorizontalAlign.NotSet), NotifyParentProperty(true)]
		public HorizontalAlign HorizontalAlign {
			get { return Style.HorizontalAlign; }
			set { Style.HorizontalAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesVerticalAlign"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(VerticalAlign.NotSet), NotifyParentProperty(true)]
		public virtual VerticalAlign VerticalAlign {
			get { return Style.VerticalAlign; }
			set { Style.VerticalAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesWrap"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean Wrap {
			get { return Style.Wrap; }
			set { Style.Wrap = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesImage"),
#endif
		Category("Images"), AutoFormatEnable, NotifyParentProperty(true), DefaultValue(""),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ImageProperties Image {
			get {
				if(fImage == null)
					fImage = new ImageProperties();
				return fImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesParentImage"),
#endif
		Category("Images"), AutoFormatEnable, NotifyParentProperty(true), DefaultValue(""),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ImageProperties ParentImage {
			get {
				if(fParentImage == null)
					fParentImage = new ImageProperties();
				return fParentImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesImageSpacing"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), ""), NotifyParentProperty(true)]
		public virtual Unit ImageSpacing {
			get { return GetUnitProperty("ImageSpacing", Unit.Empty); }
			set { SetUnitProperty("ImageSpacing", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesChildNodesPaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public virtual Paddings ChildNodesPaddings {
			get {
				if(fChildNodesPaddings == null)
					fChildNodesPaddings = new Paddings();
				return fChildNodesPaddings;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesNodePaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public virtual Paddings NodePaddings {
			get {
				if(fNodePaddings == null)
					fNodePaddings = new Paddings();
				return fNodePaddings;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesNodeSpacing"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), ""), NotifyParentProperty(true)]
		public virtual Unit NodeSpacing {
			get { return GetUnitProperty("NodeSpacing", Unit.Empty); }
			set { SetUnitProperty("NodeSpacing", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesBulletStyle"),
#endif
		Category("Misc"), AutoFormatEnable, DefaultValue(NodeBulletStyle.NotSet), NotifyParentProperty(true)]
		public NodeBulletStyle BulletStyle {
			get { return (NodeBulletStyle)GetEnumProperty("BulletStyle ", NodeBulletStyle.NotSet); }
			set { SetEnumProperty("BulletStyle ", NodeBulletStyle.NotSet, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesTarget"),
#endif
		Category("Misc"), DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		TypeConverter(typeof(TargetConverter)), AutoFormatDisable]
		public virtual string Target {
			get { return GetStringProperty("Target", ""); }
			set { SetStringProperty("Target", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LevelPropertiesCurrentNodeStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceSelectedStyle CurrentNodeStyle {
			get {
				if(fCurrentNodeStyle == null)
					fCurrentNodeStyle = new AppearanceSelectedStyle();
				return fCurrentNodeStyle;
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(NodeTemplateContainer)),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate NodeTemplate {
			get { return fNodeTemplate; }
			set {
				fNodeTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(NodeTemplateContainer)),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate NodeTextTemplate {
			get { return fNodeTextTemplate; }
			set {
				fNodeTextTemplate = value;
				TemplatesChanged();
			}
		}
		protected internal AppearanceStyleBase Style {
			get {
				if (fStyle == null)
					fStyle = CreateStyle();
				return fStyle;
			}
		}
		public LevelProperties()
			: base() {
		}
		public override void Assign(CollectionItem source) {
			if (source is LevelProperties) {
				LevelProperties src = source as LevelProperties;
				Style.Assign(src.Style);
				CurrentNodeStyle.Assign(src.CurrentNodeStyle);
				BulletStyle = src.BulletStyle;
				Image.Assign(src.Image);
				ParentImage.Assign(src.ParentImage);
				ImageSpacing = src.ImageSpacing;
				ChildNodesPaddings.Assign(src.ChildNodesPaddings);
				NodePaddings.Assign(src.NodePaddings);
				NodeSpacing = src.NodeSpacing;
				Target = src.Target;
				NodeTemplate = src.NodeTemplate;
				NodeTextTemplate = src.NodeTextTemplate;
			}
			base.Assign(source);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { ChildNodesPaddings, CurrentNodeStyle, NodePaddings, Image, ParentImage, Style };
		}
		protected virtual AppearanceStyleBase CreateStyle() {
			return new AppearanceStyleBase();
		}
	}
	public class DefaultLevelProperties : LevelProperties {
		public override string ToString() {
			return "";
		}
	}
	[AutoFormatUrlPropertyClass]
	public class LevelPropertiesCollection : Collection<LevelProperties> {
		public LevelPropertiesCollection()
			: base() {
		}
		public LevelPropertiesCollection(ASPxSiteMapControlBase siteMapControl)
			: base(siteMapControl) {
		}
		public LevelProperties Add() {
			return AddInternal(new LevelProperties());
		}
	}
}
