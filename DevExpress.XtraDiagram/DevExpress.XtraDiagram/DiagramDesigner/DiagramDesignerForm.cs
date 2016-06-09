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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Diagram.Core.Localization;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Options;
namespace DevExpress.XtraDiagram.Designer {
	public partial class DiagramDesignerFormBase : RibbonForm {
		static DiagramDesignerFormBase() {
			SkinManager.EnableFormSkins();
		}
		DiagramDesignerControlBase designerControl;
		public DiagramDesignerFormBase() {
			this.designerControl = CreateDesignerControl();
			InitializeComponent();
		}
		protected DiagramDesignerControlBase CreateDesignerControl() {
			DiagramDesignerControlBase designerControl = CreateDesignerControlInstance();
			InitializeDesignerControl(designerControl);
			Controls.Add(designerControl);
			return designerControl;
		}
		protected virtual void InitializeDesignerControl(DiagramDesignerControlBase designerControl) {
			designerControl.Dock = DockStyle.Fill;
		}
		protected virtual DiagramDesignerControlBase CreateDesignerControlInstance() {
			return new DiagramDesignerControlBase();
		}
		public void Initialize() {
			DesignerControl.Initialize();
			ApplyDefaultOptions();
		}
		protected virtual void ApplyDefaultOptions() {
			Icon = LoadDefaultIcon();
		}
		protected virtual Icon LoadDefaultIcon() {
			return ResourceImageHelper.CreateIconFromResources("DevExpress.XtraDiagram.Images.DX.ico", typeof(DiagramControl).Assembly);
		}
		public DiagramControl Diagram { get { return DesignerControl.Diagram; } }
		public DiagramDesignerControlBase DesignerControl { get { return designerControl; } }
	}
	public class DiagramDesignerForm : DiagramDesignerFormBase {
		public DiagramDesignerForm() : base() {
		}
		protected override DiagramDesignerControlBase CreateDesignerControlInstance() {
			return new DiagramDesignerControl();
		}
		public void Initialize(DiagramControl ownerDiagram) {
			DesignerControl.Initialize(ownerDiagram);
			ApplyOptions(ownerDiagram.OptionsDesigner);
		}
		protected virtual void ApplyOptions(DiagramOptionsDesigner options) {
			Icon = options.FormIcon ?? LoadDefaultIcon();
			ShowInTaskbar = false;
		}
		protected bool IsDocumentChanged() {
			return Diagram.HasChanges || Diagram.IsNewDocumentLoaded;
		}
		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);
			if(IsDocumentChanged()) {
				Diagram.ConfirmSaveChanges(this, e);
			}
		}
		public new DiagramDesignerControl DesignerControl { get { return base.DesignerControl as DiagramDesignerControl; } }
	}
}
