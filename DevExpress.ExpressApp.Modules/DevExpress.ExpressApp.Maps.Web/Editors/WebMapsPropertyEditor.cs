﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Web.UI.WebControls;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Controls;
using DevExpress.ExpressApp.Maps.Web.Helpers;
namespace DevExpress.ExpressApp.Maps.Web {
	public class WebMapsPropertyEditor : ASPxObjectPropertyEditor {
		public MapViewer MapViewer { get; private set; }
		protected override WebControl CreateViewModeControlCore() {
			MapsAspNetModule.SetEmbeddingClientLibraries(application);
			MapSettings mapSettings = MapsAspNetModule.MapSettingsFromModel((IModelMaps)Model, application);
			mapSettings.IsShowDetailsEnabled = false;
			MapViewer = new MapViewer(new ObjectInfoHelper(null), mapSettings);
			return MapViewer;
		}
		protected override void ReadViewModeValueCore() {
			var markersList = MapViewer.MapSettings.Markers as List<IMapsMarker>;
			markersList.Clear();
			markersList.Add((IMapsMarker)PropertyValue);
		}
		public WebMapsPropertyEditor(Type objectType, IModelMemberViewItem info)
			: base(objectType, info) {
		}
	}
}
