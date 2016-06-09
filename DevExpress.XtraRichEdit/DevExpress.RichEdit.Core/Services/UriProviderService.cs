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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
#if !SL
using System.Drawing.Imaging;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraRichEdit.Services {
	#region IUriProviderService
	[ComVisible(true)]
	[Obsolete("Please use the DevExpress.Office.Services.IUriProviderService instead", true)]
	public interface IUriProviderService : DevExpress.Office.Services.IUriProviderService {
	}
	#endregion
	#region IUriProvider
	[ComVisible(true)]
	[Obsolete("Please use the DevExpress.Office.Services.IUriProvider instead", true)]
	public interface IUriProvider : DevExpress.Office.Services.IUriProvider {
	}
	#endregion
	public interface IPdfLinkUpdater {
		string UpdateLinkToPage(string uri, string anchor);
		string UpdateLinkToUri(string uri);
	}
}
namespace DevExpress.XtraRichEdit.Services.Implementation {
	#region UriProviderCollection
	[Obsolete("Please use the DevExpress.Office.Services.Implementation.UriProviderCollection instead", true)]
	[ComVisible(true)]
	public class UriProviderCollection : List<IUriProvider> {
	}
	#endregion
	#region UriProviderService
	[ComVisible(true)]
	[Obsolete("Please use the DevExpress.Office.Services.Implementation.UriProviderService instead", true)]
	public class UriProviderService : DevExpress.Office.Services.Implementation.UriProviderService, IUriProviderService {
		protected override void RegisterDefaultProviders() {
#if !SL && !DXPORTABLE
			RegisterProvider(new FileBasedUriProvider());
#endif
		}
	}
	#endregion
	#region EmptyUriProvider
	[ComVisible(true)]
	[Obsolete("Please use the DevExpress.Office.Services.Implementation.EmptyUriProvider instead", true)]
	public class EmptyUriProvider : DevExpress.Office.Services.Implementation.EmptyUriProvider, IUriProvider {
	}
	#endregion
	#region DataStringUriProvider
	[ComVisible(true)]
	[Obsolete("Please use the DevExpress.Office.Services.Implementation.DataStringUriProvider instead", true)]
	public class DataStringUriProvider : DevExpress.Office.Services.Implementation.DataStringUriProvider, IUriProvider {
	}
	#endregion
#if !SL && !DXPORTABLE
	#region FileBasedUriProvider
	[ComVisible(true)]
	[Obsolete("Please use the DevExpress.Office.Services.Implementation.FileBasedUriProvider instead", true)]
	public class FileBasedUriProvider : DevExpress.Office.Services.Implementation.FileBasedUriProvider, IUriProvider {
	}
	#endregion
#endif
}
