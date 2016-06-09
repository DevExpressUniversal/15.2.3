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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxSpreadsheet.Localization;
using DevExpress.Web.Internal;
using System.Drawing.Printing;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Forms;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Forms {
	public class PageSetupDialog : SpreadsheetDialogBase {
		protected static class ClassNames {
			public const string Dialog = "dxssDlgPageSetupForm";
			public const string Container = "dxssPSContainer";
			public const string PageControl = "dxssPageControl";
			public const string PrintContainer = "dxssPSPrintContainer";
			public const string CompactLayoutItem = "dxssCompactLayoutItem";
			public const string CompactGroupBox = "dxssCompactGroupBox";
		}
		protected ASPxPageControl PageControl { get; private set; }
		protected Control Container { get; private set; }
		protected Control PrintContainer { get; private set; }
		protected PSPageTab PageTab { get; private set; }
		protected PSMarginsTab MarginsTab { get; private set; }
		protected PSHeaderFooterTab HeaderFooterTab { get; private set; }
		protected PSSheetTab SheetTab { get; private set; }
		protected ASPxButton Print { get; private set; }
		protected override string GetDialogCssClassName() {
			return ClassNames.Dialog;
		}
		protected override string GetContentTableID() {
			return "dxPageSetupForm";
		}
		protected const string PrintClickClientHandler =
				"function(s, e) {{ ASPx.SpreadsheetDialog.PrintClicked(); }}";
		protected override void PopulateContentArea(Control container) {
			InitializePageControl();
			PrintContainer = RenderUtils.CreateDiv(ClassNames.PrintContainer);
			Print = new ASPxButton() {
				Text = Localize(ASPxSpreadsheetStringId.PageSetup_Print),
				CssClass = SpreadsheetStyles.DialogFooterButtonCssClass,
				AutoPostBack = false
			};
			Print.ClientSideEvents.Click = PrintClickClientHandler;
			PrintContainer.Controls.Add(Print);
			Container = RenderUtils.CreateDiv(ClassNames.Container);
			Container.Controls.Add(PageControl);
			container.Controls.Add(Container);
			container.Controls.Add(PrintContainer);
		}
		protected void InitializePageControl() {
			PageControl = new ASPxPageControl();
			PageControl.CssClass = ClassNames.PageControl;
			PageControl.Border.BorderWidth = Unit.Pixel(0);
			PageControl.ContentStyle.Border.BorderWidth = Unit.Pixel(0);
			PageControl.Width = Unit.Percentage(100);
			PageControl.Height = Unit.Percentage(100);
			PageControl.EnableTabScrolling = true;
			PageTab = new PSPageTab(this);
			MarginsTab = new PSMarginsTab(this);
			HeaderFooterTab = new PSHeaderFooterTab(this);
			SheetTab = new PSSheetTab(this);
			PageControl.TabPages.Add(PageTab);
			PageControl.TabPages.Add(MarginsTab);
			PageControl.TabPages.Add(HeaderFooterTab);
			PageControl.TabPages.Add(SheetTab);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
		}
		protected override void ApplyLocalization() {
			base.ApplyLocalization();
		}
		public string Localize(ASPxSpreadsheetStringId stringId) {
			return ASPxSpreadsheetLocalizer.GetString(stringId);
		}
		protected override ASPxButton[] GetChildDxButtons() {
			return base.GetChildDxButtons();
		}
		protected override ASPxEditBase[] GetChildDxEdits() {
			List<ASPxEditBase> baseCollection = new List<ASPxEditBase>(base.GetChildDxEdits());
			baseCollection.AddRange(PageTab.GetChildDxEdits());
			baseCollection.AddRange(MarginsTab.GetChildDxEdits());
			baseCollection.AddRange(HeaderFooterTab.GetChildDxEdits());
			baseCollection.AddRange(SheetTab.GetChildDxEdits());
			return baseCollection.ToArray();
		}
		protected abstract class PageSetupTabBase : TabPage {
			protected abstract string DefaultText { get; }
			protected ASPxFormLayout FormLayout { get; set; }
			protected PageSetupDialog Owner { get; private set; }
			protected const string ValidationGroupBase = "_dxPageSetup";
			public PageSetupTabBase(PageSetupDialog owner)
				: base() {
					Owner = owner;
					Text = DefaultText;
			}
			public abstract List<ASPxEditBase> GetChildDxEdits();
			protected override ContentControlCollection CreateContentControlCollection(Control ownerControl) {
				ContentControlCollection controlCollection = base.CreateContentControlCollection(ownerControl);
				ContentControl content = new ContentControl();
				CreateFormLayout();
				content.Controls.Add(FormLayout);
				controlCollection.Add(content);
				return controlCollection;
			}
			protected void CreateFormLayout() {
				InitializeFormLayout();
				CreateFormControls();
				PopulateFormLayout();
			}
			protected virtual void InitializeFormLayout() {
				FormLayout = new ASPxFormLayout() {
					Width = Unit.Percentage(100)
				};
			}
			protected abstract void CreateFormControls();
			protected abstract void PopulateFormLayout();
			protected void AddControlToLayoutGroup(Control control, LayoutGroup group, Action<LayoutItem> layoutItemAction = null) {
				LayoutItem layoutItem = new LayoutItem() {
					ShowCaption = Utils.DefaultBoolean.False
				};
				layoutItem.Controls.Add(control);
				if(layoutItemAction != null)
					layoutItemAction(layoutItem);
				group.Items.Add(layoutItem);
			}
			protected ASPxCheckBox CreateCheckBox(ASPxSpreadsheetStringId stringId, string clientName) {
				return new ASPxCheckBox() {
					Text = Localize(stringId),
					ClientInstanceName = GetControlClientInstanceName(clientName)
				};
			}
			protected string GetControlClientInstanceName(string controlName) {
				return DialogUtils.GetControlClientInstanceName(Owner, controlName);
			}
			protected string Localize(ASPxSpreadsheetStringId stringId) {
				return Owner.Localize(stringId);
			}
		}
		protected class PSPageTab : PageSetupTabBase {
			protected static class ControlID {
				public const string AdjustTo = "cbAdjustTo";
				public const string FitTo = "cbFitTo";
				public const string Scale = "seScale";
				public const string FitToWidth = "seFitToWidth";
				public const string FitToHeight = "seFitToHeight";
				public const string PaperSize = "cbPaperSize";
				public const string PrintQuality = "cbPrintQuality";
				public const string FirstPageNumber = "tbFirstPageNumber";
			}
			protected static class ControlClientName {
				public const string Orientation = "_rlOrientationPortrait";
				public const string AdjustTo = "_rbAdjustTo";
				public const string FitToPage = "_rbFitToPage";
				public const string Scale = "_seScale";
				public const string FitToWidth = "_seFitToWidth";
				public const string FitToHeight = "_seFitToHeight";
				public const string PaperType = "_cbPaperType";
				public const string PrintQuality = "_cbPrintQualityMode";
				public const string FirstPageNumber = "_tbFirstPageNumber";
			}
			protected static class StringID {
				public const ASPxSpreadsheetStringId Caption = ASPxSpreadsheetStringId.PageSetup_PageTab_Caption;
				public const ASPxSpreadsheetStringId OrientationGroup = ASPxSpreadsheetStringId.PageSetup_PageTab_OrientationGroup;
				public const ASPxSpreadsheetStringId OrientationPortrait = ASPxSpreadsheetStringId.PageSetup_PageTab_OrientationPortrait;
				public const ASPxSpreadsheetStringId OrientationLandscape = ASPxSpreadsheetStringId.PageSetup_PageTab_OrientationLandscape;
				public const ASPxSpreadsheetStringId ScalingGroup = ASPxSpreadsheetStringId.PageSetup_PageTab_ScalingGroup;
				public const ASPxSpreadsheetStringId AdjustTo = ASPxSpreadsheetStringId.PageSetup_PageTab_AdjustTo;
				public const ASPxSpreadsheetStringId Scale = ASPxSpreadsheetStringId.PageSetup_PageTab_Scale;
				public const ASPxSpreadsheetStringId FitTo = ASPxSpreadsheetStringId.PageSetup_PageTab_FitTo;
				public const ASPxSpreadsheetStringId FitToWidth = ASPxSpreadsheetStringId.PageSetup_PageTab_FitToWidth;
				public const ASPxSpreadsheetStringId FitToHeight = ASPxSpreadsheetStringId.PageSetup_PageTab_FitToHeight;
				public const ASPxSpreadsheetStringId PaperSize = ASPxSpreadsheetStringId.PageSetup_PageTab_PaperSize;
				public const ASPxSpreadsheetStringId PrintQuality = ASPxSpreadsheetStringId.PageSetup_PageTab_PrintQuality;
				public const ASPxSpreadsheetStringId FirstPageNumber = ASPxSpreadsheetStringId.PageSetup_PageTab_FirstPageNumber;
			}
			const string FitToPageGroupName = "PageSetup_FitToPageGroup";
			protected override string DefaultText {
				get {
					return Localize(StringID.Caption);
				}
			}
			#region FormLayout controls
			protected LayoutGroup OrientationGroup { get; private set; }
			protected LayoutGroup ScalingGroup { get; private set; }
			protected LayoutGroup PrintGroup { get; private set; }
			protected LayoutGroup FirstPageGroup { get; private set; }
			protected ASPxLabel OrientationLabel { get; private set; }
			protected ASPxRadioButtonList Orientation { get; private set; }
			protected ASPxRadioButton AdjustTo { get; private set; }
			protected ASPxRadioButton FitTo { get; private set; }
			protected ASPxLabel ScaleLabel { get; private set; }
			protected ASPxLabel FitToWidthLabel { get; private set; }
			protected ASPxLabel FitToHeightLabel { get; private set; }
			protected ASPxSpinEdit Scale { get; private set; }
			protected ASPxSpinEdit FitToWidth { get; private set; }
			protected ASPxSpinEdit FitToHeight { get; private set; }
			protected ASPxComboBox PaperSize { get; private set; }
			protected ASPxComboBox PrintQuality { get; private set; }
			protected ASPxTextBox FirstPageNumber { get; private set; }
			public override List<ASPxEditBase> GetChildDxEdits() {
				return new List<ASPxEditBase>() {
					AdjustTo, FitTo, Scale, FitToWidth, FitToHeight, PaperSize, PrintQuality, FirstPageNumber
				};
			}
			#endregion
			public PSPageTab(PageSetupDialog owner)
				: base(owner) {				
			}
			protected override void CreateFormControls() {
				#region Orientation group controls
				Orientation = new ASPxRadioButtonList();
				Orientation.ClientInstanceName = GetControlClientInstanceName(ControlClientName.Orientation);
				Orientation.ClientSideEvents.SelectedIndexChanged = GetOrientationChangedClientHandler();
				Orientation.Border.BorderWidth = Unit.Pixel(0);
				Orientation.Items.Add(Localize(StringID.OrientationPortrait));
				Orientation.Items.Add(Localize(StringID.OrientationLandscape));
				#endregion
				#region Scaling group controls
				AdjustTo = new ASPxRadioButton() {
					ID = ControlID.AdjustTo,
					ClientInstanceName = GetControlClientInstanceName(ControlClientName.AdjustTo),
					Text = Localize(StringID.AdjustTo),
					Wrap = Utils.DefaultBoolean.False,
					GroupName = FitToPageGroupName
				};
				Scale = new ASPxSpinEdit() {
					ID = ControlID.Scale,
					ClientInstanceName = GetControlClientInstanceName(ControlClientName.Scale),
					Width = Unit.Pixel(70),
				};
				ScaleLabel = new ASPxLabel() {
					Text = Localize(StringID.Scale),
					Wrap = Utils.DefaultBoolean.False,
					AssociatedControlID = ControlID.Scale
				};
				FitTo = new ASPxRadioButton() {
					ID = ControlID.FitTo,
					ClientInstanceName = GetControlClientInstanceName(ControlClientName.FitToPage),
					Text = Localize(StringID.FitTo),
					Wrap = Utils.DefaultBoolean.False,
					GroupName = FitToPageGroupName
				};
				FitToWidthLabel = new ASPxLabel() {
					Text = Localize(StringID.FitToWidth),
					AssociatedControlID = ControlID.FitToWidth,
					Wrap = Utils.DefaultBoolean.False
				};
				FitToWidth = new ASPxSpinEdit() {
					ID = ControlID.FitToWidth,
					ClientInstanceName = GetControlClientInstanceName(ControlClientName.FitToWidth),
					Width = Unit.Pixel(70)
				};
				FitToHeightLabel = new ASPxLabel() {
					Text = Localize(StringID.FitToHeight),
					AssociatedControlID = ControlID.FitToHeight
				};
				FitToHeight = new ASPxSpinEdit() {
					ID = ControlID.FitToHeight,
					ClientInstanceName = GetControlClientInstanceName(ControlClientName.FitToHeight),
					Width = Unit.Pixel(70)
				};
				#endregion
				#region Printing group controls
				InitializePaperSizeComboBox();
				InitializePrintQualityComboBox();
				#endregion
				#region First page group controls
				FirstPageNumber = new ASPxTextBox() {
					ID = ControlID.FirstPageNumber,
					ClientInstanceName = GetControlClientInstanceName(ControlClientName.FirstPageNumber),
					Width = Unit.Pixel(100)
				};
				#endregion
			}
			protected void InitializePaperSizeComboBox() {
				PaperSize = new ASPxComboBox() {
					ID = ControlID.PaperSize,
					ClientInstanceName = GetControlClientInstanceName(ControlClientName.PaperType),
					Width = Unit.Percentage(100)
				};
				IList<PaperKind> defaultPaperKind = PageSetupSetPaperKindCommand.DefaultPaperKindList;
				foreach(PaperKind kind in defaultPaperKind) 
					PaperSize.Items.Add(kind.ToString());
			}
			protected void InitializePrintQualityComboBox() {
				PrintQuality = new ASPxComboBox() {
					ID = ControlID.PrintQuality,
					ClientInstanceName = GetControlClientInstanceName(ControlClientName.PrintQuality),
					Width = Unit.Percentage(100)
				};
				var printQualityList = PageSetupViewModel.DefaultPrintQualityList;
				foreach(KeyValuePair<int, string> entry in printQualityList)
					PrintQuality.Items.Add(entry.Value, entry.Key);
			}
			protected override void PopulateFormLayout() {
				#region OrientationGroup
				OrientationGroup = new LayoutGroup() {
					Caption = Localize(StringID.OrientationGroup),
					GroupBoxDecoration = GroupBoxDecoration.HeadingLine,
					UseDefaultPaddings = false
				};
				OrientationGroup.GroupBoxStyle.CssClass = ClassNames.CompactGroupBox;
				AddControlToLayoutGroup(Orientation, OrientationGroup, i => i.CssClass = ClassNames.CompactLayoutItem);
				#endregion
				#region ScalingGroup
				ScalingGroup = new LayoutGroup() {
					Caption = Localize(StringID.ScalingGroup),
					GroupBoxDecoration = GroupBoxDecoration.HeadingLine,
					ColCount = 5,
					UseDefaultPaddings = false
				};
				ScalingGroup.GroupBoxStyle.CssClass = ClassNames.CompactGroupBox;
				ScalingGroup.Paddings.PaddingLeft = Unit.Pixel(20);
				ScalingGroup.Paddings.PaddingRight = Unit.Pixel(20);
				AddControlToLayoutGroup(AdjustTo, ScalingGroup, i => i.CssClass = ClassNames.CompactLayoutItem);
				AddControlToLayoutGroup(Scale, ScalingGroup);
				AddControlToLayoutGroup(ScaleLabel, ScalingGroup, i => i.ColSpan = 3);
				AddControlToLayoutGroup(FitTo, ScalingGroup);
				AddControlToLayoutGroup(FitToWidth, ScalingGroup);
				AddControlToLayoutGroup(FitToWidthLabel, ScalingGroup);
				AddControlToLayoutGroup(FitToHeight, ScalingGroup);
				AddControlToLayoutGroup(FitToHeightLabel, ScalingGroup);
				#endregion
				#region PrintGroup
				PrintGroup = new LayoutGroup() {
					ShowCaption = Utils.DefaultBoolean.False,
					GroupBoxDecoration = GroupBoxDecoration.HeadingLine,
					UseDefaultPaddings = false
				};
				PrintGroup.Paddings.PaddingLeft = Unit.Pixel(0);
				PrintGroup.Paddings.PaddingRight = Unit.Pixel(0);
				LayoutItem layoutItem = new LayoutItem() {
					Caption = Localize(StringID.PaperSize)
				};
				layoutItem.Controls.Add(PaperSize);
				PrintGroup.Items.Add(layoutItem);
				layoutItem = new LayoutItem() {
					Caption = Localize(StringID.PrintQuality)
				};
				layoutItem.Controls.Add(PrintQuality);
				PrintGroup.Items.Add(layoutItem);
				#endregion
				#region FirstPageGroup
				FirstPageGroup = new LayoutGroup() {
					ShowCaption = Utils.DefaultBoolean.False,
					GroupBoxDecoration = GroupBoxDecoration.None,
					UseDefaultPaddings = false,
					ColCount = 3
				};
				layoutItem = new LayoutItem() {
					Caption = Localize(StringID.FirstPageNumber)
				};
				layoutItem.Controls.Add(FirstPageNumber);
				FirstPageGroup.Items.Add(layoutItem);
				#endregion
				FormLayout.Items.Add(OrientationGroup);
				FormLayout.Items.Add(ScalingGroup);
				FormLayout.Items.Add(PrintGroup);
				FormLayout.Items.Add(FirstPageGroup);
			}
			protected string GetOrientationChangedClientHandler() {
				return "function(s, e) {{ ASPx.SpreadsheetDialog.OnOrientationChanged(s.GetSelectedIndex() === 0); }}";
			}
		}
		protected class PSMarginsTab : PageSetupTabBase {
			protected static class ControlID {
				public const string MarginLeft = "seMarginLeft";
				public const string MarginTop = "seMarginTop";
				public const string MarginHeader = "seMarginHeader";
				public const string MarginRight = "seMarginRight";
				public const string MarginFooter = "seMarginFooter";
				public const string MarginBottom = "seMarginBottom";
			}
			protected static class ControlClientName {
				public const string MarginLeft = "_seLeftMargin";
				public const string MarginTop = "_seTopMargin";
				public const string MarginHeader = "_seHeaderMargin";
				public const string MarginRight = "_seRightMargin";
				public const string MarginFooter = "_seFooterMargin";
				public const string MarginBottom = "_seBottomMargin";
				public const string CenterHorizontally = "_ckCenterHorizontally";
				public const string CenterVertically = "_ckCenterVertically";
			}
			protected static class StringID {
				public const ASPxSpreadsheetStringId Caption = ASPxSpreadsheetStringId.PageSetup_MarginsTab_Caption;
				public const ASPxSpreadsheetStringId Left = ASPxSpreadsheetStringId.PageSetup_MarginsTab_Left;
				public const ASPxSpreadsheetStringId Top = ASPxSpreadsheetStringId.PageSetup_MarginsTab_Top;
				public const ASPxSpreadsheetStringId Header = ASPxSpreadsheetStringId.PageSetup_MarginsTab_Header;
				public const ASPxSpreadsheetStringId Right = ASPxSpreadsheetStringId.PageSetup_MarginsTab_Right;
				public const ASPxSpreadsheetStringId Footer = ASPxSpreadsheetStringId.PageSetup_MarginsTab_Footer;
				public const ASPxSpreadsheetStringId Bottom = ASPxSpreadsheetStringId.PageSetup_MarginsTab_Bottom;
				public const ASPxSpreadsheetStringId CenterOnPageGroup = ASPxSpreadsheetStringId.PageSetup_MarginsTab_CenterOnPageGroup;
				public const ASPxSpreadsheetStringId CenterHorizontally = ASPxSpreadsheetStringId.PageSetup_MarginsTab_CenterHorizontally;
				public const ASPxSpreadsheetStringId CenterVertically = ASPxSpreadsheetStringId.PageSetup_MarginsTab_CenterVertically;
			}
			private const string
				PreviewTableCssClassName = "dxssPreviewTable";
			protected const string seFocusChangedHandlerTemplate = 
				"function(s, e) {{ ASPx.SpreadsheetDialog.MarginEditorFocusChanged(\"{0}\", \"{1}\", {2}); }}";
			protected const string cbCenterOnPageChangedHandlerTemplate =
				"function(s, e) {{ ASPx.SpreadsheetDialog.CenterOnPageChanged({0}, s.GetChecked()); }}";
			#region FormLayout controls
			protected LayoutGroup MarginsPreviewGroup { get; private set; }
			protected LayoutGroup CenterOnPageGroup { get; private set; }
			protected HtmlTable PreviewTable { get; private set; }
			protected MarginsPreviewAreaControl MarginsPreviewArea { get; private set; }
			protected ASPxSpinEdit MarginLeft { get; private set; }
			protected ASPxSpinEdit MarginTop { get; private set; }
			protected ASPxSpinEdit MarginHeader { get; private set; }
			protected ASPxSpinEdit MarginRight { get; private set; }
			protected ASPxSpinEdit MarginFooter { get; private set; }
			protected ASPxSpinEdit MarginBottom { get; private set; }
			protected ASPxCheckBox CenterHorizontally { get; private set; }
			protected ASPxCheckBox CenterVertically { get; private set; }
			public override List<ASPxEditBase> GetChildDxEdits() {
				return new List<ASPxEditBase>() {
					MarginLeft, MarginTop, MarginHeader, MarginRight, MarginFooter, MarginBottom, CenterHorizontally, CenterVertically
				};
			}
			#endregion
			protected override string DefaultText {
				get {
					return Localize(StringID.Caption);
				}
			}
			public PSMarginsTab(PageSetupDialog owner)
				: base(owner) {
			}
			protected override void CreateFormControls() {
				CreatePreviewTableControls();
				CenterHorizontally = new ASPxCheckBox() {
					Text = Localize(StringID.CenterHorizontally),
					ClientInstanceName = GetControlClientInstanceName(ControlClientName.CenterHorizontally)
				};
				CenterVertically = new ASPxCheckBox() {
					Text = Localize(StringID.CenterVertically),
					ClientInstanceName = GetControlClientInstanceName(ControlClientName.CenterVertically)
				};
				CenterHorizontally.ClientSideEvents.CheckedChanged = GetCenterOnPageValueChangedClientHandler(false);
				CenterVertically.ClientSideEvents.CheckedChanged = GetCenterOnPageValueChangedClientHandler(true);
			}
			protected override void PopulateFormLayout() {
				MarginsPreviewGroup = new LayoutGroup() {
					ShowCaption = Utils.DefaultBoolean.False,
					GroupBoxDecoration = GroupBoxDecoration.None,
					UseDefaultPaddings = false
				};
				AddControlToLayoutGroup(PreviewTable, MarginsPreviewGroup);
				CenterOnPageGroup = new LayoutGroup() {
					Caption = Localize(StringID.CenterOnPageGroup),
					GroupBoxDecoration = GroupBoxDecoration.HeadingLine,
					UseDefaultPaddings = false
				};
				AddControlToLayoutGroup(CenterHorizontally, CenterOnPageGroup, i => i.CssClass = ClassNames.CompactLayoutItem);
				AddControlToLayoutGroup(CenterVertically, CenterOnPageGroup);
				FormLayout.Items.Add(MarginsPreviewGroup);
				FormLayout.Items.Add(CenterOnPageGroup);
			}
			protected void CreatePreviewTable() {
				PreviewTable = new HtmlTable();
				PreviewTable.Attributes.Add("class", PreviewTableCssClassName);
				string className = PreviewTableCssClassName + " dxssVOrientation";
				PreviewTable.Attributes.Add("class", className);
				PreviewTable.Align = "center";
			}
			protected void CreatePreviewTableControls() {
				CreatePreviewTable();			  
				MarginsPreviewArea = new MarginsPreviewAreaControl();
				CreateSpinEdits();
				HtmlTableRow row = CreatePreviewRow();
				AddControlToPreviewRow(row, null);
				AddControlToPreviewRow(row, MarginTop);
				AddControlToPreviewRow(row, MarginHeader);
				row = CreatePreviewRow();
				AddControlToPreviewRow(row, MarginLeft);
				AddControlToPreviewRow(row, MarginsPreviewArea);
				AddControlToPreviewRow(row, MarginRight);
				row = CreatePreviewRow();
				AddControlToPreviewRow(row, null);
				AddControlToPreviewRow(row, MarginBottom);
				AddControlToPreviewRow(row, MarginFooter);
			}
			protected void CreateSpinEdits() {
				MarginLeft = CreateSpinEdit(ControlClientName.MarginLeft, StringID.Left, MarginsPreviewAreaControl.ClassNames.LeftRightLinesBox, "left");
				MarginTop = CreateSpinEdit(ControlClientName.MarginTop, StringID.Top, MarginsPreviewAreaControl.ClassNames.BottomTopLinesBox, "top");
				MarginHeader = CreateSpinEdit(ControlClientName.MarginHeader, StringID.Header, MarginsPreviewAreaControl.ClassNames.FooterHeaderLinesBox, "top");
				MarginRight = CreateSpinEdit(ControlClientName.MarginRight, StringID.Right, MarginsPreviewAreaControl.ClassNames.LeftRightLinesBox, "right");
				MarginFooter = CreateSpinEdit(ControlClientName.MarginFooter, StringID.Footer, MarginsPreviewAreaControl.ClassNames.FooterHeaderLinesBox, "bottom");
				MarginBottom = CreateSpinEdit(ControlClientName.MarginBottom, StringID.Bottom, MarginsPreviewAreaControl.ClassNames.BottomTopLinesBox, "bottom");
			}
			protected ASPxSpinEdit CreateSpinEdit(string controlName, ASPxSpreadsheetStringId stringId, string boxClassName, string borderLocation) {
				var spinEdit = new ASPxSpinEdit() {
					ClientInstanceName = GetControlClientInstanceName(controlName),
					Caption = Localize(stringId) + DialogUtils.LabelEndMark,
					Increment = 0.25m,
					MinValue = 0m,
					MaxValue = 1000m,
					Width = Unit.Pixel(70)					
				};
				spinEdit.ClientSideEvents.GotFocus = GetSpinEditGotFocusClientHandler(boxClassName, borderLocation);
				spinEdit.ClientSideEvents.LostFocus = GetSpinEditLostFocusClientHandler(boxClassName, borderLocation);
				spinEdit.CaptionSettings.Position = EditorCaptionPosition.Top;
				return spinEdit;
			}
			protected string GetSpinEditGotFocusClientHandler(string className, string borderLocation) {
				return GetSpinEditFocusChangedClientHandler(className, borderLocation, true);
			}
			protected string GetSpinEditLostFocusClientHandler(string className, string borderLocation) {
				return GetSpinEditFocusChangedClientHandler(className, borderLocation, false);
			}
			protected string GetSpinEditFocusChangedClientHandler(string className, string borderLocation, bool focused) {
				return String.Format(seFocusChangedHandlerTemplate, className, borderLocation, focused ? "true" : "false");
			}
			protected string GetCenterOnPageValueChangedClientHandler(bool isVertical) {
				return String.Format(cbCenterOnPageChangedHandlerTemplate, isVertical ? "true" : "false");
			}
			protected HtmlTableRow CreatePreviewRow() {
				HtmlTableRow row = new HtmlTableRow();
				PreviewTable.Rows.Add(row);
				return row;
			}
			protected void AddControlToPreviewRow(HtmlTableRow row, Control control) {
				HtmlTableCell cell = new HtmlTableCell();
				row.Cells.Add(cell);
				if(control != null)
					cell.Controls.Add(control);
			}
			protected class MarginsPreviewAreaControl : ASPxInternalWebControl {
				protected const string CellsGridID = "cellsGrid";
				public static class ClassNames {
					public const string PreviewArea = "dxssMPArea";
					public const string CellsGrid = "dxssMPCG";
					public const string VerticalAlignHelper = "dxssVHelper";
					public const string LinesBoxBase = "dxssLinesBox";
					public const string LeftRightLinesBox = LinesBoxBase + "LR";
					public const string BottomTopLinesBox = LinesBoxBase + "BT";
					public const string FooterHeaderLinesBox = LinesBoxBase + "FH";
				}
				protected WebControl CellsGrid { get; private set; }
				protected WebControl VerticalAlignHelper { get; private set; }
				protected WebControl LeftRightLinesBox { get; private set; }
				protected WebControl BottomTopLinesBox { get; private set; }
				protected WebControl FooterHeaderLinesBox { get; private set; }
				public MarginsPreviewAreaControl()
					: base() {
						CssClass = ClassNames.PreviewArea;
				}
				protected override bool HasRootTag() {
					return true;
				}
				protected override HtmlTextWriterTag TagKey {
					get {
						return HtmlTextWriterTag.Div;
					}
				}
				protected override void CreateControlHierarchy() {
					VerticalAlignHelper = CreateVerticalAlignHelper();
					CellsGrid = CreateCellsGrid();
					LeftRightLinesBox = CreateLinesBox(ClassNames.LeftRightLinesBox);
					BottomTopLinesBox = CreateLinesBox(ClassNames.BottomTopLinesBox);
					FooterHeaderLinesBox = CreateLinesBox(ClassNames.FooterHeaderLinesBox);
				}
				protected WebControl CreateLinesBox(string cssClass) {
					WebControl box = RenderUtils.CreateDiv();
					box.CssClass = cssClass;
					RenderUtils.AppendDefaultDXClassName(box, ClassNames.LinesBoxBase);
					return box;
				}
				protected WebControl CreateCellsGrid() {
					return new WebControl(HtmlTextWriterTag.Div) {
						ID = CellsGridID,
						CssClass = ClassNames.CellsGrid
					};
				}
				protected WebControl CreateVerticalAlignHelper() {
					return new WebControl(HtmlTextWriterTag.Span) {
						CssClass = ClassNames.VerticalAlignHelper
					};
				}
				protected override void PrepareControlHierarchy() {
					Controls.Add(CreateVerticalAlignHelper());
					Controls.Add(CellsGrid);
					Controls.Add(LeftRightLinesBox);
					Controls.Add(BottomTopLinesBox);
					Controls.Add(FooterHeaderLinesBox);
				}
			}
		}
		protected class PSHeaderFooterTab : PageSetupTabBase {
			protected static class ClassNames {
				public const string HeaderPreview = "dxssPSHeader";
				public const string FooterPreview = "dxssPSFooter";
			}
			protected static class ControlClientName {
				public const string Header = "_cbHeader";
				public const string Footer = "_cbFooter";
				public const string DifferentOddAndEvenPages = "_ckDifferentOddEven";
				public const string DifferentFirstPage = "_ckDifferentFirstPage";
				public const string ScaleWithDocument = "_ckScaleWithDocument";
				public const string AlignWithPageMargins = "_ckAlignWithMargins";
			}
			protected static class StringID {
				public const ASPxSpreadsheetStringId Caption = ASPxSpreadsheetStringId.PageSetup_HeaderFooterTab_Caption;
				public const ASPxSpreadsheetStringId Header = ASPxSpreadsheetStringId.PageSetup_HeaderFooterTab_Header;
				public const ASPxSpreadsheetStringId Footer = ASPxSpreadsheetStringId.PageSetup_HeaderFooterTab_Footer;
				public const ASPxSpreadsheetStringId DifferentOddEven = ASPxSpreadsheetStringId.PageSetup_HeaderFooterTab_DifferentOddEven;
				public const ASPxSpreadsheetStringId DifferentFirst = ASPxSpreadsheetStringId.PageSetup_HeaderFooterTab_DifferentFirst;
				public const ASPxSpreadsheetStringId ScaleWithDoc = ASPxSpreadsheetStringId.PageSetup_HeaderFooterTab_ScaleWithDoc;
				public const ASPxSpreadsheetStringId AlignWithMargins = ASPxSpreadsheetStringId.PageSetup_HeaderFooterTab_AlignWithMargins;
			}
			protected const string HeaderFooterChangedClientHandlerTemplate =
				"function(s, e) {{ ASPx.SpreadsheetDialog.HeaderFooterChanged(s.GetValue(), {0}); }}";
			protected override string DefaultText {
				get {
					return Localize(StringID.Caption);
				}
			}
			#region FormLayout controls 
			protected LayoutGroup HeaderFooterGroup { get; private set; }
			protected ASPxComboBox Header { get; private set; }
			protected ASPxComboBox Footer { get; private set; }
			protected HeaderFooterPreviewControl HeaderPreview { get; private set; }
			protected HeaderFooterPreviewControl FooterPreview { get; private set; }
			protected ASPxCheckBox DifferentOddAndEvenPages { get; private set; }
			protected ASPxCheckBox DifferentFirstPage { get; private set; }
			protected ASPxCheckBox ScaleWithDocument { get; private set; }
			protected ASPxCheckBox AlignWithPageMargins { get; private set; }
			public override List<ASPxEditBase> GetChildDxEdits() {
				return new List<ASPxEditBase>() {
					Header, Footer, DifferentFirstPage, DifferentOddAndEvenPages, ScaleWithDocument, AlignWithPageMargins
				};
			}
			#endregion
			public PSHeaderFooterTab(PageSetupDialog owner)
				: base(owner) {
			}
			protected override void CreateFormControls() {
				HeaderPreview = new HeaderFooterPreviewControl(ClassNames.HeaderPreview);
				Header = CreateValuesComboBox(StringID.Header, ControlClientName.Header, true);
				Footer = CreateValuesComboBox(StringID.Footer, ControlClientName.Footer, false);
				FooterPreview = new HeaderFooterPreviewControl(ClassNames.FooterPreview);
				DifferentOddAndEvenPages = CreateCheckBox(StringID.DifferentOddEven, ControlClientName.DifferentOddAndEvenPages);
				DifferentOddAndEvenPages.Enabled = false;
				DifferentFirstPage = CreateCheckBox(StringID.DifferentFirst, ControlClientName.DifferentFirstPage);
				DifferentFirstPage.Enabled = false;
				ScaleWithDocument = CreateCheckBox(StringID.ScaleWithDoc, ControlClientName.ScaleWithDocument);
				AlignWithPageMargins = CreateCheckBox(StringID.AlignWithMargins, ControlClientName.AlignWithPageMargins);
			}
			protected ASPxComboBox CreateValuesComboBox(ASPxSpreadsheetStringId stringID, string clientName, bool isHeader) {
				var cb = new ASPxComboBox() {
					Caption = Localize(stringID) + DialogUtils.LabelEndMark,
					ClientInstanceName = GetControlClientInstanceName(clientName),
					Width = Unit.Percentage(100)
				};
				cb.ClientSideEvents.ValueChanged = GetHeaderFooterChangedClientHandler(isHeader);
				cb.CaptionSettings.HorizontalAlign = EditorCaptionHorizontalAlign.Left;
				cb.CaptionSettings.Position = EditorCaptionPosition.Top;
				return cb;
			}
			protected string GetHeaderFooterChangedClientHandler(bool isHeader) {
				return String.Format(HeaderFooterChangedClientHandlerTemplate, isHeader ? "true" : "false");
			}
			protected override void PopulateFormLayout() {
				HeaderFooterGroup = new LayoutGroup() {
					ShowCaption = Utils.DefaultBoolean.False,
					GroupBoxDecoration = GroupBoxDecoration.None,
					UseDefaultPaddings = false
				};
				AddControlToLayoutGroup(HeaderPreview, HeaderFooterGroup);
				AddControlToLayoutGroup(Header, HeaderFooterGroup);
				AddControlToLayoutGroup(Footer, HeaderFooterGroup);
				AddControlToLayoutGroup(FooterPreview, HeaderFooterGroup);
				AddControlToLayoutGroup(DifferentOddAndEvenPages, HeaderFooterGroup);
				AddControlToLayoutGroup(DifferentFirstPage, HeaderFooterGroup);
				AddControlToLayoutGroup(ScaleWithDocument, HeaderFooterGroup);
				AddControlToLayoutGroup(AlignWithPageMargins, HeaderFooterGroup);
				FormLayout.Items.Add(HeaderFooterGroup);
			}
			protected class HeaderFooterPreviewControl : ASPxInternalWebControl {
				public static class ClassNames {
					public const string PreviewArea = "dxssHFPArea";
					public const string PreviewClassNameBase = "dxssHFP";
					public const string LeftPreview = PreviewClassNameBase + "Left";
					public const string CenterPreview = PreviewClassNameBase + "Center";
					public const string RightPreview = PreviewClassNameBase + "Right";
				}
				protected WebControl LeftPreview { get; private set; }
				protected WebControl CenterPreview { get; private set; }
				protected WebControl RightPreview { get; private set; }
				public HeaderFooterPreviewControl(string className)
					: base() {
						CssClass = className;
				}
				protected override bool HasRootTag() {
					return true;
				}
				protected override HtmlTextWriterTag TagKey {
					get {
						return HtmlTextWriterTag.Div;
					}
				}
				protected override void CreateControlHierarchy() {
					LeftPreview = CreatePreviewControl(ClassNames.LeftPreview);
					CenterPreview = CreatePreviewControl(ClassNames.CenterPreview);
					RightPreview = CreatePreviewControl(ClassNames.RightPreview);
					Controls.Add(LeftPreview);
					Controls.Add(CenterPreview);
					Controls.Add(RightPreview);
				}
				protected override void PrepareControlHierarchy() {
					base.PrepareControlHierarchy();
					RenderUtils.AppendDefaultDXClassName(this, ClassNames.PreviewArea);
				}
				protected WebControl CreatePreviewControl(string className) {
					var span = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
					span.CssClass = className;
					span.EnableViewState = false;
					RenderUtils.AppendDefaultDXClassName(span, ClassNames.PreviewClassNameBase);
					return span;
				}
			}
		}
		protected class PSSheetTab : PageSetupTabBase {
			protected static class ControlID {
				public const string MarginLeft = "seMarginLeft";
			}
			protected static class ControlClientName {
				public const string PrintArea = "_tbPrintArea";
				public const string Gridlines = "_ckPrintGridlines";
				public const string DraftQuality = "_ckDraft";
				public const string RowAndColumnHeadings = "_ckPrintHeadings";
				public const string Comments = "_cbCommentsPrintMode";
				public const string CellErrorsAs = "_cbErrorsPrintMode";
				public const string PageOrder = "_rlDownThenOver";
			}
			protected static class StringID {
				public const ASPxSpreadsheetStringId Caption = ASPxSpreadsheetStringId.PageSetup_SheetTab_Caption;
				public const ASPxSpreadsheetStringId PrintArea = ASPxSpreadsheetStringId.PageSetup_SheetTab_PrintArea;
				public const ASPxSpreadsheetStringId PrintAreaErrorText = ASPxSpreadsheetStringId.PageSetup_SheetTab_PrintAreaErrorText;
				public const ASPxSpreadsheetStringId PrintGroup = ASPxSpreadsheetStringId.PageSetup_SheetTab_PrintGroup;
				public const ASPxSpreadsheetStringId Gridlines = ASPxSpreadsheetStringId.PageSetup_SheetTab_Gridlines;
				public const ASPxSpreadsheetStringId Draft = ASPxSpreadsheetStringId.PageSetup_SheetTab_Draft;
				public const ASPxSpreadsheetStringId PrintHeadings = ASPxSpreadsheetStringId.PageSetup_SheetTab_PrintHeadings;
				public const ASPxSpreadsheetStringId Comments = ASPxSpreadsheetStringId.PageSetup_SheetTab_Comments;
				public const ASPxSpreadsheetStringId CellErrorAs = ASPxSpreadsheetStringId.PageSetup_SheetTab_CellErrorAs;
				public const ASPxSpreadsheetStringId PageOrderGroup = ASPxSpreadsheetStringId.PageSetup_SheetTab_PageOrderGroup;
				public const ASPxSpreadsheetStringId PageOrder_DownThenOver = ASPxSpreadsheetStringId.PageSetup_SheetTab_PageOrder_DownThenOver;
				public const ASPxSpreadsheetStringId PageOrder_OverThenDown = ASPxSpreadsheetStringId.PageSetup_SheetTab_PageOrder_OverThenDown;
			}
			const string PrintAreaValidationGroup = ValidationGroupBase + "_PrintArea";
			protected override string DefaultText {
				get { 
					return Localize(StringID.Caption);
				}
			}
			#region FormLayout controls
			protected LayoutGroup PrintAreaGroup { get; private set; }
			protected LayoutGroup PrintGroup { get; private set; }
			protected LayoutGroup PageOrderGroup { get; private set; }
			protected ASPxTextBox PrintArea { get; private set; }
			protected ASPxCheckBox Gridlines { get; private set; }
			protected ASPxCheckBox DraftQuality { get; private set; }
			protected ASPxCheckBox RowAndColumnHeadings { get; private set; }
			protected ASPxComboBox Comments { get; private set; }
			protected ASPxComboBox CellErrorsAs { get; private set; }
			protected ASPxLabel CommentsCaption { get; private set; }
			protected ASPxLabel CellErrorsAsCaption { get; private set; }
			protected ASPxRadioButtonList PageOrder { get; private set; }
			public override List<ASPxEditBase> GetChildDxEdits() {
				return new List<ASPxEditBase>() {
					PrintArea, Gridlines, DraftQuality, RowAndColumnHeadings, Comments, CellErrorsAs, PageOrder
				};
			}
			#endregion
			public PSSheetTab(PageSetupDialog owner)
				: base(owner) {
			}
			protected override void CreateFormControls() {
				PrintArea = new ASPxTextBox() {
					Caption = Localize(StringID.PrintArea) + DialogUtils.LabelEndMark,
					ClientInstanceName = GetControlClientInstanceName(ControlClientName.PrintArea),
					Width = Unit.Percentage(100),
				};
				PrintArea.ValidationSettings.RegularExpression.ValidationExpression = "([A-Z]+[0-9]+:[A-Z]+[0-9]+;?)+";
				PrintArea.ValidationSettings.RegularExpression.ErrorText = Localize(StringID.PrintAreaErrorText);
				PrintArea.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithTooltip;
				PrintArea.ValidationSettings.ValidationGroup = PrintAreaValidationGroup;
				Gridlines = CreateCheckBox(StringID.Gridlines, ControlClientName.Gridlines);
				DraftQuality = CreateCheckBox(StringID.Draft, ControlClientName.DraftQuality);
				RowAndColumnHeadings = CreateCheckBox(StringID.PrintHeadings, ControlClientName.RowAndColumnHeadings);
				Comments = new ASPxComboBox() {
					ClientInstanceName = GetControlClientInstanceName(ControlClientName.Comments),
					Width = Unit.Pixel(130)
				};
				CommentsCaption = new ASPxLabel() {
					Text = Localize(StringID.Comments) + DialogUtils.LabelEndMark
				};
				CellErrorsAs = new ASPxComboBox() {
					ClientInstanceName = GetControlClientInstanceName(ControlClientName.CellErrorsAs),
					Width = Unit.Pixel(130)
				};
				CellErrorsAsCaption = new ASPxLabel() {
					Text = Localize(StringID.CellErrorAs) + DialogUtils.LabelEndMark
				};
				PageOrder = new ASPxRadioButtonList();
				PageOrder.ClientInstanceName = GetControlClientInstanceName(ControlClientName.PageOrder);
				PageOrder.Border.BorderWidth = Unit.Pixel(0);
				PageOrder.Items.Add(Localize(StringID.PageOrder_DownThenOver));
				PageOrder.Items.Add(Localize(StringID.PageOrder_OverThenDown));
			}
			protected override void PopulateFormLayout() {
				PrintAreaGroup = new LayoutGroup() {
					ShowCaption = Utils.DefaultBoolean.False,
					GroupBoxDecoration = GroupBoxDecoration.None,
					UseDefaultPaddings = false
				};
				AddControlToLayoutGroup(PrintArea, PrintAreaGroup);
				PrintGroup = new LayoutGroup() {
					Caption = Localize(StringID.PrintGroup),
					GroupBoxDecoration = GroupBoxDecoration.HeadingLine,
					UseDefaultPaddings = false,
					ColCount = 4
				};
				PrintGroup.GroupBoxStyle.CssClass = ClassNames.CompactGroupBox;
				AddControlToLayoutGroup(Gridlines, PrintGroup, i => { i.ColSpan = 2; i.CssClass = ClassNames.CompactLayoutItem; });
				AddControlToLayoutGroup(CommentsCaption, PrintGroup, i => i.CssClass = ClassNames.CompactLayoutItem);
				AddControlToLayoutGroup(Comments, PrintGroup, i => i.HorizontalAlign = FormLayoutHorizontalAlign.NotSet);
				AddControlToLayoutGroup(DraftQuality, PrintGroup, i => i.ColSpan = 2);
				AddControlToLayoutGroup(CellErrorsAsCaption, PrintGroup);
				AddControlToLayoutGroup(CellErrorsAs, PrintGroup, i => i.HorizontalAlign = FormLayoutHorizontalAlign.NotSet);
				AddControlToLayoutGroup(RowAndColumnHeadings, PrintGroup);
				PageOrderGroup = new LayoutGroup() {
					Caption = Localize(StringID.PageOrderGroup),
					GroupBoxDecoration = GroupBoxDecoration.HeadingLine,
					UseDefaultPaddings = false
				};
				PageOrderGroup.GroupBoxStyle.CssClass = ClassNames.CompactGroupBox;
				AddControlToLayoutGroup(PageOrder, PageOrderGroup, i => i.CssClass = ClassNames.CompactLayoutItem);
				FormLayout.Items.Add(PrintAreaGroup);
				FormLayout.Items.Add(PrintGroup);
				FormLayout.Items.Add(PageOrderGroup);
			}
		}
	}
}
