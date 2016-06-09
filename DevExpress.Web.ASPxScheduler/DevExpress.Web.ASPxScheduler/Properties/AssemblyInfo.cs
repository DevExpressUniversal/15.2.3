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
using System.Resources;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Web.UI;
using DevExpress.Web.ASPxScheduler;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web.ASPxScheduler.Drawing;
using DevExpress.Web.ASPxScheduler.Controls;
using DevExpress.Web.Internal;
using DevExpress.Web;
[assembly: AllowPartiallyTrustedCallers]
[assembly: System.Resources.NeutralResourcesLanguage("en-US")]
[assembly: AssemblyTitle("DevExpress.Web.ASPxScheduler")]
[assembly: AssemblyDescription("ASPxScheduler")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Developer Express Inc.")]
[assembly: AssemblyProduct("DevExpress.Web.ASPxScheduler")]
[assembly: AssemblyCopyright("Copyright (c) 2000-2015 Developer Express Inc.")]
[assembly: AssemblyTrademark("ASPxScheduler")]
[assembly: AssemblyCulture("")]
[assembly: TagPrefix("DevExpress.Web.ASPxScheduler", "dxwschs")]
[assembly: TagPrefix("DevExpress.Web.ASPxScheduler.Controls", "dxwschsc")]
[assembly: TagPrefix("DevExpress.Web.ASPxScheduler.Reporting", "dxwschsc")]
[assembly: CLSCompliantAttribute(true)]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion(AssemblyInfo.Version)]
[assembly: AssemblyFileVersion(AssemblyInfo.FileVersion)]
[assembly: SatelliteContractVersion(AssemblyInfo.SatelliteContractVersion)]
[assembly: WebResource(ASPxScheduler.SchedulerScriptCommonResourceName, "text/javascript")]
[assembly: WebResource(ASPxScheduler.SchedulerScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxViewSelector.ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxViewNavigator.ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxDateNavigator.ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxScheduler.SchedulerScriptRecurrenceControlsResourceName, "text/javascript")]
[assembly: WebResource(ASPxScheduler.SchedulerScriptRecurrenceTypeEditResourceName, "text/javascript")]
[assembly: WebResource(ASPxResourceNavigator.ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxViewVisibleInterval.ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxTimeZoneEdit.ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxScheduler.SchedulerScriptSelectionResourceName, "text/javascript")]
[assembly: WebResource(ASPxScheduler.SchedulerScriptViewInfosResourceName, "text/javascript")]
[assembly: WebResource(ASPxScheduler.SchedulerScriptClientAppointmentResourceName, "text/javascript")]
[assembly: WebResource(ASPxScheduler.SchedulerScriptGlobalFunctionsResourceName, "text/javascript")]
[assembly: WebResource(ASPxScheduler.SchedulerScriptMouseUtilsResourceName, "text/javascript")]
[assembly: WebResource(ASPxScheduler.SchedulerScriptAPIResourceName, "text/javascript")]
[assembly: WebResource(ASPxSchedulerToolTipControlBase.ToolTipScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSchedulerStatusInfo.ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxScheduler.SchedulerScriptMouseHandlerResourceName, "text/javascript")]
[assembly: WebResource(ASPxScheduler.SchedulerScriptKeyboardHandlerResourceName, "text/javascript")]
[assembly: WebResource(ASPxDateNavigatorCalendar.SchedulerCalendarScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxScheduler.SchedulerDefaultCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxScheduler.SchedulerSpriteCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxScheduler.SchedulerSystemCssResourceName, "text/css")]
[assembly: WebResource(ASPxSchedulerImages.ResourceImagesPath + ASPxSchedulerImages.LoadingPanelImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxSchedulerImages.ResourceImagesPath + ImagesBase.SpriteImageName + ".png", "image/png")]
#region CSS images
[assembly: WebResource(ASPxSchedulerImages.ResourceImagesPath + "CssImages.ButtonBack.gif", "image/gif")]
[assembly: WebResource(ASPxSchedulerImages.ResourceImagesPath + "CssImages.ButtonBackHover.gif", "image/gif")]
[assembly: WebResource(ASPxSchedulerImages.ResourceImagesPath + "CssImages.ViewSelectorButtonBack.gif", "image/gif")]
[assembly: WebResource(ASPxSchedulerImages.ResourceImagesPath + "CssImages.ViewSelectorButtonBackHover.gif", "image/gif")]
#endregion
#region InternalsVisibleTo
[assembly: InternalsVisibleTo(AssemblyInfo.SRAssemblySchedulerWebDesign + ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100dfcd8cadc2dd24a7cd4ce95c4a9c1b8e7cb1dc2d665120556b4b0ec35495fddb2bd6eed0ca1e56480276295a225ba2a9746f3d3e1a04547ccf5b26acc3f96eb2a13ac467512497aa79208e32f242fd0618014d53c95a36e5de0e891873841fa8f559566e38e968426488b4aa4d0f0b59e59f38dcf3fbccf25d990ab19c27ddc2")]
[assembly: InternalsVisibleTo(AssemblyInfo.SRAssemblySchedulerReporting + ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100dfcd8cadc2dd24a7cd4ce95c4a9c1b8e7cb1dc2d665120556b4b0ec35495fddb2bd6eed0ca1e56480276295a225ba2a9746f3d3e1a04547ccf5b26acc3f96eb2a13ac467512497aa79208e32f242fd0618014d53c95a36e5de0e891873841fa8f559566e38e968426488b4aa4d0f0b59e59f38dcf3fbccf25d990ab19c27ddc2")]
#endregion
