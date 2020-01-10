using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Serilog;
using Storage.Net;
using Storage.Net.Blobs;
using Storage.Net.Microsoft.Azure.Storage.Blobs;
using StorAmp.Core.Model;
using StorAmp.Core.Model.Account;
using StorAmp.Core.Services;

namespace StorAmp.Core.ViewModel.Blobs.Definitions
{
   class AzureBlobStorageAccount : BlobApplicationAccount
   {
      public AzureBlobStorageAccount() : base(
         "blob",
         "azure.blob",
         "Azure Blob Storage",
         new AccountConnectionType("sk", "Key",
            new AccountField(KnownParameter.AccountName, "Name"),
            new AccountField(KnownParameter.KeyOrPassword, "Key")),
         new AccountConnectionType("sas", "Shared Access Signature",
            new AccountField("url", "SAS URL")),
         new AccountConnectionType("emu", "Local Emulator",
            new HiddenAccountField(KnownParameter.IsLocalEmulator, null)))
      {
         AddColumnView(new ContainerColumnView());
         AddColumnView(new AnyColumnView());

         AddActionGroup(new ActionGroup("Containers",
            new SetContainerPublicAccessLevelAction()));

         AddActionGroup(new ActionGroup("SAS",
            new GetBlobPublicUrlAction()));

         AddActionGroup(new ActionGroup("Lease",
            new AcquireLeaseAction(),
            new BreakLeaseAction()));
      }

      private static string GetLeaseString(Blob b)
      {
         bool hasLeaseStatus = b.TryGetProperty("LeaseStatus", out LeaseStatus leaseStatus);
         string leaseStatusString = leaseStatus switch { LeaseStatus.Locked => "🔑", LeaseStatus.Unlocked => "🔓", _ => null };

         bool hasLeaseDuration = b.TryGetProperty("LeaseDuration", out LeaseDurationType leaseDuration);
         string leaseDurationString = hasLeaseDuration
            ? leaseDuration switch { LeaseDurationType.Fixed => "⌛", LeaseDurationType.Infinite => "∞", _ => "?" }
            : null;

         bool hasLeaseState = b.TryGetProperty("LeaseState", out LeaseState leaseState);

         if(!hasLeaseStatus && !hasLeaseDuration && !hasLeaseState)
            return null;


         return $"{leaseStatusString} {leaseDurationString} {FirstLastLetter(leaseState)}";
      }

      private static string FirstLastLetter(LeaseState ls)
      {
         string s = ls.ToString();
         return (s.Substring(0, 1) + s.Substring(s.Length - 1)).ToUpper();
      }

      class AnyColumnView : ExtraColumnView
      {
         public AnyColumnView() : base("Any",
            new ExtraColumn("Type", null, b =>
            {
               return b.TryGetProperty("BlobType", out BlobType blobType)
                  ? blobType.ToString().ToLower()
                  : null;
            }),
            new ExtraColumn("Lease", Strings.LeaseHint, GetLeaseString),
            new ExtraColumn("Access Tier", Strings.AccessTierHint, b =>
            {
               if(b.TryGetProperty("AccessTier", out AccessTier accessTier))
               {
                  b.TryGetProperty("AccessTierInferred", out bool accessTierInferred);

                  string s = accessTier.ToString().ToLower();
                  if(accessTierInferred)
                     s += "?";

                  return s;
               }

               return null;
            })
            )
         {

         }

         public override bool IsMatch(Blob anyBlob)
         {
            return true;
         }
      }

      class ContainerColumnView : ExtraColumnView
      {
         public ContainerColumnView() : base("Containers",
            new ExtraColumn("Public Access", null, b =>
            {
               b.Properties.TryGetValue("PublicAccess", out object value);
               return value?.ToString();
            }),
            new ExtraColumn("Lease", Strings.LeaseHint, GetLeaseString))
         {

         }

         public override bool IsMatch(Blob anyBlob)
         {
            return anyBlob != null && anyBlob.Properties.ContainsKey("IsContainer");
         }
      }
   }


   class SetContainerPublicAccessLevelAction : ConnectedAccountAction
   {
      public override string Name => "Set public Access Level";

      public override Symbol Icon => Symbol.Mail;

      public override object[] ArgumentRange => new object[]
      {
         ContainerPublicAccessType.Blob,
         ContainerPublicAccessType.Container,
         ContainerPublicAccessType.Off
      };

