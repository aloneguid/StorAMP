using Storage.Net;

namespace StorAmp.Core
{
   public static class GlobalInit
   {
      public static void Initialise()
      {
         StorageFactory.Modules.UseAzureBlobStorage();
         StorageFactory.Modules.UseAzureFilesStorage();
         StorageFactory.Modules.UseAwsStorage();
         StorageFactory.Modules.UseAzureDataLake();
         StorageFactory.Modules.UseFtpStorage();
         StorageFactory.Modules.UseAzureKeyVault();
         StorageFactory.Modules.UseAzureDatabricksDbfsStorage();
         StorageFactory.Modules.UseGoogleCloudStorage();

         StorageFactory.Modules.UseAzureEventHubs();
         StorageFactory.Modules.UseAzureQueues();
      }
   }
}
