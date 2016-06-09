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
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using System.ComponentModel.Design;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraEditors;
using DevExpress.Utils;
namespace DevExpress.XtraLayout.Converter {
	[Designer(LayoutControlConstants.LayoutConverterDesignerName, typeof(System.ComponentModel.Design.IDesigner)), ToolboxBitmap(typeof(LayoutControl), "Images.layoutconverter256.bmp"),
	 Description("Converts an existing regular layout of controls to a LayoutControl, and vice versa."),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation), DXToolboxItem(DXToolboxItemKind.Free)
	]
	public class LayoutConverter : Component {
		ToStandardLayoutConverterHelper standardLayoutHelper;
		ToXtraLayoutConverterHelper xtraLayoutHelper;
		public LayoutConverter() {
			standardLayoutHelper = CreateStandardLayoutHelper();
			xtraLayoutHelper = CreateXtraLayoutHelper();
		}
		public LayoutConverter(IContainer container) {
			container.Add(this);
		}
		protected virtual ToStandardLayoutConverterHelper CreateStandardLayoutHelper() {
			return new ToStandardLayoutConverterHelper(this);
		}
		protected virtual ToXtraLayoutConverterHelper CreateXtraLayoutHelper() {
			return new ToXtraLayoutConverterHelper(this);
		}
		public Form ContainerForm {
			get {
				if(Container == null) return null;
				foreach(Component temp in Container.Components) {
					Form tempForm = temp as Form;
					if(tempForm != null) return tempForm;
				}
				return null;
			}
		}
		public void ConvertToStandardLayout(LayoutControl layoutControl) {
			standardLayoutHelper.ConvertToStandardLayout(layoutControl);
		}
		public virtual LayoutControl ConvertToXtraLayout(Control container) {
			return xtraLayoutHelper.ConvertToXtraLayout(container);
		}
		public virtual void ShowDetails(Control container) {
			xtraLayoutHelper.ShowDetails(container);
		}
		public virtual void HideDetails() {
			xtraLayoutHelper.HideDetails();
		}
	}
}
