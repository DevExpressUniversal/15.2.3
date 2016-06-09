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
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Wizard;
namespace DevExpress.XtraCharts.Design {
	class ChartMenuCommand : MenuCommand, IDisposable {
		static Hashtable chartCommandsStacks = new Hashtable();
		IMenuCommandService service;
		EventHandler handler;
		bool handled;
		public ChartMenuCommand(IMenuCommandService service, CommandID commandID, EventHandler handler) : base(handler, commandID) {
			this.service = service;
			this.handler = handler;
			MenuCommand command = service.FindCommand(commandID);
			if (command == null)
				return;
			int commandServiceHashCode = service.GetHashCode();
			Hashtable serviceCommandsStacks = (Hashtable)chartCommandsStacks[commandServiceHashCode];
			if (serviceCommandsStacks == null) {
				serviceCommandsStacks = new Hashtable();
				chartCommandsStacks[commandServiceHashCode] = serviceCommandsStacks;
			}
			ArrayList commandStack = (ArrayList)serviceCommandsStacks[commandID];
			if (commandStack == null) {
				commandStack = new ArrayList();
				serviceCommandsStacks[commandID] = commandStack;
			}
			if (commandStack.Count == 0)
				commandStack.Insert(0, command);
			commandStack.Insert(0, this);
			service.RemoveCommand(command);
			service.AddCommand(this);
		}
		public void Handled() {
			handled = true;
		}
		public void Dispose() {
			if (service == null)
				return;
			Hashtable serviceCommandsStacks = (Hashtable)chartCommandsStacks[service.GetHashCode()];
			if (serviceCommandsStacks == null)
				return;
			ArrayList commandStack = (ArrayList)serviceCommandsStacks[CommandID];
			if (commandStack == null || !commandStack.Contains(this))
				return;
			if (commandStack[0] == this) {
				service.RemoveCommand(this);
				if (commandStack.Count > 1) {
					service.AddCommand((MenuCommand)commandStack[1]);
					if (commandStack.Count == 2) {
						commandStack.Clear();
						return;
					}
				}
			}
			commandStack.Remove(this);
		}
		public override void Invoke() {
			handled = false;
			base.Invoke();
			if (handled || service == null)
				return;
			Hashtable serviceCommandsStacks = (Hashtable)chartCommandsStacks[service.GetHashCode()];
			if (serviceCommandsStacks == null)
				return;
			ArrayList commandStack = (ArrayList)serviceCommandsStacks[CommandID];
			if (commandStack == null)
				return;
			int index = commandStack.IndexOf(this);
			if (index >= 0 && index < commandStack.Count - 1)
				((MenuCommand)commandStack[index + 1]).Invoke();
		}
	}
	[System.Security.SecuritySafeCritical]
	public class ChartDesigner : IDisposable {
		static void SubstitutePropertyDescriptor(IDictionary properties, string propertyName) {
			PropertyDescriptor descriptor = properties[propertyName] as PropertyDescriptor;
			if (descriptor != null) {
				descriptor = new ReadOnlyPropertyDescriptor(descriptor);
				properties[propertyName] = descriptor;
			}
		}
		public static void ShowErrorMessage(string message, IChartContainer container) {
			if (container != null)
				container.ShowErrorMessage(message, string.Empty);				
			else
				MessageBox.Show(message, ChartLocalizer.GetString(ChartStringId.IOCaption), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
		public static void PostFilterProperties(IComponent chartComponent, IDictionary properties) {
			if (chartComponent == null || chartComponent.Site == null)
				return;
			IInheritanceService inheritanceService = chartComponent.Site.GetService(typeof(IInheritanceService)) as IInheritanceService;
			if (inheritanceService == null)
				return;
			InheritanceAttribute inheritanceAttribute = inheritanceService.GetInheritanceAttribute(chartComponent);
			if (inheritanceAttribute == null || inheritanceAttribute.InheritanceLevel == InheritanceLevel.NotInherited)
				return;
			SubstitutePropertyDescriptor(properties, "Diagram");
			SubstitutePropertyDescriptor(properties, "Series");
			SubstitutePropertyDescriptor(properties, "SeriesSerializable");
			SubstitutePropertyDescriptor(properties, "SeriesTemplate");
			SubstitutePropertyDescriptor(properties, "PaletteName");
		}
		public static InheritanceAttribute GetInheritanceAttribute(IChartContainer container) {
			IComponent component = container as IComponent;
			if (component == null || container.ServiceProvider == null)
				return InheritanceAttribute.NotInherited;
			ILockService lockService = container.ServiceProvider.GetService(typeof(ILockService)) as ILockService;
			if (lockService != null && !lockService.CanChangeComponents(new IComponent[] { component }))
				return InheritanceAttribute.InheritedReadOnly;
			IInheritanceService inheritanceService = container.ServiceProvider.GetService(typeof(IInheritanceService)) as IInheritanceService;
			return inheritanceService == null ? InheritanceAttribute.NotInherited : inheritanceService.GetInheritanceAttribute(component);
		}
		public static bool CanModifyElement(object element, IChartContainer container) {
			if (element != null && !container.Chart.Contains(element))
				return true;
			if (GetInheritanceAttribute(container) == InheritanceAttribute.InheritedReadOnly)
				return false;
			string propertyName;
			if (element is Series || element is SeriesLabelBase)
				propertyName = "SeriesSerializable";
			else if (element is Diagram || element is AxisBase)
				propertyName = "Diagram";
			else
				return true;
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(container);
			foreach (PropertyDescriptor descriptor in properties)
				if (descriptor.Name == propertyName)
					return descriptor.SerializationVisibility != DesignerSerializationVisibility.Hidden;
			return true;
		}
		Chart Chart { get { return control.Chart; } }
		IChartContainer control;
		ISelectionService selectionService = null;
		IComponentChangeService changeService;
		IDesignerHost designerHost = null;
		IInheritanceService inheritanceService = null;
		DesignSurface designSurface = null;
		FakeComponent lastSelectedFakeComponent = null;
		ChartMenuCommand keyCancelCommand = null;
		ChartMenuCommand deleteCommand = null;
		ChartDesignerActionList designerActionList = null;
		MenuCommand[] workersCommands = new MenuCommand[] {};
		bool keyCancelPressed = false;
		bool controlChangedInTransaction = false;
		public ChartDesigner(IChartContainer control) {
			this.control = control;
			if (control != null) {
				IServiceProvider serviceProvider = control.ServiceProvider;
				if (serviceProvider != null) {
					selectionService = serviceProvider.GetService(typeof(ISelectionService)) as ISelectionService;
					if (selectionService != null)
						selectionService.SelectionChanged += new EventHandler(OnSelectionChanged);
					changeService = serviceProvider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
					if (changeService != null) {
						changeService.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
						changeService.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
					}
					designerHost = serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
					if (designerHost != null) {
						designerHost.TransactionOpened += new EventHandler(OnTransactionOpened);
						designerHost.TransactionClosed += new DesignerTransactionCloseEventHandler(OnTransactionClosed);
						if (designerHost.Loading)
							designerHost.LoadComplete += new EventHandler(OnLoadComplete);
						else
							ProcessMenuCommands();
					}
					inheritanceService = serviceProvider.GetService(typeof(IInheritanceService)) as IInheritanceService;
					designSurface = serviceProvider.GetService(typeof(DesignSurface)) as DesignSurface;
					if (designSurface != null)
						designSurface.Flushed += new EventHandler(OnFlushed);
				}
			}
			control.InteractionProvider.ObjectHotTracked += new HotTrackEventHandler(OnObjectHotTracked);
			control.InteractionProvider.ObjectSelected += new HotTrackEventHandler(OnObjectSelected);
			Chart.ViewController.Update();
			Chart.ViewController.OnEndLoading();
		}
		public void Dispose() {
			workersCommands = new MenuCommand[] {};
			if (keyCancelCommand != null) {
				keyCancelCommand.Dispose();
				keyCancelCommand = null;
			}
			if (deleteCommand != null) {
				deleteCommand.Dispose();
				deleteCommand = null;
			}
			if (control != null) {
				if (selectionService != null)
					selectionService.SelectionChanged -= new EventHandler(OnSelectionChanged);
				if (changeService != null) {
					changeService.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
					changeService.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
				}
				if (designerHost != null) {
					designerHost.TransactionOpened -= new EventHandler(OnTransactionOpened);
					designerHost.TransactionClosed -= new DesignerTransactionCloseEventHandler(OnTransactionClosed);
				}
				if (designSurface != null)
					designSurface.Flushed -= new EventHandler(OnFlushed);
				control.InteractionProvider.ObjectHotTracked -= new HotTrackEventHandler(OnObjectHotTracked);
				control.InteractionProvider.ObjectSelected -= new HotTrackEventHandler(OnObjectSelected);
			}
		}
		public void SetDesignerActionList(ChartDesignerActionList designerActionList) {
			this.designerActionList = designerActionList;
		}
		void ProcessMenuCommands() {
			IMenuCommandService menuCommandService = MenuCommandServiceHelper.RegisterMenuCommandService(designerHost);
			if (menuCommandService == null)
				return;
			keyCancelCommand = new ChartMenuCommand(menuCommandService, MenuCommands.KeyCancel, new EventHandler(OnKeyCancel));
			deleteCommand = new ChartMenuCommand(menuCommandService, StandardCommands.Delete, new EventHandler(OnDelete));
			MenuCommand cutCommand = menuCommandService.FindCommand(StandardCommands.Cut);
			MenuCommand copyCommand = menuCommandService.FindCommand(MenuCommands.Copy);
			MenuCommand pasteCommand = menuCommandService.FindCommand(StandardCommands.Paste);
			workersCommands = new MenuCommand[] { cutCommand, copyCommand, pasteCommand };
		}
		void SelectObject(object obj) {
			if (obj != null && selectionService != null && obj != lastSelectedFakeComponent && control.Site != null) {
				foreach (IComponent comp in control.Site.Container.Components) {
					IChartContainer chart = comp as IChartContainer;
					if (chart != null && chart != control)
						chart.Chart.ClearSelection();
				}
				IComponent component;
				if (typeof(IChartContainer).IsAssignableFrom(obj.GetType()))
					component = (IComponent)obj;
				else {
					bool canModify = CanModifyElement(obj, Chart.Container);
					component = canModify ? new FakeComponent(obj, (IComponent)control) : (IComponent)control;
				}
				FakeComponent fakeComponent = component as FakeComponent;
				if (fakeComponent != null || lastSelectedFakeComponent != null)
					selectionService.SetSelectedComponents(new object[] { component }, SelectionTypes.Replace);
				lastSelectedFakeComponent = fakeComponent;
			}
		}
		public object GetSelectedObject() {
			if (selectionService != null) {
				IEnumerator enumerator = selectionService.GetSelectedComponents().GetEnumerator();
				if (enumerator != null && enumerator.MoveNext())
					return enumerator.Current;
			}
			return null;
		}
		public void SelectChartControl() {
			Chart.SetObjectSelection(Chart, false);
			SelectObject(control);
		}
		void ClearHitTestSelection() {
			Chart chart = Chart;
			if (chart != null)
				chart.ClearSelection();
		}
		bool IsInheritedComponent(IComponent component) {
			if (inheritanceService == null)
				return false;
			FakeComponent fakeComponent = component as FakeComponent;
			return inheritanceService.GetInheritanceAttribute(fakeComponent == null ? component : fakeComponent.Parent).InheritanceLevel != InheritanceLevel.NotInherited;
		}
		bool RemoveElementFromCollection(ChartElement element, ChartCollectionBase collection, string transactionName) {
			CollectionItemDeleteAction deleteAction = new CollectionItemDeleteAction(element, collection, control, designerHost, changeService);
			bool success = deleteAction.PerformAction(transactionName);
			ChartDesignHelper.SelectHitTestable(control, collection);
			return success;
		}
		bool DeleteAnnotation(Annotation annotationToDelete) {
			AnnotationRepository annotationCollection = Chart.AnnotationRepository;
			return annotationCollection.Contains(annotationToDelete) &&
				RemoveElementFromCollection(annotationToDelete, annotationCollection, ChartLocalizer.GetString(ChartStringId.TrnAnnotationDeleted));
		}
		bool DeleteSeries(Series seriesToDelete) {
			SeriesCollection seriesCollection = Chart.Series;
			return seriesCollection.Contains(seriesToDelete) &&
				RemoveElementFromCollection(seriesToDelete, seriesCollection, ChartLocalizer.GetString(ChartStringId.TrnSeriesDeleted));
		}
		bool DeleteChartTitle(ChartTitle titleToDelete) {
			ChartTitleCollection titleCollection = Chart.Titles;
			return titleCollection.Contains(titleToDelete) &&
				RemoveElementFromCollection(titleToDelete, titleCollection, ChartLocalizer.GetString(ChartStringId.TrnChartTitleDeleted));
		}
		bool DeleteSeriesTitle(SeriesTitle titleToDelete) {
			SimpleDiagramSeriesViewBase view = ChartDesignHelper.GetOwner<SimpleDiagramSeriesViewBase>(titleToDelete);
			if (view == null)
				return false;
			SeriesTitleCollection titleCollection = view.Titles;
			return titleCollection.Contains(titleToDelete) &&
				RemoveElementFromCollection(titleToDelete, titleCollection, ChartLocalizer.GetString(ChartStringId.TrnSeriesTitleDeleted));
		}
		bool DeleteConstantLine(ConstantLine constantLineToDelete, ConstantLineCollection collection) {
			return collection.Contains(constantLineToDelete) &&
				RemoveElementFromCollection(constantLineToDelete, collection, ChartLocalizer.GetString(ChartStringId.TrnConstantLineDeleted));
		}
		bool DeleteConstantLine(ConstantLine constantLineToDelete) {
			XYDiagram diagram = Chart.Diagram as XYDiagram;
			if (diagram == null)
				return false;
			if (DeleteConstantLine(constantLineToDelete, diagram.AxisX.ConstantLines))
				return true;
			if (DeleteConstantLine(constantLineToDelete, diagram.AxisY.ConstantLines))
				return true;
			foreach (SecondaryAxisX secondaryAxisX in diagram.SecondaryAxesX)
				if (DeleteConstantLine(constantLineToDelete, secondaryAxisX.ConstantLines))
					return true;
			foreach (SecondaryAxisY secondaryAxisY in diagram.SecondaryAxesY)
				if (DeleteConstantLine(constantLineToDelete, secondaryAxisY.ConstantLines))
					return true;
			return false;
		}
		bool DeleteSecondaryAxis(AxisBase axis, SecondaryAxisCollection axisCollection, string transactionName) {
			return ((IList)axisCollection).Contains(axis) && RemoveElementFromCollection(axis, axisCollection, transactionName);
		}
		bool DeleteSecondaryAxisX(SecondaryAxisX secondaryAxisX) {
			XYDiagram diagram = Chart.Diagram as XYDiagram;
			return diagram != null && 
				DeleteSecondaryAxis(secondaryAxisX, diagram.SecondaryAxesX, ChartLocalizer.GetString(ChartStringId.TrnSecondaryAxisXDeleted));
		}
		bool DeleteSecondaryAxisY(SecondaryAxisY secondaryAxisY) {
			XYDiagram diagram = Chart.Diagram as XYDiagram;
			return diagram != null && 
				DeleteSecondaryAxis(secondaryAxisY, diagram.SecondaryAxesY, ChartLocalizer.GetString(ChartStringId.TrnSecondaryAxisYDeleted));
		}
		bool DeleteSwiftPlotDiagramSecondaryAxisX(SwiftPlotDiagramSecondaryAxisX secondaryAxisX) {
			SwiftPlotDiagram diagram = Chart.Diagram as SwiftPlotDiagram;
			return diagram != null &&
				DeleteSecondaryAxis(secondaryAxisX, diagram.SecondaryAxesX, ChartLocalizer.GetString(ChartStringId.TrnSecondaryAxisXDeleted));
		}
		bool DeleteSwiftPlotDiagramSecondaryAxisY(SwiftPlotDiagramSecondaryAxisY secondaryAxisY) {
			SwiftPlotDiagram diagram = Chart.Diagram as SwiftPlotDiagram;
			return diagram != null &&
				DeleteSecondaryAxis(secondaryAxisY, diagram.SecondaryAxesY, ChartLocalizer.GetString(ChartStringId.TrnSecondaryAxisYDeleted));
		}
		bool DeletePane(XYDiagramPane pane) {
			XYDiagram2D diagram = Chart.Diagram as XYDiagram2D;
			if (diagram == null)
				return false;
			XYDiagramPaneCollection paneCollection = diagram.Panes;
			return paneCollection.Contains(pane) &&
				RemoveElementFromCollection(pane, paneCollection, ChartLocalizer.GetString(ChartStringId.TrnPaneDeleted));
		}
		void UpdateCommands(object selectedObject) {
			bool isCommandSupported = !(selectedObject is FakeComponent);
			foreach (MenuCommand command in workersCommands)
				if (command != null)
					command.Supported = isCommandSupported;
		}
		void OnFlushed(object sender, EventArgs args) {
			FakeComponent selectedObject = GetSelectedObject() as FakeComponent;
			if (selectedObject != null && selectedObject.Parent == control)
				SelectChartControl();
		}
		void OnObjectHotTracked(object sender, HotTrackEventArgs args) {
			if (!CanModifyElement(args.Object, Chart.Container))
				args.Cancel = true;
		}
		void OnObjectSelected(object sender, HotTrackEventArgs args) {
			object element = args.Object;
			if (CanModifyElement(element, Chart.Container))
				SelectObject(element);
			else
				args.Cancel = true;
		}
		void OnSelectionChanged(object sender, EventArgs args) {
			object obj = GetSelectedObject();
			if (obj == null)
				ClearHitTestSelection();
			if (!(obj is FakeComponent)) {
				if (obj is IChartContainer)
					Chart.SetObjectSelection(obj, false);
				else if (!keyCancelPressed || lastSelectedFakeComponent == null)
					ClearHitTestSelection();
				else {
					ChartElement element = lastSelectedFakeComponent.Object as ChartElement;
					if (element == null)
						SelectChartControl();
					else
						ChartDesignHelper.SelectOwnerHitTestable(control, element);
				}
				lastSelectedFakeComponent = GetSelectedObject() as FakeComponent;
			}
			UpdateCommands(obj);
			keyCancelPressed = false;
		}
		void OnComponentChanged(object sender, ComponentChangedEventArgs args) {
			if (args.Component == control)
				controlChangedInTransaction = true;
		}
		void OnComponentRemoved(object sender, ComponentEventArgs args) {
			if (!(args.Component is IChartContainer)) 
				Chart.DataContainer.OnObjectDeleted(args.Component);
		}
		void OnTransactionOpened(object sender, EventArgs args) {
			controlChangedInTransaction = false;
		}
		void OnTransactionClosed(object sender, DesignerTransactionCloseEventArgs args) {
			if (!controlChangedInTransaction)
				return;
			object obj = GetSelectedObject();
			if (obj == null) 
				return;
			FakeComponent comp = obj as FakeComponent;
			if (comp == null || comp.Parent != control)
				return;
			foreach (IComponent component in control.Site.Container.Components) {
				IChartContainer container = component as IChartContainer;
				if (container != null && container.Chart.Contains(comp.Object))
					return;
			}
			SelectChartControl();
		}
		void OnLoadComplete(object sender, EventArgs args) {
			designerHost.LoadComplete -= new EventHandler(OnLoadComplete);
			ProcessMenuCommands();
		}
		void OnKeyCancel(object sender, EventArgs args) {
			keyCancelPressed = true;
		}
		void OnDelete(object sender, EventArgs args) {
			FakeComponent component = GetSelectedObject() as FakeComponent;
			if (component == null)
				return;
			bool completed = false;
			IComponent parentComponent = component.Parent;
			if (IsInheritedComponent(component))
				SelectObject(component.Parent);
			else if (component.Object is Annotation)
				completed = DeleteAnnotation((Annotation)component.Object);
			else if (component.Object is Series)
				completed = DeleteSeries((Series)component.Object);
			else if (component.Object is ChartTitle)
				completed = DeleteChartTitle((ChartTitle)component.Object);
			else if (component.Object is SeriesTitle)
				completed = DeleteSeriesTitle((SeriesTitle)component.Object);
			else if (component.Object is ConstantLine)
				completed = DeleteConstantLine((ConstantLine)component.Object);
			else if (component.Object is SecondaryAxisX)
				completed = DeleteSecondaryAxisX((SecondaryAxisX)component.Object);
			else if (component.Object is SecondaryAxisY)
				completed = DeleteSecondaryAxisY((SecondaryAxisY)component.Object);
			else if (component.Object is SwiftPlotDiagramSecondaryAxisX)
				completed = DeleteSwiftPlotDiagramSecondaryAxisX((SwiftPlotDiagramSecondaryAxisX)component.Object);
			else if (component.Object is SwiftPlotDiagramSecondaryAxisY)
				completed = DeleteSwiftPlotDiagramSecondaryAxisY((SwiftPlotDiagramSecondaryAxisY)component.Object);
			else if (component.Object is XYDiagramPane)
				completed = DeletePane((XYDiagramPane)component.Object);
			else if (component.Object is Indicator) {
				Indicator indicator = (Indicator)component.Object;
				completed = RemoveElementFromCollection(indicator, 
					indicator.View.Indicators, ChartLocalizer.GetString(ChartStringId.TrnIndicatorDeleted));
			}
			else if (component.Object is XYDiagram2D) {
				ShowErrorMessage(ChartLocalizer.GetString(ChartStringId.IODeleteDefaultPane), parentComponent as IChartContainer);
				SelectObject(component.Object);
				completed = true;
			}
			else if (component.Object is AxisX || component.Object is AxisY || component.Object is SwiftPlotDiagramAxisX || component.Object is SwiftPlotDiagramAxisY) {
				ShowErrorMessage(ChartLocalizer.GetString(ChartStringId.IODeleteAxis), parentComponent as IChartContainer);
				SelectObject(component.Object);
				completed = true;
			}
			else if (designerHost != null) {
				SelectObject(control.Parent);
				designerHost.DestroyComponent(control as IComponent);
				completed = true;
			}
			if (completed) {
				ChartMenuCommand command = sender as ChartMenuCommand;
				if (command != null)
					command.Handled();
			}
		}
		internal void OnAnnotations() {
			Chart chart = Chart;
			using (AnnotationCollectionEditorForm form = new AnnotationCollectionEditorForm(chart.AnnotationRepository)) {
				form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)control.RenderProvider.LookAndFeel;
				form.Initialize(chart);
				DesignerTransaction transaction = null;
				if (designerHost != null && changeService != null)
					transaction = designerHost.CreateTransaction(ChartLocalizer.GetString(ChartStringId.TrnAnnotationsChanged));
				try {
					if (transaction != null)
						chart.PerformChanging();
					form.ShowDialog();
					chart.PerformChanged();
				}
				catch (Exception e) {
					if (transaction != null) {
						transaction.Cancel();
						transaction = null;
					}
					ShowErrorMessage(e.Message, control);
				}
				if (transaction != null)
					transaction.Commit();
			}
			SelectChartControl();
		}		
		internal void OnSeries() {
			Chart chart = Chart;
			using (SeriesCollectionForm form = new SeriesCollectionForm(chart)) {
				DesignerTransaction transaction = null;
				if (designerHost != null && changeService != null)
					transaction = designerHost.CreateTransaction(ChartLocalizer.GetString(ChartStringId.TrnSeriesChanged));
				try {
					if (transaction != null)
						chart.PerformChanging();
					form.ShowDialog();
					chart.PerformChanged();
				}
				catch (Exception e) {
					if (transaction != null) {
						transaction.Cancel();
						transaction = null;
					}
					ShowErrorMessage(e.Message, control);
				}
				if (transaction != null)
					transaction.Commit();
			}
			SelectChartControl();
		}		
		internal void OnResetLegendPointOptions() {
			DesignerTransaction transaction = null;
			if (designerHost != null)
				transaction = designerHost.CreateTransaction(ChartLocalizer.GetString(ChartStringId.TrnLegendPointOptionsReset));
			try {
				if (transaction != null) {
					transaction.Commit();
					transaction = null;
				}
			}
			catch {
				if (transaction != null)
					transaction.Cancel();
			}
		}
		internal void OnEditPalettes() {
			Chart chart = Chart;
			if (chart != null) {
				PaletteRepository repository = chart.PaletteRepository;
				if (repository != null) {
					using (PaletteEditorForm form = new PaletteEditorForm(chart.Container, repository)) {
						if (chart.Palette != null)
							form.CurrentPalette = chart.Palette;
						form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)control.RenderProvider.LookAndFeel;
						DesignerTransaction transaction = null;
						if (designerHost != null && changeService != null)
							transaction = designerHost.CreateTransaction(ChartLocalizer.GetString(ChartStringId.TrnPalettesChanged));
						try {
							if (transaction != null)
								changeService.OnComponentChanging(control, null);
							form.StartPosition = FormStartPosition.CenterScreen;
							form.ShowDialog();
						} catch (Exception e) {
							if (transaction != null) {
								transaction.Cancel();
								transaction = null;
							}
							ShowErrorMessage(e.Message, control);
							return;
						}
						Palette palette = form.CurrentPalette;
						if (palette != null && palette != chart.Palette) {
							chart.Palette = palette;
							foreach (ChartAppearance appearance in chart.AppearanceRepository) {
								if (appearance.PaletteName == palette.Name) {
									chart.Appearance = appearance;
									break;
								}
							}
						}
						changeService.OnComponentChanged(control, null, null, null);
						if (transaction != null)
							transaction.Commit();
					}
				}
			}
		}
		internal void OnDesigner() {
			DevExpress.XtraCharts.Designer.ChartDesigner designer = new DevExpress.XtraCharts.Designer.ChartDesigner(control, designerHost);
			designer.ShowDialog();
		}
		internal void OnSaveLayout() {
			Chart chart = Chart;
			if (chart != null) {
				SaveFileDialog dialog = new SaveFileDialog();
				dialog.Filter = Constants.SaveLoadLayoutFileDialogFilter;
				if (dialog.ShowDialog() == DialogResult.OK) {
					using (Stream stream = new FileStream(dialog.FileName, FileMode.Create))
						chart.SaveLayout(stream);
				}
			}
		}
		internal void OnLoadLayout() {
			Chart chart = Chart;
			if (chart != null) {
				OpenFileDialog dialog = new OpenFileDialog();
				dialog.Filter = Constants.SaveLoadLayoutFileDialogFilter;
				if (dialog.ShowDialog() == DialogResult.OK) {
					if (designerHost != null) {
						DesignerTransaction transaction = designerHost.CreateTransaction(ChartLocalizer.GetString(ChartStringId.TrnLoadLayout));
						try {
							using (Stream stream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
								chart.LoadLayout(stream);
							if (transaction != null) {
								transaction.Commit();
								transaction = null;
							}
						}
						catch (Exception ex) {
							if (transaction != null) {
								transaction.Cancel();
								transaction = null;
							}
							ShowErrorMessage(ex.Message, control);
						}
					}
				}
			}
		}		
	}
	[System.Security.SecurityCritical]
	public abstract class ChartDesignerActionList : DesignerActionList {
		ChartDesigner chartDesigner;
		bool annotationsActionEnabled;
		bool seriesActionEnabled;
		bool editPalettesEnabled;
		bool wizardEnabled;
		bool saveLayoutEnabled;
		bool loadLayoutEnabled;
		public bool AnnotationsActionEnabled { get { return annotationsActionEnabled; } set { annotationsActionEnabled = value; } }
		public bool SeriesActionEnabled { get { return seriesActionEnabled; } set { seriesActionEnabled = value; } }
		public bool EditPalettesEnabled { get { return editPalettesEnabled; } set { editPalettesEnabled = value; } }
		public bool WizardEnabled { get { return wizardEnabled; } set { wizardEnabled = value; } }
		public bool SaveLayoutEnabled { get { return saveLayoutEnabled; } set { saveLayoutEnabled = value; } }
		public bool LoadLayoutEnabled { get { return loadLayoutEnabled; } set { loadLayoutEnabled = value; } }
		protected ChartDesignerActionList(ChartDesigner chartDesigner, IComponent component) : base(component) {
			this.chartDesigner = chartDesigner;
		}
		protected void OnAnnotationsAction() {
			chartDesigner.OnAnnotations();
		}
		protected void OnSeriesAction() {
			chartDesigner.OnSeries();
		}		
		protected void OnResetLegendPointOptionsAction() {
			chartDesigner.OnResetLegendPointOptions();
		}
		protected void OnEditPalettesAction() {
			chartDesigner.OnEditPalettes();
		}
		protected void OnDesignerAction() {
			chartDesigner.OnDesigner();
		}
		protected void OnSaveLayoutAction() {
			chartDesigner.OnSaveLayout();
		}
		protected void OnLoadLayoutAction() {
			chartDesigner.OnLoadLayout();
		}
		protected void AddAnnotationsAction(DesignerActionItemCollection actionItems, string category) {
			if (AnnotationsActionEnabled)
				AddItemAction(actionItems, "OnAnnotationsAction", ChartStringId.VerbAnnotations, ChartStringId.VerbAnnotationsDescription, category);
		}
		protected void AddSeriesAction(DesignerActionItemCollection actionItems, string category) {
			if (SeriesActionEnabled)
				AddItemAction(actionItems, "OnSeriesAction", ChartStringId.VerbSeries, ChartStringId.VerbSeriesDescription, category);			
		}
		protected void AddEditPalettesAction(DesignerActionItemCollection actionItems, string category) {
			if (EditPalettesEnabled)
				AddItemAction(actionItems, "OnEditPalettesAction", ChartStringId.VerbEditPalettes, ChartStringId.VerbEditPalettesDescription, category);			
		}
		protected void AddWizardAction(DesignerActionItemCollection actionItems, string category) {
			if (WizardEnabled)
				AddItemAction(actionItems, "OnDesignerAction", ChartStringId.VerbDesigner, ChartStringId.VerbDesignerDescription, category);
		}
		protected void AddSaveLayoutAction(DesignerActionItemCollection actionItems, string category) {
			if (SaveLayoutEnabled)
				AddItemAction(actionItems, "OnSaveLayoutAction", ChartStringId.VerbSaveLayout, ChartStringId.VerbSaveLayoutDescription, category);
		}
		protected void AddLoadLayoutAction(DesignerActionItemCollection actionItems, string category) {
			if (LoadLayoutEnabled)
				AddItemAction(actionItems, "OnLoadLayoutAction", ChartStringId.VerbLoadLayout, ChartStringId.VerbLoadLayoutDescription, category);
		}
		void AddItemAction(DesignerActionItemCollection actionItems, string methodName, ChartStringId itemName, ChartStringId itemDescription, string category) {
			DesignerActionItem actionItem = new DesignerActionMethodItem(this, methodName, ChartLocalizer.GetString(itemName),
					category, ChartLocalizer.GetString(itemDescription), true);
			actionItem.AllowAssociate = true;
			actionItems.Add(actionItem);
		}
	}
	public abstract class DeleteAction {
		readonly IChartContainer control;
		readonly IDesignerHost designerHost;
		readonly IComponentChangeService changeService;
		public DeleteAction(IChartContainer control, IDesignerHost designerHost, IComponentChangeService changeService) {
			this.control = control;
			this.designerHost = designerHost;
			this.changeService = changeService;
		}
		protected abstract void PerformActionInternal();
		public bool PerformAction(string transactionName) {
			DesignerTransaction transaction = null;
			if (designerHost != null && changeService != null)
				transaction = designerHost.CreateTransaction(transactionName);
			try {
				if (transaction != null)
					changeService.OnComponentChanging(control, null);
				PerformActionInternal();
			}
			catch {
				if (transaction != null) {
					transaction.Cancel();
					transaction = null;
				}
				return false;
			}
			if (changeService != null)
				changeService.OnComponentChanged(control, null, null, null);
			if (transaction != null)
				transaction.Commit();
			return true;
		}
	}
	public class CollectionItemDeleteAction : DeleteAction {
		ChartElement element;
		ChartCollectionBase collection;
		public CollectionItemDeleteAction(ChartElement element, ChartCollectionBase collection, IChartContainer control, IDesignerHost designerHost, IComponentChangeService changeService)
			: base(control, designerHost, changeService) {
			this.element = element;
			this.collection = collection;
		}
		protected override void PerformActionInternal() {
			collection.Remove(element);
		}
	}
}
