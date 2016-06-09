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
using System.Globalization;
using System.Resources;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.DataAccess.UI.Localization {
	public partial class DataAccessUILocalizer : XtraLocalizer<DataAccessUIStringId> {
		static DataAccessUILocalizer() {
			if(GetActiveLocalizerProvider() == null)
				SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<DataAccessUIStringId>(CreateDefaultLocalizer()));
		}
		public static new XtraLocalizer<DataAccessUIStringId> Active {
			get { return XtraLocalizer<DataAccessUIStringId>.Active; }
			set {
				if (GetActiveLocalizerProvider() as DefaultActiveLocalizerProvider<DataAccessUIStringId> == null) {
					SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<DataAccessUIStringId>(value));
					RaiseActiveChanged();
				}
				else
					XtraLocalizer<DataAccessUIStringId>.Active = value;
			}
		}
		public static XtraLocalizer<DataAccessUIStringId> CreateDefaultLocalizer() {
			return new DataAccessUIResLocalizer();
		}
		public static string GetString(DataAccessUIStringId id) {
			return Active.GetLocalizedString(id);
		}
		protected override void PopulateStringTable() {
			AddStrings();
		}
		public override XtraLocalizer<DataAccessUIStringId> CreateResXLocalizer() {
			return new DataAccessUIResLocalizer();
		}
	}
	public class DataAccessUIResLocalizer : DataAccessUILocalizer {
		readonly ResourceManager manager;
		public override string Language { get { return CultureInfo.CurrentUICulture.Name; } }
		public DataAccessUIResLocalizer() {
			if(manager != null)
				manager.ReleaseAllResources();
			manager = new ResourceManager("DevExpress.DataAccess.UI.LocalizationRes", GetType().Assembly);
		}
		public override string GetLocalizedString(DataAccessUIStringId id) {
			return manager.GetString("DataAccessUIStringId." + id) ?? String.Empty;
		}
	}
}
