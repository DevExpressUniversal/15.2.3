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
	using System.Collections.Generic;
	class ServerModeSettingsModel : ServerModeSettingsModelBase, IServerModeSettingsModel {
		public ServerModeSettingsModel(IDataSourceInfo info)
			: base(info) {
			UpdateKeyExpressions();
		}
		IEnumerable<string> keyExpressionsCore;
		public IEnumerable<string> KeyExpressions {
			get { return keyExpressionsCore; }
			private set { SetProperty(ref keyExpressionsCore, value, "KeyExpressions"); }
		}
		string keyExpressionCore;
		public string KeyExpression {
			get { return keyExpressionCore; }
			set { SetProperty(ref keyExpressionCore, value, "KeyExpression"); }
		}
		protected IKeyExpressionsInfo KeyExpressionsInfo {
			get { return SelectedElement as IKeyExpressionsInfo; }
		}
		protected override void OnSelectedElementChanged() {
			base.OnSelectedElementChanged();
			UpdateKeyExpressions();
		}
		protected void UpdateKeyExpressions() {
			var keyExpressions = (KeyExpressionsInfo != null) ? KeyExpressionsInfo.KeyExpressions : new string[] { };
			if(!System.Linq.Enumerable.Any(keyExpressions))
				keyExpressions = Fields;
			KeyExpressions = keyExpressions;
			KeyExpression = System.Linq.Enumerable.FirstOrDefault(keyExpressions);
		}
		protected override System.Type GetKey() {
			return typeof(IServerModeSettingsModel);
		}
	}
	class PLinqServerModeSettingsModel : ServerModeSettingsModelBase, IPLinqServerModeSettingsModel {
		public PLinqServerModeSettingsModel(IDataSourceInfo info)
			: base(info) {
		}
		protected sealed override System.Type GetKey() {
			return typeof(IPLinqServerModeSettingsModel);
		}
	}
	class XPOServerModeSettingsModel : ServerModeSettingsModelBase, IXPServerCollectionSourceSettingsModel {
		public XPOServerModeSettingsModel(IDataSourceInfo info)
			: base(info) {
		}
		protected sealed override System.Type GetKey() {
			return typeof(IXPServerCollectionSourceSettingsModel);
		}
	}
}
