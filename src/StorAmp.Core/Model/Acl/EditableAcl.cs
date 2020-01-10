using System.ComponentModel;
using GalaSoft.MvvmLight;
using Storage.Net.Microsoft.Azure.Storage.Blobs.Gen2.Model;

namespace StorAmp.Core.Model.Acl
{
   public class EditableAcl : ViewModelBase
   {
      public EditableAcl(AccessControl acl)
      {
         OwnerUser = new EditableAclEntry(acl.OwningUserPermissions);
         OwnerGroup = new EditableAclEntry(acl.OwningGroupPermissions);
         OwnerUser.Identity = acl.OwnerUserId;
         OwnerGroup.Identity = acl.OwnerGroupId;

         foreach(AclEntry acle in acl.Acl)
         {
            Acl.Add(new EditableAclEntry(acle));
         }

         foreach(AclEntry acle in acl.DefaultAcl)
         {
            DefaultAcl.Add(new EditableAclEntry(acle));
         }
      }

      private EditableAclEntry _ownerUser;
      public EditableAclEntry OwnerUser
      {
         get => _ownerUser;
         set { Set(() => OwnerUser, ref _ownerUser, value); }
      }


      private EditableAclEntry _ownerGroup;
      public EditableAclEntry OwnerGroup
      {
         get => _ownerGroup;
         set { Set(() => OwnerGroup, ref _ownerGroup, value); }
      }

      public BindingList<EditableAclEntry> Acl { get; set; } = new BindingList<EditableAclEntry>();

      public BindingList<EditableAclEntry> DefaultAcl { get; set; } = new BindingList<EditableAclEntry>();
   }
}
