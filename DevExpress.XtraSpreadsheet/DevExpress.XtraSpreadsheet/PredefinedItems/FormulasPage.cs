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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraSpreadsheet.Forms;
namespace DevExpress.XtraSpreadsheet.UI {
	#region SpreadsheetFunctionLibraryItemBuilder
	public class SpreadsheetFunctionLibraryItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarSubItem subItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.FunctionsAutoSumCommandGroup);
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FunctionsInsertSum));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FunctionsInsertAverage));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FunctionsInsertCountNumbers));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FunctionsInsertMax));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FunctionsInsertMin));
			items.Add(subItem);
			items.Add(new FunctionsFinancialItem());
			items.Add(new FunctionsLogicalItem());
			items.Add(new FunctionsTextItem());
			items.Add(new FunctionsDateAndTimeItem());
			items.Add(new FunctionsLookupAndReferenceItem());
			items.Add(new FunctionsMathAndTrigonometryItem());
			subItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.FunctionsMoreCommandGroup);
			subItem.AddBarItem(new FunctionsStatisticalItem());
			subItem.AddBarItem(new FunctionsEngineeringItem());
			subItem.AddBarItem(new FunctionsInformationItem());
			subItem.AddBarItem(new FunctionsCompatibilityItem());
			subItem.AddBarItem(new FunctionsWebItem());
			items.Add(subItem);
		}
	}
	#endregion
	#region SpreadsheetFormulaDefinedNamesItemBuilder
	public class SpreadsheetFormulaDefinedNamesItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormulasShowNameManager));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormulasDefineNameCommand, RibbonItemStyles.SmallWithText));
			DefinedNameListItem item = new DefinedNameListItem();
			item.RibbonStyle = RibbonItemStyles.SmallWithText;
			items.Add(item);
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormulasCreateDefinedNamesFromSelection, RibbonItemStyles.SmallWithText));
		}
	}
	#endregion
	#region SpreadsheetFormulaAuditingItemBuilder
	public class SpreadsheetFormulaAuditingItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.ViewShowFormulas));
		}
	}
	#endregion
	#region SpreadsheetFormulaCalculationItemBuilder
	public class SpreadsheetFormulaCalculationItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarSubItem subItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.FormulasCalculationOptionsCommandGroup);
			subItem.AddBarItem(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.FormulasCalculationModeAutomatic));
			subItem.AddBarItem(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.FormulasCalculationModeManual));
			items.Add(subItem);
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormulasCalculateNow, RibbonItemStyles.SmallWithText));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormulasCalculateSheet, RibbonItemStyles.SmallWithText));
		}
	}
	#endregion
	#region SpreadsheetFunctionLibraryBarCreator
	public class SpreadsheetFunctionLibraryBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(FormulasRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(FunctionLibraryRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(FunctionLibraryBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 3; } }
		public override Bar CreateBar() {
			return new FunctionLibraryBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetFunctionLibraryItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new FormulasRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new FunctionLibraryRibbonPageGroup();
		}
	}
	#endregion
	#region SpreadsheetFormulaDefinedNamesBarCreator
	public class SpreadsheetFormulaDefinedNamesBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(FormulasRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(FormulaDefinedNamesRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(FormulaDefinedNamesBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 3; } }
		public override Bar CreateBar() {
			return new FormulaDefinedNamesBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetFormulaDefinedNamesItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new FormulasRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new FormulaDefinedNamesRibbonPageGroup();
		}
	}
	#endregion
	#region SpreadsheetFormulaAuditingBarCreator
	public class SpreadsheetFormulaAuditingBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(FormulasRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(FormulaAuditingRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(FormulaAuditingBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 3; } }
		public override Bar CreateBar() {
			return new FormulaAuditingBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetFormulaAuditingItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new FormulasRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new FormulaAuditingRibbonPageGroup();
		}
	}
	#endregion
	#region SpreadsheetFormulaCalculationBarCreator
	public class SpreadsheetFormulaCalculationBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(FormulasRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(FormulaCalculationRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(FormulaCalculationBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 3; } }
		public override Bar CreateBar() {
			return new FormulaCalculationBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetFormulaCalculationItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new FormulasRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new FormulaCalculationRibbonPageGroup();
		}
	}
	#endregion
	#region FormulasRibbonPage
	public class FormulasRibbonPage : ControlCommandBasedRibbonPage {
		public FormulasRibbonPage() {
		}
		public FormulasRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageFormulas); } }
	}
	#endregion
	#region FunctionLibraryBar
	public class FunctionLibraryBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public FunctionLibraryBar() {
		}
		public FunctionLibraryBar(BarManager manager)
			: base(manager) {
		}
		public FunctionLibraryBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupFunctionLibrary); } }
	}
	#endregion
	#region FormulaDefinedNamesBar
	public class FormulaDefinedNamesBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public FormulaDefinedNamesBar() {
		}
		public FormulaDefinedNamesBar(BarManager manager)
			: base(manager) {
		}
		public FormulaDefinedNamesBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupFormulaDefinedNames); } }
	}
	#endregion
	#region FormulaAuditingBar
	public class FormulaAuditingBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public FormulaAuditingBar() {
		}
		public FormulaAuditingBar(BarManager manager)
			: base(manager) {
		}
		public FormulaAuditingBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupFormulaAuditing); } }
	}
	#endregion
	#region FormulaCalculationBar
	public class FormulaCalculationBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public FormulaCalculationBar() {
		}
		public FormulaCalculationBar(BarManager manager)
			: base(manager) {
		}
		public FormulaCalculationBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupFormulaCalculation); } }
	}
	#endregion
	#region FunctionLibraryRibbonPageGroup
	public class FunctionLibraryRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public FunctionLibraryRibbonPageGroup() {
		}
		public FunctionLibraryRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupFunctionLibrary); } }
	}
	#endregion
	#region FormulaDefinedNamesRibbonPageGroup
	public class FormulaDefinedNamesRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public FormulaDefinedNamesRibbonPageGroup() {
		}
		public FormulaDefinedNamesRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupFormulaDefinedNames); } }
	}
	#endregion
	#region FormulaAuditingRibbonPageGroup
	public class FormulaAuditingRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public FormulaAuditingRibbonPageGroup() {
		}
		public FormulaAuditingRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupFormulaAuditing); } }
	}
	#endregion
	#region FormulaCalculationRibbonPageGroup
	public class FormulaCalculationRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public FormulaCalculationRibbonPageGroup() {
		}
		public FormulaCalculationRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupFormulaCalculation); } }
	}
	#endregion
	#region FunctionsListItemBase (abstract class)
	public abstract class FunctionsListItemBase : SpreadsheetCommandBarButtonItem {
		PopupMenu popupMenu = new PopupMenu();
		protected FunctionsListItemBase(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected FunctionsListItemBase(BarManager manager)
			: base(manager) {
		}
		protected FunctionsListItemBase(string caption)
			: base(caption) {
		}
		protected FunctionsListItemBase()
			: base() {
		}
		#region Properties
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ActAsDropDown { get { return true; } set { } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarButtonStyle ButtonStyle { get { return BarButtonStyle.DropDown; } set { } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override PopupControl DropDownControl { get { return popupMenu; } set { } }
		#endregion
		protected abstract IList<string> GetFunctionList();
		protected static IList<string> GetFunctionList(Dictionary<string, ISpreadsheetFunction> functions) {
			List<string> result = new List<string>();
			foreach (string key in functions.Keys) {
				ISpreadsheetFunction function = functions[key];
				if (!(function is NotSupportedFunction))
					result.Add(key);
			}
			result.Sort();
			return result;
		}
		protected virtual void PopulatePopupMenu() {
			if (this.popupMenu == null)
				return;
			IList<string> functionList = GetFunctionList();
			this.popupMenu.BeginUpdate();
			try {
				foreach (string functionName in functionList) {
					FunctionsInsertSpecificFunctionMenuItem item = new FunctionsInsertSpecificFunctionMenuItem(Control, functionName);
					this.popupMenu.ItemLinks.Add(item);
				}
			}
			finally {
				this.popupMenu.EndUpdate();
			}
		}
		protected virtual void RefreshPopupMenu() {
			DeletePopupItems();
			if (Control != null)
				PopulatePopupMenu();
		}
		protected virtual void DeletePopupItems() {
			if (this.popupMenu == null)
				return;
			BarItemLinkCollection itemLinks = this.popupMenu.ItemLinks;
			this.popupMenu.BeginUpdate();
			try {
				while (itemLinks.Count > 0)
					itemLinks[0].Item.Dispose();
			}
			finally {
				this.popupMenu.EndUpdate();
			}
		}
		protected override void SubscribeControlEvents() {
			base.SubscribeControlEvents();
			if (this.popupMenu != null)
				this.popupMenu.BeforePopup += OnBeforePopup;
		}
		protected override void UnsubscribeControlEvents() {
			if (this.popupMenu != null)
				this.popupMenu.BeforePopup -= OnBeforePopup;
			base.UnsubscribeControlEvents();
		}
		protected virtual void OnBeforePopup(object sender, CancelEventArgs e) {
			RefreshPopupMenu();
		}
		protected override void OnManagerChanged() {
			base.OnManagerChanged();
			if (this.popupMenu != null)
				this.popupMenu.Manager = this.Manager;
		}
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing) {
				if (popupMenu != null) {
					DeletePopupItems();
					popupMenu.Dispose();
					popupMenu = null;
				}
			}
		}
		#endregion
	}
	#endregion
	#region FunctionsInsertSpecificFunctionMenuItem
	public class FunctionsInsertSpecificFunctionMenuItem : BarCheckItem {
		#region Fields
		readonly SpreadsheetControl control;
		readonly string functionName;
		#endregion
		public FunctionsInsertSpecificFunctionMenuItem(SpreadsheetControl control, string functionName) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.functionName = functionName;
			this.Caption = CreateCaption();
			FunctionsInsertSpecificFunctionCommand command = CreateCommand();
			BarCheckItemUIState state = new BarCheckItemUIState(this);
			command.UpdateUIState(state);
		}
		protected override void OnClick(BarItemLink link) {
			FunctionsInsertSpecificFunctionCommand command = CreateCommand();
			command.Execute();
		}
		protected internal virtual FunctionsInsertSpecificFunctionCommand CreateCommand() {
			FunctionsInsertSpecificFunctionCommand command = new FunctionsInsertSpecificFunctionCommand(control, functionName);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
		protected internal virtual string CreateCaption() {
			return FormulaCalculator.GetFunctionName(functionName, control.DocumentModel.DataContext);
		}
	}
	#endregion
	#region DefinedNameListItem
	public class DefinedNameListItem : SpreadsheetCommandBarButtonItem {
		PopupMenu popupMenu = new PopupMenu();
		public DefinedNameListItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public DefinedNameListItem(BarManager manager)
			: base(manager) {
		}
		public DefinedNameListItem(string caption)
			: base(caption) {
		}
		public DefinedNameListItem()
			: base() {
		}
		#region Properties
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ActAsDropDown { get { return true; } set { } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarButtonStyle ButtonStyle { get { return BarButtonStyle.DropDown; } set { } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override PopupControl DropDownControl { get { return popupMenu; } set { } }
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FormulasInsertDefinedNameCommandGroup; } }
		#endregion
		protected IList<string> GetDefinedNameList() {
			List<string> result = new List<string>();
			if (Control == null)
				return new List<string>();
			NameManagerViewModel viewModel = new NameManagerViewModel(Control);
			return viewModel.GetAvailableDefinedNameList(Control.DocumentModel.ActiveSheet);
		}
		protected virtual void PopulatePopupMenu() {
			if (this.popupMenu == null)
				return;
			IList<string> definedNameList = GetDefinedNameList();
			this.popupMenu.BeginUpdate();
			try {
				foreach (string definedName in definedNameList) {
					FormulasInsertDefinedNameMenuItem item = new FormulasInsertDefinedNameMenuItem(Control, definedName);
					this.popupMenu.ItemLinks.Add(item);
				}
			}
			finally {
				this.popupMenu.EndUpdate();
			}
		}
		protected virtual void RefreshPopupMenu() {
			DeletePopupItems();
			if (Control != null)
				PopulatePopupMenu();
		}
		protected virtual void DeletePopupItems() {
			if (this.popupMenu == null)
				return;
			BarItemLinkCollection itemLinks = this.popupMenu.ItemLinks;
			this.popupMenu.BeginUpdate();
			try {
				while (itemLinks.Count > 0)
					itemLinks[0].Item.Dispose();
			}
			finally {
				this.popupMenu.EndUpdate();
			}
		}
		protected override void SubscribeControlEvents() {
			base.SubscribeControlEvents();
			if (this.popupMenu != null)
				this.popupMenu.BeforePopup += OnBeforePopup;
		}
		protected override void UnsubscribeControlEvents() {
			if (this.popupMenu != null)
				this.popupMenu.BeforePopup -= OnBeforePopup;
			base.UnsubscribeControlEvents();
		}
		protected virtual void OnBeforePopup(object sender, CancelEventArgs e) {
			RefreshPopupMenu();
		}
		protected override void OnManagerChanged() {
			base.OnManagerChanged();
			if (this.popupMenu != null)
				this.popupMenu.Manager = this.Manager;
		}
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing) {
				if (popupMenu != null) {
					DeletePopupItems();
					popupMenu.Dispose();
					popupMenu = null;
				}
			}
		}
		#endregion
	}
	#endregion
	#region FormulasInsertDefinedNameMenuItem
	public class FormulasInsertDefinedNameMenuItem : BarCheckItem {
		#region Fields
		readonly SpreadsheetControl control;
		readonly string definedName;
		#endregion
		public FormulasInsertDefinedNameMenuItem(SpreadsheetControl control, string definedName) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.definedName = definedName;
			this.Caption = CreateCaption();
			InsertDefinedNameCommand command = CreateCommand();
			BarCheckItemUIState state = new BarCheckItemUIState(this);
			command.UpdateUIState(state);
		}
		protected override void OnClick(BarItemLink link) {
			InsertDefinedNameCommand command = CreateCommand();
			command.Execute();
		}
		protected internal virtual InsertDefinedNameCommand CreateCommand() {
			InsertDefinedNameCommand command = new InsertDefinedNameCommand(control, definedName);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
		protected internal virtual string CreateCaption() {
			return definedName;
		}
	}
	#endregion
	#region FunctionsFinancialItem
	public class FunctionsFinancialItem : FunctionsListItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		public FunctionsFinancialItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public FunctionsFinancialItem(BarManager manager)
			: base(manager) {
		}
		public FunctionsFinancialItem(string caption)
			: base(caption) {
		}
		public FunctionsFinancialItem()
			: base() {
		}
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FunctionsFinancialCommandGroup; } }
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddFinancialFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region FunctionsLogicalItem
	public class FunctionsLogicalItem : FunctionsListItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		public FunctionsLogicalItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public FunctionsLogicalItem(BarManager manager)
			: base(manager) {
		}
		public FunctionsLogicalItem(string caption)
			: base(caption) {
		}
		public FunctionsLogicalItem()
			: base() {
		}
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FunctionsLogicalCommandGroup; } }
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddLogicalFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region FunctionsTextItem
	public class FunctionsTextItem : FunctionsListItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		public FunctionsTextItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public FunctionsTextItem(BarManager manager)
			: base(manager) {
		}
		public FunctionsTextItem(string caption)
			: base(caption) {
		}
		public FunctionsTextItem()
			: base() {
		}
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FunctionsTextCommandGroup; } }
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddTextFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region FunctionsDateAndTimeItem
	public class FunctionsDateAndTimeItem : FunctionsListItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		public FunctionsDateAndTimeItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public FunctionsDateAndTimeItem(BarManager manager)
			: base(manager) {
		}
		public FunctionsDateAndTimeItem(string caption)
			: base(caption) {
		}
		public FunctionsDateAndTimeItem()
			: base() {
		}
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FunctionsDateAndTimeCommandGroup; } }
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddDateTimeFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region FunctionsLookupAndReferenceItem
	public class FunctionsLookupAndReferenceItem : FunctionsListItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		public FunctionsLookupAndReferenceItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public FunctionsLookupAndReferenceItem(BarManager manager)
			: base(manager) {
		}
		public FunctionsLookupAndReferenceItem(string caption)
			: base(caption) {
		}
		public FunctionsLookupAndReferenceItem()
			: base() {
		}
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FunctionsLookupAndReferenceCommandGroup; } }
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddLookupAndReferenceFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region FunctionsMathAndTrigonometryItem
	public class FunctionsMathAndTrigonometryItem : FunctionsListItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		public FunctionsMathAndTrigonometryItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public FunctionsMathAndTrigonometryItem(BarManager manager)
			: base(manager) {
		}
		public FunctionsMathAndTrigonometryItem(string caption)
			: base(caption) {
		}
		public FunctionsMathAndTrigonometryItem()
			: base() {
		}
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FunctionsMathAndTrigonometryCommandGroup; } }
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddMathAndTrigonometryFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region FunctionsStatisticalItem
	public class FunctionsStatisticalItem : FunctionsListItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		public FunctionsStatisticalItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public FunctionsStatisticalItem(BarManager manager)
			: base(manager) {
		}
		public FunctionsStatisticalItem(string caption)
			: base(caption) {
		}
		public FunctionsStatisticalItem()
			: base() {
		}
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FunctionsStatisticalCommandGroup; } }
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddStatisticalFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region FunctionsEngineeringItem
	public class FunctionsEngineeringItem : FunctionsListItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		public FunctionsEngineeringItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public FunctionsEngineeringItem(BarManager manager)
			: base(manager) {
		}
		public FunctionsEngineeringItem(string caption)
			: base(caption) {
		}
		public FunctionsEngineeringItem()
			: base() {
		}
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FunctionsEngineeringCommandGroup; } }
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddEngineeringFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region FunctionsCubeItem
	public class FunctionsCubeItem : FunctionsListItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		public FunctionsCubeItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public FunctionsCubeItem(BarManager manager)
			: base(manager) {
		}
		public FunctionsCubeItem(string caption)
			: base(caption) {
		}
		public FunctionsCubeItem()
			: base() {
		}
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FunctionsCubeCommandGroup; } }
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddCubeFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region FunctionsInformationItem
	public class FunctionsInformationItem : FunctionsListItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		public FunctionsInformationItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public FunctionsInformationItem(BarManager manager)
			: base(manager) {
		}
		public FunctionsInformationItem(string caption)
			: base(caption) {
		}
		public FunctionsInformationItem()
			: base() {
		}
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FunctionsInformationCommandGroup; } }
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddInformationalFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region FunctionsCompatibilityItem
	public class FunctionsCompatibilityItem : FunctionsListItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		public FunctionsCompatibilityItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public FunctionsCompatibilityItem(BarManager manager)
			: base(manager) {
		}
		public FunctionsCompatibilityItem(string caption)
			: base(caption) {
		}
		public FunctionsCompatibilityItem()
			: base() {
		}
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FunctionsCompatibilityCommandGroup; } }
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddCompatibilityFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region FunctionsWebItem
	public class FunctionsWebItem : FunctionsListItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		public FunctionsWebItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public FunctionsWebItem(BarManager manager)
			: base(manager) {
		}
		public FunctionsWebItem(string caption)
			: base(caption) {
		}
		public FunctionsWebItem()
			: base() {
		}
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FunctionsWebCommandGroup; } }
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddWebFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
}
