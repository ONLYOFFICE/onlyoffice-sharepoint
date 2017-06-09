namespace Onlyoffice
{
    public class Payload
    {
        public Payload() { }

        public Payload(string SPListItemId, string Folder, string SPListURLDir, string action)
        {
            this.SPListItemId = SPListItemId;
            this.Folder = Folder;
            this.SPListDir = SPListURLDir;
            this.action = action;
        }

        public string SPListItemId { get; set; }
        public string Folder { get; set; }
        public string SPListDir { get; set; }
        public string action { get; set; }
    }
}
