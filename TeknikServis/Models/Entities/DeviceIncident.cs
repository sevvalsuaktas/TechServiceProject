namespace TechService.Models.Entities
{
    public class DeviceIncident
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string DeviceModel { get; set; }
        public string IssueDescription { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public byte[]? DeviceImage { get; set; }

        //Arızanın atandığı personeli tutacağımız Foreign Key (İlişki)
        public int? AssignedUserId { get; set; }
        public AppUser? AssignedUser { get; set; }
    }
}