      public override bool CanExecute(IBlobStorage storage, Blob blob) => blob.Properties.ContainsKey("IsContainer");

      public override async Task ExecuteAsync(IBlobStorage storage, Blob blob, object accessLevelObject)
      {
         IDialogService dlg = ServiceLocator.GetInstance<IDialogService>();

         var level = (ContainerPublicAccessType)accessLevelObject;
         IAzureBlobStorage azureBlobStorage = (IAzureBlobStorage)storage;

         EventLog.LogEvent("setContainerPublicAccess", "access for {container} set to {level}", blob.FullPath, level);

         await azureBlobStorage.SetContainerPublicAccessAsync(blob.Name, level);

         await dlg.ShowMessageAsync("Access Level", $"Access level for container '{blob.Name}' was set to '{level}'");
      }
   }

   class GetBlobPublicUrlAction : ConnectedAccountAction
   {
      public override string Name => "Get public download URL for 1 day";

      public override Symbol Icon => Symbol.Link;

      public override bool CanExecute(IBlobStorage storage, Blob blob) => blob.IsFile;

      public override async Task ExecuteAsync(IBlobStorage storage, Blob blob, object arg)
      {
         var azb = (IAzureBlobStorage)storage;

         string url = await azb.GetBlobSasAsync(blob,
            new BlobSasPolicy(DateTime.UtcNow, TimeSpan.FromDays(1)) { Permissions = BlobSasPermission.Read });

         IDialogService dlg = ServiceLocator.GetInstance<IDialogService>();
         ISystemService sys = ServiceLocator.GetInstance<ISystemService>();

         sys.SetClipboardText(url);

         EventLog.LogEvent("get1hourBlob", "got sas for {path}", blob.FullPath);

         await dlg.ShowMessageAsync(
            "Public URL",
            $"'{url}' is copied to clipboard.");
      }
   }

   class AcquireLeaseAction : ConnectedAccountAction
   {
      private static readonly Dictionary<string, TimeSpan?> _nameToTimeSpan = new Dictionary<string, TimeSpan?>
      {
         ["Infinite"] = null,
         ["15 seconds (min)"] = TimeSpan.FromSeconds(15),
         ["30 seconds"] = TimeSpan.FromSeconds(30),
         ["1 minute (max)"] = TimeSpan.FromMinutes(1)
      };

      public override string Name => "Acquire Lease";

      public override Symbol Icon => Symbol.SetLockScreen;

      public override object[] ArgumentRange => _nameToTimeSpan.Keys.ToArray();

      public override bool CanExecute(IBlobStorage storage, Blob blob) =>
         blob.Properties.TryGetValue("LeaseStatus", out object s) && s != "Locked";

      public override bool CanExecute(ConnectedAccount connectedAccount) => true;

      public override async Task ExecuteAsync(IBlobStorage storage, Blob blob, object timeNameString)
      {
         var azb = (IAzureBlobStorage)storage;

         TimeSpan? leaseTime = _nameToTimeSpan[(string)timeNameString];

         EventLog.LogEvent("acquireLease", "setting lease for {blob} for {time}", blob.FullPath, timeNameString);

         try
         {
            await azb.AcquireLeaseAsync(blob.FullPath, leaseTime);

            await ServiceLocator.Dialogs.ShowMessageAsync("Lease", $"Acquired for '{timeNameString}'");
         }
         catch(Exception ex)
         {
            await ServiceLocator.Dialogs.ShowMessageAsync("Lease", "Failed to aquire: " + ex.Message);
         }
      }
   }

   class BreakLeaseAction : ConnectedAccountAction
   {
      public override string Name => "Break Lease";

      public override Symbol Icon => Symbol.DisconnectDrive;

      public override bool CanExecute(IBlobStorage storage, Blob blob) =>
         blob.Properties.TryGetValue("LeaseStatus", out object s) && s == "Locked";

      public override async Task ExecuteAsync(IBlobStorage storage, Blob blob, object arg)
      {
         var azb = (IAzureBlobStorage)storage;

         EventLog.LogEvent("breakLease", "breaking {blob}", blob.FullPath);


         try
         {
            await azb.BreakLeaseAsync(blob);


            await ServiceLocator.Dialogs.ShowMessageAsync("Lease", $"Broken successfully.");
         }
         catch(Exception ex)
         {
            await ServiceLocator.Dialogs.ShowMessageAsync("Lease", "Failed to break: " + ex.Message);
         }
      }
   }
}
