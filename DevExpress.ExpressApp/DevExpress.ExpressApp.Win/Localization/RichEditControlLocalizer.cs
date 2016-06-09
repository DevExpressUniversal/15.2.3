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

using System.ComponentModel;
using System.Resources;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Win.Localization {
	[DisplayName("XtraRichEdit Control")]
	public class RichEditControlLocalizer : DevExpress.XtraRichEdit.Localization.XtraRichEditResLocalizer, IXafResourceLocalizer {
		private ControlResourcesLocalizerLogic logic;
		private IXafResourceManagerParameters xafResourceManagerParameters;
		protected override ResourceManager CreateResourceManagerCore() {
			logic = new ControlResourcesLocalizerLogic(this);
			return logic.Manager;
		}
		void IXafResourceLocalizer.Setup(IModelApplication applicationNode) {
			logic.Setup(applicationNode);
			Active = this;
		}
		void IXafResourceLocalizer.Reset() {
			logic.Reset();
		}
		IXafResourceManagerParameters IXafResourceManagerParametersProvider.XafResourceManagerParameters {
			get {
				if(xafResourceManagerParameters == null) {
					xafResourceManagerParameters = new XafResourceManagerParameters(
						"DevExpress.XtraRichEdit",
						"DevExpress.XtraRichEdit.LocalizationRes",
						"XtraRichEditStringId.",
						typeof(DevExpress.XtraRichEdit.Localization.XtraRichEditResLocalizer).Assembly);
				}
				return xafResourceManagerParameters;
			}
		}
	}
}
