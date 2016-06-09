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
using System.ComponentModel.Design;
using System.Collections;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Design;
using System.Windows.Forms;
namespace DevExpress.XtraPrinting.Design {
	public class PrintingSystemDesigner : BaseComponentDesignerSimple {
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
		PrintingSystem PrintingSystem { get { return (PrintingSystem)Component; } }
		public PrintingSystemDesigner()
			: base() {
		}
#if DXWhidbey
		public override void InitializeNewComponent(IDictionary dictionary) {
			base.InitializeNewComponent(dictionary);
#else
		public override void OnSetComponentDefaults() {
			base.OnSetComponentDefaults();
#endif
			IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			foreach(IComponent component in designerHost.Container.Components) {
				PrintControl printControl = component as PrintControl;
				if(printControl != null && printControl.PrintingSystem == null)
					printControl.PrintingSystem = PrintingSystem;
			}
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			properties["Watermark"] = Accessor.CreateProperty(typeof(PrintingSystem),
				(PropertyDescriptor)properties["Watermark"],
				new Attribute[] { new System.ComponentModel.BrowsableAttribute(true), new EditorAttribute(typeof(WatermarkEditor), typeof(System.Drawing.Design.UITypeEditor)) });
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			base.RegisterActionLists(list);
			list.Insert(0, new PrintingSystemActionList(PrintingSystem));
		}
		protected override DXAboutActionList GetAboutAction() { return new DXAboutActionList(Component, new MethodInvoker(PrintingSystem.About)); }
	}
	public class WatermarkEditor : System.Drawing.Design.UITypeEditor {
		static void CopyWatermark(PrintingSystem printingSystem, DevExpress.XtraPrinting.Drawing.Watermark source, IServiceProvider serviceProvider) {
			IDesignerHost designerHost = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
			IComponentChangeService componentChangeService = (IComponentChangeService)serviceProvider.GetService(typeof(IComponentChangeService));
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(printingSystem)["Watermark"];
			if(designerHost == null || componentChangeService == null || propertyDescriptor == null)
				return;
			DesignerTransaction transaction = designerHost.CreateTransaction("Change Watermark");
			try {
				componentChangeService.OnComponentChanging(printingSystem, TypeDescriptor.GetProperties(printingSystem)["Watermark"]);
				printingSystem.Watermark.CopyFrom(source);
				componentChangeService.OnComponentChanged(printingSystem, propertyDescriptor, null, null);
				transaction.Commit();
			} catch {
				transaction.Cancel();
			}
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			PrintingSystem printingSystem = context.Instance as PrintingSystem;
			System.Windows.Forms.Design.IWindowsFormsEditorService edSvc = provider.GetService(typeof(System.Windows.Forms.Design.IWindowsFormsEditorService)) as System.Windows.Forms.Design.IWindowsFormsEditorService;
			if(printingSystem != null && edSvc != null) {
				DevExpress.XtraPrinting.Native.WinControls.WatermarkEditorForm form = new DevExpress.XtraPrinting.Native.WinControls.WatermarkEditorForm();
				DevExpress.LookAndFeel.Helpers.ControlUserLookAndFeel userLookAndFeel = new DevExpress.LookAndFeel.Helpers.ControlUserLookAndFeel(form);
				userLookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
				userLookAndFeel.UseWindowsXPTheme = false;
				userLookAndFeel.UseDefaultLookAndFeel = false;
				form.LookAndFeel.ParentLookAndFeel = userLookAndFeel;
				form.Assign(printingSystem.Watermark);
				if(edSvc.ShowDialog(form) == System.Windows.Forms.DialogResult.OK && !printingSystem.Watermark.Equals(form.Watermark)) {
					CopyWatermark(printingSystem, form.Watermark, provider);
				}
				form.Dispose();
				userLookAndFeel.Dispose();
			}
			return value;
		}
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return System.Drawing.Design.UITypeEditorEditStyle.Modal;
		}
	}
	class PrintingSystemActionList : DesignerActionList {
		PrintingSystem PrintingSystem { get { return (PrintingSystem)Component; } }
		public PrintingSystemActionList(PrintingSystem ps)
			: base(ps) {
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = new DesignerActionItemCollection();
			result.Add(new DesignerActionPropertyItem("Links", "Links", NativeSR.CatPrinting));
			return result;
		}
		[
		Category(NativeSR.CatPrinting),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraPrinting.Design.LinkItemsEditor," + AssemblyInfo.SRAssemblyPrintingDesign, typeof(System.Drawing.Design.UITypeEditor))
		]
		public LinkCollection Links {
			get {
				return PrintingSystem.Links;
			}
		}
	}
}
