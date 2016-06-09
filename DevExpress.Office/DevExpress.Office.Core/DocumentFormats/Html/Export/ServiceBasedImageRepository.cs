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
using DevExpress.Utils;
using DevExpress.Office.Services;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System;
namespace DevExpress.Office.Export.Html {
	#region ServiceBasedImageRepository
	public class ServiceBasedImageRepository : IOfficeImageRepository {
		readonly IServiceProvider serviceProvider;
		readonly string rootUri;
		readonly string relativeUri;
		public ServiceBasedImageRepository(IServiceProvider serviceProvider, string rootUri, string relativeUri) {
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			this.serviceProvider = serviceProvider;
			this.rootUri = rootUri;
			this.relativeUri = relativeUri;
		}
		#region IOfficeImageRepository Members
		public string GetImageSource(OfficeImage image, bool autoDisposeImage) {
			string result = String.Empty;
			IUriProviderService service = (IUriProviderService)serviceProvider.GetService(typeof(IUriProviderService));
			if (service != null)
				result = service.CreateImageUri(rootUri, image, relativeUri);
			if (autoDisposeImage)
				image.Dispose();
			return result;
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
	}
	#endregion
}
