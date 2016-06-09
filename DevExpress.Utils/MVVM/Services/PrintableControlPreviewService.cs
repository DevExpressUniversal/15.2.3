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

namespace DevExpress.Utils.MVVM.Services {
	using System;
	using System.Drawing.Printing;
	using System.Reflection;
	using DevExpress.XtraPrinting;
	public class PrintableControlPreviewService {
		protected PrintableControlPreviewService(IPrintable printingProvider) {
			this.AssociatedObject = printingProvider;
		}
		public static PrintableControlPreviewService Create(IPrintable printingProvider) {
			IMVVMServiceTypesResolver typesResolver = MVVMTypesResolver.Instance as IMVVMServiceTypesResolver;
			return DynamicServiceSource.Create<PrintableControlPreviewService, IPrintable>(
				typesResolver.GetIPrintableControlPreviewServiceType(), printingProvider);
		}
		public object GetPreview() {
			PrintableComponentLinkWrapper link = new PrintableComponentLinkWrapper();
			link.Component = AssociatedObject;
			link.Landscape = Landscape;
			link.PageHeaderFooter = PageHeaderFooter;
			link.PaperKind = PaperKind;
			LinkPreviewModel previewModel = POCOViewModelSourceProxy.Instance.Create(typeof(LinkPreviewModel), link.InnerLink) as LinkPreviewModel;
			ConfigurePreviewModel(previewModel);
			link.CreateDocument(true);
			IMVVMTypesResolver typesResolver = MVVMTypesResolver.Instance as IMVVMTypesResolver;
			return DynamicServiceSource.Create<PreviewModelWrapper, LinkPreviewModel>(
				typesResolver.GetIPreviewModelWrapperType(), previewModel);
		}
		public IPrintable AssociatedObject { get; set; }
		public bool Landscape { get; set; }
		public object PageHeaderFooter { get; set; }
		public PaperKind PaperKind { get; set; }
		protected virtual void ConfigurePreviewModel(object model) { }
		class PrintableComponentLinkWrapper {
			public PrintableComponentLinkWrapper() {
				InitWrapper();
			}
			void InitWrapper() {
				var xtraPrintingAssembly = AssemblyHelper.LoadDXAssembly(AssemblyInfo.SRAssemblyPrinting);
				Type[] types = xtraPrintingAssembly.GetTypes();
				Type printableComponentLinkType = xtraPrintingAssembly.GetType("DevExpress.XtraPrinting.PrintableComponentLink");
				InnerLink = Activator.CreateInstance(printableComponentLinkType) as LinkBase;
				componentPropertyInfo = printableComponentLinkType.GetProperty("Component", BindingFlags.Instance | BindingFlags.Public);
			}
			PropertyInfo componentPropertyInfo;
			public IPrintable Component {
				get { return componentPropertyInfo.GetValue(InnerLink, null) as IPrintable; }
				set { componentPropertyInfo.SetValue(InnerLink, value, null); }
			}
			public bool Landscape {
				get { return InnerLink.Landscape; }
				set { InnerLink.Landscape = value; }
			}
			public object PageHeaderFooter {
				get { return this.InnerLink.PageHeaderFooter; }
				set { this.InnerLink.PageHeaderFooter = value; }
			}
			public PaperKind PaperKind {
				get { return this.InnerLink.PaperKind; }
				set { this.InnerLink.PaperKind = value; }
			}
			public LinkBase InnerLink { get; private set; }
			public virtual void CreateDocument(bool buildPagesInBackground) {
				InnerLink.CreateDocument(buildPagesInBackground);
			}
		}
	}
	public class PreviewModelWrapper : IPreviewModelWrapper<IPrintingSystem>, IDisposable {
		private readonly LinkPreviewModel previewModel;
		public PreviewModelWrapper(LinkPreviewModel previewModel) {
			this.previewModel = previewModel;
		}
		public object PreviewModel {
			get { return previewModel as LinkPreviewModel; }
		}
		public void Dispose() {
			LinkBase link = previewModel.Link;
			previewModel.Dispose();
			link.Dispose();
		}
		public LinkBase GetLink() {
			return previewModel.Link;
		}
		#region IPreviewModelWrapper<IPrintingSystem> Members
		public IPrintingSystem PrintingSystem {
			get { return previewModel; }
		}
		#endregion
	}
	public interface IPrintingSystem {
		object PrintingSystem { get; }
		void Print();
	}
	public interface IPreviewModelWrapper<T> {
		T PrintingSystem { get; }
	}
	public class LinkPreviewModel : IDisposable, IPrintingSystem {
		public LinkBase Link { get; set; }
		public LinkPreviewModel(LinkBase link) {
			Link = link;
			printingSystemCore = new PrintingSystemWrapper();
			printingSystemCore.Links.Clear();
			printingSystemCore.Links.Add(Link);
		}
		public void Print() {
			printingSystemCore.Print();
		}
		public void Dispose() {
			printingSystemCore.Dispose();
		}
		PrintingSystemWrapper printingSystemCore;
		public virtual object PrintingSystem {
			get { return printingSystemCore.InnerPrintingSystem; }
			set { printingSystemCore.InnerPrintingSystem = value; }
		}
		class PrintingSystemWrapper {
			public PrintingSystemWrapper() {
				InitWrapper();
			}
			void InitWrapper() {
				var xtraPrintingAssembly = AssemblyHelper.LoadDXAssembly(AssemblyInfo.SRAssemblyPrinting);
				Type printingSystemType = xtraPrintingAssembly.GetType("DevExpress.XtraPrinting.PrintingSystem", true, true);
				innerPrintingSystemCore = Activator.CreateInstance(printingSystemType) as PrintingSystemBase;
				printMethodInfo = printingSystemType.GetMethod("Print", new Type[] { });
				linksPropertyInfo = printingSystemType.GetProperty("Links", BindingFlags.Instance | BindingFlags.Public);
			}
			MethodInfo printMethodInfo;
			public void Print() {
				printMethodInfo.Invoke(innerPrintingSystemCore, null);
			}
			internal PrintingSystemBase innerPrintingSystemCore;
			public virtual object InnerPrintingSystem {
				get { return innerPrintingSystemCore; }
				set { innerPrintingSystemCore = value as PrintingSystemBase; }
			}
			PropertyInfo linksPropertyInfo;
			public LinkCollection Links {
				get { return linksPropertyInfo.GetValue(InnerPrintingSystem, null) as LinkCollection; }
			}
			public void Dispose() {
				Links.Clear();
				innerPrintingSystemCore.Dispose();
			}
		}
	}
}
