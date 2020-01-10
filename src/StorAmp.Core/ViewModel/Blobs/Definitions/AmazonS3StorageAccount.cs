using System;
using System.Collections.Generic;
using System.Linq;
using Amazon;
using Storage.Net;
using Storage.Net.Amazon.Aws;
using Storage.Net.Blobs;
using StorAmp.Core.Model.Account;

namespace StorAmp.Core.ViewModel.Blobs.Definitions
{
   class AmazonS3StorageAccount : BlobApplicationAccount
   {
      public AmazonS3StorageAccount() : base(
         "blob",
         "aws.s3",
         "Amazon S3",
         new AccountConnectionType("ks", "Key and Secret",
            new AccountField(KnownParameter.KeyId, "Access Key ID"),
            new AccountField(KnownParameter.KeyOrPassword, "Secret"),
            new AccountField(KnownParameter.BucketName, "Bucket Name"),
            new DropDownAccountField("region", "Region", GetAllRegionEndpoints())),
         new AccountConnectionType("awscli", "AWS CLI Profile",
            new DropDownAccountField(KnownParameter.LocalProfileName, "Profile",
               string.Join(",", GetProfileNames())),
            new AccountField(KnownParameter.BucketName, "Bucket Name"),
            new DropDownAccountField(KnownParameter.Region, "Region", GetAllRegionEndpoints()))
         )
      {
         AddColumnView(new AnyColumnView());
      }

      class AnyColumnView : ExtraColumnView
      {
         public AnyColumnView() : base("Any",
            new ExtraColumn("Storage Class", null, b =>
            {
               b.Properties.TryGetValue("StorageClass", out object value);
               return value?.ToString().ToLower();
            }))
         {

         }

         public override bool IsMatch(Blob anyBlob)
         {
            return true;
         }
      }

      private static string GetAllRegionEndpoints()
      {
         return string.Join(",", RegionEndpoint.EnumerableAllRegions.Select(e => e.SystemName));
      }

      private static IReadOnlyCollection<string> GetProfileNames()
      {
         try
         {
            return AwsCliCredentials.EnumerateProfiles();
         }
         catch
         {
            return new List<string>();
         }
      }
   }

}
