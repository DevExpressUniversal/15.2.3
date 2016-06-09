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
using System.ComponentModel.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Localization;
using DevExpress.Data;
using System.Globalization;
using DevExpress.Utils.FormatStrings;
namespace DevExpress.XtraReports.Design {
	public class XRFormatStringEditorForm : FormatStringEditorForm {
		public XRFormatStringEditorForm(string initialFormat, IServiceProvider provider)
			: base(initialFormat, provider) {
		}
		protected override bool ShouldHideGeneralItem {
			get {
				ISelectionService selectionService = provider != null ? (ISelectionService)provider.GetService(typeof(ISelectionService)) : null;
				if (selectionService == null)
					return false;
				IDesignerHost designerHost = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
				if (designerHost == null)
					return false;
				XRFieldEmbeddableControl control = selectionService.PrimarySelection as XRFieldEmbeddableControl;
				if (control == null)
					return false;
				XRTextControlDesigner designer = designerHost.GetDesigner(control) as XRTextControlDesigner;
				return (designer != null && designer.Editor != null) || !control.HasDisplayDataBinding;
			}
		}
		protected override string GetEditValue() {
			string s0 = tbPrefix.Text;
			string s1 = lbxFormat.Text;
			string s2 = tbSuffix.Text;
			if (IsCustomFormat) {
				s0 = s2 = "";
				s1 = tbCustomFormat.Text;
			} else if (CategoryName == CategoryItem.GetDisplayName(general)) {
				return s0 + "{0}" + s2;
			}
			return MailMergeFieldInfo.MakeFormatString(s1);
		}
		protected override string RegistryPath {
			get { return DevExpress.XtraPrinting.Native.NativeSR.RegistryPath + "CustomFormat\\"; }
		}
	}
}
