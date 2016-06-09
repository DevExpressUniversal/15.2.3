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
using System.Collections.ObjectModel;
using System.ComponentModel;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql.ConnectionStrategies;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Views;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	[ToolboxItem(false)]
	public partial class ConnectionPropertiesPageView : WizardViewBase, IConnectionPropertiesPageView {
		readonly ReadOnlyCollection<ProviderLookupItem> dataProviders;
		string connectionName;
		public ConnectionPropertiesPageView() : this(null) { }
		public ConnectionPropertiesPageView(List<ProviderLookupItem> dataProviders) {
			if(dataProviders != null)
				this.dataProviders = dataProviders.AsReadOnly();
			InitializeComponent();
		}
		#region IWizardPageView Members
		public override string HeaderDescription {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageConnectionProperties); }
		}
		#endregion
		#region IConnectionPropertiesPageView Members
		public string ConnectionName {
			get { return connectionName ?? this.connectionParametersControl.ConnectionName; }
			set {
				connectionName = value;
			}
		}
		public DataConnectionParametersBase DataConnectionParameters {
			get {
				return this.connectionParametersControl.GetParameters();
			}
			set {
				connectionParametersControl.SetParameters(value);
			}
		}
		public event EventHandler Changed;
		public void SetConnections(IEnumerable<INamedItem> connections) {
			this.connectionParametersControl.SetExistingConnections(connections);
		}
		public void InitControls() {
			layoutControlContent.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			if(dataProviders != null)
				connectionParametersControl.Providers = dataProviders;
		}
		public void SelectProvider(string provider) {
			connectionParametersControl.SetProvider(provider);
			RaiseChanged();
		}
		#endregion
		protected void RaiseChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
	}
}
