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
using DevExpress.XtraReports.UserDesigner;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using DevExpress.XtraReports.UI;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using System.ComponentModel;
namespace DevExpress.XtraReports.Native {
	public class ResLoader : ResLoaderBase {
		public static void LoadDesignTimeProperties(XRControl control, System.ComponentModel.Design.IDesignerHost designerHost, System.Resources.ResourceSet resourceSet) {
			object designer = designerHost.GetDesigner(control);
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(designer, new Attribute[] { DesignOnlyAttribute.Yes });
			foreach(PropertyDescriptor p in props) {
				string resourceName = control is XtraReport ? String.Format("$this.{0}", p.Name) : String.Format("{0}.{1}", control.Name, p.Name);
				PropInfoAccessor.SetPropertyFromResource(resourceSet, resourceName, designer, p.Name);
			}
		}
	}
}
