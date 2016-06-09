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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxScheduler.Localization;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.Web.ASPxScheduler.Internal {
	#region ASPxSchedulerContainerControl
	[ToolboxItem(false)]
	public class ASPxSchedulerContainerControl : ASPxSchedulerControlBlock {
		#region Fields
		WebViewRenderer renderer;
		WebControl moreButtonControl;
		Image timeMarkerImage;
		WebControl timeMarkerDiv;
		bool isHierarchyCreated;
		bool isTemplateAssigned;
		#endregion
		public ASPxSchedulerContainerControl(ASPxScheduler control)
			: base(control) {
			this.renderer = control.ActiveView.FactoryHelper.CreateWebViewRenderer(Owner.ViewInfo);
			this.isTemplateAssigned = control.IsTemplatesAssigned();
		}
		#region Properties
		internal WebViewRenderer Renderer { get { return renderer; } }
		internal WebControl MoreButtonControl { get { return moreButtonControl; } }
		public override string ContentControlID { get { return "containerBlock"; } }
		public override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.RenderView; } }
		internal bool IsTemplateAssigned { get { return isTemplateAssigned; } }
		internal bool EarlyHierarchyCreation { get { return IsTemplateAssigned || Owner.IsRenderStage || !Owner.IsCallback ; } }
		internal bool IsHierarchyCreated { get { return isHierarchyCreated; } }
		public Image TimeMarkerImage { get { return timeMarkerImage; } }
		public WebControl TimeMarkerDiv { get { return timeMarkerDiv; } }
		#endregion
		protected internal override void CreateControlHierarchyCore(Control parent) {
			if (!IsBlockVisible)
				return;
			if(EarlyHierarchyCreation) 
				CreateContainerControlHierarchyCore(parent);
		}
		protected internal override void FinalizeCreateControlHierarchyCore(Control parent) {
			if(IsBlockVisible && !IsHierarchyCreated) 
				CreateContainerControlHierarchyCore(parent);
		}
		protected internal virtual void CreateContainerControlHierarchyCore(Control parent) {
			this.isHierarchyCreated = true;
			Renderer.Render(parent);
			DayView dayView = Owner.ActiveView as DayView;
			this.moreButtonControl = dayView != null ? ((WebControl)new ASPxSchedulerDayViewMoreButtonsControl(Owner, dayView)) : ((WebControl)new ASPxSchedulerMoreButtonControl(Owner));
			if(Owner.ActiveView.ShowMoreButtons) {
				if(dayView == null)
					parent.Controls.Add(this.moreButtonControl);
				else
					((DayViewWebRenderer)Renderer).AddMoreButtonControl(MoreButtonControl);
			}
			this.moreButtonControl.ID = "MoreButtons";
			CreateTimeMarker(parent);
		}
		void CreateTimeMarker(Control parent) {
			this.timeMarkerImage = RenderUtils.CreateImage();
			TimeMarkerImage.ID = "timeMarkerImg";
			parent.Controls.Add(TimeMarkerImage);
			this.timeMarkerDiv = RenderUtils.CreateDiv();
			TimeMarkerDiv.ID = "timeMarkerDiv";
			parent.Controls.Add(TimeMarkerDiv);
		}
		protected internal override void PrepareControlHierarchyCore() {
			if (!IsBlockVisible)
				return;
			ContentControl.Style.Add(HtmlTextWriterStyle.Position, "relative");
			PrepareContentTable();
			Renderer.ApplyStyles();
			if (Owner.DesignMode)
				MoreButtonControl.Visible = false;
			PrepareTimeMarker();
		}
		protected internal virtual void PrepareContentTable() {
			Table table = Renderer.WebTable;
			table.ID = "content";
			RenderUtils.AppendDefaultDXClassName(table, "dxscRendererTable");
		}
		public override void RenderCommonScript(StringBuilder sb, string localVarName, string clientName) {
			base.RenderCommonScript(sb, localVarName, clientName);
			if(!Owner.FormMode)
				Renderer.GetCreateClientViewInfoScript(sb, localVarName);
		}
		protected override bool OnBubbleEvent(object source, EventArgs args) {
			return Owner.ProtectedOnBubbleEvent(source, args);
		}
		void PrepareTimeMarker() {
			StylesHelper stylesHelper = StylesHelper.Create(Owner.ActiveView, Owner.ViewInfo, Owner.Styles);
			if(TimeMarkerImage != null) {
				Page page = ASPxScheduler.ActiveControl != null ? ASPxScheduler.ActiveControl.Page : null;
				ImageProperties timeMarkerImageProperties = Owner.Images.GetImageProperties(page, ASPxSchedulerImages.TimeMarkerName);
				bool designMode = ASPxScheduler.ActiveControl.DesignMode;
				timeMarkerImageProperties.AssignToControl(TimeMarkerImage, designMode);
				TimeMarkerImage.Style.Add(HtmlTextWriterStyle.Display, "none");
				DayViewStylesHelper dayViewStylesHelper = stylesHelper as DayViewStylesHelper;
				if(dayViewStylesHelper != null)
					dayViewStylesHelper.GetTimeMarkerStyle().AssignToControl(TimeMarkerImage);
				if(Owner.DesignMode)
					TimeMarkerImage.Visible = false;
			}
			if(TimeMarkerDiv != null) {
				stylesHelper.GetTimeMarkerLineStyle().AssignToControl(TimeMarkerDiv);
				TimeMarkerDiv.Style.Add(HtmlTextWriterStyle.Display, "none");
				if(Owner.DesignMode)
					TimeMarkerDiv.Visible = false;
			}
		}
	}
	#endregion
	public class ASPxSchedulerDayViewMoreButtonsControl : ASPxInternalWebControl, INamingContainer {
		#region Fields
		DayView view;
		ASPxScheduler schedulerControl;
		List<DayViewBottomMoreButtonInternal> bottomButtons;
		List<DayViewTopMoreButtonInternal> topButtons;
		List<WebControl> bottomButtonDivs;
		List<WebControl> topButtonDivs;
		#endregion
		public ASPxSchedulerDayViewMoreButtonsControl(ASPxScheduler schedulerControl, DayView view) {
			if(schedulerControl == null)
				Exceptions.ThrowArgumentNullException("schedulerControl");
			if (view == null)
				Exceptions.ThrowArgumentNullException("view");
			this.view = view;
			this.schedulerControl = schedulerControl;
		}
		#region Properties
		protected internal List<DayViewBottomMoreButtonInternal> BottomButtons { get { return bottomButtons; } }
		protected internal List<DayViewTopMoreButtonInternal> TopButtons { get { return topButtons; } }
		protected internal List<WebControl> BottomButtonDivs { get { return bottomButtonDivs; } }
		protected internal List<WebControl> TopButtonDivs { get { return topButtonDivs; } }
		protected internal DayView View { get { return view; } }
		#endregion
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.topButtons = new List<DayViewTopMoreButtonInternal>();
			this.topButtonDivs = new List<WebControl>();
			this.bottomButtons = new List<DayViewBottomMoreButtonInternal>();
			this.bottomButtonDivs = new List<WebControl>();
			int count = View.ShowMoreButtonsOnEachColumn ? schedulerControl.ViewInfo.GetContainers().Count : 1;
			for(int i = 0; i < count; i++) {
				WebControl topButtonDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);				
				WebControl bottomButtonDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				Controls.Add(topButtonDiv);
				Controls.Add(bottomButtonDiv);
				topButtonDiv.ID = String.Format("Top_{0}", i);
				bottomButtonDiv.ID = String.Format("Bottom_{0}", i);
				DayViewTopMoreButtonInternal topButton = new DayViewTopMoreButtonInternal();
				DayViewBottomMoreButtonInternal bottomButton = new DayViewBottomMoreButtonInternal();
				topButton.ParentSkinOwner = bottomButton.ParentSkinOwner = schedulerControl;
				topButtonDiv.Controls.Add(topButton);
				bottomButtonDiv.Controls.Add(bottomButton);
				topButton.ID = String.Format("TopButton_{0}", i);
				bottomButton.ID = String.Format("BottomButton_{0}", i);
				topButtonDivs.Add(topButtonDiv);
				bottomButtonDivs.Add(bottomButtonDiv);
				topButtons.Add(topButton);
				bottomButtons.Add(bottomButton);
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			int count = topButtonDivs.Count;
			DayView view = (DayView)schedulerControl.ActiveView;
			DayViewStyles styles = (DayViewStyles)view.Styles;
			for(int i = 0; i < count; i++ ) {
				topButtonDivs[i].Style.Add(HtmlTextWriterStyle.Position, "absolute");
				topButtonDivs[i].Style.Add(HtmlTextWriterStyle.Display, "none");
				bottomButtonDivs[i].Style.Add(HtmlTextWriterStyle.Position, "absolute");
				bottomButtonDivs[i].Style.Add(HtmlTextWriterStyle.Display, "none");
				styles.GetTopMoreButtonStyle().AssignToControl(topButtons[i]);
				styles.GetBottomMoreButtonStyle().AssignToControl(bottomButtons[i]);
				topButtons[i].Image.Assign(GetTopMoreButtonImage());
				bottomButtons[i].Image.Assign(GetBottomMoreButtonImage());
				topButtons[i].ImagePosition = ImagePosition.Top;
				bottomButtons[i].ImagePosition = ImagePosition.Bottom;
			}
		}
		protected internal virtual ImageProperties GetTopMoreButtonImage() {
			return schedulerControl.Images.MoreButton.GetImageProperties(Page, MoreButtonImages.TopName);
		}
		protected internal virtual ImageProperties GetBottomMoreButtonImage()  {
			return schedulerControl.Images.MoreButton.GetImageProperties(Page, MoreButtonImages.BottomName);
		}
	}
	public class ASPxSchedulerMoreButtonControl : ASPxInternalWebControl {
		WebControl mainDiv;
		ASPxScheduler schedulerControl;
		public ASPxSchedulerMoreButtonControl(ASPxScheduler schedulerControl) {
			if (schedulerControl == null)
				Exceptions.ThrowArgumentNullException("schedulerControl");
			this.schedulerControl = schedulerControl;
		}
		protected internal WebControl MainDiv { get { return mainDiv; } set { mainDiv = value; } }
		protected internal ASPxScheduler SchedulerControl { get { return schedulerControl; } }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			MainDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			LiteralControl buttonText = new LiteralControl();
			string activeViewShowMoreButtonText = schedulerControl.ActiveView.MoreButtonHTML;
			string defaultShowMoreButtonText = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_ShowMore);
			string actualText = (String.IsNullOrEmpty(activeViewShowMoreButtonText)) ? defaultShowMoreButtonText : activeViewShowMoreButtonText;
			buttonText.Text = actualText;
			MainDiv.Controls.Add(buttonText);
			this.Controls.Add(MainDiv);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			MainDiv.ID = "moreButton";
			StylesHelper stylesHelper = StylesHelper.Create(schedulerControl.ActiveView, schedulerControl.ViewInfo, schedulerControl.Styles);
			AppearanceStyle textStyle = stylesHelper.GetMoreButtonStyle();
			textStyle.AssignToControl(MainDiv);
			RenderUtils.AppendDefaultDXClassName(MainDiv, "dxscMoreButton");
			MainDiv.Style.Add(HtmlTextWriterStyle.Display, "none");
		}
	}
}
