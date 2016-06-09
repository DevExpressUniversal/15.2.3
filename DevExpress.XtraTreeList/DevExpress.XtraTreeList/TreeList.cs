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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Persistent;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList.Blending;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Data;
using DevExpress.XtraTreeList.Localization;
using DevExpress.XtraTreeList.ViewInfo;
using DevExpress.XtraTreeList.Handler;
using DevExpress.XtraTreeList.Painter;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using DevExpress.XtraTreeList.Printing;
using DevExpress.XtraPrinting;
using DevExpress.Utils.Menu;
using DevExpress.XtraTreeList.StyleFormatConditions;
using DevExpress.XtraTreeList.Accessibility;
using DevExpress.Accessibility;
using DevExpress.Utils.Drawing.Animation;
using System.Collections.Generic;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Utils.Internal;
using DevExpress.XtraGrid;
using DevExpress.Data.Filtering;
using DevExpress.XtraTreeList.Internal;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraTreeList.FilterEditor;
using DevExpress.XtraEditors.Filtering;
using DevExpress.Utils.Gesture;
using DevExpress.Utils.Text;
using System.Text;
using DevExpress.Data.Helpers;
using DevExpress.XtraTreeList.Scrolling;
using DevExpress.XtraEditors.Design;
using System.Linq;
using DevExpress.XtraTreeList.Helpers;
using DevExpress.XtraExport.Helpers;
using DevExpress.Export;
namespace DevExpress.XtraTreeList {
	[Designer("DevExpress.XtraTreeList.Design.TreeListDesigner, " + AssemblyInfo.SRAssemblyTreeListDesign, typeof(System.ComponentModel.Design.IDesigner)),
	DXToolboxItem(true),
	DefaultProperty("Nodes"), DefaultEvent("FocusedNodeChanged"),
	ToolboxTabName(AssemblyInfo.DXTabData),
	Docking(DockingBehavior.Ask),
	Description("Represents data in a tree-like structure, enables editing data, provides data filtering, sorting and summary calculation features.")
]
	[DevExpress.Utils.Design.DataAccess.DataAccessMetadata("All", SupportedProcessingModes = "Simple"), TreeListCustomBindingProperties]
	[ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "TreeList")]
	public class TreeList : EditorContainer, IPrintableEx, IXtraSerializable, INavigatableControl, IToolTipControlClient, IAppearanceOwner, ISupportLookAndFeel, DevExpress.Utils.Menu.IDXManagerPopupMenu, IAccessibleGrid, IXtraSerializableLayout, ISupportXtraAnimation, IEvaluatorDataAccess, IXtraSerializableLayoutEx, ISupportXtraSerializer, IGestureClient, IServiceProvider, IStringImageProvider, IFilteredComponent, IFilteredComponentColumnsClient {
		#region events
		private static readonly object cellValueChanging = new object();
		private static readonly object cellValueChanged = new object();
		private static readonly object columnChanged = new object();
		private static readonly object getStateImage = new object();
		private static readonly object getSelectImage = new object();
		private static readonly object getPreviewText = new object();
		private static readonly object measurePreviewHeight = new object();
		private static readonly object getPrintPreviewText = new object();
		private static readonly object getPrintCustomSummaryValue = new object();
		private static readonly object getCustomSummaryValue = new object();
		private static readonly object customNodeCellEdit = new object();
		private static readonly object customNodeCellEditForEditing = new object();
		private static readonly object nodeCellStyle = new object();
		private static readonly object getNodeDisplayValue = new object();
		private static readonly object customUnboundColumnData = new object();
		private static readonly object beforeExpand = new object();
		private static readonly object beforeCollapse = new object();
		private static readonly object beforeDragNode = new object();
		private static readonly object beforeFocusNode = new object();
		private static readonly object afterExpand = new object();
		private static readonly object afterCollapse = new object();
		private static readonly object afterDragNode = new object();
		private static readonly object afterFocusNode = new object();
		private static readonly object calcNodeHeight = new object();
		private static readonly object calcNodeDragImageIndex = new object();
		private static readonly object nodeChanged = new object();
		private static readonly object focusedNodeChanged = new object();
		private static readonly object focusedColumnChanged = new object();
		private static readonly object columnWidthChanged = new object();
		private static readonly object compareNodeValues = new object();
		private static readonly object validatingEditor = new object();
		private static readonly object validateNode = new object();
		private static readonly object invalidValueException = new object();
		private static readonly object invalidNodeException = new object();
		private static readonly object selectionChanged = new object();
		private static readonly object stateChanged = new object();
		private static readonly object dragCancelNode = new object();
		private static readonly object showCustomizationForm = new object();
		private static readonly object hideCustomizationForm = new object();
		private static readonly object columnButtonClick = new object();
		private static readonly object shownEditor = new object();
		private static readonly object hiddenEditor = new object();
		private static readonly object customDrawNodeIndicator = new object();
		private static readonly object customDrawNodeImages = new object();
		private static readonly object customDrawColumnHeader = new object();
		private static readonly object customDrawBandHeader = new object();
		private static readonly object customDrawNodePreview = new object();
		private static readonly object customDrawNodeButton = new object();
		private static readonly object customDrawNodeCell = new object();
		private static readonly object customDrawFooter = new object();
		private static readonly object customDrawRowFooter = new object();
		private static readonly object customDrawFooterCell = new object();
		private static readonly object customDrawRowFooterCell = new object();
		private static readonly object customDrawEmptyArea = new object();
		private static readonly object customDrawNodeIndent = new object();
		private static readonly object customDrawNodeCheckBox = new object();
		private static readonly object customDrawFilterPanel = new object();
		private static readonly object treelistMenuItemClick = new object();
		private static readonly object showTreeListMenu = new object();
		private static readonly object onPopupMenuShowing = new object();
		private static readonly object leftCoordChanged = new object();
		private static readonly object topVisibleNodeIndexChanged = new object();
		private static readonly object stateImageClick = new object();
		private static readonly object selectImageClick = new object();
		private static readonly object defaultPaintHelperChanged = new object();
		private static readonly object showingEditor = new object();
		private static readonly object nodesReloaded = new object();
		private static readonly object createCustomNode = new object();
		private static readonly object startSorting = new object();
		private static readonly object endSorting = new object();
		private static readonly object layoutUpdated = new object();
		static readonly object layoutUpgrade = new object();
		private static readonly object beforeLoadLayout = new object();
		private static readonly object virtualTreeGetChildNodes = new object();
		private static readonly object virtualTreeGetCellValue = new object();
		private static readonly object virtualTreeSetCellValue = new object();
		private static readonly object filterNode = new object();
		private static readonly object dragObjectStart = new object();
		private static readonly object dragObjectOver = new object();
		private static readonly object dragObjectDrop = new object();
		private static readonly object beforeCheckNode = new object();
		private static readonly object afterCheckNode = new object();
		private static readonly object customFilterDisplayText = new object();
		private static readonly object filterEditorCreated = new object();
		private static readonly object columnFilterChanged = new object();
		private static readonly object showFilterPopupListBox = new object();
		private static readonly object showFilterPopupCheckedListBox = new object();
		private static readonly object showFilterPopupDate = new object();
		private static readonly object columnUnboundExpressionChanged = new object();
		private static readonly object unboundExpressionEditorCreated = new object();
		private static readonly object load = new object();
		private static readonly object bandWidthChanged = new object();
		private static readonly object printExportProgress = new object();
		private static readonly object columnPositionChanged = new object();
		private readonly static object beforeDropNode = new object();
		private readonly static object afterDropNode = new object();
		private readonly static object customizeNewNodeFromOuterData = new object();
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnChanged"),
#endif
 Category("Property Changed")]
		public event ColumnChangedEventHandler ColumnChanged {
			add { Events.AddHandler(columnChanged, value); }
			remove { Events.RemoveHandler(columnChanged, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnWidthChanged"),
#endif
 Category("Property Changed")]
		public event ColumnWidthChangedEventHandler ColumnWidthChanged {
			add { this.Events.AddHandler(columnWidthChanged, value); }
			remove { this.Events.RemoveHandler(columnWidthChanged, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListGetStateImage"),
#endif
 Category("Nodes")]
		public event GetStateImageEventHandler GetStateImage {
			add { Events.AddHandler(getStateImage, value); }
			remove { Events.RemoveHandler(getStateImage, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListGetSelectImage"),
#endif
 Category("Nodes")]
		public event GetSelectImageEventHandler GetSelectImage {
			add { Events.AddHandler(getSelectImage, value); }
			remove { Events.RemoveHandler(getSelectImage, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListStateImageClick"),
#endif
 Category("Nodes")]
		public event NodeClickEventHandler StateImageClick {
			add { Events.AddHandler(stateImageClick, value); }
			remove { Events.RemoveHandler(stateImageClick, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListSelectImageClick"),
#endif
 Category("Nodes")]
		public event NodeClickEventHandler SelectImageClick {
			add { Events.AddHandler(selectImageClick, value); }
			remove { Events.RemoveHandler(selectImageClick, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListGetPreviewText"),
#endif
 Category("Nodes")]
		public event GetPreviewTextEventHandler GetPreviewText {
			add { Events.AddHandler(getPreviewText, value); }
			remove { Events.RemoveHandler(getPreviewText, value); }
		}
		[Category("Data"), 
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListMeasurePreviewHeight")
#else
	Description("")
#endif
]
		public event NodePreviewHeightEventHandler MeasurePreviewHeight {
			add { Events.AddHandler(measurePreviewHeight, value); }
			remove { Events.RemoveHandler(measurePreviewHeight, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListGetCustomSummaryValue"),
#endif
 Category("Nodes")]
		public event GetCustomSummaryValueEventHandler GetCustomSummaryValue {
			add { Events.AddHandler(getCustomSummaryValue, value); }
			remove { Events.RemoveHandler(getCustomSummaryValue, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCreateCustomNode"),
#endif
 Category("Nodes")]
		public event CreateCustomNodeEventHandler CreateCustomNode {
			add { Events.AddHandler(createCustomNode, value); }
			remove { Events.RemoveHandler(createCustomNode, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListGetPrintPreviewText"),
#endif
 Category("Printing")]
		public event GetPreviewTextEventHandler GetPrintPreviewText {
			add { Events.AddHandler(getPrintPreviewText, value); }
			remove { Events.RemoveHandler(getPrintPreviewText, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListGetPrintCustomSummaryValue"),
#endif
 Category("Printing")]
		public event GetCustomSummaryValueEventHandler GetPrintCustomSummaryValue {
			add { Events.AddHandler(getPrintCustomSummaryValue, value); }
			remove { Events.RemoveHandler(getPrintCustomSummaryValue, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("You must use the CustomNodeCellEdit event instead")]
		public event GetCustomNodeCellEditEventHandler GetCustomNodeCellEdit {
			add { Events.AddHandler(customNodeCellEdit, value); }
			remove { Events.RemoveHandler(customNodeCellEdit, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCustomNodeCellEdit"),
#endif
 Category("Editor")]
		public event GetCustomNodeCellEditEventHandler CustomNodeCellEdit {
			add { Events.AddHandler(customNodeCellEdit, value); }
			remove { Events.RemoveHandler(customNodeCellEdit, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCustomNodeCellEditForEditing"),
#endif
 Category("Editor")]
		public event GetCustomNodeCellEditEventHandler CustomNodeCellEditForEditing {
			add { Events.AddHandler(customNodeCellEditForEditing, value); }
			remove { Events.RemoveHandler(customNodeCellEditForEditing, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("You must use the NodeCellStyle event instead")]
		public event GetCustomNodeCellStyleEventHandler GetCustomNodeCellStyle {
			add { Events.AddHandler(nodeCellStyle, value); }
			remove { Events.RemoveHandler(nodeCellStyle, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListNodeCellStyle"),
#endif
 Category("Appearance")]
		public event GetCustomNodeCellStyleEventHandler NodeCellStyle {
			add { Events.AddHandler(nodeCellStyle, value); }
			remove { Events.RemoveHandler(nodeCellStyle, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListGetNodeDisplayValue"),
#endif
 Category("Nodes")]
		public event GetNodeDisplayValueEventHandler GetNodeDisplayValue {
			add { Events.AddHandler(getNodeDisplayValue, value); }
			remove { Events.RemoveHandler(getNodeDisplayValue, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCustomUnboundColumnData"),
#endif
 DXCategory("Behavior")]
		public event CustomColumnDataEventHandler CustomUnboundColumnData {
			add { this.Events.AddHandler(customUnboundColumnData, value); }
			remove { this.Events.RemoveHandler(customUnboundColumnData, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListBeforeExpand"),
#endif
 Category("Nodes")]
		public event BeforeExpandEventHandler BeforeExpand {
			add { Events.AddHandler(beforeExpand, value); }
			remove { Events.RemoveHandler(beforeExpand, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListBeforeCollapse"),
#endif
 Category("Nodes")]
		public event BeforeCollapseEventHandler BeforeCollapse {
			add { Events.AddHandler(beforeCollapse, value); }
			remove { Events.RemoveHandler(beforeCollapse, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListBeforeDragNode"),
#endif
 Category("Nodes")]
		public event BeforeDragNodeEventHandler BeforeDragNode {
			add { Events.AddHandler(beforeDragNode, value); }
			remove { Events.RemoveHandler(beforeDragNode, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListBeforeFocusNode"),
#endif
 Category("Nodes")]
		public event BeforeFocusNodeEventHandler BeforeFocusNode {
			add { Events.AddHandler(beforeFocusNode, value); }
			remove { Events.RemoveHandler(beforeFocusNode, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAfterExpand"),
#endif
 Category("Nodes")]
		public event NodeEventHandler AfterExpand {
			add { Events.AddHandler(afterExpand, value); }
			remove { Events.RemoveHandler(afterExpand, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAfterCollapse"),
#endif
 Category("Nodes")]
		public event NodeEventHandler AfterCollapse {
			add { Events.AddHandler(afterCollapse, value); }
			remove { Events.RemoveHandler(afterCollapse, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAfterDragNode"),
#endif
 Category("Nodes")]
		public event AfterDragNodeEventHandler AfterDragNode {
			add { Events.AddHandler(afterDragNode, value); }
			remove { Events.RemoveHandler(afterDragNode, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAfterFocusNode"),
#endif
 Category("Nodes")]
		public event NodeEventHandler AfterFocusNode {
			add { Events.AddHandler(afterFocusNode, value); }
			remove { Events.RemoveHandler(afterFocusNode, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListBeforeCheckNode"),
#endif
 Category("Nodes")]
		public event CheckNodeEventHandler BeforeCheckNode {
			add { Events.AddHandler(beforeCheckNode, value); }
			remove { Events.RemoveHandler(beforeCheckNode, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAfterCheckNode"),
#endif
 Category("Nodes")]
		public event NodeEventHandler AfterCheckNode {
			add { Events.AddHandler(afterCheckNode, value); }
			remove { Events.RemoveHandler(afterCheckNode, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCalcNodeHeight"),
#endif
 Category("Behavior")]
		public event CalcNodeHeightEventHandler CalcNodeHeight {
			add { Events.AddHandler(calcNodeHeight, value); }
			remove { Events.RemoveHandler(calcNodeHeight, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListNodeChanged"),
#endif
 Category("Property Changed")]
		public event NodeChangedEventHandler NodeChanged {
			add { Events.AddHandler(nodeChanged, value); }
			remove { Events.RemoveHandler(nodeChanged, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListFocusedNodeChanged"),
#endif
 Category("Property Changed")]
		public event FocusedNodeChangedEventHandler FocusedNodeChanged {
			add { Events.AddHandler(focusedNodeChanged, value); }
			remove { Events.RemoveHandler(focusedNodeChanged, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListFocusedColumnChanged"),
#endif
 Category("Property Changed")]
		public event FocusedColumnChangedEventHandler FocusedColumnChanged {
			add { Events.AddHandler(focusedColumnChanged, value); }
			remove { Events.RemoveHandler(focusedColumnChanged, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCompareNodeValues"),
#endif
 Category("Behavior")]
		public event CompareNodeValuesEventHandler CompareNodeValues {
			add { Events.AddHandler(compareNodeValues, value); }
			remove { Events.RemoveHandler(compareNodeValues, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListValidatingEditor"),
#endif
 Category("Editor")]
		public event BaseContainerValidateEditorEventHandler ValidatingEditor {
			add { Events.AddHandler(validatingEditor, value); }
			remove { Events.RemoveHandler(validatingEditor, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListInvalidValueException"),
#endif
 Category("Action")]
		public event InvalidValueExceptionEventHandler InvalidValueException {
			add { Events.AddHandler(invalidValueException, value); }
			remove { Events.RemoveHandler(invalidValueException, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListInvalidNodeException"),
#endif
 Category("Action")]
		public event InvalidNodeExceptionEventHandler InvalidNodeException {
			add { Events.AddHandler(invalidNodeException, value); }
			remove { Events.RemoveHandler(invalidNodeException, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListValidateNode"),
#endif
 Category("Action")]
		public event ValidateNodeEventHandler ValidateNode {
			add { Events.AddHandler(validateNode, value); }
			remove { Events.RemoveHandler(validateNode, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListSelectionChanged"),
#endif
 Category("Property Changed")]
		public event EventHandler SelectionChanged {
			add { Events.AddHandler(selectionChanged, value); }
			remove { Events.RemoveHandler(selectionChanged, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListStateChanged"),
#endif
 Category("Property Changed")]
		public event EventHandler StateChanged {
			add { Events.AddHandler(stateChanged, value); }
			remove { Events.RemoveHandler(stateChanged, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListDragCancelNode"),
#endif
 Category("Drag Drop")]
		public event EventHandler DragCancelNode {
			add { Events.AddHandler(dragCancelNode, value); }
			remove { Events.RemoveHandler(dragCancelNode, value); }
		}		
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListBeforeDropNode"),
#endif
 Category("Drag Drop")]
		public event BeforeDropNodeEventHandler BeforeDropNode {
			add { Events.AddHandler(beforeDropNode, value); }
			remove { Events.RemoveHandler(beforeDropNode, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAfterDropNode"),
#endif
 Category("Drag Drop")]
		public event AfterDropNodeEventHandler AfterDropNode {
			add { Events.AddHandler(afterDropNode, value); }
			remove { Events.RemoveHandler(afterDropNode, value); }
		}
		[ Category("Drag Drop")]
		public event CustomizeNewNodeFromOuterDataEventHandler CustomizeNewNodeFromOuterData {
			add { Events.AddHandler(customizeNewNodeFromOuterData, value); }
			remove { Events.RemoveHandler(customizeNewNodeFromOuterData, value); }
		}		
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCalcNodeDragImageIndex"),
#endif
 Category("Drag Drop")]
		public event CalcNodeDragImageIndexEventHandler CalcNodeDragImageIndex {
			add { Events.AddHandler(calcNodeDragImageIndex, value); }
			remove { Events.RemoveHandler(calcNodeDragImageIndex, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListShowCustomizationForm"),
#endif
 Category("Behavior")]
		public event EventHandler ShowCustomizationForm {
			add { Events.AddHandler(showCustomizationForm, value); }
			remove { Events.RemoveHandler(showCustomizationForm, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListHideCustomizationForm"),
#endif
 Category("Behavior")]
		public event EventHandler HideCustomizationForm {
			add { Events.AddHandler(hideCustomizationForm, value); }
			remove { Events.RemoveHandler(hideCustomizationForm, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnButtonClick"),
#endif
 Category("Behavior")]
		public event EventHandler ColumnButtonClick {
			add { Events.AddHandler(columnButtonClick, value); }
			remove { Events.RemoveHandler(columnButtonClick, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListShownEditor"),
#endif
 Category("Editor")]
		public event EventHandler ShownEditor {
			add { Events.AddHandler(shownEditor, value); }
			remove { Events.RemoveHandler(shownEditor, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListHiddenEditor"),
#endif
 Category("Editor")]
		public event EventHandler HiddenEditor {
			add { Events.AddHandler(hiddenEditor, value); }
			remove { Events.RemoveHandler(hiddenEditor, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCustomDrawNodeIndicator"),
#endif
 Category("CustomDraw")]
		public event CustomDrawNodeIndicatorEventHandler CustomDrawNodeIndicator {
			add { Events.AddHandler(customDrawNodeIndicator, value); }
			remove { Events.RemoveHandler(customDrawNodeIndicator, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCustomDrawColumnHeader"),
#endif
 Category("CustomDraw")]
		public event CustomDrawColumnHeaderEventHandler CustomDrawColumnHeader {
			add { Events.AddHandler(customDrawColumnHeader, value); }
			remove { Events.RemoveHandler(customDrawColumnHeader, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCustomDrawBandHeader"),
#endif
 Category("CustomDraw")]
		public event CustomDrawBandHeaderEventHandler CustomDrawBandHeader {
			add { Events.AddHandler(customDrawBandHeader, value); }
			remove { Events.RemoveHandler(customDrawBandHeader, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCustomDrawNodePreview"),
#endif
 Category("CustomDraw")]
		public event CustomDrawNodePreviewEventHandler CustomDrawNodePreview {
			add { Events.AddHandler(customDrawNodePreview, value); }
			remove { Events.RemoveHandler(customDrawNodePreview, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCustomDrawNodeCell"),
#endif
 Category("CustomDraw")]
		public event CustomDrawNodeCellEventHandler CustomDrawNodeCell {
			add { Events.AddHandler(customDrawNodeCell, value); }
			remove { Events.RemoveHandler(customDrawNodeCell, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCustomDrawFooter"),
#endif
 Category("CustomDraw")]
		public event CustomDrawFooterEventHandler CustomDrawFooter {
			add { Events.AddHandler(customDrawFooter, value); }
			remove { Events.RemoveHandler(customDrawFooter, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCustomDrawRowFooter"),
#endif
 Category("CustomDraw")]
		public event CustomDrawRowFooterEventHandler CustomDrawRowFooter {
			add { Events.AddHandler(customDrawRowFooter, value); }
			remove { Events.RemoveHandler(customDrawRowFooter, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCustomDrawNodeIndent"),
#endif
 Category("CustomDraw")]
		public event CustomDrawNodeIndentEventHandler CustomDrawNodeIndent {
			add { Events.AddHandler(customDrawNodeIndent, value); }
			remove { Events.RemoveHandler(customDrawNodeIndent, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCustomDrawFooterCell"),
#endif
 Category("CustomDraw")]
		public event CustomDrawFooterCellEventHandler CustomDrawFooterCell {
			add { Events.AddHandler(customDrawFooterCell, value); }
			remove { Events.RemoveHandler(customDrawFooterCell, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCustomDrawRowFooterCell"),
#endif
 Category("CustomDraw")]
		public event CustomDrawRowFooterCellEventHandler CustomDrawRowFooterCell {
			add { Events.AddHandler(customDrawRowFooterCell, value); }
			remove { Events.RemoveHandler(customDrawRowFooterCell, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCustomDrawEmptyArea"),
#endif
 Category("CustomDraw")]
		public event CustomDrawEmptyAreaEventHandler CustomDrawEmptyArea {
			add { Events.AddHandler(customDrawEmptyArea, value); }
			remove { Events.RemoveHandler(customDrawEmptyArea, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCustomDrawNodeImages"),
#endif
 Category("CustomDraw")]
		public event CustomDrawNodeImagesEventHandler CustomDrawNodeImages {
			add { Events.AddHandler(customDrawNodeImages, value); }
			remove { Events.RemoveHandler(customDrawNodeImages, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCustomDrawNodeCheckBox"),
#endif
 Category("CustomDraw")]
		public event CustomDrawNodeCheckBoxEventHandler CustomDrawNodeCheckBox {
			add { Events.AddHandler(customDrawNodeCheckBox, value); }
			remove { Events.RemoveHandler(customDrawNodeCheckBox, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCustomDrawNodeButton"),
#endif
 Category("CustomDraw")]
		public event CustomDrawNodeButtonEventHandler CustomDrawNodeButton {
			add { Events.AddHandler(customDrawNodeButton, value); }
			remove { Events.RemoveHandler(customDrawNodeButton, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCustomDrawFilterPanel"),
#endif
 Category("CustomDraw")]
		public event CustomDrawObjectEventHandler CustomDrawFilterPanel {
			add { Events.AddHandler(customDrawFilterPanel, value); }
			remove { Events.RemoveHandler(customDrawFilterPanel, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListTreeListMenuItemClick"),
#endif
 Category("Behavior")]
		public event TreeListMenuItemClickEventHandler TreeListMenuItemClick {
			add { this.Events.AddHandler(treelistMenuItemClick, value); }
			remove { this.Events.RemoveHandler(treelistMenuItemClick, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListShowTreeListMenu"),
#endif
 Category("Behavior")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'PopupMenuShowing' instead", false)]
		public event TreeListMenuEventHandler ShowTreeListMenu {
			add { this.Events.AddHandler(showTreeListMenu, value); }
			remove { this.Events.RemoveHandler(showTreeListMenu, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListPopupMenuShowing"),
#endif
 Category("Behavior")]
		public event PopupMenuShowingEventHandler PopupMenuShowing {
			add { this.Events.AddHandler(onPopupMenuShowing, value); }
			remove { this.Events.RemoveHandler(onPopupMenuShowing, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListLeftCoordChanged"),
#endif
 Category("Property Changed")]
		public event EventHandler LeftCoordChanged {
			add { Events.AddHandler(leftCoordChanged, value); }
			remove { Events.RemoveHandler(leftCoordChanged, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListTopVisibleNodeIndexChanged"),
#endif
 Category("Property Changed")]
		public event EventHandler TopVisibleNodeIndexChanged {
			add { Events.AddHandler(topVisibleNodeIndexChanged, value); }
			remove { Events.RemoveHandler(topVisibleNodeIndexChanged, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListDefaultPaintHelperChanged"),
#endif
 Category("Property Changed")]
		public event EventHandler DefaultPaintHelperChanged {
			add { Events.AddHandler(defaultPaintHelperChanged, value); }
			remove { Events.RemoveHandler(defaultPaintHelperChanged, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCellValueChanging"),
#endif
 Category("Property Changed")]
		public event CellValueChangedEventHandler CellValueChanging {
			add { this.Events.AddHandler(cellValueChanging, value); }
			remove { this.Events.RemoveHandler(cellValueChanging, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCellValueChanged"),
#endif
 Category("Property Changed")]
		public event CellValueChangedEventHandler CellValueChanged {
			add { this.Events.AddHandler(cellValueChanged, value); }
			remove { this.Events.RemoveHandler(cellValueChanged, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListShowingEditor"),
#endif
 Category("Editor")]
		public event CancelEventHandler ShowingEditor {
			add { Events.AddHandler(showingEditor, value); }
			remove { Events.RemoveHandler(showingEditor, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListNodesReloaded"),
#endif
 Category("Nodes")]
		public event EventHandler NodesReloaded {
			add { Events.AddHandler(nodesReloaded, value); }
			remove { Events.RemoveHandler(nodesReloaded, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListStartSorting"),
#endif
 Category("Sorting")]
		public event EventHandler StartSorting {
			add { Events.AddHandler(startSorting, value); }
			remove { Events.RemoveHandler(startSorting, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListEndSorting"),
#endif
 Category("Sorting")]
		public event EventHandler EndSorting {
			add { Events.AddHandler(endSorting, value); }
			remove { Events.RemoveHandler(endSorting, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListLayoutUpdated"),
#endif
 Category("Layout")]
		public event EventHandler LayoutUpdated {
			add { Events.AddHandler(layoutUpdated, value); }
			remove { Events.RemoveHandler(layoutUpdated, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListLayoutUpgrade"),
#endif
 Category("Layout")]
		public event LayoutUpgradeEventHandler LayoutUpgrade {
			add { this.Events.AddHandler(layoutUpgrade, value); }
			remove { this.Events.RemoveHandler(layoutUpgrade, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListBeforeLoadLayout"),
#endif
 Category("Layout")]
		public event LayoutAllowEventHandler BeforeLoadLayout {
			add { this.Events.AddHandler(beforeLoadLayout, value); }
			remove { this.Events.RemoveHandler(beforeLoadLayout, value); }
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListVirtualTreeGetChildNodes")]
#endif
		public event VirtualTreeGetChildNodesEventHandler VirtualTreeGetChildNodes {
			add { this.Events.AddHandler(virtualTreeGetChildNodes, value); }
			remove { this.Events.RemoveHandler(virtualTreeGetChildNodes, value); }
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListVirtualTreeGetCellValue")]
#endif
		public event VirtualTreeGetCellValueEventHandler VirtualTreeGetCellValue {
			add { this.Events.AddHandler(virtualTreeGetCellValue, value); }
			remove { this.Events.RemoveHandler(virtualTreeGetCellValue, value); }
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListVirtualTreeSetCellValue")]
#endif
		public event VirtualTreeSetCellValueEventHandler VirtualTreeSetCellValue {
			add { this.Events.AddHandler(virtualTreeSetCellValue, value); }
			remove { this.Events.RemoveHandler(virtualTreeSetCellValue, value); }
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListFilterNode")]
#endif
		public event FilterNodeEventHandler FilterNode {
			add { this.Events.AddHandler(filterNode, value); }
			remove { this.Events.RemoveHandler(filterNode, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListDragObjectDrop"),
#endif
 Category("Drag Drop")]
		public event DragObjectDropEventHandler DragObjectDrop {
			add { this.Events.AddHandler(dragObjectDrop, value); }
			remove { this.Events.RemoveHandler(dragObjectDrop, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListDragObjectStart"),
#endif
 Category("Drag Drop")]
		public event DragObjectStartEventHandler DragObjectStart {
			add { this.Events.AddHandler(dragObjectStart, value); }
			remove { this.Events.RemoveHandler(dragObjectStart, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListDragObjectOver"),
#endif
 Category("Drag Drop")]
		public event DragObjectOverEventHandler DragObjectOver {
			add { this.Events.AddHandler(dragObjectOver, value); }
			remove { this.Events.RemoveHandler(dragObjectOver, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCustomFilterDisplayText"),
#endif
 Category("Behavior")]
		public event ConvertEditValueEventHandler CustomFilterDisplayText {
			add { this.Events.AddHandler(customFilterDisplayText, value); }
			remove { this.Events.RemoveHandler(customFilterDisplayText, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListFilterEditorCreated"),
#endif
 Category("Behavior")]
		public event FilterControlEventHandler FilterEditorCreated {
			add { this.Events.AddHandler(filterEditorCreated, value); }
			remove { this.Events.RemoveHandler(filterEditorCreated, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnFilterChanged"),
#endif
 Category("Behavior")]
		public event EventHandler ColumnFilterChanged {
			add { this.Events.AddHandler(columnFilterChanged, value); }
			remove { this.Events.RemoveHandler(columnFilterChanged, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListShowFilterPopupListBox"),
#endif
 Category("Behavior")]
		public event FilterPopupListBoxEventHandler ShowFilterPopupListBox {
			add { this.Events.AddHandler(showFilterPopupListBox, value); }
			remove { this.Events.RemoveHandler(showFilterPopupListBox, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListShowFilterPopupDate"),
#endif
 Category("Behavior")]
		public event FilterPopupDateEventHandler ShowFilterPopupDate {
			add { this.Events.AddHandler(showFilterPopupDate, value); }
			remove { this.Events.RemoveHandler(showFilterPopupDate, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListShowFilterPopupCheckedListBox"),
#endif
 Category("Behavior")]
		public event FilterPopupCheckedListBoxEventHandler ShowFilterPopupCheckedListBox {
			add { this.Events.AddHandler(showFilterPopupCheckedListBox, value); }
			remove { this.Events.RemoveHandler(showFilterPopupCheckedListBox, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnUnboundExpressionChanged"),
#endif
 Category("Behavior")]
		public event ColumnChangedEventHandler ColumnUnboundExpressionChanged {
			add { this.Events.AddHandler(columnUnboundExpressionChanged, value); }
			remove { this.Events.RemoveHandler(columnUnboundExpressionChanged, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListUnboundExpressionEditorCreated"),
#endif
 Category("Behavior")]
		public event UnboundExpressionEditorEventHandler UnboundExpressionEditorCreated {
			add { this.Events.AddHandler(unboundExpressionEditorCreated, value); }
			remove { this.Events.RemoveHandler(unboundExpressionEditorCreated, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListLoad"),
#endif
 Category("Behavior")]
		public event EventHandler Load {
			add { this.Events.AddHandler(load, value); }
			remove { this.Events.RemoveHandler(load, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListBandWidthChanged"),
#endif
 Category("Behavior")]
		public event BandEventHandler BandWidthChanged {
			add { this.Events.AddHandler(bandWidthChanged, value); }
			remove { this.Events.RemoveHandler(bandWidthChanged, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListPrintExportProgress"),
#endif
 DXCategory(CategoryName.Data)]
		public event ProgressChangedEventHandler PrintExportProgress {
			add { this.Events.AddHandler(printExportProgress, value); }
			remove { this.Events.RemoveHandler(printExportProgress, value); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnPositionChanged"),
#endif
 DXCategory("Behavior")]
		public event EventHandler ColumnPositionChanged {
			add { this.Events.AddHandler(columnPositionChanged, value); }
			remove { this.Events.RemoveHandler(columnPositionChanged, value); }
		}
		#endregion
		public const int AutoFilterNodeId = -100000;
		protected const int LayoutIdAppearance = 1;
		LineStyle treeLineStyle;
		object stateImageList, selectImageList, columnsImageList;
		IComparer nodesComparer;
		ScrollVisibility horzScrollVisibility, vertScrollVisibility;
		TreeListNodesIterator nodesIterator;
		TreeListData data;
		CurrencyManager currencyManager;
		TreeListColumnCollection columns;
		TreeListBandCollection bands;
		TreeListNodes nodes;
		NavigatableControlHelper navigationHelper;
		ErrorInfo errorInfo;
		object dataSource;
		object rootValue;
		string dataMember;
		string keyFieldName, parentFieldName, imageIndexFieldName, previewFieldName;
		TreeListHandler handler;
		TreeListPainter painter;
		TreeListOptionsView optionsView;
		TreeListOptionsSelection optionsSelection;
		TreeListOptionsBehavior optionsBehavior;
		TreeListOptionsNavigation optionsNavigation;
		TreeListOptionsPrint optionsPrint;
		TreeListOptionsMenu optionsMenu;
		TreeListOptionsFilter optionsFilter;
		TreeListOptionsFind optionsFind;
		TreeListOptionsCustomization optionsCustomization;
		TreeListOptionsDragAndDrop optionsDragAndDrop;
		BorderStyles borderStyle;
		ShowButtonModeEnum showButtonMode;
		DevExpress.XtraTreeList.Scrolling.ScrollInfo scrollInfo;
		TreeListViewInfo viewInfo;
		TreeListCustomizationForm customizationForm;
		TreeListMultiSelection selection;
		TreeListPrinter printer;
		UserLookAndFeel lookAndFeel, elementsLookAndFeel;
		CloneInfoCollection cloneInfoList;
		int leftCoord, rowHeight, columnPanelRowHeight, bandPanelRowHeight, footerPanelHeight, indicatorWidth, treeLevelWidth;
		int deletingNodeID;
		int topVisibleNodeIndex, focusedCellIndex, focusedRowIndex, topVisibleNodePixel;
		int previewLineCount, rowAsFooterCount, rowCount, allNodesCount;		
		int lockUpdate, lockScrollUpdate, lockSort, lockUpdateDataSource, lockListChanged, lockUnbound, lockReloadNodes, lockDelete, lockRefresh, lockDeserializing, lockExpandCollapse;
		int horzScrollStep;
		TreeListColumn pressedColumn, sizedColumn;
		TreeListBand pressedBand;
		TreeListNode pressedNode, focusedNode, currentSetFocus, topVisibleNode;
		TreeListAppearanceCollection appearance;
		XtraTreeListBlending blending;
		bool bestFitVisibleOnly, treeListDisposing, mouseIn, isFocusedNodeDataModified;
		Rectangle customizationFormBounds;
		StyleFormatConditionCollection formatConditionsCore;
		FilterConditionCollection filterConditionsCore;
		TreeListFormatRuleCollection formatRules;
		string incrementalText;
		int fixedLineWidth;
		OptionsLayoutTreeList optionsLayout;
		VisibleColumnsList visibleColumns;
		protected internal VisibleColumnsList SortedColumns;
		internal Hashtable autoHeights;
		TreeListAutoFilterNode autoFilterNode;
		CriteriaOperator activeFilterCriteria;
		CriteriaOperator nonColumnFilterCriteria;
		bool activeFilterEnabled;
		TreeListMRUFilterPopup mruFilterPopup = null;
		TreeListColumnFilterPopupBase filterPopup = null;
		TreeListFilterInfoCollection mruFilters;
		TreeListFilterInfo activeFilterInfo;
		string findFilterText = string.Empty;
		string caption = string.Empty;
		int captionHeight = -1;
		ImageCollection htmlImages;
		protected bool allowCloseEditor = true;
		protected bool shouldFitColumns = true;
		protected internal new bool IsRightToLeft { get { return base.IsRightToLeft; } }
		protected override void OnRightToLeftChanged() {
			base.OnRightToLeftChanged();
			EditorHelper.DestroyEditorsCache();
			BeginUpdate();
			try {
				UpdateAppearance();
			}
			finally {
				EndUpdate();
			}
		}
		Dictionary<TreeListNode, int> nodeToVisibleIndexCache = new Dictionary<TreeListNode, int>();
		Dictionary<int, TreeListNode> visibleIndexToNodeCache = new Dictionary<int, TreeListNode>();
		Dictionary<TreeListColumn, TreeListDateFilterInfoCache> dateFilterCache;
		protected internal const string DefaultKeyFieldName = "ID",
			DefaultParentFieldName = "ParentID",
			DefaultImageIndexFieldName = "ImageIndex";
		public TreeList()
			: this(null) {
		}
		protected TreeList(object ignore) {
			RepositoryItemTreeListLookUpEdit.RegisterTreeListLookUpEdit();
			this.appearance = CreateAppearances();
			this.appearance.Changed += new EventHandler(OnAppearanceChanged);
			this.lookAndFeel = new DevExpress.LookAndFeel.Helpers.ControlUserLookAndFeel(this);
			this.lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeel_StyleChanged);
			this.dateFilterCache = new Dictionary<TreeListColumn, TreeListDateFilterInfoCache>();
			this.elementsLookAndFeel = new TreeListEmbeddedLookAndFeel(this);
			UpdateElementsLookAndFeel();
			this.HeaderWidthCalculator = CreateHeaderWidthCalculator();
			this.autoFilterNode = new TreeListAutoFilterNode(this);
			this.mruFilters = new TreeListFilterInfoCollection();
			this.activeFilterEnabled = true;
			this.painter = CreatePainterCore();
			this.viewInfo = CreateViewInfo();
			this.handler = CreateHandler();
			this.printer = CreatePrinter();
			this.customizationForm = null;
			this.navigationHelper = new NavigatableControlHelper();
			this.scrollInfo = CreateScrollInfo();
			this.scrollInfo.VScroll.ValueChanged += new EventHandler(OnVScroll);
			this.scrollInfo.VScroll.Scroll += new ScrollEventHandler(OnVertScroll);
			this.scrollInfo.HScroll.ValueChanged += new EventHandler(OnHScroll);
			this.scrollInfo.VScroll.LookAndFeel.ParentLookAndFeel = ElementsLookAndFeel;
			this.scrollInfo.HScroll.LookAndFeel.ParentLookAndFeel = ElementsLookAndFeel;
			this.Controls.AddRange(new Control[] { scrollInfo.VScroll, scrollInfo.HScroll });
			this.horzScrollVisibility = ScrollVisibility.Auto;
			this.vertScrollVisibility = ScrollVisibility.Auto;
			this.nodes = CreateNodes();
			this.columns = CreateColumns();
			this.bands = CreateBands();
			this.bands.CollectionChanged += new CollectionChangeEventHandler(OnBandCollectionChanged);
			this.columns.CollectionChanged += new CollectionChangeEventHandler(Columns_Changed);
			this.dataSource = null;
			this.dataMember = string.Empty;
			this.currencyManager = null;
			this.data = CreateDataCore();
			this.visibleColumns = new VisibleColumnsList();
			this.SortedColumns = new VisibleColumnsList();
			this.nodesComparer = CreateNodesComparer();
			this.nodesIterator = CreateNodesIterator();
			this.selection = new TreeListMultiSelection(this);
			this.errorInfo = new ErrorInfoEx();
			this.errorInfo.Changed += new EventHandler(OnErrorInfo_Changed);
			this.cloneInfoList = new CloneInfoCollection();
			this.rootValue = 0;
			this.deletingNodeID = -1;
			this.pressedColumn = sizedColumn = null;
			this.pressedNode = null;
			this.currentSetFocus = null;
			this.selectImageList = this.stateImageList = this.columnsImageList = null;
			this.lockUpdate = this.lockScrollUpdate = this.lockSort = this.lockListChanged =
				this.lockUpdateDataSource = this.lockUnbound = this.lockReloadNodes =
				this.lockDelete = this.lockRefresh = this.lockDeserializing = this.lockExpandCollapse = 0;
			this.keyFieldName = DefaultKeyFieldName;
			this.parentFieldName = DefaultParentFieldName;
			this.imageIndexFieldName = DefaultImageIndexFieldName;
			this.previewFieldName = string.Empty;
			this.borderStyle = BorderStyles.Default;
			this.showButtonMode = ShowButtonModeEnum.ShowForFocusedCell;
			this.autoHeights = new Hashtable();
			this.optionsView = CreateOptionsView();
			this.optionsView.Changed += new BaseOptionChangedEventHandler(OnOptionsViewChanged);
			this.optionsBehavior = CreateOptionsBehavior();
			this.optionsBehavior.Changed += new BaseOptionChangedEventHandler(OnOptionsBehaviorChanged);
			this.optionsNavigation = CreateOptionsNavigation();
			this.optionsNavigation.Changed += new BaseOptionChangedEventHandler(OnOptionsNavigationChanged);
			this.optionsPrint = CreateOptionsPrint();
			this.optionsPrint.Changed += new BaseOptionChangedEventHandler(OnOptionsPrintChanged);
			this.optionsSelection = CreateOptionsSelection();
			this.optionsSelection.Changed += new BaseOptionChangedEventHandler(OnOptionsSelectionChanged);
			this.optionsMenu = CreateOptionsMenu();
			this.optionsLayout = CreateOptionsLayout();
			this.optionsFilter = CreateOptionsFilter();
			this.optionsFilter.Changed += new BaseOptionChangedEventHandler(OnOptionsFilterChanged);
			this.optionsFind = CreateOptionsFind();
			this.optionsFind.Changed += new BaseOptionChangedEventHandler(OnOptionsFindChanged);
			this.optionsCustomization = CreateOptionsCustomization();
			this.optionsCustomization.Changed += new BaseOptionChangedEventHandler(OnOptionsCustomizationChanged);
			this.optionsClipboard = CreateOptionsClipboad();
			this.optionsDragAndDrop = CreateOptionsDragAndDrop();
			this.optionsDragAndDrop.Changed += new BaseOptionChangedEventHandler(OnOptionsDragAndDropChanged);
			this.ClearInternalSettings();
			this.rowHeight = -1;
			this.columnPanelRowHeight = -1;
			this.bandPanelRowHeight = -1;
			this.footerPanelHeight = -1;
			this.previewLineCount = 1;
			this.indicatorWidth = -1;
			this.rowAsFooterCount = 0;			
			this.horzScrollStep = 3;
			this.treeLevelWidth = 18;
			this.treeLineStyle = LineStyle.Percent50;
			this.bestFitVisibleOnly = false;
			this.treeListDisposing = false;
			this.mouseIn = false;
			this.blending = null;
			this.customizationFormBounds = Rectangle.Empty;
			this.AllowDrop = OptionsDragAndDrop.DragNodesMode != XtraTreeList.DragNodesMode.None;
			this.incrementalText = "";
			this.fixedLineWidth = 2;
			this.formatRules = new TreeListFormatRuleCollection(this);
			this.formatRules.CollectionChanged += OnFormatRulesCollectionChanged;
			this.formatConditionsCore = new StyleFormatConditionCollection(this);
			this.filterConditionsCore = new FilterConditionCollection(this);
			this.formatConditionsCore.CollectionChanged += new CollectionChangeEventHandler(OnFormatConditionChanged);
			this.filterConditionsCore.CollectionChanged += new CollectionChangeEventHandler(OnFilterCollectionChanged);
			this.BackColor = SystemColors.Control;
			this.ExtraFilterHightlightText = this.ExtraFilter = string.Empty;
			this.UpdateTopVisibleNodeIndexOnCollapse = true;
			this.EnableDynamicLoading = true;
			SetUpControlStyles();
		}		
		protected virtual IComparer CreateNodesComparer() {
			return new NodesComparer(this, SortedColumns);
		}
		protected virtual TreeListAppearanceCollection CreateAppearances() {
			return new TreeListAppearanceCollection(this);
		}
		protected virtual TreeListNodes CreateNodes() {
			return new TreeListNodes(this);
		}
		protected virtual DevExpress.XtraTreeList.Scrolling.ScrollInfo CreateScrollInfo() {
			return new DevExpress.XtraTreeList.Scrolling.ScrollInfo(this);
		}
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		private void SetUpControlStyles() {
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlConstants.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse, true);
		}
		protected virtual void OnFormatConditionChanged(object sender, CollectionChangeEventArgs e) {
			if(!IsLoading) {
				LayoutChanged();
				FireChanged();
			}
		}
		protected virtual void OnFilterCollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(!IsLoading) {
				FilterNodes();
				FireChanged();
			}
		}
		protected virtual void OnFormatRulesCollectionChanged(object sender, FormatConditionCollectionChangedEventArgs e) {
			ViewInfo.SummaryFooterInfo.NeedsRecalcAll = true;
			LayoutChanged();
			FireChanged();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ConvertFormatConditionToFormatRules() {
			if(FormatConditions.Count == 0) return;
			BeginUpdate();
			try {
				FormatRules.BeginUpdate();
				foreach(StyleFormatCondition condition in FormatConditions) {
					TreeListFormatRule format = new TreeListFormatRule() { ApplyToRow = condition.ApplyToRow, Enabled = condition.Enabled, Tag = condition.Tag, Name = condition.Name };
					if(condition.Column != null) 
						format.Column = condition.Column;
					else
						if(!string.IsNullOrEmpty(condition.ColumnName)) format.ColumnName = condition.ColumnName;
					if(condition.Condition == FormatConditionEnum.Expression) {
						FormatConditionRuleExpression ruleExpression = new FormatConditionRuleExpression() { Expression = condition.Expression };
						ruleExpression.Appearance.Assign(condition.Appearance);
						format.Rule = ruleExpression;
					}
					else {
						FormatConditionRuleValue ruleValue = new FormatConditionRuleValue() { Condition = (FormatCondition)condition.Condition, Value1 = condition.Value1, Value2 = condition.Value2 };
						ruleValue.Appearance.Assign(condition.Appearance);
						format.Rule = ruleValue;
					}
					FormatRules.Add(format);
				}
				FormatConditions.Clear();
			}
			finally {
				FormatRules.EndUpdate();
				EndUpdate();
			}
		}
		internal object lastKeyboardMessage = null;
		GestureHelper touchHelper;
		protected override void WndProc(ref Message m) {
			if(touchHelper == null) touchHelper = new GestureHelper(this);
			lastKeyboardMessage = DevExpress.XtraEditors.Senders.BaseSender.SaveMessage(ref m, lastKeyboardMessage);
			if(DevExpress.XtraEditors.Senders.BaseSender.RequireShowEditor(ref m)) ShowEditor();
			if(touchHelper.WndProc(ref m)) return;
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		protected override void Dispose(bool disposing) {
			treeListDisposing = disposing;
			if(disposing && !IsDisposed) {
				DestroyCustomization();
				Data.Dispose();
				searchInfo = null;
				CurrencyManager = null;
				lookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeel_StyleChanged);
				lookAndFeel.Dispose();
				elementsLookAndFeel.Dispose();
				this.appearance.Changed -= new EventHandler(OnAppearanceChanged);
				columns.CollectionChanged -= new CollectionChangeEventHandler(Columns_Changed);
				bands.CollectionChanged -= new CollectionChangeEventHandler(OnBandCollectionChanged);
				scrollInfo.VScroll.ValueChanged -= new EventHandler(OnVScroll);
				scrollInfo.VScroll.Scroll -= new ScrollEventHandler(OnVertScroll);
				scrollInfo.HScroll.ValueChanged -= new EventHandler(OnHScroll);
				errorInfo.Changed -= new EventHandler(OnErrorInfo_Changed);
				errorInfo.ClearErrors();
				cloneInfoList.Clear();
				optionsView.Changed -= new BaseOptionChangedEventHandler(OnOptionsViewChanged);
				optionsBehavior.Changed -= new BaseOptionChangedEventHandler(OnOptionsBehaviorChanged);
				optionsNavigation.Changed -= new BaseOptionChangedEventHandler(OnOptionsNavigationChanged);
				optionsPrint.Changed -= new BaseOptionChangedEventHandler(OnOptionsPrintChanged);
				optionsSelection.Changed -= new BaseOptionChangedEventHandler(OnOptionsSelectionChanged);
				optionsDragAndDrop.Changed -= new BaseOptionChangedEventHandler(OnOptionsDragAndDropChanged);
				printer.Dispose();
				viewInfo.Dispose();
				painter.Dispose();
				handler.Dispose();
				VisibleColumns.Clear();
				Columns.Clear();
				ClearAutoHeights();
				ClearVisibleIndicesCache();
				DestroyDragArrows();
				MRUFilterPopup = null;
				FilterPopup = null;
				DestroyFindPanel();
				LookUpOwner = null;
				ClearDateFilterCache();
			}
			base.Dispose(disposing);
		}
		public static void About() {
			DevExpress.Utils.About.AboutHelper.Show(DevExpress.Utils.About.ProductKind.DXperienceWin, new DevExpress.Utils.About.ProductStringInfoWin(DevExpress.Utils.About.ProductInfoHelper.WinTreeList));
		}
		public virtual void AccessibleNotifyClients(AccessibleEvents accEvent, int objectId, int childId) {
#if DXWhidbey
			AccessibilityNotifyClients(accEvent, objectId, childId);
#endif
		}
#if DXWhidbey
		protected override AccessibleObject GetAccessibilityObjectById(int objectId) {
			TreeListControlAccessible acc = DXAccessible as TreeListControlAccessible;
			if(acc == null) return base.GetAccessibilityObjectById(objectId);
			return acc.GetAccessibleObjectById(objectId, -1);
		}
#endif
		#region Properties
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListDataSource"),
#endif
 Category("Data"), DefaultValue(null),
#if DXWhidbey
 AttributeProvider(typeof(IListSource))]
#else
 TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
#endif
		public object DataSource {
			get { return dataSource; }
			set {
				if(DataSource == value) return;
				dataSource = value;
				UpdateDataSourceOnChangeDataSource();
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListDataMember"),
#endif
 Category("Data"), DefaultValue(""), Localizable(true), Editor(ControlConstants.DataMemberEditor, typeof(System.Drawing.Design.UITypeEditor))]
		public string DataMember {
			get { return dataMember; }
			set {
				if(value == null) value = string.Empty;
				if(DataMember == value) return;
				dataMember = value;
				UpdateDataSourceOnChangeDataSource();
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListRootValue"),
#endif
 Category("Data"), DefaultValue(0), Localizable(true),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public object RootValue {
			get { return rootValue; }
			set {
				if(RootValue == value) return;
				rootValue = value;
				OnProperyChanged();
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListStateImageList"),
#endif
 Category("Appearance"), DefaultValue(null),
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))
		]
		public object StateImageList {
			get { return stateImageList; }
			set {
				if(StateImageList != value) {
					stateImageList = value;
					ViewInfo.RC.NeedsRestore = true;
					if(printer != null) printer.PrintLayoutChanged();
					OnProperyChanged();
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListSelectImageList"),
#endif
 Category("Appearance"), DefaultValue(null),
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))
		]
		public object SelectImageList {
			get { return selectImageList; }
			set {
				if(SelectImageList != value) {
					selectImageList = value;
					ViewInfo.RC.NeedsRestore = true;
					if(printer != null) printer.PrintLayoutChanged();
					OnProperyChanged();
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnsImageList"),
#endif
 Category("Appearance"), DefaultValue(null),
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))
		]
		public object ColumnsImageList {
			get { return columnsImageList; }
			set {
				if(ColumnsImageList != value) {
					columnsImageList = value;
					ViewInfo.RC.NeedsRestore = true;
					OnProperyChanged();
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListImageIndexFieldName"),
#endif
 Category("Data"), DefaultValue(DefaultImageIndexFieldName),
		TypeConverter("DevExpress.XtraTreeList.Design.TypeConverters.FieldNameTypeConverter, " + AssemblyInfo.SRAssemblyTreeListDesign),
		XtraSerializableProperty()]
		public string ImageIndexFieldName {
			get { return imageIndexFieldName; }
			set {
				if(value == null) value = string.Empty;
				if(ImageIndexFieldName != value) {
					imageIndexFieldName = value;
					ServiceColumnChanged(ServiceColumnEnum.ImageIndexFieldName, ImageIndexFieldName);
				}
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListKeyFieldName"),
#endif
 Category("Data"), DefaultValue(DefaultKeyFieldName),
		TypeConverter("DevExpress.XtraTreeList.Design.TypeConverters.FieldNameTypeConverter, " + AssemblyInfo.SRAssemblyTreeListDesign),
		XtraSerializableProperty()]
		public string KeyFieldName {
			get { return keyFieldName; }
			set {
				if(value == null) value = string.Empty;
				if(KeyFieldName != value) {
					keyFieldName = value;
					ServiceColumnChanged(ServiceColumnEnum.KeyFieldName, KeyFieldName);
				}
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListParentFieldName"),
#endif
 Category("Data"), DefaultValue(DefaultParentFieldName),
		TypeConverter("DevExpress.XtraTreeList.Design.TypeConverters.FieldNameTypeConverter, " + AssemblyInfo.SRAssemblyTreeListDesign),
		XtraSerializableProperty()]
		public string ParentFieldName {
			get { return parentFieldName; }
			set {
				if(value == null) value = string.Empty;
				if(ParentFieldName != value) {
					parentFieldName = value;
					ServiceColumnChanged(ServiceColumnEnum.ParentFieldName, ParentFieldName);
				}
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListPreviewFieldName"),
#endif
 Category("Data"), DefaultValue(""),
		TypeConverter("DevExpress.XtraTreeList.Design.TypeConverters.FieldNameTypeConverter, " + AssemblyInfo.SRAssemblyTreeListDesign),
		XtraSerializableProperty()]
		public string PreviewFieldName {
			get { return previewFieldName; }
			set {
				if(value == null) value = string.Empty;
				if(PreviewFieldName != value) {
					previewFieldName = value;
					InvalidatePixelScrollingInfo();
					OnProperyChanged();
					LayoutChanged();
				}
			}
		}
		bool ShouldSerializeOptionsView() { return OptionsView.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsView"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public TreeListOptionsView OptionsView { get { return optionsView; } }
		bool ShouldSerializeOptionsSelection() { return OptionsSelection.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsSelection"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public TreeListOptionsSelection OptionsSelection { get { return optionsSelection; } }
		bool ShouldSerializeOptionsBehavior() { return OptionsBehavior.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehavior"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public TreeListOptionsBehavior OptionsBehavior { get { return optionsBehavior; } }
		bool ShouldSerializeOptionsPrint() { return OptionsPrint.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsPrint"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public TreeListOptionsPrint OptionsPrint { get { return optionsPrint; } }
		bool ShouldSerializeOptionsMenu() { return OptionsMenu.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsMenu"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public TreeListOptionsMenu OptionsMenu { get { return optionsMenu; } }
		bool ShouldSerializeOptionsLayout() { return OptionsLayout.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsLayout"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OptionsLayoutTreeList OptionsLayout { get { return optionsLayout; } }
		bool ShouldSerializeOptionsFilter() { return OptionsFilter.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsFilter"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListOptionsFilter OptionsFilter { get { return optionsFilter; } }
		bool ShouldSerializeOptionsFind() { return OptionsFind.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsFind"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListOptionsFind OptionsFind { get { return optionsFind; } }
		bool ShouldSerializeOptionsCustomization() { return OptionsCustomization.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsCustomization"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListOptionsCustomization OptionsCustomization { get { return optionsCustomization; } }
		bool ShouldSerializeOptionsNavigation() { return OptionsNavigation.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsNavigation"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public TreeListOptionsNavigation OptionsNavigation { get { return optionsNavigation; } }
		bool ShouldSerializeOptionsDragAndDrop() { return OptionsDragAndDrop.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsDragAndDrop"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public TreeListOptionsDragAndDrop OptionsDragAndDrop { get { return optionsDragAndDrop; } }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListBorderStyle"),
#endif
 Category("Appearance"), DefaultValue(BorderStyles.Default), XtraSerializableProperty()]
		public BorderStyles BorderStyle {
			get { return borderStyle; }
			set {
				if(BorderStyle == value) return;
				borderStyle = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListShowButtonMode"),
#endif
 Category("Appearance"), DefaultValue(ShowButtonModeEnum.ShowForFocusedCell), Localizable(true),
		XtraSerializableProperty()]
		public ShowButtonModeEnum ShowButtonMode {
			get { return showButtonMode; }
			set {
				if(ShowButtonMode != value) {
					showButtonMode = value;
					OnProperyChanged();
					LayoutChanged();
				}
			}
		}
		protected internal virtual ShowButtonModeEnum GetColumnShowButtonMode(TreeListColumn column) {
			ShowButtonModeEnum showMode = column.ShowButtonMode;
			if(showMode == ShowButtonModeEnum.Default) showMode = ShowButtonMode;
			if(showMode == ShowButtonModeEnum.Default) showMode = ShowButtonModeEnum.ShowForFocusedCell;
			return showMode;
		}
		[Obsolete(), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int GroupButtonSize {
			get { return -1; }
			set { }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListIndicatorWidth"),
#endif
 DefaultValue(-1), Category("Appearance"), XtraSerializableProperty()]
		public virtual int IndicatorWidth {
			get { return indicatorWidth; }
			set {
				if(value < 4) value = -1;
				if(IndicatorWidth == value) return;
				indicatorWidth = value;
				OnProperyChanged();
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListPreviewLineCount"),
#endif
 Category("Appearance"), DefaultValue(1), Localizable(true),
		XtraSerializableProperty()]
		public int PreviewLineCount {
			get { return previewLineCount; }
			set {
				if(value < 1) value = 1;
				if(value > 10) value = 10;
				if(PreviewLineCount != value) {
					previewLineCount = value;
					OnProperyChanged();
					LayoutChanged();
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Obsolete("The Customization form can be resized at runtime. Use the CustomizationFormBounds property to specify the size of the form, in pixels.")]
		public int CustomizationRowCount {
			get { return 7; }
			set {  }
		}
		protected virtual int MinRowHeight { get { return 8; } }
		internal int CheckRowHeight(int value) {
			if(value < MinRowHeight && value != -1) value = MinRowHeight;
			return value;
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListRowHeight"),
#endif
 DefaultValue(-1), Localizable(true), Category("Appearance"), XtraSerializableProperty()]
		public int RowHeight {
			get { return rowHeight; }
			set {
				value = CheckRowHeight(value);
				if(RowHeight == value) return;
				rowHeight = value;
				viewInfo.RC.NeedsRestore = true;
				ClearAutoHeights();
				viewInfo.PixelScrollingInfo.Invalidate();
				OnProperyChanged();
				LayoutChanged();
			}
		}
		protected override void ScaleControl(SizeF factor, BoundsSpecified specified) {
			if(factor.Width != ScaleFactor.Width || factor.Height != ScaleFactor.Height)
				viewInfo.RC.NeedsRestore = true;
			base.ScaleControl(factor, specified);
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnPanelRowHeight"),
#endif
 DefaultValue(-1), Localizable(true), Category("Appearance"), XtraSerializableProperty()]
		public virtual int ColumnPanelRowHeight {
			get { return columnPanelRowHeight; }
			set {
				if(value < -1) value = -1;
				if(ColumnPanelRowHeight != value) {
					columnPanelRowHeight = value;
					viewInfo.RC.NeedsRestore = true;
					OnProperyChanged();
					LayoutChanged();
				}
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListBandPanelRowHeight"),
#endif
 DefaultValue(-1), XtraSerializableProperty()]
		public virtual int BandPanelRowHeight {
			get { return bandPanelRowHeight; }
			set {
				if(value < -1) value = -1;
				if(BandPanelRowHeight == value) return;
				bandPanelRowHeight = value;
				viewInfo.RC.NeedsRestore = true;
				OnProperyChanged();
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListFooterPanelHeight"),
#endif
 DefaultValue(-1), Category("Appearance"), XtraSerializableProperty()]
		public virtual int FooterPanelHeight {
			get { return footerPanelHeight; }
			set {
				if(value < -1) value = -1;
				if(FooterPanelHeight == value) return;
				footerPanelHeight = value;
				ViewInfo.RC.NeedsRestore = true;
				OnProperyChanged();
				LayoutChanged();
			}
		}
		[Obsolete("Use the OptionsDragAndDrop.DragNodesExpandDelay property instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int DragExpandDelay {
			get { return OptionsDragAndDrop.DragNodesExpandDelay; }
			set { OptionsDragAndDrop.DragNodesExpandDelay = value; }
		}
		[Browsable(false)]
		public TreeListCustomizationForm CustomizationForm { get { return customizationForm; } }
		[Browsable(false)]
		public TreeListNodesIterator NodesIterator { get { return nodesIterator; } }
		[Browsable(false)]
		public TreeListPainter Painter { get { return painter; } }
		[Browsable(false)]
		public TreeListState State { get { return Handler.State; } }
		[Browsable(false)]
		public virtual bool IsUnboundMode { get { return Data.IsUnboundMode; } }
		[Browsable(false)]
		public virtual bool CanShowEditor {
			get {
				if(State != TreeListState.Regular && State != TreeListState.NodePressed) return false;
				if(IsFilterRow(FocusedRowIndex)) {
					if(!IsColumnAllowAutoFilter(FocusedColumn)) return false;
					return true;
				}
				if(!Editable) return false;
				if(FocusedRowIndex < 0 || FocusedRowIndex > RowCount - 1 || FocusedCellIndex < 0 || FocusedCellIndex > VisibleColumns.Count - 1) return false;
				if(!FocusedColumn.OptionsColumn.AllowEdit) return false;
				bool cancel = false;
				if(FocusedColumn.ColumnEdit != null && !FocusedColumn.ColumnEdit.Editable) return false;
				RaiseShowingEditor(ref cancel);
				return !cancel;
			}
		}
		protected internal virtual bool Editable { get { return OptionsBehavior.Editable && !IsLookUpMode; } }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListBestFitVisibleOnly"),
#endif
 Category("Behavior"), DefaultValue(false), Localizable(true),
		XtraSerializableProperty()]
		public bool BestFitVisibleOnly {
			get { return bestFitVisibleOnly; }
			set {
				if(BestFitVisibleOnly == value) return;
				bestFitVisibleOnly = value;
				OnProperyChanged();
			}
		}
		bool TreeListFormDisposing {
			get {
				Form form = FindForm();
				return (form == null ? false : form.Disposing);
			}
		}
		[Browsable(false)]
		public bool TreeListDisposing { get { return treeListDisposing; } }
		[Browsable(false)]
		public TreeListMultiSelection Selection { get { return selection; } }
		bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListLookAndFeel"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		protected internal UserLookAndFeel ElementsLookAndFeel { get { return elementsLookAndFeel; } }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListHorzScrollVisibility"),
#endif
 Category("Layout"), DefaultValue(ScrollVisibility.Auto), Localizable(true), XtraSerializableProperty()]
		public virtual ScrollVisibility HorzScrollVisibility {
			get { return horzScrollVisibility; }
			set {
				if(HorzScrollVisibility != value) {
					horzScrollVisibility = value;
					OnProperyChanged();
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListHorzScrollStep"),
#endif
 DefaultValue(3), XtraSerializableProperty(), Category("Behavior")]
		public int HorzScrollStep {
			get { return horzScrollStep; }
			set {
				value = Math.Min(100, Math.Max(value, 1));
				if(HorzScrollStep == value) return;
				horzScrollStep = value;
				OnProperyChanged();
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListVertScrollVisibility"),
#endif
 DefaultValue(ScrollVisibility.Auto), Localizable(true), XtraSerializableProperty(), Category("Layout")]
		public virtual ScrollVisibility VertScrollVisibility {
			get { return vertScrollVisibility; }
			set {
				if(VertScrollVisibility != value) {
					vertScrollVisibility = value;
					OnProperyChanged();
					LayoutChanged();
				}
			}
		}
		[Browsable(false)]
		public int SortedColumnCount {
			get {
				if(lockSort == 0) return SortedColumns.Count;
				int count = 0;
				foreach(TreeListColumn col in Columns) {
					if(col.SortOrder != SortOrder.None || col.SortIndex > -1)
						count++;
				}
				return count;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false)]
		public TreeListNode FocusedNode {
			get {
				if(focusedNode == null && FocusedRowIndex != -1)
					SetFocusedNodeCore(GetNodeByVisibleIndex(FocusedRowIndex));
				return focusedNode;
			}
			set {
				if(FocusedNode != value)
					SetFocusedNode(value);
			}
		}
		bool ShouldSerializeCustomizationFormBounds() { return CustomizationFormBounds != Rectangle.Empty; }
		[Browsable(false), XtraSerializableProperty()]
		public Rectangle CustomizationFormBounds {
			get { return customizationFormBounds; }
			set {
				value = TreeListCustomizationForm.CheckCustomizationFormBounds(value);
				if(CustomizationFormBounds == value) return;
				customizationFormBounds = value;
				if(CustomizationForm != null && !value.IsEmpty) CustomizationForm.Bounds = CustomizationFormBounds;
				OnProperyChanged();
			}
		}
		protected internal TreeListData Data {
			get { return data; }
			set {
				if(Data != null) Data.Dispose();
				data = value;
			}
		}
		protected CurrencyManager CurrencyManager {
			get { return currencyManager; }
			set {
				if(CurrencyManager == value) return;
				if(CurrencyManager != null) {
					CurrencyManager.PositionChanged -= new EventHandler(CurrencyManager_PositionChanged);
					CurrencyManager.ItemChanged -= new ItemChangedEventHandler(CurrencyManager_ItemChanged);
				}
				currencyManager = value;
				if(CurrencyManager != null) {
					CurrencyManager.PositionChanged += new EventHandler(CurrencyManager_PositionChanged);
					CurrencyManager.ItemChanged += new ItemChangedEventHandler(CurrencyManager_ItemChanged);
				}
			}
		}
		protected int Position {
			get {
				if(CurrencyManager == null) return -1;
				return CurrencyManager.Position;
			}
			set {
				if(CurrencyManager == null || IsLookUpMode) return;
				if(value < -1) value = -1;
				if(value > CurrencyManager.List.Count - 1) value = CurrencyManager.List.Count - 1;
				if(CurrencyManager.Position == value) return;
				CurrencyManager.Position = value;
			}
		}
		internal DevExpress.XtraTreeList.Scrolling.ScrollInfo ScrollInfo { get { return scrollInfo; } }
		[Browsable(false)]
		public virtual bool HasFocus {
			get {
				if(ContainsFocus || ContainerHelper.InternalFocusLock != 0) return true;
				if(ActiveEditor != null)
					return ActiveEditor.EditorContainsFocus;
				return false;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LeftCoord {
			get { return leftCoord; }
			set {
				int hsRange = CalcHScrollRange() - ViewInfo.ViewRects.IndicatorWidth - ViewInfo.ViewRects.ColumnPanelWidth;
				if(value > hsRange) value = hsRange;
				if(value < 0 ) value = 0;
				if(LeftCoord != value) {
					CloseEditor();
					leftCoord = value;
					DoLeftCoordChanged();
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int TopVisibleNodeIndex {
			get { return topVisibleNodeIndex; }
			set {
				if(value >= RowCount) value = RowCount - 1;
				if(value < 0) value = 0;
				if(value == TopVisibleNodeIndex) return;
				if(IsPixelScrolling)
					TopVisibleNodePixel = ViewInfo.CalcPixelPositionByVisibleIndex(value);
				else
					InternalSetTopVisibleNodeIndex(value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int TopVisibleNodePixel {
			get { return topVisibleNodePixel; }
			set {
				if(value < 0) value = 0;
				if(value == TopVisibleNodePixel) return;
				InternalSetTopVisibleNodePixel(value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleNodesCount {
			get {
				return RowCount;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int AllNodesCount {
			get {
				if(this.allNodesCount < Nodes.Count)
					this.allNodesCount = CalcAllNodesCount();
				return this.allNodesCount;
			}
		}
		[Obsolete("Use the OptionsLayout.LayoutVersion property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string LayoutVersion {
			get { return OptionsLayout.LayoutVersion; }
			set { OptionsLayout.LayoutVersion = value; }
		}
		protected internal TreeListHandler Handler { get { return handler; } }
		protected internal virtual int FocusedRowIndex {
			get { return focusedRowIndex; }
			set {
				if(value < -1 && !IsFilterRow(value)) value = -1;
				if(value >= RowCount) value = RowCount - 1;
				if(value == FocusedRowIndex) return;
				InternalSetFocusedRowIndex(value);
			}
		}
		protected internal virtual RowInfo FocusedRow {
			get {
				if(IsFilterRow(FocusedRowIndex))
					return ViewInfo.AutoFilterRowInfo;
				RowInfo ri = null;
				if(!ViewInfo.IsValid) ViewInfo.CalcViewInfo();
				UpdateScrollBars();
				int visibleIndex = FocusedRowIndex - TopVisibleNodeIndex;
				if(visibleIndex > -1 && visibleIndex < ViewInfo.RowsInfo.Rows.Count) {
					ri = ViewInfo.RowsInfo.Rows[visibleIndex] as RowInfo;
				}
				return ri;
			}
		}
		protected internal virtual int FocusedCellIndex {
			get { return focusedCellIndex; }
			set {
				int focusRowInd = FocusedRowIndex;
				if(value >= VisibleColumns.Count) value = VisibleColumns.Count - 1;
				if(value < 0) value = 0;
				if(value == FocusedCellIndex) return;
				InternalSetFocusedCellIndex(value, 0);
				FocusedRowIndex = Math.Min(focusRowInd, RowCount - 1);
			}
		}
		protected internal virtual TreeListColumn PressedColumn {
			get { return pressedColumn; }
			set {
				if(PressedColumn != value) {
					pressedColumn = value;
					RefreshColumnsInfo();
				}
			}
		}
		protected internal TreeListBand PressedBand {
			get { return pressedBand; }
			set {
				if(PressedBand == value)
					return;
				pressedBand = value;
				OnPressedBandChanged();
			}
		}
		protected internal TreeListBand SizedBand { get; set; }
		protected virtual void OnPressedBandChanged() {			
			RefreshHeadersInfo(viewInfo.ViewRects.ActualBandPanel);
		}
		protected internal virtual TreeListNode PressedNode {
			get { return pressedNode; }
			set {
				if(PressedNode != value) {
					pressedNode = value;
				}
			}
		}
		protected internal virtual TreeListColumn SizedColumn {
			get { return sizedColumn; }
			set {
				if(SizedColumn != value) {
					sizedColumn = value;
				}
			}
		}
		protected internal virtual string IncrementalText {
			get { return incrementalText; }
			set {
				if(value == null) value = "";
				if(IncrementalText == value) return;
				incrementalText = value;
			}
		}
		protected internal bool IsAutoWidth { get { return OptionsView.AutoWidth; } }
		protected internal TreeListContainerHelper ContainerHelper { get { return base.EditorHelper as TreeListContainerHelper; } }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListNodes"),
#endif
 Category("Data"), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraTreeList.Design.NodesEditor, " + AssemblyInfo.SRAssemblyTreeListDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public TreeListNodes Nodes { get { return nodes; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(true, true, true)]
		public TreeListColumnCollection Columns { get { return columns; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 1, XtraSerializationFlags.DefaultValue)]
		public TreeListBandCollection Bands { get { return bands; } }
		internal void XtraClearBands(XtraItemEventArgs e) { Bands.ClearBandItems(e); }
		internal object XtraCreateBandsItem(XtraItemEventArgs e) { return Bands.CreateBandItem(e); }
		internal object XtraFindBandsItem(XtraItemEventArgs e) { return Bands.FindBandItem(e); }
		internal void XtraSetIndexBandsItem(XtraSetItemIndexEventArgs e) { Bands.SetBandItemIndex(e); }
		[Browsable(false)]
		public VisibleColumnsList VisibleColumns { get { return visibleColumns; } }
		[Browsable(false)]
		public TreeListViewInfo ViewInfo { get { return viewInfo; } }
		[DefaultValue(null), 
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListHtmlImages")
#else
	Description("")
#endif
]
		public ImageCollection HtmlImages {
			get { return htmlImages; }
			set {
				if(htmlImages == value) return;
				if(htmlImages != null)
					htmlImages.Changed -= OnHtmlImagesChanged;
				htmlImages = value;
				if(htmlImages != null)
					htmlImages.Changed += OnHtmlImagesChanged;
				LayoutChanged();
			}
		}
		void OnHtmlImagesChanged(object sender, EventArgs e) {
			if(OptionsView.AllowHtmlDrawHeaders)
				LayoutChanged();
		}
		protected HeaderWidthCalculator HeaderWidthCalculator { get; private set; }
		protected virtual HeaderWidthCalculator CreateHeaderWidthCalculator() {
			return new HeaderWidthCalculator();
		}
		public List<TreeListNode> GetNodeList() { 
			List<TreeListNode> nodeList = new List<TreeListNode>();
			NodesIterator.DoOperation((node) => { if(node.Visible) nodeList.Add(node); });
			return nodeList;
		}
		protected int ClientWidth {
			get {
				int vw = this.ClientSize.Width - (scrollInfo.VScrollVisible && !scrollInfo.IsOverlapScrollbar ? scrollInfo.VScrollWidth : 0);
				return Math.Max(0, vw - ViewInfo.BorderSize.Width);
			}
		}
		protected int ClientHeight {
			get {
				int vh = this.ClientSize.Height - (scrollInfo.HScrollVisible ? scrollInfo.HScrollHeight : 0);
				return Math.Max(0, vh - ViewInfo.BorderSize.Height);
			}
		}
		protected internal virtual bool IsNeededHScrollBar {
			get {
				if(HorzScrollVisibility == ScrollVisibility.Never) return false;
				if(HorzScrollVisibility == ScrollVisibility.Always) return true;
				if(IsAutoWidth && !AllowBandColumnsMultiRow) return GetMinColumnsWidthRight(0, false) > ClientWidth - viewInfo.ViewRects.IndicatorWidth;
				return (viewInfo.ViewRects.ColumnTotalWidth > ClientWidth);
			}
		}
		bool? isNeededVScrollBarInternal = null;
		protected internal virtual bool IsNeededVScrollBar {
			get {
				if(VertScrollVisibility == ScrollVisibility.Never) return false;
				if(VertScrollVisibility == ScrollVisibility.Always) return true;
				if(Nodes.Count == 0) return false;
				if(TopVisibleNodeIndex != 0) return true;
				if(IsPixelScrolling && TopVisibleNodePixel > 0)
					return true;
				if(isNeededVScrollBarInternal != null)
					return isNeededVScrollBarInternal.Value;
				if(ViewInfo.IsValid)
					return (viewInfo.ViewRects.RowsTotalHeight > viewInfo.ViewRects.Rows.Height ||
						RowCount > viewInfo.VisibleRowCount || !LowestRowFooterIsVisible);
				return VisibleNodesCount > ViewInfo.CalcVisibleNodeCount(TopVisibleNode, CalculateVisibleRowsAreaHeight());
			}
		}
		protected internal virtual int CalculateVisibleRowsAreaHeight() {
			return ClientHeight - ViewInfo.ColumnPanelHeight - ViewInfo.BandPanelRowHeight * ViewInfo.BandPanelRowCount - ViewInfo.FooterPanelHeight - GetFilterObjectsTotalHeight();
		}
		int GetFilterObjectsTotalHeight() {
			int result = 0;
			if(ViewInfo.ShowFilterPanel)
				result += ViewInfo.GetFilterPanelHeight();
			if(OptionsView.ShowAutoFilterRow)
				result += ViewInfo.GetSpecialRowSeparatorHeight() + ViewInfo.GetTotalNodeHeight(AutoFilterNode);
			if(FindPanelVisible && FindPanel != null)
				result += FindPanel.Height;
			return result;
		}
		internal bool PreviewColumnExists {
			get { return Data.Columns[PreviewFieldName] != null; }
		}
		protected internal bool HasAllColumnsFixedRight { get { return VisibleColumns.Count == GetFixedRightColumnCount(); } }
		int GetFixedRightColumnCount() {
			int count = 0;
			for(int i = 0; i < VisibleColumns.Count; i++)
				if(VisibleColumns[i].Fixed == FixedStyle.Right)
					count++;
			return count;
		}
		private bool LowestRowFooterIsVisible { get { return (LowestRowFooterLines == 0); } }
		protected internal virtual int RowCount {
			get {
				if(rowCount <= 0) {
					rowCount = CalcRowCount();
					ViewInfo.PixelScrollingInfo.Invalidate();
				}
				return rowCount;
			}
			set {
				if(rowCount != value) {
					rowCount = value;
					LayoutChanged();
				}
			}
		}
		internal protected void RecalcRowCount() {
			ResetRowCount();
			ViewInfo.PixelScrollingInfo.Invalidate();
		}
		protected override Size DefaultSize { get { return new Size(400, 200); } }
		protected internal bool IsIniting { get { return IsLoading || !IsInitialized; } }
		protected virtual bool CanForceLoadNodes {
			get { return (Nodes.Count == 0); }
		}
		bool CanUseTab { get { return OptionsNavigation.UseTabKey; } }
		[Browsable(false)]
		public virtual bool IsDesignMode {
			get { return DesignMode; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 1000, XtraSerializationFlags.DefaultValue)]
		public virtual StyleFormatConditionCollection FormatConditions { get { return formatConditionsCore; } }
		internal void XtraClearFormatConditions(XtraItemEventArgs e) {
			FormatConditions.Clear();
		}
		internal object XtraCreateFormatConditionsItem(XtraItemEventArgs e) {
			StyleFormatCondition formatCondition = new StyleFormatCondition();
			FormatConditions.Add(formatCondition);
			return formatCondition;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 1001, XtraSerializationFlags.DefaultValue)]
		public virtual FilterConditionCollection FilterConditions { get { return filterConditionsCore; } }
		internal void XtraClearFilterConditions(XtraItemEventArgs e) {
			FilterConditions.Clear();
		}
		internal object XtraCreateFilterConditionsItem(XtraItemEventArgs e) {
			FilterCondition filterCondition = new FilterCondition();
			FilterConditions.Add(filterCondition);
			return filterCondition;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 1002, XtraSerializationFlags.DefaultValue)]
		public virtual TreeListFormatRuleCollection FormatRules { get { return formatRules; } }
		internal void XtraClearFormatRules(XtraItemEventArgs e) { FormatRules.Clear(); }
		internal object XtraCreateFormatRulesItem(XtraItemEventArgs e) { 
			return FormatRules.AddInstance(); 
		}
		protected internal virtual bool ActualAutoNodeHeight { get { return OptionsBehavior.AutoNodeHeight; } }
		#endregion
		void OnAppearanceChanged(object sender, EventArgs e) { UpdateAppearance(); }
		void UpdateAppearance() {
			FireChanged();
			viewInfo.SetAppearanceDirty();
			viewInfo.RC.NeedsRestore = true;
			viewInfo.SummaryFooterInfo.NeedsRecalcRects = true;
			ViewInfo.PixelScrollingInfo.Invalidate();
			ClearAutoHeights();
			FitColumns();
			LayoutChanged();
			RefreshCustomizationForm();
			OnProperyChanged();
		}
		int minWidthCore = 0;
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListMinWidth"),
#endif
 Category("Layout"), DefaultValue(0)]
		public int MinWidth {
			get {
				return minWidthCore;
			}
			set {
				minWidthCore = value;
			}
		}
		[Obsolete("Use the OptionsDragAndDrop.DropNodesMode property instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeListDragNodesMode DragNodesMode {
			get { return (TreeListDragNodesMode)OptionsDragAndDrop.DropNodesMode; }
			set { OptionsDragAndDrop.DropNodesMode = (DropNodesMode)value; }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCaption"),
#endif
 DefaultValue(""), XtraSerializableProperty(), Category("Appearance"), Localizable(true)]
		public string Caption {
			get { return caption; }
			set {
				if(value == null) value = string.Empty;
				if(Caption == value) return;
				caption = value;
				OnProperyChanged();
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListCaptionHeight"),
#endif
 DefaultValue(-1), Category("Appearance"), XtraSerializableProperty()]
		public int CaptionHeight {
			get { return captionHeight; }
			set {
				if(value < -1) value = -1;
				if(CaptionHeight == value) return;
				captionHeight = value;
				OnProperyChanged();
				LayoutChanged();
			}
		}
		[ DefaultValue(true), Category("Behavior"), XtraSerializableProperty()]
		public bool EnableDynamicLoading { get; set; }
		#region Accessibility
		protected override AccessibleObject CreateAccessibilityInstance() {
			if(DXAccessible == null) return base.CreateAccessibilityInstance();
			return DXAccessible.Accessible;
		}
		protected internal virtual TreeListControlAccessible CreateAccessibleInstance() {
			return new TreeListControlAccessible(this);
		}
		TreeListControlAccessible treeListAccesible = null;
		protected internal virtual BaseAccessible DXAccessible {
			get {
				if(treeListAccesible == null) treeListAccesible = CreateAccessibleInstance();
				return treeListAccesible;
			}
		}
		int IAccessibleGrid.FindRow(int x, int y) {
			TreeListHitTest ht = this.ViewInfo.GetHitTest(new Point(x, y));
			if(ht != null)
				return ht.RowInfo.VisibleIndex;
			return -1;
		}
		IAccessibleGridRow IAccessibleGrid.GetRow(int index) {
			return CreateAccessibleDataRow(index);
		}
		protected virtual IAccessibleGridRow CreateAccessibleDataRow(int index) {
			return new DevExpress.XtraTreeList.Accessibility.TreeListAccessibleDataRow(this, index);
		}
		int IAccessibleGrid.HeaderCount { get { return this.VisibleColumns.Count; } }
		ScrollBarBase IAccessibleGrid.HScroll { get { return ScrollInfo.HScrollVisible ? ScrollInfo.HScroll : null; } }
		ScrollBarBase IAccessibleGrid.VScroll { get { return ScrollInfo.VScrollVisible ? ScrollInfo.VScroll : null; } }
		int IAccessibleGrid.RowCount {
			get {
				return this.CalcAllNodesCount();
			}
		}
		int IAccessibleGrid.SelectedRow {
			get {
				if(this.FocusedNode != null) {
					return this.FocusedNode.Id;
				}
				return -1;
			}
		}
		#endregion
		#region Styles
		public void FireChanged() {
			if(LookUpOwner != null && LookUpOwner.IsDesignMode) LookUpOwner.FireChangedCore();
			DevExpress.XtraTreeList.Helpers.DesignHelper.FireChanged(this, IsLoading, IsDesignMode, GetService(typeof(IComponentChangeService)) as IComponentChangeService);
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListTreeLineStyle"),
#endif
 Category("Appearance"), DefaultValue(LineStyle.Percent50), XtraSerializableProperty()]
		public LineStyle TreeLineStyle {
			get { return treeLineStyle; }
			set {
				if(treeLineStyle != value) {
					treeLineStyle = value;
					if(!IsIniting)
						viewInfo.LineBrushChanged(treeLineStyle, ViewInfo.PaintAppearance.TreeLine);
					OnProperyChanged();
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListTreeLevelWidth"),
#endif
 Category("Appearance"), DefaultValue(18), XtraSerializableProperty()]
		public int TreeLevelWidth {
			get { return treeLevelWidth; }
			set {
				if(value < 12) value = 12;
				if(value > 100) value = 100;
				if(TreeLevelWidth == value) return;
				treeLevelWidth = value;
				ViewInfo.RC.NeedsRestore = true;
				OnProperyChanged();
				LayoutChanged();
			}
		}
		void ResetAppearance() { Appearance.Reset(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearance"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Category("Appearance"), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdAppearance)]
		public TreeListAppearanceCollection Appearance { get { return appearance; } }
		void ResetAppearancePrint() { AppearancePrint.Reset(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearancePrint"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Category("Printing"), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdAppearance)]
		public TreeListPrintAppearanceCollection AppearancePrint { get { return printer.Appearance; } }
		protected internal virtual void OnAppearancePrintChanged() {
			FireChanged();
		}
		bool useDisabledStatePainterCore = true;
		[DefaultValue(true), 
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListUseDisabledStatePainter"),
#endif
 Category("Appearance")]
		public bool UseDisabledStatePainter {
			get { return useDisabledStatePainterCore; }
			set {
				if(useDisabledStatePainterCore != value) {
					useDisabledStatePainterCore = value;
					Invalidate();
				}
			}
		}
		internal XtraTreeListBlending Blending { get { return blending; } set { blending = value; } }
		internal bool CanUpdatePaintAppearanceBlending { get { return (!DesignMode && Blending != null && Blending.Enabled); } }
		#endregion
		#region Editors
		[Browsable(false)]
		public DevExpress.XtraEditors.BaseEdit ActiveEditor { get { return ContainerHelper.ActiveEditor; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object EditingValue {
			get {
				if(ActiveEditor == null) return null;
				return ActiveEditor.EditValue;
			}
			set {
				if(ActiveEditor != null) {
					ActiveEditor.EditValue = value;
				}
			}
		}
		int closingEditor = 0;
		protected internal bool IsClosingEditor { get { return closingEditor != 0; } }
		public virtual void CloseEditor() {
			CloseEditor(true);
		}
		protected internal virtual void CloseEditor(bool causeValidation) {
			if(Disposing || IsClosingEditor) return;
			this.closingEditor++;
			try {
				PostEditor(causeValidation);
				HideEditor();
			}
			finally {
				this.closingEditor--;
			}
		}
		public virtual void HideEditor() { HideEditorCore(true); }
		protected virtual void HideEditorCore(bool setFocus) {
			if(ActiveEditor == null || !allowCloseEditor) return;
			ContainerHelper.DeactivateEditor(setFocus);
			UpdateRowIndicatorImage(FocusedRow);
			Handler.SetControlState(TreeListState.Regular);
			navigationHelper.UpdateButtons();
			RaiseHiddenEditor();
		}
		public virtual void PostEditor() {
			PostEditor(true);
		}
		protected internal virtual void PostEditor(bool causeValidation) {
			if(ActiveEditor == null || !ActiveEditor.IsModified || Data.DataHelper.Posting) return;
			object edValue = null;
			if(causeValidation && !ContainerHelper.ValidateEditor(this)) return;
			try {
				Data.DataHelper.Posting = true;
				edValue = EditingValue;
				SetNodeValue(FocusedNode, FocusedColumn, edValue, !IsFocusedNodeDataModified);
				this.isFocusedNodeDataModified = true;
				if(FocusedRow != null)
					UpdateFocusedNodeCellAndRaiseChanged(FocusedRow[FocusedColumn], FocusedColumn, edValue, true);
			}
			catch(Exception e) {
				if(!(e is EditorValueException)) e = new EditorValueException(e, e.Message);
				ContainerHelper.OnInvalidValueException(this, e, edValue);
			}
			finally {
				Data.DataHelper.Posting = false;
			}
		}
		public virtual void ShowEditor() {
			if(!CanShowEditor) return;
			MakeColumnVisible(FocusedColumn);
			if(IsPixelScrolling && !TreeListAutoFilterNode.IsAutoFilterNode(FocusedNode)) 
				MakeFocusedRowVisible();
			if(IsScrollAnimationInProgress) {
				ShowEditorAfterAnimatedScroll = true;
				return;
			}
			RowInfo ri = FocusedRow;
			if(ri != null) {
				CellInfo cell = ri[FocusedColumn];
				if(cell != null) ActivateEditor(cell);
			}
		}
		private void ActivateEditor(CellInfo cell) {
			if(cell == null) return;
			Rectangle totalRows = viewInfo.ViewRects.TotalRows;
			if(viewInfo.ViewRects.IndicatorWidth > 0)
				totalRows.X += viewInfo.ViewRects.IndicatorWidth;
			Rectangle r = Rectangle.Intersect(ViewInfo.UpdateFixedRange(cell.CellValueRect, cell.ColumnInfo), totalRows);
			if(!(r.Width > 0 || r.Height == viewInfo.RowHeight)) return;
			ScrollInfo.OnAction(ScrollNotifyAction.Hide);
			RepositoryItem cellEdit = RequestCellEditor(cell);
			ViewInfo.UpdateRowCondition(cell.RowInfo);
			ViewInfo.UpdateRowCellPaintAppearance(cell);
			AppearanceObject appearance = new AppearanceObject();
			AppearanceHelper.Combine(appearance, new AppearanceObject[] { GetActiveEditorHighAppearance(), cell.Item.Appearance, ViewInfo.PaintAppearance.FocusedCell, ViewInfo.PaintAppearance.Row, cell.PaintAppearance });
			appearance.TextOptions.HAlignment = cell.PaintAppearance.HAlignment;
			UpdateEditorInfoArgs updateArgs = new UpdateEditorInfoArgs(cell.ColumnInfo.Column.ReadOnly || cell.Item.ReadOnly, r, appearance, cell.Value, ElementsLookAndFeel, cell.EditorViewInfo.ErrorIconText, cell.EditorViewInfo.ErrorIcon, IsRightToLeft);
			ContainerHelper.ActivateEditor(cellEdit, updateArgs);
			Handler.SetControlState(TreeListState.Editing);
			UpdateRowIndicatorImage(cell.RowInfo);
			navigationHelper.UpdateButtons();
			RaiseShownEditor();
		}
		protected virtual AppearanceObject GetActiveEditorHighAppearance() {
			if(OptionsSelection.EnableAppearanceFocusedCell)
				return ViewInfo.PaintAppearance.FocusedCell;
			if(OptionsSelection.EnableAppearanceFocusedRow)
				return ViewInfo.PaintAppearance.FocusedRow;
			return ViewInfo.PaintAppearance.Row;
		}
		protected virtual RepositoryItem RequestCellEditor(CellInfo cell) {
			RepositoryItem editor = cell.Item;
			GetCustomNodeCellEditEventHandler handler = (GetCustomNodeCellEditEventHandler)this.Events[customNodeCellEditForEditing];
			if(handler != null) {
				GetCustomNodeCellEditEventArgs e = new GetCustomNodeCellEditEventArgs(cell.Column, cell.RowInfo.Node, cell.Item);
				handler(this, e);
				editor = e.RepositoryItem;
			}
			return GetColumnEditForEditing(cell.Column, editor);
		}
		protected internal virtual void OnActiveEditor_ValueChanging(object sender, ChangingEventArgs e) {
			RaiseCellValueChanging(new CellValueChangedEventArgs(FocusedColumn, FocusedNode,
				e.NewValue));
		}
		protected internal virtual void OnActiveEditor_ValueChanged(object sender, EventArgs e) {
			if(IsFilterRow(FocusedRowIndex))
				OnFilterRowValueChanging(FocusedColumn, EditingValue);
		}
		void BeginUnboundDataRowEdit() { 
			if(!this.isFocusedNodeDataModified) {
				UnboundData data = Data as UnboundData;
				if(data != null)
					data.BeginDataRowEdit(this.FocusedRowIndex);
			}
		}
		protected internal virtual void OnActiveEditor_Modified(object sender, EventArgs e) {
			BeginUnboundDataRowEdit();
			UpdateRowIndicatorImage(FocusedRow);
		}
		protected internal virtual void OnActiveEditor_GotFocus(object sender, EventArgs e) {
			if(ContainerHelper.InternalFocusLock != 0) return;
			RefreshRowsInfo();
		}
		protected internal virtual void OnActiveEditor_LostFocus(object sender, EventArgs e) {
			if(ContainerHelper.InternalFocusLock != 0 || (ActiveEditor != null && ActiveEditor.EditorContainsFocus)) return;
			if(OptionsBehavior.CloseEditorOnLostFocus && !ActiveEditor.CanShowDialog) {
				if(ActiveEditor.IsModified) {
					try {
						ContainerHelper.BeginAllowHideException();
						PostEditor();
					}
					catch(HideException) {
						return;
					}
					finally {
						ContainerHelper.EndAllowHideException();
					}
				}
				HideEditorCore(false);
			}
			if(OptionsSelection.MultiSelect)
				RefreshRowsInfo();
			else {
				UpdateFocusedNode();
			}
		}
		protected override void OnValidating(CancelEventArgs e) {
			try {
				ContainerHelper.BeginAllowHideException();
				if(IsClosingEditor) return;
				if(!ContainerHelper.ValidateEditor(this)) {
					e.Cancel = true;
				}
				else {
					CloseEditor(false);
					e.Cancel = !CheckValidateFocusNode();
				}
			}
			catch(HideException) {
				e.Cancel = true;
			}
			finally {
				ContainerHelper.EndAllowHideException();
			}
			base.OnValidating(e);
		}
		private void UpdateRowIndicatorImage(RowInfo ri) {
			if(ri == null) return;
			int imageIndex = ViewInfo.GetRowIndicatorImageIndex(ri);
			ViewInfo.PaintAnimatedItems = false;
			if(ri.IndicatorInfo.ImageIndex != imageIndex) {
				ri.IndicatorInfo.ImageIndex = imageIndex;
			}
			Invalidate(ri.IndicatorInfo.Bounds);
		}
		internal RepositoryItem InternalGetCustomNodeCellEdit(TreeListColumn column, TreeListNode node) {
			if(column == null) return null;
			RepositoryItem item;
			if(TreeListAutoFilterNode.IsAutoFilterNode(node) && GetColumnFilterMode(column) == ColumnFilterMode.DisplayText)
				item = ContainerHelper.DefaultRepository.GetRepositoryItem(typeof(string));
			else
				item = GetColumnEdit(column);
			RaiseGetCustomNodeCellEdit(new GetCustomNodeCellEditEventArgs(column, node, item), ref item);
			return item;
		}
		protected internal virtual RepositoryItem GetColumnEdit(TreeListColumn column) {
			return column.RealColumnEdit;
		}
		protected internal virtual RepositoryItem GetColumnEditForEditing(TreeListColumn column, RepositoryItem editor) {
			return GetColumnDefaultRepositoryItemForEditing(column, editor);
		}
		internal AppearanceObject InternalGetCustomNodeCellStyle(TreeListColumn column, TreeListNode node, AppearanceObject appearance) {
			return RaiseGetCustomNodeCellStyle(new GetCustomNodeCellStyleEventArgs(column, node, appearance));
		}
		public void StartIncrementalSearch(string start) {
			if(!OptionsBehavior.AllowIncrementalSearch || string.IsNullOrEmpty(start)) return;
			Handler.SetControlState(TreeListState.IncrementalSearch);
			TreeListHandler.IncrementalSearchState state = Handler.GetControlState() as TreeListHandler.IncrementalSearchState;
			if(state != null) state.DoIncrementalSearch(start);
		}
		public void StopIncrementalSearch() {
			if(State == TreeListState.IncrementalSearch) {
				IncrementalText = "";
				Handler.SetControlState(TreeListState.Regular);
				ViewInfo.PaintAnimatedItems = false;
				InvalidateCell(FocusedNode, FocusedColumn);
			}
		}
		public bool DoIncrementalSearch(bool down) {
			TreeListHandler.IncrementalSearchState state = Handler.GetControlState() as TreeListHandler.IncrementalSearchState;
			if(state != null) {
				KeyEventArgs e = new KeyEventArgs(down ? (Keys.Down | Keys.Control) : (Keys.Up | Keys.Control));
				TreeListNode prevFocusedNode = FocusedNode;
				state.DoIncrementalSearchNavigation(e);
				return prevFocusedNode != FocusedNode;
			}
			return false;
		}
		#endregion
		#region Data
		public virtual object GetDataRecordByNode(TreeListNode node) {
			if(node == null || node.TreeList != this) return null;
			try { return Data.GetDataRow(node.Id); }
			catch { }
			return null;
		}
		bool refreshFired = false;
		public virtual void RefreshDataSource() {
			if(IsIniting) return;
			if(CurrencyManager != null)
				CurrencyManager.Refresh();
			OnReset(); 
			refreshFired = true;
		}
		public virtual void ForceInitialize() {
			OnLoaded();
		}
		public virtual void CancelCurrentEdit() {
			isFocusedNodeDataModified = false;
			EndEdit(true);
			ClearColumnErrors();
		}
		public virtual void EndCurrentEdit() {
			isFocusedNodeDataModified = false;  
			EndEdit(false);
		}
		protected virtual void EndEdit(bool cancel) {
			IEditableObject obj = GetEditableObject();
			if(obj != null) {
				if(cancel)
					obj.CancelEdit();
				else
					obj.EndEdit();
			}
			Data.IsCurrentDataRowEditing = false;
			LayoutChanged();
		}
		protected virtual IEditableObject GetEditableObject() {
			if(IsUnboundMode) return Data.GetEditableObject(FocusedNode == null ? -1 : FocusedNode.Id);
			if(CurrencyManager == null || CurrencyManager.Position == -1) return null;
			return CurrencyManager.Current as IEditableObject;
		}
		protected internal virtual void OnSetValue(TreeListNode node, object columnID, object val) {
			bool setInFocusedNode = (node == FocusedNode);
			SetNodeValue(node, columnID, val, setInFocusedNode && !IsFocusedNodeDataModified);
			if(!setInFocusedNode) {
				LayoutChanged();
			}
			else {
				TreeListColumn column = Data.DataHelper.GetTreeListColumnByID(columnID);
				if(column == null || column.VisibleIndex == -1 || FocusedRow == null) return;
				UpdateFocusedNodeCellAndRaiseChanged(FocusedRow[column], column, val, false);
			}
			if(!Data.SupportNotifications)
				UpdateNodeVisibility(node);
		}
		protected virtual void UpdateNodeVisibility(TreeListNode node) {
			if(OptionsFilter.FilterMode == FilterMode.Extended) {
				TreeListNode rootNode = node.RootNode;
				if(rootNode != null) {
					BeginUpdate();
					try {
						DoFilterNode(rootNode);
						FilterNodes(rootNode.Nodes);
					}
					finally {
						EndUpdate();
					}
					return;
				}
			}
			DoFilterNode(node);
		}
		protected virtual void SetNodeValue(TreeListNode node, object columnId, object val, bool initEdit) {
			if(TreeListAutoFilterNode.IsAutoFilterNode(node)) {
				SetFilterRowValue(columnId, val);
				return;
			}
			Data.SetValue(node.Id, columnId, val, initEdit);
		}
		protected internal virtual object GetNodeValue(TreeListNode node, object columnId) {
			if(TreeListAutoFilterNode.IsAutoFilterNode(node)) {
				return GetFilterRowValue(columnId);
			}
			return Data.GetValue(node.Id, columnId);
		}
		bool lockRaiseCellValueChangedEvent = false;
		void UpdateFocusedNodeCellAndRaiseChanged(CellInfo cell, TreeListColumn column, object value, bool raiseEvent) {
			if(cell == null) return;
			ViewInfo.UpdateCell(cell, column, FocusedNode);
			if(IsFocusedNodeDataModified)
				RefreshEditorError();
			ViewInfo.PaintAnimatedItems = false;
			Invalidate(cell.Bounds);
			if(raiseEvent && !lockRaiseCellValueChangedEvent) {
				lockRaiseCellValueChangedEvent = true;
				try {
					RaiseCellValueChanged(new CellValueChangedEventArgs(column, FocusedNode, value));
				}
				finally {
					lockRaiseCellValueChangedEvent = false;
				}
			}
		}
		protected virtual void RefreshEditorError() {
			if(ActiveEditor == null || !ActiveEditor.IsModified) return;
			if(TreeListAutoFilterNode.IsAutoFilterNode(FocusedNode)) return;
			BaseEdit be = ActiveEditor;
			be.Properties.BeginUpdate();
			try {
				be.Properties.LockEvents();
				string error;
				DevExpress.XtraEditors.DXErrorProvider.ErrorType errorType;
				GetColumnError(FocusedColumn, FocusedNode, out error, out errorType);
				be.ErrorText = error;
				if(be.ErrorText != null && be.ErrorText.Length > 0)
					be.ErrorIcon = DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider.GetErrorIconInternal(errorType);
			}
			finally {
				be.Properties.UnLockEvents();
				be.Properties.EndUpdate();
			}
		}
		protected internal DataColumnInfo GetDataColumnInfo(string columnName) {
			if(Data == null) return null;
			return Data.GetDataColumnInfo(columnName);
		}
		internal bool IsValidColumnName(string columnName) {
			return GetDataColumnInfo(columnName) != null;
		}
		void UpdateDataSourceOnChangeDataSource() {
			UpdateDataSource(!IsLoading && (IsHandleCreated || IsLookUpMode)); 
			if(DesignMode && Data != null) Data.PopulateColumns();
			if(IsLookUpMode && DesignMode) return;
			if(!IsLoading && Columns.Count == 0 && OptionsBehavior.AutoPopulateColumns)
				PopulateColumns();
		}
		protected virtual void UpdateDataSource(bool updateContent) {
			if(lockUpdateDataSource != 0) return;
			lockUpdateDataSource++;
			try {
				CurrencyManager = GetCurrencyManager();
				CastDataSource();
				if(updateContent)
					DoDataSourceChanged();
			}
			finally {
				lockUpdateDataSource--;
			}
		}
		protected virtual CurrencyManager GetCurrencyManager() {
			if(DataSource == null || BindingContext == null || DataSource is IVirtualTreeListData) return null;
			return BindingContext[DataSource, DataMember] as CurrencyManager;
		}
		private void CastDataSource() {
			BeginUpdate();
			try {
				Data = CreateDataCore();
			}
			finally {
				CancelUpdate();
			}
		}
		private TreeListData CreateDataCore() {
			Columns.CollectionChanged -= new CollectionChangeEventHandler(Columns_Changed);
			try {
				return CreateData();
			}
			finally {
				Columns.CollectionChanged += new CollectionChangeEventHandler(Columns_Changed);
			}
		}
		protected internal bool IsVirtualMode { get { return Data is TreeListVirtualData; } }
		protected internal virtual bool CanExpandNodesOnIncrementalSearch {
			get { return OptionsBehavior.ExpandNodesOnIncrementalSearch && !IsVirtualMode; }
		}
		protected virtual TreeListData CreateData() {
			TreeListDataHelper dataHelper = new TreeListDataHelper(this);
			if(CurrencyManager != null) {
				if(CurrencyManager.List is DataView) return new DataViewData(dataHelper, CurrencyManager.List as DataView);
				return new ListData(dataHelper, CurrencyManager.List);
			}
			if(this.DataSource != null) {
				return new TreeListVirtualData(dataHelper, VirtualDataHelper);
			}
			return new UnboundData(dataHelper);
		}
		protected virtual void LoadDataColumns() {
			VisibleColumns.Clear();
			Columns.Clear();
			SortedColumns.Clear();
			int i = 0;
			for(int n = 0; n < Data.Columns.Count; n++) {
				DataColumnInfo info = Data.Columns[n];
				if(!Data.CanPopulate(info)) continue;
				int columnIndex = i;
				TreeListColumn col = CreateColumn(info, ref columnIndex);
				if(col != null) {
					col.visibleIndex = columnIndex;
					col.SetVisibleCore(columnIndex >= 0);
					col.absoluteIndex = i++;
					if(n == 0) {
						col.SetVisibleWidth(col.VisibleWidth + viewInfo.FirstColumnIndent);
						col.width = col.VisibleWidth;
					}
					OnColumnCreated(col);
				}
			}
			NormalizeVisibleColumnIndices();
			RefreshColumnHandles();
		}
		protected virtual void OnColumnCreated(TreeListColumn column) {
			if(Bands.Count > 0)
				Bands[0].Columns.Add(column);
		}
		protected internal virtual void OnDataListChanged(object sender, ListChangedEventArgs e) {
			viewInfo.SummaryFooterInfo.NeedsRecalcAll = true;
			if(lockReloadNodes != 0) return;
			if(lockListChanged != 0) {
				if(e.ListChangedType == ListChangedType.ItemDeleted && deletingNodeID == -1 && deletingNodeID != e.NewIndex && NodesIdManager == null)
					OnItemDeleted(e.NewIndex);
				return;
			}
			ClearDateFilterCache();
			switch(e.ListChangedType) {
				case ListChangedType.Reset:
					OnReset();
					break;
				case ListChangedType.ItemAdded:
					OnItemAdded(e.NewIndex);
					break;
				case ListChangedType.ItemDeleted:
					OnItemDeleted(e.NewIndex);
					break;
				case ListChangedType.ItemMoved:
					OnItemMoved(e.OldIndex, e.NewIndex);
					break;
				case ListChangedType.ItemChanged:
					OnItemChanged(e.NewIndex, e.PropertyDescriptor);
					break;
				case ListChangedType.PropertyDescriptorAdded:
				case ListChangedType.PropertyDescriptorDeleted:
				case ListChangedType.PropertyDescriptorChanged:
					OnPropertyDescriptorChanged();
					break;
			}
			if(IsLookUpMode)
				LookUpOwner.ClearDisplayTextCache();
		}
		protected virtual void DoDataSourceChanged() {
			BeginUpdate();
			try {
				ClearInternalSettings();
				Nodes.Clear();
				ClearColumnErrors();
				RefreshColumnHandles();
				LoadNodes();
				DoSort(Nodes, true); 
			}
			finally {
				EndUpdate();
			}
			if(RowCount > 0 && !IsLoading && !isLoadingInternal && !IsLookUpMode) {
				if(!ViewInfo.IsValid) ViewInfo.CalcViewInfo();
				FocusedNode = FindNodeByID(Position);
				FocusedCellIndex = 0;
				CheckIncreaseVisibleRows();
			}
			OnFilterDataSourceChanged();
		}
		protected virtual void OnReset() {
			BeginUpdate();
			try {
				ClearInternalSettings(false);
				ReloadNodes();
				RefreshColumnsDataInfo();
			}
			finally {
				EndUpdate();
			}
		}
		private void ReloadNodes() {
			if(lockReloadNodes != 0) return;
			Hashtable nodesData = SaveNodesData();
			int focusCellIndex = FocusedCellIndex;
			int focusedNodeId = (FocusedNode == null ? -1 : FocusedNode.Id);
			NullTopVisibleNode();
			BeginUpdate();
			focusedNodeChangedCounter++;
			try {
				Nodes.Clear();
				lockExpandCollapse++;
				if(Data != null) {
					LoadNodes();
					RestoreNodesData(nodesData);
					DoSort(Nodes, true);
				}
			}
			finally {
				lockExpandCollapse--;
				focusedNodeChangedCounter--;
				EndUpdate();
			}
			this.focusedCellIndex = focusCellIndex;
			SetFocusedRowIndexCore(-1);
			if(focusedNodeId < AllNodesCount && focusedNodeId != -1) {
				TreeListNode newFocus = FindNodeByID(focusedNodeId);
				SetFocusedNode(newFocus);
				SetFocusedNodeCore(newFocus);
				if(currentSetFocus != null) SetCurrentSetFocusCore(FindNodeByID(currentSetFocus.Id));
				Selection.Set(FocusedNode);
				Position = focusedNodeId;
			}
			else {
				if(CurrencyManager != null && !refreshFired) {
					CurrencyManager.Refresh(); 
				}
				MoveFirst(); 
				if(FocusedCellIndex == -1)
					FocusedCellIndex = 0;
			}
			refreshFired = false;
		}
		protected virtual void OnItemAdded(int index) {
			object parentID = Data.GetValue(index, ParentFieldName);
			TreeListNode addedNode = null, parentNode = null;
			ResetNodesCounters();
			BeginUpdate();
			try {
				if(FindNodeByID(index) == null) {
					if(!(parentID == null || parentID.Equals(RootValue)))
						parentNode = FindNodeByKeyID(parentID);
					addedNode = InternalCreateNode(index, parentNode, null);
					SynchronizeNodesID(index, +1);
					if(OptionsNavigation.AutoFocusNewNode)
						autoFocusedNode = addedNode;
					if(parentNode == null && Nodes.Count == 1)
						ViewInfo.CalcMaxIndents();
				}
				else {
					ReloadNodes();
					return;
				}
			}
			finally {
				EndUpdate();
			}
			if(parentNode != null)
				DoSort(parentNode.Nodes, true);
			else
				DoSort(Nodes, true);
			if(FocusedNode == null)
				MoveFirst();
		}
		protected internal virtual void OnItemDeleted(int index) {
			DeleteNode(index, null, !OptionsBehavior.AutoChangeParent);
		}
		private void DeleteNodesGroup(int id, TreeListNode sourceNode, bool deleteChildren) {
			ArrayList nodesToDelete = new ArrayList();
			if(sourceNode != null && !sourceNode.HasClones)
				nodesToDelete.Add(sourceNode);
			else {
				TreeListOperationAddNodeById op = new TreeListOperationAddNodeById(id);
				NodesIterator.DoOperation(op);
				nodesToDelete = op.Nodes;
			}
			if(sourceNode != null && nodesToDelete.Count > 1)
				DeleteNodeCore(sourceNode, deleteChildren);
			else {
				if(IsGroupDeleteOperation) {
					NodesIdManager.Add(id);
				}
				else {
					if(sourceNode != null)
						RemoveNodeDataRecord(sourceNode);
					SynchronizeNodesID(id, -1);
				}
				while(nodesToDelete.Count > 0) {
					DeleteNodeCore((TreeListNode)nodesToDelete[0], deleteChildren);
					nodesToDelete.RemoveAt(0);
				}
			}
		}
		private void DeleteChildren(TreeListNode parentNode) {
			if(parentNode.TreeList == null) return;
			ResetRowCount();
			lockDelete++;
			try {
				BeginLockListChanged();
				BeginUpdate();
				try {
					TreeListNodes nodes = parentNode.Nodes;
					RemoveNodeChildren(parentNode, true);
					for(int i = nodes.Count - 1; i > -1; i--) {
						TreeListNode node = nodes[i];
						if(node != null)
							node.owner = null;
					}
					nodes._clear();
					parentNode.HasChildren = false;
					if(!IsGroupDeleteOperation)
						viewInfo.CalcMaxIndents();
				}
				finally {
					CancelLockListChanged();
					viewInfo.SummaryFooterInfo.NeedsRecalcAll = true;
					EndUpdate();
				}
				if(TopVisibleNode != null && TopVisibleNode.TreeList == null) NullTopVisibleNode();
				if(FocusedNode != null && FocusedNode.TreeList == null)
					FocusedNode = null;
				else
					CheckFocusedNode();
			}
			finally {
				lockDelete--;
			}
		}
		protected virtual void OnItemMoved(int oldIndex, int newIndex) {
			if(oldIndex == newIndex) return;
			if(oldIndex == -1) {
				OnItemAdded(newIndex);
				return;
			}
			if(newIndex == -1) {
				OnItemDeleted(oldIndex);
				return;
			}
		}
		protected virtual void OnItemChanged(int index, PropertyDescriptor propertyDescriptor) {
			if(IsUnboundLoad) return;
			TreeListNode node = FindNodeByID(index);
			RemoveNodeHeight(node);
			CheckParentIDChanging(node);
			if(!IsLockUpdate)
				viewInfo.CalcSummaryFooterInfo();
			if(OptionsView.ShowSummaryFooter) {
				ViewInfo.PaintAnimatedItems = false;
				Invalidate(viewInfo.ViewRects.Footer);
			}
			if(node != null) {
				BeginUpdate();
				try {
					bool isModifiedNode = (node == FocusedNode && Data.IsCurrentDataRowEditing);
					if(!isModifiedNode)
						UpdateNodeVisibility(node);
					if(ShouldResortNodesOnItemChanged(index, propertyDescriptor))
						DoSort(node.owner, true);
				}
				finally {
					CancelUpdate();
				}
			}
			RefreshRowsInfo();
		}
		protected virtual bool ShouldResortNodesOnItemChanged(int index, PropertyDescriptor propertyDescriptor) {
			if(propertyDescriptor == null) return true;
			if(SortedColumns.Count == 0) return false;
			if(SortedColumns[propertyDescriptor.Name] == null) return false;
			return true;
		}
		private void CheckParentIDChanging(TreeListNode node) {
			if(!(IsValidColumnName(KeyFieldName) && IsValidColumnName(ParentFieldName))) return;
			if(node == null) return;
			object obj1 = node[ParentFieldName];
			object obj2 = (node.ParentNode == null ? RootValue : node.ParentNode[KeyFieldName]);
			bool needReloadNodes = !_objectsAreEqual(obj1, obj2);
			if(!needReloadNodes && node.HasChildren && node.Nodes.Count > 0) {
				obj1 = node.Nodes[0][ParentFieldName];
				obj2 = node[KeyFieldName];
				needReloadNodes = !_objectsAreEqual(obj1, obj2);
			}
			if(needReloadNodes) ReloadNodes();
		}
		private bool _objectsAreEqual(object obj1, object obj2) {
			if(obj1 == null) return (obj2 == null);
			return obj1.Equals(obj2);
		}
		protected virtual void OnPropertyDescriptorChanged() {
			RefreshColumnsDataInfo();
		}
		void RefreshColumnsDataInfo() {
			Data.PopulateColumns();
			RefreshColumnHandles();
			RefreshCustomizationForm();
		}
		protected virtual void CurrencyManager_PositionChanged(object sender, EventArgs e) {
			if(lockReloadNodes != 0) return;
			int oldPosition = (FocusedNode == null ? -1 : FocusedNode.Id);
			if(oldPosition != Position) {
				if(FocusedNode != null && FocusedNode.Id == deletingNodeID) return;
				if(Position < 0) return;
				TreeListNode node = FindNodeByID(Position);
				if(node != null && node.Visible) FocusedNode = node;
			}
			Handler.OnPositionChanged();
		}
		protected virtual void CurrencyManager_ItemChanged(object sender, ItemChangedEventArgs e) {
			if(Data.CurrencyManagerWasReset(sender as CurrencyManager, e))
				UpdateDataSource(true);
		}
		protected internal virtual void BeginLockListChanged() {
			lockListChanged++;
		}
		protected internal virtual void CancelLockListChanged() {
			lockListChanged--;
		}
		protected internal virtual void RejectCurrentChanges() {
			if(!IsFocusedNodeDataModified) return;
			CancelCurrentEdit();
			this.isFocusedNodeDataModified = false;
			RefreshRowsInfo();
		}
		internal void RefreshColumnHandles() {
			foreach(TreeListColumn col in Columns) {
				col.columnHandle = Data.Columns.IndexOf(col.FieldName);
			}
		}
		protected TreeListColumn CreateColumn(DataColumnInfo columnInfo, ref int columnIndex) {
			if(!OptionsBehavior.PopulateServiceColumns) {
				if(InServiceColumns(columnInfo.ColumnName)) return null;
			}
			var options = DevExpress.Data.Utils.AnnotationAttributes.GetColumnOptions(columnInfo.Descriptor, columnIndex, columnInfo.ReadOnly);
			if(!options.AutoGenerateField)
				return null;
			TreeListColumn column = Columns.Add(columnInfo);
			if(column == null) return null;
			column.SetColumnAnnotationAttributes(options.Attributes);
			columnIndex = options.ColumnIndex;
			if(options.ReadOnly)
				column.OptionsColumn.ReadOnly = true;
			if(!options.AllowEdit)
				column.OptionsColumn.AllowEdit = false;
			if(!options.AllowFilter)
				column.OptionsFilter.AllowFilter = false;
			return column;
		}
		protected internal virtual void OnColumnUnboundChanged(TreeListColumn column) {
			Data.OnColumnTypeChanged(column);
			RefreshColumnHandles();
			if(!IsLoading)
				InternalColumnChanged(column);
		}
		public virtual object InternalGetService(Type service) {
			if(service != null) {
				if(service.Equals(typeof(DataColumnInfoCollection)))
					return Data.Columns;
				if(service.Equals(typeof(DevExpress.XtraTreeList.Helpers.IDesignNotified)))
					return Handler;
				if(service.Equals(typeof(TreeListPrintInfo)))
					return printer.PrintInfo;
			}
			return GetService(service);
		}
		protected internal virtual bool EnableEnhancedSorting { get { return true; } }
		#endregion
		#region IPrintable
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsPrinting { get { return printer.IsPrinting; } }
		bool IPrintable.CreatesIntersectedBricks {
			get { return true; }
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			if(printer != null) {
				printer.Release();
			}
		}
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			printer.Initialize(ps, link, viewInfo);
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			printer.CreateArea(areaName, graph);
		}
		void IPrintable.AcceptChanges() {
			printer.AcceptChanges();
		}
		void IPrintable.RejectChanges() {
			printer.RejectChanges();
		}
		void IPrintable.ShowHelp() {
			printer.ShowHelp();
		}
		bool IPrintable.SupportsHelp() {
			return printer.SupportsHelp();
		}
		bool IPrintable.HasPropertyEditor() {
			return printer.HasPropertyEditor();
		}
		void IPrintableEx.OnStartActivity() {
			if(!IsHandleCreated) return;
			ShowProgressForm();
		}
		void IPrintableEx.OnEndActivity() {
			ProgressWindow = null;
			if(AllowPrintProgress) Focus();
		}
		protected internal void ShowProgressForm() {
			if(ProgressWindow != null) {
				ProgressWindow.SetCaption(printer.PrintingActivity);
				return;
			}
			if(!AllowPrintProgress) return;
			Form form = FindForm();
			if(form == null) return;
			ProgressWindow = new ProgressWindow();
			ProgressWindow.LookAndFeel.Assign(ElementsLookAndFeel);
			if(!OptionsPrint.AllowCancelPrintExport) ProgressWindow.DisableCancel();
			ProgressWindow.Cancel += new EventHandler(OnProgressWindowCancel);
			ProgressWindow.SetCaption(printer.PrintingActivity);
			ProgressWindow.ShowCenter(form);
		}
		protected internal void OnPrintProgress(int progress) {
			if(progress < 0) return;
			if(ProgressWindow != null) ProgressWindow.SetProgress(progress);
			RaisePrintExportProgress(new ProgressChangedEventArgs(progress, null));
		}
		protected virtual void RaisePrintExportProgress(ProgressChangedEventArgs e) {
			ProgressChangedEventHandler handler = (ProgressChangedEventHandler)this.Events[printExportProgress];
			if(handler != null) handler(this, e);
		}
		void OnProgressWindowCancel(object sender, EventArgs e) {
			printer.CancelPrint();
		}
		protected internal ProgressWindow ProgressWindow {
			get { return progressWindow; }
			set {
				if(progressWindow == value) return;
				if(progressWindow != null) 
					progressWindow.Dispose();
				this.progressWindow = value;
			}
		}
		protected internal virtual bool AllowPrintProgress { get { return OptionsPrint.ShowPrintExportProgress && allowPrintProgressCore > 0; } }
		ProgressWindow progressWindow = null;
		int allowPrintProgressCore = 0;
		UserControl IPrintable.PropertyEditorControl { get { return printer.PropertyEditorControl; } }
		internal CellInfo PrintingCell { set { painter.drawnCell = value; } }
		protected ComponentPrinterBase PrinterCore { get { return printer.PrinterCore; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsPrintingAvailable { get { return ComponentPrinterBase.IsPrintingAvailable(false); } }
		protected void ExecutePrinterAction(Action0 action) {
			allowPrintProgressCore++;
			try {
				ClearDocument();
				action();
			}
			finally {
				allowPrintProgressCore--;
			}
		}
		public void ClearDocument() {
			PrinterCore.ClearDocument();
		}
		public void PrintDialog() {
			ExecutePrinterAction(delegate() { PrinterCore.PrintDialog(); });
		}
		public void ShowPrintPreview() {
			ExecutePrinterAction(delegate() { PrinterCore.ShowPreview(ElementsLookAndFeel); });
		}
		public void ShowRibbonPrintPreview() {
			ExecutePrinterAction(delegate() { PrinterCore.ShowRibbonPreview(ElementsLookAndFeel); });
		}
		public void Print() {
			ExecutePrinterAction(delegate() { PrinterCore.Print(); });
		}
		#endregion
		#region IXtraSerializable
		protected internal virtual bool IsDeserializing { get { return lockDeserializing != 0; } }
		string IXtraSerializableLayout.LayoutVersion { get { return OptionsLayout.LayoutVersion; } }
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			RaiseBeforeLoadLayout(e);
			if(!e.Allow) return;
			this.lockDeserializing++;
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			this.lockDeserializing--;
			try {
				if(restoredVersion != OptionsLayout.LayoutVersion) RaiseLayoutUpgrade(new LayoutUpgradeEventArgs(restoredVersion));
			}
			finally {
				if(!IsLoading && !IsIniting)
					OnLoadedCore();
			}
		}
		void IXtraSerializable.OnStartSerializing() {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			OnResetSerializationProperties(options);
		}
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return OnAllowSerializationProperty(options, propertyName, id);
		}
		protected virtual bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			OptionsLayoutTreeList opt = options as OptionsLayoutTreeList;
			if(opt == null) return true;
			if(id == LayoutIdAppearance) return opt.StoreAppearance;
			return true;
		}
		protected virtual void OnResetSerializationProperties(OptionsLayoutBase options) {
			OptionsLayoutTreeList opt = options as OptionsLayoutTreeList;
			if(opt != null && opt.StoreAppearance) {
				Appearance.Reset();
				AppearancePrint.Reset();
			}
		}
		public virtual void SaveLayoutToXml(string xmlFile) {
			SaveLayoutCore(new XmlXtraSerializer(), xmlFile, OptionsLayout);
		}
		public virtual void SaveLayoutToXml(string xmlFile, OptionsLayoutBase options) {
			SaveLayoutCore(new XmlXtraSerializer(), xmlFile, options);
		}
		public virtual void RestoreLayoutFromXml(string xmlFile) {
			RestoreLayoutCore(new XmlXtraSerializer(), xmlFile, OptionsLayout);
		}
		public virtual void RestoreLayoutFromXml(string xmlFile, OptionsLayoutBase options) {
			RestoreLayoutCore(new XmlXtraSerializer(), xmlFile, options);
		}
		public virtual void SaveLayoutToRegistry(string path) {
			SaveLayoutCore(new RegistryXtraSerializer(), path, OptionsLayout);
		}
		public virtual void SaveLayoutToRegistry(string path, OptionsLayoutBase options) {
			SaveLayoutCore(new RegistryXtraSerializer(), path, options);
		}
		public virtual void RestoreLayoutFromRegistry(string path) {
			RestoreLayoutCore(new RegistryXtraSerializer(), path, OptionsLayout);
		}
		public virtual void RestoreLayoutFromRegistry(string path, OptionsLayoutBase options) {
			RestoreLayoutCore(new RegistryXtraSerializer(), path, options);
		}
		public virtual void SaveLayoutToStream(Stream stream) {
			SaveLayoutCore(new XmlXtraSerializer(), stream, OptionsLayout);
		}
		public virtual void SaveLayoutToStream(Stream stream, OptionsLayoutBase options) {
			SaveLayoutCore(new XmlXtraSerializer(), stream, options);
		}
		public virtual void RestoreLayoutFromStream(Stream stream) {
			RestoreLayoutCore(new XmlXtraSerializer(), stream, OptionsLayout);
		}
		public virtual void RestoreLayoutFromStream(Stream stream, OptionsLayoutBase options) {
			RestoreLayoutCore(new XmlXtraSerializer(), stream, options);
		}
		protected virtual void SaveLayoutCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			Stream stream = path as Stream;
			if(stream != null)
				serializer.SerializeObject(this, stream, this.GetType().Name, options);
			else
				serializer.SerializeObject(this, path.ToString(), this.GetType().Name, options);
		}
		protected virtual void RestoreLayoutCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			Stream stream = path as Stream;
			if(stream != null)
				serializer.DeserializeObject(this, stream, this.GetType().Name, options);
			else
				serializer.DeserializeObject(this, path.ToString(), this.GetType().Name, options);
			LayoutChanged();
		}
		#endregion
		#region export
		protected void Export(ExportTarget exportTarget, Stream stream) {
			ExecutePrinterAction(delegate() { PrinterCore.Export(exportTarget, stream); });
		}
		protected void Export(ExportTarget exportTarget, string fileName) {
			ExecutePrinterAction(delegate() { PrinterCore.Export(exportTarget, fileName); });
		}
		protected void Export(ExportTarget exportTarget, string fileName, ExportOptionsBase options) {
			ExecutePrinterAction(delegate() { PrinterCore.Export(exportTarget, fileName, options); });
		}
		protected void Export(ExportTarget exportTarget, Stream stream, ExportOptionsBase options) {
			ExecutePrinterAction(delegate() { PrinterCore.Export(exportTarget, stream, options); });
		}
		public virtual void ExportToRtf(Stream stream) {
			Export(ExportTarget.Rtf, stream);
		}
		public virtual void ExportToRtf(string fileName) {
			Export(ExportTarget.Rtf, fileName);
		}
		public virtual void ExportToPdf(Stream stream) {
			Export(ExportTarget.Pdf, stream);
		}
		public virtual void ExportToPdf(string fileName) {
			Export(ExportTarget.Pdf, fileName);
		}
		public virtual void ExportToPdf(Stream stream, PdfExportOptions options) {
			Export(ExportTarget.Pdf, stream, options);
		}
		public virtual void ExportToPdf(string fileName, PdfExportOptions options) {
			Export(ExportTarget.Pdf, fileName, options);
		}
		public virtual void ExportToHtml(Stream stream) {
			Export(ExportTarget.Html, stream);
		}
		public virtual void ExportToHtml(string fileName) {
			Export(ExportTarget.Html, fileName);
		}
		public virtual void ExportToHtml(Stream stream, HtmlExportOptions options) {
			Export(ExportTarget.Html, stream, options);
		}
		public virtual void ExportToHtml(String fileName, HtmlExportOptions options) {
			Export(ExportTarget.Html, fileName, options);
		}
		public virtual void ExportToText(Stream stream) {
			Export(ExportTarget.Text, stream);
		}
		public virtual void ExportToText(string fileName) {
			Export(ExportTarget.Text, fileName);
		}
		public virtual void ExportToText(string fileName, TextExportOptions options) {
			Export(ExportTarget.Text, fileName, options);
		}
		public virtual void ExportToText(Stream stream, TextExportOptions options) {
			Export(ExportTarget.Text, stream, options);
		}
		public virtual void ExportToCsv(Stream stream) {
			Export(ExportTarget.Csv, stream);
		}
		public virtual void ExportToCsv(string filePath) {
			Export(ExportTarget.Csv, filePath);
		}
		public virtual void ExportToCsv(string filePath, CsvExportOptions options) {
			Export(ExportTarget.Csv, filePath, options);
		}
		public virtual void ExportToCsv(Stream stream, CsvExportOptions options) {
			Export(ExportTarget.Csv, stream, options);
		}
		public virtual void ExportToXls(Stream stream) {
			Export(ExportTarget.Xls, stream);
		}
		public virtual void ExportToXls(string fileName) {
			Export(ExportTarget.Xls, fileName);
		}
		public virtual void ExportToXls(Stream stream, XlsExportOptions options) {
			Export(ExportTarget.Xls, stream, options);
		}
		public virtual void ExportToXls(string fileName, XlsExportOptions options) {
			Export(ExportTarget.Xls, fileName, options);
		}
		public void ExportToXlsx(string fileName) {
			Export(ExportTarget.Xlsx, fileName);
		}
		public void ExportToXlsx(Stream stream) {
			Export(ExportTarget.Xlsx, stream);
		}
		public void ExportToXlsx(Stream stream, XlsxExportOptions options) {
			Export(ExportTarget.Xlsx, stream, options);
		}
		public void ExportToXlsx(string fileName, XlsxExportOptions options) {
			Export(ExportTarget.Xlsx, fileName, options);
		}
		[Obsolete("Use the ExportToMht(Stream stream, MhtExportOptions options) method instead")]
		public virtual void ExportToMht(Stream stream, string htmlCharSet, string title, bool compressed) {
			MhtExportOptions options = new MhtExportOptions(htmlCharSet, title, compressed);
			Export(ExportTarget.Mht, stream, options);
		}
		[Obsolete("Use the ExportToMht(string fileName, MhtExportOptions options) method instead")]
		public virtual void ExportToMht(string fileName, string htmlCharSet) {
			MhtExportOptions options = new MhtExportOptions(htmlCharSet);
			Export(ExportTarget.Mht, fileName, options);
		}
		[Obsolete("Use the ExportToMht(string fileName, MhtExportOptions options) method instead")]
		public virtual void ExportToMht(string fileName, string htmlCharSet, string title, bool compressed) {
			MhtExportOptions options = new MhtExportOptions(htmlCharSet, title, compressed);
			Export(ExportTarget.Mht, fileName, options);
		}
		public virtual void ExportToMht(string fileName, MhtExportOptions options) {
			Export(ExportTarget.Mht, fileName, options);
		}
		public virtual void ExportToMht(Stream stream, MhtExportOptions options) {
			Export(ExportTarget.Mht, stream, options);
		}
		public virtual void ExportToXml(Stream stream) {
			ExportToXmlCore(new XmlSerializer(typeof(TreeList.TreeListXmlSerializationHelper)), stream);
		}
		public virtual void ExportToXml(string xmlFile) {
			ExportToXmlCore(new XmlSerializer(typeof(TreeList.TreeListXmlSerializationHelper)), xmlFile);
		}
		public virtual void ImportFromXml(Stream stream) {
			ImportFromXmlCore(new XmlSerializer(typeof(TreeList.TreeListXmlSerializationHelper)), stream);
		}
		public virtual void ImportFromXml(string xmlFile) {
			ImportFromXmlCore(new XmlSerializer(typeof(TreeList.TreeListXmlSerializationHelper)), xmlFile);
		}
		protected virtual void ExportToXmlCore(XmlSerializer serializer, object path) {
			TreeListXmlSerializationHelper sh = new TreeListXmlSerializationHelper();
			sh.TreeList = this;
			sh.OnBeforeSerializing();
			Stream stream = null;
			try {
				if(path is string) stream = new FileStream(path as String, FileMode.Create);
				else stream = path as Stream;
				if(stream != null) serializer.Serialize(stream, sh);
			}
			finally {
				if(stream is FileStream) ((FileStream)stream).Close();
			}
		}
		protected virtual void ImportFromXmlCore(XmlSerializer serializer, object path) {
			if(!IsUnboundMode || IsLockUpdate || TreeListDisposing || IsVirtualMode) return;
			Stream stream = null;
			BeginUpdate();
			BeginUnboundLoad();
			try {
				if(path is string) stream = new FileStream(path as String, FileMode.Open, FileAccess.Read);
				else stream = path as Stream;
				System.Xml.XmlReader reader = new System.Xml.XmlTextReader(stream);
				if(reader == null || !serializer.CanDeserialize(reader)) return;
				TreeListXmlSerializationHelper sh = null;
				if((sh = (TreeListXmlSerializationHelper)serializer.Deserialize(reader)) == null) return;
				sh.TreeList = this;
				sh.OnAfterDeserializing();
			}
			finally {
				if(stream is FileStream) ((FileStream)stream).Close();
				EndUnboundLoad();
				EndUpdate();
			}
		}
		[XmlRootAttribute("TreeList")]
		public class TreeListXmlSerializationHelper {
			[XmlArrayAttribute("Columns"), XmlArrayItem("Column")]
			public ColumnDataInfo[] columns;
			[XmlArrayAttribute("Nodes"), XmlArrayItem("Node"),]
			public NodeDataInfo[] nodes;
			internal TreeList TreeList {
				get { return treeList; }
				set { treeList = value; }
			}
			TreeList treeList;
			public TreeListXmlSerializationHelper() { }
			public virtual void OnBeforeSerializing() {
				AssignColumnsData();
				AssignNodesData();
			}
			public virtual void OnAfterDeserializing() {
				ClearTreeListData();
				if(treeList.Columns.Count == 0) CreateColumns();
				CreateNodes();
			}
			protected void CreateColumns() {
				for(int i = 0; i < columns.Length; i++) {
					CreateColumn(columns[i].ColumnName, columns[i].ColumnType, i);
				}
			}
			TreeListNode GetParentNode(Hashtable keyNodes, int keyID) {
				return (keyNodes[keyID] as TreeListNode);
			}
			protected void CreateNodes() {
				if(nodes.Length == 0) return;
				TreeList.BeginUpdate();
				TreeList.BeginLockListChanged();
				Hashtable keyNodes = new Hashtable();
				int key, parent;
				TreeListNode node;
				for(int i = 0; i < nodes.Length; i++) {
					key = nodes[i].Id;
					parent = nodes[i].ParentId;
					TreeListNode parentNode = GetParentNode(keyNodes, parent);
					if(parentNode != null) {
						node = TreeList.Data.Append(parentNode, nodes[i].cellsData);
					}
					else {
						node = TreeList.Data.Append(null, nodes[i].cellsData);
					}
					if(!keyNodes.ContainsKey(key)) keyNodes.Add(key, node);
				}
				TreeList.DoSort(TreeList.Nodes, true);
				TreeList.CancelLockListChanged();
				TreeList.EndUpdate();
			}
			protected virtual void CreateColumn(string fieldName, UnboundColumnType dataType, int index) {
				TreeList.Columns.AddField(fieldName);
				TreeList.Columns[fieldName].UnboundType = dataType;
				TreeList.Columns[fieldName].VisibleIndex = index;
			}
			protected void ClearTreeListData() {
				TreeList.ClearInternalSettings();
				TreeList.Nodes.Clear();
			}
			protected virtual void AssignColumnsData() {
				columns = new ColumnDataInfo[treeList.Columns.Count];
				for(int i = 0; i < treeList.Columns.Count; i++) {
					columns[i] = new ColumnDataInfo();
					columns[i].ColumnName = treeList.Columns[i].FieldName;
					if(TreeList.IsUnboundMode) columns[i].ColumnType = treeList.Columns[i].UnboundType;
					else columns[i].ColumnType = ConvertBoundTypeToUnbound(TreeList.Columns[i].ColumnType);
				}
			}
			protected virtual void AssignNodesData() {
				CollectNodesDataOperation op = new CollectNodesDataOperation(treeList);
				treeList.NodesIterator.DoOperation(op);
				nodes = op.Nodes.ToArray();
			}
			protected UnboundColumnType ConvertBoundTypeToUnbound(Type type) {
				if(type.Equals(typeof(string))) return UnboundColumnType.String;
				if(type.Equals(typeof(int))) return UnboundColumnType.Integer;
				if(type.Equals(typeof(bool))) return UnboundColumnType.Boolean;
				if(type.Equals(typeof(decimal))) return UnboundColumnType.Decimal;
				if(type.Equals(typeof(DateTime))) return UnboundColumnType.DateTime;
				return UnboundColumnType.Object;
			}
			protected class CollectNodesDataOperation : TreeListOperation {
				TreeList treeList = null;
				List<NodeDataInfo> collectedNodes = null;
				Dictionary<TreeListNode, int> nodesId;
				int count;
				public List<NodeDataInfo> Nodes {
					get { return collectedNodes; }
				}
				public CollectNodesDataOperation(TreeList treeList)
					: base() {
					this.treeList = treeList;
					this.collectedNodes = new List<NodeDataInfo>();
					this.nodesId = new Dictionary<TreeListNode, int>();
					this.count = treeList.Data.DataList.Count;
				}
				public override void Execute(TreeListNode node) {
					NodeDataInfo nodeData = new NodeDataInfo();
					nodeData.cellsData = GetNodeData(node);
					nodeData.Id = GetId(node);
					nodeData.ParentId = GetParentId(node);
					collectedNodes.Add(nodeData);
				}
				protected int GetId(TreeListNode node) {
					int id = node.Id;
					if(node.HasClones && nodesId.ContainsValue(id))
						id = count++; 
					nodesId.Add(node, id);
					return id;
				}
				protected int GetParentId(TreeListNode node) {
					TreeListNode parentNode = node.ParentNode;
					if(parentNode == null) return -1;
					return nodesId[parentNode];
				}
				object[] GetNodeData(TreeListNode node) {
					object[] data = new object[treeList.Columns.Count];
					for(int i = 0; i < treeList.Columns.Count; i++) {
						if(node[i] == null || node[i] is System.DBNull) continue;
						object cellData = node[i];
						data[i] = cellData;
					}
					return data;
				}
			}
			public class NodeDataInfo {
				int id;
				int parentId;
				[XmlAttribute("Id")]
				public int Id {
					get { return id; }
					set { id = value; }
				}
				[XmlAttribute("ParentId")]
				public int ParentId {
					get { return parentId; }
					set { parentId = value; }
				}
				[XmlArrayAttribute("NodeData"), XmlArrayItem("Cell")]
				public object[] cellsData;
			}
			public class ColumnDataInfo {
				string columnName;
				UnboundColumnType columnType;
				[XmlElement("ColumnName")]
				public string ColumnName {
					get { return columnName; }
					set { columnName = value; }
				}
				[XmlElement("ColumnType")]
				public UnboundColumnType ColumnType {
					get { return columnType; }
					set { columnType = value; }
				}
			}
		}
		#endregion
		#region INavigatableControl
		void INavigatableControl.AddNavigator(INavigatorOwner owner) { navigationHelper.AddNavigator(owner); }
		void INavigatableControl.RemoveNavigator(INavigatorOwner owner) { navigationHelper.RemoveNavigator(owner); }
		int INavigatableControl.RecordCount { get { return VisibleNodesCount; } }
		int INavigatableControl.Position { get { return FocusedRowIndex; } }
		bool INavigatableControl.IsActionEnabled(NavigatorButtonType type) {
			switch(type) {
				case NavigatorButtonType.First:
				case NavigatorButtonType.Prev:
					return (FocusedRowIndex > 0);
				case NavigatorButtonType.Last:
				case NavigatorButtonType.Next:
					return (FocusedRowIndex < ((INavigatableControl)this).RecordCount - 1);
				case NavigatorButtonType.NextPage:
					return (((INavigatableControl)this).Position + ViewInfo.VisibleRowCount < ((INavigatableControl)this).RecordCount - 1);
				case NavigatorButtonType.PrevPage:
					return (((INavigatableControl)this).Position - ViewInfo.VisibleRowCount > -1);
				case NavigatorButtonType.Append:
					return Data.CanAdd;
				case NavigatorButtonType.Remove:
					return (FocusedNode != null && Data.CanRemove(FocusedNode));
				case NavigatorButtonType.Edit:
					return CanShowEditor;
				case NavigatorButtonType.CancelEdit:
				case NavigatorButtonType.EndEdit:
					return (ActiveEditor != null);
			}
			return false;
		}
		void INavigatableControl.DoAction(NavigatorButtonType type) {
			switch(type) {
				case NavigatorButtonType.First: FocusedRowIndex = 0; break;
				case NavigatorButtonType.Last: FocusedRowIndex = ((INavigatableControl)this).RecordCount - 1; break;
				case NavigatorButtonType.Next: FocusedRowIndex++; break;
				case NavigatorButtonType.Prev: FocusedRowIndex--; break;
				case NavigatorButtonType.PrevPage: FocusedRowIndex -= ViewInfo.VisibleRowCount; break;
				case NavigatorButtonType.NextPage: FocusedRowIndex += ViewInfo.VisibleRowCount; break;
				case NavigatorButtonType.Append: AppendNode(null, (AppendParent == null ? -1 : AppendParent.Id)); break;
				case NavigatorButtonType.Remove: DeleteNode(FocusedNode); break;
				case NavigatorButtonType.CancelEdit: HideEditor(); break;
				case NavigatorButtonType.EndEdit: CloseEditor(); break;
				case NavigatorButtonType.Edit: ShowEditor(); break;
			}
		}
		private TreeListNode AppendParent {
			get {
				if((ModifierKeys & Keys.Shift) == 0) return null;
				return FocusedNode;
			}
		}
		#endregion INavigatableControl
		#region IToolTipControlClient
		bool IToolTipControlClient.ShowToolTips { get { return OptionsBehavior.ShowToolTips; } }
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) { return Handler.GetObjectTipInfo(point); }
		#endregion IToolTipControlClient
		#region IAppearanceOwner
		bool IAppearanceOwner.IsLoading { get { return IsLoading; } }
		#endregion IAppearanceOwner
		#region Columns
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeListColumn FocusedColumn {
			get { return VisibleColumns[FocusedCellIndex]; }
			set {
				if(FocusedColumn == value) return;
				if(value == null) return;
				if(value.TreeList != this) return;
				if(value.VisibleIndex == -1) return;
				CloseEditor();
				InternalSetFocusedCellIndex(value.VisibleIndex, 0, true);
			}
		}
		[Browsable(false), Obsolete("Use the VisibleColumns.Count property instead")]
		public int VisibleColumnCount { get { return VisibleColumns.Count; } }
		[Browsable(false)]
		public virtual bool HasColumnErrors { get { return ErrorInfo.HasErrors; } }
		protected internal virtual bool IsColumnHeaderAutoHeight { get { return OptionsView.ColumnHeaderAutoHeight == DefaultBoolean.True; } }
		protected virtual ErrorInfo ErrorInfo { get { return errorInfo; } }
		protected internal virtual CloneInfoCollection CloneInfoList { get { return cloneInfoList; } }
		protected internal virtual string GetColumnError(TreeListColumn column, TreeListNode node) {
			if(node == FocusedNode || node == null) return GetColumnError(column);
			if(column == null) return Data.GetErrorText(node.Id);
			if(IsVirtualMode) return Data.GetColumnErrorText(node.Id, column);
			return Data.GetColumnErrorText(node.Id, GetColumnByColumnHandle(column.ColumnHandle));
		}
		public virtual string GetColumnError(TreeListColumn column) {
			if(FocusedNode == null) return null;
			string res = ErrorInfo[column];
			if(res != null && res.Length > 0) return res;
			if(column == null) return Data.GetErrorText(FocusedNode.Id);
			if(IsVirtualMode) return Data.GetColumnErrorText(FocusedNode.Id, column);
			return Data.GetColumnErrorText(FocusedNode.Id, GetColumnByColumnHandle(column.ColumnHandle));
		}
		protected internal virtual DevExpress.XtraEditors.DXErrorProvider.ErrorType GetColumnErrorType(TreeListColumn column, TreeListNode node) {
			if(node == FocusedNode || node == null) return GetColumnErrorType(column);
			if(column == null) return Data.GetErrorType(node.Id);
			if(IsVirtualMode) return Data.GetColumnErrorType(node.Id, column);
			return Data.GetColumnErrorType(node.Id, GetColumnByColumnHandle(column.ColumnHandle));
		}
		public virtual DevExpress.XtraEditors.DXErrorProvider.ErrorType GetColumnErrorType(TreeListColumn column) {
			if(FocusedNode == null) return DevExpress.XtraEditors.DXErrorProvider.ErrorType.None;
			ErrorInfoEx info = (ErrorInfoEx)errorInfo;
			if(column == null) {
				if(info.ErrorText != null && info.ErrorText.Length > 0) return info.ErrorType;
				else return Data.GetErrorType(FocusedNode.Id);
			}
			if(info[column] != null && info[column].Length > 0) return info.GetErrorType(column);
			if(IsVirtualMode) return Data.GetColumnErrorType(FocusedNode.Id, column);
			return Data.GetColumnErrorType(FocusedNode.Id, GetColumnByColumnHandle(column.ColumnHandle));
		}
		public virtual void SetColumnError(TreeListColumn column, string errorText, DevExpress.XtraEditors.DXErrorProvider.ErrorType errorType) {
			if(FocusedNode == null) return;
			ErrorInfoEx info = (ErrorInfoEx)errorInfo;
			info.SetError(column, errorText, errorType);
		}
		public virtual void SetColumnError(TreeListColumn column, string errorText) {
			SetColumnError(column, errorText, DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default);
		}
		public virtual void ClearColumnErrors() {
			ErrorInfo.ClearErrors();
		}
		internal void GetColumnError(TreeListColumn column, TreeListNode node, out string error, out DevExpress.XtraEditors.DXErrorProvider.ErrorType errorType) {
			error = GetColumnError(column, node);
			errorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.None;
			if(error != null && error.Length > 0) {
				errorType = GetColumnErrorType(column, node);
				if(errorType == DevExpress.XtraEditors.DXErrorProvider.ErrorType.None) error = null;
			}
		}
		protected virtual void OnErrorInfo_Changed(object sender, EventArgs e) {
			RefreshRowsInfo();
		}
		public virtual void PopulateColumns() {
			BeginUpdate();
			try {
				LoadDataColumns();
				FitColumns();
			}
			finally {
				RestoreColumnWidths();
				EndUpdate();
			}
			if(!IsIniting)
				FocusedCellIndex = 0;
		}
		public virtual void BestFitColumns() {
			BestFitColumns(false);
		}
		public virtual void BestFitColumns(bool applyAutoWidth) {
			if(VisibleColumns.Count == 0) return;
			BeginUpdate();
			try {
				if(IsAutoWidth) applyAutoWidth = true;
				HeaderWidthCalculator.CalculateBestFit(applyAutoWidth && !CheckMinColumnWidthes(), ClientWidth - ViewInfo.ViewRects.IndicatorWidth, 0, new ArrayList(VisibleColumns), (headerObject) => { return CalcColumnBestWidth((TreeListColumn)headerObject); });
			}
			finally {
				viewInfo.CalcColumnTotalWidth();
				viewInfo.SummaryFooterInfo.NeedsRecalcRects = true;
				viewInfo.SummaryFooterInfo.NeedsRecalcAll = true;
				EndUpdate();
			}
		}
		public void ColumnsCustomization() {
			ColumnsCustomization(DevExpress.XtraEditors.Customization.CustomizationFormBase.DefaultPoint);
		}
		public virtual void ColumnsCustomization(Point location) {
			DestroyCustomization();
			customizationForm = CreateCustomizationForm();
			customizationForm.RightToLeft = IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
			customizationForm.Disposed += new EventHandler(RaiseHideCustomizationForm);
			customizationForm.ShowCustomization(location);
			RaiseShowCustomizationForm();
		}
		protected virtual TreeListCustomizationForm CreateCustomizationForm() {
			if(ActualShowBands)
				return new TreeListBandCustomizationForm(this);
			return new TreeListCustomizationForm(this, Handler);
		}
		[EditorBrowsable(EditorBrowsableState.Never), System.Obsolete("You should use the 'ColumnsCustomization' method instead of 'CreateCustomization'")]
		public virtual void CreateCustomization() {
			ColumnsCustomization();
		}
		public virtual void DestroyCustomization() {
			if(customizationForm != null) {
				Form parentForm = FindForm();
				if(parentForm != null)
					parentForm.RemoveOwnedForm(CustomizationForm);
				customizationForm.Dispose();
				customizationForm = null;
			}
		}
		public TreeListColumn GetColumnByColumnHandle(int columnHandle) {
			if(columnHandle == -1) return null;
			foreach(TreeListColumn col in Columns) {
				if(col.ColumnHandle == columnHandle)
					return col;
			}
			return null;
		}
		public TreeListColumn GetColumnByVisibleIndex(int visibleIndex) {
			return VisibleColumns[visibleIndex];
		}
		public TreeListColumn GetSortColumn(int sortIndex) {
			if(lockSort == 0) return SortedColumns[sortIndex];
			int index = 0;
			foreach(TreeListColumn col in Columns) {
				if(col.SortOrder != SortOrder.None || col.SortIndex > -1) {
					if(sortIndex == index) return col;
					index++;
				}
			}
			return null;
		}
		protected virtual void XtraClearColumns(XtraItemEventArgs e) {
			OptionsLayoutTreeList opt = e.Options as OptionsLayoutTreeList;
			bool addNewColumns = true;
			if(opt != null)
				addNewColumns = opt.AddNewColumns;
			if(e.Item.ChildProperties == null || e.Item.ChildProperties.Count == 0) {
				if(!addNewColumns) Columns.Clear();
				return;
			}
			ArrayList list = new ArrayList();
			foreach(DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo xp in e.Item.ChildProperties) {
				object col = XtraFindColumnsItem(new XtraItemEventArgs(this, Columns, xp));
				if(col != null) list.Add(col);
			}
			for(int n = Columns.Count - 1; n >= 0; n--) {
				TreeListColumn col = Columns[n];
				if(!list.Contains(col)) {
					if(addNewColumns) continue;
					col.Dispose();
				}
			}
		}
		protected virtual object XtraCreateColumnsItem(XtraItemEventArgs e) {
			OptionsLayoutTreeList opt = e.Options as OptionsLayoutTreeList;
			if(opt != null && opt.RemoveOldColumns) return null;
			TreeListColumn column = Columns.Add();
			return column;
		}
		protected virtual object XtraFindColumnsItem(XtraItemEventArgs e) {
			if(e.Item.ChildProperties == null) return null;
			string name = null;
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo xp = e.Item.ChildProperties["Name"];
			if(xp != null && xp.Value != null) name = xp.Value.ToString();
			if(name != null && name != string.Empty) return Columns.ColumnByName(name);
			xp = e.Item.ChildProperties["FieldName"];
			if(xp != null && xp.Value != null) return Columns[xp.Value.ToString()];
			return null;
		}
		internal void SetColumnHandle(TreeListColumn column) {
			column.SetColumnHandle(Data.GetColumnHandleByFieldName(column.FieldName));
		}
		private void RefreshCustomizationForm() {
			if(CustomizationForm != null)
				CustomizationForm.RefreshItems();
		}
		protected internal virtual int CalcColumnBestWidth(TreeListColumn column) {
			if(column.OptionsColumn.FixedWidth) return column.Width;
			if(column.ColumnEdit != null && column.ColumnEdit.BestFitWidth > -1)
				return column.ColumnEdit.BestFitWidth;
			TreeListOperationColumnBestWidth op = new TreeListOperationColumnBestWidth(this, column);
			ViewInfo.GInfo.AddGraphics(null);
			try {
				nodesIterator.DoOperation(op);
			}
			finally {
				ViewInfo.GInfo.ReleaseGraphics();
			}
			return op.BestWidth;
		}
		protected internal virtual int GetColumnIndent(TreeListColumn column) {
			int result = 0;
			if(column.VisibleIndex == 0) {
				result += viewInfo.FirstColumnIndent;
			}
			return result;
		}
		protected internal virtual int GetBandIndent(TreeListBand band) { 
			int result = 0;
			if(ViewInfo.IsFirstBand(band))
				result += viewInfo.FirstColumnIndent;
			return result;
		}
		protected internal int GetBandVisibleIndex(TreeListBand band) {
			if(!band.Visible || band.OwnedCollection == null) return -1;
			int res = 0;
			for(int n = 0; n < band.OwnedCollection.Count; n++) {
				TreeListBand currentBand = band.OwnedCollection[n];
				if(currentBand == band) return res;
				if(currentBand.Visible) res++;
			}
			return res;
		}
		protected internal virtual int GetColumnIndent(TreeListColumn column, TreeListNode node) {
			int result = 0;
			if(column.VisibleIndex == 0)
				result = ViewInfo.GetViewLevel(node.Level) * ViewInfo.RC.LevelWidth + ViewInfo.RC.SelectImageSize.Width + ViewInfo.RC.StateImageSize.Width + ViewInfo.RC.CheckBoxSize.Width;
			return result;
		}
		internal int GetColumnAbsoluteIndex(TreeListColumn column) {
			return Columns.IndexOf(column);
		}
		protected internal bool IsValidColumnHandle(int columnHandle) {
			return Data.IsValidColumnHandle(columnHandle);
		}
		internal void ColumnMoved(TreeListColumn column, int visibleIndexFrom, TreeListColumn focusCol) {
			int visibleIndexTo = column.VisibleIndex;
			int ind = viewInfo.FirstColumnIndent;
			BeginUpdate();
			if(visibleIndexTo == 0) {
				if(VisibleColumns.Count > 0) {
					TreeListColumn first = VisibleColumns[0];
					first.SetVisibleWidth(first.visibleWidth + ind);
					if(VisibleColumns.Count > 1) {
						TreeListColumn next = VisibleColumns[1];
						next.SetVisibleWidth(next.visibleWidth - ind);
					}
				}
				RestoreColumnWidths();
			}
			else if(visibleIndexFrom == 0) {
				if(visibleIndexTo > -1 && visibleIndexTo < VisibleColumns.Count) {
					TreeListColumn to = VisibleColumns[visibleIndexTo];
					to.SetVisibleWidth(to.visibleWidth - ind);
				}
				if(VisibleColumns.Count > 0) {
					TreeListColumn first = VisibleColumns[0];
					first.SetVisibleWidth(first.visibleWidth + ind);
				}
				RestoreColumnWidths();
			}
			if(visibleIndexTo == -1 || visibleIndexFrom == -1) FitColumns();
			CancelUpdate();
			int focusCell = VisibleColumns.IndexOf(focusCol);
			int oldCell = VisibleColumns.IndexOf(FocusedColumn);
			focusedCellIndex = focusCell;
			RaiseFocusedColumnChanged(focusCell, oldCell);
			RaiseColumnPositionChanged(column);
		}
		internal void RefreshVisibleColumnsIndexes() {
			TreeListColumn column;
			for(int i = 0; i < Columns.Count; i++) {
				column = Columns[i];
				column.visibleIndex = -1;
				column.SetVisibleCore(false);
			}
			for(int i = 0; i < VisibleColumns.Count; i++) {
				column = VisibleColumns[i];
				column.visibleIndex = i;
				column.SetVisibleCore(true);
			}
		}
		protected internal virtual void NormalizeVisibleColumnIndices() {
			if(calculatingLayout) return;
			if(HasBands) {
				NormalizeVisibleBandIndices();
				return;
			}
			ArrayList tempList = new ArrayList();
			foreach(TreeListColumn column in Columns) {
				if(column.VisibleIndex > -1) tempList.Add(column);
			}
			tempList.Sort(new VisibleColumnIndexComparer());
			VisibleColumns.Clear();
			for(int n = 0; n < tempList.Count; n++) {
				TreeListColumn column = tempList[n] as TreeListColumn;
				VisibleColumns.Add(column);
				column.visibleIndex = n;
				column.SetVisibleCore(true);
			}
		}
		protected internal void SetColumnVisibleIndex(TreeListColumn column, int newIndex, int prevIndex) {
			TreeListColumn focusedColumn = FocusedColumn;
			bool changed = false;
			if(newIndex < 0)
				changed = VisibleColumns.Hide(column);
			else
				changed = VisibleColumns.Show(column, newIndex);
			if(IsIniting || !changed) return;
			ColumnMoved(column, prevIndex, focusedColumn);
		}
		internal ArrayList CreateColumnWidthesList(int indexFrom) {
			ArrayList al = new ArrayList();
			if(indexFrom >= VisibleColumns.Count) return al;
			for(int i = indexFrom; i < VisibleColumns.Count; i++)
				al.Add(new ColumnWidthInfo(VisibleColumns[i]));
			return al;
		}
		Func<IHeaderObject, TreeListBandRowCollection> GetBandRowsDelegate() {
			if(AllowBandColumnsMultiRow)
				return (headerObject) => GetBandRows((TreeListBand)headerObject);
			return null;
		}
		protected virtual void FitColumns() {
			if(!shouldFitColumns) {
				shouldFitColumns = true;
				return;
			}
			BeginUpdate();
			lockScrollUpdate++;
			try {
				ViewInfo.CalcIndicatorWidth();
				if(Bands.VisibleCount > 0)
					HeaderWidthCalculator.Calculate(IsAutoWidth, ClientWidth - ViewInfo.ViewRects.IndicatorWidth, 0, Bands, GetBandRowsDelegate());
				else
					HeaderWidthCalculator.Calculate(IsAutoWidth, ClientWidth - ViewInfo.ViewRects.IndicatorWidth, 0, new ArrayList(VisibleColumns), GetBandRowsDelegate());
			}
			finally {
				viewInfo.SummaryFooterInfo.NeedsRecalcAll = true;
				viewInfo.SummaryFooterInfo.NeedsRecalcRects = true;
				lockScrollUpdate--;
				CancelUpdate();
			}
		}
		private bool CheckMinColumnWidthes() {
			bool minimized = false;
			int minWidth = HasBands ? GetMinBandsWidthRight(0, false) : GetMinColumnsWidthRight(0, false);
			int cx = ClientWidth - minWidth - viewInfo.ViewRects.IndicatorWidth;
			if(cx < 0) {
				if(lockScrollUpdate == 0) { 
					scrollInfo.HScroll.Visible = true;
					scrollInfo.HScrollRange = minWidth;
					scrollInfo.HScroll.LargeChange = ClientWidth;
				}
				MinimizeVisibleWidthes();
				minimized = true;
			}
			return minimized;
		}
		internal void ResizeColumnOnBestFit(int index, int byUnits) {
			TreeListColumn column = VisibleColumns[index];
			ResizeColumn(index, byUnits, byUnits + (AllowBandColumnsMultiRow ? GetMinColumnWidthRightInRow(VisibleColumns[index], false) : GetMinColumnsWidthRight(index + 1, false)));
		}
		protected internal virtual void ResizeBand(TreeListBand band, int delta, int maxPossibleWidth) {
			if(band == null || !band.Visible) return;
			BeginUpdate();
			try {
				HeaderWidthCalculator.ResizeHeader(band, Bands, delta, maxPossibleWidth, IsAutoWidth, GetBandRowsDelegate());
				if(delta != 0)
					ClearAutoHeights();
			}
			finally {
				viewInfo.SummaryFooterInfo.NeedsRecalcAll = true;
				viewInfo.SummaryFooterInfo.NeedsRecalcRects = true;
				ViewInfo.PixelScrollingInfo.Invalidate();
				EndUpdate();
				RaiseBandWidthChanged(band);
			}
		}
		protected internal virtual void OnBandWidthChanged(TreeListBand band) {
			ResizeBand(band, 0, ViewInfo.ViewRects.BandPanelWidth);
		}
		protected virtual void RaiseBandWidthChanged(TreeListBand band) {
			BandEventHandler handler = (BandEventHandler)this.Events[bandWidthChanged];
			if(handler != null) handler(this, new BandEventArgs(band));
		}
		protected internal virtual void ResizeColumn(int index, int byUnits, int maxPossibleWidth) { 
			if(byUnits == 0) return;
			if(index < 0 || (index > VisibleColumns.Count - 1)) return;
			TreeListColumn column = VisibleColumns[index];
			BeginUpdate();
			try {
				IList headerObjects = new ArrayList(VisibleColumns);
				HeaderWidthCalculator.ResizeHeader(column, headerObjects, byUnits, maxPossibleWidth, IsAutoWidth, GetBandRowsDelegate());
				ClearAutoHeights();
			}
			finally {
				viewInfo.SummaryFooterInfo.NeedsRecalcAll = true;
				viewInfo.SummaryFooterInfo.NeedsRecalcRects = true;
				ViewInfo.PixelScrollingInfo.Invalidate();
				EndUpdate();
				RaiseColumnWidthChanged(column);
			}
		}
		protected bool IsUnboundLoad { get { return (lockUnbound != 0); } }
		private void CorrectIndentWidth() {
			if(IsUnboundLoad || IsIniting) return;
			int oldMaxIndents = viewInfo.ViewRects.MaxIndents;
			viewInfo.CalcMaxIndents();
			if(viewInfo.ViewRects.MaxIndents > oldMaxIndents) {
				FitColumns();
			}
		}
		private bool InServiceColumns(string serviceName) {
			return (serviceName == KeyFieldName || serviceName == ParentFieldName ||
				serviceName == ImageIndexFieldName);
		}
		private int GetMinColumnWidthRightInRow(TreeListColumn column, bool ignoreFixedWidth) {
			TreeListBandRowCollection rows = GetBandRows(column.ParentBand);
			TreeListBandRow row = rows.FindRow(column);
			if(row != null) 
				return GetMinColumnsWidthRightCore(row.Columns.IndexOf(column) + 1, ignoreFixedWidth, row.Columns);
			return 0;
		}
		private int GetMinColumnsWidthRight(int fromIndex, bool ignoreFixedWidth) {
			return GetMinColumnsWidthRightCore(fromIndex, ignoreFixedWidth, VisibleColumns);
		}
		private int GetMinBandsWidthRight(int fromIndex, bool ignoreFixedWidth) {
			int sum = 0;
			for(int i = fromIndex; i < Bands.Count; i++) {
				TreeListBand band = Bands[i];
				if(!band.Visible) continue;
				if(band.OptionsBand.FixedWidth && !ignoreFixedWidth)
					sum += band.VisibleWidth;
				else
					sum += band.MinWidth;
			}
			return sum;
		}
		private int GetMinColumnsWidthRightCore(int fromIndex, bool ignoreFixedWidth, VisibleColumnsList columns) {
			int sum = 0;
			for(int i = fromIndex; i < columns.Count; i++) {
				TreeListColumn col = columns[i];
				if(col.OptionsColumn.FixedWidth && !ignoreFixedWidth)
					sum += col.VisibleWidth;
				else
					sum += VisibleColumns[i].MinWidth;
			}
			return sum;
		}
		private int GetFixedColumnsCountRight(int fromIndex) {
			int count = 0;
			for(int i = fromIndex; i < VisibleColumns.Count; i++)
				if(VisibleColumns[i].OptionsColumn.FixedWidth)
					count++;
			return count;
		}
		private void MinimizeVisibleWidthes() {
			for(int i = 0; i < VisibleColumns.Count; i++) {
				TreeListColumn col = VisibleColumns[i];
				if(!col.OptionsColumn.FixedWidth)
					col.SetVisibleWidth(col.MinWidth);
			}
		}
		int GetMaxFocusableColumnIndex() {
			int count = VisibleColumns.Count;
			for(int i = VisibleColumns.Count - 1; i >= 0; i--) {
				count = i;
				if(GetAllowFocus(VisibleColumns[i])) break;
			}
			return count;
		}
		int GetMinFocusableColumnIndex() {
			int count = 0;
			for(int i = 0; i < VisibleColumns.Count; i++) {
				count = i;
				if(GetAllowFocus(VisibleColumns[i])) break;
			}
			return count;
		}
		protected internal void MoveFocusedCell(int delta) {
			int focusRowInd = FocusedRowIndex;
			int supposedIndex = FocusedCellIndex + delta;
			if(supposedIndex > GetMaxFocusableColumnIndex()) {
				if(OptionsNavigation.AutoMoveRowFocus) {
					focusRowInd = Math.Min(focusRowInd + 1, RowCount - 1);
					if(focusRowInd > FocusedRowIndex) supposedIndex = 0;
				}
				else supposedIndex = VisibleColumns.Count - 1;
			}
			else if(supposedIndex < GetMinFocusableColumnIndex() && OptionsNavigation.AutoMoveRowFocus) {
				focusRowInd = Math.Max(focusRowInd - 1, 0);
				if(focusRowInd < FocusedRowIndex) supposedIndex = VisibleColumns.Count - 1;
			}
			InternalSetFocusedCellIndex(supposedIndex, delta);
			FocusedRowIndex = Math.Min(focusRowInd, RowCount - 1);
		}
		private int GetNearestCanFocusedColumnIndex(int supposedIndex, int delta) {
			if(VisibleColumns.Count == 0 || TreeListDisposing) return -1;
			TreeListColumn col = VisibleColumns[supposedIndex];
			if(col != null && GetAllowFocus(col)) return supposedIndex;
			if(delta == 0) {
				if(col.VisibleIndex == VisibleColumns.Count - 1) delta = -1; else delta = 1;
			}
			int stopCount = 0;
			for(; ; ) {
				supposedIndex += delta;
				if(stopCount > 2) {
					if(FocusedCellIndex > -1 && GetAllowFocus(VisibleColumns[FocusedCellIndex]))
						supposedIndex = FocusedCellIndex;
					else
						supposedIndex = -1;
					break;
				}
				if(supposedIndex >= VisibleColumns.Count) {
					supposedIndex = VisibleColumns.Count - 1;
					col = VisibleColumns[supposedIndex];
					if(col.OptionsColumn.AllowFocus) break;
					delta = -1;
					stopCount++;
					continue;
				}
				if(supposedIndex <= 0) {
					supposedIndex = 0;
					col = VisibleColumns[supposedIndex];
					if(GetAllowFocus(col)) break;
					delta = 1;
					stopCount++;
					continue;
				}
				col = VisibleColumns[supposedIndex];
				if(col != null && GetAllowFocus(col)) break;
			}
			return supposedIndex;
		}
		protected internal bool GetAllowFocus(TreeListColumn column) {
			return column.OptionsColumn.AllowFocus || TreeListAutoFilterNode.IsAutoFilterNode(FocusedNode);
		}
		private void RestoreColumnsVisibleWidthes(int indexFrom, ArrayList al) {
			foreach(ColumnWidthInfo wi in al) {
				VisibleColumns[indexFrom++].SetVisibleWidth(wi.Width);
			}
			ClearAutoHeights();
		}
		private void RestoreColumnsVisibleWidthes() {
			for(int i = 0; i < VisibleColumns.Count; i++) {
				TreeListColumn col = VisibleColumns[i];
				col.SetVisibleWidth(col.Width);
			}
		}
		protected virtual void RestoreColumnWidths() {
			for(int i = 0; i < VisibleColumns.Count; i++) {
				TreeListColumn col = VisibleColumns[i];
				col.width = col.VisibleWidth;
			}
			ClearAutoHeights();
		}
		private void ServiceColumnChanged(ServiceColumnEnum servColumn, string columnName) {
			if(columnName == string.Empty || IsLoading) return;
			BeginUpdate();
			try {
				if(!OptionsBehavior.PopulateServiceColumns) {
					TreeListColumn col = Columns[columnName];
					if(col != null) col.Dispose();
				}
				if(!IsUnboundMode && (servColumn == ServiceColumnEnum.KeyFieldName ||
					servColumn == ServiceColumnEnum.ParentFieldName)) {
					Data.PopulateColumns();
					LoadNodes();
					DoSort(Nodes, true); 
				}
			}
			finally {
				EndUpdate();
			}
			OnProperyChanged();
		}
		[DefaultValue(2), Category("Appearance"), XtraSerializableProperty()]
		public virtual int FixedLineWidth {
			get { return fixedLineWidth; }
			set {
				if(value < 1) value = 1;
				if(value > 12) value = 12;
				if(FixedLineWidth == value) return;
				this.fixedLineWidth = value;
				OnProperyChanged();
				LayoutChanged();
			}
		}
		protected internal virtual void SetColumnFixedStyle(TreeListColumn column, FixedStyle newValue) {
			if(IsLoading || IsDeserializing) return;
			RefreshVisibleColumnsList();
			LayoutChanged();
		}
		protected internal virtual void RefreshVisibleColumnsList() {
			NormalizeVisibleColumnIndices(); 
			if(ViewInfo == null) return;
			ViewInfo.UpdateFixedColumnInfo();
		}
		int lockColumnPositionChanged = 0;
		internal void LockRaiseColumnPositionChanged() { lockColumnPositionChanged++; }
		internal void UnlockRaiseColumnPositionChanged() { lockColumnPositionChanged--; }
		protected internal virtual void RaiseColumnPositionChanged(TreeListColumn column) {
			if(this.lockColumnPositionChanged != 0) return;
			EventHandler handler = (EventHandler)this.Events[columnPositionChanged];
			if(handler != null) 
				handler(column, EventArgs.Empty);
		}
		#endregion
		#region Nodes
		internal TreeListNode InternalCreateNode(int nodeID, TreeListNode parentNode, object tag) {
			if(parentNode == null) {
				TreeListNode node = Nodes.add(nodeID, tag);
				node.HasChildren = false;
				return node;
			}
			else {
				parentNode.HasChildren = true;
				TreeListNode node = parentNode.Nodes.add(nodeID, tag);
				node.HasChildren = false;
				if(!parentNode._visible) node._visible = false;
				int ind = node.Level + (OptionsView.ShowRoot ? 1 : 0);
				if(viewInfo.ViewRects.MaxIndents < ind) {
					viewInfo.ViewRects.MaxIndents = ind;
					LayoutChanged();
				}
				return node;
			}
		}
		public virtual TreeListNode AppendNode(object nodeData, int parentNodeId, int imageIndex, int selectImageIndex, int stateImageIndex) {
			TreeListNode node = AppendNode(nodeData, parentNodeId);
			if(node != null) {
				BeginUpdate();
				try {
					node.ImageIndex = imageIndex;
					node.SelectImageIndex = selectImageIndex;
					node.StateImageIndex = stateImageIndex;
				}
				finally {
					EndUpdate();
				}
			}
			return node;
		}
		public virtual TreeListNode AppendNode(object nodeData, int parentNodeId, int imageIndex, int selectImageIndex, int stateImageIndex, CheckState checkState) {
			TreeListNode node = AppendNode(nodeData, parentNodeId, imageIndex, selectImageIndex, stateImageIndex);
			if(node != null)
				node.CheckState = checkState;
			return node;
		}
		bool HasFocusedColumn {
			get {
				foreach(TreeListColumn column in Columns)
					if(column.VisibleIndex > -1 && column.OptionsColumn.AllowFocus) return true;
				return false;
			}
		}
		public virtual TreeListNode AppendNode(object nodeData, int parentNodeId) {
			TreeListNode parent = FindNodeByID(parentNodeId);
			return AppendNode(nodeData, parent);
		}
		public virtual TreeListNode AppendNode(object nodeData, int parentNodeId, object tag) {
			TreeListNode parent = FindNodeByID(parentNodeId);
			return AppendNode(nodeData, parent, tag);
		}
		public virtual TreeListNode AppendNode(object nodeData, int parentNodeId, CheckState checkState) {
			TreeListNode parent = FindNodeByID(parentNodeId);
			return AppendNode(nodeData, parent, checkState);
		}
		public virtual TreeListNode AppendNode(object nodeData, TreeListNode parentNode, CheckState checkState) {
			TreeListNode node = AppendNode(nodeData, parentNode, null);
			if(node != null)
				node.CheckState = checkState;
			return node;
		}
		public virtual TreeListNode AppendNode(object nodeData, TreeListNode parentNode) {
			return AppendNode(nodeData, parentNode, null);
		}
		TreeListNode autoFocusedNode = null;
		internal TreeListNode AutoFocusedNode { get { return autoFocusedNode; } set { autoFocusedNode = value; } }
		internal void SetAutoFocusedNodeInternal(TreeListNode node) {
			this.autoFocusedNode = node;
		}
		public virtual TreeListNode AppendNode(object nodeData, TreeListNode parentNode, object tag) {
			TreeListNode node = null;
			BeginUpdate();
			BeginLockListChanged();
			try {
				node = Data.Append(parentNode, nodeData, tag);
				if(node == null) return null;
				if(parentNode != null) {
					parentNode.HasChildren = true;
					if(!parentNode._visible) node._visible = false;
				}
				NullTopVisibleNode(); 
				ResetNodesCounters();
				ClearAutoHeights(true);
				DoSort(node.owner, false);
				viewInfo.SummaryFooterInfo.NeedsRecalcAll = true;
				if(!IsVirtualMode) DoFilterNode(node);
				if(node.Visible) {
					if(parentNode == null && Nodes.Count == 1) {
						ClearInternalSettings();
						SetFocusedNodeCore(node);
						SetFocusedRowIndexCore(0);
						focusedCellIndex = HasFocusedColumn ? 0 : -1; 
						Selection.Set(node);
						ViewInfo.CalcMaxIndents(); 
					}
					else if(OptionsNavigation.AutoFocusNewNode) {
						if(IsLockUpdate)
							autoFocusedNode = node;
						else
							FocusedNode = node;
					}
				}
			}
			finally {
				CancelLockListChanged();
				EndUpdate();
			}
			if(parentNode == null && Nodes.Count == 1 && node.Visible) {
				RaiseFocusedNodeChanged(null, FocusedNode);
				RaiseFocusedColumnChanged(0, -1);
			}
			return node;
		}
		public bool MoveNode(TreeListNode sourceNode, TreeListNode destinationNode) {
			return MoveNode(sourceNode, destinationNode, false);
		}
		public bool MoveNode(TreeListNode sourceNode, TreeListNode destinationNode, bool modifySource) {
			bool res = MoveNodeCore(sourceNode, destinationNode, modifySource);
			if(res) {
				ClearAutoHeights(true);
				TreeListNodes nodes = (destinationNode == null ? Nodes : destinationNode.owner);
				if(!DoSort(nodes, false))
					LayoutChanged();
			}
			return res;
		}
		protected internal virtual bool MoveNodeCore(TreeListNode sourceNode, TreeListNode destinationNode, bool modifySource) {
			if(!CanMove(sourceNode, destinationNode)) return false;
			using(TreeListMoveContext context = sourceNode.TreeList != this ?
				new TreeListOuterMoveContext(sourceNode.TreeList, this) :
				new TreeListMoveContext(this)) {
				return context.Move(sourceNode, destinationNode, modifySource);
			}
		}
		public TreeListNode CopyNode(TreeListNode sourceNode, TreeListNode destinationNode, bool cloneChildren) {
			return CopyNode(sourceNode, destinationNode, cloneChildren, false);
		}
		public TreeListNode CopyNode(TreeListNode sourceNode, TreeListNode destinationNode, bool cloneChildren, bool modifySource) {
			TreeListNode node = CopyNodeCore(sourceNode, destinationNode, modifySource, cloneChildren);
			if(node != null) {
				TreeListNodes nodes = (destinationNode == null ? Nodes : destinationNode.owner);
				if(!DoSort(nodes, false))
					LayoutChanged();
			}
			return node;
		}
		protected virtual TreeListNode CopyNodeCore(TreeListNode sourceNode, TreeListNode destinationNode, bool modifySource, bool cloneChildren) {
			if(IsVirtualMode) return null;
			if(sourceNode == null) return null;
			if(destinationNode != null && destinationNode.TreeList != this) return null;
			using(TreeListCopyContext context = sourceNode.TreeList != this ?
				new TreeListOuterCopyContext(sourceNode.TreeList, this) :
				new TreeListCopyContext(this)) {
				return context.Copy(sourceNode, destinationNode, modifySource, cloneChildren);
			}
		}
		protected virtual bool CanMove(TreeListNode sourceNode, TreeListNode destinationNode) {
			if(sourceNode == null) return false;
			if(sourceNode == destinationNode) return false;
			TreeListNodes destNodes = (destinationNode == null ? Nodes : destinationNode.Nodes);
			if(destNodes.TreeList != this) return false;
			if(sourceNode.owner == destNodes) return false;
			if(destinationNode != null && destinationNode.HasAsParent(sourceNode)) return false;
			return true;
		}
		protected virtual bool CanCloneChildren(TreeListNode sourceNode, TreeListNode destinationNode) {
			if(sourceNode == destinationNode) return false;
			TreeListNodes destNodes = (destinationNode == null ? Nodes : destinationNode.Nodes);
			if(destNodes.TreeList != this) return false;			
			if(destinationNode != null && destinationNode.HasAsParent(sourceNode)) return false;
			return true;
		}
		private void CopyChildren(TreeListNode sourceNode, TreeListNode destinationNode, TreeListNode clonedNode) {
			if(CanCloneChildren(sourceNode, destinationNode)) {
				TreeListOperation cloneChildren = new TreeListOperationCloneChildren(clonedNode.Nodes, sourceNode.Nodes);
				NodesIterator.DoLocalOperation(cloneChildren, sourceNode.Nodes);
			}
			clonedNode.HasChildren = clonedNode.Nodes.Count > 0;
		}
		public void DeleteNode(TreeListNode node) {
			InternalDeleteNode(node, true);
		}
		internal NodesIdInfoManager NodesIdManager;
		internal int lockDeleteGroup = 0;
		protected internal bool IsGroupDeleteOperation { get { return lockDeleteGroup > 0; } }
		public void BeginDelete() {
			lockDeleteGroup++;
			if(lockDeleteGroup > 1) return;
			BeginUpdate();
			NodesIdManager = new NodesIdInfoManager();
		}
		public void EndDelete() {
			if(lockDeleteGroup == 0) return;
			if(--lockDeleteGroup == 0) {
				SynchronizeDataListOnRemove(NodesIdManager);
				NodesIdManager = null;
				ViewInfo.CalcMaxIndents();
				EndUpdate();
			}
		}
		void SynchronizeNodesID(NodesIdInfoManager manager) {
			TreeListOperationSourceSynchronizeNodesID op = new TreeListOperationSourceSynchronizeNodesID(manager);
			NodesIterator.DoOperation(op);
			CloneInfoList.SynchronizeID(op);
		}
		internal void SynchronizeDataListOnRemove(NodesIdInfoManager manager) {
			if(manager.Head == null) return;
			SynchronizeNodesID(manager);
			manager.BuildReversibleList();
			BeginLockListChanged();
			try {
				Data.RemoveList(manager.Head);
			}
			finally {
				CancelLockListChanged();
			}
		}
		protected internal virtual void InternalDeleteNode(TreeListNode node, bool modifySource) {
			if(node == null || node.TreeList == null) return;
			ResetNodesCounters();
			if(lockListChanged != 0) return;
			DeleteNode(node.Id, modifySource ? node : null, true);
		}
		private void DeleteNode(int nodeId, TreeListNode dataSourceNode, bool deleteChildren) {
			deletingNodeID = nodeId;
			int selectionCount = Selection.Count;
			ClearAutoHeights(true);
			BeginUpdate();
			try {
				DeleteNodesGroup(deletingNodeID, dataSourceNode, deleteChildren);
			}
			finally {
				deletingNodeID = -1;
				EndUpdate();
			}
			if(selectionCount != Selection.Count)
				RaiseSelectionChanged();
		}
		private void RemoveNodeDataRecord(TreeListNode node) {
			BeginUpdate();
			BeginLockListChanged();
			try { Data.Remove(node); }
			finally {
				CancelLockListChanged();
				CancelUpdate();
			}
		}
		protected virtual void DeleteNodeCore(TreeListNode node, bool deleteChildren) {
			ResetRowCount();
			TreeListNode focusNodeCandidate = FocusedNode == node ? GetNewFocusedNodeOnRemove(node) : null;
			lockDelete++;
			try {
				BeginLockListChanged();
				BeginUpdate();
				try {
					RemoveNodeChildren(node, deleteChildren);
					if(Selection.Contains(node))
						Selection.InternalRemove(node);
					RemoveNodeHeight(node);
					node.owner.remove(node);
					if(node.ParentNode != null)
						node.ParentNode.HasChildren = node.owner.Count > 0;
					if(!IsUnboundLoad && !IsGroupDeleteOperation)
						viewInfo.CalcMaxIndents();
				}
				finally {
					CancelLockListChanged();
					viewInfo.SummaryFooterInfo.NeedsRecalcAll = true;
					EndUpdate();
				}
				CheckCachedNodeRemoved(node, focusNodeCandidate);
				node.owner = null;
				if((this.topVisibleNode != null && this.topVisibleNode.TreeList == null)) NullTopVisibleNode();
				if(FocusedNode != null && FocusedNode.TreeList == null) CheckFocusedNode(focusNodeCandidate);
			}
			finally {
				lockDelete--;
			}
		}
		protected internal void RemoveNodeHeight(TreeListNode node) {
			if(node == null) return;
			if(autoHeights.ContainsKey(node))
				autoHeights.Remove(node);
		}
		protected virtual void RemoveNodeChildren(TreeListNode deletedNode, bool deleteChildren) {
			TreeListOperation op = null;
			if(deleteChildren) {
				op = new TreeListOperationDeleteNodes(this);
				this.allNodesCount -= CalcNodesCountWithParent(deletedNode);
			}
			else {
				op = new TreeListOperationMoveChildNodesToRoot(deletedNode);
				this.allNodesCount--;
			}
			NodesIterator.DoLocalOperation(op, deletedNode.Nodes);
		}
		public int GetNodeIndex(TreeListNode node) {
			if(node == null) return -1;
			if(node.owner == null) return -1;
			return node.owner.IndexOf(node);
		}
		protected internal void SetNodeIndexCore(TreeListNode node, int index) {
			if(node == null) return;
			if(node.owner == null) return;
			index = Math.Min(node.owner.Count - 1, Math.Max(index, 0));
			BeginUpdate();
			try {
				node.owner.remove(node);
				if(index == node.owner.Count)
					node.owner._add(node);
				else node.owner._insert(index, node);
			}
			finally {
				ClearVisibleIndicesCache();
				ViewInfo.PixelScrollingInfo.Invalidate();
				EndUpdate();
			}
		}
		public void SetNodeIndex(TreeListNode node, int index) {
			SetNodeIndexCore(node, index);
			if(!IsUnboundLoad)
				CheckFocusedNode();
			if(!DoSort(node.owner, true))
				LayoutChanged();
		}
		public virtual void SetDefaultRowHeight() {
			RowHeight = -1;
		}
		static public bool IsNodeVisible(TreeListNode node) {
			if(node == null) return false;
			if(!TreeListFilterHelper.IsNodeVisible(node)) return false;
			TreeListNode parent = TreeListFilterHelper.GetVisibleParent(node);
			while(parent != null) {
				if(!parent.Expanded) return false;
				parent = TreeListFilterHelper.GetVisibleParent(parent);
			}
			return true;
		}
		public TreeListNode FindNodeByID(int nodeID) {
			TreeListOperationFindNodeByID findNode = new TreeListOperationFindNodeByID(nodeID);
			NodesIterator.DoOperation(findNode);
			return findNode.Node;
		}
		public TreeListNode FindNodeByKeyID(object keyID) {
			return FindNodeByServiceTreeFieldValue(KeyFieldName, keyID);
		}
		public TreeListNode FindNode(Predicate<TreeListNode> match) {
			TreeListOperationFindNode op = new TreeListOperationFindNode(match);
			NodesIterator.DoOperation(op);
			return op.Node;
		}
		protected internal TreeListNode FindNodeByParentID(object parentID) {
			return FindNodeByServiceTreeFieldValue(ParentFieldName, parentID);
		}
		TreeListNode FindNodeByServiceTreeFieldValue(string fieldName, object cellValue) {
			if(TreeListData.IsNull(cellValue)) return null;
			return FindNodeByFieldValue(fieldName, cellValue);
		}
		public virtual TreeListNode FindNodeByFieldValue(string fieldName, object cellValue) {
			if(!Data.IsValidColumnName(fieldName)) return null;
			TreeListOperationFindNodeByFieldValue findNode = new TreeListOperationFindNodeByFieldValue(cellValue, fieldName);
			nodesIterator.DoOperation(findNode);
			return findNode.Node;
		}
		public void ClearNodes() { Nodes.Clear(); }
		protected internal virtual void OnClearNodes(TreeListNodes nodes) {
			CloneInfoList.OnClearNodes(nodes);
			if(!IsUnboundMode) return;
			TreeListNode fNode = FocusedNode;
			bool clearRoot = (nodes == this.Nodes);
			if(clearRoot) {
				LockFocusedNodeChanged();
			}
			BeginUpdate();
			BeginLockRefresh();
			try {
				if(clearRoot || lockUpdateDataSource != 0) {
					for(int i = nodes.Count - 1; i > -1; i--)
						nodes.removeAt(i);
				}
				else {
					DeleteChildren(nodes.ParentNode);
				}
			}
			finally {
				CancelLockRefresh();
				CancelUpdate();
				if(clearRoot) {
					Data.Clear();
					UnlockFocusedNodeChanged(fNode);
				}
			}
		}
		int focusedNodeChangedCounter = 0;
		protected bool IsFocusedNodeChangedLocked { get { return (focusedNodeChangedCounter != 0); } }
		void LockFocusedNodeChanged() {
			focusedNodeChangedCounter++;
		}
		void UnlockFocusedNodeChanged(TreeListNode oldNode) {
			focusedNodeChangedCounter--;
			RaiseFocusedNodeChanged(oldNode, FocusedNode);
		}
		protected internal virtual void OnClearNodesComplete(TreeListNodes nodes) {
			if(nodes == this.Nodes) Selection.Clear();
			else {
				TreeListOperation op = new TreeListOperationDeleteNodeFromSelection(Selection);
				BeginUpdate();
				try {
					NodesIterator.DoLocalOperation(op, nodes);
				}
				finally {
					CancelUpdate();
				}
			}
			ResetNodesCounters(); 
			ClearAutoHeights();
			NullTopVisibleNode();
			if(FocusedNode != null && FocusedNode.HasAsParent(nodes.ParentNode)) {
				TreeListNode oldNode = FocusedNode;
				SetFocusedRowIndexCore(-1);
				SetFocusedNodeCore(null);
				int oldCell = focusedCellIndex;
				focusedCellIndex = -1;
				RaiseFocusedNodeChanged(oldNode, null);
				RaiseFocusedColumnChanged(-1, oldCell);
			}
			viewInfo.SummaryFooterInfo.NeedsRecalcAll = true;
			LayoutChanged();
		}
		protected virtual bool CheckValidateFocusNode() {
			if(!IsFocusedNodeDataModified || FocusedNode == null) return true;
			try {
				ValidateNodeEventArgs e = new ValidateNodeEventArgs(FocusedNode);
				RaiseValidateNode(e);
				if(!e.Valid) throw new WarningException(e.ErrorText);
				if(!TreeListAutoFilterNode.IsAutoFilterNode(FocusedNode))
					canFilterNode = true;
				EndCurrentEdit();
			}
			catch(Exception e) {
				try {
					InvalidNodeExceptionEventArgs ex = new InvalidNodeExceptionEventArgs(e, e.Message + TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.InvalidNodeExceptionText), FocusedNode);
					RaiseInvalidNodeException(ex);
					if(ex.ExceptionMode == ExceptionMode.Ignore) {
						return true;
					}
					return false;
				}
				catch(HideException) {
					return false;
				}
				finally {
					RefreshRowsInfo();
				}
			}
			return true;
		}
		private bool LoadAll() {
			Data.CreateNodes(null);
			return true;
		}
		private bool LoadRoot() { 
			return true;
		}
		private bool LoadChildren(TreeListNode node) {
			return true;
		}
		bool loadingNodes = false;
		private void LoadNodes() {
			if(lockReloadNodes != 0) return;
			BeginUpdate();
			try {
				loadingNodes = true;
				LoadAll();
			}
			finally {
				loadingNodes = false;
				if(!IsLoading)
					FilterNodes();
				EndUpdate();
			}
			ViewInfo.CalcMaxIndents();
			RaiseNodesReloaded();
		}
		public virtual void LockReloadNodes() {
			lockReloadNodes++;
		}
		public virtual void UnlockReloadNodes() {
			lockReloadNodes--;
			if(lockReloadNodes == 0 && !IsUnboundMode)
				ReloadNodes();
		}
		private void NodeImageChanged(TreeListNode node) {
			RefreshRowsInfo();
		}
		protected virtual int GetImageIndexFromDataSource(TreeListNode node) {
			if(node == null) return -1;
			object objIndex = node[ImageIndexFieldName];
			int index = -1;
			if(objIndex != null && objIndex.GetType().IsValueType)
				index = Convert.ToInt32(objIndex);
			return index;
		}
		internal string InternalGetPreviewText(TreeListNode node) {
			if(node == null) return string.Empty;
			string previewText = GetNativePreviewText(node);
			RaiseGetPreviewText(node, ref previewText);
			return previewText;
		}
		internal string InternalGetPrintPreviewText(TreeListNode node) {
			if(node == null) return string.Empty;
			string previewText = GetNativePreviewText(node);
			RaiseGetPrintPreviewText(node, ref previewText);
			return previewText;
		}
		private string GetNativePreviewText(TreeListNode node) {
			string previewText = string.Empty;
			if(!(PreviewFieldName == null || PreviewFieldName == string.Empty)) {
				if(Columns[PreviewFieldName] != null) {
					previewText = node.GetDisplayText(PreviewFieldName);
				}
				else {
					object value = Data.GetValue(node.Id, PreviewFieldName);
					if(value != null)
						previewText = value.ToString();
				}
			}
			return previewText;
		}		
		internal object GetPrintSummaryValue(TreeListNodes nodes, TreeListColumn col, SummaryItemType item, bool recursively, bool isSummaryFooter) {
			if(nodes == null || col == null) return null;
			if(item == SummaryItemType.Custom) {
				object val = null;
				RaiseGetPrintCustomSummaryValue(nodes, col, ref val, isSummaryFooter);
				return val;
			}
			return GetSummaryValueCore(nodes, col, item, recursively, nodes == Nodes);
		}
		public object GetSummaryValue(TreeListColumn column) {
			if(column == null) return null;
			return GetSummaryValue(column, column.AllNodesSummary);
		}
		public object GetSummaryValue(TreeListColumn column, bool allNodes) {
			if(column == null) return null;
			return GetSummaryValueCore(Nodes, column, column.SummaryFooter, allNodes, nodes == Nodes);
		}
		public object GetGroupSummaryValue(TreeListColumn column, TreeListNodes nodes) {
			if(column == null) return null;
			if(nodes == null) nodes = Nodes;
			return GetSummaryValueCore(nodes, column, column.RowFooterSummary, false, nodes == Nodes);
		}
		protected internal virtual object GetSummaryValueCore(TreeListNodes nodes, TreeListColumn col, SummaryItemType item, bool recursively, bool isSummaryFooter) {
			if(nodes == null || col == null) return null;
			TreeListOperationCalcSummary operation = null;
			switch(item) {
				case SummaryItemType.Count:
					if(recursively) {
						TreeListOperationVisibleFullCount op = new TreeListOperationVisibleFullCount(nodes);
						nodesIterator.DoOperation(op);
						return op.Count;
					}
					return TreeListFilterHelper.GetVisibleNodeCount(nodes);
				case SummaryItemType.Sum:
					operation = new TreeListOperationCalcSummarySum(col, item, recursively);
					break;
				case SummaryItemType.Min:
				case SummaryItemType.Max:
					operation = new TreeListOperationCalcSummaryMinMax(col, item, recursively);
					break;
				case SummaryItemType.Average:
					operation = new TreeListOperationCalcSummaryAverage(col, item, recursively);
					break;
				case SummaryItemType.Custom:
					object val = null;
					RaiseGetCustomSummaryValue(nodes, col, ref val, isSummaryFooter);
					return val;
			}
			if(operation != null) {
				nodesIterator.DoLocalOperation(operation, nodes);
				return operation.Result;
			}
			return null;
		}
		protected internal virtual bool IncludeRowFooterSummaryWidthInColumnBestFit {
			get { return OptionsView.ShowRowFooterSummary; }
		}
		protected int GetRowsAsFootersCount(int indexFrom, int indexTo) {
			int rf = 0;
			if(OptionsView.ShowRowFooterSummary &&
				indexFrom < indexTo && indexFrom != -1) {
				TreeListNode from = GetNodeByVisibleIndex(indexFrom);
				TreeListNode to = GetNodeByVisibleIndex(indexTo);
				int numFooters = GetVisibleParentNodesCount(from, to);
				rf = numFooters * Convert.ToInt32((float)viewInfo.RC.GroupFooterHeight / (float)viewInfo.RowHeight);
			}
			return rf;
		}
		protected int GetVisibleParentNodesCount(TreeListNode from, TreeListNode to) {
			TreeListNode fromRoot = from.RootNode;
			TreeListNode toRoot = to.RootNode;
			int fromIndex = Nodes.IndexOf(fromRoot),
				toIndex = Nodes.IndexOf(toRoot);
			if(fromIndex > toIndex || fromIndex == -1) return 0;
			TreeListOperationGroupNodesCount op = new TreeListOperationGroupNodesCount();
			for(int i = toIndex; i >= fromIndex; i--) {
				TreeListNode rootNode = Nodes[i];
				if(rootNode == fromRoot || rootNode == toRoot) {
					if(fromRoot == toRoot) {
						TreeListOperationGroupNodesCount op2 = new TreeListOperationGroupNodesCount();
						GetTopParents(fromRoot, from, op2);
						int count = op2.Count;
						op2.Count = 0;
						GetTopParents(toRoot, to, op2);
						return op2.Count - count;
					}
					else {
						if(rootNode == fromRoot) {
							TreeListOperationGroupNodesCount op2 = new TreeListOperationGroupNodesCount();
							GetTopParents(rootNode, from, op2);
							int count = op2.Count;
							op2.Count = (rootNode.Expanded ? 1 : 0);
							nodesIterator.DoLocalOperation(op2, rootNode.Nodes);
							op.Count += op2.Count - count;
						}
						if(rootNode == toRoot) {
							GetTopParents(rootNode, to, op);
						}
					}
				}
				else {
					if(rootNode.HasChildren && rootNode.Expanded) {
						nodesIterator.DoLocalOperation(op, rootNode.Nodes);
						op.Count++;
					}
				}
			}
			return op.Count;
		}
		private void GetTopParents(TreeListNode rootNode, TreeListNode from, TreeListOperationGroupNodesCount op) {
			if(rootNode == from || from == null) return;
			int index = from.owner.IndexOf(from);
			for(int i = 0; i < index; i++) {
				TreeListNode node = from.owner[i];
				if(node.HasChildren && node.Expanded) {
					nodesIterator.DoLocalOperation(op, node.Nodes);
					op.Count++;
				}
			}
			from = from.ParentNode;
			GetTopParents(rootNode, from, op);
		}
		protected internal virtual TreeListNode CreateNode(int nodeID, TreeListNodes owner, object tag) {
			CreateCustomNodeEventArgs e = new CreateCustomNodeEventArgs(nodeID, owner, tag);
			RaiseCreateCustomNode(e);
			if(e.Node != null)
				return e.Node;
			return new TreeListNode(nodeID, owner);
		}
		protected virtual TreeListNodesIterator CreateNodesIterator() {
			return new TreeListNodesIterator(this, true);
		}
		private TreeListPainter CreatePainterCore() {
			TreeListPainter tlPainter = CreatePainter();
			tlPainter.PaintLink = CreatePaintLink();
			return tlPainter;
		}
		protected virtual PaintLink CreatePaintLink() {
			return new PaintLink(this);
		}
		protected virtual TreeListPainter CreatePainter() {
			return new TreeListPainter();
		}
		protected virtual TreeListViewInfo CreateViewInfo() {
			return new TreeListViewInfo(this);
		}
		protected virtual TreeListHandler CreateHandler() {
			return new TreeListHandler(this);
		}
		protected virtual TreeListColumnCollection CreateColumns() {
			return new TreeListColumnCollection(this);
		}
		protected virtual TreeListBandCollection CreateBands() {
			return new TreeListBandCollection(null, this);
		}
		protected override EditorContainerHelper CreateHelper() {
			return new TreeListContainerHelper(this);
		}
		protected virtual TreeListPrinter CreatePrinter() {
			return new TreeListPrinter(this);
		}
		protected virtual TreeListOptionsView CreateOptionsView() {
			return new TreeListOptionsView();
		}
		protected virtual TreeListOptionsBehavior CreateOptionsBehavior() {
			return new TreeListOptionsBehavior(this);
		}
		protected virtual TreeListOptionsNavigation CreateOptionsNavigation() {
			return new TreeListOptionsNavigation();
		}
		protected virtual TreeListOptionsPrint CreateOptionsPrint() {
			return new TreeListOptionsPrint();
		}
		protected virtual TreeListOptionsSelection CreateOptionsSelection() {
			return new TreeListOptionsSelection();
		}
		protected virtual TreeListOptionsMenu CreateOptionsMenu() {
			return new TreeListOptionsMenu();
		}
		protected virtual OptionsLayoutTreeList CreateOptionsLayout() {
			return new OptionsLayoutTreeList();
		}
		protected virtual TreeListOptionsFilter CreateOptionsFilter() {
			return new TreeListOptionsFilter();
		}
		protected virtual TreeListOptionsFind CreateOptionsFind() {
			return new TreeListOptionsFind();
		}
		protected virtual TreeListOptionsCustomization CreateOptionsCustomization() { 
			return new TreeListOptionsCustomization();
		}
		protected virtual TreeListOptionsClipboard CreateOptionsClipboad() {
			return new TreeListOptionsClipboard();
		}
		protected virtual TreeListOptionsDragAndDrop CreateOptionsDragAndDrop() {
			return new TreeListOptionsDragAndDrop();
		}
		protected internal virtual void OnSelectionChanged() {
			if(lockSelection > 0) return;
			Handler.OnSelectionChanged(); 
		}
		private void CheckCachedNodeRemoved(TreeListNode nodeRemoved, TreeListNode node) {
			if(this.topVisibleNode == nodeRemoved) NullTopVisibleNode();
			if(FocusedNode == null) return;
			if(FocusedNode == nodeRemoved) {
				if(VisibleNodesCount > 0)
					SetFocusedRowIndexCore(-1);
				FocusedNode = node;
			}
		}
		TreeListNode GetNewFocusedNodeOnRemove(TreeListNode nodeToRemove) {
			int index = nodeToRemove.owner.IndexOf(nodeToRemove);
			int count = TreeListFilterHelper.GetVisibleNodeCount(nodeToRemove.owner);
			TreeListNode candidate = null;
			if(index < count - 1) 
				candidate = GetNextNewFocusedNode(nodeToRemove, index, count);
			if(candidate != null)
				return candidate;
			if(index > 0) 
				candidate = GetPrevNewFocusedNode(nodeToRemove, index, count);
			return candidate != null ? candidate : TreeListFilterHelper.GetVisibleParent(nodeToRemove);
		}
		TreeListNode GetNextNewFocusedNode(TreeListNode nodeToRemove, int index, int count) {
			while(++index < count) {
				TreeListNode node = nodeToRemove.owner[index];
				if(node.Visible)
					return node;
			}
			return null;
		}
		TreeListNode GetPrevNewFocusedNode(TreeListNode nodeToRemove, int index, int count) {
			while(--index >= 0) {
				TreeListNode node = nodeToRemove.owner[index];
				if(node.Visible)
					return node;
			}
			return null;
		}
		bool IsDeletingFocusedNode(TreeListNode nodeToRemove) { return (nodeToRemove == null); }
		internal void CheckCurrencyManagerPosition(bool focused, int nodeID) {
			if(focused) {
				TreeListNode newFocus = GetNewFocusedNodeOnRemove(FocusedNode);
				if(newFocus == null) Position = -1;
				else if(newFocus.Id < Data.DataList.Count - 1)
					Position = newFocus.Id;
			}
			else if(Position == Data.DataList.Count - 1)
				Position = Math.Max(-1, Data.DataList.Count - 2);
		}
		internal void CheckFocusedNode() {
			CheckFocusedNode(FocusedNode);
		}
		private void CheckFocusedNode(TreeListNode node) {
			NullTopVisibleNode();
			if(FocusedNode == node) {
				SetFocusedRowIndexCore(GetVisibleIndexByNode(node));
				ExpandToRoot(node);
			}
		}
		internal void SynchronizeNodesID(int indexFrom, int step) {
			TreeListOperationSynchronizeID op = new TreeListOperationSynchronizeNodesID(indexFrom, step);
			nodesIterator.allowHasChildren = false;
			nodesIterator.DoOperation(op);
			nodesIterator.allowHasChildren = true;
			CloneInfoList.SynchronizeID(op);
		}
		protected virtual Hashtable SaveNodesData() {
			if(Nodes.Count == 0) return null;
			TreeListOperationSaveNodesData save = new TreeListOperationSaveNodesData();
			nodesIterator.DoOperation(save);
			return save.NodesData;
		}
		protected virtual void RestoreNodesData(Hashtable nodesData) {
			if(nodesData == null) return;
			TreeListOperationRestoreNodesData restore = new TreeListOperationRestoreNodesData(nodesData);
			nodesIterator.DoOperation(restore);
		}
		private int CalcRowCount() {
			TreeListOperationCount countNodes = new TreeListOperationCount(Nodes);
			nodesIterator.DoOperation(countNodes);
			return countNodes.Count;
		}
		private int CalcAllNodesCount() {
			TreeListOperationCount countNodes = new TreeListOperationFullCount(Nodes);
			nodesIterator.DoOperation(countNodes);
			return countNodes.Count;
		}
		private int CalcNodesCountWithParent(TreeListNode parent) {
			TreeListOperationCountWithParent op = new TreeListOperationCountWithParent(parent.Nodes);
			nodesIterator.DoLocalOperation(op, parent.Nodes);
			return op.Count;
		}
		protected internal bool IsFocusedNodeDataModified { get { return isFocusedNodeDataModified; } }
		public void DeleteSelectedNodes() {
			nodes.DeleteSelectedNodes();
		}
		#endregion
		#region Navigation
		public virtual TreeListNode MoveFirst() {
			if(RowCount > 0) {
				FocusedRowIndex = 0;
				return FocusedNode;
			}
			return null;
		}
		public virtual TreeListNode MovePrev() {
			TreeListNode pvNode, node = null;
			if(FocusedNode != null) {
				node = FocusedNode;
				pvNode = MovePrevVisible();
				if(node == pvNode)
					return node;
				int oldRowCount = RowCount;
				node = ExpandToLast(pvNode.Nodes);
				FocusedRowIndex += RowCount - oldRowCount;
			}
			return node;
		}
		public virtual TreeListNode MovePrevVisible() {
			if(RowCount > 0) {
				FocusedRowIndex = Math.Max(0, FocusedRowIndex - 1);
				return FocusedNode;
			}
			return null;
		}
		public virtual TreeListNode MoveNext() {
			TreeListNode node = null;
			if(FocusedNode != null) {
				node = FocusedNode;
				if(node.HasChildren && !node.Expanded) {
					node.Expanded = true;
					RowCount = -1;
				}
				node = MoveNextVisible();
			}
			return node;
		}
		public virtual TreeListNode MoveNextVisible() {
			if(RowCount > 0) {
				FocusedRowIndex++;
				return FocusedNode;
			}
			return null;
		}
		public virtual TreeListNode MoveLast() {
			TreeListNode node = ExpandToLast(Nodes);
			FocusedRowIndex = RowCount - 1;
			return node;
		}
		public virtual TreeListNode MoveLastVisible() {
			if(RowCount > 0) {
				FocusedRowIndex = RowCount - 1;
				return FocusedNode;
			}
			return null;
		}
		protected internal TreeListNode ExpandToLast(TreeListNodes nodes) {
			TreeListNode node = null;
			BeginUpdate();
			try {
				if(nodes.ParentNode != null) nodes.ParentNode.Expanded = true;
				if(nodes.Count > 0) {
					node = nodes[nodes.Count - 1];
					TreeListNode child = node;
					while(child != null) {
						if(child.HasChildren) {
							child.Expanded = true;
							node = child.Nodes.LastNode;
							child = node;
						}
						else child = null;
					}
				}
				ResetRowCount();
			}
			finally { EndUpdate(); }
			return node;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("You must use the ExpandAll method instead")]
		public void FullExpand() {
			ExpandAll();
		}
		public virtual void ExpandAll() {
			ExpandAllCore(-1);
		}
		public virtual void ExpandToLevel(int level) {
			ExpandAllCore(level);
		}
		protected void ExpandAllCore(int maxLevel) {
			if(CanForceLoadNodes)
				UpdateDataSource(true);
			if(Nodes.Count > 0) {
				TreeListOperationExpandAll expand = new TreeListOperationExpandAll(maxLevel);
				BeginUpdate();
				lockExpandCollapse++;
				try {
					ResetRowCount();
					ExecuteExpandAction(delegate {
						nodesIterator.DoOperation(expand);
					}, Nodes.FirstNode);
					SetFocusedRowIndexCore(GetVisibleIndexByNode(FocusedNode));
				}
				finally {
					lockExpandCollapse--;
					EndUpdate();
					ViewInfo.PixelScrollingInfo.Invalidate();
				}
			}
		}
		internal void ExecuteExpandAction(Action0 action, TreeListNode node) {
			if(TopVisibleNodeIndex > 0 && GetVisibleIndexByNode(node) < TopVisibleNodeIndex) {
				int oldTopNodesCount = CalcVisibleNodeCount(node, TopVisibleNode);
				action();
				if(oldTopNodesCount > -1) {
					int delta = CalcVisibleNodeCount(node, TopVisibleNode) - oldTopNodesCount;
					if(delta > 0)
						TopVisibleNodeIndex += delta;
				}
			}
			else {
				action();
			}
		}
		int CalcVisibleNodeCount(TreeListNode fromNode, TreeListNode toNode) {
			TreeListOperationCountVisible op = new TreeListOperationCountVisible();
			NodesIterator.DoVisibleNodesOperation(op, fromNode, toNode);
			return op.Count;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("You must use the CollapseAll method instead")]
		public void FullCollapse() {
			CollapseAll();
		}
		public virtual void CollapseAll() {
			if(Nodes.Count > 0) {
				TreeListNode focusNode = FocusedNode;
				TreeListNode rootNode = (focusNode == null ? null : focusNode.RootNode);
				TreeListOperationCollapseExpand collapse = new TreeListOperationCollapseExpand(false);
				BeginUpdate();
				lockExpandCollapse++;
				try {
					ResetRowCount();
					SetTopVisibleNodeIndexCore(0);
					nodesIterator.DoOperation(collapse);
					if(!TreeListAutoFilterNode.IsAutoFilterNode(rootNode))
						FocusedRowIndex = Nodes.IndexOf(rootNode);
				}
				finally {
					lockExpandCollapse--;
					EndUpdate();
					ViewInfo.PixelScrollingInfo.Invalidate();
				}
				if(rootNode != focusNode)
					RaiseFocusedNodeChanged(focusNode, rootNode);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("You must use the node.ExpandAll method instead")]
		public virtual void FullExpandNode(TreeListNode node) { node.ExpandAll(); }
		protected internal virtual void InternalFullExpandNode(TreeListNode node) {
			if(node == null) {
				ExpandAll();
				return;
			}
			if(!node.HasChildren) return;
			TreeListOperationCollapseExpand expand = new TreeListOperationCollapseExpand(true);
			BeginUpdate();
			lockExpandCollapse++;
			try {
				ResetRowCount();
				Action0 expandAction = delegate {
					node.Expanded = true;
					nodesIterator.DoLocalOperation(expand, node.Nodes);
				};
				ExecuteExpandAction(expandAction, node);
				SetFocusedRowIndexCore(GetVisibleIndexByNode(FocusedNode));
			}
			finally {
				lockExpandCollapse--;
				EndUpdate();
			}
		}
		public virtual int SetFocusedNode(TreeListNode node) {
			if(node == this.focusedNode) return FocusedRowIndex;
			if(node != null && node.TreeList != this) return FocusedRowIndex;
			int index = MakeNodeVisible(node);
			FocusedRowIndex = index;
			return index;
		}
		internal void PresetFocusedNode(TreeListNode precomputedNode) {
			if(precomputedNode != null && precomputedNode.TreeList != this) return;
			SetFocusedNodeCore(precomputedNode);
		}
		internal void SetFocusedNodeCore(TreeListNode value) {
			focusedNode = value;
		}
		public virtual int MakeNodeVisible(TreeListNode node) {
			if(node == null) return -1;
			ExpandToRoot(node);
			int index = GetVisibleIndexByNode(node);
			if(index < 0) return index;
			if(index < TopVisibleNodeIndex) TopVisibleNodeIndex = index;
			else {
				rowAsFooterCount = GetRowsAsFootersCount(TopVisibleNodeIndex, index);
				int visibleCount = ViewInfo.CalcVisibleNodeCount(TopVisibleNode);
				if(TopVisibleNodeIndex + visibleCount <= index)
					TopVisibleNodeIndex = index;
				rowAsFooterCount = 0;
			}
			CheckIncreaseVisibleRows();
			return index;
		}
		protected internal virtual void CheckIncreaseVisibleRows() {
			if(IsPixelScrolling) { 
				if(ViewInfo.IsValid) 
					TopVisibleNodePixel -= ViewInfo.ViewRects.EmptyRows.Height;
				return;
			}
			if(ViewInfo.RowsInfo.IncreaseVisibleRows > 0)
				TopVisibleNodeIndex -= ViewInfo.RowsInfo.IncreaseVisibleRows;
		}
		protected void ExpandToRoot(TreeListNode node) {
			if(node == null) return;
			BeginUpdate();
			try {
				TreeListNode parentNode = node.ParentNode;
				while(parentNode != null) {
					int index = GetVisibleIndexByNode(parentNode);
					if(parentNode.Expanded && index > -1) break;
					parentNode.Expanded = true;
					if(index < TopVisibleNodeIndex)
						TopVisibleNodeIndex += parentNode.Nodes.Count;
					parentNode = parentNode.ParentNode;
				}
			}
			finally {
				EndUpdate();
			}
		}
		void GetVisibleNodesCount(TreeListNode node, ref int count) {
			if(TreeListFilterHelper.IsNodeVisible(node))
				count++;
			if(node.HasChildren && node.Expanded)
				foreach(TreeListNode child in node.Nodes)
					GetVisibleNodesCount(child, ref count);
		}
		public int GetVisibleIndexByNode(TreeListNode node) {
			if(TreeListAutoFilterNode.IsAutoFilterNode(node))
				return TreeList.AutoFilterNodeId;
			if(!IsNodeVisible(node)) return -1;
			int index = -1;
			if(nodeToVisibleIndexCache.TryGetValue(node, out index))
				return index;
			index = GetVisibleIndexByNodeCore(node);
			nodeToVisibleIndexCache[node] = index;
			return index;
		}
		int GetVisibleIndexByNodeCore(TreeListNode node) {
			int count = 0;
			TreeListNodes nodes = node.owner;
			if(nodes == null) return -1;
			int nodeIndex = nodes.IndexOf(node);
			for(int i = nodeIndex - 1; i >= 0; i--)
				GetVisibleNodesCount(nodes[i], ref count);
			node = nodes.ParentNode;
			if(node == null) {
				return count;
			}
			else {
				if(TreeListFilterHelper.IsNodeVisible(node))
					count++;
			}
			count += GetVisibleIndexByNodeCore(node);
			return count;
		}
		public TreeListNode GetNodeByVisibleIndex(int index) {
			if(index == AutoFilterNodeId)
				return ViewInfo.AutoFilterRowInfo != null ? ViewInfo.AutoFilterRowInfo.Node : null;
			int count = 0;
			TreeListNode node = null;
			if(visibleIndexToNodeCache.TryGetValue(index, out node))
				return node;
			node = LocateNode(Nodes, ref count, index);
			if(node != null)
				visibleIndexToNodeCache[index] = node;
			return node;
		}
		private TreeListNode LocateNode(TreeListNodes nodes, ref int count, int index) {
			TreeListNode node = null;
			if(count > index) return node;
			for(int i = 0; i < nodes.Count; i++) {
				TreeListNode nd = nodes[i];
				if(count == index && TreeListFilterHelper.IsNodeVisible(nd)) {
					node = nd;
					break;
				}
				if(TreeListFilterHelper.IsNodeVisible(nd))
					count++;
				if(nd.HasChildren && nd.Expanded) {
					node = LocateNode(nd.Nodes, ref count, index);
					if(node != null && TreeListFilterHelper.IsNodeVisible(node)) break;
				}
			}
			return node;
		}
		internal bool IsLockExpandCollapse { get { return (lockExpandCollapse != 0); } }
		internal int GetNodeVisibleIndexOnExpand(TreeListNode node) {
			if(IsLockExpandCollapse) return -1;
			return GetVisibleIndexByNode(node);
		}
		#endregion
		#region Menu
		public void InvokeMenuItemClick(TreeListMenuItemClickEventArgs e) {
			RaiseTreeListMenuItemClick(e);
		}
		Point contextMenuPoint = Dragging.DragWindow.InvisiblePoint;
		protected internal void DoShowTreeListMenu(DevExpress.XtraTreeList.Menu.TreeListMenu menu, Point p) {
#pragma warning disable 612 // Obsolete
#pragma warning disable 618 // Obsolete
			TreeListMenuEventArgs e = new TreeListMenuEventArgs(menu, p, true);
			RaisePopupMenuShowing(e);
			RaiseShowTreeListMenu(e);
#pragma warning restore 618 // Obsolete
#pragma warning restore 612 // Obsolete
			if(e.Menu == null || !e.Allow) return;
			contextMenuPoint = p;
			try {
				e.Menu.Show(p); 
			}
			finally {
				contextMenuPoint = Dragging.DragWindow.InvisiblePoint;
			}
		}
		MouseEventArgs GetMouseUpEventArgs(MouseEventArgs e) {
			if(contextMenuPoint != Dragging.DragWindow.InvisiblePoint)
				e = new MouseEventArgs(e.Button, e.Clicks, contextMenuPoint.X, contextMenuPoint.Y, e.Delta);
			return e;
		}
		#endregion
		#region Common Functions
		protected internal virtual bool ShowEditorOnMouseUp { get { return OptionsBehavior.ShowEditorOnMouseUp || IsCellSelect; } }
		protected override void RaiseEditorKeyDown(KeyEventArgs e) {
			base.RaiseEditorKeyDown(e);
			if(e.Handled) return;
			e.Handled = Handler.ProcessChildControlKey(e);
		}
		protected override bool IsInputKey(Keys key) {
			bool res = base.IsInputKey(key);
			switch(key) {
				case Keys.Left:
				case Keys.Right:
				case Keys.Up:
				case Keys.Down:
					res = true;
					break;
				case Keys.Tab:
					if(!CanUseTab)
						res = false;
					break;
			}
			return res;
		}
		protected override bool IsInputChar(char charCode) {
			return true;
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			Keys key = keyData & (~Keys.Modifiers);
			if(key == Keys.Down || key == Keys.Up ||
				key == Keys.Left || key == Keys.Right) return false;
			if(key == Keys.Tab) {
				if((keyData & Keys.Control) == Keys.Control) return ProcessControlTab((keyData & Keys.Shift) != Keys.Shift);
				if(CanUseTab) return false;
			}
			if((key == Keys.Enter || key == Keys.Escape) && State == TreeListState.Editing) return false;
			return base.ProcessDialogKey(keyData);
		}
		protected internal virtual bool ProcessControlTab(bool forward) {
			EditorHelper.BeginAllowHideException();
			try {
				if(ActiveEditor != null) CloseEditor();
				if(forward)
					base.ProcessDialogKey(Keys.Tab);
				else
					base.ProcessDialogKey(Keys.Tab | Keys.Shift);
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
			}
			return true;
		}
		protected virtual bool ProcessControlTab() { return ProcessControlTab(true); }
		public void SetDefaultOptionsView() { OptionsView.Assign(CreateOptionsView()); }
		public void SetDefaultBehaviorOptions() { OptionsBehavior.Assign(CreateOptionsBehavior()); }
		public void SetDefaultNavigationOptions() { OptionsNavigation.Assign(CreateOptionsNavigation()); }
		public void SetDefaultCustomizationOptions() { OptionsCustomization.Assign(CreateOptionsCustomization()); }
		public void SetDefaultPrintOptions() { OptionsPrint.Assign(CreateOptionsPrint()); }
		public void SetDefaultSelectionOptions() { OptionsSelection.Assign(CreateOptionsSelection()); }
		public virtual void BeginUpdate() {
			lockUpdate++;
		}
		public virtual void EndUpdate() {
			if(lockUpdate == 0) return;
			if(--lockUpdate == 0)
				LayoutChanged();
		}
		public virtual void CancelUpdate() {
			if(lockUpdate == 0) return;
			--lockUpdate;
		}
		public virtual void BeginSort() {
			BeginUpdate();
			lockSort++;
		}
		public virtual void EndSort() {
			EndUpdate();
			if(--lockSort == 0) {
				DoSort(Nodes, true);
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			scrollInfo.CreateHandles(); 
		}
		void BeginLockRefresh() { this.lockRefresh++; }
		void CancelLockRefresh() { this.lockRefresh--; }
		public override void Refresh() {
			ViewInfo.IsValid = false;
			if(lockRefresh == 0)
				base.Refresh();
		}
		protected internal bool IsLockUpdate { get { return (lockUpdate != 0); } }
		public virtual void LayoutChanged() {
			HideEditorCore(!Data.DataHelper.Posting);
			if(!IsLookUpMode) {
				CheckAutoFocusedNode(); 
				UpdateFocusedRowIndex();
			}
			if(IsLockUpdate || IsIniting || TreeListDisposing || IsDeserializing) return;
			NormalizeVisibleColumnIndices();
			CheckUpdateRootBands();
			ViewInfo.IsValid = false;
			UpdateLayout();
			ViewInfo.PaintAnimatedItems = false;
			Invalidate();
			if(CustomizationForm != null) {
				SaveCustomizationFormBounds();
				CustomizationForm.CheckAndUpdate();
			}
		}
		bool needsUpdateRootBands = false;
		void CheckUpdateRootBands() {
			if(needsUpdateRootBands) {
				needsUpdateRootBands = false;
				UpdateRootBands();
			}
		}
		void CheckAutoFocusedNode() {
			if(IsLockUpdate || TreeListDisposing || IsDeserializing) return;
			if(autoFocusedNode != null && autoFocusedNode != FocusedNode) {
				BeginUpdate();
				try {
					ViewInfo.IsValid = false;
					ViewInfo.CalcViewInfo();
					SetFocusedNode(autoFocusedNode);
				}
				finally {
					autoFocusedNode = null;
					CancelUpdate();
				}
			}
		}
		void UpdateFocusedRowIndex() {
			if(IsLockUpdate || IsLoading || TreeListDisposing || IsDeserializing) return;
			SetFocusedRowIndexCore(GetVisibleIndexByNode(focusedNode)); 
		}
		bool calculatingLayout = false;
		protected virtual void UpdateLayout() {
			if(ViewInfo.IsValid || IsLockUpdate || calculatingLayout) return;
			calculatingLayout = true;
			try {
				FitColumns();
				ViewInfo.CalcViewInfo();
				UpdateScrollBars();
			}
			finally {
				calculatingLayout = false;
			}
			navigationHelper.UpdateButtons();
			if(!IsUpdateSize)
				RaiseLayoutUpdated();
		}
		internal void UpdateScrollBars() {			
			if(IsLockUpdate || lockScrollUpdate != 0) return;
			lockScrollUpdate++;
			try {
				bool prevHScrollVisible = scrollInfo.HScrollVisible;
				bool prevVScrollVisible = scrollInfo.VScrollVisible;
				UpdateHScrollBar();
				UpdateVScrollBar();
				UpdateHScrollBar();
				UpdateVScrollBar();
				if(prevHScrollVisible != scrollInfo.HScrollVisible ||
					prevVScrollVisible != scrollInfo.VScrollVisible) {
					ViewInfo.SummaryFooterInfo.NeedsRecalcRects = true;
					ViewInfo.IsValid = false;
					ClearAutoHeights();
					FitColumns();
					ViewInfo.CalcViewInfo();
					if(IsNeededVScrollBar != scrollInfo.VScrollVisible)
						UpdateVScrollBarInternal(true);
					ViewInfo.PaintAnimatedItems = false;
					Invalidate();
					scrollInfo.ClientRect = Rectangle.Empty;
				}
				scrollInfo.ClientRect = CalcScrollClientRect();
				scrollInfo.RightToLeft = IsRightToLeft;
				viewInfo.CalcBeyondScrollSquare();
			}
			finally {
				lockScrollUpdate--;
			}
		}
		void UpdateVScrollBarInternal(bool visible) {
			isNeededVScrollBarInternal = visible;
			ViewInfo.SummaryFooterInfo.NeedsRecalcRects = true;
			UpdateHScrollBar();
			UpdateVScrollBar();
			ClearAutoHeights();
			ViewInfo.IsValid = false;
			FitColumns();
			ViewInfo.CalcViewInfo();
			isNeededVScrollBarInternal = null;
		}
		protected virtual Rectangle CalcScrollClientRect() {
			Rectangle scrollRect = viewInfo.ViewRects.ScrollArea;
			int autoFilterRowHeigth = scrollInfo.IsOverlapScrollbar ? viewInfo.ViewRects.AutoFilterRow.Height + viewInfo.ViewRects.TopRowSeparator.Height + viewInfo.RC.hlw : 0;
			scrollRect.Y = viewInfo.ViewRects.ColumnPanel.Bottom + autoFilterRowHeigth;
			scrollRect.Height -= viewInfo.ViewRects.ColumnPanel.Height + autoFilterRowHeigth;
			return scrollRect;
		}
		protected virtual int CalcHScrollRange() {
			int res = ViewInfo.ViewRects.ColumnTotalWidth;
			if(ViewInfo.HasFixedLeft) res += FixedLineWidth - viewInfo.RC.hlw;
			if(ViewInfo.HasFixedRight) res += FixedLineWidth - viewInfo.RC.hlw;
			return res;
		}
		private void UpdateHScrollBar() {
			scrollInfo.HScrollVisible = IsNeededHScrollBar;
			if(scrollInfo.HScrollVisible) {
				int hScrollRange = CalcHScrollRange();
				scrollInfo.HScrollRange = (hScrollRange > ClientWidth ? hScrollRange : 0);
				scrollInfo.HScroll.SmallChange = HorzScrollStep;
				scrollInfo.HScroll.LargeChange = (viewInfo.ViewRects.Client.Width > 0 ? viewInfo.ViewRects.Client.Width : 0);
				scrollInfo.HScrollPos = LeftCoord;
				scrollInfo.CheckHScroll();
			}
			else if(HorzScrollVisibility != ScrollVisibility.Never) LeftCoord = 0; 
		}
		private void UpdateVScrollBar() {
			lockScrollEvents = true;
			try {
				UpdateVScrollBarCore();
			}
			finally {
				lockScrollEvents = false;
			}
		}
		private void UpdateVScrollBarCore() {
			scrollInfo.VScrollVisible = IsNeededVScrollBar;
			if(scrollInfo.VScrollVisible) {
				if(IsPixelScrolling) {
					scrollInfo.VScroll.LargeChange = Math.Max(0, ViewInfo.ViewRects.Rows.Height);
					scrollInfo.VScrollPos = TopVisibleNodePixel;
					scrollInfo.VScrollRange = ViewInfo.CalcTotalScrollableRowsHeight();
					scrollInfo.VScroll.SmallChange = 10;
				}
				else {
					int n = viewInfo.VisibleRowCount;
					if(n < 1) n = 1;
					scrollInfo.VScroll.LargeChange = n;
					scrollInfo.VScrollRange = Math.Max(0, RowCount - 1 + LowestRowFooterLines);
					scrollInfo.VScrollPos = TopVisibleNodeIndex;
					bool enabled = true;
					if(VertScrollVisibility == ScrollVisibility.Always) {
						RowsInfo rowsInfo = viewInfo.RowsInfo;
						int riCount = rowsInfo.Rows.Count;
						bool isLastVisible = riCount > 0 && riCount == RowCount && ViewInfo.IsRowVisible(((RowInfo)rowsInfo.Rows[riCount - 1]));
						enabled = TopVisibleNodeIndex != 0 || !isLastVisible;
					}
					scrollInfo.VScroll.Enabled = enabled;
				}
			}
		}
		private int LowestRowFooterLines {
			get {
				if(!OptionsView.ShowRowFooterSummary) return 0;
				RowsInfo rowsInfo = viewInfo.RowsInfo;
				int riCount = rowsInfo.Rows.Count;
				if(riCount > 0 && ((RowInfo)rowsInfo.Rows[riCount - 1]).VisibleIndex == RowCount - 1) {
					int numFooters = rowsInfo.GetInvisibleRowFooterLinesCount((RowInfo)rowsInfo.Rows[riCount - 1],
						viewInfo.ViewRects.Rows.Bottom);
					return numFooters * Convert.ToInt32((float)viewInfo.RC.GroupFooterHeight / (float)viewInfo.RowHeight);
				}
				return 0;
			}
		}
		void InternalSetTopVisibleNodeIndex(int newTopVisibleNodeIndex) {
			if(isHotTrackMode) return;
			if(newTopVisibleNodeIndex + viewInfo.VisibleRowCount >= RowCount + LowestRowFooterLines) {
				newTopVisibleNodeIndex = RowCount + LowestRowFooterLines - viewInfo.VisibleRowCount;
				rowAsFooterCount = 0;
			}
			ForceSetTopVisibleNodeIndex(newTopVisibleNodeIndex);
		}
		protected void ForceSetTopVisibleNodeIndex(int newTopVisibleNodeIndex) {
			if(newTopVisibleNodeIndex < 0) newTopVisibleNodeIndex = 0;
			if(newTopVisibleNodeIndex == TopVisibleNodeIndex) return;
			CloseEditor();
			SetTopVisibleNodeIndexCore(newTopVisibleNodeIndex + rowAsFooterCount);
			OnTopVisibleNodeIndexChanged();
		}
		protected internal virtual TreeListNode TopVisibleNode {
			get {
				if(this.topVisibleNode == null)
					SetTopVisibleNode(GetNodeByVisibleIndex(TopVisibleNodeIndex));
				return this.topVisibleNode;
			}
		}
		void SetTopVisibleNode(TreeListNode value) { this.topVisibleNode = value; }
		internal void NullTopVisibleNode() { SetTopVisibleNode(null); }
		internal void SetTopVisibleNodeIndexCore(int value) {
			int dy = value - TopVisibleNodeIndex;
			this.topVisibleNodeIndex = value;
			if(IsPixelScrolling)
				this.topVisibleNodePixel = ViewInfo.CalcPixelPositionByVisibleIndex(value);
			TreeListNode newTopVisible = null;
			if(IsInitialized && this.topVisibleNode != null) {
				if(dy == 1) newTopVisible = TreeListNodesIterator.GetNextVisible(this.topVisibleNode);
				if(dy == -1) newTopVisible = TreeListNodesIterator.GetPrevVisible(this.topVisibleNode);
			}
			SetTopVisibleNode(newTopVisible);
		}
		internal void SetFocusedRowIndexCore(int value) { this.focusedRowIndex = value; }
		void SetCurrentSetFocusCore(TreeListNode value) { this.currentSetFocus = value; }
		void ResetRowCount() { 
			this.rowCount = -1;
			ClearVisibleIndicesCache();
		}
		void ClearVisibleIndicesCache() {
			visibleIndexToNodeCache.Clear();
			nodeToVisibleIndexCache.Clear();
		}
		void ResetNodesCounters() {
			ResetRowCount();
			this.allNodesCount = -1;
		}
		private void OnTopVisibleNodeIndexChanged(bool raiseEvent = true) {
			if(IsIniting) return;
			ViewInfo.IsValid = false;
			if(!IsLockUpdate) {
				shouldFitColumns = false;
				UpdateVScrollBar();
			}
			ViewInfo.PaintAnimatedItems = false;
			Invalidate();
			if(raiseEvent)
				RaiseTopVisibleNodeIndexChanged();
		}
		private void CheckFocusedNodeSelected() {
			if(!OptionsSelection.MultiSelect) {
				Selection.InternalSet(FocusedNode);
				return;
			}
			if(Selection.Count == 0 && !IsCellSelect) {
				if(TreeListAutoFilterNode.IsAutoFilterNode(FocusedNode)) return;
				BeginUpdate();
				try {
					Selection.Set(FocusedNode);
				}
				finally {
					CancelUpdate();
				}
			}
		}
		internal bool isHotTrackMode = false;
		bool canFilterNode = false;
		bool isNodeCollectionChanged;
		internal void InternalSetFocusedRowIndex(int newFocusedRowIndex) {
			if(currentSetFocus != null) return;
			SetCurrentSetFocusCore(GetNewFocusedNodeByVisibleIndex(newFocusedRowIndex));
			CloseEditor();
			bool wasModified = IsFocusedNodeDataModified;
			isNodeCollectionChanged = false;
			bool canSetNewFocus = false;
			try {
				canSetNewFocus = CanFocusNode(FocusedNode, currentSetFocus);
			}
			finally {
				if(!canSetNewFocus) {
					SetCurrentSetFocusCore(null);
				}
			}
			if(!canSetNewFocus) return;
			int oldfocusedRowIndex = focusedRowIndex;
			TreeListNode oldFocusedNode = FocusedNode;
			if(isNodeCollectionChanged) {
				if(currentSetFocus != null) {
					int visibleIndex = GetVisibleIndexByNode(currentSetFocus);
					if(visibleIndex > -1 && newFocusedRowIndex != visibleIndex)
						newFocusedRowIndex = visibleIndex;
				}
				isNodeCollectionChanged = false;
			}
			SetFocusedRowIndexCore(newFocusedRowIndex);
			int oldTopVisibleNodeIndex = TopVisibleNodeIndex;
			int selectedCount = Selection.Count;
			SetFocusedNodeCore(currentSetFocus);
			SetCurrentSetFocusCore(null);
			CheckFocusedNodeSelected();
			if(TreeListAutoFilterNode.IsAutoFilterNode(FocusedNode)) {
				UpdateFocusedRowInfo(oldfocusedRowIndex, oldTopVisibleNodeIndex, selectedCount);
				RaiseAfterFocusNode(FocusedNode);
				RaiseFocusedNodeChanged(oldFocusedNode, FocusedNode);
				return;
			}
			if(oldfocusedRowIndex > 0)
				rowAsFooterCount = GetRowsAsFootersCount(oldfocusedRowIndex, newFocusedRowIndex);
			int visibleCount = ViewInfo.CalcVisibleNodeCount(TopVisibleNode);			
			UpdateScrollBars();
			if(!IsScrollAnimationInProgress) {
				if(TopVisibleNodeIndex + visibleCount <= FocusedRowIndex)
					TopVisibleNodeIndex = FocusedRowIndex - visibleCount  + 1;
				if(FocusedRowIndex < TopVisibleNodeIndex) {
					TopVisibleNodeIndex = FocusedRowIndex;
				}
				if(IsPixelScrolling)
					MakeFocusedRowVisible();
			}
			rowAsFooterCount = 0;
			UpdateFocusedRowInfo(oldfocusedRowIndex, oldTopVisibleNodeIndex, selectedCount);
			if(FocusedNode != null)
				Position = FocusedNode.Id;
			else if(!IsLookUpMode)
				Position = 0;
			navigationHelper.UpdateButtons();
			RaiseAfterFocusNode(FocusedNode);
			if(IsCellSelect)
				UpdateSelectionAnchor();
			RaiseFocusedNodeChanged(oldFocusedNode, FocusedNode);
			if(canFilterNode && oldFocusedNode != null) {
				if(!TreeListAutoFilterNode.IsAutoFilterNode(oldFocusedNode))
					UpdateNodeVisibility(oldFocusedNode);
				canFilterNode = false;
			}
			if(wasModified)
				DoSort(Nodes, true);
		}
		protected void MakeFocusedRowVisible() {
			int focusedRowPixelPostion = ViewInfo.CalcPixelPositionByVisibleIndex(FocusedRowIndex);
			if(focusedRowPixelPostion < TopVisibleNodePixel)
				TopVisibleNodePixel = focusedRowPixelPostion;
		}
		void UpdateFocusedNode() {
			Rectangle rc = viewInfo.SetFocusedRowActive(FocusedRowIndex, HasFocus);
			ViewInfo.PaintAnimatedItems = false;
			Invalidate(rc);
		}
		private void UpdateFocusedRowInfo(int oldfocusedRowIndex, int oldTopVisibleNodeIndex, int selectedCount) {
			if(IsLockUpdate) return;
			if(!(oldTopVisibleNodeIndex == TopVisibleNodeIndex && ViewInfo.IsValid)) return;
			RowInfo riNew = null, riOld = null;
			if(IsFilterRow(oldfocusedRowIndex)) {
				riOld = ViewInfo.AutoFilterRowInfo;
			}
			else {
				int k = oldfocusedRowIndex - TopVisibleNodeIndex;
				if(k < viewInfo.RowsInfo.Rows.Count && k > -1)
					riOld = (RowInfo)viewInfo.RowsInfo.Rows[k];
			}
			if(IsFilterRow(FocusedRowIndex)) {
				riNew = ViewInfo.AutoFilterRowInfo;
			}
			else {
				int k = FocusedRowIndex - TopVisibleNodeIndex;
				if(k > -1 && k < viewInfo.RowsInfo.Rows.Count)
					riNew = (RowInfo)viewInfo.RowsInfo.Rows[k];
			}
			if(riNew != null && riOld != null) {
				viewInfo.ChangeFocusedRow(riNew, riOld);
				ViewInfo.PaintAnimatedItems = false;
				Invalidate(riOld.Bounds);
				Invalidate(riNew.Bounds);
			}
			else {
				UpdateFocusedNode();
			}
			if(selectedCount > 1) {
				RefreshRowsInfo(false);   
			}
		}
		protected bool IsSorted { get; private set; }
		protected internal virtual bool DoSort(TreeListNodes nodes, bool makeFocusedNodeVisible) {
			if(lockSort != 0) return false;
			if(!IsSorted && SortedColumns.Count == 0) return false;
			OnStartSorting();
			try {
				SortNodes(nodes);
				ClearVisibleIndicesCache();
				ViewInfo.PixelScrollingInfo.Invalidate();
				IsSorted = SortedColumns.Count > 0;
				SetFocusedRowIndexCore(GetVisibleIndexByNode(focusedNode));
				if(makeFocusedNodeVisible && ViewInfo.IsValid) {
					NullTopVisibleNode();
					MakeNodeVisible(FocusedNode); 
				}
			}
			finally {
				OnEndSorting();
			}
			return true;
		}
		protected virtual void OnStartSorting() {
			NullTopVisibleNode();
			BeginUpdate();
			RaiseStartSorting();
		}
		protected virtual void OnEndSorting() {
			EndUpdate();
			RaiseEndSorting();
		}
		protected virtual void SortNodes(TreeListNodes nodes) {
			foreach(TreeListNode node in nodes) {
				if(node.HasChildren)
					SortNodes(node.Nodes);
			}
			nodes.SortNodes(nodesComparer);
		}
		protected internal void RefreshRowsInfo() {
			RefreshRowsInfo(true);
		}
		protected internal void RefreshRowsInfo(bool recalcRowsInfo, bool refreshCellState = false) {
			if(IsLockUpdate || calculatingLayout) return;
			if(recalcRowsInfo) {
				RecreateRowsInfo();
			}
			else {
				viewInfo.UpdateRowsInfo(refreshCellState);
			}
			Rectangle rect = viewInfo.ViewRects.Rows;
			if(ViewInfo.AutoFilterRowInfo != null)
				rect = Rectangle.Union(viewInfo.AutoFilterRowInfo.Bounds, rect);
			ViewInfo.PaintAnimatedItems = false;
			Invalidate(rect);
		}
		void RecreateRowsInfo() {
			try {
				calculatingLayout = true;
				viewInfo.RowsInfo.Clear();
				if(viewInfo.AutoFilterRowInfo != null)
					viewInfo.UpdateRowInfo(viewInfo.AutoFilterRowInfo);
				Rectangle rect = ViewInfo.BriefCalcRowsInfo();
				if(rect.Bottom < ViewInfo.ViewRects.Rows.Bottom) {
					ViewInfo.ViewRects.EmptyRows = new Rectangle(ViewInfo.ViewRects.Rows.Left, rect.Bottom,
						ViewInfo.ViewRects.Rows.Width, ViewInfo.ViewRects.Rows.Bottom - rect.Bottom);
					ViewInfo.ViewRects.Rows.Height -= ViewInfo.ViewRects.EmptyRows.Height;
				}
				ViewInfo.CheckIncreaseVisibleRows(rect.Width);
			}
			finally {
				calculatingLayout = false;
			}
		}
		protected void RefreshHeadersInfo(Rectangle bounds){
			if(IsLockUpdate) return;
			viewInfo.ColumnsInfo.Columns.Clear();
			viewInfo.CalcColumnsInfo();
			viewInfo.BandsInfo.Clear();
			viewInfo.CalcBandsInfo();
			ViewInfo.PaintAnimatedItems = false;
			Invalidate(bounds);
		}
		protected internal void RefreshColumnsInfo() {
			RefreshHeadersInfo(viewInfo.ViewRects.ActualColumnPanel);
		}
		[Browsable(false)]
		public void ClearFocusedColumn() {
			HideEditor();
			this.focusedCellIndex = -1;
			LayoutChanged();
		}
		private void InternalSetFocusedCellIndex(int newFocusedCellIndex, int delta, bool forceSet = false) {
			CloseEditor();
			int oldFocusedCellIndex = focusedCellIndex;
			if(!forceSet)
				newFocusedCellIndex = GetNearestCanFocusedColumnIndex(newFocusedCellIndex, delta);
			focusedCellIndex = newFocusedCellIndex;
			if(oldFocusedCellIndex != newFocusedCellIndex) {
				TreeListColumn column = VisibleColumns[newFocusedCellIndex];
				if(column != null) {
					bool needsRefreshRows = MakeColumnVisible(column) || !ViewInfo.IsValid || oldFocusedCellIndex < 0;
					if(needsRefreshRows) RefreshRowsInfo();
					else {
						viewInfo.ChangeFocusedCell(FocusedRow, newFocusedCellIndex, oldFocusedCellIndex);
						ViewInfo.PaintAnimatedItems = false;
						InvalidateFocusedRowCells();
					}
				}
				if(IsCellSelect)
					UpdateSelectionAnchor();
				RaiseFocusedColumnChanged(newFocusedCellIndex, oldFocusedCellIndex);
			}
		}
		internal void UpdateSelectionAnchor() {
			if(lockSelection != 0) return;
			StateData data = Handler.StateData;
			data.SelectionAnchor = data.SelectionOldCell = new TreeListCell(FocusedNode, FocusedColumn);
			data.SelectionBounds = Rectangle.Empty;
		}
		private bool MakeColumnVisible(TreeListColumn column) {
			if(column == null) return false;
			if(column.Fixed != FixedStyle.None) return false;
			int colLeft = ViewInfo.GetColumnLeftCoord(column),
				colRight, maxRight, minLeft = 0;
			if(ViewInfo.HasFixedLeft) {
				minLeft = (ViewInfo.ViewRects.FixedLeft.Right - ViewInfo.ViewRects.IndicatorWidth);
				colLeft -= (minLeft - FixedLineWidth);
			}
			else {
				minLeft = ViewInfo.ViewRects.Rows.Left;
			}
			maxRight = ViewInfo.ViewRects.Rows.Right - minLeft - ViewInfo.ViewRects.IndicatorWidth;
			colRight = colLeft + column.VisibleWidth;
			if(column.VisibleIndex == 0 && FocusedNode != null && !ViewInfo.HasFixedLeft) {
				if(colRight > maxRight) {
					const int minColumnVisibleWidth = 40;
					int minWidth = Math.Min(minColumnVisibleWidth, maxRight - minLeft);
					int delta = maxRight - ViewInfo.GetDataBoundsLeftLocation(FocusedNode);
					if(delta < minWidth) {
						LeftCoord = delta > 0 ? minWidth : minWidth - delta;
						return true;
					}
				}
			}
			if(colLeft < LeftCoord) {
				LeftCoord = colLeft;
				return true;
			}
			if(ViewInfo.HasFixedRight)
				maxRight = ViewInfo.ViewRects.FixedRight.Left - ViewInfo.ViewRects.IndicatorWidth - minLeft;
			if(colRight > LeftCoord + maxRight) {
				if(colRight - colLeft >= maxRight)
					LeftCoord = colLeft;
				else
					LeftCoord = colLeft - maxRight + column.VisibleWidth;
				return true;
			}
			return false;
		}
		internal void SetFocusedNodeCandidate(TreeListNode node) { focusedNodeCandidate = node; }
		TreeListNode focusedNodeCandidate = null;
		internal void MoveFocusedRow(int delta, KeyEventArgs e) {
			if(delta == 1)
				focusedNodeCandidate = TreeListNodesIterator.GetNextVisible(FocusedNode);
			else if(delta == -1) {
				if(TreeListAutoFilterNode.IsAutoFilterNode(FocusedNode)) return;
				focusedNodeCandidate = TreeListNodesIterator.GetPrevVisible(FocusedNode);
				if(focusedNodeCandidate == null) {
					if(e.Control && OptionsView.ShowAutoFilterRow && GetVisibleIndexByNode(FocusedNode) == 0) {
						FocusedRowIndex = TreeList.AutoFilterNodeId;
						return;
					}
				}
			}
			try {
				int newIndex = FocusedRowIndex + delta;
				if(IsPixelScrolling && AllowAnimatedScrolling)
					AnimateKeyboardNavigation(e, FocusedRowIndex, newIndex);
				FocusedRowIndex = Math.Max(0, newIndex);
			}
			finally {
				focusedNodeCandidate = null;
			}
		}
		protected virtual void AnimateKeyboardNavigation(KeyEventArgs e, int oldIndex, int newIndex) {
			if(!CanAnimateKeyboardNavigation()) {
				Handler.CancelAnimatedScroll();
				return;
			}
			if(IsScrollAnimationInProgress)
				Handler.CancelAnimatedScroll();
			if(e.KeyCode == Keys.PageDown) {
				RowInfo ri = ViewInfo.RowsInfo[GetNodeByVisibleIndex(oldIndex)];
				int oldPosition = ViewInfo.CalcPixelPositionByVisibleIndex(oldIndex + (ViewInfo.IsRowVisible(ri) ? 1 : 0));
				int distance = Math.Max(0, oldPosition - TopVisibleNodePixel);
				Handler.AnimateScroll(distance);
			}
			if(e.KeyCode == Keys.PageUp) {
				int newPosition = ViewInfo.CalcPixelPositionByVisibleIndex(newIndex);
				int distance = newPosition - TopVisibleNodePixel;
				Handler.AnimateScroll(distance);
			}
			if(e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) {
				if(!ViewInfo.IsRowVisible(newIndex)) {
					int oldPosition = ViewInfo.CalcPixelPositionByVisibleIndex(oldIndex);
					int newPosition = ViewInfo.CalcPixelPositionByVisibleIndex(newIndex);
					int distance = newPosition - oldPosition;
					if(e.KeyCode == Keys.Down && distance < 0)
						return;
					if(e.KeyCode == Keys.Up)
						distance = newPosition - TopVisibleNodePixel;
					if(Math.Abs(distance) > ViewInfo.ViewRects.Rows.Height || !ViewInfo.IsRowVisible(oldIndex))
						return;
					Handler.AnimateScroll(distance);
				}
			}
		}
		TreeListNode GetNewFocusedNodeByVisibleIndex(int newFocusedRowIndex) {
			if(IsFilterRow(newFocusedRowIndex) && viewInfo.AutoFilterRowInfo != null)
				return viewInfo.AutoFilterRowInfo.Node;
			if(focusedNodeCandidate != null) return focusedNodeCandidate;
			return GetNodeByVisibleIndex(newFocusedRowIndex);
		}
		private void ClearInternalSettings() { ClearInternalSettings(true); }
		private void ClearInternalSettings(bool resetFocusCoords) {
			ResetNodesCounters();
			ClearDateFilterCache();
			rowAsFooterCount = 0;
			if(resetFocusCoords) {
				focusedCellIndex = -1;
				leftCoord = 0;
				SetTopVisibleNodeIndexCore(0);
				SetFocusedRowIndexCore(-1);
				SetCurrentSetFocusCore(null);
			}
			ViewInfo.SummaryFooterInfo.NeedsRecalcAll = true;
			ViewInfo.IsValid = false;
			this.isFocusedNodeDataModified = false;
			SetFocusedNodeCore(null);
			Selection.Clear();
			ClearAutoHeights();
			ContainerHelper.DeactivateEditor(false);
			Handler.SetControlState(TreeListState.Regular);
		}
		public void ClearSorting() {
			if(SortedColumnCount == 0) return;
			BeginUpdate();
			try {
				RecalcSortColumns(null, true);
			}
			finally {
				EndUpdate();
			}
		}
		internal void RefreshSortColumns() {
			ArrayList cols = new ArrayList();
			cols.AddRange(Columns);
			cols.Sort(new ColumnSortIndexComparer());
			SortedColumns.Clear();
			foreach(TreeListColumn col in cols) {
				if(col.SortIndex > -1 || col.SortOrder != SortOrder.None) {
					SortedColumns.Add(col);
					col.sortIndex = SortedColumns.IndexOf(col);
					col.sortOrder = (col.SortOrder == SortOrder.None ? SortOrder.Ascending : col.SortOrder);
				}
			}
		}
		protected internal virtual void RecalcSortColumns(TreeListColumn column, bool clearSortedColumns) {
			if(clearSortedColumns) {
				SortedColumns.Clear();
			}
			if(column != null) {
				int index = SortedColumns.IndexOf(column);
				if(index < 0) {
					if(column.SortOrder != SortOrder.None)
						SortedColumns.Add(column);
				}
				else
					if(column.SortOrder == SortOrder.None)
						SortedColumns.RemoveAt(index);
			}
			UpdateColumnsSortIndices();
			DoSort(Nodes, true);
		}
		internal void SetSortedColumnIndex(TreeListColumn column, int newIndex) {
			if(newIndex < 0) newIndex = -1;
			if(newIndex > SortedColumnCount) newIndex = SortedColumnCount;
			int prevIndex = column.SortIndex;
			if(prevIndex == newIndex) return;
			if(newIndex == -1) {
				SortedColumns.RemoveAt(prevIndex);
				column.sortOrder = SortOrder.None;
			}
			else {
				if(prevIndex == -1) {
					SortedColumns.Insert(newIndex, column);
					column.sortOrder = SortOrder.Ascending;
				}
				else {
					SortedColumns.RemoveAt(prevIndex);
					if(newIndex > SortedColumnCount) newIndex--;
					SortedColumns.Insert(newIndex, column);
				}
			}
			UpdateColumnsSortIndices();
			DoSort(Nodes, true);
		}
		protected void UpdateColumnsSortIndices() {
			foreach(TreeListColumn col in Columns) {
				if(SortedColumns.IndexOf(col) < 0) {
					col.sortOrder = SortOrder.None;
					col.sortIndex = -1;
				}
				else
					col.sortIndex = SortedColumns.IndexOf(col);
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never), System.Obsolete("You should use the 'CalcHitInfo' method instead of 'GetHitInfo'")]
		public TreeListHitInfo GetHitInfo(Point pt) {
			return CalcHitInfo(pt);
		}
		public TreeListHitInfo CalcHitInfo(Point pt) {
			TreeListHitTest ht = Handler.GetHitTest(pt);
			return new TreeListHitInfo(ht);
		}
		internal bool IsComponentSelected(object component) { return Handler.IsComponentSelected(component); }
		void ClearAutoHeights() { ClearAutoHeights(false); }
		void ClearAutoHeights(bool checkOddEven) {
			bool clear = (checkOddEven ? OptionsView.EnableAppearanceEvenRow || OptionsView.EnableAppearanceOddRow : true);
			if(clear)
				autoHeights.Clear();
		}
		protected internal virtual bool CanIncrementalSearch(TreeListColumn column) {
			DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo bev = CreateColumnEditViewInfo(column);
			if(!bev.IsSupportIncrementalSearch) return false;
			return OptionsBehavior.AllowIncrementalSearch && column.AllowIncrementalSearch;
		}
		protected internal virtual DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo CreateColumnEditViewInfo(TreeListColumn column) {
			DevExpress.XtraEditors.Repository.RepositoryItem ritem = GetNodeCellRepositoryItem(column);
			if(ritem != null) {
				DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo bev = ritem.CreateViewInfo();
				return bev;
			}
			return null;
		}
		protected internal virtual DevExpress.XtraEditors.Repository.RepositoryItem GetNodeCellRepositoryItem(TreeListColumn column) {
			if(column == null) return null;
			DevExpress.XtraEditors.Repository.RepositoryItem ce = column.ColumnEdit;
			if(ce == null) ce = GetColumnDefaultRepositoryItem(column);
			return ce;
		}
		protected internal virtual DevExpress.XtraEditors.Repository.RepositoryItem GetColumnDefaultRepositoryItem(TreeListColumn column) {
			var columnType = (column == null) ? typeof(string) : column.ColumnType;
			var columnAnnotationAttributes = (column == null) ? null : column.ColumnAnnotationAttributes;
			return ContainerHelper.DefaultRepository.GetRepositoryItem(columnType, columnAnnotationAttributes);
		}
		protected internal virtual RepositoryItem GetColumnDefaultRepositoryItemForEditing(TreeListColumn column, RepositoryItem editor) {
			var columnType = (column == null) ? typeof(string) : column.ColumnType;
			var columnAnnotationAttributes = (column == null) ? null : column.ColumnAnnotationAttributes;
			return ContainerHelper.DefaultRepository.GetRepositoryItemForEditing(editor, columnType, columnAnnotationAttributes);
		}
		#endregion
		#region Pixel Scrolling
		protected internal virtual bool IsPixelScrolling { 
			get {
				if(OptionsBehavior.AllowPixelScrolling == DefaultBoolean.Default && WindowsFormsSettings.IsAllowPixelScrolling)
					return true;
				return OptionsBehavior.AllowPixelScrolling == DefaultBoolean.True; 
			} 
		}
		protected internal bool IsScrollAnimationInProgress { get { return Handler.IsScrollAnimationInProgress; } }
		protected internal virtual bool AllowAnimatedScrolling { get { return true; } }
		protected internal bool ShowEditorAfterAnimatedScroll { get; set; }
		DateTime lastScroll = DateTime.MinValue;
		bool CanAnimateKeyboardNavigation() {
			DateTime current = lastScroll;
			lastScroll = DateTime.Now;
			return DateTime.Now.Subtract(current).TotalMilliseconds > 300;
		}
		protected virtual void InternalSetTopVisibleNodePixel(int value) {
			this.topVisibleNodePixel = CheckTopVisibleNodePixelValue(value);
			int oldTopVisibleNodeIndex = TopVisibleNodeIndex;
			this.topVisibleNodeIndex = ViewInfo.CalcVisibleIndexByPixelPosition(this.topVisibleNodePixel);
			if(this.topVisibleNodeIndex < 0)
				this.topVisibleNodeIndex = 0;
			bool changed = oldTopVisibleNodeIndex != TopVisibleNodeIndex;
			if(changed)
				NullTopVisibleNode();
			CloseEditor();
			OnTopVisibleNodeIndexChanged(changed);
		}
		protected internal int CheckTopVisibleNodePixelValue(int value) {
			int visibleRowsHeight = ViewInfo.ViewRects.Rows.Height;
			int totalRowsHeight = ViewInfo.CalcTotalScrollableRowsHeight();
			int scrollableHeight = Math.Max(0, totalRowsHeight - visibleRowsHeight);
			if(value > scrollableHeight)
				value = scrollableHeight;
			return value;
		}
		#endregion
		#region Repaint
		internal void InvalidateDragArrow() {
			ViewInfo.PaintAnimatedItems = false;
			Size size = new System.Drawing.Size(Painter.NodeDragImages.ImageSize.Width, viewInfo.ViewRects.Rows.Height);
			int x = IsRightToLeft ? viewInfo.ViewRects.Rows.Right - viewInfo.ViewRects.IndicatorWidth - size.Width : viewInfo.ViewRects.Rows.Left + viewInfo.ViewRects.IndicatorWidth;
			Invalidate(new Rectangle(x, viewInfo.ViewRects.Rows.Top, size.Width, size.Height));
		}
		protected void InvalidateFocusedRowCells() {
			if(FocusedRow != null) {
				ViewInfo.PaintAnimatedItems = false;
				Invalidate(FocusedRow.DataBounds);
			}
		}
		public virtual void InvalidateColumnPanel() {
			if(OptionsView.ShowColumns) {
				ViewInfo.PaintAnimatedItems = false;
				Invalidate(viewInfo.ViewRects.ActualColumnPanel);
			}
		}
		public virtual void InvalidateBandPanel() {
			ViewInfo.PaintAnimatedItems = false;
			Invalidate(viewInfo.ViewRects.ActualBandPanel);
		}
		public virtual void InvalidateColumnHeader(TreeListColumn column) {
			ColumnInfo ci = viewInfo.ColumnsInfo[column];
			if(ci != null) {
				ViewInfo.PaintAnimatedItems = false;
				Invalidate(ci.Bounds);
			}
		}
		public virtual void InvalidateBand(TreeListBand band) {
			BandInfo bi = viewInfo.BandsInfo[band];
			if(bi != null) {
				ViewInfo.PaintAnimatedItems = false;
				Invalidate(bi.Bounds);
			}
		}
		public virtual void InvalidateNodes() {
			ViewInfo.PaintAnimatedItems = false;
			Invalidate(viewInfo.ViewRects.Rows);
		}
		public virtual void InvalidateNode(TreeListNode node) {
			RowInfo ri = viewInfo.RowsInfo[node];
			if(ri != null) {
				ViewInfo.PaintAnimatedItems = false;
				Invalidate(ri.Bounds);
			}
		}
		public virtual void InvalidateCell(TreeListNode node, TreeListColumn column) {
			if(node == FocusedNode && column == FocusedColumn && ActiveEditor != null) {
				ActiveEditor.Refresh();
				return;
			}
			RowInfo ri = viewInfo.RowsInfo[node];
			if(ri == null) return;
			CellInfo cell = ri[column];
			if(cell != null) {
				ViewInfo.PaintAnimatedItems = false;
				Invalidate(cell.Bounds);
			}
		}
		public virtual void RefreshCell(TreeListNode node, TreeListColumn column) {
			if(node == null || column == null) return;
			RefreshVirtualCellCore(node, column);
			RefreshCellCore(node, column);
		}
		protected virtual void RefreshCellCore(TreeListNode node, TreeListColumn column) {
			if(node == FocusedNode && column == FocusedColumn && ActiveEditor != null) {
				ActiveEditor.Refresh();
				return;
			}
			if(!ViewInfo.IsValid)
				return;
			RowInfo ri = viewInfo.RowsInfo[node];
			if(ri == null) return;
			CellInfo cell = ri[column];
			if(cell != null) {
				ViewInfo.UpdateCellInfo(cell, node, true);
				ViewInfo.PaintAnimatedItems = false;
				Invalidate(cell.Bounds);
			}
		}
		protected virtual void RefreshVirtualCellCore(TreeListNode node, TreeListColumn column) {
			TreeListVirtualData data = Data as TreeListVirtualData;
			if(data == null) return;
			data.RefreshCellData(node.Id, column);
		}
		public virtual void RefreshNode(TreeListNode node) {
			if(node == null) return;
			RefreshVirtualNodeCore(node);
			RefreshNodeCore(node);
		}
		protected virtual void RefreshNodeCore(TreeListNode node) {
			if(!ViewInfo.IsValid) return;
			RowInfo ri = viewInfo.RowsInfo[node];
			if(ri != null) {
				ViewInfo.UpdateRowInfo(ri, true);
				ViewInfo.PaintAnimatedItems = false;
				Invalidate(ri.Bounds);
			}
		}
		protected virtual void RefreshVirtualNodeCore(TreeListNode node) {
			TreeListVirtualData data = Data as TreeListVirtualData;
			if(data == null) return;
			data.RefreshRowData(node.Id);
		}
		public virtual void InvalidateSummaryFooterPanel() {
			if(OptionsView.ShowSummaryFooter) {
				ViewInfo.PaintAnimatedItems = false;
				Invalidate(viewInfo.ViewRects.Footer);
			}
		}
		#endregion
		#region Unbound Mode
		public virtual void BeginUnboundLoad() {
			BeginSort();
			LockReloadNodes();
			lockUnbound++;
		}
		public virtual void EndUnboundLoad() {
			lockUnbound--;
			CorrectIndentWidth();
			UnlockReloadNodes();
			EndSort();
			FilterNodes();
			CheckFocusedNode();
		}
		#endregion
		#region Events functions
		bool lockScrollEvents = false;
		protected virtual void OnHScroll(object sender, EventArgs e) {
			if(lockScrollEvents) return;
			try {
				ContainerHelper.BeginAllowHideException();
				LeftCoord = scrollInfo.HScrollPos;
			}
			catch(HideException) {
				UpdateHScrollBar();
			}
			finally {
				ContainerHelper.EndAllowHideException();
			}
		}
		protected virtual void OnVScroll(object sender, EventArgs e) {
			if(lockScrollEvents) return;
			try {
				ContainerHelper.BeginAllowHideException();
				if(IsPixelScrolling)
					TopVisibleNodePixel = scrollInfo.VScrollPos;
				else
					TopVisibleNodeIndex = scrollInfo.VScrollPos;
			}
			catch(HideException) {
				UpdateVScrollBar();
			}
			finally {
				ContainerHelper.EndAllowHideException();
			}
		}
		void OnVertScroll(object sender, ScrollEventArgs e) {
			if(IsPixelScrolling)
				Handler.CancelAnimatedScroll();
		}
		#region internal
		protected void DoLeftCoordChanged() {
			CloseEditor();
			if(IsLockUpdate) return;
			viewInfo.IsValid = false;
			viewInfo.SummaryFooterInfo.NeedsRecalcAll = true;
			viewInfo.SummaryFooterInfo.NeedsRecalcRects = true;
			viewInfo.PaintAnimatedItems = false;
			shouldFitColumns = false;
			Invalidate();
			RaiseLeftCoordChanged();
		}
		protected internal virtual bool OnBeforeChangeExpanded(TreeListNode node, bool newVal) {
			bool canChange = true;
			if(newVal) RaiseBeforeExpand(node, ref canChange);
			else RaiseBeforeCollapse(node, ref canChange);
			return canChange;
		}
		protected internal virtual void OnAfterChangeExpanded(TreeListNode node, int prevIndex) {
			TreeListNode focus = FocusedNode;
			bool raiseEv = OnChangeNodeState(prevIndex, node.Expanded);
			if(node.Expanded) {
				RaiseAfterExpand(node);
				node.HasChildren = node.Nodes.Count > 0;
			}
			else {
				RaiseAfterCollapse(node);
				if(focus == null) return;
				if(focus.HasAsParent(node)) {
					if(CanFocusNode(focus, node)) {
						Position = node.Id;
						FocusedNode = node;
						if(focus != FocusedNode) {
							Selection.InternalRemove(focus);
							if(FocusedNode != null)
								Selection.Add(FocusedNode);
						}
					}
					else {
						SetFocusedNodeCore(null);
						SetFocusedRowIndexCore(-1);
						Selection.InternalClear();
						RefreshRowsInfo();
						raiseEv = true;
					}
					if(raiseEv) RaiseFocusedNodeChanged(focus, focusedNode);
				}
			}
		}
		private bool OnChangeNodeState(int absoluteIndex, bool expand) {
			if(absoluteIndex == -1) {
				ResetRowCount();
				return false;
			}
			int oldRowCount = RowCount;
			ResetRowCount();
			bool result = false;
			int delta = oldRowCount - RowCount;
			BeginUpdate();
			try {
				CloseEditor();
				if(expand) {
					if(FocusedRowIndex > absoluteIndex)
						SetFocusedRowIndexCore(focusedRowIndex - delta);
				}
				else {
					if(FocusedRowIndex == absoluteIndex) {
						return result;
					}
					if(FocusedRowIndex > absoluteIndex) {
						SetFocusedRowIndexCore(Math.Max(absoluteIndex, FocusedRowIndex - delta));
						SetFocusedNodeCore(GetNodeByVisibleIndex(focusedRowIndex));
						if(RowCount - ViewInfo.CalcVisibleNodeCount(TopVisibleNode) <= TopVisibleNodeIndex)
							TopVisibleNodeIndex -= delta;
						result = true;
						if(!OptionsSelection.MultiSelect)
							Selection.InternalSet(focusedNode);
					}
					else if(TopVisibleNodeIndex > 0) {
						delta = RowCount - ViewInfo.CalcVisibleNodeCount(TopVisibleNode);
						if(delta <= TopVisibleNodeIndex)
							TopVisibleNodeIndex = delta;
					}
				}
				if(!expand && UpdateTopVisibleNodeIndexOnCollapse) {
					RecreateRowsInfo();
					CheckIncreaseVisibleRows();
				}
			}
			finally {
				EndUpdate();
				ViewInfo.PixelScrollingInfo.Invalidate();
			}
			return result;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public bool UpdateTopVisibleNodeIndexOnCollapse { get; set; }
		protected virtual void OnOptionsViewChanged(object sender, BaseOptionChangedEventArgs e) {
			if(e.Name == TreeListOptionsView.ShowIndicatorName)
				viewInfo.CalcIndicatorWidth();
			viewInfo.RC.NeedsRestore = true;
			viewInfo.SummaryFooterInfo.NeedsRecalcRects = (e.Name == TreeListOptionsView.ShowSummaryFooterName ||
				e.Name == TreeListOptionsView.AutoWidthName ||
				e.Name == TreeListOptionsView.ShowIndicatorName);
			if(e.Name == TreeListOptionsView.EnableAppearanceEvenRowName || e.Name == TreeListOptionsView.EnableAppearanceOddRowName)
				ClearAutoHeights();
			OnProperyChanged();
			InvalidatePixelScrollingInfo();
			LayoutChanged();
		}
		internal void InvalidatePixelScrollingInfo() {
			viewInfo.PixelScrollingInfo.Invalidate();
			if(IsPixelScrolling && !IsLockUpdate)
				CheckTopVisibleNodePixel();
		}
		protected virtual void OnOptionsFilterChanged(object sender, BaseOptionChangedEventArgs e) {
			if(e.Name == TreeListOptionsFilter.FilterModeName)
				ClearAndUpdateFilter();
			if(e.Name == TreeListOptionsFilter.AllowColumnMRUFilterListName
				|| e.Name == TreeListOptionsFilter.AllowMRUFilterListName
				|| e.Name == TreeListOptionsFilter.AllowColumnMRUFilterListName
				|| e.Name == TreeListOptionsFilter.AllowFilterEditorName)
				LayoutChanged();
			OnProperyChanged();
		}
		protected virtual void OnOptionsFindChanged(object sender, BaseOptionChangedEventArgs e) {
			if(IsAttachedToSearchControl) return;
			if(e.Name == TreeListOptionsFind.AllowFindPanelName || e.Name == TreeListOptionsFind.AlwaysVisibleName) {
				if(OptionsFind.AllowFindPanel && OptionsFind.AlwaysVisible)
					ShowFindPanel();
				else
					HideFindPanel();
			}
			if(e.Name == TreeListOptionsFind.ShowClearButtonName || e.Name == TreeListOptionsFind.ShowCloseButtonName || e.Name == TreeListOptionsFind.ShowFindButtonName || e.Name == TreeListOptionsFind.FindNullPromptName) {
				if(FindPanel != null)
					FindPanel.Owner.InitButtons();
			}
			if(e.Name == TreeListOptionsFind.FindModeName) {
				if(FindPanel != null)
					UpdateFindPanelFindMode();
			}
			if(e.Name == TreeListOptionsFind.HighlightFindResultsName)
				LayoutChanged();
			OnProperyChanged();
		}
		protected virtual void OnOptionsBehaviorChanged(object sender, BaseOptionChangedEventArgs e) {
			if(e.Name == TreeListOptionsBehavior.AutoNodeHeightName || e.Name == TreeListOptionsBehavior.AllowIndeterminateCheckStateName)
				LayoutChanged();
			if(e.Name == TreeListOptionsBehavior.AllowPixelScrollingName) {
				CheckTopVisibleNodePixel();
			}
			if(e.Name == TreeListOptionsBehavior.SmartMouseHoverName && IsHandleCreated)
				mouseIn = RectangleToScreen(Bounds).Contains(Cursor.Position);
			if(e.Name == TreeListOptionsBehavior.EnableFilteringName) {
				if((bool)e.NewValue)
					FilterNodes();
				else
					ClearFilteringInternal();
			}
			OnProperyChanged();
		}
		protected virtual void OnOptionsNavigationChanged(object sender, BaseOptionChangedEventArgs e) {
			OnProperyChanged();
		}
		protected virtual void OnOptionsDragAndDropChanged(object sender, BaseOptionChangedEventArgs e) {
			if(e.Name == TreeListOptionsDragAndDrop.DragNodesModeName)
				AllowDrop = OptionsDragAndDrop.DragNodesMode != XtraTreeList.DragNodesMode.None;
		}
		void CheckTopVisibleNodePixel() {
			Handler.CancelAnimatedScroll();
			if(IsPixelScrolling)
				TopVisibleNodePixel = ViewInfo.CalcPixelPositionByVisibleIndex(TopVisibleNodeIndex);
			else
				TopVisibleNodeIndex = ViewInfo.CalcVisibleIndexByPixelPosition(TopVisibleNodePixel);
		}
		protected virtual void OnOptionsPrintChanged(object sender, BaseOptionChangedEventArgs e) {
			if(e.Name == TreeListOptionsPrint.UsePrintStylesName)
				printer.PrintLayoutChanged();
			if(e.Name == TreeListOptionsPrint.AutoRowHeightName)
				printer.PrintLayoutChanged();
		}
		protected virtual void OnOptionsSelectionChanged(object sender, BaseOptionChangedEventArgs e) {
			if(e.Name == TreeListOptionsSelection.MultiSelectName)
				Selection.Set(FocusedNode);
			LayoutChanged();
		}
		protected bool AllowFilterNodes { get { return OptionsBehavior.EnableFiltering && !IsUnboundLoad && !IsLoading && !IsDeserializing; } }
		protected bool IsFilterApplied { get { return (ActiveFilterEnabled || IsFindFilterActive) && (FilterHelper.IsReady || this.Events[filterNode] != null || FilterConditions.Count > 0); } }
		protected internal virtual bool DoFilterNode(TreeListNode node) {
			if(!AllowFilterNodes) return true;
			bool result = FilterNodeOnFilterConditions(node);
			if(ActiveFilterEnabled || IsFindFilterActive)
				result &= FilterNodeOnFilterCriteria(node);
			FilterNodeEventArgs args = new FilterNodeEventArgs(node, result);
			RaiseFilterNodeEvent(args);
			if(!args.Handled)
				node.Visible = result;
			node.isExpandedSetInternally = false;
			if(node.IsVisible && IsFilterApplied && OptionsBehavior.ExpandNodesOnFiltering)
				UpdateParentExpandStateOnFiltering(node);
			return !node.Visible;
		}
		protected virtual void UpdateParentExpandStateOnFiltering(TreeListNode node) {
			BeginUpdate();
			try {
				TreeListNode parent = node.ParentNode;
				while(parent != null && !parent.isExpandedSetInternally) {
					parent.Expanded = true;
					parent.isExpandedSetInternally = parent.Expanded;
					parent = parent.ParentNode;
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal bool FilterNodeOnFilterCriteria(TreeListNode node) {
			if(FilterHelper.IsReady)
				return FilterHelper.Fit(node);
			return true;
		}
		protected internal bool FilterNodeOnFilterConditions(TreeListNode node) {
			bool visible = true;
			for(int i = 0; i < FilterConditions.Count; i++) {
				FilterCondition condition = FilterConditions[i];
				CheckFilterCondition(condition, node, ref visible);
			}
			return visible;
		}
		protected virtual void CheckFilterCondition(FilterCondition condition, TreeListNode node, ref bool visible) {
			if(condition.Column == null) return;
			object value = node.GetValue(condition.Column);
			if(condition.CheckValue(value)) visible &= condition.Visible;
		}
		internal void NodeCollectionChanged(TreeListNodes nodes, TreeListNode node, NodeChangeTypeEnum changeType) {
			if(changeType == NodeChangeTypeEnum.Add || changeType == NodeChangeTypeEnum.Remove) {
				ClearDateFilterCache();
				isNodeCollectionChanged = true;
			}
			if(changeType == NodeChangeTypeEnum.Expanded) 
				isNodeCollectionChanged = true;
			if(changeType == NodeChangeTypeEnum.Add && !IsUnboundMode && !loadingNodes && !IsLoading) DoFilterNode(node);
			if(changeType == NodeChangeTypeEnum.Remove && this.topVisibleNode == node)
				NullTopVisibleNode();
			LayoutChanged();
			if(node == null) {
				foreach(TreeListNode childNode in nodes)
					InternalNodeChanged(childNode, changeType);
			}
			else InternalNodeChanged(node, changeType);
		}
		internal void ColumnOptionsChanged(TreeListColumn column, BaseOptionChangedEventArgs e) {
			if(e.Name == TreeListOptionsColumn.AllowFocusName && !column.OptionsColumn.AllowFocus
				&& column == FocusedColumn) FocusedCellIndex++;
			if(e.Name == TreeListOptionsColumn.ShowInCustomizationFormName) {
				RefreshCustomizationForm();
			}
			if(e.Name == TreeListOptionsColumnFilter.FilterPopupModeName)
				ClearDateFilterCache();
			if(e.Name == TreeListOptionsColumnFilter.AllowFilterName)
				UpdateLayout();
			if(column != null)
				RaiseColumnChanged(column);
		}
		internal void InternalColumnRemoving(TreeListColumn column) {
			if(column == null || !HasBands) return;
			RemoveColumnFromBandColumns(column);
		}
		protected internal virtual void InternalColumnChanged(TreeListColumn column) {
			OnColumnChanged(column);
			RaiseColumnChanged(column);
		}
		protected internal virtual void InternalColumnWidthChanged(TreeListColumn column) {
			OnColumnChanged(column);
			RaiseColumnWidthChanged(column);
		}
		protected virtual void Columns_Changed(object sender, CollectionChangeEventArgs e) {
			ViewInfo.ColumnsInfo.CellWidthes.Clear();
			TreeListColumn column = e.Element as TreeListColumn;
			switch(e.Action) {
				case CollectionChangeAction.Add:
					OnColumnAdded(column);
					break;
				case CollectionChangeAction.Remove:
					OnColumnRemoved(column);
					break;
				default:
					FireChanged();
					break;
			}
			OnColumnChanged(e.Element as TreeListColumn);
			OnFilterDataSourceChanged();
		}
		protected internal virtual void OnBandCollectionChanged(object sender, CollectionChangeEventArgs e) {
			TreeListBand band = e.Element as TreeListBand;
			if(e.Action == CollectionChangeAction.Add) {
				if(Bands.Count == 1)
					ViewInfo.RC.NeedsRestore = true;
				if(band != null && band.Site == null && Container != null) {
					try {
						Container.Add(band);
					}
					catch { }
				}
			}
			if(e.Action == CollectionChangeAction.Remove) {
				if(band != null && Container != null)
					Container.Remove(band);
			}
			if(sender == Bands) {
				if(IsLockUpdate)
					needsUpdateRootBands = true;
				else
					UpdateRootBands();
			}
			LayoutChanged();
			InvalidatePixelScrollingInfo();
		}
		protected internal virtual void OnBandColumnCollectionChanged(object sender, CollectionChangeEventArgs e) {
			TreeListColumn column = e.Element as TreeListColumn;
			if(e.Action == CollectionChangeAction.Add && column != null) 
				Columns.Add(column);
			LayoutChanged();
		}
		protected virtual void OnColumnAdded(TreeListColumn column) {
			if(Container != null && column.Container == null) {
				try {
					if(column.FieldName != string.Empty && column.Name == string.Empty)
						Container.Add(column, GetColumnName(column.FieldName));
					else
						Container.Add(column);
				}
				catch {
					Container.Add(column);
				}
			}
			if(column.Name == string.Empty && !IsLoading)
				column.Name = GetColumnName(column.FieldName);
		}
		protected virtual string GetColumnName(string fieldName) {
			string baseName = (TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.ColumnNamePrefix) + fieldName).Replace(" ", "");
			string result = baseName;
			int n = 1;
			while(true) {
				if(Container == null) break;
				if(Container.Components[result] == null) break;
				result = baseName + n++.ToString();
			}
			return result;
		}
		protected virtual void OnColumnRemoved(TreeListColumn column) {
			int index = VisibleColumns.IndexOf(column);
			int focusIndex = FocusedCellIndex;
			if(index != -1) {
				VisibleColumns.RemoveAt(index);
				if(focusIndex == index) focusIndex = -1;
				else if(index < focusIndex) focusIndex--;
				RefreshVisibleColumnsIndexes();
			}
			if(column.SortIndex > -1 || column.SortOrder != SortOrder.None)
				RefreshSortColumns();
			if(Container != null)
				Container.Remove(column);
			if(!TreeListDisposing) {
				RefreshCustomizationForm();
			}
			if(focusIndex == -1) {
				int oldCell = FocusedCellIndex;
				focusedCellIndex = -1;
				RaiseFocusedColumnChanged(-1, oldCell);
			}
			else focusedCellIndex = focusIndex;
			FormatConditions.OnColumnRemoved(column);
			FormatRules.OnColumnRemoved(column);
		}
		protected virtual void RemoveColumnFromBandColumns(TreeListColumn column) {
			foreach(TreeListBand band in Bands) 
				RemoveColumnFromBandColumnsCore(band, column);
		}
		protected virtual void RemoveColumnFromBandColumnsCore(TreeListBand band, TreeListColumn column) {
			if(band.Columns.Contains(column))
				band.Columns.Remove(column);
			foreach(TreeListBand child in band.Bands) 
				RemoveColumnFromBandColumnsCore(child, column);
		}
		protected virtual void OnColumnChanged(TreeListColumn column) {
			if(TreeListDisposing) return;
			viewInfo.SummaryFooterInfo.NeedsRecalcAll = true;
			viewInfo.PixelScrollingInfo.Invalidate();
			ClearAutoHeights();
			if(IsUnboundMode && !Columns.isClearing)
				Data.PopulateColumns();
			if(IsColumnHeaderAutoHeight)
				ViewInfo.CalcColumnPanelHeight();
			LayoutChanged();
			RefreshCustomizationForm();
			OnProperyChanged();
		}
		protected internal virtual void OnColumnUnboundExpressionChanged(TreeListColumn treeListColumn) {
			if(IsLoading || IsDeserializing) return;
			RefreshColumnsDataInfo();
			RaiseColumnUnboundExpressionChanged(treeListColumn);
		}
		protected virtual void RaiseColumnUnboundExpressionChanged(TreeListColumn column) {
			ColumnChangedEventHandler handler = (ColumnChangedEventHandler)this.Events[columnUnboundExpressionChanged];
			if(handler != null) handler(this, new ColumnChangedEventArgs(column));
		}
		protected internal virtual void InternalNodeChanged(TreeListNode node, NodeChangeTypeEnum changeType) {
			if(changeType == NodeChangeTypeEnum.HasChildren)
				LayoutChanged();
			if(changeType == NodeChangeTypeEnum.ImageIndex ||
				changeType == NodeChangeTypeEnum.StateImageIndex ||
				changeType == NodeChangeTypeEnum.SelectImageIndex ||
				changeType == NodeChangeTypeEnum.CheckedState)
				NodeImageChanged(node);
			RaiseNodeChanged(new NodeChangedEventArgs(node, changeType));
		}
		internal int InternalGetStateImage(TreeListNode node) {
			int nodeImageIndex = node.StateImageIndex;
			RaiseGetStateImage(node, ref nodeImageIndex);
			return nodeImageIndex;
		}
		internal int InternalGetSelectImage(TreeListNode node, bool focused) {
			int nodeImageIndex = GetImageIndexFromDataSource(node);
			if(nodeImageIndex == -1)
				nodeImageIndex = (focused ? node.SelectImageIndex : node.ImageIndex);
			RaiseGetSelectImage(node, focused, ref nodeImageIndex);
			return nodeImageIndex;
		}
		internal void InternalSelectImageClick(TreeListHitTest ht) {
			if(ht.RowInfo != null) {
				NodeClickEventArgs e = new NodeClickEventArgs(ht.RowInfo.Node, ht.MousePoint);
				RaiseSelectImageClick(e);
			}
		}
		internal void InternalStateImageClick(TreeListHitTest ht) {
			if(ht.RowInfo != null) {
				NodeClickEventArgs e = new NodeClickEventArgs(ht.RowInfo.Node, ht.MousePoint);
				RaiseStateImageClick(e);
			}
		}
		internal object InternalGetNodeValue(TreeListNode node, TreeListColumn column) {
			GetNodeDisplayValueEventArgs e = new GetNodeDisplayValueEventArgs(column, node);
			RaiseGetNodeDisplayValue(e);
			return e.Value;
		}
		internal int InternalCalcNodeHeight(RowInfo ri, int nodeHeight, ArrayList ColumnWidthes, bool useCache, out ArrayList cells, bool even, bool includeColumnIndent) {
			TreeListNode node = ri.Node;
			cells = new ArrayList();
			int result = nodeHeight;
			if(!ActualAutoNodeHeight)
				RaiseCalcNodeHeight(node, ref result);
			else {
				if(useCache) {
					object realHeight = autoHeights[node];
					if(realHeight != null) return (int)realHeight;
				}
				ViewInfo.GInfo.AddGraphics(null);
				try {
					AppearanceObject[] app = GetMaxNodeCellHeightAppearances(even);
					foreach(ColumnWidthInfo cw in ColumnWidthes) {
						if(cw.Column == null) continue;
						ColumnInfo ci = viewInfo.CreateColumnInfo(cw.Column);
						CellInfo cell = viewInfo.CreateCellInfo(ci, ri);						
						cell.EditorViewInfo.EditValue = InternalGetNodeValue(node, cw.Column);
						int width = cw.Width - (ViewInfo.RC.vlw + 2 * CellInfo.CellTextIndent + (includeColumnIndent ? GetColumnIndent(cw.Column, node) : 0));
						app[app.Length - 1] = InternalGetCustomNodeCellStyle(cw.Column, node, (AppearanceObject)ViewInfo.PaintAppearance.Row.Clone());
						result = Math.Max(result, GetMaxNodeCellHeight(ri, cw.Column, cell.EditorViewInfo, nodeHeight, app, width - CellInfo.CellTextIndent - ViewInfo.RC.vlw));
						cells.Add(cell);
					}
				}
				finally {
					ViewInfo.GInfo.ReleaseGraphics();
				}
				if(useCache)
					autoHeights[node] = result;
			}
			return result;
		}
		AppearanceObject[] GetMaxNodeCellHeightAppearances(bool even) {
			AppearanceObject[] app = ViewInfo.GetRowMaxHeightAppearances(even ? TreeNodeCellState.Even : TreeNodeCellState.Odd);
			AppearanceObject[] result = new AppearanceObject[app.Length + 1];
			Array.Copy(app, result, app.Length);
			return result;
		}
		int GetMaxNodeCellHeight(RowInfo ri, TreeListColumn column, DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo editInfo, int defaultValue, AppearanceObject[] app, int cellWidth) {
			DevExpress.XtraEditors.ViewInfo.IHeightAdaptable ah = editInfo as DevExpress.XtraEditors.ViewInfo.IHeightAdaptable;
			if(ah == null) return defaultValue;
			int result = 0;
			editInfo.PaintAppearance = new AppearanceObject();
			editInfo.AllowDrawFocusRect = false;
			AppearanceHelper.Combine(editInfo.PaintAppearance, new AppearanceObject[] { ViewInfo.RC.CreateAppearance(app), column.AppearanceCell, ri.ConditionInfo.GetCellAppearance(column) });
			return Math.Max(ah.CalcHeight(ViewInfo.GInfo.Cache, cellWidth) + 2 * CellInfo.CellTextIndent, result);
		}
		internal int InternalCalcNodeDragImageIndex(CalcNodeDragImageIndexEventArgs e) {
			RaiseCalcNodeDragImageIndex(e);
			return e.ImageIndex;
		}
		protected virtual bool CanFocusNode(TreeListNode oldNode, TreeListNode node) {
			if(Position > -1 && node == null && !IsLookUpMode)
				return false;
			this.isFocusedNodeDataModified = !CheckValidateFocusNode();
			if(IsFocusedNodeDataModified) {
				if(ContainerHelper.AllowHideException) throw new HideException();
				return false;
			}
			bool canFocus = true;
			RaiseBeforeFocusNode(oldNode, node, ref canFocus);
			return canFocus;
		}
		internal int InternalGetCompareResult(CompareNodeValuesEventArgs e) {
			RaiseCompareNodeValues(e);
			return e.Result;
		}
		internal void InternalDefaultPaintHelperChanged() {
			if(ViewInfo != null) ViewInfo.PaintAnimatedItems = false;
			Invalidate();
			RaiseDefaultPaintHelperChanged();
		}
		#endregion
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if(TreeListDisposing) return;
			UpdateLayout();
			if(ViewInfo.IsValid)
				painter.DoDraw(viewInfo, new DXPaintEventArgs(e));
			if(!Enabled && UseDisabledStatePainter) BackgroundPaintHelper.PaintDisabledControl(Painter.ElementPainters.IsSkin ? ElementsLookAndFeel : null, e, ClientRectangle);
			RaisePaintEvent(this, e);
		}
		protected override void OnLoaded() {
			if(IsLoading || IsInitialized) return;
			base.OnLoaded();
			OnLoadedCore();
		}
		bool isLoadingInternal = false;
		protected virtual void OnLoadedCore() {
			Handler.SetControlState(Handler.Regular);
			Data.PopulateColumns();
			BeginUpdate();
			try {
				isLoadingInternal = true;
				if(CanForceLoadNodes)
					UpdateDataSource(true);
				CorrectIndentWidth();
				viewInfo.RC.NeedsRestore = true;
				viewInfo.SummaryFooterInfo.NeedsRecalcAll = true;
				NormalizeVisibleColumnIndices();
				RefreshSortColumns();
				RefreshColumnHandles();
				UpdateRootBands();
				ParseFindFilterText();
				FilterNodes();
				DoSort(Nodes, false);
				OnFilterDataSourceChanged();
			}
			finally {
				isLoadingInternal = false;
				EndUpdate();
			}
			if(FocusedNode == null && !IsLookUpMode)
				FocusedNode = FindNodeByID(Position);
			if(Position == -1) 
				MakeNodeVisible(FocusedNode);
			FocusedCellIndex = 0;
			RaiseLoad();
		}
		protected virtual void RaiseLoad() {
			EventHandler handler = (EventHandler)this.Events[load];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected override void OnSystemColorsChanged(EventArgs e) {
			base.OnSystemColorsChanged(e);
			DevExpress.Utils.WXPaint.Painter.ThemeChanged();
			LayoutChanged();
		}
		protected override void OnBindingContextChanged(EventArgs e) {
			base.OnBindingContextChanged(e);
			if(!IsInitialized || lockUpdateDataSource != 0) return;
			lockUpdateDataSource++;
			try {
				CurrencyManager cm = GetCurrencyManager();
				if(cm == CurrencyManager) {
					if(cm != null) {
						if(cm.List == CurrencyManager.List) return;
					}
					else return;
				}
				}
			finally {
				lockUpdateDataSource--;
			}
			UpdateDataSource(true);
		}
		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
			LayoutChanged();
		}
		protected virtual void OnLookAndFeel_StyleChanged(object sender, EventArgs e) {
			EditorHelper.DestroyEditorsCache();
			DestroyDragArrows();
			UpdateElementsLookAndFeel();
			ViewInfo.RC.NeedsRestore = true;
			ViewInfo.SummaryFooterInfo.NeedsRecalcRects = true;
			ViewInfo.SetAppearanceDirty();
			Painter.UpdateElementPainters();
			ClearAutoHeights();
			FitColumns();
			LayoutChanged();
		}
		protected virtual void UpdateElementsLookAndFeel() {
			ElementsLookAndFeel.Assign(LookAndFeel.ActiveLookAndFeel);
			ElementsLookAndFeel.UseDefaultLookAndFeel = false;
		}
		protected internal string GetNonFormattedCaption(string caption) {
			if(!OptionsView.AllowHtmlDrawHeaders) return caption;
			return StringPainter.Default.RemoveFormat(caption);
		}
		protected virtual void RaiseCellValueChanging(CellValueChangedEventArgs e) {
			CellValueChangedEventHandler handler = (CellValueChangedEventHandler)this.Events[cellValueChanging];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseCellValueChanged(CellValueChangedEventArgs e) {
			CellValueChangedEventHandler handler = (CellValueChangedEventHandler)this.Events[cellValueChanged];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseColumnChanged(TreeListColumn column) {
			ColumnChangedEventHandler handler = (ColumnChangedEventHandler)this.Events[columnChanged];
			if(handler != null) handler(this, new ColumnChangedEventArgs(column));
		}
		protected virtual void RaiseColumnWidthChanged(TreeListColumn column) {
			ColumnWidthChangedEventHandler handler = (ColumnWidthChangedEventHandler)this.Events[columnWidthChanged];
			if(handler != null) handler(this, new ColumnChangedEventArgs(column));
		}
		protected internal virtual void RaiseColumnButtonClick() {
			EventHandler handler = (EventHandler)this.Events[columnButtonClick];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseCreateCustomNode(CreateCustomNodeEventArgs e) {
			CreateCustomNodeEventHandler handler = (CreateCustomNodeEventHandler)this.Events[createCustomNode];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseValidatingEditor(BaseContainerValidateEditorEventArgs e) {
			BaseContainerValidateEditorEventHandler handler = (BaseContainerValidateEditorEventHandler)this.Events[validatingEditor];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseShowingEditor(ref bool cancel) {
			cancel = false;
			CancelEventHandler handler = (CancelEventHandler)this.Events[showingEditor];
			if(handler != null) {
				CancelEventArgs e = new CancelEventArgs();
				handler(this, e);
				cancel = e.Cancel;
			}
		}
		protected virtual void RaiseShownEditor() {
			EventHandler handler = (EventHandler)this.Events[shownEditor];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseHiddenEditor() {
			EventHandler handler = (EventHandler)this.Events[hiddenEditor];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseInvalidValueException(InvalidValueExceptionEventArgs e) {
			InvalidValueExceptionEventHandler handler = (InvalidValueExceptionEventHandler)this.Events[invalidValueException];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseValidateNode(ValidateNodeEventArgs e) {
			ValidateNodeEventHandler handler = (ValidateNodeEventHandler)this.Events[validateNode];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseInvalidNodeException(InvalidNodeExceptionEventArgs ex) {
			InvalidNodeExceptionEventHandler handler = (InvalidNodeExceptionEventHandler)this.Events[invalidNodeException];
			if(handler != null) handler(this, ex);
			if(ex.ExceptionMode == ExceptionMode.DisplayError) {
				DialogResult dr = XtraMessageBox.Show(ex.ErrorText, ex.WindowCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				if(dr == DialogResult.No)
					ex.ExceptionMode = ExceptionMode.Ignore;
			}
			if(ex.ExceptionMode == ExceptionMode.Ignore) CancelCurrentEdit();
			if(ex.ExceptionMode == ExceptionMode.ThrowException) {
				throw (ex.Exception);
			}
		}
		protected virtual void RaiseGetCustomNodeCellEdit(GetCustomNodeCellEditEventArgs e, ref RepositoryItem item) {
			GetCustomNodeCellEditEventHandler handler = (GetCustomNodeCellEditEventHandler)this.Events[customNodeCellEdit];
			if(handler != null) {
				handler(this, e);
				item = e.RepositoryItem;
			}
		}
		protected virtual AppearanceObject RaiseGetCustomNodeCellStyle(GetCustomNodeCellStyleEventArgs e) {
			GetCustomNodeCellStyleEventHandler handler = (GetCustomNodeCellStyleEventHandler)this.Events[nodeCellStyle];
			if(handler != null) {
				handler(this, e);
			}
			return e.Appearance;
		}
		protected internal virtual int RaiseMeasurePreviewHeight(TreeListNode node) {
			NodePreviewHeightEventHandler handler = (NodePreviewHeightEventHandler)this.Events[measurePreviewHeight];
			if(handler != null) {
				NodePreviewHeightEventArgs e = new NodePreviewHeightEventArgs(node);
				handler(this, e);
				return e.PreviewHeight;
			}
			return -1;
		}
		protected virtual void RaiseGetPreviewText(TreeListNode node, ref string text) {
			GetPreviewTextEventHandler handler = (GetPreviewTextEventHandler)this.Events[getPreviewText];
			if(handler != null) {
				GetPreviewTextEventArgs e = new GetPreviewTextEventArgs(node, text);
				handler(this, e);
				text = e.PreviewText;
			}
		}
		protected virtual void RaiseGetPrintPreviewText(TreeListNode node, ref string text) {
			GetPreviewTextEventHandler handler = (GetPreviewTextEventHandler)this.Events[getPrintPreviewText];
			if(handler != null) {
				GetPreviewTextEventArgs e = new GetPreviewTextEventArgs(node, text);
				handler(this, e);
				text = e.PreviewText;
			}
			else RaiseGetPreviewText(node, ref text);
		}
		protected virtual void RaiseGetPrintCustomSummaryValue(TreeListNodes nodes, TreeListColumn col, ref object val, bool isSummaryFooter) {
			GetCustomSummaryValueEventHandler handler = (GetCustomSummaryValueEventHandler)this.Events[getPrintCustomSummaryValue];
			if(handler == null) {
				RaiseGetCustomSummaryValue(nodes, col, ref val, isSummaryFooter);
				return;
			}
			GetCustomSummaryValueEventArgs e = new GetCustomSummaryValueEventArgs(nodes,
				col, nodes == Nodes);
			handler(this, e);
			val = e.CustomValue;
		}
		protected virtual void RaiseGetCustomSummaryValue(TreeListNodes nodes, TreeListColumn col, ref object val, bool isSummaryFooter) {
			GetCustomSummaryValueEventHandler handler = (GetCustomSummaryValueEventHandler)this.Events[getCustomSummaryValue];
			if(handler != null) {
				GetCustomSummaryValueEventArgs e = new GetCustomSummaryValueEventArgs(nodes,
					col, isSummaryFooter);
				handler(this, e);
				val = e.CustomValue;
			}
		}
		protected virtual void RaiseBeforeExpand(TreeListNode node, ref bool canExpand) {
			BeforeExpandEventHandler handler = (BeforeExpandEventHandler)this.Events[beforeExpand];
			if(handler != null) {
				BeforeExpandEventArgs e = new BeforeExpandEventArgs(node);
				handler(this, e);
				canExpand = e.CanExpand;
			}
		}
		protected virtual void RaiseBeforeCollapse(TreeListNode node, ref bool canCollapse) {
			BeforeCollapseEventHandler handler = (BeforeCollapseEventHandler)this.Events[beforeCollapse];
			if(handler != null) {
				BeforeCollapseEventArgs e = new BeforeCollapseEventArgs(node);
				handler(this, e);
				canCollapse = e.CanCollapse;
			}
		}
		protected internal virtual void RaiseBeforeDragNode(BeforeDragNodeEventArgs e) {
			BeforeDragNodeEventHandler handler = (BeforeDragNodeEventHandler)this.Events[beforeDragNode];
			if(handler != null)
				handler(this, e);
		}
		protected internal virtual void RaiseAfterDragNode(AfterDragNodeEventArgs e) {
			AfterDragNodeEventHandler handler = (AfterDragNodeEventHandler)this.Events[afterDragNode];
			if(handler != null) handler(this, e);
		}
		protected internal bool RaiseCustomizeNewNodeFromOuterData(CustomizeNewNodeFromOuterDataEventArgs e) {
			CustomizeNewNodeFromOuterDataEventHandler handler = Events[customizeNewNodeFromOuterData] as CustomizeNewNodeFromOuterDataEventHandler;
			if(handler != null)
				handler(this, e);
			return e.Handled;
		}
		protected internal void RaiseAfterDropNode(AfterDropNodeEventArgs e) {
			AfterDropNodeEventHandler handler = Events[afterDropNode] as AfterDropNodeEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseBeforeDropNode(BeforeDropNodeEventArgs e) {
			BeforeDropNodeEventHandler handler = Events[beforeDropNode] as BeforeDropNodeEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected virtual void RaiseBeforeFocusNode(TreeListNode oldNode, TreeListNode node, ref bool canFocus) {
			BeforeFocusNodeEventHandler handler = (BeforeFocusNodeEventHandler)this.Events[beforeFocusNode];
			if(handler != null) {
				BeforeFocusNodeEventArgs e = new BeforeFocusNodeEventArgs(oldNode, node);
				handler(this, e);
				canFocus = e.CanFocus;
			}
		}
		protected virtual void RaiseAfterFocusNode(TreeListNode node) {
			NodeEventHandler handler = (NodeEventHandler)this.Events[afterFocusNode];
			if(handler != null) handler(this, new NodeEventArgs(node));
		}
		protected internal virtual void RaiseSelectionChanged() {
			if(!OptionsSelection.MultiSelect) return;
			EventHandler handler = (EventHandler)this.Events[selectionChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseStateChanged() {
			EventHandler handler = (EventHandler)this.Events[stateChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseAfterExpand(TreeListNode node) {
			NodeEventHandler handler = (NodeEventHandler)this.Events[afterExpand];
			if(handler != null) handler(this, new NodeEventArgs(node));
		}
		protected virtual void RaiseAfterCollapse(TreeListNode node) {
			NodeEventHandler handler = (NodeEventHandler)this.Events[afterCollapse];
			if(handler != null) handler(this, new NodeEventArgs(node));
		}
		protected internal virtual CheckNodeEventArgs RaiseBeforeCheckNode(TreeListNode node, CheckState prevState, CheckState state) {
			CheckNodeEventHandler handler = (CheckNodeEventHandler)this.Events[beforeCheckNode];
			CheckNodeEventArgs e = new CheckNodeEventArgs(node, prevState, state);
			if(handler != null) handler(this, e);
			return e;
		}
		protected internal virtual void RaiseAfterCheckNode(TreeListNode node) {
			NodeEventHandler handler = (NodeEventHandler)this.Events[afterCheckNode];
			if(handler != null) handler(this, new NodeEventArgs(node));
		}
		protected virtual void RaiseGetStateImage(TreeListNode node, ref int nodeImageIndex) {
			GetStateImageEventHandler handler = (GetStateImageEventHandler)this.Events[getStateImage];
			if(handler != null) {
				GetStateImageEventArgs e = new GetStateImageEventArgs(node, nodeImageIndex);
				handler(this, e);
				nodeImageIndex = e.NodeImageIndex;
			}
		}
		protected virtual void RaiseGetSelectImage(TreeListNode node, bool focused, ref int nodeImageIndex) {
			GetSelectImageEventHandler handler = (GetSelectImageEventHandler)this.Events[getSelectImage];
			if(handler != null) {
				GetSelectImageEventArgs e = new GetSelectImageEventArgs(node, nodeImageIndex, focused);
				handler(this, e);
				nodeImageIndex = e.NodeImageIndex;
			}
		}
		protected virtual void RaiseStateImageClick(NodeClickEventArgs e) {
			NodeClickEventHandler handler = (NodeClickEventHandler)this.Events[stateImageClick];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseSelectImageClick(NodeClickEventArgs e) {
			NodeClickEventHandler handler = (NodeClickEventHandler)this.Events[selectImageClick];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseGetNodeDisplayValue(GetNodeDisplayValueEventArgs e) {
			GetNodeDisplayValueEventHandler handler = (GetNodeDisplayValueEventHandler)this.Events[getNodeDisplayValue];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomUnboundColumnData(TreeListCustomColumnDataEventArgs e) {
			CustomColumnDataEventHandler handler = (CustomColumnDataEventHandler)this.Events[customUnboundColumnData];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseCalcNodeHeight(TreeListNode node, ref int nodeHeight) {
			CalcNodeHeightEventHandler handler = (CalcNodeHeightEventHandler)this.Events[calcNodeHeight];
			if(handler != null) {
				CalcNodeHeightEventArgs e = new CalcNodeHeightEventArgs(node, nodeHeight);
				handler(this, e);
				nodeHeight = e.NodeHeight;
			}
		}
		protected virtual void RaiseCalcNodeDragImageIndex(CalcNodeDragImageIndexEventArgs e) {
			CalcNodeDragImageIndexEventHandler handler = (CalcNodeDragImageIndexEventHandler)this.Events[calcNodeDragImageIndex];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseNodeChanged(NodeChangedEventArgs e) {
			NodeChangedEventHandler handler = (NodeChangedEventHandler)this.Events[nodeChanged];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseFocusedNodeChanged(TreeListNode oldNode, TreeListNode newNode) {
			ClearColumnErrors();
			if(IsFocusedNodeChangedLocked) return;
			AccessibleNotifyClients(AccessibleEvents.Focus, 10, -1);
			FocusedNodeChangedEventHandler handler = (FocusedNodeChangedEventHandler)this.Events[focusedNodeChanged];
			if(handler != null && oldNode != newNode) handler(this, new FocusedNodeChangedEventArgs(oldNode, newNode));
		}
		protected virtual void RaiseFocusedColumnChanged(int newVisibleIndex, int oldVisibleIndex) {
			if(TreeListDisposing) return;
			AccessibleNotifyClients(AccessibleEvents.Focus, 10, -1);
			FocusedColumnChangedEventHandler handler = (FocusedColumnChangedEventHandler)this.Events[focusedColumnChanged];
			if(handler != null && newVisibleIndex != oldVisibleIndex) {
				TreeListColumn newCol = VisibleColumns[newVisibleIndex];
				TreeListColumn oldCol = VisibleColumns[oldVisibleIndex];
				FocusedColumnChangedEventArgs e = new FocusedColumnChangedEventArgs(newCol, oldCol);
				handler(this, e);
			}
		}
		protected virtual void RaiseCompareNodeValues(CompareNodeValuesEventArgs e) {
			CompareNodeValuesEventHandler handler = (CompareNodeValuesEventHandler)this.Events[compareNodeValues];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseDragCancelNode() {
			EventHandler handler = (EventHandler)this.Events[dragCancelNode];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseShowCustomizationForm() {
			EventHandler handler = (EventHandler)this.Events[showCustomizationForm];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseHideCustomizationForm(object sender, EventArgs e) {
			customizationForm.Disposed -= new EventHandler(RaiseHideCustomizationForm);
			SaveCustomizationFormBounds();
			EventHandler handler = (EventHandler)this.Events[hideCustomizationForm];
			if(handler != null) handler(this, EventArgs.Empty);
			customizationForm = null;
		}
		void SaveCustomizationFormBounds() {
			this.customizationFormBounds = customizationForm.Bounds;
		}
		protected internal virtual void RaiseCustomDrawNodeIndicator(CustomDrawNodeIndicatorEventArgs e) {
			CustomDrawNodeIndicatorEventHandler handler = (CustomDrawNodeIndicatorEventHandler)this.Events[customDrawNodeIndicator];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawColumnHeader(CustomDrawColumnHeaderEventArgs e) {
			CustomDrawColumnHeaderEventHandler handler = (CustomDrawColumnHeaderEventHandler)this.Events[customDrawColumnHeader];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawBandHeader(CustomDrawBandHeaderEventArgs e) {
			CustomDrawBandHeaderEventHandler handler = (CustomDrawBandHeaderEventHandler)this.Events[customDrawBandHeader];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawNodePreview(CustomDrawNodePreviewEventArgs e) {
			CustomDrawNodePreviewEventHandler handler = (CustomDrawNodePreviewEventHandler)this.Events[customDrawNodePreview];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawNodeCell(CustomDrawNodeCellEventArgs e) {
			CustomDrawNodeCellEventHandler handler = (CustomDrawNodeCellEventHandler)this.Events[customDrawNodeCell];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawFooter(CustomDrawEventArgs e) {
			CustomDrawFooterEventHandler handler = (CustomDrawFooterEventHandler)this.Events[customDrawFooter];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawRowFooter(CustomDrawRowFooterEventArgs e) {
			CustomDrawRowFooterEventHandler handler = (CustomDrawRowFooterEventHandler)this.Events[customDrawRowFooter];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawFooterCell(CustomDrawFooterCellEventArgs e) {
			CustomDrawFooterCellEventHandler handler = (CustomDrawFooterCellEventHandler)this.Events[customDrawFooterCell];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawRowFooterCell(CustomDrawRowFooterCellEventArgs e) {
			CustomDrawRowFooterCellEventHandler handler = (CustomDrawRowFooterCellEventHandler)this.Events[customDrawRowFooterCell];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawEmptyArea(CustomDrawEmptyAreaEventArgs e) {
			CustomDrawEmptyAreaEventHandler handler = (CustomDrawEmptyAreaEventHandler)this.Events[customDrawEmptyArea];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawNodeIndent(CustomDrawNodeIndentEventArgs e) {
			CustomDrawNodeIndentEventHandler handler = (CustomDrawNodeIndentEventHandler)this.Events[customDrawNodeIndent];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawNodeCheckBox(CustomDrawNodeCheckBoxEventArgs e) {
			CustomDrawNodeCheckBoxEventHandler handler = (CustomDrawNodeCheckBoxEventHandler)this.Events[customDrawNodeCheckBox];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawNodeImages(CustomDrawNodeImagesEventArgs e) {
			CustomDrawNodeImagesEventHandler handler = (CustomDrawNodeImagesEventHandler)this.Events[customDrawNodeImages];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawNodeButton(CustomDrawNodeButtonEventArgs e) {
			CustomDrawNodeButtonEventHandler handler = (CustomDrawNodeButtonEventHandler)this.Events[customDrawNodeButton];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawFilterPanel(CustomDrawObjectEventArgs e) {
			CustomDrawObjectEventHandler handler = (CustomDrawObjectEventHandler)this.Events[customDrawFilterPanel];
			if(handler != null) handler(this, e);
		}
		[Obsolete()]
		protected virtual void RaiseShowTreeListMenu(TreeListMenuEventArgs e) {
			TreeListMenuEventHandler handler = (TreeListMenuEventHandler)this.Events[showTreeListMenu];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaisePopupMenuShowing(PopupMenuShowingEventArgs e) {
			PopupMenuShowingEventHandler handler = (PopupMenuShowingEventHandler)this.Events[onPopupMenuShowing];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseTreeListMenuItemClick(TreeListMenuItemClickEventArgs e) {
			TreeListMenuItemClickEventHandler handler = (TreeListMenuItemClickEventHandler)this.Events[treelistMenuItemClick];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseLeftCoordChanged() {
			EventHandler handler = (EventHandler)this.Events[leftCoordChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseTopVisibleNodeIndexChanged() {
			EventHandler handler = (EventHandler)this.Events[topVisibleNodeIndexChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseDefaultPaintHelperChanged() {
			EventHandler handler = (EventHandler)this.Events[defaultPaintHelperChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseNodesReloaded() {
			EventHandler handler = (EventHandler)this.Events[nodesReloaded];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseStartSorting() {
			EventHandler handler = (EventHandler)this.Events[startSorting];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseEndSorting() {
			EventHandler handler = (EventHandler)this.Events[endSorting];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseLayoutUpdated() {
			EventHandler handler = (EventHandler)this.Events[layoutUpdated];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseLayoutUpgrade(LayoutUpgradeEventArgs e) {
			LayoutUpgradeEventHandler handler = (LayoutUpgradeEventHandler)this.Events[layoutUpgrade];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseBeforeLoadLayout(LayoutAllowEventArgs e) {
			LayoutAllowEventHandler handler = (LayoutAllowEventHandler)this.Events[beforeLoadLayout];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseFilterNodeEvent(FilterNodeEventArgs e) {
			FilterNodeEventHandler handler = (FilterNodeEventHandler)this.Events[filterNode];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseDragObjectDrop(DragObjectDropEventArgs e) {
			DragObjectDropEventHandler handler = (DragObjectDropEventHandler)this.Events[dragObjectDrop];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseDragObjectOver(DragObjectOverEventArgs e) {
			DragObjectOverEventHandler handler = (DragObjectOverEventHandler)this.Events[dragObjectOver];
			if(handler != null) handler(this, e);
		}
		protected internal virtual bool RaiseDragObjectStart(DragObjectStartEventArgs e) {
			DragObjectStartEventHandler handler = (DragObjectStartEventHandler)this.Events[dragObjectStart];
			if(handler != null) handler(this, e);
			return e.Allow;
		}
		#region for handler
		protected int fUpdateSize;
		protected internal virtual bool IsUpdateSize { get { return fUpdateSize != 0; } }
		protected override void OnResize(EventArgs e) {
			try {
				fUpdateSize++;
				shouldFitColumns = true;
				Handler.OnResize();
			}
			finally {
				fUpdateSize--;
			}
			base.OnResize(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			try {
				base.OnMouseDown(e);				
				Handler.OnMouseDown(e);
			}
			catch(HideException) {
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			Handler.OnMouseMove(e);
			base.OnMouseMove(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			try {
				base.OnMouseUp(GetMouseUpEventArgs(e));
				Handler.OnMouseUp(e);
			}
			catch(HideException) {
			}
		}
		protected override void OnDoubleClick(EventArgs e) {
			try {
				base.OnDoubleClick(e);
				Handler.OnDoubleClick();
			}
			catch(HideException) {
			}
		}
		protected override void OnMouseWheelCore(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseWheelCore(ee);
			Handler.OnMouseWheel(ee);
		}
		protected override void OnMouseEnter(EventArgs e) {
			Handler.OnMouseEnter(PointToClient(Control.MousePosition));
			if(!OptionsBehavior.SmartMouseHover) {
				base.OnMouseEnter(e);
				return;
			}
			if(mouseIn) return;
			mouseIn = RectangleToScreen(ClientRectangle).Contains(Cursor.Position);
			if(mouseIn)
				base.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			Handler.OnMouseLeave();
			if(!OptionsBehavior.SmartMouseHover) {
				base.OnMouseLeave(e);
				return;
			}
			if(!mouseIn) return;
			mouseIn = RectangleToScreen(ClientRectangle).Contains(Cursor.Position);
			if(!mouseIn)
				base.OnMouseLeave(e);
		}
		protected internal virtual void Scrollbar_MouseEnter(object sender, EventArgs e) {
			if(!OptionsBehavior.SmartMouseHover) return;
			OnMouseEnter(e);
		}
		protected internal virtual void Scrollbar_MouseLeave(object sender, EventArgs e) {
			if(!OptionsBehavior.SmartMouseHover) return;
			OnMouseLeave(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled) return;
			try { Handler.OnKeyDown(e); }
			catch(HideException) {
			}
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			base.OnKeyPress(e);
			if(e.Handled) return;
			try { Handler.OnKeyPress(e); }
			catch(HideException) {
			}
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if(e.Handled) return;
			Handler.OnKeyUp(e);
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			EditorHelper.HideHint();
			if(this.Visible) ScrollInfo.UpdateVisibility();
			if((!this.Visible || (FindForm() == null)) || !FindForm().Visible) {
				DestroyCustomization();
				if(Handler.State == TreeListState.ColumnDragging) {
					Handler.SetControlState(TreeListState.Regular);
				}
			}
		}
		protected override bool FireOnLoadOnPaint { get { return true; } }
		protected override void OnGotFocus(EventArgs e) {
			if(!TreeListDisposing && !TreeListFormDisposing)
				Handler.OnGotFocus();
			base.OnGotFocus(e);
		}
		protected override void OnLostFocus(EventArgs e) {
			if(!TreeListDisposing && !TreeListFormDisposing)
				Handler.OnLostFocus();
			base.OnLostFocus(e);
		}
		protected override void OnLostCapture() {
			Handler.OnLostCapture();
		}
		protected override void OnDragEnter(DragEventArgs drgevent) {
			Handler.OnDragEnter(drgevent);
			DragDropEffects effect = drgevent.Effect;
			base.OnDragEnter(drgevent);
			Handler.RegisterLastDragEffect(effect);
		}
		protected override void OnDragOver(DragEventArgs drgevent) {
			Handler.OnDragOver(drgevent);
			DragDropEffects effect = drgevent.Effect;
			base.OnDragOver(drgevent);
			Handler.RegisterLastDragEffect(effect);
		}
		protected override void OnDragLeave(EventArgs e) {
			Handler.OnDragLeave();
			base.OnDragLeave(e);
		}
		protected override void OnDragDrop(DragEventArgs drgevent) {
			base.OnDragDrop(drgevent);
			Handler.OnDragDrop(drgevent);
		}
		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs qcdevent) {
			Handler.OnQueryContinueDrag(qcdevent);
			base.OnQueryContinueDrag(qcdevent);
		}
		#endregion
		#endregion
		#region VirtualTreeList
		public interface IVirtualTreeListData {
			void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info);
			void VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info);
			void VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info);
		}
		protected internal void OnVirtualTreeListNodeExpand(TreeListNode node) {
			VirtualDataHelper.OnNodeExpand(node);
		}
		TreeListVirtualDataHelper virtualDataHelper = null;
		protected internal TreeListVirtualDataHelper VirtualDataHelper {
			get {
				if(virtualDataHelper == null)
					virtualDataHelper = CreateVirtualDataHelper();
				return virtualDataHelper;
			}
		}
		TreeListFilterHelper filterHelper = null;
		protected internal TreeListFilterHelper FilterHelper {
			get {
				if(filterHelper == null)
					filterHelper = CreateFilterHelper();
				return filterHelper;
			}
		}
		protected virtual TreeListFilterHelper CreateFilterHelper() {
			return new TreeListFilterHelper(this);
		}
		protected virtual TreeListVirtualDataHelper CreateVirtualDataHelper() {
			return new TreeListVirtualDataHelper(this);
		}
		public class TreeListVirtualDataHelper {
			TreeList treeList;
			protected TreeListVirtualData Data { get { return treeList.Data as TreeListVirtualData; } }
			public bool EnableDynamicLoading { get { return treeList.EnableDynamicLoading; } }
			public TreeListVirtualDataHelper(TreeList treeList) {
				this.treeList = treeList;
			}
			public virtual void OnNodeExpand(TreeListNode node) {
				BeginUpdate();
				treeList.LockReloadNodes();
				try {
					Data.CheckNodeCellsData(node);
					Data.OpenChildNodes(node);
				}
				finally {
					treeList.UnlockReloadNodes();
					treeList.DoSort(node.Nodes, false);
					treeList.EndUpdate();
				}
				treeList.ResetNodesCounters();
				treeList.CheckFocusedNode();
			}
			public virtual IList GetChildrenViaInterface(object node, IVirtualTreeListData parent) {
				if(node == null) return null;
				VirtualTreeGetChildNodesInfo info = new VirtualTreeGetChildNodesInfo(node);
				IVirtualTreeListData data = GetVirtualTreeListData(node, parent);
				if(data == null) return null;
				data.VirtualTreeGetChildNodes(info);
				return info.Children;
			}
			public virtual object GetCellDataViaInterface(object node, TreeListColumn column, IVirtualTreeListData parent) {
				if(node == null) return null;
				VirtualTreeGetCellValueInfo info = new VirtualTreeGetCellValueInfo(node, column);
				IVirtualTreeListData data = GetVirtualTreeListData(node, parent);
				if(data == null) return null;
				data.VirtualTreeGetCellValue(info);
				return info.CellData;
			}
			public virtual bool SetCellDataViaInterface(object oldValue, object newValue, object node, TreeListColumn column, IVirtualTreeListData parent) {
				VirtualTreeSetCellValueInfo info = new VirtualTreeSetCellValueInfo(oldValue, newValue, node, column);
				IVirtualTreeListData data = GetVirtualTreeListData(node, parent);
				if(data == null) return false;
				data.VirtualTreeSetCellValue(info);
				return !info.Cancel;
			}
			IVirtualTreeListData GetVirtualTreeListData(object node, IVirtualTreeListData parent) {
				IVirtualTreeListData data = (parent == null ? node as IVirtualTreeListData : parent);
				if(data != null) return data;
				return this.treeList != null ? this.treeList.DataSource as IVirtualTreeListData : null;
			}
			public virtual IList GetChildrenViaEvent(object node) {
				VirtualTreeGetChildNodesEventHandler handler = (VirtualTreeGetChildNodesEventHandler)treeList.Events[virtualTreeGetChildNodes];
				if(handler != null) {
					VirtualTreeGetChildNodesInfo args = new VirtualTreeGetChildNodesInfo(node);
					handler(treeList, args);
					return args.Children;
				}
				return null;
			}
			public virtual bool SetCellDataViaEvent(object oldValue, object newValue, object node, TreeListColumn column) {
				VirtualTreeSetCellValueEventHandler handler = (VirtualTreeSetCellValueEventHandler)treeList.Events[virtualTreeSetCellValue];
				if(handler != null) {
					VirtualTreeSetCellValueInfo args = new VirtualTreeSetCellValueInfo(oldValue, newValue, node, column);
					handler(treeList, args);
					return !args.Cancel;
				}
				return true;
			}
			public virtual object GetCellDataViaEvent(object node, TreeListColumn column) {
				VirtualTreeGetCellValueEventHandler handler = (VirtualTreeGetCellValueEventHandler)treeList.Events[virtualTreeGetCellValue];
				if(handler != null) {
					VirtualTreeGetCellValueInfo args = new VirtualTreeGetCellValueInfo(node, column);
					handler(treeList, args);
					return args.CellData;
				}
				return null;
			}
			public void BeginUpdate() {
				treeList.BeginUpdate();
			}
			public void EndUpdate() {
				treeList.CheckFocusedNode();
				treeList.EndUpdate();
			}
			public void OnNodeCountChanged() { treeList.ResetNodesCounters(); }
			public object GetDataSource() { return treeList.DataSource; }
			internal void CheckNodeVisiblity(TreeListNode node) {
				treeList.DoFilterNode(node);
			}
			internal void CheckAutoFocusNewNode(TreeListNode appendedNode) {
				if(treeList.OptionsNavigation.AutoFocusNewNode)
					treeList.autoFocusedNode = appendedNode;
			}
		}
		#endregion
		#region Filtering
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CriteriaOperator ActiveFilterCriteria {
			get { return activeFilterCriteria; }
			set {
				if(ReferenceEquals(ActiveFilterCriteria, value))
					return;
				activeFilterCriteria = value;
				OnActiveFilterCriteriaChanged();
				OnProperyChanged();
			}
		}
		[Browsable(false), XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public CriteriaOperator NonColumnFilterCriteria { get { return nonColumnFilterCriteria; } }
		[Browsable(false), XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public string NonColumnFilterString { get { return CriteriaOperator.ToString(NonColumnFilterCriteria); } }
		[Browsable(false), DefaultValue(""), XtraSerializableProperty]
		public string ActiveFilterString {
			get { return CriteriaOperator.ToString(ActiveFilterCriteria); }
			set { ActiveFilterCriteria = CriteriaOperator.TryParse(value); }
		}
		[DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ActiveFilterEnabled {
			get { return activeFilterEnabled; }
			set {
				if(ActiveFilterEnabled == value) return;
				activeFilterEnabled = value;
				OnActiveFilterEnabledChanged();
				OnProperyChanged();
			}
		}
		protected virtual void OnActiveFilterEnabledChanged() {
			FilterNodes();
			RaiseColumnFilterChanged();
		}
		[Browsable(false)]
		public virtual string FilterPanelText { get { return GetFilterDisplayText(ActiveFilterCriteria); } }
		public virtual void InvalidateFilterPanel() {
			if(!ViewInfo.IsValid || !ViewInfo.ShowFilterPanel) return;
			ViewInfo.PaintAnimatedItems = false;
			Invalidate(ViewInfo.FilterPanel.Bounds);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public TreeListFilterInfo ActiveFilterInfo { get { return activeFilterInfo; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), XtraSerializableProperty(true, false, true, 2147483645)]
		public TreeListFilterInfoCollection MRUFilters { get { return mruFilters; } }
		bool XtraShouldSerializeMRUFilters() { return MRUFilters.Count > 0; }
		void XtraClearMRUFilters(XtraItemEventArgs e) {
			MRUFilters.Clear();
		}
		object XtraCreateMRUFiltersItem(XtraItemEventArgs e) {
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo filterStringProperty = e.Item.ChildProperties["FilterString"];
			if(filterStringProperty == null) return null;
			String filterString = filterStringProperty.ValueToObject(typeof(String)) as String;
			TreeListFilterInfo filterInfo = new TreeListFilterInfo(filterString);
			MRUFilters.Add(filterInfo);
			return filterInfo;
		}
		protected internal TreeListMRUFilterPopup MRUFilterPopup {
			get { return mruFilterPopup; }
			set {
				if(MRUFilterPopup == value) return;
				if(MRUFilterPopup != null)
					MRUFilterPopup.Dispose();
				mruFilterPopup = value;
			}
		}
		protected internal TreeListColumnFilterPopupBase FilterPopup {
			get { return filterPopup; }
			set {
				if(FilterPopup == value) return;
				if(FilterPopup != null)
					FilterPopup.Dispose();
				filterPopup = value;
			}
		}
		protected internal void ShowMRUFilterPopup() {
			if(!ContainsFocus || !GetAllowMRUFilterList()) return;
			if(ViewInfo.FilterPanel.Bounds.IsEmpty) return;
			Rectangle tb = ViewInfo.FilterPanel.TextBounds, fb = ViewInfo.FilterPanel.MRUButtonInfo.Bounds;
			if(tb.IsEmpty) return;
			if(!fb.IsEmpty) {
				tb.Width = fb.Right - tb.X;
				tb.Y = Math.Min(tb.Y, fb.Y);
				tb.Height = Math.Max(tb.Bottom, fb.Bottom) - tb.Y;
			}
			MRUFilterPopup = new TreeListMRUFilterPopup(this);
			MRUFilterPopup.Init();
			if(MRUFilterPopup.CanShow)
				MRUFilterPopup.Show(tb);
			else
				MRUFilterPopup = null;
		}
		protected internal void ShowFilterPopup(TreeListColumn column) {
			if(!ContainsFocus || !GetAllowColumnFilterPopup(column) || !ViewInfo.IsValid) return;
			FilterPopup = CreateColumnFilterPopup(column);
			FilterPopup.Init();
			FilterPopup.OnBeforeShow();
			if(FilterPopup.CanShow)
				FilterPopup.Show(ViewInfo.GetFilterButtonBounds(column));
			else
				FilterPopup = null;
		}
		protected virtual TreeListColumnFilterPopupBase CreateColumnFilterPopup(TreeListColumn column) {
			FilterPopupMode mode = column.GetFilterPopupMode();
			if(mode == FilterPopupMode.Date)
				return new TreeListDateFilterPopup(this, column);
			if(mode == FilterPopupMode.CheckedList)
				return new TreeListCheckedColumnFilterPopup(this, column);
			return new TreeListColumnFilterPopup(this, column);
		}
		protected virtual bool GetAllowColumnFilterPopup(TreeListColumn column) {
			return column.OptionsFilter.AllowFilter;
		}
		protected void UpdateActiveFilterInfo(bool updateMRUList) {
			TreeListFilterInfo info = null;
			if(!object.Equals(ActiveFilterCriteria, null))
				info = new TreeListFilterInfo(ActiveFilterCriteria, GetFilterDisplayText(ActiveFilterCriteria));
			if(updateMRUList)
				MRUFilters.AddMRUFilter(ActiveFilterInfo, OptionsFilter.MRUFilterListPopupCount);
			this.activeFilterInfo = info;
			if(updateMRUList)
				MRUFilters.RemoveMRUFilter(ActiveFilterInfo);
		}
		protected internal virtual bool GetAllowMRUFilterList() {
			if(!OptionsBehavior.EnableFiltering || !OptionsFilter.AllowMRUFilterList) return false;
			if(MRUFilters.Count == 0) return false;
			if(MRUFilters.Count > 1) return true;
			return !object.Equals(MRUFilters[0].FilterCriteria, ActiveFilterCriteria);
		}
		protected internal virtual string GetFilterDisplayText(CriteriaOperator filter) {
			object result;
			ConvertEditValueEventArgs e = new ConvertEditValueEventArgs(filter);
			RaiseCustomFilterDisplayText(e);
			if(e.Handled) {
				result = e.Value;
			}
			else {
				using(FilterColumnCollection filterColumnCollection = CreateFilterColumnCollection()) {
					result = DisplayCriteriaGeneratorPathed.Process(filterColumnCollection, filter);
				}
			}
			CriteriaOperator criteriaResult = result as CriteriaOperator;
			if(!ReferenceEquals(criteriaResult, null))
				result = LocalaizableCriteriaToStringProcessor.Process(Localizer.Active, criteriaResult);
			if(result != null)
				return result.ToString();
			return string.Empty;
		}
		protected internal virtual FilterColumnCollection CreateFilterColumnCollection() {
			return new TreeListFilterColumnCollection(this);
		}
		protected internal virtual string GetFilterDisplayTextByColumn(TreeListColumn column, object val) {
			if(GetColumnFilterMode(column) == ColumnFilterMode.Value) {
				RepositoryItem item = GetColumnEdit(column);
				if(item != null) {
					BaseEditViewInfo editViewInfo = item.CreateViewInfo();
					if(editViewInfo != null) {
						editViewInfo.InplaceType = InplaceType.Grid;
						if(!column.Format.IsEmpty)
							editViewInfo.Format = column.Format;
						GetNodeDisplayValueEventArgs e = new GetNodeDisplayValueEventArgs(column, null) { Value = val };
						RaiseGetNodeDisplayValue(e);
						editViewInfo.EditValue = e.Value;
						return editViewInfo.DisplayText;
					}
				}
			}
			if(val != null)
				return val.ToString();
			return string.Empty;
		}
		protected virtual void RaiseCustomFilterDisplayText(ConvertEditValueEventArgs e) {
			ConvertEditValueEventHandler handler = (ConvertEditValueEventHandler)Events[customFilterDisplayText];
			if(handler != null)
				handler(this, e);
		}
		public virtual void ShowFilterEditor(TreeListColumn defaultColumn) {
			using(FilterColumnCollection filterColumns = CreateFilterColumnCollection()) {
				FilterColumn filterColumn = GetFilterColumnByTreeListColumn(filterColumns, defaultColumn);
				using(IFilterEditorForm form = CreateFilterEditorForm(filterColumns, filterColumn)) {
					FilterControlEventArgs e = new FilterControlEventArgs(form);
					RaiseFilterEditorCreated(e);
					if(e.ShowFilterEditor)
						form.ShowDialog(this);
				}
			}
		}
		protected virtual IFilterEditorForm CreateFilterEditorForm(FilterColumnCollection filterColumns, FilterColumn defaultFilterColumn) {
			return new FilterEditorForm(filterColumns, defaultFilterColumn, this);
		}
		protected internal virtual FilterColumn GetFilterColumnByTreeListColumn(FilterColumnCollection collection, TreeListColumn column) {
			if(column == null)
				return null;
			foreach(FilterColumn filterColumn in collection) {
				TreeListFilterColumn tlFilterColumn = filterColumn as TreeListFilterColumn;
				if(tlFilterColumn != null && column == tlFilterColumn.Column)
					return filterColumn;
			}
			return null;
		}
		protected virtual void RaiseFilterEditorCreated(FilterControlEventArgs e) {
			FilterControlEventHandler handler = (FilterControlEventHandler)this.Events[filterEditorCreated];
			if(handler != null)
				handler(this, e);
		}
		protected internal virtual bool GetAllowFilterEditor() {
			if(!OptionsBehavior.EnableFiltering || !OptionsFilter.AllowFilterEditor) return false;
			foreach(TreeListColumn column in Columns)
				if(column.OptionsFilter.AllowFilter) return true;
			return false;
		}
		protected virtual void OnActiveFilterCriteriaChanged() {
			UpdateFilter();
		}
		protected void UpdateFilter() {
			if(!IsDeserializing)
				activeFilterEnabled = true;
			FilterHelper.Reset();
			UpdateFilterInfo();
			UpdateActiveFilterInfo(lockFilterSetRowValue == 0);
			FilterNodes();
			RaiseColumnFilterChanged();
		}
		protected virtual void UpdateFilterInfo() {
			if(this.lockFilterSetRowValue != 0 || this.lockUpdateFilterInfo != 0) return;
			nonColumnFilterCriteria = null;
			foreach(TreeListColumn column in Columns)
				column.FilterInfo.Clear();
			if(ReferenceEquals(ActiveFilterCriteria, null)) return;
			foreach(KeyValuePair<OperandProperty, CriteriaOperator> affinity in CriteriaColumnAffinityResolver.SplitByColumns(ActiveFilterCriteria)) {
				string propertyName = affinity.Key.PropertyName;
				TreeListColumn column = !string.IsNullOrEmpty(propertyName) ? Columns[propertyName] : null;
				if(column != null)
					column.FilterInfo.Set(affinity.Value, FilterHelper.GetValueFromFilterCriteria(affinity.Value, ResolveAutoFilterCondition(column)));
				else
					nonColumnFilterCriteria = GroupOperator.And(nonColumnFilterCriteria, affinity.Value);
			}
		}
		protected CriteriaOperator GetColumnFilterCriteria(TreeListColumn column) {
			CriteriaOperator rv;
			CriteriaColumnAffinityResolver.SplitByColumns(ActiveFilterCriteria).TryGetValue(new OperandProperty(column.FieldName), out rv);
			return rv;
		}
		public virtual void FilterNodes() {
			FilterNodes(Nodes);
		}
		void ClearAndUpdateFilter() {
			BeginUpdate();
			try {
				ClearFilteringInternal();
				UpdateFilter();
			}
			finally {
				EndUpdate();
			}
		}
		internal void ClearFilteringInternal() {
			BeginUpdate();
			try {
				TreeListClearFilterNodesOperation op = new TreeListClearFilterNodesOperation();
				NodesIterator.DoOperation(op);
				ViewInfo.SummaryFooterInfo.NeedsRecalcAll = true;
			}
			finally {
				EndUpdate();
			}
		}
		public virtual void FilterNodes(TreeListNodes nodes) {
			if(AllowFilterNodes) {
				BeginUpdate();
				try {
					FilterNodesCore(nodes);
					ViewInfo.SummaryFooterInfo.NeedsRecalcAll = true;
				}
				finally {
					EndUpdate();
				}
			}
			LayoutChanged();
			if(ViewInfo.VisibleRowCount == 0 && TopVisibleNodeIndex > 0)
				TopVisibleNodeIndex = 0;
			CheckIncreaseVisibleRows();
		}
		void FilterNodesCore(TreeListNodes nodes) {
			foreach(TreeListNode node in nodes) {
				bool res = DoFilterNode(node);
				if(res && GetActualTreeListFilterMode() == FilterMode.Standard)
					continue;
				else
					FilterNodesCore(node.Nodes);
			}
		}
		protected internal FilterMode GetActualTreeListFilterMode() {
			if(OptionsFilter.FilterMode == FilterMode.Default)
				return IsLookUpMode ? FilterMode.Smart : FilterMode.Standard;
			return OptionsFilter.FilterMode;
		}
		public void ClearColumnsFilter() {
			ActiveFilterCriteria = null;
		}
		public void ClearColumnFilter(TreeListColumn column) {
			if(column == null) return;
			column.FilterInfo.Clear();
			ApplyColumnFilterInternal(column);
		}
		public void AddFilter(CriteriaOperator filterCriteria) {
			MergeColumnFiltersCore(filterCriteria);
		}
		public void AddFilter(string filterString) {
			AddFilter(CriteriaOperator.TryParse(filterString));
		}
		protected internal TreeListAutoFilterNode AutoFilterNode { get { return autoFilterNode; } }
		protected virtual object GetFilterRowValue(object columnId) {
			if(columnId == null) return null;
			TreeListColumn column = Data.DataHelper.GetTreeListColumnByID(columnId);
			if(column == null) return null;
			return column.FilterInfo.AutoFilterRowValue;
		}
		int lockFilterSetRowValue = 0;
		int lockUpdateFilterInfo = 0;
		protected virtual void SetFilterRowValue(object columnId, object value) {
			if(columnId == null || this.lockFilterSetRowValue != 0) return;
			TreeListColumn column = Data.DataHelper.GetTreeListColumnByID(columnId);
			if(column == null) return;
			this.lockFilterSetRowValue++;
			try {
				if(ActiveEditor != null) {
					ActiveEditor.ErrorText = "";
					EditorHelper.HideHint();
				}
				if(value == DBNull.Value) value = null;
				try {
					allowCloseEditor = false;
					UpdateColumnAutoFilter(column, value);
				}
				finally {
					allowCloseEditor = true;
				}
				UpdateAutoFilterRowActiveEditor();
			}
			catch(Exception e) {
				if(ActiveEditor != null && IsFilterRow(FocusedRowIndex)) {
					string text = e.Message;
					if(text == null || text.Length == 0) text = "Filter Error";
					EditorHelper.ShowEditorError(e.Message);
				}
				else {
					XtraMessageBox.Show(ElementsLookAndFeel, FindForm(), e.Message, "Filter Error");
				}
			}
			finally {
				this.lockFilterSetRowValue--;
			}
		}
		protected void UpdateAutoFilterRowActiveEditor() {
			if(ActiveEditor == null || !TreeListAutoFilterNode.IsAutoFilterNode(FocusedNode) || FocusedColumn == null) return;
			if(!ViewInfo.IsValid) return;
			CellInfo cell = ViewInfo.AutoFilterRowInfo[FocusedColumn];
			if(cell != null) {
				Rectangle bounds = ViewInfo.GetEditorBounds(cell);
				if(bounds.IsEmpty) {
					HideEditor();
					return;
				}
				if(ActiveEditor.Bounds == bounds) return;
				ActiveEditor.Bounds = bounds;
			}
		}
		protected virtual void UpdateColumnAutoFilter(TreeListColumn column, object value) {
			string strVal = value == null ? null : value.ToString();
			if(value == null || strVal == string.Empty) {
				if(column.FilterInfo.IsEmpty)
					return;
				column.FilterInfo.Clear();
			}
			else {
				AutoFilterCondition condition = ResolveAutoFilterCondition(column);
				CriteriaOperator op = CreateAutoFilterCriterion(column, condition, value, strVal);
				if(Equals(column.FilterInfo.FilterCriteria, op) && column.FilterInfo.AutoFilterRowValue == value)
					return;
				column.FilterInfo.Set(op, value);
			}
			ApplyColumnFilter(column);
		}
		internal void ApplyColumnFilterInternal(TreeListColumn column) {
			this.lockUpdateFilterInfo++;
			try {
				ApplyColumnFilter(column);
			}
			finally {
				this.lockUpdateFilterInfo--;
			}
		}
		protected void ApplyColumnFilter(TreeListColumn column) {
			if(!column.FilterInfo.IsEmpty)
				MergeColumnFiltersCore(column.FilterInfo.FilterCriteria);
			else
				ClearColumnFilterCore(column.FieldName);
		}
		protected void ClearColumnFilterCore(string fieldName) {
			IDictionary<OperandProperty, CriteriaOperator> splitted = CriteriaColumnAffinityResolver.SplitByColumns(ActiveFilterCriteria);
			splitted.Remove(new OperandProperty(fieldName));
			ActiveFilterCriteria = GroupOperator.And(splitted.Values);
		}
		protected void MergeColumnFiltersCore(CriteriaOperator filterCriteria) {
			Dictionary<OperandProperty, CriteriaOperator> merged = new Dictionary<OperandProperty, CriteriaOperator>(CriteriaColumnAffinityResolver.SplitByColumns(ActiveFilterCriteria));
			IDictionary<OperandProperty, CriteriaOperator> newFilter = CriteriaColumnAffinityResolver.SplitByColumns(filterCriteria);
			foreach(var affinity in newFilter)
				merged[affinity.Key] = affinity.Value;
			ActiveFilterCriteria = GroupOperator.And(merged.Values);
		}
		protected virtual CriteriaOperator CreateAutoFilterCriterion(TreeListColumn column, AutoFilterCondition condition, object value, string strVal) {
			if(condition == AutoFilterCondition.Equals)
				return FilterHelper.CreateColumnFilterCriteriaByValue(Data.GetDataColumnInfo(column.FieldName), value, GetColumnFilterMode(column) == ColumnFilterMode.DisplayText, IsRoundDateTime(column), null);
			if(string.IsNullOrEmpty(strVal))
				return null;
			OperandProperty prop = new OperandProperty(column.FieldName);
			if(condition == AutoFilterCondition.Contains)
				return new FunctionOperator(FunctionOperatorType.Contains, prop, strVal);
			else if(strVal[0] == '_' || strVal[0] == '%' || strVal[0] == '*' || strVal[0] == '%')
				return new FunctionOperator(FunctionOperatorType.Contains, prop, strVal.Substring(1));
			else
				return new FunctionOperator(FunctionOperatorType.StartsWith, prop, strVal);
		}
		protected AutoFilterCondition ResolveAutoFilterCondition(TreeListColumn column) {
			RepositoryItem item = GetColumnEdit(column);
			AutoFilterCondition condition = column.OptionsFilter.AutoFilterCondition;
			if(condition == AutoFilterCondition.Default) condition = AutoFilterCondition.Like;
			if(GetColumnFilterMode(column) == ColumnFilterMode.DisplayText) {
				if(condition == AutoFilterCondition.Contains) return condition;
				return AutoFilterCondition.Like;
			}
			if(item is RepositoryItemCheckEdit || item is RepositoryItemDateEdit || item is RepositoryItemLookUpEditBase || item is RepositoryItemImageComboBox)
				condition = AutoFilterCondition.Equals;
			return condition;
		}
		protected internal ColumnFilterMode GetColumnFilterMode(TreeListColumn column) {
			RepositoryItem item = GetColumnEdit(column);
			if(item is RepositoryItemCheckEdit) return ColumnFilterMode.Value;
			return column.FilterMode;
		}
		protected virtual void OnFilterRowValueChanging(TreeListColumn column, object value) {
			if(column == null) return;
			if(!column.OptionsFilter.ImmediateUpdateAutoFilter) return;
			try {
				Data.DataHelper.Posting = true;
				SetFilterRowValue(column, value);
			}
			finally {
				Data.DataHelper.Posting = false;
			}
		}
		protected bool IsFilterRow(int index) {
			return index == TreeList.AutoFilterNodeId;
		}
		protected virtual bool IsColumnAllowAutoFilter(TreeListColumn column) {
			if(column == null) return false;
			if(!OptionsBehavior.EnableFiltering) return false;
			RepositoryItem item = GetColumnEdit(column);
			if(item is RepositoryItemPictureEdit || item is RepositoryItemImageEdit) return false;
			return column.OptionsFilter.AllowAutoFilter;
		}
		protected internal virtual bool IsRoundDateTime(TreeListColumn column) {
			return GetColumnEdit(column) is RepositoryItemDateEdit;
		}
		protected internal virtual void OnFilterModeChanged() {
			UpdateFilter();
		}
		protected internal virtual void OnSortModeChanged() {
			DoSort(Nodes, true);
		}
		protected virtual void RaiseColumnFilterChanged() {
			OnFilterChanged();
			EventHandler handler = (EventHandler)Events[columnFilterChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseFilterPopupDate(TreeListDateFilterPopup filterPopup, List<FilterDateElement> list) {
			FilterPopupDateEventHandler handler = (FilterPopupDateEventHandler)Events[showFilterPopupDate];
			if(handler != null) handler(this, new FilterPopupDateEventArgs(filterPopup.Column, list));
		}
		protected internal virtual void RaiseColumnFilterPopupEvent(TreeListColumnFilterPopup filterPopup) {
			FilterPopupListBoxEventHandler handler = (FilterPopupListBoxEventHandler)Events[showFilterPopupListBox];
			if(handler != null) handler(this, new FilterPopupListBoxEventArgs(filterPopup.Column, filterPopup.Item));
		}
		protected internal virtual void RaiseCheckedColumnFilterPopupEvent(TreeListCheckedColumnFilterPopup filterPopup) {
			FilterPopupCheckedListBoxEventHandler handler = (FilterPopupCheckedListBoxEventHandler)this.Events[showFilterPopupCheckedListBox];
			if(handler != null) handler(this, new FilterPopupCheckedListBoxEventArgs(filterPopup.Column, filterPopup.Item));
		}
		protected internal virtual FilterButtonShowMode GetHeaderFilterButtonShowMode() {
			return OptionsView.HeaderFilterButtonShowMode == FilterButtonShowMode.Default ? FilterButtonShowMode.SmartTag : OptionsView.HeaderFilterButtonShowMode;
		}
		void ClearDateFilterCache() {
			dateFilterCache.Clear();
		}
		internal void SaveDateFilterInfo(TreeListColumn column, TreeListDateFilterInfoCache info) {
			dateFilterCache[column] = info;
		}
		internal TreeListDateFilterInfoCache GetCachedDateFilterInfo(TreeListColumn column) {
			if(!dateFilterCache.ContainsKey(column)) return null;
			return dateFilterCache[column];
		}
		#region IFilteredComponent
		EventHandler filterChanged;
		EventHandler filterDataSourceChanged;
		DevExpress.Data.Filtering.CriteriaOperator IFilteredComponentBase.RowCriteria { get { return ActiveFilterCriteria; } set { ActiveFilterCriteria = value; } }
		event EventHandler IFilteredComponentBase.RowFilterChanged {
			add { filterChanged += value; }
			remove { filterChanged -= value; }
		}
		event EventHandler IFilteredComponentBase.PropertiesChanged {
			add { filterDataSourceChanged += value; }
			remove { filterDataSourceChanged -= value; }
		}
		IBoundPropertyCollection IFilteredComponent.CreateFilterColumnCollection() {
			return new TreeListFilterColumnCollection(this);
		}
		void OnFilterChanged() {
			if(filterChanged != null) filterChanged(this, EventArgs.Empty);
		}
		void OnFilterDataSourceChanged() {
			if(IsLoading) return;
			if(filterDataSourceChanged != null) filterDataSourceChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		#region FindPanel
		FindPanel findPanel;
		CriteriaOperator findFilterCriteria = null;
		bool findPanelVisible;
		FindSearchParserResults findFilterParserResults = null;
		ICollection findPanelMruItems;
		protected internal CriteriaOperator FindFilterCriteria { get { return findFilterCriteria; } }
		protected internal FindPanel FindPanel { get { return findPanel; } }
		protected internal bool IsFindFilterActive { get { return !string.IsNullOrEmpty(FindFilterText); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty]
		public virtual bool FindPanelVisible {
			get { return findPanelVisible; }
			set {
				if(value) value = CheckAllowFindPanel();
				if(FindPanelVisible == value) return;
				findPanelVisible = value;
				OnIsFindPanelVisibleChanged();
			}
		}
		bool CheckAllowFindPanel() {		  
			if(IsAttachedToSearchControl) return false;
			return true;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), XtraSerializableProperty]
		public string FindFilterText {
			get { return findFilterText; }
			set {
				if(IsDeserializing || IsLoading)
					this.findFilterText = value;
				else
					ApplyFindFilter(value);
				OnProperyChanged();
			}
		}
		public bool ApplyFindFilter(string filter) {
			if(filter == null)
				filter = string.Empty;
			if(FindFilterText == filter)
				return false;		   
			if(FindPanelVisible && FindPanel != null)
				FindPanel.SetFilterText(filter);
			findFilterText = filter;
			ParseFindFilterText();
			FilterHelper.Reset();
			if(AllowFilterNodes) 
				FilterNodes();
			else 
				Invalidate();
			RaiseColumnFilterChanged();
			return true;
		}
		void ParseFindFilterText() {
			this.findFilterParserResults = null;
			this.findFilterCriteria = null;
			if(IsLoading || IsDeserializing) return;
			if(IsFindFilterActive) {
				this.findFilterParserResults = new FindSearchParser().Parse(FindFilterText);
				this.findFilterCriteria = DxFtsContainsHelper.Create(GetFindColumnsNames(), findFilterParserResults, DevExpress.Data.Filtering.FilterCondition.Contains);
			}
		}
		protected virtual string[] GetFindColumnsNames() {
			List<string> res = new List<string>();
			foreach(TreeListColumn column in VisibleColumns)
				if(GetAllowFindColumn(column)) res.Add(string.Concat(DxFtsContainsHelper.DxFtsPropertyPrefix, column.FieldName));
			return res.ToArray();
		}
		protected internal virtual bool GetAllowHighlightFindResults(TreeListColumn column) {
			if(!IsFindFilterActive) return false;
			if(OptionsFind.HighlightFindResults && GetAllowFindColumn(column)) return true;
			return false;
		}
		protected virtual bool GetAllowFindColumn(TreeListColumn column) {
			if(string.IsNullOrEmpty(column.FieldName)) return false;
			if(column.ColumnEdit is RepositoryItemPictureEdit || column.ColumnEdit is RepositoryItemImageEdit) return false;
			string findFilterColumns = OptionsFind.FindFilterColumns;
			if(UseSearchInfo && !string.IsNullOrEmpty(searchInfo.Columns))
				findFilterColumns = searchInfo.Columns;
			if(findFilterColumns == TreeListOptionsFind.DefaultFilterColumnsString) return true;
			return string.Concat(";", findFilterColumns, ";").Contains(string.Concat(";", column.FieldName, ";"));
		}
		protected internal string GetFindMatchedText(TreeListColumn column, string displayText) {
			if(findFilterParserResults == null) return string.Empty;
			return findFilterParserResults.GetMatchedText(column.FieldName, displayText);
		}
		void OnIsFindPanelVisibleChanged() {
			if(FindPanelVisible)
				CreateFindPanel();
			else
				DestroyFindPanel();
			LayoutChanged();
		}
		public void ShowFindPanel() {
			FindPanelVisible = true;
			if(FindPanel != null)
				FindPanel.Owner.FocusFindEditor();
		}
		public void HideFindPanel() {
			FindPanelVisible = false;
			if(OptionsFind.ClearFindOnClose) ApplyFindFilter("");
			Focus();
		}
		protected virtual FindPanel CreateFindPanelCore() {
			return new FindPanel(CreateFindPanelOwner());
		}
		protected virtual TreeListFindPanelOwner CreateFindPanelOwner() {
			return new TreeListFindPanelOwner(this);
		}
		protected virtual void InitFindPanel(FindPanel findPanel) {
			findPanel.SetFilterText(FindFilterText);
			RestoreFindPanelMruItems();
			UpdateFindPanelFindMode();
		}
		void UpdateFindPanelFindMode() {
			FindPanel.AllowAutoApply = (OptionsFind.FindMode == FindMode.Always || OptionsFind.FindMode == FindMode.Default);
		}
		protected void CreateFindPanel() {
			if(IsAttachedToSearchControl) return;
			this.findPanel = CreateFindPanelCore();
			if(FindPanel != null) {
				InitFindPanel(FindPanel);
				Controls.Add(FindPanel);
			}
		}
		protected void DestroyFindPanel() {
			if(FindPanel != null) {
				SaveFindPanelMruItems();
				FindPanel.Dispose();
			}
			this.findPanel = null;
		}
		void RestoreFindPanelMruItems() {
			if(findPanelMruItems != null)
				FindPanel.FindEdit.Properties.Items.AddRange(findPanelMruItems);
		}
		void SaveFindPanelMruItems() {
			findPanelMruItems = FindPanel.FindEdit.Properties.Items;
		}
		internal bool TryCloseActiveEditor() {
			if(ActiveEditor == null) return true;
			ContainerHelper.BeginAllowHideException();
			try {
				CloseEditor();
			}
			catch(HideException) {
				return false;
			}
			finally {
				ContainerHelper.EndAllowHideException();
			}
			return true;
		}
		protected override void OnParentChanged(EventArgs e) {
			if(IsLookUpMode && IsDesignMode && Parent != null)
				Parent = null;
			base.OnParentChanged(e);
		}
		#endregion
		#region Animation
		Control ISupportXtraAnimation.OwnerControl { get { return this; } }
		bool ISupportXtraAnimation.CanAnimate { get { return true; } }
		protected internal virtual TreeListAnimationType GetActualAnimationType() {
			if(OptionsView.AnimationType == TreeListAnimationType.Default) return TreeListAnimationType.AnimateFocusedNode;
			return OptionsView.AnimationType;
		}
		#endregion
		#region CheckBoxes
		public void SetNodeCheckState(TreeListNode node, CheckState state) {
			SetNodeCheckState(node, state, false);
		}
		public void SetNodeCheckState(TreeListNode node, CheckState state, bool recursive) {
			node.CheckState = state;
			if(recursive) {
				BeginUpdate();
				try {
					UpdateChildNodesCheckState(node, node.CheckState);
					UpdateParentNodesCheckState(node, node.CheckState);
				}
				finally {
					EndUpdate();
				}
			}
		}
		public List<TreeListNode> GetAllCheckedNodes() {
			List<TreeListNode> nodes = new List<TreeListNode>();
			NodesIterator.DoOperation((node) =>
			{
				if(node.Checked)
					nodes.Add(node);
			});
			return nodes;
		}
		public void CheckAll() {
			CheckAllCore(true);
		}
		public void UncheckAll() {
			CheckAllCore(false);
		}
		protected void CheckAllCore(bool check) {
			NodesIterator.DoOperation((node) => { node.Checked = check; });
		}
		protected void UpdateChildNodesCheckState(TreeListNode node, CheckState check) {
			for(int i = 0; i < node.Nodes.Count; i++) {
				node.Nodes[i].CheckState = check;
				UpdateChildNodesCheckState(node.Nodes[i], check);
			}
		}
		protected void UpdateParentNodesCheckState(TreeListNode node, CheckState check) {
			if(node.ParentNode != null) {
				bool b = false;
				CheckState state;
				for(int i = 0; i < node.ParentNode.Nodes.Count; i++) {
					state = (CheckState)node.ParentNode.Nodes[i].CheckState;
					if(!check.Equals(state)) {
						b = !b;
						break;
					}
				}
				node.ParentNode.CheckState = b ? CheckState.Indeterminate : check;
				UpdateParentNodesCheckState(node.ParentNode, check);
			}
		}
		#endregion
		#region CopyPaste
		TreeListOptionsClipboard optionsClipboard;
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsClipboard"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public TreeListOptionsClipboard OptionsClipboard { get { return optionsClipboard; } }
		ClipboardTreeListImplementer treeListImplementerCore = null;
		ClipboardTreeListImplementer treeListImplementer {
			get {
				if(treeListImplementerCore == null) treeListImplementerCore = new ClipboardTreeListImplementer(this);
				return treeListImplementerCore;
			}
		}
		IClipboardManager<TreeListColumnImplementer, TreeListNodeImplementer> exportManagerCore = null;						
		IClipboardManager<TreeListColumnImplementer, TreeListNodeImplementer> ClipboardManager {
			get {
				if(exportManagerCore == null) exportManagerCore = ClipboardHelper<TreeListColumnImplementer, TreeListNodeImplementer>.GetManager(treeListImplementer, optionsClipboard);
				return exportManagerCore;
			}
		}
		public void CopyToClipboard() {
			BeginShowProgress();
			if(SetDataAwareClipboardData()) {
				EndShowProgress();
				return;
			} else new TreeListClipboardHelper(this).CopyToClipboard();
			EndShowProgress();
		}
		void EndShowProgress() {
			if(ProgressWindow != null) {
				ProgressWindow.Dispose();
				ProgressWindow = null;
			}
		}
		void BeginShowProgress() {
			switch(OptionsClipboard.ShowProgress) {
				case ProgressMode.Automatic:
					if(treeListImplementer.GetSelectedCellsCount() > 10000)
						treeListImplementer.ShowProgress = true;
					else treeListImplementer.ShowProgress = false;
					break;
				case ProgressMode.Always:
					treeListImplementer.ShowProgress = true;
					break;
				case ProgressMode.Never:
					treeListImplementer.ShowProgress = false;
					break;
			}
			if(treeListImplementer.ShowProgress) ShowProgressFormForClipboard();
		}
		internal void ShowProgressFormForClipboard() {
			Form form = FindForm();
			if(form == null) return;
			ProgressWindow = new ProgressWindow();
			ProgressWindow.LookAndFeel.Assign(ElementsLookAndFeel);
			ProgressWindow.SetCaption(XtraPrinting.PrintingSystemActivity.Exporting);
			ProgressWindow.DisableCancel();
			ProgressWindow.ShowCenter(form);
		}
		bool SetDataAwareClipboardData() {
			try {
				if(OptionsClipboard.ClipboardMode != ClipboardMode.Formatted || !treeListImplementer.CanCopyToClipboard())
					return false;
				DataObject data = new DataObject();
				ClipboardManager.AssignOptions(OptionsClipboard);
				ClipboardManager.SetClipboardData(data);
				if(data.GetFormats().Count() == 0) return false;
				Clipboard.SetDataObject(data);
				return true;
			} catch {
				return false;
			}
		}
		#endregion
		#region LookUp
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsLookUpMode { get { return LookUpOwner != null; } }
		protected internal RepositoryItemTreeListLookUpEdit LookUpOwner { get; set; }
		protected internal string ExtraFilter { get; private set; }
		protected internal string ExtraFilterHightlightText { get; private set; }
		protected internal CriteriaOperator ExtraFilterCriteria {
			get {
				if(!string.IsNullOrEmpty(ExtraFilter))
					return CriteriaOperator.Parse(ExtraFilter);
				return null;
			}
		}
		protected internal TreeListColumn LookUpDisplayColumn { get { return LookUpOwner == null ? null : Columns[LookUpOwner.DisplayMember]; } }
		protected internal void SetExtraFilter(string text) {
			if(LookUpDisplayColumn == null || Data.Columns[LookUpOwner.DisplayMember] == null) return;
			if(text == null) text = string.Empty;
			ExtraFilterHightlightText = text;
			string filterString = string.Empty;
			if(text != string.Empty) {
				FunctionOperatorType opType = LookUpOwner.GetActualFilterMode() == PopupFilterMode.StartsWith ? FunctionOperatorType.StartsWith : FunctionOperatorType.Contains;
				CriteriaOperator op = new FunctionOperator(opType, new OperandProperty(LookUpOwner.DisplayMember), text);
				filterString = CriteriaOperator.ToString(op);
			}
			if(filterString == ExtraFilter)
				return;
			ExtraFilter = filterString;
			FilterHelper.Reset();
			FilterNodes();
		}
		internal void SetLookUpFocusedNode(TreeListNode node) {
			FocusedNode = node;
		}
		internal void CreateHandleCore() {
			if(IsHandleCreated) return;
			CreateHandle();
		}
		internal void AssignLookUpPropertiesFrom(TreeList source) {
			if(source == null) return;
			BeginUpdate();
			try {
				AssignLookUpColumns(source);
				AssignLookUpProperties(source);
				AssignLookUpEvents(source);
			}
			finally {
				EndUpdate();
			}
		}
		void AssignLookUpColumns(TreeList source) {
			source.Columns.AssignTo(Columns);
			RefreshSortColumns();
		}
		void AssignLookUpProperties(TreeList source) {
			Appearance.AssignInternal(source.Appearance);
			AppearancePrint.AssignInternal(source.AppearancePrint);
			ViewInfo.SetAppearanceDirty();
			FormatConditions.Assign(source.FormatConditions);
			FilterConditions.Assign(source.FilterConditions);
			ShowButtonMode = source.ShowButtonMode;
			ColumnPanelRowHeight = source.ColumnPanelRowHeight;
			ColumnsImageList = source.ColumnsImageList;
			SelectImageList = source.SelectImageList;
			StateImageList = source.StateImageList;
			CustomizationFormBounds = source.CustomizationFormBounds;
			FixedLineWidth = source.FixedLineWidth;
			FooterPanelHeight = source.FooterPanelHeight;
			IndicatorWidth = source.IndicatorWidth;
			PreviewFieldName = source.PreviewFieldName;
			PreviewLineCount = source.PreviewLineCount;
			RowHeight = source.RowHeight;
			HorzScrollStep = source.HorzScrollStep;
			HtmlImages = source.HtmlImages;
			BestFitVisibleOnly = source.BestFitVisibleOnly;
			HorzScrollVisibility = source.HorzScrollVisibility;
			VertScrollVisibility = source.VertScrollVisibility;			
			PreviewFieldName = source.PreviewFieldName;
			ImageIndexFieldName = source.ImageIndexFieldName;
			KeyFieldName = source.KeyFieldName;
			ParentFieldName = source.ParentFieldName;
			RootValue = source.RootValue;
			TreeLevelWidth = source.TreeLevelWidth;
			TreeLineStyle = source.TreeLineStyle;
			ActiveFilterCriteria = source.ActiveFilterCriteria;
			FindFilterText = source.FindFilterText;
			ActiveFilterEnabled = source.ActiveFilterEnabled;
			OptionsView.Assign(source.OptionsView);
			OptionsBehavior.Assign(source.OptionsBehavior);
			OptionsNavigation.Assign(source.OptionsNavigation);
			OptionsCustomization.Assign(source.OptionsCustomization);
			OptionsFilter.Assign(source.OptionsFilter);
			OptionsFind.Assign(source.OptionsFind);
			OptionsSelection.Assign(source.OptionsSelection);
			OptionsLayout.Assign(source.OptionsLayout);
			OptionsMenu.Assign(source.OptionsMenu);
			AllowDrop = source.AllowDrop;
			Caption = source.Caption;
			CaptionHeight = source.CaptionHeight;
			Tag = source.Tag;
		}
		void AssignLookUpEvents(TreeList source) {
			object[] eventKeys = new object[] {
				  columnChanged, getStateImage, getSelectImage, getPreviewText, measurePreviewHeight, getCustomSummaryValue, customNodeCellEdit, nodeCellStyle, getNodeDisplayValue, beforeExpand, beforeCollapse, 
				  beforeDragNode, beforeFocusNode, afterExpand, afterCollapse, afterDragNode, afterFocusNode, calcNodeHeight, calcNodeDragImageIndex, nodeChanged,focusedNodeChanged, focusedColumnChanged,
				  columnWidthChanged, compareNodeValues, selectionChanged, stateChanged, dragCancelNode, showCustomizationForm, hideCustomizationForm, columnButtonClick, customDrawNodeIndicator, customDrawNodeImages, 
				  customDrawColumnHeader, customDrawNodePreview, customDrawNodeButton, customDrawNodeCell, customDrawFooter, customDrawRowFooter, customDrawFooterCell, customDrawRowFooterCell, customDrawEmptyArea,
				  customDrawNodeIndent, customDrawNodeCheckBox, customDrawFilterPanel, treelistMenuItemClick,showTreeListMenu, onPopupMenuShowing, leftCoordChanged, topVisibleNodeIndexChanged,
				  stateImageClick, selectImageClick, nodesReloaded, createCustomNode, startSorting, endSorting, layoutUpdated, layoutUpgrade, beforeLoadLayout, filterNode, dragObjectStart, dragObjectOver, dragObjectDrop, beforeCheckNode,
				  afterCheckNode, customFilterDisplayText, filterEditorCreated, columnFilterChanged, showFilterPopupListBox, showFilterPopupCheckedListBox, showFilterPopupDate, customUnboundColumnData, columnUnboundExpressionChanged
			};
			foreach(object eventKey in eventKeys)
				Events.AddHandler(eventKey, source.Events[eventKey]);
		}
		protected virtual void OnProperyChanged() {
			if(LookUpOwner != null && !IsDeserializing)
				LookUpOwner.OnTreeListPropertyChanged();
		}
		#endregion
		#region Bands
		protected internal virtual bool ActualShowBands { get { return OptionsView.ShowBandsMode == DefaultBoolean.True || OptionsView.ShowBandsMode == DefaultBoolean.Default && HasBands; } }
		protected internal bool HasBands { get { return Bands.Count > 0; } }
		protected internal virtual bool AllowBandColumnsMultiRow { get { return OptionsView.AllowBandColumnsMultiRow && HasBands; } }
		protected internal virtual bool UseBandsAdvancedHorizontalNavigation { get { return OptionsNavigation.UseBandsAdvHorzNavigation; } }
		protected internal virtual bool UseBandsAdvancedVerticalNavigation { get { return OptionsNavigation.UseBandsAdvVertNavigation; } }
		protected virtual void OnAllowBandColumnMultiRowChanged() {
			DoForEachBand(Bands, (band) => { if(band.Columns.Count > 0) UpdateBandColumnsOrder(band); }, true);
		}
		protected internal virtual void NormalizeVisibleBandIndices() {
			if(calculatingLayout) return;
			Dictionary<TreeListBand, TreeListColumn> dict = new Dictionary<TreeListBand, TreeListColumn>();
			VisibleColumns.Clear();
			foreach(TreeListColumn column in Columns) {
				if(column.ParentBand == null || column.ParentBand.TreeList == null || !column.ParentBand.ActualVisible) continue;
				dict[column.ParentBand] = column;
			}
			if(dict.Count == 0) return;
			List<TreeListBand> bands = new List<TreeListBand>(dict.Keys);
			bands.Sort(new BandIndexComparer());
			int index = 0;
			foreach(TreeListBand band in bands) {
				foreach(TreeListColumn column in band.Columns) {
					if(column.Visible) {
						VisibleColumns.Add(column);
						column.visibleIndex = index;
						index++;
						if(AllowBandColumnsMultiRow)
							UpdateBandColumnsIndices(GetBandRows(band));
					}
				}
			}
		}
		protected internal void DoForEachBand(TreeListBandCollection bands, Action<TreeListBand> action, bool includeInvisible = false) {
			foreach(TreeListBand band in bands) {
				if(!band.Visible && !includeInvisible) continue;
				action(band);
				DoForEachBand(band.Bands, action, includeInvisible);
			}
		}
		protected internal virtual bool GetAllowChangeBandParent() { return OptionsCustomization.AllowChangeBandParent; }
		protected internal virtual bool CanShowBandsInCustomizationForm { get { return OptionsCustomization.ShowBandsInCustomizationForm; } }
		protected internal virtual bool CanResizeColumn(TreeListColumn column) {
			return OptionsCustomization.AllowColumnResizing && column.OptionsColumn.AllowSize;
		}
		protected internal virtual bool CanResizeBand(TreeListBand band) {
			return OptionsCustomization.AllowBandResizing && band.OptionsBand.AllowSize;
		}
		protected internal virtual bool CanMoveColumn(TreeListColumn column) {
			return OptionsCustomization.AllowColumnMoving && column.OptionsColumn.AllowMove;
		}
		protected internal virtual bool CanMoveBand(TreeListBand band) {
			return OptionsCustomization.AllowBandMoving && band.OptionsBand.AllowMove;
		}
		protected internal virtual void OnBandChanged(TreeListBand band) {
			LayoutChanged();
		}
		protected internal virtual void OnBandFixedChanged(TreeListBand band) {
			if(IsLoading) return;
			UpdateRootBands();
		}
		protected virtual void UpdateRootBands() {
			if(IsLoading || Bands.Count == 0) return;
			List<TreeListBand> bands = new List<TreeListBand>(Bands);
			bands.Sort(new BandVisibleIndexComparer());
			if(!Bands.SequenceEqual<TreeListBand>(bands)) 
				Bands.Set(bands);
			ViewInfo.UpdateFixedColumnInfo();
		}
		protected internal virtual void OnColumnRowIndexChanged(TreeListColumn column) {
			if(column.ParentBand == null || IsLoading || IsDeserializing) return;
			UpdateBandColumnsOrder(column.ParentBand);
			RefreshVisibleColumnsList();
		}
		protected virtual void UpdateBandColumnsIndices(TreeListBandRowCollection columnRows) {
			for(int i = 0; i < columnRows.Count; i++) {
				TreeListBandRow row = columnRows[i];
				for(int k = 0; k < row.Columns.Count; k++) {
					TreeListColumn column = row.Columns[k];
					column.SetRowIndex(i);
				}
			}
			ViewInfo.RC.NeedsRestore = true;
		}
		protected internal virtual void UpdateBandColumnsOrder(TreeListBand band) {
			if(!AllowBandColumnsMultiRow || IsLoading || IsDeserializing) return;
			UpdateBandColumnsIndices(GetBandRows(band, true));
			band.Columns.Sort(new BandColumnRowIndexComparer());
		}
		protected internal virtual TreeListBandRowCollection GetBandRows(TreeListBand band, bool includeInvisible = false) {
			return GetBandRowsCore(band, (column) => { return includeInvisible || column.Visible; });
		}
		protected internal virtual TreeListBandRowCollection GetBandRowsCore(TreeListBand band, Func<TreeListColumn, bool> getColumnVisible) {
			TreeListBandRowCollection rows = new TreeListBandRowCollection();
			Dictionary<int, TreeListBandRow> tempRows = new Dictionary<int, TreeListBandRow>();
			for(int i = 0; i < band.Columns.Count; i++) {
				TreeListColumn column = band.Columns[i];
				if(!getColumnVisible(column))
					continue;
				int rowIndex = column.RowIndex;
				if(!tempRows.ContainsKey(rowIndex))
					tempRows[rowIndex] = new TreeListBandRow();
				tempRows[rowIndex].Columns.Add(column);
			}
			foreach(int index in tempRows.Keys.OrderBy(i => i))
				rows.Add(tempRows[index]);
			return rows;
		}
		protected internal virtual TreeListBandRowCollection GetFocusableBandRows(TreeListBand band) {
			TreeListBandRowCollection rows = GetBandRows(band);
			for(int i = rows.Count - 1; i >= 0; i--) {
				TreeListBandRow row = rows[i];
				for(int n = row.Columns.Count - 1; n >= 0; n--) {
					if(!GetAllowFocus(row.Columns[n]))
						row.Columns.RemoveAt(n);
				}
				if(row.Columns.Count == 0)
					rows.RemoveAt(i);
			}
			return rows;
		}
		protected internal virtual int GetBandColumnIndex(TreeListColumn column) {
			if(IsLoading || column.ParentBand == null) return -1;
			TreeListBandRowCollection rows = GetBandRows(column.ParentBand, false);
			if(rows == null) return -1;
			TreeListBandRow row = rows.FindRow(column);
			if(row == null) return -1;
			return row.Columns.IndexOf(column);
		}
		public virtual void SetColumnPosition(TreeListColumn column, int rowIndex, int columnIndex) {
			if(column.ParentBand == null) return;
			bool hasChanges = false;
			BeginUpdate();
			try {
				LockRaiseColumnPositionChanged();
				if(!column.Visible) {
					column.Visible = true;
					hasChanges = true;
				}
				TreeListBandRowCollection rows = GetBandRows(column.ParentBand, true);
				if(column.RowIndex != rowIndex || columnIndex == -1) {
					TreeListBandRow currentRow = rows.FindRow(column);
					if(currentRow != null) 
						currentRow.Columns.Remove(column);
					if(rowIndex >= 0 && rowIndex < rows.Count && columnIndex != -1) {
						TreeListBandRow targetRow = rows[rowIndex];
						column.SetRowIndex(rowIndex);
						targetRow.Columns.SetColumnIndex(columnIndex, column);
					}
					else {
						TreeListBandRow targetRow = new TreeListBandRow();
						if(rowIndex < rows.Count) {
							rows.Insert(rowIndex, targetRow);
							column.SetRowIndex(rowIndex);
						}
						else {
							column.SetRowIndex(rows.Add(targetRow));
						}
						targetRow.Columns.Add(column);
					}
					column.ParentBand.Columns.Set(rows);
					UpdateBandColumnsIndices(rows);
					hasChanges = true;
				}
				else {
					TreeListBandRow row = rows.FindRow(column);
					if(row == null) return;
					if(columnIndex < 0) columnIndex = 0;
					if(columnIndex > row.Columns.Count) columnIndex = row.Columns.Count;
					int currentIndex = row.Columns.IndexOf(column);
					if(columnIndex != currentIndex) {
						row.Columns.SetColumnIndex(columnIndex, column);
						column.ParentBand.Columns.Set(rows);
						hasChanges = true;
					}
				}
			}
			finally {
				UnlockRaiseColumnPositionChanged();
				if(hasChanges) {
					EndUpdate();
					InvalidatePixelScrollingInfo();
					RaiseColumnPositionChanged(column);
				}
				else
					CancelUpdate();
			}
		}
		#endregion 
		#region CellSelection
		internal int lockSelection = 0;
		protected internal virtual bool IsCellSelect { get { return OptionsSelection.MultiSelect && OptionsSelection.MultiSelectMode == TreeListMultiSelectMode.CellSelect; } }
		public virtual void BeginSelection() {
			lockSelection++;
		}
		protected internal void CancelSelection() { 
			lockSelection--;
		}
		public virtual void EndSelection() {
			if(lockSelection == 0) return;
			if(--lockSelection == 0)
				OnSelectionChanged();
		}
		internal void InvokeSelectionAction(Action action) {
			if(IsCellSelect)
				BeginSelection();
			try {
				action();
			}
			finally {
				if(IsCellSelect)
					CancelSelection();
			}
		}
		public void SelectAll() {
			TreeListOperationAccumulateNodes op = new TreeListOperationAccumulateNodes(true);
			NodesIterator.DoOperation(op);
			BeginSelection();
			try {
				Selection.InternalClear();
				foreach(TreeListNode node in op.Nodes) {
					SelectNode(node);
				}
			}
			finally {
				EndSelection();
			}
		}
		public bool IsCellSelected(TreeListNode node, TreeListColumn column) {
			if(!IsCellSelect) return false;
			CellSelectionInfo info = Selection.GetCellSelectionInfo(node);
			return info == null ? false : info.Contains(column);
		}
		public void SelectCell(TreeListNode node, TreeListColumn column) {
			SelectCellCore(node, column, false);
		}
		protected virtual void SelectCellCore(TreeListNode node, TreeListColumn column, bool useSelectionCount) {
			if(node == null || column == null || !IsCellSelect) return;
			CellSelectionInfo info = Selection.GetCellSelectionInfo(node);
			if(info == null)
				info = Selection.InternalAdd(node);
			if(info.AddCell(column, useSelectionCount))
				OnSelectionChanged();
		}
		public void UnselectCell(TreeListNode node, TreeListColumn column) {
			UnselectCellCore(node, column, false);
		}
		protected virtual void UnselectCellCore(TreeListNode node, TreeListColumn column, bool useSelectionCount) {
			if(node == null || column == null || !IsCellSelect) return;
			CellSelectionInfo info = Selection.GetCellSelectionInfo(node);
			if(info == null) return;
			if(!info.RemoveCell(column, useSelectionCount)) return;
			if(info.IsEmpty)
				Selection.InternalRemove(node);
			OnSelectionChanged();
		}
		public List<TreeListCell> GetSelectedCells() {
			List<TreeListCell> list = new List<TreeListCell>();
			if(!IsCellSelect) return list;
			foreach(TreeListNode node in Selection) {
				List<TreeListColumn> cells = GetSelectedCells(node);
				foreach(TreeListColumn column in cells) {
					list.Add(new TreeListCell(node, column));
				}
			}
			return list;
		}
		public List<TreeListColumn> GetSelectedCells(TreeListNode node) {
			List<TreeListColumn> list = new List<TreeListColumn>();
			if(!IsCellSelect) return list;
			CellSelectionInfo info = Selection.GetCellSelectionInfo(node);
			if(info != null) {
				List<TreeListColumn> cells = info.GetCells();
				if(cells != null) {
					list.AddRange(cells);
					list.Sort((column1, column2) => Comparer<int>.Default.Compare(column1.VisibleIndex, column2.VisibleIndex));
				}
			}
			return list;
		}
		public void SelectCells(TreeListNode startNode, TreeListColumn startColumn, TreeListNode endNode, TreeListColumn endColumn) {
			SetCellSelection(startNode, startColumn, endNode, endColumn, true);
		}
		public void UnselectCells(TreeListNode startNode, TreeListColumn startColumn, TreeListNode endNode, TreeListColumn endColumn) {
			SetCellSelection(startNode, startColumn, endNode, endColumn, false);
		}
		public void SelectNode(TreeListNode node) {
			if(IsCellSelect) {
				if(VisibleColumns.Count > 0)
					SetCellSelection(node, VisibleColumns[0], node, VisibleColumns[VisibleColumns.Count - 1], true);
			}
			else {
				Selection.Add(node);
			}
		}
		public void UnselectNode(TreeListNode node) {
			if(IsCellSelect) {
				if(VisibleColumns.Count > 0)
					SetCellSelection(node, VisibleColumns[0], node, VisibleColumns[VisibleColumns.Count - 1], false);
			}
			else {
				Selection.Remove(node);
			}
		}
		protected internal void SelectRange(TreeListNode startNode, TreeListNode endNode) {
			if(startNode == null || endNode == null) return;
			if(startNode == endNode) {
				SelectNode(startNode);
				return;
			}
			int startIndex = GetVisibleIndexByNode(startNode),
				endIndex = GetVisibleIndexByNode(endNode);
			if(startIndex < 0 || endIndex < 0) return;
			if(startIndex > endIndex) {
				int a = endIndex; endIndex = startIndex; startIndex = a;			
			}
			BeginSelection();
			try {
				for(int n = startIndex; n < endIndex + 1; n++) {
					SelectNode(GetNodeByVisibleIndex(n));
				}
			}
			finally {
				EndSelection();
			}
		}
		protected virtual void SetCellSelection(TreeListNode startNode, TreeListColumn startColumn, TreeListNode endNode, TreeListColumn endColumn, bool setSelected) {
			if(startNode == null || endNode == null || startColumn == null || endColumn == null || !startColumn.Visible || !endColumn.Visible) return;
			int startIndex = GetVisibleIndexByNode(startNode), endIndex = GetVisibleIndexByNode(endNode);
			int colStart = startColumn.VisibleIndex, colEnd = endColumn.VisibleIndex;
			SetCellSelectionCore(startIndex, colStart, endIndex, colEnd, setSelected);
		}
		protected internal void SetCellSelectionCore(int startIndex, int colStart, int endIndex, int colEnd, bool setSelected, bool useSelectionCount = false) {
			if(startIndex < 0 || endIndex < 0) return;
			if(colStart > colEnd) {
				int tmp = colEnd; colEnd = colStart; colStart = tmp;
			}
			if(startIndex > endIndex) {
				int tmp = endIndex; endIndex = startIndex; startIndex = tmp;
			}
			BeginSelection();
			try {
				for(int n = startIndex; n < endIndex + 1; n++) {
					for(int c = colStart; c < colEnd + 1; c++) {
						TreeListColumn column = VisibleColumns[c];
						TreeListNode node = GetNodeByVisibleIndex(n);
						if(column != null && node != null) {
							if(setSelected)
								SelectCellCore(node, column, useSelectionCount);
							else
								UnselectCellCore(node, column, useSelectionCount);
						}
					}
				}
			}
			finally {
				EndSelection();
			}
		}
		#endregion
		protected virtual void OnOptionsCustomizationChanged(object sender, BaseOptionChangedEventArgs e) {
			if(e.Name == TreeListOptionsView.AllowBandColumnsMultiRowName)
				OnAllowBandColumnMultiRowChanged();
			LayoutChanged();
		}
		#region Expression Editor
		public virtual void ShowUnboundExpressionEditor(TreeListColumn column) {
			if(column == null) return;
			using(ExpressionEditorForm form = new UnboundColumnExpressionEditorForm(new TreeListUnboundColumnWrapper(column), null)) {
				form.SetMenuManager(MenuManager);
				form.StartPosition = FormStartPosition.CenterParent;
				form.RightToLeft = IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
				TreeListUnboundExpressionEditorEventArgs e = new TreeListUnboundExpressionEditorEventArgs(form, column);
				RaiseUnboundExpressionEditorCreated(e);
				if(!e.ShowExpressionEditor) return;
				if(form.ShowDialog() == DialogResult.OK)
					column.UnboundExpression = form.Expression;
			}
		}
		protected virtual void RaiseUnboundExpressionEditorCreated(TreeListUnboundExpressionEditorEventArgs e) {
			UnboundExpressionEditorEventHandler handler = (UnboundExpressionEditorEventHandler)this.Events[unboundExpressionEditorCreated];
			if(handler != null)
				handler(this, e);
		}
		#endregion
		#region IEvaluatorDataAccess Members
		object IEvaluatorDataAccess.GetValue(PropertyDescriptor descriptor, object theObject) {
			TreePropertyDescriptor treeDesc = descriptor as TreePropertyDescriptor;
			if(treeDesc != null) {
				if(theObject is int)
					return Data.GetValue((int)theObject, treeDesc.Name);
				TreeDisplayPropertyDescriptor treeDisplayDesc = treeDesc as TreeDisplayPropertyDescriptor;
				if(treeDisplayDesc != null)
					return Data.DataHelper.GetDisplayText(treeDesc.Name, (TreeListNode)theObject);
				TreeFindFilterDisplayPropertyDescriptor treeFindDisplayDesc = treeDesc as TreeFindFilterDisplayPropertyDescriptor;
				if(treeFindDisplayDesc != null)
					return Data.DataHelper.GetDisplayText(treeFindDisplayDesc.OriginalName, (TreeListNode)theObject);
				return Data.GetValue(((TreeListNode)theObject).Id, treeDesc.Name);
			}
			return null;
		}
		#endregion
		protected override void OnHandleDestroyed(EventArgs e) {
			base.OnHandleDestroyed(e);
			DestroyDragArrows();
		}
		DragArrowsHelper dragArrows = null;
		internal DragArrowsHelper CreateArrowsHelper() {
			if(dragArrows == null)
				dragArrows = new DragArrowsHelper(LookAndFeel, this);
			return dragArrows;
		}
		void DestroyDragArrows() {
			if(dragArrows != null) dragArrows.Dispose();
			dragArrows = null;
		}
		#region IGestureClient Members
		Point touchStart = Point.Empty;
		Point touchOverpan = Point.Empty;
		IntPtr IGestureClient.Handle { get { return Handle; } }
		IntPtr IGestureClient.OverPanWindowHandle { get { return GestureHelper.FindOverpanWindow(this); } }
		Point IGestureClient.PointToClient(Point p) { return PointToClient(p); }
		protected virtual bool AllowTouchScroll { get { return true; } }
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			if(!AllowTouchScroll) return GestureAllowArgs.None;
			TreeListHitInfo hitInfo = CalcHitInfo(point);
			if(CanStartTouchScrolling(hitInfo)) {
				int allow = GestureHelper.GC_PAN_ALL;
				if(OptionsView.AutoWidth)
					allow &= ~GestureHelper.GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY;
				return new GestureAllowArgs[] { new GestureAllowArgs() { GID = GID.PAN, AllowID = allow }, GestureAllowArgs.PressAndTap };
			}
			return GestureAllowArgs.None;
		}
		protected virtual bool CanStartTouchScrolling(TreeListHitInfo ht) {
			if(OptionsDragAndDrop.DragNodesMode != XtraTreeList.DragNodesMode.None)
				return (ht.Node != null && (ht.HitInfoType == HitInfoType.Cell || ht.HitInfoType == HitInfoType.RowPreview || ht.HitInfoType == HitInfoType.RowFooter)) || ht.HitInfoType == HitInfoType.Empty;
			return ht.Node != null || ht.HitInfoType == HitInfoType.Empty;
		}
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			OnTouchScroll(info, delta, ref overPan);
		}
		protected virtual void OnTouchScroll(GestureArgs info, Point delta, ref Point overPan) {
			if(info.GF == GF.BEGIN) {
				touchStart.X = LeftCoord;
				if(IsPixelScrolling)
					touchStart.Y = TopVisibleNodePixel;
				else
					touchStart.Y = TopVisibleNodeIndex;
				touchOverpan = Point.Empty;
				return;
			}
			if(delta.Y != 0) {
				if(IsPixelScrolling) {
					int prevTop = TopVisibleNodePixel;
					TopVisibleNodePixel -= delta.Y;
					if(prevTop == TopVisibleNodePixel)
						touchOverpan.Y += delta.Y;
					else
						touchOverpan.Y = 0;
				}
				else {
					int totalDelta = (info.Current.Y - info.Start.Y) / CheckRowHeight(ViewInfo.RowHeight);
					int prevTopRowIndex = TopVisibleNodeIndex;
					TopVisibleNodeIndex = touchStart.Y - totalDelta;
					if(CheckTopVisibleRowIndex(TopVisibleNodeIndex + (delta.Y > 0 ? -1 : 1)) == TopVisibleNodeIndex) {
						if(prevTopRowIndex == TopVisibleNodeIndex)
							touchOverpan.Y += delta.Y;
						else
							touchOverpan.Y = 0;
					}
				}
			}
			if(delta.X != 0) {
				int prevLeftCoord = LeftCoord;
				LeftCoord -= delta.X;
				if(prevLeftCoord == LeftCoord)
					touchOverpan.X += delta.X;
				else
					touchOverpan.X = 0;
			}
			overPan = touchOverpan;
		}
		protected internal virtual int CheckTopVisibleRowIndex(int newTopRowIndex) {
			if(newTopRowIndex < 0)
				newTopRowIndex = 0;
			if(newTopRowIndex + viewInfo.VisibleRowCount >= RowCount + LowestRowFooterLines)
				newTopRowIndex = RowCount + LowestRowFooterLines - viewInfo.VisibleRowCount;
			return newTopRowIndex;
		}
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs info) { }
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) { }
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) { }
		void IGestureClient.OnTwoFingerTap(GestureArgs info) { }
		void IGestureClient.OnPressAndTap(GestureArgs info) { }
		#endregion
		#region IStringImageProvider Members
		Image IStringImageProvider.GetImage(string id) {
			if(HtmlImages == null) return null;
			return HtmlImages.Images[id];
		}
		#endregion
		#region IServiceProvider Members
		object IServiceProvider.GetService(Type serviceType) {
			if(serviceType == typeof(IStringImageProvider)) return this;
			if(serviceType == typeof(ISite)) return Site;
			return null;
		}
		#endregion
		#region DataAccess
		[CustomBindingProperty("KeyFieldName", "Key Field", "")]
		[CustomBindingProperty("ParentFieldName", "Parent Field", "")]
		protected class TreeListCustomBindingPropertiesAttribute :
			Utils.Design.DataAccess.CustomBindingPropertiesAttribute {
		}
		#endregion DataAccess#
		#region ISearchControlClient Members
		SearchColumnsInfo searchInfo;
		ISearchControl searchControl;
		IEnumerable IFilteredComponentColumns.Columns {
			get { return ((IFilteredComponent)this).CreateFilterColumnCollection(); }
		}
		bool UseSearchInfo { get { return IsAttachedToSearchControl && searchInfo != null; } }
		void ISearchControlClient.ApplyFindFilter(SearchInfoBase args) {
			this.searchInfo = args as SearchColumnsInfo;
			if(args != null)
				this.ApplyFindFilter(args.SearchText);
		}
		SearchControlProviderBase ISearchControlClient.CreateSearchProvider() {
			return new TreeListProvider(this);
		}
		bool ISearchControlClient.IsAttachedToSearchControl { get { return IsAttachedToSearchControl; } }
		protected bool IsAttachedToSearchControl { get { return searchControl != null; } }
		public void SetSearchControl(ISearchControl searchControl) {
			this.searchControl = searchControl;
			this.ApplyFindFilter(null);
			FindPanelVisible = !IsAttachedToSearchControl && OptionsFind.AlwaysVisible;
		}
		internal void SetFocusSearchControl() {
			if(IsAttachedToSearchControl)
				searchControl.Focus();
		}
		#endregion
		#region RepositoryItems
		protected internal virtual void OnRepositoryItemRefreshRequired(RepositoryItem item) {
			if(!ViewInfo.IsValid) return;
			bool requireInvalidate = false;
			foreach(RowInfo rowInfo in ViewInfo.RowsInfo.Rows) {
				foreach(CellInfo cell in rowInfo.Cells) {
					if(cell.EditorViewInfo != null && cell.EditorViewInfo.Item == item) {
						cell.EditorViewInfo.RefreshDisplayText = true;
						ViewInfo.UpdateCellInfo(cell, rowInfo.Node, false);
						requireInvalidate = true;
					}
				}
			}
			if(requireInvalidate) InvalidateNodes();
		}
		#endregion
		#region move and copy context
		class TreeListMoveContextBase : IDisposable {
			TreeList treeListCore;
			public TreeListMoveContextBase(TreeList treeList) {
				treeListCore = treeList;
				treeListCore.BeginUpdate();
			}
			protected TreeList TreeList { get { return treeListCore; } }
			protected virtual bool MoveCore(TreeListNode sourceNode, TreeListNode destinationNode, bool modifySource) {
				RemoveNode(sourceNode.owner, sourceNode);
				AddNode(sourceNode, destinationNode);
				if(modifySource)
					ModifySource(sourceNode);
				CheckFocusedNode(sourceNode);
				return true;
			}
			protected virtual void RemoveNode(TreeListNodes nodes, TreeListNode sourceNode) { 
				nodes.remove(sourceNode);
				if(sourceNode.ParentNode != null)
					sourceNode.ParentNode.HasChildren = sourceNode.owner.Count > 0;
			}
			protected virtual void AddNode(TreeListNode sourceNode, TreeListNode destinationNode) {
				TreeListNodes destNodes = (destinationNode == null ? treeListCore.Nodes : destinationNode.Nodes);
				destNodes._add(sourceNode);
				sourceNode.owner = destNodes;				
				if(destinationNode != null)
					destinationNode.HasChildren = true;
			}
			protected virtual void ModifySource(TreeListNode sourceNode) {
				treeListCore.Data.SetParentRelation(sourceNode, sourceNode.ParentNode);
			}
			protected virtual void CheckFocusedNode(TreeListNode sourceNode) {
				if(!treeListCore.IsLockUpdate)
					treeListCore.CheckFocusedNode(sourceNode);
			}
			#region IDisposable Members
			bool isDisposing;
			void IDisposable.Dispose() {
				if(isDisposing) return;
				isDisposing = true;
				OnDispose();
			}
			protected virtual void OnDispose() {
				treeListCore.CorrectIndentWidth();
				treeListCore.CancelUpdate();
				treeListCore = null;
			}
			#endregion
		}
		class TreeListOuterMoveContext : TreeListMoveContext {
			TreeList outerTreeListCore;
			Dictionary<TreeListNode, int> customData;
			public TreeListOuterMoveContext(TreeList outerTreeList, TreeList treeList)
				: base(treeList) {
				outerTreeListCore = outerTreeList;
				outerTreeListCore.BeginUpdate();
				customData = new Dictionary<TreeListNode, int>();
			}
			protected override bool MoveCore(TreeListNode sourceNode, TreeListNode destinationNode, bool modifySource) {
				if(TreeList.IsVirtualMode || outerTreeListCore.IsVirtualMode) return false;
				CustomizeDataRow(sourceNode, destinationNode);
				return base.MoveCore(sourceNode, destinationNode, modifySource);
			}
			Dictionary<string, object> GetDataRow(TreeListNode sourceNode) {
				Dictionary<string, object> newData = new Dictionary<string, object>();
				foreach(DataColumnInfo columnInfo in TreeList.Data.Columns) {
					object data = GetData(sourceNode, columnInfo.ColumnName);
					newData.Add(columnInfo.ColumnName, data);
				}
				return newData;
			}
			protected override void RemoveNode(TreeListNodes nodes, TreeListNode sourceNode) {
				nodes.Remove(sourceNode);
			}
			protected override void MoveComplited(TreeListNode sourceNode, TreeListNode destinationNode) {
				foreach(var data in customData)
					data.Key.SetId(data.Value);
				base.MoveComplited(sourceNode, destinationNode);
			}
			object GetData(TreeListNode sourceNode, string columnId) {
				if(columnId == TreeList.ParentFieldName) return null;
				if(columnId == TreeList.KeyFieldName) return null;
				return outerTreeListCore.GetNodeValue(sourceNode, columnId);
			}
			void CustomizeDataRow(TreeListNode sourceNode, TreeListNode destinationNode) {
				CustomizeNewNodeFromOuterDataEventArgs e = new CustomizeNewNodeFromOuterDataEventArgs(sourceNode, destinationNode, GetDataRow(sourceNode));
				if(!TreeList.RaiseCustomizeNewNodeFromOuterData(e)) {
					int id = TreeList.Data.Append(e.NewData.Values.ToArray());
					customData.Add(sourceNode, id);
					CustomizeDataRowChildren(sourceNode);
				}
			}
			void CustomizeDataRowChildren(TreeListNode sourceNode) {
				if(sourceNode.Nodes.Count == 0) return;
				foreach(TreeListNode node in sourceNode.Nodes)
					CustomizeDataRow(node, sourceNode);
			}
			protected override void OnDispose() {
				TreeList.ResetNodesCounters();
				base.OnDispose();
				customData.Clear();				
				outerTreeListCore.ResetNodesCounters();
				outerTreeListCore.ResetRowCount();
				outerTreeListCore.CorrectIndentWidth();
				outerTreeListCore.ClearAutoHeights(true);
				outerTreeListCore.CancelUpdate();
				outerTreeListCore = null;
			}
		}
		class TreeListOuterCopyContext : TreeListCopyContext {
			TreeList outerTreeListCore;
			protected TreeList OuterTreeList { get { return outerTreeListCore; } }
			public TreeListOuterCopyContext(TreeList outerTreeList, TreeList treeList)
				: base(treeList) {
				outerTreeListCore = outerTreeList;
				outerTreeListCore.BeginUpdate();
			}
			Dictionary<string, object> GetDataRow(TreeListNode sourceNode) {
				Dictionary<string, object> newData = new Dictionary<string, object>();
				foreach(DataColumnInfo columnInfo in TreeList.Data.Columns) {
					object data = GetData(sourceNode, columnInfo.ColumnName);
					newData.Add(columnInfo.ColumnName, data);
				}
				return newData;
			}
			object GetData(TreeListNode sourceNode, string columnId) {
				if(columnId == TreeList.ParentFieldName) return null;
				if(columnId == TreeList.KeyFieldName) return null;
				return outerTreeListCore.GetNodeValue(sourceNode, columnId);
			}
			protected override void CopyComplited(TreeListNode sourceNode, TreeListNode destinationNode, TreeListNode copyNode, bool cloneChildren) {
				CustomizeDataRow(sourceNode, destinationNode, copyNode);
				base.CopyComplited(sourceNode, destinationNode, copyNode, cloneChildren);
			}
			protected override void CopyChildren(TreeListNode sourceNode, TreeListNode destinationNode, TreeListNode clonedNode) {
				if(sourceNode.Nodes.Count == 0) return;
				if(TreeList.CanCloneChildren(sourceNode, destinationNode)) return;
				CopyChildrenCore(sourceNode, clonedNode);
			}
			void CopyChildrenCore(TreeListNode sourceNode, TreeListNode clonedNode) {
				foreach(TreeListNode node in sourceNode.Nodes) {
					TreeListNode clonedChild = (TreeListNode)node.Clone();
					clonedChild.owner = clonedNode.Nodes;
					clonedChild.owner._add(clonedChild);
					CustomizeDataRow(node, clonedNode, clonedChild);
					if(node.Nodes.Count > 0)
						CopyChildrenCore(node, clonedChild);
				}
				clonedNode.HasChildren = clonedNode.Nodes.Count > 0;
			}
			void CustomizeDataRow(TreeListNode sourceNode, TreeListNode destinationNode, TreeListNode cloneNode) {
				CustomizeNewNodeFromOuterDataEventArgs e = new CustomizeNewNodeFromOuterDataEventArgs(sourceNode, destinationNode, GetDataRow(sourceNode));
				if(!TreeList.RaiseCustomizeNewNodeFromOuterData(e)) {
					int id = TreeList.Data.Append(e.NewData.Values.ToArray());
					cloneNode.SetId(id);
				}
			}
			protected override void OnDispose() {
				base.OnDispose();
				outerTreeListCore.ResetNodesCounters();
				outerTreeListCore.CorrectIndentWidth();
				outerTreeListCore.CancelUpdate();
				outerTreeListCore = null;
			}
		}
		class TreeListMoveContext : TreeListMoveContextBase {
			public TreeListMoveContext(TreeList treeList) : base(treeList) { }
			public bool Move(TreeListNode sourceNode, TreeListNode destinationNode, bool modifySource) {
				bool moveComplited = false;
				try {
					moveComplited = MoveCore(sourceNode, destinationNode, modifySource);
					return moveComplited;
				}
				finally {
					if(moveComplited)
						MoveComplited(sourceNode, destinationNode);
				}
			}
			protected virtual void MoveComplited(TreeListNode sourceNode, TreeListNode destinationNode) {
				TreeList.ResetRowCount();
				if(sourceNode._visible && sourceNode.ParentNode != null && !sourceNode.ParentNode._visible) {
					sourceNode._visible = false;
					TreeListFilterHelper.UpdateChildNodesVisibility(sourceNode);
				}
				else if(sourceNode.IsVisible && !sourceNode._visible && sourceNode.ParentNode != null && sourceNode.ParentNode._visible) {
					sourceNode._visible = true;
					TreeListFilterHelper.UpdateChildNodesVisibility(sourceNode);
				}
			}
		}
		class TreeListCopyContext : TreeListMoveContextBase {
			public TreeListCopyContext(TreeList treeList) : base(treeList) { }
			public TreeListNode Copy(TreeListNode sourceNode, TreeListNode destinationNode, bool modifySource, bool cloneChildren) {
				TreeListNode resultNode = (TreeListNode)sourceNode.Clone();
				try {
					MoveCore(resultNode, destinationNode, modifySource);
					return resultNode;
				}
				finally {
					CopyComplited(sourceNode, destinationNode, resultNode, cloneChildren);
				}
			}
			protected override void RemoveNode(TreeListNodes nodes, TreeListNode sourceNode) { }
			protected virtual void CopyComplited(TreeListNode sourceNode, TreeListNode destinationNode, TreeListNode copyNode, bool cloneChildren) {
				TreeList.ResetNodesCounters();
				if(cloneChildren) {
					CopyChildren(sourceNode, destinationNode, copyNode);
				}
			}
			protected virtual void CopyChildren(TreeListNode sourceNode, TreeListNode destinationNode, TreeListNode clonedNode) {
				TreeList.CopyChildren(sourceNode, destinationNode, clonedNode);
			}
		}
		#endregion
	}
	public class TreeListClipboardHelper {
		protected const string CrLf = "\xd\xa", 
							   ColumnSeparator = "\t";
		public TreeListClipboardHelper(TreeList treeList) {
			TreeList = treeList;
		}
		protected TreeList TreeList { get; private set; }
		protected bool IsCellSelect { get { return TreeList.IsCellSelect; } }
		#region public methods
		public virtual void CopyToClipboard() {
			DataObject data = new DataObject();
			List<SelectionInfo> selection = CreateSelection();
			data.SetData(typeof(string), GetSelectionText(selection));
			object selData = GetSelectionData(selection);
			if(selData != null) data.SetData(selData);
			Clipboard.SetDataObject(data);
		}
		#endregion
		protected internal List<SelectionInfo> CreateSelection() {
			List<SelectionInfo> selection = new List<SelectionInfo>();
			TreeList.NodesIterator.DoOperation((node) =>
			{
				if(node.Selected)
					selection.Add(new SelectionInfo(node, GetCells(node)));
			});
			NormalizeLevels(selection);
			return selection;
		}
		protected List<TreeListColumn> GetCells(TreeListNode node) {
			if(!IsCellSelect) return null;
			CellSelectionInfo info = TreeList.Selection.GetCellSelectionInfo(node);
			return info == null ? null : info.GetCells();
		}
		protected void NormalizeLevels(List<SelectionInfo> selection) {
			if(TreeList.OptionsClipboard.CopyNodeHierarchy == DefaultBoolean.False) return;
			List<SelectionInfo> selectionCopy = new List<SelectionInfo>(selection);
			selectionCopy.Sort((a, b) => { return Comparer<int>.Default.Compare(a.Level, b.Level); });
			int currentLevel = -1;
			for(int i = 0; i < selectionCopy.Count; i++) {
				int level = selectionCopy[i].Level;
				if(level == currentLevel)
					continue;
				int nextLevel = currentLevel + 1;
				if(level == nextLevel) {
					currentLevel = nextLevel;
					continue;
				}
				for(int n = i; n < selectionCopy.Count; n++)
					selectionCopy[n].Level -= (level - nextLevel);
				currentLevel = nextLevel;
			}
		}
		protected virtual object GetSelectionData(List<SelectionInfo> selection) {
			if(selection.Count == 0) return null;
			ArrayList list = new ArrayList();
			for(int i = 0; i < selection.Count; i++)
				list.Add(TreeList.GetDataRecordByNode(selection[i].Node));
			return list;
		}
		protected virtual string GetSelectionText(List<SelectionInfo> selection) {
			int maxLevel = GetSelectionMaxLevel(selection) + 1;
			StringBuilder sb = new StringBuilder();
			List<TreeListColumn> columns = GetVisibleColumns(selection);
			if(AppendHeadersText(sb, maxLevel, columns))
				AppendNewLine(sb);
			for(int i = 0; i < selection.Count; i++) {
				if(AppendRowText(sb, selection[i], maxLevel, columns))
					AppendNewLine(sb);
			}
			return sb.ToString();
		}
		protected int GetSelectionMaxLevel(List<SelectionInfo> selection) {
			int level = 0;
			for(int i = 0; i < selection.Count; i++)
				level = Math.Max(level, selection[i].Level);
			return level;
		}
		protected virtual bool AppendHeadersText(StringBuilder sb, int maxLevel, List<TreeListColumn> columns) {
			if(TreeList.OptionsClipboard.CopyColumnHeaders == DefaultBoolean.False) return false;
			return AppendRowText(sb, null, maxLevel, columns);
		}
		protected List<TreeListColumn> GetVisibleColumns(List<SelectionInfo> selection) {
			if(!IsCellSelect) return new List<TreeListColumn>(TreeList.VisibleColumns);
			List<TreeListColumn> columns = new List<TreeListColumn>();
			foreach(SelectionInfo info in selection) {
				if(info.Cells == null) continue;
				foreach(TreeListColumn column in info.Cells) { 
					if(!columns.Contains(column))
						columns.Add(column);
				}
			}
			columns.Sort((column1, column2) => Comparer<int>.Default.Compare(column1.VisibleIndex, column2.VisibleIndex));
			return columns;
		}
		protected virtual bool AppendRowText(StringBuilder sb, SelectionInfo info, int maxLevel, List<TreeListColumn> columns) {
			bool hasData = false, copyHierarchy = (TreeList.OptionsClipboard.CopyNodeHierarchy != DefaultBoolean.False) && !IsCellSelect;
			for(int i = 0; i < columns.Count; i++) {
				TreeListColumn column = columns[i];
				if(copyHierarchy && i == 0)
					AppendRowIndent(sb, info, maxLevel, false);
				if(IsCellSelect && info != null) {
					if(info.Cells == null || !info.Cells.Contains(column)) {
						sb.Append(ColumnSeparator);
						continue;
					}
				}
				string text = (info == null ? column.GetTextCaption() : info.Node.GetDisplayText(column));
				if(text == null) text = string.Empty;
				if(text.Contains(Environment.NewLine))
					text = "\"" + text.Replace("\"", "\"\"").Replace(Environment.NewLine, "\x0A") + "\"";
				text = text.Replace(ColumnSeparator, " ");
				sb.Append(text);
				if(copyHierarchy && i == 0)
					AppendRowIndent(sb, info, maxLevel, true);
				if(i < columns.Count - 1)
					sb.Append(ColumnSeparator);
				hasData = true;
			}
			return hasData;
		}
		protected virtual void AppendRowIndent(StringBuilder sb, SelectionInfo info, int maxLevel, bool rightIndent) {
			char separator = '\t';
			if(info == null) {
				if(rightIndent)
					for(int i = 1; i < maxLevel; i++) sb.Append(separator);
				return;
			}
			if(!rightIndent)
				for(int i = 0; i < info.Level; i++) sb.Append(separator);
			else
				for(int i = info.Level + 1; i < maxLevel; i++) sb.Append(separator);
		}
		protected void AppendNewLine(StringBuilder sb) {
			sb.Append(CrLf);
		}
		#region inner classes
		protected internal class SelectionInfo {
			public SelectionInfo(TreeListNode node, List<TreeListColumn> cells) {
				Node = node;
				Cells = cells;
				Level = Node.Level;
			}
			public int Level { get; set; }
			public TreeListNode Node { get; private set; }
			public List<TreeListColumn> Cells { get; private set; }
			public override string ToString() {
				return Level.ToString();
			}
		}
		#endregion
	}
	public class TreeListContainerHelper : EditorContainerHelper {
		public TreeListContainerHelper(TreeList owner) : base(owner) { }
		public virtual void ActivateEditor(RepositoryItem ritem, UpdateEditorInfoArgs args) {
			DevExpress.XtraEditors.BaseEdit be = UpdateEditor(ritem, args);
			if(be == null) return;
			UpdateEditorProperties(be);
			be.EditValueChanged += new EventHandler(Owner.OnActiveEditor_ValueChanged);
			be.EditValueChanging += new ChangingEventHandler(Owner.OnActiveEditor_ValueChanging);
			be.LostFocus += new EventHandler(Owner.OnActiveEditor_LostFocus);
			be.GotFocus += new EventHandler(Owner.OnActiveEditor_GotFocus);
			be.Modified += new EventHandler(Owner.OnActiveEditor_Modified);
			ShowEditor(be, Owner);
			if(ActiveEditor != null) {
				if(Owner.OptionsBehavior.AutoSelectAllInEditor)
					ActiveEditor.SelectAll();
			}
		}
		protected virtual void UpdateEditorProperties(BaseEdit be) {
			if(TreeListAutoFilterNode.IsAutoFilterNode(Owner.FocusedNode)) {
				be.Properties.BeginUpdate();
				try {
					be.Properties.NullText = "";
					be.Properties.ReadOnly = false;
					CheckEdit check = be as CheckEdit;
					if(check != null) {
						check.Properties.AllowGrayed = true;
						check.Properties.ValueGrayed = null;
					}
					ImageComboBoxEdit image = be as ImageComboBoxEdit;
					if(image != null)
						image.Properties.Items.Insert(0, new ImageComboBoxItem("", null));
					TextEdit textEdit = be as TextEdit;
					if(textEdit != null)
						textEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
				}
				finally {
					be.Properties.EndUpdate();
				}
			}
		}
		public virtual void DeactivateEditor(bool setFocus) {
			if(ActiveEditor == null) return;
			ActiveEditor.EditValueChanging -= new ChangingEventHandler(Owner.OnActiveEditor_ValueChanging);
			ActiveEditor.EditValueChanged -= new EventHandler(Owner.OnActiveEditor_ValueChanged);
			ActiveEditor.LostFocus -= new EventHandler(Owner.OnActiveEditor_LostFocus);
			ActiveEditor.GotFocus -= new EventHandler(Owner.OnActiveEditor_GotFocus);
			ActiveEditor.Modified -= new EventHandler(Owner.OnActiveEditor_Modified);
			HideEditorCore(Owner, Owner.ContainsFocus);
		}
		public virtual void FocusEditor() {
			if(ActiveEditor == null || ActiveEditor.Focused) return;
			BeginLockFocus();
			try { ActiveEditor.Focus(); }
			finally { EndLockFocus(); }
		}
		public void BeginLockFocus() { fInternalFocusLock++; }
		public void EndLockFocus() { fInternalFocusLock--; }
		protected override void OnEditorsRepositoryChanged() {  }
		protected override void OnRepositoryItemChanged(RepositoryItem item) {
			base.OnRepositoryItemChanged(item);
			Owner.ViewInfo.SummaryFooterInfo.NeedsRecalcAll = true; 
			Owner.LayoutChanged();
		}
		protected override void OnRepository_RefreshRequired(object sender, RepositoryItemChangedEventArgs e) {
			base.OnRepository_RefreshRequired(sender, e);
			Owner.OnRepositoryItemRefreshRequired(e.Item);
		}
		protected override void RaiseValidatingEditor(BaseContainerValidateEditorEventArgs e) {
			BeginLockFocus();
			try {
				Owner.RaiseValidatingEditor(e);
			}
			finally {
				EndLockFocus();
			}
		}
		protected override void RaiseInvalidValueException(InvalidValueExceptionEventArgs e) {
			BeginLockFocus();
			try {
				Owner.RaiseInvalidValueException(e);
			}
			finally {
				EndLockFocus();
			}
		}
		protected new TreeList Owner { get { return base.Owner as TreeList; } }
	}
}
