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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Native;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.Utils.UI {
	using System.Reflection;
	class ResourcesUtilsUI {
		public static Assembly Assembly { get { return typeof(ResourcesUtilsUI).Assembly; } }
		public static string GetFullName(string name) {
			return string.Concat(typeof(ResourcesUtilsUI).Namespace, ".", name);
		}
	}
}
namespace System.ComponentModel.Design {
	public static class IDesignerHostExtentions {
		public static void AddToContainer(this IDesignerHost host, Type componentType) {
			IToolboxUser serv = (IToolboxUser)host.GetDesigner(host.RootComponent);
			serv.ToolPicked(new ToolboxItem(componentType));
		}
		public static void AddToContainer(this IDesignerHost host, IComponent c, bool useDefaultSettings) {
			try {
				host.Container.Add(c);
			} catch { }
			if(useDefaultSettings) {
				IComponentInitializer designer = host.GetDesigner(c) as IComponentInitializer;
				if(designer != null)
					designer.InitializeNewComponent(new SortedList());
			}
		}
		public static IEnumerable<T> AllComponents<T>(this IDesignerHost host) where T : Component {
			foreach(IComponent c in host.Container.Components) {
				if(c is T)
					yield return (T)c;
			}
		}
		public static T FirstComponent<T>(this IDesignerHost host) where T : Component {
			foreach(IComponent c in host.Container.Components) {
				if(c is T)
					return (T)c;
			}
			return null;
		}
	}
}
namespace DevExpress.Design.TypePickEditor {
	using DevExpress.XtraReports.Design;
	using DevExpress.XtraReports;
	using DevExpress.Utils.UI;
	public abstract class TreeViewTypePickEditor : UITypeEditor, IDisposable {
		TypePickerTreeView picker;
		TypePickerPanel pickerPanel;
		bool disposed;
		public override bool IsDropDownResizable {
			get { return true; }
		}
		protected TreeViewTypePickEditor() {
		}
		#region Disposing
		~TreeViewTypePickEditor() {
			Dispose(false);
		}
		void IDisposable.Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if(!disposed) {
				if(disposing) {
					if(pickerPanel != null) {
						pickerPanel.Dispose();
						pickerPanel = null;
					}
					if(picker != null) {
						picker.Dispose();
						picker = null;
					}
				}
				disposed = true;
			}
		}
		#endregion
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(picker == null || picker.IsDisposed)
				picker = CreateTypePicker(provider);
			if(pickerPanel == null || pickerPanel.IsDisposed)
				pickerPanel = CreatePickerPanel(picker, provider);
			try {
				pickerPanel.Initialize(context, provider, value);
				IWindowsFormsEditorService editServ = provider.GetService<IWindowsFormsEditorService>();
				if(editServ != null) {
					editServ.DropDownControl(pickerPanel);
				}
				PickerNode node = picker.SelectedPickerNode;
				if(node != null)
					return GetEditValue(node, context, provider);
			} catch {
			}
			return value;
		}
		protected virtual object GetEditValue(PickerNode selectedPickerNode, ITypeDescriptorContext context, IServiceProvider provider) {
			IDesignerHost host = provider.GetService<IDesignerHost>();
			IComponent component = selectedPickerNode.GetInstance(provider);
			if(component != null && component.Site == null)
				host.AddToContainer(component, true);
			return component;
		}
		protected abstract TypePickerTreeView CreateTypePicker(IServiceProvider provider);
		protected abstract TypePickerPanel CreatePickerPanel(TypePickerTreeView picker, IServiceProvider provider);
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null) {
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}
	}
	public abstract class PickerNode : XtraListNode {
		protected PickerNode(string text, TreeListNodes owner)
			: base(text, owner) {
		}
		public abstract IComponent GetInstance(IServiceProvider provider);
	}
	public class ProjectObjectsPickerNode : XtraListNode {
		public ProjectObjectsPickerNode(string text, TreeListNodes owner)
			: base(text, owner) {
		}
	}
	public class NonePickerNode : PickerNode {
		public NonePickerNode(string text, TreeListNodes owner)
			: base(text, owner) {
		}
		public override IComponent GetInstance(IServiceProvider provider) {
			return null;
		}
	}
	public class TypePickerNode : PickerNode {
		readonly Type type;
		public Type Type {
			get { return type; }
		}
		public TypePickerNode(string text, Type type, TreeListNodes owner)
			: base(text, owner) {
			this.type = type;
		}
		public override IComponent GetInstance(IServiceProvider provider) {
			return Activator.CreateInstance(type) as IComponent;
		}
	}
	public class InstancePickerNode : PickerNode {
		readonly IComponent instance;
		public InstancePickerNode(string text, IComponent instance, TreeListNodes owner)
			: base(text, owner) {
			this.instance = instance;
		}
		public override IComponent GetInstance(IServiceProvider provider) {
			return instance;
		}
	}
	public class PickerTreeView : XtraTreeView {
		IWindowsFormsEditorService editorService;
		PickerNode selectedPickerNode;
		public PickerNode SelectedPickerNode {
			get { return selectedPickerNode; }
		}
		public PickerTreeView() {
		}
		public virtual void Start(IServiceProvider provider) {
			editorService = provider.GetService<IWindowsFormsEditorService>();
			selectedPickerNode = null;
			DesignLookAndFeelHelper.SetParentLookAndFeel(this, provider);
		}
		protected override bool IsInputKey(Keys key) {
			if(key == Keys.Enter) {
				return true;
			}
			return base.IsInputKey(key);
		}
		protected override void OnClick(EventArgs e) {
			base.OnClick(e);
			CloseDropDown(FindNodeAt(PointToClient(MousePosition)) as PickerNode);
		}
		protected override void OnDoubleClick(EventArgs e) {
			base.OnDoubleClick(e);
			CloseDropDown(FindNodeAt(PointToClient(MousePosition)) as PickerNode);
		}
		public virtual void CloseDropDown(PickerNode node) {
			if(editorService != null) {
				if(node != null)
					selectedPickerNode = node;
				editorService.CloseDropDown();
			}
		}
		TreeListNode FindNodeAt(Point point) {
			TreeListHitInfo hitInfo = CalcHitInfo(point);
			if(hitInfo.HitInfoType == HitInfoType.Cell)
				return hitInfo.Node;
			return null;
		}
	}
	public class TypePickerTreeView : PickerTreeView {
		protected Type type;
		string noneNodeText;
		public TypePickerTreeView(Type type, string noneNodeText) {
			this.type = type;
			this.noneNodeText = noneNodeText;
			BorderStyle = BorderStyles.NoBorder;
			StateImageList = CreateImageList();
			GetStateImage += OnGetStateImage;
		}
		protected virtual void OnGetStateImage(object sender, GetStateImageEventArgs e) {
			if(e.Node is NonePickerNode)
				e.NodeImageIndex = 0;
			else if(e.Node is ProjectObjectsPickerNode)
				e.NodeImageIndex = 2;
			else if(e.Node is InstancePickerNode)
				e.NodeImageIndex = 4;
			else if(e.Node is TypePickerNode)
				e.NodeImageIndex = 7;
		}
		protected virtual object CreateImageList() {
			return ImageHelper.CreateImageCollectionFromResources(ResourcesUtilsUI.GetFullName("Images.TypePickerImages.bmp"), ResourcesUtilsUI.Assembly, new Size(16, 16), Color.Magenta);
		}
		public virtual void Start(ITypeDescriptorContext context, IServiceProvider provider, object selectedValue) {
			Nodes.Clear();
			PickerNode selectedNode = new NonePickerNode(noneNodeText, Nodes);
			((IList)Nodes).Add(selectedNode);
			TypeConverter converter = context.PropertyDescriptor.Converter;
			if(converter != null && converter.GetStandardValuesSupported(context)) {
				IEnumerable<IComponent> filteredComponentCollection = FilterSelectedComponents(converter.GetStandardValues(context), provider, context);
				foreach(IComponent component in filteredComponentCollection) {
					PickerNode node = AddNode(component);
					if(component == selectedValue)
						selectedNode = node;
				}
			} else {
				IDesignerHost designerHost = provider.GetService<IDesignerHost>();
				IEnumerable<IComponent> filteredComponentCollection = FilterSelectedComponents(designerHost.Container.Components, provider, context);
				foreach(IComponent component in filteredComponentCollection)
					if(type.IsAssignableFrom(component.GetType()) || (component is System.Drawing.Printing.PrintDocument)) {
						PickerNode node = AddNode(component);
						if(component == selectedValue)
							selectedNode = node;
					}
			}
			SelectNode(selectedNode);
			base.Start(provider);
		}
		public override void CloseDropDown(PickerNode node) {
			if(node != null) {
				base.CloseDropDown(node);
			}
		}
		protected PickerNode CreatePickerNode(IComponent component, TreeListNodes owner) {
			if(component != null) {
				string name = GetDisplayName(component);
				if(name != null)
					return new InstancePickerNode(name, component, owner);
			}
			return null;
		}
		protected virtual string GetDisplayName(IComponent comp) {
			if(comp is IDisplayNameProvider)
				return ((IDisplayNameProvider)comp).GetDataSourceDisplayName();
			if(comp != null && comp.Site != null)
				return comp.Site.Name;
			return null;
		}
		PickerNode AddNode(IComponent component) {
			PickerNode pickerNode = CreatePickerNode(component, Nodes);
			if(pickerNode != null)
				((IList)Nodes).Add(pickerNode);
			return pickerNode;
		}
		protected virtual IEnumerable<IComponent> FilterSelectedComponents(IEnumerable collection, IServiceProvider serviceProvider, ITypeDescriptorContext context) {
			IEnumerable<IComponent> components = collection.OfType<IComponent>();
			ISelectionService selectionService = serviceProvider.GetService<ISelectionService>();
			if(selectionService != null) {
				var selectedComponents = selectionService.GetSelectedComponents().OfType<IComponent>();
				components.Except(selectedComponents);
			}
			return components;
		}
	}
	public class BindingSourceTypePickerNode : TypePickerNode {
		public BindingSourceTypePickerNode(string text, Type type, TreeListNodes owner)
			: base(text, type, owner) {
		}
		public override IComponent GetInstance(IServiceProvider provider) {
			if(typeof(System.Data.DataSet).IsAssignableFrom(Type))
				return base.GetInstance(provider);
			BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = Type;
			return bindingSource;
		}
	}
	public abstract class DataSourcePickerPanelBase : TypePickerPanel {
		LinkLabelControl hyperLink;
		ControlUserLookAndFeel userLookAndFeel;
		public DataSourcePickerPanelBase(TypePickerTreeView treeView, string hyperLinkText) : base(treeView) {
			Panel bottomPanel = CreateBottomPanel();
			hyperLink.Text = hyperLinkText;
			userLookAndFeel = new ControlUserLookAndFeel(this);
			PopupSeparatorLine separatorLine = new PopupSeparatorLine(userLookAndFeel) { Dock = DockStyle.Bottom };
			Height += (bottomPanel.Height + separatorLine.Height);
			Controls.AddRange(new Control[] { separatorLine, bottomPanel });
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(userLookAndFeel != null) {
					userLookAndFeel.Dispose();
					userLookAndFeel = null;
				}
				if(hyperLink != null) {
					hyperLink.LinkClicked -= OnHyperLinkClick;
					hyperLink.Dispose();
					hyperLink = null;
				}
			}
			base.Dispose(disposing);
		}
		Panel CreateBottomPanel() {
			hyperLink = CreateHyperLink();
			hyperLink.LinkClicked += OnHyperLinkClick;
			Panel panel = new Panel() {
				Height = Math.Max(20, hyperLink.GetPreferredSize(Size.Empty).Height),
				Dock = DockStyle.Bottom,
				BorderStyle = BorderStyle.None
			};
			panel.Controls.AddRange(new Control[] { hyperLink, CreatePictureBox() });
			return panel;
		}
		static LinkLabelControl CreateHyperLink() {
			LinkLabelControl linkLabel = new LinkLabelControl() {
				Bounds = new Rectangle(new Point(20, 6), new Size(200, 15)),
				AutoSize = true,
				BackColor = Color.Transparent,
				LinkBehavior = LinkBehavior.HoverUnderline
			};
			return linkLabel;
		}
		static PictureBox CreatePictureBox() {
			Bitmap bitmap = new Bitmap(typeof(ResFinder), "Images.AddNewDataSource.bmp");
			bitmap.MakeTransparent(Color.Magenta);
			return new PictureBox() {
				Image = bitmap,
				BackColor = Color.Transparent,
				Width = bitmap.Width,
				Height = bitmap.Height,
				Location = new Point(4, 4),
				AccessibleRole = AccessibleRole.Graphic
			};
		}
		public override void Initialize(ITypeDescriptorContext context, IServiceProvider provider, object currentValue) {
			base.Initialize(context, provider, currentValue);
			ILookAndFeelService lookAndFeelService = provider.GetService<ILookAndFeelService>();
			if(lookAndFeelService != null) {
				userLookAndFeel.ParentLookAndFeel = lookAndFeelService.LookAndFeel;
				hyperLink.Initialize(lookAndFeelService.LookAndFeel);
			}
			hyperLink.Enabled = CanAddNewDataSource(context != null ? context.Instance : null);
		}
		protected abstract bool CanAddNewDataSource(object instance);
		protected abstract void OnHyperLinkClick(object sender, LinkLabelLinkClickedEventArgs e);
		protected void CloseDropDown() {
			if(Provider != null) {
				IWindowsFormsEditorService editorService = Provider.GetService<IWindowsFormsEditorService>();
				if(editorService != null)
					editorService.CloseDropDown();
			}
		}
	}
	[ToolboxItem(false)]
	public class TypePickerPanel : Control {
		protected TypePickerTreeView treeView;
		IServiceProvider provider;
		protected ITypeDescriptorContext context;
		public IServiceProvider Provider {
			get { return provider; }
		}
		public TypePickerPanel(TypePickerTreeView treeView) {
			this.treeView = treeView;
			Width = treeView.Width;
			Height = treeView.Height;
			treeView.Dock = DockStyle.Fill;
			Controls.Add(treeView);
		}
		public virtual void Initialize(ITypeDescriptorContext context, IServiceProvider provider, object currentValue) {
			this.context = context;
			this.provider = provider;
			treeView.Start(context, provider, currentValue);
		}
	}
}
