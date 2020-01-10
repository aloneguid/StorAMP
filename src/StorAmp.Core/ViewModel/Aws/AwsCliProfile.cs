using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using GalaSoft.MvvmLight.Command;
using NetBox.Extensions;
using Storage.Net.Amazon.Aws;
using StorAmp.Core.Model;
using StorAmp.Core.Services;
using IDialogService = StorAmp.Core.Services.IDialogService;

namespace StorAmp.Core.ViewModel.Aws
{
   public class AwsCliProfile : HierarchicalResource
   {
      public AwsCliProfile(string name, AWSCredentials nativeCredentials) : base(name, "cloud-aws-profile")
      {
         CommandGroups.Add(new HierarchicalResourceCommandGroup(
            new HierarchicalResourceCommand("Discover resources", Symbol.Refresh, RefreshCommand),
            new HierarchicalResourceCommand("Properties", Symbol.Setting, PropertiesCommand)));

         NativeCredentials = nativeCredentials;

         DiscoverAsync().Forget();
      }

      public AWSCredentials NativeCredentials { get; }

      private async Task DiscoverAsync()
      {
         var client = new AmazonS3Client(NativeCredentials, RegionEndpoint.USEast1);

         ListBucketsResponse allBuckets = await client.ListBucketsAsync();

         Children.Clear();

         await Task.WhenAll(allBuckets.Buckets.Select(b => DiscoverAsync(client, b)));
      }

      private async Task DiscoverAsync(AmazonS3Client client, S3Bucket bucket)
      {
         GetBucketLocationResponse bucketLocation = await client.GetBucketLocationAsync(bucket.BucketName);

         string location = bucketLocation.Location.Value;

         Children.Add(new AwsS3Bucket(bucket.BucketName, DisplayName, location));
      }

      private RelayCommand _refreshCommand;
      public RelayCommand RefreshCommand
      {
         get
         {
            return _refreshCommand
                ?? (_refreshCommand = new RelayCommand(
                () =>
                {
                   DiscoverAsync().Forget();
                }));
         }
      }

      private RelayCommand _PropertiesCommand;
      public RelayCommand PropertiesCommand => _PropertiesCommand
                ?? (_PropertiesCommand = new RelayCommand(
                () =>
                {
                   IDialogService dialogService = ServiceLocator.GetInstance<IDialogService>();

                   var props = new Dictionary<string, object>(
                      AwsCliCredentials.GetRawCredentials(DisplayName).ToDictionary(p => p.Key, p => (object)p.Value));
                   props["Profile Name"] = DisplayName;

                   dialogService.ShowPropertiesAsync("Properties", props).Forget();
                }
                ));

   }
}
