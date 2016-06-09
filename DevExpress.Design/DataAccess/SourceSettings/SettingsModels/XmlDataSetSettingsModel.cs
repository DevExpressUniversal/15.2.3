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

namespace DevExpress.Design.DataAccess {
	class XmlDataSetSettingsModel : DataSourceSettingsModelBase, IXmlDataSetSettingsModel {
		public XmlDataSetSettingsModel(IDataSourceInfo info)
			: base(info) {
			ResetXmlPathCommand = new Design.UI.WpfDelegateCommand(ResetXmlPath);
			SetXmlPathCommand = new Design.UI.WpfDelegateCommand(SetXmlPath);
		}
		protected sealed override System.Type GetKey() {
			return typeof(IXmlDataSetSettingsModel);
		}
		protected new IXmlDataSourceInfo Info {
			get { return base.Info as IXmlDataSourceInfo; }
		}
		protected override void RegisterValidationRules() {
			RegisterValidationRule("SelectedElement", (model) =>
			{
				if(string.IsNullOrEmpty(((IXmlDataSetSettingsModel)model).XmlPath)) return null;
				return (model.SelectedElement == null) ? "Table must be specified" : null;
			});
			RegisterValidationRule(DataSourcePropertyCodeName.XmlPath, (model) =>
			{
				return string.IsNullOrEmpty(((IXmlDataSetSettingsModel)model).XmlPath) ? "Path must be specified" : null;
			});
		}
		#region Properties
		string xmlPathCore;
		public string XmlPath {
			get { return xmlPathCore; }
			set { SetProperty(ref xmlPathCore, value, "XmlPath", OnXmlPathChanged); }
		}
		public bool HasXmlPath {
			get { return !string.IsNullOrEmpty(XmlPath); }
		}
		#endregion Properties
		#region Commands
		public Design.UI.ICommand<object> ResetXmlPathCommand {
			get;
			private set;
		}
		public Design.UI.ICommand<object> SetXmlPathCommand {
			get;
			private set;
		}
		#endregion Properties
		protected void OnXmlPathChanged() {
			RaisePropertyChanged("HasXmlPath");
			Info.UpdateElements(XmlPath);
			RaisePropertyChanged("Elements");
		}
		void ResetXmlPath() {
			XmlPath = null;
		}
		void SetXmlPath() {
			Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
			openFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
			openFileDialog.RestoreDirectory = true;
			bool? result = openFileDialog.ShowDialog();
			if(result.HasValue && result.Value)
				XmlPath = openFileDialog.FileName;
		}
	}
}
