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
using System.Linq;
using System.Text;
using DevExpress.DocumentView.Controls;
using System.Drawing;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.DocumentView;
using DevExpress.XtraPrinting.Native;
using DevExpress.ReportServer.Printing.Services;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraPrinting.Localization;
using DevExpress.ReportServer.Printing;
using DevExpress.Printing.Core.ReportServer.Services;
using DevExpress.LookAndFeel;
using System.Windows.Forms;
namespace DevExpress.XtraPrinting.Preview {
	[
#if !DEBUG
#endif // DEBUG
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "DocumentViewer.bmp"),
	Designer("DevExpress.XtraPrinting.Design.PrintControlDesigner," + AssemblyInfo.SRAssemblyPrintingDesign),
	Description("A visual control to preview, print, and/or export documents created based on your reports and printing links."),
	ToolboxTabName(AssemblyInfo.DXTabReporting),
	DXToolboxItem(true),
	DefaultProperty("DocumentSource")
	]
	public class DocumentViewer : PrintControl {
		static DocumentViewer() {
			BrickResolver.EnsureStaticConstructor();
		}
		object documentSource;
		IDocumentSource instance;
		IDocumentSource Instance {
			get {
				if(instance == null && documentSource != null) {
					if(documentSource is IDocumentSource)
						instance = (IDocumentSource)documentSource;
					else if(documentSource is Type)
						instance = Activator.CreateInstance((Type)documentSource) as IDocumentSource;
					else if(documentSource is System.Drawing.Printing.PrintDocument)
						instance = new PrintDocumentContainer(documentSource as System.Drawing.Printing.PrintDocument);
					if(instance is RemoteDocumentSource && PrintingSystem != null) {
						PrintingSystem.AddCommandHandler(new RemotePreviewCommandHandler(PrintingSystem));
						PrintingSystem.SetCommandVisibility(
							new PrintingSystemCommand[] { 
								PrintingSystemCommand.FillBackground, 
								PrintingSystemCommand.Watermark, 
								PrintingSystemCommand.PageSetup, 
								PrintingSystemCommand.Customize,
								PrintingSystemCommand.Scale,
								PrintingSystemCommand.Find,
								PrintingSystemCommand.EditPageHF
						}, CommandVisibility.None);
					}
				}
				return instance;
			}
		}
		[
		Category(NativeSR.CatPrinting),
		TypeConverter("DevExpress.XtraPrinting.Design.DocumentSourceConvertor," + AssemblyInfo.SRAssemblyPrintingDesign),
		Editor("DevExpress.XtraPrinting.Design.DocumentSourceEditor," + AssemblyInfo.SRAssemblyPrintingDesign, typeof(System.Drawing.Design.UITypeEditor)),
		DefaultValue(null),
		]
		public object DocumentSource {
			get {
				return documentSource;
			}
			set {
				if(documentSource != value) {
					instance = null;
					documentSource = value;
					base.PrintingSystem = null;
				}
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public override PrintingSystemBase PrintingSystem {
			get {
				if(!DesignMode && base.PrintingSystem == null && Instance != null) {
					base.PrintingSystem = Instance.PrintingSystemBase;
				}
				return base.PrintingSystem;
			}
			set {
				instance = null;
				documentSource = null;
				base.PrintingSystem = value;
			}
		}
		[
		Category(NativeSR.CatPrinting),
		DefaultValue(true)
		]
		public bool RequestDocumentCreation { get; set; }
		public DocumentViewer() {
			RequestDocumentCreation = true;
		}
		protected override void OnDocumentDisposed(object sender, EventArgs e) {
			if(ReferenceEquals(DocumentSource, Document))
				NullInstance();
			base.OnDocumentDisposed(sender, e);
		}
		void NullInstance() {
			instance = null;
			documentSource = null;
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(RequestDocumentCreation && PrintingSystem != null && PrintingSystem.Document.IsEmpty) {
				IDocumentSource originalInstance = Instance;
				BeginInvoke(new MethodInvoker(() => {
					if(!PrintingSystem.Document.IsCreating && originalInstance == Instance) {
						InitiateDocumentCreation();
					}
				}));
			}
		}
		public void InitiateDocumentCreation() {
			if(Instance != null)
				Instance.CreateDocument();
		} 
		protected override void Dispose(bool disposing) {
			NullInstance();
			base.Dispose(disposing);
		}
		protected override void BindPrintingSystem() {
			base.BindPrintingSystem();
			if(PrintingSystem != null && PrintingSystem.GetService<ICredentialsService>() == null) {
				var credentialsService = new CredentialsService(FindForm, LookAndFeel);
				PrintingSystem.AddService<ICredentialsService>(credentialsService);
			}
		}
		protected override void UnbindPrintingSystem() {
			base.UnbindPrintingSystem();
			if(PrintingSystem != null && PrintingSystem.GetService<ICredentialsService>() != null)
				PrintingSystem.RemoveService(typeof(ICredentialsService));
		}
	}
	class WaitFormService : IWaitIndicator, IDisposable {
		UserLookAndFeel lookAndFeel;
		PreviewWaitForm waitForm;
		System.Windows.Forms.Control viewControl;
		object result = null;
		public WaitFormService(System.Windows.Forms.Control viewControl, UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
			this.viewControl = viewControl;
		}
		object IWaitIndicator.Show(string description) {
			if(result == null) {
				result = new object();
				DisposeIndicator();
				waitForm = new PreviewWaitForm(lookAndFeel);
				waitForm.SetCaption(PreviewStringId.WaitForm_Caption.GetString());
				waitForm.SetDescription(description);
				waitForm.Parent = viewControl;
				waitForm.Location = new Point((waitForm.Parent.Width - waitForm.Width) / 2, (waitForm.Parent.Height - waitForm.Height) / 2);
				viewControl.Controls.Add(waitForm);
				waitForm.Show();
				waitForm.Update();
				return result;
			}
			return null;
		}
		bool IWaitIndicator.Hide(object result) {
			if(this.result != null && ReferenceEquals(this.result, result)) {
				DisposeIndicator();
				return true;
			}
			return false;
		}
		void DisposeIndicator() {
			if(waitForm != null && !waitForm.IsDisposed) {
				waitForm.Close();
				viewControl.Controls.Remove(waitForm);
				waitForm.Dispose();
			}
			waitForm = null;
		}
		void IDisposable.Dispose() {
			DisposeIndicator();
		}
	}
	class PreviewWaitForm : DevExpress.XtraWaitForm.DemoWaitForm {
		UserLookAndFeel lookAndFeel;
		public PreviewWaitForm() : this(UserLookAndFeel.Default) { }
		public PreviewWaitForm(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
			this.ProgressPanel.LookAndFeel.ParentLookAndFeel = TargetLookAndFeel;
			this.TopLevel = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		}
		protected override UserLookAndFeel TargetLookAndFeel {
			get {
				return lookAndFeel != null ? lookAndFeel : base.TargetLookAndFeel;
			}
		}
		public new void Show() {
			base.Show();
			Parent.SizeChanged += Parent_SizeChanged;
		}
		public new void Close() {
			Parent.SizeChanged -= Parent_SizeChanged;
			base.Close();
		}
		void Parent_SizeChanged(object sender, EventArgs e) {
			var parent = sender as System.Windows.Forms.Control;
			this.Location = new Point((parent.Width - this.Width) / 2, (parent.Height - this.Height) / 2);
		}
	}
}
