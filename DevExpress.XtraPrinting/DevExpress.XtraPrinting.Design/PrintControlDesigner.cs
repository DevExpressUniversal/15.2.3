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
using System.Windows.Forms;
using DevExpress.Design.TypePickEditor;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraPrinting.Preview;
namespace DevExpress.XtraPrinting.Design {
	using System.Reflection;
	class ResourcesPrintingDesign {
		public static Assembly Assembly { get { return typeof(ResourcesPrintingDesign).Assembly; } }
		public static string GetFullName(string name) {
			return string.Concat(typeof(ResourcesPrintingDesign).Namespace, ".", name);
		}
	}
}
namespace DevExpress.XtraPrinting.Design {
	using System.Collections.Generic;
	using System.Drawing;
	using System.Globalization;
	using System.Runtime.InteropServices;
	using DevExpress.ReportServer.Printing;
	using DevExpress.Utils;
	using DevExpress.Utils.Controls;
	using DevExpress.Utils.Design;
	using DevExpress.XtraPrinting.Native;
	using DevExpress.XtraTreeList.Nodes;
	public class PrintControlDesigner : DevExpress.Utils.Design.BaseControlDesignerSimple {
		public static void InitializeLookAndFeelService(IDesignerHost host, ILookAndFeelService lookAndFeelService) {
			if(lookAndFeelService == null) {
				var serv = new VSLookAndFeelService(host);
				host.AddService(typeof(ILookAndFeelService), serv);
			}
		}
		DesignerVerbCollection verbs;
		public override DesignerVerbCollection Verbs {
			get {
				if(verbs == null) {
					verbs = new DesignerVerbCollection();
					verbs.Add(new DesignerVerb("About", OnAbout));
					DXSmartTagsHelper.CreateDefaultVerbs(this, verbs);
				}
				return verbs;
			}
		}
		void OnAbout(object sender, EventArgs e) {
			PrintingSystem.About();
		}
		DocumentViewer DocumentViewer { get { return (DocumentViewer)Component; } }
#if DXWhidbey
		public override void InitializeNewComponent(IDictionary dictionary) {
			base.InitializeNewComponent(dictionary);
			DocumentViewer.Dock = DockStyle.Fill;
#else
		public override void OnSetComponentDefaults() {
			base.OnSetComponentDefaults();
#endif
			IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			foreach(IComponent component in designerHost.Container.Components) {
				IPrintPreviewControl printPreviewControl = component as IPrintPreviewControl;
				if(printPreviewControl != null && printPreviewControl.PrintControl == null)
					printPreviewControl.PrintControl = DocumentViewer;
				PrintingSystem printingSystem = component as PrintingSystem;
				if(printingSystem != null && DocumentViewer.DocumentSource == null)
					DocumentViewer.DocumentSource = printingSystem;
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			ILookAndFeelService serv = GetService(typeof(ILookAndFeelService)) as ILookAndFeelService;
			InitializeLookAndFeelService(GetService(typeof(IDesignerHost)) as IDesignerHost, serv);
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			base.RegisterActionLists(list);
			list.Insert(0, new PrintControlActionList(DocumentViewer));
		}
		protected override DXAboutActionList GetAboutAction() { return new DXAboutActionList(Component, new MethodInvoker(PrintingSystem.About)); }
	}
	public static class ActionListSR {
		public const string
			CreateRibbonToolbar = "Create Ribbon Toolbar",
			CreateStandardToolbar = "Create Standard Toolbar",
			DocumentSource = "Document Source",
			ShowPageMargins = "Show Page Margins",
			Dock = "Dock in Parent Container",
			Undock = "Undock in Parent Container";
	}
	public class PrintControlActionList : DesignerActionList {
		DocumentViewer Control { get { return (DocumentViewer)Component; } }
		public PrintControlActionList(DocumentViewer control)
			: base(control) {
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = new DesignerActionItemCollection();
			IDesignerHost host = Component.Site.GetService<IDesignerHost>();
			if(host.FirstComponent<PrintRibbonController>() == null && host.FirstComponent<PrintBarManager>() == null) {
				result.Add(new DesignerActionMethodItem(this, "CreateRibbonView", ActionListSR.CreateRibbonToolbar, NativeSR.CatPrinting, false));
				result.Add(new DesignerActionMethodItem(this, "CreateBarView", ActionListSR.CreateStandardToolbar, NativeSR.CatPrinting, false));
			}
			if(Control.Dock == DockStyle.Fill)
				result.Add(new DesignerActionMethodItem(this, "UndockInParentContainer", ActionListSR.Undock, NativeSR.CatPrinting));
			else
				result.Add(new DesignerActionMethodItem(this, "DockInParentContainer", ActionListSR.Dock, NativeSR.CatPrinting));
			result.Add(new DesignerActionPropertyItem("DocumentSource", ActionListSR.DocumentSource, NativeSR.CatPrinting));
			result.Add(new DesignerActionPropertyItem("ShowPageMargins", ActionListSR.ShowPageMargins, NativeSR.CatPrinting));
			return result;
		}
		void DockInParentContainer() {
			Control.Dock = DockStyle.Fill;
			Refresh();
		}
		void UndockInParentContainer() {
			Control.Dock = DockStyle.None;
			Refresh();
		}
		void CreateRibbonView() {
			AddComponent(typeof(DocumentViewerRibbonController));
			ISelectionService serv = Component.Site.GetService<ISelectionService>();
			IMenuCommandService commandServ = Component.Site.GetService<IMenuCommandService>();
			if(serv != null && commandServ != null) {
				serv.SetSelectedComponents(new object[] { Control }, SelectionTypes.Primary);
				commandServ.GlobalInvoke(StandardCommands.BringToFront);
			}
		}
		void AddComponent(Type componentType) {
			IDesignerHost host = Component.Site.GetService<IDesignerHost>();
			host.AddToContainer(componentType);
			Refresh();
		}
		void Refresh() {
			DesignerActionUIService serv = Component.Site.GetService<DesignerActionUIService>();
			if(serv != null)
				serv.Refresh(Control);
		}
		void CreateBarView() {
			AddComponent(typeof(DocumentViewerBarManager));
		}
		[
		Category(NativeSR.CatPrinting),
		TypeConverter("DevExpress.XtraPrinting.Design.DocumentSourceConvertor," + AssemblyInfo.SRAssemblyPrintingDesign),
		Editor("DevExpress.XtraPrinting.Design.DocumentSourceEditor," + AssemblyInfo.SRAssemblyPrintingDesign, typeof(System.Drawing.Design.UITypeEditor))
		]
		public object DocumentSource {
			get { return Control.DocumentSource; }
			set {
				if(DocumentSource != value) {
					Control.DocumentSource = value;
					FireChanged();
				}
			}
		}
		[
		Category(NativeSR.CatPrinting),
		DefaultValue(true)
		]
		public bool ShowPageMargins {
			get { return Control.ShowPageMargins; }
			set {
				if(ShowPageMargins != value) {
					Control.ShowPageMargins = value;
					FireChanged();
				}
			}
		}
		void FireChanged() { 
			EditorContextHelper.FireChanged(Component.Site, Control); 
		}
	}
	static class TypePickerSR {
		public const string
			ProjectReports = "Project Reports",
			StandardSources = "Standard Sources";
	}
	public class DocumentSourceEditor : TreeViewTypePickEditor {
		protected override TypePickerTreeView CreateTypePicker(IServiceProvider provider) {
			TypePickerTreeView picker = new DocumentSourcePickerVS(typeof(IDocumentSource), new DocumentSourceAttribute(true), "true");
			return picker;
		}
		protected override TypePickerPanel CreatePickerPanel(TypePickerTreeView picker, IServiceProvider provider) {
			return new TypePickerPanel(picker);
		}
		protected override object GetEditValue(PickerNode selectedPickerNode, ITypeDescriptorContext context, IServiceProvider provider) {
			if(selectedPickerNode is TypePickerNode && Equals(selectedPickerNode.ParentNode.GetText(), TypePickerSR.ProjectReports))
				return ((TypePickerNode)selectedPickerNode).Type;
			return base.GetEditValue(selectedPickerNode, context, provider);
		}
	}
	class DocumentSourcePickerVS : TypePickerTreeViewVS {
		Dictionary<Type, int> imageIndices = new Dictionary<Type, int>();
		public DocumentSourcePickerVS(Type type, Attribute attr, string attrValue)
			: base(type, attr, attrValue) {
		}
		protected override void OnGetStateImage(object sender, XtraTreeList.GetStateImageEventArgs e) {
			if(e.Node is NonePickerNode)
				e.NodeImageIndex = 0;
			else if(e.Node is ProjectObjectsPickerNode)
				e.NodeImageIndex = e.Node.Expanded ? 2 : 1;
			else if(e.Node is InstancePickerNode) {
				 var instance = ((InstancePickerNode)e.Node).GetInstance(null);
				 int index = GetImageIndex(instance.GetType());
				 if(index >= 0)
					 e.NodeImageIndex = index;
			} else if(e.Node is TypePickerNode) {
				int index = GetImageIndex(((TypePickerNode)e.Node).Type);
				if(index >= 0)
					e.NodeImageIndex = index;
			}
		}
		int GetImageIndex(Type type) {
			int index = -1;
			if(!imageIndices.TryGetValue(type, out index)) {
				Image image = GetImage(type);
				if(image != null) {
					ImageCollection collection = StateImageList as ImageCollection;
					collection.AddImage(image);
					index = collection.Images.Count - 1;
					imageIndices.Add(type, index);
				}
			}
			return index;
		}
		static Image GetImage(Type type) {
			try {
				ToolboxBitmapAttribute attribute = TypeDescriptor.GetAttributes(type)[typeof(ToolboxBitmapAttribute)] as ToolboxBitmapAttribute;
				return attribute.GetImage(type);
			} catch {
				return null;
			}
		}
		protected override object CreateImageList() {
			return ImageHelper.CreateImageCollectionFromResources(ResourcesPrintingDesign.GetFullName("Images.DocumentSourceImages.bmp"), ResourcesPrintingDesign.Assembly, new Size(16, 16), Color.Magenta);
		}
		protected override void FillNodeInternal(IDesignerHost designerHost) {
			var node = new ProjectObjectsPickerNode(TypePickerSR.ProjectReports, Nodes);
			((IList)Nodes).Add(node);
			FillNodes(node.Nodes, designerHost);
			node = new ProjectObjectsPickerNode(TypePickerSR.StandardSources, Nodes);
			((IList)Nodes).Add(node);
			Type[] types = new Type[] { typeof(PrintingSystem), typeof(RemoteDocumentSource), typeof(System.Drawing.Printing.PrintDocument) };
			foreach(Type type in types) {
				PickerNode typeNode = new TypePickerNode(type.Name, type, node.Nodes);
				node.Nodes.Add(typeNode);
			}
		}
	}
	public class DocumentSourceConvertor : TypeConverter {
		const string none = "(none)";
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if(!(destinationType == typeof(string))) {
				return base.ConvertTo(context, culture, value, destinationType);
			}
			if(value == null)
				return none;
			if(value is Type)
				return ((Type)value).Name;
			if(context != null) {
				IReferenceService service = (IReferenceService)context.GetService(typeof(IReferenceService));
				if(service != null) {
					string name = service.GetName(value);
					if(name != null) {
						return name;
					}
				}
			}
			if(!Marshal.IsComObject(value) && (value is IComponent)) {
				IComponent component = (IComponent)value;
				ISite site = component.Site;
				if(site != null && site.Name != null)
					return site.Name;
			}
			return string.Empty;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return false;
		}
	}
}
namespace DevExpress.XtraTreeList.Nodes {
	using DevExpress.XtraTreeList.Native;
	static class TreeListNodeExtentions {
		public static string GetText(this TreeListNode node) {
			return node is XtraListNode ? ((XtraListNode)node).Text : string.Empty;
		}
	}
}
