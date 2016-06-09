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

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web.UI;
using DevExpress.Data;
using DevExpress.Web.ASPxPivotGrid;
using DevExpress.Web.ASPxPivotGrid.Data;
using System.Security;
using DevExpress.Web.Internal;
using DevExpress.Web;
using DevExpress.XtraPivotGrid.Data;
using System;
using System.Resources;
[assembly: AllowPartiallyTrustedCallers()]
[assembly: AssemblyTitle("DevExpress.Web.ASPxPivotGrid")]
[assembly: AssemblyDescription("ASPxPivotGrid")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Developer Express Inc.")]
[assembly: AssemblyProduct("DevExpress.Web.ASPxPivotGrid")]
[assembly: AssemblyCopyright("Copyright (c) 2000-2015 Developer Express Inc.")]
[assembly: AssemblyTrademark("ASPxPivotGrid")]
[assembly: CLSCompliant(true)]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: InternalsVisibleTo(AssemblyInfo.SRAssemblyWebDesign + ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100dfcd8cadc2dd24a7cd4ce95c4a9c1b8e7cb1dc2d665120556b4b0ec35495fddb2bd6eed0ca1e56480276295a225ba2a9746f3d3e1a04547ccf5b26acc3f96eb2a13ac467512497aa79208e32f242fd0618014d53c95a36e5de0e891873841fa8f559566e38e968426488b4aa4d0f0b59e59f38dcf3fbccf25d990ab19c27ddc2")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion(AssemblyInfo.Version)]
[assembly: SatelliteContractVersion(AssemblyInfo.SatelliteContractVersion)]
[assembly: AssemblyFileVersion(AssemblyInfo.FileVersion)] 
#pragma warning disable 1699
[assembly: AssemblyKeyFile(@"..\..\..\Devexpress.Key\StrongKey.snk")]
#pragma warning restore 1699
[assembly: AssemblyKeyName("")]
[assembly: TagPrefix("DevExpress.Web.ASPxPivotGrid", "dx")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + ImagesBase.SpriteImageName + ".png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + PivotGridImages.LoadingPanelImageName + ".gif", "image/gif")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + PivotGridImages.CustomizationFieldsBackgroundName + ".gif", "image/gif")]
[assembly: WebResource(PivotGridWebData.PivotGridScriptResourceName, "text/javascript")]
[assembly: WebResource(PivotGridWebData.PivotTableWrapperScriptResourceName, "text/javascript")]
[assembly: WebResource(PivotGridWebData.AdjustingManagerScriptResourceName, "text/javascript")]
[assembly: WebResource(PivotGridWebData.DragAndDropScriptResourceName, "text/javascript")]
[assembly: WebResource(PivotGridWebData.GroupFilterScriptResourceName, "text/javascript")]
[assembly: WebResource(PivotGridWebData.CustomizationTreeScriptResourceName, "text/javascript")]
[assembly: WebResource(PivotGridWebData.PivotGridSystemCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(PivotGridWebData.PivotGridDefaultCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(PivotGridWebData.PivotGridSpriteCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "Cylinder.-1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "Cylinder.0.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "Cylinder.1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "Faces.-1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "Faces.0.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "Faces.1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "Gauge.-1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "Gauge.0.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "Gauge.1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "ReversedCylinder.-1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "ReversedCylinder.0.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "ReversedCylinder.1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "ReversedGauge.-1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "ReversedGauge.0.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "ReversedGauge.1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "ReversedStatusArrow.-1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "ReversedStatusArrow.0.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "ReversedStatusArrow.1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "ReversedThermometer.-1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "ReversedThermometer.0.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "ReversedThermometer.1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "RoadSigns.-1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "RoadSigns.0.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "RoadSigns.1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "Shapes.-1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "Shapes.0.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "Shapes.1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "StandardArrow.-1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "StandardArrow.0.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "StandardArrow.1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "StatusArrow.-1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "StatusArrow.0.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "StatusArrow.1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "Thermometer.-1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "Thermometer.0.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "Thermometer.1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "TrafficLights.-1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "TrafficLights.0.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "TrafficLights.1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "VarianceArrow.-1.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "VarianceArrow.0.png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + "VarianceArrow.1.png", "image/png")]
