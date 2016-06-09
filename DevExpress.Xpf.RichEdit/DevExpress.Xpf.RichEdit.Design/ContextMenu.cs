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

extern alias Platform;
using System.Collections.Generic;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using Platform::System.Collections.ObjectModel;
using Platform::DevExpress.Xpf.Core.Design;
namespace DevExpress.Xpf.RichEdit.Design {
	[UsesItemPolicy(typeof(PrimarySelectionPolicy))]
	class RichEditContextMenuProvider : ContextMenuProviderBase {
		public RichEditContextMenuProvider() {
			RichEditBarsPopupMenuCreatorBase barsMenuCreator = new RichEditBarsMenuCreator();
			barsMenuCreator.CreateMenu(Items);
			RichEditBarsPopupMenuCreatorBase ribbonMenuCreator = new RichEditRibbonMenuCreator();
			ribbonMenuCreator.CreateMenu(Items);
			RichEditCommentsMenuCreator commentsMenuCreator = new RichEditCommentsMenuCreator();
			commentsMenuCreator.CreateMenu(Items);
		}
		protected override bool IsActive(Selection selection) {
			return selection.PrimarySelection != null;
		}
	}
	public static class RichEditBarInfos {
		public static BarInfo[] GetAllBarInfos() {
			List<BarInfo> result = new List<BarInfo>();
			result.AddRange(GetCommonBarInfos());
			result.AddRange(GetHomeBarInfos());
			result.AddRange(GetInsertBarInfos());
			result.AddRange(GetPageLayoutBarInfos());
			result.AddRange(GetReferencesBarInfos());
			result.AddRange(GetMailingsBarInfos());
			result.AddRange(GetReviewBarInfos());
			result.AddRange(GetViewBarInfos());
			result.AddRange(GetHeaderFooterBarInfos());
			result.AddRange(GetTablesBarInfos());
			result.AddRange(GetPictureToolsBarInfos());
			return result.ToArray();
		}
		public static BarInfo[] GetCommonBarInfos() {
			return new BarInfo[] { BarInfos.File };
		}
		public static BarInfo[] GetHomeBarInfos() {
			return new BarInfo[] { BarInfos.Clipboard, BarInfos.Font, BarInfos.Paragraph, BarInfos.Styles, BarInfos.Editing };
		}
		public static BarInfo[] GetInsertBarInfos() {
			return new BarInfo[] { BarInfos.Pages, BarInfos.Tables, BarInfos.Illustrations, BarInfos.Links, BarInfos.HeaderFooter, BarInfos.Text, BarInfos.Symbols };
		}
		public static BarInfo[] GetPageLayoutBarInfos() {
			return new BarInfo[] { BarInfos.PageSetup, BarInfos.PageBackground };
		}
		public static BarInfo[] GetReferencesBarInfos() {
			return new BarInfo[] { BarInfos.ReferencesTableOfContents, BarInfos.Captions };
		}
		public static BarInfo[] GetMailingsBarInfos() {
			return new BarInfo[] { BarInfos.MailingsWriteAndInsertFields, BarInfos.MailingsPreviewResults };
		}
		public static BarInfo[] GetReviewBarInfos() {
			return new BarInfo[] { BarInfos.ReviewProofing, BarInfos.ReviewProtection, BarInfos.ReviewComment, BarInfos.ReviewTracking };
		}
		public static BarInfo[] GetViewBarInfos() {
			return new BarInfo[] { BarInfos.DocumentViews, BarInfos.Show, BarInfos.Zoom };
		}
		public static BarInfo[] GetHeaderFooterBarInfos() {
			return new BarInfo[] { BarInfos.HeaderFooterNavigation, BarInfos.HeaderFooterOptions, BarInfos.HeaderFooterClose };
		}
		public static BarInfo[] GetTablesBarInfos() {
			return new BarInfo[] { BarInfos.TableDesignStyles, BarInfos.TableDesignDrawBorders, BarInfos.TableLayoutTable, BarInfos.TableLayoutRowsAndColumns, BarInfos.TableLayoutMerge, BarInfos.TableLayoutCellSize, BarInfos.TableLayoutAlignment };
		}
		public static BarInfo[] GetPictureToolsBarInfos() {
			return new BarInfo[] { BarInfos.PictureToolsShapeStyles, BarInfos.PictureToolsArrange };
		}
	}
	public abstract class RichEditBarsMenuCreatorBase<T> {
		public void PopulateItems(System.Collections.Generic.IList<T> items) {
			items.Add(CreateMenuItem("All", RichEditBarInfos.GetAllBarInfos()));
			items.Add(CreateMenuItem("File", RichEditBarInfos.GetCommonBarInfos()));
			items.Add(CreateMenuItem("Home", RichEditBarInfos.GetHomeBarInfos()));
			items.Add(CreateMenuItem("Insert", RichEditBarInfos.GetInsertBarInfos()));
			items.Add(CreateMenuItem("Page Layout", RichEditBarInfos.GetPageLayoutBarInfos()));
			items.Add(CreateMenuItem("References", RichEditBarInfos.GetReferencesBarInfos()));
			items.Add(CreateMenuItem("Picture Tools", RichEditBarInfos.GetPictureToolsBarInfos()));
			items.Add(CreateMenuItem("Mail Merge", RichEditBarInfos.GetMailingsBarInfos()));
			items.Add(CreateMenuItem("Review", RichEditBarInfos.GetReviewBarInfos()));
			items.Add(CreateMenuItem("View", RichEditBarInfos.GetViewBarInfos()));
			items.Add(CreateMenuItem("Table", RichEditBarInfos.GetTablesBarInfos()));
			items.Add(CreateMenuItem("Header And Footer", RichEditBarInfos.GetHeaderFooterBarInfos()));
		}
		protected void CreateBars(ModelItem primarySelection, BarInfo[] barInfos) {
			CommandBarCreator creator = CreateBarCreator();
			ModelItem parent = DockManagerCreator.FindDockManager(primarySelection);
			if (parent == null)
				creator.CreateBars(primarySelection, barInfos);
			else
				creator.CreateBars(parent, primarySelection, barInfos);
		}
		protected abstract T CreateMenuItem(string caption, BarInfo[] barInfos);
		protected internal abstract CommandBarCreator CreateBarCreator();
	}
	public abstract class RichEditBarsPopupMenuCreatorBase : RichEditBarsMenuCreatorBase<MenuBase> {
		protected internal abstract string SubMenuCaption { get; }
		protected internal abstract string MenuGroupId { get; }
		public void CreateMenu(System.Collections.Generic.IList<MenuBase> items) {
			MenuGroup createToolbarsMenuGroup = new MenuGroup(MenuGroupId, SubMenuCaption);
			createToolbarsMenuGroup.HasDropDown = true;
			PopulateItems(createToolbarsMenuGroup.Items);
			items.Add(createToolbarsMenuGroup);
		}
		protected override MenuBase CreateMenuItem(string caption, BarInfo[] barInfos) {
			MenuAction result = new MenuAction(FormatMenuItemCaption(caption));
			result.Execute += (sender, e) => CreateBars(e.Selection.PrimarySelection, barInfos);
			return result;
		}
		protected internal abstract string FormatMenuItemCaption(string name);
	}
	public class RichEditBarsMenuCreator : RichEditBarsPopupMenuCreatorBase {
		protected internal override string MenuGroupId { get { return "CreateRichEditBarItems"; } }
		protected internal override string SubMenuCaption { get { return "Create Bars"; } }
		protected internal override CommandBarCreator CreateBarCreator() {
			return new RichEditCommandBarCreator();
		}
		protected internal override string FormatMenuItemCaption(string name) {
			return name;
		}
	}
	public class RichEditRibbonMenuCreator : RichEditBarsPopupMenuCreatorBase {
		protected internal override string MenuGroupId { get { return "CreateRichEditRibbonItems"; } }
		protected internal override string SubMenuCaption { get { return "Create Ribbon Items"; } }
		protected internal override CommandBarCreator CreateBarCreator() {
			return new RichEditCommandRibbonCreator();
		}
		protected internal override string FormatMenuItemCaption(string name) {
			return name;
		}
	}
	public class RichEditCommentsMenuCreator {
		MenuAction showComment;
		public void CreateMenu(System.Collections.Generic.IList<MenuBase> items) {
			showComment = new MenuAction(FormatMenuItemCaption("Create Reviewing Pane"));
			showComment.Execute += OnCreateCommentsDockManager;
			items.Add(showComment);
		}
		private void OnCreateCommentsDockManager(object sender, MenuActionEventArgs e) {
			DockManagerCreator creator = CreateDockManagerCreator();
			creator.CreateDockManager(e.Selection.PrimarySelection);
		}
		protected internal string MenuGroupId { get { return "CreateCommentsDockPanel"; } }
		protected internal string SubMenuCaption { get { return "Create Reviewing Pane"; } }
		protected internal DockManagerCreator CreateDockManagerCreator() {
			return new RichEditCommandCommentCreator();
		}
		protected internal string FormatMenuItemCaption(string name) {
			return name;
		}
	}
}
