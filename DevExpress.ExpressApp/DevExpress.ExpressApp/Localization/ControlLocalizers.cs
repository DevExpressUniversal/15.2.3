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

using System.Resources;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraPrinting.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Utils.Localization;
using System;
namespace DevExpress.ExpressApp.Localization {
	public class SessionLocalizerProvider<LocalizerType, StringIdType> : DefaultActiveLocalizerProvider<StringIdType>
		where StringIdType : struct
		where LocalizerType : XtraLocalizer<StringIdType>, new() {
		protected override XtraLocalizer<StringIdType> GetActiveLocalizerCore() {
			return XafActiveLocalizer ?? new LocalizerType();
		}
		protected override void SetActiveLocalizerCore(XtraLocalizer<StringIdType> localizer) {
		}
		static SessionLocalizerProvider() {
		}
		public SessionLocalizerProvider()
			: base(null) {
		}
		public static LocalizerType XafActiveLocalizer {
			get {
				return DevExpress.Persistent.Base.ValueManager.GetValueManager<LocalizerType>("XafLocalizerProvider_LocalizerType").Value;
			}
			set {
				DevExpress.Persistent.Base.ValueManager.GetValueManager<LocalizerType>("XafLocalizerProvider_LocalizerType").Value = value;
				if(value != null) {
					XtraLocalizer<StringIdType>.SetActiveLocalizerProvider(new SessionLocalizerProvider<LocalizerType, StringIdType>());
				}
			}
		}
	}
	[System.ComponentModel.DisplayName("XtraPrinting Control")]
	public class PreviewControlLocalizer : PreviewResLocalizer, IXafResourceLocalizer {
		ControlResourcesLocalizerLogic logic;
		protected override ResourceManager CreateResourceManagerCore() {
			logic = new ControlResourcesLocalizerLogic(this);
			return logic.Manager;
		}
		#region IXafResourceLocalizer Members
		public void Setup(IModelApplication applicationNode) {
			logic.Setup(applicationNode);
			SessionLocalizerProvider<PreviewControlLocalizer, PreviewStringId>.XafActiveLocalizer = this;
		}
		public void Reset() {
			logic.Reset();
		}
		#endregion
		#region IXafResourceManagerParametersProvider Members
		private IXafResourceManagerParameters xafResourceManagerParameters;
		public IXafResourceManagerParameters XafResourceManagerParameters {
			get {
				if(xafResourceManagerParameters == null) {
					xafResourceManagerParameters = new XafResourceManagerParameters(
						"DevExpress.Printing",
						"DevExpress.Data.Printing.LocalizationRes",
						"PreviewStringId.",
						typeof(PreviewResLocalizer).Assembly
						);
				}
				return xafResourceManagerParameters;
			}
		}
		#endregion
	}
	public enum ApplicationStatusMesssageId {
		LoadApplicationModules,
		LoadUserDifferences,
		GenerateApplicationModel,
		UpdateDatabaseSchema,
		UpdateDatabaseData,
		ApplicationSetupStarted,
		ShowStartupWindow
	}
	public class ApplicationStatusMesssagesLocalizer : XafResourceLocalizer<ApplicationStatusMesssageId> {
		private static readonly ApplicationStatusMesssagesLocalizer localizer = new ApplicationStatusMesssagesLocalizer();
		protected override IXafResourceManagerParameters GetXafResourceManagerParameters() {
			return new XafResourceManagerParameters(
				"ApplicationStatusMesssages",
				"DevExpress.ExpressApp.Localization.ApplicationStatusMesssages",
				String.Empty,
				GetType().Assembly
				);
		}
		public static ApplicationStatusMesssagesLocalizer Active {
			get { return localizer; }
		}
	}
}
