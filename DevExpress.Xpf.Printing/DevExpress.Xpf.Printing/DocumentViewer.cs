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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Printing.Parameters;
#if SL
using System;
#endif
namespace DevExpress.Xpf.Printing {
	[DXToolboxBrowsable(false)]
	public class DocumentViewer : Control {
		#region Fields and Properties
		IDialogService originalDialogService;
#if SILVERLIGHT
		public static readonly DependencyProperty AutoCreateDocumentProperty = DependencyProperty.Register("AutoCreateDocument", typeof(bool), typeof(DocumentViewer), new PropertyMetadata(false, OnAutoCreateDocumentChangedCallback));
#endif
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(IPreviewModel), typeof(DocumentViewer), new PropertyMetadata(null, ModelChangedCallback));
		public static readonly DependencyProperty DocumentViewerProperty = DependencyProperty.RegisterAttached("DocumentViewer", typeof(DocumentViewer), typeof(DocumentViewer), new PropertyMetadata(null));
#if SILVERLIGHT
		IDesignerPropertiesService designerPropertiesService;
		public bool AutoCreateDocument {
			get { return (bool)GetValue(AutoCreateDocumentProperty); }
			set { SetValue(AutoCreateDocumentProperty, value); }
		}
#endif
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("DocumentViewerModel")]
#endif
		public IPreviewModel Model {
			get { return (IPreviewModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
		public static DocumentViewer GetDocumentViewer(DependencyObject obj) {
			return (DocumentViewer)obj.GetValue(DocumentViewerProperty);
		}
		public static void SetDocumentViewer(DependencyObject obj, DocumentPreview value) {
			obj.SetValue(DocumentViewerProperty, value);
		}
		CursorContainer CursorContainer {
			get {
				return (CursorContainer)GetTemplateChild("previewCursorContainer");
			}
		}
		DockLayoutManager DockLayoutManager {
			get {
				return (DockLayoutManager)GetTemplateChild("documentPreviewDockLayoutManager");
			}
		}
		public ParametersPanel ParametersPanel {
			get {
				return GetTemplateChild("parametersPanel") as ParametersPanel;
			}
		}
		#endregion
		#region Constructors
#if SILVERLIGHT
		public DocumentViewer()
			: this(new DesignerPropertiesService()) {
			MouseLeftButtonUp += (o, e) => CollapsePinnedPanels();
		}
		internal DocumentViewer(IDesignerPropertiesService designerPropertiesService) {
			if(designerPropertiesService == null)
				throw new ArgumentNullException("designerPropertiesService");
			this.designerPropertiesService = designerPropertiesService;
			DefaultStyleKey = typeof(DocumentViewer);
		}
		static DocumentViewer() {
#if SL
			ThemeLoadingTypeManager.RegisterType("DevExpress.Xpf.Printing.DocumentViewer");
#endif
		}
#else
		static DocumentViewer() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentViewer), new FrameworkPropertyMetadata(typeof(DocumentViewer)));
		}
		public DocumentViewer() {
			FocusVisualStyle = null;
		}
#endif
		#endregion
		#region Methods
		static void ModelChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DocumentViewer)d).OnModelChanged((IPreviewModel)e.OldValue, (IPreviewModel)e.NewValue);
		}
		void OnModelChanged(IPreviewModel oldModel, IPreviewModel newModel) {
			if(oldModel != null) {
				oldModel.DialogService = originalDialogService;
				originalDialogService = null;
			}
			if(newModel != null) {
				originalDialogService = Model.DialogService;
				Model.DialogService = new DialogService(this);
				if(CursorContainer != null)
					Model.CursorService = new CursorService(CursorContainer);
#if SILVERLIGHT
				if(AutoCreateDocument && !designerPropertiesService.GetIsInDesignMode(this) && Model is IDocumentPreviewModel) {
					((IDocumentPreviewModel)Model).CreateDocument();
				}
#endif
			}
		}
#if SILVERLIGHT
		static void OnAutoCreateDocumentChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DocumentViewer)d).OnAutoCreateDocumentChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		void OnAutoCreateDocumentChanged(bool oldValue, bool newValue) {
			if(AutoCreateDocument && !designerPropertiesService.GetIsInDesignMode(this) && Model is IDocumentPreviewModel) {
				((IDocumentPreviewModel)Model).CreateDocument();
			}
		}
		void CollapsePinnedPanels() {
			if(DockLayoutManager != null)
				DockLayoutManager.Collapse(false);
		}
#endif
		#endregion
	}
}
