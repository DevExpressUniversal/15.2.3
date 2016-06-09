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

using System.Drawing.Printing;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using DevExpress.Utils;
namespace DevExpress.Xpf.Printing {
	class PreviewModelWrapper : IPreviewModelWrapper {
		readonly LinkPreviewModel previewModel;
		public PreviewModelWrapper(LinkPreviewModel previewModel) {
			Guard.ArgumentNotNull(previewModel, "previewModel");
			this.previewModel = previewModel;
		}
		public object PreviewModel {
			get { return previewModel; }
		}
		public void Dispose() {
			LinkBase link = previewModel.Link;
			previewModel.Dispose();
			link.Dispose();
		}
	}
	public class PrintableControlPreviewService : ServiceBase, IPrintableControlPreviewService {
		#region Properties
		public bool Landscape {
			get { return (bool)GetValue(LandscapeProperty); }
			set { SetValue(LandscapeProperty, value); }
		}
		public static readonly DependencyProperty LandscapeProperty =
			DependencyProperty.Register("Landscape", typeof(bool), typeof(PrintableControlPreviewService), new PropertyMetadata(false));
		public DataTemplate PageHeaderTemplate {
			get { return (DataTemplate)GetValue(PageHeaderTemplateProperty); }
			set { SetValue(PageHeaderTemplateProperty, value); }
		}
		public static readonly DependencyProperty PageHeaderTemplateProperty =
			DependencyProperty.Register("PageHeaderTemplate", typeof(DataTemplate), typeof(PrintableControlPreviewService), new PropertyMetadata(null));
		public object PageHeaderData {
			get { return (object)GetValue(PageHeaderDataProperty); }
			set { SetValue(PageHeaderDataProperty, value); }
		}
		public static readonly DependencyProperty PageHeaderDataProperty =
			DependencyProperty.Register("PageHeaderData", typeof(object), typeof(PrintableControlPreviewService), new PropertyMetadata(null));
		public DataTemplate PageFooterTemplate {
			get { return (DataTemplate)GetValue(PageFooterTemplateProperty); }
			set { SetValue(PageFooterTemplateProperty, value); }
		}
		public static readonly DependencyProperty PageFooterTemplateProperty =
			DependencyProperty.Register("PageFooterTemplate", typeof(DataTemplate), typeof(PrintableControlPreviewService), new PropertyMetadata(null));
		public object PageFooterData {
			get { return (object)GetValue(PageFooterDataProperty); }
			set { SetValue(PageFooterDataProperty, value); }
		}
		public static readonly DependencyProperty PageFooterDataProperty =
			DependencyProperty.Register("PageFooterData", typeof(object), typeof(PrintableControlPreviewService), new PropertyMetadata(null));
		public PaperKind PaperKind {
			get { return (PaperKind)GetValue(PaperKindProperty); }
			set { SetValue(PaperKindProperty, value); }
		}
		public static readonly DependencyProperty PaperKindProperty =
			DependencyProperty.Register("PaperKind", typeof(PaperKind), typeof(PrintableControlPreviewService), new PropertyMetadata(DevExpress.XtraPrinting.XtraPageSettingsBase.DefaultPaperKind));
		#endregion
		public IPreviewModelWrapper GetPreview() {
			PrintableControlLink link = new PrintableControlLink(AssociatedObject as IPrintableControl);
			link.Landscape = Landscape;
			link.PageHeaderTemplate = PageHeaderTemplate;
			link.PageHeaderData = PageHeaderData;
			link.PageFooterTemplate = PageFooterTemplate;
			link.PageFooterData = PageFooterData;
			link.PaperKind = PaperKind;
			LinkPreviewModel previewModel = new LinkPreviewModel(link);
			ConfigurePreviewModel(previewModel);
			PreviewModelWrapper wrapper = new PreviewModelWrapper(previewModel);
			link.CreateDocument(true);
			return wrapper;
		}
		protected virtual void ConfigurePreviewModel(LinkPreviewModel model) {
		}
	}
}
