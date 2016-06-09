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

using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using DevExpress.Web;
using DevExpress.ExpressApp.Maps.Web.Helpers;
namespace DevExpress.ExpressApp.Maps.Web {
	[DXWebToolboxItem(false)]
	public class MapViewer : MapViewerBase {
		public MapSettings MapSettings { get; set; }
		public MapViewerClientSideEvents ClientSideEvents {
			get { return base.ClientSideEventsInternal as MapViewerClientSideEvents; }
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new MapViewerClientSideEvents();
		}
		protected override string GetClientObjectClassName() {
			return "MapViewer";
		}
		protected override void GetCreateClientObjectScript(StringBuilder scriptContainer, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(scriptContainer, localVarName, clientName);
			MapViewerJSGenerator jsGenerator = new MapViewerJSGenerator(localVarName, MapSettings, objectInfoHelper);
			scriptContainer.Append(jsGenerator.GenerateScript());
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MapViewer), MapsAspNetModule.MapScriptResourceName);
		}
		public MapViewer(IObjectInfoHelper objectInfoHelper, MapSettings mapSettings)
			: base(objectInfoHelper) {
			MapSettings = mapSettings;
			Width = mapSettings.Width == 0 ? Unit.Percentage(100) : mapSettings.Width;
			Height = mapSettings.Height == 0 ? Unit.Pixel(400) : mapSettings.Height;
			if(mapSettings.Height == 0) {
				mapSettings.Height = 400;
			}
		}
	}
}
