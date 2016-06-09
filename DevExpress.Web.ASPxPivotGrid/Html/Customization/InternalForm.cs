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
using DevExpress.Web;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Web.ASPxPivotGrid.Html;
using System.Web.UI.WebControls;
using System.Collections;
namespace DevExpress.Web.ASPxPivotGrid.HtmlControls {
	[ToolboxItem(false)]
	public class PivotGridHtmlCustomizationFieldsPopup : DevExpress.Web.ASPxPopupControl {
		ASPxPivotGrid pivotGrid;
		PivotGridHtmlCustomizationFields customizationFields;
		int MinimumExcelStyleHeight = 400, MinimumExcelStyleWidth = 250;
		int MinimumSimpleStyleHeight = 170, MinimumSimpleStyleWidth = 150;
		int? left, top;
		public PivotGridHtmlCustomizationFieldsPopup(ASPxPivotGrid pivotGrid) {
			this.pivotGrid = pivotGrid;
			InitInternalValues();
			ParentSkinOwner = PivotGrid;
			ID = ElementNames.CustomizationFieldsId;
			AllowResize = OptionsCustomization.AllowCustomizationWindowResizing;
			AllowDragging = true;
			CloseAction = CloseAction.CloseButton;
			ClientSideEvents.PopUp = "function(s, e) { ASPx.pivotGrid_DoCustomizationFieldsVisibleChanged(s); }";
			ClientSideEvents.CloseUp = "function(s, e) { ASPx.pivotGrid_DoCustomizationFieldsVisibleChanged(s); }";
			ClientSideEvents.Shown = "function(s, e) { ASPx.pivotGrid_DoUpdateContentSize(s);  }";
			ClientSideEvents.BeforeResizing = "function(s, e) { ASPx.pivotGrid_DoResetContentSize(s); }";
			ClientSideEvents.AfterResizing = "function(s, e) { ASPx.pivotGrid_DoUpdateContentSize(s); }";
		}
		void InitInternalValues() {
			base.Left = 100;
			base.Top = 100;
		}
		public new int Left {
			get {
				if(left.HasValue)
					return left.Value;
				return base.Left;
			}
			set {
				base.Left = value;
				if(!PivotGrid.IsLoading())
					left = value;
			}
		}
		public new int Top {
			get {
				if(top.HasValue)
					return top.Value;
				return base.Top;
			}
			set {
				base.Top = value;
				if(!PivotGrid.IsLoading())
					top = value;
			}
		}
		public void Refresh() {
			ResetControlHierarchy();
		}
		protected ASPxPivotGrid PivotGrid { get { return pivotGrid; } }
		protected PivotGridWebData Data { get { return PivotGrid.Data; } }
		protected PivotGridWebOptionsCustomization OptionsCustomization { get { return Data.OptionsCustomization; } }
		protected internal PivotGridHtmlCustomizationFields CustomizationFields {
			get {
				if(customizationFields == null)
					customizationFields = CreateCustomizationFields();
				return customizationFields; 
			}
		}
		protected virtual PivotGridHtmlCustomizationFields CreateCustomizationFields() {
			return new PivotGridHtmlCustomizationFields(PivotGrid);
		}
		protected int WindowWidth {
			get {
				return OptionsCustomization.CustomizationFormStyle == CustomizationFormStyle.Simple ?
						OptionsCustomization.CustomizationWindowWidth : OptionsCustomization.CustomizationExcelWindowWidth;
			}
			set {
				if(OptionsCustomization.CustomizationFormStyle == CustomizationFormStyle.Simple)
					OptionsCustomization.CustomizationWindowWidth = value;
				else
					OptionsCustomization.CustomizationExcelWindowWidth = value;
			}
		}
		protected int WindowHeight {
			get {
				return OptionsCustomization.CustomizationFormStyle == CustomizationFormStyle.Simple ?
						  OptionsCustomization.CustomizationWindowHeight : OptionsCustomization.CustomizationExcelWindowHeight;
			}
			set {
				if(OptionsCustomization.CustomizationFormStyle == CustomizationFormStyle.Simple)
					OptionsCustomization.CustomizationWindowHeight = value;
				else
					OptionsCustomization.CustomizationExcelWindowHeight = value;
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			MinHeight = OptionsCustomization.CustomizationFormStyle == CustomizationFormStyle.Simple ? MinimumSimpleStyleHeight : MinimumExcelStyleHeight;
			MinWidth = OptionsCustomization.CustomizationFormStyle == CustomizationFormStyle.Simple ? MinimumSimpleStyleWidth : MinimumExcelStyleWidth;
			ApplyInternalCashedValues();
		}
		void ApplyInternalCashedValues() {
			if(left.HasValue)
				base.Left = left.Value;
			if(top.HasValue)
				base.Top = top.Value;
		}
		protected override Paddings GetContentPaddings(PopupWindow window) {
			return new Paddings(0);
		}
		public void ReadPostData() {
			if(Request == null) return;
			CustomizationFields.ReadPostData();
			InitParameters();
			if(Height.Value == 0 || Width.Value == 0)
				return;
			WindowHeight = Convert.ToInt32(Height.Value);
			WindowWidth = Convert.ToInt32(Width.Value);
		}
		protected void InitParameters() {
			if(ClientObjectState == null) return;
			EnsureDataBound();
			string stateString = GetClientObjectStateValue<string>(WindowsStateKey);
			if(!string.IsNullOrEmpty(stateString))
				LoadWindowsState(stateString);
		}
		protected override string GetClientObjectStateInputID() {
			string id = base.GetClientObjectStateInputID();
			if(id.LastIndexOf("$") < 0)
				id = PivotGrid.UniqueID + "$" + id;
			return id;
		}
		protected override void CreateControlHierarchy() {
			ScrollBars = ScrollBars.None;
			HeaderText = PivotGridLocalizer.GetString(PivotGridStringId.CustomizationFormCaption);
			customizationFields = CreateCustomizationFields();
			Controls.Clear();
			Controls.Add(CustomizationFields);
			base.CreateControlHierarchy();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Height = WindowHeight;
			Width = WindowWidth;
			CustomizationFields.Style[System.Web.UI.HtmlTextWriterStyle.Display] = "none";
			CloseButtonImage.CopyFrom(PivotGrid.RenderHelper.GetCustomizationFieldsCloseImage());
			ControlStyle.CopyFrom(PivotGrid.Styles.CustomizationFieldsStyle);
			CloseButtonStyle.CopyFrom(PivotGrid.Styles.CustomizationFieldsCloseButtonStyle);
			HeaderStyle.CopyFrom(PivotGrid.Styles.GetCustomizationFieldsHeaderStyle());
			ContentStyle.CopyFrom(PivotGrid.Styles.GetCustomizationFieldsContentStyle());
		}
		internal void PrepareLayoutMenu() {
			CustomizationFields.PrepareLayoutMenu();
		}
		internal object[] GetLayoutState() {
			return new object[] { 
				ShowOnPageLoad,
				Width.ToString(),
				Height.ToString(),
				base.Left,
				base.Top,
			};
		}
		internal void ApplyLayoutState(object[] state) {
			if(state.Length != 5)
				return;
			ShowOnPageLoad = (bool)state[0];
			Width = Unit.Parse(state[1] as string);
			Height = Unit.Parse(state[2] as string);
			Left = (int)state[3];
			Top = (int)state[4];
		}
	}
}
