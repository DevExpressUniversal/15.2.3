#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Model;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Utils.Localization;
namespace DevExpress.ExpressApp.Localization {
	public abstract class XafResourceLocalizer : IXafResourceLocalizer {
		private XafResourceManager manager = null;
		private IModelApplication modelApplication;
		private bool raiseExceptionIfItemIsNotFound;
		public bool RaiseExceptionIfItemIsNotFound {
			get { return raiseExceptionIfItemIsNotFound; }
			set { raiseExceptionIfItemIsNotFound = value; }
		}
		public XafResourceLocalizer() {
			manager = new XafResourceManager(this);
		}
		protected abstract IXafResourceManagerParameters GetXafResourceManagerParameters();
		public string GetLocalizedString(string stringId) {
			string result = manager.GetString(stringId);
			if(result == null) {
				if(raiseExceptionIfItemIsNotFound) {
					throw new ArgumentException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ResourceItemWithNameIsNotFound, stringId));
				}
				result = string.Empty;
			}
			return result.Replace("\\n", "\n").Replace("\\r", "\r");
		}
		public XafResourceManager ResourceManager { get { return manager; } }
		public string GetLocalizedString(string messageId, params object[] args) {
			return string.Format(GetLocalizedString(messageId), args);
		}
		#region IXafResourceLocalizer Members
		public void Setup(IModelApplication modelApplication) {
			this.modelApplication = modelApplication;
			xafResourceManagerParameters.ModelApplication = modelApplication;
			Reset();
			Activate();
		}
		public void Reset() {
			manager.ReloadModelData();
		}
		public virtual void Activate() { }
		#endregion
		#region IXafResourceManagerParametersProvider Members
		private IXafResourceManagerParameters xafResourceManagerParameters;
		public IXafResourceManagerParameters XafResourceManagerParameters {
			get {
				if(xafResourceManagerParameters == null) {
					xafResourceManagerParameters = GetXafResourceManagerParameters();
				}
				return xafResourceManagerParameters;
			}
		}
		#endregion
	}
	public abstract class XafResourceLocalizer<EnumType> : XafResourceLocalizer where EnumType : IConvertible {
		public string GetLocalizedString(EnumType messageId) {
			return GetLocalizedString(messageId.ToString());
		}
		public string GetLocalizedString(EnumType messageId, params object[] args) {
			return string.Format(GetLocalizedString(messageId), args);
		}
	}
}
