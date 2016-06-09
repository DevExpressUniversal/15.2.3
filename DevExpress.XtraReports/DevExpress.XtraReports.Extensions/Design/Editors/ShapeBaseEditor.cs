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
using System.Text;
using System.Collections;
using System.Reflection;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data;
using DevExpress.Data.Native;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.WinControls;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UI.BarCode;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Shape.Native;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.XtraReports.Design {
	public class ShapeObjectPickerControl : PickerFromValuesControl {
		static Size ImageSize { get { return ShapeFactory.SampleImages.ImageSize; } }
		protected override bool SupportCustomDrawItem { get { return true; } }
		public ShapeObjectPickerControl(ObjectPickerEditor editor, object editValue, ICollection values)
			: base(editor, editValue, values, false) {
		}
		public override void Initialize() {
			listBox.ItemHeight = ImageSize.Height;
			base.Initialize();
		}
		protected override void CustomDrawItem(DrawItemEventArgs e) {
			DrawImageItemText(e, ShapeHelper.GetDisplayName((PreviewStringId)listBox.Items[e.Index]), ImageSize);
			DrawImageItemImage(e, ShapeFactory.SampleImages, e.Index, ImageSize);
		}
	}
	public class ShapeBaseEditor : ObjectPickerEditor {
		#region static
		static void SetShape(XRShape control, string shapeName) {
			PropertyDescriptor descriptor = TypeDescriptor.GetProperties(control)["Shape"];
			descriptor.SetValue(control, ShapeFactory.Create(control, shapeName));
		}
		static XRShape[] GetControls(ITypeDescriptorContext context) {
			XRShape control = context.Instance as XRShape;
			if(control != null)
				return new XRShape[] { control };
			Array componentsArray = context.Instance as Array;
			if(componentsArray != null)
				return GetComponentsFromArray(componentsArray);
			return null;
		}
		static XRShape[] GetComponentsFromArray(Array componentsArray) {
			XRShape[] controls = new XRShape[componentsArray.Length];
			componentsArray.CopyTo(controls, 0);
			return controls;
		}
		#endregion
		protected override ObjectPickerControl CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			return new ShapeObjectPickerControl(this, value, ShapeFactory.ShapeNamesIds);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(IsContextInvalid(context))
				return null;
			XRShape[] controls = GetControls(context);
			if(controls == null || controls.Length <= 0)
				return null;
			PreviewStringId oldValue = controls[0].Shape.ShapeStringId;
			PreviewStringId newValue = (PreviewStringId)base.EditValue(context, provider, oldValue);
			if(newValue == oldValue)
				return null;
			IDesignerHost designerHost = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
			DesignerTransaction transaction = designerHost.CreateTransaction(DesignSR.TransFmt_ShapeChanged);
			try {
				context.OnComponentChanging();
				foreach(XRShape control in controls)
					SetShape(control, ShapeHelper.GetInvariantName(newValue));
				context.OnComponentChanged();
			} catch {
				transaction.Cancel();
			} finally {
				transaction.Commit();
			}
			return null;
		}
	}
}
