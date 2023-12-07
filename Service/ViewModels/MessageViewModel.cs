namespace Service.ViewModels
{
    public partial class MessageViewModel
    {
        public int Id { get; set; }
        public int? SenderUserId { get; set; }
        public int? ReceiverUserId { get; set; }
        public string? Content { get; set; }
        public DateTime? CreateTime { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsArchived { get; set; }
    }
}
