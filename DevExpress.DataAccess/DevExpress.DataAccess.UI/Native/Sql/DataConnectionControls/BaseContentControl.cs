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
using System.Windows.Forms;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.Xpo.DB;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Native.Sql.DataConnectionControls {
	[DXToolboxItem(false)]
	public partial class BaseContentControl : XtraUserControl {
		protected static int Delta { get { return 26; } }
		ProviderChooser providerChooser;
		protected XtraForm WizardParentForm
		{
			get
			{
				Control control = this;
				while(control.Parent != null) {
					XtraForm xtraForm = control.Parent as XtraForm;
					if(xtraForm != null)
						return xtraForm;
					control = control.Parent;
				}
				return null;
			}
		}
		protected ProviderChooser ProviderChooser { get { return this.providerChooser; } }
		public virtual bool IsServerbased { get; set; }
		public virtual ProviderFactory Factory { get; set; }
		public virtual string ConnectionNamePatternServerPart { get { return null; } }
		public virtual string ConnectionNamePatternDatabasePart { get { return null; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataConnectionParametersBase Parameters
		{
			get
			{
				DataConnectionParametersBase parameters = GetParameters();
				parameters.Factory = Factory;
				return parameters;
			}
			set { ApplyParameters(value); }
		}
		public BaseContentControl() {
			InitializeComponent();
		}
		protected void ChangeConnectionName() {
			if(this.providerChooser != null)
				this.providerChooser.ChangeConnectionName(true);
		}
		public virtual event EventHandler Changed;
		protected virtual DataConnectionParametersBase GetParameters() {
			throw new NotSupportedException();
		}
		protected virtual void ApplyParameters(DataConnectionParametersBase parameters) {
			throw new NotSupportedException();
		}
		public void Initialize(ProviderFactory providerFactory, ProviderChooser providerChooser) {
			Factory = providerFactory;
			this.providerChooser = providerChooser;
			ChangeConnectionName();
		}
		protected virtual void OnChanged() {
			var handler = Changed;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
	}
}
