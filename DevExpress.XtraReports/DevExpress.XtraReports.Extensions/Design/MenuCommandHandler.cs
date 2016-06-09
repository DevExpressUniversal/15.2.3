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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Entity;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native.EntityFramework;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.XtraReports.Configuration;
using DevExpress.XtraReports.Wizards3;
using DevExpress.XtraReports.Wizards3.Views;
using DevExpress.DataAccess.UI.Native.Sql;
namespace DevExpress.XtraReports.Design.Commands {
	public class MenuCommandHandler : MenuCommandHandlerBase {
		#region inner classes
		protected class PasteCommandSetItem : CommandSetItem {
			static void MoveControlsUp(ArrayList controlList) {
				if(controlList.Count == 0)
					return;
				float top = Int32.MaxValue;
				foreach(XRControl ctrl in controlList)
					top = Math.Min(top, ctrl.TopF);
				if(top == 0)
					return;
				foreach(XRControl ctrl in controlList)
					ctrl.LocationF = new PointF(ctrl.LeftF, ctrl.TopF - top);
			}
			ArrayList components, winControls;
			IComponentChangeService changeService;
			ISelectionService selectionService;
			IMenuCommandService menuService;
			IDesignerHost host;
			public PasteCommandSetItem(IMenuCommandService menuService, EventHandler statusHandler, IDesignerHost host)
				: base(menuService, null, statusHandler, StandardCommands.Paste) {
				components = new ArrayList();
				winControls = new ArrayList();
				this.host = host;
				this.menuService = menuService;
				changeService = host.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				selectionService = host.GetService(typeof(ISelectionService)) as ISelectionService;
			}
			public override void Invoke(object[] args) {
				DesignerTransaction transaction = host.CreateTransaction(DesignSR.Trans_Paste);
				try {
					components.Clear();
					winControls.Clear();
					if(!LockService.GetInstance(host).CanChangeComponents(selectionService.GetSelectedComponents()))
						return;
					ICollection selection = selectionService.GetSelectedComponents();
					changeService.ComponentAdded += new ComponentEventHandler(this.OnComponentAdded);
					Serialization.XRSerializer.Begin((XtraReport)host.RootComponent, false);
					base.Invoke(args);
					changeService.ComponentAdded -= new ComponentEventHandler(this.OnComponentAdded);
					selectionService.SetSelectedComponents(selection);
					WrapWinControls();
					ArrayList controlsList = GetXRControlList();
					MoveControlsUp(controlsList);
					CallOnSetComponentDefaults(GetComponentList());
					ArrangeControls(controlsList);
				} catch(XRTableOfContentsException ex){
					transaction.Cancel();
					NotificationService.ShowException<XtraReport>(DevExpress.LookAndFeel.DesignService.LookAndFeelProviderHelper.GetLookAndFeel(host), host.GetOwnerWindow(), ex);
				} finally {
					Serialization.XRSerializer.End((XtraReport)host.RootComponent);
					transaction.Commit();
				}
			}
			private void CallOnSetComponentDefaults(IList compList) {
				foreach(Component component in compList) {
					XRControlDesignerBase designer = host.GetDesigner(component) as XRControlDesignerBase;
					if(designer != null)
						designer.InitializeNewComponentCore();
				}
			}
			void ArrangeControls(ArrayList controlList) {
				if(controlList.Count == 0)
					return;
				selectionService.SetSelectedComponents(controlList, SelectionTypes.Replace);
				InvokeCommand(StandardCommands.CenterVertically);
				InvokeCommand(StandardCommands.CenterHorizontally);
				InvokeCommand(StandardCommands.BringToFront);
			}
			ArrayList GetXRControlList() {
				IList components = GetComponentList();
				ArrayList controls = new ArrayList(components.Count);
				foreach(object obj in components)
					if(obj is XRControl && !(obj is Band))
						controls.Add(obj);
				controls.Cast<XRControl>().OrderByDescending(x => x.NestedLevel);
				for(int i = controls.Count - 1; i > 0; i--) {
					if(((XRControl)controls[i]).NestedLevel > ((XRControl)controls[0]).NestedLevel)
						controls.RemoveAt(i);
				}
				return controls;
			}
			private IList GetComponentList() {
				return components;
			}
			private void InvokeCommand(CommandID cmdID) {
				MenuCommand menuCommand = menuService.FindCommand(cmdID);
				if(menuCommand != null) menuCommand.Invoke();
			}
			private void OnComponentAdded(object source, ComponentEventArgs e) {
				if(e.Component is XRControl) {
					components.Add(e.Component);
				} else if(e.Component is Control) {
					foreach(WinControlContainer container in components)
						if(container != null && container.WinControl == e.Component as Control) return;
					winControls.Add(e.Component);
				}
			}
			private void WrapWinControls() {
				ReportDesigner repDesigner = host.GetDesigner(host.RootComponent) as ReportDesigner;
				if(repDesigner == null) return;
				foreach(Control ctl in winControls) {
					WinControlContainer winControlContainer = repDesigner.AddWinControlContainer(ctl);
					if(winControlContainer == null) continue;
					winControlContainer.SizeF = XRConvert.Convert(ctl.Size, GraphicsDpi.Pixel, winControlContainer.Dpi);
					components.Add(winControlContainer);
				}
			}
		}
		protected class DeleteCommandSetItem : CommandSetItem {
			public static Type[] UndeletedBandTypes = new Type[] { typeof(XtraReport), typeof(DetailBand), typeof(TopMarginBand), typeof(BottomMarginBand) };
			ISelectionService selectionService;
			IMenuCommandService menuService;
			IDesignerHost host;
			public DeleteCommandSetItem(IMenuCommandService menuService, EventHandler statusHandler, IDesignerHost host)
				: base(menuService, null, statusHandler, StandardCommands.Delete) {
				this.host = host;
				this.menuService = menuService;
				selectionService = host.GetService(typeof(ISelectionService)) as ISelectionService;
			}
			public override void Invoke(object[] args) {
				if(CanDeleteSelection() && !DeleteTableComponents() && !DeleteGroupBands(args))
					base.Invoke(args);
			}
			bool CanDeleteSelection() {
				ResizeService resizeService = (ResizeService)host.GetService(typeof(ResizeService));
				if(resizeService.IsRunning) {
					return false;
				}
				ICollection selection = selectionService.GetSelectedComponents();
				foreach(Object obj in selection)
					if(NativeMethods.IsAssignableTypes(UndeletedBandTypes, obj.GetType()))
						return false;
				return LockService.GetInstance(host).CanDeleteComponents(selection);
			}
			bool DeleteGroupBands(object[] args) {
				XtraReportBase report = host.RootComponent as XtraReportBase;
				if(report == null)
					return false;
				ICollection selection = selectionService.GetSelectedComponents();
				List<GroupBand> deletingGroupBands = new List<GroupBand>();
				foreach(Object obj in selection) {
					if(obj is GroupBand)
						deletingGroupBands.Add((GroupBand)obj);
				}
				if(deletingGroupBands.Count == 0)
					return false;
				GroupBand targetForCascadeChange = null;
				foreach(Band band in report.Bands) {
					GroupBand groupBand = band as GroupBand;
					if(groupBand != null && !deletingGroupBands.Contains(groupBand)) {
						targetForCascadeChange = groupBand;
						break;
					}
				}
				if(targetForCascadeChange == null)
					return false;
				IComponentChangeService changeService = host.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				DesignerTransaction transaction = host.CreateTransaction(DesignSR.Trans_Delete);
				try {
					XRControlDesignerBase.RaiseComponentChanging(changeService, targetForCascadeChange, XRComponentPropertyNames.Level);
					base.Invoke(args);
					XRControlDesignerBase.RaiseComponentChanged(changeService, targetForCascadeChange,
						XRAccessor.GetPropertyDescriptor(targetForCascadeChange, XRComponentPropertyNames.Level));
					return true;
				} finally {
					if(transaction != null)
						transaction.Commit();
				}
			}
			bool DeleteTableComponents() {
				ICollection selection = selectionService.GetSelectedComponents();
				ArrayList tableRowsList = new ArrayList();
				ArrayList tableCellsList = new ArrayList();
				ArrayList objectsList = new ArrayList();
				foreach(Object obj in selection)
					if(obj is XRTableRow)
						tableRowsList.Add(obj);
					else if(obj is XRTableCell)
						tableCellsList.Add(obj);
					else
						objectsList.Add(obj);
				if(tableRowsList.Count == 0 && tableCellsList.Count == 0)
					return false;
				DesignerTransaction transaction = host.CreateTransaction(DesignSR.Trans_Delete);
				try {
					if(tableRowsList.Count > 0)
						foreach(XRTableRow row in tableRowsList) {
							selectionService.SetSelectedComponents(new object[] { row }, SelectionTypes.Replace);
							menuService.GlobalInvoke(TableCommands.DeleteRow);
						}
					if(tableCellsList.Count > 0)					 
						 if(TableHelper.IsColumnCellsArray(tableCellsList.ToArray())) {
							selectionService.SetSelectedComponents(new object[] { tableCellsList[0] }, SelectionTypes.Replace);
							menuService.GlobalInvoke(TableCommands.DeleteColumn);
						} else
							foreach(XRTableCell cell in tableCellsList)
								if(!cell.IsDisposed) {
									selectionService.SetSelectedComponents(new object[] { cell }, SelectionTypes.Replace);
									menuService.GlobalInvoke(TableCommands.DeleteCell);
								}
					if(objectsList.Count > 0) {
						selectionService.SetSelectedComponents(objectsList, SelectionTypes.Replace);
						base.Invoke();
					}
				} finally {
					if(transaction != null)
						transaction.Commit();
				}
				return true;
			}
		}
		public class CommandSetItemWrapper : CommandSetItem {
			IMenuCommandService menuCommandService;
			CommandID commandID;
			public CommandSetItemWrapper(IMenuCommandService menuService, EventHandler statusHandler, CommandID commandID, CommandID fakeCommandID)
				: base(menuService, null, statusHandler, fakeCommandID) {
				this.menuCommandService = menuService;
				this.commandID = commandID;
			}
			public override void Invoke(object[] args) {
				menuCommandService.GlobalInvoke(commandID);
			}
		}
		#endregion
		#region static
		static bool IsAssignableFrom(Type[] baseTypes, ICollection objs) {
			foreach(object obj in objs)
				if(NativeMethods.IsAssignableTypes(baseTypes, obj.GetType()))
					return true;
			return false;
		}
		static bool IsAssignableFrom(Type[] baseTypes, object obj) {
			return (obj != null) ? NativeMethods.IsAssignableTypes(baseTypes, obj.GetType()) : false;
		}
		static object GetArg(EventArgs e, int index) {
			object[] args = e is CommandExecuteEventArgs ? ((CommandExecuteEventArgs)e).Args : null;
			return args != null && index < args.Length ? args[index] : null;
		}
		public static bool IsStylePropertySupported(object component, string propertyName) {
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(component)[XRComponentDesigner.GetStylePropertyName(propertyName)];
			if(propertyDescriptor != null) {
				BrowsableAttribute attribute = propertyDescriptor.Attributes[typeof(BrowsableAttribute)] as BrowsableAttribute;
				return attribute.Browsable;
			}
			return false;
		}
		#endregion
		IDesignerHost designerHost;
		ISelectionService selectionServ;
		IComponent primarySelection;
		IList selectedComponents;
		IList selectedComponentsWithoutPrimarySelection;
		int selectionCount;
		bool xrControlsOnlySelection;
		bool fontSupported;
		bool foreColorSupported;
		bool backColorSupported;
		bool hasTextAlignProperty;
		bool selectionLocked;
		public MenuCommandHandler(IDesignerHost designerHost)
			: base(designerHost) {
			this.designerHost = designerHost;
			selectionServ = designerHost.GetService(typeof(ISelectionService)) as ISelectionService;
			selectionServ.SelectionChanged += new EventHandler(OnSelectionChanged);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				selectionServ.SelectionChanged -= new EventHandler(OnSelectionChanged);
			}
			base.Dispose(disposing);
		}
		public void RegisterMenuCommands() {
			if(commands.Count > 0) return;
			AddCommandExecutor(new FieldListCommandExecutor(designerHost), new EventHandler(OnStatusFieldListSelection), false,
				CommandGroups.FieldListCommands);
			AddCommandExecutor(new FormattingComponentCommandExecutor(designerHost), new EventHandler(OnStatusFormattingComponentSelection), false,
				CommandGroups.FormattingComponentCommands);
			AddCommandExecutor(new DetailReportCommandExecutor(designerHost), new EventHandler(OnStatusReportCommands), false,
				CommandGroups.ReportCommands);
			AddCommandExecutor(new BandCommandExecutor(designerHost), new EventHandler(OnStatusBandAdd), false,
				BandCommands.AddSubBand);
			AddCommandExecutor(new BandCommandExecutor(designerHost), new EventHandler(OnStatusBandInsert), false,
				CommandGroups.BandInsertCommands);
			AddCommandExecutor(new FieldDropCommandExecutor(designerHost), new EventHandler(OnStatusAlwaysEnabled), false,
				CommandGroups.FieldDropCommands);
			AddCommandExecutor(new RichTextBoxCommandExecutor(designerHost), new EventHandler(OnStatusRichTextBoxCommands), false,
				CommandGroups.RichTextBoxCommands);
			AddCommandExecutor(new VerbCommandExecutor(designerHost), new EventHandler(OnStatusExecuteVerbCommand), false,
				VerbCommands.ExecuteVerb);
			AddCommandExecutor(new PivotGridCommandExecutor(designerHost), new EventHandler(OnStatusPivotGridCommands), false,
				CommandGroups.PivotGridCommands);
			AddCommandExecutor(new TableCellCommandExecutor(designerHost), new EventHandler(OnStatusTableCellCommands), false,
				CommandGroups.TableCellCommands);
			AddCommandExecutor(new TableRowCommandExecutor(designerHost), new EventHandler(OnStatusTableRowCommands), false,
				CommandGroups.TableRowCommands);
			AddCommandExecutor(new TableColumnCommandExecutor(designerHost), new EventHandler(OnStatusTableColumnCommands), false,
				CommandGroups.TableColumnCommands);
			AddCommandExecutor(new TableCommonCommandExecutor(designerHost), new EventHandler(OnStatusCommonTableCommands), false,
				CommandGroups.TableCommands);
			AddCommandExecutor(new KeyMoveCommandExecutor(designerHost), new EventHandler(OnStatusKeyCommands), false,
				CommandGroups.KeyMoveCommands);
			AddCommandExecutor(new KeyNudgeMoveCommandExecutor(designerHost), new EventHandler(OnStatusKeyCommands), false,
				CommandGroups.KeyNudgeCommands);
			AddCommandExecutor(new KeySizeCommandExecutor(designerHost), new EventHandler(OnStatusKeyCommands), false,
				CommandGroups.KeySizeCommands);
			AddCommandExecutor(new KeyNudgeSizeCommandExecutor(designerHost), new EventHandler(OnStatusKeyCommands), false,
				CommandGroups.KeyNudgeSizeCommands);
			AddCommandExecutor(new EscCommandExecutor(designerHost), new EventHandler(OnStatusAlwaysEnabled), false,
				MenuCommands.KeyCancel);
			AddCommandExecutor(new KeySelectionCommandExecutor(designerHost), new EventHandler(OnStatusNotInInplaceMode), false,
				CommandGroups.KeySelectionCommands);
			AddCommandExecutor(new AlignCommandExecutor(designerHost), new EventHandler(OnStatusMultiSelection), false,
				CommandGroups.AlignCommands);
			AddCommandExecutor(new SameSizeCommandExecutor(designerHost), new EventHandler(OnStatusMultiSelection), false,
				CommandGroups.SizeCommands);
			AddCommandExecutor(new HSpaceCommandExecutor(designerHost), new EventHandler(OnStatusMultiSelection), false,
				CommandGroups.HorizSpaceCommands);
			AddCommandExecutor(new VSpaceCommandExecutor(designerHost), new EventHandler(OnStatusMultiSelection), false,
				CommandGroups.VertSpaceCommands);
			AddCommandExecutor(new ZOrderCommandExecutor(designerHost), new EventHandler(OnStatusZOrder), false,
				StandardCommands.BringToFront,
				StandardCommands.SendToBack);
			AddCommandExecutor(new SizeCommandExecutor(designerHost), new EventHandler(OnStatusAnyXRControlSelection), false,
				StandardCommands.SizeToGrid);
			AddCommandExecutor(new MoveCommandExecutor(designerHost), new EventHandler(OnStatusAnyXRControlSelection), false,
				StandardCommands.AlignToGrid,
				StandardCommands.CenterHorizontally,
				StandardCommands.CenterVertically);
			AddCommandExecutor(new FormatFontCommandExecutor(designerHost), new EventHandler(OnStatusFormat), true,
				CommandGroups.FormatCommands);
			AddCommandExecutor(new FormatFontCommandExecutor(designerHost), new EventHandler(OnStatusForeColor), true,
				FormattingCommands.ForeColor);
			AddCommandExecutor(new FormatFontCommandExecutor(designerHost), new EventHandler(OnStatusBackColor), true,
				FormattingCommands.BackColor);
			AddCommandExecutor(new JustifyCommandExecutor(designerHost), new EventHandler(OnStatusJustifyCommand), false,
				CommandGroups.JustifyCommands);
			AddCommandExecutor(new FontInfoCommandExecutor(designerHost), new EventHandler(OnStatusFormat), false,
				CommandGroups.FontInfoCommands);
			AddCommandExecutor(new ReportCommandExecutor(designerHost), new EventHandler(OnStatusAnySelection), true,
				CommandGroups.RootReportCommands);
			AddCommandExecutor(new TextControlCommandExecutor(designerHost), new EventHandler(OnStatusTextControl), true,
				CommandGroups.TextControlCommands);
			AddCommandExecutor(new ZoomCommandExecutor(designerHost), new EventHandler(OnStatusZoom), true,
				CommandGroups.ZoomCommands);
			AddCommandExecutor(new ReorderBandsCommandExecutor(designerHost), new EventHandler(OnStatusReorderBands), true,
				CommandGroups.ReorderBandsCommands);
			AddCommand(new PasteCommandSetItem(menuService, new EventHandler(OnStatusPaste), designerHost));
			AddCommand(new CommandSetItemWrapper(menuService, new EventHandler(OnStatusAlwaysEnabled), StandardCommands.PropertiesWindow, WrappedCommands.PropertiesWindow));
			AddCommand(new CommandSetItemWrapper(menuService, new EventHandler(OnStatusToolbox), new CommandID(new Guid("5EFC7975-14BC-11CF-9B2B-00AA00573819"), 42), WrappedCommands.Toolbox));
			AddCommand(new CommandSetItem(menuService, null, null, StandardCommands.SelectAll));
			AddCommand(new DeleteCommandSetItem(menuService, new EventHandler(OnStatusDelete), designerHost));
			AddCommand(new CommandSetItem(menuService, null, new EventHandler(OnStatusCut), StandardCommands.Cut));
			AddCommand(new CommandSetItem(menuService, null, new EventHandler(OnStatusCopy), StandardCommands.Copy));
			AddCommand(new CommandSetItem(menuService, null, new EventHandler(OnStatusNotInInplaceMode), MenuCommands.KeyDefaultAction));
			AddCommand(new CommandSetItem(menuService, OnAddNewDataSource, null, ReportCommands.AddNewDataSource));
			AddCommand(new CommandSetItem(menuService, OnShowTab, OnStatusAlwaysEnabled, ReportTabControlCommands.ShowDesignerTab));
			AddCommand(new CommandSetItem(menuService, OnShowTab, OnStatusAlwaysEnabled, ReportTabControlCommands.ShowScriptsTab));
			AddCommand(new CommandSetItem(menuService, OnShowTab, OnStatusAlwaysEnabled, ReportTabControlCommands.ShowPreviewTab));
			AddCommand(new CommandSetItem(menuService, OnShowTab, OnStatusAlwaysEnabled, ReportTabControlCommands.ShowHTMLViewTab));
		}
		internal void OnSelectionChangedCore() {
			this.selectionCount = selectionServ.SelectionCount;
			this.primarySelection = (selectionServ.PrimarySelection is IComponent) ? (IComponent)selectionServ.PrimarySelection : null;
			this.selectedComponents = new ArrayList(selectionServ.GetSelectedComponents());
			this.selectedComponentsWithoutPrimarySelection = new ArrayList(selectionServ.GetSelectedComponents());
			if(this.primarySelection != null)
				this.selectedComponentsWithoutPrimarySelection.Remove(this.primarySelection);
			selectionLocked = false;
			foreColorSupported = backColorSupported = xrControlsOnlySelection = fontSupported = hasTextAlignProperty = true;
			if(selectionCount > 0) {
				AssignSelectionModes(selectionServ.GetSelectedComponents());
				if(selectionLocked) DisableCommands();
			}
			UpdateCommandStatus();
		}
		private void OnSelectionChanged(object sender, EventArgs e) {
			if(menuService == null || ReportDesigner.HostIsLoading(designerHost))
				return;
			OnSelectionChangedCore();
		}
		private void AssignSelectionModes(ICollection selection) {
			foreach(object obj in selection) {
				if(xrControlsOnlySelection && IsUnsupportedControlSelected(obj)) {
					xrControlsOnlySelection = false;
				}
				if(fontSupported && !IsStylePropertySupported(obj, XRComponentPropertyNames.Font))
					fontSupported = false;
				if(hasTextAlignProperty && !IsStylePropertySupported(obj, XRComponentPropertyNames.TextAlignment))
					hasTextAlignProperty = false;
				if(!LockService.GetInstance(designerHost).CanChangeComponent(obj as IComponent)) {
					selectionLocked = true;
					xrControlsOnlySelection = fontSupported = hasTextAlignProperty = false;
				}
				if(foreColorSupported && !IsStylePropertySupported(obj, XRComponentPropertyNames.ForeColor))
					foreColorSupported = false;
				if(backColorSupported && !IsStylePropertySupported(obj, XRComponentPropertyNames.BackColor))
					backColorSupported = false;
			}
		}
		protected virtual bool IsUnsupportedControlSelected(object obj) {
			return !(obj is XRControl) || (obj is Band);
		}
		public void DisableCommands() {
			foreach(CommandSetItem item in commands) {
				item.Disable();
			}
		}
		public void LockCommands() {
			foreach(CommandSetItem item in commands) {
				item.Locked = true;
			}
		}
		public void UnlockCommands(params CommandID[] ids) {
			foreach(CommandSetItem item in commands) {
				if(ids.Length == 0 || Array.IndexOf(ids, item.CommandID) >= 0)
					item.Locked = false;
			}
		}
		#region OnStatus event handlers
		private void OnStatusTextControl(object sender, EventArgs e) {
			OnStatusNotInInplaceMode(sender, e);
			MenuCommand cmd = (MenuCommand)sender;
			if(cmd.Enabled && cmd.Supported) {
				cmd.Enabled = cmd.Supported = selectionServ.PrimarySelection is XRFieldEmbeddableControl;
			}
		}
		void OnStatusToolbox(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			cmd.Enabled = cmd.Supported = !DesignToolHelper.IsEndUserDesigner(this.designerHost);
		}
		private void OnStatusAlwaysEnabled(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			cmd.Enabled = cmd.Supported = true;
		}
		void OnStatusNotInInplaceMode(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			cmd.Enabled = cmd.Supported = !IsInplaceEditorActive();
		}
		private void OnStatusTableRowCommands(object sender, EventArgs e) {
			OnStatusTableCommands(sender, e);
			MenuCommand cmd = (MenuCommand)sender;
			if(!cmd.Enabled && (primarySelection is XRTableRow) && !selectionLocked && selectionCount == 1) 
				cmd.Supported = cmd.Enabled = true;
			if(cmd.Enabled) 
				cmd.Enabled = ValidateInheritance(GetTableBySelection());
		}
		private void OnStatusTableCommands(object sender, EventArgs e) {
			OnStatusSingleSelection(sender, e);
			MenuCommand cmd = (MenuCommand)sender;
			if(selectionLocked || (cmd.Enabled && !(primarySelection is XRTableCell))) 
				cmd.Supported = cmd.Enabled = false;
			if(cmd.Enabled && (primarySelection is XRTableCell) && (primarySelection as XRTableCell).RowSpan > 1) {
				cmd.Supported = cmd.Enabled = false;
			}
		}
		private void OnStatusTableCellCommands(object sender, EventArgs e) {
			OnStatusTableCommands(sender, e);
			MenuCommand cmd = (MenuCommand)sender;
			if(!cmd.Enabled && (primarySelection is XRTableCell) && selectionCount == 1 && cmd.CommandID == Commands.TableCommands.DeleteCell) {
				cmd.Supported = cmd.Enabled = true;
			}
			if(cmd.Enabled)
				cmd.Enabled = ValidateInheritance(GetRowBySelection());
		}
		private void OnStatusTableColumnCommands(object sender, EventArgs e) {
			OnStatusTableCommands(sender, e);
			MenuCommand cmd = (MenuCommand)sender;
			if(cmd.Enabled) {
				XRTable table = GetTableBySelection();
				if(table == null) {
					cmd.Enabled = false;
					return;
				}
				bool valid = true;
				foreach(XRTableRow row in table.Rows) {
					valid &= ValidateInheritance(row);
					if(!valid)
						break;
				}
				cmd.Enabled = valid;
			}
		}
		static bool ValidateInheritance(Component component) {
			return component == null || GetInheritanceLevel(component) != InheritanceLevel.InheritedReadOnly;
		}
		static InheritanceLevel GetInheritanceLevel(Component component) {
			return ((InheritanceAttribute)TypeDescriptor.GetAttributes(component)[typeof(InheritanceAttribute)]).InheritanceLevel;
		}
		XRTable GetTableBySelection() {
			XRTableRow row = GetRowBySelection();
			XRTable table = row != null ? (XRTable)row.Parent : primarySelection as XRTable;
			return table;
		}
		XRTableRow GetRowBySelection() {
			XRTableCell cell = primarySelection as XRTableCell;
			XRTableRow row = cell != null ? (XRTableRow)cell.Parent : primarySelection as XRTableRow;
			return row;
		}
		private void OnStatusCommonTableCommands(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			cmd.Supported = cmd.Enabled = !selectionLocked && TrueForAll(selectedComponents, o => o is XRTableCell || o is XRTableRow || o is XRTable);
		}
		private void OnStatusMultiSelection(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			cmd.Enabled = xrControlsOnlySelection && selectionCount > 1;
			cmd.Enabled &= !IsAssignableFrom(new Type[] { typeof(XRCrossBandControl) }, primarySelection);
			cmd.Supported = cmd.Enabled;
		}
		public void OnStatusAnySelection(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			cmd.Supported = cmd.Enabled = selectionCount > 0;
		}
		#region OnStatus Formatting Component Selection
		public void OnStatusFormattingComponentSelection(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			XtraReport report = designerHost.RootComponent as XtraReport;
			cmd.Supported = report != null;
			if(report != null) {
				if(object.Equals(cmd.CommandID, FormattingComponentCommands.PurgeStyles))
					OnStatusFormattingComponentPurgeStyles(cmd, report);
				else if(object.Equals(cmd.CommandID, FormattingComponentCommands.PurgeFormattingRules))
					OnStatusFormattingComponentPurgeRules(cmd, report);
				else if(object.Equals(cmd.CommandID, FormattingComponentCommands.ClearStyles))
					OnStatusFormattingComponentClearStyles(cmd, report);
				else if(object.Equals(cmd.CommandID, FormattingComponentCommands.ClearFormattingRules))
					OnStatusFormattingComponentClearRules(cmd, report);
				else
					cmd.Enabled = true;
			}
		}
		void OnStatusFormattingComponentClearStyles(MenuCommand cmd, XtraReport report) {
			System.Diagnostics.Debug.Assert(cmd.Supported);
			System.Diagnostics.Debug.Assert(object.Equals(cmd.CommandID, FormattingComponentCommands.ClearStyles));
			System.Diagnostics.Debug.Assert(report != null);
			LockService lockService = LockService.GetInstance(designerHost);
			cmd.Enabled = report.StyleSheet.Count > 0 && lockService.CanDeleteAnyComponent(report.StyleSheet);
		}
		void OnStatusFormattingComponentClearRules(MenuCommand cmd, XtraReport report) {
			System.Diagnostics.Debug.Assert(cmd.Supported);
			System.Diagnostics.Debug.Assert(object.Equals(cmd.CommandID, FormattingComponentCommands.ClearFormattingRules));
			System.Diagnostics.Debug.Assert(report != null);
			LockService lockService = LockService.GetInstance(designerHost);
			cmd.Enabled = report.FormattingRuleSheet.Count > 0 && lockService.CanDeleteAnyComponent(report.FormattingRuleSheet);
		}
		void OnStatusFormattingComponentPurgeStyles(MenuCommand cmd, XtraReport report) {
			System.Diagnostics.Debug.Assert(cmd.Supported);
			System.Diagnostics.Debug.Assert(object.Equals(cmd.CommandID, FormattingComponentCommands.PurgeStyles));
			System.Diagnostics.Debug.Assert(report != null);
			LockService lockService = LockService.GetInstance(designerHost);
			cmd.Enabled = report.GetUnusedStyles().Where(x => lockService.CanDeleteComponent(x)).Count() > 0;
		}
		void OnStatusFormattingComponentPurgeRules(MenuCommand cmd, XtraReport report) {
			System.Diagnostics.Debug.Assert(cmd.Supported);
			System.Diagnostics.Debug.Assert(object.Equals(cmd.CommandID, FormattingComponentCommands.PurgeFormattingRules));
			System.Diagnostics.Debug.Assert(report != null);
			LockService lockService = LockService.GetInstance(designerHost);
			cmd.Enabled = report.GetUnusedRules().Where(x => lockService.CanDeleteComponent(x)).Count() > 0;
		}
		#endregion
		public void OnStatusFieldListSelection(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			XtraReport report = designerHost.RootComponent as XtraReport;
			cmd.Supported = report != null;
			if(report != null) {
				if(object.Equals(cmd.CommandID, FieldListCommands.ClearCalculatedFields))
					OnStatusFieldListClearCalculatedFields(cmd, report);
				else if(object.Equals(cmd.CommandID, FieldListCommands.ClearParameters))
					OnStatusFieldListClearParameters(cmd, report);
				else if(object.Equals(cmd.CommandID, FieldListCommands.DeleteCalculatedField))
					OnStatusFieldListDeleteCalculatedField(cmd);
				else if(object.Equals(cmd.CommandID, FieldListCommands.DeleteParameter))
					OnStatusFieldListDeleteParameter(cmd);
				else
					cmd.Enabled = true;
			}
		}
		void OnStatusFieldListClearCalculatedFields(MenuCommand cmd, XtraReport report) {
			System.Diagnostics.Debug.Assert(cmd.Supported);
			System.Diagnostics.Debug.Assert(object.Equals(cmd.CommandID, FieldListCommands.ClearCalculatedFields));
			System.Diagnostics.Debug.Assert(report != null);
			LockService lockService = LockService.GetInstance(designerHost);
			cmd.Enabled = report.CalculatedFields.Count > 0 && lockService.CanDeleteAnyComponent(report.CalculatedFields);
		}
		void OnStatusFieldListClearParameters(MenuCommand cmd, XtraReport report) {
			System.Diagnostics.Debug.Assert(cmd.Supported);
			System.Diagnostics.Debug.Assert(object.Equals(cmd.CommandID, FieldListCommands.ClearParameters));
			System.Diagnostics.Debug.Assert(report != null);
			LockService lockService = LockService.GetInstance(designerHost);
			cmd.Enabled = report.Parameters.Count > 0 && lockService.CanDeleteAnyComponent(report.Parameters);
		}
		void OnStatusFieldListDeleteCalculatedField(MenuCommand cmd) {
			System.Diagnostics.Debug.Assert(cmd.Supported);
			System.Diagnostics.Debug.Assert(object.Equals(cmd.CommandID, FieldListCommands.DeleteCalculatedField));
			LockService lockService = LockService.GetInstance(designerHost);
			cmd.Enabled = lockService.CanDeleteComponents(selectedComponents);
		}
		void OnStatusFieldListDeleteParameter(MenuCommand cmd) {
			System.Diagnostics.Debug.Assert(cmd.Supported);
			System.Diagnostics.Debug.Assert(object.Equals(cmd.CommandID, FieldListCommands.DeleteParameter));
			LockService lockService = LockService.GetInstance(designerHost);
			cmd.Enabled = lockService.CanDeleteComponents(selectedComponents);
		}
		void OnStatusKeyCommands(object sender, EventArgs e) {
			OnStatusAnySelection(sender, e);
			MenuCommand cmd = (MenuCommand)sender;
			if(cmd.Supported)
				OnStatusNotInInplaceMode(sender, e);
		}
		private void OnStatusSingleSelection(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			cmd.Supported = cmd.Enabled = xrControlsOnlySelection && selectionCount == 1;
		}
		private void OnStatusAnyXRControlSelection(object sender, EventArgs e) {
			OnStatusNotInInplaceMode(sender, e);
			MenuCommand cmd = (MenuCommand)sender;
			if(!cmd.Supported) return;
			cmd.Enabled = xrControlsOnlySelection && selectionCount > 0;
			cmd.Enabled &= !IsAssignableFrom(new Type[] { typeof(XRTableCell), typeof(XRTableRow) }, primarySelection);
			if(cmd.CommandID == StandardCommands.CenterHorizontally || cmd.CommandID == StandardCommands.CenterVertically)
				cmd.Enabled &= !IsAssignableFrom(new Type[] { typeof(XRCrossBandControl) }, primarySelection);
			cmd.Supported = cmd.Enabled;
		}
		private void OnStatusZOrder(object sender, EventArgs e) {
			OnStatusNotInInplaceMode(sender, e);
			MenuCommand cmd = (MenuCommand)sender;
			if(!cmd.Supported) return;
			Type[] types = new Type[] { typeof(XRTableCell), typeof(XRTableRow) };
			cmd.Supported = xrControlsOnlySelection && !IsAssignableFrom(types, primarySelection);
			if(cmd.Supported) {
				XRControl selControl = primarySelection as XRControl;
				cmd.Enabled = (selControl != null && selControl.ControlContainer != null) ? selControl.ControlContainer.Count > 1 : false;
			} else
				cmd.Enabled = false;
		}
		private void OnStatusPaste(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			cmd.Supported = true;
			System.Windows.Forms.IDataObject dataObj = System.Windows.Forms.Clipboard.GetDataObject();
			const string CF_DESIGNER = "CF_DESIGNERCOMPONENTS_V2";
			if(primarySelection == null || !LockService.GetInstance(designerHost).CanChangeComponent(primarySelection))
				cmd.Enabled = false;
			else if(dataObj != null)
				cmd.Enabled = dataObj.GetDataPresent(CF_DESIGNER) ? true : false;
		}
		private void OnStatusDelete(object sender, EventArgs e) {
			OnStatusNotInInplaceMode(sender, e);
			MenuCommand cmd = (MenuCommand)sender;
			if(!cmd.Supported) return;
			cmd.Supported = true;
			if(selectionLocked || !LockService.GetInstance(designerHost).CanDeleteComponents(selectedComponents))
				cmd.Enabled = false;
			else {
				cmd.Enabled = !IsAssignableFrom(DeleteCommandSetItem.UndeletedBandTypes, primarySelection);
			}
		}
		private void OnStatusCopy(object sender, EventArgs e) {
			OnStatusCut(sender, e);
		}
		private void OnStatusCut(object sender, EventArgs e) {
			OnStatusDelete(sender, e);
			MenuCommand cmd = (MenuCommand)sender;
			if(cmd.Enabled) {
				cmd.Enabled = xrControlsOnlySelection && TrueForAll(selectedComponents, o => {
					XRControlDesignerBase designer = designerHost.GetDesigner((IComponent)o) as XRControlDesignerBase;
					return designer == null || designer.CanCutControl;
				});
			}
			cmd.Supported = cmd.Enabled;
		}
		private void OnStatusExecuteVerbCommand(object sender, EventArgs e) {
			bool value = false;
			try {
				if(primarySelection == null) return;
				ComponentDesigner designer = designerHost.GetDesigner(primarySelection) as ComponentDesigner;
				if(designer == null) return;
				foreach(DesignerVerb verb in designer.Verbs) {
					if(verb.Visible && verb.Supported && verb.Enabled) {
						value = true;
						return;
					}
				}
			} finally {
				MenuCommand cmd = (MenuCommand)sender;
				cmd.Supported = cmd.Enabled = value;
			}
		}
		private void OnStatusRichTextBoxCommands(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			cmd.Supported = cmd.Enabled = primarySelection is XRRichTextBase && !selectionLocked;
		}
		private void OnStatusPivotGridCommands(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			cmd.Supported = cmd.Enabled = primarySelection is XRPivotGrid;
		}
		private void OnStatusBandAdd(object sender, EventArgs e) {
			OnStatusAnySelection(sender, e);
			MenuCommand cmd = (MenuCommand)sender;
			if(!cmd.Supported)
				return;
			XRComponentDesigner designer = designerHost.GetDesigner(primarySelection) as XRComponentDesigner;
			BandKind bandKind = BandCommandID.GetBandKind(cmd.CommandID);
			cmd.Supported = designer != null && designer.CanAddBand(bandKind);
			cmd.Enabled = cmd.Supported && !(bandKind == BandKind.SubBand && (designer as XRControlDesignerBase).Locked);
		}
		private void OnStatusBandInsert(object sender, EventArgs e) {
			OnStatusAnySelection(sender, e);
			MenuCommand cmd = (MenuCommand)sender;
			if(!cmd.Supported)
				return;
			if(!TrueForAll(selectedComponents, o => o is Band)) {
				cmd.Supported = cmd.Enabled = false;
				return;
			}
			BandKind bandKind = BandCommandID.GetBandKind(cmd.CommandID);
			XRComponentDesigner designer = ReportDesigner.GetSelectedReportDesigner(designerHost);
			if(designer != null)
				cmd.Enabled = designer.CanAddBand(bandKind);
		}
		void OnStatusReportCommands(object sender, EventArgs e) {
			OnStatusAnySelection(sender, e);
			MenuCommand cmd = (MenuCommand)sender;
			if(!cmd.Supported)
				return;
			if(!TrueForAll(selectedComponents, o => o is Band)) {
				cmd.Supported = cmd.Enabled = false;
				return;
			}
			if(cmd.CommandID == ReportCommands.InsertDetailReport) {
				XRComponentDesigner designer = ReportDesigner.GetSelectedReportDesigner(designerHost);
				if(designer != null)
					cmd.Enabled = designer.CanAddBand(BandKind.DetailReport);
			}
		}
		private bool OnCommandStatusCheck(CommandID cmdID) {
			CommandExecutorBase executor = (CommandExecutorBase)execHT[cmdID];
			if(executor == null)
				return false;
			ICollection selection = selectionServ.GetSelectedComponents();
			bool cmdChecked = false;
			XRControl ctrl = selectionServ.PrimarySelection as XRControl;
			if(ctrl != null)
				cmdChecked = executor.OnUpdateCommandCheck(ctrl, cmdID);
			foreach(object obj in selection) {
				ctrl = obj as XRControl;
				if(ctrl != null && executor.OnUpdateCommandCheck(ctrl, cmdID) != cmdChecked) {
					cmdChecked = false;
					break;
				}
			}
			return cmdChecked;
		}
		private void OnStatusFormat(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			cmd.Supported = cmd.Enabled = fontSupported;
			cmd.Checked = OnCommandStatusCheck(cmd.CommandID);
		}
		void OnStatusForeColor(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			cmd.Supported = cmd.Enabled = foreColorSupported;
			cmd.Checked = OnCommandStatusCheck(cmd.CommandID);
		}
		void OnStatusBackColor(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			cmd.Supported = cmd.Enabled = backColorSupported;
			cmd.Checked = OnCommandStatusCheck(cmd.CommandID);
		}
		private void OnStatusJustifyCommand(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			cmd.Supported = cmd.Enabled = hasTextAlignProperty || IsInplaceEditorActive();
			cmd.Checked = OnCommandStatusCheck(cmd.CommandID);
		}
		private void OnStatusZoom(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			if(!(selectionServ.PrimarySelection is Band)) {
				cmd.Supported = cmd.Enabled = false;
				return;
			}
			ZoomService zoomService = ZoomService.GetInstance(designerHost);
			if(object.Equals(cmd.CommandID, ZoomCommands.ZoomIn))
				cmd.Enabled = zoomService.CanZoomIn;
			if(object.Equals(cmd.CommandID, ZoomCommands.ZoomOut))
				cmd.Enabled = zoomService.CanZoomOut;
			bool canChangeZoomFactor = zoomService.CanChangeZoomFactor;
			if(!canChangeZoomFactor)
				cmd.Enabled = false;
			if(object.Equals(cmd.CommandID, ZoomCommands.Zoom))
				cmd.Enabled = canChangeZoomFactor;
			cmd.Supported = cmd.Enabled;
		}
		private void OnStatusReorderBands(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			IMoveableBand moveableBand = selectionServ.PrimarySelection as IMoveableBand;
			cmd.Supported = moveableBand != null;
			if(moveableBand != null) {
				if(object.Equals(cmd.CommandID, ReorderBandsCommands.MoveUp))
					cmd.Enabled = moveableBand.CanBeMoved(BandReorderDirection.Up);
				else if(object.Equals(cmd.CommandID, ReorderBandsCommands.MoveDown))
					cmd.Enabled = moveableBand.CanBeMoved(BandReorderDirection.Down);
			}
		}
		public bool IsInplaceEditorActive() {
			XRControl control = selectionServ.PrimarySelection as XRControl;
			if(control != null) {
				XRTextControlBaseDesigner designer = designerHost.GetDesigner(control) as XRTextControlBaseDesigner;
				return designer != null && designer.IsInplaceEditingMode;
			}
			return false;
		}
		#endregion
		void OnAddNewDataSource(object sender, CommandExecuteEventArgs e) {
			var lookAndFeelService = (ILookAndFeelService)this.designerHost.GetService(typeof(ILookAndFeelService));
			var uiService = (IUIService)this.designerHost.GetService(typeof(IUIService));
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			var runner = new XtraReportDataSourceWizardRunner(lookAndFeelService.LookAndFeel, owner);
			IParameterService parameterService = (IParameterService)designerHost.GetService(typeof(IParameterService));
			byte[] key = GetType().Assembly.GetName().GetPublicKeyToken();
			ISolutionTypesProvider solutionTypesProvider = new RuntimeSolutionTypesProvider(() => EntityServiceHelper.GetTypes(Assembly.GetEntryAssembly(), type => {
				byte[] key2 = type.Assembly.GetName().GetPublicKeyToken();
				return !key.SequenceEqual<byte>(key2);
			}));
			IConnectionStringsProvider connectionStringsProvider = new RuntimeConnectionStringsProvider();
			var connectionStorageService = (IConnectionStorageService)this.designerHost.GetService(typeof(IConnectionStorageService)) ?? new ConnectionStorageService();
			var dbSchemaProvider = (IDBSchemaProvider)this.designerHost.GetService(typeof(IDBSchemaProvider)) ?? new DBSchemaProvider();
			var repositoryItemsProvider = (IRepositoryItemsProvider)designerHost.GetService(typeof(IRepositoryItemsProvider)) ?? DefaultRepositoryItemsProvider.Instance;
			SqlWizardOptions options = (designerHost.GetService(typeof(ISqlWizardOptionsProvider)) as ISqlWizardOptionsProvider).GetSqlWizardOptions();
			var client = new DataSourceWizardClientUI(connectionStorageService, parameterService, solutionTypesProvider, connectionStringsProvider, dbSchemaProvider) {
					RepositoryItemsProvider = repositoryItemsProvider,
					PropertyGridServices = designerHost,
					DataSourceTypes = DataSourceTypes.All,
					Options = options
				};
			ICustomQueryValidator validator = (ICustomQueryValidator)designerHost.GetService(typeof(ICustomQueryValidator));
			if(validator != null)
				client.CustomQueryValidator = validator;
			IWizardCustomizationService serv = designerHost.GetService(typeof(IWizardCustomizationService)) as IWizardCustomizationService;
			var dataSourceModel = new DataSourceModel();
			if(runner.Run(client, dataSourceModel, customization => {
				DataSourceTypes dataSourceTypes = (DataSourceTypes)customization.Resolve(typeof(DataSourceTypes));
				if(dataSourceTypes.Count == 1) {
					IEnumerable<SqlDataConnection> connections = (IEnumerable<SqlDataConnection>) customization.Resolve(typeof(IEnumerable<SqlDataConnection>));
					var dataSourceType = dataSourceTypes.First();
					dataSourceModel.DataSourceType = dataSourceType;
					customization.StartPage = DataSourceTypeHelper.GetNextPageType<DataSourceModel>(dataSourceType, connections.Any());
				}
				customization.RegisterPageView<IChooseObjectConstructorPageView, ChooseObjectConstructorPageViewEx>();
				serv.CustomizeDataSourceWizardSafely(customization);
			})) {
				DataComponentCreator.SaveConnectionIfShould(runner.WizardModel, connectionStorageService);
				object dataSource; string dataMember;
				serv.CreateDataSourceSafely(runner.WizardModel, out dataSource, out dataMember);
				DesignerTransaction designerTransaction = this.designerHost.CreateTransaction();
				try {
					if(dataSource is IComponent)
						this.designerHost.Container.Add((IComponent)dataSource);
					IDataContainer dataContainer = e.GetValue<IDataContainer>();
					if(dataContainer != null) {
						TypeDescriptor.GetProperties(dataContainer)["DataSource"].SetValue(dataContainer, dataSource);
						TypeDescriptor.GetProperties(dataContainer)["DataMember"].SetValue(dataContainer, dataMember);
					}
				} catch {
					designerTransaction.Cancel();
				} finally {
					designerTransaction.Commit();
				}
			}
		}
		void OnShowTab(object sender, EventArgs e) {
			ReportTabControl reportTabControl = (ReportTabControl)designerHost.GetService(typeof(ReportTabControl));
			System.Diagnostics.Debug.Assert(reportTabControl != null);
			if(reportTabControl != null)
				reportTabControl.SelectedIndex = ((MenuCommand)sender).CommandID.ID;
		}
		static bool TrueForAll(IList list, Func<object, bool> action) {
			if(list == null)
				return false;
			foreach(object o in list) {
				if(!action(o))
					return false;
			}
			return true;
		}
	}
	static class CommandExecuteEventArgsExtension {
		public static T GetValue<T>(this CommandExecuteEventArgs args) where T : class {
			return args != null ? args.Args.First<object>(item => item is T) as T: null;
		}
	}
}
