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
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.ExpressApp.Maps.Web.Helpers;
namespace DevExpress.ExpressApp.Maps.Web {
	[DXWebToolboxItem(false)]
	public class VectorMapViewer : MapViewerBase {
		public VectorMapSettings VectorMapSettings { get; set; }
		public VectorMapViewerClientSideEvents ClientSideEvents {
			get { return base.ClientSideEventsInternal as VectorMapViewerClientSideEvents; }
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new VectorMapViewerClientSideEvents();
		}
		protected override string GetClientObjectClassName() {
			return "VectorMapViewer";
		}
		protected string GetVectorMapResourceName(VectorMapType vectorMapType) {
			string mapResourceName = MapsAspNetModule.WorldMapScriptResourceName;
			switch(vectorMapType) {
				case VectorMapType.Africa:
					mapResourceName = MapsAspNetModule.AfricaMapScriptResourceName;
					break;
				case VectorMapType.Canada:
					mapResourceName = MapsAspNetModule.CanadaMapScriptResourceName;
					break;
				case VectorMapType.Eurasia:
					mapResourceName = MapsAspNetModule.EurasiaMapScriptResourceName;
					break;
				case VectorMapType.Europe:
					mapResourceName = MapsAspNetModule.EuropeMapScriptResourceName;
					break;
				case VectorMapType.USA:
					mapResourceName = MapsAspNetModule.UsaMapScriptResourceName;
					break;
				default:
					mapResourceName = MapsAspNetModule.WorldMapScriptResourceName;
					break;
			}
			return mapResourceName;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			VectorMapViewerJSGenerator vectorJsGenerator = new VectorMapViewerJSGenerator(localVarName, VectorMapSettings, objectInfoHelper);
			stb.Append(vectorJsGenerator.GenerateScript());
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(VectorMapViewer), GetVectorMapResourceName(VectorMapSettings.Type));
			RegisterIncludeScript(typeof(VectorMapViewer), MapsAspNetModule.VectorMapScriptResourceName);
			RegisterDevExtremeVizWidgetsScript(Page);
		}
		public VectorMapViewer(IObjectInfoHelper objectInfoHelper, VectorMapSettings vectorMapSettings)
			: base(objectInfoHelper) {
			VectorMapSettings = vectorMapSettings;
			Width = vectorMapSettings.Width == 0 ? Unit.Percentage(100) : vectorMapSettings.Width;
			Height = vectorMapSettings.Height == 0 ? Unit.Percentage(100) : vectorMapSettings.Height;
		}
	}
}
