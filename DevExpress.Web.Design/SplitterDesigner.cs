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
using System.ComponentModel;
using System.Web.UI.Design;
using System.ComponentModel.Design;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using System.Web.UI.WebControls;
using DevExpress.Utils.About;
namespace DevExpress.Web.Design {
	public class ASPxSplitterDesigner : ASPxWebControlDesigner {
		ASPxSplitter splitter;
		public ASPxSplitter Splitter { 
			get { return splitter; } 
			set { splitter = value; }
		}
		public Orientation Orientation {
			get { return splitter.Orientation; }
			set {
				Orientation oldValue = Orientation;
				splitter.Orientation = value;
				RaiseComponentChanged(TypeDescriptor.GetProperties(typeof(ASPxSplitter))["Orientation"], oldValue, value);
			}
		}
		public override void Initialize(IComponent component) {
			this.splitter = (ASPxSplitter)component;
			base.Initialize(component);
			SetViewFlags(ViewFlags.TemplateEditing, true);
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Panes", "Panes");
		}
		protected override string GetBaseProperty() {
			return "Panes";
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new SplitterDesignerActionList(this);
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new SplitterPanesOwner(Splitter, DesignerHost)));
		}
		protected internal void FillPanesList(List<SplitterPane> panesList, SplitterPane pane) {
			if(!pane.HasVisibleChildren && (pane.Parent != null))
				panesList.Add(pane);
			else {
				foreach(SplitterPane child in pane.Panes.GetVisibleItems())
					FillPanesList(panesList, child);
			}
		}
		protected override void AddDesignerRegions(DesignerRegionCollection regions) {
			base.AddDesignerRegions(regions);
			int regionCount = regions.Count;
			List<SplitterPane> panesList = new List<SplitterPane>();
			FillPanesList(panesList, Splitter.RootPane);
			for(int i = 0; i < panesList.Count; i++) {
				regions.Add(new SplitterEditableRegion(this, "Edit pane " + panesList[i].GetPath(), panesList[i]));
				((ASPxSplitter)ViewControl).GetPaneByStringPath(Splitter.RenderHelper.GetPanePath(panesList[i])).ContentControlInternal.Attributes[DesignerRegion.DesignerRegionAttributeName] = (regionCount + i).ToString();
			}
		}
		public override string GetEditableDesignerRegionContent(EditableDesignerRegion region) {
			SplitterEditableRegion splitterRegion = (SplitterEditableRegion)region;
			return GetEditableDesignerRegionContent(splitterRegion.Pane.ContentControlInternal.Controls);
		}
		public override void SetEditableDesignerRegionContent(EditableDesignerRegion region, string content) {
			SplitterEditableRegion splitterRegion = (SplitterEditableRegion)region;
			SetEditableDesignerRegionContent(splitterRegion.Pane.ContentControlInternal.Controls, content);
		}
		public override void ShowAbout() {
			SplitterAboutDialogHelper.ShowAbout(Component.Site);
		}
	}
	public class SplitterAboutDialogHelper : AboutDialogHelperBase {
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxSplitter), new ProductKind[] { ProductKind.FreeOffer, ProductKind.DXperienceASP });
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
			if (ShouldShowTrialAbout(typeof(ASPxSplitter)))
				ShowAbout(provider);
		}
	}
	public class SplitterEditableRegion : EditableDesignerRegion {
		private SplitterPane pane = null;
		public SplitterEditableRegion(ASPxSplitterDesigner designer, string name, SplitterPane pane)
			: base(designer, name, false) {
			this.pane = pane;
		}
		public SplitterPane Pane { get { return pane; } }
	}
	public class SplitterDesignerActionList: ASPxWebControlDesignerActionList {
		ASPxSplitterDesigner designer;
		public Orientation Orientation {
			get { return designer.Orientation; }
			set { designer.Orientation = value; }
		}
		public SplitterDesignerActionList(ASPxSplitterDesigner designer)
			: base(designer) {
			this.designer = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("Orientation",
				StringResources.SplitterActionList_Orientation,
				StringResources.ActionList_MiscCategory,
				StringResources.SplitterActionList_OrientationDescription));
			return collection;
		}
	}
	public class SplitterPanesOwner : HierarchicalItemOwner<SplitterPane> {
		public SplitterPanesOwner(ASPxSplitter splitter, IServiceProvider provider) 
			: base(splitter, provider, splitter.Panes) {
		}
		protected internal override string GetNavBarItemsGroupName() {
			return "Panes";
		}
	}
}
