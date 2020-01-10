namespace StorAmp.Core.Model.Account
{
   public class ActionGroup
   {
      public ActionGroup(string name, params IConnectedAccountAction[] actions)
      {
         Name = name;
         Actions = actions;
      }

      public string Name { get; }
      public IConnectedAccountAction[] Actions { get; }
   }

}
