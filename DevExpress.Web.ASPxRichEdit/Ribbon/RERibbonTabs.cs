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

using System.ComponentModel;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
namespace DevExpress.Web.ASPxRichEdit {
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class RichEditRibbonTabCollection : Collection<RibbonTab> {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxRichEdit RichEdit { get { return Owner as ASPxRichEdit; } }
		public RichEditRibbonTabCollection(IWebControlObject owner)
			: base(owner) {
		}
		public RibbonTab Add(string text) {
			RibbonTab tab = new RibbonTab(text);
			Add(tab);
			return tab;
		}
		public void CreateDefaultRibbonTabs() {
			AddRange(new RichEditDefaultRibbon(RichEdit).DefaultRibbonTabs);
		}
		protected override void OnChanged() {
			if(Owner != null)
				Owner.LayoutChanged();
			base.OnChanged();
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class RichEditRibbonContextTabCategoryCollection : Collection<RibbonContextTabCategory> {
		public ASPxRichEdit RichEdit { get { return Owner as ASPxRichEdit; } }
		public RichEditRibbonContextTabCategoryCollection(IWebControlObject owner)
			: base(owner) {
		}
		public RibbonContextTabCategory Add(string name) {
			RibbonContextTabCategory tabCategory = new RibbonContextTabCategory(name);
			Add(tabCategory);
			return tabCategory;
		}
		public void CreateDefaultRibbonContextTabCategories() {
			AddRange(new RichEditDefaultRibbon(RichEdit).DefaultRibbonContextTabCategories);
		}
		protected override void OnChanged() {
			if(Owner != null)
				Owner.LayoutChanged();
			base.OnChanged();
		}
	}
	public class RERFileTab : RERTab {
		public RERFileTab()
			: base(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.PageFile)) { }
		public RERFileTab(string text) {
			Text = text;
		}
		[ DefaultValue("File")]
		public new string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName { get { return "File"; } }
	}
	public class RERHomeTab : RERTab {
		public RERHomeTab()
			: base(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.PageHome)) { }
		public RERHomeTab(string text) {
			Text = text;
		}
		[ DefaultValue("Home")]
		public new string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName { get { return "Home"; } }
	}
	public class RERInsertTab : RERTab {
		public RERInsertTab()
			: base(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.PageInsert)) { }
		public RERInsertTab(string text) {
			Text = text;
		}
		[ DefaultValue("Insert")]
		public new string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName { get { return "Insert"; } }
	}
	public class RERPageLayoutTab : RERTab {
		public RERPageLayoutTab()
			: base(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.PagePageLayout)) { }
		public RERPageLayoutTab(string text) {
			Text = text;
		}
		[ DefaultValue("Page Layout")]
		public new string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName { get { return "PageLayout"; } }
	}
	public class RERMailMergeTab : RERTab {
		public RERMailMergeTab()
			: base(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.PageMailings)) { }
		public RERMailMergeTab(string text) {
			Text = text;
		}
		[ DefaultValue("MailMerge")]
		public new string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName { get { return "MailMerge"; } }
	}
	public class RERViewTab : RERTab {
		public RERViewTab()
			: base(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.PageView)) { }
		public RERViewTab(string text) {
			Text = text;
		}
		[ DefaultValue("View")]
		public new string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName { get { return "View"; } }
	}
	public class RERReviewTab : RERTab {
		public RERReviewTab()
			: base(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.PageReview)) { }
		public RERReviewTab(string text) {
			Text = text;
		}
		[ DefaultValue("Review")]
		public new string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName { get { return "Review"; } }
	}
	public class RERReferencesTab : RERTab {
		public RERReferencesTab()
			: base(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.PageReferences)) { }
		public RERReferencesTab(string text) {
			Text = text;
		}
		[ DefaultValue("References")]
		public new string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName { get { return "References"; } }
	}
	public class RERTableDesignTab : RERTab {
		public RERTableDesignTab()
			: base(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.PageTableDesign)) { }
		public RERTableDesignTab(string text) {
			Text = text;
		}
		[ DefaultValue("Design")]
		public new string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName { get { return "TableDesign"; } }
	}
	public class RERTableLayoutTab : RERTab {
		public RERTableLayoutTab()
			: base(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.PageTableLayout)) { }
		public RERTableLayoutTab(string text) {
			Text = text;
		}
		[ DefaultValue("Layout")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName { get { return "TableLayout"; } }
	}
	public class RERHeaderAndFooterTab : RERTab {
		public RERHeaderAndFooterTab()
			: base(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.PageHeaderAndFooter)) { }
		public RERHeaderAndFooterTab(string text) {
			Text = text;
		}
		[ DefaultValue("HeaderAndFooter")]
		public new string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName { get { return "HeaderAndFooter"; } }
	}
	public class RERPictureTab : RERTab {
		public RERPictureTab()
			: base(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.PagePicture)) { }
		public RERPictureTab(string text) {
			Text = text;
		}
		[ DefaultValue("Picture")]
		public new string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName { get { return "Picture"; } }
	}
	public class RERTableToolsContextTabCategory : RERContextTabCategory {
		protected override string DefaultName {
			get {
				return ((int)RichEditClientCommand.ContextItem_Tables).ToString();
			}
		}
		protected override System.Drawing.Color DefaultColor {
			get {
				return System.Drawing.Color.FromArgb(211, 19, 19);
			}
		}
	}
	public class RERHeaderAndFooterToolsContextTabCategory : RERContextTabCategory {
		protected override string DefaultName {
			get {
				return ((int)RichEditClientCommand.ContextItem_HeadersFooters).ToString();
			}
		}
		protected override System.Drawing.Color DefaultColor {
			get {
				return System.Drawing.Color.FromArgb(23, 163, 0);
			}
		}
	}
}
