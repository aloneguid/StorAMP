namespace StorAmp.Core.Model.Account
{

   class LocalDiskAccount : BlobApplicationAccount
   {
      public LocalDiskAccount() : base(
         "disk",
         "Local Disk",
         new AccountField("path", "Path"))
      {
      }
   }

   class AzureFilesAccount : BlobApplicationAccount
   {
      public AzureFilesAccount() : base(
         "azure.file",
         "Azure Files",
         new AccountField("account", "Name"),
         new AccountField("key", "Key"))
      {

      }
   }

   class InMemoryAccount : BlobApplicationAccount
   {
      public InMemoryAccount() : base(
         "inmemory",
         "In-Memory")
      {

      }
   }

   class AzdbfsAccount : BlobApplicationAccount
   {
      public AzdbfsAccount() : base(
         "azure.databricks.dbfs",
         "Azure Databricks DBFS",
         new AccountField("baseUri", "Cluster Base URI"),
         new AccountField("token", "Access Token"))
      {
      }
   }

   class AzureDataLakeGen1Account : BlobApplicationAccount
   {
      public AzureDataLakeGen1Account() : base(
         "azure.datalake.gen1",
         "Azure Data Lake (Gen 1)",
         new AccountField("account", "Account Name"),
         new AccountField("tenantId", "Tenant Id"),
         new AccountField("principalId", "Principal ID"),
         new SensitiveAccountField("principalSecret", "Principal Secret"))
      {

      }
   }

   class GoogleStorageAccount : BlobApplicationAccount
   {
      public GoogleStorageAccount() : base(
         "google.storage",
         "Google Storage",
         new AccountField("bucket", "Bucket Name"),
         new TextAreaAccountField("cred", "Credentials JSON", true))
      {

      }
   }

   class AzureAppInsightsWorker : AccountDefinition
   {
      public AzureAppInsightsWorker() : base(
         "azure.appinsights",
         "Azure Application Insights",
         new AccountField("appId", "Application ID"),
         new AccountField("apiKey", "API Key"))
      {
         Type = "ai";
      }
   }

   // I think storage queues should go into a separate app
   class AzureStorageQueuesAccount : AccountDefinition
   {
      public AzureStorageQueuesAccount() : base(
         "azure.queue",
         "Azure Storage Queue",
         new AccountField("account", "Name"),
         new AccountField("key", "Key"),
         new AccountField("queue", "Queue Name")
         )
      {
         Type = "queue";
      }
   }

   class AzureDocumentDbAccount : AccountDefinition
   {
      public AzureDocumentDbAccount() : base(
         "azdocdb",
         "azure.docdb",
         "Azure Cosmos DB (SQL API)",
         new AccountConnectionType(
            "ek",
            "Endpoint and Key",
            new AccountField("url", "Account Endpoint"),
            new AccountField("key", "Account Key")))
      {

      }
   }

}
