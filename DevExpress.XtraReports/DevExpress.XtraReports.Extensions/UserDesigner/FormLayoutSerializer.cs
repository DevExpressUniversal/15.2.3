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
using System.Windows.Forms;
using DevExpress.XtraPrinting.Preview;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraReports.UserDesigner {
	public class FormLayoutSerializer : IDisposable {
		Form form;
		string name;
		FormLayout formLayout;
		public FormLayoutSerializer(Form form, string name) {
			this.form = form;
			this.name = name;
			this.formLayout = CreateFormLayout();
		}
		protected virtual FormLayout CreateFormLayout() {
			return new FormLayout(form);
		}
		protected virtual XtraSerializer CreateSerializer() {
			return new RegistryXtraSerializer();
		}
		protected virtual string AppName {
			get { return name; }
		}
		protected virtual string Path {
			get { return "\\Software\\Developer Express\\XtraReports\\"; }
		}
		public virtual void SaveLayout() {
			XtraSerializer serializer = CreateSerializer();
			formLayout.SaveFormLayout();
			serializer.SerializeObject(formLayout, Path, AppName);
		}
		public virtual void RestoreLayout() {
			XtraSerializer serializer = CreateSerializer();
			serializer.DeserializeObject(formLayout, Path, AppName);
			formLayout.RestoreFormLayout();
		}
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
		}
		public void Dispose(bool disposing) {
			if(disposing) {
				if(formLayout != null) {
					formLayout.Dispose();
					formLayout = null;
				}
			}
		}
		~FormLayoutSerializer() {
			Dispose(false);
		}
		#endregion
	}
}
