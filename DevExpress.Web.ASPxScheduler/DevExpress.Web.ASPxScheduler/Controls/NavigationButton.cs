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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.Web.ASPxScheduler.Internal {
	public abstract class ASPxSchedulerNavigationButton : ASPxInternalWebControl {
		#region Fields
		ASPxScheduler control;
		WebControl div;
		TimeInterval interval;
		XtraScheduler.Resource resource;
		NavigationButtonInternal button;
		#endregion
		protected ASPxSchedulerNavigationButton(ASPxScheduler control, TimeInterval interval, XtraScheduler.Resource resource) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			if (interval == null)
				interval = TimeInterval.Empty;
			if (resource == null)
				Exceptions.ThrowArgumentNullException("resource");
			this.control = control;
			this.interval = interval;
			this.resource = resource;
		}
		#region Properties
		public WebControl Div { get { return div; } }
		public TimeInterval Interval { get { return interval; } }
		public Resource Resource { get { return resource; } }
		public bool IsButtonEnabled { get { return !TimeInterval.Empty.Equals(interval); } }
		protected internal abstract string Text { get; }
		protected internal abstract AnchorType Anchor { get; }
		protected internal abstract ImagePosition ButtonImagePosition { get; }
		protected internal abstract string ButtonImagePrefix { get; }
		protected internal ASPxScheduler Control { get { return control; } }
		public string InnerId { get { return String.Format("{0}i", this.ID); ; } }
		#endregion
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.div = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(div);
			this.button = new NavigationButtonInternal();
			this.button.ParentSkinOwner = this.control;
			div.Controls.Add(button);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			div.ID = InnerId;			
			RenderUtils.SetStyleStringAttribute(div, "position", "absolute");
			RenderUtils.SetStyleStringAttribute(div, "top", "-100px");
			button.ControlStyle.CopyFrom(control.Styles.GetNavigationButtonStyle());
			button.Image.Assign(GetNavigationButtonImage());
			button.ImagePosition = ButtonImagePosition;
			button.Text = Text;
			button.ToolTip = ToolTip;
			button.Enabled = IsButtonEnabled;
			if (IsButtonEnabled) {
				string resourceId = SchedulerIdHelper.GenerateResourceId(resource);
				string handler = ASPxSchedulerScripts.GetNavigationButtonClickFunction(control.ClientID, interval, resourceId);
				SchedulerWebEventHelper.AddClickEvent(button, handler);
			}
			button.ID = div.ID + "_core" + (IsButtonEnabled ? "" : "D");
		}
		protected internal virtual ImageProperties GetNavigationButtonImage() {
			return control.Images.NavigationButton.GetImageProperties(Page, ButtonImagePrefix);
		}
	}
	public class ASPxSchedulerBackwardNavigationButton : ASPxSchedulerNavigationButton {
		public ASPxSchedulerBackwardNavigationButton(ASPxScheduler control, TimeInterval interval, XtraScheduler.Resource resource)
			: base(control, interval, resource) {
		}
		protected internal override string Text { get { return String.Empty; } }
		public override string ToolTip { get { return Control.OptionsView.NavigationButtons.PrevCaption; } set {} }
		protected internal override AnchorType Anchor { get { return AnchorType.Left; } }
		protected internal override ImagePosition ButtonImagePosition { get { return ImagePosition.Left; } }
		protected internal override string ButtonImagePrefix { get { return NavigationButtonImages.BackwardName; } }
	}
	public class ASPxSchedulerForwardNavigationButton : ASPxSchedulerNavigationButton {
		public ASPxSchedulerForwardNavigationButton(ASPxScheduler control, TimeInterval interval, XtraScheduler.Resource resource)
			: base(control, interval, resource) {
		}
		protected internal override string Text { get { return String.Empty; } }
		public override string ToolTip { get { return Control.OptionsView.NavigationButtons.NextCaption; } set { } }
		protected internal override AnchorType Anchor { get { return AnchorType.Right; } }
		protected internal override ImagePosition ButtonImagePosition { get { return ImagePosition.Right; } }
		protected internal override string ButtonImagePrefix { get { return NavigationButtonImages.ForwardName; } }
	}
	public class ASPxNavigationButtonCalculator : NavigationButtonCalculator {
		ASPxScheduler control;
		public ASPxNavigationButtonCalculator(ASPxScheduler control)
			: base(control.InnerControl, control.InnerControl.ActiveView) {
			this.control = control;
		}
		protected internal ASPxScheduler Control { get { return control; } }
		protected internal override AppointmentBaseCollection GetVisibleAppointments() {
			return control.ActiveView.FilteredAppointments;
		}
		protected internal override bool UseGroupByNoneCriteriaCalculator() {
			SchedulerViewBase view = control.ActiveView;
			return view.FactoryHelper.CalcActualGroupType(view) == SchedulerGroupType.None;
		}
	}
}
