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

namespace DevExpress.Xpf.Map.Native {
	public static class DXMapStrings {
		public const string MsgIncorrectBingKey = "The specified Bing Maps key is invalid.\nTo create a developer account, refer to\nhttp://www.microsoft.com/maps/developers";
		public const string MsgUnsupportedDbfFIleFormat = "Unsupported Dbf file format.";
		public const string MsgIncorrectDbfFIleFormat = "Invalid Dbf file.";
		public const string MsgIncorrectShpFIleFormat = "Invalid Shp file.";
		public const string MsgDisagreeDbfFIle = "The Dbf file does not match the Shp file.";
		public const string MsgIncorrectShpFIleUri = "Incorrect Uri of the Shp file.";
		public const string MsgIncorrectGeoPointStringFormat = "The specified string format is incorrect.";
		public const string MsgIncorrectGeoPointCollectionStringFormat = "The specified string format is incorrect.";
		public const string MsgIncorrectGeoPointLatitude = "The specified latitude is incorrect. It should be greater than or equal to -90 and less than or equal to 90.";
		public const string MsgIncorrectMapProjection = "The image tiles projection is not compatible with the map projection.";
		public const string MsgIncorrectUtmZone = "The specified UTM Zone is incorrect. It should be greater than or equal to 1 and less than or equal to 60.";
		public const string MsgIncorrectItemSize = "Item size should be greater than 0.";
		public const string MsgIncorrectItemMinMaxSize = "Item maximum size should be greater than or equal to its minimal size.";
		public const string LatitudeDataMemberCaption = "Latitude Data Member";
		public const string LatitudeDataMemberDescription = "This field value will be assigned to a map item latitude.";
		public const string LongitudeDataMemberCaption = "Longitude Data Member";
		public const string LongitudeDataMemberDescription = "This field value will be assigned to a map item longitude.";
		public const string ItemIdDataMemberCaption = "Item Id Data Member";
		public const string ItemIdDataMemberDescription = "This field value specifies a data member that used to generate chart items.";
		public const string BubbleValueDataMemberCaption = "Value Data Member";
		public const string BubbleValueDataMemberDescription = "This field value will be assigned to a bubble chart value.";
		public const string SegmentIdDataMemberCaption = "Segment Id Data Member";
		public const string SegmentIdDataMemberDescription = "This field value specifies a data member that used to generate pie segments.";
		public const string SegmentValueDataMemberCaption = "Segment Value Data Member";
		public const string SegmentValueDataMemberDescription = "This field value will be assigned to a map pie segment value.";
	}
}
