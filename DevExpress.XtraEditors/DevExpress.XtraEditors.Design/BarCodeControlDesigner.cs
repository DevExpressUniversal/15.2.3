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
using System.Text;
using DevExpress.Utils.Design;
using System.ComponentModel.Design;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraPrinting.BarCode;
using System.Drawing.Design;
namespace DevExpress.XtraEditors.Design {
	class BarCodeControlDesigner : BaseControlDesigner {
		BarCodeControl BarCodeControl { get { return (BarCodeControl)this.Control; } }
		public override void InitializeNewComponent(IDictionary defaultValues) {
			string initialBarCodeText = BarCodeControl.Text;
			base.InitializeNewComponent(defaultValues);
			BarCodeControl.Text = initialBarCodeText;
			BarCodeControl.Padding = new System.Windows.Forms.Padding(10, 2, 10, 0);
		}
		public override void Initialize(System.ComponentModel.IComponent component) {
			base.Initialize(component);
			IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			changeService.ComponentChanged += new ComponentChangedEventHandler(changeService_ComponentChanged);
		}
		void changeService_ComponentChanged(object sender, ComponentChangedEventArgs e) {
			if(e.Component == this.Control)
				Control.Invalidate();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if(changeService != null)
				changeService.ComponentChanged -= new ComponentChangedEventHandler(changeService_ComponentChanged);
		}
		protected override bool CanUseComponentSmartTags { get { return true; } }
	}
	public class BarCodeDataConverter : ArrayConverter {
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			BarCodeControl barCode = context.Instance as BarCodeControl;
			if(barCode != null)
				return barCode.Symbology is BarCode2DGenerator;
			return base.GetPropertiesSupported(context);
		}
	}
	public class BarCodeDataEditor : ArrayEditor {
		public BarCodeDataEditor(Type type) : base(type) { }
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			BarCodeControl barCode = context.Instance as BarCodeControl;
			if(barCode != null && barCode.Symbology is BarCode2DGenerator)
				return base.GetEditStyle(context);
			return UITypeEditorEditStyle.None;
		}
	}
}
