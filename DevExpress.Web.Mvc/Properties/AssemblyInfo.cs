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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Web.UI;
using DevExpress.Web.Mvc;
using DevExpress.Web.Mvc.Internal;
#if(MVC_FULL_TRUST)
[assembly: AssemblyTitle("DevExpress.Web.Mvc5")]
#else
[assembly: AllowPartiallyTrustedCallers]
[assembly: AssemblyTitle("DevExpress.Web.Mvc")]
#endif
[assembly: AssemblyDescription("MVC Extensions")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Developer Express Inc.")]
[assembly: AssemblyProduct("MVC Extensions")]
[assembly: AssemblyCopyright("Copyright (c) 2000-2015 Developer Express Inc.")]
[assembly: AssemblyTrademark("MVC Extensions")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
[assembly: Guid("8d9b31c6-5456-43f1-a716-256ca463c185")]
[assembly: AssemblyVersion(AssemblyInfo.Version)]
[assembly: AssemblyFileVersion(AssemblyInfo.FileVersion)]
#pragma warning disable 1699
[assembly: AssemblyKeyFile(@"..\..\..\Devexpress.Key\StrongKey.snk")]
#pragma warning restore 1699
[assembly: AssemblyKeyName("")]
[assembly: WebResource(Utils.UtilsScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.GridLookupScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.GridViewScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.FormLayoutScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.HtmlEditorScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.SpellCheckerScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.FileManagerScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.CallbackPanelScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.NavBarScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.PopupControlScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.TabControlScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.TreeViewScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.UploadControlScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.LabelScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.ComboBoxScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.CalendarScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.CaptchaScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.FilterControlScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.ListBoxScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.ChartScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.ButtonScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.BinaryImageResourceName, "text/javascript")]
[assembly: WebResource(Utils.ReportScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.ReportDesignerScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.WebDocumentViewerScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.PivotGridScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.SchedulerScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.DockPanelScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.DockManagerScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.DataViewScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.TreeListScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.ImageGalleryScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.ImageZoomScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.ImageZoomNavigatorScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.TokenBoxScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.RoundPanelScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.SpreadsheetScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.RichEditScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.CardViewScriptResourceName, "text/javascript")]
[assembly: WebResource(Utils.GridAdapterScriptResourceName, "text/javascript")]
