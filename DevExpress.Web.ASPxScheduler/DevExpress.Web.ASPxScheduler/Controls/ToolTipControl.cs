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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web.ASPxScheduler.Localization;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxScheduler.Drawing {
	[ToolboxItem(false)]
	public abstract class ASPxSchedulerToolTipControlBase : ASPxWebControl {
		protected internal const string ToolTipScriptResourceName = "Scripts.SchedulerToolTip.js";
		readonly ASPxScheduler control;
		WebControl mainDiv;
		TooltipTemplateContainer templateContainer;
		readonly ToolTipWrapperBuilderBase toolTipWrapperBuilder;
		protected ASPxSchedulerToolTipControlBase(ASPxScheduler control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.toolTipWrapperBuilder = ToolTipWrapperBuilderBase.CreateBuilder(control, CornerType);
		}
		public abstract string Name { get; }
		public abstract bool CanShowToolTip { get; }
		protected ASPxScheduler Control { get { return control; } }
		TooltipTemplateContainer TemplateContainer { get { return templateContainer; } }
		protected abstract string TemplateUrl { get; }
		protected abstract ToolTipCornerType CornerType { get; }
		ToolTipWrapperBuilderBase BallownWrapperBuilder { get { return toolTipWrapperBuilder; } }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.mainDiv = RenderUtils.CreateDiv();
			Controls.Add(this.mainDiv);
			this.mainDiv.ID = "mainDiv";
			Control toolTip = CreateToolTipForm();
			this.templateContainer = CreateTemplateContainer();
			this.templateContainer.ID = "tc";
			WebControl content = this.toolTipWrapperBuilder.CreateWrap(mainDiv);
			content.Controls.Add(this.templateContainer);
			SchedulerUserControl.PrepareUserControl(toolTip, this.templateContainer, Name);
			if(this.templateContainer.Content != null)
				this.templateContainer.Content.ID = "content";
		}
		protected Control CreateToolTipForm() {
			if(IsCreateUserControl())
				return CreateUserControl(TemplateUrl);
			return CreateDefaultForm();
		}
		protected virtual bool IsCreateUserControl() {
			return !String.IsNullOrEmpty(TemplateUrl);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(TemplateContainer == null)
				return;
			bool showStem = GetStemVisibility(this.templateContainer);
			BallownWrapperBuilder.PrepareWrap(showStem);
			this.mainDiv.Style.Add(HtmlTextWriterStyle.ZIndex, "10000");
			this.mainDiv.Style.Add(HtmlTextWriterStyle.Position, "absolute");
			this.mainDiv.Style.Add(HtmlTextWriterStyle.Display, "none");
		}
		protected virtual bool GetStemVisibility(TooltipTemplateContainer tooltipTemplateContainer) {
			if(tooltipTemplateContainer == null || tooltipTemplateContainer.Content == null)
				return false;
			return this.templateContainer.Content.ToolTipShowStem;
		}
		protected internal virtual ITemplate LoadTemplate(string name, string templateUrl) {
			if(!String.IsNullOrEmpty(templateUrl))
				return LoadTemplateCore(templateUrl);
			return LoadTemplateCore(CommonUtils.GetDefaultFormUrl(name, typeof(ASPxScheduler)));
		}
		protected internal virtual ITemplate LoadTemplateCore(string templateUrl) {
			if(String.IsNullOrEmpty(templateUrl))
				return null;
			try {
				return Control.Page.LoadTemplate(templateUrl);
			}
			catch(Exception e) {
				string subject = String.Format(ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_LoadError), templateUrl);
				SchedulerStatusInfoHelper.AddError(Control, subject, DevExpress.Web.ASPxScheduler.Internal.ExceptionHelper.PrepareDetailedExceptionMessageAsHtml(Control, e, Control.OptionsBehavior.ShowDetailedErrorInfo));
				return null;
			}
		}
		protected virtual TooltipTemplateContainer CreateTemplateContainer() {
			return new TooltipTemplateContainer(Control);
		}
		protected virtual string GetTemplateContentClientInstanceName(TooltipTemplateContainer templateContainer) {
			if(templateContainer == null || templateContainer.Content == null || String.IsNullOrEmpty(templateContainer.Content.ClassName))
				return String.Empty;
			return templateContainer.Content.ActualClientInstanceName;
		}
		#region Client scripts support
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterPopupUtilsScripts();
			RegisterIncludeScript(typeof(ASPxSchedulerAppointmentToolTipControl), ToolTipScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.SchedulerToolTip";
		}
		protected override void GetFinalizeClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetFinalizeClientObjectScript(stb, localVarName, clientName);
			string tooltipClientInstanceName = GetTemplateContentClientInstanceName(TemplateContainer);
			stb.AppendFormat("{0}.canShowToolTip = {1};\n", localVarName, CanShowToolTip ? "true" : "false"); 
			if (!String.IsNullOrEmpty(tooltipClientInstanceName))
				stb.AppendFormat("{0}.templatedToolTip = {1};\n", localVarName, tooltipClientInstanceName);
		}
		#region GetClientName
		internal string GetClientName() {
			System.Diagnostics.Debug.Assert(Name.Length > 1);
			return Name.Substring(0, 1).ToLower() + Name.Substring(1);
		}
		#endregion
		#endregion
		protected abstract Control CreateDefaultForm();
	}
	public class ASPxSchedulerAppointmentDragToolTipControl
		: ASPxSchedulerToolTipControlBase {
		public ASPxSchedulerAppointmentDragToolTipControl(ASPxScheduler control) : base(control) {
		}
		public override string Name { get { return SchedulerFormNames.AppointmentDragToolTip; } }
		public override bool CanShowToolTip { get { return Control.OptionsToolTips.ShowAppointmentDragToolTip; } }
		protected override string TemplateUrl { get { return Control.OptionsToolTips.AppointmentDragToolTipUrl; } }
		protected override ToolTipCornerType CornerType { get { return Control.OptionsToolTips.AppointmentDragToolTipCornerType; } }
		protected override Control CreateDefaultForm() {
			return new DevExpress.Web.ASPxScheduler.Forms.Internal.AppointmentDragToolTip();
		}
	}
	public class ASPxSchedulerAppointmentToolTipControl : ASPxSchedulerToolTipControlBase {
		public ASPxSchedulerAppointmentToolTipControl(ASPxScheduler control)
			: base(control) {
		}
		public override string Name { get { return SchedulerFormNames.AppointmentToolTip; } }
		public override bool CanShowToolTip { get { return Control.OptionsToolTips.ShowAppointmentToolTip; } }
		protected override string TemplateUrl { get { return Control.OptionsToolTips.AppointmentToolTipUrl; } }
		protected override ToolTipCornerType CornerType { get { return Control.OptionsToolTips.AppointmentToolTipCornerType; } }
		protected override Control CreateDefaultForm() {
			return new DevExpress.Web.ASPxScheduler.Forms.Internal.AppointmentToolTip();
		}
	}
	public class ASPxSchedulerSelectionToolTtipControl : ASPxSchedulerToolTipControlBase {
		public ASPxSchedulerSelectionToolTtipControl(ASPxScheduler control)
			: base(control) {
		}
		public override string Name { get { return SchedulerFormNames.SelectionToolTip; } }
		public override bool CanShowToolTip { get { return Control.OptionsToolTips.ShowSelectionToolTip; } }
		protected override string TemplateUrl { get { return Control.OptionsToolTips.SelectionToolTipUrl; } }
		protected override ToolTipCornerType CornerType { get { return Control.OptionsToolTips.SelectionToolTipCornerType; } }
		protected override Control CreateDefaultForm() {
			return new DevExpress.Web.ASPxScheduler.Forms.Internal.SelectionToolTip();
		}
	}
}
namespace DevExpress.Web.ASPxScheduler {
	public class ASPxSchedulerToolTipBase : ASPxSchedulerClientScriptSupportUserControlBase {
		public virtual bool ToolTipShowStem { get { return true; } }
		public virtual bool ToolTipResetPositionByTimer { get { return true; } }
		public virtual bool ToolTipCloseOnClick { get { return false; } }
		protected internal virtual ImageProperties GetSmartTagImage() {
			TooltipTemplateContainer container = Parent as TooltipTemplateContainer;
			if(container == null)
				return new ImageProperties();
			ImageProperties image = new ImageProperties();
			ASPxSchedulerImages images = container.Control.Images;
			image.CopyFrom(images.GetImageProperties(Page, ASPxSchedulerImages.SmartTagName));
			image.CopyFrom(images.SmartTag);
			return image;
		}
		protected override void RenderInitialScript(System.Text.StringBuilder sb, string instanceName) {
			if(ToolTipResetPositionByTimer)
				sb.AppendFormat("{0}.resetPositionByTimer = true;\n", instanceName);
			if (ToolTipCloseOnClick)
				sb.AppendFormat("{0}.canCloseByClick = true;\n", instanceName);
		}
	}
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	public abstract class ToolTipWrapperBuilderBase {
		public static ToolTipWrapperBuilderBase CreateBuilder(ASPxScheduler control, ToolTipCornerType cornerType) {
			switch(cornerType) {
				case ToolTipCornerType.None:
					return new ToolTipEmptyWrapperBuilder(control);
				case ToolTipCornerType.Rounded:
					return new ToolTipRoundedCornersWrapperBuilder(control);
				case ToolTipCornerType.Square:
					return new ToolTipSquaredCornersWrapperBuilder(control);
			}
			return null;
		}
		WebControl toolTipContainerDiv;
		InternalTable layoutTable;
		WebControl stemCell;
		readonly ASPxScheduler control;
		protected ToolTipWrapperBuilderBase(ASPxScheduler control) {
			this.control = control;
		}
		protected WebControl ContentDiv { get { return toolTipContainerDiv; } set { toolTipContainerDiv = value; } }
		public WebControl StemControl { get { return stemCell; } }
		protected ASPxScheduler Control { get { return control; } }
		public WebControl CreateWrap(WebControl parent) {
			this.layoutTable = RenderUtils.CreateTable(true);
			parent.Controls.Add(layoutTable);
			WebControl toolTipContainerTableCell = CreateLayoutTableContent(this.layoutTable);
			this.toolTipContainerDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			toolTipContainerTableCell.Controls.Add(this.toolTipContainerDiv);
			this.stemCell = CreateDivWithImage(parent, ToolTipImages.BottomStemName);
			this.stemCell.ID = "BA";
			return ContentDiv;
		}
		protected internal virtual WebControl CreateDivWithImage(WebControl parent, string imageName) {
			WebControl div = RenderUtils.CreateDiv();
			parent.Controls.Add(div);
			ASPxSchedulerImageControl image = new ASPxSchedulerImageControl(GetImageProperties(imageName));
			div.Controls.Add(image);
			return div;
		}
		public virtual void PrepareWrap(bool showStem) {
			StemControl.Style.Add(HtmlTextWriterStyle.Position, "relative");
			StemControl.Style.Add(HtmlTextWriterStyle.Top, "-1px");
			if(!showStem)
				StemControl.Style.Add(HtmlTextWriterStyle.Display, "none");
		}
		protected internal virtual ImageProperties GetImageProperties(string imageName) {
			Page page = ASPxScheduler.ActiveControl != null ? ASPxScheduler.ActiveControl.Page : null;
			return Control.Images.ToolTip.GetImageProperties(page, imageName);
		}
		protected abstract WebControl CreateLayoutTableContent(InternalTable internalTable);
	}
	public class ToolTipSquaredCornersWrapperBuilder : ToolTipWrapperBuilderBase {
		TableCell toolTipContainerCell;
		public ToolTipSquaredCornersWrapperBuilder(ASPxScheduler control)
			: base(control) {
		}
		TableCell ToolTipContainerCell { get { return toolTipContainerCell; } }
		protected override WebControl CreateLayoutTableContent(InternalTable layoutTable) {
			TableRow row = RenderUtils.CreateTableRow();
			layoutTable.Controls.Add(row);
			this.toolTipContainerCell = RenderUtils.CreateTableCell();
			row.Controls.Add(this.toolTipContainerCell);
			return this.toolTipContainerCell;
		}
		public override void PrepareWrap(bool showStem) {
			base.PrepareWrap(showStem);
			ToolTipSquareCornerStyle style = Control.Styles.GetToolTipSquaredCornersStyle();
			style.AssignToControl(ToolTipContainerCell);
		}
	}
	public class ToolTipEmptyWrapperBuilder : ToolTipWrapperBuilderBase {
		TableCell toolTipContainerCell;
		public ToolTipEmptyWrapperBuilder(ASPxScheduler control)
			: base(control) {
		}
		protected override WebControl CreateLayoutTableContent(InternalTable layoutTable) {
			TableRow row = RenderUtils.CreateTableRow();
			layoutTable.Controls.Add(row);
			this.toolTipContainerCell = RenderUtils.CreateTableCell();
			row.Controls.Add(this.toolTipContainerCell);
			return this.toolTipContainerCell;
		}
	}
	public class ToolTipRoundedCornersWrapperBuilder : ToolTipWrapperBuilderBase {
		TableCell toolTipContainerCell;
		TableCell topMiddleCell;
		TableCell rightCell;
		TableCell leftCell;
		TableCell bottomMiddleCell;
		TableRow topRow;
		TableRow bottomRow;
		public ToolTipRoundedCornersWrapperBuilder(ASPxScheduler control)
			: base(control) {
		}
		protected override WebControl CreateLayoutTableContent(InternalTable layoutTable) {
			this.topRow = RenderUtils.CreateTableRow();
			layoutTable.Controls.Add(this.topRow);
			CreateCellWithImage(this.topRow, ToolTipImages.TopLeftName);
			this.topMiddleCell = CreateCellWithImage(this.topRow, ToolTipImages.EmptyName);
			CreateCellWithImage(this.topRow, ToolTipImages.TopRightName);
			TableRow middleRow = new InternalTableRow();
			layoutTable.Controls.Add(middleRow);
			this.leftCell = CreateCellWithImage(middleRow, ToolTipImages.EmptyName);
			this.toolTipContainerCell = RenderUtils.CreateTableCell();
			middleRow.Controls.Add(this.toolTipContainerCell);
			this.rightCell = CreateCellWithImage(middleRow, ToolTipImages.EmptyName);
			this.bottomRow = new InternalTableRow();
			layoutTable.Controls.Add(this.bottomRow);
			CreateCellWithImage(this.bottomRow, ToolTipImages.BottomLeftName);
			this.bottomMiddleCell = CreateCellWithImage(this.bottomRow, ToolTipImages.EmptyName);
			CreateCellWithImage(this.bottomRow, ToolTipImages.BottomRightName);
			return this.toolTipContainerCell;
		}
		public override void PrepareWrap(bool showStem) {
			base.PrepareWrap(showStem);
			ToolTipRoundedCornersStyles styles = Control.Styles.ToolTipRoundedCorners;
			ToolTipSideStyle topSideStyle = styles.GetTopSideStyle();
			topSideStyle.AssignToControl(this.topMiddleCell);
			ToolTipSideStyle leftSideStyle = styles.GetLeftSideStyle();
			leftSideStyle.AssignToControl(this.leftCell);
			ToolTipSideStyle rightSideStyle = styles.GetRightSideStyle();
			rightSideStyle.AssignToControl(this.rightCell);
			ToolTipSideStyle bottomSideStyle = styles.GetBottomSideStyle();
			bottomSideStyle.AssignToControl(this.bottomMiddleCell);
			AppearanceStyle contentStyle = styles.GetContentStyle();
			contentStyle.AssignToControl(ContentDiv);
			RenderUtils.AppendDefaultDXClassName(this.bottomRow, "dxscRndTTR");
			RenderUtils.AppendDefaultDXClassName(this.topRow, "dxscRndTTR");
		}
		protected internal virtual TableCell CreateCellWithImage(TableRow row, string imageName) {
			TableCell cell = RenderUtils.CreateTableCell();
			row.Controls.Add(cell);
			ASPxSchedulerImageControl image = new ASPxSchedulerImageControl(GetImageProperties(imageName));
			cell.Controls.Add(image);
			return cell;
		}
	}
	public class TooltipTemplateContainer : SchedulerFormTemplateContainer {
		public TooltipTemplateContainer(ASPxScheduler control)
			: base(control) {
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public override string CancelHandler {
			get { return "String.Empty"; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public override string CancelScript {
			get { return "String.Empty"; }
		}
		internal ASPxSchedulerToolTipBase Content {
			get {
				if(Controls.Count <= 0 || Controls.Count > 1)
					return null;
				return Controls[0] as ASPxSchedulerToolTipBase;
			}
		}
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return e;
		}
	}
}
